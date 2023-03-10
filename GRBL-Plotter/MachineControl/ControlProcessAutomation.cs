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
/* 
 * 2023-01-05 new feature
 * 2023-01-26 line 186 check for 3 columns
 * 2023-02-23 line 422 Feedback check index 
*/

using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GrblPlotter
{
    public partial class ControlProcessAutomation : Form
    {

        private int processCount = 0;
        private int processJumpTo = 0;
        private int processStep = 0;
        private string processAction = "";
        private string processValue = "";
        private bool isRunning = false;
        private bool stepTriggered = false;
        private bool stepCompleted = false;
        private bool cameraFormOpen = false;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;


        public ControlProcessAutomation()
        {
            Logger.Trace("++++++ ControlProcessAutomation START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        private void ControlProcessAutomation_Load(object sender, EventArgs e)
        {
            SendProcessEvent(new ProcessEventArgs("CheckForm", "Cam"));
            textBox1.AppendText(Lblxml.Text);

            BtnStart.Enabled = false;
            string file = Properties.Settings.Default.processLastFile;
            if (string.IsNullOrEmpty(file))
                file = Datapath.Automations + "\\" + "example.xml";
            if (File.Exists(file))
            {
                if (file.Length > 50)
                    LblLoaded.Text = "..." + file.Substring(file.Length - 50);
                else
                    LblLoaded.Text = file;
                Properties.Settings.Default.processLastFile = file;
                LoadXML(file);
                ControlProcessAutomation_SizeChanged(sender, e);
            }
            else
            {
                splitContainer1.SplitterDistance = 120;
                ControlProcessAutomation_SizeChanged(sender, e);
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    SetDGVToolTip(i, "");
                }
                textBox1.Text = "Start process automation:\r\n";
                processStep = 0;
                isRunning = true;
                stepTriggered = false;
                stepCompleted = false;
                timer1.Enabled = true;
                LblCount.Text = "1";

                LblInfo.Text = "Script is running";
                LblInfo.BackColor = Color.Yellow;
                BtnStart.BackColor = Color.Yellow;
                Logger.Trace("+++ Start automation +++");
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            SendProcessEvent(new ProcessEventArgs("CheckForm", "Cam"));
            if (isRunning && !stepCompleted)
            {
                LblInfo.Text = "Wait for finishing line " + (processStep + 1).ToString();
                BtnStart.BackColor = LblInfo.BackColor = Color.Yellow;
            }
            else
            {
                BtnStart.Enabled = CheckData();
            }
            processStep = 0;
            isRunning = false;
            timer1.Enabled = false;
            Logger.Trace("+++ Stop automation +++");
        }


        private void LoadXML(string file)
        {
            BtnStart.Enabled = false;
            isRunning = false;
            try
            {
                XmlReader xmlFile;
                xmlFile = XmlReader.Create(file, new XmlReaderSettings());
                DataSet ds = new DataSet();
                ds.ReadXml(xmlFile);
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = ds.Tables[0];
                //    DataGridViewColumn column = dataGridView1.Columns[0];

                foreach (DataGridViewColumn Column in dataGridView1.Columns)
                {
                    Column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                BtnStart.Enabled = CheckData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private bool CheckData()
        {
            textBox1.Text = "CheckData\r\n";

            string action = "";
            string value = "";
            int lineNr;
            bool ok, finalOK = true;
            if (dataGridView1.Rows.Count <= 0)
            {
                textBox1.Text += "Too less rows\r\n";
                return false;
            }

            if (dataGridView1.Columns.Count < 3)
                textBox1.Text += "Too less columns\r\n";

            dataGridView1.Rows[0].Cells[0].Selected = true;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                lineNr = i + 1;
                action = (string)dataGridView1.Rows[i].Cells[0].Value;
                if (string.IsNullOrEmpty(action))
                {
                    textBox1.Text += string.Format("{0}) action=null\r\n", lineNr);
   					return false;
					break;
                }

                if (dataGridView1.Rows[i].Cells.Count < 3)
                {
                    MessageBox.Show("At least 3 columns are needed, please check your XML file.", "Error");
					return false;
                    break;
                }

                value = (string)dataGridView1.Rows[i].Cells[1].Value;
                ok = true;

                SetDGVToolTip(i, "Line is ok");
                dataGridView1.Rows[i].HeaderCell.Value = String.Format("{0}", i + 1);

                if (action.Contains("Load"))
                {
                    string mypath = Datapath.MakeAbsolutePath(value);
                    if (!File.Exists(mypath))
                    {
                        textBox1.Text += string.Format("{0}) Load - File not found:'{1}'\r\n", lineNr, mypath); ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "File not found");
                    }
                    else
                    {
                        textBox1.Text += string.Format("{0}) {1}  {2}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                else if (action.Contains("G-Code"))
                {
                    if (!Grbl.isConnected)
                    {
                        textBox1.Text += string.Format("{0}) G-Code - grbl is not connected\r\n", lineNr); ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "grbl is not connected");
                    }
                    else
                    {
                        textBox1.Text += string.Format("{0}) {1}  {2}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                else if (action.Contains("Fiducial"))
                {
                    if (!cameraFormOpen)
                    {
                        textBox1.Text += string.Format("{0}) Fiducial - Camera form is not open\r\n", lineNr); ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "Camera form is not open");
                    }
                    else
                    {
                        textBox1.Text += string.Format("{0}) {1}  {2}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                else if (action.Contains("Jump"))
                {
                    string[] vals = value.Split(',');
                    int line, rep;
                    processCount = 0;

                    bool okVal = int.TryParse(vals[0], out line);
                    if (!ok || (line <= 1) || (line >= i))
                    {
                        textBox1.Text += string.Format("{0}) Jump to line '{1}' doesn't work\r\n", lineNr, line); ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "jump target nok");
                    }
                    else
                    { processJumpTo = line - 1; SetDGVToolTip(i, "Line is ok"); }

                    if (ok && (vals.Length > 1))
                    {
                        bool okRep = int.TryParse(vals[1], out rep);
                        if (!ok || (rep < 0))
                        {
                            textBox1.Text += string.Format("{0}) Jump to line '{1}', repitition '{2}' doesn't work\r\n", lineNr, line, rep); ok = false;
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                            SetDGVToolTip(i, "repitition nok");
                        }
                        else
                        {
                            processCount = rep;
                            LblCount.Text = processCount.ToString();
                        }
                    }
                    if (ok)
                    {
                        textBox1.Text += string.Format("{0}) {1}  {2}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                        if (finalOK)
                            dataGridView1.Rows[line].DefaultCellStyle.BackColor = Color.Green;
                    }
                }

                else
                {
                    textBox1.Text += string.Format("{0}) {1}  {2}  ok\r\n", lineNr, action, value);
                    //    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                }

                finalOK = finalOK && ok;
                if (finalOK)
                {
                    LblInfo.Text = "Ready to start";
                    BtnStart.BackColor = LblInfo.BackColor = Color.Lime;
                }
                else
                {
                    LblInfo.Text = "Not ready";
                    BtnStart.BackColor = LblInfo.BackColor = Color.Fuchsia;
                }
            }
            return finalOK;
        }

        private void SetDGVToolTip(int row, string text)
        {
            if (row < dataGridView1.Rows.Count)
                for (int i = 0; i < dataGridView1.Rows[row].Cells.Count; i++)
                {
                    dataGridView1.Rows[row].Cells[i].ToolTipText = text;
                }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Grbl.Status == GrblState.idle)
            {
                if (!stepTriggered)
                {
                    if (processStep < dataGridView1.Rows.Count)
                    {
                        int lineNr = processStep + 1;
                        string action = (string)dataGridView1.Rows[processStep].Cells[0].Value;
                        if (string.IsNullOrEmpty(action))
                        {
                        }
                        string value = (string)dataGridView1.Rows[processStep].Cells[1].Value;

                        dataGridView1.Rows[processStep].Cells[0].Selected = true;

                        stepTriggered = true;
                        stepCompleted = false;
                        processAction = action;
                        processValue = value;

                        Logger.Trace("Timer line:{0})  {1}  {2}  count:{3}", (processStep + 1), action, value, processCount);
                        textBox1.AppendText(string.Format("{0})  {1}  {2}   ", lineNr, action, value));

                        if (action.Contains("Load"))
                        {
                            SendProcessEvent(new ProcessEventArgs(action, value));
                        }
                        else if (action.Contains("G-Code"))
                        {
                            SendProcessEvent(new ProcessEventArgs(action, value));
                            dataGridView1.Rows[processStep].DefaultCellStyle.BackColor = Color.LightGreen;
                            stepCompleted = true;       /* check for IDLE should be enough */
                            textBox1.AppendText("OK\r\n");
                        }
                        else if (action.Contains("Fiducial"))
                        {
                            SendProcessEvent(new ProcessEventArgs(action, value));
                        }
                        else if (action.Contains("Stream"))
                        {
                            SendProcessEvent(new ProcessEventArgs(action, value));
                        }
                        else if (action.Contains("Jump"))
                        {
                            processCount--;
                            if ((processCount > 0) || (processCount < 0))
                            {
                                for (int k = processJumpTo; k <= processStep; k++)
                                    dataGridView1.Rows[k].DefaultCellStyle.BackColor = Color.White;

                                processStep = processJumpTo;
                                dataGridView1.Rows[processStep].Cells[0].Selected = true;
                            }
                            else
                            {
                                dataGridView1.Rows[processStep].DefaultCellStyle.BackColor = Color.LightGreen;
                                processStep++;
                            }

                            textBox1.AppendText(string.Format("  count:{0}  jump to:{1}  OK\r\n", processCount, processStep + 1));
                            LblCount.Text = Math.Abs(processCount).ToString();

                            stepTriggered = false;
                            stepCompleted = true;
                            return;
                        }
                    }
                    else  /* flow finished - STOP*/
                    {
                        processStep = 0;
                        isRunning = false;
                        stepCompleted = false;
                        timer1.Enabled = false;
                        Logger.Trace("Timer finish");
                    }
                }
                else if (stepCompleted)
                {
                    stepTriggered = false;
                    processStep++;
                }
            }
        }

        public event EventHandler<ProcessEventArgs> RaiseProcessEvent;
        protected virtual void SendProcessEvent(ProcessEventArgs e)
        {
            RaiseProcessEvent?.Invoke(this, e);
        }

        public void Feedback(string action, string value, bool resultOk)
        {
            if (action == "CheckForm")
            {
                processAction = resultOk.ToString();
                if (value == "Cam") { cameraFormOpen = resultOk; }
            }
            else if (action == processAction)
            {
                if (stepTriggered)
                {
					if ((dataGridView1 != null) && (processStep < dataGridView1.Rows.Count))
                    {	
						if (!resultOk)
						{
							LblInfo.BackColor = dataGridView1.Rows[processStep].DefaultCellStyle.BackColor = Color.Fuchsia;
							SetDGVToolTip(processStep, value);
							textBox1.AppendText(string.Format("NOK  {0}  {1}  \r\n", action, value));
						}
						else
						{
							dataGridView1.Rows[processStep].DefaultCellStyle.BackColor = Color.LightGreen;
							stepCompleted = true;
							textBox1.AppendText(string.Format(" {0} OK\r\n", value));
						}
					}
					else
					{	 Logger.Warn("Feedback failed processStep:{0} ", processStep);}
                }
            }
            Logger.Trace("Feedback '{0}'  '{1}'  '{2}'", action, value, processAction);
            if (!isRunning)
            {
                bool ok = BtnStart.Enabled = CheckData();
                if (ok) { BtnStart.BackColor = Color.Lime; }
                else { BtnStart.BackColor = Color.Fuchsia; }
            }
        }

        private void ControlProcessAutomation_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Width = splitContainer1.Width - 10;
            dataGridView1.Height = splitContainer1.SplitterDistance - 110;
            LblLoaded.Width = Width - 200;

            int part = (dataGridView1.Width - 10) / 10;
            if ((dataGridView1.Rows.Count > 0) && (dataGridView1.Columns.Count > 2) && (Width > 300))
            {
                DataGridViewColumn column = dataGridView1.Columns[0]; column.Width = 1 * part;
                column = dataGridView1.Columns[1]; column.Width = 4 * part;
                column = dataGridView1.Columns[2]; column.Width = 4 * part;
            }
        }

        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Height = splitContainer1.SplitterDistance - 110;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                InitialDirectory = Datapath.Automations + "\\",//    Application.StartupPath + Datapath.Examples;
                Filter = "Script|*.xml"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.processLastFile = LblLoaded.Text = sfd.FileName;
                LoadXML(sfd.FileName);
            }
            sfd.Dispose();
        }

        private void ControlProcessAutomation_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }

    public class ProcessAutomationItem
    {
        public int ActionID { get; set; }
        public string Value { get; set; }
    }

}
