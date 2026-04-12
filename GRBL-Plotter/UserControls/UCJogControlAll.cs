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
    public partial class UCJogControlAll : UserControl
    {
        private readonly float DpiScaling;
        private bool isLarge = true;
        private readonly int controlHeightFold = MyControl.MinimumHeightFolded;
        private int controlHeightUnfold = 142;
        private int setHeight = 142;
        private int neededWidth = 120;
        private int axisCount = 3;
        MyControl m = new MyControl();

    //    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
		
        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }
        public event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent;
        protected virtual void OnRaiseGuiControlEvent(UserControlGuiControlEventArgs e)
        { RaiseGuiControlEvent?.Invoke(this, e); }

        System.Windows.Forms.Timer foldTimer = new System.Windows.Forms.Timer();

        public UCJogControlAll()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
            MyControl.DpiScaling = DpiScaling;
            GbJogControl.Click += GbJogControl_Click;
            GbJogControl.MouseEnter += GbJogControl_MouseEnter;
            GbJogControl.MouseLeave += GbJogControl_MouseLeave;

            userControlJogControlXY.RaiseCmdEvent += OnRaiseCmdEventControl;
            userControlJogControludZ.RaiseCmdEvent += OnRaiseCmdEventControl;
            userControlJogControludA.RaiseCmdEvent += OnRaiseCmdEventControl;
            userControlJogControludB.RaiseCmdEvent += OnRaiseCmdEventControl;
            userControlJogControludC.RaiseCmdEvent += OnRaiseCmdEventControl;

            foldTimer.Interval = 500;
            foldTimer.Tick += new System.EventHandler(FoldTimer_Tick);
            foldTimer.Stop();
        }
        private void UCJogControlAll_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
        //    SetUserControlSize();
            userControlJogControludA.SetAxisName("A");
            userControlJogControludB.SetAxisName("B");
            userControlJogControludC.SetAxisName("C");
            SetHeight(Properties.Settings.Default.UserControlJogControlAllIsLarge);
        }
        private void SetDistFeed()
        {
            var p = Properties.Settings.Default;
            userControlJogControlXY.SetDistFeed(1, (float)p.guiJoystickXYStep1, (int)p.guiJoystickXYSpeed1);
            userControlJogControlXY.SetDistFeed(2, (float)p.guiJoystickXYStep2, (int)p.guiJoystickXYSpeed2);
            userControlJogControlXY.SetDistFeed(3, (float)p.guiJoystickXYStep3, (int)p.guiJoystickXYSpeed3);
            userControlJogControlXY.SetDistFeed(4, (float)p.guiJoystickXYStep4, (int)p.guiJoystickXYSpeed4);
            userControlJogControlXY.SetDistFeed(5, (float)p.guiJoystickXYStep5, (int)p.guiJoystickXYSpeed5);

            userControlJogControludZ.SetDistFeed(1, (float)p.guiJoystickZStep1, (int)p.guiJoystickZSpeed1);
            userControlJogControludZ.SetDistFeed(2, (float)p.guiJoystickZStep2, (int)p.guiJoystickZSpeed2);
            userControlJogControludZ.SetDistFeed(3, (float)p.guiJoystickZStep3, (int)p.guiJoystickZSpeed3);
            userControlJogControludZ.SetDistFeed(4, (float)p.guiJoystickZStep4, (int)p.guiJoystickZSpeed4);
            userControlJogControludZ.SetDistFeed(5, (float)p.guiJoystickZStep5, (int)p.guiJoystickZSpeed5);

            userControlJogControludA.SetDistFeed(1, (float)p.guiJoystickAStep1, (int)p.guiJoystickASpeed1);
            userControlJogControludA.SetDistFeed(2, (float)p.guiJoystickAStep2, (int)p.guiJoystickASpeed2);
            userControlJogControludA.SetDistFeed(3, (float)p.guiJoystickAStep3, (int)p.guiJoystickASpeed3);
            userControlJogControludA.SetDistFeed(4, (float)p.guiJoystickAStep4, (int)p.guiJoystickASpeed4);
            userControlJogControludA.SetDistFeed(5, (float)p.guiJoystickAStep5, (int)p.guiJoystickASpeed5);

            userControlJogControludB.SetDistFeed(1, (float)p.guiJoystickAStep1, (int)p.guiJoystickASpeed1);
            userControlJogControludB.SetDistFeed(2, (float)p.guiJoystickAStep2, (int)p.guiJoystickASpeed2);
            userControlJogControludB.SetDistFeed(3, (float)p.guiJoystickAStep3, (int)p.guiJoystickASpeed3);
            userControlJogControludB.SetDistFeed(4, (float)p.guiJoystickAStep4, (int)p.guiJoystickASpeed4);
            userControlJogControludB.SetDistFeed(5, (float)p.guiJoystickAStep5, (int)p.guiJoystickASpeed5);

            userControlJogControludC.SetDistFeed(1, (float)p.guiJoystickAStep1, (int)p.guiJoystickASpeed1);
            userControlJogControludC.SetDistFeed(2, (float)p.guiJoystickAStep2, (int)p.guiJoystickASpeed2);
            userControlJogControludC.SetDistFeed(3, (float)p.guiJoystickAStep3, (int)p.guiJoystickASpeed3);
            userControlJogControludC.SetDistFeed(4, (float)p.guiJoystickAStep4, (int)p.guiJoystickASpeed4);
            userControlJogControludC.SetDistFeed(5, (float)p.guiJoystickAStep5, (int)p.guiJoystickASpeed5);
        }
        internal void SetCommandProtocol(CommandProtocol val)
        {
            userControlJogControlXY.SetCommandProtocol(val);
            userControlJogControludZ.SetCommandProtocol(val);
            userControlJogControludA.SetCommandProtocol(val);
            userControlJogControludB.SetCommandProtocol(val);
            userControlJogControludC.SetCommandProtocol(val);
        }

        internal void SetHeight(bool setLarge)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
            if (setLarge)
            {
                BtnSetup.Visible = true;
                GbJogControl.BackColor = MyControl.PanelBackColor;  // Color.WhiteSmoke;
                GbJogControl.ForeColor = MyControl.PanelForeColor;
                SetControlHeight(controlHeightUnfold);
            }
            else
            {
                BtnSetup.Visible = false;
                GbJogControl.BackColor = MyControl.NotifyYellow;
                GbJogControl.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                SetControlHeight(controlHeightFold);
            }
            isLarge = setLarge;
            Invalidate();
        }
        private void SetControlHeight(int h)
        { Height = (int)(DpiScaling * h); }

        internal int GetControlWidth()
        {
            return neededWidth;
        }

        internal void TriggerMove(string action)
        {
            if (action.Contains("X") || action.Contains("Y"))
                userControlJogControlXY.TriggerMove(action);
            else if (action.Contains("Z") && (axisCount >= 3) && userControlJogControludZ.Enabled)
                userControlJogControludZ.TriggerMove(action);
            else if (action.Contains("A") && (axisCount >= 4) && userControlJogControludA.Enabled)
                userControlJogControludA.TriggerMove(action);
        }
        internal void EnableButtons(bool enable)
        {
            userControlJogControlXY.SetEnabled(enable);
            userControlJogControludZ.SetEnabled(enable);
            userControlJogControludA.SetEnabled(enable);
            userControlJogControludB.SetEnabled(enable);
            userControlJogControludC.SetEnabled(enable);
            BtnSetup.Enabled = true;
        }

        private void OnRaiseCmdEventControl(object sender, GrblPlotter.UserControls.UserControlCmdEventArgs e)
        {
            OnRaiseCmdEvent(e);
        }

        #region FOLD-UNFOLD
        private void GbJogControl_MouseEnter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlJogControlAutomaticUnfold)
            { SetHeight(true); foldTimer.Start(); }
        }
        private void GbJogControl_MouseLeave(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlJogControlAutomaticUnfold)
            {
                var screenPosition = Cursor.Position;
                var clientPosition = GbJogControl.PointToClient(screenPosition);
                if (clientPosition.Y < 0 || clientPosition.Y > GbJogControl.Height ||
                    clientPosition.X < 0 || clientPosition.X > GbJogControl.Width)
                {
                    isLarge = false;
                    SetHeight(isLarge); foldTimer.Stop();
                    Properties.Settings.Default.UserControlJogControlAllIsLarge = isLarge;
                    Properties.Settings.Default.Save();
                }
            }
        }
        protected virtual void FoldTimer_Tick(object sender, EventArgs e)
        { GbJogControl_MouseLeave(sender, e); }
        private void GbJogControl_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbJogControl.PointToClient(screenPosition);
            if (clientPosition.Y <= MyControl.MinimumHeightFolded)
            {
                isLarge = !isLarge;
                SetHeight(isLarge);

                Properties.Settings.Default.UserControlJogControlAllIsLarge = isLarge;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        internal void SetAxisCount(int val, CommandProtocol cp)
        {
            SetCommandProtocol(cp);
            axisCount = val;
            if (val < 4)
            {
                userControlJogControludA.Visible = userControlJogControludB.Visible = userControlJogControludC.Visible = false;
            }
            if (val == 4)
            {
                userControlJogControludA.Visible = true;
                userControlJogControludB.Visible = userControlJogControludC.Visible = false;
            }
            if (val == 5)
            {
                userControlJogControludA.Visible = userControlJogControludB.Visible = true;
                userControlJogControludC.Visible = false;
            }
            if (val == 6)
            {
                userControlJogControludA.Visible = userControlJogControludB.Visible = userControlJogControludC.Visible = true;
            }

            SetUserControlSize();
            if (isLarge) this.Height = (int)(DpiScaling * setHeight);
        }
        internal void SetUserControlSize()
        {
            int sizeNew = Properties.Settings.Default.UserControlJogControlSize;
            controlHeightUnfold = setHeight = (int)(DpiScaling * (sizeNew + 22));
            int sizexy = (int)(DpiScaling * sizeNew);
            int sizez = (int)(DpiScaling * sizeNew / 4);
            int newWith = sizexy;
            if (axisCount > 2) { newWith += (axisCount - 2) * sizez; }
            userControlJogControludZ.Left = sizexy + (int)(DpiScaling * 10);
            userControlJogControludA.Left = sizexy + 1 * sizez + 2 * (int)(DpiScaling * 10);
            userControlJogControludB.Left = sizexy + 2 * sizez + 2 * (int)(DpiScaling * 10);
            userControlJogControludC.Left = sizexy + 3 * sizez + 2 * (int)(DpiScaling * 10);

            neededWidth = this.Width = newWith + (int)(DpiScaling * 25);
            if (isLarge) this.Height = (int)(DpiScaling * setHeight);

            int h = (int)(DpiScaling * Properties.Settings.Default.UserControlJogControlSize);
            int w = h / 3;
            userControlJogControlXY.Size = new Size(h, h);
            userControlJogControludZ.Size = new Size(w, h);
            userControlJogControludA.Size = new Size(w, h);
            userControlJogControludB.Size = new Size(w, h);
            userControlJogControludC.Size = new Size(w, h);
            bool useClassic = Properties.Settings.Default.UserControlJogControlShowButtons;
            userControlJogControlXY.SetApperance(useClassic);
            userControlJogControludZ.SetApperance(useClassic);
            userControlJogControludA.SetApperance(useClassic);
            userControlJogControludB.SetApperance(useClassic);
            userControlJogControludC.SetApperance(useClassic);
            SetDistFeed();
            Invalidate();
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.guiUpdate));
        }
        private void UCJogControlAll_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
        }

        private void BtnSetup_Click(object sender, EventArgs e)
        {
            //nud.Minimum nud.Maximum nud.IncrementStep nud.DecimalPlaces 
            List<ControlDefaults> cd = new List<ControlDefaults>();
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupUnfoldOnMouseOver"), "UserControlJogControlAutomaticUnfold"));
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupControlSize"), "UserControlJogControlSize", new decimal[] { 100m, 300m, 10m }));
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupJoyStickRaster"), "guiJoystickRaster", new decimal[] { 3m, 5m, 1m }));
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupStopJoggingOnMouseUp"), "ctrlSendStopJog"));
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupShowClassicButtons"), "UserControlJogControlShowButtons"));
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupShowArrows"), "UserControlJogControlShowArrow"));
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupShowLabel"), "UserControlJogControlShowLabel"));

            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd);
            if (Properties.Settings.Default.UserControlJogControlAutomaticUnfold)
            {
                Properties.Settings.Default.UserControlJogControlAllIsLarge = false;
                Properties.Settings.Default.Save();
            }
            SetUserControlSize();
        }

        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            SetHeight(isLarge);
        }
    }
}
