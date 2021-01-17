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
*/

// OnRaiseStreamEvent(new StreamEventArgs((int)lineNr, codeFinish, buffFinish, status));
// OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));

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
    public partial class ControlSerialForm : Form       // Form can be loaded twice!!! COM1, COM2
    {

        ControlSerialForm _serial_form2;
        SimpleSerialForm _serial_form3;

        public bool serialPortOpen { get; private set; } = false;

        private System.Timers.Timer timerSerial = new System.Timers.Timer();

        private int iamSerial = 1;
		private int axisCount = 0;
        private int rxErrorCount = 0;
//		private int idleInterval = 500;		// timer intervall in IDLE state
		
        private string rxString;
		private string actualPort = "";     
        private string formTitle = "";
        private string strCheckResult = "";		// result from grbl-setup check

        public string lastError = "";
        private int countGrblError = 0;
        private bool flag_closeForm = false;

        private bool useSerial2 = false;
        private bool serial2Busy = false;
        private bool useSerial3 = false;
        private bool serial3Busy = false;

        // Note: receive-event waits for line-end char, if missing -> timeout exception
        private const string lineEndRX = "\n";      // read - grbl accepts '\n' or '\r' and sends "\r\n", but Marlin sends '\n'
        private const string lineEndTXgrbl = "\r";    // send - grbl accepts '\n' or '\r' and sends "\r\n", but Marlin sends '\n'

        /* counters will be decreased in timer event - action on 0 */
        private int countMinimizeForm = 0;              // timer to minimize form
        private int countMissingStatusReport = 20;      // count missing status reports after sending ?
        private int countCallCheckGRBL = 0;             // timer to check result after start check
        private int countStateReset = 10;               // 
        private int countPreventOutput = 0;             // prevent log output e.g. during startup $$
        private int countPreventEvent = 0;              // prevent event
        private int countPreventIdle = 0;               // delay Idle proccessing
        private int countPreventIdle2nd = 0;            // delay Idle proccessing
        private int countShutdown = 0;
        private int countPreventInterlock = 100;
        private int countPreventWaitForOkLock = 5;
        private int countLoggerUpdate = 0;

        private bool isMarlin = false;
        private bool updateMarlinPosition = false;
        private bool getMarlinPositionWasSent = false;
        private int insertMarlinCounterReload = 10;
        private int insertMarlinCounter = 10;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
//        private static readonly NLog.Logger LogPos = NLog.LogManager.GetLogger("LogPos");

        private static bool logReceiveStatus = false;	// send '?' get '< Idle | MPos:0.000,0.000,0.000 | FS:0,0 | WCO:0.000,0.000,0.000 >'
        private static bool logReceive = false;			// any other RX data
        private static bool logTransmit = false;			// TX data
		private static bool logStartStop = false;
        private static bool log2ndGrbl = false;
        private static bool log3rdCOM = false;

		private void updateLogging()
		{	// LogEnable { Level1=1, Level2=2, Level3=4, Level4=8, Detailed=16, Coordinates=32, Properties=64, Sort = 128, GroupAllGraphics = 256, ClipCode = 512, PathModification = 1024 }
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
			bool logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnable.Level4) > 0);
			logReceiveStatus = logEnable && ((logFlags & (uint)LogEnable.Detailed) > 0);
			logReceive = logEnable && ((logFlags & (uint)LogEnable.Coordinates) > 0);
			logTransmit = logEnable && ((logFlags & (uint)LogEnable.Properties) > 0);
			logStartStop = logEnable && ((logFlags & (uint)LogEnable.Sort) > 0);
        }
		
        public ControlSerialForm(string txt, int nr, ControlSerialForm handle2 = null, SimpleSerialForm handle3 = null)
        {
            iamSerial = nr;
            updateLogging();
            Logger.Info("====== SerialForm {0} {1} START ======", iamSerial, txt);
            formTitle = txt;
            set2ndSerial(handle2);
            set3rdSerial(handle3);
            InitializeComponent();
            mParserState.reset();
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            this.Icon = Properties.Resources.Icon;
            this.Invalidate();

            timerSerial.Elapsed += timerSerial_Tick;
            timerSerial.Interval = 1000;
        }
        public void set2ndSerial(ControlSerialForm handle = null)
        {   _serial_form2 = handle;
            if (handle != null)
            {   useSerial2 = true;
                if (log2ndGrbl) Logger.Trace("set2ndSerial {0}", handle);
            }
            else
                useSerial2 = false;
       }
        public void set3rdSerial(SimpleSerialForm handle = null)
        {   _serial_form3 = handle;
            if (handle != null)
            {   useSerial3 = true;
                if (log3rdCOM) Logger.Trace("set3rdSerial {0}", handle);
            }
            else
                useSerial3 = false;
        }

        private bool externalCOMReady()
        {
            if (!useSerial2 && !useSerial3)
                return true;
            bool c2 = true, c3 = true;
            if (useSerial2) c2 = !serial2Busy && (_serial_form2.grblStateNow == grblState.idle);
            if (useSerial3) c3 = !serial3Busy && !_serial_form3.busy;
            return c2 && c3;
        }

        private void SerialForm_Load(object sender, EventArgs e)
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            SerialForm_Resize(sender, e);
            machineState.Clear();
            updateControls();       // disable controls
            loadSettings();         // set last COM and Baud
            refreshPorts();         // scan for COMs
            openPort();             // open COM
            if (iamSerial == 1)
            {   Location = Properties.Settings.Default.locationSerForm1;
                Size = Properties.Settings.Default.sizeSerForm1;
                if ((Size.Width < 342) || (Size.Height < 480))
                { Size = new Size(342, 480); }
            }
            else
            {   Location = Properties.Settings.Default.locationSerForm2;}
            Text = formTitle;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { this.CenterToScreen(); }    // Location = new Point(100, 100);    }
            isLasermode = Properties.Settings.Default.ctrlLaserMode;
            resetVariables(true);
            if (Properties.Settings.Default.serialMinimize)
				addToLog("Form will be reduced after connecting");

            timerSerial.Enabled = true;
            timerSerial.Start();
        }

        private void SerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {   Logger.Trace("====== Try closing SerialForm {0} {1}", iamSerial, e.CloseReason);
            if ((e.CloseReason.ToString() == "ApplicationExitCall") || (e.CloseReason.ToString() == "FormOwnerClosing"))
            {
				isDataProcessing = false;
				if (countShutdown == 0)
                {	serialPort.DataReceived -= (this.serialPort1_DataReceived); // stop receiving data
					justgrblReset();	// nötig?
					stopStreaming();
					timerSerial.Stop();
					timerSerial.Interval = 1000;
					timerSerial.Start();
					closePort();
					countShutdown = 5;
				}
                e.Cancel = false;
                flag_closeForm = true;
                Logger.Info("====== SerialForm {0} STOP ======", iamSerial);
            }
            else
            {
                MessageBox.Show(Localization.getString("serialCloseError"), Localization.getString("mainAttention"));    //"Serial Connection is needed.\r\nClose main window instead","Attention");
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
		{	if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{   pasteCode(); e.Handled = true; }
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {   copyCode(); e.Handled = true; }
            if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            { selectAll(); e.Handled = true; }
            base.OnKeyDown(e);
	//		e.Handled = true;
		}
        private void pasteCodeFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        { pasteCode(); }
        private void copySelectionToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        { Clipboard.SetText(rtbLog.SelectedText); }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        { rtbLog.SelectAll(); }

        private void selectAll()
        {   rtbLog.SelectAll(); }

        private void copyCode()
        {   Clipboard.SetText(rtbLog.SelectedText); }

        private void pasteCode()
        {   string[] temp = Clipboard.GetText(TextDataFormat.Text).Split('\n');
		
			if (!cBCommand.Focused)
			{	foreach (string tmp in temp)
				{   if (tmp.Length > 1)
					{   string cmd = cleanUpCodeLine(tmp);
						if (!isStreaming || isStreamingPause)
						{   requestSend(cmd);
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
        private void timerSerial_Tick(object sender, EventArgs e)
        {
            if (countMinimizeForm > 0)							// minimize form after connection - if enabled (then set start value)
            {   if ((--countMinimizeForm == 0) && serialPort.IsOpen)
                {   this.WindowState = FormWindowState.Minimized;
                }
            }

            if (serialPortOpen = serialPort.IsOpen)
            {   try
                {   
                    if (isMarlin) {
                        if (!isStreaming) { serialPort.Write("M114" + lineEndTXgrbl); getMarlinPositionWasSent = true; }    // marlin pos request
                        updateMarlinPosition = true;
                    }
                    else {
                        var dataArray = new byte[] { Convert.ToByte('?') };
                        serialPort.Write(dataArray, 0, 1);
                        rtsrResponse++;     						// real time status report was sent  / will be decreased in processGrblMessages()                  
                    }
                }
                catch (Exception er)
                {   Logger.Error(er, "Ser:{0} GRBL status not received ",iamSerial);
                    logError("! Retrieving GRBL status", er);
                    serialPort.Close();
                }
				
                if (!isMarlin && !isHoming && (countMissingStatusReport-- <= 0))
                {   if (Math.Abs(rtsrResponse) > 10)
                    {   if (resetProcessed) lastError = string.Format("Missing {0} Real-time Status Reports per 10 seconds",rtsrResponse);
						if (iamSerial == 1)
							grbl.lastMessage = lastError;
                        if (resetProcessed) addToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        if (resetProcessed) addToLog(lastError);
                        Logger.Error("Ser:{0}  {1}",iamSerial, lastError);

                        if (!resetProcessed)    // try to get Marlin response
                        {   addToLog("Correct baud rate? Try Marlin response...");
                            requestSend("\r\nM115\n");
                        }
                    }
                    countMissingStatusReport = (int)(10000 / timerSerial.Interval);
                    rtsrResponse = 0;
                }

                if (countCallCheckGRBL > 0)
                {   if (countCallCheckGRBL-- == 0)
                    { btnCheckGRBL_Click(sender, e); }			// calculate if grbl-setup is ok (30 kHz pin frequency)
                }

                if (!resetProcessed && (axisCount>0))
                {   if (countStateReset > 0)
                    {   countStateReset--;

                        if (countStateReset == 4)
                        {
                            serialPortOpen = serialPort.IsOpen;
                            sendResetEvent();
                        }
                        else if (countStateReset == 1)
                        {
                            grblBufferSize = grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
                            grblBufferFree = grbl.RX_BUFFER_SIZE;
                            Logger.Trace("timerSerial_Tick  grblBufferFree:{0} ", grblBufferFree);
                        }
                        else if (countStateReset == 0)
                        {
                            resetProcessed = true;
                        }
                    }
                }
                if (countPreventWaitForOkLock > 0)
                {   countPreventWaitForOkLock--;
                    if (countPreventWaitForOkLock == 0)
                    {   waitForOk = false; }
                }
                if (countPreventOutput > 0) { countPreventOutput--; }
                if (countPreventEvent > 0) { countPreventEvent--; }
                if (countPreventIdle > 0) { if (--countPreventIdle == 0) serial2Busy = serial3Busy = false; }
                if (countPreventIdle2nd > 0) { countPreventIdle2nd--; }
                if (countPreventInterlock > 0)
                {   countPreventInterlock--;
                    if (countPreventInterlock == 0)
                    {   grblStateLast = grblState.unknown;   }
                }

                if (isStreaming)
                {   if (countLoggerUpdate-- <= 0)
                    {   //Logger.Info("timer update infoStream:{0}", listInfoStream());
                        //Logger.Info("timer update infoSend:  {0}", listInfoSend());
                        countLoggerUpdate = (int)(10000 / timerSerial.Interval);
                    }
                    if (!isStreamingRequestPause && !isStreamingPause)
                    {   preProcessStreaming(); }
                }
/* preProcessStreaming and  processSend may block further timer-code */                
                if (!waitForIdle)
                {   processSend(); }                
            }

            if (countShutdown > 0)		//(flag_closeForm)
            {   Logger.Trace("Ser:{0} timer: countShutdown:{1}",iamSerial,countShutdown);
				countShutdown--;
                Application.Exit();
            }
        }
		
		
        private void loadSettings()
        {   try
            {   if (iamSerial == 1)
                {   cbPort.Text = Properties.Settings.Default.serialPort1;
                    cbBaud.Text = Properties.Settings.Default.serialBaud1;
                }
                else
                {   cbPort.Text = Properties.Settings.Default.serialPort2;
                    cbBaud.Text = Properties.Settings.Default.serialBaud2;
                }
            }
            catch (Exception e)
            {   Logger.Error(e, "Ser:{0}  -loadSettings-",iamSerial);
                logError("! Loading settings", e);
            }
        }
        private void saveSettings()
        {   try
            {   if (iamSerial == 1)
                {   Properties.Settings.Default.locationSerForm1 = Location;
                    Properties.Settings.Default.sizeSerForm1 = Size;
                    Properties.Settings.Default.ctrlLaserMode = isLasermode;
                    Properties.Settings.Default.serialPort1 = cbPort.Text;
                    Properties.Settings.Default.serialBaud1 = cbBaud.Text;
                    saveLastPos();
                }
                else
                {   Properties.Settings.Default.locationSerForm2 = Location;
                    Properties.Settings.Default.serialPort2 = cbPort.Text;
                    Properties.Settings.Default.serialBaud2 = cbBaud.Text;
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Ser:{0} -saveSettings-",iamSerial);
                logError("! Saving settings", e);
            }
        }

        private void logError(string message, Exception error)
        {   string textmsg = "\r\n[ERROR]: " + message + ". ";
            if (error != null) textmsg += error.Message;
            textmsg += "\r\n";
            addToLog(textmsg);
        }

        delegate void addToLogCallback(string text);
        public void addToLog(string text)
        {   if (this.InvokeRequired == true)
            {   addToLogCallback callback = new addToLogCallback(addToLog);
                this.Invoke(callback, new object[] { text });
            }
            else
            {   try
                {   rtbLog.AppendText(text + "\r");
                    rtbLog.ScrollToCaret();
                }
                catch { }
            }
        }

#region serialPort
        private bool refreshPorts()
        {   List<String> tList = new List<String>();
            cbPort.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) tList.Add(s);
            if (tList.Count < 1) logError("! No serial ports found", null);
            else
            {   tList.Sort();
                cbPort.Items.AddRange(tList.ToArray());
            }
            return tList.Contains(cbPort.Text);
        }
        private void openPort()
        {   rxErrorCount = 0;
			try
            {   Logger.Info("Ser:{0}, openPort {1} {2}", iamSerial, cbPort.Text, cbBaud.Text);
                serialPort.PortName = actualPort = cbPort.Text;
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
                    {   try
                        { SerialPortFixer.Execute(cbPort.Text); }
                        catch (Exception err)
                        { Logger.Error(err, "Ser:{0} -SerialPortFixer-",iamSerial); }
                    }
                    serialPort.Open();
                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();
					
                    addToLog("* Open " + cbPort.Text + "\r\n");
                    if (iamSerial == 1)
						grbl.lastMessage = "Open COM Port: " + cbPort.Text;
                    btnOpenPort.Text = Localization.getString("serialClose");  // "Close";
                    isDataProcessing = true;
                    if (iamSerial == 1)
						grbl.Clear();

                    if (Properties.Settings.Default.serialMinimize)
                        countMinimizeForm = (int)(3000 / timerSerial.Interval); 	// minimize window after 5 sec.

                    timerSerial.Interval = grbl.pollInterval;       		// timerReload;
                    countMissingStatusReport = (int)(2000 / timerSerial.Interval);

                    countPreventOutput = 0; countPreventEvent = 0;
                    isHeightProbing = false;
                    if (grbl.grblSimulate)
                    {   grbl.grblSimulate = false;
                        addToLog("* Stop simulation\r\n");
                    }
                    grblReset(false);

                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.unknown, machineState, mParserState, ""));// lastCmd));
                }
                else
                {
                    addToLog("* " + cbPort.Text + " not available\r\n");
                    Logger.Warn("Ser:{0}, Port {1} not available", iamSerial, cbPort.Text);
					if (iamSerial == 1)
						grbl.lastMessage = "Open COM Port: " + cbPort.Text + " failed";
                }
                serialPortOpen = serialPort.IsOpen;
                updateControls();
            }
            catch (Exception err)
            {
                Logger.Error(err, "Ser:{0} -openPort-",iamSerial);
                countMinimizeForm = 0;
                logError("! Opening port", err);
                serialPortOpen = false;
                updateControls();
            }
        }
        private void closePort(object sender, EventArgs e)
        {   closePort(); }
        public void closePort()
        {   try
            {   if (serialPort.IsOpen)
                {   Logger.Info("Ser:{0}, closePort {1} ", iamSerial, actualPort);
                    serialPort.Close();
                }
                serialPort.Dispose();
                saveSettings();
                if (!flag_closeForm)
                {   addToLog("\r* Close " + actualPort + "\r");
                    btnOpenPort.Text = Localization.getString("serialOpen");  // "Open";
                    updateControls();
                }
                timerSerial.Interval = 1000;
            }
            catch (Exception err)
            {   Logger.Error(err, "Ser:{0} -closePort- ",iamSerial);
                logError("! Closing port", err);
                if (!flag_closeForm)
                { updateControls(); }
                timerSerial.Enabled = false;
            }
            serialPortOpen = false;
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.unknown, machineState, mParserState, ""));// lastCmd));
        }
        #endregion

        #region reset
        //Send reset sentence
        private void justgrblReset()
		{	var dataArray = new byte[] { 24 };//Ctrl-X
            if (serialPort.IsOpen)
            {   serialPort.Write(dataArray, 0, 1); }
		}
		
        public void grblReset(bool savePos = true)      //Stop/reset button
        {   justgrblReset();
            stateReset(savePos);
            addToLog("> [CTRL-X] reset");
            if (iamSerial == 1)
				grbl.lastMessage = "RESET, waiting for response of grbl-controller";
        }

        private void stateReset(bool savePos)
        {   if (savePos)
            { saveLastPos(); }
            countStateReset = 10;
            timerSerial.Interval = grbl.pollInterval;
            updateGrblBufferSettings();
            countMissingStatusReport = (int)(2000 / timerSerial.Interval);
            waitForIdle = false;
            waitForOk = false;
            rtbLog.Clear();
            resetVariables();
			resetStreaming();
            isHeightProbing = false;
            toolInSpindle = false;
            externalProbe = false;
            countPreventOutput = 0; countPreventEvent = 0;
            if (iamSerial == 1)
				grbl.Clear();
            updateControls();
            rxErrorCount = 0;
            rtsrResponse = 0;     // real time status report sent / receive differnence                    
            lblSrState.BackColor = Color.LightGray;
            lblSrState.Text = "reset";  // status;
            grblStateNow = grblState.unknown;
            machineState.Clear();
            mParserState.reset();
	//		if (iamSerial == 1)
				OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
        }

        public void grblHardReset(bool savePos = true)      //Stop/reset button
        {
            isMarlin = false;
            grbl.isMarlin = false;
            if (serialPort.IsOpen)
            {
                timerSerial.Enabled = false;
                serialPort.DtrEnable = true;
                stateReset(savePos);
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                addToLog("> DTR/RTS reset");
                serialPort.DtrEnable = false;
                serialPort.RtsEnable = false;
                if (iamSerial == 1)
					grbl.lastMessage = "Hard-RESET, waiting for response of grbl-controller";
            }
            else
            {
                addToLog("> ERROR: DTR/RTS reset failed - port not open!!!");
            }
        }
#endregion

        #region serial receive handling
/*************************************************************************
 * RX Interupt
 * 1) serialPort1_DataReceived get data, call processGrblMessages
 * 2) processGrblMessages -> process 'ok', '<...>', reset '[...]'
 *************************************************************************/
//https://stackoverflow.com/questions/10871339/terminating-a-form-while-com-datareceived-event-keeps-fireing-c-sharp
//https://stackoverflow.com/questions/1696521/c-proper-way-to-close-serialport-with-winforms/3176959

        public delegate void InvokeDelegate();

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))// && !blockSend)
            {   rxString = string.Empty;
                try
                {
                    rxString = serialPort.ReadTo(lineEndRX).Trim();  //read line from grbl, discard CR LF
                    isDataProcessing = true;
                    this.BeginInvoke(new EventHandler(processMessages));        //tigger rx process 2020-09-16 change from Invoke to BeginInvoke
                    while ((serialPort.IsOpen) && isDataProcessing)// && !blockSend)   //wait previous data line processed done
                    { }
                }
                catch (TimeoutException err1)
                {   Logger.Error(err1, "TimeoutException");
                    addToLog("Error reading line from serial port - correct baud rate?");
                    rxString = serialPort.ReadExisting().Trim();
                    Logger.Error("ReadExisting '{0}'",rxString);
                    this.BeginInvoke(new EventHandler(closePort));    //closePort();

                    grblStateNow = grblState.notConnected;
                    grbl.lastMessage = "Serial timeout exception - correct baud rate?";
                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.notConnected, machineState, mParserState, grbl.lastMessage));
                }
                catch (Exception err)
                {
                    if (++rxErrorCount > 2)
                    {   addToLog("Close port after 3 retries");
                        Logger.Error(err, "Ser:{0} -DataReceived- Close port after 3 retries", iamSerial);
                        this.BeginInvoke(new EventHandler(closePort));    //closePort();
                    }
                    else
                        Logger.Error(err, "Ser:{0} -DataReceived- ", iamSerial);

                    grblStateNow = grblState.notConnected;
                    grbl.lastMessage = "Serial exception - correct baud rate?";
                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.notConnected, machineState, mParserState, grbl.lastMessage));
                }
            }
        }
		
        private void sendResetEvent()
        {
			if (lastError.Length > 2)
			{	addToLog("* last error: " + lastError);
				OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.reset, machineState, mParserState, rxString));// lastCmd));
			}
			else
			{	OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.reset, machineState, mParserState, rxString));// lastCmd));
			}
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
            int index = streamingBuffer.IndexSent + 1;
            int linenr = streamingBuffer.GetSentLineNr();   // gCodeLineNr[streamingBuffer.Sent];
            addToLog("!!! Override: " + data + " in line "+linenr);
            streamingBuffer.Insert(index, data, linenr);
            index++;
        }
		#endregion

		#region controls
        private void updateControls()
        {
            bool isConnected = serialPort.IsOpen || grbl.grblSimulate;
            serialPortOpen = isConnected;
            bool isSensing = isStreaming;
            cbPort.Enabled = !isConnected;
            cbBaud.Enabled = !isConnected;
            btnScanPort.Enabled = !isConnected;
            btnClear.Enabled = isConnected;
            cBCommand.Enabled = isConnected &&       (!isStreaming || isStreamingPause);
            btnSend.Enabled = isConnected &&         (!isStreaming || isStreamingPause);
            btnGRBLCommand0.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand1.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand2.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCmndParser.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCmndBuild.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand3.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnGRBLCommand4.Enabled = isConnected && (!isStreaming || isStreamingPause);
            btnCheckGRBL.Enabled = isConnected &&    (!isStreaming || isStreamingPause);// && !isGrblVers0;
            btnGRBLReset.Enabled = isConnected;// & !isSensing;
			if (iamSerial > 1)
			{	btnCheckGRBL.Visible = false;
				btnCheckGRBLResult.Visible = false;
			}
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
        {   if (e.KeyChar != (char)13) return;
            btnSend_Click(sender, e);
        }
        private void btnSend_Click(object sender, EventArgs e)
        { sendCommand();}
		private void sendCommand()
		{	if (!isStreaming || isStreamingPause)
            {
                string cmd = cBCommand.Text.ToUpper();
                requestSend(cmd);
                cBCommand.Items.Remove(cBCommand.SelectedItem);
                cBCommand.Items.Insert(0, cmd);
                cBCommand.Text = cmd;
            }
        }
        private void btnGRBLCommand0_Click(object sender, EventArgs e)
        { requestSend("$"); }

        public bool checkGRBLSettingsOk()
        {
            bool isOk = true;
            float maxfX = grbl.getSetting(100) * grbl.getSetting(110) / 60000;
            float maxfY = grbl.getSetting(101) * grbl.getSetting(111) / 60000;
            float maxfZ = grbl.getSetting(102) * grbl.getSetting(112) / 60000;
            float maxfA = grbl.getSetting(103) * grbl.getSetting(113) / 60000;
            float maxfB = grbl.getSetting(104) * grbl.getSetting(114) / 60000;
            float maxfC = grbl.getSetting(105) * grbl.getSetting(115) / 60000;
            if ((maxfX > 30) || (maxfY > 30) || (maxfZ > 30) || (maxfA > 30) || (maxfB > 30) || (maxfC > 30))
            {   isOk = false;
                grbl.lastMessage = "ATTENTION: One or more axis exceeds 30kHz step frequency";
                addToLog("\r\rATTENTION:\rOne or more axis exceeds \r30kHz step frequency - Calculation:");
                addToLog("f= $100 steps/mm * $110 speed /60000\r");
            }
            if (maxfX > 30) addToLog(string.Format(" X Axis ($100={0}, $110={1}): {2} kHz", grbl.getSetting(100), grbl.getSetting(110), maxfX));
            if (maxfY > 30) addToLog(string.Format(" Y Axis ($101={0}, $111={1}): {2} kHz", grbl.getSetting(101), grbl.getSetting(111), maxfY));
            if (maxfZ > 30) addToLog(string.Format(" Z Axis ($102={0}, $112={1}): {2} kHz", grbl.getSetting(102), grbl.getSetting(112), maxfZ));
            if (maxfA > 30) addToLog(string.Format(" A Axis ($103={0}, $113={1}): {2} kHz", grbl.getSetting(103), grbl.getSetting(113), maxfA));
            if (maxfB > 30) addToLog(string.Format(" B Axis ($104={0}, $114={1}): {2} kHz", grbl.getSetting(104), grbl.getSetting(114), maxfB));
            if (maxfC > 30) addToLog(string.Format(" C Axis ($105={0}, $115={1}): {2} kHz", grbl.getSetting(105), grbl.getSetting(115), maxfC));
            if (!isOk)
                addToLog("\r\r");
            return isOk;
        }
        private void btnCheckGRBL_Click(object sender, EventArgs e)
        {
            float stepX = 0, stepY = 0, stepZ = 0;
            float speedX = 0, speedY = 0, speedZ = 0;
            float maxfX = 0, maxfY = 0, maxfZ = 0;
            string rx, ry, rz;
            int id;
            if ((GRBLSettings.Count > 0))
            {
                foreach (string setting in GRBLSettings)
                {
                    string[] splt = setting.Split('=');
                    if (splt.Length > 1)
                    {
                        if (int.TryParse(splt[0].Substring(1), out id))
                        {
                            if (id == 100) { float.TryParse(splt[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stepX); }
                            else if (id == 101) { float.TryParse(splt[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stepY); }
                            else if (id == 102) { float.TryParse(splt[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out stepZ); }
                            else if (id == 110) { float.TryParse(splt[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out speedX); }
                            else if (id == 111) { float.TryParse(splt[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out speedY); }
                            else if (id == 112) { float.TryParse(splt[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out speedZ); }
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
                System.Windows.Forms.Clipboard.SetText(string.Join("\r\n", GRBLSettings.ToArray()));
                GRBLSettings.Clear();
            }
            else
            {   if (grblStateNow == grblState.idle)
                {   requestSend("$$"); //GRBLSettings.Clear();
                    countCallCheckGRBL = 4;                         // timer1 will recall this function after 2 seconds
                }
                else {
                    addToLog("Wait for IDLE, then try again!");
                }
            }
        }
        private void btnCheckGRBLResult_Click(object sender, EventArgs e)
        {   MessageBox.Show(strCheckResult, "Information");    }

        private void btnGRBLCommand1_Click(object sender, EventArgs e)
        { requestSend("$$"); GRBLSettings.Clear(); }
        private void btnGRBLCommand2_Click(object sender, EventArgs e)
        { requestSend("$#"); }
        private void btnGRBLCmndParser_Click(object sender, EventArgs e)
        { requestSend("$G"); }
        private void btnGRBLCmndBuild_Click(object sender, EventArgs e)
        { requestSend("$I"); }
        private void btnGRBLCommand3_Click(object sender, EventArgs e)
        { requestSend("$N"); }
        private void btnGRBLCommand4_Click(object sender, EventArgs e)
        { requestSend("$X"); }
        private void btnGRBLReset_Click(object sender, EventArgs e)
        { grblReset(); }
        private void btnGRBLHardReset_Click(object sender, EventArgs e)
        {   grblHardReset(); }

        #endregion
    }
}
