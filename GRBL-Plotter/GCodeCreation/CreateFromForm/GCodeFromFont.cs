/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2018-2021 Sven Hasemann contact: svenhb@web.de

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
 * 2019-06-05 add new Hershey fonts from https://github.com/evil-mad/EggBot/tree/master/inkscape_driver
 * 2019-06-10 insert xmlMarker.figureStart tag
 * 2019-07-08 add char-info to xmlMarker.figureStart tag
 * 2019-08-15 add logger
 * 2020-02-22 bug fix line 290
 * 2020-02-25 switch from gcode.xx to plotter.xx to support tangential axis
 * 2020-04-28 insert Graphic.xx
 * 2020-07-10 clean up
 * 2021-02-13 line 125 return if no font found
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GRBL_Plotter
{
    public static partial class GCodeFromFont
    {
        public static string gcFontName = "standard";
        public static string gcText = "";       // text to convert
        public static int gcFont = 0;           // text to convert
        public static int gcAttachPoint = 7;    // origin of text 1 = Top left; 2 = Top center; 3 = Top right; etc
        public static double gcHeight = 1;      // desired Text height
        public static double gcWidth = 1;       // desired Text width
        public static double gcAngleRad = 0;
        public static double gcSpacing = 1;     // Percentage of default (3-on-5) line spacing to be applied. Valid values range from 0.25 to 4.00.
        public static double gcOffX = 0;
        public static double gcOffY = 0;
        public static double gcLineDistance = 1.5;
        public static double gcFontDistance = 0;

        public static bool gcPauseLine = false;
        public static bool gcPauseWord = false;
        public static bool gcPauseChar = false;
        public static bool gcConnectLetter = false;

        private static double gcLetterSpacing = 3;  
        private static double gcWordSpacing = 6.75; 

        private static double offsetX = 0;
        private static double offsetY = 0;
        private static bool useLFF = false;
        private static bool isSameWord = false;
        
        private static XNamespace nspace = "http://www.w3.org/2000/svg";
        private static XElement svgCode;
        private static bool useSVGFile = false;
        
        public class globalSettings
        {
            public string FontName;     //font-family="EMS Allure"
            public double UnitsPerEM;   //units-per-em="1000"
            public double Ascent;       //ascent="800"
            public double Descent;      //descent="-200"
            public double CapHeight;    //cap-height="500"
            public double XHeight;      //x-height="300"     
            public globalSettings()
            {FontName=""; UnitsPerEM = 1000;Descent=800;Descent=-200;CapHeight=500;XHeight=300;}
        }
        private static globalSettings SVGFontProp = new globalSettings();

        public class glyph
        {
            public double x;
            public string d;
            public glyph()
            {x=0;d="";}
            public glyph(double tx, string td)
            {x=tx;d=td;}
        }
        private static Dictionary<string, glyph> svgGlyphs = new Dictionary<string, glyph>();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string[] fontFileName()
        {
            if (Directory.Exists(datapath.fonts))
            {   List<string> tmp = new List<string>(Directory.GetFiles(datapath.fonts, "*.*", SearchOption.AllDirectories));
                for (int k = tmp.Count-1; k >= 0; k--)
                {
                    tmp[k] = tmp[k].Substring(datapath.fonts.Length+1);
                    if (!(tmp[k].EndsWith("lff") || tmp[k].EndsWith("svg")))
                    { tmp.RemoveAt(k); }
                }
                return tmp.ToArray();
            }
            return new string[0];
        }
        public static void reset()
        {
            gcFontName = "standard"; gcText = ""; gcFont = 0; gcAttachPoint = 7;
            gcHeight = 0; gcWidth = 0; gcAngleRad = 0; gcSpacing = 1; gcOffX = 0; gcOffY = 0;
            gcPauseLine = false; gcPauseWord = false; gcPauseChar = false; 
            useLFF = false; gcLineDistance = 1.5; gcFontDistance = 0;
            useSVGFile = true;
        }

        public static void getCode() 
        {   
            double scale = gcHeight / 21;
            string tmp1 = gcText.Replace('\r', '|');

            Logger.Trace("Create GCode, text length {0}, font '{1}', Text '{2}'", gcText.Length, gcFontName, tmp1.Replace('\n', ' '));
            string[] fileContent=new string[] { "" };

            useSVGFile = false;
            useLFF = false;
            string fileName = "";
            //            if (gcFontName.IndexOf(@"fonts\") >= 0)     // file path included?
            if (gcFontName.ToLower().EndsWith(".svg") || gcFontName.ToLower().EndsWith(".lff"))    // file path included?
                fileName = datapath.fonts + "\\" + gcFontName;

            if (gcFontName != "")
            {   if (File.Exists(fileName))
                {   Graphic.SetHeaderInfo(" Font from file: "+fileName);
                    if (fileName.ToLower().EndsWith(".svg"))
                    {   loadSVGFont(fileName); }
                    else
                    {
                        fileContent = File.ReadAllLines(fileName);
                        scale = gcHeight / 9;
                        useLFF = true;
                        offsetY = 0;
                        gcLineDistance = 1.667 * gcSpacing;
                        foreach (string line in fileContent)
                        {   if (line.IndexOf("LetterSpacing") >= 0)
                            {   string[] tmp = line.Split(':');
                                gcLetterSpacing = double.Parse(tmp[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);//Convert.ToDouble(tmp[1].Trim());
                            }
                            if (line.IndexOf("WordSpacing") >= 0)
                            {
                                string[] tmp = line.Split(':');
                                gcWordSpacing = double.Parse(tmp[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);//Convert.ToDouble(tmp[1].Trim());
                            }
                        }
                    }
                }
                else
                {   if (!hersheyFonts.ContainsKey(gcFontName))
                    {
                        string info = string.Format(" Font '{0}' or file '{1}' not found", gcFontName, fileName);
                        Logger.Error(info);
                        Graphic.SetHeaderInfo(info);
                        return;
                    }
                    else
                    { Graphic.SetHeaderInfo(" Font from array: " + gcFontName); }

                }
            }
            bool centerLine = false;
            bool rightLine = false;
            if ((gcAttachPoint == 2) || (gcAttachPoint == 5) || (gcAttachPoint == 8))
            { gcOffX -= gcWidth / 2; centerLine = true; }
            if ((gcAttachPoint == 3) || (gcAttachPoint == 6) || (gcAttachPoint == 9))
            { gcOffX -= gcWidth; rightLine = true; }
            if ((gcAttachPoint == 4) || (gcAttachPoint == 5) || (gcAttachPoint == 6))
                gcOffY -= gcHeight / 2;
            if ((gcAttachPoint == 1) || (gcAttachPoint == 2) || (gcAttachPoint == 3))
                gcOffY -= gcHeight;

            string[] lines;
            if (gcText.IndexOf("\\P") >= 0)
            {   gcText = gcText.Replace("\\P", "\n"); }
            lines = gcText.Split('\n');

            int maxCharCount = 0;
            foreach (string tmp in lines)
            { maxCharCount = Math.Max(maxCharCount, tmp.Length); }
            double charWidth = gcWidth / maxCharCount;

            offsetX = 0;
            offsetY = 9 * scale + ((double)lines.Length - 1) * gcHeight * gcLineDistance;
            if (useLFF)
                offsetY = ((double)lines.Length - 1) * gcHeight * gcLineDistance;
            if (useSVGFile)
                offsetY = ((double)lines.Length - 1) * gcHeight * gcLineDistance;

            isSameWord = false;

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)          // loop text-lines
            {
                if (lineIndex > 0)
                {
                    offsetY -= gcHeight * gcLineDistance;
                    if (gcPauseLine)
                        gcodePause("Pause before line");

                }
                string actualLine = lines[lineIndex];
                for (int txtIndex = 0; txtIndex < actualLine.Length; txtIndex++)    // loop single char of a text-line
                {
                    char actualChar = actualLine[txtIndex];
                    int chrIndex = (int)actualChar - 32;
                    int chrIndexLFF = (int)actualChar;
                    Graphic.SetPathId(lineIndex+"-"+txtIndex);

                    if (txtIndex==0)    //actualChar == '\n')                       // next line
                    {
                        offsetX = 0;
                        if (centerLine)
                            offsetX = (gcWidth - (charWidth * actualLine.Length)) / 2;      //  center line
                        else if (rightLine)
                            offsetX = (gcWidth - (charWidth * actualLine.Length));
                        isSameWord = false;
                    }
                    if (useLFF)                                                     // LFF Font (LibreCAD font file format)
                    {
                        if (chrIndexLFF > 32)
                        { Graphic.SetGeometry(string.Format("Char '{0}'", actualChar)); }

                        drawLetterLFF(ref fileContent, chrIndexLFF, scale);         // regular char
                        gcodePenUp("getCode     ");
                    }
                    else if (!useSVGFile)
                    {
                        if (((chrIndex < 0) || (chrIndex > 95)))     // no valid char
                        {
                            offsetX += 2 * gcSpacing;                    // apply space
                            isSameWord = false;
                            if (gcPauseWord)
                                gcodePause("Pause before word");
                        }
                        else
                        {
                            if (gcPauseChar)
                                gcodePause("Pause before char");
                            if (gcPauseChar && (actualChar == ' '))
                                gcodePause("Pause before word");
                            Graphic.SetGeometry(string.Format("Char {0}", actualChar));
                            drawLetter(hersheyFonts[gcFontName][chrIndex], scale, actualChar.ToString()); // regular char
                        }
                    }
                    else // useSVGFile
                    {
                        if (actualChar < '_')
                        {
                            isSameWord = false;
                            if (gcPauseWord)
                                gcodePause("Pause before word");
                        }
                        if (gcPauseChar)
                            gcodePause("Pause before char");
                        if (gcPauseChar && (actualChar == ' '))
                            gcodePause("Pause before word");
                        Graphic.SetGeometry(string.Format("Char {0}", actualChar));
                        drawLetterSVGFont(scale, actualChar.ToString());
                    }
                }
            }
            if (!useLFF)
            {   gcodePenUp("getCode"); }
        }

        // http://forum.librecad.org/Some-questions-about-the-LFF-fonts-td5715159.html

        private static void loadSVGFont(string fileName)
        {   
            svgCode = XElement.Load(fileName, LoadOptions.None);    // PreserveWhitespace);
            XElement svgFont;

            /* get global settings*/ 
            if ((svgCode.Element(nspace + "defs") != null) && (svgCode.Element(nspace + "defs").Element(nspace + "font") != null))
            {   svgFont = svgCode.Element(nspace + "defs").Element(nspace + "font");
                if (svgFont.Element(nspace + "font-face") != null)
                {   XElement ff = svgFont.Element(nspace + "font-face");
                    if (ff.Attribute("font-family") != null)
                    {   SVGFontProp.FontName = ff.Attribute("font-family").Value;}
                    if (ff.Attribute("units-per-em") != null)
                    {   SVGFontProp.UnitsPerEM = double.Parse(ff.Attribute("units-per-em").Value, NumberFormatInfo.InvariantInfo);}
                    if (ff.Attribute("x-height") != null)
                    {   SVGFontProp.XHeight = double.Parse(ff.Attribute("x-height").Value, NumberFormatInfo.InvariantInfo);}
                    if (ff.Attribute("ascent") != null)
                    {   SVGFontProp.Ascent = double.Parse(ff.Attribute("ascent").Value, NumberFormatInfo.InvariantInfo);}
                    if (ff.Attribute("descent") != null)
                    {   SVGFontProp.Descent = double.Parse(ff.Attribute("descent").Value, NumberFormatInfo.InvariantInfo);}
                    if (ff.Attribute("cap-height") != null)
                    {   SVGFontProp.CapHeight = double.Parse(ff.Attribute("cap-height").Value, NumberFormatInfo.InvariantInfo);}
               }
            }
            else
            {   Logger.Error("loadSVGFont - Elements defs and font not found");
                return;
            }
            
            /* fill dictionary */
            svgGlyphs = new Dictionary<string, glyph>();
            double x = 0; string d = "";
            int cntChar=0;
            foreach (var glyphElement in svgFont.Elements(nspace + "glyph"))
            {   if (glyphElement != null)
                {   if ((glyphElement.Attribute("unicode") != null))    // && (glyphElement.Attribute("unicode").Value == actualChar)
                    {   string uniChar = glyphElement.Attribute("unicode").Value;
                        
                        if (glyphElement.Attribute("horiz-adv-x") != null)
                        {   x = double.Parse(glyphElement.Attribute("horiz-adv-x").Value, NumberFormatInfo.InvariantInfo);  }
                        
                        if (glyphElement.Attribute("d") != null)
                        {   d = glyphElement.Attribute("d").Value; }

                        if (!svgGlyphs.ContainsKey(uniChar))
                            svgGlyphs.Add(uniChar, new glyph(x, d));
                        else
                            Logger.Trace("loadSVGFont key already added: '{0}'",uniChar);
                        cntChar++;
                    }
                }
            }

            /* missing glyph */
            x=0; d="";
            if (svgFont.Element(nspace + "missing-glyph") != null)
            {   if (svgFont.Element(nspace + "missing-glyph").Attribute("horiz-adv-x") != null)
                {   x = double.Parse(svgFont.Element(nspace + "missing-glyph").Attribute("horiz-adv-x").Value, NumberFormatInfo.InvariantInfo);   }      
                if (svgFont.Element(nspace + "missing-glyph").Attribute("d") != null)
                {   d = svgFont.Element(nspace + "missing-glyph").Attribute("d").Value;       }
                svgGlyphs.Add("missing-glyph", new glyph(x,d));                        
            }

            Logger.Trace("loadSVGFont: '{0}'  chars:{1}",fileName,cntChar);
            useSVGFile = true;
        }

        private static void drawLetterSVGFont(double scale, string actualCharString)
        {   // https://gitlab.com/oskay/svg-fonts
            // <missing-glyph horiz-adv-x="378" />
            // <glyph unicode="!" glyph-name="exclam" horiz-adv-x="359" d="M 444 665 L 403 592 L 295 362 L 232 214 M 204 63 L 182 40.9" />
            //Logger.Trace("drawLetterSVGFont");
            scale = gcHeight/ (21*31.52);   //=661.92 factor_Em2Px = 150; scale = 0.0254    *1000 = 25,4

            glyph tmpGlyph = null;
            if (svgGlyphs.ContainsKey(actualCharString))
            {
                //Logger.Trace("drawLetterSVGFont '{0}'", actualCharString);
                tmpGlyph = svgGlyphs[actualCharString];
            }
            else if (svgGlyphs.ContainsKey("missing-glyph"))
            {
                Logger.Trace("drawLetterSVGFont 'missing - glyph'");
                tmpGlyph = svgGlyphs["missing-glyph"];
            }
            else
            {   Logger.Error("Glyph not found");
                return;
            }

     //       var tmpX = offsetX;// + tmpGlyph.x * scale;
            if (tmpGlyph.d.Length > 0)
            {
                string separators = @"(?=[A-Za-z])";
                var tokens = Regex.Split(tmpGlyph.d, separators).Where(t => !string.IsNullOrEmpty(t));                
                int token_cnt = 0;
                foreach (string token in tokens)
                {   drawToken(token, offsetX + gcOffX, offsetY + gcOffY, scale, token_cnt++, false); }
            }                  
            offsetX += tmpGlyph.x * scale + gcFontDistance; //double.Parse(svgsplit[1]) * scale + gcFontDistance;
            isSameWord = true;
        }

        private static double drawLetterLFF(ref string[] txtFont, int index, double scale, bool isCopy=false)
        {
            int lineIndex = 0;
            double maxX = 0;
            char[] charsToTrim = { '#' };

            if (index <= 32)
            {   offsetX += gcWordSpacing * scale;
                if (gcPauseWord)
                {   gcodePause("Pause before word");
                }
            }
            else
            {
                for (lineIndex = 0; lineIndex < txtFont.Length; lineIndex++)
                {
                    if ((txtFont[lineIndex].Length > 0) && (txtFont[lineIndex][0] == '['))
                    {
                        string nrString = txtFont[lineIndex].Substring(1, txtFont[lineIndex].IndexOf(']') - 1);
                        nrString = nrString.Trim('#');// charsToTrim);
                        int chrIndex = 0;
                        try
                        {  chrIndex = Convert.ToInt32(nrString, 16);
                        }
                        catch { MessageBox.Show("Line "+ txtFont[lineIndex]+"  Found "+nrString); }
                        if (chrIndex == index)                                              // found char
                        {
                            charXOld = -1; charYOld = -1;
                            if (gcPauseChar)
                                gcodePause("Pause before char");

                            int pathIndex;
                            for (pathIndex = lineIndex + 1; pathIndex < txtFont.Length; pathIndex++)
                            {
                                if (txtFont[pathIndex].Length < 2)
                                    break;
                                if (txtFont[pathIndex][0] == 'C')       // copy other char first
                                {
                                    int copyIndex = Convert.ToInt16(txtFont[pathIndex].Substring(1, 4), 16);
                                    maxX = drawLetterLFF(ref txtFont, copyIndex, scale, true);
                                }
                                else
                                    maxX = Math.Max(maxX, drawTokenLFF(txtFont[pathIndex], offsetX, offsetY, scale));
                            }
                            break;
                        }
                    }
                }
                if (!isCopy)
                    offsetX += maxX + gcLetterSpacing * scale + gcFontDistance; ;// + (double)nUDFontDistance.Value;
            }
            return maxX;
        }

        private static double charX, charY, charXOld=0, charYOld=0;

        private static double drawTokenLFF(string txtPath, double offX, double offY, double scale)
        {   string[] points = txtPath.Split(';');
            int cnt = 0;
            double xx,yy,x,y,xOld=0,yOld=0,bulge=0,maxX = 0;
            foreach (string point in points)
            {   string[] scoord = point.Split(',');
                charX = double.Parse(scoord[0], CultureInfo.InvariantCulture.NumberFormat);
                xx = charX * scale ;
                maxX = Math.Max(maxX, xx);
                xx += offX;
                charY = double.Parse(scoord[1], CultureInfo.InvariantCulture.NumberFormat); 
                yy = charY * scale + offY;
                if (gcAngleRad == 0)
                { x = xx + gcOffX; y = yy + gcOffY; }
                else
                {   x = xx * Math.Cos(gcAngleRad) - yy * Math.Sin(gcAngleRad) + gcOffX;
                    y = xx * Math.Sin(gcAngleRad) + yy * Math.Cos(gcAngleRad) + gcOffY;
                }

                if (scoord.Length > 2)
                {   if (scoord[2].IndexOf('A')>=0)
                        bulge = double.Parse(scoord[2].Substring(1), CultureInfo.InvariantCulture.NumberFormat); 
                    AddRoundCorner(bulge, xOld, yOld, x, y);
                }
                else if (cnt == 0)
                {   if ((charX != charXOld) || (charY != charYOld))
                    {
                        gcodePenUp("drawTokenLFF");
                        gcodeMove(0, (float)x, (float)y);
                    }
                }
                else
                    gcodeMove(1, (float)x, (float)y);
                cnt++;
                xOld = x; yOld = y;
                charXOld = charX; charYOld = charY;
            }
            return maxX;
        }

        // break down path of a single char into pieces
        private static void drawLetter(string svgtxt, double scale, string comment)
        {
            string separators = @"(?=[A-Za-z])";
            var tokens = Regex.Split(svgtxt, separators).Where(t => !string.IsNullOrEmpty(t));
            string[] svgsplit = svgtxt.Split(' ');
            double tmpX = 0;
            tmpX = offsetX - double.Parse(svgsplit[0], NumberFormatInfo.InvariantInfo) * scale;
            int token_cnt = 0;
            foreach (string token in tokens)
            {   if (token.StartsWith("M") || token.StartsWith("L"))
                {   drawToken(token, tmpX + gcOffX, offsetY + gcOffY, scale, token_cnt++, true);
//                    Logger.Trace("token:{0}  x:{1}  y:{2}  cnt:{3}", token, (tmpX + gcOffX), (offsetY + gcOffY), token_cnt);
                }
            }
            offsetX = tmpX + double.Parse(svgsplit[1], NumberFormatInfo.InvariantInfo) * scale + gcFontDistance; //double.Parse(svgsplit[1]) * scale + gcFontDistance;
            isSameWord = true;
        }

        // draw a piece of the letter path: M x,y  or L x,y
        private static void drawToken(string svgPath, double offX, double offY, double scale, int tnr, bool invertY)
        {
            var cmd = svgPath.Take(1).Single();
            string remainingargs = svgPath.Substring(1);
            string argSeparators = @"[\s,]|(?=-)";
            var splitArgs = Regex
                .Split(remainingargs, argSeparators)
                .Where(t => !string.IsNullOrEmpty(t));

            double[] floatArgs = splitArgs.Select(arg => double.Parse(arg, NumberFormatInfo.InvariantInfo) * scale).ToArray();
            double y;
            if (cmd == 'M')
            {   if (gcConnectLetter && isSameWord && (tnr == 0))
                {   for (int i = 0; i < floatArgs.Length; i += 2)
                    {   y = invertY ? -floatArgs[i + 1] : floatArgs[i + 1];
                        gcodeMove(1, (float)(offX + floatArgs[i]), (float)(offY + y));  // G 1
                    }
                }
                else
                {   gcodePenUp("drawToken");
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    {   y = invertY ? -floatArgs[i + 1] : floatArgs[i + 1];
                        gcodeMove(0, (float)(offX + floatArgs[i]), (float)(offY + y));  // G 0
                    }
                }
            }
            if (cmd == 'L')
            {   for (int i = 0; i < floatArgs.Length; i += 2)
                {   y = invertY ? -floatArgs[i + 1] : floatArgs[i + 1];
                    gcodeMove(1, (float)(offX + floatArgs[i]), (float)(offY + y));
                }
            }
        }

        private static void gcodeMove(int gnr, float x, float y, string cmd = "")
        {   if (gnr == 0)
				Graphic.StartPath(new Point(x, y));
			else
				Graphic.AddLine(new Point(x, y));
        }

        private static void gcodePenUp(string cmt)
        {  	Graphic.StopPath(cmt);  }
        private static void gcodePause(string cmt)
        {   Graphic.OptionInsertPause();	}     

        private static void AddRoundCorner(double bulge, double p1x, double p1y, double p2x, double p2y)
        {
            //Definition of bulge, from Autodesk DXF fileformat specs
            double angle = Math.Abs(Math.Atan(bulge) * 4);
            bool girou = false;

            //For my method, this angle should always be less than 180. 
            if (angle > Math.PI)
            {
                angle = Math.PI * 2 - angle;
                girou = true;
            }

            //Distance between the two vertexes, the angle between Center-P1 and P1-P2 and the arc radius
            double distance = Math.Sqrt(Math.Pow(p1x - p2x, 2) + Math.Pow(p1y - p2y, 2));
            double alpha = (Math.PI - angle) / 2;
            double ratio = distance * Math.Sin(alpha) / Math.Sin(angle);
            if (angle == Math.PI)
                ratio = distance/2;

            double xc, yc, direction;

            //Used to invert the signal of the calculations below
            bool isg2 = bulge < 0;
            if (isg2)
                direction = 1;
            else
                direction = -1;

            //calculate the arc center
            double part = Math.Sqrt(Math.Pow(2 * ratio / distance, 2) - 1);

            if (!girou)
            {   xc = ((p1x + p2x) / 2) - direction * ((p1y - p2y) / 2) * part;
                yc = ((p1y + p2y) / 2) + direction * ((p1x - p2x) / 2) * part;
            }
            else
            {   xc = ((p1x + p2x) / 2) + direction * ((p1y - p2y) / 2) * part;
                yc = ((p1y + p2y) / 2) - direction * ((p1x - p2x) / 2) * part;
            }

            Graphic.AddArc(isg2, p2x, p2y, (xc - p1x), (yc - p1y), "");	
        }
    }
}
