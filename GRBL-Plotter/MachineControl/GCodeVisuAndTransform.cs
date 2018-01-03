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
 * 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
        public static GraphicsPath path = pathPenUp;

        private double toolPosX = 0;
        private double toolPosY = 0;
        private double toolPosZ = 0;
        private double markerPosX = 0;
        private double markerPosY = 0;
        private double zeroOffsetX = 0;
        private double zeroOffsetY = 0;
        private int picWidth = 640;
        private int picHeight = 480;
        private double picScaling = 1;
        private double zLimit = 0.0;
        private bool containsG2G3 = false;
        private bool containsG91 = false;

        public void setPosTool(double x, double y, double z)
        { toolPosX = x; toolPosY = y; toolPosZ = z; }
        public void setPosMarker(double x, double y)
        { markerPosX = x; markerPosY = y; }
        public void setPosMarkerX(double x)
        { markerPosX = x; }
        public double GetPosMarkerX()
        { return markerPosX; }
        public void setPosMarkerY(double y)
        { markerPosY = y; }
        public double GetPosMarkerY()
        { return markerPosY; }
        public bool containsG2G3Command()
        { return containsG2G3; }
        public bool containsG91Command()
        { return containsG91; }

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
            {   resetAll(); lineNumber = tmp.lineNumber; codeLine = tmp.codeLine; motionMode = tmp.motionMode; isdistanceModeG90 = tmp.isdistanceModeG90;
                //                motionMode2 = tmp.motionMode2; codeM = tmp.codeM; codeP = tmp.codeP;
                spindleState = tmp.spindleState; coolantState = tmp.coolantState;spindleSpeed = tmp.spindleSpeed;feedRate = tmp.feedRate;
                x = tmp.x;y = tmp.y;z = tmp.z; i = tmp.i; j = tmp.j;
                actualPos = tmp.actualPos;distance = tmp.distance;
            }
            /// <summary>
            /// Reset coordinates and set G90, M5, M9
            /// </summary>
            public void resetAll()
            {   motionMode = 0; isdistanceModeG90 = true; actualPos.X = 0; actualPos.Y = 0; actualPos.Z = 0; distance = -1;
                //                motionMode2 = 0; codeM = 0; codeP = 0;
                spindleState = 5; coolantState = 9; 
                resetCoordinates(); }
            /// <summary>
            /// Reset coordinates
            /// </summary>
            public void resetCoordinates()
            {   x = null; y = null; z = null; a = null; b = null; c = null; u = null; v = null; w = null; i = null; j=null ;            }
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
            oldLine.actualPos.X = toolPosX;
            oldLine.actualPos.Y = toolPosY;
            oldLine.actualPos.Z = toolPosZ;
            clearDrawingnPath();                    // reset path, dimensions
            newLine.resetAll();                     // reset coordinates and parser modes
            containsG2G3 = false;
            containsG91 = false;
            bool programEnd = false;
            actualM = 0; actualP = 0; actualO = 0; actualL = 1;
            for (int index = 0; index < GCode.Length; index++)
            {
                newLine.resetCoordinates();
                newLine.lineNumber = index;
                actualM = 0; actualO = 0; actualP = 0; actualL = 1;
                singleLine = GCode[index].ToUpper().Trim(); // get line
                getGCodeLine(singleLine);                   // parse line, fill up newLine.xyz and actualM,P,O
                calcAbsPosition(newLine, oldLine);          // calc abs position
                if ((actualM == 98) && processSubs)
                    newLine.codeLine = "(" + GCode[index] + ")";
                else
                    newLine.codeLine = GCode[index];// + String.Format(" X:{0} Y:{1} Z:{2} ", newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);
                gcodeList.Add(new gcodeLine(newLine));      // add parsed line to list
                coordList.Add(new coordinateLine(index, newLine.actualPos));
                if (!programEnd)
                {   //if ((newLine.motionMode > 0))// || (newLine.z != null))
                    //    xyzSize.setDimensionXYZ(newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);             // calculate max dimensions
                    // add data to drawing path
                    createDarwingPathFromGCode(newLine.motionMode, oldLine.actualPos.X, oldLine.actualPos.Y, oldLine.actualPos.Z, newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z, newLine.i, newLine.j);
                    oldLine = new gcodeLine(newLine);   // get copy of newLine      
                }
                if ((actualM == 30)|| (actualM == 2)) { programEnd = true; }
                if (actualM == 98)
                {   findAddSubroutine(actualP, GCode, actualL, processSubs);      // scan complete GCode for matching O-word
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
            for (int index = 0; index < GCode.Length; index++)
            {
                actualM = 0;
                singleLine = GCode[index].ToUpper().Trim(); // get line
                getGCodeLine(singleLine);                   // parse line, fill up newLine.xyz and actualM,P,O
                if (actualO == foundP)
                {   if (!foundO)
                    {
                        subStart = index; //MessageBox.Show("Find O " + index.ToString());
                        foundO = true;
                    }
                    else
                    {                    
                        if (actualM == 99)
                        {
                            subEnd = index; //MessageBox.Show("Find M " + index.ToString());
                            break;
                        }
                    }
                }
            }
            if ((subStart > 0) && (subEnd > subStart))
            {
                string debug = "O "+ actualO.ToString()+"\r\n";
                //MessageBox.Show("P-Word " + foundP.ToString()+"  O "+ subStart.ToString());
                newLine.resetAll();         // reset coordinates and parser mode
                for (int loop = 0; loop < repeat; loop++)
                {
                    for (int index = subStart + 1; index < subEnd; index++)   // go through real line numbers and parse sub-code
                    {
                        newLine.resetCoordinates();
                        newLine.lineNumber = index;
                        singleLine = GCode[index].ToUpper().Trim(); // get line
                        getGCodeLine(singleLine);                   // parse line, fill up newLine.xyz and actualM,P,O
                        calcAbsPosition(newLine, oldLine);          // calc abs position
                        newLine.codeLine = GCode[index];// + String.Format(" X:{0} Y:{1} Z:{2} ", newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);
                        debug += GCode[index] + "\r\n";
                        if (processSubs)
                            gcodeList.Add(new gcodeLine(newLine));      // add parsed line to list
                        coordList.Add(new coordinateLine(index, newLine.actualPos));
                        if ((newLine.motionMode > 0) || (newLine.z != null))
                            xyzSize.setDimensionXYZ(newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z);             // calculate max dimensions
                                                                                                                                // add data to drawing path
                        createDarwingPathFromGCode(newLine.motionMode, oldLine.actualPos.X, oldLine.actualPos.Y, oldLine.actualPos.Z, newLine.actualPos.X, newLine.actualPos.Y, newLine.actualPos.Z, newLine.i, newLine.j);
                        oldLine = new gcodeLine(newLine);   // get copy of newLine                         
                    }
                }//MessageBox.Show(debug);
            }
            return String.Format("Start:{0} EndX:{1} ", subStart, subEnd);
        }

        private void calcAbsPosition(gcodeLine newLine, gcodeLine oldLine)
        {
            if (newLine.x != null)
            {
                if (newLine.isdistanceModeG90)
                {
                    newLine.actualPos.X = (double)newLine.x;
                    xyzSize.setDimensionX(newLine.actualPos.X);
                }
                else
                {   newLine.actualPos.X = oldLine.actualPos.X + (double)newLine.x;
                    xyzSize.setDimensionX(newLine.actualPos.X);// - toolPosX);
                }
            }
            else
                newLine.actualPos.X = oldLine.actualPos.X;
            if (newLine.y != null)
            {
                if (newLine.isdistanceModeG90)
                {   newLine.actualPos.Y = (double)newLine.y;
                    xyzSize.setDimensionY(newLine.actualPos.Y);
                }
                else
                {   newLine.actualPos.Y = oldLine.actualPos.Y + (double)newLine.y;
                    xyzSize.setDimensionY(newLine.actualPos.Y);// - toolPosY);
                }
            }
            else
                newLine.actualPos.Y = oldLine.actualPos.Y;
            if (newLine.z != null)
            {
                if (newLine.isdistanceModeG90)
                {   newLine.actualPos.Z = (double)newLine.z;
                    xyzSize.setDimensionZ(newLine.actualPos.Z); // removed - toolPosZ
                }
                else
                {   newLine.actualPos.Z = oldLine.actualPos.Z + (double)newLine.z;
                    xyzSize.setDimensionZ(newLine.actualPos.Z);// - toolPosZ);
                }
            }
            else
                newLine.actualPos.Z = oldLine.actualPos.Z;
        }

        /// <summary>
        /// parse single gcode line
        /// </summary>
        private void getGCodeLine(string line)
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
                                    parseGCodeToken(cmd,value);
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
                        parseGCodeToken(cmd, double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo));
                }
                catch { }
            }
        }

        /// <summary>
        /// fill current gcode line structure
        /// </summary>
        private void parseGCodeToken(char cmd, double value)
        {   switch (Char.ToUpper(cmd))
            {   case 'X':
                    newLine.x = value;
                    break;
                case 'Y':
                    newLine.y = value;
                    break;
                case 'Z':
                    newLine.z = value;
                    break;
                case 'A':
                    newLine.a = value;
                    break;
                case 'B':
                    newLine.b = value;
                    break;
                case 'C':
                    newLine.c = value;
                    break;
                case 'U':
                    newLine.u = value;
                    break;
                case 'V':
                    newLine.v = value;
                    break;
                case 'W':
                    newLine.w = value;
                    break;
                case 'I':
                    newLine.i = value;
                    break;
                case 'J':
                    newLine.j = value;
                    break;
                case 'F':
                    newLine.feedRate = (int)value;
                    break;
                case 'S':
                    newLine.spindleSpeed = (int)value;
                    break;
                case 'G':
                    if (value <= 3)
                    {   newLine.motionMode = (byte)value;
                        if (value >=2)
                            containsG2G3 = true;
                    }
                    if (value == 90)
                    { newLine.isdistanceModeG90 = true; }
                     //   containsG91 = false; }
                    if (value == 91)
                    {   newLine.isdistanceModeG90 = false;        
                        containsG91 = true;  }
                    break;
                case 'M':
                    if (value >= 3 && value <= 5)
                        newLine.spindleState = (byte)value;
                    if (value >= 7 && value <= 9)
                        newLine.coolantState = (byte)value;
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
        {   //MessageBox.Show(String.Format("Line:{0} X:{1} Y:{2} ", line, gcodeList[line].actualPos.X, gcodeList[line].actualPos.Y));
            if (line < coordList.Count)
            {   if (line == coordList[line].lineNumber)
                {
                    setPosMarker(coordList[line].actualPos.X, coordList[line].actualPos.Y);
                    createMarkerPath();
                }
                else
                {
                    foreach (coordinateLine gcline in coordList)
                    {   if (line == gcline.lineNumber)
                        {
                            setPosMarker(gcline.actualPos.X, gcline.actualPos.Y);
                            createMarkerPath();
                            break;
                        }
                    }
                }
            }
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
            setPosMarker(SortedList[line].actualPos.X, SortedList[line].actualPos.Y);
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
            oldLine.actualPos.X = toolPosX;
            oldLine.actualPos.Y = toolPosY;
            oldLine.actualPos.Z = toolPosZ;
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeLine gcline in gcodeList)
            {
                // switch circle direction
                if ((shiftToZero == translate.MirrorX) || (shiftToZero == translate.MirrorY))           // mirror xy 
                {   if (gcline.motionMode == 2) { gcline.motionMode = 3; }
                    else if (gcline.motionMode == 3) { gcline.motionMode = 2; }
                }
                if (shiftToZero == translate.MirrorX)           // mirror x
                {
                    if (gcline.x != null)
                    {   if (gcline.isdistanceModeG90)
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

                //newLine = new gcodeLine(gcline);
                calcAbsPosition(gcline, oldLine);
                if (gcline.motionMode > 0)
                    xyzSize.setDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);             // calculate max dimensions
                // add data to drawing path
                createDarwingPathFromGCode(gcline.motionMode, oldLine.actualPos.X, oldLine.actualPos.Y, oldLine.actualPos.Z, gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z, gcline.i, gcline.j);
                oldLine = new gcodeLine(gcline);   // get copy of newLine
            }
            return createGCodeProg(false,false);
        }
        public string transformGCodeRotate(double angle)
        {
            double? newvalx, newvaly, newvali, newvalj;
            oldLine.resetAll();         // reset coordinates and parser modes
            oldLine.actualPos.X = toolPosX;
            oldLine.actualPos.Y = toolPosY;
            oldLine.actualPos.Z = toolPosZ;
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeLine gcline in gcodeList)
            {
                if ((gcline.x != null) || (gcline.y != null))
                {   if (gcline.isdistanceModeG90)
                    {
                        newvalx = gcline.actualPos.X * Math.Cos(angle * Math.PI / 180) - gcline.actualPos.Y * Math.Sin(angle * Math.PI / 180);
                        newvaly = gcline.actualPos.X * Math.Sin(angle * Math.PI / 180) + gcline.actualPos.Y * Math.Cos(angle * Math.PI / 180);
                    }
                    else
                    {   if (gcline.x == null) { gcline.x = 0; }
                        if (gcline.y == null) { gcline.y = 0; }
                        newvalx = gcline.x * Math.Cos(angle * Math.PI / 180) - gcline.y * Math.Sin(angle * Math.PI / 180);
                        newvaly = gcline.x * Math.Sin(angle * Math.PI / 180) + gcline.y * Math.Cos(angle * Math.PI / 180);
                    }
                    gcline.x = newvalx;
                    gcline.y = newvaly;
                }
                if ((gcline.i != null) || (gcline.j != null))
                {
                    newvali = (double)gcline.i* Math.Cos(angle* Math.PI / 180) - (double)gcline.j * Math.Sin(angle* Math.PI / 180);
                    newvalj = (double)gcline.i * Math.Sin(angle* Math.PI / 180) + (double)gcline.j * Math.Cos(angle* Math.PI / 180);
                    gcline.i = newvali;
                    gcline.j = newvalj;
                }
                //newLine = new gcodeLine(gcline);
                calcAbsPosition(gcline, oldLine);
                if (gcline.motionMode > 0)
                    xyzSize.setDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);             // calculate max dimensions
                // add data to drawing path
                createDarwingPathFromGCode(gcline.motionMode, oldLine.actualPos.X, oldLine.actualPos.Y, oldLine.actualPos.Z, gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z, gcline.i, gcline.j);
                oldLine = new gcodeLine(gcline);   // get copy of newLine
            }
            return createGCodeProg(false,false);
        }
        public string transformGCodeScale(double scaleX, double scaleY)
        {
            double factor_x = scaleX / 100;
            double factor_y = scaleY / 100;
            oldLine.resetAll();         // reset coordinates and parser modes
            oldLine.actualPos.X = toolPosX;
            oldLine.actualPos.Y = toolPosY;
            oldLine.actualPos.Z = toolPosZ;
            clearDrawingnPath();                    // reset path, dimensions
            foreach (gcodeLine gcline in gcodeList)
            {
                if (gcline.x != null)
                    gcline.x = gcline.x * factor_x;
                if (gcline.y != null)
                    gcline.y = gcline.y * factor_y;
                if (gcline.i != null)
                    gcline.i = gcline.i * factor_x;
                if (gcline.j != null)
                    gcline.j = gcline.j * factor_y;

                //newLine = new gcodeLine(gcline);
                calcAbsPosition(gcline, oldLine);
                if (gcline.motionMode > 0)
                    xyzSize.setDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);             // calculate max dimensions
                // add data to drawing path
                createDarwingPathFromGCode(gcline.motionMode, oldLine.actualPos.X, oldLine.actualPos.Y, oldLine.actualPos.Z, gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z, gcline.i, gcline.j);
                oldLine = new gcodeLine(gcline);   // get copy of newLine
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
            oldLine.actualPos.X = toolPosX;
            oldLine.actualPos.Y = toolPosY;
            oldLine.actualPos.Z = toolPosZ;
            if (shiftToZero == translate.Offset1) { offsetX = x + xyzSize.minx; offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset2) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2; offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset3) { offsetX = x + xyzSize.minx + xyzSize.dimx; offsetY = y + xyzSize.miny + xyzSize.dimy; }
            if (shiftToZero == translate.Offset4) { offsetX = x + xyzSize.minx; offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset5) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2; offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset6) { offsetX = x + xyzSize.minx + xyzSize.dimx; offsetY = y + xyzSize.miny + xyzSize.dimy / 2; }
            if (shiftToZero == translate.Offset7) { offsetX = x + xyzSize.minx; offsetY = y + xyzSize.miny; }
            if (shiftToZero == translate.Offset8) { offsetX = x + xyzSize.minx + xyzSize.dimx / 2; offsetY = y + xyzSize.miny; }
            if (shiftToZero == translate.Offset9) { offsetX = x + xyzSize.minx + xyzSize.dimx; offsetY = y + xyzSize.miny; }
            clearDrawingnPath();                    // reset path, dimensions
            if (containsG91)    // insert rapid movement before pen down, to be able applying offset
            {
                newLine.resetAll();
                int i,k;
                for (i = 0; i < gcodeList.Count; i++)       // find first relative move
                {   if ((!gcodeList[i].isdistanceModeG90) && (gcodeList[i].motionMode == 0) && (gcodeList[i].z != null))       
                    { break; }
                }
                for (k = i + 1; k < gcodeList.Count; k++)   // find G0 x y
                {   if ((gcodeList[k].motionMode == 0) && (gcodeList[k].x != null) && (gcodeList[k].y != null))
                    { noInsertNeeded = true; break; }
                    if (gcodeList[k].motionMode > 0)
                        break;
                }
                if ((gcodeList[i + 1].motionMode != 0) || ((gcodeList[i + 1].motionMode == 0)  && ((gcodeList[i + 1].x == null) || (gcodeList[i + 1].y == null))))
                {   if (!noInsertNeeded)
                    {//MessageBox.Show(gcodeList[i + 1].motionMode.ToString()+" "+ gcodeList[i + 1].x.ToString()+" "+ gcodeList[i + 1].y.ToString());
                        getGCodeLine("G0 X0 Y0 (Insert offset movement)");                   // parse line, fill up newLine.xyz and actualM,P,O
                        gcodeList.Insert(i + 1, newLine);
                    }
                }
            }
            foreach (gcodeLine gcline in gcodeList)
            {
                if (gcline.codeLine.IndexOf("(Setup - GCode") < 1)
                {
                    if (gcline.isdistanceModeG90)
                    {
                        if (gcline.x != null)
                            gcline.x = gcline.x - offsetX;      // apply offset
                        if (gcline.y != null)
                            gcline.y = gcline.y - offsetY;      // apply offset
                    }
                    else
                    {   if (!offsetApplied)
                        {   if (gcline.motionMode == 0)
                            {
                                    gcline.x = gcline.x - offsetX;
                                    gcline.y = gcline.y - offsetY;
                                    if ((gcline.x != null) && (gcline.y != null))
                                        offsetApplied = true;
                            }
                        }
                    }
                }
                calcAbsPosition(gcline, oldLine);
                if (gcline.motionMode > 0)
                    xyzSize.setDimensionXYZ(gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z);             // calculate max dimensions
                // add data to drawing path
                createDarwingPathFromGCode(gcline.motionMode, oldLine.actualPos.X, oldLine.actualPos.Y, oldLine.actualPos.Z, gcline.actualPos.X, gcline.actualPos.Y, gcline.actualPos.Z, gcline.i, gcline.j);
                oldLine = new gcodeLine(gcline);   // get copy of newLine
            }
            return createGCodeProg(false, false);
        }

        public string replaceG23()
        {   return createGCodeProg(true,false); }

        /// <summary>
        /// generate GCode from given coordinates in GCodeList
        /// only replace lines with coordinate information
        /// </summary>
        private string createGCodeProg(bool replaceG23, bool applyNewZ, HeightMap Map=null)
        {
            StringBuilder newCode = new StringBuilder();
            StringBuilder tmpCode = new StringBuilder();
            bool getCoordinate;
            double feedRate = 0;
            double spindleSpeed=0;
            double lastActualX = 0, lastActualY = 0,i,j;
            double newZ = 0;
            int lastMotionMode = 0;
            foreach (gcodeLine gcline in gcodeList)
            {
                tmpCode.Clear();
                getCoordinate = false;
                if (gcline.codeLine.Length == 0)
                    continue;

                if (replaceG23)
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
                    { tmpCode.AppendFormat(" X{0}", gcode.frmtNum((double)gcline.x)); getCoordinate = true; }
                    if (gcline.y != null)
                    { tmpCode.AppendFormat(" Y{0}", gcode.frmtNum((double)gcline.y)); getCoordinate = true; }
                    if (getCoordinate && applyNewZ && (Map != null))
                    {   newZ = Map.InterpolateZ(gcline.actualPos.X, gcline.actualPos.Y);
                        gcline.z = gcline.actualPos.Z + newZ;
                    }

                    if (gcline.z != null)
                    { tmpCode.AppendFormat(" Z{0}", gcode.frmtNum((double)gcline.z)); getCoordinate = true; }
                    if (gcline.i != null)
                    { tmpCode.AppendFormat(" I{0}", gcode.frmtNum((double)gcline.i)); getCoordinate = true; }
                    if (gcline.j != null)
                    { tmpCode.AppendFormat(" J{0}", gcode.frmtNum((double)gcline.j)); getCoordinate = true; }
                    if (getCoordinate)
                    {   if ((gcline.motionMode > 0) && (lastMotionMode != gcline.motionMode))//(feedRate != gcline.feedRate)
                        { tmpCode.AppendFormat(" F{0,0}", gcline.feedRate); }
                        if (spindleSpeed != gcline.spindleSpeed)
                        { tmpCode.AppendFormat(" S{0,0}", gcline.spindleSpeed); }
                        tmpCode.Replace(',', '.');
                        if (gcline.codeLine.IndexOf("(Setup - GCode") > 1)
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
        }

        // add given coordinates to drawing path
        private int onlyZ = 0;
        public void createDarwingPathFromGCode(int motionMode, double ox, double oy, double oz, double nx, double ny, double nz, double? ii, double? jj)
        {
            //MessageBox.Show(String.Format("G{0} ox{1} oy{2} nx{3} ny{4} i{5} j{6}", motionMode, ox, oy, nx, ny, ii, jj));
            bool passLimit = false;
            var pathOld = path;
            if (nz <= zLimit)                    // select path to draw depending on Z-Value
            { path = pathPenDown; }
            else
            { path = pathPenUp;  }
            if (path != pathOld)
                passLimit = true;
            path.StartFigure();                 // Start Figure but never close to avoid connecting first and last point
            if (motionMode == 0 || motionMode == 1)
            {   if ((nx != ox) || (ny != oy))
                {
                    path.AddLine((float)ox, (float)oy, (float)nx, (float)ny);
                    onlyZ = 0;  // x or y has changed
                }
                else
                { onlyZ++; }
                // mark Z-only movements - could be drills
                if ((onlyZ > 1) && (passLimit) && (path == pathPenUp))  // pen moved from -z to +z
                {
                    createMarker(pathPenDown, (float)nx, (float)ny, 1.0f, 1, false);
                    createMarker(pathPenUp, (float)nx, (float)ny, 1.0f, 4, false);
                    path = pathPenUp;      
                    onlyZ = 0;
                }
            }
            if ((motionMode == 2 || motionMode == 3) && (ii != null || jj != null))
            {   if (ii == null) { ii = 0; }
                if (jj == null) { jj = 0; }
                double i = (double)ii;
                double j = (double)jj;
                float radius = (float)Math.Sqrt(i * i + j * j);
                if (radius == 0)               // kleinster Wert > 0
                { radius = 0.0000001f; }

                float x1 = (float)(ox + i - radius);
                float y1 = (float)(oy + j - radius);

                float cos1 = (float)i / radius;
                if (cos1 > 1) cos1 = 1;
                if (cos1 < -1) cos1 = -1;
                float a1 = 180-180*(float)(Math.Acos(cos1)/Math.PI);

                if (j > 0) { a1 = -a1; }
                float cos2 = (float)(ox + i - nx) / radius;
                if (cos2 > 1) cos2 = 1;
                if (cos2 < -1) cos2 = -1;
                float a2 = 180-180*(float)(Math.Acos(cos2)/Math.PI);

                if ((oy+j-ny) > 0) { a2 = -a2; }
                float da = -(360 + a1 - a2);
                if (motionMode == 3) { da=-(360 + a2 - a1); }
                if (da > 360) { da -= 360; }
                if (da < -360) { da += 360; }
                if (motionMode == 2)
                {   path.AddArc(x1, y1, 2 * radius, 2 * radius, a1, da);
                    xyzSize.setDimensionCircle(x1 + radius, y1 + radius, radius, a1, da);        // calculate new dimensions
                }
                else
                {   path.AddArc(x1, y1, 2 * radius, 2 * radius, a1, -da);
                    xyzSize.setDimensionCircle(x1 + radius, y1 + radius, radius, a1, -da);       // calculate new dimensions
                }
            }
        }

        // setup drawing area 
        public void createImagePath()
        {   double extend = 1.01;                                                       // extend dimension a little bit
            double roundTo = 5;                                                         // round-up dimensions
            drawingSize.minX = Math.Floor(xyzSize.minx* extend / roundTo)* roundTo;                  // extend dimensions
            if (drawingSize.minX >= 0) { drawingSize.minX = -roundTo; }                                          // be sure to show 0;0 position
            drawingSize.maxX = Math.Ceiling(xyzSize.maxx* extend / roundTo) * roundTo;
            drawingSize.minY = Math.Floor(xyzSize.miny* extend / roundTo) * roundTo;
            if (drawingSize.minY >= 0) { drawingSize.minY = -roundTo; }
            drawingSize.maxY = Math.Ceiling(xyzSize.maxy* extend / roundTo) * roundTo;
            double xRange = (drawingSize.maxX - drawingSize.minX);                                              // calculate new size
            double yRange = (drawingSize.maxY - drawingSize.minY);
            zeroOffsetX = drawingSize.minX;
            zeroOffsetY = drawingSize.minY;
            picScaling = Math.Min(picWidth/(xRange), picHeight/(yRange));               // calculate scaling px/unit
            createRuler(drawingSize.maxX, drawingSize.maxY);
            createMarkerPath();
        }

        public void createMarkerPath()
        {
            float msize = (float) Math.Max(xyzSize.dimx, xyzSize.dimy) / 50; 
            createMarker(pathTool, (float)toolPosX, (float)toolPosY, msize, 2);
            createMarker(pathMarker, (float)markerPosX, (float)markerPosY, msize, 3);
        }
        private void createRuler(double maxX, double maxY)
        {
            for (int i = 0; i < maxX; i++)          // horizontal ruler
            {   pathRuler.StartFigure();
                if (i % 5 == 0)
                {   if (i % 100 == 0)
                    { pathRuler.AddLine((float)i, 0, (float)i, -5F); }  // 100                    
                    else if ((i % 10 == 0) && (maxX < 1000))
                    { pathRuler.AddLine((float)i, 0, (float)i, -3F); }  // 10                  
                    else if (maxX < 500)
                    { pathRuler.AddLine((float)i, 0, (float)i, -2F); }  // 5
                    }
                    else if (maxX < 200)
                    { pathRuler.AddLine((float)i, 0, (float)i, -1F); }  // 1
            }
            for (int i = 0; i < maxY; i++)          // vertical ruler
            {   pathRuler.StartFigure();
                if (i % 5 == 0)
                {   if (i % 100 == 0)
                    { pathRuler.AddLine(0, (float)i, -5F, (float)i); } // 100                   
                    else if ((i % 10 == 0) && (maxY < 1000))
                    { pathRuler.AddLine(0, (float)i, -3F, (float)i); } // 10           
                    else if (maxY < 500)
                    { pathRuler.AddLine(0, (float)i, -2F, (float)i); } // 5
                }
                else if (maxY < 200)
                { pathRuler.AddLine(0, (float)i, -1F, (float)i); }     // 1
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


    // calculate overall dimensions of drawing
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
                for (double i = start; i < end; i += 10)
                {   setDimensionX(x+radius*Math.Cos(i/180*Math.PI));
                    setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                }
            }
            else
            {
                for (double i = start; i > end; i -= 10)
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
            return String.Format("    Min.   | Max.\r\nX:{0,8:####0.00} |{1,8:####0.00}\r\nY:{2,8:####0.00} |{3,8:####0.00}\r\nZ:{4,8:####0.00} |{5,8:####0.00}", minx, maxx, miny, maxy, minz,maxz);
        }
    }
}
