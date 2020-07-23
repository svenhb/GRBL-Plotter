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
/*  Thanks to https://github.com/PavelTorgashov/FastColoredTextBox
*/
/* 2016-09-18 improve performance for low-performance PC: during streaming show background-image with toolpath
 *            instead of redrawing toolpath with each onPaint.
 *            Joystick-control: adjustable step-width and speed.
 * 2016-12-31 Add GRBL 1.1 function
 * 2017-01-01 check form-location and fix strange location
 * 2017-01-03 Add 'Replace M3 by M4' during GCode file open
 * 2017-06-22 Cleanup transform actions
 * 2018-01-02 Add Override Buttons
 *            Bugfix - no zooming during  streaming - disable background image (XP Problems?)
 * 2018-03-18 Divide this file into several files
 * 2018-12-26 Commits from RasyidUFA via Github
 * 2019-01-01 Pass CustomButton command "($abc)" through DIY-Control "[abc]"
 *            Add variable hotkeys
 * 2019-01-06 Remove feedback-loop at cBSpindle / cBCoolant, save last value for spindle-textbox
 * 2019-01-16 line 922 || !_serial_form.isHeightProbing
 * 2019-03-02 Swap code to MainFormGetCodeTransform
 * 2019-03-05 Add SplitContainer for Editor, resize Joystick controls
 * 2019-03-17 Add custom buttons 13-16, save size of form
 * 2019-04-23 use joyAStep in gamePadTimer_Tick and gamePadGCode line 1360, 1490
 * 2019-05-10 extend override features
 * 2019-10-27 localization of strings
 * 2019-12-22 Line 1891 check if xyWidth < 0;
 * 2020-01-13 convert GCodeVisuAndTransform to a static class
 * 2020-03-10 add gui.variable GMIX,Y,Z GMAX,Y,Z - graphics dimensions
 * 2020-03-12 outsourcing GamePad, SimulatePath
 * 2020-05-06 add status strip info during check for Prog-update
 */

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using virtualJoystick;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using GRBL_Plotter.GUI;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {
        ControlSerialForm _serial_form = null;
        ControlSerialForm _serial_form2 = null;
        Control2ndGRBL _2ndGRBL_form = null;
        ControlStreamingForm _streaming_form = null;
        ControlStreamingForm2 _streaming_form2 = null;
        ControlCameraForm _camera_form = null;
        ControlSetupForm _setup_form = null;
        ControlProbing _probing_form = null;
        ControlHeightMapForm _heightmap_form = null;
        ControlDIYControlPad _diyControlPad = null;
        ControlCoordSystem _coordSystem_form = null;
        ControlLaser _laser_form = null;
        splashscreen _splashscreen = null;

        GCodeFromText _text_form = null;
        GCodeFromImage _image_form = null;
        GCodeFromShape _shape_form = null;
        GCodeForBarcode _barcode_form = null;
        MessageForm _message_form = null;

        private const string appName = "GRBL Plotter";
        private const string fileLastProcessed = "lastProcessed";
        private xyzPoint posProbe = new xyzPoint(0, 0, 0);
        private double? alternateZ = null;
        private grblState machineStatus;
        public bool flagResetOffset = false;
        private double[] joystickXYStep = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickXYSpeed = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickZStep = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickZSpeed = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickAStep = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickASpeed = { 0, 1, 2, 3, 4, 5 };

        private bool ctrl4thAxis = false;
        private string ctrl4thName = "A";
        private string lastLoadSource = "Nothing loaded";
        private string lastLoadFile = "Nothing loaded";
        private int coordinateG = 54;
        private string zeroCmd = "G10 L20 P0";      // "G92"

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MainForm()
        {
            Logger.Info("++++++ GRBL-Plotter Ver. {0} START ++++++", Application.ProductVersion);
            _splashscreen = new splashscreen();
            _splashscreen.Show();
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Localization.UpdateLanguage(Properties.Settings.Default.guiLanguage);
            Logger.Info("Language: {0}", ci);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            gcode.setup();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
        }
        //Unhandled exception
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            this.Opacity = 100;
            if (_splashscreen != null)
            {   _splashscreen.Close();
                _splashscreen.Dispose();
            }
            Exception ex = e.Exception;
            Logger.Error(ex, "Application_ThreadException");
            MessageBox.Show(ex.Message + "\r\n\r\n" + GetAllFootprints(ex), "Main Form Thread exception");
            if (MessageBox.Show(Localization.getString("mainQuit"), Localization.getString("mainProblem"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            { Application.Exit(); }
        }
        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Opacity = 100;
            if (_splashscreen != null)
            {   _splashscreen.Close();
                _splashscreen.Dispose();
            }
            if (e.ExceptionObject != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                Logger.Error(ex, "UnhandledException - Quit GRBL Plotter?");
                MessageBox.Show(ex.Message + "\r\n\r\n" + GetAllFootprints(ex), "Main Form Application exception");
                if (MessageBox.Show(Localization.getString("mainQuit") + "\r\n\r\nCheck " + Application.StartupPath + "\\logfile.txt", Localization.getString("mainProblem"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                { Application.Exit(); }
            }
        }
        public string GetAllFootprints(Exception x)
        {
            var st = new StackTrace(x, true);
            var frames = st.GetFrames();
            var traceString = new StringBuilder();

            foreach (var frame in frames)
            {
                if (frame.GetFileLineNumber() < 1)
                    continue;

                traceString.Append("File: " + frame.GetFileName());
                traceString.Append(", Method:" + frame.GetMethod().Name);
                traceString.Append(", LineNumber: " + frame.GetFileLineNumber());
                traceString.Append("  -->  ");
            }
            return traceString.ToString();
        }

        // initialize Main form
        Dictionary<int, Button> CustomButtons17 = new Dictionary<int, Button>();
        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.Info("MainForm_Load start");
            if (Properties.Settings.Default.ctrlUpgradeRequired)
            {   Logger.Info("MainForm_Load - Properties.Settings.Default.ctrlUpgradeRequired");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ctrlUpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
            this.Icon = Properties.Resources.Icon;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            Location = Properties.Settings.Default.locationMForm;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }
            Size = Properties.Settings.Default.mainFormSize;

            WindowState = Properties.Settings.Default.mainFormWinState;
            splitContainer1.SplitterDistance = Properties.Settings.Default.mainFormSplitDistance;

            this.Text = appName + " Ver. " + System.Windows.Forms.Application.ProductVersion.ToString();

            loadToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            loadMachineParametersToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Control | Keys.O;
            saveMachineParametersToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Control | Keys.S;

            cmsCodeSelect.ShortcutKeys = Keys.Control | Keys.A;
            cmsCodeCopy.ShortcutKeys =  Keys.Control | Keys.C;
            cmsCodePaste.ShortcutKeys = Keys.Control | Keys.V;
            cmsCodeSendLine.ShortcutKeys = Keys.Alt | Keys.Control | Keys.M;

            foldCodeBlocks1stLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D2;
            foldCodeBlocks2ndLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D3;
            foldCodeBlocks3rdLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D4;
            expandCodeBlocksToolStripMenuItem1.ShortcutKeys = Keys.Alt | Keys.D1;

            cmsPicBoxMoveToMarkedPosition.ShortcutKeys = Keys.Control | Keys.M;
            cmsPicBoxPasteFromClipboard.ShortcutKeys = Keys.Control | Keys.V;

            toolStrip_tBRadiusCompValue.Text = string.Format("{0:0.000}", Properties.Settings.Default.crcValue);

            this.gBoxOverride.Click += gBoxOverride_Click;

            gBoxOverride.Height = 15;
            gBoxOverrideBig = false;

            lbDimension.Select(0, 0);

            CustomButtons17.Clear();
            for (int i = 17; i <= 32; i++)
            {
                Button b = new Button();
                b.Text = "b" + i;
                b.Name = "btnCustom" + i.ToString();
                b.Width = btnCustom1.Width - 20;
                b.Click += btnCustomButton_Click;
                CustomButtons17.Add(i, b);
                setCustomButton(b, Properties.Settings.Default["guiCustomBtn" + i.ToString()].ToString(), i);
                flowLayoutPanel1.Controls.Add(b);
            }

            loadSettings(sender, e);    // includes loadHotkeys();
                                        //           loadHotkeys();
            cmsPicBoxEnable(false);
            updateControls();
            LoadRecentList();
            cmsPicBoxReloadFile.ToolTipText = string.Format("Load '{0}'", MRUlist[0]);
            LoadExtensionList();
            foreach (string item in MRUlist)
            {   ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);  //create new menu for each item in list
                toolStripMenuItem2.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            }

            if (Properties.Settings.Default.guiCheckUpdate)
            {   statusStripSet(2,Localization.getString("statusStripeCheckUpdate"), Color.LightGreen);
                checkUpdate.CheckVersion();     // check update
            }
//            statusStripClear(2,2);

            grbl.init();                    // load and set grbl messages
            toolTable.init();               // fill structure

            gui.resetVariables();

            try { ControlGamePad.Initialize(); }
            catch { }
            Logger.Trace("MainForm_Load finish, start splashScreen timer");

            SplashScreenTimer.Enabled = true;
            SplashScreenTimer.Stop();
            SplashScreenTimer.Start();
   //         this.Opacity = 100;
        }

        private void SplashScreenTimer_Tick(object sender, EventArgs e)
        {
            if (_splashscreen != null)
            {
//                SplashScreenTimer.Enabled = false;
                this.Opacity = 100;
                if (_serial_form == null)
                {
                    if (Properties.Settings.Default.ctrlUseSerial2)
                    {
                        _serial_form2 = new ControlSerialForm("COM Tool changer", 2);
                        _serial_form2.Show(this);
                    }
                    _serial_form = new ControlSerialForm("COM CNC", 1, _serial_form2);
                    _serial_form.Show(this);
                    _serial_form.RaisePosEvent += OnRaisePosEvent;
                    _serial_form.RaiseStreamEvent += OnRaiseStreamEvent;
                }
                if (Properties.Settings.Default.ctrlUseSerialDIY)
                { DIYControlopen(sender, e); }
                _splashscreen.Close();
                _splashscreen.Dispose();
                _splashscreen = null;
                Logger.Info("++++++ MainForm SplashScreen closed");

                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    Logger.Info("Load file via CommandLineArgs[1] {0}", args[1]);
                    loadFile(args[1]);
                }
                SplashScreenTimer.Stop();
                SplashScreenTimer.Interval = 5000;
                SplashScreenTimer.Start();
            }
            else
            {   SplashScreenTimer.Enabled = false;
                statusStripClear(2, 2);
                Logger.Info("++++++ SplashScreenTimer disabled");
            }
        }

        // close Main form
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {   // Note all other forms will be closed, before reaching following code...
            Logger.Trace("FormClosing ");

            if (_serial_form != null)
            {   _serial_form.closePort();
                _serial_form.RaisePosEvent -= OnRaisePosEvent;
                _serial_form.RaiseStreamEvent -= OnRaiseStreamEvent;
            }

            Properties.Settings.Default.mainFormWinState = WindowState;
            WindowState = FormWindowState.Normal;
            Properties.Settings.Default.mainFormSize = Size;
            Properties.Settings.Default.locationMForm = Location;
            ControlPowerSaving.EnableStandby();
            Properties.Settings.Default.mainFormSplitDistance = splitContainer1.SplitterDistance;

            saveSettings();
            Logger.Info("++++++ GRBL-Plotter STOP ++++++", Application.ProductVersion);
        }

        // handle position events from serial form
        private void OnRaisePosEvent(object sender, PosEventArgs e)
        {
            //log.Add("MainForm OnRaisePosEvent");
            machineStatus = e.Status;
            if (e.StatMsg.Ov.Length > 1)    // check and submit override values
            {
                if (_streaming_form2 != null)
                { _streaming_form2.showOverrideValues(e.StatMsg.Ov); }

                string[] value = e.StatMsg.Ov.Split(',');
                if (value.Length > 2)
                { lblOverrideFRValue.Text = value[0];
                    lblOverrideRapidValue.Text = value[1];
                    lblOverrideSSValue.Text = value[2];
                }
            }
            if (e.StatMsg.FS.Length > 1)    // check and submit override values
            {
                if (_streaming_form2 != null)
                    _streaming_form2.showActualValues(e.StatMsg.FS);
                string[] value = e.StatMsg.FS.Split(',');
                if (value.Length > 1)
                { lblStatusFeed.Text = value[0];// + " mm/min";
                    lblStatusSpeed.Text = value[1];// + " RPM";
                }
                else
                { lblStatusFeed.Text = value[0];// + " mm/min";
                    lblStatusSpeed.Text = "-";// + " RPM";
                }
            }

            if (true)//e.StatMsg.A.Length > 0)
            {
                cBSpindle.CheckedChanged -= cBSpindle_CheckedChanged;
                cBCoolant.CheckedChanged -= cBCoolant_CheckedChanged;

                if (e.StatMsg.A.Contains("S"))
                { btnOverrideSpindle.Image = Properties.Resources.led_on;   // Spindle on CW
                    btnOverrideSpindle.Text = "Spindle CW";
                    cBSpindle.Checked = true;
                }
                if (e.StatMsg.A.Contains("C"))
                { btnOverrideSpindle.Image = Properties.Resources.led_on;   // Spindle on CCW
                    btnOverrideSpindle.Text = "Spindle CCW";
                    cBSpindle.Checked = true;
                }
                if (!e.StatMsg.A.Contains("S") && !e.StatMsg.A.Contains("C"))
                { btnOverrideSpindle.Image = Properties.Resources.led_off; cBSpindle.Checked = false; }  // Spindle off

                if (e.StatMsg.A.Contains("F")) { btnOverrideFlood.Image = Properties.Resources.led_on; cBCoolant.Checked = true; }   // Flood on
                else { btnOverrideFlood.Image = Properties.Resources.led_off; cBCoolant.Checked = false; }

                if (e.StatMsg.A.Contains("M")) { btnOverrideMist.Image = Properties.Resources.led_on; } // Mist on
                else { btnOverrideMist.Image = Properties.Resources.led_off; }

                cBCoolant.CheckedChanged += cBCoolant_CheckedChanged;
                cBSpindle.CheckedChanged += cBSpindle_CheckedChanged;
            }
            if (e.Status == grblState.probe)
            {
                posProbe = _serial_form.posProbe;
                if (_diyControlPad != null)
                { if (alternateZ != null)
                        posProbe.Z = (double)alternateZ;
                    //_diyControlPad.sendFeedback("Probe: "+posProbe.Z.ToString());
                }
                if (_heightmap_form != null)
                    _heightmap_form.setPosProbe = posProbe;

                if (_probing_form != null)
                { _probing_form.setPosProbe = grbl.getCoord("PRB");
                    Logger.Info("Update Probing {0}", grbl.displayCoord("PRB"));
                }
            }

            label_mx.Text = string.Format("{0:0.000}", grbl.posMachine.X);
            label_my.Text = string.Format("{0:0.000}", grbl.posMachine.Y);
            label_mz.Text = string.Format("{0:0.000}", grbl.posMachine.Z);
            label_wx.Text = string.Format("{0:0.000}", grbl.posWork.X);
            label_wy.Text = string.Format("{0:0.000}", grbl.posWork.Y);
            label_wz.Text = string.Format("{0:0.000}", grbl.posWork.Z);
            if (grbl.axisA)
            { label_ma.Text = string.Format("{0:0.000}", grbl.posMachine.A);
                label_wa.Text = string.Format("{0:0.000}", grbl.posWork.A);
            }
            if (grbl.axisB)
            { label_mb.Text = string.Format("{0:0.000}", grbl.posMachine.B);
                label_wb.Text = string.Format("{0:0.000}", grbl.posWork.B);
            }
            if (grbl.axisC)
            { label_mc.Text = string.Format("{0:0.000}", grbl.posMachine.C);
                label_wc.Text = string.Format("{0:0.000}", grbl.posWork.C);
            }

            if (flagResetOffset)            // Restore saved position after reset and set initial feed rate:
            {
                if (!_serial_form.checkGRBLSettingsOk())   // check 30 kHz limit
                {   statusStripSet(1,grbl.lastMessage, Color.Orange);
                    statusStripSet(2, Localization.getString("statusStripeCheckCOM"), Color.Yellow);
                }

                if (Properties.Settings.Default.resetRestoreWorkCoordinates)
                {
                    double x = Properties.Settings.Default.grblLastOffsetX;
                    double y = Properties.Settings.Default.grblLastOffsetY;
                    double z = Properties.Settings.Default.grblLastOffsetZ;
                    double a = Properties.Settings.Default.grblLastOffsetA;
                    double b = Properties.Settings.Default.grblLastOffsetB;
                    double c = Properties.Settings.Default.grblLastOffsetC;

                    Logger.Info("restoreWorkCoordinates [Setup - Flow control - Behavior after grbl reset]");
                    coordinateG = Properties.Settings.Default.grblLastOffsetCoord;
                    _serial_form.addToLog("* Restore last selected coordinate system:");
                    sendCommand(String.Format("G{0}", coordinateG));

                    _serial_form.addToLog("* Restore saved position after reset\r\n* and set initial feed rate:");
                    string setAxis = String.Format("{0} X{1} Y{2} Z{3} ", zeroCmd, x, y, z);

                    if (grbl.axisA)
                    {
                        if (ctrl4thAxis)
                            setAxis += String.Format("{0}{1} ", ctrl4thName, a);
                        else
                            setAxis += String.Format("A{0} ", a);
                    }
                    if (grbl.axisB)
                        setAxis += String.Format("B{0} ", b);
                    if (grbl.axisC)
                        setAxis += String.Format("C{0} ", c);

                    setAxis += String.Format("F{0}", Properties.Settings.Default.importGCXYFeed);

                    sendCommand(setAxis.Replace(',', '.'));
                }
                if (Properties.Settings.Default.resetSendCodeEnable)
                {   _serial_form.addToLog("* Code from [Setup - Flow control]");
                    _serial_form.addToLog("* Code after reset: " + Properties.Settings.Default.resetSendCode);
                    processCommands(Properties.Settings.Default.resetSendCode);
                }

                setGRBLBuffer();
                flagResetOffset = false;
                updateControls();
            }

            processStatus();                    // grblState {idle, run, hold, home, alarm, check, door}
            processParserState(e.parserState);  // parser state 
            if (grbl.posChanged)
            { VisuGCode.createMarkerPath();
                VisuGCode.updatePathPositions();
                checkMachineLimit();
                pictureBox1.Invalidate();
                grbl.posChanged = false;
            }
            if (grbl.wcoChanged)
            { checkMachineLimit();
                grbl.wcoChanged = false;
            }
            if (_diyControlPad != null)
            { if (oldRaw != e.Raw)
                { _diyControlPad.sendFeedback(e.Raw);     //hand over original grbl text
                    oldRaw = e.Raw;
                }
            }
            if (((isStreaming || isStreamingRequestStop)) && Properties.Settings.Default.guiProgressShow)
                VisuGCode.ProcessedPath.processedPathDraw(grbl.posWork);

        }
        private string oldRaw = "";
        // handle status events from serial form
        private grblState lastMachineStatus = grblState.unknown;
        private string lastInfoText = "";
        private string lastLabelInfoText = "";
        private bool updateDrawingPath = false;
        private void processStatus() // {idle, run, hold, home, alarm, check, door}
        {
            if ((machineStatus != lastMachineStatus) || (grbl.lastMessage.Length > 5))
            {
                label_status.Text = grbl.statusToText(machineStatus);
                label_status.BackColor = grbl.grblStateColor(machineStatus);
                switch (machineStatus)
                {
                    case grblState.idle:
                        if ((lastMachineStatus == grblState.hold) || (lastMachineStatus == grblState.alarm))
                        {
                            statusStripClear(1,2);
                            lbInfo.Text = lastInfoText;
                            lbInfo.BackColor = SystemColors.Control;
                            if (!_serial_form.checkGRBLSettingsOk())   // check 30 kHz limit
                            {   statusStripSet(1,grbl.lastMessage, Color.Fuchsia);
                                statusStripSet(2,Localization.getString("statusStripeCheckCOM"), Color.Yellow);
                            }
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        cBTool.Checked = _serial_form.toolInSpindle;
                        if (signalLock > 0)
                        { btnKillAlarm.BackColor = SystemColors.Control; signalLock = 0; }
                        if (!isStreaming)                       // update drawing if G91 is used
                            updateDrawingPath = true;

  //                      statusStripClear(1,2);
                        grbl.lastMessage = "";

                        break;
                    case grblState.run:
                        if (lastMachineStatus == grblState.hold)
                        {
                            statusStripClear();
                            lbInfo.Text = lastInfoText;
                            lbInfo.BackColor = SystemColors.Control;
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        break;
                    case grblState.hold:
                        btnResume.BackColor = Color.Yellow;
                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = Localization.getString("mainInfoResume");     //"Press 'Resume' to proceed";
                        lbInfo.BackColor = Color.Yellow;
                        statusStripClear();
                        statusStripSet(1,grbl.statusToText(machineStatus), grbl.grblStateColor(machineStatus));
                        statusStripSet(2,lbInfo.Text, lbInfo.BackColor);
                        if (signalResume == 0) { signalResume = 1; }
                        break;
                    case grblState.home:
                        break;
                    case grblState.alarm:
                        signalLock = 1;
                        btnKillAlarm.BackColor = Color.Yellow;
                        lbInfo.Text = Localization.getString("mainInfoKill");     //"Press 'Kill Alarm' to proceed";
                        lbInfo.BackColor = Color.Yellow;
      //                  statusStripClear(1,2);
                        statusStripSet(1,grbl.statusToText(machineStatus) + " " + grbl.lastMessage, grbl.grblStateColor(machineStatus));
                        statusStripSet(2,lbInfo.Text, lbInfo.BackColor);
                        grbl.lastMessage = "";
                        if (_heightmap_form != null)
                            _heightmap_form.stopScan();
                        break;
                    case grblState.check:
                        break;
                    case grblState.door:
                        btnResume.BackColor = Color.Yellow;
                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = Localization.getString("mainInfoResume");     //"Press 'Resume' to proceed";
                        lbInfo.BackColor = Color.Yellow;
        //                statusStripClear(1,2);
                        statusStripSet(1,grbl.statusToText(machineStatus), grbl.grblStateColor(machineStatus));
                        statusStripSet(2,lbInfo.Text, lbInfo.BackColor);
                        if (signalResume == 0) { signalResume = 1; }
                        break;
                    case grblState.probe:
                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = string.Format("{0}: X:{1:0.00} Y:{2:0.00} Z:{3:0.00}", Localization.getString("mainInfoProbing"), posProbe.X, posProbe.Y, posProbe.Z);
                        lbInfo.BackColor = Color.Yellow;
                        break;
                    default:
                        break;
                }
                if (_probing_form != null)
                {   _probing_form.setGrblSaState = machineStatus; }

            }
            lastMachineStatus = machineStatus;
        }

        // handle last sent commands from serial form
        private string actualFR = "";
        private string actualSS = "";
        private void processParserState(pState cmd)//string cmd)
        {
            if (cmd.changed)
            {
                actualFR = cmd.FR.ToString();
                if (_streaming_form != null)
                    _streaming_form.show_value_FR(actualFR);
                actualSS = cmd.SS.ToString();
                if (_streaming_form != null)
                    _streaming_form.show_value_SS(actualSS);

                if (grbl.isVersion_0)
                { cBSpindle.CheckedChanged -= cBSpindle_CheckedChanged;
                    cBSpindle.Checked = (cmd.spindle <= 4) ? true : false;  // M3, M4 start, M5 stop
                    cBSpindle.CheckedChanged += cBSpindle_CheckedChanged;
                    cBCoolant.CheckedChanged -= cBCoolant_CheckedChanged;
                    cBCoolant.Checked = (cmd.coolant <= 8) ? true : false;  // M7, M8 on   M9 coolant off
                    cBCoolant.CheckedChanged += cBCoolant_CheckedChanged;
                }
                if (cmd.toolchange)
                    lblTool.Text = cmd.tool.ToString();

                lblCurrentG.Text = "G" + cmd.coord_select.ToString();
                lblCurrentG.BackColor = (cmd.coord_select == 54) ? Color.Lime : Color.Fuchsia;
                if (_camera_form != null)
                    _camera_form.setCoordG = cmd.coord_select;
                if (_coordSystem_form != null)
                { _coordSystem_form.markActiveCoordSystem(lblCurrentG.Text);
                    _coordSystem_form.updateTLO(cmd.TLOactive, cmd.tool_length);
                }
            }
        }

        // send command via serial form
        private void sendRealtimeCommand(int cmd)
        { _serial_form.realtimeCommand((byte)cmd); }
        private void sendRealtimeCommand(byte cmd)
        { _serial_form.realtimeCommand(cmd); }

        // send command via serial form
        private void sendCommand2(string txt)
        {
            if (!_serial_form.requestSend(txt, true))     // check if COM is still open
                updateControls();

            if ((txt.Contains("G92") || txt.Contains("G10") || txt.Contains("G43")) && (_coordSystem_form != null))
                _coordSystem_form.refreshValues();
        }
        private void sendCommand(string txt, bool jogging = false)
        {
            if ((jogging) && (grbl.isVersion_0 == false))
                txt = "$J=" + txt;
            txt = gui.insertVariable(txt);
            if (!_serial_form.requestSend(txt))     // check if COM is still open
                updateControls();

            if ((txt.Contains("G92") || txt.Contains("G10") || txt.Contains("G43")) && (_coordSystem_form != null))
                _coordSystem_form.refreshValues();
        }

        private void OnRaiseOverrideMessage(object sender, OverrideMsgArgs e)   // command from streaming_form2 - Override
        { sendRealtimeCommand(e.MSG); }

        // get override events from form "StreamingForm" for GRBL 0.9
        private string overrideMessage = "";
        private void OnRaiseOverrideEvent(object sender, OverrideEventArgs e)
        {
            if (e.Source == overrideSource.feedRate)
                _serial_form.injectCode("F", (int)e.Value, e.Enable);
            if (e.Source == overrideSource.spindleSpeed)
                _serial_form.injectCode("S", (int)e.Value, e.Enable);

            overrideMessage = "";
            if (e.Enable)
                overrideMessage = " !!! Override !!!";
            lbInfo.Text = lastLabelInfoText + overrideMessage;
        }

        // Main-Menu File outsourced to MaiFormLoadFile.cs

        #region MAIN-MENU Machine control

        private void controlStreamingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (grbl.isVersion_0)
            {
                if (_streaming_form2 != null)
                    _streaming_form2.Visible = false;
                if (_streaming_form == null)
                {
                    _streaming_form = new ControlStreamingForm();
                    _streaming_form.FormClosed += formClosed_StreamingForm;
                    _streaming_form.RaiseOverrideEvent += OnRaiseOverrideEvent;      // assign  event
                    _streaming_form.show_value_FR(actualFR);
                    _streaming_form.show_value_SS(actualSS);
                }
                else
                {
                    _streaming_form.Visible = false;
                }
                _streaming_form.Show(this);
                _streaming_form.WindowState = FormWindowState.Normal;

            }
            else
            {
                if (_streaming_form != null)
                    _streaming_form.Visible = false;
                if (_streaming_form2 == null)
                {
                    _streaming_form2 = new ControlStreamingForm2();
                    _streaming_form2.FormClosed += formClosed_StreamingForm;
                    _streaming_form2.RaiseOverrideEvent += OnRaiseOverrideMessage;      // assign  event
                }
                else
                {
                    _streaming_form2.Visible = false;
                }
                _streaming_form2.Show(this);
                _streaming_form2.WindowState = FormWindowState.Normal;
            }
        }
        private void formClosed_StreamingForm(object sender, FormClosedEventArgs e)
        { _streaming_form = null; _streaming_form2 = null; }

        private void control2ndGRBLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_2ndGRBL_form == null)
            {
                _2ndGRBL_form = new Control2ndGRBL(_serial_form2);
                if (_serial_form2 == null)
                {
                    _serial_form2 = new ControlSerialForm("COM Tool changer", 2);
                    _serial_form2.Show(this);
                }
                _2ndGRBL_form.set2ndSerial(_serial_form2);
                _serial_form.set2ndSerial(_serial_form2);
                _2ndGRBL_form.FormClosed += formClosed_2ndGRBLForm;
            }
            else
            {
                _2ndGRBL_form.Visible = false;
            }
            _2ndGRBL_form.Show(this);
            _2ndGRBL_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_2ndGRBLForm(object sender, FormClosedEventArgs e)
        { _2ndGRBL_form = null; }
        // open Camera form
        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_camera_form == null)
            {
                _camera_form = new ControlCameraForm();
                _camera_form.FormClosed += formClosed_CameraForm;
                _camera_form.RaiseXYEvent += OnRaiseCameraClickEvent;
                //               _camera_form.setPosMarker(grbl.posMarker);// visuGCode.GetPosMarker());
            }
            else
            {
                _camera_form.Visible = false;
            }
            _camera_form.Show(this);
            _camera_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_CameraForm(object sender, FormClosedEventArgs e)
        { _camera_form = null; }


        // DIY ControlPad _diyControlPad
        private void DIYControlopen(object sender, EventArgs e)
        {
            if (_diyControlPad == null)
            {
                _diyControlPad = new ControlDIYControlPad();
                _diyControlPad.FormClosed += formClosed_DIYControlForm;
                _diyControlPad.RaiseStreamEvent += OnRaiseDIYCommandEvent;
            }
            else
            {
                _diyControlPad.Visible = false;
            }
            _diyControlPad.Show(this);
            _diyControlPad.WindowState = FormWindowState.Normal;
        }
        private void formClosed_DIYControlForm(object sender, FormClosedEventArgs e)
        { _diyControlPad = null; }
        private void OnRaiseDIYCommandEvent(object sender, CommandEventArgs e)
        {
            if (e.RealTimeCommand > 0x00)
            { if (!isStreaming || isStreamingPause)
                    sendRealtimeCommand(e.RealTimeCommand);
            }
            else
            { if ((!isStreaming || isStreamingPause) && !_serial_form.isHeightProbing)    // only hand over DIY commands in normal mode
                    sendCommand(e.Command);
                if (e.Command.StartsWith("(PRB:Z"))
                {
                    string num = e.Command.Substring(6);
                    double myZ;
                    num = num.Trim(')');
                    alternateZ = null;
                    if (double.TryParse(num, out myZ))
                    { alternateZ = myZ; }
                    else
                    { _diyControlPad.sendFeedback("Error in parsing " + num, true); }
                }
            }
        }

        // Coordinates
        private void coordSystemopen(object sender, EventArgs e)
        {
            if (_coordSystem_form == null)
            {
                _coordSystem_form = new ControlCoordSystem();
                _coordSystem_form.FormClosed += formClosed_CoordSystemForm;
                _coordSystem_form.RaiseCmdEvent += OnRaiseCoordSystemEvent;
            }
            else
            {
                _coordSystem_form.Visible = false;
            }
            _coordSystem_form.Show(this);
            _coordSystem_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_CoordSystemForm(object sender, FormClosedEventArgs e)
        { _coordSystem_form = null; }
        private void OnRaiseCoordSystemEvent(object sender, CmdEventArgs e)
        {
            sendCommand(e.Command);
        }

        // Laser
        private void laseropen(object sender, EventArgs e)
        {
            if (_laser_form == null)
            {
                _laser_form = new ControlLaser();
                _laser_form.FormClosed += formClosed_LaserForm;
                _laser_form.RaiseCmdEvent += OnRaiseLaserEvent;
            }
            else
            {
                _laser_form.Visible = false;
            }
            _laser_form.Show(this);
            _laser_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_LaserForm(object sender, FormClosedEventArgs e)
        { _laser_form = null; }
        private void OnRaiseLaserEvent(object sender, CmdEventArgs e)
        {
            if (!_serial_form.requestSend(e.Command, true))     // check if COM is still open
                updateControls();
			Properties.Settings.Default.counterUseLaserSetup += 1;
        }

        // edge finder / Probing
        private void edgeFinderopen(object sender, EventArgs e)
        {
            if (_probing_form == null)
            {
                _probing_form = new ControlProbing();
                _probing_form.FormClosed += formClosed_ProbingForm;
                _probing_form.RaiseCmdEvent += OnRaiseProbingEvent;
                _probing_form.btnGetAngleEF.Click += btnGetAngleEF_Click;
            }
            else
            {
                _probing_form.Visible = false;
            }
            _probing_form.Show(this);
            _probing_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_ProbingForm(object sender, FormClosedEventArgs e)
        { _probing_form = null; }
        private void btnGetAngleEF_Click(object sender, EventArgs e)
        {
            if ((VisuGCode.xyzSize.dimx > 0) && (VisuGCode.xyzSize.dimy > 0))
            {
                transformStart("Rotate");
                fCTBCode.Text = VisuGCode.transformGCodeRotate(_probing_form.getAngle, 1, new xyPoint(0, 0));
                transformEnd();
            }
        }
        private void OnRaiseProbingEvent(object sender, CmdEventArgs e)
        {
            string[] commands;
            commands = e.Command.Split(';');
            if (!_serial_form.serialPortOpen)
                return;
            foreach (string btncmd in commands)
            {
                sendCommand(btncmd.Trim());
            }

            updateControls();
            Properties.Settings.Default.counterUseProbing += 1;
        }


        // Height Map
        private void heightMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_heightmap_form == null)
            {
                _heightmap_form = new ControlHeightMapForm();
                _heightmap_form.FormClosed += formClosed_HeightmapForm;
                _heightmap_form.btnStartHeightScan.Click += getGCodeScanHeightMap;      // assign btn-click event
                _heightmap_form.loadHeightMapToolStripMenuItem.Click += loadHeightMap;
                _heightmap_form.btnApply.Click += applyHeightMap;
                _heightmap_form.RaiseXYZEvent += OnRaisePositionClickEvent;
                _heightmap_form.btnGCode.Click += getGCodeFromHeightMap;      // assign btn-click event
            }
            else
            {
                _heightmap_form.Visible = false;
            }
            _heightmap_form.Show(this);
            _heightmap_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_HeightmapForm(object sender, FormClosedEventArgs e)
        {
            _heightmap_form = null;
            VisuGCode.clearHeightMap();
            _serial_form.stopStreaming();
        }

        // open Setup form
        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_setup_form == null)
            {
                _setup_form = new ControlSetupForm();
                _setup_form.FormClosed += formClosed_SetupForm;
                _setup_form.btnApplyChangings.Click += loadSettings;
                _setup_form.btnReloadFile.Click += reStartConvertFile;
                _setup_form.btnMoveToolXY.Click += moveToPickup;
                _setup_form.setLastLoadedFile(lastLoadSource);
                gamePadTimer.Enabled = false;
            }
            else
            {
                _setup_form.Visible = false;
            }
            _setup_form.Show(null);// this);
            _setup_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_SetupForm(object sender, FormClosedEventArgs e)
        {
            loadSettings(sender, e);
            _setup_form = null;
            VisuGCode.drawMachineLimit(toolTable.getToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
            gamePadTimer.Enabled = Properties.Settings.Default.gamePadEnable;
        }
        #endregion
        // open About form
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }

        private void showLaserMode()
        {
            if (!grbl.isVersion_0 && _serial_form.isLasermode)
            {
                lbInfo.Text = Localization.getString("mainInfoLaserModeOn"); // "Laser Mode active $32=1";
                lbInfo.BackColor = Color.Fuchsia;
            }
            else
            {
                lbInfo.Text = Localization.getString("mainInfoLaserModeOff");  //"Laser Mode not active $32=0";
                lbInfo.BackColor = Color.Lime;
            }
        }

        #region Streaming
        // handle file streaming
        TimeSpan elapsed;               //elapsed time from file burnin
        DateTime timeInit;              //time start to burning file
        private int signalResume = 0;   // blinking button
        private int signalLock = 0;     // blinking button
        private int signalPlay = 0;     // blinking button
        private int delayedSend = 0;
        private int streamingPauseFirstLine = 0;
        private bool isStreaming = false;
        private bool isStreamingPause = false;
        private bool isStreamingCheck = false;
        private bool isStreamingRequestStop = false;
        private bool isStreamingOk = true;
        private bool isStreamingPauseFirst = false;
        private void OnRaiseStreamEvent(object sender, StreamEventArgs e)
        {
            int cPrgs = (int)Math.Max(0, Math.Min(100, e.CodeProgress));
            int bPrgs = (int)Math.Max(0, Math.Min(100, e.BuffProgress));
            pbFile.Value = cPrgs;
            pbBuffer.Value = bPrgs;
            lblFileProgress.Text = string.Format("Progress {0:0.0}%", e.CodeProgress);
            int actualCodeLine = e.CodeLineSent;
            if (e.CodeLineSent > fCTBCode.LinesCount)
                actualCodeLine = fCTBCode.LinesCount - 1;
            fCTBCode.Selection = fCTBCode.GetLine(actualCodeLine);
            fCTBCodeClickedLineNow = e.CodeLineSent - 1;
            fCTBCodeMarkLine();         // set Bookmark and marker in 2D-View
            fCTBCode.DoCaretVisible();
            if (_diyControlPad != null)
                _diyControlPad.sendFeedback("[" + e.Status.ToString() + "]");
            
            if (Properties.Settings.Default.guiProgressShow)
                VisuGCode.ProcessedPath.processedPathLine(e.CodeLineConfirmed);
            switch (e.Status)
            {
                case grblStreaming.lasermode:
                    showLaserMode();
                    break;
                case grblStreaming.reset:
                    flagResetOffset = true;
                    stopStreaming(false);
                    if (e.CodeProgress < 0)
                    {   lbInfo.Text = _serial_form.lastError;
                        lbInfo.BackColor = Color.Fuchsia;
                    }
                    else
                    {   lbInfo.Text = "Vers. " + _serial_form.grblVers;
                        lbInfo.BackColor = Color.Lime;
                    }
                    statusStripClear(1, 2);
                    toolTip1.SetToolTip(lbInfo, lbInfo.Text);
                    updateControls();
                    if (_coordSystem_form != null)
                        _coordSystem_form.showValues();

                    ControlPowerSaving.EnableStandby();
                    VisuGCode.ProcessedPath.processedPathClear();
                    break;
                case grblStreaming.error:
                    Logger.Info("streaming error at line {0}", e.CodeLineSent);
                    statusStripSet(1,grbl.lastMessage, Color.Fuchsia);
                    isStreaming = false;
                    isStreamingCheck = false;
                    pbFile.ForeColor = Color.Red;
                    lbInfo.Text = Localization.getString("mainInfoErrorLine") + e.CodeLineSent.ToString();
                    lbInfo.BackColor = Color.Fuchsia;
                    fCTBCode.BookmarkLine(actualCodeLine - 1);
                    fCTBCode.DoSelectionVisible();
                    fCTBCode.CurrentLineColor = Color.Red;
                    isStreamingOk = false;
                    break;
                case grblStreaming.ok:
                    if (!isStreamingCheck)
                    {
                        updateControls();
                        lbInfo.Text = Localization.getString("mainInfoSendCode") + "(" + e.CodeLineSent.ToString() + ")";
                        lbInfo.BackColor = Color.Lime;
                        signalPlay = 0;
                        btnStreamStart.BackColor = SystemColors.Control;
 //                       VisuGCode.Simulation.createSimulationPath(VisuGCode.Simulation.getLinePos(e.CodeLine));
                    }
                    break;
                case grblStreaming.finish:
                    Logger.Info("streaming finished ok {0}", isStreamingOk);
                    if (isStreamingOk)
                    {
                        if (isStreamingCheck)
                        { lbInfo.Text = Localization.getString("mainInfoFinishCheck"); }   // "Finish checking G-Code"; }
                        else
                        { lbInfo.Text = Localization.getString("mainInfoFinishSend"); }   // "Finish sending G-Code"; }
                        lbInfo.BackColor = Color.Lime;
                        pbFile.Value = 0;
                        pbBuffer.Value = 0;
                    }
                    isStreaming = false; isStreamingCheck = false;
                    btnStreamStart.Image = Properties.Resources.btn_play;
                    btnStreamStart.Enabled = true;
                    btnStreamCheck.Enabled = true;
                    showPicBoxBgImage = false;                     // don't show background image anymore
                    pictureBox1.BackgroundImage = null;
                    updateControls();
                    ControlPowerSaving.EnableStandby();
                    VisuGCode.ProcessedPath.processedPathClear();
                    break;
                case grblStreaming.waitidle:
                    updateControls(true);
                    btnStreamStart.Image = Properties.Resources.btn_play;
                    isStreamingPause = true;
                    lbInfo.Text = Localization.getString("mainInfoWaitIdle") + e.CodeLineSent.ToString() + ")";
                    lbInfo.BackColor = Color.Yellow;
                    break;
                case grblStreaming.pause:
                    updateControls(true);
                    btnStreamStart.Image = Properties.Resources.btn_play;
                    isStreamingPause = true;
                    lbInfo.Text = Localization.getString("mainInfoPause") + e.CodeLineSent.ToString() + ")";
                    signalPlay = 1;
                    lbInfo.BackColor = Color.Yellow;
                    isStreamingPauseFirst = (streamingPauseFirstLine != e.CodeLineSent);
                    // save parser state on pause
                    saveStreamingStatus(e.CodeLineSent);
                    streamingPauseFirstLine = e.CodeLineSent;

                    if (isStreamingPauseFirst && Properties.Settings.Default.flowControlEnable) // send extra Pause-Code
                        delayedSend = 2;

                    if (isStreamingPauseFirst && fCTBCode.Lines[fCTBCodeClickedLineNow].Contains("(T"))
                    { string msg = Localization.getString("mainToolChange1") + fCTBCode.Lines[fCTBCodeClickedLineNow] + Localization.getString("mainToolChange2");
                        MessageBox.Show(msg, Localization.getString("mainToolChange"));
                    }

                    break;
                case grblStreaming.toolchange:
                    updateControls();
                    btnStreamStart.Image = Properties.Resources.btn_play;
                    lbInfo.Text = Localization.getString("mainInfoToolChange");
                    lbInfo.BackColor = Color.Yellow;
                    cBTool.Checked = _serial_form.toolInSpindle;
                    break;
                case grblStreaming.stop:
                    lbInfo.Text = Localization.getString("mainInfoStopStream") + e.CodeLineSent.ToString() + ")";
                    lbInfo.BackColor = Color.Fuchsia;

                    if (Properties.Settings.Default.flowControlEnable) // send extra Pause-Code
                        delayedSend = 2;
  //                  VisuGCode.ProcessedPath.processedPathClear();
                    break;
                default:
                    break;
            }

            lastLabelInfoText = lbInfo.Text;
            lbInfo.Text += overrideMessage;
        }
        private void btnStreamStart_Click(object sender, EventArgs e)
        { startStreaming(); }
        // if startline > 0 start with pause
        private void startStreaming(int startLine = 0)
        {
            isStreamingRequestStop = false;
            if (fCTBCode.LinesCount > 1)
            {
                if (!isStreaming)
                {
                    Logger.Info("Start streaming at line:{0}  showProgress:{1}  backgroundImage:{2}", startLine, Properties.Settings.Default.guiProgressShow, Properties.Settings.Default.guiBackgroundImageEnable);
                    VisuGCode.ProcessedPath.processedPathClear();
                    isStreaming = true;
                    isStreamingPause = false;
                    isStreamingCheck = false;
                    isStreamingOk = true;
                    streamingPauseFirstLine = 0;
                    VisuGCode.markSelectedFigure(0);
                    if (startLine > 0)
                    { btnStreamStart.Image = Properties.Resources.btn_pause;
                        isStreamingPause = true;
                    }

                    if (!grbl.isVersion_0)
                    { gBoxOverride.Height = 175;
                        gBoxOverrideBig = true;
                    }

                    updateControls();
                    timeInit = DateTime.UtcNow;
                    elapsed = TimeSpan.Zero;
                    lbInfo.Text = Localization.getString("mainInfoSendCode");// "Send G-Code";
                    lbInfo.BackColor = Color.Lime;
                    for (int i = 0; i < fCTBCode.LinesCount; i++)
                        fCTBCode.UnbookmarkLine(i);

                    //save gcode
                    string fileName = Application.StartupPath + "\\" + fileLastProcessed;
                    string txt = fCTBCode.Text;
                    File.WriteAllText(fileName + ".nc", txt);
                    File.Delete(fileName + ".xml");
                    SaveRecentFile(fileLastProcessed + ".nc");

                    lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
                    _serial_form.startStreaming(fCTBCode.Lines, startLine);
                    btnStreamStart.Image = Properties.Resources.btn_pause;
                    btnStreamCheck.Enabled = false;
                    onPaint_setBackground();
                    ControlPowerSaving.SuppressStandby();
                }
                else
                {
                    if (!isStreamingPause)
                    {
                        Logger.Info("Pause streaming - pause stream");
                        btnStreamStart.Image = Properties.Resources.btn_play;
                        _serial_form.pauseStreaming();
                        isStreamingPause = true;
                        statusStripSet(0,Localization.getString("statusStripeStreamingStatusSaved"), Color.LightGreen);
                    }
                    else
                    {
                        Logger.Info("Pause streaming - continue stream");
                        btnStreamStart.Image = Properties.Resources.btn_pause;
                        _serial_form.pauseStreaming();
                        isStreamingPause = false;
                        statusStripClear(0);
                    }
                }
            }
        }
        private void btnStreamCheck_Click(object sender, EventArgs e)
        {
            if ((fCTBCode.LinesCount > 1) && (!isStreaming))
            {
                Logger.Info("check code");
                isStreaming = true;
                isStreamingCheck = true;
                isStreamingOk = true;
                updateControls();
                timeInit = DateTime.UtcNow;
                elapsed = TimeSpan.Zero;
                lbInfo.Text = Localization.getString("mainInfoCheckCode");// "Check G-Code";
                lbInfo.BackColor = SystemColors.Control;
                for (int i = 0; i < fCTBCode.LinesCount; i++)
                    fCTBCode.UnbookmarkLine(i);
                _serial_form.startStreaming(fCTBCode.Lines, 0, true);
                btnStreamStart.Enabled = false;
                onPaint_setBackground();
            }
        }
        private void btnStreamStop_Click(object sender, EventArgs e)
        { stopStreaming(); }
        private void stopStreaming(bool showMessage=true)
        {   Logger.Info("stop streaming at line {0}", (fCTBCodeClickedLineNow + 1));
            showPicBoxBgImage = false;                 // don't show background image anymore
//            pictureBox1.BackgroundImage = null;
            btnStreamStart.Image = Properties.Resources.btn_play;
            btnStreamStart.BackColor = SystemColors.Control;
            btnStreamStart.Enabled = true;
            btnStreamCheck.Enabled = true;
            isStreamingRequestStop = true;
            _serial_form.stopStreaming(showMessage);
            if (isStreaming || isStreamingCheck)
            {
                lbInfo.Text = Localization.getString("mainInfoStopStream2") + (fCTBCodeClickedLineNow + 1).ToString() + " )";
                lbInfo.BackColor = Color.Fuchsia;
            }
            if (!isStreaming)
            {   VisuGCode.ProcessedPath.processedPathClear();
                pictureBox1.Invalidate();
                pbFile.Value = 0;
                pbBuffer.Value = 0;
                signalPlay = 0;
                isStreamingRequestStop = false;
            }
            isStreaming = false;
            isStreamingCheck = false;
            ControlPowerSaving.EnableStandby();
            updateControls();
        }
        private void btnStreamPause_Click(object sender, EventArgs e)
        { _serial_form.pauseStreaming(); }
        #endregion

        // update 500ms
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (isStreaming)
            {   elapsed = DateTime.UtcNow - timeInit;
                lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
            }
            else
            {
                if (updateDrawingPath && VisuGCode.containsG91Command())
                {
                    //redrawGCodePath();
                    pictureBox1.Invalidate(); // will be called by parent function
                }
                updateDrawingPath = false;

                if (Properties.Settings.Default.flowCheckRegistryChange)
                {
                    int update = 0;
                    const string reg_key = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";
                    try
                    {   update = (int)Registry.GetValue(reg_key, "update", 0);
                        Registry.SetValue(reg_key, "update", 0);
                    }
                    catch (Exception er) { Logger.Error(er, "Reading reg-key update"); }

                    if (update > 0)
                    {   Logger.Trace("Automatic update from clipboard");
                        loadFromClipboard();
                        enableCmsCodeBlocks(VisuGCode.codeBlocksAvailable());
						Properties.Settings.Default.counterImportExtension += 1;
                    }
                }
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
            if (delayedSend > 0)
            {   if (delayedSend-- == 1)
                {   _serial_form.addToLog("* Code from [Setup - Flow control]");
                    _serial_form.addToLog("* Code after pause/stop: " + Properties.Settings.Default.flowControlText);
                    processCommands(Properties.Settings.Default.flowControlText);
                }
            }
            if (grbl.lastMessage.Length > 3)
            {   if (grbl.lastMessage.ToLower().Contains("missing"))
                {   statusStripClear();
                    statusStripSet(1,grbl.lastMessage, Color.Red);
                    statusStripSet(2, Localization.getString("statusStripeResetNeeded"), Color.Yellow);
                }
                else if (grbl.lastMessage.ToLower().Contains("reset"))
                {   label_status.Text = "";
                    label_status.BackColor = SystemColors.Control;
                    statusStripClear(2);
                    statusStripSet(1,grbl.lastMessage, Color.Yellow);
                    if (grbl.lastMessage.ToLower().Contains("hard"))
                        statusStripSet(2,Localization.getString("statusStripeGrblResetNeeded"), Color.Yellow);
                }
                else
                    statusStripSet(1,grbl.lastMessage, Color.Yellow);
                grbl.lastMessage = "";
            }
        }

        private void setGRBLBuffer()
        {
            if (!grbl.axisUpdate && (grbl.axisCount > 0))
            {
                grbl.axisUpdate = true;
                if (Properties.Settings.Default.grblBufferAutomatic)
                {
                    if (grbl.bufferSize > 0)
                    {
                        grbl.RX_BUFFER_SIZE = (grbl.bufferSize != 128) ? grbl.bufferSize : 127;
                        _serial_form.addToLog("* Read buffer size of " + grbl.RX_BUFFER_SIZE + " bytes");
                        Logger.Info("Read buffer size of {0} [Setup - Flow control - grbl buffer size]", grbl.RX_BUFFER_SIZE);
                    }
                    else
                    {
                        if (grbl.axisCount > 3)
                            grbl.RX_BUFFER_SIZE = 255;
                        else
                            grbl.RX_BUFFER_SIZE = 127;

                        _serial_form.addToLog("* Assume buffer size of " + grbl.RX_BUFFER_SIZE + " bytes");
                        Logger.Info("Assume buffer size of {0} [Setup - Flow control - grbl buffer size]", grbl.RX_BUFFER_SIZE);
                    }
                }
                else
                {  //if (grbl.RX_BUFFER_SIZE != 127)
                    _serial_form.addToLog("* Buffer size was manually set to " + grbl.RX_BUFFER_SIZE + " bytes!\r* Check [Setup - Flow control]");
                    Logger.Info("Buffer size was manually set to {0} [Setup - Flow control - grbl buffer size]", grbl.RX_BUFFER_SIZE);
                }
                statusStripSet(1,string.Format("grbl-controller connected: vers: {0}, axis: {1}, buffer: {2}", _serial_form.grblVers, grbl.axisCount, grbl.RX_BUFFER_SIZE), Color.Lime);
            }
        }
        // handle positon click event from camera form
        private void OnRaisePositionClickEvent(object sender, XYZEventArgs e)
        {
            if (e.Command.IndexOf("G91") >= 0)
            {
                string final = e.Command;
                if (e.PosX != null)
                    final += string.Format(" X{0}", gcode.frmtNum((float)e.PosX));
                if (e.PosY != null)
                    final += string.Format(" Y{0}", gcode.frmtNum((float)e.PosY));
                if (e.PosZ != null)
                    final += string.Format(" Z{0}", gcode.frmtNum((float)e.PosZ));
                sendCommand(final.Replace(',', '.'), true);
            }
        }
        private void OnRaiseCameraClickEvent(object sender, XYEventArgs e)
        {
            if (e.Command == "a")
            {
                if (fCTBCode.LinesCount > 1)
                {
                    routeTransformCode(e.Angle, e.Scale, e.Point);
                    VisuGCode.setPosMarkerLine(fCTBCodeClickedLineNow);
                }
            }
            else
            {
                double realStepX = Math.Round(e.Point.X, 3);
                double realStepY = Math.Round(e.Point.Y, 3);
                int speed = 1000;
                string s = "";
                string[] line = e.Command.Split(';');
                foreach (string cmd in line)
                {
                    if (cmd.Trim() == "G92")
                    {
                        s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        sendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G0") >= 0) || (cmd.Trim().IndexOf("G1") >= 0))        // no jogging
                    {
                        s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        sendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G90") == 0) || (cmd.Trim().IndexOf("G91") == 0))      // no G0 G1, then jogging
                    {
                        speed = 100 + (int)Math.Sqrt(realStepX * realStepX + realStepY * realStepY) * 120;
                        s = String.Format("F{0} " + cmd + " X{1} Y{2}", speed, realStepX, realStepY).Replace(',', '.');
                        sendCommand(s, true);
                    }
                    else
                    {
                        sendCommand(cmd.Trim());
                    }
                }
            }
        }
        public void routeTransformCode(double angle, double scale, xyPoint offset)
        {
            fCTBCode.Text = VisuGCode.transformGCodeRotate(angle, scale, offset);
            update_GCode_Depending_Controls();
            return;
        }


        #region GUI Objects

        // Setup Custom Buttons during loadSettings()
        string[] btnCustomCommand = new string[33];
        private int setCustomButton(Button btn, string text, int cnt)
        {
            int index = Convert.ToUInt16(btn.Name.Substring("btnCustom".Length));
            if (text.Contains("|"))
            { string[] parts = text.Split('|');
                btn.Text = parts[0];
                if ((parts[1].Length > 4) && File.Exists(parts[1]))
                { toolTip1.SetToolTip(btn, parts[0] + "\r\nFile: " + parts[1] + "\r\n" + File.ReadAllText(parts[1])); }
                else
                { toolTip1.SetToolTip(btn, parts[0] + "\r\n" + parts[1]); }
                btnCustomCommand[index] = parts[1];
                return parts[0].Trim().Length + parts[1].Trim().Length;
            }
            else
                btnCustomCommand[index] = "";
            return 0;
        }
        private void btnCustomButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int index = Convert.ToUInt16(clickedButton.Name.Substring("btnCustom".Length));
            processCommands(btnCustomCommand[index]);
        }


        // virtualJoystic sends two step-width-values per second. One position should be reached before next command
        // speed (units/min) = 2 * stepsize * 60 * factor (to compensate speed-ramps)
        private int virtualJoystickXY_lastIndex = 1;
        private int virtualJoystickZ_lastIndex = 1;
        private int virtualJoystickA_lastIndex = 1;
        private void virtualJoystickXY_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Down:
                case Keys.Up:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void virtualJoystickXY_JoyStickEvent(object sender, JogEventArgs e)
        { virtualJoystickXY_move(e.JogPosX, e.JogPosY); }
        private void virtualJoystickXY_move(int index_X, int index_Y)
        {
            int indexX = Math.Abs(index_X);
            int indexY = Math.Abs(index_Y);
            int dirX = Math.Sign(index_X);
            int dirY = Math.Sign(index_Y);
            virtualJoystickXY_lastIndex = Math.Max(indexX, indexY);
            if (indexX > joystickXYStep.Length)
            { indexX = joystickXYStep.Length; index_X = indexX; }
            if (indexX < 0)
            { indexX = 0; index_X = 0; }
            if (indexY > joystickXYStep.Length)
            { indexY = joystickXYStep.Length; index_Y = indexY; }
            if (indexY < 0)
            { indexY = 0; index_Y = 0; }
            int speed = (int)Math.Max(joystickXYSpeed[indexX], joystickXYSpeed[indexY]);
            String strX = gcode.frmtNum(joystickXYStep[indexX] * dirX);
            String strY = gcode.frmtNum(joystickXYStep[indexY] * dirY);
            String s = "";
            if (speed > 0)
            { if (Properties.Settings.Default.machineLimitsAlarm && Properties.Settings.Default.machineLimitsShow)
                {
                    if (!VisuGCode.xyzSize.withinLimits(grbl.posMachine, joystickXYStep[indexX] * dirX, joystickXYStep[indexY] * dirY))
                    {
                        decimal minx = Properties.Settings.Default.machineLimitsHomeX;
                        decimal maxx = minx + Properties.Settings.Default.machineLimitsRangeX;
                        decimal miny = Properties.Settings.Default.machineLimitsHomeY;
                        decimal maxy = miny + Properties.Settings.Default.machineLimitsRangeY;

                        string tmp = string.Format("minX: {0:0.0} moveTo: {1:0.0} maxX: {2:0.0}", minx, (grbl.posMachine.X + joystickXYStep[indexX] * dirX), maxx);
                        tmp += string.Format("\r\nminY: {0:0.0} moveTo: {1:0.0} maxY: {2:0.0}", miny, (grbl.posMachine.Y + joystickXYStep[indexY] * dirY), maxy);
                        System.Media.SystemSounds.Beep.Play();
                        DialogResult dialogResult = MessageBox.Show(Localization.getString("mainLimits1") + tmp + Localization.getString("mainLimits2"), Localization.getString("mainAttention"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                        if (dialogResult == DialogResult.Cancel)
                            return;
                    }
                }
                //if ((e.JogPosX == 0) && (e.JogPosY == 0))
                //    s = String.Format("(stop)");// sendRealtimeCommand(133); 
                if (index_X == 0)
                    s = String.Format("G91 Y{0} F{1}", strY, speed).Replace(',', '.');
                else if (index_Y == 0)
                    s = String.Format("G91 X{0} F{1}", strX, speed).Replace(',', '.');
                else
                    s = String.Format("G91 X{0} Y{1} F{2}", strX, strY, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }
        private void virtualJoystickXY_MouseUp(object sender, MouseEventArgs e)
        { if (!grbl.isVersion_0 && cBSendJogStop.Checked) sendRealtimeCommand(133); }
        private void btnJogStop_Click(object sender, EventArgs e)
        { if (!grbl.isVersion_0) sendRealtimeCommand(133); }    //0x85

        private void virtualJoystickXY_Enter(object sender, EventArgs e)
        { if (grbl.isVersion_0) sendCommand("G91G1F100");
            gB_Jogging.BackColor = Color.LightGreen;
        }
        private void virtualJoystickXY_Leave(object sender, EventArgs e)
        { if (grbl.isVersion_0) sendCommand("G90");
            gB_Jogging.BackColor = SystemColors.Control;
            virtualJoystickXY.JoystickRasterMark = 0;
            virtualJoystickZ.JoystickRasterMark = 0;
            virtualJoystickA.JoystickRasterMark = 0;
            virtualJoystickB.JoystickRasterMark = 0;
            virtualJoystickC.JoystickRasterMark = 0;
        }
        private void virtualJoystickZ_JoyStickEvent(object sender, JogEventArgs e)
        { virtualJoystickZ_move(e.JogPosY); }
        private void virtualJoystickZ_move(int index_Z)
        {
            int indexZ = Math.Abs(index_Z);
            int dirZ = Math.Sign(index_Z);
            if (indexZ > joystickZStep.Length)
            { indexZ = joystickZStep.Length; index_Z = indexZ; }
            if (indexZ < 0)
            { indexZ = 0; index_Z = 0; }

            virtualJoystickZ_lastIndex = indexZ;
            int speed = (int)joystickZSpeed[indexZ];
            String strZ = gcode.frmtNum(joystickZStep[indexZ] * dirZ);
            if (speed > 0)
            {
                String s = String.Format("G91 Z{0} F{1}", strZ, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }
        private void virtualJoystickA_JoyStickEvent(object sender, JogEventArgs e)
        { virtualJoystickA_move(e.JogPosY, ctrl4thName); }
        private void virtualJoystickA_move(int index_A, string name)
        { int indexA = Math.Abs(index_A);
            int dirA = Math.Sign(index_A);
            if (indexA > joystickAStep.Length)
            { indexA = joystickAStep.Length; index_A = indexA; }
            if (indexA < 0)
            { indexA = 0; index_A = 0; }

            virtualJoystickA_lastIndex = indexA;
            int speed = (int)joystickASpeed[indexA];
            String strZ = gcode.frmtNum(joystickAStep[indexA] * dirA);
            if (speed > 0)
            {
                String s = String.Format("G91 {0}{1} F{2}", name, strZ, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }
        private void virtualJoystickB_JoyStickEvent(object sender, JogEventArgs e)
        { virtualJoystickA_move(e.JogPosY, "B"); }
        private void virtualJoystickC_JoyStickEvent(object sender, JogEventArgs e)
        { virtualJoystickA_move(e.JogPosY, "C"); }

        // Spindle and coolant
        private void cBSpindle_CheckedChanged(object sender, EventArgs e)
        {
            string m = "M3";
            if (Properties.Settings.Default.importGCSDirM3)
                cBSpindle.Text = "Spindle CW";
            else
            { cBSpindle.Text = "Spindle CCW"; m = "M4"; }

            if (cBSpindle.Checked)
            { sendCommand(m + " S" + tBSpeed.Text); }
            else
            { sendCommand("M5"); }
        }

        private void btnPenUp_Click(object sender, EventArgs e)
        {   sendCommand(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMUp));}

        private void btnPenDown_Click(object sender, EventArgs e)
        {   sendCommand(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMDown)); }

        private void cBCoolant_CheckedChanged(object sender, EventArgs e)
        {   if (cBCoolant.Checked)
            { sendCommand("M8"); }
            else
            { sendCommand("M9"); }
        }
        private void btnHome_Click(object sender, EventArgs e)
        { sendCommand("$H"); }
        private void btnZeroX_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " X0"); }
        private void btnZeroY_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " Y0"); }
        private void btnZeroZ_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " Z0"); }
        private void btnZeroA_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " " + ctrl4thName + "0"); }
        private void btnZeroB_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " B0"); }
        private void btnZeroC_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " C0"); }
        private void btnZeroXY_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " X0 Y0"); }
        private void btnZeroXYZ_Click(object sender, EventArgs e)
        { sendCommand(zeroCmd + " X0 Y0 Z0"); }

        private void btnJogX_Click(object sender, EventArgs e)
        {   if (cBMoveG0.Checked)
                sendCommand("G0 G90 X0");
            else
                sendCommand("G90 X0 F" + joystickXYSpeed[5].ToString(), true); }
        private void btnJogY_Click(object sender, EventArgs e)
        {
            if (cBMoveG0.Checked)
                sendCommand("G0 G90 Y0");
            else
                sendCommand("G90 Y0 F" + joystickXYSpeed[5].ToString(), true); }
        private void btnJogZ_Click(object sender, EventArgs e)
        {
            if (cBMoveG0.Checked)
                sendCommand("G0 G90 Z0");
            else
                sendCommand("G90 Z0 F" + joystickZSpeed[5].ToString(), true); }
        private void btnJogZeroA_Click(object sender, EventArgs e)
        {
            if (cBMoveG0.Checked)
                sendCommand("G0 G90 " + ctrl4thName + "0");
            else
                sendCommand("G90 " + ctrl4thName + "0 F" + joystickZSpeed[5].ToString(), true); }
        private void btnJogXY_Click(object sender, EventArgs e)
        {
            if (cBMoveG0.Checked)
                sendCommand("G0 G90 X0 Y0");
            else
                sendCommand("G90 X0 Y0 F" + joystickXYSpeed[5].ToString(), true); }

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
            cBSpindle.CheckedChanged -= cBSpindle_CheckedChanged;
            cBSpindle.Checked = false;
            cBSpindle.CheckedChanged += cBSpindle_CheckedChanged;
            cBCoolant.CheckedChanged -= cBSpindle_CheckedChanged;
            cBCoolant.Checked = false;
            cBCoolant.CheckedChanged += cBSpindle_CheckedChanged;
            updateControls();
            ControlPowerSaving.EnableStandby();
        }
        private void btnFeedHold_Click(object sender, EventArgs e)
        { grblFeedHold(); }
        private void grblFeedHold()
        { sendRealtimeCommand('!');
            Logger.Trace("FeedHold");
            signalResume = 1;
            updateControls(true);
        }
        private void btnResume_Click(object sender, EventArgs e)
        { grblResume(); }
        private void grblResume()
        { sendRealtimeCommand('~');
            Logger.Trace("Resume");
            btnResume.BackColor = SystemColors.Control;
            signalResume = 0;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            updateControls();
        }
        private void btnKillAlarm_Click(object sender, EventArgs e)
        { grblKillAlarm(); }
        private void grblKillAlarm()
        { sendCommand("$X");
            Logger.Trace("KillAlarm");
            signalLock = 0;
            btnKillAlarm.BackColor = SystemColors.Control;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            updateControls();
        }
        #endregion

        //        public GCodeVisuAndTransform visuGCode = new GCodeVisuAndTransform();

        private void cBTool_CheckedChanged(object sender, EventArgs e)
        { _serial_form.toolInSpindle = cBTool.Checked;   }

        private void btnOverrideFR0_Click(object sender, EventArgs e)
        { sendRealtimeCommand(144); }     // 0x90 : Set 100% of programmed rate.    
        private void btnOverrideFR1_Click(object sender, EventArgs e)
        { sendRealtimeCommand(145); }     // 0x91 : Increase 10%        
        private void btnOverrideFR4_Click(object sender, EventArgs e)
        { sendRealtimeCommand(146); }     // 0x92 : Decrease 10%   
        private void btnOverrideFR2_Click(object sender, EventArgs e)
        { sendRealtimeCommand(147); }     // 0x93 : Increase 1%   
        private void btnOverrideFR3_Click(object sender, EventArgs e)
        { sendRealtimeCommand(148); }     // 0x94 : Decrease 1%   

        private void btnOverrideRapid0_Click(object sender, EventArgs e)
        { sendRealtimeCommand(149); }     // 0x95 : Set to 100% full rapid rate.
        private void btnOverrideRapid1_Click(object sender, EventArgs e)
        { sendRealtimeCommand(150); }     // 0x96 : Set to 50% of rapid rate.
        private void btnOverrideRapid2_Click(object sender, EventArgs e)
        { sendRealtimeCommand(151); }     // 0x97 : Set to 25% of rapid rate.

        private void btnOverrideSS0_Click(object sender, EventArgs e)
        { sendRealtimeCommand(153); }     // 0x99 : Set 100% of programmed spindle speed    
        private void btnOverrideSS1_Click(object sender, EventArgs e)
        { sendRealtimeCommand(154); }     // 0x9A : Increase 10%        
        private void btnOverrideSS4_Click(object sender, EventArgs e)
        { sendRealtimeCommand(155); }     // 0x9B : Decrease 10%   
        private void btnOverrideSS2_Click(object sender, EventArgs e)
        { sendRealtimeCommand(156); }     // 0x9C : Increase 1%   
        private void btnOverrideSS3_Click(object sender, EventArgs e)
        { sendRealtimeCommand(157); }     // 0x9D : Decrease 1%   

        private void btnOverrideSpindle_Click(object sender, EventArgs e)
        { sendRealtimeCommand(158); }     // 0x9E : Toggle Spindle Stop
        private void btnOverrideFlood_Click(object sender, EventArgs e)
        { sendRealtimeCommand(160); }     // 0xA0 : Toggle Flood Coolant  
        private void btnOverrideMist_Click(object sender, EventArgs e)
        { sendRealtimeCommand(161); }     // 0xA1 : Toggle Mist Coolant 

        private void btnOverrideDoor_Click(object sender, EventArgs e)
        { sendRealtimeCommand(132); }     // 0x84 : Safety Door  


        private void processCommands(string command)
        { if (command.Length <= 1)
                return;
            string[] commands;
            //            Logger.Trace("processCommands");
            if (!command.StartsWith("(") && File.Exists(command))
            {
                string fileCmd = File.ReadAllText(command);
                _serial_form.addToLog("* File: " + command);
                commands = fileCmd.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }
            else
            { commands = command.Split(';');
            }
            if (_diyControlPad != null)
            { _diyControlPad.isHeightProbing = false; }

            foreach (string btncmd in commands)
            { if (btncmd.StartsWith("($") && (_diyControlPad != null))
                {
                    string tmp = btncmd.Replace("($", "[");
                    tmp = tmp.Replace(")", "]");
                    _diyControlPad.sendFeedback(tmp);
                }
                else
                { if (!processSpecialCommands(command))
                        sendCommand(btncmd.Trim());         // processCommands
                }
            }
        }
        private bool processSpecialCommands(string command)
        {
            //            Logger.Trace("processSpecialCommands");
            bool commandFound = false;
            if (command.ToLower().IndexOf("#start") >= 0) { btnStreamStart_Click(this, EventArgs.Empty); commandFound = true; }
            else if (command.ToLower().IndexOf("#stop") >= 0) { btnStreamStop_Click(this, EventArgs.Empty); commandFound = true; }
            else if (command.ToLower().IndexOf("#f100") >= 0) { sendRealtimeCommand(144); commandFound = true; }
            else if (command.ToLower().IndexOf("#f+10") >= 0) { sendRealtimeCommand(145); commandFound = true; }
            else if (command.ToLower().IndexOf("#f-10") >= 0) { sendRealtimeCommand(146); commandFound = true; }
            else if (command.ToLower().IndexOf("#f+1") >= 0) { sendRealtimeCommand(147); commandFound = true; }
            else if (command.ToLower().IndexOf("#f-1") >= 0) { sendRealtimeCommand(148); commandFound = true; }
            else if (command.ToLower().IndexOf("#s100") >= 0) { sendRealtimeCommand(153); commandFound = true; }
            else if (command.ToLower().IndexOf("#s+10") >= 0) { sendRealtimeCommand(154); commandFound = true; }
            else if (command.ToLower().IndexOf("#s-10") >= 0) { sendRealtimeCommand(155); commandFound = true; }
            else if (command.ToLower().IndexOf("#s+1") >= 0) { sendRealtimeCommand(156); commandFound = true; }
            else if (command.ToLower().IndexOf("#s-1") >= 0) { sendRealtimeCommand(157); commandFound = true; }
            else if (command.ToLower().IndexOf("#hrst") >= 0) { _serial_form.grblHardReset(); commandFound = true; }
            else if (command.ToLower().IndexOf("#rst") >= 0) { _serial_form.grblReset(); commandFound = true; }
            else if (command.ToLower().IndexOf("#feedhold") >= 0) { grblFeedHold(); commandFound = true; }
            else if (command.ToLower().IndexOf("#resume") >= 0) { grblResume(); commandFound = true; }
            else if (command.ToLower().IndexOf("#killalarm") >= 0) { grblKillAlarm(); commandFound = true; }

            return commandFound;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        { pictureBox1.Focus(); }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        { pictureBox1.Focus(); }

        private void fCTBCode_MouseHover(object sender, EventArgs e)
        { fCTBCode.Focus(); }

        private void toolStrip_tb_StreamLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                int lineNr; ;
                if (int.TryParse(toolStrip_tb_StreamLine.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out lineNr))
                { startStreaming(lineNr);      // 1142
                }
                else
                { MessageBox.Show(Localization.getString("mainParseError1"), Localization.getString("mainAttention"));
                    toolStrip_tb_StreamLine.Text = "0";
                }
            }
        }

        private void updateView(object sender, EventArgs e)
        {
            Properties.Settings.Default.gui2DRulerShow = toolStripViewRuler.Checked;
            Properties.Settings.Default.gui2DInfoShow = toolStripViewInfo.Checked;
            Properties.Settings.Default.gui2DPenUpShow = toolStripViewPenUp.Checked;
            Properties.Settings.Default.machineLimitsShow = toolStripViewMachine.Checked;
            Properties.Settings.Default.gui2DToolTableShow = toolStripViewTool.Checked;
            Properties.Settings.Default.guiDimensionShow = toolStripViewDimension.Checked;
            Properties.Settings.Default.guiBackgroundShow = toolStripViewBackground.Checked;
            Properties.Settings.Default.machineLimitsFix = toolStripViewMachineFix.Checked;
            zoomFactor = 1;
            VisuGCode.drawMachineLimit(toolTable.getToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
        }

   /*     private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
    //        MessageBox.Show(log.get());
    //        log.clear();
        }*/

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                resizeJoystick();
            for (int i = 17; i <= 32; i++)
            {   if (CustomButtons17.ContainsKey(i))
                {   Button b = CustomButtons17[i];
                    b.Width = btnCustom1.Width - 24;
                    b.Height = btnCustom1.Height;
                }
            }
        }
        private void resizeJoystick()
        {   int virtualJoystickSize = Properties.Settings.Default.guiJoystickSize;
            int zRatio = 25;                    // 20% of xyJoystick width
            int zCount = 1;
            // grbl.axisB = true;
            // grbl.axisC = true;
            if (ctrl4thAxis || grbl.axisA) zCount = 2;
            if (grbl.axisB) { zCount = 3; zRatio = 25; }
            if (grbl.axisC) { zCount = 4; zRatio = 25; }
            int spaceY = this.Height - 520;     // width is 125% or 150%    485
            int spaceX = this.Width - 670;      // heigth is 100%
            spaceX = Math.Max(spaceX, 235);     // minimum width is 235px

            int aWidth = 0, bWidth = 0, cWidth = 0;
            int zWidth = (spaceX * zRatio / (100 + zCount * zRatio));           // 
            zWidth = Math.Min(zWidth, virtualJoystickSize * zRatio / 100);
            int xyWidth = spaceX - zCount * zWidth;
            if (xyWidth < 0) xyWidth = 0;

            tLPRechtsUntenRechtsMitte.ColumnStyles[1].Width = zWidth;       // Z
            virtualJoystickA.Visible = false;
            virtualJoystickB.Visible = false;
            virtualJoystickC.Visible = false;
            if (ctrl4thAxis || grbl.axisA)
            { aWidth = zWidth; virtualJoystickA.Visible = true; }
            if (grbl.axisB)
            { aWidth = bWidth = zWidth; virtualJoystickB.Visible = true; }
            if (grbl.axisC)
            { aWidth = bWidth = cWidth = zWidth; virtualJoystickC.Visible = true; }

            tLPRechtsUntenRechtsMitte.ColumnStyles[2].Width = aWidth;       // A
            tLPRechtsUntenRechtsMitte.ColumnStyles[3].Width = bWidth;       // B
            tLPRechtsUntenRechtsMitte.ColumnStyles[4].Width = cWidth;       // C

            xyWidth = Math.Min(xyWidth, spaceY);
            xyWidth = Math.Min(xyWidth, virtualJoystickSize);
            xyWidth = Math.Max(xyWidth, 100);

            spaceX = Math.Min(xyWidth + zWidth + aWidth + bWidth + cWidth + 10, spaceX);
            spaceX = Math.Max(spaceX, 235);
            tLPRechtsUntenRechts.Width = spaceX;
            tLPRechtsUntenRechtsMitte.Width = spaceX;

            tLPRechtsUntenRechtsMitte.Height = xyWidth;
            tLPRechtsUntenRechtsMitte.ColumnStyles[0].Width = xyWidth;
            virtualJoystickXY.Size = new Size(xyWidth, xyWidth);
            virtualJoystickZ.Size = new Size(zWidth, xyWidth);
            virtualJoystickA.Size = new Size(aWidth, xyWidth);
            virtualJoystickB.Size = new Size(bWidth, xyWidth);
            virtualJoystickC.Size = new Size(cWidth, xyWidth);
//            toolStripStatusLabel1.Text = string.Format("[Resize Form XY:{0} Z:{1} A:{2} B:{3} C:{4} spaceX:{5}]", xyWidth, zWidth, aWidth, bWidth, cWidth, spaceX);
        }


        // adapt size of controls
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        { int add = splitContainer1.Panel1.Width - 296;
            pbFile.Width = 194 + add;
            pbBuffer.Left = 219 + add;
            btnSimulate.Width = 194 + add;
            btnSimulateFaster.Left = 202 + add;
            btnSimulateSlower.Left = 244 + add;
            btnSimulatePause.Left = 158 + add;
            gBOverrideFRGB.Width = 284 + add;
            gBOverrideSSGB.Width = 284 + add;
            gBOverrideASGB.Width = 284 + add;
            gBOverrideRGB.Width = 284 + add;

            lbInfo.Width = 280 + add;
            lbDimension.Width = 130 + add;
            btnLimitExceed.Left = 112 + add;
            groupBox4.Left = 133 + add;
        }

        private bool gBoxOverrideBig = false;
        private void gBoxOverride_Click(object sender, EventArgs e)
        {
            if (gBoxOverrideBig)
                gBoxOverride.Height = 15;
            else
                gBoxOverride.Height = 175;
            gBoxOverrideBig = !gBoxOverrideBig;
        }

        public void setUndoText(string txt)
        {
            if (txt.Length > 1)
            {
                unDoToolStripMenuItem.Text = txt;
                unDoToolStripMenuItem.Enabled = true;
                unDo2ToolStripMenuItem.Text = txt;
                unDo2ToolStripMenuItem.Enabled = true;
                unDo3ToolStripMenuItem.Text = txt;
                unDo3ToolStripMenuItem.Enabled = true;
            }
            else
            {
                unDoToolStripMenuItem.Text = Localization.getString("mainInfoUndo");    // "Undo last action";
                unDoToolStripMenuItem.Enabled = false;
                unDo2ToolStripMenuItem.Text = Localization.getString("mainInfoUndo");    //"Undo last action";
                unDo2ToolStripMenuItem.Enabled = false;
                unDo3ToolStripMenuItem.Text = Localization.getString("mainInfoUndo");    //"Undo last action";
                unDo3ToolStripMenuItem.Enabled = false;
            }
        }


        private void cBServoButtons_CheckedChanged(object sender, EventArgs e)
        {
            btnPenUp.Visible = cBServoButtons.Checked;
            btnPenDown.Visible = cBServoButtons.Checked;
        }

        private void statusStripSet(int nr, string text, Color color)
        {
            if (nr == 0)
            {
                toolStripStatusLabel0.Text = "[ " + text + " ]";
                toolStripStatusLabel0.BackColor = color;
            }
            else if (nr == 1)

            {
                toolStripStatusLabel1.Text = "[ " + text + " ]";
                toolStripStatusLabel1.BackColor = color;
            }
            else if (nr == 2)
            {
                toolStripStatusLabel2.Text = "[ " + text + " ]";
                toolStripStatusLabel2.BackColor = color;
            }
        }
        private void statusStripSetToolTip(int nr, string text)
        {
            if (nr == 0)
            {   toolStripStatusLabel0.ToolTipText = text; }
            else if (nr == 1)
            {   toolStripStatusLabel1.ToolTipText = text; }
            else if (nr == 2)
            {   toolStripStatusLabel2.ToolTipText = text; }
        }

        private void statusStripClear(int nr1, int nr2=-1)
        {   if ((nr1 == 0) || (nr2 == 0))
            { toolStripStatusLabel0.Text = ""; toolStripStatusLabel0.BackColor = SystemColors.Control; toolStripStatusLabel0.ToolTipText = ""; }
            if ((nr1 == 1) || (nr2 == 1))
            { toolStripStatusLabel1.Text = ""; toolStripStatusLabel1.BackColor = SystemColors.Control; toolStripStatusLabel1.ToolTipText = ""; }
            if ((nr1 == 2) || (nr2 == 2))
            { toolStripStatusLabel2.Text = ""; toolStripStatusLabel2.BackColor = SystemColors.Control; toolStripStatusLabel2.ToolTipText = ""; }
        }
        private void statusStripClear()
        {   toolStripStatusLabel0.Text = toolStripStatusLabel1.Text = toolStripStatusLabel2.Text = "";
            toolStripStatusLabel0.BackColor = toolStripStatusLabel1.BackColor = toolStripStatusLabel2.BackColor = SystemColors.Control;
        }

        private void copyContentTroClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g = Graphics.FromImage(img);
            Point pB1 = PointToScreen(pictureBox1.Location); 
            Point pB2 = new Point(pB1.X+splitContainer1.Panel2.Left+ tLPRechtsUnten.Left+6, pB1.Y+tLPRechts.Location.Y+ splitContainer1.Panel2.Top+ tLPRechtsUnten.Top+30);
            g.CopyFromScreen(pB2, new Point(0, 0), new Size(pictureBox1.Width, pictureBox1.Height));

            Clipboard.SetImage(img);

            g.Dispose();
        }
    }
}

