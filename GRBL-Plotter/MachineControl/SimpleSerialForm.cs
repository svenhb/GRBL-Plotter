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
/* 2021-01-15 First version
 * 2021-01-19 dont apply .toUpper for send box - line 95 
 * 2021-04-06 log RX and TX
 * 2021-07-26 code clean up / code quality
 * 2021-12-14 add try catch for sending
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class SimpleSerialForm : Form
    {
        public bool SerialPortOpen { get; private set; } = false;
        public bool Busy { get; private set; } = false;

        private readonly System.Timers.Timer timerSerial = new System.Timers.Timer();
        private readonly int timerReload = 100;

        private bool flag_closeForm = false;
        private int countMinimizeForm = 0;              // timer to minimize form
        private int countShutdown = 0;
        private int countTimeOut = 0;
        private int countTimeOutMax = 10;               // busy time out in seconds

        // Note: receive-event waits for line-end char, if missing -> timeout exception
        // to allow test hardware RX-Pin connected to TX-Pin line-end chars should be same
        private const string lineEndRX = "\n";          // grbl accepts '\n' or '\r', but marlin just sends '\n'
        private const string lineEndTX = "\r\n";        // grbl accepts '\n' or '\r' and sends "\r\n", but Marlin sends '\n'

        private string rxString;
        private bool isDataProcessing = false;      		// false when no data processing pending
        private string readyString = "ready";

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public SimpleSerialForm()
        {
            InitializeComponent();
            timerSerial.Elapsed += TimerSerial_Tick;
            timerSerial.Interval = timerReload;
            timerSerial.Enabled = true;
            timerSerial.Start();
        }

        private void BtnScanPort_Click(object sender, EventArgs e)
        { RefreshPorts(); }

        private void BtnOpenPort_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
                ClosePort();
            else
                OpenPort();
            UpdateControls();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        { rtbLog.Clear(); }
        private void TbCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) return;
            BtnSend_Click(sender, e);
        }
        private void BtnSend_Click(object sender, EventArgs e)
        { SendCommand(); }
        private void SendCommand()
        {
            {
                string cmd = cBCommand.Text;    //.ToUpper();
                SerialPortDataSend(cmd.Trim());
                cBCommand.Items.Remove(cBCommand.SelectedItem);
                cBCommand.Items.Insert(0, cmd);
                cBCommand.Text = cmd;
            }
        }

        /**************************************************************************
         * Timer to retry sending data   timerSerial.Interval = timerReload;
         **************************************************************************/
        private void TimerSerial_Tick(object sender, EventArgs e)
        {
            if (countMinimizeForm > 0)							// minimize form after connection - if enabled (then set start value)
            {
                if ((--countMinimizeForm == 0) && serialPort.IsOpen)
                {
                    this.WindowState = FormWindowState.Minimized;
                }
            }

            if (countTimeOut > 0)
            {
                if (--countTimeOut == 0)
                {
                    Busy = false;
                    AddToLog("Busy time out: didn't receive '" + readyString + "' within " + countTimeOutMax.ToString() + " sec.\r\n");
                    Logger.Trace("3rd serial time-out: reset busy signal after {0} sec", countTimeOutMax.ToString());
                }
            }

            /*     removed 2021-04-07   
             *     if (countShutdown > 0)		//(flag_closeForm)
                   {   Logger.Trace(" timer: countShutdown:{0}", countShutdown);
                       countShutdown--;
                       Application.Exit();
                   }*/
        }

        #region serial receive handling
        /*************************************************************************
         * RX Interupt
         * 1) serialPort1_DataReceived get data, call processGrblMessages
         *************************************************************************/
        //https://stackoverflow.com/questions/10871339/terminating-a-form-while-com-datareceived-event-keeps-fireing-c-sharp
        //https://stackoverflow.com/questions/1696521/c-proper-way-to-close-serialport-with-winforms/3176959

        internal delegate void InvokeDeleg();

        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))// && !blockSend)
            {
                rxString = string.Empty;
                try
                {
                    rxString = serialPort.ReadTo(lineEndRX).Trim();  //read line from grbl, discard CR LF
                    isDataProcessing = true;
                    this.BeginInvoke(new EventHandler(ProcessMessages));        //tigger rx process 2020-09-16 change from Invoke to BeginInvoke
                    while ((serialPort.IsOpen) && isDataProcessing)// && !blockSend)   //wait previous data line processed done
                    { }
                }
                catch (TimeoutException err1)
                {
                    Logger.Error(err1, "TimeoutException");
                    AddToLog("Error reading line from serial port - correct baud rate? Missing line-end?");
                    rxString = serialPort.ReadExisting().Trim();
                    Logger.Error(err1, "ReadExisting '{0}'", rxString);
                    AddToLog("RX TimeoutException" + rxString);
                    //          this.BeginInvoke(new EventHandler(closePort));    //closePort();
                }
                catch (Exception err)
                {
                    Busy = false;
                    AddToLog("RX Exception - Close port ");
                    Logger.Error(err, " -DataReceived- Close port, clear busy flag ");
                    this.BeginInvoke(new EventHandler(ClosePort));    //closePort();
                    throw;
                }
            }
        }

        public void SerialPortDataSend(string data)
        {
            if (serialPort.IsOpen && !string.IsNullOrEmpty(data))
            {
				try {
					serialPort.Write(data.Trim() + lineEndTX);      // send single command via form
					Busy = true;
					countTimeOut = (int)(countTimeOutMax * 1000 / timerSerial.Interval);
					string sndTXT = string.Format("> {0,-10} | > set busy flag:{1,4:G} | {2}", data.Trim(), countTimeOut.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
					AddToLog(sndTXT);
					Logger.Trace("send {0}", sndTXT);
				}
                catch (Exception err)
                {       // InvalidOperationException, ArgumentNullException, TimeoutException
                    Busy = false;
                    AddToLog("TX Exception "+err.Message+" - Close port, clear busy flag");
                    Logger.Error(err, "SerialPortDataSend 3rd com ");
                    this.BeginInvoke(new EventHandler(ClosePort));    //closePort();
                }
            }
        }

        private void ProcessMessages(object sender, EventArgs e)	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#message-summary
        {
            if (string.IsNullOrEmpty(rxString)) { isDataProcessing = false; return; }     // unlock serialPort1_DataReceived
            if (countShutdown > 0) { isDataProcessing = false; return; }

            if (rxString.Contains(readyString))
            { Busy = false; countTimeOut = 0; AddToLog(string.Format("< {0,-10} | {1,-20} | {2}", rxString, "> clear busy flag!", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"))); }
            else
                AddToLog(string.Format("< {0,-35} | {1}", rxString, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));

            Logger.Trace("3rd serial RX '{0}'   busy:{1}", rxString, Busy);
            isDataProcessing = false;                   // unlock serialPort1_DataReceived
        }


        #endregion		


        #region serialPort
        private bool RefreshPorts()
        {
            List<String> tList = new List<String>();
            cbPort.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) tList.Add(s);
            if (tList.Count < 1) LogError("! No serial ports found", null);
            else
            {
                tList.Sort();
                cbPort.Items.AddRange(tList.ToArray());
            }
            return tList.Contains(cbPort.Text);
        }
        private void OpenPort()
        {
            try
            {
                Logger.Info("OpenPort {0} {1}", cbPort.Text, cbBaud.Text);
                serialPort.PortName = cbPort.Text;
                serialPort.DataBits = 8;
                serialPort.BaudRate = Convert.ToInt32(cbBaud.Text);
                serialPort.Parity = System.IO.Ports.Parity.None;
                serialPort.StopBits = System.IO.Ports.StopBits.One;
                serialPort.Handshake = System.IO.Ports.Handshake.None;
                serialPort.DtrEnable = false;
				serialPort.ReadTimeout = 500;
				serialPort.WriteTimeout = 1000;		
				
                rtbLog.Clear();
                if (RefreshPorts())
                {
                    if (Properties.Settings.Default.ctrlUseSerialPortFixer)
                    {
                        try
                        { SerialPortFixer.Execute(cbPort.Text); }
                        catch (Exception err)
                        {
                            Logger.Error(err, "Error SerialPortFixer: "); //throw;
                        }
                    }
                    serialPort.Open();
                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();

                    AddToLog("* Open " + cbPort.Text + "\r\n");
                    btnOpenPort.Text = Localization.GetString("serialClose");  // "Close";

                    if (Properties.Settings.Default.serialMinimize)
                        countMinimizeForm = (int)(3000 / timerSerial.Interval); 	// minimize window after 3 sec.

                    timerSerial.Interval = timerReload;       		// timerReload;
                }
                else
                {
                    AddToLog("* " + cbPort.Text + " not available\r\n");
                    Logger.Warn("Port {1} not available", cbPort.Text);
                }
                SerialPortOpen = serialPort.IsOpen;
                UpdateControls();
            }
            catch (Exception err)
            {
                Logger.Error(err, "Error OpenPort: ");
                countMinimizeForm = 0;
                LogError("! Opening port", err);
                SerialPortOpen = false;
                UpdateControls();
                //    throw;
            }
        }
        private void ClosePort(object sender, EventArgs e)
        { ClosePort(); }
        public void ClosePort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    Logger.Info("ClosePort ");
                    serialPort.Close();
                }
                serialPort.Dispose();
                SaveSettings();
                if (!flag_closeForm)
                {
                    AddToLog("\r* Close \r");
                    btnOpenPort.Text = Localization.GetString("serialOpen");  // "Open";
                    UpdateControls();
                }
                timerSerial.Interval = 1000;
                Busy = false;
            }
            catch (Exception err)
            {
                Logger.Error(err, "Error ClosePort: ");
                LogError("! Closing port", err);
                if (!flag_closeForm)
                { UpdateControls(); }
                timerSerial.Enabled = false;
                //    throw;
            }
            SerialPortOpen = false;
        }
        #endregion
        private void UpdateControls()
        {
            bool isConnected = serialPort.IsOpen;
            SerialPortOpen = isConnected;
        }
        private void LogError(string message, Exception error)
        {
            string textmsg = "\r\n[ERROR]: " + message + ". ";
            if (error != null) textmsg += error.Message;
            textmsg += "\r\n";
            AddToLog(textmsg);
        }
        delegate void addToLogCallback(string text);
        public void AddToLog(string text)
        {
            if (this.InvokeRequired == true)
            {
                addToLogCallback callback = new addToLogCallback(AddToLog);
                this.Invoke(callback, new object[] { text });
            }
            else
            {
                try
                {
                    rtbLog.AppendText(text + "\r");
                    rtbLog.ScrollToCaret();
                }
                catch
                {
                    // throw;
                }
            }
        }

        private void SerialForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Icon;
            //   Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            UpdateControls();       // disable controls
            LoadSettings();         // set last COM and Baud
            RefreshPorts();         // scan for COMs
            OpenPort();             // open COM
        }

        private void LoadSettings()
        {
            Logger.Trace("loadSettings '{0}' '{1}' '{2}' '{3}'", Properties.Settings.Default.serialPort3, Properties.Settings.Default.serialBaud3, Properties.Settings.Default.serial3Ready, Properties.Settings.Default.serial3Timeout);
            try
            {
                cbPort.Text = Properties.Settings.Default.serialPort3;
                cbBaud.Text = Properties.Settings.Default.serialBaud3;

                Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
                Location = Properties.Settings.Default.locationSerForm3;
                if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { this.CenterToScreen(); }    // Location = new Point(100, 100);    }

                readyString = tBMessageReady.Text = Properties.Settings.Default.serial3Ready;
                countTimeOutMax = Properties.Settings.Default.serial3Timeout;
                nUDTimeout.Value = countTimeOutMax;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error LoadSettings: ");
                LogError("! Loading settings", e);
                //    throw;
            }
        }
        private void SaveSettings()
        {
            Logger.Trace("saveSettings {0} {1} {2} {3}", cbPort.Text, cbBaud.Text, tBMessageReady.Text, nUDTimeout.Value);
            try
            {
                Properties.Settings.Default.locationSerForm3 = Location;
                Properties.Settings.Default.serialPort3 = cbPort.Text;
                Properties.Settings.Default.serialBaud3 = cbBaud.Text;
                Properties.Settings.Default.serial3Ready = tBMessageReady.Text;
                Properties.Settings.Default.serial3Timeout = (int)nUDTimeout.Value;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error SaveSettings: ");
                LogError("! Saving settings", e);
                //    throw;
            }
        }

        private void NudTimeout_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.serial3Timeout = countTimeOutMax = (int)nUDTimeout.Value;
            Properties.Settings.Default.Save();
        }

        private void SimpleSerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("====== Try closing SimpleSerialForm ");
            serialPort.DataReceived -= (this.SerialPort1_DataReceived); // stop receiving data
            SaveSettings();
            ClosePort();
            countShutdown = 3;
            flag_closeForm = true;
            e.Cancel = false;
        }

        private void TbMessageReady_TextChanged(object sender, EventArgs e)
        {
            readyString = Properties.Settings.Default.serial3Ready = tBMessageReady.Text;
            Properties.Settings.Default.Save();
        }

        private void SimpleSerialForm_SizeChanged(object sender, EventArgs e)
        {
            rtbLog.Height = Height - 122;
            rtbLog.Width = Width - 20;
            btnClear.Top = Height - 90;
            cBCommand.Top = Height - 89;
            btnSend.Top = Height - 90;
            label1.Top = Height - 58;
            tBMessageReady.Top = Height - 61;
            label2.Top = Height - 58;
            nUDTimeout.Top = Height - 60;
        }
    }
}
