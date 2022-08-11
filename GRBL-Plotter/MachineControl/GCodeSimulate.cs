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
/*  GCodeAnalyze.cs
	Parse/analyze GCode from editor for 2D-view and extract further data
*/
/* 
 * 2021-07-09 split code from GCodeTransform
 * 2021-09-04 new struct to store simulation data: SimuCoordByLine in simuList
 * 2021-09-07 take care of tile-offset in ProcessedPathDraw
 * 2021-10-09 improove processedPath (remove glitches)
 * 2021-10-29 add option to shift processed tile path to work area - importGraphicClipShowOrigPositionShiftTileProcessed
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GrblPlotter
{
    internal static partial class VisuGCode
    {
        // calculate intermediate steps betwee code lines
        public static class Simulation
        {
            private static int lineNr = 0;  // actual code line
            public static int dt = 50;    // step width
            private static XyzPoint posXY = new XyzPoint();
            private static double posZ = 0;
            private static double posA = 0;
            private static float feedRate = 1000;
            private const float feedRateRapid = 2000;
            private static double stepWidth = 10;
            private static double remainingStep = 10;
            private static bool isIntermediate = false;
            private static double distance = 0;
            private static SimuCoordByLine codeLast = new SimuCoordByLine();
            private static SimuCoordByLine codeNext = new SimuCoordByLine();
            private static bool isTangentialZ = false;
            private static bool isPenDownOld = false;
            private static bool isPenDownNow = false;
            private static bool isG2G3 = false;
            private static float drawAngleOld = 0;
            private static float drawAngleNow = 0;
            public static GraphicsPath pathSimulation = new GraphicsPath();

            private struct Length
            {
                public double XY;
                public double Z;
                public double A;
                public double Arc;
                public double Max;
            };
            private static Length diff;

            public static void Reset()
            {
                lineNr = 0; isIntermediate = false;
                diff.XY = diff.Z = diff.A = diff.Arc = 0;
                distance = 0; dt = 50; posZ = 0; posA = 0;
                remainingStep = stepWidth = 10;
                lastPosMarker = posXY = Grbl.PosMarker = new XyzPoint();
                Grbl.PosMarkerAngle = 0;
                codeNext = new SimuCoordByLine(simuList[lineNr]);
                CreateMarkerPath();
                isTangentialZ = (tangentialAxisName == "Z");
                pathSimulation.Reset();
                MarkSelectedFigure(-1);
                isPenDownOld = isPenDownNow = false;
            }
            public static double GetZ()
            { return posZ; }
            internal static int Next(ref XyzPoint coord)
            {
                if (isIntermediate)
                { isIntermediate = CalcIntermediatePos(); }
                if (!isIntermediate)
                {
                    if (!GetNextPos())                      //  finish simu if nextPos = false
                    {
                        Grbl.PosMarker = (XyzPoint)codeNext.actualPos;
                        Grbl.PosMarkerAngle = codeNext.alpha;
                        CreateMarkerPath(false, (XyPoint)codeNext.actualPos);
                        return -1;		// < 0 = stop
                    }
                    if (remainingStep <= 0)
                        remainingStep += stepWidth;
                    remainingStep -= distance;

                    if (remainingStep == 0)                 // just next full pos
                    {
                        Grbl.PosMarker = (XyzPoint)codeNext.actualPos;
                        Grbl.PosMarkerAngle = codeNext.alpha;
                        isPenDownNow = codeNext.motionMode > 0;
                        CreateSimulationPath((XyzPoint)codeNext.actualPos);
                        CreateMarkerPath(false, (XyPoint)codeNext.actualPos);
                        posXY = Grbl.PosMarker;
                        posA = Grbl.PosMarkerAngle;
                        coord.X = codeNext.actualPos.X;
                        coord.Y = codeNext.actualPos.Y;
                        coord.Z = codeNext.actualPos.Z;
                        coord.A = codeNext.alpha;
                        return codeNext.lineNumber;
                    }
                    else if (remainingStep > 0)             // move too short, get next gcode
                    {
                        isPenDownNow = codeNext.motionMode > 0;
                        CreateSimulationPath((XyzPoint)codeNext.actualPos);
                        while (remainingStep > 0)
                        {
                            if (!GetNextPos())              //  finish simu if nextPos = false //  calc distance & remaining steps
                            {
                                Grbl.PosMarker = (XyzPoint)codeNext.actualPos;
                                Grbl.PosMarkerAngle = codeNext.alpha;
                                CreateMarkerPath(false, (XyPoint)codeNext.actualPos);
                                return -codeNext.lineNumber;	// < 0 = stop
                            }
                            remainingStep -= distance;
                        }
                        if (remainingStep == 0)             // just next full pos
                        {
                            Grbl.PosMarker = (XyzPoint)codeNext.actualPos;
                            Grbl.PosMarkerAngle = codeNext.alpha;
                            isPenDownNow = codeNext.motionMode > 0;
                            CreateSimulationPath((XyzPoint)codeNext.actualPos);
                            CreateMarkerPath(false, (XyPoint)codeNext.actualPos);
                            posXY = Grbl.PosMarker;
                            posA = Grbl.PosMarkerAngle;
                            coord.X = codeNext.actualPos.X;
                            coord.Y = codeNext.actualPos.Y;
                            coord.Z = codeNext.actualPos.Z;
                            coord.A = codeNext.alpha;
                            return codeNext.lineNumber;
                        }
                    }
                    // remainingStep < 0 calc intermediate steps
                    posXY = (XyzPoint)codeLast.actualPos;
                    posZ = codeLast.actualPos.Z;
                    posA = codeLast.alpha;
                    diff.XY = ((XyPoint)codeLast.actualPos).DistanceTo((XyPoint)codeNext.actualPos);
                    diff.Z = Math.Abs(codeNext.actualPos.Z - codeLast.actualPos.Z);
                    diff.A = Math.Abs(codeNext.alpha - codeLast.alpha);
                    isIntermediate = true;
                    CalcIntermediatePos();
                    remainingStep = stepWidth;
                }
                Grbl.PosMarker = posXY;
                Grbl.PosMarkerAngle = posA; // posAngle;
                isPenDownNow = codeNext.motionMode > 0;
                CreateSimulationPath(posXY);
                CreateMarkerPath(false, (XyPoint)posXY);
                coord.X = posXY.X;
                coord.Y = posXY.Y;
                coord.Z = posZ;
                coord.A = posA;
                return codeNext.lineNumber;
            }

            /* Calculate intermediate position after fix time delay: simulationTimer = 50ms = 20Hz
               Step width depends on feedrate
            */
            private static bool CalcIntermediatePos()
            {
                UpdateFeedRate();
                remainingStep = stepWidth;
                double deltaA = codeNext.alpha - posA;
                isPenDownNow = codeNext.motionMode > 0;
                isG2G3 = codeNext.motionMode > 1;
                if (!isG2G3)
                {
                    double deltaS = ((XyPoint)(posXY)).DistanceTo((XyPoint)codeNext.actualPos);      // XY remaining max distance
                    if (tangentialAxisName != "Z")
                        deltaS = Math.Max(deltaS, Math.Abs(codeNext.actualPos.Z - posZ));   // Z  remaining max distance

                    if ((deltaS < remainingStep) && ((deltaA) < 0.1))       // return false if finish with intermediate
                    {
                        remainingStep -= deltaS;
                        CreateSimulationPath((XyzPoint)codeNext.actualPos);
                        return false;
                    }
                    double deltaX = codeNext.actualPos.X - posXY.X;     // get remaining distance
                    double deltaY = codeNext.actualPos.Y - posXY.Y;
                    double deltaZ = codeNext.actualPos.Z - posZ;
                    double dX = 0, dY = 0, dZ = 0, aStep = 10;
                    if (deltaS != 0)
                    {
                        dX = deltaX * remainingStep / deltaS;        // get step width relativ to max distance
                        dY = deltaY * remainingStep / deltaS;
                        dZ = deltaZ * remainingStep / deltaS;
                        aStep = diff.Max / remainingStep;               // amount of steps to reach end-value
                    }
                    posXY.X += dX;
                    posXY.Y += dY;
                    if (!CheckWithin((XyPoint)codeLast.actualPos, (XyPoint)codeNext.actualPos, (XyPoint)posXY))  // avoid going too far
                        posXY = (XyzPoint)codeNext.actualPos;

                    posA += (codeNext.alpha - codeLast.alpha) / aStep;	// step width = 1/10	dA;
                    if ((codeNext.z != null) && !isTangentialZ)
                        posZ += dZ;
                    return true;
                }
                else
                {
                    double aStep2 = remainingStep / arcMove.radius;         // get delta angle
                    double turnAngle = arcMove.angleEnd - arcMove.angleStart;
                    if (codeNext.motionMode == 2)
                    {
                        if (turnAngle > 0)
                            turnAngle -= 2 * Math.PI;
                    }
                    else
                    {
                        if (turnAngle < 0)
                            turnAngle += 2 * Math.PI;
                    }

                    if (turnAngle == 0)
                        turnAngle = 2 * Math.PI;
                    double dA = Math.Abs((codeNext.alpha - codeLast.alpha) / (turnAngle / aStep2));	// get step width 

                    drawAngleOld = drawAngleNow;
                    if (arcMove.angleDiff > 0)
                    {
                        if (angleTmp < (arcMove.angleStart + arcMove.angleDiff))
                            angleTmp += aStep2;
                        if (posA < codeNext.alpha)
                            posA += dA;
                        if ((angleTmp >= (arcMove.angleStart + arcMove.angleDiff)) && (posA >= codeNext.alpha)) // return false if finish with intermediate
                        {
                            drawAngleNow = (float)((arcMove.angleStart + arcMove.angleDiff) * 180 / Math.PI);
                            CreateSimulationPath((XyzPoint)codeNext.actualPos);
                            return false;
                        }
                    }
                    else
                    {
                        if (angleTmp > (arcMove.angleStart + arcMove.angleDiff))
                            angleTmp -= aStep2;
                        if (posA > codeNext.alpha)
                            posA -= dA;
                        if ((angleTmp <= (arcMove.angleStart + arcMove.angleDiff)) && (posA <= codeNext.alpha)) // return false if finish with intermediate
                        {
                            drawAngleNow = (float)((arcMove.angleStart + arcMove.angleDiff) * 180 / Math.PI);
                            CreateSimulationPath((XyzPoint)codeNext.actualPos);
                            return false;
                        }
                    }
                    posXY.X = arcMove.center.X + arcMove.radius * Math.Cos(angleTmp);
                    posXY.Y = arcMove.center.Y + arcMove.radius * Math.Sin(angleTmp);
                    drawAngleNow = (float)(angleTmp * 180 / Math.PI);
                    return true;
                }
            }

            private static bool GetNextPos()
            {
                lineNr++;
                if (lineNr >= simuList.Count)
                    return false;
                codeLast = new SimuCoordByLine(codeNext);
                codeNext = new SimuCoordByLine(simuList[lineNr]);
                if (codeNext.codeLine.Contains("M30"))          // program end
                    return false;
                UpdateFeedRate();
                distance = GetDistance();
                lastPosMarker = (XyzPoint)codeLast.actualPos;
                if ((remainingStep - distance) > 0)
                {
                    isG2G3 = codeNext.motionMode > 1;
                    CreateSimulationPath((XyzPoint)codeNext.actualPos);
                }
                return true;
            }

            private static void UpdateFeedRate()
            {
                feedRate = codeNext.feedRate;
                if (codeNext.motionMode == 0)
                    feedRate = feedRateRapid;
                stepWidth = feedRate * dt / 60000;   // feedrate in mm/min; dt in ms
            }

            private static double angleTmp = 0;
            private static ArcProperties arcMove;
            private static double GetDistance()
            {
                diff.Max = 0;
                diff.XY = ((XyPoint)codeLast.actualPos).DistanceTo((XyPoint)codeNext.actualPos);
                if (isTangentialZ)
                    diff.Z = 0;
                else
                    diff.Z = Math.Abs(codeNext.actualPos.Z - codeLast.actualPos.Z);
                diff.A = Math.Abs(codeNext.alpha - codeLast.alpha);

                if (diff.Max == 0)
                    diff.Max = diff.A;

                if (codeNext.motionMode < 2)
                {
                    diff.Max = Math.Max(diff.XY, diff.Z);
                    return diff.Max;
                }
                arcMove = GcodeMath.GetArcMoveProperties((XyPoint)codeLast.actualPos, (XyPoint)codeNext.actualPos, codeNext.i, codeNext.j, (codeNext.motionMode == 2));
                angleTmp = arcMove.angleStart;
                drawAngleOld = drawAngleNow = (float)(angleTmp * 180 / Math.PI);
                diff.Arc = Math.Abs(arcMove.angleDiff * arcMove.radius);
                diff.Max = diff.Arc;
                return diff.Max;
            }

            private static XyzPoint lastPosMarker = new XyzPoint();
            internal static void CreateSimulationPath(XyzPoint moveto)
            {
                PointF start = new PointF((float)lastPosMarker.X, (float)lastPosMarker.Y);
                PointF end = new PointF((float)moveto.X, (float)moveto.Y);
                if ((isPenDownOld == false) && (isPenDownNow == true))
                { pathSimulation.StartFigure(); }
                if (isPenDownNow)
                {
                    if (isG2G3)
                    {
                        float x1 = (float)(arcMove.center.X - arcMove.radius);
                        float y1 = (float)(arcMove.center.Y - arcMove.radius);
                        float r2 = 2 * (float)arcMove.radius;

                        if (r2 > 0)
                            pathSimulation.AddArc(x1, y1, r2, r2, drawAngleOld, (drawAngleNow - drawAngleOld));
                        lastPosMarker = moveto;
                    }
                    else if (CheckWithin((XyPoint)codeLast.actualPos, (XyPoint)codeNext.actualPos, (XyPoint)moveto))
                    {
                        pathSimulation.AddLine(start, end);
                        lastPosMarker = moveto;
                    }
                }
                isPenDownOld = isPenDownNow;
            }
            private static bool CheckWithin(XyPoint start, XyPoint end, XyPoint check)
            {
                if (check.X < Math.Min(start.X, end.X))
                    return false;
                if (check.X > Math.Max(start.X, end.X))
                    return false;
                if (check.Y < Math.Min(start.Y, end.Y))
                    return false;
                if (check.Y > Math.Max(start.Y, end.Y))
                    return false;
                return true;
            }
        }

        public static class ProcessedPath
        {
            private static XyzPoint lastPos = new XyzPoint();
            private static XyzPoint lastGCodePos = new XyzPoint();
            private static int lastLine = 0;
            private static int maxLine = 0;
            private static readonly float tolerance = 0.1f;              // to assume two values as same
            public static System.Windows.Point offset2DView;
            public static System.Windows.Point offset2DViewOld;

            private static int indexLastSucess = 0;
            private static readonly bool logProgress = false;

            public static void ProcessedPathClear()
            {
                Simulation.pathSimulation.Reset();
                lastLine = maxLine = indexLastSucess = 0;//currentLine
                lastPos = Grbl.posWork;
                VisuGCode.ProcessedPath.offset2DView = new System.Windows.Point();
            }

            public static void ProcessedPathLine(int line)		// 
            {
                maxLine = line;
            }

            private static int GetNextXYIndex(int start)
            {
                for (int i = start; i < simuList.Count; i++)
                {
                    if (!IsSameXYPos(start, i))
                        return i;
                }
                return start + 1;
            }
            private static bool IsSameXYPos(int a, int b)
            {
                if (!IsSameXYPos(simuList[a].actualPos, simuList[b].actualPos))
                    return false;
                if ((simuList[a].motionMode == 0) && (simuList[b].motionMode > 1))
                    return false;   // same pos but diff motionMode -> full circle?
                return true;
            }
            private static bool IsSameXYPos(XyzPoint a, XyzPoint b)
            {
                if ((Math.Abs(a.X - b.X) > tolerance) || (Math.Abs(a.Y - b.Y) > tolerance))
                    return false;
                return true;
            }


            internal static void ProcessedPathDraw(XyzPoint newPos)			// called by OnRaisePosEvent in MainFormInterface.cs during streaming
            {
                int iStart, iEnd;
                bool onTrack = false;

                if ((lastPos.X == newPos.X) && (lastPos.Y == newPos.Y))		// no change in grbl position - nothing to do
                    return;

                if ((simuList == null) || (simuList.Count == 0) || ((maxLine + 1) >= simuList.Count))
                    return;

                XyzPoint newPosTileOffseted;
                XyzPoint tileOffset;

                if (logProgress) Logger.Trace("Processed path lastLine:{0,2}", lastLine);
                for (iStart = lastLine; iStart < maxLine; iStart++)			            // find two consecutive command lines
                {
                    onTrack = false;
                    iEnd = iStart + 1;
                    if ((simuList[iEnd].motionMode < 1) || IsSameXYPos(iStart, iEnd))	// no change in position or motionMode? Try next line
                    {
                        iEnd = GetNextXYIndex(iEnd);
                        iStart = iEnd - 1;
                    }

                    if ((simuList[iStart].motionMode == 0) && (simuList[iEnd].motionMode > 0))  // new figure starts
                    { Simulation.pathSimulation.StartFigure(); }

                    // simuList contains the, in 2D-view, visible coordinates - for tiles also out of working coordinates - needs to be corrected:
                    tileOffset = simuList[iEnd].actualOffset;
                    newPosTileOffseted = newPos + tileOffset;       // shift simuList-Coordinate to working-coordinates

                    if (simuList[iEnd].motionMode > 1)			    // check if newPos is on track of an arc
                    {
                        ArcProperties arcMove1, arcMove2;
                        arcMove1 = GcodeMath.GetArcMoveProperties((XyPoint)simuList[iStart].actualPos, (XyPoint)simuList[iEnd].actualPos, simuList[iEnd].i, simuList[iEnd].j, (simuList[iEnd].motionMode == 2));
                        arcMove2 = GcodeMath.GetArcMoveProperties((XyPoint)simuList[iStart].actualPos, (XyPoint)newPosTileOffseted, simuList[iEnd].i, simuList[iEnd].j, (simuList[iEnd].motionMode == 2));
                        onTrack = PointOnArc(arcMove1, arcMove2, ToPointF(newPosTileOffseted));
                    }
                    else if (simuList[iEnd].motionMode == 1)				// check if newPos is on track of a line
                    {
                        onTrack = PointOnLine(ToPointF(simuList[iStart].actualPos), ToPointF(simuList[iEnd].actualPos), ToPointF(newPosTileOffseted));
                    }

                    if (logProgress) Logger.Trace("Processed path iStart:{0,2}  iStart.X:{1,2:0.0} Y:{2,2:0.0}  iEnd:{3,2}  iEnd.X:{4,2:0.0} Y:{5,2:0.0}  newPos X:{6,2:0.0} Y:{7,2:0.0}  onTrack:{8}", iStart, simuList[iStart].actualPos.X, simuList[iStart].actualPos.Y, iEnd, simuList[iEnd].actualPos.X, simuList[iEnd].actualPos.Y, newPos.X, newPos.Y, onTrack);

                    if (IsSameXYPos(simuList[iEnd].actualPos, newPosTileOffseted))
                    { lastPos = newPos; }

                    if (onTrack)			// two consecutive command lines found (where newPos is in-between) - were lines skipped?
                    {
                        indexLastSucess = iStart;
                        for (int k = lastLine + 1; k <= iStart; k++)	// were lines skipped?
                        {
                            // no movement - nothing to do - except probably full circle
                            if ((lastGCodePos.X == simuList[k].actualPos.X) && (lastGCodePos.Y == simuList[k].actualPos.Y) && (simuList[k].motionMode < 2))
                            { continue; }

                            // draw skipped lines
                            if (simuList[k].motionMode == 0)		// new path
                            {
                                lastGCodePos = new XyzPoint(simuList[k].actualPos.X, simuList[k].actualPos.Y, simuList[k].actualPos.Z);
                                Simulation.pathSimulation.StartFigure();
                            }
                            else if (simuList[k].motionMode == 1)	// line
                            {
                                Simulation.pathSimulation.AddLine(ToPointF(lastGCodePos - tileOffset), ToPointF(simuList[k].actualPos - tileOffset));
                                if (logProgress) Logger.Trace("...for k AddLine lastGCodePos.X:{0,2:0.0} Y:{1,2:0.0}  k:{2,2}  k.X:{3,2:0.0} Y:{4,2:0.0} ", lastGCodePos.X, lastGCodePos.Y, k, simuList[k].actualPos.X, simuList[k].actualPos.Y);
                            }
                            else if (simuList[k].motionMode >= 2)	// arc
                            {
                                ArcProperties arcMove;
                                arcMove = GcodeMath.GetArcMoveProperties((XyPoint)lastGCodePos, (XyPoint)simuList[k].actualPos, simuList[k].i, simuList[k].j, (simuList[k].motionMode == 2));

                                float x1 = (float)(arcMove.center.X - arcMove.radius);
                                float y1 = (float)(arcMove.center.Y - arcMove.radius);
                                float r2 = 2 * (float)arcMove.radius;
                                float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
                                float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);
                                if (arcMove.radius > 0)
                                {
                                    Simulation.pathSimulation.AddArc(x1, y1, r2, r2, aStart, aDiff);
                                    if (logProgress) Logger.Trace("...for k AddArc lastGCodePos.X:{0,2:0.0} Y:{1,2:0.0}  k:{2,2}  k.X:{3,2:0.0} Y:{4,2:0.0} ", lastGCodePos.X, lastGCodePos.Y, k, simuList[k].actualPos.X, simuList[k].actualPos.Y);
                                }
                            }
                            lastGCodePos = new XyzPoint(simuList[k].actualPos.X, simuList[k].actualPos.Y, simuList[k].actualPos.Z);
                        }


                        if (simuList[iEnd].motionMode == 1)	// draw path-line between last grblPos and actual newPos
                        {
                            Simulation.pathSimulation.AddLine(ToPointF(lastGCodePos - tileOffset), ToPointF(newPos));
                        }
                        else if (simuList[iEnd].motionMode > 1) 	// draw path-arc between last grblPos and actual newPos
                        {
                            Simulation.pathSimulation.AddLine(ToPointF(lastPos - tileOffset), ToPointF(newPos));
                        }

                        lastLine = iStart;

                        if (Properties.Settings.Default.importGraphicClipShowOrigPositionShiftTileProcessed)
                        {
                            offset2DView = new System.Windows.Point(simuList[iStart].actualOffset.X, simuList[iStart].actualOffset.Y);

                            if (!offset2DView.Equals(offset2DViewOld))  // special case tiles - new tile, reset simulation-path
                                Simulation.pathSimulation.Reset();
                        }
                        offset2DViewOld = offset2DView;

                        lastPos = newPos;
                        indexLastSucess = iStart;
                        break;
                    }
                }
                // no match found, try new start-pos
                if (!onTrack)
                { lastGCodePos = new XyzPoint(simuList[indexLastSucess].actualPos.X, simuList[indexLastSucess].actualPos.Y, simuList[indexLastSucess].actualPos.Z); }
            }
            private static PointF ToPointF(XyzPoint tmp)
            { return new PointF((float)tmp.X, (float)tmp.Y); }

            private static bool PointOnArc(ArcProperties arcMove1, ArcProperties arcMove2, PointF xp)
            {
                double dx = arcMove1.center.X - xp.X;
                double dy = arcMove1.center.Y - xp.Y;
                double r = Math.Sqrt(dx * dx + dy * dy);

                if (!IsEqual(r, arcMove1.radius))
                    return false;

                if (arcMove1.angleEnd == arcMove1.angleStart)
                    return true;

                if (arcMove2.angleEnd < arcMove1.angleStart)
                    return false;
                if (arcMove2.angleEnd > arcMove1.angleEnd)
                    return false;

                return true;
            }

            private static bool PointOnLine(PointF p1, PointF p2, PointF xp)
            {
                if ((xp.X < Math.Min(p1.X - tolerance, p2.X - tolerance)) || (xp.X > Math.Max(p1.X + tolerance, p2.X + tolerance)))
                {
                    return false;
                }
                if ((xp.Y < Math.Min(p1.Y - tolerance, p2.Y - tolerance)) || (xp.Y > Math.Max(p1.Y + tolerance, p2.Y + tolerance)))
                {
                    return false;
                }
                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;
                if (IsEqual(dx, 0, tolerance))
                {
                    return (IsEqual(p1.X, xp.X, tolerance));
                }
                if (IsEqual(dy, 0, tolerance))
                {
                    return (IsEqual(p1.Y, xp.Y, tolerance));
                }

                double m = dy / dx;
                double b = p1.Y - (m * p1.X);
                double y = m * xp.X + b;
                return (IsEqual(y, xp.Y, tolerance));
            }
            private static bool IsEqual(double a, double b, double max = 0.1)
            {
                if (Math.Abs(a - b) < max)
                    return true;
                return false;
            }
        }
    }
}