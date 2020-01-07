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
*/

using System;
using System.Text;
using System.Windows;

namespace GRBL_Plotter
{
    public static class Plotter
    {
        private static bool loggerTrace = false;    //true;

        private const int gcodeStringMax = 260;                 // max amount of tools
        private static int gcodeStringIndex = 0;                // index for stringBuilder-Array
        private static int gcodeStringIndexOld = 0;             // detect change in index
        private static StringBuilder[] gcodeString = new StringBuilder[gcodeStringMax];
        private static StringBuilder finalGcodeString = new StringBuilder();

        private static bool penIsDown = false;
        private static bool comments = false;
        private static bool pauseBeforePath = false;
        private static bool pauseBeforePenDown = false;
        private static bool groupObjects = false;
        private static int sortOption = 0;
        private static bool sortInvert = false;
        private static int amountOfTools = 0;
        private static bool gcodeUseSpindle = false;
        private static bool gcodeReduce = false;            // if true remove G1 commands if distance is < limit
        private static float gcodeReduceVal = .1f;          // limit when to remove G1 commands
        private static int lastSetGroup = -1;

        private static Point lastGC, lastSetGC;             // store last position

        public static int PathCount { get; set; } = 0;
        public static int PathToolNr { get; set; } = 0;
        public static bool IsPathReduceOk { get; set; } = false;
        public static bool IsPathAvoidG23 { get; set; } = false;
        public static bool IsPathFigureEnd { get; set; } = true;
        public static string PathId { get; set; } = "";
        public static string PathName { get; set; } = "";
        private static string pathColor = "";
        public static string PathColor
        {   get {   return pathColor; }
            set {   pathColor = value;
                    if (pathColor.Length == 3)
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
            sortOption = Properties.Settings.Default.importGroupSort;                // SVG-Import sort by tool
            sortInvert = Properties.Settings.Default.importGroupSortInvert;
            gcodeUseSpindle = Properties.Settings.Default.importGCZEnable;
            gcodeReduce = Properties.Settings.Default.importRemoveShortMovesEnable;
            gcodeReduceVal = (float)Properties.Settings.Default.importRemoveShortMovesLimit;
            comments = Properties.Settings.Default.importSVGAddComments;
            lastSetGroup = -1;

            gcodeStringIndex = 0;
            gcodeStringIndexOld = 0;
            for (int i = 0; i < gcodeStringMax; i++)        // hold gcode snippes for later sorting
            {   gcodeString[i] = new StringBuilder();
                gcodeString[i].Clear();
            }
            finalGcodeString.Clear();

            PathCount = 0;
            PathToolNr  = 0;
            IsPathReduceOk  = false;
            IsPathAvoidG23  = false;
            IsPathFigureEnd  = true;
            PathId  = "";
            PathName = "";
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

            if (loggerTrace) Logger.Trace(" StartPath X{0:0.000} Y{1:0.000} {2}", coordxy.X, coordxy.Y,cmt);

            if ((gcodeStringIndex == gcodeStringIndexOld) && (lastGC == coordxy))    // no change in position, no need for pen-up -down
            {
                //Comment( cmt +" same pos");
            }
            else
            {
                PenUp(cmt);
                if (!IsPathFigureEnd)
                    Comment(string.Format("{0} nr=\"{1}\" >", xmlMarker.figureEnd, PathCount));
                IsPathFigureEnd = true;

                string attributeId = (PathId.Length > 0) ? string.Format(" Id=\"{0}\"", PathId) : "";
                string attributeColor = (pathColor.Length > 0) ? string.Format(" Color=\"#{0}\"", pathColor) : "";
                string attributeToolNr = string.Format(" ToolNr=\"{0}\"", PathToolNr);

                Comment(string.Format("{0} {1}{2}{3}{4}> ", xmlMarker.figureStart, (++PathCount), attributeId, attributeColor, attributeToolNr));
                if (comments && (PathName.Length > 0)) { Comment(PathName); }

                if (pauseBeforePath && !pauseBeforePenDown) { InsertPause("Pause before path"); }
                PenUp("Start path");
                IsPathFigureEnd = false;

                gcode.MoveToRapid(gcodeString[gcodeStringIndex], coordxy, cmt);
            }
            lastGC = coordxy;
            lastSetGC = coordxy;
            IsPathReduceOk = false;
            gcodeStringIndexOld = gcodeStringIndex;
        }

        /// <summary>
        /// Finish path
        /// </summary>
        public static void StopPath(string cmt)
        {
            if (loggerTrace) Logger.Trace(" StopPath {0}",cmt);

            if (gcodeReduce)
            {   if ((lastSetGC.X != lastGC.X) || (lastSetGC.Y != lastGC.Y)) // restore last skipped point for accurat G2/G3 use
                    MoveToDashed(lastGC, cmt); //gcode.MoveTo(gcodeString[gcodeStringIndex], lastGC, "restore Point");
            }

            PenUp(cmt);     // PenUp -> SVG Clipboard-Code
        }


        /// <summary>
        /// Move to next coordinate
        /// </summary>
        public static void MoveTo(Point coordxy, string cmt)
        {
            if (loggerTrace) Logger.Trace(" MoveTo X{0:0.000} Y{1:0.000}", coordxy.X, coordxy.Y);

            bool rejectPoint = false;
            PenDown(cmt + " moveto");
            if (gcodeReduce && IsPathReduceOk)
            {
                double distance = Math.Sqrt(((coordxy.X - lastSetGC.X) * (coordxy.X - lastSetGC.X)) + ((coordxy.Y - lastSetGC.Y) * (coordxy.Y - lastSetGC.Y)));
                if (distance < gcodeReduceVal)      // discard actual G1 movement
                {   rejectPoint = true;  }
                else
                {   lastSetGC = coordxy;  }
            }
            if (!gcodeReduce || !rejectPoint)       // write GCode
            { MoveToDashed(coordxy, cmt); }
            //gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt);  }
            lastGC = coordxy;
        }
        private static void MoveToDashed(Point coordxy, string cmt)
        {
            if (loggerTrace) Logger.Trace(" MoveToDashed X{0:0.000} Y{1:0.000}", coordxy.X, coordxy.Y);

            bool showDashInfo = false;
            string dashInfo = "";

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
                            PenDown("");
                            dd = PathDashArray[i++];
                            if (showDashInfo) { dashInfo = "dash " + dd.ToString(); }
                            yy += dd;
                            if (yy < coordxy.Y)
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), dashInfo); }
                            else
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp("", false);
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
                            PenDown("");
                            if (yy > coordxy.Y)
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], new Point(coordxy.X, yy), cmt); }
                            else
                            { gcode.MoveTo(gcodeString[gcodeStringIndex], coordxy, cmt); break; }
                            if (i >= PathDashArray.Length)
                                i = 0;
                            PenUp("", false);
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
            if (loggerTrace) Logger.Trace(" ArcTo X{0:0.000} Y{1:0.000}", coordxy.X, coordxy.Y);

            PenDown(cmt);
            if (gcodeReduce && IsPathReduceOk)      // restore last skipped point for accurat G2/G3 use
            {
                if ((lastSetGC.X != lastGC.X) || (lastSetGC.Y != lastGC.Y))
                    MoveToDashed(lastGC, cmt); //gcode.MoveTo(gcodeString[gcodeStringIndex], lastGC, cmt);
            }
            gcode.Arc(gcodeString[gcodeStringIndex], 3, coordxy, coordij, cmt, IsPathAvoidG23);
        }
        public static void Arc(int gnr, float x, float y, float i, float j, string cmt = "", bool avoidG23 = false)
        {
            if (loggerTrace) Logger.Trace(" Arc X{0:0.000} Y{1:0.000}", x, y);

            PenDown(cmt);   // added for DXF circle multiple pass
            if (gcodeReduce && IsPathReduceOk)      // restore last skipped point for accurat G2/G3 use
            {   if ((lastSetGC.X != lastGC.X) || (lastSetGC.Y != lastGC.Y))
                    MoveToDashed(lastGC, cmt); 
            }

            gcode.Arc(gcodeString[gcodeStringIndex], gnr, x, y, i, j, cmt, avoidG23);
            lastSetGC.X = x; lastSetGC.Y = y;
            lastGC.X = x; lastGC.Y = y;
        }

        /// <summary>
        /// Insert Start, End code, sort indexed code inbetween
        /// </summary>
        public static void SortCode()
        {
            gcode.jobStart(finalGcodeString, "StartJob");
            Logger.Trace("SortCode() group:{0}", groupObjects);

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
                        toolTable.indexSetCodeSize(gcodeString[toolnr].Length);
                    tmp += toolnr.ToString() + "  ";
                }

                if (sortOption == 1)
                    toolTable.sortByToolNr(sortInvert);
                else if (sortOption == 2)
                    toolTable.sortByCodeSize(sortInvert);

                finalGcodeString.AppendLine();
                int groupnr = 0;
                bool useDefTool = Properties.Settings.Default.importGCToolTableUse && Properties.Settings.Default.importGCToolDefNrUse;
                int useDefToolNr = (int)Properties.Settings.Default.importGCToolDefNr;
                int toolUse = 0;
                string toolName = "";
                for (int i = 0; i < gcodeStringMax; i++) 
                {
                    toolTable.setIndex(i);                  // set index in svgPalette
                    toolnr = toolTable.indexToolNr();       // get value from set index
                    toolUse = useDefTool ? useDefToolNr : toolnr;
                    toolName = useDefTool ? "Default" : toolTable.indexName();
                    if ((toolnr >= 0) && (gcodeString[toolnr].Length > 1))
                    {
                        gcode.Comment(finalGcodeString, string.Format("{0} {1} ToolNr='{2}' ToolName='{3}'>", xmlMarker.groupStart, ++groupnr, toolUse, toolName));
                        gcode.Tool(finalGcodeString, toolUse, toolName); // add tool change commands (if enabled) and set XYFeed etc.
                        finalGcodeString.Append(gcodeString[toolnr]);
                        gcode.Comment(finalGcodeString, xmlMarker.groupEnd + " " + groupnr + ">");
                        gcodeString[toolnr].Clear();            // don't append a 2nd time
                    }
                }
                toolName = useDefTool ? "Default" : "not in tool table";
                for (int i = 0; i < gcodeStringMax; i++)  
                {
                    if (gcodeString[i].Length > 1)
                    {
                        gcode.Comment(finalGcodeString, string.Format("{0} {1} ToolNr='{2}' {3}>", xmlMarker.groupStart, ++groupnr, useDefToolNr, toolName));
                        gcode.Tool(finalGcodeString, useDefToolNr, toolName); // add tool change commands (if enabled) and set XYFeed etc.
                        finalGcodeString.Append(gcodeString[i]);
                        gcode.Comment(finalGcodeString, xmlMarker.groupEnd + " " + groupnr + ">");
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
        public static void SetGroup(int grp)
        {   if (lastSetGroup == grp)    // nothing to do
                return;
            lastSetGroup = grp;
            PenUp();
            if (!IsPathFigureEnd)      
            {   Comment(xmlMarker.figureEnd + " " + PathCount + ">");  }
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

 //           if (setTool)
 //           { ToolChange(PathToolNr,pathColor); }
        }

        /// <summary>
        /// add header and footer, return string of gcode
        /// </summary>
        public static string FinalGCode(string titel, string file)
        {
            Logger.Trace("FinalGCode() ");
            gcode.docTitle = DocTitle;
            gcode.docDescription = DocDescription;
            string header = gcode.GetHeader(titel, file);
            string footer = gcode.GetFooter();
            string output = "";
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
        /// set comment
        /// </summary>
        public static void Comment(string cmt)
        {   gcode.Comment(gcodeString[gcodeStringIndex], cmt); }

        /// <summary>
        /// add additional header info
        /// </summary>
        public static void AddToHeader(string cmt)
        {   gcode.AddToHeader(cmt); }

        /// <summary>
        /// return figure end tag string
        /// </summary>
        public static string SetFigureEnd(int nr)
        {   return string.Format("{0} {1}>", xmlMarker.figureEnd, nr); }

        /// <summary>
        /// Insert Pen-up gcode command
        /// </summary>
        public static bool PenUp(string cmt = "", bool endFigure=true)
        {
            if (!comments)
                cmt = "";
            bool penWasDown = penIsDown;
            if (penIsDown)
                gcode.PenUp(gcodeString[gcodeStringIndex], cmt);
            penIsDown = false;
            if (loggerTrace) Logger.Trace(" PenUp pen was down {0}", penWasDown);

            if (endFigure)
            {   if ((Plotter.PathCount > 0) && !Plotter.IsPathFigureEnd)
                    Plotter.Comment(xmlMarker.figureEnd + " " + Plotter.PathCount + ">");   // finish old index first
                Plotter.IsPathFigureEnd = true;
            }

            return penWasDown;
        }

        /// <summary>
        /// Insert Pen-down gcode command
        /// </summary>
        public static void PenDown(string cmt)
        {
            if (!comments)
                cmt = "";
            if (loggerTrace) Logger.Trace(" PenDown pen was down {0}", penIsDown);

            if (!penIsDown)
            {   if (pauseBeforePenDown) { gcode.Pause(gcodeString[gcodeStringIndex], "Pause before pen down"); }
                gcode.PenDown(gcodeString[gcodeStringIndex], cmt);
            }
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
        {
            int oldPathCount = PathCount;
            PathCount = GCodeFromFont.getCode(gcodeString[gcodeStringIndex], PathCount,tmp);
            if (PathCount == oldPathCount) { AddToHeader(string.Format("Text insertion failed '{0}' with font '{1}'", GCodeFromFont.gcText, GCodeFromFont.gcFontName)); }
        }

        /// <summary>
        /// Insert M0 into gcode 
        /// </summary>
        public static void InsertPause(string cmt="")
        { gcode.Pause(gcodeString[gcodeStringIndex], cmt); }
    }

    public enum xmlMarkerType { none, Group, Figure, Pass, Contour, Fill };
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
    }
}
