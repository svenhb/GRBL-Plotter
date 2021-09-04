/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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

/* 2019-11-24 new
 * 
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using AForge.Math;

namespace GrblPlotter
{
    public static class ImportMath
    {
//        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Calculate Path-Arc-Command - Code from https://github.com/vvvv/SVG/blob/master/Source/Paths/SvgArcSegment.cs
        /// </summary>
        public static void CalcArc(float astartX, float astartY, float radiusX, float radiusY, 
            float angle, float size, float sweep, float endX, float endY, Action<Point, string> moveTo)
        {
 //           Logger.Trace(" calcArc Start: {0};{1} rx: {2} ry: {3} a: {4} size: {5} sweep: {6} End: {7};{8}", StartX, StartY, RadiusX, RadiusY,
//            Angle, Size, Sweep, EndX, EndY);
            if (radiusX == 0.0f && radiusY == 0.0f)
            {
                //              graphicsPath.AddLine(this.Start, this.End);
                return;
            }
            double sinPhi = Math.Sin(angle * Math.PI / 180.0);
            double cosPhi = Math.Cos(angle * Math.PI / 180.0);
            double x1dash = cosPhi * (astartX - endX) / 2.0 + sinPhi * (astartY - endY) / 2.0;
            double y1dash = -sinPhi * (astartX - endX) / 2.0 + cosPhi * (astartY - endY) / 2.0;
            double root;
            double numerator = radiusX * radiusX * radiusY * radiusY - radiusX * radiusX * y1dash * y1dash - radiusY * radiusY * x1dash * x1dash;
            float rx = radiusX;
            float ry = radiusY;
            if (numerator < 0.0)
            {
                float s = (float)Math.Sqrt(1.0 - numerator / (radiusX * radiusX * radiusY * radiusY));

                rx *= s;
                ry *= s;
                root = 0.0;
            }
            else
            {
                root = ((size == 1 && sweep == 1) || (size == 0 && sweep == 0) ? -1.0 : 1.0) * Math.Sqrt(numerator / (radiusX * radiusX * y1dash * y1dash + radiusY * radiusY * x1dash * x1dash));
            }
            double cxdash = root * rx * y1dash / ry;
            double cydash = -root * ry * x1dash / rx;
            double cx = cosPhi * cxdash - sinPhi * cydash + (astartX + endX) / 2.0;
            double cy = sinPhi * cxdash + cosPhi * cydash + (astartY + endY) / 2.0;
            double theta1 = CalculateVectorAngle(1.0, 0.0, (x1dash - cxdash) / rx, (y1dash - cydash) / ry);
            double dtheta = CalculateVectorAngle((x1dash - cxdash) / rx, (y1dash - cydash) / ry, (-x1dash - cxdash) / rx, (-y1dash - cydash) / ry);
            if (sweep == 0 && dtheta > 0)
            {
                dtheta -= 2.0 * Math.PI;
            }
            else if (sweep == 1 && dtheta < 0)
            {
                dtheta += 2.0 * Math.PI;
            }
            int segments = (int)Math.Ceiling((double)Math.Abs(dtheta / (Math.PI / 2.0)));
            double delta = dtheta / segments;
            double t = 8.0 / 3.0 * Math.Sin(delta / 4.0) * Math.Sin(delta / 4.0) / Math.Sin(delta / 2.0);

            double startX = astartX;
            double startY = astartY;

            for (int i = 0; i < segments; ++i)
            {
                double cosTheta1 = Math.Cos(theta1);
                double sinTheta1 = Math.Sin(theta1);
                double theta2 = theta1 + delta;
                double cosTheta2 = Math.Cos(theta2);
                double sinTheta2 = Math.Sin(theta2);

                double endpointX = cosPhi * rx * cosTheta2 - sinPhi * ry * sinTheta2 + cx;
                double endpointY = sinPhi * rx * cosTheta2 + cosPhi * ry * sinTheta2 + cy;

                double dx1 = t * (-cosPhi * rx * sinTheta1 - sinPhi * ry * cosTheta1);
                double dy1 = t * (-sinPhi * rx * sinTheta1 + cosPhi * ry * cosTheta1);

                double dxe = t * (cosPhi * rx * sinTheta2 + sinPhi * ry * cosTheta2);
                double dye = t * (sinPhi * rx * sinTheta2 - cosPhi * ry * cosTheta2);

                points = new Point[4];
                points[0] = new Point(startX, startY);
                points[1] = new Point((startX + dx1), (startY + dy1));
                points[2] = new Point((endpointX + dxe), (endpointY + dye));
                points[3] = new Point(endpointX, endpointY);
                var b = GetBezierApproximation(points, (int)Properties.Settings.Default.importBezierLineSegmentsCnt);
                if (moveTo != null)
                    for (int k = 1; k < b.Points.Count; k++)
                    moveTo(b.Points[k], "arc"); //svgMoveTo(b.Points[k], "arc");

                theta1 = theta2;
                startX = (float)endpointX;
                startY = (float)endpointY;
            }
        }
        private static double CalculateVectorAngle(double ux, double uy, double vx, double vy)
        {
            double ta = Math.Atan2(uy, ux);
            double tb = Math.Atan2(vy, vx);
            if (tb >= ta)
            { return tb - ta; }
            return Math.PI * 2 - (ta - tb);
        }


        public static void CalcQuadraticBezier(Point p0, Point c2, Point c3, Action<Point, string> moveTo, string cmt)
        {
            Vector qp1  = ((Vector)c2 - (Vector)p0); qp1 *= (2d / 3d); qp1 += (Vector)p0;      // float qpx1 = (cx2 - lastX) * 2 / 3 + lastX;     // shorten control points to 2/3 length to use 
            Vector qp2  = ((Vector)c2 - (Vector)c3); qp2 *= (2d / 3d); qp2 += (Vector)c3;      // float qpx2 = (cx2 - cx3) * 2 / 3 + cx3;
            Point[] points = new Point[4]; 
            points[0] = p0;             // new Point(lastX, lastY);
            points[1] = (Point)qp1;     // new Point(qpx1, qpy1);
            points[2] = (Point)qp2;     // new Point(qpx2, qpy2);
            points[3] = c3;             // new Point(cx3, cy3);
            var b = GetBezierApproximation(points, (int)Properties.Settings.Default.importBezierLineSegmentsCnt);
            if(moveTo!=null)
                for (int i = 1; i < b.Points.Count; i++)
                    moveTo(b.Points[i], cmt);
        }

        public static void CalcCubicBezier(Point p0, Point c1, Point c2, Point c3, Action<Point, string> moveTo, string cmt)
        {
            Point[] points = new Point[4];
            points[0] = p0;             // new Point(lastX, lastY);
            points[1] = c1;             
            points[2] = c2;             
            points[3] = c3;             // new Point(cx3, cy3);
            var b = GetBezierApproximation(points, (int)Properties.Settings.Default.importBezierLineSegmentsCnt);
            if (moveTo != null)
                for (int i = 1; i < b.Points.Count; i++)
                    moveTo(b.Points[i], cmt);
        }

        /// <summary>
        /// Calculate Bezier line segments
        /// from http://stackoverflow.com/questions/13940983/how-to-draw-bezier-curve-by-several-points
        /// </summary>
        private static Point[] points;
        public static PolyLineSegment GetBezierApproximation(Point[] controlPoints, int outputSegmentCount)
        {
     //       return BezierTools.FlattenTo(controlPoints).ToArray();
            Point[] points = new Point[outputSegmentCount + 1];
            if (controlPoints!=null)
                for (int i = 0; i <= outputSegmentCount; i++)
                {
                    double t = (double)i / outputSegmentCount;
                    points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
                }
            return new PolyLineSegment(points, true);
        }
        private static Point GetBezierPoint(double t, Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            double x = (1 - t) * P0.X + t * P1.X;
            return new Point(x, (1 - t) * P0.Y + t * P1.Y);
        }




        // https://dlacko.org/blog/2016/10/19/approximating-bezier-curves-by-biarcs/
        // https://github.com/domoszlai/bezier2biarc

     //   var bezier = Current;
     //   private static var biarcs = Algorithm.ApproxCubicBezier(bezier, 5, (float)ErrorLevel.Value);

        public class Algorithm
        {

            private static bool IsRealInflexionPoint(Complex t)
            {
                return t.Im == 0 && t.Re > 0 && t.Re < 1;
            }

            /// <summary>
            /// Algorithm to approximate a bezier curve with biarcs
            /// </summary>
            /// <param name="bezier">The bezier curve to be approximated.</param>
            /// <param name="nrPointsToCheck">The number of points used for calculating the approximation error.</param>
            /// <param name="tolerance">The approximation is accepted if the maximum devation at the sampling points is smaller than this number.</param>
            /// <returns></returns>
            public static List<BiArc> ApproxCubicBezier(CubicBezier bezier, int nrPointsToCheck, float tolerance)
            {
                // The result will be put here
                List<BiArc> biarcs = new List<BiArc>();

                // The bezier curves to approximate
                var curves = new Stack<CubicBezier>();
                curves.Push(bezier);

                // ---------------------------------------------------------------------------
                // First, calculate the inflexion points and split the bezier at them (if any)

                var toSplit = curves.Pop();

                // Edge case: P1 == P2 -> Split bezier
                if (bezier.P1 == bezier.P2)
                {
                    var bs = bezier.Split(0.5f);
                    curves.Push(bs.Item2);
                    curves.Push(bs.Item1);
                }
                // Edge case -> no inflexion points
                else if (toSplit.P1 == toSplit.C1 || toSplit.P2 == toSplit.C2)
                {
                    curves.Push(toSplit);
                }
                else
                {
                    var inflex = toSplit.InflexionPoints;

                    var i1 = IsRealInflexionPoint(inflex.Item1);
                    var i2 = IsRealInflexionPoint(inflex.Item2);

                    if (i1 && !i2)
                    {
                        var splited = toSplit.Split((float)inflex.Item1.Re);
                        curves.Push(splited.Item2);
                        curves.Push(splited.Item1);
                    }
                    else if (!i1 && i2)
                    {
                        var splited = toSplit.Split((float)inflex.Item2.Re);
                        curves.Push(splited.Item2);
                        curves.Push(splited.Item1);
                    }
                    else if (i1 && i2)
                    {
                        var t1 = (float)inflex.Item1.Re;
                        var t2 = (float)inflex.Item2.Re;

                        // I'm not sure if I need, but it does not hurt to order them
                        if (t1 > t2)
                        {
                            var tmp = t1;
                            t1 = t2;
                            t2 = tmp;
                        }

                        // Make the first split and save the first new curve. The second one has to be splitted again
                        // at the recalculated t2 (it is on a new curve)

                        var splited1 = toSplit.Split(t1);

                        t2 = (1 - t1) * t2;

                        toSplit = splited1.Item2;
                        var splited2 = toSplit.Split(t2);

                        curves.Push(splited2.Item2);
                        curves.Push(splited2.Item1);
                        curves.Push(splited1.Item1);
                    }
                    else
                    {
                        curves.Push(toSplit);
                    }
                }

                // ---------------------------------------------------------------------------
                // Second, approximate the curves until we run out of them

                while (curves.Count > 0)
                {
                    bezier = curves.Pop();

                    // ---------------------------------------------------------------------------
                    // Calculate the transition point for the BiArc 

                    // V: Intersection point of tangent lines
                    var C1 = bezier.P1 == bezier.C1 ? bezier.C2 : bezier.C1;
                    var C2 = bezier.P2 == bezier.C2 ? bezier.C1 : bezier.C2;

                    var T1 = new Line(bezier.P1, C1);
                    var T2 = new Line(bezier.P2, C2);

                    // Edge case: control lines are parallel
                    if (T1.m == T2.m)
                    {
                        var bs = bezier.Split(0.5f);
                        curves.Push(bs.Item2);
                        curves.Push(bs.Item1);
                        continue;
                    }

                    var V = T1.Intersection(T2);

                    // G: incenter point of the triangle (P1, V, P2)
                    // http://www.mathopenref.com/coordincenter.html
                    var dP2V = Vector2.Distance(bezier.P2, V);
                    var dP1V = Vector2.Distance(bezier.P1, V);
                    var dP1P2 = Vector2.Distance(bezier.P1, bezier.P2);
                    var G = (dP2V * bezier.P1 + dP1V * bezier.P2 + dP1P2 * V) / (dP2V + dP1V + dP1P2);

                    // ---------------------------------------------------------------------------
                    // Calculate the BiArc

                    BiArc biarc = new BiArc(bezier.P1, (bezier.P1 - C1), bezier.P2, (bezier.P2 - C2), G);

                    // ---------------------------------------------------------------------------
                    // Calculate the maximum error

                    var maxDistance = 0f;
                    var maxDistanceAt = 0f;

                    var parameterStep = 1f / nrPointsToCheck;

                    for (int i = 0; i <= nrPointsToCheck; i++)
                    {
                        var t = parameterStep * i;
                        var u1 = biarc.PointAt(t);
                        var u2 = bezier.PointAt(t);
                        var distance = (u1 - u2).Length();

                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            maxDistanceAt = t;
                        }
                    }

                    // Check if the two curves are close enough
                    if (maxDistance > tolerance)
                    {
                        // If not, split the bezier curve the point where the distance is the maximum
                        // and try again with the two halfs
                        var bs = bezier.Split(maxDistanceAt);
                        curves.Push(bs.Item2);
                        curves.Push(bs.Item1);
                    }
                    else
                    {
                        // Otherwise we are done with the current bezier
                        biarcs.Add(biarc);
                    }
                }

                return biarcs;
            }

        }
        public struct CubicBezier
        {
            /// <summary>
            /// Start point
            /// </summary>
            public readonly Vector2 P1;
            /// <summary>
            /// End point
            /// </summary>
            public readonly Vector2 P2;
            /// <summary>
            /// First control point
            /// </summary>
            public readonly Vector2 C1;
            /// <summary>
            /// Second control point
            /// </summary>
            public readonly Vector2 C2;

            public CubicBezier(Vector2 P1, Vector2 C1, Vector2 C2, Vector2 P2)
            {
                this.P1 = P1;
                this.C1 = C1;
                this.P2 = P2;
                this.C2 = C2;
            }

            /// <summary>
            /// Implement the parametric equation.
            /// </summary>
            /// <param name="t">Parameter of the curve. Must be in [0,1]</param>
            /// <returns></returns>
            public Vector2 PointAt(float t)
            {
                return (float)Math.Pow(1 - t, 3) * P1 +
                           (float)(3 * Math.Pow(1 - t, 2) * t) * C1 +
                           (float)(3 * (1 - t) * Math.Pow(t, 2)) * C2 +
                           (float)Math.Pow(t, 3) * P2;
            }

            /// <summary>
            /// Split a bezier curve at a given parameter value. It returns both of the new ones
            /// </summary>
            /// <param name="t">Parameter of the curve. Must be in [0,1]</param>
            /// <returns></returns>
            public Tuple<CubicBezier, CubicBezier> Split(float t)
            {
                var p0 = P1 + t * (C1 - P1);
                var p1 = C1 + t * (C2 - C1);
                var p2 = C2 + t * (P2 - C2);

                var p01 = p0 + t * (p1 - p0);
                var p12 = p1 + t * (p2 - p1);

                var dp = p01 + t * (p12 - p01);

                return Tuple.Create(new CubicBezier(P1, p0, p01, dp), new CubicBezier(dp, p12, p2, P2));
            }

            /// <summary>
            /// The orientation of the Bezier curve
            /// </summary>
            public bool IsClockwise
            {
                get
                {
                    var sum = 0d;
                    sum += (C1.X - P1.X) * (C1.Y + P1.Y);
                    sum += (C2.X - C1.X) * (C2.Y + C1.Y);
                    sum += (P2.X - C2.X) * (P2.Y + C2.Y);
                    sum += (P1.X - P2.X) * (P1.Y + P2.Y);
                    return sum < 0;
                }
            }

            /// <summary>
            /// Inflexion points of the Bezier curve. They only valid if they are real and in the range of [0,1]
            /// </summary>
            /// <param name="bezier"></param>
            /// <returns></returns>
            public Tuple<Complex, Complex> InflexionPoints
            {
                get
                {
                    // http://www.caffeineowl.com/graphics/2d/vectorial/cubic-inflexion.html

                    var A = C1 - P1;
                    var B = C2 - C1 - A;
                    var C = P2 - C2 - A - 2 * B;

                    var a = new Complex(B.X * C.Y - B.Y * C.X, 0);
                    var b = new Complex(A.X * C.Y - A.Y * C.X, 0);
                    var c = new Complex(A.X * B.Y - A.Y * B.X, 0);

                    var t1 = (-b + Complex.Sqrt(b * b - 4 * a * c)) / (2 * a);
                    var t2 = (-b - Complex.Sqrt(b * b - 4 * a * c)) / (2 * a);

                    return Tuple.Create(t1, t2);
                }
            }
        }

        public struct BiArc
        {
            public readonly Arc A1;
            public readonly Arc A2;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="P1">Start point</param>
            /// <param name="T1">Tangent vector at P1</param>
            /// <param name="P2">End point</param>
            /// <param name="T2">Tangent vector at P2</param>
            /// <param name="T">Transition point</param>
            public BiArc(Vector2 P1, Vector2 T1, Vector2 P2, Vector2 T2, Vector2 T)
            {
                // Calculate the orientation
                // https://en.wikipedia.org/wiki/Curve_orientation

                var sum = 0d;
                sum += (T.X - P1.X) * (T.Y + P1.Y);
                sum += (P2.X - T.X) * (P2.Y + T.Y);
                sum += (P1.X - P2.X) * (P1.Y + P2.Y);
                var cw = sum < 0;

                // Calculate perpendicular lines to the tangent at P1 and P2
                var tl1 = Line.CreatePerpendicularAt(P1, P1 + T1);
                var tl2 = Line.CreatePerpendicularAt(P2, P2 + T2);

                // Calculate the perpendicular bisector of P1T and P2T
                var P1T2 = (P1 + T) / 2;
                var pbP1T = Line.CreatePerpendicularAt(P1T2, T);

                var P2T2 = (P2 + T) / 2;
                var pbP2T = Line.CreatePerpendicularAt(P2T2, T);

                // The origo of the circles are at the intersection points
                var C1 = tl1.Intersection(pbP1T);
                var C2 = tl2.Intersection(pbP2T);

                // Calculate the radii
                var r1 = (C1 - P1).Length();
                var r2 = (C2 - P2).Length();

                // Calculate start and sweep angles
                var startVector1 = P1 - C1;
                var endVector1 = T - C1;
                var startAngle1 = Math.Atan2(startVector1.Y, startVector1.X);
                var sweepAngle1 = Math.Atan2(endVector1.Y, endVector1.X) - startAngle1;

                var startVector2 = T - C2;
                var endVector2 = P2 - C2;
                var startAngle2 = Math.Atan2(startVector2.Y, startVector2.X);
                var sweepAngle2 = Math.Atan2(endVector2.Y, endVector2.X) - startAngle2;

                // Adjust angles according to the orientation of the curve
                if (cw && sweepAngle1 < 0) sweepAngle1 += 2 * Math.PI;      // 2 * Math.PI + sweepAngle1;
                if (!cw && sweepAngle1 > 0) sweepAngle1 -= 2 * Math.PI;     // sweepAngle1 - 2 * Math.PI;
                if (cw && sweepAngle2 < 0) sweepAngle2 += 2 * Math.PI;      // 2 * Math.PI + sweepAngle2;
                if (!cw && sweepAngle2 > 0) sweepAngle2 -= 2 * Math.PI;     // sweepAngle2 - 2 * Math.PI;

                A1 = new Arc(C1, r1, (float)startAngle1, (float)sweepAngle1, P1, T);
                A2 = new Arc(C2, r2, (float)startAngle2, (float)sweepAngle2, T, P2);
            }

            /// <summary>
            /// Implement the parametric equation.
            /// </summary>
            /// <param name="t">Parameter of the curve. Must be in [0,1]</param>
            /// <returns></returns>
            public Vector2 PointAt(float t)
            {
                var s = A1.Length / (A1.Length + A2.Length);

                if (t <= s)
                {
                    return A1.PointAt(t / s);
                }
                else
                {
                    return A2.PointAt((t - s) / (1 - s));
                }
            }

            public float Length
            {
                get { return A1.Length + A2.Length; }
            }
        }
   
        public struct Arc
        {
            /// <summary>
            /// Center point
            /// </summary>
            public readonly Vector2 C;
            /// <summary>
            /// Radius
            /// </summary>
            public readonly float r;
            /// <summary>
            /// Start angle in radian
            /// </summary>
            public readonly float startAngle;
            /// <summary>
            /// Sweep angle in radian
            /// </summary>
            public readonly float sweepAngle;
            /// <summary>
            /// Start point of the arc
            /// </summary>
            public readonly Vector2 P1;
            /// <summary>
            /// End point of the arc
            /// </summary>
            public readonly Vector2 P2;

            public Arc(Vector2 C, float r, float startAngle, float sweepAngle, Vector2 P1, Vector2 P2)
            {
                this.C = C;
                this.r = r;
                this.startAngle = startAngle;
                this.sweepAngle = sweepAngle;
                this.P1 = P1;
                this.P2 = P2;
            }

            /// <summary>
            /// Orientation of the arc
            /// </summary>
            public bool IsClockwise
            {
                get { return sweepAngle > 0; }
            }

            /// <summary>
            /// Implement the parametric equation.
            /// </summary>
            /// <param name="t">Parameter of the curve. Must be in [0,1]</param>
            /// <returns></returns>
            public Vector2 PointAt(float t)
            {
                var x = C.X + r * Math.Cos(startAngle + t * sweepAngle);
                var y = C.Y + r * Math.Sin(startAngle + t * sweepAngle);
                return new Vector2((float)x, (float)y);
            }

            public float Length
            {
                get { return r * Math.Abs(sweepAngle); }
            }
        }

        public struct Line
        {
            /// <summary>
            /// Slope
            /// </summary>
            public readonly float m;
            /// <summary>
            /// Point
            /// </summary>
            public readonly Vector2 P;

            /// <summary>
            /// Define a line by two points
            /// </summary>
            /// <param name="P1"></param>
            /// <param name="P2"></param>
            public Line(Vector2 P1, Vector2 P2) : this(P1, Slope(P1, P2))
            {
            }

            /// <summary>
            /// Define a line by a point and slope
            /// </summary>
            /// <param name="P"></param>
            /// <param name="m"></param>
            public Line(Vector2 P, float m)
            {
                this.P = P;
                this.m = m;
            }

            /// <summary>
            /// Calculate the intersection point of this line and another one
            /// </summary>
            /// <param name="l"></param>
            /// <returns></returns>
            public Vector2 Intersection(Line l)
            {
                if (float.IsNaN(this.m))
                {
                    return VerticalIntersection(this, l);
                }
                else if (float.IsNaN(l.m))
                {
                    return VerticalIntersection(l, this);
                }
                else
                {
                    var x = (this.m * this.P.X - l.m * l.P.X - this.P.Y + l.P.Y) / (this.m - l.m);
                    var y = m * x - m * P.X + P.Y;
                    return new Vector2(x, y);
                }
            }

            /// <summary>
            /// Special case, the first one is vertical (we suppose that the other one is not, 
            /// otherwise they do not cross)
            /// </summary>
            /// <param name="hl"></param>
            /// <param name="l"></param>
            /// <returns></returns>
            private static Vector2 VerticalIntersection(Line vl, Line l)
            {
                var x = vl.P.X;
                var y = l.m * (x - l.P.X) + l.P.Y;
                return new Vector2(x, y);
            }

            /// <summary>
            /// Creates a a line which is perpendicular to the line defined by P and P1 and goes through P
            /// </summary>
            /// <param name="P"></param>
            /// <param name="P1"></param>
            /// <returns></returns>
            public static Line CreatePerpendicularAt(Vector2 P, Vector2 P1)
            {
                var m = Slope(P, P1);

                if (m == 0)
                {
                    return new Line(P, float.NaN);
                }
                else if (float.IsNaN(m))
                {
                    return new Line(P, 0);
                }
                else
                {
                    return new Line(P, -1f / m);
                }
            }

            private static float Slope(Vector2 P1, Vector2 P2)
            {
                if (P2.X == P1.X)
                {
                    return float.NaN;
                }
                else
                {
                    return (float)((P2.Y - P1.Y) / (P2.X - P1.X));
                }
            }

        }
    }
}
