/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2018-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2024-12-12 New routines to draw pixels dot by dot
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class GCodeFromImage
    {
        /// <summary>
        /// process color map and generate gcode
        /// </summary>
        private static double pixelArtDotWidth = 1;
        private static double pixelArtGapWidth = 0.1;
        private static double pixelArtDistance = 1.1;
        private static double pixelArtPenRadius = 0.2;
        private static bool pixelArtFill = false;
        private void ConvertColorMapPixelArt()
        {
            pixelArtDotWidth = (double)NuDPixelArtDotSize.Value;
            pixelArtGapWidth = (double)NuDPixelArtGapSize.Value;
            pixelArtDistance = pixelArtDotWidth + pixelArtGapWidth;
            pixelArtPenRadius = (double)NuDPixelArtShapePenDiameter.Value / 2;
            pixelArtFill = CbPixelArtShapeFill.Checked;

            Logger.Info("▼▼  ConvertColorMapPixelArt  px-dist:{0}  colorMapCount:{1}", pixelArtDistance, colorMap.Count);
            Gcode.Comment(finalString, string.Format("{0} Width=\"{1:0.0}\" />", XmlMarker.PixelArt, pixelArtDotWidth));
            int toolNr, skipTooNr = 1;
            short key;
            PointF tmpP;
            int pathCount = 0;

            double gCodeWidth = pixelArtDotWidth / 2;
            if (RbPixelArtShape.Checked)
                gCodeWidth = pixelArtPenRadius * 2;

            for (int index = 0; index < toolTableCount; index++)  	// go through lists
            {
                ToolTable.SetIndex(index);                      	// set index in class
                key = (short)ToolTable.IndexToolNR();               // if tool-nr == known key go on
                                                                    //    if (ToolTable.IndexSelected())
                if (logEnable) Logger.Trace("ConvertColorMapPixelArt  index:{0}  key:{1}   IndexSelected:{2}  colorMap.ContainsKey:{3}  color:{4}", index, key, ToolTable.IndexSelected(), colorMap.ContainsKey(key), ToolTable.IndexColor());

                if (colorMap.ContainsKey(key) && ToolTable.IndexSelected())
                {
                    toolNr = key;                               // use tool in palette order
                    if (cbSkipToolOrder.Checked)
                        toolNr = skipTooNr++;                   // or start tool-nr at 1

                    finalString.AppendLine("\r\n( +++++ Tool change +++++ )");

                    Gcode.Tool(finalString, toolNr, 0, ToolTable.IndexName() + " [" + ColorTranslator.ToHtml(ToolTable.IndexColor()) + "]");  // + svgPalette.pixelCount());

                    Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" PenColor=\"{2}\" PenName=\"{3}\" PenWidth=\"{4:0.00}\">", XmlMarker.GroupStart, (++pathCount), ColorTranslator.ToHtml(ToolTable.IndexColor()).Substring(1), ToolTable.IndexName(), gCodeWidth));		//toolTable.indexName()));
                    Gcode.ReduceGCode = false;
                    Gcode.PenUp(finalString, " start ");
                    Gcode.ReduceGCode = true;

                    int factor = resoFactorX;
                    int start = 0;// factor / 2;

                    Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" Geometry=\"Pixel\" PenColor=\"{2}\" PenWidth=\"{3:0.00}\">", XmlMarker.FigureStart, pathCount, ColorTranslator.ToHtml(ToolTable.IndexColor()).Substring(1), gCodeWidth));
                    Logger.Trace("adaptRadius = pixelArtDotWidth / 2 - pixelArtPenRadius; {0}  {1}", pixelArtDotWidth, pixelArtPenRadius);

                    for (int y = start; y < resultImage.Height; y += factor)  // go through all lines
                    {
                        while (colorMap[key][y].Count > 1)          // start at line 0 and check line by line
                            DrawColorMapPixelArt(key, y, 0);
                    }
                    Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.FigureEnd));
                    Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.GroupEnd));
                }
            }
        }

        /// <summary>
        /// process color map and generate gcode
        /// </summary>
        private void ConvertColorMapBWPixelArt()
        {
            pixelArtDotWidth = (double)NuDPixelArtDotSize.Value;
            pixelArtGapWidth = (double)NuDPixelArtGapSize.Value;
            pixelArtDistance = pixelArtDotWidth + pixelArtGapWidth;
            pixelArtPenRadius = (double)NuDPixelArtShapePenDiameter.Value / 2;
            pixelArtFill = CbPixelArtShapeFill.Checked;

            Logger.Info("▼▼  ConvertColorMapBWPixelArt reso:{0}", pixelArtDistance);
            Gcode.Comment(finalString, string.Format("{0} Width=\"{1:0.0}\" />", XmlMarker.PixelArt, pixelArtDotWidth));
            short key;
            PointF tmpP;
            int pathCount = 0;
            SortByGrayValue(false);
            short grayVal;

            for (int index = 0; index <= 255; index++)  // go through possible gray values
            {
                key = (short)index;           // if tool-nr == known key go on
                if (GrayValueMap[index].Use)
                    if (logEnable) Logger.Trace("ConvertColorMapBWPixelArt  index:{0}  key:{1}   use:{2}  colorMap.ContainsKey:{3}", index, key, GrayValueMap[index].Use, colorMap.ContainsKey(key));

                if (colorMap.ContainsKey(key) && (GrayValueMap[index].Use))
                {
                    finalString.AppendLine("\r\n( +++++ Tool change +++++ )");

                    Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" PenColor=\"{2}\" PenName=\"{3}\" PenWidth=\"{4:0.00}\">", XmlMarker.GroupStart, (++pathCount), ColorTranslator.ToHtml(Color.FromArgb(key, key, key)).Substring(1), key, pixelArtDistance));     //toolTable.indexName()));
                    Gcode.ReduceGCode = false;
                    Gcode.PenUp(finalString, " start ");
                    Gcode.ReduceGCode = true;

                    int factor = resoFactorX;
                    int start = factor / 2;

                    Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" Geometry=\"Pixel\" PenColor=\"{2}\" PenWidth=\"{3:0.00}\">", XmlMarker.FigureStart, pathCount, ColorTranslator.ToHtml(Color.FromArgb(key, key, key)).Substring(1), pixelArtDistance));

                    for (int y = start; y < resultImage.Height; y += factor)  // go through all lines
                    {
                        while (colorMap[key][y].Count > 1)          // start at line 0 and check line by line
                            DrawColorMapPixelArt(key, y, 0);
                    }
                    Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.FigureEnd));
                    Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.GroupEnd));
                }
            }
        }



        /// <summary>
        /// check recursive line by line for same color near by, by given x-value
        /// </summary>
        private void DrawColorMapPixelArt(int toolNr, int line, int startIndex)
        {
            int start, stop, newIndex;
            int factor = resoFactorX;
            bool useZnotS = RbStartGrayZ.Checked;
            bool drawShape = RbPixelArtShape.Checked;
            double coordY = pixelArtDistance * (double)line + pixelArtDistance / 2;
            int drawType = 0;
            if (RbPixelArtDrawShapeRect.Checked)
                drawType = 1;

            if (colorMap[toolNr][line].Count > 1)   // at least two numbers needed: start, stop
            {
                if ((startIndex % 2 == 0) && ((startIndex + 1) < colorMap[toolNr][line].Count)) // get start,stop pair
                {   // if startIndex is even, then start = start of gcode
                    start = colorMap[toolNr][line][startIndex];     // inital start of color
                    stop = colorMap[toolNr][line][startIndex + 1];  // inital stop of color
                    colorMap[toolNr][line].RemoveRange(startIndex, 2);  // remove pair from list - needs to be drawn just once
                }
                else
                {   // if its odd, get stop-pos first, but this is start for gcode
                    start = colorMap[toolNr][line][startIndex];     // inital stop of color
                    stop = colorMap[toolNr][line][startIndex - 1];    // inital start of color
                    colorMap[toolNr][line].RemoveRange(startIndex - 1, 2);  // remove pair from list - needs to be drawn just once
                }

                if ((start >= 0) && (stop >= 0))
                {
                    double coordX;

                    int pdir = 1;
                    if (stop < start) pdir = -1;
                    for (int px = start; px != stop; px += pdir)
                    {
                        coordX = pixelArtDistance * (double)px + pixelArtDistance / 2;
                        //    Gcode.MoveToRapid(finalString, coordX, coordY, "pixel");     // move to start pos
                        if (drawShape)
                            SetShapeColorMap(finalString, drawType, coordX, coordY);
                        else
                        {
                            Gcode.MoveToRapid(finalString, coordX, coordY, "pixel");     // move to start pos
                            SetPixelColorMap(finalString, useZnotS);
                        }
                    }
                    coordX = pixelArtDistance * stop + pixelArtDistance / 2;
                    //    Gcode.MoveToRapid(finalString, coordX, coordY, "pixel");     // move to start pos
                    if (drawShape)
                        SetShapeColorMap(finalString, drawType, coordX, coordY);
                    else
                    {
                        Gcode.MoveToRapid(finalString, coordX, coordY, "pixel");     // move to start pos
                        SetPixelColorMap(finalString, useZnotS);
                    }
                }
                if (line < (resultImage.Height - factor - 1))
                {
                    var nextLine = colorMap[toolNr][line + factor];      // check for start-pos nearby in next line

                    int sfactor = 0;// (int)(factor * 1.5);

                    if (start < stop)                                   // comming from left, search from rigth to left
                    {
                        for (int k = stop + sfactor; k >= start - sfactor; k--)
                        {
                            newIndex = nextLine.IndexOf(k);             // first check direction were I came from
                            if (newIndex >= 0)                          // entry found
                            {
                                DrawColorMapPixelArt(toolNr, line + factor, newIndex);  // go on with next line
                                                                                        //   end = false;
                                break;
                            }
                        }
                    }
                    else
                    {                                                   // comming from right, search from left to right
                        for (int k = stop - sfactor; k <= start + sfactor; k++)
                        {
                            newIndex = nextLine.IndexOf(k);             // first check direction were I came from
                            if (newIndex >= 0)                          // entry found
                            {
                                DrawColorMapPixelArt(toolNr, line + factor, newIndex);  // go on with next line
                                                                                        //     end = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void SetShapeColorMap(StringBuilder finalString, int drawType, double centerX, double centerY, bool upDown = true)
        {
            double adaptRadius = pixelArtDotWidth / 2 - pixelArtPenRadius;
            if (drawType == 0)
            {
                if (upDown)
                {
                    Gcode.MoveToRapid(finalString, centerX - adaptRadius, centerY, "");
                    Gcode.PenDown(finalString, "(PD)");
                }
                Gcode.Arc(finalString, 2, centerX - adaptRadius, centerY, adaptRadius, 0, "");

                if (pixelArtFill && (adaptRadius - pixelArtPenRadius > pixelArtPenRadius))
                {
                    for (double i = adaptRadius - pixelArtPenRadius; i > 0; i -= pixelArtPenRadius)
                    {
                        Gcode.MoveTo(finalString, centerX - i, centerY, "");
                        Gcode.Arc(finalString, 2, centerX - i, centerY, i, 0, "");
                    }
                }
                if (upDown) Gcode.PenUp(finalString, "(PU)");
            }
            else
            {
                if (upDown)
                {
                    Gcode.MoveToRapid(finalString, centerX - adaptRadius, centerY - adaptRadius, "");
                    Gcode.PenDown(finalString, "(PD)");
                }
                Gcode.MoveTo(finalString, centerX - adaptRadius, centerY + adaptRadius, "");
                Gcode.MoveTo(finalString, centerX + adaptRadius, centerY + adaptRadius, "");
                Gcode.MoveTo(finalString, centerX + adaptRadius, centerY - adaptRadius, "");
                Gcode.MoveTo(finalString, centerX - adaptRadius, centerY - adaptRadius, "");
                if (pixelArtFill && (adaptRadius - pixelArtPenRadius > pixelArtPenRadius))
                {
                    for (double i = adaptRadius - pixelArtPenRadius; i > 0; i -= pixelArtPenRadius)
                    {
                        Gcode.MoveTo(finalString, centerX - i, centerY + i, "");
                        Gcode.MoveTo(finalString, centerX + i, centerY + i, "");
                        Gcode.MoveTo(finalString, centerX + i, centerY - i, "");
                        Gcode.MoveTo(finalString, centerX - i, centerY - i, "");
                    }
                }

                if (upDown) Gcode.PenUp(finalString, "(PU)");
            }
        }

        private void SetPixelColorMap(StringBuilder finalString, bool useZnotS)
        {
            if (useZnotS)
            {
                finalString.AppendFormat("G{0} Z{1} F{2} (PD)\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(nUDZBottom.Value), Gcode.GcodeZFeed);
                finalString.AppendFormat("G{0} Z{1} (PU)\r\n", Gcode.FrmtCode(0), Gcode.FrmtNum(Gcode.GcodeZUp));
                cncCoordLastZ = cncCoordZ = (float)nUDZTop.Value;
            }
            else
            {
                finalString.AppendFormat("S{0} (PD)\r\n", Math.Round(nUDSBottom.Value));
                finalString.AppendFormat("G{0} P{1}\r\n", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyDown));
                finalString.AppendFormat("S{0} (PU DOT)\r\n", Math.Round(nUDSTop.Value));
                finalString.AppendFormat("G{0} P{1}\r\n", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyUp));
            }
        }

        private void CreatePixelPatternHeight(int width, int height, bool useZnotS)
        {
            pixelArtDotWidth = (double)NuDPixelArtDotSize.Value;
            pixelArtGapWidth = (double)NuDPixelArtGapSize.Value;
            bool drawShape = RbPixelArtShape.Checked;
            int drawType = 0;
            if (RbPixelArtDrawShapeRect.Checked)
                drawType = 1;

            Logger.Trace("CreatePixelPattern  width:{0}  heigth:{1} resultImg-w:{2}  resultImg-h:{3}  cncPixelResoX:{4}  resoDesiredX:{5}", width, height, resultImage.Width, resultImage.Height, cncPixelResoX, resoDesiredX);
            int x, y;
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    ApplyPixelValueHeight(x, y);
                }
                if (++y < height)
                {
                    for (x = width - 1; x >= 0; x--)
                    {
                        ApplyPixelValueHeight(x, y);
                    }
                }
            }

            void ApplyPixelValueHeight(int xx, int yy)
            {
                double pixelPosX = pixelArtDistance * xx;
                double pixelPosY = pixelArtDistance * yy;
                double adaptRadius = pixelArtDotWidth / 2 - pixelArtPenRadius;
                int brightnes = GetPixelBrightnes(xx, yy);          // resultImage
                if ((CbPenUpOn0.Checked) && (brightnes == 255))
                {
                    brightnes = -1;
                    return;
                }
                cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                if (drawShape)
                    finalString.AppendFormat("G{0} X{1} Y{2}\r\n", Gcode.FrmtCode(0), Gcode.FrmtNum(pixelPosX- adaptRadius), Gcode.FrmtNum(pixelPosY));
                else
                    finalString.AppendFormat("G{0} X{1} Y{2}\r\n", Gcode.FrmtCode(0), Gcode.FrmtNum(pixelPosX), Gcode.FrmtNum(pixelPosY));

                SetPixelHeight(brightnes, useZnotS, false);

                if (drawShape)
                {
                        Gcode.Arc(finalString, 2, pixelPosX - adaptRadius, pixelPosY, adaptRadius, 0, "");
                   // SetShapeColorMap(finalString, drawType, pixelPosX, pixelPosX, false);
                }

                if (useZnotS)
                {
                    finalString.AppendFormat("G{0} Z{1} (PU)\r\n", Gcode.FrmtCode(0), Gcode.FrmtNum(nUDZTop.Value), Gcode.GcodeXYFeed);
                    cncCoordLastZ = cncCoordZ = (float)nUDZTop.Value;
                }
                else
                {
                    finalString.AppendFormat("G{0} P{1} (PD)\r\n", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyDown));
                    finalString.AppendFormat("S{0} (PU DOT)\r\n", Math.Round(nUDSTop.Value));
                    finalString.AppendFormat("G{0} P{1}\r\n", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyUp));
                }
                cncCoordLastZ = cncCoordZ = (float)nUDZTop.Value;

            }
        }

        private void SetPixelHeight(int bright, bool useZnotS, bool relative)
        {
            if (bright > 255) { bright = 255; }
            if (bright < 0) { bright = 0; }
            int height = 255 - bright;    // pixelVal 0=black, 255=white
            string SZ;

            if (useZnotS)
            {
                cncCoordZ = (float)nUDZTop.Value - height * (float)(nUDZTop.Value - nUDZBottom.Value) / 255;    // calc Z value
                if (relative) { SZ = string.Format("Z{0}", Gcode.FrmtNum(cncCoordZ - cncCoordLastZ)); }
                else { SZ = string.Format("Z{0}", Gcode.FrmtNum(cncCoordZ)); }
                finalString.AppendFormat("G{0} {1} F{2} (PD)\r\n", Gcode.FrmtCode(1), SZ, Gcode.GcodeZFeed);
            }
            else
            {
                int sVal = (int)Math.Abs((float)nUDSTop.Value + (((255 - bright) * (float)(nUDSBottom.Value - nUDSTop.Value)) / 255));
                SZ = string.Format("S{0}", sVal);
                finalString.AppendFormat("{0} (PD)\r\n", SZ);
            }
        }

        private void BtnPixelArtCalcSize_Click(object sender, EventArgs e)
        {
            UpdateSizeControls();
            ApplyColorCorrections("BtnPixelArtCalcSize_Click");
        }

        private void UpdateSizeControls()
        {
            decimal penWidth = NuDPixelArtDotSize.Value;
            decimal gapWidth = NuDPixelArtGapSize.Value;
            decimal w = NuDPixelArtDotsPerPixel.Value * (penWidth + gapWidth) * originalImage.Width;
            decimal h = NuDPixelArtDotsPerPixel.Value * (penWidth + gapWidth) * originalImage.Height;
            LbLSizeXCode.Text = w.ToString(); 
            LbLSizeYCode.Text = h.ToString();
        }

    }

    class PixelBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            base.OnPaint(pe);
        }
    }
}
