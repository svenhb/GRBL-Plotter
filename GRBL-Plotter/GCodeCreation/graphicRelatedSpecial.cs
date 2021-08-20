/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

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
 * 2021-08-10 new
*/


using System;
using System.Collections.Generic;
using System.Windows;

//#pragma warning disable CA1305

namespace GrblPlotter
{
    public static partial class Graphic
    {
        // process development of a figure.
        internal static void Develop()
        {
            bool feedIsX = Properties.Settings.Default.importGraphicDevelopmentFeedX;   // axis which follows circumference - material feed
            bool feedInvert = Properties.Settings.Default.importGraphicDevelopmentFeedInvert;
            double notchWidth = (double)Properties.Settings.Default.importGraphicDevelopmentNotchWidth;
            double notchDistance = (double)Properties.Settings.Default.importGraphicDevelopmentNotchDistance;   // feed after angle process
            double limit = notchDistance;   // if line length is < limit, accumulate lengths and disribute notches then
            double actualNotchPos = 0;

            double zNotch = (double)Properties.Settings.Default.importGraphicDevelopmentNotchZNotch;
            double zCut = (double)Properties.Settings.Default.importGraphicDevelopmentNotchZCut;

            double toolAngle = (double)Properties.Settings.Default.importGraphicDevelopmentToolAngle;
            bool applyZUp = Properties.Settings.Default.importGraphicDevelopmentNotchLift;

            double toolDistance = Math.Round(2 * Math.Abs(zNotch) * Math.Tan(Math.PI * toolAngle / 360), 1);    // width of notch at given V-angle and notch depth
            double neededDistance;
            double fullDistance;
            int startIndex;

            notchDistance = Math.Min(notchDistance, toolDistance);

            Logger.Trace("...Develop tool-angle:{0:0.0} notch-depth:{1:0.0}  notch-dist:{2:0.00}", toolAngle, zNotch, notchDistance);

            Point pStart, pNow, pNext;
            double distance, distAccumulated = 0, angleDiff, angle1, angle2;

            Point actualXY = new Point(0, 0);
            ItemPath tmpItemPath = new ItemPath();
            List<PathObject> developGraphic = new List<PathObject>();
            PathInformation pathInfo = new PathInformation();

            foreach (PathObject graphicItem in completeGraphic)
            {
                if (!(graphicItem is ItemDot))
                {
                    ItemPath item = (ItemPath)graphicItem;

                    pStart = item.Path[0].MoveTo;
                    pNext = item.Path[item.Path.Count - 1].MoveTo;

                    pathInfo.Id = item.Info.Id;
                    pathInfo.PathId = item.Info.PathId;
                    pathInfo.PenColorId = item.Info.PenColorId;
                    pathInfo.GroupAttributes = item.Info.GroupAttributes;

                    pathInfo.PathGeometry = item.Info.PathGeometry + "_Cut1";
                    AddNotchPath(zCut, true);     // first notch

                    pathInfo.PathGeometry = item.Info.PathGeometry + "_Notch";

                    fullDistance = 0;
                    startIndex = 1;
                    if (item.IsClosed) { startIndex = 0; }         // last point = first point

                    Logger.Info("  Start isClosed:{0} startIndex:{1}  count:{2}", item.IsClosed, startIndex, item.Path.Count);
                    for (int i = startIndex; i < item.Path.Count; i++)      				// go through path objects
                    {
                        if (i == 0) { pStart = item.Path[item.Path.Count - 2].MoveTo; }	// if closedPath take account of 1st angle by using penultimate point
                        else { pStart = item.Path[i - 1].MoveTo; }
                        pNow = item.Path[i].MoveTo;
                        if (i < (item.Path.Count - 1))
                            pNext = item.Path[i + 1].MoveTo;
                        else
                            pNext = item.Path[i - item.Path.Count + 2].MoveTo;

                        /* Process Line */
                        if (item.Path[i] is GCodeLine)
                        {
                            distance = GcodeMath.DistancePointToPoint(pStart, pNow);

                            angle1 = GcodeMath.GetAlpha(pStart, pNow);
                            angle2 = GcodeMath.GetAlpha(pNow, pNext);
                            angleDiff = 180 * (angle2 - angle1) / Math.PI;
                            double angleDiffOrig = angleDiff;
                //            Logger.Info("i:{6} of {7} p1.x:{0:0.00} p1.y:{1:0.00} p2.x:{2:0.00} p2.y:{3:0.00} p3.x:{4:0.00} p3.y:{5:0.00}", pStart.X, pStart.Y, pNow.X, pNow.Y, pNext.X, pNext.Y, i, item.Path.Count);
                //            Logger.Info("angle1:{0:0.00}  angle2:{1:0.00}  diff:{2:0.00}", 180*angle1 / Math.PI, 180 * angle2 / Math.PI, angleDiffOrig);

                            while (angleDiff < -180) { angleDiff += 360; }
                            angleDiff = Math.Abs(angleDiff);
                            while (angleDiff > 360) { angleDiff -= 360; }
                            if (angleDiff > 180) { angleDiff = 360- angleDiff; }
                            if (angleDiff < 0) { angleDiff *= -1; }

                            double notchAngle =  angleDiff;

                            // tool angle a allows a bending angle of 180° - a
                            if (angleDiff > toolAngle)//< 180)
                            {
                                neededDistance = Math.Round(2 * Math.Abs(zNotch) * Math.Tan(Math.PI * notchAngle / 360), 1); // needed width of notch at needed bend angle and notch depth
                            }
                            else
                                neededDistance = toolDistance;

                            if (i == 0) { distance = 0; }
                            fullDistance += distance;

                            if ((distance > limit) || (i == 0))
                            {
                                if (distAccumulated > 0)        // process previous curve
                                {
                                    ProcessCurve();             // add notches, distributed over accumulated length			
                                }
                //                Logger.Info("Add notch-distance:{0:0.000}, angle:{1:0.000}, orig:{2:0.000}, isClosed:{3}", distance, notchAngle, angleDiffOrig, item.IsClosed);

                                if (!item.IsClosed)
                                {
                                    if ((i == 0) || (i == (item.Path.Count - 1)))   // on open path, don't apply multiple notches
                                    { angleDiff = 0; }
                                }

                                AddFeedXY(distance);            // move to next notch
                                                                //        if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); } //2021-08-10

                                if (angleDiff <= toolAngle)
                                {   // notch on edge
                                    if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }    //2021-08-10
                                    AddNotchPath(zNotch, applyZUp);     // notch width is ok
                                }
                                else		// multiple notches will be applied on pos and negative angle - nobody knows if notches are needed inside or outside the shape
                                {
                                    int addNotches = (int)Math.Floor(2 * neededDistance / toolDistance);
                                    double addStep = (neededDistance - toolDistance) / addNotches;

                     //               Logger.Info("toolDistance:{0:0.00} neededDistance:{1:0.00}  cnt:{2}  addStep:{3:0.00}", toolDistance, neededDistance, addNotches, addStep);
                    //                Logger.Info("AddNotches cnt:{0}, step:{1:0.000}, target-angle:{2:0.00}, target-distance:{3:0.00}", addNotches, addStep, angleDiff, neededDistance);

                                    // notch before edge
                                    if (i > 0)								// don't move backwards for start notch
                                    {
                     //                   Logger.Info(" i:{0} multiple notches before:{1:0.00}",i, addStep * addNotches / 2);
                                        AddFeedXY(-addStep * addNotches / 2);
                                        if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }
                                        AddNotchPath(zNotch, applyZUp);
                                        for (int k = 0; k < (addNotches / 2); k++)          // from minus to 0
                                        {
                     //                       Logger.Info(" i:{0} multiple notches after k:{1}", i, k);
                                            AddFeedXY(addStep);
                                            if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }
                                            AddNotchPath(zNotch, applyZUp);
                                        }
                                    }

                                    // notch behind edge
                                    if (i < (item.Path.Count - 1))			// don't move further than distance length on last point
                                    {
                     //                   Logger.Info(" i:{0} multiple notches after cnt:{1}", i, addNotches);
                                        double tmpTooFar = 0;
                                        for (int k = (addNotches / 2); k < addNotches; k++) // from 0 to plus
                                        {
                                            AddFeedXY(addStep);
                                            if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }
                                            AddNotchPath(zNotch, applyZUp);
                                            tmpTooFar += addStep;
                                        }
                                        AddFeedXY(-tmpTooFar);
                                    }
                                }
                            }
                            else
                            { distAccumulated += distance; }   // is curve, accumulate small distances
                        }
                    }   // for
                    if (distAccumulated > 0)        // process previous curve
                    {
                        ProcessCurve();
                    }
                    pathInfo.PathGeometry = item.Info.PathGeometry + "_Cut2";
                    AddNotchPath(zCut, true);

                    SetHeaderInfo(string.Format(" Id:{0} Length:{1:0.00}", item.Info.Id, fullDistance));
                }
                AddFeedXY((double)Properties.Settings.Default.importGraphicDevelopmentFeedAfter);
            }

            // replace original 2D shape by developed shape
            completeGraphic.Clear();
            foreach (PathObject item in developGraphic)     // add tile to full graphic
                completeGraphic.Add(item);
            return;

            void AddNotchPath(double deepth, bool zUp)
            {
                if (zUp)
                {
                    if (!applyZUp && (tmpItemPath.Path.Count > 1))     // finish last path
                    {
                        tmpItemPath.Info.CopyData(pathInfo);    // add info to path
                        tmpItemPath.Add(actualXY, deepth, 0);   // add line to path
                        developGraphic.Add(tmpItemPath);        // add path to graphic
                    }
                    tmpItemPath = new ItemPath(actualXY, deepth);   // start new path
                    AddNotchXY();                           // calc new actualXY
                    tmpItemPath.Info.CopyData(pathInfo);    // add info to path
                    tmpItemPath.Add(actualXY, deepth, 0);   // add line to path
                    developGraphic.Add(tmpItemPath);        // add path to graphic
                    if (!applyZUp)
                    {
                        tmpItemPath = new ItemPath(actualXY, deepth);   // start new path
                        tmpItemPath.Info.CopyData(pathInfo);    // add info to path
                    }
                }
                else
                {
                    AddNotchXY();                           // calc new actualXY
                    tmpItemPath.Add(actualXY, deepth, 0);   // add line to path
                    tmpItemPath.Path[0].Depth = deepth;
                }
            }

            void AddNotchXY()
            {
                if (actualNotchPos > 0) { actualNotchPos = 0; }
                else { actualNotchPos = notchWidth; }

                if (feedIsX)
                { actualXY.Y = actualNotchPos; }
                else
                { actualXY.X = actualNotchPos; }
            }

            void AddFeedXY(double s)
            {
                if (feedInvert) { s = -s; }
                if (feedIsX)
                { actualXY.X += s; }
                else
                { actualXY.Y += s; }
                //		Logger.Info("AddFeedXY X:{0:0.000} Y:{1:0.000}", actualXY.X, actualXY.Y);
            }

            void ProcessCurve()
            {
                int cntNeededNotches = (int)Math.Round(distAccumulated / notchDistance);
                double notchFeedUse = distAccumulated / cntNeededNotches;

                for (int cnt = 0; cnt < cntNeededNotches; cnt++)
                {
                    AddFeedXY(notchFeedUse);
                    if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }
                    AddNotchPath(zNotch, applyZUp);
                }
                distAccumulated = 0;
            }
        }
    }
}
