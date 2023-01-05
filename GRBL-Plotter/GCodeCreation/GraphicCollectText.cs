/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2023 Sven Hasemann contact: svenhb@web.de

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
 * 2022-12-29 new function to add text from windows font
*/

using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace GrblPlotter
{
    public static partial class Graphic
    {

        private static float fontSize = 16f;
        private static System.Drawing.FontFamily fontFamily = new System.Drawing.FontFamily("Arial");
        private static System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
        private static bool fontPauseChar = false;

        public static void SetFont(Font font, bool pause = false)
        {
            fontSize = font.Size;
            fontFamily = font.FontFamily;
            fontStyle = font.Style;
            fontPauseChar = pause;
        }
        public static void AddText(string text, StringAlignment alignment)
        {
            float ox = 0;
            float oy = 0;
            float yOffset = 0;
            float rotation = 0;

            RectangleF box = GetTextBounds(text, alignment);
            List<short> pathsPerChar = new List<short>();

            if (true)    /* do whole conversion in one - disadvantage: each path gernerates a figure = 2 figures for '0', 'A', 'P'... */
            {
                //    ox = -box.Width;//-GetGlyphProperty(text[0], 0) * fontSize;  // get LSB
               // oy = -box.Height;
                using (var path = new GraphicsPath())                   // do whole text in one go
                {
                    DrawGlyphPath(path, new PointF(ox, oy), new PointF(ox, oy + yOffset), rotation, text, alignment);
                    PointF pathOffset = GetPathOffset(path);
                    GetPathsPerChar(pathsPerChar, text);
                    ExtractGlyphPath(path, pathOffset, text, pathsPerChar);     // StartPath & Graphic.StopPath
                }
            }
            else      /* alternative function converts char by char - all char related paths in one figure. But kerning is not applied */
            {
                yOffset = box.Height;

                RectangleF lineBox;

                float xOffsetBounds, xOffsetAlignment;

                float width = 0;
                float tmpWidth;

                string[] lines = text.Split('\n');
                string textline;
                foreach (string tl in lines)
                {
                    textline = tl.Replace('\r', ' ');

                    List<float> posX = new List<float>
                    {
                        0f                                      // first glyph starts at zero
                    };

                    lineBox = GetTextBounds(textline, alignment);
                    xOffsetAlignment = 0;
                    if (alignment == StringAlignment.Center)
                        xOffsetAlignment = (box.Width - lineBox.Width) / 2;
                    else if (alignment == StringAlignment.Far)
                        xOffsetAlignment = box.Width - lineBox.Width;

                    xOffsetBounds = 0;  // GetGlyphProperty(textline[0], 0) * fontSize;  // get LSB

                    width = 0;
                    for (int i = 0; i < textline.Length; i++)   // get individual xOffset for each char of this line
                    {
                        tmpWidth = GetGlyphProperty(textline[i], 1) * fontSize; // AdvanceWidths
                        width += tmpWidth;
                        posX.Add(width);
                    }
                    posX.Add(width);

                    using (var path = new GraphicsPath())       // do char by char to adjust e.g. dx individual
                    {
                        int ci;
                        for (int i = 0; i < textline.Length; i++)
                        {
                            ox = posX[i] + xOffsetAlignment - xOffsetBounds;
                            oy = -yOffset;

                            DrawGlyphPath(path, new PointF(ox, oy), new PointF(ox, oy + yOffset), 0, textline[i].ToString(), StringAlignment.Near);
                            ExtractGlyphPath(path, new PointF(0, 0), textline.Substring(i, 1), pathsPerChar, true);            // StartPath & Graphic.StopPath
                        }
                    }

                    yOffset -= box.Height / lines.Count();
                }
            }

        }

        private static void DrawGlyphPath(GraphicsPath myPath, PointF origin, PointF originR, float angle, string text, StringAlignment alignment)
        {
            var format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);
            format.Alignment |= alignment;

            myPath.Reset();
            myPath.AddString(text, fontFamily, (int)fontStyle, fontSize,
                             origin, format);

            if (angle != 0)
            {
                Matrix rotation_matrix = new Matrix();
                rotation_matrix.RotateAt(angle, originR);
                myPath.Transform(rotation_matrix);
            }

            Matrix flip_matrix = new Matrix();
            flip_matrix.Scale(1, -1);
            myPath.Transform(flip_matrix);
        }

        private static PointF GetPathOffset(GraphicsPath myPath)
        {
            if (!(myPath.PointCount > 2))
                return new PointF();

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            PointF[] tmpP = myPath.PathPoints;
            foreach (PointF pt in tmpP)
            {
                minX = Math.Min(minX, pt.X);
                minY = Math.Min(minY, pt.Y);
            }
            return new PointF(-minX, -minY);
        }

        private static void GetPathsPerChar(List<short> list, string text)
        {
            var format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);

            GraphicsPath myPath = new GraphicsPath();
            list.Clear();
            short pathCount;

            for (int i = 0; i < text.Length; i++)
            {
                myPath.Reset();
                myPath.AddString(text.Substring(i, 1), fontFamily, (int)fontStyle, fontSize,
                                 new PointF(), format);

                pathCount = 0;
                myPath.Flatten(new Matrix(), 0.02f);

                if ((myPath != null) && (myPath.PointCount > 0))
                {
                    byte[] types = myPath.PathTypes;
                    if (types.Length > 0)
                    {
                        foreach (byte type in types)
                        {
                            if ((type & 0x03) == 0) { pathCount++; }
                        }
                    }
                }
                list.Add(pathCount);
            }
        }

        private static void ExtractGlyphPath(GraphicsPath extractPath, PointF offset, string text, List<short> pathsPerChar, bool onlyOneFigure = false)
        {
            if (!(extractPath.PointCount > 2))
                return;

            extractPath.Flatten(new Matrix(), 0.02f);
            float offsetX = 0;
            float offsetY = 0;

            extractPath.Flatten(new Matrix(), 0.02f);
            if (logEnable) Logger.Info("ExtractGlyphPath  '{0}'  count:{1}", text, extractPath.PointCount);
            System.Drawing.PointF gpc;
            System.Drawing.PointF gpcStart = new System.Drawing.PointF();

            PointF[] tmpP = extractPath.PathPoints;
            byte[] types = extractPath.PathTypes;
            bool setGeometry = true;
            byte type;
            int charIndex = 0;
            char c = ' ';

            int pathIndex;
            pathIndex = pathsPerChar[charIndex];
            if (pathIndex > 0)
                SetGeometry(string.Format("Path '{0}'", text[charIndex]));
            charIndex++;

            for (int gpi = 0; gpi < tmpP.Length; gpi++)
            {
                gpc = tmpP[gpi];
                type = types[gpi];

                if ((type & 0x03) == 0)
                {
                    if (fontPauseChar) { OptionInsertPause(); }

                    if (pathIndex == 0)     /* Start new figure*/
                    {
                        do
                        {
                            if (charIndex < pathsPerChar.Count)
                                pathIndex = pathsPerChar[charIndex];
                            else
                                pathIndex = 1;

                            if (charIndex < text.Length)
                                c = text[charIndex];

                            charIndex++;
                        } while (pathIndex <= 0);

                        SetGeometry(string.Format("Path '{0}'", c));
                    }
                    StartPath(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY);
                    gpcStart = gpc;
                    if (onlyOneFigure) { setGeometry = false; }

                    pathIndex--;
                }
                else if ((type & 0x03) == 1)
                {
                    AddLine(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY);
                }

                else if ((type & 0x03) == 3)             // cubic Bézier-Spline
                {
                    AddLine(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY);
                }

                if ((type & 0x80) > 0)                  // https://docs.microsoft.com/de-de/dotnet/api/system.drawing.drawing2d.graphicspath.pathtypes?view=netframework-4.0
                {
                    AddLine(gpc.X + offset.X + offsetX, gpc.Y + offset.Y + offsetY);
                    AddLine(gpcStart.X + offset.X + offsetX, gpcStart.Y + offset.Y + offsetY);
                    StopPath("Char");
                }
            }
            StopPath("Char");
        }

        public static RectangleF GetTextBounds(string c, StringAlignment alignment)
        {
            if (string.IsNullOrEmpty(c))
                return new RectangleF();

            try
            {
                using (var path = new GraphicsPath())
                using (var format = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces))
                {
                    format.Alignment |= alignment;

                    path.AddString(c, fontFamily, (int)fontStyle, fontSize,
                                   new PointF(), format);
                    RectangleF textBounds = path.GetBounds();
                    return textBounds;
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "GetTextBounds ");
                return new RectangleF();
            }
        }

        private static float GetGlyphProperty(ushort c, int property)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.glyphtypeface?redirectedfrom=MSDN&view=netframework-4.0#remarks
            System.Windows.Media.FontFamily fontFamily1 = new System.Windows.Media.FontFamily(fontFamily.Name);

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

    }
}