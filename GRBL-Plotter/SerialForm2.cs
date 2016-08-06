/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2016 Sven Hasemann contact: svenhb@web.de

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class SerialForm2 : Form
    {
        public xyzPoint posWorld, posMachine, posPause, posProbe, posProbeOld;

        private string rxString;
        private string parserState = "";
        private bool serialPortOpen = false;
        public bool SerialPortOpen
        { get { return serialPortOpen; } }

        private Dictionary<string, double> gcodeVariable = new Dictionary<string, double>();

        public SerialForm2()
        {
            InitializeComponent();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
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
            if ((Location.X < 0) || (Location.X > desktopSize.Width) || (Location.Y < 0) || (Location.Y > desktopSize.Height)) { Location = new Point(325, 0); }
            SerialForm_Resize(sender, e);
            refreshPorts();
            updateControls();
            loadSettings();
            openPort();
        }
        private bool mainformAskClosing = false;
        private void SerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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
            rtbLog.Height = this.Height - 140;
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
            {
                minimizeCount--;
                if (minimizeCount == 0)
                    this.WindowState = FormWindowState.Minimized;
            }

            if (!serialPort2.IsOpen)
            { }     // BringToFront();
            else
            {
                try
                {
                    var dataArray = new byte[] { Convert.ToByte('?') };
                    serialPort2.Write(dataArray, 0, 1);
                }
                catch (Exception er)
                {
                    logError("Retrieving GRBL status", er);
                    serialPort2.Close();
                }
            }
            if (waitForIdle)
                processSend();
        }

        private void loadSettings()
        {
            try
            {
                cbPort.Text = Properties.Settings.Default.port2;
                cbBaud.Text = Properties.Settings.Default.baud2;
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
                Properties.Settings.Default.port2 = cbPort.Text;
                Properties.Settings.Default.baud2 = cbBaud.Text;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                logError("Saving settings", e);
            }
        }

        private void updateControls()
        {
            bool isConnected = serialPort2.IsOpen;
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
            if (serialPort2.IsOpen)
                closePort();
            else
                openPort();
        }
        private int minimizeCount = 0;
        private bool openPort()
        {
            try
            {
                serialPort2.PortName = cbPort.Text;
                serialPort2.BaudRate = Convert.ToInt32(cbBaud.Text);
                serialPort2.Open();
                rtbLog.Clear();
                rtbLog.AppendText("Open " + cbPort.Text + "\r\n");
                btnOpenPort.Text = "Close";
                isDataProcessing = true;
                grblReset(false);
                updateControls();
                if (Properties.Settings.Default.serialMinimize)
                    minimizeCount = 10;     // minimize window after 10 timer ticks
                timerSerial.Interval = 200;
                //                this.WindowState = FormWindowState.Minimized;
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
                if (serialPort2.IsOpen)
                {
                    serialPort2.Close();
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
                return (false);
            }
        }

        //Send reset sentence
        public void grblReset(bool savePos = true)//Stop/reset button
        {
            isStreaming = false;
            isStreamingPause = false;
            waitForIdle = false;
            rtbLog.AppendText("[CTRL-X]");
            var dataArray = new byte[] { 24 };//Ctrl-X
            serialPort2.Write(dataArray, 0, 1);
        }

        string mens;
        Exception err;
        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while ((serialPort2.IsOpen) && (serialPort2.BytesToRead > 0))
            {
                rxString = string.Empty;
                try
                {
                    rxString = serialPort2.ReadTo("\r\n");              //read line from grbl, discard CR LF
                    isDataProcessing = true;
                    this.Invoke(new EventHandler(handleRxData));        //tigger rx process 
                    while ((serialPort2.IsOpen) && (isDataProcessing)) ;  //wait previous data line processed done
                }
                catch (Exception errort)
                {
                    serialPort2.Close();
                    mens = "Error reading line from serial port";
                    err = errort;
                    this.Invoke(new EventHandler(logErrorThr));
                }
            }
        }
        private void handleRxData(object sender, EventArgs e)
        {
            if (rxString.IndexOf("['$' for help]") >= 0)            // reset message
            {
                isDataProcessing = false;
                sendLines.Clear();
                sendLinesCount = 0;
                sendLinesSent = 0;
                sendLinesConfirmed = 0;
                grblBufferFree = grblBufferSize;
//                OnRaiseStreamEvent(new StreamEventArgs(0, 0, 0, grblStreaming.reset));
                addToLog("> RESET\r\n" + rxString);
                return;
            }
            if ((rxString.Length > 0) && (rxString[0] == '<'))      // Status message with coordinates
            {
                handleStatusMessage(rxString);
                isDataProcessing = false;
                return;
            }
            if (rxString.IndexOf("[PRB:") >= 0)                       // Probe message with coordinates
            {   // [PRB:-155.000,-160.000,-28.208:1]
                handleStatusMessage(rxString, true);
                addToLog(string.Format("rx> {0}", rxString));
                isDataProcessing = false;
                return;
            }
            updateStreaming(rxString);
            isDataProcessing = false;
            return;
        }

        public grblState grblStateNow = grblState.unknown;
        private grblState grblStateLast = grblState.unknown;
        private void handleStatusMessage(string text, bool probe = false)
        {
            string[] sections = text.Split(',');
            if (probe)              // [PRB:-155.000,-160.000,-28.208:1]
            {
            }
            else
            {   // <Idle,MPos:0.000,0.000,0.000,WPos:0.000,0.000,0.000>
                string status = sections[0].Substring(1).Trim(' ');
                Double.TryParse(sections[1].Substring(5), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out posMachine.X);
                Double.TryParse(sections[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out posMachine.Y);
                Double.TryParse(sections[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out posMachine.Z);
                Double.TryParse(sections[4].Substring(5), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out posWorld.X);
                Double.TryParse(sections[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out posWorld.Y);
                Double.TryParse(sections[6].TrimEnd('>'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out posWorld.Z);
                grblStateNow = grbl.parseStatus(status);
                lblStatus.BackColor = grbl.grblStateColor(grblStateNow);
                lblStatus.Text = status;
                lblPos.Text = string.Format("X={0:0.000} Y={1:0.000} Z={2:0.000}", posMachine.X, posMachine.Y, posMachine.Z);

            }
            if (grblStateNow != grblStateLast) { grblStateChanged(); }
            if (grblStateNow == grblState.idle)
            {
                waitForIdle = false;
                processSend();
            }
            grblStateLast = grblStateNow;
//            OnRaisePosEvent(new PosEventArgs(posWorld, posMachine, grblStateNow));
        }
        public event EventHandler<PosEventArgs> RaisePosEvent;
        protected virtual void OnRaisePosEvent(PosEventArgs e)
        {
            EventHandler<PosEventArgs> handler = RaisePosEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        // check free buffer before sending 
        // 1. requestSend(data) to add data to stack for sending
        // 2. processSend() check if grbl-buffer is free to take commands
        // 3. sendLine(data) if buffer can take commands
        // 4. updateStreaming(rxdata) check if command was sent
        private const int grblBufferSize = 127;  //rx bufer size of grbl on arduino 
        private int grblBufferFree = grblBufferSize;    //actual suposed free bytes on grbl buffer
        private List<string> sendLines = new List<string>();
        private int sendLinesCount;             // actual buffer size
        private int sendLinesSent;              // actual sent line
        private int sendLinesConfirmed;         // already received line
        public void requestSend(string data)
        {
            var tmp = cleanUpCodeLine(data);
            if ((!string.IsNullOrEmpty(tmp)) && (tmp[0] != ';'))//trim lines and remove all empty lines and comment lines
            {
                sendLines.Add(tmp);
                sendLinesCount++;
                processSend();
            }
        }
        private string cleanUpCodeLine(string data)
        {
            var line = data.Replace("\r", "");  //remove CR
            line = line.Replace("\n", "");      //remove LF
            var orig = line;
            int start = orig.IndexOf('(');
            int end = orig.IndexOf(')');
            if (start >= 0) line = orig.Substring(0, start);
            if (end >= 0) line += orig.Substring(end + 1);
            line = line.ToUpper();              //all uppercase
            line = line.Trim();
            return line;
        }
        private bool waitForIdle = false;
        public void processSend()
        {
            while ((sendLinesSent < sendLinesCount) && (grblBufferFree >= sendLines[sendLinesSent].Length + 1))
            {
                var line = sendLines[sendLinesSent];
                bool replaced = false;
                if (line.IndexOf('@') > 0)
                {
                    line = insertVariable(line);
                    replaced = true;
                    if (grblBufferFree < grblBufferSize)
                        waitForIdle = true;
                }
                if (!waitForIdle)
                {
                    if (replaced)
                        sendLines[sendLinesSent] = line;    // needed to get correct length when receiving 'ok'
                    sendLine(line);
                    grblBufferFree -= (line.Length + 1);
                    sendLinesSent++;
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
        private void sendLine(string data)
        {
            try
            {
                serialPort2.Write(data + "\r");
                if (!(isStreaming && !isStreamingPause))
                {
                    rtbLog.AppendText(string.Format("> {0} \r\n", data));//if not in transfer log the txLine
                    rtbLog.ScrollToCaret();
                }
            }
            catch (Exception err)
            {
                logError("Sending line", err);
                updateControls();
            }
        }


        // Streaming
        // 1. startStreaming() copy and filter gcode to list
        // 2. proceedStreaming() to copy data to stack for sending
        private List<string> gCodeLines;         // buffer with gcode commands
        private List<int> gCodeLineNr;           // corresponding line-nr from main-form
        private int gCodeLinesCount;             // amount of lines to sent
        private int gCodeLinesSent;              // actual sent line
        private int gCodeLinesConfirmed;         // received line
        private bool isStreaming = false;        // true when steaming is in progress
        private bool isStreamingRequestPause = false; // true when request pause (wait for idle to switch to pause)
        private bool isStreamingPause = false;    // true when steaming-pause 
        private bool isStreamingCheck = false;    // true when steaming is in progress (check)
        private bool getParserState = false;      // true to send $G after status switched to idle
        private bool isDataProcessing;      // false when no data processing pending
        private grblStreaming grblStatus = grblStreaming.ok;
        public void stopStreaming()
        {
            isStreaming = false;
            if (isStreamingCheck)
            { requestSend("$C"); isStreamingCheck = false; }
        }
        public void pauseStreaming()
        {
            if (!isStreamingPause)
            {
                isStreamingRequestPause = true;     // wait until buffer is empty before switch to pause
                addToLog("[Pause streaming]");
                addToLog("[Save Settings]");
                grblStatus = grblStreaming.waitidle;
                getParserState = true;
            }
            else
            {
/*                if ((posPause.X != posWorld.X) || (posPause.Y != posWorld.Y) || (posPause.Z != posWorld.Z))
                {
                    addToLog("[Restore Position]");
                    requestSend(string.Format("G90 G0 X{0:0.0} Y{1:0.0}", posPause.X, posPause.Y).Replace(',', '.'));  // restore last position
                    requestSend(string.Format("G1 Z{0:0.0}", posPause.Z).Replace(',', '.'));                      // restore last position
                }
  */              addToLog("[Restore Settings]");
                var cmds = parserState.Split(' ');
                foreach (var cmd in cmds)
                { requestSend(cmd); }           // restore actual GCode settings one by one
                addToLog("[Start streaming - no echo]");
                isStreamingPause = false;
                isStreamingRequestPause = false;
                grblStatus = grblStreaming.ok;
                proceedStreaming();
            }
        }
        public void startStreaming(IList<string> gCodeList, bool check = false)
        {
            rtbLog.Clear();
            addToLog("[Start streaming - no echo]");
            isStreaming = true;
            isStreamingPause = false;
            isStreamingCheck = check;
            grblStatus = grblStreaming.ok;
            if (isStreamingCheck)
                requestSend("$C");
            string[] gCode = gCodeList.ToArray<string>();
            gCodeLinesSent = 0;
            gCodeLinesCount = 0;
            gCodeLinesConfirmed = 0;
            grblBufferFree = grblBufferSize;
            gCodeLines = new List<string>();
            gCodeLineNr = new List<int>();
            sendLinesSent = 0;
            sendLinesCount = 0;
            sendLinesConfirmed = 0;
            sendLines.Clear();
            string tmp;
            for (int i = 0; i < gCode.Length; i++)
            {
                tmp = cleanUpCodeLine(gCode[i]);
                if ((!string.IsNullOrEmpty(tmp)) && (tmp[0] != ';'))//trim lines and remove all empty lines and comment lines
                {
                    gCodeLines.Add(tmp);       // add gcode line to list to send
                    gCodeLineNr.Add(i);         // add line nr
                    gCodeLinesCount++;          // Count total lines
                }
            }
            proceedStreaming();
        }
        private void proceedStreaming()
        {
            while ((gCodeLinesSent < gCodeLinesCount) && (grblBufferFree >= gCodeLines[gCodeLinesSent].Length + 1))
            {
                string line = gCodeLines[gCodeLinesSent];
                if (((line == "M0") || (line.IndexOf("M00") >= 0) || (line.IndexOf("M0 ") >= 0)) && !isStreamingCheck)
                {
                    isStreamingRequestPause = true;
                    addToLog("[Pause streaming]");
                    addToLog("[Save Settings]");
                    grblStatus = grblStreaming.waitidle;
                    getParserState = true;
                    sendStreamEvent(gCodeLineNr[gCodeLinesSent], grblStatus);
                    //                    requestSend("N0 ");     // send nothing, but expect ok
                    //                    gCodeLinesSent++;
                    return;                 // abort while - don't fill up buffer
                }
                else
                    requestSend(line);
                gCodeLinesSent++;
            }
        }

        public void updateStreaming(string rxString)
        {
            int tmpIndex = gCodeLinesSent;
            if ((rxString != "ok") || (rxString != "[enabled]"))
            {
                if (rxString.IndexOf("error") >= 0)
                {
                    grblStatus = grblStreaming.error;
                    tmpIndex = gCodeLinesConfirmed;
                    addToLog(">> " + rxString);
                }
                if (getParserState)
                {
                    if (rxString.IndexOf("[G") >= 0)
                    {
                        parserState = rxString.Substring(1, rxString.Length - 2);
                        parserState = parserState.Replace("M0", "");
 //                       posPause = posWorld;
                        getParserState = false;
                    }
                }
            }
            if (!(isStreaming && !isStreamingPause))
                addToLog(string.Format("< {0}", rxString));
            if (sendLinesConfirmed < sendLinesCount)
            {
                grblBufferFree += (sendLines[sendLinesConfirmed].Length + 1);   //update bytes supose to be free on grbl rx bufer
                sendLinesConfirmed++;                   // line processed
            }
            if ((sendLinesConfirmed == sendLinesCount) && (grblStateNow == grblState.idle))   // addToLog(">> Buffer empty\r");
            {
                if (isStreamingRequestPause)
                {
                    isStreamingPause = true;
                    isStreamingRequestPause = false;
                    grblStatus = grblStreaming.pause;
//                    gcodeVariable["MLAX"] = posMachine.X; gcodeVariable["MLAY"] = posMachine.Y; gcodeVariable["MLAZ"] = posMachine.Z;
 //                   gcodeVariable["WLAX"] = posWorld.X; gcodeVariable["WLAY"] = posWorld.Y; gcodeVariable["WLAZ"] = posWorld.Z;

                    if (getParserState)
                    { requestSend("$G"); }
                }
            }
            if (isStreaming)
            {
                if (!isStreamingPause)
                    gCodeLinesConfirmed++;  //line processed
                if (gCodeLinesConfirmed >= gCodeLinesCount)//Transfer finished and processed? Update status and controls
                {
                    isStreaming = false;
                    addToLog("[Streaming finish]");
                    grblStatus = grblStreaming.finish;
                    if (isStreamingCheck)
                    { requestSend("$C"); isStreamingCheck = false; }
                    updateControls();
                }
                else//not finished
                {
                    if (!(isStreamingPause || isStreamingRequestPause))
                        proceedStreaming();//If more lines on file, send it  
                }
                if (tmpIndex >= gCodeLinesCount) { tmpIndex = gCodeLinesCount - 1; }
                sendStreamEvent(gCodeLineNr[tmpIndex], grblStatus);
            }
        }
        private void sendStreamEvent(int lineNr, grblStreaming status)
        {
            float codeFinish = (float)gCodeLinesConfirmed * 100 / (float)gCodeLinesCount;
            float buffFinish = (float)(grblBufferSize - grblBufferFree) * 100 / (float)grblBufferSize;
            if (codeFinish > 100) { codeFinish = 100; }
            if (buffFinish > 100) { buffFinish = 100; }
            OnRaiseStreamEvent(new StreamEventArgs((int)lineNr, codeFinish, buffFinish, status));
        }

        private void grblStateChanged()
        {
            if ((sendLinesConfirmed == sendLinesCount) && (grblStateNow == grblState.idle))   // addToLog(">> Buffer empty\r");
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
            string cmd = cBCommand.Text;
            cBCommand.Items.Remove(cBCommand.SelectedItem);
            cBCommand.Items.Insert(0, cmd);
            requestSend(cmd);
            cBCommand.Text = cmd;
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            string cmd = cBCommand.Text;
            cBCommand.Items.Remove(cBCommand.SelectedItem);
            cBCommand.Items.Insert(0, cmd);
            requestSend(cmd);
            cBCommand.Text = cmd;
        }
        private void btnGRBLCommand0_Click(object sender, EventArgs e)
        { requestSend("$"); }
        private void btnGRBLCommand1_Click(object sender, EventArgs e)
        { requestSend("$$"); }
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
