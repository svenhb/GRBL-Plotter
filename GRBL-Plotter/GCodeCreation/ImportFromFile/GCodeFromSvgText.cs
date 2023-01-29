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

/*  GCodeFromSVG.cs a static class to convert SVG data into G-Code 
    Not implemented: 
        Basic-shapes: Image
        Transform: rotation with offset, skewX, skewY

    GCode will be written to gcodeString[gcodeStringIndex] where gcodeStringIndex corresponds with color of element to draw
*/
/* 
 * 2022-08-06 implement text import		// https://www.w3.org/TR/SVG2/text.html    https://www.w3.org/TR/SVG11/text.html
 * 2022-09-29 line 676, 765 add (fill != "none"))
 * 2023-01-12 line 368 set default for fill and stroke; line 282
 * 2023-01-13 add textLetterSpacing
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
//using System.Windows.Media;
using System.Xml.Linq;

namespace GrblPlotter
{
    public static partial class GCodeFromSvg
    {
        private static TextProperties globalTextProp = new TextProperties();
        private static readonly GraphicsPath textOnPath = new GraphicsPath();
        private static bool getTextPath = false;

        private static void ParseText(XElement pathElement, int level)
        {
            ParseTransform(pathElement, false, level);
            globalTextProp.Update(pathElement);
            int overAllCharCount = 0;
            ParseTextElement(pathElement, globalTextProp, ref overAllCharCount);
        }

        private static PointF ParseTextElement(XElement pathElement, TextProperties newTextProp, ref int newTextPathCharIndex)
        {
            PointF origin = new PointF(newTextProp.GetX(), newTextProp.GetY());
            TextProperties localTextProp = new TextProperties(newTextProp);	    // reset properties after each node, preset with prev node
            TextProperties origTextProp = new TextProperties(newTextProp);	    // reset properties after each node, preset with prev node
            int textPathCount = 0;
            int textPathCharIndex = newTextPathCharIndex;
            string txt;
            int nodeMax = pathElement.Nodes().Count();
            int nodeCount = 0;
            foreach (var node in pathElement.Nodes())
            {
                nodeCount++;
                localTextProp.CharIndex = textPathCharIndex;
                origTextProp.CharIndex = textPathCharIndex;
                if (node.NodeType == System.Xml.XmlNodeType.Text)               // Add text to myPath
                {
                    txt = StripWhiteSpace(node.ToString(), textPathCount, nodeCount, nodeMax);
                    Logger.Trace("►► ParseTextElement  id:{0}  ci:{1}   stroke:'{2}'  fill:'{3}'  text:'{4}'  style:'{5}'", pathElement.Attribute("id"), newTextProp.CharIndex, newTextProp.stroke, newTextProp.fill, txt, newTextProp.fontStyle);
                    //Logger.Info("►► ParseTextElement  id:{0}  ci:{1}   ref-ci:{2}", pathElement.Attribute("id"), newTextProp.CharIndex, newTextPathCharIndex);
                    Graphic.SetGeometry("text");
                    origin = AddText(txt, newTextProp, origin, ref textPathCharIndex);
                    newTextProp.SetX(origin.X);                             // set start posisiton
                    newTextProp.SetY(origin.Y);
                    txt = "";
                    textPathCount++;
                }
                else if (node.NodeType == System.Xml.XmlNodeType.Element)
                {
                    var nElement = (XElement)node;
                    if (nElement.Name == (nspace + "tspan"))
                    {
                        localTextProp.SetX(origin.X);                       // set start position
                        localTextProp.SetY(origin.Y);
                        if (localTextProp.Update(nElement))                 // get actual properties, perhaps overwrite x,y
                            localTextProp.CharIndex = 0;
                        origin = ParseTextElement(nElement, localTextProp, ref textPathCharIndex);
                        localTextProp = new TextProperties(origTextProp);   // restore
                        newTextProp.CharIndex += textPathCharIndex;
                    }
                    else if (nElement.Name == (nspace + "textPath"))
                    {
                        localTextProp.SetX(origin.X);                       // set start position
                        localTextProp.SetY(origin.Y);
                        if (localTextProp.Update(nElement))                 // get actual attribute (also href), perhaps overwrite x,y
                            localTextProp.CharIndex = 0;
                        origin = ProcessTextPath(nElement, localTextProp, origin, 0);
                        localTextProp = new TextProperties(origTextProp);   // restore
                        newTextProp.CharIndex += textPathCharIndex;
                    }
                    else
                    {
                        Graphic.SetHeaderMessage(string.Format(" {0}-1103: Not supported SVG element: '{1}'", CodeMessage.Warning, nElement.Name));
                    }
                }
            }
            return origin;
        }

        private static string StripWhiteSpace(string tmp, int txtCount, int nCount, int nMax)
        {   // https://www.w3.org/TR/SVG2/text.html#WhiteSpace
            string ret = WebUtility.HtmlDecode(tmp).Replace("\r", "");         // remove 'CR', keep 'LF'
            ret = ret.Replace("\t", " ");               // replace tab by space
            bool textWasFound = (txtCount > 0);
            string tmpLine;
            string retVal = "";

            string[] lines = ret.Split('\n');

            if (lines.Length == 1)
            { retVal = lines[0]; }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    //        Logger.Trace("stripWhiteSpace i:{0}  txtCount:{1}  nodeCount:{2}   nodeMax:{3}  txt:'{4}'", i, txtCount, nCount, nMax, lines[i]);
                    tmpLine = lines[i].Trim();

                    if (!textWasFound)
                    {
                        retVal += lines[i].TrimStart();         // remove leading spaces
                        if (tmpLine.Length > 0)
                            textWasFound = true;
                        continue;
                    }

                    if (i == lines.Length - 1)
                    {
                        if ((txtCount == 0) && (nCount < nMax))
                        { retVal += " "; continue; }

                        if ((nCount == nMax) && (tmp.Trim() == ""))
                            return "";

                        if (tmpLine.Length == 0)                // nothing to do
                            continue;
                    }
                    if (tmpLine.Length == 0)                    // empty but not last line, add space
                        retVal += " ";

                    if (lines[i].StartsWith(" "))               // reduce amount of leading space to one 
                    {
                        retVal += " " + lines[i].TrimStart();
                    }
                    else
                        retVal += lines[i];

                    if (lines[i].EndsWith(" "))
                    { retVal = retVal.TrimEnd() + " "; }
                }

                if (retVal.StartsWith(" "))                     // reduce amount of leading space to one 
                {
                    retVal = " " + retVal.TrimStart();
                }
                if (retVal.EndsWith(" "))                       // reduce amount of leading space to one 
                {
                    retVal = retVal.TrimEnd() + " ";
                }
            }
            //    Logger.Trace("stripWhiteSpace out:'{0}'", retVal);
            return retVal;
        }


        private static PointF ProcessTextPath(XElement pathElement, TextProperties textProp, PointF origin, int level)
        {
            textOnPath.Reset();
            string pathData = "";
            if (textProp.href != "")                        // find referenced path
            {
                string href = textProp.href.Substring(1);   // remove '#'

                IEnumerable<XElement> ids =
                    from id in svgCode.Descendants()        // get a list of IDs of all elements
                    select id;

                foreach (XElement id in ids)
                {
                    if (id.Attribute("id") != null && id.Attribute("id").Value == href) // find referenced element
                    {
                        Logger.Info("► ProcessTextPath  href:{0}  type:{1}  origin:{2:0.00}/{3:0.00}  level:{4}", href, id.Name.ToString(), origin.X, origin.Y, level);
                        Graphic.SetGeometry("textPath");

                        if (id.Name == (nspace + "path"))
                        {
                            lastTransformMatrix.SetIdentity();
                            //        lastTransformMatrix = ParseTransform(id, false, level);
                            ParseAttributs(id);

                            if (id.Attribute("d") != null)
                                pathData = id.Attribute("d").Value;
                            if (pathData == "")
                            {
                                if (id.Attribute("path") != null)
                                    pathData = id.Attribute("path").Value;
                            }
                            if (pathData != "")
                            {
                                //    Logger.Trace("d:{0}", pathData);
                                string separators = @"(?=[A-Za-z-[e]])";
                                var tokens = System.Text.RegularExpressions.Regex.Split(pathData, separators).Where(t => !string.IsNullOrEmpty(t));
                                int objCount = 0;
                                getTextPath = true;
                                foreach (string token in tokens)
                                    objCount += ParsePathCommandGraphicsPath(token);    // fill graphics path with shape
                                getTextPath = false;
                            }
                            else
                                Logger.Error("ProcessTextPath path found in <path id:{0}", href);
                        }
                        else
                        {
                            getTextPath = true;
                            lastTransformMatrix.SetIdentity();
                            ParseBasicElement(id, level);                               // fill graphics path with shape  incl. ParseTransform and ParseAttributs
                            getTextPath = false;
                        }
                    }
                }
            }

            if (pathElement.Attribute("path") != null)          // get path from text-element attribute
            {
                pathData = pathElement.Attribute("path").Value;
                if (pathData != "")
                {
                    Logger.Trace("d:{0}", pathData);
                    string separators = @"(?=[A-Za-z-[e]])";
                    var tokens = System.Text.RegularExpressions.Regex.Split(pathData, separators).Where(t => !string.IsNullOrEmpty(t));
                    int objCount = 0;
                    getTextPath = true;
                    foreach (string token in tokens)
                        objCount += ParsePathCommandGraphicsPath(token);
                    getTextPath = false;
                }
                else
                    Logger.Error("ProcessTextPath <textPath no path found");
            }

            //    string txt = pathElement.Value.TrimStart().TrimEnd(' ').TrimEnd('\n').Replace("\r", " ").Replace("\n", " ");
            string txt = "";// pathElement.Value.Trim().Replace("\r", "").Replace("\n", "");
            int nodeMax = pathElement.Nodes().Count();
            int nodeCount = 0;
            int textPathCount = 0;
            foreach (var node in pathElement.Nodes())
            {
                nodeCount++;
                if (node.NodeType == System.Xml.XmlNodeType.Text)               // Add text to myPath
                {
                    txt += StripWhiteSpace(node.ToString(), textPathCount, nodeCount, nodeMax);
                }
                else if (node.NodeType == System.Xml.XmlNodeType.Element)
                {
                    var nElement = (XElement)node;
                    if (nElement.Name == (nspace + "tspan"))
                    { txt += StripWhiteSpace(nElement.Value.ToString(), textPathCount, nodeCount, nodeMax); }
                    Graphic.SetHeaderInfo(" Unsupported SVG element order: 'tspan' within 'textPath'");
                    Graphic.SetHeaderMessage(string.Format(" {0}-1105: 'tspan' within 'textPath' is not supported", CodeMessage.Warning));
                }
            }

            if ((!string.IsNullOrEmpty(txt)) && (textOnPath.PointCount > 0))
            {
                //         lastTransformMatrix = System.Windows.Media.Matrix.Multiply(lastTransformMatrix, matrixElement);
                //    lastTransformMatrix = System.Windows.Media.Matrix.Multiply(matrixGroup[1],lastTransformMatrix);	// '1' = 1st g-level
                Matrix tmpM = new Matrix((float)lastTransformMatrix.M11, (float)lastTransformMatrix.M12, (float)lastTransformMatrix.M21, (float)lastTransformMatrix.M22,
                                        (float)lastTransformMatrix.OffsetX, (float)lastTransformMatrix.OffsetY);
                textOnPath.Transform(tmpM);

                //    Logger.Info("PathTransform M11:{0}  M12:{1}  M21:{2}  M22:{3}   ox:{4}  oy:{5}", lastTransformMatrix.M11, lastTransformMatrix.M12, lastTransformMatrix.M21, lastTransformMatrix.M22, lastTransformMatrix.OffsetX, lastTransformMatrix.OffsetY);

                textOnPath.Flatten(new Matrix(), 0.1f);
                textProp.ExportTextOnPath(textOnPath, txt);
            }
            return origin;
        }

        private static PointF AddText(string svgText, TextProperties textProp, PointF origin, ref int charIndex)
        {
            Logger.Info("► AddText txt:'{0}'  size:{1}   family:{2}   offsetX:{3}  offsetY:{4}   globalX:{5}  globalY:{6}", svgText, textProp.fontSize, textProp.fontFamily, origin.X, origin.Y, offsetX, offsetY);

            textProp.SetX(origin.X);
            textProp.SetY(origin.Y);

            float stringPos = textProp.ExportString(svgText);
            charIndex = textProp.CharIndex;
            return new PointF(stringPos, origin.Y);
        }

        private static void ExtractGlyphPath(GraphicsPath extractPath, PointF offset, string geometry)
        {
            if (!(extractPath.PointCount > 2))
                return;

            extractPath.Flatten(new Matrix(), 0.02f);
            if (logEnable) Logger.Info("ExtractGlyphPath  '{0}'  count:{1}", geometry, extractPath.PointCount);
            System.Drawing.PointF gpc;
            System.Drawing.PointF gpcStart = new System.Drawing.PointF();

            PointF[] tmpP = extractPath.PathPoints;
            byte[] types = extractPath.PathTypes;
            bool setGeometry = true;
            byte type;

            for (int gpi = 0; gpi < tmpP.Length; gpi++)
            {
                gpc = tmpP[gpi];
                type = types[gpi];

                if ((type & 0x03) == 0)
                {
                    SVGStartPath(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY, geometry, setGeometry);
                    setGeometry = false;
                    gpcStart = gpc;
                }
                else if ((type & 0x03) == 1)
                {
                    SVGMoveTo(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY, geometry);
                }

                else if ((type & 0x03) == 3)             // cubic Bézier-Spline
                {
                    SVGMoveTo(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY, geometry);
                }

                if ((type & 0x80) > 0)                  // https://docs.microsoft.com/de-de/dotnet/api/system.drawing.drawing2d.graphicspath.pathtypes?view=netframework-4.0
                {
                    SVGMoveTo(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY, geometry);
                    SVGMoveTo(gpcStart.X + offset.X + offsetX, gpcStart.Y + offset.Y + offsetY, geometry);
                    Graphic.StopPath(geometry);
                }
            }
            Graphic.StopPath(geometry);
        }


        private class TextProperties
        {   // https://www.w3.org/TR/SVG11/text.html    https://www.w3.org/TR/SVG2/text.html
            private float[] x = { 0f };
            private float[] y = { 0f };
            private float[] dx = { 0f };
            private float[] dy = { 0f };
            private float[] r = { };
            public string textFontFamily = "Times New Roman";  //"Arial";
            private string textFontWeight = "";         // normal | bold | bolder | lighter | <number>
            private string textFontStyle = "";
            private string textDecoration = "";
            private string textStartOffset = "";
            private float textLetterSpacing = 0;

            public int CharIndex = 0;
            public string stroke = "#000000";	//"none";
            public string fill = "#000000";
            public float fontSize = 16f;
            public System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Arial");
            public System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
            public System.Drawing.StringFormat stringFormat = new System.Drawing.StringFormat();
            public string href = "";

            public void List()
            {
                //Logger.Trace(" TextProperties  Weight:{0}  Style:{1}  Decoration:{2}   FontStyle:{3}", textFontWeight, textFontStyle, textDecoration, fontStyle);
                string tmp = "";
                if (r.Length > 0)
                {
                    for (int i = 0; i < r.Length; i++)
                        tmp += string.Format("{0}) {1}  ", i, r[i]);

                    Logger.Trace(" TextProperties r= {0}", tmp);
                }
                else
                    Logger.Trace(" TextProperties r= null");
            }

            public TextProperties()
            { }
            public TextProperties(XElement pathElement)
            {
                ReadAttributs(pathElement);
                SetTextProperties();
            }
            public TextProperties(TextProperties temp)
            {
                Array.Resize(ref x, temp.x.Length); temp.x.CopyTo(x, 0);
                Array.Resize(ref y, temp.y.Length); temp.y.CopyTo(y, 0);
                Array.Resize(ref dx, temp.dx.Length); temp.dx.CopyTo(dx, 0);
                Array.Resize(ref dy, temp.dy.Length); temp.dy.CopyTo(dy, 0);
                Array.Resize(ref r, temp.r.Length); temp.r.CopyTo(r, 0);
                textFontFamily = temp.textFontFamily;
                textFontWeight = temp.textFontWeight;
                textFontStyle = temp.textFontStyle;
                textDecoration = temp.textDecoration;
                textLetterSpacing = temp.textLetterSpacing;
                CharIndex = temp.CharIndex;

                stroke = temp.stroke;
                fill = temp.fill;
                fontSize = temp.fontSize;
                fontFamily = temp.fontFamily;
                fontStyle = temp.fontStyle;
                stringFormat = temp.stringFormat;
                SetTextProperties();
            }

            public bool Update(XElement pathElement)
            {
                bool val = ReadAttributs(pathElement);
                SetTextProperties();
                return val;
            }

            private void SetTextProperties()
            {
                stringFormat.FormatFlags |= System.Drawing.StringFormatFlags.NoClip | System.Drawing.StringFormatFlags.NoWrap | System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
                stringFormat.LineAlignment |= System.Drawing.StringAlignment.Near;  // Center;// Far;
                stringFormat.Alignment |= System.Drawing.StringAlignment.Near;

                fontStyle = System.Drawing.FontStyle.Regular;
                //    if (textFontWeight == "normal") fontStyle = System.Drawing.FontStyle.Regular;
                if (textFontWeight == "bold") fontStyle |= System.Drawing.FontStyle.Bold;
                if (textFontStyle == "italic") fontStyle |= System.Drawing.FontStyle.Italic;
                if (textDecoration == "line-through") fontStyle |= System.Drawing.FontStyle.Strikeout;
                if (textDecoration == "underline") fontStyle |= System.Drawing.FontStyle.Underline;

                try
                {
                    if (textFontFamily != "")
                    {
                        string[] parameters = textFontFamily.Split(',');
                        fontFamily = new FontFamily(parameters[0]);
                    }
                }
                catch (Exception err)
                {
                    Logger.Error(err, "SetTextProperties Font not found: {0}  ", textFontFamily);
                    textFontFamily = "Arial";
                    fontFamily = new FontFamily(textFontFamily);
                }
            }

            public float GetX()
            { return GetX(0); }
            public float GetX(int index)
            {
                if (index < 0) index = 0;
                if (index >= x.Length) index = x.Length - 1;
                return x[index];
            }
            public int GetXLength()
            { return x.Length; }

            public void SetX(float val)
            { x[0] = val; }

            public float GetY()
            { return GetY(0); }
            public int GetYLength()
            { return y.Length; }
            public float GetY(int index)
            {
                if (index < 0) index = 0;
                if (index >= y.Length) index = y.Length - 1;
                return y[index];
            }
            public void SetY(float val)
            { y[0] = val; }

            public float GetdX()
            { return GetdX(0); }
            public float GetdX(int index)
            {
                if (index < 0) index = 0;
                if (index >= dx.Length) index = dx.Length - 1;
                return dx[index];
            }
            public float GetdY()
            { return GetdY(0); }
            public float GetdY(int index)
            {
                if (index < 0) index = 0;
                if (index >= dy.Length) index = dy.Length - 1;
                return dy[index];
            }
            public float GetRotation()
            { return GetRotation(0); }
            public float GetRotation(int index)
            {
                if (r.Length == 0) return 0;
                if (index < 0) index = 0;
                if (index >= r.Length) index = r.Length - 1;
                return r[index];
            }
            public int GetRotationLength()
            { return r.Length; }


            public bool ReadAttributs(XElement element)
            {
                bool rotation = false;
                if (element.Attribute("style") != null)
                {
                    logSource = "ParseAttributs: font-size";
                    string fontValue = GetStyleProperty(element, "font-size");
                    if (!string.IsNullOrEmpty(fontValue))
                        fontSize = ConvertToPixel(fontValue);

                    fontValue = GetStyleProperty(element, "font-family").Replace("'", "");
                    if (!string.IsNullOrEmpty(fontValue))
                    { textFontFamily = fontValue; }

                    fontValue = GetStyleProperty(element, "font-weight").Replace("'", "");
                    if (!string.IsNullOrEmpty(fontValue))
                    { textFontWeight = fontValue; }

                    fontValue = GetStyleProperty(element, "font-style");
                    if (!string.IsNullOrEmpty(fontValue))
                    { textFontStyle = fontValue; }

                    fontValue = GetStyleProperty(element, "text-decoration");
                    if (!string.IsNullOrEmpty(fontValue))
                    { textDecoration = fontValue; }

                    fontValue = GetStyleProperty(element, "letter-spacing");
                    if (!string.IsNullOrEmpty(fontValue))
                    { textLetterSpacing = ConvertToPixel(fontValue); }

                    /* not implemented
                        fontValue = GetStyleProperty(element, "font-style");
                        fontValue = GetStyleProperty(element, "font-variant");
                        fontValue = GetStyleProperty(element, "font-stretch");
                        fontValue = GetStyleProperty(element, "stroke-width");*/
                }

                if (element.Attribute("font-style") != null)
                { textFontStyle = element.Attribute("font-style").Value; }

                if (element.Attribute("text-decoration") != null)
                { textDecoration = element.Attribute("text-decoration").Value; }

                if (element.Attribute("font-variant") != null)
                { }

                if (element.Attribute("font-weight") != null)
                { textFontWeight = element.Attribute("font-weight").Value.Replace("'", ""); }

                if (element.Attribute("font-stretch") != null)
                { }

                if (element.Attribute("font-family") != null)
                { textFontFamily = element.Attribute("font-family").Value.Replace("'", ""); }

                if (element.Attribute("font-size") != null)
                { fontSize = ConvertToPixel(element.Attribute("font-size").Value); }

                if (element.Attribute("letter-spacing") != null)
                { textLetterSpacing = ConvertToPixel(element.Attribute("letter-spacing").Value); }

                if (element.Attribute("stroke-width") != null)
                { }

                float tmpEM = factor_Em2Px;
                factor_Em2Px = fontSize;
                if (element.Attribute("x") != null) x = ConvertToPixelArray(element.Attribute("x").Value);
                if (element.Attribute("y") != null) y = ConvertToPixelArray(element.Attribute("y").Value);
                if (element.Attribute("dx") != null) dx = ConvertToPixelArray(element.Attribute("dx").Value);
                if (element.Attribute("dy") != null) dy = ConvertToPixelArray(element.Attribute("dy").Value);
                factor_Em2Px = tmpEM;

                if (element.Attribute("rotate") != null) { r = ConvertToPixelArray(element.Attribute("rotate").Value); rotation = true; }
                if (element.Attribute("fill") != null)
                {
                    fill = element.Attribute("fill").Value;
                    if (stroke == "")
                    {
                        if (fill != "none") { stroke = fill; }
                        else { stroke = "black"; }
                    }
                }
                if (element.Attribute("stroke") != null) stroke = element.Attribute("stroke").Value;

                if (element.Attribute(xlink + "href") != null) href = element.Attribute(xlink + "href").Value;
                if (element.Attribute("href") != null) href = element.Attribute("href").Value;

                if (element.Attribute("startOffset") != null) textStartOffset = element.Attribute("startOffset").Value;

                string[] notImplemented = { "textLength", "lengthAdjust", "method", "spacing" };
                for (int i = 0; i < notImplemented.Length; i++)
                {
                    if (element.Attribute(notImplemented[i]) != null)
                    {
                        Graphic.SetHeaderMessage(string.Format(" {0}-1106: Attribute is not implemented: '{1}'", CodeMessage.Warning, notImplemented[i]));
                    }
                }
                return rotation;
            }

            private float[] ConvertToPixelArray(string text)
            {
                char seperator = ' ';
                if (text.Contains(','))
                    seperator = ',';

                string[] parts = text.Split(seperator);
                int size = parts.Length;

                if (size > 0)
                {
                    float[] result = new float[size];
                    for (int i = 0; i < size; i++)
                    {
                        result[i] = ConvertToPixel(parts[i]);
                    }
                    return result;
                }
                return new float[] { 0 };
            }

            public float ExportString(string text)
            {
                if (text.Length == 0)
                    return GetX(0) + GetdX(0);

                if (text.StartsWith(" ") && (text.Trim() == ""))    // no content to draw
                {
                    CharIndex += text.Length;
                    return (text.Length * GetGlyphProperty(text[0], 1) * fontSize) + GetX(0) + GetdX(0);
                }

                List<float> posX = new List<float>
                {
                    0f   // first glyph starts at zero
                };
                float spaceWidth = GetGlyphProperty(' ', 1) * fontSize;

                /* if leading space " " */
                string tempText = text.Trim();
                float spaceWidthStartApply = 0;
                if (text.StartsWith(" "))
                {
                    spaceWidthStartApply += spaceWidth;
                    posX.Add(spaceWidth - (GetGlyphProperty(tempText[0], 0) * fontSize));
                }

                /* get glyph-positions  */
                float width = 0;
                for (int i = 1; i <= tempText.Length; i++)
                {
                    width = GetCharWidth(tempText.Substring(0, i)).Width + spaceWidthStartApply;
                    if ((i < tempText.Length) && (tempText[i - 1] == ' ')) { width += spaceWidth; }
                    posX.Add(width);
                }

                /* if ending with  space " "    */
                float spaceWidthEndApply = 0;
                if (text.EndsWith(" ")) { spaceWidthEndApply += spaceWidth; posX.Add(width + (GetGlyphProperty(tempText[tempText.Length - 1], 2) * fontSize) + spaceWidth); }

                for (int i = tempText.Length; i <= text.Length; i++)    // just for safety...
                { posX.Add(posX[tempText.Length]); }

                Font font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Millimeter);
                float yOffset = fontFamily.GetCellAscent(fontStyle) * font.Size / fontFamily.GetEmHeight(fontStyle);

                float xOffsetBounds = GetCharWidth(tempText).Left;            // Left-pos of bounds
                float XOffsetLSB = (GetGlyphProperty(text[0], 0) * fontSize);
                bool isPureText = ((x.Length <= 1) && (y.Length <= 1) && (dx.Length <= 1) && (dy.Length <= 1) && (r.Length <= 1) && (GetRotation() == 0) && (textLetterSpacing == 0));

                Logger.Trace("● ExportString '{0}'  pureText:{1}  start-X:{2:0.00}  textLen:{3:0.00}   spaceWidth:{4:0.00}   fontSize:{5:0.00}", text, isPureText, GetX(0), posX[text.Length], spaceWidth, fontSize);

                if (stroke != "") { Graphic.SetPenColor(stroke.StartsWith("#") ? stroke.Substring(1) : stroke); }  //Logger.Info("SetPenColor '{0}'", stroke); 
                if ((fill != "") && (fill != "none")) { Graphic.SetPenFill(fill.StartsWith("#") ? fill.Substring(1) : fill); }     //Logger.Info("SetPenFill  '{0}'", fill); 

                float oxOrig = GetX(0) + GetdX(0);
                float ox = oxOrig - xOffsetBounds + spaceWidthStartApply + XOffsetLSB;
                float oy = GetY(0) + GetdY(0) - yOffset;

                //        Logger.Info("TEST oxOrig:{0:0.00}  ox:{1:0.00}   xOffset:{2:0.00}   spaceWidthStartApply:{3:0.00}  LSB:{4:0.00}", oxOrig, ox, xOffsetBounds, spaceWidthStartApply, XOffsetLSB);
                if (isPureText)
                {
                    using (var path = new GraphicsPath())               // do whole text in one go
                    {
                        DrawGlyphPath(path, new PointF(ox, oy), new PointF(ox, oy + yOffset), GetRotation(), text, StringAlignment.Near);
                        ExtractGlyphPath(path, new PointF(0, 0), "tspan '" + text + "'");            // StartPath & Graphic.StopPath
                    }
                    CharIndex += text.Length;
                    return posX[text.Length] + ox + spaceWidthEndApply;
                }
                else
                {
                    xOffsetBounds = GetGlyphProperty(text[0], 0) * fontSize;  // get LSB
                    width = 0;
                    float tmpWidth;
                    posX = new List<float>
                    {
                        0f                                       // first glyph starts at zero
                    };
                    for (int i = 0; i < text.Length; i++)
                    {
                        tmpWidth = GetGlyphProperty(text[i], 1) * fontSize;
                        width += tmpWidth + textLetterSpacing;
                        posX.Add(width);
                    }
                    posX.Add(width);

                    using (var path = new GraphicsPath())               // do char by char to adjust e.g. dx individual
                    {
                        int ci;
                        for (int i = 0; i < text.Length; i++)
                        {
                            ci = i + CharIndex;
                            oxOrig = GetX(ci) + GetdX(ci);
                            if (x.Length > i)                           // if individual position is given
                                ox = GetX(ci) + GetdX(ci) - xOffsetBounds + spaceWidthStartApply;
                            else
                                ox = GetX(ci) + GetdX(ci) + posX[i] - xOffsetBounds + spaceWidthStartApply;
                            oy = GetY(ci) + GetdY(ci) - yOffset;

                            DrawGlyphPath(path, new PointF(ox, oy), new PointF(ox, oy + yOffset), GetRotation(ci), text[i].ToString(), StringAlignment.Near);
                            ExtractGlyphPath(path, new PointF(0, 0), "tspan '" + text[i] + "'");            // StartPath & Graphic.StopPath
                        }
                    }
                }
                CharIndex += text.Length;
                return GetGlyphProperty(text[text.Length - 1], 1) * fontSize + ox + spaceWidthEndApply;
            }

            public void ExportTextOnPath(GraphicsPath toPath, string text)
            {
                if ((!string.IsNullOrEmpty(text)) && (toPath.PointCount > 0))
                {
                    string testChar = "#";
                    float tmpWidth = GetCharWidth(testChar + testChar).Width;
                    float angle, width;
                    PointF origin;
                    List<float> posX = new List<float>();
                    List<float> GlyphWidth = new List<float>();

                    for (int i = 0; i < text.Length; i++)       // get char positions
                    {
                        width = GetCharWidth(testChar + text.Substring(0, i) + testChar).Width - tmpWidth + textLetterSpacing * i;
                        posX.Add(width);
                        //    width = GetCharWidth(testChar + text.Substring(i, 1) + testChar).Width - tmpWidth;
                        width = GetGlyphProperty(text[i], 1) * fontSize;
                        GlyphWidth.Add(width);
                    }

                    PointF[] pPath = toPath.PathPoints;
                    int pIndex = 0;
                    Font font = new Font(fontFamily, fontSize, fontStyle, GraphicsUnit.Millimeter);
                    float yOffset = fontFamily.GetCellAscent(fontStyle) * font.Size / fontFamily.GetEmHeight(fontStyle);
                    float xpos;
                    float pathLength = GetPathLength(toPath);
                    float startOffset = 0;

                    if (textStartOffset != "")
                    { startOffset = ConvertToPixel(textStartOffset, pathLength); }

                    Logger.Trace("● ExportTextOnPath '{0}'  count:{1}   length:{2:0.00}   startOffset:{3:0.00}   height:{4}   size:{5}  size:{6}  emheight:{7}", text.Replace("\r", "").Replace("\n", ""), pPath.Length, pathLength, startOffset, yOffset, fontSize, font.Size, fontFamily.GetEmHeight(fontStyle));
                    if (stroke != "") { Graphic.SetPenColor(stroke.StartsWith("#") ? stroke.Substring(1) : stroke); }
                    if ((fill != "") && (fill != "none")) { Graphic.SetPenFill(fill.StartsWith("#") ? fill.Substring(1) : fill); }

                    using (var path = new GraphicsPath())       // place glyphs
                    {
                        for (int i = 0; i < text.Length; i++)
                        {
                            xpos = posX[i] + startOffset;           // left pos of glyph...
                            if (xpos > (pathLength * 1.2))          // still on path?
                                break;
                            xpos += GlyphWidth[i] / 2;              // glyph origin is center

                            origin = getPathPos(xpos, ref pIndex);

                            if (pIndex > 0)
                                angle = GetAngle(pPath[pIndex - 1], pPath[pIndex + 1]);
                            else
                                angle = GetAngle(pPath[pIndex], pPath[pIndex + 1]);

                            //       Logger.Trace("●  ExportTextOnPath '{0}' x:{1}  y:{2}", text[i],origin.X,origin.Y);
                            DrawGlyphPath(path, new PointF(origin.X, origin.Y - yOffset), new PointF(origin.X, origin.Y), angle, text[i].ToString(), StringAlignment.Center);
                            ExtractGlyphPath(path, new PointF(0, 0), "textPath '" + text[i] + "'");            // StartPath & Graphic.StopPath
                        }
                    }
                    CharIndex += text.Length;

                    PointF getPathPos(float s, ref int index)
                    {
                        double dc, distanceOld = 0, distance = 0;
                        float x, y, lx = pPath[0].X, ly = pPath[0].Y;

                        for (int i = 0; i < pPath.Length - 1; i++)
                        {
                            x = pPath[i].X - lx;
                            y = pPath[i].Y - ly;
                            dc = Math.Sqrt(x * x + y * y);
                            distance = distanceOld + dc;
                            index = i;

                            if (distance == s)          // exactly on pPath coordinate
                            {
                                return pPath[i];
                            }
                            else if (distance > s)
                            {
                                double needed = s - distanceOld;
                                double ratio = needed / dc;
                                x = (float)ratio * x;
                                y = (float)ratio * y;
                                //   double tc = Math.Sqrt(x * x + y * y);
                                //   float nx = pPath[i].X + x, ny = pPath[i].Y + y;
                                //   Logger.Info("getPathIndex > s:{0:0.00}   distance:{1:0.00}  distanceOld:{2:0.00}  delta:{3:0.00}  needed:{4:0.00}  corrected:{5:0.00}  x:{6:0.00}  y:{7:0.00}  index:{8}", s, distance, distanceOld, dc, needed, (distanceOld + tc), nx, ny, i);
                                i--;
                                if (i < 0) i = 0;
                                return new PointF(pPath[i].X + x, pPath[i].Y + y);
                            }

                            lx = pPath[i].X;
                            ly = pPath[i].Y;
                            distanceOld = distance;
                        }
                        return pPath[pPath.Length - 1];
                    }
                    float GetAngle(PointF p1, PointF p2)
                    {
                        double c;
                        c = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
                        if (c == 0)
                            return 0;

                        if (p1.X > p2.X)
                            return (float)(Math.Asin((p1.Y - p2.Y) / c) * 180 / Math.PI - 180);
                        else
                            return (float)(Math.Asin((p2.Y - p1.Y) / c) * 180 / Math.PI);
                    }
                }
            }

            private void DrawGlyphPath(GraphicsPath myPath, PointF origin, PointF originR, float angle, string c, StringAlignment alignment)
            {
                var format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);
                format.Alignment |= alignment;

                myPath.Reset();
                myPath.AddString(c, fontFamily, (int)fontStyle, fontSize,
                                 origin, format);// stringFormat);

                //    myPath.AddLine(origin.X, originR.Y + 2, origin.X, originR.Y);     // draw ceoss at origin
                //    myPath.AddLine(origin.X, originR.Y, origin.X + 1, originR.Y);

                if (angle != 0)
                {
                    Matrix rotation_matrix = new Matrix();
                    rotation_matrix.RotateAt(angle, originR);    //new PointF(x, y));
                    myPath.Transform(rotation_matrix);
                }
            }

            private RectangleF GetCharWidth(string c)
            {
                if (string.IsNullOrEmpty(c))
                    return new RectangleF();

                using (var path = new GraphicsPath())
                using (var format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces))
                {
                    format.Alignment |= StringAlignment.Near;

                    path.AddString(c, fontFamily, (int)fontStyle, fontSize,
                                   new PointF(), format);

                    //    path.Flatten();
                    RectangleF textBounds = path.GetBounds();
                    return textBounds;
                }
            }

            private float GetGlyphProperty(ushort c, int property)
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.glyphtypeface?redirectedfrom=MSDN&view=netframework-4.0#remarks
                System.Windows.Media.FontFamily fontFamily1 = new System.Windows.Media.FontFamily(textFontFamily);

                var typeface = new System.Windows.Media.Typeface(fontFamily1, System.Windows.FontStyles.Normal, System.Windows.FontWeights.Normal, System.Windows.FontStretches.Medium);
                System.Windows.Media.GlyphTypeface gtf;

                if (!typeface.TryGetGlyphTypeface(out gtf))
                {
                    Logger.Error("GetGlyphProperty  GlyphTypeface = null");
                    return 0;
                }

                double retVal = 0;
                ushort gi;

                if (gtf.CharacterToGlyphMap.ContainsKey(c))
                { gi = gtf.CharacterToGlyphMap[c]; }
                else
                { Logger.Error("GetGlyphProperty  key not in CharacterToGlyphMap:{0}", c); return 1; }

                if (gtf != null)
                {
                    if (property == 0)
                        retVal = gtf.LeftSideBearings[gi];
                    if (property == 1)
                        retVal = gtf.AdvanceWidths[gi];
                    if (property == 2)
                        retVal = gtf.RightSideBearings[gi];
                    if (property == 3)
                        retVal = gtf.LeftSideBearings[gi] + gtf.AdvanceWidths[gi] + gtf.RightSideBearings[gi];
                    if (property == 4)
                        retVal = gtf.AdvanceWidths[gi] - gtf.LeftSideBearings[gi] - gtf.RightSideBearings[gi];
                }
                return (float)retVal;
            }

            private float GetPathLength(GraphicsPath toPath)
            {
                PointF[] tmpP = toPath.PathPoints;
                PointF pLast, pNew;
                float length = 0;

                pLast = tmpP[0];
                for (int gpi = 1; gpi < tmpP.Length; gpi++)
                {
                    pNew = tmpP[gpi];
                    length += calcDiff(pNew, pLast);
                    pLast = pNew;
                }
                return length;

                float calcDiff(PointF a, PointF b)
                {
                    float dx = a.X - b.X;
                    float dy = a.Y - b.Y;
                    return (float)Math.Sqrt(dx * dx + dy * dy);
                }
            }
        }
    }
}