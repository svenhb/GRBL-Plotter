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

using System;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCDeviceRouter2 : UserControl
    {
        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public UCDeviceRouter2()
        {
            InitializeComponent();
        }

        public void SetStatusSpindle(bool on)
        {
            CbSpindle.CheckedChanged -= CbSpindle_CheckedChanged;
            CbSpindle.Checked = on;
            CbSpindle.CheckedChanged += CbSpindle_CheckedChanged;
        }
        public void SetStatusMist(bool on)
        {
            CbMist.CheckedChanged -= CbMist_CheckedChanged;
            CbCoolant.CheckedChanged -= CbCoolant_CheckedChanged;
            CbMist.Checked = on;
            CbMist.CheckedChanged += CbMist_CheckedChanged;
            CbCoolant.CheckedChanged += CbCoolant_CheckedChanged;
        }
        public void SetStatusFlood(bool on)
        {
            CbMist.CheckedChanged -= CbMist_CheckedChanged;
            CbCoolant.CheckedChanged -= CbCoolant_CheckedChanged;
            CbCoolant.Checked = on;
            CbMist.CheckedChanged += CbMist_CheckedChanged;
            CbCoolant.CheckedChanged += CbCoolant_CheckedChanged;
        }
        private void CbSpindle_CheckedChanged(object sender, EventArgs e)
        {
            if (CbSpindle.Checked)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3S{0:0}", Properties.Settings.Default.DeviceRouterSpindle).Replace(",", "."), 0, sender, e));
            else
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M5").Replace(",", "."), 0, sender, e));
        }

        private void CbCoolant_CheckedChanged(object sender, EventArgs e)
        {
            if (CbCoolant.Checked)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M8").Replace(",", "."), 0, sender, e));
            else
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M9").Replace(",", "."), 0, sender, e));
        }

        private void CbMist_CheckedChanged(object sender, EventArgs e)
        {
            if (CbMist.Checked)
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M7").Replace(",", "."), 0, sender, e));
            else
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M9").Replace(",", "."), 0, sender, e));
        }

        private void UCDeviceRouter2_BackColorChanged(object sender, EventArgs e)
        {
            MyControl.ChangeColor(this);
            MyControl.ChangeColor(TableLayoutPanel1);
        }
    }
}
