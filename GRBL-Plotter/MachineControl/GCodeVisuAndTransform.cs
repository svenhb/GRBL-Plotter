/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
 */

//#define debuginfo 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace GRBL_Plotter
{
    public partial class GCodeVisuAndTransform
    {   public enum translate { None, ScaleX, ScaleY, Offset1, Offset2, Offset3, Offset4, Offset5, Offset6, Offset7, Offset8, Offset9, MirrorX, MirrorY, MirrorRotary };
        public Dimensions xyzSize = new Dimensions();
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
        public static GraphicsPath path = pathPenUp;

        public enum convertMode { nothing, removeZ, convertZToS };

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool containsG2G3Command()
        { return modal.containsG2G3; }
        public bool containsG91Command()
        { return modal.containsG91; }

        private xyPoint origWCOLandMark = new xyPoint();
        private List<coordByLine> coordListLandMark = new List<coordByLine>();
        /// <summary>
        /// copy actual gcode-pathPenDown to background path with machine coordinates
        /// </summary>
        public void setPathAsLandMark(bool clear=false)
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
                coordListLandMark.Add(new coordByLine(0, -1, gcline.actualPos + (xyPoint)grbl.posWCO, isArc));
            }
            origWCOLandMark = (xyPoint)grbl.posWCO;
        }
        /// <summary>
        /// translate background path with machine coordinates to take account of changed WCO
        /// </summary>
        public void updatePathPositions()
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

        private xyPoint origWCOMachineLimit = new xyPoint();
        /// <summary>
        /// create paths with machine limits and tool positions in machine coordinates
        /// </summary>
        public void drawMachineLimit(toolPos[] toolTable)
        {
            float offsetX =  (float)grbl.posWCO.X;   // (float)machinePos.X - (float)grbl.posWork.X;// toolPos.X;
            float offsetY =  (float)grbl.posWCO.Y;   // (float)machinePos.Y- (float)grbl.posWork.Y;//toolPos.Y;
            float x1 = (float)Properties.Settings.Default.machineLimitsHomeX - offsetX;
            float y1 = (float)Properties.Settings.Default.machineLimitsHomeY - offsetY;
            float rx = (float)Properties.Settings.Default.machineLimitsRangeX;
            float ry = (float)Properties.Settings.Default.machineLimitsRangeY;
            float extend = 2 * rx;
            RectangleF pathRect1 = new RectangleF(x1, y1, rx, ry);
            RectangleF pathRect2 = new RectangleF(x1- extend, y1- extend, rx + 2 * extend, ry + 2 * extend); //(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
            pathMachineLimit.Reset();
            pathMachineLimit.StartFigure();
            pathMachineLimit.AddRectangle(pathRect1);
            pathMachineLimit.AddRectangle(pathRect2);
            pathToolTable.Reset();
            if ((toolTable != null) && (toolTable.Length >= 1))
            {
                Matrix matrix = new Matrix();
                matrix.Scale(1, -1);
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
                    catch { }
                }
            }
            origWCOMachineLimit = (xyPoint)grbl.posWCO;
        }

        private static float maxStep = 100;
        /// <summary>
        /// create height map path in work coordinates
        /// </summary>
        public void drawHeightMap(HeightMap Map)
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
        public string applyHeightMap(IList<string> oldCode, HeightMap Map)
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
        private List<gcodeByLine> gcodeList;        // keep original program
        private List<coordByLine> coordList;        // get all coordinates (also subroutines)
        private List<coordByLine> centerList;       // get center of arcs

        private gcodeByLine oldLine = new gcodeByLine();    // actual parsed line
        private gcodeByLine newLine = new gcodeByLine();    // last parsed line

        private modalGroup modal = new modalGroup();        // keep modal states and helper variables
        private int figureMarkerCount;
        private Dictionary<int, int> figureCountNr = new Dictionary<int, int>();
        /// <summary>
        /// Entrypoint for generating drawing path from given gcode
        /// </summary>
        public void getGCodeLines(IList<string> oldCode, bool processSubs=false)
        {
#if (debuginfo)
            log.Add("   GCodeVisu getGCodeLines");
            File.WriteAllText("logfile.txt", "");
#endif
            string[] GCode = oldCode.ToArray<string>();
            string singleLine;
            modal = new modalGroup();               // clear

            gcodeList = new List<gcodeByLine>();    //.Clear();
            coordList = new List<coordByLine>();    //.Clear();
            centerList = new List<coordByLine>();    //.Clear();
            clearDrawingnPath();                    // reset path, dimensions
            figureMarkerCount = 0;
//            int figureMarkerLine = 0;
            bool figureActive = false;

            oldLine.resetAll(grbl.posWork);         // reset coordinates and parser modes, set initial pos
            newLine.resetAll();                     // reset coordinates and parser modes
            bool programEnd = false;
            figureCount = 1;                        // will be inc. in createDrawingPathFromGCode
            bool isArc = false;
            bool upDateFigure = false;
            figureCountNr.Clear();

            for (int lineNr = 0; lineNr < GCode.Length; lineNr++)   // go through all gcode lines
            {
                modal.resetSubroutine();                            // reset m, p, o, l Word
                singleLine = GCode[lineNr].ToUpper().Trim();        // get line, remove unneeded chars
                if (singleLine == "")
                    continue;

                if (GCode[lineNr].Contains(xmlMarker.figureStart))                    // check if marker available
                {   figureMarkerCount++;
                    figureActive = true;
                }

                if (processSubs && programEnd)
                { singleLine = "( " + singleLine + " )"; }          // don't process subroutine itself when processed

                newLine.parseLine(lineNr, singleLine, ref modal);
                calcAbsPosition(newLine, oldLine);                  // Calc. absolute positions and set object dimension: xyzSize.setDimension

                if (figureMarkerCount > 0)                          // preset figure nr
                    newLine.figureNumber = figureMarkerCount;

                if ((modal.mWord == 98) && processSubs)
                    newLine.codeLine = "(" + GCode[lineNr] + ")";
                else
                {   if (processSubs && programEnd)
                        newLine.codeLine = "( " + GCode[lineNr] + " )";   // don't process subroutine itself when processed
                    else
                        newLine.codeLine = GCode[lineNr];                 // store original line
                }

                if (!programEnd)
                    upDateFigure = createDrawingPathFromGCode(newLine, oldLine);        // add data to drawing path

                if (figureMarkerCount > 0)
                    if (figureActive)
                        newLine.figureNumber = figureMarkerCount;
                    else
                        newLine.figureNumber = -1;

                isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                oldLine = new gcodeByLine(newLine);                     // get copy of newLine      
                gcodeList.Add(new gcodeByLine(newLine));                // add parsed line to list
                coordList.Add(new coordByLine(lineNr, newLine.figureNumber, (xyPoint)newLine.actualPos, isArc));
#if (debuginfo)
                File.AppendAllText("logfile.txt",lineNr+"  "+ newLine.figureNumber+"  "+ newLine.actualPos.X+"  "+ newLine.actualPos.Y + "\r");
#endif
                if ((modal.mWord == 30) || (modal.mWord == 2)) { programEnd = true; }
                if (modal.mWord == 98)
                {   if (lastSubroutine[0] == modal.pWord)
                        addSubroutine(GCode, lastSubroutine[1], lastSubroutine[2], modal.lWord, processSubs);
                    else
                        findAddSubroutine(modal.pWord, GCode, modal.lWord, processSubs);      // scan complete GCode for matching O-word
                }

                if (GCode[lineNr].Contains(xmlMarker.figureEnd))                    // check if marker available
                {   figureActive = false;  }
            }
        }
        /// <summary>
        /// Find and add subroutine within given gcode
        /// </summary>
        private string findAddSubroutine(int foundP, string[] GCode, int repeat, bool processSubs)
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
        private int[] lastSubroutine = new int[] { 0, 0, 0 };

        /// <summary>
        /// process subroutines
        /// </summary>
        private void addSubroutine(string[] GCode, int start, int stop, int repeat, bool processSubs)
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
                    if (!newLine.ismachineCoordG53)
                    {
                        isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                        coordList.Add(new coordByLine(subLineNr, newLine.figureNumber, (xyPoint)newLine.actualPos,isArc));
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
        /// Calc. absolute positions and set object dimension: xyzSize.setDimension
        /// </summary>
        private void calcAbsPosition(gcodeByLine newLine, gcodeByLine oldLine)
        {
            if (!newLine.ismachineCoordG53)         // only use world coordinates
            {   if ((newLine.motionMode >= 1) && (oldLine.motionMode == 0))     // take account of last G0 move
                {   xyzSize.setDimensionX(oldLine.actualPos.X);
                    xyzSize.setDimensionY(oldLine.actualPos.Y);
                }
                if (newLine.x != null)
                {   if (newLine.isdistanceModeG90)  // absolute move
                    {   newLine.actualPos.X = (double)newLine.x;
                        if(newLine.motionMode >=1 )//if (newLine.actualPos.X != toolPos.X)            // don't add actual tool pos
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
                {   if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.A = (double)newLine.a;
                    else
                        newLine.actualPos.A = oldLine.actualPos.A + (double)newLine.a;
                }
                else
                    newLine.actualPos.A = oldLine.actualPos.A;

                if (newLine.b != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.B = (double)newLine.b;
                    else
                        newLine.actualPos.B = oldLine.actualPos.B + (double)newLine.b;
                }
                else
                    newLine.actualPos.B = oldLine.actualPos.B;

                if (newLine.c != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.C = (double)newLine.c;
                    else
                        newLine.actualPos.C = oldLine.actualPos.C + (double)newLine.c;
                }
                else
                    newLine.actualPos.C = oldLine.actualPos.C;

                if (newLine.u != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.U = (double)newLine.u;
                    else
                        newLine.actualPos.U = oldLine.actualPos.U + (double)newLine.u;
                }
                else
                    newLine.actualPos.U = oldLine.actualPos.U;

                if (newLine.v != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.V = (double)newLine.v;
                    else
                        newLine.actualPos.V = oldLine.actualPos.V + (double)newLine.v;
                }
                else
                    newLine.actualPos.V = oldLine.actualPos.V;

                if (newLine.w != null)
                {
                    if (newLine.isdistanceModeG90)  // absolute move
                        newLine.actualPos.W = (double)newLine.w;
                    else
                        newLine.actualPos.W = oldLine.actualPos.W + (double)newLine.w;
                }
                else
                    newLine.actualPos.W = oldLine.actualPos.W;
            }
        }

        /// <summary>
        /// set marker into drawing on xy-position of desired line
        /// </summary>
        public void setPosMarkerLine(int line, bool markFigure=true)
        {
#if (debuginfo)
            log.Add("   GCodeVisu setPosMarkerLine line: " + line.ToString());
#endif
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
                        if (coordList[line].isArc)
                        {   foreach (coordByLine point in centerList)
                            {   if (point.lineNumber == coordList[line].lineNumber)
                                {   center = point.actualPos; showCenter = true; break; }
                            }
                        }
                        createMarkerPath(showCenter,center, coordList[line-1].actualPos);

                        figureNr = coordList[line].figureNumber;
               //         Logger.Trace(string.Format("1 Line:{0} Figure:{1} code:{2}",line,figureNr, gcodeList[line].codeLine));
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
                                line--; // FCTB line 1-x = index 0 - x
                                grbl.posMarker = (xyPoint)gcline.actualPos;
                                if (gcline.isArc)
                                {   foreach (coordByLine point in centerList)
                                    {   if (point.lineNumber == line)
                                        {   center = point.actualPos; showCenter = true;  break; }
                                    }
                                }
                                createMarkerPath(showCenter, center, last);

                                figureNr = coordList[line].figureNumber;
                   //             Logger.Trace(string.Format("2 Line:{0} Figure:{1} code:{2}", line, figureNr, gcodeList[line].codeLine));
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
            catch { }
        }

        /// <summary>
        /// find gcode line with xy-coordinates near by given coordinates
        /// </summary>
        public int setPosMarkerNearBy(xyPoint pos)
        {   List<coordByLine> tmpList = new List<coordByLine>();     // get all coordinates (also subroutines)
            int figureNr;
            xyPoint center = new xyPoint(0, 0);
            bool showCenter = false;
            foreach (coordByLine gcline in coordList)
            {   gcline.calcDistance(pos);       // calculate distance work coordinates
                tmpList.Add(gcline);            // add to new list
            }
            if (Properties.Settings.Default.guiBackgroundShow && (coordListLandMark.Count > 1))
            {   foreach (coordByLine gcline in coordListLandMark)
                {   gcline.calcDistance(pos+(xyPoint)grbl.posWCO);      // calculate distance machine coordinates
                    tmpList.Add(new coordByLine(0, gcline.figureNumber, gcline.actualPos - (xyPoint)grbl.posWCO, gcline.distance)); // add as work coord.
                }
            }
            int line = 0;
            List<coordByLine> SortedList = tmpList.OrderBy(o => o.distance).ToList();
            grbl.posMarker = (xyPoint)SortedList[line].actualPos;
            figureNr = SortedList[line].figureNumber;
#if (debuginfo)
            log.Add("   GCodeVisu setPosMarkerNearBy found figure: " + figureNr.ToString() + " last: "+ lastFigureNumber.ToString());
#endif
            if (SortedList[line].isArc)
            {   foreach (coordByLine point in centerList)
                {   if (point.lineNumber == SortedList[line].lineNumber)
                    {   center = point.actualPos; showCenter = true; break; }
                }
            }
            createMarkerPath(showCenter, center);
            if (figureNr != lastFigureNumber)
                markSelectedFigure(figureNr);
            lastFigureNumber = figureNr;

            return SortedList[line].lineNumber;
        }
        private int lastFigureNumber = -1;

        /// <summary>
        /// return GCode lineNr of first point in selected path (figure)
        /// </summary>
        public int getLineOfFirstPointInFigure()
        {   if (lastFigureNumber < 0)
                return -1;
            foreach (coordByLine gcline in coordList)           // start search at beginning
            {   if (gcline.figureNumber == lastFigureNumber)    // 1st occurance = hit
                {   return gcline.lineNumber;
                }
            }
            return -1;
        }

        /// <summary>
        /// return GCode lineNr of last point in selected path (figure)
        /// </summary>
        public int getLineOfEndPointInFigure(int start=0)
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
          /*          if (figureMarkerCount > 0)
                    {   for (int v = i; v < coordList.Count; v++)  //coordList[i].lineNumber
                        {   if (gcodeList[v].codeLine.Contains(xmlMarker.figureEnd))
                                return gcodeList[v].lineNumber;
                        }
                    }*/
                    return coordList[i].lineNumber;
                }
            }
            return -1;
        }
        public int getLineOfEndPointInFigureExtend(int start = 0)
        {
            int max = Math.Min(coordList.Count - 1, (start + 5));
            for (int k = start-1; k < max; k++)
            {   if (gcodeList[k].codeLine.StartsWith(xmlMarker.figureEnd))
                    return k;
            }
            return start;
        }

        public void markSelectedGroup(int start = 0)
        {   List<int> figures = new List<int>();
            int figNr = 0;

            pathMarkSelection.Reset();
            GraphicsPath tmpPath = new GraphicsPath();
            tmpPath.Reset();

            GraphicsPathIterator myPathIterator = new GraphicsPathIterator(pathPenDown);
            myPathIterator.Rewind();

            for (int line = start; line < gcodeList.Count; line++)
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
//                    Logger.Trace("copied " + figNr.ToString());
//                    tmpPath.Reset();
                }
            }

            RectangleF selectionBounds = pathMarkSelection.GetBounds();
            float centerX = selectionBounds.X + selectionBounds.Width / 2;
            float centerY = selectionBounds.Y + selectionBounds.Height / 2;
    //        selectedFigureInfo = string.Format("Selection: {0}\r\nWidth : {1:0.000}\r\nHeight: {2:0.000}\r\nCenter: X {3:0.000} Y {4:0.000}", figureNr, selectionBounds.Width, selectionBounds.Height, centerX, centerY);
        }

        public string selectedFigureInfo = "";
        /// <summary>
        /// create path of selected figure
        /// </summary>
        public void markSelectedFigure(int figureNr)
        {
#if (debuginfo)
            log.Add("   GCodeVisu markSelectedFigure fnr: " + fnr.ToString());
#endif
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
        public string transformGCodeRotate(double angle, double scale, xyPoint offset)
        {
            Logger.Debug("Rotate angle: {0}", angle);
#if (debuginfo)
            log.Add("   GCodeVisu transform Rotate");
#endif
            if (gcodeList == null) return "";
            xyPoint centerOfFigure = xyzSize.getCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = getCenterOfMarkedFigure();

            offset = centerOfFigure;

//            pathMarkSelection.Reset(); lastFigureNumber = -1;
            double? newvalx, newvaly, newvali, newvalj;
            oldLine.resetAll(grbl.posWork);         // reset coordinates and parser modes
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))
                { continue; }

                if (!gcline.ismachineCoordG53)
                {
                    if ((gcline.x != null) || (gcline.y != null))
                    {
                        if (gcline.isdistanceModeG90)
                        {
                            newvalx = (gcline.actualPos.X - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.actualPos.Y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            newvaly = (gcline.actualPos.X - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.actualPos.Y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                        }
                        else
                        {
                            if (gcline.x == null) { gcline.x = 0; }
                            if (gcline.y == null) { gcline.y = 0; }
                            newvalx = (gcline.x - offset.X) * Math.Cos(angle * Math.PI / 180) - (gcline.y - offset.Y) * Math.Sin(angle * Math.PI / 180);
                            newvaly = (gcline.x - offset.X) * Math.Sin(angle * Math.PI / 180) + (gcline.y - offset.Y) * Math.Cos(angle * Math.PI / 180);
                        }
                        gcline.x = (newvalx * scale) + offset.X;
                        gcline.y = (newvaly * scale) + offset.Y;
                    }
                    if ((gcline.i != null) || (gcline.j != null))
                    {
                        newvali = (double)gcline.i * Math.Cos(angle * Math.PI / 180) - (double)gcline.j * Math.Sin(angle * Math.PI / 180);
                        newvalj = (double)gcline.i * Math.Sin(angle * Math.PI / 180) + (double)gcline.j * Math.Cos(angle * Math.PI / 180);
                        gcline.i = newvali * scale;
                        gcline.j = newvalj * scale;
                    }

                    calcAbsPosition(gcline, oldLine);
                    oldLine = new gcodeByLine(gcline);   // get copy of newLine
                }
            }
            return createGCodeProg();
        }
        /// <summary>
        /// scale x and y seperatly in %
        /// </summary>
        public string transformGCodeScale(double scaleX, double scaleY)
        {
            Logger.Debug("Scale scaleX: {0}, scale Y: {1}", scaleX, scaleY);
#if (debuginfo)
            log.Add("   GCodeVisu transform Scale");
#endif
            if (gcodeList == null) return "";
            xyPoint centerOfFigure = xyzSize.getCenter();
            if (lastFigureNumber > 0)
                centerOfFigure = getCenterOfMarkedFigure();
//            pathMarkSelection.Reset(); lastFigureNumber = -1;
            double factor_x = scaleX / 100;
            double factor_y = scaleY / 100;

            oldLine.resetAll(grbl.posWork);         // reset coordinates and parser modes
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeByLine gcline in gcodeList)
            {
                if ((lastFigureNumber > 0) && (gcline.figureNumber != lastFigureNumber))
                { continue; }

                if (!gcline.ismachineCoordG53)
                {
                    if (gcline.x != null)
                        gcline.x = gcline.x * factor_x - centerOfFigure.X * (factor_x - 1);
                    if (gcline.y != null)
                        gcline.y = gcline.y * factor_y - centerOfFigure.Y * (factor_y - 1);
                    if (gcline.i != null)
                        gcline.i = gcline.i * factor_x;
                    if (gcline.j != null)
                        gcline.j = gcline.j * factor_y;

                    calcAbsPosition(gcline, oldLine);
                    oldLine = new gcodeByLine(gcline);   // get copy of newLine
                }
            }
            return createGCodeProg();        
        }
        public string transformGCodeOffset(double x, double y, translate shiftToZero)
        {
            Logger.Debug("Transform X: {0}, Y: {1}, Offset: {2}", x, y, shiftToZero);
#if (debuginfo)
            log.Add("   GCodeVisu transform Offset");
#endif
            if (gcodeList == null) return "";
            pathMarkSelection.Reset(); lastFigureNumber = -1;
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
                            {   //getGCodeLine("G0 X0 Y0 (Insert offset movement)", newLine);                   // parse line, fill up newLine.xyz and actualM,P,O
                                modalGroup tmp = new modalGroup();
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
            return createGCodeProg();   
        }

        public string replaceG23()
        {   return createGCodeProg(true, false, false, convertMode.nothing); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        public string convertZ()
        { return createGCodeProg(false, false, false, convertMode.convertZToS); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)
        public string removeZ()
        { return createGCodeProg(false, false, false, convertMode.removeZ); }   // createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, bool removeZ, HeightMap Map=null)

        /// <summary>
        /// Generate GCode from given coordinates in GCodeList
        /// only replace lines with coordinate information
        /// </summary>
        private string createGCodeProg()
        {   return createGCodeProg(false,false,false, convertMode.nothing); }
        private string createGCodeProg(bool replaceG23, bool splitMoves, bool applyNewZ, convertMode specialCmd, HeightMap Map=null)
        {
            Logger.Debug("createGCodeProg replaceG23: {0}, splitMoves: {1}, applyNewZ: {2}, specialCmd: {3}", replaceG23, splitMoves, applyNewZ, specialCmd);
#if (debuginfo)
            log.Add("   GCodeVisu createGCodeProg");
#endif
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

                    //infoCode = "(" + gcline.spindleState + "  " + gcline.coolantState + "  "+gcline.motionMode + "  " + gcline.feedRate + ")";// " ( " + gcline.figureNumber + " ) ";

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

     //                              infoCode = "( Fig-Nr.:"+gcline.figureNumber.ToString()+" )";
    //                if (gcline.info.Length > 0)
     //                   infoCode = "( " + gcline.info + " )";
     //               else
     //                   infoCode = "";
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

                        //infoCode = " ( " + gcline.ismachineCoordG53 +"  "+ gcline.isdistanceModeG90 + " ) ";
                        //infoCode = "("+ gcline.spindleState+")";// " ( " + gcline.figureNumber + " ) ";
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
//                    if (gcline.isSetCoordinateSystem)
//                    { newCode.AppendLine(gcline.codeLine); }    // keep G10 command
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
                //coordList[iCode] = new coordByLine(iCode, gcline.figureNumber, (xyPoint)gcline.actualPos);
                isArc = ((gcline.motionMode == 2) || (gcline.motionMode == 3));
                coordList.Add( new coordByLine(iCode, gcline.figureNumber, (xyPoint)gcline.actualPos, isArc));
            }
            return newCode.ToString().Replace(',','.');
        }

        public GCodeVisuAndTransform()
        {   xyzSize.resetDimension(); }
        private void clearDrawingnPath()
        {
#if (debuginfo)
            log.Add("   GCodeVisu clearDrawingPath");
#endif
            xyzSize.resetDimension();
            pathPenUp.Reset();
            pathPenDown.Reset();
            pathRotaryInfo.Reset();
            pathRuler.Reset();
            pathTool.Reset();
            pathMarker.Reset();
            pathMarkSelection.Reset();
//            lastFigureNumber = -1;
            path = pathPenUp;
        }

        // add given coordinates to drawing path
        private int onlyZ = 0;
        private int figureCount = 0;
        /// <summary>
        /// add segement to drawing path 'PenUp' or 'PenDown' from old-xyz to new-xyz
        /// </summary>
        private bool createDrawingPathFromGCode(gcodeByLine newL, gcodeByLine oldL)
        {
            bool passLimit = false;
            bool figureStart = false;
            var pathOld = path;
            bool xyMove = ((newL.x != null) || (newL.y != 0));

            if (newL.isSubroutine && (!oldL.isSubroutine))
                markPath(pathPenUp, (float)newL.actualPos.X, (float)newL.actualPos.Y, 2); // 2=rectangle

            if (!newL.ismachineCoordG53)    
            {
          /*      if (newL.codeLine.Contains(xmlMarker.penUp))
                { path = pathPenUp; path.StartFigure(); }
                if (newL.codeLine.Contains(xmlMarker.penDown))
                { path = pathPenDown; path.StartFigure(); } */

                if ((newL.motionMode > 0)  && (oldL.motionMode == 0))
                { path = pathPenDown; path.StartFigure(); }
                if ((newL.motionMode == 0) && (oldL.motionMode > 0))
                { path = pathPenUp; path.StartFigure(); }             

                if ((path != pathOld))
                {   passLimit = true;
                    if (figureMarkerCount <= 0)
                        path.SetMarkers(); //path.StartFigure();
                    else
                    {   if (figureMarkerCount != figureCount)
                        { path.SetMarkers(); }// path.StartFigure(); }
                    }
                    if (path == pathPenDown)
                    {
                        figureStart = true;
                        if (figureMarkerCount <= 0)
                        {
                            if (pathPenDown.PointCount > 0)
                            {
                                figureCount++;                  // only inc. if old figure was filled
                                oldL.figureNumber = figureCount;
                            }
                        }
                        else
                        {   figureCount = figureMarkerCount;
                            oldL.figureNumber = figureCount;
                        }
/*#if (debuginfo)
                        File.AppendAllText("logfile.txt", ">>>>"+newL.codeLine + "  " + figureCount +"\r" );
#endif*/
                    }
                }

                if (newL.motionMode == 0 || newL.motionMode == 1)
                {
                    bool otherAxis = (newL.actualPos.A != oldL.actualPos.A) || (newL.actualPos.B != oldL.actualPos.B) || (newL.actualPos.C != oldL.actualPos.C);
                    otherAxis = otherAxis || (newL.actualPos.U != oldL.actualPos.U) || (newL.actualPos.V != oldL.actualPos.V) || (newL.actualPos.W != oldL.actualPos.W);
                    if ((newL.actualPos.X != oldL.actualPos.X) || (newL.actualPos.Y != oldL.actualPos.Y) || otherAxis || (oldL.motionMode == 2 || oldL.motionMode == 3))
                    {
                        if ((Properties.Settings.Default.ctrl4thUse) && (path == pathPenDown))
                        {   if (passLimit)
                                pathRotaryInfo.StartFigure();
                            float scale = (float)Properties.Settings.Default.rotarySubstitutionDiameter * (float)Math.PI/360;
                            if (Properties.Settings.Default.ctrl4thInvert)
                                scale = scale* -1;

                            float newR = 0, oldR = 0;
                            if (Properties.Settings.Default.ctrl4thName == "A") { oldR = (float)oldL.actualPos.A * scale; newR = (float)newL.actualPos.A * scale;}
                            else if (Properties.Settings.Default.ctrl4thName == "B") { oldR = (float)oldL.actualPos.B * scale; newR = (float)newL.actualPos.B * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "C") { oldR = (float)oldL.actualPos.C * scale; newR = (float)newL.actualPos.C * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "U") { oldR = (float)oldL.actualPos.U * scale; newR = (float)newL.actualPos.U * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "V") { oldR = (float)oldL.actualPos.V * scale; newR = (float)newL.actualPos.V * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "W") { oldR = (float)oldL.actualPos.W * scale; newR = (float)newL.actualPos.W * scale; }

                            if (Properties.Settings.Default.ctrl4thOverX)
                            {   pathRotaryInfo.AddLine((float)oldL.actualPos.X, oldR, (float)newL.actualPos.X, newR); // rotary over X
                                xyzSize.setDimensionY(newR);
                            }
                            else
                            {   pathRotaryInfo.AddLine(oldR, (float)oldL.actualPos.Y, newR, (float)newL.actualPos.Y); // rotary over Y
                                xyzSize.setDimensionX(newR);
                            }
                        }
                        path.AddLine((float)oldL.actualPos.X, (float)oldL.actualPos.Y, (float)newL.actualPos.X, (float)newL.actualPos.Y);
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
                        createMarker(pathPenDown, (float)newL.actualPos.X, (float)newL.actualPos.Y, markerSize, 1, false);  // draw cross
                        createMarker(pathPenUp, (float)newL.actualPos.X, (float)newL.actualPos.Y, markerSize, 4, false);    // draw circle
                        path = pathPenUp;
                        onlyZ = 0;
                        passLimit = false;
                    }
                }
            }
            if ((newL.motionMode == 2 || newL.motionMode == 3) && (newL.i != null || newL.j != null))
            {   if (newL.i == null) { newL.i = 0; }
                if (newL.j == null) { newL.j = 0; }
                double i = (double)newL.i;
                double j = (double)newL.j;
                float radius = (float)Math.Sqrt(i * i + j * j);
                if (radius == 0)               // kleinster Wert > 0
                { radius = 0.0001f; }

                float x1 = (float)(oldL.actualPos.X + i - radius);
                float y1 = (float)(oldL.actualPos.Y + j - radius);

                centerList.Add(new coordByLine(newL.lineNumber, figureCount, new xyPoint(x1+ radius,y1+ radius),true));
   //             coordList.Add(new coordByLine(newL.lineNumber, figureCount, new xyPoint(x1 + radius, y1 + radius),false));

                float cos1 = (float)i / radius;
                if (cos1 > 1) cos1 = 1;
                if (cos1 < -1) cos1 = -1;
                float a1 = 180-180*(float)(Math.Acos(cos1)/Math.PI);

                if (j > 0) { a1 = -a1; }
                float cos2 = (float)(oldL.actualPos.X + i - newL.actualPos.X) / radius;
                if (cos2 > 1) cos2 = 1;
                if (cos2 < -1) cos2 = -1;
                float a2 = 180-180*(float)(Math.Acos(cos2)/Math.PI);

                if ((oldL.actualPos.Y + j- newL.actualPos.Y) > 0) { a2 = -a2; }
                float da = -(360 + a1 - a2);
                if (newL.motionMode == 3) { da=-(360 + a2 - a1); }
                if (da > 360) { da -= 360; }
                if (da < -360) { da += 360; }
                if (newL.motionMode == 2)
                {   path.AddArc(x1, y1, 2 * radius, 2 * radius, a1, da);
                    if (!newL.ismachineCoordG53)
                        xyzSize.setDimensionCircle(x1 + radius, y1 + radius, radius, a1, da);        // calculate new dimensions
                }
                else
                {   path.AddArc(x1, y1, 2 * radius, 2 * radius, a1, -da);
                    if (!newL.ismachineCoordG53)
                        xyzSize.setDimensionCircle(x1 + radius, y1 + radius, radius, a1, -da);       // calculate new dimensions
                }
            }
            if (path == pathPenDown)
                newL.figureNumber = figureCount;
            else
                newL.figureNumber = -1;

            return figureStart;
        }

        private void markPath(GraphicsPath path, float x, float y, int type)
        {   float markerSize = 1;
            if (!Properties.Settings.Default.importUnitmm)
            { markerSize /= 25.4F; }
            createMarker(path, x, y, markerSize, type, false);    // draw circle
        }

        // setup drawing area 
        public void calcDrawingArea()
        {
#if (debuginfo)
            log.Add("   GCodeVisu calcDrawinArea");
#endif
            double extend = 1.01;                                                       // extend dimension a little bit
            double roundTo = 5;                                                         // round-up dimensions
            if (!Properties.Settings.Default.importUnitmm)
            { roundTo = 0.25; }

            if ((xyzSize.miny == 0) && (xyzSize.maxy == 0))
            { xyzSize.miny = -1; xyzSize.maxy = 1; }

            drawingSize.minX = Math.Floor(xyzSize.minx* extend / roundTo)* roundTo;                  // extend dimensions
            if (drawingSize.minX >= 0) { drawingSize.minX = -roundTo; }                                          // be sure to show 0;0 position
            drawingSize.maxX = Math.Ceiling(xyzSize.maxx* extend / roundTo) * roundTo;
            drawingSize.minY = Math.Floor(xyzSize.miny* extend / roundTo) * roundTo;
            if (drawingSize.minY >= 0) { drawingSize.minY = -roundTo; }
            drawingSize.maxY = Math.Ceiling(xyzSize.maxy* extend / roundTo) * roundTo;
            createRuler(drawingSize.minX, drawingSize.maxX, drawingSize.minY, drawingSize.maxY);
            createMarkerPath();
        }

        public void createMarkerPath()
        { createMarkerPath(false, new xyPoint(0,0)); }

        public void createMarkerPath(bool showCenter, xyPoint center)
        { createMarkerPath(showCenter, center, center); }
        public void createMarkerPath(bool showCenter, xyPoint center, xyPoint last)
        {
#if (debuginfo)
            log.Add("   GCodeVisu createMarkerPath");
#endif
            float msize = (float) Math.Max(xyzSize.dimx, xyzSize.dimy) / 50;
            createMarker(pathTool,   (float)grbl.posWork.X,   (float)grbl.posWork.Y, msize, 2);
            createMarker(pathMarker, (float)grbl.posMarker.X, (float)grbl.posMarker.Y, msize, 3);
            if (showCenter)
            {   createMarker(pathMarker, (float)center.X, (float)center.Y, msize, 0,false);
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
        private void createRuler(double minX, double maxX, double minY, double maxY)
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
        private void createMarker(GraphicsPath path, float centerX,float centerY, float dimension,int style,bool rst=true)
        {   if (dimension == 0) { return; }
            if (rst)
                path.Reset();
            if (style == 0)   // horizontal cross
            {
                path.StartFigure(); path.AddLine(centerX , centerY + dimension, centerX , centerY - dimension);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY , centerX - dimension, centerY );
            }
            else if (style == 1)   // diagonal cross
            {
                path.StartFigure(); path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX - dimension, centerY - dimension, centerX + dimension, centerY + dimension);
            }
            else if (style == 2)            // box
            {
                path.StartFigure(); path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY + dimension);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX - dimension, centerY - dimension, centerX - dimension, centerY + dimension);
            }
            else if (style == 3)            // marker
            {
                path.StartFigure(); path.AddLine(centerX, centerY, centerX, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX, centerY - dimension, centerX + dimension, centerY);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY, centerX, centerY + dimension);
                path.StartFigure(); path.AddLine(centerX, centerY + dimension, centerX - dimension, centerY);
                path.StartFigure(); path.AddLine(centerX - dimension, centerY, centerX, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX, centerY - dimension, centerX, centerY);
            }
            else
            {
                path.StartFigure(); path.AddArc(centerX - dimension, centerY - dimension, 2 * dimension, 2 * dimension, 0, 360);
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
