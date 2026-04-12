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
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCOverrides : UserControl
    {
        private readonly float DpiScaling;
        private bool isLarge = true;
        private readonly int controlHeightFold = MyControl.MinimumHeightFolded;
        private int controlHeightUnfold = 245;
        MyControl m = new MyControl();

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public UCOverrides()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
            MyControl.DpiScaling = DpiScaling;

            GbOverrides.Click += GbOverrides_Click;
        }
        private void UserControlOverrides_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            SetHeight(Properties.Settings.Default.UserControlOverrideIsLarge);
     //       UpdateHeight();
        }
        private void ResetLabels()
        {
            LblFeedSet.Text = "100%";
            LblRapidSet.Text = "100%";
            LblSpindleSet.Text = "100%";
        }

        internal void SetHeight(bool setLarge)
        {
            UpdateHeight();
            if (setLarge)
            {
                GbOverrides.BackColor = MyControl.PanelBackColor;	// Color.WhiteSmoke;
				GbOverrides.ForeColor = MyControl.PanelForeColor;
                SetControlHeight(controlHeightUnfold); BtnSetup.Visible = true;
            }
            else
            {
                GbOverrides.BackColor = MyControl.NotifyYellow;
 				GbOverrides.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                SetControlHeight(controlHeightFold); BtnSetup.Visible = false;
            }
            isLarge = setLarge;
            Invalidate();
        }
    
        internal void SetLabelFeedSet(string txt)
        { m.SetLabelSave(LblFeedSet, txt + "%"); }
        internal void SetLabelFeedValue(string txt)
        { m.SetLabelSave(LblFeedValue, txt); }
        internal void SetLabelRapidSet(string txt)
        { m.SetLabelSave(LblRapidSet, txt + "%"); }
        internal void SetLabelRapidValue(string txt)
        { m.SetLabelSave(LblRapidValue, txt); }
        internal void SetLabelSpindleSet(string txt)
        { m.SetLabelSave(LblSpindleSet, txt + "%"); }
        internal void SetLabelSpindleValue(string txt)
        { m.SetLabelSave(LblSpindleValue, txt); }

        internal void SetStateDx(string digits)
        {
            int din = 0;
            int dout = 0;
            if (digits.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                { dout |= ((digits[i] == '1') ? 1 : 0) << (3 - i); }
                SetAccessoryButton(BtnOverrideD3, (digits[0] == '1'));
                SetAccessoryButton(BtnOverrideD2, (digits[1] == '1'));
                SetAccessoryButton(BtnOverrideD1, (digits[2] == '1'));
                SetAccessoryButton(BtnOverrideD0, (digits[3] == '1'));
                BtnOverrideD3.BackColor = default;
                BtnOverrideD2.BackColor = default;
                BtnOverrideD1.BackColor = default;
                BtnOverrideD0.BackColor = default;
            }
            else if (digits.Length == 8)
            {
                for (int i = 0; i < 4; i++)
                { din |= ((digits[i] == '1') ? 1 : 0) << i; }
                BtnOverrideD3.BackColor = (digits[0] == '1') ? Color.Honeydew : Color.LightPink;
                BtnOverrideD2.BackColor = (digits[1] == '1') ? Color.Honeydew : Color.LightPink;
                BtnOverrideD1.BackColor = (digits[2] == '1') ? Color.Honeydew : Color.LightPink;
                BtnOverrideD0.BackColor = (digits[3] == '1') ? Color.Honeydew : Color.LightPink;
                for (int i = 4; i < 8; i++)
                { dout |= ((digits[i] == '1') ? 1 : 0) << (7 - i - 4); }
                SetAccessoryButton(BtnOverrideD3, (digits[4] == '1'));
                SetAccessoryButton(BtnOverrideD2, (digits[5] == '1'));
                SetAccessoryButton(BtnOverrideD1, (digits[6] == '1'));
                SetAccessoryButton(BtnOverrideD0, (digits[7] == '1'));
            }
            Grbl.grblDigitalIn = (byte)din;
            Grbl.grblDigitalOut = (byte)dout;
        }

        internal void SetStateSpindle(bool on)
        { SetAccessoryButton(BtnToggleSpindle, on); }
        internal void SetStateFlood(bool on)
        {
            SetAccessoryButton(BtnToggleFlood, on);
            BtnToggleFlood.BackColor = on ? Color.Lime : Color.Transparent;
        }
        internal void SetStateMist(bool on)
        {
            SetAccessoryButton(BtnToggleMist, on);
            BtnToggleMist.BackColor = on ? Color.Lime : Color.Transparent;
        }
        private void SetAccessoryButton(Button Btn, bool setOn)
        {
            if (setOn)
            { Btn.Image = Properties.Resources.led_on; Btn.Tag = "on"; }
            else
            { Btn.Image = Properties.Resources.led_off; ; Btn.Tag = "off"; }
        }

        internal void TriggerCmd(string action)
        {
            if (action.Contains("FeedDec10")) { BtnFeed1.PerformClick(); }
            else if (action.Contains("FeedDec1")) { BtnFeed2.PerformClick(); }
            else if (action.Contains("FeedSet100")) { BtnFeed3.PerformClick(); }
            else if (action.Contains("FeedInc1")) { BtnFeed4.PerformClick(); }
            else if (action.Contains("FeedInc10")) { BtnFeed5.PerformClick(); }
            else if (action.Contains("SpindleDec10")) { BtnSpindle1.PerformClick(); }
            else if (action.Contains("SpindleDec1")) { BtnSpindle2.PerformClick(); }
            else if (action.Contains("SpindleSet100")) { BtnSpindle3.PerformClick(); }
            else if (action.Contains("SpindleInc1")) { BtnSpindle4.PerformClick(); }
            else if (action.Contains("SpindleInc10")) { BtnSpindle5.PerformClick(); }
        }
      

        internal void UpdateHeight(bool growOnChange = false)
        {
            panel1.Visible = Properties.Settings.Default.UserControlOverrideShow1;
            panel2.Visible = Properties.Settings.Default.UserControlOverrideShow2;
            panel3.Visible = Properties.Settings.Default.UserControlOverrideShow3;
            panel4.Visible = Properties.Settings.Default.UserControlOverrideShow4;
            //     if (Properties.ListSettings.Default.grblDescriptionDxEnable)
            controlHeightUnfold = MyControl.MinimumHeightFolded;
            controlHeightUnfold += panel1.Visible ? 50 : 0;
            controlHeightUnfold += panel2.Visible ? 50 : 0;
            controlHeightUnfold += panel3.Visible ? 50 : 0;
            controlHeightUnfold += panel4.Visible ? 75 : 0;
        }
        private void GbOverrides_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbOverrides.PointToClient(screenPosition);
            if (clientPosition.Y < MyControl.MinimumHeightFolded)
            {
                UpdateHeight();
                isLarge = !isLarge;
                SetHeight(isLarge);

                Properties.Settings.Default.UserControlOverrideIsLarge = isLarge;
                Properties.Settings.Default.Save();
                Invalidate();
            }
        }
        private void SetControlHeight(int h)
        { Height = (int)(DpiScaling * h); }

        private void BtnFeed1_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x92, sender, e)); }  // -10%
        private void BtnFeed2_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x94, sender, e)); }  // -1%
        private void BtnFeed3_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x90, sender, e)); }  // 100%
        private void BtnFeed4_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x93, sender, e)); }  // +1%
        private void BtnFeed5_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x91, sender, e)); }  // +10%

        private void BtnRapid1_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x95, sender, e)); }  // 100%
        private void BtnRapid2_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x96, sender, e)); }  // 50%
        private void BtnRapid3_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x97, sender, e)); }  // 25%

        private void BtnSpindle1_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x9B, sender, e)); }  // -10%
        private void BtnSpindle2_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x9D, sender, e)); }  // -1%
        private void BtnSpindle3_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x99, sender, e)); }  // 100%
        private void BtnSpindle4_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x9C, sender, e)); }  // +1%
        private void BtnSpindle5_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x9A, sender, e)); }  // +10%

        private void BtnToggleSpindle_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x9E, sender, e)); }  // Spindle
        private void BtnToggleFlood_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0xA0, sender, e)); }  // Flood
        private void BtnToggleMist_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0xA1, sender, e)); }  // Mist

        private void BtnOverrideD0_Click(object sender, EventArgs e)
        { if (BtnOverrideD0.Tag.ToString() == "off") OnRaiseCmdEvent(new UserControlCmdEventArgs("M64 P0", 0, sender, e)); else OnRaiseCmdEvent(new UserControlCmdEventArgs("M65 P0", 0, sender, e)); }
        private void BtnOverrideD1_Click(object sender, EventArgs e)
        { if (BtnOverrideD1.Tag.ToString() == "off") OnRaiseCmdEvent(new UserControlCmdEventArgs("M64 P1", 0, sender, e)); else OnRaiseCmdEvent(new UserControlCmdEventArgs("M65 P1", 0, sender, e)); }
        private void BtnOverrideD2_Click(object sender, EventArgs e)
        { if (BtnOverrideD2.Tag.ToString() == "off") OnRaiseCmdEvent(new UserControlCmdEventArgs("M64 P2", 0, sender, e)); else OnRaiseCmdEvent(new UserControlCmdEventArgs("M65 P2", 0, sender, e)); }
        private void BtnOverrideD3_Click(object sender, EventArgs e)
        { if (BtnOverrideD3.Tag.ToString() == "off") OnRaiseCmdEvent(new UserControlCmdEventArgs("M64 P3", 0, sender, e)); else OnRaiseCmdEvent(new UserControlCmdEventArgs("M65 P3", 0, sender, e)); }

        internal void EnableButtons(bool enable)
        {
            panel1.Enabled = enable;
            panel2.Enabled = enable;
            panel3.Enabled = enable;
            panel4.Enabled = enable;
        }
        private void BtnSetup_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>();
            cd.Add(new ControlDefaults(LblSetupOption1.Text, "UserControlOverrideShow1"));
            cd.Add(new ControlDefaults(LblSetupOption2.Text, "UserControlOverrideShow2"));
            cd.Add(new ControlDefaults(LblSetupOption3.Text, "UserControlOverrideShow3"));
            cd.Add(new ControlDefaults(LblSetupOption4.Text, "UserControlOverrideShow4"));

            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd);
            UpdateHeight();
            SetControlHeight(controlHeightUnfold);
            Invalidate();
        }

        private void UCOverrides_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
        }

        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            SetHeight(isLarge);
        }
    }
}
