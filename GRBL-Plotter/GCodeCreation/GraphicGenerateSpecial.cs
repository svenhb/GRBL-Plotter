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
*/


using System;
using System.Collections.Generic;
using System.Windows;

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
                            if (item.IsClosed) { pLabel = new Point(pLabel.X + emSize/4, pLabel.Y - emSize / 4); }
                        }

                        /* Process Line */
                        if (item.Path[i] is GCodeLine)
                        {
                            distance = GcodeMath.DistancePointToPoint(pStart, pNow);

                            angle1 = GcodeMath.GetAlpha(pStart, pNow);
                            angle2 = GcodeMath.GetAlpha(pNow, pNext);
                            angleDiff = 180 * (angle2 - angle1) / Math.PI;
                            double angleDiffOrig = angleDiff;

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
                                    backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance/2), actualXY.X + (toolDistance / 2)));
                                }
                                else		// multiple notches will be applied on pos and negative angle - nobody knows if notches are needed inside or outside the shape
                                {
                                    int addNotches = (int)Math.Floor(neededDistance / toolDistance);	// amount of notches symetric arround center notch
                                    double addStep = (neededDistance - toolDistance) / addNotches;			// needed step-width from notch to notch on front and behind center notch
									
									double addStepHalf = addStep/2;

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
											if (k == (addNotches-1))
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

            void AddNotchPath(double deepth, bool zUp, bool addNumber=false)
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
                { 	actualXY.Y = actualNotchPos; 
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
					double addedFeeds=0;
                    for (int cnt = 0; cnt < cntNeededNotches; cnt++)
                    {
                        AddFeedXY(notchFeedUse);
						addedFeeds +=notchFeedUse;
                        if (!applyZUp) 
						{ tmpItemPath.Add(actualXY, zNotch, 0); }
                        if (cnt == (cntNeededNotches - 1))
                        {
                            AddNotchPath(zNotch, applyZUp, true);       // add number at last notch
                            backgroundNotchStartEnd.Add(new Point(actualXY.X - (toolDistance / 2), actualXY.X + (toolDistance / 2)));
                        }
                        else
                        {   AddNotchPath(zNotch, applyZUp); 
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
                    { VisuGCode.pathBackground.AddLine((float)lastEnd, (float)posYTop, (float)tmp.X, (float)posYTop);  }
                    lastEnd = tmp.Y;
                }
            }
            void AddBackgroundNotchX()
            {
                if (!feedIsX || feedInvert)
                    return;
                double thickness = Math.Max(Math.Abs(zCut), Math.Abs(zNotch));
                double posY = -1.6* emSize - thickness;// notchWidth + 2;
                double posYTop = posY + thickness;
                double dX = toolDistance / 2;

                float x1 = (float)(actualXY.X - dX);
                float y1 = (float)posYTop;
                if ((backgroundNotchProperties & (uint)2) > 0)	// shorten left edge
                { 	x1 = (float)(actualXY.X - dX/2);
					y1 = (float)(posYTop - Math.Abs(zNotch)/2);
				}
								
                float x2 = (float)(actualXY.X + dX);
                float y2 = (float)posYTop;
                if ((backgroundNotchProperties & (uint)4) > 0)	// shorten right edge
				{	x2 = (float)(actualXY.X + dX/2);
					y2 = (float)(posYTop - Math.Abs(zNotch)/2);	
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
            {	System.Drawing.StringFormat sFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault)
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
			catch (Exception err) {Logger.Error(err,"AddBackgroundText ");}
        }
    }
}
