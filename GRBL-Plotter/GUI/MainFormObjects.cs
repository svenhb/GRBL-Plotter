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
/*
 * 2020-03-11 add gui class
 * 2020-05-01 add setDimensionArc
 * 2020-07-26 fix setDimensionCircle line 371
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
using Microsoft.Win32;

namespace GRBL_Plotter
{

    public enum LogEnable { Level1=1, Level2=2, Level3=4, Level4=8, Detailed=16, Coordinates=32, Properties=64, Sort = 128, GroupAllGraphics = 256, ClipCode = 512, PathModification = 1024 }

    public static class datapath
    {
        public const string fonts    = "data\\fonts";
        public const string tools    = "\\data\\tools";
        public const string scripts  = "\\data\\scripts";
        public const string usecases = "\\data\\usecases";
        public const string hotkeys  = "\\data\\hotkeys.xml";
        public const string examples = "\\data\\examples";
        public const string extension= "\\data\\extensions";
    }

    public static class gui
    {
        public static Dictionary<string, double> variable = new Dictionary<string, double>();
        public static string insertVariable(string line)//, Dictionary<string, double> variable)
        {
            if (line.Length > 5)        // min length needed to be replaceable: x#TOLX
            {
                int pos = 0, posold = 0;
                double myvalue;
                string myvar, mykey;
                do
                {
                    pos = line.IndexOf('#', posold);
                    if (pos > 0)
                    {
                        myvalue = 0;
                        myvar = line.Substring(pos, 5);
                        mykey = myvar.Substring(1);
                        if (variable.ContainsKey(mykey))
                        { myvalue = variable[mykey]; }
                        else { line += " (" + mykey + " not found)"; }
                        line = line.Replace(myvar, string.Format("{0:0.000}", myvalue));
                        //                  addToLog("replace "+ mykey+" by "+ myvalue.ToString());
                    }
                    posold = pos + 5;
                } while (pos > 0);
            }
            return line.Replace(',', '.');
        }
        public static void resetVariables()
        {
            variable.Clear();
            variable.Add("GMIX", 0.0); // Graphic Minimum X
            variable.Add("GMAX", 0.0); // Graphic Maximum X
            variable.Add("GMIY", 0.0); // Graphic Minimum Y
            variable.Add("GMAY", 0.0); // Graphic Maximum Y
            variable.Add("GMIZ", 0.0); // Graphic Minimum Z
            variable.Add("GMAZ", 0.0); // Graphic Maximum Z
        }

        public static void writeSettingsToRegistry()
        {
            const string reg_key0 = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";
            const string reg_key = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter\\GCodeSettings";
            try
            {
                Registry.SetValue(reg_key, "DecimalPlaces", Properties.Settings.Default.importGCDecPlaces);
                Registry.SetValue(reg_key, "XY_FeedRate", Properties.Settings.Default.importGCXYFeed);
                Registry.SetValue(reg_key, "Z_FeedRate", Properties.Settings.Default.importGCZFeed);
                Registry.SetValue(reg_key, "Z_Save", Properties.Settings.Default.importGCZUp);
                Registry.SetValue(reg_key, "Z_Engrave", Properties.Settings.Default.importGCZDown);
                Registry.SetValue(reg_key, "SpindleSpeed", Properties.Settings.Default.guiSpindleSpeed);
                Registry.SetValue(reg_key, "SpindleDelay", Properties.Settings.Default.importGCSpindleDelay);
                Registry.SetValue(reg_key, "GcodeHeader", Properties.Settings.Default.importGCHeader);
                Registry.SetValue(reg_key, "GcodeFooter", Properties.Settings.Default.importGCFooter);
                Registry.SetValue(reg_key0, "Update", 0, RegistryValueKind.DWord);
            }
            catch { };//            Logger.Error(er, "writeSettingsToRegistry"); }
        }
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
        public xyPoint(System.Windows.Point xy)
        { X = xy.X; Y = xy.Y; }
        public xyPoint(xyPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(Point tmp)
        { X = tmp.X; Y = tmp.Y; }
        public xyPoint(xyzPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public static explicit operator xyPoint(Point tmp)
        { return new xyPoint(tmp); }
        public static explicit operator xyPoint(System.Windows.Point tmp)
        { return new xyPoint(tmp); }
        public static explicit operator xyPoint(xyzPoint tmp)
        { return new xyPoint(tmp); }
        public static explicit operator xyPoint(xyArcPoint tmp)
        { return new xyPoint(tmp.X,tmp.Y); }

        public System.Windows.Point ToPointDouble()
        { return new System.Windows.Point(X, Y); }
        public Point ToPoint()
        { return new Point((int)X, (int)Y); }

        //       public static explicit operator System.Windows.Point(xyPoint tmp) => new System.Windows.Point(tmp.X,tmp.Y);

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
        public xyPoint Round(int decimals = 4)
        {   X = Math.Round(X, decimals);
            Y = Math.Round(Y, decimals);
            return this;
        }

        public static xyPoint Round(xyPoint tmpIn, int decimals = 4)
        {   xyPoint tmpOut = new xyPoint();
            tmpOut.X = Math.Round(tmpIn.X,decimals);
            tmpOut.Y = Math.Round(tmpIn.Y, decimals);
            return tmpOut;
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
        // Overload * operator 
        public static xyPoint operator *(xyPoint b, double c)
        {
            xyPoint a = new xyPoint();
            a.X = b.X * c;
            a.Y = b.Y * c;
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

        public Dimensions(Dimensions old)
        {   Dimensions n = new Dimensions();
            n.minx = old.minx; n.maxx = old.maxx; n.miny = old.miny; n.maxy = old.maxy;
            n.minz = old.minz; n.maxz = old.maxz;
            n.dimx = old.dimx; n.dimy = old.dimy; n.dimz = old.dimz;
        }

            public Dimensions()
        { resetDimension(); }

        public void addDimensionXY(Dimensions tmp)
        {   setDimensionXY(tmp.minx, tmp.miny);
            setDimensionXY(tmp.maxx, tmp.maxy);
        }
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
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            minx = Math.Min(minx, value);
            maxx = Math.Max(maxx, value);
            dimx = maxx - minx;
        }
        public void setDimensionY(double value)
        {
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            miny = Math.Min(miny, value);
            maxy = Math.Max(maxy, value);
            dimy = maxy - miny;
        }
        public void setDimensionZ(double value)
        {
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            minz = Math.Min(minz, value);
            maxz = Math.Max(maxz, value);
            dimz = maxz - minz;
        }
        public void offsetXY(double x, double y)
        { minx += x; maxx += x; miny += y; maxy += y;}
        public void scaleXY(double scaleX, double scaleY)
        { minx *= scaleX; maxx *= scaleX; miny *= scaleY; maxy *= scaleY; dimx = maxx - minx; dimy = maxy - miny; }

        public double getArea()
        {   return dimx* dimy;  }

        // calculate min/max dimensions of a circle
        public void setDimensionCircle(double x, double y, double radius, double startDeg, double deltaDeg)
        {   double end = startDeg + deltaDeg;
            double i = startDeg;

            if (Math.Abs(Math.Abs(deltaDeg) - 360) < 0.00001)
            {
                setDimensionXY(x - radius, y - radius);
                setDimensionXY(x + radius, y + radius);
            }
            else
            {
                setDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                i = end;
                setDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));

                for (int k = -360; k <= 360; k += 90)
                {
                    if (deltaDeg > 0)
                    {
                        if ((k > startDeg) && (k < end))
                        {
                            i = k;
                            setDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                            setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                        }
                    }
                    else
                    {
                        if ((k < startDeg) && (k > end))
                        {
                            i = k;
                            setDimensionX(x + radius * Math.Cos(i / 180 * Math.PI));
                            setDimensionY(y + radius * Math.Sin(i / 180 * Math.PI));
                        }
                    }
                }
            }
        }

        public void setDimensionArc(xyPoint oldPos, xyPoint newPos, double i, double j, bool isG2)
        {
            ArcProperties arcMove;
            arcMove = gcodeMath.getArcMoveProperties(oldPos, newPos, i, j, isG2);

            float x1 = (float)(arcMove.center.X - arcMove.radius);
            float x2 = (float)(arcMove.center.X + arcMove.radius);
            float y1 = (float)(arcMove.center.Y - arcMove.radius);
            float y2 = (float)(arcMove.center.Y + arcMove.radius);
            float r2 = 2 * (float)arcMove.radius;
            float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
            float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);

            if (gcodeMath.isEqual(oldPos, newPos))
            {   setDimensionXY(x1, y1);
                setDimensionXY(x2, y2);
            }
            else
                setDimensionCircle(arcMove.center.X, arcMove.center.Y, arcMove.radius, aStart, aDiff);        // calculate new dimensions
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
