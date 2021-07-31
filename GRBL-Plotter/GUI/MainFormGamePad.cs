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
 * 2020-03-11 split from MainForm.cs
 * 2021-02-06 add gamePad PointOfViewController0
 * 2021-07-26 code clean up / code quality
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

//#pragma warning disable CA1305
//#pragma warning disable CA1307

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        #region gamePad

        //        private bool gamePadSendCmd = false;
        private string gamePadSendString = "";
        //        private int gamePadRepitition = 0;
        private void GamePadTimer_Tick(object sender, EventArgs e)
        {   //if (true)
            ProcessGamePadNew();
            //            else
            //                processGamePadOld();
        }

        //        private static Stopwatch gamePadWatch = new Stopwatch();
        private Dictionary<string, int> gamePadValue = new Dictionary<string, int>();
        private void ProcessGamePadNew()
        {
            string command = "";
            try
            {
                if (ControlGamePad.gamePad != null)
                {
                    ControlGamePad.gamePad.Poll();
                    var datas = ControlGamePad.gamePad.GetBufferedData();

                    if (datas.Length > 0)   // no trigger if no change in data
                    {
                        bool doJog = false;
                        //   var property = Properties.Settings.Default;
                        foreach (var state in datas)
                        {
                            string offset = state.Offset.ToString();        // execute gPButtonsx strings
                            int value = state.Value;
                            if ((value > 0) && (offset.IndexOf("Buttons") >= 0))        // Buttons
                            {
                                try
                                {
                                    command = Properties.Settings.Default["gamePad" + offset].ToString();        // gP
                                    if (command.IndexOf('#') >= 0)
                                    { ProcessSpecialCommands(command); }
                                    else
                                    { ProcessCommands(command); }
                                }
                                catch (Exception Ex)
                                { Logger.Error(Ex, "ProcessGamePadNew "); throw; }
                                return;
                            }
                            else if (offset.IndexOf("PointOfViewControllers0") >= 0)
                            {
                                command = "";
                                if (value == 0) { command = Properties.Settings.Default.gamePadPOVC00; } // up
                                else if (value == 4500) { command = Properties.Settings.Default.gamePadPOVC01; } // up-right
                                else if (value == 9000) { command = Properties.Settings.Default.gamePadPOVC02; } // right
                                else if (value == 13500) { command = Properties.Settings.Default.gamePadPOVC03; } // down-right
                                else if (value == 18000) { command = Properties.Settings.Default.gamePadPOVC04; } // down
                                else if (value == 22500) { command = Properties.Settings.Default.gamePadPOVC05; } // down-left
                                else if (value == 27000) { command = Properties.Settings.Default.gamePadPOVC06; } // left
                                else if (value == 31500) { command = Properties.Settings.Default.gamePadPOVC07; } // up-left
                                if (command.IndexOf('#') >= 0)
                                { ProcessSpecialCommands(command); }
                                else
                                { ProcessCommands(command); }
                            }

                            else if ((offset == "X") || (offset == "Y") || (offset == "Z") || (offset == "RotationZ"))
                            {
                                doJog = true;
                                if (!gamePadValue.ContainsKey(offset))  // just keep latest value
                                    gamePadValue.Add(offset, value);
                                else
                                    gamePadValue[offset] = value;
                            }
                        }       // end foreach
                        if (doJog)
                        {   // execute analog joystick https://github.com/gnea/grbl/wiki/Grbl-v1.1-Jogging#joystick-implementation
                            gamePadSendString = "";
                            float maxSpeed = 0, maxValue = 0, minAxisSteps = 1000;   // max. feed rate on axis
                            int jval, jdir, invert = 1;
                            int center = 32767, deadzone = 1000;
                            center = (int)Properties.Settings.Default.gamePadAnalogOffset;
                            deadzone = (int)Properties.Settings.Default.gamePadAnalogDead;
                            string axis = "X";

                            foreach (var item in gamePadValue.Where(kvp => Math.Abs(kvp.Value - center) <= deadzone).ToList())
                            { gamePadValue.Remove(item.Key); }            // if value within deadzone, nothing to do

                            foreach (string key in gamePadValue.Keys)       // find maxima, keep values as long as they are not < limit
                            {
                                jval = Math.Abs(gamePadValue[key] - center);
                                if ((gamePadValue[key] <= 0) || (gamePadValue[key] >= 65535))
                                    jval = 32767;
                                maxValue = Math.Max(maxValue, jval);
                                axis = GamePadGetAssignedAxis(key);
                                maxSpeed = Math.Max(maxSpeed, Grbl.GetSetting(110 + GetGrblSetupOffset(axis)));
                                minAxisSteps = Math.Min(minAxisSteps, Grbl.GetSetting(100 + GetGrblSetupOffset(axis)));
                            }

                            if (maxValue > deadzone)            // send move-command if any value is > limit; otherwise send jog-cancel
                            {
                                float feedRate = maxSpeed * (maxValue - deadzone) / (32767f - deadzone);
                                float minFeedRate = 1800 / minAxisSteps;
                                if (feedRate < minFeedRate)
                                    feedRate = 1.1f * 1800 / minAxisSteps;      // 10% above minimum speed
                                float stepWidth;
                                bool justOneAxis = (gamePadValue.Keys.Count <= 1);
                                foreach (string key in gamePadValue.Keys)
                                {
                                    jdir = Math.Sign(gamePadValue[key] - center);
                                    jval = Math.Abs(gamePadValue[key] - center);
                                    if ((gamePadValue[key] <= 0) || (gamePadValue[key] >= 65535))
                                        jval = 32767;
                                    axis = GamePadGetAssignedAxis(key);
                                    invert = GamePadGetAssignedDir(key);
                                    // step width must be a bit longer to avoid finishing move within timer-interval
                                    stepWidth = 1.2f * feedRate * jdir * invert * gamePadTimer.Interval / 60000; // s = v * dt   20% longer
                                    stepWidth *= jval / maxValue;
                                    if (justOneAxis && (jval <= (int)Properties.Settings.Default.gamePadAnalogMinimum))
                                    {
                                        gamePadSendString += string.Format("{0}{1:0.000}", axis, Properties.Settings.Default.gamePadAnalogMinStep);
                                        feedRate = (float)Properties.Settings.Default.gamePadAnalogMinFeed;
                                        gamePadTimer.Interval = 200;
                                    }
                                    else
                                    {
                                        gamePadSendString += string.Format("{0}{1:0.000}", axis, stepWidth);
                                        gamePadTimer.Interval = 50;
                                    }
                                }

                                if (_serial_form.GetFreeBuffer() >= 99)
                                {
                                    gamePadSendString = "G91" + gamePadSendString;
                                    if (Grbl.isMarlin) gamePadSendString += ";";
                                    gamePadSendString += string.Format("F{0:0}", feedRate);
                                    SendCommands(gamePadSendString.Replace(",", "."), true);
                                }
                            }
                            else
                            {
                                SendRealtimeCommand(133); // Stop jogging
                                gamePadSendString = "";
                            }
                        }
                    }   // end if datalength
                    else
                    {   // if (datas.Length > 0)
                        if ((gamePadSendString.Length > 0) && (_serial_form.GetFreeBuffer() >= 99))     // keep sending commands if joystick is still on full speed
                        { SendCommands(gamePadSendString.Replace(",", "."), true); }
                    }
                }
                else
                {   // if (ControlGamePad.gamePad == null)
                    try { ControlGamePad.Initialize(); gamePadTimer.Interval = 50; }
                    catch { gamePadTimer.Interval = 5000; }
                }
            }
            catch   // ControlGamePad.gamePad failed
            {
                try { ControlGamePad.Initialize(); gamePadTimer.Interval = 50; }
                catch { gamePadTimer.Interval = 5000; }
            }
        }
        private static string GamePadGetAssignedAxis(string offset)
        {
            var prop = Properties.Settings.Default;
            if (offset == "X") { return prop.gamePadXAxis; }
            if (offset == "Y") { return prop.gamePadYAxis; }
            if (offset == "Z") { return prop.gamePadZAxis; }
            if (offset == "RotationZ") { return prop.gamePadRAxis; }
            return "X";
        }
        private static int GamePadGetAssignedDir(string offset)
        {
            var prop = Properties.Settings.Default;
            if (offset == "X") { return prop.gamePadXInvert ? -1 : 1; }
            if (offset == "Y") { return prop.gamePadYInvert ? -1 : 1; }
            if (offset == "Z") { return prop.gamePadZInvert ? -1 : 1; }
            if (offset == "RotationZ") { return prop.gamePadRInvert ? -1 : 1; }
            return 1;
        }
        private static int GetGrblSetupOffset(string axis)
        {
            if (axis == "X") return 0;
            if (axis == "Y") return 1;
            if (axis == "Z") return 2;
            if (axis == "A") return 3;
            if (axis == "B") return 4;
            if (axis == "C") return 5;
            return 0;
        }
        /*
        private void processGamePadOld()
        {   string command = "";
            try
            {
                if (ControlGamePad.gamePad != null)
                {
                    ControlGamePad.gamePad.Poll();
                    var datas = ControlGamePad.gamePad.GetBufferedData();
                    int stepIndex = 0, feed = 10000;
                    string cmdX = "", cmdY = "", cmdZ = "", cmdR = "", cmd = "";
                    bool stopJog = false;
                    var prop = Properties.Settings.Default;

                    gamePadRepitition++;
                    if (gamePadRepitition > 4) { gamePadRepitition = 0; }

                    if (datas.Length > 0)
                    {
                        cmd = "G91";
                        foreach (var state in datas)
                        {
                            string offset = state.Offset.ToString();        // execute gPButtonsx strings
                            int value = state.Value;
                            if ((value > 0) && (offset.IndexOf("Buttons") >= 0))        // Buttons
                            {
                                try
                                {
                                    command = Properties.Settings.Default["gamePad" + offset].ToString();        // gP
                                    if (command.IndexOf('#') >= 0)
                                    { processSpecialCommands(command); }
                                    else
                                    { processCommands(command); }
                                }
                                catch (Exception)
								{ }
                            }

                            // execute analog joystick
                            if ((offset == "X") || (offset == "Y") || (offset == "Z") || (offset == "RotationZ"))
                            {
                                // center position = 32767 (7FFF), range = 0 - 32767
                                int jval = Math.Abs(value - 32767);
                                int jdir = Math.Sign(value - 32767);

                                if ((value > 28000) && (value < 36000))
                                {
                                    sendRealtimeCommand(133); stopJog = true;
                                    gamePadSendCmd = false;
                                    gamePadSendString = "";
                                }
                                else
                                {
                                    stepIndex = gamePadIndex(value);// absVal) / 6500;

                                    if (offset == "X")
                                    {
                                        gamePadSendCmd = true;
                                        cmdX = gamePadGCode(value, stepIndex, prop.gamePadXAxis, prop.gamePadXInvert, out feed);    // refresh axis data
                                    }
                                    if (offset == "Y")
                                    {
                                        gamePadSendCmd = true;
                                        cmdY = gamePadGCode(value, stepIndex, prop.gamePadYAxis, prop.gamePadYInvert, out feed);    // refresh axis data
                                    }
                                    if (offset == "Z")
                                    {
                                        gamePadSendCmd = true;
                                        cmdZ = gamePadGCode(value, stepIndex, prop.gamePadZAxis, prop.gamePadZInvert, out feed);    // refresh axis data
                                    }
                                    if (offset == "RotationZ")
                                    {
                                        gamePadSendCmd = true;
                                        cmdR = gamePadGCode(value, stepIndex, prop.gamePadRAxis, prop.gamePadRInvert, out feed);    // refresh axis data
                                    }
                                }
                            }
                            else
                            {
                                gamePadSendCmd = false;
                                gamePadSendString = "";
                            }
                        }
                        cmd += cmdX + cmdY + cmdZ + cmdR;               // build up command word with last axis information
                        if (cmd.Length > 4)
                            gamePadSendString = cmd + "F" + feed;
                    }

                    if (gamePadSendCmd && !stopJog && gamePadRepitition == 0)   // send in 500 ms raster
                    {
                        if (gamePadSendString.Length > 0)
                            sendCommand(gamePadSendString.Replace(",","."), true);
                    }
                }
                else
                {   try { ControlGamePad.Initialize(); gamePadTimer.Interval = 100; }
                    catch { gamePadTimer.Interval = 5000; }
                }
            }
            catch
            {
                try { ControlGamePad.Initialize(); gamePadTimer.Interval = 100; }
                catch { gamePadTimer.Interval = 5000; }
            }
        }

        private int gamePadIndex(int value)         // calculate matching index for virtual joystick values
        {
            int absval = Math.Abs(value - 32767);   // depending on joystick position (strange behavior)
            if (absval < 5000) { return 0; }
            if (absval < 12000) { return 1; }
            if (absval < 19000) { return 2; }
            if (absval < 26000) { return 3; }
            if (absval < 32700) { return 4; }
            if (absval >= 32700) { return 5; }
            return 0;
        }

        private string gamePadGCode(int value, int stpIndex, string axis, bool invert, out int speed)
        {
            //_serial_form.addToLog(value+"  "+stpIndex+"  "+axis);
            string sign = (((value < 32767) && (!invert)) || ((value > 32767) && (invert))) ? "-" : "";
            if (stpIndex > 0)
            {
                Int32.TryParse(Properties.Settings.Default["guiJoystickASpeed" + stpIndex.ToString()].ToString(), out speed);

                string sstep = "1";
                if ((axis == "X") || (axis == "Y"))
                {   sstep = Properties.Settings.Default["guiJoystickXYStep" + stpIndex.ToString()].ToString();
                    Int32.TryParse(Properties.Settings.Default["guiJoystickXYSpeed" + stpIndex.ToString()].ToString(), out speed);
                }
                else if ((axis == "Z"))
                {   sstep = Properties.Settings.Default["guiJoystickZStep" + stpIndex.ToString()].ToString();
                    Int32.TryParse(Properties.Settings.Default["guiJoystickZSpeed" + stpIndex.ToString()].ToString(), out speed);
                }
                else if ((axis == "A") || (axis == "B") || (axis == "C"))
                {   sstep = Properties.Settings.Default["guiJoystickAStep" + stpIndex.ToString()].ToString();
                    Int32.TryParse(Properties.Settings.Default["guiJoystickASpeed" + stpIndex.ToString()].ToString(), out speed);
                }
                return string.Format("{0}{1}{2}", axis, sign, sstep);
            }
            speed = 10;
            return "";
        }
        */
        #endregion

    }
}
