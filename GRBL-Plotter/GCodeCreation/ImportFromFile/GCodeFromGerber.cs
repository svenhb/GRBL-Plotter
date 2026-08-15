/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
 *			- path modifications: remove offset, hatch FillToolListElements, repeat paths, sort by distance and merge, 
 *			- tangential axis, drag-knife, clipping and tiling, path extension
 *
 * Level 3: graphic2Gcode: translate graphic-paths into GCode commands
 *
 * Level 4: gcodeRelated: implement Pen up/down options, cutter correction, write GCode commands 
*/

/*
 * 2020-07-31 Implementation https://www.ucamco.com/files/downloads/file/81/the_gerber_file_format_specification.pdf
                https://d1.amobbs.com/bbs_upload782111/files_11/ourdev_450330.pdf
	            https://github.com/rsmith-nl/nctools/blob/master/doc/GERBER.pdf

 * 2020-08-15 if aperture is applied (D10...) lines will be drawn as elongated hole segments, applying apertures-radius
 * seperate M19 'advanced' (for notch) to get closed path
 * 2020-12-08 add BackgroundWorker updates
 * 2021-07-01 try to guess missing settings: aperture (line 175), number format (line 700)
 * 2021-07-31 code clean up / code quality
 * 2022-01-19 line 145 err.Message instead of e.String()
 * 2023-06-29 l:512 f:SetType remove "handleM19" it has no function
 * 2026-06-29 add gerber X2 
 * 2026-07-02 add aperture template-paths
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GrblPlotter
{
    class GCodeFromGerber
    {
        /***** Display settings *************************************/
        private static bool drawCenterLineOnly = false;
        private static bool ListAperturesInGCode = false;
        /************************************************************/

        private static readonly List<string> messageList = new List<string>();   // flag to remember if warning was sent

        public static string conversionInfo = "";
        private static int shapeCounter = 0;

        private static double setX = 1;
        private static double setY = 1;
        private static double setI = 0;
        private static double setJ = 0;

        private static int numberFormatIX;  // integer part
        private static int numberFormatFX;  // floating part
        private static int numberFormatIY;  // integer part
        private static int numberFormatFY;  // floating part
        private static bool numberFormatUpdate;

        private static int gMode = 1;

        private static bool isPenDown = false;
        private static bool isUnitInch = true;
        private static double scaleFactor = 1;

        private static Aperture actualAperture = new Aperture();
        private static Dictionary<string, Aperture> apertures = new Dictionary<string, Aperture>();
        private static bool xyIsGivenInCommand = false;
        private static bool d1IsGivenInCommand = false;
        private static bool OutstandingStartPath = false;
        private static bool SetStartCoordinate = false;
        private static Point StartCoordinate = new Point();
        private static Point lastMove = new Point();

        //private static bool handleM19 = true;

        private static string geometryPen = "pen";
        private static string geometryKnife = "knife";
        private static string geometryM19 = "notch";
        private static string toolDiameter = "0.1";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logDetailed = false;
        private static bool logCoordinate = false;

        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent = null;


        /// <summary>
        /// Entrypoint for conversion: apply file-path 
        /// </summary>
        /// <param name="file">String keeping file-name</param>
        /// <returns>String with GCode of imported data</returns>
        public static bool ConvertFromFile(string file, BackgroundWorker worker, DoWorkEventArgs e)
        {
            Logger.Info(" Create GCode from {0}", file);

            backgroundWorker = worker;
            backgroundEvent = e;

            if (String.IsNullOrEmpty(file))
            {
                MessageBox.Show("Empty file name");
                return false;
            }
            else if (file.Substring(0, 4) == "http")
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
                    catch (Exception err)
                    {
                        Logger.Error(err, "Error loading Gerber Code");
                        MessageBox.Show("Error '" + err.Message + "' in Gerber file " + file); //throw;
                    }
                }
                else { MessageBox.Show("Gerber file does not exist: " + file); return false; }
            }
            return false;
        }

        private static bool ConvertGerber(string gerberCode, string filePath)
        {
            Logger.Info(" convertGerber {0}", filePath);
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnables.Detailed) > 0);
            logCoordinate = logEnable && ((logFlags & (uint)LogEnables.Coordinates) > 0);
            if (logEnable) Logger.Trace("  logging:{1}", Convert.ToString(logFlags, 2));

            conversionInfo = "";
            shapeCounter = 0;

            isPenDown = false;
            isUnitInch = true;
            scaleFactor = 1;
            setX = 0; setY = 0; setI = 0; setJ = 0;
            gMode = 1;

            numberFormatIX = 2;
            numberFormatFX = 5;
            numberFormatIY = 2;
            numberFormatFY = 5;
            numberFormatUpdate = false;

            actualAperture = new Aperture();
            apertures = new Dictionary<string, Aperture>();
            string key = "def";  //%ADD16R,0.07874X0.06299*%
            string val = "C,0.03200X0";
            if (logEnable) Logger.Trace("__Set 1 Aperture {0}  {1}", key, val);
            apertures.Add(key, new Aperture(key, val, 0, true));

            //handleM19 = Properties.ListSettings.Default.importGerberTypeEnable;
            geometryPen = Properties.Settings.Default.importGerberTypePen;
            geometryKnife = Properties.Settings.Default.importGerberTypeKnife;
            geometryM19 = Properties.Settings.Default.importGerberTypeM19;

            toolDiameter = Properties.Settings.Default.importGerberFillToolDiameter.ToString().Replace(',', '.');

            messageList.Clear();

            Graphic.Init(Graphic.SourceType.Gerber, filePath, backgroundWorker, backgroundEvent);
            Graphic.SetPenColor("black");
            Graphic.SetPenFill("black");

            GetVectorGerber(gerberCode);                        // convert graphics
            conversionInfo += string.Format("{0} elements imported", shapeCounter);
            return Graphic.CreateGCode();
        }

        private static void GetVectorGerber(string gerberCode)
        {   // https://github.com/rsmith-nl/nctools/blob/master/nctools/dumpgerber.py
            // https://github.com/rsmith-nl/nctools/blob/master/doc/GERBER.pdf

            char[] charsToTrim = { ' ', '\r', '\n' };
            string line;

            string[] lines = gerberCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string longExtendedCommand = "";
            bool nextIsInfo = false;

            Logger.Info(" Amount Lines:{0}", lines.Length);
            backgroundWorker?.ReportProgress(0, new MyUserState { Value = 10, Content = "Read Gerber vector data of " + lines.Length.ToString() + " length" });

            bool logNotImplemented = false;

            int lineNr = 0;
            bool sectionStarted = false;
            bool isActiveAM = false;
            bool isUnknown = false;
            string[] unknownCmd = new string[] { "%AS", "%MI", "%OF", "%SF", "%TF", "%I", "%P", "%TA", "%TO", "%TD", "%LPD" };
            foreach (string singleLine in lines)
            {
                line = singleLine.Trim(charsToTrim);

                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress(lineNr++ * 100 / lines.Length);
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundEvent.Cancel = true;
                        break;
                    }
                }

                if (line.StartsWith("G04"))	// just a comment
                { //Graphic.SetHeaderInfo(" " + line.Substring(3));
                    continue;
                }

                if (line.StartsWith("%"))
                {
                    sectionStarted = true;
                }
                #region extended
                if (sectionStarted)
                {
                    if (logDetailed) Logger.Trace("  Extended Command:{0}", line);

                    isUnknown = false;
                    foreach (string ucmd in unknownCmd)
                    {
                        if (line.Contains(ucmd))     // Axis select
                        {
                            if (logNotImplemented) Logger.Trace("  Extended Command not implemented:{0}", line);
                            if (line.EndsWith("%"))
                            {
                                sectionStarted = false;
                                isActiveAM = false;
                            }
                            isUnknown = true;
                            break;
                        }
                    }
                    if (isUnknown)
                        continue;

                    if (isActiveAM)
                    { //Logger.Trace("isActiveAM "); 
                    }

                    else if (line.Contains("%AM"))
                    {
                        if (logNotImplemented) Logger.Trace("  Extended Command not implemented:{0}", line);
                        isActiveAM = true;
                        // Logger.Trace("isActiveAM = true");
                    }

                    else if (line.Contains("%MO"))         // Mode of units
                    {
                        if (line.Contains("MM"))
                            isUnitInch = false;
                        else if (line.Contains("IN"))
                            isUnitInch = true;
                        if (logEnable) Logger.Trace("__Set units  isUnitInch:{0}", isUnitInch);
                    }

                    else if (line.Contains("%FS"))     // Format Statement 
                    {   // %FSLAX25Y25*% Leading zero’s omitted, Absolute coordinates, 
                        if (line.Contains("L")) { }         // Leading zeros omitted
                        else if (line.Contains("T")) { }    // Trailing zeros omitted
                        else if (line.Contains("D")) { }    // Explicit decimal point (i.e. no zeros omitted)

                        if (line.Contains("A")) { }         // Absolute coordinate mode
                        if (line.Contains("I")) { }         // Incremental coordinate mode

                        int pX = line.IndexOf("X");    // Coordinates format is 2.5:
                        if (pX > 2)
                        {
                            numberFormatIX = GetInt(line.Substring(pX + 1, 1)); // 2 digits in the integer part
                            numberFormatFX = GetInt(line.Substring(pX + 2, 1)); // 5 digits in the fractional part
                            numberFormatUpdate = true;
                        }
                        int pY = line.IndexOf("Y");    // Coordinates format is 2.5:
                        if (pY > 2)
                        {
                            numberFormatIY = GetInt(line.Substring(pY + 1, 1)); // 2 digits in the integer part
                            numberFormatFY = GetInt(line.Substring(pY + 2, 1)); // 5 digits in the fractional part
                            numberFormatUpdate = true;
                        }
                        if (logEnable) Logger.Trace("__Set number format XI:{0} XF:{1} YI:{2} YF:{3}", numberFormatIX, numberFormatFX, numberFormatIY, numberFormatFY);
                    }

                    else if (line.Contains("%AD"))//Define the aperture: D10 is a circle with diameter 0.01 inch
                    {
                        string key = line.Substring(3, 3);  //%ADD16R,0.07874X0.06299*%
                        string val = line.Substring(6, line.Length - 8);

                        bool keepCenterClear = true;
                        double penWidth = (double)Properties.Settings.Default.importGerberFillToolDiameter;

                        if (!Properties.Settings.Default.importGerberFillEnable)
                            penWidth = 0;
                        if (logEnable) Logger.Trace("Set Aperture: key:{0}  val:{1}  pen-dia:{2}  centerClear:{3}", key, val, penWidth, keepCenterClear);
                        apertures.Add(key, new Aperture(key, val, penWidth, keepCenterClear));
                    }

                    else
                    { Logger.Trace("#####  Extended Command not implemented:{0}", line); }
                    // }
                }
                #endregion

                if (line.EndsWith("%"))
                {
                    sectionStarted = false;
                    isActiveAM = false;
                }
                else if (!sectionStarted)
                {
                    string[] commands = line.Split('*');

                    /*	Process commands lines*/
                    foreach (string cmdline in commands)
                    {
                        if (cmdline.Length <= 1)
                            continue;
                        if (logCoordinate) Logger.Trace("..cmdline {0}", cmdline);

                        if (nextIsInfo)
                        {
                            if (logDetailed) Logger.Trace("   setInfo {0}", cmdline);
                            Graphic.SetHeaderInfo(" " + cmdline);
                            nextIsInfo = false;
                            continue;
                        }
                        if (cmdline.Contains("M20"))            // Info
                        {
                            if (logDetailed) Logger.Trace("   nextIsInfo ");
                            nextIsInfo = true; continue;
                        }

                        string separators = @"(?=[A-Za-z-[e]])";
                        var tokens = Regex.Split(cmdline, separators).Where(t => !string.IsNullOrEmpty(t));
                        /* Process X,Y,D...*/
                        xyIsGivenInCommand = false;
                        d1IsGivenInCommand = false;
                        {
                            foreach (string token in tokens)
                                if (token.Length > 0)
                                {
                                    if (!ParseCommand(token))
                                    { Logger.Warn("ParseCommand cmdline:'{0}'", cmdline); }
                                }
                        }
                        if (cmdline.Contains("X"))
                        { if (logCoordinate && logDetailed) Logger.Trace("....coord  X:{0:0.00}  Y:{1:0.00}  I:{2:0.00}  J:{3:0.00} ", setX, setY, setI, setJ); }

                        Point tmp = ScalePosition(setX, setY);

                        if (xyIsGivenInCommand)
                        {
                            if (logDetailed) Logger.Trace("   xyIsGivenInCommand");

                            if (SetStartCoordinate)
                            {
                                StartCoordinate = tmp;
                                SetStartCoordinate = false;
                                if (logDetailed) Logger.Trace("   SetStartCoordinate {0:0.00}  {1:0.00}", StartCoordinate.X, StartCoordinate.Y);
                            }
                            if (OutstandingStartPath && ((actualAperture.apType == Aperture.Type.none) || (actualAperture.sizes[0] == 0)))   // if D1 was in prev line
                            { PenDown(StartCoordinate); }

                            if (isPenDown && !d1IsGivenInCommand)   // move to next coordinate
                            { Draw(); }
                        }
                    }
                }
                else
                {
                    if (!isActiveAM)
                        Logger.Error(" undefined state {0}", line);
                }
            }
        }
        private static int GetInt(string val)
        {
            //    int num;
            if (!int.TryParse(val, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int num))
            { Logger.Error(" Fail to convert integer-part of {0} ", val); }
            return num;
        }

        private static bool ParseCommand(string token)
        {
            if (logDetailed) Logger.Trace("   ParseCommand {0}", token);
            char command = token[0];
            if (token.Length == 1)
            {
                if (command == 'A') { ProcessD(2); }
                ;  	// knife up
                if (command == 'B') { SetType(geometryKnife); ProcessD(1); }  	// knife down
                return true;
            }
            string val = token.Substring(1);

            if (int.TryParse(val, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out int value))
            {
                //Logger.Trace("ParseCommand command:{0} value:{1}", token, value);
                if (command == 'X') { SetXValue(val); xyIsGivenInCommand = true; }
                else if (command == 'Y') { SetYValue(val); xyIsGivenInCommand = true; }
                else if (command == 'I') { SetIValue(val); xyIsGivenInCommand = true; }
                else if (command == 'J') { SetJValue(val); xyIsGivenInCommand = true; }

                else if (command == 'D')
                {
                    if (value < 10)
                    {
                        SetType(geometryPen);
                        ProcessD(value);
                    }
                    else
                    {
                        if (!apertures.ContainsKey(token))
                        {
                            actualAperture = apertures["def"];
                            Logger.Error("Aperture key not found {0}, use default", token);
                            token += "_nok";
                        }
                        else//if (apertures.ContainsKey(token))
                        {
                            actualAperture = apertures[token];
                            if (logEnable) Logger.Trace("Select Aperture: {0}  {1}", token, actualAperture.apType.ToString());
                        }
                        Graphic.SetLayer(token);
                        if (ListAperturesInGCode)
                            Graphic.SetHeaderInfo(token + " = " + actualAperture.content);
                        if (Properties.Settings.Default.importGerberFillEnable)
                            Graphic.SetPenWidth(toolDiameter);
                        //Graphic.SetPenWidth("0.01");// toolDiameter);
                        else
                            Graphic.SetPenWidth(actualAperture.sizes[0].ToString().Replace(',', '.'));
                        if (logEnable) Logger.Trace("   apply aperture {0}  {1}", token, actualAperture.apType.ToString());
                    }
                }

                else if (command == 'G')
                {
                    if ((value >= 1) && (value <= 3))
                    {
                        gMode = value;
                        if (logEnable) Logger.Trace("   set G {0}", value);
                    }
                }

                else if (command == 'M')
                { ProcessM(value); }


                else if (command == 'N')
                { Graphic.SetLayer("Sequence_" + val.ToString().PadLeft(4, '0')); }


                else if (command == 'R')
                { }


                else if (command == 'H')
                {
                    numberFormatIX = 6; numberFormatFX = 2;
                    numberFormatIY = 6; numberFormatFY = 2;
                    numberFormatUpdate = true;
                }
            }
            else
            {
                Logger.Warn("ParseCommand command {0} fail int", token);
                return false;
            }
            return true;
        }

        private static void ProcessD(int value)
        {
            if (logDetailed) Logger.Trace("....processD {0}", value);

            if (value == 1)				// D1 Pen down
            {
                d1IsGivenInCommand = true;

                if (xyIsGivenInCommand)	// no XY? Wait until XY is given
                {
                    if (!isPenDown && ((actualAperture.apType == Aperture.Type.none) || (actualAperture.sizes[0] == 0)))
                    { PenDown(StartCoordinate); }

                    Draw();
                }
                else
                    OutstandingStartPath = true;
            }
            else if (value == 2)		// D2 = Pen up
            {
                PenUp();
                if (xyIsGivenInCommand)
                {
                    StartCoordinate = ScalePosition(setX, setY);
                    if (logDetailed) Logger.Trace("   D2 SetStartCoordinate {0:0.00}  {1:0.00}", StartCoordinate.X, StartCoordinate.Y);
                }
                else
                    SetStartCoordinate = true;
            }
            else if (value == 3)		// D3 = Dot
            {
                PenUp();
                ApplyApertureShape();
                isPenDown = false;
            }
        }

        private static void ProcessM(int value)
        {
            if (logEnable) Logger.Trace("....processM {0}", value);
            if (value == 14) { SetType(geometryKnife); ProcessD(1); }	//knife down - same as B
            if (value == 15) { ProcessD(2); }                           //knife up - same as A

            if (value == 19)
            {
                Graphic.StopPath(); isPenDown = false;
                SetType(geometryM19);
                //Graphic.StartPath(lastMove);	// start at centerXY position
                PenDown(lastMove);
                isPenDown = true;
            }
        }

        private static void SetType(string geometry)
        {
            //    if (handleM19)
            if (logDetailed) Logger.Trace("    setType {0}", geometry);
            Graphic.SetType(geometry);
        }

        private static void PenUp()
        {
            if (logDetailed) Logger.Trace("----penUp()  isPenDown:{0}", isPenDown);
            if (isPenDown)
                Graphic.StopPath();
            isPenDown = false;
        }

        private static void PenDown(Point tmp)
        {
            string geo = "Gerber_" + shapeCounter.ToString();
            //            Graphic.SetGeometry(geo);
            if (logDetailed) Logger.Trace("++++penDown()  X:{0:0.000}  Y:{1:0.000} isPenDown:{2}  geometry:{3}", tmp.X, tmp.Y, isPenDown, geo);

            if (!isPenDown)
            { Graphic.StartPath(tmp); }
            else
            {
                Graphic.StopPath();
                Graphic.StartPath(tmp);
            }
            isPenDown = true;
            shapeCounter++;
            OutstandingStartPath = false;
        }

        /* Command D01 */
        /*
		collect lines, arcs dicitionary(apertureNr, path)
		sort paths - find connections
		*/
        private static void Draw()
        {
            Point tmp = ScalePosition(setX, setY);
            if (logDetailed)
                Logger.Trace("++++Draw()   aperture:{0}  X:{1:0.000}  Y:{2:0.000} isPenDown:{3}  gMode:{4}", actualAperture.name, tmp.X, tmp.Y, isPenDown, gMode);

            if (gMode == 1)
            {
                if ((actualAperture.apType == Aperture.Type.none) || (actualAperture.sizes[0] == 0))
                {
                    if (logCoordinate)
                        Logger.Trace("....AddLine    X:{0:0.000}  Y:{1:0.000} ", tmp.X, tmp.Y);
                    Graphic.AddLine(tmp); isPenDown = true;
                }
                else
                    DrawSlotLine(StartCoordinate, tmp, actualAperture.sizes[0]);
            }        // move to with pen down
            else
            {
                if ((actualAperture.apType == Aperture.Type.none) || (actualAperture.sizes[0] == 0))
                {
                    if (logCoordinate) Logger.Trace("....AddArc     X:{0:0.000}  Y:{1:0.000}  I:{2:0.00}  J:{3:0.00} ", tmp.X, tmp.Y, setI, setJ);
                    Graphic.AddArc((gMode == 2), tmp, ScalePosition(setI, setJ));
                }
                else
                    DrawSlotArc(StartCoordinate, tmp, ScalePosition(setI, setJ), actualAperture.sizes[0]);
            }
            lastMove = tmp;
        }

        private static void DrawSlotArc(Point pStart, Point pEnd, Point IJ, double xInch)
        {
            double toolDiameter = 0;
            if (Properties.Settings.Default.importGerberFillEnable)
                toolDiameter = (double)Properties.Settings.Default.importGerberFillToolDiameter;
            double r = (ScaleValue(xInch) - toolDiameter) / 2;   // get mm value
            if (r < 0) r = 0;

            if (logCoordinate)
                Logger.Trace("drawSlotArc pStart:x:{0:0.00} y:{1:0.00}  pEnd:x:{2:0.00} y:{3:0.00}  r:{4:0.00} ", pStart.X, pStart.Y, pEnd.X, pEnd.Y, r);

            if (drawCenterLineOnly || (r == 0)) // draw center line
            {
                Graphic.StopPath(); isPenDown = false;
                //Graphic.StartPath(p1out);
                PenDown(pEnd);
                Graphic.AddArc(true, pEnd, IJ);
                Graphic.StopPath(); isPenDown = false;
                StartCoordinate = pEnd;
                return;
            }

            Point center = Sub(pEnd, IJ);
            double angle = GcodeMath.GetAlpha(center, pStart);
            Point p1out = CalcOffsetPoint(pStart, angle, r);
            Point p1in = CalcOffsetPoint(pStart, angle - Math.PI, r);

            angle = GcodeMath.GetAlpha(center, pEnd);
            Point p2out = CalcOffsetPoint(pEnd, angle, r);
            Point p2in = CalcOffsetPoint(pEnd, angle - Math.PI, r);

            // isCW?

            Graphic.StopPath(); isPenDown = false;
            //Graphic.StartPath(p1out);
            PenDown(p1out);
            Graphic.AddArc(true, p2out, Sub(pEnd, p2out));
            Graphic.AddArc(true, p2in, Sub(pEnd, p2out));
            Graphic.AddArc(true, p1in, Sub(pStart, p2in));
            Graphic.AddArc(true, p1out, Sub(pStart, p1in));
            Graphic.StopPath(); isPenDown = false;

            StartCoordinate = pEnd;
        }
        private static void DrawSlotLine(Point pStart, Point pEnd, double xInch)
        {
            double toolDiameter = 0;
            if (Properties.Settings.Default.importGerberFillEnable)
                toolDiameter = (double)Properties.Settings.Default.importGerberFillToolDiameter;
            double r = (ScaleValue(xInch) - toolDiameter) / 2;   // get mm value
            if (r < 0) r = 0;

            if (logCoordinate)
                Logger.Trace("drawSlotLine pStart:x:{0:0.00} y:{1:0.00}  pEnd:x:{2:0.00} y:{3:0.00}  r:{4:0.00} ", pStart.X, pStart.Y, pEnd.X, pEnd.Y, r);
            if (pStart == pEnd)
            {
                Logger.Error("..same coordinates x:{0}  y:{1}, nothing to do", pStart.X, pStart.Y);
                return;
            }
            if (drawCenterLineOnly || (r == 0)) // draw center line
            {
                PenDown(pStart);
                Graphic.AddLine(pEnd);
                Graphic.StopPath(); isPenDown = false;
                StartCoordinate = pEnd;
                return;
            }

            double quarter = Math.PI / 2;
            double angle = GcodeMath.GetAlpha(pStart, pEnd);
            Point p1l = CalcOffsetPoint(pStart, angle + quarter, r);
            Point p1r = CalcOffsetPoint(pStart, angle - quarter, r);
            Point p2l = CalcOffsetPoint(pEnd, angle + quarter, r);
            Point p2r = CalcOffsetPoint(pEnd, angle - quarter, r);

            Graphic.StopPath(); isPenDown = false;

            if (Properties.Settings.Default.importGerberFillEnable)
            {
                /* take account of tool width */
                int fillCount = (int)Math.Ceiling(2 * r / toolDiameter);
                double newToolDiameter = 2 * r / fillCount;
                double dist, extend, toolR = toolDiameter / 2;
                double aa, rr = r * r;

                Point p1f, p2f;
                bool moveBack = true;
                if (fillCount > 1)
                {
                    PenDown(p1l);
                    Graphic.AddLine(p2l);
                    Graphic.AddArc(true, p2r, Sub(pEnd, p2l));
                    Graphic.AddLine(p1r);
                    Graphic.AddArc(true, p1l, Sub(pStart, p1r));
                    for (int i = 1; i < fillCount; i++)
                    {
                        dist = i * newToolDiameter;
                        if (dist > r)
                            aa = dist - r;
                        else
                            aa = r - dist;
                        extend = Math.Sqrt(rr - (aa * aa));

                        //    if (Math.Abs(fillCount / 2 - i) < 2)
                        //        extend = -toolDiameter;

                        p1f = CalcOffsetPoint(p1l, angle - quarter, dist);
                        p1f = CalcOffsetPoint(p1f, angle, -extend);

                        p2f = CalcOffsetPoint(p2l, angle - quarter, dist);
                        p2f = CalcOffsetPoint(p2f, angle + Math.PI, -extend);
                        if (moveBack)
                        {
                            Graphic.AddLine(p1f);
                            Graphic.AddLine(p2f);
                        }
                        else
                        {
                            Graphic.AddLine(p2f);
                            Graphic.AddLine(p1f);
                        }
                        moveBack = !moveBack;
                    }
                }
                else // just the outline are needed
                {
                    if (r > toolDiameter / 2)
                    {
                        PenDown(p1l);
                        Graphic.AddLine(p2l);
                        Graphic.AddArc(true, p2r, Sub(pEnd, p2l));
                        Graphic.AddLine(p1r);
                        Graphic.AddArc(true, p1l, Sub(pStart, p1r));
                    }
                    else
                    {
                        PenDown(p1l);
                        Graphic.AddLine(p2l);
                        Graphic.AddLine(p2r);
                        Graphic.AddLine(p1r);
                        Graphic.AddLine(p1l);
                    }
                }
            }
            else // just draw outline
            {
                PenDown(p1l);
                Graphic.AddLine(p2l);
                Graphic.AddArc(true, p2r, Sub(pEnd, p2l));
                Graphic.AddLine(p1r);
                Graphic.AddArc(true, p1l, Sub(pStart, p1r));
            }
            Graphic.StopPath(); isPenDown = false;

            StartCoordinate = pEnd;
        }
        private static Point Sub(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        private static Point CalcOffsetPoint(Point P, double angle, double radius)
        {
            Point tmp = new Point
            {
                X = P.X + Math.Cos(angle) * radius,
                Y = P.Y + Math.Sin(angle) * radius
            };
            return tmp;
        }

        /* Command D03 */
        private static void ApplyApertureShape()
        {
            Point centerPos = ScalePosition(setX, setY);
            StartCoordinate = centerPos;

            double toolDiameter = 0;
            if (Properties.Settings.Default.importGerberFillEnable)
                toolDiameter = (double)Properties.Settings.Default.importGerberFillToolDiameter * 0.9;

            if (actualAperture.sizes.Count == 0)
            { Logger.Error("ApplyApertureShape  no sizes set"); return; }

            if (logEnable) Logger.Trace("applyApertureShape() {0}  x:{1}  y:{2}  toolDiameter:{3}", actualAperture.apType.ToString(), centerPos.X, centerPos.Y, toolDiameter);

            if (actualAperture.apType == Aperture.Type.Circle)
            {
                AddGraphicsItemPath(actualAperture.shapePath.Path, centerPos);
            }
            else if (actualAperture.apType == Aperture.Type.Rectangle)
            {
                AddGraphicsItemPath(actualAperture.shapePath.Path, centerPos);
            }
            else if (actualAperture.apType == Aperture.Type.Octagon)
            {
                AddGraphicsItemPath(actualAperture.shapePath.Path, centerPos);
            }
            else if (actualAperture.apType == Aperture.Type.Obround)
            {
                AddGraphicsItemPath(actualAperture.shapePath.Path, centerPos);
            }
            else if (actualAperture.apType == Aperture.Type.RoundRect)
            {
                AddGraphicsItemPath(actualAperture.shapePath.Path, centerPos);
            }
            else
            {
                Logger.Warn("ApplyApertureShape not implemented:{0}", actualAperture.apType);
                double tmpX, tmpY;
                tmpY = tmpX = ScaleValue(actualAperture.sizes[0]) - toolDiameter;
                if (actualAperture.sizes.Count > 1)
                    tmpY = ScaleValue(actualAperture.sizes[1]) - toolDiameter;
                if (tmpX < 0) tmpX = 0.05;
                if (tmpY < 0) tmpY = 0.05;
                tmpX /= 2; tmpY /= 2;
                double sX = ScaleValue(setX);
                double sY = ScaleValue(setY);
                Graphic.StartPath(sX - tmpX, sY - tmpY);
                Graphic.AddLine(sX - tmpX, sY + tmpY);
                Graphic.AddLine(sX + tmpX, sY + tmpY);
                Graphic.AddLine(sX + tmpX, sY - tmpY);
                Graphic.AddLine(sX - tmpX, sY - tmpY);
                Graphic.StopPath(); isPenDown = false;
            }
        }

        private static Point GetMinMaxCoord(List<double> vals, int start, int count, bool getMin)
        {
            if ((start + 2 * count) <= vals.Count)
            {
                double x = vals[start];
                double y = vals[start + 1];
                if (count > 1)
                {
                    for (int i = start + 2; i < start + 2 * count; i += 2)
                    {
                        if (getMin)
                        {
                            x = Math.Min(x, vals[i]);
                            y = Math.Min(x, vals[i + 1]);
                        }
                        else
                        {
                            x = Math.Max(x, vals[i]);
                            y = Math.Max(x, vals[i + 1]);
                        }
                    }
                }
                return new Point(x, y);
            }
            return new Point();
        }

        private static void AddGraphicsItemPath(List<Graphic.GCodeMotion> Path, Point offset)
        {
            if (Path.Count > 0)
            {
                Graphic.StartPath(Path[0].MoveTo.X + offset.X, Path[0].MoveTo.Y + offset.Y);
                for (int i = 1; i < Path.Count; i++)
                {
                    Graphic.AddMotion(Path[i], offset.X, offset.Y);
                }
                Graphic.StopPath(); isPenDown = false;
            }
        }

        private static Point ScalePosition(Point val)
        { return ScalePosition(val.X, val.Y); }

        private static Point ScalePosition(double valX, double valY)
        {
            double x = valX, y = valY;

            if (isUnitInch)
            {
                x *= 25.4 * scaleFactor;
                y *= 25.4 * scaleFactor;
            }
            else
            {
                x *= scaleFactor;
                y *= scaleFactor;
            }
            Point tmp = new Point(x, y);
            return tmp;
        }

        private static double ScaleValue(double valX)
        {
            if (isUnitInch)
                return valX * 25.4 * scaleFactor;
            else
                return valX * scaleFactor;
        }

        private static void SetXValue(string val)
        { setX = CalcValue(val, numberFormatIX, numberFormatFX); }
        private static void SetYValue(string val)
        { setY = CalcValue(val, numberFormatIY, numberFormatFY); }
        private static void SetIValue(string val)
        { setI = CalcValue(val, numberFormatIX, numberFormatFX); }
        private static void SetJValue(string val)
        { setJ = CalcValue(val, numberFormatIY, numberFormatFY); }

        private static double CalcValue(string val, int i, int f)
        {
            double value = 0;
            int partI = 0, partF = 0;
            string pflt = "";
            string pint = "";

            if ((val == "0") || (String.IsNullOrEmpty(val)))
                return 0.0;

            int valLen = val.Length;
            if (!numberFormatUpdate)    // i=4, f=0
            {
                int tmpi = 0, tmpf = 0;
                for (int a = 0; a < valLen; a++)
                {
                    if (Char.IsNumber(val[a]))
                        tmpi++;
                    else
                        break;
                }
                for (int a = tmpi + 1; a < valLen; a++)
                {
                    if (Char.IsNumber(val[a]))
                        tmpf++;
                    else
                        break;
                }
                numberFormatIX = numberFormatIY = tmpi;
                numberFormatFX = numberFormatFY = tmpf;
                Logger.Error("__guess number format XI:{0} XF:{1} YI:{2} YF:{3}", numberFormatIX, numberFormatFX, numberFormatIY, numberFormatFY);
                if (numberFormatFX == 0)
                { scaleFactor = 0.001; Logger.Info("__set scaleFctor to 0.001"); }
                numberFormatUpdate = true;
            }

            if (val.Length >= f)
            {
                pflt = val.Substring(valLen - f);
                pint = val.Substring(0, valLen - f);

                if (pflt.Length > 0)
                {
                    if (!int.TryParse(pflt, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out partF))
                    { Logger.Error(" Fail to convert float-part of {0} i:{1} f:{2}", val, i, f); }
                }
                if (pint.Length > 0)
                {
                    if (pint == "-")
                        partI = -0;
                    else
                        if (!int.TryParse(pint, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out partI))
                    { Logger.Error(" Fail to convert integer-part of {0} i:{1} f:{2}", val, i, f); }
                }
                if (logDetailed) Logger.Trace("        pint:'{0}' i:'{1}' pflt:{2}  f:{3}", pint, partI, pflt, partF);

                value = ((double)partF * Math.Pow(10, -f));
                if ((pint == "-") || (partI < 0))
                    value *= -1;
                value += (double)partI;
                //                if (logDetailed) Logger.Trace( "  val:{0}  I:'{1}' I:{2}  F:'{3}'  F:{4}   value:{5:0.00000000}", val, pint, partI, pflt, partF, value);
            }
            if (logDetailed) Logger.Trace("      convert val:{0}  pint:'{1}' pflt:'{2}' result:{3:0.0000}  final:{4:0.0000}", val, pint, pflt, value, ScaleValue(value));

            return value;
        }

        /*********************************************************************************************/
        class Aperture
        {
            public enum Type { none, Circle, Rectangle, Obround, Octagon, Polygon, RoundRect };
            public Type apType;
            public List<double> sizes;
            public double diameter;
            public string name;
            public string content;
            public Graphic.ItemPath shapePath;

            public Aperture()
            {
                apType = Type.none;
                sizes = new List<double>();
                diameter = 0;
                content = "";
                name = "";
                shapePath = new Graphic.ItemPath();
            }

            public Aperture(string key, string val, double penWidth, bool keepCenterClear)
            {
                sizes = new List<double>();
                shapePath = new Graphic.ItemPath();
                diameter = 0;
                name = key;
                content = val;
                if (val.Length > 1)
                {
                    string[] parts = val.Split(',');
                    int coordinates = 1;
                    if (parts.Length > 1)
                    {
                        if (parts[0] == "C") { apType = Type.Circle; coordinates = 1; }
                        if (parts[0] == "R") { apType = Type.Rectangle; coordinates = 2; }
                        if (parts[0] == "O") { apType = Type.Obround; coordinates = 2; }
                        if (parts[0] == "OC8") { apType = Type.Octagon; coordinates = 2; }
                        if (parts[0] == "P") { apType = Type.Polygon; coordinates = 0; }
                        if (parts[0] == "RoundRect") { apType = Type.RoundRect; coordinates = 9; }

                        if (parts[1].Contains("X"))
                        {
                            string[] sizeToken = parts[1].Split('X');
                            for (int i = 0; i < sizeToken.Length; i++)
                            { sizes.Add(GetNumber(sizeToken[i])); }

                            if (sizeToken.Length > coordinates)
                                diameter = GetNumber(sizeToken[sizeToken.Length - 1]);
                        }
                        else
                        { sizes.Add(GetNumber(parts[1])); }

                        CreateShapePath(penWidth, keepCenterClear);
                    }
                }
            }

            private static double GetNumber(string val)
            {
                if (!double.TryParse(val, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out double tmp))
                { Logger.Error(" getNumber {0} ", val); }
                return tmp;
            }

            private void CreateShapePath(double penWidth, bool keepCenterClear)
            {
                /* create path template with center = 0;0 */
                bool fillShape = penWidth > 0;
                bool failPenWidth = false;
                bool isG2 = true;
                bool arcToLine = false;
                bool optionNoise = false;

                double centerX = 0, centerY = 0, radiusFallBack = 0.05;

                //   Logger.Trace("CreateShapePath type:{0}  penWidth:{1}", apType, penWidth);

                if (apType == Aperture.Type.Circle)
                {
                    drawCircle();
                }
                else if (apType == Aperture.Type.Rectangle)
                {
                    if (sizes.Count < 2)
                    { Logger.Error("CreateShapePath type:{0}  sizes.Count:{1} not enough data ", apType, sizes.Count); return; }

                    double rX = (ScaleValue(sizes[0]) - penWidth) / 2;
                    double rY = (ScaleValue(sizes[1]) - penWidth) / 2;
                    if (rX <= 0) { rX = radiusFallBack; failPenWidth = true; }
                    if (rY <= 0) { rY = radiusFallBack; failPenWidth = true; }

                    Point start = new Point(centerX - rX, centerY - rY);
                    shapePath = new Graphic.ItemPath(start);
                    shapePath.Add(centerX - rX, centerY + rY, 0, 0);
                    shapePath.Add(centerX + rX, centerY + rY, 0, 0);
                    shapePath.Add(centerX + rX, centerY - rY, 0, 0);
                    shapePath.Add(centerX - rX, centerY - rY, 0, 0);

                    if (fillShape)
                    {
                        double centerRadius = penWidth;
                        if (!keepCenterClear)
                            centerRadius = 0;

                        rX -= penWidth; rY -= penWidth;
                        while ((rX > centerRadius) || (rY > centerRadius))
                        {
                            if (rX <= centerRadius) rX = centerRadius;
                            if (rY <= centerRadius) rY = centerRadius;
                            shapePath.Add(centerX - rX, centerY + rY, 0, 0);
                            shapePath.Add(centerX + rX, centerY + rY, 0, 0);
                            shapePath.Add(centerX + rX, centerY - rY, 0, 0);
                            shapePath.Add(centerX - rX, centerY - rY, 0, 0);
                            rX -= penWidth; rY -= penWidth;
                        }
                        if (keepCenterClear)
                        {
                            shapePath.Add(new Point(centerX - penWidth, centerY), 0, 0);
                            shapePath.AddArc(new Point(centerX - penWidth, centerY), new Point(penWidth, 0), 0, isG2, arcToLine, optionNoise);
                            Logger.Trace("x:{0}  y:{1}  w:{2}", centerX, centerY, penWidth);
                        }
                    }
                }
                else if (apType == Aperture.Type.Obround)
                {
                    if (sizes.Count < 2)
                    { Logger.Error("CreateShapePath type:{0}  sizes.Count:{1} not enough data ", apType, sizes.Count); return; }

                    double rX = (ScaleValue(sizes[0]) - penWidth) / 2;
                    double rY = (ScaleValue(sizes[1]) - penWidth) / 2;
                    if (rX <= 0) { rX = radiusFallBack; failPenWidth = true; }
                    if (rY <= 0) { rY = radiusFallBack; failPenWidth = true; }

                    if (rX > rY)
                    {
                        shapePath = new Graphic.ItemPath(new Point(centerX + rX - rY, centerY - rY));
                        drawObround(rX, rY);
                    }
                    else
                    {
                        shapePath = new Graphic.ItemPath(new Point(centerX - rX, centerY - rY + rX));
                        drawObround(rX, rY);
                    }
                    if (fillShape)
                    {
                        double centerRadius = penWidth;
                        if (!keepCenterClear)
                            centerRadius = 0;

                        rX -= penWidth; rY -= penWidth;
                        while ((rX > centerRadius) || (rY > centerRadius))
                        {
                            if (rX <= centerRadius) rX = centerRadius;
                            if (rY <= centerRadius) rY = centerRadius;
                            drawObround(rX, rY);
                            rX -= penWidth; rY -= penWidth;
                        }
                        if (keepCenterClear)
                        {
                            shapePath.Add(new Point(centerX, centerY - penWidth), 0, 0);
                            shapePath.AddArc(new Point(centerX, centerY - penWidth), new Point(0, penWidth), 0, isG2, arcToLine, optionNoise);
                        }
                    }
                }
                else if (apType == Aperture.Type.Polygon)
                {
                    Logger.Error("CreateShapePath Polygon is not implemented - use circle");
                    if (sizes.Count < 2)
                    { Logger.Error("CreateShapePath type:{0}  sizes.Count:{1} not enough data ", apType, sizes.Count); return; }

                    drawCircle();
                }
                else if (apType == Aperture.Type.Octagon)
                {
                    Logger.Error("CreateShapePath Octagon is not implemented - use circle");
                    if (sizes.Count < 2)
                    { Logger.Error("CreateShapePath type:{0}  sizes.Count:{1} not enough data ", apType, sizes.Count); return; }

                    drawCircle();
                }
                else if (apType == Aperture.Type.RoundRect)
                {
                    double cr = ScaleValue(sizes[0]);
                    Point cMin = ScalePosition(GetMinMaxCoord(sizes, 1, 4, true));
                    Point cMax = ScalePosition(GetMinMaxCoord(sizes, 1, 4, false));
                    //    cMin.X += penWidth/2; cMin.Y += penWidth/2;
                    //    cMax.X -= penWidth/2; cMax.Y -= penWidth/2;

                    shapePath = new Graphic.ItemPath(new Point(centerX + cMin.X, centerY + cMin.Y + cr));
                    drawRoundRect(centerX + cMin.X, centerY + cMin.Y, centerX + cMax.X, centerY + cMax.Y, cr);

                    if (fillShape)
                    {
                        double centerRadius = penWidth;
                        if (!keepCenterClear)
                            centerRadius = 0;

                        cMin.X += penWidth; cMin.Y += penWidth;
                        cMax.X -= penWidth; cMax.Y -= penWidth;
                        cr -= penWidth;
                        penWidth *= 0.8;
                        while ((cMax.X > centerRadius) || (cMax.Y > centerRadius))
                        {
                            if (cMax.X < centerRadius) cMax.X = centerRadius;
                            if (cMax.Y < centerRadius) cMax.Y = centerRadius;
                            if (cMin.X > -centerRadius) cMin.X = -centerRadius;
                            if (cMin.Y > -centerRadius) cMin.Y = -centerRadius;
                            if (cr < 0) cr = 0;
                            drawRoundRect(centerX + cMin.X, centerY + cMin.Y, centerX + cMax.X, centerY + cMax.Y, cr);
                            cMin.X += penWidth; cMin.Y += penWidth;
                            cMax.X -= penWidth; cMax.Y -= penWidth;
                            cr -= penWidth;
                        }
                        if (keepCenterClear)
                        {
                            cMax.X = centerRadius; cMax.Y = centerRadius;
                            cMin.X = -centerRadius; cMin.Y = -centerRadius;
                            cr = 0;
                            drawRoundRect(centerX + cMin.X, centerY + cMin.Y, centerX + cMax.X, centerY + cMax.Y, cr);
                            Point centerXY = new Point(centerX - centerRadius, centerY);
                            Point centerIJ = new Point(centerRadius, 0);
                            shapePath.Add(centerXY, 0, 0);
                            shapePath.AddArc(centerXY, centerIJ, 0, isG2, arcToLine, optionNoise);
                        }
                    }
                }

                void drawCircle()
                {
                    double radius = (ScaleValue(sizes[0]) - penWidth) / 2;
                    if (radius <= 0) { radius = radiusFallBack; failPenWidth = true; }


                    Point arcStart = new Point(centerX + radius, centerY);
                    shapePath = new Graphic.ItemPath(arcStart);
                    shapePath.AddArc(arcStart, new Point(-radius, 0), 0, isG2, arcToLine, optionNoise);  // full circle

                    if (fillShape)
                    {
                        Point arcEnd = new Point(centerX + radius, centerY);
                        Point arcIJ = new Point(-radius, 0);
                        Point centerXY = arcStart;
                        Point centerIJ = arcIJ;

                        double centerRadius = penWidth;
                        if (!keepCenterClear)
                            centerRadius = 0;

                        double rDiff = radius - centerRadius;
                        int cnt = (int)Math.Ceiling(rDiff / penWidth);
                        double rDec = radius;
                        if (cnt > 0) { rDec = (rDiff / cnt) / 2; }
                        radius -= rDec;

                        //    Logger.Trace("penWidth:{0}  rDiff:{1}  cnt:{2}  rDec:{3}", penWidth, rDiff, cnt, rDec);
                        if (radius > 0)
                        {
                            centerXY.X = centerX + centerRadius; centerIJ.X = -centerRadius;
                            while (radius > centerRadius)
                            {
                                arcEnd.X = arcStart.X - 2 * radius;
                                arcIJ.X = -radius;
                                if (radius < rDec / 2)
                                { shapePath.Add(arcEnd, 0, 0); break; }

                                shapePath.AddArc(arcEnd, arcIJ, 0, isG2, arcToLine, optionNoise);
                                radius -= rDec;
                                if (radius <= centerRadius) { centerXY.X = centerX - centerRadius; centerIJ.X = centerRadius; break; }

                                arcStart.X = arcEnd.X + 2 * radius;
                                arcIJ.X = +radius;
                                if (radius < rDec / 2)
                                { shapePath.Add(arcStart, 0, 0); break; }

                                shapePath.AddArc(arcStart, arcIJ, 0, isG2, arcToLine, optionNoise);
                                radius -= rDec;
                                if (radius <= centerRadius) { centerXY.X = centerX + centerRadius; centerIJ.X = -centerRadius; break; }
                            }
                            if (keepCenterClear)
                            {
                                shapePath.Add(centerXY, 0, 0);
                                shapePath.AddArc(centerXY, centerIJ, 0, isG2, arcToLine, optionNoise);
                            }
                        }
                    }
                }

                void drawObround(double rX, double rY)
                {
                    if (rX > rY)    // horizontal
                    {
                        shapePath.Add(new Point(centerX - rX + rY, centerY - rY), 0, 0);
                        shapePath.AddArc(new Point(centerX - rX + rY, centerY + rY), new Point(0, +rY), 0, isG2, arcToLine, optionNoise);  // full circle
                        shapePath.Add(new Point(centerX + rX - rY, centerY + rY), 0, 0);
                        shapePath.AddArc(new Point(centerX + rX - rY, centerY - rY), new Point(0, -rY), 0, isG2, arcToLine, optionNoise);  // full circle
                    }
                    else
                    {
                        shapePath.Add(new Point(centerX - rX, centerY + rY - rX), 0, 0);
                        shapePath.AddArc(new Point(centerX + rX, centerY + rY - rX), new Point(+rX, 0), 0, isG2, arcToLine, optionNoise);  // full circle
                        shapePath.Add(new Point(centerX + rX, centerY - rY + rX), 0, 0);
                        shapePath.AddArc(new Point(centerX - rX, centerY - rY + rX), new Point(-rX, 0), 0, isG2, arcToLine, optionNoise);  // full circle
                    }
                }

                void drawRoundRect(double x1, double y1, double x2, double y2, double r, bool cw = true)
                {   // start bottom left
                    if (cw)
                    {
                        shapePath.Add(x1, y2 - r, 0, 0);        //BL to TL
                        if (r > 0) { shapePath.AddArc(new Point(x1 + r, y2), new Point(r, 0), 0, isG2, arcToLine, optionNoise); }
                        shapePath.Add(x2 - r, y2, 0, 0);         // TL to TR
                        if (r > 0) { shapePath.AddArc(new Point(x2, y2 - r), new Point(0, -r), 0, isG2, arcToLine, optionNoise); }
                        shapePath.Add(x2, y1 + r, 0, 0);         // TR to BR
                        if (r > 0) { shapePath.AddArc(new Point(x2 - r, y1), new Point(-r, 0), 0, isG2, arcToLine, optionNoise); }
                        shapePath.Add(x1 + r, y1, 0, 0);         // BR to BL
                        if (r > 0) { shapePath.AddArc(new Point(x1, y1 + r), new Point(0, r), 0, isG2, arcToLine, optionNoise); }
                    }
                    else
                    {
                        if (r > 0) { shapePath.AddArc(new Point(x1 + r, y1), new Point(r, 0), 0, isG2, arcToLine, optionNoise); }
                        shapePath.Add(x2 - r, y1, 0, 0);          // to BR
                        if (r > 0) { shapePath.AddArc(new Point(x2, y1 + r), new Point(0, r), 0, isG2, arcToLine, optionNoise); }
                        shapePath.Add(x2, y2 - r, 0, 0);           // to TR
                        if (r > 0) { shapePath.AddArc(new Point(x2 - r, y2), new Point(-r, 0), 0, isG2, arcToLine, optionNoise); }
                        shapePath.Add(x1 + r, y2, 0, 0);           // to TL
                        if (r > 0) { shapePath.AddArc(new Point(x1, y2 - r), new Point(0, -r), 0, isG2, arcToLine, optionNoise); }
                        shapePath.Add(x1, y1 + r, 0, 0);           // to BL 
                    }
                }

            }
        }
    }
}
