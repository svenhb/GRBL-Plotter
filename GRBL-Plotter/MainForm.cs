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
/*  Thanks to https://github.com/PavelTorgashov/FastColoredTextBox

*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using virtualJoystick;
using FastColoredTextBoxNS;
using System.Deployment.Application;

namespace GRBL_Plotter
{
    public struct xyzPoint
    {   public double X, Y, Z;
        public xyzPoint(double x, double y, double z)
        { X = x; Y = y; Z = z; }
    };

    public partial class MainForm : Form
    {
        SerialForm _serial_form = null;
        CameraForm _camera_form = null;
        SetupForm _setup_form = null;
        TextToGCode _text_form = null;
        ImageToGCode _image_form = null;

        private const string appName = "GRBL Plotter";
        private xyzPoint posMachine = new xyzPoint(0, 0, 0);
        private xyzPoint posWorld = new xyzPoint(0, 0, 0);
        private xyzPoint posProbe = new xyzPoint(0, 0, 0);
        private grblState machineStatus;
        public bool flagResetOffset = false;

        public MainForm()
        {
            InitializeComponent();
            updateDrawing();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
        }
        //Unhandled exception
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            MessageBox.Show(ex.Message, "Thread exception");
        }
        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show(ex.Message, "Application exception");
            }
        }
        // open Camera form
        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((Application.OpenForms["CameraForm"] as CameraForm) == null)
            {//            if (_camera_form == null)
            
                _camera_form = new CameraForm();
                _camera_form.FormClosed += formClosed_CameraForm;
                _camera_form.RaiseXYEvent += OnRaiseCameraClickEvent;
            }
            else
            {
                _camera_form.Visible = false;
            }
            _camera_form.Show(this);
        }
        private void formClosed_CameraForm(object sender, FormClosedEventArgs e)
        { _camera_form = null; }

        // open Setup form
        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((Application.OpenForms["SetupForm"] as SetupForm) == null)
            {//            if (_setup_form == null)
            
                _setup_form = new SetupForm();
                _setup_form.FormClosed += formClosed_SetupForm;
                _setup_form.btnApplyChangings.Click += loadSettings;
                _setup_form.btnReloadFile.Click += reStartConvertSVG;
            }
            else
            {
                _setup_form.Visible = false;
            }
            _setup_form.Show(this);
        }
        private void formClosed_SetupForm(object sender, FormClosedEventArgs e)
        { _setup_form = null; }

        // open text creation form
        private void textWizzardToolStripMenuItem_Click(object sender, EventArgs e)
        {
//            if ((Application.OpenForms["TextForm"] as TextToGCode) == null)
            if (_text_form == null)
            { 
                _text_form = new TextToGCode();
                _text_form.FormClosed += formClosed_TextToGCode;
                _text_form.btnApply.Click += getGCodeFromText;      // assign btn-click event
            }
            else
            {
                _text_form.Visible = false;
            }
            _text_form.Show(this);
        }
        private void formClosed_TextToGCode(object sender, FormClosedEventArgs e)
        { _text_form = null; }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((Application.OpenForms["Image2GCode"] as ImageToGCode) == null)
            {//            if (_image_form == null)
            
                _image_form = new ImageToGCode();
                _image_form.FormClosed += formClosed_ImageToGCode;
                _image_form.btnGenerate.Click += getGCodeFromImage;      // assign btn-click event
            }
            else
            {
                _image_form.Visible = false;
            }
            _image_form.Show(this);
        }
        private void formClosed_ImageToGCode(object sender, FormClosedEventArgs e)
        {  _image_form = null;  }

        // open About form
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }
        // initialize Main form
        private void MainForm_Load(object sender, EventArgs e)
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < 0) || (Location.X > desktopSize.Width) || (Location.Y < 0) || (Location.Y > desktopSize.Height)) { Location = new Point(0, 0); }
            //            this.Text = appName + " Ver " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();    //Application.ProductVersion;
            this.Text = appName + " Ver " + System.Windows.Forms.Application.ProductVersion.ToString(); // Application.ProductVersion.ToString();    //Application.ProductVersion;
            loadSettings(sender, e);
            if ((Application.OpenForms["SerialForm"] as SetupForm) == null)
            {
                this._serial_form = new SerialForm();
                this._serial_form.Show(this);
                _serial_form.RaisePosEvent += OnRaisePosEvent;
                _serial_form.RaiseStreamEvent += OnRaiseStreamEvent;
            }
            updateControls();
            LoadRecentList();
            foreach (string item in MRUlist)
            {
                ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);  //create new menu for each item in list
                recentToolStripMenuItem.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            }

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            { loadFile(args[1]); }
        }
        // close Main form
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettings();
            _serial_form.Close();
            if ((Application.OpenForms["CameraForm"] as CameraForm) != null) _camera_form.Close();
        }
        // load settings
        private void loadSettings(object sender, EventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                }
                tbFile.Text = Properties.Settings.Default.file;
                setCustomButton(btnCustom1, Properties.Settings.Default.custom1);
                setCustomButton(btnCustom2, Properties.Settings.Default.custom2);
                setCustomButton(btnCustom3, Properties.Settings.Default.custom3);
                setCustomButton(btnCustom4, Properties.Settings.Default.custom4);
                setCustomButton(btnCustom5, Properties.Settings.Default.custom5);
                setCustomButton(btnCustom6, Properties.Settings.Default.custom6);
                setCustomButton(btnCustom7, Properties.Settings.Default.custom7);
                setCustomButton(btnCustom8, Properties.Settings.Default.custom8);
                fCTBCode.BookmarkColor = Properties.Settings.Default.colorMarker; ;
                pictureBox1.BackColor = Properties.Settings.Default.colorBackground;
                //                visuGCode.setColors();
                penUp.Color = Properties.Settings.Default.colorPenUp;
                penDown.Color = Properties.Settings.Default.colorPenDown;
                penRuler.Color = Properties.Settings.Default.colorRuler;
                penTool.Color = Properties.Settings.Default.colorTool;
                penMarker.Color = Properties.Settings.Default.colorMarker;
                penRuler.Width = (float)Properties.Settings.Default.widthRuler;
                penUp.Width = (float)Properties.Settings.Default.widthPenUp;
                penDown.Width = (float)Properties.Settings.Default.widthPenDown;
                penTool.Width = (float)Properties.Settings.Default.widthTool;
                penMarker.Width = (float)Properties.Settings.Default.widthMarker;
                updateDrawing();
            }
            catch (Exception a)
            {
                MessageBox.Show("Load Settings: " + a);
                //               logError("Loading settings", e);
            }
        }
        // Save settings
        public void saveSettings()
        {
            try
            {
                Properties.Settings.Default.file = tbFile.Text;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show("Save Settings: " + e);
                //               logError("Saving settings", e);
            }
        }
        // update controls on Main form
        public void updateControls(bool allowControl = false)
        {
            bool isConnected = _serial_form.SerialPortOpen;
            virtualJoystickXY.Enabled = isConnected && (!isStreaming || allowControl);
            virtualJoystickZ.Enabled = isConnected && (!isStreaming || allowControl);
            btnCustom1.Enabled = isConnected && (!isStreaming || allowControl);
            btnCustom2.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom3.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom4.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom5.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom6.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom7.Enabled = isConnected & !isStreaming | allowControl;
            btnCustom8.Enabled = isConnected & !isStreaming | allowControl;
            btnHome.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroX.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroY.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroZ.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroXY.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroXYZ.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroX.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroY.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroZ.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroXY.Enabled = isConnected & !isStreaming | allowControl;
            cBSpindle.Enabled = isConnected & !isStreaming | allowControl;
            tBSpeed.Enabled = isConnected & !isStreaming | allowControl;
            cBCoolant.Enabled = isConnected & !isStreaming | allowControl;
            cBTool.Enabled = isConnected & !isStreaming | allowControl;
            btnReset.Enabled = isConnected;
            btnFeedHold.Enabled = isConnected;
            btnResume.Enabled = isConnected;
            btnKillAlarm.Enabled = isConnected;
            btnStreamStart.Enabled = isConnected;// & isFileLoaded;
            btnStreamStop.Enabled = isConnected; // & isFileLoaded;
            btnStreamCheck.Enabled = isConnected;// & isFileLoaded;
        }
        // handle position events from serial form
        private void OnRaisePosEvent(object sender, PosEventArgs e)
        {
            posWorld = e.PosWorld;
            posMachine = e.PosMachine;
            machineStatus = e.Status;
            if (e.Status == grblState.probe)
            { posProbe = _serial_form.posProbe; }

            label_mx.Text = string.Format("{0:0.000}", posMachine.X);
            label_my.Text = string.Format("{0:0.000}", posMachine.Y);
            label_mz.Text = string.Format("{0:0.000}", posMachine.Z);
            label_wx.Text = string.Format("{0:0.000}", posWorld.X);
            label_wy.Text = string.Format("{0:0.000}", posWorld.Y);
            label_wz.Text = string.Format("{0:0.000}", posWorld.Z);
            visuGCode.setPosTool(posWorld.X, posWorld.Y);
            if (_camera_form != null)
            {
                _camera_form.setPosWorld = posWorld;
                _camera_form.setPosMachine = posMachine;
            }
            if (flagResetOffset)
            {
                double x = Properties.Settings.Default.lastOffsetX;
                double y = Properties.Settings.Default.lastOffsetY;
                double z = Properties.Settings.Default.lastOffsetZ;
                _serial_form.addToLog("Restore saved position after reset:");
                sendCommand(String.Format("G92 X{0} Y{1} Z{2}", x, y, z).Replace(',', '.'));
                flagResetOffset = false;
            }
            processStatus();
            processLastCommand(e.lastCommand);
            visuGCode.createMarkerPath();
            pictureBox1.Invalidate();
        }
        // handle status events from serial form
        private grblState lastMachineStatus = grblState.unknown;
        private string lastInfoText = "";
        private void processStatus() // {idle, run, hold, home, alarm, check, door}
        {
            if (machineStatus != lastMachineStatus)
            {
                label_status.Text = grbl.statusToText(machineStatus);
                label_status.BackColor = grbl.grblStateColor(machineStatus);
                switch (machineStatus)
                {
                    case grblState.idle:
                        if ((lastMachineStatus == grblState.hold) || (lastMachineStatus == grblState.alarm))
                        {
                            lbInfo.Text = lastInfoText;
                            lbInfo.BackColor = SystemColors.Control;
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        cBTool.Checked = _serial_form.TooInSpindle;
                        break;
                    case grblState.run:
                        if (lastMachineStatus == grblState.hold)
                        {
                            lbInfo.Text = lastInfoText;
                            lbInfo.BackColor = SystemColors.Control;
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        break;
                    case grblState.hold:
                        btnResume.BackColor = Color.Yellow;
                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = "Press 'Resume' to proceed";
                        lbInfo.BackColor = Color.Yellow;
                        if (signalResume == 0) { signalResume = 1; }
                        break;
                    case grblState.home:
                        break;
                    case grblState.alarm:
                        signalLock = 1;
                        btnKillAlarm.BackColor = Color.Yellow;
                        lbInfo.Text = "Press 'Kill Alarm' to proceed";
                        lbInfo.BackColor = Color.Yellow;
                        break;
                    case grblState.check:
                        break;
                    case grblState.door:
                        break;
                    case grblState.probe:
                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = string.Format("Probing: Z={0:0.000}", posProbe.Z);
                        lbInfo.BackColor = Color.Yellow;
                        break;
                    default:
                        break;
                }
            }
            lastMachineStatus = machineStatus;
        }
        private void processLastCommand(string cmd)
        {   if (cmd.Length < 2) return;
            if ((cmd.IndexOf("M3 ") >= 0) || (cmd.IndexOf("M03") >= 0) || (cmd.IndexOf("M4") >= 0) || (cmd.IndexOf("M04") >= 0))
                cBSpindle.Checked = true;
            if ((cmd.IndexOf("M5") >= 0) || (cmd.IndexOf("M05") >= 0))
                cBSpindle.Checked = false;
            if ((cmd.IndexOf("M7") >= 0) || (cmd.IndexOf("M07") >= 0) || (cmd.IndexOf("M8") >= 0) || (cmd.IndexOf("M08") >= 0))
                cBCoolant.Checked = true;
            if ((cmd.IndexOf("M9") >= 0) || (cmd.IndexOf("M09") >= 0))
                cBCoolant.Checked = false;
        }

        // update drawing on Main form
        private void updateDrawing()
        {
            visuGCode.createImagePath();  // show initial empty picture
            pictureBox1.Invalidate();
        }
        // send command via serial form
        private void sendCommand(string txt)
        { _serial_form.requestSend(txt); }
        // open a file via dialog
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "gcode files (*.nc)|*.nc|SVG files (*.svg)|*.svg|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            { loadFile(openFileDialog1.FileName); }
        }
        private void loadFile(string fileName)
        {
            visuGCode.setMarkerOnDrawing("G0 X0 Y0");
            String ext = Path.GetExtension(fileName).ToLower();
            if (ext == ".svg")
            { startConvertSVG(fileName); }
            if (ext == ".nc")
            {
                tbFile.Text = fileName;
                loadGcode();
            }
            if ((ext == ".bmp") || (ext == ".gif") || (ext == ".png") || (ext == ".jpg"))
            {
                if (_image_form == null)
                {
                    _image_form = new ImageToGCode(true);
                    _image_form.FormClosed += formClosed_ImageToGCode;
                    _image_form.btnGenerate.Click += getGCodeFromImage;      // assign btn-click event
                }
                else
                {
                    _image_form.Visible = false;
                }
                _image_form.Show(this);
                _image_form.loadExtern(fileName);
            }
            SaveRecentFile(fileName);
            isFileLoaded = true;
        }
        // save content from TextEditor (GCode) to file
        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "GCode|*.nc";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string txt = fCTBCode.Text;
                File.WriteAllText(sfd.FileName, txt);
            }
        }
        // load selected file
        bool isFileLoaded = false;
        bool blockRTBEvents = false;
        private void btnLoad_Click(object sender, EventArgs e)
        { }
        private void loadGcode()
        {
            if (File.Exists(tbFile.Text))
            {
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                fCTBCodeClickedLineNow = 0;
                fCTBCodeClickedLineLast = 0;
                visuGCode.setPosMarker(0, 0);
                blockRTBEvents = true;
                fCTBCode.OpenFile(tbFile.Text);
                redrawGCodePath();
                isFileLoaded = true;
                blockRTBEvents = false;
                lbInfo.Text = "G-Code loaded";
                lbInfo.BackColor = SystemColors.Control;
                updateControls();
                SaveRecentFile(tbFile.Text);
                this.Text = appName + " | File: " + tbFile.Text;
            }
        }

        // handle file streaming
        TimeSpan elapsed;               //elapsed time from file burnin
        DateTime timeInit;              //time start to burning file
        private int signalResume = 0;   // blinking button
        private int signalLock = 0;     // blinking button
        private int signalPlay = 0;     // blinking button
        private bool isStreaming = false;
        private bool isStreamingPause = false;
        private bool isStreamingCheck = false;
        private bool isStreamingOk = true;
        private void OnRaiseStreamEvent(object sender, StreamEventArgs e)
        {
            int cPrgs = (int)Math.Max(0, Math.Min(100, e.CodeProgress));
            int bPrgs = (int)Math.Max(0, Math.Min(100, e.BuffProgress));
            pbFile.Value = cPrgs;
            pbBuffer.Value = bPrgs;
            lblFileProgress.Text = string.Format("Progress {0:0.0}%", e.CodeProgress);
            fCTBCode.Selection = fCTBCode.GetLine(e.CodeLine);
            fCTBCodeClickedLineNow = e.CodeLine;
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();

            if (e.Status == grblStreaming.reset)
            {
                flagResetOffset = true;
                isStreaming = false;
                isStreamingCheck = false;
                updateControls();
            }
            if (e.Status == grblStreaming.error)
            {
                pbFile.ForeColor = Color.Red;
                lbInfo.Text = "Error in line " + e.CodeLine.ToString();
                lbInfo.BackColor = Color.Fuchsia;
                fCTBCode.BookmarkLine(e.CodeLine - 1);
                fCTBCode.DoSelectionVisible();
                fCTBCode.CurrentLineColor = Color.Red;
                isStreamingOk = false;
            }
            if ((e.Status == grblStreaming.ok) && !isStreamingCheck)
            {
                updateControls();
                lbInfo.Text = "Send G-Code";
                lbInfo.BackColor = Color.Lime;
                signalPlay = 0;
                btnStreamStart.BackColor = SystemColors.Control;
                //                btnStreamPause.BackColor = SystemColors.Control; 
            }
            if (e.Status == grblStreaming.finish)
            {
                if (isStreamingOk)
                {
                    if (isStreamingCheck)
                    { lbInfo.Text = "Finish checking G-Code"; }
                    else
                    { lbInfo.Text = "Finish sending G-Code"; }
                    lbInfo.BackColor = Color.Lime;
                    pbFile.Value = 0;
                    pbBuffer.Value = 0;
                }
                isStreaming = false; isStreamingCheck = false;
                btnStreamStart.Image = Properties.Resources.btn_play;
                btnStreamStart.Enabled = true;
                btnStreamCheck.Enabled = true;
                updateControls();
            }
            if (e.Status == grblStreaming.waitidle)
            {
                updateControls(true);
                btnStreamStart.Image = Properties.Resources.btn_play;
                isStreamingPause = true;
                lbInfo.Text = "Wait for IDLE, then pause";
                lbInfo.BackColor = Color.Yellow;
            }
            if (e.Status == grblStreaming.pause)
            {
                updateControls(true);
                btnStreamStart.Image = Properties.Resources.btn_play;
                isStreamingPause = true;
                lbInfo.Text = "Pause streaming - press play";
                signalPlay = 1;
                lbInfo.BackColor = Color.Yellow;
            }
            if (e.Status == grblStreaming.toolchange)
            {
                updateControls();
                btnStreamStart.Image = Properties.Resources.btn_play;
                lbInfo.Text = "Tool change...";
                lbInfo.BackColor = Color.Yellow;
                cBTool.Checked = _serial_form.TooInSpindle;
            }
        }
        private void btnStreamStart_Click(object sender, EventArgs e)
        {
            if (!isStreaming)
            {
                isStreaming = true;
                isStreamingPause = false;
                isStreamingCheck = false;
                isStreamingOk = true;
                updateControls();
                timeInit = DateTime.UtcNow;
                elapsed = TimeSpan.Zero;
                lbInfo.Text = "Send G-Code";
                lbInfo.BackColor = Color.Lime;
                for (int i = 0; i < fCTBCode.LinesCount; i++)
                    fCTBCode.UnbookmarkLine(i);
                lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
                _serial_form.startStreaming(fCTBCode.Lines);
                btnStreamStart.Image = Properties.Resources.btn_pause;
                btnStreamCheck.Enabled = false;
            }
            else
            {
                if (!isStreamingPause)
                {
                    btnStreamStart.Image = Properties.Resources.btn_play;
                    _serial_form.pauseStreaming();
                    isStreamingPause = true;
                }
                else
                {
                    btnStreamStart.Image = Properties.Resources.btn_pause;
                    _serial_form.pauseStreaming();
                    isStreamingPause = false;
                }
            }
        }
        private void btnStreamCheck_Click(object sender, EventArgs e)
        {
            isStreaming = true;
            isStreamingCheck = true;
            isStreamingOk = true;
            updateControls();
            timeInit = DateTime.UtcNow;
            elapsed = TimeSpan.Zero;
            lbInfo.Text = "Check G-Code";
            lbInfo.BackColor = SystemColors.Control;
            for (int i = 0; i < fCTBCode.LinesCount; i++)
                fCTBCode.UnbookmarkLine(i);
            _serial_form.startStreaming(fCTBCode.Lines, true);
            btnStreamStart.Enabled = false;
        }
        private void btnStreamStop_Click(object sender, EventArgs e)
        {
            btnStreamStart.Image = Properties.Resources.btn_play;
            btnStreamStart.Enabled = true;
            btnStreamCheck.Enabled = true;
            _serial_form.stopStreaming();
            if (isStreaming || isStreamingCheck)
            {
                lbInfo.Text = " STOP streaming";
                lbInfo.BackColor = Color.Fuchsia;
            }
            isStreaming = false;
            isStreamingCheck = false;
            pbFile.Value = 0;
            pbBuffer.Value = 0;
            signalPlay = 0;
            updateControls();
        }
        private void btnStreamPause_Click(object sender, EventArgs e)
        { _serial_form.pauseStreaming(); }

        // handle event from create Text form
        private void getGCodeFromText(object sender, EventArgs e)
        {
            fCTBCode.Text = _text_form.textGCode;
            redrawGCodePath();
            isFileLoaded = true;
            updateControls();
        }
        // handle event from create Image form
        private void getGCodeFromImage(object sender, EventArgs e)
        {
            fCTBCode.Text = _image_form.imageGCode;
            redrawGCodePath();
            isFileLoaded = true;
            updateControls();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (isStreaming)
            {
                elapsed = DateTime.UtcNow - timeInit;
                lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
            }
            if (signalResume > 0)   // activate blinking buttob
            {
                if ((signalResume++ % 2) > 0) btnResume.BackColor = Color.Yellow;
                else btnResume.BackColor = SystemColors.Control;
            }
            if (signalLock > 0) // activate blinking buttob
            {
                if ((signalLock++ % 2) > 0) btnKillAlarm.BackColor = Color.Yellow;
                else btnKillAlarm.BackColor = SystemColors.Control;
            }
            if (signalPlay > 0) // activate blinking buttob
            {
                if ((signalPlay++ % 2) > 0) btnStreamStart.BackColor = Color.Yellow;
                else btnStreamStart.BackColor = SystemColors.Control;
            }
        }

        // Setup Custom Buttons during loadSettings()
        string[] btnCustomCommand = new string[9];
        private void setCustomButton(Button btn, string text)
        {
            int index = Convert.ToUInt16(btn.Name.Substring("btnCustom".Length));
            string[] parts = text.Split('|');
            if (parts.Length > 1)
            {
                btn.Text = parts[0];
                if (File.Exists(parts[1]))
                { toolTip1.SetToolTip(btn, parts[0] + "\r\nFile: " + parts[1] + "\r\n" + File.ReadAllText(parts[1])); }
                else
                { toolTip1.SetToolTip(btn, parts[0] + "\r\n" + parts[1]); }
                btnCustomCommand[index] = parts[1];
            }
            else
                btnCustomCommand[index] = "";
        }
        private void btnCustomButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int index = Convert.ToUInt16(clickedButton.Name.Substring("btnCustom".Length));
            string btnCmd = btnCustomCommand[index];
            string[] commands;
            if (File.Exists(btnCmd))
            {
                string fileCmd = File.ReadAllText(btnCmd);
                _serial_form.addToLog("file: " + btnCmd);
                commands = fileCmd.Split('\n');
            }
            else
            {
                commands = btnCustomCommand[index].Split(';');
            }
            foreach (string cmd in commands)
                sendCommand(cmd.Trim());
        }

        // handle positon click event from camera form
        private void OnRaiseCameraClickEvent(object sender, XYEventArgs e)
        {
            if (e.Command == "a")
            { routeTransformCode((float)e.PosX); }
            else
            {
                double realStepX = Math.Round(e.PosX * 100) / 100;
                double realStepY = Math.Round(e.PosY * 100) / 100;
                int speed = 1000;
                //               String s = String.Format("F{0}"+ G91G1 X{1} Y{2}", speed, realStepX, realStepY);
                String s = String.Format("F{0} " + e.Command + " X{1} Y{2}", speed, realStepX, realStepY).Replace(',', '.');
                sendCommand(s);
            }
        }

        // virtualJoystic sends two step-width-values per second. One position should be reached before next command
        // speed (units/min) = 2 * stepsize * 60 * factor (to compensate speed-ramps)
        private void virtualJoystickXY_JoyStickEvent(object sender, JogEventArgs e)
        {
            double maxStep = Math.Max(Math.Abs(e.JogPosX), Math.Abs(e.JogPosY));
            int speed = Convert.ToInt32(2 * maxStep * 60 * 0.95);
            String s = "";
            if (maxStep > 0)
            {
                if (e.JogPosX == 0)
                    s = String.Format("F{0} G1 Y{1}", speed, e.JogPosY).Replace(',', '.');
                else if (e.JogPosY == 0)
                    s = String.Format("F{0} G1 X{1}", speed, e.JogPosX).Replace(',', '.');
                else
                    s = String.Format("F{0} G1 X{1} Y{2}", speed, e.JogPosX, e.JogPosY).Replace(',', '.');
                sendCommand(s);
            }
        }
        private void virtualJoystickXY_Enter(object sender, EventArgs e)
        { sendCommand("G91"); }
        private void virtualJoystickXY_Leave(object sender, EventArgs e)
        { sendCommand("G90"); }
        private void virtualJoystickZ_JoyStickEvent(object sender, JogEventArgs e)
        {
            int speed = Convert.ToInt32(Math.Abs(e.JogPosY) * 120 * 0.99);
            if (speed > 0)
            {
                String s = String.Format("F{0} G1 Z{1}", speed, e.JogPosY).Replace(',', '.');
                sendCommand(s);
            }
        }
        // Spindle and coolant
        private void cBSpindle_CheckedChanged(object sender, EventArgs e)
        {
            if (cBSpindle.Checked)
            { sendCommand("M3 S" + tBSpeed.Text); }
            else
            { sendCommand("M5"); }
        }
        private void cBCoolant_CheckedChanged(object sender, EventArgs e)
        {
            if (cBCoolant.Checked)
            { sendCommand("M8"); }
            else
            { sendCommand("M9"); }
        }
        private void btnHome_Click(object sender, EventArgs e)
        { sendCommand("$H"); }
        private void btnZeroX_Click(object sender, EventArgs e)
        { sendCommand("G92 X0"); }
        private void btnZeroY_Click(object sender, EventArgs e)
        { sendCommand("G92 Y0"); }
        private void btnZeroZ_Click(object sender, EventArgs e)
        { sendCommand("G92 Z0"); }
        private void btnZeroXY_Click(object sender, EventArgs e)
        { sendCommand("G92 X0 Y0"); }
        private void btnZeroXYZ_Click(object sender, EventArgs e)
        { sendCommand("G92 X0 Y0 Z0"); }

        private void btnJogX_Click(object sender, EventArgs e)
        { sendCommand("G90 G0 X0"); }
        private void btnJogY_Click(object sender, EventArgs e)
        { sendCommand("G90 G0 Y0"); }
        private void btnJogZ_Click(object sender, EventArgs e)
        { sendCommand("G90 G0 Z0"); }
        private void btnJogXY_Click(object sender, EventArgs e)
        { sendCommand("G90 G0 X0 Y0"); }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _serial_form.grblReset();
            pbFile.Value = 0;
            signalResume = 0;
            signalLock = 0;
            signalPlay = 0;
            btnResume.BackColor = SystemColors.Control;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            cBSpindle.Checked = false;
            cBCoolant.Checked = false;
            updateControls();
        }
        private void btnFeedHold_Click(object sender, EventArgs e)
        {
            sendCommand("!");
            signalResume = 1;
            updateControls(true);
        }
        private void btnResume_Click(object sender, EventArgs e)
        {
            sendCommand("~");
            btnResume.BackColor = SystemColors.Control;
            signalResume = 0;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            updateControls();
        }
        private void btnKillAlarm_Click(object sender, EventArgs e)
        {
            sendCommand("$X");
            signalLock = 0;
            btnKillAlarm.BackColor = SystemColors.Control;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            updateControls();
        }


        /// <summary>
        /// Handling of RichTextBox rtBCode
        /// </summary>
        bool showChangedMessage = true;     // show Message if TextChanged
        int rtbSize = 0;
        private void rtbCode_TextChanged(object sender, EventArgs e)
        {
            if (!blockRTBEvents)
            {
                int rtbActualSize = fCTBCode.LinesCount;
                if (Math.Abs(rtbActualSize - rtbSize) > 20)     // Highlight / Redraw after huge change
                {
                    redrawGCodePath();
                    showChangedMessage = true;
                }
                else
                {
                    if (showChangedMessage)
                    {
                        lbInfo.Text = "G-Code was changed";
                        lbInfo.BackColor = Color.Orange;
                        if (Math.Abs(rtbActualSize - rtbSize) > 20)     // Highlight / Redraw after huge change
                            redrawGCodePath();
                    }
                }
                rtbSize = rtbActualSize;
            }
        }

        private void btnTransformCode_Click(object sender, EventArgs e)
        {
            double size, angle;
            if (!Double.TryParse(tbChangeSize.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out size))
            {
                size = 100; tbChangeSize.Text = "100.00";
            }
            if (!Double.TryParse(tbChangeAngle.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out angle))
            {
                angle = 0; tbChangeAngle.Text = "0.00";
            }
            fCTBCode.Cursor = Cursors.WaitCursor;
            transformGCode(size, angle, GCodeVisuAndTransform.translate.None);
            fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
            fCTBCodeMarkLine();
            fCTBCode.Cursor = Cursors.IBeam;
            showChangedMessage = true;
            tbChangeSize.Text = "100.00";
            tbChangeAngle.Text = "0.00";
        }
        public void routeTransformCode(float angle)
        {
            tbChangeSize.Text = "100.00";
            tbChangeAngle.Text = String.Format("{0:0.00}", angle);
            fCTBCode.Cursor = Cursors.WaitCursor;
            transformGCode(Convert.ToDouble(tbChangeSize.Text), Convert.ToDouble(tbChangeAngle.Text), GCodeVisuAndTransform.translate.None);
            visuGCode.createImagePath();
            pictureBox1.Invalidate();
            fCTBCode.Cursor = Cursors.IBeam;
            showChangedMessage = true;
            tbChangeSize.Text = "100.00";
            tbChangeAngle.Text = "0.00";
        }
        private void btnShiftToZero_MirrorXY_Click(object sender, EventArgs e)
        {
            fCTBCode.Cursor = Cursors.WaitCursor;
            Button clickedButton = sender as Button;
            if (clickedButton.Name == "btnShiftToZero")
                transformGCode(100, 0, GCodeVisuAndTransform.translate.Offset0);
            else if (clickedButton.Name == "btnMirrorX")
                transformGCode(100, 0, GCodeVisuAndTransform.translate.MirrorX);
            else if (clickedButton.Name == "btnMirrorY")
                transformGCode(100, 0, GCodeVisuAndTransform.translate.MirrorY);
            updateDrawing();
            fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
            fCTBCodeClickedLineLast = 0;
            fCTBCodeMarkLine();
            fCTBCode.Cursor = Cursors.IBeam;
            showChangedMessage = true;
        }

        public GCodeVisuAndTransform visuGCode = new GCodeVisuAndTransform(100, 100);
        // Refresh drawing path in GCodeVisuAndTransform by applying no transform
        private void redrawGCodePath()
        {
            visuGCode.transformGCode(fCTBCode.Lines, 100, 0, GCodeVisuAndTransform.translate.None);
            updateDrawing();
            lbDimension.Text = visuGCode.xyzSize.GetString(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
            toolStripScaleTextBoxWidth.Text = string.Format("{0:0.00}", visuGCode.xyzSize.dimx);
            toolStripScaleTextBoxHeight.Text = string.Format("{0:0.00}", visuGCode.xyzSize.dimy);
        }
        // tranform drawing and refresh path
        private void transformGCode(double scale, double angle, GCodeVisuAndTransform.translate trans)
        {
            fCTBCode.Text = visuGCode.transformGCode(fCTBCode.Lines, scale, angle, trans);
            updateDrawing();
            lbDimension.Text = visuGCode.xyzSize.GetString(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
            toolStripScaleTextBoxWidth.Text = string.Format("{0:0.00}", visuGCode.xyzSize.dimx);
            toolStripScaleTextBoxHeight.Text = string.Format("{0:0.00}", visuGCode.xyzSize.dimy);
        }

        // highlight code in editor
        Style StyleComment = new TextStyle(Brushes.Gray, null, FontStyle.Italic);
        Style StyleGWord = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        Style StyleMWord = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Regular);
        Style StyleFWord = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        Style StyleSWord = new TextStyle(Brushes.OrangeRed, null, FontStyle.Regular);
        Style StyleTool = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        Style StyleXAxis = new TextStyle(Brushes.Green, null, FontStyle.Bold);
        Style StyleYAxis = new TextStyle(Brushes.BlueViolet, null, FontStyle.Bold);
        Style StyleZAxis = new TextStyle(Brushes.Red, null, FontStyle.Bold);
        private void fCTBCode_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(StyleComment);
            e.ChangedRange.SetStyle(StyleComment, "(\\(.*\\))", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleGWord, "(G\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleMWord, "(M\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleFWord, "(F\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleSWord, "(S\\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleTool, "(T\\d{1,2})", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleXAxis, "[XIxi]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleYAxis, "[YJyj]{1}-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            e.ChangedRange.SetStyle(StyleZAxis, "[Zz]-?\\d+(.\\d+)?", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        // mark clicked line in editor
        int fCTBCodeClickedLineNow = 0;
        int fCTBCodeClickedLineLast = 0;
        private void fCTBCode_Click(object sender, EventArgs e)
        {
            fCTBCodeClickedLineNow = fCTBCode.Selection.ToLine;
            fCTBCodeMarkLine();
        }
        private void fCTBCode_KeyDown(object sender, KeyEventArgs e)
        {
            int key = e.KeyValue;
            if ((key == 38) && (fCTBCodeClickedLineNow > 0))
            {
                fCTBCodeClickedLineNow -= 1;
                fCTBCode.Selection = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                fCTBCodeMarkLine();
            }
            if ((key == 40) && (fCTBCodeClickedLineNow < (fCTBCode.Lines.Count - 1)))
            {
                fCTBCodeClickedLineNow += 1;
                fCTBCode.Selection = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                fCTBCodeMarkLine();
            }
        }
        private void fCTBCodeMarkLine()
        {
            if (fCTBCodeClickedLineNow <= fCTBCode.LinesCount)
            {
                if (fCTBCodeClickedLineNow != fCTBCodeClickedLineLast)
                {
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                    fCTBCode.BookmarkLine(fCTBCodeClickedLineNow);
                    Range selected = fCTBCode.GetLine(fCTBCodeClickedLineNow);
                    fCTBCode.Selection = selected;
                    fCTBCode.SelectionColor = Color.Orange;
                    fCTBCodeClickedLineLast = fCTBCodeClickedLineNow;
                    // Set marker in drawing
                    visuGCode.setMarkerOnDrawing(fCTBCode.SelectedText);
                    pictureBox1.Invalidate();
                    if (_camera_form != null)
                        _camera_form.setPosMarker(visuGCode.GetPosMarkerX(), visuGCode.GetPosMarkerY());
                }
            }
        }

        // context Menu on fastColoredTextBox
        private void cmsCode_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "cmsCodeSelect")
            {
                fCTBCode.SelectAll();
            }
            if (e.ClickedItem.Name == "cmsCodeCopy")
            {
                if (fCTBCode.SelectedText.Length > 0)
                    fCTBCode.Copy();
            }
            if (e.ClickedItem.Name == "cmsCodePaste")
            {
                // groupBox1.Focus();
                fCTBCode.Paste();
            }
            if (e.ClickedItem.Name == "cmsCodeSendLine")
            {
                int clickedLine = fCTBCode.Selection.ToLine;
                sendCommand("G90 " + fCTBCode.Lines[clickedLine]);
                MessageBox.Show("G90 " + fCTBCode.Lines[clickedLine]);
            }
        }

        private void fCTBCode_TextChangedDelayed(object sender, TextChangedEventArgs e)
        { showChangedMessage = true; }

        private void toolStripScaleTextBox1_TextChanged(object sender, EventArgs e)
        {
            double sizenew;
            double sizeold = visuGCode.xyzSize.dimx;
            if (!Double.TryParse(toolStripScaleTextBoxWidth.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                sizenew = sizeold;
            tbChangeSize.Text = string.Format("{0:0.00}", (100 * sizenew / sizeold));
        }
        private void toolStripScaleTextBoxHeight_TextChanged(object sender, EventArgs e)
        {
            double sizenew;
            double sizeold = visuGCode.xyzSize.dimy;
            if (!Double.TryParse(toolStripScaleTextBoxHeight.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                sizenew = sizeold;
            tbChangeSize.Text = string.Format("{0:0.00}", (100 * sizenew / sizeold));
        }
        private void toolStripScaleTextBoxWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                btnTransformCode_Click(sender, e);
                cmsScale.Close();
            }
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
            {
                loadFile(files[0]);
            }
            else
            { tBURL.Text = s; }
        }
        private void tBURL_TextChanged(object sender, EventArgs e)
        {
            var parts = tBURL.Text.Split('.');
            string ext = parts[parts.Length - 1];
            if (ext.ToLower() == "svg")
            {
                startConvertSVG(tBURL.Text);
                tBURL.Text = "";
            }
            else
            {
                if (tBURL.Text.Length > 5)
                    MessageBox.Show("URL extension is not 'svg'");
            }
        }
        public void reStartConvertSVG(object sender, EventArgs e)   // event from setup form
        { startConvertSVG(lastSource); }
        private string lastSource = "";
        private void startConvertSVG(string source)
        {
            lastSource = source;
            this.Cursor = Cursors.WaitCursor;
            string gcodeh = "( Imported with GRBL-Plotter )\r\n";
            gcodeh += string.Format("( Source: {0} )\r\n", source);
            string gcode = SVGToGCode.ConvertFile(source);
            if (gcode.Length > 2)
            {
                fCTBCode.Text = gcodeh + gcode;
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                redrawGCodePath();
                SaveRecentFile(source);
                isFileLoaded = true;
                this.Text = appName + " | Source: " + source;
            }
            this.Cursor = Cursors.Default;
            updateControls();
        }

        // handle MRU List
        private int MRUnumber = 20;
        private List<string> MRUlist = new List<string>();
        private void SaveRecentFile(string path)
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            LoadRecentList(); //load list from file
            if (MRUlist.Contains(path)) //prevent duplication on recent list
                MRUlist.Remove(path);
            MRUlist.Insert(0, path);    //insert given path into list on top
                                        //keep list number not exceeded the given value
            while (MRUlist.Count > MRUnumber)
            { MRUlist.RemoveAt(MRUlist.Count - 1); }
            foreach (string item in MRUlist)
            {
                ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);
                recentToolStripMenuItem.DropDownItems.Add(fileRecent);
            }
            StreamWriter stringToWrite =
            new StreamWriter(System.Environment.CurrentDirectory + "\\Recent.txt");
            foreach (string item in MRUlist)
            { stringToWrite.WriteLine(item); }
            stringToWrite.Flush(); //write stream to file
            stringToWrite.Close(); //close the stream and reclaim memory
        }
        private void LoadRecentList()
        {
            MRUlist.Clear();
            try
            {
                StreamReader listToRead =
                new StreamReader(System.Environment.CurrentDirectory + "\\Recent.txt");
                string line;
                MRUlist.Clear();
                while ((line = listToRead.ReadLine()) != null) //read each line until end of file
                    MRUlist.Add(line); //insert to list
                listToRead.Close(); //close the stream
            }
            catch (Exception) { }
        }
        private void RecentFile_click(object sender, EventArgs e)
        {
            loadFile(sender.ToString());
        }

        // onPaint drawing
        private Pen penUp = new Pen(Color.Green, 0.1F);
        private Pen penDown = new Pen(Color.Red, 0.4F);
        private Pen penRuler = new Pen(Color.Blue, 0.1F);
        private Pen penTool = new Pen(Color.Black, 0.5F);
        private Pen penMarker = new Pen(Color.DeepPink, 1F);
        private double picAbsPosX = 0;
        private double picAbsPosY = 0;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            double minx = GCodeVisuAndTransform.drawingSize.minX;                  // extend dimensions
            double maxx = GCodeVisuAndTransform.drawingSize.maxX;
            double miny = GCodeVisuAndTransform.drawingSize.minY;
            double maxy = GCodeVisuAndTransform.drawingSize.maxY;
            double xRange = (maxx - minx);                                              // calculate new size
            double yRange = (maxy - miny);
            double picScaling = Math.Min(pictureBox1.Width / (xRange), pictureBox1.Height / (yRange));               // calculate scaling px/unit
            if ((picScaling > 0.001) && (picScaling < 10000))
            {
                double relposX = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).X) / pictureBox1.Width);
                double relposY = (Convert.ToDouble(pictureBox1.PointToClient(MousePosition).Y) / pictureBox1.Height);
                double ratioVisu = xRange / yRange;
                double ratioPic = Convert.ToDouble(pictureBox1.Width) / pictureBox1.Height;
                if (ratioVisu > ratioPic)
                    relposY = relposY * ratioVisu / ratioPic;
                else
                    relposX = relposX * ratioPic / ratioVisu;
                picAbsPosX = relposX * xRange + minx;
                picAbsPosY = yRange - relposY * yRange + miny;
                int offX = +5;
                if (pictureBox1.PointToClient(MousePosition).X > (pictureBox1.Width - 100))
                { offX = -75; }
                Point stringpos = new Point(pictureBox1.PointToClient(MousePosition).X + offX, pictureBox1.PointToClient(MousePosition).Y - 10);
                e.Graphics.DrawString(String.Format("Worl-Pos:\r\nX:{0,7:0.00}\r\nY:{1,7:0.00}", picAbsPosX, picAbsPosY), new Font("Lucida Console", 8), Brushes.Black, stringpos);
                //                e.Graphics.DrawString(String.Format("Ratio:\r\nVisu:{0,7:0.00}\r\nPic:{1,7:0.00}", ratioVisu, ratioPic), new Font("Lucida Console", 8), Brushes.Black, 10,10);

                e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);        // apply scaling (flip Y)
                e.Graphics.TranslateTransform((float)-minx, (float)(-yRange - miny));       // apply offset
                e.Graphics.DrawPath(penRuler, GCodeVisuAndTransform.pathRuler);
                e.Graphics.DrawPath(penDown, GCodeVisuAndTransform.pathPenDown);
                e.Graphics.DrawPath(penUp, GCodeVisuAndTransform.pathPenUp);
                e.Graphics.DrawPath(penMarker, GCodeVisuAndTransform.pathMarker);
                e.Graphics.DrawPath(penTool, GCodeVisuAndTransform.pathTool);
            }
        }
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {   // find closest coordinate in GCode and mark
            // MessageBox.Show(picAbsPosX + "  " + picAbsPosY);
            double last_distance = 999999999999;
            double distance;
            int last_linenr = 0;
            string singleLine;
            string tokens = "(X-?\\d+(.\\d+)?)|(Y-?\\d+(.\\d+)?)|(Z-?\\d+(.\\d+)?)|(I-?\\d+(.\\d+)?)|(J-?\\d+(.\\d+)?)";
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(tokens);
            System.Text.RegularExpressions.MatchCollection mc;
            double valx = 0, valy = 0, value = 0;
            double lastx = 0, lasty = 0;
            for (int i = 0; i < fCTBCode.LinesCount; i++)
            {
                singleLine = fCTBCode.Lines[i];
                mc = rex.Matches(singleLine.ToUpper());
                if ((singleLine.Length > 1) && (mc.Count > 0))
                {
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        int startIndex = m.Index;
                        int StopIndex = m.Length;
                        string matchText = singleLine.Substring(startIndex, StopIndex);
                        Double.TryParse(matchText.Substring(1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
                        if (m.Groups[1].Success || m.Groups[2].Success)             // X
                        { valx = value; }
                        if (m.Groups[3].Success || m.Groups[4].Success)             // Y
                        { valy = value; }
                    }
                    distance = Math.Sqrt((picAbsPosX - valx) * (picAbsPosX - valx) + (picAbsPosY - valy) * (picAbsPosY - valy));
                    if (distance < last_distance)
                    {
                        last_distance = distance;
                        last_linenr = i;
                        lastx = valx;
                        lasty = valy;
                    }
                }
            }
            fCTBCode.Selection = fCTBCode.GetLine(last_linenr);
            fCTBCodeClickedLineNow = last_linenr;
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();
        }

        private int findEndOfPath(int startLine, bool toEnd)
        {
            int endVal = fCTBCode.LinesCount;
            int lineNr = startLine;
            int lastNr = lineNr;
            string curLine;
            if (endVal < 2) return -1;
            if (toEnd)
            {   if (startLine > endVal) return -1; }
            else
            {   endVal = 0;
                if (startLine < endVal) return -1;                 
            }
            do
            {
                curLine = fCTBCode.Lines[lineNr];
                if ((curLine.IndexOf("X") >= 0) || (curLine.IndexOf("Y") >= 0))
                { lastNr = lineNr; }
                if ((curLine.IndexOf("Z") >= 0) || (curLine.IndexOf("G0") >= 0) || (curLine.IndexOf("M30") >= 0) || (curLine.IndexOf("F") >= 0))
                {
                    if (toEnd)
                        lastNr++;
                    return lastNr;
                }
                if (toEnd)
                { lineNr++; }
                else
                { lineNr--; }
            } while ((lineNr <= fCTBCode.LinesCount) || (lineNr > 0));
            return -1;
        }

        private void moveToFirstPosToolStripMenuItem_Click(object sender, EventArgs e)
        {   // rotate coordinates until marked line first
            int lineNr = fCTBCodeClickedLineNow;
            Range mySelection=fCTBCode.Range;
            Place selStart, selEnd;
            selStart.iLine = fCTBCodeClickedLineNow;
            selStart.iChar = 0;
            mySelection.Start = selStart;
            selEnd.iLine = lineNr;
            selEnd.iChar = 0;
            // select from marked line until end of this path - needs to be moved to front
            lineNr = findEndOfPath(fCTBCodeClickedLineNow,true);            // find end
            if (lineNr > 0)
            {   selEnd.iLine = lineNr;
                selEnd.iChar = 0;
                mySelection.End = selEnd;
                fCTBCode.Selection = mySelection;
                fCTBCode.SelectionColor = Color.Red;
                // find current begin of path, to insert selected code
                lineNr = findEndOfPath(fCTBCodeClickedLineNow, false);      // find start
                if (lineNr > 0)
                {   if (deleteMarkedCode)
                    {
                        fCTBCode.Cut();
                        selStart.iLine = lineNr;
                        selStart.iChar = 0;
                        selEnd.iLine = lineNr;
                        selEnd.iChar = 0;
                        mySelection.Start = selStart;
                        mySelection.End = selEnd;
                        fCTBCode.Selection = mySelection;
                        fCTBCode.Paste();
                        fCTBCodeClickedLineNow = lineNr;
                        fCTBCodeMarkLine();
                    }
                    fCTBCode.DoCaretVisible();
                    redrawGCodePath();
                    return;
                }
            }
            MessageBox.Show("Path start / end could not be identified");
        }

        private void deletePathToolStripMenuItem_Click(object sender, EventArgs e)
        {   // mark start to end of path and delete
            int lineNr = fCTBCodeClickedLineNow;
            Range mySelection = fCTBCode.Range;
            Place selStart, selEnd;
            selStart.iLine = fCTBCodeClickedLineNow;
            selStart.iChar = 0;
            mySelection.Start = selStart;
            selEnd.iLine = lineNr;
            selEnd.iChar = 0;
            // find start of path
            lineNr = findEndOfPath(fCTBCodeClickedLineNow, false);
            if (lineNr > 0)
            {   if (fCTBCode.Lines[lineNr].IndexOf("Z") >= 0) { lineNr--; }
                selStart.iLine = lineNr;
                selStart.iChar = 0;
                mySelection.Start = selStart;
                // find end of path
                lineNr = findEndOfPath(fCTBCodeClickedLineNow, true);
                if (lineNr > 0)
                {
                    if (fCTBCode.Lines[lineNr].IndexOf("Z") >= 0) { lineNr++; }
                    selEnd.iLine = lineNr;
                    selEnd.iChar = 0;
                    mySelection.End = selEnd;
                    fCTBCode.Selection = mySelection;
                    fCTBCode.SelectionColor = Color.Red;
                    fCTBCodeClickedLineNow = selStart.iLine;

                    if (deleteMarkedCode)
                    {
                        fCTBCode.Cut();
                        fCTBCodeMarkLine();
                    }
                    fCTBCode.DoCaretVisible();
                    redrawGCodePath();
                    return;
                }
            }
            MessageBox.Show("Path start / end could not be identified");
        }

        private void deleteThisCodeLineToolStripMenuItem_Click(object sender, EventArgs e)
        {   if (fCTBCode.LinesCount < 1) return;
            fCTBCodeClickedLineLast = 1;
            fCTBCodeMarkLine();
            if (deleteMarkedCode)
            {
                fCTBCode.Cut();
                fCTBCodeClickedLineNow--;
                fCTBCodeMarkLine();
            }
            fCTBCode.DoCaretVisible();
            redrawGCodePath();
            return;
        }

        private bool deleteMarkedCode = false;
        private void deletenotMarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (deleteMarkedCode)
            {   deleteMarkedCode = false;
                deletenotMarkToolStripMenuItem.Text = "Mark (not delete)";
            }
            else
            {
                deleteMarkedCode = true;
                deletenotMarkToolStripMenuItem.Text = "Delete (not mark)";
            }
        }

        private void cBTool_CheckedChanged(object sender, EventArgs e)
        {
            _serial_form.TooInSpindle = cBTool.Checked;
        }
    }
}
