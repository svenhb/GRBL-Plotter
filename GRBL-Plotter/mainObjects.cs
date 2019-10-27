/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;
using GRBL_Plotter.Resources;

namespace GRBL_Plotter
{
    public static class datapath
    {
        public const string fonts    = "data\\fonts";
        public const string tools    = "\\data\\tools";
        public const string scripts  = "\\data\\scripts";
        public const string usecases = "\\data\\usecases";
        public const string hotkeys = "\\data\\hotkeys.xml";
        public const string examples = "\\data\\examples";
    }

    public struct xyzPoint
    {
        public double X, Y, Z, A, B, C;
        public xyzPoint(double x, double y, double z, double a = 0)
        { X = x; Y = y; Z = z; A = a; B = 0; C = 0; }
        // Overload + operator 
        public static xyzPoint operator +(xyzPoint b, xyzPoint c)
        {
            xyzPoint a = new xyzPoint();
            a.X = b.X + c.X;
            a.Y = b.Y + c.Y;
            a.Z = b.Z + c.Z;
            a.A = b.A + c.A;
            a.B = b.B + c.B;
            a.C = b.C + c.C;
            return a;
        }
        public static xyzPoint operator -(xyzPoint b, xyzPoint c)
        {
            xyzPoint a = new xyzPoint();
            a.X = b.X - c.X;
            a.Y = b.Y - c.Y;
            a.Z = b.Z - c.Z;
            a.A = b.A - c.A;
            a.B = b.B - c.B;
            a.C = b.C - c.C;
            return a;
        }
        public static bool AlmostEqual(xyzPoint a, xyzPoint b)
        {
       //     return (Math.Abs(a.X - b.X) <= grbl.resolution) && (Math.Abs(a.Y - b.Y) <= grbl.resolution) && (Math.Abs(a.Z - b.Z) <= grbl.resolution);
            return (gcode.isEqual(a.X, b.X) && gcode.isEqual(a.Y, b.Y) && gcode.isEqual(a.Z, b.Z));
        }


        public string Print(bool singleLines, bool full=false)
        {
            bool ctrl4thUse = Properties.Settings.Default.ctrl4thUse;
            string ctrl4thName = Properties.Settings.Default.ctrl4thName;

            if (!full)
            {
                if (ctrl4thUse || grbl.axisA)
                    if (singleLines)
                        return string.Format("X={0,9:0.000}\rY={1,9:0.000}\rZ={2,9:0.000}\r{3}={4,9:0.000}", X, Y, Z, ctrl4thName, A);
                    else
                        return string.Format("X={0,9:0.000}  Y={1,9:0.000}  Z={2,9:0.000}\r{3}={4,9:0.000}", X, Y, Z, ctrl4thName, A);

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
    public struct xyPoint
    {
        public double X, Y;
        public xyPoint(double x, double y)
        { X = x; Y = y; }
        public xyPoint(xyPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(Point tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(xyzPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public static explicit operator xyPoint(Point tmp)
        { return new xyPoint(tmp); }
        public static explicit operator xyPoint(xyzPoint tmp)
        { return new xyPoint(tmp); }
        public static explicit operator xyPoint(xyArcPoint tmp)
        { return new xyPoint(tmp.X,tmp.Y); }

        public Point ToPoint()
        { return new Point((int)X, (int)Y); }

        public double DistanceTo(xyPoint anotherPoint)
        {
            double distanceCodeX = X - anotherPoint.X;
            double distanceCodeY = Y - anotherPoint.Y;
            return Math.Sqrt(distanceCodeX * distanceCodeX + distanceCodeY * distanceCodeY);
        }
        public double AngleTo(xyPoint anotherPoint)
        {
            double distanceX = anotherPoint.X - X;
            double distanceY = anotherPoint.Y - Y;
            double radius = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            if (radius == 0) { return 0; }
            double cosinus = distanceX / radius;
            if (cosinus > 1) { cosinus = 1; }
            if (cosinus < -1) { cosinus = -1; }
            double angle = 180 * (float)(Math.Acos(cosinus) / Math.PI);
            if (distanceY > 0) { angle = -angle; }
            return angle;
        }

        // Overload + operator 
        public static xyPoint operator +(xyPoint b, xyPoint c)
        {   xyPoint a = new xyPoint();
            a.X = b.X + c.X;
            a.Y = b.Y + c.Y;
            return a;
        }
        // Overload - operator 
        public static xyPoint operator -(xyPoint b, xyPoint c)
        {
            xyPoint a = new xyPoint();
            a.X = b.X - c.X;
            a.Y = b.Y - c.Y;
            return a;
        }
        // Overload / operator 
        public static xyPoint operator /(xyPoint b, double c)
        {
            xyPoint a = new xyPoint();
            a.X = b.X / c;
            a.Y = b.Y / c;
            return a;
        }
    };
    public struct xyArcPoint
    {
        public double X, Y, CX, CY;
        public byte mode;
        public xyArcPoint(double x, double y, double cx, double cy, byte m)
        {
            X = x; Y = y; CX = cx; CY = cy; mode = m;
        }
        public xyArcPoint(xyPoint tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public xyArcPoint(Point tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public xyArcPoint(xyzPoint tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public xyArcPoint(xyzabcuvwPoint tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public static explicit operator xyArcPoint(Point tmp)
        {
            return new xyArcPoint(tmp);
        }
        public static explicit operator xyArcPoint(xyzPoint tmp)
        {
            return new xyArcPoint(tmp);
        }
        public static explicit operator xyArcPoint(xyPoint tmp)
        {
            return new xyArcPoint(tmp);
        }
    }
    /// <summary>
    /// calculate overall dimensions of drawing
    /// </summary>
    public class Dimensions
    {
        public double minx, maxx, miny, maxy, minz, maxz;
        public double dimx, dimy, dimz;

        public Dimensions()
        { resetDimension(); }
        public void setDimensionXYZ(double? x, double? y, double? z)
        {
            if (x != null) { setDimensionX((double)x); }
            if (y != null) { setDimensionY((double)y); }
            if (z != null) { setDimensionZ((double)z); }
        }
        public void setDimensionXY(double? x, double? y)
        {
            if (x != null) { setDimensionX((double)x); }
            if (y != null) { setDimensionY((double)y); }
        }
        public void setDimensionX(double value)
        {
            minx = Math.Min(minx, value);
            maxx = Math.Max(maxx, value);
            dimx = maxx - minx;
        }
        public void setDimensionY(double value)
        {
            miny = Math.Min(miny, value);
            maxy = Math.Max(maxy, value);
            dimy = maxy - miny;
        }
        public void setDimensionZ(double value)
        {
            minz = Math.Min(minz, value);
            maxz = Math.Max(maxz, value);
            dimz = maxz - minz;
        }
        // calculate min/max dimensions of a circle
        public void setDimensionCircle(double x, double y, double radius, double start, double delta)
        {
            double end = start + delta;
            if (delta > 0)
            {
                for (double i = start; i < end; i += 5)
                {
                    setDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                    setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                }
            }
            else
            {
                for (double i = start; i > end; i -= 5)
                {
                    setDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                    setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                }
            }

        }
        public void resetDimension()
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
        }

        public xyPoint getCenter()
        {   double cx = minx + ((maxx - minx) / 2);
            double cy = miny + ((maxy - miny) / 2);
            return new xyPoint(cx, cy);
        }

        // return string with dimensions
        public String getMinMaxString()
        {
            string x = String.Format("X:{0,8:####0.000} |{1,8:####0.000}\r\n", minx, maxx);
            string y = String.Format("Y:{0,8:####0.000} |{1,8:####0.000}\r\n", miny, maxy);
            string z = String.Format("Z:{0,8:####0.000} |{1,8:####0.000}", minz, maxz);
            if ((minx == Double.MaxValue) || (maxx == Double.MinValue))
                x = "X: unknown | unknown\r\n";
            if ((miny == Double.MaxValue) || (maxy == Double.MinValue))
                y = "Y: unknown | unknown\r\n";
            if ((minz == Double.MaxValue) || (maxz == Double.MinValue))
                z = "";// z = "Z: unknown | unknown";
            return "    Min.   | Max.\r\n" + x + y + z;
        }
        public bool withinLimits(xyzPoint actualMachine, xyzPoint actualWorld)
        {
            return (withinLimits(actualMachine, minx - actualWorld.X, miny - actualWorld.Y) && withinLimits(actualMachine, maxx - actualWorld.X, maxy - actualWorld.Y));
        }
        public bool withinLimits(xyzPoint actualMachine, double tstx, double tsty)
        {
            double minlx = (double)Properties.Settings.Default.machineLimitsHomeX;
            double maxlx = minlx + (double)Properties.Settings.Default.machineLimitsRangeX;
            double minly = (double)Properties.Settings.Default.machineLimitsHomeY;
            double maxly = minly + (double)Properties.Settings.Default.machineLimitsRangeY;
            tstx += actualMachine.X;
            tsty += actualMachine.Y;
            if ((tstx < minlx) || (tstx > maxlx))
                return false;
            if ((tsty < minly) || (tsty > maxly))
                return false;
            return true;
        }
    }


    /*   class eventArgsTemplates    // just copy and paste 
       {
           public event EventHandler<XYZEventArgs> RaiseXYZEvent;
           protected virtual void OnRaiseXYZEvent(XYZEventArgs e)
           {
               EventHandler<XYZEventArgs> handler = RaiseXYZEvent;
               if (handler != null)
               {
                   handler(this, e);
               }
           }
       }
       */

    public class CmdEventArgs : EventArgs
    {
        string command;
        public CmdEventArgs(string cmd)
        {
            command = cmd;
        }
        public string Command
        { get { return command; } }
    }

    public class XYEventArgs : EventArgs
    {
        private double angle,scale;
        private xyPoint point;
        string command;
        public XYEventArgs(double a, double s, xyPoint p, string cmd)
        {
            angle = a;
            scale = s;
            point = p;
            command = cmd;
        }
        public XYEventArgs(double a, double x, double y, string cmd)
        {
            angle = a;
            point.X = x;
            point.Y = y;
            command = cmd;
        }
        public double Angle
        { get { return angle; } }
        public double Scale
        { get { return scale; } }
        public xyPoint Point
        { get { return point; } }
        public double PosX
        { get { return point.X; } }
        public double PosY
        { get { return point.Y; } }
        public string Command
        { get { return command; } }
    }

    public class XYZEventArgs : EventArgs
    {
        private double? posX, posY, posZ;
        string command;
        public XYZEventArgs(double? x, double? y, string cmd)
        {
            posX = x;
            posY = y;
            posZ = null;
            command = cmd;
        }
        public XYZEventArgs(double? x, double? y, double? z, string cmd)
        {
            posX = x;
            posY = y;
            posZ = z;
            command = cmd;
        }
        public double? PosX
        {   get { return posX; } }
        public double? PosY
        {   get { return posY; } }
        public double? PosZ
        { get { return posZ; } }
        public string Command
        {   get { return command; } }
    }

    public static class log
    {   public static StringBuilder logText = new StringBuilder();
        public static void Add(string tmp)
        {   logText.AppendLine(tmp); }
        public static string get()
        {   return logText.ToString(); }
        public static void clear()
        { logText.Clear(); }
    }

    public static class unDo
    {
        private static string unDoCode = "";
        private static string unDoAction = "";
        private static MainForm form;
        public static void setCode(string code, string comment, MainForm mform)
        {
            unDoCode = code.ToString();
            unDoAction = comment;
            form = mform;
            form.setUndoText("Undo '" + comment + "'");
        }
        public static string getCode()
        {   form.setUndoText("");
            return unDoCode;
        }
    }

    sealed class Localization
    {   // https://www.mycsharp.de/wbb2/thread.php?threadid=61039

        private static ResourceManager resMgr;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void UpdateLanguage(string langID)
        {
            try
            {
                //Set Language  
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(langID);
  
                // Init ResourceManager  
                resMgr = new ResourceManager("ResStrings", Assembly.GetExecutingAssembly());
  
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Set culture info");
            }
        }
  
        public static string getString(String pattern)
        {
            //return resMgr.GetString(pattern);
            string tmp = " #not found# ";
            try { tmp = ResStrings.ResourceManager.GetString(pattern).Replace("\\r", Environment.NewLine); }
            catch (Exception ex)
            {
                Logger.Error(ex, "String not found '{0}'",pattern);
            }

            return tmp.Replace("\\n","");
        }
  
    }
}
