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

/*  GCodeFromHPGL.cs a static class to convert HPGL data into G-Code 
        https://github.com/alexforencich/python-hpgl/tree/master/hpgl     
	    https://www.isoplotec.co.jp/HPGL/eHPGL.htm#-PD(Pen%20Down)
		PU PD uses last mode for PA, PR absolute, relative
*/

/*
 * 2020-02-18 Implementation
 * 2020-05-26 Replace class Plotter by class Graphic for sorting
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
    public static class GCodeFromHPGL
    {
        private static bool absoluteCoordinates = true;			// process absolute or relative coordinates
        private static Point position = new Point();		// actual absolute coordinate
        private static double factor = 1 / 40.00;				// factor between HPGL units and mm
        private static bool groupObjects = false;
        private static List<string> messageList = new List<string>();	// flag to remember if warning was sent
		private static bool penDown=false;
        private static Point startPathPos = new Point();
		private static string[] defaultColor = new string[] {"white","black","red","green","blue","cyan","magenta","yellow"};

        public static string conversionInfo = "";
		private static int shapeCounter = 0;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
            { if (File.Exists(file))
                { try
                    { string HPGLCode = File.ReadAllText(file);     // get drill coordinates
                        return ConvertHPGL(HPGLCode, file);
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

        private static string ConvertHPGL(string hpglCode, string filePath)
        {
            Logger.Info(" convertHPGL {0}", filePath);
            absoluteCoordinates = true;
            position = new Point();
            groupObjects = Properties.Settings.Default.importGroupObjects;
			conversionInfo = "";
			shapeCounter = 0;

            messageList.Clear();

            Graphic.Init(Graphic.SourceTypes.HPGL, filePath); 
            GetVectorHPGL(hpglCode);      	                // convert graphics
			conversionInfo += string.Format("{0} elements imported", shapeCounter);
            return Graphic.CreateGCode(); 
        }

        private static void GetVectorHPGL(string hpglCode)
        {   string[] commands = hpglCode.Split(';');
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
                        //    Plotter.AddToHeader(string.Format("Unknown command: {0}", cmd));
							conversionInfo += string.Format("Error: Unknown command: {0} ", cmd);
							Graphic.SetHeaderInfo(string.Format("Unknown command: {0}", cmd));
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
				//Plotter.PathColor = pen.ToString();
                //Plotter.SetGroup(pen);
				Graphic.SetPenColorId(pen);
				if ((pen >= 0) && (pen < 8))
					Graphic.SetPenColor(defaultColor[pen]);
				else
					Graphic.SetPenColor(pen.ToString());

//                if (!groupObjects)
//                    Plotter.ToolChange(pen, "Pen "+pen.ToString());   // add tool change commands (if enabled) and set XYFeed etc.
            }
            else
                Graphic.StopPath();
        }
        private static void HPGL_PU(string coord)		// Pen up
        {   //Plotter.PenUp();
			Graphic.StopPath();
   			penDown=false;
			startPathPos = MoveTo(coord);				// get last Pen up position
        }
        private static void HPGL_PD(string coord)		// Pen down
        {   //Plotter.PenDown("");
			//Graphic.StartPath();
			if (!penDown)
				Graphic.StartPath(startPathPos);
			penDown=true;
            MoveTo(coord);
			shapeCounter++;
        }
        private static void HPGL_PA(string coord)		// absolute positions
        {   absoluteCoordinates = true; MoveTo(coord); }
        private static void HPGL_PR(string coord)		// relative positions
        {   absoluteCoordinates = false; MoveTo(coord); }

        private static void HPGL_FS(string cmd, string coord)
        {   if (!messageList.Contains(cmd))
            {   Graphic.SetHeaderInfo(string.Format("FS - 'Force Select' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_VS(string cmd, string coord)
        {   if (!messageList.Contains(cmd))
            {   Graphic.SetHeaderInfo(string.Format("VS - 'Velocity Select' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_WU(string cmd, string coord)
        {   if (!messageList.Contains(cmd))
            {   Graphic.SetHeaderInfo(string.Format("WU - 'pen Width Unit' not supported"));
                messageList.Add(cmd);
            }
        }
        private static void HPGL_PW(string cmd, string coord)
        {   //Graphic.SetPenWidth(cmd);
			if (!messageList.Contains(cmd))
            {   Graphic.SetHeaderInfo(string.Format("PW - 'Pen Width' not supported"));
                messageList.Add(cmd);
            }
        }


        private static Point MoveTo(string coord, bool rapid = false)
        {   if (coord.Length > 0)
            {
                char[] delimiters = new char[] { ',', ' ' };
                string[] coordinates = coord.Split(delimiters);
                Point tmpPoint = new Point();
                double[] floatArgs = coordinates.Select(arg => double.Parse(arg, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo)).ToArray();
                Graphic.SetGeometry("moveTo");
                for (int i = 0; i < floatArgs.Length; i += 2)
                {
//                    Logger.Trace("=> {0} {1}", floatArgs[i], floatArgs[i+1]);
                    tmpPoint = new Point(floatArgs[i] * factor, floatArgs[i + 1] * factor);
                    if (absoluteCoordinates)
                        position = tmpPoint;
                    else
                    { position.X += tmpPoint.X; position.Y += tmpPoint.Y; }
					if (penDown)
						Graphic.AddLine(position);		//Plotter.MoveTo(position, "");
                }
				return position;
            }
			return startPathPos;		// no new value, return old value;
        }
    };






}