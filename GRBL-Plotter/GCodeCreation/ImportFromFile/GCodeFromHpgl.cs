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

/*  GCodeFromHPGL.cs a static class to convert HPGL data into G-Code 
        https://github.com/alexforencich/python-hpgl/tree/master/hpgl     
	    https://www.isoplotec.co.jp/HPGL/eHPGL.htm#-PD(Pen%20Down)
		PU PD uses last mode for PA, PR absolute, relative
*/

/*
 * 2020-02-18 Implementation
 * 2020-05-26 Replace class Plotter by class Graphic for sorting
 * 2020-12-08 add BackgroundWorker updates
 * 2021-07-31 code clean up / code quality
 * 2023-07-06 l:288 f:MoveTo	improove parsing coord-token, add text support
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace GrblPlotter
{
    public static class GCodeFromHpgl
    {
        private static bool absoluteCoordinates = true;			// process absolute or relative coordinates
        private static Point position = new Point();		// actual absolute coordinate
        private const double factor = 1 / 40.00;                // factor between HPGL units and mm
                                                                //    private static bool groupObjects = false;
        private static readonly List<string> messageList = new List<string>();  // flag to remember if warning was sent
        private static bool penDown = false;
        private static Point lastPosition = new Point();
        private static readonly string[] defaultColor = new string[] { "white", "black", "red", "green", "blue", "cyan", "magenta", "yellow" };
        private static int penColor = 1;

        public static string ConversionInfo { get; set; }
        private static int shapeCounter = 0;
        private static bool logEnable = true;
        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent = null;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool ConvertFromText(string text)
        {
            return ConvertHPGL(text, "from Clipboard");
        }
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
                        string HPGLCode = File.ReadAllText(file);     // get drill coordinates
                        return ConvertHPGL(HPGLCode, file);
                    }
                    catch (Exception err)
                    {
                        Logger.Error(err, "Error loading HPGL Code");
                        MessageBox.Show("Error '" + err.ToString() + "' in HPGL file " + file);// throw;
                    }
                }
                else { MessageBox.Show("HPGL file does not exist: " + file); return false; }
            }
            return false;
        }

        private static bool ConvertHPGL(string hpglCode, string filePath)
        {
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);

            Logger.Info(" convertHPGL {0}", filePath);
            absoluteCoordinates = true;
            penDown = false;
            charAngle = 0;

            position = new Point();
            lastPosition = new Point();

            ConversionInfo = "";
            shapeCounter = 0;

            messageList.Clear();

            Graphic.Init(Graphic.SourceType.HPGL, filePath, backgroundWorker, backgroundEvent);
            GetVectorHPGL(hpglCode);                        // convert graphics
            ConversionInfo += string.Format("{0} elements imported", shapeCounter);
            return Graphic.CreateGCode();
        }

        private static void GetVectorHPGL(string hpglCode)
        {
            string[] commands = hpglCode.Split(';');
            char[] charsToTrim = { ' ', '\r', '\n' };
            string line, cmd, parameter;

            Logger.Info(" Amount Lines:{0}  ", commands.Length);
            if (backgroundWorker != null) backgroundWorker.ReportProgress(0, new MyUserState { Value = 10, Content = "Read HPGL vector data of " + commands.Length.ToString() + " lines" });

            int lineNr = 0;
            foreach (string cmdline in commands)
            {

                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress(lineNr++ * 100 / commands.Length);
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundEvent.Cancel = true;
                        break;
                    }
                }

                line = cmdline.Trim(charsToTrim);
                if (line.Length >= 2)
                {
                    cmd = line.Substring(0, 2).ToUpper();
                    parameter = line.Substring(2);
                    if (cmd == "IN")
                    {
                        HPGL_IN();
                        if (parameter.Length >= 2)
                        {
                            cmd = parameter.Substring(0, 2).ToUpper();
                            parameter = cmd.Substring(2);
                        }
                        else
                            continue;
                    }

                    if (cmd == "LB")	// Label
                    {
                        string[] cmd2 = parameter.Split('\n');
                        HPGL_LB(cmd2[0].Trim('\r'));            // output text
                        if (cmd2.Length > 1)
                        {
                            cmd = cmd2[1].Trim();
                            if (cmd2[1].Length > 2)
                                cmd = cmd2[1].Trim().Substring(0, 2).ToUpper();
                            if (cmd2[1].Length > 2)
                                parameter = cmd2[1].Trim().Substring(2);
                            else
                                parameter = "";

                        }
                        if (logEnable) Logger.Trace("split cmd:'{0}'  parameter:{1}", cmd, parameter);
                    }

                    if (cmd == "SP")
                        HPGL_SP(parameter);
                    else if (cmd == "PU")
                        HPGL_PU(parameter);
                    else if (cmd == "PD")
                        HPGL_PD(parameter);
                    else if (cmd == "PA")
                        HPGL_PA(parameter);
                    else if (cmd == "PR")
                        HPGL_PR(parameter);
                    else if (cmd == "LT")
                        HPGL_LT(parameter);	// Line Type

                    else if (cmd == "AA")	// Arc absolute
                        HPGL_AA(parameter);
                    else if (cmd == "AR")	// Arc relative
                        HPGL_AR(parameter);
                    else if (cmd == "CI")	// circle
                        HPGL_CI(parameter);

                    else if (cmd == "SI")	// Absolute Character size
                        HPGL_SI(parameter);
                    else if (cmd == "DR")	// Absolute Character size
                    {
                        if (parameter.Length == 0)
                            charAngle = 0;
                        else
                            HPGL_DR(parameter);
                    }
                    else if (cmd == "EA")
                        HPGL_EA(parameter);
                    else if (cmd == "FS")
                        HPGL_FS(cmd);//,parameter);
                    else if (cmd == "VS")
                        HPGL_VS(cmd);//,parameter);
                    else if (cmd == "WU")
                        HPGL_WU(cmd);//,parameter);
                    else if (cmd == "PW")
                        HPGL_PW(cmd);//,parameter);
                    else
                    {
                        if ((cmd.Length > 1) && !messageList.Contains(cmd))
                        {
                            Logger.Warn(" UNKOWN command {0} ", cmd);
                            ConversionInfo += string.Format("Error: Unknown command: {0} ", cmd);
                            Graphic.SetHeaderInfo(string.Format(" Unknown HPGL command: {0}", cmd));
                            Graphic.SetHeaderMessage(string.Format(" {0}-1301: Unknown HPGL command: {1}", CodeMessage.Attention, cmd));
                            messageList.Add(cmd);
                        }
                    }
                }
            }
        }

        private static void HPGL_IN()					// Initialize
        {
            absoluteCoordinates = true;
            position = new Point();
            lastPosition = new Point();
            penDown = false;
            charAngle = 0;
        }
        private static void HPGL_SP(string nr)			// select Pen Nr
        {
            if (nr.Length > 0)
            {
                //  int pen = 0;
                if (!int.TryParse(nr, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out int pen))
                {
                    Logger.Error("Error parsing int {0}", nr);
                    penColor = 1;
                }
                penColor = pen;

                if (logEnable) Logger.Trace("Set pen:{0}", penColor);
                Graphic.SetPenColorId(penColor);
                if ((penColor >= 0) && (penColor < 8))
                    Graphic.SetPenColor(defaultColor[penColor]);
                else
                    Graphic.SetPenColor(penColor.ToString());
            }
        }
        private static void HPGL_PU(string coord)		// Pen up
        {
            Graphic.StopPath();
            penDown = false;
            if (logEnable) Logger.Trace("PU {0} {1}", lastPosition.X, lastPosition.Y);
        }
        private static void HPGL_PD(string coord)		// Pen down
        {
            if (!penDown)
                Graphic.StartPath(lastPosition);
            penDown = true;
            if (logEnable) Logger.Trace("PD {0} {1}", lastPosition.X, lastPosition.Y);
            MoveTo(coord);				// get last Pen down position
            shapeCounter++;
        }
        private static void HPGL_PA(string coord)		// absolute positions
        { absoluteCoordinates = true; MoveTo(coord); }
        private static void HPGL_PR(string coord)		// relative positions
        { absoluteCoordinates = false; MoveTo(coord); }

        private static void HPGL_AA(string coord)
        {
            HPGL_Arc(coord, false);
        }
        private static void HPGL_AR(string coord)
        {
            HPGL_Arc(coord, true);
        }

        private static void HPGL_Arc(string coord, bool relative)
        {
            double[] floatArgs = ConvertArgs(coord);    // cx,cy, angle, optional-resolution
            if (floatArgs.Length > 2)
            {
                double cx = floatArgs[0] * factor;
                double cy = floatArgs[1] * factor;
                if (relative)
                {
                    cx += lastPosition.X;
                    cy += lastPosition.Y;
                }

                double ai = cx - lastPosition.X;
                double aj = cy - lastPosition.Y;
                double r = Math.Sqrt(ai * ai + aj * aj);

                double cos1 = ai / r;
                if (cos1 > 1) cos1 = 1;
                if (cos1 < -1) cos1 = -1;
                double angleStart = Math.PI - Math.Acos(cos1);
                if (aj > 0) { angleStart = -angleStart; }
                double angleEnd = angleStart + (Math.PI * floatArgs[2] / 180);

                double px = cx + r * Math.Cos(angleEnd);
                double py = cy + r * Math.Sin(angleEnd);

                if (logEnable) Logger.Trace("Arc start:{0}  {1}  center:{2}  {3}   r:{4}  a:{5}  end:{6}  {7}", lastPosition.X, lastPosition.Y, cx, cy, r, 180 * angleEnd / Math.PI, px, py);
                Graphic.AddArc((floatArgs[2] < 0), px, py, ai, aj);	// (bool isg2, double ax, double ay, double ai, double aj)

                lastPosition.X = px;
                lastPosition.Y = py;
            }
        }

        private static void HPGL_CI(string coord)
        {
            double[] floatArgs = ConvertArgs(coord);    // radius
            if (floatArgs.Length > 0)
            {
                double r = floatArgs[0] * factor;
                if (logEnable) Logger.Trace("Circle center:{0} {1}   r:{2}", position.X, position.Y, r);
                Graphic.StartPath((float)(lastPosition.X + r), (float)(lastPosition.Y));
                Graphic.AddCircle(lastPosition.X, lastPosition.Y, floatArgs[0] * factor);    // (double centerX, double centerY, double radius)
                Graphic.StopPath();
            }
        }

        private static void HPGL_DR(string arg)
        {
            double[] floatArgs = ConvertArgs(arg);
            if (floatArgs.Length > 1)
            {
                if (floatArgs[0] == 0)
                    charAngle = Math.PI / 2 * Math.Sign(floatArgs[1]);
                else if (floatArgs[1] == 0)
                    charAngle = Math.PI * Math.Sign(floatArgs[0]);
                else
                    charAngle = Math.Atan(floatArgs[0] / floatArgs[1]);
            }
        }

        private static void HPGL_LB(string text)
        {
            if (logEnable) Logger.Trace("Set Label:'{0}'", text);
            GCodeFromFont.Reset();
            GCodeFromFont.GCText = text;
            GCodeFromFont.GCHeight = charHeight;
            GCodeFromFont.GCWidth = charWidth;
            GCodeFromFont.GCOffX = lastPosition.X;
            GCodeFromFont.GCOffY = lastPosition.Y;
            GCodeFromFont.GCAngleRad = charAngle;
            //    GCodeFromFont.GCFontName += charFont + ".lff";
            GCodeFromFont.GetCode(0);   // no page break
        }

        private static double charWidth = 2.85;
        private static double charHeight = 3.75;
        private static double charAngle = 0;
        private static string charFont = "standard";		// default GCFontName = "lff\\standard.lff";
        private static void HPGL_SI(string coord)
        {
            double[] floatArgs = ConvertArgs(coord);
            if (floatArgs.Length > 1)
            {
                charWidth = floatArgs[0] * 10;
                charHeight = floatArgs[1] * 10;
            }
        }

        private static void HPGL_LT(string index)       // Line Type
        {
            if (index.Length == 0)
                Graphic.SetDash(new double[0]);

            double[] floatArgs = ConvertArgs(index);
            if (floatArgs.Length > 0)
            {
                if ((int)floatArgs[0] == 1) Graphic.SetDash(new double[] { 1, 10 });
                if ((int)floatArgs[0] == 2) Graphic.SetDash(new double[] { 5, 5 });
                if ((int)floatArgs[0] == 3) Graphic.SetDash(new double[] { 8, 2 });
                if ((int)floatArgs[0] == 4) Graphic.SetDash(new double[] { 8, 1, 1, 1 });
                if ((int)floatArgs[0] == 5) Graphic.SetDash(new double[] { 8, 1, 2, 1 });
                if ((int)floatArgs[0] == 6) Graphic.SetDash(new double[] { 8, 2, 2, 2, 2, 2 });
                if ((int)floatArgs[0] == 7) Graphic.SetDash(new double[] { 1, 10 });
            }
        }

        private static void HPGL_EA(string coord)		// relative positions
        {
            double[] floatArgs = ConvertArgs(coord);
            if (floatArgs.Length > 1)
            {
                double x = floatArgs[0] * factor;
                double y = floatArgs[1] * factor;
                if (logEnable) Logger.Trace("Rect {0} {1}   dx:{2}  dy:{3}", position.X, position.Y, x, y);
                Graphic.StartPath(lastPosition);
                Graphic.AddLine((float)(lastPosition.X), (float)(y));
                Graphic.AddLine((float)(x), (float)(y));
                Graphic.AddLine((float)(x), (float)(lastPosition.Y));
                Graphic.AddLine((float)(lastPosition.X), (float)(lastPosition.Y));
                Graphic.StopPath();
            }
        }
        private static void HPGL_FS(string cmd)//, string coord)
        {
            if (!messageList.Contains(cmd))
            {
                Graphic.SetHeaderInfo(string.Format("FS - 'Force Select' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_VS(string cmd)//, string coord)
        {
            if (!messageList.Contains(cmd))
            {
                Graphic.SetHeaderInfo(string.Format("VS - 'Velocity Select' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_WU(string cmd)//, string coord)
        {
            if (!messageList.Contains(cmd))
            {
                Graphic.SetHeaderInfo(string.Format("WU - 'pen Width Unit' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_PW(string cmd)//, string coord)
        {
            if (!messageList.Contains(cmd))
            {
                Graphic.SetHeaderInfo(string.Format("PW - 'Pen Width' not supported"));
                messageList.Add(cmd);
            }
        }


        private static Point MoveTo(string coord)
        {
            if (coord.Length > 0)
            {
                Point tmpPoint = new Point();
                double[] floatArgs = ConvertArgs(coord); // floatArgs = coordinates.Select(arg => double.Parse(arg.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.NumberFormatInfo.InvariantInfo)).ToArray();
                Graphic.SetGeometry("moveTo");
                for (int i = 0; i < floatArgs.Length; i += 2)
                {
                    tmpPoint = new Point(floatArgs[i] * factor, floatArgs[i + 1] * factor);
                    if (absoluteCoordinates)
                        position = tmpPoint;
                    else
                    { position.X = lastPosition.X + tmpPoint.X; position.Y = lastPosition.Y + tmpPoint.Y; }
                    if (penDown)
                        Graphic.AddLine(position);
                    if (logEnable) Logger.Trace("MoveToOrig: {0} {1}  MoveToCode: {2} {3} abs?:{4}", floatArgs[i], floatArgs[i + 1], position.X, position.Y, absoluteCoordinates);

                    lastPosition = position;
                }
                return position;
            }
            return lastPosition;		// no new value, return old value;
        }

        private static double[] ConvertArgs(string coord)
        {
            if (coord.Length > 0)
            {
                char delimiter = ' ';
                if (coord.IndexOf(",") != -1) { delimiter = ','; }

                string[] coordinates = coord.Trim().Split(delimiter);
                Point tmpPoint = new Point();

                double[] floatArgs = coordinates.Select(arg => double.Parse(arg.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.NumberFormatInfo.InvariantInfo)).ToArray();
                return floatArgs;
            }
            else
                return new double[0];
        }
    }
}