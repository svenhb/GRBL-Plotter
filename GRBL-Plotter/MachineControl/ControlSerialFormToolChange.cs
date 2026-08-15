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
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlSerialForm : Form
    {
        private readonly Dictionary<string, string> gcodeVariableString = new Dictionary<string, string>();
        private void SetToolChangeCommand()
        {
            gcodeVariableString.Clear();
            gcodeVariableString.Add("TOGO", Properties.Settings.Default.ctrlToolCommandGripperOpen.Replace(",", ".")); // Gripper open
            gcodeVariableString.Add("TOGC", Properties.Settings.Default.ctrlToolCommandGripperClose.Replace(",", ".")); // Gripper close
        }

        private void SetToolChangeCoordinates(int cmdTNr, string line = "")
        {
            //    ToolProperty toolInfo = ToolList.GetToolProperties(cmdTNr);
            ToolPosition toolPos = ToolChanger.GetToolPosition(cmdTNr);
            gcodeVariable["TOAN"] = cmdTNr;
            if (toolPos.ToolNr != cmdTNr)
            {
                gcodeVariable["TOAX"] = gcodeVariable["TOAY"] = gcodeVariable["TOAZ"] = gcodeVariable["TOAA"] = 0;
                if (cBStatus1.Checked || cBStatus.Checked) AddToLog("\r[Tool change: " + cmdTNr.ToString() + " no Information found! (" + line + ")]");
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
            Logger.Info("InsertToolChangeCode line:{0} tool is in spindle:{1}", line, inSpindle);
            streamingBuffer.Add("($TS)", line);         // keyword for receiving-buffer (sendBuffer.GetConfirmedLine();) "Tool change start"
            if (inSpindle)
            {
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptPut, line);
                inSpindle = false;
                if (gcodeVariable.ContainsKey("TOLN"))
                { streamingBuffer.Add("($TO T" + gcodeVariable["TOLN"] + ")", line); }   // keyword for receiving-buffer "Tool removed"
                else
                {
                    AddToLog("InsertToolChangeCode var 'TOLN' not set!");
                    Logger.Error("InsertToolChangeCode var 'TOLN' not set!");
                }
            }
            /* Don't load new tool if keep empty */
            if (!Properties.Settings.Default.ctrlToolChangeEmpty || (gcodeVariable["TOAN"] != (int)Properties.Settings.Default.ctrlToolChangeEmptyNr))
            {
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptSelect, line);
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptGet, line);
                inSpindle = true;
                streamingBuffer.Add("($TI T" + gcodeVariable["TOAN"] + ")", line);  // keyword for receiving-buffer "Tool inserted"
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptProbe, line);
            }

            streamingBuffer.Add("($TE)", line);         // keyword for receiving-buffer "Tool change finished"

            if (Properties.Settings.Default.ctrlToolScriptDelay > 0)
                streamingBuffer.Add(string.Format("G04 P{0:0.00}", Properties.Settings.Default.ctrlToolScriptDelay), line);

            // save actual tool info as last tool info
            gcodeVariable["TOLN"] = gcodeVariable["TOAN"];	// TOol Last Number = TOol Actual Number
            gcodeVariable["TOLX"] = gcodeVariable["TOAX"];
            gcodeVariable["TOLY"] = gcodeVariable["TOAY"];
            gcodeVariable["TOLZ"] = gcodeVariable["TOAZ"];
            gcodeVariable["TOLA"] = gcodeVariable["TOAA"];
        }

    }
}
