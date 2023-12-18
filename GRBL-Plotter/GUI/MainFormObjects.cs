/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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
 * 2020-09-30 Preset variable GMIZ and GMAZ with Properties.Settings.Default.importGCZUp and -Down
 * 2021-07-27 code clean up / code quality
 * 2021-11-22 change AppDataFolder start-path
 * 2021-11-23 line 688 add check (form != null)
 * 2021-11-29 add SaveEncoding array
 * 2021-12-14 add log path 
 * 2022-01-02 add MakeAbsolutePath (from ControlSetup.cs)
 * 2022-11-24 line 99 InsertVariable check length
 * 2023-03-04 l:68 f:Datapath add path for "data"
 * 2023-03-04 l:68 f:Datapath add path for "filter"
*/

using GrblPlotter.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace GrblPlotter
{
    [Flags]
    public enum LogEnables { None = 0, Level1 = 1, Level2 = 2, Level3 = 4, Level4 = 8, Detailed = 16, Coordinates = 32, Properties = 64, Sort = 128, GroupAllGraphics = 256, ClipCode = 512, PathModification = 1024 }

    public static class MyApplication
    {
        private static readonly string VersionAddOn = "";

        public static string GetVersion()
        { return System.Windows.Forms.Application.ProductVersion.ToString() + VersionAddOn; }

        /* date/time of compilation */
        public static string GetCompilationDate()
        { return GetLinkerTimestampUtc(System.Reflection.Assembly.GetExecutingAssembly()).ToString("yyyy-MM-dd"); }
        public static DateTime GetLinkerTimestampUtc(System.Reflection.Assembly assembly)
        {
            var location = assembly.Location;
            if (!string.IsNullOrEmpty(location))
                return GetLinkerTimestampUtc(location);
            else
                return DateTime.MinValue;
        }
        public static DateTime GetLinkerTimestampUtc(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var bytes = new byte[2048];

            try
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    file.Read(bytes, 0, bytes.Length);
                }
            }
            catch
            { return DateTime.MinValue; }

            var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(secondsSince1970);
        }

    }

    public static class Datapath
    {   // https://stackoverflow.com/questions/66430190/how-do-i-get-access-to-c-program-files-in-c-sharp
        internal static string Application = System.Windows.Forms.Application.StartupPath;
        //        public static string AppDataFolder = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.CommonAppDataPath);   // without vers.Nr
        // https://stackoverflow.com/questions/10563148/where-is-the-correct-place-to-store-my-application-specific-data#:~:text=AppData%20(maps%20to%20C%3A%5C,their%20save%20games%20into%20Environment.
        // https://docs.microsoft.com/en-us/dotnet/api/system.environment.specialfolder?view=netframework-4.7.2
        internal static string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);   // will be changed by MainFormUpdate()-GetAppDataPath
        public static string Automations { get => AppDataFolder + "\\data\\automations"; }
        public static string Fonts { get => AppDataFolder + "\\data\\fonts"; }
        public static string Tools { get => AppDataFolder + "\\data\\tools"; }
        public static string Scripts { get => AppDataFolder + "\\data\\scripts"; }
        public static string Usecases { get => AppDataFolder + "\\data\\usecases"; }
        public static string Hotkeys { get => AppDataFolder + "\\data\\hotkeys.xml"; }
        public static string Examples { get => AppDataFolder + "\\data\\examples"; }
        public static string Extension { get => AppDataFolder + "\\data\\extensions"; }
        public static string Buttons { get => AppDataFolder + "\\data\\buttons"; }
        public static string Jogpath { get => AppDataFolder + "\\data\\jogpaths"; }
        public static string Filter { get => AppDataFolder + "\\data\\filters"; }
        public static string RecentFile { get => AppDataFolder + "\\Recent.txt"; }
        public static string LogFiles { get => AppDataFolder + "\\logfiles"; }
        public static string Data { get => AppDataFolder + "\\data"; }


        public static string MakeAbsolutePath(string fileName)
        {
            if (fileName.ToLower().StartsWith("http"))
            { return fileName; }
            if (fileName.ToLower().StartsWith("ftp"))
            { return fileName; }

            if (Path.IsPathRooted(fileName))
            { return fileName; }

            return Path.Combine(Datapath.AppDataFolder, fileName);
        }
    }

    public static class GuiVariables
    {
        // origin of imported file onload = Properties.Settings.Default.importGraphicOffsetOriginX
        internal static double offsetOriginX = 0;
        internal static double offsetOriginY = 0;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static Encoding[] SaveEncoding = { Encoding.Unicode, Encoding.ASCII, Encoding.UTF8, Encoding.GetEncoding("iso-8859-1") };
        internal static Dictionary<string, double> variable = new Dictionary<string, double>();
        public static string InsertVariable(string line)//, Dictionary<string, double> variable)
        {
            if (!string.IsNullOrEmpty(line) && line.Length > 5)        // min length needed to be replaceable: x#TOLX
            {
                int pos, posold = 0;
                double myvalue;
                string myvar, mykey;
                int safetyExit = 6;
                do
                {
                    pos = line.IndexOf('#', posold);				// not found, pos = -1
                    if (pos > 0)
                    {
                        if (pos <= (line.Length - 5))				// max pos exceeded?
                        {
                            myvar = line.Substring(pos, 5);
                            mykey = myvar.Substring(1);             // get variable

                            if (variable.ContainsKey(mykey))
                            {
                                myvalue = variable[mykey];
                                line = line.Replace(myvar, string.Format("{0:0.000}", myvalue));
                            }
                            else
                            { Logger.Error("⚠⚠⚠ InsertVariable '{0}' not found in '{1}'", mykey, line); }
                        }
                        else
                        { Logger.Error("⚠⚠⚠ InsertVariable pos:{0} string is too short in '{1}'", pos, line); pos = -1; }
                    }
                    posold = pos + 5;
                } while ((pos > 0) && (safetyExit-- > 0));
            }
            return line.Replace(',', '.');
        }
        public static void ResetVariables()	// used in MainForm-sendCommand-376 and MainFormLoadFile-setGcodeVariables 1617
        {
            offsetOriginX = (double)Properties.Settings.Default.importGraphicOffsetOriginX;
            offsetOriginY = (double)Properties.Settings.Default.importGraphicOffsetOriginY;

            variable.Clear();
            variable.Add("GMIX", 0.0); // Graphic Minimum X
            variable.Add("GMAX", 0.0); // Graphic Maximum X
            variable.Add("GCTX", 0.0); // Graphic Center X
            variable.Add("GMIY", 0.0); // Graphic Minimum Y
            variable.Add("GMAY", 0.0); // Graphic Maximum Y
            variable.Add("GCTY", 0.0); // Graphic Center Y
            variable.Add("GMIZ", (double)Properties.Settings.Default.importGCZDown); 	// Graphic Minimum Z
            variable.Add("GMAZ", (double)Properties.Settings.Default.importGCZUp); 		// Graphic Maximum Z
            variable.Add("GCTZ", (double)(Properties.Settings.Default.importGCZDown + Properties.Settings.Default.importGCZUp) / 2); // Graphic Center Z
            variable.Add("GMIS", (double)Properties.Settings.Default.importGCPWMDown);
            variable.Add("GMAS", (double)Properties.Settings.Default.importGCPWMUp);
            variable.Add("GZES", (double)Properties.Settings.Default.importGCPWMZero);
            variable.Add("GCTS", (double)(Properties.Settings.Default.importGCPWMDown + Properties.Settings.Default.importGCPWMUp) / 2);
            WriteDimensionToRegistry();
        }

        public static void WriteDimensionToRegistry()
        {
            const string reg_key0 = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";
            const string reg_key = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter\\Dimension";
            try
            {
                Registry.SetValue(reg_key, "GMIX", variable["GMIX"]);
                Registry.SetValue(reg_key, "GMAX", variable["GMAX"]);
                Registry.SetValue(reg_key, "GCTX", variable["GCTX"]);
                Registry.SetValue(reg_key, "GMIY", variable["GMIY"]);
                Registry.SetValue(reg_key, "GMAY", variable["GMAY"]);
                Registry.SetValue(reg_key, "GCTY", variable["GCTY"]);
                Registry.SetValue(reg_key0, "offsetX", "0.0");
                Registry.SetValue(reg_key0, "offsetY", "0.0");
                Registry.SetValue(reg_key0, "rotate", "0.0");
            }
            catch (Exception Ex) { Logger.Error(Ex, "WriteDimensionToRegistry "); };
        }
        public static void WriteSettingsToRegistry()
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
                Registry.SetValue(reg_key0, "Update", 0, RegistryValueKind.DWord);		// will be checked in MainForm-MainTimer_Tick
            }
            catch (Exception Ex) { Logger.Error(Ex, "WriteSettingsToRegistry "); };
        }
        public static void WritePositionToRegistry()
        {
            //            const string reg_key0 = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";
            const string reg_key = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter\\Position";
            try
            {
                Registry.SetValue(reg_key, "Work_X", Grbl.posWork.X);
                Registry.SetValue(reg_key, "Work_Y", Grbl.posWork.Y);
                Registry.SetValue(reg_key, "Work_Z", Grbl.posWork.Z);
                Registry.SetValue(reg_key, "Work_A", Grbl.posWork.A);
                Registry.SetValue(reg_key, "Work_B", Grbl.posWork.B);
                Registry.SetValue(reg_key, "Work_C", Grbl.posWork.C);

                Registry.SetValue(reg_key, "Machine_X", Grbl.posMachine.X);
                Registry.SetValue(reg_key, "Machine_Y", Grbl.posMachine.Y);
                Registry.SetValue(reg_key, "Machine_Z", Grbl.posMachine.Z);
                Registry.SetValue(reg_key, "Machine_A", Grbl.posMachine.A);
                Registry.SetValue(reg_key, "Machine_B", Grbl.posMachine.B);
                Registry.SetValue(reg_key, "Machine_C", Grbl.posMachine.C);
                //                Registry.SetValue(reg_key0, "Update", 0, RegistryValueKind.DWord);
            }
            catch (Exception Ex) { Logger.Error(Ex, "WritePositionToRegistry "); };
        }
    }

    internal struct XyzPoint
    {
        public double X, Y, Z, A, B, C;
        public XyzPoint(XyzPoint xy)
        { X = xy.X; Y = xy.Y; Z = xy.Z; A = xy.A; B = xy.B; C = xy.C; }
        public XyzPoint(XyPoint xy, double z)
        { X = xy.X; Y = xy.Y; Z = z; A = 0; B = 0; C = 0; }
        public XyzPoint(XyPoint xy, double z, double a)
        { X = xy.X; Y = xy.Y; Z = z; A = a; B = 0; C = 0; }
        public XyzPoint(double x, double y, double z)
        { X = x; Y = y; Z = z; A = 0; B = 0; C = 0; }
        public XyzPoint(double x, double y, double z, double a)
        { X = x; Y = y; Z = z; A = a; B = 0; C = 0; }
        public XyzPoint(double x, double y, double z, double a, double b, double c)
        { X = x; Y = y; Z = z; A = a; B = b; C = c; }
        // Overload + operator 
        public static XyzPoint operator +(XyzPoint b, XyzPoint c)
        {
            XyzPoint a = new XyzPoint
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
        public static XyzPoint operator -(XyzPoint b, XyzPoint c)
        {
            XyzPoint a = new XyzPoint
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
        public static bool AlmostEqual(XyzPoint a, XyzPoint b)
        {
            //     return (Math.Abs(a.X - b.X) <= grbl.resolution) && (Math.Abs(a.Y - b.Y) <= grbl.resolution) && (Math.Abs(a.Z - b.Z) <= grbl.resolution);
            return (Gcode.IsEqual(a.X, b.X) && Gcode.IsEqual(a.Y, b.Y) && Gcode.IsEqual(a.Z, b.Z));
        }
        public PointF ToPointF()
        { return new PointF((float)X, (float)Y); }


        public string Print(bool singleLines, bool full)
        {
            bool ctrl4thUse = Properties.Settings.Default.ctrl4thUse;
            string ctrl4thName = Properties.Settings.Default.ctrl4thName;

            if (!full)
            {
                if (ctrl4thUse || Grbl.axisA)
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
    internal struct XyPoint
    {
        public double X, Y;
        public XyPoint(double x, double y)
        { X = x; Y = y; }
        public XyPoint(System.Windows.Point xy)
        { X = xy.X; Y = xy.Y; }
        public XyPoint(XyPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public XyPoint(Point tmp)
        { X = tmp.X; Y = tmp.Y; }
        public XyPoint(System.Drawing.PointF tmp)
        { X = tmp.X; Y = tmp.Y; }
        public XyPoint(XyzPoint tmp)
        { X = tmp.X; Y = tmp.Y; }
        public static explicit operator XyPoint(Point tmp)
        { return new XyPoint(tmp); }
        public static explicit operator XyPoint(PointF tmp)
        { return new XyPoint(tmp); }
        public static explicit operator XyPoint(System.Windows.Point tmp)
        { return new XyPoint(tmp); }
        public static explicit operator XyPoint(XyzPoint tmp)
        { return new XyPoint(tmp); }
        public static explicit operator XyPoint(XyArcPoint tmp)
        { return new XyPoint(tmp.X, tmp.Y); }

        public System.Windows.Point ToPointDouble()
        { return new System.Windows.Point(X, Y); }
        public Point ToPoint()
        { return new Point((int)X, (int)Y); }

        //       public static explicit operator System.Windows.Point(xyPoint tmp) => new System.Windows.Point(tmp.X,tmp.Y);

        public double DistanceTo(XyPoint anotherPoint)
        {
            double distanceCodeX = X - anotherPoint.X;
            double distanceCodeY = Y - anotherPoint.Y;
            return Math.Sqrt(distanceCodeX * distanceCodeX + distanceCodeY * distanceCodeY);
        }
        public double AngleTo(XyPoint anotherPoint)
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
        public XyPoint Round()
        {
            int decimals = 4;
            X = Math.Round(X, decimals);
            Y = Math.Round(Y, decimals);
            return this;
        }

        /*    public static xyPoint Round(xyPoint tmpIn, int decimals = 4)
            {   xyPoint tmpOut = new xyPoint();
                tmpOut.X = Math.Round(tmpIn.X,decimals);
                tmpOut.Y = Math.Round(tmpIn.Y, decimals);
                return tmpOut;
            }*/
        // Overload + operator 
        public static XyPoint operator +(XyPoint b, XyPoint c)
        {
            XyPoint a = new XyPoint
            {
                X = b.X + c.X,
                Y = b.Y + c.Y
            };
            return a;
        }
        // Overload - operator 
        public static XyPoint operator -(XyPoint b, XyPoint c)
        {
            XyPoint a = new XyPoint
            {
                X = b.X - c.X,
                Y = b.Y - c.Y
            };
            return a;
        }
        // Overload * operator 
        public static XyPoint operator *(XyPoint b, double c)
        {
            XyPoint a = new XyPoint
            {
                X = b.X * c,
                Y = b.Y * c
            };
            return a;
        }
        // Overload / operator 
        public static XyPoint operator /(XyPoint b, double c)
        {
            XyPoint a = new XyPoint
            {
                X = b.X / c,
                Y = b.Y / c
            };
            return a;
        }
    };
    internal struct XyArcPoint
    {
        public double X, Y, CX, CY;
        public byte mode;
        public XyArcPoint(double x, double y, double cx, double cy, byte m)
        {
            X = x; Y = y; CX = cx; CY = cy; mode = m;
        }
        public XyArcPoint(XyPoint tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public XyArcPoint(Point tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public XyArcPoint(XyzPoint tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public XyArcPoint(XyzabcuvwPoint tmp)
        {
            X = tmp.X; Y = tmp.Y; CX = 0; CY = 0; mode = 0;
        }
        public static explicit operator XyArcPoint(Point tmp)
        {
            return new XyArcPoint(tmp);
        }
        public static explicit operator XyArcPoint(XyzPoint tmp)
        {
            return new XyArcPoint(tmp);
        }
        public static explicit operator XyArcPoint(XyPoint tmp)
        {
            return new XyArcPoint(tmp);
        }
    }

    internal struct ImgPoint
    {
        public float X;
        public float Y;
        public int brightnes;
        public ImgPoint(float x, float y, int z)
        { X = x; Y = y; brightnes = z; }
        //    public ImgPoint(float x, float y)
        //    { X = x; Y = y; brightnes = -1; }
        public double DistanceTo(ImgPoint anotherPoint)
        {
            double distanceCodeX = X - anotherPoint.X;
            double distanceCodeY = Y - anotherPoint.Y;
            return Math.Sqrt(distanceCodeX * distanceCodeX + distanceCodeY * distanceCodeY);
        }
    }

    /// <summary>
    /// calculate overall dimensions of drawing
    /// </summary>
    internal class Dimensions
    {
        public double minx, maxx, miny, maxy, minz, maxz;
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
        }
        public void SetDimensionY(double value)
        {
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            miny = Math.Min(miny, value);
            maxy = Math.Max(maxy, value);
            dimy = maxy - miny;
        }
        public void SetDimensionZ(double value)
        {
            if ((value == Double.MaxValue) || (value == Double.MinValue))
                return;
            minz = Math.Min(minz, value);
            maxz = Math.Max(maxz, value);
            dimz = maxz - minz;
        }
        public void OffsetXY(double x, double y)
        { minx += x; maxx += x; miny += y; maxy += y; }
        public void ScaleXY(double scaleX, double scaleY)
        { minx *= scaleX; maxx *= scaleX; miny *= scaleY; maxy *= scaleY; dimx = maxx - minx; dimy = maxy - miny; }

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

        public void SetDimensionArc(XyPoint oldPos, XyPoint newPos, double i, double j, bool isG2)
        {
            ArcProperties arcMove;
            arcMove = GcodeMath.GetArcMoveProperties(oldPos, newPos, i, j, isG2);

            float x1 = (float)(arcMove.center.X - arcMove.radius);
            float x2 = (float)(arcMove.center.X + arcMove.radius);
            float y1 = (float)(arcMove.center.Y - arcMove.radius);
            float y2 = (float)(arcMove.center.Y + arcMove.radius);
            //   float r2 = 2 * (float)arcMove.radius;
            float aStart = (float)(arcMove.angleStart * 180 / Math.PI);
            float aDiff = (float)(arcMove.angleDiff * 180 / Math.PI);

            if (GcodeMath.IsEqual(oldPos, newPos))
            {
                SetDimensionXY(x1, y1);
                SetDimensionXY(x2, y2);
            }
            else
                SetDimensionCircle(arcMove.center.X, arcMove.center.Y, arcMove.radius, aStart, aDiff);        // calculate new dimensions
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
        public bool WithinLimits(XyzPoint actualMachine, XyzPoint actualWorld)
        {
            return (WithinLimits(actualMachine, minx - actualWorld.X, miny - actualWorld.Y) && WithinLimits(actualMachine, maxx - actualWorld.X, maxy - actualWorld.Y));
        }
        public static bool WithinLimits(XyzPoint actualMachine, double tstx, double tsty)
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

        public bool IsWithin(Dimensions tmp)
        {
            return ((minx >= tmp.minx) && (miny >= tmp.miny) && (maxx <= tmp.maxx) && (maxy <= tmp.maxy));
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


    public class ProcessEventArgs : EventArgs
    {
        private readonly string command;
        private readonly string value;
        public ProcessEventArgs(string cmd, string val)
        {
            command = cmd;
            value = val;
        }
        public string Command
        { get { return command; } }
        public string Value
        { get { return value; } }
    }

    public class CmdEventArgs : EventArgs
    {
        private readonly string command;
        public CmdEventArgs(string cmd)
        {
            command = cmd;
        }
        public string Command
        { get { return command; } }
    }

    public class XYEventArgs : EventArgs
    {
        private readonly double angle, scale;
        private XyPoint point;
        private readonly string command;
        internal XYEventArgs(double a, double s, XyPoint p, string cmd)
        {
            angle = a;
            scale = s;
            point = p;
            command = cmd;
        }
        public XYEventArgs(double angl, double px, double py, string cmd)
        {
            angle = angl;
            point.X = px;
            point.Y = py;
            command = cmd;
        }
        public double Angle
        { get { return angle; } }
        public double Scale
        { get { return scale; } }
        internal XyPoint Point
        { get { return point; } }
        public double PosX
        { get { return point.X; } }
        public double PosY
        { get { return point.Y; } }
        public string Command
        { get { return command; } }
    }

    public class XyzEventArgs : EventArgs
    {
        private readonly double? posX, posY, posZ;
        private readonly string command;
        public XyzEventArgs(double? px, double? py, string cmd)
        {
            posX = px;
            posY = py;
            posZ = null;
            command = cmd;
        }
        public XyzEventArgs(double? px, double? py, double? pz, string cmd)
        {
            posX = px;
            posY = py;
            posZ = pz;
            command = cmd;
        }
        public double? PosX
        { get { return posX; } }
        public double? PosY
        { get { return posY; } }
        public double? PosZ
        { get { return posZ; } }
        public string Command
        { get { return command; } }
    }

    public static class UnDo
    {
        private static string unDoCode = "";
        //    private static string unDoAction = "";
        private static MainForm form;
        public static void SetCode(string code, string comment, MainForm mform)
        {
            if (!string.IsNullOrEmpty(code))
            {
                unDoCode = code.ToString();
                //        unDoAction = comment;
                if (mform == null) return;
                form = mform;
                form.SetUndoText("Undo '" + comment + "'");
            }
        }
        public static string GetCode()
        {
            form?.SetUndoText("");
            return unDoCode;
        }
    }

    public static class Localization
    {   // https://www.mycsharp.de/wbb2/thread.php?threadid=61039

        //   private static ResourceManager resMgr;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void UpdateLanguage(string langId)
        {
            try
            {       //Set Language  
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(langId);
                Logger.Info("UpdateLanguage {0} {1}", langId, Thread.CurrentThread.CurrentUICulture);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "UpdateLanguage Set culture info ");
            }
        }

        public static string GetString(String pattern)
        {
            string tmp = " #not found# ";
            try { tmp = ResStrings.ResourceManager.GetString(pattern).Replace("\\r", Environment.NewLine); }
            catch (Exception ex)
            {
                Logger.Error(ex, "GetString String not found '{0}'", pattern);
            }
            return tmp.Replace("\\n", "");
        }

    }
}
