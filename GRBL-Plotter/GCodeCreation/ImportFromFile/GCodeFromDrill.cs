/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2018-2020 Sven Hasemann contact: svenhb@web.de

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
/*
 * 2019-08-15 add logger
 * 2020-06-19 add conversionInfo
 * 2020-12-08 add BackgroundWorker updates
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    class GCodeFromDrill
    {
        private static StringBuilder finalString = new StringBuilder();
        private static StringBuilder gcodeString = new StringBuilder();
        private static bool gcodeUseSpindle = false;            // Switch on/off spindle for Pen down/up (M3/M5)
        private static bool gcodeToolChange = false;            // Apply tool exchange command
        private static bool importComments = true;              // if true insert additional comments into GCode
        private static bool importUnitmm = true;                // convert units if needed

        private static string   infoDate = "unknown";
        private static bool     infoModeIsAbsolute = true;
        private static string   infoUnits = "Inch";
        private static double   infoFraction = 0.00001;         // default 1/100000
        private static string[] infoDrill = new string[20];

        public static string conversionInfo = "";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent  = null;


        /// <summary>
        /// Entrypoint for conversion: apply file-path 
        /// </summary>
        /// <param name="file">String keeping file-name</param>
        /// <returns>String with GCode of imported data</returns>
        public static string ConvertFromFile(string file, BackgroundWorker worker, DoWorkEventArgs e)
        {
            Logger.Info(" Create GCode from {0}",file);
            if (file == "")
            {   MessageBox.Show("Empty file name");
                return "";
            }

            backgroundWorker = worker;
            backgroundEvent  = e;

            gcode.setup();      // initialize GCode creation (get stored settings for export)
            gcodeToolChange = Properties.Settings.Default.importGCTool;         // Add tool change command
            importComments = Properties.Settings.Default.importSVGAddComments;
            importUnitmm  = Properties.Settings.Default.importUnitmm;
			conversionInfo = "";
			
            for (int i = 0; i < 20; i++)
                infoDrill[i] = "";

            if (file.Substring(0, 4) == "http")
            {
                MessageBox.Show("Load via http is not supported up to now");
            }
            else
            {
                string file_dri="", file_drd="";
                if (file.Substring(file.Length - 3, 3).ToLower() == "dri")      // build up filenames
                {
                    file_dri = file;
                    file_drd = file.Substring(0, file.Length - 3) + "drd";
                }
                else if (file.Substring(file.Length - 3, 3).ToLower() == "drd")      // build up filenames
                {
                    file_drd = file;
                    file_dri = file.Substring(0, file.Length - 3) + "dri";
                }
                else
                {   file_drd = file;        // KiCad drl
                    file_dri = "";
                }
/*                if (File.Exists(file_dri))              
                {   try
                    {   string[] drillInformation = File.ReadAllLines(file_dri);     // get drill information
                        GetDrillInfos(drillInformation);
                    }
                    catch (Exception e)
                    {   MessageBox.Show("Error '" + e.ToString() + "' in file " + file_dri); return ""; }
                }
                else {  conversionInfo += "Error: DRI-File not found ";
						MessageBox.Show("Drill information not found : " + file_dri + "\r\nTry to convert *.drd with default settings"); return ""; }
*/
                if (File.Exists(file_drd))              
                {   try
                    {   string[] drillCoordinates = File.ReadAllLines(file_drd);     // get drill coordinates
                        ConvertDrill(drillCoordinates, file_drd);
                    }
                    catch (Exception err)
                    {   MessageBox.Show("Error '" + err.ToString() + "' in file " + file_drd); return ""; }
                }
                else {  conversionInfo += "Error: DRD-File not found ";
						MessageBox.Show("Drill file does not exist: " + file_drd); return ""; }
            }

            string header = gcode.GetHeader("Drill import", file);
            string footer = gcode.GetFooter();
            gcodeUseSpindle = Properties.Settings.Default.importGCZEnable;

            finalString.Clear();

            if (gcodeUseSpindle && !gcodeToolChange) gcode.SpindleOn(finalString, "Start spindle - Option Z-Axis");

            finalString.Append(gcodeString);     
            if (gcodeUseSpindle) gcode.SpindleOff(finalString, "Stop spindle - Option Z-Axis");

            return header + finalString.ToString().Replace(',', '.') + footer;
        }

        private static void GetDrillInfos(string[] drillInfo)
        {
            foreach (string line in drillInfo)
            {
                string[] part = line.Split(':');
                if (part[0].IndexOf("Date") >= 0)       { infoDate = part[1].Trim(); }
                if (part[0].IndexOf("Data Mode") >= 0)
                {   infoModeIsAbsolute = (part[1].IndexOf("Absolute") >= 0) ? true : false;
                }
                if (part[0].IndexOf("Units") >= 0)
                {   }
                if (part[0].IndexOf("T") >= 0)
                {   }
            }
        }

        private static void ConvertDrill(string[] drillCode, string info)
        {
            Logger.Info(" ConvertDrill {0}", info);
            gcodeString.Clear();
            if (importComments)
            {
                gcodeString.AppendFormat("( Import Unit    : {0} )\r\n", infoUnits);
                gcodeString.AppendFormat("( Import Fraction: {0} )\r\n", infoFraction);
                gcodeString.Append("( Numbers exported to mm )\r\n");
            }
            gcode.PenUp(gcodeString, "Drill Start ");
            bool isHeader = false;
			int holeCount = 0;
			int drillCount = 0;
			int groupCount = 0;
            Dictionary<string, string> tNrToDiameter = new Dictionary<string, string>();
            string attDiameter = "";
            string xmlString = "";

            Logger.Info(" Amount Lines:{0}", drillCode.Length);
            if (backgroundWorker != null) backgroundWorker.ReportProgress(0, new MyUserState { Value = 10, Content = "Read DXF vector data of " + drillCode.Length.ToString() + " lines" });

            int lineNr = 0;
            foreach (string line in drillCode)
            {
                if (backgroundWorker != null) 
                {   backgroundWorker.ReportProgress(lineNr++ * 100 / drillCode.Length);
                    if (backgroundWorker.CancellationPending)
                    {   backgroundEvent.Cancel = true;
                        break;
                    } 
                }

                if (line.IndexOf("%") >= 0)
                {   isHeader = (isHeader)? false:true; }

                if (isHeader)
                {   if (importComments)
                        gcodeString.AppendLine("( " + line + " )");
                    if ((line.IndexOf("T") >= 0) && (line.IndexOf("C") >= 0))
                    {
                        string[] part = line.Split('C');
                        int tnr = 0;
						double dinch = 0;
                        double dmm = 0;
                        Int32.TryParse(part[0].Substring(1), out tnr);
                        Double.TryParse(part[1].Substring(0), out dinch);
                        dmm = dinch * 25.4;
                        infoDrill[tnr] = part[1] + " Inch = " + dmm.ToString() + " mm";
						gcode.AddToHeader(string.Format(" Tool-Nr.:{0} Diameter:{1:0.00} mm  ({2:0.000} inch)",tnr, dmm, dinch));
						Logger.Debug(" Tool-Nr.:{0} Diameter:{1:0.00} mm  ({2:0.000} inch)",tnr, dmm, dinch);
						drillCount++;
                        if (!tNrToDiameter.ContainsKey(part[0].Trim()))
                            tNrToDiameter.Add(part[0].Trim(), string.Format(" Diameter_mm=\"{0:0.00}\" Diameter_inch=\"{1:0.000}\"", dmm, dinch));
                    }
                }
                else
                {
                    if (line.IndexOf("T") >= 0)
                    {
                        gcodeString.AppendLine(" ");        // add empty line for better view
						if (groupCount != 0)
						{	gcodeString.AppendFormat("({0}>)\r\n", xmlMarker.groupEnd);
							Logger.Debug("({0}>)", xmlMarker.groupEnd);
						}

                        attDiameter = "";
                        if (tNrToDiameter.ContainsKey(line))
                            attDiameter = tNrToDiameter[line];

                        xmlString = string.Format("({0} Id=\"{1}\"{2}>)\r\n", xmlMarker.groupStart, line, attDiameter);
                        gcodeString.Append(xmlString);
						Logger.Debug("{0}", xmlString);
						groupCount++;

                        if (gcodeToolChange)
                        {   int tnr = 0;
                            Int32.TryParse(line.Substring(1), out tnr);
                            gcode.Tool(gcodeString, tnr, infoDrill[tnr]);
                        }
                        else
                        {   gcodeString.AppendLine("( " + line + " tool change not enabled)"); }
                    }

                    if ((line.IndexOf("X") >= 0) && (line.IndexOf("Y") >= 0))
                    {
                        string[] part = line.Split('Y');
                        double x = 0;
                        double y = 0;
                        double.TryParse(part[0].Substring(1), out x);
                        double.TryParse(part[1].Substring(0), out y);

                        x = x * infoFraction;
                        y = y * infoFraction;
                        if (importUnitmm)
                        {   x = x * 25.4;
                            y = y * 25.4;
                        }
                        string cmt = "";
                        if (importComments)
                            cmt = line;
                        gcode.MoveToRapid(gcodeString, (float)x, (float)y, cmt);
                        gcode.PenDown(gcodeString);
                        gcode.PenUp(gcodeString);
						holeCount++;
                    }
                }
            }
			gcodeString.AppendFormat("({0}>)\r\n", xmlMarker.groupEnd);

			conversionInfo += string.Format("Drills:{0} Holes:{1}",drillCount, holeCount);
        }
    }
}
