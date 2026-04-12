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
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCMoveToZero : UserControl
    {
        private readonly float DpiScaling;
        private bool isLarge = true;
        private readonly bool hasJogging = true;

        private readonly int controlHeightFold = MyControl.MinimumHeightFolded;
        private int controlHeightUnfold = 90;

        private string jogCommand = "$J=G90";
        private int feed = 10000;
        MyControl m = new MyControl();

   //     private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
   
        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        System.Windows.Forms.Timer foldTimer = new System.Windows.Forms.Timer();

        public UCMoveToZero()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
            GbZero.Click += GbZero_Click;
            GbZero.MouseEnter += GbZero_MouseEnter;
            GbZero.MouseLeave += GbZero_MouseLeave;

            foldTimer.Interval = 500;
            foldTimer.Tick += new System.EventHandler(FoldTimer_Tick);
            foldTimer.Stop();
        }
        private void UCMoveToZero_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            SetHeight(Properties.Settings.Default.UserControlMoveToZeroIsLarge);
        }

        internal void SetHeight(bool setLarge)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
            if (setLarge)
            {
                BtnSetup.Visible = true;
                GbZero.BackColor = MyControl.PanelBackColor;    // Color.WhiteSmoke;
                GbZero.ForeColor = MyControl.PanelForeColor;
                SetControlHeight(controlHeightUnfold);
            }
            else
            {
                BtnSetup.Visible = false;
                GbZero.BackColor = MyControl.NotifyYellow;
                GbZero.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                SetControlHeight(controlHeightFold);
            }
            isLarge = setLarge;
            Invalidate();
        }
      
        internal void EnableButtons(bool enable)
        {
            MyControl.EnableButtons(GbZero, enable);
            BtnSetup.Enabled = true;
        }

        internal void SetCommandProtocol(CommandProtocol val)
        {
            if (val == CommandProtocol.grblCanJog)
            {
                controlHeightUnfold = 90;
                jogCommand = "$J=G90";
                BtnStop.Visible = true;
            }
            else
            {
                controlHeightUnfold = 65;
                jogCommand = "G1G90";
                BtnStop.Visible = false;
            }
        }

        internal void TriggerMove(string action)
        {
            if (action.Contains("XY")) { BtnXY.PerformClick(); }
            else if (action.Contains("X")) { BtnX.PerformClick(); }
            else if (action.Contains("Y")) { BtnY.PerformClick(); }
            else if (action.Contains("Z")) { BtnZ.PerformClick(); }
            else if (action.Contains("A")) { BtnA.PerformClick(); }
        }

        private void BtnX_Click(object sender, EventArgs e)
        { SendCommand("X0", sender, e); }
        private void BtnY_Click(object sender, EventArgs e)
        { SendCommand("Y0", sender, e); }
        private void BtnZ_Click(object sender, EventArgs e)
        { SendCommand("Z0", sender, e); }
        private void BtnA_Click(object sender, EventArgs e)
        { SendCommand("A0", sender, e); }
        private void BtnB_Click(object sender, EventArgs e)
        { SendCommand("B0", sender, e); }
        private void BtnC_Click(object sender, EventArgs e)
        { SendCommand("C0", sender, e); }
        private void BtnXY_Click(object sender, EventArgs e)
        { SendCommand("X0Y0", sender, e); }
        private void BtnStop_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs("", 0x85, sender, e)); }

        private void SendCommand(string axis, object sender, EventArgs e)
        {
            feed = (int)Properties.Settings.Default.UserControlMoveToGraphicFeed;
            if (CbG0.Checked)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G0{0}", axis).Replace(",", "."), 0, sender, e));
            else
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}{1}F{2}", jogCommand, axis, feed).Replace(",", "."), 0, sender, e));
        }

        #region FOLD-UNFOLD
        private void GbZero_MouseEnter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlMoveToZeroAutomaticUnfold)
            { SetHeight(true); foldTimer.Start(); }
        }
        private void GbZero_MouseLeave(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlMoveToZeroAutomaticUnfold)
            {
                var screenPosition = Cursor.Position;
                var clientPosition = GbZero.PointToClient(screenPosition);
                if (clientPosition.Y < 0 || clientPosition.Y > GbZero.Height ||
                    clientPosition.X < 0 || clientPosition.X > GbZero.Width)
                {
                    isLarge = false;
                    SetHeight(isLarge); foldTimer.Stop();
                    Properties.Settings.Default.UserControlMoveToZeroIsLarge = isLarge;
                    Properties.Settings.Default.Save();
                }
            }
        }
        protected virtual void FoldTimer_Tick(object sender, EventArgs e)
        { GbZero_MouseLeave(sender, e); }

        private void GbZero_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbZero.PointToClient(screenPosition);
            if (clientPosition.Y <= MyControl.MinimumHeightFolded)
            {
                isLarge = !isLarge;
                SetHeight(isLarge);

                Properties.Settings.Default.UserControlMoveToZeroIsLarge = isLarge;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        private void UCMoveToZero_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);

            CbG0.Top= BtnXY.Top = (int)(DpiScaling*38);
            BtnStop.Top = (int)(DpiScaling * 62);
        }

        private void SetControlHeight(int val)
        { this.Height = (int)(DpiScaling * val); }

        internal void SetAxisCount(int val, CommandProtocol cp)
        {
            SetCommandProtocol(cp);
            if (val < 4)
            {
                BtnA.Visible = BtnB.Visible = BtnC.Visible = false;
                this.Width = (int)(DpiScaling * 165);
            }
            if (val == 4)
            {
                BtnA.Visible = true;
                BtnB.Visible = BtnC.Visible = false;
                this.Width = (int)(DpiScaling * 200);
            }
            if (val == 5)
            {
                BtnA.Visible = BtnB.Visible = true;
                BtnC.Visible = false;
                this.Width = (int)(DpiScaling * 235);
            }
            if (val == 6)
            {
                BtnA.Visible = BtnB.Visible = BtnC.Visible = true;
                this.Width = (int)(DpiScaling * 270);
            }

        }

        private void BtnSetup_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>();
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupUnfoldOnMouseOver"), "UserControlMoveToZeroAutomaticUnfold"));

            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd); //keyValueSetupParameter);
            if (Properties.Settings.Default.UserControlMoveToZeroAutomaticUnfold)
            {
                Properties.Settings.Default.UserControlMoveToZeroIsLarge = false;
                Properties.Settings.Default.Save();
            }
        }
        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            SetHeight(isLarge);
        }
    }
}
