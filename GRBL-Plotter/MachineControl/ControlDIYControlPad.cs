using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class ControlDIYControlPad : Form
    {
        private string rxString;
        private bool isDataProcessing = false;      // false when no data processing pending

        public ControlDIYControlPad()
        {
            InitializeComponent();
        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
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
        {
            try
            {
                serialPort.PortName = cbPort.Text;
                serialPort.BaudRate = Convert.ToInt32(cbBaud.Text);
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

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {   while ((serialPort.IsOpen) && (serialPort.BytesToRead > 0))
            {   rxString = string.Empty;
                try
                {   rxString = serialPort.ReadTo("\r\n");              //read line from grbl, discard CR LF
                    this.Invoke(new EventHandler(handleRxData));        //tigger rx process 
                    while ((serialPort.IsOpen) && (isDataProcessing)) ;  //wait previous data line processed done
                }
                catch (Exception errort)
                {   serialPort.Close();
                    logError("Error reading line from serial port", errort);
                }
            }
        }

        private void handleRxData(object sender, EventArgs e)
        {
            rtbLog.AppendText(string.Format("< {0} \r\n", rxString));
            OnRaiseCommandEvent(new CommandEventArgs(rxString));
            isDataProcessing = false;
        }

        public void sendFeedback(string data)
        { sendLine(data); }

        /// <summary>
        /// sendLine - now really send data to Arduino
        /// </summary>
        private void sendLine(string data)
        {
            try
            {
                serialPort.Write(data + "\r");
                if (cBFeedback.Checked)
                    rtbLog.AppendText(string.Format("> {0} \r\n", data));
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

            rtbLog.Select(rtbLog.TextLength, 0);
            rtbLog.ScrollToCaret();
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
        private string command;
        public CommandEventArgs(string cmd)
        {   command = cmd;
        }
        public string Command
        { get { return command; } }
    }

}
