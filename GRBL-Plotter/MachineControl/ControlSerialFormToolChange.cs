/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
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
    How does it work:
    During code streaming (in ControlSerialFormStreaming - StartStreaming), 
    if "Tx" command appears (line 306), SetToolChangeCoordinates will be called to get actual tool coordinates loaded into gcodeVariable["TOAX"] and further.
    if "M6" comand appears, InsertToolChangeCode will be called to run the neede scripts from Properties.Settings.Default.ctrlToolScriptPut and so on.
    Loaded script contains variables like '#TOAX', which will be replaced by gcodeVariable["TOAX"] - value (ControlSerialFormInterface - InsertVariable).
    This code will placed in the streamingBuffer including keywords like #TS #TO #TI #TE
    The keywords will be interpreted in ControlSerialFormInterface - ProcessGrblOkMessage line 416 to add log information

*/
/* 2026-07-28 split file
 * 2026-08-20 add new variables TOV1 - TOV8
 * 2026-09-08 track also subroutine call "$SR"
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlSerialForm : Form
    {
        private readonly Dictionary<string, string> gcodeVariableString = new Dictionary<string, string>();
        private void SetToolChangeVariables(bool listContent = false)
        {
            if (listContent)
                Logger.Trace("List of variables:");
            var psd = Properties.Settings.Default;
            gcodeVariableString.Clear();
            addToGcodeVariableString("TOGO", psd.ctrlToolCommandGripperOpen); // Gripper open
            addToGcodeVariableString("TOGC", psd.ctrlToolCommandGripperClose); // Gripper close
            addToGcodeVariableString("TOV1", psd.ctrlToolCommandVar1);
            addToGcodeVariableString("TOV2", psd.ctrlToolCommandVar2);
            addToGcodeVariableString("TOV3", psd.ctrlToolCommandVar3);
            addToGcodeVariableString("TOV4", psd.ctrlToolCommandVar4);
            addToGcodeVariableString("TOV5", psd.ctrlToolCommandVar5);
            addToGcodeVariableString("TOV6", psd.ctrlToolCommandVar6);
            addToGcodeVariableString("TOV7", psd.ctrlToolCommandVar7);
            addToGcodeVariableString("TOV8", psd.ctrlToolCommandVar8);

            void addToGcodeVariableString(string key, string content)
            {
                string strippedContent = StripCmt(content);
                gcodeVariableString.Add(key, strippedContent); // Gripper open

                if (listContent)
                {
                    if (gcodeVariableString.ContainsKey(key))
                        Logger.Trace("{0} = {1,-25}  original:'{2}'", "#" + key, "'" + gcodeVariableString[key] + "'", content);
                    else
                        Logger.Error("{0} not found", key);
                }
            }
        }
        private string StripCmt(string tmp, bool listContent = false)
        {
            //    return tmp;
            if (tmp.Contains("#") || tmp.Contains("$") || tmp.Contains("%"))
                return tmp;

            int pos = tmp.IndexOf("(");
            if (pos == 0)
            {
                if (tmp.Length > 15)
                    return tmp.Substring(0, 14) + ")";
                return tmp;
            }
            else if (pos > 0)
            {
                return String.Concat(tmp.Substring(0, pos).Replace(",", ".").Where(c => !Char.IsWhiteSpace(c)));
                //    return tmp.Substring(0, pos).Replace(",", ".").Trim();
            }
            return String.Concat(tmp.Replace(",", ".").Where(c => !Char.IsWhiteSpace(c)));
            //    return tmp.Replace(",", ".");
        }
        private void SetToolChangeCoordinates(int cmdTNr, string line = "")
        {
            ToolPosition toolPos = ToolChanger.GetToolPosition(cmdTNr);			// if cmdTNr=0, result = toolPositionArray[0] means from toolNr 1
            gcodeVariable["TOAN"] = cmdTNr;
            if (toolPos.ToolNr != cmdTNr)										// happens if cmdTNr=0
            {
                if (cmdTNr == 0)
                    AddToLog("\r[Streaming setup: Tool change: T0, remove last pen ]");
                else
                    AddToLog("\r!!! Streaming setup: Tool change: T" + cmdTNr.ToString() + ", no Information found!!! (" + line + ")");
            }
            else
            {   // get new values
                double tx, ty, tz, ta;
                gcodeVariable["TOAX"] = tx = toolPos.Position.X + (double)Properties.Settings.Default.toolTableOffsetX;
                gcodeVariable["TOAY"] = ty = toolPos.Position.Y + (double)Properties.Settings.Default.toolTableOffsetY;
                gcodeVariable["TOAZ"] = tz = toolPos.Position.Z + (double)Properties.Settings.Default.toolTableOffsetZ;
                gcodeVariable["TOAA"] = ta = toolPos.Position.A + (double)Properties.Settings.Default.toolTableOffsetA;
                string coord = string.Format("X:{0:0.0} Y:{1:0.0} Z:{2:0.0} A:{3:0.0}", tx, ty, tz, ta);
                if (cBStatus1.Checked || cBStatus.Checked) AddToLog("\r[set tool coordinates " + cmdTNr.ToString() + " " + coord + "]");
            }
        }

        private void InsertToolChangeCode(int line, ref bool inSpindle)
        {
            Logger.Info("InsertToolChangeCode line:{0} tool is in spindle:{1}  T-Actual:{2}  T-Last:{3}", line, inSpindle, gcodeVariable["TOAN"], gcodeVariable["TOLN"]);
            streamingBuffer.Add("($TS)", line);         // keyword for receiving-buffer (sendBuffer.GetConfirmedLine();) "Tool change start"

            /* remove pen, if in holder */
            if (inSpindle)
            {
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptPut, line);	// remove
                inSpindle = false;
                if (gcodeVariable.ContainsKey("TOLN"))
                { streamingBuffer.Add("($TO T" + gcodeVariable["TOLN"] + ")", line); }   // keyword for receiving-buffer "Tool removed"
                else
                {
                    AddToLog("!!! InsertToolChangeCode var 'TOLN' not set!");
                    Logger.Error("InsertToolChangeCode var 'TOLN' not set!");
                }
            }

            /* Don't load new tool if keep empty and TOAN==ctrlToolChangeEmptyNr */
            if (!Properties.Settings.Default.ctrlToolChangeEmpty || (gcodeVariable["TOAN"] != (int)Properties.Settings.Default.ctrlToolChangeEmptyNr))
            {
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptSelect, line);
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptGet, line);
                inSpindle = true;
                streamingBuffer.Add("($TI T" + gcodeVariable["TOAN"] + ")", line);  // keyword for receiving-buffer "Tool inserted"
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptProbe, line);
            }
            else
            {
                if (cBStatus1.Checked || cBStatus.Checked) AddToLog("[Skip scripts 'select' and 'get' for T0]");
            }

            streamingBuffer.Add("($TE)", line);         // keyword for receiving-buffer "Tool change finished"

            if (Properties.Settings.Default.ctrlToolScriptDelay > 0)
                streamingBuffer.Add(string.Format("G04 P{0:0.00}", Properties.Settings.Default.ctrlToolScriptDelay).Replace(',', '.'), line);

            // save actual tool info as last tool info
            gcodeVariable["TOLN"] = gcodeVariable["TOAN"];	// TOol Last Number = TOol Actual Number
            gcodeVariable["TOLX"] = gcodeVariable["TOAX"];
            gcodeVariable["TOLY"] = gcodeVariable["TOAY"];
            gcodeVariable["TOLZ"] = gcodeVariable["TOAZ"];
            gcodeVariable["TOLA"] = gcodeVariable["TOAA"];
        }

        private void ProcessToolChangeTracker(string rxLine)
        {
            if (rxLine.Contains("($TS")) { AddToLog("[Tool change start]"); }
            else if (rxLine.Contains("($TO")) { AddToLog("[Tool " + rxLine.Substring(4).Trim(')') + " removed]"); ToolInSpindle = false; }
            else if (rxLine.Contains("($TI")) { AddToLog("[Tool " + rxLine.Substring(4).Trim(')') + " selected]"); ToolInSpindle = true; }
            else if (rxLine.Contains("($TE")) { AddToLog("[Tool change finished]"); }
            else if (rxLine.Contains("($SR")) { AddToLog("[Subroutine " + rxLine.Substring(4).Trim(')') + "]"); }
            else if (rxLine.Contains("($END"))
            {
                AddToLog("[Program END: " + GetTimeStampString(2) + " Duration: " + GetTimeStampString(3) + "]");
                Logger.Info("ProcessGrblOkMessage [Program end]");
                StreamingFinish();      // reset streaming, clear sendBuffer, streamingBuffer, ListAccessoryStateRunTime
            }
        }
    }
}
