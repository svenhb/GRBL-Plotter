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

        public static void Rotate(double angleRad, double offsetX, double offsetY)  // scaleX != scaleY will not work for arc!
        { Rotate(completeGraphic, angleRad, offsetX, offsetY); }
        private static void Rotate(List<PathObject> graphicToRotate, double angleRad, double offsetX, double offsetY)
        {
            System.Diagnostics.StackTrace s = new System.Diagnostics.StackTrace(System.Threading.Thread.CurrentThread, true);
            if (logEnable) Logger.Trace("...Rotate a:{0:0.00} X:{1:0.00} Y:{2:0.00} caller:{3} --------------------------------------", angleRad * 180 / Math.PI, offsetX, offsetY, s.GetFrame(1).GetMethod().Name);
            foreach (PathObject item in graphicToRotate)    // dot or path
            {
                item.Start = RotatePoint(item.Start, angleRad, offsetX, offsetY);
                item.End = RotatePoint(item.End, angleRad, offsetX, offsetY);
                if (item is ItemPath PathData)
                {
                    foreach (GCodeMotion entity in PathData.Path)
                    {
                        entity.MoveTo = RotatePoint(entity.MoveTo, angleRad, offsetX, offsetY);
                        if (entity is GCodeArc arcEntity)
                        {
                            Point tmp = RotatePoint(arcEntity.CenterIJ, angleRad, 0, 0);
                            arcEntity.CenterIJ = new Point(tmp.X, tmp.Y);
                        }
                    }
                }
            }
            ReCalcDimension(graphicToRotate);
        }
        public static void MirrorX()    // scaleX != scaleY will not work for arc!
        {
            List<PathObject> graphicToMirror = completeGraphic;
            if (logEnable) Logger.Trace("...MirrorX  --------------------------------------");
            foreach (PathObject item in graphicToMirror)    // dot or path
            {
                item.Start = new Point(-item.Start.X, item.Start.Y);
                item.End = new Point(-item.End.X, item.End.Y);
                if (item is ItemPath PathData)
                {
                    foreach (GCodeMotion entity in PathData.Path)
                    {
                        entity.MoveTo = new Point(-entity.MoveTo.X, entity.MoveTo.Y);
                        if (entity is GCodeArc arcEntity)
                        {
                            arcEntity.CenterIJ = new Point(-arcEntity.CenterIJ.X, arcEntity.CenterIJ.Y);
                            arcEntity.IsCW = !arcEntity.IsCW;
                        }
                    }
                }
            }
            ReCalcDimension(graphicToMirror);
        }

        private static void ReCalcDimension(List<PathObject> graphicToReCalc)
        {
            if (logEnable) Logger.Trace("ReCalcDimension  before  dimx:{0:0.00}  dimy:{1:0.00}", actualDimension.dimx, actualDimension.dimy);
            actualDimension.ResetDimension();
            Point start;
            foreach (PathObject item in graphicToReCalc)    // dot or path
            {
                if (item is ItemPath PathData)
                {
                    item.Dimension.ResetDimension();
                    start = PathData.Start;
                    foreach (GCodeMotion entity in PathData.Path)
                    {
                        item.Dimension.SetDimensionXY(entity.MoveTo.X, entity.MoveTo.Y);
                        if (entity is GCodeArc arcEntity)
                        {
                            item.Dimension.SetDimensionArc(new XyPoint(start), new XyPoint(entity.MoveTo), arcEntity.CenterIJ.X, arcEntity.CenterIJ.Y, arcEntity.IsCW);
                        }
                        start = entity.MoveTo;
                    }
                }
                actualDimension.AddDimensionXY(item.Dimension);
            }
            if (logEnable) Logger.Trace("ReCalcDimension  after   dimx:{0:0.00}  dimy:{1:0.00}", actualDimension.dimx, actualDimension.dimy);
        }
        private static Point RotatePoint(Point p, double angleRad, double offX, double offY)
        {
            double newvalx = (p.X - offX) * Math.Cos(angleRad) - (p.Y - offY) * Math.Sin(angleRad);
            double newvaly = (p.X - offX) * Math.Sin(angleRad) + (p.Y - offY) * Math.Cos(angleRad);
            Point pn = new Point(newvalx + offX, newvaly + offY);
            return pn;
        }

        public static void ScaleXY(double scaleX, double scaleY)    // scaleX != scaleY will not work for arc!
        { Scale(completeGraphic, scaleX, scaleY); }

        private static void Scale(List<PathObject> graphicToOffset, double scaleX, double scaleY, int start = 0)
        {
            if (logEnable) Logger.Trace("...Scale scaleX:{0:0.00} scaleY:{1:0.00} ", scaleX, scaleY);
            for (int i = start; i < graphicToOffset.Count; i++)
            {
                var item = graphicToOffset[i];
                item.Start = new Point(item.Start.X * scaleX, item.Start.Y * scaleY);
                item.End = new Point(item.End.X * scaleX, item.End.Y * scaleY);
                if (item is ItemPath PathData)
                {
                    if (logEnable) Logger.Trace("  ID:{0} Geo:{1} Size:{2}", item.Info.Id, item.Info.PathGeometry, PathData.Path.Count);
                    foreach (GCodeMotion entity in PathData.Path)
                    {
                        entity.MoveTo = new Point(entity.MoveTo.X * scaleX, entity.MoveTo.Y * scaleY);
                        if (entity is GCodeArc arcEntity)
                        { arcEntity.CenterIJ = new Point(arcEntity.CenterIJ.X * scaleX, arcEntity.CenterIJ.Y * scaleY); }
                    }
                    PathData.Dimension.ScaleXY(scaleX, scaleY);
                }
            }
            actualDimension.ScaleXY(scaleX, scaleY);
        }

        #region remove offset
        internal static void RemoveOffset()
        {
            RemoveOffset(completeGraphic, actualDimension.minx, actualDimension.miny);
        }
        internal static void RemoveOffset(ItemPath tmp, double ox, double oy)
        {
            foreach (GCodeMotion entity in tmp.Path)
            { entity.MoveTo = new Point(entity.MoveTo.X - ox, entity.MoveTo.Y - oy); }
        }

        private static void RemoveOffset(List<PathObject> graphicToOffset, double offsetX, double offsetY, int start = 0)
        {
            // 2026-03-01 seperate "move largest object to the end"
            System.Diagnostics.StackTrace s = new System.Diagnostics.StackTrace(System.Threading.Thread.CurrentThread, true);
            List<int> iLargest = new List<int>();
            if (logEnable) Logger.Trace("...RemoveOffset before min X:{0:0.00} Y:{1:0.00} caller:{2} --------------------------------------", actualDimension.minx, actualDimension.miny, s.GetFrame(1).GetMethod().Name);

            PathObject item;
            for (int i = start; i < graphicToOffset.Count; i++)
            {
                item = graphicToOffset[i];
                item.Start = new Point(item.Start.X - offsetX, item.Start.Y - offsetY);
                item.End = new Point(item.End.X - offsetX, item.End.Y - offsetY);
                item.Dimension.OffsetXY(-offsetX, -offsetY);
                //      Logger.Trace("RemoveOffset i:{0}   id:{1}   geo:{2}", i, item.FigureId, item.Info.Id);
                if (item is ItemPath PathData)
                {
                    foreach (GCodeMotion entity in PathData.Path)
                    { entity.MoveTo = new Point(entity.MoveTo.X - offsetX, entity.MoveTo.Y - offsetY); }
                }
            }
            actualDimension.OffsetXY(-offsetX, -offsetY);
        }
        private static void LargestLast(List<PathObject> graphicToOffset, bool removeLast)
        {
            // only move/remove objects with a single ID
            if (graphicToOffset.Count < 3)
            {
                Logger.Trace("LargestLast nothing to do Count:{0}", graphicToOffset.Count);
                return;
            }
            List<int> iLargest = new List<int>();
            double tlarge, largest = 0;

            int pathIdLast = graphicToOffset[0].Info.Id;
            int pathIdNow = graphicToOffset[1].Info.Id;
            int pathIdNext;
            PathObject item;
            for (int i = 2; i < graphicToOffset.Count; i++)
            {
                item = graphicToOffset[i];
                //        Logger.Trace("LargestLast i:{0}   id:{1}   geo:{2}", i, item.FigureId, item.Info.Id);

                pathIdNext = item.Info.Id;
                if (item is ItemPath PathData)
                {
                    if ((pathIdNow != pathIdLast) && (pathIdNow != pathIdNext))
                    {
                        tlarge = Math.Round(PathData.Dimension.dimx + PathData.Dimension.dimy);
                        if (tlarge > largest)
                        {
                            iLargest.Clear();
                            largest = tlarge;
                            iLargest.Add(i);
                            //        Logger.Trace("LargestLast clear/add {0} ", i);
                        }
                        else if (tlarge == largest)
                        {
                            iLargest.Add(i);
                            //        Logger.Trace("LargestLast add {0} ", i);
                        }
                    }
                    pathIdLast = pathIdNow;
                    pathIdNow = pathIdNext;
                }
            }
            {
                if (!removeLast)
                {
                    //  if (logEnable) 
                    Logger.Trace("...LargestLast move largest objects to the end ({0}), ids:{1}  figures:{2}", iLargest.Count, String.Join("; ", iLargest), graphicToOffset.Count);
                    for (int li = 0; li < iLargest.Count; li++)
                    {
                        if (iLargest[li] < graphicToOffset.Count)
                        {
                            //  graphicToOffset[iLargest[li]].FigureId
                            graphicToOffset.Add(graphicToOffset[iLargest[li]]); //  1st add largest at the end
                            Logger.Trace("...LargestLast add largest objects to the end {0} figures:{1}  geo:{2}", li, graphicToOffset.Count, graphicToOffset[iLargest[li]].Info.AuxInfo);
                        }
                    }
                }
                else
                   if (logEnable) Logger.Trace("...LargestLast remove largest objects ({0}), ids:{1}", iLargest.Count, String.Join("; ", iLargest));

                for (int li = iLargest.Count - 1; li >= 0; li--)
                {
                    if (graphicToOffset.Count > iLargest[li])
                    {
                        graphicToOffset.RemoveAt(iLargest[li]);                 // 2nd remove largest from origin index
                        Logger.Trace("...LargestLast remove largest objects {0}  {1}", li, graphicToOffset.Count);
                    }
                }
            }
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
                    Point lastPoint = PathData.End;
                    double angleNow = 0;                       // when adjacent line segments have the same angle - end point of first can be removed 
                    double angleLast = 0;
                    double zNow = 0;
                    double zLast = 0;
                    bool isLineNow = false;
                    bool isLineLast = false;

                    if (PathData.Path.Count > 2)
                    {
                        zLast = PathData.Path[PathData.Path.Count - 1].Depth;
                        for (int i = (PathData.Path.Count - 2); i >= 0; i--)
                        {
                            if (PathData.Path[i] is GCodeLine)
                            {
                                angleNow = GcodeMath.GetAlpha(PathData.Path[i].MoveTo, lastPoint);
                                zNow = PathData.Path[i].Depth;
                                PathData.Path[i].Angle = angleNow;
                                isLineNow = true;
                            }
                            else
                            {
                                PathData.Path[i].Angle = angleNow;
                                angleNow = angleLast + 1;
                                zNow = zLast + 1;
                                isLineNow = false;
                            }

                            if (isLineNow && isLineLast && IsEqual(angleNow, angleLast) && IsEqual(zNow, zLast))
                            {
                                if (((i + 2) < PathData.Path.Count) && (PathData.Path[i + 2] is GCodeLine) && (IsEqual(zNow, PathData.Path[i + 2].Depth)))	// don't delete if start-point for arc
                                {
                                    PathData.Path.RemoveAt(i + 1);
                                    removed++;
                                }
                            }
                            else
                            {
                                angleLast = angleNow;
                                zLast = zNow;
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
            bool backward = !Properties.Settings.Default.importDepthFromWidth;
            List<int> indexToRemove = new List<int>();


            int countAll = 0;
            foreach (PathObject item in graphicToImprove)    // dot or path
            {
                if (item is ItemPath PathData)
                {
                    Point lastPoint = PathData.Path[PathData.Path.Count - 1].MoveTo;  // PathData.End;
                    double lastAngle = PathData.Path[PathData.Path.Count - 1].Angle;
                    double lastZ = PathData.Path[PathData.Path.Count - 1].Depth;
                    double distance;
                    if (PathData.Path.Count > 3)
                    {
                        if (backward)
                        {
                            int count = 0;
                            for (int i = (PathData.Path.Count - 2); i > 0; i--)
                            {
                                if (PathData.Path[i] is GCodeLine)
                                    distance = Math.Sqrt(PointDistanceSquared(PathData.Path[i].MoveTo, lastPoint));
                                else
                                    distance = minDistance;

                                if ((distance < minDistance) && (Math.Abs(lastAngle - PathData.Path[i].Angle) < 0.5))   // && IsEqual(lastZ, PathData.Path[1].Depth))   // < 30°
                                { PathData.Path.RemoveAt(i); count++; }

                                lastPoint = PathData.Path[i].MoveTo;    // if i was removed, i+1 takes place
                                lastAngle = PathData.Path[i].Angle;
                                lastZ = PathData.Path[1].Depth;
                            }
                            countAll = count;
                        }
                        else
                        {
                            indexToRemove.Clear();
                            lastPoint = PathData.Path[0].MoveTo;
                            for (int i = 1; i < (PathData.Path.Count - 2); i++)
                            {
                                if (PathData.Path[i] is GCodeLine)
                                    distance = Math.Sqrt(PointDistanceSquared(PathData.Path[i].MoveTo, lastPoint));
                                else
                                    distance = minDistance;

                                if (distance < minDistance)
                                {
                                    if ((Math.Abs(lastAngle - PathData.Path[i].Angle) < 0.5) && IsEqual(lastZ, PathData.Path[i].Depth) && IsEqual(lastZ, PathData.Path[i + 1].Depth))   // < 30°
                                    { indexToRemove.Add(i); }
                                    else
                                    {
                                        lastPoint = PathData.Path[i].MoveTo;    // if i will be removed, keep old last-value
                                        lastAngle = PathData.Path[i].Angle;
                                        lastZ = PathData.Path[i].Depth;
                                    }
                                }
                                else
                                {
                                    lastPoint = PathData.Path[i].MoveTo;
                                    lastAngle = PathData.Path[i].Angle;
                                    lastZ = PathData.Path[i].Depth;
                                }
                            }
                            indexToRemove.Reverse();
                            foreach (int i in indexToRemove)
                            { PathData.Path.RemoveAt(i); }
                            countAll = indexToRemove.Count;
                        }
                    }
                }
            }
            if (logEnable) Logger.Trace("...RemoveShortMoves backward:{0}  Removed:{1} ------------------------------------", backward, countAll); ;
        }
        #endregion


	}
}