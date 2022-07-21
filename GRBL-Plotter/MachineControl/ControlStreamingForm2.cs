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
 * 2021-07-12 code clean up / code quality
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlStreamingForm2 : Form
    {
        public ControlStreamingForm2()
        {
            InitializeComponent();
        }
        public void ShowOverrideValues(string val)  // get values from Ov Message
        {
            if (!string.IsNullOrEmpty(val))
            {
                string[] value = val.Split(',');
                if (value.Length > 2)
                {
                    lblOverrideFRValue.Text = value[0];
                    lblOverrideSSValue.Text = value[2];
                }
            }
        }
        public void ShowActualValues(string val)    // get values from FS Message
        {
            if (!string.IsNullOrEmpty(val))
            {
                string[] value = val.Split(',');
                if (value.Length > 1)
                {
                    lblFRValue.Text = value[0];
                    lblSSValue.Text = value[1];
                }
            }
        }

        private void BtnOverrideFR0_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(144)); }     // 0x90 : Set 100% of programmed rate.    
        private void BtnOverrideFR1_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(145)); }     // 0x91 : Increase 10%        
        private void BtnOverrideFR4_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(146)); }     // 0x92 : Decrease 10%   
        private void BtnOverrideFR2_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(147)); }     // 0x93 : Increase 1%   
        private void BtnOverrideFR3_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(148)); }     // 0x94 : Decrease 1%   

        private void BtnOverrideSS0_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(153)); }     // 0x99 : Set 100% of programmed spindle speed    
        private void BtnOverrideSS1_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(154)); }     // 0x9A : Increase 10%        
        private void BtnOverrideSS4_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(155)); }     // 0x9B : Decrease 10%   
        private void BtnOverrideSS2_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(156)); }     // 0x9C : Increase 1%   
        private void BtnOverrideSS3_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(157)); }     // 0x9D : Decrease 1%   

        private void BtnToggleSS_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(158)); }     // 0x9E : Toggle Spindle Stop
        private void BtnToggleFC_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(160)); }     // 0xA0 : Toggle Flood Coolant
        private void BtnToggleMC_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(161)); }     // 0xA1 : Toggle Mist Coolant

        private void BtnOverrideSD_Click(object sender, EventArgs e)
        { OnRaiseOverrideEvent(new OverrideMsgEventArgs(132)); }     // 

        public event EventHandler<OverrideMsgEventArgs> RaiseOverrideEvent;
        protected virtual void OnRaiseOverrideEvent(OverrideMsgEventArgs e)
        {
            RaiseOverrideEvent?.Invoke(this, e);
            /*     EventHandler<OverrideMsgEventArgs> handler = RaiseOverrideEvent;
                 if (handler != null)
                 {
                     handler(this, e);
                 }*/
        }

        private void ControlStreamingForm2_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationStreamForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }
        }

        private void ControlStreamingForm2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationStreamForm = Location;
        }
    }

    public class OverrideMsgEventArgs : EventArgs
    {
        private readonly int Msg;
        public OverrideMsgEventArgs(int msg)
        {
            Msg = msg;
        }
        public int MSG
        { get { return Msg; } }
    }

}
