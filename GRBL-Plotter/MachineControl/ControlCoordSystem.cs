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
 * 2019-08-13 add PRB, TLO status
 * 2019-08-15 add logger
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2019-11-10 add .Replace(',', '.')
 * 2021-07-02 code clean up / code quality
 * 2021-12-22 check if is connected to grbl before sending code
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlCoordSystem : Form
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ControlCoordSystem()
        {
            Logger.Trace("++++++ ControlCoordSystem START ++++++");
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }
        private void ControlCoordSystem_Load(object sender, EventArgs e)
        {
            RefreshValues(true);
            Location = Properties.Settings.Default.locationCntrlCoordForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }
        }
        private void ControlCoordSystem_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ ControlCoordSystem Stop ++++++");
            Properties.Settings.Default.locationCntrlCoordForm = Location;
        }

        public void UpdateTLO(bool tlOactive, double tlOvalue)
        {
            if (tlOactive)
                lblTLO.BackColor = Color.Lime;
            else
                lblTLO.BackColor = SystemColors.Control;
            lblTLO.Text = String.Format("                  {0,8:###0.000}", tlOvalue);
        }
        public void MarkActiveCoordSystem(string cmd)
        {
            if (cmd == "G54") lblOffset1.BackColor = btnSelect1.BackColor = Color.Lime; else lblOffset1.BackColor = btnSelect1.BackColor = SystemColors.Control;
            if (cmd == "G55") lblOffset2.BackColor = btnSelect2.BackColor = Color.Lime; else lblOffset2.BackColor = btnSelect2.BackColor = SystemColors.Control;
            if (cmd == "G56") lblOffset3.BackColor = btnSelect3.BackColor = Color.Lime; else lblOffset3.BackColor = btnSelect3.BackColor = SystemColors.Control;
            if (cmd == "G57") lblOffset4.BackColor = btnSelect4.BackColor = Color.Lime; else lblOffset4.BackColor = btnSelect4.BackColor = SystemColors.Control;
            if (cmd == "G58") lblOffset5.BackColor = btnSelect5.BackColor = Color.Lime; else lblOffset5.BackColor = btnSelect5.BackColor = SystemColors.Control;
            if (cmd == "G59") lblOffset6.BackColor = btnSelect6.BackColor = Color.Lime; else lblOffset6.BackColor = btnSelect6.BackColor = SystemColors.Control;
        }

        int delay = 0;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (delay > 0)
                delay--;
            else
            {
                ShowValues();
                timer1.Stop();
            }
        }

        public void ShowValues()
        {
            Logger.Info("Update values");
            lblOffset1.Text = Grbl.DisplayCoord("G54");
            lblOffset2.Text = Grbl.DisplayCoord("G55");
            lblOffset3.Text = Grbl.DisplayCoord("G56");
            lblOffset4.Text = Grbl.DisplayCoord("G57");
            lblOffset5.Text = Grbl.DisplayCoord("G58");
            lblOffset6.Text = Grbl.DisplayCoord("G59");
            lblG28.Text = Grbl.DisplayCoord("G28");
            lblG30.Text = Grbl.DisplayCoord("G30");
            lblG92.Text = Grbl.DisplayCoord("G92");
            lblPRB.Text = Grbl.DisplayCoord("PRB");
            if (Grbl.GetPRBStatus() == true)
                lblPRB.BackColor = Color.Lime;
            else
                lblPRB.BackColor = SystemColors.Control;

            lblTLO.Text = Grbl.DisplayCoord("TLO");

            ResizeForm(Grbl.axisCount - 3);
        }

        private void BtnSelect1_Click(object sender, EventArgs e) { SendCmd("G54"); }
        private void BtnSelect2_Click(object sender, EventArgs e) { SendCmd("G55"); }
        private void BtnSelect3_Click(object sender, EventArgs e) { SendCmd("G56"); }
        private void BtnSelect4_Click(object sender, EventArgs e) { SendCmd("G57"); }
        private void BtnSelect5_Click(object sender, EventArgs e) { SendCmd("G58"); }
        private void BtnSelect6_Click(object sender, EventArgs e) { SendCmd("G59"); }

        private void SendCmd(string cmd)
        {
            MarkActiveCoordSystem(cmd);
            SendCommandEvent(new CmdEventArgs(cmd.Replace(',', '.')));
        }
        private void BtnSet1_Click(object sender, EventArgs e) { SetCoord(1, Grbl.posWork); }
        private void BtnSet2_Click(object sender, EventArgs e) { SetCoord(2, Grbl.posWork); }
        private void BtnSet3_Click(object sender, EventArgs e) { SetCoord(3, Grbl.posWork); }
        private void BtnSet4_Click(object sender, EventArgs e) { SetCoord(4, Grbl.posWork); }
        private void BtnSet5_Click(object sender, EventArgs e) { SetCoord(5, Grbl.posWork); }
        private void BtnSet6_Click(object sender, EventArgs e) { SetCoord(6, Grbl.posWork); }

        private void BtnSetM1_Click(object sender, EventArgs e) { SetCoord(1, Grbl.PosMarker); }
        private void BtnSetM2_Click(object sender, EventArgs e) { SetCoord(2, Grbl.PosMarker); }
        private void BtnSetM3_Click(object sender, EventArgs e) { SetCoord(3, Grbl.PosMarker); }
        private void BtnSetM4_Click(object sender, EventArgs e) { SetCoord(4, Grbl.PosMarker); }
        private void BtnSetM5_Click(object sender, EventArgs e) { SetCoord(5, Grbl.PosMarker); }
        private void BtnSetM6_Click(object sender, EventArgs e) { SetCoord(6, Grbl.PosMarker); }

        private void BtnSetC1_Click(object sender, EventArgs e) { SetCoord(1); }
        private void BtnSetC2_Click(object sender, EventArgs e) { SetCoord(2); }
        private void BtnSetC3_Click(object sender, EventArgs e) { SetCoord(3); }
        private void BtnSetC4_Click(object sender, EventArgs e) { SetCoord(4); }
        private void BtnSetC5_Click(object sender, EventArgs e) { SetCoord(5); }
        private void BtnSetC6_Click(object sender, EventArgs e) { SetCoord(6); }

        private void BtnG28Move_Click(object sender, EventArgs e) { SendCommandEvent(new CmdEventArgs("G28")); }
        private void BtnG28Set_Click(object sender, EventArgs e) { SendCommandEvent(new CmdEventArgs("G28.1")); RefreshValues(); }
        private void BtnG30Move_Click(object sender, EventArgs e) { SendCommandEvent(new CmdEventArgs("G30")); }
        private void BtnG30Set_Click(object sender, EventArgs e) { SendCommandEvent(new CmdEventArgs("G30.1")); RefreshValues(); }
        private void BtnG92Off_Click(object sender, EventArgs e) { SendCommandEvent(new CmdEventArgs("G92.1")); RefreshValues(); }
        private void BtnG43_Click(object sender, EventArgs e)
        {
            lblTLO.BackColor = Color.Lime;
            SendCommandEvent(new CmdEventArgs((string.Format("G43.1 Z{0:0.000}", Grbl.posWork.Z).Replace(',', '.')))); RefreshValues();
        }
        private void BtnG49_Click(object sender, EventArgs e)
        {
            lblTLO.BackColor = SystemColors.Control;
            SendCommandEvent(new CmdEventArgs("G49")); RefreshValues();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!Grbl.isConnected)
            {
                MessageBox.Show(Localization.GetString("grblNotConnected"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            RefreshValues();
        }

        private void SetCoord(int nr, XyzPoint tmp = new XyzPoint())
        {
            string cmd = String.Format("G10 L2 P{0} X{1:0.000} Y{2:0.000} Z{3:0.000}", nr, tmp.X, tmp.Y, tmp.Z);
            if (Grbl.axisA) cmd = String.Format("{0} A{1:0.000}", cmd, tmp.A);
            if (Grbl.axisB) cmd = String.Format("{0} B{1:0.000}", cmd, tmp.B);
            if (Grbl.axisC) cmd = String.Format("{0} C{1:0.000}", cmd, tmp.C);
            SendCommandEvent(new CmdEventArgs(cmd.Replace(',', '.')));
            //            refreshValues();
        }
        public void RefreshValues()
        { RefreshValues(false); }
        public void RefreshValues(bool init)
        {
            Logger.Info("Ask refresh");
            SendCommandEvent(new CmdEventArgs("$#"));
            delay = 5;
            timer1.Enabled = true;
            timer1.Start();
            if (init)
                SendCommandEvent(new CmdEventArgs("$G"));
        }

        private void ResizeForm(int add)
        {
            if (add < 0) add = 0;
            if (add > 3) add = 3;
            int newWidth = 70 * add;
            Width = 470 + newWidth;
            gB_offset.Width = 200 + newWidth;
            gB_G28.Width = gB_G38.Width = gB_G92.Width = 446 + newWidth;
        }

        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void SendCommandEvent(CmdEventArgs e)
        {
            EventHandler<CmdEventArgs> handler = RaiseCmdEvent;
            handler?.Invoke(this, e);
        }
    }

}
