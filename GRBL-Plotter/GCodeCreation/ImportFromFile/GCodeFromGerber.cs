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

/* Level 1: import graphics SVG, DXF, HPGL, Drill, CSV
 *			- collect colors, pen-widths, layer-names for grouping
 *          - extract objects, get coordinates, convert Bezier to line-segments
 *			- convert circle to dot (option)
 *
 * Level 2: graphicRelated: collect dots, lines, arcs; sorting by distance, merging, clipping, grouping, tangential axis
 *			- collect path-data (pen-down path): either path with line and arc or just a dot
 *			- path modifications: remove offset, hatch fill, repeat paths, sort by distance and merge, 
 *			- tangential axis, drag-knife, clipping and tiling, path extension
 *
 * Level 3: graphic2Gcode: translate graphic-paths into GCode commands
 *
 * Level 4: gcodeRelated: implement Pen up/down options, cutter correction, write GCode commands 
*/

/*
 * 2020-07-31 Implementation https://www.ucamco.com/files/downloads/file/81/the_gerber_file_format_specification.pdf
                https://d1.amobbs.com/bbs_upload782111/files_11/ourdev_450330.pdf
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    class GCodeFromGerber
    {
        private static List<string> messageList = new List<string>();   // flag to remember if warning was sent

        public static string conversionInfo = "";
        private static int shapeCounter = 0;


        private static bool isCutterData = false;
        private static double setX = 0;
        private static double setY = 0;
        private static double setI = 0;
        private static double setJ = 0;

        private static int numberFormatIX;  // integer part
        private static int numberFormatFX;  // floating part
        private static int numberFormatIY;  // integer part
        private static int numberFormatFY;  // floating part

        private static int gMode = 1;

        private static bool isPenDown = false;
        private static bool isOnlyXY = false;
        private static bool isUnitInch = true;
        private static aperture actualAperture = new aperture();
        private static Dictionary<string,aperture> apertures = new Dictionary<string, aperture>();


        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logDetailed = false;



        /// <summary>
        /// Entrypoint for conversion: apply file-path 
        /// </summary>
        /// <param name="file">String keeping file-name</param>
        /// <returns>String with GCode of imported data</returns>
        public static string ConvertFromFile(string file)
        {
            Logger.Info(" Create GCode from {0}", file);
            if (file == "")
            { MessageBox.Show("Empty file name"); return ""; }

            if (file.Substring(0, 4) == "http")
            { MessageBox.Show("Load via http is not supported up to now"); }
            else
            {
                if (File.Exists(file))
                {
                    try
                    {
                        string GerberCode = File.ReadAllText(file);
                        return ConvertGerber(GerberCode, file);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Error loading Gerber Code");
                        MessageBox.Show("Error '" + e.ToString() + "' in Gerber file " + file); return "";
                    }
                }
                else { MessageBox.Show("Gerber file does not exist: " + file); return ""; }
            }
            return "";
        }

        private static string ConvertGerber(string gerberCode, string filePath)
        {
            Logger.Info(" convertGerber {0}", filePath);
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnable.Level1) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnable.Detailed) > 0);
            if (logEnable) Logger.Trace("  logging:{1}", Convert.ToString(logFlags, 2));

            conversionInfo = "";
            shapeCounter = 0;

            isCutterData = false;
            isPenDown = false;
            isOnlyXY = false;
            isUnitInch = true;
            setX = 0; setY = 0; setI = 0; setJ = 0;
            gMode = 1;

            numberFormatIX = 2;
            numberFormatFX = 5;
            numberFormatIY = 2;
            numberFormatFY = 5;
            actualAperture = new aperture();
            apertures = new Dictionary<string, aperture>();

            messageList.Clear();

            Graphic.Init(Graphic.SourceTypes.Gerber, filePath);
            GetVectorGerber(gerberCode);                        // convert graphics
            conversionInfo += string.Format("{0} elements imported", shapeCounter);
            return Graphic.CreateGCode();
        }

        private static void GetVectorGerber(string gerberCode)
        {   // https://github.com/rsmith-nl/nctools/blob/master/nctools/dumpgerber.py
            // https://github.com/rsmith-nl/nctools/blob/master/doc/GERBER.pdf

            char[] charsToTrim = { ' ', '\r', '\n' };
            string line, cmd, parameter;

            string[] lines = gerberCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string longExtendedCommand = "";
            foreach (string singleLine in lines)
            {
                line = singleLine.Trim(charsToTrim);
                #region extended
                if (line.StartsWith("%"))
                {
                    if (line.EndsWith("%"))
                    {
                        if (line.Length > 2)
                        {
                            if (logDetailed) Logger.Trace("  Extended Command:{0}", line);

                            if (line.Contains("%MO"))         // Mode of units
                            {   if (line.Contains("MM"))
                                    isUnitInch = false;
                                else if (line.Contains("IN"))
                                    isUnitInch = true;
                                if (logEnable) Logger.Trace("__Set units  isUnitInch:{0}", isUnitInch);
                            }

                            else if (line.Contains("%AS"))     // Axis select
                            { Logger.Trace("  Extended Command not implemented:{0}", line); }
                            else if (line.Contains("%MI"))     // Mirror
                            { Logger.Trace("  Extended Command not implemented:{0}", line); }
                            else if (line.Contains("%OF"))     // Offset    %OFA0B0*%
                            { Logger.Trace("  Extended Command not implemented:{0}", line); }
                            else if (line.Contains("%SF"))     // Scale factor
                            { Logger.Trace("  Extended Command not implemented:{0}", line); }

                            else if (line.Contains("%FS"))     // Format Statement 
                            {   // %FSLAX25Y25*% Leading zero’s omitted, Absolute coordinates, 
                                if (line.Contains("L")) { }         // Leading zeros omitted
                                else if (line.Contains("T")) { }    // Trailing zeros omitted
                                else if (line.Contains("D")) { }    // Explicit decimal point (i.e. no zeros omitted)

                                if (line.Contains("A")) { }         // Absolute coordinate mode
                                if (line.Contains("I")) { }         // Incremental coordinate mode

                                int pX = line.IndexOf("X");    // Coordinates format is 2.5:
                                if (pX > 2)                       
                                {   numberFormatIX = getInt(line.Substring(pX + 1, 1)); // 2 digits in the integer part
                                    numberFormatFX = getInt(line.Substring(pX + 2, 1)); // 5 digits in the fractional part
                                }
                                int pY = line.IndexOf("Y");    // Coordinates format is 2.5:
                                if (pY > 2)
                                {   numberFormatIY = getInt(line.Substring(pY + 1, 1)); // 2 digits in the integer part
                                    numberFormatFY = getInt(line.Substring(pY + 2, 1)); // 5 digits in the fractional part
                                }
                                if (logEnable) Logger.Trace("__Set number format XI:{0} XF:{1} YI:{2} YF:{3}", numberFormatIX, numberFormatFX, numberFormatIY, numberFormatFY);
                            }
                            else if (line.Contains("%AD"))//Define the aperture: D10 is a circle with diameter 0.01 inch
                            {
                                string key = line.Substring(3, 3);  //%ADD16R,0.07874X0.06299*%
                                string val = line.Substring(6, line.Length - 8);
                                if (logEnable) Logger.Trace("__Set Aperture {0}  {1}", key, val);
                                apertures.Add(key, new aperture(val));
                            }


                            else if (line.Contains("%I"))     // Image...
                            { Logger.Trace("  Extended Command not implemented:{0}", line); }
                            else if (line.Contains("%P"))     // Image...
                            { Logger.Trace("  Extended Command not implemented:{0}", line); }
                        }
                        else
                        {
                            if (longExtendedCommand.Length > 5)
                            {
                                if (logEnable) Logger.Trace("  Long Extended Command:{0}", longExtendedCommand);
                                longExtendedCommand = "";
                            }
                        }
                    }
                    else
                    { longExtendedCommand = line; }

                }
                #endregion
                else
                {   if (longExtendedCommand.Length > 5)
                    { longExtendedCommand += line; continue; }
                    bool nextIsInfo = false;
                    string[] commands = line.Split('*');
                    foreach (string cmdline in commands)
                    {
                        if (cmdline.Length <= 1)
                            continue;
                        if (logDetailed) Logger.Trace("...cmdline {0}", cmdline);

                        if (nextIsInfo)
                        {   Graphic.SetHeaderInfo(" " + cmdline);
                            nextIsInfo = false;
                            continue;
                        }
                        if (cmdline.Contains("M20"))            // Info
                        { nextIsInfo = true; continue; }

                        if (cmdline.StartsWith("G04"))          // Info
                        { Graphic.SetHeaderInfo(" " + cmdline.Substring(3)); continue; }

                        string separators = @"(?=[A-Za-z-[e]])";
                        var tokens = Regex.Split(cmdline, separators).Where(t => !string.IsNullOrEmpty(t));

                        foreach (string token in tokens)
                            if (token.Length > 0)
                                ParseCommand(token);

                        if (cmdline.Contains("X"))
                        { if (logDetailed) Logger.Trace("....coord  X:{0:0.00}  Y:{1:0.00}  I:{2:0.00}  J:{3:0.00} ", setX, setY, setI, setJ); } 

                        Point tmp = scalePosition(setX, setY);

                        if (isPenDown && isOnlyXY)
                        {
                            if (logDetailed) Logger.Trace("....addLine X:{0:0.00}  Y:{1:0.00}  PenDown:{2}", tmp.X, tmp.Y, isPenDown);
                            Graphic.AddLine(scalePosition(setX, setY));
                        }

             //           if (logDetailed) Logger.Trace(" commands {0}  X:{1:0.00}  Y:{2:0.00}  PenDown:{3}", cmdline, tmp.X, tmp.Y, isPenDown);
                    }
                }
            }
        }
        private static int getInt(string val)
        {
            int num = 2;
            if (!int.TryParse(val, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num))
            { Logger.Error(" Fail to convert integer-part of {0} ", val); }
            return num;
        }

        private static void ParseCommand(string token)
        {
            char command = token[0];
            if (token.Length == 1)
            {
 //               Logger.Trace("ParseCommand single command {0}", token);
                if (command == 'A') penUp();        // knife up
                if (command == 'B') penDown();      // knife down
                return;
            }
            string val = token.Substring(1);
            int value = 0;
            isOnlyXY = false;

            if (int.TryParse(val, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out value))
            {
                //               Logger.Trace("ParseCommand command:{0} value:{1}", token, value);
                if (command == 'X') { setXValue(val); isOnlyXY = true; }
                else if (command == 'Y') { setYValue(val); isOnlyXY = true; }
                else if (command == 'I') { setIValue(val); isOnlyXY = true; }
                else if (command == 'J') { setJValue(val); isOnlyXY = true; }

                else if (command == 'D')
                {
                    if (value < 10)
                        processD(value);
                    else
                    {
                        if (apertures.ContainsKey(token))
                        {
                            actualAperture = apertures[token];
                            Graphic.SetLayer(token);// + " " + actualAperture.content);
                            Graphic.SetHeaderInfo(token + " = " + actualAperture.content);
                            Graphic.SetPenWidth(actualAperture.XSize.ToString());
                            if (logEnable) Logger.Trace("   apply aperture {0}  {1}",token, actualAperture.apType.ToString());
                        }
                        else
                        {   Logger.Error("Aperture key not found {0}", token); }
                    }
                }

                else if (command == 'G')
                {   if ((value >= 1) || (value <= 3))
                        gMode = value;
                    if (logDetailed) Logger.Trace("   set G {0}",value);
                }

                else if (command == 'M')
                { processM(value); }


                else if (command == 'N')
                { Graphic.SetLayer("Sequence " + val); }


                else if (command == 'R')
                { }


                else if (command == 'H')
                {
                    isCutterData = true;
                    numberFormatIX = 6; numberFormatFX = 2;
                    numberFormatIY = 6; numberFormatFY = 2;
                }
            }
            else
            {
                Logger.Trace("ParseCommand command {0} fail int", token);
                if (val.Contains("G04"))
                    Graphic.SetHeaderInfo(val.Substring(3));
            }
        }

        private static void penUp()
        {
            if (logDetailed) Logger.Trace(" penUp()  isPenDown:{0}", isPenDown);
            if (isPenDown)
                Graphic.StopPath();
            isPenDown = false;
        }

        private static void penDown()
        {
            if (logDetailed) Logger.Trace(" penDown()  isPenDown:{0}", isPenDown);
            if (!isPenDown)
                Graphic.StartPath(scalePosition(setX, setY));
            else
            {
                Graphic.StopPath();
                Graphic.StartPath(scalePosition(setX, setY));
            }
            isPenDown = true;
            shapeCounter++;
            Graphic.SetGeometry("Gerber_"+shapeCounter.ToString());
        }

        private static void applyApertureShape()
        {   double tmpX, tmpY, diameter;
            Point startPos = scalePosition(setX, setY);
            tmpX = actualAperture.XSize;
            tmpY = actualAperture.YSize;
            diameter = actualAperture.HoleDiameter;
            if (logDetailed) Logger.Trace("    Dot: applyApertureShape() {0}", actualAperture.apType.ToString());

            if (actualAperture.apType == aperture.type.Circle)
            {
                tmpX /= 2;
                Graphic.StartPath(scalePosition(setX + tmpX, setY));
                Graphic.AddCircle(2, startPos.X, startPos.Y, scaleValue( tmpX));
                Graphic.StopPath();
            }
            else if (actualAperture.apType == aperture.type.Rectangle)
            {
                tmpX /= 2; tmpY /= 2;
                Graphic.StartPath(scalePosition(setX - tmpX, setY - tmpY));
                Graphic.AddLine(scalePosition(setX - tmpX, setY + tmpY));
                Graphic.AddLine(scalePosition(setX + tmpX, setY + tmpY));
                Graphic.AddLine(scalePosition(setX + tmpX, setY - tmpY));
                Graphic.AddLine(scalePosition(setX - tmpX, setY - tmpY));
                Graphic.StopPath();
            }
            else if (actualAperture.apType == aperture.type.Octagon)
            {
                tmpX /= 2; tmpY /= 2;
                Graphic.StartPath(scalePosition(setX - tmpX, setY - tmpY));
                Graphic.AddLine(scalePosition(setX - tmpX, setY + tmpY));
                Graphic.AddLine(scalePosition(setX + tmpX, setY + tmpY));
                Graphic.AddLine(scalePosition(setX + tmpX, setY - tmpY));
                Graphic.AddLine(scalePosition(setX - tmpX, setY - tmpY));
                Graphic.StopPath();
            }

        }

        private static void processD(int value)
        {
            if (logDetailed) Logger.Trace("processD {0}", value);
            if (isCutterData) return;

            if (value == 1)
            {
                if (gMode == 1)
                {   Graphic.AddLine(scalePosition(setX, setY)); isPenDown = true; }        // move to with pen down
                else
                {   Graphic.AddArc((gMode==2), scalePosition(setX, setY), scalePosition(setI, setJ)); }
            }
            else if (value == 2)
            { penDown(); }              // move to with pen up
            else if (value == 3)
            {
                penUp();
                // Graphic.AddDot(getPosition(setX, setY));
                applyApertureShape();
                isPenDown = false;
            }        // make dot
        }

        private static void processM(int value)
        {
            if (logEnable) Logger.Trace("processM {0}", value);
            if (value == 14) penDown();     // knife down - same as B
            if (value == 15) penUp();       // knife up - same as A
        }

        private static Point scalePosition(double valX, double valY)
        {   double x = valX, y = valY;

            if (isUnitInch)
            {   x *= 25.4;
                y *= 25.4;
            }
            Point tmp = new Point(x,y);
            return tmp;
        }
        private static double scaleValue(double valX)
        {
            if (isUnitInch)
                return valX * 25.4;
            else
                return valX;
        }


        private static void setXValue(string val)
        {   setX = calcValue(val, numberFormatIX, numberFormatFX); }
        private static void setYValue(string val)
        {   setY = calcValue(val, numberFormatIY, numberFormatFY); }
        private static void setIValue(string val)
        {   setI = calcValue(val, numberFormatIX, numberFormatFX); }
        private static void setJValue(string val)
        {   setJ = calcValue(val, numberFormatIY, numberFormatFY); }

        private static double calcValue(string val, int i, int f)
        {
            double value = 0;
            int partI = 0, partF = 0;

            if ((val == "0") || (val == ""))
                return 0.0;

            if (val.Length >= f)
            {   int valLen = val.Length;
                string pflt = val.Substring(valLen - f);
                string pint = val.Substring(0, valLen - f);

                if (pflt.Length > 0)
                {
                    if (!int.TryParse(pflt, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out partF))
                    { Logger.Error(" Fail to convert float-part of {0} i:{1} f:{2}",val,i,f); }
                }
                if (pint.Length > 0)
                {   if (pint == "-")
                        partI = -0;
                    else
                        if (!int.TryParse(pint, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out partI))
                        { Logger.Error(" Fail to convert integer-part of {0} i:{1} f:{2}", val, i, f); }
                }

                if (pint == "")
                    value = ((double)partF * Math.Pow(10, -f));
                else if (pint == "-")
                    value = -((double)partF * Math.Pow(10, -f));
                else
                    value = partI + Math.Sign(partI)*((double)partF * Math.Pow(10, -f));
                if (logDetailed) Logger.Trace("  val:{0}  I:'{1}' I:{2}  F:'{3}'  F:{4}   value:{5:0.00000000}", val, pint, partI, pflt, partF, value);
            }
            return value;
        }


        class aperture
        {
            public enum type { Circle, Rectangle, Obround, Octagon, Polygon };
            public type apType;
            public double XSize;
            public double YSize;
            public double HoleDiameter;
            public string content;

            public aperture()
            { }

            public aperture(string val)
            {   content = val;
                XSize = 0; YSize = 0; HoleDiameter = 0;
                if (val.Length > 1)
                {
                    string[] parts = val.Split(',');
                    if (parts.Length > 1)
                    {
                        string[] size = parts[1].Split('X');
                        XSize = YSize = getNumber(size[0]);
                        if (parts[0] == "C")
                        {
                            apType = type.Circle;
                            if (size.Length > 1)  HoleDiameter = getNumber(size[1]);
                        }

                        if (size.Length > 1) YSize = getNumber(size[1]);
                        if (size.Length > 2) HoleDiameter = getNumber(size[2]);
                        if (parts[0] == "R") { apType = type.Rectangle; }
                        if (parts[0] == "O") { apType = type.Obround; }
                        if (parts[0] == "OC8") { apType = type.Octagon; }
                        if (parts[0] == "P") { apType = type.Polygon; }
                    }
                }
            }

            private static double getNumber(string val)
            {
                double tmp = 0;
                if (!double.TryParse(val, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out tmp))
                { Logger.Error(" getNumber {0} ", val); }
                return tmp;
            }
        }
    }
}
