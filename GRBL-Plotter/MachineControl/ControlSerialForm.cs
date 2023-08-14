/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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
/* 2016-07-xx add 2nd serial port for tool changer
 * 2016-09-17 improve performance by removing already sent lines from sendLines[] and gCodeLines[]
 *            Remove unknown G-Codes in preProcessStreaming() (list in grblRelated)
 * 2016-09-25 Implement override function
 * 2016-09-26 reduce  grblBufferSize to 100 during $C check gcode to reduce fake errors
 * 2016-12-31 add GRBL 1.1 compatiblity, clean-up
 * 2017-01-01 check form-location and fix strange location
 * 2018-01-02 Bugfix route errors during streaming from serialform to gui
 * 2018-04-05 Code clean up
 * 2018-07-27 change key-signs : variable: old:@, new:#   internal sign old:#, new:$
 * 2018-12-26 Commits from RasyidUFA via Github
 * 2019-01-12 print last sent commands to grbl after error as info
 * 2019-08-13 line 833 check array length
 * 2019-08-15 add logger
 *            line 528 replace .Invoke by .BeginInvoke to avoid deadlock
 * 2019-09-24 line 410 add serialPort.DtrEnable = true;serialPort.DtrEnable = false; sequence - thanks to ivahru
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2019-10-27 localization of strings
 * 2019-10-31 add SerialPortFixer http://zachsaw.blogspot.com/2010/07/serialport-ioexception-workaround-in-c.html
 * 2019-12-07 add additional log-info during streamning cBStatus1
 * 2020-01-04 add "errorBecauseOfBadCode" line 652
 * 2020-01-22 remove hard reset because of missing status reports
 * 2020-04-02 extend hard-reset dtr-low time, improove rx,tx data display, check 30kHz after reset
 * 2020-07-06 add tool change a axis
 * 2020-08-08 #144
 * 2020-09-06 Missing xx Real-time Status Reports per 10 seconds during MessageBox for Tool exchange
 *            change to System.Timers.Timer
 * 2020-09-18 split file
 * 2020-12-27 add Marlin support
 * 2021-01-15 add 3rd serial com and lineEndRXTX
 * 2021-01-23 add trgEvent to "sendStreamEvent" in time with the status query
 * 2021-04-27 IOEception add more closings line 333+
 * 2021-07-14 code clean up / code quality
 * 2021-09-29 reduce polling frequency on missing reports line 376
 * 2021-10-24 handle serial port System.TimeoutException -> close port
 * 2021-12-13 replace serialPort.Write by SerialPortDataSend
 * 2021-12-14 add run time for spindle, flood, mist
 * 2021-12-22 add Grbl.isConnected
 * 2022-01-04 change readtimeout from 500 to 1000
 * 2022-01-07 OnKeyDown add try/catch
 * 2022-04-08 line 463 remove if
 * 2023-03-29 l:930 f:SerialPort1_DataReceived add option to use Invoke -> lowLevelPerformance
 * 2023-03-31 l:266 f:SerialForm_FormClosing improove shut-down behavior
 * 2023-06-28 l:665 f:AddToLog limit amount of lines to 10000   replaced RichTextBox by TextBox
 * 2023-07-27 l:269 f:SerialForm_FormClosing  set grblCharacterCounting = true;   // may helps to avoid locking the form
*/

// OnRaiseStreamEvent(new StreamEventArgs((int)lineNr, codeFinish, buffFinish, status));
// OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlSerialForm : Form       // Form can be loaded twice!!! COM1, COM2
    {

        ControlSerialForm _serial_form2;
        SimpleSerialForm _serial_form3;

        public bool SerialPortOpen { get; private set; } = false;
        private bool SerialPortOpenLast = false;
        private bool serialPortError = false;

        private readonly System.Timers.Timer timerSerial = new System.Timers.Timer();

        private readonly int iamSerial = 1;
        private int axisCount = 0;
        private int rxErrorCount = 0;
        //		private int idleInterval = 500;		// timer intervall in IDLE state

        private string rxString;
        private string actualPort = "";
        private readonly string formTitle = "";
        private string strCheckResult = "";		// result from grbl-setup check

        internal string lastError = "";
        private int countGrblError = 0;
        private bool flag_closeForm = false;
        private bool tryDoSerialConnection = false;	// during connection setup show dots


        private bool useSerial2 = false;
        private bool serial2Busy = false;
        private bool useSerial3 = false;
        private bool serial3Busy = false;
        private bool useEthernet = false;

        // Note: receive-event waits for line-end char, if missing -> timeout exception
        private const string lineEndRX = "\n";      // read - grbl accepts '\n' or '\r' and sends "\r\n", but Marlin sends '\n'
        private const string lineEndTXgrbl = "\r";    // send - grbl accepts '\n' or '\r' and sends "\r\n", but Marlin sends '\n'
        private const string lineEndTXmarlin = "\n";    // send - grbl accepts '\n' or '\r' and sends "\r\n", but Marlin sends '\n'

        /* counters will be decreased in timer event - action on 0 */
        private int countMinimizeForm = 0;              // timer to minimize form
        private int countMissingStatusReport = 20;      // count missing status reports after sending ?
        private uint countCallCheckGRBL = 0;             // timer to check result after start check
        private uint countStateReset = 10;               // 
        private uint countPreventOutput = 0;             // prevent log output e.g. during startup $$
        private uint countPreventEvent = 0;              // prevent event
        private int countPreventIdle = 0;               // delay Idle proccessing
        private uint countPreventIdle2nd = 0;            // delay Idle proccessing
        private uint countShutdown = 0;
        private uint countPreventInterlock = 100;
        private uint countPreventWaitForOkLock = 5;
        private int countLoggerUpdate = 0;

        private bool isMarlin = false;
        private bool updateMarlinPosition = false;
        private bool getMarlinPositionWasSent = false;
    //    private readonly int insertMarlinCounterReload = 5;
    //    private int insertMarlinCounter = 5;

        private bool lowLevelPerformance = false;// Properties.Settings.Default.grblPollIntervalReduce

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //        private static readonly NLog.Logger LogPos = NLog.LogManager.GetLogger("LogPos");

        private static bool logEnable = false;
        private static bool logReceiveStatus = false;	// send '?' get '< Idle | MPos:0.000,0.000,0.000 | FS:0,0 | WCO:0.000,0.000,0.000 >'
        private static bool logReceive = false;			// any other RX data
        private static bool logTransmit = false;            // TX data
        private static bool logStartStop = false;
        private static readonly bool log2ndGrbl = false;
        private static readonly bool log3rdCOM = false;
        private static bool logStreamData = false;

        private const string logFileGCode = "logStreamGCode.nc";
        private const string logFileSentData = "logSendBuffer.nc";
        private const string logFileEcho = "logGrblEcho.nc";

        private static void UpdateLogging()
        {	// LogEnable { Level1=1, Level2=2, Level3=4, Level4=8, Detailed=16, Coordinates=32, Properties=64, Sort = 128, GroupAllGraphics = 256, ClipCode = 512, PathModification = 1024 }
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level4) > 0);
            logReceiveStatus = logEnable && ((logFlags & (uint)LogEnables.Detailed) > 0);
            logReceive = logEnable && ((logFlags & (uint)LogEnables.Coordinates) > 0);
            logTransmit = logEnable && ((logFlags & (uint)LogEnables.Properties) > 0);
            logStartStop = logEnable && ((logFlags & (uint)LogEnables.Sort) > 0);
            logStreamData = Properties.Settings.Default.guiExtendedLoggingCOMEnabled;
        }

        public ControlSerialForm(string txt, int nr, ControlSerialForm handle2 = null, SimpleSerialForm handle3 = null)
        {
            iamSerial = nr;
            UpdateLogging();
            Logger.Info("====== SerialForm {0} {1} START ======", iamSerial, txt);
            formTitle = txt;
            Set2ndSerial(handle2);
            Set3rdSerial(handle3);
            InitializeComponent();
            mParserState.Reset();
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            this.Icon = Properties.Resources.Icon;
            this.Invalidate();

            timerSerial.Elapsed += TimerSerial_Tick;
            timerSerial.Interval = 1000;
        }
        public void Set2ndSerial(ControlSerialForm handle)
        {
            _serial_form2 = handle;
            if (handle != null)
            {
                useSerial2 = true;
                if (log2ndGrbl) Logger.Trace("set2ndSerial {0}", handle);
            }
            else
                useSerial2 = false;
        }
        public void Set3rdSerial(SimpleSerialForm handle)
        {
            _serial_form3 = handle;
            if (handle != null)
            {
                useSerial3 = true;
                if (log3rdCOM) Logger.Trace("set3rdSerial {0}", handle);
            }
            else
                useSerial3 = false;
        }

        private bool ExternalCOMReady()
        {   // !isStreaming added 2021-04-07 
            if (!isStreaming || (!useSerial2 && !useSerial3))
                return true;
            bool c2 = true, c3 = true;
            if (useSerial2) c2 = !serial2Busy && (_serial_form2.grblStateNow == GrblState.idle);
            if (useSerial3) c3 = !serial3Busy && !_serial_form3.Busy;
            return c2 && c3;
        }

        private void SerialForm_Load(object sender, EventArgs e)
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            SerialForm_Resize(sender, e);
            machineState.Clear();
            UpdateControls();       // disable controls
            LoadSettings();         // set last COM and Baud
            RefreshPorts();         // scan for COMs
            useEthernet = CbEthernetUse.Checked;
            if (!useEthernet)
                this.BeginInvoke(new EventHandler(ConnectToGrbl));  //    ConnectToGrbl(false);    // OpenPortSerial();             // open COM
            else
                AddToLog("No auto-connect for ethernet");

            if (iamSerial == 1)
            {
                Location = Properties.Settings.Default.locationSerForm1;
                Size = Properties.Settings.Default.sizeSerForm1;
                if ((Size.Width < 342) || (Size.Height < 480))
                { Size = new Size(342, 480); }
            }
            else
            { Location = Properties.Settings.Default.locationSerForm2; }
            Text = formTitle;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { this.CenterToScreen(); }    // Location = new Point(100, 100);    }
            IsLasermode = Properties.Settings.Default.ctrlLaserMode;
            ResetVariables(true);
            if (Properties.Settings.Default.serialMinimize)
                AddToLog("\nForm will be reduced after connecting");

            timerSerial.Enabled = true;
            timerSerial.Start();
        }

        private void SerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("====== Try closing SerialForm {0} {1}", iamSerial, e.CloseReason);
            if ((e.CloseReason.ToString() == "ApplicationExitCall") || (e.CloseReason.ToString() == "FormOwnerClosing"))
            {
            //    isDataProcessing = false;
                grblCharacterCounting = true;   // may helps to avoid locking the form

                if (countShutdown == 0)
                {
                    timerSerial.Stop();
                    countShutdown = 5;
                    StopStreaming(true);    // isNotStartup = true
				//	lowLevelPerformance = false;	// hope to reduce shut-down time 2023-03-31
                    serialPort.DataReceived -= (this.SerialPort1_DataReceived); // stop receiving data
                                                                                //    JustgrblReset();    // nötig?
                    timerSerial.Interval = 1000;
                    timerSerial.Start();
                    if (lowLevelPerformance)
                        Close();
                    DisconnectFromGrbl(null, null);   // ClosePortSerial();
                    reader?.Dispose();
                }
                e.Cancel = false;
                flag_closeForm = true;
                isDataProcessing = false;
                Logger.Info("====== SerialForm {0} STOP ======", iamSerial);
                AddToLog("SerialForm STOP 1");
            }
            else 
            {
                if (countShutdown == 0)
                {
                    MessageBox.Show(Localization.GetString("serialCloseError"), Localization.GetString("mainAttention"));    //"Serial Connection is needed.\r\nClose main window instead","Attention");
                }
                e.Cancel = true;
                Logger.Trace("------ Closing SerialForm {0} canceled", iamSerial);
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
            btnGRBLCmndParser.Location = new Point(btnGRBLCmndParser.Location.X, this.Height - 62);
            btnGRBLCmndBuild.Location = new Point(btnGRBLCmndBuild.Location.X, this.Height - 62);
            btnGRBLCommand3.Location = new Point(btnGRBLCommand3.Location.X, this.Height - 62);
            btnGRBLCommand4.Location = new Point(btnGRBLCommand4.Location.X, this.Height - 62);
            btnGRBLReset.Location = new Point(btnGRBLReset.Location.X, this.Height - 62);
            btnGRBLHardReset.Location = new Point(btnGRBLHardReset.Location.X, this.Height - 62);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null) return;
            try
            {
                if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
                { PasteCode(); e.Handled = true; }
                else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                { CopyCode(); e.Handled = true; }
                else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
                { SelectAll(); e.Handled = true; }
                base.OnKeyDown(e);
            }
            catch (Exception err)
            { Logger.Error(err, "OnKeyDown "); }
            //		e.Handled = true;
        }
        private void PasteCodeFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        { PasteCode(); }
        private void CopySelectionToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbLog.SelectedText))
                Clipboard.SetText(rtbLog.SelectedText);
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        { rtbLog.SelectAll(); }

        private void SelectAll()
        { rtbLog.SelectAll(); }

        private void CopyCode()
        {
            if (!string.IsNullOrEmpty(rtbLog.SelectedText))
                Clipboard.SetText(rtbLog.SelectedText);
        }

        private void PasteCode()
        {
            string[] temp = Clipboard.GetText(TextDataFormat.Text).Split('\n');

            if (!cBCommand.Focused)
            {
                foreach (string tmp in temp)
                {
                    if (tmp.Length > 1)
                    {
                        string cmd = CleanUpCodeLine(tmp);
                        if (!isStreaming || isStreamingPause)
                        {
                            RequestSend(cmd);
                            cBCommand.Items.Insert(0, cmd);
                            cBCommand.Text = cmd;
                        }
                    }
                }
            }
        }

        /**************************************************************************
         * Timer to retry sending data   timerSerial.Interval = grbl.pollInterval;
         **************************************************************************/
        private void TimerSerial_Tick(object sender, EventArgs e)
        {

            /* minimize form after connection - if enabled (then set start value) */
            if (countMinimizeForm > 0)
            {
                if (--countMinimizeForm == 0)
                {
                    if (IsConnectedToGrbl())
                    {
                        this.WindowState = FormWindowState.Minimized;
                        Logger.Info("Minimize window");
                        AddToLog("Minimize window...");
                        Application.DoEvents();
                        return;
                    }
                }
                Logger.Trace("TimerSerial_Tick countMinimizeForm:{0}", countMinimizeForm);
            }

            SerialPortOpen = IsConnectedToGrbl();
            if (iamSerial == 1) { Grbl.isConnected = SerialPortOpen; }

            if (logTransmit) Logger.Trace("TimerSerial_Tick SerialPortOpen:{0} IsConnectedToGrbl():{1} serialPortError:{2}", SerialPortOpen, IsConnectedToGrbl(), serialPortError);

            /* Connection lost ??? */
            if (!SerialPortOpen && SerialPortOpenLast && (grblStateNow != GrblState.unknown) && (grblStateNow != GrblState.notConnected))
            {
                grblStateNow = GrblState.notConnected;
                AddToLog("\r* Lost connection, close port at " + GetTimeStampString());
                AddToLog("* Last line-nr. sent: " + streamingBuffer.GetSentLineNr() + "   confirmed: " + streamingBuffer.GetConfirmedLineNr());
                Logger.Error("⚠⚠⚠ Lost connection, close port. Last sent:{0}  confirmed:{1}", streamingBuffer.GetSentLineNr(), streamingBuffer.GetConfirmedLineNr());
                EventCollector.SetCommunication(string.Format("CLost{0}", iamSerial), true);
                Grbl.lastMessage = lastError = "Lost connection, close port";

                SendStreamEvent(GrblStreaming.error);		// SaveStreamingStatus
                OnRaisePosEvent(new PosEventArgs(posWork, posMachine, GrblState.notConnected, machineState, mParserState, Grbl.lastMessage));
                this.BeginInvoke(new EventHandler(DisconnectFromGrbl));
                lblSrState.BackColor = Grbl.GrblStateColor(grblStateNow);
                lblSrState.Text = Grbl.StatusToText(grblStateNow);  // status;
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            SerialPortOpenLast = SerialPortOpen;


            if (IsConnectedToGrbl() && !serialPortError && (countShutdown == 0))
            {
                /* poll status */
                try
                {
                    //		if (resetProcessed)		// only poll status if reset was successfully recognized
                    {
                        if (isMarlin)
                        {
                            if (!isStreaming) { SerialPortDataSend("M114" + lineEndTXmarlin); getMarlinPositionWasSent = true; }    // marlin pos request
                                                                                                                                    // if isSteaming, insert M114 in PreProcessStreaming
                            updateMarlinPosition = true;
                        }
                        else
                        {
                            if (!serialPortError)
                            {
                                rtsrResponse++;                           // real time status report was sent  / will be decreased in processGrblMessages()      
                                SerialPortDataSend("?");
                                if (useEthernet)
                                    EthernetDataReceive();
                            }
                        }
                    }
                }
                catch (TimeoutException err)
                {
                    serialPortError = true;
                    Logger.Error(err, "Ser:{0} GRBL status could not be queried (send ?) TimeoutException rtsrResponse:{1} ", iamSerial, rtsrResponse);
                    LogError("! GRBL status could not be queried " + rtsrResponse, err);
                    ResetStreaming();
                    DisconnectFromGrbl(null, null);   // ClosePortSerial();
                }
                catch (InvalidOperationException err)
                {
                    serialPortError = true;
                    Logger.Error(err, "Ser:{0} GRBL status could not be queried (send ?) or (send M114) in TimerSerial_Tick ", iamSerial);
                    LogError("! GRBL status could not be queried " + rtsrResponse, err);
                    ResetStreaming();
                    DisconnectFromGrbl(null, null);   // ClosePortSerial();
                }
                catch (Exception err)
                {
                    Logger.Error(err, "Ser:{0} unknown error in TimerSerial_Tick", iamSerial);
                }

                /* missing status reports ??? */
                if (!isMarlin && !isHoming && (countMissingStatusReport-- <= 0))
                {
                    if (Math.Abs(rtsrResponse) > 10)
                    {
                        if (resetProcessed) lastError = string.Format("Missing {0} Real-time Status Reports per 10 seconds. Interval:{1}", rtsrResponse, timerSerial.Interval);
                        timerSerial.Interval += 200;    // reduce polling frequency

                        if (iamSerial == 1)
                            Grbl.lastMessage = lastError;
                        if (resetProcessed) AddToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        if (resetProcessed) AddToLog(lastError);
                        Logger.Error("Ser:{0}  {1}", iamSerial, lastError);

                        //   if (!resetProcessed)    // try to get Marlin response - removed 2022-04-08
                        {
                            AddToLog("Correct baud rate? Try Marlin response...");
                            SerialPortDataSend("M114" + lineEndTXmarlin);       // marlin
                        }
                    }
                    countMissingStatusReport = (int)(10000 / timerSerial.Interval);
                    rtsrResponse = 0;
                }

                /* some state counters... */
                if (countCallCheckGRBL > 0)
                {
                    if (countCallCheckGRBL-- == 0)
                    { BtnCheckGRBL_Click(sender, e); }			// calculate if grbl-setup is ok (30 kHz pin frequency)
                }

                if (!resetProcessed && (axisCount > 0))			// to get axis count, a status must be received
                {
                    //   Logger.Info("Process reset:{0} ", countStateReset);
                    if (countStateReset > 0)					// set in StateReset to 10
                    {
                        countStateReset--;

                        if (countStateReset == 4)
                        {
                            SerialPortOpen = IsConnectedToGrbl();
                            if (iamSerial == 1) { Grbl.isConnected = IsConnectedToGrbl(); }
                            SendResetEvent();
                        }
                        else if (countStateReset == 1)
                        {
                            grblBufferSize = Grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
                            grblBufferFree = Grbl.RX_BUFFER_SIZE;
                            Logger.Trace("timerSerial_Tick  grblBufferFree:{0} ", grblBufferFree);
                        }
                        else if (countStateReset == 0)
                        {
                            resetProcessed = true;
                        }
                    }
                }
                if (countPreventWaitForOkLock > 0)
                {
                    countPreventWaitForOkLock--;
                    if (countPreventWaitForOkLock == 0)
                    { waitForOk = false; }
                }
                if (countPreventOutput > 0) { countPreventOutput--; }
                if (countPreventEvent > 0) { countPreventEvent--; }
                if (countPreventIdle > 0) { if (--countPreventIdle == 0) serial2Busy = serial3Busy = false; }
                if (countPreventIdle2nd > 0) { countPreventIdle2nd--; }
                if (countPreventInterlock > 0)
                {
                    countPreventInterlock--;
                    if (countPreventInterlock == 0)
                    { grblStateLast = GrblState.unknown; }
                }

                trgEvent = true;    // in TimerSerial_Tick


                /* preProcessStreaming and  processSend may block further timer-code */
                if (isStreaming)
                {
                    if (countLoggerUpdate-- <= 0)
                    { countLoggerUpdate = (int)(10000 / timerSerial.Interval); }
                    if (!isStreamingRequestPause && !isStreamingPause)
                    { PreProcessStreaming(); }          // TimerSerial_Tick

                    if (!lowLevelPerformance)
                        StreamingMonitor();	// ControlSerialFormStreaming.cs
                }
                if (!waitForIdle)
                { ProcessSend(); }      // TimerSerial_Tick
            }
            if (tryDoSerialConnection)
            {
                if (rtbLog.InvokeRequired)  // rtbLog.AppendText("*"); }
                { rtbLog.BeginInvoke((MethodInvoker)delegate () { rtbLog.AppendText("*"); }); }
                else
                {
                    rtbLog.AppendText("*");
                    Application.DoEvents();
                }
            }
        }


        private void LoadSettings()
        {
            try
            {
                if (iamSerial == 1)
                {
                    cbPort.Text = Properties.Settings.Default.serialPort1;
                    cbBaud.Text = Properties.Settings.Default.serialBaud1;
                    TbEthernetIP.Text = Properties.Settings.Default.serialEthernetIP1;
                    TbEthernetPort.Text = Properties.Settings.Default.serialEthernetPort1.ToString();
                    useEthernet = CbEthernetUse.Checked = Properties.Settings.Default.serialEthernetUse1;
                }
                else
                {
                    cbPort.Text = Properties.Settings.Default.serialPort2;
                    cbBaud.Text = Properties.Settings.Default.serialBaud2;
                    TbEthernetIP.Text = Properties.Settings.Default.serialEthernetIP2;
                    TbEthernetPort.Text = Properties.Settings.Default.serialEthernetPort2.ToString();
                    useEthernet = CbEthernetUse.Checked = Properties.Settings.Default.serialEthernetUse2;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Ser:{0}  -loadSettings-", iamSerial);
                LogError("! Loading settings", e);
            }
        }
        private void SaveSettings()
        {
            try
            {
                if (iamSerial == 1)
                {
                    Properties.Settings.Default.locationSerForm1 = Location;
                    Properties.Settings.Default.sizeSerForm1 = Size;
                    Properties.Settings.Default.ctrlLaserMode = IsLasermode;
                    Properties.Settings.Default.serialPort1 = cbPort.Text;
                    Properties.Settings.Default.serialBaud1 = cbBaud.Text;
                    Properties.Settings.Default.serialEthernetIP1 = TbEthernetIP.Text;
                    if (int.TryParse(TbEthernetPort.Text, out int port))
                        Properties.Settings.Default.serialEthernetPort1 = port;
                    else
                        Properties.Settings.Default.serialEthernetPort1 = 34000;
                    Properties.Settings.Default.serialEthernetUse1 = CbEthernetUse.Checked;
                    SaveLastPos();
                }
                else
                {
                    Properties.Settings.Default.locationSerForm2 = Location;
                    Properties.Settings.Default.serialPort2 = cbPort.Text;
                    Properties.Settings.Default.serialBaud2 = cbBaud.Text;
                    Properties.Settings.Default.serialEthernetIP2 = TbEthernetIP.Text;
                    if (int.TryParse(TbEthernetPort.Text, out int port))
                        Properties.Settings.Default.serialEthernetPort2 = port;
                    else
                        Properties.Settings.Default.serialEthernetPort2 = 34000;
                    Properties.Settings.Default.serialEthernetUse2 = CbEthernetUse.Checked;
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Ser:{0} -saveSettings-", iamSerial);
                LogError("! Saving settings", e);
            }
        }

        private void LogError(string message, Exception error)
        {
            string textmsg = "\r\n[ERROR]: " + message + ": ";
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
                    rtbLog.AppendText(text + "\r\n");
                    rtbLog.ScrollToCaret();

                    var lines = rtbLog.Lines;
                    if (lines.Length > 11000)
                    {   var newlines = lines.Skip(1000);
                        rtbLog.Lines = newlines.ToArray();
                     //   rtbLog.AppendText("--------------------------------------------------------------\r\n");
                    }
                }
                catch
                {
                }
            }
        }

        #region serialPort
        private bool RefreshPorts()
        {
            List<String> tList = new List<String>();
            cbPort.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) tList.Add(s);
            if ((tList.Count < 1) && !CbEthernetUse.Checked)
            { LogError("! No serial ports found", null); }
            else
            {
                tList.Sort();
                cbPort.Items.AddRange(tList.ToArray());
            }
            return tList.Contains(cbPort.Text);
        }
        private void OpenPortSerial()
        {
            rxErrorCount = 0;
            bool errorOnOpen = false;
            serialPortError = false;
            try
            {
                Logger.Info("Ser:{0}, ==== openPort '{1}' @ {2} Bd ======", iamSerial, cbPort.Text, cbBaud.Text);
                serialPort.PortName = actualPort = cbPort.Text;
                serialPort.DataBits = 8;
                serialPort.BaudRate = Convert.ToInt32(cbBaud.Text);
                serialPort.Parity = System.IO.Ports.Parity.None;
                serialPort.StopBits = System.IO.Ports.StopBits.One;
                serialPort.Handshake = System.IO.Ports.Handshake.None;
                serialPort.DtrEnable = false;
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;

                rtbLog.Clear();
                if (RefreshPorts())				// check if last used port is listed
                {
                    if (Properties.Settings.Default.ctrlUseSerialPortFixer)
                    {
                        try
                        { SerialPortFixer.Execute(cbPort.Text); }
                        catch (Exception err)
                        {
                            Logger.Error(err, "Ser:{0} -SerialPortFixer- ", iamSerial);
                            errorOnOpen = true;
                            serialPortError = true;
                        }
                    }
                }
                else
                { errorOnOpen = true; }

                if (errorOnOpen)				// last used port is not available
                {
                    tryDoSerialConnection = false;
                    AddToLog("* " + cbPort.Text + " not available\r\n");
                    Logger.Warn("⚠ Ser:{0}, Port '{1}' not available", iamSerial, cbPort.Text);
                    if (iamSerial == 1)
                        Grbl.lastMessage = "Open COM Port: " + cbPort.Text + " failed";
                }
                else                            // last used port is available
                {
                    serialPort.Open();
                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();

                    AddToLog("* Open " + cbPort.Text + "\r\n");
                    ConnectionSucceed("Open COM Port: " + cbPort.Text);
                }
            }
            catch (Exception err)
            {
                tryDoSerialConnection = false;
                Logger.Error(err, "Ser:{0} -openPort-", iamSerial);
                EventCollector.SetCommunication(string.Format("COpS{0}", iamSerial));
                countMinimizeForm = 0;
                LogError("! Opening port", err);
                if (iamSerial == 1) { Grbl.isConnected = SerialPortOpen = IsConnectedToGrbl(); }
                UpdateControls();
                BtnOpenPortSerial.Text = Localization.GetString("serialOpen");  // "Open";   
            }
        }
        private void ClosePortSerial(object sender, EventArgs e)
        { DisconnectFromGrbl(null, null); }// ClosePortSerial(); }
        public void ClosePortSerial()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    Logger.Info("Ser:{0}, ==== closePort {1} ====", iamSerial, actualPort);
                    AddToLog("SerialForm STOP 2");
                    serialPort.Close();
                }
                //   serialPort.Dispose();
                if (!isStreaming)
                    SerialPortOpenLast = false;     // avoid connection lost error
                SaveSettings();
                if (!flag_closeForm)
                {
                    AddToLog("\r* Close " + actualPort + "\r");
                    UpdateControls();
                }
                timerSerial.Interval = 1000;
            }
            catch (Exception err)
            {
                Logger.Error(err, "Ser:{0} -closePort- ", iamSerial);
                LogError("! Closing port", err);
                if (!flag_closeForm)
                { UpdateControls(); }
                timerSerial.Enabled = false;
            }
        }
        #endregion

        #region reset
        //Send reset sentence
        private void JustgrblReset()
        {
            var dataArray = new byte[] { 24 };//Ctrl-X
            if (IsConnectedToGrbl())
            {
                useEthernet = CbEthernetUse.Checked;
                if (!useEthernet)
                {
                    try
                    {
                        if (serialPort.IsOpen)
                            serialPort.Write(dataArray, 0, 1);
                        Logger.Info("RESET_grbl send Ctrl-X ⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠");
                    }
                    catch (TimeoutException err)
                    {
                        Logger.Error(err, "Ser:{0} Error sending reset (Ctrl-X)", iamSerial);
                        ResetStreaming();
                        serialPortError = true;
                        timerSerial.Enabled = false;
                        DisconnectFromGrbl(null, null);   // ClosePortSerial();
                        LogError("! Error sending reset (Ctrl-X)", err);
                    }
                    catch (Exception err)
                    {
                        Logger.Error(err, "Ser:{0} Error sending reset (Ctrl-X)", iamSerial);
                        LogError("! Closing port", err);
                        if (!flag_closeForm)
                        { UpdateControls(); }
                        timerSerial.Enabled = false;
                    }
                }
                else
                {
                    try
                    {
                        if (Connection.CanWrite)
                            Connection.Write(dataArray, 0, 1);
                        Logger.Info("JustgrblReset");
                    }
                    catch (Exception err)
                    {
                        Logger.Error(err, "Ethernet JustgrblReset Error sending reset (Ctrl-X)");
                    }
                    EthernetDataReceive();
                }
            }
        }

        private void StateReset(bool savePos)
        {
            if (savePos)
            { SaveLastPos(); }
            countStateReset = 10;
            timerSerial.Interval = Grbl.pollInterval;
            UpdateGrblBufferSettings();
            countMissingStatusReport = (int)(2000 / timerSerial.Interval);
            waitForIdle = false;
            waitForOk = false;
            //    rtbLog.Clear();
            ResetVariables();
            ResetStreaming();
            IsHeightProbing = false;
            ToolInSpindle = false;
            externalProbe = false;
            countPreventOutput = 0; countPreventEvent = 0;
            if (iamSerial == 1)
                Grbl.Clear();
            UpdateControls();
            rxErrorCount = 0;
            rtsrResponse = 0;     // real time status report sent / receive differnence                    
            lblSrState.BackColor = Color.LightGray;
            lblSrState.Text = "reset";  // status;
            grblStateNow = GrblState.unknown;
            machineState.Clear();
            mParserState.Reset();
            //		if (iamSerial == 1)
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
        }

        public void GrblReset(bool savePos)      //Stop/reset button
        {
            StateReset(savePos);
            AddToLog("> [CTRL-X] reset");
            resetProcessed = false;
            JustgrblReset();
            if (iamSerial == 1)
            {
                Grbl.lastMessage = "RESET, waiting for response of grbl-controller";
                SendResetEvent();
            }
        }

        public void GrblHardReset()      //Stop/reset button
        {
            useEthernet = CbEthernetUse.Checked;
            if (useEthernet)
                return;
            resetProcessed = false;
            bool savePos = true;
            Grbl.isMarlin = isMarlin = Properties.Settings.Default.ctrlConnectMarlin;
            if (serialPort.IsOpen)
            {
                try
                {
                    timerSerial.Enabled = false;
                    serialPort.DtrEnable = true;
                    StateReset(savePos);
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    AddToLog("> DTR/RTS reset");
                    serialPort.DtrEnable = false;
                    serialPort.RtsEnable = false;
                    if (iamSerial == 1)
                    {
                        Grbl.lastMessage = "Hard-RESET, waiting for response of grbl-controller";
                        Logger.Info("RESET_grbl send DTR/RTS reset ⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠⚠");
                    }
                }
                catch (Exception err)
                {
                    Logger.Error(err, "GrblHardReset");
                    EventCollector.SetCommunication(string.Format("CRst{0}-{1}", iamSerial, err.Message));
                    AddToLog("> DTR/RTS reset ERROR " + err.Message);
                }
            }
            else
            {
                AddToLog("> ERROR: DTR/RTS reset failed - port not open!!!");
            }
        }
        #endregion

        #region serial receive handling
        /*************************************************************************
         * RX Interupt
         * 1) serialPort1_DataReceived get data, call ProcessMessages
         * 2) ProcessMessages -> process 'ok', '<...>', reset '[...]'
         *************************************************************************/
        //https://stackoverflow.com/questions/10871339/terminating-a-form-while-com-datareceived-event-keeps-fireing-c-sharp
        //https://stackoverflow.com/questions/1696521/c-proper-way-to-close-serialport-with-winforms/3176959

        internal delegate void InvokeDeleg();
        /* SerialDataReceivedEventHandler */
        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))// && !blockSend)
            {
                try
                {
                    rxString = string.Empty;
                    rxString = serialPort.ReadTo(lineEndRX).Trim();  //read line from grbl, discard CR LF
                    isDataProcessing = true;
                    if (!lowLevelPerformance)
                        this.BeginInvoke(new EventHandler(ProcessMessages));        //tigger rx process 2020-09-16 change from Invoke to BeginInvoke
                    else
                        this.Invoke(new EventHandler(ProcessMessages));             // 2023-03-29 add option for slow PCs

                    while ((serialPort.IsOpen) && isDataProcessing && (countShutdown == 0))// && !blockSend)   //wait previous data line processed done
                    { }
                }
                catch (TimeoutException err1)
                {
                    Logger.Error(err1, "TimeoutException try:{0} ", rxErrorCount);
                    AddToLog("Error reading line from serial port - correct baud rate? Try:" + rxErrorCount.ToString());
                    if (serialPort.IsOpen)
                        rxString = serialPort.ReadExisting().Trim();
                    Logger.Error("ReadExisting '{0}'", rxString);

                    if (++rxErrorCount > 2)
                    {
                        EventCollector.SetCommunication(string.Format("CRSa{0}-{1}", iamSerial, err1.Message));
                        this.BeginInvoke(new EventHandler(ClosePortSerial));    //closePort();
                        grblStateNow = GrblState.notConnected;
                        Grbl.lastMessage = "Serial timeout exception - correct baud rate?";
                    }

                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, GrblState.notConnected, machineState, mParserState, Grbl.lastMessage));
                }
                catch (Exception err)
                {
                    if (++rxErrorCount > 2)
                    {
                        AddToLog("Close port after 3 retries");
                        Logger.Error(err, "Ser:{0} -DataReceived- Close port after 3 retries", iamSerial);
                        this.BeginInvoke(new EventHandler(ClosePortSerial));    //closePort();
                    }
                    else
                        Logger.Error(err, "Ser:{0} -DataReceived- ", iamSerial);

                    grblStateNow = GrblState.notConnected;
                    Grbl.lastMessage = "Serial receive exception - correct baud rate?";
                    EventCollector.SetCommunication(string.Format("CRSb{0}-{1}", iamSerial, err.Message));
                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, GrblState.notConnected, machineState, mParserState, Grbl.lastMessage));
                }
            }
        }

        /* will be called frequently by timer */
        private void EthernetDataReceive()//object sender, EventArgs e)
        {
            if (!Connected || (rxErrorCount > 2))
                return;
            string line = "";
            int maxTry = 0;
            do
            {
                try
                {
                    if ((Connection != null) && (Connection.CanRead) && (reader != null))
                        line = reader.ReadLine();
                }
                catch (System.ArgumentException)    // avoid shutdown beacause of 'System.ArgumentException: The output character buffer is too small for the encoded characters. Encoding of "Unicode (UTF-8)" in fallback position "System.Text.DecoderReplacementFallback".'
                { Logger.Warn("⚠ Ethernet ArgumentException - but fixed: Encoding of 'Unicode(UTF - 8)' in fallback position 'System.Text.DecoderReplacementFallback'... '{0}'", line); }
                catch (System.IO.IOException err)
                {
                    if (Connected)
                    {
                        Logger.Error(err, "EthernetDataReceive Ethernet IO.IOException... ");
                        this.BeginInvoke(new EventHandler(DisconnectFromGrbl));
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        break;
                    }
                }
                catch (Exception err)
                {
                    rxErrorCount++;
                    AddToLog("Error ethernet receive - disconnect: " + err.Message);
                    Logger.Error(err, "EthernetDataReceive - disconnect:{0} ", rxErrorCount);
                    EventCollector.SetCommunication(string.Format("CRE-{0}", err.Message));
                    grblStateNow = GrblState.notConnected;
                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, GrblState.notConnected, machineState, mParserState, Grbl.lastMessage));
                    //   DisconnectFromGrbl(null, null);
                    this.BeginInvoke(new EventHandler(DisconnectFromGrbl));
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    break;
                }
                if (!String.IsNullOrEmpty(line))
                {
                    rxString = line;
                    //	Logger.Info("EthernetDataReceive {0}",line);
                    isDataProcessing = true;
                    this.BeginInvoke(new EventHandler(ProcessMessages));
                    while (IsConnectedToGrbl() && isDataProcessing)// && !blockSend)   //wait previous data line processed done
                    {
                        //              Application.DoEvents();
                    }
                    Application.DoEvents();
                    rxErrorCount = 0;
                }
            } while ((!String.IsNullOrEmpty(line) && (maxTry++ < 5)));
        }

        private void SerialPortDataSend(byte[] tmp, int a, int b)
        {
            if (logTransmit) Logger.Trace("SerialPortDataSend data:{0} IsConnectedToGrbl:{1} Connected:{2} useEthernet:{3}  serOpen:{4}", tmp.ToString(), IsConnectedToGrbl(), Connected, useEthernet, serialPort.IsOpen);
            if (IsConnectedToGrbl())
            {
                //   useEthernet = CbEthernetUse.Checked;
                if (!useEthernet)
                {
                    try
                    {
                        serialPort.Write(tmp, a, b);
                        if (logStreamData) WriteLog(logFileSentData, tmp.ToString(), "1");
                        return;
                    }
                    catch (Exception err)
                    {       // InvalidOperationException, ArgumentNullException, TimeoutException
                        AddToLog("Data send exception " + err.Message + " -Close port");
                        Logger.Error(err, "SerialPortDataSend Ser:{0} ", iamSerial);
                        EventCollector.SetCommunication(string.Format("CSSa{0}-{1}", iamSerial, err.Message));
                    }
                }
                else
                {
                    try
                    {
                        if (Connection.CanWrite)
                            Connection.Write(tmp, a, b);       //tmp[0]);
                        if (logStreamData) WriteLog(logFileSentData, tmp.ToString(), "2");
                    }
                    catch (System.IO.IOException err)
                    {
                        if (Connected)
                        {
                            Logger.Error(err, "SerialPortDataSend byte, Ethernet IO.IOException... ");
                            this.BeginInvoke(new EventHandler(DisconnectFromGrbl));
                            if (this.WindowState == FormWindowState.Minimized)
                            {
                                this.WindowState = FormWindowState.Normal;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        AddToLog("Data send exception " + err.Message);
                        Logger.Error(err, "SerialPortDataSend Ethernet ");
                        EventCollector.SetCommunication(string.Format("CSEa-{0}", err.Message));
                    }
                    //    EthernetDataReceive();
                }
            }
        }
        private bool SerialPortDataSend(string tmp)
        {
            if (logTransmit) Logger.Trace("SerialPortDataSend data:{0} IsConnectedToGrbl:{1} Connected:{2} useEthernet:{3}  serOpen:{4}", tmp, IsConnectedToGrbl(), Connected, useEthernet, serialPort.IsOpen);
            //   if (IsConnectedToGrbl() && (!string.IsNullOrEmpty(tmp)))
            if (!string.IsNullOrEmpty(tmp))
            {
                //   useEthernet = CbEthernetUse.Checked;
                if (!useEthernet)
                {
                    if (serialPort.IsOpen)// && (!string.IsNullOrEmpty(tmp)))
                    {
                        try
                        {
                            serialPort.Write(tmp);
                            if (logStreamData && (tmp != "?")) WriteLog(logFileSentData, tmp, "3");
                            return true;
                        }
                        catch (System.IO.IOException err)
                        {
                            if (Connected)
                            {
                                Logger.Error(err, "SerialPortDataSend, Serial:{0} IO.IOException... ", iamSerial);
                                this.BeginInvoke(new EventHandler(DisconnectFromGrbl));
                                if (this.WindowState == FormWindowState.Minimized)
                                {
                                    this.WindowState = FormWindowState.Normal;
                                }
                            }
                            return false;
                        }
                        catch (Exception err)
                        {       // InvalidOperationException, ArgumentNullException, TimeoutException
                            AddToLog("Data send exception " + err.Message + " -Close port");
                            Logger.Error(err, "SerialPortDataSend Ser:{0}", iamSerial);
                            EventCollector.SetCommunication(string.Format("CSSb{0}-{1}", iamSerial, err.Message));
                            this.BeginInvoke(new EventHandler(ClosePortSerial));    //closePort();
                            Grbl.lastMessage = "Serial send exception";
                            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, GrblState.notConnected, machineState, mParserState, Grbl.lastMessage));
                            return false;
                        }
                    }
                }
                else
                {
                    try
                    {
                        byte[] myWriteBuffer = Encoding.ASCII.GetBytes(tmp);
                        if (Connection.CanWrite)
                            Connection.Write(myWriteBuffer, 0, myWriteBuffer.Length); //(tmp);
                        if (logStreamData && (tmp != "?")) WriteLog(logFileSentData, tmp, "4");
                        return true;
                    }
                    catch (Exception err)
                    {
                        AddToLog("Data send exception " + err.Message);
                        Logger.Error(err, "SerialPortDataSend Ethernet ");
                        EventCollector.SetCommunication(string.Format("CSEb-{0}", err.Message));
                    }
                    //	EthernetDataReceive();
                }
            }
            return false;
        }

        private readonly object threadlock = new object();

        private void WriteLog(string fileName, string message, string source)
        {
            try
            {
                lock (threadlock)
                {
                    System.IO.File.AppendAllText(Datapath.LogFiles + "\\" + fileName, message);
                }
            }
            catch (Exception err)
            {
                AddToLog("WriteLog exception " + source + " " + err.Message);
            }
        }
        private void SendResetEvent()
        {
            if (lastError.Length > 2)
            { AddToLog("* last error: " + lastError); }
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, GrblState.reset, machineState, mParserState, rxString));// lastCmd));
        }

        #endregion

        #region override // grbl 0.9	
        private bool replaceFeedRate = false;
        private bool replaceSpindleSpeed = false;
        private string replaceFeedRateCmd = "";
        private string replaceSpindleSpeedCmd = "";
        private string replaceFeedRateCmdOld = "";
        private string replaceSpindleSpeedCmdOld = "";
        // called by MainForm -> get override events from form "StreamingForm" for GRBL 0.9
        public void InjectCode(string cmd, int value, bool enable)
        {
            if (cmd == "F")
            {
                replaceFeedRate = enable;
                replaceFeedRateCmd = cmd + value.ToString();
                if (isStreaming)
                {
                    if (enable)
                        InjectCodeLine(replaceFeedRateCmd);
                    else
                    {
                        if (!string.IsNullOrEmpty(replaceFeedRateCmdOld))
                            InjectCodeLine(replaceFeedRateCmdOld);
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
                        InjectCodeLine(replaceSpindleSpeedCmd);
                    else
                    {
                        if (!string.IsNullOrEmpty(replaceSpindleSpeedCmdOld))
                            InjectCodeLine(replaceSpindleSpeedCmdOld);
                    }
                }
            }
        }

        private void InjectCodeLine(string data)
        {
            int index = streamingBuffer.IndexSent + 1;
            int linenr = streamingBuffer.GetSentLineNr();   // gCodeLineNr[streamingBuffer.Sent];
            AddToLog("!!! Override: " + data + " in line " + linenr);
            streamingBuffer.Insert(index, data, linenr);
            //    index++;
        }
        #endregion

        #region controls
        private void UpdateControls()
        {
            bool isConnected = IsConnectedToGrbl() || Grbl.grblSimulate;
            SerialPortOpen = isConnected;
            if (iamSerial == 1) { Grbl.isConnected = SerialPortOpen; }
            //  bool isSensing = isStreaming;
            cbPort.Enabled = !isConnected;
            cbBaud.Enabled = !isConnected;
            btnScanPort.Enabled = !isConnected;
            //    btnClear.Enabled = isConnected;
            cBCommand.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnSend.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand0.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand1.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand2.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCmndParser.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCmndBuild.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand3.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand4.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnCheckGRBL.Enabled = isConnected && (!isStreaming || isStreamingPause);// && !isGrblVers0;
            btnGRBLReset.Enabled = isConnected;// & !isSensing;
            if (iamSerial > 1)
            {
                btnCheckGRBL.Visible = false;
                btnCheckGRBLResult.Visible = false;
            }
        }

        private void BtnScanPort_Click(object sender, EventArgs e)
        { RefreshPorts(); }

        private void BtnOpenPortSerial_Click(object sender, EventArgs e)
        {
            if (IsConnectedToGrbl())// serialPort.IsOpen)
            {
                DisconnectFromGrbl(null, null);
            }
            else
            { ConnectToGrbl(); }// OpenPortSerial(); }
            UpdateControls();
        }
        private void BtnOpenPortEthernet_Click(object sender, EventArgs e)
        {
            if (IsConnectedToGrbl())
            {
                DisconnectFromGrbl(null, null);
            }
            else
            {
                ConnectToGrbl();
            }
            UpdateControls();
        }

        private void ShowConnectionControls(bool showEthernet = false)
        {
            LblEthernetIP.Visible = LblEthernetPort.Visible = showEthernet;
            TbEthernetIP.Visible = TbEthernetPort.Visible = showEthernet;
            BtnOpenPortEthernet.Visible = showEthernet;

            cbPort.Visible = cbBaud.Visible = !showEthernet;
            BtnOpenPortSerial.Visible = btnScanPort.Visible = !showEthernet;
        }

        private void CbEthernetUse_CheckedChanged(object sender, EventArgs e)
        {
            ShowConnectionControls(CbEthernetUse.Checked);
        }

        private void CbStatus1_CheckedChanged(object sender, EventArgs e)
        {
            if (cBStatus1.Checked)
            {
                Logger.Info("Status1 Enable");
                AddToLog(string.Format("TX> {0,-28} {1,2} {2,3}  line-Nr", "Code line", " len", "free "));
            }
            else
            { Logger.Info("Status1 Disable"); }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        { UpdateLogging(); rtbLog.Clear(); }
        private void TbCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) return;
            BtnSend_Click(sender, e);
        }
        private void BtnSend_Click(object sender, EventArgs e)
        { SendCommand(); }
        private void SendCommand()
        {
            if (!isStreaming || isStreamingPause)
            {
                string cmd = cBCommand.Text.ToUpper();
                RequestSend(cmd);
                cBCommand.Items.Remove(cBCommand.SelectedItem);
                cBCommand.Items.Insert(0, cmd);
                cBCommand.Text = cmd;
            }
        }
        private void BtnGRBLCommand0_Click(object sender, EventArgs e)
        { RequestSend("$"); }

        public bool CheckGRBLSettingsOk()
        {
            bool isOk = true;
            float maxfX = Grbl.GetSetting(100) * Grbl.GetSetting(110) / 60000;
            float maxfY = Grbl.GetSetting(101) * Grbl.GetSetting(111) / 60000;
            float maxfZ = Grbl.GetSetting(102) * Grbl.GetSetting(112) / 60000;
            float maxfA = Grbl.GetSetting(103) * Grbl.GetSetting(113) / 60000;
            float maxfB = Grbl.GetSetting(104) * Grbl.GetSetting(114) / 60000;
            float maxfC = Grbl.GetSetting(105) * Grbl.GetSetting(115) / 60000;
            if ((maxfX > 30) || (maxfY > 30) || (maxfZ > 30) || (maxfA > 30) || (maxfB > 30) || (maxfC > 30))
            {
                isOk = false;
                Grbl.lastMessage = "ATTENTION: One or more axis exceeds 30kHz step frequency";
                AddToLog("\r\rATTENTION:\rOne or more axis exceeds \r30kHz step frequency - Calculation:");
                AddToLog("f= $100 steps/mm * $110 speed /60000\r");
            }
            if (maxfX > 30) AddToLog(string.Format(" X Axis ($100={0}, $110={1}): {2} kHz", Grbl.GetSetting(100), Grbl.GetSetting(110), maxfX));
            if (maxfY > 30) AddToLog(string.Format(" Y Axis ($101={0}, $111={1}): {2} kHz", Grbl.GetSetting(101), Grbl.GetSetting(111), maxfY));
            if (maxfZ > 30) AddToLog(string.Format(" Z Axis ($102={0}, $112={1}): {2} kHz", Grbl.GetSetting(102), Grbl.GetSetting(112), maxfZ));
            if (maxfA > 30) AddToLog(string.Format(" A Axis ($103={0}, $113={1}): {2} kHz", Grbl.GetSetting(103), Grbl.GetSetting(113), maxfA));
            if (maxfB > 30) AddToLog(string.Format(" B Axis ($104={0}, $114={1}): {2} kHz", Grbl.GetSetting(104), Grbl.GetSetting(114), maxfB));
            if (maxfC > 30) AddToLog(string.Format(" C Axis ($105={0}, $115={1}): {2} kHz", Grbl.GetSetting(105), Grbl.GetSetting(115), maxfC));
            if (!isOk)
                AddToLog("\r\r");
            return isOk;
        }
        private void BtnCheckGRBL_Click(object sender, EventArgs e)
        {
            float stepX = 0, stepY = 0, stepZ = 0;
            float speedX = 0, speedY = 0, speedZ = 0;
            float maxfX, maxfY, maxfZ;
            string rx, ry, rz;
            //     int id;
            if ((GRBLSettings.Count > 0))
            {
                foreach (string setting in GRBLSettings)
                {
                    string[] splt = setting.Split('=');
                    if (splt.Length > 1)
                    {
                        if (int.TryParse(splt[0].Substring(1), out int id))
                        {
                            string val = splt[1];
                            int maxcnt = splt[1].IndexOf(" ");
                            if (maxcnt > 1) val = splt[1].Substring(0, maxcnt);

                            if (id == 100) { float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stepX); }
                            else if (id == 101) { float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stepY); }
                            else if (id == 102) { float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stepZ); }
                            else if (id == 110) { float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out speedX); }
                            else if (id == 111) { float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out speedY); }
                            else if (id == 112) { float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out speedZ); }
                            //    AddToLog(id.ToString() + " " + splt[1].Substring(0, maxcnt));
                        }
                    }
                }
                maxfX = stepX * speedX / 60000; rx = (maxfX < 30) ? "ok" : "problem!";
                maxfY = stepY * speedY / 60000; ry = (maxfY < 30) ? "ok" : "problem!";
                maxfZ = stepZ * speedZ / 60000; rz = (maxfZ < 30) ? "ok" : "problem!";
                if ((maxfX < 30) && (maxfY < 30) && (maxfZ < 30))
                    btnCheckGRBLResult.BackColor = Color.Lime;
                else
                    btnCheckGRBLResult.BackColor = Color.Fuchsia;
                btnCheckGRBLResult.Enabled = true;
                float minF = 1800 / Math.Max(stepX, Math.Max(stepY, stepZ));
                strCheckResult = "Maximum frequency at a 'STEP' pin (at Arduino UNO, Nano) must not exceed 30kHz.\r\nCalculation: steps/mm ($100) * speed-mm/min ($110) / 60 / 1000\r\n";
                strCheckResult += string.Format("Max frequency X = {0:.##}kHz - {1}\r\nMax frequency Y = {2:.##}kHz - {3}\r\nMax frequency Z = {4:.##}kHz - {5}\r\n\r\n", maxfX, rx, maxfY, ry, maxfZ, rz);
                strCheckResult += "Minimum feedrate (F) must not go below 30 steps/sec.\r\nCalculation: (lowest mm/min) = (30 steps/sec) * (60 sec/min) / (axis steps/mm setting)\r\n";
                strCheckResult += string.Format("Min Feedrate for X = {0:.#}mm/min\r\nMin Feedrate for Y = {1:.#}mm/min\r\nMin Feedrate for Z = {2:.#}mm/min\r\n\r\n", (1800 / stepX), (1800 / stepY), (1800 / stepZ));
                strCheckResult += string.Format("Avoid feedrates (F) below {0:.#}mm/min\r\n", minF);
                strCheckResult += "\r\nSettings are copied to clipboard for further use (e.g. save as text file)";
                try
                {
                    string text = string.Join("\r\n", GRBLSettings.ToArray());
                    if (!string.IsNullOrEmpty(text))
                        System.Windows.Forms.Clipboard.SetText(text);
                }
                catch
                {
                    AddToLog("Could not copy settings to clipboard!");
                }
                GRBLSettings.Clear();
            }
            else
            {
                if (grblStateNow == GrblState.idle)
                {
                    RequestSend("$$"); //GRBLSettings.Clear();
                    countCallCheckGRBL = 4;                         // timer1 will recall this function after 2 seconds
                }
                else
                {
                    AddToLog("Wait for IDLE, then try again!");
                }
            }
        }
        private void BtnCheckGRBLResult_Click(object sender, EventArgs e)
        { MessageBox.Show(strCheckResult, "Information"); }

        public bool FlagGrblSettingClick { get; set; } = false;
        private void BtnGRBLCommand1_Click(object sender, EventArgs e)      // $$and $x=val - View and write Grbl settings
        { RequestSend("$$"); GRBLSettings.Clear(); FlagGrblSettingClick = true; }
        private void BtnGRBLCommand2_Click(object sender, EventArgs e)      // $# - View gcode parameters
        { RequestSend("$#"); }
        private void BtnGRBLCmndParser_Click(object sender, EventArgs e)    // $G - View gcode parser state
        { RequestSend("$G"); }
        private void BtnGRBLCmndBuild_Click(object sender, EventArgs e)     // $I - View build info
        { RequestSend("$I"); }
        private void BtnGRBLCommand3_Click(object sender, EventArgs e)      // $N - View startup blocks
        { RequestSend("$N"); }
        private void BtnGRBLCommand4_Click(object sender, EventArgs e)      // $X - Kill alarm lock
        { RequestSend("$X"); }
        private void BtnGRBLReset_Click(object sender, EventArgs e)
        { UpdateLogging(); GrblReset(true); }    // savePos
        private void BtnGRBLHardReset_Click(object sender, EventArgs e)
        { UpdateLogging(); GrblHardReset(); }

        #endregion

        #region runtimecounter

        private static string GetTimeStampString()
        { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }

        private void LblAccessoryState_Click(object sender, EventArgs e)
        { ListAccessoryStateRunTime(true); }
        private void ListAccessoryStateRunTime(bool showAll)
        {
            AddToRunTimer(stopwatchSpindle, "grblRunTimeSpindle");	// update timer if needed
            AddToRunTimer(stopwatchFlood, "grblRunTimeFlood");
            AddToRunTimer(stopwatchMist, "grblRunTimeMist");
            long tSpindle = Properties.Settings.Default.grblRunTimeSpindle;
            long tFlood = Properties.Settings.Default.grblRunTimeFlood;
            long tMist = Properties.Settings.Default.grblRunTimeMist;

            if (showAll || ((tSpindle + tFlood + tMist) > 1))
            {
                AddToLog("");
                AddToLog("Accessory State run times :");
                AddToLog("             h: m: s");
                if (showAll || (tSpindle > 1)) AddToLog("Spindle: " + GetTimeString(tSpindle));
                if (showAll || (tFlood > 1)) AddToLog("Flood  : " + GetTimeString(tFlood));
                if (showAll || (tMist > 1)) AddToLog("Mist   : " + GetTimeString(tMist));
                AddToLog("---------------------------");
            }
            Logger.Info("🛠🛠🛠 ListAccessoryStateRunTime Spindle:{0}  Flood:{1}  Mist:{2}", GetTimeString(tSpindle), GetTimeString(tFlood), GetTimeString(tMist));
        }
        private string GetTimeString(long tmp)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(Convert.ToDouble(tmp));
            string answer = string.Format("{0:D5}:{1:D2}:{2:D2}",
                            (int)(t.TotalHours),
                            t.Minutes,
                            t.Seconds);
            return answer;
        }

        #endregion

    }
}
