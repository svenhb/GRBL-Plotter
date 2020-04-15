/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2020 Sven Hasemann contact: svenhb@web.de

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
 * 2019-09-06 new class for high level commands
 * 2019-11-28 add penup in line 180
 * 2019-11-30 add line 381- Arc added code - for DXF circle multiple pass 
 * 2020-01-01 add public enum xmlMarkerType
 * 2020-01-10 add Use-case to output line 501
 * 2020-02-18 add tangential axis support (doesn't work with 'repeatZ' because of inserted PenUp/Down to process swivel angle)
 * 2020-02-28 remove empty figure sections FigureCheck[] lastFigureStart
 * 2020-04-04 replace ArcToCCW
 * 2020-04-09 extend class xmlMarker
 * 2020-04-11 fix splitting problem for attributes in element-string (if value contains ' ')
 * 2020-04-13 add splitArc to support tangential axis
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GRBL_Plotter
{
    public static class Plotter
    {
        private static bool loggerTrace = false;

        struct FigureCheck      // collect data of last created code for removement if needed
        {   public int lastIndexStart;
            public int lastIndexEnd;
            public int codeLength;
            public int figureNr;
        };

        private const int gcodeStringMax = 260;                 // max amount of tools
        private static int gcodeStringIndex = 0;                // index for stringBuilder-Array
        private static int gcodeStringIndexOld = 0;             // detect change in index
        private static Dictionary<string,int> stringToIndex = new Dictionary<string,int>();   // Translate string to gcodeStringIndex
        private static int stringToIndexIndex = 1;
        private static string stringToIndexString = "";

        private static StringBuilder[] gcodeString = new StringBuilder[gcodeStringMax];
        private static StringBuilder finalGcodeString = new StringBuilder();
        private static FigureCheck[] lastFigureStart = new FigureCheck[gcodeStringMax];
        private static Dimensions[] gcodeDimension = new Dimensions[gcodeStringMax];

        private static bool penIsDown = false;
        private static bool comments = false;
        private static bool pauseBeforePath = false;
        private static bool pauseBeforePenDown = false;
        private static bool groupObjects = false;
        private static bool groupByColor = true;
        private static int sortOption = 0;
        private static bool sortInvert = false;
        private static int amountOfTools = 0;
        private static bool gcodeUseSpindle = false;
        private static bool gcodeReduce = false;            // if true remove G1 commands if distance is < limit
        private static float gcodeReduceVal = .1f;          // limit when to remove G1 commands
        private static int lastSetGroup = -1;
        private static bool gcodeTangEnable = false;
        private static string gcodeTangName = "C";
        private static double gcodeTangTurn = 360;
        private static bool gcodeNoArcs = false;

        private static Point lastGC, lastSetGC;             // store last position
		private static bool isStartPathIsPending = false;
		private static Point posStartPath;
        private static double posStartAngle = 0;

        public static int PathCount { get; set; } = 0;
        public static int PathToolNr { get; set; } = 0;
        public static bool IsPathReduceOk { get; set; } = false;
        public static bool IsPathAvoidG23 { get; set; } = false;
        public static bool IsPathFigureEnd { get; set; } = true;
        public static string PathId { get; set; } = "";
        public static string PathName { get; set; } = "";
        public static string Geometry { get; set; } = "";
        private static string pathColor = "";
        public static string PathColor
        {   get {   return pathColor; }
            set {   pathColor = value;
                    if ((pathColor.Length == 3) && (System.Text.RegularExpressions.Regex.IsMatch(pathColor, @"\A\b[0-9a-fA-F]+\b\Z")))
                    {   char[] tmp = new char[6]; 
                        tmp[0] = tmp[1] = pathColor[0];
                        tmp[2] = tmp[3] = pathColor[1];
                        tmp[4] = tmp[5] = pathColor[2];
                        pathColor = new string(tmp);
                    }
                }
        }
        public static string PathComment { get; set; } = "";
        public static double[] PathDashArray { get; set; } = { };

        public static string DocTitle { get; set; } = "";
        public static string DocDescription { get; set; } = "";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void StartCode()
        {
            Logger.Trace("startCode()");
            pauseBeforePath = Properties.Settings.Default.importPauseElement;
            pauseBeforePenDown = Properties.Settings.Default.importPausePenDown;
            groupObjects = Properties.Settings.Default.importGroupObjects;           // DXF-Import group objects
            groupByColor = Properties.Settings.Default.importGroupByColor;           // DXF-Import group objects
            sortOption = Properties.Settings.Default.importGroupSort;                // SVG-Import sort by tool
            sortInvert = Properties.Settings.Default.importGroupSortInvert;
            gcodeUseSpindle = Properties.Settings.Default.importGCZEnable;
            gcodeReduce = Properties.Settings.Default.importRemoveShortMovesEnable;
            gcodeReduceVal = (float)Properties.Settings.Default.importRemoveShortMovesLimit;
            gcodeTangEnable = Properties.Settings.Default.importGCTangentialEnable;
            gcodeTangName   = Properties.Settings.Default.importGCTangentialAxis;
            gcodeTangTurn = (double)Properties.Settings.Default.importGCTangentialTurn;
            gcodeNoArcs = Properties.Settings.Default.importGCNoArcs;        // reduce code by 
            comments = Properties.Settings.Default.importSVGAddComments;
            lastSetGroup = -1;
            penIsDown = false;

            gcodeMath.resetAngles();
            posStartAngle = 0;

            isStartPathIsPending = false;
			
            gcodeStringIndex = 0;
            gcodeStringIndexOld = -1;
            stringToIndex.Clear();
            stringToIndexIndex = 1;

            for (int i = 0; i < gcodeStringMax; i++)        // hold gcode snippes for later sorting
            {   gcodeString[i] = new StringBuilder();
                gcodeString[i].Clear();
                gcodeDimension[i] = new Dimensions();
            }
            finalGcodeString.Clear();

            PathCount = 0;
            PathToolNr  = 0;
            IsPathReduceOk  = false;
            IsPathAvoidG23  = false;
            IsPathFigureEnd  = true;
            PathId  = "";
            PathName = "";
            Geometry = "";
            pathColor  = "";
            PathComment  = "";
            DocTitle = "";
            DocDescription = "";

            amountOfTools = toolTable.init();
            gcode.setup();                              // initialize GCode creation (get stored settings for export)

            if (!groupObjects)       // Load initial tool
            {   toolProp tmpTool = toolTable.getToolProperties((int)Properties.Settings.Default.importGCToolDefNr);
                gcode.Tool(gcodeString[gcodeStringIndex], tmpTool.toolnr, tmpTool.name);    // add tool change commands (if enabled) and set XYFeed etc.
            }
            else
            { }
        }

        /// <summary>
        /// Set start tag, move to beginning of path via G0, finish old path
        /// </summary>
        public static void StartPath(Point coordxy, string cmt)
        {
            if (!comments) { cmt = ""; }

            if (loggerTrace) Logger.Trace(" StartPath at X:{0:0.000} Y:{1:0.000}, old X:{2:0.000} Y:{3:0.000} strIndex:{4} strIndexOld:{5} {6}", coordxy.X, coordxy.Y, lastGC.X, lastGC.Y, gcodeStringIndex, gcodeStringIndexOld, cmt);

/* only if change in position, do Pen-up, G0-Move, Pen-down  */
            if ((gcodeStringIndex != gcodeStringIndexOld) || !gcodeMath.isEqual(lastGC,coordxy))    // only if change in position, do pen-up -down
            {
                PenUp(cmt+ " in  StartPath");
                if (!IsPathFigureEnd)
                {   SetFigureEndTag(PathCount);  }
                IsPathFigureEnd = true;

                string attributeGeometry = (Geometry.Length > 0) ? string.Format(" Geometry=\"{0}\"", Geometry) : "";
                string attributeId = (PathId.Length > 0) ? string.Format(" Id=\"{0}\"", PathId) : "";
                string attributeColor = (pathColor.Length > 0) ? string.Format(" Color=\"#{0}\"", pathColor) : "";
                string attributeToolNr = string.Format(" ToolNr=\"{0}\"", PathToolNr);

/* set XML comment (<Figure...  */
                string xml = string.Format("{0} Id=\"{1}\"{2}{3}{4}{5}> ", xmlMarker.figureStart, (++PathCount), attributeGeometry, attributeId, attributeColor, attributeToolNr);
                lastFigureStart[gcodeStringIndex].lastIndexStart = gcodeString[gcodeStringIndex].Length;
                lastFigureStart[gcodeStringIndex].figureNr = PathCount;
                Comment(xml);
                lastFigureStart[gcodeStringIndex].lastIndexEnd = gcodeString[gcodeStringIndex].Length;
                lastFigureStart[gcodeStringIndex].codeLength = lastFigureStart[gcodeStringIndex].lastIndexEnd - lastFigureStart[gcodeStringIndex].lastIndexStart;
                if (loggerTrace) Logger.Trace(" {0}", xml);

                if (comments && (PathName.Length > 0)) { Comment(PathName); }

                if (pauseBeforePath && !pauseBeforePenDown) { InsertPause("Pause before path"); }
                IsPathFigureEnd = false;

                gcodeMath.cutAngle = gcodeMath.getAngle(lastGC, coordxy, 0, 0); // get and store position
                isStartPathIsPending=true;                  // and angle of desired
                posStartPath = coordxy;                     // start-point
                posStartAngle = gcodeMath.cutAngle;                   // Apply G0 on Pen-down, when needed (in Arc or MoveTo)
                if (loggerTrace) Logger.Trace("  StartPath get angle for x{0:0.000} y{1:0.000} a={2:0.00}", coordxy.X, coordxy.Y,180* posStartAngle/Math.PI);
            }
            lastGC = coordxy;
            lastSetGC = coordxy;
            IsPathReduceOk = false;
            gcodeStringIndexOld = gcodeStringIndex;
        }

        public static void SetFigureEndTag(int nr)
        {
            if (gcodeString[gcodeStringIndex].Length == (lastFigureStart[gcodeStringIndex].lastIndexEnd))     // no code generated
            {   gcodeString[gcodeStringIndex].Remove(lastFigureStart[gcodeStringIndex].lastIndexStart, lastFigureStart[gcodeStringIndex].codeLength);
                if (loggerTrace) Logger.Trace("Code removed figure {0}", lastFigureStart[gcodeStringIndex].figureNr);
            }
            else
            { string xml = string.Format("{0}>", xmlMarker.figureEnd);//, nr);    //string.Format("{0} nr=\"{1}\" >", xmlMarker.figureEnd, nr);
                Comment(xml);
                if (loggerTrace) Logger.Trace(" {0}", xml);
            }
        }

        /// <summary>
        /// Finish path
        /// </summary>
        public static void StopPath(string cmt)
        {
            if (loggerTrace) Logger.Trace("  StopPath {0}",cmt);

            if (gcodeReduce)
            {   if (loggerTrace) Logger.Trace("   StopPath get angle");
                gcodeMath.cutAngle = getAngle(lastSetGC,lastGC,0,0);

                if (!gcodeMath.isEqual(lastSetGC, lastGC))        //(lastSetGC.X != lastGC.X) || (lastSetGC.Y != lastGC.Y)) // restore last skipped point for accurat G2/G3 use
                {
                    if (loggerTrace) Logger.Trace("   StopPath get angle - restore point");
                    gcodeMath.cutAngle = getAngle(lastGC, lastSetGC, 0, 0);
                    processTangentialAxis(gcodeMath.cutAngleLast, gcodeMath.cutAngle);
                    MoveToDashed(lastGC, cmt);
                    lastSetGC = lastGC;
                }
            }
            PenUp(cmt + " Stop path"); 
        }

        /// <summary>
        /// Move to next coordinate
        /// </summary>
        public static void MoveTo(Point coordxy, string cmt)
        {
            if (gcodeMath.isEqual(lastGC,coordxy))        // nothing to do
                return;
            bool rejectPoint = false;

            if (gcodeReduce && IsPathReduceOk)
            {
                double distance = gcodeMath.distancePointToPoint(coordxy, lastSetGC);
                if (distance < gcodeReduceVal)      // discard actual G1 movement
                {   rejectPoint = true;  }
            }
            if (!gcodeReduce || !rejectPoint)       // write GCode
            {
				if (loggerTrace) Logger.Trace(" MoveTo get angle p1 {0:0.000};{1:0.000}  p2 {2:0.000};{3:0.000}", lastSetGC.X, lastSetGC.Y, coordxy.X, coordxy.Y);
                gcodeMath.cutAngle = getAngle(lastSetGC, coordxy, 0, 0);
                fixAngleExceed();
                posStartAngle = gcodeMath.cutAngle;

                PenDown(cmt + " moveto");                           // also process tangetial axis
                if (loggerTrace) Logger.Trace(" MoveTo X{0:0.000} Y{1:0.000}", coordxy.X, coordxy.Y);
                gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
                gcodeMath.cutAngleLast = gcodeMath.cutAngle;
                MoveToDashed(coordxy, cmt);
                lastSetGC = coordxy;
            }
            lastGC = coordxy;
        }

        public static void MoveToSimple(xyPoint coordxy, string cmt, bool rapid = false)
        {   MoveToSimple(new Point(coordxy.X, coordxy.Y), cmt, rapid); }
        public static void MoveToSimple(Point coordxy, string cmt, bool rapid=false)
        {
            if (loggerTrace) Logger.Trace(" MoveToSimple X{0:0.000} Y{1:0.000} rapid {2}", coordxy.X, coordxy.Y,rapid);

            gcodeMath.cutAngle = gcodeMath.getAngle(lastGC, coordxy, 0, 0); // get and store position
            fixAngleExceed();
            if (rapid)
            {
                isStartPathIsPending = true;                  // and angle of desired
                posStartPath = coordxy;                     // start-point
                posStartAngle = gcodeMath.cutAngle;                   // Apply G0 on Pen-down, when needed (in Arc or MoveTo 
            }
            else
            {
                posStartAngle = gcodeMath.cutAngle;
                PenDown(cmt + " movetosimple");                           // also process tangetial axis
                gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
                gcodeMath.cutAngleLast = gcodeMath.cutAngle;
                MoveTo(coordxy, cmt);  //gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt);
            }

            lastSetGC = coordxy;
            lastGC = coordxy;
        }

        private static void MoveToDashed(Point coordxy, string cmt)
        {
            if (loggerTrace) Logger.Trace("  MoveToDashed X{0:0.000} Y{1:0.000}", coordxy.X, coordxy.Y);

            bool showDashInfo = false;
            string dashInfo = "";

            gcodeDimension[gcodeStringIndex].setDimensionXY(coordxy.X, coordxy.Y);

            if (!Properties.Settings.Default.importLineDashPattern || (PathDashArray.Length <= 1))
            {   gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); }
            else
            {
                bool penUpG1 = !Properties.Settings.Default.importLineDashPatternG0;
                double dX = coordxy.X - lastGC.X;
                double dY = coordxy.Y - lastGC.Y;
                double xx = lastGC.X, yy = lastGC.Y, dd ;
                int i = 0;
                int save = 1000;
                if (dX == 0)
                {
                    if (dY > 0)
                    {
                        while (yy < coordxy.Y)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenDown("MoveToDashed");
                            dd = PathDashArray[i++];
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            yy += dd;
                            if (yy < coordxy.Y)
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), dashInfo); }
                            else
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp("MoveToDashed", false);
                            dd = PathDashArray[i++];
                            yy += dd;
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (yy < coordxy.Y)
                            {   if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), dashInfo, true);
                                else         gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), dashInfo);
                            }
                            else
                            {   if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt,true);
                                else         gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 3"); break; }
                        }
                    }
                    else
                    {
                        while (yy > coordxy.Y)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            yy -= PathDashArray[i++];
                            PenDown("MoveToDashed");
                            if (yy > coordxy.Y)
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), cmt); }
                            else
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp("MoveToDashed", false);
                            yy -= PathDashArray[i++];
                            if (yy > coordxy.Y)
                            {   if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), cmt, true);
                                else         gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), cmt);
                            }
                            else
                            {   if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt, true);
                                else         gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 4"); break; }
                        }
                    }
                }
                else
                {   double dC = Math.Sqrt(dX * dX + dY * dY);
                    double fX = dX / dC;        // factor X
                    double fY = dY / dC;
                    if (dX > 0)
                    {
                        while (xx < coordxy.X)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            PenDown("");
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx < coordxy.X)
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo); }
                            else
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            PenUp("", false);
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx < coordxy.X)
                            {   if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo, true);
                                else         gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo);
                            }
                            else
                            {   if (penUpG1) gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt, true);
                                else         gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 1"); break; }
                        }
                    }
                    else
                    {
                        while (xx > coordxy.X)
                        {
                            if (i >= PathDashArray.Length)
                                i = 0;
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            PenDown("");
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx > coordxy.X)
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo); }
                            else
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp("", false);
                            dd = PathDashArray[i++];
                            xx += fX * dd;
                            yy += fY * dd;
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            if (xx > coordxy.X)
                            {   if (penUpG1)    gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo);
                                else            gcode.MoveToRapid(gcodeString[gcodeStringIndex], new Point(xx, yy), dashInfo);
                            }
                            else
                            {   if (penUpG1)    gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt);
                                else            gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
                                break;
                            }
                            if (save-- < 0) { Comment("break up dash 2"); break; }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Draw arc
        /// </summary>
        public static void ArcToCCW(Point coordxy, Point coordij, string cmt)
        {
            Arc(3, (float)coordxy.X, (float)coordxy.Y, (float)coordij.X, (float)coordij.Y, cmt);
            return;
        }
        public static void Arc(int gnr, float x, float y, float i, float j, string cmt = "", bool avoidG23 = false)
        {
            Point coordxy = new Point(x,y);
			Point center = new Point(lastGC.X + i, lastGC.Y + j);
            double offset = +Math.PI / 2;
            if (loggerTrace) Logger.Trace("  Start Arc G{0} X{1:0.000} Y{2:0.000} cx{3:0.000} cy{4:0.000} ", gnr, x, y, center.X,center.Y);
            if (gnr > 2) { offset = -offset; }

            if (gcodeReduce && IsPathReduceOk)      			// restore last skipped point for accurat G2/G3 use
            {   if (!gcodeMath.isEqual(lastSetGC, lastGC)) 
                {
                    if (loggerTrace) Logger.Trace("   gcodeReduce MoveTo X{0:0.000} Y{1:0.000}", lastGC.X, lastGC.Y);
                    gcodeMath.cutAngle = getAngle(lastSetGC, lastGC, 0, 0);
                    fixAngleExceed();
                    posStartAngle = gcodeMath.cutAngle;
                    gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
                    gcodeMath.cutAngleLast = gcodeMath.cutAngle;
                    MoveToDashed(lastGC, cmt);
                }
            }

            gcodeMath.cutAngle = getAngle(lastGC, center, offset, 0);       // start angle
            fixAngleExceed();
            posStartAngle = gcodeMath.cutAngle;
            if (loggerTrace) Logger.Trace("   Start Arc alpha{0:0.000} offset{1:0.000}  ", 180 * gcodeMath.cutAngle / Math.PI, 180 * offset / Math.PI);

            PenDown(cmt + " from Arc");

            gcodeMath.cutAngle = getAngle(coordxy, center, offset, gnr);  // end angle
            if (gcodeMath.isEqual(coordxy,lastGC))				// end = start position? Full circle!
            {   if (gnr > 2)
                    gcodeMath.cutAngle += 2 * Math.PI;        			// CW 360°
                else
                    gcodeMath.cutAngle -= 2 * Math.PI;        			// CCW 360°
            }

            setG2Dimension(gnr, x, y, i, j);
            if (gcodeNoArcs || avoidG23)
            {
                if (loggerTrace) Logger.Trace("   Replace Arc by MoveTo");
                Comment("Replace Arc by MoveTo");
                splitArc(gnr, x, y, i, j, cmt); }
            else
            {   gcode.setTangential(gcodeString[gcodeStringIndex], 180 * gcodeMath.cutAngle / Math.PI);
                gcode.Arc(gcodeString[gcodeStringIndex], gnr, x, y, i, j, cmt, avoidG23);
            }
            gcodeMath.cutAngleLast = gcodeMath.cutAngle;

            lastSetGC = coordxy;
            lastGC = coordxy;
        }

        public static void splitArc(int gnr, float x2, float y2, float i, float j, string cmt = "")
        {
            float segmentLength = (float)Properties.Settings.Default.importGCLineSegmentLength;
            float gcodeAngleStep = (float)Properties.Settings.Default.importGCSegment;
            if (loggerTrace) Logger.Trace("   splitArc gnr:{0} segmentLength:{1:0.000} gcodeAngleStep:{2:0.000} ", gnr, segmentLength, gcodeAngleStep);

            ArcProperties arcMove;
            xyPoint p1 = new xyPoint(lastSetGC);
            xyPoint p2 = new xyPoint(x2, y2);
            p1.Round(); p2.Round();
            if (loggerTrace) Logger.Trace("   splitArc P1:{0};{1}  P2:{2};{3}  i:{4}  j:{5}", p1.X, p1.Y, p2.X, p2.Y, i, j);
            arcMove = gcodeMath.getArcMoveProperties( p1, p2, i, j, (gnr == 2));
            double step = Math.Asin(gcodeAngleStep / arcMove.radius);     // in RAD
            if (step > Math.Abs(arcMove.angleDiff))
                step = Math.Abs(arcMove.angleDiff / 2);

            if (loggerTrace) Logger.Trace("   splitArc arcMove.angleStart:{0:0.000} arcMove.angleEnd:{1:0.000} arcMove.angleDiff:{2:0.000} step:{2:0.000} ", arcMove.angleStart, arcMove.angleEnd, arcMove.angleDiff, step);

            if (arcMove.angleDiff > 0)   //(da > 0)                                             // if delta >0 go counter clock wise
            {
                for (double angle = (arcMove.angleStart + step); angle < (arcMove.angleStart + arcMove.angleDiff); angle += step)
                {
                    double x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                    double y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                    MoveTo(new Point(x, y), "");
                }
            }
            else                                                       // else go clock wise
            {
                for (double angle = (arcMove.angleStart - step); angle > (arcMove.angleStart + arcMove.angleDiff); angle -= step)
                {
                    double x = arcMove.center.X + arcMove.radius * Math.Cos(angle);
                    double y = arcMove.center.Y + arcMove.radius * Math.Sin(angle);
                    MoveTo(new Point(x,y), "");
                }
            }
            MoveTo(new Point(x2, y2), "End Arc conversion");
        }


        private static void setG2Dimension(int gnr, double x, double y, double i, double j)
        { setG2Dimension(gnr, new Point(x, y), new Point(i, j)); }
        private static void setG2Dimension(int gnr, Point xy, Point ij)
        {
            ArcProperties arcMove;
            xyPoint p1 = new xyPoint(lastSetGC);
            xyPoint p2 = new xyPoint(xy);
            p1.Round(); p2.Round();
            arcMove = gcodeMath.getArcMoveProperties(p1, p2, ij.X, ij.Y, (gnr == 2));   // 2020-04-14 add round()

            float x1 = (float)(arcMove.center.X - arcMove.radius);
            float x2 = (float)(arcMove.center.X + arcMove.radius);
            float y1 = (float)(arcMove.center.Y - arcMove.radius);
            float y2 = (float)(arcMove.center.Y + arcMove.radius);
            float r2 = 2 * (float)arcMove.radius;
            float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
            float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);
            gcodeDimension[gcodeStringIndex].setDimensionCircle(arcMove.center.X, arcMove.center.Y, arcMove.radius, aStart, aDiff);        // calculate new dimensions
        }
        private static void processTangentialAxis(double angleOld, double angleNew)
        {   if (gcodeTangEnable)
            {
                double angleDiff = 180*Math.Abs(angleNew-angleOld)/Math.PI;
                double swivelAngle = (double)Properties.Settings.Default.importGCTangentialAngle;
         /*       bool rangeExceed = false;
                if (Properties.Settings.Default.importGCTangentialRange)
                {   if ((angleNew < 0) || (angleNew > (2 * Math.PI)))
                        rangeExceed = true;
                }*/
                if ((angleDiff > swivelAngle) || fixAngleExceed())//rangeExceed)
                {   // do pen up, turn, pen down
                    if (penIsDown)
                    {
                        string cmt = "";
                        if (comments) { cmt = "Tangential axis PenUp"; }
                        bool tmp = gcode.RepeatZ;			
                        gcode.RepeatZ = false;				// doesn't solve the problem with repeatZ
                        if (loggerTrace) Logger.Trace("processTangentialAxis PenUp {0}:{1:0.00} offset:{2:0.00}", gcodeTangName, 180 * angleOld / Math.PI, swivelAngle, 180 * gcodeMath.angleOffset / Math.PI, swivelAngle);
                 /*       if (rangeExceed)
                        {   if (angleNew < 0)
                            { gcodeMath.cutAngle = angleNew += 2 * Math.PI; gcodeMath.angleOffset += 2 * Math.PI; }
                            else if (angleNew > (2 * Math.PI))
                            { gcodeMath.cutAngle = angleNew -= 2 * Math.PI; gcodeMath.angleOffset -= 2 * Math.PI; }
                        }*/
                        gcode.PenUp(gcodeString[gcodeStringIndex], cmt);
                        gcodeString[gcodeStringIndex].AppendFormat("G00 {0}{1:0.000} (a > {2:0.0})\r\n", gcodeTangName, (gcodeTangTurn/2) * angleNew / Math.PI, swivelAngle);
                        if (comments) { cmt = "Tangential axis PenDown"; }
                        if (loggerTrace) Logger.Trace("processTangentialAxis PenDown {0}:{1:0.00} offset:{2:0.00}", gcodeTangName, 180 * angleNew / Math.PI, swivelAngle, swivelAngle, 180 * gcodeMath.angleOffset / Math.PI, swivelAngle);
                        gcode.PenDown(gcodeString[gcodeStringIndex], cmt);
                        gcodeMath.cutAngleLast = angleNew;
                        gcode.RepeatZ = tmp;
                    }
                }
                else
                {   // just turn
                    if (angleDiff != 0)
                    {
                        string tmp = "";
                        if (comments)
                            tmp = " (Tangential axis)";
                        gcodeString[gcodeStringIndex].AppendFormat("G01 {0}{1:0.000}{2}\r\n", gcodeTangName, (gcodeTangTurn/2) * angleNew / Math.PI, tmp);
                        gcodeMath.cutAngleLast = angleNew;
                    }
                }
            }                
        }
        private static bool fixAngleExceed()
        {   if (Properties.Settings.Default.importGCTangentialRange)
            {   if (gcodeMath.cutAngle < 0)
                { gcodeMath.cutAngle += 2 * Math.PI; gcodeMath.angleOffset += 2 * Math.PI; return true; }
                else if (gcodeMath.cutAngle > (2 * Math.PI))
                { gcodeMath.cutAngle -= 2 * Math.PI; gcodeMath.angleOffset -= 2 * Math.PI; return true; }
            }
            return false;
        }
        private static double getAngle(Point a, Point b, double offset, int dir)
        {   if (!gcodeTangEnable)
				return 0;
			double w = gcodeMath.getAngle(a, b, offset, dir);			//monitorAngle(gcodeMath.getAlpha(a, b) + offset, dir);
//            if (loggerTrace) Logger.Trace("   getAngle p1 {0:0.000};{1:0.000}  p2 {2:0.000};{3:0.000} a{4:0.00}", a.X, a.Y, b.X, b.Y, 180*w/Math.PI);
            return w;
        }

        /// <summary>
        /// Insert Start, End code, sort indexed code inbetween
        /// </summary>
        public static void SortCode()
        {
            gcode.jobStart(finalGcodeString, "StartJob");
            Logger.Trace("SortCode() group:{0} byColor:{1}", groupObjects, groupByColor);

            #region sort
            if (groupObjects)
            {
                string tmp = "toolnr ";
                int toolnr;
                for (int i = 1; i < amountOfTools; i++)     // get code-size information
                {
                    toolTable.setIndex(toolTable.getIndexByToolNr(i));      // set index in svgPalette
                    toolnr = toolTable.indexToolNr();                       // get value from set index
                    if (toolnr >= 0)
                    {   toolTable.indexSetCodeSize(gcodeString[toolnr].Length);
                        toolTable.indexSetCodeDimension(gcodeDimension[toolnr].getArea());
                    }
                    tmp += toolnr.ToString() + "  ";
                }

                if (sortOption == 1)
                    toolTable.sortByToolNr(sortInvert);
                else if (sortOption == 2)
                    toolTable.sortByCodeSize(sortInvert);
                else if (sortOption == 3)
                    toolTable.sortByCodeDim(sortInvert);

                finalGcodeString.AppendLine();
                int groupnr = 0;
                bool useDefTool = Properties.Settings.Default.importGCToolTableUse && Properties.Settings.Default.importGCToolDefNrUse;
                int useDefToolNr = (int)Properties.Settings.Default.importGCToolDefNr;
                int toolUse = 0;
                string toolNrText = "";
                string toolName = "";
                string toolLayer = "";
                string codeSize = "";
                string codeDimension = "";
                for (int i = 0; i < gcodeStringMax; i++) 
                {
                    toolTable.setIndex(i);                  // set index in svgPalette
                    toolnr = toolTable.indexToolNr();       // get value from set index
                    toolUse = useDefTool ? useDefToolNr : toolnr;
                    toolNrText = " ToolNr=\"" + toolUse+ "\"";
                    toolName = useDefTool ? " ToolName=\"Default\"" : " ToolName=\"" + toolTable.indexName()+ "\"";
                    codeSize = " CodeSize=\""+ toolTable.indexCodeSize() + "\"";
                    codeDimension = " CodeArea=\"" + Math.Round(toolTable.indexCodeDimension()) + "\"";
                    if (!groupByColor)
                    {   toolLayer = " Layer=\"" + stringToIndex.FirstOrDefault(x => x.Value == toolnr).Key + "\"";
                        toolName = ""; toolNrText = "";
                    }
                    if ((toolnr >= 0) && (gcodeString[toolnr].Length > 1))
                    {   
                        gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\"{2}{3}{4}{5}{6}>", xmlMarker.groupStart, ++groupnr, toolLayer, toolNrText, toolName, codeSize, codeDimension));
                        gcode.Tool(finalGcodeString, toolUse, toolName); // add tool change commands (if enabled) and set XYFeed etc.
                        finalGcodeString.Append(gcodeString[toolnr]);
                        gcode.Comment(finalGcodeString, xmlMarker.groupEnd + ">"); //+ " " + groupnr + ">");
                        gcodeString[toolnr].Clear();            // don't append a 2nd time
                    }
                }
                if (groupByColor)
                { toolName = useDefTool ? " ToolName=\"Default\"" : " ToolName=\"not in tool table\""; toolLayer = ""; }
                else
                { toolLayer = " Layer=\"not set\""; toolName = ""; }

                for (int i = 0; i < gcodeStringMax; i++)  
                {
                    if (gcodeString[i].Length > 1)
                    {
                        gcode.Comment(finalGcodeString, string.Format("{0} Id=\"{1}\" ToolNr=\"{2}\" {3}{4}>", xmlMarker.groupStart, ++groupnr, useDefToolNr, toolName, toolLayer));
                        gcode.Tool(finalGcodeString, useDefToolNr, toolName); // add tool change commands (if enabled) and set XYFeed etc.
                        finalGcodeString.Append(gcodeString[i]);
                        gcode.Comment(finalGcodeString, xmlMarker.groupEnd + ">"); //+ " " + groupnr + ">");
                        gcodeString[i].Clear();         // don't append a 2nd time
                    }
                }
            }
            else
                finalGcodeString.Append(gcodeString[0]);
            #endregion

            gcode.jobEnd(finalGcodeString, "EndJob");      // Spindle / laser off
        }

        /// <summary>
        /// set new index for code
        /// </summary>
        public static void SetGroup(string grp)
        {
            stringToIndexString = grp;
            if(stringToIndex.ContainsKey(grp))
            {   SetGroup(stringToIndex[grp]); }
            else
            {   stringToIndexIndex++;
                stringToIndex.Add(grp, stringToIndexIndex);
                SetGroup(stringToIndex[grp]);
            }
        }
        public static void SetGroup(int grp)
        {   if (lastSetGroup == grp)    // nothing to do
                return;
            lastSetGroup = grp;
            PenUp("SetGroup");
            if (!IsPathFigureEnd)      
            {   SetFigureEndTag(PathCount); } //Comment(xmlMarker.figureEnd + " " + PathCount + ">");  }
            IsPathFigureEnd = true;

            if (groupObjects)
            {   if ((grp >= 0) && (grp < gcodeStringMax))
                    gcodeStringIndex = grp;
                else
                {   gcode.Comment(gcodeString[gcodeStringIndex], "[plotter - setGroup] new index out of range");
                    Logger.Warn(string.Format("setGroup - new gcodeStringIndex out of range: {0}", grp));
                }
            }

            // set gcode variable from tool properties - XY feed, Z values
            int toolnr = PathToolNr;
            if (Properties.Settings.Default.importGCToolTableUse && Properties.Settings.Default.importGCToolDefNrUse)
                toolnr = (int)Properties.Settings.Default.importGCToolDefNr;
            gcode.getValuesFromToolTable(toolTable.getToolProperties(toolnr));
        }

        /// <summary>
        /// add header and footer, return string of gcode
        /// </summary>
        public static string FinalGCode(string titel, string file)
        {
            Logger.Trace("FinalGCode() ");
            gcode.docTitle = DocTitle;
            gcode.docDescription = DocDescription;
            string header = string.Format("( Use case: {0} )\r\n", Properties.Settings.Default.useCaseLastLoaded);
            header += gcode.GetHeader(titel, file);

            string footer = gcode.GetFooter();
            string output = "";
            if (gcodeTangEnable)
                footer = string.Format("G00 {0}{1:0.000} ({2})\r\n", gcodeTangName, 0, "Tangential axis move to zero") + footer;

            if (Properties.Settings.Default.importRepeatEnable)      // repeat code x times
            {
                for (int i = 0; i<Properties.Settings.Default.importRepeatCnt; i++)
                    output += finalGcodeString.ToString().Replace(',', '.');

                return header + output + footer;
            }
            else
                return header + finalGcodeString.ToString().Replace(',', '.') + footer;
        }

        /// <summary>
        /// add additional header info
        /// </summary>
        public static void AddToHeader(string cmt)
        {   gcode.AddToHeader(cmt);
            if (gcode.loggerTrace) Logger.Trace("AddToHeader: {0}", cmt);
        }

        /// <summary>
        /// return figure end tag string
        /// </summary>
        public static string SetFigureEnd(int nr)
        { return string.Format("{0}>", xmlMarker.figureEnd); }//, nr); }

        /// <summary>
        /// Insert Pen-up gcode command
        /// </summary>
        public static bool PenUp(string cmt = "", bool endFigure=true)
        {
            if (loggerTrace) Logger.Trace("  PenUp {0}",cmt);

            if (!comments)
                cmt = "";
            bool penWasDown = penIsDown;
            if (penIsDown)
            {   gcode.PenUp(gcodeString[gcodeStringIndex], cmt);   }
            penIsDown = false;

            if (endFigure)
            {   if ((Plotter.PathCount > 0) && !Plotter.IsPathFigureEnd)
                    SetFigureEndTag(PathCount); //Plotter.Comment(xmlMarker.figureEnd + " " + Plotter.PathCount + ">");   // finish old index first
                Plotter.IsPathFigureEnd = true;
            }
            return penWasDown;
        }

        /// <summary>
        /// Insert Pen-down gcode command
        /// </summary>
        public static void PenDownWithZ(float z, string cmt)
        {   float orig = gcode.gcodeZDown;
            float setZ = -Math.Abs(z);      // be sure for right sign
            setZ = Math.Max(orig, setZ);    // don't go deeper than set Z
            gcode.gcodeZDown = setZ;
            PenDown(cmt);
            gcode.gcodeZDown = orig;
        }
       public static void PenDown(string cmt)
        {
            if (loggerTrace) Logger.Trace(" PenDown {0}",cmt);
            if (!comments)
                cmt = "";

            if (!penIsDown)
            {   if (isStartPathIsPending)
                {   if (loggerTrace) Logger.Trace("  PenDown - MoveToRapid X{0:0.000} Y{1:0.000} alpha {2:0.00}", posStartPath.X, posStartPath.Y, 180*posStartAngle/Math.PI);
                    fixAngleExceed();
                    gcode.setTangential(gcodeString[gcodeStringIndex], 180 * posStartAngle / Math.PI);
                    gcode.MoveToRapid(gcodeString[gcodeStringIndex], posStartPath, cmt);
                    gcodeMath.cutAngleLast = gcodeMath.cutAngle;
                    isStartPathIsPending = false;
                    gcodeMath.cutAngleLast = posStartAngle;
                    lastGC = posStartPath;
                    lastSetGC = posStartPath;
                    gcodeStringIndexOld = gcodeStringIndex;
                }
                if (pauseBeforePenDown) { gcode.Pause(gcodeString[gcodeStringIndex], "Pause before pen down"); }
                gcode.PenDown(gcodeString[gcodeStringIndex], cmt);
            }
            else
                processTangentialAxis(gcodeMath.cutAngleLast, gcodeMath.cutAngle);       

            penIsDown = true;
        }

        /// <summary>
        /// Insert tool change command
        /// </summary>
        public static void ToolChange(int toolnr, string cmt = "")
        { gcode.Tool(gcodeString[gcodeStringIndex], toolnr, cmt + " plotter toolchange"); }  // add tool change commands (if enabled) and set XYFeed etc.

        /// <summary>
        /// Insert text with settings via GCodeFromFont.xxx
        /// </summary>
        public static void InsertText( string tmp)
        {   int oldPathCount = PathCount;
            PathCount = GCodeFromFont.getCode(PathCount,tmp);
            if (PathCount == oldPathCount) { AddToHeader(string.Format("Text insertion failed '{0}' with font '{1}'", GCodeFromFont.gcText, GCodeFromFont.gcFontName)); }
        }

        /// <summary>
        /// Insert M0 into gcode 
        /// </summary>
        public static void InsertPause(string cmt="")
        {   gcode.Pause(gcodeString[gcodeStringIndex], cmt); }

        /// <summary>
        /// set comment
        /// </summary>
        public static void Comment(string cmt)
        {   gcode.Comment(gcodeString[gcodeStringIndex], cmt); }
    }

    public enum xmlMarkerType { none, Group, Figure, Pass, Contour, Fill, Line };
    public static class xmlMarker
    {   public const string groupStart = "<Group";
        public const string groupEnd  = "</Group";
        public const string figureStart = "<Figure";
        public const string figureEnd  = "</Figure";
        public const string passStart = "<Pass";
        public const string passEnd  = "</Pass";
        public const string contourStart = "<Contour";
        public const string contourEnd  = "</Contour";
        public const string fillStart = "<Fill";
        public const string fillEnd  = "</Fill";
        public const string revolutionStart = "<Revolution";
        public const string revolutionEnd = "</Revolution";
        public const string clearanceStart = "<Clearance";
        public const string clearanceEnd = "</Clearance";

        public const string tangentialAxis = "<Tangential";

        public struct BlockData      
        {   public int lineStart;
            public int lineEnd;
            public int id;
            public int toolNr;
            public int codeSize;
            public int codeArea;
            public string geometry;
            public string color;
            public string toolName;
            public string layer;
        };

        private static List<BlockData> listFigures = new List<BlockData>();
        private static List<BlockData> listGroups = new List<BlockData>();
        public static BlockData tmpFigure = new BlockData();
        private static BlockData tmpGroup = new BlockData();
        public static BlockData lastFigure = new BlockData();
        public static BlockData lastGroup = new BlockData();
        public static BlockData header = new BlockData();
        public static BlockData footer = new BlockData();

        public enum sortItem {id, geometry, toolNr, toolName, layer, color, codeSize, codeArea};

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Reset()
        {   listFigures.Clear(); listGroups.Clear();
            header.lineStart = 0; header.lineEnd = 999999;
            footer.lineStart = footer.lineEnd = 0; }

        public static void sortById(bool reverse=false)
        {   if (reverse) { listFigures.Sort((x, y) => y.id.CompareTo(x.id)); listGroups.Sort((x, y) => y.id.CompareTo(x.id)); }
            else         { listFigures.Sort((x, y) => x.id.CompareTo(y.id)); listGroups.Sort((x, y) => x.id.CompareTo(y.id)); }
        }
        public static void sortFigureById(bool reverse = false)
        {   if (reverse) { listFigures.Sort((x, y) => y.id.CompareTo(x.id)); }
            else         { listFigures.Sort((x, y) => x.id.CompareTo(y.id)); }
        }
        public static void sortByGeometry(bool reverse = false)
        {   if (reverse) listFigures.Sort((x, y) => y.geometry.CompareTo(x.geometry));
            else listFigures.Sort((x, y) => x.geometry.CompareTo(y.geometry));
        }
        public static void sortByToolNr(bool reverse = false)
        {   if (reverse) { listFigures.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); listGroups.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); }
            else         { listFigures.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); listGroups.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); }
        }
        public static void sortByToolName(bool reverse = false)
        {   if (listGroups.Count > 0)
            {   sortGroupByToolName(reverse); sortFigureById(reverse); }
            else
                sortFigureByToolName(reverse);
        }
        public static void sortFigureByToolName(bool reverse = false)
        {   if (reverse) { listFigures.Sort((x, y) => y.toolName.CompareTo(x.toolName)); }
            else         { listFigures.Sort((x, y) => x.toolName.CompareTo(y.toolName)); }
        }
        public static void sortGroupByToolName(bool reverse = false)
        {   if (reverse) { listGroups.Sort((x, y) => y.toolName.CompareTo(x.toolName)); }
            else         { listGroups.Sort((x, y) => x.toolName.CompareTo(y.toolName)); }
        }
        public static void sortByLayer(bool reverse = false)
        {   if (reverse) listGroups.Sort((x, y) => y.layer.CompareTo(x.layer));
            else listGroups.Sort((x, y) => x.layer.CompareTo(y.layer));
            sortFigureById(reverse);
        }
        public static void sortByColor(bool reverse = false)
        {   if (reverse) listFigures.Sort((x, y) => y.color.CompareTo(x.color));
            else listFigures.Sort((x, y) => x.color.CompareTo(y.color));
        }
        public static void sortByCodeSize(bool reverse = false)
        {   if (reverse) listGroups.Sort((x, y) => y.codeSize.CompareTo(x.codeSize));
            else listGroups.Sort((x, y) => x.codeSize.CompareTo(y.codeSize));
            sortFigureById(reverse);
        }
        public static void sortByCodeArea(bool reverse = false)
        {   if (reverse) listGroups.Sort((x, y) => y.codeArea.CompareTo(x.codeArea));
            else listGroups.Sort((x, y) => x.codeArea.CompareTo(y.codeArea));
            sortFigureById(reverse);
        }

        public static string getSortedCode(string[] oldCode)
        {
            StringBuilder tmp = new StringBuilder();

            for (int i = 0; i < xmlMarker.header.lineEnd; i++)          // copy header
            { tmp.AppendLine(oldCode[i]); }

            if (listGroups.Count > 0)
            {
                foreach (BlockData group in listGroups)            // go through all listed groups
                {
                    tmp.AppendLine(oldCode[group.lineStart]);
                    if (gcode.loggerTrace) Logger.Trace(" AddGroup {0}", oldCode[group.lineStart]);

                    foreach (BlockData figure in listFigures)       // check if figure is within group
                    {
                        if ((figure.lineStart >= group.lineStart) && (figure.lineEnd <= group.lineEnd))
                        {
                            if (gcode.loggerTrace) Logger.Trace("  AddFigure {0}", oldCode[figure.lineStart]);
                            for (int i = figure.lineStart; i <= figure.lineEnd; i++)
                            {   tmp.AppendLine(oldCode[i]); }
                        }
                    }
                    tmp.AppendLine(oldCode[group.lineEnd]);
                }
            }
            else
            {
                foreach (BlockData figure in listFigures)
                {   for (int i = figure.lineStart; i <= figure.lineEnd; i++) { tmp.AppendLine(oldCode[i]); } }  // copy sorted blocks
            }

            for (int i = footer.lineStart + 1; i < oldCode.Length; i++)          // copy footer
            { tmp.AppendLine(oldCode[i]); }

            return tmp.ToString();
        }


        public static string getAttributeValue(string Element, string Attribute)
        {
            int posAttribute = Element.IndexOf(Attribute);
            if (posAttribute <= 0) return "";
            int strt = Element.IndexOf('"',posAttribute + Attribute.Length);
            int end = Element.IndexOf('"', strt + 1);
            string val = Element.Substring(strt + 1, (end - strt - 1));
//            if (gcode.loggerTrace) Logger.Trace(" getAttributeValue {0}  {1}  {2}", Element, Attribute, val);
            return val;
        }
        public static int getAttributeValueInt(string Element, string Attribute)
        {   string tmp = getAttributeValue(Element, Attribute);
            if (tmp == "") return -1;
            int att;
            if (int.TryParse(tmp, out att))
                return att;
            return -1;
        }

        public static void AddFigure(int lineStart, string element)
        {   tmpFigure = setBlockData(lineStart, element);
 //           if (gcode.loggerTrace) Logger.Trace("AddFigure Line {0}  Id {1}  Geometry {2}", lineStart, tmpFigure.id, tmpFigure.geometry);
        }

        public static BlockData setBlockData(int lineStart, string element)
        {
            header.lineEnd = Math.Min(header.lineEnd, lineStart);   // lowest block-line = end of header
            BlockData tmp = new BlockData();
            tmp.lineStart = lineStart;
            tmp.id = tmp.toolNr = tmp.codeSize = tmp.codeArea = -1;
            tmp.geometry = tmp.layer = tmp.color = tmp.toolName = "";
//            if (gcode.loggerTrace) Logger.Trace("setBlockData {0}", element);
            if (element.Contains("Id"))        { tmp.id       = getAttributeValueInt(element, "Id"); }
            if (element.Contains("ToolNr"))    { tmp.toolNr   = getAttributeValueInt(element, "ToolNr"); }
            if (element.Contains("CodeSize"))  { tmp.codeSize = getAttributeValueInt(element, "CodeSize"); }
            if (element.Contains("CodeArea"))  { tmp.codeArea = getAttributeValueInt(element, "CodeArea"); }
            if (element.Contains("Geometry"))  { tmp.geometry = getAttributeValue(element, "Geometry"); }
            if (element.Contains("Color"))     { tmp.color    = getAttributeValue(element, "Color"); }
            if (element.Contains("ToolName"))  { tmp.toolName = getAttributeValue(element, "ToolName"); }
            return tmp;
        }


        public static void FinishFigure(int lineEnd)
        {   tmpFigure.lineEnd = lineEnd;
            listFigures.Add(tmpFigure);
            footer.lineStart = footer.lineEnd = Math.Max(footer.lineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetFigureCount()
        { return listFigures.Count; }

        public static bool GetFigure(int lineNr, int search = 0)
        {
            if (listFigures.Count > 0)
            {   if (search <= -1)     // search start/end before actual block
                {   BlockData tmp = listFigures[0];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is first block
                        return false;

                    lastFigure.lineStart = listFigures[0].lineStart;
                    for (int i = 1; i < listFigures.Count; i++)
                    {   if ((lineNr >= listFigures[i].lineStart) && (lineNr <= listFigures[i].lineEnd))
                        {   lastFigure.lineEnd = listFigures[i - 1].lineEnd;
                            if (search == -1)
                                lastFigure.lineStart = listFigures[i - 1].lineStart;
                            return true;
                        }
                    }
                }
                else if (search >= 1)     // search start/end before actual block
                {   BlockData tmp = listFigures[listFigures.Count - 1];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is last block
                        return false;

                    lastFigure.lineEnd = listFigures[listFigures.Count - 1].lineEnd;
                    for (int i = listFigures.Count - 1; i >= 0; i--)
                    {   if ((lineNr >= listFigures[i].lineStart) && (lineNr <= listFigures[i].lineEnd))
                        {   lastFigure.lineStart = listFigures[i + 1].lineStart;
                            if (search == 1)
                                lastFigure.lineEnd = listFigures[i + 1].lineEnd;
                            return true;
                        }
                    }
                }
                else
                {   foreach (BlockData tmp in listFigures)
                    {   if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))
                        {   lastFigure = tmp;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int FindInsertPositionFigureMostTop(int current)
        {   int start = 0;
            if (GetGroup(current))      // only check within same group
                start = lastGroup.lineStart;
            foreach (BlockData tmp in listFigures)
            {   if (tmp.lineStart > start)
                { return tmp.lineStart; }
            }
            return -1;
        }
        public static int FindInsertPositionFigureTop(int current)
        {   int start = 0;
            if (GetGroup(current))      // only check within same group
                start = lastGroup.lineStart;
            for (int i=1; i < listFigures.Count; i++)
            {   if (listFigures[i-1].lineStart > start)
                {   if ((current >= listFigures[i].lineStart) && (current <= listFigures[i].lineEnd))
                    { return listFigures[i - 1].lineStart; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionFigureMostBottom(int current)
        {   int end = listFigures[listFigures.Count-1].lineEnd;
            if (GetGroup(current))      // only check within same group
                end = lastGroup.lineEnd;
            else
                return listFigures[listFigures.Count - 1].lineEnd + 1;
            int lastEnd = 0;
            foreach (BlockData tmp in listFigures)
            {   if (tmp.lineStart >= end)      //
                { return lastEnd + 1; }
                lastEnd = tmp.lineEnd;
            }          
            return -1;
        }
        public static int FindInsertPositionFigureBottom(int current)
        {
            int end = listFigures[listFigures.Count - 1].lineEnd;
            if (GetGroup(current))      // only check within same group
                end = lastGroup.lineEnd;
            for (int i = 0; i < listFigures.Count - 1; i++)
            {   if (listFigures[i + 1].lineEnd <= end)
                {   if ((current >= listFigures[i].lineStart) && (current <= listFigures[i].lineEnd))
                    {    return listFigures[i + 1].lineEnd + 1;   }
                }
            }
            return -1;
        }
        public static bool isFoldingMarkerFigure(int line)
        {   foreach (BlockData tmp in listFigures)
            {   if ((line == tmp.lineStart) || (line == tmp.lineEnd))
                {   return true;  }
            }
            return false;
        }
    
        public static void AddGroup(int lineStart, string element)
        {   tmpGroup = setBlockData(lineStart, element);  }

        public static void FinishGroup(int lineEnd)
        {   tmpGroup.lineEnd = lineEnd;
            listGroups.Add(tmpGroup);
            footer.lineStart = footer.lineEnd = Math.Max(footer.lineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetGroupCount()
        { return listGroups.Count; }

        public static bool GetGroup(int lineNr, int search=0)
        {   if (listGroups.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {   BlockData tmp = listGroups[0];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is first block
                        return false;

                    lastGroup.lineStart = listGroups[0].lineStart;
                    for (int i = 1; i < listGroups.Count; i++)
                    {
                        if ((lineNr >= listGroups[i].lineStart) && (lineNr <= listGroups[i].lineEnd))
                        {
                            lastGroup.lineEnd = listGroups[i - 1].lineEnd;
                            if (search == -1)
                                lastGroup.lineStart = listGroups[i - 1].lineStart;
                            return true;
                        }
                    }
                }
                else if (search >= 1)     // search start/end before actual block
                {   BlockData tmp = listGroups[listGroups.Count-1];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is last block
                        return false;

                    lastGroup.lineEnd = listGroups[listGroups.Count - 1].lineEnd;
                    for (int i = listGroups.Count-1; i >= 0 ; i--)
                    {
                        if ((lineNr >= listGroups[i].lineStart) && (lineNr <= listGroups[i].lineEnd))
                        {   lastGroup.lineStart = listGroups[i + 1].lineStart;
                            if (search == 1)
                                lastGroup.lineEnd = listGroups[i + 1].lineEnd;
                            return true;
                        }
                    }
                }
                else
                {   foreach (BlockData tmp in listGroups)
                    {   if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))
                        {   lastGroup = tmp;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int FindInsertPositionGroupMostTop(int current)
        {   if (listGroups.Count > 1)
                return listGroups[0].lineStart;
            return -1;
        }
        public static int FindInsertPositionGroupTop(int current)
        {   if (listGroups.Count > 1)
            {   for (int i = 1; i < listGroups.Count; i++)
                {   if ((current >= listGroups[i].lineStart) && (current <= listGroups[i].lineEnd))
                    {   return listGroups[i-1].lineStart; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionGroupMostBottom(int current)
        {   if (listGroups.Count > 0)
                return listGroups[listGroups.Count-1].lineEnd+1;
            return -1;
        }
        public static int FindInsertPositionGroupBottom(int current)
        {   if (listGroups.Count > 1)
            {   for (int i = 1; i < listGroups.Count-1; i++)
                {   if ((current >= listGroups[i].lineStart) && (current <= listGroups[i].lineEnd))
                    { return listGroups[i + 1].lineEnd+1; }
                }
            }
            return -1;
        }
        public static bool isFoldingMarkerGroup(int line)
        {   foreach (BlockData tmp in listGroups)
            {   if ((line == tmp.lineStart) || (line == tmp.lineEnd))
                { return true; }
            }
            return false;
        }


    }
}
