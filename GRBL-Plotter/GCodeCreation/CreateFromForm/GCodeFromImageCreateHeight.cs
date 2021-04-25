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
 * 2021-04-17 new
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GRBL_Plotter
{   public partial class GCodeFromImage
    {

        private static float cncCoordZ;   
        private static float cncCoordLastZ;   

        private void generateHeightData()
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
            AForge.Imaging.Filters.ResizeNearestNeighbor filterResize = new AForge.Imaging.Filters.ResizeNearestNeighbor(newWidth, newHeigth);
            resultImage = filterResize.Apply(adjustedImage);

            int pixelCount = resultImage.Width * resultImage.Height;
            int pixelProcessed = 0;
            int percentDone = 0;

            int pixelPosY;                  // top/botom pixel
            int pixelPosX;                  // Left/right pixel
            bool useZnotS = rBGrayZ.Checked;    // calculate Z-value or S-value
            bool backAndForth = !cBOnlyLeftToRight.Checked;
            bool relative = cBCompress.Checked; // true;
            bool firstValue, firstLine=true;

            int pixelValLast, pixelValNow, pixelValNext;    // gray-values at pixel pos
            cncCoordLastX = cncCoordX = pixelPosX = 0;
            cncCoordLastY = cncCoordY = pixelPosY = 0;

            if (rbEngravingPattern1.Checked)        // horizontal
                generateGCodePreset(0, cncPixelResoY * (resultImage.Height - 1));    // gcode.setup, -jobStart, PenUp, -G0 to 1st pos.
            else if (rbEngravingPattern2.Checked)
                generateGCodePreset(0, 0);    // gcode.setup, -jobStart, PenUp, -G0 to 1st pos.
            else
            {   cncCoordLastX = cncCoordX = cncPixelResoX * resultImage.Width / 2;
                cncCoordLastY = cncCoordY = cncPixelResoY * resultImage.Height / 2;
                generateGCodePreset(cncCoordX, cncCoordY);
            }

            if (useZnotS)   // set 2D-View display option to translate S/Z value to pen width
                gcode.Comment(finalString, string.Format("{0} Min=\"{1}\" Max=\"{2}\" Width=\"{3}\" />", xmlMarker.halftoneZ, nUDZTop.Value, nUDZBot.Value, penWidth)); 
            else
                gcode.Comment(finalString, string.Format("{0} Min=\"{1}\" Max=\"{2}\" Width=\"{3}\" />", xmlMarker.halftoneS, nUDSMin.Value, nUDSMax.Value, penWidth)); 

            gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" PenColor=\"black\" >", xmlMarker.figureStart, 1));
            finalString.AppendFormat("F{0}\r\n", gcode.gcodeXYFeed);    // set feedrate

            if (rbEngravingPattern1.Checked)        // horizontal
            {
                //Start image
                cncCoordLastX = cncCoordX = pixelPosX = 0;      //Left pixel
                pixelPosY = resultImage.Height - 1;             //top tile
                cncCoordLastY = cncCoordY = cncPixelResoY * (float)pixelPosY;

                while (pixelPosY >= 0)  // line by line from top to bottom
                {
                    pixelPosX = 0;      // 1st line starts at 0
                    cncCoordX = cncPixelResoX * (float)pixelPosX;
                    cncCoordY = cncPixelResoY * (float)pixelPosY;
                    gcode.Comment(finalString, string.Format("{0} Y=\"{1}\">", xmlMarker.passStart, cncCoordY));

                    // move to first position in line                    
                    if (!backAndForth || (pixelPosY == (resultImage.Height - 1)))              // move line by line from left to right
                    {
                        if (relative) { finalString.AppendLine("G90"); }  // switch to absolute
                        gcode.PenUp(finalString);
                        cncCoordLastZ = cncCoordZ = gcode.gcodeZUp;
                        gcode.MoveToRapid(finalString, cncCoordX, cncCoordY);
                        if (useZnotS)   // move servo down and delay
                        {
                            pixelValNow = pixelValNext = getPixelValue(pixelPosX, pixelPosY, useZnotS);
                            setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, false);
                        }
                        else
                        {
                            finalString.AppendFormat("G{0} X{1} S{2}\r\n", gcode.frmtCode(1), gcode.frmtNum(-0.01), Math.Round(nUDSMin.Value));
                            finalString.AppendFormat("G{0} P{1}\r\n", gcode.frmtCode(4), gcode.frmtNum(Properties.Settings.Default.importGCPWMDlyUp));
                        }
                        cncCoordLastX = cncCoordX;
                        cncCoordLastY = cncCoordY;
                    }
                    // create horizontal data left to rigth, check current and next pixel for change in value to reduce gcode      
                    if (relative) { finalString.AppendLine("G91G1"); }
                    pixelValLast = -1; pixelValNow = pixelValNext = getPixelValue(pixelPosX, pixelPosY, useZnotS);
                    while ((pixelPosX < (resultImage.Width - 1)) && (pixelPosY >= 0))   //From left to right
                    {
                        firstValue = (pixelValLast < 0) && firstLine;
                        pixelValNext = getPixelValue(pixelPosX + 1, pixelPosY, useZnotS);
                        if ((pixelPosX == 0) || (pixelValNow != pixelValLast) || (pixelValNow != pixelValNext))
                        { setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative); }
                        pixelValLast = pixelValNow; pixelValNow = pixelValNext;
                        pixelProcessed++; pixelPosX++;
                    }
                    setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);
                    pixelPosY--;
                    gcode.Comment(finalString, string.Format("{0}>", xmlMarker.passEnd));

                    // create horizontal data rigth to left
                    if (backAndForth && (pixelPosY >= 0))
                    {
                        gcode.Comment(finalString, string.Format("{0} Y=\"{1}\">", xmlMarker.passStart, cncCoordY));
                        pixelPosX = (resultImage.Width - 1);  // 2nd line starts far right
                        setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);
                        pixelValLast = -1; pixelValNow = pixelValNext = getPixelValue(pixelPosX, pixelPosY, useZnotS);
                        while ((pixelPosX > 0) && (pixelPosY >= 0))     //From right to left
                        {
                            pixelValNext = getPixelValue(pixelPosX - 1, pixelPosY, useZnotS);
                            if ((pixelPosX == (resultImage.Width - 1)) || (pixelValNow != pixelValLast) || (pixelValNow != pixelValNext))
                            { setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative); }
                            pixelValLast = pixelValNow; pixelValNow = pixelValNext;
                            pixelProcessed++; pixelPosX--;
                        }
                        setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);
                        pixelPosY--;
                        gcode.Comment(finalString, string.Format("{0}>", xmlMarker.passEnd));
                        firstLine = false;
                    }

                    percentDone = (pixelProcessed * 100) / pixelCount;
                    lblStatus.Text = "Generating GCode... " + Convert.ToString(percentDone) + "%";
                    if ((percentDone % 10) == 0)
                        Refresh();
                }
            }
            else if (rbEngravingPattern2.Checked)        // diagonal
            {
                //Start image - diagonal up-left to bottom-right
                pixelPosX = 0;
                pixelPosY = 0;
                if (relative) { finalString.AppendLine("G91G1"); }
                while ((pixelPosX < resultImage.Width) | (pixelPosY < resultImage.Height))
                {
                    while ((pixelPosX < resultImage.Width) & (pixelPosY >= 0))                      // top-left to bot-right
                    {
                        pixelValNext = getPixelValue(pixelPosX - 1, pixelPosY, useZnotS);
                        { setCommand(pixelPosX, pixelPosY, pixelValNext, useZnotS, relative); }     // scan with 1px reso
                        pixelProcessed++; pixelPosX++; pixelPosY--;
                    }
                    pixelPosX--; pixelPosY++;   // loop did one too much

                    if (pixelPosX >= resultImage.Width - 1) pixelPosY += (int)resoRatioYX;    // next line distance is higher  pixelPosY++;
                    else pixelPosX += (int)resoRatioYX;    // ++

                    while ((pixelPosX >= 0) & (pixelPosY < resultImage.Height))             // bot-rigth to top-left
                    {
                        pixelValNext = getPixelValue(pixelPosX - 1, pixelPosY, useZnotS);
                        { setCommand(pixelPosX, pixelPosY, pixelValNext, useZnotS, relative); }
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
            }
            else if (rbEngravingPattern3.Checked)        // spiral
            {
                createSpiral((float)nUDResoY.Value, cncPixelResoX, resultImage.Width);
                createScanPath(resultImage.Width, resultImage.Height, resultImage.Width/2, resultImage.Height/2);
                applyScanPath(useZnotS, relative);
            }
            else
            {
                createFrom2DView(cncPixelResoX);
                createScanPath(resultImage.Width, resultImage.Height,0,0);
                applyScanPath(useZnotS, relative);
            }


            if (relative) { finalString.AppendLine("G90"); }
            gcode.Comment(finalString, string.Format("{0}>", xmlMarker.figureEnd));
            gcode.jobEnd(finalString);
            if (rBGrayS.Checked && cBLaserModeOffEnd.Checked)
            { finalString.AppendLine("$32=0 (Lasermode off)"); }

            imagegcode = "( Generated by GRBL-Plotter )\r\n";
            imagegcode += gcode.GetHeader("Image import") + finalString.Replace(',', '.').ToString() + gcode.GetFooter();
        }

        private int getPixelValue(int picX, int picY, bool useZ)
        {   if ((picX < 0) || (picY < 0) || (picX >= resultImage.Width) || (picY >= resultImage.Height))
                return 0;
            Color myColor = resultImage.GetPixel(picX, (resultImage.Height - 1) - picY);    // Get pixel color
            int brightness = (int)Math.Round((double)(myColor.R + myColor.G + myColor.B) / 3);        // calc height FF=white, 0=black
            if (myColor.A < 128)    // assume transparency as white
                return useZ? 255:0;
            if (useZ)
            { return brightness; }      // calc height FF=white, 0=black
            else
            { return (int)((255- brightness) * (float)(nUDSMax.Value - nUDSMin.Value)) / 255; }
        }

        private void setCommand(int pixelPosX, int pixelPosY, int pixelVal, bool useZnotS, bool relative)
        {
            int height = 255 - pixelVal;    // pixelVal 0=black, 255=white
            string SZ = "(SZ)";
            if (useZnotS)
            {
                cncCoordZ = (float)nUDZTop.Value - height * (float)(nUDZTop.Value - nUDZBot.Value) / 255;    // calc Z value
                if (relative) { SZ = string.Format("Z{0:0.##}", (cncCoordZ - cncCoordLastZ)); }
                else { SZ = string.Format("Z{0}", gcode.frmtNum(cncCoordZ)); }
                cncCoordLastZ = cncCoordZ;
            }
            else
            { SZ = string.Format("S{0}", Math.Round(nUDSMin.Value + pixelVal)); }

            setXYCommand(pixelPosX, pixelPosY, SZ, relative);
        }
        private void setXYCommand(int pixelPosX, int pixelPosY, string SZ, bool relative)
        {
            cncCoordX = cncPixelResoX* (float)pixelPosX;
            cncCoordY = cncPixelResoY * (float)pixelPosY;
            if (relative)
            {
                float difX = cncCoordX - cncCoordLastX; string x = (difX != 0) ? string.Format("X{0:0.##}", difX) : "";
                float difY = cncCoordY - cncCoordLastY; string y = (difY != 0) ? string.Format("Y{0:0.##}", difY) : "";
                finalString.AppendFormat("{0}{1}{2}\r\n", x,y,SZ);
            }
            else
            { finalString.AppendFormat("G{0} X{1} Y{2} {3}\r\n", gcode.frmtCode(1), gcode.frmtNum(cncCoordX), gcode.frmtNum(cncCoordY), SZ); }      // set absolute position
            cncCoordLastX = cncCoordX;
            cncCoordLastY = cncCoordY;
        }
        
        
        private List<Point> scanPixelPos = new List<Point>();
        private List<PointF> scanCNCPos = new List<PointF>();

        private void applyScanPath(bool useZnotS, bool relative)
        {
            Logger.Trace("applyScanPath");
            int pixelValLast, pixelValNow, pixelValNext, pixelPosX, pixelPosY;    // gray-values at pixel pos
            if (scanPixelPos.Count <= 2)
                return;

            bool up = false;
            pixelValLast = -1; pixelValNow = pixelValNext = getPixelValue(scanPixelPos[0].X, scanPixelPos[0].Y, useZnotS);
            if (relative) { finalString.AppendLine("G91G1"); }
            for (int i=1; i < scanPixelPos.Count-1; i++)                
            {
                pixelPosX = scanPixelPos[i].X; pixelPosY = scanPixelPos[i].Y;
//                Logger.Trace("x:{0}  y:{1}",pixelPosX, pixelPosY);
                if ((pixelPosX < 0) || (pixelPosY < 0))
                {
                    if (!up)
                    {
                        gcode.PenUp(finalString);
                        cncCoordLastZ = cncCoordZ = gcode.gcodeZUp;
                    }
                    up = true;
                }
                else
                {
                    if (up)
                    {
                        cncCoordX = cncPixelResoX * (float)pixelPosX;
                        cncCoordY = cncPixelResoY * (float)pixelPosY;
                        if (relative) { finalString.AppendLine("G90"); }  // switch to absolute
                        gcode.MoveToRapid(finalString, cncCoordX, cncCoordY);
                        if (useZnotS)   // move servo down and delay
                        {
                            pixelValNow = pixelValNext = getPixelValue(pixelPosX, pixelPosY, useZnotS);
                            setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, false);
                        }
                        else
                        {
                            finalString.AppendFormat("G{0} X{1} Y{2} S{3}\r\n", gcode.frmtCode(1), gcode.frmtNum(cncCoordX), gcode.frmtNum(cncCoordY), Math.Round(nUDSMin.Value));
                            finalString.AppendFormat("G{0} P{1}\r\n", gcode.frmtCode(4), gcode.frmtNum(Properties.Settings.Default.importGCPWMDlyUp));
                        }
                        cncCoordLastX = cncCoordX;
                        cncCoordLastY = cncCoordY;
                        if (relative) { finalString.AppendLine("G91G1"); }
                    }

                    up = false;
                    pixelValNext = getPixelValue(scanPixelPos[i + 1].X, scanPixelPos[i + 1].Y, useZnotS);
                    if ((pixelValNow != pixelValLast) || (pixelValNow != pixelValNext))
                    {    setCommand(pixelPosX, pixelPosY, pixelValNow, useZnotS, relative);  }
                    else
                    {   setXYCommand(pixelPosX, pixelPosY, "", relative); }
                }
                pixelValLast = pixelValNow; pixelValNow = pixelValNext;
            }
        }

        private void createScanPath(int width, int height, float offsetX, float offsetY)
        {   scanPixelPos.Clear();
            Logger.Trace("createScanPath width:{0} height:{1}", width, height);
            int x,y,oldX=0,oldY=0;
            foreach(PointF pos in scanCNCPos)
            {   x = (int)(pos.X/ cncPixelResoX + offsetX);
                y = (int)(pos.Y/ cncPixelResoX + offsetY);
                if (pos.X == float.NaN) { Logger.Trace("NaN1 x:{0}  y:{1}",x,y); }
                if ((x != oldX) || (y != oldY))
                {   if ((x < 0) || (x >= width)) {x = -1;}
                    if ((y < 0) || (y >= height)) {y = -1;}
                    scanPixelPos.Add(new Point(x,y));
                }
                oldX = x; oldY = y;
                if (pos.X == float.NaN)
                { Logger.Trace("NaN2 x:{0}  y:{1}", pos.X, pos.Y); }
            }            
        }
        
        private void createSpiral(float distance, float step, float maxR)
        {
            scanCNCPos.Clear();
            Logger.Trace("createSpiral createSpiral:{0} step:{1} maxR:{2}", distance, step, maxR);
            if ((distance <=0) || (maxR <= distance))
                return;
            float r = 0,a = 0,x,y;
            float aDeg = 0, deltaDeg=1;
            while (r < maxR)
            {   x = (float)Math.Cos(a) * r;
                y = -(float)Math.Sin(a) * r;
                /*           scanCNCPos.Add(new PointF(x,y));
                           aDeg+=deltaDeg; r+=distance/360;                // 1 deg step, 1 distance/turn
                           a = (float)(aDeg * Math.PI / 180);         
                           deltaDeg = (float)(Math.Acos(step/r) * 180 / Math.PI);  */ // step width should be pixel distance
                scanCNCPos.Add(new PointF(x, y));
                deltaDeg = (float)(Math.Atan(step / r) * 180 / Math.PI);   // step width should be pixel distance
                aDeg += deltaDeg; r += distance * deltaDeg / 360;                // 1 deg step, 1 distance/turn
                a = (float)(aDeg * Math.PI / 180);

            }
        }
        
        private void createFrom2DView(float step)
        {
            scanCNCPos.Clear();
            Logger.Trace("createFrom2DView G2G3:{0}",VisuGCode.containsG2G3Command());

            // Export from VisuGCode.
            if (!VisuGCode.getPathCordinates(scanCNCPos, step))
            { }
        }

    }
}
