/*  GRBL-Plotter2. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2025-2026 Sven Hasemann contact: svenhb@web.de

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

using GrblPlotter;
using GrblPlotter.UserControls;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace GrblPlotter.Helper
{
	
    /// <summary>
    /// calculate overall dimensions of drawing
    /// </summary>
    internal class Dimensions
    {
        public double minx, maxx, miny, maxy, minz, maxz;
        public double cenx, ceny, cenz;
        public double dimx, dimy, dimz;

        public Dimensions(Dimensions old)
        {
            minx = old.minx;
            maxx = old.maxx;
            miny = old.miny;
            maxy = old.maxy;
            minz = old.minz;
            maxz = old.maxz;
            dimx = old.dimx;
            dimy = old.dimy;
            dimz = old.dimz;
			cenx = old.cenx;
			ceny = old.ceny;
			cenz = old.cenz;
        }

        public Dimensions()
        { ResetDimension(); }

        public void AddDimensionXY(Dimensions tmp)
        {
            SetDimensionXY(tmp.minx, tmp.miny);
            SetDimensionXY(tmp.maxx, tmp.maxy);
        }
        public void SetDimensionXYZ(double? x, double? y, double? z)
        {
            if (x != null) { SetDimensionX((double)x); }
            if (y != null) { SetDimensionY((double)y); }
            if (z != null) { SetDimensionZ((double)z); }
        }

  //      public void SetDimensionXY(System.Windows.Point tmp)
  //      { SetDimensionXY(tmp.X, tmp.Y); }
        public void SetDimensionXY(XyPoint tmp)
        { SetDimensionXY(tmp.X, tmp.Y); }
        public void SetDimensionXY(double? x, double? y)
        {
            if (x != null) { SetDimensionX((double)x); }
            if (y != null) { SetDimensionY((double)y); }
        }
        public void SetDimensionX(double value)
        {
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            minx = Math.Min(minx, value);
            maxx = Math.Max(maxx, value);
            dimx = maxx - minx;
			cenx = (maxx + minx)/2;
        }
        public void SetDimensionY(double value)
        {
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            miny = Math.Min(miny, value);
            maxy = Math.Max(maxy, value);
            dimy = maxy - miny;
			ceny = (maxy + miny)/2;
        }
        public void SetDimensionZ(double value)
        {
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            minz = Math.Min(minz, value);
            maxz = Math.Max(maxz, value);
            dimz = maxz - minz;
			cenz = (maxz + minz)/2;
        }
        public void OffsetXY(double x, double y)
        { minx += x; maxx += x; miny += y; maxy += y; }
        public void ScaleXY(double scaleX, double scaleY)
        { minx *= scaleX; maxx *= scaleX; cenx *= scaleX; miny *= scaleY; maxy *= scaleY; ceny *= scaleY; dimx = maxx - minx; dimy = maxy - miny; }

        public double GetArea()
        { return dimx * dimy; }

        // calculate min/max dimensions of a circle
        public void SetDimensionCircle(double x, double y, double radius, double startDeg, double deltaDeg)
        {
            double end = startDeg + deltaDeg;
            double i = startDeg;

            if (Math.Abs(Math.Abs(deltaDeg) - 360) < 0.00001)
            {
                SetDimensionXY(x - radius, y - radius);
                SetDimensionXY(x + radius, y + radius);
            }
            else
            {
                SetDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                SetDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                i = end;
                SetDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                SetDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));

                for (int k = -360; k <= 360; k += 90)
                {
                    if (deltaDeg > 0)
                    {
                        if ((k > startDeg) && (k < end))
                        {
                            i = k;
                            SetDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                            SetDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                        }
                    }
                    else
                    {
                        if ((k < startDeg) && (k > end))
                        {
                            i = k;
                            SetDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                            SetDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                        }
                    }
                }
            }
        }

        public void SetDimensionArc(XyPoint End,  XyPoint tmp, double X, double Y, bool isCW)
        {
        }

        public void ResetDimension()
        {
            minx = Double.MaxValue;
            miny = Double.MaxValue;
            minz = Double.MaxValue;
            maxx = Double.MinValue;
            maxy = Double.MinValue;
            maxz = Double.MinValue;
            dimx = 0;
            dimy = 0;
            dimz = 0;
            cenx = 0;
            ceny = 0;
            cenz = 0;
        }

        public bool IsXYSet()
        { return ((minx != Double.MaxValue) && (miny != Double.MaxValue) && (maxx != Double.MinValue) && (maxy != Double.MinValue)); }
        //		{	return ((dimx != 0) || (dimy != 0));}

        public XyPoint GetCenter()
        {
            if (IsXYSet())
            {
                double cx = minx + ((maxx - minx) / 2);
                double cy = miny + ((maxy - miny) / 2);
                return new XyPoint(cx, cy);
            }
            else
                return new XyPoint();
        }

        // return string with dimensions
        public String GetXYString()
        { return String.Format("minX:{0:0.000} minY:{1:0.000} maxX:{2:0.000}  maxY:{3:0.000}", minx, miny, maxx, maxy); }

        public String GetMinMaxString()
        {
            string x = String.Format("X:{0,8:####0.000} |{1,8:####0.000} |{2,8:####0.000}\r\n", minx, maxx, dimx);
            string y = String.Format("Y:{0,8:####0.000} |{1,8:####0.000} |{2,8:####0.000}\r\n", miny, maxy, dimy);
            string z = String.Format("Z:{0,8:####0.000} |{1,8:####0.000} |{2,8:####0.000}", minz, maxz, dimz);
            if ((minx == Double.MaxValue) || (maxx == Double.MinValue))
                x = "X: unknown | unknown\r\n";
            if ((miny == Double.MaxValue) || (maxy == Double.MinValue))
                y = "Y: unknown | unknown\r\n";
            if ((minz == Double.MaxValue) || (maxz == Double.MinValue))
                z = "";// z = "Z: unknown | unknown";
            return "    Min.   |   Max.  | Dimension\r\n" + x + y + z;
        }
        public bool WithinLimits(GrblPoint actualMachine, GrblPoint actualWorld)
        {
            return (WithinLimits(actualMachine, minx - actualWorld.X, miny - actualWorld.Y) && WithinLimits(actualMachine, maxx - actualWorld.X, maxy - actualWorld.Y));
        }
        public bool WithinLimits(XyzPoint actualMachine, XyzPoint actualWorld)
        {
            return (WithinLimits(actualMachine, minx - actualWorld.X, miny - actualWorld.Y) && WithinLimits(actualMachine, maxx - actualWorld.X, maxy - actualWorld.Y));
        }
        public static bool WithinLimits(GrblPoint actualMachine, double tstx, double tsty)
        {
      /*      double minlx = (double)Properties.ListSettings.Default.machineLimitsHomeX;
            double maxlx = minlx + (double)Properties.ListSettings.Default.machineLimitsRangeX;
            double minly = (double)Properties.ListSettings.Default.machineLimitsHomeY;
            double maxly = minly + (double)Properties.ListSettings.Default.machineLimitsRangeY;
            tstx += actualMachine.X;
            tsty += actualMachine.Y;
            if ((tstx < minlx) || (tstx > maxlx))
                return false;
            if ((tsty < minly) || (tsty > maxly))
      */          return false;
            return true;
        }

        public bool IsWithin(Dimensions tmp)
        {
            return ((minx >= tmp.minx) && (miny >= tmp.miny) && (maxx <= tmp.maxx) && (maxy <= tmp.maxy));
        }
    }
}