/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace GRBL_Plotter
{
    class GCodeFromSVG
    {
//        private static bool loggerTrace = false;    //true;
        private static int svgBezierAccuracy = 6;       // applied line segments at bezier curves
        private static bool svgScaleApply = true;       // try to scale final GCode if true
        private static float svgMaxSize = 100;          // final GCode size (greater dimension) if scale is applied
        private static bool svgClosePathExtend = true;  // not working if true move to first and second point of path to get overlap
        private static bool svgGroupObjects = true;     // if true sort objects by tool-nr. (to avoid back and forth pen change)
        private static bool svgNodesOnly = true;        // if true only do pen-down -up on given coordinates
        private static bool svgComments = true;         // if true insert additional comments into GCode

        private static bool svgConvertToMM = true;
        private static float gcodeScale = 1;                    // finally scale with this factor if svgScaleApply and svgMaxSize
        private static Matrix[] matrixGroup = new Matrix[100];  // store SVG-Group transformation matrixes
        private static Matrix matrixElement = new Matrix();     // store finally applied matrix
        private static Matrix oldMatrixElement = new Matrix();  // store finally applied matrix

        private static float factor_In2Px = 96;
        private static float factor_Mm2Px = 96f / 25.4f;
        private static float factor_Cm2Px = 96f / 2.54f;
        private static float factor_Pt2Px = 96f / 72f;
        private static float factor_Pc2Px = 12 * 96f / 72f;
        private static float factor_Em2Px = 150;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static string logSource = "";

        /// <summary>
        /// Entrypoint for conversion: apply file-path or file-URL
        /// </summary>
        /// <param name="file">String keeping file-name or URL</param>
        /// <returns>String with GCode of imported data</returns>
        private static XElement svgCode;
        private static bool fromClipboard = false;
        private static bool unitIsPixel = false;
        public static string convertFromText(string svgText, bool replaceUnitByPixel = false)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(svgText);
            MemoryStream stream = new MemoryStream(byteArray);
            svgCode = XElement.Load(stream, LoadOptions.None);
            fromClipboard = true;
            if (svgText.IndexOf("Adobe") >= 0)
                replaceUnitByPixel = false;
            unitIsPixel = replaceUnitByPixel;
            return convertSVG(svgCode, "from Clipboard");                   // startConvert(svgCode);
        }
        public static string convertFromFile(string file)
        {
            unitIsPixel = false;
            fromClipboard = false;

            if (file == "")
            { MessageBox.Show("Empty file name"); return ""; }
            if (file.Substring(0, 4) == "http")
            {
                string content = "";
                using (var wc = new System.Net.WebClient())
                {
                    try { content = wc.DownloadString(file); }
                    catch { MessageBox.Show("Could not load content from " + file); return ""; }
                }
                if ((content != "") && (content.IndexOf("<?xml") == 0))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(content);
                    MemoryStream stream = new MemoryStream(byteArray);
                    svgCode = XElement.Load(stream, LoadOptions.None);
                    System.Windows.Clipboard.SetData("image/svg+xml", stream);
                    return convertSVG(svgCode, file);                   // startConvert(svgCode);
                }
                else
                    MessageBox.Show("This is probably not a SVG document.\r\nFirst line: " + content.Substring(0, 50));
            }
            else
            {
                if (File.Exists(file))
                {   try
                    {   svgCode = XElement.Load(file, LoadOptions.None);    // PreserveWhitespace);
                        return convertSVG(svgCode, file);                   // startConvert(svgCode);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e,"Error loading SVG-Code");
                        MessageBox.Show("Error '" + e.ToString() + "' in XML file " + file + "\r\n\r\nTry to save file with other encoding e.g. UTF-8"); return "";
                    }
                }
                else { MessageBox.Show("File does not exist: " + file); return ""; }
            }
            return "";
        }

        private static string convertSVG(XElement svgCode, string txt)
        {
            Logger.Debug("convertSVG {0}", txt);

            svgBezierAccuracy = (int)Properties.Settings.Default.importBezierLineSegmentsCnt;
            svgScaleApply = Properties.Settings.Default.importSVGRezise;
            svgMaxSize = (float)Properties.Settings.Default.importSVGMaxSize;
            svgClosePathExtend = Properties.Settings.Default.importSVGPathExtend;
            svgComments = Properties.Settings.Default.importSVGAddComments;
            svgConvertToMM = Properties.Settings.Default.importUnitmm;                  // Target units and display in setup
            svgGroupObjects = Properties.Settings.Default.importGroupObjects;           // SVG-Import group objects
            svgNodesOnly = Properties.Settings.Default.importSVGNodesOnly;

            Plotter.StartCode();        // initalize variables
            GetVectorSVG(svgCode);      // convert graphics
            Plotter.SortCode();         // sort objects
            return Plotter.FinalGCode("SVG import", txt);
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
            factor_Em2Px = 150;

 //           countSubPath = 0;
            startFirstElement = true;
            gcodeScale = 1;
            svgWidthPx = 0; svgHeightPx = 0;
            currentX = 0; currentY = 0;
            offsetX = 0; offsetY = 0;
            firstX = null;
            firstY = null;
            lastX = 0;
            lastY = 0;

            matrixElement.SetIdentity();                // preset transform matrix
            oldMatrixElement.SetIdentity();             // preset backup of transform matrix
            for (int i=0; i<matrixGroup.Length;i++)     // preset sub transform matrixes
                matrixGroup[i].SetIdentity(); 

            parseGlobals(svgCode);                      // parse main svg elements

            parseBasicElements(svgCode,1);
            parsePath(svgCode,1);                       // 1st level paths
            parseGroup(svgCode,1);                      // parse groups (recursive)
            if (svgGroupObjects)
            {   if (!Plotter.IsPathFigureEnd)
                {   Plotter.Comment(xmlMarker.figureEnd + Plotter.PathCount + ">");     // reached if SVG code via copy & paste was converted
                    Logger.Debug(" FigureEnd");
                }
                Plotter.IsPathFigureEnd = true;
            }
            return;
        }

        /// <summary>
        /// Parse SVG dimension (viewbox, width, height)
        /// </summary>
        private static XNamespace nspace = "http://www.w3.org/2000/svg";
        private static void parseGlobals(XElement svgCode)
        {   // One px unit is defined to be equal to one user unit. Thus, a length of "5px" is the same as a length of "5".
            Matrix tmp = new Matrix(1, 0, 0, 1, 0, 0); // m11, m12, m21, m22, offsetx, offsety
            svgWidthPx = 0;
            svgHeightPx = 0;
            float vbOffX = 0;
            float vbOffY = 0;
            float vbWidth = 0;
            float vbHeight = 0;
            float scale = 1;
            string tmpString="";

            if (svgCode.Attribute("viewBox") != null)   // viewBox unit always in px
            {
                string viewbox = svgCode.Attribute("viewBox").Value;
                logSource = "viewBox "+ viewbox;
                viewbox = Regex.Replace(viewbox, @"\s+", " ").Replace(' ', '|');    // remove double space
                var split = viewbox.Split('|');
                vbOffX = -convertToPixel(split[0]);
                vbOffY = -convertToPixel(split[1]);
                vbWidth  = convertToPixel(split[2]);    
                vbHeight = convertToPixel(split[3].TrimEnd(')'));
                tmp.M11 = 1; tmp.M22 = -1;      // flip Y
                tmp.OffsetY = vbHeight;
                if (svgComments) Plotter.AddToHeader(" SVG viewbox :" + viewbox);
                //MessageBox.Show(string.Format("{0} {1} {2} {3}",vbOffX,vbOffY,vbWidth,vbHeight));
            }

            scale = 1;
            if (svgConvertToMM)                 // convert back from px to mm 
                scale = (1 / factor_Mm2Px);         
            else                                // convert back from px to inch
                scale = (1 / factor_In2Px);

            if (unitIsPixel)                    // got svg-text from clipboard perhaps from maker.js
            {   scale = 1;
            }

            if (svgComments) Plotter.AddToHeader(" SVG dpi :" + factor_In2Px.ToString());
            if (svgCode.Attribute("width") != null)
            {
                tmpString = svgCode.Attribute("width").Value;
                logSource = "width " + tmpString;
                svgWidthPx = convertToPixel(tmpString);             // convert in px

                if (svgComments) Plotter.AddToHeader(" SVG width :" + svgCode.Attribute("width").Value);
                tmp.M11 = scale; // get desired scale
                if (fromClipboard)
                    tmp.M11 = 1 / factor_Mm2Px; // 3.543307;         // https://www.w3.org/TR/SVG/coords.html#Units
                if (vbWidth > 0)
                {   tmp.M11 = scale * svgWidthPx / vbWidth;
                    tmp.OffsetX = vbOffX * scale;   // svgWidthUnit / vbWidth;
                }
            }

            if (svgCode.Attribute("height") != null)
            {
                tmpString = svgCode.Attribute("height").Value;
                logSource = "height " + tmpString;
                svgHeightPx = convertToPixel(tmpString);

                if (svgComments) Plotter.AddToHeader(" SVG height :" + svgCode.Attribute("height").Value);
                tmp.M22 = -scale;   // get desired scale and flip vertical
                tmp.OffsetY = scale * svgHeightPx;  // svgHeightUnit;

                if (fromClipboard)
                {   tmp.M22 = -1 / factor_Mm2Px;// 3.543307;
                    tmp.OffsetY = svgHeightPx / factor_Mm2Px; // 3.543307;     // https://www.w3.org/TR/SVG/coords.html#Units
                }
                if (vbHeight > 0)
                {   tmp.M22 = -scale * svgHeightPx / vbHeight;
                    tmp.OffsetY = -vbOffY * svgHeightPx / vbHeight + (svgHeightPx * scale);
                }
            }

            float newWidth = Math.Max(svgWidthPx, vbWidth);     // use value from 'width' or 'viewbox' parameter
            float newHeight = Math.Max(svgHeightPx, vbHeight);
            if ((newWidth > 0) && (newHeight > 0))
            {   if (svgScaleApply)
                {   gcodeScale = svgMaxSize / Math.Max(newWidth, newHeight);        // calc. factor to get desired max. size
                    tmp.Scale((double)gcodeScale, (double)gcodeScale);
                    if (svgConvertToMM)                         // https://www.w3.org/TR/SVG/coords.html#Units
                        tmp.Scale(factor_Mm2Px, factor_Mm2Px);  // 3.543307, 3.543307);
                    else
                        tmp.Scale(factor_In2Px, factor_In2Px);

                    if (svgComments)
                        Plotter.AddToHeader(string.Format(" Scale to X={0} Y={1} f={2} ", newWidth * gcodeScale, newHeight * gcodeScale, gcodeScale));
                }
            }
            else
                if (svgComments) Plotter.AddToHeader(" SVG Dimension not given ");

            for (int i = 0; i < matrixGroup.Length; i++)
            { matrixGroup[i] = tmp; }
            matrixElement = tmp;

            var element = svgCode.Element(nspace + "title");
            if (element != null)
                Plotter.DocTitle = element.Value;

            var xtmp = svgCode.Element(nspace + "metadata");
            if ((xtmp) != null)
            {   XNamespace ccspace = "http://creativecommons.org/ns#";
                XNamespace rdfspace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
                XNamespace dcspace = "http://purl.org/dc/elements/1.1/";
                if ((xtmp = xtmp.Element(rdfspace + "RDF")) != null)
                {   if ((xtmp = xtmp.Element(ccspace + "Work")) != null)
                    {   if ((xtmp = xtmp.Element(dcspace + "description")) != null)
                            Plotter.DocDescription = xtmp.Value;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Parse Group-Element and included elements
        /// </summary>
        private static void parseGroup(XElement svgCode, int level)
        {   foreach (XElement groupElement in svgCode.Elements(nspace + "g"))
            {
                if (groupElement.Attribute("id") != null)
                    Plotter.PathId = groupElement.Attribute("id").Value;

                Plotter.PathId = "";
                Plotter.PathDashArray = new double[0];      // reset dash pattern

                parseTransform(groupElement,true, level);   // transform will be applied in gcodeMove
                parseAttributs(groupElement);               // process color and stroke-dasharray
                parseBasicElements(groupElement, level);
                parsePath(groupElement, level);
                parseGroup(groupElement, level+1);
            }
            return;
        }

        /// <summary>
        /// Parse stroke attributes
        /// </summary>
        private static void parseAttributs(XElement element)
        {
            if (element.Attribute("style") != null)
            {   //if (svgComments) Plotter.Comment(string.Format(" style '{0}'", element.Attribute("style").Value));
                string pathColor = getStyleProperty(element, "stroke");
                if (pathColor.Length > 1)
                    Plotter.PathColor = pathColor.Substring(1);       //getColor(pathElement);
                Plotter.PathToolNr = toolTable.getToolNr(Plotter.PathColor, 0);
                setDashPattern(getStyleProperty(element, "stroke-dasharray"));
            }
            if (element.Attribute("stroke") != null)
            {   //if (svgComments) Plotter.Comment(string.Format(" stroke '{0}'", element.Attribute("stroke").Value));
                string pathColor = element.Attribute("stroke").Value;
                Plotter.PathColor = pathColor.StartsWith("#") ? pathColor.Substring(1) : pathColor;
                Plotter.PathToolNr = toolTable.getToolNr(Plotter.PathColor, 0);
            }
            if (element.Attribute("stroke-dasharray") != null)
            {   //if (svgComments) Plotter.Comment(string.Format(" stroke-dasharray '{0}'", element.Attribute("stroke-dasharray").Value));
                setDashPattern(element.Attribute("stroke-dasharray").Value);
            }
        }

        /// <summary>
        /// Parse Transform information - more information here: http://www.w3.org/TR/SVG/coords.html
        /// transform will be applied in gcodeMove
        /// </summary>
        private static bool parseTransform(XElement element,bool isGroup, int level)
        {   Matrix tmp = new Matrix(1, 0, 0, 1, 0, 0); // m11, m12, m21, m22, offsetx, offsety
            bool transf = false;

            if (element.Attribute("transform") != null)
            {
                transf = true;
                string transform = element.Attribute("transform").Value;
                logSource = "transform " + transform;
                if ((transform != null) && (transform.IndexOf("translate") >= 0))
                {
                    var coord = getTextBetween(transform, "translate(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');

                    tmp.OffsetX = convertToPixel(split[0]);
                    if (split.Length > 1)
                        tmp.OffsetY = convertToPixel(split[1].TrimEnd(')'));
                    if (svgComments) Plotter.Comment(string.Format(" SVG-Translate {0} {1} ", tmp.OffsetX, tmp.OffsetY));
                }
                if ((transform != null) && (transform.IndexOf("scale") >= 0))
                {
                    var coord = getTextBetween(transform, "scale(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');
                    tmp.M11 = convertToPixel(split[0]);
                    if (split.Length > 1)
                    {   tmp.M22 = convertToPixel(split[1]); }
                    else
                    {
                        tmp.M11 = convertToPixel(coord);
                        tmp.M22 = convertToPixel(coord);
                    }
                    if (svgComments) Plotter.Comment(string.Format(" SVG-Scale {0} {1} ", tmp.M11, tmp.M22));
                }
                if ((transform != null) && (transform.IndexOf("rotate") >= 0))
                {
                    var coord = getTextBetween(transform, "rotate(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');
                    float angle = convertToPixel(split[0]) * (float)Math.PI / 180;
                    tmp.M11 = Math.Cos(angle); tmp.M12 = Math.Sin(angle);
                    tmp.M21 = -Math.Sin(angle); tmp.M22 = Math.Cos(angle);

                    if (svgComments) Plotter.Comment(string.Format(" SVG-Rotate {0} ", angle));
                }
                if ((transform != null) && (transform.IndexOf("matrix") >= 0))
                {
                    var coord = getTextBetween(transform, "matrix(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');
                    tmp.M11 = convertToPixel(split[0]);     // a    scale x         a c e
                    tmp.M12 = convertToPixel(split[1]);     // b                    b d f
                    tmp.M21 = convertToPixel(split[2]);     // c                    0 0 1
                    tmp.M22 = convertToPixel(split[3]);     // d    scale y
                    tmp.OffsetX = convertToPixel(split[4]); // e    offset x
                    tmp.OffsetY = convertToPixel(split[5]); // f    offset y
                    if (svgComments) Plotter.Comment(string.Format(" SVG-Matrix {0} {1} {2} ", coord.Replace(',', '|'), level, isGroup));
                }
            }
            if (isGroup)
            {
                matrixGroup[level].SetIdentity();
                if (level > 0)
                {
                    for (int i = level; i < matrixGroup.Length; i++)
                    { matrixGroup[i] = Matrix.Multiply(tmp, matrixGroup[level - 1]); }
                }
                else
                { matrixGroup[level] = tmp; }
                matrixElement = matrixGroup[level];
            }
            else
            {   matrixElement = Matrix.Multiply(tmp, matrixGroup[level]);
            }
            return transf;
        }

        private static string getTextBetween(string source,string s1, string s2)
        {
            int start = source.IndexOf(s1) + s1.Length;
            char c;
            for (int i = start; i < source.Length; i++)
            {   c = source[i];
                if (!(Char.IsNumber(c) || c == '.' || c==',' || c == ' ' || c=='-' || c == 'e'))    // also exponent
                    return source.Substring(start, i - start);
            }
            return source.Substring(start,source.Length-start-1);
        }
        private static float convertToPixel(string str, float ext=1)        // return value in px
        {       // https://www.w3.org/TR/SVG/coords.html#Units          // in=90 or 96 ???
            bool percent = false;
     //       Logger.Trace("convert to pixel in {0}", str);
            float factor = 1;   // no unit = px
            if (str.IndexOf("mm") > 0) { factor = factor_Mm2Px; }               // Millimeter
            else if (str.IndexOf("cm") > 0) { factor = factor_Cm2Px; }          // Centimeter
            else if (str.IndexOf("in") > 0) { factor = factor_In2Px; }          // Inch    72, 90 or 96?
            else if (str.IndexOf("pt") > 0) { factor = factor_Pt2Px; }          // Point
            else if (str.IndexOf("pc") > 0) { factor = factor_Pc2Px; }          // Pica
            else if (str.IndexOf("em") > 0) { factor = factor_Em2Px; }          // Font size
            else if (str.IndexOf("%") > 0)  { percent = true; }
            str = str.Replace("pt", "").Replace("pc", "").Replace("mm", "").Replace("cm", "").Replace("in", "").Replace("em ", "").Replace("%", "").Replace("px", "");
            float test;
            if (str.Length > 0)
            {   if (percent)
                {   if (float.TryParse(str, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out test))
                    { return (test * ext / 100); }
                }
                else
                {   if (float.TryParse(str, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out test))
                    { return (test * factor); }
                }
            }
            Logger.Error("convertToPixel source '{0}' ", logSource);
            Logger.Error("convertToPixel TryParse failed '{0}' ",str);
            Plotter.AddToHeader(string.Format(" !!! Error: convert to float, string is: '{0}'",str));
            return 0f;
        }
        private static string removeUnit(string str)
        {   return str.Replace("pt", "").Replace("pc", "").Replace("mm", "").Replace("cm", "").Replace("in", "").Replace("em ", "").Replace("%", "").Replace("px", ""); }

        private static string getStyleProperty(XElement pathElement, string property)
        {   if (pathElement.Attribute("style") != null)
            {   string style = pathElement.Attribute("style").Value;
                string[] prop = style.Split(';');
                foreach (string propitem in prop)
                {   string[] keyval = propitem.Split(':');
                    if ((keyval.Length >1) && (keyval[0].Contains(property)))
                        return keyval[1];
                }
            }
            return "";
        }
        private static void setDashPattern(string dasharray)
        {   try
            {   if (dasharray.Length > 2)
                {   if (dasharray.Contains("none"))
                    {   Plotter.PathDashArray = new double[0]; }
                    else
                    {
                        string[] pattern;
                        if (dasharray.Contains(',')) { pattern = dasharray.Split(','); }
                        else { pattern = dasharray.Split(' '); }
                        //  float[] dash = Array.ConvertAll(pattern, float.Parse);
                        double tmp;
                        double[] dash = Array.ConvertAll(pattern,
                               s => double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out tmp) ? (tmp * (double)gcodeScale) : 0);
                        Plotter.PathDashArray = dash;
                    }
                }
            }
            catch (Exception er)
            { Logger.Error(er,"dasharray: {0}",dasharray); }
        }


    /// <summary>
    /// Convert Basic shapes (up to now: line, rect, circle) check: http://www.w3.org/TR/SVG/shapes.html
    /// </summary>
    private static void parseBasicElements(XElement svgCode, int level)
        {   string[] forms = { "rect", "circle", "ellipse", "line", "polyline", "polygon", "text", "image" };
            foreach (var form in forms)
            {
                foreach (var pathElement in svgCode.Elements(nspace + form))
                {
                    if (pathElement != null)
                    {
                        if (gcode.loggerTrace) Logger.Trace("parseBasicElements: {0} Level: {1}", form,level);

                        parseAttributs(pathElement);   // process color and stroke-dasharray
                        logSource = "Basic element " + form;

                        Plotter.SetGroup(Plotter.PathToolNr);

                        if (pathElement.Attribute("id") != null)
                            Plotter.PathId = pathElement.Attribute("id").Value;

                        if (startFirstElement)
                        { Plotter.PenUp("1st shape"); startFirstElement = false; }

                        offsetX = 0; offsetY = 0;

                        oldMatrixElement = matrixElement;

                        parseTransform(pathElement, false, level);  // transform will be applied in gcodeMove

                        float x=0, y=0, x1=0, y1=0, x2=0, y2=0, width=0, height=0, rx=0, ry=0,cx=0,cy=0,r=0;
                        string[] points= {""};
                        if (pathElement.Attribute("x") !=null)      x = convertToPixel(pathElement.Attribute("x").Value);
                        if (pathElement.Attribute("y") != null)     y = convertToPixel(pathElement.Attribute("y").Value);
                        if (pathElement.Attribute("x1") != null)    x1 = convertToPixel(pathElement.Attribute("x1").Value);
                        if (pathElement.Attribute("y1") != null)    y1 = convertToPixel(pathElement.Attribute("y1").Value);
                        if (pathElement.Attribute("x2") != null)    x2 = convertToPixel(pathElement.Attribute("x2").Value);
                        if (pathElement.Attribute("y2") != null)    y2 = convertToPixel(pathElement.Attribute("y2").Value);
                        if (pathElement.Attribute("width") != null)  width =convertToPixel(pathElement.Attribute("width").Value,  svgWidthPx);
                        if (pathElement.Attribute("height") != null) height=convertToPixel(pathElement.Attribute("height").Value, svgHeightPx);
                        if (pathElement.Attribute("rx") != null)    rx=convertToPixel(pathElement.Attribute("rx").Value);
                        if (pathElement.Attribute("ry") != null)    ry=convertToPixel(pathElement.Attribute("ry").Value);
                        if (pathElement.Attribute("cx") != null)    cx=convertToPixel(pathElement.Attribute("cx").Value);
                        if (pathElement.Attribute("cy") != null)    cy=convertToPixel(pathElement.Attribute("cy").Value);
                        if (pathElement.Attribute("r") != null)     r=convertToPixel(pathElement.Attribute("r").Value);
                        if (pathElement.Attribute("points") != null) points = pathElement.Attribute("points").Value.Split(' ');

                        if (form == "rect")
                        {
                            if (ry == 0) { ry = rx; }
                            else if (rx == 0) { rx = ry; }
                            else if (rx != ry) { rx = Math.Min(rx,ry); ry = rx; }   // only same r for x and y are possible
                            if (svgComments) Plotter.Comment(string.Format(" SVG-Rect x:{0} y:{1} width:{2} height:{3} rx:{4} ry:{5}", x, y, width, height, rx, ry));
                            if (gcode.loggerTrace) Logger.Trace(" SVG-Rect x:{0} y:{1} width:{2} height:{3} rx:{4} ry:{5}", x, y, width, height, rx, ry);
                            x += offsetX; y += offsetY;
                            if (!svgNodesOnly)
                            {
                                svgStartPath(x + rx, y + height, form);
                                svgMoveTo(x + width - rx, y + height, form + " a1");
                                if (rx > 0) svgArcToCCW(x + width, y + height - ry, 0, -ry, form);  // +ry
                                svgMoveTo(x + width, y + ry, form + " b1");                        // upper right
                                if (rx > 0) svgArcToCCW(x + width - rx, y, -rx, 0, form);
                                svgMoveTo(x + rx, y, form + " a2");                                // upper left
                                if (rx > 0) svgArcToCCW(x, y + ry, 0, ry, form);                    // -ry
                                svgMoveTo(x, y + height - ry, form + " b2");                       // lower left
                                if (rx > 0)
                                {
                                    svgArcToCCW(x + rx, y + height, rx, 0, form);
                                    svgMoveTo(x + rx, y + height, form);  // repeat first point to avoid back draw after last G3
                                }
                            }
                            else
                            {
                                gcodeDotOnly(x + rx, y + height, form);
                                gcodeDotOnly(x + width - rx, y + height, form + " a1");
                                if (rx > 0) gcodeDotOnly(x + width, y + height - ry, form);  // +ry
                                gcodeDotOnly(x + width, y + ry, form + " b1");                        // upper right
                                if (rx > 0) gcodeDotOnly(x + width - rx, y, form);
                                gcodeDotOnly(x + rx, y, form + " a2");                                // upper left
                                if (rx > 0) gcodeDotOnly(x, y + ry, form);                    // -ry
                                gcodeDotOnly(x, y + height - ry, form + " b2");                       // lower left
                                if (rx > 0) gcodeDotOnly(x + rx, y + height, form);
                            }
                            Plotter.StopPath(form);
                        }
                        else if (form == "circle")
                        {
                            if (svgComments) Plotter.Comment(string.Format(" circle cx:{0} cy:{1} r:{2} ", cx, cy, r));
                            if (gcode.loggerTrace) Logger.Trace(" circle cx:{0} cy:{1} r:{2} ", cx, cy, r);
                            cx += offsetX; cy += offsetY;
                            if (!svgNodesOnly)
                            {
                                svgStartPath(cx + r, cy, form);
                                svgArcToCCW(cx + r, cy, -r, 0, form);
                            }
                            else
                            {
                                gcodeDotOnly(cx + r, cy, form);
                                gcodeDotOnly(cx + r, cy, form);
                            }
                            Plotter.StopPath(form);
                        }
                        else if (form == "ellipse")
                        {
                            if (svgComments) Plotter.Comment(string.Format(" ellipse cx:{0} cy:{1} rx:{2}  ry:{3}", cx, cy, rx, ry));
                            if (gcode.loggerTrace) Logger.Trace(" ellipse cx:{0} cy:{1} rx:{2}  ry:{3}", cx, cy, rx, ry);
                            cx += offsetX; cy += offsetY;
                            if (!svgNodesOnly)
                            {
                                svgStartPath(cx + rx, cy, form);
                                Plotter.IsPathReduceOk = true;
                                importMath.calcArc(cx + rx, cy, rx, ry, 0, 1, 1, cx - rx, cy, svgMoveTo);
                                importMath.calcArc(cx - rx, cy, rx, ry, 0, 1, 1, cx + rx, cy, svgMoveTo);
                            }
                            else
                            {
                                gcodeDotOnly(cx + rx, cy, form);
                                Plotter.IsPathReduceOk = true;
                            }
                            Plotter.StopPath(form);
                        }
                        else if (form == "line")
                        {
                            if (svgComments) Plotter.Comment(string.Format(" SVG-Line x1:{0} y1:{1} x2:{2} y2:{3} ", x1, y1, x2, y2));
                            if (gcode.loggerTrace) Logger.Trace(" SVG-Line x1:{0} y1:{1} x2:{2} y2:{3} ", x1, y1, x2, y2);
                            x1 += offsetX; y1 += offsetY;
                            if (!svgNodesOnly)
                            {
                                svgStartPath(x1, y1, form);
                                svgMoveTo(x2, y2, form);
                            }
                            else
                            {
                                gcodeDotOnly(x1, y1, form);
                                gcodeDotOnly(x2, y2, form);
                            }
                            Plotter.StopPath(form);
                        }
                        else if ((form == "polyline") || (form == "polygon"))
                        {
                            offsetX = 0;// (float)matrixElement.OffsetX;
                            offsetY = 0;// (float)matrixElement.OffsetY;
                            x1 = -1;y1 = -1;
                            if (svgComments) Plotter.Comment(" SVG-Polyline ");
                            int index = 0;
                            for (index = 0; index < points.Length; index++)
                            {   if (points[index].IndexOf(",") >= 0)
                                {
                                    string[] coord = points[index].Split(',');
                                    x = convertToPixel(coord[0]); y = convertToPixel(coord[1]);
                                    if (index == 0)
                                    {   x1 = x; y1 = y;
                                        if (!svgNodesOnly)
                                            svgStartPath(x, y, form);     // move to
                                        else
                                            gcodeDotOnly(x, y, form);
                                    }
                                    Plotter.IsPathReduceOk = true;

                                    if (!svgNodesOnly)
                                        svgMoveTo(x, y, form);            // line to
                                    else
                                        gcodeDotOnly(x, y, form);
                                }
                            }
                            if (form == "polygon")
                            {
                                if (!svgNodesOnly)
                                    svgMoveTo(x1, y1, form);              // close path
                                else
                                    gcodeDotOnly(x1, y1, form);
                            }
                            Plotter.StopPath(form);
                        }
                        else if ((form == "text") || (form == "image"))
                        {
                            Plotter.Comment(" +++++++++++++++++++++++++++++++++ ");
                            Plotter.Comment(" ++++++ " + form + " is not supported ++++ ");
                            if (form == "text")
                            {
                                Plotter.Comment(" ++ Convert Object to Path first + ");
                                Plotter.AddToHeader(" !!!!!!!!!!!!!!!!!!!! SVG Text is not supported, convert text to path in inkscape first");
                            }
                            Plotter.Comment(" +++++++++++++++++++++++++++++++++ ");
                        }
                        else
                        { if (svgComments) Plotter.Comment(" ++++++ Unknown Shape: " + form ); }

                        matrixElement = oldMatrixElement;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Convert all Path commands, check: http://www.w3.org/TR/SVG/paths.html
        /// Split command tokens
        /// </summary>
        private static void parsePath(XElement svgCode, int level)
        {
            foreach (var pathElement in svgCode.Elements(nspace + "path"))
            {
                if (pathElement != null)
                {
                    offsetX = 0;// (float)matrixElement.OffsetX;
                    offsetY = 0;// (float)matrixElement.OffsetY;
                    currentX = offsetX; currentY = offsetX;
                    firstX = null; firstY = null;
                    startPath = true;
                    startSubPath = true;
                    lastX = offsetX; lastY = offsetY;
                    string d = pathElement.Attribute("d").Value;

                    string id = d.Replace("\n","");
                    if (id.Length > 20)
                        id = id.Substring(0, 20);

                    parseAttributs(pathElement);   // process color and stroke-dasharray

                    Plotter.SetGroup(Plotter.PathToolNr); 

                    if (pathElement.Attribute("id") != null)
                        Plotter.PathId = pathElement.Attribute("id").Value;

                    if (svgComments)
                    {   if (pathElement.Attribute("id") != null)
                            Plotter.Comment(" Path level:" + level.ToString() + " id=" + pathElement.Attribute("id").Value );
                        else
                            Plotter.Comment(" SVG path=" + id );
                    }

                    oldMatrixElement = matrixElement;
                    parseTransform(pathElement,false,level);        // transform will be applied in gcodeMove

                    if (d.Length > 0)
                    {
                        // split complete path in to command-tokens
                        if (gcode.loggerTrace) Logger.Trace("  Path d {0}", d);
                        string separators = @"(?=[A-Za-z-[e]])";            
                        var tokens = Regex.Split(d, separators).Where(t => !string.IsNullOrEmpty(t));
                        int objCount = 0;
                        foreach (string token in tokens)
                            objCount += parsePathCommand(token);
                    }

                    Plotter.PenUp("");
                    matrixElement = oldMatrixElement;
                }
            }
            return;
        }

        private static bool startSubPath = true;
        private static bool startPath = true;
        private static bool startFirstElement = true;
        private static float svgWidthPx, svgHeightPx;
        private static float offsetX, offsetY;
        private static float currentX, currentY;
        private static float? firstX, firstY;
        private static float lastX, lastY;
        private static float cxMirror=0, cyMirror=0;
        private static Vector cMirror = new Vector();
        private static StringBuilder secondMove = new StringBuilder();

        /// <summary>
        /// Convert all Path commands, check: http://www.w3.org/TR/SVG/paths.html
        /// Convert command tokens
        /// </summary>
        private static int parsePathCommand(string svgPath)
        {
            var command = svgPath.Take(1).Single();
            logSource = "path " + svgPath;
            char cmd = char.ToUpper(command);
            bool absolute = (cmd==command);
            string remainingargs = svgPath.Substring(1);
            string argSeparators = @"[\s,]|(?=(?<!e)-)";// @"[\s,]|(?=-)|(-{,2})";        // support also -1.2e-3 orig. @"[\s,]|(?=-)"; 
            var splitArgs = Regex
                .Split(remainingargs, argSeparators)
                .Where(t => !string.IsNullOrEmpty(t));
// get command coordinates
            float[] floatArgs = splitArgs.Select(arg => convertToPixel(arg)).ToArray();
            int objCount = 0;

            switch (cmd)
            {
                case 'M':       // Start a new sub-path at the given (x,y) coordinate
                    #region Move
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        if (floatArgs.Length < (i+2))
                        {   Logger.Error("Move to command needs 2 arguments '{0}'", svgPath);
                            Plotter.AddToHeader(" !!! Error: Move to command needs 2 arguments '" + svgPath+"'");
                            break;
                        }
                        objCount++;
                        if (absolute || startPath)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = floatArgs[i] + lastX;   currentY = floatArgs[i + 1] + lastY; }
                        if (startSubPath)
                        {   if (svgComments) { Plotter.Comment(string.Format(" Start new subpath at {0} {1} ", floatArgs[i], floatArgs[i+1])); }
                            if (svgNodesOnly)
                                gcodeDotOnly(currentX, currentY, (command.ToString()));
                            else
                                svgStartPath(currentX, currentY, command.ToString());
                            Plotter.IsPathReduceOk = true;
                            firstX = currentX; firstY = currentY;
                            startPath = false;
                            startSubPath = false;
                        }
                        else
                        {
                            if (svgNodesOnly)
                                gcodeDotOnly(currentX, currentY, command.ToString());
                            else if (i<=1) // amount of coordinates
                            {   svgStartPath(currentX, currentY, command.ToString()); }//gcodeMoveTo(currentX, currentY, command.ToString());  // G1
                            else
                                svgMoveTo(currentX, currentY, command.ToString());  // G1
                        }
                        if (firstX == null) { firstX = currentX; }
                        if (firstY == null) { firstY = currentY; }
                        lastX = currentX; lastY = currentY;
                    }
                    cxMirror = currentX; cyMirror = currentY;
                    break;
                #endregion
                case 'Z':       // Close the current subpath
                    #region ZClose
                    if (!svgNodesOnly)
                    {
                        if (firstX == null) { firstX = currentX; }
                        if (firstY == null) { firstY = currentY; }
                        if (!gcode.isEqual((float)firstX,lastX) || !gcode.isEqual((float)firstY,lastY))
                            svgMoveTo((float)firstX, (float)firstY, command.ToString());// (isNearlyEqual((float)firstX, lastX).ToString()+"  "+ firstX.ToString()+" "+lastX.ToString()+"  "+ firstY.ToString() + " " + lastY.ToString()));// 
                    }
                    lastX = (float)firstX; lastY = (float)firstY;
                    firstX = null; firstY = null;
                    startSubPath = true;
                    Plotter.StopPath("Z");
                    break;
                #endregion
                case 'L':       // Draw a line from the current point to the given (x,y) coordinate
                    #region Line
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        if (floatArgs.Length < (i + 2))
                        {   Logger.Error("Line to command needs 2 arguments '{0}'", svgPath);
                            Plotter.AddToHeader(" !!! Error: Line to command needs 2 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY + floatArgs[i + 1]; }
                        if (svgNodesOnly)
                            gcodeDotOnly(currentX, currentY, command.ToString());
                        else
                            svgMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;
                #endregion
                case 'H':       // Draws a horizontal line from the current point (cpx, cpy) to (x, cpy)
                    #region Horizontal
                    for (int i = 0; i < floatArgs.Length; i ++)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = lastY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY; }
                        if (svgNodesOnly)
                            gcodeDotOnly(currentX, currentY, command.ToString());
                        else
                            svgMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
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
                        { currentX = lastX ; currentY = lastY + floatArgs[i]; }
                        if (svgNodesOnly)
                            gcodeDotOnly(currentX, currentY, command.ToString());
                        else
                            svgMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;
                #endregion
                case 'A':       // Draws an elliptical arc from the current point to (x, y)
                    #region Arc
                    if (svgComments) { Plotter.Comment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 7)
                    {
                        if (floatArgs.Length < (rep+7))
                        {   Logger.Error("Elliptical arc curve command needs 7 arguments '{0}'", svgPath);
                            Plotter.AddToHeader(" !!! Error: Elliptical arc curve command needs 7 arguments '" + svgPath + "'");
                            break;
                        }

                        objCount++;
                        if (svgComments) { Plotter.Comment(string.Format(" draw arc nr. {0} ", (1 + rep / 6))); }
                        float rx,ry,rot,large,sweep,nx,ny;
                        rx = floatArgs[rep] ;     ry = floatArgs[rep + 1] ;
                        rot = floatArgs[rep + 2];
                        large = floatArgs[rep + 3];
                        sweep = floatArgs[rep + 4];
                        if (absolute)
                        {   nx = floatArgs[rep + 5] + offsetX   ; ny = floatArgs[rep + 6] + offsetY;   }
                        else
                        {   nx = floatArgs[rep + 5] + lastX     ; ny = floatArgs[rep + 6] + lastY;     }
                        if (svgNodesOnly)
                            gcodeDotOnly(currentX, currentY, command.ToString());
                        else
                            importMath.calcArc(lastX, lastY, rx, ry, rot, large, sweep, nx, ny, svgMoveTo);
                        lastX = nx; lastY = ny;
                    }
                    startSubPath = true;
                    break;
#endregion
                case 'C':       // Draws a cubic Bézier curve from the current point to (x,y)
                    #region Cubic
                    if (svgComments) { Plotter.Comment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 6)
                    {
                        if (floatArgs.Length < (rep + 6))
                        {   Logger.Error("Cubic Bézier curve command needs 6 arguments '{0}'", svgPath);
                            Plotter.AddToHeader(" !!! Error: Cubic Bézier curve command needs 6 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Plotter.Comment(string.Format(" draw curve nr. {0} ", (1 + rep / 6))); }


                        if ((rep + 5) < floatArgs.Length)
                        {

                            /*
                            float cx1, cy1, cx2, cy2, cx3, cy3;
                            if (absolute)
                            {
                                cx1 = floatArgs[rep] + offsetX; cy1 = floatArgs[rep + 1] + offsetY;
                                cx2 = floatArgs[rep + 2] + offsetX; cy2 = floatArgs[rep + 3] + offsetY;
                                cx3 = floatArgs[rep + 4] + offsetX; cy3 = floatArgs[rep + 5] + offsetY;
                            }
                            else
                            {
                                cx1 = lastX + floatArgs[rep]; cy1 = lastY + floatArgs[rep + 1];
                                cx2 = lastX + floatArgs[rep + 2]; cy2 = lastY + floatArgs[rep + 3];
                                cx3 = lastX + floatArgs[rep + 4]; cy3 = lastY + floatArgs[rep + 5];
                            }

                            Point[] points = new Point[4];
                            points[0] = new Point(lastX, lastY);
                            points[1] = new Point(cx1, cy1);
                            points[2] = new Point(cx2, cy2);
                            points[3] = new Point(cx3, cy3);
                            var b = importMath.GetBezierApproximation(points, svgBezierAccuracy);
                            if (svgNodesOnly)
                            {
                                gcodeDotOnly(cx3, cy3, command.ToString());
                            }
                            else
                            {
                                for (int i = 1; i < b.Points.Count; i++)
                                    svgMoveTo((float)b.Points[i].X, (float)b.Points[i].Y, command.ToString());
                            }
                            cxMirror = cx3 - (cx2 - cx3); cyMirror = cy3 - (cy2 - cy3);
                            lastX = cx3; lastY = cy3;
                            */
                            Point Off = new Point(offsetX, offsetY);
                            if (!absolute)
                                Off = new Point(lastX, lastY);
                            Vector c1 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                            Vector c2 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;
                            Vector c3 = new Vector(floatArgs[rep + 4], floatArgs[rep + 5]) + (Vector)Off;
                            if (svgNodesOnly)
                                gcodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                            else
                                importMath.calcCubicBezier(new Point(lastX, lastY), (Point)c1, (Point)c2, (Point)c3, svgMoveTo, command.ToString());

                            lastX = (float)c3.X; lastY = (float)c3.Y;
                            cxMirror = (float)(2 * c3.X - c2.X); cyMirror = (float)(2 * c3.Y - c2.Y);
                            cMirror = c3 * 2 - c2;


                        }
                        else
                        { Plotter.Comment(string.Format(" Missing argument after {0} ", rep)); }
                    }
                    startSubPath = true;
                    break;
                #endregion
                case 'S':       // Draws a cubic Bézier curve from the current point to (x,y)
                    #region Small Cubic
                    if (svgComments) { Plotter.Comment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        if (floatArgs.Length < (rep + 4))
                        {   Logger.Error("Smooth curveto command needs 4 arguments '{0}'", svgPath);
                            Plotter.AddToHeader(" !!! Error: smooth curveto command needs 4 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Plotter.Comment(string.Format(" draw curve nr. {0} ", (1 + rep / 4))); }
                        
                        /*
                        float cx2, cy2, cx3, cy3;
                        if (absolute)
                        {
                            cx2 = floatArgs[rep] + offsetX;       cy2 = floatArgs[rep + 1] + offsetY;
                            cx3 = floatArgs[rep + 2] + offsetX;   cy3 = floatArgs[rep + 3] + offsetY;
                        }
                        else
                        {
                            cx2 = lastX + floatArgs[rep];       cy2 = lastY + floatArgs[rep + 1];
                            cx3 = lastX + floatArgs[rep + 2];   cy3 = lastY + floatArgs[rep + 3];
                        }
                        Point[] points = new Point[4];
                        points[0] = new Point(lastX, lastY);
                        points[1] = new Point(cxMirror, cyMirror);
                        points[2] = new Point(cx2, cy2);
                        points[3] = new Point(cx3, cy3);
                        var b = importMath.GetBezierApproximation(points, svgBezierAccuracy);
                        if (svgNodesOnly)
                        {
                            gcodeDotOnly(cx3, cy3, command.ToString());
                        }
                        else
                        {
                            for (int i = 1; i < b.Points.Count; i++)
                                svgMoveTo((float)b.Points[i].X, (float)b.Points[i].Y, command.ToString());
                        }
                        cxMirror = cx3 - (cx2 - cx3); cyMirror = cy3 - (cy2 - cy3);
                        lastX = cx3; lastY = cy3;
                        */
                        Point Off = new Point(offsetX, offsetY);
                        if (!absolute)
                            Off = new Point(lastX, lastY);
                        Vector c2 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                        Vector c3 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;
                        if (svgNodesOnly)
                            gcodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                        else
                            importMath.calcCubicBezier(new Point(lastX, lastY), (Point)cMirror, (Point)c2, (Point)c3, svgMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;
                        cxMirror = (float)(2 * c3.X - c2.X); cyMirror = (float)(2 * c3.Y - c2.Y);
                        cMirror = c3 * 2 - c2;


                    }
                    startSubPath = true;
                    break;
                #endregion
                case 'Q':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    #region Quadratic
                    if (svgComments) { Plotter.Comment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        if (floatArgs.Length < (rep + 4))
                        {   Logger.Error("Quadratic Bézier curveto command needs 4 arguments '{0}'", svgPath);
                            Plotter.AddToHeader(" !!! Error: Quadratic Bézier curveto command needs 4 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Plotter.Comment(string.Format(" draw curve nr. {0} ", (1 + rep / 4))); }
                                            
                        Point Off = new Point(offsetX, offsetY);
                        if (!absolute)
                            Off = new Point(lastX, lastY);
                        Vector c2 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                        Vector c3 = new Vector(floatArgs[rep + 2], floatArgs[rep + 3]) + (Vector)Off;
                        if (svgNodesOnly)
                            gcodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                        else
                            importMath.calcQuadraticBezier(new Point(lastX, lastY), (Point)c2, (Point)c3, svgMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;
                        cxMirror = (float)(2 * c3.X - c2.X); cyMirror = (float)(2 * c3.Y - c2.Y);
                        cMirror = c3 * 2 - c2;
                    }
                    startSubPath = true;
                    break;
                #endregion
                case 'T':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    #region TQuadratic
                    if (svgComments) { Plotter.Comment(string.Format(" Command {0} {1} ", command.ToString(), ((absolute == true) ? "absolute" : "relative"))); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 2)
                    {
                        if (floatArgs.Length < (rep + 2))
                        {   Logger.Error("Smooth quadratic Bézier curveto command needs 2 arguments '{0}'", svgPath);
                            Plotter.AddToHeader(" !!! Error: Smooth quadratic Bézier curveto command needs 2 arguments '" + svgPath + "'");
                            break;
                        }
                        objCount++;
                        if (svgComments) { Plotter.Comment(string.Format(" draw curve nr. {0} ", (1 + rep / 2))); }

                        Point Off = new Point(offsetX, offsetY);
                        if (!absolute)
                            Off = new Point(lastX, lastY);
                        Vector c3 = new Vector(floatArgs[rep + 0], floatArgs[rep + 1]) + (Vector)Off;
                        if (svgNodesOnly)
                            gcodeDotOnly((float)c3.X, (float)c3.Y, command.ToString());
                        else
                            importMath.calcQuadraticBezier(new Point(lastX, lastY), (Point)cMirror, (Point)c3, svgMoveTo, command.ToString());

                        lastX = (float)c3.X; lastY = (float)c3.Y;
                        cxMirror = (float)c3.X; cyMirror = (float)c3.Y;
                        cMirror = c3;
                    }
                    startSubPath = true;
                    break;
                #endregion
                default:
                    if (svgComments) Plotter.Comment(" *********** unknown: " + command.ToString()+ " ***** ");
                    break;
            }
            return objCount;
        }
// Prepare G-Code

        /// <summary>
        /// Transform XY coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static Point translateXY(float x, float y)
        {   Point coord = new Point(x,y);
            return translateXY(coord);
        }
        private static Point translateXY(Point pointStart)
        {   Point pointResult = matrixElement.Transform(pointStart);
            return pointResult;
        }
        /// <summary>
        /// Transform I,J coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static Point translateIJ(float i, float j)
        {   Point coord = new Point(i,j);
            return translateIJ(coord);
        }
        private static Point translateIJ(Point pointStart)
        {   Point pointResult = pointStart;
            double tmp_i = pointStart.X, tmp_j = pointStart.Y;
            pointResult.X = tmp_i * matrixElement.M11 + tmp_j * matrixElement.M21;  // - tmp
            pointResult.Y = tmp_i * matrixElement.M12 + tmp_j * matrixElement.M22; // tmp_i*-matrix     // i,j are relative - no offset needed, but perhaps rotation
            return pointResult;
        }

        private static void gcodeDotOnly(float x, float y, string cmt)
        {
            if (!svgComments)
                cmt = "";
            svgStartPath(x, y, cmt);
            Plotter.PenDown(cmt);
            Plotter.PenUp(cmt,false);
        }

        /// <summary>
        /// Insert G0 and Pen down gcode command
        /// </summary>
        private static void svgStartPath(float x, float y, string cmt)
        {   Point tmp = translateXY(x, y);  // convert from SVG-Units to GCode-Units
            Plotter.StartPath(tmp, cmt);  
        }

        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void svgMoveTo(float x, float y, string cmt)
        {
            if (!svgComments)
                cmt = "";
            svgMoveTo(new Point(x, y), cmt);
        }

        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void svgMoveTo(Point orig, string cmt)
        {   
            Point tmp = translateXY(orig);
            Plotter.MoveTo(tmp, cmt);
        }

        /// <summary>
        /// Insert G2/G3 gcode command
        /// </summary>
        private static void svgArcToCCW(float x, float y, float i, float j, string cmt)
        {   
            Point coordxy = translateXY(x, y);
            Point coordij = translateIJ(i, j);
            Plotter.ArcToCCW(coordxy, coordij, cmt);
        }
    }
}
