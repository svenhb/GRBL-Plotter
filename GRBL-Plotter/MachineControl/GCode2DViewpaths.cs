/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
/*  GCode2DViewPaths.cs
	Fill GraphicPaths
*/
/* 
 * 2021-07-08 split code from GCodeVisuAndTransform
 * 2021-07-27 code clean up / code quality
 * 2021-09-02 CreateDrawingPathFromGCode add viewOffset for tiles
 * 2021-09-29 update fiducialDimension
 * 2021-09-30 take care for inch:  if (!Properties.Settings.Default.importUnitmm || (modal.unitsMode == 20))
 * 2021-11-18 show path-nodes gui2DShowVertexEnable - will be switched off on prog-start	 
 * 2022-04-07 DrawHeightMap add side-view of shape at y=0 and x=0 (below and left of hight-map grid)
 * 2023-04-11 l:683 f:CreateRuler lock object to avoid 'object is currently in use elsewhere'
 * 2023-09-01 new l:700 f:SetRulerDimension / l:380 f:DrawMachineLimit update grid
 * 2023-09-14 l:400 f:DrawMachineLimit() add toolTableOffsetX to show tool positions; issue #361
 * 2024-01-13 l:675 f:CreateMarkerArrow add try/catch
 * 2024-01-25 l:262 f:CreateDrawingPathFromGCode get markerSize from graphics dimension
 * 2024-09-13 l:329 add p-word for G2/3
 * 2024-12-09 l:282 f:CreateDrawingPathFromGCode add pixelArtEnable
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GrblPlotter
{
    internal struct DrawingProperties
    {
        public float minX, minY, maxX, maxY, rangeX, rangeY;
        public void DrawingProperty()
        { minX = 0; minY = 0; maxX = 0; maxY = 0; rangeX = 0; rangeY = 0; }
        public void SetX(float min, float max)
        { minX = Math.Min(min, max); maxX = Math.Max(min, max); rangeX = maxX - minX; }
        public void SetY(float min, float max)
        { minY = Math.Min(min, max); maxY = Math.Max(min, max); rangeY = maxY - minY; }
    };
    internal static partial class VisuGCode
    {
        internal static DrawingProperties drawingSize = new DrawingProperties();
        internal static DrawingProperties RulerDimension = new DrawingProperties();

        internal static GraphicsPath pathPenUp = new GraphicsPath();
        internal static GraphicsPath pathPenDown = new GraphicsPath();
        internal static GraphicsPath pathRuler = new GraphicsPath();
        internal static GraphicsPath pathGrid1 = new GraphicsPath();
        internal static GraphicsPath pathGrid10 = new GraphicsPath();
        internal static GraphicsPath pathGrid100 = new GraphicsPath();
        internal static GraphicsPath pathGrid1000 = new GraphicsPath();
        internal static GraphicsPath pathGrid10000 = new GraphicsPath();
        internal static GraphicsPath pathTool = new GraphicsPath();
        internal static GraphicsPath pathMarker = new GraphicsPath();
        internal static GraphicsPath pathHeightMap = new GraphicsPath();
        internal static GraphicsPath pathMachineLimit = new GraphicsPath();
        internal static GraphicsPath pathToolTable = new GraphicsPath();
        internal static GraphicsPath pathBackground = new GraphicsPath();
        internal static GraphicsPath pathMarkSelection = new GraphicsPath();
        internal static GraphicsPath pathRotaryInfo = new GraphicsPath();
        internal static GraphicsPath pathDimension = new GraphicsPath();
        internal static GraphicsPath path = pathPenUp;

        internal static GraphicsPath pathActualDown = pathPenDown;

        private static XyPoint lastXYG123Position = new XyPoint();
        private static XyPoint figurePoint0 = new XyPoint();
        private static XyPoint figurePoint1 = new XyPoint();
        private static int figurePointCount = 0;

        // Clear GraphicsPath and dimensions
        public static void ClearDrawingPath()
        {
            xyzSize.ResetDimension();
            G0Size.ResetDimension();
            pathPenUp.Reset();
            pathPenDown.Reset();
            pathRuler.Reset();
            pathTool.Reset();
            pathMarker.Reset();
            pathHeightMap.Reset();
            //       pathBackground.Reset();
            pathMarkSelection.Reset();
            pathRotaryInfo.Reset();
            pathDimension.Reset();
            Simulation.pathSimulation.Reset();

            pathObject.Clear();
            path = pathPenUp;
            onlyZ = 0;
            figureCount = 0;
        }

        /// <summary>
        /// add segement to drawing path 'PenUp' or 'PenDown' from old-xyz to new-xyz
        /// </summary>
        private static bool CreateDrawingPathFromGCode(GcodeByLine newL, GcodeByLine oldL, PointF viewOffset, int pWord)
        {
            bool passLimit = false;
            bool zUp, zDown;
            var pathOld = path;

            if (newL.isSubroutine && (!oldL.isSubroutine))
                MarkPath(pathPenUp, (float)newL.actualPos.X, (float)newL.actualPos.Y, 2); // 2=rectangle

            if (!newL.ismachineCoordG53)
            {
                zDown = (newL.actualPos.Z < 0) || (newL.codeLine.Contains("(PD"));
                if (((newL.motionMode > 0) && (oldL.motionMode == 0)) || zDown)  // G0 = PenUp
                {
                    path = pathPenDown;
                    path.StartFigure();
                    pathActualDown?.StartFigure();
                }
                zUp = oldL.codeLine.Contains("(PU") || ((newL.actualPos.Z > 0) && (oldL.actualPos.Z < 0));
                if (((newL.motionMode == 0) && (oldL.motionMode > 0)) || zUp)
                { path = pathPenUp; path.StartFigure(); }

                if (newL.wasSetXY)              // set pen-up arrow
                {
                    if (newL.motionMode == 0)   // a new path will start
                    {
                        double tmpAngle = GcodeMath.GetAlpha(lastXYG123Position, (XyPoint)newL.actualPos);
                        tempPathInfo = new PathInfo
                        {
                            position = new PointF((float)newL.actualPos.X + viewOffset.X, (float)newL.actualPos.Y + viewOffset.Y),
                            angleArrow = tmpAngle,
                            angleFigure = tmpAngle
                        };
                        figurePointCount = 0;
                        figurePoint0 = (XyPoint)newL.actualPos;
                    }
                    else                        // store end of current path
                    {
                        lastXYG123Position = (XyPoint)newL.actualPos;
                        if (figurePointCount++ == 0)
                        {
                            figurePoint1 = (XyPoint)newL.actualPos;
                            //tempPathInfo.angle = GcodeMath.GetAlpha(lastXYG123Position, (XyPoint)newL.actualPos);
                            if (pathInfoMarker.Count > 0)		// will be overwritten if G2/G3
                            {
                                PathInfo tmpPathInfo = pathInfoMarker[pathInfoMarker.Count - 1];
                                tmpPathInfo.angleFigure = GcodeMath.GetAlpha(figurePoint0, figurePoint1);
                                pathInfoMarker[pathInfoMarker.Count - 1] = tmpPathInfo;
                            }

                        }       // handle G2 G3 !!!				
                    }
                }

                if ((path != pathOld))
                {
                    passLimit = true;
                    if (figureMarkerCount <= 0)
                        path.SetMarkers(); //path.StartFigure();
                    else
                    {
                        if (figureMarkerCount != figureCount)
                        { path.SetMarkers(); }// path.StartFigure(); }
                    }

                    if (path == pathPenDown)    // this means pathPenUp ended
                    {
                        if (figureMarkerCount <= 0)
                        {
                            if (pathPenDown.PointCount > 0)
                            {
                                figureCount++;                  // only inc. if old figure was filled
                                oldL.figureNumber = figureCount;
                            }
                        }
                        else
                        {
                            figureCount = figureMarkerCount;
                            oldL.figureNumber = figureCount;
                        }
                        tempPathInfo.info = figureCount.ToString();
                        pathInfoMarker.Add(tempPathInfo);
                    }
                }

                if (newL.motionMode == 0 || newL.motionMode == 1 || passLimit)
                {
                    bool otherAxis = (newL.actualPos.A != oldL.actualPos.A) || (newL.actualPos.B != oldL.actualPos.B) || (newL.actualPos.C != oldL.actualPos.C);
                    otherAxis = otherAxis || (newL.actualPos.U != oldL.actualPos.U) || (newL.actualPos.V != oldL.actualPos.V) || (newL.actualPos.W != oldL.actualPos.W);
                    if ((newL.actualPos.X != oldL.actualPos.X) || (newL.actualPos.Y != oldL.actualPos.Y) || otherAxis || (oldL.motionMode == 2 || oldL.motionMode == 3))
                    {
                        if ((Properties.Settings.Default.ctrl4thUse) && (path == pathPenDown))
                        {
                            if (passLimit)
                                pathRotaryInfo.StartFigure();
                            float scale = (float)Properties.Settings.Default.rotarySubstitutionDiameter * (float)Math.PI / 360;
                            if (Properties.Settings.Default.ctrl4thInvert)
                                scale *= -1;

                            float newR = 0, oldR = 0;
                            if (Properties.Settings.Default.ctrl4thName == "A") { oldR = (float)oldL.actualPos.A * scale; newR = (float)newL.actualPos.A * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "B") { oldR = (float)oldL.actualPos.B * scale; newR = (float)newL.actualPos.B * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "C") { oldR = (float)oldL.actualPos.C * scale; newR = (float)newL.actualPos.C * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "U") { oldR = (float)oldL.actualPos.U * scale; newR = (float)newL.actualPos.U * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "V") { oldR = (float)oldL.actualPos.V * scale; newR = (float)newL.actualPos.V * scale; }
                            else if (Properties.Settings.Default.ctrl4thName == "W") { oldR = (float)oldL.actualPos.W * scale; newR = (float)newL.actualPos.W * scale; }

                            if (Properties.Settings.Default.ctrl4thOverX)
                            {
                                pathRotaryInfo.AddLine((float)oldL.actualPos.X, oldR, (float)newL.actualPos.X, newR); // rotary over X
                                xyzSize.SetDimensionY(newR);
                            }
                            else
                            {
                                pathRotaryInfo.AddLine(oldR, (float)oldL.actualPos.Y, newR, (float)newL.actualPos.Y); // rotary over Y
                                xyzSize.SetDimensionX(newR);
                            }
                        }

                        //    if (!((path == pathPenUp) && largeDataAmount && (oldL.lineNumber > 10) && (oldL.lineNumber < (numberDataLines - 10))))
                        // no improovement, when skipping pen-up paths 2022-04-20
                        path.AddLine((float)oldL.actualPos.X + viewOffset.X, (float)oldL.actualPos.Y + viewOffset.Y, (float)newL.actualPos.X + viewOffset.X, (float)newL.actualPos.Y + viewOffset.Y);   // 2021-09-02

                        if (Properties.Settings.Default.gui2DShowVertexEnable && !largeDataAmount)
                        {
                            CreateMarker(path, (float)oldL.actualPos.X + viewOffset.X, (float)oldL.actualPos.Y + viewOffset.Y, (float)Properties.Settings.Default.gui2DShowVertexSize, (int)Properties.Settings.Default.gui2DShowVertexType, false);
                            path.StartFigure();
                        }

                        if ((path == pathPenDown) && (pathActualDown != null))
                        {
                            pathActualDown.AddLine((float)oldL.actualPos.X + viewOffset.X, (float)oldL.actualPos.Y + viewOffset.Y, (float)newL.actualPos.X + viewOffset.X, (float)newL.actualPos.Y + viewOffset.Y);
                            if (Properties.Settings.Default.gui2DShowVertexEnable && !largeDataAmount)
                            {
                                CreateMarker(pathActualDown, (float)oldL.actualPos.X + viewOffset.X, (float)oldL.actualPos.Y + viewOffset.Y, (float)Properties.Settings.Default.gui2DShowVertexSize, (int)Properties.Settings.Default.gui2DShowVertexType, false);
                                pathActualDown.StartFigure();
                            }
                            if (fiducialEnable)
                            {
                                fiducialDimension.SetDimensionXY((XyPoint)oldL.actualPos); fiducialDimension.SetDimensionXY((XyPoint)newL.actualPos);
                                //                                Logger.Trace("Set fiducial dim line {0} {1}  {2} {3}", oldL.actualPos.X, oldL.actualPos.Y, newL.actualPos.X, newL.actualPos.Y);
                            }
                        }
                        onlyZ = 0;  // x or y has changed
                    }
                    if (newL.actualPos.Z != oldL.actualPos.Z)  //else
                    { onlyZ++; }

                    // mark Z-only movements - could be drills
                    if ((onlyZ > 1) && (passLimit) && (path == pathPenUp) || (oldL.codeLine.Contains("DOT")))  // pen moved from -z to +z
                    {
                        float markerSize = (float)markerSizeGraphic;
                        int markerType = 1;
                        if (!Properties.Settings.Default.importUnitmm || (modal.unitsMode == 20))
                        { markerSize /= 25.4F; }
                        if (Properties.Settings.Default.importSVGCircleToDot)
                        {
                            if (Properties.Settings.Default.importSVGCircleToDotZ && Properties.Settings.Default.importGCZEnable)
                            {
                                markerSize = (float)oldL.actualPos.Z;
                                if (markerSize < 0)
                                    markerSize = Math.Abs(markerSize);
                                else

                                    markerSize = 0;
                            }
                            //        markerType = 4;
                        }

                        if (pixelArtEnable)
                        {
                            markerType = 5;
                            markerSize = (float)pixelArtDotWidth / 4;
                            CreateMarker(pathPenDown, (XyPoint)newL.actualPos, markerSize * 2, markerType, false); // draw rect
                        }
                        CreateMarker(pathPenDown, (XyPoint)newL.actualPos, markerSize, markerType, false);       // draw cross
                                                                                                                 //    if ((path == pathPenDown) && (pathActualDown != null))
                        if (pathActualDown != null)
                        {
                            XyPoint tmpPoint = new XyPoint(newL.actualPos.X + viewOffset.X, newL.actualPos.Y + viewOffset.Y);
                            CreateMarker(pathActualDown, tmpPoint, markerSize, markerType, false);               // draw cross
                                                                                                                 //        Logger.Trace("draw x:{0}  z:{1}", tmpPoint.X, markerSize);
                        }
                        CreateMarker(pathPenUp, (XyPoint)newL.actualPos, markerSize, 4, false);       	// draw circle
                        path = pathPenUp;
                        onlyZ = 0;
                        if (fiducialEnable)
                        {
                            fiducialsCenter.Add((XyPoint)newL.actualPos);
                            //                Logger.Trace("Set fiducial add point {0} {1}  ", newL.actualPos.X, newL.actualPos.Y);
                        }
                        //       passLimit = false;
                    }
                }
                //         }
                else if ((newL.motionMode == 2 || newL.motionMode == 3) && (newL.i != null || newL.j != null))
                {
                    if (newL.i == null) { newL.i = 0; }
                    if (newL.j == null) { newL.j = 0; }

                    ArcProperties arcMove;
                    arcMove = GcodeMath.GetArcMoveProperties((XyPoint)oldL.actualPos, (XyPoint)newL.actualPos, newL.i, newL.j, (newL.motionMode == 2));
                    //       centerList.Add(new CoordByLine(newL.lineNumber, figureCount, new XyzPoint(arcMove.center, 0), new XyzPoint(arcMove.center, 0), newL.motionMode, 0, true));
                    centerList.Add(new CenterByLine(newL.lineNumber, newL.figureNumber, new XyzPoint(arcMove.center, newL.actualPos.Z), (XyzPoint)newL.actualPos, newL.alpha));
                    if (fiducialEnable) { fiducialsCenter.Add(arcMove.center); }

                    newL.distance = Math.Abs(arcMove.radius * arcMove.angleDiff);

                    double x1 = (arcMove.center.X - arcMove.radius);
                    double y1 = (arcMove.center.Y - arcMove.radius);
                    double r2 = 2 * arcMove.radius;
                    double aStart = (arcMove.angleStart * 180 / Math.PI);
                    double aDiff = (arcMove.angleDiff * 180 / Math.PI);

                    /*    if ((aDiff != 0) && (aDiff < 1) && (arcMove.radius > 1000))
                        {   // just draw a line
                            path.AddLine((float)oldL.actualPos.X + viewOffset.X, (float)oldL.actualPos.Y + viewOffset.Y, (float)newL.actualPos.X + viewOffset.X, (float)newL.actualPos.Y + viewOffset.Y);
                            if ((path == pathPenDown) && (pathActualDown != null))
                                pathActualDown.AddLine((float)oldL.actualPos.X + viewOffset.X, (float)oldL.actualPos.Y + viewOffset.Y, (float)newL.actualPos.X + viewOffset.X, (float)newL.actualPos.Y + viewOffset.Y);
                        }
                        else*/
                    {   // Draw real arc
                        if (arcMove.radius > 0)
                        {
                            if (pWord > 1)
                            {
                                if (aDiff > 0) { aDiff = 360; }
                                else { aDiff = -360; }
                            }

                            path.AddArc((float)x1 + viewOffset.X, (float)y1 + viewOffset.Y, (float)r2, (float)r2, (float)aStart, (float)aDiff);
                            if ((path == pathPenDown) && (pathActualDown != null))
                                pathActualDown.AddArc((float)x1 + viewOffset.X, (float)y1 + viewOffset.Y, (float)r2, (float)r2, (float)aStart, (float)aDiff);
                        }
                        else
                        {
                            if (errorString.Length == 0) errorString += "ERROR ";
                            errorString += string.Format("Line:{0} radius=0 '{1}' | ", (newL.lineNumber + 1), newL.codeLine);
                        }

                        if (pathInfoMarker.Count > 0)       // will be overwritten if G2/G3
                        {
                            PathInfo tmpPathInfo = pathInfoMarker[pathInfoMarker.Count - 1];
                            tmpPathInfo.angleFigure = GcodeMath.GetAlpha(figurePoint0, new XyPoint(arcMove.center.X + viewOffset.X, arcMove.center.Y + viewOffset.Y)) + ((newL.motionMode == 2) ? (Math.PI / 2) : -(Math.PI / 2));
                            pathInfoMarker[pathInfoMarker.Count - 1] = tmpPathInfo;
                        }

                    }
                    if (!newL.ismachineCoordG53)
                        xyzSize.SetDimensionCircle(arcMove.center.X, arcMove.center.Y, arcMove.radius, aStart, aDiff);        // calculate new dimensions
                }
            }
            if (path == pathPenDown)
                newL.figureNumber = figureCount;
            else
                newL.figureNumber = -1;

            return true;// figureStart;
        }

        /// <summary>
        /// create paths with machine limits and tool positions in machine coordinates
        /// </summary>
        internal static void DrawMachineLimit()
        {
            float offsetX = (float)Grbl.posWCO.X;
            float offsetY = (float)Grbl.posWCO.Y;
            float x1 = (float)Properties.Settings.Default.machineLimitsHomeX - offsetX;
            float y1 = (float)Properties.Settings.Default.machineLimitsHomeY - offsetY;
            float rx = (float)Properties.Settings.Default.machineLimitsRangeX;
            float ry = (float)Properties.Settings.Default.machineLimitsRangeY;
            if (rx <= 0) rx = 10;
            if (ry <= 0) ry = 10;
            float extend = 2 * rx;
            Matrix matrix = new Matrix();
            matrix.Scale(1, -1);

            RectangleF pathRect1 = new RectangleF(x1, y1, rx, ry);
            RectangleF pathRect2 = new RectangleF(x1 - extend, y1 - extend, rx + 2 * extend, ry + 2 * extend);

            pathMachineLimit.Reset();
            pathMachineLimit.StartFigure();
            pathMachineLimit.AddRectangle(pathRect1);
            pathMachineLimit.AddRectangle(pathRect2);

            if (Properties.Settings.Default.machineLimitsShow)
            {
                DrawingProperties drawingSize = new DrawingProperties();
                drawingSize.SetX(x1, x1 + rx);
                drawingSize.SetY(y1, y1 + ry);
                SetRulerDimension(drawingSize);
            }

            pathToolTable.Reset();
            if ((ToolTable.toolTableArray != null) && (ToolTable.toolTableArray.Count > 1))
            {
                double wx, wy;
                XyzPoint tmppos;
                ToolProp tmpTool;
                for (int i = 1; i < ToolTable.toolTableArray.Count; i++)
                {
                    tmpTool = ToolTable.toolTableArray[i];
                    tmppos = tmpTool.Position;
                    wx = tmppos.X - offsetX + (double)Properties.Settings.Default.toolTableOffsetX;
                    wy = tmppos.Y - offsetY + (double)Properties.Settings.Default.toolTableOffsetY;
                    try
                    {
                        FontFamily myFont = new FontFamily("Arial");
                        if ((tmpTool.Name != null) && (tmpTool.Name.Length > 1) && (tmpTool.Toolnr >= 0))
                        {
                            pathToolTable.StartFigure();
                            pathToolTable.AddEllipse((float)(wx - 4), (float)(wy - 4), 8, 8);
                            pathToolTable.Transform(matrix);
                            pathToolTable.AddString(tmpTool.Toolnr.ToString() + ") " + tmpTool.Name, myFont, (int)FontStyle.Regular, 4, new Point((int)wx - 12, -(int)wy + 4), StringFormat.GenericDefault);
                            pathToolTable.Transform(matrix);
                        }
                        myFont.Dispose();
                    }
                    catch (Exception er) { Logger.Error(er, " drawMachineLimit"); }
                }
            }
            matrix.Dispose();
            origWCOMachineLimit = (XyPoint)Grbl.posWCO;
        }

        /// <summary>
        /// create height map path in work coordinates
        /// </summary>
        internal static void DrawHeightMap(HeightMap Map)
        {
            pathHeightMap.Reset();
            Vector2 tmp, tmpOld;
            int x = 0, y;
            for (y = 0; y < Map.SizeY; y++)
            {
                tmp = Map.GetCoordinates(x, y);
                pathHeightMap.StartFigure();
                pathHeightMap.AddLine((float)Map.Min.X, (float)tmp.Y, (float)Map.Max.X, (float)tmp.Y);
            }
            for (x = 0; x < Map.SizeX; x++)
            {
                tmp = Map.GetCoordinates(x, Map.SizeY - 1);
                pathHeightMap.StartFigure();
                pathHeightMap.AddLine((float)tmp.X, (float)Map.Min.Y, (float)tmp.X, (float)Map.Max.Y);
            }

            // show X shape -> Z on Y axis
            double z, zOld, offsetX = -10, offsetY = -10;
            //	double dimX = Map.Max.X - Map.Min.X;
            float emSize = 2;
            float emOffset = emSize / 2;
            GraphicsPath pathToDraw = pathBackground;
            pathToDraw.Reset();
            pathToDraw.StartFigure();
            tmpOld = Map.GetCoordinates(0, 0);
            zOld = Map.InterpolateZ(tmpOld.X, tmpOld.Y);
            if (Math.Abs(zOld) < emSize)
                emOffset = emSize;

            /* info below x axis */
            pathToDraw.AddLine((float)Map.Min.X, (float)(Map.Min.Y + offsetY), (float)Map.Max.X, (float)(Map.Min.Y + offsetY));     // zreo Z
            AddBackgroundText(pathToDraw, new PointF((float)Map.Min.X, (float)(Map.Min.Y + offsetY + emSize * 1.5)), emSize, string.Format("Z profile over X, at Y={0:0.00}", tmpOld.Y));
            AddBackgroundText(pathToDraw, new PointF((float)Map.Max.X + emSize, (float)(Map.Min.Y + offsetY + emSize / 2)), emSize, "Z= 0.00");
            AddBackgroundText(pathToDraw, new PointF((float)Map.Max.X + emSize, (float)(Map.Min.Y + offsetY + zOld - emOffset)), emSize, string.Format("Z= {0:0.00}", zOld));

            pathToDraw.StartFigure();
            for (x = 1; x < Map.SizeX; x++)
            {
                tmp = Map.GetCoordinates(x, 0);
                z = Map.InterpolateZ(tmp.X, tmp.Y);
                pathToDraw.AddLine((float)tmpOld.X, (float)(Map.Min.Y + offsetY + zOld), (float)tmp.X, (float)(Map.Min.Y + offsetY + z));
                tmpOld = tmp;
                zOld = z;
            }

            /* info left of y axis */
            tmpOld = Map.GetCoordinates(0, 0);
            zOld = Map.InterpolateZ(tmpOld.X, tmpOld.Y);

            pathToDraw.StartFigure();
            pathToDraw.AddLine((float)(Map.Min.X + offsetX), (float)Map.Min.Y, (float)(Map.Min.X + offsetX), (float)Map.Max.Y);     // zreo Z
                                                                                                                                    //	AddBackgroundText(pathToDraw, new PointF((float)Map.Min.X, (float)(offsetY + emSize * 1.5)), emSize, string.Format("Z profile over X, at Y={0:0.00}", tmpOld.Y));
                                                                                                                                    //	AddBackgroundText(pathToDraw, new PointF((float)(Map.Max.X + offsetX - emSize), (float)(Map.Min.Y - emSize/2)), emSize, "Z= 0.00", true);
                                                                                                                                    //	AddBackgroundText(pathToDraw, new PointF((float)(Map.Max.X + offsetX + zOld - emOffset), (float)(Map.Min.Y - emSize/2)), emSize, string.Format("Z= {0:0.00}",zOld), true);

            pathToDraw.StartFigure();
            for (y = 1; y < Map.SizeY; y++)
            {
                tmp = Map.GetCoordinates(0, y);
                z = Map.InterpolateZ(tmp.X, tmp.Y);
                pathToDraw.AddLine((float)(Map.Min.X + offsetX + zOld), (float)tmpOld.Y, (float)(Map.Min.X + offsetX + z), (float)tmp.Y);
                tmpOld = tmp;
                zOld = z;
            }

            tmp = Map.GetCoordinates(0, 0);
            xyzSize.SetDimensionXY(tmp.X, tmp.Y);
            tmp = Map.GetCoordinates(Map.SizeX, Map.SizeY);
            xyzSize.SetDimensionXY(tmp.X, tmp.Y);
        }

        /// <summary>
        /// copy actual gcode-pathPenDown to background path with machine coordinates
        /// </summary>
        public static void SetPathAsLandMark(bool clear)
        {
            if (clear)
            {
                pathBackground.Reset();
                coordListLandMark.Clear();
                return;
            }
            pathBackground = (GraphicsPath)pathPenDown.Clone();
            coordListLandMark = new List<CoordByLine>();
            bool isArc;
            if ((coordList != null) && (coordList.Count > 0))
            {
                foreach (CoordByLine gcline in coordList)        // copy coordList and add WCO
                {
                    isArc = ((newLine.motionMode == 2) || (newLine.motionMode == 3));
                    coordListLandMark.Add(new CoordByLine(0, -1, gcline.lastPos + Grbl.posWCO, gcline.actualPos + Grbl.posWCO, gcline.actualG, gcline.alpha, isArc));
                }
            }
            origWCOLandMark = (XyPoint)Grbl.posWCO;
        }
        /// <summary>
        /// translate background path with machine coordinates to take account of changed WCO
        /// </summary>
        public static void UpdatePathPositions()
        {
            Matrix matrix = new Matrix();
            matrix.Translate((float)(origWCOLandMark.X - Grbl.posWCO.X), (float)(origWCOLandMark.Y - Grbl.posWCO.Y));
            pathBackground.Transform(matrix);
            origWCOLandMark = (XyPoint)Grbl.posWCO;

            matrix.Reset();
            matrix.Translate((float)(origWCOMachineLimit.X - Grbl.posWCO.X), (float)(origWCOMachineLimit.Y - Grbl.posWCO.Y));
            pathMachineLimit.Transform(matrix);
            pathToolTable.Transform(matrix);
            origWCOMachineLimit = (XyPoint)Grbl.posWCO;
            matrix.Dispose();
        }

        private static void AddBackgroundText(GraphicsPath path, PointF pos, float emSize, string txt)//, bool vertical = false)
        {
            Logger.Info("AddBackgroundText x:{0} y:{1}  emSize:{2}  text:{3}", pos.X, pos.Y, emSize, txt);
            float centerX = (float)pos.X;// (float)((clipMax.X + clipMin.X) / 2);
            float centerY = (float)pos.Y;// (float)((clipMax.Y + clipMin.Y) / 2);

            try
            {
                System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
                matrix.Scale(1, -1);
                path.Transform(matrix);
                /*      if (vertical)
                      {
                          matrix = new System.Drawing.Drawing2D.Matrix(); 
                          matrix.Rotate(-90);
                          path.Transform(matrix);
                      }*/
                path.StartFigure();

                System.Drawing.FontFamily myFont = new System.Drawing.FontFamily("Arial");
                System.Drawing.StringFormat sFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault)
                {
                    Alignment = System.Drawing.StringAlignment.Near,
                    LineAlignment = System.Drawing.StringAlignment.Near
                };
                path.AddString(txt, myFont, (int)System.Drawing.FontStyle.Regular, emSize, new System.Drawing.PointF(centerX, -centerY), sFormat);

                /*	if (vertical)
                    {
                        matrix = new System.Drawing.Drawing2D.Matrix();
                        matrix.Rotate(90);
                        path.Transform(matrix);
                    }*/
                matrix = new System.Drawing.Drawing2D.Matrix();
                matrix.Scale(1, -1);
                path.Transform(matrix);

                myFont.Dispose();
                sFormat.Dispose();
            }
            catch (Exception err)
            { Logger.Error(err, "AddBackgroundText failed {0} ", txt); }
        }


        private static void AddArrow(GraphicsPath path, PathInfo pinfo, bool showArrow, bool showInfo)
        {
            PointF p2 = pinfo.position;
            double angleA = pinfo.angleArrow;
            double angleF = pinfo.angleFigure;
            string info = pinfo.info;
            double msize = (float)Math.Max(Math.Sqrt(xyzSize.dimx * xyzSize.dimx + xyzSize.dimy * xyzSize.dimy) / 80f, 0.5);
            float emSize = (float)Math.Max((msize * 1), 0.5);

            //            Logger.Trace("addArrow x:{0:0.00}  y:{1:0.00}   size:{2:0.00}", p2.X, p2.Y, msize);
            if (showArrow)
            {
                try
                {	// arrow at the end of the pen-up path
                    double aoff = Math.PI / 6;
                    float pointToX1 = (float)(p2.X - msize * Math.Cos(angleA + aoff));  // draw triangle
                    float pointToY1 = (float)(p2.Y - msize * Math.Sin(angleA + aoff));  // tip is end of pen-up path
                    float pointToX2 = (float)(p2.X - msize * Math.Cos(angleA - aoff));  // two additional points needed
                    float pointToY2 = (float)(p2.Y - msize * Math.Sin(angleA - aoff));

                    path.AddLine((float)p2.X, (float)p2.Y, pointToX1, pointToY1);
                    path.AddLine(pointToX1, pointToY1, pointToX2, pointToY2);
                    path.AddLine(pointToX2, pointToY2, (float)p2.X, (float)p2.Y);

                    // small arrow to show pen-down direction at 1st coordinate		
                    double startAngle = (angleA + angleF) / 2;//aoff; if (angleF > Math.PI/2) {startAngle = -aoff;}
                    float pointPdX0 = (float)(p2.X + msize * 0.5 * Math.Cos(startAngle));  	// start point
                    float pointPdY0 = (float)(p2.Y + msize * 0.5 * Math.Sin(startAngle));
                    float pointPdX1 = (float)(pointPdX0 + msize * 1 * Math.Cos(angleF));  			// end point
                    float pointPdY1 = (float)(pointPdY0 + msize * 1 * Math.Sin(angleF));
                    float pointPdX2 = (float)(pointPdX1 - msize * 0.5 * Math.Cos(angleF + aoff));  	// draw triangle
                    float pointPdY2 = (float)(pointPdY1 - msize * 0.5 * Math.Sin(angleF + aoff));  	// tip is end of pen-up path
                    float pointPdX3 = (float)(pointPdX1 - msize * 0.5 * Math.Cos(angleF - aoff));  	// two additional points needed
                    float pointPdY3 = (float)(pointPdY1 - msize * 0.5 * Math.Sin(angleF - aoff));

                    path.StartFigure();
                    path.AddLine(pointPdX0, pointPdY0, pointPdX1, pointPdY1);
                    path.AddLine(pointPdX1, pointPdY1, pointPdX2, pointPdY2);
                    path.AddLine(pointPdX2, pointPdY2, pointPdX3, pointPdY3);
                    path.AddLine(pointPdX3, pointPdY3, pointPdX1, pointPdY1);
                }
                catch (Exception err) { Logger.Error(err, "addArrow Addline - msize:{0}   ", msize); }
            }
            if (showInfo)
            {
                try
                {
                    float pointToX0 = (float)(p2.X + msize * Math.Cos(angleA));
                    float pointToY0 = (float)(p2.Y + msize * Math.Sin(angleA));
                    FontFamily family = new FontFamily("Lucida Console");
                    int fontStyle = (int)FontStyle.Italic;

                    PointF origin = new PointF((float)pointToX0, -(float)pointToY0);
                    StringFormat format = StringFormat.GenericDefault;

                    GraphicsPath tmpString = new GraphicsPath();
                    tmpString.AddString(info, family, fontStyle, emSize, origin, format);
                    RectangleF boundRect = tmpString.GetBounds();

                    Matrix translateMatrix = new Matrix();
                    translateMatrix.Translate(-boundRect.Width / 2, -boundRect.Height / 2);
                    tmpString.Transform(translateMatrix);
                    translateMatrix.Scale(1, -1);
                    tmpString.Transform(translateMatrix);
                    family.Dispose();
                    path.AddPath(tmpString, false);
                    tmpString.Dispose();
                    translateMatrix.Dispose();
                }
                catch (Exception err) { Logger.Error(err, "addArrow AddString - emSize:{0}   ", emSize); }
            }
        }


        // Add arrow (triangle) to given position with given angle (for pen-up path)
        private static void CreateMarkerArrow(GraphicsPath path, float msize, XyPoint pos, double angle, int type = 0)
        {
            float pointToX = (float)(pos.X + 2 * msize * Math.Cos(angle));		// direction to show
            float pointToY = (float)(pos.Y + 2 * msize * Math.Sin(angle));
            double aoff = Math.PI / 2;
            if (type > 0)
                angle -= Math.PI / 4;
            float pointToX1 = (float)(pos.X + msize * Math.Cos(angle + aoff));
            float pointToY1 = (float)(pos.Y + msize * Math.Sin(angle + aoff));
            float pointToX2 = (float)(pos.X + msize * Math.Cos(angle + 2 * aoff));
            float pointToY2 = (float)(pos.Y + msize * Math.Sin(angle + 2 * aoff));
            float pointToX3 = (float)(pos.X + msize * Math.Cos(angle + 3 * aoff));
            float pointToY3 = (float)(pos.Y + msize * Math.Sin(angle + 3 * aoff));
            float pointToX4 = (float)(pos.X + msize * Math.Cos(angle + 4 * aoff));
            float pointToY4 = (float)(pos.Y + msize * Math.Sin(angle + 4 * aoff));

            try
            {
                path.Reset();
                // draw outline
                path.StartFigure();
                path.AddLine(pointToX, pointToY, pointToX1, pointToY1);
                path.AddLine(pointToX1, pointToY1, pointToX2, pointToY2);
                path.AddLine(pointToX2, pointToY2, pointToX3, pointToY3);
                if (type > 0)
                {
                    path.AddLine(pointToX3, pointToY3, pointToX4, pointToY4);
                    path.AddLine(pointToX4, pointToY4, pointToX, pointToY);     // square
                }
                else
                    path.AddLine(pointToX3, pointToY3, pointToX, pointToY);     // rhombus
                path.CloseFigure();
                // draw diagonal cross in center
                float ssize = msize / 2;
                if (type > 0)
                {
                    path.StartFigure(); path.AddLine((float)pos.X - ssize, (float)pos.Y, (float)pos.X + ssize, (float)pos.Y);
                    path.StartFigure(); path.AddLine((float)pos.X, (float)pos.Y + ssize, (float)pos.X, (float)pos.Y - ssize);
                }
                else
                {
                    ssize = msize / 3;
                    path.StartFigure(); path.AddLine((float)pos.X - ssize, (float)pos.Y - ssize, (float)pos.X + ssize, (float)pos.Y + ssize);
                    path.StartFigure(); path.AddLine((float)pos.X - ssize, (float)pos.Y + ssize, (float)pos.X + ssize, (float)pos.Y - ssize);
                }
            }
            catch (Exception err)
            { Logger.Error(err, "CreateMarkerArrow failed "); }
        }


        internal static DrawingProperties SetRulerDimension(DrawingProperties dP, bool allowSmaller = false)
        {
            bool update = false;
            if (allowSmaller)
            {
                RulerDimension = dP;
                update = true;
            }
            else
            {
                if (dP.minX < RulerDimension.minX) { RulerDimension.minX = dP.minX; update = true; }
                if (dP.maxX > RulerDimension.maxX) { RulerDimension.maxX = dP.maxX; update = true; }
                if (dP.minY < RulerDimension.minY) { RulerDimension.minY = dP.minY; update = true; }
                if (dP.maxY > RulerDimension.maxY) { RulerDimension.maxY = dP.maxY; update = true; }
            }

            if (update)
                CreateRuler(pathRuler, RulerDimension);

            return RulerDimension;
        }


        private static readonly object lockObject = new object();

        // Add ruler with division
        internal static void CreateRuler(GraphicsPath path, DrawingProperties dP, bool finest = false)
        {
            if (path == null)
            { Logger.Error("CreateRuler, path=null"); return; }

            //    Logger.Info("CreateRuler minX:{0}  maxX:{1}",dP.minY, dP.maxX);

            path.Reset();
            pathGrid1.Reset();
            pathGrid10.Reset();
            pathGrid100.Reset();
            pathGrid1000.Reset();
            pathGrid10000.Reset();
            float unit = 1;
            int divider = 1;
            int divider_long = 100;
            int divider_med = 10;
            int divider_short = 5;
            int show_short = 500;
            int show_smallest = 200;
            float length1 = 1F, length2 = 2F, length3 = 3F, length5 = 5F;
            if (!Properties.Settings.Default.importUnitmm || (modal.unitsMode == 20))
            {
                divider = 16;
                divider_long = divider;
                divider_med = 8;
                divider_short = 4;
                show_short = 20 * divider;
                show_smallest = 6 * divider;
                dP.minX *= divider; // unit;
                dP.maxX *= divider; // unit;
                dP.minY *= divider; // unit;
                dP.maxY *= divider; // unit;
                length1 = 0.05F; length2 = 0.1F; length3 = 0.15F; length5 = 0.25F;
            }
            double rangeX = dP.maxX - dP.minX;
            double rangeY = dP.maxY - dP.minY;
            if (finest)
            { rangeX = rangeY = 1; }

            float x, y;
            for (float i = dP.minX; i < dP.maxX; i++)          // horizontal ruler
            {
                lock (lockObject)
                {
                    path.StartFigure();
                }
                x = (float)i * unit / (float)divider;
                if (i % divider_short == 0)
                {
                    if (i % 10000 == 0)
                    {
                        path.AddLine(x, 0, x, -length5);        // 1000
                        pathGrid10000.AddLine(x, dP.minY - 2000, x, dP.maxY + 2000);
                        pathGrid10000.StartFigure();
                    }
                    else if (i % 1000 == 0)
                    {
                        path.AddLine(x, 0, x, -length5);        // 1000
                        pathGrid1000.AddLine(x, dP.minY - 500, x, dP.maxY + 500);
                        pathGrid1000.StartFigure();
                    }
                    else if (i % divider_long == 0)
                    {
                        path.AddLine(x, 0, x, -length5);        // 100 
                        pathGrid100.AddLine(x, dP.minY, x, dP.maxY);
                        pathGrid100.StartFigure();
                    }
                    else if ((i % divider_med == 0) && (rangeX < (2 * show_short)))
                    {
                        path.AddLine(x, 0, x, -length3);    // 10   
                        pathGrid10.AddLine(x, dP.minY, x, dP.maxY);
                        pathGrid10.StartFigure();
                    }
                    else if (rangeX < show_short)
                    { path.AddLine(x, 0, x, -length2); }    // 5
                }
                else if (dP.maxX < show_smallest)
                {
                    path.AddLine(x, 0, x, -length1);        // 1
                }
                pathGrid1.AddLine(x, dP.minY, x, dP.maxY);
                pathGrid1.StartFigure();

            }
            for (float i = dP.minY; i < dP.maxY; i++)          // vertical ruler
            {
                lock (lockObject)
                {
                    path.StartFigure();
                }
                y = (float)i * unit / (float)divider;
                if (i % divider_short == 0)
                {
                    if (i % 10000 == 0)
                    {
                        path.AddLine(0, y, -length5, y);    // 100
                        pathGrid10000.AddLine(dP.minX - 2000, y, dP.maxX + 2000, y);
                        pathGrid10000.StartFigure();
                    }
                    else if (i % 1000 == 0)
                    {
                        path.AddLine(0, y, -length5, y);    // 100
                        pathGrid1000.AddLine(dP.minX - 500, y, dP.maxX + 500, y);
                        pathGrid1000.StartFigure();
                    }
                    else if (i % divider_long == 0)
                    {
                        path.AddLine(0, y, -length5, y);    // 100
                        pathGrid100.AddLine(dP.minX, y, dP.maxX, y);
                        pathGrid100.StartFigure();
                    }
                    else if ((i % divider_med == 0) && (rangeY < (2 * show_short)))
                    {
                        path.AddLine(0, y, -length3, y);    // 10           
                        pathGrid10.AddLine(dP.minX, y, dP.maxX, y);
                        pathGrid10.StartFigure();
                    }
                    else if (rangeY < show_short)
                    { path.AddLine(0, y, -length2, y); }    // 5
                }
                else if (dP.maxY < show_smallest)
                {
                    path.AddLine(0, y, -length1, y);        // 1
                }
                pathGrid1.AddLine(dP.minX, y, dP.maxX, y);
                pathGrid1.StartFigure();
            }
        }

        // Add marker (cross, box, circle) to path
        private static void CreateMarker(GraphicsPath path, XyPoint center, float dimension, int style, bool rst = true)
        { CreateMarker(path, (float)center.X, (float)center.Y, dimension, style, rst); }
        private static readonly object pathDrawLock = new object();
        private static void CreateMarker(GraphicsPath path, float centerX, float centerY, float dimension, int style, bool rst = true)
        {
            if (dimension == 0) { return; }
            lock (pathDrawLock)
            {
                if (rst)
                    path.Reset();
                if (style == 0)   // horizontal cross
                {
                    path.StartFigure(); path.AddLine(centerX, centerY + dimension, centerX, centerY - dimension);
                    path.StartFigure(); path.AddLine(centerX + dimension, centerY, centerX - dimension, centerY);
                }
                else if (style == 1)   // diagonal cross
                {
                    path.StartFigure(); path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY - dimension);
                    path.StartFigure(); path.AddLine(centerX - dimension, centerY - dimension, centerX + dimension, centerY + dimension);
                }
                else if (style == 2)            // box
                {
                    path.StartFigure();
                    path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY + dimension);
                    path.AddLine(centerX + dimension, centerY + dimension, centerX + dimension, centerY - dimension);

                    path.AddLine(centerX + dimension, centerY - dimension, centerX, centerY);
                    path.AddLine(centerX, centerY, centerX - dimension, centerY - dimension);

                    path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY - dimension);
                    path.AddLine(centerX - dimension, centerY - dimension, centerX - dimension, centerY + dimension);
                    path.CloseFigure();
                }
                else if (style == 3)            // marker
                {
                    path.StartFigure();
                    path.AddLine(centerX, centerY, centerX, centerY - dimension);
                    path.AddLine(centerX, centerY - dimension, centerX + dimension, centerY);
                    path.AddLine(centerX + dimension, centerY, centerX, centerY + dimension);
                    path.AddLine(centerX, centerY + dimension, centerX - dimension, centerY);
                    path.AddLine(centerX - dimension, centerY, centerX, centerY - dimension);
                    path.CloseFigure();
                }
                else if (style == 4)  	    	// circle
                {
                    path.StartFigure(); path.AddArc(centerX - dimension, centerY - dimension, 2 * dimension, 2 * dimension, 0, 360);
                }
                else							// rect
                {
                    path.StartFigure();
                    path.AddLine(centerX - dimension, centerY + dimension, centerX + dimension, centerY + dimension);
                    path.AddLine(centerX + dimension, centerY + dimension, centerX + dimension, centerY - dimension);

                    path.AddLine(centerX + dimension, centerY - dimension, centerX - dimension, centerY - dimension);
                    path.AddLine(centerX - dimension, centerY - dimension, centerX - dimension, centerY + dimension);
                    path.CloseFigure();
                }
            }
        }

        private static void MarkPath(GraphicsPath path, float x, float y, int type)
        {
            float markerSize = 1;
            if (!Properties.Settings.Default.importUnitmm || (modal.unitsMode == 20))
            { markerSize /= 25.4F; }
            CreateMarker(path, x, y, markerSize, type, false);    // draw circle
        }

        // setup drawing area 
        public static void CalcDrawingArea(float markerSize = -1)
        {
            double extend = 1.01;                                                       // extend dimension a little bit
            double roundTo = 5;                                                         // round-up dimensions
            if (!Properties.Settings.Default.importUnitmm || (modal.unitsMode == 20))
            { roundTo = 0.25; }

            if ((xyzSize.dimx == 0) && (xyzSize.dimy == 0))
            {
                xyzSize.SetDimensionXY(G0Size.minx, G0Size.miny);
                xyzSize.SetDimensionXY(G0Size.maxx, G0Size.maxy);
                Logger.Info("xyz-Dimension=0, use G0-Dimension dim-x {0} dim-y {1}", G0Size.dimx, G0Size.dimy);
            }

            if ((xyzSize.miny == 0) && (xyzSize.maxy == 0))
            { xyzSize.miny = -1; xyzSize.maxy = 1; }

            drawingSize.minX = (float)(Math.Floor(xyzSize.minx * extend / roundTo) * roundTo);                  // extend dimensions
            if (drawingSize.minX >= 0) { drawingSize.minX = (float)-roundTo; }                                          // be sure to show 0;0 position
            drawingSize.maxX = (float)(Math.Ceiling(xyzSize.maxx * extend / roundTo) * roundTo);
            drawingSize.minY = (float)(Math.Floor(xyzSize.miny * extend / roundTo) * roundTo);
            if (drawingSize.minY >= 0) { drawingSize.minY = (float)-roundTo; }
            drawingSize.maxY = (float)(Math.Ceiling(xyzSize.maxy * extend / roundTo) * roundTo);

            //           createRuler(pathRuler, drawingSize.minX, drawingSize.maxX, drawingSize.minY, drawingSize.maxY);
            //CreateRuler(pathRuler, drawingSize);
            SetRulerDimension(drawingSize, true);

            if (markerSize <= 0)
                markerSize = (float)((double)Properties.Settings.Default.gui2DSizeTool / (500 / xyzSize.dimy));
            CreateMarkerPath(markerSize);

            CreateDimensionBox();
        }

        public static void CreateDimensionBox()
        {
            pathDimension.Reset();
            pathDimension.StartFigure();
            pathDimension.AddLine((float)xyzSize.minx, (float)xyzSize.miny, (float)xyzSize.minx, (float)xyzSize.maxy);
            pathDimension.AddLine((float)xyzSize.minx, (float)xyzSize.maxy, (float)xyzSize.maxx, (float)xyzSize.maxy);
            pathDimension.AddLine((float)xyzSize.maxx, (float)xyzSize.maxy, (float)xyzSize.maxx, (float)xyzSize.miny);
            pathDimension.CloseFigure();
        }


        public static float MarkerSize = 10;
        public static void CreateMarkerPath()
        { CreateMarkerPath(MarkerSize); }
        public static void CreateMarkerPath(float size)
        {
            if (size > 0)
                MarkerSize = size;
            else
                MarkerSize = (float)Math.Sqrt(xyzSize.dimx * xyzSize.dimx + xyzSize.dimy * xyzSize.dimy) / 40f;
            CreateMarkerPath(false, new XyPoint(0, 0));
        }

        internal static void CreateMarkerPath(bool showCenter, XyPoint center)
        { CreateMarkerPath(showCenter, center, center); }
        internal static void CreateMarkerPath(bool showCenter, XyPoint center, XyPoint last)
        {
            //float MarkerSize = (float)Math.Sqrt(xyzSize.dimx * xyzSize.dimx + xyzSize.dimy * xyzSize.dimy) / 40f;
            MarkerSize = Math.Max(MarkerSize, 0.2f);
            //            createMarker(pathTool,   (xyPoint)grbl.posWork, msize, 2);

            if (tangentialAxisEnable)
            {
                double posAngle = 0;
                double factor = Math.PI / ((double)Properties.Settings.Default.importGCTangentialTurn / 2);
                if (tangentialAxisName == "C") { posAngle = factor * Grbl.posWork.C; }
                else if (tangentialAxisName == "B") { posAngle = factor * Grbl.posWork.B; }
                else if (tangentialAxisName == "A") { posAngle = factor * Grbl.posWork.A; }
                else if (tangentialAxisName == "Z") { posAngle = factor * Grbl.posWork.Z; }
                CreateMarkerArrow(pathTool, MarkerSize, (XyPoint)Grbl.posWork, posAngle, 1);
                CreateMarkerArrow(pathMarker, MarkerSize, (XyPoint)Grbl.PosMarker, Grbl.PosMarkerAngle * 360 / (double)Properties.Settings.Default.importGCTangentialTurn);
            }
            else
            {
                CreateMarker(pathTool, (XyPoint)Grbl.posWork, MarkerSize, 2);
                CreateMarker(pathMarker, (XyPoint)Grbl.PosMarker, MarkerSize, 3);
            }

            if (showCenter)
            {
                CreateMarker(pathMarker, center, MarkerSize, 0, false);
                pathMarker.StartFigure(); pathMarker.AddLine((float)last.X, (float)last.Y, (float)center.X, (float)center.Y);
                pathMarker.StartFigure(); pathMarker.AddLine((float)center.X, (float)center.Y, (float)Grbl.PosMarker.X, (float)Grbl.PosMarker.Y);
            }
            if (Properties.Settings.Default.ctrl4thUse)
            {
                float scale = (float)Properties.Settings.Default.rotarySubstitutionDiameter * (float)Math.PI / 360;
                if (Properties.Settings.Default.ctrl4thInvert)
                    scale *= -1;

                if (Properties.Settings.Default.ctrl4thOverX)
                {
                    CreateMarker(pathTool, (float)Grbl.posWork.X, (float)Grbl.posWork.A * scale, MarkerSize, 2);
                }
                else
                {
                    CreateMarker(pathTool, (float)Grbl.posWork.A * scale, (float)Grbl.posWork.Y, MarkerSize, 2);
                }
            }
        }


    }
}
