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
 * 2019-10-03 new
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2019-11-10 add .Replace(',', '.')
*/

using System;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class ControlLaser : Form
    {
        public string gcode;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ControlLaser()
        {
            Logger.Trace("++++++ ControlLaser START ++++++");
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }

        private void runDelay()
        {   if (nUDMotionDelay.Value > 0)
            {   sendCommandEvent(new CmdEventArgs("(++++++++++ Delay)"));
                sendCommandEvent(new CmdEventArgs((string.Format("G1 M3 S{0} F100", nUDMotionDelayPower.Value).Replace(',', '.'))));
                sendCommandEvent(new CmdEventArgs((string.Format("G4 P{0}", nUDMotionDelay.Value).Replace(',', '.'))));
            }
        }

        private void finish(bool useZ=false)
        {
            sendCommandEvent(new CmdEventArgs("(++++++++++ Move back to start pos.)"));
            if (useZ)
                sendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0 Z0 M5")));
            else
                sendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0 M5")));

            if (nUDMotionY.Value != 0)
            {   sendCommandEvent(new CmdEventArgs((string.Format("G91 G0 Y{0}", nUDMotionY.Value).Replace(',', '.'))));
                sendCommandEvent(new CmdEventArgs("G90"));
            }
        }

        private void btnScanZ_Click(object sender, EventArgs e)
        {   string m = rBM3.Checked ? "3" : "4";
            sendCommandEvent(new CmdEventArgs("(++++++++++ Scan Z)"));
            sendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0 Z0")));
            runDelay();
            sendCommandEvent(new CmdEventArgs((string.Format("G0 Z{0} M{1} S0", nUDMotionZ.Value, m).Replace(',', '.'))));
            sendCommandEvent(new CmdEventArgs((string.Format("S{0}", nUDMotionPower.Value).Replace(',', '.'))));
            sendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} Z{1} F{2}", nUDMotionX.Value, -nUDMotionZ.Value, nUDMotionSpeed.Value).Replace(',', '.'))));
            finish(true);
        }

        private void btnScanSpeed_Click(object sender, EventArgs e)
        {
            string m = rBM3.Checked ? "3" : "4";
            decimal rangeSpeed = nUDSpeedMax.Value - nUDSpeedMin.Value;
            if (rangeSpeed == 0)
            { MessageBox.Show("There is no differnece between 'from' and 'to speed."); return; }

            decimal steps = Math.Abs(rangeSpeed / nUDSpeedStep.Value)+1;
            decimal xWidth = nUDMotionX.Value / steps;
            sendCommandEvent(new CmdEventArgs("(++++++++++ Scan Speed)"));
            sendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0")));
            runDelay();
            sendCommandEvent(new CmdEventArgs((string.Format("G91 M{0} S{1}", m, nUDSpeedPower.Value).Replace(',', '.'))));
            if (rangeSpeed > 0)     // bottom - up
            {
                for (decimal actSpeed = nUDSpeedMin.Value; actSpeed <= nUDSpeedMax.Value; actSpeed += nUDSpeedStep.Value)   // bottom - up
                    sendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} F{1}", GRBL_Plotter.gcode.frmtNum(xWidth), actSpeed).Replace(',', '.'))));
            }
            else
            {
                for (decimal actSpeed = nUDSpeedMin.Value; actSpeed >= nUDSpeedMax.Value; actSpeed -= nUDSpeedStep.Value)   // top - down
                    sendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} F{1}", GRBL_Plotter.gcode.frmtNum(xWidth), actSpeed).Replace(',', '.'))));
            }
            finish();
        }

        private void btnScanPower_Click(object sender, EventArgs e)
        {
            string m = rBM3.Checked ? "3" : "4";
            decimal rangePower = nUDPowerMax.Value - nUDPowerMin.Value;
            if (rangePower == 0)
            { MessageBox.Show("There is no differnece between 'from' and 'to power."); return; }

            decimal steps = Math.Abs(rangePower / nUDPowerStep.Value) +1;
            decimal xWidth = nUDMotionX.Value / steps;
            sendCommandEvent(new CmdEventArgs("(++++++++++ Scan Power)"));
            sendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0")));
            runDelay();
            sendCommandEvent(new CmdEventArgs(string.Format("G91 M{0} S0 F{1}", m, nUDPowerSpeed.Value)));
            if (rangePower > 0)     // bottom - up
            {
                for (decimal actPower = nUDPowerMin.Value; actPower <= nUDPowerMax.Value; actPower += nUDPowerStep.Value)   // bottom - up
                    sendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} S{1}", GRBL_Plotter.gcode.frmtNum(xWidth), actPower).Replace(',', '.'))));
            }
            else
            {
                for (decimal actPower = nUDPowerMin.Value; actPower >= nUDPowerMax.Value; actPower -= nUDPowerStep.Value)   // top - down
                    sendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} S{1}", GRBL_Plotter.gcode.frmtNum(xWidth), actPower).Replace(',', '.'))));
            }

            finish();
        }

        private void btnScanTool_Click(object sender, EventArgs e)
        {
            string m = rBM3.Checked ? "3" : "4";
            sendCommandEvent(new CmdEventArgs("(++++++++++ Try Tool )"));
            sendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0")));
            runDelay();
            sendCommandEvent(new CmdEventArgs((string.Format("G0 M{0} S0", m).Replace(',', '.'))));
            sendCommandEvent(new CmdEventArgs((string.Format("S{0}", tool_spindle).Replace(',', '.'))));
            sendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} F{1}", nUDMotionX.Value, tool_xyfeed).Replace(',', '.'))));
            finish();
        }



        private void cBLaserMode_CheckedChanged(object sender, EventArgs e)
        {
            if (grbl.getSetting(32) > 0)
            { gcode = "$32=0"; grbl.setSettings(32, "0"); }
            else
            { gcode = "$32=1"; grbl.setSettings(32, "1"); }
            sendCommandEvent(new CmdEventArgs(gcode));
            refreshValues();
        }

        private void refreshValues()
        {
            lblInfo.Text = string.Format("Max spindle speed: $30={0}; Min spindle speed: $31={1}; Laser Mode: $32={2}", grbl.getSetting(30), grbl.getSetting(31), grbl.getSetting(32));
            cBLaserMode.CheckedChanged -= cBLaserMode_CheckedChanged;
            cBLaserMode.Checked = (grbl.getSetting(32) > 0) ? true : false;
            cBLaserMode.CheckedChanged += cBLaserMode_CheckedChanged;
        }

        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void sendCommandEvent(CmdEventArgs e)
        {
            EventHandler<CmdEventArgs> handler = RaiseCmdEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ControlLaser_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ ControlLaser Stop ++++++");
        }

        private void ControlLaser_Load(object sender, EventArgs e)
        {
            btnToolUpdate_Click(sender, e);
            refreshValues();
        }

        float tool_xyfeed;
        float tool_spindle;
        private void cBTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tmp = cBTool.SelectedItem.ToString();
            if (tmp.IndexOf(")") > 0)
            {
                int tnr = int.Parse(tmp.Substring(0, tmp.IndexOf(")")));
                Properties.Settings.Default.importGCToolDefNr = tnr;
                tprop = toolTable.getToolProperties(tnr);
                tool_xyfeed = tprop.feedXY;
                tool_spindle = tprop.spindleSpeed;
                lblToolProp.Text = string.Format("XY-Feed F={0}, Laser pow. S={1}", tool_xyfeed, tool_spindle);
            }
        }

        toolProp tprop;
        private void btnToolUpdate_Click(object sender, EventArgs e)
        {
            int toolCount = toolTable.init();
            toolProp tmpTool;
            cBTool.Items.Clear();
            for (int i = 1; i < toolCount; i++)
            {
                tmpTool = toolTable.getToolProperties(i);
                cBTool.Items.Add(i.ToString() + ") " + tmpTool.name);
            }
            cBTool.SelectedIndex = 0;
            tprop = toolTable.getToolProperties(1);
            cBTool_SelectedIndexChanged(sender, e);
        }
    }
}

