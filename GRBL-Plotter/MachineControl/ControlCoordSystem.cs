/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GRBL_Plotter
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
        {   refreshValues(true);
            Location = Properties.Settings.Default.locationCntrlCoordForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }
        }
        private void ControlCoordSystem_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ ControlCoordSystem Stop ++++++");
            Properties.Settings.Default.locationCntrlCoordForm = Location;
        }

        public void updateTLO(bool TLOactive, double TLOvalue)
        {   if (TLOactive)
                lblTLO.BackColor = Color.Lime; 
            else
                lblTLO.BackColor = SystemColors.Control;     
            lblTLO.Text= String.Format("                  {0,8:###0.000}", TLOvalue);
        }
        public void markActiveCoordSystem(string cmd)
        {   if (cmd == "G54") lblOffset1.BackColor = btnSelect1.BackColor = Color.Lime; else lblOffset1.BackColor = btnSelect1.BackColor = SystemColors.Control;
            if (cmd == "G55") lblOffset2.BackColor = btnSelect2.BackColor = Color.Lime; else lblOffset2.BackColor = btnSelect2.BackColor = SystemColors.Control;
            if (cmd == "G56") lblOffset3.BackColor = btnSelect3.BackColor = Color.Lime; else lblOffset3.BackColor = btnSelect3.BackColor = SystemColors.Control;
            if (cmd == "G57") lblOffset4.BackColor = btnSelect4.BackColor = Color.Lime; else lblOffset4.BackColor = btnSelect4.BackColor = SystemColors.Control;
            if (cmd == "G58") lblOffset5.BackColor = btnSelect5.BackColor = Color.Lime; else lblOffset5.BackColor = btnSelect5.BackColor = SystemColors.Control;
            if (cmd == "G59") lblOffset6.BackColor = btnSelect6.BackColor = Color.Lime; else lblOffset6.BackColor = btnSelect6.BackColor = SystemColors.Control;
        }

        int delay = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {   if (delay > 0)
                delay--;
            else
            {   showValues();
                timer1.Stop();
            }
        }

        public void showValues()
        {   lblOffset1.Text = grbl.displayCoord("G54");
            lblOffset2.Text = grbl.displayCoord("G55");
            lblOffset3.Text = grbl.displayCoord("G56");
            lblOffset4.Text = grbl.displayCoord("G57");
            lblOffset5.Text = grbl.displayCoord("G58");
            lblOffset6.Text = grbl.displayCoord("G59");
            lblG28.Text = grbl.displayCoord("G28");
            lblG30.Text = grbl.displayCoord("G30");
            lblG92.Text = grbl.displayCoord("G92");
            lblPRB.Text = grbl.displayCoord("PRB");
            if (grbl.getPRBStatus() == true)
                lblPRB.BackColor = Color.Lime;
            else
                lblPRB.BackColor = SystemColors.Control;

            lblTLO.Text = grbl.displayCoord("TLO");

            resizeForm(grbl.axisCount-3);
        }

        private void btnSelect1_Click(object sender, EventArgs e) { sendCmd("G54"); }
        private void btnSelect2_Click(object sender, EventArgs e) { sendCmd("G55"); }
        private void btnSelect3_Click(object sender, EventArgs e) { sendCmd("G56"); }
        private void btnSelect4_Click(object sender, EventArgs e) { sendCmd("G57"); }
        private void btnSelect5_Click(object sender, EventArgs e) { sendCmd("G58"); }
        private void btnSelect6_Click(object sender, EventArgs e) { sendCmd("G59"); }

        private void sendCmd(string cmd)
        {   markActiveCoordSystem(cmd);
            sendCommandEvent(new CmdEventArgs(cmd)); 
        }
        private void btnSet1_Click(object sender, EventArgs e) { setCoord(1,grbl.posWork); }
        private void btnSet2_Click(object sender, EventArgs e) { setCoord(2,grbl.posWork); }
        private void btnSet3_Click(object sender, EventArgs e) { setCoord(3,grbl.posWork); }
        private void btnSet4_Click(object sender, EventArgs e) { setCoord(4,grbl.posWork); }
        private void btnSet5_Click(object sender, EventArgs e) { setCoord(5,grbl.posWork); }
        private void btnSet6_Click(object sender, EventArgs e) { setCoord(6,grbl.posWork); }

        private void btnSetM1_Click(object sender, EventArgs e) { setCoord(1, grbl.posMarker); }
        private void btnSetM2_Click(object sender, EventArgs e) { setCoord(2, grbl.posMarker); }
        private void btnSetM3_Click(object sender, EventArgs e) { setCoord(3, grbl.posMarker); }
        private void btnSetM4_Click(object sender, EventArgs e) { setCoord(4, grbl.posMarker); }
        private void btnSetM5_Click(object sender, EventArgs e) { setCoord(5, grbl.posMarker); }
        private void btnSetM6_Click(object sender, EventArgs e) { setCoord(6, grbl.posMarker); }

        private void btnSetC1_Click(object sender, EventArgs e) { setCoord(1); }
        private void btnSetC2_Click(object sender, EventArgs e) { setCoord(2); }
        private void btnSetC3_Click(object sender, EventArgs e) { setCoord(3); }
        private void btnSetC4_Click(object sender, EventArgs e) { setCoord(4); }
        private void btnSetC5_Click(object sender, EventArgs e) { setCoord(5); }
        private void btnSetC6_Click(object sender, EventArgs e) { setCoord(6); }

        private void btnG28Move_Click(object sender, EventArgs e) { sendCommandEvent(new CmdEventArgs("G28")); }
        private void btnG28Set_Click(object sender, EventArgs e) { sendCommandEvent(new CmdEventArgs("G28.1")); refreshValues(); }
        private void btnG30Move_Click(object sender, EventArgs e) { sendCommandEvent(new CmdEventArgs("G30")); }
        private void btnG30Set_Click(object sender, EventArgs e) { sendCommandEvent(new CmdEventArgs("G30.1")); refreshValues(); }
        private void btnG92Off_Click(object sender, EventArgs e) { sendCommandEvent(new CmdEventArgs("G92.1")); refreshValues(); }
        private void btnG43_Click(object sender, EventArgs e)
        {   lblTLO.BackColor = Color.Lime;
            sendCommandEvent(new CmdEventArgs(string.Format("G43.1 Z{0:0.000}", grbl.posWork.Z))); refreshValues();
        }
        private void btnG49_Click(object sender, EventArgs e)
        {   lblTLO.BackColor = SystemColors.Control;
            sendCommandEvent(new CmdEventArgs("G49")); refreshValues();
        }

        private void btnUpdate_Click(object sender, EventArgs e) { refreshValues(); }

        private void setCoord(int nr, xyPoint tmp)
        {   string cmd = String.Format("G10 L2 P{0} X{1:0.000} Y{2:0.000}", nr, tmp.X, tmp.Y);
            sendCommandEvent(new CmdEventArgs(cmd.Replace(',', '.')));
//            refreshValues();
        }    

        private void setCoord(int nr, xyzPoint tmp=new xyzPoint())
        {   string cmd = String.Format("G10 L2 P{0} X{1:0.000} Y{2:0.000} Z{3:0.000}", nr, tmp.X, tmp.Y, tmp.Z);
            if (grbl.axisA) cmd = String.Format("{0} A{1:0.000}", cmd, tmp.A);
            if (grbl.axisB) cmd = String.Format("{0} B{1:0.000}", cmd, tmp.B);
            if (grbl.axisC) cmd = String.Format("{0} C{1:0.000}", cmd, tmp.C);
            sendCommandEvent(new CmdEventArgs(cmd.Replace(',', '.')));
//            refreshValues();
        }

        public void refreshValues(bool init=false)
        {   sendCommandEvent(new CmdEventArgs("$#"));
            delay = 5;
            timer1.Enabled = true;
            timer1.Start();
            if (init)
                sendCommandEvent(new CmdEventArgs("$G"));
        }

        private void resizeForm(int add)
        {   if (add < 0) add = 0;
            if (add > 3) add = 3;
            int newWidth = 70 * add;
            Width = 470 + newWidth;
            gB_offset.Width = 200 + newWidth;
            gB_G28.Width = gB_G38.Width = gB_G92.Width = 446 + newWidth;
        }

        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void sendCommandEvent(CmdEventArgs e)
        {
            EventHandler<CmdEventArgs> handler = RaiseCmdEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

}
