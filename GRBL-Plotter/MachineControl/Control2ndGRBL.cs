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
 * 2019-08-15 add logger
 * 2021-07-26 code clean up / code quality
 * 2026-04-09 GUI rework for vers. 1.8.0.0
 * 2026-04-09 use new UCJogControlUD instead of VirtualJoystick
*/

using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class Control2ndGRBL : Form
    {
        ControlSerialForm _serial_form2;
        private XyzPoint posMachine = new XyzPoint(0, 0, 0);
        private XyzPoint posWorld = new XyzPoint(0, 0, 0);

        private GrblState machineStatus;
        private readonly double[] joystickXYStep = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickZStep = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickXYSpeed = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickZSpeed = { 0, 1, 2, 3, 4, 5 };

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Control2ndGRBL(ControlSerialForm handle = null)
        {
            Logger.Trace("++++++ Control2ndGRBL START ++++++");
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            Set2ndSerial(handle);
        }

        public void Set2ndSerial(ControlSerialForm handle)
        {
            _serial_form2 = handle;
            if (handle != null)
            {
                _serial_form2.RaisePosEvent += OnRaisePosEvent;
                Logger.Trace("add handler RaisePosEvent");
            }
        }

        // send command via serial form
        private void SendRealtimeCommand(byte cmd)
        {
            if (_serial_form2.SerialPortOpen)
                _serial_form2.RealtimeCommand(cmd);
        }

        // send command via serial form
        private void SendCommand(string txt, bool jogging = false)
        {
            if (_serial_form2.SerialPortOpen)
            {
                if ((jogging) && (_serial_form2.IsGrblVers0 == false))
                    txt = "$J=" + txt;
                _serial_form2.RequestSend(txt);
            }
        }

        // handle position events from serial form
        private void OnRaisePosEvent(object sender, PosEventArgs e)
        {
            posWorld = e.PosWorld;
            posMachine = e.PosMachine;
            machineStatus = e.Status;

            label_mx.Text = string.Format("{0:0.000}", posMachine.X);
            label_my.Text = string.Format("{0:0.000}", posMachine.Y);
            label_mz.Text = string.Format("{0:0.000}", posMachine.Z);
            label_wx.Text = string.Format("{0:0.000}", posWorld.X);
            label_wy.Text = string.Format("{0:0.000}", posWorld.Y);
            label_wz.Text = string.Format("{0:0.000}", posWorld.Z);
            label_status.Text = Grbl.StatusToText(machineStatus);
            label_status.BackColor = Grbl.GrblStateColor(machineStatus);
        }

        private void BtnHome_Click(object sender, EventArgs e)
        { SendCommand("$H"); }
        private void BtnZeroX_Click(object sender, EventArgs e)
        { SendCommand("G92 X0"); }
        private void BtnZeroY_Click(object sender, EventArgs e)
        { SendCommand("G92 Y0"); }
        private void BtnZeroZ_Click(object sender, EventArgs e)
        { SendCommand("G92 Z0"); }
        private void BtnJogX_Click(object sender, EventArgs e)
        { SendCommand("G90 X0 F" + joystickXYSpeed[5].ToString(), true); }
        private void BtnJogY_Click(object sender, EventArgs e)
        { SendCommand("G90 Y0 F" + joystickXYSpeed[5].ToString(), true); }
        private void BtnJogZ_Click(object sender, EventArgs e)
        { SendCommand("G90 Z0 F" + joystickZSpeed[5].ToString(), true); }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            _serial_form2.GrblReset(true);  // savePos
        }
        private void BtnFeedHold_Click(object sender, EventArgs e)
        {
            SendRealtimeCommand((byte)'!');
        }
        private void BtnResume_Click(object sender, EventArgs e)
        {
            SendRealtimeCommand((byte)'~');
        }
        private void BtnKillAlarm_Click(object sender, EventArgs e)
        {
            SendCommand("$X");
        }

        private void Control2ndGRBL_Load(object sender, EventArgs e)
        {
            joystickXYStep[1] = (double)Properties.Settings.Default.guiJoystickXYStep1;
            joystickXYStep[2] = (double)Properties.Settings.Default.guiJoystickXYStep2;
            joystickXYStep[3] = (double)Properties.Settings.Default.guiJoystickXYStep3;
            joystickXYStep[4] = (double)Properties.Settings.Default.guiJoystickXYStep4;
            joystickXYStep[5] = (double)Properties.Settings.Default.guiJoystickXYStep5;
            joystickZStep[1] = (double)Properties.Settings.Default.guiJoystickZStep1;
            joystickZStep[2] = (double)Properties.Settings.Default.guiJoystickZStep2;
            joystickZStep[3] = (double)Properties.Settings.Default.guiJoystickZStep3;
            joystickZStep[4] = (double)Properties.Settings.Default.guiJoystickZStep4;
            joystickZStep[5] = (double)Properties.Settings.Default.guiJoystickZStep5;
            joystickXYSpeed[1] = (double)Properties.Settings.Default.guiJoystickXYSpeed1;
            joystickXYSpeed[2] = (double)Properties.Settings.Default.guiJoystickXYSpeed2;
            joystickXYSpeed[3] = (double)Properties.Settings.Default.guiJoystickXYSpeed3;
            joystickXYSpeed[4] = (double)Properties.Settings.Default.guiJoystickXYSpeed4;
            joystickXYSpeed[5] = (double)Properties.Settings.Default.guiJoystickXYSpeed5;
            joystickZSpeed[1] = (double)Properties.Settings.Default.guiJoystickZSpeed1;
            joystickZSpeed[2] = (double)Properties.Settings.Default.guiJoystickZSpeed2;
            joystickZSpeed[3] = (double)Properties.Settings.Default.guiJoystickZSpeed3;
            joystickZSpeed[4] = (double)Properties.Settings.Default.guiJoystickZSpeed4;
            joystickZSpeed[5] = (double)Properties.Settings.Default.guiJoystickZSpeed5;

            Location = Properties.Settings.Default.location2ndGRBLForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(50, 50); }

            bool useClassic = Properties.Settings.Default.UserControlJogControlShowButtons;
            ucJogControludX.SetApperance(useClassic);
            ucJogControludY.SetApperance(useClassic);
            ucJogControludZ.SetApperance(useClassic);
            ucJogControludX.SetAxisName("X");
            ucJogControludY.SetAxisName("Y");
            ucJogControludZ.SetAxisName("Z");
            ucJogControludX.SetEnabled(true);
            ucJogControludY.SetEnabled(true);
            ucJogControludZ.SetEnabled(true);
        }

        private void Control2ndGRBL_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ Control2ndGRBL Stop ++++++");
            Properties.Settings.Default.location2ndGRBLForm = Location;
        }

        private void ucJogControlUD1_RaiseCmdEvent(object sender, UserControls.UserControlCmdEventArgs e)
        {
            SendCommand(e.Command,false);
        }
    }
}
