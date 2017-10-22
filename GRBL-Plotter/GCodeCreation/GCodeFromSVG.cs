/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2017 Sven Hasemann contact: svenhb@web.de

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
/*  2016-07-18 get stroke-color from shapes, use GIMP-palette information to find tool-nr related to stroke-color
    add gcode for tool change
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
//using System.Drawing;

namespace GRBL_Plotter
{
    class GCodeFromSVG
    {
        private static int svgToolMax = 100;            // max amount of tools
        private static StringBuilder[] gcodeString = new StringBuilder[svgToolMax];
        private static int gcodeStringIndex = 0;
        private static StringBuilder finalString = new StringBuilder();

        // following settings will be read from Properties.Settings.Default in startConvert()
        private static int svgBezierAccuracy = 6;       // applied line segments at bezier curves
        private static bool svgScaleApply = true;       // try to scale final GCode if true
        private static float svgMaxSize = 100;          // final GCode size (greater dimension) if scale is applied
        private static bool svgClosePathExtend = true;  // if true move to first and second point of path to get overlap
        private static bool svgToolColor = true;        // if true take tool nr. from nearest pallet entry
        private static bool svgToolSort = true;         // if true sort objects by tool-nr. (to avoid back and forth pen change)
        private static int svgToolIndex = 0;            // last index

        private static bool svgNodesOnly = true;        // if true only do pen-down -up on given coordinates

        private static bool svgPauseElement = true;     // if true insert GCode pause M0 before each element
        private static bool svgPausePenDown = true;     // if true insert pause M0 before pen down
        private static bool svgComments = true;         // if true insert additional comments into GCode

        private static bool gcodeReduce = false;        // if true remove G1 commands if distance is < limit
        private static float gcodeReduceVal = .1f;        // limit when to remove G1 commands

        private static float gcodeXYFeed = 2000;        // XY feed to apply for G1

        private static bool gcodeToolChange = false;          // Apply tool exchange command
        private static int gcodeToolNr = 0;

        // Using Z-Axis for Pen up down
        private static bool gcodeZApply = true;         // if true insert Z movements for Pen up/down
        private static float gcodeZUp = 2;              // Z-up position
        private static float gcodeZDown = -2;           // Z-down position
        private static float gcodeZFeed = 500;          // Z feed to apply for G1

        // Using Spindle pwr. to switch on/off laser
        private static bool gcodeUseSpindle = false; // Switch on/off spindle for Pen down/up (M3/M5)

        private static float gcodeScale = 1;            // finally scale with this factor if svgScaleApply and svgMaxSize
        private static Matrix[] matrixGroup = new Matrix[10];   // store SVG-Group transformation matrixes
        private static Matrix matrixElement = new Matrix();     // store finally applied matrix

        /// <summary>
        /// Entrypoint for conversion: apply file-path or file-URL
        /// </summary>
        /// <param name="file">String keeping file-name or URL</param>
        /// <returns>String with GCode of imported data</returns>
        public static string ConvertFile(string file)
        {
            if (file == "")
            {   MessageBox.Show("Empty file name"); return ""; }

            gcodeStringIndex = 0;
            for (int i = 0; i < svgToolMax; i++)    // hold gcode snippes for later sorting
            {
                gcodeString[i] = new StringBuilder();
                gcodeString[i].Clear();
            }
            gcode.setup();          // initialize GCode creation (get stored settings for export)
            finalString.Clear();
            gcode.PenUp(finalString,"SVG Start ");

            XElement svgCode;
            if (file.Substring(0, 4) == "http")
            {
                string content = "";
                using (var wc = new System.Net.WebClient())
                {
                    try { content = wc.DownloadString(file); }
                    catch { MessageBox.Show("Could not load content from " + file); return ""; }
                }
                if (content != "")
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(content);
                    MemoryStream stream = new MemoryStream(byteArray);
                    svgCode = XElement.Load(stream, LoadOptions.None);
                    startConvert(svgCode);
                }
            }
            else
            {   if (File.Exists(file))
                {
                    try
                    {   //string strcode= File.ReadAllText(file);
                        //strcode.Replace((char)0x00, ' ');
                        svgCode = XElement.Load(file, LoadOptions.None);    // PreserveWhitespace);
                        startConvert(svgCode);
                    }
                    catch (Exception e)
                    { MessageBox.Show("Error '"+e.ToString()+"' in XML file " + file+"\r\n\r\nTry to save file with other encoding e.g. UTF-8"); return ""; }
                }
                else { MessageBox.Show("File does not exist: " + file); return ""; }
            }

//            gcodeString[gcodeStringIndex].Replace(',', '.');

            if (gcodeUseSpindle) gcode.SpindleOn(finalString, "Start spindle - Option Z-Axis");
            if (svgToolSort)
            {
                int toolnr;
                svgPalette.sortByToolNr();
                for (int i = 0; i < svgToolIndex; i++)
                {
                    svgPalette.setIndex(i);                 // set index in svgPalette
                    toolnr = svgPalette.indexToolNr();      // get value from set index
                    if ((toolnr >= 0) && (gcodeString[toolnr].Length > 1))
                    {
                        finalString.Append("\r\n\r\n");
                        if ((gcodeToolChange) && svgPalette.indexUse())
                        {   gcode.Tool(finalString, toolnr, svgPalette.indexName());
                        }
                        finalString.Append(gcodeString[toolnr]);
                    }
                }
            }
            else
                finalString.Append(gcodeString[0]);
            if (gcodeUseSpindle) gcode.SpindleOff(finalString, "Stop spindle - Option Z-Axis");
            string header = gcode.GetHeader("SVG import",file);
            string footer = gcode.GetFooter();
            return header + finalString.ToString().Replace(',', '.') +footer;
        }

        /// <summary>
        /// Set defaults and parse main element of SVG-XML
        /// </summary>
        private static void startConvert(XElement svgCode)
        {
            svgBezierAccuracy = (int)Properties.Settings.Default.importSVGBezier;
            svgScaleApply = Properties.Settings.Default.importSVGRezise;
            svgMaxSize = (float)Properties.Settings.Default.importSVGMaxSize;
            svgClosePathExtend = Properties.Settings.Default.importSVGPathExtend;
            svgPauseElement = Properties.Settings.Default.importSVGPauseElement;
            svgPausePenDown = Properties.Settings.Default.importSVGPausePenDown;
            svgComments = Properties.Settings.Default.importSVGAddComments;

            gcodeReduce = Properties.Settings.Default.importSVGReduce;
            gcodeReduceVal = (float)Properties.Settings.Default.importSVGReduceLimit;

            gcodeXYFeed = (float)Properties.Settings.Default.importGCXYFeed;
            gcodeZApply = Properties.Settings.Default.importGCZEnable;
            gcodeZUp = (float)Properties.Settings.Default.importGCZUp;
            gcodeZDown = (float)Properties.Settings.Default.importGCZDown;
            gcodeZFeed = (float)Properties.Settings.Default.importGCZFeed;
            gcodeUseSpindle = Properties.Settings.Default.importGCZEnable;

            svgToolColor    = Properties.Settings.Default.importSVGToolColor;
            svgToolSort     = Properties.Settings.Default.importSVGToolSort;
            gcodeToolChange = Properties.Settings.Default.importGCTool;

            svgNodesOnly    = Properties.Settings.Default.importSVGNodesOnly;

            svgToolIndex = svgPalette.init();

            startFirstElement = true;
            gcodeScale = 1;
            currentX = 0; currentY = 0;
            offsetX = 0; offsetY = 0;

            matrixElement.SetIdentity();
            for (int i=0; i<matrixGroup.Length;i++)
                matrixGroup[i].SetIdentity(); 

            parseGlobals(svgCode);
            if (!svgNodesOnly)
                parseBasicElements(svgCode,1);
            parsePath(svgCode,1);
            parseGroup(svgCode,1);
            return;
        }

        /// <summary>
        /// Parse SVG dimension (viewbox, width, height)
        /// </summary>
        private static XNamespace nspace = "http://www.w3.org/2000/svg";
        private static void parseGlobals(XElement svgCode)
        {
            Matrix tmp = new Matrix(1, 0, 0, 1, 0, 0); // m11, m12, m21, m22, offsetx, offsety
            svgWidthPx = 0;
            svgHeightPx = 0;
            float vbOffX = 0;
            float vbOffY = 0;
            float vbWidth = 0;
            float vbHeight = 0;
            if (svgCode.Attribute("viewBox") != null)
            {
                string viewbox = svgCode.Attribute("viewBox").Value.Replace(' ', '|');
                var split = viewbox.Split('|');
                vbOffX = -floatParse(split[0]);
                vbOffY = -floatParse(split[1]);
                vbWidth  = floatParse(split[2]);    
                vbHeight = floatParse(split[3].TrimEnd(')'));
                tmp.M11 = 1; tmp.M22 = -1;      // flip Y
                tmp.OffsetY = vbHeight;
                if (svgComments) gcodeString[gcodeStringIndex].AppendLine("( SVG viewbox :" + viewbox+" )");
            }

            if (svgCode.Attribute("width") != null)
            {
                svgWidthPx = floatParse(svgCode.Attribute("width").Value);
                float svgWidthUnit = floatParse(svgCode.Attribute("width").Value.Replace("mm", ""));

                if (svgComments) gcodeString[gcodeStringIndex].AppendLine("( SVG width :" + svgCode.Attribute("width").Value + " )");
                tmp.M11 = svgWidthUnit / svgWidthPx; // get desired scale
                if (vbWidth > 0)
                {   tmp.M11 = svgWidthUnit / vbWidth;
                    tmp.OffsetX = vbOffX * svgWidthUnit / vbWidth;
                }
            }

            if (svgCode.Attribute("height") != null)
            {
                svgHeightPx = floatParse(svgCode.Attribute("height").Value);
                float svgHeightUnit = floatParse(svgCode.Attribute("height").Value.Replace("mm", ""));

                if (svgComments) gcodeString[gcodeStringIndex].AppendLine("( SVG height :" + svgCode.Attribute("height").Value + " )");
                tmp.M22 = -svgHeightUnit / svgHeightPx;   // get desired scale and flip vertical
                tmp.OffsetY = svgHeightUnit;
                if (vbHeight > 0)
                {   tmp.M22 = -svgHeightUnit / vbHeight;
                    tmp.OffsetY = -vbOffX * svgHeightUnit / vbHeight + svgHeightUnit;
                }
            }
            matrixGroup[0] = tmp;
            matrixElement = tmp;
            if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( Inital Matrix {0} )\r\n", tmp.ToString());

            if ((svgWidthPx > 0) && (svgHeightPx > 0))
            {
                if (svgScaleApply)
                {
                    gcodeScale = svgMaxSize / Math.Max(svgWidthPx, svgHeightPx);
                    if (svgComments) 
                        gcodeString[gcodeStringIndex].AppendFormat("( Scale to X={0} Y={1} f={2} )\r\n", svgWidthPx * gcodeScale, svgHeightPx * gcodeScale, gcodeScale);
                }
            }
            else
                if (svgComments) gcodeString[gcodeStringIndex].Append("( SVG Dimension not given )");

            return;
        }

        /// <summary>
        /// Parse Group-Element and included elements
        /// </summary>
        private static void parseGroup(XElement svgCode, int level)
        {   foreach (XElement groupElement in svgCode.Elements(nspace + "g"))
            {
                if (svgComments)
                    if (groupElement.Attribute("id") != null)
                        gcodeString[gcodeStringIndex].Append("\r\n( Group level:"+level.ToString()+" id=" + groupElement.Attribute("id").Value + " )\r\n");
                parseTransform(groupElement,true, level);   // transform will be applied in gcodeMove
                if (!svgNodesOnly)
                    parseBasicElements(groupElement, level);
                parsePath(groupElement, level);
                parseGroup(groupElement, level+1);
            }
            return;
        }

        /// <summary>
        /// Parse Transform information - more information here: http://www.w3.org/TR/SVG/coords.html
        /// transform will be applied in gcodeMove
        /// </summary>
        private static void parseTransform(XElement element,bool isGroup, int level)
        {   Matrix tmp = new Matrix(1, 0, 0, 1, 0, 0); // m11, m12, m21, m22, offsetx, offsety
            bool transf = false;
            if (element.Attribute("transform") != null)
            {
                transf = true;
                string transform = element.Attribute("transform").Value;
                if ((transform != null) && (transform.IndexOf("translate") >= 0))
                {
                    var coord = getTextBetween(transform, "translate(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');

                    //MessageBox.Show(transform+"\r\n>"+coord + "< "+ split[0]);
                    tmp.OffsetX = floatParse(split[0]);
                    if (split.Length > 1)
                        tmp.OffsetY = floatParse(split[1].TrimEnd(')'));
                    if (svgComments) gcodeString[gcodeStringIndex].Append(string.Format("( SVG-Translate {0} {1} )\r\n", tmp.OffsetX, tmp.OffsetY));
                }
                if ((transform != null) && (transform.IndexOf("scale") >= 0))
                {
                    var coord = getTextBetween(transform, "scale(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');
                    //MessageBox.Show(transform + "\r\n>" + coord + "< " + split[0]);
                    tmp.M11 = floatParse(split[0]);
                    if (split.Length > 1)
                        tmp.M22 = floatParse(split[1].TrimEnd(')'));
                    if (svgComments) gcodeString[gcodeStringIndex].Append(string.Format("( SVG-Scale {0} {1} )\r\n", tmp.M11, tmp.M22));
                }
                if ((transform != null) && (transform.IndexOf("rotate") >= 0))
                {
                    var coord = getTextBetween(transform, "rotate(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');
                    float angle = floatParse(split[0]) * (float)Math.PI / 180;
                    tmp.M11 = Math.Cos(angle); tmp.M12 = Math.Sin(angle);
                    tmp.M21 = -Math.Sin(angle); tmp.M22 = Math.Cos(angle);

                    if (svgComments) gcodeString[gcodeStringIndex].Append(string.Format("( SVG-Rotate {0} )\r\n", angle));
                }
                if ((transform != null) && (transform.IndexOf("matrix") >= 0))
                {
                    var coord = getTextBetween(transform, "matrix(", ")");
                    var split = coord.Split(',');
                    if (coord.IndexOf(',') < 0)
                        split = coord.Split(' ');
                    tmp.M11 = floatParse(split[0]);     // a    scale x         a c e
                    tmp.M12 = floatParse(split[1]);     // b                    b d f
                    tmp.M21 = floatParse(split[2]);     // c                    0 0 1
                    tmp.M22 = floatParse(split[3]);     // d    scale y
                    tmp.OffsetX = floatParse(split[4]); // e    offset x
                    tmp.OffsetY = floatParse(split[5]); // f    offset y
                    if (svgComments) gcodeString[gcodeStringIndex].Append(string.Format("\r\n( SVG-Matrix {0} {1} {2} )\r\n", coord.Replace(',', '|'), level, isGroup));
                }
                //}
                if (isGroup)
                {
                    matrixGroup[level].SetIdentity();
                    if (level > 0)
                        matrixGroup[level] = Matrix.Multiply(tmp, matrixGroup[level - 1]);
                    else
                        matrixGroup[level] = tmp;
                    matrixElement = matrixGroup[level];
                }
                else
                {   matrixElement = Matrix.Multiply(tmp, matrixGroup[level]);                }

                if (svgComments && transf)
                {
                    for (int i = 0; i <= level; i++)
                        gcodeString[gcodeStringIndex].AppendFormat("( gc-Matrix level[{0}] {1} )\r\n", i, matrixGroup[i].ToString());

                    if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( gc-Scale {0} {1} )\r\n", matrixElement.M11, matrixElement.M22);
                    if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( gc-Offset {0} {1} )\r\n", matrixElement.OffsetX, matrixElement.OffsetY);
                }
            }
            return;
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
        private static float floatParse(string str, float ext=1)
        {
            bool percent = false;
            float factor = 1;
            if (str.IndexOf("pt") > 0) { factor = 1.25f; }
            if (str.IndexOf("pc") > 0) { factor = 15f; }
            if (str.IndexOf("mm") > 0) { factor = 3.543307f; }
            if (str.IndexOf("cm") > 0) { factor = 35.43307f; }
            if (str.IndexOf("in") > 0) { factor = 90f; }
            if (str.IndexOf("em") > 0) { factor = 150f; }
            if (str.IndexOf("%") > 0)  { percent = true; }
            str = str.Replace("pt", "").Replace("pc", "").Replace("mm", "").Replace("cm", "").Replace("in", "").Replace("em ", "").Replace("%", "").Replace("px", "");

            if (percent)
                return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat) * ext/100;
            else
                return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat) * factor;
        }

        private static string getColor(XElement pathElement)
        {
            string style = "";
            string stroke_color = "000000";        // default=black
            if (pathElement.Attribute("style") != null)
            {
                int start, end;
                style = pathElement.Attribute("style").Value;
                start = style.IndexOf("stroke:#");
                if (start >= 0)
                {
                    end = style.IndexOf(';', start);
                    if (end > start)
                        stroke_color = style.Substring(start + 8, end - start - 8);
                }
                return stroke_color;
            }
            return "";
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
                        string myColor = getColor(pathElement);
                        int myTool = svgPalette.getToolNr(myColor,0);

                        if ((svgToolSort) && (myTool >= 0))
                            gcodeStringIndex = myTool;

                        if (svgComments)
                        {
                            if (pathElement.Attribute("id") != null)
                                gcodeString[gcodeStringIndex].Append("\r\n( Basic shape level:" + level.ToString() + " id=" + pathElement.Attribute("id").Value + " )\r\n");
                            gcodeString[gcodeStringIndex].AppendFormat("\r\n(SVG color=#{0})\r\n", myColor);
                        }
                        gcodeTool(myTool);

                        if (startFirstElement)
                        { gcodePenUp("1st shape"); startFirstElement = false; }

                        offsetX = 0; offsetY = 0;
                        parseTransform(pathElement, false, level);  // transform will be applied in gcodeMove
                        float x=0, y=0, x1=0, y1=0, x2=0, y2=0, width=0, height=0, rx=0, ry=0,cx=0,cy=0,r=0;
                        string[] points= {""};
                        if (pathElement.Attribute("x") !=null)      x = floatParse(pathElement.Attribute("x").Value);
                        if (pathElement.Attribute("y") != null)     y = floatParse(pathElement.Attribute("y").Value);
                        if (pathElement.Attribute("x1") != null)    x1 = floatParse(pathElement.Attribute("x1").Value);
                        if (pathElement.Attribute("y1") != null)    y1 = floatParse(pathElement.Attribute("y1").Value);
                        if (pathElement.Attribute("x2") != null)    x2 = floatParse(pathElement.Attribute("x2").Value);
                        if (pathElement.Attribute("y2") != null)    y2 = floatParse(pathElement.Attribute("y2").Value);
                        if (pathElement.Attribute("width") != null)  width =floatParse(pathElement.Attribute("width").Value,  svgWidthPx);
                        if (pathElement.Attribute("height") != null) height=floatParse(pathElement.Attribute("height").Value, svgHeightPx);
                        if (pathElement.Attribute("rx") != null)    rx=floatParse(pathElement.Attribute("rx").Value);
                        if (pathElement.Attribute("ry") != null)    ry=floatParse(pathElement.Attribute("ry").Value);
                        if (pathElement.Attribute("cx") != null)    cx=floatParse(pathElement.Attribute("cx").Value);
                        if (pathElement.Attribute("cy") != null)    cy=floatParse(pathElement.Attribute("cy").Value);
                        if (pathElement.Attribute("r") != null)     r=floatParse(pathElement.Attribute("r").Value);
                        if (pathElement.Attribute("points") != null) points = pathElement.Attribute("points").Value.Split(' ');

                        if (svgPauseElement||svgPausePenDown) { gcode.Pause(gcodeString[gcodeStringIndex], "Pause before path"); }
                        if (form == "rect")
                        {
                            if (ry == 0) { ry = rx; }
                            else if (rx == 0) { rx = ry; }
                            if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( SVG-Rect x:{0} y:{1} width:{2} height:{3} )\r\n", x, y, width, height);
                            x += offsetX; y += offsetY;
                            gcodeStartPath(x + rx, y + height, form);
                            gcodeMoveTo(x + width - rx, y + height, form);
                            if (rx > 0) gcodeArcToCCW(x + width, y + height - ry, 0, ry, form);
                            gcodeMoveTo(x + width, y + ry, form);                        // upper right
                            if (rx > 0) gcodeArcToCCW(x + width - rx, y, -rx, 0, form);
                            gcodeMoveTo(x + rx, y, form);                                // upper left
                            if (rx > 0) gcodeArcToCCW(x, y + ry, 0, -ry, form);
                            gcodeMoveTo(x, y + height - ry, form);                       // lower left
                            if (rx > 0) gcodeArcToCCW(x + rx, y + height, rx, 0, form);

                            gcodeStopPath(form);
                        }
                        else if (form == "circle")
                        {
                            if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( circle cx:{0} cy:{1} r:{2} )\r\n", cx, cy, r);
                            cx += offsetX; cy += offsetY;
                            gcodeStartPath(cx + r, cy, form);
                            gcodeArcToCCW(cx + r, cy, -r, 0, form);
                            gcodeStopPath(form);
                        }
                        else if (form == "ellipse")
                        {
                            if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( ellipse cx:{0} cy:{1} rx:{2}  ry:{2})\r\n", cx, cy, rx, ry);
                            cx += offsetX; cy += offsetY;
                            gcodeStartPath(cx + rx, cy, form);
                            isReduceOk = true;
                            calcArc(cx + rx, cy, rx, ry, 0, 1, 1, cx - rx, cy);
                            calcArc(cx - rx, cy, rx, ry, 0, 1, 1, cx + rx, cy);
                            gcodeStopPath(form);

                        }
                        else if (form == "line")
                        {
                            if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( SVG-Line x1:{0} y1:{1} x2:{2} y2:{3} )\r\n", x1, y1, x2, y2);
                            x1 += offsetX; y1 += offsetY;
                            gcodeStartPath(x1, y1, form);
                            gcodeMoveTo(x2, y2, form);
                            gcodeStopPath(form);
                        }
                        else if ((form == "polyline") || (form == "polygon"))
                        {
                            offsetX = 0;// (float)matrixElement.OffsetX;
                            offsetY = 0;// (float)matrixElement.OffsetY;
                            if (svgComments) gcodeString[gcodeStringIndex].AppendFormat("( SVG-Polyline )\r\n");
                            string[] coord = points[0].Split(',');
                            x = floatParse(coord[0]); y = floatParse(coord[1]);
                            x1 = x;y1 = y;
                            gcodeStartPath(x, y, form);
                            isReduceOk = true;
                            for (int i = 1; i < points.Length; i++)
                            {   if (points[i].Length > 3)
                                {
                                    coord = points[i].Split(',');
                                    x = floatParse(coord[0]); y = floatParse(coord[1]);
                                    x += offsetX; y += offsetY;
                                    gcodeMoveTo(x, y, form);
                                }
                            }
                            if (form == "polygon")
                                gcodeMoveTo(x1, y1, form);
                            gcodeStopPath(form);
                        }
                        else if ((form == "text") || (form == "image"))
                        {
                            gcodeString[gcodeStringIndex].AppendLine("( +++++++++++++++++++++++++++++++++ )");
                            gcodeString[gcodeStringIndex].AppendLine("( ++++++ " + form + " is not supported ++++ )");
                            gcodeString[gcodeStringIndex].AppendLine("( +++++++++++++++++++++++++++++++++ )");
                        }
                        else
                        { if (svgComments) gcodeString[gcodeStringIndex].Append("( ++++++ Unknown Shape: " + form + " )"); }
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
                    string id = d;
                    if (id.Length > 20)
                        id = id.Substring(0, 20);

                    string myColor = getColor(pathElement);
                    int myTool = svgPalette.getToolNr(myColor,0);// svgToolTable[myIndex].toolnr;

                    if ((svgToolSort) && (myTool >= 0))
                    {   if(penIsDown) gcodePenUp("Start path");
                        gcodeStringIndex = myTool;
                    }

                    // gcodeString[gcodeStringIndex].Append("( Start path )\r\n");
                    if (svgComments)
                    {
                        if (pathElement.Attribute("id") != null)
                            gcodeString[gcodeStringIndex].Append("\r\n( Path level:" + level.ToString() + " id=" + pathElement.Attribute("id").Value + " )\r\n");
                        else
                            gcodeString[gcodeStringIndex].Append("\r\n( SVG path=" + id + " )\r\n");
                        gcodeString[gcodeStringIndex].AppendFormat("\r\n(SVG color=#{0})\r\n", myColor);
                    }

                    if (pathElement.Attribute("id") != null)
                        id = pathElement.Attribute("id").Value;
                    parseTransform(pathElement,false,level);        // transform will be applied in gcodeMove

                    gcodeTool(myTool);

                    if (d.Length > 0)
                    {
        // split complete path in to command-tokens
                        if (svgPauseElement||svgPausePenDown) { gcode.Pause(gcodeString[gcodeStringIndex], "Pause before path"); }
                        string separators = @"(?=[A-Za-z-[e]])";            
                        var tokens = Regex.Split(d, separators).Where(t => !string.IsNullOrEmpty(t));
                        int objCount = 0;
                        foreach (string token in tokens)
                            objCount += parsePathCommand(token);
                        //if (svgComments) { svgInfo += string.Format("( {0}: {1} Elements )\r\n", id, objCount); }
                    }
                    gcodePenUp("End path");
                    //gcodeString[gcodeStringIndex].Append("( End path )\r\n");
                }
            }
            return;
        }

        private static bool penIsDown = true;
        private static bool startSubPath = true;
        private static bool startPath = true;
        private static bool startFirstElement = true;
        private static float svgWidthPx, svgHeightPx;
        private static float offsetX, offsetY;
        private static float currentX, currentY;
        private static float? firstX, firstY;
        private static float lastX, lastY;
        private static float cxMirror=0, cyMirror=0;
        private static StringBuilder secondMove = new StringBuilder();

        /// <summary>
        /// Convert all Path commands, check: http://www.w3.org/TR/SVG/paths.html
        /// Convert command tokens
        /// </summary>
        private static int parsePathCommand(string svgPath)
        {
            var command = svgPath.Take(1).Single();
            char cmd = char.ToUpper(command);
            bool absolute = (cmd==command);
            string remainingargs = svgPath.Substring(1);
            string argSeparators = @"[\s,]|(?=(?<!e)-)";// @"[\s,]|(?=-)|(-{,2})";        // support also -1.2e-3 orig. @"[\s,]|(?=-)"; 
            var splitArgs = Regex
                .Split(remainingargs, argSeparators)
                .Where(t => !string.IsNullOrEmpty(t));
// get command coordinates
            float[] floatArgs = splitArgs.Select(arg => floatParse(arg)).ToArray();
            int objCount = 0;

            switch (cmd)
            {
                case 'M':       // Start a new sub-path at the given (x,y) coordinate
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        objCount++;
                        if (absolute || startPath)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = floatArgs[i] + lastX;   currentY = floatArgs[i + 1] + lastY; }
                        if (startSubPath)
                        {   if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( Start new subpath at {0} {1} )\r\n", floatArgs[i], floatArgs[i+1]); }
//                            pathCount = 0;
//                            gcodeStopPath("");
                            if (svgNodesOnly)
                                gcodeDotOnly(currentX, currentY, command.ToString());
                            else
                                gcodeStartPath(currentX, currentY, command.ToString());
                            isReduceOk = true;
                            firstX = currentX; firstY = currentY;
                            startPath = false;
                            startSubPath = false;
                        }
                        else
                        {
                            if (svgNodesOnly)
                                gcodeDotOnly(currentX, currentY, command.ToString());
                            else
                                gcodeMoveTo(currentX, currentY, command.ToString());  // G1
                        }
                        if (firstX == null) { firstX = currentX; }
                        if (firstY == null) { firstY = currentY; }
                        lastX = currentX; lastY = currentY;
                    }
                    cxMirror = currentX; cyMirror = currentY;
                    break;

                case 'Z':       // Close the current subpath
                    if (!svgNodesOnly)
                        gcodeMoveTo((float)firstX, (float)firstY, command.ToString());    // G1
                    lastX = (float)firstX; lastY = (float)firstY;
                    firstX = null; firstY = null;
                    startSubPath = true;
                    if ((svgClosePathExtend) && (!svgNodesOnly))
                    {   gcodeString[gcodeStringIndex].Append(secondMove); }
                    gcodeStopPath("Z");
                    break;

                case 'L':       // Draw a line from the current point to the given (x,y) coordinate
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY + floatArgs[i + 1]; }
                        if (svgNodesOnly)
                            gcodeDotOnly(currentX, currentY, command.ToString());
                        else
                            gcodeMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;

                case 'H':       // Draws a horizontal line from the current point (cpx, cpy) to (x, cpy)
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
                            gcodeMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;

                case 'V':       // Draws a vertical line from the current point (cpx, cpy) to (cpx, y)
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
                            gcodeMoveTo(currentX, currentY, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;

                case 'A':       // Draws an elliptical arc from the current point to (x, y)
                    if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( Command {0} {1} )\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 7)
                    {
                        objCount++;
                        if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( draw arc nr. {0} )\r\n", (1 + rep / 6)); }
                        float rx,ry,rot,large,sweep,nx,ny;
                        rx = floatArgs[rep] ;     ry = floatArgs[rep + 1] ;
                        rot = floatArgs[rep + 2];
                        large = floatArgs[rep + 3];
                        sweep = floatArgs[rep + 4];
                        if (absolute)
                        {
                            nx = floatArgs[rep + 5] + offsetX   ; ny = floatArgs[rep + 6] + offsetY;
                        }
                        else
                        {
                            nx = floatArgs[rep + 5] + lastX     ; ny = floatArgs[rep + 6] + lastY;
                        }
                        if (svgNodesOnly)
                            gcodeDotOnly(currentX, currentY, command.ToString());
                        else
                            calcArc(lastX, lastY, rx, ry, rot, large, sweep, nx, ny);
                        lastX = nx; lastY = ny;
                    }
                    startSubPath = true;
                    break;

                case 'C':       // Draws a cubic Bézier curve from the current point to (x,y)
                    if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( Command {0} {1} )\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 6)
                    {
                        objCount++;
                        if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( draw curve nr. {0} )\r\n", (1 + rep / 6)); }
                        if ((rep + 5) < floatArgs.Length)
                        {
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
                            points = new Point[4];
                            points[0] = new Point(lastX, lastY);
                            points[1] = new Point(cx1, cy1);
                            points[2] = new Point(cx2, cy2);
                            points[3] = new Point(cx3, cy3);
                            var b = GetBezierApproximation(points, svgBezierAccuracy);
                            if (svgNodesOnly)
                            {
                                //gcodeDotOnly(cx1, cy1, command.ToString());
                                //gcodeDotOnly(cx2, cy2, command.ToString());
                                gcodeDotOnly(cx3, cy3, command.ToString());
                            }
                            else
                            {
                                for (int i = 1; i < b.Points.Count; i++)
                                    gcodeMoveTo((float)b.Points[i].X, (float)b.Points[i].Y, command.ToString());
                            }
                            cxMirror = cx3 - (cx2 - cx3); cyMirror = cy3 - (cy2 - cy3);
                            lastX = cx3; lastY = cy3;
                        }
                        else
                        { gcodeString[gcodeStringIndex].AppendFormat("( Missing argument after {0} )\r\n", rep); }
                    }
                    startSubPath = true;
                    break;

                case 'S':       // Draws a cubic Bézier curve from the current point to (x,y)
                    if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( Command {0} {1} )\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        objCount++;
                        if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( draw curve nr. {0} )\r\n", (1 + rep / 4)); }
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
                        points = new Point[4];
                        points[0] = new Point(lastX, lastY);
                        points[1] = new Point(cxMirror, cyMirror);
                        points[2] = new Point(cx2, cy2);
                        points[3] = new Point(cx3, cy3);
                        var b = GetBezierApproximation(points, svgBezierAccuracy);
                        if (svgNodesOnly)
                        {
                            //gcodeDotOnly(cxMirror, cyMirror, command.ToString());
                            //gcodeDotOnly(cx2, cy2, command.ToString());
                            gcodeDotOnly(cx3, cy3, command.ToString());
                        }
                        else
                        {
                            for (int i = 1; i < b.Points.Count; i++)
                                gcodeMoveTo((float)b.Points[i].X, (float)b.Points[i].Y, command.ToString());
                        }
                        cxMirror = cx3 - (cx2 - cx3); cyMirror = cy3 - (cy2 - cy3);
                        lastX = cx3; lastY = cy3;
                    }
                    startSubPath = true;
                    break;

                case 'Q':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( Command {0} {1} )\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        objCount++;
                        if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( draw curve nr. {0} )\r\n", (1 + rep / 4)); }
                        float cx2, cy2, cx3, cy3;
                        if (absolute)
                        {
                            cx2 = floatArgs[rep] + offsetX; cy2 = floatArgs[rep + 1] + offsetY;
                            cx3 = floatArgs[rep + 2] + offsetX; cy3 = floatArgs[rep + 3] + offsetY;
                        }
                        else
                        {
                            cx2 = lastX + floatArgs[rep]; cy2 = lastY + floatArgs[rep + 1];
                            cx3 = lastX + floatArgs[rep + 2]; cy3 = lastY + floatArgs[rep + 3];
                        }

                        float qpx1 = (cx2 - lastX) * 2 / 3 + lastX;     // shorten control points to 2/3 length to use 
                        float qpy1 = (cy2 - lastY) * 2 / 3 + lastY;     // qubic function
                        float qpx2 = (cx2 - cx3) * 2 / 3 + cx3;
                        float qpy2 = (cy2 - cy3) * 2 / 3 + cy3;
                        points = new Point[4];
                        points[0] = new Point(lastX, lastY);
                        points[1] = new Point(qpx1, qpy1);   
                        points[2] = new Point(qpx2, qpy2);  
                        points[3] = new Point(cx3, cy3);
                        cxMirror = cx3 - (cx2 - cx3); cyMirror = cy3 - (cy2 - cy3);
                        lastX = cx3; lastY = cy3;
                        var b = GetBezierApproximation(points, svgBezierAccuracy);
                        if (svgNodesOnly)
                        {
                            //gcodeDotOnly(qpx1, qpy1, command.ToString());
                            //gcodeDotOnly(qpx2, qpy2, command.ToString());
                            gcodeDotOnly(cx3, cy3, command.ToString());
                        }
                        else
                        {
                            for (int i = 1; i < b.Points.Count; i++)
                                gcodeMoveTo((float)b.Points[i].X, (float)b.Points[i].Y, command.ToString());
                        }
                    }
                    startSubPath = true;
                    break;

                case 'T':       // Draws a quadratic Bézier curve from the current point to (x,y)
                    if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( Command {0} {1} )\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 2)
                    {
                        objCount++;
                        if (svgComments) { gcodeString[gcodeStringIndex].AppendFormat("( draw curve nr. {0} )\r\n", (1 + rep / 2)); }
                        float cx3, cy3;
                        if (absolute)
                        {
                            cx3 = floatArgs[rep] + offsetX; cy3 = floatArgs[rep + 1] + offsetY;
                        }
                        else
                        {
                            cx3 = lastX + floatArgs[rep]; cy3 = lastY + floatArgs[rep + 1];
                        }

                        float qpx1 = (cxMirror - lastX) * 2 / 3 + lastX;     // shorten control points to 2/3 length to use 
                        float qpy1 = (cyMirror - lastY) * 2 / 3 + lastY;     // qubic function
                        float qpx2 = (cxMirror - cx3) * 2 / 3 + cx3;
                        float qpy2 = (cyMirror - cy3) * 2 / 3 + cy3;
                        points = new Point[4];
                        points[0] = new Point(lastX, lastY);
                        points[1] = new Point(qpx1, qpy1);
                        points[2] = new Point(qpx2, qpy2);
                        points[3] = new Point(cx3, cy3);
                        cxMirror = cx3; cyMirror = cy3;
                        lastX = cx3; lastY = cy3;
                        var b = GetBezierApproximation(points, svgBezierAccuracy);
                        if (svgNodesOnly)
                        {
                            //gcodeDotOnly(qpx1, qpy1, command.ToString());
                            //gcodeDotOnly(qpx2, qpy2, command.ToString());
                            gcodeDotOnly(cx3, cy3, command.ToString());
                        }
                        else
                        {
                            for (int i = 1; i < b.Points.Count; i++)
                                gcodeMoveTo((float)b.Points[i].X, (float)b.Points[i].Y, command.ToString());
                        }
                    }
                    startSubPath = true;
                    break;

                default:
                    if (svgComments) gcodeString[gcodeStringIndex].Append("( *********** unknown: " + command.ToString()+ " ***** )\r\n");
                    break;
            }
            return objCount;
        }

        /// <summary>
        /// Calculate Path-Arc-Command - Code from https://github.com/vvvv/SVG/blob/master/Source/Paths/SvgArcSegment.cs
        /// </summary>
        private static void calcArc(float StartX, float StartY, float RadiusX, float RadiusY,float Angle, float Size,  float Sweep, float EndX, float EndY)
        {
            if (RadiusX == 0.0f && RadiusY == 0.0f)
            {
  //              graphicsPath.AddLine(this.Start, this.End);
                return;
            }
            double sinPhi = Math.Sin(Angle * Math.PI / 180.0);
            double cosPhi = Math.Cos(Angle * Math.PI / 180.0);
            double x1dash = cosPhi * (StartX - EndX) / 2.0 + sinPhi * (StartY - EndY) / 2.0;
            double y1dash = -sinPhi * (StartX - EndX) / 2.0 + cosPhi * (StartY - EndY) / 2.0;
            double root;
            double numerator = RadiusX * RadiusX * RadiusY * RadiusY - RadiusX * RadiusX * y1dash * y1dash - RadiusY * RadiusY * x1dash * x1dash;
            float rx = RadiusX;
            float ry = RadiusY;
            if (numerator < 0.0)
            {
                float s = (float)Math.Sqrt(1.0 - numerator / (RadiusX * RadiusX * RadiusY * RadiusY));

                rx *= s;
                ry *= s;
                root = 0.0;
            }
            else
            {
                root = ((Size == 1 && Sweep == 1) || (Size == 0 && Sweep == 0) ? -1.0 : 1.0) * Math.Sqrt(numerator / (RadiusX * RadiusX * y1dash * y1dash + RadiusY * RadiusY * x1dash * x1dash));
            }
            double cxdash = root * rx * y1dash / ry;
            double cydash = -root * ry * x1dash / rx;
            double cx = cosPhi * cxdash - sinPhi * cydash + (StartX + EndX) / 2.0;
            double cy = sinPhi * cxdash + cosPhi * cydash + (StartY + EndY) / 2.0;
            double theta1 = CalculateVectorAngle(1.0, 0.0, (x1dash - cxdash) / rx, (y1dash - cydash) / ry);
            double dtheta = CalculateVectorAngle((x1dash - cxdash) / rx, (y1dash - cydash) / ry, (-x1dash - cxdash) / rx, (-y1dash - cydash) / ry);
            if (Sweep == 0 && dtheta > 0)
            {
                dtheta -= 2.0 * Math.PI;
            }
            else if (Sweep == 1 && dtheta < 0)
            {
                dtheta += 2.0 * Math.PI;
            }
            int segments = (int)Math.Ceiling((double)Math.Abs(dtheta / (Math.PI / 2.0)));
            double delta = dtheta / segments;
            double t = 8.0 / 3.0 * Math.Sin(delta / 4.0) * Math.Sin(delta / 4.0) / Math.Sin(delta / 2.0);

            double startX = StartX;
            double startY = StartY;

            for (int i = 0; i < segments; ++i)
            {
                double cosTheta1 = Math.Cos(theta1);
                double sinTheta1 = Math.Sin(theta1);
                double theta2 = theta1 + delta;
                double cosTheta2 = Math.Cos(theta2);
                double sinTheta2 = Math.Sin(theta2);

                double endpointX = cosPhi * rx * cosTheta2 - sinPhi * ry * sinTheta2 + cx;
                double endpointY = sinPhi * rx * cosTheta2 + cosPhi * ry * sinTheta2 + cy;

                double dx1 = t * (-cosPhi * rx * sinTheta1 - sinPhi * ry * cosTheta1);
                double dy1 = t * (-sinPhi * rx * sinTheta1 + cosPhi * ry * cosTheta1);

                double dxe = t * (cosPhi * rx * sinTheta2 + sinPhi * ry * cosTheta2);
                double dye = t * (sinPhi * rx * sinTheta2 - cosPhi * ry * cosTheta2);

                points = new Point[4];
                points[0] = new Point(startX, startY);
                points[1] = new Point((startX + dx1), (startY + dy1));
                points[2] = new Point((endpointX + dxe), (endpointY + dye));
                points[3] = new Point(endpointX, endpointY);
                var b = GetBezierApproximation(points, svgBezierAccuracy);
                for (int k = 1; k < b.Points.Count; k++)
                    gcodeMoveTo(b.Points[k], "arc");
                //gcodeMove(1, (float)b.Points[k].X, (float)b.Points[k].Y, 0, 0, "arc");

                theta1 = theta2;
                startX = (float)endpointX;
                startY = (float)endpointY;
            }
        }
        private static double CalculateVectorAngle(double ux, double uy, double vx, double vy)
        {
            double ta = Math.Atan2(uy, ux);
            double tb = Math.Atan2(vy, vx);
            if (tb >= ta)
            {    return tb - ta;  }
            return Math.PI * 2 - (ta - tb);
        }
        // helper functions
        private static float fsqrt(float x) { return (float)Math.Sqrt(x); }
        private static float fvmag(float x, float y) { return fsqrt(x * x + y * y); }
        private static float fdistance(float x1, float y1, float x2, float y2) { return fvmag(x2-x1,y2-y1); }

        /// <summary>
        /// Calculate Bezier line segments
        /// from http://stackoverflow.com/questions/13940983/how-to-draw-bezier-curve-by-several-points
        /// </summary>
        private static Point[] points;
        private static PolyLineSegment GetBezierApproximation( Point[] controlPoints, int outputSegmentCount)
        {
            Point[] points = new Point[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return new PolyLineSegment(points, true);
        }
        private static Point GetBezierPoint(double t, Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            double x= (1 - t) * P0.X + t * P1.X;
            return new Point(x, (1 - t) * P0.Y + t * P1.Y);
        }

// Prepare G-Code

        /// <summary>
        /// Transform XY coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static Point translateXY(float x, float y)
        {
            Point coord = new Point(x,y);
            return translateXY(coord);
        }
        private static Point translateXY(Point pointStart)
        {
            Point pointResult = matrixElement.Transform(pointStart);
//            pointResult.Y = svgHeightUnit - pointResult.Y;          // mirror Y axis, because GCode 0;0 is lower-left. SVG is upper-left
            if (gcodeScale != 1)
            {
                pointResult.X = pointResult.X * gcodeScale; // final scaling to reach given size svgMaxSize
                pointResult.Y = pointResult.Y * gcodeScale;
            }
            return pointResult;
        }
        /// <summary>
        /// Transform I,J coordinate using matrix and scale  
        /// </summary>
        /// <param name="pointStart">coordinate to transform</param>
        /// <returns>transformed coordinate</returns>
        private static Point translateIJ(float i, float j)
        {
            Point coord = new Point(i,j);
            return translateIJ(coord);
        }
        private static Point translateIJ(Point pointStart)
        {
            Point pointResult = pointStart;
            double tmp_i = pointStart.X, tmp_j = pointStart.Y;
            pointResult.X = tmp_i * matrixElement.M11 - tmp_j * matrixElement.M21;
            pointResult.Y = tmp_i * -matrixElement.M12 + tmp_j * matrixElement.M22; // i,j are relative - no offset needed, but perhaps rotation

            if (gcodeScale != 1)
            {
                pointResult.X = pointResult.X * gcodeScale; // final scaling to reach given size svgMaxSize
                pointResult.Y = pointResult.Y * gcodeScale;
            }
            return pointResult;
        }


        private static void gcodeDotOnly(float x, float y, string cmt)
        {
            gcodeStartPath(x, y, cmt);
            gcode.PenUp(gcodeString[gcodeStringIndex], cmt);
            penIsDown = false;
        }


        /// <summary>
        /// Insert G0 and Pen down gcode command
        /// </summary>
        private static void gcodeStartPath(float x, float y, string cmt)
        {   Point coord = translateXY(x, y);
            lastGCX = coord.X; lastGCY = coord.Y;
            lastSetGCX = coord.X; lastSetGCY = coord.Y;
            gcode.MoveToRapid(gcodeString[gcodeStringIndex], coord, cmt);
            if (svgPausePenDown) { gcode.Pause(gcodeString[gcodeStringIndex], "Pause before Pen Down"); }
            gcode.PenDown(gcodeString[gcodeStringIndex], cmt);
            penIsDown = true;
            isReduceOk = false;
        }
        /// <summary>
        /// Insert Pen-up gcode command
        /// </summary>
        private static void gcodeStopPath(string cmt)
        {
            if (gcodeReduce)
            {
                if ((lastSetGCX != lastGCX) || (lastSetGCY != lastGCY)) // restore last skipped point for accurat G2/G3 use
                    gcode.MoveTo(gcodeString[gcodeStringIndex], new System.Windows.Point(lastGCX, lastGCY), "restore Point");
            }
            gcode.PenUp(gcodeString[gcodeStringIndex], cmt);
            penIsDown = false;
        }

        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void gcodeMoveTo(float x, float y, string cmt)
        {
            Point coord = new Point(x, y);
            gcodeMoveTo(coord, cmt);
        }

        private static bool isReduceOk = false;
        private static bool rejectPoint = false;
        private static double lastGCX = 0, lastGCY = 0, lastSetGCX = 0, lastSetGCY = 0, distance;
        /// <summary>
        /// Insert G1 gcode command
        /// </summary>
        private static void gcodeMoveTo(Point orig, string cmt)
        {
            Point coord = translateXY(orig);
            rejectPoint = false;
            if (gcodeReduce && isReduceOk)
            {   distance = Math.Sqrt(((coord.X - lastSetGCX) * (coord.X - lastSetGCX)) + ((coord.Y - lastSetGCY) * (coord.Y - lastSetGCY)));
                if (distance < gcodeReduceVal)      // discard actual G1 movement
                { rejectPoint = true;
                }
                else
                { lastSetGCX = coord.X; lastSetGCY = coord.Y;
                }
            }
            if (!gcodeReduce || !rejectPoint)       // write GCode
            {
                gcode.MoveTo(gcodeString[gcodeStringIndex], coord, cmt);
            }
            lastGCX = coord.X; lastGCY = coord.Y;
        }

        /// <summary>
        /// Insert G2/G3 gcode command
        /// </summary>
        private static void gcodeArcToCCW(float x, float y, float i, float j, string cmt)
        {
            Point coordxy = translateXY(x, y);
            Point coordij = translateIJ(i, j);
            if (gcodeReduce && isReduceOk)      // restore last skipped point for accurat G2/G3 use
            {
                if ((lastSetGCX != lastGCX) || (lastSetGCY != lastGCY))
                    gcode.MoveTo(gcodeString[gcodeStringIndex], new System.Windows.Point(lastGCX, lastGCY), cmt);
            }
            gcode.Arc(gcodeString[gcodeStringIndex], 3, coordxy, coordij, cmt);
        }

        /// <summary>
        /// Insert Pen-up gcode command
        /// </summary>
        private static void gcodePenUp(string cmt)
        {   if (penIsDown)
                gcode.PenUp(gcodeString[gcodeStringIndex], cmt);
            penIsDown = false;
        }

        /// <summary>
        /// Insert tool change gcode command
        /// </summary>
        private static void gcodeTool(int toolnr)
        {
            if (toolnr >= 0)
            {
                svgPalette.setUse(true);
                if (!svgToolSort)               // if sort, insert tool command later
                {
                    if (gcodeToolNr != toolnr)  // if tool already in use, don't select again
                    {
                        gcode.Tool(gcodeString[gcodeStringIndex], toolnr, svgPalette.getName());
                        gcodeToolNr = toolnr;
                    }
                }
            }
        }
    }
}
