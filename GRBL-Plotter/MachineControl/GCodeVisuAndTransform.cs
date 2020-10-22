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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Globalization;

namespace GRBL_Plotter
{
    public static partial class VisuGCode
    {   public enum translate { None, ScaleX, ScaleY, Offset1, Offset2, Offset3, Offset4, Offset5, Offset6, Offset7, Offset8, Offset9, MirrorX, MirrorY, MirrorRotary };
        public static Dimensions xyzSize = new Dimensions();
        public static Dimensions G0Size = new Dimensions();
        public static drawingProperties drawingSize = new drawingProperties();
        public static GraphicsPath pathPenUp = new GraphicsPath();
        public static GraphicsPath pathPenDown = new GraphicsPath();
        public static GraphicsPath pathRuler = new GraphicsPath();
        public static GraphicsPath pathTool = new GraphicsPath();
        public static GraphicsPath pathMarker = new GraphicsPath();
        public static GraphicsPath pathHeightMap = new GraphicsPath();
        public static GraphicsPath pathMachineLimit = new GraphicsPath();
        public static GraphicsPath pathToolTable = new GraphicsPath();
        public static GraphicsPath pathBackground = new GraphicsPath();
        public static GraphicsPath pathMarkSelection = new GraphicsPath();
        public static GraphicsPath pathRotaryInfo = new GraphicsPath();
        public static GraphicsPath pathDimension= new GraphicsPath();
        public static GraphicsPath path = pathPenUp;
		
        public static GraphicsPath pathActualDown = pathPenDown;


		public class pathData
		{
			public GraphicsPath path;
            public Color color = Color.White;
			public float width = 0;
			public Pen pen;
			public pathData()
			{	path = new GraphicsPath();
				color = Properties.Settings.Default.gui2DColorPenDown;
				width = (float)Properties.Settings.Default.gui2DWidthPenDown;
				pen = new Pen(color, width);
			}
			public pathData(string pencolor, double penwidth)
			{	path = new GraphicsPath();
				uint clr = 0;
//                Logger.Trace("pathData color:{0}  width:{1}",pencolor,penwidth);
                if (pencolor == "")
                {   color = Properties.Settings.Default.gui2DColorPenDown; }
				else if (UInt32.TryParse(pencolor, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out clr))	// try Hex code #00ff00
				{	clr = clr | 0xff000000;	// remove alpha
					color = System.Drawing.Color.FromArgb((int)clr);
				}
				else
				{	color = Color.FromName(pencolor);
					if (color == System.Drawing.Color.FromArgb(0))
					{	color = Properties.Settings.Default.gui2DColorPenDown; }
				}
				
				width = (float)penwidth;
				if (width <= 0)
					width = (float)Properties.Settings.Default.gui2DWidthPenDown;
				pen = new Pen(color, width);
				pen.LineJoin = LineJoin.Round;
				pen.StartCap = LineCap.Round;
				pen.EndCap = LineCap.Round;				
			}
		}
		public static List<pathData> pathObject = new List<pathData>();


        public struct pathInfo
        {
            public PointF position;
            public double angle;
            public string info;
        };
        private static pathInfo tempPathInfo = new pathInfo();
        public static List<pathInfo> pathInfoMarker = new List<pathInfo>();

        public enum convertMode { nothing, removeZ, convertZToS };

        public static double gcodeMinutes = 0;
        public static double gcodeDistance = 0;
        
        public static bool usesOthersThanXYZ;
        public static bool usesZUpDown;
       
        public static string errorString = "";

        private static double feedXmax = 5000;
        private static double feedYmax = 5000;
        private static double feedZmax = 5000;
        private static double feedAmax = 5000;
        private static double feedBmax = 5000;
        private static double feedCmax = 5000;
        private static double feedDefault = 5000;

        private static bool tangentialAxisEnable = false;
        private static string tangentialAxisName = "C";
		
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static bool logEnable = false;
        public static bool logDetailed = false;
        public static bool logCoordinates = false;


        public static string getProcessingTime()
        {   try
            {   TimeSpan t = TimeSpan.FromSeconds(gcodeMinutes * 60);
                return string.Format("Est. time: {0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            }
            catch
            {   return "Est. time: ?"; }
        }

        public static bool containsG2G3Command()
        { return modal.containsG2G3; }
        public static bool containsG91Command()
        { return modal.containsG91; }
        public static bool containsTangential()
        { return tangentialAxisEnable; }

        private static xyPoint origWCOLandMark = new xyPoint();
        private static List<coordByLine> coordListLandMark = new List<coordByLine>();
        /// <summary>
        /// copy actual gcode-pathPenDown to background path with machine coordinates
        /// </summary>
        public static void setPathAsLandMark(bool clear=false)
        {   if (clear)
            {   pathBackground.Reset();
                coordListLandMark.Clear();
                return;
            }
            pathBackground = (GraphicsPath)pathPenDown.Clone();
            coordListLandMark = new List<coordByLine>();
            bool isArc;
            foreach (coordByLine gcline in coordList)        // copy coordList and add WCO
            {
                isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                coordListLandMark.Add(new coordByLine(0, -1, gcline.actualPos + (xyPoint)grbl.posWCO, gcline.alpha, isArc));
            }
            origWCOLandMark = (xyPoint)grbl.posWCO;
        }
        /// <summary>
        /// translate background path with machine coordinates to take account of changed WCO
        /// </summary>
        public static void updatePathPositions()
        {
            Matrix matrix = new Matrix();
            matrix.Translate((float)(origWCOLandMark.X - grbl.posWCO.X), (float)(origWCOLandMark.Y - grbl.posWCO.Y));
            pathBackground.Transform(matrix);
            origWCOLandMark = (xyPoint)grbl.posWCO;

            matrix.Reset();
            matrix.Translate((float)(origWCOMachineLimit.X - grbl.posWCO.X), (float)(origWCOMachineLimit.Y - grbl.posWCO.Y));
            pathMachineLimit.Transform(matrix);
            pathToolTable.Transform(matrix);
            origWCOMachineLimit = (xyPoint)grbl.posWCO;
        }

        private static xyPoint origWCOMachineLimit = new xyPoint();
        /// <summary>
        /// create paths with machine limits and tool positions in machine coordinates
        /// </summary>
        public static void drawMachineLimit(toolPos[] toolTable)
        {
            float offsetX =  (float)grbl.posWCO.X;   // (float)machinePos.X - (float)grbl.posWork.X;// toolPos.X;
            float offsetY =  (float)grbl.posWCO.Y;   // (float)machinePos.Y- (float)grbl.posWork.Y;//toolPos.Y;
            float x1 = (float)Properties.Settings.Default.machineLimitsHomeX - offsetX;
            float y1 = (float)Properties.Settings.Default.machineLimitsHomeY - offsetY;
            float rx = (float)Properties.Settings.Default.machineLimitsRangeX;
            float ry = (float)Properties.Settings.Default.machineLimitsRangeY;
            float extend = 2 * rx;
			Matrix matrix = new Matrix();
			matrix.Scale(1, -1);
			
            RectangleF pathRect1 = new RectangleF(x1, y1, rx, ry);
            RectangleF pathRect2 = new RectangleF(x1- extend, y1- extend, rx + 2 * extend, ry + 2 * extend); //(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
            RectangleF pathRect3 = new RectangleF(x1, y1-30, rx, ry + 20); //(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
            pathMachineLimit.Reset();
            pathMachineLimit.StartFigure();
            pathMachineLimit.AddRectangle(pathRect1);			
            pathMachineLimit.AddRectangle(pathRect2);
    /*        pathMachineLimit.AddRectangle(pathRect3);

            pathMachineLimit.Transform(matrix);
            pathMachineLimit.AddString("Set limitation in setup", new FontFamily("Arial"), (int)FontStyle.Regular, rx/20, new Point((int)x1,(int)-(y1-10)), StringFormat.GenericDefault);
			pathMachineLimit.Transform(matrix);
			*/
            pathToolTable.Reset();
            if ((toolTable != null) && (toolTable.Length >= 1))
            {
                float wx, wy;
                foreach (toolPos tpos in toolTable)
                {
                    wx = tpos.X - offsetX; wy = tpos.Y - offsetY;
                    try
                    {
                        if ((tpos.name != null) && (tpos.name.Length > 1) && (tpos.toolnr >= 0))
                        {
                            pathToolTable.StartFigure();
                            pathToolTable.AddEllipse(wx - 4, wy - 4, 8, 8);
                            pathToolTable.Transform(matrix);
                            pathToolTable.AddString(tpos.toolnr.ToString() + ") " + tpos.name, new FontFamily("Arial"), (int)FontStyle.Regular, 4, new Point((int)wx - 12, -(int)wy + 4), StringFormat.GenericDefault);
                            pathToolTable.Transform(matrix);
                        }
                    }
                    catch (Exception er) { Logger.Error(er, " drawMachineLimit"); }
                }
            }
            origWCOMachineLimit = (xyPoint)grbl.posWCO;
        }

        private static float maxStep = 100;
        /// <summary>
        /// create height map path in work coordinates
        /// </summary>
        public static void drawHeightMap(HeightMap Map)
        {   pathHeightMap.Reset();
            Vector2 tmp;
            int x = 0, y = 0;
            for (y = 0; y < Map.SizeY; y++)
            {   tmp = Map.GetCoordinates(x, y);
                pathHeightMap.StartFigure();
                pathHeightMap.AddLine((float)Map.Min.X, (float)tmp.Y, (float)Map.Max.X, (float)tmp.Y);
            }
            for (x = 0; x < Map.SizeX; x++)
            {
                tmp = Map.GetCoordinates(x, Map.SizeY-1);
                pathHeightMap.StartFigure();
                pathHeightMap.AddLine((float)tmp.X, (float)Map.Min.Y, (float)tmp.X, (float)Map.Max.Y);
            }
            tmp = Map.GetCoordinates(0, 0);
            xyzSize.setDimensionXY(tmp.X, tmp.Y);
            tmp = Map.GetCoordinates(Map.SizeX, Map.SizeY);
            xyzSize.setDimensionXY(tmp.X, tmp.Y);
        }

        /// <summary>
        /// apply new z-value to all gcode coordinates
        /// </summary>
        public static string applyHeightMap(IList<string> oldCode, HeightMap Map)
        {
            maxStep = (float)Map.GridX;
            getGCodeLines(oldCode,true);                // read gcode and process subroutines
            IList<string> tmp=createGCodeProg(true,true,false, convertMode.nothing).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();      // split lines and arcs createGCodeProg(bool replaceG23, bool applyNewZ, bool removeZ, HeightMap Map=null)
            getGCodeLines(tmp, false);                  // reload code
            return createGCodeProg(false, false, true, convertMode.nothing, Map);        // apply new Z-value;
        }

        /// <summary>
        /// undo height map (reload saved backup)
        /// </summary>
        public static void clearHeightMap()
        {   pathHeightMap.Reset(); }

        // analyse each GCode line and track actual position and modes for each code line
        private static List<gcodeByLine> gcodeList;        // keep original program
        private static List<gcodeByLine> simuList;         // as gcodeList but resolved subroutines
        private static List<coordByLine> coordList;        // get all coordinates (also subroutines)
        private static List<coordByLine> centerList;       // get center of arcs

        private static gcodeByLine oldLine = new gcodeByLine();    // actual parsed line
        private static gcodeByLine newLine = new gcodeByLine();    // last parsed line

        private static modalGroup modal = new modalGroup();        // keep modal states and helper variables
        private static int figureMarkerCount;

        public static bool codeBlocksAvailable()
        { return (figureMarkerCount > 0); }

        /// <summary>
        /// Entrypoint for generating drawing path from given gcode
        /// </summary>
        public static void getGCodeLines(IList<string> oldCode, bool processSubs=false)
        {
            string[] GCode = oldCode.ToArray<string>();
            string singleLine;
            modal = new modalGroup();               // clear

            gcodeList = new List<gcodeByLine>();    //.Clear();
            simuList = new List<gcodeByLine>();    //.Clear();
            coordList = new List<coordByLine>();    //.Clear();
            centerList = new List<coordByLine>();    //.Clear();
            pathInfoMarker = new List<pathInfo>();
            clearDrawingnPath();                    // reset path, dimensions
            figureMarkerCount = 0;
            lastFigureNumber = -1;
			pathActualDown = null;

            bool figureActive = false;
            gcodeMinutes = 0;
            gcodeDistance = 0;
            feedXmax = (grbl.getSetting(110) > 0) ? grbl.getSetting(110) : feedDefault;
            feedYmax = (grbl.getSetting(111) > 0) ? grbl.getSetting(111) : feedDefault;
            feedZmax = (grbl.getSetting(112) > 0) ? grbl.getSetting(112) : feedDefault;
            feedAmax = (grbl.getSetting(113) > 0) ? grbl.getSetting(113) : feedDefault;
            feedBmax = (grbl.getSetting(114) > 0) ? grbl.getSetting(114) : feedDefault;
            feedCmax = (grbl.getSetting(115) > 0) ? grbl.getSetting(115) : feedDefault;

            oldLine.resetAll(grbl.posWork);         // reset coordinates and parser modes, set initial pos
            newLine.resetAll();                     // reset coordinates and parser modes
            bool programEnd = false;
            figureCount = 1;                        // will be inc. in createDrawingPathFromGCode
            bool isArc = false;
            bool upDateFigure = false;
            tangentialAxisEnable = false;
            xmlMarker.Reset();                      // reset lists, holding marker line numbers

            bool xyPosChanged;
            bool updateFigureLineNeeded = false;
            errorString = "";
            for (int lineNr = 0; lineNr < GCode.Length; lineNr++)   // go through all gcode lines
            {
                modal.resetSubroutine();                            // reset m, p, o, l Word
                singleLine = GCode[lineNr].ToUpper().Trim();        // get line, remove unneeded chars

                if (processSubs && programEnd)
                { singleLine = "( " + singleLine + " )"; }          // don't process subroutine itself when processed

                newLine.parseLine(lineNr, singleLine, ref modal);
                xyPosChanged = calcAbsPosition(newLine, oldLine);                  // Calc. absolute positions and set object dimension: xyzSize.setDimension

/* Process Group marker */
                if (GCode[lineNr].Contains(xmlMarker.groupStart))                   // check if marker available
                {
                    string clean = GCode[lineNr].Replace("'", "\"");
                    figureMarkerCount++;
                    xmlMarker.AddGroup(lineNr, clean, figureMarkerCount);
                    figureActive = true;
                    if (logCoordinates) Logger.Trace(" Set Group  figureMarkerCount:{0}  {1}", figureMarkerCount, GCode[lineNr]);
				}
/* Process Figure marker */
                if (GCode[lineNr].Contains(xmlMarker.figureStart))                  // check if marker available
                {   if (!figureActive)
                        figureMarkerCount++;
                    figureActive = true;
                    updateFigureLineNeeded = true;                                  // update coordList.actualPos of this line later on
                    xyPosChanged = false;
                    string clean = GCode[lineNr].Replace("'", "\"");
                    xmlMarker.AddFigure(lineNr, clean, figureMarkerCount);
                    if (logCoordinates) Logger.Trace(" Set Figure figureMarkerCount:{0}  {1}", figureMarkerCount, GCode[lineNr]);
					if(Properties.Settings.Default.gui2DColorPenDownModeEnable)	// enable color mode
					{ 	pathData tmp = new pathData(xmlMarker.tmpFigure.penColor, xmlMarker.tmpFigure.penWidth);		// set color, width, pendownpath
                        //Logger.Trace("pathObject  {0}   {1}", xmlMarker.tmpFigure.penColor, xmlMarker.tmpFigure.penWidth);
						pathObject.Add(tmp);
						pathActualDown = pathObject[pathObject.Count -1].path;
					}					
                }
                if (GCode[lineNr].Contains(xmlMarker.tangentialAxis))                    
                {   tangentialAxisEnable = true;
                    tangentialAxisName = xmlMarker.getAttributeValue(GCode[lineNr],"Axis");
                    if (logEnable) Logger.Trace("Show tangetial axis '{0}'",tangentialAxisName);
                }

                if ((modal.mWord == 98) && processSubs)
                    newLine.codeLine = "(" + GCode[lineNr] + ")";
                else
                {   if (processSubs && programEnd)
                        newLine.codeLine = "( " + GCode[lineNr] + " )";   // don't process subroutine itself when processed
                    else
                        newLine.codeLine = GCode[lineNr];                 // store original line
                }

                if (!programEnd)
                {   upDateFigure = createDrawingPathFromGCode(newLine, oldLine);        // add data to drawing path
                    calculateProcessTime(newLine, oldLine);
                }
                if (figureMarkerCount > 0)
                {   if (figureActive)
                        newLine.figureNumber = figureMarkerCount;
                    else
                        newLine.figureNumber = -1;
                }
                isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
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

                oldLine = new gcodeByLine(newLine);                     // get copy of newLine      
                gcodeList.Add(new gcodeByLine(newLine));                // add parsed line to list
                simuList.Add(new gcodeByLine(newLine));                // add parsed line to list
                coordList.Add(new coordByLine(lineNr, newLine.figureNumber, (xyPoint)newLine.actualPos, newLine.alpha, isArc));

                if (updateFigureLineNeeded && xyPosChanged)
                {   updateFigureLineNeeded = false;
                    xmlMarker.tmpFigure.posStart = (xyPoint)newLine.actualPos;
                    coordList[xmlMarker.tmpFigure.lineStart].actualPos = (xyPoint)newLine.actualPos;
                    coordList[xmlMarker.tmpFigure.lineStart].alpha = newLine.alpha;
//                    Logger.Debug("updateFigureLine {0}  at {1}  X {2:0.000}  Y {3:0.000}", lineNr, xmlMarker.tmpFigure.lineStart, newLine.actualPos.X, newLine.actualPos.Y);
                }

                if ((modal.mWord == 30) || (modal.mWord == 2)) { programEnd = true; }
                if (modal.mWord == 98)
                {   if (lastSubroutine[0] == modal.pWord)
                        addSubroutine(GCode, lastSubroutine[1], lastSubroutine[2], modal.lWord, processSubs);
                    else
                        findAddSubroutine(modal.pWord, GCode, modal.lWord, processSubs);      // scan complete GCode for matching O-word
                }
/* Process Figure end */
                if (GCode[lineNr].Contains(xmlMarker.figureEnd))                    // check if marker available
                {   figureActive = false;
                    xmlMarker.tmpFigure.posEnd = (xyPoint)newLine.actualPos;
                    xmlMarker.FinishFigure(lineNr);
					pathActualDown = null;
                }
/* Process Group end */
                if (GCode[lineNr].Contains(xmlMarker.groupEnd))                    // check if marker available
                {   xmlMarker.FinishGroup(lineNr); 
				}

            }   // finish reading lines

            bool showArrow = Properties.Settings.Default.gui2DPenUpArrow;
            bool showId = Properties.Settings.Default.gui2DPenUpId;
            if (showArrow || showId)
            {   foreach (pathInfo tmp in pathInfoMarker)
                {   addArrow(pathPenUp, tmp.position, tmp.angle, tmp.info, showArrow, showId); }
            }
        }
        /// <summary>
        /// Find and add subroutine within given gcode
        /// </summary>
        private static string findAddSubroutine(int foundP, string[] GCode, int repeat, bool processSubs)
        {
            modalGroup tmp = new modalGroup();                      // just temporary use
            gcodeByLine tmpLine = new gcodeByLine();                // just temporary use
            int subStart=0, subEnd=0;
            bool foundO = false;
            for (int lineNr = 0; lineNr < GCode.Length; lineNr++)   // go through GCode lines
            {   tmpLine.parseLine(lineNr, GCode[lineNr], ref tmp);       // parse line
                if (tmp.oWord == foundP)                            // subroutine ID found?
                {   if (!foundO)
                    {   subStart = lineNr;       
                        foundO = true;
                    }
                    else
                    {   if (tmp.mWord == 99)                        // subroutine end found?
                        {   subEnd = lineNr;    
                            break;
                        }
                    }
                }
            }
            if ((subStart > 0) && (subEnd > subStart))
            {   addSubroutine(GCode, subStart, subEnd, repeat, processSubs);    // process subroutine
                lastSubroutine[0] = foundP;
                lastSubroutine[1] = subStart;
                lastSubroutine[2] = subEnd;
            }
            return String.Format("Start:{0} EndX:{1} ", subStart, subEnd);      
        }
        private static int[] lastSubroutine = new int[] { 0, 0, 0 };

        /// <summary>
        /// process subroutines
        /// </summary>
        private static void addSubroutine(string[] GCode, int start, int stop, int repeat, bool processSubs)
        {   bool showPath = true;
            bool isArc = false;
            for (int loop = 0; loop < repeat; loop++)
            {   for (int subLineNr = start + 1; subLineNr < stop; subLineNr++)      // go through real line numbers and parse sub-code
                {   if (GCode[subLineNr].IndexOf("%START_HIDECODE") >= 0) { showPath = false; }
                    if (GCode[subLineNr].IndexOf("%STOP_HIDECODE") >= 0) { showPath = true; }

                    newLine.parseLine(subLineNr, GCode[subLineNr], ref modal);      // reset coordinates, set lineNumber, parse GCode
                    newLine.isSubroutine = !processSubs;
                    calcAbsPosition(newLine, oldLine);                              // calc abs position

                    if (!showPath) newLine.ismachineCoordG53 = true;

                    if (processSubs)
                        gcodeList.Add(new gcodeByLine(newLine));      // add parsed line to list
                    simuList.Add(new gcodeByLine(newLine));      // add parsed line to list
                    if (!newLine.ismachineCoordG53)
                    {
                        isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                        coordList.Add(new coordByLine(subLineNr, newLine.figureNumber, (xyPoint)newLine.actualPos, newLine.alpha, isArc));
                        if (((newLine.motionMode > 0) || (newLine.z != null)) && !((newLine.x == grbl.posWork.X) && (newLine.y == grbl.posWork.Y)))
                            xyzSize.setDimensionXYZ(newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);             // calculate max dimensions
                    }                                                                                                       // add data to drawing path
                    if (showPath)
                        createDrawingPathFromGCode(newLine, oldLine);
                    oldLine = new gcodeByLine(newLine);   // get copy of newLine                         
                }
            }
        }


        /// <summary>
        /// Calc. absolute positions and set object dimension: xyzSize.setDimension.
        /// Return if X,Y or Z changed
        /// </summary>
        private static bool calcAbsPosition(gcodeByLine newLine, gcodeByLine oldLine)
        {
            bool posChanged = false;
            if (!newLine.ismachineCoordG53)         // only use world coordinates
            {   if ((newLine.motionMode >= 1) && (oldLine.motionMode == 0))     // take account of last G0 move
                {   xyzSize.setDimensionX(oldLine.actualPos.X);
                    xyzSize.setDimensionY(oldLine.actualPos.Y);
                }
                else
                {   G0Size.setDimensionX(newLine.actualPos.X);
                    G0Size.setDimensionY(newLine.actualPos.Y);
                }
                if (newLine.x != null)
                {   if (newLine.isdistanceModeG90)  // absolute move
                    {   newLine.actualPos.X = (double)newLine.x; 
                        if (newLine.motionMode >=1 )//if (newLine.actualPos.X != toolPos.X)            // don't add actual tool pos
                        {   xyzSize.setDimensionX(newLine.actualPos.X);
                        }
                    }
                    else
                    {   newLine.actualPos.X = oldLine.actualPos.X + (double)newLine.x;
                        if (newLine.motionMode >= 1)//if (newLine.actualPos.X != toolPos.X)            // don't add actual tool pos
                        {   xyzSize.setDimensionX(newLine.actualPos.X);// - toolPosX);
                        }
                    }
                }
                else
                    newLine.actualPos.X = oldLine.actualPos.X;

                if (newLine.y != null)
                {   if (newLine.isdistanceModeG90)
                    {   newLine.actualPos.Y = (double)newLine.y;
                        if (newLine.motionMode >= 1)//if (newLine.actualPos.Y != toolPos.Y)            // don't add actual tool pos
                        {   xyzSize.setDimensionY(newLine.actualPos.Y);
                        }
                    }
                    else
                    {   newLine.actualPos.Y = oldLine.actualPos.Y + (double)newLine.y;
                        if (newLine.motionMode >= 1)//if (newLine.actualPos.Y != toolPos.Y)            // don't add actual tool pos
                        {   xyzSize.setDimensionY(newLine.actualPos.Y);// - toolPosY);
                        }
                    }
                }
                else
                    newLine.actualPos.Y = oldLine.actualPos.Y;

                if (newLine.z != null)
                {   if (newLine.isdistanceModeG90)
                    {   newLine.actualPos.Z = (double)newLine.z;
                        if (newLine.actualPos.Z != grbl.posWork.Z)            // don't add actual tool pos
                            xyzSize.setDimensionZ(newLine.actualPos.Z); // removed - toolPosZ
                    }
                    else
                    {   newLine.actualPos.Z = oldLine.actualPos.Z + (double)newLine.z;
                        if (newLine.actualPos.Z != grbl.posWork.Z)            // don't add actual tool pos
                            xyzSize.setDimensionZ(newLine.actualPos.Z);// - toolPosZ);
                    }
                }
                else
                    newLine.actualPos.Z = oldLine.actualPos.Z;

                if (newLine.a != null)
                {   if (newLine.isdistanceModeG90)          // absolute move
                        newLine.actualPos.A = (double)newLine.a;
                    else
                        newLine.actualPos.A = oldLine.actualPos.A + (double)newLine.a;
                }
                else
                    newLine.actualPos.A = oldLine.actualPos.A;

                if (newLine.b != null)
                {
                    if (newLine.isdistanceModeG90)          // absolute move
                        newLine.actualPos.B = (double)newLine.b;
                    else
                        newLine.actualPos.B = oldLine.actualPos.B + (double)newLine.b;
                }
                else
                    newLine.actualPos.B = oldLine.actualPos.B;

                if (newLine.c != null)
                {
                    if (newLine.isdistanceModeG90)          // absolute move
                        newLine.actualPos.C = (double)newLine.c;
                    else
                        newLine.actualPos.C = oldLine.actualPos.C + (double)newLine.c;
                }
                else
                    newLine.actualPos.C = oldLine.actualPos.C;

                if (newLine.u != null)
                {
                    if (newLine.isdistanceModeG90)          // absolute move
                        newLine.actualPos.U = (double)newLine.u;
                    else
                        newLine.actualPos.U = oldLine.actualPos.U + (double)newLine.u;
                }
                else
                    newLine.actualPos.U = oldLine.actualPos.U;

                if (newLine.v != null)
                {
                    if (newLine.isdistanceModeG90)          // absolute move
                        newLine.actualPos.V = (double)newLine.v;
                    else
                        newLine.actualPos.V = oldLine.actualPos.V + (double)newLine.v;
                }
                else
                    newLine.actualPos.V = oldLine.actualPos.V;

                if (newLine.w != null)
                {
                    if (newLine.isdistanceModeG90)          // absolute move
                        newLine.actualPos.W = (double)newLine.w;
                    else
                        newLine.actualPos.W = oldLine.actualPos.W + (double)newLine.w;
                }
                else
                    newLine.actualPos.W = oldLine.actualPos.W;
            }
            newLine.alpha = oldLine.alpha;
            if (((xyPoint)oldLine.actualPos).DistanceTo((xyPoint)newLine.actualPos) != 0)
            {   newLine.alpha = gcodeMath.getAlpha((xyPoint)oldLine.actualPos, (xyPoint)newLine.actualPos);
                posChanged = true;
            }         
            return posChanged;
        }

        /// <summary>
        /// set marker into drawing on xy-position of desired line
        /// </summary>
        public static void setPosMarkerLine(int line, bool markFigure=true)
        {
            if (logDetailed) Logger.Trace("  setPosMarkerLine line:{0}  markFigure:{1}", line, markFigure);
            int figureNr;
            xyPoint center = new xyPoint(0,0);
            bool showCenter = false;
            try
            {
                if (line < coordList.Count)
                {
                    if (line == coordList[line].lineNumber)
                    {
                        grbl.posMarker = (xyPoint)coordList[line].actualPos;
                        grbl.posMarkerAngle = coordList[line].alpha;
                        if (coordList[line].isArc)
                        {   foreach (coordByLine point in centerList)
                            {   if (point.lineNumber == coordList[line].lineNumber)
                                {   center = point.actualPos; showCenter = true; break; }
                            }
                        }
                        createMarkerPath(showCenter,center, coordList[line].actualPos);// line-1
                        figureNr = coordList[line].figureNumber;
                        if ((figureNr != lastFigureNumber) && (markFigure))
                            markSelectedFigure(figureNr);
                        lastFigureNumber = figureNr;
                    }
                    else
                    {
                        xyPoint last = new xyPoint(0, 0);
                        foreach (coordByLine gcline in coordList)
                        {
                            if (line == gcline.lineNumber)
                            {
                                grbl.posMarker = (xyPoint)gcline.actualPos;
                                grbl.posMarkerAngle = gcline.alpha;
                                if (gcline.isArc)
                                {   foreach (coordByLine point in centerList)
                                    {   if (point.lineNumber == gcline.lineNumber) // ==line
                                        {   center = point.actualPos; showCenter = true;  break; }
                                    }
                                }
                                createMarkerPath(showCenter, center, last);
                                figureNr = coordList[line].figureNumber;
                                if ((figureNr != lastFigureNumber) && (markFigure))
                                    markSelectedFigure(figureNr);
                                lastFigureNumber = figureNr;

                                break;
                            }
                            last = gcline.actualPos;
                        }
                    }
                }
            }
            catch (Exception er) { Logger.Error(er, " setPosMarkerLine"); }
        }

        /// <summary>
        /// find gcode line with xy-coordinates near by given coordinates
        /// </summary>
        public static int setPosMarkerNearBy(xyPoint pos,bool toggleHighlight = true)
        {
            if (logDetailed) Logger.Trace(" setPosMarkerNearBy x:{0:0.00} y:{1:0.00}", pos.X, pos.Y);
            List<coordByLine> tmpList = new List<coordByLine>();     // get all coordinates (also subroutines)
            int figureNr;
            xyPoint center = new xyPoint(0, 0);
            bool showCenter = false;

            /* fill list with coordByLine with actual distance to given point */
            foreach (coordByLine gcline in coordList)
            {   gcline.calcDistance(pos);       // calculate distance work coordinates
                tmpList.Add(gcline);            // add to new list
            }
            if (Properties.Settings.Default.guiBackgroundShow && (coordListLandMark.Count > 1))
            {   foreach (coordByLine gcline in coordListLandMark)
                {   gcline.calcDistance(pos+(xyPoint)grbl.posWCO);      // calculate distance machine coordinates
                    tmpList.Add(new coordByLine(0, gcline.figureNumber, gcline.actualPos - (xyPoint)grbl.posWCO, gcline.alpha, gcline.distance)); // add as work coord.
                }
            }

            /* sort list by distance, get associated linenr. */
            int line = 0;
            List<coordByLine> SortedList = tmpList.OrderBy(o => o.distance).ToList();
            grbl.posMarker = (xyPoint)SortedList[line].actualPos;
            grbl.posMarkerAngle = SortedList[line].alpha;
            figureNr = SortedList[line].figureNumber;

            /* if possible, get center of arc */
            if (SortedList[line].isArc)
            {   foreach (coordByLine point in centerList)
                {   if (point.lineNumber == SortedList[line].lineNumber)
                    {   center = point.actualPos; showCenter = true; break; }
                }
            }

            /* highlight */
            createMarkerPath(showCenter, center);
            if ((figureNr != lastFigureNumber) || !toggleHighlight)
            {   markSelectedFigure(figureNr);   // select
                lastFigureNumber = figureNr;
            }
            else
            {   markSelectedFigure(-1);  // deselcet
                lastFigureNumber = -1;
            }

            return SortedList[line].lineNumber;
        }
        private static int lastFigureNumber = -1;
        public static int getHighlightStatus()
        { return lastFigureNumber; }

        public static int getFigureNumber(int line)
        {   foreach (coordByLine gcline in coordList)           // start search at beginning
            {   if (gcline.lineNumber == line)    // 1st occurance = hit
                {   return gcline.figureNumber;
                }
            }
            return -1;
        }
        /// <summary>
        /// return GCode lineNr of first point in selected path (figure)
        /// </summary>
        public static int getLineOfFirstPointInFigure(int lineNr=0)
        {
            int figureToFind = lastFigureNumber;
            if (lineNr > 0)
                figureToFind = getFigureNumber(lineNr);
            if ((figureToFind) < 0)
                return -1;
            foreach (coordByLine gcline in coordList)           // start search at beginning
            {   if (gcline.figureNumber == (figureToFind))    // 1st occurance = hit
                {   return gcline.lineNumber;
                }
            }
            return -1;
        }

        /// <summary>
        /// return GCode lineNr of last point in selected path (figure)
        /// </summary>
        public static int getLineOfEndPointInFigure(int start=0)
        {   if (start < 0)
                return -1;
            for (int k = 0; k < coordList.Count; k++)
            {   if (coordList[k].lineNumber >= start)       // get index in coordlist
                {   start = k;
                    break;
                }
            }
            int figNr = coordList[start].figureNumber;      // get figNr by index
            for (int i=start; i < coordList.Count(); i++)
            {   if (coordList[i].figureNumber != figNr)     // if figure-nr changed, end reached
                {
                    return coordList[i-1].lineNumber;
                }
            }
            return -1;
        }
        public static int getLineOfEndPointInFigureExtend(int start = 0)
        {
            int max = Math.Min(coordList.Count - 1, (start + 5));
            for (int k = start-1; k < max; k++)
            {   if (gcodeList[k].codeLine.StartsWith(xmlMarker.figureEnd))
                    return k;
            }
            return start;
        }

        public static void markSelectedGroup(int start = 0)
        {   List<int> figures = new List<int>();
            int figNr = 0;

            pathMarkSelection.Reset();
            GraphicsPath tmpPath = new GraphicsPath();
            tmpPath.Reset();

            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();

            for (int line = start+1; line < gcodeList.Count; line++)
            {   if (gcodeList[line].codeLine.Contains(xmlMarker.groupEnd))
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

            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
        }

        public static string selectedFigureInfo = "";
        /// <summary>
        /// create path of selected figure, or clear (-1)
        /// </summary>
        public static void markSelectedFigure(int figureNr)
        {
            if (figureNr <= 0)
            {   pathMarkSelection.Reset();
                lastFigureNumber = -1;
                selectedFigureInfo = "";
                return;
            }
            // get dimension of selection
            Dimensions tmpSize = new Dimensions();
            foreach (gcodeByLine gcline in gcodeList)
            {   if ((gcline.figureNumber == figureNr) && xyMove(gcline))
                { tmpSize.setDimensionXY(gcline.actualPos.X, gcline.actualPos.Y); }
            }
            xyPoint tmpCenter = new xyPoint(tmpSize.getCenter());
            selectedFigureInfo = string.Format("Selection: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", figureNr, tmpSize.dimx,tmpSize.dimy, tmpCenter.X, tmpCenter.Y);

            // find and copy selected path
            pathMarkSelection.Reset();
            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();
            for (int i = 1; i <= figureNr; i++)
                myPathIterator.NextMarker(pathMarkSelection);

            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
            selectedFigureInfo = string.Format("Selection: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", figureNr, selectionBounds.Width, selectionBounds.Height, centerX, centerY);
        }


        /// <summary>
        /// rotate and scale arround offset
        /// </summary>
        public static string transformGCodeRotate(double angle, double scale, xyPoint offset)
        {
            Logger.Debug("Rotate angle: {0}", angle);
            if (gcodeList == null) return "";
            xyPoint centerOfFigure = xyzSize.getCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = getCenterOfMarkedFigure();

            offset = centerOfFigure;

            double? newvalx, newvaly, newvali, newvalj;
            oldLine.resetAll(grbl.posWork);         // reset coordinates and parser modes
            clearDrawingnPath();                    // reset path, dimensions
			bool offsetApplied = false;
			double lastAbsPosX = 0;
			double lastAbsPosY = 0;

            foreach (gcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))
                { 	if (!gcline.isdistanceModeG90 && offsetApplied)			// correct relative movement of next figure
					{   if ((gcline.motionMode == 0) && ((gcline.x != null) || (gcline.y != null)))
                        {	gcline.x = gcline.actualPos.X - lastAbsPosX;	
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

						if (gcline.isdistanceModeG90)	// absolute
                        {
                            newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                            gcline.x = (newvalx * scale) + offset.X;
							gcline.y = (newvaly * scale) + offset.Y;
						}
						else
                        {
                            if ((gcline.motionMode == 0) && !offsetApplied)
                            {
                                newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                                newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                                gcline.x = gcline.x + ((newvalx * scale) + offset.X) - gcline.actualPos.X;
                                gcline.y = gcline.y + ((newvaly * scale) + offset.Y) - gcline.actualPos.Y;
                                if ((gcline.x != null) && (gcline.y != null))
                                    offsetApplied = true;
                            }
                            else
                            {
                                newvalx = gcline.x * Math.Cos(angle * Math.PI / 180) - gcline.y * Math.Sin(angle * Math.PI / 180);
                                newvaly = gcline.x * Math.Sin(angle * Math.PI / 180) + gcline.y * Math.Cos(angle * Math.PI / 180);
                                gcline.x = newvalx;
                                gcline.y = newvaly;
                            }
                            newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
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
                             if ((tangentialAxisName == "C") && (gcline.c != null)){ gcline.c += angle; }
                        else if ((tangentialAxisName == "B") && (gcline.b != null)){ gcline.b += angle; }
                        else if ((tangentialAxisName == "A") && (gcline.a != null)){ gcline.a += angle; }
                        else if ((tangentialAxisName == "Z") && (gcline.z != null)){ gcline.z += angle; }
                    }

             //       calcAbsPosition(gcline, oldLine);
             //       oldLine = new gcodeByLine(gcline);   // get copy of newLine
                }
            }
//			pathBackground.Reset();
            return createGCodeProg();
        }
 
        /// <summary>
        /// scale x and y seperatly in %
        /// </summary>
        public static string transformGCodeScale(double scaleX, double scaleY)
        {
            Logger.Debug("Scale scaleX: {0}, scale Y: {1}", scaleX, scaleY);
            if (gcodeList == null) return "";
            xyPoint centerOfFigure = xyzSize.getCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = getCenterOfMarkedFigure();
            double factor_x = scaleX / 100;
            double factor_y = scaleY / 100;
			bool offsetApplied = false;
			double lastAbsPosX = 0;
			double lastAbsPosY = 0;
			
            oldLine.resetAll(grbl.posWork);         // reset coordinates and parser modes
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))
                { 	if (!gcline.isdistanceModeG90 && offsetApplied)			// correct relative movement of next figure
					{   if ((gcline.motionMode == 0) && ((gcline.x != null) || (gcline.y != null)))
                        {	gcline.x = gcline.actualPos.X - lastAbsPosX;	
							gcline.y = gcline.actualPos.Y - lastAbsPosY;	
							offsetApplied = false;
						}
					}				
					continue; 
				}

                if (!gcline.ismachineCoordG53)
                {
                    if (gcline.isdistanceModeG90)           // absolute move: apply offset to any XY position
					{	if (gcline.x != null)
							gcline.x = gcline.x * factor_x - centerOfFigure.X * (factor_x - 1);
						if (gcline.y != null)
							gcline.y = gcline.y * factor_y - centerOfFigure.Y * (factor_y - 1);
					}
				    else
                    {   //if (!offsetApplied)                 // relative move: apply offset only once
					    if ((gcline.motionMode == 0) && !offsetApplied)
						{
                            if (gcline.x != null)
                                gcline.x = gcline.x  - (centerOfFigure.X - gcline.actualPos.X) * (factor_x - 1);
                            if (gcline.y != null)
                                gcline.y = gcline.y  - (centerOfFigure.Y - gcline.actualPos.Y) * (factor_y - 1);
							if ((gcline.x != null) && (gcline.y != null))
								offsetApplied = true;
						}
						else
						{	if (gcline.x != null)
								gcline.x = gcline.x * factor_x;
							if (gcline.y != null)
								gcline.y = gcline.y * factor_y;
						}
                        lastAbsPosX = gcline.actualPos.X * factor_x - centerOfFigure.X * (factor_x - 1);
                        lastAbsPosY = gcline.actualPos.Y * factor_y - centerOfFigure.Y * (factor_y - 1);
					}

                    if (gcline.i != null)
                        gcline.i = gcline.i * factor_x;
                    if (gcline.j != null)
                        gcline.j = gcline.j * factor_y;

                    calcAbsPosition(gcline, oldLine);
                    oldLine = new gcodeByLine(gcline);   // get copy of newLine
                }
            }
   			pathBackground.Reset();
            return createGCodeProg();        
        }
        public static string transformGCodeOffset(double x, double y, translate shiftToZero)
        {
            Logger.Debug("Transform X: {0}, Y: {1}, Offset: {2}", x, y, shiftToZero);
            if (gcodeList == null) return "";
            if ((lastFigureNumber <= 0) || (!(shiftToZero == translate.None)))
            { pathMarkSelection.Reset(); lastFigureNumber = -1; }
            double offsetX = 0;
            double offsetY = 0;
            bool offsetApplied = false;
            bool noInsertNeeded = false;
            oldLine.resetAll(grbl.posWork);         // reset coordinates and parser modes
            if (shiftToZero == translate.Offset1) { offsetX = x + xyzSize.minx;                     offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset2) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2;  offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset3) { offsetX = x + xyzSize.minx + xyzSize.dimx;      offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset4) { offsetX = x + xyzSize.minx;                     offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset5) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2;  offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset6) { offsetX = x + xyzSize.minx + xyzSize.dimx;      offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset7) { offsetX = x + xyzSize.minx;                     offsetY = y + xyzSize.miny; }
            if (shiftToZero == translate.Offset8) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2;  offsetY = y + xyzSize.miny; }
            if (shiftToZero == translate.Offset9) { offsetX = x + xyzSize.minx + xyzSize.dimx;      offsetY = y + xyzSize.miny; }
            if (shiftToZero == translate.None)    { offsetX = x; offsetY = y; }

            if (modal.containsG91)    // relative move: insert rapid movement before pen down, to be able applying offset
            {
                newLine.resetAll();
                int i,k;
                bool foundG91 = false;
                for (i = 0; i < gcodeList.Count; i++)       // find first relative move
                {   if ((!gcodeList[i].isdistanceModeG90) && (!gcodeList[i].isSubroutine) && (gcodeList[i].motionMode == 0) && (gcodeList[i].z != null))       
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
                            {   modalGroup tmp = new modalGroup();
                                newLine.parseLine(i, "G0 X0 Y0 (Insert offset movement)", ref tmp);
                                gcodeList.Insert(i + 1, newLine);
                            }
                        }
                    }
                }
            }
            bool hide_code = false; ;
            foreach (gcodeByLine gcline in gcodeList)
            {
                if (gcline.codeLine.IndexOf("%START_HIDECODE") >= 0) { hide_code  = true; }
                if (gcline.codeLine.IndexOf("%STOP_HIDECODE") >= 0)  { hide_code = false; }
                if ((!hide_code) && (!gcline.isSubroutine) && (!gcline.ismachineCoordG53) && (gcline.codeLine.IndexOf("(Setup - GCode") < 1)) // ignore coordinates from setup footer
                {
                    if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))    // 2019-11-30
                    { continue; }

                    if (gcline.isdistanceModeG90)           // absolute move: apply offset to any XY position
                    {
                        if (gcline.x != null)
                            gcline.x = gcline.x - offsetX;      // apply offset
                        if (gcline.y != null)
                            gcline.y = gcline.y - offsetY;      // apply offset
                    }
                    else
                    {   if (!offsetApplied)                 // relative move: apply offset only once
                        {   if (gcline.motionMode == 0)
                            {
                                    gcline.x = gcline.x - offsetX;
                                    gcline.y = gcline.y - offsetY;
                                    if ((gcline.x != null) && (gcline.y != null))
                                        offsetApplied = true;
                            }
                        }
                    }
                    calcAbsPosition(gcline, oldLine);
                    oldLine = new gcodeByLine(gcline);   // get copy of newLine
                }
            }
			pathBackground.Reset();
            return createGCodeProg();   
        }

        public static string replaceG23()
        {   return createGCodeProg(true, false, false, convertMode.nothing); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        public static string convertZ()
        { return createGCodeProg(false, false, false, convertMode.convertZToS); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)
        public static string removeZ()
        { return createGCodeProg(false, false, false, convertMode.removeZ); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        /// <summary>
        /// Generate GCode from given coordinates in GCodeList
        /// only replace lines with coordinate information
        /// </summary>
        private static string createGCodeProg()
        {   return createGCodeProg(false,false,false, convertMode.nothing); }
        private static string createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, convertMode specialCmd, HeightMap Map=null)
        {
            Logger.Debug("createGCodeProg replaceG23: {0}, splitMoves: {1}, applyNewZ: {2}, specialCmd: {3}", replaceG23, splitMoves, applyNewZ, specialCmd);
            if (gcodeList == null) return "";
            pathMarkSelection.Reset(); lastFigureNumber = -1;
            StringBuilder newCode = new StringBuilder();
            StringBuilder tmpCode = new StringBuilder();
            string infoCode="";
            bool getCoordinateXY, getCoordinateZ, forceM;
            double feedRate = 0;
            double spindleSpeed=-1; // force change
            byte spindleState = 5;
            byte coolantState = 9;
            double lastActualX = 0, lastActualY = 0,i,j;
            double newZ = 0;
            int lastMotionMode = 0;
            double convertMinZ = xyzSize.minz;          // 1st get last minimum
            xyzSize.resetDimension();                   // then reset
            bool hide_code = false;
            double convertMaxSpeed = (double)Properties.Settings.Default.convertZtoSMax;
            double convertMinSpeed = (double)Properties.Settings.Default.convertZtoSMin;
            double convertSpeedRange = Math.Abs(convertMaxSpeed - convertMinSpeed);
            double convertOffSpeed = (double)Properties.Settings.Default.convertZtoSOff;
            bool isArc = false;

            for (int iCode=0; iCode < gcodeList.Count; iCode++)     // go through all code lines
            {   gcodeByLine gcline = gcodeList[iCode];
                tmpCode.Clear();
                getCoordinateXY = false;
                getCoordinateZ = false;
                forceM = false;
                if (gcline.codeLine.Length == 0)
                    continue;

                if (gcline.codeLine.IndexOf("%START_HIDECODE") >= 0) { hide_code = true; }
                if (gcline.codeLine.IndexOf("%STOP_HIDECODE") >= 0) { hide_code = false; }

                #region replace circle by lines
                if ((!hide_code) && (replaceG23))                   // replace circles
                {
                    gcode.setup(false);                             // don't apply intermediate Z steps in certain sub functions
                    gcode.lastx = (float)lastActualX;
                    gcode.lasty = (float)lastActualY;
                    gcode.gcodeXYFeed = gcline.feedRate;
                    if (gcline.isdistanceModeG90)
                        gcode.gcodeRelative = false;
                    else
                        gcode.gcodeRelative = true;
                    if (gcline.motionMode > 1)
                    {
                        i = (double)((gcline.i != null) ? gcline.i : 0.0);
                        j = (double)((gcline.j != null) ? gcline.j : 0.0);
                        gcode.splitArc(newCode, gcline.motionMode, (float)lastActualX, (float)lastActualY, (float)gcline.actualPos.X, (float)gcline.actualPos.Y, (float)i, (float)j, true, gcline.codeLine);
                    }
                    else if (gcline.motionMode == 1)
                    {   if (((gcline.x != null) || (gcline.y != null)) && splitMoves)
                        {   
                                gcode.splitLine(newCode, gcline.motionMode, (float)lastActualX, (float)lastActualY, (float)gcline.actualPos.X, (float)gcline.actualPos.Y, maxStep, true, gcline.codeLine);
                        }
                        else
                        { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }
                    }
                    else
                    { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }

                }
                #endregion
                else
                {
                    if (gcline.x != null)
                    { tmpCode.AppendFormat(" X{0}", gcode.frmtNum((double)gcline.x)); getCoordinateXY = true; }
                    if (gcline.y != null)
                    { tmpCode.AppendFormat(" Y{0}", gcode.frmtNum((double)gcline.y)); getCoordinateXY = true; }
                    if ((getCoordinateXY || (gcline.z != null)) && applyNewZ && (Map != null))  //(gcline.motionMode != 0) &&       if (getCoordinateXY && applyNewZ && (Map != null))
                    {   newZ = Map.InterpolateZ(gcline.actualPos.X, gcline.actualPos.Y);
                        if ((gcline.motionMode != 0) || (newZ > 0))
                            gcline.z = gcline.actualPos.Z + newZ;
                    }

                    if (specialCmd == convertMode.convertZToS)
                    {   gcline.spindleSpeed = 0;                //  reset old speed
                        spindleSpeed = -1;                      // force output
                        if (gcline.spindleState < 5)            // if spindle on
                            forceM = true;
                    }

                    if (gcline.z != null)
                    {   if (specialCmd == convertMode.nothing)               //  !removeZ
                        {   tmpCode.AppendFormat(" Z{0}", gcode.frmtNum((double)gcline.z)); }
                        else if ((specialCmd == convertMode.convertZToS) && (convertMinZ != 0))
                        {   double convertTmp = (double)gcline.z * convertSpeedRange / convertMinZ + convertMinSpeed;
                            if (gcline.z > 0)
                                convertTmp = convertOffSpeed;
                            gcline.spindleSpeed = (int)convertTmp;
                            //tmpCode.AppendFormat(" S{0}", Math.Round(convertTmp));
                        }
                        getCoordinateZ = true;
                    }
                    if (gcline.a != null)
                    { tmpCode.AppendFormat(" A{0}", gcode.frmtNum((double)gcline.a)); getCoordinateZ = true; }
                    if (gcline.b != null)
                    { tmpCode.AppendFormat(" B{0}", gcode.frmtNum((double)gcline.b)); getCoordinateZ = true; }
                    if (gcline.c != null)
                    { tmpCode.AppendFormat(" C{0}", gcode.frmtNum((double)gcline.c)); getCoordinateZ = true; }
                    if (gcline.u != null)
                    { tmpCode.AppendFormat(" U{0}", gcode.frmtNum((double)gcline.u)); getCoordinateZ = true; }
                    if (gcline.v != null)
                    { tmpCode.AppendFormat(" V{0}", gcode.frmtNum((double)gcline.v)); getCoordinateZ = true; }
                    if (gcline.w != null)
                    { tmpCode.AppendFormat(" W{0}", gcode.frmtNum((double)gcline.w)); getCoordinateZ = true; }
                    if (gcline.i != null)
                    { tmpCode.AppendFormat(" I{0}", gcode.frmtNum((double)gcline.i)); getCoordinateXY = true; }
                    if (gcline.j != null)
                    { tmpCode.AppendFormat(" J{0}", gcode.frmtNum((double)gcline.j)); getCoordinateXY = true; }

                    if ((getCoordinateXY || getCoordinateZ) && (!gcline.ismachineCoordG53) && (!hide_code))
                    {
                        if ((gcline.motionMode > 0) && (feedRate != gcline.feedRate) && (getCoordinateXY || getCoordinateZ)) //((getCoordinateXY && !getCoordinateZ) || (!getCoordinateXY && getCoordinateZ)))
                        { tmpCode.AppendFormat(" F{0,0}", gcline.feedRate); }       // feed
                        if (spindleState != gcline.spindleState)
                        { tmpCode.AppendFormat(" M{0,0}", gcline.spindleState); }   // state
                        if (spindleSpeed != gcline.spindleSpeed)
                        { tmpCode.AppendFormat(" S{0,0}", gcline.spindleSpeed); }   // speed
                        if (coolantState != gcline.coolantState)
                        { tmpCode.AppendFormat(" M{0,0}", gcline.coolantState); }   // state

                        tmpCode.Replace(',', '.');
                        if (gcline.codeLine.IndexOf("(Setup - GCode") > 1)  // ignore coordinates from setup footer
                            newCode.AppendLine(gcline.codeLine);
                        else
                            newCode.AppendLine(gcline.otherCode + "G" + gcode.frmtCode(gcline.motionMode) + tmpCode.ToString() + infoCode);
                    }
                    else
                    {
                        if (forceM && ((spindleState != gcline.spindleState)||(spindleSpeed != gcline.spindleSpeed)))
                        {   tmpCode.AppendFormat("M{0,0} S{1,0}", gcode.frmtCode(gcline.spindleState), gcline.spindleSpeed);    // state
                            newCode.AppendLine(tmpCode.ToString());
                        }
                        else
                            newCode.AppendLine(gcline.codeLine.Trim('\r', '\n') + infoCode);    // add orignal code-line
                    }
                    lastMotionMode = gcline.motionMode;
                }
                feedRate = gcline.feedRate;
                spindleSpeed = gcline.spindleSpeed;
                spindleState = gcline.spindleState;
                coolantState = gcline.coolantState;
                lastActualX = gcline.actualPos.X; lastActualY = gcline.actualPos.Y;

                if ((!hide_code) && (!gcline.ismachineCoordG53) && (gcline.codeLine.IndexOf("(Setup - GCode") < 1)) // ignore coordinates from setup footer
                {
                    if (!((gcline.actualPos.X == grbl.posWork.X) && (gcline.actualPos.Y == grbl.posWork.Y)))            // don't add actual tool pos
                        xyzSize.setDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);
                }

                isArc = ((gcline.motionMode == 2) || (gcline.motionMode == 3));
                coordList.Add( new coordByLine(iCode, gcline.figureNumber, (xyPoint)gcline.actualPos, gcline.alpha, isArc));
            }
            return newCode.ToString().Replace(',','.');
        }

        private static void clearDrawingnPath()
        {
            xyzSize.resetDimension();
            G0Size.resetDimension();
            pathPenUp.Reset();
            pathPenDown.Reset();
            pathRotaryInfo.Reset();
            pathRuler.Reset();
            pathTool.Reset();
            pathMarker.Reset();
            pathMarkSelection.Reset();
            Simulation.pathSimulation.Reset();
			pathObject.Clear();
            path = pathPenUp;
            onlyZ = 0;
            figureCount = 0;
        }

        // add given coordinates to drawing path
        private static int onlyZ = 0;
        private static int figureCount = 0;

        /// <summary>
        /// add segement to drawing path 'PenUp' or 'PenDown' from old-xyz to new-xyz
        /// </summary>
        private static bool createDrawingPathFromGCode(gcodeByLine newL, gcodeByLine oldL)
        {
            bool passLimit = false;
            var pathOld = path;

            if (newL.isSubroutine && (!oldL.isSubroutine))
                markPath(pathPenUp, (float)newL.actualPos.X, (float)newL.actualPos.Y, 2); // 2=rectangle

            if (!newL.ismachineCoordG53)
            {
                if (((newL.motionMode > 0) && (oldL.motionMode == 0)) || (newL.actualPos.Z < 0))  // G0 = PenUp
                { 	path = pathPenDown; 
					path.StartFigure();
                    if (pathActualDown != null)
                        pathActualDown.StartFigure();
                }
                if (((newL.motionMode == 0) && (oldL.motionMode > 0)) || (newL.actualPos.Z > 0))
                {   path = pathPenUp; path.StartFigure();
                    tempPathInfo = new pathInfo();
                    tempPathInfo.position = new PointF((float)newL.actualPos.X, (float)newL.actualPos.Y);
                    tempPathInfo.angle = gcodeMath.getAlpha((xyPoint)oldL.actualPos, (xyPoint)newL.actualPos);
                }

            if ((path != pathOld))
                {
                    passLimit = true;
                    if (figureMarkerCount <= 0)
                        path.SetMarkers(); //path.StartFigure();
                    else
                    {
                        if (figureMarkerCount != figureCount)
                        { path.SetMarkers(); }// path.StartFigure(); }
                    }

                    if (path == pathPenDown)    // this means pathPenUp ended
                    {
                        if (figureMarkerCount <= 0)
                        {
                            if (pathPenDown.PointCount > 0)
                            {
                                figureCount++;                  // only inc. if old figure was filled
                                oldL.figureNumber = figureCount;
                            }
                        }
                        else
                        {
                            figureCount = figureMarkerCount;
                            oldL.figureNumber = figureCount;
                        }
                        tempPathInfo.info = figureCount.ToString();
                        pathInfoMarker.Add(tempPathInfo);
                    }
                }

                if (newL.motionMode == 0 || newL.motionMode == 1)
                {
                    bool otherAxis = (newL.actualPos.A != oldL.actualPos.A) || (newL.actualPos.B != oldL.actualPos.B) || (newL.actualPos.C != oldL.actualPos.C);
                    otherAxis = otherAxis || (newL.actualPos.U != oldL.actualPos.U) || (newL.actualPos.V != oldL.actualPos.V) || (newL.actualPos.W != oldL.actualPos.W);
                    if ((newL.actualPos.X != oldL.actualPos.X) || (newL.actualPos.Y != oldL.actualPos.Y) || otherAxis || (oldL.motionMode == 2 || oldL.motionMode == 3))
                    {
                        if ((Properties.Settings.Default.ctrl4thUse) && (path == pathPenDown))
                        {
                            if (passLimit)
                                pathRotaryInfo.StartFigure();
                            float scale = (float)Properties.Settings.Default.rotarySubstitutionDiameter * (float)Math.PI / 360;
                            if (Properties.Settings.Default.ctrl4thInvert)
                                scale = scale * -1;

                            float newR = 0, oldR = 0;
                            if (Properties.Settings.Default.ctrl4thName == "A") { oldR = (float)oldL.actualPos.A * scale; newR = (float)newL.actualPos.A * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "B") { oldR = (float)oldL.actualPos.B * scale; newR = (float)newL.actualPos.B * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "C") { oldR = (float)oldL.actualPos.C * scale; newR = (float)newL.actualPos.C * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "U") { oldR = (float)oldL.actualPos.U * scale; newR = (float)newL.actualPos.U * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "V") { oldR = (float)oldL.actualPos.V * scale; newR = (float)newL.actualPos.V * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "W") { oldR = (float)oldL.actualPos.W * scale; newR = (float)newL.actualPos.W * scale; }

                            if (Properties.Settings.Default.ctrl4thOverX)
                            {
                                pathRotaryInfo.AddLine((float)oldL.actualPos.X, oldR, (float)newL.actualPos.X, newR); // rotary over X
                                xyzSize.setDimensionY(newR);
                            }
                            else
                            {
                                pathRotaryInfo.AddLine(oldR, (float)oldL.actualPos.Y, newR, (float)newL.actualPos.Y); // rotary over Y
                                xyzSize.setDimensionX(newR);
                            }
                        }
                        path.AddLine((float)oldL.actualPos.X, (float)oldL.actualPos.Y, (float)newL.actualPos.X, (float)newL.actualPos.Y);
						if ((path == pathPenDown) && (pathActualDown != null))
							pathActualDown.AddLine((float)oldL.actualPos.X, (float)oldL.actualPos.Y, (float)newL.actualPos.X, (float)newL.actualPos.Y);
                        onlyZ = 0;  // x or y has changed
                    }
                    if (newL.actualPos.Z != oldL.actualPos.Z)  //else
                    { onlyZ++; }

                    // mark Z-only movements - could be drills
                    if ((onlyZ > 1) && (passLimit) && (path == pathPenUp))  // pen moved from -z to +z
                    {
                        float markerSize = 1;
                        if (!Properties.Settings.Default.importUnitmm)
                        { markerSize /= 25.4F; }
                        createMarker(pathPenDown, (xyPoint)newL.actualPos, markerSize, 1, false);       // draw cross
                        if ((path == pathPenDown) && (pathActualDown != null))
                            createMarker(pathActualDown, (xyPoint)newL.actualPos, markerSize, 1, false);       // draw cross
                        createMarker(pathPenUp, (xyPoint)newL.actualPos, markerSize, 4, false);       // draw circle
                        path = pathPenUp;
                        onlyZ = 0;
                        passLimit = false;
                    }
                }
                //         }
                else if ((newL.motionMode == 2 || newL.motionMode == 3) && (newL.i != null || newL.j != null))
                {
                    if (newL.i == null) { newL.i = 0; }
                    if (newL.j == null) { newL.j = 0; }

                    ArcProperties arcMove;
                    arcMove = gcodeMath.getArcMoveProperties((xyPoint)oldL.actualPos, (xyPoint)newL.actualPos, newL.i, newL.j, (newL.motionMode == 2));
                    centerList.Add(new coordByLine(newL.lineNumber, figureCount, arcMove.center, 0, true));

                    newL.distance = Math.Abs(arcMove.radius * arcMove.angleDiff);
                    float x1 = (float)(arcMove.center.X - arcMove.radius);
                    float x2 = (float)(arcMove.center.X + arcMove.radius);
                    float y1 = (float)(arcMove.center.Y - arcMove.radius);
                    float y2 = (float)(arcMove.center.Y + arcMove.radius);
                    float r2 = 2 * (float)arcMove.radius;
                    float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
                    float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);
                    if (arcMove.radius > 0)
                    {   path.AddArc(x1, y1, r2, r2, aStart, aDiff);
						if ((path == pathPenDown) && (pathActualDown != null))
							pathActualDown.AddArc(x1, y1, r2, r2, aStart, aDiff);			
					}
                    else
                    {   if (errorString.Length == 0) errorString += "ERROR ";
                        errorString += string.Format("Line:{0} radius=0 '{1}' | ", (newL.lineNumber + 1), newL.codeLine);
                    }
                    if (!newL.ismachineCoordG53)
                        xyzSize.setDimensionCircle(arcMove.center.X, arcMove.center.Y, arcMove.radius, aStart, aDiff);        // calculate new dimensions
                }
            }
            if (path == pathPenDown)
                newL.figureNumber = figureCount;
            else
                newL.figureNumber = -1;

            return true;// figureStart;
        }

        private static void addArrow(GraphicsPath path, PointF p2, double angle, string info, bool showArrow, bool showInfo)
        {
            double msize = (float)Math.Max(Math.Sqrt(xyzSize.dimx * xyzSize.dimx + xyzSize.dimy * xyzSize.dimy) / 80f, 0.5);
            float emSize = (float)Math.Max((msize * 1), 0.5);

            //            Logger.Trace("addArrow x:{0:0.00}  y:{1:0.00}   size:{2:0.00}", p2.X, p2.Y, msize);
            if (showArrow)
            {   try
                {
                    double aoff = Math.PI / 6;
                    float pointToX1 = (float)(p2.X - msize * Math.Cos(angle + aoff));
                    float pointToY1 = (float)(p2.Y - msize * Math.Sin(angle + aoff));
                    float pointToX2 = (float)(p2.X - msize * Math.Cos(angle - aoff));
                    float pointToY2 = (float)(p2.Y - msize * Math.Sin(angle - aoff));

                    path.AddLine((float)p2.X, (float)p2.Y, pointToX1, pointToY1);
                    path.AddLine(pointToX1, pointToY1, pointToX2, pointToY2);
                    path.AddLine(pointToX2, pointToY2, (float)p2.X, (float)p2.Y);
                }
                catch (Exception err) { Logger.Error(err, "addArrow Addline - msize:{0}   ", msize); }
            }
            if (showInfo)
            {   try
                {
                    float pointToX0 = (float)(p2.X + msize * Math.Cos(angle));
                    float pointToY0 = (float)(p2.Y + msize * Math.Sin(angle));
                    FontFamily family = new FontFamily("Lucida Console");
                    int fontStyle = (int)FontStyle.Italic;

                    PointF origin = new PointF((float)pointToX0, -(float)pointToY0);
                    StringFormat format = StringFormat.GenericDefault;

                    GraphicsPath tmpString = new GraphicsPath();
                    tmpString.AddString(info, family, fontStyle, emSize, origin, format);
                    RectangleF boundRect = tmpString.GetBounds();

                    Matrix translateMatrix = new Matrix();
                    translateMatrix.Translate(-boundRect.Width / 2, -boundRect.Height / 2);
                    tmpString.Transform(translateMatrix);
                    translateMatrix.Scale(1, -1);
                    tmpString.Transform(translateMatrix);

                    path.AddPath(tmpString, false);
                }
                catch (Exception err) { Logger.Error(err, "addArrow AddString - emSize:{0}   ", emSize); }
            }
        }


        private static void calculateProcessTime(gcodeByLine newL, gcodeByLine oldL)
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

        private static void markPath(GraphicsPath path, float x, float y, int type)
        {   float markerSize = 1;
            if (!Properties.Settings.Default.importUnitmm)
            { markerSize /= 25.4F; }
            createMarker(path, x, y, markerSize, type, false);    // draw circle
        }

        // setup drawing area 
        public static void calcDrawingArea()
        {
            double extend = 1.01;                                                       // extend dimension a little bit
            double roundTo = 5;                                                         // round-up dimensions
            if (!Properties.Settings.Default.importUnitmm)
            { roundTo = 0.25; }

            if ((xyzSize.dimx == 0) && (xyzSize.dimy == 0))
            {   xyzSize.setDimensionXY(G0Size.minx, G0Size.miny);
                xyzSize.setDimensionXY(G0Size.maxx, G0Size.maxy);
                Logger.Info("xyz-Dimension=0, use G0-Dimension dim-x {0} dim-y {1}", G0Size.dimx, G0Size.dimy);
            }

            if ((xyzSize.miny == 0) && (xyzSize.maxy == 0))
            {   xyzSize.miny = -1; xyzSize.maxy = 1; }

            drawingSize.minX = Math.Floor(xyzSize.minx* extend / roundTo)* roundTo;                  // extend dimensions
            if (drawingSize.minX >= 0) { drawingSize.minX = -roundTo; }                                          // be sure to show 0;0 position
            drawingSize.maxX = Math.Ceiling(xyzSize.maxx* extend / roundTo) * roundTo;
            drawingSize.minY = Math.Floor(xyzSize.miny* extend / roundTo) * roundTo;
            if (drawingSize.minY >= 0) { drawingSize.minY = -roundTo; }
            drawingSize.maxY = Math.Ceiling(xyzSize.maxy* extend / roundTo) * roundTo;

            createRuler(drawingSize.minX, drawingSize.maxX, drawingSize.minY, drawingSize.maxY);

            createMarkerPath();

            createDimensionBox();
        }

        public static void createDimensionBox()
        {
            pathDimension.Reset();
            pathDimension.StartFigure();
            pathDimension.AddLine((float)xyzSize.minx, (float)xyzSize.miny, (float)xyzSize.minx, (float)xyzSize.maxy);
            pathDimension.AddLine((float)xyzSize.minx, (float)xyzSize.maxy, (float)xyzSize.maxx, (float)xyzSize.maxy);
            pathDimension.AddLine((float)xyzSize.maxx, (float)xyzSize.maxy, (float)xyzSize.maxx, (float)xyzSize.miny);
            pathDimension.CloseFigure();
        }


        public static void createMarkerPath()
        { createMarkerPath(false, new xyPoint(0,0)); }

        public static void createMarkerPath(bool showCenter, xyPoint center)
        { createMarkerPath(showCenter, center, center); }
        public static void createMarkerPath(bool showCenter, xyPoint center, xyPoint last)
        {
            float msize = (float) Math.Sqrt(xyzSize.dimx * xyzSize.dimx + xyzSize.dimy * xyzSize.dimy) / 40f;
            msize = Math.Max(msize, 2);
//            createMarker(pathTool,   (xyPoint)grbl.posWork, msize, 2);

            if (tangentialAxisEnable)
            {   double posAngle = 0;
                double factor = Math.PI / ((double)Properties.Settings.Default.importGCTangentialTurn/2);
                if (tangentialAxisName == "C") { posAngle = factor * grbl.posWork.C; }
                else if (tangentialAxisName == "B") { posAngle = factor * grbl.posWork.B; }
                else if (tangentialAxisName == "A") { posAngle = factor * grbl.posWork.A; }
                else if (tangentialAxisName == "Z") { posAngle = factor * grbl.posWork.Z; }
                createMarkerArrow(pathTool, msize, (xyPoint)grbl.posWork, posAngle, 1);
				createMarkerArrow(pathMarker, msize, grbl.posMarker, grbl.posMarkerAngle * 360 / (double)Properties.Settings.Default.importGCTangentialTurn);
			}
            else
            {   createMarker(pathTool,   (xyPoint)grbl.posWork, msize, 2);
				createMarker(pathMarker, grbl.posMarker, msize, 3);
			}

            if (showCenter)
            {   createMarker(pathMarker, center, msize, 0,false);
                pathMarker.StartFigure(); pathMarker.AddLine((float)last.X, (float)last.Y, (float)center.X, (float)center.Y);
                pathMarker.StartFigure(); pathMarker.AddLine((float)center.X, (float)center.Y, (float)grbl.posMarker.X, (float)grbl.posMarker.Y);
            }
            if (Properties.Settings.Default.ctrl4thUse)
            {   float scale = (float)Properties.Settings.Default.rotarySubstitutionDiameter * (float)Math.PI / 360;
                if (Properties.Settings.Default.ctrl4thInvert)
                    scale = scale * -1;

                if (Properties.Settings.Default.ctrl4thOverX)
                {   createMarker(pathTool, (float)grbl.posWork.X, (float)grbl.posWork.A*scale, msize, 2);
                }
                else
                {   createMarker(pathTool, (float)grbl.posWork.A*scale, (float)grbl.posWork.Y, msize, 2);
                }
            }
        }
        private static void createMarkerArrow(GraphicsPath path, float msize, xyPoint pos, double angle, int type=0)
        {   float pointToX = (float)(pos.X + 2 * msize * Math.Cos(angle));		// direction to show
            float pointToY = (float)(pos.Y + 2 * msize * Math.Sin(angle));
			double aoff = Math.PI/2;
			if (type > 0)
				angle -= Math.PI/4;
            float pointToX1 = (float)(pos.X + msize * Math.Cos(angle + aoff));
            float pointToY1 = (float)(pos.Y + msize * Math.Sin(angle + aoff));
            float pointToX2 = (float)(pos.X + msize * Math.Cos(angle + 2* aoff));
            float pointToY2 = (float)(pos.Y + msize * Math.Sin(angle + 2* aoff));
            float pointToX3 = (float)(pos.X + msize * Math.Cos(angle + 3* aoff));
            float pointToY3 = (float)(pos.Y + msize * Math.Sin(angle + 3* aoff));
            float pointToX4 = (float)(pos.X + msize * Math.Cos(angle + 4* aoff));
            float pointToY4 = (float)(pos.Y + msize * Math.Sin(angle + 4* aoff));
            path.Reset();
			// draw outline
            path.StartFigure();
            path.AddLine(pointToX, pointToY, pointToX1, pointToY1);
			path.AddLine(pointToX1, pointToY1, pointToX2, pointToY2);
			path.AddLine(pointToX2, pointToY2, pointToX3, pointToY3);
			if (type > 0)
			{	path.AddLine(pointToX3, pointToY3, pointToX4, pointToY4);
				path.AddLine(pointToX4, pointToY4, pointToX, pointToY);		// square
			}
			else
				path.AddLine(pointToX3, pointToY3, pointToX, pointToY);		// rhombus
            path.CloseFigure();
			// draw diagonal cross in center
            float ssize = msize / 2;
			if (type > 0)
			{	path.StartFigure(); path.AddLine((float)pos.X - ssize, (float)pos.Y, (float)pos.X + ssize, (float)pos.Y);
				path.StartFigure(); path.AddLine((float)pos.X, (float)pos.Y + ssize, (float)pos.X, (float)pos.Y - ssize);				
			}
			else
			{
                ssize = msize / 3;
                path.StartFigure(); path.AddLine((float)pos.X - ssize, (float)pos.Y - ssize, (float)pos.X + ssize, (float)pos.Y + ssize);
				path.StartFigure(); path.AddLine((float)pos.X - ssize, (float)pos.Y + ssize, (float)pos.X + ssize, (float)pos.Y - ssize);
			}
        }

        private static void createRuler(double minX, double maxX, double minY, double maxY)
        {
            pathRuler.Reset();
            float unit = 1;
            int divider = 1;
            int divider_long    = 100; //
            int divider_med     = 10;
            int divider_short   = 5;
            int show_short      = 500;
            int show_smallest   = 200;
            float length1 = 1F, length2 = 2F, length3 = 3F, length5 = 5F;
            if (!Properties.Settings.Default.importUnitmm)
            {   divider = 16;
                divider_long    = divider; 
                divider_med     = 8;
                divider_short   = 4;
                show_short      = 20*divider;
                show_smallest   = 6*divider;
                minX = minX * divider; // unit;
                maxX = maxX * divider; // unit;
                minY = minY * divider; // unit;
                maxY = maxY * divider; // unit;
                length1 = 0.05F; length2 = 0.1F; length3 = 0.15F; length5 = 0.25F;
            }
            float x = 0, y = 0;
            for (int i = 0; i < maxX; i++)          // horizontal ruler
            {   pathRuler.StartFigure();
                x = (float)i*unit / (float)divider;
                if (i % divider_short == 0)
                {   if (i % divider_long == 0)
                    { pathRuler.AddLine(x, 0, x, -length5); }  // 100                    
                    else if ((i % divider_med == 0) && (maxX < (2* show_short)))
                    { pathRuler.AddLine(x, 0, x, -length3); }  // 10                  
                    else if (maxX < show_short)
                    { pathRuler.AddLine(x, 0, x, -length2); }  // 5
                }
                else if (maxX < show_smallest)
                { pathRuler.AddLine(x, 0, x, -length1); }  // 1
            }
            for (int i = 0; i > minX; i--)          // horizontal ruler
            {
                pathRuler.StartFigure();
                x = (float)i * unit / (float)divider;
                if (i % divider_short == 0)
                {
                    if (i % divider_long == 0)
                    { pathRuler.AddLine(x, 0, x, -length5); }  // 100                    
                    else if ((i % divider_med == 0) && (Math.Abs(minX) < (2 * show_short)))
                    { pathRuler.AddLine(x, 0, x, -length3); }  // 10                  
                    else if (Math.Abs(minX) < show_short)
                    { pathRuler.AddLine(x, 0, x, -length2); }  // 5
                }
                else if (Math.Abs(minX) < show_smallest)
                { pathRuler.AddLine(x, 0, x, -length1); }  // 1
            }
            for (int i = 0; i < maxY; i++)          // vertical ruler
            {   pathRuler.StartFigure();
                y = (float)i*unit / (float)divider;
                if (i % divider_short == 0)
                {   if (i % divider_long == 0)
                    { pathRuler.AddLine(0, y, -length5, y); } // 100                   
                    else if ((i % divider_med == 0) && (maxY < (2* show_short)))
                    { pathRuler.AddLine(0, y, -length3, y); } // 10           
                    else if (maxY < show_short)
                    { pathRuler.AddLine(0, y, -length2, y); } // 5
                }
                else if (maxY < show_smallest)
                { pathRuler.AddLine(0, y, -length1, y); }     // 1
            }
            for (int i = 0; i > minY; i--)          // vertical ruler
            {
                pathRuler.StartFigure();
                y = (float)i * unit / (float)divider;
                if (i % divider_short == 0)
                {
                    if (i % divider_long == 0)
                    { pathRuler.AddLine(0, y, -length5, y); } // 100                   
                    else if ((i % divider_med == 0) && (Math.Abs(minY) < (2 * show_short)))
                    { pathRuler.AddLine(0, y, -length3, y); } // 10           
                    else if (Math.Abs(minY) < show_short)
                    { pathRuler.AddLine(0, y, -length2, y); } // 5
                }
                else if (Math.Abs(minY) < show_smallest)
                { pathRuler.AddLine(0, y, -length1, y); }     // 1
            }
        }
        private static void createMarker(GraphicsPath path, xyPoint center, float dimension, int style, bool rst = true)
        {   createMarker(path, (float)center.X, (float)center.Y, dimension, style, rst); }

        static readonly object pathDrawLock = new object();
        private static void createMarker(GraphicsPath path, float centerX,float centerY, float dimension,int style,bool rst=true)
        {
            if (dimension == 0) { return; }
            lock (pathDrawLock)
            {
                if (rst)
                    path.Reset();
                if (style == 0)   // horizontal cross
                {
                    path.StartFigure(); path.AddLine(centerX, centerY + dimension, centerX, centerY - dimension);
                    path.StartFigure(); path.AddLine(centerX + dimension, centerY, centerX - dimension, centerY);
                }
                else if (style == 1)   // diagonal cross
                {
                    path.StartFigure(); path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                    path.StartFigure(); path.AddLine(centerX - dimension, centerY - dimension, centerX + dimension, centerY + dimension);
                }
                else if (style == 2)            // box
                {
                    path.StartFigure();
                    path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY + dimension);
                    path.AddLine(centerX + dimension, centerY + dimension, centerX + dimension, centerY - dimension);

                    path.AddLine(centerX + dimension, centerY - dimension, centerX, centerY);
                    path.AddLine(centerX, centerY, centerX - dimension, centerY - dimension);

                    path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY - dimension);
                    path.AddLine(centerX - dimension, centerY - dimension, centerX - dimension, centerY + dimension);
                    path.CloseFigure();
                }
                else if (style == 3)            // marker
                {
                    path.StartFigure();
                    path.AddLine(centerX, centerY, centerX, centerY - dimension);
                    path.AddLine(centerX, centerY - dimension, centerX + dimension, centerY);
                    path.AddLine(centerX + dimension, centerY, centerX, centerY + dimension);
                    path.AddLine(centerX, centerY + dimension, centerX - dimension, centerY);
                    path.AddLine(centerX - dimension, centerY, centerX, centerY - dimension);
                    path.CloseFigure();
                }
                else
                {
                    path.StartFigure(); path.AddArc(centerX - dimension, centerY - dimension, 2 * dimension, 2 * dimension, 0, 360);
                }
            }
        }				
    }

    public struct drawingProperties
    {
        public double minX,minY,maxX,maxY;
        public void drawingProperty()
         { minX = 0;minY = 0;maxX = 0;maxY=0; }
    };

}
