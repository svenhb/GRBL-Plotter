/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

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
 * 2021-04-09 split finish commands
 * 2021-07-02 code clean up / code quality
 * 2021-11-23 set default for tprop
 * 2021-12-09 line 222 check if (cBTool.Count == 0)
 * 2021-12-22 check if is connected to grbl before sending code
*/

using System;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlLaser : Form
    {
        internal string gcode;
        private ToolProp tprop = new ToolProp();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ControlLaser()
        {
            Logger.Trace("++++++ ControlLaser START ++++++");
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }

        private void RunDelay()
        {
            if (nUDMotionDelay.Value > 0)
            {
                SendCommandEvent(new CmdEventArgs("(++++++++++ Delay)"));
                SendCommandEvent(new CmdEventArgs((string.Format("G1 M3 S{0} F100", nUDMotionDelayPower.Value).Replace(',', '.'))));
                SendCommandEvent(new CmdEventArgs((string.Format("G4 P{0}", nUDMotionDelay.Value).Replace(',', '.'))));
            }
        }

        private void Finish(bool useZ = false)
        {
            SendCommandEvent(new CmdEventArgs("(++++++++++ Move back to start pos.)"));
            if (useZ)
                SendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0 Z0 M5")));
            else
            {
                SendCommandEvent(new CmdEventArgs(string.Format("S0 G4 P0.5")));
                SendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0")));
                SendCommandEvent(new CmdEventArgs(string.Format("M5")));
            }

            if (nUDMotionY.Value != 0)
            {
                SendCommandEvent(new CmdEventArgs((string.Format("G91 G0 Y{0}", nUDMotionY.Value).Replace(',', '.'))));
                SendCommandEvent(new CmdEventArgs("G90"));
            }
        }

        private void BtnScanZ_Click(object sender, EventArgs e)
        {
			if (!Grbl.isConnected)
			{	MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
            string m = rBM3.Checked ? "3" : "4";
            SendCommandEvent(new CmdEventArgs("(++++++++++ Scan Z)"));
            SendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0 Z0")));
            RunDelay();
            SendCommandEvent(new CmdEventArgs((string.Format("G0 Z{0} M{1} S0", nUDMotionZ.Value, m).Replace(',', '.'))));
            SendCommandEvent(new CmdEventArgs((string.Format("S{0}", nUDMotionPower.Value).Replace(',', '.'))));
            SendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} Z{1} F{2}", nUDMotionX.Value, -nUDMotionZ.Value, nUDMotionSpeed.Value).Replace(',', '.'))));
            Finish(true);
        }

        private void BtnScanSpeed_Click(object sender, EventArgs e)
        {
			if (!Grbl.isConnected)
			{	MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
            string m = rBM3.Checked ? "3" : "4";
            decimal rangeSpeed = nUDSpeedMax.Value - nUDSpeedMin.Value;
            if (rangeSpeed == 0)
            { MessageBox.Show("There is no differnece between 'from' and 'to speed."); return; }

            decimal steps = Math.Abs(rangeSpeed / nUDSpeedStep.Value) + 1;
            decimal xWidth = nUDMotionX.Value / steps;
            SendCommandEvent(new CmdEventArgs("(++++++++++ Scan Speed)"));
            SendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0")));
            RunDelay();
            SendCommandEvent(new CmdEventArgs((string.Format("G91 M{0} S{1}", m, nUDSpeedPower.Value).Replace(',', '.'))));
            if (rangeSpeed > 0)     // bottom - up
            {
                for (decimal actSpeed = nUDSpeedMin.Value; actSpeed <= nUDSpeedMax.Value; actSpeed += nUDSpeedStep.Value)   // bottom - up
                    SendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} F{1}", GrblPlotter.Gcode.FrmtNum(xWidth), actSpeed).Replace(',', '.'))));
            }
            else
            {
                for (decimal actSpeed = nUDSpeedMin.Value; actSpeed >= nUDSpeedMax.Value; actSpeed -= nUDSpeedStep.Value)   // top - down
                    SendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} F{1}", GrblPlotter.Gcode.FrmtNum(xWidth), actSpeed).Replace(',', '.'))));
            }
            Finish();
        }

        private void BtnScanPower_Click(object sender, EventArgs e)
        {
			if (!Grbl.isConnected)
			{	MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
            string m = rBM3.Checked ? "3" : "4";
            decimal rangePower = nUDPowerMax.Value - nUDPowerMin.Value;
            if (rangePower == 0)
            { MessageBox.Show("There is no differnece between 'from' and 'to power."); return; }

            decimal steps = Math.Abs(rangePower / nUDPowerStep.Value) + 1;
            decimal xWidth = nUDMotionX.Value / steps;
            SendCommandEvent(new CmdEventArgs("(++++++++++ Scan Power)"));
            SendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0")));
            RunDelay();
            SendCommandEvent(new CmdEventArgs(string.Format("G91 M{0} S0 F{1}", m, nUDPowerSpeed.Value)));
            if (rangePower > 0)     // bottom - up
            {
                for (decimal actPower = nUDPowerMin.Value; actPower <= nUDPowerMax.Value; actPower += nUDPowerStep.Value)   // bottom - up
                    SendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} S{1}", GrblPlotter.Gcode.FrmtNum(xWidth), actPower).Replace(',', '.'))));
            }
            else
            {
                for (decimal actPower = nUDPowerMin.Value; actPower >= nUDPowerMax.Value; actPower -= nUDPowerStep.Value)   // top - down
                    SendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} S{1}", GrblPlotter.Gcode.FrmtNum(xWidth), actPower).Replace(',', '.'))));
            }

            Finish();
        }

        private void BtnScanTool_Click(object sender, EventArgs e)
        {
			if (!Grbl.isConnected)
			{	MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
            string m = rBM3.Checked ? "3" : "4";
            SendCommandEvent(new CmdEventArgs("(++++++++++ Try Tool )"));
            SendCommandEvent(new CmdEventArgs(string.Format("G90 G0 X0")));
            RunDelay();
            SendCommandEvent(new CmdEventArgs((string.Format("G0 M{0} S0", m).Replace(',', '.'))));
            SendCommandEvent(new CmdEventArgs((string.Format("S{0}", tool_spindle).Replace(',', '.'))));
            SendCommandEvent(new CmdEventArgs((string.Format("G1 X{0} F{1}", nUDMotionX.Value, tool_xyfeed).Replace(',', '.'))));
            Finish();
        }



        private void CbLaserMode_CheckedChanged(object sender, EventArgs e)
        {
            if (Grbl.GetSetting(32) > 0)
            { gcode = "$32=0"; Grbl.SetSettings(32, "0"); }
            else
            { gcode = "$32=1"; Grbl.SetSettings(32, "1"); }
            SendCommandEvent(new CmdEventArgs(gcode));
            RefreshValues();
        }

        private void RefreshValues()
        {
            lblInfo.Text = string.Format("Max spindle speed: $30={0}; Min spindle speed: $31={1}; Laser Mode: $32={2}", Grbl.GetSetting(30), Grbl.GetSetting(31), Grbl.GetSetting(32));
            cBLaserMode.CheckedChanged -= CbLaserMode_CheckedChanged;
            cBLaserMode.Checked = (Grbl.GetSetting(32) > 0) ? true : false;
            cBLaserMode.CheckedChanged += CbLaserMode_CheckedChanged;
        }

        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void SendCommandEvent(CmdEventArgs e)
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
            BtnToolUpdate_Click(sender, e);
            RefreshValues();
        }

        float tool_xyfeed;
        float tool_spindle;
        private void CbTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tmp = cBTool.SelectedItem.ToString();
            if (tmp.IndexOf(")") > 0)
            {
                int tnr = int.Parse(tmp.Substring(0, tmp.IndexOf(")")));
                Properties.Settings.Default.importGCToolDefNr = tnr;
                tprop = ToolTable.GetToolProperties(tnr);
                tool_xyfeed = tprop.FeedXY;
                tool_spindle = tprop.SpindleSpeed;
                lblToolProp.Text = string.Format("XY-Feed F={0}, Laser pow. S={1}", tool_xyfeed, tool_spindle);
            }
        }

        private void BtnToolUpdate_Click(object sender, EventArgs e)
        {
            int toolCount = ToolTable.Init();
            ToolProp tmpTool;
            cBTool.Items.Clear();
            for (int i = 1; i < toolCount; i++)
            {
                tmpTool = ToolTable.GetToolProperties(i);
                cBTool.Items.Add(i.ToString() + ") " + tmpTool.Name);
            }
			if (cBTool.Items.Count == 0)
			{
				cBTool.Items.Add("No tool table entries found!!!");
			}
			else
			{
				cBTool.SelectedIndex = 0;
				tprop = ToolTable.GetToolProperties(1);
			}
            CbTool_SelectedIndexChanged(sender, e);
        }
    }
}

