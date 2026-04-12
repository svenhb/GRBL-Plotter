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
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCFlowControl : UserControl
    {
        private readonly float DpiScaling;
        private bool isLarge = true;
        private readonly int controlHeightFold = MyControl.MinimumHeightFolded;
        private readonly int controlHeightUnfold = 115;

        private readonly static System.Timers.Timer timerBlink = new System.Timers.Timer();
        private static bool highlightKillAlarm = false;
        private static bool highlightResume = false;
        private static bool blinkKillAlarm = false;
        private static bool blinkResume = false;
        private readonly static bool isConnected = true;


        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public UCFlowControl()
        {
            InitializeComponent();
            timerBlink.Interval = 330;
            timerBlink.Enabled = true;
            timerBlink.Start();
            DpiScaling = (float)DeviceDpi / 96;

            GbFlowControl.Click += GbFlowControl_Click;
            timerBlink.Elapsed += TimerBlink_Tick;
        }
        private void UCFlowControl_Load(object sender, EventArgs e)
        {
            SetHeight(Properties.Settings.Default.UserControlFlowControlIsLarge);
        }

        internal void SetHeight(bool setLarge)
        {
            if (setLarge)
            {
                GbFlowControl.BackColor = MyControl.PanelBackColor; // Color.WhiteSmoke;
                GbFlowControl.ForeColor = MyControl.PanelForeColor;
                SetControlHeight(controlHeightUnfold);
            }
            else
            {
                GbFlowControl.BackColor = MyControl.NotifyYellow;
                GbFlowControl.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                SetControlHeight(controlHeightFold);
            }
            isLarge = setLarge;
            Invalidate();
        }

        internal void TriggerCmd(string action)
        {
            if (action.Contains("FeedHold")) { BtnFeedHold.PerformClick(); }
            else if (action.Contains("Reset")) { BtnReset.PerformClick(); }
            else if (action.Contains("Resume")) { BtnResume.PerformClick(); }
            else if (action.Contains("KillAlarm")) { BtnKillAlarm.PerformClick(); }
        }
        internal void EnableButtons(bool enable)
        {
            MyControl.EnableButtons(GbFlowControl, enable);
        }

        private void GbFlowControl_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbFlowControl.PointToClient(screenPosition);
            if (clientPosition.Y < MyControl.MinimumHeightFolded)
            {
                isLarge = !isLarge;
                SetHeight(isLarge);

                Properties.Settings.Default.UserControlFlowControlIsLarge = isLarge;
                Properties.Settings.Default.Save();
                Invalidate();
            }
        }
        private void SetControlHeight(int h)
        { Height = (int)(DpiScaling * h); }

        public void HighlightKillAlarm(bool highlight)
        {
            blinkKillAlarm = highlight;
            if (highlight)
            {
                timerBlink.Enabled = true;
                SetHeight(true);
            }
            else
            {
                timerBlink.Enabled = blinkResume;
                BtnKillAlarm.BackColor = BtnFeedHold.BackColor;
                BtnKillAlarm.ForeColor = Colors.ContrastColor(BtnFeedHold.BackColor);
            }
        }
        public void HighlightResume(bool highlight)
        {
            blinkResume = highlight;
            if (highlight)
            {
                timerBlink.Enabled = true;
                SetHeight(true);
            }
            else
            {
                timerBlink.Enabled = blinkKillAlarm;
                BtnResume.BackColor = BtnFeedHold.BackColor;
                BtnResume.ForeColor = Colors.ContrastColor(BtnFeedHold.BackColor);
            }
        }

        private void TimerBlink_Tick(object sender, EventArgs e)
        {
            if (blinkKillAlarm)
            {
                if (highlightKillAlarm)
                {
                    BtnKillAlarm.BackColor = BtnFeedHold.BackColor;
                    BtnKillAlarm.ForeColor = Colors.ContrastColor(BtnResume.BackColor);

                }
                else
                {
                    BtnKillAlarm.BackColor = MyControl.NotifyYellow;
                    BtnKillAlarm.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                }
                highlightKillAlarm = !highlightKillAlarm;
            }
            if (blinkResume)
            {
                if (highlightResume)
                {
                    BtnResume.BackColor = BtnFeedHold.BackColor;
                    BtnResume.ForeColor = Colors.ContrastColor(BtnResume.BackColor);

                }
                else
                {
                    BtnResume.BackColor = MyControl.NotifyYellow;
                    BtnResume.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                }
                highlightResume = !highlightResume;
            }
        }

        private void BtnFeedHold_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs("", (byte)'!', sender, e));
            if (isConnected)
                HighlightResume(true);
        }
        private void BtnSafetyDoor_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x84, sender, e));
            if (isConnected)
                HighlightResume(true);
        }
        private void BtnResume_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs("", (byte)'~', sender, e));
            HighlightResume(false);
        }
        private void BtnKillAlarm_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("$X", 0, sender, e)); HighlightKillAlarm(false); }
        private void BtnReset_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x18, sender, e)); }

        public void RestoreColors()
        {
            SetHeight(isLarge);
        }
    }
}
