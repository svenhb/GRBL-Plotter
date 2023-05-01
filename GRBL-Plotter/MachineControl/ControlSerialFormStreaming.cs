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
/* 2020-09-18 split file
 * 2020-12-18 fix OnRaiseStreamEvent %
 * 2021-01-23 ### synchronize SendStreamEvent via trgEvent in TimerSerial_Tick to status query poll frequency
 * 2021-01-26 line 270 make delay between probe exchange scripts variable
 * 2021-02-19 insert variables in subroutine already in startStreaming()
 * 2021-02-28 preProcessStreaming lock copy-routine and replace "requestSend()" by sendBuffer.Add
 * 2021-07-12 code clean up / code quality
 * 2022-01-02 line 354 AddCodeFromFile -> add MakeAbsolutePath
 * 2022-01-17 Avoid grbl-error:20 if T01M06 but "Perform tool change" not enabled line 272;  AddCodeFromFile - no error if filename = "" line 354
 * 2022-05-27 line 650 keep size of sendBuffer small
 * 2022-08-10 lock {sendBuffer.Clear();
 * 2023-01-25 #321 line 757 remove streamingBuffer.LineWasSent();
 * 2023-01-29 line 757 streamingBuffer.Add to increment streamingBuffer.Count
 * 2023-03-29 l:112, 583, 689, 851 add option lowLevelPerformance
 * 2023-03-30 l:309 add "M0" to tool change command (to allow manual tool change on TxM06 command)
 * 2023-04-01 l:200 f:StartStreaming bug fix, subroutine number should be usable with 4 digits
 * 2023-04-07 l:198 f: f:StartStreaming skip line if starts with "("; l:482 f:FindDouble -> change to TryParse
 */

// OnRaiseStreamEvent(new StreamEventArgs((int)lineNr, codeFinish, buffFinish, status));
// OnRaisePosEvent(new PosEventArgs(posWork, posMachine, grblStateNow, machineState, mParserState, rxString));// lastCmd));

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
        private bool progressUpdateMarker = false;

        private Dictionary<int, List<string>> subroutines = new Dictionary<int, List<string>>();

        private bool posChangedMonitor = false;
        private uint countDownMonitor = 10;

        public event EventHandler<StreamEventArgs> RaiseStreamEvent;
        protected virtual void OnRaiseStreamEvent(StreamEventArgs e)
        { RaiseStreamEvent?.Invoke(this, e); }

        private string ListInfoStream()
        { return string.Format("{0}strmBuffer snt:{1,3}  cnfrmnd:{2,3}  cnt:{3,5}  BFree:{4,3}  lineNr:{5}  code:'{6}' state:{7}", "", streamingBuffer.IndexSent, streamingBuffer.IndexConfirmed, streamingBuffer.Count, grblBufferFree, streamingBuffer.GetConfirmedLineNr(), streamingBuffer.GetConfirmedLine(), grblStateNow.ToString()); }

        /****************************************************************************
        *  startStreaming called by main-Prog
        *  get complete GCode list and copy to streamingBuffer
        *  initialize streaming
        *  if startAtLine > 0 start with pause
        ****************************************************************************/
        public void StartStreaming(IList<string> gCodeList, int startAtLine, int stopAtLine, bool check)
        {
            lowLevelPerformance = Properties.Settings.Default.grblPollIntervalReduce;
            grblCharacterCounting = Properties.Settings.Default.grblStreamingProtocol1 && !isMarlin;
            Logger.Info("▼▼▼▼▼ Ser:{0} startStreaming at line:{1} to line:{2} ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼", iamSerial, startAtLine, stopAtLine);

            string infoProtocol = "Streaming Protocol: ";
            infoProtocol += grblCharacterCounting ? "Character-Counting" : "Simple Send -Response";

            string infoLowPerformance = "RX-mode: ";
            infoLowPerformance += lowLevelPerformance ? "sync." : "async.";

            Logger.Info("{0} | {1}", infoProtocol, infoLowPerformance);

            UpdateLogging();

            grblBufferSize = Grbl.RX_BUFFER_SIZE;  //rx bufer size of grbl on arduino 127
            grblBufferFree = Grbl.RX_BUFFER_SIZE;
            Logger.Info("Set Buffer Free:{0}  Size:{1}", grblBufferFree, grblBufferSize);

            if (lowLevelPerformance)
            { timerSerial.Interval = Grbl.pollInterval * 2; }
            else
            { timerSerial.Interval = Grbl.pollInterval; }
            countMissingStatusReport = (int)(10000 / timerSerial.Interval);
            Logger.Info("Poll frequency:{0}  max. missing reports:{1}", 1000 / timerSerial.Interval, countMissingStatusReport);

            skipM30 = false;
            lastError = "";
            countGrblError = 0;
            lastSentToCOM.Clear();
            ToolTable.Init(" (StartStreaming)");       // fill structure
            rtbLog.Clear();

            // check if other serial are still alive
            if (useSerial2)
            {
                try
                {
                    if (!_serial_form2.SerialPortOpen)
                    { AddToLog("[2nd serial port is not open]"); useSerial2 = false; }
                }
                catch
                {
                    useSerial2 = false;
                }
            }
            if (useSerial3)
            {
                try
                {
                    if (!_serial_form3.SerialPortOpen)
                    { AddToLog("[3rd serial port is not open]"); useSerial3 = false; }
                }
                catch
                {
                    useSerial3 = false;
                }
            }

            if (!check)
            {
                if (logStreamData) AddToLog("* Logging enabled");
                AddToLog("[Start streaming - no echo at " + GetTimeStampString() + " ]");
                AddToLog("[" + infoProtocol + " | " + infoLowPerformance + "]");
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

            streamingBuffer.Clear();    // on start streaming
            ResetStreaming();           // startStreaming
            if (isStreamingCheck)
            {
                SendLine("$C");         // startStreaming check
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
            {
                if (gCode[i].Trim().StartsWith("("))    // don't care about comments
                    continue;

                if (gCode[i].Contains("O"))             // find and store subroutines - nothing else
                {
                    int cmdONr = (int)FindDouble("O", -1, gCode[i]); //Gcode.GetCodeNrFromGCode4Digit('O', gCode[i]);
                    if (cmdONr <= 0)
                        continue;
                    Logger.Trace("Add subroutine O{0}", cmdONr);
                    if (!subroutines.ContainsKey(cmdONr))   // 2021-12-17 added
                        subroutines.Add(cmdONr, new List<string>());
                    else
                        Logger.Error("StartStreaming subroutines key {0} already added", cmdONr);
                    useSubroutine = true;

                    for (int k = i + 1; k < gCode.Length; k++)
                    {
                        if (gCode[k].IndexOf("M99") >= 0)
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
                                Logger.Error("Start stresaming Subroutine {0} not found", pWord);
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
                                    foreach (string subroutineLine in subroutines[(int)pWord])          // copy subroutine
                                    {
                                        if (subroutineLine.Contains('#'))                    // check if variable neededs to be replaced
                                            streamingBuffer.Add(InsertVariable(subroutineLine).Replace(" ", ""), i);
                                        else
                                            streamingBuffer.Add(subroutineLine.Replace(" ", ""), i);             // add gcode line to list to send 
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
                            {
                                streamingBuffer.SetSentLine("(" + tmp + " - unknown)");    // don't pass unkown GCode to GRBL because is unknown
                                tmp = streamingBuffer.GetSentLine();
                                streamingBuffer.LineWasReceived();
                                AddToLog(tmp);
                            }
                            if (cmdTNr >= 0)                                        // T-word is allowed by grbl - no need to filter
                            {
                                SetToolChangeCoordinates(cmdTNr, tmp);              // update variables e.g. "gcodeVariable[TOAX]" from tool-table
                            }
                            if (cmdMNr == 6)                                        // M06 is not allowed - remove
                            {
                                tmp = "(" + tmp + ")";
                                if (Properties.Settings.Default.ctrlToolChange)
                                { 	InsertToolChangeCode(i, ref tmpToolInSpindle); }// insert external script-code and insert variables 
                                else
                                {
                                    AddToLog(tmp + " !!! Tool change is disabled");
                                    Logger.Warn("⚠ Found '{0}' but tool change is disabled in [Setup - Tool change]", tmp);
									tmp = "M0 " + tmp;
                                }
                            }
                            if (cmdMNr == 30)
                            {
                                if (skipM30)
                                { tmp = "(" + tmp + ")"; }      // hide M30
                            }

                            streamingBuffer.Add(tmp.Replace(" ", ""), i);        // add gcode line to list to send

                            if (cmdMNr == 30)                   // stop filling buffer, to avoid sending subroutine code
                            {
                                foundM30 = true;
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
                                                                  //    streamingBuffer.Add("()", stopAtLine);            // add gcode line to list to send
            }   // lock
            timerSerial.Start();

            if (logStreamData || useSubroutine)
            {
                string startText = string.Format("( {0} )\r\n( Start at line:{1} stop:{2} )\r\n( Set ParserState:{3} )\r\n", GetTimeStampString(), startAtLine, stopAtLine, parserStateGC);
                File.WriteAllText(Datapath.LogFiles + "\\" + logFileGCode, "( Data to stream from editor )\r\n" + startText); // clear file
                File.AppendAllLines(Datapath.LogFiles + "\\" + logFileGCode, streamingBuffer.Buffer);
            }

            if (logStreamData)
            {
                string startText = string.Format("( {0} )\r\n( Start at line:{1} stop:{2} )\r\n( Set ParserState:{3} )\r\n", GetTimeStampString(), startAtLine, stopAtLine, parserStateGC);
                File.WriteAllText(Datapath.LogFiles + "\\" + logFileSentData, "( Data sent to grbl )\r\n" + startText); // clear file
                File.WriteAllText(Datapath.LogFiles + "\\" + logFileEcho, "( Data echoed by grbl if #define REPORT_ECHO_LINE_RECEIVED )\r\n" + startText); // clear file
            }
            isStreaming = true;
            UpdateControls();
            if (startAtLine > 0)
            {
                PauseStreaming();
                isStreamingPause = true;
            }
            else
            {
                waitForIdle = false;
                PreProcessStreaming();              // StartStreaming
            }
        }   // startStreaming

        private void InsertToolChangeCode(int line, ref bool inSpindle)
        {
            Logger.Info("InsertToolChangeCode line:{0} tool is in spindle:{1}", line, inSpindle);
            streamingBuffer.Add("($TS)", line);         // keyword for receiving-buffer (sendBuffer.GetConfirmedLine();) "Tool change start"
            if (inSpindle)
            {
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptPut, line);
                inSpindle = false;
                if (gcodeVariable.ContainsKey("TOLN"))
                { streamingBuffer.Add("($TO T" + gcodeVariable["TOLN"] + ")", line); }   // keyword for receiving-buffer "Tool removed"
                else
                {
                    AddToLog("InsertToolChangeCode var 'TOLN' not set!");
                    Logger.Error("InsertToolChangeCode var 'TOLN' not set!");
                }
            }
            if (!Properties.Settings.Default.ctrlToolChangeEmpty || (gcodeVariable["TOAN"] != (int)Properties.Settings.Default.ctrlToolChangeEmptyNr))
            {
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptSelect, line);
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptGet, line);
                inSpindle = true;
                streamingBuffer.Add("($TI T" + gcodeVariable["TOAN"] + ")", line);  // keyword for receiving-buffer "Tool inserted"
                AddCodeFromFile(Properties.Settings.Default.ctrlToolScriptProbe, line);
            }

            streamingBuffer.Add("($TE)", line);         // keyword for receiving-buffer "Tool change finished"

            if (Properties.Settings.Default.ctrlToolScriptDelay > 0)
                streamingBuffer.Add(string.Format("G4 P{0:0.00}", Properties.Settings.Default.ctrlToolScriptDelay), line);

            // save actual tool info as last tool info
            gcodeVariable["TOLN"] = gcodeVariable["TOAN"];	// TOol Last Number = TOol Actual Number
            gcodeVariable["TOLX"] = gcodeVariable["TOAX"];
            gcodeVariable["TOLY"] = gcodeVariable["TOAY"];
            gcodeVariable["TOLZ"] = gcodeVariable["TOAZ"];
            gcodeVariable["TOLA"] = gcodeVariable["TOAA"];
        }

        private void AddCodeFromFile(string fileRaw, int linenr)
        {
            if (fileRaw.Length < 3)     // no path at all
                return;

            string file = Datapath.MakeAbsolutePath(fileRaw);
            if (Path.GetFileName(file) == string.Empty)         // path but no filename
                return;

            Logger.Info("◯◯◯ AddCodeFromFile file:{0}  line:{1}", file, linenr);

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
                        { Logger.Error("AddCodeFromFile Subroutine {0} not found", pWord); }
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
            else
            {
                Logger.Error("AddCodeFromFile file not found:{0}", file);
                AddToLog("!!! Tool change script not found: " + fileRaw);
            }
        }


        /****** get number from string ******/
        private static double FindDouble(string start, double notfound, string txt)
        {
            int istart = txt.IndexOf(start);
            if (istart < 0)
                return notfound;
            string line = txt.Substring(istart + start.Length);
            string num = "";
            foreach (char c in line)
            {
                if (Char.IsLetter(c))
                    break;
                else if (Char.IsNumber(c) || c == '.' || c == '-')
                    num += c;
            }
            if (num.Length < 1)
                return notfound;

            if (double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out double parsed))
        //    if (double.TryParse(num, out double parsed))
                return parsed;

            Logger.Warn("FindDouble {0}  {1}", start, txt);
            return notfound;
        }

        /************************************************************************
         * pauseStreaming()
         * Pause or restart streaming after button click
         ************************************************************************/
        public void PauseStreaming()
        {
            UpdateLogging();
            //    if (logStartStop) Logger.Trace("Ser:{0} pauseStreaming()  isStreamingPause {1}   isStreamingRequestPause {2}   {3}", iamSerial, isStreamingPause, isStreamingRequestPause, ListInfoStream());
            // start pause			
            if (!isStreamingPause)
            {
                isStreamingRequestPause = true;     // wait until buffer is empty before switch to pause
                AddToLog("[Pause streaming - wait for IDLE]");
                AddToLog("[Save Settings]");
                Logger.Info("▲▼▲▼▲ pauseStreaming RequestPause    ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ Outstanding lines:{0}", (sendBuffer.Count - sendBuffer.IndexConfirmed));
                Logger.Info("  capacity:{0,6} {1}", sendBuffer.Capacity, ListInfoSend());
                Logger.Info("  capacity:{0,6} {1}", streamingBuffer.Capacity, ListInfoStream());
                streamingStateNow = GrblStreaming.waitidle;
                getParserState = true;
            }
            // restart streaming			
            else
            {   //if ((posPause.X != posWork.X) || (posPause.Y != posWork.Y) || (posPause.Z != posWork.Z))
                AddToLog("++++++++++++++++++++++++++++++++++++");
                timerSerial.Interval = Grbl.pollInterval;
                if (parserStateGC.Contains("F0"))
                {
                    parserStateGC = parserStateGC.Replace("F0", "F100");            // Avoid missing feed rate
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
                Logger.Info("▲▼▲▼▲ pauseStreaming start streaming ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼");
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

                PreProcessStreaming();              // pauseStreaming start streaming
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
            {
                SendLine("$C");         // stopStreaming check
                isStreamingCheck = false;
            }

            if (isNotStartup)
                StopStreamingFinal();
            else
            {
                IsHeightProbing = false;
                ResetStreaming(false);      // stopStreaming
                isStreamingRequestStopp = true;     // 20200717
                isStreamingRequestPause = true;     // 20200717
                Logger.Info("▲▲▲▲▲ stopStreaming() - wait for IDLE - sent:{0}  received:{1}  ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲", streamingBuffer.IndexSent, streamingBuffer.IndexConfirmed);
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
            Logger.Info("▲▲▲▲▲ StopStreamingFinal() ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲");

            IsHeightProbing = false;

            ResetStreaming(true);       // stopStreamingFinal()
            if (logStartStop) Logger.Trace(" stopStreamingFinal() - lines in buffer {0}", (streamingBuffer.IndexSent - streamingBuffer.IndexConfirmed));

            if (isStreamingCheck)
            {
                SendLine("$C");         // stopStreamingFinal check
                isStreamingCheck = false;
            }
            UpdateControls();
            if (lowLevelPerformance)
            {
                timerSerial.Interval = Grbl.pollInterval;   // set back original value
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
            {
                lock (sendDataLock)
                {
                    streamingBuffer.Clear();    // ResetStreaming
                    sendBuffer.Clear();         // ResetStreaming
                }
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
                {
                    streamingBuffer.LineWasReceived();
                    //                streamingBuffer.DeleteLine();		// better don't removeAt(0)...
                    allowStreamingEvent = true;
                }
                else
                    streamingStateNow = GrblStreaming.pause;   // update status

                // send status via event to main GUI
                if ((streamingStateOld != streamingStateNow) || allowStreamingEvent)
                {
                    if (streamingStateNow != GrblStreaming.pause)
                    {
                        if (trgEvent) { SendStreamEvent(streamingStateNow); } // streaming processOkStreaming
                        trgEvent = false;
                    }
                    streamingStateOld = streamingStateNow;      //grblStatus = oldStatus;
                    allowStreamingEvent = false;
                }
            }
        }

        /*****  sendStreamEvent update main prog  *****/
		/* update only on  streamingStateNow change or 'ok' synchronized with timer via trgEvent=true*/
        private void SendStreamEvent(GrblStreaming status, int linePause = -1)
        {
            int lineNrSent = streamingBuffer.GetSentLineNr();
            if (linePause > 0)
                lineNrSent = linePause;

            int lineNrConfirmed = streamingBuffer.GetConfirmedLineNr();
            if (status == GrblStreaming.error)
                lineNrConfirmed = sendBuffer.GetConfirmedLineNr() + 1;

            // progressbar.value is int
            int codeFinish = 0;
            if (streamingBuffer.Count != 0)
            { codeFinish = (int)Math.Ceiling((float)lineNrConfirmed * 100 / streamingBuffer.MaxLineNr) + 1; }     	// to reach 100%
            int buffFinish = 0;
            if (grblBufferSize != 0)
            { buffFinish = (int)Math.Ceiling((float)(grblBufferSize - grblBufferFree) * 100 / grblBufferSize) + 1; }	// to reach 100%

            if (codeFinish > 100) { codeFinish = 100; }
            if (buffFinish > 100) { buffFinish = 100; }

            if ((codeFinish % 5) == 0)
            {
                if (progressUpdateMarker)
                {
                    progressUpdateMarker = false;
                    AddToLog(string.Format("Progress:{0,3}%  line:{1,6}   time:{2}", codeFinish, lineNrConfirmed, DateTime.Now.ToString("HH:mm:ss")));
                    Logger.Info("🗲🗲 SendStreamEvent    Progress:{0,3}%  Bfree:{1,3}/{2,3}  snt:{3,3}  cnfrmnd:{4,3}  cnt:{5,5}   lineNr:{6}  code:'{7}'  state:{8}", codeFinish, grblBufferFree, grblBufferSize, sendBuffer.IndexSent, sendBuffer.IndexConfirmed, sendBuffer.Count, sendBuffer.GetConfirmedLineNr(), sendBuffer.GetConfirmedLine(), grblStateNow.ToString());
                }
            }
            else { progressUpdateMarker = true; }
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
            if (!lowLevelPerformance)
            {
                int lengthFree = sendBuffer.MissingConfirmationLength();        // 2022-05-27 keep sendBuffer small, for short reaction time on pause request
                if (lengthFree > grblBufferSize)
                {
                    //Logger.Trace("PreProcessStreaming abort- MissingConfirmationLength:{0}  size:{1}", lengthFree, grblBufferSize);
                    return;
                }
            }

            if (waitForIdle || isStreamingRequestPause || (countPreventIdle > 0))
            {
                Logger.Trace("PreProcessStreaming abort- waitForIdle:{0}  isStreamingRequestPause:{1}  countPreventIdle:{2}", waitForIdle, isStreamingRequestPause, countPreventIdle);
                return;
            }

            int lengthToSend = streamingBuffer.LengthSent() + 1;
            while ((streamingBuffer.IndexSent < streamingBuffer.Count) && (grblBufferFree >= lengthToSend) && !waitForIdle && (streamingStateNow != GrblStreaming.pause) && ExternalCOMReady())
            {
                lock (sendDataLock)
                {
                    string line = streamingBuffer.GetSentLine();
                    if ((line == "OV") || (line == "UR"))
                    {
                        Logger.Error("preProcessStreaming read:{0} from  streamingBuffer index:{1} count:{2}", line, streamingBuffer.IndexSent, streamingBuffer.Count);
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
                    {	// frequently insert M114 command into sendBuffer
                        if (updateMarlinPosition)// || (--insertMarlinCounter <= 0))
                        {
                            sendBuffer.Add("M114", streamingBuffer.GetSentLineNr());
                            streamingBuffer.Add("()", streamingBuffer.Count);      // streamingBuffer.Count will be checked later
                            getMarlinPositionWasSent = true;
                            streamingStateOld = streamingStateNow;
                        //    insertMarlinCounter = insertMarlinCounterReload;
                        }
                        updateMarlinPosition = false;
                    }
                    /* remove spaces from line (in echo, there are also no spaces) */
                    sendBuffer.Add(line.Replace(" ", ""), streamingBuffer.GetSentLineNr());
                    streamingBuffer.LineWasSent();
                    streamingStateOld = streamingStateNow;
                    lengthToSend = streamingBuffer.LengthSent() + 1;    // update while-variable
                }   // lock
                ProcessSend();          // PreProcessStreaming
            }   // while

            if (streamingStateNow != GrblStreaming.pause)
            {
                if (trgEvent) { SendStreamEvent(streamingStateNow); } // streaming in preProcessStreaming
                trgEvent = false;
            }
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
                    { RequestSend("$G"); }

                    if (isStreamingRequestStopp)    // 20200717
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
                {
                    Logger.Info("StreamingIDLE -> StreamingFinish");
                    StreamingFinish();
                }
            }
        }

        /***** last command was sent/received - end streaming *****/
        private void StreamingFinish()
        {
            countPreventEvent = 0; countPreventOutput = 0;

            AddToLog(string.Format("\r[Streaming finish line:{0}]", streamingBuffer.GetConfirmedLineNr()));
            Logger.Info("▲▲▲▲▲ streamingFinish  IndexSent:{0}  IndexConfirmed:{1} ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲", sendBuffer.IndexSent, sendBuffer.IndexConfirmed);
            //    SendStreamEvent(streamingStateNow);     // streaming in streamingFinish()
            streamingStateNow = GrblStreaming.finish;

            OnRaiseStreamEvent(new StreamEventArgs(streamingBuffer.MaxLineNr, streamingBuffer.MaxLineNr, 100, 0, GrblStreaming.finish));
            if (lowLevelPerformance)
            {
                timerSerial.Interval = Grbl.pollInterval;   // set back original value
                countMissingStatusReport = (int)(10000 / timerSerial.Interval);
            }
            ResetStreaming();       // sendBuffer.Clear(); streamingBuffer.Clear();
            UpdateControls();
            if (isStreamingCheck)
            { RequestSend("$C"); isStreamingCheck = false; }

            ListAccessoryStateRunTime(false);
        }

        private void SetToolChangeCoordinates(int cmdTNr, string line = "")
        {
            ToolProp toolInfo = ToolTable.GetToolProperties(cmdTNr);
            gcodeVariable["TOAN"] = cmdTNr;
            if (toolInfo.Toolnr != cmdTNr)
            {
                gcodeVariable["TOAX"] = gcodeVariable["TOAY"] = gcodeVariable["TOAZ"] = gcodeVariable["TOAA"] = 0;
                if (cBStatus1.Checked || cBStatus.Checked) AddToLog("\r[Tool change: " + cmdTNr.ToString() + " no Information found! (" + line + ")]");
            }
            else
            {   // get new values
                double tx, ty, tz, ta;
                gcodeVariable["TOAX"] = tx = toolInfo.Position.X + (double)Properties.Settings.Default.toolTableOffsetX;
                gcodeVariable["TOAY"] = ty = toolInfo.Position.Y + (double)Properties.Settings.Default.toolTableOffsetY;
                gcodeVariable["TOAZ"] = tz = toolInfo.Position.Z + (double)Properties.Settings.Default.toolTableOffsetZ;
                gcodeVariable["TOAA"] = ta = toolInfo.Position.A + (double)Properties.Settings.Default.toolTableOffsetA;
                string coord = string.Format("X:{0:0.0} Y:{1:0.0} Z:{2:0.0} A:{3:0.0}", tx, ty, tz, ta);
                if (cBStatus1.Checked || cBStatus.Checked) AddToLog("\r[set tool coordinates " + cmdTNr.ToString() + " " + coord + "]");
            }
        }

        private void StreamingMonitor()
        {
            if (posChangedMonitor)      // check change in X,Y and Z
            {
                posChangedMonitor = false;
                countDownMonitor = 10;
            }
            else
            {
                if (countDownMonitor-- > 0)
                    return;

                countDownMonitor = (uint)(10 * 1000 / timerSerial.Interval); // update 10 sec.

                if (isStreamingRequestPause || isStreamingPause)
                {
                    if (isStreamingPause)
                        countDownMonitor = (uint)(60000 / timerSerial.Interval);     // update after one minute
                    Logger.Info("⏱ StreamingMonitor isStreamingPause:{0} request:{1}  counter:{2}   interval:{3} ms", isStreamingPause, isStreamingRequestPause, countDownMonitor, timerSerial.Interval);
                    return;
                }

                Logger.Info("⚠ StreamingMonitor sendBuffer.OkToSend:{0}  grblBufferFree:{1}  waitForIdle:{2}  (true(x) && false)", sendBuffer.OkToSend(grblBufferFree), grblBufferFree, waitForIdle);
                Logger.Info("    StreamingMonitor sendBuffer.GetSentLine:'{0}'  IndexSent:{1}  Count:{2}  LineSent:{3}", sendBuffer.GetSentLine(), sendBuffer.IndexSent, sendBuffer.Count, sendBuffer.GetSentLineNr());

                /* why it's IDLE, what we are waiting for? */
                bool ready = ExternalCOMReady();
                if (!ready)
                {
                    Logger.Warn("    StreamingMonitor  serial2Busy:{0} form2.grblStateNow:{1}  (false && idle)", serial2Busy, _serial_form2.grblStateNow);
                    Logger.Warn("    StreamingMonitor  serial3Busy:{0} form3.Busy:{1}  (false && false)", serial3Busy, _serial_form3.Busy);
                }
                Logger.Info("    StreamingMonitor  ExternalCOMReady:{0}  waitForOk:{1} grblStateNow:{2}  (true, false || alarm)", ExternalCOMReady(), waitForOk, grblStateNow);
                Logger.Info("    StreamingMonitor  call PreProcessStreaming");
                Logger.Info("    capacity:{0,6} {1}", sendBuffer.Capacity, ListInfoSend());
                Logger.Info("    capacity:{0,6} {1}", streamingBuffer.Capacity, ListInfoStream());
                PreProcessStreaming();              // StreamingMonitor
                                                    //    this.BeginInvoke(new EventHandler(PreProcessStreaming));	// try to wake-up 
            }
        }
    }
}
