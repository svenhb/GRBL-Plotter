/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

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

/*  GCodeFromCSV.cs a static class to convert CSV data into G-Code 
    GCode will be written to gcodeString[gcodeStringIndex] where gcodeStringIndex corresponds with color of element to draw
*/
/* 2020-06-03 new implementation
 * 2020-12-08 add BackgroundWorker updates
 * 2021-07-31 code clean up / code quality
*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

//#pragma warning disable CA1303	// Do not pass literals as localized parameters

namespace GrblPlotter
{
    public static class GCodeFromCsv
    {
        private static bool tryAutomatic = true;
        private static char delimeter = ';';
        private static readonly char[] deliSelect = new char[] { ';', '\t', ' ', ':', '|', ',' };
        private static int startAtLine = 0;
        private static int columnX = 0;
        private static int columnY = 1;
        private static int columnZ = 2;
        private static double scaleX = 1;
        private static double scaleY = 1;
        private static double scaleZ = 1;
        private static bool continuousX = false;    // x will be assumed as index (1,2,3,..) with scaleX
        private static bool processZ = false;
        private static bool processAsLine = true;   // or as dot
        private static Dimensions dimension = new Dimensions();

        public static string ConversionInfo { get; set; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent = null;

        /// <summary>
        /// Entrypoint for conversion: apply file-path 
        /// </summary>
        /// <param name="filePath">String keeping file-name</param>
        /// <returns>String with GCode of imported data</returns>
        public static bool ConvertFromFile(string filePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            Logger.Info(" Create GCode from {0}", filePath);

            ConversionInfo = "";
            backgroundWorker = worker;
            backgroundEvent = e;

            if (String.IsNullOrEmpty(filePath))
            { MessageBox.Show("Empty file name"); return false; }
            else if (filePath.Substring(0, 4) == "http")
            { MessageBox.Show("Load via http is not supported up to now"); }
            else
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        string[] CSVCode = File.ReadAllLines(filePath);
                        return ConvertCSV(CSVCode, filePath);
                    }
                    catch //(Exception err)
                    {
                        Logger.Error("Error loading CSV Code");
                        //MessageBox.Show("Error '" + e.ToString() + "' in CSV file " + filePath); return false;throw; 
                        MessageBox.Show("Error in CSV file " + filePath);  //throw;
                    }
                }
                else { MessageBox.Show("CSV file does not exist: " + filePath); return false; }
            }
            return false;
        }

        private static bool ConvertCSV(string[] csvCode, string filePath)
        {
            Graphic.Init(Graphic.SourceType.CSV, filePath, backgroundWorker, backgroundEvent);
            Logger.Info(" convertCSV {0}", filePath);
            dimension = new Dimensions();
            int size = csvCode.Length;
            double csvX, csvY, csvZ = 0;
            string[] splitSample;
            ConversionInfo = string.Format("CSV Import: Lines:{0} ", size);

            tryAutomatic = Properties.Settings.Default.importCSVAutomatic;
            if (tryAutomatic)
            {   // get probe from middle of data range
                int sampleLine = size / 2;
                string sample = csvCode[sampleLine].Trim();
                int columns = 0;
                columnX = 0;
                columnY = 1;
                processZ = false;
                if (sample.Length >= 3)     // min. expected length: 2 numbers, 1 delimeter
                {
                    for (int i = 0; i < deliSelect.Length; i++)
                    {
                        if (sample.Contains(deliSelect[i]))
                        {
                            delimeter = deliSelect[i];
                            break;
                        }
                    }
                    splitSample = sample.Split(delimeter);
                    columns = splitSample.Length;
                }
                Logger.Info(" Automatic, found '{0}', columns:{1} in sample '{2}', line {3}", delimeter, columns, sample, sampleLine);

                if (columns <= 1)
                {	//string errorTxt = string.Format("(Automatic CSV import failed!)\r\n(Delimeter '{0}' in sample '{1}' from line {2})",delimeter,sample, sampleLine);
                    return false;// errorTxt;					
                }
                columnY = columns - 1;
                columnX = columns - 2;

                Graphic.SetHeaderInfo(string.Format(" Automatic: delimeter:'{0}', index-X:{1} , index-X:{2} ", delimeter, columnX, columnY));
                Graphic.SetHeaderInfo(string.Format(" Sample Line:{0} {1} ", (sampleLine + 1), sample));

                for (int i = 0; i < sampleLine; i++)    // find first CSV line
                {
                    splitSample = csvCode[i].Trim().Split(delimeter);
                    if (splitSample.Length == columns)
                    {
                        if (double.TryParse(splitSample[columnX], out _) && double.TryParse(splitSample[columnY], out _))
                        {   // seems to be regualr data
                            startAtLine = i;
                            break;
                        }
                        else
                            Graphic.SetHeaderInfo(string.Format(" CSV Line:{0} {1} ", (i + 1), csvCode[i]));
                    }
                    else if (csvCode[i].Length > 1)
                        Graphic.SetHeaderInfo(string.Format(" CSV Line:{0} {1} ", (i + 1), csvCode[i]));
                }
                ConversionInfo += string.Format("Automatic: delimeter:'{0}', column-X:{1}, -Y:{2}, 1st data line:{3}.  ", delimeter, columnX, columnY, startAtLine);
            }
            else        // get defaults
            {
                delimeter = Properties.Settings.Default.importCSVDelimeter[0];
                startAtLine = (int)Properties.Settings.Default.importCSVStartLine;
                columnX = (int)Properties.Settings.Default.importCSVColumnX;
                columnY = (int)Properties.Settings.Default.importCSVColumnY;
                columnZ = (int)Properties.Settings.Default.importCSVColumnZ;
                scaleX = (double)Properties.Settings.Default.importCSVScaleX;
                scaleY = (double)Properties.Settings.Default.importCSVScaleY;
                scaleZ = (double)Properties.Settings.Default.importCSVScaleZ;
                processZ = Properties.Settings.Default.importCSVProzessZ;
                processAsLine = Properties.Settings.Default.importCSVProzessAsLine;
                continuousX = false;
            }

            int maxCol = Math.Max(columnX, columnY);
            if (processZ)
                maxCol = Math.Max(maxCol, columnZ);

            bool lastOk = false;
            bool xOk = true;
            bool zOk = true;
            int blockCount = 0;
            int indexX = 0;

            Logger.Info(" Amount Lines:{0}", size);
            if (backgroundWorker != null) backgroundWorker.ReportProgress(0, new MyUserState { Value = 10, Content = "Read CSV data of " + size.ToString() + " lines" });

            for (int i = startAtLine; i < size; i++)
            {
                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress(i * 100 / size);
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundEvent.Cancel = true;
                        break;
                    }
                }

                splitSample = csvCode[i].Trim().Split(delimeter);
                if (splitSample.Length > maxCol)
                {
                    if (!continuousX)
                        xOk = double.TryParse(splitSample[columnX], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out csvX);
                    else
                        csvX = indexX++;

                    if (processZ)
                        zOk = double.TryParse(splitSample[columnZ], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out csvZ);

                    if (zOk && xOk && double.TryParse(splitSample[columnY], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out csvY))
                    {
                        csvX *= scaleX;
                        csvY *= scaleY;
                        csvZ *= scaleZ;
                        dimension.SetDimensionXY(csvX, csvY);

                        if (!lastOk)
                        {
                            Graphic.StopPath();
                            Graphic.SetLayer(blockCount.ToString());
                            if (processAsLine)
                            {
                                Graphic.SetGeometry("Line");
                                Graphic.StartPath(new Point(csvX, csvY));
                            }
                            else
                            {
                                Graphic.SetGeometry("Point");
                                if (processZ)
                                    Graphic.AddDotWithZ(csvX, csvY, csvZ);
                                else
                                    Graphic.AddDot(csvX, csvY);
                            }
                        }
                        else
                        {
                            if (processAsLine)
                            {   //Graphic.SetGeometry("Line");
                                Graphic.AddLine(new Point(csvX, csvY));
                            }
                            else
                            {   //Graphic.SetGeometry("Point");
                                if (processZ)
                                    Graphic.AddDotWithZ(csvX, csvY, csvZ);
                                else
                                    Graphic.AddDot(csvX, csvY);
                            }
                        }
                        lastOk = true;
                    }
                    else
                        lastOk = false;
                }
                else
                    lastOk = false;
            }
            Graphic.StopPath();
            if (tryAutomatic)
            {
                double sx = 1, sy = 1;
                if (dimension.dimx < 3)
                    sx = 100;
                if (dimension.dimy < 3)
                    sy = 100;
                ConversionInfo += string.Format("Apply scale-X:{0}, -Y:{1}", sx, sy);
                Logger.Info(" Automatic scale dimX:{0} scaleX:{1}, dimY:{2}, scaleY:{3}", dimension.dimx, sx, dimension.dimy, sy);
                Graphic.ScaleXY(sx, sy);
            }
            return Graphic.CreateGCode();
        }
    }
}