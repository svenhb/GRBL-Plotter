/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2024-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2024-08-07 Implementation for https://github.com/sbtrn-devil/pdn-json
 * 2024-08-20 option to find white background
 * 2024-09-20 add paste from clipboard
 * 2024-10-08 support PoTrace with different DPIs
 * 2026-03-20 get penWidth from selected device default - MyControl.GetActualToolDiameter()
*/

using AForge.Imaging.Filters;
using CsPotrace;
using GrblPlotter.Helper;
using GrblPlotter.UserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        private static Dictionary<Color, int> differentColor = new Dictionary<Color, int>();
        private static List<KeyValuePair<Color, int>> colorsToUse = new List<KeyValuePair<Color, int>>();

        private static double resoPxMm = (double)Properties.Settings.Default.importPDNDpi;

        private static string temOutput = Datapath.Data + "\\_imageProcessing";
        private static bool logSaveImages = true;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void LoadFromClipboard(string tempFile)									// called from MainFormLoadFile 1383
        {
            backgroundWorker = null;
            backgroundEvent = null;
            try
            {
                IDataObject iData = Clipboard.GetDataObject();
                Logger.Info("▼▼▼▼  ConvertBitmap pasteFromClipboard");
                if ((iData != null) && (iData.GetDataPresent(DataFormats.Bitmap)))
                {
                    shapeCounter = 0;
                    Bitmap image = new Bitmap(Clipboard.GetImage());
                    try
                    {
                        Logger.Trace("LoadFromClipboard, save tmpFile:{0} as png", tempFile);
                        image.Save(tempFile, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception err) { Logger.Error(err, " LoadFromClipboard, could not save image data to temporary file {0} ", tempFile); }

                    Graphic.Init(Graphic.SourceType.Image, "from Clipboard", backgroundWorker, backgroundEvent);
                    Graphic.SetGeometry(Path.GetExtension("clipboard").ToLower().Replace(".", ""));
                    ConvertBitmap(image, "");
                    ConversionInfo += string.Format("{0} elements imported", shapeCounter);
                    Logger.Info("▲▲▲▲  ConvertPDNJson Finish: shapeCounter: {0} ", shapeCounter);
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

            if (logSaveImages)	// save color extracted bw-images to special folder
            {
                if (!Directory.Exists(temOutput))
                { Directory.CreateDirectory(temOutput); }
                else
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(temOutput);
                    foreach (FileInfo file in di.EnumerateFiles())
                    {
                        file.Delete();
                    }
                }
            }

            penWidth = MyControl.GetActualToolDiameter().ToString();
            Logger.Info("▼▼▼▼  ConvertBitmaps Start {0}  Pen width {1:0.000}  from {2}", filePath, penWidth, MyControl.GetSelectedDeviceName());
            Logger.Trace("►►►► pjsFile width:{0}  height:{1}  layers:{2}", pjsFile.width, pjsFile.height, pjsFile.layers.Count);

            ConversionInfo = "";
            shapeCounter = 0;

            Graphic.Init(Graphic.SourceType.Image, filePath, backgroundWorker, backgroundEvent);
            Graphic.SetPenWidth(penWidth);

            bool showAllLayers = !Properties.Settings.Default.importPDNLayerVisible;
            for (int i = 0; i < pjsFile.layers.Count; i++)
            {
                Logger.Trace("▼▼▼▼ layer:{0}  width:{1}  height:{2}  visible:{3}  name:{4}", i, pjsFile.layers[i].width, pjsFile.layers[i].height, pjsFile.layers[i].visible, pjsFile.layers[i].name);
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
                        Bitmap image, bmpBw8bpp;
                        byte[] bytes = Convert.FromBase64String(pjsFile.layers[i].base64);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = (Bitmap)System.Drawing.Image.FromStream(ms);
                            if (logSaveImages) image.Save(temOutput + "\\ConvertBitmapLayer_" + i.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        int colorAmount = CountImageColors(image, 5);
                        string colorHtml = "#000000";
                        if (colorsToUse.Count > 0)
                        {
                            colorHtml = ColorTranslator.ToHtml(colorsToUse[0].Key);
                        }
                        if (colorHtml.StartsWith("#")) { colorHtml = colorHtml.Substring(1); }
                        Logger.Trace("Selected color:{0}", colorHtml);
                        Graphic.SetPenColor(colorHtml);
                        Graphic.SetPenFill(colorHtml);

                        bmpBw8bpp = SetTo8bpp(image, colorsToUse[0].Key);
                        if (logSaveImages) bmpBw8bpp.Save(temOutput + "\\ConvertBitmapExtractedColor_" + i.ToString() + "_" + colorHtml + ".png", System.Drawing.Imaging.ImageFormat.Png);

                        if (backgroundWorker != null)
                        {
                            backgroundWorker.ReportProgress(10, new MyUserState { Value = i * 100 / pjsFile.layers.Count, Content = string.Format("Convert PDN layers {0}/{1}  {2}", i + 1, pjsFile.layers.Count, colorHtml) });
                            if (backgroundWorker.CancellationPending)
                            {
                                backgroundEvent.Cancel = true;
                                break;
                            }
                        }

                        if (usePoTrace)
                        { DoPoTrace(bmpBw8bpp, true); }
                        else
                        { DoMyTrace(bmpBw8bpp, true); }

                        image.Dispose();
                        bmpBw8bpp.Dispose();
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
            shapeCounter = 0;
            Logger.Info("▼▼▼▼  ConvertBitmap Start path:{0}", filePath);
            Graphic.Init(Graphic.SourceType.Image, filePath, backgroundWorker, backgroundEvent);
            Graphic.SetGeometry(Path.GetExtension(filePath).ToLower().Replace(".", ""));
            Bitmap orig = (Bitmap)System.Drawing.Image.FromFile(filePath);
            return ConvertBitmap(orig, filePath);
        }
        private static bool ConvertBitmap(Bitmap orig, string filename)
        {
            int pixelArtSize = 320;
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
            bool usePoTrace = Properties.Settings.Default.importVectorizeAlgorithmPoTrace;

            if (logSaveImages)	// save color extracted bw-images to special folder
            {
                if (!Directory.Exists(temOutput))
                { Directory.CreateDirectory(temOutput); }
                else
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(temOutput);
                    foreach (FileInfo file in di.EnumerateFiles())
                    {
                        //	Logger.Trace("Empty folder {0}  keep:{1}",file.FullName, filename);
                        if (filename != file.FullName)
                            file.Delete();
                    }
                }
            }
            ConversionInfo = "";

            bool useBWThreshold = !Properties.Settings.Default.importVectorizeAutomatic;
            bool pixelArt = (orig.Width <= pixelArtSize) && (orig.Height <= pixelArtSize);
            penWidth = MyControl.GetActualToolDiameter().ToString();
            Graphic.SetPenWidth(penWidth);
            Logger.Info("▼▼▼▼  ConvertBitmap size:{0}x{1}  orig pixelFormat:{2}   Device:{3}   pixelArt:{4} Pen width:{5:0.000} use PoTrace?:{6}", orig.Width, orig.Height, orig.PixelFormat, MyControl.GetSelectedDeviceName(), pixelArt, penWidth, usePoTrace);

            Bitmap image = orig;

            // 1bpp image source wiil be load as PixelFormat.Format32bppArgb
            if ((orig.PixelFormat == PixelFormat.Format8bppIndexed) || (orig.PixelFormat == PixelFormat.Format4bppIndexed) || (orig.PixelFormat == PixelFormat.Format1bppIndexed))
            {
                Logger.Info("▼▼▼▼  ConvertBitmap convert pixelFormat from {0}  to  Format24bppRgb", orig.PixelFormat);
                backgroundWorker?.ReportProgress(0, new MyUserState { Value = 10, Content = string.Format("Convert from {0} to 24bpp", orig.PixelFormat) });
                image = SetPixelFormat(orig, PixelFormat.Format24bppRgb);   // very slow
                /*  using (Bitmap oldBmp = new Bitmap(orig))
                    using (Bitmap newBmp = new Bitmap(oldBmp))
                    image = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), PixelFormat.Format24bppRgb);
                */
            }

            /* get color-count for count > 5%, store in colorsToUse - most frequent first */
            bool colorMode = true;
            double colorAmountLimitPercent = pixelArt ? 0.5 : 5;
            int colorAmount = CountImageColors(image, colorAmountLimitPercent);
            string colorHtml = "000000";


            if (colorAmount == 0)
            { useBWThreshold = true; }
            else if (colorAmount == 1)
            {
                colorMode = false;  // use brightness tthreshold
                colorHtml = ColorTranslator.ToHtml(colorsToUse[0].Key);
            }
            else if (colorAmount == 2)
            {
                int b0 = Colors.GrayColorValue(colorsToUse[0].Key);
                int b1 = Colors.GrayColorValue(colorsToUse[1].Key);
                if (b1 < b0)
                    colorsToUse.RemoveAt(0);
                else
                    colorsToUse.RemoveAt(1);
                colorHtml = ColorTranslator.ToHtml(colorsToUse[0].Key);
            }
            if (colorHtml.StartsWith("#")) { colorHtml = colorHtml.Substring(1); }

            Logger.Trace("●●●● colorAmount:{0}  colorsToUse:{1}  colorMode:{2}  useBWThreshold:{3}", colorAmount, colorsToUse.Count, colorMode, useBWThreshold);

            if (logSaveImages) image.Save(temOutput + "\\ConvertBitmapBeforeExtraction.png", System.Drawing.Imaging.ImageFormat.Png);

            if (!colorMode || useBWThreshold)
            {
                Graphic.SetPenColor(colorHtml);
                Graphic.SetPenFill(colorHtml);
                Logger.Trace("▼▼▼▼ Use BW threshold");
                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress(10, new MyUserState { Value = 20, Content = string.Format("Convert black-white") });
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundEvent.Cancel = true;
                        //    break;
                    }
                }
                if (usePoTrace)
                { DoPoTrace(image, false); }
                else
                { DoMyTrace(image, false); }
                shapeCounter = 1;
            }
            else
            {
                if (colorAmount >= colorsToUse.Count) colorAmount = colorsToUse.Count;
                if (colorAmount == 0)
                {
                    ConversionInfo = "Error - No single color above 5% occurance";
                }
                else
                {
                    Bitmap bmpBw8bpp = image;
                    for (int i = 0; i < colorAmount; i++)
                    {
                        colorHtml = ColorTranslator.ToHtml(colorsToUse[i].Key);
                        if (colorHtml.StartsWith("#")) { colorHtml = colorHtml.Substring(1); }
                        Logger.Trace("▼▼▼▼ Selected color:{0}", colorHtml);
                        bmpBw8bpp = SetTo8bpp(image, colorsToUse[i].Key);

                        /* keep pixel-art */
                        if (!pixelArt)
                        {
                            Opening op = new Opening();
                            op.ApplyInPlace(bmpBw8bpp);

                            Closing cl = new Closing();
                            cl.ApplyInPlace(bmpBw8bpp);
                        }

                        Graphic.SetPenColor(colorHtml);
                        Graphic.SetPenFill(colorHtml);
                        if (logSaveImages) bmpBw8bpp.Save(temOutput + "\\ConvertBitmapExtractedColor_" + i.ToString() + "_" + colorHtml + ".png", System.Drawing.Imaging.ImageFormat.Png);

                        if (backgroundWorker != null)
                        {
                            backgroundWorker.ReportProgress(10, new MyUserState { Value = i * 100 / colorAmount, Content = string.Format("Convert colors {0}/{1}  {2}", i + 1, colorAmount, colorHtml) });
                            if (backgroundWorker.CancellationPending)
                            {
                                backgroundEvent.Cancel = true;
                                break;
                            }
                        }
                        if (usePoTrace)
                        { DoPoTrace(bmpBw8bpp, true); }
                        else
                        { DoMyTrace(bmpBw8bpp, true); }
                    }
                    shapeCounter = colorAmount;
                    bmpBw8bpp.Dispose();
                }
            }
            if (usePoTrace)
                Graphic.ScaleXY(1 / resoPxMm, 1 / resoPxMm);

            image.Dispose();
            orig.Dispose();

            ConversionInfo += string.Format("{0} colors imported, with {1}  {2}", shapeCounter, usePoTrace ? "'Po Trace'" : "'Geometric Trace'", useBWThreshold ? "BW mode" : "color mode");
            Logger.Info("▲▲▲▲  ConvertPDNJson Finish: shapeCounter: {0} ", shapeCounter);
            return Graphic.CreateGCode();
        }

        private static void CalcResolution(Bitmap image)
        {
            string logString = " DPI set to:";
            resoPxMm = (double)Properties.Settings.Default.importPDNDpi;
            if (Properties.Settings.Default.importVectorizeDpiFromImage)
            { resoPxMm = image.HorizontalResolution; logString = " DPI from image:"; }
            logString += string.Format(" {0:0.0}", resoPxMm);
            resoPxMm /= 25.4;

            if (Properties.Settings.Default.importVectorizeSetWidthOfImage)
            {
                resoPxMm = image.Width / (double)Properties.Settings.Default.importPDNWidth;
                logString = string.Format(" DPI from given width: {0:0.0}", Properties.Settings.Default.importPDNWidth);
            }
            Logger.Trace("CalcResolution {0}", logString);
        }

        private static void DoPoTrace(Bitmap image, bool colorMode = true)
        {
            bool findTransparency = Properties.Settings.Default.importVectorizeDetectTransparency;
            short greyThreshold = (short)Properties.Settings.Default.importVectorizeThreshold;

            CalcResolution(image);

            /* use of PoTrace https://potrace.sourceforge.net/potrace.pdf
                https://potrace.sourceforge.net/potracelib.pdf */

            Potrace.turdsize = (int)Properties.Settings.Default.importVectorizePoTraceTurdsize;//  2;
            Potrace.alphamax = (double)Properties.Settings.Default.importVectorizePoTraceAlphamax;//  1;
            Potrace.opttolerance = (double)Properties.Settings.Default.importVectorizePoTraceOpttolerance;  //0.2;
            Potrace.curveoptimizing = Properties.Settings.Default.importVectorizePoTraceCurveoptimizing;//true;

            Logger.Trace("●●●● PoTrace  find transparency:{0}  turdsize:{1}  alphamax:{2}  opttol:{3}", findTransparency, Potrace.turdsize, Potrace.alphamax, Potrace.opttolerance);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            string colorHtml;
            bool[,] Matrix;
            ArrayList ListOfCurveArray;
            ListOfCurveArray = new ArrayList();

            if (colorMode)
            {
                Bitmap tmp = image;
                Matrix = Potrace.BitMapToBinaryFrom8bpp(image, 127);

                tmp = Potrace.BinaryToBitmap(Matrix, false);
                tmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                //    if (logSaveImages) tmp.Save(temOutput + "\\ConvertBitmapPotraceResult" + "_" + colorHtml + ".png", System.Drawing.Imaging.ImageFormat.Png);

                Graphic.SetHeaderInfo(" Vectorize - PoTrace - colorMode:" + colorMode.ToString());
                Potrace.potrace_trace(Matrix, ListOfCurveArray);
                Potrace.Export2Graphic(ListOfCurveArray, image.Width, image.Height);
                tmp.Dispose();
            }
            else
            {
                //   Graphic.SetPenColor("black");
                //    Graphic.SetPenFill("black");
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
                //   Graphic.ScaleXY(1 / resoPxMm, 1 / resoPxMm);
            }

            image.Dispose();
        }

        private static void DoMyTrace(Bitmap image, bool colorMode)
        {
            CalcResolution(image);

            bool findTransparency = Properties.Settings.Default.importVectorizeDetectTransparency;
            bool invertSearch = Properties.Settings.Default.importVectorizeInvertResult;
            short greyThreshold = (short)Properties.Settings.Default.importVectorizeThreshold;
            int smoothCnt = (int)Properties.Settings.Default.importVectorizeSmoothCycles;
            Logger.Trace("●●●● GeometricTrace  find transparency:{0} ", findTransparency);
            try
            {
                //    GeometricVectorize.AvailableToolColors.Clear();
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

            }
            catch (Exception err)
            {
                Logger.Error(err, "DoMyTrace ");
                MessageBox.Show(Localization.GetString("importMessagePDNJErrorMyTrace"), Localization.GetString("codeMessage_error"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            int scl = GeometricVectorize.pixelScale;
            string penColor;
            if (!colorMode)
            {
                string colorHtml = ColorTranslator.ToHtml(GeometricVectorize.ObjectColor);
                if (colorHtml.StartsWith("#")) { colorHtml = colorHtml.Substring(1); }
                Graphic.SetPenColor(colorHtml);
                Graphic.SetPenFill(colorHtml);
            }
            outlineList = GeometricVectorize.outlinePaths;
            image.Dispose();

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

        private static int CountImageColors(Bitmap bmp, double colorAmountLimitPercent)
        {
            int psize = System.Drawing.Image.GetPixelFormatSize(bmp.PixelFormat) / 8;  // 4;
            Logger.Info("#### CountImageColors {0} x {1}  format:{2}    psize:{3}", bmp.Width, bmp.Height, bmp.PixelFormat, psize);
            BitmapData dataAdjusted;
            byte[,] Result = new byte[bmp.Width, bmp.Height];

            Color pxColor;
            byte pxIndex;
            long index = 0;
            differentColor.Clear();
            colorsToUse.Clear();
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            dataAdjusted = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr ptrAdjusted = dataAdjusted.Scan0;
            long bsize = dataAdjusted.Stride * bmp.Height;
            byte[] pixelData = new byte[bsize];
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(ptrAdjusted, pixelData, 0, pixelData.Length);

                if (psize == 1)
                {
                    for (index = 0; index < pixelData.Length; index += psize)
                    {
                        pxIndex = pixelData[index];   //pixelData[index].;
                        pxColor = bmp.Palette.Entries[pxIndex];
                        if (!differentColor.ContainsKey(pxColor))
                            differentColor.Add(pxColor, 1);
                        else
                            differentColor[pxColor]++;
                    }
                }
                else
                {
                    byte r = 0, g = 0, b = 0, a = 255;  // default, if no pixel found
                    if (psize == 4)
                    {
                        for (index = 0; index < pixelData.Length - psize; index += psize)
                        {
                            b = pixelData[index];
                            g = pixelData[index + 1];
                            r = pixelData[index + 2];
                            a = pixelData[index + 3];

                            if (a > 192)
                            {
                                pxColor = Color.FromArgb(r, g, b);
                                if (!differentColor.ContainsKey(pxColor))
                                    differentColor.Add(pxColor, 1);
                                else
                                    differentColor[pxColor]++;
                            }
                        }
                    }
                    else
                    {
                        for (index = 0; index < pixelData.Length - psize; index += psize)
                        {
                            b = pixelData[index];
                            g = pixelData[index + 1];
                            r = pixelData[index + 2];

                            pxColor = Color.FromArgb(r, g, b);
                            if (!differentColor.ContainsKey(pxColor))
                                differentColor.Add(pxColor, 1);
                            else
                                differentColor[pxColor]++;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                string errString = string.Format("#### CountImageColors: size:{0} x {1}  bits:{2}  psize:{3}  index:{4}", bmp.Width, bmp.Height, System.Drawing.Image.GetPixelFormatSize(bmp.PixelFormat), psize, index);
                Logger.Error(err, "{0}  ", errString);
                EventCollector.StoreException(errString + "  " + err.Message);
            }
            finally
            {
                bmp.UnlockBits(dataAdjusted);
            }

            double percent;
            colorsToUse = differentColor.ToList();
            colorsToUse.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            int count = 0, i = 0;
            string skey;
            foreach (KeyValuePair<Color, int> kvp in colorsToUse)
            {
                percent = (double)100 * kvp.Value / (bmp.Width * bmp.Height);
                skey = ColorTranslator.ToHtml(kvp.Key);
                Logger.Trace("#### CountImageColors Key = {0}, Value = {1,7}   percent:{2:0.00}%", skey, kvp.Value, percent);
                if ((percent > colorAmountLimitPercent) || ((count == 0) && ((skey == "#FFFFFF") || (skey == "#000000"))))
                    count++;
                if (percent < 0.1)
                {
                    Logger.Trace("#### CountImageColors stop at {0} of {1}", i, colorsToUse.Count);
                    break;
                }
                i++;
            }
            return count;
        }

        //Apply dithering to an image (Convert to 8 bit)
        private static Bitmap SetTo8bpp(Bitmap input, Color useColor)
        {
            int psize = Image.GetPixelFormatSize(input.PixelFormat) / 8;
            Logger.Info("●●●● SetTo8bpp {0} x {1}  format:{2}   psize:{3} ", input.Width, input.Height, input.PixelFormat, psize);
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format8bppIndexed);
            var data = new byte[input.Width, input.Height];
            var data2 = new byte[input.Width, input.Height];
            var inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, input.PixelFormat);//PixelFormat.Format24bppRgb);
            var outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);// Format1bppIndexed);
            try
            {
                var scanLine = inputData.Scan0;
                var scanLine2 = outputData.Scan0;
                var line = new byte[inputData.Stride];
                var alpha = 255;
                for (var y = 0; y < inputData.Height; y++, scanLine += inputData.Stride)
                {
                    Marshal.Copy(scanLine, line, 0, line.Length);
                    var line2 = new byte[outputData.Stride];
                    for (var x = 0; x < input.Width; x++)
                    {
                        if (psize == 4) { alpha = line[x * psize + 3]; }
                        line2[x] = (byte)(((alpha > 192) && (line[x * psize + 2] == useColor.R) && (line[x * psize + 1] == useColor.G) && (line[x * psize + 0] == useColor.B)) ? 0 : 255);
                    }
                    Marshal.Copy(line2, 0, scanLine2, outputData.Stride);
                    scanLine2 += outputData.Stride;
                }
            }
            catch (Exception err)
            {
                string errString = string.Format("SetTo8bpp: size:{0} x {1}  bits:{2}", input.Width, input.Height, System.Drawing.Image.GetPixelFormatSize(input.PixelFormat));
                Logger.Error(err, " {0}  ", errString);
                EventCollector.StoreException(errString + "  " + err.Message);
            }
            finally
            {
                input.UnlockBits(inputData);
                output.UnlockBits(outputData);
            }
            return output;
        }

        //Apply dithering to an image (Convert to 1 bit)
        private static Bitmap SetToBW(Bitmap input, Color useColor)
        {
            Logger.Info("●●●● SetToBW {0} x {1}  format:{2}    ", input.Width, input.Height, input.PixelFormat);
            var masks = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format1bppIndexed);
            var data = new byte[input.Width, input.Height];
            var data2 = new byte[input.Width, input.Height];
            var inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);// Format1bppIndexed);
            try
            {
                var scanLine = inputData.Scan0;
                var scanLine2 = outputData.Scan0;
                var line = new byte[inputData.Stride];
                //    var line2 = new byte[outputData.Stride];
                byte val;
                for (var y = 0; y < inputData.Height; y++, scanLine += inputData.Stride)
                {
                    scanLine2 += outputData.Stride;
                    Marshal.Copy(scanLine, line, 0, line.Length);
                    var line2 = new byte[outputData.Stride];
                    for (var x = 0; x < input.Width; x++)
                    {
                        val = (byte)(((line[x * 3 + 2] == useColor.R) && (line[x * 3 + 1] == useColor.G) && (line[x * 3 + 0] == useColor.B)) ? 0 : 255);
                        //    line2[x * 3 + 2] = line2[x * 3 + 1] = line2[x * 3 + 0] = val;
                        //    var j = data[x, y] > 0;
                        if (val > 0) line2[x / 8] |= masks[x % 8];
                    }
                    Marshal.Copy(line2, 0, scanLine2, outputData.Stride);
                }
            }
            finally
            {
                input.UnlockBits(inputData);
                output.UnlockBits(outputData);
            }
            return (output);
        }

        /* convert any image format to Format32bppArgb 
     https://stackoverflow.com/questions/2016406/converting-bitmap-pixelformats-in-c-sharp 
    */
        private static Bitmap SetPixelFormat(Bitmap orig, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            Bitmap clone = new Bitmap(orig.Width, orig.Height, pixelFormat);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(orig, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            return clone;
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
