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

/* Level 1: import graphics SVG, DXF, HPGL, Drill, 
 *          extract objects, coordinates, colors, groups
 *
 * Level 2: plotterRelated: collect blocks, sort groups, code repition, tangential axis
 *
 * Level 3: gcodeRelated: implement Pen up/down options, avoid G23, cutter correction, write GCode commands 
*/

/*  GCodeFromDXF.cs a static class to convert HPGL data into G-Code 
        https://github.com/alexforencich/python-hpgl/tree/master/hpgl     
	    https://www.isoplotec.co.jp/HPGL/eHPGL.htm#-PD(Pen%20Down)
		PU PD uses last mode for PA, PR absolute, relative
*/

/*
 * 2020-02-18 Implementation
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    class GCodeFromHPGL
    {
        private static bool absoluteCoordinates = true;			// process absolute or relative coordinates
        private static Point position = new Point();		// actual absolute coordinate
        private static double factor = 1 / 40.00;				// factor between HPGL units and mm
        private static bool groupObjects = false;
        private static List<string> messageList = new List<string>();	// flag to remember if warning was sent
		private static bool penDown=false;
        private static Point startPathPos = new Point();
		

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Entrypoint for conversion: apply file-path 
        /// </summary>
        /// <param name="file">String keeping file-name</param>
        /// <returns>String with GCode of imported data</returns>
        public static string convertFromFile(string file)
        {
            Logger.Debug("Create GCode from {0}", file);
            if (file == "")
            { MessageBox.Show("Empty file name"); return ""; }

            if (file.Substring(0, 4) == "http")
            { MessageBox.Show("Load via http is not supported up to now"); }
            else
            { if (File.Exists(file))
                { try
                    { string HPGLCode = File.ReadAllText(file);     // get drill coordinates
                        return convertHPGL(HPGLCode, file);
                    }
                    catch (Exception e)
                    {   Logger.Error(e, "Error loading HPGL Code");
                        MessageBox.Show("Error '" + e.ToString() + "' in HPGL file " + file); return "";
                    }
                }
                else { MessageBox.Show("HPGL file does not exist: " + file); return ""; }
            }
            return "";
        }

        private static string convertHPGL(string hpglCode, string txt)
        {
            Logger.Debug("convertHPGL {0}", txt);
            absoluteCoordinates = true;
            position = new Point();
            groupObjects = Properties.Settings.Default.importGroupObjects;
            messageList.Clear();

            Plotter.StartCode();        	// initalize variables
            GetVectorHPGL(hpglCode);      	// convert graphics
            Plotter.SortCode();         	// sort objects
            return Plotter.FinalGCode("SVG import", txt);
        }

        private static void GetVectorHPGL(string htmlCode)
        {   string[] commands = htmlCode.Split(';');
            char[] charsToTrim = { ' ', '\r', '\n' };
            string line, cmd, parameter;
            foreach (string cmdline in commands)
            {
                line = cmdline.Trim(charsToTrim);
                if (line.Length >= 2)
                {
                    cmd = line.Substring(0, 2).ToUpper();
					parameter = line.Substring(2);
//                    Logger.Trace("=> {0} ", line);
                    if (cmd == "IN")
                    {   HPGL_IN();
                        if (parameter.Length >= 2)
                        {   cmd = parameter.Substring(0, 2).ToUpper();
                            parameter = cmd.Substring(2);
                        }
                        else
                            continue;
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
                    else if (cmd == "FS")
                        HPGL_FS(cmd,parameter);
                    else if (cmd == "VS")
                        HPGL_VS(cmd,parameter);
                    else if (cmd == "WU")
                        HPGL_WU(cmd,parameter);
                    else if (cmd == "PW")
                        HPGL_PW(cmd,parameter);
                    else
                    {   if (!messageList.Contains(cmd))
                        {   Logger.Warn(" UNKOWN command {0} ", cmd);
                            Plotter.AddToHeader(string.Format("Unknown command: {0}", cmd));
                            messageList.Add(cmd);
                        }
					}
                }
            }
        }

        private static void HPGL_IN()					// Initialize
        {   absoluteCoordinates = true;
            position = new Point();
			penDown=false;
        }
        private static void HPGL_SP(string nr)			// select Pen Nr
        {   if (nr.Length > 0)
            {   int pen = int.Parse(nr);
				Plotter.PathColor = pen.ToString();
                Plotter.SetGroup(pen);
                if (!groupObjects)
                    Plotter.ToolChange(pen, "Pen "+pen.ToString());   // add tool change commands (if enabled) and set XYFeed etc.
            }
            else
                Plotter.StopPath("");
        }
        private static void HPGL_PU(string coord)		// Pen up
        {   Plotter.PenUp();
   			penDown=false;
			startPathPos = moveTo(coord);				// get last Pen up position
        }
        private static void HPGL_PD(string coord)		// Pen down
        {   Plotter.PenDown("");
			if (!penDown)
				Plotter.StartPath(startPathPos,"");
			penDown=true;
            moveTo(coord);
        }
        private static void HPGL_PA(string coord)		// absolute positions
        {   absoluteCoordinates = true; moveTo(coord); }
        private static void HPGL_PR(string coord)		// relative positions
        {   absoluteCoordinates = false; moveTo(coord); }

        private static void HPGL_FS(string cmd, string coord)
        {   if (!messageList.Contains(cmd))
            {   Plotter.AddToHeader(string.Format("FS - 'Force Select' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_VS(string cmd, string coord)
        {   if (!messageList.Contains(cmd))
            {   Plotter.AddToHeader(string.Format("VS - 'Velocity Select' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_WU(string cmd, string coord)
        {   if (!messageList.Contains(cmd))
            {   Plotter.AddToHeader(string.Format("WU - 'pen Width Unit' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_PW(string cmd, string coord)
        {   if (!messageList.Contains(cmd))
            {   Plotter.AddToHeader(string.Format("PW - 'Pen Width' not supported"));
                messageList.Add(cmd);
            }
        }


        private static Point moveTo(string coord, bool rapid = false)
        {   if (coord.Length > 0)
            {
                char[] delimiters = new char[] { ',', ' ' };
                string[] coordinates = coord.Split(delimiters);
                Point tmpPoint = new Point();
                double[] floatArgs = coordinates.Select(arg => double.Parse(arg, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo)).ToArray();
                for (int i = 0; i < floatArgs.Length; i += 2)
                {
//                    Logger.Trace("=> {0} {1}", floatArgs[i], floatArgs[i+1]);
                    tmpPoint = new Point(floatArgs[i] * factor, floatArgs[i + 1] * factor);
                    if (absoluteCoordinates)
                        position = tmpPoint;
                    else
                    { position.X += tmpPoint.X; position.Y += tmpPoint.Y; }
					if (penDown)
						Plotter.MoveTo(position, "");
                }
				return position;
            }
			return startPathPos;		// no new value, return old value;
        }
    };






}