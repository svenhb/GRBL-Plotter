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
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.Helper;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCDeviceLaser2 : UserControl
    {

        private readonly float DpiScaling;
        private readonly bool isLarge = true;
        private int neededWidth = 230;
        private string toolTipLaserOn = "";

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public UCDeviceLaser2()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
        }
        public void SetStatusSpindle(bool on)
        {
            CbLaser.CheckedChanged -= CbLaser_CheckedChanged;
            PbLaser.Visible = CbLaser.Checked = on;
            CbLaser.CheckedChanged += CbLaser_CheckedChanged;
        }
        public void SetStatusMWord(string m)
        {
            CbPilotLaser.CheckedChanged-= CbPilotLaser_CheckedChanged;
            CbAirAssist.CheckedChanged -= CbAirAssist_CheckedChanged;
            /*
            if (Properties.Settings.Default.DeviceLaserCmndPilotOn.Contains(m))
                CbPilotLaser.Checked = true;
            if (Properties.Settings.Default.DeviceLaserCmndPilotOff.Contains(m))
                CbPilotLaser.Checked = false;
            if (Properties.Settings.Default.DeviceLaserCmndAirOn.Contains(m))
                CbAirAssist.Checked = true;
            if (Properties.Settings.Default.DeviceLaserCmndAirOff.Contains(m))
                CbAirAssist.Checked = false;
            */
            CbPilotLaser.CheckedChanged += CbPilotLaser_CheckedChanged;
            CbAirAssist.CheckedChanged += CbAirAssist_CheckedChanged;
        }

        internal int GetControlWidth()
        {
            return neededWidth;
        }

        private void UCDeviceLaser2_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            Width = neededWidth = (int)(DpiScaling * Width);
            string power = string.Format("{0:0.0}% ", Properties.Settings.Default.DeviceLaserPowerLow);
            this.toolTip1.SetToolTip(this.CbLaser, string.Format("Switch on laser with low energy: {0}", power));
            updateTimer.Start();
        }
        protected virtual void UpdateTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(Convert.ToDouble(Properties.Settings.Default.grblRunTimeSpindle));
            LblUptime.Text = string.Format("{0:D4}:{1:D2}:{2:D2}",
                            (int)(t.TotalHours),
                            t.Minutes,
                            t.Seconds);
        }
        private void BtnSetup_Click(object sender, EventArgs e)
        {
            decimal max = (decimal)Grbl.GetSetting(30);
            if (max < 1)
                max = 10000;

            List<ControlDefaults> cd = new List<ControlDefaults>();
            cd.Add(new ControlDefaults("Enable Laser mode", "BtnSend", "$32=1"));
            cd.Add(new ControlDefaults("Disable Laser mode", "BtnSend", "$32=0"));
            cd.Add(new ControlDefaults("Laser power in % for 'Laser on' button", "DeviceLaserPowerLow", new decimal[] { 0.1m, max, 1m, 1 }));
            cd.Add(new ControlDefaults("Command Pilot on", "DeviceLaserCmndPilotOn"));
            cd.Add(new ControlDefaults("Command Pilot off", "DeviceLaserCmndPilotOff"));
            cd.Add(new ControlDefaults("Command Air on", "DeviceLaserCmndAirOn"));
            cd.Add(new ControlDefaults("Command Air off", "DeviceLaserCmndAirOff"));
            MyControl.ShowSimpleSetup("Setup Laser", "", Cursor.Position, cd);

            int speed = (int)Math.Round(Properties.Settings.Default.DeviceLaserPowerLow * max / 100);
            string power = string.Format("{0:0.0}% = S{1}", Properties.Settings.Default.DeviceLaserPowerLow, speed);
            if (toolTipLaserOn != "")
            { toolTipLaserOn = this.toolTip1.GetToolTip(this.CbLaser); }
            this.toolTip1.SetToolTip(this.CbLaser, string.Format("{0}\r\n{1}", toolTipLaserOn, power));
        }

        private void UCDeviceLaser2_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
        }

        private void CbLaser_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Trace("CbLaser_CheckedChanged {0}", CbLaser.Checked);
            string m = "M3";

            float speed = Grbl.GetSetting(30);
            if (speed > 1)
                speed = (int)Math.Round((float)Properties.Settings.Default.DeviceLaserPowerLow * speed / 100);
            else
                speed = 0;

            if (CbLaser.Checked)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G1F100{0}S{1:0}", m, speed).Replace(",", "."), 0, sender, e));
                PbLaser.Visible = true;
            }
            else
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs("M5", 0, sender, e));
                PbLaser.Visible = false;
            }
        }

        private void CbPilotLaser_CheckedChanged(object sender, EventArgs e)
        {
            if (CbPilotLaser.Checked)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(Properties.Settings.Default.DeviceLaserCmndPilotOn, 0, sender, e));
            else
                OnRaiseCmdEvent(new UserControlCmdEventArgs(Properties.Settings.Default.DeviceLaserCmndPilotOff, 0, sender, e));
        }

        private void CbAirAssist_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Trace("CbAirAssist_CheckedChanged {0}", CbAirAssist.Checked);
            if (CbAirAssist.Checked)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(Properties.Settings.Default.DeviceLaserCmndAirOn, 0, sender, e));
            else
                OnRaiseCmdEvent(new UserControlCmdEventArgs(Properties.Settings.Default.DeviceLaserCmndAirOff, 0, sender, e));
        }

        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
        }
    }
}
