/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Collections.Generic;
using System.Windows;

namespace GRBL_Plotter
{
    public static partial class Graphic
    {

        public static void CalculateTangentialAxis()
        { const uint loggerSelect = (uint)LogEnable.PathModification;
            double maxAngleChangeDeg = (double)Properties.Settings.Default.importGCTangentialAngle;
            bool limitRange = Properties.Settings.Default.importGCTangentialRange;

            double maxAngleRad = maxAngleChangeDeg * Math.PI / 180;     // in RAD
            double angleNow, angleLast, angleOffset, angleApply, angleLastApply;
            finalPathList = new List<PathObject>();    // figures of one tile
            ItemPath tempPath = new ItemPath();
            Point pStart, pEnd;
            Logger.Trace("...CalculateTangentialAxis maxAngle:{0}", maxAngleChangeDeg);
            gcodeMath.resetAngles();

            foreach (PathObject graphicItem in completeGraphic)
            {
                angleLast = angleApply = angleLastApply = 0;
                angleOffset = 0;
                if (!(graphicItem is ItemDot))
                {
                    ItemPath item = (ItemPath)graphicItem;

                    if (item.path.Count == 0)
                        continue;

                    pStart = item.path[0].MoveTo;

                    /* copy general info */
                    actualDashArray = new double[0];
                    if (item.dashArray.Length > 0)
                    { actualDashArray = new double[item.dashArray.Length];
                        item.dashArray.CopyTo(actualDashArray, 0);
                    }

                    tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // create new path
                    tempPath.Info.CopyData(graphicItem.Info);                   // preset global info for GROUP																			
                    if (actualDashArray.Length > 0)                             // set dash array
                    { tempPath.dashArray = new double[actualDashArray.Length];
                        actualDashArray.CopyTo(tempPath.dashArray, 0);
                    }

                    for (int i = 1; i < item.path.Count; i++)                   // go through path objects
                    {
                        pStart = item.path[i - 1].MoveTo;
                        pEnd = item.path[i].MoveTo;

                        /* Process Line */
                        if (item.path[i] is GCodeLine)
                        { angleNow = gcodeMath.getAlpha(pStart, pEnd);      // angle-i = p[i-1] to p[i] in radiant

                            //                            if ((loggerTrace & loggerSelect) > 0) Logger.Trace("    TangentialAxis diff:{0:0.00} now:{1:0.00}  last:{2:0.00} offfset:{3:0.00}  ", diff, angleNow,angleLast, angleOffset);

                            if (i == 1)
                            { angleLast = item.path[0].Angle = angleNow;
                                tempPath.StartAngle = angleNow;
                                angleLastApply = angleApply = angleNow;
                            }

                            if ((logFlags & loggerSelect) > 0) Logger.Trace("    before angleNow:{0:0.00}  angleLast:{1:0.00} angleApply:{2:0.00}  offset:{3:0.00}", (angleNow * 180 / Math.PI), (angleLast * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset * 180 / Math.PI));

                            double diff = angleNow - angleLast;	// + angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = angleNow + angleOffset;

                            if ((logFlags & loggerSelect) > 0) Logger.Trace("    after  angleNow:{0:0.00}  angleLast:{1:0.00} angleApply:{2:0.00}  offset:{3:0.00}", (angleNow * 180 / Math.PI), (angleLast * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset * 180 / Math.PI));

                            item.path[i].Angle = angleApply;
                            //		if ((loggerTrace & loggerSelect) > 0) Logger.Trace("    TangentialAxis Line   X:{0:0.00} Y:{1:0.00}  end X:{2:0.00} Y:{3:0.00}  angleNow:{4:0.00}  angleApply:{5:0.00}  offset:{6:0.00}", pStart.X, pStart.Y, pEnd.X, pEnd.Y, (angleNow * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset* 180 / Math.PI));

                            /* split path if swivel angle is reached*/
                            if ((Math.Abs(angleLastApply - angleApply) > maxAngleRad) || fixAngleExceed(ref angleApply, ref angleNow, ref angleOffset))         // change in angle is too large -> insert pen up/turn/down -> seperate path
                            { if (tempPath.path.Count > 1)
                                    finalPathList.Add(tempPath);                // save prev path, start new path to force pen up/turn/down
                                if ((logFlags & loggerSelect) > 0) Logger.Trace("      Exceed angle max:{0:0.00}  actual:{1:0.00}", (maxAngleRad * 180 / Math.PI), (Math.Abs(angleLast - angleNow) * 180 / Math.PI));

                                tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // start new path with clipped start position
                                tempPath.Info.CopyData(graphicItem.Info);                   // preset global info for GROUP
                                if (actualDashArray.Length > 0)                             // set dash array
                                { tempPath.dashArray = new double[actualDashArray.Length];
                                    actualDashArray.CopyTo(tempPath.dashArray, 0);
                                }
                                tempPath.Add(pEnd, item.path[i].Depth, angleApply);
                                tempPath.StartAngle = angleApply;
                            }
                            else
                            { tempPath.Add(pEnd, item.path[i].Depth, angleApply); }     // add point and angle

                            angleLast = angleApply; //angleNow;
                            angleLastApply = angleApply;
                        }
                        else
                        /* Process Arc   implement fixAngleExceed(ref angleApply, ref angleNow, ref angleOffset)?*/
                        {
                            double offset = +Math.PI / 2;        // angle-i = center to p[i] + 90° it is the tangente
                            double aStart = 0;
                            if (!((GCodeArc)item.path[i]).IsCW) { offset = -offset; }                                          // angleStart-i = center to p[i-1] + 90°

                            Point center = new Point(pStart.X + ((GCodeArc)item.path[i]).CenterIJ.X, pStart.Y + ((GCodeArc)item.path[i]).CenterIJ.Y);

                            aStart = gcodeMath.getAngle(pStart, center, offset, 0);	// angle of tangente
                            if (i == 1)
                            { angleLast = item.path[0].Angle = aStart;
                                tempPath.StartAngle = aStart;
                            }

                            double diff = aStart - angleLast;	// + angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = aStart + angleOffset;

                            //             if (loggerTrace) Logger.Trace(" Tang Circle a-start:{0:0.00} a-end:{1:0.00}  ", (angle * 180 / Math.PI));
                            ((GCodeArc)item.path[i]).AngleStart = angleApply;

                            angleNow = gcodeMath.getAngle(pEnd, center, offset, 0);
                            diff = angleNow - angleLast;	 //+ angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = angleNow + angleOffset;

                            ((GCodeArc)item.path[i]).Angle = angleApply;
                            if ((logFlags & loggerSelect) > 0) Logger.Trace("    Tangential Circle X:{0:0.00} Y:{1:0.00}  end X:{2:0.00} Y:{3:0.00}  angleStart:{4:0.00} angleEnd:{5:0.00}", pStart.X, pStart.Y, pEnd.X, pEnd.Y, (aStart * 180 / Math.PI), (angleNow * 180 / Math.PI));

                            if (Math.Abs(angleLast - angleNow) > maxAngleRad)                     // change in angle is too large -> insert pen up/turn/down -> seperate path
                            { if (tempPath.path.Count > 1)
                                    finalPathList.Add(tempPath);           // save prev path, start new path to force pen up/turn/down

                                tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // start new path with clipped start position
                                tempPath.Info.CopyData(graphicItem.Info);                // preset global info for GROUP
                                if (actualDashArray.Length > 0)
                                { tempPath.dashArray = new double[actualDashArray.Length];
                                    actualDashArray.CopyTo(tempPath.dashArray, 0);
                                }
                            }
                            else
                            { tempPath.AddArc((GCodeArc)item.path[i], aStart, angleApply, item.path[i].Depth); }                // add point and angle
                        }
                        angleLast = angleNow;
                    }
                    if (tempPath.path.Count > 1)
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

        private static bool fixAngleExceed(ref double angleApply, ref double angleNow, ref double angleOffset)
        { if (Properties.Settings.Default.importGCTangentialRange)
            { if (angleApply < 0)
                { angleApply += 2 * Math.PI; angleOffset += 2 * Math.PI; return true; }
                else if (angleApply > (2 * Math.PI))
                { angleApply -= 2 * Math.PI; angleOffset -= 2 * Math.PI; return true; }
            }
            return false;
        }


        public static void DragToolModification(List<PathObject> graphicToModify)
        { const uint loggerSelect = (uint)LogEnable.PathModification;
            double gcodeDragRadius = (double)Properties.Settings.Default.importGCDragKnifeLength;
     /*       if (Properties.Settings.Default.importGCDragKnifePercentEnable)
            {   gcodeDragRadius = Math.Abs((double)Properties.Settings.Default.importGCZDown * (double)Properties.Settings.Default.importGCDragKnifePercent / 100); }
*/
            double gcodeDragAngle = (double)Properties.Settings.Default.importGCDragKnifeAngle;

            Logger.Trace("...DragToolModification radius:{0:0.00} angle:{1:0.00}", gcodeDragRadius, gcodeDragAngle);
            ListGraphicObjects(graphicToModify, true);

            List<PathObject> modifiedGraphic = new List<PathObject>();

            foreach (PathObject item in graphicToModify)      // replace original list?
            {
                if (item is ItemPath)               // if Dot: nothing to correct
                {
                    Point origMoveTo, center, shorten;
                    double a, a1, a2;
                    ItemPath opath = (ItemPath)item;
                    if (opath.path.Count > 1)
                    {
                        origMoveTo = opath.path[0].MoveTo;
                        gcodeDragRadius = getDragRadius(opath.path[0]);     //  2020-08-03
                        opath.path[0].MoveTo = extendPath(opath.path[0].MoveTo, opath.path[1].MoveTo, gcodeDragRadius, ExtendDragPath.startEarlier);
                        opath.Start = opath.path[0].MoveTo;

/*Go from path-end to path-start for easier path-insertion*/
                        for (int i = opath.path.Count - 1; i > 0; i--)
                        {
                            if (isEqual(opath.path[i - 1].MoveTo, opath.path[i].MoveTo))
                            { if ((logFlags & loggerSelect) > 0) Logger.Trace(" Error same coord: Drag (i-1) x:{0:0.00} y:{1:0.00} (i) x:{2:0.00} y:{3:0.00}  ", opath.path[i - 1].MoveTo.X, opath.path[i - 1].MoveTo.Y, opath.path[i].MoveTo.X, opath.path[i].MoveTo.Y);
                                continue;
                            }

                            origMoveTo = opath.path[i].MoveTo;
                            gcodeDragRadius = getDragRadius(opath.path[i]);     //  2020-08-03
                            opath.path[i].MoveTo = extendPath(opath.path[i - 1].MoveTo, opath.path[i].MoveTo, gcodeDragRadius, ExtendDragPath.endLater);
                            if (i < opath.path.Count - 1)
                            {
                                a1 = gcodeMath.getAlpha(opath.path[i - 1].MoveTo, origMoveTo);
                                a2 = gcodeMath.getAlpha(origMoveTo, opath.path[i + 1].MoveTo);
                                a = a2 - a1;
                                if (a > Math.PI) { a = a - 2 * Math.PI; }
                                if (a < -Math.PI) { a = 2 * Math.PI + a; }
                                if ((logFlags & loggerSelect) > 0) Logger.Trace(" Drag (i-1) x:{0:0.00} y:{1:0.00} (i) x:{2:0.00} y:{3:0.00}  (i+1) x:{4:0.00} y:{5:0.00} a1:{6:0.00} a2:{7:0.00}", opath.path[i - 1].MoveTo.X, opath.path[i - 1].MoveTo.Y, opath.path[i].MoveTo.X, opath.path[i].MoveTo.Y, opath.path[i + 1].MoveTo.X, opath.path[i + 1].MoveTo.Y, (a1 * 180 / Math.PI), (a2 * 180 / Math.PI));
                                /* if change in angle is too much, insert extra path */
                                if ((Math.Abs(a) * 180 / Math.PI) > gcodeDragAngle)
                                { if ((logFlags & loggerSelect) > 0) Logger.Trace("   Abs angle exceeds max a:{0:0.00} > max:{1:0.00}", (a * 180 / Math.PI), gcodeDragAngle);
                                    shorten = extendPath(origMoveTo, opath.path[i + 1].MoveTo, gcodeDragRadius, ExtendDragPath.startLater);
                                    center = (Point)Point.Subtract(origMoveTo, opath.path[i].MoveTo);

/* insert arc as line segments to avoid problems with other options 2020-08-03*/
                                    insertArcMove(opath, i, opath.path[i].MoveTo, shorten, center, Math.Sign(a) < 0, opath.path[i].Depth);
                             //       GCodeMotion tmpArc = new GCodeArc(shorten, center, Math.Sign(a) < 0, opath.path[i].Depth);
                             //       opath.path.Insert(i + 1, tmpArc);
                                }
                            }
                        }
                        opath.End = opath.path[opath.path.Count - 1].MoveTo;
                    }
                }
            }
        }
        private static  void insertArcMove(ItemPath item, int pos, Point oldPoint, Point endPoint, Point center, bool isCW, double depth)
        {
            GCodeMotion motion;
            ArcProperties arcMove;
            Point p1 = Round(oldPoint);
            Point p2 = Round(endPoint);
            double x = 0, y = 0;
            arcMove = gcodeMath.getArcMoveProperties(p1, p2, center, isCW);
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
                    item.Dimension.setDimensionXY(x, y);
                    item.path.Insert(pos+ insertCounter, motion);
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
                    item.Dimension.setDimensionXY(x, y);
                    item.path.Insert(pos + insertCounter, motion);
                    insertCounter++;
                    item.PathLength += PointDistance(oldPoint, endPoint);    // distance from last to current point
                    oldPoint = endPoint;
                }
            }
            motion = new GCodeLine(new Point(endPoint.X, endPoint.Y), depth);
            item.Dimension.setDimensionXY(endPoint.X, endPoint.Y);
            item.path.Insert(pos + insertCounter, motion);
            item.PathLength += PointDistance(oldPoint, endPoint);    // distance from last to current point
            oldPoint = endPoint;
        }
        private static Point Round(Point tmp, int decimals = 4)
        { return new Point(Math.Round(tmp.X, decimals), Math.Round(tmp.Y, decimals)); }

        private static double PointDistance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }


        private static double getDragRadius(GCodeMotion entity)
        {   if (!Properties.Settings.Default.importGCDragKnifePercentEnable)
                return (double)Properties.Settings.Default.importGCDragKnifeLength;
            else
            {
                double useZ = (double)Properties.Settings.Default.importGCZDown;
                if (graphicInformation.OptionZFromWidth)
                { useZ = Graphic2GCode.calculateZFromRange(graphicInformation.PenWidthMin, graphicInformation.PenWidthMax, entity.Depth); }
                return Math.Abs(useZ * (double)Properties.Settings.Default.importGCDragKnifePercent / 100); }
        }

        private enum ExtendDragPath { startEarlier, startLater, endLater };
        private static Point extendPath(Point start, Point end, double gcodeDragRadius, ExtendDragPath extend)
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
