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
/*
 * 2018-08 only use tool table for color palette
 * 2019-09 add logger, change sub-path
 * 2020-04 add codeSize (line-count), codeDimension (xSize * ySize)
 * 2020-07-06 add axis A
 * 2020-09-24 add indexWidth()
 * 2021-03-26 add getColorWidth()
 * 2021-07-26 code clean up / code quality
 * 2021-08-26 GetToolColor remove leading '#'
 * 2021-12-09 set default tool-nr=1
 * 2022-07-29 Init add try catch
 * 2023-03-03 add xmlAtrribute
 * 2023-03-08 l:87 f:ToolProp add WriteAttributes ReadAttributes for XML data
 * 2024-01-07 l:181 f:GetToolDiameter needed to fix issue #370
 * 2024-12-12 clean up
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace GrblPlotter
{
    internal class ToolProp
    {	// tool properties general
        public short Toolnr { get; set; }
        public System.Drawing.Color Color { get; set; }
        public string Name { get; set; }
        public XyzPoint Position;// { get; set; }
        public float Diameter { get; set; }
        public float FeedXY { get; set; }
        public float FeedZ { get; set; }
        public float SaveZ { get; set; }
        public float FinalZ { get; set; }
        public float StepZ { get; set; }
        public float SpindleSpeed { get; set; }
        public float Overlap { get; set; }
        public string Gcode { get; set; }

        // path properties (image import) for sorting
        public bool ColorPresent { get; set; }
        public bool ToolSelected { get; set; }
        public int CodeSize { get; set; }
        public double CodeDimension { get; set; }
        public int PixelCount { get; set; }
        public double Diff { get; set; }

        public ToolProp()
        { ResetToolProperties(); ResetPathProperties(); }

        // tool-nr, color, name, xyz-pos, diameter, feedxy, feedz, savez, finalz, stpz
        public ToolProp(short tnr, System.Drawing.Color col, string nam)//, XyzPoint pos, float dia, float fxy, float fz, float savz, float finz, float stpz, fload sspd, float ovlp, string cod)
        {
            ResetToolProperties(); ResetPathProperties();
            Toolnr = tnr; Color = col; Name = nam;//	Position=pos;	Diameter=dia; FeedXY=fxy; FeedZ=fz; SaveZ=savz; FinalZ=finz; ResetPathProperties();}
        }

        public void ResetToolProperties()
        {
            Toolnr = 1; Color = Color.Black; Name = "default"; Position = new XyzPoint(); Diameter = 1; FeedXY = 1111; FeedZ = 555; SaveZ = 4.444f; FinalZ = -1.111f;
            StepZ = 1.111f; SpindleSpeed = 999; Overlap = 99.9f; Gcode = "";
        }

        public void ResetPathProperties()
        { ColorPresent = false; ToolSelected = false; CodeSize = 0; CodeDimension = 0; PixelCount = 0; Diff = int.MaxValue; }
		
		public void WriteAttributes(XmlWriter xmlWrite)
		{
		//	xmlWrite.WriteAttributeString("tnr", Toolnr.ToString().Replace(',', '.'));
			xmlWrite.WriteAttributeString("dia", Diameter.ToString().Replace(',', '.'));
			xmlWrite.WriteAttributeString("stz", StepZ.ToString().Replace(',', '.'));
			xmlWrite.WriteAttributeString("fnz", FinalZ.ToString().Replace(',', '.'));
			xmlWrite.WriteAttributeString("fxy", FeedXY.ToString().Replace(',', '.'));
			xmlWrite.WriteAttributeString("fz", FeedZ.ToString().Replace(',', '.'));
			xmlWrite.WriteAttributeString("ovl", Overlap.ToString().Replace(',', '.'));
			xmlWrite.WriteAttributeString("spd", SpindleSpeed.ToString().Replace(',', '.'));
		}
		
		public void ReadAttributes(XmlReader xmlRead)
		{                    
			float? tmp;
			tmp = ReadAttribute(xmlRead, "dia"); if(tmp!=null) {Diameter = (float)tmp;}
			tmp = ReadAttribute(xmlRead, "stz"); if(tmp!=null) {StepZ = (float)tmp;}
			tmp = ReadAttribute(xmlRead, "fnz"); if(tmp!=null) {FinalZ = (float)tmp;}
			tmp = ReadAttribute(xmlRead, "fxy"); if(tmp!=null) {FeedXY = (float)tmp;}
			tmp = ReadAttribute(xmlRead, "fz");  if(tmp!=null) {FeedZ = (float)tmp;}
			tmp = ReadAttribute(xmlRead, "ovl"); if(tmp!=null) {Overlap = (float)tmp;}
			tmp = ReadAttribute(xmlRead, "spd"); if(tmp!=null) {SpindleSpeed = (float)tmp;}
		}
		
		private float? ReadAttribute(XmlReader xmlRead, string att)
		{
			try
			{
				if (xmlRead[att].Length > 0) { return float.Parse(xmlRead[att].Replace(',', '.'), NumberFormatInfo.InvariantInfo); }
			}
			catch
			{}	
			return null;
		}
    }

    internal static class ToolTable
    {
        private const int toolTableMax = 260;            // max amount of tools
        internal static List<ToolProp> toolTableArray = new List<ToolProp>();   // load color palette into this array
        private static int toolTableIndex = 0;            // last index
        private static bool useException = false;
        private static int tmpIndex = 0;
        public const string DefaultFileName = "_current_.csv";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public enum ToolSortOption
        {
            Nosort = 0,
            Bytool = 1,
            Bysize = 2,
            Bydim = 3
        }
        // defaultTool needed in Setup   fields:   ToolNr,color,name,X,Y,Z,A, diameter,XYspeed,Z-speed,Z-save,Z-depth,Z-step, spindleSpeed, overlap, gcode
        internal static string[] defaultTool = { "1", "000000", "Default black", "0", "0", "0", "0", "1", "999", "555", "3", "-1", "1", "1111", "100", "" };

        public static string GetToolName(int index)
        {
            foreach (ToolProp tool in toolTableArray)
            {
                if (index == tool.Toolnr)
                    return tool.Name;
            }
            return "not defined";
        }
		
        public static string GetToolColor(int index)
        {
            foreach (ToolProp tool in toolTableArray)
            {
                if (index == tool.Toolnr)
                {
                    string tmp = ColorTranslator.ToHtml(tool.Color);
                    if (tmp.StartsWith("#"))
                        tmp = tmp.Substring(1);
                    return tmp; // 2021-08-26 remove '#'
                }
            }
            return "000000"; // return black
        }

        public static double GetToolDiameter(int index)
        {
            foreach (ToolProp tool in toolTableArray)
            {
                if (index == tool.Toolnr)
                {
                    return tool.Diameter;
                }
            }
            return 1.23; 
        }

        public static int GetIndexByToolNR(int toolNr)
        {
            for (int i = 0; i < toolTableArray.Count; i++)
            {
                if (toolTableArray[i].Toolnr == toolNr)
                    return i;
            }
            return -1;
        }

        internal static ToolProp GetToolProperties(int toolNr)
        {
            if ((toolTableArray == null) || (toolTableArray.Count == 0))
            {
                Logger.Warn("GetToolProperties toolTableArray is empty - do Init");
                Init(" (GetToolProperties)");
            }

            int index;
            if (toolTableArray != null)
            {
                foreach (ToolProp tool in toolTableArray)
                {
                    if (toolNr == tool.Toolnr)
                    { index = toolNr; return tool; }
                }
            }
            index = toolTableArray.Count - 1;// 2;
            return toolTableArray[index]; // return 1st regular tool;
        }

        public static void SetIndex(int index)
        {
            if ((index >= 0) && (index <= toolTableIndex))
                tmpIndex = index;
        }

        public static short IndexToolNR()
        {
            if (tmpIndex < toolTableArray.Count)
                return toolTableArray[tmpIndex].Toolnr;
            return -1;
        }
        public static Color IndexColor()
        { return toolTableArray[tmpIndex].Color; }
        public static bool IndexUse()
        { return toolTableArray[tmpIndex].ColorPresent; }
        public static bool IndexSelected()
        { return toolTableArray[tmpIndex].ToolSelected; }
        public static string IndexName()
        { return toolTableArray[tmpIndex].Name; }

        public static float IndexWidth()
        { return toolTableArray[tmpIndex].Diameter; }

        public static void SortByToolNR(bool invert)
        {
            if ((toolTableArray == null) || (toolTableArray.Count == 0))
            {
                Logger.Warn("SortByToolNR toolTableArray is empty - do Init");
                Init(" (SortByToolNR)");
            }
            List<ToolProp> SortedList;
            if (!invert)
                SortedList = toolTableArray.OrderBy(o => o.Toolnr).ToList();
            else
                SortedList = toolTableArray.OrderByDescending(o => o.Toolnr).ToList();
            toolTableArray = SortedList;
        }
		
        public static void SortByPixelCount(bool invert)
        {
            if ((toolTableArray == null) || (toolTableArray.Count == 0))
            {
                Logger.Warn("SortByPixelCount toolTableArray is empty - do Init");
                Init(" (SortByPixelCount)");
            }
            List<ToolProp> SortedList;
            if (invert)
                SortedList = toolTableArray.OrderBy(o => o.PixelCount).ToList();
            else
                SortedList = toolTableArray.OrderByDescending(o => o.PixelCount).ToList();
            toolTableArray = SortedList;
        }

        internal static ToolProp FindToolByFSZ(double valF, double valS, double valZ, ToolProp deflt)
        {
            int tmpIndex = -1;
            List<ToolProp> SortedList;
            SortedList = toolTableArray.OrderBy(o => o.Toolnr).ToList();
            toolTableArray = SortedList;

            for (int i = 1; i < toolTableArray.Count; i++)
            {
                if (valF == toolTableArray[i].FeedXY)
                {
                    tmpIndex = i;
                    if (((valS > 0) && (valS == toolTableArray[i].SpindleSpeed)) || ((valZ < double.MaxValue) && (valZ == toolTableArray[i].FinalZ)))
                        return toolTableArray[i];
                }
            }
            if (tmpIndex >= 0)
                return toolTableArray[tmpIndex];

            deflt.Toolnr = -1;
            return deflt;
        }

        /// <summary>
        /// set tool/color table
        /// get table size
        /// </summary>
        public static int Init(string cmt = "")    // return number of entries
        {
            useException = false;
            toolTableArray.Clear();

            toolTableArray.Add(new ToolProp(-2, Color.White, "No White"));  // add exception color

            string file = Datapath.Tools + "\\" + DefaultFileName;
            Logger.Info("🛠🛠 Init tool table{0}: {1}", cmt, DefaultFileName);
            bool anyToolFound = false;
            if (File.Exists(file))
            {
                try
                {
                    string[] readText = File.ReadAllLines(file);
                    string[] col;
                    toolTableIndex = 1;
                    foreach (string s in readText)
                    {
                        if (s.StartsWith("#") || s.StartsWith("/"))     // jump over comments
                            continue;
                        if (s.Length > 25)
                        {
                            col = s.Split(','); //ToolNr,color,name,X,Y,Z,diameter,XYspeed,Z-step, Zspeed, spindleSpeed, overlap

                            toolTableArray.Add(new ToolProp());             // add empty property, fill later
                            toolTableIndex = toolTableArray.Count - 1;

                            try
                            {
                                toolTableArray[toolTableIndex].Toolnr = Convert.ToInt16(col[0].Trim(), culture);
                                long clr = Convert.ToInt32(col[1].Trim(), 16) | 0xff000000;
                                toolTableArray[toolTableIndex].Color = System.Drawing.Color.FromArgb((int)clr);
                                toolTableArray[toolTableIndex].Name = col[2].Trim();
                                if (col.Length >= 4) toolTableArray[toolTableIndex].Position.X = float.Parse(col[3].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 5) toolTableArray[toolTableIndex].Position.Y = float.Parse(col[4].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 6) toolTableArray[toolTableIndex].Position.Z = float.Parse(col[5].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 7) toolTableArray[toolTableIndex].Position.A = float.Parse(col[6].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 8) toolTableArray[toolTableIndex].Diameter = float.Parse(col[7].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 9) toolTableArray[toolTableIndex].FeedXY = float.Parse(col[8].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 10) toolTableArray[toolTableIndex].FeedZ = float.Parse(col[9].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 11) toolTableArray[toolTableIndex].SaveZ = float.Parse(col[10].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 12) toolTableArray[toolTableIndex].FinalZ = float.Parse(col[11].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 13) toolTableArray[toolTableIndex].StepZ = float.Parse(col[12].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 14) toolTableArray[toolTableIndex].SpindleSpeed = float.Parse(col[13].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 15) toolTableArray[toolTableIndex].Overlap = float.Parse(col[14].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                                if (col.Length >= 16) toolTableArray[toolTableIndex].Gcode = col[15].Trim();
                                anyToolFound = true;
                            }
                            catch (Exception ex) { Logger.Error(ex, "ToolTable.Init() Error "); }
                            if (toolTableIndex >= (toolTableMax - 1)) break;
                        }
                    }
                }
                catch (Exception err)
                {
                    Logger.Error(err, "ToolTable Init read file ");
                    EventCollector.StoreException("ToolTable Init read file " + err.Message);
                }
            }
            if (!anyToolFound)
            {
                toolTableArray.Add(new ToolProp(1, Color.Black, "Default 1"));  // add default colors
                toolTableIndex = toolTableArray.Count - 1;
                Logger.Error("Tool table not found or empty, use defaults. Count:{0}", toolTableArray.Count);
                Properties.Settings.Default.importGCToolDefNr = 1;
            }
            return toolTableArray.Count;
        }
		
        public static int Clear()
        {   //sortByToolNr();
            for (int i = 0; i < toolTableArray.Count; i++)
            {
                toolTableArray[i].ColorPresent = false;
                toolTableArray[i].ToolSelected = true;
                toolTableArray[i].PixelCount = 0;
                toolTableArray[i].Diff = int.MaxValue;
            }
            return toolTableIndex;
        }
		
        public static void SetAllSelected(bool val)
        {
            for (int i = 0; i < toolTableArray.Count; i++)   // add colors to AForge filter
            { toolTableArray[i].ToolSelected = val; }
        }

        // set exception color
        public static string SetExceptionColor(Color mycolor)
        {
            useException = true;
            if ((toolTableArray == null) || (toolTableArray.Count == 0))
            {
                Logger.Warn("SetExceptionColor toolTableArray is empty - do init");
                Init(" (SetExceptionColor)");
            }
            List<ToolProp> SortedList = toolTableArray.OrderBy(o => o.Toolnr).ToList();
            toolTableArray = SortedList;
            toolTableArray[0].Toolnr = -1;
            toolTableArray[0].Color = mycolor;
            toolTableArray[0].ColorPresent = true;
            toolTableArray[0].Diff = int.MaxValue;
            toolTableArray[0].Name = "No " + ColorTranslator.ToHtml(Color.FromArgb(mycolor.ToArgb()));

            for (int i = 1; i < toolTableArray.Count; i++)
            {
                if (toolTableArray[i].Color == mycolor)
                {
                    toolTableArray[0].Name = "No " + toolTableArray[i].Name;
                    break;
                }
            }
            return toolTableArray[0].Color.ToString();
        }
		
        // Clear exception color
        public static void ClrExceptionColor()
        {
            useException = false; SortByToolNR(false);
            if (toolTableArray.Count > 0)
                toolTableArray[0].ColorPresent = false;
        }

        // return tool nr of nearest color
        public static int GetToolNRByToolColor(String mycolor, int mode)
        {
            if (string.IsNullOrEmpty(mycolor))
                return (int)Properties.Settings.Default.importGCToolDefNr;  // return default tool
            else if (OnlyHexInString(mycolor))
            {
                if (!mycolor.StartsWith("#"))
                    mycolor = "#" + mycolor;
                return GetToolNRByColor(ColorTranslator.FromHtml(mycolor), mode);
            }
            else
                return GetToolNRByColor(Color.FromName(mycolor), mode);
        }
		
        public static bool OnlyHexInString(string test)
        {   // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }
		
        public static short GetToolNRByColor(Color mycolor, int mode)
        {
            if ((toolTableArray == null) || (toolTableArray.Count == 0))
            {
                Logger.Warn("GetToolNRByColor toolTableArray is empty - do Init");
                Init(" (GetToolNRByColor)");
            }

            int i, start = 1;
            List<ToolProp> SortedList = toolTableArray.OrderBy(o => o.Toolnr).ToList();
            toolTableArray = SortedList;
            if (useException) start = 0;  // first element is exception

            for (i = start; i < toolTableArray.Count; i++)
            {
                if (mycolor == toolTableArray[i].Color)         // direct hit
                {
                    tmpIndex = i;
                    return toolTableArray[i].Toolnr;
                }
                else if (mode == 0)
                    toolTableArray[i].Diff = ColorDiff(mycolor, toolTableArray[i].Color);
                else if (mode == 1)
                    toolTableArray[i].Diff = GetHueDistance(mycolor.GetHue(), toolTableArray[i].Color.GetHue());
                else
                    toolTableArray[i].Diff = Math.Abs(ColorNum(toolTableArray[i].Color) - ColorNum(mycolor)) +
                                              GetHueDistance(toolTableArray[i].Color.GetHue(), mycolor.GetHue());
            }
            SortedList = toolTableArray.OrderBy(o => o.Diff).ToList();
            toolTableArray = SortedList;
            tmpIndex = 0;
            return toolTableArray[0].Toolnr; ;   // return tool nr of nearest color
        }

        public static short GetToolNRByToolDiameter(string txtWidth)
        {
            if ((toolTableArray == null) || (toolTableArray.Count == 0))
            {
                Logger.Warn("GetToolNRByToolDiameter toolTableArray is empty - do Init");
                Init(" (GetToolNRByToolDiameter)");
            }

            if (!double.TryParse(txtWidth, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double width))
            { Logger.Error(culture, "Error converting getToolNrByWidth '{0}' ", txtWidth); width = 1; }

            List<ToolProp> SortedList = toolTableArray.OrderBy(o => o.Toolnr).ToList();
            toolTableArray = SortedList;

            for (int i = 1; i < toolTableArray.Count; i++)
            {
                if (width == toolTableArray[i].Diameter)         // direct hit
                {
                    tmpIndex = i;
                    return toolTableArray[i].Toolnr;
                }
                toolTableArray[i].Diff = Math.Abs(width - toolTableArray[i].CodeDimension);
            }
            SortedList = toolTableArray.OrderBy(o => o.Diff).ToList();
            toolTableArray = SortedList;
            tmpIndex = 0;
            return toolTableArray[0].Toolnr; ;   // return tool nr of nearest color
        }

        public static short GetToolNRByToolName(string layer)
        {
            if ((toolTableArray == null) || (toolTableArray.Count == 0))
            {
                Logger.Warn("GetToolNRByToolName toolTableArray is empty - do Init");
                Init(" (GetToolNRByToolName)");
            }
            List<ToolProp> SortedList = toolTableArray.OrderBy(o => o.Toolnr).ToList();
            toolTableArray = SortedList;
            int diff = 0;

            for (int i = 1; i < toolTableArray.Count; i++)
            {
                if (layer == toolTableArray[i].Name)         // direct hit
                {
                    tmpIndex = i;
                    return toolTableArray[i].Toolnr;
                }
                diff = LevenshteinDistanceAlgorithm(layer, toolTableArray[i].Name);

                toolTableArray[i].Diff = diff;
            }
            SortedList = toolTableArray.OrderBy(o => o.Diff).ToList();
            toolTableArray = SortedList;
            tmpIndex = 0;
            return toolTableArray[0].Toolnr; ;   // return tool nr of nearest color
        }
		
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistanceAlgorithm(string s, string t)
        {
            if (s == null) return 100;
            if (t == null) return 100;

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            { return m; }

            if (m == 0)
            { return n; }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            { }

            for (int j = 0; j <= m; d[0, j] = j++)
            { }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            //         Logger.Debug("  Compute '{0}' '{1}'  Result:{2}", s,t, d[n, m]);
            return d[n, m];
        }



        public static void CountPixel()
        { toolTableArray[tmpIndex].PixelCount++; }

        public static int PixelCount()
        { return toolTableArray[tmpIndex].PixelCount; }
		
        public static int PixelCount(int index)
        {
            SetIndex(index);
            return toolTableArray[tmpIndex].PixelCount;
        }

        public static Color GetColor()
        { return toolTableArray[tmpIndex].Color; }
		
        public static void SetPresent(bool use)
        { toolTableArray[tmpIndex].ColorPresent = use; }
		
        public static void SetSelected(bool use)
        { toolTableArray[tmpIndex].ToolSelected = use; }

        public static String GetName()
        {
            if (tmpIndex < toolTableArray.Count)
                return toolTableArray[tmpIndex].Name;
            return "no set";
        }
		
        public static int GetToolNr()
        {
            if (tmpIndex < toolTableArray.Count)
                return toolTableArray[tmpIndex].Toolnr;
            return -9;
        }

        // http://stackoverflow.com/questions/27374550/how-to-compare-color-object-and-get-closest-color-in-an-color
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
    }
}
