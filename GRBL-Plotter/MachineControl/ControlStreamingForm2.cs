/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2017 Sven Hasemann contact: svenhb@web.de

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
using System.Windows.Forms;
using System.Drawing;

namespace GRBL_Plotter
{
    public partial class ControlStreamingForm2 : Form
    {
        public ControlStreamingForm2()
        {
            InitializeComponent();
        }
        public void showOverrideValues(string val)  // get values from Ov Message
        {
            string[] value = val.Split(',');
            if (value.Length > 2)
            {
                lblOverrideFRValue.Text = value[0];
                lblOverrideSSValue.Text = value[2];
            }
        }
        public void showActualValues(string val)    // get values from FS Message
        {
            string[] value = val.Split(',');
            if (value.Length > 1)
            {
                lblFRValue.Text = value[0];
                lblSSValue.Text = value[1];
            }
        }

        private void btnOverrideFR0_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(144)); }     // 0x90 : Set 100% of programmed rate.    
        private void btnOverrideFR1_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(145)); }     // 0x91 : Increase 10%        
        private void btnOverrideFR4_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(146)); }     // 0x92 : Decrease 10%   
        private void btnOverrideFR2_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(147)); }     // 0x93 : Increase 1%   
        private void btnOverrideFR3_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(148)); }     // 0x94 : Decrease 1%   

        private void btnOverrideSS0_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(153)); }     // 0x99 : Set 100% of programmed spindle speed    
        private void btnOverrideSS1_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(154)); }     // 0x9A : Increase 10%        
        private void btnOverrideSS4_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(155)); }     // 0x9B : Decrease 10%   
        private void btnOverrideSS2_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(156)); }     // 0x9C : Increase 1%   
        private void btnOverrideSS3_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(157)); }     // 0x9D : Decrease 1%   

        private void btnToggleSS_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(158)); }     // 0x9E : Toggle Spindle Stop
        private void btnToggleFC_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(160)); }     // 0xA0 : Toggle Flood Coolant
        private void btnToggleMC_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(161)); }     // 0xA1 : Toggle Mist Coolant

        private void btnOverrideSD_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgArgs(132)); }     // 

        public event EventHandler<OverrideMsgArgs> RaiseOverrideEvent;
        protected virtual void OnRaiseOverrideEvent(OverrideMsgArgs e)
        {
            EventHandler<OverrideMsgArgs> handler = RaiseOverrideEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ControlStreamingForm2_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationStreamForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }
        }

        private void ControlStreamingForm2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationStreamForm = Location;
        }
    }

    public class OverrideMsgArgs : EventArgs
    {
        private int Msg;
        public OverrideMsgArgs(int msg)
        {
            Msg=msg;
        }
        public int MSG
        { get { return Msg; } }
    }

}
