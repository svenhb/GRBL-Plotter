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

using GrblPlotter.Helper;
using GrblPlotter.UserControls;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using static GrblPlotter.DeviceToolProperties;

namespace GrblPlotter
{
    internal class ToolProperty
    {	/* Grouping properties */
        public short ToolNr { get; set; }
        public System.Drawing.Color GroupColor { get; set; }
        public float GroupWidth { get; set; }
        public string GroupLayer { get; set; }
        public string ToolName { get; set; }
        public XyzPoint Position { get; set; }
        public string Gcode { get; set; }

        public DeviceToolProperties Laser { get; set; }
        public DeviceToolProperties Plotter { get; set; }
        public DeviceToolProperties Router { get; set; }
        public double Diff { get; set; } // helper
        public ToolProperty()
        { ResetToolProperties(); }
        public ToolProperty(string col)
        {
            ResetToolProperties();
            GroupColor = Colors.TryConvertColor(col);
        }

        public void ResetToolProperties()
        {
            ToolNr = 1; GroupColor = Color.Black; GroupWidth = 1;
            GroupLayer = "1"; ToolName = "default"; Diff = 0; Position = new XyzPoint(); Gcode = "";
            Laser = new DeviceToolProperties();
            Plotter = new DeviceToolProperties();
            Router = new DeviceToolProperties();
        }

        public ToolProperty Copy()
        {
            ToolProperty other = (ToolProperty)MemberwiseClone();
            other.Laser = Laser.Copy();
            other.Plotter = Plotter.Copy();
            other.Router = Router.Copy();
            return other;
        }
        public int CompareTo(Color ccolor2, bool invert)
        {
            Color ccolor1 = this.GroupColor;
            //    Color ccolor2 = tmp;
            double brighness1 = (0.299 * ccolor1.R * ccolor1.R + 0.587 * ccolor1.G * ccolor1.G + 0.114 * ccolor1.B * ccolor1.B);
            double brighness2 = (0.299 * ccolor2.R * ccolor2.R + 0.587 * ccolor2.G * ccolor2.G + 0.114 * ccolor2.B * ccolor2.B);
            if (invert)
                return brighness2.CompareTo(brighness1);
            else
                return brighness1.CompareTo(brighness2);
        }

        public void WriteXML(ref XmlWriter w)
        {
            w.WriteStartElement("Tool");
            w.WriteAttributeString("Nr", ToolNr.ToString());
            w.WriteAttributeString("Color", ColorTranslator.ToHtml(GroupColor));
            w.WriteAttributeString("Width", GroupWidth.ToString());
            w.WriteAttributeString("Layer", GroupLayer);
            w.WriteAttributeString("Name", ToolName);
            w.WriteAttributeString("Gcode", Gcode.ToString());

            w.WriteStartElement("Position");
            w.WriteAttributeString("X", Position.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("Y", Position.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("Z", Position.Z.ToString().Replace(',', '.'));
            w.WriteAttributeString("A", Position.A.ToString().Replace(',', '.'));
            w.WriteEndElement();

            Laser.WriteXML(ref w, "Laser");
            Plotter.WriteXML(ref w, "Plotter");
            Router.WriteXML(ref w, "Router");
            w.WriteEndElement();
        }
        public void ReadPositionXML(XmlReader reader)
        {
            XyzPoint pos = Position;
            pos.X = XML.GetFloat(reader, "X", 0);
            pos.Y = XML.GetFloat(reader, "Y", 0);
            pos.Z = XML.GetFloat(reader, "Z", 0);
            pos.A = XML.GetFloat(reader, "A", 0);
            Position = pos;
        }
    }

    internal static class ToolList
    {
        private const int toolTableMax = 260;            // max amount of ToolProperty
        internal static List<ToolProperty> toolListArray = new List<ToolProperty>();   // load color palette into this array
        private static readonly int toolTableIndex = 0;            // last index
        private static readonly bool useException = false;
        public const string DefaultFileName = "_current_.csv";
        public const string DefaultFileNameXML = "_currentToolList.xml";
        private static readonly bool log = false;
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

        public static string GetToolName(int index)
        {
            foreach (ToolProperty tool in toolListArray)
            {
                if (index == tool.ToolNr)
                    return tool.ToolName;
            }
            return "not defined";
        }
        public static string GetToolColor(int index)
        {
            foreach (ToolProperty tool in toolListArray)
            {
                if (index == tool.ToolNr)
                {
                    string tmp = ColorTranslator.ToHtml(tool.GroupColor);
                    if (tmp.StartsWith("#"))
                        tmp = tmp.Substring(1);
                    return tmp; // 2021-08-26 remove '#'
                }
            }
            return "000000"; // return black
        }

        public static double GetToolDiameter(int index)
        {
            foreach (ToolProperty tool in toolListArray)
            {
                if (index == tool.ToolNr)
                {
                    return tool.GroupWidth;
                }
            }
            return 1.23;
        }


        public static void Reset()
        {
            toolListArray.Clear();
        }
        public static void Add(ToolProperty tp)
        {
            toolListArray.Add(tp.Copy());
        }
        public static int GetToolListIndexByToolNr(int nr)
        {
            for (int i = 0; i < toolListArray.Count; i++)
            {
                if (toolListArray[i].ToolNr == nr)
                    return i;
            }
            return -1;
        }
        public static ToolProperty GetToolProperties(int toolNr)
        {
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                Logger.Warn("GetToolProperties toolTableArray is empty - do Init toolNr:{0}  count {1}", toolNr, toolListArray.Count);
                Init(" (GetToolProperties)");
            }
            if ((toolNr <= 0) || toolNr > toolListArray.Count)
            {
                Logger.Warn("GetToolProperties toolTableArray toolNr nok toolNr:{0}  count {1}", toolNr, toolListArray.Count);
                return toolListArray[0];
            }
            foreach (ToolProperty tool in toolListArray)
            {
                if (toolNr == tool.ToolNr)
                { return tool; }
            }

            return toolListArray[toolNr - 1];
        }
        public static OptionPropHatchFill GetToolFill(int toolNr, DeviceSelection SelectedDevice)
        {
            int i = GetToolListIndexByToolNr(toolNr);
            if ((i >= 0) && (i < toolListArray.Count))
            {
                if (SelectedDevice == DeviceSelection.Laser)
                    return toolListArray[i].Laser.Fill;
                if (SelectedDevice == DeviceSelection.Plotter)
                    return toolListArray[i].Plotter.Fill;
                if (SelectedDevice == DeviceSelection.Router)
                    return toolListArray[i].Router.Fill;
                return new OptionPropHatchFill();
            }
            Logger.Warn("OptionPropHatchFill  PROBLEM index:{0}  toolNr:{1}  Count:{2}", i, toolNr, toolListArray.Count);
            return new OptionPropHatchFill();
        }
        public static int GetToolRepetition(int toolNr, DeviceSelection SelectedDevice)
        {
            int i = GetToolListIndexByToolNr(toolNr);
            if ((i >= 0) && (i < toolListArray.Count))
            {
                if (SelectedDevice == DeviceSelection.Laser)
                    return toolListArray[i].Laser.Passes;
                return 1;
            }
            return 1;
        }

        public static void Log(DeviceSelection device)
        {
            OptionPropHatchFill tmpFill;
            for (int i = 0; i < toolListArray.Count; i++)
            {
                if (device == DeviceSelection.Laser)
                    tmpFill = toolListArray[i].Laser.Fill;
                else if (device == DeviceSelection.Plotter)
                    tmpFill = toolListArray[i].Plotter.Fill;
                else //if (device == DeviceSelection.Plotter)
                    tmpFill = toolListArray[i].Router.Fill;
                Logger.Trace("ToolList Log i:{0} toolNr:{1} Color:{2} xy:{3}  FillEn:{4}  FillDist:{5}   FillAngle:{6}", i, toolListArray[i].ToolNr, ColorTranslator.ToHtml(toolListArray[i].GroupColor), toolListArray[i].Laser.FeedXY, tmpFill.Enable, tmpFill.Distance, tmpFill.Angle);
            }
        }

        // return tool nr of nearest color
        public static int GetToolNRByToolColor(String mycolor, int mode)
        {
        //    Logger.Trace("GetToolNRByToolColor  count:{0}   color:{1}", toolListArray.Count, mycolor);
            if (toolListArray.Count == 1)
                return 1;

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
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                //        Logger.Warn("GetToolNRByColor toolTableArray is empty - do Init");
                Init(" (GetToolNRByColor)");
            }

            for (int i = 0; i < toolListArray.Count; i++)
            {
                if (mycolor == toolListArray[i].GroupColor)         // direct hit
                {
                    //		Logger.Trace("GetToolNRByColor - direct hit");
                    return toolListArray[i].ToolNr;
                }
                else if (mode == 0)
                {
                    toolListArray[i].Diff = ColorDiff(mycolor, toolListArray[i].GroupColor);
                }
                else
                    toolListArray[i].Diff = GetHueDistance(mycolor.GetHue(), toolListArray[i].GroupColor.GetHue());
            }
            //	Logger.Trace("GetToolNRByColor - smallest diff");
            List<ToolProperty> SortedList = toolListArray.OrderBy(o => o.Diff).ToList();
            return SortedList[0].ToolNr;
        }
        private static float GetHueDistance(float hue1, float hue2)
        { float d = Math.Abs(hue1 - hue2); return d > 180 ? 360 - d : d; }
        private static int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B));
        }


        public static short GetToolNRByToolWidth(string txtWidth)
        {
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                Logger.Warn("GetToolNRByToolDiameter toolTableArray is empty - do Init");
                Init(" (GetToolNRByToolDiameter)");
            }
            if (toolListArray.Count == 1)
                return 1;

            if (!double.TryParse(txtWidth, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double Gwidth))
            { Logger.Error(culture, "Error converting getToolNrByWidth '{0}' ", txtWidth); Gwidth = 1; }

            for (int i = 0; i < toolListArray.Count; i++)
            {
                if (Gwidth == toolListArray[i].GroupWidth)         // direct hit
                {
                    return toolListArray[i].ToolNr;
                }
                toolListArray[i].Diff = Math.Abs(Gwidth - toolListArray[i].GroupWidth);
            }
            List<ToolProperty> SortedList = toolListArray.OrderBy(o => o.Diff).ToList();
            return SortedList[0].ToolNr;
        }

        public static short GetToolNRByToolLayer(string layer)
        {
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                Logger.Warn("GetToolNRByToolName toolTableArray is empty - do Init");
                Init(" (GetToolNRByToolName)");
            }
            if (toolListArray.Count == 1)
                return 1;

            for (int i = 0; i < toolListArray.Count; i++)
            {
                if (layer == toolListArray[i].GroupLayer)         // direct hit
                {
                    return toolListArray[i].ToolNr;
                }
                toolListArray[i].Diff = LevenshteinDistanceAlgorithm(layer, toolListArray[i].GroupLayer);
            }
            List<ToolProperty> SortedList = toolListArray.OrderBy(o => o.Diff).ToList();
            return SortedList[0].ToolNr; ;   // return tool nr of nearest color
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
            if (n == 0)
            { return m; }
            if (m == 0)
            { return n; }
            for (int i = 0; i <= n; d[i, 0] = i++)
            { }
            for (int j = 0; j <= m; d[0, j] = j++)
            { }
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }


        public static void ReNumberToolNr()
        {
            if (toolListArray.Count > 0)
            {
                for (int i = 0; i < toolListArray.Count; i++)
                { toolListArray[i].ToolNr = (short)(i + 1); }
            }
        }
        public static void SortByToolNr(bool invert)
        {
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                Logger.Warn("SortByToolNr toolTableArray is empty - do Init");
                Init("(SortByToolNr)");
            }
            List<ToolProperty> SortedList;
            if (!invert)
                SortedList = toolListArray.OrderBy(o => o.ToolNr).ToList();
            else
                SortedList = toolListArray.OrderByDescending(o => o.ToolNr).ToList();
            toolListArray = SortedList;
        }
        public static void SortByColor(bool invert)
        {
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                Logger.Warn("SortByColor toolTableArray is empty - do Init");
                Init("(SortByColor)");
            }
            //    List<ToolProperty> SortedList;
            toolListArray.Sort((x, y) => x.CompareTo(y.GroupColor, invert));
            //    if (!invert)
            //        SortedList = toolListArray.OrderBy(o => o.GroupColor.GetBrightness()).ToList();
            //     else
            //         SortedList = toolListArray.OrderByDescending(o => o.GroupColor.GetBrightness()).ToList();
            //     toolListArray = SortedList;
        }

        public static void SortByWidth(bool invert)
        {
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                Logger.Warn("SortByWidth toolTableArray is empty - do Init");
                Init("(SortByWidth)");
            }
            List<ToolProperty> SortedList;
            if (!invert)
                SortedList = toolListArray.OrderBy(o => o.GroupWidth).ToList();
            else
                SortedList = toolListArray.OrderByDescending(o => o.GroupWidth).ToList();
            toolListArray = SortedList;
        }
        public static void SortByLayer(bool invert)
        {
            if ((toolListArray == null) || (toolListArray.Count == 0))
            {
                Logger.Warn("SortByLayer toolTableArray is empty - do Init");
                Init("(SortByLayer)");
            }
            List<ToolProperty> SortedList;
            if (!invert)
                SortedList = toolListArray.OrderBy(o => o.GroupLayer).ToList();
            else
                SortedList = toolListArray.OrderByDescending(o => o.GroupLayer).ToList();
            toolListArray = SortedList;
        }


        private static readonly XmlReaderSettings settings = new XmlReaderSettings()
        { DtdProcessing = DtdProcessing.Prohibit };
        public static int Init(string cmt = "")    // return number of entries
        {
            Logger.Info("🛠🛠🛠 Init ToolList {0}", cmt);
            return ReadXML();
        }
        public static int ReadXML(string FileName = "")
        {
            if (FileName == "") { FileName = Datapath.Tools + "\\_currentToolList.xml"; }
            toolListArray.Clear();
            Logger.Info("ToolList ReadXML {0}", FileName);
            int toolCnt = 0;

            if (File.Exists(FileName))
            {
                try
                {
                    XmlReader reader = XmlReader.Create(FileName, settings);
                    int device = 0;
                    var psd = Properties.Settings.Default;
                    XyzPoint positionOffset = new XyzPoint((double)psd.toolTableOffsetX, (double)psd.toolTableOffsetY, (double)psd.toolTableOffsetZ, (double)psd.toolTableOffsetA);

                    while (reader.Read())
                    {
                        if (!reader.IsStartElement())
                            continue;
                        switch (reader.Name)
                        {
                            case "ToolList":
                                if (log) Logger.Trace("XMLread ToolList: {0}   {1}", reader["Device"], reader["Amount"]);
                                string tmpDevice = XML.GetString(reader, "Device", "");
                                /*	if (tmpDevice=="Laser")
                                        Control.
                                    else if (tmpDevice=="Plotter")

                                    else if (tmpDevice=="Router")*/
                                break;
                            case "ToolChange":
                                if (log) Logger.Trace("XMLread ToolChange: {0}   {1}   {2}   {3}  ", reader["Off-X"], reader["Off-Y"], reader["Off-Z"], reader["Off-A"]);
                                psd.ctrlToolChange = XML.GetBool(reader, "Enable", false);
                                psd.ctrlToolScriptDelay = XML.GetInt(reader, "ScriptDelay", 1);
                                positionOffset.X = XML.GetFloat(reader, "Off-X", 0.1f);
                                positionOffset.Y = XML.GetFloat(reader, "Off-Y", 0.2f);
                                positionOffset.Z = XML.GetFloat(reader, "Off-Z", 0.3f);
                                positionOffset.A = XML.GetFloat(reader, "Off-A", 0.4f);
                                Properties.Settings.Default.toolTableOffsetX = (decimal)positionOffset.X;
                                Properties.Settings.Default.toolTableOffsetY = (decimal)positionOffset.Y;
                                Properties.Settings.Default.toolTableOffsetZ = (decimal)positionOffset.Z;
                                Properties.Settings.Default.toolTableOffsetA = (decimal)positionOffset.A;
                                break;
                            case "ToolChangeScriptPut":
                                if ((reader["ToolChangeScriptPut"] != null) && (reader["ToolChangeScriptPut"].Length > 0))
                                    psd.ctrlToolScriptPut = reader["ToolChangeScriptPut"];
                                break;
                            case "ToolChangeScriptSelect":
                                if ((reader["ToolChangeScriptSelect"] != null) && (reader["ToolChangeScriptSelect"].Length > 0))
                                    psd.ctrlToolScriptSelect = reader["ToolChangeScriptSelect"];
                                break;
                            case "ToolChangeScriptGet":
                                if ((reader["ToolChangeScriptGet"] != null) && (reader["ToolChangeScriptGet"].Length > 0))
                                    psd.ctrlToolScriptGet = reader["ToolChangeScriptGet"];
                                break;
                            case "ToolChangeScriptProbe":
                                if ((reader["ToolChangeScriptProbe"] != null) && (reader["ToolChangeScriptProbe"].Length > 0))
                                    psd.ctrlToolScriptProbe = reader["ToolChangeScriptProbe"];
                                break;

                            case "Tool":
                                if (log) Logger.Trace("XMLread Tool: {0}   {1}   {2}   {3}  '{4}'", reader["Nr"], reader["Color"], reader["Width"], reader["Layer"], reader["Name"]);
                                toolListArray.Add(new ToolProperty());
                                toolCnt = toolListArray.Count - 1;
                                toolListArray[toolCnt].ToolNr = (short)XML.GetInt(reader, "Nr", 1);
                                if ((reader["Color"] != null) && (reader["Color"].Length > 0))
                                {
                                    string col = reader["Color"].Trim();
                                    if (col.StartsWith("#")) { col = col.Substring(1); }
                                    long clr = 0xffffffff;
                                    try
                                    {
                                        clr = Convert.ToInt32(col, 16) | 0xff000000;
                                        toolListArray[toolCnt].GroupColor = System.Drawing.Color.FromArgb((int)clr);
                                    }
                                    catch
                                    {
                                        toolListArray[toolCnt].GroupColor = Color.Black;
                                    }
                                }

                                toolListArray[toolCnt].GroupWidth = XML.GetFloat(reader, "Width", 1);
                                toolListArray[toolCnt].GroupLayer = XML.GetString(reader, "Layer", "not set");
                                toolListArray[toolCnt].ToolName = XML.GetString(reader, "Name", "Default black");
                                toolListArray[toolCnt].Gcode = XML.GetString(reader, "Gcode", "");
                                break;
                            case "Laser":
                                device = 0;
                                toolListArray[toolCnt].Laser.ReadXML(reader);
                                break;
                            case "Plotter":
                                device = 1;
                                toolListArray[toolCnt].Plotter.ReadXML(reader);
                                break;
                            case "Router":
                                device = 2;
                                toolListArray[toolCnt].Router.ReadXML(reader);
                                break;
                            case "Position":
                                toolListArray[toolCnt].ReadPositionXML(reader);
                                //        if (device == 0) { toolListArray[toolCnt].Laser.ReadPositionXML(reader); }
                                //        else if (device == 1) { toolListArray[toolCnt].Plotter.ReadPositionXML(reader); }
                                //        else if (device == 2) { toolListArray[toolCnt].Router.ReadPositionXML(reader); }
                                break;
                            case "HatchFill":
                                if (device == 0) { toolListArray[toolCnt].Laser.ReadFillXML(reader); }
                                else if (device == 1) { toolListArray[toolCnt].Plotter.ReadFillXML(reader); }
                                else if (device == 2) { toolListArray[toolCnt].Router.ReadFillXML(reader); }
                                break;
                        }
                    }
                    reader.Close();

                }
                catch (Exception err)
                { Logger.Error(err, "ReadXML nok"); }
            }
            else
            {
                Logger.Error("ReadXML file doesn't exist: {0} - create one default tool", FileName);
                toolListArray.Add(new ToolProperty());
            }
            return toolCnt;
        }

        public static void GetFromToolTable(List<ToolProp> tpList)
        {
            /* Copy ToolTable settings to ToolList */
            int toolCnt;
            toolListArray.Clear();
            Logger.Info("GetFromToolTable {0}", tpList.Count);
            foreach (ToolProp tp in tpList)
            {
                if (tp.Toolnr <= 0)
                    continue;
                toolListArray.Add(new ToolProperty());
                toolCnt = toolListArray.Count - 1;
                toolListArray[toolCnt].ToolNr = tp.Toolnr;
                toolListArray[toolCnt].GroupColor = tp.Color;

                toolListArray[toolCnt].GroupWidth = tp.Diameter;
                toolListArray[toolCnt].GroupLayer = tp.Name;
                toolListArray[toolCnt].ToolName = tp.Name;

                toolListArray[toolCnt].Position = tp.Position;
                toolListArray[toolCnt].Gcode = tp.Gcode;

                toolListArray[toolCnt].Laser.Diameter = tp.Diameter;
                toolListArray[toolCnt].Laser.FeedXY = tp.FeedXY;
                toolListArray[toolCnt].Laser.FeedZ = tp.FeedZ;
                toolListArray[toolCnt].Laser.SaveZ = tp.SaveZ;
                toolListArray[toolCnt].Laser.FinalZ = tp.FinalZ;
                //    toolListArray[toolCnt].Laser.SaveS = tp.Diameter;
                toolListArray[toolCnt].Laser.FinalS = tp.SpindleSpeed;
                //        toolListArray[toolCnt].Laser.Position = tp.Position;

                toolListArray[toolCnt].Plotter = toolListArray[toolCnt].Laser.Copy();
                toolListArray[toolCnt].Router = toolListArray[toolCnt].Laser.Copy();
            }
        }
        public static void WriteXML(string FileName = "")
        {
            if (FileName == "") { FileName = Datapath.Tools + "\\_currentToolList.xml"; }
            if (File.Exists(FileName))
                File.Delete(FileName);
            var psd = Properties.Settings.Default;
            XyzPoint positionOffset = new XyzPoint((double)psd.toolTableOffsetX, (double)psd.toolTableOffsetY, (double)psd.toolTableOffsetZ, (double)psd.toolTableOffsetA);

            Logger.Info("ToolList WriteXML  {0}  Tools:{1}", FileName, toolListArray.Count);
            XmlWriterSettings set = new XmlWriterSettings
            { Indent = true };
            XmlWriter w = XmlWriter.Create(FileName, set);
            w.WriteStartDocument();
            w.WriteStartElement("ToolList");
            w.WriteAttributeString("Device", MyControl.GetSelectedDeviceName());
            w.WriteAttributeString("Amount", toolListArray.Count.ToString());

            w.WriteStartElement("ToolChange");
            w.WriteAttributeString("Enable", psd.ctrlToolChange.ToString());
            //            w.WriteAttributeString("Empty", psd.ctrlToolChangeEmpty.ToString());
            //            w.WriteAttributeString("EmptyNr", psd.ctrlToolChangeEmptyNr.ToString());

            w.WriteAttributeString("ScriptDelay", psd.ctrlToolScriptDelay.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-X", positionOffset.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-Y", positionOffset.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-Z", positionOffset.Z.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-A", positionOffset.A.ToString().Replace(',', '.'));

            w.WriteStartElement("ToolChangeScriptPut");
            w.WriteString(psd.ctrlToolScriptPut);
            w.WriteEndElement();
            w.WriteStartElement("ToolChangeScriptSelect");
            w.WriteString(psd.ctrlToolScriptSelect);
            w.WriteEndElement();
            w.WriteStartElement("ToolChangeScriptGet");
            w.WriteString(psd.ctrlToolScriptGet);
            w.WriteEndElement();
            w.WriteStartElement("ToolChangeScriptProbe");
            w.WriteString(psd.ctrlToolScriptProbe);
            w.WriteEndElement();

            w.WriteEndElement();		// "ToolChange"

            if (toolListArray.Count > 0)
            {
                foreach (ToolProperty tP in toolListArray)
                {
                    tP.WriteXML(ref w);
                }
            }
            w.WriteEndElement();    // ToolList
            w.Close();
        }
    }

    internal class DeviceToolProperties
    {
        public float Diameter { get; set; }		// Laser-beam, Pen-tip, Router-bit
        public float FeedXY { get; set; }
        public float FeedZ { get; set; }
        public float FinalZ { get; set; }
        public float SaveZ { get; set; }
        public float FinalS { get; set; }		// SpindleSpeed; LaserPower; Servo-Val
        public float SaveS { get; set; }
        public bool UseM3 { get; set; }			// Laser
        public bool UseAir { get; set; }		// Laser
        public bool UseSorZ { get; set; }  		// Plotter use S or Z
        public int Passes { get; set; }         // Laser
                                                //        public string Gcode { get; set; }
        public OptionPropHatchFill Fill { get; set; }

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool log = false;
        public DeviceToolProperties()
        {
            Reset();
        }
        public void Reset()
        {
            Diameter = 0.9f; FeedXY = 999; FeedZ = 99; FinalZ = -0.99f; SaveZ = 4.9f; FinalS = 9; SaveS = 19;
            UseM3 = false; UseAir = false; UseSorZ = false; Passes = 1; Fill = new OptionPropHatchFill();    //Position = new XyzPoint(); Gcode = "";
        }

        public DeviceToolProperties Copy()
        {
            DeviceToolProperties other = (DeviceToolProperties)MemberwiseClone();
            other.Fill = Fill.Copy();
            return other;
        }
        public string Settings()//int device = 0)
        {           //if (device==0)
            return string.Format("FeedXY:{0}  MinS:{1}  MaxS:{2} | UseSorZ:{3}  FeedZ:{4}  MinZ:{5}  MaxZ:{6} | Air:{7}  M3/4:{8}  Passes:{9} | FillEnable:{10}", FeedXY, SaveS, FinalS, UseSorZ, FeedZ, SaveZ, FinalZ, UseAir, UseM3, Passes, Fill.Enable);
        }
        public void WriteXML(ref XmlWriter w, string tag)
        {
            w.WriteStartElement(tag);
            w.WriteAttributeString("Diameter", Diameter.ToString().Replace(',', '.'));
            w.WriteAttributeString("FeedXY", FeedXY.ToString().Replace(',', '.'));
            w.WriteAttributeString("FeedZ", FeedZ.ToString().Replace(',', '.'));
            w.WriteAttributeString("FinalZ", FinalZ.ToString().Replace(',', '.'));
            w.WriteAttributeString("SaveZ", SaveZ.ToString().Replace(',', '.'));
            w.WriteAttributeString("FinalS", FinalS.ToString().Replace(',', '.'));
            w.WriteAttributeString("SaveS", SaveS.ToString().Replace(',', '.'));

            w.WriteAttributeString("UseM3", UseM3.ToString());
            w.WriteAttributeString("UseAir", UseAir.ToString());
            w.WriteAttributeString("UseS", UseSorZ.ToString());
            w.WriteAttributeString("Passes", Passes.ToString().Replace(',', '.'));

            Fill.WriteXML(ref w);

            w.WriteEndElement();
        }
        public void ReadXML(XmlReader reader)
        {
            if (log) Logger.Trace("XMLread Tool: {0}   {1}   {2}   {3}  '{4}'", reader["FeedXY"], reader["FeedZ"], reader["FinalZ"], reader["SaveZ"], reader["FinalS"]);
            if (reader["Diameter"].Length > 0) { Diameter = XML.GetFloat(reader, "Diameter", 1); }

            FeedXY = XML.GetFloat(reader, "FeedXY", 101);
            FeedZ = XML.GetFloat(reader, "FeedZ", 101);
            FinalZ = XML.GetFloat(reader, "FinalZ", -0.9f);
            SaveZ = XML.GetFloat(reader, "SaveZ", 1.9f);
            FinalS = XML.GetFloat(reader, "FinalS", 10);
            SaveS = XML.GetFloat(reader, "SaveS", 20);

            UseM3 = XML.GetBool(reader, "UseM3", false);
            UseAir = XML.GetBool(reader, "UseAir", false);
            UseSorZ = XML.GetBool(reader, "UseS", false);
            Passes = XML.GetInt(reader, "Passes", 1);
            //     Gcode = XML.GetString(reader, "Gcode", "");
        }

        public void ReadFillXML(XmlReader reader)
        {
            Fill.ReadXML(reader);
        }
        internal class OptionPropHatchFill
        {
            public bool Enable { get; set; }
            public bool Cross { get; set; }
            public float Distance { get; set; }
            public bool DistanceOffsetEnable { get; set; }
            public float DistanceOffset { get; set; }
            public float Angle { get; set; }
            public bool AngleIncrementEnable { get; set; }
            public float AngleIncrement { get; set; }
            public bool InsetEnable { get; set; }
            public float InsetDistance { get; set; }
            public bool InsetShrink { get; set; }
            public bool DeletePath { get; set; }
            public object this[string propertyName]
            {
                get
                {
                    // probably faster without reflection:
                    // like:  return Properties.Settings.Default.PropertyValues[propertyName] 
                    // instead of the following
                    Type myType = typeof(OptionPropHatchFill);
                    PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                    return myPropInfo.GetValue(this, null);
                }
                set
                {
                    Type myType = typeof(OptionPropHatchFill);
                    PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                    myPropInfo.SetValue(this, value, null);
                }
            }
            public OptionPropHatchFill()
            {
                Enable = false; Cross = false; Distance = 1; DistanceOffsetEnable = false; DistanceOffset = 0.1f;
                Angle = 0; AngleIncrementEnable = false; AngleIncrement = 5; InsetEnable = true; InsetDistance = 0.1f;
                InsetShrink = true; DeletePath = false; //AddNoise = false;
            }
            public OptionPropHatchFill Copy()
            {
                OptionPropHatchFill other = (OptionPropHatchFill)MemberwiseClone();
                return other;
            }
            public string ListSettings()
            {
                return string.Format("Fill Enable:{0}  Distance:{1}  Angle:{2}  InsetDistance:{3}", Enable, Distance, Angle, InsetDistance);
            }
            public static void ControlDefaultsSetList(List<ControlDefaults> list)
            {
                list.Add(new ControlDefaults(Localization.GetString("optionFillEnable"), "Enable"));              //  min, max, inc
                list.Add(new ControlDefaults(Localization.GetString("optionFillCross"), "Cross"));              //  min, max, inc
                list.Add(new ControlDefaults(Localization.GetString("optionFillDistance"), "Distance", new decimal[] { 0.01m, 10m, 0.1m, 2m }));
                list.Add(new ControlDefaults(Localization.GetString("optionFillAngle"), "Angle", new decimal[] { 0m, 180m, 15m, 1m }));
                list.Add(new ControlDefaults(Localization.GetString("optionFillAngleIncrementEnable"), "AngleIncrementEnable"));              //  min, max, inc
                list.Add(new ControlDefaults(Localization.GetString("optionFillAngleIncrement"), "AngleIncrement", new decimal[] { 0m, 90m, 5m, 1m }));
                list.Add(new ControlDefaults(Localization.GetString("optionFillInsetDistance"), "InsetDistance", new decimal[] { 0.0m, 10m, 0.1m, 2m }));
                list.Add(new ControlDefaults(Localization.GetString("optionFillDeletePath"), "DeletePath"));              //  min, max, inc
            }
            public void WriteXML(ref XmlWriter w)
            {
                w.WriteStartElement("HatchFill");
                w.WriteAttributeString("Enable", Enable.ToString());
                w.WriteAttributeString("Distance", Distance.ToString().Replace(',', '.'));
                w.WriteAttributeString("Angle", Angle.ToString().Replace(',', '.'));
                w.WriteAttributeString("IncAngleEnable", AngleIncrementEnable.ToString());
                w.WriteAttributeString("IncAngle", AngleIncrement.ToString().Replace(',', '.'));

                w.WriteAttributeString("Inset", InsetDistance.ToString().Replace(',', '.'));
                w.WriteAttributeString("DeletePath", DeletePath.ToString().Replace(',', '.'));
                w.WriteEndElement();
            }
            public void ReadXML(XmlReader reader)
            {
                if (log) Logger.Trace("Fill ReadXML Enable:'{0}'", reader["Enable"]);
                Enable = XML.GetBool(reader, "Enable", false);
                Distance = XML.GetFloat(reader, "Distance", 1);
                Angle = XML.GetFloat(reader, "Angle", 0);
                AngleIncrementEnable = XML.GetBool(reader, "IncAngleEnable", false);
                AngleIncrement = XML.GetFloat(reader, "IncAngle", 5);
                InsetDistance = XML.GetFloat(reader, "Inset", 0);
                DeletePath = XML.GetBool(reader, "DeletePath", false);
            }
        }
    }
}
