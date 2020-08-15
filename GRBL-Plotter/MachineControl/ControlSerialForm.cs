/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
        private xyzPoint posWCO, posWork, posMachine;
        public xyzPoint posPause, posProbe, posProbeOld;
        private mState machineState = new mState();     // Keep info about Bf, Ln, FS, Pn, Ov, A from grbl status
        private pState mParserState = new pState();     // keep info about last M and G settings from GCode
        private bool resetProcessed = false;

        public bool serialPortOpen { get; private set; } = false;
        public bool isGrblVers0 { get; private set; } = true;
        public string grblVers { get; private set; } = "";
        public bool isLasermode { get; private set; } = false;
        public bool toolInSpindle { get; set; } = false;
        public bool isHeightProbing { get; set; } = false;      // automatic height probing -> less feedback
        public List<string> GRBLSettings = new List<string>();          // keep $$ settings
        private Queue<string> lastSentToCOM = new Queue<string>();      // store last sent commands via COM

        private Dictionary<string, double> gcodeVariable = new Dictionary<string, double>();    // keep variables "PRBX" etc.
        public string parserStateGC = "";                  // keep parser state response [GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]

        private int timerReload = grbl.pollInterval;     //200;
        private string rxString;
        private bool useSerial2 = false;
        private int iamSerial = 1;
        private string formTitle = "";
//        private System.Net.Sockets.TcpClient sock;
        private int rtsrResponse = 0;     // real time status report sent / receive differnence - should be zero.                    

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ControlSerialForm(string txt, int nr, ControlSerialForm handle = null)
        {
            Logger.Info("++++++ SerialForm {0} START ++++++", iamSerial);
            this.Icon = Properties.Resources.Icon;
            mParserState.reset();
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            formTitle = txt;
            this.Invalidate();
            iamSerial = nr;
            set2ndSerial(handle);
            InitializeComponent();
   //         Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
   //         currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
        }
        public void set2ndSerial(ControlSerialForm handle = null)
        {   _serial_form2 = handle;
            if (handle != null)
                useSerial2 = true;
        }

        //Unhandled exception
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Logger.Error(ex, "Application_ThreadException");
            MessageBox.Show(ex.Message, "Serial Form Thread exception");
            closePort();
        }
        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                Logger.Error(ex, "Application_ThreadException");
                MessageBox.Show(ex.Message, "Serial Form Application exception");
                closePort();
            }
        }

        private void SerialForm_Load(object sender, EventArgs e)
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            SerialForm_Resize(sender, e);
            refreshPorts();         // scan for COMs
            updateControls();       // disable controls
            loadSettings();         // set last COM and Baud
            openPort();             // open COM
            machineState.Clear();
            if (iamSerial == 1)
            {   Location = Properties.Settings.Default.locationSerForm1;}
            else
            {   Location = Properties.Settings.Default.locationSerForm2;}
            Text = formTitle;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width-100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height-100))) { Location = new Point(100, 100); }
            isLasermode = Properties.Settings.Default.ctrlLaserMode;
            resetVariables(true);
        }
 //       private bool mainformAskClosing = false;
        private void SerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("Try closing SerialForm {0} {1}", iamSerial, e.CloseReason);
            if ((e.CloseReason.ToString() == "ApplicationExitCall") || (e.CloseReason.ToString() == "FormOwnerClosing"))
            {
                serialPort.DataReceived -= (this.serialPort1_DataReceived); // stop receiving data
                stopStreaming();
                grblReset(false);
                closePort();
                e.Cancel = false;
                Logger.Info("++++++ SerialForm {0} STOP ++++++", iamSerial);
            }
            else
            {
                MessageBox.Show(Localization.getString("serialCloseError"), Localization.getString("mainAttention"));    //"Serial Connection is needed.\r\nClose main window instead","Attention");
                e.Cancel = true;
                Logger.Trace("Closing SerialForm {0} canceled", iamSerial);
                return;
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

        public void updateGrblBufferSettings()
        {
            if (!Properties.Settings.Default.grblBufferAutomatic)
                grbl.RX_BUFFER_SIZE = (int)Properties.Settings.Default.grblBufferSize;
            grblBufferSize = grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
            grblBufferFree = grbl.RX_BUFFER_SIZE;
            timerSerial.Interval = grbl.pollInterval;
            failCounter = 10000 / timerSerial.Interval;
        }

        /*
         * Timer to retry sending data   timerSerial.Interval = grbl.pollInterval;
         */
        private int failCounter = 100;
        private void timerSerial_Tick(object sender, EventArgs e)
        {
            failCounter--;
            if (minimizeCount > 0)
            {   minimizeCount--;
                if (minimizeCount == 0)
                    this.WindowState = FormWindowState.Minimized;
            }

            if (serialPort.IsOpen)
            {   try
                {   var dataArray = new byte[] { Convert.ToByte('?') };
                    serialPort.Write(dataArray, 0, 1);
                    rtsrResponse++;     // real time status report sent                    
                }
                catch (Exception er)
                {   Logger.Error(er, "GRBL status not received");
                    logError("! Retrieving GRBL status", er);
                    serialPort.Close();
                }
                if (failCounter <= 0)
                {   if (Math.Abs(rtsrResponse) > 10)
                    {   lastError = string.Format("Missing {0} Real-time Status Reports per 10 seconds",rtsrResponse);
                        grbl.lastMessage = lastError;
                        addToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        addToLog(lastError);
                        Logger.Error(lastError);
                    }
                    failCounter = 10000 / timerSerial.Interval;
                    rtsrResponse = 0;
                }
            }
            if (waitForIdle)
            {   processSend();
            }
            if (isStreaming && !isStreamingRequestPause && !isStreamingPause)
                preProcessStreaming();
            if (callCheckGRBL > 0)
            {   callCheckGRBL--;
                if (callCheckGRBL == 0)
                { btnCheckGRBL_Click(sender, e); }
            }
            if (!resetProcessed && (grbl.axisCount>0))
            {   if (resetState > 0)
                {   resetState--;
                    if (resetState == 4)
                    { sendResetEvent(); }
                    else if (resetState == 1)
                    {   grblBufferSize = grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
                        grblBufferFree = grbl.RX_BUFFER_SIZE;
                    }
                    else if (resetState == 0)   
                        resetProcessed = true;
                }
            }
            if (preventOutput > 0)
                preventOutput--;
            if (preventEvent > 0)
                preventEvent--;
        }

        private void resetVariables(bool resetToolCoord=false)
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
            gcodeVariable.Add("WACX", 0.0); // actual Work coordinates
            gcodeVariable.Add("WACY", 0.0);
            gcodeVariable.Add("WACZ", 0.0);
            gcodeVariable.Add("MLAX", 0.0); // last Machine coordinates (before break)
            gcodeVariable.Add("MLAY", 0.0);
            gcodeVariable.Add("MLAZ", 0.0);
            gcodeVariable.Add("WLAX", 0.0); // last Work coordinates (before break)
            gcodeVariable.Add("WLAY", 0.0);
            gcodeVariable.Add("WLAZ", 0.0);
            if (resetToolCoord)
            {   gcodeVariable.Add("TOAN", 0.0); // TOol Actual Number
                gcodeVariable.Add("TOAX", 0.0); // Tool change position
                gcodeVariable.Add("TOAY", 0.0);
                gcodeVariable.Add("TOAZ", 0.0);
                gcodeVariable.Add("TOAA", 0.0);
                gcodeVariable.Add("TOLN", 0.0); // TOol Last Number
                gcodeVariable.Add("TOLX", 0.0); // Tool change position
                gcodeVariable.Add("TOLY", 0.0);
                gcodeVariable.Add("TOLZ", 0.0);
                gcodeVariable.Add("TOLA", 0.0);
            }
        }
        private void loadSettings()
        {
            try
            {
                if (iamSerial == 1)
                {
                    cbPort.Text = Properties.Settings.Default.serialPort1;
                    cbBaud.Text = Properties.Settings.Default.serialBaud1;
                }
                else
                {
                    cbPort.Text = Properties.Settings.Default.serialPort2;
                    cbBaud.Text = Properties.Settings.Default.serialBaud2;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "-loadSettings-");
                logError("! Loading settings", e);
            }
        }
        private void saveSettings()
        {   try
            {   if (iamSerial == 1)
                {   Properties.Settings.Default.locationSerForm1 = Location;
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
                Logger.Error(e, "-saveSettings-");
                logError("! Saving settings", e);
            }
        }
        private void saveLastPos()
        {
            if (iamSerial == 1)
            {
                addToLog("\r* Save last pos.: \r"+posWork.Print(false,(grbl.axisCount>3))+"\n");    // print in single lines
                Properties.Settings.Default.grblLastOffsetX = Math.Round(posWork.X, 3);
                Properties.Settings.Default.grblLastOffsetY = Math.Round(posWork.Y, 3);
                Properties.Settings.Default.grblLastOffsetZ = Math.Round(posWork.Z, 3);
                Properties.Settings.Default.grblLastOffsetA = Math.Round(posWork.A, 3);
                Properties.Settings.Default.grblLastOffsetB = Math.Round(posWork.B, 3);
                Properties.Settings.Default.grblLastOffsetC = Math.Round(posWork.C, 3);
                int gNr = mParserState.coord_select;
                gNr = ((gNr >= 54) && (gNr <= 59)) ? gNr : 54;
                Properties.Settings.Default.grblLastOffsetCoord = gNr;    //global.grblParserState.coord_select;
                Properties.Settings.Default.Save();
            }
        }

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
        }

        private void logError(string message, Exception error)
        {
            string textmsg = "\r\n[ERROR]: " + message + ". ";
            if (error != null) textmsg += error.Message;
            textmsg += "\r\n";
            addToLog(textmsg);
        }
        public void logErrorThr(object sender, EventArgs e)
        {
            logError(logMessage, logErr);
            updateControls();
        }
        public void addToLog(string text)
        {   try
            {
                rtbLog.AppendText(text + "\r");
                rtbLog.ScrollToCaret();
            }
            catch { }
        }

        private void btnScanPort_Click(object sender, EventArgs e)
        { refreshPorts(); }
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
        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
                closePort();
            else
                openPort();
            updateControls();
        }
        private int minimizeCount = 0;
        private void openPort()
        {
            try
            {
                Logger.Info("Form {0}, openPort {1} {2}", iamSerial, cbPort.Text, cbBaud.Text);
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
                    {   try
                        { SerialPortFixer.Execute(cbPort.Text); }
                        catch (Exception err)
                        { Logger.Error(err, "-SerialPortFixer-"); }
                    }
                    serialPort.Open();
                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();

                    addToLog("* Open " + cbPort.Text + "\r\n");
                    grbl.lastMessage = "Open COM Port: " + cbPort.Text;
                    btnOpenPort.Text = Localization.getString("serialClose");  // "Close";
                    isDataProcessing = true;
                    grbl.Clear();
                    grblReset(false);
                    if (Properties.Settings.Default.serialMinimize)
                        minimizeCount = 10;                         // minimize window after 10 timer ticks

                    timerSerial.Interval = grbl.pollInterval;       // timerReload;
                    failCounter = 10000 / timerSerial.Interval;

                    preventOutput = 0; preventEvent = 0;
                    isHeightProbing = false;
                    if (grbl.grblSimulate)
                    {   grbl.grblSimulate = false;
                        addToLog("* Stop simulation\r\n");
                    }
                }
                else
                {
                    addToLog("* " + cbPort.Text + " not available\r\n");
                    Logger.Warn("Form {0}, Port {1} not available", iamSerial, cbPort.Text);
                    grbl.lastMessage = "Open COM Port: " + cbPort.Text + " failed";
                }
                updateControls();
            }
            catch (Exception err)
            {
                Logger.Error(err, "-openPort-");
                minimizeCount = 0;
                logError("! Opening port", err);
                updateControls();
            }
        }
        public bool closePort()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    Logger.Info("Form {0}, closePort {1}", iamSerial, serialPort.PortName);
                    serialPort.Close();
                }
                serialPort.Dispose();
                addToLog("\r* Close " + cbPort.Text + "\r");
                btnOpenPort.Text = Localization.getString("serialOpen");  // "Open";
                saveSettings();
                updateControls();
                timerSerial.Interval = 1000;
                return (true);
            }
            catch (Exception err)
            {
                Logger.Error(err, "-closePort-");
                logError("! Closing port", err);
                updateControls();
                timerSerial.Enabled = false;
                return (false);
            }
        }

        private int resetState = 10;
        private void stateReset(bool savePos)
        {
            if (savePos)
            { saveLastPos(); }
            resetState = 10;
            timerSerial.Interval = grbl.pollInterval;
            failCounter = 10000 / timerSerial.Interval;
            updateGrblBufferSettings();
            rtbLog.Clear();
            resetVariables();
            isStreaming = false;
            isStreamingPause = false;
            isHeightProbing = false;
            toolInSpindle = false;
            waitForIdle = false;
            externalProbe = false;
            preventOutput = 0; preventEvent = 0;
            grbl.Clear();
            updateControls();
            rxErrorCount = 0;
            rtsrResponse = 0;     // real time status report sent / receive differnence                    
            lblSrState.BackColor = Color.LightGray;
            lblSrState.Text = "reset";  // status;
            grblStateNow = grblState.unknown;
            machineState.Clear();
            mParserState.reset();
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
        }

        //Send reset sentence
        public void grblReset(bool savePos = true)      //Stop/reset button
        {   var dataArray = new byte[] { 24 };//Ctrl-X
            if (serialPort.IsOpen)
            {   serialPort.Write(dataArray, 0, 1); }
            stateReset(savePos);
            addToLog("> [CTRL-X] reset");
            grbl.lastMessage = "RESET, waiting for response of grbl-controller";
        }

        private void btnGRBLHardReset_Click(object sender, EventArgs e)
        {   grblHardReset(); }
        public void grblHardReset(bool savePos = true)      //Stop/reset button
        {
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
                grbl.lastMessage = "Hard-RESET, waiting for response of grbl-controller";
            }
            else
            {
                addToLog("> ERROR: DTR/RTS reset failed - port not open!!!");
            }
        }


        #region serial receive handling
        /*  RX Interupt
         *  1) serialPort1_DataReceived get data, call handleRxData
         *  2) handleRxData -> process 'ok', '<...>', reset '[...]'
         *  3) updateStreaming() process 'ok'
         *  4) processSend() -> replace varaibles, fill up grbl buffer
         * */
        string logMessage;
        Exception logErr;
        int rxErrorCount = 0;
        //https://stackoverflow.com/questions/10871339/terminating-a-form-while-com-datareceived-event-keeps-fireing-c-sharp
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))
            {   rxString = string.Empty;
                try
                {   rxString = serialPort.ReadTo("\r\n");              //read line from grbl, discard CR LF
                    isDataProcessing = true;
//                    Logger.Trace("DataReceived {0}", rxString);
                    this.Invoke(new EventHandler(handleRxData));        //tigger rx process 
                    while ((serialPort.IsOpen) && (isDataProcessing))   //wait previous data line processed done
					{}
                }
                catch (Exception err)
                {
                    Logger.Error(err, "-DataReceived- ");
                    serialPort.Close();
                    logErr = err;
                    logMessage = "Error reading line from serial port";
                    addToLog(logMessage);
                    this.BeginInvoke(new EventHandler(logErrorThr));
                    if (rxErrorCount++ > 5)
                    {   closePort();
                        this.WindowState = FormWindowState.Minimized;
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                        Logger.Error(err, "-DataReceived- Close port after 5 retries");
                    }
                    else
                        Logger.Error(err, "-DataReceived- ");
                }
            }
        }

        /*  Filter received message before further use
         * */
        public string lastError = "";
        private int preventOutput = 0;
        private int preventEvent = 0;
        private void handleRxData(object sender, EventArgs e)
        {
            char[] charsToTrim = { '<', '>', '[', ']', ' ' };
            int tmp;
            // action by importance
            // grbl buffer processed
            if (rxString.Contains("ok"))
            {   updateStreaming();                          // process all other messages
                rxString = "";                              // clear if simulation is on
                if (!isStreaming || isStreamingPause)
                {   if (!(isHeightProbing || (cBStatus1.Checked || cBStatus.Checked || (preventOutput>0))))
                        addToLog("< ok");   // string.Format("< {0}", rxString)); // < ok
                }
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                return;
            }

            // Process status message with coordinates
            else if (((tmp = rxString.IndexOf('<')) >= 0) && (rxString.IndexOf('>') > tmp))
            {
                if (cBStatus.Checked)
                    addToLog(rxString);
                handleRX_Status(rxString.Trim(charsToTrim));// Process status message with coordinates
                rtsrResponse--;                             // real time status report received                    
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                return;
            }

            // reset message
            else if (rxString.ToLower().IndexOf("['$' for help]") >= 0)
            {   Logger.Debug("Received Reset message");
                resetState = 8;         // reset message received
                handleRX_Reset(rxString);
                timerSerial.Enabled = true;
                lastError = "";
                if (true)               // read grbl settings
                {   addToLog("* Read grbl settings, hide response from '$$', '$#'");
                    readSettings();
                    requestSend("$10=2"); //if (grbl.getSetting(10) != 2) { requestSend("$10=2"); } // to get buffer size
                }
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                return;
            }

            // Process feedback message with coordinates
            else if (((tmp = rxString.IndexOf('[')) >= 0) && (rxString.IndexOf(']') > tmp))
            {   handleRX_Feedback(rxString.Trim(charsToTrim).Split(':'));
                if (!isHeightProbing || cBStatus.Checked)
                {  if (preventOutput == 0)
                        addToLog(rxString);
                }
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                return;
            }

            else if (rxString.ToUpper().IndexOf("ALARM") >= 0)
            {
                lastError = "";
                grbl.lastMessage = string.Format("grbl ALARM '{0}' {1}", rxString, grbl.getAlarmDescription(rxString));
                Logger.Warn(grbl.lastMessage);
                addToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                addToLog(string.Format("< {0} \t{1}", rxString, grbl.getAlarmDescription(rxString)));
                resetStreaming();   // ALARM
                isHeightProbing = false;
                grblStateNow = grblState.alarm;
                OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                mParserState.changed = false;
                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                return;
            }
            else if (rxString.ToUpper().IndexOf("ERROR") >= 0)
            {
                string tmpMsg = "";
                if (rxString != lastError)
                {
                    grbl.lastMessage = string.Format("grbl ERROR '{0}' {1}", rxString, grbl.getErrorDescription(rxString));
                    Logger.Warn(grbl.lastMessage);
                    addToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    addToLog(string.Format("< {0} \t{1}", rxString, grbl.getErrorDescription(rxString)));
                    lastError = rxString+" "+ grbl.getErrorDescription(rxString)+"\r\n";
                    this.WindowState = FormWindowState.Minimized;
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    if (grbl.errorBecauseOfBadCode(rxString))
                    {   addToLog(">>> Last sent commmands to grbl, oldest first:");
                        lastError += ">>> Last sent commmands to grbl, oldest first:";
                        foreach (string lastLine in lastSentToCOM)
                        {
                            tmpMsg = ">>> " + lastLine;
                            addToLog(tmpMsg);
                            lastError += tmpMsg + "\r\n";
                        }
                    }
                }
                grblStatus = grblStreaming.error;
                if (isStreaming)
                {   tmpMsg = string.Format("! Error before code line {0} \r\n", gCodeLineNr[gCodeLinesSent]);
                    addToLog(tmpMsg);
                    lastError += tmpMsg;
                    sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStatus);   // error
                    stopStreaming();
                }
                resetStreaming();   // ERROR
                isHeightProbing = false;
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                return;
            }

            // Show GRBL Settings Info if Version is >= 1.0
            else if ((rxString.IndexOf("$") >= 0) && (rxString.IndexOf("=") >= 0))
            {   handleRX_Setup(rxString);
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                return;
            }

            isDataProcessing = false;                   // unlock serialPort1_DataReceived
            return;
        }

        // process ok messages for streaming
        public void updateStreaming()
        {
            int tmpIndex = gCodeLinesSent;
            int receivedByteCount;

            // 'ok' received, increment confirmend
            if (sendLinesConfirmed < sendLinesCount)
            {
                receivedByteCount = sendLines[sendLinesConfirmed].Length + 1;   // + "\r"
                grbl.updateParserState(sendLines[sendLinesConfirmed], ref mParserState);

/* increase free buffer if ok was received */
                grblBufferFree += (receivedByteCount);   //update bytes supose to be free on grbl rx bufer
                if (cBStatus1.Checked || cBStatus.Checked)
                { addToLog(string.Format("RX< {0,-30} {1,2} {2,3}",sendLines[sendLinesConfirmed], receivedByteCount, grblBufferFree)); } // send line 1271
                sendLinesConfirmed++;                                           // line processed

                // Remove already sent lines to release memory
                if ((sendLines.Count > 1) && (sendLinesConfirmed == sendLinesSent == sendLinesCount > 1))
                {   sendLines.RemoveAt(0);
                    sendLinesConfirmed--;
                    sendLinesSent--;
                    sendLinesCount--;
                }
                if ((mParserState.changed) && (grblStateNow != grblState.probe))    // probe will be send later
                {   OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                    mParserState.changed = false;
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
                    gcodeVariable["WLAX"] = posWork.X; gcodeVariable["WLAY"] = posWork.Y; gcodeVariable["WLAZ"] = posWork.Z;

                    if (getParserState)
                    { requestSend("$G"); }
                }
                isStreamingRequestPause = false;
            }
            if (isStreaming)    // || isStreamingRequestStopp)
            {   if (!isStreamingPause)
                {   gCodeLinesConfirmed++;  //line processed
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
                else
                    grblStatus = grblStreaming.pause;   // update status
                //Transfer finished and processed? Update status and controls
                if ((gCodeLinesConfirmed >= gCodeLinesCount) && (sendLinesConfirmed == sendLinesCount))
                {
                    isStreaming = false;
                    addToLog("\r\n[Streaming finish]");
                    grblStatus = grblStreaming.finish;
                    requestSend("$G");
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
                if ((oldStatus != grblStatus) || allowStreamingEvent)
                {
                    sendStreamEvent(gCodeLinesSent,gCodeLinesConfirmed, grblStatus);    // streaming
                    oldStatus = grblStatus;     //grblStatus = oldStatus;
                    allowStreamingEvent = false;
                }
            }
 //           Logger.Trace("isStreamingRequestStopp {0}", isStreamingRequestStopp);

 //           if (isStreamingRequestStopp)
  //              sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStatus);    // streaming

            processSend();  
        }

        /*  sendStreamEvent update main prog 
         * */
        private void sendStreamEvent(int indexSent, int indexConfirmed, grblStreaming status)
        {
            int max = gCodeLineNr.Count();
  //          Logger.Trace("sendStreamEvent i1Sent:{0}  iConfirmed:{1}  LineCount:{2}  Status:{3}", indexSent, indexConfirmed, max, status);
            int lineNrSent = 0;
            int lineNrConfirmed = 0;
            if ((max > 0) && (indexSent < max) && (indexConfirmed < max))
            {
                lineNrSent = gCodeLineNr[indexSent];
                lineNrConfirmed = gCodeLineNr[indexConfirmed];
            }
			
            float codeFinish = (float)lineNrSent * 100 / (float)gCodeLinesTotal;
            float buffFinish = (float)(grblBufferSize - grblBufferFree) * 100 / (float)grblBufferSize;
            if (codeFinish > 100) { codeFinish = 100; }
            if (buffFinish > 100) { buffFinish = 100; }
            OnRaiseStreamEvent(new StreamEventArgs(lineNrSent, lineNrConfirmed, codeFinish, buffFinish, status));
        }

        private void handleRX_Reset(string rxString)
        {
            grbl.axisCount = 0;
            resetProcessed = false;
            rxErrorCount = 0;
            rtsrResponse = 0;       // real time status report sent / receive differnence                    
            resetStreaming();       // handleRX_Reset
            addToLog("* RESET\r\n< " + rxString);
            if (rxString.ToLower().IndexOf("grbl 0") >= 0)
            { isGrblVers0 = true; isLasermode = false; }
            if (rxString.ToLower().IndexOf("grbl 1") >= 0)
            { isGrblVers0 = false; addToLog("* Version 1.x"); }

            if (iamSerial == 1)
                grbl.isVersion_0 = isGrblVers0;

            grblVers = rxString.Substring(0, rxString.IndexOf('['));

            lblSrBf.Text = "";
            lblSrFS.Text = "";
            lblSrPn.Text = "";
            lblSrLn.Text = "";
            lblSrOv.Text = "";
            lblSrA.Text = "";
            return;
        }

        private void sendResetEvent()
        {
            if (lastError.Length > 2)
            {
                addToLog("* last error: " + lastError);
                OnRaiseStreamEvent(new StreamEventArgs(0, 0, -1, 0, grblStreaming.reset));
            }
            else
                OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, 0, grblStreaming.reset));
        }
        private void handleRX_Feedback(string[] dataField)  // dataField = rxString.Trim(charsToTrim).Split(':')
        {
            if (iamSerial == 1 && dataField.Length > 1)
            {   string info = "";
                if (dataField.Length > 2)
                    info = dataField[2];
                grbl.setCoordinates(dataField[0], dataField[1], info);    // store gcode parameters https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#---view-gcode-parameters
            }
            if (dataField[0].IndexOf("GC") >= 0)            // handle G-Code parser state [GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]
            {
                parserStateGC = dataField[1];
                grbl.updateParserState(dataField[1], ref mParserState);
                if (isGrblVers0)
                    parserStateGC = parserStateGC.Replace("M0 ", "");
                posPause = posWork;
                getParserState = false;
                OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                mParserState.changed = false;
            }
            else if (dataField[0].IndexOf("PRB") >= 0)                // Probe message with coordinates // [PRB:-155.000,-160.000,-28.208:1]
            {   if (preventEvent==0)
                    grblStateNow = grblState.probe;
                posProbeOld = posProbe;
                grbl.getPosition("PRB:" + dataField[1], ref posProbe);  // get numbers from string
                gcodeVariable["PRBX"] = posProbe.X; gcodeVariable["PRBY"] = posProbe.Y; gcodeVariable["PRBZ"] = posProbe.Z;
                gcodeVariable["PRDX"] = posProbe.X - posProbeOld.X; gcodeVariable["PRDY"] = posProbe.Y - posProbeOld.Y; gcodeVariable["PRDZ"] = posProbe.Z - posProbeOld.Z;
                if (preventEvent == 0)
                {
                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                    mParserState.changed = false;
                }
            }

            else if (dataField[0].IndexOf("MSG") >= 0) //[MSG:Pgm End]
            {   if (dataField[1].IndexOf("Pgm End") >= 0)
                {   if ((isStreaming) || (isHeightProbing))
                    {
                        isStreaming = false;
                        isHeightProbing = false;
                        preventEvent = 0; preventOutput = 0;
                        addToLog("\r[Streaming finish]");
                        grblStatus = grblStreaming.finish;
                        if (isStreamingCheck)
                        { requestSend("$C"); isStreamingCheck = false; }
                        updateControls();
                        allowStreamingEvent = true;
                        OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, 0, grblStreaming.finish));
                        if (Properties.Settings.Default.grblPollIntervalReduce)
                        {
                            timerSerial.Interval = grbl.pollInterval;
                            failCounter = 10000 / timerSerial.Interval;
                        }
                    }
                }
            }
          /*  if (iamSerial == 1)
            {
                string info = "";
                if (dataField.Length > 2)
                    info = dataField[2];
                grbl.setCoordinates(dataField[0], dataField[1], info);    // store gcode parameters https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#---view-gcode-parameters
            }*/
        }

        private void handleRX_Setup(string rxString)
        {
            timerSerial.Interval = grbl.pollInterval;
            failCounter = 10000 / timerSerial.Interval;

            string[] splt = rxString.Split('=');
            int id;
            if (int.TryParse(splt[0].Substring(1), out id))
            {
                if (!isGrblVers0)
                {   string msgNr = splt[0].Substring(1).Trim();
                    if (preventOutput == 0)
                        addToLog(string.Format("< {0} ({1})", rxString.PadRight(14,' '), grbl.getSettingDescription(msgNr)));   // output $$ response
                    if (id == 32)
                    {   if (splt[1].IndexOf("1") >= 0)
                            isLasermode = true;
                        else
                            isLasermode = false;
                        OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, 0, grblStreaming.lasermode));
                    }
                }
                else
                    addToLog(string.Format("< {0}", rxString));
                GRBLSettings.Add(rxString);
                if (iamSerial == 1)
                    grbl.setSettings(id, splt[1]);
            }
            else
                addToLog(string.Format("< {0}", rxString));
        }

        private grblState grblStateNow = grblState.unknown;
        private grblState grblStateLast = grblState.unknown;

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
            {
                if (dataField.Length > 3)
                    grbl.getPosition(dataField[1] + "," + dataField[2] + "," + dataField[3]+" ", ref posMachine);
                if (dataField.Length > 6)
                    grbl.getPosition(dataField[4] + "," + dataField[5] + "," + dataField[6]+" ", ref posWork);
                posWCO = posMachine - posWork;
            }
            else
            {
                //machineState.Clear(); //lblSrPn.Text = ""; //lblSrA.Text = "";
                if (dataField.Length > 2)
                {
                    for (int i = 2; i < dataField.Length; i++)
                    {
                        if (dataField[i].IndexOf("WCO") >= 0)           // Work Coordinate Offset
                        {   grbl.getPosition(dataField[i], ref posWCO);
                            continue;
                        }
                        string[] data = dataField[i].Split(':');
                        if (dataField[i].IndexOf("Bf:") >= 0)            // Buffer state - needs to be enabled in config.h file
                        {   machineState.Bf=lblSrBf.Text = data[1];
                            if (grbl.getBufferSize(data[1])) requestSend("$10="+((grbl.getSetting(10)>=0)? grbl.getSetting(10).ToString():"0"));
                            continue; }
                        if (dataField[i].IndexOf("Ln:") >= 0)            // Line number - needs to be enabled in config.h file
                        { machineState.Ln=lblSrLn.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("FS:") >= 0)            // Current Feed and Speed - This data field will always appear, unless it was explicitly disabled in the config.h file
                        { machineState.FS=lblSrFS.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("F:") >= 0)             // Current Feed - see above is speed is disabled in config.h
                        { machineState.FS=lblSrFS.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("Pn:") >= 0)            // Input Pin State - will not appear if No input pins are detected as triggered.
                        { machineState.Pn=lblSrPn.Text = data[1]; continue; }
                        if (dataField[i].IndexOf("Ov:") >= 0)            // Override Values - This data field will not appear if It is disabled in the config.h file
                        {   machineState.Ov=lblSrOv.Text = data[1]; lblSrPn.Text = "";

                            if (dataField[dataField.Length-1].IndexOf("A:") >= 0)             // Accessory State
                            {   machineState.A = lblSrA.Text = dataField[dataField.Length- 1].Split(':')[1]; }   
                            else
                            {   machineState.A = lblSrA.Text = ""; }
                            continue;
                        }
                    }
                }
                if (dataField[1].IndexOf("MPos") >= 0)
                {   grbl.getPosition(dataField[1], ref posMachine);
                    posWork = posMachine - posWCO;
                }
                else
                {   grbl.getPosition(dataField[1], ref posWork);
                    posMachine = posWork + posWCO;
                }
            }

            if (iamSerial == 1)
            {   if (!grbl.posChanged)
                    grbl.posChanged = ! (xyzPoint.AlmostEqual(grbl.posWCO, posWCO) && xyzPoint.AlmostEqual(grbl.posMachine, posMachine));
                if (!grbl.wcoChanged)
                    grbl.wcoChanged = !(xyzPoint.AlmostEqual(grbl.posWCO, posWCO));
                grbl.posWCO = posWCO; grbl.posWork = posWork; grbl.posMachine = posMachine;
            } // make it global

            gcodeVariable["MACX"] = posMachine.X; gcodeVariable["MACY"] = posMachine.Y; gcodeVariable["MACZ"] = posMachine.Z;
            gcodeVariable["WACX"] = posWork.X;   gcodeVariable["WACY"] = posWork.Y;   gcodeVariable["WACZ"] = posWork.Z;
            grblStateNow = grbl.parseStatus(status);
            lblSrState.BackColor = grbl.grblStateColor(grblStateNow);
            lblSrState.Text = grbl.statusToText(grblStateNow);  // status;

            lblSrPos.Text = posWork.Print(false, grbl.axisB || grbl.axisC); // show actual work position
            if (grblStateNow != grblStateLast) { grblStateChanged(); }
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));
            mParserState.changed = false;

            if ((grblStateNow == grblState.idle) || (grblStateNow == grblState.check))
            {   if (useSerial2 && _serial_form2.serialPortOpen)
                {
                    if (_serial_form2.grblStateNow == grblState.idle)
                        waitForIdle = false;
                    else
                        grblStateNow = _serial_form2.grblStateNow;
                }
                else
                    waitForIdle = false;
                if (externalProbe)
                {   posProbe = posMachine;
                    externalProbe = false;
                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.probe, machineState, mParserState, "($PROBE)"));
                    mParserState.changed = false;
                }
                processSend();
            }
            grblStateLast = grblStateNow;
            allowStreamingEvent = true;
        }
        public event EventHandler<PosEventArgs> RaisePosEvent;
        protected virtual void OnRaisePosEvent(PosEventArgs e)
        {   RaisePosEvent?.Invoke(this, e);  }

        #endregion


        // check free buffer before sending 
        // 1. requestSend(data) to add cleaned data to stack (sendLines) for sending / extract code for 2nd COM port
        // 2. processSend() check if grbl-buffer is free to take commands
        // 3. sendLine(data) if buffer can take commands
        // 4. updateStreaming(rxdata) check if command was sent
        private int grblBufferSize = grbl.RX_BUFFER_SIZE;               //rx bufer size of grbl on arduino 127
        private int grblBufferFree = grbl.RX_BUFFER_SIZE;               //actual suposed free bytes on grbl buffer
        private List<string> sendLines = new List<string>();
        private int sendLinesCount=0;             // actual buffer size
        private int sendLinesSent=0;              // actual sent line
        private int sendLinesConfirmed=0;         // already received line

        public int getFreeBuffer()
        { return ((int)(100*grblBufferFree/(float)grblBufferSize)); }

        /*  requestSend fill up send buffer, called by main-prog for single commands
         *  or called by preProcessStreaming to stream GCode data
         *  requestSend -> processSend -> sendLine
         * */
        public bool requestSend(string data, bool keepComments=false)
        {

            if ((isStreamingRequestPause) && (grblStateNow == grblState.run))
            {   addToLog("!!! Command blocked - wait for IDLE " + data); }
            else
            {   var tmp = cleanUpCodeLine(data, keepComments);
                if ((!string.IsNullOrEmpty(tmp)) && (tmp[0] != ';'))    // trim lines and remove all empty lines and comment lines
                {   if (tmp == "$#") preventEvent = 5;                  // no response echo for parser state
                    sendLines.Add(tmp);
                    sendLinesCount++;
                    processSend();
                    feedBackSettings(tmp);
                }
            }
            return serialPort.IsOpen;
        }
        private void feedBackSettings(string tmp)
        {
            if (!isStreaming || isStreamingPause)
            {
                tmp = tmp.Replace(" ", String.Empty);
                if (tmp.Contains("$32"))
                {
                    if (tmp.Contains("$32=1")) isLasermode = true;
                    if (tmp.Contains("$32=0")) isLasermode = false;
                    OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, 0, grblStreaming.lasermode));
                }
                if (tmp.IndexOf("$") >= 0)
                { btnCheckGRBLResult.Enabled = false; btnCheckGRBLResult.BackColor = SystemColors.Control; }
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
        private string cleanUpCodeLine(string data, bool keepComments = false)
        {
            var line = data.Replace("\r", "");  //remove CR
            line = line.Replace("\n", "");      //remove LF
            if (!keepComments)
            {   var orig = line;
                int start = orig.IndexOf('(');
                int end = orig.LastIndexOf(')');
                if (start >= 0) line = orig.Substring(0, start);
                if (end >= 0) line += orig.Substring(end + 1);
                // extract GCode for 2nd COM Port
                if ((start >= 0) && (end > start))  // send data to 2nd COM-Port
                {   var cmt = orig.Substring(start, end - start + 1);
                    if ((cmt.IndexOf("(^2") >= 0) || (cmt.IndexOf("($") == 0))
                    {   line += cmt;                // keep 2nd COM port data for further use
                    }
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
        private bool externalProbe = false;
        // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#eeprom-issues
        private string[] eeprom1 = { "G54", "G55", "G56", "G57", "G58", "G59"};
        private string[] eeprom2 = { "G10", "G28", "G30"};
        public void processSend()
        {
  //          Logger.Trace(" processSend() {0} {1}", sendLinesSent , sendLinesCount);

            while ((sendLinesSent < sendLinesCount) && (grblBufferFree >= sendLines[sendLinesSent].Length + 1))
            {
                var line = sendLines[sendLinesSent];
                bool replaced = false;

                if (!isStreaming)       // check tool change coordinates
                {   int cmdTNr = gcode.getIntGCode('T', line);
                    if (cmdTNr >= 0)
                    {   toolTable.init();       // fill structure
                        setToolChangeCoordinates(cmdTNr, line);
                        // save actual tool info as last tool info
                        gcodeVariable["TOLN"] = gcodeVariable["TOAN"];
                        gcodeVariable["TOLX"] = gcodeVariable["TOAX"];
                        gcodeVariable["TOLY"] = gcodeVariable["TOAY"];
                        gcodeVariable["TOLZ"] = gcodeVariable["TOAZ"];
                        gcodeVariable["TOLA"] = gcodeVariable["TOAA"];
                    }
                }
                if (line.Contains('#'))                      // check if variable neededs to be replaced
                {   line = insertVariable(line);
                    replaced = true;
   //                 if (grblBufferFree < grblBufferSize)
  //                      waitForIdle = true;
                }
                if (line.Contains("(^2"))                   // forward cmd to 2nd GRBL
                    if (grblBufferFree < grblBufferSize)
                        waitForIdle = true;

                for(int i=0; i < eeprom1.Length; i++)           // wait for IDLE beacuse of EEPROM access
                {   if (line.IndexOf(eeprom1[i]) >= 0)
                    {   if (grblBufferFree < grblBufferSize)
                            waitForIdle = true;
                        break;
                    }
                }
                for (int i = 0; i < eeprom2.Length; i++)        // wait for IDLE beacuse of EEPROM access
                {   if (line.IndexOf(eeprom2[i]) >= 0)
                    {   if (grblBufferFree < grblBufferSize)
                            waitForIdle = true;
                        break;
                    }
                }

                if ((!waitForIdle) || (grblStateNow == grblState.alarm))
                {   if (replaced)
                        sendLines[sendLinesSent] = line;    // needed to get correct length when receiving 'ok'
                            //  rtbLog.AppendText(string.Format("!!!> {0} {1}\r\n", line, sendLinesSent));
                    if (serialPort.IsOpen || grbl.grblSimulate)
                    {   sendLine(line);                         // now really send data to Arduino
/* decrease free buffer after sending code */
                        grblBufferFree -= (line.Length + 1);
                        if (cBStatus1.Checked || cBStatus.Checked)
                            addToLog(string.Format("TX> {0,-30} {1,2} {2,3}", line, line.Length, grblBufferFree));    // send, rec line 754

                        if (lastSentToCOM.Count > 10)
                            lastSentToCOM.Dequeue();            // store last sent commands via COM for error analysis
                        sendLinesSent++;
        /*                if (grbl.grblSimulate && !serialPort.IsOpen)
                        {   rxString = "ok";
                            this.Invoke(new EventHandler(handleRxData)); //handleRxData(this, null);
                        }*/
                    }
                    else
                    {   addToLog("!!! Port is closed !!!");
                        resetStreaming();   // ALARM
                    }


                    if (line.Contains("(^2"))
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
                    if (line.IndexOf("$TOOL") >=0) { grblStatus = grblStreaming.toolchange; }
                    if (line == "($TOOL-IN)")  { toolInSpindle = true; }
                    if (line == "($TOOL-OUT)") { toolInSpindle = false; }
                    if (line == "($END)")      { grblStatus = grblStreaming.ok; }
                    if (line == "($PROBE)")
                    { waitForIdle = true; externalProbe = true; }
                }
                else
                    return;
            }
        }

        /// <summary>
        /// sendLine - now really send data to Arduino
        /// </summary>
        private void sendLine(string data)
        {
            try
            {   if (serialPort.IsOpen)
                    serialPort.Write(data + "\r");
                if (!isHeightProbing && (!(isStreaming && !isStreamingPause)))// || (cBStatus1.Checked || cBStatus.Checked))
                {   if (!(cBStatus1.Checked || cBStatus.Checked || (preventOutput > 0))                   )
                        addToLog(string.Format("> {0}", data));     //if not in transfer log the txLine
                }
            }
            catch (Exception err)
            {   Logger.Error(err, "-sendLine-");
                logErr = err;
                logMessage = "Error reading line from serial port";
                if (!grbl.grblSimulate)
                    logError("! Sending line", err);
                updateControls();
            }
        }



        /// <summary>
        /// Clear all streaming counters
        /// </summary>
        private void resetStreaming(bool isStopping = true)
        {
            externalProbe = false;
            isStreaming = false;
            isStreamingRequestPause = false;
            isStreamingPause = false;
			isStreamingRequestStopp = false;
			if (isStopping)		// 20200717
            {	gCodeLinesSent = 0;
				gCodeLinesCount = 0;
				gCodeLinesConfirmed = 0;
				gCodeLinesTotal = 0;
				gCodeLines.Clear();
				gCodeLineNr.Clear();
				sendLinesSent = 0;
				sendLinesCount = 0;
				sendLinesConfirmed = 0;
				sendLines.Clear();
				grblBufferFree = grblBufferSize;
			}
            grbl.lastMessage = "";
        }

        private string insertVariable(string line)
        {
            int pos = 0, posold = 0;
            string variable, mykey = "";
            double myvalue = 0;
            if (line.Length > 5)        // min length needed to be replaceable: x#TOLX
            {   do
                {   pos = line.IndexOf('#', posold);
                    if (pos > 0)
                    {   myvalue = 0;
                        variable = line.Substring(pos, 5);
                        mykey = variable.Substring(1);
                        if (gcodeVariable.ContainsKey(mykey))
                        { myvalue = gcodeVariable[mykey]; }
                        else if (gui.variable.ContainsKey(mykey))
                        { myvalue = gui.variable[mykey]; }
                        else { line += " (" + mykey + " not found)"; }
                        if (cBStatus1.Checked || cBStatus.Checked)
                        { addToLog("< replace " + mykey + " = "+ myvalue.ToString()); }

                        line = line.Replace(variable, string.Format("{0:0.000}", myvalue));
                        //                  addToLog("replace "+ mykey+" by "+ myvalue.ToString());
                    }
                    posold = pos + 5;
                } while (pos > 0);
            }
            return line.Replace(',', '.');
        }


        public void realtimeCommand(byte cmd)
        {
            var dataArray = new byte[] { Convert.ToByte(cmd) };
            if (serialPort.IsOpen)
                serialPort.Write(dataArray, 0, 1);
            addToLog("> '0x" + cmd.ToString("X") + "' " + grbl.getRealtimeDescription(cmd));
            if ((cmd == 0x85) && !(isStreaming && !isStreamingPause))                   //  Jog Cancel
            {   sendLinesSent = 0;
                sendLinesCount = 0;
                sendLinesConfirmed = 0;
                sendLines.Clear();
                grblBufferFree = grblBufferSize;
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
        private bool isStreamingRequestStopp = false;    // 
        private bool getParserState = false;      // true to send $G after status switched to idle
        private bool isDataProcessing=false;      // false when no data processing pending
        private grblStreaming grblStatus = grblStreaming.ok;
        private grblStreaming oldStatus = grblStreaming.ok;
		
        public void stopStreaming(bool isNotStartup = true)
        {
            if (isStreamingCheck)
            {   sendLine("$C");
                isStreamingCheck = false;
            }

            if (isNotStartup)
                stopStreamingFinal();
            else
            {
                isHeightProbing = false;
                resetStreaming(false);      // stopStreaming
                isStreamingRequestStopp = true;         // 20200717
                isStreamingRequestPause = true;     // 20200717
                Logger.Trace(" stopStreaming() - wait for IDLE - lines in buffer {0}", (gCodeLinesSent - gCodeLinesConfirmed));
            }
        }
        public void stopStreamingFinal()
        {
						
            int line = 0;
            if ((gCodeLineNr != null) && (gCodeLinesSent < gCodeLineNr.Count))
            {   line = gCodeLineNr[gCodeLinesSent];
                sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStreaming.stop);   // stop
            }
            isHeightProbing = false;
     //       if (showMessage)
     //           addToLog("[STOP Streaming ("+line.ToString()+")]");
			
            resetStreaming();   // stopStreamingFinal()
            Logger.Trace(" stopStreamingFinal() - lines in buffer {0}", (gCodeLinesSent - gCodeLinesConfirmed));

            if (isStreamingCheck)
            {   sendLine("$C");
                isStreamingCheck = false;
            }
            updateControls();
            if (Properties.Settings.Default.grblPollIntervalReduce)
            {   timerSerial.Interval = grbl.pollInterval;
                failCounter = 10000 / timerSerial.Interval;
            }
        }
		
		
		
        public void pauseStreaming()
        {   if (!isStreamingPause)
            {   isStreamingRequestPause = true;     // wait until buffer is empty before switch to pause
                addToLog("[Pause streaming - wait for IDLE]");
                addToLog("[Save Settings]");
                grblStatus = grblStreaming.waitidle;
                getParserState = true;
            }
            else
            {   //if ((posPause.X != posWork.X) || (posPause.Y != posWork.Y) || (posPause.Z != posWork.Z))
                addToLog("++++++++++++++++++++++++++++++++++++");
                if (!xyzPoint.AlmostEqual(posPause, posWork))
                {
                    addToLog("[Restore Position]");
                    requestSend(string.Format("G90 G0 X{0:0.000} Y{1:0.000}", posPause.X, posPause.Y).Replace(',', '.'));  // restore last position
                    string noG = parserStateGC.Substring(parserStateGC.IndexOf("M")-1);
                    addToLog("[Restore Settings: " + noG + " ]");
                    requestSend(noG);           // restore actual GCode settings one by one
                    requestSend("G4 P2");       // wait 2 seconds
                    requestSend(string.Format("G1 Z{0:0.000}", posPause.Z).Replace(',', '.'));                      // restore last position
                }
                addToLog("[Start streaming - no echo]");
                addToLog("[Restore Settings: "+ parserStateGC+" ]");
                isStreamingPause = false;
                isStreamingRequestPause = false;
                grblStatus = grblStreaming.ok;
                requestSend(parserStateGC);         // restore actual GCode settings one by one
                gCodeLinesConfirmed--;              // each restored setting will cause 'ok' and gCodeLinesConfirmed++
                Logger.Debug("continue streaming after pause gCodeLinesSent {0} gCodeLinesCount {1} grblBufferFree {2} length {3} waitForIdle {4}", gCodeLinesSent, gCodeLinesCount, grblBufferFree, (gCodeLines[gCodeLinesSent].Length + 1), waitForIdle);
                preProcessStreaming();
            }
            updateControls();
        }

        /*  startStreaming called by main-Prog
         *  get complete GCode list and copy to own list
         *  initialize streaming
         *  if startAtLine > 0 start with pause
         * */
        public void startStreaming(IList<string> gCodeList, int startAtLine, bool check = false)
        {
            grblBufferSize = grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
            grblBufferFree = grbl.RX_BUFFER_SIZE;
            if (Properties.Settings.Default.grblPollIntervalReduce)
            {   timerSerial.Interval = grbl.pollInterval * 2;
                failCounter = 10000 / timerSerial.Interval;
            }

            lastError = "";
            lastSentToCOM.Clear();
            toolTable.init();       // fill structure
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
            isStreamingRequestPause = false;
            isStreamingCheck = check;
            grblStatus = grblStreaming.ok;
            string[] gCode = gCodeList.ToArray<string>();
            gCodeLines = new List<string>();
            gCodeLineNr = new List<int>();
            resetStreaming();       // startStreaming
            if (isStreamingCheck)
            {   sendLine("$C");
                grblBufferSize = 100;  //reduce size to avoid fake errors
            }

            string tmp;
            double pWord,lWord, oWord;
            string subline;
            for (int i = startAtLine; i < gCode.Length; i++)
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
            updateControls();
            if (startAtLine > 0)
            {  // pauseStreaming();
                isStreamingPause = true;
            }
            else
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
            if (num.Length<1)
                return notfound;
            return double.Parse(num, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        /*  preProcessStreaming copy line by line (requestSend(line)) to sendBuffer 
         *  if buffer free, to be able to track line-nr for feedback
         * */
        private void preProcessStreaming()
        {   while ((gCodeLinesSent < gCodeLinesCount) && (grblBufferFree >= gCodeLines[gCodeLinesSent].Length + 1) && !waitForIdle)
            {
                string line = gCodeLines[gCodeLinesSent];
                int cmdMNr = gcode.getIntGCode('M',line);
                int cmdGNr = gcode.getIntGCode('G',line);
                int cmdTNr = gcode.getIntGCode('T', line);
                if (grbl.unknownG.Contains(cmdGNr))
                {
                    gCodeLines[gCodeLinesSent] = "(" + line + " - unknown)";    // don't pass unkown GCode to GRBL because is unknown
                    line = gCodeLines[gCodeLinesSent];
                    gCodeLinesConfirmed++;                                      // GCode is count as sent (but wasn't send) also count as received
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
                // regular GCode expression 'T'
                if (cmdTNr >= 0) //&& (line.IndexOf("T") == 0) && (line.IndexOf("#T") < 0) && (line.IndexOf("$T") < 0))
                {   // T-word is allowed by grbl - no need to filter
                    setToolChangeCoordinates(cmdTNr, line);
                }
                if (cmdMNr == 6)
                {
                    if (Properties.Settings.Default.ctrlToolChange)
                    {   // insert script code into GCODE
                        int index = gCodeLinesSent + 1;
                        int linenr = gCodeLineNr[gCodeLinesSent];
                        grblStatus = grblStreaming.toolchange;
        //                sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStatus);   // tool change M6
                        index = insertComment(index, linenr, "($TOOL-START)");
                        addToLog("\r[TOOL change: T" + gcodeVariable["TOAN"].ToString() + " at " + gcodeVariable["TOAX"].ToString() + " , " + gcodeVariable["TOAY"].ToString() + " , " + gcodeVariable["TOAZ"].ToString() + "]");
                        if (toolInSpindle)
                        {   addToLog("[TOOL run script 1) " + Properties.Settings.Default.ctrlToolScriptPut + "  T" + gcodeVariable["TOLN"].ToString() + " at " + gcodeVariable["TOLX"].ToString() + " , " + gcodeVariable["TOLY"].ToString() + " , " + gcodeVariable["TOLZ"].ToString() + "]");
                            index = insertCode(Properties.Settings.Default.ctrlToolScriptPut, index, linenr, true);
                            index = insertComment(index, linenr, "($TOOL-OUT)");
                        }
                        addToLog("[TOOL run script 2) " + Properties.Settings.Default.ctrlToolScriptSelect + "]");
                        index = insertCode(Properties.Settings.Default.ctrlToolScriptSelect,index, linenr,true);
                        addToLog("[TOOL run script 3) " + Properties.Settings.Default.ctrlToolScriptGet + "]");
                        index = insertCode(Properties.Settings.Default.ctrlToolScriptGet,   index, linenr, true);
                        index = insertComment(index, linenr, "($TOOL-IN)");
                        addToLog("[TOOL run script 4) " + Properties.Settings.Default.ctrlToolScriptProbe + "]");
                        index = insertCode(Properties.Settings.Default.ctrlToolScriptProbe, index, linenr, true);
                        index = insertComment(index, linenr, "($END)");

                        // save actual tool info as last tool info
                        gcodeVariable["TOLN"] = gcodeVariable["TOAN"];
                        gcodeVariable["TOLX"] = gcodeVariable["TOAX"];
                        gcodeVariable["TOLY"] = gcodeVariable["TOAY"];
                        gcodeVariable["TOLZ"] = gcodeVariable["TOAZ"];
                        gcodeVariable["TOLA"] = gcodeVariable["TOAA"];

                        grblStatus = grblStreaming.toolchange;
                        sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStatus);   // tool change M6
                    }
                    gCodeLines[gCodeLinesSent] = "($" + line + ")";  // don't pass M6 to GRBL because is unknown
                    line = gCodeLines[gCodeLinesSent];
                    gCodeLinesConfirmed++;      // M6 is count as sent (but wasn't send) also count as received
                }
                if (cmdMNr == 30)
                {
                    if (Properties.Settings.Default.ctrlToolChange)
                    {   // insert script code into GCODE
                        int index = gCodeLinesSent + 1;
                        int linenr = gCodeLineNr[gCodeLinesSent];
                        grblStatus = grblStreaming.toolchange;
                        sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStatus);   // tool change M30

                        if (toolInSpindle)
                        {
                            addToLog("[TOOL run script 1) " + Properties.Settings.Default.ctrlToolScriptPut + "  T" + gcodeVariable["TOLN"].ToString() + " at " + gcodeVariable["TOLX"].ToString() + " , " + gcodeVariable["TOLY"].ToString() + " , " + gcodeVariable["TOLZ"].ToString() + "]");
                            index = insertCode(Properties.Settings.Default.ctrlToolScriptPut, index, linenr, true);
                            index = insertComment(index, linenr, "($TOOL-OUT)");
                        }
                    }
                }
                if ((cmdMNr == 0) && !isStreamingCheck) // M0 request pause
                {
                    isStreamingRequestPause = true;
                    addToLog("[Pause streaming]");
                    addToLog("[Save Settings]");
                    grblStatus = grblStreaming.waitidle;
                    getParserState = true;
                    sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStatus);   // wait for idle
                    gCodeLinesSent++;
                    return;                 // abort while - don't fill up buffer
                }
                requestSend(line);
                gCodeLinesSent++;
            }
        }

        private void setToolChangeCoordinates(int cmdTNr, string line="")
        { 
            toolProp toolInfo = toolTable.getToolProperties(cmdTNr);
            if (toolInfo.toolnr != cmdTNr)
            {
                addToLog("\r[TOOL change: " + cmdTNr.ToString() + " no Information found! (" + line + ")]");
            }
            else
            {   // get new values
//                addToLog("\r[set tool coordinates "+ cmdTNr.ToString() + "]");
                gcodeVariable["TOAN"] = cmdTNr;
                gcodeVariable["TOAX"] = (double)toolInfo.X + (double)Properties.Settings.Default.toolTableOffsetX;
                gcodeVariable["TOAY"] = (double)toolInfo.Y + (double)Properties.Settings.Default.toolTableOffsetY;
                gcodeVariable["TOAZ"] = (double)toolInfo.Z + (double)Properties.Settings.Default.toolTableOffsetZ;
                gcodeVariable["TOAA"] = (double)toolInfo.A + (double)Properties.Settings.Default.toolTableOffsetA;
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
                string[] commands = fileCmd.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string tmp;
                foreach (string cmd in commands)
                {
                    tmp = cleanUpCodeLine(cmd);         // remove comments
                    if (replace)
                        tmp = insertVariable(tmp);
                    if (tmp.Length > 0)
                    {
                        gCodeLineNr.Insert(index,linenr);
                        gCodeLines.Insert(index, tmp);
                        index++;
                        gCodeLinesCount++;
                    }
                }
            }
            return index;
        }


        private void grblStateChanged()
        {   if ((sendLinesConfirmed == sendLinesCount) && (grblStateNow == grblState.idle))   // addToLog(">> Buffer empty\r");
            {
                if (isStreamingRequestPause || isStreamingRequestStopp)
                {
                    isStreamingPause = true;
                    isStreamingRequestPause = false;
                    grblStatus = grblStreaming.pause;
					addToLog("---------- IDLE state reached ---------");
					
                    if (getParserState)
                    {   
                        requestSend("$G");
                    }
					
					if (isStreamingRequestStopp)	// 20200717
					{	
			//			resetStreaming();
			//			sendStreamEvent(gCodeLinesSent, gCodeLinesConfirmed, grblStreaming.stop);
						Logger.Trace(" grblStateChanged() - now really stop - resetStreaming");
						stopStreamingFinal();
					}

                    updateControls();
                }
            }
        }

        public event EventHandler<StreamEventArgs> RaiseStreamEvent;
        protected virtual void OnRaiseStreamEvent(StreamEventArgs e)
        {
            //addToLog("OnRaiseStreamEvent " + e.Status.ToString());
            //RaisePosEvent?.Invoke(this, e);
			EventHandler<StreamEventArgs> handler = RaiseStreamEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        { rtbLog.Clear(); }
        private void tbCommand_KeyPress(object sender, KeyPressEventArgs e)
        {   if (e.KeyChar != (char)13) return;
            btnSend_Click(sender, e);
        }
        private void btnSend_Click(object sender, EventArgs e)
        {   if (!isStreaming || isStreamingPause)
            {
                string cmd = cBCommand.Text.ToUpper();
                cBCommand.Items.Remove(cBCommand.SelectedItem);
                cBCommand.Items.Insert(0, cmd);
                requestSend(cmd);
                cBCommand.Text = cmd;
            }
        }
        private void btnGRBLCommand0_Click(object sender, EventArgs e)
        { requestSend("$"); }

        private int callCheckGRBL = 0;
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

 //               MessageBox.Show(strCheckResult, "Information");
                GRBLSettings.Clear();
            }
            else
            {   if (grblStateNow == grblState.idle)
                {   requestSend("$$"); //GRBLSettings.Clear();
                    callCheckGRBL = 4;                         // timer1 will recall this function after 2 seconds
                }
                else {
                    addToLog("Wait for IDLE, then try again!");
                }
            }
        }
        private string strCheckResult = "";
        private void btnCheckGRBLResult_Click(object sender, EventArgs e)
        {   MessageBox.Show(strCheckResult, "Information");
        }

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

        public void readSettings()
        {
            preventOutput = 10; preventEvent = 10;
            requestSend("$$");  // get setup
            requestSend("$#");  // get parameter
        }
    }
}
