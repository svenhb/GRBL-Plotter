/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2018-2025 Sven Hasemann contact: svenhb@web.de

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
 * 2021-04-17 new
 * 2021-07-02 code clean up / code quality
 * 2022-01-02 update
 * 2024-01-17 laser engraving useLaser
 * 2024-12-11 update
 * 2025-04-02 change PixelArt attribute from 'Width' to 'Source' = axis-name or 'S' value
*/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace GrblPlotter
{
    public partial class GCodeFromImage
    {

        private static float cncCoordZ;
        private static float cncCoordLastZ;

        private void GenerateHeightData(bool applyPixelArt)
        {
            Logger.Debug("generateHeightData horizontal:{0}  useZ:{1}", rbEngravingPattern1.Checked, RbStartGrayZ.Checked);
            if (adjustedImage == null) return;                //if no image, do nothing

            cncPixelResoX = (float)nUDResoX.Value;           // resolution = distance between pixels / lines / columns
            cncPixelResoY = (float)nUDResoY.Value;
            double resoRatioYX = cncPixelResoY / cncPixelResoX;
            double penWidth = cncPixelResoY;

            resultImage = new Bitmap(adjustedImage);        // adjustedImage is shown in preview, resultImage will be scanned

            if (rbEngravingPattern1.Checked)        // horizontal no change
            { }
            else if (rbEngravingPattern2.Checked)                // if diagonal, take account of 45° angle
            {
                int factor = (int)Math.Round(resoRatioYX) + 1;
                //          resoRatioYX = factor;
                cncPixelResoX = cncPixelResoY * 1.414f / factor;
                cncPixelResoY = cncPixelResoX;// cncPixelResoY * 1.414f; 
            }
            else
            { cncPixelResoY = cncPixelResoX; }

            int newWidth = (int)((float)nUDWidth.Value / cncPixelResoX);
            int newHeigth = (int)((float)nUDHeight.Value / cncPixelResoY);

            if (RbPixelArt.Checked)
            {
                newWidth = originalImage.Width * (int)NuDPixelArtDotsPerPixel.Value;
                newHeigth = originalImage.Height * (int)NuDPixelArtDotsPerPixel.Value;
            }

            if ((newWidth > 0) && (newHeigth > 0))
            {
                AForge.Imaging.Filters.ResizeNearestNeighbor filterResize = new AForge.Imaging.Filters.ResizeNearestNeighbor(newWidth, newHeigth);
                resultImage = filterResize.Apply(adjustedImage);
            }
            //     int pixelCount = resultImage.Width * resultImage.Height;
            //     int pixelProcessed = 0;
            int percentDone;

            int pixelPosY;                  			// top/botom pixel
            int pixelPosX;                  			// Left/right pixel
            bool useZnotS = RbStartGrayZ.Checked;       // calculate Z-value or S-value
                                                        //       bool useLaserOnly = CbLaserOnly.Checked;
                                                        //     bool backAndForth = !cBOnlyLeftToRight.Checked;
            bool relative = cBCompress.Checked;         // true;
                                                        //     bool firstValue;//, firstLine=true;

            int pixelValLast, pixelValNow, pixelValNext;    // gray-values at pixel pos
            cncCoordLastX = cncCoordX = 0;
            cncCoordLastY = cncCoordY = 0;

            if (RbEngravingLine.Checked)        // horizontal
                GenerateGCodePreset(0, cncPixelResoY * (resultImage.Height - 1), false);    // gcode.setup, -jobStart, PenUp, -G0 to 1st pos.
            else if (rbEngravingPattern2.Checked)
                GenerateGCodePreset(0, 0, false);    // gcode.setup, -jobStart, PenUp, -G0 to 1st pos.
            else
            {
                cncCoordLastX = cncCoordX = cncPixelResoX * resultImage.Width * (float)NuDSpiralCenterX.Value;  // start pos
                cncCoordLastY = cncCoordY = cncPixelResoY * resultImage.Height * (float)NuDSpiralCenterY.Value;
                GenerateGCodePreset(cncCoordX, cncCoordY);
            }

            bool pixelArtSpecial = applyPixelArt && RbStartGraySpecial.Checked;
            string pixelArtSource = "";
            long lineCount = 0;

            if (applyPixelArt)
            {
                penWidth = (double)NuDPixelArtDotSize.Value;
            }
            if (pixelArtSpecial)
            {
                Gcode.GcodeZApply = false;
                Gcode.GcodePWMEnable = false;
                Gcode.Comment(finalString, string.Format("{0} Min=\"{1:0.000}\" Max=\"{2:0.000}\" Width=\"{3:0.000}\" />", XmlMarker.HalftoneZ, nUDSpecialTop.Value, nUDSpecialBottom.Value, penWidth));
                specialCodeValue1 = tBCodeValue1.Text;
                if (specialCodeValue1.Length > 0)
                    pixelArtSource = specialCodeValue1.Substring(specialCodeValue1.Length - 1);
            }
            else if (useZnotS)   // set 2D-View display option to translate S/Z value to pen width
            {
                Gcode.GcodeZApply = true;
                Gcode.GcodePWMEnable = false;
                Gcode.Comment(finalString, string.Format("{0} Min=\"{1:0.000}\" Max=\"{2:0.000}\" Width=\"{3:0.000}\" />", XmlMarker.HalftoneZ, nUDZTop.Value, nUDZBottom.Value, penWidth));
                pixelArtSource = "Z";
            }
            else
            {
                Gcode.GcodeZApply = false;
                Gcode.GcodePWMEnable = true;
                Gcode.Comment(finalString, string.Format("{0} Min=\"{1:0.000}\" Max=\"{2:0.000}\" Width=\"{3:0.000}\" />", XmlMarker.HalftoneS, nUDSTop.Value, nUDSBottom.Value, penWidth));
                pixelArtSource = "S";
            }
            if (applyPixelArt)
            {
                Gcode.Comment(finalString, string.Format("{0} Source=\"{1}\" />", XmlMarker.PixelArt, pixelArtSource));
                Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" PenColor=\"black\" PenWidth=\"{2:0.00}\">", XmlMarker.FigureStart, 1, 0.01));
            }
            else
                Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" PenColor=\"black\" >", XmlMarker.FigureStart, 1));
            finalString.AppendFormat("F{0}\r\n", Gcode.GcodeXYFeed);    // set feedrate

            if (applyPixelArt)
            {
                Logger.Info("Create halftone Pixel Art width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                lineCount=CreatePixelPatternHeight(resultImage.Width, resultImage.Height, useZnotS, pixelArtSpecial);
            }
            else if (RbEngravingLine.Checked)        // line by line with angle
            {
                Logger.Info("Create halftone spiral width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                scanCNCPos.Clear();
                CreateLinePattern((double)nUDResoY.Value, cncPixelResoX, (double)NudEngravingAngle.Value, resultImage.Width, resultImage.Height);
                if (CbEngravingCross.Checked)
                    CreateLinePattern((double)nUDResoY.Value, cncPixelResoX, 90 + (double)NudEngravingAngle.Value, resultImage.Width, resultImage.Height);
                // fill path with brightnes values
                CreateScanPath(resultImage.Width, resultImage.Height, 0, 0);// resultImage.Width, resultImage.Height);
                                                                            // improove path
                                                                            //    RemoveIntermediateSteps();
                ApplyScanPath(useZnotS, relative, true);
            }
            else if (rbEngravingPattern3.Checked)        // spiral
            {
                Logger.Info("Create halftone spiral width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                CreateSpiral((double)nUDResoY.Value, cncPixelResoX, Math.Sqrt(resultImage.Width * resultImage.Width + resultImage.Height * resultImage.Height));
                // fill path with brightnes values
                CreateScanPath(resultImage.Width, resultImage.Height, resultImage.Width * (double)NuDSpiralCenterX.Value, resultImage.Height * (double)NuDSpiralCenterY.Value);
                // improove path
                RemoveIntermediateSteps();
                ApplyScanPath(useZnotS, relative);
            }
            else
            {
                Logger.Info("Create halftone external path width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                CreateFrom2DView(cncPixelResoX);
                CreateScanPath(resultImage.Width, resultImage.Height, resultImage.Width * (double)NuDSpiralCenterX.Value, resultImage.Height * (double)NuDSpiralCenterY.Value);
                RemoveIntermediateSteps();
                ApplyScanPath(useZnotS, relative);
            }


            if (relative) { finalString.AppendLine("G90"); }
            Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.FigureEnd));
            Gcode.JobEnd(finalString, "EndJob");
            if (RbStartGrayS.Checked && cBLaserModeOffEnd.Checked)
            { finalString.AppendLine("$32=0 (Lasermode off)"); }

            imagegcode = "";// "( Generated by GRBL-Plotter )\r\n";
            imagegcode += Gcode.GetHeader("Image import", "", lineCount) + finalString.Replace(',', '.').ToString() + Gcode.GetFooter();
        }

        /*      private int GetPixelValue(int picX, int picY, bool useZ)
              {
                  if ((picX < 0) || (picY < 0) || (picX >= resultImage.Width) || (picY >= resultImage.Height))
                      return useZ ? 255 : 0;
                  Color myColor = resultImage.GetPixel(picX, (resultImage.Height - 1) - picY);    // Get pixel color
                  int brightness = (int)Math.Round((double)(myColor.R + myColor.G + myColor.B) / 3);        // calc height FF=white, 0=black
                  if (myColor.A < 128)        // assume transparency as white
                      return useZ ? 255 : 0;
                  if ((cbExceptColor.Checked) && (cbExceptColor.BackColor == myColor))    //added 2021-12-24
                      return useZ ? 255 : 0;
                  if (useZ)
                  { return brightness; }      // calc height FF=white, 0=black
                  else
                  { return (int)((255 - brightness) * (float)(nUDSBottom.Value - nUDSTop.Value)) / 255; }
              }*/
        private int GetPixelBrightnes(double picX, double picY)
        {
            if ((picX < 0) || (picY < 0) || (picX >= resultImage.Width) || (picY >= resultImage.Height))
                return 255;
            Color myColor = resultImage.GetPixel((int)picX, (resultImage.Height - 1) - (int)picY);    // Get pixel color
            int brightness = (int)Math.Round((double)(myColor.R + myColor.G + myColor.B) / 3);        // calc height FF=white, 0=black
            if (myColor.A < 128)        // assume transparency as white
                return 255;
            if ((cbExceptColor.Checked) && (cbExceptColor.BackColor == myColor))    //added 2021-12-24
                return 255;
            return brightness;       // calc height FF=white, 0=black
        }

        private void SetCommandFloat(double pixelPosX, double pixelPosY, int bright, bool useZnotS, bool relative)
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
                if (SetXYCommandFloat(pixelPosX, pixelPosY, SZ, useZnotS, relative))
                    cncCoordLastZ = cncCoordZ;
            }
            else
            {
                int sVal = (int)Math.Abs((float)nUDSTop.Value + (((255 - bright) * (float)(nUDSBottom.Value - nUDSTop.Value)) / 255));
                SZ = string.Format("S{0}", sVal);// Math.Round(nUDSTop.Value + sVal)); 
                SetXYCommandFloat(pixelPosX, pixelPosY, SZ, useZnotS, relative);
            }

        }
        /*       private void SetCommand(int pixelPosX, int pixelPosY, int pixelVal, bool useZnotS, bool relative)
               {
                   int height = 255 - pixelVal;    // pixelVal 0=black, 255=white
                   string SZ;
                   if (useZnotS)
                   {
                       cncCoordZ = (float)nUDZTop.Value - height * (float)(nUDZTop.Value - nUDZBottom.Value) / 255;    // calc Z value
                       if (relative) { SZ = string.Format("Z{0:0.##}", (cncCoordZ - cncCoordLastZ)); }
                       else { SZ = string.Format("Z{0}", Gcode.FrmtNum(cncCoordZ)); }
                       cncCoordLastZ = cncCoordZ;
                   }
                   else
                   { SZ = string.Format("S{0}", Math.Round(nUDSTop.Value + pixelVal)); }

                   SetXYCommand(pixelPosX, pixelPosY, SZ, relative);
               }*/
        private static bool SetXYCommandFloat(double pixelPosX, double pixelPosY, string SZ, bool useZnotS, bool relative)
        {
            cncCoordX = cncPixelResoX * pixelPosX;
            cncCoordY = cncPixelResoY * pixelPosY;
            double difX = cncCoordX - cncCoordLastX;
            double difY = cncCoordY - cncCoordLastY;
            bool wasSend = false;
            if (relative)
            {
                string x = (difX != 0) ? string.Format("X{0:0.##}", difX) : "";
                string y = (difY != 0) ? string.Format("Y{0:0.##}", difY) : "";
                if ((x != "") || (y != ""))
                {
                    string finalCommand = string.Format("{0}{1}{2}\r\n", x, y, SZ);
                    if (finalCommand.Length > 1)
                    {
                        finalString.AppendFormat("{0}", finalCommand); //finalString.AppendFormat("{0}{1}{2}\r\n", x,y,SZ);
                        wasSend = true;
                    }
                }
            }
            else
            {
                if ((difX != 0) || (difY != 0))
                {
                    finalString.AppendFormat("G{0} X{1} Y{2} {3}\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), SZ);
                    wasSend = true;
                }      // set absolute position
            }
            cncCoordLastX = cncCoordX;
            cncCoordLastY = cncCoordY;
            return wasSend;
        }
        /*    private static void SetXYCommand(int pixelPosX, int pixelPosY, string SZ, bool relative)
            {
                cncCoordX = cncPixelResoX * (float)pixelPosX;
                cncCoordY = cncPixelResoY * (float)pixelPosY;
                if (relative)
                {
                    double difX = cncCoordX - cncCoordLastX; string x = (difX != 0) ? string.Format("X{0:0.##}", difX) : "";
                    double difY = cncCoordY - cncCoordLastY; string y = (difY != 0) ? string.Format("Y{0:0.##}", difY) : "";
                    if ((x != "") || (y != ""))
                        finalString.AppendFormat("{0}{1}{2}\r\n", x, y, SZ);
                }
                else
                { finalString.AppendFormat("G{0} X{1} Y{2} {3}\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), SZ); }      // set absolute position
                cncCoordLastX = cncCoordX;
                cncCoordLastY = cncCoordY;
            }*/

        private static readonly List<ImgPoint> scanCNCPos = new List<ImgPoint>();
        private void ApplyScanPath(bool useZnotS, bool relative, bool linearMove = false)
        {
            int pixelValLast, pixelValNow, pixelValNext;
            double pixelPosX, pixelPosY;    // gray-values at pixel pos
            if (scanCNCPos.Count <= 2)
                return;

            bool isPenUp = false;
            bool noPen = !useZnotS && CbLaserOnly.Checked;
            bool doPenUpLater = false;
            Logger.Trace("applyScanPath noPen:{0}", noPen);

            cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
            pixelValLast = -1; pixelValNow = pixelValNext = scanCNCPos[0].brightnes;    // GetPixelValue(scanCNCPos[0].X, scanCNCPos[0].Y, useZnotS);

            if (relative) { finalString.AppendLine("G91G1 (relative mode)"); }
            for (int i = 1; i < scanCNCPos.Count - 1; i++)
            {
                pixelPosX = scanCNCPos[i].X;
                pixelPosY = scanCNCPos[i].Y;
                if ((pixelPosX < 0) || (pixelPosY < 0) || (scanCNCPos[i].brightnes < 0))
                {   // do pen-up
                    if (!noPen && !isPenUp)
                    {
                        if (relative)
                        {
                            doPenUpLater = true;
                        }
                        else
                        {
                            //       finalString.AppendLine("G90 (absolute mode 1)"); }  // switch to absolute
                            Gcode.PenUp(finalString, "PU");
                            cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                            //    if (relative) { finalString.AppendLine("G91G1 (relative mode 1)"); }
                        }
                    }
                    isPenUp = true;
                    continue;
                }
                else
                {
                    cncCoordX = cncPixelResoX * pixelPosX;
                    cncCoordY = cncPixelResoY * pixelPosY;
                    if (scanCNCPos[i].brightnes < 0)    // keep pen-up
                    {
                        if (!noPen && !isPenUp)
                        {
                            if (relative)
                            {
                                doPenUpLater = true;
                            }
                            else
                            {
                                //finalString.AppendLine("G90 (absolute mode 2 )"); }  // switch to absolute
                                Gcode.PenUp(finalString, "PU");
                                cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                            } //if (relative) { finalString.AppendLine("G91G1 (relative mode 2)"); }
                        }
                        isPenUp = true;
                        continue;
                    }
                    // do pen-down
                    if (isPenUp)
                    {
                        if (relative)
                        {
                            finalString.AppendLine("G90 (absolute mode 3)");
                            if (doPenUpLater)
                            {
                                Gcode.PenUp(finalString, "PU");
                                cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                            }
                        }  // switch to absolute
                        //Gcode.MoveToRapid(finalString, cncCoordX, cncCoordY,"");
                        //   finalString.AppendFormat("G{0} X{1} Y{2} F10000\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY));
                        //   if (useZnotS) finalString.AppendFormat("G{0} Z{1} (PU)\r\n", Gcode.FrmtCode(0), Gcode.FrmtNum(Gcode.GcodeZUp)); //Gcode.PenUp(finalString, "PU");
                        //   cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                        finalString.AppendFormat("G{0} X{1} Y{2}\r\n", Gcode.FrmtCode(0), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY));

                        if (useZnotS)
                        {   // move z down
                            //    finalString.AppendFormat("G{0} X{1} Y{2} Z{3} F{4} (PD)\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), Gcode.FrmtNum(nUDZTop.Value), Gcode.GcodeXYFeed);
                            finalString.AppendFormat("Z{0} F{1} (PD)\r\n", Gcode.FrmtNum(nUDZTop.Value), Gcode.GcodeXYFeed);
                            cncCoordLastZ = cncCoordZ = (float)nUDZTop.Value;
                        }
                        else
                        {   // move servo down and delay
                            //   finalString.AppendFormat("G{0} X{1} Y{2} F{3} S{4} (PD)\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), Gcode.GcodeXYFeed, Math.Round(nUDSTop.Value));
                            if (!noPen)
                            {
                                finalString.AppendFormat("F{0} S{1} (PD)\r\n", Gcode.GcodeXYFeed, Math.Round(nUDSTop.Value));
                                finalString.AppendFormat("G{0} P{1}\r\n", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyUp));
                            }
                        }
                        cncCoordLastZ = cncCoordZ = (float)nUDZTop.Value;
                        if (relative) { finalString.AppendFormat("G91G1 (relative mode 3)\r\n"); }

                        isPenUp = false;
                        cncCoordLastX = cncCoordX;
                        cncCoordLastY = cncCoordY;
                    }

                    pixelValNext = scanCNCPos[i + 1].brightnes;

                    if ((pixelValNow != pixelValLast) || (pixelValNow != pixelValNext))
                    { SetCommandFloat(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative); }
                    else
                    { if (!linearMove && useZnotS) SetXYCommandFloat(pixelPosX, pixelPosY, "", useZnotS, relative); }

                    Gcode.lastz = cncCoordZ;
                }
                pixelValLast = pixelValNow; pixelValNow = pixelValNext;
            }
        }

        private void CreateScanPath(int width, int height, double offsetX, double offsetY)
        {   //scanPixelPos.Clear();
            Logger.Trace("createScanPath width:{0:0.00} height:{1:0.00}", width, height);
            double x, y;
            int brightnes;
            for (int k = 0; k < scanCNCPos.Count; k++)
            {
                x = (scanCNCPos[k].X / cncPixelResoX + offsetX);
                y = (scanCNCPos[k].Y / cncPixelResoX + offsetY);
                if (scanCNCPos[k].brightnes == 0)
                {
                    scanCNCPos[k] = new ImgPoint(x, y, -2);
                    continue;
                }
                //if ((x != oldX) || (y != oldY))
                {
                    if ((x < 0) || (x >= width)) { x = -1; }
                    if ((y < 0) || (y >= height)) { y = -1; }
                    brightnes = GetPixelBrightnes(x, y);
                    if ((CbPenUpOn0.Checked) && (brightnes == 255))
                        brightnes = -1;
                    scanCNCPos[k] = new ImgPoint(x, y, brightnes);
                }
            }
        }

        private void CreateLinePattern(double distance, double step, double angle, double width, double height)
        {
            bool oneDirection = cBOnlyLeftToRight.Checked;
            Logger.Trace("CreateLinePattern distance:{0} step:{1} angle:{2}", distance, step, angle);
            if ((distance <= 0))
                return;

            double r = Math.Sqrt(width * width + height * height) / 2;

            // Rotation information
            double ca = Math.Cos((angle - 90) * Math.PI / 180);
            double sa = Math.Sin((angle - 90) * Math.PI / 180);

            // Translation information
            double cx = 0 + (width / 2);
            double cy = 0 + (height / 2);

            double x1, y1, x2, y2, dx, dy, deltaX, deltaY;
            bool changeDirection = false;

            if (CbEngravingTopDown.Checked)
                for (double i = -r; i < r; i += distance)
                { if (!CreateLine(i)) continue; }
            else
                for (double i = r; i > -r; i -= distance)
                { if (!CreateLine(i)) continue; }

            bool CreateLine(double i)
            {
                x1 = cx + (i * ca) + (r * sa);
                y1 = cy + (i * sa) - (r * ca);
                x2 = cx + (i * ca) - (r * sa);
                y2 = cy + (i * sa) + (r * ca);

                if ((x1 < 0 && x2 < 0) || (x1 > width && x2 > width))
                    return false;   // continue;
                if ((y1 < 0 && y2 < 0) || (y1 > height && y2 > height))
                    return false;   // continue;

                if (!oneDirection && changeDirection)
                {
                    dx = x2; dy = y2;
                    x2 = x1; y2 = y1;
                    x1 = dx; y1 = dy;
                    deltaX = -step * Math.Cos(angle * Math.PI / 180);
                    deltaY = -step * Math.Sin(angle * Math.PI / 180);
                }
                else
                {
                    dx = x1; dy = y1;
                    deltaX = step * Math.Cos(angle * Math.PI / 180);
                    deltaY = step * Math.Sin(angle * Math.PI / 180);
                }
                scanCNCPos.Add(new ImgPoint(x1, y1, 1));

                int safe = (int)(Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)) / step * 1.5);//1000;
                while ((safe-- > 0))
                {
                    dx += deltaX;
                    dy += deltaY;
                    scanCNCPos.Add(new ImgPoint(dx, dy, 1));

                    if ((Math.Abs(dx - x2) <= Math.Abs(deltaX)) && (Math.Abs(dy - y2) <= Math.Abs(deltaY)))
                        break;
                }

                scanCNCPos.Add(new ImgPoint(x2, y2, 0));
                changeDirection = !changeDirection;
                return true;
            }
        }

        private void CreateSpiral(double distance, double step, double maxR)
        {
            scanCNCPos.Clear();
            Logger.Trace("CreateSpiral distance:{0} step:{1} maxR:{2}", distance, step, maxR);
            if ((distance <= 0) || (maxR <= distance))
                return;
            double r = 0, a = 0, x, y;
            double aDeg = 0, deltaDeg;

            try
            {
                while (r < maxR)
                {
                    x = Math.Cos(a) * r;
                    y = -Math.Sin(a) * r;
                    scanCNCPos.Add(new ImgPoint(x, y, 1));
                    deltaDeg = (Math.Atan(step / r) * 180 / Math.PI);   // step width should be pixel distance
                    aDeg += deltaDeg; r += distance * deltaDeg / 360;                // 1 deg step, 1 distance/turn
                    a = (aDeg * Math.PI / 180);
                }
            }
            catch (Exception err)
            {
                EventCollector.StoreException("CreateSpiral scanCNCPos.Count=" + scanCNCPos.Count + " " + err.Message + " ---");
                Logger.Error(err, "Could not create spiral, scanCNCPos.Count:{0}  r:{1}   maxR:{2}", scanCNCPos.Count, r, maxR);
                System.Windows.Forms.MessageBox.Show("Error: " + err.Message + " \r\n\r\nPattern is may not complete.\r\nTry with higher Line distance", "Error");
            }
        }

        private void CreateFrom2DView(double step)
        {
            scanCNCPos.Clear();
            Logger.Trace("createFrom2DView G2G3:{0}", VisuGCode.ContainsG2G3Command());

            if (VisuGCode.GetPathCordinates(scanCNCPos, (float)step))  // get X,Y-pos and G-nr
            { }
        }

        private void RemoveIntermediateSteps()
        {
            Logger.Trace("RemoveIntermediateSteps start");

            int brightnesNext, brightnesNow, brightnesLast;
            double angleNext;                       // when adjacent line segments have the same angle - end point of first can be removed 
            double angleNow = 0;                       // when adjacent line segments have the same angle - end point of first can be removed 
            double angleLast = 0;
            int removed = 0;
            if (scanCNCPos.Count > 3)
            {
                System.Windows.Point pointNext;
                System.Windows.Point pointNow = new System.Windows.Point(scanCNCPos[scanCNCPos.Count - 2].X, scanCNCPos[scanCNCPos.Count - 2].Y);
                brightnesNow = scanCNCPos[scanCNCPos.Count - 2].brightnes;
                brightnesLast = scanCNCPos[scanCNCPos.Count - 1].brightnes;
                for (int i = (scanCNCPos.Count - 3); i > 0; i--)
                {
                    brightnesNext = scanCNCPos[i].brightnes;
                    pointNext = new System.Windows.Point(scanCNCPos[i].X, scanCNCPos[i].Y);
                    angleNext = GcodeMath.GetAlpha(pointNext, pointNow);
                    if ((brightnesNext == brightnesNow) && (brightnesNow == brightnesLast) && (brightnesNow >= 0) && IsEqual(angleNext, angleNow) && IsEqual(angleNow, angleLast))
                    {
                        if (((i + 2) < scanCNCPos.Count))
                        {
                            scanCNCPos.RemoveAt(i + 1);
                            removed++;
                        }
                    }
                    else
                    {
                        angleLast = angleNow;
                        angleNow = angleNext;
                    }

                    pointNow = pointNext;
                    brightnesLast = brightnesNow;
                    brightnesNow = brightnesNext;
                }
            }
            Logger.Trace("RemoveIntermediateSteps removed:{0}", removed);
        }
        private static bool IsEqual(double a, double b)
        { return (Math.Abs(a - b) < 0.001); }

    }
}
