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

using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GrblPlotter.Helper
{
    internal class PaletteEntry
    {
        internal Color Col { get; set; }
        internal string Name { get; set; }
        internal int ToolNr { get; set; }
        internal int PixelCount { get; set; }
        internal bool Use { get; set; }
        internal double Diff { get; set; }
    }
    public static class Colors
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static List<PaletteEntry> MyPalette = new List<PaletteEntry>();
        internal static PaletteEntry ExceptionColor { get; set; }
        public static Color ContrastColor(Color color)
        {
            int d;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

        public static Color GrayColor(Color color)
        {
            double a = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            int g = (int)(a * 255);
            return Color.FromArgb(g, g, g);
        }

        /*
            h = 0 - 360
            s = 0 - 1
            v = 0 - 1
        */
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public static Color TryConvertColor(string txt)
        {
            Color ctmp = Color.Black;
            long clr = 0xff000000;
            if (txt.Length > 1)
            {
                ctmp = Color.FromName(txt.Trim());
                if (ctmp.ToArgb() == 0)
                {
                    if (!txt.StartsWith("#"))
                        txt = "#" + txt;
                    ctmp = ColorTranslator.FromHtml(txt);
                }
            }
            return ctmp;
        }

        internal static PaletteEntry ExtractPaletteEntry(string txt, bool isToolTable = false)
        {
            PaletteEntry tmp = new PaletteEntry();
            tmp.Col = Color.FromArgb(0, 0, 0);
            tmp.Name = "black";
            tmp.ToolNr = 1;
            tmp.PixelCount = 0;
            tmp.Use = true;
            tmp.Diff = 0;

            string hex;
            string[] splt, splt2;

            if (txt.Contains("\t"))
            {
                splt = txt.Split('\t');
                tmp.Name = splt[1];
                try
                {
                    if (splt[0].Contains(" "))
                    {
                        splt2 = splt[0].Split(' ');
                        if (splt2.Length == 3)
                        {
                            tmp.Col = Color.FromArgb(int.Parse(splt2[0].Trim()), int.Parse(splt2[1].Trim()), int.Parse(splt2[2].Trim()));
                        }
                        else
                        {
                            tmp.Col = Color.FromArgb(int.Parse(splt[0].Substring(0, 3).Trim()), int.Parse(splt[0].Substring(4, 3).Trim()), int.Parse(splt[0].Substring(8, 3).Trim()));
                        }
                    }
                }
                catch { tmp.Col = Color.Transparent; }
            }
            else if (txt.Contains(","))
            {
                splt = txt.Split(',');
				int indexStart = 0;
				if (isToolTable)
				{ indexStart = 1; }
			
				tmp.Name = splt[indexStart + 1];
				
                try
                {
                    tmp.Col = ColorTranslator.FromHtml('#' + splt[indexStart].Trim());
                }
                catch { tmp.Col = Color.Transparent; }
            }
            else
            {
                tmp.Name = "not set";
                if (txt.Length > 6)
                    txt = txt.Substring(2, 6);
                try
                {
                    tmp.Col = ColorTranslator.FromHtml('#' + txt.Trim());
                }
                catch { tmp.Col = Color.Transparent; }
            }
            return tmp;
        }

        internal static bool LoadColorPalette(string file = "")
        {
            if (file == "")
                file = Datapath.ColorPalette + "\\" + Properties.Settings.Default.colorPaletteLastLoaded;// ToolTable.DefaultFileName;

            bool loadOk = false;
            string[] readText = { "000000,Black", "FF0000,Red", "00FF00,Green", "0000FF,Blue" };
            Logger.Trace("LoadColorPalette {0}", file);
            if (File.Exists(file))
            {
                try
                {
                    readText = File.ReadAllLines(file);
                    loadOk = true;
                }
                catch (IOException err)
                {
                    // read already opened file???
                    // https://stackoverflow.com/questions/9759697/reading-a-file-used-by-another-process
                    EventCollector.StoreException("LoadToolList IOException: " + file + " " + err.Message + " - ");
                    Logger.Error(err, "LoadColorPalette IOException:{0}", file);
                    MessageBox.Show("Could not read " + file + "\r\n" + err.Message, "Error");
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }
            }
            MyPalette.Clear();
            PaletteEntry tmp;
            int toolNr = 1;
            foreach (string line in readText)
            {
                if (line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("/") || line.StartsWith("Gimp") || line.StartsWith("Name") || (line.Length < 3))     // jump over comments
                    continue;

				if (Path.GetExtension(file).ToLower().Contains("csv"))
					tmp = Helper.Colors.ExtractPaletteEntry(line, true);
				else	
					tmp = Helper.Colors.ExtractPaletteEntry(line);
                tmp.ToolNr= toolNr++;
                MyPalette.Add(tmp);
                if (tmp.Col == Color.Transparent)
                { Logger.Error("Error in conversion: {0}", line); }
            }
            return loadOk;
        }

        internal static void SortByPixelCount(bool invert)
        {
            if ((MyPalette == null) || (MyPalette.Count == 0))
            {
                Logger.Warn("SortByPixelCount MyPalette is empty - do Init");
                LoadColorPalette();
            }
            List<PaletteEntry> SortedList;
            if (invert)
                SortedList = MyPalette.OrderBy(o => o.PixelCount).ToList();
            else
                SortedList = MyPalette.OrderByDescending(o => o.PixelCount).ToList();
            MyPalette = SortedList;
        }
        public static void SortByToolNR(bool invert)
        {
            if ((MyPalette == null) || (MyPalette.Count == 0))
            {
                Logger.Warn("SortByToolNR MyPalette is empty - do Init");
                LoadColorPalette();
            }
            List<PaletteEntry> SortedList;
            if (!invert)
                SortedList = MyPalette.OrderBy(o => o.ToolNr).ToList();
            else
                SortedList = MyPalette.OrderByDescending(o => o.ToolNr).ToList();
            MyPalette = SortedList;
        }
        public static void ClearUsage()
        {
            foreach (PaletteEntry pe in MyPalette)  
            {
                pe.Diff = 0;
                pe.Use = false;
                pe.PixelCount = 0;
            }
        }
        public static void SetAllSelected(bool val)
        {
            for (int i = 0; i < MyPalette.Count; i++)   // add colors to AForge filter
            {
                PaletteEntry tmp = MyPalette[i];
                tmp.Use = val;
                MyPalette[i] = tmp;
            }
        }
        public static void SetUse(int index, bool val)
        {
            if ((index >= 0) && (index < MyPalette.Count))
            {
                PaletteEntry tmp = MyPalette[index];
                tmp.Use = val;
                MyPalette[index] = tmp;
            }
        }
        public static int GetIndexByToolNR(int toolNr)
        {
            for (int i = 0; i < MyPalette.Count; i++)
            {
                if (MyPalette[i].ToolNr == toolNr)
                    return i;
            }
            return -1;
        }
        internal static PaletteEntry GetPaletteEntryByToolNR(int toolNr, bool setUse=false)
        {
            if (toolNr == -1)
            { ExceptionColor.PixelCount++; return ExceptionColor; }

            for (int i = 0; i < MyPalette.Count; i++)
            {
                if (MyPalette[i].ToolNr == toolNr)
                {
                    if (setUse)
                    {
                        MyPalette[i].PixelCount++;
                        MyPalette[i].Use = true;
                    }
                    return MyPalette[i];
                }
            }
            return ExceptionColor;// MyPalette[0];
        }

        public static int GetToolNRByColor(Color mycolor, int mode)
        {
            if ((MyPalette == null) || (MyPalette.Count == 0))
            {
                Logger.Warn("GetToolNRByColor toolTableArray is empty - do Init");
                LoadColorPalette();
            }

            int i;/*, start = 1, tmpIndex;
            List<PaletteEntry> SortedList = MyPalette.OrderBy(o => o.ToolNr).ToList();
            MyPalette = SortedList;
       */     //     if (useException) start = 0;  // first element is exception

            for (i = 0; i < MyPalette.Count; i++)
            {
                if (mycolor == MyPalette[i].Col)         // direct hit
                {
             //       tmpIndex = i;
                    return MyPalette[i].ToolNr;
                }
                else if (mode == 0)
                    MyPalette[i].Diff = ColorDiff(mycolor, MyPalette[i].Col);
                else if (mode == 1)
                    MyPalette[i].Diff = GetHueDistance(mycolor.GetHue(), MyPalette[i].Col.GetHue());
                else
                    MyPalette[i].Diff = Math.Abs(ColorNum(MyPalette[i].Col) - ColorNum(mycolor)) +
                                              GetHueDistance(MyPalette[i].Col.GetHue(), mycolor.GetHue());
            }
            List<PaletteEntry>  SortedList = MyPalette.OrderBy(o => o.Diff).ToList();
            MyPalette = SortedList;
       //     tmpIndex = 0;
            return MyPalette[0].ToolNr; ;   // return tool nr of nearest color
        }
        internal static PaletteEntry GetPaletteEntryByColor(Color mycolor, int mode, bool setUse=false)
        {
            if ((MyPalette == null) || (MyPalette.Count == 0))
            {
                Logger.Warn("GetToolNRByColor toolTableArray is empty - do Init");
                LoadColorPalette();
            }
            if (ExceptionColor.Use && (ExceptionColor.Col == mycolor))
            { ExceptionColor.PixelCount++; return ExceptionColor; }

            for (int i = 0; i < MyPalette.Count; i++)
            {
                if (mycolor == MyPalette[i].Col)         // direct hit
                {
                    if (setUse)
                    {
                        MyPalette[i].PixelCount++;
                        MyPalette[i].Use = true;
                    }
                    return MyPalette[i];
                }
                else if (mode == 0)
                    MyPalette[i].Diff = ColorDiff(mycolor, MyPalette[i].Col);
                else if (mode == 1)
                    MyPalette[i].Diff = GetHueDistance(mycolor.GetHue(), MyPalette[i].Col.GetHue());
                else
                    MyPalette[i].Diff = Math.Abs(ColorNum(MyPalette[i].Col) - ColorNum(mycolor)) +
                                              GetHueDistance(MyPalette[i].Col.GetHue(), mycolor.GetHue());
            }
            List<PaletteEntry> SortedList = MyPalette.OrderBy(o => o.Diff).ToList();
            MyPalette = SortedList;
            if (ExceptionColor.Use)
            {
                if (mode == 0)
                    ExceptionColor.Diff= ColorDiff(mycolor, ExceptionColor.Col);
                else if (mode == 1)
                    ExceptionColor.Diff = GetHueDistance(mycolor.GetHue(), ExceptionColor.Col.GetHue());
                else
                    ExceptionColor.Diff = Math.Abs(ColorNum(ExceptionColor.Col) - ColorNum(mycolor)) +
                                              GetHueDistance(ExceptionColor.Col.GetHue(), mycolor.GetHue());
             
                if (ExceptionColor.Diff < MyPalette[0].Diff)
                {
                    ExceptionColor.PixelCount++;
                    return ExceptionColor;
                }
            }
            if (setUse)
            {
                MyPalette[0].PixelCount++;
                MyPalette[0].Use = true;
            }
            return MyPalette[0];   // return tool nr of nearest color
        }

        // distance between two hues:
        private static float GetHueDistance(float hue1, float hue2)
        { float d = Math.Abs(hue1 - hue2); return d > 180 ? 360 - d : d; }
        // color brightness as perceived:
        private static float GetBrightness(Color c)
        { return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f; }
        //  weighed only by saturation and brightness 
        private static float ColorNum(Color c)
        { return c.GetSaturation() * 5 + GetBrightness(c) * 4; }
        // distance in RGB space
        private static int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B));
        }

        public static void Clear()
        {   //sortByToolNr();
            for (int i = 0; i < MyPalette.Count; i++)
            {
                MyPalette[i].Diff = int.MaxValue;
                MyPalette[i].PixelCount=0;
                MyPalette[i].Use=false;
            }
        }

    }
}