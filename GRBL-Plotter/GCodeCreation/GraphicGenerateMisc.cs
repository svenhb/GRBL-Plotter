/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2021 Sven Hasemann contact: svenhb@web.de

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
 * 2021-09-10 split from graphicRelated
 * 2021-10-29 bugfix in RemoveIntermediateSteps
 * 2021-11-16 bugfix RemoveIntermediateSteps, RemoveShortMoves
*/

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
        // create GraphicsPath for 2-D view
        private static void CreateGraphicsPath(List<PathObject> graphicToDraw, GraphicsPath path)
        {
            if (logEnable) Logger.Trace("...createGraphicsPath of original graphic (background)");
            stopwatch = new Stopwatch();
            stopwatch.Start();
            int itemcount = 0;
            int maxcount = completeGraphic.Count;

            foreach (PathObject item in graphicToDraw)    // dot or path
            {
                if (item is ItemPath PathData)
                {
                    //ItemPath PathData = (ItemPath)item;
                    if (logDetailed) Logger.Trace("   createGraphicsPath - StartFigure id:{0} geo:{1}", item.Info.Id, item.Info.PathGeometry);
                    path.StartFigure();

                    if (PathData.Path.Count > 1)
                    {
                        for (int i = 1; i < PathData.Path.Count; i++)
                        {
                            if (backgroundWorker != null)
                            {
                                if (UpdateGUI()) backgroundWorker.ReportProgress((itemcount++ * 100) / maxcount);
                                if (backgroundWorker.CancellationPending)
                                {
                                    cancelByWorker = true;
                                    break;
                                }
                            }

                            if (PathData.Path[i] is GCodeLine)
                            {
                                if (logDetailed) Logger.Trace("    createGraphicsPath - AddLine {0,7:0.00} {1,7:0.00}  {2,7:0.00} {3,7:0.00}", (float)PathData.Path[i - 1].MoveTo.X, (float)PathData.Path[i - 1].MoveTo.Y, (float)PathData.Path[i].MoveTo.X, (float)PathData.Path[i].MoveTo.Y);
                                try
                                { path.AddLine((float)PathData.Path[i - 1].MoveTo.X, (float)PathData.Path[i - 1].MoveTo.Y, (float)PathData.Path[i].MoveTo.X, (float)PathData.Path[i].MoveTo.Y); }
                                catch (Exception err)
                                {
                                    Logger.Error(err, "....CreateGraphicsPath path.AddLine, skip AddLine! "); // probably happens during extensive logging
                                    break;  // try to go on
                                }
                            }
                            else
                            {
                                GCodeArc arcPath = (GCodeArc)PathData.Path[i];
                                ArcProperties arcMove;
                                arcMove = GcodeMath.GetArcMoveProperties((XyPoint)PathData.Path[i - 1].MoveTo, (XyPoint)arcPath.MoveTo, arcPath.CenterIJ.X, arcPath.CenterIJ.Y, arcPath.IsCW);
                                float x1 = (float)(arcMove.center.X - arcMove.radius);
                                // float x2 = (float)(arcMove.center.X + arcMove.radius);
                                float y1 = (float)(arcMove.center.Y - arcMove.radius);
                                // float y2 = (float)(arcMove.center.Y + arcMove.radius);
                                float r2 = 2 * (float)arcMove.radius;
                                float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
                                float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);
                                if (arcMove.radius > equalPrecision)
                                {
                                    if (logDetailed) Logger.Trace("    createGraphicsPath - AddArc  {0,7:0.00} {1,7:0.00}  {2,7:0.00} {3,7:0.00} {4:0.00} {5:0.00}", x1, y1, r2, r2, aStart, aDiff);
                                    try
                                    { path.AddArc(x1, y1, r2, r2, aStart, aDiff); }
                                    catch (Exception err)
                                    {
                                        Logger.Error(err, "....CreateGraphicsPath path.AddArc, skip AddArc! ");   // probably happens during extensive logging										
                                        break;  // try to go on
                                    }
                                }
                                else
                                {
                                    Logger.Error("....createGraphicsPath radius too small r:{0} - convert arc to line  id:{1} lastX:{2:0.00} lastY:{3:0.00} newX:{4:0.00} newY:{5:0.00} I:{6:0.00} J:{7:0.00}", arcMove.radius, item.Info.Id, PathData.Path[i - 1].MoveTo.X, PathData.Path[i - 1].MoveTo.Y, arcPath.MoveTo.X, arcPath.MoveTo.Y, arcPath.CenterIJ.X, arcPath.CenterIJ.Y);
                                    // replace arc by line
                                    PathData.Path[i] = new GCodeLine(arcPath.MoveTo);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (logDetailed) Logger.Trace("    createGraphicsPath - AddDot {0:0.00} {1:0.00} ", item.Start.X, item.Start.Y);
                    path.StartFigure();
                    path.AddLine((float)item.Start.X - (float)equalPrecision, (float)item.Start.Y, (float)item.Start.X + (float)equalPrecision, (float)item.Start.Y);
                } // is Dot
            }
        }


        private static bool GroupAllGraphics(List<PathObject> completeGraphic, List<GroupObject> groupedGraphicLocal, GraphicInformationClass graphicInformation, bool preventReversal = false)
        {
            //  groupedGraphicLocal = new List<GroupObject>();   // local 2020-12-14
            //  groupedGraphicLocal = groupedGraphic;
            bool log = logEnable && ((logFlags & (uint)LogEnables.GroupAllGraphics) > 0);
            keyToIndex = new Dictionary<string, int>();
            string tmpKey = "";
            int groupCount = -1;
            int toolNr = (int)Properties.Settings.Default.importGCToolDefNr;
            string toolName = "";

            if (logEnable) Logger.Trace("...GroupAllGraphics by:{0}  ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++", graphicInformation.GroupOption.ToString());
            if (logEnable)
            {
                foreach (GroupOption tmp in (GroupOption[])Enum.GetValues(typeof(GroupOption)))
                { Logger.Trace("   Property count {0} {1}", tmp, groupPropertiesCount[(int)tmp].Count); }
            }

            int differentGroupValues = groupPropertiesCount[(int)graphicInformation.GroupOption].Count;     // 2020-08-11
            if (differentGroupValues < 2)   // too less values to group
            {   //SetHeaderInfo(" Too less values to group ");
                Logger.Info("---- Group members:{0} group by:{1}", differentGroupValues, graphicInformation.GroupOption);
                graphicInformation.GroupEnable = false; // disable in case of ReDoReversePath
                                                        //				return false;   2021-06-03
            }

            foreach (PathObject pathObject in completeGraphic)
            {
                if (pathObject is ItemPath apath)
                {
                    if (apath.Path.Count <= 0)
                    {
                        continue;
                    }
                }
                tmpKey = pathObject.Info.GroupAttributes[(int)graphicInformation.GroupOption];      // get value to group by GroupOption-key

                if (log) Logger.Trace("    GroupAll {0} type:{1} count:{2}  key:'{3}'", pathObject.Info.Id, pathObject.GetType().ToString(), groupCount, tmpKey);

                if (keyToIndex.ContainsKey(tmpKey))
                {
                    if (groupCount < groupedGraphicLocal.Count)
                    {
                        groupedGraphicLocal[keyToIndex[tmpKey]].AddInfo(pathObject);        // add length, dimension, area
                        groupedGraphicLocal[keyToIndex[tmpKey]].GroupPath.Add(pathObject);
                    }
                    if (log) Logger.Trace("     Add {0} to key:'{1}'", pathObject.Info.Id, tmpKey);
                }
                else
                {
                    groupCount++;
                    keyToIndex.Add(tmpKey, groupCount);

                    switch (graphicInformation.GroupOption)
                    {
                        case GroupOption.ByColor:
                            toolNr = ToolTable.GetToolNRByToolColor(pathObject.Info.GroupAttributes[(int)GroupOption.ByColor], 0);
                            toolName = ToolTable.GetToolName(toolNr);
                            break;
                        case GroupOption.ByWidth:
                            toolNr = ToolTable.GetToolNRByToolDiameter(pathObject.Info.GroupAttributes[(int)GroupOption.ByWidth]);
                            toolName = ToolTable.GetToolName(toolNr);
                            break;
                        case GroupOption.ByLayer:
                            toolNr = ToolTable.GetToolNRByToolName(pathObject.Info.GroupAttributes[(int)GroupOption.ByLayer]);
                            toolName = ToolTable.GetToolName(toolNr);
                            break;
                        default: break;
                    }

                    tmpGroup = new GroupObject(tmpKey, toolNr, toolName, pathObject);
                    groupedGraphicLocal.Add(tmpGroup);
                    if (log) Logger.Trace("     Create {0} new key:'{1}'", pathObject.Info.Id, tmpKey);
                }
            }

            if (differentGroupValues > 1)	// 2021-06-03
            {
                // Sort Groups by		
                bool invert = Properties.Settings.Default.importGroupSortInvert;
                if (graphicInformation.SortOption != SortOption.none)       // 		public enum SortOption  { none=0, ByToolNr=1, ByCodeSize=2, ByGraphicDimension=3};
                {
                    if (log) Logger.Trace("    Sort by {0}", graphicInformation.SortOption);
                    switch (graphicInformation.SortOption)
                    {
                        case SortOption.ByProperty:
                            if (!invert)
                                groupedGraphicLocal.Sort((a, b) => a.Key.CompareTo(b.Key));
                            else
                                groupedGraphicLocal.Sort((a, b) => b.Key.CompareTo(a.Key));
                            break;
                        case SortOption.ByToolNr:
                            if (!invert)
                                groupedGraphicLocal.Sort((a, b) => a.ToolNr.CompareTo(b.ToolNr));
                            else
                                groupedGraphicLocal.Sort((a, b) => b.ToolNr.CompareTo(a.ToolNr));
                            break;
                        case SortOption.ByCodeSize:
                            if (invert)
                                groupedGraphicLocal.Sort((a, b) => a.PathLength.CompareTo(b.PathLength));
                            else
                                groupedGraphicLocal.Sort((a, b) => b.PathLength.CompareTo(a.PathLength));
                            break;
                        case SortOption.ByGraphicDimension:
                            if (invert)
                                groupedGraphicLocal.Sort((a, b) => a.PathArea.CompareTo(b.PathArea));
                            else
                                groupedGraphicLocal.Sort((a, b) => b.PathArea.CompareTo(a.PathArea));
                            break;
                        default:
                            break;
                    }
                }

                // Sort paths in group by distance		
                if (Properties.Settings.Default.importGraphicSortDistance)
                {
                    foreach (GroupObject groupObject in groupedGraphicLocal)
                    {
                        if (groupObject.GroupPath.Count > 1)
                            SortByDistance(groupObject.GroupPath, preventReversal);
                    }
                }
            }

            if (log)
            {
                Logger.Trace(" Grouping result +++++++++++++++++++++++++++++++++++++++++++++++++");
                int i = 0;
                foreach (GroupObject groupObject in groupedGraphicLocal)
                {
                    Logger.Trace("  GroupCount i:'{0}' key:'{1}' toolNr:'{2}' toolName:'{3}' pathLength:{4:0.2}", i++, groupObject.Key, groupObject.ToolNr, groupObject.ToolName, groupObject.PathLength);
                    ListGraphicObjects(groupObject.GroupPath);
                }
            }

            return true;
        }

        private static void ListGraphicObjects(List<PathObject> graphicToShow, bool showCoord = false)
        {
            if (logEnable) Logger.Trace("  ListGraphicObjects Count:{0}  ##########################################", graphicToShow.Count);
            int cnt;
            string coordByLine;
            string info;
            double length;
            bool reversed = false;

            foreach (PathObject graphicItem in graphicToShow)
            {
                if (graphicItem is ItemPath apath)
                {
                    cnt = apath.Path.Count;
                    length = graphicItem.PathLength;
                    reversed = apath.Reversed;
                }
                else
                {
                    cnt = 1;
                    length = 0;
                }
                coordByLine = string.Format("x1:{0,6:0.00} y1:{1,6:0.00}  x2:{2,6:0.00} y2:{3,6:0.00}  reversed:{4}", graphicItem.Start.X, graphicItem.Start.Y, graphicItem.End.X, graphicItem.End.Y, reversed);
                info = string.Format(" color:'{0}' width:'{1}' layer:'{2}' length:'{3:0.00}'", graphicItem.Info.GroupAttributes[1], graphicItem.Info.GroupAttributes[2], graphicItem.Info.GroupAttributes[3], length);   // index 0 not used
                if (logEnable) Logger.Trace("    graphicItem Id:{0,3} pathId:{1,-12} Geo:{2,-8}  {3}   Points:{4,3}  Coord:{5}", graphicItem.Info.Id, graphicItem.Info.PathId, graphicItem.Info.PathGeometry, info, cnt, coordByLine);

                if (logCoordinates)
                {
                    if (showCoord && (graphicItem is ItemPath bpath))
                    {
                        foreach (GCodeMotion ent in bpath.Path)
                        { Logger.Trace("       X:{0:0.00} Y:{1:0.00} Z:{2:0.00}  Line:{3} ", ent.MoveTo.X, ent.MoveTo.Y, ent.Depth, (ent is GCodeLine).ToString()); }
                    }
                }
            }
            if (logEnable) Logger.Trace("  ListGraphicObjects          ##########################################", graphicToShow.Count);
        }

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
            if (logEnable) Logger.Trace("...ClipCode Path min X:{0:0.0} Y:{1:0.0} max X:{2:0.0} Y:{3:0.0} ##################################################################", clipMin.X, clipMin.Y, clipMax.X, clipMax.Y);

            RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny);

            if (log) ListGraphicObjects(completeGraphic);

            if ((tileSizeX <= 0) || (tileSizeY <= 0))
            {
                Logger.Error("  Tile size is <= 0  x:{0}  y:{1}", tileSizeX, tileSizeY);
                return;
            }

            int tilesX = (int)Math.Ceiling(actualDimension.maxx / tileSizeX);        // loop through all possible tiles
            int tilesY = (int)Math.Ceiling(actualDimension.maxy / tileSizeY);
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
            }

            Matrix matrix = new Matrix();
            matrix.Scale(1, -1);

            int tileShowNr = 1;

            //	bool applyOffset = Properties.Settings.Default.importGraphicClipOffsetX;

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
                    CountProperty((int)GroupOption.ByTile, tileID);
                    tileCommand = GetTileCommand(indexX, indexY, tileSizeX, tileSizeY);// new 2020-12-14
                    if ((indexX == 0) && (indexY == 0) && Properties.Settings.Default.importGraphicClipSkipCode)
                    { tileCommand = ""; }

                    if (log) Logger.Trace("New tile {0} +++++++++++++++++++++++++++++++++++++++", tileID);
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
                            tilePath.Info.SetGroupAttribute((int)GroupOption.ByTile, tileID);
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

                                if (ClipLine(ref pStart, ref pEnd, clipMin, clipMax))   // true: line needs to be clipped
                                {
                                    if (origStart != pStart)		// path was clipped, store old path, start new path, becaue pen-up/down is needed
                                    {
                                        if (tilePath.Path.Count > 1)
                                            finalPathList.Add(tilePath);                   	    // save prev path
                                        tilePath = new ItemPath(new Point(pStart.X, pStart.Y)); // start new path with clipped start position
                                        tilePath.Info.CopyData(graphicItem.Info);               // preset global info for GROUP
                                                                                                //                           tilePath.Info.clipInfo = string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY);
                                        tilePath.Info.SetGroupAttribute((int)GroupOption.ByTile, tileID);// string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
                                        if (actualDashArray.Length > 0)
                                        {
                                            tilePath.DashArray = new double[actualDashArray.Length];
                                            actualDashArray.CopyTo(tilePath.DashArray, 0);
                                        }
                                    }

                                    if (pStart != pEnd)             // avoid zero length path
                                    {
                                        tilePath.Add(pEnd, item.Path[i].Depth, 0); //Logger.Trace("   start!=end ID:{0} i:{1} at end start x:{2} y:{3}  end x:{4} y:{5}", item.Info.id, i, pStart.X, pStart.Y, pEnd.X, pEnd.Y);
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
                                            tilePath.Info.SetGroupAttribute((int)GroupOption.ByTile, tileID);  // string.Format("{0}_X{1}_Y{2}", tileNr, indexX, indexY));
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
                        else
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

                    if (log) ListGraphicObjects(finalPathList);

                    if (Properties.Settings.Default.importGraphicClipOffsetApply)
                        RemoveOffset(finalPathList, clipMin.X, clipMin.Y);

                    SortByDistance(finalPathList);                 // sort objects of current tile

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
            }	// for Y
            completeGraphic.Clear();
            matrix.Dispose();
            foreach (PathObject tile in tileGraphicAll)     // add tile to full graphic
                completeGraphic.Add(tile);
        }

        private static void GroupTileContent(GraphicInformationClass graphicInformation)
        {
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


        #region remove short moves

        private static void RemoveIntermediateSteps(List<PathObject> graphicToImprove)
        {
            int removed = 0;
            foreach (PathObject item in graphicToImprove)    // dot or path
            {
                if (item is ItemPath PathData)
                {
                    //ItemPath PathData = (ItemPath)item;
                    Point lastPoint = PathData.End;
                    double angleNow = 0;                       // when adjacent line segments have the same angle - end point of first can be removed 
                    double angleLast = 0;
                    bool isLineNow = false;
                    bool isLineLast = false;
					bool removeMinDistance;
                    if (PathData.Path.Count > 2)
                    {
                        for (int i = (PathData.Path.Count - 2); i >= 0; i--)
                        {
                            if (PathData.Path[i] is GCodeLine)
                            {
                                angleNow = GcodeMath.GetAlpha(PathData.Path[i].MoveTo, lastPoint);
                                PathData.Path[i].Angle = angleNow;
                                isLineNow = true;
                            }
                            else
                            {
                                PathData.Path[i].Angle = angleNow;
                                angleNow = angleLast + 1;
                                isLineNow = false;
                            }

                            if (isLineNow && isLineLast && IsEqual(angleNow, angleLast))    
                            {
                                if (((i + 2) < PathData.Path.Count) && (PathData.Path[i + 2] is GCodeLine))	// don't delete if start-point for arc
                                {
                                    PathData.Path.RemoveAt(i + 1);
                                    removed++;
                                }
                            }
                            else
                            {
                                angleLast = angleNow;
                            }
                            lastPoint = PathData.Path[i].MoveTo;
                            isLineLast = isLineNow;
                        }
                    }
                }
            }
            if (logEnable) Logger.Trace("...RemoveIntermediateSteps removed:{0} --------------------------------------", removed);
        }

        private static void RemoveShortMoves(List<PathObject> graphicToImprove, double minDistance)
        {
            if (logEnable) Logger.Trace("...RemoveShortMoves before min X:{0:0.00} Y:{1:0.00} --------------------------------------", actualDimension.minx, actualDimension.miny);
            foreach (PathObject item in graphicToImprove)    // dot or path
            {
                if (item is ItemPath PathData)
                {
                    Point lastPoint = PathData.Path[PathData.Path.Count-1].MoveTo;  // PathData.End;
                    double lastAngle = PathData.Path[PathData.Path.Count - 1].Angle;
                    double distance;
                    if (PathData.Path.Count > 3)
                    {
                        for (int i = (PathData.Path.Count - 2); i > 0; i--)
                        {
                            if (PathData.Path[i] is GCodeLine)
                                distance = Math.Sqrt(PointDistanceSquared(PathData.Path[i].MoveTo, lastPoint));
                            else
                                distance = minDistance;

                            if ((distance < minDistance) && (Math.Abs(lastAngle - PathData.Path[i].Angle) < 0.5))   // < 30°
                            { PathData.Path.RemoveAt(i); }

                            lastPoint = PathData.Path[i].MoveTo; 
                            lastAngle = PathData.Path[i].Angle; 
                        }
                    }
                }
            }
        }
        #endregion

        #region remove offset
        private static void RemoveOffset(List<PathObject> graphicToOffset, double offsetX, double offsetY)
        {
            System.Diagnostics.StackTrace s = new System.Diagnostics.StackTrace(System.Threading.Thread.CurrentThread, true);

            //           MessageBox.Show("Methode B wurde von Methode " + s.GetFrame(1).GetMethod().Name + " aufgerufen");
            if (logEnable) Logger.Trace("...RemoveOffset before min X:{0:0.00} Y:{1:0.00} caller:{2} --------------------------------------", actualDimension.minx, actualDimension.miny, s.GetFrame(1).GetMethod().Name);
            foreach (PathObject item in graphicToOffset)    // dot or path
            {
                item.Start = new Point(item.Start.X - offsetX, item.Start.Y - offsetY);
                item.End = new Point(item.End.X - offsetX, item.End.Y - offsetY);
                if (item is ItemPath PathData)
                {
                    //ItemPath PathData = (ItemPath)item;
                    foreach (GCodeMotion entity in PathData.Path)
                    { entity.MoveTo = new Point(entity.MoveTo.X - offsetX, entity.MoveTo.Y - offsetY); }
                    PathData.Dimension.OffsetXY(-offsetX, -offsetY);
                }
            }
            actualDimension.OffsetXY(-offsetX, -offsetY);
        }
        #endregion

        public static void ScaleXY(double scaleX, double scaleY)	// scaleX != scaleY will not work for arc!
        { Scale(completeGraphic, scaleX, scaleY); }
        private static void Scale(List<PathObject> graphicToOffset, double scaleX, double scaleY)
        {
            if (logEnable) Logger.Trace("...Scale scaleX:{0:0.00} scaleY:{1:0.00} ", scaleX, scaleY);
            foreach (PathObject item in graphicToOffset)    // dot or path
            {
                item.Start = new Point(item.Start.X * scaleX, item.Start.Y * scaleY);
                item.End = new Point(item.End.X * scaleX, item.End.Y * scaleY);
                if (item is ItemPath PathData)
                {
                    //ItemPath PathData = (ItemPath)item;
                    if (logEnable) Logger.Trace("  ID:{0} Geo:{1} Size:{2}", item.Info.Id, item.Info.PathGeometry, PathData.Path.Count);
                    foreach (GCodeMotion entity in PathData.Path)
                    { entity.MoveTo = new Point(entity.MoveTo.X * scaleX, entity.MoveTo.Y * scaleY); }
                    PathData.Dimension.ScaleXY(scaleX, scaleY);
                }
            }
        }

        private static void RepeatPaths(List<PathObject> graphicToRepeat, int repetitions)
        {
            if (logEnable) Logger.Trace("...RepeatPaths({0})", repetitions);
            if (logDetailed) ListGraphicObjects(graphicToRepeat, true);

            List<PathObject> repeatedGraphic = new List<PathObject>();

            foreach (PathObject item in graphicToRepeat)      // replace original list
            {
                for (int i = 0; i < repetitions; i++)
                { repeatedGraphic.Add(item.Copy()); }
            }

            graphicToRepeat.Clear();
            foreach (PathObject item in repeatedGraphic)      // replace original list
                graphicToRepeat.Add(item);

            repeatedGraphic.Clear();
            if (logDetailed) ListGraphicObjects(graphicToRepeat, true);
        }

        private static void AddFrame(Dimensions dim, double distance, bool applyRadius)
        {
            if (logEnable) Logger.Trace("...AddFrame({0}  {1})", distance, applyRadius);

            double minX = dim.minx - distance;
            double maxX = dim.maxx + distance;
            double minY = dim.miny - distance;
            double maxY = dim.maxy + distance;
            double radius = 0;
            if (applyRadius)
                radius = distance;

            SetGeometry("Frame");
            SetPenColor(Properties.Settings.Default.importGraphicAddFramePenColor);
            SetPenWidth(Properties.Settings.Default.importGraphicAddFramePenWidth.ToString());
            SetLayer(Properties.Settings.Default.importGraphicAddFramePenLayer);

            StartPath(new Point(minX, minY + radius));      // start bottom-left
            AddLine(new Point(minX, maxY - radius));        // move to top-left
            if (radius > 0) AddArc(true, minX + radius, maxY, radius, 0);
            AddLine(new Point(maxX - radius, maxY));        // move to top-right
            if (radius > 0) AddArc(true, maxX, maxY - radius, 0, -radius);
            AddLine(new Point(maxX, minY + radius));        // move to bottom-right
            if (radius > 0) AddArc(true, maxX - radius, minY, -radius, 0);
            AddLine(new Point(minX + radius, minY));        // move to bottom-left
            if (radius > 0) AddArc(true, minX, minY + radius, 0, radius);
            StopPath();

            actualDimension.SetDimensionXY(minX, minY);
            actualDimension.SetDimensionXY(maxX, maxY);
        }

        private static void MultiplyGraphics(List<PathObject> graphicToRepeat, Dimensions dim, double distance, int x, int y)     // repititions in x and y direction
        {
            if (logEnable) Logger.Trace("...MultiplyGraphics(distance:{0} x:{1}  y:{2})", distance, x, y);

            List<PathObject> repeatedGraphic = new List<PathObject>();

            //  double startx = dim.minx;
            //  double starty = dim.miny;
            double dx = dim.dimx + distance;
            double dy = dim.dimy + distance;
            double offsetX, offsetY;

            // add code wie bei tiling?

            for (int my = 0; my < y; my++)
            {
                for (int mx = 0; mx < x; mx++)
                {
                    offsetX = (mx * dx);
                    offsetY = (my * dy);
                    foreach (PathObject item in graphicToRepeat)    // dot or path
                    {
                        PathObject itemCopy;// = new PathObject();
                        if (item is ItemPath)
                        {
                            itemCopy = new ItemPath();
                            itemCopy = item.Copy();
                            itemCopy.Start = new Point(item.Start.X + offsetX, item.Start.Y + offsetY);
                            itemCopy.End = new Point(item.End.X + offsetX, item.End.Y + offsetY);
                            ItemPath PathData = (ItemPath)itemCopy;
                            foreach (GCodeMotion entity in PathData.Path)
                            { entity.MoveTo = new Point(entity.MoveTo.X + offsetX, entity.MoveTo.Y + offsetY); }
                            PathData.Dimension.OffsetXY(offsetX, offsetY);
                        }
                        else
                        { itemCopy = new ItemDot(item.Start.X + offsetX, item.Start.Y + offsetY); }

                        repeatedGraphic.Add(itemCopy);
                    }
                }
            }
            actualDimension.SetDimensionXY(x * dx, y * dy);

            graphicToRepeat.Clear();
            foreach (PathObject item in repeatedGraphic)      // replace original list
                graphicToRepeat.Add(item);

            repeatedGraphic.Clear();
        }


        private static void ExtendClosedPaths(List<PathObject> graphicToExtend, double extensionOrig)
        {
            //const uint loggerSelect = (uint)LogEnable.PathModification;
            bool log = logEnable && ((logFlags & (uint)LogEnables.ClipCode) > 0);

            double dx, dy, newX, newY, length, maxElements;
            double extension;
            if (logEnable) Logger.Trace("...ExtendClosedPaths extend:{0}", extensionOrig);

            foreach (PathObject item in graphicToExtend)
            {
                if (item is ItemPath PathData)
                {
                    //ItemPath PathData = (ItemPath)item; // is closed polygon?

                    if (IsEqual(PathData.Start, PathData.End))      //(PathData.Start.X == PathData.End.X) && (PathData.Start.Y == PathData.End.Y))
                    {
                        if (PathData.Path.Count > 1)
                        {   // first line could be shorter than needed extension
                            // Loop
                            int index = 1;
                            maxElements = PathData.Path.Count;

                            extension = Math.Abs(extensionOrig);
                            while (extension > 0)
                            {
                                if (PathData.Path[index] is GCodeLine)
                                {
                                    dx = PathData.Path[index].MoveTo.X - PathData.Path[index - 1].MoveTo.X;
                                    dy = PathData.Path[index].MoveTo.Y - PathData.Path[index - 1].MoveTo.Y;
                                    length = Math.Sqrt(dx * dx + dy * dy);
                                    if (log) Logger.Trace("    {0} Line length:{1:0.00}  extend:{2:0.00}", PathData.Info.Id, length, extension);
                                    if ((extension - length) >= 0)
                                    {
                                        extension -= length;
                                        PathData.Add(PathData.Path[index].MoveTo, PathData.Path[index].Depth, 0);
                                        goto NextElement;
                                    }
                                    // interpolate new pos.
                                    newX = PathData.Path[index - 1].MoveTo.X + dx * extension / length;
                                    newY = PathData.Path[index - 1].MoveTo.Y + dy * extension / length;
                                    PathData.Add(new Point(newX, newY), PathData.Path[index].Depth, 0);
                                    break;
                                }

                                else    // is Arc
                                {
                                    GCodeArc ArcData = (GCodeArc)PathData.Path[index];
                                    ArcProperties arcProp = GcodeMath.GetArcMoveProperties(PathData.Path[index - 1].MoveTo, ArcData.MoveTo, ArcData.CenterIJ, ArcData.IsCW); // in radians
                                    double circum = Math.Abs(arcProp.angleDiff * arcProp.radius);
                                    if (log) Logger.Trace("      diff:{0:0.00}  radius:{1:0.00}    circ:{2:0.00}", arcProp.angleDiff, arcProp.radius, circum);

                                    if (log) Logger.Trace("    {0} Arc  circum:{1:0.00}  extend:{2:0.00}", PathData.Info.Id, circum, extension);
                                    if ((extension - circum) >= 0)
                                    {
                                        extension -= circum;
                                        PathData.AddArc(ArcData.MoveTo, ArcData.CenterIJ, ArcData.IsCW, ArcData.Depth, ArcData.AngleStart, ArcData.Angle);
                                        goto NextElement;
                                    }
                                    // interpolate new pos.
                                    double angleNewDiff = arcProp.angleDiff * extension / circum;
                                    double angleEnd = arcProp.angleStart + angleNewDiff;
                                    newX = arcProp.center.X + arcProp.radius * Math.Cos(angleEnd);
                                    newY = arcProp.center.Y + arcProp.radius * Math.Sin(angleEnd);
                                    if (log) Logger.Trace("      diff:{0:0.00}  new diff:{1:0.00}    end:{2:0.00}  ext.{3}", arcProp.angleDiff, angleNewDiff, angleEnd, extension);

                                    PathData.AddArc(new Point(newX, newY), ArcData.CenterIJ, ArcData.IsCW, ArcData.Depth, ArcData.AngleStart, ArcData.Angle);
                                    break;
                                }

                            NextElement:
                                index++;
                                if (index >= maxElements)
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static void SortByDistance(List<PathObject> graphicToSort, bool preventReversal = false)
        {
            if (logEnable) Logger.Trace("...SortByDistance() count:{0}", graphicToSort.Count);
            stopwatch = new Stopwatch();
            stopwatch.Start();

            List<PathObject> sortedGraphic = new List<PathObject>();
            Point actualPos = new Point(0, 0);
            double distanceReverse;
            bool allowReverse = false;
            PathObject tmp;
            bool closedPathRotate = false;
            if (!graphicInformation.OptionTangentialAxis)   // angle calc is not ok
            {
                allowReverse = true;
                closedPathRotate = Properties.Settings.Default.importGraphicSortDistanceAllowRotate;
            }
            int maxElements = graphicToSort.Count;

            while ((graphicToSort.Count > 0) && (!cancelByWorker))                      // items will be removed step by step from completeGraphic
            {
                for (int i = 0; i < graphicToSort.Count; i++)     // calculate distance to all remaining items check start and end position
                {
                    tmp = graphicToSort[i];
                    tmp.Distance = PointDistanceSquared(actualPos, tmp.Start);

                    if (backgroundWorker != null)
                    {
                        if (UpdateGUI()) backgroundWorker.ReportProgress(((maxElements - graphicToSort.Count) * 100) / maxElements);
                        if (backgroundWorker.CancellationPending)
                        {
                            cancelByWorker = true;
                            backgroundWorker.ReportProgress(100, new MyUserState { Value = 100, Content = "Stop processing, clean up data. Please wait!" });
                            break;
                        }
                    }

                    if (tmp is ItemPath tmpItemPath)
                    {
                        /* if closed path, find closest point */
                        if (IsEqual(tmp.Start, tmp.End))
                        {
                            if (closedPathRotate && !preventReversal)
                            {
                                //ItemPath tmpItemPath = (ItemPath)tmp;
                                if (tmpItemPath.Path.Count > 2)
                                {
                                    PathDistanceSquared(actualPos, tmpItemPath);
                                    int index = tmp.TmpIndex;   //(int)tmp.StartAngle;
                                    if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  path index:{2}   distance:{3:0.00}", tmp.Info.Id, tmp.Info.PathGeometry, index, tmp.Distance);
                                    if ((index >= 0) && (index < tmpItemPath.Path.Count))
                                    { tmp.Start = tmp.End = tmpItemPath.Path[tmp.TmpIndex].MoveTo; } //(int)tmp.StartAngle
                                }
                                else if (tmpItemPath.Path.Count == 2)       // is circle
                                {
                                    if (tmpItemPath.Path[1] is GCodeArc)
                                    {
                                        CircleDistanceSquared(actualPos, tmpItemPath);
                                        if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  angle:{2:0.0}  distance:{3:0.00}", tmp.Info.Id, tmp.Info.PathGeometry, (tmp.StartAngle * 180 / Math.PI), tmp.Distance);
                                    }
                                }
                            }
                        }
                        else
                        {
                            /* if other end is closer, reverse path */
                            distanceReverse = PointDistanceSquared(actualPos, tmp.End);
                            if (allowReverse && !preventReversal && !(IsEqual(tmp.Start, tmp.End)) && (distanceReverse < tmp.Distance))
                            {
                                tmp.Distance = distanceReverse;
                                ((ItemPath)tmp).Reversed = !((ItemPath)tmp).Reversed;
                                Point start = tmp.Start;            //##################### new Point
                                tmp.Start = tmp.End;
                                tmp.End = start;
                            }
                            if (logSortMerge) Logger.Trace("    id:{0}  object:{1}  distance:{2:0.00}", tmp.Info.Id, tmp.Info.PathGeometry, tmp.Distance);
                        }
                        graphicToSort[i] = tmp;
                    }
                }
                graphicToSort.Sort((x, y) => x.Distance.CompareTo(y.Distance));   // sort by distance

                sortedGraphic.Add(graphicToSort[0]);       	// get closest item = first in list
                actualPos = graphicToSort[0].End;         	// set new start pos
                if (logSortMerge) Logger.Trace("   remove id:{0} ", graphicToSort[0].Info.Id);
                graphicToSort.RemoveAt(0);                  // remove item from remaining list
            }

            if (cancelByWorker)//stopwatch.Elapsed.Minutes >= maxTimeMinute)  // time expired, just copy missing figures
            {
                foreach (PathObject tmpgrp in graphicToSort)
                    sortedGraphic.Add(tmpgrp);
            }

            PathObject sortedItem;
            for (int i = 0; i < sortedGraphic.Count; i++)     // loop through all items
            {
                sortedItem = sortedGraphic[i];

                /* now rotate path-points */
                if (!cancelByWorker && (sortedItem is ItemPath apath) && (closedPathRotate) && (IsEqual(sortedItem.Start, sortedItem.End)))
                { RotatePath(apath); }            // finally reverse path if needed

                /* now reverse path */
                if (!cancelByWorker && (sortedItem is ItemPath bpath) && bpath.Reversed)
                { ReversePath(bpath); }            // finally reverse path if needed
            }

            graphicToSort.Clear();
            foreach (PathObject item in sortedGraphic)      // replace original list
                graphicToSort.Add(item);

            sortedGraphic.Clear();
            if (logEnable) Logger.Trace("...SortByDistance()  finish");
        }
        private static double PointDistanceSquared(Point a, Point b)	// avoid square-root, to save time
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (dx * dx + dy * dy);	// speed up
        }
        private static void CircleDistanceSquared(Point a, ItemPath tmp)
        {
            ArcProperties arcMove = GcodeMath.GetArcMoveProperties((XyPoint)tmp.Path[0].MoveTo, (XyPoint)tmp.Path[1].MoveTo, ((GCodeArc)tmp.Path[1]).CenterIJ.X, ((GCodeArc)tmp.Path[1]).CenterIJ.Y, ((GCodeArc)tmp.Path[1]).IsCW);
            double aLine = GcodeMath.GetAlpha(ToPointF(arcMove.center), a);
            Point ptmp = new Point
            {
                X = arcMove.center.X + arcMove.radius * Math.Cos(aLine),
                Y = arcMove.center.Y + arcMove.radius * Math.Sin(aLine)
            };

            double distance = PointDistanceSquared(a, ptmp);
            //            Logger.Trace("       circle angle:{0:0.00}  radius:{1:0.00}  x:{2:0.00} y:{3:0.00}  distance:{4:0.00}  ", (aLine*180/Math.PI), arcMove.radius, ptmp.X, ptmp.Y, distance);

            tmp.Distance = distance;
            tmp.StartAngle = aLine;
        }
        private static void PathDistanceSquared(Point a, ItemPath tmp)	// go through coordinates of path
        {
            if (tmp.Path.Count < 2)
                return;
            double distance = PointDistanceSquared(a, tmp.Path[0].MoveTo);
            double distTemp;
            int i, index = 0;
            for (i = 0; i < tmp.Path.Count; i++)
            {
                distTemp = PointDistanceSquared(a, tmp.Path[i].MoveTo);
                if (distTemp < distance)
                {
                    distance = distTemp;
                    index = i;
                }
                //                Logger.Trace("       path a.x:{0:0.00}  a.y:{1:0.00}  distance:{2:0.00}   i:{3}  index:{4}", a.X, a.Y, distTemp, i, index);
            }
            tmp.Distance = distance;
            tmp.TmpIndex = index;     //tmp.StartAngle = index;
            if (logSortMerge) Logger.Trace("       path a.x:{0:0.00}  a.y:{1:0.00}  distance:{2:0.00}   i:{3}", a.X, a.Y, distance, index);
        }
        private static Point ToPointF(XyPoint tmp)
        { return new Point((float)tmp.X, (float)tmp.Y); }

        private static bool HasSameProperties(ItemPath a, ItemPath b)
        {
            bool sameProperties = true;
            //            bool checkAll = false;// true;
            if (graphicInformation.GroupEnable)
            {
                for (int i = 1; i <= 5; i++)    // GroupOptions { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByType = 4, ByTile = 5};
                {
                    if (logDetailed) Logger.Trace("  hasSameProperties - GroupEnable-Option:{0} a:'{1}'  b:'{2}'", i, a.Info.GroupAttributes[i], b.Info.GroupAttributes[i]);
                    sameProperties = a.Info.GroupAttributes[i] == b.Info.GroupAttributes[i];
                    if (!sameProperties)
                        return false;
                }
            }
            bool sameDash = true;
            if (Properties.Settings.Default.importLineDashPattern)
            {
                //                Logger.Trace("  hasSameProperties a-count:{0} dash-a:{1}  b-count:{2} dash-b:{3}", a.dashArray.Count(), showArray(a.dashArray), b.dashArray.Count(), showArray(b.dashArray));
                sameDash = false;
                if ((a.DashArray.Length == 0) && (b.DashArray.Length == 0))
                    sameDash = true;
                else if (a.DashArray == b.DashArray)
                    sameDash = true;
            }
            return (sameProperties && sameDash);
        }
        /*     private static string ShowArray(double[] tmp)
             {   string tmps = "";
                 foreach (double nr in tmp)
                     tmps += string.Format( "{0:0.00} ", nr);
                 return tmps;
             }*/
        private static void MergeFigures(List<PathObject> graphicToMerge)
        {
            if (logEnable) Logger.Trace("...MergeFigures before:{0}    ------------------------------------", graphicToMerge.Count);
            stopwatch = new Stopwatch();
            stopwatch.Start();

            int i, k;
            int maxElements = graphicToMerge.Count;

            for (i = graphicToMerge.Count - 1; i > 0; i--)
            {
                if (graphicToMerge[i] is ItemDot)
                    continue;
                else if (graphicToMerge[i] is ItemPath ipath)
                {
                    if (IsEqual(graphicToMerge[i].Start, graphicToMerge[i].End))    // closed path can't be merged
                        continue;
                    if (ipath.Path.Count <= 1)              // 2020-08-10
                        continue;

                    if (backgroundWorker != null)
                    {
                        if (UpdateGUI()) backgroundWorker.ReportProgress(((maxElements - i) * 100) / maxElements);
                        if (backgroundWorker.CancellationPending)
                        {
                            cancelByWorker = true;
                            backgroundWorker.ReportProgress(100, new MyUserState { Value = 100, Content = "Stop processing, clean up data. Please wait!" });
                            break;
                        }
                    }

                    for (k = 0; k < graphicToMerge.Count; k++)
                    {
                        if ((k == i) || (i >= graphicToMerge.Count))
                            continue;
                        if (graphicToMerge[k] is ItemDot)                           // 2020-08-10
                            continue;
                        if (((ItemPath)graphicToMerge[k]).Path.Count <= 1)          // 2020-08-10
                            continue;

                        //                        if (loggerTrace > 0) Logger.Trace("  i:{0}  k:{1}   count:{2}", i, k, graphicToMerge.Count);
                        if (graphicToMerge[k] is ItemPath kpath)
                        {
                            // closed path can't be merged
                            if (IsEqual(graphicToMerge[k].Start, graphicToMerge[k].End))
                                continue;

                            // paths with different propertys should not be merged
                            if (!HasSameProperties(ipath, kpath))
                            {   //                             Logger.Trace("  not same Properties");
                                continue;
                            }

                            // i-start = k-end -> just connect paths (i at the end of k)
                            if (IsEqual(graphicToMerge[i].Start, graphicToMerge[k].End))
                            {
                                //                  if (loggerTrace) Logger.Trace("   1) merge:{0} start  x1:{1} y1:{2}  addto:{3} end x:{4}  y:{5}", i, graphicToMerge[i].Start.X, graphicToMerge[i].Start.Y, k, graphicToMerge[k].End.X, graphicToMerge[k].End.Y);
                                MergePaths(kpath, ipath);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
                            // i-end = k-end -> reverse i, then connect paths (i at the end of k)
                            else if (IsEqual(graphicToMerge[i].End, graphicToMerge[k].End))
                            {
                                //                    if (loggerTrace) Logger.Trace("   2) merge:{0} end  x1:{1} y1:{2}  addto:{3} end x:{4}  y:{5}", i, graphicToMerge[i].End.X, graphicToMerge[i].End.Y, k, graphicToMerge[k].End.X, graphicToMerge[k].End.Y);
                                ReversePath(ipath);
                                MergePaths(kpath, ipath);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
                            // i-start = k-start -> reverse k, then connect paths (i at the end of k)
                            else if (IsEqual(graphicToMerge[i].Start, graphicToMerge[k].Start))
                            {
                                //                    if (loggerTrace) Logger.Trace("   3) merge:{0} start  x1:{1} y1:{2}  addto:{3} start x:{4}  y:{5}", i, graphicToMerge[i].Start.X, graphicToMerge[i].Start.Y, k, graphicToMerge[k].Start.X, graphicToMerge[k].Start.Y);
                                ReversePath(kpath);
                                MergePaths(kpath, ipath);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
                            // i-end = k-start -> reverse i and k, then connect paths (i at the end of k)
                            else if (IsEqual(graphicToMerge[i].End, graphicToMerge[k].Start))
                            {
                                //                    if (loggerTrace) Logger.Trace("   4) merge:{0} end  x1:{1} y1:{2}  addto:{3} start x:{4}  y:{5}", i, graphicToMerge[i].End.X, graphicToMerge[i].End.Y, k, graphicToMerge[k].Start.X, graphicToMerge[k].Start.Y);
                                ReversePath(ipath);
                                ReversePath(kpath);
                                MergePaths(kpath, ipath);
                                graphicToMerge.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            if (logDetailed) ListGraphicObjects(graphicToMerge, true);
            if (logEnable) Logger.Trace("...MergeFigures after :{0}    ------------------------------------", graphicToMerge.Count);
        }
        private static bool IsEqual(System.Windows.Point a, System.Windows.Point b)
        { return ((Math.Abs(a.X - b.X) < equalPrecision) && (Math.Abs(a.Y - b.Y) < equalPrecision)); }
        private static bool IsEqual(double a, double b)
        { return (Math.Abs(a - b) < equalPrecision); }
        private static void MergePaths(ItemPath addAtEnd, ItemPath toMerge)
        {
            if (logDetailed) Logger.Trace(".....MergePaths ID:{0} {1}     ID:{2} {3}", addAtEnd.Info.Id, addAtEnd.Info.PathGeometry, toMerge.Info.Id, toMerge.Info.PathGeometry);
            toMerge.Path.RemoveAt(0);   // avoid double coordinate (start = end)
            addAtEnd.Path = addAtEnd.Path.Concat(toMerge.Path).ToList();
            addAtEnd.Dimension.AddDimensionXY(toMerge.Dimension);
            addAtEnd.End = toMerge.End;
            //            addAtEnd.Info.pathGeometry += ".";// " Merged " + toMerge.Info.id.ToString();
        }

        /*
                private static void SetDotOnly()
                {   foreach (PathObject item in completeGraphic)        // replace original list
                    {   foreach (grblMotion path in item.path)
                        {   path.lineType = MotionType.dot; }
                    }           
                }*/


        private static void RotatePath(ItemPath item)
        {
            List<GCodeMotion> rotatedPath = new List<GCodeMotion>();
            GCodeMotion tmpMove;
            int i, iStart = item.TmpIndex;  // (int)item.StartAngle;
            int listIndex = 0;
            if (logSortMerge) Logger.Trace(".....RotatePath ID:{0}  index:{1}  count:{2}", item.Info.Id, iStart, item.Path.Count);

            /* only logging */
            if (logSortMerge && logDetailed)
            {
                Logger.Trace("     before rotation");
                if (item.Path.Count < listIndex)
                {
                    for (i = 0; i < item.Path.Count; i++)
                    {
                        if (item.Path[i] is GCodeLine)
                            Logger.Trace("      index:{0} Line  x:{1:0.00}  y:{2:0.00}", i, item.Path[i].MoveTo.X, item.Path[i].MoveTo.Y);
                        else
                            Logger.Trace("      index:{0} Arc   x:{1:0.00}  y:{2:0.00}  i:{3:0.00}  j:{4:0.00}", i, item.Path[i].MoveTo.X, item.Path[i].MoveTo.Y, ((GCodeArc)item.Path[i]).CenterIJ.X, ((GCodeArc)item.Path[i]).CenterIJ.Y);
                    }
                }
            }

            if ((iStart >= 0) && (iStart < item.Path.Count))
                item.Start = item.End = item.Path[iStart].MoveTo;       // restore start/end position

            if (item.Path.Count == 2)       // is circle
            {
                if (item.Path[1] is GCodeArc arc)
                {
                    ArcProperties arcMove = GcodeMath.GetArcMoveProperties((XyPoint)item.Path[0].MoveTo, (XyPoint)item.Path[1].MoveTo, arc.CenterIJ.X, arc.CenterIJ.Y, arc.IsCW);
                    Point ptmp = new Point();
                    Point pij = new Point();
                    ptmp.X = arcMove.center.X + arcMove.radius * Math.Cos(item.StartAngle);
                    ptmp.Y = arcMove.center.Y + arcMove.radius * Math.Sin(item.StartAngle);

                    pij.X = arcMove.center.X - ptmp.X;
                    pij.Y = arcMove.center.Y - ptmp.Y;
                    item.Start = item.Path[0].MoveTo = ptmp;
                    item.End = item.Path[1].MoveTo = ptmp;
                    arc.CenterIJ = pij;
                    //          Logger.Trace("       circle angle:{0:0.00}  radius:{1:0.00}  x:{2:0.00} y:{3:0.00}  distance:{4:0.00}  ", aLine, arcMove.radius, ptmp.X, ptmp.Y, distance);
                    rotatedPath.Add(item.Path[0]);
                    rotatedPath.Add(item.Path[1]);
                }
            }
            else
            {
                //                if ((item.StartAngle == 0) || (iStart >= item.path.Count))           // nothing to rotate
                if ((item.TmpIndex == 0) || (iStart >= item.Path.Count))           // nothing to rotate
                    return;

                tmpMove = new GCodeLine(item.Start);     	// 
                rotatedPath.Add(tmpMove);

                for (i = iStart + 1; i < item.Path.Count; i++)
                { rotatedPath.Add(item.Path[i]); }
                for (i = 1; i <= iStart; i++)
                { rotatedPath.Add(item.Path[i]); }
            }

            //item.StartAngle = 0;
            item.TmpIndex = 0;

            /* replace original path */
            item.Path.Clear();
            foreach (GCodeMotion ent in rotatedPath)
                item.Path.Add(ent);

            if (logSortMerge)
            {
                if (item.Path.Count < listIndex)
                {
                    Logger.Trace("     after rotation");
                    for (i = 0; i < item.Path.Count; i++)
                    {
                        if (item.Path[i] is GCodeLine)
                            Logger.Trace("      Line i:{0}   x:{1:0.00}  y:{2:0.00}", i, item.Path[i].MoveTo.X, item.Path[i].MoveTo.Y);
                        else
                            Logger.Trace("      Arc  i:{0}   x:{1:0.00}  y:{2:0.00}  i:{3:0.00}  j:{4:0.00}", i, item.Path[i].MoveTo.X, item.Path[i].MoveTo.Y, ((GCodeArc)item.Path[i]).CenterIJ.X, ((GCodeArc)item.Path[i]).CenterIJ.Y);
                    }
                }
            }
        }

        private static void ReversePath(ItemPath item)
        {
            List<GCodeMotion> reversedPath = new List<GCodeMotion>();
            if (logDetailed) Logger.Trace(".....ReversePath ID:{0} {1}", item.Info.Id, item.Info.PathGeometry);
            GCodeMotion tmpMove;

            if (item.IsReversed)    // indicatior if Start/End was already switched (in SortCode()
                tmpMove = new GCodeLine(item.Start, item.Path[item.Path.Count - 1].Depth);      // first reversed point is End of original (but original is already switched in SortCode() -> use start)
            else
                tmpMove = new GCodeLine(item.End, item.Path[item.Path.Count - 1].Depth);        // first reversed point is End of original 

            reversedPath.Add(tmpMove);

            for (int i = item.Path.Count - 2; i >= 0; i--)              // -1 start from rear, with item one before last
            {
                if (item.Path[i + 1] is GCodeLine)         // isLine
                {
                    tmpMove = new GCodeLine(item.Path[i].MoveTo, item.Path[i].Depth);
                    reversedPath.Add(tmpMove);
                }
                else
                {
                    double cx = item.Path[i].MoveTo.X + ((GCodeArc)item.Path[i + 1]).CenterIJ.X;       // arc is a bit more work
                    double cy = item.Path[i].MoveTo.Y + ((GCodeArc)item.Path[i + 1]).CenterIJ.Y;
                    //       Point center = new Point(cx, cy);
                    double newi = cx - item.Path[i + 1].MoveTo.X;
                    double newj = cy - item.Path[i + 1].MoveTo.Y;
                    tmpMove = new GCodeArc(item.Path[i].MoveTo, new Point(newi, newj), !((GCodeArc)item.Path[i + 1]).IsCW, item.Path[i + 1].Depth);
                    reversedPath.Add(tmpMove);
                }
            }
            item.Path.Clear();
            foreach (GCodeMotion ent in reversedPath)
                item.Path.Add(ent);

            if (!item.IsReversed)				// indicatior if Start/End was already switched (in SortCode()
            {
                Point tmp = item.End;       // if not, do now
                item.End = item.Start;
                item.Start = tmp;
            }
            item.IsReversed = false;			// reset indicator
        }

        public static void AlignLines(int align)	// apply offset to figures with same AuxInfo
        {
            //        Logger.Info("AlignLines  gcnt:{0}", completeGraphic.Count);
            if (completeGraphic.Count > 0)
            {
                Dictionary<int, Dimensions> lineDim = new Dictionary<int, Dimensions>();
                Dictionary<int, double> lineOffset = new Dictionary<int, double>();
                double offsetX, offsetY = 0;
                int cnt = 0;
                foreach (PathObject item in completeGraphic)            // calc dimension of lines
                {

                    if (lineDim.ContainsKey(item.Info.AuxInfo))
                    {
                        lineDim[item.Info.AuxInfo].AddDimensionXY(item.Dimension);
                        //                    Logger.Info("AlignLines      line:{0} x:{1}", item.Info.AuxInfo, lineDim[item.Info.AuxInfo].dimx);
                    }
                    else
                    {
                        lineDim.Add(item.Info.AuxInfo, new Dimensions(item.Dimension));
                        //                    Logger.Info("AlignLines  new line:{0} cnt:{1}  x:{2}", item.Info.AuxInfo, cnt, item.Dimension.dimx);
                    }
                    cnt++;
                }
                foreach (KeyValuePair<int, Dimensions> pair in lineDim) // calc offsets of lines
                {
                    if (align == 1)
                        offsetX = (actualDimension.dimx - pair.Value.dimx) / 2;   // center
                    else
                        offsetX = (actualDimension.dimx - pair.Value.dimx);     // right
                    lineOffset.Add(pair.Key, offsetX);
                    //                Logger.Info("AlignLines  line:{0}  offset:{1:0.000}", pair.Key, offsetX);
                }

                foreach (PathObject item in completeGraphic)            // apply offsets
                {
                    offsetX = lineOffset[item.Info.AuxInfo];
                    item.Start = new Point(item.Start.X + offsetX, item.Start.Y + offsetY);
                    item.End = new Point(item.End.X + offsetX, item.End.Y + offsetY);
                    if (item is ItemPath PathData)
                    {
                        //ItemPath PathData = (ItemPath)item;
                        foreach (GCodeMotion entity in PathData.Path)
                        { entity.MoveTo = new Point(entity.MoveTo.X + offsetX, entity.MoveTo.Y + offsetY); }
                        PathData.Dimension.OffsetXY(offsetX, offsetY);
                    }
                }
            }
        }



        private static void SetOrderOfFigures(List<PathObject> tmp, int[] sortOrder)
        {
            tileGraphicAll = new List<PathObject>();    // 
            foreach (int figureId in sortOrder)                  // go through all figures in 2D-view
            {
                foreach (PathObject graphicItem in tmp)         // go through all graphic figures
                {
                    if (graphicItem.FigureId == figureId)       // if match, add to return graphics
                    {
                        tileGraphicAll.Add(graphicItem);
                        if (logSortMerge) Logger.Trace(" setOrder {0}", figureId);
                    }
                }
            }
            tmp.Clear();
            foreach (PathObject fig in tileGraphicAll)
                tmp.Add(fig);
        }

    }
}