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
/* 2020-09-18 split file
 * 2020-12-18 fix OnRaiseStreamEvent %
 * 2021-01-23 ### synchronize SendStreamEvent via trgEvent in TimerSerial_Tick to status query poll frequency
 * 2021-01-26 line 270 make delay between probe exchange scripts variable
 * 2021-02-19 insert variables in subroutine already in startStreaming()
 * 2021-02-28 preProcessStreaming lock copy-routine and replace "requestSend()" by sendBuffer.Add
 * 2021-07-12 code clean up / code quality
 */

// OnRaiseStreamEvent(new StreamEventArgs((int)lineNr, codeFinish, buffFinish, status));
// OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

//#pragma warning disable CA1303
//#pragma warning disable CA1305
//#pragma warning disable CA1307

namespace GrblPlotter
{
    public partial class ControlSerialForm : Form        // Form can be loaded twice!!! COM1, COM2
    {
        /************************************************************************************************************
         * Streaming data to grbl with the possibility to interrupt (pause) and sending other code (manual tool change)
         * 1. startStreaming() copy and filter gcode to streamingBuffer
         * 2. preProcessStreaming() to copy data to sendBuffer for sending if grblBuffer (grblBufferFree) has enough space
         ************************************************************************************************************/
        private bool allowStreamingEvent = true;
        private readonly CodeBuffer streamingBuffer = new CodeBuffer();

        private bool isStreaming = false;        		// true when steaming is in progress
        private bool isStreamingRequestPause = false; 	// true when request pause (wait for idle to switch to pause)
        private bool isStreamingPause = false;    		// true when steaming-pause 
        private bool isStreamingCheck = false;    		// true when steaming is in progress (check)
        private bool isStreamingRequestStopp = false;   // 
        private bool getParserState = false;      		// true to send $G after status switched to idle
        private bool isDataProcessing = false;      		// false when no data processing pending
        private GrblStreaming streamingStateNow = GrblStreaming.ok;
        private GrblStreaming streamingStateOld = GrblStreaming.ok;
  //      private int lineStreamingPause = 0;
        private bool trgEvent = false;
        private bool skipM30 = false;

        private Dictionary<int, List<string>> subroutines = new Dictionary<int, List<string>>();

        public event EventHandler<StreamEventArgs> RaiseStreamEvent;
        protected virtual void OnRaiseStreamEvent(StreamEventArgs e)
        { RaiseStreamEvent?.Invoke(this, e); }

        private string ListInfoStream()
        { return string.Format("{0}strmBuffer snt:{1,3}  cnfrmnd:{2,3}  cnt:{3,3}  BFree:{4,3}  lineNr:{5}  code:'{6}' state:{7}", "", streamingBuffer.IndexSent, streamingBuffer.IndexConfirmed, streamingBuffer.Count, grblBufferFree, streamingBuffer.GetConfirmedLineNr(), streamingBuffer.GetConfirmedLine(), grblStateNow.ToString()); }

/****************************************************************************
*  startStreaming called by main-Prog
*  get complete GCode list and copy to streamingBuffer
*  initialize streaming
*  if startAtLine > 0 start with pause
****************************************************************************/
        public void StartStreaming(IList<string> gCodeList, int startAtLine, int stopAtLine, bool check)
        {
            grblCharacterCounting = Properties.Settings.Default.grblStreamingProtocol1 && !isMarlin;
            Logger.Info("Ser:{0} startStreaming at line:{1} to line:{2} ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼", iamSerial, startAtLine, stopAtLine);
            if (grblCharacterCounting)
                Logger.Info("Streaming Protocol: Character-Counting");
            else
                Logger.Info("Streaming Protocol: Simple Send-Response");

            UpdateLogging();

            grblBufferSize = Grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
            grblBufferFree = Grbl.RX_BUFFER_SIZE;
            Logger.Info("Set Buffer Free:{0}  Size:{1}", grblBufferFree, grblBufferSize);

            if (Properties.Settings.Default.grblPollIntervalReduce)
            { timerSerial.Interval = Grbl.pollInterval * 2; }
            else
            { timerSerial.Interval = Grbl.pollInterval; }
            countMissingStatusReport = (int)(10000 / timerSerial.Interval);
            Logger.Info("Poll frequency:{0}  max. missing reports:{1}", 1000/timerSerial.Interval, countMissingStatusReport);

            skipM30 = false;
            lastError = "";
            countGrblError = 0;
            lastSentToCOM.Clear();
            ToolTable.Init();       // fill structure
            rtbLog.Clear();

            // check if other serial are still alive
            if (useSerial2)
            {   try
                {   if (!_serial_form2.SerialPortOpen)
                    { AddToLog("[2nd serial port is not open]"); useSerial2 = false; }
                }
                catch
                { useSerial2 = false; //throw;
                }
            }
            if (useSerial3)
            {   try
                {   if (!_serial_form3.SerialPortOpen)
                    { AddToLog("[3rd serial port is not open]"); useSerial3 = false; }
                }
                catch
                { useSerial3 = false;// throw;
                }
            }

            if (!check)
            {   AddToLog("[Start streaming - no echo]");
                if (useSerial2) AddToLog("[Use serial 2]");
                if (useSerial3) AddToLog("[Use serial 3]");
            }
            else
                AddToLog("[Start code check]");
            SaveLastPos();
            if (replaceFeedRate)
                AddToLog("!!! Override Feed Rate");
            if (replaceSpindleSpeed)
                AddToLog("!!! Override Spindle Speed");
            isStreamingPause = false;
            isStreamingRequestPause = false;
            isStreamingCheck = check;
            streamingStateNow = GrblStreaming.ok;
       //     lineStreamingPause = -1;                // save line were M0 appeared for main GUI to show notification
            streamingBuffer.Clear();    // = new List<string>();
            ResetStreaming();           // startStreaming
            if (isStreamingCheck)
            {   SendLine("$C");         // startStreaming check
                grblBufferSize = 100;   // reduce size to avoid fake errors
            }
            countLoggerUpdate = (int)(10000 / timerSerial.Interval);

            timerSerial.Stop();

            /***** collect subroutines, without resolving variables *****/
            #region subroutines
            subroutines = new Dictionary<int, List<string>>();
            string tmp;
            string[] gCode = gCodeList.ToArray<string>();

            bool useSubroutine = false;
            for (int i = startAtLine; i < gCode.Length; i++)
    //        for (int i = startAtLine; i <= stopAtLine; i++)
            {
                if (gCode[i].Contains("O"))     // find and store subroutines - nothing else
                {
                    int cmdONr = Gcode.GetCodeNrFromGCode('O', gCode[i]);
                    if (cmdONr <= 0)
                        continue;
                    subroutines.Add(cmdONr, new List<string>());
                    Logger.Trace("Add subroutine O{0}", cmdONr);
                    useSubroutine = true;

                    for (int k = i + 1; k < gCode.Length; k++)
                    {   if (gCode[k].IndexOf("M99") >= 0)
                        { break; }
                        if (gCode[k].IndexOf("M98") >= 0)
                        {
                            double pWord = FindDouble("P", -1, gCode[k]);
                            double lWord = FindDouble("L", 1, gCode[k]);

                            if (subroutines.ContainsKey((int)pWord))
                            {
                                for (int repeat = 0; repeat < lWord; repeat++)
                                {
                                    foreach (string subroutineLine in subroutines[(int)pWord])          // copy subroutine
                                    {
                                        subroutines[cmdONr].Add(subroutineLine);
                                        Logger.Trace(" sub in sub {0}", subroutineLine);
                                    }
                                }
                            }
                            else
                                Logger.Error("Start stresaming Subroutine {0} not found",pWord);
                        }
                        else
                        {
                            subroutines[cmdONr].Add(gCode[k]);
                            Logger.Trace(" {0}", gCode[k]);
                        }
                    }
                }
            }
            /****************************************************************************/
#endregion
            lock (sendDataLock)
            {
                double pWord, lWord;//, oWord;
                bool tmpToolInSpindle = ToolInSpindle;
                int cmdTNr = -1;
                bool foundM30 = false;
                //            for (int i = startAtLine; i < gCode.Length; i++)    // now copy code, replace subroutines
                if (stopAtLine >= gCode.Length)
                    stopAtLine = gCode.Length - 1;
                for (int i = startAtLine; i <= stopAtLine; i++)    // now copy code, replace subroutines
                {
                    tmp = CleanUpCodeLine(gCode[i]);
                    if ((!string.IsNullOrEmpty(tmp)) && (tmp[0] != ';'))//trim lines and remove all empty lines and comment lines
                    {
                        int cmdMNr = Gcode.GetCodeNrFromGCode('M', tmp);
                        int cmdGNr = Gcode.GetCodeNrFromGCode('G', tmp);
                        cmdTNr = Gcode.GetCodeNrFromGCode('T', tmp);

                        /***** Insert Subroutine? ********************************************************/
                        #region subroutine
                        //                    if (tmp.IndexOf("M98") >= 0)    // any subroutines?
                        if (cmdMNr == 98)   // subroutine call
                        {
                            pWord = FindDouble("P", -1, tmp);
                            lWord = FindDouble("L", 1, tmp);

                            if (subroutines.ContainsKey((int)pWord))
                            {
                                for (int repeat = 0; repeat < lWord; repeat++)
                                {
                                    foreach(string subroutineLine in subroutines[(int)pWord])          // copy subroutine
                                    {
                                        if (subroutineLine.Contains('#'))                    // check if variable neededs to be replaced
                                            streamingBuffer.Add(InsertVariable(subroutineLine),i); 
                                        else 
                                            streamingBuffer.Add(subroutineLine, i);             // add gcode line to list to send 
                                    }
                                }
                            }
                            else
                                Logger.Error("Subroutine {0} not found", pWord);


                        }
                        #endregion
                        /***** Subroutine end ********************************************************/
                        else
                        {
                            if (Grbl.unknownG.Contains(cmdGNr))
                            {   streamingBuffer.SetSentLine("(" + tmp + " - unknown)");    // don't pass unkown GCode to GRBL because is unknown
                                tmp = streamingBuffer.GetSentLine();
                                streamingBuffer.LineWasReceived();
                                AddToLog(tmp);
                            }
                            if (cmdTNr >= 0)                                        // T-word is allowed by grbl - no need to filter
                            {   SetToolChangeCoordinates(cmdTNr, tmp);              // update variables e.g. "gcodeVariable[TOAX]" from tool-table
                            }
                            if (cmdMNr == 6)                                        // M06 is not allowed - remove
                            {   if (Properties.Settings.Default.ctrlToolChange)
                                {   InsertToolChangeCode(i, ref tmpToolInSpindle);  // insert external script-code and insert variables 
                                    tmp = "(" + tmp + ")";
                                }
                            }
                            if (cmdMNr == 30)
                            {   if (skipM30)
                                { tmp = "(" + tmp + ")"; }      // hide M30
                            }

                            streamingBuffer.Add(tmp, i);        // add gcode line to list to send

                            if (cmdMNr == 30)                   // stop filling buffer, to avoid sending subroutine code
                            {   foundM30 = true;
                                break;
                            }
                        }
                    }
                }
                if (!foundM30)
                {   //if (!skipM30)
                    streamingBuffer.Add("M30", stopAtLine);    // add end
                }
                streamingBuffer.Add("($END)", stopAtLine);        // add gcode line to list to send
                streamingBuffer.Add("()", stopAtLine);            // add gcode line to list to send
            }   // lock
            timerSerial.Start();

            if (logEnable || useSubroutine)
            {   string startText = string.Format("( {0} )\r\n", GetTimeStampString());
                File.WriteAllText(Datapath.AppDataFolder + "\\logStreamGCode.nc", startText); // clear file
                File.AppendAllLines(Datapath.AppDataFolder + "\\logStreamGCode.nc", streamingBuffer.Buffer);
            }

            if (logEnable)
            {   string startText = string.Format("( {0} )\r\n", GetTimeStampString());
                File.WriteAllText(Datapath.AppDataFolder + "\\logSendBuffer.nc", startText); // clear file
            }
            isStreaming = true;
            UpdateControls();
            if (startAtLine > 0)
            {   PauseStreaming();
                isStreamingPause = true;
            }
            else
            {   waitForIdle = false;
                PreProcessStreaming();  // 411 
            }
        }   // startStreaming
    
        private void InsertToolChangeCode(int line, ref bool inSpindle)
        {
            streamingBuffer.Add("($TS)", line);         // keyword for receiving-buffer (sendBuffer.GetConfirmedLine();) "Tool change start"
            if (inSpindle)
            {   AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptPut, line);
                inSpindle = false;
                streamingBuffer.Add("($TO T"+ gcodeVariable["TOLN"]+")", line);     // keyword for receiving-buffer "Tool removed"
            }
            if (!Properties.Settings.Default.ctrlToolChangeEmpty || (gcodeVariable["TOAN"] != (int)Properties.Settings.Default.ctrlToolChangeEmptyNr))
            {   AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptSelect, line);
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptGet, line);
                inSpindle = true;
                streamingBuffer.Add("($TI T" + gcodeVariable["TOAN"] + ")", line);  // keyword for receiving-buffer "Tool inserted"
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptProbe, line);
            }

            streamingBuffer.Add("($TE)", line);         // keyword for receiving-buffer "Tool change finished"

            if (Properties.Settings.Default.ctrlToolScriptDelay > 0)
                streamingBuffer.Add(string.Format("G4 P{0:0.00}",Properties.Settings.Default.ctrlToolScriptDelay), line);

            // save actual tool info as last tool info
            gcodeVariable["TOLN"] = gcodeVariable["TOAN"];
            gcodeVariable["TOLX"] = gcodeVariable["TOAX"];
            gcodeVariable["TOLY"] = gcodeVariable["TOAY"];
            gcodeVariable["TOLZ"] = gcodeVariable["TOAZ"];
            gcodeVariable["TOLA"] = gcodeVariable["TOAA"];
        } 

		private void AddCodeFromFile(string file, int linenr)
		{
			if (File.Exists(file))
			{
				string fileCmd = File.ReadAllText(file);
				string[] commands = fileCmd.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
				string tmp;
				foreach (string cmd in commands)
				{
					tmp = CleanUpCodeLine(cmd);         // remove comments
                    if (tmp.IndexOf("M98") >= 0)
                    {
                        double pWord = FindDouble("P", -1, tmp);
                        double lWord = FindDouble("L", 1, tmp);

                        if (subroutines.ContainsKey((int)pWord))
                        {
                            for (int repeat = 0; repeat < lWord; repeat++)
                            {
                                foreach (string subroutineLine in subroutines[(int)pWord])          // copy subroutine
                                {
                                    if (subroutineLine.Contains('#'))                    // check if variable neededs to be replaced
                                        streamingBuffer.Add(InsertVariable(subroutineLine), linenr);
                                    else
                                        streamingBuffer.Add(subroutineLine, linenr);             // add gcode line to list to send 
                                }
                            }
                        }
                        else
                            Logger.Error("addCodeFromFile Subroutine {0} not found", pWord);

                    }
                    else
                    {
                        if (tmp.Contains('#'))              // check if variable neededs to be replaced                
                        { tmp = InsertVariable(tmp); }
                        if (tmp.Length > 0)
                        { streamingBuffer.Add(tmp, linenr); }
                    }
				}
			}
		}


    /****** get number from string ******/
		private static double FindDouble(string start, double notfound, string txt)
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
		
/************************************************************************
 * pauseStreaming()
 * Pause or restart streaming after button click
 ************************************************************************/ 
        public void PauseStreaming()
        {
			UpdateLogging();
            if (logStartStop) Logger.Trace("Ser:{0} pauseStreaming()  isStreamingPause {1}   isStreamingRequestPause {2}   {3}", iamSerial, isStreamingPause, isStreamingRequestPause, ListInfoStream());
// start pause			
            if (!isStreamingPause)
            {   isStreamingRequestPause = true;     // wait until buffer is empty before switch to pause
                AddToLog("[Pause streaming - wait for IDLE]");
                AddToLog("[Save Settings]");
                Logger.Info("pauseStreaming RequestPause ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲");
                streamingStateNow = GrblStreaming.waitidle;
                getParserState = true;
            }
// restart streaming			
            else
            {   //if ((posPause.X != posWork.X) || (posPause.Y != posWork.Y) || (posPause.Z != posWork.Z))
                AddToLog("++++++++++++++++++++++++++++++++++++");
                timerSerial.Interval = Grbl.pollInterval;
                if (parserStateGC.Contains("F0"))
                {   parserStateGC = parserStateGC.Replace("F0", "F100");            // Avoid missing feed rate
                    AddToLog(string.Format("[Fix F0: {0}]", parserStateGC));
                }
							
                if (!XyzPoint.AlmostEqual(posPause, posWork))	// restore position
                {
                    if (logStartStop) Logger.Trace("AlmostEqual posPause X{0:0.000} Y{1:0.000}  posWork X{2:0.000} Y{3:0.000}", posPause.X, posPause.Y, posWork.X, posWork.Y);
                    AddToLog("[Restore Position]");
                    RequestSend(string.Format("G90 G0 X{0:0.000} Y{1:0.000}", posPause.X, posPause.Y).Replace(',', '.'));  // restore last position
                    if (logStartStop) Logger.Trace("[Restore] X{0:0.000} Y{1:0.000}  State:{2}", posPause.X, posPause.Y, parserStateGC);
					RequestSend("G4 P0.5");       // wait 1 second // changed from 1 to 0,5 2021-01-26
                    RequestSend(string.Format("G0 Z{0:0.000}", posPause.Z).Replace(',', '.'));                      // restore last position
                }
				
                AddToLog(string.Format("[Start streaming line:{0} - no echo]", streamingBuffer.GetSentLineNr()));
         //       AddToLog("[Restore Settings: "+ parserStateGC+" ]");
                Logger.Info("pauseStreaming start streaming ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼");
                streamingStateNow = GrblStreaming.ok;

				if (parserStateGC.Length > 0)
				{
					AddToLog("[Restore Settings: " + parserStateGC + " ]");     // [GC:G0 G54 G17 G21 G90 G94 M5 M9 T0 F0.0 S0]
                    RequestSend(parserStateGC);          
				}

                waitForIdle = false;
                waitForOk = false;
                isStreamingPause = false;
                isStreamingRequestPause = false;

                PreProcessStreaming();
            }
            UpdateControls();
        }   // pauseStreaming

/******************************************************************
 * stopStreaming()
 * Initiate stop (wait until IDLE)
 ******************************************************************/
        public void StopStreaming(bool isNotStartup)
        {
            if (isStreamingCheck)
            {   SendLine("$C");         // stopStreaming check
                isStreamingCheck = false;
            }

            if (isNotStartup)
                StopStreamingFinal();
            else
            {
                IsHeightProbing = false;
                ResetStreaming(false);      // stopStreaming
                isStreamingRequestStopp = true;         // 20200717
                isStreamingRequestPause = true;     // 20200717
                Logger.Info(" stopStreaming() - wait for IDLE - sent:{0}  received:{1}  ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲", streamingBuffer.IndexSent, streamingBuffer.IndexConfirmed);
            }
        }
		
/******************************************************************
 * stopStreamingFinal()
 * Finally stop 
 ******************************************************************/
        public void StopStreamingFinal()
        {						
           // int line = 0;
           // line = streamingBuffer.GetSentLineNr();
            if (logStartStop) Logger.Trace(" stopStreamingFinal() gCodeLinesSent {0}  gCodeLineNr.Count {1}", streamingBuffer.IndexSent, streamingBuffer.Count);
            SendStreamEvent(GrblStreaming.stop);        // stopStreamingFinal

            IsHeightProbing = false;
			
            ResetStreaming(true);       // stopStreamingFinal()
            if (logStartStop) Logger.Trace(" stopStreamingFinal() - lines in buffer {0}", (streamingBuffer.IndexSent - streamingBuffer.IndexConfirmed));

            if (isStreamingCheck)
            {   SendLine("$C");         // stopStreamingFinal check
                isStreamingCheck = false;
            }
            UpdateControls();
            if (Properties.Settings.Default.grblPollIntervalReduce)
            {   timerSerial.Interval = Grbl.pollInterval;
                countMissingStatusReport = (int)(10000 / timerSerial.Interval);
            }
        }

/***** reset streaming variables *****/
        private void ResetStreaming(bool isStopping = true)
        {
            externalProbe = false;
            isStreaming = false;
            isStreamingRequestPause = false;
            isStreamingPause = false;
			isStreamingRequestStopp = false;
			
			if (isStopping)		// 20200717
            {	streamingBuffer.Clear();
                sendBuffer.Clear();
                grblBufferFree = grblBufferSize;
            }
            Grbl.lastMessage = "";
            Grbl.lastErrorNr = 0;
        }

/********************************************************
 * processOkStreaming()
 * process ok message for streaming
 * called in RX event chain, from processGrblMessages()
 ********************************************************/
        public void ProcessOkStreaming()
        {
            if (isStreaming)    // || isStreamingRequestStopp)
            {
// confirm sent data
                if (!isStreamingPause)
                {   streamingBuffer.LineWasReceived();
    //                streamingBuffer.DeleteLine();		// better don't removeAt(0)...
                    allowStreamingEvent = true;
                }
                else
                    streamingStateNow = GrblStreaming.pause;   // update status
				
// send status via event to main GUI
                if ((streamingStateOld != streamingStateNow) || allowStreamingEvent)
                {
                    if (streamingStateNow != GrblStreaming.pause)
                    { if (trgEvent) SendStreamEvent(streamingStateNow); trgEvent = false; }  // streaming processOkStreaming
                    streamingStateOld = streamingStateNow;      //grblStatus = oldStatus;
                    allowStreamingEvent = false;
                }
            }
        }

/*****  sendStreamEvent update main prog  *****/
        private void SendStreamEvent(GrblStreaming status, int linePause=-1)
        {
			int lineNrSent = streamingBuffer.GetSentLineNr();
            if (linePause > 0)
                lineNrSent = linePause;

            int lineNrConfirmed = streamingBuffer.GetConfirmedLineNr();
            if (status == GrblStreaming.error)
                lineNrConfirmed = sendBuffer.GetConfirmedLineNr() +1;

            // progressbar.value is int
            int codeFinish = 0;
			if (streamingBuffer.Count != 0)
				codeFinish = (int)Math.Ceiling((float)lineNrConfirmed * 100 / streamingBuffer.MaxLineNr) +1;      // to reach 100%
            int buffFinish = 0;
			if (grblBufferSize != 0)
				buffFinish = (int)Math.Ceiling((float)(grblBufferSize - grblBufferFree) * 100 / grblBufferSize) +1; // to reach 100%

            if (codeFinish > 100) { codeFinish = 100; }
            if (buffFinish > 100) { buffFinish = 100; }

//            Logger.Trace("OnRaiseStreamEvent {0} {1} {2} {3} {4}", lineNrSent, lineNrConfirmed, codeFinish, buffFinish, status);
            OnRaiseStreamEvent(new StreamEventArgs(lineNrSent, lineNrConfirmed, codeFinish, buffFinish, status));
        }

/**********************************************************************************  
 *  preProcessStreaming copy line by line (requestSend(line)) to sendBuffer until 
 *  grbl-buffer (grblBufferFree) is filled, M0 or M30 or buffer-end reached.
 *  Insert script-code on tool change.
 *  requestSend -> processSend -> sendLine  -  dec. grblBufferFree
 *  called in startStreaming, pauseStreaming (to restart) and timer
 **********************************************************************************/
        private void PreProcessStreaming()
        {
            int lengthToSend = streamingBuffer.LengthSent() + 1;

            if (waitForIdle || isStreamingRequestPause || (countPreventIdle > 0))
            {   return;    }

            while ((streamingBuffer.IndexSent < streamingBuffer.Count) && (grblBufferFree >= lengthToSend) && !waitForIdle && (streamingStateNow != GrblStreaming.pause) && ExternalCOMReady())
            {
                lock (sendDataLock)
                {
                    string line = streamingBuffer.GetSentLine();
                    if ((line == "OV") || (line == "UR"))
                    {   Logger.Error("preProcessStreaming read:{0} from  streamingBuffer index:{1} count:{2}",line, streamingBuffer.IndexSent, streamingBuffer.Count);
                        break;
                    }
                    streamingStateNow = GrblStreaming.ok;       // default status

                    int cmdMNr = Gcode.GetCodeNrFromGCode('M', line);

                    if ((replaceFeedRate) && (!string.IsNullOrEmpty(Gcode.GetStringValue('F', line))))
                    {
                        string old_value = Gcode.GetStringValue('F', line);
                        replaceFeedRateCmdOld = old_value;
                        line = line.Replace(old_value, replaceFeedRateCmd);
                        streamingBuffer.SetSentLine(line);
                    }

                    if ((replaceSpindleSpeed) && (!string.IsNullOrEmpty(Gcode.GetStringValue('S', line))))
                    {
                        string old_value = Gcode.GetStringValue('S', line);
                        line = line.Replace(old_value, replaceSpindleSpeedCmd);
                        replaceSpindleSpeedCmdOld = old_value;
                        streamingBuffer.SetSentLine(line);
                    }

                    #region M0  // Program pause
                    if ((cmdMNr == 0) && !isStreamingCheck) // M0 request pause
                    {
                        if (!Properties.Settings.Default.guiDisableProgramPause)
                        {
                            if (logStartStop || logReceive) Logger.Trace("[Pause streaming - skip M0 - wait for IDLE]  indx-sent:{0}  line:{1} lineNr:{2} grblBufferFree:{3}", streamingBuffer.IndexSent, streamingBuffer.GetSentLine(), streamingBuffer.GetSentLineNr(), grblBufferFree);
                            AddToLog("[Pause streaming - skip M0 line:" + streamingBuffer.GetSentLineNr() + "]");
                      //      lineStreamingPause = streamingBuffer.GetSentLineNr();

                            if (grblStateNow == GrblState.idle)
                            {
                                RequestSend("G4 P1");   // changed from 2 to 1 2021-01-26
                                grblStateNow = grblStateLast = GrblState.unknown;
                                countPreventInterlock = 10;
                            }

                            isStreamingRequestPause = true;
                            streamingStateNow = GrblStreaming.waitidle;
                            waitForIdle = true;                         // block further sending
                            waitForOk = false;
                            getParserState = true;                      // ask for parser state
                            line = "(M0)";
                        }
                    }
                    #endregion

                    if (isMarlin)
                    {   if (updateMarlinPosition || (--insertMarlinCounter <= 0))
                        {   //requestSend("M114", streamingBuffer.GetSentLineNr(), false);    // insert getPosition commands
                            sendBuffer.Add("M114", streamingBuffer.GetSentLineNr());
                            getMarlinPositionWasSent = true;
                            streamingBuffer.LineWasSent();
                            streamingStateOld = streamingStateNow;
                            lengthToSend = streamingBuffer.LengthSent() + 1;    // update while-variable
                            insertMarlinCounter = insertMarlinCounterReload;
                        }
                        updateMarlinPosition = false;
                    }
         //           requestSend(line, streamingBuffer.GetSentLineNr(), false);   // fill sendBuffer, 
                    sendBuffer.Add(line, streamingBuffer.GetSentLineNr());
                    if (logEnable) System.IO.File.AppendAllText(Datapath.AppDataFolder + "\\logSendBuffer.nc", line + "\r\n"); // clear file
                    streamingBuffer.LineWasSent();
                    streamingStateOld = streamingStateNow;
                    lengthToSend = streamingBuffer.LengthSent() + 1;    // update while-variable
                }   // lock
                ProcessSend();
                //                Logger.Trace("preProcessStreaming sent {0}  lengthToSend {1}  grblBufferFree {2} 3busy {3} countPreventIdle {4} line {5}", streamingBuffer.IndexSent, lengthToSend, grblBufferFree, serial3Busy, countPreventIdle, line);
            }   // while

            if (streamingStateNow != GrblStreaming.pause)
            {   if (trgEvent) SendStreamEvent(streamingStateNow); trgEvent = false; }    // streaming in preProcessStreaming
        }


/***************************************************************
* streamingIDLE()
* called in RX event chain, from processGrblStateChange()
***************************************************************/
        private void StreamingIDLE()
        {
            // in main GUI: send extra Pause-Code in MainTimer_Tick from Properties.Settings.Default.flowControlText
            // OnRaiseStreamEvent - case grblStreaming.pause: if (isStreamingPauseFirst && Properties.Settings.Default.flowControlEnable) delayedSend = 2;
            if (countPreventIdle <= 1)
            {
                waitForIdle = false;
//                addToLog("---------- IDLE state reached ---------");
                if (logStartStop) Logger.Trace(" grblStateChanged() ---------- IDLE state reached --------- {0}", ListInfoStream());
                if ((isStreamingRequestPause || isStreamingRequestStopp) && !isStreamingPause)
                {
                    isStreamingPause = true;
                    isStreamingRequestPause = false;
                    streamingStateNow = GrblStreaming.pause;
                    posPause = posWork;
                    AddToLog(string.Format("[Save Settings X:{0} Y:{1}]", posPause.X, posPause.Y));

                    if (logStartStop) Logger.Trace("updateStreaming IDLE reached");
                    gcodeVariable["MLAX"] = posMachine.X; gcodeVariable["MLAY"] = posMachine.Y; gcodeVariable["MLAZ"] = posMachine.Z;
                    gcodeVariable["WLAX"] = posWork.X; gcodeVariable["WLAY"] = posWork.Y; gcodeVariable["WLAZ"] = posWork.Z;

                    if (getParserState)
                    {   RequestSend("$G"); }

                    if (isStreamingRequestStopp)	// 20200717
					{	
						if (logStartStop) Logger.Trace(" grblStateChanged() - now really stop - resetStreaming");
						StopStreamingFinal();
                        isStreaming = false;
					}
                    UpdateControls();
                }

                if (streamingStateOld != streamingStateNow)
                    SendStreamEvent(streamingStateNow);     // streaming in streamingIDLE()
                streamingStateOld = streamingStateNow;

                if (streamingBuffer.IndexConfirmed >= streamingBuffer.Count)
                {   StreamingFinish(); }
            }
        }

/***** last command was sent/received - end streaming *****/
        private void StreamingFinish()
        {
            countPreventEvent = 0; countPreventOutput = 0;

            AddToLog(string.Format("\r[Streaming finish line:{0}]", streamingBuffer.GetConfirmedLineNr()));
            Logger.Info("streamingFinish ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲");
            SendStreamEvent(streamingStateNow);     // streaming in streamingFinish()
            streamingStateNow = GrblStreaming.finish;

            OnRaiseStreamEvent(new StreamEventArgs(streamingBuffer.MaxLineNr, streamingBuffer.MaxLineNr, 100, 0, GrblStreaming.finish));
            if (Properties.Settings.Default.grblPollIntervalReduce)
            {
                timerSerial.Interval = Grbl.pollInterval;
                countMissingStatusReport = (int)(10000 / timerSerial.Interval);
            }
            sendBuffer.Clear();
            streamingBuffer.Clear();
            ResetStreaming();
            UpdateControls();
            if (isStreamingCheck)
            { RequestSend("$C"); isStreamingCheck = false; }
        }

        private void SetToolChangeCoordinates(int cmdTNr, string line="")
        { 
            ToolProp toolInfo = ToolTable.GetToolProperties(cmdTNr);
            gcodeVariable["TOAN"] = cmdTNr;
            if (toolInfo.Toolnr != cmdTNr)
            {   gcodeVariable["TOAX"] = gcodeVariable["TOAY"] = gcodeVariable["TOAZ"] = gcodeVariable["TOAA"] = 0;
                if (cBStatus1.Checked || cBStatus.Checked) AddToLog("\r[Tool change: " + cmdTNr.ToString() + " no Information found! (" + line + ")]");
            }
            else
            {   // get new values
                double tx, ty, tz, ta;
                gcodeVariable["TOAX"] = tx = toolInfo.Position.X + (double)Properties.Settings.Default.toolTableOffsetX;
                gcodeVariable["TOAY"] = ty = toolInfo.Position.Y + (double)Properties.Settings.Default.toolTableOffsetY;
                gcodeVariable["TOAZ"] = tz = toolInfo.Position.Z + (double)Properties.Settings.Default.toolTableOffsetZ;
                gcodeVariable["TOAA"] = ta = toolInfo.Position.A + (double)Properties.Settings.Default.toolTableOffsetA;
                string coord = string.Format("X:{0:0.0} Y:{1:0.0} Z:{2:0.0} A:{3:0.0}",tx,ty,tz,ta);
                if (cBStatus1.Checked || cBStatus.Checked)  AddToLog("\r[set tool coordinates " + cmdTNr.ToString() + " " + coord +  "]");
            }
        }

   /*     private int InsertComment(int index, int linenr, string cmt)
        {
            if (!streamingBuffer.Insert(index, cmt, linenr))
                Logger.Error("insertComment nok index:{0} buffer:{1}",index, streamingBuffer.Count);
            index++;
            return index;
        }*/
    }
}
