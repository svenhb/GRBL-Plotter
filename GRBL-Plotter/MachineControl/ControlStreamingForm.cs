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
    public partial class ControlStreamingForm : Form
    {
        public ControlStreamingForm()
        {
            InitializeComponent();
        }

        private void StreamingForm_Load(object sender, EventArgs e)
        {
            //      SetRange_ValueChanged(sender, e);
            Location = Properties.Settings.Default.locationStreamForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }

            tBOverrideFR.Minimum = (int)nUDOverrideFRBtm.Value;
            tBOverrideFR.Maximum = (int)nUDOverrideFRTop.Value;
            tBOverrideSS.Minimum = (int)nUDOverrideSSBtm.Value;
            tBOverrideSS.Maximum = (int)nUDOverrideSSTop.Value;
            lblOverrideFRValue.Text = tBOverrideFR.Value.ToString();
            lblOverrideSSValue.Text = tBOverrideSS.Value.ToString();
        }
        private void SetRange_ValueChanged(object sender, EventArgs e)
        {
            lblOverrideFRValue.Text = tBOverrideFR.Value.ToString();
            lblOverrideSSValue.Text = tBOverrideSS.Value.ToString();
        }
        private void nUDOverrideFRTop_ValueChanged(object sender, EventArgs e)
        {   tBOverrideFR.Maximum = (int)nUDOverrideFRTop.Value;  }
        private void nUDOverrideFRBtm_ValueChanged(object sender, EventArgs e)
        {   tBOverrideFR.Minimum = (int)nUDOverrideFRBtm.Value;  }
        private void nUDOverrideSSTop_ValueChanged(object sender, EventArgs e)
        {   tBOverrideSS.Maximum = (int)nUDOverrideSSTop.Value;  }
        private void nUDOverrideSSBtm_ValueChanged(object sender, EventArgs e)
        {   tBOverrideSS.Minimum = (int)nUDOverrideSSBtm.Value;  }

        public void tBOverrideFR_Scroll(object sender, EventArgs e)
        {
            lblOverrideFRValue.Text = tBOverrideFR.Value.ToString();
        }

        public void tBOverrideSS_Scroll(object sender, EventArgs e)
        {
            lblOverrideSSValue.Text = tBOverrideSS.Value.ToString();
        }

        public void show_value_FR(string val)
        { lblFRValue.Text = val; }
        public void show_value_SS(string val)
        { lblSSValue.Text = val; }

        private void tBOverrideFR_MouseUp(object sender, MouseEventArgs e)
        {
            if (cBOverrideFREnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.feedRate, (float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void tBOverrideFR_KeyUp(object sender, KeyEventArgs e)
        {
            if (cBOverrideFREnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.feedRate, (float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void tBOverrideSS_MouseUp(object sender, MouseEventArgs e)
        {
            if (cBOverrideSSEnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }
        private void tBOverrideSS_KeyUp(object sender, KeyEventArgs e)
        {
            if (cBOverrideSSEnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }

        private void cBOverrideFREnable_CheckedChanged(object sender, EventArgs e)
        {
            OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.feedRate,(float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void cBOverrideSSEnable_CheckedChanged(object sender, EventArgs e)
        {
            OnRaiseOverrideEvent(new OverrideEventArgs(overrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }

        public event EventHandler<OverrideEventArgs> RaiseOverrideEvent;
        protected virtual void OnRaiseOverrideEvent(OverrideEventArgs e)
        {
            EventHandler<OverrideEventArgs> handler = RaiseOverrideEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ControlStreamingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationStreamForm = Location;
        }
    }
    public enum overrideSource { spindleSpeed, feedRate};

    public class OverrideEventArgs : EventArgs
    {
        private overrideSource source;
        private float value;
        private bool enable;
        public OverrideEventArgs(overrideSource in_source, float in_value, bool in_enable)
        {
            source = in_source;
            value = in_value;
            enable = in_enable;
        }
        public overrideSource Source
        { get { return source; } }
        public float Value
        { get { return value; } }
        public bool Enable
        { get { return enable; } }
    }

}
