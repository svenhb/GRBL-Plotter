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
/*  GCodeAnalyze.cs
	Parse/analyze GCode from editor for 2D-view and extract further data
*/
/* 
 * 2021-07-27 split code from GCodeVisuAndTransform
 * 2021-09-02 CreateDrawingPathFromGCode add viewOffset for tiles
 * 2021-09-04 new struct to store simulation data: SimuCoordByLine in simuList
 * 2021-09-21 collect fiducials
 * 2021-09-29 add fiducialDimension
 * 2021-11-30 line 451 check if index < objec.count
 * 2021-12-13 bug fix line 843 && (pathInfoMarker.Any())
 * 2023-03-17 l:398 f:GetGCodeLines avoid same log twice
 * 2023-04-18 add clipDimension, clipOffset, isHeaderSection to be read from GCode-Header, too feed buttons in setup-clipping
 * 2023-04-26 l:822 f:ProcessXmlTagStart pen width option
 * 2023-11-15 l:579 f:GetGCodeLines add pathObjectPenColorOnlyNone = false; show default color, if all imported colors = none
 * 2023-11-29 l:510 f:GetGCodeLines add findSubroutineFailCounter; abort searching for subroutine if not found after 10 times
 * 2023-12-16 l:850 f:ProcessXmlTagStart check for fiducials via .ToUpper().Contains(fiducialLabel.ToUpper()))
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;

namespace GrblPlotter
{
    internal static partial class VisuGCode
    {
        // PathData: collect path information of one specific color 
        public static List<PathData> pathObject = new List<PathData>();
        public static bool pathObjectPenColorOnlyNone = false;

        public class PathData : IDisposable
        {
            public GraphicsPath path;
            public Color color = Color.White;
            public float width = 0;
            public Pen pen;
            public PointF offsetView;

            public PathData()
            {
                path = new GraphicsPath();
                color = Properties.Settings.Default.gui2DColorPenDown;
                width = (float)Properties.Settings.Default.gui2DWidthPenDown;
                pen = new Pen(color, width);
            }
            public PathData(string pencolor, double penwidth, PointF offset)
            {
                path = new GraphicsPath();
                if (string.IsNullOrEmpty(pencolor))
                { color = Properties.Settings.Default.gui2DColorPenDown; }
                else if (UInt32.TryParse(pencolor, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint clr))  // try Hex code #00ff00
                {
                    clr |= 0xff000000; // remove alpha
                    color = System.Drawing.Color.FromArgb((int)clr);
                }
                else
                {
                    color = Color.FromName(pencolor);
                    if (color == System.Drawing.Color.FromArgb(0))
                    { color = Properties.Settings.Default.gui2DColorPenDown; }
                }
                SetPathData(color, penwidth, offset);
            }
            public PathData(Color color, double penwidth, PointF offset)
            {
                SetPathData(color, penwidth, offset);
            }

            public void SetPathData(Color colr, double penwidth, PointF offset)
            {
                path = new GraphicsPath();
                width = (float)penwidth;
                offsetView = offset;

                if (width <= 0)
                    width = (float)Properties.Settings.Default.gui2DWidthPenDown;
                pen = new Pen(colr, width)
                {
                    LineJoin = LineJoin.Round,
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round
                };
            }

            // To detect redundant calls
            private bool _disposed = false;
            // Public implementation of Dispose pattern callable by consumers.
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            // Protected implementation of Dispose pattern.
            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                { return; }

                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    path.Dispose();
                    pen.Dispose();
                }
                _disposed = true;
            }
        }

        // PathInfo: store angle informaion for each start/end-point for Path-up arrow
        internal static List<PathInfo> pathInfoMarker = new List<PathInfo>();
        private static PathInfo tempPathInfo = new PathInfo();
        internal struct PathInfo
        {
            public PointF position;		// where next pen-down starts
            public double angleArrow;   // end of pen-up line
            public double angleFigure;  // start of pen-down figure
            public string info;			// figureCount
        };

        private static int figureMarkerCount;
        public static bool CodeBlocksAvailable()
        { return (figureMarkerCount >= 1); }

        private static bool updateFigureLineNeeded = false;
        private static bool xyPosChanged;
        private static bool figureActive = false;
        private static bool tileActive = false;
        private static int tileCount = 0;
        internal static bool ShiftTilePaths { get; set; }

        private static bool tangentialAxisEnable = false;
        private static string tangentialAxisName = "C";

        internal static double gcodeMinutes = 0;
        internal static double gcodeDistance = 0;

        public static string errorString = "";
        private static int error33cnt;

        private static bool halfToneEnable = false;
        private static PointF offset2DView = new PointF();

        internal static double feedXmax = 5000;		// to calculate processing time CalculateProcessTime
        internal static double feedYmax = 5000;
        internal static double feedZmax = 5000;
        internal static double feedAmax = 5000;
        internal static double feedBmax = 5000;
        internal static double feedCmax = 5000;
        internal static double feedDefault = 5000;

        // analyse each GCode line and track actual position and modes for each code line
        private static List<GcodeByLine> gcodeList = new List<GcodeByLine>();        // keep original program                                                                                     //      private static List<GcodeByLine> simuList;         // as gcodeList but resolved subroutines
        private static List<SimuCoordByLine> simuList = new List<SimuCoordByLine>();         // as gcodeList but resolved subroutines
        private static List<CoordByLine> coordList = new List<CoordByLine>();        // get all coordinates (also subroutines)
        private static List<CenterByLine> centerList = new List<CenterByLine>();

        internal static List<XyPoint> fiducialsCenter = new List<XyPoint>();
        private static bool fiducialEnable = false;
        private static Dimensions fiducialDimension = new Dimensions();
        private static string fiducialLabel = "Fiducials";

        internal static XyPoint clipDimension = new XyPoint(-1, -1);
        internal static XyPoint clipOffset = new XyPoint(-1, -1);
        private static bool isHeaderSection = false;

        internal static bool largeDataAmount = false;
        internal static int numberDataLines = 0;

        private static GcodeByLine oldLine = new GcodeByLine();    // actual parsed line
        private static readonly GcodeByLine newLine = new GcodeByLine();    // last parsed line

        private static ModalGroup modal = new ModalGroup();        // keep modal states and helper variables

        private static readonly Dictionary<double, int> showHalftonePath = new Dictionary<double, int>();    // assignment penWidth to pathIndex

        // HalfTone: store F, S, Z values for pen-width calculation
        private static class HalfTone
        {
            public static bool showHalftoneS;   		// true if XML marker found
            public static bool showHalftoneZ;   		// true if XML marker found
            public static double showHalftoneMin;		// set by XML marker info
            public static double showHalftoneMax;		// set by XML marker info
            public static double showHalftoneWidth;		// set by XML marker info
            public static double showHalftoneLastS;
            public static double showHalftoneLastZ;
            public static double lastS;
            public static double lastZ;
            public static double lastF;

            public static void Reset()
            {
                showHalftoneS = false;
                showHalftoneZ = false;
                showHalftoneMin = 0;
                showHalftoneMax = 100;
                showHalftoneWidth = 1;
                showHalftoneLastS = 1;
                showHalftoneLastZ = 1;
                lastS = -1;
                lastZ = double.MaxValue;
                lastF = -1;
            }

            public static int UpdateFSZ(int count)
            {
                if (newLine.wasSetF) { lastF = newLine.feedRate; }
                if (newLine.wasSetS) { lastS = newLine.spindleSpeed; }
                if (newLine.wasSetZ)
                {
                    lastZ = newLine.actualPos.Z;
                    if (modal.motionMode > 0)
                        count++;
                }
                return count;
            }

            public static double CalcWidth()
            {
                double width = -1;
                double diff;
                if (showHalftoneS && (showHalftoneLastS != lastS))
                {
                    diff = showHalftoneMax - showHalftoneMin;
                    //    if (diff > 0)
                    //        width = (lastS - showHalftoneMin) * showHalftoneWidth / diff;    // calculate pen-width
                    width = Math.Abs((lastS - showHalftoneMin) * showHalftoneWidth / diff);    // calculate pen-width
                }

                else if (showHalftoneZ && (showHalftoneLastZ != lastZ))
                {
                    diff = showHalftoneMax - showHalftoneMin;
                    //   if (diff > 0)
                    width = Math.Abs((lastZ - showHalftoneMin) * showHalftoneWidth / diff);    // calculate pen-width
                }
                return width;
            }
        }

        public static string GetGcodeListLine(int line)
        {
            if ((line >= 0) && (line < gcodeList.Count))
                return gcodeList[line].codeLine;
            else
                return "";
        }
        public static void SetGcodeListLine(int line, string newCode)
        {
            if ((line >= 0) && (line < gcodeList.Count))
                gcodeList[line].codeLine = newCode;
        }

        private static void ResetGlobalObjects()
        {
            gcodeList = new List<GcodeByLine>();    	// needed for code transformations
            simuList = new List<SimuCoordByLine>();    	// needed for simulation
            coordList = new List<CoordByLine>();    	// needed to find GCode-line by coordiante
            centerList = new List<CenterByLine>();  	// center coordinates of arcs
            pathInfoMarker = new List<PathInfo>();		// collect Pen-up path data (fig-id, angle)
            fiducialsCenter = new List<XyPoint>();
            fiducialEnable = false;
            fiducialLabel = Properties.Settings.Default.importFiducialLabel;

            clipDimension = new XyPoint();
            clipOffset = new XyPoint();
            isHeaderSection = false;

            modal = new ModalGroup();               // clear
            XmlMarker.Reset();                      // reset lists, holding marker line numbers
            oldLine.ResetAll(Grbl.posWork);         // reset coordinates and parser modes, set initial pos
            newLine.ResetAll();                     // reset coordinates and parser modes

            ClearDrawingPath();                    	// reset paths, dimensions
            HalfTone.Reset();
            showHalftonePath.Clear();

            figureMarkerCount = 0;
            tileCount = 0;
            lastFigureNumber = -1;
            lastFigureNumbers.Clear();
            pathActualDown = null;

            largeDataAmount = false;

            figureActive = false;
            tileActive = false;
            ShiftTilePaths = false;

            gcodeMinutes = 0;
            gcodeDistance = 0;
            tangentialAxisEnable = false;
            halfToneEnable = false;
            errorString = "";
            error33cnt = 0;
            VisuGCode.ProcessedPath.offset2DView = new System.Windows.Point();
            offset2DView = new PointF();
            pathObjectPenColorOnlyNone = false;

            lastSubroutine = new int[] { 0, 0, 0 };
        }

        /// <summary>
        /// Entrypoint for generating drawing path from given gcode
        /// </summary>
        public static bool GetGCodeLines(IList<string> oldCode, BackgroundWorker worker, DoWorkEventArgs e, bool processSubs = false)
        {
            ResetGlobalObjects();
            string[] GCode = oldCode.ToArray<string>();
            string singleLine;
            bool programEnd = false;
            bool isArc;
            bool showArrow = Properties.Settings.Default.gui2DPenUpArrow;
            bool showId = Properties.Settings.Default.gui2DPenUpId;
            bool showColors = Properties.Settings.Default.ctrlColorizeGCode;        // gui2DColorPenDownModeEnable;
            int skipLimit = (int)Properties.Settings.Default.ctrlImportSkip * 1000;
            int countZ = 0;
            int lastMotion = 0;
            int findSubroutineFailCounter = 0;
            string newPathProp;
            string oldPathProp = "";

            Logger.Info("▽▽▽▽ GetGCodeLines Count:{0} skip:{1}   Show colors if no XML-Tags:{2}  Show pen-up path arrows:{3}  Show pen-up path Ids:{4}  Use BackgroundWorker:{5}", oldCode.Count, skipLimit, showColors, showArrow, showId, (worker != null));
            if (oldCode.Count > skipLimit) // huge amount of code, reduce time consuming functionality
            {
                Logger.Info("⚠⚠⚠⚠ Huge amount of code (> {0} lines), reduce time consuming functionality (no pen-up-path-arrows/-ids, no colored pen-down-paths) !!!!!", skipLimit);
                showArrow = false;
                showId = false;
                showColors = false;
                largeDataAmount = true;
                numberDataLines = oldCode.Count;
            }
            else if (Properties.Settings.Default.gui2DShowVertexEnable)
            { Logger.Info("!!!!! Show path-node markers, type:{0}  size:{1} !!!!!", Properties.Settings.Default.gui2DShowVertexType, Properties.Settings.Default.gui2DShowVertexSize); }

            if (showColors)
                ToolTable.Init(" (GetGCodeLines)");

            worker?.ReportProgress(0, new MyUserState { Value = 0, Content = "Analyse GCode..." });
            int progressMain;
            float progressMainFactor = 1;
            if (showArrow || showId) progressMainFactor = 0.5f;

            int progressSub;
            int progressSubOld = 0;

            /*********************************************************************/
            for (int lineNr = 0; lineNr < GCode.Length; lineNr++)   // go through all gcode lines
            {
                modal.ResetSubroutine();                            // reset m, p, o, l Word
                singleLine = GCode[lineNr].ToUpper().Trim();        // get line, remove unneeded chars

                if (worker != null)
                {
                    progressSub = (lineNr * 100) / GCode.Length;
                    if (progressSub != progressSubOld)
                    {   //worker.ReportProgress(progressSub);
                        progressMain = (int)(progressSub * progressMainFactor);
                        worker.ReportProgress(progressSub, new MyUserState { Value = progressMain, Content = "Analyse GCode " + progressSub.ToString() + " %" });
                        progressSubOld = progressSub;
                    }
                    if (worker.CancellationPending)
                    { break; }
                }

                if (processSubs && programEnd)						// surround subroutine-code in ( )
                { singleLine = "( " + singleLine + " )"; }          // don't process subroutine itself when processed

                newLine.ParseLine(lineNr, singleLine, ref modal);	// extract data from GCode
                xyPosChanged = CalcAbsPosition(newLine, oldLine);   // Calc. absolute positions and set object dimension: xyzSize.setDimension

                countZ = HalfTone.UpdateFSZ(countZ);    // use Z, S or F to find tool/color in tool table, if not found, don't switch color

                if (!largeDataAmount && showColors && (figureMarkerCount == 0))  // if no XML-Tags found...
                {
                    // try to colorize tool-path with tool-table settings, even if no xml-tags available
                    if (newLine.wasSetXY && (lastMotion == 0) && (modal.motionMode > 0))// else if G1/2/3 XY F				
                    {
                        string log = string.Format("Line {0} ", (newLine.lineNumber + 1));
                        newPathProp = string.Format("F:{0} S:{1} Z:{2:0.000} ", HalfTone.lastF, HalfTone.lastS, HalfTone.lastZ);
                        log += newPathProp;
                        ToolProp penProp = new ToolProp(1, Properties.Settings.Default.gui2DColorPenDown, "PenDown")
                        {
                            Diameter = (float)Properties.Settings.Default.gui2DWidthPenDown
                        };

                        if (countZ == 0)
                            HalfTone.lastZ = double.MaxValue;
                        penProp = ToolTable.FindToolByFSZ(HalfTone.lastF, HalfTone.lastS, HalfTone.lastZ, penProp);
                        PathData tmp = new PathData(penProp.Color, penProp.Diameter, offset2DView);
                        pathObject.Add(tmp);
                        pathActualDown = pathObject[pathObject.Count - 1].path;
                        log += string.Format("Set tool nr:{0} color:{1} figureMarkerCount:{2}", penProp.Toolnr, ColorTranslator.ToHtml(penProp.Color), figureMarkerCount);
                        if (penProp.Toolnr < 0)
                        {
                            if (!String.Equals(newPathProp, oldPathProp))
                            { Logger.Trace("!!!!!!!!!!!!!!  Tool not found: {0}  !!!!!!!!!!!!!!!", log); }
                            oldPathProp = newPathProp;
                        }
                    }
                    if (newLine.wasSetXY)
                        lastMotion = modal.motionMode;
                }

                if (singleLine.Contains("<"))
                {
                    ProcessXmlTagStart(GCode[lineNr], lineNr);
                }        // Original line transferred because of uppercase lowercase

                // collect halftone data
                if (halfToneEnable)   // S or Z are calculated from gray-value - range 0-255
                { ProcessHalftoneData(); }


                if ((modal.mWord == 98) && processSubs)
                { newLine.codeLine = "(" + GCode[lineNr] + ")"; }
                else
                {
                    if (processSubs && programEnd)
                        newLine.codeLine = "( " + GCode[lineNr] + " )";   // don't process subroutine itself when processed
                    else
                        newLine.codeLine = GCode[lineNr];                 // store original line
                }

                if (!programEnd)
                {
                    CreateDrawingPathFromGCode(newLine, oldLine, offset2DView);        // add data to drawing path
                    CalculateProcessTime(newLine, oldLine);
                }
                if (figureMarkerCount > 0)
                {
                    if (figureActive)
                        newLine.figureNumber = figureMarkerCount;
                    else
                        newLine.figureNumber = -1;
                }
                if (tangentialAxisEnable)
                {
                    if (tangentialAxisName == "C") { newLine.alpha = Math.PI * newLine.actualPos.C / 180; }
                    else if (tangentialAxisName == "B") { newLine.alpha = Math.PI * newLine.actualPos.B / 180; }
                    else if (tangentialAxisName == "A") { newLine.alpha = Math.PI * newLine.actualPos.A / 180; }
                    else if (tangentialAxisName == "Z") { newLine.alpha = Math.PI * newLine.actualPos.Z / 180; }
                    //                    else if (tangentialAxisName == "U") { newLine.alpha = Math.PI * newLine.actualPos.U / 180; }
                    //                    else if (tangentialAxisName == "V") { newLine.alpha = Math.PI * newLine.actualPos.V / 180; }
                    //                    else if (tangentialAxisName == "W") { newLine.alpha = Math.PI * newLine.actualPos.W / 180; }
                }
                else
                    newLine.alpha = 0;

                isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));

                // tiles are shifted in 2D view
                if (tileActive && Properties.Settings.Default.importGraphicClipShowOrigPosition && Properties.Settings.Default.importGraphicClipOffsetApply)
                {
                    XyzPoint tmpOldLine = new XyzPoint(oldLine.actualPos.X + offset2DView.X, oldLine.actualPos.Y + offset2DView.Y, oldLine.actualPos.Z);
                    XyzPoint tmpNewLine = new XyzPoint(newLine.actualPos.X + offset2DView.X, newLine.actualPos.Y + offset2DView.Y, newLine.actualPos.Z);
                    CoordByLine tmpLine = new CoordByLine(lineNr, newLine.figureNumber, tmpOldLine, tmpNewLine, newLine.motionMode, newLine.alpha, isArc);
                    coordList.Add(tmpLine);

                    GcodeByLine stmpLine = new GcodeByLine(newLine);
                    stmpLine.actualPos.X = newLine.actualPos.X + offset2DView.X;
                    stmpLine.actualPos.Y = newLine.actualPos.Y + offset2DView.Y;
                    simuList.Add(new SimuCoordByLine(stmpLine, offset2DView));         // add parsed line to list
                }
                else
                {
                    coordList.Add(new CoordByLine(lineNr, newLine.figureNumber, (XyzPoint)oldLine.actualPos, (XyzPoint)newLine.actualPos, newLine.motionMode, newLine.alpha, isArc));
                    simuList.Add(new SimuCoordByLine(newLine, new PointF()));         // add parsed line to list
                }

                oldLine = new GcodeByLine(newLine);       		// get copy of newLine      
                gcodeList.Add(new GcodeByLine(newLine));    	// add parsed line to list

                if (updateFigureLineNeeded) // f (line.Contains(XmlMarker.FigureStart)) 
                {
                    if ((XmlMarker.tmpFigure.LineStart >= 0) && (XmlMarker.tmpFigure.LineStart < coordList.Count))
                    {
                        coordList[XmlMarker.tmpFigure.LineStart].actualG = 0;
                        if (xyPosChanged)
                        {
                            updateFigureLineNeeded = false;
                            XmlMarker.tmpFigure.PosStart = (XyPoint)newLine.actualPos;
                            coordList[XmlMarker.tmpFigure.LineStart].actualPos = (XyzPoint)newLine.actualPos;
                            coordList[XmlMarker.tmpFigure.LineStart].alpha = newLine.alpha;
                        }
                    }
                }

                if ((modal.mWord == 30) || (modal.mWord == 2)) { programEnd = true; }
                if ((modal.mWord == 98) && (findSubroutineFailCounter < 10))
                {
                    if (lastSubroutine[0] == modal.pWord)
                    { AddSubroutine(GCode, lastSubroutine[1], lastSubroutine[2], modal.lWord, processSubs); }
                    else
                    {
                        FindAddSubroutine(modal.pWord, GCode, modal.lWord, processSubs);      // scan complete GCode for matching O-word
                        if (lastSubroutine[0] < 0)
                        {
                            findSubroutineFailCounter++;
                            Logger.Error("GetGCodeLines FindAddSubroutine P{0} not found:{1}", modal.pWord, findSubroutineFailCounter);
                            if (findSubroutineFailCounter == 1)
                                errorString += string.Format(" Needed subroutine P{0} not found \r\n", modal.pWord);
                        }
                        //Logger.Trace("GetGCodeLines FindAddSubroutine pWord:{0}  [0]:{1}  subStart:{2}  subEnd:{3}  fail:{4}", modal.pWord, lastSubroutine[0], lastSubroutine[1], lastSubroutine[2], findSubroutineFailCounter);
                    }
                }

                if (isHeaderSection)
                {
                    if (singleLine.Contains("SVG DIMENSION") && singleLine.Contains(":"))
                    {
                        var tmpLine = singleLine.Split(':');
                        var tmpVals = tmpLine[1].Trim().Split(' ');
                        if (double.TryParse(tmpVals[0], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out double parsed1))
                        { clipDimension.X = parsed1; }
                        if (double.TryParse(tmpVals[1], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out double parsed2))
                        { clipDimension.Y = parsed2; }
                        Logger.Trace("----- clipDimension {0} -> {1:0.00} {2:0.00}", singleLine, clipDimension.X, clipDimension.Y);
                    }
                    else if (singleLine.Contains("GRAPHIC OFFSET") && singleLine.Contains(":"))
                    {
                        var tmpLine = singleLine.Split(':');
                        var tmpVals = tmpLine[1].Trim().Split(' ');
                        if (double.TryParse(tmpVals[0], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out double parsed1))
                        { clipOffset.X = parsed1; }
                        if (double.TryParse(tmpVals[1], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out double parsed2))
                        { clipOffset.Y = parsed2; }
                        Logger.Trace("----- clipOffset {0} -> {1:0.00} {2:0.00}", singleLine, clipOffset.X, clipOffset.Y);
                    }
                }

                if (singleLine.Contains("</"))
                {
                    ProcessXmlTagEnd(GCode[lineNr], lineNr);
                }        // Original line transferred because of uppercase lowercase

            }   // finish reading lines
            /*********************************************************************/
            if (!largeDataAmount && (figureMarkerCount > 1) && (showArrow || showId))
            {
                ModifyPenUpPath(worker, e, ref progressSubOld, progressMainFactor, showArrow, showId);
            }
            else
            {
                Logger.Trace("No ModifyPenUpPath largeAmount:{0}  figureCnt:{1}  showArrow:{2}  showId:{3}", largeDataAmount, figureMarkerCount, showArrow, showId);
            }

            // delete zero
            if (Properties.Settings.Default.importImage2DViewHideZero && halfToneEnable) // S or Z are calculated from gray-value - range 0-255
            {
                if (showHalftonePath.ContainsKey(0))                        // if pen-width was used before, continue path
                {
                    int index = showHalftonePath[0];
                    if ((index >= 0) && (index < pathObject.Count))
                    { pathObject[index].path.Reset(); }
                }
            }

            worker?.ReportProgress(100, new MyUserState { Value = 100, Content = "Wait for update of text editor" });

            if ((XmlMarker.figurePenColorNoneCount > 0) && (XmlMarker.figurePenColorAnyCount == XmlMarker.figurePenColorNoneCount))
            {
                pathObjectPenColorOnlyNone = true;
                Logger.Warn("⚠⚠⚠⚠ only PenColor 'none' found - use default color and pen-width (disable color-mode)");
            }

            Logger.Info("△△△△ GetGCodeLines finish  pathObjectPenColorOnlyNone:{0} any:{1}  none:{2}", pathObjectPenColorOnlyNone, XmlMarker.figurePenColorAnyCount, XmlMarker.figurePenColorNoneCount);
            if (figureMarkerCount == 0)
                figureMarkerCount = tileCount;
            return true;
        }

        /// <summary>
        /// Find and add subroutine within given gcode
        /// </summary>
        private static string FindAddSubroutine(int foundP, string[] GCode, int repeat, bool processSubs)
        {
            ModalGroup tmp = new ModalGroup();                      // just temporary use
            GcodeByLine tmpLine = new GcodeByLine();                // just temporary use
            int subStart = 0, subEnd = 0;
            bool foundO = false;
            for (int lineNr = 0; lineNr < GCode.Length; lineNr++)   // go through GCode lines
            {
                tmpLine.ParseLine(lineNr, GCode[lineNr], ref tmp);       // parse line
                if (tmp.oWord == foundP)                            // subroutine ID found?
                {
                    if (!foundO)
                    {
                        subStart = lineNr;
                        foundO = true;
                    }
                    else
                    {
                        if (tmp.mWord == 99)                        // subroutine end found?
                        {
                            subEnd = lineNr;
                            break;
                        }
                    }
                }
            }
            if ((subStart > 0) && (subEnd > subStart))
            {
                AddSubroutine(GCode, subStart, subEnd, repeat, processSubs);    // process subroutine
                lastSubroutine[0] = foundP;
                lastSubroutine[1] = subStart;
                lastSubroutine[2] = subEnd;
            }
            else
            { lastSubroutine[0] = -1; }
            return String.Format("Start:{0} EndX:{1} ", subStart, subEnd);
        }
        private static int[] lastSubroutine = new int[] { 0, 0, 0 };

        /// <summary>
        /// process subroutines
        /// </summary>
        private static void AddSubroutine(string[] GCode, int start, int stop, int repeat, bool processSubs)
        {
            //            Logger.Trace("addSubroutine start:{0}  stop:{1}  repeat:{2} processSubs:{3}",start, stop, repeat, processSubs);
            bool showPath = true;
            bool isArc;
            if ((start >= GCode.Length) || (stop >= GCode.Length))
            {
                Logger.Error("AddSubroutine start:{0}  stop:{1}  GCode.Length:{2}", start, stop, GCode.Length);
                return;
            }
            for (int loop = 0; loop < repeat; loop++)
            {
                for (int subLineNr = start + 1; subLineNr < stop; subLineNr++)      // go through real line numbers and parse sub-code
                {
                    if (GCode[subLineNr].Contains("%START_HIDECODE")) { showPath = false; }
                    if (GCode[subLineNr].Contains("%STOP_HIDECODE")) { showPath = true; }

                    newLine.ParseLine(subLineNr, GCode[subLineNr], ref modal);      // reset coordinates, set lineNumber, parse GCode
                    newLine.isSubroutine = !processSubs;
                    CalcAbsPosition(newLine, oldLine);                              // calc abs position

                    if (!showPath) newLine.ismachineCoordG53 = true;

                    if (processSubs)
                        gcodeList.Add(new GcodeByLine(newLine));      // add parsed line to list
                    simuList.Add(new SimuCoordByLine(newLine, new PointF()));      // add parsed line to list
                    if (!newLine.ismachineCoordG53)
                    {
                        isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                        coordList.Add(new CoordByLine(subLineNr, newLine.figureNumber, (XyzPoint)oldLine.actualPos, (XyzPoint)newLine.actualPos, newLine.motionMode, newLine.alpha, isArc));
                        if (((newLine.motionMode > 0) || (newLine.z != null)) && !((newLine.x == Grbl.posWork.X) && (newLine.y == Grbl.posWork.Y)))
                            xyzSize.SetDimensionXYZ(newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);             // calculate max dimensions
                    }                                                                                                       // add data to drawing path
                    if (showPath)
                        CreateDrawingPathFromGCode(newLine, oldLine, offset2DView);
                    oldLine = new GcodeByLine(newLine);   // get copy of newLine                         
                }
            }
        }

        /// <summary>
        /// Calc. absolute positions and set object dimension: xyzSize.setDimension.
        /// Return if X,Y or Z changed
        /// </summary>
        private static bool CalcAbsPosition(GcodeByLine newLine, GcodeByLine oldLine)
        {
            bool posChanged = false;
            bool calcAbolute = !newLine.ismachineCoordG53 && newLine.isdistanceModeG90;
            bool setDimension = newLine.motionMode >= 1;
            bool isArc = newLine.motionMode >= 2;
            bool posWasSet;

            if (!newLine.ismachineCoordG53)         // only use world coordinates
            {
                if ((newLine.motionMode >= 1) && (oldLine.motionMode == 0))     // take account of last G0 move
                {
                    xyzSize.SetDimensionX(oldLine.actualPos.X);
                    xyzSize.SetDimensionY(oldLine.actualPos.Y);
                }
                else
                {
                    G0Size.SetDimensionX(newLine.actualPos.X);
                    G0Size.SetDimensionY(newLine.actualPos.Y);
                }

                posWasSet = CalcAxisPosition(ref newLine.actualPos.X, newLine.x, oldLine.actualPos.X, calcAbolute);
                if (setDimension && posWasSet) { xyzSize.SetDimensionX(newLine.actualPos.X); }

                posWasSet = CalcAxisPosition(ref newLine.actualPos.Y, newLine.y, oldLine.actualPos.Y, calcAbolute);
                if (setDimension && posWasSet) { xyzSize.SetDimensionY(newLine.actualPos.Y); }

                posWasSet = CalcAxisPosition(ref newLine.actualPos.Z, newLine.z, oldLine.actualPos.Z, calcAbolute);
                if (posWasSet && (newLine.actualPos.Z != Grbl.posWork.Z)) { xyzSize.SetDimensionZ(newLine.actualPos.Z); }

                CalcAxisPosition(ref newLine.actualPos.A, newLine.a, oldLine.actualPos.A, calcAbolute);
                CalcAxisPosition(ref newLine.actualPos.B, newLine.b, oldLine.actualPos.B, calcAbolute);
                CalcAxisPosition(ref newLine.actualPos.C, newLine.c, oldLine.actualPos.C, calcAbolute);

                CalcAxisPosition(ref newLine.actualPos.U, newLine.u, oldLine.actualPos.U, calcAbolute);
                CalcAxisPosition(ref newLine.actualPos.V, newLine.v, oldLine.actualPos.V, calcAbolute);
                CalcAxisPosition(ref newLine.actualPos.W, newLine.w, oldLine.actualPos.W, calcAbolute);

                /* Error 33: When the arc is projected on the selected plane, the distance from the current point to the center 
                   differs from the distance from the end point to the center by more than (.05 inch/.5 mm) OR 
                   ((.0005 inch/.005mm) AND .1% of radius).                    */
                if (isArc) { CheckError33(newLine, oldLine); }
            }
            newLine.alpha = oldLine.alpha;
            if (((XyPoint)oldLine.actualPos).DistanceTo((XyPoint)newLine.actualPos) != 0)
            {
                newLine.alpha = GcodeMath.GetAlpha((XyPoint)oldLine.actualPos, (XyPoint)newLine.actualPos);
                posChanged = true;
            }
            return posChanged;
        }
        private static bool CalcAxisPosition(ref double newActualPos, double? newPos, double oldActualPos, bool modeAbsolute)
        {
            if (newPos != null)
            {
                if (modeAbsolute) { newActualPos = (double)newPos; }
                else { newActualPos = oldActualPos + (double)newPos; }
                return true;
            }
            else
                newActualPos = oldActualPos;
            return false;
        }

        private static void CheckError33(GcodeByLine newLine, GcodeByLine oldLine)
        {
            double i = 0, j = 0;
            if (newLine.i != null) i = (double)newLine.i;
            if (newLine.j != null) j = (double)newLine.j;
            double r = Math.Sqrt(i * i + j * j);
            double centerX = oldLine.actualPos.X + i;
            double centerY = oldLine.actualPos.Y + j;
            double newi = newLine.actualPos.X - centerX;
            double newj = newLine.actualPos.Y - centerY;
            double newr = Math.Sqrt(newi * newi + newj * newj);
            if (Math.Abs(r - newr) > 0.005)
            {   // Note "[" and "]" are used to parse line number in MainFormLoadFile to mark line via MainFormFCTB
                string err1 = string.Format("i,j not ok: r-old:{0:0.000} r-new:{1:0.000}", r, newr);
                string err2 = string.Format(" [{0}] '{1}' would cause error 33: Radii are not equal between r-start:{2:0.000} and r-end:{3:0.000}", (newLine.lineNumber + 1), newLine.codeLine, r, newr);
                string err3 = string.Format(" [{0}] error 33: X:{1:0.000} Y:{2:0.000}  r-start:{3:0.000} r-end:{4:0.000}", (newLine.lineNumber + 1), oldLine.actualPos.X, oldLine.actualPos.Y, r, newr);
                if (error33cnt == 0)
                {
                    errorString += err2 + "\r\n  You may increase decimal places in (Setup Graphics import - G-Code generation) and import the graphic again.\r\n\r\n";
                }
                else
                { errorString += err3 + "\r\n"; }
                Logger.Error("{0}  '{1}' to '{2}'", err3, oldLine.codeLine, newLine.codeLine);
                newLine.codeLine += string.Format("({0})", err1);
                error33cnt++;
            }
        }

        private static void CalculateProcessTime(GcodeByLine newL, GcodeByLine oldL)
        {
            double feed = Math.Min(feedXmax, feedYmax);         // feed in mm/min
            if (newL.z != null)
                feed = Math.Min(feed, feedZmax);                // max feed defines final speed
            if (newL.a != null)
                feed = Math.Min(feed, feedAmax);                // max feed defines final speed
            if (newL.b != null)
                feed = Math.Min(feed, feedBmax);                // max feed defines final speed
            if (newL.c != null)
                feed = Math.Min(feed, feedCmax);                // max feed defines final speed

            double distanceX = Math.Abs(newL.actualPos.X - oldL.actualPos.X);
            double distanceY = Math.Abs(newL.actualPos.Y - oldL.actualPos.Y);
            double distanceXY = Math.Max(distanceX, distanceY);
            double distanceZ = Math.Abs(newL.actualPos.Z - oldL.actualPos.Z);

            if (newL.motionMode > 1)
                distanceXY = newL.distance;     // Arc is calc in createDrawingPathFromGCode

            double distanceAll = Math.Max(distanceXY, distanceZ);

            if (newL.motionMode > 0)
                feed = Math.Min(feed, newL.feedRate);           // if G1,2,3 use set feed

            gcodeDistance += distanceAll;
            gcodeMinutes += distanceAll / feed;
        }

        private static void ProcessXmlTagStart(string line, int lineNr)
        {
            // XML-Tag available!
            /* Process Collection marker */
            if (line.Contains(XmlMarker.CollectionStart))                   // check if marker available
            {
                XmlMarker.AddCollection(lineNr, line, figureMarkerCount);
                //    figureActive = true;
            }
            /* Process Tile marker */
            else if (line.Contains(XmlMarker.TileStart))                   // check if marker available
            {
                XmlMarker.AddTile(lineNr, line, figureMarkerCount);
                if (Properties.Settings.Default.importGraphicClipShowOrigPosition && Properties.Settings.Default.importGraphicClipOffsetApply)
                {
                    offset2DView.X = (float)XmlMarker.tmpTile.Offset.X;
                    offset2DView.Y = (float)XmlMarker.tmpTile.Offset.Y;
                    ShiftTilePaths = true;
                }
                tileActive = true;
                tileCount++;
            }
            /* Process Group marker */
            else if (line.Contains(XmlMarker.GroupStart))                   // check if marker available
            {
                string clean = line.Replace("'", "\"");
                figureMarkerCount++;
                XmlMarker.AddGroup(lineNr, clean, figureMarkerCount);
                figureActive = true;
                if (logCoordinates) { Logger.Trace(" Set Group  figureMarkerCount:{0}  {1}", figureMarkerCount, line); }
                if (XmlMarker.tmpGroup.Layer.ToUpper().Contains(fiducialLabel.ToUpper()))
                {   //fiducialEnable=true; 
                }
            }
            /* Process Figure marker */
            else if (line.Contains(XmlMarker.FigureStart))                  // check if marker available
            {
                if (!figureActive)
                    figureMarkerCount++;
                figureActive = true;
                updateFigureLineNeeded = true;                                  // update coordList.actualPos of this line later on
                xyPosChanged = false;
                string clean = line.Replace("'", "\"");
                XmlMarker.AddFigure(lineNr, clean, figureMarkerCount);
                if (logCoordinates) Logger.Trace(" Set Figure figureMarkerCount:{0}  {1}", figureMarkerCount, line);

                fiducialDimension = new Dimensions();

                if (XmlMarker.tmpFigure.Layer.ToUpper().Contains(fiducialLabel.ToUpper()))
                { fiducialEnable = true; Logger.Trace("◯◯◯ Fiducial found Layer:'{0}'", XmlMarker.tmpFigure.Layer); }
                if (XmlMarker.tmpFigure.PathId.ToUpper().Contains(fiducialLabel.ToUpper()))
                { fiducialEnable = true; Logger.Trace("◯◯◯ Fiducial found PathId:'{0}'", XmlMarker.tmpFigure.PathId); }

                if (Properties.Settings.Default.gui2DColorPenDownModeEnable)// && !largeDataAmount)    // Graphic.SizeOk())    // enable color mode 
                {
                    PathData tmp;
                    if (Properties.Settings.Default.gui2DColorPenDownModeWidth)
                        tmp = new PathData(XmlMarker.tmpFigure.PenColor, XmlMarker.tmpFigure.PenWidth, offset2DView);      // set color, width, pendownpath
                    else
                        tmp = new PathData(XmlMarker.tmpFigure.PenColor, (double)Properties.Settings.Default.gui2DWidthPenDown, offset2DView);      // set color, width, pendownpath

                    pathObject.Add(tmp);
                    pathActualDown = pathObject[pathObject.Count - 1].path;
                }
            }

            else if (line.Contains(XmlMarker.TangentialAxis))
            {
                tangentialAxisEnable = true;
                tangentialAxisName = XmlMarker.GetAttributeValue(line, "Axis");
                if (logEnable) Logger.Trace("Show tangetial axis '{0}'", tangentialAxisName);
            }

            else if (line.Contains(XmlMarker.HalftoneS) || line.Contains(XmlMarker.HalftoneZ))
            {
                if (line.Contains("Min")) { HalfTone.showHalftoneMin = XmlMarker.GetAttributeValueDouble(line, "Min"); }
                if (line.Contains("Max")) { HalfTone.showHalftoneMax = XmlMarker.GetAttributeValueDouble(line, "Max"); }
                if (line.Contains("Width")) { HalfTone.showHalftoneWidth = XmlMarker.GetAttributeValueDouble(line, "Width"); }
                Logger.Info("Display halftone  {0}  {1}  {2}  {3}", line, HalfTone.showHalftoneMin, HalfTone.showHalftoneMax, HalfTone.showHalftoneWidth);

                if (line.Contains(XmlMarker.HalftoneS))
                {
                    HalfTone.showHalftoneS = true; halfToneEnable = true;
                    Logger.Info("showHalftoneS min:{0}  max:{1}  width:{2}", HalfTone.showHalftoneMin, HalfTone.showHalftoneMax, HalfTone.showHalftoneWidth);
                }
                if (line.Contains(XmlMarker.HalftoneZ))
                {
                    HalfTone.showHalftoneZ = true; halfToneEnable = true;
                    Logger.Info("showHalftoneZ min:{0}  max:{1}  width:{2}", HalfTone.showHalftoneMin, HalfTone.showHalftoneMax, HalfTone.showHalftoneWidth);
                }
            }
            else if (line.Contains(XmlMarker.HeaderStart))
            { isHeaderSection = true; }
        }

        private static void ProcessXmlTagEnd(string line, int lineNr)
        {   /* Process Figure end */
            if (line.Contains(XmlMarker.FigureEnd))                    // check if marker available
            {
                figureActive = false;
                XmlMarker.tmpFigure.PosEnd = (XyPoint)newLine.actualPos;
                XmlMarker.FinishFigure(lineNr);
                pathActualDown = null;
                if (fiducialEnable)
                {
                    if (fiducialDimension.IsXYSet())
                    {
                        XyPoint tmpFiducial = fiducialDimension.GetCenter();
                        if (fiducialsCenter.Count > 0)
                        {
                            if (!fiducialsCenter[fiducialsCenter.Count - 1].Equals(tmpFiducial))
                            { fiducialsCenter.Add(fiducialDimension.GetCenter()); } // avoid same coordinates
                        }
                        else
                            fiducialsCenter.Add(fiducialDimension.GetCenter());
                        Logger.Trace("Set fiducial add dim {0} {1} ", fiducialDimension.GetCenter().X, fiducialDimension.GetCenter().Y);
                    }
                }
                fiducialEnable = false;
            }
            /* Process Group end */
            else if (line.Contains(XmlMarker.GroupEnd))                    // check if marker available
            {
                XmlMarker.FinishGroup(lineNr);
                //fiducialEnable = false;
            }
            /* Process Tile end */
            else if (line.Contains(XmlMarker.TileEnd))                    // check if marker available
            { XmlMarker.FinishTile(lineNr); }
            else if (line.Contains(XmlMarker.CollectionEnd))                   // check if marker available
            {
                XmlMarker.FinishCollection(lineNr);
            }
            else if (line.Contains(XmlMarker.HeaderEnd))
            { isHeaderSection = false; }
        }

        private static void ProcessHalftoneData()
        {
            double width = HalfTone.CalcWidth();

            if (width >= 0)
            {
                if (showHalftonePath.ContainsKey(width))                        // if pen-width was used before, continue path
                {
                    int index = showHalftonePath[width];
                    if ((index >= 0) && (index < pathObject.Count))
                    { pathActualDown = pathObject[index].path; }
                }
                else
                {
                    PathData tmp = new PathData(XmlMarker.tmpFigure.PenColor, width, offset2DView);      // set color, width, pendownpath                                                                                                                           
                    pathObject.Add(tmp);
                    pathActualDown = pathObject[pathObject.Count - 1].path;
                    showHalftonePath.Add(width, pathObject.Count - 1);          // store new pen-width with according index
                }
                pathActualDown.StartFigure();
            }
            HalfTone.showHalftoneLastS = HalfTone.lastS;
            HalfTone.showHalftoneLastZ = HalfTone.lastZ;
        }

        private static void ModifyPenUpPath(BackgroundWorker worker, DoWorkEventArgs e, ref int progressSubOld, double progressMainFactor, bool showArrow, bool showId)
        {
            worker?.ReportProgress(0, new MyUserState { Value = 50, Content = "Pen-up path: Add dir-arrows and figure-Ids" });
            int count = 0;
            if ((pathInfoMarker != null) && (pathInfoMarker.Any()))
            {
                Logger.Info("🡷🡴🡵🡶 ModifyPenUpPath - add arrows and ids - count:{0}", pathInfoMarker.Count);
                foreach (PathInfo tmp in pathInfoMarker)
                {
                    if (worker != null)
                    {
                        int progressSub = (count++ * 100) / pathInfoMarker.Count;
                        int progressMain = (int)(50 + progressSub * progressMainFactor);
                        if (progressSub != progressSubOld)
                        {
                            worker.ReportProgress(progressSub, new MyUserState { Value = progressMain, Content = "Pen-up path: Add dir-arrows and figure-Ids " + progressSub + " %" });
                            progressSubOld = progressSub;
                        }
                        if (worker.CancellationPending)
                        {
                            if (e != null)
                                e.Cancel = true;
                            break;
                        }
                    }
                    AddArrow(pathPenUp, tmp, showArrow, showId);
                }
            }
            else
            {
                Logger.Error("ModifyPenUpPath - pathInfoMarker is null or empty, can't add arrows and ids to PenUp path");
            }
        }

        public static string GetParserState(int lineNr)
        {
            if ((lineNr < 0) || (lineNr >= gcodeList.Count))
            { Logger.Error("GetParserState lineNr:{0}", lineNr); return ""; }

            string state = "";
            for (int i = 0; 1 < gcodeList.Count; i++)
            {
                if (gcodeList[i].lineNumber == lineNr)  // <Parser State="G1 G54 G17 G21 G90 G94 M3 M9 T0 F1000 S10000" />
                {
                    state += string.Format("G{0} G{1} G{2} ", gcodeList[i].motionMode, gcodeList[i].coordSystem, gcodeList[i].planeSelect);
                    state += string.Format("G{0} G{1} G{2} ", (gcodeList[i].isunitModeG21 ? "21" : "20"), (gcodeList[i].isdistanceModeG90 ? "90" : "91"), (gcodeList[i].isfeedrateModeG94 ? "94" : "93"));
                    state += string.Format("M{0} M{1} ", gcodeList[i].spindleState, gcodeList[i].coolantState);
                    state += string.Format("T{0} F{1} S{2}", gcodeList[i].toolNumber, gcodeList[i].feedRate, gcodeList[i].spindleSpeed);
                    break;
                }
            }
            return state;
        }
        public static XyzPoint GetActualPosition(int lineNr)
        {
            if ((lineNr < 0) || (lineNr >= gcodeList.Count))
            { Logger.Error("GetActualPosition lineNr:{0}", lineNr); return new XyzPoint(); }

            for (int i = 0; i < gcodeList.Count; i++)
            {
                if (gcodeList[i].lineNumber == lineNr)
                {
                    return (XyzPoint)gcodeList[i].actualPos;
                    //    break;
                }
            }
            return new XyzPoint();
        }

    }
}
