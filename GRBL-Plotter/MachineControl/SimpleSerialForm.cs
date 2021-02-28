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
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class SimpleSerialForm : Form
    {
        public bool serialPortOpen { get; private set; } = false;
        public bool busy { get; private set; } = false;

        private System.Timers.Timer timerSerial = new System.Timers.Timer();
        private int timerReload = 100;
        
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
            timerSerial.Elapsed += timerSerial_Tick;
            timerSerial.Interval = timerReload;
            timerSerial.Enabled = true;
            timerSerial.Start();
        }


        private void btnScanPort_Click(object sender, EventArgs e)
        { refreshPorts(); }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
                closePort();
            else
                openPort();
            updateControls();
        }

        private void btnClear_Click(object sender, EventArgs e)
        { rtbLog.Clear(); }
        private void tbCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) return;
            btnSend_Click(sender, e);
        }
        private void btnSend_Click(object sender, EventArgs e)
        { sendCommand(); }
        private void sendCommand()
        {
            {
                string cmd = cBCommand.Text;    //.ToUpper();
                send(cmd.Trim());
                cBCommand.Items.Remove(cBCommand.SelectedItem);
                cBCommand.Items.Insert(0, cmd);
                cBCommand.Text = cmd;
            }
        }

        public void send(string data)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(data.Trim() + lineEndTX);      // send single command via form
                busy = true;
                countTimeOut = (int)(countTimeOutMax * 1000 / timerSerial.Interval);
                addToLog("> " + data.Trim() + " set busy flag: " + countTimeOut.ToString());
            }
        }
        
        /**************************************************************************
         * Timer to retry sending data   timerSerial.Interval = timerReload;
         **************************************************************************/
        private void timerSerial_Tick(object sender, EventArgs e)
        {   if (countMinimizeForm > 0)							// minimize form after connection - if enabled (then set start value)
            {   if ((--countMinimizeForm == 0) && serialPort.IsOpen)
                {   this.WindowState = FormWindowState.Minimized;
                }
            }

            if (countTimeOut > 0)
            {   if (--countTimeOut == 0)
                {
                    busy = false;
                    addToLog("Busy time out: didn't receive '" + readyString + "' within " + countTimeOutMax.ToString() + " sec.\r\n");
                }
            }

            if (countShutdown > 0)		//(flag_closeForm)
            {   Logger.Trace(" timer: countShutdown:{0}", countShutdown);
                countShutdown--;
                Application.Exit();
            }
        }

        #region serial receive handling
        /*************************************************************************
         * RX Interupt
         * 1) serialPort1_DataReceived get data, call processGrblMessages
         *************************************************************************/
        //https://stackoverflow.com/questions/10871339/terminating-a-form-while-com-datareceived-event-keeps-fireing-c-sharp
        //https://stackoverflow.com/questions/1696521/c-proper-way-to-close-serialport-with-winforms/3176959

        public delegate void InvokeDelegate();

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))// && !blockSend)
            {
                rxString = string.Empty;
                try
                {
                    rxString = serialPort.ReadTo(lineEndRX).Trim();  //read line from grbl, discard CR LF
                    isDataProcessing = true;
                    this.BeginInvoke(new EventHandler(processMessages));        //tigger rx process 2020-09-16 change from Invoke to BeginInvoke
                    while ((serialPort.IsOpen) && isDataProcessing)// && !blockSend)   //wait previous data line processed done
                    { }
                }
                catch (TimeoutException err1)
                {
                    Logger.Error(err1, "TimeoutException");
                    addToLog("Error reading line from serial port - correct baud rate? Missing line-end?");
                    rxString = serialPort.ReadExisting().Trim();
                    Logger.Error("ReadExisting '{0}'",rxString);
                    addToLog(rxString);
          //          this.BeginInvoke(new EventHandler(closePort));    //closePort();
                }
                catch (Exception err)
                {
                    addToLog("Close port ");
                    Logger.Error(err, " -DataReceived- Close port ");
         //           this.BeginInvoke(new EventHandler(closePort));    //closePort();
                }
            }
        }

        private void processMessages(object sender, EventArgs e)	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#message-summary
        {
            if (rxString == "") { isDataProcessing = false; return; }     // unlock serialPort1_DataReceived
            if (countShutdown > 0) { isDataProcessing = false; return; }

            if (rxString.Contains(readyString))
            {   busy = false; countTimeOut = 0; addToLog("< " + rxString + " clear busy flag");  }
            else
                addToLog("< " + rxString);

            isDataProcessing = false;                   // unlock serialPort1_DataReceived
        }


        #endregion		


        #region serialPort
        private bool refreshPorts()
        {
            List<String> tList = new List<String>();
            cbPort.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) tList.Add(s);
            if (tList.Count < 1) logError("! No serial ports found", null);
            else
            {
                tList.Sort();
                cbPort.Items.AddRange(tList.ToArray());
            }
            return tList.Contains(cbPort.Text);
        }
        private void openPort()
        {
            try
            {
                Logger.Info("openPort {0} {1}", cbPort.Text, cbBaud.Text);
                serialPort.PortName = cbPort.Text;
                serialPort.DataBits = 8;
                serialPort.BaudRate = Convert.ToInt32(cbBaud.Text);
                serialPort.Parity = System.IO.Ports.Parity.None;
                serialPort.StopBits = System.IO.Ports.StopBits.One;
                serialPort.Handshake = System.IO.Ports.Handshake.None;
                serialPort.DtrEnable = false;
                rtbLog.Clear();
                if (refreshPorts())
                {
                    if (Properties.Settings.Default.ctrlUseSerialPortFixer)
                    {
                        try
                        { SerialPortFixer.Execute(cbPort.Text); }
                        catch (Exception err)
                        { Logger.Error(err, " -SerialPortFixer-"); }
                    }
                    serialPort.Open();
                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();

                    addToLog("* Open " + cbPort.Text + "\r\n");
                    btnOpenPort.Text = Localization.getString("serialClose");  // "Close";

                    if (Properties.Settings.Default.serialMinimize)
                        countMinimizeForm = (int)(3000 / timerSerial.Interval); 	// minimize window after 3 sec.

                    timerSerial.Interval = timerReload;       		// timerReload;
                }
                else
                {
                    addToLog("* " + cbPort.Text + " not available\r\n");
                    Logger.Warn("Port {1} not available", cbPort.Text);
                }
                serialPortOpen = serialPort.IsOpen;
                updateControls();
            }
            catch (Exception err)
            {
                Logger.Error(err, " -openPort-");
                countMinimizeForm = 0;
                logError("! Opening port", err);
                serialPortOpen = false;
                updateControls();
            }
        }
        private void closePort(object sender, EventArgs e)
        { closePort(); }
        public void closePort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    Logger.Info(" closePort ");
                    serialPort.Close();
                }
                serialPort.Dispose();
                saveSettings();
                if (!flag_closeForm)
                {   addToLog("\r* Close \r");
                    btnOpenPort.Text = Localization.getString("serialOpen");  // "Open";
                    updateControls();
                }
                timerSerial.Interval = 1000;
            }
            catch (Exception err)
            {
                Logger.Error(err, " -closePort- ");
                logError("! Closing port", err);
                if (!flag_closeForm)
                { updateControls(); }
                timerSerial.Enabled = false;
            }
            serialPortOpen = false;
        }
        #endregion
        private void updateControls()
        {
            bool isConnected = serialPort.IsOpen;
            serialPortOpen = isConnected;
        }
        private void logError(string message, Exception error)
        {
            string textmsg = "\r\n[ERROR]: " + message + ". ";
            if (error != null) textmsg += error.Message;
            textmsg += "\r\n";
            addToLog(textmsg);
        }
        delegate void addToLogCallback(string text);
        public void addToLog(string text)
        {
            if (this.InvokeRequired == true)
            {
                addToLogCallback callback = new addToLogCallback(addToLog);
                this.Invoke(callback, new object[] { text });
            }
            else
            {
                try
                {
                    rtbLog.AppendText(text + "\r");
                    rtbLog.ScrollToCaret();
                }
                catch { }
            }
        }

        private void SerialForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Icon;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            updateControls();       // disable controls
            loadSettings();         // set last COM and Baud
            refreshPorts();         // scan for COMs
            openPort();             // open COM
        }

        private void loadSettings()
        {
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
                Logger.Error(e, " -loadSettings-");
                logError("! Loading settings", e);
            }
        }
        private void saveSettings()
        {
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
                Logger.Error(e, " -saveSettings-");
                logError("! Saving settings", e);
            }
        }

        private void nUDTimeout_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.serial3Timeout = countTimeOutMax = (int)nUDTimeout.Value;
            Properties.Settings.Default.Save();
        }

        private void SimpleSerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("====== Try closing SimpleSerialForm ");
            serialPort.DataReceived -= (this.serialPort1_DataReceived); // stop receiving data
            saveSettings();
            closePort();
            countShutdown = 3;
            flag_closeForm = true;
            e.Cancel = false;
        }

        private void tBMessageReady_TextChanged(object sender, EventArgs e)
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
