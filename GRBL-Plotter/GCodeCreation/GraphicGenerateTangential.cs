/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2022-04-07 add DragToolModificationTangential, to preset path for tangential knife with offset in knife-tip
 * 2022-04-13 add drag-tool, option knife (expect knife-angle = 0 on path-start, leave path with 0 deg.)
 * 2023-05-19 l:440 f:DragToolModification bug fix #340: rotate knife if path does not start with 0° degree
 * 2024-04-07 l:484 f:InsertArcMove set fix value for stepwidth to avoid out of memory
 * 2024-04-17 rotary cutter - avoid overcut
*/

using System;
using System.Collections.Generic;
using System.Windows;

namespace GrblPlotter
{
    public static partial class Graphic
    {
        /* Calculate angle of each path segment, to be applied at tangential axis */
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
                    {
                        Point pStart = item.Path[0].MoveTo;
                        Point pEnd = item.Path[1].MoveTo;
                        double angleNow = GcodeMath.GetAlpha(pStart, pEnd);
                        graphicItem.StartAngle = angleNow;
                    }
                    else
                    { graphicItem.StartAngle = 0; }
                }
            }
        }

        /* 1st drag tool 2nd tangential axis */
        public static void CalculateTangentialAxis()
        {
            const uint loggerSelect = (uint)LogEnables.PathModification;
            double maxAngleChangeDeg = (double)Properties.Settings.Default.importGCTangentialAngle;
            bool pathShorteningEnable = Properties.Settings.Default.importGCTangentialShorteningEnable;
            double pathShortening = (double)Properties.Settings.Default.importGCTangentialShortening;

            double maxAngleRad = maxAngleChangeDeg * Math.PI / 180;     // in RAD
            double angleNow = 0, angleLast, angleOffset, angleApply, angleLastApply;
            finalPathList = new List<PathObject>();    // figures of one tile
            ItemPath tempPath;
            Point pStart, pEnd;
            Logger.Trace("...CalculateTangentialAxis maxAngle:{0}", maxAngleChangeDeg);
            //   GcodeMath.ResetAngles();

            bool showLog = ((logFlags & loggerSelect) > 0);

            //    ListGraphicObjects(completeGraphic, true);

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
                    {
                        actualDashArray = new double[item.DashArray.Length];
                        item.DashArray.CopyTo(actualDashArray, 0);
                    }

                    tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // create new path
                    tempPath.Info.CopyData(graphicItem.Info);                   // preset global info for GROUP																			
                    if (actualDashArray.Length > 0)                             // set dash array
                    {
                        tempPath.DashArray = new double[actualDashArray.Length];
                        actualDashArray.CopyTo(tempPath.DashArray, 0);
                    }

                    for (int i = 1; i < item.Path.Count; i++)                   // go through path objects
                    {
                        pStart = item.Path[i - 1].MoveTo;
                        pEnd = item.Path[i].MoveTo;

                        /* Process Line */
                        if (item.Path[i] is GCodeLine)
                        {
                            angleNow = GcodeMath.GetAlpha(pStart, pEnd);      // angle-i = p[i-1] to p[i] in radiant
                            if ((angleNow - angleLast) < -Math.PI)
                                angleNow += 2 * Math.PI;
                            else if ((angleNow - angleLast) > Math.PI)
                                angleNow -= 2 * Math.PI;

                            if (i == 1)
                            {
                                angleLast = item.Path[0].Angle = angleNow;
                                tempPath.StartAngle = angleNow;
                                angleLastApply = angleApply = angleNow;
                            }

                            if (showLog) Logger.Trace("{0}    Line before angleNow:{1:0.00}  angleLast:{2:0.00} angleApply:{3:0.00}  offset:{4:0.00}", i, (angleNow * 180 / Math.PI), (angleLast * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset * 180 / Math.PI));

                            double diff = angleNow - angleLast;	// + angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = angleNow + angleOffset;

                            if (showLog) Logger.Trace("     Line after  angleNow:{0:0.00}  angleLast:{1:0.00} angleApply:{2:0.00}  offset:{3:0.00}", (angleNow * 180 / Math.PI), (angleLast * 180 / Math.PI), (angleApply * 180 / Math.PI), (angleOffset * 180 / Math.PI));

                            item.Path[i].Angle = angleApply;

                            if (showLog) Logger.Trace("    GCodeLine angleLastApply:{0:0.00}  angleApply:{1:0.00}   maxAngleRad:{2:0.00}", (angleLastApply * 180 / Math.PI), (angleApply * 180 / Math.PI), (maxAngleRad * 180 / Math.PI));
                            if (showLog) Logger.Trace("    GCodeLine (Math.Abs(angleLastApply - angleApply) > maxAngleRad):{0}  FixAngleExceed(ref angleApply, ref angleOffset):{1}   ", (Math.Abs(angleLastApply - angleApply) > maxAngleRad), FixAngleExceed(ref angleApply, ref angleOffset));

                            /* split path if swivel angle is reached*/
                            if ((Math.Abs(angleLastApply - angleApply) > maxAngleRad) || FixAngleExceed(ref angleApply, ref angleOffset))         // change in angle is too large -> insert pen up/turn/down -> seperate path
                            {
                                if (tempPath.Path.Count > 0)
                                {
                                    finalPathList.Add(tempPath.Copy());                // save prev path, start new path to force pen up/turn/down
                                    if (showLog) Logger.Trace("    GCodeLine finalPathList.Add(tempPath)");
                                }
                                if (showLog) Logger.Trace("      Exceed angle max:{0:0.00}  actual:{1:0.00}", (maxAngleRad * 180 / Math.PI), (Math.Abs(angleLast - angleNow) * 180 / Math.PI));

                                tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // start new path with clipped start position
                                tempPath.Info.CopyData(graphicItem.Info);                   // preset global info for GROUP
                                if (actualDashArray.Length > 0)                             // set dash array
                                {
                                    tempPath.DashArray = new double[actualDashArray.Length];
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
                            bool isCW = ((GCodeArc)item.Path[i]).IsCW;
                            if (!isCW) { offset = -offset; }                                          // angleStart-i = center to p[i-1] + 90°

                            Point center = new Point(pStart.X + ((GCodeArc)item.Path[i]).CenterIJ.X, pStart.Y + ((GCodeArc)item.Path[i]).CenterIJ.Y);

                            /* Start angle */
                            double angleArcStart = GcodeMath.GetAlpha(pStart, center) + offset;   // angle of tangente (isCW?2:3)
                            if ((angleArcStart - angleLast) < -Math.PI)
                                angleArcStart += 2 * Math.PI;
                            else if ((angleArcStart - angleLast) > Math.PI)
                                angleArcStart -= 2 * Math.PI;

                            if (i == 1)
                            {
                                angleLast = item.Path[0].Angle = angleArcStart;
                                tempPath.StartAngle = angleArcStart;
                            }

                            double diff = angleArcStart - angleLast;	// + angleOffset;
                            if (diff > Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -Math.PI) { angleOffset += 2 * Math.PI; }
                            angleArcStart = angleArcStart + angleOffset;

                            ((GCodeArc)item.Path[i]).AngleStart = angleApply;
                        //    Logger.Trace("=== AngleStart {0:0.0}       angleArcStart:{1:0.00}         angleLast:{2:0.00}     angleOffset:{3:0.00}    isCW:{4}", angleApply * 180 / Math.PI, angleArcStart * 180 / Math.PI, angleLast * 180 / Math.PI, angleOffset * 180 / Math.PI, isCW);

                            /* End angle */
                            double angleArcEnd = GcodeMath.GetAlpha(pEnd, center) + offset;
                            if (isCW && (angleArcEnd > angleArcStart))          // CW aEnd must be < aStart
                                while (isCW && (angleArcEnd > angleArcStart))
                                    angleArcEnd -= 2 * Math.PI;
                            else if (!isCW && (angleArcEnd < angleArcStart))    // CCW aEnd must be > aStart
                                while (!isCW && (angleArcEnd < angleArcStart))
                                    angleArcEnd += 2 * Math.PI;

                            diff = angleArcEnd - angleArcStart;//angleLast;	 //+ angleOffset;
                            if (diff > 2*Math.PI) { angleOffset -= 2 * Math.PI; }
                            else if (diff < -2*Math.PI) { angleOffset += 2 * Math.PI; }
                            angleApply = angleArcEnd;// + angleOffset;

                            ((GCodeArc)item.Path[i]).Angle = angleApply;
                        //    Logger.Trace("=== Angle     {0:0.0}          angleArcEnd:{1:0.00}        angleLast:{2:0.00}     angleOffset:{3:0.00}    diff:{4:0.00}", angleApply * 180 / Math.PI, angleArcEnd * 180 / Math.PI, angleLast * 180 / Math.PI, angleOffset * 180 / Math.PI, diff);

                            if (showLog) Logger.Trace("{0}    Tangential Circle X:{1:0.00} Y:{2:0.00}  end X:{3:0.00} Y:{4:0.00}  angleStart:{5:0.00} angleEnd:{6:0.00}  angleLast:{7:0.00}", i, pStart.X, pStart.Y, pEnd.X, pEnd.Y, (angleArcStart * 180 / Math.PI), (angleArcEnd * 180 / Math.PI), (angleLast * 180 / Math.PI));
                            ////////////////////
                            //               if (Math.Abs(angleLast - angleArcEnd) > maxAngleRad)               // change in angle is too large -> insert pen up/turn/down -> seperate path
                         
                            
                            if (Math.Abs(angleLastApply - angleArcStart) > maxAngleRad)               // change in angle is too large -> insert pen up/turn/down -> seperate path
                            {
                                if (tempPath.Path.Count > 0)
                                {
                                    finalPathList.Add(tempPath);           // save prev path, start new path to force pen up/turn/down
                                    if (showLog) Logger.Trace("    Tangential finalPathList.Add");
                                }
                                tempPath = new ItemPath(new Point(pStart.X, pStart.Y));     // start new path with clipped start position
                                tempPath.Info.CopyData(graphicItem.Info);                   // preset global info for GROUP
                                if (actualDashArray.Length > 0)
                                {
                                    tempPath.DashArray = new double[actualDashArray.Length];
                                    actualDashArray.CopyTo(tempPath.DashArray, 0);
                                }
                                tempPath.StartAngle = angleArcStart;
                                tempPath.AddArc((GCodeArc)item.Path[i], item.Path[i].Depth, angleArcStart, angleApply);
                                if (showLog) Logger.Trace("a    Tangential tempPath.AddArc  aStart:{0:0.0}  aEnd:{1:0.0}", angleArcStart * 180 / Math.PI, angleApply * 180 / Math.PI);
                            }////////////////////////////
                            else
                            {
                                tempPath.AddArc((GCodeArc)item.Path[i], item.Path[i].Depth, angleArcStart, angleApply);
                                if (showLog) Logger.Trace("b    Tangential tempPath.AddArc  aStart:{0:0.0}  aEnd:{1:0.0}", angleArcStart * 180 / Math.PI, angleApply * 180 / Math.PI);
                            }                // add point and angle
                            angleLastApply = angleNow = angleApply;
                        }
                        angleLast = angleNow;
                    }
                    if (tempPath.Path.Count > 0)
                    {
                        finalPathList.Add(tempPath.Copy());                       // save prev path
                        if (showLog) Logger.Trace("    Tangential finalPathList.Add(tempPath)");
                    }
                }
                else
                /* Process Dot */
                {
                    ItemDot dot = new ItemDot(graphicItem.Start.X, graphicItem.Start.Y);
                    dot.Info.CopyData(graphicItem.Info);              // preset global info for GROUP
                    finalPathList.Add(dot.Copy());
                    if ((logFlags & loggerSelect) > 0) Logger.Trace("   Clip Dot copied");
                }
            }
            completeGraphic.Clear();
            if (pathShorteningEnable)
            {
                ExtendClosedPaths(finalPathList, pathShortening, true);
            }
            foreach (PathObject item in finalPathList)
                completeGraphic.Add(item);
            //   ListGraphicObjects(completeGraphic, true);
        }

        private static bool FixAngleExceed(ref double angleApply, ref double angleOffset)
        {
            if (Properties.Settings.Default.importGCTangentialRange)
            {
                if (angleApply < 0)
                { angleApply += 2 * Math.PI; angleOffset += 2 * Math.PI; return true; }
                else if (angleApply > (2 * Math.PI))
                { angleApply -= 2 * Math.PI; angleOffset -= 2 * Math.PI; return true; }
            }
            return false;
        }

        /* 1st drag tool 2nd tangential axis */
        internal static void DragToolModificationTangential(List<PathObject> graphicToModify)
        {
            const uint loggerSelect = (uint)LogEnables.PathModification;
            double gcodeDragRadius = (double)Properties.Settings.Default.importGCDragKnifeLength;
            double gcodeDragAngle = (double)Properties.Settings.Default.importGCDragKnifeAngle;
            finalPathList = new List<PathObject>();
            ItemPath tempPath;

            Logger.Info("...DragToolModificationTangential radius:{0:0.00} angle:{1:0.00}", gcodeDragRadius, gcodeDragAngle);

            if (graphicToModify == null) return;

            ListGraphicObjects(graphicToModify, true);

            foreach (PathObject item in graphicToModify)      // replace original list?
            {
                if (item is ItemPath opath)               // if Dot: nothing to correct
                {
                    Point origMoveTo, newMoveTo, shorten;
                    double a, a1, a2;

                    if (opath.Path.Count > 1)
                    {
                        origMoveTo = newMoveTo = opath.Path[0].MoveTo;
                        gcodeDragRadius = GetDragRadius(opath.Path[0]);     //  2020-08-03
                        /* First move starts earlier */
                        newMoveTo = ExtendPath(opath.Path[0].MoveTo, opath.Path[1].MoveTo, gcodeDragRadius, ExtendDragPath.startLater);
                        opath.Start = newMoveTo;

                        tempPath = new ItemPath(new Point(newMoveTo.X, newMoveTo.Y));     // create new path
                        tempPath.Info.CopyData(item.Info);                                  // preset global info for GROUP																			
                        if (actualDashArray.Length > 0)                                     // set dash array
                        {
                            tempPath.DashArray = new double[actualDashArray.Length];
                            actualDashArray.CopyTo(tempPath.DashArray, 0);
                        }

                        for (int i = 1; i < (opath.Path.Count); i++)                   // go through path objects
                        {
                            if (IsEqual(opath.Path[i - 1].MoveTo, opath.Path[i].MoveTo, 0.001))
                            {
                                Logger.Info(" Error same coord - remove: i:{0}  Drag (i-1) x:{1:0.00000} y:{2:0.00000} (i) x:{3:0.00000} y:{4:0.00000}  ", i, opath.Path[i - 1].MoveTo.X, opath.Path[i - 1].MoveTo.Y, opath.Path[i].MoveTo.X, opath.Path[i].MoveTo.Y);
                                opath.Path.RemoveAt(i);
                                continue;
                            }

                            origMoveTo = opath.Path[i].MoveTo;
                            gcodeDragRadius = GetDragRadius(opath.Path[i]);     //  2020-08-03

                            /* Each move ends later */
                            newMoveTo = ExtendPath(opath.Path[i - 1].MoveTo, opath.Path[i].MoveTo, gcodeDragRadius, ExtendDragPath.endLater);

                            if ((i < opath.Path.Count - 1))
                            {
                                a1 = GcodeMath.GetAlpha(opath.Path[i - 1].MoveTo, origMoveTo);
                                a2 = GcodeMath.GetAlpha(origMoveTo, opath.Path[i + 1].MoveTo);
                                a = a2 - a1;
                                if (a > Math.PI) { a -= 2 * Math.PI; }
                                if (a < -Math.PI) { a += 2 * Math.PI; }
                                if ((logFlags & loggerSelect) > 0) Logger.Trace(" Drag (i-1) x:{0:0.00} y:{1:0.00} (i) x:{2:0.00} y:{3:0.00}  (i+1) x:{4:0.00} y:{5:0.00} a1:{6:0.00} a2:{7:0.00}", opath.Path[i - 1].MoveTo.X, opath.Path[i - 1].MoveTo.Y, opath.Path[i].MoveTo.X, opath.Path[i].MoveTo.Y, opath.Path[i + 1].MoveTo.X, opath.Path[i + 1].MoveTo.Y, (a1 * 180 / Math.PI), (a2 * 180 / Math.PI));

                                /* if change in angle is too much, stop path and start new path to force pen-up /-down for tangential knife */
                                if ((Math.Abs(a) * 180 / Math.PI) > gcodeDragAngle)
                                {
                                    if ((logFlags & loggerSelect) > 0) Logger.Trace("   Abs angle exceeds max a:{0:0.00} > max:{1:0.00}", (a * 180 / Math.PI), gcodeDragAngle);
                                    shorten = ExtendPath(origMoveTo, opath.Path[i + 1].MoveTo, gcodeDragRadius, ExtendDragPath.startLater);


                                    tempPath.Add(newMoveTo, opath.Path[i].Depth, 0); // finish old, extended path
                                    if (tempPath.Path.Count > 1)
                                        finalPathList.Add(tempPath);                // save prev path, start new path to force pen up/turn/down

                                    tempPath = new ItemPath(new Point(shorten.X, shorten.Y));   // start new path with clipped start position
                                    tempPath.Info.CopyData(item.Info);                          // preset global info for GROUP
                                    if (actualDashArray.Length > 0)                             // set dash array
                                    {
                                        tempPath.DashArray = new double[actualDashArray.Length];
                                        actualDashArray.CopyTo(tempPath.DashArray, 0);
                                    }
                                }
                                else
                                { tempPath.Add(newMoveTo, opath.Path[i].Depth, 0); }     // finish old, extended path
                            }
                        }	// for (i
                        tempPath.Add(newMoveTo, opath.Path[opath.Path.Count - 1].Depth, 0);
                        if (tempPath.Path.Count > 1)
                            finalPathList.Add(tempPath);                       // save prev path
                    }	// if (opath.Path.Count				
                }	// if (item is ItemPath
            }	// foreach (PathObject
            completeGraphic.Clear();
            foreach (PathObject item in finalPathList)     // add tile to full graphic
                completeGraphic.Add(item);
        }


        internal static void DragToolModification(List<PathObject> graphicToModify)
        {
            const uint loggerSelect = (uint)LogEnables.PathModification;
            double gcodeDragRadius = (double)Properties.Settings.Default.importGCDragKnifeLength;
            double gcodeDragAngle = (double)Properties.Settings.Default.importGCDragKnifeAngle;
            bool useKnife = Properties.Settings.Default.importGCDragKnifeUse;

            Logger.Info("...DragToolModification radius:{0:0.00} angle:{1:0.00}", gcodeDragRadius, gcodeDragAngle);

            if (graphicToModify == null) return;

            ListGraphicObjects(graphicToModify, true);

            /* 2022-04-13 assume start-angle = 0 and leave path at 0 deg. for a KNIFE */

            foreach (PathObject item in graphicToModify)
            {
                if (item is ItemPath opath)               // if Dot: nothing to correct
                {
                    Point origMoveTo, lastOrigMoveTo;
                    if (opath.Path.Count > 1)
                    {
                        origMoveTo = opath.Path[0].MoveTo;
                        gcodeDragRadius = GetDragRadius(opath.Path[0]);     //  2020-08-03

                        if ((logFlags & loggerSelect) > 0) Logger.Trace(" New path count:{0}", opath.Path.Count);

                        /* First move starts earlier ? */
                        if (!useKnife)
                        {
                            opath.Path[0].MoveTo = ExtendPath(opath.Path[0].MoveTo, opath.Path[1].MoveTo, gcodeDragRadius, ExtendDragPath.startEarlier);
                            if ((logFlags & loggerSelect) > 0) Logger.Trace(" not useKnife extend first move old x:{0:0.00} y:{1:0:00}  new x:{2:0.00} y:{3:0.00}", opath.Start.X, opath.Start.Y, opath.Path[0].MoveTo.X, opath.Path[0].MoveTo.Y);
                            opath.Start = opath.Path[0].MoveTo;
                        }
                        else
                        /* Find path-segment with direction ca. 0 deg. and set it as 1st point, to avoid adjustment of knife */
                        {
                            int lastIndex = 0;
                            double lastAlpha = Math.PI;
                            for (int i = 0; i < opath.Path.Count - 1; i++)
                            {
                                Point pStart = opath.Path[i].MoveTo;
                                Point pEnd = opath.Path[i + 1].MoveTo;
                                double angleNow = GcodeMath.GetAlpha(pStart, pEnd);         // in radiant
                                if (Math.Abs(angleNow) < Math.Abs(lastAlpha))
                                {
                                    lastAlpha = angleNow;
                                    lastIndex = i;
                                }
                            }
                            if (lastIndex != 0)
                            {
                                opath.TmpIndex = lastIndex;
                                RotatePath(opath);
                            }
                            origMoveTo = opath.Path[0].MoveTo;
                            gcodeDragRadius = GetDragRadius(opath.Path[0]);     //  2020-08-03
                        }

                        /*Go from path-end to path-start for easier path-insertion*/
                        lastOrigMoveTo = opath.Path[opath.Path.Count - 1].MoveTo;
                        for (int i = opath.Path.Count - 1; i > 0; i--)
                        {
                            if (IsEqual(opath.Path[i - 1].MoveTo, opath.Path[i].MoveTo, 0.001))
                            {
                                Logger.Info(" Error same coord - remove: i:{0}  Drag (i-1) x:{1:0.00000} y:{2:0.00000} (i) x:{3:0.00000} y:{4:0.00000}  ", i, opath.Path[i - 1].MoveTo.X, opath.Path[i - 1].MoveTo.Y, opath.Path[i].MoveTo.X, opath.Path[i].MoveTo.Y);
                                opath.Path.RemoveAt(i);
                                continue;
                            }

                            origMoveTo = opath.Path[i].MoveTo;
                            gcodeDragRadius = GetDragRadius(opath.Path[i]);     //  2020-08-03

                            /* Each move ends later */
                            opath.Path[i].MoveTo = ExtendPath(opath.Path[i - 1].MoveTo, opath.Path[i].MoveTo, gcodeDragRadius, ExtendDragPath.endLater);
                            if (i == opath.Path.Count - 1)
                            {   /* last move */
                                if (useKnife)   // leave path at 0 deg.
                                {
                                    /* add arc to leave path with knife angle = 0° degree */
                                    Point leavePath = new Point(origMoveTo.X + gcodeDragRadius, origMoveTo.Y);
                                    if ((logFlags & loggerSelect) > 0) Logger.Trace("CheckArcMove useKnife last-element correct leave angle");
                                    CheckArcMove(opath, i, opath.Path[i - 1].MoveTo, origMoveTo, leavePath, gcodeDragRadius, 1);    // on path end, move knife to the right
                                }
                            }
                            else
                            {   /* add arc if needed befoe next segment starts */
                                CheckArcMove(opath, i, opath.Path[i - 1].MoveTo, origMoveTo, lastOrigMoveTo, gcodeDragRadius, gcodeDragAngle);
                            }

                            lastOrigMoveTo = origMoveTo;
                        }   // end for

                        opath.End = opath.Path[opath.Path.Count - 1].MoveTo;
                        if (useKnife)
                        {   /* knife has angle of 0° - and 1st path? #340 */
                            Point startPathOriginal = new Point(opath.Path[0].MoveTo.X, opath.Path[0].MoveTo.Y);
                            Point startPath0Degree = new Point(opath.Path[0].MoveTo.X - gcodeDragRadius, opath.Path[0].MoveTo.Y);

                            if ((logFlags & loggerSelect) > 0) Logger.Trace("CheckArcMove useKnife first-element extend path");

                            /* Calculate extended path endpoint of virtual horizontal movement */
                            opath.Path[0].MoveTo = ExtendPath(startPath0Degree, startPathOriginal, gcodeDragRadius, ExtendDragPath.endLater);
                            /* add rotation if needed for 1st real move */
                            CheckArcMove(opath, 0, startPath0Degree, startPathOriginal, opath.Path[1].MoveTo, gcodeDragRadius, 1); // on path start, expect knife to the right

                            //original 1.6.9.4 { opath.Path[0].MoveTo = ExtendPath(opath.Path[0].MoveTo, opath.Path[1].MoveTo, gcodeDragRadius, ExtendDragPath.startLater); }
                            opath.Start = opath.Path[0].MoveTo;
                        }
                    }
                }
            }
        }
        private static bool CheckArcMove(ItemPath opath, int pos, Point pLast, Point pNow, Point pNext, double gcodeDragRadius, double limitAngle)
        {
            const uint loggerSelect = (uint)LogEnables.PathModification;
            double a1 = GcodeMath.GetAlpha(pLast, pNow);
            double a2 = GcodeMath.GetAlpha(pNow, pNext);
            double a = a2 - a1;
            if (a > Math.PI) { a -= 2 * Math.PI; }
            if (a < -Math.PI) { a += 2 * Math.PI; }
            if ((logFlags & loggerSelect) > 0) Logger.Trace(" Drag {0}) (i-1) x:{1:0.00} y:{2:0.00} (i) x:{3:0.00} y:{4:0.00}  (i+1) x:{5:0.00} y:{6:0.00} a1:{7:0.00} a2:{8:0.00}  a:{9:0.00}  limit:{10:0.00}", pos, pLast.X, pLast.Y, opath.Path[pos].MoveTo.X, opath.Path[pos].MoveTo.Y, pNext.X, pNext.Y, (a1 * 180 / Math.PI), (a2 * 180 / Math.PI), (a * 180 / Math.PI), limitAngle);

            /* if change in angle is too much, insert curve, to match next move angle */
            if ((Math.Abs(a) * 180 / Math.PI) > limitAngle)
            {
                if ((logFlags & loggerSelect) > 0) Logger.Trace("   Abs angle exceeds max a:{0:0.00} > max:{1:0.00}", (a * 180 / Math.PI), limitAngle);
                Point shorten = ExtendPath(pNow, pNext, gcodeDragRadius, ExtendDragPath.startLater);
                Point center = (Point)Point.Subtract(pNow, opath.Path[pos].MoveTo);

                /* insert arc as line segments to avoid problems with other options 2020-08-03*/
                InsertArcMove(opath, pos, opath.Path[pos].MoveTo, shorten, center, Math.Sign(a) < 0, opath.Path[pos].Depth);
                return true;
            }
            return false;
        }

        /* inserts intermediate arc-move into existing path at index 'pos'
			from oldPoint to endPoint at center in direction isCW */
        private static void InsertArcMove(ItemPath item, int pos, Point oldPoint, Point endPoint, Point center, bool isCW, double depth)
        {
            GCodeMotion motion;
            ArcProperties arcMove;
            Point p1 = Round(oldPoint);
            Point p2 = Round(endPoint);
            double x, y;
            arcMove = GcodeMath.GetArcMoveProperties(p1, p2, center, isCW);
            double stepwidth = arcMove.radius / 6; //(double)Properties.Settings.Default.importGCSegment;

            int insertCounter = 1;

            //    if (stepwidth > arcMove.radius / 2)
            //    { stepwidth = arcMove.radius / 5; }
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
                    item.Path.Insert(pos + insertCounter, motion);
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
        {
            if (!Properties.Settings.Default.importGCDragKnifePercentEnable)
                return (double)Properties.Settings.Default.importGCDragKnifeLength;
            else
            {
                double useZ = (double)Properties.Settings.Default.importGCZDown;
                if (graphicInformation.OptionZFromWidth)
                { useZ = Graphic2GCode.CalculateZFromRange(graphicInformation.PenWidthMin, graphicInformation.PenWidthMax, entity.Depth); }
                return Math.Abs(useZ * (double)Properties.Settings.Default.importGCDragKnifePercent / 100);
            }
        }

        private enum ExtendDragPath { startEarlier, startLater, endEarlier, endLater };
        private static Point ExtendPath(Point start, Point end, double gcodeDragRadius, ExtendDragPath extend)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double moveLength = (float)Math.Sqrt(dx * dx + dy * dy);
            if (moveLength == 0)
                return end;
            if (extend == ExtendDragPath.endLater)
            {
                double newx = end.X + gcodeDragRadius * dx / moveLength;
                double newy = end.Y + gcodeDragRadius * dy / moveLength;
                return new Point(newx, newy);
            }
            if (extend == ExtendDragPath.endEarlier)
            {
                double newx = end.X - gcodeDragRadius * dx / moveLength;
                double newy = end.Y - gcodeDragRadius * dy / moveLength;
                return new Point(newx, newy);
            }
            else if (extend == ExtendDragPath.startLater)
            {
                double newx = start.X + gcodeDragRadius * dx / moveLength;
                double newy = start.Y + gcodeDragRadius * dy / moveLength;
                return new Point(newx, newy);
            }
            else if (extend == ExtendDragPath.startEarlier)
            {
                double newx = start.X - gcodeDragRadius * dx / moveLength;
                double newy = start.Y - gcodeDragRadius * dy / moveLength;
                return new Point(newx, newy);
            }
            return end;
        }
    }
}
