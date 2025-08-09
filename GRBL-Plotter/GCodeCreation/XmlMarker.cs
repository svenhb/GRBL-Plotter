﻿/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2025 Sven Hasemann contact: svenhb@web.de

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
 * 2020-06-14 move class into sperate file
 * 2020-06-15 add 'Header'
 * 2020-08-04 add figureNr, penWidth
 * 2020-12-16 add Tile
 * 2021-07-02 code clean up / code quality
 * 2021-09-02 read XML attribute OffsetX, -Y
 * 2022-04-04 change PathId from int to string
 * 2023-04-15 improove GetFigure, GetGroup, GetTile
 * 2023-07-03 l:338 f:GetAttributeValue check if remaining length is enough
 * 2023-07-31 new functions FindInsertPositionFigure/Group/Next
 * 2023-11-15 add figurePenColorAnyCount
 * 2024-01-14 l:452 f:FinishFigure change check if (tmpFigure.PenColor.Contains("none"))
 * 2025-06-20 bug fix in FindInsertPositionFigureNext and FindInsertPositionGroupNext; add ListAllFigures, ListAllGroups
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace GrblPlotter
{
    public enum XmlMarkerType { None, Collection, Tile, Group, Figure, Path, Pass, Contour, Fill, Node };
    public static class XmlMarker
    {   // order by hierachy / importance
        public const string CollectionStart = "<Collection";
        public const string CollectionEnd = "</Collection";
        public const string TileStart = "<Tile";
        public const string TileEnd = "</Tile";
        public const string GroupStart = "<Group";
        public const string GroupEnd = "</Group";
        public const string FigureStart = "<Figure";
        public const string FigureEnd = "</Figure";
        public const string PathStart = "<Path";
        public const string PathEnd = "</Path";
        public const string PassStart = "<Pass";
        public const string PassEnd = "</Pass";
        public const string ContourStart = "<Contour";
        public const string ContourEnd = "</Contour";
        public const string FillStart = "<Fill";
        public const string FillEnd = "</Fill";
        public const string RevolutionStart = "<Revolution";
        public const string RevolutionEnd = "</Revolution";
        public const string ClearanceStart = "<Clearance";
        public const string ClearanceEnd = "</Clearance";
        public const string HeaderStart = "<Header";
        public const string HeaderEnd = "</Header";

        public const string TangentialAxis = "<Tangential";
        public const string HalftoneS = "<DisplayPenWidthS";
        public const string HalftoneZ = "<DisplayPenWidthZ";
        public const string PixelArt = "<DisplayPixelArt";

        internal class BlockData
        {
            public int MyIndex { get; set; }
            public int LineStart { get; set; }           // line nr. in editor
            public int LineEnd { get; set; }             // line nr. in editor
            public XyPoint PosStart { get; set; }        // xy position
            public XyPoint PosEnd { get; set; }
            public XyPoint Offset { get; set; }
            public double Distance { get; set; }
            public bool Reverse { get; set; }
            public int FigureNr { get; set; }

            public int Id { get; set; }                  // block informations
            public int ToolNr { get; set; }
            public int CodeSize { get; set; }
            public int CodeArea { get; set; }
            //        public int PathId { get; set; }
            public double PathLength { get; set; }
            public double PathArea { get; set; }
            public double PenWidth { get; set; }

            public string Geometry { get; set; }
            public string PenColor { get; set; }
            public string ToolName { get; set; }
            public string Layer { get; set; }
            public string PathId { get; set; }
            public string Type { get; set; }
        };

        internal static readonly List<BlockData> listFigures = new List<BlockData>();
        private static readonly List<BlockData> listGroups = new List<BlockData>();
        private static readonly List<BlockData> listTiles = new List<BlockData>();
        private static readonly List<BlockData> listCollections = new List<BlockData>();

        internal static BlockData tmpFigure = new BlockData();
        internal static BlockData tmpGroup = new BlockData();
        internal static BlockData tmpTile = new BlockData();
        internal static BlockData tmpCollection = new BlockData();

        internal static BlockData lastFigure = new BlockData();
        internal static BlockData lastGroup = new BlockData();
        internal static BlockData lastTile = new BlockData();
        internal static BlockData lastCollection = new BlockData();
        internal static BlockData header = new BlockData();
        internal static BlockData footer = new BlockData();

		internal static int figurePenColorNoneCount = 0;
		internal static int figurePenColorAnyCount = 0;
		
        public enum SortOption { Id, Color, Width, Layer, Type, Geometry, ToolNR, ToolName, CodeSize, CodeArea, Distance };

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static uint logFlags = 0;
        private static bool logEnable = false;


        public static void Reset()
        {
            listFigures.Clear(); listGroups.Clear(); listTiles.Clear(); listCollections.Clear();
            header.LineStart = 0; header.LineEnd = 999999;
            footer.LineStart = footer.LineEnd = 0;
			figurePenColorNoneCount = 0;
			figurePenColorAnyCount = 0;
			
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level4) > 0);
        }


        public static void SortBy(SortOption sort)
        { SortBy(sort, false); }
        public static void SortBy(SortOption sort, bool reverse)
        {
            switch (sort)
            {
                case SortOption.Id:
                    if (reverse) { listFigures.Sort((x, y) => y.Id.CompareTo(x.Id)); }
                    else { listFigures.Sort((x, y) => x.Id.CompareTo(y.Id)); }
                    return;

                case SortOption.Color:
                    if (reverse) listFigures.Sort((x, y) => y.PenColor.CompareTo(x.PenColor));
                    else listFigures.Sort((x, y) => x.PenColor.CompareTo(y.PenColor));
                    return;

                case SortOption.Width:
                    if (reverse) listFigures.Sort((x, y) => y.PenWidth.CompareTo(x.PenWidth));
                    else listFigures.Sort((x, y) => x.PenWidth.CompareTo(y.PenWidth));
                    return;

                case SortOption.Layer:
                    if (reverse) listGroups.Sort((x, y) => y.Layer.CompareTo(x.Layer));
                    else listGroups.Sort((x, y) => x.Layer.CompareTo(y.Layer));
                    SortFigureById(reverse);
                    return;

                case SortOption.Type:
                    if (reverse) listFigures.Sort((x, y) => y.Type.CompareTo(x.Type));
                    else listFigures.Sort((x, y) => x.Type.CompareTo(y.Type));
                    SortFigureById(reverse);
                    return;

                case SortOption.Geometry:
                    if (reverse) listFigures.Sort((x, y) => y.Geometry.CompareTo(x.Geometry));
                    else listFigures.Sort((x, y) => x.Geometry.CompareTo(y.Geometry));
                    return;

                case SortOption.ToolNR:
                    if (reverse) { listFigures.Sort((x, y) => y.ToolNr.CompareTo(x.ToolNr)); listGroups.Sort((x, y) => y.ToolNr.CompareTo(x.ToolNr)); }
                    else { listFigures.Sort((x, y) => y.ToolNr.CompareTo(x.ToolNr)); listGroups.Sort((x, y) => y.ToolNr.CompareTo(x.ToolNr)); }
                    return;

                case SortOption.ToolName:
                    if (listGroups.Count > 0)
                    { SortGroupByToolName(reverse); SortFigureById(reverse); }
                    else
                        SortFigureByToolName(reverse);
                    return;

                case SortOption.CodeSize:
                    if (reverse) listGroups.Sort((x, y) => y.CodeSize.CompareTo(x.CodeSize));
                    else listGroups.Sort((x, y) => x.CodeSize.CompareTo(y.CodeSize));
                    SortFigureById(reverse);
                    return;

                case SortOption.CodeArea:
                    if (reverse) listGroups.Sort((x, y) => y.CodeArea.CompareTo(x.CodeArea));
                    else listGroups.Sort((x, y) => x.CodeArea.CompareTo(y.CodeArea));
                    SortFigureById(reverse);
                    return;

                case SortOption.Distance:
                    SortByDistance();
                    return;

                default:
                    return;
            }
        }


        /*   public static void sortById(bool reverse = false)
           {
               if (reverse) { listFigures.Sort((x, y) => y.id.CompareTo(x.id)); listGroups.Sort((x, y) => y.id.CompareTo(x.id)); }
               else { listFigures.Sort((x, y) => x.id.CompareTo(y.id)); listGroups.Sort((x, y) => x.id.CompareTo(y.id)); }
           }*/
        public static void SortFigureById(bool reverse)
        {
            if (reverse) { listFigures.Sort((x, y) => y.Id.CompareTo(x.Id)); }
            else { listFigures.Sort((x, y) => x.Id.CompareTo(y.Id)); }
        }
        public static void SortFigureByToolName(bool reverse)
        {
            if (reverse) { listFigures.Sort((x, y) => y.ToolName.CompareTo(x.ToolName)); }
            else { listFigures.Sort((x, y) => x.ToolName.CompareTo(y.ToolName)); }
        }
        public static void SortGroupByToolName(bool reverse)
        {
            if (reverse) { listGroups.Sort((x, y) => y.ToolName.CompareTo(x.ToolName)); }
            else { listGroups.Sort((x, y) => x.ToolName.CompareTo(y.ToolName)); }
        }

        public static void ListIds()
        {
            for (int i = 0; i < listGroups.Count; i++)
            { Logger.Trace("	xmlMarker Group  {0}  Id:{1}", i, listGroups[i].Id); }

            for (int i = 0; i < listFigures.Count; i++)
            { Logger.Trace("	xmlMarker Figure {0}  Id:{1}", i, listFigures[i].Id); }
        }

        public static int[] GetFigureIdOrder()
        {
            int[] tmp = new int[listFigures.Count];
            for (int i = 0; i < listFigures.Count; i++)
            { tmp[i] = listFigures[i].Id; }
            return tmp;
        }
        public static int[] GetGroupIdOrder()
        {
            int[] tmp = new int[listGroups.Count];
            for (int i = 0; i < listGroups.Count; i++)
            { tmp[i] = listGroups[i].Id; }
            return tmp;
        }

        public static void SortByDistance()
        {
            List<BlockData> result = new List<BlockData>();
            XyPoint first = new XyPoint();
            double distanceReverse;
            BlockData tmp;
            bool allowReverse = !VisuGCode.ContainsG2G3Command() && !VisuGCode.ContainsG91Command() && !VisuGCode.ContainsTangential();

            while (listFigures.Count > 0)
            {
                for (int i = 0; i < listFigures.Count; i++)
                {
                    tmp = listFigures[i];
                    tmp.Distance = first.DistanceTo(tmp.PosStart);
                    distanceReverse = first.DistanceTo(tmp.PosEnd);
                    if (allowReverse && (distanceReverse < tmp.Distance))
                    {
                        tmp.Distance = distanceReverse;
                        tmp.Reverse = true;
                    }
                    listFigures[i] = tmp;
                }
                listFigures.Sort((x, y) => x.Distance.CompareTo(y.Distance));

                result.Add(listFigures[0]);
                first = listFigures[0].PosEnd;
                listFigures.RemoveAt(0);
            }

            listFigures.Clear();
            foreach (BlockData item in result)
                listFigures.Add(item);
        }


        public static string GetSortedCode(string[] oldCode)
        {
            if (oldCode == null) return "";

            StringBuilder tmp = new StringBuilder();
            for (int i = 0; i < XmlMarker.header.LineEnd; i++)          // copy header
            { tmp.AppendLine(oldCode[i]); }

            if (listGroups.Count > 0)
            {
                foreach (BlockData group in listGroups)            // go through all listed groups
                {
                    tmp.AppendLine(oldCode[group.LineStart]);
                    if (Gcode.LoggerTrace) Logger.Trace(culture, " AddGroup {0}", oldCode[group.LineStart]);

                    foreach (BlockData figure in listFigures)       // check if figure is within group
                    {
                        if ((figure.LineStart >= group.LineStart) && (figure.LineEnd <= group.LineEnd))
                        {
                            if (Gcode.LoggerTrace) Logger.Trace(culture, "  AddFigure {0}", oldCode[figure.LineStart]);
                            if (figure.Reverse)
                                oldCode[figure.LineStart] += "(reverse)";

                            for (int i = figure.LineStart; i <= figure.LineEnd; i++)
                            { tmp.AppendLine(oldCode[i]); }
                        }
                    }
                    tmp.AppendLine(oldCode[group.LineEnd]);
                }
            }
            else
            {
                foreach (BlockData figure in listFigures)
                {
                    if (figure.Reverse)
                        oldCode[figure.LineStart] += "(reverse)";

                    for (int i = figure.LineStart; i <= figure.LineEnd; i++)
                    { tmp.AppendLine(oldCode[i]); }
                }  // copy sorted blocks
            }

            for (int i = footer.LineStart + 1; i < oldCode.Length; i++)          // copy footer
            { tmp.AppendLine(oldCode[i]); }

            return tmp.ToString();
        }

        public static string GetAttributeValue(string element, string attribute)
        { return GetAttributeValue(element, attribute, 0); }
        public static string GetAttributeValue(string element, string attribute, int offset)
        {
            //            Logger.Trace("   getAttributeValue  element:{0}  attribute:{1}", Element, Attribute);
            if (string.IsNullOrEmpty(element)) return "";		//if (element == null) return "";
            if (string.IsNullOrEmpty(attribute)) return "";		//if (attribute == null) return "";
            int posAttribute = element.IndexOf(attribute, offset);
            if (posAttribute <= 0) return "";
        //   	if ((attribute.Length + posAttribute) <= element.Length) return "";

            int strt = element.IndexOf('"', posAttribute + attribute.Length);
            int end = element.IndexOf('"', strt + 1);
			string val = "";
			if (end > strt)
				val = element.Substring(strt + 1, (end - strt - 1));
			else
            //            if (gcode.loggerTrace)  
				Logger.Error(" getAttributeValue({0}, {1})  '{2}'  s:{3}  e:{4}", element, attribute, val, strt, end);
            return val;
        }
        public static int GetAttributeValueNumber(string element, string attribute)
        {
            //            Logger.Trace("   getAttributeValueInt  element:{0}  attribute:{1}", Element, Attribute);
            string tmp = GetAttributeValue(element, attribute);
            if (string.IsNullOrEmpty(tmp)) return -1;
            //   int att;
            if (int.TryParse(tmp, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out int att))
                return att;
            Logger.Error("getAttributeValueInt Element:{0} Attribut:{1}", element, attribute);
            return -1;
        }
        public static double GetAttributeValueDouble(string element, string attribute)
        {
            //            Logger.Trace("   getAttributeValueInt  element:{0}  attribute:{1}", Element, Attribute);
            if (element == null) return -1;
            int start = element.IndexOf(" ");
            string tmp = GetAttributeValue(element, attribute, start);
            if (string.IsNullOrEmpty(tmp)) return -1;
            //    double att;
            if (double.TryParse(tmp, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double att))
            {   //Logger.Trace("getAttributeValueDouble({0}, {1}) tmp:{2} result:{3}",Element,Attribute,tmp,att);
                return att;
            }
            Logger.Error("getAttributeValueDouble Element:{0} Attribut:{1}", element, attribute);
            return -1;
        }

        internal static BlockData SetBlockData(int lineStart, string element, int figNr)
        {
            //Logger.Trace("   setBlockData  {0}  {1}", lineStart, element);
            header.LineEnd = Math.Min(header.LineEnd, lineStart);   // lowest block-line = end of header
            BlockData tmp = new BlockData
            {
                MyIndex = 0,
                LineStart = lineStart,
                Reverse = false
            };
            tmp.Id = tmp.ToolNr = tmp.CodeSize = tmp.CodeArea = -1;
            tmp.PenWidth = tmp.PathLength = tmp.PathArea = -1;
            tmp.Geometry = tmp.Layer = tmp.Type = tmp.PenColor = tmp.ToolName = tmp.PathId = "";
            tmp.FigureNr = figNr;
            tmp.Offset = new XyPoint();

            try
            { //            if (gcode.loggerTrace) Logger.Trace("setBlockData {0}", element);
                if (element.Contains("Id")) { tmp.Id = GetAttributeValueNumber(element, "Id"); }                            // Id here and PathId below, should work anyway, because 'Id' comes first in XML-Tag
                if (element.Contains("ToolNr")) { tmp.ToolNr = GetAttributeValueNumber(element, "ToolNr"); }
                if (element.Contains("ToolName")) { tmp.ToolName = GetAttributeValue(element, "ToolName"); }
                if (element.Contains("PathLength")) { tmp.PathLength = GetAttributeValueDouble(element, "PathLength"); }
                if (element.Contains("PathArea")) { tmp.PathArea = GetAttributeValueDouble(element, "PathArea"); }
                if (element.Contains("PathId")) { tmp.PathId = GetAttributeValue(element, "PathId"); }                      // ...Id here
                if (element.Contains("Layer")) { tmp.Layer = GetAttributeValue(element, "Layer"); }
                if (element.Contains("Type")) { tmp.Type = GetAttributeValue(element, "Type"); }
                if (element.Contains("CodeSize")) { tmp.CodeSize = GetAttributeValueNumber(element, "CodeSize"); }
                if (element.Contains("CodeArea")) { tmp.CodeArea = GetAttributeValueNumber(element, "CodeArea"); }
                if (element.Contains("Geometry")) { tmp.Geometry = GetAttributeValue(element, "Geometry"); }
                if (element.Contains("PenColor")) { tmp.PenColor = GetAttributeValue(element, "PenColor"); }
                if (element.Contains("PenWidth")) { tmp.PenWidth = GetAttributeValueDouble(element, "PenWidth"); }
                if (element.Contains("OffsetX")) { XyPoint tmpPoint = tmp.Offset; tmpPoint.X = GetAttributeValueDouble(element, "OffsetX"); tmp.Offset = tmpPoint; }
                if (element.Contains("OffsetY")) { XyPoint tmpPoint = tmp.Offset; tmpPoint.Y = GetAttributeValueDouble(element, "OffsetY"); tmp.Offset = tmpPoint; }
            }
            catch (Exception err)
            {
                Logger.Error(err, " SetBlockData {0} ", element);
            }
            return tmp;
        }


        /*************************************************************************************
		 **** Figures
		 *************************************************************************************/
        public static void AddFigure(int lineStart, string element, int figureNR)
        {
            if (element != null)
                tmpFigure = SetBlockData(lineStart, element, figureNR);
        }

        public static void FinishFigure(int lineEnd)
        {
            tmpFigure.LineEnd = lineEnd;
            tmpFigure.MyIndex = listFigures.Count;

            if (!string.IsNullOrEmpty(tmpFigure.PenColor))
            {
                figurePenColorAnyCount++;

                if (tmpFigure.PenColor.Contains("none"))
                    figurePenColorNoneCount++;
            }
            listFigures.Add(tmpFigure);
            footer.LineStart = footer.LineEnd = Math.Max(footer.LineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetFigureCount()
        { return listFigures.Count; }

        public static bool GetFigureByFigureNR(int figNR)
        {
            foreach (BlockData tmp in listFigures)
            {
                if (tmp.FigureNr == figNR)
                {
                    tmpFigure = tmp;
                    return true;
                }
            }
            return false;
        }

        internal static bool GetFigureFirst(ref BlockData tmp)
        {
            if (listFigures.Count > 0)
            { tmp = listFigures[0]; return true; }
            return false;
        }
        internal static bool GetFigurePrev(ref BlockData tmp)
        {
            if ((listFigures.Count > 0) && (lastFigure.MyIndex > 0))
            { tmp = listFigures[lastFigure.MyIndex - 1]; return true; }
            return false;
        }
        internal static bool GetFigureNext(ref BlockData tmp)
        {
            if ((listFigures.Count > 0) && (lastFigure.MyIndex < listFigures.Count - 1))
            { tmp = listFigures[lastFigure.MyIndex + 1]; return true; }
            return false;
        }
        internal static bool GetFigureLast(ref BlockData tmp)
        {
            if (listFigures.Count > 0)
            { tmp = listFigures[listFigures.Count - 1]; return true; }
            return false;
        }

        public static bool GetFigure(int lineNR)
        { return GetFigure(lineNR, 0); }
        public static bool GetFigure(int lineNR, int search)
        {
            if (listFigures.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    if (lastFigure.MyIndex > 0)
                    { lastFigure = listFigures[lastFigure.MyIndex - 1]; return true; }
                    return false;
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    if (lastFigure.MyIndex < listFigures.Count - 1)
                    { lastFigure = listFigures[lastFigure.MyIndex + 1]; return true; }
                    return false;
                }
                else
                {
                    foreach (BlockData tmp in listFigures)
                    {
                        if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))
                        {
                            lastFigure = tmp;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int FindInsertPositionFigureMostTop(int current)
        {
            int start = 0;
            if (GetGroup(current))      // only check within same group
                start = lastGroup.LineStart;
            foreach (BlockData tmp in listFigures)
            {
                if (tmp.LineStart > start)
                { return tmp.LineStart; }
            }
            return -1;
        }
        public static int FindInsertPositionFigureTop(int current)
        {
            int start = 0;
            if (GetGroup(current))      // only check within same group
                start = lastGroup.LineStart;
            for (int i = 1; i < listFigures.Count; i++)
            {
                if (listFigures[i - 1].LineStart > start)
                {
                    if ((current >= listFigures[i].LineStart) && (current <= listFigures[i].LineEnd))
                    { return listFigures[i - 1].LineStart; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionFigureMostBottom(int current)
        {
            int end;
            if (GetGroup(current))      // only check within same group
                end = lastGroup.LineEnd;
            else
                return listFigures[listFigures.Count - 1].LineEnd + 1;
            int lastEnd = 0;
            foreach (BlockData tmp in listFigures)
            {
                if (tmp.LineStart >= end)      //
                { return lastEnd + 1; }
                lastEnd = tmp.LineEnd;
            }
            return -1;
        }
        public static int FindInsertPositionFigureBottom(int current)
        {
            int end = listFigures[listFigures.Count - 1].LineEnd;
            if (GetGroup(current))      // only check within same group
                end = lastGroup.LineEnd;
            for (int i = 0; i < listFigures.Count - 1; i++)
            {
                if (listFigures[i + 1].LineEnd <= end)
                {
                    if ((current >= listFigures[i].LineStart) && (current <= listFigures[i].LineEnd))
                    { return listFigures[i + 1].LineEnd + 1; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionFigureNext(int current)
        {
            int end = listFigures[listFigures.Count - 1].LineEnd;
            if (GetGroup(current))      // only check within same group
                end = lastGroup.LineEnd;
            for (int i = 0; i < listFigures.Count; i++)
            {
                if (listFigures[i].LineEnd <= end)
                {
                    if ((current >= listFigures[i].LineStart) && (current <= listFigures[i].LineEnd))
                    { return listFigures[i].LineEnd + 1; }
                }
            }
            return -1;
        }

        public static bool IsFoldingMarkerFigure(int line)
        {
            foreach (BlockData tmp in listFigures)
            {
                if ((line == tmp.LineStart) || (line == tmp.LineEnd))
                { return true; }
            }
            return false;
        }
		
		public static void ListAllFigures()
		{			
            for (int i = 0; i < listFigures.Count; i++)
            {
				Logger.Trace("ListAllFigures i:{0}  LineStart:{1}  LineEnd:{2}  Id:{3}  FigureNr:{4}", i, listFigures[i].LineStart, listFigures[i].LineEnd, listFigures[i].Id, listFigures[i].FigureNr);
			}
		}



        /*************************************************************************************
		 **** Groups
		 *************************************************************************************/
        public static void AddGroup(int lineStart, string element, int figureNR)
        {
            if (element != null)
            {
                tmpGroup = SetBlockData(lineStart, element, figureNR);
                if (logEnable) Logger.Trace("AddGroup color:{0}  width{1}", tmpGroup.PenColor, tmpGroup.PenWidth);
            }
        }

        public static void FinishGroup(int lineEnd)
        {
            tmpGroup.LineEnd = lineEnd;
            tmpGroup.MyIndex = listGroups.Count;
            listGroups.Add(tmpGroup);
            footer.LineStart = footer.LineEnd = Math.Max(footer.LineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetGroupCount()
        { return listGroups.Count; }

        internal static bool GetGroupFirst(ref BlockData tmp)
        {
            if (listGroups.Count > 0)
            { tmp = listGroups[0]; return true; }
            return false;
        }
        internal static bool GetGroupPrev(ref BlockData tmp)
        {
            if ((listGroups.Count > 0) && (lastGroup.MyIndex > 0))
            { tmp = listGroups[lastGroup.MyIndex - 1]; return true; }
            return false;
        }
        internal static bool GetGroupNext(ref BlockData tmp)
        {
            if ((listGroups.Count > 0) && (lastGroup.MyIndex < listGroups.Count - 1))
            { tmp = listGroups[lastGroup.MyIndex + 1]; return true; }
            return false;
        }
        internal static bool GetGroupLast(ref BlockData tmp)
        {
            if (listGroups.Count > 0)
            { tmp = listGroups[listGroups.Count - 1]; return true; }
            return false;
        }

        public static bool GetGroup(int lineNR)
        { return GetGroup(lineNR, 0); }

        public static int GetStartLineOfGroup(int index)
        {
            if ((index >=0) && (index < listGroups.Count))
                return listGroups[index].LineStart;
            return -1;
        }

        public static bool GetGroup(int lineNR, int search)
        {
            if (listGroups.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    if (lastGroup.MyIndex > 0)
                    { lastGroup = listGroups[lastGroup.MyIndex - 1]; return true; }
                    return false;
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    if (lastGroup.MyIndex < listGroups.Count - 1)
                    { lastGroup = listGroups[lastGroup.MyIndex + 1]; return true; }
                    return false;
                }
                else
                {
                    foreach (BlockData tmp in listGroups)
                    {
                        if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))
                        {
                            lastGroup = tmp;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int FindInsertPositionGroupMostTop()//int current)
        {
            if (listGroups.Count > 0)
                return listGroups[0].LineStart;
            return -1;
        }
        public static int FindInsertPositionGroupTop(int current)
        {
            if (listGroups.Count > 1)
            {
                for (int i = 1; i < listGroups.Count; i++)
                {
                    if ((current >= listGroups[i].LineStart) && (current <= listGroups[i].LineEnd))
                    { return listGroups[i - 1].LineStart; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionGroupMostBottom()//int current)
        {
            if (listGroups.Count > 0)
                return listGroups[listGroups.Count - 1].LineEnd + 1;
            return -1;
        }
        public static int FindInsertPositionGroupBottom(int current)
        {
            if (listGroups.Count > 1)
            {
                for (int i = 1; i < listGroups.Count - 1; i++)
                {
                    if ((current >= listGroups[i].LineStart) && (current <= listGroups[i].LineEnd))
                    { return listGroups[i + 1].LineEnd + 1; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionGroupNext(int current)
        {
            if (listGroups.Count >= 1)
            {	
				int i = listGroups.Count - 1;
				if ((current >= listGroups[i].LineStart) && (current <= listGroups[i].LineEnd))	// if last group
				{ return listGroups[i].LineEnd + 1; }
				
                for (i = 0; i < listGroups.Count - 1; i++)
                {
                    if ((current >= listGroups[i].LineStart) && (current <= listGroups[i].LineEnd))
                    { return listGroups[i + 1].LineStart; }
                }
            }
            return -1;
        }

        public static bool IsFoldingMarkerGroup(int line)
        {
            foreach (BlockData tmp in listGroups)
            {
                if ((line == tmp.LineStart) || (line == tmp.LineEnd))
                { return true; }
            }
            return false;
        }

		public static void ListAllGroups()
		{			
            for (int i = 0; i < listGroups.Count; i++)
            {
				Logger.Trace("ListAllGroups i:{0}  LineStart:{1}  LineEnd:{2}  Id:{3}  FigureNr:{4}", i, listGroups[i].LineStart, listGroups[i].LineEnd, listGroups[i].Id, listGroups[i].FigureNr);
			}
		}


        /*************************************************************************************
		 **** Tiles
		 *************************************************************************************/
        public static void AddTile(int lineStart, string element, int figureNR)
        {
            if (element != null)
            {
                tmpTile = SetBlockData(lineStart, element, figureNR);
                if (logEnable)
                    Logger.Trace("AddTile line:{0}  figNr:{1}  xml:{2}", lineStart, figureNR, element);
            }
        }

        public static void FinishTile(int lineEnd)
        {
            tmpTile.LineEnd = lineEnd;
            tmpTile.MyIndex = listTiles.Count;
            listTiles.Add(tmpTile);
            footer.LineStart = footer.LineEnd = Math.Max(footer.LineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetTileCount()
        { return listTiles.Count; }

        public static bool GetTile(int lineNR)
        { return GetTile(lineNR, 0); }
        public static bool GetTile(int lineNR, int search)
        {
            if (listTiles.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    if (lastTile.MyIndex > 0)
                    { lastTile = listTiles[lastTile.MyIndex - 1]; return true; }
                    return false;
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    if (lastTile.MyIndex < listTiles.Count - 1)
                    { lastTile = listTiles[lastTile.MyIndex + 1]; return true; }
                    return false;
                }
                else
                {
                    foreach (BlockData tmp in listTiles)
                    {
                        if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))
                        {
                            lastTile = tmp;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool IsFoldingMarkerTile(int line)
        {
            foreach (BlockData tmp in listTiles)
            {
                if ((line == tmp.LineStart) || (line == tmp.LineEnd))
                { return true; }
            }
            return false;
        }

       
        /*************************************************************************************
		 **** Collections
		 *************************************************************************************/
        public static void AddCollection(int lineStart, string element, int figureNR)
        {
            if (element != null)
            {
                tmpCollection = SetBlockData(lineStart, element, figureNR);
                if (logEnable) Logger.Trace("AddCollection color:{0}  width{1}", tmpCollection.PenColor, tmpCollection.PenWidth);
            }
        }

        public static void FinishCollection(int lineEnd)
        {
            tmpCollection.LineEnd = lineEnd;
            tmpCollection.MyIndex = listCollections.Count;
            listCollections.Add(tmpCollection);
            footer.LineStart = footer.LineEnd = Math.Max(footer.LineStart, lineEnd);   // highest block-line = start of footer
            //Logger.Info("FinishCollection {0}  {1}", tmpCollection.LineStart, tmpCollection.LineEnd);
        }
        public static int FindInsertPositionCollectionMostTop()//int current)
        {
            if (listCollections.Count > 0)
                return listCollections[0].LineStart;
            return -1;
        }
        public static int FindInsertPositionCollectionMostBottom()//int current)
        {
            if (listCollections.Count > 0)
                return listCollections[listCollections.Count - 1].LineEnd + 1;
            return -1;
        }
        public static int GetCollectionCount()
        { return listCollections.Count; }

        public static bool GetCollection(int lineNR)
        { return GetCollection(lineNR, 0); }
        public static bool GetCollection(int lineNR, int search)
        {
            if (listCollections.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    if (lastCollection.MyIndex > 0)
                    { lastCollection = listCollections[lastCollection.MyIndex - 1]; return true; }
                    return false;
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    if (lastCollection.MyIndex < listCollections.Count - 1)
                    { lastCollection = listCollections[lastCollection.MyIndex + 1]; return true; }
                    return false;
                }
                else
                {
                    foreach (BlockData tmp in listCollections)
                    {
                        if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))
                        {
                            lastCollection = tmp;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool IsFoldingMarkerCollection(int line)
        {
            foreach (BlockData tmp in listCollections)
            {
                if ((line == tmp.LineStart) || (line == tmp.LineEnd))
                { return true; }
            }
            return false;
        }

    }
}
