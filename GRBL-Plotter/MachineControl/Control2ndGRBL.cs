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
using virtualJoystick;
using System.Globalization;
using System.Threading;
using System.Drawing;

namespace GRBL_Plotter
{
    public partial class Control2ndGRBL : Form
    {
        ControlSerialForm _serial_form2;
        private xyzPoint posMachine = new xyzPoint(0, 0, 0);
        private xyzPoint posWorld = new xyzPoint(0, 0, 0);
        private xyzPoint posProbe = new xyzPoint(0, 0, 0);
        private grblState machineStatus;
        private double[] joystickXYStep = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickZStep = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickXYSpeed = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickZSpeed = { 0, 1, 2, 3, 4, 5 };

        public Control2ndGRBL(ControlSerialForm handle = null)
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            set2ndSerial(handle);
        }

        public void set2ndSerial(ControlSerialForm handle = null)
        {   _serial_form2 = handle;
            if (handle != null)
                _serial_form2.RaisePosEvent += OnRaisePosEvent;
        }

        // send command via serial form
        private void sendRealtimeCommand(byte cmd)
        { if (_serial_form2.serialPortOpen)
                _serial_form2.realtimeCommand(cmd);
        }

        // send command via serial form
        private void sendCommand(string txt, bool jogging = false)
        {
            if (_serial_form2.serialPortOpen)
            {
                if ((jogging) && (_serial_form2.isGrblVers0 == false))
                    txt = "$J=" + txt;
                _serial_form2.requestSend(txt);
                btnJogStop.Visible = !_serial_form2.isGrblVers0;
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
            label_status.Text = grbl.statusToText(machineStatus);
            label_status.BackColor = grbl.grblStateColor(machineStatus);
        }

        private void btnHome_Click(object sender, EventArgs e)
        { sendCommand("$H"); }
        private void btnZeroX_Click(object sender, EventArgs e)
        { sendCommand("G92 X0"); }
        private void btnZeroY_Click(object sender, EventArgs e)
        { sendCommand("G92 Y0"); }
        private void btnZeroZ_Click(object sender, EventArgs e)
        { sendCommand("G92 Z0"); }
        private void btnJogX_Click(object sender, EventArgs e)
        { sendCommand("G90 X0 F" + joystickXYSpeed[5].ToString(), true); }
        private void btnJogY_Click(object sender, EventArgs e)
        { sendCommand("G90 Y0 F" + joystickXYSpeed[5].ToString(), true); }
        private void btnJogZ_Click(object sender, EventArgs e)
        { sendCommand("G90 Z0 F" + joystickZSpeed[5].ToString(), true); }
        private void btnJogStop_Click(object sender, EventArgs e)
        { sendRealtimeCommand(133); }    //0x85

        private void btnReset_Click(object sender, EventArgs e)
        {   _serial_form2.grblReset();
        }
        private void btnFeedHold_Click(object sender, EventArgs e)
        {   sendRealtimeCommand((byte)'!');
        }
        private void btnResume_Click(object sender, EventArgs e)
        {   sendRealtimeCommand((byte)'~');
        }
        private void btnKillAlarm_Click(object sender, EventArgs e)
        {   sendCommand("$X");
        }

        private void virtualJoystickXY_Enter(object sender, EventArgs e)
        { if (_serial_form2.isGrblVers0) sendCommand("G91G1F100"); }
        private void virtualJoystickXY_Leave(object sender, EventArgs e)
        { if (_serial_form2.isGrblVers0) sendCommand("G90"); }
        private void virtualJoystickX_JoyStickEvent(object sender, JogEventArgs e)
        {
            int indexZ = Math.Abs(e.JogPosY);
            int dirZ = Math.Sign(e.JogPosY);
            int speed = (int)joystickXYSpeed[indexZ];
            String strZ = gcode.frmtNum(joystickXYStep[indexZ] * dirZ);
            if (speed > 0)
            {
                String s = String.Format("G91 X{0} F{1}", strZ, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }
        private void virtualJoystickY_JoyStickEvent(object sender, JogEventArgs e)
        {
            int indexZ = Math.Abs(e.JogPosY);
            int dirZ = Math.Sign(e.JogPosY);
            int speed = (int)joystickXYSpeed[indexZ];
            String strZ = gcode.frmtNum(joystickXYStep[indexZ] * dirZ);
            if (speed > 0)
            {
                String s = String.Format("G91 Y{0} F{1}", strZ, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }
        private void virtualJoystickZ_JoyStickEvent(object sender, JogEventArgs e)
        {
            int indexZ = Math.Abs(e.JogPosY);
            int dirZ = Math.Sign(e.JogPosY);
            int speed = (int)joystickZSpeed[indexZ];
            String strZ = gcode.frmtNum(joystickZStep[indexZ] * dirZ);
            if (speed > 0)
            {
                String s = String.Format("G91 Z{0} F{1}", strZ, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }

        private void Control2ndGRBL_Load(object sender, EventArgs e)
        {
            joystickXYStep[1] = (double)Properties.Settings.Default.joyXYStep1;
            joystickXYStep[2] = (double)Properties.Settings.Default.joyXYStep2;
            joystickXYStep[3] = (double)Properties.Settings.Default.joyXYStep3;
            joystickXYStep[4] = (double)Properties.Settings.Default.joyXYStep4;
            joystickXYStep[5] = (double)Properties.Settings.Default.joyXYStep5;
            joystickZStep[1] = (double)Properties.Settings.Default.joyZStep1;
            joystickZStep[2] = (double)Properties.Settings.Default.joyZStep2;
            joystickZStep[3] = (double)Properties.Settings.Default.joyZStep3;
            joystickZStep[4] = (double)Properties.Settings.Default.joyZStep4;
            joystickZStep[5] = (double)Properties.Settings.Default.joyZStep5;
            joystickXYSpeed[1] = (double)Properties.Settings.Default.joyXYSpeed1;
            joystickXYSpeed[2] = (double)Properties.Settings.Default.joyXYSpeed2;
            joystickXYSpeed[3] = (double)Properties.Settings.Default.joyXYSpeed3;
            joystickXYSpeed[4] = (double)Properties.Settings.Default.joyXYSpeed4;
            joystickXYSpeed[5] = (double)Properties.Settings.Default.joyXYSpeed5;
            joystickZSpeed[1] = (double)Properties.Settings.Default.joyZSpeed1;
            joystickZSpeed[2] = (double)Properties.Settings.Default.joyZSpeed2;
            joystickZSpeed[3] = (double)Properties.Settings.Default.joyZSpeed3;
            joystickZSpeed[4] = (double)Properties.Settings.Default.joyZSpeed4;
            joystickZSpeed[5] = (double)Properties.Settings.Default.joyZSpeed5;
            virtualJoystickX.JoystickLabel = joystickXYStep;
            virtualJoystickY.JoystickLabel = joystickXYStep;
            virtualJoystickZ.JoystickLabel = joystickZStep;

            btnJogStop.Visible = !_serial_form2.isGrblVers0;

            Location = Properties.Settings.Default.location2ndGRBLForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(50, 50); }
        }

        private void Control2ndGRBL_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.location2ndGRBLForm = Location;
        }
    }
}
