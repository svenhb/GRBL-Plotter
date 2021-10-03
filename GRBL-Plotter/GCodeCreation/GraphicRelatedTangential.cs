/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * 2020-07-05 new
 * 2020-08-03 Drag-Tool radius = % from Z-depth -> use path.depth if graphicInformation.OptionZFromWidth is enabled
 * 2021-07-02 code clean up / code quality
*/

using System;
using System.Collections.Generic;
using System.Windows;

//#pragma warning disable CA1303	// Do not pass literals as localized parameters
//#pragma warning disable CA1305

namespace GrblPlotter
{
    public static partial class Graphic
    {
        public static void CalculateStartAngle()
        {
            foreach (PathObject graphicItem in completeGraphic)
            {
                if (!(graphicItem is ItemDot))
                {
                    ItemPath item = (ItemPath)graphicItem;

                    if (item.Path.Count == 0)
                        break;

                    if (item.Path.Count > 1)
                    {   Point pStart = item.Path[0].MoveTo;
                        Point pEnd = item.Path[1].MoveTo;
                        double angleNow = GcodeMath.GetAlpha(pStart, pEnd);
                        graphicItem.StartAngle = angleNow;
                    }
                    else
                    {   graphicItem.StartAngle = 0; }
                }
            }
        }

        public static void CalculateTangentialAxis()
        {   const uint loggerSelect = (uint)LogEnables.PathModification;
            double maxAngleChangeDeg = (double)Properties.Settings.Default.importGCTangentialAngle;
          //  bool limitRange = Properties.Settings.Default.importGCTangentialRange;

            double maxAngleRad = maxAngleChangeDeg * Math.PI / 180;     // in RAD
            double angleNow, angleLast, angleOffset, angleApply, angleLastApply;
            finalPathList = new List<PathObject>();    // figures of one tile
            ItemPath tempPath;
            Point pStart, pEnd;
            Logger.Trace( "...CalculateTangentialAxis maxAngle:{0}", maxAngleChangeDeg);
            GcodeMath.ResetAngles();

            foreach (PathObject graphicItem in completeGraphic)
            {
                angleLast = angleApply = angleLastApply = 0;
                angleOffset = 0;
                if (!(graphicItem is ItemDot))
                {
                    ItemPath item = (ItemPath)graphicItem;

                    if (item.Path.Count == 0)
                        continue;

                    pStart = item.Path[0].MoveTo;

                    /* copy general info */
                    actualDashArray = new double[0];
                    if (item.DashArray.Length > 0)
                    { actualDashArray = new double[item.DashArray.Length];
                        item.DashArray.CopyTo(actualDashArray, 0);
                    }

                    tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // create new path
                    tempPath.Info.CopyData(graphicItem.Info);                   // preset global info for GROUP																			
                    if (actualDashArray.Length > 0)                             // set dash array
                    { tempPath.DashArray = new double[actualDashArray.Length];
                        actualDashArray.CopyTo(tempPath.DashArray, 0);
                    }

                    for (int i = 1; i < item.Path.Count; i++)                   // go through path objects
                    {
                        pStart = item.Path[i - 1].MoveTo;
                        pEnd = item.Path[i].MoveTo;

                        /* Process Line */
                        if (item.Path[i] is GCodeLine)
                        {   angleNow = GcodeMath.GetAlpha(pStart, pEnd);      // angle-i = p[i-1] to p[i] in radiant

                            //                            if ((loggerTrace & loggerSelect) > 0) Logger.Trace("    TangentialAxis diff:{0:0.00} now:{1:0.00}  last:{2:0.00} offfset:{3:0.00}  ", diff, angleNow,angleLast, angleOffset);

                            if (i == 1)
                            {   angleLast = item.Path[0].Angle = angleNow;
                                tempPath.StartAngle = angleNow;
                                angleLastApply = angleApply = angleNow;
                            }

                            if ((logFlags & loggerSelect) > 0) Logger.Trace( "    before angleNow:{0:0.00}  angleLast:{1:0.00} angleApply:{2:0.00}  offset:{3:0.00}", (angleNow * 180 / Math.PI), (angleLast * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset * 180 / Math.PI));

                            double diff = angleNow - angleLast;	// + angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = angleNow + angleOffset;

                            if ((logFlags & loggerSelect) > 0) Logger.Trace( "    after  angleNow:{0:0.00}  angleLast:{1:0.00} angleApply:{2:0.00}  offset:{3:0.00}", (angleNow * 180 / Math.PI), (angleLast * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset * 180 / Math.PI));

                            item.Path[i].Angle = angleApply;
                            //		if ((loggerTrace & loggerSelect) > 0) Logger.Trace("    TangentialAxis Line   X:{0:0.00} Y:{1:0.00}  end X:{2:0.00} Y:{3:0.00}  angleNow:{4:0.00}  angleApply:{5:0.00}  offset:{6:0.00}", pStart.X, pStart.Y, pEnd.X, pEnd.Y, (angleNow * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset* 180 / Math.PI));

                            /* split path if swivel angle is reached*/
                            if ((Math.Abs(angleLastApply - angleApply) > maxAngleRad) || FixAngleExceed(ref angleApply, ref angleOffset))         // change in angle is too large -> insert pen up/turn/down -> seperate path
                            {   if (tempPath.Path.Count > 1)
                                    finalPathList.Add(tempPath);                // save prev path, start new path to force pen up/turn/down
                                if ((logFlags & loggerSelect) > 0) Logger.Trace("      Exceed angle max:{0:0.00}  actual:{1:0.00}", (maxAngleRad * 180 / Math.PI), (Math.Abs(angleLast - angleNow) * 180 / Math.PI));

                                tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // start new path with clipped start position
                                tempPath.Info.CopyData(graphicItem.Info);                   // preset global info for GROUP
                                if (actualDashArray.Length > 0)                             // set dash array
                                { tempPath.DashArray = new double[actualDashArray.Length];
                                    actualDashArray.CopyTo(tempPath.DashArray, 0);
                                }
                                tempPath.Add(pEnd, item.Path[i].Depth, angleApply);
                                tempPath.StartAngle = angleApply;
                            }
                            else
                            { tempPath.Add(pEnd, item.Path[i].Depth, angleApply); }     // add point and angle

                            angleLast = angleApply; //angleNow;
                            angleLastApply = angleApply;
                        }
                        else
                        /* Process Arc   implement fixAngleExceed(ref angleApply, ref angleNow, ref angleOffset)?*/
                        {
                            double offset = +Math.PI / 2;        // angle-i = center to p[i] + 90° it is the tangente
                            double aStart = 0;
                            if (!((GCodeArc)item.Path[i]).IsCW) { offset = -offset; }                                          // angleStart-i = center to p[i-1] + 90°

                            Point center = new Point(pStart.X + ((GCodeArc)item.Path[i]).CenterIJ.X, pStart.Y + ((GCodeArc)item.Path[i]).CenterIJ.Y);

                            aStart = GcodeMath.GetAngle(pStart, center, offset, 0);	// angle of tangente
                            if (i == 1)
                            { angleLast = item.Path[0].Angle = aStart;
                                tempPath.StartAngle = aStart;
                            }

                            double diff = aStart - angleLast;	// + angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = aStart + angleOffset;

                            //             if (loggerTrace) Logger.Trace(" Tang Circle a-start:{0:0.00} a-end:{1:0.00}  ", (angle * 180 / Math.PI));
                            ((GCodeArc)item.Path[i]).AngleStart = angleApply;

                            angleNow = GcodeMath.GetAngle(pEnd, center, offset, 0);
                            diff = angleNow - angleLast;	 //+ angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = angleNow + angleOffset;

                            ((GCodeArc)item.Path[i]).Angle = angleApply;
                            if ((logFlags & loggerSelect) > 0) Logger.Trace( "    Tangential Circle X:{0:0.00} Y:{1:0.00}  end X:{2:0.00} Y:{3:0.00}  angleStart:{4:0.00} angleEnd:{5:0.00}", pStart.X, pStart.Y, pEnd.X, pEnd.Y, (aStart * 180 / Math.PI), (angleNow * 180 / Math.PI));

                            if (Math.Abs(angleLast - angleNow) > maxAngleRad)                     // change in angle is too large -> insert pen up/turn/down -> seperate path
                            { if (tempPath.Path.Count > 1)
                                    finalPathList.Add(tempPath);           // save prev path, start new path to force pen up/turn/down

                                tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // start new path with clipped start position
                                tempPath.Info.CopyData(graphicItem.Info);                // preset global info for GROUP
                                if (actualDashArray.Length > 0)
                                { tempPath.DashArray = new double[actualDashArray.Length];
                                    actualDashArray.CopyTo(tempPath.DashArray, 0);
                                }
                            }
                            else
                            { tempPath.AddArc((GCodeArc)item.Path[i], aStart, angleApply, item.Path[i].Depth); }                // add point and angle
                        }
                        angleLast = angleNow;
                    }
                    if (tempPath.Path.Count > 1)
                        finalPathList.Add(tempPath);                       // save prev path
                }
                else
                /* Process Dot */
                {
                    ItemDot dot = new ItemDot(graphicItem.Start.X, graphicItem.Start.Y);
                    dot.Info.CopyData(graphicItem.Info);              // preset global info for GROUP
                    finalPathList.Add(dot);
                    if ((logFlags & loggerSelect) > 0) Logger.Trace("   Clip Dot copied");
                }
            }
            completeGraphic.Clear();
            foreach (PathObject item in finalPathList)     // add tile to full graphic
                completeGraphic.Add(item);
        }

        private static bool FixAngleExceed(ref double angleApply, ref double angleOffset)
        {   if (Properties.Settings.Default.importGCTangentialRange)
            {   if (angleApply < 0)
                { angleApply += 2 * Math.PI; angleOffset += 2 * Math.PI; return true; }
                else if (angleApply > (2 * Math.PI))
                { angleApply -= 2 * Math.PI; angleOffset -= 2 * Math.PI; return true; }
            }
            return false;
        }


        internal static void DragToolModification(List<PathObject> graphicToModify)
        {   const uint loggerSelect = (uint)LogEnables.PathModification;
            double gcodeDragRadius = (double)Properties.Settings.Default.importGCDragKnifeLength;
     /*       if (Properties.Settings.Default.importGCDragKnifePercentEnable)
            {   gcodeDragRadius = Math.Abs((double)Properties.Settings.Default.importGCZDown * (double)Properties.Settings.Default.importGCDragKnifePercent / 100); }
*/
            double gcodeDragAngle = (double)Properties.Settings.Default.importGCDragKnifeAngle;

            Logger.Trace("...DragToolModification radius:{0:0.00} angle:{1:0.00}", gcodeDragRadius, gcodeDragAngle);

            if (graphicToModify == null) return;

            ListGraphicObjects(graphicToModify, true);

          //  List<PathObject> modifiedGraphic = new List<PathObject>();

            foreach (PathObject item in graphicToModify)      // replace original list?
            {
                if (item is ItemPath opath)               // if Dot: nothing to correct
                {
                    Point origMoveTo, center, shorten;
                    double a, a1, a2;
                    //ItemPath opath = (ItemPath)item;
                    if (opath.Path.Count > 1)
                    {
                        origMoveTo = opath.Path[0].MoveTo;
                        gcodeDragRadius = GetDragRadius(opath.Path[0]);     //  2020-08-03
                        opath.Path[0].MoveTo = ExtendPath(opath.Path[0].MoveTo, opath.Path[1].MoveTo, gcodeDragRadius, ExtendDragPath.startEarlier);
                        opath.Start = opath.Path[0].MoveTo;

/*Go from path-end to path-start for easier path-insertion*/
                        for (int i = opath.Path.Count - 1; i > 0; i--)
                        {
                            if (IsEqual(opath.Path[i - 1].MoveTo, opath.Path[i].MoveTo))
                            { if ((logFlags & loggerSelect) > 0) Logger.Trace( " Error same coord: Drag (i-1) x:{0:0.00} y:{1:0.00} (i) x:{2:0.00} y:{3:0.00}  ", opath.Path[i - 1].MoveTo.X, opath.Path[i - 1].MoveTo.Y, opath.Path[i].MoveTo.X, opath.Path[i].MoveTo.Y);
                                continue;
                            }

                            origMoveTo = opath.Path[i].MoveTo;
                            gcodeDragRadius = GetDragRadius(opath.Path[i]);     //  2020-08-03
                            opath.Path[i].MoveTo = ExtendPath(opath.Path[i - 1].MoveTo, opath.Path[i].MoveTo, gcodeDragRadius, ExtendDragPath.endLater);
                            if (i < opath.Path.Count - 1)
                            {
                                a1 = GcodeMath.GetAlpha(opath.Path[i - 1].MoveTo, origMoveTo);
                                a2 = GcodeMath.GetAlpha(origMoveTo, opath.Path[i + 1].MoveTo);
                                a = a2 - a1;
                                if (a > Math.PI) { a -= 2 * Math.PI; }
                                if (a < -Math.PI) { a += 2 * Math.PI; }
                                if ((logFlags & loggerSelect) > 0) Logger.Trace( " Drag (i-1) x:{0:0.00} y:{1:0.00} (i) x:{2:0.00} y:{3:0.00}  (i+1) x:{4:0.00} y:{5:0.00} a1:{6:0.00} a2:{7:0.00}", opath.Path[i - 1].MoveTo.X, opath.Path[i - 1].MoveTo.Y, opath.Path[i].MoveTo.X, opath.Path[i].MoveTo.Y, opath.Path[i + 1].MoveTo.X, opath.Path[i + 1].MoveTo.Y, (a1 * 180 / Math.PI), (a2 * 180 / Math.PI));
                                /* if change in angle is too much, insert extra path */
                                if ((Math.Abs(a) * 180 / Math.PI) > gcodeDragAngle)
                                { if ((logFlags & loggerSelect) > 0) Logger.Trace( "   Abs angle exceeds max a:{0:0.00} > max:{1:0.00}", (a * 180 / Math.PI), gcodeDragAngle);
                                    shorten = ExtendPath(origMoveTo, opath.Path[i + 1].MoveTo, gcodeDragRadius, ExtendDragPath.startLater);
                                    center = (Point)Point.Subtract(origMoveTo, opath.Path[i].MoveTo);

/* insert arc as line segments to avoid problems with other options 2020-08-03*/
                                    InsertArcMove(opath, i, opath.Path[i].MoveTo, shorten, center, Math.Sign(a) < 0, opath.Path[i].Depth);
                             //       GCodeMotion tmpArc = new GCodeArc(shorten, center, Math.Sign(a) < 0, opath.path[i].Depth);
                             //       opath.path.Insert(i + 1, tmpArc);
                                }
                            }
                        }
                        opath.End = opath.Path[opath.Path.Count - 1].MoveTo;
                    }
                }
            }
        }
        private static  void InsertArcMove(ItemPath item, int pos, Point oldPoint, Point endPoint, Point center, bool isCW, double depth)
        {
            GCodeMotion motion;
            ArcProperties arcMove;
            Point p1 = Round(oldPoint);
            Point p2 = Round(endPoint);
            double x, y;
            arcMove = GcodeMath.GetArcMoveProperties(p1, p2, center, isCW);
            double stepwidth = (double)Properties.Settings.Default.importGCSegment;

            int insertCounter = 1;

            if (stepwidth > arcMove.radius / 2)
            { stepwidth = arcMove.radius / 5; }
            double step = Math.Asin(stepwidth / arcMove.radius);     // in RAD
                                                                     //                    double step = Math.Asin((double)Properties.Settings.Default.importGCSegment / arcMove.radius);     // in RAD
            if (step > Math.Abs(arcMove.angleDiff))
                step = Math.Abs(arcMove.angleDiff / 2);

            if (arcMove.angleDiff > 0)   //(da > 0)                                             // if delta >0 go counter clock wise
            {
                for (double angle = (arcMove.angleStart + step); angle < (arcMove.angleStart + arcMove.angleDiff); angle += step)
                {
                    x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                    y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                    motion = new GCodeLine(new Point(x, y), depth);
                    item.Dimension.SetDimensionXY(x, y);
                    item.Path.Insert(pos+ insertCounter, motion);
                    insertCounter++;
                    item.PathLength += PointDistance(oldPoint, endPoint);    // distance from last to current point
                    oldPoint = endPoint;
                }
            }
            else                                                       // else go clock wise
            {
                for (double angle = (arcMove.angleStart - step); angle > (arcMove.angleStart + arcMove.angleDiff); angle -= step)
                {
                    x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                    y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                    motion = new GCodeLine(new Point(x, y), depth);
                    item.Dimension.SetDimensionXY(x, y);
                    item.Path.Insert(pos + insertCounter, motion);
                    insertCounter++;
                    item.PathLength += PointDistance(oldPoint, endPoint);    // distance from last to current point
                    oldPoint = endPoint;
                }
            }
            motion = new GCodeLine(new Point(endPoint.X, endPoint.Y), depth);
            item.Dimension.SetDimensionXY(endPoint.X, endPoint.Y);
            item.Path.Insert(pos + insertCounter, motion);
            item.PathLength += PointDistance(oldPoint, endPoint);    // distance from last to current point
         //   oldPoint = endPoint;
        }
        private static Point Round(Point tmp, int decimals = 4)
        { return new Point(Math.Round(tmp.X, decimals), Math.Round(tmp.Y, decimals)); }

        private static double PointDistance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }


        private static double GetDragRadius(GCodeMotion entity)
        {   if (!Properties.Settings.Default.importGCDragKnifePercentEnable)
                return (double)Properties.Settings.Default.importGCDragKnifeLength;
            else
            {
                double useZ = (double)Properties.Settings.Default.importGCZDown;
                if (graphicInformation.OptionZFromWidth)
                { useZ = Graphic2GCode.CalculateZFromRange(graphicInformation.PenWidthMin, graphicInformation.PenWidthMax, entity.Depth); }
                return Math.Abs(useZ * (double)Properties.Settings.Default.importGCDragKnifePercent / 100); }
        }

        private enum ExtendDragPath { startEarlier, startLater, endLater };
        private static Point ExtendPath(Point start, Point end, double gcodeDragRadius, ExtendDragPath extend)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double moveLength = (float)Math.Sqrt(dx * dx + dy * dy);
            if (moveLength == 0)
                return end;
            if (extend == ExtendDragPath.endLater)
            {   double newx = end.X + gcodeDragRadius * dx / moveLength;
                double newy = end.Y + gcodeDragRadius * dy / moveLength;
                return new Point(newx, newy);
            }
            else if (extend == ExtendDragPath.startLater)
            {   double newx = start.X + gcodeDragRadius * dx / moveLength;
                double newy = start.Y + gcodeDragRadius * dy / moveLength;
                return new Point(newx, newy);
            }
            else if (extend == ExtendDragPath.startEarlier)
            {   double newx = start.X - gcodeDragRadius * dx / moveLength;
                double newy = start.Y - gcodeDragRadius * dy / moveLength;
                return new Point(newx, newy);
            }
            return end;
        }
    }
}
