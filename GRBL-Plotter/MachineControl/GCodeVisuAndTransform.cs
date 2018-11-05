/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

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
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;

namespace GRBL_Plotter
{
    public class GCodeVisuAndTransform
    {   public enum translate { None, ScaleX, ScaleY, Offset1, Offset2, Offset3, Offset4, Offset5, Offset6, Offset7, Offset8, Offset9, MirrorX, MirrorY };
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
        public static GraphicsPath path = pathPenUp;

        private xyzPoint toolPos = new xyzPoint(0,0,0);
        private xyzPoint machinePos = new xyzPoint(0, 0, 0);
        private xyPoint markerPos = new xyPoint(0, 0);
        private double zLimit = -0.001;
        private bool containsG2G3 = false;
        private bool containsG91 = false;

        public void setPosTool(xyzPoint tmp)
        { toolPos = tmp; }
        public void setPosMachine(xyzPoint tmp)
        { machinePos = tmp; }
        public void setPosMarker(xyPoint tmp)
        { markerPos = tmp; }
        public xyPoint GetPosMarker()
        { return markerPos; }
        public bool containsG2G3Command()
        { return containsG2G3; }
        public bool containsG91Command()
        { return containsG91; }

        public void drawMachineLimit(toolPos[] toolpos)
        {
            float offsetX = (float)machinePos.X- (float)toolPos.X;
            float offsetY = (float)machinePos.Y- (float)toolPos.Y;
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
            if ((toolpos != null) && (toolpos.Length >= 1))
            {
                Matrix matrix = new Matrix();
                matrix.Scale(1, -1);
                float wx, wy;
                foreach (toolPos tpos in toolpos)
                {
                    wx = tpos.X - offsetX; wy = tpos.Y - offsetY;
                    if ((tpos.name.Length > 1) && (tpos.toolnr >= 0))
                    {
                        pathToolTable.StartFigure();
                        pathToolTable.AddEllipse(wx - 4, wy - 4, 8, 8);
                        pathToolTable.Transform(matrix);
                        pathToolTable.AddString(tpos.toolnr.ToString() + ") " + tpos.name, new FontFamily("Arial"), (int)FontStyle.Regular, 4, new Point((int)wx - 12, -(int)wy + 4), StringFormat.GenericDefault);
                        pathToolTable.Transform(matrix);
                    }
                }
            }
        }
        private static float maxStep = 100;
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

        public string applyHeightMap(IList<string> oldCode, HeightMap Map)
        {
            maxStep = (float)Map.GridX;
            getGCodeLines(oldCode,true);                // read gcode and process subroutines
            IList<string> tmp=createGCodeProg(true,false).Split('\r').ToList();      // split lines and arcs
            getGCodeLines(tmp, false);                  // reload code
            return createGCodeProg(false, true, Map);        // apply new Z-value;
        }

        public static void clearHeightMap()
        {
            pathHeightMap.Reset();
        }

        // analyse each GCode line and track actual position and modes for each code line
        private List<gcodeLine> gcodeList;          // keep original program
        private List<coordinateLine> coordList;     // get all coordinmates (also subroutines)
        /// <summary>
        /// Hold absolute coordinate for given linenumber
        /// </summary>
        private class coordinateLine
        {
            public int lineNumber;          // line number in fCTBCode
            public xyzPoint actualPos;      // accumulates position
            public double distance;         // distance to specific point
            public coordinateLine(int line, xyzPoint p)
            { lineNumber = line; actualPos = p; distance = -1; }
        }
        // gcode line structure
        private class gcodeLine
        {
            public int lineNumber;          // line number in fCTBCode
            public string codeLine;         // copy of original gcode line
            public byte motionMode;         // G0,1,2,3
            public bool isdistanceModeG90;  // G90,91
            public bool ismachineCoordG53;  // don't apply transform to machine coordinates
            public bool isSubroutine;
            public byte spindleState;       // M3,4,5
            public byte coolantState;       // M7,8,9
            public int spindleSpeed;        // actual spindle spped
            public int feedRate;            // actual feed rate
            public double? x, y, z, a, b, c, u, v, w, i, j; // current parameters
            public xyzPoint actualPos;      // accumulates position
            public double distance;         // distance to specific point
            public gcodeLine()
            { resetAll(); }
            public gcodeLine(gcodeLine tmp)
            {   resetAll(); lineNumber = tmp.lineNumber; codeLine = tmp.codeLine; motionMode = tmp.motionMode;
                isdistanceModeG90 = tmp.isdistanceModeG90; ismachineCoordG53 = tmp.ismachineCoordG53;
                isSubroutine = tmp.isSubroutine; spindleState = tmp.spindleState; coolantState = tmp.coolantState;
                spindleSpeed = tmp.spindleSpeed;feedRate = tmp.feedRate;
                x = tmp.x;y = tmp.y;z = tmp.z; i = tmp.i; j = tmp.j; a = tmp.a; b = tmp.b; c = tmp.c; u = tmp.u; v = tmp.v; w = tmp.w;
                actualPos = tmp.actualPos;distance = tmp.distance;
            }
            /// <summary>
            /// Reset coordinates and set G90, M5, M9
            /// </summary>
            public void resetAll()
            {   motionMode = 0; isdistanceModeG90 = true; ismachineCoordG53 = false; isSubroutine = false; actualPos.X = 0; actualPos.Y = 0; actualPos.Z = 0; distance = -1;
                //                motionMode2 = 0; codeM = 0; codeP = 0;
                spindleState = 5; coolantState = 9; 
                resetCoordinates(); }
            /// <summary>
            /// Reset coordinates
            /// </summary>
            public void resetCoordinates()
            {   x = null; y = null; z = null; a = null; b = null; c = null; u = null; v = null; w = null; i = null; j=null; ismachineCoordG53 = false; isSubroutine = false; }
        };
        private gcodeLine oldLine = new gcodeLine();
        private gcodeLine newLine = new gcodeLine();
        // analyze GCode and fill up gcode line strucutres
        private int actualM;
        private int actualP;
        private int actualO;
        private int actualL;

        /// <summary>
        /// Entrypoint for generating drawing path from given gcode
        /// </summary>
        public void getGCodeLines(IList<string> oldCode, bool processSubs=false)
        {
            string[] GCode = oldCode.ToArray<string>();
            string singleLine;
            gcodeList = new List<gcodeLine>();    //.Clear();
            coordList = new List<coordinateLine>();    //.Clear();
            oldLine.resetAll();                     // reset coordinates and parser modes
            oldLine.actualPos = toolPos;
            clearDrawingnPath();                    // reset path, dimensions
            newLine.resetAll();                     // reset coordinates and parser modes
            containsG2G3 = false;
            containsG91 = false;
            bool programEnd = false;
            lastSubroutine[0] = -1; lastSubroutine[1] = 0; lastSubroutine[2] = 0;

            actualM = 0; actualP = 0; actualO = 0; actualL = 1;
            for (int index = 0; index < GCode.Length; index++)  // go through all gcode lines
            {
                newLine.resetCoordinates();
                newLine.lineNumber = index;
                actualM = 0; actualO = 0; actualP = 0; actualL = 1;
                singleLine = GCode[index].ToUpper().Trim(); // get line

                getGCodeLine(singleLine, newLine);          // parse line, fill up newLine.xyz and actualM,P,O
                calcAbsPosition(newLine, oldLine);          // calc abs position
                if ((actualM == 98) && processSubs)
                    newLine.codeLine = "(" + GCode[index] + ")";
                else
                    newLine.codeLine = GCode[index];// + String.Format(" X:{0} Y:{1} Z:{2} ", newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);
                gcodeList.Add(new gcodeLine(newLine));      // add parsed line to list
                coordList.Add(new coordinateLine(index, newLine.actualPos));
                if (!programEnd)
                {   // add data to drawing path
                    createDrawingPathFromGCode(newLine, oldLine);
                    oldLine = new gcodeLine(newLine);   // get copy of newLine      
                }
                if ((actualM == 30) || (actualM == 2)) { programEnd = true; }
                if (actualM == 98)
                {   if (lastSubroutine[0] == actualP)
                        addSubroutine(GCode, lastSubroutine[1], lastSubroutine[2], actualL, processSubs);
                     else
                        findAddSubroutine(actualP, GCode, actualL, processSubs);      // scan complete GCode for matching O-word
                }
            }
        }
        /// <summary>
        /// Find and add subroutine within given gcode
        /// </summary>
        private string findAddSubroutine(int foundP, string[] GCode, int repeat, bool processSubs)
        {
            string singleLine;
            int subStart=0, subEnd=0;
            bool foundO = false;
            actualO = 0; actualP = 0;
            gcodeLine tmpLine = new gcodeLine(); 
            for (int index = 0; index < GCode.Length; index++)
            {
                actualM = 0;
                singleLine = GCode[index].ToUpper().Trim(); // get line

                newLine.lineNumber = index;
                getGCodeLine(singleLine, tmpLine);       // parse line, don't fill up newLine.xyz but actualM,P,O
                if (actualO == foundP)
                {   if (!foundO)
                    {   subStart = index;       //MessageBox.Show("Find O " + index.ToString());
                        foundO = true;
                    }
                    else
                    {   if (actualM == 99)
                        {   subEnd = index;     //MessageBox.Show("Find M " + index.ToString());
                            break;
                        }
                    }
                }
            }
            if ((subStart > 0) && (subEnd > subStart))
            {
                addSubroutine(GCode, subStart, subEnd, repeat, processSubs);
                lastSubroutine[0] = foundP;
                lastSubroutine[1] = subStart;
                lastSubroutine[2] = subEnd;
            }
            return String.Format("Start:{0} EndX:{1} ", subStart, subEnd);
        }
        private int[] lastSubroutine = new int[] { 0, 0, 0 };

        private void addSubroutine(string[] GCode, int start, int stop, int repeat, bool processSubs)
        {
            string singleLine;
            bool showPath = true;
            newLine.resetCoordinates();                         // reset coordinates and parser mode
            for (int loop = 0; loop < repeat; loop++)
            {
                for (int index = start + 1; index < stop; index++)   // go through real line numbers and parse sub-code
                {
                    if (GCode[index].IndexOf("%START_HIDECODE") >= 0) { showPath = false; }
                    if (GCode[index].IndexOf("%STOP_HIDECODE") >= 0) { showPath = true; }
                    newLine.resetCoordinates();
                    newLine.lineNumber = index;
                    singleLine = GCode[index].ToUpper().Trim(); // get line
                    newLine.isSubroutine = !processSubs;
                    getGCodeLine(singleLine, newLine);          // parse line, fill up newLine.xyz and actualM,P,O
                    calcAbsPosition(newLine, oldLine);          // calc abs position
                    newLine.codeLine = GCode[index];// + String.Format(" X:{0} Y:{1} Z:{2} ", newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);

                    if (!showPath) newLine.ismachineCoordG53 = true;

                    if (processSubs)
                        gcodeList.Add(new gcodeLine(newLine));      // add parsed line to list
                    if (!newLine.ismachineCoordG53)
                    { coordList.Add(new coordinateLine(index, newLine.actualPos));
                        if (((newLine.motionMode > 0) || (newLine.z != null)) && !((newLine.x == toolPos.X) && (newLine.y == toolPos.Y)))
                            xyzSize.setDimensionXYZ(newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);             // calculate max dimensions
                    }                                                                                                       // add data to drawing path
                    if (showPath)
                        createDrawingPathFromGCode(newLine, oldLine);
                    oldLine = new gcodeLine(newLine);   // get copy of newLine                         
                }
            }
        }


        /// <summary>
        /// Calc. absolute positions and set object dimension: xyzSize.setDimension
        /// </summary>
        private void calcAbsPosition(gcodeLine newLine, gcodeLine oldLine)
        {
            if (!newLine.ismachineCoordG53)         // only use world coordinates
            {   if (newLine.x != null)
                {   if (newLine.isdistanceModeG90)  // absolute move
                    {   newLine.actualPos.X = (double)newLine.x;
                        if (newLine.actualPos.X != toolPos.X)            // don't add actual tool pos
                        {   xyzSize.setDimensionX(newLine.actualPos.X);
                        }
                    }
                    else
                    {   newLine.actualPos.X = oldLine.actualPos.X + (double)newLine.x;
                        if (newLine.actualPos.X != toolPos.X)            // don't add actual tool pos
                        {   xyzSize.setDimensionX(newLine.actualPos.X);// - toolPosX);
                        }
                    }
                }
                else
                    newLine.actualPos.X = oldLine.actualPos.X;

                if (newLine.y != null)
                {   if (newLine.isdistanceModeG90)
                    {   newLine.actualPos.Y = (double)newLine.y;
                        if (newLine.actualPos.Y != toolPos.Y)            // don't add actual tool pos
                        {   xyzSize.setDimensionY(newLine.actualPos.Y);
                        }
                    }
                    else
                    {   newLine.actualPos.Y = oldLine.actualPos.Y + (double)newLine.y;
                        if (newLine.actualPos.Y != toolPos.Y)            // don't add actual tool pos
                        {   xyzSize.setDimensionY(newLine.actualPos.Y);// - toolPosY);
                        }
                    }
                }
                else
                    newLine.actualPos.Y = oldLine.actualPos.Y;

                if (newLine.z != null)
                {   if (newLine.isdistanceModeG90)
                    {   newLine.actualPos.Z = (double)newLine.z;
                        if (newLine.actualPos.Z != toolPos.Z)            // don't add actual tool pos
                            xyzSize.setDimensionZ(newLine.actualPos.Z); // removed - toolPosZ
                    }
                    else
                    {   newLine.actualPos.Z = oldLine.actualPos.Z + (double)newLine.z;
                        if (newLine.actualPos.Z != toolPos.Z)            // don't add actual tool pos
                            xyzSize.setDimensionZ(newLine.actualPos.Z);// - toolPosZ);
                    }
                }
                else
                    newLine.actualPos.Z = oldLine.actualPos.Z;
            }
        }
        /// <summary>
        /// parse single gcode line
        /// </summary>
        private void getGCodeLine(string line, gcodeLine parseLine)
        {
            char cmd = '\0';
            string num = "";
            bool comment = false;
            double value = 0;

            if (! (line.StartsWith("$")|| line.StartsWith("("))) //do not parse grbl commands
            {
                try
                {
                    foreach (char c in line)
                    {
                        if (c == ';')
                            break;
                        if (c == '(')
                            comment = true;
                        if (!comment)
                        {
                            if (Char.IsLetter(c))
                            {
                                if (cmd != '\0')
                                {
                                    value = 0;
                                    if (num.Length > 0)
                                    {
                                        try { value = double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo); }
                                        catch { }
                                    }
                                    try { parseGCodeToken(cmd, value, parseLine); }
                                    catch { }
                                }
                                cmd = c;
                                num = "";
                            }
                            else if (Char.IsNumber(c) || c == '.' || c == '-')
                            {
                                num += c;
                            }
                        }
                        if (c == ')')
                            comment = false;
                    }
                    if (cmd != '\0')
                    {
                        try { parseGCodeToken(cmd, double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo), parseLine); }
                        catch { }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// fill current gcode line structure
        /// </summary>
        private void parseGCodeToken(char cmd, double value, gcodeLine parseLine)
        {   switch (Char.ToUpper(cmd))
            {   case 'X':
                    parseLine.x = value;
                    break;
                case 'Y':
                    parseLine.y = value;
                    break;
                case 'Z':
                    parseLine.z = value;
                    break;
                case 'A':
                    parseLine.a = value;
                    break;
                case 'B':
                    parseLine.b = value;
                    break;
                case 'C':
                    parseLine.c = value;
                    break;
                case 'U':
                    parseLine.u = value;
                    break;
                case 'V':
                    parseLine.v = value;
                    break;
                case 'W':
                    parseLine.w = value;
                    break;
                case 'I':
                    parseLine.i = value;
                    break;
                case 'J':
                    parseLine.j = value;
                    break;
                case 'F':
                    parseLine.feedRate = (int)value;
                    break;
                case 'S':
                    parseLine.spindleSpeed = (int)value;
                    break;
                case 'G':
                    if (value <= 3)
                    {   parseLine.motionMode = (byte)value;
                        if (value >=2)
                            containsG2G3 = true;
                    }
                    if (value == 53)
                    { parseLine.ismachineCoordG53 = true; }

                    if (value == 90)
                    { parseLine.isdistanceModeG90 = true; }

                    if (value == 91)
                    {   parseLine.isdistanceModeG90 = false;        
                        containsG91 = true;  }
                    break;
                case 'M':
                    if (value >= 3 && value <= 5)
                    { parseLine.spindleState = (byte)value; }
                    if (value >= 7 && value <= 9)
                    { parseLine.coolantState = (byte)value; }
                    actualM = (int)value;
                    break;
                case 'P':
                    actualP = (int)value;
                    break;
                case 'O':
                    actualO = (int)value;
                    break;
                case 'L':
                    actualL = (int)value;
                    break;
            }
        }

        /// <summary>
        /// set marker into drawing on xy-position of desired line
        /// </summary>
        public void setPosMarkerLine(int line)
        {   try
            {
                if (line < coordList.Count)
                {
                    if (line == coordList[line].lineNumber)
                    {
                        setPosMarker((xyPoint)coordList[line].actualPos);//.X, coordList[line].actualPos.Y);
                        createMarkerPath();
                    }
                    else
                    {
                        foreach (coordinateLine gcline in coordList)
                        {
                            if (line == gcline.lineNumber)
                            {
                                setPosMarker((xyPoint)gcline.actualPos);//.X, gcline.actualPos.Y);
                                createMarkerPath();
                                break;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// find gcode line with xy-coordinates near by given coordinates
        /// </summary>
        public int setPosMarkerNearBy(double x, double y)
        {   foreach (coordinateLine gcline in coordList)
            {
                gcline.distance = Math.Sqrt((x - gcline.actualPos.X) * (x - gcline.actualPos.X) + (y - gcline.actualPos.Y) * (y - gcline.actualPos.Y));
            }
            int line = 0;
            List<coordinateLine> SortedList = coordList.OrderBy(o => o.distance).ToList();
            setPosMarker((xyPoint)SortedList[line].actualPos);//.X, SortedList[line].actualPos.Y);
            createMarkerPath();
            return SortedList[line].lineNumber;
        }

        public string getLineInfo(int line)
        {
            if (line == gcodeList[line].lineNumber)
            {   return gcodeList[line].codeLine; }
            else
            {
                foreach (gcodeLine gcline in gcodeList)
                {
                    if (line == gcline.lineNumber)
                    {
                        return gcline.codeLine;
                    }
                }
            }
            return "not found";
        }

        public string transformGCodeMirror(translate shiftToZero = translate.MirrorX)
        {
            double oldmaxx = xyzSize.maxx;
            double oldmaxy = xyzSize.maxy;
            oldLine.resetAll();         // reset coordinates and parser modes
            oldLine.actualPos = toolPos;
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeLine gcline in gcodeList)
            {
                if (!gcline.ismachineCoordG53)
                {
                    // switch circle direction
                    if ((shiftToZero == translate.MirrorX) || (shiftToZero == translate.MirrorY))           // mirror xy 
                    {
                        if (gcline.motionMode == 2) { gcline.motionMode = 3; }
                        else if (gcline.motionMode == 3) { gcline.motionMode = 2; }
                    }
                    if (shiftToZero == translate.MirrorX)           // mirror x
                    {
                        if (gcline.x != null)
                        {
                            if (gcline.isdistanceModeG90)
                                gcline.x = oldmaxx - gcline.x;
                            else
                                gcline.x = -gcline.x;
                        }
                        gcline.i = -gcline.i;
                    }
                    if (shiftToZero == translate.MirrorY)           // mirror y
                    {
                        if (gcline.y != null)
                        {
                            if (gcline.isdistanceModeG90)
                                gcline.y = oldmaxy - gcline.y;
                            else
                                gcline.y = -gcline.y;
                        }
                        gcline.j = -gcline.j;
                    }

                    calcAbsPosition(gcline, oldLine);
                    oldLine = new gcodeLine(gcline);   // get copy of newLine
                }
            }
            return createGCodeProg(false,false);
        }
        /// <summary>
        /// rotate and scale arround offset
        /// </summary>
        public string transformGCodeRotate(double angle, double scale, xyPoint offset)
        {
            double? newvalx, newvaly, newvali, newvalj;
            oldLine.resetAll();         // reset coordinates and parser modes
            oldLine.actualPos = toolPos;
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeLine gcline in gcodeList)
            {
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
                    oldLine = new gcodeLine(gcline);   // get copy of newLine
                }
            }
            return createGCodeProg(false,false);
        }
        /// <summary>
        /// scale x and y seperatly in %
        /// </summary>
        public string transformGCodeScale(double scaleX, double scaleY)
        {
            double factor_x = scaleX / 100;
            double factor_y = scaleY / 100;
            oldLine.resetAll();         // reset coordinates and parser modes
            oldLine.actualPos = toolPos;
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeLine gcline in gcodeList)
            {
                if (!gcline.ismachineCoordG53)
                {
                    if (gcline.x != null)
                        gcline.x = gcline.x * factor_x;
                    if (gcline.y != null)
                        gcline.y = gcline.y * factor_y;
                    if (gcline.i != null)
                        gcline.i = gcline.i * factor_x;
                    if (gcline.j != null)
                        gcline.j = gcline.j * factor_y;

                    calcAbsPosition(gcline, oldLine);
                    oldLine = new gcodeLine(gcline);   // get copy of newLine
                }
            }
            return createGCodeProg(false, false);
        }
        public string transformGCodeOffset(double x, double y, translate shiftToZero)
        {
            double offsetX = 0;
            double offsetY = 0;
            bool offsetApplied = false;
            bool noInsertNeeded = false;
            oldLine.resetAll();         // reset coordinates and parser modes
            oldLine.actualPos = toolPos;
            if (shiftToZero == translate.Offset1) { offsetX = x + xyzSize.minx;                     offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset2) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2;  offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset3) { offsetX = x + xyzSize.minx + xyzSize.dimx;      offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset4) { offsetX = x + xyzSize.minx;                     offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset5) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2;  offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset6) { offsetX = x + xyzSize.minx + xyzSize.dimx;      offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset7) { offsetX = x + xyzSize.minx;                     offsetY = y + xyzSize.miny; }
            if (shiftToZero == translate.Offset8) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2;  offsetY = y + xyzSize.miny; }
            if (shiftToZero == translate.Offset9) { offsetX = x + xyzSize.minx + xyzSize.dimx;      offsetY = y + xyzSize.miny; }

            if (containsG91)    // relative move: insert rapid movement before pen down, to be able applying offset
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
                            {   getGCodeLine("G0 X0 Y0 (Insert offset movement)", newLine);                   // parse line, fill up newLine.xyz and actualM,P,O
                                gcodeList.Insert(i + 1, newLine);
                            }
                        }
                    }
                }
            }
            bool hide_code = false; ;
            foreach (gcodeLine gcline in gcodeList)
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
                    oldLine = new gcodeLine(gcline);   // get copy of newLine
                }
            }
            return createGCodeProg(false, false);
        }

        public string replaceG23()
        {   return createGCodeProg(true,false); }

        /// <summary>
        /// Generate GCode from given coordinates in GCodeList
        /// only replace lines with coordinate information
        /// </summary>
        private string createGCodeProg(bool replaceG23, bool applyNewZ, HeightMap Map=null)
        {
            StringBuilder newCode = new StringBuilder();
            StringBuilder tmpCode = new StringBuilder();
            bool getCoordinateXY, getCoordinateZ;
            double feedRate = 0;
            double spindleSpeed=0;
            double lastActualX = 0, lastActualY = 0,i,j;
            double newZ = 0;
            int lastMotionMode = 0;
            xyzSize.resetDimension();
            bool hide_code = false;
            for (int iCode=0; iCode < gcodeList.Count; iCode++)
            {   gcodeLine gcline = gcodeList[iCode];
                tmpCode.Clear();
                getCoordinateXY = false;
                getCoordinateZ = false;
                if (gcline.codeLine.Length == 0)
                    continue;

                if (gcline.codeLine.IndexOf("%START_HIDECODE") >= 0) { hide_code = true; }
                if (gcline.codeLine.IndexOf("%STOP_HIDECODE") >= 0) { hide_code = false; }

                if ((!hide_code) && (replaceG23))     // replace circles
                {
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
                    {   if ((gcline.x != null) || (gcline.y != null))
                            gcode.splitLine(newCode, gcline.motionMode, (float)lastActualX, (float)lastActualY, (float)gcline.actualPos.X, (float)gcline.actualPos.Y, maxStep, true, gcline.codeLine);
                        else
                        { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }
                    }
                    else
                    { newCode.AppendLine(gcline.codeLine.Trim('\r', '\n')); }

                }
                else
                {
                    if (gcline.x != null)
                    { tmpCode.AppendFormat(" X{0}", gcode.frmtNum((double)gcline.x)); getCoordinateXY = true; }
                    if (gcline.y != null)
                    { tmpCode.AppendFormat(" Y{0}", gcode.frmtNum((double)gcline.y)); getCoordinateXY = true; }
                    if (getCoordinateXY && applyNewZ && (Map != null))
                    {   newZ = Map.InterpolateZ(gcline.actualPos.X, gcline.actualPos.Y);
                        gcline.z = gcline.actualPos.Z + newZ;
                    }

                    if (gcline.z != null)
                    { tmpCode.AppendFormat(" Z{0}", gcode.frmtNum((double)gcline.z)); getCoordinateZ = true; }
                    if (gcline.i != null)
                    { tmpCode.AppendFormat(" I{0}", gcode.frmtNum((double)gcline.i)); getCoordinateXY = true; }
                    if (gcline.j != null)
                    { tmpCode.AppendFormat(" J{0}", gcode.frmtNum((double)gcline.j)); getCoordinateXY = true; }
                    if ((getCoordinateXY || getCoordinateZ) && (!gcline.ismachineCoordG53) && (!hide_code))
                    {   if ((gcline.motionMode > 0) && (feedRate != gcline.feedRate) && ((getCoordinateXY && !getCoordinateZ) || (!getCoordinateXY && getCoordinateZ)))
                        { tmpCode.AppendFormat(" F{0,0}", gcline.feedRate); }
                        if (spindleSpeed != gcline.spindleSpeed)
                        { tmpCode.AppendFormat(" S{0,0}", gcline.spindleSpeed); }
                        tmpCode.Replace(',', '.');
                        if (gcline.codeLine.IndexOf("(Setup - GCode") > 1)  // ignore coordinates from setup footer
                            newCode.AppendLine(gcline.codeLine);
                        else
                            newCode.AppendLine("G" + gcode.frmtCode(gcline.motionMode) + tmpCode.ToString());
                    }
                    else
                    {   newCode.AppendLine(gcline.codeLine.Trim('\r','\n'));
                    }
                    lastMotionMode = gcline.motionMode;
                }
                feedRate = gcline.feedRate;
                spindleSpeed = gcline.spindleSpeed;
                lastActualX = gcline.actualPos.X; lastActualY = gcline.actualPos.Y;

                if ((!hide_code) && (!gcline.ismachineCoordG53) && (gcline.codeLine.IndexOf("(Setup - GCode") < 1)) // ignore coordinates from setup footer
                {
                    if (!((gcline.actualPos.X == toolPos.X) && (gcline.actualPos.Y == toolPos.Y)))            // don't add actual tool pos
                        xyzSize.setDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);
                }
                coordList[iCode] = new coordinateLine(iCode, gcline.actualPos);
            }
            return newCode.ToString().Replace(',','.');
        }

        public GCodeVisuAndTransform()
        {   xyzSize.resetDimension();
        }
        private void clearDrawingnPath()
        {   xyzSize.resetDimension();
            pathPenUp.Reset();
            pathPenDown.Reset();
            pathRuler.Reset();
            pathTool.Reset();
            pathMarker.Reset();
            path = pathPenUp;
            containsG2G3 = false;
            containsG91 = false;
        }

        // add given coordinates to drawing path
        private int onlyZ = 0;
        private byte oldSpindleState = 5;
        /// <summary>
        /// add segement to drawing path 'PenUp' or 'PenDown' from old-xyz to new-xyz
        /// </summary>
        private void createDrawingPathFromGCode(gcodeLine newL, gcodeLine oldL)
        {
            bool passLimit = false;
            var pathOld = path;

            if (newL.isSubroutine && (!oldL.isSubroutine))
                markPath(pathPenUp, (float)newL.actualPos.X, (float)newL.actualPos.Y, 2); // 2=rectangle

            if (!newL.ismachineCoordG53)    
            {
                pathOld = path;
                // select path to draw depending on Z-Value
                if ((oldSpindleState == 5) && ((newL.spindleState == 3) || (newL.spindleState == 4)))   // Laser on
                { path = pathPenDown; }
                if ((newL.spindleState == 5) && ((oldSpindleState == 3) || (oldSpindleState == 4)))     // laser off
                { path = pathPenUp;  }
                oldSpindleState = newL.spindleState;

                if ((newL.actualPos.Z != oldL.actualPos.Z) && (newL.actualPos.Z < zLimit))            // tool down
                { path = pathPenDown; }
                else if ((newL.actualPos.Z < oldL.actualPos.Z) && (newL.actualPos.Z >= zLimit))       // overwrite Laser on
                {   path = pathPenUp; }
                else if ((newL.actualPos.Z > oldL.actualPos.Z) && (newL.actualPos.Z >= zLimit))       // tool up
                {   path = pathPenUp; }
                if (newL.actualPos.Z == (float)Properties.Settings.Default.importGCZUp)
                { path = pathPenUp; }

                if ((path != pathOld))// && !((ox == toolPosX) && (oy == toolPosY) ))
                {   passLimit = true; }

                path.StartFigure();                 // Start Figure but never close to avoid connecting first and last point
                if (newL.motionMode == 0 || newL.motionMode == 1)
                {
                    if ((newL.actualPos.X != oldL.actualPos.X) || (newL.actualPos.Y != oldL.actualPos.Y) || (oldL.motionMode == 2 || oldL.motionMode == 3))
                    {
                        path.AddLine((float)oldL.actualPos.X, (float)oldL.actualPos.Y, (float)newL.actualPos.X, (float)newL.actualPos.Y);
                        onlyZ = 0;  // x or y has changed
                    }
                    else
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
                { radius = 0.0000001f; }

                float x1 = (float)(oldL.actualPos.X + i - radius);
                float y1 = (float)(oldL.actualPos.Y + j - radius);

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
        }

        private void markPath(GraphicsPath path, float x, float y, int type)
        {   float markerSize = 1;
            if (!Properties.Settings.Default.importUnitmm)
            { markerSize /= 25.4F; }
            createMarker(path, x, y, markerSize, type, false);    // draw circle
        }

        // setup drawing area 
        public void createImagePath()
        {   double extend = 1.01;                                                       // extend dimension a little bit
            double roundTo = 5;                                                         // round-up dimensions
            if (!Properties.Settings.Default.importUnitmm)
            { roundTo = 0.25; }
            drawingSize.minX = Math.Floor(xyzSize.minx* extend / roundTo)* roundTo;                  // extend dimensions
            if (drawingSize.minX >= 0) { drawingSize.minX = -roundTo; }                                          // be sure to show 0;0 position
            drawingSize.maxX = Math.Ceiling(xyzSize.maxx* extend / roundTo) * roundTo;
            drawingSize.minY = Math.Floor(xyzSize.miny* extend / roundTo) * roundTo;
            if (drawingSize.minY >= 0) { drawingSize.minY = -roundTo; }
            drawingSize.maxY = Math.Ceiling(xyzSize.maxy* extend / roundTo) * roundTo;
            double xRange = (drawingSize.maxX - drawingSize.minX);                                              // calculate new size
            double yRange = (drawingSize.maxY - drawingSize.minY);
            createRuler(drawingSize.maxX, drawingSize.maxY);
            createMarkerPath();
        }

        public void createMarkerPath()
        {
            float msize = (float) Math.Max(xyzSize.dimx, xyzSize.dimy) / 50;
            createMarker(pathTool, (float)toolPos.X, (float)toolPos.Y, msize, 2);
            createMarker(pathMarker, (float)markerPos.X, (float)markerPos.Y, msize, 3);
        }
        private void createRuler(double maxX, double maxY)
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
                maxX = maxX * divider; // unit;
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
        }
        private void createMarker(GraphicsPath path, float centerX,float centerY, float dimension,int style,bool rst=true)
        {   if (dimension == 0) { return; }
            if (rst)
                path.Reset();
            if (style == 0)   // horizontal cross
            {
                path.StartFigure(); path.AddLine(centerX , centerY + dimension, centerX , centerY - dimension);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY , centerX - dimension, centerY );
        //        path.CloseFigure();
            }
            else if (style == 1)   // diagonal cross
            {
                path.StartFigure(); path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX - dimension, centerY - dimension, centerX + dimension, centerY + dimension);
        //        path.CloseFigure();
            }
            else if (style == 2)            // box
            {
                path.StartFigure(); path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY + dimension);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY - dimension);
                path.StartFigure(); path.AddLine(centerX - dimension, centerY - dimension, centerX - dimension, centerY + dimension);
         //       path.CloseFigure();
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
          //      path.CloseFigure();
            }
        }
    }


    public struct drawingProperties
    {
        public double minX,minY,maxX,maxY;
        public void drawingProperty()
         { minX = 0;minY = 0;maxX = 0;maxY=0; }
    };


    /// <summary>
    /// calculate overall dimensions of drawing
    /// </summary>
    public class Dimensions
    {
        public double minx, maxx, miny, maxy, minz, maxz;
        public double dimx, dimy, dimz;

        public Dimensions()
        { resetDimension(); }
        public void setDimensionXYZ(double? x, double? y, double? z)
        {   if (x != null) { setDimensionX((double)x); }
            if (y != null) { setDimensionY((double)y); }
            if (z != null) { setDimensionZ((double)z); }
        }
        public void setDimensionXY(double? x, double? y)
        {   if (x != null) { setDimensionX((double)x); }
            if (y != null) { setDimensionY((double)y); }
        }
        public void setDimensionX(double value)
        {
            minx = Math.Min(minx, value);
            maxx = Math.Max(maxx, value);
            dimx = maxx - minx;
        }
        public void setDimensionY(double value)
        {
            miny = Math.Min(miny, value);
            maxy = Math.Max(maxy, value);
            dimy = maxy - miny;
        }
        public void setDimensionZ(double value)
        {
            minz = Math.Min(minz, value);
            maxz = Math.Max(maxz, value);
            dimz = maxz - minz;
        }
        // calculate min/max dimensions of a circle
        public void setDimensionCircle(double x, double y, double radius, double start, double delta)
        {
            double end = start + delta;
            if (delta > 0)
            {
                for (double i = start; i < end; i += 5)
                {   setDimensionX(x+radius*Math.Cos(i/180*Math.PI));
                    setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                }
            }
            else
            {
                for (double i = start; i > end; i -= 5)
                {
                    setDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                    setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                }
            }

        }
        public void resetDimension()
        {
            minx = Double.MaxValue;
            miny = Double.MaxValue;
            minz = Double.MaxValue;
            maxx = Double.MinValue;
            maxy = Double.MinValue;
            maxz = Double.MinValue;
            dimx = 0;
            dimy = 0;
            dimz = 0;
        }
        // return string with dimensions
        public String getMinMaxString()
        {   
            string x = String.Format("X:{0,8:####0.00} |{1,8:####0.00}\r\n", minx, maxx);
            string y = String.Format("Y:{0,8:####0.00} |{1,8:####0.00}\r\n", miny, maxy);
            string z = String.Format("Z:{0,8:####0.00} |{1,8:####0.00}", minz, maxz);
            if ((minx == Double.MaxValue) || (maxx == Double.MinValue))
                x = "X: unknown | unknown\r\n";
            if ((miny == Double.MaxValue) || (maxy == Double.MinValue))
                y = "Y: unknown | unknown\r\n";
            if ((minz == Double.MaxValue) || (maxz == Double.MinValue))
                z = "Z: unknown | unknown";
            return  "    Min.   | Max.\r\n" + x + y + z;
//            return String.Format("    Min.   | Max.\r\nX:{0,8:####0.00} |{1,8:####0.00}\r\nY:{2,8:####0.00} |{3,8:####0.00}\r\nZ:{4,8:####0.00} |{5,8:####0.00}", minx, maxx, miny, maxy, minz,maxz);
        }
        public bool withinLimits(xyzPoint actualMachine, xyzPoint actualWorld)
        {
            return (withinLimits(actualMachine, minx - actualWorld.X, miny - actualWorld.Y) && withinLimits(actualMachine, maxx - actualWorld.X, maxy - actualWorld.Y));
        }
        public bool withinLimits(xyzPoint actualMachine, double tstx, double tsty)
        {
            double minlx = (double)Properties.Settings.Default.machineLimitsHomeX;
            double maxlx = minlx + (double)Properties.Settings.Default.machineLimitsRangeX;
            double minly = (double)Properties.Settings.Default.machineLimitsHomeY;
            double maxly = minly + (double)Properties.Settings.Default.machineLimitsRangeY;
            tstx += actualMachine.X;
            tsty += actualMachine.Y;
            if ((tstx < minlx) || (tstx > maxlx))
                return false;
            if ((tsty < minly) || (tsty > maxly))
                return false;
            return true;
        }
    }
}
