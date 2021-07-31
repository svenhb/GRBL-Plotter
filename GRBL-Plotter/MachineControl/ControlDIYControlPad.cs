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
 * 2018-12-26 Commits from RasyidUFA via Github
 * 2019-08-15 add logger, line 120 replace .Invoke by .BeginInvoke
 * 2019-10-31 add SerialPortFixer http://zachsaw.blogspot.com/2010/07/serialport-ioexception-workaround-in-c.html
 * 2021-07-22 code clean up / code quality
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//#pragma warning disable CA1303
//#pragma warning disable CA1305
//#pragma warning disable CA1307

namespace GrblPlotter
{
    public partial class ControlDiyControlPad : Form
    {
        private string rxString, rxTmpString;
        private byte rxChar;
        internal bool isHeightProbing = false;
        //      internal bool isConnected;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool IsConnected
        {
            set { IsConnected = value; }
            get { return serialPort.IsOpen; }
        }
        public ControlDiyControlPad()
        {
            Logger.Trace("++++++ ControlDIYControlPad START ++++++");
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }

        private void BtnOpenPort_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
                ClosePort();
            else
                OpenPort();
        }

        private void BtnScanPort_Click(object sender, EventArgs e)
        { RefreshPorts(); }
        private void RefreshPorts()
        {
            List<String> tList = new List<String>();
            cbPort.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) tList.Add(s);
            if (tList.Count < 1) LogError("No serial ports found", null);
            else
            {
                tList.Sort();
                cbPort.Items.AddRange(tList.ToArray());
            }
        }

        private bool OpenPort()
        {
            try
            {
                Logger.Info("OpenPort {0}", serialPort.PortName);
                serialPort.PortName = cbPort.Text;
                serialPort.BaudRate = Convert.ToInt32(cbBaud.Text);
                serialPort.Encoding = Encoding.GetEncoding(28591);
                SerialPortFixer.Execute(cbPort.Text);
                serialPort.Open();
                rtbLog.Clear();
                rtbLog.AppendText("Open " + cbPort.Text + "\r\n");
                btnOpenPort.Text = "Close";
                return (true);
            }
            catch (Exception err)
            {
                Logger.Error(err, "Error OpenPort: ");
                LogError("Opening port", err);
                return (false);
            }
        }
        public bool ClosePort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    Logger.Info("ClosePort {0}", serialPort.PortName);
                    serialPort.Close();
                }
                rtbLog.AppendText("Close " + cbPort.Text + "\r\n");
                btnOpenPort.Text = "Open";
                return (true);
            }
            catch (Exception err)
            {
                Logger.Error(err, "Error ClosePort: ");
                LogError("Closing port", err);
                return (false);
            }
        }
        private void LogError(string message, Exception err)
        {
            string textmsg = "\r\n[ERROR]: " + message + ". ";
            if (err != null) textmsg += err.Message;
            textmsg += "\r\n";
            rtbLog.AppendText(textmsg);
            rtbLog.ScrollToCaret();
        }

        private static byte lastChar = 0;
        private static readonly List<byte> isRealTimeCmd = new List<byte> { 0x18, (byte)'?', (byte)'~', (byte)'!' };
        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))
            {
                byte rxTmpChar = 0;
                try
                {
                    byte[] rxBuff = new byte[serialPort.BytesToRead];
                    serialPort.Read(rxBuff, 0, serialPort.BytesToRead);
                    foreach (byte rxTmpChart in rxBuff)
                    {
                        rxTmpChar = rxTmpChart;
                        if ((rxTmpChar > 0x7F) || (isRealTimeCmd.Contains(rxTmpChar)))  // is real time cmd ?
                        {
                            rxChar = rxTmpChar;
                            this.BeginInvoke(new EventHandler(HandleRxData));
                        }
                        else
                        {
                            rxTmpString += (char)rxTmpChar;
                            if ((rxTmpChar == '\r') || (rxTmpChar == '\n'))             // end of regular command
                            {
                                if (lastChar >= ' ')
                                {
                                    rxChar = 0;
                                    rxString = rxTmpString.Trim();
                                    this.BeginInvoke(new EventHandler(HandleRxData));
                                }
                                rxTmpString = "";
                            }
                        }
                        lastChar = rxTmpChar;
                    }
                }
                catch (Exception err)
                {
                    Logger.Error(err, "Error DataReceived: ");
                    LogError("Error reading line from serial port", err);
                }
            }
        }

        private void HandleRxData(object sender, EventArgs e)
        {
            if ((rxChar > 0x7F) || (isRealTimeCmd.Contains(rxChar)))
            {
                rtbLog.AppendText(string.Format("< 0x{0:X} {1}\r\n", rxChar, Grbl.GetRealtimeDescription(rxChar)));
                rtbLog.ScrollToCaret();
                OnRaiseCommandEvent(new CommandEventArgs(rxChar));
                rxChar = 0;
            }
            else
            {
                if (rxString.Length > 2)        // don't send single \n
                {
                    if (cBFeedback.Checked || (!isHeightProbing && !rxString.StartsWith("(")))
                    {
                        rtbLog.AppendText(string.Format("< {0} \r\n", rxString));
                        rtbLog.ScrollToCaret();
                    }
                    OnRaiseCommandEvent(new CommandEventArgs(rxString));
                }
                rxString = "";
            }
        }
        private void BtnSimulate_Click(object sender, EventArgs e)
        {
            rtbLog.AppendText(string.Format("< {0} \r\n", tBSimulate.Text));
            OnRaiseCommandEvent(new CommandEventArgs(tBSimulate.Text));
        }

        public void SendFeedback(string data)
        { SendFeedback(data, false); }
        public void SendFeedback(string data, bool forceShow)
        {
            SendLine(data);
            if (forceShow)
            {
                rtbLog.AppendText(string.Format(">!!! {0} \r\n", data));
                rtbLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// sendLine - now really send data to Arduino
        /// </summary>
        private void SendLine(string data)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Write(data + "\r\n");
                    if (cBFeedback.Checked)
                    {
                        rtbLog.AppendText(string.Format("> {0} \r\n", data));
                        rtbLog.ScrollToCaret();
                    }
                }
                else
                {
                    if (cBFeedback.Checked)
                    {
                        rtbLog.AppendText(string.Format(">| {0} \r\n", data));
                        rtbLog.ScrollToCaret();
                    }
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "Error SendLine: ");
                if (cBFeedback.Checked)
                    rtbLog.AppendText(string.Format(">| {0} \r\n", data));
                //          throw;
            }
            while (rtbLog.Lines.Length > 99)
            {
                rtbLog.SelectionStart = 0;
                rtbLog.SelectionLength = rtbLog.Text.IndexOf("\n", 0) + 1;
                rtbLog.SelectedText = "";
            }

            if (cBFeedback.Checked)
            {
                rtbLog.Select(rtbLog.TextLength, 0);
                rtbLog.ScrollToCaret();
            }
        }
        public event EventHandler<CommandEventArgs> RaiseStreamEvent;
        protected virtual void OnRaiseCommandEvent(CommandEventArgs e)
        {
            RaiseStreamEvent?.Invoke(this, e);
        }

        private void ControlDIYControlPad_Load(object sender, EventArgs e)
        {
            cbPort.Text = Properties.Settings.Default.serialPortDIY;
            cbBaud.Text = Properties.Settings.Default.serialBaudDIY;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            Location = Properties.Settings.Default.locationSerForm4;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { this.CenterToScreen(); }    // Location = new Point(100, 100);    }
            OpenPort();
        }

        private void ControlDIYControlPad_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ ControlDIYControlPad Stop ++++++");
            Properties.Settings.Default.serialPortDIY = cbPort.Text;
            Properties.Settings.Default.serialBaudDIY = cbBaud.Text;
            Properties.Settings.Default.locationSerForm4 = Location;
        }

        private void ControlDIYControlPad_Resize(object sender, EventArgs e)
        {
            rtbLog.Width = this.Width - 20;
            rtbLog.Height = this.Height - 90;
        }
    }

    public class CommandEventArgs : EventArgs
    {
        private readonly string cmdString;
        private readonly byte cmdChar;
        public CommandEventArgs(string cmd)
        {
            cmdString = cmd;
        }
        public CommandEventArgs(byte cmd)
        {
            cmdChar = cmd;
        }
        public string Command
        { get { return cmdString; } }
        public byte RealTimeCommand
        { get { return cmdChar; } }
    }
}