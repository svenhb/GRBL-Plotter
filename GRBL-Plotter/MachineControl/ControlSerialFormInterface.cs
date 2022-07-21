/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
/* 2020-09-18 split file
 * 2020-12-18 add CodeBuffer max
 * 2020-12-27 add Marlin support
 * 2021-01-15 add lineEndRXTX
 * 2021-01-16 don't apply toUpper to comment (^2, (^3
 * 2021-02-01 line 880 remove waitForOk
 * 2021-03-05 add reset info to logger, check index-range in processGrblRealTimeStatus line 349
 * 2021-03-26 sendTo3rdCOM find 1st ')' not last
 * 2021-04-12 line 876 only send setup-command '$...' if system is IDLE
 * 2021-10-14 grbl 0.9 fix $10=3
 * 2021-11-23 line 446 check dataField.Length, line 793 add if (serialPort.IsOpen) 
 * 2021-12-13 replace serialPort.Write by SerialPortDataSend (in ControlSerialForm.cs)
 * 2021-12-14 add run time for spindle, flood, mist
 * 2021-12-21 line 819 replace serialPort.Write by 	SerialPortDataSend
 * 2022-01-03 InsertVariable error handling
 * 2022-01-07 rework ResetVariables
 * 2022-02-14 line 1035 use of 3rd, reset counter
 * 2022-04-08 line 120 force Marlin check
 * 2022-04-15 extend MarlinReset(), line 390 avoid sendBuffer.Clear, if Marlin
 * 2022-04-18 line 951 if (splt.Length < 2) return;
 * 2022-05-27 line 1496 add method MissingConfirmationLength()
*/

// OnRaiseStreamEvent(new StreamEventArgs((int)lineNr, codeFinish, buffFinish, status));
// OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlSerialForm : Form        // Form can be loaded twice!!! COM1, COM2
    {
        private XyzPoint posWCO, posWork, posMachine;
        internal XyzPoint posPause, posProbe, posProbeOld;
        private readonly ModState machineState = new ModState();     // Keep info about Bf, Ln, FS, Pn, Ov, A from grbl status
        private ParsState mParserState = new ParsState();     // keep info about last M and G settings from GCode
        private bool resetProcessed = false;
        private GrblState grblStateNow = GrblState.unknown;
        private GrblState grblStateLast = GrblState.unknown;
        private string lastMessage = "";

        public bool IsGrblVers0 { get; private set; } = true;
        public string GrblVers { get; private set; } = "";
        public bool IsLasermode { get; private set; } = false;
        public bool ToolInSpindle { get; set; } = false;
        public bool IsHeightProbing { get; set; } = false;      // automatic height probing -> less feedback
        internal List<string> GRBLSettings = new List<string>();          // keep $$ settings
        private readonly Queue<string> lastSentToCOM = new Queue<string>();      // store last sent commands via COM
        private int rtsrResponse = 0;     // real time status report sent / receive differnence - should be zero.                    

        private readonly Dictionary<string, double> gcodeVariable = new Dictionary<string, double>();    // keep variables "PRBX" etc.
        internal string parserStateGC = "";                  // keep parser state response [GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]

        private int grblBufferSize = Grbl.RX_BUFFER_SIZE;               //rx bufer size of grbl on arduino 127
        private int grblBufferFree = Grbl.RX_BUFFER_SIZE;               //actual suposed free bytes on grbl buffer
        private bool grblCharacterCounting = true;                      //if false, then Simple Send-Response https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#streaming-a-g-code-program-to-grbl
                                                                        // https://github.com/Smoothieware/Smoothieware/issues/1369
        private readonly CodeBuffer sendBuffer = new CodeBuffer();

        private bool waitForIdle = false;
        private bool waitForOk = false;
        private bool externalProbe = false;
        private bool isHoming = false;

        private int useSerial3LastBufferFree = 0;       // avoid lock in line 1020 "process 3rd free:109  size:127"
        private int useSerial3LastBufferFreeSame = 0;


        private readonly Stopwatch stopwatchSpindle = new Stopwatch();
        private readonly Stopwatch stopwatchFlood = new Stopwatch();
        private readonly Stopwatch stopwatchMist = new Stopwatch();

        private readonly object sendDataLock = new object();
        private readonly object receiveDataLock = new object();

        public event EventHandler<PosEventArgs> RaisePosEvent;
        protected virtual void OnRaisePosEvent(PosEventArgs e)
        { RaisePosEvent?.Invoke(this, e); }

        public int GetFreeBuffer()
        { return ((int)(100 * grblBufferFree / (float)grblBufferSize)); }

        private string ListInfoSend()
        { return string.Format("{0}sendBuffer snt:{1,3}  cnfrmnd:{2,3}  cnt:{3,5}  BFree:{4,3}  lineNr:{5}  code:'{6}' state:{7}", "", sendBuffer.IndexSent, sendBuffer.IndexConfirmed, sendBuffer.Count, grblBufferFree, sendBuffer.GetConfirmedLineNr(), sendBuffer.GetConfirmedLine(), grblStateNow.ToString()); }


        #region processGRBL
        /***** Seperate grbl / Marlin support *****/
        private void ProcessMessages(object sender, EventArgs e)	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#message-summary
        {
            if (string.IsNullOrEmpty(rxString)) { isDataProcessing = false; return; }     // unlock serialPort1_DataReceived
            if (countShutdown > 0) { isDataProcessing = false; return; }
            if ((rxString[0] < 32))
            {
                AddToLog("-" + rxString);
                if (rxString.Contains("M11"))
                    isMarlin = true;
                isDataProcessing = false;                   // unlock serialPort1_DataReceived
                return;
            }

            if (!isMarlin)
                ProcessGrblMessages(sender, e);
            else
                ProcessMarlinMessages(sender, e);

            isDataProcessing = false;                   // unlock serialPort1_DataReceived
        }

        private void ProcessMarlinMessages(object sender, EventArgs e)	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#message-summary
        {
            bool isOk = rxString.StartsWith("ok");
            bool isMarlinEcho = rxString.StartsWith("echo:") || rxString.StartsWith("start") || rxString.StartsWith("Marlin");

            /***** buffer processed *****/
            if (isOk)
            {
                ProcessGrblOkMessage();
                ProcessOkStreaming();                          // process all other messages
                if (!getMarlinPositionWasSent)
                {
                    if (!isStreaming || isStreamingPause)
                    {
                        if (!(IsHeightProbing || (cBStatus1.Checked || cBStatus.Checked || (countPreventOutput > 0))))
                            AddToLog("< ok");   // string.Format("< {0}", rxString)); // < ok
                    }
                }
                else
                    getMarlinPositionWasSent = false;

                if (isStreaming)
                {
                    if (streamingBuffer.IndexConfirmed >= streamingBuffer.Count)
                    { StreamingFinish(); }
                }
            }
            /***** Marlin start up *****/
            else if (isMarlinEcho)
            {
                if (rxString.Contains("Marlin"))
                {
                    MarlinReset();
                }    // set global flag
                AddToLog(rxString);
            }

            else
            {
                // M114 response: X:0.00 Y:0.00 Z:0.00 E:0.00 Count X:0 Y:0 Z:0
                if (rxString.Contains("X:") && rxString.Contains("Y:") && rxString.Contains("Z:"))
                {
                    string[] axis = rxString.Split(' ');
                    if (axis.Length > 3)
                        axisCount = Grbl.GetPosition(iamSerial, "WPOS:" + axis[0].Substring(2) + "," + axis[1].Substring(2) + "," + axis[2].Substring(2) + "," + axis[3].Substring(2) + " ", ref posWork);
                    else if (axis.Length > 2)
                        axisCount = Grbl.GetPosition(iamSerial, "WPOS:" + axis[0].Substring(2) + "," + axis[1].Substring(2) + "," + axis[2].Substring(2) + " ", ref posWork);
                    else if (axis.Length > 1)
                        axisCount = Grbl.GetPosition(iamSerial, "WPOS:" + axis[0].Substring(2) + "," + axis[1].Substring(2) + " ", ref posWork);
                    posMachine = posWork;
                    ProcessGrblPositionUpdate();        // 1st status would be reset
                    grblStateNow = GrblState.Marlin;
                    rtsrResponse--;
                    if (cBStatus.Checked) AddToLog(rxString);
                    //                    timerSerial.Interval = 250;
                }
                else
                    AddToLog(rxString);
            }

            isDataProcessing = false;                   // unlock serialPort1_DataReceived
        }

        private void MarlinReset()
        {
            isMarlin = true;
            grblStateNow = GrblState.reset;
            if (isStreaming) SendStreamEvent(GrblStreaming.reset);
            if (iamSerial == 1) { Grbl.isMarlin = true; }
            AddToLog("Set Marlin mode");
            ProcessWelcomeMessage();
            SerialPortDataSend("M114" + lineEndTXmarlin);       // marlin
            getMarlinPositionWasSent = true;
            EventCollector.SetStreaming("Marlin");
            SendResetEvent();
        }

        /********************************************************************************* 
        * processGrblMessages - called in RX event chain, from serialPort1_DataReceived()
        * 1) processGrblOkMessage() processOkStreaming();   			process 'ok'
        * 2) processGrblRealTimeStatus, processGrblPositionUpdate, 	process '< Idle | MPos:0.000,0.000,0.000 | FS:0,0 | WCO:0.000,0.000,0.000 >'
        *    processGrblStateChange
        * 3) processGrblWelcomeMessage		'Reset'
        * 4) processGrblFeedbackMessage	'[GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]'
        * 5) processGrblAlarmMessage
        * 6) processGrblErrorMessage
        * 7) processGrblUserQuery			'$$'
        *********************************************************************************/
        private void ProcessGrblMessages(object sender, EventArgs e)	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#message-summary
        {
            int tmp;
            char[] charsToTrim = { '<', '>', '[', ']', ' ' };
            bool isStatusReport = (((tmp = rxString.IndexOf('<')) >= 0) && (rxString.IndexOf('>') > tmp));
            bool isOk = rxString.StartsWith("ok");
            bool isMarlinEcho = false;

            if (isStatusReport)
            { if (logReceiveStatus) Logger.Trace("s{0} RX '{1}'", iamSerial, rxString); }
            else
            {
                if (isOk)   // sendBuffer.GetConfirmedLine = index = sinnlos wg. FeedbackMessage
                { }//if (logReceive) Logger.Trace("Ser:{0} RX '{1}'  sent:'{2}'  line:{3}   BufferFree:{4}", iamSerial, rxString, sendBuffer.GetConfirmedLine(), streamingBuffer.GetConfirmedLineNr(), grblBufferFree); }
                else
                {
                    if (logReceive) Logger.Trace("s{0} RX '{1}'", iamSerial, rxString);
                    isMarlinEcho = rxString.StartsWith("echo:") || rxString.StartsWith("start") || rxString.StartsWith("Marlin");
                }
            }

            /***** action by occurance / importance *****/
            /***** grbl buffer processed *****/
            if (isOk)
            {
                ProcessGrblOkMessage();
                ProcessOkStreaming();                          // process all other messages
                rxString = "";                              // clear if simulation is on
                if (!isStreaming || isStreamingPause)
                {
                    if (!(IsHeightProbing || (cBStatus1.Checked || cBStatus.Checked || (countPreventOutput > 0))))
                        AddToLog("< ok");   // string.Format("< {0}", rxString)); // < ok
                }
            }
            /***** Marlin start up *****/
            else if (isMarlinEcho)
            {
                if (rxString.Contains("Marlin") || rxString.Contains("Unknown command:"))
                { MarlinReset(); }    // set global flag
                AddToLog(rxString);
            }

            /***** Process status message with coordinates *****/
            else if (isStatusReport)
            {
                ProcessGrblRealTimeStatus(rxString.Trim(charsToTrim));  // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#real-time-status-reports
                ProcessGrblPositionUpdate();
                ProcessGrblStateChange();
                rtsrResponse--;                             			// real time status report received                    			
                if (cBStatus.Checked) AddToLog(rxString);
            }

            /***** reset message *****/
            else if (rxString.ToLower().IndexOf("['$' for help]") >= 0)
            {
                Grbl.isMarlin = isMarlin = false;
                ProcessGrblWelcomeMessage(rxString);    // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#welcome-message
            }

            /***** Process feedback message with coordinates  *****/
            else if (((tmp = rxString.IndexOf('[')) >= 0) && (rxString.IndexOf(']') > tmp))
            {
                ProcessGrblFeedbackMessage(rxString.Trim(charsToTrim).Split(':'));  // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#feedback-messages
                if (cBStatus1.Checked || cBStatus.Checked)
                {
                    AddToLog(string.Format("RX> {0} ", rxString));
                    Logger.Info("RX> {0} ", rxString);
                }
                else if (!IsHeightProbing || cBStatus.Checked)
                {
                    if (countPreventOutput == 0)
                        AddToLog(rxString);
                }
            }

            else if (rxString.ToUpper().IndexOf("ALARM") >= 0)
            { ProcessGrblAlarmMessage(rxString); }  // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#alarm-message

            else if (rxString.ToUpper().IndexOf("ERROR") >= 0)
            { ProcessGrblErrorMessage(rxString); }   // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#grbl-response-messages

            /***** Show GRBL Settings Info if Version is >= 1.0  *****/
            else if ((rxString.IndexOf("$") >= 0) && (rxString.IndexOf("=") >= 0))
            { ProcessGrblUserQuery(rxString); }

            isDataProcessing = false;                   // unlock serialPort1_DataReceived
        }

        /***** processGrblOkMessage 'Ok' *****/
        private void ProcessGrblOkMessage()
        {
            int receivedByteCount = sendBuffer.LengthConfirmed() + 1;   // + "\r"
            Grbl.UpdateParserState(iamSerial, sendBuffer.GetConfirmedLine(), ref mParserState);

            /***** increase free buffer if ok was received *****/
            lock (receiveDataLock)
            {
                receivedByteCount = sendBuffer.LengthConfirmed() + 1;   // + "\r"
                grblBufferFree += receivedByteCount;   	                // update bytes supose to be free on grbl rx bufer
                if (!grblCharacterCounting)
                    grblBufferFree = Grbl.RX_BUFFER_SIZE;

                if (logReceive || cBStatus1.Checked || cBStatus.Checked) Logger.Trace("s{0} RX '{1,-20}' length:{2,2}  BufferFree:{3,3}  Index:{4,3}  max:{5,3}  lineNr:{6}", iamSerial, sendBuffer.GetConfirmedLine(), receivedByteCount, grblBufferFree, sendBuffer.IndexConfirmed, sendBuffer.Count, sendBuffer.GetConfirmedLineNr());
                sendBuffer.LineWasReceived();           // inc counter      TX in line 737

                if (grblBufferFree >= Grbl.RX_BUFFER_SIZE)
                {
                    waitForOk = false;  // matching ok was received
                                        //                    if (logReceive) Logger.Trace("waitForOk = false", grblBufferFree);
                }

                if (grblBufferFree > Grbl.RX_BUFFER_SIZE)
                {
                    grblBufferFree = Grbl.RX_BUFFER_SIZE;
                    waitForOk = false;
                    if (sendBuffer.Count > 0)
                    {
                        Logger.Warn("⚠ grblBufferFree too big! {0} rx:'{1}' in processGrblOkMessage() - fix | last RX:'{2}' RX-1:'{3}' RX-2:'{4}'", grblBufferFree, rxString, sendBuffer.GetConfirmedLine(), sendBuffer.GetConfirmedLine(-1), sendBuffer.GetConfirmedLine(-2));
                        Logger.Info("⚠ {0}", ListInfoSend());
                        Logger.Info("⚠ {0}", ListInfoStream());
                    }
                }
            }   // lock
            string rxLine = sendBuffer.GetConfirmedLine();

            if (rxLine.Contains("($TS")) { AddToLog("[Tool change start]"); }
            else if (rxLine.Contains("($TO")) { AddToLog("[Tool " + rxLine.Substring(4).Trim(')') + " removed]"); ToolInSpindle = false; }
            else if (rxLine.Contains("($TI")) { AddToLog("[Tool " + rxLine.Substring(4).Trim(')') + " selected]"); ToolInSpindle = true; }
            else if (rxLine == "($TE)") { AddToLog("[Tool change finished]"); }
            else if (rxLine == "($END)")
            {
                AddToLog("[Program end: " + GetTimeStampString() + " ]");
                StreamingFinish();      // reset streaming, clear sendBuffer, streamingBuffer, ListAccessoryStateRunTime
                                        //    RequestSend("$G");	// read actual gcode parameter
            }

            if ((cBStatus1.Checked || cBStatus.Checked) && (!isMarlin || isStreaming))  // TX in line 737
            { AddToLog(string.Format("RX< {0,-30} {1,2} {2,3}  line-Nr:{3}", sendBuffer.GetConfirmedLine(), receivedByteCount, grblBufferFree, (sendBuffer.GetConfirmedLineNr() + 1))); }

            if ((mParserState.changed) && (grblStateNow != GrblState.probe))    // probe will be send later
            {
                OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                mParserState.changed = false;
            }
            if (isHoming)
            { isHoming = false; rtsrResponse = 0; countMissingStatusReport = (int)(10000 / timerSerial.Interval); }

            if (!waitForOk)
            {
                if (logReceive) Logger.Trace("in RX start processSend");
                if (isStreaming && !isStreamingRequestPause && !isStreamingPause)
                {
                    Application.DoEvents();
                    PreProcessStreaming();              // ProcessGrblOkMessage
                }
            }

            if (!isStreaming)
            {
                if (!isMarlin && (sendBuffer.IndexConfirmed > sendBuffer.Count))  // nok
                {
                    Logger.Warn("⚠ processGrblOkMessage  fix overflow  IndexConfirmed:{0}  Count:{1}", sendBuffer.IndexConfirmed, sendBuffer.Count);
                    sendBuffer.Clear();
                }
            }
        }

        /**********************************************************************************************
         * processGrblRealTimeStatus
         * should occur with same frequent as timer interrupt -> each 200ms
         * old:         <Idle,MPos:0.000,0.000,0.000,WPos:0.000,0.000,0.000>
         * new in 1.1   < Idle | MPos:0.000,0.000,0.000 | FS:0,0 | WCO:0.000,0.000,0.000 >
         **********************************************************************************************/
        private void ProcessGrblRealTimeStatus(string text)    // '<' and '>' already removed
        {
            char splitAt = '|';
            if (IsGrblVers0)
                splitAt = ',';
            string[] dataField = text.Split(splitAt);
            if (dataField.Length < 1)   //<= 2)
            {
                Logger.Error("processGrblRealTimeStatus dataField.Length<=2: '{0}'", text);
                return;
            }
            string status = dataField[0].Trim(' ');

            if (IsGrblVers0)	//	handle old format from grbl vers. 0.9
            {
                if (dataField.Length > 3)   // get 1st part
                {
                    if (dataField[1].StartsWith("WPos"))
                    {
                        axisCount = Grbl.GetPosition(iamSerial, dataField[1] + "," + dataField[2] + "," + dataField[3] + " ", ref posWork);
                        posMachine = posWork + posWCO;
                    }
                    else
                    {
                        axisCount = Grbl.GetPosition(iamSerial, dataField[1] + "," + dataField[2] + "," + dataField[3] + " ", ref posMachine);
                        posWork = posMachine - posWCO;
                    }
                }
                if (dataField.Length > 6)   // get 2nd part -index 1-3 = posMachine
                {
                    Grbl.GetPosition(iamSerial, dataField[4] + "," + dataField[5] + "," + dataField[6] + " ", ref posWork);
                    posWCO = posMachine - posWork;
                }
            }
            else
            {
                if (dataField.Length > 2)
                {
                    for (int i = 2; i < dataField.Length; i++)
                    {
                        if (dataField[i].IndexOf("WCO") >= 0)           // Work Coordinate Offset
                        {
                            Grbl.GetPosition(iamSerial, dataField[i], ref posWCO);
                            continue;
                        }
                        string[] data = dataField[i].Split(':');
                        if (data.Length > 1)
                        {
                            if (dataField[i].IndexOf("Bf:") >= 0)            // Buffer state - needs to be enabled in config.h file
                            {
                                machineState.Bf = lblSrBf.Text = data[1];
                                //    if (Grbl.GetBufferSize(data[1])) RequestSend("$10=" + ((Grbl.GetSetting(10) >= 0) ? Grbl.GetSetting(10).ToString() : "0"));
                                //    continue;
                            }
                            if (dataField[i].IndexOf("Ln:") >= 0)            // Line number - needs to be enabled in config.h file
                            { machineState.Ln = lblSrLn.Text = data[1]; continue; }
                            if (dataField[i].IndexOf("FS:") >= 0)            // Current Feed and Speed - This data field will always appear, unless it was explicitly disabled in the config.h file
                            { machineState.FS = lblSrFS.Text = data[1]; continue; }
                            if (dataField[i].IndexOf("F:") >= 0)             // Current Feed - see above is speed is disabled in config.h
                            { machineState.FS = lblSrFS.Text = data[1]; continue; }
                            if (dataField[i].IndexOf("Pn:") >= 0)            // Input Pin State - will not appear if No input pins are detected as triggered.
                            { machineState.Pn = lblSrPn.Text = data[1]; continue; }
                            if (dataField[i].IndexOf("Ov:") >= 0)            // Override Values - This data field will not appear if It is disabled in the config.h file
                            {
                                machineState.Ov = lblSrOv.Text = data[1]; lblSrPn.Text = "";

                                if (dataField[dataField.Length - 1].IndexOf("A:") >= 0)             // Accessory State
                                {
                                    machineState.A = lblSrA.Text = dataField[dataField.Length - 1].Split(':')[1];
                                    if (iamSerial == 1)
                                    {
                                        if (machineState.A.Contains("S") || machineState.A.Contains("C"))
                                        { stopwatchSpindle.Start(); }
                                        else
                                        { AddToRunTimer(stopwatchSpindle, "grblRunTimeSpindle"); }

                                        if (machineState.A.Contains("F"))
                                        { stopwatchFlood.Start(); }
                                        else
                                        { AddToRunTimer(stopwatchFlood, "grblRunTimeFlood"); }

                                        if (machineState.A.Contains("M"))
                                        { stopwatchMist.Start(); }
                                        else
                                        { AddToRunTimer(stopwatchMist, "grblRunTimeMist"); }
                                    }
                                }
                                else
                                {
                                    machineState.A = lblSrA.Text = "";
                                    AddToRunTimer(stopwatchSpindle, "grblRunTimeSpindle");
                                    AddToRunTimer(stopwatchFlood, "grblRunTimeFlood");
                                    AddToRunTimer(stopwatchMist, "grblRunTimeMist");
                                }
                                continue;
                            }
                        }
                    }
                }
                if (dataField.Length >= 2)
                {
                    if (dataField[1].IndexOf("MPos") >= 0)
                    {
                        axisCount = Grbl.GetPosition(iamSerial, dataField[1], ref posMachine);
                        posWork = posMachine - posWCO;
                    }
                    else
                    {
                        axisCount = Grbl.GetPosition(iamSerial, dataField[1], ref posWork);
                        posMachine = posWork + posWCO;
                    }
                }
            }   // if (isGrblVers0)
            grblStateNow = Grbl.ParseStatus(status);            // get actual state
            lblSrState.BackColor = Grbl.GrblStateColor(grblStateNow);
            lblSrState.Text = Grbl.StatusToText(grblStateNow);  // status;

            lblSrPos.Text = posWork.Print(false, Grbl.axisB || Grbl.axisC); // show actual work position
        }

        private void AddToRunTimer(Stopwatch sw, string timerVar)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
                try
                {
                    if (iamSerial == 1)
                    {
                        var tmp = Convert.ToDouble(Properties.Settings.Default[timerVar]);
                        tmp += sw.Elapsed.TotalMilliseconds;
                        Properties.Settings.Default[timerVar] = tmp;
                    }
                }
                catch (Exception err)
                { Logger.Error(err, "AddToRunTimer failed {0} ", timerVar); }
                sw.Reset();
            }
        }

        /***** process position change *****/
        private void ProcessGrblPositionUpdate()
        {
            if (iamSerial == 1)
            {
                if (!Grbl.posChanged)
                    Grbl.posChanged = posChangedMonitor = !(XyzPoint.AlmostEqual(Grbl.posWCO, posWCO) && XyzPoint.AlmostEqual(Grbl.posMachine, posMachine));
                if (!Grbl.wcoChanged)
                    Grbl.wcoChanged = !(XyzPoint.AlmostEqual(Grbl.posWCO, posWCO));
                Grbl.posWCO = posWCO; Grbl.posWork = posWork; Grbl.posMachine = posMachine;
            }
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));

            // set local variables
            gcodeVariable["MACX"] = posMachine.X; gcodeVariable["MACY"] = posMachine.Y; gcodeVariable["MACZ"] = posMachine.Z;
            gcodeVariable["WACX"] = posWork.X; gcodeVariable["WACY"] = posWork.Y; gcodeVariable["WACZ"] = posWork.Z;

            mParserState.changed = false;
        }

        /***** process IDLE state *****/
        private void ProcessGrblStateChange()
        {
            if ((grblStateNow != grblStateLast) || (countPreventIdle > 0))
            {
                if ((grblStateNow == GrblState.idle) || (grblStateNow == GrblState.check))
                {
                    if (externalProbe)
                    {
                        posProbe = posMachine;
                        externalProbe = false;
                        OnRaisePosEvent(new PosEventArgs(posWork, posMachine, GrblState.probe, machineState, mParserState, "($PROBE)"));
                        mParserState.changed = false;
                    }
                    if (isStreaming) StreamingIDLE(); // streaming reached idle
                    if (countPreventIdle <= 1) waitForIdle = false;
                    countPreventInterlock = 50;
                }
            }
            grblStateLast = grblStateNow;
        }

        /***** processGrblWelcomeMessage  RESET *****/
        private void ProcessGrblWelcomeMessage(string rxStringTmp)
        {
            if (isStreaming)
            {
                Logger.Info("ProcessGrblWelcomeMessage - SendStreamEvent");
                SendStreamEvent(GrblStreaming.reset);
            }
            ProcessWelcomeMessage();

            AddToLog("* RESET\r\n< " + rxStringTmp);
            IsGrblVers0 = false;    // default, if no welcome message received

            if (rxStringTmp.ToLower().IndexOf("grbl 0") >= 0)
            { IsGrblVers0 = true; IsLasermode = false; }
            if (rxStringTmp.ToLower().IndexOf("grbl 1") >= 0)
            { IsGrblVers0 = false; AddToLog("* Version 1.x"); }

            if (iamSerial == 1)
            {
                Grbl.axisCount = 0;
                Grbl.isVersion_0 = IsGrblVers0;
            }

            GrblVers = rxStringTmp.Substring(0, rxStringTmp.IndexOf('['));

            Logger.Info("grbl reset: '{0}'  isGrblVers0:{1}  isMarlin:{2}", rxStringTmp, IsGrblVers0, isMarlin);

            string tmp = "Rst " + GrblVers;
            EventCollector.SetStreaming(tmp);       // RstGrbl 1.1f ['$' for help] COM9

            if (logStreamData) AddToLog("* Logging enabled");
            AddToLog("* Read grbl settings, hide response from '$$', '$#'");
            //            ReadSettings();
            if (Grbl.isVersion_0)
                RequestSend("$10=3");   // grbl v 0.9 get WPos and MPos
            else
                RequestSend("$10=2");   // grbl v 1.1 Enable WPos: Disable MPos: Enabled Buf:
            ReadSettings();
            return;
        }
        private void ProcessWelcomeMessage()
        {
            countMissingStatusReport = 10;     // reset error counter
            waitForIdle = false;
            waitForOk = false;
            externalProbe = false;
            countStateReset = 8;    // reset message received
            rxErrorCount = 0;
            rtsrResponse = 0;       // real time status report sent / receive differnence                    
            isHoming = false;
            ResetStreaming();       // handleRX_Reset
            IsGrblVers0 = true;
            resetProcessed = false;
            lblSrBf.Text = "";
            lblSrFS.Text = "";
            lblSrPn.Text = "";
            lblSrLn.Text = "";
            lblSrOv.Text = "";
            lblSrA.Text = "";
            lastError = "";
            timerSerial.Enabled = true;
        }

        public void ReadSettings()
        {
            countPreventOutput = 10; countPreventEvent = 10;
            RequestSend("$$");  // get setup
            RequestSend("$#");  // get parameter
            RequestSend("$I");  // get infos
        }

        /****************************************************************
         * processGrblFeedbackMessage
         * Feed back values in [ ] G54-G59,G28,G92,TLO,PRB,GC 
         ****************************************************************/
        private void ProcessGrblFeedbackMessage(string[] dataField)  // dataField = rxString.Trim(charsToTrim).Split(':')
        {
            if (dataField.Length <= 1)
            { return; }
            if (iamSerial == 1 && dataField.Length > 1)
            {
                string info = "";
                if (dataField.Length > 2)
                    info = dataField[2];
                // store gcode parameters https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#---view-gcode-parameters
                // store coordinates of PRB,G54,G55,G56,G57,G58,G59,G28,G30,G92,TLO
                Grbl.SetCoordinates(dataField[0], dataField[1], info);
            }
            if (dataField[0].IndexOf("GC") >= 0)            // handle G-Code parser state [GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]
            {
                parserStateGC = dataField[1];
                Grbl.UpdateParserState(iamSerial, dataField[1], ref mParserState);
                if (IsGrblVers0)
                    parserStateGC = parserStateGC.Replace("M0 ", "");
                getParserState = false;
                //		if (iamSerial == 1)
                OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                mParserState.changed = false;
            }
            else if (dataField[0].IndexOf("PRB") >= 0)                // Probe message with coordinates // [PRB:-155.000,-160.000,-28.208:1]
            {
                if (countPreventEvent == 0)
                    grblStateNow = GrblState.probe;
                posProbeOld = posProbe;
                Grbl.GetPosition(iamSerial, "PRB:" + dataField[1], ref posProbe);  // get numbers from string
                gcodeVariable["PRBX"] = posProbe.X; gcodeVariable["PRBY"] = posProbe.Y; gcodeVariable["PRBZ"] = posProbe.Z;
                gcodeVariable["PRDX"] = posProbe.X - posProbeOld.X; gcodeVariable["PRDY"] = posProbe.Y - posProbeOld.Y; gcodeVariable["PRDZ"] = posProbe.Z - posProbeOld.Z;

                if (countPreventEvent == 0)   // (iamSerial == 1)
                {
                    OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                    mParserState.changed = false;
                }
            }

            else if (dataField[0].IndexOf("MSG") >= 0) //[MSG:Pgm End]
            {
                Grbl.GetOtherFeedbackMessage(dataField);
            }
            // [VER:], [AXS:] and [OPT:]: Indicates build info data from a $I user query. 
            // https://github.com/fra589/grbl-Mega-5X/wiki/grbl-Mega-5X-v1.2-interface#feedback-messages
            else if (dataField[0].IndexOf("VER") >= 0)
            {
                Grbl.GetOtherFeedbackMessage(dataField);
                Logger.Info("Feedback> {0} ", rxString);
            }
            else if (dataField[0].IndexOf("OPT") >= 0)
            {
                Grbl.GetOtherFeedbackMessage(dataField);
                Logger.Info("Feedback> {0} ", rxString);
            }
            else if (dataField[0].IndexOf("AXS") >= 0)
            {
                Grbl.GetOtherFeedbackMessage(dataField);
                Logger.Info("Feedback> {0} ", rxString);
            }
            else if (dataField[0].IndexOf("echo") >= 0)
            {
                if (logStreamData)
                {
                    string echo = dataField[1];
                    if (dataField.Length <= 1)
                    { echo = rxString; }
                    System.IO.File.AppendAllText(Datapath.LogFiles + "\\" + logFileEcho, echo.Trim() + "\r\n");
                }
            }
        }

        /***** process [ALARM... *****/
        private void ProcessGrblAlarmMessage(string rxStringTmp)
        {
            waitForOk = false;
            waitForIdle = false;
            lastError = "";
            lastMessage = string.Format("grbl ALARM '{0}' {1}", rxStringTmp, Grbl.GetAlarmDescription(rxStringTmp));
            Logger.Warn("⚠ Ser:{0}  {1}", iamSerial, lastMessage);
            AddToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            AddToLog(string.Format("< {0} \t{1}", rxStringTmp, Grbl.GetAlarmDescription(rxStringTmp)));
            lastError = rxStringTmp + " " + Grbl.GetAlarmDescription(rxStringTmp) + "\r\n";
            ResetStreaming();   // ALARM
            IsHeightProbing = false;
            grblStateNow = GrblState.alarm;
            if (iamSerial == 1)
            {
                Grbl.lastMessage = lastMessage;
            }
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxStringTmp));// lastCmd));

            mParserState.changed = false;
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /***** process [ERROR... *****/
        private void ProcessGrblErrorMessage(string rxStringTmp)
        {
            waitForOk = false;
            waitForIdle = false;
            string errNr = Grbl.GetMsgNr(rxStringTmp);
            //   string errTxt = grbl.getErrorDescription(rxStringTmp);
            lastMessage = string.Format("grbl ERROR '{0}' {1}", rxStringTmp, Grbl.GetErrorDescription(rxStringTmp));
            lastError = rxStringTmp + " " + Grbl.GetErrorDescription(rxStringTmp) + "\r\n";

            Logger.Warn("⚠ Ser:{0}  {1}", iamSerial, lastMessage);
            AddToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            AddToLog(string.Format("< {0} \t{1}", rxStringTmp, Grbl.GetErrorDescription(rxStringTmp)));
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;

            streamingStateNow = GrblStreaming.error;
            if (isStreaming)
            {
                Logger.Warn("⚠ Last RX '{0}'  BufferFree:{1,3}  Index:{2,3}  max:{3,3}  lineNr:{4}", sendBuffer.GetConfirmedLine(), grblBufferFree, sendBuffer.IndexConfirmed, sendBuffer.Count, (sendBuffer.GetConfirmedLineNr() + 1));
                AddToLog(string.Format("! Last processed '{0}'  line-Nr:{1}", sendBuffer.GetConfirmedLine(), (sendBuffer.GetConfirmedLineNr() + 1)));
                AddToLog(string.Format("Try to continue err-count:{0}", ++countGrblError));
                SendStreamEvent(streamingStateNow);         // processGrblErrorMessage
            }

            lock (receiveDataLock)  // error appears instead of ok, keep counting characters
            {
                grblBufferFree += sendBuffer.LengthConfirmed() + 1;
                if (!grblCharacterCounting)
                    grblBufferFree = Grbl.RX_BUFFER_SIZE;
                sendBuffer.LineWasReceived();
            }

            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxStringTmp));// lastCmd));

            if (errNr == "33")
            { RealtimeCommand((byte)'!'); }
            return;
        }

        /***** process $$ *****/
        private void ProcessGrblUserQuery(string rxStringTmp)
        {
            timerSerial.Interval = Grbl.pollInterval;
            countMissingStatusReport = (int)(10000 / timerSerial.Interval);

            string[] splt = rxStringTmp.Split('=');     // $123=456
            if (splt[0].Length < 2)
                return;
            if (iamSerial != 1)
                return;

            if (int.TryParse(splt[0].Substring(1), out int id))
            {
                if (!IsGrblVers0)
                {
                    string msgNr = splt[0].Substring(1).Trim();
                    if (countPreventOutput == 0)
                        AddToLog(string.Format("< {0} ({1})", rxStringTmp.PadRight(14, ' '), Grbl.GetSettingDescription(msgNr)));   // output $$ response
                    if (id == 32)
                    {
                        if (splt[1].IndexOf("1") >= 0)
                            IsLasermode = true;
                        else
                            IsLasermode = false;
                        OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, 0, GrblStreaming.lasermode));
                    }
                }
                else
                    AddToLog(string.Format("< {0}", rxStringTmp));

                GRBLSettings.Add(rxStringTmp);
                Grbl.SetSettings(id, splt[1]);
                OnRaiseStreamEvent(new StreamEventArgs(id, 0, 0, 0, GrblStreaming.setting));
            }
            else
                AddToLog(string.Format("!< {0}", rxStringTmp));
        }

        #endregion

        #region send

        /***** Send real time command immediately *****/
        public void RealtimeCommand(byte cmd)
        {
            var dataArray = new byte[] { Convert.ToByte(cmd) };
            if (dataArray.Length > 0)// && !blockSend)
                SerialPortDataSend(dataArray, 0, 1);
            AddToLog("> '0x" + cmd.ToString("X") + "' " + Grbl.GetRealtimeDescription(cmd));
            if ((cmd == 0x85) && !(isStreaming && !isStreamingPause))                   //  Jog Cancel
            {
                sendBuffer.Clear();
                grblBufferFree = grblBufferSize;
            }
        }

        /**********************************************************************************
         * requestSend fill up send buffer, called by main-prog for single commands or by preProcessStreaming
         * or called by preProcessStreaming to stream GCode data
         * requestSend -> processSend -> sendLine
         **********************************************************************************/
        public bool RequestSend(string data)
        { return RequestSend(data, false); }
        public bool RequestSend(string data, bool keepComments)
        { return RequestSend(data, 0, keepComments); }
        public bool RequestSend(string data, int lineNr, bool keepComments)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if ((isStreamingRequestPause) && (grblStateNow == GrblState.run))
                {
                    AddToLog("!!! Command blocked - wait for IDLE " + data);
                    Logger.Info("requestSend waitForIdle:{0}", waitForIdle);
                    Logger.Info("requestSend infoStream:{0}", ListInfoStream());
                    Logger.Info("requestSend infoSend:  {0}", ListInfoSend());
                }
                else
                {
                    var tmp = CleanUpCodeLine(data, keepComments);
                    if ((!string.IsNullOrEmpty(tmp)) && (!tmp.StartsWith(";")))  //(tmp[0] != ';'))    // trim lines and remove all empty lines and comment lines
                    {
                        if (tmp == "$#") { countPreventEvent = 5; }                 // no response echo for parser state
                        if (tmp == "$H") { isHoming = true; AddToLog("Homing"); Logger.Info("requestSend Start Homing"); }

                        if (tmp == "$X")
                        {
                            SerialPortDataSend(tmp + lineEndTXgrbl);
                            return IsConnectedToGrbl();
                        }
                        lock (sendDataLock)
                        { sendBuffer.Add(tmp, lineNr); }

                        ProcessSend();          // RequestSend
                        FeedBackSettings(tmp);
                    }
                }
            }
            return IsConnectedToGrbl();
        }

        /*******************************************************
         * cleanUpCodeLine
         * remove unneccessary chars but keep keywords
         * called from requestSend() 
         *******************************************************/
        private string CleanUpCodeLine(string data, bool keepComments = false)
        {
            if (data == null)
                return "";

            var line = data.Replace("\r", "").Replace("\n", "");  //remove CR LF
                                                                  //            line = line.Replace("\n", "");      //remove LF
            if (!keepComments)
            {
                var orig = line;
                int start = orig.IndexOf('(');
                int end = orig.LastIndexOf(')');
                if (start >= 0) line = orig.Substring(0, start);    // get line without comment
                if (end > start) line += orig.Substring(end + 1);
                line = line.ToUpper();                              //all uppercase

                // extract GCode for 2nd / 3rd COM Port
                if ((start >= 0) && (end > start))  // send data to 2nd COM-Port
                {
                    var cmt = orig.Substring(start, end - start + 1);
                    if ((cmt.IndexOf("(^2") >= 0) || (cmt.IndexOf("(^3") >= 0) || (cmt.IndexOf("($") == 0))
                    {
                        line += cmt;                // keep 2nd / 3rd COM port data for further use
                    }
                    if (cmt.Contains("(SKIP M30)"))
                    { skipM30 = true; AddToLog("Skip M30"); }
                }
            }
            else
                line = line.ToUpper();              //all uppercase

            line = line.TrimEnd(';', ' ');
            line = line.Trim();
            return line;
        }

        /*******************************************************
         * feedBackSettings
         * called from requestSend() to notify main GUI
         *******************************************************/
        private void FeedBackSettings(string tmp)
        {
            if (!isStreaming || isStreamingPause)
            {
                if (string.IsNullOrEmpty(tmp))
                    return;
                if (iamSerial != 1)
                    return;
                if (tmp.IndexOf("$") >= 0)
                {
                    string[] splt = tmp.Split('=');     // $123=456
                    if (splt.Length < 2)
                        return;
                    if (splt[0].Length < 2)
                        return;
                    btnCheckGRBLResult.Enabled = false; btnCheckGRBLResult.BackColor = SystemColors.Control;
                    if (int.TryParse(splt[0].Substring(1), out int id))
                    {
                        if (id == 32)
                        {
                            if (splt[1].IndexOf("1") >= 0)
                                IsLasermode = true;
                            else
                                IsLasermode = false;
                            OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, 0, GrblStreaming.lasermode));
                        }
                        Grbl.SetSettings(id, splt[1]);
                        OnRaiseStreamEvent(new StreamEventArgs(id, 0, 0, 0, GrblStreaming.setting));
                    }
                }
            }
        }

        /*********************************************************************
         * processSend 
         * send data if GRBL-buffer is ready to take new data
         * called by timer
         * take care of keywords
         *********************************************************************/
        // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#eeprom-issues
        private readonly string[] eeprom1 = { "G54", "G55", "G56", "G57", "G58", "G59" };
        private readonly string[] eeprom2 = { "G10", "G28", "G30" };
        public void ProcessSend()
        {
            // Logger.Trace("processSend grblBufferFree:{0}  waitForIdle:{1}", grblBufferFree, waitForIdle);
            // OkToSend = grblBufferFree >= buffer[sent].Length + 1
            while (sendBuffer.OkToSend(grblBufferFree) && (!waitForIdle))		//((sent < buffer.Count) && (bufferSize >= (buffer[sent].Length + 1)));}
            {
                var line = sendBuffer.GetSentLine();

                bool replaced = false;
                #region replace
                if (!isStreaming)       // check tool change coordinates
                {
                    if (line.Contains("#T"))
                    { }
                    else if (line.Contains("$T"))
                    { }
                    else if (line.Contains('T'))
                    {
                        int cmdTNr = Gcode.GetCodeNrFromGCode('T', line);
                        if (cmdTNr >= 0)
                        {
                            ToolTable.Init(" (ProcessSend)");       // fill structure
                            SetToolChangeCoordinates(cmdTNr, line);
                            // save actual tool info as last tool info
                            gcodeVariable["TOLN"] = gcodeVariable["TOAN"];
                            gcodeVariable["TOLX"] = gcodeVariable["TOAX"];
                            gcodeVariable["TOLY"] = gcodeVariable["TOAY"];
                            gcodeVariable["TOLZ"] = gcodeVariable["TOAZ"];
                            gcodeVariable["TOLA"] = gcodeVariable["TOAA"];
                        }
                    }
                }
                if (line.Contains('#'))                      // check if variable neededs to be replaced
                {
                    line = InsertVariable(line);
                    replaced = true;
                }

                #endregion
                if ((!waitForOk) || (grblStateNow == GrblState.alarm))    // (!waitForIdle) ||
                {
                    if (replaced)
                        sendBuffer.SetSentLine(line);
                    if (IsConnectedToGrbl() || Grbl.grblSimulate)
                    {

                        // Delay "IDLE" to give the controler a chance to switch to "RUN"
                        // as long as delayed, do nothing
                        if ((countPreventIdle > 0) || (countPreventIdle2nd > 0))
                        { break; }
                        // If connected and command is for 2nd grbl						
                        if (useSerial2 && (iamSerial == 1) && line.Contains("(^2"))// && (grblStateNow == grblState.idle))
                        {
                            serial2Busy = true;                 // avoid further copy from streamingBuffer to sendBuffer
                            if (grblStateNow != GrblState.idle)	// only send if 1st grbl is IDLE
                                break;
                            if (grblBufferFree < (Grbl.RX_BUFFER_SIZE - 1))	//  added 2021-04-07 
                                break;

                            lock (sendDataLock)
                            {
                                SendTo2ndGrbl(line);			// send to 2nd grbl
                                sendBuffer.LineWasSent();		// mark as sent
                                sendBuffer.LineWasReceived();   // mark as received, because will not get 'ok'
                            }
                            countPreventIdle = (int)(1000 / timerSerial.Interval); // wait 1 sec, give 2nd grbl a chance to change from IDLE to run
                            serial2Busy = false;
                        }
                        // If connected and command is for 3rd serial com 					
                        if (useSerial3 && (iamSerial == 1) && line.Contains("(^3"))// && (grblStateNow == grblState.idle))
                        {
                            serial3Busy = true;                 // avoid further copy from streamingBuffer to sendBuffer
                            if (grblStateNow != GrblState.idle)	// only send if 1st grbl is IDLE
                                break;
                            Logger.Trace("process 3rd BufferFree:{0}  BufferSize:{1}  BufferState:{2}  lineNr:{3}  line:{4}", grblBufferFree, Grbl.RX_BUFFER_SIZE, machineState.Bf, sendBuffer.LineNr, line);
                            if (grblBufferFree < (Grbl.RX_BUFFER_SIZE - 1)) //  added 2021-04-07 
                            {
                                if (useSerial3LastBufferFree != grblBufferFree) // any progress? - yes:
                                {
                                    useSerial3LastBufferFree = grblBufferFree;  // new reference
                                    useSerial3LastBufferFreeSame = 0;           // reset counter 2022-02-14
                                }
                                else
                                { useSerial3LastBufferFreeSame++; }         // no progress

                                if (useSerial3LastBufferFreeSame > 10)          // assume miscounting
                                {
                                    useSerial3LastBufferFreeSame = 0;
                                    Logger.Warn("use 3rd: no progress in increasing BufferFree ({0}) -> set BufferFree to BufferSize ({1}) to force continuation", grblBufferFree, Grbl.RX_BUFFER_SIZE);
                                    grblBufferFree = Grbl.RX_BUFFER_SIZE;   // reset buffer - must be empty
                                }
                                else
                                    break;
                            }
                            lock (sendDataLock)
                            {
                                SendTo3rdCOM(line);			    // send to 3rd serial
                                sendBuffer.LineWasSent();		// mark as sent
                                sendBuffer.LineWasReceived();   // mark as received, because will not get 'ok'
                            }
                            countPreventIdle = (int)(1000 / timerSerial.Interval); // wait 1 sec, give 3rd com a chance to change from IDLE to busy
                            serial3Busy = false;
                        }
                        // If one grbl, or 2 grbl and command is for 1st						
                        else if (ExternalCOMReady() && (countPreventIdle <= 0))
                        {
                            lock (sendDataLock)
                            {
                                line = sendBuffer.GetSentLine();    // needed?
                                int sendLength = (line.Length + 1);
                                if (line.StartsWith("$"))
                                {
                                    if (grblStateNow != GrblState.idle)	// only send if 1st grbl is IDLE
                                        break;
                                }
                                if (IsConnectedToGrbl() && (grblBufferFree >= sendLength) && (line != "OV") && (!waitForOk))// && !blockSend)
                                {
                                    if (SerialPortDataSend(line + lineEndTXgrbl))         // grbl accepts '\n' or '\r'			
                                    {
                                        grblBufferFree -= sendLength;
                                        if (!grblCharacterCounting)
                                            grblBufferFree = 0;
                                        sendBuffer.LineWasSent();       // RX in 189
                                        if (logTransmit || cBStatus1.Checked || cBStatus.Checked) Logger.Trace("s{0} TX '{1,-20}' length:{2,2}  BufferFree:{3,3}  Index:{4,3}  max:{5,3}  lineNr:{6}", iamSerial, line, sendLength, grblBufferFree, sendBuffer.IndexSent, sendBuffer.Count, sendBuffer.GetSentLineNr());
                                    }
                                }
                                else
                                    break;
                            }

                            if (!IsHeightProbing && (!(isStreaming && !isStreamingPause)))// || (cBStatus1.Checked || cBStatus.Checked))
                            {
                                if (!(cBStatus1.Checked || cBStatus.Checked || (countPreventOutput > 0)))
                                    AddToLog(string.Format("> {0}", line));     //if not in transfer log the txLine
                            }
                            if (cBStatus1.Checked || cBStatus.Checked)
                            { AddToLog(string.Format("TX> {0,-30} {1,2} {2,3}  line-Nr:{3}", line, line.Length, grblBufferFree, (sendBuffer.GetSentLineNr() + 1))); }


                            //https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#eeprom-issues				
                            for (int i = 0; i < eeprom1.Length; i++)           // wait for IDLE beacuse of EEPROM access
                            {
                                if (line.IndexOf(eeprom1[i]) >= 0)          // is eeprom command?
                                {  // waitForOk = true;
                                   //  countPreventWaitForOkLock = 5;
                                    if (logTransmit) Logger.Trace("EEPROM1 wait for ok {0}", grblBufferFree);
                                    //    return;
                                    break;
                                }
                            }
                            for (int i = 0; i < eeprom2.Length; i++)        // wait for IDLE beacuse of EEPROM access
                            {
                                if (line.IndexOf(eeprom2[i]) >= 0)          // is eeprom command?
                                {   //waitForOk = true;
                                    //countPreventWaitForOkLock = 5;
                                    if (logTransmit) Logger.Trace("EEPROM2 wait for ok {0}", grblBufferFree);
                                    //  return;
                                    break;
                                }
                            }

                            lastSentToCOM.Enqueue(line);
                            if (lastSentToCOM.Count > 10)
                                lastSentToCOM.Dequeue();            // store last sent commands via COM for error analysis

                            // wait 1 sec, give 1st grbl a chance to change from IDLE
                            //                   if (useSerial2 || useSerial3)
                            //                      countPreventIdle2nd = (int)(1000/timerSerial.Interval); // wait 1 sec
                        }
                        if ((useSerial2 && (_serial_form2.grblStateNow != GrblState.idle))
                            || (useSerial3 && _serial_form3.Busy))
                        { break; }
                    }
                    else
                    {
                        AddToLog(line + " !!! Port is closed !!! ");
                        ResetStreaming();   // ALARM
                        break;
                    }

                    if (line == "($PROBE)")
                    { waitForIdle = true; waitForOk = true; externalProbe = true; grblStateLast = GrblState.unknown; countPreventIdle = 0; }
                }
            }   // while
        }

        private void SendTo2ndGrbl(string line)
        {
            if ((_serial_form2 != null) && (_serial_form2.SerialPortOpen))
            {
                int start = line.IndexOf('(');
                int end = line.IndexOf(')');		// find 1st index to avoid sending real comment - old: line.LastIndexOf(')');
                if ((start >= 0) && (end > start))  // send data to 2nd COM-Port
                {
                    var cmt = line.Substring(start, end - start + 1);

                    string txt = cmt.Substring(start + 3, cmt.Length - 4).Trim();
                    if (log2ndGrbl) Logger.Trace("processSend 2nd '{0}' ", txt);
                    if (logTransmit || cBStatus1.Checked || cBStatus.Checked) Logger.Trace("s{0} TX '{1,-20}'   lineNr:{2}", iamSerial, line, sendBuffer.GetSentLineNr());
                    _serial_form2.RequestSend(txt);
                }
            }
        }
        private void SendTo3rdCOM(string line)
        {
            if ((_serial_form3 != null) && (_serial_form3.SerialPortOpen))
            {
                int start = line.IndexOf('(');
                int end = line.IndexOf(')');		// find 1st index to avoid sending real comment - old: line.LastIndexOf(')');
                if ((start >= 0) && (end > start))  // send data to 2nd COM-Port
                {
                    var cmt = line.Substring(start, end - start + 1);

                    string txt = cmt.Substring(start + 3, cmt.Length - 4).Trim();
                    if (log3rdCOM) Logger.Trace("processSend 3rd '{0}' ", txt);
                    if (logTransmit || cBStatus1.Checked || cBStatus.Checked) Logger.Trace("s{0} TX '{1,-20}'   lineNr:{2}", iamSerial, line, sendBuffer.GetSentLineNr());
                    _serial_form3.SerialPortDataSend(txt);
                }
            }
        }

        /*********************************************************************
         * sendLine - now really send data to Arduino
         * called from processSend()
         *********************************************************************/
        private void SendLine(string data)
        {
            try
            {
                if (isMarlin)
                    SerialPortDataSend(data + lineEndTXmarlin);         // sendLine single command
                else
                    SerialPortDataSend(data + lineEndTXgrbl);         // sendLine single command

                if (!IsHeightProbing && (!(isStreaming && !isStreamingPause)))// || (cBStatus1.Checked || cBStatus.Checked))
                {
                    if (!(cBStatus1.Checked || cBStatus.Checked || (countPreventOutput > 0)))
                        AddToLog(string.Format("> {0}", data));     // if not in transfer log the txLine
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "Ser:{0} -sendLine-", iamSerial);
                if (!Grbl.grblSimulate)
                    LogError("! Sending line", err);
                UpdateControls();
            }
            if (logTransmit) Logger.Trace("Ser:{0} TX '{1}'", iamSerial, data);
        }
        #endregion

        /*********************************************************************
         * insertVariable - replace variable e.g. '#WACX' by real value
         * called from processSend()
         *********************************************************************/
        private string InsertVariable(string line)
        {
            int pos, posold = 0;
            string variable, mykey;
            double myvalue;
            if (line.Length > 5)        // min length needed to be replaceable: x#TOLX
            {
                do
                {
                    pos = line.IndexOf('#', posold);
                    if (pos > 0)
                    {
                        myvalue = 0;
                        if ((pos + 5) <= (line.Length))
                        { variable = line.Substring(pos, 5); }
                        else
                        {
                            Logger.Error("InsertVariable pos#:{0} line-len:{1} line:'{2}'", pos, line.Length, line);
                            AddToLog("!!! Error replacing variable: " + line);
                            continue;
                        }
                        mykey = variable.Substring(1);
                        if (gcodeVariable.ContainsKey(mykey))
                        { myvalue = gcodeVariable[mykey]; }
                        else if (GuiVariables.variable.ContainsKey(mykey))
                        { myvalue = GuiVariables.variable[mykey]; }
                        else { line += " (" + mykey + " not found)"; }
                        if (cBStatus1.Checked || cBStatus.Checked)
                        { AddToLog("< replace " + mykey + " = " + myvalue.ToString()); }

                        line = line.Replace(variable, string.Format("{0:0.000}", myvalue));
                        Logger.Trace("insertVariable line:{0} var:{1}  value:{2}", line, mykey, myvalue);
                    }
                    posold = pos + 5;
                } while (pos > 0);
            }
            return line.Replace(',', '.');
        }

        public void UpdateGrblBufferSettings()
        {
            if (!Properties.Settings.Default.grblBufferAutomatic)
                Grbl.RX_BUFFER_SIZE = (int)Properties.Settings.Default.grblBufferSize;
            grblBufferSize = Grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
            grblBufferFree = Grbl.RX_BUFFER_SIZE;
            timerSerial.Interval = Grbl.pollInterval;
            countMissingStatusReport = (int)(10000 / timerSerial.Interval);
        }

        private void ResetVariables(bool resetToolCoord = false)
        {
            Logger.Trace("resetVariables  keep last tool:{0}", resetToolCoord);
            double toln = 0, tolx = 0, toly = 0, tolz = 0, tola = 0;
            if (!resetToolCoord)    // try to keep last tool informations
            {
                if (gcodeVariable.ContainsKey("TOLN")) { toln = gcodeVariable["TOLN"]; }
                if (gcodeVariable.ContainsKey("TOLX")) { tolx = gcodeVariable["TOLX"]; }
                if (gcodeVariable.ContainsKey("TOLY")) { toly = gcodeVariable["TOLY"]; }
                if (gcodeVariable.ContainsKey("TOLZ")) { tolz = gcodeVariable["TOLZ"]; }
                if (gcodeVariable.ContainsKey("TOLY")) { tola = gcodeVariable["TOLA"]; }
            }

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
            gcodeVariable.Add("TOAN", 0.0); // Tool Actual Number
            gcodeVariable.Add("TOAX", 0.0); // Tool change position
            gcodeVariable.Add("TOAY", 0.0);
            gcodeVariable.Add("TOAZ", 0.0);
            gcodeVariable.Add("TOAA", 0.0);

            gcodeVariable.Add("TOLN", toln); // Tool Last Number
            gcodeVariable.Add("TOLX", tolx); // Tool change position
            gcodeVariable.Add("TOLY", toly);
            gcodeVariable.Add("TOLZ", tolz);
            gcodeVariable.Add("TOLA", tola);
        }
        private void SaveLastPos()
        {
            if (iamSerial == 1)
            {
                AddToLog("\r[Save last pos.: " + posWork.Print(false, (Grbl.axisCount > 3)) + "]");    // print in single lines
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


        /********************************************************************* 
         * Class CodeBuffer:
         * keep Code, track index for sent and received lines
         *********************************************************************/
        private class CodeBuffer
        {
            private List<string> buffer = new List<string>();
            private List<int> lineNr = new List<int>();
            private int sent = 0;              // actual sent line
            private int confirmed = 0;         // already received line
            private int max = 0;

            public CodeBuffer()
            {
                buffer = new List<string>();
                lineNr = new List<int>();
                sent = 0;
                confirmed = 0;
                max = 0;
            }
            public List<string> Buffer
            {
                get { return buffer; }
                set { buffer = value; }
            }
            public List<int> LineNr
            {
                get { return lineNr; }
                set { lineNr = value; }
            }
            public int IndexSent
            {
                get { return sent; }
                set { sent = value; }
            }
            public int IndexConfirmed
            {
                get { return confirmed; }
                set { confirmed = value; }
            }
            public int Count
            {
                get { return buffer.Count; }
            }
            public int Capacity
            {
                get { return buffer.Capacity; }
            }
            public int MaxLineNr
            {
                get { return max; }
            }
            public void Clear()
            {
                sent = 0;
                confirmed = 0;
                max = 0;
                buffer.Clear();
                lineNr.Clear();
            }
            public void Add(string txt)
            {
                buffer.Add(txt);
                lineNr.Add(0);
            }
            public void Add(string txt, int nr)
            {
                buffer.Add(txt);
                lineNr.Add(nr);
                max = Math.Max(max, nr);
            }
            public bool DeleteLine()	// ReleaseMemory
            {
                if ((buffer.Count > 2) && (confirmed == sent) && (sent > 1)) //(sent > 1) && (confirmed > 1))		//(confirmed == sent) &&(sent > 1))
                {
                    buffer.RemoveAt(0);
                    lineNr.RemoveAt(0);
                    confirmed--;
                    sent--;
                    return true;
                }
                return false;
            }
            public bool IsBufferEmpty()
            { return (confirmed == buffer.Count); }

            public int LengthSent()
            {
                if (sent < buffer.Count)
                    return buffer[sent].Length;
                else
                    return 0;
            }
            public int LengthConfirmed()
            {
                if (confirmed < buffer.Count)
                    return buffer[confirmed].Length;
                else
                    return 0;
            }
            public string GetSentLine()
            {
                if (sent < buffer.Count)
                    return buffer[sent];
                else if (buffer.Count > 0)
                    return "OV";// buffer[buffer.Count-1];
                else
                    return "UR";
            }
            public int GetSentLineNr()
            {
                if (sent < lineNr.Count)
                    return lineNr[sent];
                else if (lineNr.Count > 0)
                    return lineNr[lineNr.Count - 1];
                else
                    return 0;
            }
            public string GetConfirmedLine(int off = 0)
            {
                if ((confirmed + off) < 0)
                    off -= (confirmed + off);
                if ((confirmed + off) < buffer.Count)
                    return buffer[confirmed + off];
                else if (buffer.Count > 0)
                    return buffer[buffer.Count - 1];
                else
                    return "";
            }
            public int GetConfirmedLineNr()
            {
                if (confirmed < lineNr.Count)
                    return lineNr[confirmed];
                else if (lineNr.Count > 0)
                    return lineNr[lineNr.Count - 1];
                else
                    return 0;
            }
            public void SetSentLine(string txt)
            {
                if (sent < buffer.Count)
                    buffer[sent] = txt;
            }
            public void LineWasSent()
            { sent++; }
            public void LineWasReceived()
            { confirmed++; }
            public bool OkToSend(int bufferSize)
            {
                if ((buffer == null) || (sent >= buffer.Count) || (sent < 0))
                    return false;
                int blen = buffer[sent].Length + 1;
                return (bufferSize >= blen);
            }
            public int MissingConfirmationLength()
            {
                if ((buffer == null) || (confirmed + 1 >= buffer.Count) || (confirmed < 0))
                    return 0;
                int blen = 0;
                for (int i = confirmed + 1; i < buffer.Count; i++)
                { blen += buffer[i].Length + 1; }    // length + \r
                return blen;
            }

            public bool Insert(int index, string cmt)
            {
                if ((index >= 0) && (index < buffer.Count))
                {
                    buffer.Insert(index, cmt);
                    lineNr.Insert(index, 0);
                    return true;
                }
                return false;
            }
            public bool Insert(int index, string cmt, int nr)
            {
                if ((index >= 0) && (index < buffer.Count))
                {
                    buffer.Insert(index, cmt);
                    lineNr.Insert(index, nr);
                    return true;
                }
                return false;
            }

        }
    }
}
