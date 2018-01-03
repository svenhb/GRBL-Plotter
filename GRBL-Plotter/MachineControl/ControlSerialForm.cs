/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

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
/*  Many thanks to:
    3dpBurner Sender. A GCODE sender for GRBL based devices.
    This file is part of 3dpBurner Sender application.   
    Copyright (C) 2014-2015  Adrian V. J. (villamany) contact: villamany@gmail.com

    This project was my starting point
*/
/*  2016-07-xx add 2nd serial port for tool changer
 *  2016-09-17  improve performance by removing already sent lines from sendLines[] and gCodeLines[]
 *              Remove unknown G-Codes in preProcessStreaming() (list in grblRelated)
 *  2016-09-25  Implement override function
 *  2016-09-26  reduce  grblBufferSize to 100 during $C check gcode to reduce fake errors
 *  2016-12-31  add GRBL 1.1 compatiblity, clean-up
 *  2017-01-01  check form-location and fix strange location
 *  2018-01-02  Bugfix route errors during streaming from serialform to gui
*/

//#define debuginfo 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Threading;

namespace GRBL_Plotter
{
    public partial class ControlSerialForm : Form
    {
        public xyzPoint posWorld, posMachine, posWCO, posPause, posProbe, posProbeOld;
        private strStruct statusMsg;
        ControlSerialForm _serial_form2;

        public bool isGrblVers0 { get; private set; } = true;
        public bool isLasermode { get; private set; } = false;
        private int timerReload=200;
        private string rxString;
        private string parserState = "";
        public bool serialPortOpen { get; private set; } = false;
        private bool useSerial2 = false;
        private int iamSerial = 1;
        private string title = "";
        public bool toolInSpindle { get; set; } = false;
        public bool isHeightProbing { get; set; } = false;  // automatic height probing -> less feedback
        private bool ctrl4thUse = false;
        private string ctrl4thName = "A";

        public List<string> GRBLSettings = new List<string>();

        private Dictionary<string, double> gcodeVariable = new Dictionary<string, double>();

        public ControlSerialForm(string txt, int nr, ControlSerialForm handle = null)
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            title = txt;
            this.Invalidate();
            iamSerial = nr;
            set2ndSerial(handle);
            InitializeComponent();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
        }
        public void set2ndSerial(ControlSerialForm handle = null)
        {   _serial_form2 = handle;
            if (handle != null)
                useSerial2 = true;
        }

        //Unhandled exception
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            closePort();
            Exception ex = e.Exception;
            MessageBox.Show(ex.Message, "Thread exception");
        }
        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                closePort();
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show(ex.Message, "Application exception");
            }
        }

        private void SerialForm_Load(object sender, EventArgs e)
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            SerialForm_Resize(sender, e);
            refreshPorts();
            updateControls();
            loadSettings();
            openPort();
            statusMsg.Bf = ""; statusMsg.Ln = ""; statusMsg.FS = ""; statusMsg.Pn = ""; statusMsg.Ov = ""; statusMsg.A = "";
            if (iamSerial == 1)
            {   Location = Properties.Settings.Default.locationSerForm1;
                ctrl4thUse  = Properties.Settings.Default.ctrl4thUse;
                ctrl4thName = Properties.Settings.Default.ctrl4thName;
            }
            else
            {   Location = Properties.Settings.Default.locationSerForm2;}
            Text = title;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width-100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height-100))) { Location = new Point(100, 100); }
            isLasermode = Properties.Settings.Default.ctrlLaserMode;
        }
        private bool mainformAskClosing = false;
        private void SerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (iamSerial == 1)
            {
                saveLastPos();
                Properties.Settings.Default.locationSerForm1 = Location;
                Properties.Settings.Default.ctrlLaserMode = isLasermode;
            }
            else
            {
                Properties.Settings.Default.locationSerForm2 = Location;
            }
            saveSettings();
            if ((e.CloseReason.ToString() != "FormOwnerClosing") & !mainformAskClosing)
            {
                var window = MessageBox.Show("Serial Connection is needed.\r\nClose main window instead");
                e.Cancel = true;
                return;
            }
            else
            {
                closePort();
                mainformAskClosing = true;
            }

        }
        private void SerialForm_Resize(object sender, EventArgs e)
        {
            rtbLog.Width = this.Width - 20;
            rtbLog.Height = this.Height - 205;
            btnClear.Location = new Point(btnClear.Location.X, this.Height - 86);
            cBCommand.Location = new Point(cBCommand.Location.X, this.Height - 84);
            btnSend.Location = new Point(btnSend.Location.X, this.Height - 86);
            btnGRBLCommand0.Location = new Point(btnGRBLCommand0.Location.X, this.Height - 62);
            btnGRBLCommand1.Location = new Point(btnGRBLCommand1.Location.X, this.Height - 62);
            btnGRBLCommand2.Location = new Point(btnGRBLCommand2.Location.X, this.Height - 62);
            btnGRBLCommand3.Location = new Point(btnGRBLCommand3.Location.X, this.Height - 62);
            btnGRBLCommand4.Location = new Point(btnGRBLCommand4.Location.X, this.Height - 62);
            btnGRBLReset.Location = new Point(btnGRBLReset.Location.X, this.Height - 62);
        }

        private void timerSerialEnable(bool value)
        {
            timerSerial.Enabled = value;
        }

        private void timerSerial_Tick(object sender, EventArgs e)
        {
            if (minimizeCount > 0)
            { minimizeCount--;
                if (minimizeCount == 0)
                    this.WindowState = FormWindowState.Minimized;
            }

            if (serialPort1.IsOpen)
            {
                try
                {   var dataArray = new byte[] { Convert.ToByte('?') };
                    serialPort1.Write(dataArray, 0, 1);                        
                }
                catch (Exception er)
                {   logError("Retrieving GRBL status", er);
                    serialPort1.Close();
                }
            }
            if (waitForIdle)
                processSend();
            if (isStreaming && !isStreamingRequestPause && !isStreamingPause)
                preProcessStreaming();
        }

        private void resetVariables()
        {
            gcodeVariable.Clear();
            gcodeVariable.Add("PRBX", 0.0); // Probing coordinates
            gcodeVariable.Add("PRBY", 0.0);
            gcodeVariable.Add("PRBZ", 0.0);
            gcodeVariable.Add("PRDX", 0.0); // Probing delta coordinates
            gcodeVariable.Add("PRDY", 0.0); // delta = actual - last
            gcodeVariable.Add("PRDZ", 0.0);
            gcodeVariable.Add("MACX", 0.0); // actual Machine coordinates
            gcodeVariable.Add("MACY", 0.0);
            gcodeVariable.Add("MACZ", 0.0);
            gcodeVariable.Add("WACX", 0.0); // actual World coordinates
            gcodeVariable.Add("WACY", 0.0);
            gcodeVariable.Add("WACZ", 0.0);
            gcodeVariable.Add("MLAX", 0.0); // last Machine coordinates (before break)
            gcodeVariable.Add("MLAY", 0.0);
            gcodeVariable.Add("MLAZ", 0.0);
            gcodeVariable.Add("WLAX", 0.0); // last World coordinates (before break)
            gcodeVariable.Add("WLAY", 0.0);
            gcodeVariable.Add("WLAZ", 0.0);
            gcodeVariable.Add("TOOL", 0.0); // Last Tool number
        }
        private void loadSettings()
        {
            try
            {
                cbPort.Text = Properties.Settings.Default.serialPort1;
                cbBaud.Text = Properties.Settings.Default.serialBaud1;
                if (iamSerial == 2)
                {
                    cbPort.Text = Properties.Settings.Default.serialPort2;
                    cbBaud.Text = Properties.Settings.Default.serialBaud2;
                }
            }
            catch (Exception e)
            {
                logError("Loading settings", e);
            }
        }
        private void saveSettings()
        {
            try
            {
                if (iamSerial == 1)
                {
                    Properties.Settings.Default.serialPort1 = cbPort.Text;
                    Properties.Settings.Default.serialBaud1 = cbBaud.Text;
                    Properties.Settings.Default.ctrlLaserMode = isLasermode;
                }
                else
                {
                    Properties.Settings.Default.serialPort2 = cbPort.Text;
                    Properties.Settings.Default.serialBaud2 = cbBaud.Text;
                }

                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                logError("Saving settings", e);
            }
        }
        private void saveLastPos()
        {
            if (iamSerial == 1)
            {   if (ctrl4thUse)
                    rtbLog.AppendText(String.Format("Save last pos.: X{0:0.###} Y{1:0.###} Z{2:0.###} {3}{4:0.###}\r\n", posWorld.X, posWorld.Y, posWorld.Z, ctrl4thName, posWorld.A));
                else
                    rtbLog.AppendText(String.Format("Save last pos.: X{0:0.###} Y{1:0.###} Z{2:0.###}\r\n", posWorld.X, posWorld.Y, posWorld.Z));
                Properties.Settings.Default.lastOffsetX = Math.Round(posWorld.X, 3);
                Properties.Settings.Default.lastOffsetY = Math.Round(posWorld.Y, 3);
                Properties.Settings.Default.lastOffsetZ = Math.Round(posWorld.Z, 3);
                Properties.Settings.Default.lastOffsetA = Math.Round(posWorld.A, 3);
                Properties.Settings.Default.Save();
            }
        }

        private void updateControls()
        {
            bool isConnected = serialPort1.IsOpen;
            serialPortOpen = isConnected;
            bool isSensing = isStreaming;
            cbPort.Enabled = !isConnected;
            cbBaud.Enabled = !isConnected;
            btnScanPort.Enabled = !isConnected;
            btnClear.Enabled = isConnected;
            cBCommand.Enabled = isConnected & !isSensing;
            btnSend.Enabled = isConnected & !isSensing;
            btnGRBLCommand0.Enabled = isConnected & !isSensing;
            btnGRBLCommand1.Enabled = isConnected & !isSensing;
            btnGRBLCommand2.Enabled = isConnected & !isSensing;
            btnGRBLCommand3.Enabled = isConnected & !isSensing;
            btnGRBLCommand4.Enabled = isConnected & !isSensing;
            btnGRBLReset.Enabled = isConnected;// & !isSensing;
        }

        private void logError(string message, Exception err)
        {
            string textmsg = "\r\n[ERROR]: " + message + ". ";
            if (err != null) textmsg += err.Message;
            textmsg += "\r\n";
            rtbLog.AppendText(textmsg);
            rtbLog.ScrollToCaret();
        }
        public void logErrorThr(object sender, EventArgs e)
        {
            logError(mens, err);
            updateControls();
        }
        public void addToLog(string text)
        {
            rtbLog.AppendText(text + "\r");
            rtbLog.ScrollToCaret();
        }

        private void btnScanPort_Click(object sender, EventArgs e)
        { refreshPorts(); }
        private void refreshPorts()
        {
            List<String> tList = new List<String>();
            cbPort.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) tList.Add(s);
            if (tList.Count < 1) logError("No serial ports found", null);
            else
            {
                tList.Sort();
                cbPort.Items.AddRange(tList.ToArray());
            }
        }
        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
                closePort();
            else
                openPort();
            updateControls();
        }
        private int minimizeCount = 0;
        private bool openPort()
        {
            try
            {
                serialPort1.PortName = cbPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(cbBaud.Text);
                serialPort1.Open();
                rtbLog.Clear();
                rtbLog.AppendText("Open " + cbPort.Text + "\r\n");
                btnOpenPort.Text = "Close";
                isDataProcessing = true;
                grblReset(false);
                updateControls();
                if (Properties.Settings.Default.serialMinimize)
                    minimizeCount = 10;     // minimize window after 10 timer ticks
                timerSerial.Interval = timerReload;
                return (true);
            }
            catch (Exception err)
            {
                minimizeCount = 0;
                logError("Opening port", err);
                updateControls();
                return (false);
            }
        }
        private bool closePort()
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
                rtbLog.AppendText("Close " + cbPort.Text + "\r\n");
                btnOpenPort.Text = "Open";
                updateControls();
                timerSerial.Interval = 1000;
                return (true);
            }
            catch (Exception err)
            {
                logError("Closing port", err);
                updateControls();
                timerSerial.Enabled = false;
                return (false);
            }
        }

        //Send reset sentence
        public void grblReset(bool savePos = true)//Stop/reset button
        {
            if (savePos)
            { saveLastPos(); }
            resetVariables();
            isStreaming = false;
            isStreamingPause = false;
            waitForIdle = false;
            var dataArray = new byte[] { 24 };//Ctrl-X
            serialPort1.Write(dataArray, 0, 1);
            rtbLog.AppendText("[CTRL-X]\r\n");
            lastCmd = "M5M9M0";
            if (iamSerial == 1)
            {   ctrl4thUse  = Properties.Settings.Default.ctrl4thUse;
                ctrl4thName = Properties.Settings.Default.ctrl4thName;
            }
        }

        #region serial receive handling
        /*  RX Interupt
         * */
        string mens;
        Exception err;
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort1.IsOpen) && (serialPort1.BytesToRead > 0))
            {   rxString = string.Empty;
                try
                {   rxString = serialPort1.ReadTo("\r\n");              //read line from grbl, discard CR LF
                    isDataProcessing = true;
                    this.Invoke(new EventHandler(handleRxData));        //tigger rx process 
                    while ((serialPort1.IsOpen) && (isDataProcessing)) ;  //wait previous data line processed done
                }
                catch (Exception errort)
                {
                    //MessageBox.Show(errort.ToString());
                    serialPort1.Close();
                    mens = "Error reading line from serial port";
                    err = errort;
                    this.Invoke(new EventHandler(logErrorThr));
                }
            }
        }

        /*  Filter received message before further use
         * */
        private void handleRxData(object sender, EventArgs e)
        {
            char[] charsToTrim = { '<', '>', '[', ']', ' ' };
            int tmp;

            // reset message
            if (rxString.IndexOf("['$' for help]") >= 0)
            {
                handleRX_Reset(rxString);
                timerSerial.Enabled = true;
                isDataProcessing = false;
                return;
            }

            else if (rxString.IndexOf("ok") >= 0)
            {   if (!isStreaming || isStreamingPause)
                {   if (!isHeightProbing || cbStatus.Checked)
                        addToLog(string.Format("< {0}", rxString)); }         // < ok
#if (debuginfo)
          //  rtbLog.AppendText(string.Format("> ok {0} {1} {2}\r\n", sendLinesSent, sendLinesConfirmed, sendLinesCount));//if not in transfer log the txLine
                rtbLog.AppendText(string.Format("< {0} {1} {2}  \r\n", sendLinesSent, sendLinesConfirmed, grblBufferFree));//if not in transfer log the txLine
#endif
                updateStreaming(rxString);                              // process all other messages
                isDataProcessing = false;
                return;
            }

            // Process status message with coordinates
            else if (((tmp=rxString.IndexOf('<')) >=0) && (rxString.IndexOf('>') > tmp)) 
            {   if (cbStatus.Checked)
                    addToLog(rxString);
                handleRX_Status(rxString.Trim(charsToTrim));// Process status message with coordinates
                isDataProcessing = false;
                return;
            }

            // Process feedback message with coordinates
            else if (((tmp = rxString.IndexOf('[')) >= 0) && (rxString.IndexOf(']') > tmp))
            {   handleRX_Feedback(rxString.Trim(charsToTrim).Split(':'));
                addToLog(rxString);
                isDataProcessing = false;
                return;
            }

            else if (rxString.IndexOf("ALARM") >= 0)
            {
                addToLog("<\r\n< !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\r\n<");
                addToLog(string.Format("< {0} \t{1}", rxString, grbl.getAlarm(rxString)));
                isDataProcessing = false;
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                return;
            }
            else if (rxString.IndexOf("error") >= 0)
            {
                addToLog("<\r\n< !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\r\n<");
                addToLog(string.Format("< {0} \t{1}", rxString, grbl.getError(rxString)));
                grblStatus = grblStreaming.error;
                if (isStreaming)
                {
                    addToLog(string.Format("< Error before code line {0} \r\n", gCodeLineNr[gCodeLinesSent]));
                    sendStreamEvent(gCodeLineNr[gCodeLinesSent], grblStatus);
                    isStreamingRequestPause = true;
                }
                isDataProcessing = false;
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                return;
            }

            // Show GRBL Settings Info if Version is >= 1.0
            else if ((rxString.IndexOf("$") >= 0) && (rxString.IndexOf("=") >= 0))
            {   handleRX_Setup(rxString);
                isDataProcessing = false;
                return;
            }

            isDataProcessing = false;
            return;
        }

        // process further RX messages (> ok)
        public void updateStreaming(string rxString)
        {
            int tmpIndex = gCodeLinesSent;
            // 'ok' received, increment confirmend
            if (sendLinesConfirmed < sendLinesCount)
            {
                grblBufferFree += (sendLines[sendLinesConfirmed].Length + 1);   //update bytes supose to be free on grbl rx bufer
                sendLinesConfirmed++;                   // line processed
                // Remove already sent lines to release memory
                if ((sendLines.Count > 1) && (sendLinesConfirmed == sendLinesSent == sendLinesCount > 1))
                {
                    sendLines.RemoveAt(0);
                    sendLinesConfirmed--;
                    sendLinesSent--;
                    sendLinesCount--;
                }
            }
            // check if buffer is empty and system = IDLE 
            if ((sendLinesConfirmed == sendLinesCount) && (grblStateNow == grblState.idle))   // addToLog(">> Buffer empty\r");
            {
                if (isStreamingRequestPause)
                {
                    isStreamingPause = true;
                    isStreamingRequestPause = false;
                    grblStatus = grblStreaming.pause;
                    gcodeVariable["MLAX"] = posMachine.X; gcodeVariable["MLAY"] = posMachine.Y; gcodeVariable["MLAZ"] = posMachine.Z;
                    gcodeVariable["WLAX"] = posWorld.X; gcodeVariable["WLAY"] = posWorld.Y; gcodeVariable["WLAZ"] = posWorld.Z;

                    if (getParserState)
                    { requestSend("$G"); }
                }
            }
            if (isStreaming)
            {
                if (!isStreamingPause)
                {
                    gCodeLinesConfirmed++;  //line processed
#if (debuginfo)
   //                 rtbLog.AppendText(string.Format("> ok {0} {1} {2}\r\n", gCodeLinesSent, gCodeLinesConfirmed, gCodeLinesCount));//if not in transfer log the txLine
                    rtbLog.AppendText(string.Format("> ok {0} {1} {2}  \r\n", sendLinesSent, sendLinesConfirmed, grblBufferFree));//if not in transfer log the txLine
#endif
                    // Remove already handled GCode lines to release memory
                    if ((gCodeLines.Count > 1) && (gCodeLinesSent > 1))
                    {
                        gCodeLines.RemoveAt(0);
                        gCodeLineNr.RemoveAt(0);
                        gCodeLinesConfirmed--;
                        gCodeLinesSent--;
                        gCodeLinesCount--;
                        tmpIndex = gCodeLinesSent;
                    }

                }
                //Transfer finished and processed? Update status and controls
                if ((gCodeLinesConfirmed >= gCodeLinesCount) && (sendLinesConfirmed == sendLinesCount))
                {
                    isStreaming = false;
                    addToLog("[Streaming finish]");
                    grblStatus = grblStreaming.finish;
                    if (isStreamingCheck)
                    { requestSend("$C"); isStreamingCheck = false; }
                    updateControls();
                    allowStreamingEvent = true;
                }
                else//not finished
                {
                    if (!(isStreamingPause || isStreamingRequestPause))
                        preProcessStreaming();//If more lines on file, send it  
                }
#if (debuginfo)
                addToLog("Line " + gCodeLineNr[gCodeLinesSent].ToString());
#endif
                if (allowStreamingEvent)    // allowed each 200ms to prevent too much events
                    sendStreamEvent(gCodeLineNr[gCodeLinesSent], grblStatus);
                allowStreamingEvent = false;
            }
            processSend();
        }

        /*  sendStreamEvent update main prog 
         * */
        private void sendStreamEvent(int lineNr, grblStreaming status)
        {
            float codeFinish = (float)lineNr * 100 / (float)gCodeLinesTotal;
            float buffFinish = (float)(grblBufferSize - grblBufferFree) * 100 / (float)grblBufferSize;
            if (codeFinish > 100) { codeFinish = 100; }
            if (buffFinish > 100) { buffFinish = 100; }
            OnRaiseStreamEvent(new StreamEventArgs((int)lineNr, codeFinish, buffFinish, status));
        }

        private void handleRX_Reset(string rxString)
        {
            sendLines.Clear();
            sendLinesCount = 0;
            sendLinesSent = 0;
            sendLinesConfirmed = 0;
            grblBufferSize = 127;  //rx bufer size of grbl on arduino 127
            grblBufferFree = grblBufferSize;
            addToLog("> RESET\r\n" + rxString);
            if (rxString.IndexOf("Grbl 0") >= 0)
            { isGrblVers0 = true; isLasermode = false; }
            if (rxString.IndexOf("Grbl 1") >= 0)
            { isGrblVers0 = false; }
            OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, grblStreaming.reset));
            lblSrBf.Text = "";
            lblSrFS.Text = "";
            lblSrPn.Text = "";
            lblSrLn.Text = "";
            lblSrOv.Text = "";
            lblSrA.Text = "";
            return;
        }

        private void handleRX_Feedback(string[] dataField)
        {
            if (dataField[0].IndexOf("GC") >= 0)
            {
                parserState = dataField[1];
                if (isGrblVers0)
                    parserState = parserState.Replace("M0 ", "");
                posPause = posWorld;
                getParserState = false;
            }
            else if (dataField[0].IndexOf("PRB") >= 0)                // Probe message with coordinates // [PRB:-155.000,-160.000,-28.208:1]
            {
                grblStateNow = grblState.probe;
                posProbeOld = posProbe;
                grbl.getPosition("PRB:" + dataField[1], ref posProbe);
                gcodeVariable["PRBX"] = posProbe.X; gcodeVariable["PRBY"] = posProbe.Y; gcodeVariable["PRBZ"] = posProbe.Z;
                gcodeVariable["PRDX"] = posProbe.X - posProbeOld.X; gcodeVariable["PRDY"] = posProbe.Y - posProbeOld.Y; gcodeVariable["PRDZ"] = posProbe.Z - posProbeOld.Z;
                OnRaisePosEvent(new PosEventArgs(posWorld, posMachine, grblStateNow, statusMsg, lastCmd));
            }
        }

        private void handleRX_Setup(string rxString)
        {
            string[] splt = rxString.Split('=');
            int id;
            if (int.TryParse(splt[0].Substring(1), out id))
            {
                if (!isGrblVers0)
                {
                    addToLog(string.Format("< {0} \t({1})", rxString, grbl.getSetting(id)));
                    if (id == 32)
                    {
                        if (splt[1].IndexOf("1") >= 0)
                            isLasermode = true;
                        else
                            isLasermode = false;
                        OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, grblStreaming.lasermode));
                    }
                }
                else
                    addToLog(string.Format("< {0}", rxString));
                GRBLSettings.Add(rxString);
            }
            else
                addToLog(string.Format("< {0}", rxString));
        }

        private grblState grblStateNow = grblState.unknown;
        private grblState grblStateLast = grblState.unknown;
        private string lastCmd = "";

        // should occur with same frequent as timer interrupt -> each 200ms
        // old:         <Idle,MPos:0.000,0.000,0.000,WPos:0.000,0.000,0.000>
        // new in 1.1   < Idle | MPos:0.000,0.000,0.000 | FS:0,0 | WCO:0.000,0.000,0.000 >
        private bool allowStreamingEvent = true;
        private void handleRX_Status(string text)    // '<' and '>' already removed
        {
            char splitAt = '|';
            if (isGrblVers0)
                splitAt = ',';
            string[] dataField = text.Split(splitAt);
            string status = dataField[0].Trim(' ');
            if (isGrblVers0)
            {   grbl.getPosition(dataField[1] + "," + dataField[2] + "," + dataField[3]+" ", ref posMachine);
                grbl.getPosition(dataField[4] + "," + dataField[5] + "," + dataField[6]+" ", ref posWorld);
            }
            else
            {
                if (dataField[1].IndexOf("MPos") >= 0)
                {
                    grbl.getPosition(dataField[1], ref posMachine);
                    posWorld.X = posMachine.X - posWCO.X;
                    posWorld.Y = posMachine.Y - posWCO.Y;
                    posWorld.Z = posMachine.Z - posWCO.Z;
                    posWorld.A = posMachine.A - posWCO.A;
                }
                else
                {
                    grbl.getPosition(dataField[1], ref posWorld);
                    posMachine.X = posWorld.X + posWCO.X;
                    posMachine.Y = posWorld.Y + posWCO.Y;
                    posMachine.Z = posWorld.Z + posWCO.Z;
                    posMachine.A = posWorld.A + posWCO.A;
                }

                statusMsg.Bf = ""; statusMsg.Ln = ""; statusMsg.FS = "";
                statusMsg.Pn = lblSrPn.Text = ""; statusMsg.Ov = ""; statusMsg.A = lblSrA.Text = "";
                if (dataField.Length > 2)
                {
                    for (int i = 2; i < dataField.Length; i++)
                    {
                        if (dataField[i].IndexOf("WCO") >= 0)           // Work Coordinate Offset
                        {
                            grbl.getPosition(dataField[i], ref posWCO);
                            continue;
                        }
                        string[] data = dataField[i].Split(':');
                        if (dataField[i].IndexOf("Bf:") >= 0)            // Buffer state
                        { statusMsg.Bf=lblSrBf.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("Ln:") >= 0)            // Line number
                        { statusMsg.Ln=lblSrLn.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("FS:") >= 0)            // Current Feed and Speed
                        { statusMsg.FS=lblSrFS.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("F:") >= 0)             // Current Feed 
                        { statusMsg.FS=lblSrFS.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("Pn:") >= 0)            // Input Pin State
                        { statusMsg.Pn=lblSrPn.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("Ov:") >= 0)            // Override Values
                        { statusMsg.Ov=lblSrOv.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("A:") >= 0)             // Accessory State
                        { statusMsg.A=lblSrA.Text = data[1]; continue; }
                    }
                }
            }
            gcodeVariable["MACX"] = posMachine.X; gcodeVariable["MACY"] = posMachine.Y; gcodeVariable["MACZ"] = posMachine.Z;
            gcodeVariable["WACX"] = posWorld.X; gcodeVariable["WACY"] = posWorld.Y; gcodeVariable["WACZ"] = posWorld.Z;
            grblStateNow = grbl.parseStatus(status);
            lblSrState.BackColor = grbl.grblStateColor(grblStateNow);
            lblSrState.Text = status;
            if (ctrl4thUse)
                lblSrPos.Text = string.Format("X={0:0.000} Y={1:0.000} Z={2:0.000} {3}={4:0.000}", posWorld.X, posWorld.Y, posWorld.Z, ctrl4thName, posWorld.A);
            else
                lblSrPos.Text = string.Format("X={0:0.000} Y={1:0.000} Z={2:0.000}", posWorld.X, posWorld.Y, posWorld.Z);

            if (grblStateNow != grblStateLast) { grblStateChanged(); }
            if (grblStateNow == grblState.idle)
            {   if (useSerial2 && _serial_form2.serialPortOpen)
                {
                    if (_serial_form2.grblStateNow == grblState.idle)
                        waitForIdle = false;
                    else
                        grblStateNow = _serial_form2.grblStateNow;
                }
                else
                    waitForIdle = false;
                processSend();
            }
            grblStateLast = grblStateNow;
            OnRaisePosEvent(new PosEventArgs(posWorld, posMachine, grblStateNow, statusMsg, lastCmd));
            allowStreamingEvent = true;
        }
        public event EventHandler<PosEventArgs> RaisePosEvent;
        protected virtual void OnRaisePosEvent(PosEventArgs e)
        {   EventHandler<PosEventArgs> handler = RaisePosEvent;
            if (handler != null)
            {
                handler(this, e);
            }
            lastCmd = "";
        }

        #endregion


        // check free buffer before sending 
        // 1. requestSend(data) to add cleaned data to stack (sendLines) for sending / extract code for 2nd COM port
        // 2. processSend() check if grbl-buffer is free to take commands
        // 3. sendLine(data) if buffer can take commands
        // 4. updateStreaming(rxdata) check if command was sent
        private int grblBufferSize = 127;  //rx bufer size of grbl on arduino 127
        private int grblBufferFree = 127;    //actual suposed free bytes on grbl buffer
        private List<string> sendLines = new List<string>();
        private int sendLinesCount=0;             // actual buffer size
        private int sendLinesSent=0;              // actual sent line
        private int sendLinesConfirmed=0;         // already received line

        /*  requestSend fill up send buffer, called by main-prog for single commands
         *  or called by preProcessStreaming to steam GCode data
         * */
        public void requestSend(string data)
        {
            if (isStreamingRequestPause)
            {   addToLog("!!! Command blocked - wait for IDLE " + data); }
            else
            {
                var tmp = cleanUpCodeLine(data);
                if ((!string.IsNullOrEmpty(tmp)) && (tmp[0] != ';'))//trim lines and remove all empty lines and comment lines
                {
                    sendLines.Add(tmp); // cleanUpCodeLine(data));
                    sendLinesCount++;
                    processSend();
                }
            }
        }

        private bool replaceFeedRate = false;
        private bool replaceSpindleSpeed = false;
        private string replaceFeedRateCmd = "";
        private string replaceSpindleSpeedCmd = "";
        private string replaceFeedRateCmdOld = "";
        private string replaceSpindleSpeedCmdOld = "";
        // called by MainForm -> get override events from form "StreamingForm" for GRBL 0.9
        public void injectCode(string cmd, int value, bool enable)
        {
            if (cmd == "F")
            {   replaceFeedRate = enable;
                replaceFeedRateCmd = cmd + value.ToString();
                if (isStreaming)
                {
                    if (enable)
                        injectCodeLine(replaceFeedRateCmd);
                    else
                    {
                        if (replaceFeedRateCmdOld != "")
                            injectCodeLine(replaceFeedRateCmdOld);
                    }
                }
            }
            if (cmd == "S")
            {
                replaceSpindleSpeed = enable;
                replaceSpindleSpeedCmd = cmd + value.ToString();
                if (isStreaming)
                {
                    if (enable)
                        injectCodeLine(replaceSpindleSpeedCmd);
                    else
                    {
                        if (replaceSpindleSpeedCmdOld != "")
                            injectCodeLine(replaceSpindleSpeedCmdOld);
                    }
                }
            }
        }
        private void injectCodeLine(string data)
        {
            int index = gCodeLinesSent + 1;
            int linenr = gCodeLineNr[gCodeLinesSent];
            addToLog("!!! Override: " + data + " in line "+linenr);
            gCodeLineNr.Insert(index, linenr);
            gCodeLines.Insert(index, data);
            index++;
            gCodeLinesCount++;
        }

        /*  cleanUpCodeLine remove unneccessary char but keep keywords
        */
        private string cleanUpCodeLine(string data)
        {
            var line = data.Replace("\r", "");  //remove CR
            line = line.Replace("\n", "");      //remove LF
            var orig = line;
            int start = orig.IndexOf('(');
            int end = orig.LastIndexOf(')');
            if (start >= 0) line = orig.Substring(0, start);
            if (end >= 0) line += orig.Substring(end + 1);

            // extract GCode for 2nd COM Port
            if ((start >= 0) && (end > start))  // send data to 2nd COM-Port
            {   var cmt = orig.Substring(start, end - start + 1);
                if ((cmt.IndexOf("(^2") >= 0) || (cmt.IndexOf("(#") == 0))
                {   line += cmt;                // keep 2nd COM port data for further use
                }
            }

            line = line.ToUpper();              //all uppercase
            line = line.Trim();
            return line;
        }

        /*  processSend - send data if GRBL-buffer is ready to take new data
         *  called by timer and rx-interrupt
         *  take care of keywords
         * */
        private bool waitForIdle = false;
        public void processSend()
        {   while ((sendLinesSent < sendLinesCount) && (grblBufferFree >= sendLines[sendLinesSent].Length + 1))
            {
                var line = sendLines[sendLinesSent];
                bool replaced = false;
                if (line.IndexOf('@') > 0)              // check if variable neededs to be replaced
                { line = insertVariable(line);
                    replaced = true;
                    if (grblBufferFree < grblBufferSize)
                        waitForIdle = true;
                }
                if (line.IndexOf("(^2") >= 0)
                    if (grblBufferFree < grblBufferSize)
                        waitForIdle = true;

                if (!waitForIdle)
                {   if (replaced)
                        sendLines[sendLinesSent] = line;    // needed to get correct length when receiving 'ok'
                                                            //                    rtbLog.AppendText(string.Format("!!!> {0} {1}\r\n", line, sendLinesSent));
                    if (serialPort1.IsOpen)
                    {
                        //System.Threading.Thread.Sleep(200);
                        sendLine(line);                         // now really send data to Arduino
                        grblBufferFree -= (line.Length + 1);
                        sendLinesSent++;
                    }
                    else
                    {
                        addToLog("!!! Port is closed !!!");
                        isStreaming = false;
                        isStreamingRequestPause = false;
                        isStreamingPause = false;
                        gCodeLinesSent = 0;
                        gCodeLinesCount = 0;
                        gCodeLinesConfirmed = 0;
                        gCodeLinesTotal = 0;
                        sendLinesSent = 0;
                        sendLinesCount = 0;
                        sendLinesConfirmed = 0;
                        gCodeLines.Clear();
                        gCodeLineNr.Clear();
                    }


                    if (line.IndexOf("(^2") >= 0)
                    {   int start = line.IndexOf('(');
                        int end = line.LastIndexOf(')');
                        if ((start >= 0) && (end > start))  // send data to 2nd COM-Port
                        {
                            var cmt = line.Substring(start, end - start + 1);
                            if (useSerial2)
                            {
                                _serial_form2.requestSend(cmt.Substring(start + 3, cmt.Length - 4));
                                waitForIdle = true;
                            }
                        }
                    }
                    if (line == "(#TOOL-IN)") { toolInSpindle = true; }
                    if (line == "(#TOOL-OUT)") { toolInSpindle = false; }
                }
                else
                    return;
            }
        }

        private string insertVariable(string line)
        {
            int pos = 0, posold = 0;
            string variable, mykey = "";
            double myvalue = 0;
            do
            {
                pos = line.IndexOf('@', posold);
                if (pos > 0)
                {
                    myvalue = 0;
                    variable = line.Substring(pos, 5);
                    mykey = variable.Substring(1);
                    if (gcodeVariable.ContainsKey(mykey))
                    { myvalue = gcodeVariable[mykey]; }
                    else { line += " (" + mykey + " not found)"; }
                    line = line.Replace(variable, string.Format("{0:0.000}", myvalue));
                }
                posold = pos + 5;
            } while (pos > 0);
            return line.Replace(',', '.');
        }


        public void realtimeCommand(int cmd)
        {
            var dataArray = new byte[] { Convert.ToByte(cmd) };
            serialPort1.Write(dataArray, 0, 1);
            addToLog("> '0x"+cmd.ToString("X")+"' "+grbl.getRealtime(cmd));
        }
        /*  sendLine - now really send data to Arduino
         * */
        private void sendLine(string data)
        {   try
            {   serialPort1.Write(data + "\r");
#if (debuginfo)
                rtbLog.AppendText(string.Format("< {0} {1} {2} {3} \r\n", data, sendLinesSent, sendLinesConfirmed, grblBufferFree));//if not in transfer log the txLine
#endif
                if (!isHeightProbing && (!(isStreaming && !isStreamingPause)) || (cbStatus.Checked))
                {   rtbLog.AppendText(string.Format("> {0} \r\n", data));//if not in transfer log the txLine
                    rtbLog.ScrollToCaret();
                }
                if ((!isStreamingCheck)&&(data.IndexOf("(^2") < 0) && ((data.IndexOf("M") >= 0) || (data.IndexOf("F") >= 0) || (data.IndexOf("S") >= 0)))
                {   lastCmd += data + " ";
//                    addToLog(lastCmd);
                }
            }
            catch (Exception err)
            {   logError("Sending line", err);
                updateControls();
            }
        }


        // Streaming
        // 1. startStreaming() copy and filter gcode to list
        // 2. proceedStreaming() to copy data to stack for sending
        private List<string> gCodeLines = new List<string>();      // buffer with gcode commands
        private List<int> gCodeLineNr   = new List<int>();         // corresponding line-nr from main-form
        private int gCodeLinesCount=0;             // amount of lines to sent
        private int gCodeLinesSent=0;              // actual sent line
        private int gCodeLinesConfirmed=0;         // received line
        private int gCodeLinesTotal=0;
        private bool isStreaming = false;        // true when steaming is in progress
        private bool isStreamingRequestPause = false; // true when request pause (wait for idle to switch to pause)
        private bool isStreamingPause = false;    // true when steaming-pause 
        private bool isStreamingCheck = false;    // true when steaming is in progress (check)
        private bool getParserState = false;      // true to send $G after status switched to idle
        private bool isDataProcessing=false;      // false when no data processing pending
        private grblStreaming grblStatus = grblStreaming.ok;
        public void stopStreaming()
        {
            int line = 0;
            if ((gCodeLineNr != null) && (gCodeLinesSent < gCodeLineNr.Count))
            {
                line = gCodeLineNr[gCodeLinesSent];
                sendStreamEvent(line, grblStreaming.stop);
            }
            isHeightProbing = false;
            isStreaming = false;
            isStreamingRequestPause = false;
            isStreamingPause = false;
            addToLog("[STOP Streaming ("+line.ToString()+")]");
            gCodeLinesSent = 0;
            gCodeLinesCount = 0;
            gCodeLinesConfirmed = 0;
            gCodeLinesTotal = 0;
            gCodeLines.Clear();
            gCodeLineNr.Clear();
            sendLinesCount = 0;
            sendLinesSent = 0;
            sendLinesConfirmed = 0;
            sendLines.Clear();
            grblBufferFree = grblBufferSize;
            if (isStreamingCheck)
            {   sendLine("$C");
                isStreamingCheck = false;
            }
        }
        public void pauseStreaming()
        {   if (!isStreamingPause)
            {   isStreamingRequestPause = true;     // wait until buffer is empty before switch to pause
                addToLog("[Pause streaming]");
                addToLog("[Save Settings]");
                grblStatus = grblStreaming.waitidle;
                getParserState = true;
            }
            else
            {   if ((posPause.X != posWorld.X) || (posPause.Y != posWorld.Y) || (posPause.Z != posWorld.Z))
                {
                    addToLog("[Restore Position]");
                    requestSend(string.Format("G90 G0 X{0:0.0} Y{1:0.0}", posPause.X, posPause.Y).Replace(',', '.'));  // restore last position
                    requestSend(string.Format("G1 Z{0:0.0}", posPause.Z).Replace(',', '.'));                      // restore last position
                }
                addToLog("[Restore Settings]");
                isStreamingPause = false;
                isStreamingRequestPause = false;
                grblStatus = grblStreaming.ok;
/*                var cmds = parserState.Split(' ');
                foreach (var cmd in cmds)
                {
                    if (cmd != "M0")
                    {
                        requestSend(cmd);            // restore actual GCode settings one by one
                        gCodeLinesConfirmed--;           // each restored setting will cause 'ok' and gCodeLinesConfirmed++
                    }
                }
*/              requestSend(parserState);            // restore actual GCode settings one by one
                gCodeLinesConfirmed--;           // each restored setting will cause 'ok' and gCodeLinesConfirmed++

                addToLog("[Start streaming - no echo]");
                preProcessStreaming();
            }
        }

        /*  startStreaming called by main-Prog
         *  get complete GCode list and copy to own list
         *  initialize streaming
         * */
        public void startStreaming(IList<string> gCodeList, bool check = false)
        {
            rtbLog.Clear();
            if (!check)
                addToLog("[Start streaming - no echo]");
            else
                addToLog("[Start code check]");
            saveLastPos();
            if (replaceFeedRate)
                addToLog("!!! Override Feed Rate");
            if (replaceSpindleSpeed)
                addToLog("!!! Override Spindle Speed");
            isStreamingPause = false;
            isStreamingCheck = check;
            grblStatus = grblStreaming.ok;
            string[] gCode = gCodeList.ToArray<string>();
            gCodeLinesSent = 0;
            gCodeLinesCount = 0;
            gCodeLinesConfirmed = 0;
            gCodeLinesTotal = 0;
            gCodeLines = new List<string>();
            gCodeLineNr = new List<int>();
            gCodeLines.Clear();
            gCodeLineNr.Clear();
            sendLinesSent = 0;
            sendLinesCount = 0;
            sendLinesConfirmed = 0;
            sendLines.Clear();
            if (isStreamingCheck)
            {   sendLine("$C");
                grblBufferSize = 100;  //reduce size to avoid fake errors
            }
            grblBufferFree = grblBufferSize;

            string tmp;
            double pWord,lWord, oWord;
            string subline;
            for (int i = 0; i < gCode.Length; i++)
            {
                tmp = cleanUpCodeLine(gCode[i]);
                if ((!string.IsNullOrEmpty(tmp)) && (tmp[0] != ';'))//trim lines and remove all empty lines and comment lines
                {
                    if (tmp.IndexOf("M98") >= 0)    // any subroutines?
                    {   pWord = findDouble("P", -1, tmp);
                        lWord = findDouble("L", 1, tmp);
                        int subStart = 0, subEnd = 0;
                        if (pWord > 0)
                        {   oWord = -1;
                            for (int si = i; si < gCode.Length; si++)   // find subroutine
                            {
                                subline = gCode[si];
                                if (subline.IndexOf("O") >= 0)          // find O-Word
                                {   oWord = findDouble("O", -1, subline);
                                    if (oWord == pWord)
                                        subStart = si + 1;              // note start of sub
                                }
                                else                                    // find end of sub
                                {   if (subStart > 0)                   // is match?
                                    {   if (subline.IndexOf("M99") >= 0)
                                        { subEnd = si; break; }     // note end of sub
                                    }
                                }
                            }
                            //MessageBox.Show("start " + subStart.ToString()+" end "+ subEnd.ToString());
                            if (subStart < subEnd)
                            {   for (int repeat = 0; repeat < lWord; repeat++)
                                {
                                    for (int si = subStart; si < subEnd; si++)   // copy subroutine
                                    {
                                        gCodeLines.Add(gCode[si]);          // add gcode line to list to send
                                        gCodeLineNr.Add(si);                // add line nr
                                        gCodeLinesCount++;                  // Count total lines
                                    }
                                }
                            }                                                           
                        }
                    }
                    else
                    {
                        gCodeLines.Add(tmp);        // add gcode line to list to send
                        gCodeLineNr.Add(i);         // add line nr
                        gCodeLinesCount++;          // Count total lines
                        if (tmp.IndexOf("M30")>=0)
                            break;
                    }
                }
            }
            gCodeLines.Add("()");        // add gcode line to list to send
            gCodeLineNr.Add(gCode.Length-1);         // add line nr
            gCodeLinesTotal = gCode.Length - 1;  // gCodeLinesCount will reduced after each 'confirmed' line
            isStreaming = true;
            preProcessStreaming();
        }
        private static double findDouble(string start, double notfound, string txt)
        {   int istart = txt.IndexOf(start);
            if (istart < 0)
                return notfound;
            string line = txt.Substring(istart+start.Length);
            string num = "";
            foreach (char c in line)
            {
                if (Char.IsLetter(c))
                    break; 
                else if (Char.IsNumber(c) || c == '.' || c == '-')
                    num += c;
            }
            return double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        /*  preProcessStreaming copy line by line (requestSend(line)) to sendBuffer 
         *  if buffer free, to be able to track line-nr for feedback
         * */
        int currentTool = -1;
        private void preProcessStreaming()
        { while ((gCodeLinesSent < gCodeLinesCount) && (grblBufferFree >= gCodeLines[gCodeLinesSent].Length + 1) && !waitForIdle)
            {
                string line = gCodeLines[gCodeLinesSent];
                int cmdMNr = gcode.getIntGCode('M',line);
                int cmdGNr = gcode.getIntGCode('G',line);
                if (grbl.unknownG.Contains(cmdGNr))
                {
                    gCodeLines[gCodeLinesSent] = "(" + line + " - unknown)";  // don't pass unkown GCode to GRBL because is unknown
                    line = gCodeLines[gCodeLinesSent];
                    gCodeLinesConfirmed++;      // GCode is count as sent (but wasn't send) also count as received
                    addToLog(line);
                }
                if ((replaceFeedRate) && (gcode.getStringValue('F', line) !=""))
                {   string old_value = gcode.getStringValue('F', line);
                    replaceFeedRateCmdOld = old_value;
                    line = line.Replace(old_value, replaceFeedRateCmd);
                    gCodeLines[gCodeLinesSent] = line;
//                    addToLog("Replace feed in [" + line + "] old : " + old_value);
                }
                if ((replaceSpindleSpeed) && (gcode.getStringValue('S', line) != ""))
                {
                    string old_value = gcode.getStringValue('S', line);
                    line = line.Replace(old_value, replaceSpindleSpeedCmd);
                    replaceSpindleSpeedCmdOld = old_value;
                    gCodeLines[gCodeLinesSent] = line;
//                    addToLog("Replace spindle speed in [" + line + "] old : " + old_value);
                }

                if (line.IndexOf("T") >= 0)
                {
                    int start = line.IndexOf("T");
                    int num;
                    string snum = line.Substring(start + 1);
                    if (snum.Length > 2) { snum = snum.Substring(0, 2); }
                    int.TryParse(snum, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out num);
                    gcodeVariable["TOOL"] = num;
                }
                if (cmdMNr == 6)
                {
                    if (Properties.Settings.Default.ctrlToolChange)
                    {   // insert script code into GCODE
                        int index = gCodeLinesSent + 1;
                        int linenr = gCodeLineNr[gCodeLinesSent];
                        addToLog("[TOOL change: " + gcodeVariable["TOOL"].ToString() + " ]");
                        if (toolInSpindle)
                        {   index = insertCode(Properties.Settings.Default.ctrlToolScriptPut, index, linenr);
                            index = insertComment(index, linenr, "(#TOOL-OUT)");
                        }
                        index = insertCode(Properties.Settings.Default.ctrlToolScriptSelect,index, linenr,true);
                        index = insertCode(Properties.Settings.Default.ctrlToolScriptGet,   index, linenr);
                        index = insertComment(index, linenr, "(#TOOL-IN)");
                        index = insertCode(Properties.Settings.Default.ctrlToolScriptProbe, index, linenr);
                        index = insertComment(index, linenr, "(#END)");

                        currentTool = (int)gcodeVariable["TOOL"];
                        grblStatus = grblStreaming.toolchange;
                        sendStreamEvent(gCodeLineNr[gCodeLinesSent], grblStatus);
                    }
                    gCodeLines[gCodeLinesSent] = "(#" + line + ")";  // don't pass M6 to GRBL because is unknown
                    line = gCodeLines[gCodeLinesSent];
                    gCodeLinesConfirmed++;      // M6 is count as sent (but wasn't send) also count as received
                }
                if (line == "(#END)")
                {
                    grblStatus = grblStreaming.ok;
                    sendStreamEvent(gCodeLineNr[gCodeLinesSent], grblStatus);
                }

                if ((cmdMNr == 0) && !isStreamingCheck)
                {
                    isStreamingRequestPause = true;
                    addToLog("[Pause streaming]");
                    addToLog("[Save Settings]");
                    grblStatus = grblStreaming.waitidle;
                    getParserState = true;
                    sendStreamEvent(gCodeLineNr[gCodeLinesSent], grblStatus);
                    gCodeLinesSent++;
                    return;                 // abort while - don't fill up buffer
                }
                requestSend(line);
                gCodeLinesSent++;
            }
        }

        private int insertComment(int index, int linenr, string cmt)
        {
            gCodeLineNr.Insert(index, linenr);
            gCodeLines.Insert(index, cmt);
            index++;
            gCodeLinesCount++;
            return index;
        }
        private int insertCode(string file, int index, int linenr, bool replace=false)
        {
            if (File.Exists(file))
            {
                string fileCmd = File.ReadAllText(file);
                string[] commands = fileCmd.Split('\n');
                string tmp;
                foreach (string cmd in commands)
                {
                    tmp = cleanUpCodeLine(cmd);
                    if (tmp.Length > 0)
                    {
                        gCodeLineNr.Insert(index,linenr);
                        if (replace)
                            gCodeLines.Insert(index, tmp.Replace("@TOOL", string.Format("{0:0}", gcodeVariable["TOOL"])));
                        else
                            gCodeLines.Insert(index, tmp);
                        index++;
                        gCodeLinesCount++;
                    }
                }
            }
            return index;
        }


        private void grblStateChanged()
        { if ((sendLinesConfirmed == sendLinesCount) && (grblStateNow == grblState.idle))   // addToLog(">> Buffer empty\r");
            {
                if (isStreamingRequestPause)
                {
                    isStreamingPause = true;
                    isStreamingRequestPause = false;
                    grblStatus = grblStreaming.pause;
                    if (getParserState)
                    { requestSend("$G"); }
                }
            }
        }

        public event EventHandler<StreamEventArgs> RaiseStreamEvent;
        protected virtual void OnRaiseStreamEvent(StreamEventArgs e)
        {
            EventHandler<StreamEventArgs> handler = RaiseStreamEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        { rtbLog.Clear(); }
        private void tbCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) return;
            btnSend_Click(sender, e);
        }
        private void btnSend_Click(object sender, EventArgs e)
        {   if (!isStreaming)
            {
                string cmd = cBCommand.Text;
                cBCommand.Items.Remove(cBCommand.SelectedItem);
                cBCommand.Items.Insert(0, cmd);
                requestSend(cmd);
                cBCommand.Text = cmd;
                if (cmd.IndexOf("$32") >= 0)
                {
                    if (cmd.IndexOf("=") >= 0) isLasermode = true;
                    if (cmd.IndexOf("0") >= 0) isLasermode = false;
                    OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, grblStreaming.lasermode));
                }
            }
        }
        private void btnGRBLCommand0_Click(object sender, EventArgs e)
        { requestSend("$"); }
        private void btnGRBLCommand1_Click(object sender, EventArgs e)
        { requestSend("$$"); GRBLSettings.Clear(); }
        private void btnGRBLCommand2_Click(object sender, EventArgs e)
        { requestSend("$#"); }
        private void btnGRBLCommand3_Click(object sender, EventArgs e)
        { requestSend("$N"); }
        private void btnGRBLCommand4_Click(object sender, EventArgs e)
        { requestSend("$X"); }
        private void btnGRBLReset_Click(object sender, EventArgs e)
        { grblReset(); }
    }
}
