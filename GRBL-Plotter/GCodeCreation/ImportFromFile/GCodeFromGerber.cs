/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
	            https://github.com/rsmith-nl/nctools/blob/master/doc/GERBER.pdf

 * 2020-08-15 if aperture is applied (D10...) lines will be drawn as elongated hole segments, applying apertures-radius
 * seperate M19 'advanced' (for notch) to get closed path
 * 2020-12-08 add BackgroundWorker updates
 * 2021-07-01 try to guess missing settings: aperture (line 175), number format (line 700)
 * 2021-07-31 code clean up / code quality
 * 2022-01-19 line 145 err.Message instead of e.String()
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

namespace GrblPlotter
{
    class GCodeFromGerber
    {
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

        private static bool handleM19 = true;

        private static string geometryPen = "pen";
        private static string geometryKnife = "knife";
        private static string geometryM19 = "notch";


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
            string val = "C,0.03200";
            apertures.Add(key, new Aperture(val));

            handleM19 = Properties.Settings.Default.importGerberTypeEnable;
            geometryPen = Properties.Settings.Default.importGerberTypePen;
            geometryKnife = Properties.Settings.Default.importGerberTypeKnife;
            geometryM19 = Properties.Settings.Default.importGerberTypeM19;

            messageList.Clear();

            Graphic.Init(Graphic.SourceType.Gerber, filePath, backgroundWorker, backgroundEvent);
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
            if (backgroundWorker != null) backgroundWorker.ReportProgress(0, new MyUserState { Value = 10, Content = "Read Gerber vector data of " + lines.Length.ToString() + " length" });

            int lineNr = 0;
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

                if (line.StartsWith("%"))
                {
                    #region extended
                    if (line.EndsWith("%"))
                    {
                        if (line.Length > 2)
                        {
                            if (logDetailed) Logger.Trace("  Extended Command:{0}", line);

                            if (line.Contains("%MO"))         // Mode of units
                            {
                                if (line.Contains("MM"))
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
                                if (logEnable) Logger.Trace("__Set Aperture {0}  {1}", key, val);
                                apertures.Add(key, new Aperture(val));
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
                    #endregion
                }
                else
                {
                    if (longExtendedCommand.Length > 5)
                    { longExtendedCommand += line; continue; }
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

                        if (cmdline.StartsWith("G04"))          // Info
                        { Graphic.SetHeaderInfo(" " + cmdline.Substring(3)); continue; }

                        string separators = @"(?=[A-Za-z-[e]])";
                        var tokens = Regex.Split(cmdline, separators).Where(t => !string.IsNullOrEmpty(t));
                        /* Process X,Y,D...*/
                        xyIsGivenInCommand = false;
                        d1IsGivenInCommand = false;
                        foreach (string token in tokens)
                            if (token.Length > 0)
                                ParseCommand(token);

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
                            if (OutstandingStartPath && ((actualAperture.apType == Aperture.Type.none) || (actualAperture.XSize == 0)))   // if D1 was in prev line
                            { PenDown(StartCoordinate); }

                            if (isPenDown && !d1IsGivenInCommand)   // move to next coordinate
                            { Draw(); }
                        }
                    }
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

        private static void ParseCommand(string token)
        {
            if (logDetailed) Logger.Trace("   ParseCommand {0}", token);
            char command = token[0];
            if (token.Length == 1)
            {
                if (command == 'A') { ProcessD(2); };  	// knife up
                if (command == 'B') { SetType(geometryKnife); ProcessD(1); }  	// knife down
                return;
            }
            string val = token.Substring(1);
            //       int value;

            if (int.TryParse(val, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out int value))
            {
                //               Logger.Trace( "ParseCommand command:{0} value:{1}", token, value);
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
                        }
                        Graphic.SetLayer(token);// + " " + actualAperture.content);
                        Graphic.SetHeaderInfo(token + " = " + actualAperture.content);
                        Graphic.SetPenWidth(actualAperture.XSize.ToString().Replace(',', '.'));
                        if (logEnable) Logger.Trace("   apply aperture {0}  {1}", token, actualAperture.apType.ToString());
                        //   }
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
                Logger.Trace("ParseCommand command {0} fail int", token);
                if (val.Contains("G04"))
                    Graphic.SetHeaderInfo(val.Substring(3));
            }
        }

        private static void ProcessD(int value)
        {
            if (logDetailed) Logger.Trace("....processD {0}", value);

            if (value == 1)				// D1 Pen down
            {
                d1IsGivenInCommand = true;

                if (xyIsGivenInCommand)	// no XY? Wait until XY is given
                {
                    if (!isPenDown && ((actualAperture.apType == Aperture.Type.none) || (actualAperture.XSize == 0)))
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
                //Graphic.StartPath(lastMove);	// start at last position
                PenDown(lastMove);
                isPenDown = true;
            }
        }

        private static void SetType(string geometry)
        {
            if (handleM19)
                //				Graphic.SetPenColor(geometry);
                if (logDetailed) Logger.Trace("    setType {0}", geometry);
            Graphic.SetType(geometry);
            //                Graphic.SetGeometry(geometry);
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

        private static void Draw()
        {
            Point tmp = ScalePosition(setX, setY);
            if (logDetailed) Logger.Trace("++++Draw()     X:{0:0.000}  Y:{1:0.000} isPenDown:{2}  gMode:{3}", tmp.X, tmp.Y, isPenDown, gMode);

            if (gMode == 1)
            {
                if ((actualAperture.apType == Aperture.Type.none) || (actualAperture.XSize == 0))
                {
                    if (logCoordinate) Logger.Trace("....AddLine    X:{0:0.000}  Y:{1:0.000} ", tmp.X, tmp.Y);
                    Graphic.AddLine(tmp); isPenDown = true;
                }
                else
                    DrawSlotLine(StartCoordinate, tmp, actualAperture.XSize);
            }        // move to with pen down
            else
            {
                if ((actualAperture.apType == Aperture.Type.none) || (actualAperture.XSize == 0))
                {
                    if (logCoordinate) Logger.Trace("....AddArc     X:{0:0.000}  Y:{1:0.000}  I:{2:0.00}  J:{3:0.00} ", tmp.X, tmp.Y, setI, setJ);
                    Graphic.AddArc((gMode == 2), tmp, ScalePosition(setI, setJ));
                }
                else
                    DrawSlotArc(StartCoordinate, tmp, ScalePosition(setI, setJ), actualAperture.XSize);
            }
            lastMove = tmp;
        }

        private static void DrawSlotArc(Point pStart, Point pEnd, Point IJ, double xInch)
        {
            double r = ScaleValue(xInch) / 2;
            if (logCoordinate) Logger.Trace("drawSlotArc pStart:x:{0:0.00} y:{1:0.00}  pEnd:x:{2:0.00} y:{3:0.00}  r:{4:0.00} ", pStart.X, pStart.Y, pEnd.X, pEnd.Y, r);
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
            double r = ScaleValue(xInch) / 2;
            if (logCoordinate) Logger.Trace("drawSlotLine pStart:x:{0:0.00} y:{1:0.00}  pEnd:x:{2:0.00} y:{3:0.00}  r:{4:0.00} ", pStart.X, pStart.Y, pEnd.X, pEnd.Y, r);
            if (pStart == pEnd)
            {
                Logger.Error("..same coordinates x:{0}  y:{1}, nothing to do", pStart.X, pStart.Y);
                return;
            }
            double quarter = Math.PI / 2;
            double angle = GcodeMath.GetAlpha(pStart, pEnd);
            Point p1l = CalcOffsetPoint(pStart, angle + quarter, r);
            Point p1r = CalcOffsetPoint(pStart, angle - quarter, r);
            Point p2l = CalcOffsetPoint(pEnd, angle + quarter, r);
            Point p2r = CalcOffsetPoint(pEnd, angle - quarter, r);

            //Logger.Trace( " drawSlotLine pStart:x:{0:0.00} y:{1:0.00}  p1l:x:{2:0.00} y:{3:0.00}  p1r:x:{4:0.00} y:{5:0.00}  ",pStart.X,pStart.Y,p1l.X,p1l.Y,p1r.X,p1r.Y);

            Graphic.StopPath(); isPenDown = false;
            //   Graphic.StartPath(p1l);
            PenDown(p1l);
            Graphic.AddLine(p2l);
            Graphic.AddArc(true, p2r, Sub(pEnd, p2l));
            Graphic.AddLine(p1r);
            Graphic.AddArc(true, p1l, Sub(pStart, p1r));
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


        private static void ApplyApertureShape()
        {
            double tmpX, tmpY;//, diameter;
            Point startPos = ScalePosition(setX, setY);
            StartCoordinate = startPos;
            tmpX = actualAperture.XSize;
            tmpY = actualAperture.YSize;
            //   diameter = actualAperture.HoleDiameter;
            if (logDetailed) Logger.Trace("    Dot: applyApertureShape() {0}", actualAperture.apType.ToString());

            if (actualAperture.apType == Aperture.Type.Circle)
            {
                tmpX /= 2;
                Graphic.StartPath(ScalePosition(setX + tmpX, setY));
                Graphic.AddCircle(startPos.X, startPos.Y, ScaleValue(tmpX));
                Graphic.StopPath(); isPenDown = false;
            }
            else if (actualAperture.apType == Aperture.Type.Rectangle)
            {
                tmpX /= 2; tmpY /= 2;
                Graphic.StartPath(ScalePosition(setX - tmpX, setY - tmpY));
                Graphic.AddLine(ScalePosition(setX - tmpX, setY + tmpY));
                Graphic.AddLine(ScalePosition(setX + tmpX, setY + tmpY));
                Graphic.AddLine(ScalePosition(setX + tmpX, setY - tmpY));
                Graphic.AddLine(ScalePosition(setX - tmpX, setY - tmpY));
                Graphic.StopPath(); isPenDown = false;
            }
            else if (actualAperture.apType == Aperture.Type.Octagon)
            {
                tmpX /= 2; tmpY /= 2;
                Graphic.StartPath(ScalePosition(setX - tmpX, setY - tmpY));
                Graphic.AddLine(ScalePosition(setX - tmpX, setY + tmpY));
                Graphic.AddLine(ScalePosition(setX + tmpX, setY + tmpY));
                Graphic.AddLine(ScalePosition(setX + tmpX, setY - tmpY));
                Graphic.AddLine(ScalePosition(setX - tmpX, setY - tmpY));
                Graphic.StopPath(); isPenDown = false;
            }

        }


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


        class Aperture
        {
            public enum Type { none, Circle, Rectangle, Obround, Octagon, Polygon };
            public Type apType;
            public double XSize;
            public double YSize;
            //    public double HoleDiameter;
            public string content;

            public Aperture()
            {
                apType = Type.none;
                XSize = YSize = 0;
                //        HoleDiameter = 0;
                content = "";
            }

            public Aperture(string val)
            {
                content = val;
                XSize = 0; YSize = 0; //HoleDiameter = 0;
                if (val.Length > 1)
                {
                    string[] parts = val.Split(',');
                    if (parts.Length > 1)
                    {
                        string[] size = parts[1].Split('X');
                        XSize = YSize = GetNumber(size[0]);
                        if (parts[0] == "C")
                        {
                            apType = Type.Circle;
                            //    if (size.Length > 1)  HoleDiameter = GetNumber(size[1]);
                        }

                        if (size.Length > 1) YSize = GetNumber(size[1]);
                        //    if (size.Length > 2) HoleDiameter = GetNumber(size[2]);
                        if (parts[0] == "R") { apType = Type.Rectangle; }
                        if (parts[0] == "O") { apType = Type.Obround; }
                        if (parts[0] == "OC8") { apType = Type.Octagon; }
                        if (parts[0] == "P") { apType = Type.Polygon; }
                    }
                }
            }

            private static double GetNumber(string val)
            {
                //      double tmp;
                if (!double.TryParse(val, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out double tmp))
                { Logger.Error(" getNumber {0} ", val); }
                return tmp;
            }
        }
    }
}
