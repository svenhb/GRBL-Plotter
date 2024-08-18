/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2024-06-06 new form for wire cutter 
*/

using NLog;
using System;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using static GrblPlotter.Graphic;

namespace GrblPlotter
{
    public partial class GCodeForWireCutter : Form
    {

        public GraphicsPath PathBackground = new GraphicsPath();
        public GraphicsPath PathTemplate = new GraphicsPath();
        private static Point current = new Point(0, 0);

        double gapX = 5, gapY = 5;
        int arrayX = 3, arrayY = 3;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public GCodeForWireCutter()
        {
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Logger.Info("++++++ GCodeForWireCutter START ++++++");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            PathBackground.Reset();

            double toolD = (double)NudToolDiameter.Value;
            double toolR = toolD / 2;
            double ri = (double)NudCRingIn.Value / 2;
            double ra = (double)NudCRingOut.Value / 2;
            double angleRad = 2 * Math.PI / (double)NudCRingSegments.Value;

            arrayX = (int)NudArrayX.Value;
            arrayY = (int)NudArrayY.Value;

            gapX = (double)NudArrayGapX.Value;
            gapY = (double)NudArrayGapY.Value;// toolD);			

            if (ra < (ri + toolR + gapX))
            {
                ri = ra - (toolR + gapX);
                if (ri > 0)
                    NudCRingIn.Value = (decimal)ri * 2;
            }

            WCS.Init((double)NudToolDiameter.Value, (double)NudArrayGapX.Value);
            WCS.SetCircleRing((double)NudCRingIn.Value / 2, (double)NudCRingOut.Value / 2, 2 * Math.PI / (double)NudCRingSegments.Value, (int)NudCRingSegments.Value);

            Graphic.Init(Graphic.SourceType.Wire, "", null, null);
            Graphic.graphicInformation.ResetOptions(true);
            Graphic.graphicInformation.GroupEnable = false;
            Graphic.graphicInformation.OptionCodeSortDistance = false;
            Graphic.graphicInformation.OptionCodeSortDimension = false;
            Graphic.graphicInformation.OptionCodeOffset = Properties.Settings.Default.importGraphicOffsetOrigin;

            Graphic.SetHeaderInfo(string.Format(" Wire cutter "));
            Graphic.SetGeometry("Line");
            Graphic.SetPenColor("blue");
            PathBackground.Reset();
            PathTemplate.Reset();

            double riChord = 0;// CreateCircularRing(templateSize, ri, ra, angleRad, toolR);    // shape

            Point templateSize = new Point();

            double h1 = GetSegmentHeight(riChord, ri - toolR, ra + toolR, angleRad);
            templateSize.X -= h1;

            Point start = new Point(WCS.Dimension.X, -toolR - gapY);
            Graphic.StartPath(new Point(WCS.Dimension.X - WCS.OffsetStartX - toolR - gapX, 0));
            //  Graphic.AddLine(start); 
            current = start;
            Point currentOld = current;

            Logger.Info("++++++ Create ring-segment angle:{0:0.00}   ri:{1:0.00}  ra:{2:0.00}  toolD:{3:0.00}  gapX:{4:0.00}   gapY:{5:0.00}", angleRad * 180 / Math.PI, ri, ra, toolD, gapX, gapY);

            //   bool shrink = true;
            Point lastPathEnd = new Point();
            ;
            int dir = 1;
            int indexStartDir1 = 2;
            int indexEndDir1 = 2;
            int indexStartDir2 = 0;
            int indexEndDir2 = 1;

            for (int iy = 0; iy < arrayY; iy++)
            {
                for (int ix = 0; ix < arrayX; ix++)
                {
                    currentOld = current;
                    if (iy > 0)//(ix >= arrayX - 1)
                        indexEndDir1 = 1;
                    if (dir > 0)
                    {
                        lastPathEnd = Graphic.AddGraphicsItemPath(WCS.toolPath1, current, indexStartDir1, indexEndDir1);
                    }
                    else
                    {
                        lastPathEnd = Graphic.AddGraphicsItemPath(WCS.toolPath2, current, indexStartDir2, indexEndDir2);
                        indexStartDir2 = 0;
                    }
                    current.X += dir * (WCS.Dimension.X + WCS.Stack.X);// - toolR;
                    Logger.Trace("Offset X  DimX:{0}  StackX:{1}  Offset.X:{2}", WCS.Dimension.X, WCS.Stack.X, WCS.Offset.X);

                    if (iy > 0)
                        indexStartDir1 = 0;
                }
                indexStartDir1 = 0;
                if (dir > 0)
                {
                    if (iy < arrayY - 1)
                    {
                        Graphic.AddLine(AddPoints(ref currentOld, WCS.FeedPoint[3]));
                        Graphic.AddLine(AddPoints(ref currentOld, WCS.FeedPoint[4]));
                        if (!WCS.ShiftPatternRight)
                            Graphic.AddLine(AddPoints(ref currentOld, WCS.FeedPoint[5]));
                    }

                    //   current.X += -WCS.Dimension.X + WCS.Offset.X;
                    current.X = lastPathEnd.X - WCS.Dimension.X - WCS.Offset.X;
                    if (WCS.ShiftPatternRight)
                    {
                        current.X += WCS.Dimension.X;
                        indexStartDir2 = 2;
                    }
                    else if (WCS.ShiftPatternLeft)
                    {
                        current.X -= (WCS.Dimension.X + WCS.Stack.X);
                    }
                    Logger.Trace("Offset X2  DimX:{0}  StackX:{1}  Offset.X:{2}", WCS.Dimension.X, WCS.Stack.X, WCS.Offset.X);
                }
                else
                {
                    if (iy < arrayY - 1)
                    {
                        Graphic.AddLine(currentOld.X - WCS.FeedPoint[3].X, currentOld.Y + WCS.FeedPoint[3].Y);
                        Graphic.AddLine(currentOld.X - WCS.FeedPoint[4].X, currentOld.Y + WCS.FeedPoint[4].Y);
                        if (!WCS.ShiftPatternRight)
                            Graphic.AddLine(currentOld.X - WCS.FeedPoint[5].X, currentOld.Y + WCS.FeedPoint[5].Y);
                    }
                    if (WCS.ShiftPatternRight)
                    {
                        indexStartDir1 = 2;
                    }
                    current.X = start.X;
                }

                if (iy < arrayY)
                {
                    current.Y += WCS.Offset.Y;
                    //    current.Y -= WCS.Dimension.Y - WCS.Stack.Y - WCS.Clearance.Y;
                    Logger.Trace("Offset Y  DimY:{0}  StackY:{1}  Offset.X:{2}", WCS.Dimension.Y, WCS.Stack.Y, WCS.Offset.X);
                }
                dir *= -1;
            }

            Graphic.StopPath();
            Graphic.CreateGCode();
        }

        private double GetSegmentHeight(double s1, double ri, double ra, double angleRad)
        {   // https://de.wikipedia.org/wiki/Kreissegment
            // https://en.wikipedia.org/wiki/Circular_segment
            //    double h1 = ri * (1 - Math.Cos(angleRad / 2));
            if (angleRad >= Math.PI)
            {
                s1 = 2 * ri;
                return ra - 0.5 * Math.Sqrt(4 * ra * ra - s1 * s1);
            }

            double h2 = ra - 0.5 * Math.Sqrt(4 * ra * ra - s1 * s1);
            return h2;
        }

        /*    private Point DrawGraphicsPath(GraphicsPath path, double cx, double cy, double ri, double ra, double angleRad, bool rightSide)
            {
                double angleStart = 90;
                if (angleRad < Math.PI)
                    angleStart = 90 + ((Math.PI - angleRad) * 90 / Math.PI);

                if (rightSide)
                    angleStart += 180;

                cx += ra;
                path.StartFigure();
                System.Drawing.PointF arcPos = new System.Drawing.PointF((float)(cx - ra), (float)(cy - ra));
                System.Drawing.PointF arcSize = new System.Drawing.PointF((float)ra * 2, (float)ra * 2);
                Point sizeCalc = new Point((ra - ri) + ri * (1 - Math.Cos(angleRad / 2)), 2 * ra * Math.Sin(angleRad / 2));
                if (ra > 0)
                    path.AddArc(arcPos.X, arcPos.Y, arcSize.X, arcSize.Y, (float)angleStart, (float)(angleRad * 180 / Math.PI));

                if (ri > 0)
                {
                    arcPos = new System.Drawing.PointF((float)(cx - ri), (float)(cy - ri));
                    arcSize = new System.Drawing.PointF((float)ri * 2, (float)ri * 2);
                    path.AddArc(arcPos.X, arcPos.Y, arcSize.X, arcSize.Y, (float)(angleRad * 180 / Math.PI + angleStart), -(float)(angleRad * 180 / Math.PI));
                }
                else
                {
                    path.AddLine((float)cx, (float)(cy), (float)cx, (float)(cy + ra));
                }
                path.CloseFigure();
                return sizeCalc;
            }*/

        /*    private static Point GetDistancePoint(Point start, double addR, double angleRad)
            {
                return new Point(start.X + addR * Math.Cos(angleRad), start.Y + addR * Math.Sin(angleRad));
            }*/

        /*    private static Point GetIntermediatePoint(Point start, Point end, double distance, bool extend = false)
            {
                double dx = end.X - start.X;
                double dy = end.Y - start.Y;
                double moveLength = (float)Math.Sqrt(dx * dx + dy * dy);
                if (moveLength == 0)
                    return end;
                if (extend)
                {
                    double newx = end.X + distance * dx / moveLength;
                    double newy = end.Y + distance * dy / moveLength;
                    return new Point(newx, newy);
                }
                else
                {
                    double newx = start.X + distance * dx / moveLength;
                    double newy = start.Y + distance * dy / moveLength;
                    return new Point(newx, newy);
                }
            }*/

        private static Point AddPoints(ref Point a, Point b)
        { return new Point(a.X + b.X, a.Y + b.Y); }

    }

    internal static class WCS
    {
        /* 	Generate the tool path for a circle ring segment 
			Shape path: the resulting work pice					4 points: outer-, inner-edges
			Tool path: Shape path + tool radius offset			8 points: outer-, inner-edges + tool radius extension
			Feed path: Safety distance to tool path (Clearance) 5 points 
		*/

        internal static Point Origin = new Point();		// start point of shape
        internal static Point Start = new Point();		// start point lead in
        internal static Point End = new Point();        // start point lead out
        internal static Point Stack = new Point();      // possible offset to stack shapes
        internal static PathObject toolPath1 = new ItemPath();
        internal static PathObject toolPath2 = new ItemPath();
        internal static Point Dimension = new Point();   // size of shape
        internal static Point Offset = new Point();      // possible offset to stack shapes
        internal static double OffsetStartX = 0;
        internal static bool ShiftPatternRight = false;
        internal static bool ShiftPatternLeft = false;

        internal static Point[] ShapePoint = new Point[4] { new Point(), new Point(), new Point(), new Point() };
        internal static Point[] ToolPoint = new Point[8] { new Point(), new Point(), new Point(), new Point(), new Point(), new Point(), new Point(), new Point() };
        internal static Point[] FeedPoint = new Point[6] { new Point(), new Point(), new Point(), new Point(), new Point(), new Point() };

        internal static Point Clearance = new Point();   // distance lead-path to shape-path
        private static double toolR = 1;
        private static bool arcToLine = Properties.Settings.Default.importGCNoArcs;// false;

        private static Dimensions tempDim = new Dimensions();

        // circular ring segment
        private static double crsRi = 1, crsRa = 2, crsAngle = Math.PI;
        private static int crsSegments = 2;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Init(double toolDiameter, double gapX)
        {
            toolR = toolDiameter / 2;
            Clearance.X = gapX;
            Clearance.Y = gapX;
        }

        public static void SetCircleRing(double radiusIn, double radiusOut, double angleRad, int segments)
        {
            crsRi = radiusIn;
            crsRa = radiusOut;
            crsAngle = angleRad;
            crsSegments = segments;
            toolPath1 = new ItemPath();
            ShiftPatternRight = false;
            ShiftPatternLeft = false;
            CreateCircularRing(Origin, crsRi, crsRa, crsAngle, toolR);
        }

        private static void CreateCircularRing(Point shapeStart, double ri, double ra, double angleRad, double toolR)
        {
            if (ri < toolR)
                ri = toolR;
            if (ri == 0)
                ri = 0.1;
            // moveTo edge1, arcTo edge2, moveTo edge3, arcTo edge 4
            Point toolStart = new Point(shapeStart.X + toolR, shapeStart.Y + toolR);
            FeedPoint[0] = new Point(shapeStart.X, toolStart.Y + Clearance.Y);
            Point center = new Point(shapeStart.X, shapeStart.Y - ra);
            double toolDeltaR = ra - ri + 2 * toolR;
            double shapeDeltaR = ra - ri;
            double angleEnd = angleRad + Math.PI / 2;
            Point arcToRa = new Point(center.X + (ra + toolR) * Math.Cos(angleEnd), center.Y + (ra + toolR) * Math.Sin(angleEnd));
            double rotateAngle = (Math.PI - angleRad) / 2;
            Point rotateCenter = FeedPoint[0];
            FeedPoint[1] = rotateCenter;

            Logger.Trace("CreateCircularRing  toolR:{0}   shapeStart:{1}  segmentAngle:{2:0.00}  rotateAngle:{3:0.00}", toolR, shapeStart, angleRad * 180 / Math.PI, rotateAngle * 180 / Math.PI);

            /* Dimension */
            tempDim.ResetDimension();
            double angleDegDelta = angleRad * 180 / Math.PI;
            double angleDegStart = 90 + (180 - angleDegDelta) / 2;
            tempDim.SetDimensionCircle(center.X, center.Y, ra + toolR, angleDegStart, angleDegDelta);
            OffsetStartX = tempDim.dimx - toolR - Clearance.X;
            double dimRaOnly = tempDim.dimx + toolR * Math.Cos(rotateAngle);
            tempDim.SetDimensionCircle(center.X, center.Y, ri - toolR, angleDegStart, angleDegDelta);
            Dimension.X = tempDim.dimx + toolR * Math.Cos(rotateAngle);
            if ((ri == 0) || (angleRad == Math.PI))
            { Dimension.X = ra + 2 * toolR; }

            Dimension.Y = 2 * (ra + toolR) * Math.Sin(angleRad / 2);

            double rRatio = ri / ra;
            double feedPoint2Factor = 0.97;

            if (ri == ra)               // go same way back
            {
                Stack.X = Clearance.X - ri + toolR;
                Stack.Y = Clearance.Y;
            }
            else                        // circular ring segment
            {
                double segmentLength = 2 * (ri - toolR) * Math.Sin(angleRad / 2);
                double raExtend = ra + toolR + Clearance.X;
                double segmentHeight = raExtend - 0.5 * Math.Sqrt(4 * raExtend * raExtend - segmentLength * segmentLength);

                Stack.X = -segmentHeight + Clearance.X; ;
                double c = 2 * raExtend;    // distance center to center
                double b = 2 * raExtend - dimRaOnly - segmentHeight - toolR - 2 * Clearance.X;// distance horizontal

                Stack.Y = Dimension.Y - Math.Sqrt(c * c - b * b) - Clearance.X * Math.Cos(rotateAngle);
                Offset.X = Stack.X - Clearance.X - toolR;        // ##################################

                double deltaRatio = (ra - ri + toolR + Clearance.X) / (ra + 2 * toolR);

                if (crsSegments == 2)
                {
                    ShiftPatternLeft = rRatio > 0.78;
                    feedPoint2Factor = 0.3 + rRatio;// 0.97;
                    if (feedPoint2Factor > 0.6)
                        feedPoint2Factor = 0.6;
                }

                if (crsSegments == 3)
                {
                    Offset.X += deltaRatio * 7.5;// + ra * 0.0005;
                    Stack.Y += deltaRatio * 8.5 - ra * 0.005;
                    ShiftPatternRight = rRatio < 0.25;
                }
                else if (crsSegments == 4)
                {
                    Offset.X += toolR + Clearance.X + deltaRatio * 8 + ra * 0.0005;
                    Stack.Y += deltaRatio * 4 - ra * 0.01;
                    ShiftPatternRight = rRatio < 0.6;
                }

                else if (crsSegments == 5)
                {
                    Offset.X += toolR + Clearance.X + deltaRatio * 4.5 + ra * 0.001;
                    Stack.Y += deltaRatio * 8.5 - ra * 0.002;
                    ShiftPatternRight = rRatio < 0.78;
                }
                else if (crsSegments == 6)
                {
                    Offset.X += toolR + Clearance.X + deltaRatio * 3.5;
                    Stack.Y += deltaRatio * 8.5 - ra * 0.002;
                    ShiftPatternRight = rRatio < 0.78;
                }
                else if (crsSegments == 7)
                {
                    Offset.X += toolR + Clearance.X + deltaRatio * 2.5;
                    Stack.Y += deltaRatio * 8 - ra * 0.002;
                    ShiftPatternRight = rRatio < 0.78;
                }
                else if (crsSegments == 8)
                {
                    Offset.X += toolR + Clearance.X + deltaRatio * 2;
                    Stack.Y += deltaRatio * 7 - ra * 0.002;
                    ShiftPatternRight = rRatio < 0.78;
                }
                else if (crsSegments == 9)
                {
                    Offset.X += toolR + Clearance.X + deltaRatio * 1.6;
                    Stack.Y += deltaRatio * 6.5 - ra * 0.002;
                    ShiftPatternRight = rRatio < 0.78;
                }
                else if (crsSegments >= 10)
                {
                    Offset.X += toolR + Clearance.X + deltaRatio * 1.2;
                    Stack.Y += deltaRatio * 5.5 - ra * 0.002;
                    ShiftPatternRight = rRatio < 0.78;
                }

                Offset.Y = -Dimension.Y + Stack.Y;


            }

            //lead in rotate by rotateCenter (not center)
            double leadInR = ra + toolR + Clearance.X;
            FeedPoint[0].Y = -Stack.Y - Clearance.Y;
            double dY = Math.Abs(FeedPoint[0].Y - center.Y);
            double q = dY / leadInR;
            if ((q < -1) || (q > 1))
            {
                Logger.Error("Lead in q nok  dY:{0}  leadInR:{1}", dY, leadInR);
                q = 1;
            }

            double leadInAngle = Math.Asin(q);
            //    Logger.Trace("Lead in  dY:{0}   feedStart.Y:{1}   center.Y:{2}  leadInR:{3}  dY / leadInR:{4} leadInAngle:{5}", dY, FeedPoint[0].Y, center.Y, leadInR, dY / leadInR, leadInAngle * 180 / Math.PI);

            FeedPoint[0].X = center.X - leadInR * Math.Cos(leadInAngle);
            //    Logger.Trace("Lead in  y:{0}   dy:{1}   stack:{2}  angle:{3}  centerx:{4}  x:{5}", FeedPoint[0].Y, dY, Stack, leadInAngle * 180 / Math.PI, center.X, FeedPoint[0].X);

            ToolStart(toolPath1, FeedPoint[0]);
            ToolMoveTo(toolPath1, FeedPoint[0]);   // index 0
            ToolArcTo(toolPath1, FeedPoint[1].X, FeedPoint[1].Y, center.X - FeedPoint[0].X, -dY, true);

            // edge 1
            ToolMoveTo(toolPath1, FeedPoint[1]);
            ToolMoveTo(toolPath1, toolStart);
            Logger.Trace("mt  x:{0}   y:{1}", toolStart.X - toolR, toolStart.Y);
            ToolMoveTo(toolPath1, toolStart.X - toolR, toolStart.Y);
            // edge 2
            ToolArcTo(toolPath1, arcToRa.X, arcToRa.Y, 0, -(ra + toolR), false);
            Point extendOuter = GetDistancePoint(arcToRa, toolR, angleEnd + Math.PI / 2);
            ToolMoveTo(toolPath1, extendOuter);                                           // extend by tool radius

            if (ri == ra)               // go same way back
            {
                ToolArcTo(toolPath1, shapeStart.X, shapeStart.Y + toolR, center.X - arcToRa.X, center.Y - arcToRa.Y, true);
            }
            else if (ri <= 0)           // circle segment
            {
                Point centerOffset = GetDistancePoint(center, toolR, angleEnd + Math.PI / 2);   // 
                Point centerExtend = GetIntermediatePoint(extendOuter, centerOffset, toolR, true);
                FeedPoint[4] = new Point(center.X - Clearance.X, center.Y - Clearance.Y);
                if (centerExtend.X > (center.X + toolR)) { centerExtend.X = center.X + toolR; }
                ToolMoveTo(toolPath1, centerExtend);  // extend behind center
                ToolMoveTo(toolPath1, center.X + toolR, center.Y);        // center				
                ToolMoveTo(toolPath1, toolStart.X, toolStart.Y);      // up
            }
            else                        // circular ring segment
            {
                Point startRi = GetIntermediatePoint(arcToRa, center, toolDeltaR);
                Point startRiOffset = GetDistancePoint(startRi, toolR, angleEnd + Math.PI / 2);

                FeedPoint[4] = GetDistancePoint(startRiOffset, Clearance.X, angleEnd + Math.PI / 2);            // #####################################
                // edge 3
                ToolMoveTo(toolPath1, startRiOffset); // extend towards center
                ToolMoveTo(toolPath1, startRi);
                //    Point c1 = startRi;
                // edge 4
                ToolArcTo(toolPath1, shapeStart.X, shapeStart.Y - shapeDeltaR - toolR, center.X - startRi.X, center.Y - startRi.Y, true);    // draw inner segment
                Point c2 = new Point(shapeStart.X + toolR, shapeStart.Y - shapeDeltaR - toolR);
                ToolMoveTo(toolPath1, c2);
                ToolMoveTo(toolPath1, toolStart.X, toolStart.Y);
                FeedPoint[3] = new Point(c2.X + Clearance.X, c2.Y - Clearance.Y);

            }
            /* lead out */
            ToolMoveTo(toolPath1, toolStart.X + Clearance.X, toolStart.Y);

            Point feedEnd = new Point(toolStart.X + Clearance.X, toolStart.Y);
            FeedPoint[2] = GetIntermediatePoint(feedEnd, new Point(center.X + +toolR + Clearance.X, center.Y), shapeDeltaR * feedPoint2Factor - toolR - Clearance.X);

            FeedPoint[5] = GetDistancePoint(extendOuter, (toolR + Clearance.X), angleEnd + Math.PI / 4);      // #####################################

            ToolMoveTo(toolPath1, FeedPoint[2]);
            ToolMoveTo(toolPath1, FeedPoint[3]);

            if (angleRad < Math.PI)
            {
                RotatePath(toolPath1, rotateAngle, rotateCenter);
            }

            // fix lead in start and arc
            center = RotatePoint(center, rotateAngle, rotateCenter);
            dY = Math.Abs(FeedPoint[0].Y - center.Y);
            q = dY / leadInR;
            if ((q < -1) || (q > 1))
                Logger.Error("Lead in q nok  dY:{0}  leadInR:{1}", dY, leadInR);

            leadInAngle = Math.Asin(q);

            FeedPoint[0].X = center.X - leadInR * Math.Cos(leadInAngle);

            var tmp0 = ((ItemPath)toolPath1).Path[0];
            tmp0.MoveTo = FeedPoint[0];
            if (((ItemPath)toolPath1).Path[1] is GCodeArc tmp1)
                tmp1.CenterIJ = new Point(center.X - FeedPoint[0].X, center.Y - FeedPoint[0].Y);


            RotateArray(FeedPoint, rotateAngle, rotateCenter);

            Start = FeedPoint[0];  // RotatePoint(feedStart, rotateAngle, rotateCenter);
            End = RotatePoint(feedEnd, rotateAngle, rotateCenter);

            toolPath2 = toolPath1.Copy();
            MirrorX(toolPath2);
        }

        private static void ToolStart(PathObject p, Point start)
        {
            p = new ItemPath(start, 0);
            p.Start = start;
            //    Logger.Trace("ToolStart  start:{0}", start);
        }
        private static void ToolMoveTo(PathObject p, double x, double y)
        {
            ((ItemPath)p).Add(new Point(x, y), 0, 0);
            //    Logger.Trace("ToolMoveTo  x:{0}  y:{1}", x, y);
        }
        private static void ToolMoveTo(PathObject p, Point xy)
        {
            ((ItemPath)p).Add(xy, 0, 0);
            //    Logger.Trace("ToolMoveTo  start:{0}", xy);
        }
        private static void ToolArcTo(PathObject p, double x, double y, double i, double j, bool isCW)
        {
            ((ItemPath)p).AddArc(new Point(x, y), new Point(i, j), 0, isCW, arcToLine, false);    // draw inner segment
        }
        private static void RotatePath(PathObject item, double angleRad, Point offset)
        { RotatePath(item, angleRad, offset.X, offset.Y); }
        private static void RotatePath(PathObject item, double angleRad, double offsetX, double offsetY)
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

        private static void RotateArray(Point[] pArray, double angleRad, Point offset)
        {
            for (int i = 0; i < pArray.Length; i++)
            {
                pArray[i] = RotatePoint(pArray[i], angleRad, offset);
            }
        }

        private static Point RotatePoint(Point p, double angleRad, Point offset)
        { return RotatePoint(p, angleRad, offset.X, offset.Y); }
        private static Point RotatePoint(Point p, double angleRad, double offX, double offY)
        {
            double newvalx = (p.X - offX) * Math.Cos(angleRad) - (p.Y - offY) * Math.Sin(angleRad);
            double newvaly = (p.X - offX) * Math.Sin(angleRad) + (p.Y - offY) * Math.Cos(angleRad);
            Point pn = new Point(newvalx + offX, newvaly + offY);
            return pn;
        }

        private static void MirrorX(PathObject item)    // scaleX != scaleY will not work for arc!
        {
            Logger.Trace("...MirrorX  --------------------------------------");
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

        private static Point GetDistancePoint(Point start, double addR, double angleRad)
        {
            return new Point(start.X + addR * Math.Cos(angleRad), start.Y + addR * Math.Sin(angleRad));
        }

        private static Point GetIntermediatePoint(Point start, Point end, double distance, bool extend = false)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double moveLength = (float)Math.Sqrt(dx * dx + dy * dy);
            if (moveLength == 0)
                return end;
            if (extend)
            {
                double newx = end.X + distance * dx / moveLength;
                double newy = end.Y + distance * dy / moveLength;
                return new Point(newx, newy);
            }
            else
            {
                double newx = start.X + distance * dx / moveLength;
                double newy = start.Y + distance * dy / moveLength;
                return new Point(newx, newy);
            }
        }
    }
}
