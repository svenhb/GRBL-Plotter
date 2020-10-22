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
/* 2020-09-18 split file

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
using System.Runtime.Remoting.Messaging;

namespace GRBL_Plotter
{
    public partial class ControlSerialForm : Form        // Form can be loaded twice!!! COM1, COM2
    {
        private xyzPoint posWCO, posWork, posMachine;
        public xyzPoint posPause, posProbe, posProbeOld;
        private mState machineState = new mState();     // Keep info about Bf, Ln, FS, Pn, Ov, A from grbl status
        private pState mParserState = new pState();     // keep info about last M and G settings from GCode
        private bool resetProcessed = false;
        private grblState grblStateNow = grblState.unknown;
        private grblState grblStateLast = grblState.unknown;
		private string lastMessage = "";
		
        private static gcodeByLine oldLine = new gcodeByLine();    // actual parsed line
        private static gcodeByLine newLine = new gcodeByLine();    // last parsed line
        private static modalGroup modal = new modalGroup();        // keep modal states and helper variables

        public bool isGrblVers0 { get; private set; } = true;
        public string grblVers { get; private set; } = "";
        public bool isLasermode { get; private set; } = false;
        public bool toolInSpindle { get; set; } = false;
        public bool isHeightProbing { get; set; } = false;      // automatic height probing -> less feedback
        public List<string> GRBLSettings = new List<string>();          // keep $$ settings
        private Queue<string> lastSentToCOM = new Queue<string>();      // store last sent commands via COM
        private int rtsrResponse = 0;     // real time status report sent / receive differnence - should be zero.                    

        private Dictionary<string, double> gcodeVariable = new Dictionary<string, double>();    // keep variables "PRBX" etc.
        public string parserStateGC = "";                  // keep parser state response [GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]
		
        private int grblBufferSize = grbl.RX_BUFFER_SIZE;               //rx bufer size of grbl on arduino 127
        private int grblBufferFree = grbl.RX_BUFFER_SIZE;               //actual suposed free bytes on grbl buffer
		private bool grblCharacterCounting = true;                      //if false, then Simple Send-Response https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#streaming-a-g-code-program-to-grbl
                                                                        // https://github.com/Smoothieware/Smoothieware/issues/1369
        private CodeBuffer sendBuffer = new CodeBuffer();

        private bool waitForIdle = false;
        private bool waitForOk = false;
        private int waitForOkVal = 0;
        private bool externalProbe = false;
        private bool isHoming = false;

		static readonly object sendDataLock = new object();
		static readonly object receiveDataLock = new object();

        public event EventHandler<PosEventArgs> RaisePosEvent;
        protected virtual void OnRaisePosEvent(PosEventArgs e)
        {   RaisePosEvent?.Invoke(this, e);  }

        public int getFreeBuffer()
        { return ((int)(100 * grblBufferFree / (float)grblBufferSize)); }

        private string listInfoSend()
        { return string.Format("{0}sendBuffer snt:{1,3}  cnfrmnd:{2,3}  cnt:{3,3}  BFree:{4,3}  lineNr:{5}  code:'{6}' state:{7}", "", sendBuffer.IndexSent, sendBuffer.IndexConfirmed, sendBuffer.Count, grblBufferFree, sendBuffer.GetConfirmedLineNr(), sendBuffer.GetConfirmedLine(), grblStateNow.ToString()); }

    #region processGRBL
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
		private void processGrblMessages(object sender, EventArgs e)	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#message-summary
        {
            if (rxString == "")    { isDataProcessing = false; return; }     // unlock serialPort1_DataReceived
			if (countShutdown > 0) { isDataProcessing = false; return; }  
			
            int tmp;
            char[] charsToTrim = { '<', '>', '[', ']', ' ' };
            bool isStatusReport = (((tmp = rxString.IndexOf('<')) >= 0) && (rxString.IndexOf('>') > tmp));
			bool  isOk = rxString.StartsWith("ok");

            if (isStatusReport)
            { 	if (logReceiveStatus) Logger.Trace("Ser:{0} RX '{1}'", iamSerial, rxString); }
            else
            { 	if (isOk)   // sendBuffer.GetConfirmedLine = index = sinnlos wg. FeedbackMessage
                {	} //if (logReceive) Logger.Trace("Ser:{0} RX '{1}'  sent:'{2}'  line:{3}   BufferFree:{4}", iamSerial, rxString, sendBuffer.GetConfirmedLine(), streamingBuffer.GetConfirmedLineNr(), grblBufferFree); }
				else
				{	if (logReceive) Logger.Trace("Ser:{0} RX '{1}'", iamSerial, rxString); }
			}
		
/***** action by occurance / importance *****/
/***** grbl buffer processed *****/
            if (isOk)
            {   processGrblOkMessage();
                processOkStreaming();                          // process all other messages
                rxString = "";                              // clear if simulation is on
                if (!isStreaming || isStreamingPause)
                {   if (!(isHeightProbing || (cBStatus1.Checked || cBStatus.Checked || (countPreventOutput > 0))))
                        addToLog("< ok");   // string.Format("< {0}", rxString)); // < ok
                }
            }

/***** Process status message with coordinates *****/
            else if (isStatusReport)
            {   processGrblRealTimeStatus(rxString.Trim(charsToTrim));	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#real-time-status-reports
				processGrblPositionUpdate();
                processGrblStateChange();
                rtsrResponse--;                             			// real time status report received                    			
                if (cBStatus.Checked) addToLog(rxString);
            }

/***** reset message *****/
            else if (rxString.ToLower().IndexOf("['$' for help]") >= 0)
            {   processGrblWelcomeMessage(rxString); }	                // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#welcome-message

/***** Process feedback message with coordinates  *****/
            else if (((tmp = rxString.IndexOf('[')) >= 0) && (rxString.IndexOf(']') > tmp))
            {   processGrblFeedbackMessage(rxString.Trim(charsToTrim).Split(':'));	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#feedback-messages
                if (!isHeightProbing || cBStatus.Checked)
                {  if (countPreventOutput == 0)
                        addToLog(rxString);
                }
            }

            else if (rxString.ToUpper().IndexOf("ALARM") >= 0)
            {	processGrblAlarmMessage(rxString);	}	// https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#alarm-message
			
            else if (rxString.ToUpper().IndexOf("ERROR") >= 0)
            {	processGrblErrorMessage(rxString);	}   // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#grbl-response-messages

/***** Show GRBL Settings Info if Version is >= 1.0  *****/
            else if ((rxString.IndexOf("$") >= 0) && (rxString.IndexOf("=") >= 0))
            {   processGrblUserQuery(rxString);   }

            isDataProcessing = false;                   // unlock serialPort1_DataReceived
        }

/***** processGrblOkMessage 'Ok' *****/
        private void processGrblOkMessage()
        {
            int receivedByteCount = sendBuffer.LengthConfirmed() + 1;   // + "\r"
            grbl.updateParserState(iamSerial, sendBuffer.GetConfirmedLine(), ref mParserState);

            /***** increase free buffer if ok was received *****/
            lock (receiveDataLock)
            {
                receivedByteCount = sendBuffer.LengthConfirmed() + 1;   // + "\r"
                grblBufferFree += receivedByteCount;   	                // update bytes supose to be free on grbl rx bufer
                if (!grblCharacterCounting)
                    grblBufferFree = grbl.RX_BUFFER_SIZE;

                if (logReceive || cBStatus1.Checked || cBStatus.Checked) Logger.Trace("s{0} RX '{1,-20}' length:{2}  BufferFree:{3}  Index:{4}  max:{5}", iamSerial, sendBuffer.GetConfirmedLine(), receivedByteCount, grblBufferFree, sendBuffer.IndexConfirmed, sendBuffer.Count);
                sendBuffer.LineWasReceived();           // inc counter

                if (grblBufferFree >= grbl.RX_BUFFER_SIZE)
                {   waitForOk = false;  // matching ok was received
                    if (logReceive) Logger.Trace("waitForOk = false", grblBufferFree);
                }

                if (grblBufferFree > grbl.RX_BUFFER_SIZE)
                {   grblBufferFree = grbl.RX_BUFFER_SIZE;
                    waitForOk = false;
                    if (sendBuffer.Count > 0)
                    {   Logger.Error("### grblBufferFree too big! {0} rx:'{1}' in processGrblOkMessage() - fix | last RX:'{2}' RX-1:'{3}' RX-2:'{4}'", grblBufferFree, rxString, sendBuffer.GetConfirmedLine(), sendBuffer.GetConfirmedLine(-1), sendBuffer.GetConfirmedLine(-2));
                        Logger.Info("{0}", listInfoSend());
                        Logger.Info("{0}", listInfoStream());
                    }
                }
            }   // lock
			string rxLine = sendBuffer.GetConfirmedLine();

            if (rxLine.Contains("($TS"))  { addToLog("[Tool change start]"); }
            else if (rxLine.Contains("($TO"))    { addToLog("[Tool " + rxLine.Substring(4).Trim(')') + " removed]"); toolInSpindle = false; }
            else if (rxLine.Contains("($TI"))     { addToLog("[Tool " + rxLine.Substring(4).Trim(')') + " selected]"); toolInSpindle = true; }
            else if (rxLine == "($TE)")    { addToLog("[Tool change finished]"); }

            if (cBStatus1.Checked || cBStatus.Checked)
            {   addToLog(string.Format("RX< {0,-30} {1,2} {2,3}  line:{3}", sendBuffer.GetConfirmedLine(), receivedByteCount, grblBufferFree, streamingBuffer.GetConfirmedLineNr()));  }

            if ((mParserState.changed) && (grblStateNow != grblState.probe))    // probe will be send later
            {	OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                mParserState.changed = false;
            }
            if (isHoming)
            {   isHoming = false; rtsrResponse = 0; countMissingStatusReport = (int)(10000 / timerSerial.Interval); }

            if (!waitForOk)
            {   if (logReceive) Logger.Trace("in RX start processSend");
                if (isStreaming && !isStreamingRequestPause && !isStreamingPause)
                {   Application.DoEvents();
                    preProcessStreaming();
                }
            }
        }

/**********************************************************************************************
 * processGrblRealTimeStatus
 * should occur with same frequent as timer interrupt -> each 200ms
 * old:         <Idle,MPos:0.000,0.000,0.000,WPos:0.000,0.000,0.000>
 * new in 1.1   < Idle | MPos:0.000,0.000,0.000 | FS:0,0 | WCO:0.000,0.000,0.000 >
 **********************************************************************************************/
        private void processGrblRealTimeStatus(string text)    // '<' and '>' already removed
        {
            char splitAt = '|';
            if (isGrblVers0)
                splitAt = ',';
            string[] dataField = text.Split(splitAt);
            string status = dataField[0].Trim(' ');
            if (isGrblVers0)	//	handle old format from grbl vers. 0.9
            {
                if (dataField.Length > 3)
                    grbl.getPosition(iamSerial, dataField[1] + "," + dataField[2] + "," + dataField[3]+" ", ref posMachine);
                if (dataField.Length > 6)
                    grbl.getPosition(iamSerial, dataField[4] + "," + dataField[5] + "," + dataField[6]+" ", ref posWork);
                posWCO = posMachine - posWork;
            }
            else
            {
                if (dataField.Length > 2)
                {
                    for (int i = 2; i < dataField.Length; i++)
                    {
                        if (dataField[i].IndexOf("WCO") >= 0)           // Work Coordinate Offset
                        {   grbl.getPosition(iamSerial, dataField[i], ref posWCO);
                            continue;
                        }
                        string[] data = dataField[i].Split(':');
                        if (data.Length > 1)
                        {
                            if (dataField[i].IndexOf("Bf:") >= 0)            // Buffer state - needs to be enabled in config.h file
                            {
                                machineState.Bf = lblSrBf.Text = data[1];
                                if (grbl.getBufferSize(iamSerial, data[1])) requestSend("$10=" + ((grbl.getSetting(10) >= 0) ? grbl.getSetting(10).ToString() : "0"));
                                continue;
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
                                { machineState.A = lblSrA.Text = dataField[dataField.Length - 1].Split(':')[1]; }
                                else
                                { machineState.A = lblSrA.Text = ""; }
                                continue;
                            }
                        }
                    }
                }
                if (dataField[1].IndexOf("MPos") >= 0)
                {   axisCount = grbl.getPosition(iamSerial, dataField[1], ref posMachine);
                    posWork = posMachine - posWCO;
                }
                else
                {   axisCount = grbl.getPosition(iamSerial, dataField[1], ref posWork);
                    posMachine = posWork + posWCO;
                }
            }
            grblStateNow = grbl.parseStatus(status);            // get actual state
            lblSrState.BackColor = grbl.grblStateColor(grblStateNow);
            lblSrState.Text = grbl.statusToText(grblStateNow);  // status;

            lblSrPos.Text = posWork.Print(false, grbl.axisB || grbl.axisC); // show actual work position
        }

/***** process position change *****/
        private void processGrblPositionUpdate()
        {
            if (iamSerial == 1)
            {
                if (!grbl.posChanged)
                    grbl.posChanged = !(xyzPoint.AlmostEqual(grbl.posWCO, posWCO) && xyzPoint.AlmostEqual(grbl.posMachine, posMachine));
                if (!grbl.wcoChanged)
                    grbl.wcoChanged = !(xyzPoint.AlmostEqual(grbl.posWCO, posWCO));
                grbl.posWCO = posWCO; grbl.posWork = posWork; grbl.posMachine = posMachine;
            }
			OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));

/*            if (isStreaming)
            {
                string[] tmp = machineState.FS.Split(',');
                string s = "";
                if (tmp.Length > 1)
                    s = "S"+tmp[1];
                LogPos.Trace("G1 X{0} Y{1} Z{2} {3} (FS:{4})", posWork.X, posWork.Y, posWork.Z, s, machineState.FS);
            }*/
            // set local variables
            gcodeVariable["MACX"] = posMachine.X; gcodeVariable["MACY"] = posMachine.Y; gcodeVariable["MACZ"] = posMachine.Z;
            gcodeVariable["WACX"] = posWork.X; gcodeVariable["WACY"] = posWork.Y; gcodeVariable["WACZ"] = posWork.Z;

            mParserState.changed = false;
        }

/***** process IDLE state *****/
        private void processGrblStateChange()
        {
            if ((grblStateNow != grblStateLast) || (countPreventIdle > 0))
            {
                if ((grblStateNow == grblState.idle) || (grblStateNow == grblState.check))
                {   
					if (externalProbe)
                    {   posProbe = posMachine;
                        externalProbe = false;
						OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblState.probe, machineState, mParserState, "($PROBE)"));
                        mParserState.changed = false;
                    }
                    if (isStreaming) streamingIDLE(); // streaming reached idle
                    if (countPreventIdle <= 1) waitForIdle = false;
                    countPreventInterlock = 50;
                }				
            }
            grblStateLast = grblStateNow;
        }
	
/***** processGrblWelcomeMessage  RESET *****/
		private void processGrblWelcomeMessage(string rxString)
        {
            countMissingStatusReport = 100;     // reset error counter
            waitForIdle = false;
            waitForOk = false;
            externalProbe = false;
            countStateReset = 8;         // reset message received
            rxErrorCount = 0;
            rtsrResponse = 0;       // real time status report sent / receive differnence                    
            isHoming = false;
            resetStreaming();       // handleRX_Reset
            addToLog("* RESET\r\n< " + rxString);
            if (rxString.ToLower().IndexOf("grbl 0") >= 0)
            { isGrblVers0 = true; isLasermode = false; }
            if (rxString.ToLower().IndexOf("grbl 1") >= 0)
            { isGrblVers0 = false; addToLog("* Version 1.x"); }

            if (iamSerial == 1)
            {	grbl.axisCount = 0;
				grbl.isVersion_0 = isGrblVers0;
			}
//            sendResetEvent();
            resetProcessed = false;

            grblVers = rxString.Substring(0, rxString.IndexOf('['));

            lblSrBf.Text = "";
            lblSrFS.Text = "";
            lblSrPn.Text = "";
            lblSrLn.Text = "";
            lblSrOv.Text = "";
            lblSrA.Text = "";
            timerSerial.Enabled = true;
            lastError = "";
			addToLog("* Read grbl settings, hide response from '$$', '$#'");
			readSettings();
			requestSend("$10=2"); //if (grbl.getSetting(10) != 2) { requestSend("$10=2"); } // to get buffer size
            return;
        }
		public void readSettings()
        {   countPreventOutput = 10; countPreventEvent = 10;
            requestSend("$$");  // get setup
            requestSend("$#");  // get parameter
        }

/****************************************************************
 * processGrblFeedbackMessage
 * Feed back values in [ ] G54-G59,G28,G92,TLO,PRB,GC 
 ****************************************************************/
        private void processGrblFeedbackMessage(string[] dataField)  // dataField = rxString.Trim(charsToTrim).Split(':')
        {
            if (dataField.Length <= 1)
            {   return;  }
            if (iamSerial == 1 && dataField.Length > 1)
            {   string info = "";
                if (dataField.Length > 2)
                    info = dataField[2];
                grbl.setCoordinates(dataField[0], dataField[1], info);    // store gcode parameters https://github.com/gnea/grbl/wiki/Grbl-v1.1-Commands#---view-gcode-parameters
            }
            if (dataField[0].IndexOf("GC") >= 0)            // handle G-Code parser state [GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]
            {
                parserStateGC = dataField[1];
                grbl.updateParserState(iamSerial, dataField[1], ref mParserState);
                if (isGrblVers0)
                    parserStateGC = parserStateGC.Replace("M0 ", "");
                getParserState = false;
		//		if (iamSerial == 1)
					OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                mParserState.changed = false;
            }
            else if (dataField[0].IndexOf("PRB") >= 0)                // Probe message with coordinates // [PRB:-155.000,-160.000,-28.208:1]
            {   if (countPreventEvent==0)
                    grblStateNow = grblState.probe;
                posProbeOld = posProbe;
                grbl.getPosition(iamSerial,"PRB:" + dataField[1], ref posProbe);  // get numbers from string
                gcodeVariable["PRBX"] = posProbe.X; gcodeVariable["PRBY"] = posProbe.Y; gcodeVariable["PRBZ"] = posProbe.Z;
                gcodeVariable["PRDX"] = posProbe.X - posProbeOld.X; gcodeVariable["PRDY"] = posProbe.Y - posProbeOld.Y; gcodeVariable["PRDZ"] = posProbe.Z - posProbeOld.Z;
				
                if (countPreventEvent == 0)   // (iamSerial == 1)
                {   OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));
                    mParserState.changed = false;
                }
            }

            else if (dataField[0].IndexOf("MSG") >= 0) //[MSG:Pgm End]
            {   if (dataField[1].IndexOf("Pgm End") >= 0)
                {   if ((isStreaming) || (isHeightProbing))
                    {
                        streamingFinish();
                    }
                }
            }
        }
		
/***** process [ALARM... *****/		
		private void processGrblAlarmMessage(string rxString)
		{
            waitForOk = false;
            waitForIdle = false;
            lastError = "";
			lastMessage = string.Format("grbl ALARM '{0}' {1}", rxString, grbl.getAlarmDescription(rxString));
			Logger.Warn("Ser:{0}  {1}",iamSerial, lastMessage);
			addToLog("\r\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			addToLog(string.Format("< {0} \t{1}", rxString, grbl.getAlarmDescription(rxString)));
            lastError = rxString + " " + grbl.getAlarmDescription(rxString) + "\r\n";
            resetStreaming();   // ALARM
			isHeightProbing = false;
			grblStateNow = grblState.alarm;
            if (iamSerial == 1)
            {
                grbl.lastMessage = lastMessage;
            }
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));

            mParserState.changed = false;
			this.WindowState = FormWindowState.Minimized;
			this.Show();
			this.WindowState = FormWindowState.Normal;			
		}
		
/***** process [ERROR... *****/		
		private void processGrblErrorMessage(string rxString)
		{
            waitForOk = false;
            waitForIdle = false;
            string tmpMsg = "";
			if (rxString != lastError)
			{
				lastMessage = string.Format("grbl ERROR '{0}' {1}", rxString, grbl.getErrorDescription(rxString));
				Logger.Warn("Ser:{0}  {1}",iamSerial, lastMessage);
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
			streamingStateNow = grblStreaming.error;
			if (isStreaming)
			{   tmpMsg = string.Format("! Error before code line {0} \r\n", streamingBuffer.GetSentLineNr());//  gCodeLineNr[streamingBuffer.Sent]);
				addToLog(tmpMsg);
				lastError += tmpMsg;
				sendStreamEvent(streamingStateNow);   // error processGrblErrorMessage
                stopStreaming();
			}
			resetStreaming();   // ERROR
			isHeightProbing = false;
			isDataProcessing = false;                   // unlock serialPort1_DataReceived
            if (iamSerial == 1)
            { grbl.lastMessage = lastMessage;  }
		
            OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));

			return;			
		}
		
/***** process $$ *****/		
		private void processGrblUserQuery(string rxString)
        {
            timerSerial.Interval = grbl.pollInterval;
            countMissingStatusReport = (int)(10000 / timerSerial.Interval);

            string[] splt = rxString.Split('=');
            int id=0;
            if (int.TryParse(splt[0].Substring(1), out id))
            {
                if (!isGrblVers0)
                {   string msgNr = splt[0].Substring(1).Trim();
                    if (countPreventOutput == 0)
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

#endregion

#region send

/***** Send real time command immediately *****/		
        public void realtimeCommand(byte cmd)
        {
            var dataArray = new byte[] { Convert.ToByte(cmd) };
            if (serialPort.IsOpen)// && !blockSend)
                serialPort.Write(dataArray, 0, 1);
            addToLog("> '0x" + cmd.ToString("X") + "' " + grbl.getRealtimeDescription(cmd));
            if ((cmd == 0x85) && !(isStreaming && !isStreamingPause))                   //  Jog Cancel
            {   sendBuffer.Clear();
                grblBufferFree = grblBufferSize;
            }
        }

/**********************************************************************************
 * requestSend fill up send buffer, called by main-prog for single commands or by preProcessStreaming
 * or called by preProcessStreaming to stream GCode data
 * requestSend -> processSend -> sendLine
 **********************************************************************************/
        public bool requestSend(string data, bool keepComments=false)
        {
            if ((isStreamingRequestPause) && (grblStateNow == grblState.run))
            {   addToLog("!!! Command blocked - wait for IDLE " + data);
                Logger.Info("requestSend waitForIdle:{0}", waitForIdle);
                Logger.Info("requestSend infoStream:{0}", listInfoStream());
                Logger.Info("requestSend infoSend:  {0}", listInfoSend());
            }
            else
            {   var tmp = cleanUpCodeLine(data, keepComments);
                if ((!string.IsNullOrEmpty(tmp)) && (tmp[0] != ';'))    // trim lines and remove all empty lines and comment lines
                {   if (tmp == "$#") countPreventEvent = 5;                  // no response echo for parser state
                    if (tmp == "$H") { isHoming = true; addToLog("Homing"); Logger.Info("requestSend Start Homing"); }

                    lock (sendDataLock)
                        sendBuffer.Add(tmp);
                    processSend();
                    feedBackSettings(tmp);
                }
            }
            return serialPort.IsOpen;
        }

/*******************************************************
 * cleanUpCodeLine
 * remove unneccessary chars but keep keywords
 * called from requestSend() 
 *******************************************************/
        private string cleanUpCodeLine(string data, bool keepComments = false)
        {
            var line = data.Replace("\r", "").Replace("\n", "");  //remove CR LF
//            line = line.Replace("\n", "");      //remove LF
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
            line = line.TrimEnd(';', ' ');
            line = line.ToUpper();              //all uppercase
            line = line.Trim();
            return line;
        }

/*******************************************************
 * feedBackSettings
 * called from requestSend() to notify main GUI
 *******************************************************/ 
        private void feedBackSettings(string tmp)
        {
            if (!isStreaming || isStreamingPause)
            {   if (tmp == "")
                    return;
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

/*********************************************************************
 * processSend 
 * send data if GRBL-buffer is ready to take new data
 * called by timer
 * take care of keywords
 *********************************************************************/
        // https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#eeprom-issues
        private string[] eeprom1 = { "G54", "G55", "G56", "G57", "G58", "G59" };
        private string[] eeprom2 = { "G10", "G28", "G30" };
        public void processSend()
        {
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
                        int cmdTNr = gcode.getIntGCode('T', line);
                        if (cmdTNr >= 0)
                        {
                            toolTable.init();       // fill structure
                            setToolChangeCoordinates(cmdTNr, line);
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
                {   line = insertVariable(line);
                    replaced = true;
                }
				
                #endregion
                if ((!waitForOk) || (grblStateNow == grblState.alarm))    // (!waitForIdle) ||
                {   if (replaced)
                        sendBuffer.SetSentLine(line);
                    if (serialPort.IsOpen || grbl.grblSimulate)
                    {
                        
// Delay "IDLE" to give the controler a chance to switch to "RUN"
// as long as delayed, do nothing
                        if ((countPreventIdle > 0) || (countPreventIdle2nd > 0))
                        {   break;     }
// If connected and command is for 2nd grbl						
                        if (useSerial2 && (iamSerial == 1) && line.Contains("(^2"))// && (grblStateNow == grblState.idle))
                        {
                            if (grblStateNow != grblState.idle)	// only send if 1st grbl is IDLE
                                break;
                            sendTo2ndGrbl(line);			// send to 2nd grbl
                            sendBuffer.LineWasSent();		// mark as sent
                            sendBuffer.LineWasReceived();   // mark as received, because will not get 'ok'
					// wait 1 sec, give 2nd grbl a chance to change from IDLE
                            countPreventIdle = (int)(1000 / timerSerial.Interval); 
                        }
// If one grbl, or 2 grbl and command is for 1st						
                        else if ((!useSerial2) || (useSerial2 && (_serial_form2.grblStateNow == grblState.idle) && (countPreventIdle <= 0)))
                        {							
							lock (sendDataLock)
							{
                                line = sendBuffer.GetSentLine();
                                int len = (line.Length + 1);
                                if (serialPort.IsOpen && (grblBufferFree >= len) && (line != "OV") && (!waitForOk))// && !blockSend)
								{	serialPort.Write(line + "\r");							
									grblBufferFree -= len;
									if (!grblCharacterCounting)
										grblBufferFree = 0;
									sendBuffer.LineWasSent();
                                    if (logTransmit || cBStatus1.Checked || cBStatus.Checked) Logger.Trace("s{0} TX '{1,-20}' length:{2}  BufferFree:{3}  Index:{4}  max:{5}", iamSerial, line, len, grblBufferFree, sendBuffer.IndexSent, sendBuffer.Count);
                                }
                                else
                                    break;
                            }
							
							if (!isHeightProbing && (!(isStreaming && !isStreamingPause)))// || (cBStatus1.Checked || cBStatus.Checked))
							{   if (!(cBStatus1.Checked || cBStatus.Checked || (countPreventOutput > 0))                   )
									addToLog(string.Format("> {0}", line));     //if not in transfer log the txLine
							}
                            if (cBStatus1.Checked || cBStatus.Checked)
                            {   addToLog(string.Format("TX> {0,-30} {1,2} {2,3}  line:{3}", line, line.Length, grblBufferFree, streamingBuffer.GetSentLineNr()));  }


                            //https://github.com/gnea/grbl/wiki/Grbl-v1.1-Interface#eeprom-issues				
                            for (int i = 0; i < eeprom1.Length; i++)           // wait for IDLE beacuse of EEPROM access
                            {   if (line.IndexOf(eeprom1[i]) >= 0)          // is eeprom command?
                                {  // waitForOk = true;
                                   // waitForOkVal = grblBufferFree;
                                  //  countPreventWaitForOkLock = 5;
                                    if (logTransmit)  Logger.Trace("EEPROM1 wait for ok {0}", grblBufferFree);
                                //    return;
                                    break;
                                }
                            }
                            for (int i = 0; i < eeprom2.Length; i++)        // wait for IDLE beacuse of EEPROM access
                            {   if (line.IndexOf(eeprom2[i]) >= 0)          // is eeprom command?
                                {   waitForOk = true;
                                    waitForOkVal = grblBufferFree;
                                    countPreventWaitForOkLock = 5;
                                    if (logTransmit) Logger.Trace("EEPROM2 wait for ok {0}", grblBufferFree);
                                  //  return;
                                    break;
                                }
                            }

                            lastSentToCOM.Enqueue(line);
                            if (lastSentToCOM.Count > 10)
                                lastSentToCOM.Dequeue();            // store last sent commands via COM for error analysis

					// wait 1 sec, give 1st grbl a chance to change from IDLE
                            if (useSerial2)
                                countPreventIdle2nd = (int)(1000/timerSerial.Interval); // wait 1 sec
                        }
                        if (useSerial2 && (_serial_form2.grblStateNow != grblState.idle))
                        { break; }
                    }
                    else
                    {   addToLog("!!! Port is closed !!!");
                        resetStreaming();   // ALARM
                        break;
                    }

                    if (line == "($PROBE)")
                    { waitForIdle = true; waitForOk = true; externalProbe = true; grblStateLast = grblState.unknown; countPreventIdle = 5; }
                }
            }   // while
        }

        private void sendTo2ndGrbl(string line)
        {
            int start = line.IndexOf('(');
            int end = line.LastIndexOf(')');
            if ((start >= 0) && (end > start))  // send data to 2nd COM-Port
            {
                var cmt = line.Substring(start, end - start + 1);

                string txt = cmt.Substring(start + 3, cmt.Length - 4);
                if (log2ndGrbl) Logger.Trace("processSend 2nd '{0}' ", txt);
                _serial_form2.requestSend(txt);
            }
        }

/*********************************************************************
 * sendLine - now really send data to Arduino
 * called from processSend()
 *********************************************************************/
        private void sendLine(string data)
        {
            try
            {   if (serialPort.IsOpen)// && !blockSend)
                    serialPort.Write(data + "\r");
                if (!isHeightProbing && (!(isStreaming && !isStreamingPause)))// || (cBStatus1.Checked || cBStatus.Checked))
                {   if (!(cBStatus1.Checked || cBStatus.Checked || (countPreventOutput > 0))                   )
                        addToLog(string.Format("> {0}", data));     //if not in transfer log the txLine
                }
            }
            catch (Exception err)
            {   Logger.Error(err, "Ser:{0} -sendLine-",iamSerial);
                logMessage = "Error reading line from serial port";
                if (!grbl.grblSimulate)
                    logError("! Sending line", err);
                updateControls();
            }
            if (logTransmit) Logger.Trace("Ser:{0} TX '{1}'", iamSerial, data);
        }
#endregion

/*********************************************************************
 * insertVariable - replace variable e.g. '#WACX' by real value
 * called from processSend()
 *********************************************************************/
        private string insertVariable(string line)
        {
            int pos = 0, posold = 0;
            string variable, mykey = "";
            double myvalue = 0;
            if (line.Length > 5)        // min length needed to be replaceable: x#TOLX
            {
                do
                {
                    pos = line.IndexOf('#', posold);
                    if (pos > 0)
                    {
                        myvalue = 0;
                        variable = line.Substring(pos, 5);
                        mykey = variable.Substring(1);
                        if (gcodeVariable.ContainsKey(mykey))
                        { myvalue = gcodeVariable[mykey]; }
                        else if (gui.variable.ContainsKey(mykey))
                        { myvalue = gui.variable[mykey]; }
                        else { line += " (" + mykey + " not found)"; }
                        if (cBStatus1.Checked || cBStatus.Checked)
                        { addToLog("< replace " + mykey + " = " + myvalue.ToString()); }

                        line = line.Replace(variable, string.Format("{0:0.000}", myvalue));
                    }
                    posold = pos + 5;
                } while (pos > 0);
            }
            return line.Replace(',', '.');
        }

        public void updateGrblBufferSettings()
        {
            if (!Properties.Settings.Default.grblBufferAutomatic)
                grbl.RX_BUFFER_SIZE = (int)Properties.Settings.Default.grblBufferSize;
            grblBufferSize = grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
            grblBufferFree = grbl.RX_BUFFER_SIZE;
            timerSerial.Interval = grbl.pollInterval;
            countMissingStatusReport = (int)(10000 / timerSerial.Interval);
        }

        private void resetVariables(bool resetToolCoord=false)
        {
            Logger.Trace("resetVariables");
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
        private void saveLastPos()
        {
            if (iamSerial == 1)
            {
                addToLog("[Save last pos.: "+posWork.Print(false,(grbl.axisCount>3))+"]");    // print in single lines
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

            public CodeBuffer()
            {   buffer = new List<string>();
                lineNr = new List<int>();
                confirmed = 0;
            }
            public List<string> Buffer
            {   get { return buffer; }
                set { buffer = value; }
            }
            public List<int> LineNr
            {   get { return lineNr; }
                set { lineNr = value; }
            }
            public int IndexSent
            {   get { return sent; }
                set { sent = value; }
            }
            public int IndexConfirmed
            {   get { return confirmed; }
                set { confirmed = value; }
            }
			public int Count
			{	get { return buffer.Count();}
			}
            public void Clear()
            {   buffer.Clear();
                lineNr.Clear();
                sent = 0;
                confirmed = 0;
            }
            public void Add(string txt)
            {   buffer.Add(txt);
                lineNr.Add(0);
            }
            public void Add(string txt, int nr)
            {   buffer.Add(txt);
                lineNr.Add(nr);
            }
            public bool DeleteLine()	// ReleaseMemory
            {   if ((buffer.Count > 2) && (confirmed == sent) && (sent > 1)) //(sent > 1) && (confirmed > 1))		//(confirmed == sent) &&(sent > 1))
                {   buffer.RemoveAt(0);
                    lineNr.RemoveAt(0);
                    confirmed--;
                    sent--;
                    return true;
                }
                return false;
            }
            public bool isBufferEmpty()
            { return (confirmed == buffer.Count); }

            public int LengthSent()
            {   if (sent < buffer.Count)
                    return buffer[sent].Length;
                else
                    return 0;
            }
            public int LengthConfirmed()
            {   if (confirmed < buffer.Count)
                    return buffer[confirmed].Length;
                else
                    return 0;
            }
            public string GetSentLine()
            {   if (sent < buffer.Count)
                    return buffer[sent];
                else if (buffer.Count > 0)
                    return "OV";// buffer[buffer.Count-1];
				else
					return "UR";
            }
            public int GetSentLineNr()
            {   if (sent < lineNr.Count)
                    return lineNr[sent];
                else if (lineNr.Count > 0)
                    return lineNr[lineNr.Count-1];
				else
					return 0;
            }
            public string GetConfirmedLine(int off=0)
            {   if ((confirmed + off) < 0)
                    off -= (confirmed + off);
                if ((confirmed + off) < buffer.Count)
                    return buffer[confirmed + off];
                else if (buffer.Count > 0)
                    return buffer[buffer.Count-1];
				else
					return "";
            }
            public int GetConfirmedLineNr()
            {   if (confirmed < lineNr.Count)
                    return lineNr[confirmed];
                else if (lineNr.Count > 0)
                    return lineNr[lineNr.Count-1];
				else 
					return 0;
            }
            public void SetSentLine(string txt)
            {   if (sent < buffer.Count)
                    buffer[sent] = txt;
            }
			public void LineWasSent()
			{sent++;}
  			public void LineWasReceived()
			{confirmed++;}
			public bool OkToSend(int bufferSize)
			{   if ((buffer == null) || (sent >= buffer.Count) || (sent < 0))
                    return false;
                int blen = buffer[sent].Length + 1;
                return (bufferSize >= blen);
            }   

            public bool Insert(int index, string cmt)
            {   if ((index >= 0) && (index < buffer.Count))
                {   buffer.Insert(index, cmt);
                    lineNr.Insert(index, 0);
                    return true;
                }
                return false;
            }
            public bool Insert(int index, string cmt, int nr)
            {   if ((index >= 0) && (index < buffer.Count))
                {   buffer.Insert(index, cmt);
                    lineNr.Insert(index, nr);
                    return true;
                }
                return false;
            }

        }
    }
}
