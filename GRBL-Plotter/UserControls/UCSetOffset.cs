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
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCSetOffset : UserControl
    {
        private float DpiScaling;
        private bool isLarge = true;
        private int controlHeightFold = MyControl.MinimumHeightFolded;
        private int controlHeightUnfold = 100;

        private bool showNud = true;
        MyControl m = new MyControl();

        public event EventHandler<UserControlTransformEventArgs> RaiseTransformEvent;
        protected virtual void OnRaiseTransformEvent(UserControlTransformEventArgs e)
        { RaiseTransformEvent?.Invoke(this, e); }

        public UCSetOffset()
        {
            InitializeComponent();
            GbOffset.Click += GbOffset_Click;
            DpiScaling = (float)DeviceDpi / 96;

            BtnShowNud_Click(null, null);
        }
        private void UCSetOffset_Load(object sender, EventArgs e)
        {
            SetHeight(Properties.Settings.Default.UserControlOffsetIsLarge);
        }
        internal void SetHeight(bool setLarge)
        {
            if (setLarge)
            {
                GbOffset.BackColor = MyControl.PanelBackColor;  // Color.WhiteSmoke;
                GbOffset.ForeColor = MyControl.PanelForeColor;
                setControlHeight(controlHeightUnfold); 
            }
            else
            {
                GbOffset.BackColor = MyControl.NotifyYellow;
                GbOffset.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
                setControlHeight(controlHeightFold);
            }
            isLarge = setLarge;
            Invalidate();
        }
        internal void SetDimensionText(string txt)
        {
            m.SetLabelSave(LblDimension, txt);
            //   if (!string.IsNullOrEmpty(txt))
            //       LblDimension.Select(0, 0);
        }
        internal void SetDimensionStatus(int nr)
        {
            if (nr == 0) { LblDimension.BackColor = MyControl.PanelHighlight; }
            if (nr == 1) { LblDimension.BackColor = MyControl.NotifyGreen; }
            if (nr == 2) { LblDimension.BackColor = MyControl.NotifyRed; }
        }

        private void GbOffset_Click(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbOffset.PointToClient(screenPosition);
            if (clientPosition.Y < MyControl.MinimumHeightFolded)
            {
                isLarge = !isLarge;
                SetHeight(isLarge);

                Properties.Settings.Default.UserControlOffsetIsLarge = isLarge;
                Properties.Settings.Default.Save();
                Invalidate();
            }
        }
        private void setControlHeight(int h)
        { Height = (int)(DpiScaling * h); }

        private void BtnOffsetApply_Click(object sender, EventArgs e)
        {
            double offsetx = (double)NudX.Value, offsety = (double)NudY.Value;
            int nr = 0;
            if (rBOrigin1.Checked) { nr = 1; }
            else if (rBOrigin2.Checked) { nr = 2; }
            else if (rBOrigin3.Checked) { nr = 3; }
            else if (rBOrigin4.Checked) { nr = 4; }
            else if (rBOrigin5.Checked) { nr = 5; }
            else if (rBOrigin6.Checked) { nr = 6; }
            else if (rBOrigin7.Checked) { nr = 7; }
            else if (rBOrigin8.Checked) { nr = 8; }
            else if (rBOrigin9.Checked) { nr = 9; }

            if (nr != 0)
            {
                OnRaiseTransformEvent(new UserControlTransformEventArgs(offsetx, offsety, nr, sender, e));
            }
        }

        private void BtnShowNud_Click(object sender, EventArgs e)
        {
            if (!showNud)
            {
                BtnShowNud.Left = (int)(DpiScaling * 178);
                BtnOffset.Width = (int)(DpiScaling * 190);
                panel1.Width = (int)(DpiScaling * 194);
                tableLayoutPanel1.ColumnStyles[1].Width = (int)(DpiScaling * 200);
                lblX.Visible = lblY.Visible = true;
            }
            else
            {
                BtnShowNud.Left = (int)(DpiScaling * 58);
                BtnOffset.Width = (int)(DpiScaling * 74);
                panel1.Width = (int)(DpiScaling * 78);
                tableLayoutPanel1.ColumnStyles[1].Width = (int)(DpiScaling * 82);
                lblX.Visible = lblY.Visible = false;
            }
            if ((NudX.Value != 0) || (NudY.Value != 0))
            { BtnShowNud.BackColor = Color.Fuchsia; }
            else
            { BtnShowNud.BackColor = Color.Lime; }

            showNud = !showNud;
        }

        private void numericUpDownOffset_ValueChanged(object sender, EventArgs e)
        {
            if ((NudX.Value != 0) || (NudY.Value != 0))
            { BtnShowNud.BackColor = Color.Fuchsia; }
            else
            { BtnShowNud.BackColor = Color.Lime; }
        }

        public void RestoreColors()
        {
            SetHeight(isLarge);
            if ((NudX.Value != 0) || (NudY.Value != 0))
            { BtnShowNud.BackColor = Color.Fuchsia; }
            else
            { BtnShowNud.BackColor = Color.Lime; }
            MyControl.ChangeColor(LblDimension, MyControl.PanelHighlight, Colors.ContrastColor(MyControl.PanelHighlight));
        }
    }
}
