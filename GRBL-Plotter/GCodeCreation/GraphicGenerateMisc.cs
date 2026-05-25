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
 * 2021-09-10 split from graphicRelated
 * 2021-10-29 bugfix in RemoveIntermediateSteps
 * 2021-11-16 bugfix RemoveIntermediateSteps, RemoveShortMoves
 * 2022-01-24 add CalculateDistances
 * 2023-04-12 l:364  f:ClipCode only remove offset, if option is enabled
 * 2023-04-22 l:1280 f:HasSameProperties loop until 6
 * 2023-04-25 l:845  f:FilterProperties new function; only show tile-id if needed
 * 2023-07-03 l:1307 f:HasSameProperties also check pen-width if graphicInformation.OptionZFromWidth || graphicInformation.OptionSFromWidth
 * 2023-07-05 l:776  f:RemoveIntermediateSteps also compare depth informnation
 * 2023-07-13 l:800  f:RemoveShortMoves also compare depth informnation
 * 2023-07-30 l:898  f:RemoveOffset calc new dimesnion for all types
 * 2023-08-06 l:1283 f:SortByDistance get also start-pos
 * 2023-08-16 l:1370 f:SortByDistance pull request Speed up merge and sort #348
 * 2023-08-16 l:1490 f:HasSameProperties pull request Speed up merge and sort #348
 * 2023-09-01 l:915  f:RemoveOffset check index before shifting largest object
 * 2023-11-09 l:1067 f:AddFrame -> SetPenFill("none") to avoid hatch-FillToolListElements
 * 2023-11-11 l_1030 f:Scale scale also ((GCodeArc)entity).CenterIJ
 * 2024-04-17 l:1144 f:ExtendClosedPaths also allow path-shortening (negative value)
 * 2024-07-25 l:896/1568 f:RemoveOffset/SortByDistance move all largset (same size) objects to the end -> List<int> iLargest = new List<int>();
 * 2024-08-11 l:1684 f:HasSameProperties at least compare color
 * 2026-03-01 l:954 f:RemoveOffset seperate "move largest object to the end"
 * 2026-03-02 l:1214 f:RepeatPaths add tool specific repetitions
 * 2026-04-09 GUI rework for vers. 1.8.0.0
 * 2026-05-12 split functions to GraphicGenerateClipAndTile.cs; GraphicGenerateTransform.cs
*/

using burningmime.curves;
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

        private static void CalculateDistances()
        {
            foreach (PathObject pathObject in completeGraphic)
            {
                if (pathObject is ItemPath apath)
                {
                    if (apath.Path.Count <= 0)
                    { continue; }

                    Point pStart, pEnd;
                    apath.Path[0].Depth = 0;
                    for (int i = 1; i < apath.Path.Count; i++)                   // go through path objects
                    {
                        pStart = apath.Path[i - 1].MoveTo;
                        pEnd = apath.Path[i].MoveTo;

                        if (apath.Path[i] is GCodeLine)
                        {
                            apath.Path[i].Depth = PointDistance(pStart, pEnd);  // distance between a and b
                        }
                        else if (apath.Path[i] is GCodeArc aPathArc)
                        {   // is arc
                            Point center = new Point(pStart.X + aPathArc.CenterIJ.X, pStart.Y + aPathArc.CenterIJ.Y);
                            double r = PointDistance(center, pEnd);
                            ArcProperties tmp = GcodeMath.GetArcMoveProperties((XyPoint)pStart, (XyPoint)pEnd, aPathArc.CenterIJ.X, aPathArc.CenterIJ.Y, aPathArc.IsCW);
                            aPathArc.Depth = Math.Abs(tmp.angleDiff * r);
                        }
                    }
                }
            }
        }

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
            bool log = logEnable && ((logFlags & (uint)LogEnables.GroupAllGraphics) > 0);
            keyToIndex = new Dictionary<string, int>();
            string tmpKey = "";
            int groupCount = -1;
            int toolNr = (int)Properties.Settings.Default.importGCToolDefNr;
            string toolName = "";
            bool applyToolList = Properties.Settings.Default.importGCToolListUse;
            bool useToolList = MyControl.UseToolList();


            Logger.Info("►►► GroupAllGraphics by:{0} ", graphicInformation.GroupOption.ToString());
            if (logEnable)
            {
                foreach (GroupOption tmp in (GroupOption[])Enum.GetValues(typeof(GroupOption)))
                { Logger.Trace("...Property count {0} {1}", tmp, groupPropertiesCount[(int)tmp].Count); }
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

                if (log) Logger.Trace("...GroupAll {0} type:{1} count:{2}  key:'{3}'", pathObject.Info.Id, pathObject.GetType().ToString(), groupCount, tmpKey);

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

                    if (applyToolList)
                    {
                        switch (graphicInformation.GroupOption)
                        {
                            case GroupOption.ByColor:
                                toolNr = ToolList.GetToolNRByToolColor(pathObject.Info.GroupAttributes[(int)GroupOption.ByColor], 0);
                                toolName = ToolList.GetToolName(toolNr);
                                break;
                            case GroupOption.ByWidth:
                                toolNr = ToolList.GetToolNRByToolWidth(pathObject.Info.GroupAttributes[(int)GroupOption.ByWidth]);
                                toolName = ToolList.GetToolName(toolNr);
                                break;
                            case GroupOption.ByLayer:
                                toolNr = ToolList.GetToolNRByToolLayer(pathObject.Info.GroupAttributes[(int)GroupOption.ByLayer]);
                                toolName = ToolList.GetToolName(toolNr);
                                break;
                            default: break;
                        }
                    }
                    else
                    {
                        toolNr = groupCount + 1;
                        toolName = graphicInformation.GroupOption.ToString() + ": " + pathObject.Info.GroupAttributes[(int)graphicInformation.GroupOption];
                    }

                    tmpGroup = new GroupObject(tmpKey, toolNr, toolName, pathObject);
                    groupedGraphicLocal.Add(tmpGroup);
                    if (log)
                        Logger.Trace("     Create {0} new key:'{1}'    nr:{2}  name:{3}", pathObject.Info.Id, tmpKey, toolNr, toolNr);
                }
            }

            if (differentGroupValues > 1)	// 2021-06-03
            {
                // Sort Groups by		
                bool invert = Properties.Settings.Default.importGroupSortInvert;
                Logger.Trace("...Sort by:{0}  invert:{1}", graphicInformation.SortOption, invert);
                if (graphicInformation.SortOption != SortOption.none)       // 		public enum SortOption  { none=0, ByToolNr=1, ByCodeSize=2, ByGraphicDimension=3};
                {
                    if (log) Logger.Trace("...Sort by:{0}", graphicInformation.SortOption);
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
            }
            // Sort paths in group by distance		
            bool preventSortNow = graphicInformation.OptionTangentialAxis || graphicInformation.OptionDragTool;

            Logger.Trace("...GroupAllGraphics  count:{0}  sort?:{1}", groupedGraphicLocal.Count, !preventSortNow);
            if (!preventSortNow && (graphicInformation.OptionCodeSortDistance || graphicInformation.OptionCodeSortDimension)) //                Properties.Settings.Default.importGraphicSortDistance)
            {
                foreach (GroupObject groupObject in groupedGraphicLocal)
                {
                    Logger.Trace("...GroupAllGraphics  groupObject count:{0}", groupObject.GroupPath.Count);
                    if (groupObject.GroupPath.Count > 1)
                    {
                        if (graphicInformation.OptionCodeSortDistance)
                            SortByDistance(groupObject.GroupPath, new Point(0, 0), graphicInformation.OptionCodeSortDistanceNewStartOnClosedPath, graphicInformation.OptionCodeSortDistanceLargestLast, preventReversal);     // GroupAllGraphics
                        else if (graphicInformation.OptionCodeSortDimension)
                            SortByDimension(groupObject.GroupPath);//, new Point(0, 0), graphicInformation.OptionCodeSortDistanceNewStartOnClosedPath, graphicInformation.OptionCodeSortDistanceLargestLast, preventReversal);     // GroupAllGraphics
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
            if (!logEnable)
                return;

            Logger.Trace("  ListGraphicObjects Count:{0}  ##########################################", graphicToShow.Count);
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
                info = string.Format(" color:'{0}' fill:'{1}' width:'{2}' layer:'{3}' length:'{4:0.00}'", graphicItem.Info.GroupAttributes[1], graphicItem.Info.GroupAttributes[6], graphicItem.Info.GroupAttributes[2], graphicItem.Info.GroupAttributes[3], length);   // index 0 not used
                if (logEnable) Logger.Trace("    graphicItem Id:{0,3} pathId:{1,-12} Geo:{2,-8}  {3}   Points:{4,3}  Coord:{5}", graphicItem.Info.Id, graphicItem.Info.PathId, graphicItem.Info.PathGeometry, info, cnt, coordByLine);

                if (logCoordinates)
                {
                    if (showCoord && (graphicItem is ItemPath bpath))
                    {
                        foreach (GCodeMotion ent in bpath.Path)
                        {
                            if (ent is GCodeArc)
                                Logger.Trace("       X:{0:0.00} Y:{1:0.00} Z:{2:0.00}  Arc   aStart:{4:0.00}  aEnd:{5:0.00}", ent.MoveTo.X, ent.MoveTo.Y, ent.Depth, ent.ToString(), ((GCodeArc)ent).AngleStart * 180 / Math.PI, ent.Angle * 180 / Math.PI);
                            else
                                Logger.Trace("       X:{0:0.00} Y:{1:0.00} Z:{2:0.00}  Line  angle:{4:0.00}", ent.MoveTo.X, ent.MoveTo.Y, ent.Depth, ent.ToString(), ent.Angle * 180 / Math.PI);

                        }
                    }
                }
            }
            if (logEnable) Logger.Trace("  ListGraphicObjects          ##########################################", graphicToShow.Count);
        }





        // keep or remove list
        private static void FilterProperties(List<PathObject> graphicToFilter)
        {
            bool doRemove = Properties.Settings.Default.importGraphicFilterChoiceRemove;
            string listRemove = Properties.Settings.Default.importGraphicFilterListRemove;
            string listKeep = Properties.Settings.Default.importGraphicFilterListKeep;
            string needle;
            if (doRemove)
            {
                if (string.IsNullOrEmpty(listRemove))
                    return;
                Logger.Warn("FilterProperties remove figures with following pen-color or pen-width: {0}", listRemove);
                string tmp;
                for (int i = graphicToFilter.Count - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(graphicToFilter[i].Info.GroupAttributes[(int)GroupOption.ByColor]))
                        if (listRemove.Contains(graphicToFilter[i].Info.GroupAttributes[(int)GroupOption.ByColor]))
                        {
                            graphicToFilter.RemoveAt(i);
                            continue;
                        }
                    if (!string.IsNullOrEmpty(graphicToFilter[i].Info.GroupAttributes[(int)GroupOption.ByWidth]))
                        if (listRemove.Contains(graphicToFilter[i].Info.GroupAttributes[(int)GroupOption.ByWidth]))
                        {
                            graphicToFilter.RemoveAt(i);
                            continue;
                        }
                }
            }
            else    // keep
            {
                if (string.IsNullOrEmpty(listKeep))
                    return;
                Logger.Warn("FilterProperties keep only figures with following pen-color or pen-width: {0}", listKeep);
                for (int i = graphicToFilter.Count - 1; i >= 0; i--)
                {
                    needle = graphicToFilter[i].Info.GroupAttributes[(int)GroupOption.ByColor];
                    if (!string.IsNullOrEmpty(needle) && listKeep.Contains(needle))
                    {
                        continue;
                    }
                    needle = graphicToFilter[i].Info.GroupAttributes[(int)GroupOption.ByWidth];
                    if (!string.IsNullOrEmpty(needle) && listKeep.Contains(needle))
                    {
                        continue;
                    }
                    graphicToFilter.RemoveAt(i);
                }
            }
        }

        private static void AddPressure(List<PathObject> graphicToProcess)
        {
            foreach (PathObject item in graphicToProcess)      // replace original list
            {
                if (item is ItemPath PathData)
                {
                    //PathData.Path[1].Depth
                    {
                    }
                }
            }
        }

        private static void RepeatPaths(List<PathObject> graphicToRepeat, int repetitions, bool addZ, double zMax)
        {
            // if (logEnable) 
            Logger.Trace("...RepeatPaths({0})  addZ:{1}  zMax:{2:0.00}", repetitions, addZ, zMax);
            if (logDetailed) ListGraphicObjects(graphicToRepeat, true);
            bool DeviceSpecificOptions = MyControl.UseToolList();
            if (!DeviceSpecificOptions && (repetitions <= 1))
            {
                Logger.Warn("RepeatPaths repetitions:{0} - abort", repetitions);
                return;
            }
            List<PathObject> repeatedGraphic = new List<PathObject>();

            /* get amount of repetitions by toolNr */
            foreach (PathObject item in graphicToRepeat)      // replace original list
            {
                if (DeviceSpecificOptions)
                {
                    int toolNr = 1;
                    switch (graphicInformation.GroupOption)
                    {
                        case GroupOption.ByColor:
                            toolNr = ToolList.GetToolNRByToolColor(item.Info.GroupAttributes[(int)GroupOption.ByColor], 0);
                            break;
                        case GroupOption.ByWidth:
                            toolNr = ToolList.GetToolNRByToolWidth(item.Info.GroupAttributes[(int)GroupOption.ByWidth]);
                            break;
                        case GroupOption.ByLayer:
                            toolNr = ToolList.GetToolNRByToolLayer(item.Info.GroupAttributes[(int)GroupOption.ByLayer]);
                            break;
                        default: break;
                    }
                    repetitions = ToolList.GetToolRepetition(toolNr, MyControl.SelectedDevice);
                    //       Logger.Trace("....RepeatPaths  toolNr:{0}  device:{1}  repetitions:{2}", toolNr, MyControl.SelectedDevice,repetitions);
                }

                if (repetitions <= 0)
                    continue;
                //     if (addZ)
                {
                    double[] deepth = new double[repetitions];
                    deepth[0] = 0;
                    if (repetitions == 2) { deepth[1] = zMax; }
                    else if (repetitions > 2)
                    {
                        double stp = zMax / (repetitions - 1);
                        for (int i = 1; i < repetitions; i++)
                        { deepth[i] = i * stp; }
                    }


                    if (item is ItemPath PathData)
                    {
                        foreach (GCodeMotion entity in PathData.Path)
                        {
                            entity.Depth = deepth[0];
                        }
                        if (PathData.IsClosed)
                        {
                            int origLength = PathData.Path.Count;
                            for (int i = 1; i < repetitions; i++)
                            {
                                for (int k = 0; k < origLength; k++)
                                {
                                    if (PathData.Path[k] is GCodeLine line)
                                    {
                                        PathData.AddMotion(new GCodeLine(line));
                                        if (addZ)
                                            PathData.Path[PathData.Path.Count - 1].Depth = deepth[i];
                                    }
                                    else if (PathData.Path[k] is GCodeArc arc)
                                    {
                                        PathData.AddMotion(new GCodeArc(arc));
                                        if (addZ)
                                            PathData.Path[PathData.Path.Count - 1].Depth = deepth[i];
                                    }
                                }
                            }
                            repeatedGraphic.Add(item.Copy());
                        }
                        else // not closed
                        {
                            int origLength = PathData.Path.Count;
                            int reps = 1;
                            while (reps < repetitions)
                            {
                                for (int k = origLength - 1; k >= 0; k--)
                                {
                                    if (PathData.Path[k] is GCodeLine line)
                                    {
                                        PathData.AddMotion(new GCodeLine(line));
                                        if (addZ)
                                            PathData.Path[PathData.Path.Count - 1].Depth = deepth[reps];
                                    }
                                    else if (PathData.Path[k] is GCodeArc arc)
                                    {
                                        PathData.AddMotion(new GCodeArc(arc));
                                        if (addZ)
                                            PathData.Path[PathData.Path.Count - 1].Depth = deepth[reps];
                                    }
                                }
                                reps++;
                                if (reps == repetitions)
                                    break;
                                for (int k = 0; k < origLength; k++)
                                {
                                    if (PathData.Path[k] is GCodeLine line)
                                    {
                                        PathData.AddMotion(new GCodeLine(line));
                                        if (addZ)
                                            PathData.Path[PathData.Path.Count - 1].Depth = deepth[reps];
                                    }
                                    else if (PathData.Path[k] is GCodeArc arc)
                                    {
                                        PathData.AddMotion(new GCodeArc(arc));
                                        if (addZ)
                                            PathData.Path[PathData.Path.Count - 1].Depth = deepth[reps];
                                    }
                                }
                                reps++;
                            }
                            PathData.End = PathData.Path[PathData.Path.Count - 1].MoveTo;
                            repeatedGraphic.Add(item.Copy());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < repetitions; i++)
                        {
                            if (item is ItemDot PathDot)
                            {
                                if (addZ)
                                {
                                    PathDot.OptZ = deepth[i];
                                    PathDot.UseZ = true;
                                }
                            }
                            repeatedGraphic.Add(item.Copy());
                        }
                    }
                }
                /*     else // if (addZ)
                     {
                         for (int i = 0; i < repetitions; i++)
                         { repeatedGraphic.Add(item.Copy()); }
                     }*/
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
            SetPenFill("none");
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


        private static void ExtendClosedPaths(List<PathObject> graphicToExtend, double extensionOrig, bool shorten = false)
        {
            //const uint loggerSelect = (uint)LogEnable.PathModification;
            bool log = logEnable && ((logFlags & (uint)LogEnables.ClipCode) > 0);

            double dx, dy, newX, newY, length, maxElements;
            double extension;
            Point newStart = new Point();
            Point newEnd = new Point();
            if (logEnable) Logger.Trace("...ExtendClosedPaths extend:{0}", extensionOrig);

            foreach (PathObject item in graphicToExtend)
            {
                if (item is ItemPath PathData)
                {
                    if (shorten && (PathData.Path.Count > 1))
                    {
                        /* shorten the start of a path */
                        extension = Math.Abs(extensionOrig);
                        for (int index = 1; index < PathData.Path.Count; index++)
                        {
                            if (PathData.Path[index] is GCodeLine)
                            {
                                dx = PathData.Path[index - 1].MoveTo.X - PathData.Path[index].MoveTo.X;
                                dy = PathData.Path[index - 1].MoveTo.Y - PathData.Path[index].MoveTo.Y;
                                length = Math.Sqrt(dx * dx + dy * dy);
                                if (length > extension)
                                {
                                    PathData.Path[index - 1].MoveTo = newStart = ExtendPath(PathData.Path[index - 1].MoveTo, PathData.Path[index].MoveTo, extension, ExtendDragPath.startLater);
                                    PathData.Start = newStart;
                                    break;
                                }
                                else if (length == extension)
                                {
                                    PathData.Path.RemoveAt(index - 1);
                                    PathData.End = newStart;
                                    break;
                                }
                                else
                                {
                                    extension -= length;
                                    PathData.Path.RemoveAt(--index);
                                    PathData.End = newStart;
                                }
                            }
                            else    // is Arc
                            {
                            }
                        }
                        /* shorten the end of a path */
                        extension = Math.Abs(extensionOrig);
                        for (int index = PathData.Path.Count - 1; index >= 1; index--)
                        {
                            if (PathData.Path[index] is GCodeLine)
                            {
                                dx = PathData.Path[index].MoveTo.X - PathData.Path[index - 1].MoveTo.X;
                                dy = PathData.Path[index].MoveTo.Y - PathData.Path[index - 1].MoveTo.Y;
                                length = Math.Sqrt(dx * dx + dy * dy);
                                if (length > extension)
                                {
                                    PathData.Path[index].MoveTo = newEnd = ExtendPath(PathData.Path[index - 1].MoveTo, PathData.Path[index].MoveTo, extension, ExtendDragPath.endEarlier);
                                    PathData.End = newEnd;
                                    break;
                                }
                                else if (length == extension)
                                {
                                    PathData.Path.RemoveAt(index);
                                    PathData.End = newEnd;
                                    break;
                                }
                                else
                                {
                                    extension -= length;
                                    PathData.Path.RemoveAt(index);
                                    PathData.End = newEnd;
                                }
                            }
                            else    // is Arc
                            {
                                /*		GCodeArc ArcData = (GCodeArc)PathData.Path[index];
                                        ArcProperties arcProp = GcodeMath.GetArcMoveProperties(PathData.Path[index - 1].MoveTo, ArcData.MoveTo, ArcData.CenterIJ, ArcData.IsCW); // in radians
                                        double circum = Math.Abs(arcProp.angleDiff * arcProp.radius);
                                        if (log) Logger.Trace("      diff:{0:0.00}  radius:{1:0.00}    circ:{2:0.00}", arcProp.angleDiff, arcProp.radius, circum);

                                        if (log) Logger.Trace("    {0} Arc  circum:{1:0.00}  extend:{2:0.00}", PathData.Info.Id, circum, extension);
                                        if (circum > extension)
                                        {
                                            extension -= circum;
                                            PathData.AddArc(ArcData.MoveTo, ArcData.CenterIJ, ArcData.IsCW, ArcData.Depth, ArcData.AngleStart, ArcData.Angle);
                                            goto NextElement;
                                        }
                                        // interpolate new pos.
                                        double angleNewDiff = arcProp.angleDiff * extension / circum;
                                        double angleEnd = arcProp.angleStart + angleNewDiff;
                                        nwX = arcProp.center.X + arcProp.radius * Math.Cos(angleEnd);
                                        nwY = arcProp.center.Y + arcProp.radius * Math.Sin(angleEnd);
                                        if (log) Logger.Trace("      diff:{0:0.00}  new diff:{1:0.00}    end:{2:0.00}  ext.{3}", arcProp.angleDiff, angleNewDiff, angleEnd, extension);

                                        PathData.AddArc(new Point(nwX, nwY), ArcData.CenterIJ, ArcData.IsCW, ArcData.Depth, ArcData.AngleStart, ArcData.Angle);
                                        break;
                                    */
                            }
                        }
                    }
                    else //if (shorten && (PathData.Path.Count > 1))
                    {
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
                    }   // shorten
                }
            }
        }

        private static void AddZProfile(List<PathObject> completeGraphic)//, double maxDepth, double rampLength)
        {
            /* Insert Z profile - mode ramp at start and end 
			 1) 1st Path segment is long enough: split and get one move from z=0 to z=max
			 2) Only one segment and too short - split in the middle, middle = z max
			 3) more than one segments needed to archive needed length - split z positions
			 4) if angle between two segemnts is too high, add ramp
			*/
            double stepWidth = (double)Properties.Settings.Default.DevicePlotterAddZGradientStepWidth;
            double rampLength = (double)Properties.Settings.Default.DevicePlotterAddZGradientRampLength;
            double maxDepth = (double)Properties.Settings.Default.DevicePlotterToolDiameter;

            double dx, dy, newX, newY, maxElements, zToApply;
            double segment, segmentFront, segmentRear, pathFullLength, zFactor;
            double lengthReached, lengthRemains;
            Point newStart = new Point();
            Point newEnd = new Point();

            bool log = false;

            segment = Math.Abs(rampLength);
            zFactor = maxDepth / segment;

            foreach (PathObject item in completeGraphic)
            {
                if (item is ItemPath PathData)
                {
                    // get distance from one index-point to previous point
                    double getDistance(int index)
                    {
                        double ddx = PathData.Path[index - 1].MoveTo.X - PathData.Path[index].MoveTo.X;
                        double ddy = PathData.Path[index - 1].MoveTo.Y - PathData.Path[index].MoveTo.Y;
                        return Math.Sqrt(ddx * ddx + ddy * ddy);
                    }

                    // add point before index-point, with target depth, fill gap with intermediate points
                    int splitPathSetZ(int index, double offset, double maxZ, bool front)
                    {
                        if (log) Logger.Trace("splitPathSetZ  offset%:{0:0.000}  i:{1}  i-1:{2}  ", offset, PathData.Path[index].MoveTo, PathData.Path[index - 1].MoveTo);
                        double deltaX = PathData.Path[index].MoveTo.X - PathData.Path[index - 1].MoveTo.X;
                        double deltaY = PathData.Path[index].MoveTo.Y - PathData.Path[index - 1].MoveTo.Y;
                        double startZ = PathData.Path[index - 1].Depth;
                        double endZ = PathData.Path[index].Depth;
                        double deltaZ = endZ - startZ;
                        if (deltaZ == 0)
                            deltaZ = maxZ;
                        double segmentX = deltaX * offset;
                        double segmentY = deltaY * offset;

                        int steps = (int)(Math.Max(Math.Abs(segmentX), Math.Abs(segmentY)) / stepWidth);
                        if (log) Logger.Trace("step {0:0.000}  {1:0.000}  {2:0.000} st:{3}  e:{4}  d:{5}", steps, segmentX, segmentY, startZ, endZ, deltaZ);
                        GCodeLine ni;
                        if (steps <= 1)
                        {
                            newX = PathData.Path[index].MoveTo.X - segmentX;
                            newY = PathData.Path[index].MoveTo.Y - segmentY;
                            ni = new GCodeLine(new Point(newX, newY), endZ);
                            PathData.Path.Insert(index, ni);
                            return 1;
                        }
                        double stepX = segmentX / (double)steps;
                        double stepY = segmentY / (double)steps;
                        double stepZ = deltaZ / steps;

                        if (log) Logger.Trace("step 2 stepX {0:0.000}  stepY {1:0.000}  {2:0.000} st:{3}  e:{4}  d:{5}", stepX, stepY, segmentX, segmentY, endZ, deltaZ);
                        //for (int k=1; k<steps; k++)
                        double originX, originY;
                        if (front)
                        {
                            originX = PathData.Path[index - 1].MoveTo.X;
                            originY = PathData.Path[index - 1].MoveTo.Y;
                            for (int k = steps; k > 0; k--)
                            {
                                newX = originX + stepX * k;
                                newY = originY + stepY * k;
                                endZ = startZ + stepZ * k;
                                ni = new GCodeLine(new Point(newX, newY), endZ);
                                PathData.Path.Insert(index, ni);
                                if (log) Logger.Trace("Insert 1 {0:0.000}  {1:0.000}  {2:0.000} ", newX, newY, endZ);
                            }
                        }
                        else
                        {
                            originX = PathData.Path[index].MoveTo.X;
                            originY = PathData.Path[index].MoveTo.Y;
                            for (int k = 1; k <= steps; k++)
                            {
                                newX = originX - stepX * k;
                                newY = originY - stepY * k;
                                startZ = endZ - stepZ * k;
                                ni = new GCodeLine(new Point(newX, newY), startZ);
                                PathData.Path.Insert(index, ni);
                                if (log) Logger.Trace("Insert 2 {0:0.000}  {1:0.000}  {2:0.000} ", newX, newY, endZ);
                            }
                        }
                        return steps;
                    }

                    pathFullLength = PathData.PathLength;
                    PathData.Path[0].Depth = PathData.Path[PathData.Path.Count - 1].Depth = 0;  // start and end = z=0
                    bool shortPath = false;
                    if (pathFullLength <= 2 * segment)
                    {   // full depth at half of full length
                        segment = Math.Abs(pathFullLength / 2);
                        zFactor = maxDepth / segment;
                        shortPath = true;
                    }
                    else
                    {
                        segment = Math.Abs(rampLength);
                        zFactor = maxDepth / segment;
                    }

                    segmentFront = segment;
                    segmentRear = segment;

                    if (PathData.Path.Count > 1)
                    {
                        if (log) Logger.Trace(" split ----------------------");
                        lengthReached = 0;
                        lengthRemains = pathFullLength;
                        double delta, diff;
                        bool wasSplitFront = false;
                        bool wasSplitRear = false;
                        for (int index = 1; index < PathData.Path.Count; index++)
                        {
                            if (PathData.Path[index] is GCodeLine)
                            {
                                delta = getDistance(index);
                                lengthReached += delta;
                                lengthRemains -= delta;
                                zToApply = maxDepth;
                                if (lengthReached < segment) zToApply = lengthReached * zFactor;
                                if (lengthRemains < segment) zToApply = lengthRemains * zFactor;
                                if (log) Logger.Trace(" split  index:{0}  lengthReached:{1:0.000}   lengthRemains:{2:0.000}  z:{3:0.000}", index, lengthReached, lengthRemains, zToApply);

                                PathData.Path[index].Depth = zToApply;       // add depth to each intermediate step
                                if (!wasSplitFront)
                                {
                                    if (lengthReached < segment)
                                    {
                                        index += splitPathSetZ(index, 1, maxDepth, true);
                                    }
                                    else
                                    {
                                        wasSplitFront = true;
                                        diff = delta - segmentFront;    //lengthReached - segment;
                                        if (log) Logger.Trace(" split-F  delta:{0:0.000}   diff:{1:0.000}", delta, diff);
                                        index += splitPathSetZ(index, segmentFront / delta, maxDepth, true);
                                    }
                                }
                                if (!wasSplitRear)
                                {
                                    if (lengthRemains < segment)
                                    {
                                        wasSplitRear = true;
                                        diff = segment - lengthRemains;
                                        if (log) Logger.Trace(" split-R  delta:{0:0.000}   diff:{1:0.000}", delta, diff);
                                        index += splitPathSetZ(index, diff / delta, maxDepth, false);
                                    }
                                }
                                else
                                {
                                    index += splitPathSetZ(index, 1, maxDepth, false);
                                }
                                segmentFront -= delta;
                            }
                            else    // is ArcData
                            { }
                        }
                        //    continue;
                    }
                }
            }
        }

        private static void SmoothPath(ItemPath item)//(ItemPath path)
        {
            // https://github.com/burningmime/curves
            List<System.Numerics.Vector2> sourcePoints = new List<System.Numerics.Vector2>();

            if (item is ItemPath PathData)
            {
                double error = 0.5;
                int ji = 0;
                for (int j = 0; j < PathData.Path.Count; j++)
                {
                    sourcePoints.Add(new System.Numerics.Vector2((float)PathData.Path[j].MoveTo.X, (float)PathData.Path[j].MoveTo.Y));
                }
                if (sourcePoints.Count > 1)
                {
                    float rdpError = 0.1f;     // 1-8
                    float fitError = (float)Properties.Settings.Default.createTextHersheySmoothCurveFittingError;     // 1-25
                //    List<System.Numerics.Vector2> ppPts = CurvePreprocess.RdpReduce(sourcePoints, rdpError);        //preprocess(inPts, ppMode, linDist, rdpError);
                //    CubicBezier[] curves = CurveFit.Fit(ppPts, fitError);
                    CubicBezier[] curves = CurveFit.Fit(sourcePoints, fitError);

                    PathData.Path.Clear();
                    foreach (CubicBezier curve in curves)
                    {
                        ImportMath.CalcCubicBezier(toPointD(curve.p0), toPointD(curve.p1), toPointD(curve.p2), toPointD(curve.p3), AddResultPath, "");
                    }
                    PathData.Start = PathData.Path[0].MoveTo;
                    PathData.End = PathData.Path[PathData.Path.Count - 1].MoveTo;
                }

                System.Windows.Point toPointD(System.Numerics.Vector2 p)
                { return new System.Windows.Point(p.X, p.Y); }

                void AddResultPath(System.Windows.Point p, string cmt)
                {
                    PathData.Path.Add(new GCodeLine(p));
                }
            }
        }


        internal struct PathProp
        {
            internal int index;
            internal int size;
            internal int group;
            internal int distance;
            internal Point start;
        }

        private static void SortByDimension(List<PathObject> graphicToSort)
        {
            //logSortMerge = true;
            if (logSortMerge) Logger.Trace("...SortByDimension() count:{0}", graphicToSort.Count);

            if (graphicToSort.Count <= 2)
            {
                Logger.Info("...SortByDimension() nothing to sort - count:{0}", graphicToSort.Count);
                return;
            }

            List<PathProp> SizesChar = new List<PathProp>();
            List<PathProp> SizesLines = new List<PathProp>();
            List<PathProp> SizesClosed = new List<PathProp>();
            PathProp pp;

            // 1. group by type: char, line closed-path
            for (int i = 0; i < graphicToSort.Count; i++)
            {
                pp = new PathProp();
                pp.index = i;
                if (graphicToSort[i].Info.PathGeometry.Contains("Char"))
                { SizesChar.Add(pp); continue; }
                else if (IsEqual(graphicToSort[i].Start, graphicToSort[i].End))
                {
                    pp.size = (int)graphicToSort[i].Dimension.dimx * (int)graphicToSort[i].Dimension.dimy;
                    SizesClosed.Add(pp); continue;
                }
                else
                { SizesLines.Add(pp); continue; }
            }
            if (logSortMerge) Logger.Trace("...SortByDimension() char:{0}  lines:{1}  closed-paths:{2}", SizesChar.Count, SizesLines.Count, SizesClosed.Count);

            int firstSize = 0, lastSize = 0;
            int maxChangeFactor = 2;
            int maxDiffFactor = 100;
            int grp = 0;
            PathProp tmp;
            if (SizesClosed.Count > 0)
            {
                // 2. sort closed-paths by size
                SizesClosed.Sort((x, y) => x.size.CompareTo(y.size));
                firstSize = lastSize = SizesClosed[0].size;

                // 3. group closed-paths by size - later sort similar sizes by distance
                for (int i = 0; i < SizesClosed.Count; i++)
                {
                    tmp = SizesClosed[i];
                    if ((SizesClosed[i].size > lastSize * maxChangeFactor) || (SizesClosed[i].size > firstSize * maxDiffFactor))
                    { grp++; firstSize = SizesClosed[i].size; }
                    tmp.group = grp;
                    lastSize = SizesClosed[i].size;
                    SizesClosed[i] = tmp;

                    if (logSortMerge) Logger.Trace("...SortByDimension() i:{0}  size:{1}, grp:{2}  type:{3}", i, lastSize, tmp.group, graphicToSort[tmp.index].Info.PathGeometry);
                }
            }

            List<PathObject> sortedGraphic = new List<PathObject>();
            Point actualPos = new Point(0, 0);  // start

            // add char unsorted
            if (SizesChar.Count > 0)
            {
                for (int i = 0; i < SizesChar.Count; i++)
                {
                    sortedGraphic.Add(graphicToSort[SizesChar[i].index]);
                }
                actualPos = graphicToSort[SizesChar[SizesChar.Count - 1].index].End;
            }
            // add lines sorted by distance
            if (SizesLines.Count > 0)
            {
                double dist1 = PointDistanceSquared(actualPos, graphicToSort[SizesLines[0].index].Start);
                double dist2 = PointDistanceSquared(actualPos, graphicToSort[SizesLines[0].index].End);
                if (dist1 < dist2) { actualPos = graphicToSort[SizesLines[0].index].Start; }
                else
                { actualPos = graphicToSort[SizesLines[0].index].End; }
                sortedGraphic.AddRange(SortByDistance(graphicToSort, SizesLines, 0, SizesLines.Count - 1, ref actualPos));
            }
            int lastI = 0;

            // 4. sort closed-paths groups by distance and add
            if (grp > 0)
            {
                for (int i = 1; i < SizesClosed.Count; i++)
                {
                    if (SizesClosed[i - 1].group != SizesClosed[i].group)
                    {
                        sortedGraphic.AddRange(SortByDistance(graphicToSort, SizesClosed, lastI, i - 1, ref actualPos));
                        if (logSortMerge) Logger.Trace("... new group {0}  start:{1}  end:{2}", SizesClosed[i - 1].group, lastI, i - 1);
                        lastI = i;
                    }
                }
                sortedGraphic.AddRange(SortByDistance(graphicToSort, SizesClosed, lastI, SizesClosed.Count - 1, ref actualPos));
                if (logSortMerge) Logger.Trace("... end group  start:{0}  end:{1}", lastI, SizesClosed.Count - 1);
            }
            else
            {
                sortedGraphic.AddRange(SortByDistance(graphicToSort, SizesClosed, 0, SizesClosed.Count - 1, ref actualPos));
                if (logSortMerge) Logger.Trace("... no group  start:{0}  end:{1}", 0, SizesClosed.Count - 1);
            }

            graphicToSort.Clear();
            foreach (PathObject item in sortedGraphic)      // replace original list
                graphicToSort.Add(item);

            sortedGraphic.Clear();
            SizesChar.Clear();
            SizesLines.Clear();
            SizesClosed.Clear();
        }
        private static List<PathObject> SortByDistance(List<PathObject> graphicToSort, List<PathProp> order, int start, int end, ref Point origin)
        {
            List<PathObject> tmp = new List<PathObject>();
            int index;
            for (int i = start; i <= end; i++)
            {
                index = order[i].index;
                tmp.Add(graphicToSort[index]);
            }
            origin = SortByDistance(tmp, origin, true, false, false);
            return tmp;
        }

        private static void SortByDimension2(List<PathObject> graphicToSort)
        {
            // 1. sort by dimension - largest first
            // 2. sort by location
            // 3. reverse order - smallest = innerst first
            if (logEnable) Logger.Trace("...SortByDimension() count:{0}", graphicToSort.Count);
            if (graphicToSort.Count <= 1)
            {
                Logger.Info("...SortByDimension() nothing to sort - count:{0}", graphicToSort.Count);
                return;
            }

            stopwatch = new Stopwatch();
            stopwatch.Start();

            PathObject tmp;

            // 1. sort by dimension - largest first
            for (int i = 0; i < graphicToSort.Count; i++)
            {
                tmp = graphicToSort[i];
                tmp.Distance = tmp.Dimension.dimx * tmp.Dimension.dimy;
                graphicToSort[i] = tmp;
            }
            graphicToSort.Sort((x, y) => y.Distance.CompareTo(x.Distance));   // sort by size, large first

            List<PathObject> sortedGraphic = new List<PathObject>();
            List<Dimensions> lastDim = new List<Dimensions>();
            double minX, minY, maxX, maxY;
            Dimensions dim;
            tmp = graphicToSort[0];
            dim = new Dimensions(tmp.Dimension);
            lastDim.Add(dim);
            sortedGraphic.Add(tmp);
            graphicToSort.RemoveAt(0);

            while (graphicToSort.Count > 0)                      // items will be removed step by step from graphicToSort
            {
                for (int k = lastDim.Count - 1; k >= 0; k--)
                {
                    dim = lastDim[k];
                    //    if (logEnable) Logger.Trace("   set k:{0}  ddx:{1:0.0}  ddy:{2:0.0}  minx:{3:0.0}  miny:{4:0.0}", k, dim.dimx, dim.dimy, dim.minx, dim.miny);
                    for (int i = 0; i < graphicToSort.Count; i++)
                    {
                        tmp = graphicToSort[i];
                        if (tmp.Dimension.IsWithin(dim))
                        {
                            dim = new Dimensions(tmp.Dimension);
                            //    if (logEnable) Logger.Trace("   added i:{0}  ddx:{1:0.0}  ddy:{2:0.0}  minx:{3:0.0}  miny:{4:0.0}", i , tmp.Dimension.dimx, tmp.Dimension.dimy, tmp.Dimension.minx, tmp.Dimension.miny);
                            if (tmp.Dimension.dimx > 0)
                                lastDim.Add(dim);
                            sortedGraphic.Add(graphicToSort[i]);
                            graphicToSort.RemoveAt(i);
                            i--;
                            k = lastDim.Count - 1;
                        }
                    }
                }

                if (graphicToSort.Count > 0)
                {
                    tmp = graphicToSort[0];
                    dim = new Dimensions(tmp.Dimension);
                    //    if (logEnable) Logger.Trace("   next top  ddx:{0:0.0}  ddy:{1:0.0}  minx:{2:0.0}  miny:{3:0.0}", tmp.Dimension.dimx, tmp.Dimension.dimy, tmp.Dimension.minx, tmp.Dimension.miny);
                    if (tmp.Dimension.dimx > 0)
                        lastDim.Add(dim);
                    sortedGraphic.Add(graphicToSort[0]);
                    graphicToSort.RemoveAt(0);
                }
            }

            graphicToSort.Clear();
            foreach (PathObject item in sortedGraphic)      // replace original list
                graphicToSort.Add(item);

            sortedGraphic.Clear();
            graphicToSort.Reverse();
            if (logEnable) Logger.Trace("...SortByDimension()  finish");
        }

        private static Point SortByDistance(List<PathObject> graphicToSort, Point actualPos, bool closedPathRotate, bool largestLast, bool preventReversal)
        {
            if (logEnable)
                Logger.Trace("...SortByDistance() count:{0}  start X:{1:0.00} y:{2:0.00}  rotate:{3}  largestlast:{4}  preventReverse:{5}", graphicToSort.Count, actualPos.X, actualPos.Y, closedPathRotate, largestLast, preventReversal);
            stopwatch = new Stopwatch();
            stopwatch.Start();

            List<PathObject> sortedGraphic = new List<PathObject>();

            double distanceReverse;
            bool allowReverse = true;
            PathObject tmp;

            int maxElements = graphicToSort.Count;
            double minDist;
            int minDistIndex;

            while ((graphicToSort.Count > 0) && (!cancelByWorker))                      // items will be removed step by step from completeGraphic
            {
                if (backgroundWorker != null)           // 2023-08-16 pull request Speed up merge and sort #348 Reduce CPU used updating progress bars
                {
                    if (UpdateGUI()) backgroundWorker.ReportProgress(((maxElements - graphicToSort.Count) * 100) / maxElements);
                    if (backgroundWorker.CancellationPending)
                    {
                        cancelByWorker = true;
                        backgroundWorker.ReportProgress(100, new MyUserState { Value = 100, Content = "Stop processing, clean up data. Please wait!" });
                        break;
                    }
                }
                minDist = double.MaxValue;
                minDistIndex = -1;
                for (int i = 0; i < graphicToSort.Count; i++)     // calculate distance to all remaining items check start and end position
                {
                    tmp = graphicToSort[i];
                    tmp.Distance = PointDistanceSquared(actualPos, tmp.Start);

                    if (tmp is ItemPath tmpItemPath)
                    {
                        /* if closed path, find closest point */
                        if (IsEqual(tmp.Start, tmp.End))
                        {
                            if (closedPathRotate && !preventReversal)
                            {
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


                    if (tmp.Distance < minDist) // 2023-08-16 pull request Speed up merge and sort #348    Remove unnecessary sort
                    {
                        minDist = tmp.Distance;
                        minDistIndex = i;
                    }
                }

                if (minDistIndex != -1)     // 2023-08-16 pull request Speed up merge and sort #348    Remove unnecessary sort
                {
                    sortedGraphic.Add(graphicToSort[minDistIndex]);         // get closest item = first in list
                    actualPos = graphicToSort[minDistIndex].End;            // set new start pos
                    if (logSortMerge) Logger.Trace("   remove id:{0} ", graphicToSort[minDistIndex].Info.Id);
                    graphicToSort.RemoveAt(minDistIndex);                  // remove item from remaining list
                }
            }

            if (cancelByWorker)//stopwatch.Elapsed.Minutes >= maxTimeMinute)  // time expired, just copy missing figures
            {
                foreach (PathObject tmpgrp in graphicToSort)
                    sortedGraphic.Add(tmpgrp);
            }

            List<int> iLargest = new List<int>();
            double tlarge, largest = 0;
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

                if (sortedItem is ItemPath tmpItemPath)
                {
                    tlarge = Math.Round(tmpItemPath.Dimension.dimx + tmpItemPath.Dimension.dimy);
                    if (tlarge > largest)
                    {
                        iLargest.Clear();
                        largest = tlarge;
                        iLargest.Add(i);
                        if (logEnable) Logger.Trace("    Larger new: id:{0}  size:{1:0.00}", i, largest);
                    }
                    else if (tlarge == largest)
                    {
                        if (logEnable) Logger.Trace("    Larger same:id:{0}  size:{1:0.00}", i, largest);
                        iLargest.Add(i);
                    }
                    else
                    {
                        if (logEnable) Logger.Trace("    Larger not :id:{0}  size:{1:0.00}", i, largest);
                    }
                }
            }

            graphicToSort.Clear();
            foreach (PathObject item in sortedGraphic)      // replace original list
                graphicToSort.Add(item);

            sortedGraphic.Clear();

            if (largestLast)	//Properties.Settings.Default.importGraphicLargestLast)   // move largest object to the end
            {
                if (logEnable) Logger.Trace("...SortByDistance move largest objects to the end id:{0}", String.Join("; ", iLargest));
                for (int i = 0; i < iLargest.Count; i++)
                    graphicToSort.Add(graphicToSort[iLargest[i]]);

                for (int i = iLargest.Count - 1; i >= 0; i--)
                    graphicToSort.RemoveAt(iLargest[i]);
            }

            if (logEnable) Logger.Trace("...SortByDistance()  finish  last pos: X:{0:0.00} y:{1:0.00}", actualPos.X, actualPos.Y);
            return actualPos;
        }
        private static double PointDistanceSquared(Point a, Point b)    // avoid square-root, to save time
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (dx * dx + dy * dy); // speed up
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

            tmp.Distance = distance;
            tmp.StartAngle = aLine;
        }
        private static void PathDistanceSquared(Point a, ItemPath tmp)  // go through coordinates of path
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
            }
            tmp.Distance = distance;
            tmp.TmpIndex = index;
            if (logSortMerge) Logger.Trace("       path a.x:{0:0.00}  a.y:{1:0.00}  distance:{2:0.00}   i:{3}", a.X, a.Y, distance, index);
        }
        private static Point ToPointF(XyPoint tmp)
        { return new Point((float)tmp.X, (float)tmp.Y); }

        private static bool HasSameProperties(ItemPath a, ItemPath b, bool importLineDashPattern)       // 2023-08-16 pull request Speed up merge and sort #348    Improve comparing GroupAttributes
        {
            if (graphicInformation.GroupEnable)
            {
                for (int i = 1; i < a.Info.GroupAttributes.Count; i++)    // GroupOptions { none = 0, ByColor = 1, ByWidth = 2, ByLayer = 3, ByType = 4, ByTile = 5};
                {
                    if (logDetailed) Logger.Trace("  hasSameProperties - GroupEnable-Option:{0} a:'{1}'  b:'{2}'", i, a.Info.GroupAttributes[i], b.Info.GroupAttributes[i]);
                    if (a.Info.GroupAttributes[i] != b.Info.GroupAttributes[i])
                        return false;
                }
            }
            else
            {
                if (a.Info.GroupAttributes[1] != b.Info.GroupAttributes[1])     // 2024-08-11 at least check color
                    return false;
            }

            if (importLineDashPattern)
            {
                if ((a.DashArray.Length == 0) && (b.DashArray.Length == 0))
                {
                    return true;
                }
                else
                {
                    if (a.DashArray.Length != b.DashArray.Length)
                        return false;
                    for (int i = 1; i <= a.DashArray.Length; i++)       // 2023-08-16 pull request Speed up merge and sort #348    Improve comparing GroupAttributes
                    {
                        if (a.DashArray[i] != b.DashArray[i])
                            return false;
                    }
                }
            }
            return true;
        }

        private static void MergeFigures(List<PathObject> graphicToMerge, bool log = false)
        {
            if (log || logEnable) Logger.Trace("...MergeFigures before:{0}    ------------------------------------", graphicToMerge.Count);
            stopwatch = new Stopwatch();
            stopwatch.Start();

            int i, k;
            int maxElements = graphicToMerge.Count;
            bool importLineDashPattern = Properties.Settings.Default.importLineDashPattern; // Keep a local copy for speed

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
                            if (!HasSameProperties(ipath, kpath, importLineDashPattern))        // 2023-08-16 pull request Speed up merge and sort #348    Improve comparing GroupAttributes
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
            if (log || logEnable) Logger.Trace("...MergeFigures after :{0}    ------------------------------------", graphicToMerge.Count);
        }
        private static bool IsEqual(System.Windows.Point a, System.Windows.Point b, double ePrecision)
        { return ((Math.Abs(a.X - b.X) < ePrecision) && (Math.Abs(a.Y - b.Y) < ePrecision)); }
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
                if ((item.TmpIndex == 0) || (iStart >= item.Path.Count))           // nothing to rotate
                    return;

                tmpMove = new GCodeLine(item.Start);        // 
                rotatedPath.Add(tmpMove);

                for (i = iStart + 1; i < item.Path.Count; i++)
                { rotatedPath.Add(item.Path[i]); }
                for (i = 1; i <= iStart; i++)
                { rotatedPath.Add(item.Path[i]); }
            }

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
                    double newi = cx - item.Path[i + 1].MoveTo.X;
                    double newj = cy - item.Path[i + 1].MoveTo.Y;
                    tmpMove = new GCodeArc(item.Path[i].MoveTo, new Point(newi, newj), !((GCodeArc)item.Path[i + 1]).IsCW, item.Path[i + 1].Depth);
                    reversedPath.Add(tmpMove);
                }
            }
            item.Path.Clear();
            foreach (GCodeMotion ent in reversedPath)
                item.Path.Add(ent);

            if (!item.IsReversed)               // indicatior if Start/End was already switched (in SortCode()
            {
                Point tmp = item.End;       // if not, do now
                item.End = item.Start;
                item.Start = tmp;
            }
            item.IsReversed = false;            // reset indicator
        }

        public static void AlignLines(int align)    // apply offset to figures with same AuxInfo
        {
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
                    }
                    else
                    {
                        lineDim.Add(item.Info.AuxInfo, new Dimensions(item.Dimension));
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
                }

                foreach (PathObject item in completeGraphic)            // apply offsets
                {
                    offsetX = lineOffset[item.Info.AuxInfo];
                    item.Start = new Point(item.Start.X + offsetX, item.Start.Y + offsetY);
                    item.End = new Point(item.End.X + offsetX, item.End.Y + offsetY);
                    if (item is ItemPath PathData)
                    {
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