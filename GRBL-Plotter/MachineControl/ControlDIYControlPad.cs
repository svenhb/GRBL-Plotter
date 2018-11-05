using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class ControlDIYControlPad : Form
    {
        private string rxString,rxTmpString;
        private byte rxChar;

        public ControlDIYControlPad()
        {   InitializeComponent();
        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {   if (serialPort.IsOpen)
                closePort();
            else
                openPort();
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

        private bool openPort()
        {   try
            {   serialPort.PortName = cbPort.Text;
                serialPort.BaudRate = Convert.ToInt32(cbBaud.Text);
                serialPort.Encoding = Encoding.GetEncoding(28591);
                serialPort.Open();
                rtbLog.Clear();
                rtbLog.AppendText("Open " + cbPort.Text + "\r\n");
                btnOpenPort.Text = "Close";
                return (true);
            }
            catch (Exception err)
            {
                logError("Opening port", err);
                return (false);
            }
        }
        public bool closePort()
        {   try
            {   if (serialPort.IsOpen)
                {   serialPort.Close(); }
                rtbLog.AppendText("Close " + cbPort.Text + "\r\n");
                btnOpenPort.Text = "Open";
                return (true);
            }
            catch (Exception err)
            {   logError("Closing port", err);
                return (false);
            }
        }
        private void logError(string message, Exception err)
        {
            string textmsg = "\r\n[ERROR]: " + message + ". ";
            if (err != null) textmsg += err.Message;
            textmsg += "\r\n";
            rtbLog.AppendText(textmsg);
            rtbLog.ScrollToCaret();
        }

        private static byte lastChar = 0;
        private static List<byte> isRealTimeCmd = new List<byte> { 0x18, (byte)'?', (byte)'~', (byte)'!'};
        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {   while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))
            {
                byte[] rxBuff = new byte[serialPort.BytesToRead];
                serialPort.Read(rxBuff, 0, serialPort.BytesToRead);
                byte rxTmpChar = 0;
                try
                {   foreach (byte rxTmpChart in rxBuff)
                    {
                        rxTmpChar = rxTmpChart;
                        if ((rxTmpChar > 0x7F) || (isRealTimeCmd.Contains(rxTmpChar)))  // is real time cmd ?
                        {   rxChar = rxTmpChar;
                            this.Invoke(new EventHandler(handleRxData));
                        }
                        else
                        {   rxTmpString += (char)rxTmpChar;
                            if ((rxTmpChar == '\r') || (rxTmpChar == '\n'))             // end of regular command
                            {   if (lastChar >= ' ')
                                {
                                    rxChar = 0;
                                    rxString = rxTmpString.Trim();
                                    this.Invoke(new EventHandler(handleRxData));
                                }
                                rxTmpString = "";
                            }
                        }
                        lastChar = rxTmpChar;
                    }
                }
                catch (Exception errort)
                {   serialPort.Close();
                    logError("Error reading line from serial port", errort);
                }
            }
        }

        private void handleRxData(object sender, EventArgs e)
        {
            if ((rxChar > 0x7F) || (isRealTimeCmd.Contains(rxChar)))
            {   rtbLog.AppendText(string.Format("< 0x{0:X} {1}\r\n", rxChar, grbl.getRealtime(rxChar)));
                OnRaiseCommandEvent(new CommandEventArgs(rxChar));
                rxChar = 0;
            }
            else
            {   if (rxString.Length > 2)        // don't send single \n
                {   rtbLog.AppendText(string.Format("< {0} \r\n", rxString));
                    OnRaiseCommandEvent(new CommandEventArgs(rxString));
                }
                rxString = "";
            }
        }

        public void sendFeedback(string data)
        { sendLine(data); }

        /// <summary>
        /// sendLine - now really send data to Arduino
        /// </summary>
        private void sendLine(string data)
        {
            try
            {   if (serialPort.IsOpen)
                {
                    serialPort.Write(data + "\r\n");
                    if (cBFeedback.Checked)
                        rtbLog.AppendText(string.Format("> {0} \r\n", data));
                }
                else {
                    if (cBFeedback.Checked)
                        rtbLog.AppendText(string.Format(">| {0} \r\n", data));
                }

            }
            catch (Exception err)
            {   if (cBFeedback.Checked)
                    rtbLog.AppendText(string.Format(">| {0} \r\n", data));
            }
            while (rtbLog.Lines.Length > 99)
            {
                rtbLog.SelectionStart = 0;
                rtbLog.SelectionLength = rtbLog.Text.IndexOf("\n", 0) + 1;
                rtbLog.SelectedText = "";
            }

            if (cBFeedback.Checked)
            {   rtbLog.Select(rtbLog.TextLength, 0);
                rtbLog.ScrollToCaret();
            }
        }
        public event EventHandler<CommandEventArgs> RaiseStreamEvent;
        protected virtual void OnRaiseCommandEvent(CommandEventArgs e)
        {
            EventHandler<CommandEventArgs> handler = RaiseStreamEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ControlDIYControlPad_Load(object sender, EventArgs e)
        {   cbPort.Text = Properties.Settings.Default.serialPortDIY;
            cbBaud.Text = Properties.Settings.Default.serialBaudDIY;
            openPort();
        }

        private void ControlDIYControlPad_FormClosing(object sender, FormClosingEventArgs e)
        {   Properties.Settings.Default.serialPortDIY = cbPort.Text;
            Properties.Settings.Default.serialBaudDIY = cbBaud.Text;
        }

        private void ControlDIYControlPad_Resize(object sender, EventArgs e)
        {   rtbLog.Width = this.Width - 20;
            rtbLog.Height = this.Height - 70;
        }
    }

    public class CommandEventArgs : EventArgs
    {
        private string cmdString;
        private byte cmdChar;
        public CommandEventArgs(string cmd)
        {   cmdString = cmd;
        }
        public CommandEventArgs(byte cmd)
        {   cmdChar = cmd;
        }
        public string Command
        { get { return cmdString; } }
        public byte RealTimeCommand
        { get { return cmdChar; } }
    }

}
