/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace GrblPlotter
{
    public static class GCodeFromPDNJson
    {
        public static string ConversionInfo { get; set; }
        private static int shapeCounter = 0;
        private static bool logEnable = true;
        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent = null;

        private static short[,] resultBinaryMatrix;
        private static List<List<PointF>> outlineList;

        private static PJSFile pjsFile = null;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool ConvertFromFile(string filePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            Logger.Info(" Create GCode from {0}", filePath);
            backgroundWorker = worker;
            backgroundEvent = e;

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
                        Logger.Error(err, "Error loading PDN-Json Code");
                        MessageBox.Show("Error '" + err.ToString() + "' in PDN-Json file " + filePath);// throw;
                    }
                }
            }
            else
            {
                if (File.Exists(filePath))
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
                else { MessageBox.Show("PDN-Json file does not exist: " + filePath); return false; }
            }
            return false;
        }

        private static bool ConvertBitmaps(string filePath)
        {
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);

            Logger.Info("▼▼▼▼  ConvertPDNJson Start {0}", filePath);
            Logger.Trace("►►►► pjsFile width:{0}  height:{1}  layers:{2}", pjsFile.width, pjsFile.height, pjsFile.layers.Count);

            ConversionInfo = "";
            shapeCounter = 0;

            Graphic.Init(Graphic.SourceType.PDNJson, filePath, backgroundWorker, backgroundEvent);
            short toolNr = 1;
            int smoothCnt = (int)Properties.Settings.Default.importPDNSmoothCycles;
            float penRadius = 0.1f;
            double resoOutlineX, resoOutlineY;// / 3.78f px/mm;
            resoOutlineX = resoOutlineY = (double)Properties.Settings.Default.importPDNDpi / 25.4;

            bool showAllLayers = !Properties.Settings.Default.importPDNLayerVisible;

            Graphic.SetPenWidth("0.5");
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
                        byte[] bytes = Convert.FromBase64String(pjsFile.layers[i].base64);
                        Bitmap image;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = (Bitmap)Image.FromStream(ms);
                            image.RotateFlip(RotateFlipType.RotateNoneFlipY);   // y-axis is inverted
                        }

                        Color bitmapColor = GenerateImageMap((Bitmap)image, ref resultBinaryMatrix);
                        Graphic.SetPenColor(ColorTranslator.ToHtml(bitmapColor).Substring(1));
                        Graphic.SetPenColorId(i);
                        outlineList = Vectorize.GetPaths(resultBinaryMatrix, pjsFile.layers[i].width, pjsFile.layers[i].height, toolNr, smoothCnt, penRadius, false);

                        Logger.Trace("►► outlineList count:{0}  color:{1}", outlineList.Count, ColorTranslator.ToHtml(bitmapColor));

                        foreach (List<PointF> path in outlineList)//	kann PointInt sein!!! Da Bitmap-Pixel-Koordinaten gespeichert werden.
                        {
                            if (path.Count > 0)
                            {
                                Graphic.StartPath(path[0].X / resoOutlineX, path[0].Y / resoOutlineY);
                                foreach (PointF aP in path)
                                {
                                    Graphic.AddLine(aP.X / resoOutlineX, aP.Y / resoOutlineY);
                                }
                                Graphic.StopPath();
                            }
                            path.Clear();
                        }
                        outlineList.Clear();
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

        // only check for alpha channel
        private static Color GenerateImageMap(Bitmap image, ref short[,] binaryMatrix)
        {
            if (image == null) { Logger.Warn("⚠ GenerateImageMap adjustedImage == null"); return Color.Black; }//if no image, do nothing

            BitmapData dataAdjusted = null;
            Color myColor = Color.Transparent;

            int rectWidth = -1, rectHeight = -1;
            try
            {
                rectWidth = image.Width;
                rectHeight = image.Height;
                binaryMatrix = new short[rectWidth, rectHeight];
                Rectangle rectAdjusted = new Rectangle(0, 0, rectWidth, rectHeight);

                Logger.Trace("► GenerateImageMap: size:{0} x {1}  bits:{2}", rectWidth, rectHeight, Image.GetPixelFormatSize(image.PixelFormat));

                dataAdjusted = image.LockBits(rectAdjusted, ImageLockMode.ReadOnly, image.PixelFormat);

                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                int psize = 4;
                long bsize = dataAdjusted.Stride * rectHeight;
                byte[] pixelData = new byte[bsize];
                Marshal.Copy(ptrAdjusted, pixelData, 0, pixelData.Length);

                byte r, g, b, a;
                int bx = 0, by = 0;
                for (long index = 0; index < pixelData.Length; index += psize)
                {
                    a = pixelData[index + 3];

                    if (a < 128)      	// transparent?          
                    {
                        binaryMatrix[bx++, by] = -1;// Vectorize.markBackground;
                    }
                    else
                    {
                        binaryMatrix[bx++, by] = 1;// Vectorize.markObject;
                        if (myColor == Color.Transparent)
                        {
                            b = pixelData[index];
                            g = pixelData[index + 1];
                            r = pixelData[index + 2];
                            myColor = Color.FromArgb(a, r, g, b);
                        }
                    }

                    if (bx >= rectWidth)
                    { bx = 0; by++; }
                }
                image?.UnlockBits(dataAdjusted);
            }
            catch (Exception err)
            {
                string errString = string.Format("GenerateImageMap: size:{0} x {1}  bits:{2}", rectWidth, rectHeight, Image.GetPixelFormatSize(image.PixelFormat));
                Logger.Error(err, " {0}  ", errString);
                EventCollector.StoreException(errString + "  " + err.Message);
                image?.UnlockBits(dataAdjusted);
            }
            return myColor;
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
