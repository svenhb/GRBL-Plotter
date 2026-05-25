/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2026-05-12 split from GraphicGenerateMisc.cs
*/

using GrblPlotter.Helper;
using GrblPlotter.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;

namespace GrblPlotter
{
    public static partial class Graphic
    {

        #region clipPath
        private static void ClipCode(double tileSizeX, double tileSizeY, double addOnX)//xyPoint p1, xyPoint p2)      // set dot only extra behandeln
        {	//const uint loggerSelect = (uint)LogEnable.ClipCode;
            double addOnY = addOnX;
            bool log = logEnable && ((logFlags & (uint)LogEnables.ClipCode) > 0);
            finalPathList = new List<PathObject>();     // figures of one tile
            tileGraphicAll = new List<PathObject>();    // figures of all tiles  // PathObject contains List<grblMotion>, PathInformation, pathStart, pathEnd, dimension
            Dimensions tileOneDimension = new Dimensions();

            ItemPath tilePath = new ItemPath();

            tiledGraphic.Clear();   // collect tileGraphicAll

            int tiledGraphicIndex = 0;

            Point pStart, pEnd;
            Point origStart, origEnd;

            Point clipMin = new Point(0, 0);
            Point clipMax = new Point(tileSizeX, tileSizeY);
            if (logEnable) Logger.Trace("...ClipCode Path min X:{0:0.0} Y:{1:0.0} max X:{2:0.0} Y:{3:0.0}  off X:{4:0.0} Y:{5:0.0} ##################################################################", clipMin.X, clipMin.Y, clipMax.X, clipMax.Y, actualDimension.minx, actualDimension.miny);

            /* rotate before clipping */
            if (Properties.Settings.Default.importGraphicClipAngleEnable)
                Rotate(completeGraphic, (double)Properties.Settings.Default.importGraphicClipAngle * Math.PI / 180, tileSizeX / 2, tileSizeY / 2);

            if (graphicInformation.OptionCodeOffset)
            {
                Logger.Info("...ClipCode Remove offset1, X: {0:0.000} , Y: {1:0.000}", actualDimension.minx, actualDimension.miny);
                SetHeaderInfo(string.Format(" Graphic offset: {0:0.00} {1:0.00} ", -actualDimension.minx, -actualDimension.miny));
                RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny);
            }

            //    if (log) ListGraphicObjects(completeGraphic);
            if (logEnable) Logger.Trace("....ClipCode GraphicObjects:{0} ", completeGraphic.Count);

            if ((tileSizeX <= 0) || (tileSizeY <= 0))
            {
                Logger.Error("  Tile size is <= 0  x:{0}  y:{1}", tileSizeX, tileSizeY);
                return;
            }

            int tilesX = (int)Math.Ceiling(actualDimension.maxx / tileSizeX);        // loop through all possible tiles
            int tilesY = (int)Math.Ceiling(actualDimension.maxy / tileSizeY);
            if (logEnable) Logger.Trace("....ClipCode Tiles X:{0} Y:{1}", tilesX, tilesY);
            int tileNr = 0;

            string tileID;
            string tileCommand;
            Point tileOffset = new Point();

            bool doTilingNotClipping = !Properties.Settings.Default.importGraphicClip;

            /* show cut lines in backgroud */
            float tmpX, tmpY;
            if (doTilingNotClipping)	// show cut lines in backgroud
            {
                for (int indexX = 0; indexX < tilesX; indexX++)
                {
                    pathBackground.StartFigure();
                    tmpX = (float)(indexX * tileSizeX);
                    pathBackground.AddLine(tmpX, 0, tmpX, (float)actualDimension.maxy);
                }
                for (int indexY = 0; indexY < tilesY; indexY++)
                {
                    pathBackground.StartFigure();
                    tmpY = (float)(indexY * tileSizeY);
                    pathBackground.AddLine(0, tmpY, (float)actualDimension.maxx, tmpY);
                }
            }
            else
            {
                pathBackground.StartFigure();
                tmpX = (float)Properties.Settings.Default.importGraphicClipOffsetX;
                tmpY = (float)Properties.Settings.Default.importGraphicClipOffsetY;
                System.Drawing.RectangleF pathRect = new System.Drawing.RectangleF(tmpX, tmpY, (float)tileSizeX, (float)tileSizeY);
                pathBackground.AddRectangle(pathRect);
                tilesX = tilesY = 1;		// not needed, a break is applied at the end of the loop
            }

            Matrix matrix = new Matrix();
            matrix.Scale(1, -1);

            int tileShowNr = 1;

            //	bool applyOffset = Properties.ListSettings.Default.importGraphicClipOffsetX;

            int clippedLines = 0;
            int clipCalls = 0;
            int clipOk = 0;

            for (int indexY = 0; indexY < tilesY; indexY++)
            {
                for (int indexX = 0; indexX < tilesX; indexX++)
                {
                    clipMin.X = indexX * tileSizeX - addOnX;
                    clipMin.Y = indexY * tileSizeY - addOnY;
                    clipMax.X = (indexX + 1) * tileSizeX + addOnX;
                    clipMax.Y = (indexY + 1) * tileSizeY + addOnY;
                    tileNr++;

                    //	if ()
                    tileOffset.X = indexX * tileSizeX;
                    tileOffset.Y = indexY * tileSizeY;

                    if (doTilingNotClipping)
                    {                                                       // Add micro-offset to avoid double consideration of points on clip-border
                        if (clipMin.X > 0) { clipMin.X -= 0.0001; }         // 2021-10-29 bugfix change from + to -
                        if (clipMin.Y > 0) { clipMin.Y -= 0.0001; }

                        pathBackground.StartFigure();
                        pathBackground.Transform(matrix);
                        float centerX = (float)((clipMax.X + clipMin.X) / 2);
                        float centerY = (float)((clipMax.Y + clipMin.Y) / 2);
                        float emSize = Math.Min((float)tileSizeX, (float)tileSizeY) / 3;
                        System.Drawing.StringFormat sFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault)
                        {
                            Alignment = System.Drawing.StringAlignment.Center,
                            LineAlignment = System.Drawing.StringAlignment.Center
                        };
                        System.Drawing.FontFamily myFont = new System.Drawing.FontFamily("Arial");
                        pathBackground.AddString((tileShowNr++).ToString(), myFont, (int)System.Drawing.FontStyle.Regular, emSize, new System.Drawing.PointF(centerX, -centerY), sFormat);
                        pathBackground.Transform(matrix);
                        myFont.Dispose();
                        sFormat.Dispose();
                    }
                    else
                    {
                        clipMin.X = (double)Properties.Settings.Default.importGraphicClipOffsetX;
                        clipMin.Y = (double)Properties.Settings.Default.importGraphicClipOffsetY;
                        clipMax.X = clipMin.X + tileSizeX;
                        clipMax.Y = clipMin.Y + tileSizeY;
                    }

                    tileID = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                    if (doTilingNotClipping) CountProperty((int)GroupOption.ByTile, tileID);
                    tileCommand = GetTileCommand(indexX, indexY, tileSizeX, tileSizeY);// new 2020-12-14
                    if ((indexX == 0) && (indexY == 0) && Properties.Settings.Default.importGraphicClipSkipCode)
                    { tileCommand = ""; }

                    if (log) Logger.Trace("New tile {0} iX:{1}  iY:{2} +++++++++++++++++++++++++++++++++++++++", tileID, indexX, indexY);
                    int foreachcnt = 1;

                    foreach (PathObject graphicItem in completeGraphic)
                    {
                        foreachcnt++;
                        if (!(graphicItem is ItemDot))
                        {
                            ItemPath item = (ItemPath)graphicItem;

                            if (item.Path.Count == 0)
                            { continue; }

                            if (!WithinRectangle(item.Dimension, clipMin, clipMax))
                            { continue; }

                            pStart = item.Path[0].MoveTo;
                            actualDashArray = new double[0];
                            if (item.DashArray.Length > 0)
                            {
                                actualDashArray = new double[item.DashArray.Length];
                                item.DashArray.CopyTo(actualDashArray, 0);
                            }

                            tilePath = new ItemPath(new Point(pStart.X, pStart.Y));
                            tilePath.Info.CopyData(graphicItem.Info);              // preset global info for GROUP
                                                                                   //               tilePath.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                            if (doTilingNotClipping) tilePath.Info.SetGroupAttribute((int)GroupOption.ByTile, tileID);
                            if (actualDashArray.Length > 0)
                            {
                                tilePath.DashArray = new double[actualDashArray.Length];
                                actualDashArray.CopyTo(tilePath.DashArray, 0);
                            }

                            for (int i = 1; i < item.Path.Count; i++)           // go through path objects { lineStart=0, line=1, dot=2, arcCW=3, arcCCW=4 }
                            {
                                //                        if (loggerTrace) Logger.Trace(" foreach i {0} of {1}", i, item.path.Count);
                                origStart = item.Path[i - 1].MoveTo;
                                pStart = new Point(origStart.X, origStart.Y);
                                origEnd = item.Path[i].MoveTo;
                                pEnd = new Point(origEnd.X, origEnd.Y);

                                clipCalls++;
                                //    Logger.Trace("call ClipLine {0}  x1:{1:0.0} y1:{2:0.0}  x2:{3:0.0} y2:{4:0.0}  xMin:{5:0.0} yMin:{6:0.0}  xMax:{7:0.0} yMax:{8:0.0}", clipCalls, pStart.X, pStart.Y, pEnd.X, pEnd.Y, clipMin.X, clipMin.Y, clipMax.X, clipMax.Y);

                                if (ClipLine(ref pStart, ref pEnd, clipMin, clipMax))   // true: line needs to be clipped
                                {
                                    clipOk++;
                                    if (origStart != pStart)		// path was clipped, store old path, start new path, becaue pen-up/down is needed
                                    {
                                        if (tilePath.Path.Count > 1)
                                            finalPathList.Add(tilePath);                   	    // save prev path
                                        tilePath = new ItemPath(new Point(pStart.X, pStart.Y)); // start new path with clipped start position
                                        tilePath.Info.CopyData(graphicItem.Info);               // preset global info for GROUP
                                                                                                //                           tilePath.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                                        if (doTilingNotClipping) tilePath.Info.SetGroupAttribute((int)GroupOption.ByTile, tileID);// string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
                                        if (actualDashArray.Length > 0)
                                        {
                                            tilePath.DashArray = new double[actualDashArray.Length];
                                            actualDashArray.CopyTo(tilePath.DashArray, 0);
                                        }
                                    }

                                    if (pStart != pEnd)             // avoid zero length path
                                    {
                                        tilePath.Add(pEnd, item.Path[i].Depth, 0); //Logger.Trace("   start!=end ID:{0} i:{1} at end start x:{2} y:{3}  end x:{4} y:{5}", item.Info.id, i, pStart.X, pStart.Y, pEnd.X, pEnd.Y);
                                        clippedLines++;
                                    }
                                    if (origEnd != pEnd)			// path was clipped, store old path, start new path, becaue pen-up/down is needed
                                    {
                                        if (tilePath.Path.Count > 1)
                                        { finalPathList.Add(tilePath); tilePath = new ItemPath(); } // save prev path, clipped end position already added     

                                        if (pStart != pEnd)		    // clipped path has a length, start new...
                                        {
                                            tilePath = new ItemPath();
                                            tilePath.Info.CopyData(graphicItem.Info);               // preset global info for GROUP
                                                                                                    //							tilePath.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                                            if (doTilingNotClipping) tilePath.Info.SetGroupAttribute((int)GroupOption.ByTile, tileID);  // string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
                                            if (actualDashArray.Length > 0)
                                            {
                                                tilePath.DashArray = new double[actualDashArray.Length];
                                                actualDashArray.CopyTo(tilePath.DashArray, 0);
                                            }
                                        }
                                    }
                                }
                            }
                            if (tilePath.Path.Count > 1)
                                finalPathList.Add(tilePath);
                        }
                        else  // isDot
                        {
                            if (WithinRectangle(graphicItem.Start, clipMin, clipMax))        // else must be Dot
                            {
                                ItemDot dot = new ItemDot(graphicItem.Start.X, graphicItem.Start.Y);
                                dot.Info.CopyData(graphicItem.Info);              // preset global info for GROUP
                                                                                  //                dot.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                                dot.Info.SetGroupAttribute((int)GroupOption.ByTile, string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
                                finalPathList.Add(dot);
                            }
                        }
                    }
                    tileOneDimension.AddDimensionXY(tilePath.Dimension);

                    //    if (log) ListGraphicObjects(finalPathList);
                    if (logEnable) Logger.Trace("....ClipCode finalPathList:{0} ", finalPathList.Count);

                    if (Properties.Settings.Default.importGraphicClipOffsetApply)
                    {
                        Logger.Info("...ClipCode Remove offset2, X: {0:0.000} , Y: {1:0.000}", clipMin.X, clipMin.Y);
                        RemoveOffset(finalPathList, clipMin.X, clipMin.Y);
                    }

                    SortByDistance(finalPathList, new Point(0, 0), graphicInformation.OptionCodeSortDistanceNewStartOnClosedPath, graphicInformation.OptionCodeSortDistanceLargestLast, false);      // ClipCode - sort objects of current tile

                    tiledGraphic.Add(new TileObject(tileID, tileCommand, tileOffset));          // new 2020-12-14
                    foreach (PathObject tile in finalPathList)      // add tile to full graphic
                    {
                        tileGraphicAll.Add(tile);
                        tiledGraphic[tiledGraphicIndex].GroupPath.Add(tile);// new 2020-12-14
                    }
                    tiledGraphicIndex++;// new 2020-12-14

                    finalPathList = new List<PathObject>();		// 

                    if (Properties.Settings.Default.importGraphicClip)	// just clipping one tile: stop X
                        break;
                }	// for X
                if (Properties.Settings.Default.importGraphicClip)		// just clipping one tile: stop Y
                    break;
            }   // for Y

            if (logEnable) Logger.Trace("....ClipCode clipCalls:{0}  clipOk:{1}  clippedLines:{2}", clipCalls, clipOk, clippedLines);

            completeGraphic.Clear();
            matrix.Dispose();
            foreach (PathObject tile in tileGraphicAll)     // add tile to full graphic
                completeGraphic.Add(tile);
        }

        private static void GroupTileContent(GraphicInformationClass graphicInformation)
        {
            Logger.Info("►►► GroupTileContent Count:{0} ", tiledGraphic.Count);
            foreach (TileObject tileObject in tiledGraphic)
            {
                if (GroupAllGraphics(tileObject.GroupPath, tileObject.Tile, graphicInformation))
                { }// tileObject.tile.Add( new TileObject(groupedGraphic)); }                
                else
                { tileObject.Tile.Clear(); }
                groupedGraphic.Clear();
            }
        }

        private static string GetTileCommand(int indexX, int indexY, double sizeX, double sizeY)
        {
            string command = Properties.Settings.Default.importGraphicClipGCode;
            if (command.Contains("#INDX")) command = command.Replace("#INDX", string.Format("{0}", indexX));    // index X
            if (command.Contains("#INDY")) command = command.Replace("#INDY", string.Format("{0}", indexY));    // index Y
            if (command.Contains("#OFFX")) command = command.Replace("#OFFX", string.Format("{0:0.000}", (indexX * sizeX)));    // offset X
            if (command.Contains("#OFFY")) command = command.Replace("#OFFY", string.Format("{0:0.000}", (indexY * sizeY)));	// offset X	
            if ((logFlags & (uint)LogEnables.ClipCode) > 0) Logger.Trace("   getTileCommand {0}", command);

            return command;
        }

        private static bool WithinRectangle(Point a, Point min, Point max)
        {
            if (a.X < min.X) return false;
            if (a.X > max.X) return false;
            if (a.Y < min.Y) return false;
            if (a.Y > max.Y) return false;
            return true;
        }
        private static bool WithinRectangle(Dimensions a, Point min, Point max)
        {
            if (a.maxx < min.X) return false;
            if (a.minx > max.X) return false;
            if (a.maxy < min.Y) return false;
            if (a.miny > max.Y) return false;
            return true;
        }

        // adaption of code example: https://de.wikipedia.org/wiki/Algorithmus_von_Cohen-Sutherland
        private enum ClipType { CLIPLEFT = 1, CLIPRIGHT = 2, CLIPLOWER = 4, CLIPUPPER = 8 };  //        
        private static bool ClipLine(ref Point p1, ref Point p2, Point clipMin, Point clipMax)
        {
            int K1 = 0, K2 = 0;             // Variablen mit je 4 Flags für rechts, links, oben, unten.
            double dx, dy;

            dx = p2.X - p1.X;                 // Die Breite der Linie vorberechnen für spätere Koordinaten Interpolation
            dy = p2.Y - p1.Y;                 // Die Höhe   der Linie vorberechnen für spätere Koordinaten Interpolation

            if (p1.Y < clipMin.Y) K1 = (int)ClipType.CLIPLOWER;  // Ermittle, wo der Anfangspunkt außerhalb des Clip Rechtecks liegt.
            if (p1.Y > clipMax.Y) K1 = (int)ClipType.CLIPUPPER;  // Es werden entweder CLIPLOWER oder CLIPUPPER und/oder CLIPLEFT oder CLIPRIGHT gesetzt
            if (p1.X < clipMin.X) K1 |= (int)ClipType.CLIPLEFT;   // Es gibt 8 zu clippende Kombinationen, je nachdem in welchem der 8 angrenzenden
            if (p1.X > clipMax.X) K1 |= (int)ClipType.CLIPRIGHT;  // Bereiche des Clip Rechtecks die Koordinate liegt.

            if (p2.Y < clipMin.Y) K2 = (int)ClipType.CLIPLOWER;  // Ermittle, wo der Endpunkt außerhalb des Clip Rechtecks liegt.
            if (p2.Y > clipMax.Y) K2 = (int)ClipType.CLIPUPPER;  // Wenn keines der Flags gesetzt ist, dann liegt die Koordinate
            if (p2.X < clipMin.X) K2 |= (int)ClipType.CLIPLEFT;   // innerhalb des Rechtecks und muss nicht geändert werden.
            if (p2.X > clipMax.X) K2 |= (int)ClipType.CLIPRIGHT;

            // Schleife nach Cohen Sutherland, die maximal zweimal durchlaufen wird
            //  bool pEndChanged = K2 > 0;
            while ((K1 > 0) || (K2 > 0))    // muss wenigstens eine der Koordinaten irgendwo geclippt werden ?
            {
                //        Logger.Trace(".....ClipLine K1:{0}  K2:{1}",K1,K2);
                if ((K1 & K2) > 0)     // logisches AND der beiden Koordinaten Flags ungleich 0 ?
                    return false;  // beide Punkte liegen jeweils auf der gleichen Seite außerhalb des Rechtecks

                if (K1 > 0)                       // schnelle Abfrage, muss Koordinate 1 geclippt werden ?
                {
                    if ((K1 & (int)ClipType.CLIPLEFT) > 0)          // ist Anfangspunkt links außerhalb ?
                    { p1.Y += (clipMin.X - p1.X) * dy / dx; p1.X = clipMin.X; }
                    else if ((K1 & (int)ClipType.CLIPRIGHT) > 0)   // ist Anfangspunkt rechts außerhalb ?
                    { p1.Y += (clipMax.X - p1.X) * dy / dx; p1.X = clipMax.X; }
                    if ((K1 & (int)ClipType.CLIPLOWER) > 0)        // ist Anfangspunkt unterhalb des Rechtecks ?
                    { p1.X += (clipMin.Y - p1.Y) * dx / dy; p1.Y = clipMin.Y; }
                    else if ((K1 & (int)ClipType.CLIPUPPER) > 0)   // ist Anfangspunkt oberhalb des Rechtecks ?
                    { p1.X += (clipMax.Y - p1.Y) * dx / dy; p1.Y = clipMax.Y; }
                    K1 = 0;                    // Erst davon ausgehen, dass der Punkt jetzt innerhalb liegt,
                                               // falls das nicht zutrifft, wird gleich korrigiert.
                    if (p1.Y < clipMin.Y) K1 = (int)ClipType.CLIPLOWER; // 4 ermittle erneut, wo der Anfangspunkt jetzt außerhalb des Clip Rechtecks liegt
                    if (p1.Y > clipMax.Y) K1 = (int)ClipType.CLIPUPPER; // 8 Für einen Punkt, bei dem im ersten Durchlauf z.&nbsp;B. CLIPLEFT gesetzt war,
                    if (p1.X < clipMin.X) K1 |= (int)ClipType.CLIPLEFT;  // 1 kann im zweiten Durchlauf z.&nbsp;B. CLIPLOWER gesetzt sein
                    if (p1.X > clipMax.X) K1 |= (int)ClipType.CLIPRIGHT; // 2
                }

                if ((K1 & K2) > 0)     // logisches AND der beiden Koordinaten Flags ungleich 0 ?
                    return false;  // beide Punkte liegen jeweils auf der gleichen Seite außerhalb des Rechtecks

                if (K2 > 0)                       // schnelle Abfrage, muss Koordinate 2 geclippt werden ?
                {                            // ja
                    if ((K2 & (int)ClipType.CLIPLEFT) > 0)         // liegt die Koordinate links außerhalb des Rechtecks ?
                    { p2.Y += (clipMin.X - p2.X) * dy / dx; p2.X = clipMin.X; }
                    else if ((K2 & (int)ClipType.CLIPRIGHT) > 0)   // liegt die Koordinate rechts außerhalb des Rechtecks ?
                    { p2.Y += (clipMax.X - p2.X) * dy / dx; p2.X = clipMax.X; }
                    if ((K2 & (int)ClipType.CLIPLOWER) > 0)        // liegt der Endpunkt unten außerhalb des Rechtecks ?
                    { p2.X += (clipMin.Y - p2.Y) * dx / dy; p2.Y = clipMin.Y; }
                    else if ((K2 & (int)ClipType.CLIPUPPER) > 0)    // liegt der Endpunkt oben außerhalb des Rechtecks ?
                    { p2.X += (clipMax.Y - p2.Y) * dx / dy; p2.Y = clipMax.Y; }
                    K2 = 0;                    // Erst davon ausgehen, dass der Punkt jetzt innerhalb liegt,
                                               // falls das nicht zutrifft, wird gleich korrigiert.
                    if (p2.Y < clipMin.Y) K2 = (int)ClipType.CLIPLOWER; // ermittle erneut, wo der Endpunkt jetzt außerhalb des Clip Rechtecks liegt
                    if (p2.Y > clipMax.Y) K2 = (int)ClipType.CLIPUPPER; // Ein Punkt, der z.&nbsp;B. zuvor über dem Clip Rechteck lag, kann jetzt entweder
                    if (p2.X < clipMin.X) K2 |= (int)ClipType.CLIPLEFT;  // rechts oder links davon liegen. Wenn er innerhalb liegt wird kein
                    if (p2.X > clipMax.X) K2 |= (int)ClipType.CLIPRIGHT; // Flag gesetzt.
                }
            }             // Ende der while Schleife, die Schleife wird maximal zweimal durchlaufen.
            return true;  // jetzt sind die Koordinaten geclippt und die gekürzte Linie kann gezeichnet werden
        }
        #endregion

	}
}