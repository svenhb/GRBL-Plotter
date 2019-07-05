/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2018-2019 Sven Hasemann contact: svenhb@web.de

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
 * 2019-06-10 define "<PD" as xmlMarker.figureStart
 * 
*/

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
        public static double gcAngle = 0;
        public static double gcSpacing = 1;     // Percentage of default (3-on-5) line spacing to be applied. Valid values range from 0.25 to 4.00.
        public static double gcOffX = 0;
        public static double gcOffY = 0;
        public static double gcLineDistance = 1.5;
        public static double gcFontDistance = 0;

        public static bool gcPauseLine = false;
        public static bool gcPauseWord = false;
        public static bool gcPauseChar = false;
        public static bool gcConnectLetter = false;

        private static double gcLetterSpacing = 3;  //  # LetterSpacing:     3
        private static double gcWordSpacing = 6.75; //  # WordSpacing:       6.75

        private static double offsetX = 0;
        private static double offsetY = 0;
        private static bool gcodePenIsUp = false;
        private static bool useLFF = false;
        private static bool isSameWord = false;

        private static int pathCount = 0;

        public static string[] fontFileName()
        {   if (Directory.Exists("fonts"))
                return Directory.GetFiles("fonts");
            return new string[0];
        }
        public static void reset()
        {
            gcFontName = "standard"; gcText = ""; gcFont = 0; gcAttachPoint = 7;
            gcHeight = 0; gcWidth = 0; gcAngle = 0; gcSpacing = 1; gcOffX = 0; gcOffY = 0;
            gcPauseLine = false; gcPauseWord = false; gcPauseChar = false; gcodePenIsUp = false;
            useLFF = false; gcLineDistance = 1.5; gcFontDistance = 0;
            pathCount = 0;
        }

        public static bool getCode(StringBuilder gcodeString)   
        {
            double scale = gcHeight / 21;
            string tmp1 = gcText.Replace('\r', '|');
            gcodeString.AppendFormat("( Text: {0} )\r\n", tmp1.Replace('\n', ' '));
            string[] fileContent=new string[] { "" };

            string fileName = "";
            if (gcFontName.IndexOf(@"fonts\") >= 0)
                fileName = gcFontName;
            else
                fileName = @"fonts\" + gcFontName + ".lff";
            bool fontFound = false;
            if (gcFontName != "")
            {   if (File.Exists(fileName))
                {   fileContent = File.ReadAllLines(fileName);
                    scale = gcHeight / 9;
                    useLFF = true;
                    offsetY = 0;
                    gcLineDistance = 1.667 * gcSpacing;
                    fontFound = true;
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
                else
                {   gcodeString.AppendFormat("( Font '{0}' not found )\r\n", gcFontName);
                    gcodeString.Append("( Using alternative font )\r\n");
                }
            }

            if (Properties.Settings.Default.importGCTool)
            {
                toolProp tmpTool = toolTable.getToolProperties((int)Properties.Settings.Default.importGCToolDefNr);
                gcode.Tool(gcodeString, tmpTool.toolnr, tmpTool.name);
            }
            if ((gcAttachPoint == 2) || (gcAttachPoint == 5) || (gcAttachPoint == 8))
                gcOffX -= gcWidth / 2;
            if ((gcAttachPoint == 3) || (gcAttachPoint == 6) || (gcAttachPoint == 9))
                gcOffX -= gcWidth;
            if ((gcAttachPoint == 4) || (gcAttachPoint == 5) || (gcAttachPoint == 6))
                gcOffY -= gcHeight / 2;
            if ((gcAttachPoint == 1) || (gcAttachPoint == 2) || (gcAttachPoint == 3))
                gcOffY -= gcHeight;

            string[] lines;
            if (gcText.IndexOf("\\P") >= 0)
            {   gcText = gcText.Replace("\\P", "\n"); }
            lines = gcText.Split('\n');
            offsetX = 0;
            offsetY = 9 * scale + ((double)lines.Length - 1) * gcHeight * gcLineDistance;// (double)nUDFontLine.Value;
            if (useLFF)
                offsetY = ((double)lines.Length - 1) * gcHeight * gcLineDistance;// (double)nUDFontLine.Value;

            isSameWord = false;
            for (int txtIndex = 0; txtIndex < gcText.Length; txtIndex++)
            {
                gcodePenUp(gcodeString);
                int chrIndex = (int)gcText[txtIndex] - 32;
                int chrIndexLFF = (int)gcText[txtIndex];

                if (gcText[txtIndex] == '\n')                   // next line
                {
                    offsetX = 0;
                    offsetY -= gcHeight * gcLineDistance;
                    isSameWord = false;
                    if (gcPauseLine)
                    {
                        gcode.Pause(gcodeString, "Pause before line");
                    }
                }
                else if (useLFF)
                {
                    if (chrIndexLFF > 32)
                    {   gcode.Comment(gcodeString, xmlMarker.figureStart + (++pathCount) + ">");
                        gcode.Comment(gcodeString, string.Format("Char: {0}", gcText[txtIndex]));
                    }
                    drawLetterLFF(gcodeString, ref fileContent, chrIndexLFF, scale);//, string.Format("Char: {0}", gcText[txtIndex])); // regular char
                    gcodePenUp(gcodeString);
                    if (chrIndexLFF > 32)
                        gcode.Comment(gcodeString, xmlMarker.figureEnd + (pathCount) + ">");
                }
                else
                {
                    if ((chrIndex < 0) || (chrIndex > 95))     // no valid char
                    {
                        offsetX += 2 * gcSpacing;                   // apply space
                        isSameWord = false;
                        if (gcPauseWord)
                        {
                            gcode.Pause(gcodeString, "Pause before word");
                        }
                    }
                    else
                    {
                        //gcodeString.AppendFormat("( Char: {0})\r\n", gcText[txtIndex]);                        
                        if (gcPauseChar)
                        {
                            gcode.Pause(gcodeString, "Pause before char");
                        }
                        if (gcPauseChar && (gcText[txtIndex] == ' '))
                        {
                            gcode.Pause(gcodeString, "Pause before word");
                        }
                        drawLetter(gcodeString, hersheyFonts[gcFontName][chrIndex], scale, string.Format("Char: {0}", gcText[txtIndex])); // regular char
                    }
                }
            }
            if (!useLFF)
            {   gcode.PenUp(gcodeString);
                gcode.Comment(gcodeString, xmlMarker.figureEnd + pathCount + ">");
            }
            return fontFound;
        }

        // http://forum.librecad.org/Some-questions-about-the-LFF-fonts-td5715159.html
        private static double drawLetterLFF(StringBuilder gcodeString, ref string[] txtFont, int index, double scale, bool isCopy=false)
        {
            int lineIndex = 0;
            double maxX = 0;
            char[] charsToTrim = { '#' };

            if (index <= 32)
            {   offsetX += gcWordSpacing * scale;
                if (gcPauseWord)
                {   gcode.Pause(gcodeString, "Pause before word");
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
                                gcode.Pause(gcodeString, "Pause before char");

                            int pathIndex;
                            for (pathIndex = lineIndex + 1; pathIndex < txtFont.Length; pathIndex++)
                            {
                                if (txtFont[pathIndex].Length < 2)
                                    break;
                                if (txtFont[pathIndex][0] == 'C')       // copy other char first
                                {
                                    int copyIndex = Convert.ToInt16(txtFont[pathIndex].Substring(1, 4), 16);
                                    maxX = drawLetterLFF(gcodeString, ref txtFont, copyIndex, scale, true);
                                }
                                else
                                    maxX = Math.Max(maxX, drawTokenLFF(gcodeString, txtFont[pathIndex], offsetX + gcOffX, offsetY + gcOffY, scale));
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

        private static double drawTokenLFF(StringBuilder gcodeString, string txtPath, double offX, double offY, double scale)
        {   string[] points = txtPath.Split(';');
            int cnt = 0;
            double xx,yy,x,y,xOld=0,yOld=0,bulge=0,maxX = 0;
            foreach (string point in points)
            {   string[] scoord = point.Split(',');
                charX = double.Parse(scoord[0], CultureInfo.InvariantCulture.NumberFormat);// Convert.ToDouble(scoord[0]);
                xx = charX * scale ;
                maxX = Math.Max(maxX, xx);
                xx += offX;
                charY = double.Parse(scoord[1], CultureInfo.InvariantCulture.NumberFormat); // Convert.ToDouble(scoord[1]);
                yy = charY * scale + offY;
                if (gcAngle == 0)
                { x = xx; y = yy; }
                else
                {
                    x = xx * Math.Cos(gcAngle * Math.PI / 180) - yy * Math.Sin(gcAngle * Math.PI / 180);
                    y = xx * Math.Sin(gcAngle * Math.PI / 180) + yy * Math.Cos(gcAngle * Math.PI / 180);
                }

                if (scoord.Length > 2)
                {   if (scoord[2].IndexOf('A')>=0)
                        bulge = double.Parse(scoord[2].Substring(1), CultureInfo.InvariantCulture.NumberFormat); //Convert.ToDouble(scoord[2].Substring(1)) ;
                    //AddRoundCorner(gcodeString, bulge, xOld + offX, yOld + offY, x + offX, y + offY);
                    AddRoundCorner(gcodeString, bulge, xOld, yOld, x, y);
                }
                else if (cnt == 0)
                {   if ((charX != charXOld) || (charY != charYOld))
                    {
                        gcodePenUp(gcodeString);
                        gcodeMove(gcodeString, 0, (float)x, (float)y);
                        gcodePenDown(gcodeString);
                    }
                }
                else
                    gcodeMove(gcodeString, 1, (float)x, (float)y);
                //gcodeMove(gcodeString, 1, (float)(x + offX), (float)(y + offY));
                cnt++;
                xOld = x; yOld = y;
                charXOld = charX; charYOld = charY;
            }
            return maxX;
        }

        // break down path of a single char into pieces
        private static void drawLetter(StringBuilder gcodeString, string svgtxt, double scale, string comment)
        {
            string separators = @"(?=[A-Za-z])";
            var tokens = Regex.Split(svgtxt, separators).Where(t => !string.IsNullOrEmpty(t));
            string[] svgsplit = svgtxt.Split(' ');
            double tmpX = 0;
            tmpX = offsetX - double.Parse(svgsplit[0], NumberFormatInfo.InvariantInfo) * scale;
            int token_cnt = 0;
            foreach (string token in tokens)
            {   drawToken(gcodeString, token, tmpX + gcOffX, offsetY + gcOffY, (float)scale, token_cnt++, comment);
            }
            offsetX = tmpX + double.Parse(svgsplit[1], NumberFormatInfo.InvariantInfo) * scale + gcFontDistance; //double.Parse(svgsplit[1]) * scale + gcFontDistance;
            isSameWord = true;
        }

        // draw a piece of the letter path: M x,y  or L x,y
        private static void drawToken(StringBuilder gcodeString, string svgPath, double offX, double offY, float scale, int tnr, string comment)
        {
            var cmd = svgPath.Take(1).Single();
            string remainingargs = svgPath.Substring(1);
            string argSeparators = @"[\s,]|(?=-)";
            var splitArgs = Regex
                .Split(remainingargs, argSeparators)
                .Where(t => !string.IsNullOrEmpty(t));

            double[] floatArgs = splitArgs.Select(arg => double.Parse(arg, NumberFormatInfo.InvariantInfo) * scale).ToArray();
            if (tnr == 1)
            {
                if (pathCount > 0)
                    gcode.Comment(gcodeString, xmlMarker.figureEnd + pathCount + ">");
                gcode.Comment(gcodeString, xmlMarker.figureStart + (++pathCount) + ">");
                gcode.Comment(gcodeString, comment);
            }
            if (cmd == 'M')
            {   if (gcConnectLetter && isSameWord && (tnr == 1))
                {   for (int i = 0; i < floatArgs.Length; i += 2)
                    { gcodeMove(gcodeString, 1, (float)(offX + floatArgs[i]), (float)(offY - floatArgs[i + 1])); }
                }
                else
                {   gcodePenUp(gcodeString);
                    for (int i = 0; i < floatArgs.Length; i += 2)
                    { gcodeMove(gcodeString, 0, (float)(offX + floatArgs[i]), (float)(offY - floatArgs[i + 1])); }
                    gcodePenDown(gcodeString);
                }
            }
            if (cmd == 'L')
            {   for (int i = 0; i < floatArgs.Length; i += 2)
                {   gcodeMove(gcodeString, 1, (float)(offX + floatArgs[i]), (float)(offY - floatArgs[i + 1])); }
            }
        }

        private static void gcodeMove(StringBuilder gcodeString, int gnr, float x, float y, string cmd = "")
        {
            if (gnr == 0)
                gcode.MoveToRapid(gcodeString, x, y, cmd);
            else
                gcode.MoveTo(gcodeString, x, y, cmd);
        }

        private static void gcodePenDown(StringBuilder gcodeString)
        {
            gcode.PenDown(gcodeString, "");
            gcodePenIsUp = false;
        }
        private static void gcodePenUp(StringBuilder gcodeString)
        {
            if (!gcodePenIsUp)
                gcode.PenUp(gcodeString, "");
            gcodePenIsUp = true;
        }

        private static void AddRoundCorner(StringBuilder gcodeString, double bulge, double p1x, double p1y, double p2x, double p2y)
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
            if (bulge < 0)
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

            string cmt = "";
//            if (dxfComments) { cmt = "Bulge " + bulge.ToString(); }
            if (bulge > 0)
                gcode.Arc(gcodeString, 3, (float)p2x, (float)p2y, (float)(xc - p1x), (float)(yc - p1y), cmt);
            else
                gcode.Arc(gcodeString, 2, (float)p2x, (float)p2y, (float)(xc - p1x), (float)(yc - p1y), cmt);
        }


    }
}
