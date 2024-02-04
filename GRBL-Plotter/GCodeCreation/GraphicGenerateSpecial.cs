/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
 * 2021-09-03 add "no curve" option
 * 2021-09-07 set limit=0 if noCurve
 * 2021-09-15 add material side view as background - only in +X direction
 * 2021-12-10 line 447 try/catch
 * 2021-12-10 line 443 add logger
 * 2022-04-23 add wire bender
*/


using System;
using System.Collections.Generic;
using System.Windows;

namespace GrblPlotter
{
    public static partial class Graphic
    {
        // wire bender
        internal static void WireBender()
        {
            double rBend = (double)Properties.Settings.Default.importGraphicWireBenderRadius;

            if (logModification) Logger.Trace("...Wire bender, r:{0}", rBend);

            Point pStart, pNow, pNext, pLabel;
            double fullDistance = 0, angleDiff, angleDiffOld = 0, angle1, angle2;
            //    double angleDiffOrig;
            double distanceOrig, distanceCorrected;
            double angleApply;
            bool pegActivated = false;
            int startIndex;

            Point actualXY = new Point(0, 0);
            ItemPath tmpItemPath = new ItemPath(actualXY, 0);
            List<PathObject> developGraphic = new List<PathObject>();
            PathInformation pathInfo = new PathInformation();
            foreach (PathObject graphicItem in completeGraphic)
            {
                if (!(graphicItem is ItemDot))
                {
                    ItemPath item = (ItemPath)graphicItem;
                    pNext = item.Path[item.Path.Count - 1].MoveTo;

                    pathInfo.Id = item.Info.Id;
                    pathInfo.PathId = item.Info.PathId;
                    pathInfo.PenColorId = item.Info.PenColorId;
                    pathInfo.GroupAttributes = item.Info.GroupAttributes;
                    pathInfo.PathGeometry = item.Info.PathGeometry + "_WireBender";

                    tmpItemPath.Info.CopyData(pathInfo);    // add info to path

                    startIndex = 1;

                    if (logModification) Logger.Trace("...Start isClosed:{0} startIndex:{1}  count:{2}", item.IsClosed, startIndex, item.Path.Count);
                    for (int i = startIndex; i < item.Path.Count; i++)      				// go through path objects
                    {
                        pLabel = pNow = item.Path[i].MoveTo;

                        pStart = item.Path[i - 1].MoveTo;

                        if (i < (item.Path.Count - 1))
                            pNext = item.Path[i + 1].MoveTo;
                        else
                        {
                            pNext = item.Path[i - item.Path.Count + 2].MoveTo;
                        }

                        /* Process Line */
                        if (item.Path[i] is GCodeLine)
                        {
                            distanceOrig = GcodeMath.DistancePointToPoint(pStart, pNow);

                            angle1 = GcodeMath.GetAlpha(pStart, pNow);
                            angle2 = GcodeMath.GetAlpha(pNow, pNext);
                            angleDiff = angle2 - angle1;

                            while (angleDiff < -Math.PI) { angleDiff += 2 * Math.PI; }
                            while (angleDiff > 2 * Math.PI) { angleDiff -= 2 * Math.PI; }

                            distanceCorrected = distanceOrig - (2 * rBend) + ((Math.Abs(angleDiff) + Math.Abs(angleDiffOld)) * rBend / 2);
                            fullDistance += distanceCorrected;

                            Logger.Trace("Correct length: orig:{0:0.00}  new:{1:0.00}  a1:{2:0.00}  a2:{3:0.00}  diff:{4:0.00}", distanceOrig, distanceCorrected, (angle1 * 180 / Math.PI), (angle2 * 180 / Math.PI), (angleDiff * 180 / Math.PI));

                            angleApply = Math.Sign(angleDiff) * (Math.Abs(angleDiff) + (double)Properties.Settings.Default.importGraphicWireBenderAngleAddOn * Math.PI / 180);
                            if (!Properties.Settings.Default.importGraphicWireBenderAngleAbsolute)
                                angleApply *= (1 + (double)Properties.Settings.Default.importGraphicWireBenderAngleAddOn);

                            if (distanceCorrected < 0)
                            { FeedAndBend(distanceOrig, angleApply); }
                            else
                            {
                                FeedDistance(distanceCorrected);
                                BendAngle(angleApply);
                            }

                            angleDiffOld = angleDiff;
                        }
                    }   // for
                    developGraphic.Add(tmpItemPath);            // add path to graphic
                    tmpItemPath = new ItemPath(actualXY, 0);    // start new path

                    //            pathInfo.PathGeometry = item.Info.PathGeometry + "_Cut2";
                    //            SetHeaderInfo(string.Format(" Id:{0} Length:{1:0.00}", item.Info.Id, fullDistance));
                }
            }

            // replace original 2D shape by developed shape
            completeGraphic.Clear();
            foreach (PathObject item in developGraphic)     // add tile to full graphic
                completeGraphic.Add(item);
            return;


            void FeedAndBend(double s, double a)
            {
                actualXY.X += s;

                if (!pegActivated)
                {
                    tmpItemPath.Add(actualXY, 0, 0);    // set feed
                    actualXY.Y = a * 180 / Math.PI;
                    tmpItemPath.Add(actualXY, 1, 0);    // set peg and bend
                    pegActivated = true;
                }
                else
                    tmpItemPath.Add(actualXY, 1, 0);   // keep peg, keep angle, set feed
            }
            void FeedDistance(double s)
            {
                if (pegActivated)
                {
                    actualXY.Y = 0;
                    tmpItemPath.Add(actualXY, 0, 0);   // remove peg and turn back
                    pegActivated = false;
                }
                actualXY.X += s;
                actualXY.Y = 0;
                tmpItemPath.Add(actualXY, 0, 0);        // add line to path     xy, deepth, angle
            }

            void BendAngle(double a)
            {
                actualXY.Y = a * 180 / Math.PI;
                tmpItemPath.Add(actualXY, 1, 0);   // set peg and bend
                pegActivated = true;
                actualXY.Y = 0;
                tmpItemPath.Add(actualXY, 0, 0);   // remove peg and turn back
                pegActivated = false;
            }
        }


        // process development of a figure.
        internal static void Develop()
        {
            bool feedIsX = Properties.Settings.Default.importGraphicDevelopmentFeedX;   // axis which follows circumference - material feed
            bool feedInvert = Properties.Settings.Default.importGraphicDevelopmentFeedInvert;

            double notchWidth = (double)Properties.Settings.Default.importGraphicDevelopmentNotchWidth;
            double notchDistance = (double)Properties.Settings.Default.importGraphicDevelopmentNotchDistance;   // feed after angle process
            double limit = notchDistance;   // if line length is < limit, accumulate lengths and disribute notches then
            double actualNotchPos = 0;

            bool noCurve = Properties.Settings.Default.importGraphicDevelopmentNoCurve;

            double zNotch = (double)Properties.Settings.Default.importGraphicDevelopmentNotchZNotch;
            double zCut = (double)Properties.Settings.Default.importGraphicDevelopmentNotchZCut;

            double toolAngle = (double)Properties.Settings.Default.importGraphicDevelopmentToolAngle;
            bool applyZUp = Properties.Settings.Default.importGraphicDevelopmentNotchLift;

            double toolDistance = Math.Round(2 * Math.Abs(zNotch) * Math.Tan(Math.PI * toolAngle / 360), 1);    // width of notch at given V-angle and notch depth
            double neededDistance;
            double fullDistance;
            int startIndex;
            int edgeCount = 1;
            float emSize = 50;
            Dimensions tmpDim;
            notchDistance = Math.Min(notchDistance, toolDistance);
            List<Point> backgroundNotchStartEnd = new List<Point>();

            uint backgroundNotchProperties;

            if (logModification) Logger.Trace("...Develop tool-angle:{0:0.0} notch-depth:{1:0.0}  notch-dist:{2:0.00}", toolAngle, zNotch, notchDistance);

            Point pStart, pNow, pNext, pLabel, pLabelLast;
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
                    tmpDim = item.Dimension;
                    emSize = (float)tmpDim.dimy / 5;
                    if (noCurve) limit = tmpDim.dimy / 10;

                    pLabelLast = pStart = item.Path[0].MoveTo;
                    pNext = item.Path[item.Path.Count - 1].MoveTo;

                    pathInfo.Id = item.Info.Id;
                    pathInfo.PathId = item.Info.PathId;
                    pathInfo.PenColorId = item.Info.PenColorId;
                    pathInfo.GroupAttributes = item.Info.GroupAttributes;

                    pathInfo.PathGeometry = item.Info.PathGeometry + "_Cut1";

                    backgroundNotchProperties = 1;
                    AddNotchPath(zCut, true);     // first notch
                    backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance / 2), actualXY.X + (toolDistance / 2)));

                    pathInfo.PathGeometry = item.Info.PathGeometry + "_Notch";

                    fullDistance = 0;
                    startIndex = 1;
                    if (item.IsClosed) { startIndex = 0; }         // last point = first point

                    if (logModification) Logger.Trace("...Start isClosed:{0} startIndex:{1}  count:{2}", item.IsClosed, startIndex, item.Path.Count);
                    for (int i = startIndex; i < item.Path.Count; i++)      				// go through path objects
                    {
                        pLabel = pNow = item.Path[i].MoveTo;
                        if (i == 0)
                        {
                            pStart = item.Path[item.Path.Count - 2].MoveTo;     // if closedPath take account of 1st angle by using penultimate point
                            pLabel = new Point(pLabel.X - emSize / 4, pLabel.Y + emSize / 4);
                            pLabelLast = pLabel;
                        }
                        else { pStart = item.Path[i - 1].MoveTo; }
                        if (i < (item.Path.Count - 1))
                            pNext = item.Path[i + 1].MoveTo;
                        else
                        {
                            pNext = item.Path[i - item.Path.Count + 2].MoveTo;
                            if (item.IsClosed) { pLabel = new Point(pLabel.X + emSize / 4, pLabel.Y - emSize / 4); }
                        }

                        /* Process Line */
                        if (item.Path[i] is GCodeLine)
                        {
                            distance = GcodeMath.DistancePointToPoint(pStart, pNow);

                            angle1 = GcodeMath.GetAlpha(pStart, pNow);
                            angle2 = GcodeMath.GetAlpha(pNow, pNext);
                            angleDiff = 180 * (angle2 - angle1) / Math.PI;

                            while (angleDiff < -180) { angleDiff += 360; }
                            angleDiff = Math.Abs(angleDiff);
                            while (angleDiff > 360) { angleDiff -= 360; }
                            if (angleDiff > 180) { angleDiff = 360 - angleDiff; }
                            if (angleDiff < 0) { angleDiff *= -1; }

                            double notchAngle = angleDiff;

                            // tool angle a allows a bending angle of 180° - a
                            if (angleDiff > toolAngle)//< 180)
                            {
                                neededDistance = Math.Round(2 * Math.Abs(zNotch) * Math.Tan(Math.PI * notchAngle / 360), 1); // needed width of notch at needed bend angle and notch depth
                            }
                            else
                                neededDistance = toolDistance;
                            //				Logger.Trace("....i:{0} aNotch:{1:0.0}  aTool:{2:0.0}  neededDist:{3:0.000}  toolDist:{4:0.000}",i,angleDiff, toolAngle, neededDistance, toolDistance);

                            if (i == 0) { distance = 0; }
                            fullDistance += distance;

                            if ((distance > limit) || (i == 0))
                            {
                                if (distAccumulated > 0)        // process previous curve
                                {
                                    AddBackgroundTextFigure(pLabelLast, tmpDim, emSize / 2, edgeCount.ToString());
                                    ProcessCurve();             // add notches, distributed over accumulated length			
                                }

                                if (!item.IsClosed)
                                {
                                    if ((i == 0) || (i == (item.Path.Count - 1)))   // on open path, don't apply multiple notches
                                    { angleDiff = 0; }
                                }

                                AddFeedXY(distance);            // move to next notch

                                backgroundNotchProperties = 0;
                                if (angleDiff <= toolAngle)		// bend/kink-angle is < 180- tool angle 
                                {   // notch on edge
                                    if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }    // apply notch position
                                    AddBackgroundTextFigure(pLabel, item.Dimension, emSize / 2, edgeCount.ToString());
                                    AddNotchPath(zNotch, applyZUp, true);     					// make a single (center) notch 
                                    backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance / 2), actualXY.X + (toolDistance / 2)));
                                }
                                else		// multiple notches will be applied on pos and negative angle - nobody knows if notches are needed inside or outside the shape
                                {
                                    int addNotches = (int)Math.Floor(neededDistance / toolDistance);	// amount of notches symetric arround center notch
                                    double addStep = (neededDistance - toolDistance) / addNotches;          // needed step-width from notch to notch on front and behind center notch

                                    double addStepHalf = addStep / 2;

                                    backgroundNotchStartEnd.Add(new Point(actualXY.X - (neededDistance / 2), actualXY.X + (neededDistance / 2)));

                                    // notch in front of edge
                                    if (i > 0)									// don't move backwards for start notch
                                    {
                                        AddFeedXY(-addStepHalf * addNotches);	// move back to 1st front notch
                                        if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }

                                        backgroundNotchProperties = 4; // shorten right edge
                                        for (int k = 0; k < addNotches; k++)          // from minus to 0
                                        {
                                            AddNotchPath(zNotch, applyZUp);
                                            AddFeedXY(addStepHalf);
                                            if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }
                                            backgroundNotchProperties = 6; // shorten left & right edge
                                        }
                                    }

                                    // center notch
                                    AddBackgroundTextFigure(pLabel, item.Dimension, emSize / 2, edgeCount.ToString());
                                    AddNotchPath(zNotch, applyZUp, true);
                                    AddBackgroundNeededNotchX(neededDistance);

                                    // notch behind edge
                                    if (i < (item.Path.Count - 1))			// don't move further than distance length on last point
                                    {
                                        double tmpTooFar = 0;

                                        backgroundNotchProperties = 6; // shorten left & right edge
                                        for (int k = 0; k < addNotches; k++) // from 0 to plus
                                        {
                                            if (k == (addNotches - 1))
                                                backgroundNotchProperties = 2; // shorten left edge

                                            AddFeedXY(addStepHalf);
                                            if (!applyZUp) { tmpItemPath.Add(actualXY, zNotch, 0); }
                                            AddNotchPath(zNotch, applyZUp);
                                            tmpTooFar += addStepHalf;
                                        }
                                        AddFeedXY(-tmpTooFar);
                                    }
                                }
                            }
                            else
                            {
                                distAccumulated += distance;    // is curve, accumulate small distances
                                pLabelLast = pLabel;
                            }
                        }
                    }   // for
                    if (distAccumulated > 0)        // process previous curve
                    {
                        AddBackgroundTextFigure(pLabelLast, tmpDim, emSize / 2, edgeCount.ToString());
                        ProcessCurve();
                    }
                    pathInfo.PathGeometry = item.Info.PathGeometry + "_Cut2";
                    backgroundNotchProperties = 1;
                    AddNotchPath(zCut, true);
                    backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance / 2), actualXY.X + (toolDistance / 2)));

                    SetHeaderInfo(string.Format(" Id:{0} Length:{1:0.00}", item.Info.Id, fullDistance));
                }
                AddFeedXY((double)Properties.Settings.Default.importGraphicDevelopmentFeedAfter);
            }

            AddBackgroundSurfaces();
            // replace original 2D shape by developed shape
            completeGraphic.Clear();
            foreach (PathObject item in developGraphic)     // add tile to full graphic
                completeGraphic.Add(item);
            return;

            void AddNotchPath(double deepth, bool zUp, bool addNumber = false)
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
                if (addNumber)
                {
                    AddBackgroundTextDevelop(actualXY, feedIsX, emSize, edgeCount.ToString());
                    edgeCount++;
                }
            }

            void AddNotchXY()
            {
                if (actualNotchPos > 0) { actualNotchPos = 0; }
                else { actualNotchPos = notchWidth; }

                if (feedIsX)
                {
                    actualXY.Y = actualNotchPos;
                    AddBackgroundNotchX();
                }
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
            }

            void ProcessCurve()
            {
                if (noCurve)
                {
                    AddFeedXY(distAccumulated);
                    if (!applyZUp)
                    { tmpItemPath.Add(actualXY, zNotch, 0); }
                    AddNotchPath(zNotch, applyZUp, true);               // add number
                    backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance / 2), actualXY.X + (toolDistance / 2)));
                }
                else
                {
                    int cntNeededNotches = (int)Math.Round(distAccumulated / notchDistance);
                    double notchFeedUse = distAccumulated / cntNeededNotches;
                    double addedFeeds = 0;
                    for (int cnt = 0; cnt < cntNeededNotches; cnt++)
                    {
                        AddFeedXY(notchFeedUse);
                        addedFeeds += notchFeedUse;
                        if (!applyZUp)
                        { tmpItemPath.Add(actualXY, zNotch, 0); }
                        if (cnt == (cntNeededNotches - 1))
                        {
                            AddNotchPath(zNotch, applyZUp, true);       // add number at last notch
                            backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance / 2), actualXY.X + (toolDistance / 2)));
                        }
                        else
                        {
                            AddNotchPath(zNotch, applyZUp);
                            backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance / 2), actualXY.X + (toolDistance / 2)));
                        }
                    }
                    //			Logger.Trace("ProcessCurve  needed length:{0:0.000}  summed steps:{1:0.000}",distAccumulated, addedFeeds);
                }
                distAccumulated = 0;
            }

            void AddBackgroundSurfaces()
            {
                if (!feedIsX || feedInvert)
                    return;
                double thickness = Math.Max(Math.Abs(zCut), Math.Abs(zNotch));
                double posY = -1.6 * emSize - thickness;// notchWidth + 2;
                double posYTop = posY + thickness;
                VisuGCode.pathBackground.StartFigure();
                VisuGCode.pathBackground.AddLine(0, (float)posY, (float)actualXY.X, (float)posY);
                VisuGCode.pathBackground.StartFigure();

                double lastEnd = toolDistance / 2;
                foreach (Point tmp in backgroundNotchStartEnd)
                {
                    VisuGCode.pathBackground.StartFigure();
                    if ((tmp.X > 0) && (lastEnd < tmp.X) && (lastEnd < actualXY.X))
                    { VisuGCode.pathBackground.AddLine((float)lastEnd, (float)posYTop, (float)tmp.X, (float)posYTop); }
                    lastEnd = tmp.Y;
                }
            }
            void AddBackgroundNotchX()
            {
                if (!feedIsX || feedInvert)
                    return;
                double thickness = Math.Max(Math.Abs(zCut), Math.Abs(zNotch));
                double posY = -1.6 * emSize - thickness;// notchWidth + 2;
                double posYTop = posY + thickness;
                double dX = toolDistance / 2;

                float x1 = (float)(actualXY.X - dX);
                float y1 = (float)posYTop;
                if ((backgroundNotchProperties & (uint)2) > 0)	// shorten left edge
                {
                    x1 = (float)(actualXY.X - dX / 2);
                    y1 = (float)(posYTop - Math.Abs(zNotch) / 2);
                }

                float x2 = (float)(actualXY.X + dX);
                float y2 = (float)posYTop;
                if ((backgroundNotchProperties & (uint)4) > 0)  // shorten right edge
                {
                    x2 = (float)(actualXY.X + dX / 2);
                    y2 = (float)(posYTop - Math.Abs(zNotch) / 2);
                }

                float y = (float)(posYTop - Math.Abs(zNotch));
                if ((backgroundNotchProperties & (uint)1) > 0)	// go full depth
                    y = (float)posY;

                VisuGCode.pathBackground.StartFigure();
                VisuGCode.pathBackground.AddLine(x1, y1, (float)actualXY.X, y);
                VisuGCode.pathBackground.AddLine((float)actualXY.X, y, x2, y2);
            }
            void AddBackgroundNeededNotchX(double width)
            {
                if (!feedIsX || feedInvert)
                    return;
                double thickness = Math.Max(Math.Abs(zCut), Math.Abs(zNotch));
                double posY = -1.6 * emSize - thickness;// notchWidth + 2;
                double posYTop = posY + thickness;
                double dX = width / 2;

                double x1 = actualXY.X - dX;
                double x2 = actualXY.X + dX;
                double y = posYTop - Math.Abs(zNotch);
                VisuGCode.pathBackground.StartFigure();
                VisuGCode.pathBackground.AddLine((float)x1, (float)posYTop + 5, (float)x1, (float)posYTop + 1);
                VisuGCode.pathBackground.StartFigure();
                VisuGCode.pathBackground.AddLine((float)x2, (float)posYTop + 1, (float)x2, (float)posYTop + 5);
            }
        }

        private static void AddBackgroundTextFigure(Point pos, Dimensions dim, float emSize, string txt)
        {
            XyPoint pCenter = dim.GetCenter();
            double r = pCenter.DistanceTo((XyPoint)pos) - emSize;
            double a = Math.PI * pCenter.AngleTo((XyPoint)pos) / 180;
            Point newP = new Point
            {
                X = r * Math.Cos(a) - r * Math.Sin(a),
                Y = r * Math.Sin(a) + r * Math.Cos(a)
            };
            newP.X += pCenter.X;
            newP.Y += pCenter.Y;
            AddBackgroundText(pos, emSize, txt);
        }

        private static void AddBackgroundTextDevelop(Point pos, bool feedAxisX, float emSize, string txt)
        {
            if (feedAxisX) pos.Y = -1f * emSize;
            else pos.X = -1f * emSize;
            AddBackgroundText(pos, emSize, txt);
        }
        private static void AddBackgroundText(Point pos, float emSize, string txt)
        {
            Logger.Info("AddBackgroundText x:{0} y:{1}  emSize:{2}  text:{3}", pos.X, pos.Y, emSize, txt);
            System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
            matrix.Scale(1, -1);
            VisuGCode.pathBackground.StartFigure();
            VisuGCode.pathBackground.Transform(matrix);
            float centerX = (float)pos.X;// (float)((clipMax.X + clipMin.X) / 2);
            float centerY = (float)pos.Y;// (float)((clipMax.Y + clipMin.Y) / 2);

            try
            {
                System.Drawing.StringFormat sFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault)
                {
                    Alignment = System.Drawing.StringAlignment.Center,
                    LineAlignment = System.Drawing.StringAlignment.Center
                };
                System.Drawing.FontFamily myFont = new System.Drawing.FontFamily("Arial");
                VisuGCode.pathBackground.AddString(txt, myFont, (int)System.Drawing.FontStyle.Regular, emSize, new System.Drawing.PointF(centerX, -centerY), sFormat);
                VisuGCode.pathBackground.Transform(matrix);
                myFont.Dispose();
                sFormat.Dispose();
            }
            catch (Exception err) { Logger.Error(err, "AddBackgroundText "); }
        }
    }


    //https://github.com/WardBenjamin/SimplexNoise/blob/master/SimplexNoise/Noise.cs
    public static class Noise
    {
        /// <summary>
        /// Creates 1D Simplex noise
        /// </summary>
        /// <param name="width">The number of points to generate</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>An array containing 1D Simplex noise</returns>
        public static float[] Calc1D(int width, float scale)
        {
            var values = new float[width];
            for (var i = 0; i < width; i++)
                values[i] = Generate(i * scale) * 128 + 128;
            return values;
        }

        /// <summary>
        /// Creates 2D Simplex noise
        /// </summary>
        /// <param name="width">The number of points to generate in the 1st dimension</param>
        /// <param name="height">The number of points to generate in the 2nd dimension</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>An array containing 2D Simplex noise</returns>
        public static float[,] Calc2D(int width, int height, float scale)
        {
            var values = new float[width, height];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    values[i, j] = Generate(i * scale, j * scale) * 128 + 128;
            return values;
        }

        /// <summary>
        /// Creates 3D Simplex noise
        /// </summary>
        /// <param name="width">The number of points to generate in the 1st dimension</param>
        /// <param name="height">The number of points to generate in the 2nd dimension</param>
        /// <param name="length">The number of points to generate in the 3nd dimension</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>An array containing 3D Simplex noise</returns>
        public static float[,,] Calc3D(int width, int height, int length, float scale)
        {
            var values = new float[width, height, length];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    for (var k = 0; k < length; k++)
                        values[i, j, k] = Generate(i * scale, j * scale, k * scale) * 128 + 128;
            return values;
        }

        /// <summary>
        /// Gets the value of an index of 1D simplex noise
        /// </summary>
        /// <param name="x">Index</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>The value of an index of 1D simplex noise</returns>
        public static float CalcPixel1D(int x, float scale)
        {
            return Generate(x * scale);// * 128 + 128;
        }

        /// <summary>
        /// Gets the value of an index of 2D simplex noise
        /// </summary>
        /// <param name="x">1st dimension index</param>
        /// <param name="y">2st dimension index</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>The value of an index of 2D simplex noise</returns>
        public static float CalcPixel2D(int x, int y, float scale)
        {
            return Generate(x * scale, y * scale);// * 128 + 128;
        }


        /// <summary>
        /// Gets the value of an index of 3D simplex noise
        /// </summary>
        /// <param name="x">1st dimension index</param>
        /// <param name="y">2nd dimension index</param>
        /// <param name="z">3rd dimension index</param>
        /// <param name="scale">The scale of the noise. The greater the scale, the denser the noise gets</param>
        /// <returns>The value of an index of 3D simplex noise</returns>
        public static float CalcPixel3D(int x, int y, int z, float scale)
        {
            return Generate(x * scale, y * scale, z * scale) * 128 + 128;
        }

        static Noise()
        {
            _perm = new byte[PermOriginal.Length];
            PermOriginal.CopyTo(_perm, 0);
        }

        /// <summary>
        /// Arbitrary integer seed used to generate lookup table used internally
        /// </summary>
        public static int Seed
        {
            get => _seed;
            set
            {
                if (value == 0)
                {
                    _perm = new byte[PermOriginal.Length];
                    PermOriginal.CopyTo(_perm, 0);
                }
                else
                {
                    _perm = new byte[512];
                    var random = new Random(value);
                    random.NextBytes(_perm);
                }

                _seed = value;
            }
        }

        private static int _seed;

        /// <summary>
        /// 1D simplex noise
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static float Generate(float x)
        {
            var i0 = FastFloor(x);
            var i1 = i0 + 1;
            var x0 = x - i0;
            var x1 = x0 - 1.0f;

            var t0 = 1.0f - x0 * x0;
            t0 *= t0;
            var n0 = t0 * t0 * Grad(_perm[i0 & 0xff], x0);

            var t1 = 1.0f - x1 * x1;
            t1 *= t1;
            var n1 = t1 * t1 * Grad(_perm[i1 & 0xff], x1);
            // The maximum value of this noise is 8*(3/4)^4 = 2.53125
            // A factor of 0.395 scales to fit exactly within [-1,1]
            return 0.395f * (n0 + n1);
        }

        /// <summary>
        /// 2D simplex noise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static float Generate(float x, float y)
        {
            const float F2 = 0.366025403f; // F2 = 0.5*(sqrt(3.0)-1.0)
            const float G2 = 0.211324865f; // G2 = (3.0-Math.sqrt(3.0))/6.0

            float n0, n1, n2; // Noise contributions from the three corners

            // Skew the input space to determine which simplex cell we're in
            var s = (x + y) * F2; // Hairy factor for 2D
            var xs = x + s;
            var ys = y + s;
            var i = FastFloor(xs);
            var j = FastFloor(ys);

            var t = (i + j) * G2;
            var X0 = i - t; // Unskew the cell origin back to (x,y) space
            var Y0 = j - t;
            var x0 = x - X0; // The x,y distances from the cell origin
            var y0 = y - Y0;

            // For the 2D case, the simplex shape is an equilateral triangle.
            // Determine which simplex we are in.
            int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
            if (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            else { i1 = 0; j1 = 1; }      // upper triangle, YX order: (0,0)->(0,1)->(1,1)

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6

            var x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            var y1 = y0 - j1 + G2;
            var x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
            var y2 = y0 - 1.0f + 2.0f * G2;

            // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
            var ii = Mod(i, 256);
            var jj = Mod(j, 256);

            // Calculate the contribution from the three corners
            var t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 < 0.0f) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Grad(_perm[ii + _perm[jj]], x0, y0);
            }

            var t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 < 0.0f) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Grad(_perm[ii + i1 + _perm[jj + j1]], x1, y1);
            }

            var t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 < 0.0f) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Grad(_perm[ii + 1 + _perm[jj + 1]], x2, y2);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 40.0f * (n0 + n1 + n2); // TODO: The scale factor is preliminary!
        }


        private static float Generate(float x, float y, float z)
        {
            // Simple skewing factors for the 3D case
            const float F3 = 0.333333333f;
            const float G3 = 0.166666667f;

            float n0, n1, n2, n3; // Noise contributions from the four corners

            // Skew the input space to determine which simplex cell we're in
            var s = (x + y + z) * F3; // Very nice and simple skew factor for 3D
            var xs = x + s;
            var ys = y + s;
            var zs = z + s;
            var i = FastFloor(xs);
            var j = FastFloor(ys);
            var k = FastFloor(zs);

            var t = (i + j + k) * G3;
            var X0 = i - t; // Unskew the cell origin back to (x,y,z) space
            var Y0 = j - t;
            var Z0 = k - t;
            var x0 = x - X0; // The x,y,z distances from the cell origin
            var y0 = y - Y0;
            var z0 = z - Z0;

            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords

            /* This code would benefit from a backport from the GLSL version! */
            if (x0 >= y0)
            {
                if (y0 >= z0)
                { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // X Y Z order
                else if (x0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; } // X Z Y order
                else { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; } // Z X Y order
            }
            else
            { // x0<y0
                if (y0 < z0) { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; } // Z Y X order
                else if (x0 < z0) { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; } // Y Z X order
                else { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // Y X Z order
            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.

            var x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            var y1 = y0 - j1 + G3;
            var z1 = z0 - k1 + G3;
            var x2 = x0 - i2 + 2.0f * G3; // Offsets for third corner in (x,y,z) coords
            var y2 = y0 - j2 + 2.0f * G3;
            var z2 = z0 - k2 + 2.0f * G3;
            var x3 = x0 - 1.0f + 3.0f * G3; // Offsets for last corner in (x,y,z) coords
            var y3 = y0 - 1.0f + 3.0f * G3;
            var z3 = z0 - 1.0f + 3.0f * G3;

            // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
            var ii = Mod(i, 256);
            var jj = Mod(j, 256);
            var kk = Mod(k, 256);

            // Calculate the contribution from the four corners
            var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 < 0.0f) n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Grad(_perm[ii + _perm[jj + _perm[kk]]], x0, y0, z0);
            }

            var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0.0f) n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Grad(_perm[ii + i1 + _perm[jj + j1 + _perm[kk + k1]]], x1, y1, z1);
            }

            var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0.0f) n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Grad(_perm[ii + i2 + _perm[jj + j2 + _perm[kk + k2]]], x2, y2, z2);
            }

            var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0.0f) n3 = 0.0f;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * Grad(_perm[ii + 1 + _perm[jj + 1 + _perm[kk + 1]]], x3, y3, z3);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0f * (n0 + n1 + n2 + n3); // TODO: The scale factor is preliminary!
        }

        private static byte[] _perm;

        private static readonly byte[] PermOriginal = {
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        private static int FastFloor(float x)
        {
            return (x > 0) ? ((int)x) : (((int)x) - 1);
        }

        private static int Mod(int x, int m)
        {
            var a = x % m;
            return a < 0 ? a + m : a;
        }

        private static float Grad(int hash, float x)
        {
            var h = hash & 15;
            var grad = 1.0f + (h & 7);   // Gradient value 1.0, 2.0, ..., 8.0
            if ((h & 8) != 0) grad = -grad;         // Set a random sign for the gradient
            return (grad * x);           // Multiply the gradient with the distance
        }

        private static float Grad(int hash, float x, float y)
        {
            var h = hash & 7;      // Convert low 3 bits of hash code
            var u = h < 4 ? x : y;  // into 8 simple gradient directions,
            var v = h < 4 ? y : x;  // and compute the dot product with (x,y).
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0f * v : 2.0f * v);
        }

        private static float Grad(int hash, float x, float y, float z)
        {
            var h = hash & 15;     // Convert low 4 bits of hash code into 12 simple
            var u = h < 8 ? x : y; // gradient directions, and compute dot product.
            var v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
        }

        private static float Grad(int hash, float x, float y, float z, float t)
        {
            var h = hash & 31;      // Convert low 5 bits of hash code into 32 simple
            var u = h < 24 ? x : y; // gradient directions, and compute dot product.
            var v = h < 16 ? y : z;
            var w = h < 8 ? z : t;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v) + ((h & 4) != 0 ? -w : w);
        }
    }
}
