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
/*
    2018-08 only use tool table for color palette
    2019-09 add logger, change sub-path
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public struct toolProp
    {
        public short toolnr;
        public System.Drawing.Color color;
        public string name;
        public bool colorPresent;
        public bool toolSelected;
        public int codeSize;
        public int pixelCount;
        public double diff;

        public float X, Y, Z;
        public float diameter;
        public float feedXY;
        public float feedZ;
        public float saveZ;
        public float finalZ;
        public float stepZ;
        public float spindleSpeed;
        public float overlap;
        public string gcode;
    }
    public struct toolPos
    {
        public int toolnr;
        public String name;
        public float X, Y;
    }
    public static class toolTable
    {
        private static int toolTableMax = 260;            // max amount of tools
        private static toolProp[] toolTableArray = new toolProp[toolTableMax];   // load color palette into this array
        private static int toolTableIndex = 0;            // last index
        private static bool useException = false;
        private static int tmpIndex = 0;
        private static bool init_done = false;
        public const string defaultFileName = "_current_.csv";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public enum toolSortOption
        {   nosort = 0,
            bytool = 1,
            bysize = 2
        }
 // defaultTool needed in Setup   fields:   ToolNr,color,name,X,Y,Z,diameter,XYspeed,Z-speed,Z-save,Z-depth,Z-step, spindleSpeed, overlap, gcode
        public static string[] defaultTool = { "1", "000000", "Default black", "0", "0", "0", "1", "999", "555", "3", "-1", "1", "1111", "100", "" };

        public static toolPos[] getToolCordinates()
        {   if (!init_done)        //|| !Properties.Settings.Default.importGCTool
                return null;
            toolPos[] newpos = new toolPos[toolTableArray.Length];
            int index = 0;
            foreach (toolProp tool in toolTableArray)
            {   if (tool.toolnr >= 0)
                {   newpos[index].toolnr = tool.toolnr;
                    newpos[index].name = tool.name;
                    newpos[index].X = tool.X + (float)Properties.Settings.Default.toolTableOffsetX;
                    newpos[index].Y = tool.Y + (float)Properties.Settings.Default.toolTableOffsetY;
                    index++;
                }
            }
            Array.Resize(ref newpos, index);
            return newpos;
        }
        public static string getToolName(int index)
        {   foreach (toolProp tool in toolTableArray)
            {   if (index == tool.toolnr)
                    return tool.name;
            }
            return "not defined";
        }

        public static int getIndexByToolNr(int toolNr)
        {
            for (int i = 0; i < toolTableIndex; i++)
            {   if (toolTableArray[i].toolnr == toolNr)
                    return i;
            }
            return -1;
         }

        public static toolProp getToolProperties(int index)
        {   foreach (toolProp tool in toolTableArray)
            {   if (index == tool.toolnr)
                { tmpIndex = index; return tool; }
            }
            tmpIndex = toolTableArray.Length - 1;// 2;
            return toolTableArray[tmpIndex]; // return 1st regular tool;
        }

        public static void indexSetCodeSize(int size)
        {   toolTableArray[tmpIndex].codeSize = size; }
        public static void setToolCodeSize(int index, int size)
        {
            Array.Sort<toolProp>(toolTableArray, (x, y) => x.toolnr.CompareTo(y.toolnr));
            if (index < 0) index = 0;
            if (index >= toolTableIndex - 2) index = toolTableIndex - 2;
            toolTableArray[index + 1].codeSize = size;
        }
        public static void setIndex(int index)
        {   if ((index >= 0) && (index < toolTableIndex))
                tmpIndex = index;
        }
        public static short indexToolNr()
        {   if (tmpIndex < toolTableArray.Length)
                return toolTableArray[tmpIndex].toolnr;
            return -1;
        }
        public static Color indexColor()
        { return toolTableArray[tmpIndex].color; }
        public static bool indexUse()
        {   return toolTableArray[tmpIndex].colorPresent;    }
        public static bool indexSelected()
        { return toolTableArray[tmpIndex].toolSelected; }
        public static string indexName()
        { return toolTableArray[tmpIndex].name; }

        public static void sortByToolNr(bool invert)
        {   if (invert)
                Array.Sort<toolProp>(toolTableArray, (x, y) => y.toolnr.CompareTo(x.toolnr));
            else
                Array.Sort<toolProp>(toolTableArray, (x, y) => x.toolnr.CompareTo(y.toolnr));    // sort by tool nr
        }
        public static void sortByCodeSize(bool invert)
        {
            if (invert)
                Array.Sort<toolProp>(toolTableArray, (x, y) => x.codeSize.CompareTo(y.codeSize));
            else
                Array.Sort<toolProp>(toolTableArray, (x, y) => y.codeSize.CompareTo(x.codeSize));    // sort by size
        }
        public static void sortByPixelCount(bool invert)
        {
            if (invert)
                Array.Sort<toolProp>(toolTableArray, (x, y) => x.pixelCount.CompareTo(y.pixelCount));
            else
                Array.Sort<toolProp>(toolTableArray, (x, y) => y.pixelCount.CompareTo(x.pixelCount));    // sort by size
        }

        public static toolProp setDefault()
        {   toolProp tmp = new toolProp();
            tmp.X = 0; tmp.Y = 0; tmp.Z = 0; tmp.diameter=3; tmp.feedXY=1000; tmp.feedZ=500;
            tmp.saveZ = 2; tmp.finalZ = -1; tmp.stepZ= 1; tmp.spindleSpeed=1000;tmp.overlap = 100; tmp.gcode = "";
            return tmp;
        }

        /// <summary>
        /// set tool/color table
        /// get table size
        /// </summary>
        public static int init()    // return number of entries
        {
            Logger.Debug("Init tool table");
            init_done = true;
            useException=false;
            Array.Resize(ref toolTableArray, toolTableMax);
            toolTableIndex = 2;
            toolTableArray[0] = setDefault();
            toolTableArray[0].toolnr = -2;            // alpha=0
            toolTableArray[0].color = Color.White; 
            toolTableArray[0].colorPresent = false;             // never use alpha
            toolTableArray[0].diff = int.MaxValue; 
            toolTableArray[0].name = "No White";
            toolTableArray[0].pixelCount=0;

            long clr = 0;
            clr = Convert.ToInt32(defaultTool[1], 16) | 0xff000000;
            toolTableArray[1] = setDefault();
            toolTableArray[1].toolnr =  Convert.ToInt16(defaultTool[0]);
            toolTableArray[1].pixelCount = 0;
            toolTableArray[1].colorPresent = false;
            toolTableArray[1].toolSelected = true;
            toolTableArray[1].color = System.Drawing.Color.FromArgb((int)clr);
            toolTableArray[1].diff = int.MaxValue;
            toolTableArray[1].name = defaultTool[2];

            string file = System.Windows.Forms.Application.StartupPath + datapath.tools + "\\" + defaultFileName;
            if (File.Exists(file))
            {
                string[] readText = File.ReadAllLines(file);
                string[] col;
                toolTableIndex = 1;
                foreach (string s in readText)
                {
                    if (s.StartsWith("#") || s.StartsWith("/"))     // jump over comments
                        continue;
                    if (s.Length > 25)
                    {   col = s.Split(','); //ToolNr,color,name,X,Y,Z,diameter,XYspeed,Z-step, Zspeed, spindleSpeed, overlap

                        toolTableArray[toolTableIndex] = setDefault();
                        toolTableArray[toolTableIndex].colorPresent = false;
                        toolTableArray[toolTableIndex].toolSelected = true;
                        toolTableArray[toolTableIndex].diff = int.MaxValue;
                        toolTableArray[toolTableIndex].pixelCount = 0;
                        toolTableArray[toolTableIndex].colorPresent = false;

                        try
                        {
                            toolTableArray[toolTableIndex].toolnr = Convert.ToInt16(col[0].Trim());
                            clr = Convert.ToInt32(col[1].Trim(), 16) | 0xff000000;
                            toolTableArray[toolTableIndex].color = System.Drawing.Color.FromArgb((int)clr);
                            toolTableArray[toolTableIndex].name = col[2].Trim();
                            if (col.Length >= 4) toolTableArray[toolTableIndex].X = float.Parse(col[3].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 5) toolTableArray[toolTableIndex].Y = float.Parse(col[4].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 6) toolTableArray[toolTableIndex].Z = float.Parse(col[5].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 7) toolTableArray[toolTableIndex].diameter = float.Parse(col[6].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 8) toolTableArray[toolTableIndex].feedXY = float.Parse(col[7].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 9) toolTableArray[toolTableIndex].feedZ = float.Parse(col[8].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 10) toolTableArray[toolTableIndex].saveZ = float.Parse(col[9].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 11) toolTableArray[toolTableIndex].finalZ = float.Parse(col[10].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 12) toolTableArray[toolTableIndex].stepZ = float.Parse(col[11].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 13) toolTableArray[toolTableIndex].spindleSpeed = float.Parse(col[12].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 14) toolTableArray[toolTableIndex].overlap = float.Parse(col[13].Trim(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (col.Length >= 15) toolTableArray[toolTableIndex].gcode = col[14].Trim();
                        }
                        catch (Exception ex) { Logger.Debug(ex, "Error "); }
                        if (toolTableIndex < (toolTableMax - 1)) toolTableIndex++;
                    }
                }
            }
            Array.Resize(ref toolTableArray, toolTableIndex);
            
            return toolTableIndex;
        }
        public static int clear()
        {   //sortByToolNr();
            for (int i = 0; i < toolTableIndex; i++)
            {   toolTableArray[i].colorPresent = false;
                toolTableArray[i].toolSelected = true;
                toolTableArray[i].pixelCount = 0;
                toolTableArray[i].diff = int.MaxValue;
            }
            return toolTableIndex;
        }
        public static void setAllSelected(bool val)
        {   for (int i = 0; i < toolTableIndex; i++)   // add colors to AForge filter
            { toolTableArray[i].toolSelected = val; }
        }

        private static void showList()
        {
            string tmp="";
            for (int i = 0; i < toolTableArray.Length; i++)
             { tmp += i.ToString() + "  " + toolTableArray[i].toolnr + "  " + toolTableArray[i].name + "  " + toolTableArray[i].color.ToString() + "  " + String.Format("{0:X}", toolTableArray[i].pixelCount) + "\r\n"; }
             MessageBox.Show(tmp);
        }
        // set exception color
        public static string setExceptionColor(Color mycolor)
        {   useException=true;
            Array.Sort<toolProp>(toolTableArray, (x, y) => x.toolnr.CompareTo(y.toolnr));    // sort by tool nr
            toolTableArray[0].toolnr = -1; 
            toolTableArray[0].color = mycolor; 
            toolTableArray[0].colorPresent = true; 
            toolTableArray[0].diff = int.MaxValue;
            toolTableArray[0].name = "No " + ColorTranslator.ToHtml(Color.FromArgb(mycolor.ToArgb()));
    //        bool nameFound = false;
            for (int i = 1; i < toolTableIndex; i++)
            {   if (toolTableArray[i].color == mycolor)
                {   toolTableArray[0].name = "No " + toolTableArray[i].name;
   //                 nameFound = true;
                    break;
                }
            }
  /*          if (false)//!nameFound)
            {   foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor)))
                {   Color known = Color.FromKnownColor(kc);
                    if (mycolor.ToArgb() == known.ToArgb())
                    {   toolTableArray[0].name = "No " + known.Name;
                        break;
                    }
                }
            }*/
            return toolTableArray[0].color.ToString();
        }
        // Clear exception color
        public static void clrExceptionColor()
        {   useException=false; toolTableArray[0].colorPresent = false; }

        // return tool nr of nearest color
        public static int getToolNr(String mycolor, int mode)
        {
 //           Logger.Trace("getToolNr {0}",mycolor);
            if (mycolor == "")
                return (int)Properties.Settings.Default.importGCToolDefNr;  // return default tool
            else if (OnlyHexInString(mycolor))
            {
                int cr, cg, cb;
                int num = int.Parse(mycolor, System.Globalization.NumberStyles.AllowHexSpecifier);
                cb = num & 255; cg = num >> 8 & 255; cr = num >> 16 & 255;
                return getToolNr(Color.FromArgb(255, cr, cg, cb), mode);
            }
            else
                return getToolNr(Color.FromName(mycolor),mode);
        }
        public static bool OnlyHexInString(string test)
        {   // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }
        public static short getToolNr(Color mycolor,int mode)
        {
            int i,start=1;
            Array.Sort<toolProp>(toolTableArray, (x, y) => x.toolnr.CompareTo(y.toolnr));    // sort by tool nr
            if (useException) start=0;  // first element is exception
            for (i = start; i < toolTableIndex; i++)
            {
                if (mycolor == toolTableArray[i].color)         // direct hit
                {
                    tmpIndex = i;
                    return toolTableArray[i].toolnr;
                }
                else if (mode == 0)
                    toolTableArray[i].diff = ColorDiff(mycolor, toolTableArray[i].color);
                else if (mode == 1)
                    toolTableArray[i].diff = getHueDistance(mycolor.GetHue(), toolTableArray[i].color.GetHue());
                else
                    toolTableArray[i].diff = Math.Abs(ColorNum(toolTableArray[i].color) - ColorNum(mycolor)) +
                                              getHueDistance(toolTableArray[i].color.GetHue(), mycolor.GetHue());
            }
            Array.Sort<toolProp>(toolTableArray, (x, y) => x.diff.CompareTo(y.diff));    // sort by color difference
            tmpIndex = 0;
            return toolTableArray[0].toolnr; ;   // return tool nr of nearest color
        }

        public static void countPixel()
        { toolTableArray[tmpIndex].pixelCount++; }

        public static int pixelCount()
        { return toolTableArray[tmpIndex].pixelCount; }
        public static int pixelCount(int index)
        {   setIndex(index);
            return toolTableArray[tmpIndex].pixelCount;
        }

        public static Color getColor()
        { return toolTableArray[tmpIndex].color; }
        public static Color getColor(int index)
        {   setIndex(index);
            return toolTableArray[tmpIndex].color;
        }
        public static int getIndex()
        { return tmpIndex; }
        public static void setPresent(bool use)
        { toolTableArray[tmpIndex].colorPresent = use; }
        public static void setSelected(bool use)
        { toolTableArray[tmpIndex].toolSelected = use; }

        public static String getName()
        {   if (tmpIndex < toolTableArray.Length)
                return toolTableArray[tmpIndex].name;
            return "no set";
        }

        // http://stackoverflow.com/questions/27374550/how-to-compare-color-object-and-get-closest-color-in-an-color
        // distance between two hues:
        private static float getHueDistance(float hue1, float hue2)
        { float d = Math.Abs(hue1 - hue2); return d > 180 ? 360 - d : d; }
        // color brightness as perceived:
        private static float getBrightness(Color c)
        { return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f; }
        //  weighed only by saturation and brightness 
        private static float ColorNum(Color c)
        { return c.GetSaturation() * 5 + getBrightness(c) * 4; }
        // distance in RGB space
        private static int ColorDiff(Color c1, Color c2) 
              { return  (int ) Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) 
                                       + (c1.G - c2.G) * (c1.G - c2.G)
                                       + (c1.B - c2.B) * (c1.B - c2.B)); }
    }

}
