/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2024-02-25 overhaul the process automation
*/

using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GrblPlotter
{
    public partial class ProcessAutomation : Form
    {
        /*
            Each process contains a command (or action) and a value, which will be sent as an event if processed.
            In MainFormProcessAutomation.cs -OnRaiseProcessEvent the asked process will be performed.
            In Feedback the processing result will be analyzed to decide if the next process can be started.
        */
        private int processCount = 0;
        private int processJumpTo = 0;
        private int processStep = 0;
        private string processAction = "";
        //private string processValue = "";
        private bool isRunning = false;
        private bool isGrblNeeded = false;
        private bool isGrblConnected = false;
        private bool stepTriggered = false;
        private bool stepCompleted = false;
        //   private bool cameraFormOpen = false;
        //   private bool probeFormOpen = false;

        private int checkDigitalInDigit = 0;

        private int dataLine = 0;

        private static List<ProcessAutomationItem> actionItems = new List<ProcessAutomationItem>();
        private readonly ContextMenu ctm = new ContextMenu();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool showLog = false;

        #region command_definition
        public class ProcessAutomationItem
        {
            public string Command { get; set; }
            public string Value { get; set; }
            public string Comment { get; set; }
        }
        private static List<ProcessAutomationItem> GetActionItems()
        {
            return new List<ProcessAutomationItem>
            {
                new ProcessAutomationItem {Command="Load",              Value="",       Comment="Load ini or graphic file, [Value=file name]"},
                new ProcessAutomationItem {Command="Load Data",         Value="",       Comment="Load ini or graphic file, filename from data list [Value='':whole line] or [Value=column-nr]"},
                new ProcessAutomationItem {Command="Load URL",          Value="",       Comment="Open web site [Value=URL]"},
                new ProcessAutomationItem {Command="Paste clipboard",   Value="",       Comment="Paste content from clipboard"},
                new ProcessAutomationItem {Command="2D-View Clear",     Value="",       Comment="Clear workspace, delete G-Code from editor"},
                new ProcessAutomationItem {Command="2D-View Offset",    Value="7;0;0",  Comment="Set graphic origin [Value=<Origin[1-9]>;<Offset X>;<Offset Y>]"},
                new ProcessAutomationItem {Command="2D-View Rotate",    Value="45",     Comment="Rotate [Value=angle in degree]"},
                new ProcessAutomationItem {Command="2D-View Scale XYX", Value="",       Comment="Scale XY [Value=desired X dimension]"},
                new ProcessAutomationItem {Command="2D-View Scale XYY", Value="",       Comment="Scale XY [Value=desired Y dimension]"},
                new ProcessAutomationItem {Command="G-Code Send",       Value="",       Comment="Send G-Code to machine, [Value=g-code commands or macro-file] seperate single command lines with ';'"},
                new ProcessAutomationItem {Command="G-Code Stream",     Value="",       Comment="Send G-Code from editor to machine"},
                new ProcessAutomationItem {Command="Probe Automatic",   Value="start",  Comment="Start fiducial recogniton in probing window"},
                new ProcessAutomationItem {Command="Camera Automatic",  Value="start",  Comment="Start fiducial recogniton in camera window"},
                new ProcessAutomationItem {Command="CreateText Text",   Value="text",   Comment="Create text [Value=text to create]"},
                new ProcessAutomationItem {Command="CreateText Data",   Value="0",      Comment="Create text from Data list, [Value='':whole line] or [Value=column-nr]"},
                new ProcessAutomationItem {Command="CreateText Counter",Value="",       Comment="Create text from Counter"},
                new ProcessAutomationItem {Command="CreateBarcode 1D Text",     Value="text", Comment="Create barcode [Value=text to convert]"},
                new ProcessAutomationItem {Command="CreateBarcode 1D Data",     Value="0",  Comment="Create barcode from Data list, [Value='':whole line] or [Value=column-nr]"},
                new ProcessAutomationItem {Command="CreateBarcode 1D Counter",  Value="",   Comment="Create barcode from Counter"},
                new ProcessAutomationItem {Command="CreateBarcode 2D Text",     Value="text", Comment="Create QR-Code [Value=text to convert]"},
                new ProcessAutomationItem {Command="CreateBarcode 2D URL",      Value="https://grbl-plotter.de/", Comment="Create QR-Code URL [Value=Url]"},
                new ProcessAutomationItem {Command="CreateBarcode 2D Data",     Value="0",  Comment="Create QR-Code from Data list, [Value='':whole line] or [Value=column-nr]"},
                new ProcessAutomationItem {Command="CreateBarcode 2D DURL",     Value="0",  Comment="Create QR-Code URL from Data list, [Value='':whole line] or [Value=column-nr]"},
                new ProcessAutomationItem {Command="CreateBarcode 2D Counter",  Value="",   Comment="Create QR-Code from Counter"},
                new ProcessAutomationItem {Command="Jump to",       Value="2",      Comment="Jump to line number, [Value=<linenumber>;<repetitions>]"},
                new ProcessAutomationItem {Command="Data index",    Value="1",      Comment="Add Value to Data index"},
                new ProcessAutomationItem {Command="Counter index", Value="1",      Comment="Add Value to Counter"},
                new ProcessAutomationItem {Command="Wait Probe",    Value="",       Comment="Wait for probe input before continue"},
                new ProcessAutomationItem {Command="Wait DI=1",     Value="0",      Comment="Wait for digital input [Value=bit-nr] x=1"},
                new ProcessAutomationItem {Command="Wait DI=0",     Value="0",      Comment="Wait for digital input [Value=bit-nr] x=0"},
                new ProcessAutomationItem {Command="Beep",          Value="440;800",Comment="Play beep tone [Value=freqency(Hz);length(ms)]"},
                new ProcessAutomationItem {Command="Sound",         Value="0",       Comment="Play system sound [Value=0=Asterisk, 1=Exclamation, 2=Question, 3=Hand, any other=Beep]"},
                new ProcessAutomationItem {Command="Unknown",       Value="⚠⚠⚠",Comment="Please select valid command"}
            };
        }
        #endregion

        #region FormOpenClose
        public ProcessAutomation()
        {
            Logger.Trace("++++++ ControlProcessAutomation START X:{0}  Y:{1} ++++++", Properties.Settings.Default.processLocation.X, Properties.Settings.Default.processLocation.Y);
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            actionItems = GetActionItems();
            foreach (ProcessAutomationItem item in actionItems)
                ctm.MenuItems.Add(item.Command);

            InitializeComponent();
        }

        private void ControlProcessAutomation_Load(object sender, EventArgs e)
        {
            this.Size = Properties.Settings.Default.processSize;
            splitContainer1.SplitterDistance = Properties.Settings.Default.processSplitDistance;

            if (this.Location.X < 0) this.Location = new Point();

            SendProcessEvent(new ProcessEventArgs("CheckForm", "Cam"));
            SendProcessEvent(new ProcessEventArgs("CheckForm", "Probe"));

            UpdateIniVariables();
        }

        private void ControlProcessAutomation_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.processSplitDistance = splitContainer1.SplitterDistance;
            Properties.Settings.Default.processSize = Size;
            Properties.Settings.Default.Save();
        }

        public void UpdateIniVariables()
        {
            if (isRunning)
                return;

            string file = ExtendFilePath(Properties.Settings.Default.processDataLastFile);
            Logger.Trace("UpdateIniVariables Data:{0}", file);
            if (File.Exists(file))
            {
                LblDataLoaded.Text = "..." + CutString(file, 38);
                TbData.Text = File.ReadAllText(file);
            }

            file = ExtendFilePath(Properties.Settings.Default.processLastFile);
            Logger.Trace("UpdateIniVariables XML:{0}", file);
            if (File.Exists(file))
            {
                LblLoaded.Text = "..." + CutString(file, 60);
                Properties.Settings.Default.processLastFile = file;
                LoadXML(file);
                SizeChange();
            }
            else
            {
                splitContainer1.SplitterDistance = 200;
                BtnNew.PerformClick();
                SizeChange();
            }

            LblCounterResult.Text = GetCounterString();
            int line = (int)NudDataIndex.Value - 1;
            SetTextSelection(line, -1, ';');
        }
        private string ExtendFilePath(string file)
        {
            if (string.IsNullOrEmpty(file))
                return file;

            if (File.Exists(file))
                return file;

            string tmp = Path.Combine(Datapath.AppDataFolder, file);
            if (File.Exists(tmp))
                return tmp;

            tmp = Path.Combine(Datapath.Automations, file);
            if (File.Exists(tmp))
                return tmp;

            Logger.Error("extendFilePath could not find {0}", file);
            return file;
        }

        private string CutString(string text, int length)
        {
            if (text.Length > length)
                return text.Substring(text.Length - length);
            else
                return text;
        }
        #endregion

        #region FormButtons
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[2].Style.BackColor = Color.White;
                    SetDGVToolTip(i, "");
                }
                BtnDataIndexClear.BackColor = GbData.BackColor = default;
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
                BtnStep.Visible = false;
                LblInfo.BackColor = Color.Yellow;
                BtnStart.BackColor = Color.Yellow;
                Logger.Info("+++ Start automation +++++ OnRaiseProcessEvent + Feedback +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
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
                BtnStart.BackColor = Color.Lime;
                CheckData();
            }
            processStep = 0;
            isRunning = false;
            timer1.Enabled = false;
            BtnStep.Visible = true;
            Logger.Trace("+++ Stop automation +++");
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                Filter = "Script|*.xml;*.ini"
            };
            if (File.Exists(Properties.Settings.Default.processLastFile))
                sfd.InitialDirectory = Properties.Settings.Default.processLastFile;
            else
                sfd.InitialDirectory = Datapath.Automations + "\\";//    Application.StartupPath + Datapath.Examples;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string file = sfd.FileName;
                string extension = Path.GetExtension(file);
                if (extension.ToLower().Contains("ini"))
                {
                    string allText = File.ReadAllText(file);
                    if (allText.Contains(IniFile.sectionProcAuto))
                    {
                        var MyIni = new IniFile(file);
                        MyIni.ReadSection(IniFile.sectionProcAuto);
                        Logger.Info("BtnLoad_Click load INI '{0}'", file);
                        UpdateIniVariables();
                    }
                }
                else
                {
                    Properties.Settings.Default.processLastFile = file;
                    LblLoaded.Text = "..." + CutString(file, 60);
                    LoadXML(sfd.FileName);
                }
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
                Properties.Settings.Default.processLastFile = sfd.FileName;
                LblLoaded.Text = "..." + CutString(sfd.FileName, 60);
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
        private void BtnSaveIni_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Machine Ini files (*.ini)|*.ini",
                    FileName = "ProcessAutomation_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ini"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var MyIni = new IniFile(sfd.FileName);
                    /*** Set section to save ***/
                    Logger.Info("Save machine parameters as {0}", sfd.FileName);
                    MyIni.WriteSection(IniFile.sectionProcAuto);	//"Create Text");    
                }
                sfd.Dispose();
            }
            catch (Exception err)
            {
                EventCollector.StoreException("BtnSaveIni_Click " + err.Message);
                Logger.Error(err, "BtnSaveIni_Click ");
                MessageBox.Show("SaveMachineParameters: \r\n" + err.Message, "Error");
            }
        }

        private void BtnDataIndexClear_Click(object sender, EventArgs e)
        {
            NudDataIndex.Value = 1;
            CheckData();
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

        #endregion

        #region script_check
        /*************** Check *******************************/
        private bool CheckData()
        {
            Logger.Trace("🛠🛠🛠 CheckData  called by:{0}", (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            TbProcessInfo.Text = "CHECK DATA\r\n";
            BtnDataIndexClear.BackColor = GbData.BackColor = default;
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
            {
                TbProcessInfo.Text += "Too less columns\r\n";
                return false;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                lineNr = i + 1;
                action = (string)dataGridView1.Rows[i].Cells[0].Value;
                if (action == null)
                    break;

                if (string.IsNullOrEmpty(action))
                {
                    TbProcessInfo.Text += string.Format("{0,-2}) action=null\r\n", lineNr);
                    return false;
                    break;
                }

                value = (string)dataGridView1.Rows[i].Cells[1].Value;
                ok = true;

                SetDGVToolTip(i, "Line is ok");
                dataGridView1.Rows[i].HeaderCell.Value = String.Format("{0}", i + 1);

                if (action.Contains("Unknown"))
                {
                    tmp = string.Format("{0}) Unkown command", lineNr);
                    TbProcessInfo.Text += tmp + "\r\n";
                    ok = false;
                    SetCellColor(i, false);
                    SetDGVToolTip(i, "Please select regular command");
                    Logger.Trace("CheckData NOK {0}", tmp);
                }

                else if (action.Contains("Load"))
                {
                    string mypath = ExtendFilePath(value);
                    if (action.Contains("Data"))
                    {
                        if ((int)NudDataIndex.Value > TbData.Lines.Length)// FctbData.Lines.Count)
                        {
                            tmp = string.Format("{0}) Data index is too high, reset index!   <----------------", lineNr);
                            TbProcessInfo.Text += tmp + "\r\n";
                            ok = false;
                            SetCellColor(i, false);
                            GbData.BackColor = Color.Fuchsia;
                            BtnDataIndexClear.BackColor = Color.Yellow;
                            Logger.Trace("CheckData NOK {0}", tmp);
                            SetDGVToolTip(i, "Reset data index");
                            LblInfo.Text = "Data index too high";
                        }
                        else
                        {
                            /* set dataLine with file-path from data */
                            dataLine = (int)NudDataIndex.Value - 1;
                            char delimiter = (char)ComboDelimiter.Text[0]; //TbDataDelimeter.Text[0];
                            if (ComboDelimiter.Text.Contains("tab"))
                                delimiter = '\t';
                            Logger.Trace("Delimeter '{0}' '{1}'", ComboDelimiter.Text, delimiter);

                            string dataText = "";
                            if (dataLine < TbData.Lines.Length)//FctbData.Lines.Count)
                            {
                                dataText = GetDataText(dataLine, value, delimiter);   //delimiter);
                            }
                            //BtnDataIndexClear.BackColor = GbData.BackColor = default;
                            //TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok  '{3}'\r\n", lineNr, action, value, dataText);
                            //SetCellColor(i, true);

                            mypath = dataText;

                            /*    dataLine = (int)NudDataIndex.Value - 1;
                                if (dataLine < TbData.Lines.Length)
                                {
                                    mypath = extendFilePath(GetDataText(dataLine, "", ' '));
                                }*/
                        }
                        //}
                        if (!File.Exists(mypath))
                        {
                            tmp = string.Format("{0,2}) Load - File not found:'{1}'", lineNr, mypath);
                            TbProcessInfo.Text += tmp + "\r\n";
                            ok = false;
                            SetCellColor(i, false);
                            Logger.Trace("CheckData NOK {0}", tmp);
                            SetDGVToolTip(i, "File not found");
                            LblInfo.Text = "File not found";
                        }
                        else
                        {
                            TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok  '{3}'\r\n", lineNr, action, "file exists", mypath);
                            SetCellColor(i, true);
                        }
                    }
                    if (action.Contains("URL"))
                    {
                        TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok  '{3}'\r\n", lineNr, action, "url not checked", mypath);
                        SetCellColor(i, true);
                    }
                }

                else if (action.Contains("G-Code"))
                {
                    if (!Grbl.isConnected)
                    {
                        tmp = string.Format("{0}) G-Code - grbl is not connected", lineNr);
                        TbProcessInfo.Text += tmp + "\r\n";
                        ok = false;
                        SetCellColor(i, false);
                        Logger.Trace("CheckData NOK {0}", tmp);
                        SetDGVToolTip(i, "grbl is not connected");
                        LblInfo.Text = "grbl is not connected";
                        isGrblConnected = false;
                    }
                    else
                    {
                        TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok\r\n", lineNr, action, value);
                        SetCellColor(i, true);
                        isGrblConnected = true;
                    }
                }

                else if (action.Contains("Probe Automatic"))
                {
                    if (false)//(!probeFormOpen)
                    {
                        tmp = string.Format("{0}) Probe - Probing form is not open", lineNr);
                        TbProcessInfo.Text += tmp + "\r\n";
                        ok = false;
                        SetCellColor(i, false);
                        SetDGVToolTip(i, "Probing form is not open");
                        Logger.Trace("CheckData NOK {0}", tmp);
                    }
                    else
                    {
                        TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-10}  ok\r\n", lineNr, action, value);
                        SetCellColor(i, true);
                    }
                }

                else if (action.Contains("Camera Automatic"))
                {
                    if (false)//(!cameraFormOpen)
                    {
                        tmp = string.Format("{0}) Fiducial - Camera form is not open", lineNr);
                        TbProcessInfo.Text += tmp + "\r\n";
                        ok = false;
                        SetCellColor(i, false);
                        SetDGVToolTip(i, "Camera form is not open");
                        Logger.Trace("CheckData NOK {0}", tmp);
                    }
                    else
                    {
                        TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok\r\n", lineNr, action, value);
                        SetCellColor(i, true);
                    }
                }


                else if (action.Contains("Wait Probe"))
                {
                    TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok\r\n", lineNr, action, (Grbl.StatMsg.Pn.Contains("P") ? "triggerd" : "not triggered"));
                    SetCellColor(i, true);
                }


                else if (action.Contains("Jump"))
                {
                    char delimiter = ';';
                    if (value.Contains(","))
                        delimiter = ',';
                    string[] vals = value.Split(delimiter);
                    processCount = 0;

                    ok = int.TryParse(vals[0], out int line);
                    if (!ok || (line <= 0) || (line >= i))
                    {
                        tmp = string.Format("{0}) Jump to line '{1}' doesn't work", lineNr, line);
                        TbProcessInfo.Text += tmp + "\r\n"; ok = false;
                        SetCellColor(i, false);
                        Logger.Trace("CheckData NOK {0}", tmp);
                        SetDGVToolTip(i, "jump target nok");
                        LblInfo.Text = "jump target nok";
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
                            SetCellColor(i, false);
                            SetDGVToolTip(i, "repitition nok");
                            Logger.Trace("CheckData NOK {0}", tmp);
                        }
                        else
                        {
                            processCount = rep;
                            LblCount.Text = processCount.ToString();
                        }
                    }
                    if (ok)
                    {
                        TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok\r\n", lineNr, action, value);
                        SetCellColor(i, true);
                    }
                }
                else if (action.Contains("CreateText") || action.Contains("CreateBarcode"))
                {
                    if (action.Contains(" Data") || action.Contains(" DURL"))
                    {
                        if ((int)NudDataIndex.Value > TbData.Lines.Length)// FctbData.Lines.Count)
                        {
                            tmp = string.Format("{0}) Data index is too high, reset index!   <----------------", lineNr);
                            TbProcessInfo.Text += tmp + "\r\n";
                            ok = false;
                            SetCellColor(i, false);
                            GbData.BackColor = Color.Fuchsia;
                            BtnDataIndexClear.BackColor = Color.Yellow;
                            Logger.Trace("CheckData NOK {0}", tmp);
                            SetDGVToolTip(i, "Reset data index");
                            LblInfo.Text = "Data index too high";
                        }
                        else
                        {
                            dataLine = (int)NudDataIndex.Value - 1;
                            char delimiter = (char)ComboDelimiter.Text[0]; //TbDataDelimeter.Text[0];
                            if (ComboDelimiter.Text.Contains("tab"))
                                delimiter = '\t';
                            Logger.Trace("Delimeter '{0}' '{1}'", ComboDelimiter.Text, delimiter);

                            string dataText = "";
                            if (dataLine < TbData.Lines.Length)//FctbData.Lines.Count)
                            {
                                dataText = GetDataText(dataLine, value, delimiter);   //delimiter);
                            }
                            BtnDataIndexClear.BackColor = GbData.BackColor = default;
                            TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok  '{3}'\r\n", lineNr, action, value, dataText);
                            SetCellColor(i, true);
                        }
                    }
                    else if (action.Contains(" Counter"))
                    {
                        TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok  '{3}'\r\n", lineNr, action, value, GetCounterString());
                        SetCellColor(i, true);
                    }
                }
                else
                {
                    TbProcessInfo.Text += string.Format("{0,2}) {1,-20}  {2,-15}  ok\r\n", lineNr, action, value);
                    SetCellColor(i, true);
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
                    //LblInfo.Text = "Not ready";
                    // BtnStart.BackColor = 
                    LblInfo.BackColor = Color.Fuchsia;
                }
            }
            return finalOK;
        }

        private void SetCellColor(int row, bool ok)
        {
            Color col = Color.LightGreen;
            if (!ok)
            { col = Color.Fuchsia; }
            dataGridView1.Rows[row].Cells[1].Style.BackColor = dataGridView1.Rows[row].Cells[2].Style.BackColor = col;
        }
        private void SetCellColor(int row, Color col)
        {
            dataGridView1.Rows[row].Cells[1].Style.BackColor = dataGridView1.Rows[row].Cells[2].Style.BackColor = col;
        }
        private void SetDGVToolTip(int row, string text)
        {
            if (row < dataGridView1.Rows.Count)
                for (int i = 0; i < dataGridView1.Rows[row].Cells.Count; i++)
                {
                    dataGridView1.Rows[row].Cells[i].ToolTipText = text;
                }
        }
        #endregion

        #region script_process
        /*************** State machine / flow control *******************************/
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!isGrblNeeded || !isGrblConnected || (Grbl.Status == GrblState.idle))
            {
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
                            if (nextAction.Contains("G-Code") || nextAction.Contains("Probe") || nextAction.Contains("Camera") ||
                                nextAction.Contains("Stream"))
                                isGrblNeeded = true;
                        }

                        if (showLog) Logger.Trace("Timer1_Tick line:{0}  action:{1}  value:{2}", lineNr, action, value);
                        dataGridView1.Rows[processStep].Cells[0].Selected = true;

                        stepTriggered = true;
                        stepCompleted = false;      // set to true if feedback appears
                        processAction = action;     // compare sent command with feedback command

                        LblInfo.Text = string.Format("Run {0}) {1}", (processStep + 1), action);
                        LblInfo.BackColor = Color.Yellow;

                        if (showLog) Logger.Trace("Timer line:{0})  {1}  {2}  count:{3}", (processStep + 1), action, value, processCount);

                        ProcessCommand(action, value, lineNr);
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
                        BtnStep.Visible = true;
                    }
                }
                else if (stepCompleted)
                {
                    stepTriggered = false;
                    processStep++;
                }
                else
                {	// poll for events, which can't give feedback
                    if (processStep < dataGridView1.Rows.Count)
                    {
                        string action = (string)dataGridView1.Rows[processStep].Cells[0].Value;
                        if (string.IsNullOrEmpty(action))
                        {
                            stepTriggered = false;
                            processStep++;
                            return;
                        }
                        string value = (string)dataGridView1.Rows[processStep].Cells[1].Value;

                        if (action.Contains("Wait Probe"))
                        {
                            if (Grbl.StatMsg.Pn.Contains("P"))
                            {
                                SetCellColor(processStep, true);
                                stepTriggered = false;
                                stepCompleted = false;
                                processStep++;
                                TbProcessInfo.AppendText("OK\r\n");
                            }
                        }
                        else if (action.Contains("Wait DI"))
                        {
                            if (action.Contains("=1"))
                            {
                                if ((Grbl.grblDigitalIn & (1 << checkDigitalInDigit)) > 0)
                                {
                                    SetCellColor(processStep, true);
                                    stepTriggered = false;
                                    stepCompleted = false;
                                    processStep++;
                                    TbProcessInfo.AppendText("OK\r\n");
                                }
                            }
                            else if (action.Contains("=0"))
                            {
                                if ((Grbl.grblDigitalIn & (1 << checkDigitalInDigit)) == 0)
                                {
                                    SetCellColor(processStep, true);
                                    stepTriggered = false;
                                    stepCompleted = false;
                                    processStep++;
                                    TbProcessInfo.AppendText("OK\r\n");
                                }
                            }
                            Logger.Trace("Timer1 poll digital in action:{0}  value:{1}  grbl:{2}  result:{3}", action, value, Grbl.grblDigitalIn, stepTriggered);
                        }
                    }
                }
            }
        }

        private bool ProcessCommand(string action, string value, int lineNr)
        {
            bool sendProcessEvent = true;
            if (action.Contains("Load"))
            {
                if (action.Contains(" Data"))
                {
                    dataLine = (int)NudDataIndex.Value - 1;
                    char delimiter = (char)ComboDelimiter.Text[0]; //TbDataDelimeter.Text[0];
                    if (ComboDelimiter.Text.Contains("tab"))
                        delimiter = '\t';
                    Logger.Trace("Delimeter '{0}' '{1}'", ComboDelimiter.Text, delimiter);

                    string dataText = "";
                    if (dataLine < TbData.Lines.Length)
                    {
                        dataText = GetDataText(dataLine, value, delimiter);
                        value = dataText;
                    }
                    else
                    {
                        Logger.Warn("Load - Data index nok: {0} {1}  count:{2}", dataLine, value, TbData.Lines.Length);//);
                        Feedback(action, "End of data reached", false);
                        sendProcessEvent = false;
                    }
                }
                if (action.Contains(" URL"))
                { }
                TbProcessInfo.AppendText(string.Format("{0,2}) {1,-10}  {2,-48}  ", lineNr, action, "..." + CutString(value, 39)));
            }
            else
                TbProcessInfo.AppendText(string.Format("{0,2}) {1,-20}  {2,-15}   ", lineNr, action, value));

        //    if (action.Contains("Paste clipboard"))
        //    { }

            if (action.Contains("G-Code"))
            {
                if (isGrblConnected)
                {
                    SendProcessEvent(new ProcessEventArgs(action, value));
                    SetCellColor(processStep, Color.Yellow);
                    //   stepCompleted = true;       /* check for IDLE should be enough */
                    if (action.Contains("Send"))
                    {
                        stepCompleted = true;       /* check for IDLE should be enough */
                        //   stepCompleted = true;       /* check for IDLE should be enough */
                        SetCellColor(processStep, true);
                        TbProcessInfo.AppendText("OK\r\n");
                    }
                }
                else
                {
                    SetCellColor(processStep, false);
                    stepCompleted = true;
                    TbProcessInfo.AppendText("NOK\r\n");
                }
            }

            else if (action.Contains("Create") && (action.Contains(" Data") || action.Contains(" DURL")))       // Text or barcode
            {
                dataLine = (int)NudDataIndex.Value - 1;
                if (dataLine < TbData.Lines.Length)
                {
                    char delimiter = (char)ComboDelimiter.Text[0];
                    if (ComboDelimiter.Text.Contains("tab"))
                        delimiter = '\t';
                    string dataText = GetDataText(dataLine, value, delimiter);

                    SendProcessEvent(new ProcessEventArgs(action, dataText));
                }
                else
                {
                    Logger.Warn("Data index nok: {0} {1}  count:{2}", dataLine, value, TbData.Lines.Length);//);
                    LblInfo.Text = "End of data reached";
                    Feedback(action, "End of data reached", false);
                }
            }
            else if (action.Contains("Create") && action.Contains(" Counter"))      // Text or barcode
            {
                SendProcessEvent(new ProcessEventArgs(action, GetCounterString()));
            }
            else if (action == "Data index")        // change index counter
            {
                //  int col;
                if (int.TryParse(value, out int col))
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
            else if (action == "Counter index")     // change counter
            {
                //   int col;
                if (int.TryParse(value, out int col))
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
                    SetCellColor(processStep, true);
                    processStep++;
                }

                TbProcessInfo.AppendText(string.Format("  count:{0}  jump to:{1}  OK\r\n", processCount, processStep + 1));
                LblCount.Text = Math.Abs(processCount).ToString();

                stepTriggered = false;
                stepCompleted = true;
                return true;
            }
            else if (action.Contains("Wait Probe"))
            {
                SetCellColor(processStep, Color.Yellow);
                LblInfo.Text = "Wait for trigger at probe input";
            }
            else if (action.Contains("Wait DI"))
            {
                SetCellColor(processStep, Color.Yellow);
                LblInfo.Text = "Wait for trigger at digital input";
                if (int.TryParse(value, out int nr))
                    checkDigitalInDigit = nr;
                else
                    Logger.Error("Timer1_Tick int.TryParse failed action:{0} value:{1}", action, value);
            }
            else if (action.Contains("Beep"))
            {
                CreateBeep(value);
                Feedback(action, "beep", true);
            }
            else if (action.Contains("Sound"))
            {
                if (value == "0") System.Media.SystemSounds.Asterisk.Play();
                else if (value == "1") System.Media.SystemSounds.Exclamation.Play();
                else if (value == "2") System.Media.SystemSounds.Question.Play();
                else if (value == "3") System.Media.SystemSounds.Hand.Play();
                else System.Media.SystemSounds.Beep.Play();
                Feedback(action, "sound", true);
            }
            else if (sendProcessEvent)
            {
                if (showLog) Logger.Trace("Timer1_Tick SendProcessEvent  {0}  {1}", action, value);
                SendProcessEvent(new ProcessEventArgs(action, value));
            }

            return true;
        }

        private string GetDataText(int dataLine, string strCol, char delimiter)
        {
            string all = TbData.Lines[dataLine];
            Logger.Trace("GetDataText '{0}'  '{1}'  from {2}", strCol, delimiter, all);
            if (strCol == "")
            {
                SetTextSelection(dataLine, -1, delimiter);
                return all;
            }

            //    int dataCol = -1;
            if (int.TryParse(strCol, out int dataCol))
            {
                string[] txtcol = all.Split(delimiter);
                Logger.Trace("GetDataText split '{0}'  ", txtcol.Length);

                if (dataCol + 1 <= txtcol.Length)
                {
                    SetTextSelection(dataLine, dataCol, delimiter);
                    return txtcol[dataCol];
                }
                else
                {
                    SetTextSelection(dataLine, dataCol, delimiter);
                    return txtcol[txtcol.Length - 1];
                }
            }
            SetTextSelection(dataLine, -1, delimiter);
            return all;
        }

        private void CreateBeep(string val)
        {
            if (val == "")
            {
                Console.Beep();
                return;
            }

            char delimiter = ';';
            if (val.Contains(","))
                delimiter = ',';
            string[] vals = val.Split(delimiter);
            if (vals.Length < 2)
            {
                Console.Beep();
                return;
            }

            if (int.TryParse(vals[0], out int hz))
            {
                if (int.TryParse(vals[1], out int len))
                { Console.Beep(hz, len); }
            }
        }

        #endregion

        public event EventHandler<ProcessEventArgs> RaiseProcessEvent;
        protected virtual void SendProcessEvent(ProcessEventArgs e)     // event processed in MainFormOtherForms
        {
            RaiseProcessEvent?.Invoke(this, e);
        }

        #region script_feedback
        public void Feedback(string action, string value, bool resultOk)
        {
            Logger.Trace("Feedback  action:'{0}'  value:'{1}'  ok:{2}", action, value, resultOk);
            if (action == "CheckForm")
            {
                processAction = resultOk.ToString();
                //    if (value == "Cam") { cameraFormOpen = resultOk; }
                //    if (value == "Probe") { probeFormOpen = resultOk; }
            }
            else if (action == processAction)
            {
                if (stepTriggered)
                {
                    if ((dataGridView1 != null) && (processStep < dataGridView1.Rows.Count))
                    {
                        if (!resultOk)
                        {
                            SetCellColor(processStep, false);
                            SetDGVToolTip(processStep, value);
                            LblInfo.Text = value;
                            LblInfo.BackColor = Color.Fuchsia;
                            TbProcessInfo.AppendText(string.Format("NOK  {0,-20}  {1}  \r\n", action, value));
                        }
                        else
                        {
                            SetCellColor(processStep, true);
                            stepCompleted = true;
                            if (action.Contains("Load"))
                                TbProcessInfo.AppendText(string.Format("OK\r\n", value));
                            else
                                TbProcessInfo.AppendText(string.Format(" {0,-20} OK\r\n", value));

                        }
                    }
                    else
                    {
                        Logger.Warn("Feedback failed processStep:{0} ", processStep);
                        BtnStop.PerformClick();
                        //    LblInfo.Text = value;
                    }
                }
            }

            if (!isRunning)
            {
                bool ok = CheckData();
                if (ok) { BtnStart.BackColor = Color.Lime; }
                //else { BtnStart.BackColor = Color.Fuchsia; }
            }
        }
        #endregion

        #region FormResize
        private void ControlProcessAutomation_SizeChanged(object sender, EventArgs e)
        { SizeChange(); }
        private void SizeChange()
        {
            dataGridView1.Width = splitContainer1.Width - 325;
            dataGridView1.Height = splitContainer1.SplitterDistance - 184;
            TbData.Height = splitContainer1.SplitterDistance - 184;

            int dataX = splitContainer1.Width - 330;
            TbData.Left = dataX;
            GbCounter.Left = dataX;
            GbData.Left = dataX;
            SetDataGridColumnWidth();
        }
        private void SetDataGridColumnWidth()
        {
            if (dataGridView1.Columns.Count > 2)
            {
                int part = dataGridView1.Width - (dataGridView1.Columns[0].Width + dataGridView1.Columns[1].Width);
                dataGridView1.Columns[2].Width = part - 70;
            }
        }

        private void SplitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Height = splitContainer1.SplitterDistance - 184;
            TbData.Height = splitContainer1.SplitterDistance - 184;
        }

        #endregion

        private string GetCounterString()
        {
            string count = string.Format("{0}", NudCounter.Value);
            count = count.PadLeft((int)NudDigitis.Value, TbFill.Text[0]);
            string tmp = string.Format("{0} {1} {2}", TbCounterFront.Text, count, TbCounterRear.Text);
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
                Filter = "CSV|*.csv;*.txt"
            };
            if (File.Exists(Properties.Settings.Default.processDataLastFile))
                sfd.InitialDirectory = Properties.Settings.Default.processDataLastFile;
            else
                sfd.InitialDirectory = Datapath.Automations + "\\";//    Application.StartupPath + Datapath.Examples;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string file = sfd.FileName;
                Properties.Settings.Default.processDataLastFile = LblDataLoaded.Text = file;

                LblDataLoaded.Text = "..." + CutString(file, 38);
                //   if (file.Length > 38)
                //       LblDataLoaded.Text = "..." + file.Substring(file.Length - 38);
                //    else
                //        LblDataLoaded.Text = file;
                TbData.Text = File.ReadAllText(file);
            }
            sfd.Dispose();
        }

        private void NudDataIndex_ValueChanged(object sender, EventArgs e)
        {
            int nr = (int)NudDataIndex.Value - 1;
            SetTextSelection(nr, -1, ' ');
            if (!isRunning)
                CheckData();
        }
        private void SetTextSelection(int line, int column, char delimiter)
        {
            if (line >= TbData.Lines.Length)
            {
                TbData.SelectionLength = 0;
                return;
            }

            int start = TbData.GetFirstCharIndexFromLine(line);
            string txt = TbData.Lines[line];
            string[] col = txt.Split(delimiter);
            if ((column >= 0) && (column < col.Length))
            {
                if (column > 0)
                {
                    for (int i = 0; i < column; i++)
                    { start += col[column - 1].Length + 1; }
                }
                TbData.Select(start, col[column].Length);
            }
            else
            {
                TbData.Select(start, txt.Length);
            }

        }

        /*************************************** XML *******************************************/
        #region XML
        private void LoadXML(string file)
        {
            //   BtnStart.Enabled = false;
            //isRunning = false;
            try
            {
                FileStream stream = new FileStream(file, FileMode.Open,
                            FileAccess.Read, FileShare.ReadWrite);
                XmlTextReader xmlFile = new XmlTextReader(stream);

                DataSet ds = new DataSet();
                ds.ReadXml(xmlFile);

                PresetDataGrid();
                string s1, s2, s3;
                int ci, i = 0;
                bool ok = true;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    s1 = row[0].ToString();
                    s2 = row[1].ToString();
                    s3 = row[2].ToString();
                    ci = GetIndexInAutomationItems(s1);
                    try
                    {
                        if (ci >= 0)
                        {
                            dataGridView1.Rows.Add(actionItems[ci].Command, s2, s3);
                        }
                        else
                        {
                            ci = actionItems.Count - 1;
                            dataGridView1.Rows.Add(actionItems[ci].Command, s1 + " " + s2, s3);
                            Logger.Trace("LoadXML nok {0}  {1}  {2}", s1, s2, s3);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, " Load XML error set Combobox i:{0}  s1:{1} ", i + 1, s1);
                        LblInfo.Text = string.Format("Line {0} command unknown: '{1}'", i, s1);
                        LblInfo.BackColor = Color.Fuchsia;
                        dataGridView1.Rows.Add(null, s2, s3);
                        ok = false;
                    }
                    i++;
                }
                dataGridView1.Invalidate();
                ds.Dispose();

                foreach (DataGridViewColumn Column in dataGridView1.Columns)
                {
                    Column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                if (!isRunning)
                    CheckData();
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
                if (actionItems[i].Command.ToLower().Contains(tmp.ToLower()))
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

            DataGridViewComboBoxColumn dgvCmb = new DataGridViewComboBoxColumn();
            dgvCmb.HeaderText = "Command";
            foreach (ProcessAutomationItem pai in actionItems)
                dgvCmb.Items.Add(pai.Command);
            dgvCmb.Name = "Command";
            dataGridView1.Columns.Add(dgvCmb);

            dataGridView1.Columns.Add("Value", "Value");
            dataGridView1.Columns.Add("Info", "Info"); ;

            dataGridView1.Columns[0].Width = 160;
            dataGridView1.Columns[1].Width = 140;

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
            foreach (string file in files)
            { AddLoadLine(file); }
        }
        private void AddLoadLine(string fileo)
        {
            string file = ExtendFilePath(fileo);
            if (!File.Exists(file))
            {
                Logger.Error("File does not exist: '{0}'", file);
                return;
            }
            string extension = Path.GetExtension(file);
            if (extension.ToLower().Contains("ini"))
            {
                string allText = File.ReadAllText(file);
                if (allText.Contains(IniFile.sectionProcAuto))
                {
                    var MyIni = new IniFile(file);
                    MyIni.ReadSection(IniFile.sectionProcAuto);
                    Logger.Info("AddLoadLine load INI '{0}'", file);
                    UpdateIniVariables();
                }
                else
                {
                    dataGridView1.Rows.Add("Load", file, "by drag & drop");
                }
            }
            else if (extension.ToLower().Contains("txt"))
            {
                LblDataLoaded.Text = "..." + CutString(file, 38);
                Properties.Settings.Default.processDataLastFile = file;
                TbData.Text = File.ReadAllText(file);
            }
            else
                dataGridView1.Rows.Add("Load", file, "by drag & drop");
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

                        //    dataGridView1.Rows[e.RowIndex].Cells[0].Value = actionItems[combo.SelectedIndex].ActionID;

                        dataGridView1.InvalidateRow(e.RowIndex);
                        //    combo = null;
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


        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    ctm.MenuItems.Add(new MenuItem(string.Format("Do something to row {0}", currentMouseOverRow.ToString())));
                }

                ctm.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        private void DataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            combo = null;
        }


        private void TbData_MouseUp(object sender, MouseEventArgs e)
        {
            int line = TbData.GetLineFromCharIndex(TbData.SelectionStart);
            NudDataIndex.Value = line + 1;
            SetTextSelection(line, -1, ' ');
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Logger.Error("dataGridView1_DataError row:{0}  col:{1}  excep:{2}", e.RowIndex, e.ColumnIndex, e.Exception);
            Logger.Trace("Value: '{0}'", (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }

        private void BtnStep_Click(object sender, EventArgs e)
        {
            int lineNr = dataGridView1.SelectedCells[0].OwningRow.Index;
            string action = (string)dataGridView1.Rows[lineNr].Cells[0].Value;
            if (string.IsNullOrEmpty(action))
            {
                return;
            }
            string value = (string)dataGridView1.Rows[lineNr].Cells[1].Value;

            ProcessCommand(action, value, lineNr);
            //    dataGridView1.SelectedCells[0].OwningRow.
            dataGridView1.Rows[lineNr + 1].Cells[0].Selected = true;

            dataGridView1.Rows[lineNr].Cells[0].Style.BackColor = default;
            dataGridView1.Rows[lineNr + 1].Cells[0].Style.BackColor = Color.Yellow;
        }
    }
    public partial class IniFile
    {
        internal static string[,] keyValueProcAuto = {
            {"Last script file",    "processLastFile" },
            {"Last data file",      "processDataLastFile" },
            {"Timer intervall",     "processTimerInterval"},
            {"Counter value",       "processCounter" },
            {"Counter digits",      "processCounterDigits"},
            {"Counter fill",        "processCounterFill"  },
            {"Counter text front",  "processCounterFront"  },
            {"Counter text rear",   "processCounterRear"   },
            {"Data delimiter",      "processDataDelimeter" },
            {"Data index",          "processDataIndex"   }
        //    {"Open on start",       "processOpenOnProgStart"},
        //    {"Split distance",      "processSplitDistance"},
        //    {"Size",                "processSize"},
        //    {"Location",            "processLocation"}
        };
        internal static string sectionProcAuto = "Process Automation";
    }
}
