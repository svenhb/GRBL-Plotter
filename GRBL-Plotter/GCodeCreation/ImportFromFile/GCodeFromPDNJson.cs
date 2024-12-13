/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2024-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2024-08-07 Implementation for https://github.com/sbtrn-devil/pdn-json
 * 2024-08-20 option to find white background
 * 2024-09-20 add paste from clipboard
 * 2024-10-08 support PoTrace with different DPIs
*/

using CsPotrace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace GrblPlotter
{
    public static class GCodeFromPDNJson
    {
        public static string ConversionInfo { get; set; }
        private static string penWidth = "0.5";

        private static int shapeCounter = 0;
        private static bool logEnable = true;
        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent = null;
        private static List<List<Point>> outlineList;
        private static PJSFile pjsFile = null;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void LoadFromClipboard()									// called from MainFormLoadFile 1383
        {
            try
            {
                IDataObject iData = Clipboard.GetDataObject();
                Logger.Info("▼▼▼▼  ConvertBitmap pasteFromClipboard");
                if ((iData != null) && (iData.GetDataPresent(DataFormats.Bitmap)))
                {
                    Bitmap image = new Bitmap(Clipboard.GetImage());
                    Graphic.Init(Graphic.SourceType.PDNJson, "from Clipboard", backgroundWorker, backgroundEvent);
                    bool usePoTrace = Properties.Settings.Default.importVectorizeAlgorithmPoTrace;

                    Graphic.SetPenWidth(penWidth);
                    Graphic.SetGeometry(Path.GetExtension("clipboard").ToLower().Replace(".", ""));
                    Logger.Info("▼▼▼▼  ConvertBitmap pasteFromClipboard  use PoTrace:{0}", usePoTrace);
                    if (usePoTrace)
                    {
                        DoPoTrace(image);
                    }
                    else
                    {
                        DoMyTrace(image);
                    }

                    ConversionInfo += string.Format("{0} elements imported", shapeCounter);

                    Logger.Info("▲▲▲▲  ConvertPDNJson Finish: shapeCounter: {0} ", shapeCounter);
                    Graphic.CreateGCode();
                }
            }
            catch (Exception err)
            { Logger.Error(err, "LoadClipboard "); }
        }

        public static bool ConvertFromFile(string filePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            backgroundWorker = worker;
            backgroundEvent = e;
            String ext = Path.GetExtension(filePath).ToLower();
            bool isPdnJson = (ext == ".pdn-json");

            if (String.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Empty file name");
                return false;
            }
            else if (filePath.Substring(0, 4) == "http")
            {
                string content = "";
                using (var wc = new System.Net.WebClient())
                {
                    try { content = wc.DownloadString(filePath); }
                    catch { MessageBox.Show("Could not load content from " + filePath); }
                }

                if (!String.IsNullOrEmpty(content))
                {
                    if (!isPdnJson)
                    { return ConvertBitmap(filePath); }
                    else
                    {
                        try
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PJSFile));
                            pjsFile = null;
                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(content);
                            using (var fs = new MemoryStream(byteArray))
                            {
                                pjsFile = (PJSFile)ser.ReadObject(fs);
                            }
                            if (pjsFile != null)
                            {
                                return ConvertBitmaps(filePath);
                            }
                        }
                        catch (Exception err)
                        {
                            Logger.Error(err, "Error loading PDN-Json Code from {0} ", filePath);
                            MessageBox.Show("Error '" + err.ToString() + "' in PDN-Json file " + filePath);// throw;
                        }
                    }
                }
            }
            else
            {
                if (File.Exists(filePath))
                {
                    if (!isPdnJson)
                    { return ConvertBitmap(filePath); }
                    else
                    {
                        try
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PJSFile));
                            pjsFile = null;
                            using (var fs = new FileStream(filePath, FileMode.Open))
                            {
                                pjsFile = (PJSFile)ser.ReadObject(fs);
                            }
                            if (pjsFile != null)
                            {
                                return ConvertBitmaps(filePath);
                            }
                        }
                        catch (Exception err)
                        {
                            Logger.Error(err, "Error loading PDN-Json Code");
                            MessageBox.Show("Error '" + err.ToString() + "' in PDN-Json file " + filePath);// throw;
                        }
                    }
                }
                else { MessageBox.Show("PDN-Json file does not exist: " + filePath); return false; }
            }
            return false;
        }

        // Vectorize layered bitmaps from pjsFile.layers
        private static bool ConvertBitmaps(string filePath)
        {
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
            bool usePoTrace = Properties.Settings.Default.importVectorizeAlgorithmPoTrace;

            Logger.Info("▼▼▼▼  ConvertBitmaps Start {0}", filePath);
            Logger.Trace("►►►► pjsFile width:{0}  height:{1}  layers:{2}", pjsFile.width, pjsFile.height, pjsFile.layers.Count);

            ConversionInfo = "";
            shapeCounter = 0;

            Graphic.Init(Graphic.SourceType.PDNJson, filePath, backgroundWorker, backgroundEvent);

            bool showAllLayers = !Properties.Settings.Default.importPDNLayerVisible;

            Graphic.SetPenWidth(penWidth);
            for (int i = 0; i < pjsFile.layers.Count; i++)
            {
                Logger.Trace("►►► layer:{0}  width:{1}  height:{2}  visible:{3}  name:{4}", i, pjsFile.layers[i].width, pjsFile.layers[i].height, pjsFile.layers[i].visible, pjsFile.layers[i].name);
                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress(i * 100 / pjsFile.layers.Count);
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundEvent.Cancel = true;
                        break;
                    }
                }
                if (showAllLayers || pjsFile.layers[i].visible)
                {
                    Graphic.SetLayer(i.ToString());
                    Graphic.SetGeometry(i.ToString());
                    if (!string.IsNullOrEmpty(pjsFile.layers[i].base64))
                    {
                        shapeCounter++;
                        Bitmap image;
                        byte[] bytes = Convert.FromBase64String(pjsFile.layers[i].base64);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = (Bitmap)System.Drawing.Image.FromStream(ms);
                        }

                        if (usePoTrace)
                        {
                            DoPoTrace(image);
                        }
                        else
                        {
                            DoMyTrace(image);
                        }
                    }
                }
                else
                {
                    Graphic.SetHeaderInfo(string.Format(" Hide PDN Layer:{0}   ", pjsFile.layers[i].name));
                }
            }
            ConversionInfo += string.Format("{0} elements imported", shapeCounter);

            Logger.Info("▲▲▲▲  ConvertPDNJson Finish: shapeCounter: {0} ", shapeCounter);
            return Graphic.CreateGCode();
        }

        // Vectorize single bitmap
        private static bool ConvertBitmap(string filePath)
        {
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
            bool usePoTrace = Properties.Settings.Default.importVectorizeAlgorithmPoTrace;

            ConversionInfo = "";
            shapeCounter = 0;

            Logger.Info("▼▼▼▼  ConvertBitmap Start  use PoTrace:{0}  path:{1}", usePoTrace, filePath);

            Graphic.Init(Graphic.SourceType.PDNJson, filePath, backgroundWorker, backgroundEvent);
            Graphic.SetPenWidth(penWidth);
            Graphic.SetGeometry(Path.GetExtension(filePath).ToLower().Replace(".", ""));

            Bitmap image;
            image = (Bitmap)System.Drawing.Image.FromFile(filePath);

            if (usePoTrace)
            {
                DoPoTrace(image);
            }
            else
            {
                DoMyTrace(image);
            }
            image.Dispose();

            ConversionInfo += string.Format("{0} elements imported", shapeCounter);

            Logger.Info("▲▲▲▲  ConvertPDNJson Finish: shapeCounter: {0} ", shapeCounter);
            return Graphic.CreateGCode();
        }

        private static void DoPoTrace(Bitmap image)
        {
            bool findTransparency = Properties.Settings.Default.importVectorizeDetectTransparency;
            short greyThreshold = (short)Properties.Settings.Default.importVectorizeThreshold;

            string logString = " DPI set to:";
            double resoPxMm = (double)Properties.Settings.Default.importPDNDpi;
            if (Properties.Settings.Default.importVectorizeDpiFromImage)
            { resoPxMm = image.HorizontalResolution; logString = " DPI from image:"; }
            logString += string.Format(" {0:0.0}", resoPxMm);
            resoPxMm /= 25.4;

            if (Properties.Settings.Default.importVectorizeSetWidthOfImage)
            {
                resoPxMm = image.Width / (double)Properties.Settings.Default.importPDNWidth;
                logString = string.Format(" DPI from given width: {0:0.0}", Properties.Settings.Default.importPDNWidth);
            }

            Graphic.SetPenColor("black");
            /* use of PoTrace https://potrace.sourceforge.net/potrace.pdf
                https://potrace.sourceforge.net/potracelib.pdf */

            Potrace.turdsize = (int)Properties.Settings.Default.importVectorizePoTraceTurdsize;//  2;
            Potrace.alphamax = (double)Properties.Settings.Default.importVectorizePoTraceAlphamax;//  1;
            Potrace.opttolerance = (double)Properties.Settings.Default.importVectorizePoTraceOpttolerance;  //0.2;
            Potrace.curveoptimizing = Properties.Settings.Default.importVectorizePoTraceCurveoptimizing;//true;

            Logger.Trace("●●●● PoTrace  find transparency:{0}  turdsize:{1}  alphamax:{2}  opttol:{3}  {4}", findTransparency, Potrace.turdsize, Potrace.alphamax, Potrace.opttolerance, logString);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            bool[,] Matrix;
            ArrayList ListOfCurveArray;
            ListOfCurveArray = new ArrayList();

            if (findTransparency)
            {
                Matrix = Potrace.BitMapToBinaryAlpha(image, greyThreshold);
                if (Potrace.alphaCount == 0)
                {
                    Logger.Warn("⚠⚠⚠ PoTrace no transparency found, try with brightness threshold {0} ⚠⚠⚠⚠⚠", greyThreshold);
                    Matrix = Potrace.BitMapToBinary(image, greyThreshold);
                    Graphic.SetHeaderInfo(" Vectorize - PoTrace - find brightness");
                }
                else
                    Graphic.SetHeaderInfo(" Vectorize - PoTrace - find transparency");

            }
            else
            {
                Matrix = Potrace.BitMapToBinary(image, greyThreshold);
                Graphic.SetHeaderInfo(" Vectorize - PoTrace - find brightness");
            }
            Potrace.potrace_trace(Matrix, ListOfCurveArray);
            Potrace.Export2Graphic(ListOfCurveArray, image.Width, image.Height);
            image.Dispose();

            Graphic.ScaleXY(1 / resoPxMm, 1 / resoPxMm);
        }

        private static void DoMyTrace(Bitmap image)
        {
            string logString = " DPI set to:";
            double resoPxMm = (double)Properties.Settings.Default.importPDNDpi;
            if (Properties.Settings.Default.importVectorizeDpiFromImage)
            { resoPxMm = image.HorizontalResolution; logString = " DPI from image:"; }
            logString += string.Format(" {0:0.0}", resoPxMm);
            resoPxMm /= 25.4;

            if (Properties.Settings.Default.importVectorizeSetWidthOfImage)
            {
                resoPxMm = image.Width / (double)Properties.Settings.Default.importPDNWidth;
                logString = string.Format(" DPI from given width: {0:0.0}", Properties.Settings.Default.importPDNWidth);
            }
            bool findTransparency = Properties.Settings.Default.importVectorizeDetectTransparency;
            bool invertSearch = Properties.Settings.Default.importVectorizeInvertResult;
            short greyThreshold = (short)Properties.Settings.Default.importVectorizeThreshold;
            int smoothCnt = (int)Properties.Settings.Default.importVectorizeSmoothCycles;
            Logger.Trace("●●●● GeometricTrace  find transparency:{0} {1}", findTransparency, logString);

            GeometricVectorize.DoTracing(image, greyThreshold, smoothCnt, findTransparency, invertSearch);
            if (findTransparency) // repeat with find grey
            {
                if (GeometricVectorize.ObjectColor == Color.Transparent)
                {
                    findTransparency = false;
                    Logger.Warn("⚠⚠⚠ GeometricTrace no transparency found, try with brightness threshold {0} ⚠⚠⚠⚠⚠", greyThreshold);
                    GeometricVectorize.DoTracing(image, greyThreshold, smoothCnt, findTransparency, invertSearch);
                    Graphic.SetHeaderInfo(" Vectorize - GeometricTrace - find brightness");
                }
                else
                    Graphic.SetHeaderInfo(" Vectorize - GeometricTrace - find transparency");
            }
            else
                Graphic.SetHeaderInfo(" Vectorize - GeometricTrace - find brightness");


            Graphic.SetPenColor(ColorTranslator.ToHtml(GeometricVectorize.ObjectColor));
            outlineList = GeometricVectorize.outlinePaths;
            image.Dispose();

            int scl = GeometricVectorize.pixelScale;
            Logger.Trace("►►►► outlineList count:{0}  color:{1}  scale:{2}", outlineList.Count, ColorTranslator.ToHtml(GeometricVectorize.ObjectColor), scl);
            foreach (List<Point> path in outlineList)
            {
                if (path.Count > 0)
                {
                    Graphic.StartPath(path[0].X / (resoPxMm * scl), path[0].Y / (resoPxMm * scl));
                    foreach (PointF aP in path)
                    {
                        Graphic.AddLine(aP.X / (resoPxMm * scl), aP.Y / (resoPxMm * scl));
                    }
                    Graphic.StopPath();
                }
                path.Clear();
            }
            outlineList.Clear();
        }

        [DataContract]
        internal class PJSLayer
        {
            [DataMember] internal int width;
            [DataMember] internal int height;
            [DataMember] internal bool visible;
            [DataMember] internal byte opacity;
            [DataMember] internal String name;
            [DataMember] internal String blendMode;
            [DataMember] internal String mimeType;
            [DataMember] internal String base64;
        }

        [DataContract]
        internal class PJSFile
        {
            [DataMember] internal HashSet<String> features = new HashSet<String>();
            [DataMember] internal int width;
            [DataMember] internal int height;

            [DataMember]
            internal List<PJSLayer> layers = new List<PJSLayer>();
        }

        internal static class Features
        {
            // any strings that can go to "features" array are to be defined and referenced via this class
            internal const String RESERVED = "RESERVED";
        }
    }
}
