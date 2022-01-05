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
 * 2019-07-08 add xmlMarker.figureStart tag
 * 2020-09-24 change to PenColor, PenWith
 * 2021-04-15
 * 2021-07-02 code clean up / code quality
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GrblPlotter
{   public partial class GCodeFromImage
    {
        private static readonly Dictionary<int, StringBuilder> gcodeByToolNr = new Dictionary<int, StringBuilder>();
  //      private static int gcodeStringIndex = 0;
        private static readonly StringBuilder finalString = new StringBuilder();
  //      private static StringBuilder tmpString = new StringBuilder();

        private static float cncCoordX;   //X
        private static float cncCoordY;   //Y
        private static float cncCoordLastX;    //Last x/y  coords for compare
        private static float cncCoordLastY;
        private static float cncPixelResoX;    // resolution = distance between pixels / lines / columns
        private static float cncPixelResoY;

        private void GenerateGCodePreset(float startX, float startY)
        {   lblStatus.Text = "Generating GCode... ";    //Generate picture Gcode
            gcodeByToolNr.Clear();
            finalString.Clear();
            colorMap.Clear();
            colorStart = -2; colorEnd = -2; lastTool = -2; lastLine = -2;   // setColorMap startup
            Gcode.Setup(true);  // convertGraphics=true (repeat, inser sub)
            if (rBGrayS.Checked && cBLaserModeOnStart.Checked)
            {   finalString.AppendLine("$32=1 (Lasermode on)");  }
            Gcode.JobStart(finalString, "StartJob");
            Gcode.MoveToRapid(finalString, startX, startY,"");
            cncCoordLastZ = cncCoordZ = Gcode.GcodeZUp;
        }

        private void GenerateGCodeHorizontal()
        {
            Logger.Debug("generateGCodeHorizontal");
            if (resultImage == null) return;          //if no image, do nothing

            float resolX = (float)resoDesiredX;// (float)nUDReso.Value;
            float resolY = (float)resoDesiredX;// (float)nUDReso.Value;
            int pixelCount = resultImage.Width * resultImage.Height;
            int pixelProcessed = 0;
            int percentDone;// = 0;
            GenerateGCodePreset(0, resolY * (resultImage.Height - 1)); // 1st position

            //////////////////////////////////////////////
            // Generate Gcode lines by Horozontal scanning
            //////////////////////////////////////////////
            #region go through all pixels and setColorMap
            int positionY = resultImage.Height - 1;     //top tile
                int positionX = 0;                          //Left pixel
                while (positionY >= 0)
                {   //Y coordinate
                    cncCoordY = resolY * (float)positionY;
                    while (positionX < resultImage.Width)             //From left to right
                    {   //X coordinate
                        cncCoordX = resolX * (float)positionX;
                        SetColorMap(positionX, positionY);   // collect positions of same color
                        pixelProcessed++;
                        positionX++;
                    }
                    positionX--; positionY--; cncCoordY = resolY * (float)positionY;
                    while ((positionX >= 0) & (positionY >= 0))         //From right to left
                    {   //X coordinate
                        cncCoordX = resolX * (float)positionX;
                        SetColorMap(positionX, positionY);   // collect positions of same color
                        pixelProcessed++;
                        positionX--;
                    }
                    positionX++; positionY--;
                    percentDone = (pixelProcessed * 100) / pixelCount;
                    lblStatus.Text = "Generating GCode... " + Convert.ToString(percentDone) + "%";
                    if ((percentDone % 10) == 0)
                        Refresh();
                }
                if ((myToolNumber >= 0) && (colorMap.ContainsKey(myToolNumber)))
                    colorMap[myToolNumber][0].Add(colorEnd);

            #endregion

            imagegcode = "( Generated by GRBL-Plotter )\r\n";

   //         if (rBImportSVGTool.Checked)
   //             toolTable.sortByToolNr(false);
   //         else
            ToolTable.SortByPixelCount(false);      // sort by color area (max. first)

            Gcode.ReduceGCode = true;
            ConvertColorMap(resolX);                // generate GCode from coleccted pixel positions
            Gcode.PenUp(finalString, "");                             // pen up
            if (!gcodeSpindleToggle) Gcode.SpindleOff(finalString, "Stop spindle");
            imagegcode += Gcode.GetHeader("Image import", "") + finalString.Replace(',', '.').ToString() + Gcode.GetFooter();

            lblStatus.Text = "Done (" + Convert.ToString(pixelProcessed) + "/" + Convert.ToString(pixelCount) + ")";
            Refresh();
        }

        private static readonly Dictionary<int, List<int>[]> colorMap = new Dictionary<int, List<int>[]>();
        private static int colorStart = -2, colorEnd = -2, lastTool = -2, lastLine = -2;
        int myToolNumber = 0;
        /// <summary>
        /// colorMap stores x-start and x-stop values for each line(y) in an array (color)
        /// </summary>
        private void SetColorMap(int tmpX, int tmpY)
        {
            myToolNumber = resultToolNrArray[tmpX, (resultImage.Height -1)- tmpY];

            if ((myToolNumber >= 0) && (!colorMap.ContainsKey(myToolNumber)))   // if ToolNr unknown, create new buffer
            {   colorMap.Add(myToolNumber, new List<int>[resultImage.Height]);// add array with lines
                for (int i = 0; i < resultImage.Height; i++)
                { colorMap[myToolNumber][i] = new List<int>(); }              // add list to each line
            }

            if (lastTool != myToolNumber)                   // if new color is in use
            {   if (lastTool >= 0)
                    if (lastLine == tmpY)
                        colorMap[lastTool][tmpY].Add(colorEnd); // finish old color in same line
                    else if (lastLine >= 0)
                        colorMap[lastTool][lastLine].Add(colorEnd); // finish old color in last line
                colorStart = tmpX;
                colorEnd = tmpX;
                if (myToolNumber >= 0)                              // if regular toolNr
                    colorMap[myToolNumber][tmpY].Add(colorStart);   // start new color
            }
            else
            {   if ((lastLine != tmpY) && (lastLine >= 0) && (myToolNumber >= 0)) // still same color but line change
                { colorMap[myToolNumber][lastLine].Add(colorEnd); // finish old line
                    colorStart = tmpX;
                    colorMap[myToolNumber][tmpY].Add(colorStart);   // start new line
                }
                colorEnd = tmpX;    // still same line and same tool, just update end-pos
            }
            lastTool = myToolNumber;
            lastLine = tmpY;
        }

        /// <summary>
        /// process color map and generate gcode
        /// </summary>
        private void ConvertColorMap(float resol)
        {
            int toolNr, skipTooNr = 1;
            sbyte key;
            List<List<PointF>> outlineList;// = new List<List<Point>>();
            PointF tmpP;
            float resoOutlineX = (float)resoDesiredX;
            float resoOutlineY = (float)resoDesiredY;
            string tmp = "";
            int pathCount =0;
            
            for (int index = 0; index < toolTableCount; index++)  // go through lists
            {
                ToolTable.SetIndex(index);                      // set index in class
                key = (sbyte)ToolTable.IndexToolNR();           // if tool-nr == known key go on
                if (colorMap.ContainsKey(key))
                {   toolNr = key;                               // use tool in palette order
                    if (cbSkipToolOrder.Checked)
                        toolNr = skipTooNr++;                   // or start tool-nr at 1

                    finalString.AppendLine("\r\n( +++++ Tool change +++++ )");

                    Gcode.Tool(finalString, toolNr, ToolTable.IndexName());  // + svgPalette.pixelCount());
                    Gcode.Comment(finalString, string.Format("{0} Id=\"{1}\" PenColor=\"{2}\" PenName=\"{3}\" PenWidth=\"{4:0.00}\">", XmlMarker.FigureStart, (++pathCount), ColorTranslator.ToHtml(ToolTable.IndexColor()).Substring(1), ToolTable.IndexName(), ToolTable.IndexWidth()));		//toolTable.indexName()));
                    Gcode.ReduceGCode = false;
                    Gcode.PenUp(finalString," start ");
//                    gcode.MoveToRapid(finalString, 0, 0);          // move to start pos
                    Gcode.ReduceGCode = true;

                    tmp += ToolTable.IndexName() + "\r\n";

                    if (cBGCodeOutline.Checked)
                    {
                        int smoothCnt = (int)nUDGCodeOutlineSmooth.Value;
                        if (!cBGCodeOutlineSmooth.Checked)
                            smoothCnt = 0;
                        outlineList = Vectorize.GetPaths(resultToolNrArray, adjustedImage.Width, adjustedImage.Height, key, smoothCnt, (float)0.5/resoOutlineX, cBGCodeOutlineShrink.Checked);// half pen-width in pixels
                        int cnt = 0;
                        float tmpY;
                        foreach (List<PointF> path in outlineList)
                        {
                            if (path.Count > 0)
                            {   cnt++;
                                tmpP = path[0];
                                tmpY = (adjustedImage.Height - 1) - tmpP.Y; // start point
                                Gcode.Comment(finalString, string.Format("{0} Nr=\"{1}\">", XmlMarker.ContourStart, cnt));
                                Gcode.MoveToRapid(finalString, tmpP.X * resoOutlineX, tmpY * resoOutlineY, "");          // move to start pos
                                Gcode.PenDown(finalString, "");// " contour "+cnt);
                                foreach (PointF aP in path)
                                {
                                    tmpY = (adjustedImage.Height - 1) - aP.Y;
                                    Gcode.MoveTo(finalString, aP.X * resoOutlineX, tmpY * resoOutlineY, "");
                                }
                                tmpY = (adjustedImage.Height - 1) - tmpP.Y;
                                Gcode.PenUp(finalString,"");
                                Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.ContourEnd));
                            }
                        }
                        shrink = 0.4f;// 0.8f;          // shrink value in pixels!
                        if(cBGCodeOutlineShrink.Checked)
                            shrink = resoOutlineX*resoFactorX*1.2f;   // mm/px * factor * 1,6
                        tmp += "\r\nTool Nr "+ key + " Points: "+cnt+" \r\n"+Vectorize.logList.ToString();
                    }
                    else
                        shrink = 0;

                    if (cBGCodeFill.Checked)
                    {   int factor = resoFactorX;
                        int start = factor / 2;
                        // if (cBGCodeOutlineShrink.Checked)
                        //     start = factor; 
                        // if shrink, adapt start and stop posditions
                        Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.FillStart));
                        if (shrink > 0)
                        {
                            int pos1, pos2, min,max,minO,maxO, center;
                            int pxOffset = (int)(shrink / (float)resoDesiredX);
                            for (int y = start; y < resultImage.Height; y += factor)  // go through all lines
                            {   if (colorMap[key][y].Count > 1)
                                {   for (int k = 0; k < colorMap[key][y].Count; k += 2)
                                    {   pos1 = colorMap[key][y][k];
                                        pos2 = colorMap[key][y][k+1];
                                        min = Math.Min(pos1, pos2);
                                        max = Math.Max(pos1, pos2);
                                        minO = min + pxOffset;
                                        maxO = max - pxOffset;
                                        center = (min + max) / 2;
                                        if (minO < maxO)
                                        {   colorMap[key][y][k] = minO;
                                            colorMap[key][y][k+1] = maxO;
                                        }
                                        else if ((minO > max) || (maxO < min))
                                        {   colorMap[key][y][k] =-1;
                                            colorMap[key][y][k + 1] = -1;
                                        }
                                        else
                                        {   colorMap[key][y][k] = center;
                                            colorMap[key][y][k + 1] = center;
                                        }
                                    }
                                }
                            }
                        }

                        for (int y=start; y < resultImage.Height; y+= factor)  // go through all lines
                        {   while (colorMap[key][y].Count > 1)          // start at line 0 and check line by line
                                DrawColorMap(resol, key, y, 0, true);
                        }
                        Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.FillEnd));
                    }
                    Gcode.Comment(finalString, string.Format("{0}>", XmlMarker.FigureEnd));
                }
            }
//            System.Windows.Forms.Clipboard.SetText(tmp);
        }

        private static float shrink = 0.5f;
        /// <summary>
        /// check recursive line by line for same color near by, by given x-value
        /// </summary>
        private void DrawColorMap(float reso, int toolNr, int line, int startIndex, bool first)
        {
            int start, stop, newIndex;
            float coordY = reso * (float)line;
            int factor = resoFactorX;

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
           //     float tmpShrink = shrink;
           //     if (start > stop) tmpShrink = -shrink;
                float tmpShrink = 0;
                if ((start >= 0) && (stop >= 0) && (cBGCodeFill.Checked))
                {   float coordX = reso * (float)start + tmpShrink;
                    if (first)                                              // is this inital first call?
                    {   Gcode.MoveToRapid(finalString, coordX, coordY, "1st pos.");     // move to start pos
                        Gcode.PenDown(finalString, "");
                        first = false;
                    }
                    else
                        Gcode.MoveTo(finalString, coordX, coordY, "");          // move to start pos
                    coordX = reso * (float)stop - tmpShrink;
                    Gcode.MoveTo(finalString, coordX, coordY, "");              // move to end pos
                }
                if (line < (resultImage.Height - factor -1))
                {
                    var nextLine = colorMap[toolNr][line + factor];      // check for start-pos nearby in next line
                    bool end = true;
                    //int dir = Math.Sign(tmpShrink);
                    int sfactor = (int)(factor * 1.5);

                    if (start < stop)                                   // comming from left, search from rigth to left
                    {   for (int k = stop + sfactor; k >= start - sfactor; k--)
                        {   newIndex = nextLine.IndexOf(k);             // first check direction were I came from
                            if (newIndex >= 0)                          // entry found
                            {   DrawColorMap(reso, toolNr, line + factor, newIndex, first);  // go on with next line
                                end = false;
                                break;
                            }
                        }
                    } else
                    {                                                   // comming from right, search from left to right
                        for (int k = stop - sfactor; k <= start + sfactor; k++)
                        {   newIndex = nextLine.IndexOf(k);             // first check direction were I came from
                            if (newIndex >= 0)                          // entry found
                            {   DrawColorMap(reso, toolNr, line + factor, newIndex, first);  // go on with next line
                                end = false;
                                break;
                            }
                        }
                    }

                    if (end)
                    { Gcode.PenUp(finalString, " end1 "); }
                }
                else
                    Gcode.PenUp(finalString, " end2 ");
            }
        }


    }
}
