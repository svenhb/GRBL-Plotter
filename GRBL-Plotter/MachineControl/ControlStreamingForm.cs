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

#pragma warning disable CA1305

namespace GrblPlotter
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
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

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
        private void NudOverrideFRTop_ValueChanged(object sender, EventArgs e)
        { tBOverrideFR.Maximum = (int)nUDOverrideFRTop.Value; }
        private void NudOverrideFRBtm_ValueChanged(object sender, EventArgs e)
        { tBOverrideFR.Minimum = (int)nUDOverrideFRBtm.Value; }
        private void NudOverrideSSTop_ValueChanged(object sender, EventArgs e)
        { tBOverrideSS.Maximum = (int)nUDOverrideSSTop.Value; }
        private void NudOverrideSSBtm_ValueChanged(object sender, EventArgs e)
        { tBOverrideSS.Minimum = (int)nUDOverrideSSBtm.Value; }

        public void TbOverrideFRScroll(object sender, EventArgs e)
        {
            lblOverrideFRValue.Text = tBOverrideFR.Value.ToString();
        }

        public void TbOverrideSSScroll(object sender, EventArgs e)
        {
            lblOverrideSSValue.Text = tBOverrideSS.Value.ToString();
        }

        public void ShowValueFR(string val)
        { lblFRValue.Text = val; }
        public void ShowValueSS(string val)
        { lblSSValue.Text = val; }

        private void TbOverrideFR_MouseUp(object sender, MouseEventArgs e)
        {
            if (cBOverrideFREnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(OverrideSource.feedRate, (float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void TbOverrideFR_KeyUp(object sender, KeyEventArgs e)
        {
            if (cBOverrideFREnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(OverrideSource.feedRate, (float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void TbOverrideSS_MouseUp(object sender, MouseEventArgs e)
        {
            if (cBOverrideSSEnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(OverrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }
        private void TbOverrideSS_KeyUp(object sender, KeyEventArgs e)
        {
            if (cBOverrideSSEnable.Checked)
                OnRaiseOverrideEvent(new OverrideEventArgs(OverrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }

        private void CbOverrideFREnable_CheckedChanged(object sender, EventArgs e)
        {
            OnRaiseOverrideEvent(new OverrideEventArgs(OverrideSource.feedRate, (float)tBOverrideFR.Value, cBOverrideFREnable.Checked));
        }
        private void CbOverrideSSEnable_CheckedChanged(object sender, EventArgs e)
        {
            OnRaiseOverrideEvent(new OverrideEventArgs(OverrideSource.spindleSpeed, (float)tBOverrideSS.Value, cBOverrideSSEnable.Checked));
        }

        public event EventHandler<OverrideEventArgs> RaiseOverrideEvent;
        protected virtual void OnRaiseOverrideEvent(OverrideEventArgs e)
        {
            RaiseOverrideEvent?.Invoke(this, e);
            /*    EventHandler<OverrideEventArgs> handler = RaiseOverrideEvent;
                if (handler != null)
                {
                    handler(this, e);
                }*/
        }

        private void ControlStreamingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationStreamForm = Location;
        }
    }
    public enum OverrideSource { spindleSpeed, feedRate };

    public class OverrideEventArgs : EventArgs
    {
        private readonly OverrideSource source;
        private readonly float value;
        private readonly bool enable;
        public OverrideEventArgs(OverrideSource inSource, float inValue, bool inEnable)
        {
            source = inSource;
            value = inValue;
            enable = inEnable;
        }
        public OverrideSource Source
        { get { return source; } }
        public float Value
        { get { return value; } }
        public bool Enable
        { get { return enable; } }
    }

}
