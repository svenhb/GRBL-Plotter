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

/*  GCodeFromSVG.cs a static class to convert SVG data into G-Code 
    Not implemented: 
        Basic-shapes: Text, Image
        Transform: rotation with offset, skewX, skewY

    GCode will be written to gcodeString[gcodeStringIndex] where gcodeStringIndex corresponds with color of element to draw
*/
/* 2016-07-18 get stroke-color from shapes, use GIMP-palette information to find tool-nr related to stroke-color
              add gcode for tool change
   2018-01-02 Bugfix SVG rect transform (G3 in roundrect)
              Bugfix SVG End GCode Path before next SVG subpath
              Bugfix Scale to max dimension
   2018-07    importInMM = Properties.Settings.Default.importUnitmm;
   2018-11-04 Y-Offset Problem line 347: ...(svgHeightPx * scale)
              Transform problem: overwrite old matrix[index] line 467: end if here
   2019-01-23 Change order line 165
   2019-01-28 Add DotOnly for Basic shapes
   2019-05-12 change possible transformation depth from 10 to 100 - line 95
   2019-06-10 xmlMarker.figureStart
   2019-06-19 cmd="z" avoid final move if gcode.isEqual((float)firstX,lastX) line 929
   2019-07-04 fix sorting problem with <figure tag
   2019-07-08 line 728 optimize 'polyline'
   2019-08-15 add logger
   2019-08-31 swap code to new class 'plotter'
                to do line 995 check svgClosePathExtend
   2019-09-19 add stroke-dasharray
   2019-11-24 Code outsourcing to importMath.cs
   2019-12-07 add extended log
   2019-12-22 Line 100 add try/catch for bad SVG-XML
   2020-02-28 Bug fix "polygon"
   2020-03-30 Grouping also by Layer-Name (ID of 1st level groups)
   2020-04-08 Line 466 adapt changing from arkypita-LaserGRBL  Experimental SVG support #451
 * 2020-05-07 Replace class Plotter by class Graphic for sorting
 * 2020-07-20 clean up
 * 2020-08-02 convert to double instead of float, line 567; bug fix line 436 element.Attribute
 * 2020-08-04 fix #138 Descripten CRLF remove
 * 2020-08-10 fix log output line 1250
 * 2020-11-13 fix bad point list - not separated by ' ', but by ','  line 678
 *              created by https://github.com/LingDong-/linedraw
 * 2020-12-08 add BackgroundWorker updates
 * 2021-03-25 line 613 GetStyleProperty include ':' to check property
 * 2021-07-31 code clean up / code quality
 * 2021-09-21 also read inkscape:label
 * 2022-02-27 switch order of  ParseTransform (now 1st) and ParseAttributs / apply matrix-scale to svgStrokeWidthScale
 * 2022-03-01 ParsePathCommand line 1080 if found a 2nd 'M' or 'm' command within 'd' path	- if same coordinate, do nothing
 * 2022-06-14 line 440/782/1031 skip layer if style="display:none"; attribute "visibility"
 * 2022-06-18 bug fix getting first coordinates in "m" command and applying in "z" command
 * 2022-07-23 line 376, 406 issue #1868 LaserGRBL
 * 2022-08-10 add text-element
 * 2022-09-29 line 539, 603, 624 add (attributeFill != "none"))
 * 2022-09-30 line 1330 SVGStartPath -> SetGeometry = startPath to continue figure
 * 2022-11-04 line 845 dash pattern - convert px to mm
 * 2022-12-09 line 1290 ParsePathCommand check if string is empty
 * 2023-04-10 :211 f:ConvertFromFile check length for substring
 * 2023-04-17 add skip2ndCounter
 * 2023-04-18 l:410 f:ParseGlobals add GCode-Header output of SVG dimension
 * 2023-04-25 l:1260 f:ParsePath add filtering
 * 2023-05-27 l:644 f:ParseAttributs (attributeStrokeWidth.Length > 1) -> change to > 0 to allow single digit
 * 2023-05-30 l:464 f:ParseGlobals separate extraction of metadata for vers 1.7.0.0
 * 2023-06-13 l:2011 f:SVGStartPath limit SetGeometry length to 20
 * 2023-06-27 l:1520 f:ParsePathCommand add svgConvertCircleToDot functionality
 * 2023-09-07 l:1375 f:ParsePathCommand check and fix arguments before processing   // Example to fix: '68.93206.00002'
 *            l:1600 f:ParsePathCommand bug fix relative commands for C, S
 * 2023-09-12 l:1390 f:ParsePathCommand also take care of this combination          // 1.042.018.751.751
 *            
*/

/* SetHeaderMessages...
 * 1101 Unknown SVG-Path-Element
 * 1102 Element not visible
 * 1103 Not supported SVG element: Image
 * 1104 Unknown SVG element
 * 1105 'tspan' within 'textPath'
 * 1106 Attribute not implemented
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Linq;

namespace GrblPlotter
{
    public static partial class GCodeFromSvg
    {
        private static bool svgScaleApply = true;       // try to scale final GCode if true
        private static float svgMaxSize = 100;          // final GCode size (greater dimension) if scale is applied
        private static bool svgNodesOnly = true;        // if true only do pen-down -up on given coordinates
        private static bool svgComments = true;         // if true insert additional comments into GCode
        private static bool svgConvertCircleToDot = false;

        private static bool svgConvertToMM = true;
        private static float gcodeScale = 1;                    // finally scale with this factor if svgScaleApply and svgMaxSize
        private static readonly Matrix[] matrixGroup = new Matrix[100];  // store SVG-Group transformation matrixes
        private static Matrix matrixElement = new Matrix();     // store finally applied matrix
        private static Matrix oldMatrixElement = new Matrix();  // store finally applied matrix
        private static Matrix lastTransformMatrix = new Matrix();

        private static float factor_In2Px = 96;
        private static float factor_Mm2Px = 96f / 25.4f;    // 3.779
        private static float factor_Cm2Px = 96f / 2.54f;    // 37.79
        private static float factor_Pt2Px = 96f / 72f;      // 1.333
        private static float factor_Pc2Px = 12 * 96f / 72f;
        private static float factor_Em2Px = 16; //150;

        public static string ConversionInfo { get; set; }
        public static string MetaData { get; set; }

        private static int shapeCounter = 0;
        private static int skipCounter = 0;
        private static int skip2ndCounter = 0;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logEnableCoord = false;
        private static string logSource = "";

        private static BackgroundWorker backgroundWorker = null;
        private static DoWorkEventArgs backgroundEvent = null;


        /// <summary>
        /// Entrypoint for conversion: apply file-path or file-URL
        /// </summary>
        /// <param name="file">String keeping file-name or URL</param>
        /// <returns>String with GCode of imported data</returns>
        private static XElement svgCode;
        private static bool fromClipboard = false;
        private static bool unitIsPixel = false;
        public static bool ConvertFromText(string svgText, bool GetMetaData, bool replaceUnitByPixel)
        {
            backgroundWorker = null;
            backgroundEvent = null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(svgText);
                MemoryStream stream = new MemoryStream(byteArray);
                svgCode = XElement.Load(stream, LoadOptions.PreserveWhitespace);
                stream.Dispose();
                fromClipboard = true;
                if (!String.IsNullOrEmpty(svgText) && svgText.IndexOf("Adobe") >= 0)
                    replaceUnitByPixel = false;
                unitIsPixel = replaceUnitByPixel;
                if (GetMetaData)
                {
                    MetaData = GetMetadata(svgCode, "description");
                    return !string.IsNullOrEmpty(MetaData);
                }
                return ConvertSVG(svgCode, "from Clipboard");                   // startConvert(svgCode);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error loading SVG-Code ");
                MessageBox.Show("Error '" + e.ToString() + "' in XML string ");
                return false;
            }
        }
        public static bool ConvertFromFile(string filePath, BackgroundWorker worker, DoWorkEventArgs e)
        { return ConvertFromFile(filePath, false, worker, e); }
        public static bool ConvertFromFile(string filePath, bool GetMetaData, BackgroundWorker worker, DoWorkEventArgs e)
        {
            unitIsPixel = false;
            fromClipboard = false;
            MetaData = "";

            backgroundWorker = worker;
            backgroundEvent = e;

            if (String.IsNullOrEmpty(filePath) || (filePath.Length < 5))
            { MessageBox.Show("Empty file name"); return false; }
            else if (filePath.Substring(0, 4) == "http")
            {
                string content = "";
                using (var wc = new System.Net.WebClient())
                {
                    try { content = wc.DownloadString(filePath); }
                    catch { MessageBox.Show("Could not load content from " + filePath); }
                }
                if ((!String.IsNullOrEmpty(content)) && (content.Contains("<?xml")))    // (content.IndexOf("<?xml") == 0)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(content);
                    MemoryStream stream = new MemoryStream(byteArray);
                    svgCode = XElement.Load(stream, LoadOptions.PreserveWhitespace);
                    System.Windows.Clipboard.SetData("image/svg+xml", stream);
                    stream.Dispose();
                    if (GetMetaData)
                    {
                        MetaData = GetMetadata(svgCode, "description");
                        return !string.IsNullOrEmpty(MetaData);
                    }
                    return ConvertSVG(svgCode, filePath);                   // startConvert(svgCode);
                }
                else
                {
                    int len = Math.Min(50, content.Length);
                    MessageBox.Show("This is probably not a SVG document.\r\nFirst line: " + content.Substring(0, len));
                }
            }
            else
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        svgCode = XElement.Load(filePath, LoadOptions.PreserveWhitespace);    // PreserveWhitespace);
                        if (GetMetaData)
                        {
                            MetaData = GetMetadata(svgCode, "description");
                            return !string.IsNullOrEmpty(MetaData);
                        }
                        return ConvertSVG(svgCode, filePath);                   // startConvert(svgCode);
                    }
                    catch (Exception err)
                    {
                        Logger.Error(err, "Error loading SVG-Code ");
                        MessageBox.Show("Error '" + err.ToString() + "' in XML file " + filePath + "\r\n\r\nTry to save file with other encoding e.g. UTF-8");
                    }
                }
                else { MessageBox.Show("File does not exist: " + filePath); return false; }
            }
            return false;
        }

        private static bool ConvertSVG(XElement svgCode, string filePath)
        {
            //    Logger.Info(" convertSVG {0}", filePath);
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
            logEnableCoord = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Detailed) > 0);

            svgScaleApply = Properties.Settings.Default.importSVGRezise;
            svgMaxSize = (float)Properties.Settings.Default.importSVGMaxSize;
            svgComments = Properties.Settings.Default.importSVGAddComments;
            svgConvertToMM = Properties.Settings.Default.importUnitmm;                  // Target units and display in setup
            svgNodesOnly = Properties.Settings.Default.importSVGNodesOnly;
            svgConvertCircleToDot = Properties.Settings.Default.importSVGCircleToDot;
            ConversionInfo = "";
            shapeCounter = 0; skipCounter = 0;
            globalTextProp = new TextProperties();

            //    Logger.Trace(" logEnable:{0} svgScaleApply:{1} svgMaxSize:{2} svgComments:{3} svgConvertToMM:{4} svgNodesOnly:{5} svgConvertCircleToDot:{6}", logEnable, svgScaleApply, svgMaxSize, svgComments, svgConvertToMM, svgNodesOnly, svgConvertCircleToDot);
            Logger.Info("▼▼▼▼  ConvertSVG Start : svgScaleApply: {0} svgMaxSize: {1} svgComments: {2} svgConvertToMM: {3} svgNodesOnly: {4} svgConvertCircleToDot: {5}", svgScaleApply, svgMaxSize, svgComments, svgConvertToMM, svgNodesOnly, svgConvertCircleToDot);

            Graphic.Init(Graphic.SourceType.SVG, filePath, backgroundWorker, backgroundEvent);
            GetVectorSVG(svgCode);                  // convert graphics
            ConversionInfo += string.Format("{0} elements imported; {1} skipped", shapeCounter, skipCounter);
            Logger.Info("▲▲▲▲  ConvertSVG Finish: shapeCounter: {0}   skipCounter: {1}", shapeCounter, skipCounter);
            svgCode.RemoveAll();
            //		myPath.Dispose();
            return Graphic.CreateGCode();
        }

        /// <summary>
        /// Set defaults and parse main element of SVG-XML
        /// </summary>
        private static void GetVectorSVG(XElement svgCode)
        {
            if (Properties.Settings.Default.importSVGDPI96)
                factor_In2Px = 96;                      // 90
            else
                factor_In2Px = 72;
            factor_Mm2Px = factor_In2Px / 25.4f;    // 3.54
            factor_Cm2Px = factor_Mm2Px * 10;       // 35.4
            factor_Pt2Px = factor_In2Px / 72f;      // 1.25
            factor_Pc2Px = 12 * factor_Pt2Px;       // 15
            factor_Em2Px = 16;  // 150;

            gcodeScale = 1;
            svgWidthPx = 0; svgHeightPx = 0;
            currentX = 0; currentY = 0;
            offsetX = 0; offsetY = 0;
            firstX = null;
            firstY = null;
            lastX = 0;
            lastY = 0;
            getTextPath = false;
            skip2ndCounter = 0;

            int count = svgCode.Descendants("path").Count();

            //    Logger.Info(" Amount Paths:{0}", count);
            backgroundWorker?.ReportProgress(0, new MyUserState { Value = 10, Content = "Read SVG vector data of " + count.ToString() + " elements " });

            matrixElement.SetIdentity();                // preset transform matrix
            oldMatrixElement.SetIdentity();             // preset backup of transform matrix
            for (int i = 0; i < matrixGroup.Length; i++)     // preset sub transform matrixes
                matrixGroup[i].SetIdentity();

            ParseGlobals(svgCode);                      // parse main svg elements

            ParseUse(svgCode, 1);
            ParseBasicElements(svgCode, 1);
            ParsePath(svgCode, 1);                       // 1st level paths
            if (svgCode != null)
            {
                ParseGroup(svgCode, 1);                      // parse groups (recursive)            
            }

            if (skip2ndCounter > 0)
                Logger.Warn("⚠ Skipped 'm' because of no position change: {0} skipped", skip2ndCounter);

            return;
        }

        /// <summary>
        /// Parse SVG dimension (viewbox, width, height)
        /// </summary>
        private static double svgWidthPx, svgHeightPx, svgStrokeWidthScale = 1;
        private static readonly XNamespace nspace = "http://www.w3.org/2000/svg";
        private static readonly XNamespace inkscape = "http://www.inkscape.org/namespaces/inkscape";
        private static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";

        private static void ParseGlobals(XElement svgCode)
        {   // One px unit is defined to be equal to one user unit. Thus, a length of "5px" is the same as a length of "5".
            if (logEnable) Logger.Debug("parseGlobals");
            Matrix tmp = new Matrix(1, 0, 0, 1, 0, 0); // m11, m12, m21, m22, offsetx, offsety
            svgWidthPx = 0;
            svgHeightPx = 0;
            float vbOffX = 0;
            float vbOffY = 0;
            float vbWidth = 0;
            float vbHeight = 0;
            float scale;
            string tmpString;

            if (svgCode.Attribute("viewBox") != null)   // viewBox unit always in px
            {
                string viewbox = svgCode.Attribute("viewBox").Value;
                logSource = "viewBox " + viewbox;
                viewbox = Regex.Replace(viewbox, @"\s+", " ").Replace(' ', '|');    // remove double space
                var split = viewbox.Split('|');
                vbOffX = -ConvertToPixel(split[0]);
                vbOffY = -ConvertToPixel(split[1]);
                vbWidth = ConvertToPixel(split[2]);
                vbHeight = ConvertToPixel(split[3].TrimEnd(')'));
                tmp.M11 = 1; tmp.M22 = -1;      // flip Y
                tmp.OffsetY = vbHeight;
                if (svgComments) Graphic.SetHeaderInfo(" SVG viewbox : " + viewbox);
                if (logEnable) Logger.Trace(" SVG viewbox: {0:0.00} {1:0.00} {2:0.00} {3:0.00} source: '{4}'", vbOffX, vbOffY, vbWidth, vbHeight, viewbox);
            }

            //       scale = 1;
            if (svgConvertToMM)                 // convert back from px to mm 
                scale = (1 / factor_Mm2Px);
            else                                // convert back from px to inch
                scale = (1 / factor_In2Px);

            if (unitIsPixel)                    // got svg-text from clipboard perhaps from maker.js
            { scale = 1; }

            if (svgComments) Graphic.SetHeaderInfo(" SVG dpi :" + factor_In2Px.ToString());
            if (svgCode.Attribute("width") != null)
            {
                tmpString = svgCode.Attribute("width").Value;
                logSource = "width " + tmpString;
                svgWidthPx = ConvertToPixel(tmpString, vbWidth);             // convert in px

                if (vbWidth > 0)
                    svgStrokeWidthScale = ConvertToPixel(tmpString, vbWidth) / factor_Mm2Px / vbWidth;
                //                Logger.Trace( "svgStrokeWidthScale {0} {1} {2} {3}", tmpString, vbWidth, factor_Mm2Px, vbWidth);

                if (svgComments) Graphic.SetHeaderInfo(" SVG width :" + svgCode.Attribute("width").Value);
                if (logEnable) Logger.Trace(" SVG width : {0:0.00} source: '{1}'", svgWidthPx, svgCode.Attribute("width").Value);
            }
            else
            {
                if (logEnable) Logger.Trace(" SVG width not set");
                svgWidthPx = vbWidth;   // from viewbox
            }

            tmp.M11 = scale; // get desired scale
            if (fromClipboard)
                tmp.M11 = 1 / factor_Mm2Px; // 3.543307;         // https://www.w3.org/TR/SVG/coords.html#Units
            if (vbWidth > 0)
            {
                tmp.M11 = scale * svgWidthPx / vbWidth;
                //    tmp.OffsetX = vbOffX * scale;   // svgWidthUnit / vbWidth;
                tmp.OffsetX = (vbOffX * svgWidthPx / vbWidth) * scale;   // 2022-07-23 #1868
            }

            if (svgCode.Attribute("height") != null)
            {
                tmpString = svgCode.Attribute("height").Value;
                logSource = "height " + tmpString;
                svgHeightPx = ConvertToPixel(tmpString, vbHeight);

                if (svgComments) Graphic.SetHeaderInfo(" SVG height:" + svgCode.Attribute("height").Value);
                if (logEnable) Logger.Trace(" SVG height: {0:0.00} source: '{1}'", svgHeightPx, svgCode.Attribute("height").Value);
            }
            else
            {
                if (logEnable) Logger.Trace(" SVG height not set");
                svgHeightPx = vbHeight;            // from viewbox
            }

            Graphic.SetHeaderInfo(string.Format(" SVG dimension : {0:0.00} {1:0.00} ", svgWidthPx / factor_Mm2Px, svgHeightPx / factor_Mm2Px));
            if (Properties.Settings.Default.importGraphicClipEnable && Properties.Settings.Default.importGraphicClip && Properties.Settings.Default.importGraphicClipGetDimAuto)
            {
                Properties.Settings.Default.importGraphicTileX = (decimal)(svgWidthPx / factor_Mm2Px);
                Properties.Settings.Default.importGraphicTileY = (decimal)(svgHeightPx / factor_Mm2Px);
            }

            tmp.M22 = -scale;   // get desired scale and flip vertical
            tmp.OffsetY = scale * svgHeightPx;  // svgHeightUnit;

            if (fromClipboard)
            {
                tmp.M22 = -1 / factor_Mm2Px;// 3.543307;
                tmp.OffsetY = svgHeightPx / factor_Mm2Px; // 3.543307;     // https://www.w3.org/TR/SVG/coords.html#Units
            }
            if (vbHeight > 0)
            {
                tmp.M22 = -scale * svgHeightPx / vbHeight;
                //    tmp.OffsetY = -vbOffY * svgHeightPx / vbHeight + (svgHeightPx * scale);
                tmp.OffsetY = (-vbOffY * svgHeightPx / vbHeight + svgHeightPx) * scale;     // 2022-07-23 #1868
            }

            float newWidth = (float)Math.Max(svgWidthPx, vbWidth);     // use value from 'width' or 'viewbox' parameter
            float newHeight = (float)Math.Max(svgHeightPx, vbHeight);
            if ((newWidth > 0) && (newHeight > 0))
            {
                if (svgScaleApply)
                {
                    gcodeScale = svgMaxSize / Math.Max(newWidth, newHeight);        // calc. factor to get desired max. size
                    tmp.Scale((double)gcodeScale, (double)gcodeScale);
                    if (svgConvertToMM)                         // https://www.w3.org/TR/SVG/coords.html#Units
                        tmp.Scale(factor_Mm2Px, factor_Mm2Px);  // 3.543307, 3.543307);
                    else
                        tmp.Scale(factor_In2Px, factor_In2Px);

                    if (svgComments)
                        Graphic.SetHeaderInfo(string.Format(" Scale to X={0} Y={1} f={2} ", newWidth * gcodeScale, newHeight * gcodeScale, gcodeScale));
                }
            }
            else
                if (svgComments) Graphic.SetHeaderInfo(" SVG Dimension not given ");

            for (int i = 0; i < matrixGroup.Length; i++)
            { matrixGroup[i] = tmp; }
            matrixElement = tmp;

            var element = svgCode.Element(nspace + "title");
            if (element != null)
            { Graphic.SetHeaderInfo(" Title: " + element.Value.Replace("\n", "").Replace("\r", "")); }

            string metadata = GetMetadata(svgCode, "description");
            if (!string.IsNullOrEmpty(metadata))
                Graphic.SetHeaderInfo(" Description: " + metadata.Replace("\n", "_").Replace("\r", "_"));

            if (Properties.Settings.Default.importGraphicFilterEnable)
            {
                string skipTxt = Properties.Settings.Default.importGraphicFilterChoiceRemove ? "remove" : "keep";
                string listTxt = Properties.Settings.Default.importGraphicFilterChoiceRemove ? Properties.Settings.Default.importGraphicFilterListRemove : Properties.Settings.Default.importGraphicFilterListKeep;
                Graphic.SetHeaderInfo(string.Format(" Filtering is active: {0} {1}", skipTxt, listTxt));
                Logger.Info("⚠⚠ FilterProperties {0} figures: {1}", skipTxt, listTxt);
            }
            return;
        }

        private static string GetMetadata(XElement svgCode, string keyWord)
        {
            var xtmp = svgCode.Element(nspace + "metadata");
            if ((xtmp) != null)
            {
                XNamespace ccspace = "http://creativecommons.org/ns#";
                XNamespace rdfspace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
                XNamespace dcspace = "http://purl.org/dc/elements/1.1/";
                //   if ((xtmp = xtmp.Element(dcspace + keyWord)) != null)
                //     return xtmp.Value;

                if ((xtmp = xtmp.Element(rdfspace + "RDF")) != null)
                {
                    if ((xtmp = xtmp.Element(ccspace + "Work")) != null)
                    {
                        if ((xtmp = xtmp.Element(dcspace + keyWord)) != null)
                            return xtmp.Value;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Parse Group-Element and included elements
        /// </summary>
        private static void ParseGroup(XElement svgCode, int level)
        {
            //   if (logEnable) Logger.Debug("●●●● ParseGroup Level:{0} Elements:{1}", level, svgCode.Elements(nspace + "g").Count());
            foreach (XElement groupElement in svgCode.Elements(nspace + "g"))
            {
                if (level == 1)
                {
                    string idtext = "ID not set " + level.ToString();
                    if (groupElement.Attribute("id") != null)
                        idtext = groupElement.Attribute("id").Value;

                    /* skip layer if not visible */
                    if (Properties.Settings.Default.importSVGDontPlot)
                    {
                        if ((groupElement.Attribute("style") != null) && (groupElement.Attribute("style").Value.Contains("display:none")))
                        {
                            Graphic.SetHeaderInfo(string.Format(" Hide SVG Layer:{0}   ", idtext));
                            Graphic.SetHeaderMessage(string.Format(" {0}-1102: SVG Layer '{1}' is not visible and will not be imported", CodeMessage.Attention, idtext));
                            continue;
                        }
                    }

                    if (groupElement.Attribute(inkscape + "label") != null)
                        Graphic.SetLabel(groupElement.Attribute(inkscape + "label").Value);

                    Graphic.SetLayer(idtext);
                }
                if (logEnable)
                {
                    string id = "-";
                    if (groupElement.Attribute("id") != null)
                        id = groupElement.Attribute("id").Value;
                    Logger.Trace("●●●● ParseGroup Level:{0} Elements:{1} Id:{2}", level, svgCode.Elements(nspace + "g").Count(), id);
                }
                ParseTransform(groupElement, true, level);      // transform will be applied in gcodeMove
                ParseAttributs(groupElement, true);             // process color and stroke-dasharray
                ParseUse(groupElement, level);
                ParseBasicElements(groupElement, level);
                ParsePath(groupElement, level);
                ParseGroup(groupElement, level + 1);
            }
            return;
        }

        private static void ParseUse(XElement svgCode, int level)
        {
            if (svgCode.Elements(nspace + "use").Count() == 0)
                return;
            foreach (XElement useElement in svgCode.Elements(nspace + "use"))
            {
                if (useElement != null)
                {
                    string href = "";
                    if (useElement.Attribute(xlink + "href") != null) href = useElement.Attribute(xlink + "href").Value.Substring(1);
                    if (useElement.Attribute("href") != null) href = useElement.Attribute("href").Value.Substring(1);

                    lastTransformMatrix = ParseTransform(useElement, false, level);      // transform will be applied in gcodeMove
                    ParseAttributs(useElement);             // process color and stroke-dasharray

                    float x = 0, y = 0;
                    if (useElement.Attribute("x") != null) x = ConvertToPixel(useElement.Attribute("x").Value);
                    if (useElement.Attribute("y") != null) y = ConvertToPixel(useElement.Attribute("y").Value);

                    IEnumerable<XElement> defs =
                    from def in svgCode.Elements(nspace + "defs")
                    select def;
                    foreach (XElement def in defs)
                    {
                        IEnumerable<XElement> ids =
                        from id in def.Descendants()
                        select id;

                        foreach (XElement id in ids)
                        {
                            if (id.Attribute("id") != null && id.Attribute("id").Value == href)
                            {
                                Logger.Trace("●● ParseUse  href:'{0}'  value:{1}    stroke:{2}  fill:{3}", href, id.Name.ToString(), attributeStroke, attributeFill);

                                Graphic.SetPathId(href);
                                if (attributeStroke != "") Graphic.SetPenColor(attributeStroke.StartsWith("#") ? attributeStroke.Substring(1) : attributeStroke);
                                if ((attributeFill != "") && (attributeFill != "none"))
                                    Graphic.SetPenFill(attributeFill.StartsWith("#") ? attributeFill.Substring(1) : attributeFill);

                                if (id.Name == (nspace + "path"))
                                {
                                    string pathData = "";
                                    if (id.Attribute("d") != null)
                                        pathData = id.Attribute("d").Value;
                                    if (pathData == "")
                                    {
                                        if (id.Attribute("path") != null)
                                            pathData = id.Attribute("path").Value;
                                    }
                                    if (pathData != "")
                                    {
                                        string separators = @"(?=[A-Za-z-[e]])";
                                        var tokens = System.Text.RegularExpressions.Regex.Split(pathData, separators).Where(t => !string.IsNullOrEmpty(t));
                                        int objCount = 0;
                                        Logger.Trace("● ParseUse add path: {0}", id.ToString());
                                        Matrix tmpMatrix = matrixElement;
                                        matrixElement = Matrix.Multiply(lastTransformMatrix, matrixElement);

                                        foreach (string token in tokens)
                                            objCount += ParsePathCommand(token);

                                        Graphic.StopPath("if path has no z"); // if path has no z-command
                                        matrixElement = tmpMatrix;
                                    }
                                }
                                else
                                {
                                    //Logger.Trace("● ParseUse add basicE: {0}", id.ToString());
                                    ParseBasicElement(id, level, lastTransformMatrix, x, y);
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Parse stroke attributes
        /// </summary>
        private static string attributeStroke = "";
        private static string attributeFill = "";
        private static string attributeStrokeWidth = "";
        private static bool ParseAttributs(XElement element, bool isGroup = false)
        {
            if (isGroup) Graphic.SetDash(new double[0]);     // clear dash

            bool filterKeepColor = false;
            bool filterKeepWidth = false;

            attributeStroke = "";
            attributeFill = "";
            attributeStrokeWidth = "";
            if (element.Attribute("style") != null)
            {
                attributeStroke = GetStyleProperty(element, "stroke");
                logSource = "ParseAttributs: stroke: " + attributeStroke;
                if (attributeStroke.Length > 1)
                    filterKeepColor = Graphic.SetPenColor(attributeStroke.StartsWith("#") ? attributeStroke.Substring(1) : attributeStroke);

                attributeFill = GetStyleProperty(element, "fill");
                logSource = "ParseAttributs: fill: " + attributeFill;
                if ((attributeFill.Length > 1))// && (attributeFill != "none"))
                    Graphic.SetPenFill(attributeFill.StartsWith("#") ? attributeFill.Substring(1) : attributeFill);

                attributeStrokeWidth = GetStyleProperty(element, "stroke-width");
                logSource = "ParseAttributs: stroke-width: " + attributeStrokeWidth;
                if (attributeStrokeWidth.Length > 0)
                    filterKeepWidth = SetPenWidth(attributeStrokeWidth);

                SetDashPattern(GetStyleProperty(element, "stroke-dasharray"));
            }

            if (element.Attribute("stroke") != null)
            {
                attributeStroke = element.Attribute("stroke").Value;
                logSource = "ParseAttributs: stroke2: " + attributeStroke;
                filterKeepColor = Graphic.SetPenColor(attributeStroke.StartsWith("#") ? attributeStroke.Substring(1) : attributeStroke);
            }
            if (element.Attribute("fill") != null)
            {
                attributeFill = element.Attribute("fill").Value;
                logSource = "ParseAttributs: fill2: " + attributeFill;
                if ((attributeFill.Length > 1))// && (attributeFill != "none"))
                    Graphic.SetPenFill(attributeFill.StartsWith("#") ? attributeFill.Substring(1) : attributeFill);
            }
            if (element.Attribute("stroke-width") != null)
            {
                attributeStrokeWidth = element.Attribute("stroke-width").Value;
                logSource = "ParseAttributs: stroke-width2: " + attributeStrokeWidth;
                filterKeepWidth = SetPenWidth(attributeStrokeWidth);
            }
            if (element.Attribute("stroke-dasharray") != null)
            {
                SetDashPattern(element.Attribute("stroke-dasharray").Value);
            }

            globalTextProp.Update(element);
            if (Properties.Settings.Default.importGraphicFilterEnable)
                return (filterKeepColor || filterKeepWidth);
            return true;
        }

        private static bool SetPenWidth(string txt)
        {
            if (!double.TryParse(txt, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out double nr))
            {
                nr = ConvertToPixel(txt);   // = txt * 96
            }
            return Graphic.SetPenWidth(Math.Round(nr * svgStrokeWidthScale, 3).ToString().Replace(',', '.'));
        }

        /// <summary>
        /// Parse Transform information - more information here: http://www.w3.org/TR/SVG/coords.html
        /// transform will be applied in gcodeMove
        /// </summary>
        /// 
        private static Matrix ParseTransform(XElement element, bool isGroup, int level)
        {
            Matrix finalMatrix = new Matrix(1, 0, 0, 1, 0, 0); // m11, m12, m21, m22, offsetx, offsety
            if (element.Attribute("transform") != null)
            {
                // tranform elements must be processed in given order
                string[] separatingStrings = { "trans", "sca", "rot", "mat" };
                string transform = element.Attribute("transform").Value;
                //    if (logEnable) Logger.Trace("ParseTransform1: {0} ", transform);
                string[] words = transform.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (word.StartsWith("late")) { finalMatrix = Matrix.Multiply(ParseTransform("trans" + word), finalMatrix); }
                    else if (word.StartsWith("le")) { finalMatrix = Matrix.Multiply(ParseTransform("sca" + word), finalMatrix); }
                    else if (word.StartsWith("ate")) { finalMatrix = Matrix.Multiply(ParseTransform("rot" + word), finalMatrix); }
                    else if (word.StartsWith("rix")) { finalMatrix = Matrix.Multiply(ParseTransform("mat" + word), finalMatrix); }
                }
            }
            if (isGroup)
            {
                matrixGroup[level].SetIdentity();
                if (level > 0)
                {
                    for (int i = level; i < matrixGroup.Length; i++)
                    { matrixGroup[i] = Matrix.Multiply(finalMatrix, matrixGroup[level - 1]); }
                }
                else
                { matrixGroup[level] = finalMatrix; }
                matrixElement = matrixGroup[level];
            }
            else
            { matrixElement = Matrix.Multiply(finalMatrix, matrixGroup[level]); }

            svgStrokeWidthScale = matrixElement.M11;
            return finalMatrix;
            //    Logger.Info("ParseTransform {0}  isGroup:{1}", matrixElement.ToString(), isGroup);
        }
        private static Matrix ParseTransform(string transform)
        {
            Matrix tmp = new Matrix(1, 0, 0, 1, 0, 0); // m11, m12, m21, m22, offsetx, offsety
                                                       //    if (logEnable) Logger.Trace("ParseTransform2: {0} ", transform);

            logSource = "transform " + transform;
            if ((transform != null) && (transform.IndexOf("translate") >= 0))
            {
                var coord = GetTextBetween(transform, "translate(");//, ")");
                var split = coord.Split(',');
                if (coord.IndexOf(',') < 0)
                    split = coord.Split(' ');

                tmp.OffsetX = ConvertToPixel(split[0]);
                if (split.Length > 1)
                    tmp.OffsetY = ConvertToPixel(split[1].TrimEnd(')'));
                if (svgComments) Graphic.SetComment(string.Format(" SVG-Translate X:{0} Y:{1} ", tmp.OffsetX, tmp.OffsetY));
                if (logEnable) Logger.Trace("◯ SVG-Translate X:{0} Y:{1} ", tmp.OffsetX, tmp.OffsetY);
            }
            if ((transform != null) && (transform.IndexOf("scale") >= 0))
            {
                var coord = GetTextBetween(transform, "scale(");//, ")");
                var split = coord.Split(',');
                if (coord.IndexOf(',') < 0)
                    split = coord.Split(' ');
                tmp.M11 = ConvertToPixel(split[0]);
                if (split.Length > 1)
                { tmp.M22 = ConvertToPixel(split[1]); }
                else
                {
                    tmp.M11 = tmp.M22 = ConvertToPixel(coord);
                }
                if (svgComments) Graphic.SetComment(string.Format(" SVG-Scale {0} {1} ", tmp.M11, tmp.M22));
                if (logEnable) Logger.Trace("◯ SVG-Scale {0} {1} ", tmp.M11, tmp.M22);
            }
            if ((transform != null) && (transform.IndexOf("rotate") >= 0))
            {
                var coord = GetTextBetween(transform, "rotate(");//, ")");
                var split = coord.Split(',');
                if (coord.IndexOf(',') < 0)
                    split = coord.Split(' ');
                // change by arkypita LaserGRBL
                float angle = ConvertToPixel(split[0]); //no need to convert in radiant
                float px = split.Length == 3 ? ConvertToPixel(split[1]) : 0.0f; //<--- this read rotation offset point x
                float py = split.Length == 3 ? ConvertToPixel(split[2]) : 0.0f; //<--- this read rotation offset point y
                tmp.RotateAt(angle, px, py); // <--- this apply RotateAt matrix
                if (svgComments) Graphic.SetComment(string.Format(" SVG-Rotate {0} ", angle));
                if (logEnable) Logger.Trace("◯ SVG-Rotate {0} ", angle);
            }
            if ((transform != null) && (transform.IndexOf("matrix") >= 0))
            {
                var coord = GetTextBetween(transform, "matrix(");//, ")");
                var split = coord.Split(',');
                if (coord.IndexOf(',') < 0)
                    split = coord.Split(' ');
                tmp.M11 = ConvertToPixel(split[0]);     // a    scale x         a c e
                tmp.M12 = ConvertToPixel(split[1]);     // b                    b d f
                tmp.M21 = ConvertToPixel(split[2]);     // c                    0 0 1
                tmp.M22 = ConvertToPixel(split[3]);     // d    scale y
                tmp.OffsetX = ConvertToPixel(split[4]); // e    offset x
                tmp.OffsetY = ConvertToPixel(split[5]); // f    offset y
                if (svgComments) Graphic.SetComment(string.Format(" SVG-Matrix {0} ", coord.Replace(',', '|')));
                if (logEnable) Logger.Trace("◯ SVG-Matrix {0} ", coord.Replace(',', '|'));
            }
            return tmp;
        }

        private static string GetTextBetween(string source, string s1)//, string s2)
        {
            int start = source.IndexOf(s1) + s1.Length;
            char c;
            for (int i = start; i < source.Length; i++)
            {
                c = source[i];
                if (!(Char.IsNumber(c) || c == '.' || c == ',' || c == ' ' || c == '-' || c == 'e'))    // also exponent
                    return source.Substring(start, i - start);
            }
            return source.Substring(start, source.Length - start - 1);
        }

        private static float ConvertToPixel(string str, float ext = 1)        // return value in px
        {       // https://www.w3.org/TR/SVG/coords.html#Units          // in=90 or 96 ???
            bool percent = false;
            //       Logger.Trace( "convert to pixel in {0}", str);
            float factor = 1;   // no unit = px
            if (str.IndexOf("mm") > 0) { factor = factor_Mm2Px; }               // Millimeter
            else if (str.IndexOf("cm") > 0) { factor = factor_Cm2Px; }          // Centimeter
            else if (str.IndexOf("in") > 0) { factor = factor_In2Px; }          // Inch    72, 90 or 96?
            else if (str.IndexOf("pt") > 0) { factor = factor_Pt2Px; }          // Point
            else if (str.IndexOf("pc") > 0) { factor = factor_Pc2Px; }          // Pica
            else if (str.IndexOf("em") > 0) { factor = factor_Em2Px; }          // Font size
            else if (str.IndexOf("%") > 0) { percent = true; }
            str = str.Replace("pt", "").Replace("pc", "").Replace("mm", "").Replace("cm", "").Replace("in", "").Replace("em", "").Replace("%", "").Replace("px", "");
            double test;
            if (str.Length > 0)
            {
                if (percent)
                {
                    if (double.TryParse(str, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out test))
                    { return ((float)test * ext / 100); }
                }
                else
                {
                    if (double.TryParse(str, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out test))
                    { return ((float)test * factor); }
                }
            }
            Logger.Error("convertToPixel source '{0}' ", logSource);
            Logger.Error("convertToPixel TryParse failed '{0}' ", str);
            Graphic.SetHeaderInfo(string.Format(" !!! Error: convert to float, string is: '{0}'", str));
            ConversionInfo += string.Format("Error: convert to float, string is: '{0}' ", str);
            logSource = "-";
            return 0f;
        }

        private static string GetStyleProperty(XElement pathElement, string property)
        {
            if (pathElement.Attribute("style") != null)
            {
                string style = pathElement.Attribute("style").Value;
                string[] prop = style.Split(';');
                foreach (string propitem in prop)
                {
                    string[] keyval = propitem.Split(':');
                    if ((keyval.Length > 1) && (propitem.Contains(property + ":")))   // to avoid reading "stroke-linecap" as "stroke"                
                                                                                      // if ((keyval.Length >1) && (keyval[0].Contains(property)))
                        return keyval[1];
                }
            }
            return "";
        }
        private static void SetDashPattern(string dasharray)
        {
            try
            {
                if (dasharray.Length > 2)
                {
                    if (dasharray.Contains("none"))
                    { Graphic.SetDash(new double[0]); }
                    else
                    {
                        string[] pattern;
                        if (dasharray.Contains(',')) { pattern = dasharray.Split(','); }
                        else { pattern = dasharray.Split(' '); }

                        //          double tmp;
                        double[] dash = Array.ConvertAll(pattern,
                               s => double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out double tmp) ? tmp : 0);    //(tmp * (double)gcodeScale) : 0);
                        if (logEnable) Logger.Debug("SetDashPattern:{0}", dasharray);

                        for (int k = 0; k < dash.Length; k++)
                        { dash[k] = dash[k] / factor_Mm2Px; }   // convert px to mm

                        Graphic.SetDash(dash);
                    }
                }
            }
            catch (Exception er)
            { Logger.Error(er, "dasharray: {0}", dasharray); }
        }


        /// <summary>
        /// Convert Basic shapes (up to now: line, rect, circle) check: http://www.w3.org/TR/SVG/shapes.html
        /// </summary>
        private static void ParseBasicElements(XElement svgCode, int level)
        {
            string[] forms = { "rect", "circle", "ellipse", "line", "polyline", "polygon", "text", "image" };

            foreach (var form in forms)
            {
                foreach (var pathElement in svgCode.Elements(nspace + form))
                {
                    ParseBasicElement(pathElement, level);
                }
            }
            return;
        }
        private static void ParseBasicElement(XElement pathElement, int level, Matrix extMatrix = new Matrix(), float x = 0, float y = 0)
        {
            string form = pathElement.Name.ToString().Substring(nspace.NamespaceName.Length + 2);
            string elementText = pathElement.ToString();
            elementText = elementText.Substring(0, Math.Min(80, elementText.Length));

            //    Logger.Info("►►► ParseBasicElement {0}", elementText);

            string pathElementString = pathElement.ToString();
            pathElementString = pathElementString.Substring(0, Math.Min(100, pathElementString.Length));

            if (pathElement != null)
            {
                if (logEnable) Logger.Trace("►►► ParseBasicElement: '{0}' Level:{1}", form, level);
                shapeCounter++;

                oldMatrixElement = matrixElement;
                lastTransformMatrix = ParseTransform(pathElement, false, level);  // transform will be applied in gcodeMove
                matrixElement = Matrix.Multiply(extMatrix, matrixElement);

                ParseAttributs(pathElement);                // process color and stroke-dasharray

                logSource = "Basic element " + form;

                string attrId = "";
                if (pathElement.Attribute("id") != null)
                {
                    attrId = pathElement.Attribute("id").Value;
                    Graphic.SetPathId(attrId);
                }

                offsetX = 0; offsetY = 0;

                float x1 = 0, y1 = 0, x2 = 0, y2 = 0, width = 0, height = 0, rx = 0, ry = 0, cx = 0, cy = 0, r = 0;
                string[] points = { "" };

                /* skip element if not visible */
                string visibility = "";
                if (pathElement.Attribute("visibility") != null) { visibility = pathElement.Attribute("visibility").Value; }
                if (Properties.Settings.Default.importSVGDontPlot && visibility.Contains("hidden"))
                {
                    Graphic.SetHeaderInfo(string.Format(" Hide SVG Element:{0}   Id:{1}", form, attrId));
                    Graphic.SetHeaderMessage(string.Format(" {0}-1102: SVG Element '{1}' is not visible and will not be imported", CodeMessage.Attention, form));
                    return; // continue;
                }

                logSource = "ParseBasicElements";
                if (form == "text")
                {
                    ParseText(pathElement, level);
                    return; // continue;
                }
                if (pathElement.Attribute("x") != null) x = ConvertToPixel(pathElement.Attribute("x").Value);
                if (pathElement.Attribute("y") != null) y = ConvertToPixel(pathElement.Attribute("y").Value);
                if (pathElement.Attribute("x1") != null) x1 = ConvertToPixel(pathElement.Attribute("x1").Value);
                if (pathElement.Attribute("y1") != null) y1 = ConvertToPixel(pathElement.Attribute("y1").Value);
                if (pathElement.Attribute("x2") != null) x2 = ConvertToPixel(pathElement.Attribute("x2").Value);
                if (pathElement.Attribute("y2") != null) y2 = ConvertToPixel(pathElement.Attribute("y2").Value);
                if (pathElement.Attribute("width") != null) width = ConvertToPixel(pathElement.Attribute("width").Value, (float)svgWidthPx);
                if (pathElement.Attribute("height") != null) height = ConvertToPixel(pathElement.Attribute("height").Value, (float)svgHeightPx);
                if (pathElement.Attribute("rx") != null) rx = ConvertToPixel(pathElement.Attribute("rx").Value);
                if (pathElement.Attribute("ry") != null) ry = ConvertToPixel(pathElement.Attribute("ry").Value);
                if (pathElement.Attribute("cx") != null) cx = ConvertToPixel(pathElement.Attribute("cx").Value);
                if (pathElement.Attribute("cy") != null) cy = ConvertToPixel(pathElement.Attribute("cy").Value);
                if (pathElement.Attribute("r") != null) r = ConvertToPixel(pathElement.Attribute("r").Value);
                if (pathElement.Attribute("points") != null)
                {
                    points = pathElement.Attribute("points").Value.Split(' ');
                    if (points.Length == 1)     // not separated by ' '
                    {
                        string[] values = pathElement.Attribute("points").Value.Split(',');
                        List<string> list = new List<string>();
                        for (int i = 0; i < values.Length; i += 2)
                        { list.Add(values[i] + "," + values[i + 1]); }
                        points = list.ToArray();
                    }
                }

                if (form == "rect")
                {
                    if (ry == 0) { ry = rx; }
                    else if (rx == 0) { rx = ry; }
                    else if (rx != ry) { rx = Math.Min(rx, ry); ry = rx; }
                    if (logEnable) Logger.Trace(" SVG-Rect x:{0:0.00} y:{1:0.00} width:{2:0.00} height:{3:0.00} rx:{4:0.00} ry:{5:0.00}", x, y, width, height, rx, ry);
                    x += offsetX; y += offsetY;

                    if (getTextPath)
                    {
                        var arcBounds = new System.Drawing.RectangleF
                        {
                            Width = rx,
                            Height = ry
                        };

                        textOnPath.StartFigure();
                        textOnPath.AddLine(x + rx, y + height, x + width - rx, y + height);
                        if (rx > 0)
                        {
                            arcBounds.Location = new System.Drawing.PointF(x + width, y + height);
                            textOnPath.AddArc(arcBounds, 270, 90);
                        }
                        textOnPath.AddLine(x + width, y + height - ry, x + width, y + ry);
                        if (rx > 0)
                        {
                            arcBounds.Location = new System.Drawing.PointF(x + width, y + height);
                            textOnPath.AddArc(arcBounds, 0, 90);
                        }
                        textOnPath.AddLine(x + width - rx, y, x + rx, y);
                        if (rx > 0)
                        {
                            arcBounds.Location = new System.Drawing.PointF(x + width, y + height);
                            textOnPath.AddArc(arcBounds, 90, 90);
                        }
                        textOnPath.AddLine(x, y + ry, x, y + height - ry);
                        if (rx > 0)
                        {
                            arcBounds.Location = new System.Drawing.PointF(x + width, y + height);
                            textOnPath.AddArc(arcBounds, 180, 90);
                        }
                        textOnPath.CloseFigure();
                    }
                    else
                    {
                        if (svgComments) Graphic.SetComment(string.Format(" SVG-Rect x:{0:0.00} y:{1:0.00} width:{2:0.00} height:{3:0.00} rx:{4:0.00} ry:{5:0.00}", x, y, width, height, rx, ry));
                        {
                            SVGStartPath(x + rx, y + height, form);
                            SVGMoveTo(x + width - rx, y + height, form + " a1");
                            if (rx > 0) SVGArcToCCW(x + width, y + height - ry, 0, -ry, form);  // +ry
                            SVGMoveTo(x + width, y + ry, form + " b1");                         // upper right
                            if (rx > 0) SVGArcToCCW(x + width - rx, y, -rx, 0, form);
                            SVGMoveTo(x + rx, y, form + " a2");                                 // upper left
                            if (rx > 0) SVGArcToCCW(x, y + ry, 0, ry, form);                    // -ry
                            SVGMoveTo(x, y + height - ry, form + " b2");                        // lower left
                            if (rx > 0)
                            {
                                SVGArcToCCW(x + rx, y + height, rx, 0, form);
                                SVGMoveTo(x + rx, y + height, form);  // repeat first point to avoid back draw after last G3
                            }
                        }
                        Graphic.StopPath(form);//Plotter.StopPath(form);
                    }
                }
                else if (form == "circle")
                {
                    if (logEnable) Logger.Trace(" circle cx:{0:0.00} cy:{1:0.00} r:{2:0.00} r=z:{3}", cx, cy, r, svgConvertCircleToDot);

                    if (getTextPath)
                    {
                        textOnPath.StartFigure();
                        textOnPath.AddEllipse(cx - r, cy - r, 2 * r, 2 * r);
                        textOnPath.CloseFigure();
                    }
                    else
                    {
                        if (svgComments) Graphic.SetComment(string.Format(" circle cx:{0:0.00} cy:{1:0.00} r:{2:0.00} ", cx, cy, r));
                        if (svgConvertCircleToDot)
                        {
                            if (Properties.Settings.Default.importSVGCircleToDotZ)
                                GCodeDotOnlyWithZ(cx, cy, r, "Dot r=Z");
                            else
                                GCodeDotOnly(cx, cy, "Dot");
                        }
                        else
                        {
                            cx += offsetX; cy += offsetY;
                            if (!svgNodesOnly)
                            {
                                SVGStartPath(cx + r, cy, form);
                                SVGArcToCCW(cx + r, cy, -r, 0, form);
                            }
                            else
                            {
                                GCodeDotOnly(cx, cy, form);
                            }
                        }
                        Graphic.StopPath(form);
                    }
                }
                else if (form == "ellipse")
                {
                    if (logEnable) Logger.Trace(" ellipse cx:{0:0.00} cy:{1:0.00} rx:{2:0.00}  ry:{3:0.00}", cx, cy, rx, ry);
                    cx += offsetX; cy += offsetY;

                    if (getTextPath)
                    {
                        textOnPath.StartFigure();
                        textOnPath.AddEllipse(cx - rx, cy - ry, 2 * rx, 2 * ry);
                        textOnPath.CloseFigure();
                    }
                    else
                    {
                        if (svgComments) Graphic.SetComment(string.Format(" ellipse cx:{0:0.00} cy:{1:0.00} rx:{2:0.00}  ry:{3:0.00}", cx, cy, rx, ry));
                        SVGStartPath(cx + rx, cy, form);
                        ImportMath.CalcArc(cx + rx, cy, rx, ry, 0, 1, 1, cx - rx, cy, SVGMoveTo);
                        ImportMath.CalcArc(cx - rx, cy, rx, ry, 0, 1, 1, cx + rx, cy, SVGMoveTo);
                        Graphic.StopPath(form);
                    }
                }
                else if (form == "line")
                {
                    if (logEnable) Logger.Trace(" SVG-Line x1:{0:0.00} y1:{1:0.00} x2:{2:0.00} y2:{3:0.00} ", x1, y1, x2, y2);
                    x1 += offsetX; y1 += offsetY;

                    if (getTextPath)
                    {
                        textOnPath.StartFigure();
                        textOnPath.AddLine(x1, y1, x2, y2);
                        textOnPath.CloseFigure();
                    }
                    else
                    {
                        if (svgComments) Graphic.SetComment(string.Format(" SVG-Line x1:{0:0.00} y1:{1:0.00} x2:{2:0.00} y2:{3:0.00} ", x1, y1, x2, y2));
                        if (!svgNodesOnly)
                        {
                            SVGStartPath(x1, y1, form);
                            SVGMoveTo(x2, y2, form);
                        }
                        else
                        {
                            GCodeDotOnly(x1, y1, form);
                            GCodeDotOnly(x2, y2, form);
                        }
                        Graphic.StopPath(form);
                    }
                }
                else if ((form == "polyline") || (form == "polygon"))
                {
                    x1 = -1; y1 = -1;

                    if (getTextPath)
                    {
                        textOnPath.StartFigure();
                        for (int index = 0; index < points.Length; index++)
                        {
                            if (points[index].IndexOf(",") >= 0)
                            {
                                string[] coord = points[index].Split(',');
                                x = ConvertToPixel(coord[0]) + offsetX; y = ConvertToPixel(coord[1]) + offsetY;

                                if (index == 0)
                                {
                                    x1 = x; y1 = y;
                                }
                                else
                                {
                                    textOnPath.AddLine(x1, y1, x, y);
                                }
                            }
                        }
                        if (form == "polygon")
                        {
                            textOnPath.AddLine(x, y, x1, y1);
                        }
                        textOnPath.CloseFigure();
                    }
                    else
                    {
                        if (svgComments) Graphic.SetComment(" SVG-Polyline ");
                        for (int index = 0; index < points.Length; index++)
                        {
                            if (points[index].IndexOf(",") >= 0)
                            {
                                string[] coord = points[index].Split(',');
                                x = ConvertToPixel(coord[0]) + offsetX; y = ConvertToPixel(coord[1]) + offsetY;

                                if (index == 0)
                                {
                                    x1 = x; y1 = y;
                                    if (!svgNodesOnly)
                                        SVGStartPath(x, y, form);           // move to
                                    else
                                        GCodeDotOnly(x, y, form);
                                }
                                else
                                {
                                    if (!svgNodesOnly)
                                        SVGMoveTo(x, y, form);              // line to
                                    else
                                        GCodeDotOnly(x, y, form);
                                }
                            }
                        }
                        if (form == "polygon")
                        {
                            if (!svgNodesOnly)
                                SVGMoveTo(x1, y1, form);
                        }
                        Graphic.StopPath(form);
                    }
                }
                else if (form == "text")
                {
                    if (true)   // text will be processed at the beginning
                    {
                        string tmp = "Error: Text is not supported, convert Object to Path first. ";
                        ConversionInfo += tmp;
                        Logger.Error(tmp);
                        Graphic.SetHeaderInfo(tmp);
                    }
                }
                else if (form == "image")
                {
                    {
                        string tmp = "Error: Image is not supported. ";
                        ConversionInfo += tmp;
                        Logger.Error(tmp);
                        Graphic.SetHeaderInfo(tmp);
                        Graphic.SetHeaderMessage(string.Format(" {0}-1103: Not supported SVG element: Image", CodeMessage.Warning));
                    }
                    shapeCounter--;
                }
                else
                {
                    if (svgComments) Graphic.SetComment(" ++++++ Unknown Shape: " + form);
                    ConversionInfo += string.Format("Error: unknown shape: '{0}' ", form);
                    Graphic.SetHeaderInfo(string.Format("Error: unknown shape: '{0}' ", form));
                    Graphic.SetHeaderMessage(string.Format(" {0}-1104: Unknown SVG element: {1}", CodeMessage.Warning, form));
                }
                matrixElement = oldMatrixElement;
            }
        }

        /// <summary>
        /// Convert all Path commands, check: http://www.w3.org/TR/SVG/paths.html
        /// Split command tokens
        /// </summary>
		private static string lastPathInformation = "";		// stringbuilder?
        private static void ParsePath(XElement svgCode, int level)
        {
            if (logEnable) Logger.Debug("►►►► ParsePath Level:{0} Elements:{1}", level, svgCode.Elements(nspace + "path").Count());

            int maxCount = svgCode.Elements(nspace + "path").Count();
            int pathCount = 0;
            foreach (var pathElement in svgCode.Elements(nspace + "path"))
            {
                if (backgroundWorker != null)
                {
                    backgroundWorker.ReportProgress(pathCount++ * 100 / maxCount);
                    if (backgroundWorker.CancellationPending)
                    {
                        backgroundEvent.Cancel = true;
                        break;
                    }
                }
                if (pathElement != null)
                {
                    offsetX = 0;
                    offsetY = 0;
                    currentX = offsetX; currentY = offsetX;
                    firstX = null; firstY = null;
                    startPath = true;
                    lastX = offsetX; lastY = offsetY;
                    string d = pathElement.Attribute("d").Value;

                    string id = d.Replace("\n", "");
                    if (id.Length > 20)
                        id = id.Substring(0, 20);

                    lastPathInformation = "";
                    string attrId = "";
                    oldMatrixElement = matrixElement;
                    lastTransformMatrix = ParseTransform(pathElement, false, level);    // transform will be applied in gcodeMove

                    if (!ParseAttributs(pathElement))   // process color and stroke-dasharray
                    {
                        skipCounter++;
                        continue;
                    }
                    shapeCounter++;

                    //    Logger.Trace("ParseAttributs {0}", pathElement);

                    if (pathElement.Attribute("id") != null)
                    {
                        attrId = pathElement.Attribute("id").Value;
                        Graphic.SetPathId(attrId);
                        lastPathInformation = "id=" + attrId;
                    }

                    /* skip element if not visible */
                    string visibility = "";
                    if (pathElement.Attribute("visibility") != null) { visibility = pathElement.Attribute("visibility").Value; }
                    if (Properties.Settings.Default.importSVGDontPlot && visibility.Contains("hidden"))
                    {
                        Graphic.SetHeaderInfo(string.Format(" Hide SVG Path Id:{0}", attrId));
                        Graphic.SetHeaderMessage(string.Format(" {0}-1102: SVG Path-Id '{1}' is not visible and will not be imported", CodeMessage.Attention, attrId));
                        continue;
                    }

                    if (d.Length > 0)
                    {
                        // split complete path in to command-tokens
                        if (logEnable) Logger.Trace("►►► Path d {0} .....", d.Substring(0, Math.Min(100, d.Length)));
                        lastPathInformation += "  d=" + d.Substring(0, Math.Min(100, d.Length));
                        string separators = @"(?=[A-Za-z-[e]])";
                        var tokens = Regex.Split(d, separators).Where(t => !string.IsNullOrEmpty(t));
                        int objCount = 0;
                        foreach (string token in tokens)
                            objCount += ParsePathCommand(token);

                        Graphic.StopPath("if path has no z"); // if path has no z-command
                    }
                    matrixElement = oldMatrixElement;
                }
            }
            return;
        }

        private static bool startPath = true;
        private static float offsetX, offsetY;
        private static float currentX, currentY;
        private static float? firstX, firstY;
        private static float lastX, lastY;
        private static Vector cMirror = new Vector();

        /// <summary>
        /// Convert all Path commands, check: http://www.w3.org/TR/SVG/paths.html
        /// Convert command tokens
        /// </summary>
        private static int ParsePathCommand(string svgPath) // process single command 'M' or 'V', etc. with it's coordinate
        {
            if (String.IsNullOrEmpty(svgPath))      // LaserGRBL Issue #1969 
                return 0;
            var command = svgPath.Take(1).Single();	// System.IndexOutOfRangeException: Index was outside the bounds of the array.
            logSource = "path " + svgPath;
            char cmd = char.ToUpper(command);
            bool absolute = (cmd == command);
            string remainingargs = svgPath.Substring(1);

            string argSeparators = @"[\s,]|(?=(?<!e)-)";
            var splitArgs = Regex
                .Split(remainingargs, argSeparators)
                .Where(t => !string.IsNullOrEmpty(t)).ToList();

            double testNr;
            for (int iArg = 0; iArg < splitArgs.Count(); iArg++)
            {
                if (splitArgs[iArg].Count(f => (f == '.')) > 1)
                {
                    Logger.Error("ParsePathCommand found bad argument:{0} at {1}", splitArgs[iArg], iArg);  // Example to fix: '68.93206.00002'
                    var strSplit = splitArgs[iArg].Split('.');
                    if (strSplit.Length > 2)
                    {
                        splitArgs[iArg] = "0." + strSplit[strSplit.Length-1];
                        for (int k = strSplit.Length - 2; k > 1; k--)
                        {
                            splitArgs.Insert(iArg, "0." + strSplit[k]);
                        //    Logger.Info("ParsePathCommand fixed bad arguments to:{0} and {1}", splitArgs[iArg], splitArgs[iArg + 1]);
                        }
                        splitArgs.Insert(iArg, strSplit[0] + "." + strSplit[1]);
                     //   Logger.Info("ParsePathCommand fixed bad arguments to:{0} and {1}", splitArgs[iArg], splitArgs[iArg + 1]);
                    }
                }
            }

            float[] floatArgs = splitArgs.Select(arg => ConvertToPixel(arg)).ToArray();

            int objCount = 0;

            switch (cmd)
            {
                case 'M':       // Start a new sub-path at the given 1st (x,y) coordinate, and lineTo next pairs
                    #region Move
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        if (floatArgs.Length < (i + 2))
                        {
                            Logger.Error("Move to command needs 2 arguments '{0}'  {1}", svgPath, lastPathInformation);
                            Graphic.SetHeaderInfo(" !!! Error: Move to command needs 2 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (absolute || startPath)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = floatArgs[i] + lastX; currentY = floatArgs[i + 1] + lastY; }

                        if (i == 0) // first two coordinates are start of path
                        {
                            firstX = currentX; firstY = currentY;

                            if (!startPath && Gcode.IsEqual(lastX, currentX) && Gcode.IsEqual(lastY, currentY)) // found a 2nd 'M' or 'm' command within 'd' path		
                            {
                                if (skip2ndCounter++ < 10)
                                    Logger.Warn("⚠ Skip 2nd 'm' because of no position change: {0}", lastPathInformation);
                            }
                            else
                            {
                                if (svgComments) { Graphic.SetComment(string.Format(" Start new subpath at {0} {1} ", floatArgs[i], floatArgs[i + 1])); }
                                if (svgNodesOnly)
                                    GCodeDotOnly(currentX, currentY, (command.ToString()));
                                else
                                    SVGStartPath(currentX, currentY, "path", startPath);

                                startPath = false;
                            }
                        }
                        else
                        {
                            if (svgNodesOnly)
                                GCodeDotOnly(currentX, currentY, command.ToString());
                            else
                                SVGMoveTo(currentX, currentY, string.Format("{0} {1,3})  X:{2:0.00} Y:{3:0.00}", command, (i / 2), currentX, currentY));  	// G1
                        }

                        lastX = currentX; lastY = currentY;
                        cMirror = new Vector(lastX, lastY);
                    }

                    break;
                #endregion
                case 'Z':       // Close the current subpath
                    #region ZClose
                    if (!svgNodesOnly)
                    {
                        if (firstX == null) { firstX = currentX; }
                        if (firstY == null) { firstY = currentY; }
                        if (!Gcode.IsEqual((float)firstX, lastX) || !Gcode.IsEqual((float)firstY, lastY))
                            SVGMoveTo((float)firstX, (float)firstY, string.Format("{0} X:{1:0.00} Y:{2:0.00}", command, firstX, firstY));// (isNearlyEqual((float)firstX, lastX).ToString()+"  "+ firstX.ToString()+" "+lastX.ToString()+"  "+ firstY.ToString() + " " + lastY.ToString()));// 
                    }
                    lastX = (float)firstX; lastY = (float)firstY;
                    firstX = null; firstY = null;
                    Graphic.StopPath("Z");//Plotter.StopPath("Z");
                    break;
                #endregion
                case 'L':       // Draw a line from the current point to the given (x,y) coordinate
                    #region Line
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        if (floatArgs.Length < (i + 2))
                        {
                            Logger.Error("Line to command needs 2 arguments '{0}'", svgPath);
                            Graphic.SetHeaderInfo(" !!! Error: Line to command needs 2 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY + floatArgs[i + 1]; }
                        if (svgNodesOnly)
                            GCodeDotOnly(currentX, currentY, command.ToString());
                        else
                            SVGMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cMirror = new Vector(lastX, lastY);
                    }
                    break;
                #endregion
                case 'H':       // Draws a horizontal line from the current point (cpx, cpy) to (x, cpy)
                    #region Horizontal
                    for (int i = 0; i < floatArgs.Length; i++)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = lastY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY; }
                        if (svgNodesOnly)
                            GCodeDotOnly(currentX, currentY, command.ToString());
                        else
                            SVGMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cMirror = new Vector(lastX, lastY);
                    }
                    break;
                #endregion
                case 'V':       // Draws a vertical line from the current point (cpx, cpy) to (cpx, y)
                    #region Vertical
                    for (int i = 0; i < floatArgs.Length; i++)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = lastX; currentY = floatArgs[i] + offsetY; }
                        else
                        { currentX = lastX; currentY = lastY + floatArgs[i]; }
                        if (svgNodesOnly)
                            GCodeDotOnly(currentX, currentY, command.ToString());
                        else
                            SVGMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cMirror = new Vector(lastX, lastY);
                    }
                    break;
                #endregion
                case 'A':       // Draws an elliptical arc from the current point to (x, y)
                    #region Arc
                    if (svgComments) { Graphic.SetComment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 7)
                    {
                        if (floatArgs.Length < (rep + 7))
                        {
                            Logger.Error("Elliptical arc curve command needs 7 arguments {0}  {1}  '{2}'", rep, floatArgs.Length, svgPath);
                            Graphic.SetHeaderInfo(" !!! Error: Elliptical arc curve command needs 7 arguments '" + svgPath + "'");
                            break;
                        }

                        objCount++;
                        if (svgComments) { Graphic.SetComment(string.Format(" draw arc nr. {0} ", (1 + rep / 6))); }
                        float rx, ry, rot, large, sweep, nx, ny;
                        rx = floatArgs[rep]; ry = floatArgs[rep + 1];
                        rot = floatArgs[rep + 2];
                        large = floatArgs[rep + 3];
                        sweep = floatArgs[rep + 4];
                        if (absolute)
                        { nx = floatArgs[rep + 5] + offsetX; ny = floatArgs[rep + 6] + offsetY; }
                        else
                        { nx = floatArgs[rep + 5] + lastX; ny = floatArgs[rep + 6] + lastY; }
                        if (svgNodesOnly)
                            GCodeDotOnly(currentX, currentY, command.ToString());
                        else
                        {
                            if (svgConvertCircleToDot && (rx == ry))
                            {
                                if (Properties.Settings.Default.importSVGCircleToDotZ)
                                    GCodeDotOnlyWithZ(currentX, currentY, rx, "Dot r=Z");
                                else
                                    GCodeDotOnly(currentX, currentY, "Dot");
                            }
                            else
                            { ImportMath.CalcArc(lastX, lastY, rx, ry, rot, large, sweep, nx, ny, SVGMoveTo); }
                        }
                        lastX = nx; lastY = ny;
                        cMirror = new Vector(lastX, lastY);
                    }
                    break;
                #endregion
                case 'C':       // Draws a cubic Bézier curve from the current point to (x,y)
                    #region Cubic
                    // relative:      d="m 100,200 c  0,-100 150,-100 150,0  |  0,100  150,100 150,0"
                    // absolute:      d="M 100,200 C 100,100 250,100 250,200 | 250,300 400,300 400,200"
                    if (svgComments) { Graphic.SetComment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }

                    Point Off = new Point(offsetX, offsetY);
                    if (!absolute)
                        Off = new Point(lastX, lastY);

                    for (int rep = 0; rep < floatArgs.Length; rep += 6)
                    {
                        if (floatArgs.Length < (rep + 6))
                        {
                            Logger.Error("Cubic Bézier curve command needs 6 arguments '{0}'", svgPath);
                            Graphic.SetHeaderInfo(" !!! Error: Cubic Bézier curve command needs 6 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Graphic.SetComment(string.Format(" draw curve nr. {0} ", (1 + rep / 6))); }

                        if ((rep + 5) < floatArgs.Length)
                        {
                            Vector c1 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                            Vector c2 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;
                            Vector c3 = new Vector(floatArgs[rep + 4], floatArgs[rep + 5]) + (Vector)Off;
                            if (logEnable)
                            {
                                Logger.Trace("CalcCubicBezier  0: X:{0:0.000}   Y:{1:0.000}", lastX, lastY);
                                Logger.Trace("CalcCubicBezier c1: X:{0:0.000}   Y:{1:0.000}", c1.X, c1.Y);
                                Logger.Trace("CalcCubicBezier c2: X:{0:0.000}   Y:{1:0.000}", c2.X, c2.Y);
                                Logger.Trace("CalcCubicBezier c3: X:{0:0.000}   Y:{1:0.000}", c3.X, c3.Y);
                            }
                            if (svgNodesOnly)
                                GCodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                            else
                                ImportMath.CalcCubicBezier(new Point(lastX, lastY), (Point)c1, (Point)c2, (Point)c3, SVGMoveTo, command.ToString());

                            lastX = (float)c3.X; lastY = (float)c3.Y;

                            if (!absolute)  // 08.09.2023
                                Off = new Point(lastX, lastY);

                            cMirror = c3 * 2 - c2;
                        }
                        else
                        { Graphic.SetComment(string.Format(" Missing argument after {0} ", rep)); }
                    }
                    break;
                #endregion
                case 'S':       // Draws a cubic Bézier curve from the current point to (x,y)
                    #region Small Cubic
                    if (svgComments) { Graphic.SetComment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }

                    Off = new Point(offsetX, offsetY);
                    if (!absolute)
                        Off = new Point(lastX, lastY);

                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        if (floatArgs.Length < (rep + 4))
                        {
                            Logger.Error("Smooth curveto command needs 4 arguments '{0}'", svgPath);
                            Graphic.SetHeaderInfo(" !!! Error: smooth curveto command needs 4 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Graphic.SetComment(string.Format(" draw curve nr. {0} ", (1 + rep / 4))); }

                        Vector c2 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                        Vector c3 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;

                        if (svgNodesOnly)
                            GCodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                        else
                            ImportMath.CalcCubicBezier(new Point(lastX, lastY), (Point)cMirror, (Point)c2, (Point)c3, SVGMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;

                        if (!absolute)  // 08.09.
                            Off = new Point(lastX, lastY);

                        cMirror = c3 * 2 - c2;
                    }
                    break;
                #endregion
                case 'Q':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    #region Quadratic
                    if (svgComments) { Graphic.SetComment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }

                    Off = new Point(offsetX, offsetY);
                    if (!absolute)
                        Off = new Point(lastX, lastY);

                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        if (floatArgs.Length < (rep + 4))
                        {
                            Logger.Error("Quadratic Bézier curveto command needs 4 arguments '{0}'", svgPath);
                            Graphic.SetHeaderInfo(" !!! Error: Quadratic Bézier curveto command needs 4 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Graphic.SetComment(string.Format(" draw curve nr. {0} ", (1 + rep / 4))); }

                        Vector c2 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                        Vector c3 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;

                        if (svgNodesOnly)
                            GCodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                        else
                            ImportMath.CalcQuadraticBezier(new Point(lastX, lastY), (Point)c2, (Point)c3, SVGMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;

                        if (!absolute)  // 08.09.
                            Off = new Point(lastX, lastY);

                        cMirror = c3 * 2 - c2;
                    }
                    break;
                #endregion
                case 'T':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    #region TQuadratic
                    if (svgComments) { Graphic.SetComment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }

                    Off = new Point(offsetX, offsetY);
                    if (!absolute)
                        Off = new Point(lastX, lastY);

                    for (int rep = 0; rep < floatArgs.Length; rep += 2)
                    {
                        if (floatArgs.Length < (rep + 2))
                        {
                            Logger.Error("Smooth quadratic Bézier curveto command needs 2 arguments '{0}'", svgPath);
                            Graphic.SetHeaderInfo(" !!! Error: Smooth quadratic Bézier curveto command needs 2 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Graphic.SetComment(string.Format(" draw curve nr. {0} ", (1 + rep / 2))); }

                        Vector c3 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;

                        if (svgNodesOnly)
                            GCodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                        else
                            ImportMath.CalcQuadraticBezier(new Point(lastX, lastY), (Point)cMirror, (Point)c3, SVGMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;

                        cMirror = c3;
                    }
                    break;
                #endregion
                case ' ':
                    if (logEnable) Logger.Trace("ParsePathCommand Element ' ', nothing to do floatArgs.Length:{0}", floatArgs.Length);
                    break;
                default:
                    if (svgComments) Graphic.SetComment(" *********** unknown: " + command.ToString() + " ***** ");
                    Graphic.SetHeaderInfo(" Unknown SVG element: '" + command.ToString() + "'");
                    Graphic.SetHeaderMessage(string.Format(" {0}-1101: Unknown SVG path element: {1}", CodeMessage.Attention, command.ToString()));
                    break;
            }
            return objCount;
        }



        /*****************************************************************************************************************************************/
        private static int ParsePathCommandGraphicsPath(string svgPath) // process single command 'M' or 'V', etc. with it's coordinate
        {
            // https://github.com/svg-net/SVG/blob/master/Source/Paths/SvgQuadraticCurveSegment.Drawing.cs

            var command = svgPath.Take(1).Single();
            logSource = "path " + svgPath;
            char cmd = char.ToUpper(command);
            bool absolute = (cmd == command);
            string remainingargs = svgPath.Substring(1);
            string argSeparators = @"[\s,]|(?=(?<!e)-)";
            var splitArgs = Regex
                .Split(remainingargs, argSeparators)
                .Where(t => !string.IsNullOrEmpty(t));

            float[] floatArgs = splitArgs.Select(arg => ConvertToPixel(arg)).ToArray();

            int objCount = 0;

            switch (cmd)
            {
                case 'M':       // Start a new sub-path at the given 1st (x,y) coordinate, and lineTo next pairs
                    #region Move
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        if (floatArgs.Length < (i + 2))
                        {
                            Logger.Error("ParsePathCommandGraphicsPath Move to command needs 2 arguments '{0}'  {1}", svgPath, lastPathInformation);
                            break;
                        }
                        objCount++;
                        if (absolute || startPath)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = floatArgs[i] + lastX; currentY = floatArgs[i + 1] + lastY; }

                        if (i == 0) // first two coordinates are start of path
                        {
                            firstX = currentX; firstY = currentY;

                            if (!startPath && Gcode.IsEqual(lastX, currentX) && Gcode.IsEqual(lastY, currentY)) // found a 2nd 'M' or 'm' command within 'd' path		
                            { Logger.Warn("⚠ Skip 2nd 'm' because of no position change: {0}", lastPathInformation); }
                            else
                            {
                                textOnPath.StartFigure();
                                startPath = false;
                            }
                        }
                        else
                        {
                            textOnPath.StartFigure();
                            textOnPath.AddLine((float)firstX, (float)firstY, currentX, currentY);
                        }

                        lastX = currentX; lastY = currentY;
                    }
                    break;
                #endregion
                case 'Z':       // Close the current subpath
                    #region ZClose
                    if (!svgNodesOnly)
                    {
                        if (firstX == null) { firstX = currentX; }
                        if (firstY == null) { firstY = currentY; }
                        if (!Gcode.IsEqual((float)firstX, lastX) || !Gcode.IsEqual((float)firstY, lastY))
                            textOnPath.AddLine(lastX, lastY, (float)firstX, (float)firstY);

                    }
                    lastX = (float)firstX; lastY = (float)firstY;
                    firstX = null; firstY = null;
                    textOnPath.CloseFigure();
                    break;
                #endregion
                case 'L':       // Draw a line from the current point to the given (x,y) coordinate
                    #region Line
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        if (floatArgs.Length < (i + 2))
                        {
                            Logger.Error("ParsePathCommandGraphicsPath Line to command needs 2 arguments '{0}'", svgPath);
                            break;
                        }
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY + floatArgs[i + 1]; }

                        textOnPath.AddLine(lastX, lastY, currentX, currentY);
                        lastX = currentX; lastY = currentY;
                        //        cxMirror = currentX; cyMirror = currentY;
                    }
                    break;
                #endregion
                case 'H':       // Draws a horizontal line from the current point (cpx, cpy) to (x, cpy)
                    #region Horizontal
                    for (int i = 0; i < floatArgs.Length; i++)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = lastY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY; }
                        textOnPath.AddLine(lastX, lastY, currentX, currentY);
                        lastX = currentX; lastY = currentY;
                    }
                    break;
                #endregion
                case 'V':       // Draws a vertical line from the current point (cpx, cpy) to (cpx, y)
                    #region Vertical
                    for (int i = 0; i < floatArgs.Length; i++)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = lastX; currentY = floatArgs[i] + offsetY; }
                        else
                        { currentX = lastX; currentY = lastY + floatArgs[i]; }
                        textOnPath.AddLine(lastX, lastY, currentX, currentY);
                        lastX = currentX; lastY = currentY;
                    }
                    break;
                #endregion
                case 'A':       // Draws an elliptical arc from the current point to (x, y)
                    #region Arc
                    for (int rep = 0; rep < floatArgs.Length; rep += 7)
                    {
                        if (floatArgs.Length < (rep + 7))
                        {
                            Logger.Error("ParsePathCommandGraphicsPath Elliptical arc curve command needs 7 arguments '{0}'", svgPath);
                            break;
                        }

                        objCount++;

                        float rx, ry, rot, large, sweep, nx, ny;
                        rx = floatArgs[rep]; ry = floatArgs[rep + 1];
                        rot = floatArgs[rep + 2];
                        large = floatArgs[rep + 3];
                        sweep = floatArgs[rep + 4];
                        if (absolute)
                        { nx = floatArgs[rep + 5] + offsetX; ny = floatArgs[rep + 6] + offsetY; }
                        else
                        { nx = floatArgs[rep + 5] + lastX; ny = floatArgs[rep + 6] + lastY; }

                        ImportMath.CalcArc(lastX, lastY, rx, ry, rot, large, sweep, nx, ny, GraphicsPathMoveTo);

                        lastX = nx; lastY = ny;
                    }
                    break;
                #endregion
                case 'C':       // Draws a cubic Bézier curve from the current point to (x,y)
                    #region Cubic
                    for (int rep = 0; rep < floatArgs.Length; rep += 6)
                    {
                        if (floatArgs.Length < (rep + 6))
                        {
                            Logger.Error("ParsePathCommandGraphicsPath Cubic Bézier curve command needs 6 arguments '{0}'", svgPath);
                            break;
                        }
                        objCount++;

                        if ((rep + 5) < floatArgs.Length)
                        {

                            Point Off = new Point(offsetX, offsetY);
                            if (!absolute)
                                Off = new Point(lastX, lastY);
                            Vector c1 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                            Vector c2 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;
                            Vector c3 = new Vector(floatArgs[rep + 4], floatArgs[rep + 5]) + (Vector)Off;

                            textOnPath.AddBezier(new System.Drawing.PointF(lastX, lastY),
                                                    new System.Drawing.PointF((float)c1.X, (float)c1.Y),
                                                    new System.Drawing.PointF((float)c2.X, (float)c2.Y),
                                                    new System.Drawing.PointF((float)c3.X, (float)c3.Y));

                            lastX = (float)c3.X; lastY = (float)c3.Y;

                            cMirror = c3 * 2 - c2;
                        }
                        else
                        { Logger.Error("ParsePathCommandGraphicsPath Missing argument after {0} ", rep); }
                    }
                    break;
                #endregion
                case 'S':       // Draws a cubic Bézier curve from the current point to (x,y)
                    #region Small Cubic
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        if (floatArgs.Length < (rep + 4))
                        {
                            Logger.Error("ParsePathCommandGraphicsPath Smooth curveto command needs 4 arguments '{0}'", svgPath);
                            break;
                        }
                        objCount++;

                        Point Off = new Point(offsetX, offsetY);
                        if (!absolute)
                            Off = new Point(lastX, lastY);
                        Vector c2 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                        Vector c3 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;

                        textOnPath.AddBezier(new System.Drawing.PointF(lastX, lastY),
                                   new System.Drawing.PointF((float)cMirror.X, (float)cMirror.Y),
                                   new System.Drawing.PointF((float)c2.X, (float)c2.Y),
                                   new System.Drawing.PointF((float)c3.X, (float)c3.Y));

                        lastX = (float)c3.X; lastY = (float)c3.Y;

                        cMirror = c3 * 2 - c2;
                    }
                    break;
                #endregion
                case 'Q':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    #region Quadratic
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        if (floatArgs.Length < (rep + 4))
                        {
                            Logger.Error("ParsePathCommandGraphicsPath Quadratic Bézier curveto command needs 4 arguments '{0}'", svgPath);
                            break;
                        }
                        objCount++;

                        Point Off = new Point(offsetX, offsetY);
                        if (!absolute)
                            Off = new Point(lastX, lastY);
                        Vector c2 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                        Vector c3 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;

                        ImportMath.CalcQuadraticBezier(new Point(lastX, lastY), (Point)c2, (Point)c3, GraphicsPathMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;

                        cMirror = c3 * 2 - c2;
                    }
                    break;
                #endregion
                case 'T':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    #region TQuadratic
                    if (svgComments) { Graphic.SetComment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 2)
                    {
                        if (floatArgs.Length < (rep + 2))
                        {
                            Logger.Error("ParsePathCommandGraphicsPath Smooth quadratic Bézier curveto command needs 2 arguments '{0}'", svgPath);
                            break;
                        }
                        objCount++;
                        if (svgComments) { Graphic.SetComment(string.Format(" draw curve nr. {0} ", (1 + rep / 2))); }

                        Point Off = new Point(offsetX, offsetY);
                        if (!absolute)
                            Off = new Point(lastX, lastY);
                        Vector c3 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;

                        ImportMath.CalcQuadraticBezier(new Point(lastX, lastY), (Point)cMirror, (Point)c3, GraphicsPathMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;

                        cMirror = c3;
                    }
                    break;
                #endregion
                case ' ':
                    if (logEnable) Logger.Trace("ParsePathCommandGraphicsPath Element ' ', nothing to do floatArgs.Length:{0}", floatArgs.Length);
                    break;
                default:
                    break;
            }
            return objCount;
        }
        /// <summary>
        /// Transform XY coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static Point TranslateXY(float x, float y)
        {
            Point coord = new Point(x, y);
            return TranslateXY(coord);
        }
        private static Point TranslateXY(Point pointStart)
        {
            // Logger.Info("TranslateXY {0} ", matrixElement.ToString());
            Point pointResult = matrixElement.Transform(pointStart);
            return pointResult;
        }


        /// <summary>
        /// Transform I,J coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static Point TranslateIJ(float i, float j)
        {
            Point coord = new Point(i, j);
            return TranslateIJ(coord);
        }
        private static Point TranslateIJ(Point pointStart)
        {
            Point pointResult = pointStart;
            double tmp_i = pointStart.X, tmp_j = pointStart.Y;
            pointResult.X = tmp_i * matrixElement.M11 + tmp_j * matrixElement.M21;  // - tmp
            pointResult.Y = tmp_i * matrixElement.M12 + tmp_j * matrixElement.M22; // tmp_i*-matrix     // i,j are relative - no offset needed, but perhaps rotation
            return pointResult;
        }

        private static void GCodeDotOnlyWithZ(float x, float y, float z, string cmt)
        {
            Point tmp = TranslateXY(x, y);  // convert from SVG-Units to GCode-Units
            Graphic.SetGeometry(cmt);
            Graphic.AddDotWithZ(tmp, z);//, cmt);  
        }
        private static void GCodeDotOnly(float x, float y, string cmt)
        {
            Point tmp = TranslateXY(x, y);  // convert from SVG-Units to GCode-Units
            Graphic.SetGeometry(cmt);
            Graphic.AddDot(tmp);// , cmt);    		
        }

        /// <summary>
        /// Insert G0 and Pen down gcode command
        /// </summary>
        private static void SVGStartPath(float x, float y, string cmt, bool setGeometry = true)
        {
            Point tmp = TranslateXY(x, y);  // convert from SVG-Units to GCode-Units
            if (logEnable) Logger.Trace("▼▼▼ SVGStart at:x:{0:0.000} y:{1:0.000}  svg:{2}  set:{3}", tmp.X, tmp.Y, cmt, setGeometry);
            if (setGeometry) Graphic.SetGeometry((cmt.Length > 20) ? cmt.Substring(0, 20) : cmt);
            Graphic.StartPath(tmp);
        }

        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void SVGMoveTo(float x, float y, string cmt)
        {
            if (!svgNodesOnly)
                SVGMoveTo(new Point(x, y), cmt);
            else
                GCodeDotOnly(x, y, cmt);
        }

        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void SVGMoveTo(Point orig, string cmt)
        {
            if (!svgNodesOnly)
            {
                Point tmp = TranslateXY(orig);
                if (cmt.ToLower() == "z")
                {
                    if (logEnable) Logger.Trace("▲▲▲ SVGStop at: x:{0:0.000} y:{1:0.000}  svg:{2}", tmp.X, tmp.Y, cmt);
                }
                else if (logEnableCoord) Logger.Trace("  ● SVGMoveTo:  x:{0:0.000} y:{1:0.000}  svg:{2}", tmp.X, tmp.Y, cmt);
                Graphic.AddLine(tmp);//, cmt);
            }
            else
                GCodeDotOnly((float)orig.X, (float)orig.Y, cmt);
        }
        private static void GraphicsPathMoveTo(Point orig, string cmt)
        {
            textOnPath.AddLine((float)lastX, (float)lastY, (float)orig.X, (float)orig.Y);
            lastX = (float)orig.X;
            lastY = (float)orig.Y;
        }

        /// <summary>
        /// Insert G2/G3 gcode command
        /// </summary>
        private static void SVGArcToCCW(float x, float y, float i, float j, string cmt)
        {
            Point coordxy = TranslateXY(x, y);
            Point coordij = TranslateIJ(i, j);
            if (logEnable) Logger.Trace("  svgArcToCCW x:{0:0.00} y:{1:0.00} i:{2:0.00} j:{3:0.00} cmt:{4}", x, y, i, j, cmt);
            Graphic.AddArc(false, coordxy, coordij);//, cmt);
        }
    }
}