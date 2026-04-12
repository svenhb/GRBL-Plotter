/*  GRBL-Plotter. Another GCode sender for GRBL.
    This FileName is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using System;
using System.Drawing;

namespace GrblPlotter.Helper
{
    internal struct GrblPoint
    {
        private static int decimalPlaces = 3;
        public double X, Y, Z, A, B, C;
        public GrblPoint(GrblPoint xy)
        { X = xy.X; Y = xy.Y; Z = xy.Z; A = xy.A; B = xy.B; C = xy.C; }
 //       public XyzPoint(XyPoint xy, double z)
 //       { X = xy.X; Y = xy.Y; Z = z; A = 0; B = 0; C = 0; }
  //      public XyzPoint(XyPoint xy, double z, double a)
  //      { X = xy.X; Y = xy.Y; Z = z; A = a; B = 0; C = 0; }
        public GrblPoint(double x, double y, double z)
        { X = x; Y = y; Z = z; A = 0; B = 0; C = 0; }
        public GrblPoint(double x, double y, double z, double a)
        { X = x; Y = y; Z = z; A = a; B = 0; C = 0; }
        public GrblPoint(double x, double y, double z, double a, double b, double c)
        { X = x; Y = y; Z = z; A = a; B = b; C = c; }
        // Overload + operator 
        public static implicit operator GrblPoint(XyzPoint p)
        { return new GrblPoint(p.X,p.Y,p.Z,p.A,p.B,p.C); }

        public static GrblPoint operator +(GrblPoint b, GrblPoint c)
        {
            GrblPoint a = new GrblPoint
            {
                X = b.X + c.X,
                Y = b.Y + c.Y,
                Z = b.Z + c.Z,
                A = b.A + c.A,
                B = b.B + c.B,
                C = b.C + c.C
            };
            return a;
        }
        public static GrblPoint operator -(GrblPoint b, GrblPoint c)
        {
            GrblPoint a = new GrblPoint
            {
                X = b.X - c.X,
                Y = b.Y - c.Y,
                Z = b.Z - c.Z,
                A = b.A - c.A,
                B = b.B - c.B,
                C = b.C - c.C
            };
            return a;
        }
        public static bool AlmostEqual(GrblPoint a, GrblPoint b)
        {
            //     return (Math.Abs(a.X - b.X) <= grbl.resolution) && (Math.Abs(a.Y - b.Y) <= grbl.resolution) && (Math.Abs(a.Z - b.Z) <= grbl.resolution);
            return (IsEqual(a.X, b.X) && IsEqual(a.Y, b.Y) && IsEqual(a.Z, b.Z));
        }
        public static bool IsEqual(double vala, double valb)
        { return (Math.Round(vala, decimalPlaces) == Math.Round(valb, decimalPlaces)); }

        public PointF ToPointF()
        { return new PointF((float)X, (float)Y); }


        public string Print(bool singleLines, bool full)
        {
            if (!full)
            {
                if (Grbl.axisA)
                    if (singleLines)
                        return string.Format("X={0,9:0.000}\rY={1,9:0.000}\rZ={2,9:0.000}\r{3,9:0.000}", X, Y, Z,  A);
                    else
                        return string.Format("X={0,9:0.000}  Y={1,9:0.000}  Z={2,9:0.000}\r{3,9:0.000}", X, Y, Z,  A);

                else
                    if (singleLines)
                    return string.Format("X={0,9:0.000}\rY={1,9:0.000}\rZ={2,9:0.000}", X, Y, Z);
                else
                    return string.Format("X={0,9:0.000} Y={1,9:0.000} Z={2,9:0.000}", X, Y, Z);
            }
            else
            {
                if (singleLines)
                    return string.Format("X={0,9:0.000}\rY={1,9:0.000}\rZ={2,9:0.000}\rA={3,9:0.000}\rB={4,9:0.000}\rC={5,9:0.000}", X, Y, Z, A, B, C);
                else
                    return string.Format("X={0,9:0.000} Y={1,9:0.000} Z={2,9:0.000}\rA={3,9:0.000} B={4,9:0.000} C={5,9:0.000}", X, Y, Z, A, B, C);
            }
        }

    };

}
