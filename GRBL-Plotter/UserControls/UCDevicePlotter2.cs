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
    public partial class UCDevicePlotter2 : UserControl
    {
        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }
        public UCDevicePlotter2()
        {
            InitializeComponent();
        }

        private void update()
        {
            toolTip1.SetToolTip(BtnPenUp, string.Format("send 'M3 S{0}'", Properties.Settings.Default.importGCPWMUp));
            toolTip1.SetToolTip(BtnPenDown, string.Format("send 'M3 S{0}'", Properties.Settings.Default.importGCPWMDown));
        }

        private void BtnPenUp_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DevicePlotterControlIndex==0)
            //if (Properties.ListSettings.Default.importGCPWMEnable)  // to avoid error 9, do jog command at last
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMUp).Replace(",", "."), 0, sender, e));
            }
            else
        //    if (Properties.ListSettings.Default.importGCZEnable)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("$J=G90 Z{0} F{1}", Gcode.FrmtNum(Properties.Settings.Default.DevicePlotterZUp), Properties.Settings.Default.DevicePlotterSpeedZ).Replace(",", "."), 0, sender, e));
            }
        }

        private void BtnPenZero_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DevicePlotterControlIndex == 0)
            //    if (Properties.ListSettings.Default.importGCPWMEnable)  // to avoid error 9, do jog command at last
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMZero).Replace(",", "."), 0, sender, e));
            }
            else
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("$J=G90 Z{0} F{1}", Gcode.FrmtNum(0f), Properties.Settings.Default.DevicePlotterSpeedZ).Replace(",", "."), 0, sender, e));
            }
        }

        private void BtnPenDown_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DevicePlotterControlIndex == 0)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMDown).Replace(",", "."), 0, sender, e));
            }
            else
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("$J=G90 Z{0} F{1}", Gcode.FrmtNum(Properties.Settings.Default.DevicePlotterZDown), Properties.Settings.Default.DevicePlotterSpeedZ).Replace(",", "."), 0, sender, e));
            }
        }

        private void BtnPenDownUp_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DevicePlotterControlIndex == 0)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMDown).Replace(",", "."), 0, sender, e));
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G4 P{0}", Properties.Settings.Default.importGCPWMDlyDown).Replace(",", "."), 0, sender, e));
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMUp).Replace(",", "."), 0, sender, e));
            }
            else
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("$J=G90 Z{0} F{1}", Gcode.FrmtNum(Properties.Settings.Default.DevicePlotterZDown), Properties.Settings.Default.DevicePlotterSpeedZ).Replace(",", "."), 0, sender, e));
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("$J=G90 Z{0} F{1}", Gcode.FrmtNum(Properties.Settings.Default.DevicePlotterZUp), Properties.Settings.Default.DevicePlotterSpeedZ).Replace(",", "."), 0, sender, e));
            }
        }

        private void UCDevicePlotter2_BackColorChanged(object sender, EventArgs e)
        {
            MyControl.ChangeColor(this);
            MyControl.ChangeColor(TableLayoutPanel1);
        }
    }
}
