/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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
/*  GCodeVisuAndTransform.cs
*/
/* 2016-09-18 use gcode.frmtNum to control amount of decimal places
 * 2018-04-03 code clean up
 * 2019-01-12 add some comments to getGCodeLine
 * 2019-01-24 change lines 338, 345, 356, 363 to get xyz dimensions correctly
 * 2019-01-28 3 digits for dimension in getMinMaxString()
 * 2019-02-06 add selection high-light
 * 2019-02-09 outsourcing of code for parsing
 * 2019-02-27 createGCodeProg add output for A,B,C,U,V,W axis
 * 2019-03-05 Bug orgin-shift after mirroring
 * 2019-03-09 add pathRotaryInfo to show rotary over X or Y
 * 2019-05-24 add cutter radius compensation
 * 2019-06-05 edit marker-view, show end-point and center of arc
 * 2019-08-15 add logger
 * 2019-11-16 add calculation of process time
 * 2019-11-30 new line 913 offset via mouse move 
 * 2020-01-10 init path = pathPenUp in line 259
 * 2020-01-13 convert GCodeVisuAndTransform to a static class
 * 2020-07-24 pathBackground.Reset() after code-rotation, -scaling, -offset
 * 2020-07-27 clean line - replace ' by "  313, 327
 * 2020-08-13 bug fix transformGCodeRotate, transformGCodeScale with G91 
 * 2020-12-16 add markSelectedTile 
 * 2020-12-30 add N-Number
 * 2021-01-06 add G2/3 radius check (grbl error:33); createDrawingPathFromGCode: if ((aDiff < 0.1) && (arcMove.radius > 1000)) 
 *            use path.AddLine instead of path.AddArc to avoid float unprecision in drawing (https://github.com/arkypita/LaserGRBL/issues/1248)
 * 2021-01-12 extend drawingProperties
 * 2021-02-18 bug fix in addSubroutine, set lastSubroutine = new int[] { 0, 0, 0 };
 * 2021-04-01 #189 line 1620 seperate pen-up arrow-angle
 * 2021-04-13 #189 set initial positon for lastXYG123Position line 421
 * 2021-04-30 apply height-msp only if (!gcline.ismachineCoordG53 && gcline.isdistanceModeG90)   line 1660
 * 2021-07-27 code clean up / code quality
 *			  split code to GCodeAnalyze.cs, GCode2DViewPaths.cs
 * 2022-03-01 MarkSelectedGroup line 368 add try/catch for AddPath
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace GrblPlotter
{
    internal static partial class VisuGCode
    {
        internal static Dimensions xyzSize = new Dimensions();
        internal static Dimensions G0Size = new Dimensions();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal static bool logEnable = true;
        internal static bool logDetailed = false;
        internal static bool logCoordinates = false;

        // add given coordinates to drawing path
        private static int onlyZ = 0;
        private static int figureCount = 0;

        internal static string selectedFigureInfo = "";
        private static int lastFigureNumber = -1;
        private static readonly HashSet<int> lastFigureNumbers = new HashSet<int>();

        public static int GetHighlightStatus()
        { return lastFigureNumber; }

        private static XyPoint origWCOLandMark = new XyPoint();
        private static List<CoordByLine> coordListLandMark = new List<CoordByLine>();
        private static XyPoint origWCOMachineLimit = new XyPoint();

        public static string GetProcessingTime()
        {
            try
            {
                if (double.IsNaN(gcodeMinutes))
                    gcodeMinutes = 0.1;
                TimeSpan t = TimeSpan.FromSeconds(gcodeMinutes * 60);
                return string.Format("Est. time: {0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            }
            catch
            { return "Error"; }
        }

        public static bool ContainsG2G3Command()
        { return modal.containsG2G3; }
        public static bool ContainsG91Command()
        { return modal.containsG91; }
        public static bool ContainsTangential()
        { return tangentialAxisEnable; }


        /// <summary>
        /// set marker into drawing on xy-position of desired line
        /// </summary>
        public static void SetPosMarkerLine(int line, bool markFigure)
        {
            if (logDetailed) 
                Logger.Trace("  SetPosMarkerLine line:{0}  markFigure:{1}", line, markFigure);
            if (line < 0) return;

            int figureNr;
            XyPoint center = new XyPoint(0, 0);
            bool showCenter = false;
            try
            {
                if (line < coordList.Count)
                {
                    if (line == coordList[line].lineNumber)
                    {
                        Grbl.PosMarker = (XyzPoint)coordList[line].actualPos;
                        Grbl.PosMarkerAngle = coordList[line].alpha;
                        if (coordList[line].isArc)
                        {
                            foreach (CenterByLine point in centerList)
                            {
                                if (point.lineNumber == coordList[line].lineNumber)
                                { center = (XyPoint)point.center; showCenter = true; break; }
                            }
                        }
                        CreateMarkerPath(showCenter, center, (XyPoint)coordList[line].actualPos);// line-1
                        figureNr = coordList[line].figureNumber;
                        if ((figureNr != lastFigureNumber) && (markFigure))
                        {
                            MarkSelectedFigure(figureNr);
                            lastFigureNumber = figureNr;
                        }
                    }
                    else
                    {
                        XyPoint last = new XyPoint(0, 0);
                        foreach (CoordByLine gcline in coordList)
                        {
                            if (line == gcline.lineNumber)
                            {
                                Grbl.PosMarker = gcline.actualPos;
                                Grbl.PosMarkerAngle = gcline.alpha;
                                if (gcline.isArc)
                                {
                                    foreach (CenterByLine point in centerList)
                                    {
                                        if (point.lineNumber == gcline.lineNumber) // ==line
                                        { center = (XyPoint)point.center; showCenter = true; break; }
                                    }
                                }
                                CreateMarkerPath(showCenter, center, last);
                                figureNr = coordList[line].figureNumber;
                                if ((figureNr != lastFigureNumber) && (markFigure))
                                {
                                    MarkSelectedFigure(figureNr);
                                    lastFigureNumber = figureNr;
                                }

                                break;
                            }
                            last = (XyPoint)gcline.actualPos;
                        }
                    }
                }
            }
            catch (Exception er) { Logger.Error(er, " SetPosMarkerLine line:{0}  coordList.Count:{1}  markFigure:{2}", line, coordList.Count, markFigure); }
        }


        public static void SetPosMarkerLineSimple(int line)
        {
            if (logDetailed) Logger.Trace("  SetPosMarkerLineSimple line:{0} ", line);
            if (line < 0) return;

            XyPoint center = new XyPoint(0, 0);
            bool showCenter = false;
            try
            {
                if (line < coordList.Count)
                {
                    Grbl.PosMarker = (XyzPoint)coordList[line].actualPos;
                    Grbl.PosMarkerAngle = coordList[line].alpha;
                    if (coordList[line].isArc)
                    {
                        foreach (CenterByLine point in centerList)
                        {
                            if (point.lineNumber == coordList[line].lineNumber)
                            { center = (XyPoint)point.center; showCenter = true; break; }
                        }
                    }
                    CreateMarkerPath(showCenter, center, (XyPoint)coordList[line].actualPos);// line-1
                }
            }
            catch (Exception er) { Logger.Error(er, " SetPosMarkerLineSimple line:{0}  coordList.Count:{1}", line, coordList.Count); }
        }

        /// <summary>
        /// find gcode line with xy-coordinates near by given coordinates
        /// </summary>
        internal static DistanceByLine SetPosMarkerNearBy(XyPoint pos, bool findNode)//, bool toggleHighlight)
        {
            bool checkDistanceToLine = !(largeDataAmount || findNode);
            if (logDetailed)
                Logger.Trace("SetPosMarkerNearBy findNode:{0}  checkDistanceToLine:{1}  x:{2:0.00} y:{3:0.00}  ", findNode, checkDistanceToLine, pos.X, pos.Y);

            /* fill list with coordByLine with actual distance to given point */
            if ((coordList == null) || (coordList.Count == 0))
            {
                DistanceByLine tmp = new DistanceByLine(0)
                {
                    actualPos = new XyzPoint(pos, 0)
                };
                return tmp;
            }

            List<DistanceByLine> tmpList = new List<DistanceByLine>();     // get all coordinates (also subroutines)
            double minDist = double.MaxValue;
            double tmpDist;
            foreach (CoordByLine gcline in coordList)	// from actual figures
            {
                if (gcline.figureNumber >= 0)
                {
                    gcline.CalcDistance(pos, checkDistanceToLine);       // calculate distance work coordinates, also to any point on line between Pold, Pnew
                    if (gcline.distance < minDist)
                    {
                        minDist = gcline.distance;
                        tmpList.Add(new DistanceByLine(gcline, minDist));            // add to new list
                    }
                }
            }
            foreach (CenterByLine centerItem in centerList)	// from actual figures
            {
                if (centerItem.figureNumber >= 0)
                {
                    tmpDist = centerItem.CalcDistance(pos);//, checkDistanceToLine);
                                                           //    if (tmpDist < minDist)
                    {
                        minDist = tmpDist;
                        tmpList.Add(new DistanceByLine(centerItem, minDist));            // add to new list
                    }
                }
            }
            if (Properties.Settings.Default.guiBackgroundShow && (coordListLandMark.Count > 0))
            {
                foreach (CoordByLine coordItem in coordListLandMark)	// from background figures
                {
                    coordItem.CalcDistance(pos + (XyPoint)Grbl.posWCO, checkDistanceToLine);      // calculate distance machine coordinates
                    if (coordItem.distance < minDist)
                    {
                        minDist = coordItem.distance;
                        tmpList.Add(new DistanceByLine(0, coordItem.figureNumber, (coordItem.actualPos - Grbl.posWCO), coordItem.alpha, minDist, coordItem.isArc));
                    }
                }
            }

            /* sort list by distance, get associated linenr. */
            List<DistanceByLine> SortedList = tmpList.OrderBy(o => o.distance).ToList();
            if (SortedList.Count == 0)
            {
                Logger.Error("SetPosMarkerNearBy SortedList = 0");
                return new DistanceByLine(0);
            }

            int line = 0;
            DistanceByLine returnVal = SortedList[0];

            /* check if other node of same figure is closer to click position */
            int gline = SortedList[line].lineNumber;
            int clIndex = gline;
            int selIndex = clIndex;
            if (checkDistanceToLine)
            {
                if ((gline > 0) && (gline < coordList[coordList.Count - 2].lineNumber))
                {
                    for (int i = Math.Max(0, gline - 10); i < Math.Min(coordList.Count, gline + 10); i++)   // from actual figures
                    {
                        if (coordList[i].lineNumber == gline)
                        { clIndex = i; }
                    }
                    selIndex = clIndex;

                    double distBefore = ((XyPoint)coordList[clIndex - 1].actualPos).DistanceTo((XyPoint)pos);
                    double distActual = ((XyPoint)coordList[clIndex].actualPos).DistanceTo((XyPoint)pos);
                    double distAfter = ((XyPoint)coordList[clIndex + 1].actualPos).DistanceTo((XyPoint)pos);
                    if ((distBefore < distActual) && (coordList[clIndex - 1].figureNumber == coordList[clIndex].figureNumber))
                    { gline = coordList[clIndex - 1].lineNumber; selIndex = clIndex - 1; }
                    if ((distAfter < distActual) && (coordList[clIndex + 1].figureNumber == coordList[clIndex].figureNumber))
                    { gline = coordList[clIndex + 1].lineNumber; selIndex = clIndex + 1; }

                    if (logDetailed) Logger.Info("SetPosMarkerNearBy gline:{0}  dist-1:{1:0.000}  dist-0:{2:0.000}  dist+1:{3:0.000}", gline, distBefore, distActual, distAfter);

                    returnVal = new DistanceByLine(coordList[selIndex], 0);
                }
            }

            /* find G0 line with X/Y coordinates */
            if (findNode && (coordList[selIndex].actualG == 0) && (gcodeList[selIndex].x == null) && (gcodeList[selIndex].y == null))
            {
                if (logDetailed) Logger.Info("SetPosMarkerNearBy G0 without XY at {0}  '{1}'", returnVal.lineNumber, gcodeList[selIndex].codeLine);
                for (int i = selIndex; i < Math.Min(coordList.Count, selIndex + 10); i++)   // from actual figures
                {
                    if ((coordList[i].actualG == 0) && (gcodeList[i].x != null || gcodeList[selIndex].y != null))
                    {
                        selIndex = i;
                        returnVal = new DistanceByLine(coordList[selIndex], 0);
                        if (logDetailed) Logger.Info("SetPosMarkerNearBy find G0 with XY at {0}", returnVal.lineNumber);
                        break;
                    }
                }
            }

            Grbl.PosMarker = returnVal.actualPos;
            Grbl.PosMarkerAngle = returnVal.alpha;
            int figureNr = returnVal.figureNumber;

            /* if possible, get center of arc */
            XyPoint center = new XyPoint(0, 0);
            bool showCenter = false;
            if (returnVal.isArc && (centerList.Count > 0))
            {
                foreach (CenterByLine point in centerList)
                {
                    if (point.lineNumber == returnVal.lineNumber)
                    { center = (XyPoint)point.center; showCenter = true; break; }
                }
            }

            CreateMarkerPath(showCenter, center);
            return returnVal;
        }

        public static string CutOutFigure()
        {
            if (lastFigureNumber > 0)
            {
                Logger.Trace("▌ CutOutFigure figure:{0}", lastFigureNumber);
                for (int i = gcodeList.Count - 1; i >= 0; i--)
                {
                    if ((gcodeList[i].figureNumber != -1) && (gcodeList[i].figureNumber != lastFigureNumber))
                    { gcodeList.RemoveAt(i); }
                }
            }
            return CreateGCodeProg();
        }

        public static int GetFigureNumber(int line)
        {
            foreach (CoordByLine gcline in coordList)           // start search at beginning
            {
                if (gcline.lineNumber == line)    // 1st occurance = hit
                {
                    return gcline.figureNumber;
                }
            }
            return -1;
        }


        public static void MarkSelectedNode(DistanceByLine markerProperties)
        {
            CoordByLine before, after;
            bool beforeOk, afterOk;

            if ((coordList == null) || (coordList.Count == 0))
                return;

            int line = markerProperties.lineNumber;
            int lineBefore = line - 1;
            int lineAfter = line + 1;

            //     Logger.Info("MarkSelectedNode  -1:{0}   0:{1}   +1:{2}", lineBefore, line, lineAfter);
            if (line < (coordList.Count - 1))
            {
                if ((gcodeList[lineAfter].x == null) && (gcodeList[lineAfter].y == null))   // no XY move? Perhaps pen-down commands
                {
                    for (int i = lineAfter; i < Math.Min(lineAfter + 5, gcodeList.Count); i++)
                    {
                        //                Logger.Info("{0}  {1}   {2}", i, gcodeList[i].figureNumber , gcodeList[line].figureNumber);
                        if (((gcodeList[i].x != null) || (gcodeList[i].y != null)) && (gcodeList[i].figureNumber == gcodeList[line].figureNumber))
                        {
                            lineAfter = i;
                            break;
                        }
                    }
                }
                //        Logger.Info("MarkSelectedNode  -1:{0}   0:{1}   +1:{2}", lineBefore, line, lineAfter);
                before = coordList[lineBefore];
                //    now = coordList[line];
                after = coordList[lineAfter];

                beforeOk = !(before.actualG == 0);
                afterOk = !(after.actualG == 0);

                SelectionHandle.SetSelectionPath(before.actualPos.ToPointF(), beforeOk, markerProperties.actualPos.ToPointF(), after.actualPos.ToPointF(), afterOk);
                pathMarkSelection.Reset();
                pathMarkSelection.StartFigure();
                if (beforeOk) pathMarkSelection.AddLine(before.actualPos.ToPointF(), markerProperties.actualPos.ToPointF());
                if (afterOk) pathMarkSelection.AddLine(markerProperties.actualPos.ToPointF(), after.actualPos.ToPointF());
            }
            SelectionHandle.SetBounds(new RectangleF((float)markerProperties.actualPos.X, (float)markerProperties.actualPos.Y, 0, 0));
            selectedFigureInfo = string.Format("Selection: X {0:0.000} Y {1:0.000}", markerProperties.actualPos.X, markerProperties.actualPos.Y);
        }


        /// <summary>
        /// create path of selected figure, or clear (-1)
        /// </summary>
        public static void MarkSelectedFigure(int figureNr)
        {
            Logger.Trace("MarkSelectedFigure figureNr:{0}  caller:{1}", figureNr, (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            lastFigureNumbers.Clear();
            if (figureNr < 0)   // <=0
            {
                pathMarkSelection.Reset();
                lastFigureNumber = -2;
                lastFigureNumbers.Clear();
                selectedFigureInfo = "";
                SelectionHandle.IsActive = false;
                return;
            }
            lastFigureNumber = figureNr;
            // get dimension of selection
            Dimensions tmpSize = new Dimensions();
            foreach (GcodeByLine gcline in gcodeList)
            {
                if ((gcline.figureNumber == figureNr) && XyMove(gcline))
                { tmpSize.SetDimensionXY(gcline.actualPos.X, gcline.actualPos.Y); }
            }
            //     XyPoint tmpCenter = new XyPoint(tmpSize.GetCenter());
            //    selectedFigureInfo = string.Format("Selection: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", figureNr, tmpSize.dimx, tmpSize.dimy, tmpCenter.X, tmpCenter.Y);

            // find and copy selected path
            pathMarkSelection.Reset();
            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();
            for (int i = 1; i <= figureNr; i++)
                myPathIterator.NextMarker(pathMarkSelection);

            myPathIterator.Dispose();
            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            SelectionHandle.SetBounds(selectionBounds);
            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
            selectedFigureInfo = string.Format("Selected figure: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", figureNr, selectionBounds.Width, selectionBounds.Height, centerX, centerY);
        }

        public static void MarkSelectedGroup(int start)		// mark all figures within a group
        {
            if (start <= 0) return;
            if (start >= (gcodeList.Count - 1)) return;
            List<int> figures = new List<int>();
            int figNr;

            pathMarkSelection.Reset();
            GraphicsPath tmpPath = new GraphicsPath();
            tmpPath.Reset();

            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();

            int line = start + 1;
            lastFigureNumber = gcodeList[line].figureNumber;
            lastFigureNumbers.Clear();

            for (line = start + 1; line < gcodeList.Count; line++)			// go through gcode-lines
            {
                if (gcodeList[line].codeLine.Contains(XmlMarker.GroupEnd))		// find group-end tag
                    break;
                figNr = gcodeList[line].figureNumber;							// get figure nummer
                if (!figures.Contains(figNr) && (figNr > 0))					// add fig-nr to list, if not already in
                {
                    figures.Add(figNr);											// add fig-nr to list
                    myPathIterator.Rewind();
                    for (int i = 1; i <= figNr; i++)							// set path-marker to figure-position
                        myPathIterator.NextMarker(tmpPath);                     // and copy path to tmpPath

                    try { pathMarkSelection.AddPath(tmpPath, false); }          // add figure-path to marker-path
                    catch (Exception err)
                    { Logger.Error(err, "MarkSelectedGroup start:{0} figNr:{1} "); }
                    lastFigureNumbers.Add(figNr);
                }
            }
            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            SelectionHandle.SetBounds(selectionBounds);
            myPathIterator.Dispose();
            tmpPath.Dispose();

            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
            selectedFigureInfo = string.Format("Selected group: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", XmlMarker.lastGroup.Id, selectionBounds.Width, selectionBounds.Height, centerX, centerY);
        }

        public static void MarkSelectedTile(int start)
        {
            if (start <= 0) return;
            if (start >= (gcodeList.Count - 1)) return;
            List<int> figures = new List<int>();
            int figNr;

            pathMarkSelection.Reset();
            GraphicsPath tmpPath = new GraphicsPath();
            tmpPath.Reset();

            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();
            lastFigureNumbers.Clear();

            for (int line = start + 1; line < gcodeList.Count; line++)		// go through tile code lines
            {
                if (gcodeList[line].codeLine.Contains(XmlMarker.TileEnd))
                    break;
                lastFigureNumber = figNr = gcodeList[line].figureNumber;
                if (!figures.Contains(figNr) && (figNr > 0))				// find figures
                {
                    figures.Add(figNr);										// collect figure nr.
                    myPathIterator.Rewind();
                    for (int i = 1; i <= figNr; i++)
                        myPathIterator.NextMarker(tmpPath);					// find figure path
                    pathMarkSelection.AddPath(tmpPath, false);				// add figure path
                    lastFigureNumbers.Add(figNr);
                }
            }
            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            SelectionHandle.SetBounds(selectionBounds);						// set and activate selection handle
            tmpPath.Dispose();
            myPathIterator.Dispose();
        }

        public static void MarkSelectedCollection(int start)
        {
            if (start <= 0) return;
            if (start >= (gcodeList.Count - 1)) return;
            List<int> figures = new List<int>();
            int figNr;

            pathMarkSelection.Reset();
            GraphicsPath tmpPath = new GraphicsPath();
            tmpPath.Reset();

            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();

            lastFigureNumbers.Clear();

            Logger.Info("MarkSelectedCollection start:{0} {1}", start, gcodeList[start].codeLine);

            int line;
            for ( line = start + 1; line < gcodeList.Count; line++)		// go through tile code lines
            {
                if (gcodeList[line].codeLine.Contains(XmlMarker.CollectionEnd))
                    break;
                figNr = gcodeList[line].figureNumber;
                if (!figures.Contains(figNr) && (figNr > 0))				// find figures
                {
                    lastFigureNumber = figNr;
                    figures.Add(figNr);										// collect figure nr.
                    myPathIterator.Rewind();
                    for (int i = 1; i <= figNr; i++)
                        myPathIterator.NextMarker(tmpPath);					// find figure path
                    pathMarkSelection.AddPath(tmpPath, false);				// add figure path
                    lastFigureNumbers.Add(figNr);
                }
            }

            Logger.Info("MarkSelectedCollection end:{0} {1}  lastFig:{2}", line, gcodeList[line].codeLine, lastFigureNumber);

            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            SelectionHandle.SetBounds(selectionBounds);						// set and activate selection handle
            tmpPath.Dispose();
            myPathIterator.Dispose();

            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
            selectedFigureInfo = string.Format("Selected collection: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", XmlMarker.lastGroup.Id, selectionBounds.Width, selectionBounds.Height, centerX, centerY);
        }

        internal static bool GetPathCordinates(List<ImgPoint> posList, float maxDistance)
        {
            if (!gcodeList.Any())
                return false;

            if (ContainsG2G3Command())
            { }
            ImgPoint actualPos, lastPos;
            double dist;
            float dX, dY;
            int steps, i;
            bool lastWasG0 = true;
            lastPos = new ImgPoint();
            double offX = (xyzSize.maxx + xyzSize.minx) / 2;
            double offY = (xyzSize.maxy + xyzSize.miny) / 2;
            try
            {
                foreach (GcodeByLine gcline in gcodeList)
                {
                    actualPos = new ImgPoint((float)(gcline.actualPos.X - offX), (float)(gcline.actualPos.Y - offY), gcline.motionMode);
                    if (gcline.motionMode > 0)
                    {
                        if (lastWasG0)
                        { posList.Add(lastPos); }
                        dist = lastPos.DistanceTo(actualPos);
                        if ((gcline.motionMode == 1) && (dist > maxDistance))
                        {
                            steps = (int)((dist / maxDistance) + 1);
                            dX = (actualPos.X - lastPos.X) / steps;
                            dY = (actualPos.Y - lastPos.Y) / steps;
                            //        Logger.Info("GetPathCordinates dist:{0}  max:{1:0.00} steps:{2} ", dist, maxDistance, steps);
                            //        Logger.Info("GetPathCordinates start:{0:0.00} {1:0.00}  stop:{2:0.00} {3:0.00}",lastPos.X, lastPos.Y, actualPos.X, actualPos.Y);
                            if ((steps > 1) && (steps < 100000))
                            {
                                for (i = 1; i < steps; i++)
                                {
                                    posList.Add(new ImgPoint(lastPos.X + dX * i, lastPos.Y + dY * i, 1));
                                    //                Logger.Info("GetPathCordinates  {0:0.00} {1:0.00}", lastPos.X + dX * i, lastPos.Y + dY * i);
                                }
                            }
                        }
                        posList.Add(actualPos);
                        lastWasG0 = false;
                    }
                    else
                    {
                        posList.Add(actualPos);
                        lastWasG0 = true;
                    }
                    lastPos = actualPos;
                }
            }
            catch (Exception err)
            {
                EventCollector.StoreException("GetPathCordinates posList.Count=" + posList.Count + " " + err.Message + " ---");
                Logger.Error(err, "Could not create spiral, scanCNCPos.Count:{0}", posList.Count);
                System.Windows.Forms.MessageBox.Show("Error: " + err.Message + " \r\n\r\nPattern is may not complete.\r\nTry with higher Line distance", "Error");
            }

            return true;
        }
    }

}
