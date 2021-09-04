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
/*  GCodeVisuAndTransform.cs
    Scaling, Rotation, Remove OffsetXY, Mirror X or Y
    During transformation the drawing path will be generated, because cooridantes are already parsed.
    Return transformed GCode 
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
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

//#pragma warning disable CA1303	// Do not pass literals as localized parameters
//#pragma warning disable CA1305
//#pragma warning disable CA1307

namespace GrblPlotter
{
    internal static partial class VisuGCode
    {
        public enum Translate { None, ScaleX, ScaleY, Offset1, Offset2, Offset3, Offset4, Offset5, Offset6, Offset7, Offset8, Offset9, MirrorX, MirrorY, MirrorRotary };
        internal static Dimensions xyzSize = new Dimensions();
        internal static Dimensions G0Size = new Dimensions();
        internal static DrawingProperties drawingSize = new DrawingProperties();

        public enum ConvertMode { Nothing, RemoveZ, ConvertZToS };

        internal static double gcodeMinutes = 0;
        internal static double gcodeDistance = 0;

        internal static double feedXmax = 5000;
        internal static double feedYmax = 5000;
        internal static double feedZmax = 5000;
        internal static double feedAmax = 5000;
        internal static double feedBmax = 5000;
        internal static double feedCmax = 5000;
        internal static double feedDefault = 5000;


        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal static bool logEnable = true;
        internal static bool logDetailed = false;
        internal static bool logCoordinates = false;


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

        private static XyPoint origWCOLandMark = new XyPoint();
        private static List<CoordByLine> coordListLandMark = new List<CoordByLine>();
        private static XyPoint origWCOMachineLimit = new XyPoint();

        /// <summary>
        /// apply new z-value to all gcode coordinates
        /// </summary>
        internal static string ApplyHeightMap(HeightMap Map)//IList<string> oldCode,
        {
            maxStep = (float)Map.GridX;
            //getGCodeLines(oldCode, null, null, true);                // read gcode and process subroutines
            IList<string> tmp = CreateGCodeProg(true, true, false, ConvertMode.Nothing).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();      // split lines and arcs createGCodeProg(bool replaceG23, bool applyNewZ, bool removeZ, HeightMap Map=null)
            GetGCodeLines(tmp, null, null, false);                  // reload code
            return CreateGCodeProg(false, false, true, ConvertMode.Nothing, Map);        // apply new Z-value;
        }

        /// <summary>
        /// undo height map (reload saved backup)
        /// </summary>
        public static void ClearHeightMap()
        { pathHeightMap.Reset(); }

        // analyse each GCode line and track actual position and modes for each code line
        private static List<GcodeByLine> gcodeList = new List<GcodeByLine>();        // keep original program
  //      private static List<GcodeByLine> simuList;         // as gcodeList but resolved subroutines
        private static List<SimuCoordByLine> simuList;         // as gcodeList but resolved subroutines
        private static List<CoordByLine> coordList;        // get all coordinates (also subroutines)
        private static List<CoordByLine> centerList;       // get center of arcs

        private static GcodeByLine oldLine = new GcodeByLine();    // actual parsed line
        private static readonly GcodeByLine newLine = new GcodeByLine();    // last parsed line

        private static ModalGroup modal = new ModalGroup();        // keep modal states and helper variables

        private static readonly Dictionary<double, int> showHalftonePath = new Dictionary<double, int>();    // assignment penWidth to pathIndex


        /// <summary>
        /// set marker into drawing on xy-position of desired line
        /// </summary>
        public static void SetPosMarkerLine(int line, bool markFigure)
        {
            if (logDetailed) Logger.Trace("  setPosMarkerLine line:{0}  markFigure:{1}", line, markFigure);
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
                            foreach (CoordByLine point in centerList)
                            {
                                if (point.lineNumber == coordList[line].lineNumber)
                                { center = (XyPoint)point.actualPos; showCenter = true; break; }
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
                                    foreach (CoordByLine point in centerList)
                                    {
                                        if (point.lineNumber == gcline.lineNumber) // ==line
                                        { center = (XyPoint)point.actualPos; showCenter = true; break; }
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
            catch (Exception er) { Logger.Error(er, " setPosMarkerLine"); }
        }


        /// <summary>
        /// find gcode line with xy-coordinates near by given coordinates
        /// </summary>
        internal static int SetPosMarkerNearBy(XyPoint pos, bool toggleHighlight)
        {
            if (logDetailed) Logger.Trace(" setPosMarkerNearBy x:{0:0.00} y:{1:0.00}", pos.X, pos.Y);
            List<CoordByLine> tmpList = new List<CoordByLine>();     // get all coordinates (also subroutines)
            int figureNr;
            XyPoint center = new XyPoint(0, 0);
            bool showCenter = false;

            /* fill list with coordByLine with actual distance to given point */
            double minDist = double.MaxValue;
            foreach (CoordByLine gcline in coordList)	// from actual figures
            {
                if (gcline.figureNumber >= 0)
                {
                    gcline.CalcDistance(pos);       // calculate distance work coordinates
                    if (gcline.distance < minDist)
                    {
                        minDist = gcline.distance;
                        tmpList.Add(gcline);            // add to new list
                    }
                }
            }
            if (Properties.Settings.Default.guiBackgroundShow && (coordListLandMark.Count > 0))
            {
                foreach (CoordByLine gcline in coordListLandMark)	// from background figures
                {
                    gcline.CalcDistance(pos + (XyPoint)Grbl.posWCO);      // calculate distance machine coordinates
                    if (gcline.distance < minDist)
                    {
                        minDist = gcline.distance;
                        tmpList.Add(new CoordByLine(0, gcline.figureNumber, gcline.lastPos - Grbl.posWCO, gcline.actualPos - Grbl.posWCO, gcline.actualG, gcline.alpha, gcline.distance)); // add as work coord.
                    }
                }
            }

            /* sort list by distance, get associated linenr. */
            int line = 0;
            List<CoordByLine> SortedList = tmpList.OrderBy(o => o.distance).ToList();
            if (SortedList.Count == 0)
            {
                Logger.Error("SetPosMarkerNearBy SortedList = 0");
                return 0; 
            }

            Grbl.PosMarker = SortedList[line].actualPos;
            Grbl.PosMarkerAngle = SortedList[line].alpha;
            figureNr = SortedList[line].figureNumber;

            /* if possible, get center of arc */
            if (SortedList[line].isArc)
            {
                foreach (CoordByLine point in centerList)
                {
                    if (point.lineNumber == SortedList[line].lineNumber)
                    { center = (XyPoint)point.actualPos; showCenter = true; break; }
                }
            }

            CreateMarkerPath(showCenter, center);
            if ((figureNr != lastFigureNumber) || !toggleHighlight)
            {
                MarkSelectedFigure(figureNr);   // select
                lastFigureNumber = figureNr;
            }
            else
            {
                MarkSelectedFigure(-1);  // deselect
                lastFigureNumber = -1;
            }

            return SortedList[line].lineNumber;
        }
        private static int lastFigureNumber = -1;
        public static int GetHighlightStatus()
        { return lastFigureNumber; }

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

        public static void MarkSelectedGroup(int start)
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

            for (int line = start + 1; line < gcodeList.Count; line++)
            {
                if (gcodeList[line].codeLine.Contains(XmlMarker.GroupEnd))
                    break;
                figNr = gcodeList[line].figureNumber;
                if (!figures.Contains(figNr) && (figNr > 0))
                {
                    figures.Add(figNr);
                    myPathIterator.Rewind();
                    for (int i = 1; i <= figNr; i++)
                        myPathIterator.NextMarker(tmpPath);
                    pathMarkSelection.AddPath(tmpPath, false);
                }
            }
            myPathIterator.Dispose();
            tmpPath.Dispose();
            //    RectangleF selectionBounds = pathMarkSelection.GetBounds();
            //    float centerX = selectionBounds.X + selectionBounds.Width / 2;
            //   float centerY = selectionBounds.Y + selectionBounds.Height / 2;
        }

        public static void MarkSelectedTile(int start)
        {
            List<int> figures = new List<int>();
            int figNr;

            pathMarkSelection.Reset();
            GraphicsPath tmpPath = new GraphicsPath();
            tmpPath.Reset();

            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();

            for (int line = start + 1; line < gcodeList.Count; line++)
            {
                if (gcodeList[line].codeLine.Contains(XmlMarker.TileEnd))
                    break;
                figNr = gcodeList[line].figureNumber;
                if (!figures.Contains(figNr) && (figNr > 0))
                {
                    figures.Add(figNr);
                    myPathIterator.Rewind();
                    for (int i = 1; i <= figNr; i++)
                        myPathIterator.NextMarker(tmpPath);
                    pathMarkSelection.AddPath(tmpPath, false);
                }
            }
            tmpPath.Dispose();
            myPathIterator.Dispose();
            //     RectangleF selectionBounds = pathMarkSelection.GetBounds();
            //    float centerX = selectionBounds.X + selectionBounds.Width / 2;
            //    float centerY = selectionBounds.Y + selectionBounds.Height / 2;
        }

        internal static string selectedFigureInfo = "";
        /// <summary>
        /// create path of selected figure, or clear (-1)
        /// </summary>
        public static void MarkSelectedFigure(int figureNr)
        {
            if (figureNr <= 0)
            {
                pathMarkSelection.Reset();
                lastFigureNumber = -2;
                selectedFigureInfo = "";
                return;
            }
            // get dimension of selection
            Dimensions tmpSize = new Dimensions();
            foreach (GcodeByLine gcline in gcodeList)
            {
                if ((gcline.figureNumber == figureNr) && XyMove(gcline))
                { tmpSize.SetDimensionXY(gcline.actualPos.X, gcline.actualPos.Y); }
            }
            XyPoint tmpCenter = new XyPoint(tmpSize.GetCenter());
            selectedFigureInfo = string.Format("Selection: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", figureNr, tmpSize.dimx, tmpSize.dimy, tmpCenter.X, tmpCenter.Y);

            // find and copy selected path
            pathMarkSelection.Reset();
            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();
            for (int i = 1; i <= figureNr; i++)
                myPathIterator.NextMarker(pathMarkSelection);

            myPathIterator.Dispose();
            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
            selectedFigureInfo = string.Format("Selection: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", figureNr, selectionBounds.Width, selectionBounds.Height, centerX, centerY);
        }


        /// <summary>
        /// rotate and scale arround offset
        /// </summary>
        internal static string TransformGCodeRotate(double angle, double scale, XyPoint offset, bool calcCenter = true)
        {
            Logger.Debug("Rotate angle: {0}", angle);
            if (gcodeList == null) return "";
            XyPoint centerOfFigure = xyzSize.GetCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = GetCenterOfMarkedFigure();

            if (calcCenter)
                offset = centerOfFigure;

            double? newvalx, newvaly, newvali, newvalj;
            oldLine.ResetAll(Grbl.posWork);         // reset coordinates and parser modes
            ClearDrawingPath();                    // reset path, dimensions
            bool offsetApplied = false;
            double lastAbsPosX = 0;
            double lastAbsPosY = 0;

            foreach (GcodeByLine gcline in gcodeList)
            {
                // if only a single figure is marked to be rotated
                // and motion mode is 91, the relative position to the next (not rotated) figure must be adapted
                if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))
                {
                    if (!gcline.isdistanceModeG90 && offsetApplied)         // correct relative movement of next figure
                    {
                        if ((gcline.motionMode == 0) && ((gcline.x != null) || (gcline.y != null)))
                        {
                            gcline.x = gcline.actualPos.X - lastAbsPosX;
                            gcline.y = gcline.actualPos.Y - lastAbsPosY;
                            offsetApplied = false;
                        }
                    }
                    continue;
                }

                if (!gcline.ismachineCoordG53)
                {
                    if ((gcline.x != null) || (gcline.y != null))
                    {

                        newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                        newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                        if (gcline.isdistanceModeG90)	// absolute
                        {
                            //                            newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            //                            newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                            gcline.x = (newvalx * scale) + offset.X;
                            gcline.y = (newvaly * scale) + offset.Y;
                        }
                        else
                        {
                            if ((gcline.motionMode == 0) && !offsetApplied)
                            {
                                //                                newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                                //                                newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                                gcline.x = gcline.x + ((newvalx * scale) + offset.X) - gcline.actualPos.X;
                                gcline.y = gcline.y + ((newvaly * scale) + offset.Y) - gcline.actualPos.Y;
                                if ((gcline.x != null) && (gcline.y != null))
                                    offsetApplied = true;
                            }
                            else
                            {
                                //  newvalx = gcline.x * Math.Cos(angle * Math.PI / 180) - gcline.y * Math.Sin(angle * Math.PI / 180);
                                //  newvaly = gcline.x * Math.Sin(angle * Math.PI / 180) + gcline.y * Math.Cos(angle * Math.PI / 180);
                                gcline.x = gcline.x * Math.Cos(angle * Math.PI / 180) - gcline.y * Math.Sin(angle * Math.PI / 180);//newvalx;
                                gcline.y = gcline.x * Math.Sin(angle * Math.PI / 180) + gcline.y * Math.Cos(angle * Math.PI / 180);//newvaly;
                            }
                            //                            newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            //                            newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                            lastAbsPosX = ((double)newvalx * scale) + offset.X; //gcline.actualPos.X;
                            lastAbsPosY = ((double)newvaly * scale) + offset.Y; //gcline.actualPos.Y;
                        }
                    }
                    if ((gcline.i != null) || (gcline.j != null))
                    {
                        newvali = (double)gcline.i * Math.Cos(angle * Math.PI / 180) - (double)gcline.j * Math.Sin(angle * Math.PI / 180);
                        newvalj = (double)gcline.i * Math.Sin(angle * Math.PI / 180) + (double)gcline.j * Math.Cos(angle * Math.PI / 180);
                        gcline.i = newvali * scale;
                        gcline.j = newvalj * scale;
                    }
                    if (tangentialAxisEnable)
                    {
                        if ((tangentialAxisName == "C") && (gcline.c != null)) { gcline.c += angle; }
                        else if ((tangentialAxisName == "B") && (gcline.b != null)) { gcline.b += angle; }
                        else if ((tangentialAxisName == "A") && (gcline.a != null)) { gcline.a += angle; }
                        else if ((tangentialAxisName == "Z") && (gcline.z != null)) { gcline.z += angle; }
                    }

                    //       calcAbsPosition(gcline, oldLine);
                    //       oldLine = new gcodeByLine(gcline);   // get copy of newLine
                }
            }
            //			pathBackground.Reset();
            return CreateGCodeProg();
        }

        /// <summary>
        /// scale x and y seperatly in %
        /// </summary>
        public static string TransformGCodeScale(double scaleX, double scaleY)
        {
            Logger.Debug("Scale scaleX: {0}, scale Y: {1}", scaleX, scaleY);
            if (gcodeList == null) return "";
            XyPoint centerOfFigure = xyzSize.GetCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = GetCenterOfMarkedFigure();
            double factor_x = scaleX / 100;
            double factor_y = scaleY / 100;
            bool offsetApplied = false;
            double lastAbsPosX = 0;
            double lastAbsPosY = 0;

            oldLine.ResetAll(Grbl.posWork);         // reset coordinates and parser modes
            ClearDrawingPath();                    // reset path, dimensions
            foreach (GcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))
                {
                    if (!gcline.isdistanceModeG90 && offsetApplied)         // correct relative movement of next figure
                    {
                        if ((gcline.motionMode == 0) && ((gcline.x != null) || (gcline.y != null)))
                        {
                            gcline.x = gcline.actualPos.X - lastAbsPosX;
                            gcline.y = gcline.actualPos.Y - lastAbsPosY;
                            offsetApplied = false;
                        }
                    }
                    continue;
                }

                if (!gcline.ismachineCoordG53)
                {
                    if (gcline.isdistanceModeG90)           // absolute move: apply offset to any XY position
                    {
                        if (gcline.x != null)
                            gcline.x = gcline.x * factor_x - centerOfFigure.X * (factor_x - 1);
                        if (gcline.y != null)
                            gcline.y = gcline.y * factor_y - centerOfFigure.Y * (factor_y - 1);
                    }
                    else
                    {   //if (!offsetApplied)                 // relative move: apply offset only once
                        if ((gcline.motionMode == 0) && !offsetApplied)
                        {
                            if (gcline.x != null)
                                gcline.x -= (centerOfFigure.X - gcline.actualPos.X) * (factor_x - 1);
                            if (gcline.y != null)
                                gcline.y -= (centerOfFigure.Y - gcline.actualPos.Y) * (factor_y - 1);
                            if ((gcline.x != null) && (gcline.y != null))
                                offsetApplied = true;
                        }
                        else
                        {
                            if (gcline.x != null)
                                gcline.x *= factor_x;
                            if (gcline.y != null)
                                gcline.y *= factor_y;
                        }
                        lastAbsPosX = gcline.actualPos.X * factor_x - centerOfFigure.X * (factor_x - 1);
                        lastAbsPosY = gcline.actualPos.Y * factor_y - centerOfFigure.Y * (factor_y - 1);
                    }

                    if (gcline.i != null)
                        gcline.i *= factor_x;
                    if (gcline.j != null)
                        gcline.j *= factor_y;

                    CalcAbsPosition(gcline, oldLine);
                    oldLine = new GcodeByLine(gcline);   // get copy of newLine
                }
            }
            pathBackground.Reset();
            return CreateGCodeProg();
        }

        private static void GetTransaltionOffset(ref double offsetX, ref double offsetY, double tx, double ty, Translate shiftToZero)
        {
            if (shiftToZero == Translate.Offset1) { offsetX = tx + xyzSize.minx; offsetY = ty + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == Translate.Offset2) { offsetX = tx + xyzSize.minx + xyzSize.dimx / 2; offsetY = ty + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == Translate.Offset3) { offsetX = tx + xyzSize.minx + xyzSize.dimx; offsetY = ty + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == Translate.Offset4) { offsetX = tx + xyzSize.minx; offsetY = ty + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == Translate.Offset5) { offsetX = tx + xyzSize.minx + xyzSize.dimx / 2; offsetY = ty + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == Translate.Offset6) { offsetX = tx + xyzSize.minx + xyzSize.dimx; offsetY = ty + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == Translate.Offset7) { offsetX = tx + xyzSize.minx; offsetY = ty + xyzSize.miny; }
            if (shiftToZero == Translate.Offset8) { offsetX = tx + xyzSize.minx + xyzSize.dimx / 2; offsetY = ty + xyzSize.miny; }
            if (shiftToZero == Translate.Offset9) { offsetX = tx + xyzSize.minx + xyzSize.dimx; offsetY = ty + xyzSize.miny; }
            if (shiftToZero == Translate.None) { offsetX = tx; offsetY = ty; }
        }

        public static string TransformGCodeOffset(double tx, double ty, Translate shiftToZero)
        {
            Logger.Debug("Transform X: {0}, Y: {1}, Offset: {2}", tx, ty, shiftToZero);
            if (gcodeList == null) return "";
            if ((lastFigureNumber <= 0) || (!(shiftToZero == Translate.None)))
            { pathMarkSelection.Reset(); lastFigureNumber = -1; }
            double offsetX = 0;
            double offsetY = 0;
            bool offsetApplied = false;
            bool noInsertNeeded = false;

            oldLine.ResetAll(Grbl.posWork);         // reset coordinates and parser modes

            GetTransaltionOffset(ref offsetX, ref offsetY, tx, ty, shiftToZero);

            if (modal.containsG91)    // relative move: insert rapid movement before pen down, to be able applying offset
            {
                newLine.ResetAll();
                int i, k;
                bool foundG91 = false;
                for (i = 0; i < gcodeList.Count; i++)       // find first relative move
                {
                    if ((!gcodeList[i].isdistanceModeG90) && (!gcodeList[i].isSubroutine) && (gcodeList[i].motionMode == 0) && (gcodeList[i].z != null))
                    { foundG91 = true; break; }
                }
                if (foundG91)
                {
                    for (k = i + 1; k < gcodeList.Count; k++)   // find G0 x y
                    {
                        if ((gcodeList[k].motionMode == 0) && (gcodeList[k].x != null) && (gcodeList[k].y != null))
                        { noInsertNeeded = true; break; }
                        if (gcodeList[k].motionMode > 0)
                            break;
                    }
                    if (!noInsertNeeded)
                    {
                        if ((gcodeList[i + 1].motionMode != 0) || ((gcodeList[i + 1].motionMode == 0) && ((gcodeList[i + 1].x == null) || (gcodeList[i + 1].y == null))))
                        {
                            if ((!noInsertNeeded) && (!gcodeList[i + 1].ismachineCoordG53))
                            {
                                ModalGroup tmp = new ModalGroup();
                                newLine.ParseLine(i, "G0 X0 Y0 (Insert offset movement)", ref tmp);
                                gcodeList.Insert(i + 1, newLine);
                            }
                        }
                    }
                }
            }
            bool hide_code = false; ;
            foreach (GcodeByLine gcline in gcodeList)
            {
                if (gcline.codeLine.Contains("%START_HIDECODE")) { hide_code = true; }
                if (gcline.codeLine.Contains("%STOP_HIDECODE")) { hide_code = false; }
                if ((!hide_code) && (!gcline.isSubroutine) && (!gcline.ismachineCoordG53) && (gcline.codeLine.IndexOf("(Setup - GCode") < 1)) // ignore coordinates from setup footer
                {
                    if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))    // 2019-11-30
                    { continue; }

                    if (gcline.isdistanceModeG90)           // absolute move: apply offset to any XY position
                    {
                        if (gcline.x != null)
                            gcline.x -= offsetX;      // apply offset
                        if (gcline.y != null)
                            gcline.y -= offsetY;      // apply offset
                    }
                    else
                    {
                        if (!offsetApplied)                 // relative move: apply offset only once
                        {
                            if (gcline.motionMode == 0)
                            {
                                gcline.x -= offsetX;
                                gcline.y -= offsetY;
                                if ((gcline.x != null) && (gcline.y != null))
                                    offsetApplied = true;
                            }
                        }
                    }
                    CalcAbsPosition(gcline, oldLine);
                    oldLine = new GcodeByLine(gcline);   // get copy of newLine
                }
            }
            pathBackground.Reset();
            return CreateGCodeProg();
        }

        // add given coordinates to drawing path
        private static int onlyZ = 0;
        private static int figureCount = 0;

        private static void MarkPath(GraphicsPath path, float x, float y, int type)
        {
            float markerSize = 1;
            if (!Properties.Settings.Default.importUnitmm)
            { markerSize /= 25.4F; }
            CreateMarker(path, x, y, markerSize, type, false);    // draw circle
        }

        // setup drawing area 
        public static void CalcDrawingArea()
        {
            double extend = 1.01;                                                       // extend dimension a little bit
            double roundTo = 5;                                                         // round-up dimensions
            if (!Properties.Settings.Default.importUnitmm)
            { roundTo = 0.25; }

            if ((xyzSize.dimx == 0) && (xyzSize.dimy == 0))
            {
                xyzSize.SetDimensionXY(G0Size.minx, G0Size.miny);
                xyzSize.SetDimensionXY(G0Size.maxx, G0Size.maxy);
                Logger.Info("xyz-Dimension=0, use G0-Dimension dim-x {0} dim-y {1}", G0Size.dimx, G0Size.dimy);
            }

            if ((xyzSize.miny == 0) && (xyzSize.maxy == 0))
            { xyzSize.miny = -1; xyzSize.maxy = 1; }

            drawingSize.minX = (float)(Math.Floor(xyzSize.minx * extend / roundTo) * roundTo);                  // extend dimensions
            if (drawingSize.minX >= 0) { drawingSize.minX = (float)-roundTo; }                                          // be sure to show 0;0 position
            drawingSize.maxX = (float)(Math.Ceiling(xyzSize.maxx * extend / roundTo) * roundTo);
            drawingSize.minY = (float)(Math.Floor(xyzSize.miny * extend / roundTo) * roundTo);
            if (drawingSize.minY >= 0) { drawingSize.minY = (float)-roundTo; }
            drawingSize.maxY = (float)(Math.Ceiling(xyzSize.maxy * extend / roundTo) * roundTo);

            //           createRuler(pathRuler, drawingSize.minX, drawingSize.maxX, drawingSize.minY, drawingSize.maxY);
            CreateRuler(pathRuler, drawingSize);

            CreateMarkerPath();

            CreateDimensionBox();
        }

        public static void CreateDimensionBox()
        {
            pathDimension.Reset();
            pathDimension.StartFigure();
            pathDimension.AddLine((float)xyzSize.minx, (float)xyzSize.miny, (float)xyzSize.minx, (float)xyzSize.maxy);
            pathDimension.AddLine((float)xyzSize.minx, (float)xyzSize.maxy, (float)xyzSize.maxx, (float)xyzSize.maxy);
            pathDimension.AddLine((float)xyzSize.maxx, (float)xyzSize.maxy, (float)xyzSize.maxx, (float)xyzSize.miny);
            pathDimension.CloseFigure();
        }


        public static void CreateMarkerPath()
        { CreateMarkerPath(false, new XyPoint(0, 0)); }

        internal static void CreateMarkerPath(bool showCenter, XyPoint center)
        { CreateMarkerPath(showCenter, center, center); }
        internal static void CreateMarkerPath(bool showCenter, XyPoint center, XyPoint last)
        {
            float msize = (float)Math.Sqrt(xyzSize.dimx * xyzSize.dimx + xyzSize.dimy * xyzSize.dimy) / 40f;
            msize = Math.Max(msize, 2);
            //            createMarker(pathTool,   (xyPoint)grbl.posWork, msize, 2);

            if (tangentialAxisEnable)
            {
                double posAngle = 0;
                double factor = Math.PI / ((double)Properties.Settings.Default.importGCTangentialTurn / 2);
                if (tangentialAxisName == "C") { posAngle = factor * Grbl.posWork.C; }
                else if (tangentialAxisName == "B") { posAngle = factor * Grbl.posWork.B; }
                else if (tangentialAxisName == "A") { posAngle = factor * Grbl.posWork.A; }
                else if (tangentialAxisName == "Z") { posAngle = factor * Grbl.posWork.Z; }
                CreateMarkerArrow(pathTool, msize, (XyPoint)Grbl.posWork, posAngle, 1);
                CreateMarkerArrow(pathMarker, msize, (XyPoint)Grbl.PosMarker, Grbl.PosMarkerAngle * 360 / (double)Properties.Settings.Default.importGCTangentialTurn);
            }
            else
            {
                CreateMarker(pathTool, (XyPoint)Grbl.posWork, msize, 2);
                CreateMarker(pathMarker, (XyPoint)Grbl.PosMarker, msize, 3);
            }

            if (showCenter)
            {
                CreateMarker(pathMarker, center, msize, 0, false);
                pathMarker.StartFigure(); pathMarker.AddLine((float)last.X, (float)last.Y, (float)center.X, (float)center.Y);
                pathMarker.StartFigure(); pathMarker.AddLine((float)center.X, (float)center.Y, (float)Grbl.PosMarker.X, (float)Grbl.PosMarker.Y);
            }
            if (Properties.Settings.Default.ctrl4thUse)
            {
                float scale = (float)Properties.Settings.Default.rotarySubstitutionDiameter * (float)Math.PI / 360;
                if (Properties.Settings.Default.ctrl4thInvert)
                    scale *= -1;

                if (Properties.Settings.Default.ctrl4thOverX)
                {
                    CreateMarker(pathTool, (float)Grbl.posWork.X, (float)Grbl.posWork.A * scale, msize, 2);
                }
                else
                {
                    CreateMarker(pathTool, (float)Grbl.posWork.A * scale, (float)Grbl.posWork.Y, msize, 2);
                }
            }
        }


        internal static bool GetPathCordinates(List<PointF> posList)    //, float maxDistance)
        {
            if (!gcodeList.Any())
                return false;

            PointF actualPos, lastPos;
            bool lastWasG0 = true;
            lastPos = new PointF();
            foreach (GcodeByLine gcline in gcodeList)
            {
                actualPos = new PointF((float)gcline.actualPos.X, (float)gcline.actualPos.Y);
                if ((gcline.motionMode > 0))
                {
                    if (lastWasG0)
                        posList.Add(lastPos);
                    posList.Add(actualPos);
                    lastWasG0 = false;
                }
                else
                {
                    if (!lastWasG0)
                        posList.Add(new PointF(float.NaN, float.NaN));  // mark end of figure
                    lastWasG0 = true;
                }
                lastPos = actualPos;
            }
            return true;
        }
    }
    internal struct DrawingProperties
    {
        public float minX, minY, maxX, maxY, rangeX, rangeY;
        public void DrawingProperty()
        { minX = 0; minY = 0; maxX = 0; maxY = 0; rangeX = 0; rangeY = 0; }
        public void SetX(float min, float max)
        { minX = Math.Min(min, max); maxX = Math.Max(min, max); rangeX = maxX - minX; }
        public void SetY(float min, float max)
        { minY = Math.Min(min, max); maxY = Math.Max(min, max); rangeY = maxY - minY; }
    };
}
