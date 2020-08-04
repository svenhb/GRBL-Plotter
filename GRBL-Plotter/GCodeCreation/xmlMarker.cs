/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2019-2020 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRBL_Plotter
{
    public enum xmlMarkerType { none, Group, Figure, Path, Pass, Contour, Fill, Line };
    public static class xmlMarker
    {
        public const string groupStart = "<Group";
        public const string groupEnd = "</Group";
        public const string figureStart = "<Figure";
        public const string figureEnd = "</Figure";
        public const string pathStart = "<Path";
        public const string pathEnd = "</Path";
        public const string passStart = "<Pass";
        public const string passEnd = "</Pass";
        public const string contourStart = "<Contour";
        public const string contourEnd = "</Contour";
        public const string fillStart = "<Fill";
        public const string fillEnd = "</Fill";
        public const string revolutionStart = "<Revolution";
        public const string revolutionEnd = "</Revolution";
        public const string clearanceStart = "<Clearance";
        public const string clearanceEnd = "</Clearance";
        public const string headerStart = "<Header";
        public const string headerEnd = "</Header";

        public const string tangentialAxis = "<Tangential";

        public struct BlockData
        {
            public int lineStart;           // line nr. in editor
            public int lineEnd;             // line nr. in editor
            public xyPoint posStart;        // xy position
            public xyPoint posEnd;
            public double distance;
            public bool reverse;
            public int figureNr;

            public int id;                  // block informations
            public int toolNr;
            public int codeSize;
            public int codeArea;
            public int pathId;        
            public double pathLength;
            public double pathArea;
            public double penWidth;

            public string geometry;
            public string penColor;
            public string toolName;
            public string layer;
        };

        private static List<BlockData> listFigures = new List<BlockData>();
        private static List<BlockData> listGroups = new List<BlockData>();
        public static BlockData tmpFigure = new BlockData();
        private static BlockData tmpGroup = new BlockData();
        public static BlockData lastFigure = new BlockData();
        public static BlockData lastGroup = new BlockData();
        public static BlockData header = new BlockData();
        public static BlockData footer = new BlockData();

//        public enum sortItem { id, geometry, toolNr, toolName, layer, color, codeSize, codeArea };

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Reset()
        {
            listFigures.Clear(); listGroups.Clear();
            header.lineStart = 0; header.lineEnd = 999999;
            footer.lineStart = footer.lineEnd = 0;
        }

        public static void sortById(bool reverse = false)
        {
            if (reverse) { listFigures.Sort((x, y) => y.id.CompareTo(x.id)); listGroups.Sort((x, y) => y.id.CompareTo(x.id)); }
            else { listFigures.Sort((x, y) => x.id.CompareTo(y.id)); listGroups.Sort((x, y) => x.id.CompareTo(y.id)); }
        }
        public static void sortFigureById(bool reverse = false)
        {
            if (reverse) { listFigures.Sort((x, y) => y.id.CompareTo(x.id)); }
            else { listFigures.Sort((x, y) => x.id.CompareTo(y.id)); }
        }
        public static void sortByGeometry(bool reverse = false)
        {
            if (reverse) listFigures.Sort((x, y) => y.geometry.CompareTo(x.geometry));
            else listFigures.Sort((x, y) => x.geometry.CompareTo(y.geometry));
        }
        public static void sortByToolNr(bool reverse = false)
        {
            if (reverse) { listFigures.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); listGroups.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); }
            else { listFigures.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); listGroups.Sort((x, y) => y.toolNr.CompareTo(x.toolNr)); }
        }
        public static void sortByToolName(bool reverse = false)
        {
            if (listGroups.Count > 0)
            { sortGroupByToolName(reverse); sortFigureById(reverse); }
            else
                sortFigureByToolName(reverse);
        }
        public static void sortFigureByToolName(bool reverse = false)
        {
            if (reverse) { listFigures.Sort((x, y) => y.toolName.CompareTo(x.toolName)); }
            else { listFigures.Sort((x, y) => x.toolName.CompareTo(y.toolName)); }
        }
        public static void sortGroupByToolName(bool reverse = false)
        {
            if (reverse) { listGroups.Sort((x, y) => y.toolName.CompareTo(x.toolName)); }
            else { listGroups.Sort((x, y) => x.toolName.CompareTo(y.toolName)); }
        }
        public static void sortByLayer(bool reverse = false)
        {
            if (reverse) listGroups.Sort((x, y) => y.layer.CompareTo(x.layer));
            else listGroups.Sort((x, y) => x.layer.CompareTo(y.layer));
            sortFigureById(reverse);
        }
        public static void sortByColor(bool reverse = false)
        {
            if (reverse) listFigures.Sort((x, y) => y.penColor.CompareTo(x.penColor));
            else listFigures.Sort((x, y) => x.penColor.CompareTo(y.penColor));
        }
        public static void sortByCodeSize(bool reverse = false)
        {
            if (reverse) listGroups.Sort((x, y) => y.codeSize.CompareTo(x.codeSize));
            else listGroups.Sort((x, y) => x.codeSize.CompareTo(y.codeSize));
            sortFigureById(reverse);
        }
        public static void sortByCodeArea(bool reverse = false)
        {
            if (reverse) listGroups.Sort((x, y) => y.codeArea.CompareTo(x.codeArea));
            else listGroups.Sort((x, y) => x.codeArea.CompareTo(y.codeArea));
            sortFigureById(reverse);
        }

        public static void sortByDistance()
        {
            List<BlockData> result = new List<BlockData>();
            xyPoint first = new xyPoint();
            double distanceReverse;
            BlockData tmp;
            bool allowReverse = !VisuGCode.containsG2G3Command() && !VisuGCode.containsG91Command() && !VisuGCode.containsTangential();

            while (listFigures.Count > 0)
            {
                for (int i = 0; i < listFigures.Count; i++)
                {
                    tmp = listFigures[i];
                    tmp.distance = first.DistanceTo(tmp.posStart);
                    distanceReverse = first.DistanceTo(tmp.posEnd);
                    if (allowReverse && (distanceReverse < tmp.distance))
                    {
                        tmp.distance = distanceReverse;
                        tmp.reverse = true;
                    }
                    listFigures[i] = tmp;
                }
                listFigures.Sort((x, y) => x.distance.CompareTo(y.distance));

                result.Add(listFigures[0]);
                first = listFigures[0].posEnd;
                listFigures.RemoveAt(0);
            }

            listFigures.Clear();
            foreach (BlockData item in result)
                listFigures.Add(item);
        }


        public static string getSortedCode(string[] oldCode)
        {
            StringBuilder tmp = new StringBuilder();

            for (int i = 0; i < xmlMarker.header.lineEnd; i++)          // copy header
            { tmp.AppendLine(oldCode[i]); }

            if (listGroups.Count > 0)
            {
                foreach (BlockData group in listGroups)            // go through all listed groups
                {
                    tmp.AppendLine(oldCode[group.lineStart]);
                    if (gcode.loggerTrace) Logger.Trace(" AddGroup {0}", oldCode[group.lineStart]);

                    foreach (BlockData figure in listFigures)       // check if figure is within group
                    {
                        if ((figure.lineStart >= group.lineStart) && (figure.lineEnd <= group.lineEnd))
                        {
                            if (gcode.loggerTrace) Logger.Trace("  AddFigure {0}", oldCode[figure.lineStart]);
                            if (figure.reverse)
                                oldCode[figure.lineStart] += "(reverse)";

                            for (int i = figure.lineStart; i <= figure.lineEnd; i++)
                            { tmp.AppendLine(oldCode[i]); }
                        }
                    }
                    tmp.AppendLine(oldCode[group.lineEnd]);
                }
            }
            else
            {
                foreach (BlockData figure in listFigures)
                {
                    if (figure.reverse)
                        oldCode[figure.lineStart] += "(reverse)";

                    for (int i = figure.lineStart; i <= figure.lineEnd; i++)
                    { tmp.AppendLine(oldCode[i]); }
                }  // copy sorted blocks
            }

            for (int i = footer.lineStart + 1; i < oldCode.Length; i++)          // copy footer
            { tmp.AppendLine(oldCode[i]); }

            return tmp.ToString();
        }


        public static string getAttributeValue(string Element, string Attribute)
        {
//            Logger.Trace("   getAttributeValue  element:{0}  attribute:{1}", Element, Attribute);
            int posAttribute = Element.IndexOf(Attribute);
            if (posAttribute <= 0) return "";
            int strt = Element.IndexOf('"', posAttribute + Attribute.Length);
            int end = Element.IndexOf('"', strt + 1);
            string val = Element.Substring(strt + 1, (end - strt - 1));
            //            if (gcode.loggerTrace) Logger.Trace(" getAttributeValue {0}  {1}  {2}", Element, Attribute, val);
            return val;
        }
        public static int getAttributeValueInt(string Element, string Attribute)
        {
//            Logger.Trace("   getAttributeValueInt  element:{0}  attribute:{1}", Element, Attribute);
            string tmp = getAttributeValue(Element, Attribute);
            if (tmp == "") return -1;
            int att;
            if (int.TryParse(tmp, out att))
                return att;
            Logger.Error("getAttributeValueInt Element:{0} Attribut:{1}",Element, Attribute);
            return -1;
        }
        public static double getAttributeValueDouble(string Element, string Attribute)
        {
            //            Logger.Trace("   getAttributeValueInt  element:{0}  attribute:{1}", Element, Attribute);
            string tmp = getAttributeValue(Element, Attribute);
            if (tmp == "") return -1;
            double att;
            if (double.TryParse(tmp, out att))
                return att;
            Logger.Error("getAttributeValueDouble Element:{0} Attribut:{1}", Element, Attribute);
            return -1;
        }

        public static void AddFigure(int lineStart, string element, int figureNr)
        {
            tmpFigure = setBlockData(lineStart, element, figureNr);
            //           if (gcode.loggerTrace) Logger.Trace("AddFigure Line {0}  Id {1}  Geometry {2}", lineStart, tmpFigure.id, tmpFigure.geometry);
        }

        public static BlockData setBlockData(int lineStart, string element, int figNr)
        {
//            Logger.Trace("   setBlockData");
            header.lineEnd = Math.Min(header.lineEnd, lineStart);   // lowest block-line = end of header
            BlockData tmp = new BlockData();
            tmp.lineStart = lineStart; tmp.reverse = false;
            tmp.id = tmp.toolNr = tmp.codeSize = tmp.codeArea = tmp.pathId = - 1;
            tmp.penWidth = tmp.pathLength  = tmp.pathArea  = - 1;
            tmp.geometry = tmp.layer = tmp.penColor = tmp.toolName = "";
            tmp.figureNr = figNr;
            //            if (gcode.loggerTrace) Logger.Trace("setBlockData {0}", element);
            if (element.Contains("Id"))       { tmp.id = getAttributeValueInt(element, "Id"); }
            if (element.Contains("ToolNr"))   { tmp.toolNr = getAttributeValueInt(element, "ToolNr"); }
            if (element.Contains("ToolName")) { tmp.toolName = getAttributeValue(element, "ToolName"); }
            if (element.Contains("PathLength")) { tmp.pathLength = getAttributeValueDouble(element, "PathLength"); }
            if (element.Contains("PathArea")) { tmp.pathArea = getAttributeValueDouble(element, "PathArea"); }
            if (element.Contains("PathId"))   { tmp.pathId = getAttributeValueInt(element, "PathId"); }
            if (element.Contains("Layer"))    { tmp.layer = getAttributeValue(element, "Layer"); }
            if (element.Contains("CodeSize")) { tmp.codeSize = getAttributeValueInt(element, "CodeSize"); }
            if (element.Contains("CodeArea")) { tmp.codeArea = getAttributeValueInt(element, "CodeArea"); }
            if (element.Contains("Geometry")) { tmp.geometry = getAttributeValue(element, "Geometry"); }
            if (element.Contains("PenColor")) { tmp.penColor = getAttributeValue(element, "PenColor"); }
            if (element.Contains("PenWidth")) { tmp.penWidth = getAttributeValueDouble(element, "PenWidth"); }
  //          if (element.Contains("ToolName")) { tmp.toolName = getAttributeValue(element, "ToolName"); }
//            Logger.Trace("   setBlockData finish");
            return tmp;
        }


        public static void FinishFigure(int lineEnd)
        {
            tmpFigure.lineEnd = lineEnd;
            listFigures.Add(tmpFigure);
            footer.lineStart = footer.lineEnd = Math.Max(footer.lineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetFigureCount()
        { return listFigures.Count; }

        public static bool GetFigureByFigureNr(int figNr)
        {
            foreach (BlockData tmp in listFigures)
            {   if (tmp.figureNr == figNr)
                {   tmpFigure = tmp;
                    return true;
                }
            }
            return false;
        }
        public static bool GetFigure(int lineNr, int search = 0)
        {
            if (listFigures.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    BlockData tmp = listFigures[0];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is first block
                        return false;

                    lastFigure.lineStart = listFigures[0].lineStart;
                    for (int i = 1; i < listFigures.Count; i++)
                    {
                        if ((lineNr >= listFigures[i].lineStart) && (lineNr <= listFigures[i].lineEnd))
                        {
                            lastFigure.lineEnd = listFigures[i - 1].lineEnd;
                            if (search == -1)
                                lastFigure.lineStart = listFigures[i - 1].lineStart;
                            return true;
                        }
                    }
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    BlockData tmp = listFigures[listFigures.Count - 1];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is last block
                        return false;

                    lastFigure.lineEnd = listFigures[listFigures.Count - 1].lineEnd;
                    for (int i = listFigures.Count - 1; i >= 0; i--)
                    {
                        if ((lineNr >= listFigures[i].lineStart) && (lineNr <= listFigures[i].lineEnd))
                        {
                            lastFigure.lineStart = listFigures[i + 1].lineStart;
                            if (search == 1)
                                lastFigure.lineEnd = listFigures[i + 1].lineEnd;
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (BlockData tmp in listFigures)
                    {
                        if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))
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
                start = lastGroup.lineStart;
            foreach (BlockData tmp in listFigures)
            {
                if (tmp.lineStart > start)
                { return tmp.lineStart; }
            }
            return -1;
        }
        public static int FindInsertPositionFigureTop(int current)
        {
            int start = 0;
            if (GetGroup(current))      // only check within same group
                start = lastGroup.lineStart;
            for (int i = 1; i < listFigures.Count; i++)
            {
                if (listFigures[i - 1].lineStart > start)
                {
                    if ((current >= listFigures[i].lineStart) && (current <= listFigures[i].lineEnd))
                    { return listFigures[i - 1].lineStart; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionFigureMostBottom(int current)
        {
            int end = listFigures[listFigures.Count - 1].lineEnd;
            if (GetGroup(current))      // only check within same group
                end = lastGroup.lineEnd;
            else
                return listFigures[listFigures.Count - 1].lineEnd + 1;
            int lastEnd = 0;
            foreach (BlockData tmp in listFigures)
            {
                if (tmp.lineStart >= end)      //
                { return lastEnd + 1; }
                lastEnd = tmp.lineEnd;
            }
            return -1;
        }
        public static int FindInsertPositionFigureBottom(int current)
        {
            int end = listFigures[listFigures.Count - 1].lineEnd;
            if (GetGroup(current))      // only check within same group
                end = lastGroup.lineEnd;
            for (int i = 0; i < listFigures.Count - 1; i++)
            {
                if (listFigures[i + 1].lineEnd <= end)
                {
                    if ((current >= listFigures[i].lineStart) && (current <= listFigures[i].lineEnd))
                    { return listFigures[i + 1].lineEnd + 1; }
                }
            }
            return -1;
        }
        public static bool isFoldingMarkerFigure(int line)
        {
            foreach (BlockData tmp in listFigures)
            {
                if ((line == tmp.lineStart) || (line == tmp.lineEnd))
                { return true; }
            }
            return false;
        }

        public static void AddGroup(int lineStart, string element, int figureNr)
        {   tmpGroup = setBlockData(lineStart, element, figureNr);   }

        public static void FinishGroup(int lineEnd)
        {
            tmpGroup.lineEnd = lineEnd;
            listGroups.Add(tmpGroup);
            footer.lineStart = footer.lineEnd = Math.Max(footer.lineStart, lineEnd);   // highest block-line = start of footer
        }
        public static int GetGroupCount()
        { return listGroups.Count; }

        public static bool GetGroup(int lineNr, int search = 0)
        {
            if (listGroups.Count > 0)
            {
                if (search <= -1)     // search start/end before actual block
                {
                    BlockData tmp = listGroups[0];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is first block
                        return false;

                    lastGroup.lineStart = listGroups[0].lineStart;
                    for (int i = 1; i < listGroups.Count; i++)
                    {
                        if ((lineNr >= listGroups[i].lineStart) && (lineNr <= listGroups[i].lineEnd))
                        {
                            lastGroup.lineEnd = listGroups[i - 1].lineEnd;
                            if (search == -1)
                                lastGroup.lineStart = listGroups[i - 1].lineStart;
                            return true;
                        }
                    }
                }
                else if (search >= 1)     // search start/end before actual block
                {
                    BlockData tmp = listGroups[listGroups.Count - 1];
                    if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))   // actual block is last block
                        return false;

                    lastGroup.lineEnd = listGroups[listGroups.Count - 1].lineEnd;
                    for (int i = listGroups.Count - 1; i >= 0; i--)
                    {
                        if ((lineNr >= listGroups[i].lineStart) && (lineNr <= listGroups[i].lineEnd))
                        {
                            lastGroup.lineStart = listGroups[i + 1].lineStart;
                            if (search == 1)
                                lastGroup.lineEnd = listGroups[i + 1].lineEnd;
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (BlockData tmp in listGroups)
                    {
                        if ((lineNr >= tmp.lineStart) && (lineNr <= tmp.lineEnd))
                        {
                            lastGroup = tmp;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int FindInsertPositionGroupMostTop(int current)
        {
            if (listGroups.Count > 1)
                return listGroups[0].lineStart;
            return -1;
        }
        public static int FindInsertPositionGroupTop(int current)
        {
            if (listGroups.Count > 1)
            {
                for (int i = 1; i < listGroups.Count; i++)
                {
                    if ((current >= listGroups[i].lineStart) && (current <= listGroups[i].lineEnd))
                    { return listGroups[i - 1].lineStart; }
                }
            }
            return -1;
        }
        public static int FindInsertPositionGroupMostBottom(int current)
        {
            if (listGroups.Count > 0)
                return listGroups[listGroups.Count - 1].lineEnd + 1;
            return -1;
        }
        public static int FindInsertPositionGroupBottom(int current)
        {
            if (listGroups.Count > 1)
            {
                for (int i = 1; i < listGroups.Count - 1; i++)
                {
                    if ((current >= listGroups[i].lineStart) && (current <= listGroups[i].lineEnd))
                    { return listGroups[i + 1].lineEnd + 1; }
                }
            }
            return -1;
        }
        public static bool isFoldingMarkerGroup(int line)
        {
            foreach (BlockData tmp in listGroups)
            {
                if ((line == tmp.lineStart) || (line == tmp.lineEnd))
                { return true; }
            }
            return false;
        }


    }
}
