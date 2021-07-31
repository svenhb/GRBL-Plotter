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
using System.Windows;
using System.Windows.Media;

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

    }
}
