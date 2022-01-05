/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2018-2022 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace GrblPlotter
{   public partial class GCodeFromImage
    {

        private static float cncCoordZ;   
        private static float cncCoordLastZ;   

        private void GenerateHeightData()
        {
            Logger.Debug("generateHeightData horizontal:{0}  useZ:{1}", rbEngravingPattern1.Checked, rBGrayZ.Checked);
            if (resultImage == null) return;                //if no image, do nothing

            cncPixelResoX = (float)nUDResoX.Value;           // resolution = distance between pixels / lines / columns
            cncPixelResoY = (float)nUDResoY.Value;
            float resoRatioYX = cncPixelResoY / cncPixelResoX;
            float penWidth = cncPixelResoY;

            resultImage = new Bitmap(adjustedImage);        // adjustedImage is shown in preview, resultImage will be scanned

            if (rbEngravingPattern1.Checked)        // horizontal no change
            { }
            else if (rbEngravingPattern2.Checked)                // if diagonal, take account of 45° angle
            {   int factor = (int)Math.Round(resoRatioYX) + 1;
                resoRatioYX = factor;
                cncPixelResoX = cncPixelResoY * 1.414f / factor;
                cncPixelResoY = cncPixelResoX;// cncPixelResoY * 1.414f; 
            }
            else
            {   cncPixelResoY = cncPixelResoX; }

            int newWidth = (int)((float)nUDWidth.Value / cncPixelResoX);
            int newHeigth = (int)((float)nUDHeight.Value / cncPixelResoY);
            if ((newWidth > 0) && (newHeigth > 0))
            {
                AForge.Imaging.Filters.ResizeNearestNeighbor filterResize = new AForge.Imaging.Filters.ResizeNearestNeighbor(newWidth, newHeigth);
                resultImage = filterResize.Apply(adjustedImage);
            }
            int pixelCount = resultImage.Width * resultImage.Height;
            int pixelProcessed = 0;
            int percentDone;

            int pixelPosY;                  // top/botom pixel
            int pixelPosX;                  // Left/right pixel
            bool useZnotS = rBGrayZ.Checked;    // calculate Z-value or S-value
            bool backAndForth = !cBOnlyLeftToRight.Checked;
            bool relative = cBCompress.Checked; // true;
       //     bool firstValue;//, firstLine=true;

            int pixelValLast, pixelValNow, pixelValNext;    // gray-values at pixel pos
            cncCoordLastX = cncCoordX = 0;
            cncCoordLastY = cncCoordY = 0;

            if (rbEngravingPattern1.Checked)        // horizontal
                GenerateGCodePreset(0, cncPixelResoY * (resultImage.Height - 1));    // gcode.setup, -jobStart, PenUp, -G0 to 1st pos.
            else if (rbEngravingPattern2.Checked)
                GenerateGCodePreset(0, 0);    // gcode.setup, -jobStart, PenUp, -G0 to 1st pos.
            else
            {   cncCoordLastX = cncCoordX = cncPixelResoX * resultImage.Width * (float)NuDSpiralCenterX.Value;  // start pos
                cncCoordLastY = cncCoordY = cncPixelResoY * resultImage.Height * (float)NuDSpiralCenterY.Value;
                GenerateGCodePreset(cncCoordX, cncCoordY);
            }

            if (useZnotS)   // set 2D-View display option to translate S/Z value to pen width
            {
                Gcode.GcodeZApply = true;
                Gcode.GcodePWMEnable = false;
                Gcode.Comment(finalString, string.Format("{0} Min=\"{1}\" Max=\"{2}\" Width=\"{3}\" />", XmlMarker.HalftoneZ, nUDZTop.Value, nUDZBottom.Value, penWidth));
            }
            else
            {
                Gcode.GcodeZApply = false;
                Gcode.GcodePWMEnable = true;
                Gcode.Comment(finalString, string.Format("{0} Min=\"{1}\" Max=\"{2}\" Width=\"{3}\" />", XmlMarker.HalftoneS, nUDSTop.Value, nUDSBottom.Value, penWidth));
            }

            Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" PenColor=\"black\" >", XmlMarker.FigureStart, 1));
            finalString.AppendFormat("F{0}\r\n", Gcode.GcodeXYFeed);    // set feedrate

            if (rbEngravingPattern1.Checked)        // horizontal
            {
                Logger.Info("Create halftone horizontal width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                //Start image
                cncCoordLastX = cncCoordX = 0;      //Left pixel
                pixelPosY = resultImage.Height - 1;             //top tile
                cncCoordLastY = cncCoordY = cncPixelResoY * (float)pixelPosY;

                while (pixelPosY >= 0)  // line by line from top to bottom
                {
                    pixelPosX = 0;      // 1st line starts at 0
                    cncCoordX = cncPixelResoX * (float)pixelPosX;
                    cncCoordY = cncPixelResoY * (float)pixelPosY;
                    Gcode.Comment(finalString, string.Format("{0} Y=\"{1}\">", XmlMarker.PassStart, cncCoordY));

                    // move to first position in line                    
                    if (!backAndForth || (pixelPosY == (resultImage.Height - 1)))              // move line by line from left to right
                    {
                        if (relative) { finalString.AppendLine("G90"); }  // switch to absolute
                        Gcode.PenUp(finalString, "");
                        cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                        Gcode.MoveToRapid(finalString, cncCoordX, cncCoordY,"");
                        if (useZnotS)   // move servo down and delay
                        {
                            pixelValNow = GetPixelValue(pixelPosX, pixelPosY, useZnotS);
                            SetCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, false);
                        }
                        else
                        {
                            finalString.AppendFormat("G{0} X{1} S{2}\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(-0.01), Math.Round(nUDSTop.Value));
                            finalString.AppendFormat("G{0} P{1}\r\n", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyUp));
                        }
                        cncCoordLastX = cncCoordX;
                        cncCoordLastY = cncCoordY;
                    }
                    // create horizontal data left to rigth, check current and next pixel for change in value to reduce gcode      
                    if (relative) { finalString.AppendLine("G91G1"); }
                    pixelValLast = -1; pixelValNow = GetPixelValue(pixelPosX, pixelPosY, useZnotS);
                    while ((pixelPosX < (resultImage.Width - 1)) && (pixelPosY >= 0))   //From left to right
                    {
                        //firstValue = (pixelValLast < 0) && firstLine;
                        pixelValNext = GetPixelValue(pixelPosX + 1, pixelPosY, useZnotS);
                        if ((pixelPosX == 0) || (pixelValNow != pixelValLast) || (pixelValNow != pixelValNext))
                        { SetCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative); }
                        pixelValLast = pixelValNow; pixelValNow = pixelValNext;
                        pixelProcessed++; pixelPosX++;
                    }
                    SetCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);
                    pixelPosY--;
                    Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.PassEnd));

                    // create horizontal data rigth to left
                    if (backAndForth && (pixelPosY >= 0))
                    {
                        Gcode.Comment(finalString, string.Format("{0} Y=\"{1}\">", XmlMarker.PassStart, cncCoordY));
                        pixelPosX = (resultImage.Width - 1);  // 2nd line starts far right
                        SetCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);
                        pixelValLast = -1; pixelValNow = GetPixelValue(pixelPosX, pixelPosY, useZnotS);
                        while ((pixelPosX > 0) && (pixelPosY >= 0))     //From right to left
                        {
                            pixelValNext = GetPixelValue(pixelPosX - 1, pixelPosY, useZnotS);
                            if ((pixelPosX == (resultImage.Width - 1)) || (pixelValNow != pixelValLast) || (pixelValNow != pixelValNext))
                            { SetCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative); }
                            pixelValLast = pixelValNow; pixelValNow = pixelValNext;
                            pixelProcessed++; pixelPosX--;
                        }
                        SetCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);
                        pixelPosY--;
                        Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.PassEnd));
                    //    firstLine = false;
                    }

                    percentDone = (pixelProcessed * 100) / pixelCount;
                    lblStatus.Text = "Generating GCode... " + Convert.ToString(percentDone) + "%";
                    if ((percentDone % 10) == 0)
                        Refresh();
                }
                lblStatus.Text = "GCode generation finished";
                Refresh();
            }
            else if (rbEngravingPattern2.Checked)        // diagonal
            {
                //Start image - diagonal up-left to bottom-right
                Logger.Info("Create halftone diagonal width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                pixelPosX = 0;
                pixelPosY = 0;
                if (relative) { finalString.AppendLine("G91G1"); }
                while ((pixelPosX < resultImage.Width) | (pixelPosY < resultImage.Height))
                {
                    while ((pixelPosX < resultImage.Width) & (pixelPosY >= 0))                      // top-left to bot-right
                    {
                        pixelValNext = GetPixelValue(pixelPosX - 1, pixelPosY, useZnotS);
                        { SetCommand(pixelPosX, pixelPosY, pixelValNext, useZnotS, relative); }     // scan with 1px reso
                        pixelProcessed++; pixelPosX++; pixelPosY--;
                    }
                    pixelPosX--; pixelPosY++;   // loop did one too much

                    if (pixelPosX >= resultImage.Width - 1) pixelPosY += (int)resoRatioYX;    // next line distance is higher  pixelPosY++;
                    else pixelPosX += (int)resoRatioYX;    // ++

                    while ((pixelPosX >= 0) & (pixelPosY < resultImage.Height))             // bot-rigth to top-left
                    {
                        pixelValNext = GetPixelValue(pixelPosX - 1, pixelPosY, useZnotS);
                        { SetCommand(pixelPosX, pixelPosY, pixelValNext, useZnotS, relative); }
                        pixelProcessed++; pixelPosX--; pixelPosY++;
                    }
                    pixelPosX++; pixelPosY--;   // loop did one too much

                    if (pixelPosY >= resultImage.Height - 1) pixelPosX += (int)resoRatioYX;    //++
                    else pixelPosY += (int)resoRatioYX;    //++

                    percentDone = (pixelProcessed * 100) / pixelCount;
                    lblStatus.Text = "Generating GCode... " + Convert.ToString(percentDone) + "%";
                    if ((percentDone % 10) == 0)
                        Refresh();
                }
                lblStatus.Text = "GCode generation finished";
                Refresh();
            }
            else if (rbEngravingPattern3.Checked)        // spiral
            {
                Logger.Info("Create halftone spiral width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                CreateSpiral((float)nUDResoY.Value, cncPixelResoX, (float)Math.Sqrt(resultImage.Width* resultImage.Width + resultImage.Height* resultImage.Height));
                // fill path with brightnes values
                CreateScanPath(resultImage.Width, resultImage.Height, resultImage.Width * (float)NuDSpiralCenterX.Value, resultImage.Height * (float)NuDSpiralCenterY.Value);
                // improove path
                RemoveIntermediateSteps();
                ApplyScanPath(useZnotS, relative);
            }
            else
            {
                Logger.Info("Create halftone external path width:{0} height:{1} resoX:{2} resoY:{3}", resultImage.Width, resultImage.Height, cncPixelResoX, cncPixelResoY);
                CreateFrom2DView(cncPixelResoX);
                CreateScanPath(resultImage.Width, resultImage.Height, resultImage.Width * (float)NuDSpiralCenterX.Value, resultImage.Height * (float)NuDSpiralCenterY.Value);
                RemoveIntermediateSteps();
                ApplyScanPath(useZnotS, relative);
            }


            if (relative) { finalString.AppendLine("G90"); }
            Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.FigureEnd));
            Gcode.JobEnd(finalString,"EndJob");
            if (rBGrayS.Checked && cBLaserModeOffEnd.Checked)
            { finalString.AppendLine("$32=0 (Lasermode off)"); }

            imagegcode = "( Generated by GRBL-Plotter )\r\n";
            imagegcode += Gcode.GetHeader("Image import", "") + finalString.Replace(',', '.').ToString() + Gcode.GetFooter();
        }

        private int GetPixelValue(int picX, int picY, bool useZ)
        {   if ((picX < 0) || (picY < 0) || (picX >= resultImage.Width) || (picY >= resultImage.Height))
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
            { return (int)((255- brightness) * (float)(nUDSBottom.Value - nUDSTop.Value)) / 255; }
        }
        private int GetPixelBrightnes(float picX, float picY)
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

        private void SetCommandFloat(float pixelPosX, float pixelPosY, int bright, bool useZnotS, bool relative)
        {
            if (bright > 255) { bright = 255; }
            if (bright < 0) { bright = 0; }
            int height = 255 - bright;    // pixelVal 0=black, 255=white
            string SZ;
            if (useZnotS)
            {
                cncCoordZ = (float)nUDZTop.Value - height * (float)(nUDZTop.Value - nUDZBottom.Value) / 255;    // calc Z value
                if (relative) { SZ = string.Format("Z{0:0.##}", (cncCoordZ - cncCoordLastZ)); }
                else { SZ = string.Format("Z{0}", Gcode.FrmtNum(cncCoordZ)); }
                cncCoordLastZ = cncCoordZ;
            }
            else
            {
                int sVal = (int)Math.Abs((float)nUDSTop.Value + (((255 - bright) * (float)(nUDSBottom.Value - nUDSTop.Value)) / 255));
                SZ = string.Format("S{0}", sVal);// Math.Round(nUDSTop.Value + sVal)); 
            }

            SetXYCommandFloat(pixelPosX, pixelPosY, SZ, relative);
        }
        private void SetCommand(int pixelPosX, int pixelPosY, int pixelVal, bool useZnotS, bool relative)
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
        }
        private static void SetXYCommandFloat(float pixelPosX, float pixelPosY, string SZ, bool relative)
        {
            cncCoordX = cncPixelResoX * pixelPosX;
            cncCoordY = cncPixelResoY * pixelPosY;
            if (relative)
            {
                float difX = cncCoordX - cncCoordLastX; string x = (difX != 0) ? string.Format("X{0:0.##}", difX) : "";
                float difY = cncCoordY - cncCoordLastY; string y = (difY != 0) ? string.Format("Y{0:0.##}", difY) : "";
                string finalCommand = string.Format("{0}{1}{2}\r\n", x, y, SZ);
                if (finalCommand.Length > 1)
                    finalString.AppendFormat("{0}", finalCommand); //finalString.AppendFormat("{0}{1}{2}\r\n", x,y,SZ);
            }
            else
            { finalString.AppendFormat("G{0} X{1} Y{2} {3}\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), SZ); }      // set absolute position
            cncCoordLastX = cncCoordX;
            cncCoordLastY = cncCoordY;
        }
        private static void SetXYCommand(int pixelPosX, int pixelPosY, string SZ, bool relative)
        {
            cncCoordX = cncPixelResoX * (float)pixelPosX;
            cncCoordY = cncPixelResoY * (float)pixelPosY;
            if (relative)
            {
                float difX = cncCoordX - cncCoordLastX; string x = (difX != 0) ? string.Format("X{0:0.##}", difX) : "";
                float difY = cncCoordY - cncCoordLastY; string y = (difY != 0) ? string.Format("Y{0:0.##}", difY) : "";
                finalString.AppendFormat("{0}{1}{2}\r\n", x, y, SZ);
            }
            else
            { finalString.AppendFormat("G{0} X{1} Y{2} {3}\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), SZ); }      // set absolute position
            cncCoordLastX = cncCoordX;
            cncCoordLastY = cncCoordY;
        }

        private static readonly List<ImgPoint> scanCNCPos = new List<ImgPoint>();
        private void ApplyScanPath(bool useZnotS, bool relative)
        {
            Logger.Trace("applyScanPath");
            int pixelValLast, pixelValNow, pixelValNext;
            float pixelPosX, pixelPosY;    // gray-values at pixel pos
            if (scanCNCPos.Count <= 2)
                return;

            bool isPenUp = false;
            cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
            pixelValLast = -1; pixelValNow = pixelValNext = scanCNCPos[0].brightnes;    // GetPixelValue(scanCNCPos[0].X, scanCNCPos[0].Y, useZnotS);

            if (relative) { finalString.AppendLine("G91G1 (relative mode)"); }
            for (int i=1; i < scanCNCPos.Count-1; i++)                
            {
                pixelPosX = scanCNCPos[i].X; 
                pixelPosY = scanCNCPos[i].Y;
                if ((pixelPosX < 0) || (pixelPosY < 0) || (scanCNCPos[i].brightnes < 0))
                {   // do pen-up
                    if (!isPenUp)
                    {
                        if (relative) { finalString.AppendLine("G90 (absolute mode)"); }  // switch to absolute
                        Gcode.PenUp(finalString, "PU");
                        cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                        if (relative) { finalString.AppendLine("G91G1 (relative mode)"); }
                    }
                    isPenUp = true;
                }
                else
                {
                    cncCoordX = cncPixelResoX * pixelPosX;
                    cncCoordY = cncPixelResoY * pixelPosY;
                    if (scanCNCPos[i].brightnes < 0)    // keep pen-up
                    {
                        if (!isPenUp)
                        {
                            if (relative) { finalString.AppendLine("G90 (absolute mode)"); }  // switch to absolute
                            Gcode.PenUp(finalString, "PU");
                            cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
                            if (relative) { finalString.AppendLine("G91G1 (relative mode)"); }
                        }
                        isPenUp = true;
                        continue;
                    }
                    // do pen-down
                    if (isPenUp)
                    {
                        if (relative) { finalString.AppendLine("G90 (absolute mode)"); }  // switch to absolute
                        //Gcode.MoveToRapid(finalString, cncCoordX, cncCoordY,"");
                        finalString.AppendFormat("G{0} X{1} Y{2} F10000\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY));

                        if (useZnotS)
                        {   // move z down
                        //    finalString.AppendFormat("G{0} X{1} Y{2} Z{3} F{4} (PD)\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), Gcode.FrmtNum(nUDZTop.Value), Gcode.GcodeXYFeed);
                            finalString.AppendFormat("Z{0} F{1} (PD)\r\n", Gcode.FrmtNum(nUDZTop.Value), Gcode.GcodeXYFeed);
                        }
                        else
                        {   // move servo down and delay
                         //   finalString.AppendFormat("G{0} X{1} Y{2} F{3} S{4} (PD)\r\n", Gcode.FrmtCode(1), Gcode.FrmtNum(cncCoordX), Gcode.FrmtNum(cncCoordY), Gcode.GcodeXYFeed, Math.Round(nUDSTop.Value));
                            finalString.AppendFormat("F{0} S{1} (PD)\r\n", Gcode.GcodeXYFeed, Math.Round(nUDSTop.Value));
                            finalString.AppendFormat("G{0} P{1}\r\n", Gcode.FrmtCode(4), Gcode.FrmtNum(Properties.Settings.Default.importGCPWMDlyUp));
                        }
                        cncCoordLastZ = cncCoordZ = (float)nUDZTop.Value;
                        if (relative) { finalString.AppendLine("G91G1 (relative mode)"); }
                        isPenUp = false;
                        cncCoordLastX = cncCoordX;
                        cncCoordLastY = cncCoordY;
                        cncCoordLastZ = cncCoordZ;
                    }

                    pixelValNext = scanCNCPos[i + 1].brightnes; //GetPixelValue(scanCNCPos[i + 1].X, scanCNCPos[i + 1].Y, useZnotS);

                    if ((pixelValNow != pixelValLast) || (pixelValNow != pixelValNext))
                    {   SetCommandFloat(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);  }
                    else
                    {   SetXYCommandFloat(pixelPosX, pixelPosY, "", relative);  }
                }
                pixelValLast = pixelValNow; pixelValNow = pixelValNext;
            }
        }

        private void CreateScanPath(int width, int height, float offsetX, float offsetY)
        {   //scanPixelPos.Clear();
            Logger.Trace("createScanPath width:{0} height:{1}", width, height);
            float x,y;
            int brightnes;
            for(int k=0; k < scanCNCPos.Count; k++)
            {   x = (scanCNCPos[k].X/ cncPixelResoX + offsetX);
                y = (scanCNCPos[k].Y/ cncPixelResoX + offsetY);
                if (scanCNCPos[k].brightnes == 0) 
                {   scanCNCPos[k] = new ImgPoint(x, y, - 2);
                    continue;
                }
                //if ((x != oldX) || (y != oldY))
                {   if ((x < 0) || (x >= width)) {x = -1;}
                    if ((y < 0) || (y >= height)) {y = -1;}
                    brightnes = GetPixelBrightnes(x, y);
                    if ((CbPenUpOn0.Checked) && (brightnes == 255))
                        brightnes = -1;
                    scanCNCPos[k] = new ImgPoint(x, y, brightnes);
                }
            }            
        }

        private void CreateSpiral(float distance, float step, float maxR)
        {
            scanCNCPos.Clear();
            Logger.Trace("createSpiral createSpiral:{0} step:{1} maxR:{2}", distance, step, maxR);
            if ((distance <=0) || (maxR <= distance))
                return;
            double r = 0,a = 0,x,y;
            double aDeg = 0, deltaDeg;
            while (r < maxR)
            {   x = Math.Cos(a) * r;
                y = -Math.Sin(a) * r;
                scanCNCPos.Add(new ImgPoint((float)x, (float)y, 1));
                deltaDeg = (Math.Atan(step / r) * 180 / Math.PI);   // step width should be pixel distance
                aDeg += deltaDeg; r += distance * deltaDeg / 360;                // 1 deg step, 1 distance/turn
                a = (aDeg * Math.PI / 180);
            }
        }
        
        private void CreateFrom2DView(float step)
        {
            scanCNCPos.Clear();
            Logger.Trace("createFrom2DView G2G3:{0}",VisuGCode.ContainsG2G3Command());

            if (VisuGCode.GetPathCordinates(scanCNCPos, step))  // get X,Y-pos and G-nr
            {   }
        }

        private void RemoveIntermediateSteps()
        {
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
        }
        private static bool IsEqual(double a, double b)
        { return (Math.Abs(a - b) < 0.001); }

    }
}
