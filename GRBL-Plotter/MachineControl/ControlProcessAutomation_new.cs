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
 * 2023-12-01 l:415 f:Feedback add "Probing"
*/

using FastColoredTextBoxNS;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GrblPlotter
{
    public partial class ProcessAutomation : Form
    {
        /*
            Each process contains an action and a value, which will be sent as an event if processed.
            In MainFormOtherForms.cs l:671 f:OnRaiseProcessEvent the asked process will be performed.
            In l:415 f:Feedback the processing result will be analyzed to decide if the next process can be started.
        */
        private int processCount = 0;
        private int processJumpTo = 0;
        private int processStep = 0;
        private string processAction = "";
        //private string processValue = "";
        private bool isRunning = false;
        private bool isGrblNeeded = false;
        private bool stepTriggered = false;
        private bool stepCompleted = false;
        private bool cameraFormOpen = false;
        private bool probeFormOpen = false;

		private int checkDigitalInDigit = 0;
		
        private int dataLine = 0;

        private static List<ProcessAutomationItem> actionItems = new List<ProcessAutomationItem>();
		private ContextMenu ctm = new ContextMenu();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool showLog = false;


		#region FormOpenClose
        public ProcessAutomation()
        {
            Logger.Trace("++++++ ControlProcessAutomation START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            actionItems = GetActionItems();
			foreach(ProcessAutomationItem item in actionItems)
				ctm.MenuItems.Add(item.ActionID);
				
            InitializeComponent();
        }

        private void ControlProcessAutomation_Load(object sender, EventArgs e)
        {
            SendProcessEvent(new ProcessEventArgs("CheckForm", "Cam"));
            SendProcessEvent(new ProcessEventArgs("CheckForm", "Probe"));

            string file = Properties.Settings.Default.processDataLastFile;
            if (File.Exists(file))
            {
                if (file.Length > 40)
                    LblDataLoaded.Text = "..." + file.Substring(file.Length - 40);
                else
                    LblDataLoaded.Text = file;
                FctbData.OpenFile(file);
                textBox1.Text = File.ReadAllText(file);
            }

            //    BtnStart.Enabled = false;
            file = Properties.Settings.Default.processLastFile;
            if (string.IsNullOrEmpty(file))
                file = Datapath.Automations + "\\" + "example.xml";
            if (File.Exists(file))
            {
                if (file.Length > 60)
                    LblLoaded.Text = "..." + file.Substring(file.Length - 60);
                else
                    LblLoaded.Text = file;
                Properties.Settings.Default.processLastFile = file;
                LoadXML(file);
                ControlProcessAutomation_SizeChanged(sender, e);
            }
            else
            {
                splitContainer1.SplitterDistance = 200;
                ControlProcessAutomation_SizeChanged(sender, e);
            }

            LblCounterResult.Text = GetCounterString();
            int nr = (int)NudDataIndex.Value - 1;
            SetTextSelection(nr, nr);
        }

        private void ControlProcessAutomation_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

		#endregion
		
		#region FormButtons
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                if (CheckData())
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                        SetDGVToolTip(i, "");
                    }
                    TbProcessInfo.Text = "Start process automation:\r\n";
                    processStep = 0;
                    isRunning = true;
                    isGrblNeeded = false;
                    stepTriggered = false;
                    stepCompleted = false;
                    timer1.Interval = (int)NudTimerInterval.Value;
                    timer1.Enabled = true;
                    LblCount.Text = "1";

                    LblInfo.Text = "Script is running";
                    LblInfo.BackColor = Color.Yellow;
                    BtnStart.BackColor = Color.Yellow;
                    Logger.Info("+++ Start automation +++++ OnRaiseProcessEvent + Feedback +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                }
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            SendProcessEvent(new ProcessEventArgs("CheckForm", "Cam"));
            SendProcessEvent(new ProcessEventArgs("CheckForm", "Probe"));
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

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                Filter = "Script|*.xml"
            };
            if (File.Exists(Properties.Settings.Default.processLastFile))
                sfd.InitialDirectory = Properties.Settings.Default.processLastFile;
            else
                sfd.InitialDirectory = Datapath.Automations + "\\";//    Application.StartupPath + Datapath.Examples;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.processLastFile = LblLoaded.Text = sfd.FileName;
                LoadXML(sfd.FileName);
            }
            sfd.Dispose();
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Script|*.xml"
            };
            if (File.Exists(Properties.Settings.Default.processLastFile))
                sfd.InitialDirectory = Properties.Settings.Default.processLastFile;
            else
                sfd.InitialDirectory = Datapath.Automations + "\\";//    Application.StartupPath + Datapath.Examples;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.processLastFile = LblLoaded.Text = sfd.FileName;
                SaveXML(sfd.FileName);
            }
            sfd.Dispose();
        }


        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string url = "https://grbl-plotter.de/index.php?";
            try
            {
                System.Windows.Forms.Button clickedLink = sender as System.Windows.Forms.Button;
                Process.Start(url + clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "BtnHelp_Click ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }

        private void BtnDataIndexClear_Click(object sender, EventArgs e)
        {
            NudDataIndex.Value = 1;
            CheckData();
        }


		#endregion

		/*************** Check *******************************/
        private bool CheckData()
        {
            Logger.Trace("🛠🛠🛠 CheckData  called by:{0}",(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            TbProcessInfo.Text = "CHECK DATA\r\n";
            combo = null;
            string tmp;
            string action;
            string value;
            int lineNr;
            bool ok, finalOK = true;
            if (dataGridView1.Rows.Count <= 0)
            {
                TbProcessInfo.Text += "Too less rows\r\n";
                return false;
            }

            if (dataGridView1.Columns.Count < 3)
                TbProcessInfo.Text += "Too less columns\r\n";

        //    dataGridView1.Rows[0].Cells[0].Selected = true;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                lineNr = i + 1;
                action = (string)dataGridView1.Rows[i].Cells[0].Value;
                if (action == null)
                    break;

                if (string.IsNullOrEmpty(action))
                {
                    TbProcessInfo.Text += string.Format("{0}) action=null\r\n", lineNr);
                    return false;
                    break;
                }

                if (dataGridView1.Rows[i].Cells.Count < 2)
                {
                    MessageBox.Show("At least 2 columns are needed, please check your XML file.", "Error");
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
                        tmp = string.Format("{0}) Load - File not found:'{1}'", lineNr, mypath);
                        TbProcessInfo.Text += tmp+ "\r\n"; ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "File not found");
                        Logger.Trace("CheckData {0}", tmp);
                    }
                    else
                    {
                        TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                else if (action.Contains("G-Code"))
                {
                    if (!Grbl.isConnected)
                    {
                        tmp = string.Format("{0}) G-Code - grbl is not connected", lineNr);
                        TbProcessInfo.Text += tmp+ "\r\n"; ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "grbl is not connected");
                        Logger.Trace("CheckData {0}", tmp);
                    }
                    else
                    {
                        TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                else if (action.Contains("Probe Automatic"))
                {
                    if (!probeFormOpen)
                    {
                        tmp = string.Format("{0}) Probe - Probing form is not open", lineNr);
                        TbProcessInfo.Text += tmp + "\r\n"; ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "Probing form is not open");
                        Logger.Trace("CheckData {0}", tmp);
                    }
                    else
                    {
                        TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                else if (action.Contains("Camera Automatic"))
                {
                    if (!cameraFormOpen)
                    {
                        tmp = string.Format("{0}) Fiducial - Camera form is not open", lineNr);
                        TbProcessInfo.Text += tmp + "\r\n"; ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "Camera form is not open");
                        Logger.Trace("CheckData {0}", tmp);
                    }
                    else
                    {
                        TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }

                else if (action.Contains("Jump"))
                {
                    char delimeter = ';';
                    if (value.Contains(","))
                        delimeter = ',';
                    string[] vals = value.Split(delimeter);
                    processCount = 0;

                    ok = int.TryParse(vals[0], out int line);
                    if (!ok || (line <= 1) || (line >= i))
                    {
                        tmp = string.Format("{0}) Jump to line '{1}' doesn't work", lineNr, line);
                        TbProcessInfo.Text += tmp + "\r\n"; ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        SetDGVToolTip(i, "jump target nok");
                        Logger.Trace("CheckData {0}", tmp);
                    }
                    else
                    { processJumpTo = line - 1; SetDGVToolTip(i, "Line is ok"); }

                    if (ok && (vals.Length > 1))
                    {
                        ok = int.TryParse(vals[1], out int rep);
                        if (!ok || (rep < 0))
                        {
                            tmp = string.Format("{0}) Jump to line '{1}', repitition '{2}' doesn't work", lineNr, line, rep);
                            TbProcessInfo.Text += tmp + "\r\n"; ok = false;
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                            SetDGVToolTip(i, "repitition nok");
                            Logger.Trace("CheckData {0}", tmp);
                        }
                        else
                        {
                            processCount = rep;
                            LblCount.Text = processCount.ToString();
                        }
                    }
                    if (ok)
                    {
                        TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok\r\n", lineNr, action, value);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                        if (finalOK)
                            dataGridView1.Rows[line].DefaultCellStyle.BackColor = Color.Green;
                    }
                }

                else if (action.Contains(" Data"))
                {
                    if ((int)NudDataIndex.Value > textBox1.Lines.Length)// FctbData.Lines.Count)
                    {
                        tmp = string.Format("{0}) Data index is too high, reset index!   <----------------", lineNr);
                        TbProcessInfo.Text += tmp + "\r\n"; ok = false;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Fuchsia;
                        GbData.BackColor = Color.Fuchsia;
                        BtnDataIndexClear.BackColor = Color.Yellow;
                        SetDGVToolTip(i, "Reset data index");
                        Logger.Trace("CheckData {0}", tmp);
                    }
                    else
                    {
                        dataLine = (int)NudDataIndex.Value - 1;
                        char delimiter = 'e';// (char)comboBox1.Text[0]; //TbDataDelimeter.Text[0];
                        if (comboBox1.Text.Contains("\t"))
                            delimiter = '\t';
                        Logger.Trace("Delimeter '{0}' '{1}'", comboBox1.SelectedText, delimiter);

                        string dataText = "";
                        if (dataLine < textBox1.Lines.Length)
                            dataText = GetDataText(textBox1.Lines[dataLine], value, delimiter);

                        BtnDataIndexClear.BackColor = GbData.BackColor = default;
                        TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok  '{3}'\r\n", lineNr, action, value, dataText);
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }
                else if (action.Contains(" Counter"))
                {
                    TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok  '{3}'\r\n", lineNr, action, value, GetCounterString());
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else
                {
                    TbProcessInfo.Text += string.Format("{0}) {1,-20}  {2,-10}  ok\r\n", lineNr, action, value);
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }

                finalOK = finalOK && ok;
                if (finalOK)
                {
                    LblInfo.Text = "Ready to start";
                    BtnStart.BackColor = LblInfo.BackColor = Color.Lime;
                    BtnStart.Enabled = true;
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

		/*************** State machine / flow control *******************************/
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!isGrblNeeded || (Grbl.Status == GrblState.idle))
            {
            //    string action = (string)dataGridView1.Rows[processStep].Cells[0].Value;
            //    string value = (string)dataGridView1.Rows[processStep].Cells[1].Value;
                if (!stepTriggered)
                {
                    if (showLog) Logger.Trace("Timer1_Tick step:{0}  rows:{1}", processStep, dataGridView1.Rows.Count);
                    if (processStep < dataGridView1.Rows.Count)
                    {
                        int lineNr = processStep + 1;
                        string action = (string)dataGridView1.Rows[processStep].Cells[0].Value;
                        if (string.IsNullOrEmpty(action))
                        {
                            stepTriggered = false;
                            processStep++;
                            return;
                        }
                        string value = (string)dataGridView1.Rows[processStep].Cells[1].Value;

                        string nextAction = (string)dataGridView1.Rows[processStep + 1].Cells[0].Value;
                        if (!string.IsNullOrEmpty(nextAction))
                        {
                            if (nextAction.Contains("G-Code") || nextAction.Contains("Probe") || nextAction.Contains("Fiducial") ||
                                nextAction.Contains("Stream"))
                                isGrblNeeded = true;
                        }

                        if (showLog) Logger.Trace("Timer1_Tick line:{0}  action:{1}  value:{2}", lineNr, action, value);
                        dataGridView1.Rows[processStep].Cells[0].Selected = true;

                        stepTriggered = true;
                        stepCompleted = false;
                        processAction = action;
                        //processValue = value;

                        if (showLog) Logger.Trace("Timer line:{0})  {1}  {2}  count:{3}", (processStep + 1), action, value, processCount);
                        TbProcessInfo.AppendText(string.Format("{0}) {1,-20}  {2,-10}   ", lineNr, action, value));

                         if (action.Contains("G-Code Send"))
                        {
                            SendProcessEvent(new ProcessEventArgs(action, value));
                            dataGridView1.Rows[processStep].DefaultCellStyle.BackColor = Color.LightGreen;
                            stepCompleted = true;       /* check for IDLE should be enough */
                            TbProcessInfo.AppendText("OK\r\n");
                        }

                        else if (action.Contains("Create") && action.Contains(" Data"))		// Text or barcode
                        {
                            dataLine = (int)NudDataIndex.Value - 1;
                            if (dataLine < textBox1.Lines.Length)// FctbData.Lines.Count())
                            {
                                char delimiter = TbDataDelimiter.Text[0];
                                //string dataText = GetDataText(FctbData.Lines[dataLine], value, delimiter);
                                string dataText = GetDataText(textBox1.Lines[dataLine], value, delimiter);

                                SendProcessEvent(new ProcessEventArgs(action, dataText));
                            }
                            else
                            {
                                Logger.Warn("Data index nok: {0} {1}  count:{2}", dataLine, value, textBox1.Lines.Length);// FctbData.Lines.Count);
                                Feedback(action, "End of data reached", false);
                            }
                        }
                        else if (action.Contains("Create") && action.Contains(" Counter"))		// Text or barcode
                        {
                            SendProcessEvent(new ProcessEventArgs(action, GetCounterString()));
                        }
                        else if (action == "Data index")		// change index counter
                        {
                            int col;
                            if (int.TryParse(value, out col))
                            {
                                dataLine = (int)NudDataIndex.Value;
                                dataLine += col;
                                if (dataLine < 0)
                                    dataLine = 0;
                                NudDataIndex.Value = dataLine;
                                Feedback(action, "New index: " + dataLine, true);
                            }
                            else
                            {
                                Feedback(action, "New index failed", false);
                            }
                        }
                        else if (action == "Counter index")		// change counter
                        {
                            int col;
                            if (int.TryParse(value, out col))
                            {
                                NudCounter.Value += col;
                                Feedback(action, "New counter: " + NudCounter.Value, true);
                            }
                            else
                            {
                                Feedback(action, "New counter failed", false);
                            }
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

                            TbProcessInfo.AppendText(string.Format("  count:{0}  jump to:{1}  OK\r\n", processCount, processStep + 1));
                            LblCount.Text = Math.Abs(processCount).ToString();

                            stepTriggered = false;
                            stepCompleted = true;
                            return;
                        }
                        else if (action.Contains("Wait for DI=1"))
						{	
							if (int.TryParse(value, out int nr))
								checkDigitalInDigit = nr;
							else
								Logger.Error("Timer1_Tick int.TryParse failed action:{0} value:{1}", action, value);
						}
                        else
                        {	// default Load, Probe, Camera, G-Code Stream
                            if (showLog) Logger.Trace("Timer1_Tick SendProcessEvent  {0}  {1}", action, value);
                            SendProcessEvent(new ProcessEventArgs(action, value));
                        }

                    }
                    else  /* flow finished - STOP*/
                    {
                        processStep = 0;
                        isRunning = false;
                        stepCompleted = false;
                        timer1.Enabled = false;
                        Logger.Trace("Timer1_Tick Timer finish Script finished");
                        BtnStart.BackColor = LblInfo.BackColor = Color.Lime;
                        LblInfo.Text = "Script finished";
                        TbProcessInfo.Text += "Finished";

                    }
                }
                else if (stepCompleted)
                {
                    stepTriggered = false;
                    processStep++;
                }
				else
				{	// poll for events, which can't give feedback
				/*	if (action.Contains("Wait for DI"))
					{
						if (action.Contains("=1"))
						{
							if ((Grbl.grblDigitalIn & (1<<checkDigitalInDigit)) > 0)
							{
								stepTriggered = false;
								stepCompleted = false;
								processStep++;							
							}
						}
						else if (action.Contains("=0"))
						{
							if ((Grbl.grblDigitalIn & (1<<checkDigitalInDigit)) == 0)
							{
								stepTriggered = false;
								stepCompleted = false;
								processStep++;							
							}
						}
						Logger.Trace("Timer1 poll digital in action:{0}  value:{1}  grbl:{2}  result:{3}", action, value, Grbl.grblDigitalIn, stepTriggered);
					}	*/				
				}
            }
        }

        private string GetDataText(string all, string strCol, char delimiter)
        {
            int dataCol = -1;
            if (int.TryParse(strCol, out dataCol))
            { }
            if (dataCol < 0)
                return all;
            string[] txtcol = all.Split(delimiter);
            if (dataCol + 1 <= txtcol.Length)
                return txtcol[dataCol];
            return txtcol[txtcol.Length - 1];
        }

        public event EventHandler<ProcessEventArgs> RaiseProcessEvent;
        protected virtual void SendProcessEvent(ProcessEventArgs e)     // event processed in MainFormOtherForms
        {
            RaiseProcessEvent?.Invoke(this, e);
        }

        public void Feedback(string action, string value, bool resultOk)
        {
            Logger.Trace("🠈🠈🠈🠈 Feedback  action:'{0}'  value:'{1}'  ok:{2}", action, value, resultOk);
            if (action == "CheckForm")
            {
                processAction = resultOk.ToString();
                if (value == "Cam") { cameraFormOpen = resultOk; }
                if (value == "Probe") { probeFormOpen = resultOk; }
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
                            TbProcessInfo.AppendText(string.Format("NOK  {0,-20}  {1}  \r\n", action, value));
                        }
                        else
                        {
                            dataGridView1.Rows[processStep].DefaultCellStyle.BackColor = Color.LightGreen;
                            stepCompleted = true;
                            TbProcessInfo.AppendText(string.Format(" {0,-20} OK\r\n", value));
                        }
                    }
                    else
                    {
                        Logger.Warn("Feedback failed processStep:{0} ", processStep);
                        BtnStop.PerformClick();
                        LblInfo.Text = value;
                    }
                }
            }

            if (!isRunning)
            {
                bool ok = CheckData();
                if (ok) { BtnStart.BackColor = Color.Lime; }
                else { BtnStart.BackColor = Color.Fuchsia; }
            }
        }

        #region FormResize
        private void ControlProcessAutomation_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Width = splitContainer1.Width - 325;
            dataGridView1.Height = splitContainer1.SplitterDistance - 184;
            FctbData.Height = splitContainer1.SplitterDistance - 184;
            textBox1.Height = splitContainer1.SplitterDistance - 184;

            int dataX = splitContainer1.Width - 330;
            FctbData.Left = dataX;
            textBox1.Left = dataX;
            GbCounter.Left = dataX;
            GbData.Left = dataX;

            FctbData.Invalidate();

            SetDataGridColumnWidth();
        }
        private void SetDataGridColumnWidth()
        {
            if (dataGridView1.Columns.Count > 2)
            {
                int part = dataGridView1.Width - (dataGridView1.Columns[0].Width + dataGridView1.Columns[1].Width);
                dataGridView1.Columns[2].Width = part;
            }	
 /*           int part = (dataGridView1.Width - 10) / 10;
            if ((dataGridView1.Rows.Count > 0) && (dataGridView1.Columns.Count > 2) && (Width > 300))
            {
                DataGridViewColumn column = dataGridView1.Columns[0]; column.Width = 3 * part;
                column = dataGridView1.Columns[1]; column.Width = 3 * part;
                column = dataGridView1.Columns[2]; column.Width = 3 * part;
            }*/
        }

        private void SplitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Height = splitContainer1.SplitterDistance - 184;
            FctbData.Height = splitContainer1.SplitterDistance - 184;
            textBox1.Height = splitContainer1.SplitterDistance - 184;
        }

        #endregion

        private string GetCounterString()
        {
            string count = string.Format("{0}", NudCounter.Value);
            count = count.PadLeft((int)NudDigitis.Value, TbFill.Text[0]);
            string tmp = string.Format("{0}{1}{2}", TbCounterFront.Text, count, TbCounterRear.Text);
            return tmp;
        }

        private void TbCounter_TextChanged(object sender, EventArgs e)
        {
            LblCounterResult.Text = GetCounterString();
        }

        private void BtnLoadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                Filter = "CSV|*.csv"
            };
            if (File.Exists(Properties.Settings.Default.processDataLastFile))
                sfd.InitialDirectory = Properties.Settings.Default.processDataLastFile;
            else
                sfd.InitialDirectory = Datapath.Automations + "\\";//    Application.StartupPath + Datapath.Examples;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.processDataLastFile = LblDataLoaded.Text = sfd.FileName;
                FctbData.OpenFile(sfd.FileName);
                textBox1.Text = File.ReadAllText(sfd.FileName);
            }
            sfd.Dispose();
        }

        private void NudDataIndex_ValueChanged(object sender, EventArgs e)
        {
            int nr = (int)NudDataIndex.Value - 1;
            SetTextSelection(nr, nr);
            if (!isRunning)
                CheckData();
        }
        private void SetTextSelection(int start, int end)	//, bool toggle = true)
        {
            textBox1.SelectionStart=(start);

            bool fail = false;
            if (start < 0) { start = 0; fail = true; }
            if (start >= FctbData.LinesCount) { start = 0; fail = true; }
            if (end < 0) { end = 0; fail = true; }
            if (end >= FctbData.LinesCount) { end = FctbData.LinesCount - 1; fail = true; }

            Place selStart, selEnd;
            selStart.iLine = start;
            selStart.iChar = 0;
            Range mySelection = new Range(FctbData);
            mySelection.Start = selStart;
            selEnd.iLine = end;
            selEnd.iChar = FctbData.Lines[end].Length;
            mySelection.End = selEnd;

            if (fail)
            {
                mySelection.Start = mySelection.End = selStart;
                FctbData.SelectionLength = 0;
            }

            FctbData.Selection = mySelection;
            FctbData.SelectionColor = Color.Yellow;
        }

        private void FctbData_Click(object sender, EventArgs e)
        {
            int line = FctbData.Selection.ToLine;
            NudDataIndex.Value = line + 1;
        }

        /*************************************** XML *******************************************/
		#region XML
        private void LoadXML(string file)
        {
            //   BtnStart.Enabled = false;
            isRunning = false;
            try
            {
                FileStream stream = new FileStream(file, FileMode.Open,
                            FileAccess.Read, FileShare.ReadWrite);
                XmlTextReader xmlFile = new XmlTextReader(stream);

                DataSet ds = new DataSet();
                ds.ReadXml(xmlFile);

                PresetDataGrid();
                //   dataGridView1.DataSource = ds.Tables[0];
                string s1, s2, s3;
                int ci, i = 0;
                bool ok = true;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    s1 = row[0].ToString();
                    s2 = row[1].ToString();
                    s3 = row[2].ToString();
                    ci = GetIndexInAutomationItems(s1);
                    Logger.Trace("Load xml i:{0}  s1:{1}  s2:{2}  s3:{3}  ci:{4}", i, s1, s2, s3, ci);
                    try
                    { dataGridView1.Rows.Add(actionItems[ci].ActionID, s2, s3); }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, " Load XML error set Combobox i:{0}  s1:{1} ", i + 1, s1);
                        LblInfo.Text = string.Format("Line {0} Action item unknown: '{1}'", i, s1);
                        LblInfo.BackColor = Color.Fuchsia;
                        dataGridView1.Rows.Add(null, s2, s3);
                        ok = false;
                    }
                    i++;
                    //   row.mode
                }
                dataGridView1.Invalidate();
                ds.Dispose();

                foreach (DataGridViewColumn Column in dataGridView1.Columns)
                {
                    Column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                if (ok)
                { BtnStart.Enabled = CheckData(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void SaveXML(string file)
        {
            var dt = new DataTable();
            dt.TableName = "ProcessItem";
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                dt.Columns.Add(column.Name);
            }

            bool empty = false;
            object[] cellValues = new object[dataGridView1.Columns.Count];
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (row.Cells[i].Value == null)
                        empty = true;
                    cellValues[i] = row.Cells[i].Value;
                }
                if (!empty)
                    dt.Rows.Add(cellValues);
            }
            DataSet dS = new DataSet();
            dS.Tables.Add(dt);
            dS.WriteXml(file);// File.OpenWrite(file));
            dS.Dispose();
            dt.Dispose();
        }

		#endregion

        private int GetIndexInAutomationItems(string tmp)
        {
            for (int i = 0; i < actionItems.Count; i++)
            {
                if (actionItems[i].ActionID.ToLower().Contains(tmp.ToLower()))
                { return i; }
            }
            return -1;
        }
		
        private void BtnNew_Click(object sender, EventArgs e)
        {
            PresetDataGrid();
            TbProcessInfo.Clear();
            LblLoaded.Text = "new script, not saved";
        }
        private void PresetDataGrid()
        {
            dataGridView1.Columns.Clear();

    /*        DataGridViewComboBoxColumn dgvCmb = new DataGridViewComboBoxColumn();
            dgvCmb.HeaderText = "Action";
            foreach (ProcessAutomationItem pai in actionItems)
                dgvCmb.Items.Add(pai.ActionID);
            dgvCmb.Name = "Action";
            dataGridView1.Columns.Add(dgvCmb);
    */
            dataGridView1.Columns.Add("Action", "Action");
            dataGridView1.Columns.Add("Value", "Value");
            dataGridView1.Columns.Add("Info", "Info");

            dataGridView1.AllowUserToAddRows = false;
            try
            {
                while (dataGridView1.Rows.Count > 1)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                }
            }
            finally
            {
                dataGridView1.AllowUserToAddRows = true;
            }
            SetDataGridColumnWidth();
        }

		private void AddLoadLines(string[] files)
		{
			foreach(string file in files)
			{	AddLoadLine(file);}
		}
		private void AddLoadLine(string file)
		{
			dataGridView1.Rows.Add("Load", file, "by d&d");
		}
/*********************************************************/


        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //if (combo != null)
                {
                    DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[0];
                    if (cb.Value != null)
                    {
                        if (combo != null)
                        {
                         //   if (dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() == "")
                                dataGridView1.Rows[e.RowIndex].Cells[1].Value = actionItems[combo.SelectedIndex].Value;
                         //   if (dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() == "")
                                dataGridView1.Rows[e.RowIndex].Cells[2].Value = actionItems[combo.SelectedIndex].Comment;
                        }
                        dataGridView1.InvalidateRow(e.RowIndex);
                        combo = null;
                    }
                }
            }
            catch (Exception err)
            {
            }
        }

        private void DataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        ComboBox combo = null;
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            combo = e.Control as ComboBox;
        }

/*********************************/
        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int d = row.Index;
                    if ((d >= 0) && (d < dataGridView1.Rows.Count))
                        dataGridView1.Rows.RemoveAt(row.Index);
                    break;
                }
            }
            catch (Exception err)
            {
            }
        }

        private void MoveRowUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dataGridView1;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == 0)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex - 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex - 1].Cells[colIndex].Selected = true;
            }
            catch { }
        }

        private void MoveRowDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dataGridView1;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == totalRows - 1)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex + 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex + 1].Cells[colIndex].Selected = true;
            }
            catch { }
        }
		
		
        public class ProcessAutomationItem
        {
            public string ActionID { get; set; }
            public string Value { get; set; }
            public string Comment { get; set; }
        }
		private static List<ProcessAutomationItem> GetActionItems()
        {
            return new List<ProcessAutomationItem>
            {
                new ProcessAutomationItem {ActionID="Load", 			Value="", 		Comment="Load ini or graphic file, set full file name"},
                new ProcessAutomationItem {ActionID="2D-View Clear", 	Value="", 		Comment="Clear workspace"},
                new ProcessAutomationItem {ActionID="2D-View Offset", 	Value="7;0;0", 	Comment="Set graphic origin [1-9] and offset from value [o;X;Y]"},
                new ProcessAutomationItem {ActionID="2D-View Scale XYX",Value="", 		Comment="Scale XY to X=value"},
                new ProcessAutomationItem {ActionID="2D-View Scale XYY",Value="", 		Comment="Scale XY to Y=value"},
                new ProcessAutomationItem {ActionID="G-Code Send", 		Value="", 		Comment="Send G-Code to machine, start new line with ';'"},
                new ProcessAutomationItem {ActionID="G-Code Stream", 	Value="", 		Comment="Send G-Code from editor to machine"},
                new ProcessAutomationItem {ActionID="Probe Automatic",	Value="start", 	Comment="Start fiducial recogniton in probing window"},
                new ProcessAutomationItem {ActionID="Camera Automatic", Value="start", 	Comment="Start fiducial recogniton in camera window"},
                new ProcessAutomationItem {ActionID="CreateText Text", 		Value="text", 	Comment="Create text from value"},
                new ProcessAutomationItem {ActionID="CreateText Text W",	Value="text", 	Comment="Create text from value, set predefined width ' W' or height ' H'"},
                new ProcessAutomationItem {ActionID="CreateText Counter", 	Value="", 		Comment="Create text from Counter"},
                new ProcessAutomationItem {ActionID="CreateText Data", 		Value="", 		Comment="Create text from Data list, whole line"},
                new ProcessAutomationItem {ActionID="CreateText Data", 		Value="2", 		Comment="Create text from Data list, column (2)"},
                new ProcessAutomationItem {ActionID="CreateBarcode 1D Text", 	Value="text", Comment="Create barcode from value"},
                new ProcessAutomationItem {ActionID="CreateBarcode 1D Counter", Value="", 	Comment="Create barcode from Counter"},
                new ProcessAutomationItem {ActionID="CreateBarcode 1D Data", 	Value="", 	Comment="Create barcode from Data list, whole line"},
                new ProcessAutomationItem {ActionID="CreateBarcode 1D Data", 	Value="2", 	Comment="Create barcode from Data list, column (2)"},
                new ProcessAutomationItem {ActionID="CreateBarcode 2D Text", 	Value="text", Comment="Create barcode from value"},
                new ProcessAutomationItem {ActionID="CreateBarcode 2D Counter", Value="", 	Comment="Create barcode from Counter"},
                new ProcessAutomationItem {ActionID="CreateBarcode 2D Data", 	Value="", 	Comment="Create barcode from Data list, whole line"},
                new ProcessAutomationItem {ActionID="CreateBarcode 2D Data", 	Value="2", 	Comment="Create barcode from Data list, column (2)"},
                new ProcessAutomationItem {ActionID="Jump to", 			Value="2", 		Comment="Jump to line number from value"},
                new ProcessAutomationItem {ActionID="Jump to", 			Value="2;5", 	Comment="Jump to line number (2), repeat (5) times from value"},
                new ProcessAutomationItem {ActionID="Data index", 		Value="1", 		Comment="Add value to Data index value"},
                new ProcessAutomationItem {ActionID="Counter index", 	Value="1", 		Comment="Add value to Counter value"},
                new ProcessAutomationItem {ActionID="Wait for DI=1", 	Value="0", 		Comment="Wait for digital input bit x=1"},
                new ProcessAutomationItem {ActionID="Wait for DI=0", 	Value="0", 		Comment="Wait for digital input bit x=0"}
				
				//Wait for digital in 1-4, pattern only duo x - polling - value from MainFormInterface.cs l:160
            };
        }

        // drag and drop file or URL
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string s = (string)e.Data.GetData(DataFormats.Text);
            if (files != null)
            { AddLoadLines(files); }
            else if (s.Length > 0)
            { AddLoadLine(s); }
            this.WindowState = FormWindowState.Normal;
            CheckData();
        }

		private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				var currentMouseOve = dataGridView1.HitTest(e.X,e.Y);

         /*       if (currentMouseOve.ColumnIndex == 0)
                {
                    dataGridView1.ContextMenuStrip.Enabled = false;
                    ctm.Show(dataGridView1, new Point(e.X, e.Y));
                }

                dataGridView1.ContextMenuStrip.Enabled = true;
         */
			}
		}
    }
}
