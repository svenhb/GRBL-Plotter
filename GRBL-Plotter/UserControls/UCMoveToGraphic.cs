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
    public partial class UCMoveToGraphic : UserControl
    {
        private readonly Image arrow_r = Properties.Resources.a_re;		// arrow to the right
        private readonly Image arrow_d = Properties.Resources.a_de;      // arrow to upper-right
        private readonly Color ColorBtnDisabled = SystemColors.Control;
        private bool btnPressed = false;
        private bool dimensionIsSet = false;
        private Dimensions gD = new Dimensions();
        private string jogCommand = "$J=G90";
        private int feed = 10000;//"F10000";
        private bool jogEnabled = false;

        private readonly float DpiScaling;
        private bool isLarge = true;
        private readonly int controlHeightFold = MyControl.MinimumHeightFolded;
        private int controlHeightUnfold = 120;

        private readonly int setHeight = 120;
        private int neededWidth = 120;
        MyControl m = new MyControl();

    //    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
		
        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent;
        protected virtual void OnRaiseGuiControlEvent(UserControlGuiControlEventArgs e)
        { RaiseGuiControlEvent?.Invoke(this, e); }

        System.Windows.Forms.Timer foldTimer = new System.Windows.Forms.Timer();

        public UCMoveToGraphic()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
            GbGraphic.Click += GbGraphic_Click;
            GbGraphic.MouseEnter += GbGraphic_MouseEnter;
            GbGraphic.MouseLeave += GbGraphic_MouseLeave;

            foldTimer.Interval = 500;
            foldTimer.Tick += new System.EventHandler(FoldTimer_Tick);
            foldTimer.Stop();
        }
        private void UCMoveToGraphic_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            AlignButtons();
            SetEnabled(jogEnabled);
            SetHeight(Properties.Settings.Default.UserControlMoveToGraphicIsLarge);
        }
        internal void SetEnabled(bool enable)
        {
            SetButtonImages(enable);
            BtnSetup.Enabled = true;

            jogEnabled = enable;
            Invalidate();
        }

        internal void SetHeight(bool setLarge)
        {
            if (setLarge)
            {
                BtnSetup.Visible = true;
                GbGraphic.BackColor = MyControl.PanelBackColor; // Color.WhiteSmoke;
                GbGraphic.ForeColor = MyControl.PanelForeColor;
                SetControlHeight(controlHeightUnfold);
            }
            else
            {
                BtnSetup.Visible = false;
                GbGraphic.BackColor = MyControl.NotifyYellow;
                GbGraphic.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                SetControlHeight(controlHeightFold);
            }
            isLarge = setLarge;
            Invalidate();
        }


        internal int GetControlWidth()
        {
            return neededWidth;
        }
        internal void SetBackColor(Color val)
        {
            GbGraphic.BackColor = val;
        }
        internal void SetFontColor(Color val)
        { }
        internal void SetControlColor(Color val)
        { }

        internal void SetDimension(Dimensions val)
        {
            gD = val;
            SetToolTip();
            dimensionIsSet = true;
        }

        internal void SetCommandProtocol(CommandProtocol val)
        {
            if (val == CommandProtocol.grblCanJog)
                jogCommand = "$J=G90";
            else
                jogCommand = "G90G1";
        }

        internal void EnableButtons(bool enable)
        {
            SetEnabled(enable);
            BtnSetup.Enabled = true;
        }

        #region FOLD-UNFOLD
        private void GbGraphic_MouseEnter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlMoveToGraphicAutomaticUnfold)
            { SetHeight(true); foldTimer.Start(); }
        }
        private void GbGraphic_MouseLeave(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserControlMoveToGraphicAutomaticUnfold)
            {
                var screenPosition = Cursor.Position;
                var clientPosition = GbGraphic.PointToClient(screenPosition);
                if (clientPosition.Y < 0 || clientPosition.Y > GbGraphic.Height ||
                    clientPosition.X < 0 || clientPosition.X > GbGraphic.Width)
                {
                    isLarge = false;
                    SetHeight(isLarge); foldTimer.Stop();
                    Properties.Settings.Default.UserControlMoveToGraphicIsLarge = isLarge;
                    Properties.Settings.Default.Save();
                }
            }
        }
        protected virtual void FoldTimer_Tick(object sender, EventArgs e)
        { GbGraphic_MouseLeave(sender, e); }

        private void GbGraphic_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbGraphic.PointToClient(screenPosition);
            if (clientPosition.Y <= MyControl.MinimumHeightFolded)
            {
                isLarge = !isLarge;
                SetHeight(isLarge);
                Properties.Settings.Default.UserControlMoveToGraphicIsLarge = isLarge;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        private void SetControlHeight(int h)
        { Height = (int)(DpiScaling * h); }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (!dimensionIsSet)
                return;
            string coord = "";
            feed = (int)NudFeed.Value;

            if (sender == Btn1) { coord = string.Format("X{0:0.000}Y{1:0.000}", gD.minx, gD.maxy); btnPressed = true; }
            else if (sender == Btn3) { coord = string.Format("X{0:0.000}Y{1:0.000}", gD.maxx, gD.maxy); btnPressed = true; }
            else if (sender == Btn7) { coord = string.Format("X{0:0.000}Y{1:0.000}", gD.minx, gD.miny); btnPressed = true; }
            else if (sender == Btn9) { coord = string.Format("X{0:0.000}Y{1:0.000}", gD.maxx, gD.miny); btnPressed = true; }

            else if (sender == Btn2)
            {
                if (Properties.Settings.Default.UserControlMoveToGraphicCenterX)
                    coord = string.Format("X{0:0.000}Y{1:0.000}", gD.cenx, gD.maxy);
                else
                    coord = string.Format("Y{0:0.000}", gD.maxy);
                btnPressed = true;
            }
            else if (sender == Btn8)
            {
                if (Properties.Settings.Default.UserControlMoveToGraphicCenterX)
                    coord = string.Format("X{0:0.000}Y{1:0.000}", gD.cenx, gD.miny);
                else
                    coord = string.Format("Y{0:0.000}", gD.miny);
                btnPressed = true;
            }


            else if (sender == Btn4)
            {
                if (Properties.Settings.Default.UserControlMoveToGraphicCenterY)
                    coord = string.Format("X{0:0.000}Y{1:0.000}", gD.minx, gD.ceny);
                else
                    coord = string.Format("X{0:0.000}", gD.minx);
                btnPressed = true;
            }
            else if (sender == Btn6)
            {
                if (Properties.Settings.Default.UserControlMoveToGraphicCenterY)
                    coord = string.Format("X{0:0.000}Y{1:0.000}", gD.maxx, gD.ceny);
                else
                    coord = string.Format("X{0:0.000}", gD.maxx);
                btnPressed = true;
            }

            else if (sender == Btn5) { coord = string.Format("X{0:0.000}Y{1:0.000}", gD.cenx, gD.ceny); btnPressed = true; }

            if (btnPressed)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}{1}F{2}", jogCommand, coord, feed).Replace(",", "."), 0, sender, e));
        }
        private void BtnFraming_Click(object sender, EventArgs e)
        {
            if (!dimensionIsSet)
                return;
            feed = (int)NudFeed.Value;
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.000}Y{2:0.000}F{3}", jogCommand, gD.minx, gD.miny, feed).Replace(",", "."), 0, sender, e)); // lower left
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.000}Y{2:0.000}F{3}", jogCommand, gD.minx, gD.maxy, feed).Replace(",", "."), 0, sender, e)); // upper left
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.000}Y{2:0.000}F{3}", jogCommand, gD.maxx, gD.maxy, feed).Replace(",", "."), 0, sender, e)); // upper right
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.000}Y{2:0.000}F{3}", jogCommand, gD.maxx, gD.miny, feed).Replace(",", "."), 0, sender, e)); // lower right
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.000}Y{2:0.000}F{3}", jogCommand, gD.minx, gD.miny, feed).Replace(",", "."), 0, sender, e)); // lower left	
        }

        private void SetButtonImages(bool enabled)
        {
            Image tmp_r = (Image)arrow_r.Clone();
            Image tmp_d = (Image)arrow_d.Clone();

            if (!enabled)
            {
                tmp_r = MyControl.BitmapSetToGray(tmp_r);
                tmp_d = MyControl.BitmapSetToGray(tmp_d);
            }

            Btn6.BackgroundImage = (Image)tmp_r.Clone();
            Btn8.BackgroundImage = (Image)tmp_r.Clone(); Btn8.BackgroundImage.RotateFlip((RotateFlipType.Rotate90FlipNone));
            Btn4.BackgroundImage = (Image)tmp_r.Clone(); Btn4.BackgroundImage.RotateFlip((RotateFlipType.Rotate180FlipNone));
            Btn2.BackgroundImage = (Image)tmp_r.Clone(); Btn2.BackgroundImage.RotateFlip((RotateFlipType.Rotate270FlipNone));

            Btn3.BackgroundImage = (Image)tmp_d.Clone();
            Btn9.BackgroundImage = (Image)tmp_d.Clone(); Btn9.BackgroundImage.RotateFlip((RotateFlipType.Rotate90FlipNone));
            Btn7.BackgroundImage = (Image)tmp_d.Clone(); Btn7.BackgroundImage.RotateFlip((RotateFlipType.Rotate180FlipNone));
            Btn1.BackgroundImage = (Image)tmp_d.Clone(); Btn1.BackgroundImage.RotateFlip((RotateFlipType.Rotate270FlipNone));

            tmp_r = Properties.Resources.a_ce;
            if (!enabled)
            { tmp_r = MyControl.BitmapSetToGray(tmp_r); }
            Btn5.BackgroundImage = tmp_r;

            tmp_r = Properties.Resources.framing;
            if (!enabled)
            { tmp_r = MyControl.BitmapSetToGray(tmp_r); }
            BtnFraming.BackgroundImage = tmp_r;
        }

        private void UCMoveToGraphic_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = this.Width - (int)(DpiScaling * MyControl.BtnSetupRight);
        }
        private void AlignButtons()
        {
            int h = (int)(DpiScaling * Properties.Settings.Default.UserControlMoveToGraphicSize);
            if (h > (int)(DpiScaling * 300))
                h = (int)(DpiScaling * 150);
            controlHeightUnfold = h + (int)(DpiScaling * 20);

            int Offx = (int)(DpiScaling * 3);
            int Offy = (int)(DpiScaling * MyControl.MinimumHeightFolded);
            int y = h / 3;
            int BtnHeight = y;
            int x = y;
            int BtnWidth = BtnHeight;
            Btn1.Left = Btn4.Left = Btn7.Left = 0 + Offx;
            Btn2.Left = Btn5.Left = Btn8.Left = x + Offx;
            Btn3.Left = Btn6.Left = Btn9.Left = 2 * x + Offx;
            Btn1.Top = Btn2.Top = Btn3.Top = 0 + Offy;
            Btn4.Top = Btn5.Top = Btn6.Top = y + Offy;
            Btn7.Top = Btn8.Top = Btn9.Top = 2 * y + Offy;
            Btn1.Width = Btn2.Width = Btn3.Width = Btn4.Width = Btn5.Width = Btn6.Width = Btn7.Width = Btn8.Width = Btn9.Width = BtnWidth;
            Btn1.Height = Btn2.Height = Btn3.Height = Btn4.Height = Btn5.Height = Btn6.Height = Btn7.Height = Btn8.Height = Btn9.Height = BtnHeight;

            BtnFraming.Width = 2 * BtnWidth;
            BtnFraming.Height = 2 * BtnHeight;
            BtnFraming.Left = 3 * x + 2 * Offx;
            BtnFraming.Top = y + Offy;

            NudFeed.Top = Offy + (int)(DpiScaling * 4);
            NudFeed.Left = 3 * x + 2 * Offx;
            NudFeed.Width = BtnFraming.Width;

            neededWidth = this.Width = BtnFraming.Left + BtnFraming.Width + (int)(DpiScaling * 5);
            SetHeight(isLarge);
            Invalidate();

            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.guiUpdate));
        }
        private void SetToolTip()
        {
            toolTip1.SetToolTip(Btn1, string.Format("X{0:0.000}\nY{1:0.000}", gD.minx, gD.maxy));
            toolTip1.SetToolTip(Btn2, string.Format("Y{0:0.000}", gD.maxy));
            toolTip1.SetToolTip(Btn3, string.Format("X{0:0.000}\nY{1:0.000}", gD.maxx, gD.maxy));
            toolTip1.SetToolTip(Btn4, string.Format("X{0:0.000}", gD.minx));
            toolTip1.SetToolTip(Btn5, string.Format("X{0:0.000}\nY{1:0.000}", gD.cenx, gD.ceny));
            toolTip1.SetToolTip(Btn6, string.Format("X{0:0.000}", gD.maxx));
            toolTip1.SetToolTip(Btn7, string.Format("X{0:0.000}\nY{1:0.000}", gD.minx, gD.miny));
            toolTip1.SetToolTip(Btn8, string.Format("Y{0:0.000}", gD.miny));
            toolTip1.SetToolTip(Btn9, string.Format("X{0:0.000}\nY{1:0.000}", gD.maxx, gD.miny));
        }

        private void BtnSetup_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>();
            cd.Add(new ControlDefaults(Localization.GetString("ucSetupUnfoldOnMouseOver"), "UserControlMoveToGraphicAutomaticUnfold"));
            cd.Add(new ControlDefaults(LblSetupOption2.Text, "UserControlMoveToGraphicSize", new decimal[] { 80m, 300m, 10m }));
            cd.Add(new ControlDefaults(LblSetupOption3.Text, "UserControlMoveToGraphicCenterX", new decimal[] { 80m, 300m, 10m }));
            cd.Add(new ControlDefaults(LblSetupOption4.Text, "UserControlMoveToGraphicCenterY", new decimal[] { 80m, 300m, 10m }));

            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd);
            if (Properties.Settings.Default.UserControlMoveToGraphicAutomaticUnfold)
            {
                Properties.Settings.Default.UserControlMoveToGraphicIsLarge = false;
                Properties.Settings.Default.Save();
            }
            AlignButtons();
        }
        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            SetHeight(isLarge);
        }
    }
}
