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
 * 2026-08-05 new class ToolChanger
*/
/*
ToolChanger settings are tool positions, global tool position offset, tool exchange scripts
var psd = Properties.Settings.Default;

tool positions, saved to toolList.Position

global tool position offset, saved to
    psd.toolTableOffsetX = (decimal)positionOffset.X;
    psd.toolTableOffsetY = (decimal)positionOffset.Y;
    psd.toolTableOffsetZ = (decimal)positionOffset.Z;
    psd.toolTableOffsetA = (decimal)positionOffset.A;

tool exchange scripts, saved to
    psd.ctrlToolScriptPut
    psd.ctrlToolScriptSeöect
    psd.ctrlToolScriptGet
    psd.ctrlToolScriptProbe

 */

using GrblPlotter.Helper;
using GrblPlotter.UserControls;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace GrblPlotter
{
    internal class ToolPosition
    {	/* Grouping properties */
        public short ToolNr { get; set; }
        public XyzPoint Position { get; set; }
        public float Diameter { get; set; } // for display

        public ToolPosition()
        { ResetToolProperties(); }
        public ToolPosition(int tnr, decimal x, decimal y, decimal z, decimal a, float d)
        {
            ToolNr = (short)tnr;
            Position = new XyzPoint((double)x, (double)y, (double)z, (double)a);
            Diameter= d;
        }
        public void ResetToolProperties()
        {
            ToolNr = 1; 
            Position = new XyzPoint();
            Diameter = 10;
        }
        public ToolPosition Copy()
        {
            ToolPosition other = (ToolPosition)MemberwiseClone();
            return other;
        }

        public void WriteXML(ref XmlWriter w)
        {
            w.WriteStartElement("Tool");
            w.WriteAttributeString("Nr", ToolNr.ToString());
            w.WriteAttributeString("X", Position.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("Y", Position.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("Z", Position.Z.ToString().Replace(',', '.'));
            w.WriteAttributeString("A", Position.A.ToString().Replace(',', '.'));
            w.WriteAttributeString("D", Diameter.ToString("0.000").Replace(',', '.'));
            w.WriteEndElement();
        }
    }

    internal static class ToolChanger
    {
        internal static readonly string defaultFileExtension = "currentToolChanger (*.atc)|*.atc";
        internal static readonly string defaultFileName = "_currentToolChanger.atc";
        private static readonly bool log = true;
        internal static List<ToolPosition> toolPositionArray = new List<ToolPosition>();   // load color palette into this array
        internal static string Description = "";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Reset()
        {
            toolPositionArray.Clear();
        }
        public static void Add(ToolPosition tp)
        {
            toolPositionArray.Add(tp.Copy());
        }
        public static int GetToolListIndexByToolNr(int nr)
        {
            for (int i = 0; i < toolPositionArray.Count; i++)
            {
                if (toolPositionArray[i].ToolNr == nr)
                    return i;
            }
            return -1;
        }
      
        private static readonly XmlReaderSettings settings = new XmlReaderSettings()
        { DtdProcessing = DtdProcessing.Prohibit };
        public static int Init(string cmt = "")    // return number of entries
        {
            Logger.Info("🛠🛠🛠 Init ToolChanger {0}", cmt);
            return ReadXML();
        }
        public static int ReadXML(string FileName = "")
        {
            bool isCurrent = false;
            if (FileName == "") { FileName = Datapath.Tools + "\\"+ defaultFileName; isCurrent = true; }
            toolPositionArray.Clear();
            Logger.Info("ToolChanger ReadXML {0}", FileName);
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
                            case "ToolChanger":
                                if (log) Logger.Trace("ReadXML ToolChanger: {0}   {1}   {2}   {3}  ", reader["Off-X"], reader["Off-Y"], reader["Off-Z"], reader["Off-A"]);
                                psd.ctrlToolScriptDelay = XML.GetInt(reader, "ScriptDelay", 1);
                                positionOffset.X = XML.GetFloat(reader, "Off-X", 0.1f);
                                positionOffset.Y = XML.GetFloat(reader, "Off-Y", 0.2f);
                                positionOffset.Z = XML.GetFloat(reader, "Off-Z", 0.3f);
                                positionOffset.A = XML.GetFloat(reader, "Off-A", 0.4f);
                                psd.toolTableOffsetX = (decimal)positionOffset.X;
                                psd.toolTableOffsetY = (decimal)positionOffset.Y;
                                psd.toolTableOffsetZ = (decimal)positionOffset.Z;
                                psd.toolTableOffsetA = (decimal)positionOffset.A;
                                psd.Save();
                                break;
                            case "Description":
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    Description = reader.ReadElementContentAsString();
                                    Logger.Trace("ReadXML  Description  '{0}'", reader["Description"]);
                                }
                                break;

                            case "ToolChangeCmdGripperOpen":
                                if ((reader["ToolChangeCmdGripperOpen"] != null) && (reader["ToolChangeCmdGripperOpen"].Length > 0))
                                    psd.ctrlToolCommandGripperOpen = reader["ToolChangeCmdGripperOpen"];
                                break;
                            case "ToolChangeCmdGripperClose":
                                if ((reader["ToolChangeCmdGripperClose"] != null) && (reader["ToolChangeCmdGripperClose"].Length > 0))
                                    psd.ctrlToolCommandGripperClose = reader["ToolChangeCmdGripperClose"];
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
                                if (log) Logger.Trace("ReadXML Tool:{0,3}  X:{1,4}   Y:{2,4}   Z:{3,4}  A:{4,4}", reader["Nr"], reader["X"], reader["Y"], reader["Z"], reader["A"]);
                                toolPositionArray.Add(new ToolPosition());
                                toolCnt = toolPositionArray.Count - 1;
                                toolPositionArray[toolCnt].ToolNr = (short)XML.GetInt(reader, "Nr", 1);
                                XyzPoint pos = new XyzPoint()   ;
                                pos.X = XML.GetFloat(reader, "X", 0);
                                pos.Y = XML.GetFloat(reader, "Y", 0);
                                pos.Z = XML.GetFloat(reader, "Z", 0);
                                pos.A = XML.GetFloat(reader, "A", 0);
                                toolPositionArray[toolCnt].Position = pos;
                                toolPositionArray[toolCnt].Diameter = XML.GetFloat(reader, "D", 0);
                                break;
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                }
                catch (Exception err)
                { Logger.Error(err, "ReadXML nok"); }

                if (toolPositionArray.Count == 0)
                {
                    Logger.Trace("ReadXML nok - no tool added");
                    toolPositionArray.Add(new ToolPosition());
                }
            }
            else
            {
                Logger.Error("ReadXML file doesn't exist: {0} - create one default tool", FileName);
                toolPositionArray.Add(new ToolPosition());
            }
            Logger.Trace("ReadXML end");
            return toolCnt;
        }

        public static void WriteXML(string FileName = "")
        {
            if (FileName == "") { FileName = Datapath.Tools + "\\"+ defaultFileName; }
            if (File.Exists(FileName))
                File.Delete(FileName);
            var psd = Properties.Settings.Default;
            XyzPoint positionOffset = new XyzPoint((double)psd.toolTableOffsetX, (double)psd.toolTableOffsetY, (double)psd.toolTableOffsetZ, (double)psd.toolTableOffsetA);

            Logger.Info("ToolChanger WriteXML  {0}  Tools:{1}", FileName, toolPositionArray.Count);
            XmlWriterSettings set = new XmlWriterSettings
            { Indent = true };
            XmlWriter w = XmlWriter.Create(FileName, set);
            w.WriteStartDocument();
            w.WriteStartElement("ToolPositions");
            w.WriteAttributeString("Device", MyControl.GetSelectedDeviceName());
            w.WriteAttributeString("Amount", toolPositionArray.Count.ToString());

            w.WriteStartElement("ToolChange");
            w.WriteAttributeString("ScriptDelay", psd.ctrlToolScriptDelay.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-X", positionOffset.X.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-Y", positionOffset.Y.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-Z", positionOffset.Z.ToString().Replace(',', '.'));
            w.WriteAttributeString("Off-A", positionOffset.A.ToString().Replace(',', '.'));
            
            w.WriteStartElement("Description");
            w.WriteString(Description);
            w.WriteEndElement();

            w.WriteStartElement("ToolChangeCmdGripperOpen");
            w.WriteString(psd.ctrlToolCommandGripperOpen);
            w.WriteEndElement();
            w.WriteStartElement("ToolChangeCmdGripperClose");
            w.WriteString(psd.ctrlToolCommandGripperClose);
            w.WriteEndElement();

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

            if (toolPositionArray.Count > 0)
            {
                foreach (ToolPosition tP in toolPositionArray)
                {
                    tP.WriteXML(ref w);
                }
            }
            w.WriteEndElement();    // ToolList
            w.Flush();
            w.Close();
            w.Dispose();
        }

        public static ToolPosition GetToolPosition(int toolNr)
        {
            int index = toolNr - 1;
            if ((toolPositionArray == null) || (toolPositionArray.Count == 0))
            {
                Logger.Warn("GetToolPosition toolPositionArray is empty - do Init toolNr:{0}  count {1}", toolNr, toolPositionArray.Count);
                Init(" (GetToolPosition)");
            }
            if ((index < 0) || index >= toolPositionArray.Count)
            {
                Logger.Warn("GetToolPosition toolPositionArray toolNr nok toolNr:{0}  count {1}", toolNr, toolPositionArray.Count);
                return toolPositionArray[0];
            }
            foreach (ToolPosition tool in toolPositionArray)
            {
                if (toolNr == tool.ToolNr)
                { return tool; }
            }

            return toolPositionArray[toolNr - 1];
        }
    }
}
