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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class ControlCoordSystem : Form
    {
        public ControlCoordSystem()
        {
            InitializeComponent();
        }
        private void ControlCoordSystem_Load(object sender, EventArgs e)
        {   refreshValues();
        }

        public void markBtn(string cmd)
        {   if (cmd == "G54") btnSelect1.BackColor = Color.Lime; else btnSelect1.BackColor = SystemColors.Control;
            if (cmd == "G55") btnSelect2.BackColor = Color.Lime; else btnSelect2.BackColor = SystemColors.Control;
            if (cmd == "G56") btnSelect3.BackColor = Color.Lime; else btnSelect3.BackColor = SystemColors.Control;
            if (cmd == "G57") btnSelect4.BackColor = Color.Lime; else btnSelect4.BackColor = SystemColors.Control;
            if (cmd == "G58") btnSelect5.BackColor = Color.Lime; else btnSelect5.BackColor = SystemColors.Control;
            if (cmd == "G59") btnSelect6.BackColor = Color.Lime; else btnSelect6.BackColor = SystemColors.Control;

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
        }

        private void btnSelect1_Click(object sender, EventArgs e) { sendCmd("G54"); }
        private void btnSelect2_Click(object sender, EventArgs e) { sendCmd("G55"); }
        private void btnSelect3_Click(object sender, EventArgs e) { sendCmd("G56"); }
        private void btnSelect4_Click(object sender, EventArgs e) { sendCmd("G57"); }
        private void btnSelect5_Click(object sender, EventArgs e) { sendCmd("G58"); }
        private void btnSelect6_Click(object sender, EventArgs e) { sendCmd("G59"); }

        private void sendCmd(string cmd)
        {   markBtn(cmd);
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

        private void btnUpdate_Click(object sender, EventArgs e) { refreshValues(); }

        private void setCoord(int nr, xyPoint tmp)
        {   string cmd = String.Format("G10 L2 P{0} X{1:0.000} Y{2:0.000}", nr, tmp.X, tmp.Y);
            sendCommandEvent(new CmdEventArgs(cmd));
            refreshValues();
        }    

        private void setCoord(int nr, xyzPoint tmp=new xyzPoint())
        {   string cmd = String.Format("G10 L2 P{0} X{1:0.000} Y{2:0.000} Z{3:0.000}", nr, tmp.X, tmp.Y, tmp.Z);
            sendCommandEvent(new CmdEventArgs(cmd));
            refreshValues();
        }

        public void refreshValues()
        {   sendCommandEvent(new CmdEventArgs("$#"));
            delay = 5;
            timer1.Enabled = true;
            timer1.Start();
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
    public class CmdEventArgs : EventArgs
    {
        string command;
        public CmdEventArgs(string cmd)
        {
            command = cmd;
        }
        public string Command
        { get { return command; } }
    }

}
