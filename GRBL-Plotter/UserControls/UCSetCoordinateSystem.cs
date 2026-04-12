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
    public partial class UCSetCoordinateSystem : UserControl
    {
        private readonly float DpiScaling;
        private bool isLarge = true;
        private readonly int controlHeightFold = MyControl.MinimumHeightFolded;
        private int controlHeightUnfold = 60;

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        System.Windows.Forms.Timer foldTimer = new System.Windows.Forms.Timer();

        public UCSetCoordinateSystem()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
            GbSetCoordinate.Click += GbSetCoordinate_Click;
            GbSetCoordinate.MouseEnter += GbSetCoordinate_MouseEnter;
            GbSetCoordinate.MouseLeave += GbSetCoordinate_MouseLeave;

            foldTimer.Interval = 500;
            foldTimer.Tick += new System.EventHandler(FoldTimer_Tick);
            foldTimer.Stop();
        }

        private void UCSetCoordinateSystem_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            SetHeight(Properties.Settings.Default.UserControlSetCoordinateIsLarge);
        }

        internal void SetHeight(bool setLarge)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
            if (setLarge)
            {
                TlpButtons.Visible = BtnSetup.Visible = true;
                GbSetCoordinate.BackColor = MyControl.PanelBackColor;    // Color.WhiteSmoke;
                GbSetCoordinate.ForeColor = MyControl.PanelForeColor;
                SetControlHeight(controlHeightUnfold);
            }
            else
            {
                TlpButtons.Visible = BtnSetup.Visible = false;
                GbSetCoordinate.BackColor = MyControl.NotifyYellow;
                GbSetCoordinate.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                SetControlHeight(controlHeightFold);
            }
            isLarge = setLarge;
            Invalidate();
        }
        private void SetControlHeight(int val)
        { this.Height = (int)(DpiScaling * val); }

        internal void EnableButtons(bool enable)
        {
            MyControl.EnableButtons(TlpButtons, enable);
            BtnSetup.Enabled = true;
        }

        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            SetHeight(isLarge);
        }

        private void BtnSetup_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>();
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupUnfoldOnMouseOver"), "UserControlSetCoordinateAutomaticUnfold"));

            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd); //keyValueSetupParameter);
            if (Properties.Settings.Default.UserControlSetCoordinateAutomaticUnfold)
            {
                Properties.Settings.Default.UserControlSetCoordinateIsLarge = false;
                Properties.Settings.Default.Save();
            }
        }

        private void BtnG5x_Click(object sender, EventArgs e)
        {
            string nr = ((Button)sender).Text;
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}", nr), 0, sender, e));
        }
        private void SendCommand(string coord, object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G{0}", coord), 0, sender, e));
        }

        private void UCSetCoordinateSystem_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
        }
		
        #region FOLD-UNFOLD
        private void GbSetCoordinate_MouseEnter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlSetCoordinateAutomaticUnfold)
            { SetHeight(true); foldTimer.Start(); }
        }
        private void GbSetCoordinate_MouseLeave(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlSetCoordinateAutomaticUnfold)
            {
                var screenPosition = Cursor.Position;
                var clientPosition = GbSetCoordinate.PointToClient(screenPosition);
                if (clientPosition.Y < 0 || clientPosition.Y > GbSetCoordinate.Height ||
                    clientPosition.X < 0 || clientPosition.X > GbSetCoordinate.Width)
                {
                    isLarge = false;
                    SetHeight(isLarge); foldTimer.Stop();
                    Properties.Settings.Default.UserControlSetCoordinateIsLarge = isLarge;
                    Properties.Settings.Default.Save();
                }
            }
        }
        protected virtual void FoldTimer_Tick(object sender, EventArgs e)
        { GbSetCoordinate_MouseLeave(sender, e); }

        private void GbSetCoordinate_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbSetCoordinate.PointToClient(screenPosition);
            if (clientPosition.Y <= MyControl.MinimumHeightFolded)
            {
                isLarge = !isLarge;
                SetHeight(isLarge);

                Properties.Settings.Default.UserControlSetCoordinateIsLarge = isLarge;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

    }
}
