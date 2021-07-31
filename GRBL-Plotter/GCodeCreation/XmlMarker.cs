/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2021 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

//#pragma warning disable CA1303	// Do not pass literals as localized parameters
//#pragma warning disable CA1307

namespace GrblPlotter
{
    public enum XmlMarkerType { None, Tile, Group, Figure, Path, Pass, Contour, Fill, Line };
    public static class XmlMarker
    {   // order by hierachy / importance
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

        internal class BlockData
        {
            public int LineStart { get; set; }           // line nr. in editor
            public int LineEnd { get; set; }             // line nr. in editor
            public XyPoint PosStart { get; set; }        // xy position
            public XyPoint PosEnd { get; set; }
            public double Distance { get; set; }
            public bool Reverse { get; set; }
            public int FigureNr { get; set; }

            public int Id { get; set; }                  // block informations
            public int ToolNr { get; set; }
            public int CodeSize { get; set; }
            public int CodeArea { get; set; }
            public int PathId { get; set; }
            public double PathLength { get; set; }
            public double PathArea { get; set; }
            public double PenWidth { get; set; }

            public string Geometry { get; set; }
            public string PenColor { get; set; }
            public string ToolName { get; set; }
            public string Layer { get; set; }
            public string Type { get; set; }
        };

        private static readonly List<BlockData> listFigures = new List<BlockData>();
        private static readonly List<BlockData> listGroups = new List<BlockData>();
        private static readonly List<BlockData> listTiles = new List<BlockData>();

        internal static BlockData tmpFigure = new BlockData();
        internal static BlockData tmpGroup = new BlockData();
        internal static BlockData tmpTile = new BlockData();

        internal static BlockData lastFigure = new BlockData();
        internal static BlockData lastGroup = new BlockData();
        internal static BlockData lastTile = new BlockData();
        internal static BlockData header = new BlockData();
        internal static BlockData footer = new BlockData();

        public enum SortOption { Id, Color, Width, Layer, Type, Geometry, ToolNR, ToolName, CodeSize, CodeArea, Distance };

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static uint logFlags = 0;
        private static bool logEnable = false;


        public static void Reset()
        {
            listFigures.Clear(); listGroups.Clear(); listTiles.Clear();
            header.LineStart = 0; header.LineEnd = 999999;
            footer.LineStart = footer.LineEnd = 0;

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
                    CortFigureById(reverse);
                    return;

                case SortOption.Type:
                    if (reverse) listFigures.Sort((x, y) => y.Type.CompareTo(x.Type));
                    else listFigures.Sort((x, y) => x.Type.CompareTo(y.Type));
                    CortFigureById(reverse);
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
                    { SortGroupByToolName(reverse); CortFigureById(reverse); }
                    else
                        SortFigureByToolName(reverse);
                    return;

                case SortOption.CodeSize:
                    if (reverse) listGroups.Sort((x, y) => y.CodeSize.CompareTo(x.CodeSize));
                    else listGroups.Sort((x, y) => x.CodeSize.CompareTo(y.CodeSize));
                    CortFigureById(reverse);
                    return;

                case SortOption.CodeArea:
                    if (reverse) listGroups.Sort((x, y) => y.CodeArea.CompareTo(x.CodeArea));
                    else listGroups.Sort((x, y) => x.CodeArea.CompareTo(y.CodeArea));
                    CortFigureById(reverse);
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
        public static void CortFigureById(bool reverse)
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
            if (element == null) return "";
            if (attribute == null) return "";
            int posAttribute = element.IndexOf(attribute, offset);
            if (posAttribute <= 0) return "";
            int strt = element.IndexOf('"', posAttribute + attribute.Length);
            int end = element.IndexOf('"', strt + 1);
            string val = element.Substring(strt + 1, (end - strt - 1));
            //            if (gcode.loggerTrace)  Logger.Trace(" getAttributeValue({0}, {1})  '{2}'  s:{3}  e:{4}", Element, Attribute, val, strt, end);
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



        /*************************************************************************************/
        public static void AddFigure(int lineStart, string element, int figureNR)
        {
            if (element != null)
                tmpFigure = SetBlockData(lineStart, element, figureNR);
            //    if (logEnable) Logger.Trace("AddFigure color:{0}  width{1}", tmpFigure.penColor, tmpFigure.penWidth);
            //           if (gcode.loggerTrace) Logger.Trace("AddFigure Line {0}  Id {1}  Geometry {2}", lineStart, tmpFigure.id, tmpFigure.geometry);
        }

        internal static BlockData SetBlockData(int lineStart, string element, int figNr)
        {
            //            Logger.Trace("   setBlockData");
            header.LineEnd = Math.Min(header.LineEnd, lineStart);   // lowest block-line = end of header
            BlockData tmp = new BlockData();
            tmp.LineStart = lineStart; tmp.Reverse = false;
            tmp.Id = tmp.ToolNr = tmp.CodeSize = tmp.CodeArea = tmp.PathId = -1;
            tmp.PenWidth = tmp.PathLength = tmp.PathArea = -1;
            tmp.Geometry = tmp.Layer = tmp.Type = tmp.PenColor = tmp.ToolName = "";
            tmp.FigureNr = figNr;
            //            if (gcode.loggerTrace) Logger.Trace("setBlockData {0}", element);
            if (element.Contains("Id")) { tmp.Id = GetAttributeValueNumber(element, "Id"); }
            if (element.Contains("ToolNr")) { tmp.ToolNr = GetAttributeValueNumber(element, "ToolNr"); }
            if (element.Contains("ToolName")) { tmp.ToolName = GetAttributeValue(element, "ToolName"); }
            if (element.Contains("PathLength")) { tmp.PathLength = GetAttributeValueDouble(element, "PathLength"); }
            if (element.Contains("PathArea")) { tmp.PathArea = GetAttributeValueDouble(element, "PathArea"); }
            if (element.Contains("PathId")) { tmp.PathId = GetAttributeValueNumber(element, "PathId"); }
            if (element.Contains("Layer")) { tmp.Layer = GetAttributeValue(element, "Layer"); }
            if (element.Contains("Type")) { tmp.Type = GetAttributeValue(element, "Type"); }
            if (element.Contains("CodeSize")) { tmp.CodeSize = GetAttributeValueNumber(element, "CodeSize"); }
            if (element.Contains("CodeArea")) { tmp.CodeArea = GetAttributeValueNumber(element, "CodeArea"); }
            if (element.Contains("Geometry")) { tmp.Geometry = GetAttributeValue(element, "Geometry"); }
            if (element.Contains("PenColor")) { tmp.PenColor = GetAttributeValue(element, "PenColor"); }
            if (element.Contains("PenWidth")) { tmp.PenWidth = GetAttributeValueDouble(element, "PenWidth"); }
            //          if (element.Contains("ToolName")) { tmp.toolName = getAttributeValue(element, "ToolName"); }
            //            Logger.Trace("   setBlockData finish");
            return tmp;
        }


        public static void FinishFigure(int lineEnd)
        {
            tmpFigure.LineEnd = lineEnd;
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
        public static bool GetFigure(int lineNR)
        { return GetFigure(lineNR, 0); }
        public static bool GetFigure(int lineNR, int search)
        {
            if (listFigures.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    BlockData tmp = listFigures[0];
                    if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))   // actual block is first block
                        return false;

                    lastFigure.LineStart = listFigures[0].LineStart;
                    for (int i = 1; i < listFigures.Count; i++)
                    {
                        if ((lineNR >= listFigures[i].LineStart) && (lineNR <= listFigures[i].LineEnd))
                        {
                            lastFigure.LineEnd = listFigures[i - 1].LineEnd;
                            if (search == -1)
                                lastFigure.LineStart = listFigures[i - 1].LineStart;
                            return true;
                        }
                    }
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    BlockData tmp = listFigures[listFigures.Count - 1];
                    if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))   // actual block is last block
                        return false;

                    lastFigure.LineEnd = listFigures[listFigures.Count - 1].LineEnd;
                    for (int i = listFigures.Count - 1; i >= 0; i--)
                    {
                        if ((lineNR >= listFigures[i].LineStart) && (lineNR <= listFigures[i].LineEnd))
                        {
                            lastFigure.LineStart = listFigures[i + 1].LineStart;
                            if (search == 1)
                                lastFigure.LineEnd = listFigures[i + 1].LineEnd;
                            return true;
                        }
                    }
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
        public static bool IsFoldingMarkerFigure(int line)
        {
            foreach (BlockData tmp in listFigures)
            {
                if ((line == tmp.LineStart) || (line == tmp.LineEnd))
                { return true; }
            }
            return false;
        }

        /**********************************************************************************/
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
            listGroups.Add(tmpGroup);
            footer.LineStart = footer.LineEnd = Math.Max(footer.LineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetGroupCount()
        { return listGroups.Count; }

        public static bool GetGroup(int lineNR)
        { return GetGroup(lineNR, 0); }
        public static bool GetGroup(int lineNR, int search)
        {
            if (listGroups.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    BlockData tmp = listGroups[0];
                    if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))   // actual block is first block
                        return false;

                    lastGroup.LineStart = listGroups[0].LineStart;
                    for (int i = 1; i < listGroups.Count; i++)
                    {
                        if ((lineNR >= listGroups[i].LineStart) && (lineNR <= listGroups[i].LineEnd))
                        {
                            lastGroup.LineEnd = listGroups[i - 1].LineEnd;
                            if (search == -1)
                                lastGroup.LineStart = listGroups[i - 1].LineStart;
                            return true;
                        }
                    }
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    BlockData tmp = listGroups[listGroups.Count - 1];
                    if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))   // actual block is last block
                        return false;

                    lastGroup.LineEnd = listGroups[listGroups.Count - 1].LineEnd;
                    for (int i = listGroups.Count - 1; i >= 0; i--)
                    {
                        if ((lineNR >= listGroups[i].LineStart) && (lineNR <= listGroups[i].LineEnd))
                        {
                            lastGroup.LineStart = listGroups[i + 1].LineStart;
                            if (search == 1)
                                lastGroup.LineEnd = listGroups[i + 1].LineEnd;
                            return true;
                        }
                    }
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
            if (listGroups.Count > 1)
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
        public static bool IsFoldingMarkerGroup(int line)
        {
            foreach (BlockData tmp in listGroups)
            {
                if ((line == tmp.LineStart) || (line == tmp.LineEnd))
                { return true; }
            }
            return false;
        }

        /*************************************************************************************/
        public static void AddTile(int lineStart, string element, int figureNR)
        {
            if (element != null)
            {
                tmpTile = SetBlockData(lineStart, element, figureNR);
                if (logEnable) Logger.Trace("AddTile ");
            }
        }

        public static void FinishTile(int lineEnd)
        {
            tmpTile.LineEnd = lineEnd;
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
                    BlockData tmp = listTiles[0];
                    if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))   // actual block is first block
                        return false;

                    lastTile.LineStart = listTiles[0].LineStart;
                    for (int i = 1; i < listTiles.Count; i++)
                    {
                        if ((lineNR >= listTiles[i].LineStart) && (lineNR <= listTiles[i].LineEnd))
                        {
                            lastTile.LineEnd = listTiles[i - 1].LineEnd;
                            if (search == -1)
                                lastTile.LineStart = listTiles[i - 1].LineStart;
                            return true;
                        }
                    }
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    BlockData tmp = listTiles[listTiles.Count - 1];
                    if ((lineNR >= tmp.LineStart) && (lineNR <= tmp.LineEnd))   // actual block is last block
                        return false;

                    lastTile.LineEnd = listTiles[listTiles.Count - 1].LineEnd;
                    for (int i = listTiles.Count - 1; i >= 0; i--)
                    {
                        if ((lineNR >= listTiles[i].LineStart) && (lineNR <= listTiles[i].LineEnd))
                        {
                            lastTile.LineStart = listTiles[i + 1].LineStart;
                            if (search == 1)
                                lastTile.LineEnd = listTiles[i + 1].LineEnd;
                            return true;
                        }
                    }
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

    }
}
