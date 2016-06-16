/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2016 Sven Hasemann contact: svenhb@web.de

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
/*  SVGToGCode.cs a static class to convert SVG data into G-Code 
    Not implemented: 
        Basic-shapes: ellipse, polyline, polygon; Text, Image
        Transform: rotation with offset, skewX, skewY
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
    class SVGToGCode
    {
        private static StringBuilder gcodeString = new StringBuilder();

        // following settings will be read from Properties.Settings.Default in startConvert()
        private static int svgBezierAccuracy = 6;       // applied line segments at bezier curves
        private static bool svgScaleApply = true;       // try to scale final GCode if true
        private static float svgMaxSize = 100;          // final GCode size (greater dimension) if scale is applied
        private static bool svgClosePathExtend = true;  // if true move to first and second point of path to get overlap
        private static bool svgPauseElement = true;     // if true insert GCode pause M0 after each element
        private static bool svgPausePenDown = true;     // if true insert pause M0 before pen down
        private static bool svgComments = true;         // if true insert additional comments into GCode

        private static bool simpleBezier = false;       // if true replace curve by simple line if distance < simpleLimit
        private static float simpleLimit = 5;           // limit when curve will be replaced by line

        private static bool gcodeReduce = false;        // if true remove G1 commands if distance is < limit
        private static float gcodeReduceVal = 1;        // limit when to remove G1 commands

        private static float gcodeXYFeed = 2000;        // XY feed to apply for G1
        private static bool gcodeComments = true;       // if true insert additional comments into GCode

        // Using Z-Axis for Pen up down
        private static bool gcodeZApply = true;         // if true insert Z movements for Pen up/down
        private static float gcodeZUp = 2;              // Z-up position
        private static float gcodeZDown = -2;           // Z-down position
        private static float gcodeZFeed = 500;          // Z feed to apply for G1

        // Using Spindle pwr. to switch on/off laser
        private static bool gcodeSpindleToggle = false; // Switch on/off spindle for Pen down/up (M3/M5)
        private static float gcodeSpindleSpeed = 20000; // Spindle speed to apply

        // Using Spindle-Speed als PWM output to control RC-Servo
        private static bool gcodePWMEnable = false;     // Change Spindle speed for Pen down/up
        private static float gcodePWMUp = 200;          // Spindle speed for Pen-up
        private static float gcodePWMDlyUp = 0;         // Delay to apply after Pen-up (because servo is slow)
        private static float gcodePWMDown = 800;        // Spindle speed for Pen-down
        private static float gcodePWMDlyDown = 0;       // Delay to apply after Pen-down (because servo is slow)

        private static float gcodeScale = 1;            // finally scale with this factor if svgScaleApply and svgMaxSize
        private static int gcodeLines = 0;              // counter for GCode lines
        private static float gcodeDistance = 0;         // counter for GCode move distance
        private static float gcodeTime = 0;             // counter for GCode work time
        private static int gcodePauseCounter = 0;       // counter for GCode pause M0 commands
        private static string svgInfo = "";
        private static Matrix[] matrixGroup = new Matrix[10];   // store SVG-Group transformation matrixes
        private static Matrix matrixElement = new Matrix();     // store finally applied matrix

        // Entrypoint for conversion: apply file-path or file-URL
        // return string with GCODE
        // setting will be read from Properties.Settings.Default
        public static string ConvertFile(string file)
        {
            if (file == "")
            {   MessageBox.Show("Empty file name"); return ""; }

            gcodeString.Clear();
            gcodeLines = 0;
            gcodeDistance = 0;
            gcodeTime = 0;
            gcodePauseCounter = 0;
            svgInfo = "; SVG information:\r\n";

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
                    {
                        svgCode = XElement.Load(file, LoadOptions.PreserveWhitespace);
                        startConvert(svgCode);
                    }
                    catch (Exception e)
                    { MessageBox.Show("Error '"+e.ToString()+"' in XML file " + file); return ""; }
                }
                else { MessageBox.Show("File does not exist: " + file); return ""; }
            }
            if (!gcodeSpindleToggle) gcodeString.AppendFormat("M5 (Stop spindle)\r\n");
            gcodeString.Replace(',', '.');
            gcodeTime += gcodeDistance / gcodeXYFeed;

            string header = svgInfo;
            header += string.Format("; G-Code information:\r\n; G-Code lines: {0}\r\n", gcodeLines);
            header += string.Format("; Path length: {0:0.0} units\r\n; Duration: {1:0.0} min.\r\n", gcodeDistance, gcodeTime);
            if (svgPauseElement||svgPausePenDown) header += string.Format("; Prog. Pause (M0) count: {0}\r\n", gcodePauseCounter);

            string[] commands = Properties.Settings.Default.importGCHeader.Split(';');
            foreach (string cmd in commands)
                header += string.Format("{0} (Setup - GCode-Header)\r\n", cmd.Trim())  ;

            string footer = "";
            commands = Properties.Settings.Default.importGCFooter.Split(';');
            foreach (string cmd in commands)
                footer += string.Format("{0} (Setup - GCode-Footer)\r\n", cmd.Trim());

            return header+gcodeString.ToString()+footer;
        }
 
        // Set defaults and parse main element of SVG-XML
        private static void startConvert(XElement svgCode)
        {
            svgBezierAccuracy = Properties.Settings.Default.importSVGBezier;
            svgScaleApply = Properties.Settings.Default.importSVGRezise;
            svgMaxSize = (float)Properties.Settings.Default.importSVGMaxSize;
            svgClosePathExtend = Properties.Settings.Default.importSVGPathExtend;
            svgPauseElement = Properties.Settings.Default.importSVGPauseElement;
            svgPausePenDown = Properties.Settings.Default.importSVGPausePenDown;
            svgComments = Properties.Settings.Default.importSVGAddComments;

            gcodeReduce = Properties.Settings.Default.importSVGReduce;
            gcodeReduceVal = (float)Properties.Settings.Default.importSVGReduceLimit;

            gcodeXYFeed = (float)Properties.Settings.Default.importGCXYFeed;
            gcodeComments = Properties.Settings.Default.importGCAddComments;
            gcodeZApply = Properties.Settings.Default.importGCZEnable;
            gcodeZUp = (float)Properties.Settings.Default.importGCZUp;
            gcodeZDown = (float)Properties.Settings.Default.importGCZDown;
            gcodeZFeed = (float)Properties.Settings.Default.importGCZFeed;
            gcodeSpindleToggle = Properties.Settings.Default.importGCSpindleToggle;
            gcodeSpindleSpeed = (float)Properties.Settings.Default.importGCSSpeed;
            gcodePWMEnable = Properties.Settings.Default.importGCPWMEnable;
            gcodePWMUp = (float)Properties.Settings.Default.importGCPWMUp;
            gcodePWMDlyUp = (float)Properties.Settings.Default.importGCPWMDlyUp;
            gcodePWMDown = (float)Properties.Settings.Default.importGCPWMDown;
            gcodePWMDlyDown = (float)Properties.Settings.Default.importGCPWMDlyDown;
            gcodeScale = 1;
            currentX = 0; currentY = 0;
            offsetX = 0; offsetY = 0;

            matrixElement.SetIdentity();
            for (int i=0; i<matrixGroup.Length;i++)
                matrixGroup[i].SetIdentity(); 

            parseGlobals(svgCode);
            if (!gcodeSpindleToggle) gcodeString.AppendFormat("M3 S{0} (Start spindle)\r\n", gcodeSpindleSpeed);
//            gcodePenUp();
            parseBasicElements(svgCode,0);
            parsePath(svgCode,0);
            parseGroup(svgCode,0);
            gcodePenUp();
            return;
        }

        // Parse SVG dimension (viewbox, width, height)
        private static XNamespace nspace = "http://www.w3.org/2000/svg";
        private static void parseGlobals(XElement svgCode)
        {   svgWidth = 0;
            svgHeight = 0;
            if (svgCode.Attribute("viewBox") != null)
            {
                string viewbox = svgCode.Attribute("viewBox").Value.Replace(' ', '|');
                var split = viewbox.Split('|');
                svgWidth = floatParse(split[2]);    
                svgHeight = floatParse(split[3].TrimEnd(')'));    
                if (svgComments) gcodeString.AppendLine("; SVG viewbox :" + viewbox);
            }
            else
            {
                Regex r = new Regex(@"[-+]?\d+(.\d+)?");       //[0-9]+\\.[0-9]+");

                if (svgCode.Attribute("height") != null)
                {
                    string height = svgCode.Attribute("height").Value;
                    Match m = r.Match(height);
                    svgHeight = floatParse(m.ToString());
                    if (svgComments) gcodeString.AppendLine("; SVG height :" + height);
                }
                if (svgCode.Attribute("width") != null)
                {
                    string width = svgCode.Attribute("width").Value;
                    Match m = r.Match(width);
                    svgWidth = floatParse(m.ToString());
                    if (svgComments) gcodeString.AppendLine("; SVG width :" + width);
                }
            }
            if ((svgWidth > 0) && (svgHeight > 0))
            {
                if (svgScaleApply)
                {
                    gcodeScale = svgMaxSize / Math.Max(svgWidth, svgHeight);
                    if (svgComments) 
                        gcodeString.AppendFormat("; Scale to X={0} Y={1} f={2}\r\n", svgWidth * gcodeScale, svgHeight * gcodeScale, gcodeScale);
                }
            }
            else
                if (svgComments) gcodeString.Append("; SVG Dimension not given");
            return;
        }

        // Parse Group-Element and included elements
        private static void parseGroup(XElement svgCode, int level)
        {   foreach (XElement groupElement in svgCode.Elements(nspace + "g"))
            {
                if (svgComments)
                    if (groupElement.Attribute("id") != null)
                        gcodeString.Append(";\r\n; Group level:"+level.ToString()+" id=" + groupElement.Attribute("id").Value + " \r\n");
                parseTransform(groupElement,true, level);
                parseBasicElements(groupElement, level);
                parsePath(groupElement, level);
                parseGroup(groupElement, level+1);
            }
            return;
        }

        // Parse Transform information check: http://www.w3.org/TR/SVG/coords.html
        private static void parseTransform(XElement element,bool isGroup, int level)
        {   Matrix tmp = new Matrix(1, 0, 0, 1, 0, 0);
            bool transf = false;
            if (element.Attribute("transform") != null)
            {
                transf = true;
                string transform = element.Attribute("transform").Value;
                if ((transform != null) && (transform.IndexOf("translate") >= 0))
                {
                    var coord = getTextBetween(transform, "translate(", ")");
                    var split = coord.Split(',');
                    tmp.OffsetX = floatParse(split[0]);    
                    if (split.Length>1)
                        tmp.OffsetY = floatParse(split[1].TrimEnd(')'));    
                    if (svgComments) gcodeString.Append(string.Format("; SVG-Translate {0} {1}\r\n", tmp.OffsetX, tmp.OffsetY));
                }
                if ((transform != null) && (transform.IndexOf("scale") >= 0))
                {
                    var coord = getTextBetween(transform, "scale(", ")");
                    var split = coord.Split(',');
                    tmp.M11 = floatParse(split[0]);
                    if (split.Length > 1)
                        tmp.M22 = floatParse(split[1].TrimEnd(')'));
                    if (svgComments) gcodeString.Append(string.Format("; SVG-Scale {0} {1}\r\n", tmp.M11, tmp.M22));
                }
                if ((transform != null) && (transform.IndexOf("rotate") >= 0))
                {
                    var coord = getTextBetween(transform, "rotate(", ")");
                    var split = coord.Split(',');
                    float angle = floatParse(split[0])*(float)Math.PI/180;
                    tmp.M11 = Math.Cos(angle); tmp.M12 = Math.Sin(angle);
                    tmp.M21 = -Math.Sin(angle); tmp.M22 = Math.Cos(angle);

                    if (svgComments) gcodeString.Append(string.Format("; SVG-Rotate {0} \r\n", angle));
                }
                if ((transform != null) && (transform.IndexOf("matrix") >= 0))
                {
                    var coord = getTextBetween(transform, "matrix(", ")");
                    var split = coord.Split(',');
                    tmp.M11 = floatParse(split[0]);     // a    scale x
                    tmp.M12 = floatParse(split[1]);     // b
                    tmp.M21 = floatParse(split[2]);     // c
                    tmp.M22 = floatParse(split[3]);     // d    scale y
                    tmp.OffsetX = floatParse(split[4]); // e    offset x
                    tmp.OffsetY = floatParse(split[5]); // f    offset y
                    if (svgComments) gcodeString.Append(string.Format(";\r\n; SVG-Matrix {0} {1} {2}\r\n", coord.Replace(',','|'),level,isGroup));
                }
            }
            if (isGroup)
            {   matrixGroup[level].SetIdentity();
                if (level > 0)
                    matrixGroup[level] = Matrix.Multiply(tmp,matrixGroup[level - 1]);
                else
                    matrixGroup[level] = tmp;
                matrixElement = matrixGroup[level];
            }
            else
            {   matrixElement = Matrix.Multiply(tmp,matrixGroup[level]); }
            if (svgComments && transf)
            {
                for (int i = 0; i <= level; i++)
                    gcodeString.Append(string.Format("; gc-Matrix level({0}) {1}\r\n", i, matrixGroup[i].ToString()));

                if (svgComments) gcodeString.Append(string.Format("; gc-Scale {0} {1}\r\n", matrixElement.M11, matrixElement.M22));
                if (svgComments) gcodeString.Append(string.Format("; gc-Offset {0} {1}\r\n", matrixElement.OffsetX, matrixElement.OffsetY));
            }
            return;
        }
        private static string getTextBetween(string source,string s1, string s2)
        {
            int start = source.IndexOf(s1) + s1.Length;
            int end = source.IndexOf(s2);
            return source.Substring(start, end-start).Replace(' ', ',');
        }
        private static float floatParse(string str)
        {   return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat); }

        // Convert Basic shapes (up to now: line, rect, circle) check: http://www.w3.org/TR/SVG/shapes.html
        private static void parseBasicElements(XElement svgCode, int level)
        {   string[] forms = { "rect", "line", "circle", "text", "image" };
            foreach (var form in forms)
            {
                foreach (var pathElement in svgCode.Elements(nspace + form))
                {
                    if (pathElement != null)
                    {
//                        if (svgComments)
                            if (pathElement.Attribute("id") != null)
                                gcodeString.Append(";\r\n; Basic shape level:" + level.ToString() + " id=" + pathElement.Attribute("id").Value + " \r\n");
                        if (startFirstElement)
                        { gcodePenUp(); startFirstElement = false; }

                        offsetX = 0; offsetY = 0;
                        parseTransform(pathElement, false, level);
                        float x=0, y=0, x1=0, y1=0, x2=0, y2=0, width=0, height=0, rx=0, ry=0,cx=0,cy=0,r=0;
                        if (pathElement.Attribute("x") !=null)      x = floatParse(pathElement.Attribute("x").Value);
                        if (pathElement.Attribute("y") != null)     y = floatParse(pathElement.Attribute("y").Value);
                        if (pathElement.Attribute("x1") != null)    x1 = floatParse(pathElement.Attribute("x1").Value);
                        if (pathElement.Attribute("y1") != null)    y1 = floatParse(pathElement.Attribute("y1").Value);
                        if (pathElement.Attribute("x2") != null)    x2 = floatParse(pathElement.Attribute("x2").Value);
                        if (pathElement.Attribute("y2") != null)    y2 = floatParse(pathElement.Attribute("y2").Value);
                        if (pathElement.Attribute("width") != null) width=floatParse(pathElement.Attribute("width").Value);
                        if (pathElement.Attribute("height") != null) height=floatParse(pathElement.Attribute("height").Value);
                        if (pathElement.Attribute("rx") != null)    rx=floatParse(pathElement.Attribute("rx").Value);
                        if (pathElement.Attribute("ry") != null)    ry=floatParse(pathElement.Attribute("ry").Value);
                        if (pathElement.Attribute("cx") != null)    cx=floatParse(pathElement.Attribute("cx").Value);
                        if (pathElement.Attribute("cy") != null)    cy=floatParse(pathElement.Attribute("cy").Value);
                        if (pathElement.Attribute("r") != null)     r=floatParse(pathElement.Attribute("r").Value);

                        if (svgPauseElement||svgPausePenDown) { gcodeString.Append("M0 (Pause before path)\r\n"); gcodePauseCounter++; }
                        if (form == "rect")
                        {
                            if (svgComments) gcodeString.AppendFormat("; SVG-Rect x:{0} y:{1} width:{2} height:{3}\r\n", x, y, width, height);
                            x += offsetX; y += offsetY;
                            gcodeMove(0, x + rx, y + height, 0, 0, form);
                            gcodePenDown();
                            gcodeMove(1, x + width - rx, y + height, 0, 0, form);
                            if (rx > 0) gcodeMove(3, x + width, y + height - ry, 0, ry, form);
                            gcodeMove(1, x + width, y + ry, 0, 0, form);
                            if (rx > 0) gcodeMove(3, x + width - rx, y, -rx, 0, form);
                            gcodeMove(1, x + rx, y, 0, 0, form);
                            if (rx > 0) gcodeMove(3, x, y + ry, 0, -ry, form);
                            gcodeMove(1, x, y + height - ry, 0, 0, form);
                            if (rx > 0) gcodeMove(3, x + rx, y + height, rx, 0, form);
                            gcodePenUp();
                        }
                        else if (form == "line")
                        {
                            if (svgComments) gcodeString.AppendFormat("; SVG-Line x1:{0} y1:{1} x2:{2} y2:{3}\r\n", x1, y1, x2, y2);
                            x1 += offsetX; y1 += offsetY;
                            gcodeMove(0, x1, y1, 0, 0, form);
                            gcodePenDown();
                            gcodeMove(1, x2, y2, 0, 0, form);
                            gcodePenUp();
                        }
                        else if (form == "circle")
                        {
                            if (svgComments) gcodeString.AppendFormat("; circle cx:{0} cy:{1} r:{2} \r\n", cx, cy, r);
                            cx += offsetX; cy += offsetY;
                            gcodeMove(0, cx - r, cy, 0, 0, form);
                            gcodePenDown();
                            gcodeMove(2, cx - r, cy, r, 0, form);
                            gcodePenUp();
                        }
                        else if ((form == "text") || (form == "image"))
                        {
                            gcodeString.AppendLine("; +++++++++++++++++++++++++++++++++");
                            gcodeString.AppendLine("; ++++++ " + form + " is not supported ++++");
                            gcodeString.AppendLine("; +++++++++++++++++++++++++++++++++");
                        }
                        else
                        { if (svgComments) gcodeString.Append("; ++++++ Unknown Shape: " + form); }
                    }
                }
            }
            return;
        }

        // Convert all Path commands, check: http://www.w3.org/TR/SVG/paths.html
        // Split command tokens
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

                    gcodeString.Append(";\r\n; Start path \r\n");
                    if (svgComments)
                    {
                        if (pathElement.Attribute("id") != null)
                            gcodeString.Append(";\r\n; Path level:" + level.ToString() + " id=" + pathElement.Attribute("id").Value + " \r\n");
                        else
                            gcodeString.Append(";\r\n; SVG path=" + id + " \r\n");                        
                    }

                    if (pathElement.Attribute("id") != null)
                        id = pathElement.Attribute("id").Value;
                    parseTransform(pathElement,false,level);
                    if (d.Length > 0)
                    {
        // split complete path in to command-tokens
                        if (svgPauseElement||svgPausePenDown) { gcodeString.Append("M0 (Pause before path)\r\n"); gcodePauseCounter++; }
                        string separators = @"(?=[A-Za-z-[e]])";            
                        var tokens = Regex.Split(d, separators).Where(t => !string.IsNullOrEmpty(t));
                        int objCount = 0;
                        foreach (string token in tokens)
                            objCount += parsePathCommand(token);
                        svgInfo += string.Format("; {0}: {1} Elements\r\n", id, objCount);
                    }
                    gcodeString.Append(";\r\n; End path \r\n");
                }
            }
            return;
        }

        private static bool startSubPath = true;
        private static bool startPath = true;
        private static bool startFirstElement = true;
        private static float svgWidth, svgHeight;
        private static float offsetX, offsetY;
        private static float currentX, currentY;
        private static float? firstX, firstY;
        private static float lastX, lastY;
        private static float cxMirror=0, cyMirror=0;
        private static string secondMove = "";
        private static int pathCount = 0;
        // Convert all Path commands, check: http://www.w3.org/TR/SVG/paths.html
        // Convert command tokens
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
                case 'M':
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        objCount++;
                        if (absolute || startPath)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = floatArgs[i] + lastX;   currentY = floatArgs[i + 1] + lastY; }
                        if (startSubPath)
                        {   if (svgComments) { gcodeString.AppendFormat("; Start new subpath at {0} {1} \r\n", floatArgs[i], floatArgs[i+1]); }
                            pathCount = 0;
                            gcodePenUp();
                            gcodeMove(0,currentX, currentY, 0, 0, command.ToString());  // G0
                            gcodePenDown();
                            firstX = currentX; firstY = currentY;
                            startPath = false;
                            startSubPath = false;
                        }
                        else
                            gcodeMove(1,currentX, currentY, 0, 0, command.ToString());  // G1
                        if (firstX == null) { firstX = currentX; }
                        if (firstY == null) { firstY = currentY; }
                        lastX = currentX; lastY = currentY;
                    }
                    cxMirror = currentX; cyMirror = currentY;
                    break;

                case 'Z':
                    gcodeMove(1,(float)firstX, (float)firstY, 0, 0, command.ToString());    // G1
                    lastX = (float)firstX; lastY = (float)firstY;
                    firstX = null; firstY = null;
                    startSubPath = true;
                    if (svgClosePathExtend)
                    {   gcodeString.AppendLine(secondMove); }
                    gcodePenUp();
                    break;

                case 'L':
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = floatArgs[i + 1] + offsetY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY + floatArgs[i + 1]; }
                        gcodeMove(1,currentX, currentY, 0, 0, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;

                case 'H':
                    for (int i = 0; i < floatArgs.Length; i ++)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = floatArgs[i] + offsetX; currentY = lastY; }
                        else
                        { currentX = lastX + floatArgs[i]; currentY = lastY; }
                        gcodeMove(1,currentX, currentY, 0, 0, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;

                case 'V':
                    for (int i = 0; i < floatArgs.Length; i++)
                    {
                        objCount++;
                        if (absolute)
                        { currentX = lastX; currentY = floatArgs[i] + offsetY; }
                        else
                        { currentX = lastX ; currentY = lastY + floatArgs[i]; }
                        gcodeMove(1,currentX, currentY, 0, 0, command.ToString());
                        lastX = currentX; lastY = currentY;
                        cxMirror = currentX; cyMirror = currentY;
                    }
                    startSubPath = true;
                    break;

                case 'A':
                    if (svgComments) { gcodeString.AppendFormat("; Command {0} {1}\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 7)
                    {
                        objCount++;
                        if (svgComments) { gcodeString.AppendFormat("; draw arc nr. {0}\r\n", (1 + rep / 6)); }
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
                        calcArc(lastX, lastY, rx, ry, rot, large, sweep, nx, ny);
                        lastX = nx; lastY = ny;
                    }
                    startSubPath = true;
                    break;

                case 'C':
                    if (svgComments) { gcodeString.AppendFormat("; Command {0} {1}\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 6)
                    {
                        objCount++;
                        if (svgComments) { gcodeString.AppendFormat("; draw curve nr. {0}\r\n", (1 + rep / 6)); }
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
                            if ((simpleBezier) && (fdistance(lastX, lastY, cx3, cy3) < simpleLimit))
                            {
                                gcodeMove(1,cx3, cy3, 0, 0, command.ToString());
                            }
                            else
                            {
                                points = new Point[4];
                                points[0] = new Point(lastX, lastY);
                                points[1] = new Point(cx1, cy1);
                                points[2] = new Point(cx2, cy2);
                                points[3] = new Point(cx3, cy3);
                                var b = GetBezierApproximation(points, svgBezierAccuracy);
                                for (int i = 1; i < b.Points.Count; i++)
                                    gcodeMove(1,(float)b.Points[i].X, (float)b.Points[i].Y, 0, 0, command.ToString());
                            }
                            cxMirror = cx3 - (cx2 - cx3); cyMirror = cy3 - (cy2 - cy3);
                            lastX = cx3; lastY = cy3;
                        }
                        else
                        { gcodeString.AppendFormat("; Missing argument after {0}\r\n", rep); }
                    }
                    startSubPath = true;
                    break;

                case 'S':
                    if (svgComments) { gcodeString.AppendFormat("; Command {0} {1}\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        objCount++;
                        if (svgComments) { gcodeString.AppendFormat("; draw curve nr. {0}\r\n", (1 + rep / 4)); }
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
                        if ((simpleBezier) && (fdistance(lastX, lastY, cx3, cy3) < simpleLimit))
                        {
                            gcodeMove(1,cx3, cy3, 0, 0, command.ToString());
                        }
                        else
                        {
                            points = new Point[4];
                            points[0] = new Point(lastX, lastY);
                            points[1] = new Point(cxMirror, cyMirror);
                            points[2] = new Point(cx2, cy2);
                            points[3] = new Point(cx3, cy3);
                            var b = GetBezierApproximation(points, svgBezierAccuracy);
                            for (int i = 1; i < b.Points.Count; i++)
                                gcodeMove(1,(float)b.Points[i].X, (float)b.Points[i].Y, 0, 0, command.ToString());
                        }
                        cxMirror = cx3 - (cx2 - cx3); cyMirror = cy3 - (cy2 - cy3);
                        lastX = cx3; lastY = cy3;
                    }
                    startSubPath = true;
                    break;

                case 'Q':
                    if (svgComments) { gcodeString.AppendFormat("; Command {0} {1}\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 4)
                    {
                        objCount++;
                        if (svgComments) { gcodeString.AppendFormat("; draw curve nr. {0}\r\n", (1 + rep / 4)); }
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
                        for (int i = 1; i < b.Points.Count; i++)
                            gcodeMove(1,(float)b.Points[i].X, (float)b.Points[i].Y, 0, 0, command.ToString());
                    }
                    startSubPath = true;
                    break;

                case 'T':
                    if (svgComments) { gcodeString.AppendFormat("; Command {0} {1}\r\n", command.ToString(), ((absolute == true) ? "absolute" : "relative")); }
                    for (int rep = 0; rep < floatArgs.Length; rep += 2)
                    {
                        objCount++;
                        if (svgComments) { gcodeString.AppendFormat("; draw curve nr. {0}\r\n", (1 + rep / 2)); }
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
                        for (int i = 1; i < b.Points.Count; i++)
                            gcodeMove(1,(float)b.Points[i].X, (float)b.Points[i].Y, 0, 0, command.ToString());
                    }
                    startSubPath = true;
                    break;

                default:
                    if (svgComments) gcodeString.Append("; *********** unknown: " + command.ToString()+ " *****\r\n");
                    break;
            }
            return objCount;
        }
        // Calculate Path-Arc-Command - Code from https://github.com/vvvv/SVG/blob/master/Source/Paths/SvgArcSegment.cs
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
                    gcodeMove(1, (float)b.Points[k].X, (float)b.Points[k].Y, 0, 0, "arc");

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

        // Calculate Bezier line segments
        // from http://stackoverflow.com/questions/13940983/how-to-draw-bezier-curve-by-several-points
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

// Write G-Code
        // to remove tiny movements < limit:
        private static float lastGCX = 0;       // last-calculated GCode X-value
        private static float lastGCY = 0;       // last-calculated GCode Y-value
        private static float lastSetGCX = 0;    // last-written GCode X-value
        private static float lastSetGCY = 0;    // last-written GCode Y-value
        private static int lastGCgnr = 0;       // last-written GCode-Nr
        private static bool lastGCdiscard = false; // last-calculated GCode was discarded
        private static bool applyXYFeedRate = true; // apply XY feed after each Pen-move
        // GCode for XY movement
        private static void gcodeMove(int gnr, float x, float y, float i, float j, string cmd)
        {   // Transform Coordinates
            Point pointStart = new Point(x, y);
            Point pointResult;
            pointResult = matrixElement.Transform(pointStart);
            x = (float)pointResult.X; y = (float)pointResult.Y;

            i = i*(float)matrixElement.M11; j = j*(float)matrixElement.M22; // i,j are relative - no offset needed, but perhaps rotation

            y = (svgHeight - y);    // mirror Y axis, because GCode 0;0 is lower-left. SVG is upper-left
            if (gcodeScale != 1)
            {
                x = x * gcodeScale; // final scaling to reach given size svgMaxSize
                y = y * gcodeScale;
                i = i * gcodeScale;
                j = j * gcodeScale;
            }

            string feed = "";
            bool discard = false;
            // gcodeReduce: check if movement is > limit to write code
            if (gcodeReduce && applyXYFeedRate) { lastSetGCX = lastGCX; lastSetGCY = lastGCY; }
            if (gcodeReduce && !applyXYFeedRate)
            {   if (gnr==1)
                {   float dist = fdistance(lastSetGCX, lastSetGCY, x, y);
                    if (dist < gcodeReduceVal)                  // discard actual G1 movement
                    { discard = true; }
                    else
                    { lastSetGCX = lastGCX; lastSetGCY = lastGCY; }
                }
                if ((gnr > 1) && lastGCdiscard)                 // restore last discarded G1 movement if now G2 or G3
                {   gcodeString.AppendFormat("G{0} X{1:0.0000} Y{2:0.0000} {3} ({4})\r\n", lastGCgnr, lastGCX, lastGCY, feed, cmd);
                    gcodeLines++;
                }
            }
            lastGCdiscard = discard;
            if ((applyXYFeedRate))  // && (gnr>0))
            {
                feed = string.Format("F{0}", gcodeXYFeed);
                applyXYFeedRate = false;
            }
            if (!gcodeReduce || !discard)   // write GCode
            {
                pathCount++;
                if (gnr <= 1)   // straight movement
                {   if (feed.Length > 1) { gcodeString.Append(feed + "\r\n"); }
                    gcodeString.AppendFormat("G{0} X{1:0.0000} Y{2:0.0000} ({3})\r\n", gnr, x, y, cmd);
                    if (pathCount==2)
                        secondMove= string.Format("G{0} X{1:0.0000} Y{2:0.0000} ({3})", gnr, x, y, ("2nd "+cmd));
                }
                else// circle
                {
                    if (feed.Length > 1) { gcodeString.Append(feed + "\r\n"); }
                    gcodeString.AppendFormat("G{0} X{1:0.0000} Y{2:0.0000} I{3:0.0000} J{4:0.0000} ({5})\r\n", gnr, x, y, i, j, cmd);
                }
                gcodeDistance+= fdistance(lastGCX, lastGCY, x, y);
                gcodeLines++;
            }
            lastGCX = x; lastGCY = y; lastGCgnr = gnr;
        }    
        // GCode for Pen-down
        private static void gcodePenDown()
        {
            if (svgPausePenDown) { gcodeString.Append("M0 (Pause before Pen Down)\r\n"); gcodePauseCounter++; }
            string comment = "";
            if (gcodeComments) comment = "(Pen Down)";
            if (gcodeSpindleToggle) gcodeString.AppendFormat("M3 S{0} {1}\r\n", gcodeSpindleSpeed, comment);
            if (gcodeZApply) gcodeString.AppendFormat("G1 Z{0:0.0000} F{1} {2}\r\n", gcodeZDown,gcodeZFeed, comment);
            if (gcodePWMEnable) gcodeString.AppendFormat("M3 S{0} {1}\r\nG4 P{2:0.000}\r\n", gcodePWMDown, comment, gcodePWMDlyDown);
            if (gcodeZApply) applyXYFeedRate = true;
            gcodeTime += Math.Abs((gcodeZUp- gcodeZDown) / gcodeZFeed);
            gcodeLines++;
        }
        // GCode for Pen-up
        private static void gcodePenUp()
        {
            string comment = "";
            if (gcodeComments) comment = "(Pen Up)";
            if (gcodePWMEnable) gcodeString.AppendFormat("M3 S{0} {1}\r\nG4 P{2:0.000}\r\n", gcodePWMUp, comment, gcodePWMDlyUp);
            if (gcodeZApply) gcodeString.AppendFormat("G1 Z{0:0.0000} F{1} {2}\r\n", gcodeZUp, gcodeZFeed, comment);
            if (gcodeSpindleToggle) gcodeString.AppendFormat("M5 {0}\r\n", comment);
            applyXYFeedRate = true;
            gcodeTime += Math.Abs((gcodeZUp - gcodeZDown) / gcodeZFeed);
            gcodeLines++;
        }
    }
}
