/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2025 Sven Hasemann contact: svenhb@web.de

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
 * 2020-09-18 split
 * 2020-12-28 add Marlin support
 * 2021-01-13 add 3rd serial com
 * 2021-01-20 move code for camera handling from here to 'MainFormGetCodetransform'
 * 2021-02-01 line 802 change 0 to 0.000
 * 2021-02-24 save Pen up/down buttons visibillity status
 * 2021-05-19 processCommands line 975 also load ini file
 * 2021-07-14 code clean up / code quality
 * 2021-11-11 track prog-start and -end
 * 2021-11-17 show path-nodes gui2DShowVertexEnable - will be switched off on prog-start - line 146
 * 2021-11-18 add processing of accessory D0-D3 from grbl-Mega-5X - line 976
 * 2021-12-14 line 613 remove else...
 * 2022-04-19 check  if (toolStripStatusLabel0 == null) 
 * 2023-01-02 CbLaser_CheckedChanged, CbSpindle_CheckedChanged check if Grbl.isConnected
 * 2023-03-07 l:714/786/811 f:VirtualJoystickXY/Z/A_move if index =0 stop Jog -> if (!Grbl.isVersion_0) SendRealtimeCommand(133);
 * 2023-03-09 l:1213 bugfix start streaming
 * 2023-05-30 l:532  f:MainTimer_Tick add _message_form close
 * 2023-09-11 l:270  f:SplashScreenTimer_Tick multiple file import and issue #360 -> new function MainFormLoadFile.cs - LoadFiles(string[] fileList, int minIndex)
 * 2024-05-19 l:1159 f:BtnReset_Click removed StopStreaming to avoid applying code after "stop" from flowControlText
 * 2024-05-28 l:625  f:MainTimer_Tick add delayedHeightMapShow timer

*/

using GrblPlotter.GUI;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using virtualJoystick;


namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        ControlSerialForm _serial_form = null;
        Splashscreen _splashscreen = null;

        MessageForm _message_form = null;

        private const string appName = "GRBL Plotter";
        private const string fileLastProcessed = "lastProcessed";
        private XyzPoint posProbe = new XyzPoint(0, 0, 0);
        private double? alternateZ = null;
        internal bool ResetDetected = false;
        private readonly double[] joystickXYStep = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickXYSpeed = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickZStep = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickZSpeed = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickAStep = { 0, 1, 2, 3, 4, 5 };
        private readonly double[] joystickASpeed = { 0, 1, 2, 3, 4, 5 };

        private bool ctrl4thAxis = false;
        private string ctrl4thName = "A";
        private string lastLoadSource = "Nothing loaded";
        private string lastLoadFile = "Nothing loaded";
        private int coordinateG = 54;
        private readonly string zeroCmd = "G10 L20 P0";      // "G92"
        private ulong mainTimerCount = 0;

        private bool showFormInFront = false;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logDetailed = false;
        private static readonly bool logPosEvent = false;
        private static bool logStreaming = false;



        public MainForm()
        {   // Use the Constructor in a Windows Form for ensuring that initialization is done properly.
            // Use load event: code that requires the window size and location to be known.

            GetAppDataPath();           // find AppDataPath

            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Localization.UpdateLanguage(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            UpdateLogging();            // set logging flags

            Logger.Trace("###### START GRBL-Plotter Ver. {0} {1}  ######", MyApplication.GetVersion(), MyApplication.GetLinkerTimestampUtc(System.Reflection.Assembly.GetExecutingAssembly()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
            Logger.Info(culture, "###### START GRBL-Plotter Ver. {0} {1} Language: {2}   OS: {3} ######", MyApplication.GetVersion(), MyApplication.GetCompilationDate(), ci, System.Environment.OSVersion);
            Logger.Info("Info {0}", Properties.Settings.Default.guiLastEndReason);
            EventCollector.Init();

            InitializeComponent();      // controls
            RemoveCursorNavigation(this.Controls);

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);

            this.Icon = Properties.Resources.Icon;  // set icon

            // Attention: no MessageBox during splashScreen: never visible and application waits for action!
            Logger.Info(culture, "++++++ MainForm SplashScreen start");
            _splashscreen = new Splashscreen();		// shows splash screen
            _splashscreen.Show();                   // will be closed if SplashScreenTimer >= 1500

            if (Properties.Settings.Default.ctrlUpgradeRequired)		// check if update of settings are needed
            {
                Logger.Info("MainForm_Load - Properties.Settings.Default.ctrlUpgradeRequired");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ctrlUpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            Properties.Settings.Default.gui2DShowVertexEnable = false;              // don't show vertex / path-nodes on program start
                                                                                    //	expandGCode = Properties.Settings.Default.FCTBBlockExpandOnSelect;

            if (Properties.Settings.Default.flowCheckRegistryChange)	// don't load from clipboard on program start
            {
                const string reg_key = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";
                try
                {
                    Registry.SetValue(reg_key, "update", 0);
                    Registry.SetValue(reg_key, "start", 0);
                    Registry.SetValue(reg_key, "stop", 0);
                    Registry.SetValue(reg_key, "offsetX", "0.0");
                    Registry.SetValue(reg_key, "offsetY", "0.0");
                    Registry.SetValue(reg_key, "rotate", "0.0");
                }
                catch (Exception er) { Logger.Error(er, "MainForm Reading reg-key update "); }
            }

            CustomButtonsSetEvents();       // for buttons 17 to 32
            SetMenuShortCuts();				// Add shortcuts to menu items
            UpdateMenuChecker();

            LoadRecentList();               // open Recent.txt and fill menu
            if (MRUlist.Count > 0)			// add recent list to gui menu
            {
                foreach (string item in MRUlist)
                {
                    ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);  //create new menu for each item in list
                    toolStripMenuItem2.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
                }
            }
            SetRecentText();

            int toolSelect = Properties.Settings.Default.guiToolSelection;
            if ((toolSelect < 0) || (toolSelect >= tC_RouterPlotterLaser.TabCount))
                toolSelect = 0;
            tC_RouterPlotterLaser.SelectedIndex = toolSelect;

            LoadExtensionList();			// fill menu with available extension-scripts
            CmsPicBoxEnable(false);			// no graphic - no tasks

            gBoxDRO.Click += GrpBoxDRO_Click;
            gBoxDROSetCoord.Click += GrpBoxDRO_Click;

            gBoxOverride.Click += GrpBoxOverride_Click;	// add event handler to groupBox for opening/closing Feed override controls
            gBoxOverride.Height = 15;
            gBoxOverrideLarge = false;

            Gb_Jogging.Click += GrpBoxJogging_Click;	// add event handler to groupBox for opening/closing Feed override controls
            Gb_Jogging.Height = 75;
            GbJoggingLarge = false;

            lbDimension.Select(0, 0);       // unselect text Dimension box

            try
            {
                if (ControlGamePad.Initialize())
                    Logger.Info(culture, "GamePad found");
            }
            catch (Exception er) { Logger.Error(er, " MainForm - ControlGamePad.Initialize "); }

            Grbl.Init();                    // load and set grbl messages in grblRelated.cs
            CodeMessage.Init();

            GuiVariables.ResetVariables();	// set variables in MainFormObjects.cs			

            if (Properties.Settings.Default.guiExtendedLoggingEnabled || Properties.Settings.Default.guiExtendedLoggingCOMEnabled)
                StatusStripSet(0, "Logging enabled", Color.Yellow);
        }

        // initialize Main form
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Use the Constructor in a Windows Form for ensuring that initialization is done properly.
            // Use load event: code that requires the window size and location to be known.
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            SetGUISize();               // resize GUI arcodring last size and check if within display in MainFormUpdate.cs

            if (Properties.Settings.Default.guiCheckUpdate)
            {
                StatusStripSet(2, Localization.GetString("statusStripeCheckUpdate"), Color.LightGreen);
                CheckUpdate.CheckVersion(false, Properties.Settings.Default.guiLastEndReason);     // check update
            }

            mainTimerCount = 0;
            SplashScreenTimer.Enabled = true;
            SplashScreenTimer.Stop();
            SplashScreenTimer.Start();  // 1st event after 1500
            CbAddGraphic_CheckedChanged(sender, e);
        }

        private void SplashScreenTimer_Tick(object sender, EventArgs e)
        {
            if (_splashscreen != null)          // 2nd occurance, hide splashscreen windows
            {
                this.Opacity = 100;

                if (_serial_form == null)
                {
                    if (Properties.Settings.Default.ctrlUseSerial2)
                    {
                        _serial_form2 = new ControlSerialForm("COM Tool changer", 2);
                        if (showFormInFront) _serial_form2.Show(this);
                        else _serial_form2.Show();
                    }
                    if (Properties.Settings.Default.ctrlUseSerial3)
                    {
                        _serial_form3 = new SimpleSerialForm();// "COM simple", 3);
                        if (showFormInFront) _serial_form3.Show(this);
                        else _serial_form3.Show();
                    }
                    _serial_form = new ControlSerialForm("COM CNC", 1, _serial_form2, _serial_form3);
                    if (showFormInFront) _serial_form.Show(this);
                    else _serial_form.Show();

                    _serial_form.RaisePosEvent += OnRaisePosEvent;
                    _serial_form.RaiseStreamEvent += OnRaiseStreamEvent;
                }

                if (Properties.Settings.Default.ctrlUseSerialDIY)
                { DIYControlopen(sender, e); }

                if (_splashscreen != null)
                {
                    _splashscreen.Close();
                    _splashscreen.Dispose();
                }
                _splashscreen = null;
                Logger.Info(culture, "++++++ MainForm SplashScreen closed          -> mainTimer:{0}", mainTimerCount);

                string[] args = Environment.GetCommandLineArgs();

                if (args.Length > 1)
                    LoadFiles(args, 1);

                SplashScreenTimer.Stop();
                SplashScreenTimer.Interval = 2000;
                SplashScreenTimer.Start();
                ResetStreaming(false);
                timerUpdateControls = true;
                Properties.Settings.Default.guiLastStart = DateTime.Now.Ticks;
                Properties.Settings.Default.guiLastEndReason = "";

                if (Properties.Settings.Default.processOpenOnProgStart)
                { ProcessAutomationFormOpen(sender, e); }

                CheckProgramFiles();
            }
            else
            {
                SplashScreenTimer.Enabled = false;      // 1st occurance, show splashscreen windows
                                                        //    StatusStripClear(2, 2);
                Logger.Info(culture, "++++++ MainForm SplashScreen Timer disabled  -> mainTimer:{0}", mainTimerCount);
                timerUpdateControlSource = "SplashScreenTimer_Tick";
                MainTimer.Stop();
                MainTimer.Start();
            }
        }

        // close Main form
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {   // Note all other forms will be closed, before reaching following code...
            Logger.Info("###### FormClosing ");
            if (isStreaming)
                EventCollector.SetStreaming("CLOST");
            else
                EventCollector.SetStreaming("CLOSE");

            loadTimer.Stop();
            Properties.Settings.Default.mainFormWinState = WindowState;
            WindowState = FormWindowState.Normal;
            Properties.Settings.Default.mainFormSize = Size;
            Properties.Settings.Default.locationMForm = Location;
            ControlPowerSaving.EnableStandby();
            Properties.Settings.Default.mainFormSplitDistance = splitContainer1.SplitterDistance;
            Properties.Settings.Default.guiToolSelection = tC_RouterPlotterLaser.SelectedIndex;

            Properties.Settings.Default.guiLastEnd = DateTime.Now.Ticks;

            SaveSettings();
            Logger.Info("###### GRBL-Plotter STOP ######");
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.Info("###+++ GRBL-Plotter FormClosed +++###");
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // Use this since we are a WinForms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Use this since we are a console app
                System.Environment.Exit(1);
            }
            EventCollector.SetEnd();
            Logger.Info("EventCollector: {0}", Properties.Settings.Default.guiLastEndReason);
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Trace("##**** GRBL-Plotter EXIT");
            this.Close();
        }


        // send command via serial form
        private void SendRealtimeCommand(int cmd)
        { _serial_form.RealtimeCommand((byte)cmd); }
        private void SendRealtimeCommand(byte cmd)
        { _serial_form.RealtimeCommand(cmd); }


        private void SendCommands(string txt, bool jogging = false)
        {
            if (txt.Contains(";"))
            {
                string[] commands = txt.Split(';');
                foreach (string cmd in commands)
                { if (cmd.Length > 0) SendCommand(cmd.Trim(), jogging); }
            }
            else
                SendCommand(txt, jogging);
        }

        private void SendCommand(string txt, bool jogging = false)
        {
            if (!Grbl.isConnected)
            { Logger.Warn("SendCommand txt:{0}  jog:{1}  - not connected", txt, jogging); }

            if ((jogging) && (Grbl.isVersion_0 == false))   // includes (grbl.isMarlin == false) https://github.com/gnea/grbl/wiki/Grbl-v1.1-Jogging
            {
                string[] stringArray = { "G90", "G91", "G20", "G21", "G53" };
                if (txt.Contains("X") || txt.Contains("Y") || txt.Contains("Z") || txt.Contains("A") || txt.Contains("B") || txt.Contains("C"))
                {
                    foreach (string x in stringArray)
                    {
                        if (txt.Contains(x))
                        {
                            txt = "$J=" + txt;
                            break;
                        }
                    }
                }
            }
            txt = GuiVariables.InsertVariable(txt);			// will be filled in MainFormLoadFile.cs 1617, defined in MainFormObjects.cs
            if ((_serial_form != null) && (!_serial_form.RequestSend(txt, true)))     // check if COM is still open
            {
                timerUpdateControlSource = "sendCommand";
                UpdateControlEnables();	// no connection, disable buttons
            }

            if ((txt.Contains("G92") || txt.Contains("G10") || txt.Contains("G43")) && (_coordSystem_form != null))
                _coordSystem_form.RefreshValues();//_serial_form.requestSend("$#");
        }


        private void OnRaiseOverrideMessage(object sender, OverrideMsgEventArgs e)   // command from streaming_form2 - Override
        { SendRealtimeCommand(e.MSG); }

        // get override events from form "StreamingForm" for GRBL 0.9
        private string overrideMessage = "";
        private void OnRaiseOverrideEvent(object sender, OverrideEventArgs e)
        {
            if (e.Source == OverrideSource.feedRate)
                _serial_form.InjectCode("F", (int)e.Value, e.Enable);
            if (e.Source == OverrideSource.spindleSpeed)
                _serial_form.InjectCode("S", (int)e.Value, e.Enable);

            overrideMessage = "";
            if (e.Enable)
                overrideMessage = " !!! Override !!!";
            lbInfo.Text = lastLabelInfoText + overrideMessage;
        }

        private void ShowLaserMode()
        {
            if (!Grbl.isVersion_0 && _serial_form.IsLasermode)
            {
                SetTextThreadSave(lbInfo, Localization.GetString("mainInfoLaserModeOn"), Color.Fuchsia);
            }
            else
            {
                SetTextThreadSave(lbInfo, Localization.GetString("mainInfoLaserModeOff"), Color.Lime);
            }
        }
        // update 500ms
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (timerUpdateControls)    // streaming reset, finish, pause, toolchange, stop
            {
                timerUpdateControls = false;
                Logger.Trace(culture, "MainTimer_Tick - timerUpdateControls {0}", timerUpdateControlSource);
                UpdateWholeApplication();
                UpdateControlEnables();       // enable controls if serial connected
                                              //                resizeJoystick();       // shows / hide A,B,C joystick controls
                Invalidate();
            }

            if (isStreaming)
            {
                elapsed = DateTime.UtcNow - timeInit;
                lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss", culture);

                if (signalShowToolExchangeMessage)
                {
                    signalShowToolExchangeMessage = false;
                    ShowToolChangeMessage();
                }
                if (Properties.Settings.Default.flowCheckRegistryChange)
                {
                    int start = 0, stop = 0;
                    const string reg_key = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";

                    try
                    {
                        start = (int)Registry.GetValue(reg_key, "start", 0);
                        Registry.SetValue(reg_key, "start", 0);
                        stop = (int)Registry.GetValue(reg_key, "stop", 0);
                        Registry.SetValue(reg_key, "stop", 0);
                    }
                    catch (Exception er) { Logger.Error(er, "MainTimer_Tick stream Reading reg-key update "); }

                    if (start != 0)
                    {
                        Logger.Trace("MainTimer_Tick Pause streaming");
                        StartStreaming(0, fCTBCode.LinesCount - 1);   // btnStreamStart.PerformClick(); 
                    }
                    if (stop != 0)
                    {
                        Logger.Trace("MainTimer_Tick Stop streaming");
                        StopStreaming(true);    // btnStreamStop.PerformClick(); 
                    }
                }
            }
            else
            {
                if (updateDrawingPath && VisuGCode.ContainsG91Command())
                {
                    pictureBox1.Invalidate(); // will be called by parent function
                }
                updateDrawingPath = false;

                if (Properties.Settings.Default.flowCheckRegistryChange)
                {
                    int update = 0, start = 0;
                    double offX = 0;
                    double offY = 0;
                    double rotate = 0;
                    const string reg_key = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";
                    try
                    {
                        update = (int)Registry.GetValue(reg_key, "update", 0);
                        Registry.SetValue(reg_key, "update", 0);

                        if (double.TryParse((string)Registry.GetValue(reg_key, "offsetX", "0.0"), out double ox)) { offX = ox; }
                        if (double.TryParse((string)Registry.GetValue(reg_key, "offsetY", "0.0"), out double oy)) { offY = oy; }
                        if (double.TryParse((string)Registry.GetValue(reg_key, "rotate", "0.0"), out double r)) { rotate = r; }
                        Registry.SetValue(reg_key, "offsetX", "0.0");
                        Registry.SetValue(reg_key, "offsetY", "0.0");
                        Registry.SetValue(reg_key, "rotate", "0.0");

                        start = (int)Registry.GetValue(reg_key, "start", 0);
                        Registry.SetValue(reg_key, "start", 0);
                    }
                    catch (Exception er) { Logger.Error(er, "Reading reg-key update"); }

                    if (update > 0)
                    {
                        Logger.Trace("MainTimer_Tick Automatic update from clipboard");
                        LoadFromClipboard();
                        EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
                        Properties.Settings.Default.counterImportExtension += 1;
                    }

                    if ((offX != 0) || (offY != 0))
                    {
                        Logger.Trace("MainTimer_Tick OffX:{0}  OffY:{1}", offX, offY);
                        TransformStart("Offset Reg");
                        fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offX, -offY, 0);// VisuGCode.Translate.Offset1);                    
                        TransformEnd();
                    }

                    if (rotate != 0)
                    {
                        Logger.Trace("MainTimer_Tick Rotate:{0}", rotate);
                        TransformStart("Rotate Reg");
                        fCTBCode.Text = VisuGCode.TransformGCodeRotate(rotate, 1, new XyPoint(0, 0));
                        TransformEnd();
                    }

                    if (start != 0)
                    {
                        Logger.Trace("MainTimer_Tick Start streaming", rotate);
                        StartStreaming(0, fCTBCode.LinesCount - 1);   // (); btnStreamStart.PerformClick();
                    }
                }

                if (loadTimerStep == 0)
                {
                    pictureBox1.Invalidate();   // vector graphic is loading
                    Application.DoEvents();
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
            {
                if (delayedSend-- == 1)
                {
                    _serial_form.AddToLog("* Code after pause/stop: " + Properties.Settings.Default.flowControlText + " [Setup - Program behavior - Flow control]");
                    ProcessCommands(Properties.Settings.Default.flowControlText);
                }
            }

            ShowGrblLastMessage();

            if (timerShowGCodeError)
            {
                timerShowGCodeError = false;
                ShowGCodeErrors();
            }

            if (delayedStatusStripClear0 > 0)
            {
                if (delayedStatusStripClear0-- == 1)
                { StatusStripClear(0); }
            }
            if (delayedStatusStripClear1 > 0)
            {
                if (delayedStatusStripClear1-- == 1)
                { StatusStripClear(1); }
            }
            if (delayedStatusStripClear2 > 0)
            {
                if (delayedStatusStripClear2-- == 1)
                { StatusStripClear(2); }
            }
            if (delayedMessageFormClose > 0)
            {
                if (delayedMessageFormClose-- == 1)
                {
                    if (!CloseMessageForm(true))
                        delayedMessageFormClose++;

                    //    Logger.Trace("delayedMessageFormClose {0}", delayedMessageFormClose);
                }
            }
            if (delayedHeightMapShow > 0)
            {
                if (delayedHeightMapShow-- == 1)
                { LoadHeightMap(); }
            }
            mainTimerCount++;
        }

        private bool CloseMessageForm(bool keepOpen = false)
        {
            if (_message_form != null)
            {
                if (keepOpen && _message_form.DontClose)
                    return false;
                _message_form.Close();
                _message_form = null;
            }
            return true;
        }

        private void ShowGrblLastMessage()
        {
            if (Grbl.lastMessage.Length > 3)
            {
                if (Grbl.lastMessage.ToLower(culture).Contains("missing"))
                {
                    StatusStripClear();
                    StatusStripSet(1, Grbl.lastMessage, Color.Fuchsia);
                    StatusStripSet(2, Localization.GetString("statusStripeResetNeeded"), Color.Yellow);
                }
                else if (Grbl.lastMessage.ToLower(culture).Contains("reset"))
                {
                    label_status.Text = "";
                    label_status.BackColor = SystemColors.Control;
                    StatusStripClear(2); // MainTimer_Tick

                    if (Grbl.lastMessage.ToLower(culture).Contains("streaming"))
                        StatusStripSet(1, Grbl.lastMessage, Color.Fuchsia);
                    else
                        StatusStripSet(1, Grbl.lastMessage, Color.Yellow);

                    if (Grbl.lastMessage.ToLower(culture).Contains("hard"))
                        StatusStripSet(2, Localization.GetString("statusStripeGrblResetNeeded"), Color.Yellow);
                }
                else
                    StatusStripSet(1, Grbl.lastMessage, Color.Yellow);
                Grbl.lastMessage = "";
            }
        }


        #region GUI Objects

        // Setup Custom Buttons during loadSettings()
        private readonly string[] btnCustomCommand = new string[33];
        private int SetCustomButton(Button btn, string text)//, int cnt)
        {
            int index;
            try
            {
                index = Convert.ToUInt16(btn.Name.Substring("btnCustom".Length), culture);
            }
            catch (Exception err) { Logger.Error(err, "SetCustomButton {0}", btn.Name); return 0; }

            if (text.Contains("|") && (index >= 0) && (index < btnCustomCommand.Length))		// < 32
            {
                string[] parts = text.Split('|');
                Color btnColor = Control.DefaultBackColor;
                btn.Text = parts[0];
                if (parts.Length > 2)
                {
                    if (parts[2].Length > 3)
                    {
                        Color tmp = SystemColors.Control;
                        try
                        { tmp = ColorTranslator.FromHtml(parts[2]); }
                        catch (Exception err)
                        { Logger.Error(err, "SetCustomButton with {0} from {1}", parts[2], text); }
                        btnColor = tmp;
                    }
                }
                SetButtonColors(btn, btnColor);

                if (parts[1].Length > 1)
                {
                    if ((parts[1].Length > 4) && File.Exists(parts[1]))
                    {
                        string[] lines = File.ReadAllLines(parts[1]);
                        string output = "";
                        int max = 10;
                        foreach (string line in lines)
                        { output += line + "\r\n"; if (max-- <= 0) break; }
                        toolTip1.SetToolTip(btn, parts[0] + "\r\nFile: " + parts[1] + "\r\n" + output);
                    }
                    else
                    { toolTip1.SetToolTip(btn, parts[0] + "\r\n" + parts[1].Replace(";", "\r\n")); }
                }
                else
                {
                    toolTip1.SetToolTip(btn, "Right click to change content");
                }

                btnCustomCommand[index] = parts[1];
                return parts[0].Trim().Length;// + parts[1].Trim().Length;
            }
            return 0;
        }
        private static void SetButtonColors(Button btn, Color col)
        {
            btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
            if (col == Control.DefaultBackColor)
                btn.UseVisualStyleBackColor = true;
        }
        private static Color ContrastColor(Color color)
        {
            int d;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

        private void BtnCustomButton_Click(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (isStreaming)
            {
                Logger.Warn("BtnCustomButton clicked while streaming: {0}", clickedButton.Name);
                StatusStripSet(2, Localization.GetString("statusStripeButtonBlocked"), Color.Fuchsia);  // "Custom button is blocked while streaming"
                delayedStatusStripClear2 = 8;
                return;
            }
            int index;
            try
            {
                index = Convert.ToUInt16(clickedButton.Name.Substring("btnCustom".Length), culture);
            }
            catch (Exception err) { Logger.Error(err, "BtnCustomButton_Click {0}", clickedButton.Name); return; }

            if ((index >= 0) && (index < btnCustomCommand.Length))		// < 32
            {
                if (e.Button == MouseButtons.Right)
                {
                    using (ButtonEdit f = new ButtonEdit(index))
                    {
                        var result = f.ShowDialog(this);
                        if (result == DialogResult.OK)
                        {
                            timerUpdateControlSource = "btnCustomButton_Click";
                            UpdateWholeApplication();
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(clickedButton.Text))
                    { ProcessCommands(btnCustomCommand[index]); }
                }
            }
        }


        // virtualJoystic sends two step-width-values per second. One position should be reached before next command
        // speed (units/min) = 2 * stepsize * 60 * factor (to compensate speed-ramps)
        private int virtualJoystickXY_lastIndex = 1;
        private int virtualJoystickZ_lastIndex = 1;
        private int virtualJoystickA_lastIndex = 1;
        private void VirtualJoystickXY_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void VirtualJoystickXY_JoyStickEvent(object sender, JogEventArgs e)
        { VirtualJoystickXY_move(e.JogPosX, e.JogPosY); }
        private void VirtualJoystickXY_move(int index_X, int index_Y)
        {
            int indexX = Math.Abs(index_X);
            int indexY = Math.Abs(index_Y);
            int dirX = Math.Sign(index_X);
            int dirY = Math.Sign(index_Y);
            virtualJoystickXY_lastIndex = Math.Max(indexX, indexY);
            if (indexX >= joystickXYStep.Length)
            { indexX = joystickXYStep.Length - 1; index_X = indexX; }
            if (indexX < 0)
            { indexX = 0; index_X = 0; }
            if (indexY >= joystickXYStep.Length)
            { indexY = joystickXYStep.Length - 1; index_Y = indexY; }
            if (indexY < 0)
            { indexY = 0; index_Y = 0; }

            if ((index_X == 0) && (index_Y == 0))
            { if (!Grbl.isVersion_0) SendRealtimeCommand(133); return; }

            int speed = (int)Math.Max(joystickXYSpeed[indexX], joystickXYSpeed[indexY]);
            String strX = Gcode.FrmtNum(joystickXYStep[indexX] * dirX);
            String strY = Gcode.FrmtNum(joystickXYStep[indexY] * dirY);
            //    Logger.Error("VirtualJoystickXY_move speed==0  x:{0}  y:{1}", index_X, index_Y);
            if (speed > 0)
            {
                if (Properties.Settings.Default.machineLimitsAlarm && Properties.Settings.Default.machineLimitsShow)
                {
                    if (!Dimensions.WithinLimits(Grbl.posMachine, joystickXYStep[indexX] * dirX, joystickXYStep[indexY] * dirY))
                    {
                        decimal minx = Properties.Settings.Default.machineLimitsHomeX;
                        decimal maxx = minx + Properties.Settings.Default.machineLimitsRangeX;
                        decimal miny = Properties.Settings.Default.machineLimitsHomeY;
                        decimal maxy = miny + Properties.Settings.Default.machineLimitsRangeY;

                        string tmp = string.Format(culture, "minX: {0:0.0} moveTo: {1:0.0} maxX: {2:0.0}", minx, (Grbl.posMachine.X + joystickXYStep[indexX] * dirX), maxx);
                        tmp += string.Format(culture, "\r\nminY: {0:0.0} moveTo: {1:0.0} maxY: {2:0.0}", miny, (Grbl.posMachine.Y + joystickXYStep[indexY] * dirY), maxy);
                        System.Media.SystemSounds.Beep.Play();
                        DialogResult dialogResult = MessageBox.Show(Localization.GetString("mainLimits1") + tmp + Localization.GetString("mainLimits2"), Localization.GetString("mainAttention"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                        if (dialogResult == DialogResult.Cancel)
                            return;
                    }
                }
                String s = "G91 ";
                if (Grbl.isMarlin) { s += ";G1 "; }
                if (index_X == 0)
                    s += String.Format(culture, "Y{0} F{1}", strY, speed).Replace(',', '.');
                else if (index_Y == 0)
                    s += String.Format(culture, "X{0} F{1}", strX, speed).Replace(',', '.');
                else
                    s += String.Format(culture, "X{0} Y{1} F{2}", strX, strY, speed).Replace(',', '.');

                SendCommands(s, true);
            }
            else
                Logger.Error("VirtualJoystickXY_move speed==0  index: x:{0}  y:{1}", index_X, index_Y);

        }
        private void VirtualJoystickXY_MouseUp(object sender, MouseEventArgs e)
        { if (!Grbl.isVersion_0 && cBSendJogStop.Checked) SendRealtimeCommand(133); }
        private void BtnJogStop_Click(object sender, EventArgs e)
        { if (!Grbl.isVersion_0) SendRealtimeCommand(133); }    //0x85

        private void VirtualJoystickXY_Enter(object sender, EventArgs e)
        {
            if (Grbl.isVersion_0) SendCommands("G91;G1F100");
            Gb_Jogging.BackColor = Color.LightGreen;
        }
        private void VirtualJoystickXY_Leave(object sender, EventArgs e)
        {
            if (Grbl.isVersion_0) SendCommand("G90");
            Gb_Jogging.BackColor = SystemColors.Control;
            virtualJoystickXY.JoystickRasterMark = 0;
            virtualJoystickZ.JoystickRasterMark = 0;
            virtualJoystickA.JoystickRasterMark = 0;
            virtualJoystickB.JoystickRasterMark = 0;
            virtualJoystickC.JoystickRasterMark = 0;
        }
        private void VirtualJoystickZ_JoyStickEvent(object sender, JogEventArgs e)
        { VirtualJoystickZ_move(e.JogPosY); }
        private void VirtualJoystickZ_move(int index_Z)
        {
            int indexZ = Math.Abs(index_Z);
            int dirZ = Math.Sign(index_Z);
            if (indexZ >= joystickZStep.Length)
            { indexZ = joystickZStep.Length - 1; }
            if (indexZ < 0)
            { indexZ = 0; }

            if (index_Z == 0)
            { if (!Grbl.isVersion_0) SendRealtimeCommand(133); return; }

            virtualJoystickZ_lastIndex = indexZ;
            int speed = (int)joystickZSpeed[indexZ];
            String strZ = Gcode.FrmtNum(joystickZStep[indexZ] * dirZ);
            if (speed > 0)
            {
                String s = "G91 ";
                if (Grbl.isMarlin) { s += ";G1 "; }
                s += String.Format(culture, "Z{0} F{1}", strZ, speed).Replace(',', '.');
                SendCommands(s, true);
            }
        }
        private void VirtualJoystickA_JoyStickEvent(object sender, JogEventArgs e)
        { VirtualJoystickA_move(e.JogPosY, ctrl4thName); }
        private void VirtualJoystickA_move(int index_A, string name)
        {
            int indexA = Math.Abs(index_A);
            int dirA = Math.Sign(index_A);
            if (indexA >= joystickAStep.Length)
            { indexA = joystickAStep.Length - 1; }
            if (indexA < 0)
            { indexA = 0; }

            if (index_A == 0)
            { if (!Grbl.isVersion_0) SendRealtimeCommand(133); return; }

            virtualJoystickA_lastIndex = indexA;
            int speed = (int)joystickASpeed[indexA];
            String strZ = Gcode.FrmtNum(joystickAStep[indexA] * dirA);
            if (speed > 0)
            {
                String s = "G91 ";
                if (Grbl.isMarlin) { s += ";G1 "; }
                s += String.Format(culture, "{0}{1} F{2}", name, strZ, speed).Replace(',', '.');
                SendCommands(s, true);
            }
        }
        private void VirtualJoystickB_JoyStickEvent(object sender, JogEventArgs e)
        { VirtualJoystickA_move(e.JogPosY, "B"); }
        private void VirtualJoystickC_JoyStickEvent(object sender, JogEventArgs e)
        { VirtualJoystickA_move(e.JogPosY, "C"); }

        // Spindle and coolant
        private void CbSpindle_CheckedChanged(object sender, EventArgs e)
        {
            string m = "M3";
            if (RbSpindleCCW.Checked) m = "M4";

            float speed = Grbl.GetSetting(30);
            if (speed > 1)
                speed = (int)Math.Round((float)NudSpeed.Value * speed / 100);
            else
                speed = (int)Math.Round((float)NudSpeed.Value * 10);


            if (CbSpindle.Checked)
            {
                if (Grbl.isConnected)
                    SendCommand(m + " S" + speed.ToString());
                SetTextThreadSave(LblSpeedSetVal, speed.ToString(), Color.Yellow);

                CbLaser.CheckedChanged -= CbLaser_CheckedChanged;
                RbLaserM3.CheckedChanged -= CbLaser_CheckedChanged;
                RbLaserM4.CheckedChanged -= CbLaser_CheckedChanged;

                CbLaser.Checked = true;
                TbLaser.Value = (int)NudSpeed.Value;
                RbLaserM3.Checked = RbSpindleCW.Checked;
                RbLaserM4.Checked = !RbSpindleCW.Checked;
                PbLaser.Visible = true;

                RbLaserM4.CheckedChanged += CbLaser_CheckedChanged;
                RbLaserM3.CheckedChanged += CbLaser_CheckedChanged;
                CbLaser.CheckedChanged += CbLaser_CheckedChanged;
            }
            else
            {
                if (Grbl.isConnected)
                    SendCommand("M5");
                CbLaser.CheckedChanged -= CbLaser_CheckedChanged;
                CbLaser.Checked = false;
                PbLaser.Visible = false;
                CbLaser.CheckedChanged += CbLaser_CheckedChanged;
            }
        }
        private void TbLaser_MouseDown(object sender, MouseEventArgs e)
        {
            SendLaserCommand();
        }

        private void CbLaser_CheckedChanged(object sender, EventArgs e)
        {
            SendLaserCommand();
        }
        private void SendLaserCommand()
        {
            string m = "M3";
            if (RbLaserM4.Checked) m = "M4";

            float speed = Grbl.GetSetting(30);
            if (speed > 1)
                speed = (int)Math.Round((float)TbLaser.Value * speed / 100);
            else
                speed = (int)Math.Round((float)TbLaser.Value * 10);

            bool sendG1ToActivateLaser = _serial_form.IsLasermode;// && (LblLaserSetVal.Text == "0");

            if (CbLaser.Checked)
            {
                if (Grbl.isConnected)
                {
                    if (sendG1ToActivateLaser) SendCommand(string.Format(culture, "$J=G91X0.0001F1000"));    // move G1 tiny step to force laser on
                    SendCommand(m + " S" + speed.ToString());
                    SetTextThreadSave(LblLaserSetVal, speed.ToString(), Color.Yellow);
                    PbLaser.Visible = true;
                    if (sendG1ToActivateLaser) SendCommand(string.Format(culture, "$J=G91X-0.0001F1000"));   // move G1 tiny step back
                }
                CbSpindle.CheckedChanged -= CbSpindle_CheckedChanged;
                RbSpindleCW.CheckedChanged -= CbSpindle_CheckedChanged;
                RbSpindleCCW.CheckedChanged -= CbSpindle_CheckedChanged;
                NudSpeed.ValueChanged -= CbSpindle_CheckedChanged;

                CbSpindle.Checked = true;
                NudSpeed.Value = TbLaser.Value;
                RbSpindleCW.Checked = RbLaserM3.Checked;
                RbSpindleCCW.Checked = !RbLaserM3.Checked;

                NudSpeed.ValueChanged += CbSpindle_CheckedChanged;
                RbSpindleCCW.CheckedChanged += CbSpindle_CheckedChanged;
                RbSpindleCW.CheckedChanged += CbSpindle_CheckedChanged;
                CbSpindle.CheckedChanged += CbSpindle_CheckedChanged;
            }
            else
            {
                if (Grbl.isConnected)
                    SendCommand("M5"); PbLaser.Visible = false;
                CbSpindle.CheckedChanged -= CbSpindle_CheckedChanged;
                CbSpindle.Checked = false;
                CbSpindle.CheckedChanged += CbSpindle_CheckedChanged;
            }
        }
        private void CbLasermode_CheckedChanged(object sender, EventArgs e)
        {
            if (CbLasermode.Checked)
            {
                SetTextThreadSave(CbLasermodeVal, "ON", Color.Yellow);
                SendCommand("$32=1");
            }
            else
            {
                SetTextThreadSave(CbLasermodeVal, "OFF", Color.Yellow);
                SendCommand("$32=0");
            }
        }

        private void BtnPenUp_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.importGCPWMEnable)  // to avoid error 9, do jog command at last
            {
                if (_serial_form.IsLasermode) SendCommand(string.Format(culture, "$J=G91X0.0001F1000"));    // move G1 tiny step to force PWM on
                SendCommand(string.Format(culture, "M3 S{0}", Properties.Settings.Default.importGCPWMUp));
                if (_serial_form.IsLasermode) SendCommand(string.Format(culture, "$J=G91X-0.0001F1000"));   // move G1 tiny step back
            }
            if (Properties.Settings.Default.importGCZEnable)
            {
                SendCommand(string.Format(culture, "$J=G90 Z{0} F{1}", Gcode.FrmtNum(Properties.Settings.Default.importGCZUp), Properties.Settings.Default.importGCZFeed));
            }
        }
        private void BtnPenZero_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.importGCPWMEnable)  // to avoid error 9, do jog command at last
            {
                if (_serial_form.IsLasermode) SendCommand(string.Format(culture, "G91 G01 X0.0001F1000"));
                SendCommand(string.Format(culture, "M3 S{0}", Properties.Settings.Default.importGCPWMZero));
                if (_serial_form.IsLasermode) SendCommand(string.Format(culture, "G91 G01 X-0.0001F1000"));
            }
            if (Properties.Settings.Default.importGCZEnable)
            {
                SendCommand(string.Format(culture, "$J=G90 Z{0} F{1}", Gcode.FrmtNum(0f), Properties.Settings.Default.importGCZFeed));
            }
        }

        private void BtnPenDown_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.importGCPWMEnable)  // to avoid error 9, do jog command at last
            {
                if (_serial_form.IsLasermode) SendCommand(string.Format(culture, "G91 G01 X0.0001F1000"));
                SendCommand(string.Format(culture, "M3 S{0}", Properties.Settings.Default.importGCPWMDown));
                if (_serial_form.IsLasermode) SendCommand(string.Format(culture, "G91 G01 X-0.0001F1000"));
            }
            if (Properties.Settings.Default.importGCZEnable)
            {
                SendCommand(string.Format(culture, "$J=G90 Z{0} F{1}", Gcode.FrmtNum(Properties.Settings.Default.importGCZDown), Properties.Settings.Default.importGCZFeed));
            }
        }

        private void CbCoolant_CheckedChanged(object sender, EventArgs e)
        {
            if (CbCoolant.Checked)
            { SendCommand("M8"); }
            else
            { SendCommand("M9"); }
        }
        private void CbMist_CheckedChanged(object sender, EventArgs e)
        {
            if (CbCoolant.Checked)
            { SendCommand("M7"); }
            else
            { SendCommand("M9"); }
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            if (Grbl.isMarlin)
                SendCommand("G28");
            else
                SendCommand("$H");
        }
        private void BtnZeroX_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " X0.000"); }    // zeroCmd = "G10 L20 P0";
        private void BtnZeroY_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " Y0.000"); }
        private void BtnZeroZ_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " Z0.000"); }
        private void BtnZeroA_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " " + ctrl4thName + "0.000"); }
        private void BtnZeroB_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " B0.000"); }
        private void BtnZeroC_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " C0.000"); }
        private void BtnZeroXY_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " X0.000 Y0.000"); }
        private void BtnZeroXYZ_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + " X0.000 Y0.000 Z0.000"); }

        private void BtnSetCoordX_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + string.Format(" X{0:0.000}",NudSetCoordX.Value)); }    // zeroCmd = "G10 L20 P0";

        private void BtnSetCoordY_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + string.Format(" Y{0:0.000}", NudSetCoordY.Value)); }    // zeroCmd = "G10 L20 P0";

        private void BtnSetCoordZ_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + string.Format(" Z{0:0.000}", NudSetCoordZ.Value)); }    // zeroCmd = "G10 L20 P0";

        private void BtnSetCoordA_Click(object sender, EventArgs e)
        { SendCommands((Grbl.isMarlin ? "G92" : zeroCmd) + string.Format(" A{0:0.000}", NudSetCoordA.Value)); }    // zeroCmd = "G10 L20 P0";

        private void BtnJogX_Click(object sender, EventArgs e)
        { BtnMoveZero("X0", joystickXYSpeed[5].ToString(culture)); }
        private void BtnJogY_Click(object sender, EventArgs e)
        { BtnMoveZero("Y0", joystickXYSpeed[5].ToString(culture)); }
        private void BtnJogZ_Click(object sender, EventArgs e)
        { BtnMoveZero("Z0", joystickZSpeed[5].ToString(culture)); }
        private void BtnJogZeroA_Click(object sender, EventArgs e)
        { BtnMoveZero(ctrl4thName + "0", joystickZSpeed[5].ToString(culture)); }
        private void BtnJogXY_Click(object sender, EventArgs e)
        { BtnMoveZero("X0Y0", joystickXYSpeed[5].ToString(culture)); }
        private void BtnJogAbsX_Click(object sender, EventArgs e)
        { BtnMoveZero(string.Format("X{0:0.000}",NudJogAbsX.Value), joystickXYSpeed[5].ToString(culture));}
        private void BtnJogAbsY_Click(object sender, EventArgs e)
        { BtnMoveZero(string.Format("Y{0:0.000}", NudJogAbsY.Value), joystickXYSpeed[5].ToString(culture)); }
        private void BtnJogAbsZ_Click(object sender, EventArgs e)
        { BtnMoveZero(string.Format("Z{0:0.000}", NudJogAbsZ.Value), joystickZSpeed[5].ToString(culture)); }

        private void BtnMoveZero(string axis, string fed)
        {
            string seperate = "";
            string mode = "";
            if (Grbl.isMarlin) { seperate += ";"; mode = "G1"; }
            string feed = "F" + fed;

            if (cBMoveG0.Checked)
            {
                SendCommands(string.Format(culture, "G90{0} G0 {1}", seperate, axis));
            }
            else
            {
                SendCommands(string.Format(culture, "G90{0}{1} {2} {3}", seperate, mode, axis, feed), true);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (_serial_form.IsConnectedToGrbl())
            {
                Logger.Trace("BtnReset_Click  IsConnectedToGrbl");
                //    StopStreaming(true);          // removed 2024-05-19
                _serial_form.GrblReset(true);   // savePos
            }
            isStreaming = false;
            pbFile.Value = 0;
            pbFile.Maximum = 100;
            signalResume = 0;
            signalLock = 0;
            signalPlay = 0;
            btnResume.BackColor = SystemColors.Control;
            //    lbInfo.Text = "";
            //    lbInfo.BackColor = SystemColors.Control;
            SetTextThreadSave(lbInfo, "", SystemColors.Control);
            CbSpindle.CheckedChanged -= CbSpindle_CheckedChanged;
            CbSpindle.Checked = false;
            CbSpindle.CheckedChanged += CbSpindle_CheckedChanged;
            CbCoolant.CheckedChanged -= CbSpindle_CheckedChanged;
            CbCoolant.Checked = false;
            CbCoolant.CheckedChanged += CbSpindle_CheckedChanged;

            UpdateControlEnables();
            ControlPowerSaving.EnableStandby();
        }
        private void BtnFeedHold_Click(object sender, EventArgs e)
        { GrblFeedHold(); }
        private void GrblFeedHold()
        {
            SendRealtimeCommand('!');
            Logger.Trace("FeedHold");
            signalResume = 1;
            timerUpdateControlSource = "grblFeedHold";
            UpdateControlEnables();	// true overwrite streaming
        }
        private void BtnResume_Click(object sender, EventArgs e)
        { GrblResume(); StatusStripClear(); }

        private void GrblResume()
        {
            SendRealtimeCommand('~');
            Logger.Trace("Resume");
            btnResume.BackColor = SystemColors.Control;
            signalResume = 0;
            //    lbInfo.Text = "";
            //    lbInfo.BackColor = SystemColors.Control;
            SetTextThreadSave(lbInfo, "", SystemColors.Control);
            timerUpdateControlSource = "grblResume";
            UpdateControlEnables();
        }
        private void BtnKillAlarm_Click(object sender, EventArgs e)
        { GrblKillAlarm(); }
        private void GrblKillAlarm()
        {
            SendCommand("$X");
            Logger.Trace("KillAlarm");
            signalLock = 0;
            btnKillAlarm.BackColor = SystemColors.Control;
            //    lbInfo.Text = "";
            //    lbInfo.BackColor = SystemColors.Control;
            SetTextThreadSave(lbInfo, "", SystemColors.Control);
            timerUpdateControlSource = "grblKillAlarm";
            UpdateControlEnables();
        }
        #endregion

        private void CbTool_CheckedChanged(object sender, EventArgs e)
        { _serial_form.ToolInSpindle = CbTool.Checked; }

        private void BtnOverrideFR0_Click(object sender, EventArgs e)
        { SendRealtimeCommand(144); }     // 0x90 : Set 100% of programmed rate.    
        private void BtnOverrideFR1_Click(object sender, EventArgs e)
        { SendRealtimeCommand(145); }     // 0x91 : Increase 10%        
        private void BtnOverrideFR4_Click(object sender, EventArgs e)
        { SendRealtimeCommand(146); }     // 0x92 : Decrease 10%   
        private void BtnOverrideFR2_Click(object sender, EventArgs e)
        { SendRealtimeCommand(147); }     // 0x93 : Increase 1%   
        private void BtnOverrideFR3_Click(object sender, EventArgs e)
        { SendRealtimeCommand(148); }     // 0x94 : Decrease 1%   

        private void BtnOverrideRapid0_Click(object sender, EventArgs e)
        { SendRealtimeCommand(149); }     // 0x95 : Set to 100% full rapid rate.
        private void BtnOverrideRapid1_Click(object sender, EventArgs e)
        { SendRealtimeCommand(150); }     // 0x96 : Set to 50% of rapid rate.
        private void BtnOverrideRapid2_Click(object sender, EventArgs e)
        { SendRealtimeCommand(151); }     // 0x97 : Set to 25% of rapid rate.

        private void BtnOverrideSS0_Click(object sender, EventArgs e)
        { SendRealtimeCommand(153); }     // 0x99 : Set 100% of programmed spindle speed    
        private void BtnOverrideSS1_Click(object sender, EventArgs e)
        { SendRealtimeCommand(154); }     // 0x9A : Increase 10%        
        private void BtnOverrideSS4_Click(object sender, EventArgs e)
        { SendRealtimeCommand(155); }     // 0x9B : Decrease 10%   
        private void BtnOverrideSS2_Click(object sender, EventArgs e)
        { SendRealtimeCommand(156); }     // 0x9C : Increase 1%   
        private void BtnOverrideSS3_Click(object sender, EventArgs e)
        { SendRealtimeCommand(157); }     // 0x9D : Decrease 1%   

        private void BtnOverrideSpindle_Click(object sender, EventArgs e)
        { SendRealtimeCommand(158); }     // 0x9E : Toggle Spindle Stop
        private void BtnOverrideFlood_Click(object sender, EventArgs e)
        { SendRealtimeCommand(160); }     // 0xA0 : Toggle Flood Coolant  
        private void BtnOverrideMist_Click(object sender, EventArgs e)
        { SendRealtimeCommand(161); }     // 0xA1 : Toggle Mist Coolant 

        private void BtnOverrideD0_Click(object sender, EventArgs e)
        { if (BtnOverrideD0.Tag.ToString() == "off") SendCommand("M64 P0"); else SendCommand("M65 P0"); }
        private void BtnOverrideD1_Click(object sender, EventArgs e)
        { if (BtnOverrideD1.Tag.ToString() == "off") SendCommand("M64 P1"); else SendCommand("M65 P1"); }
        private void BtnOverrideD2_Click(object sender, EventArgs e)
        { if (BtnOverrideD2.Tag.ToString() == "off") SendCommand("M64 P2"); else SendCommand("M65 P2"); }
        private void BtnOverrideD3_Click(object sender, EventArgs e)
        { if (BtnOverrideD3.Tag.ToString() == "off") SendCommand("M64 P3"); else SendCommand("M65 P3"); }


        private void BtnOverrideDoor_Click(object sender, EventArgs e)
        { SendRealtimeCommand(132); }     // 0x84 : Safety Door  


        private void ProcessCommands(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;
            string[] commands = { };

            if (!command.StartsWith("(") && command.Contains('\\') && (!isStreaming || isStreamingPause))
            {
                if (File.Exists(command))
                {
                    if (command.EndsWith(".ini"))
                    {
                        var MyIni = new IniFile(command);
                        Logger.Info(culture, "Load INI: '{0}'", command);
                        MyIni.ReadAll();    // ReadImport();
                        UpdateIniVariables();
                        timerUpdateControlSource = "loadFile";
                        UpdateWholeApplication();
                        StatusStripSet(2, "INI File '" + command + "' loaded", Color.Lime);
                        return;
                    }
                    else
                    {
                        string fileCmd = File.ReadAllText(command);
                        _serial_form.AddToLog("* File: " + command);
                        commands = fileCmd.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    }
                }
                else
                {
                    _serial_form.AddToLog("Script/file does not exists: " + command);
                    Logger.Warn("ProcessCommands Script/file does not exists: {0}", command);
                }
            }
            else
            { commands = command.Split(';'); }

            if (!_serial_form.SerialPortOpen)
            {
                _serial_form.AddToLog("serial port is closed");
                return;
            }

            if (_diyControlPad != null)
            { _diyControlPad.isHeightProbing = false; }

            foreach (string btncmd in commands)
            {
                if (btncmd.StartsWith("($") && (_diyControlPad != null))
                {
                    string tmp = btncmd.Replace("($", "[");
                    tmp = tmp.Replace(")", "]");
                    _diyControlPad.SendFeedback(tmp);
                }
                else
                {
                    if (!ProcessSpecialCommands(command) && (!isStreaming || isStreamingPause))
                        SendCommand(btncmd.Trim());         // processCommands
                }
            }
        }
        private bool ProcessSpecialCommands(string command)
        {
            if (string.IsNullOrEmpty(command))
                return false;

            bool commandFound = false;
            if (command.ToLower(culture).IndexOf("#start") >= 0) { StartStreaming(0, fCTBCode.LinesCount - 1); commandFound = true; }   //  BtnStreamStart_Click(this, null); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#stop") >= 0) { BtnStreamStop_Click(this, EventArgs.Empty); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#f100") >= 0) { SendRealtimeCommand(144); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#f+10") >= 0) { SendRealtimeCommand(145); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#f-10") >= 0) { SendRealtimeCommand(146); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#f+1") >= 0) { SendRealtimeCommand(147); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#f-1") >= 0) { SendRealtimeCommand(148); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#s100") >= 0) { SendRealtimeCommand(153); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#s+10") >= 0) { SendRealtimeCommand(154); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#s-10") >= 0) { SendRealtimeCommand(155); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#s+1") >= 0) { SendRealtimeCommand(156); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#s-1") >= 0) { SendRealtimeCommand(157); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#hrst") >= 0) { _serial_form.GrblHardReset(); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#rst") >= 0) { _serial_form.GrblReset(true); commandFound = true; }//savePos
            else if (command.ToLower(culture).IndexOf("#feedhold") >= 0) { GrblFeedHold(); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#resume") >= 0) { GrblResume(); commandFound = true; }
            else if (command.ToLower(culture).IndexOf("#killalarm") >= 0) { GrblKillAlarm(); commandFound = true; }
            return commandFound;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (!pictureBox1.Focused)
                pictureBox1.Focus();
        }

        private void PictureBox1_MouseHover(object sender, EventArgs e)
        {
            if (!pictureBox1.Focused && Properties.Settings.Default.guiShowFormInFront)
            {
                //    pictureBox1.Focus();
                markerSize = (float)((double)Properties.Settings.Default.gui2DSizeTool / (picScaling * zoomFactor));
            }
        }

        private void FCTBCode_MouseHover(object sender, EventArgs e)
        {
            if (!fCTBCode.Focused && Properties.Settings.Default.guiShowFormInFront)
                fCTBCode.Focus();
        }

        private void ToolStrip_tb_StreamLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                if (int.TryParse(toolStrip_tb_StreamLine.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int lineNr))
                {
                    StartStreaming(lineNr, fCTBCode.LinesCount - 1);      // 1142
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError1"), Localization.GetString("mainAttention"));
                    toolStrip_tb_StreamLine.Text = "0";
                }
            }
        }

        private void UpdatePathDisplay(object sender, EventArgs e)
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
            VisuGCode.DrawMachineLimit();// ToolTable.GetToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
        }
        private void UpdateMenuChecker()
        {
            toolStripViewRuler.Checked = Properties.Settings.Default.gui2DRulerShow;
            toolStripViewInfo.Checked = Properties.Settings.Default.gui2DInfoShow;
            toolStripViewPenUp.Checked = Properties.Settings.Default.gui2DPenUpShow;
            toolStripViewMachine.Checked = Properties.Settings.Default.machineLimitsShow;
            toolStripViewTool.Checked = Properties.Settings.Default.gui2DToolTableShow;
            toolStripViewDimension.Checked = Properties.Settings.Default.guiDimensionShow;
            toolStripViewBackground.Checked = Properties.Settings.Default.guiBackgroundShow;
            toolStripViewMachineFix.Checked = Properties.Settings.Default.machineLimitsFix;
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                JoystickResize();
            for (int i = 17; i <= 32; i++)
            {
                if (CustomButtons17.ContainsKey(i))
                {
                    Button b = CustomButtons17[i];
                    b.Width = btnCustom1.Width - 24;
                    b.Height = btnCustom1.Height;
                }
            }

            //    StatusStripSet(0, string.Format("New size X:{0}  Y:{1}",Width, Height), Color.White);
        }
        private void JoystickResize()
        {
            int virtualJoystickSize = Properties.Settings.Default.guiJoystickSize;
            int zRatio = 25;                    // 20% of xyJoystick width
            int zCount = 1;
            Logger.Trace("resizeJoystick() visible:  A:{0} B:{1} C:{2}", Grbl.axisA, Grbl.axisB, Grbl.axisC);

            if (ctrl4thAxis || Grbl.axisA) zCount = 2;
            if (Grbl.axisB) { zCount = 3; zRatio = 25; }
            if (Grbl.axisC) { zCount = 4; zRatio = 25; }
            int spaceY = this.Height - 400;// 520;     // width is 125% or 150%    485
            int spaceX = this.Width - 670;      // heigth is 100%
            spaceX = Math.Max(spaceX, 120);// 235);     // minimum width is 235px

            int aWidth = 0, bWidth = 0, cWidth = 0;
            int zWidth = (spaceX * zRatio / (100 + zCount * zRatio));           // 
            zWidth = Math.Min(zWidth, virtualJoystickSize * zRatio / 100);
            int xyWidth = spaceX - zCount * zWidth;
            if (xyWidth < 0) xyWidth = 0;

            tLPRechtsUntenRechtsMitte.ColumnStyles[1].Width = zWidth;       // Z
            virtualJoystickA.Visible = false;
            virtualJoystickB.Visible = false;
            virtualJoystickC.Visible = false;
            if (ctrl4thAxis || Grbl.axisA)
            { aWidth = zWidth; virtualJoystickA.Visible = true; }
            if (Grbl.axisB)
            { aWidth = bWidth = zWidth; virtualJoystickB.Visible = true; }
            if (Grbl.axisC)
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
            virtualJoystickXY.Invalidate();
            virtualJoystickZ.Invalidate();
            virtualJoystickA.Invalidate();
            virtualJoystickB.Invalidate();
            virtualJoystickC.Invalidate();
        }


        // adapt size of controls
        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            int add = splitContainer1.Panel1.Width - 296;
            pbFile.Width = 194 + add;
            pbBuffer.Left = 219 + add;
            btnSimulate.Width = 194 + add;
            btnSimulateFaster.Left = 202 + add;
            btnSimulateSlower.Left = 244 + add;
            btnSimulatePause.Left = 158 + add;
            gBOverrideFRGB.Width = 284 + add;
            gBOverrideSSGB.Width = 284 + add;

            gBOverrideASGB.Width = 284 + add;
            int btnSpindleW = (279 + add) / 7;
            btnOverrideSpindle.Width = 3 * btnSpindleW;
            btnOverrideFlood.Width = 2 * btnSpindleW;
            btnOverrideMist.Width = 2 * btnSpindleW;
            btnOverrideFlood.Left = 3 + 3 * btnSpindleW;
            btnOverrideMist.Left = (281 + add) - 2 * btnSpindleW;
            if (Properties.Settings.Default.grblDescriptionDxEnable)
            {
                int btnwidth = (278 + add) / 4;
                BtnOverrideD0.Width = btnwidth;
                BtnOverrideD1.Width = btnwidth;
                BtnOverrideD2.Width = btnwidth;
                BtnOverrideD3.Width = btnwidth;
                BtnOverrideD1.Left = 3 + btnwidth;
                BtnOverrideD2.Left = 4 + 2 * btnwidth;
                BtnOverrideD3.Left = (281 + add) - btnwidth;
            }
            gBOverrideRGB.Width = 284 + add;

            lbInfo.Width = 280 + add;
            lbDimension.Width = 130 + add;
            btnLimitExceed.Left = 112 + add;
            groupBox4.Left = 133 + add;
        }

        private bool gBoxDROShowSetCoord = false;
        private void GrpBoxDRO_Click(object sender, EventArgs e)
        {
            if (!gBoxDROShowSetCoord)
            {
                if (Grbl.axisB || Grbl.axisC)
                    gBoxDRO.Width = 400;
                else
                    gBoxDRO.Width = 267;
                gBoxDROSetCoord.Visible = true;
            }
            else
            {
                if (Grbl.axisB || Grbl.axisC)
                    gBoxDRO.Width = 400;
                else
                    gBoxDRO.Width = 230;
                gBoxDROSetCoord.Visible = false;
            }
            gBoxDROShowSetCoord = !gBoxDROShowSetCoord;
        }

        private bool gBoxOverrideLarge = false;
        private void GrpBoxOverride_Click(object sender, EventArgs e)
        {
            if (gBoxOverrideLarge)
                gBoxOverride.Height = 15;
            else
            {
                if (Properties.Settings.Default.grblDescriptionDxEnable)
                {
                    gBoxOverride.Height = 200;  // 175
                    gBOverrideASGB.Height = 62;
                }
                else
                {
                    gBoxOverride.Height = 175;
                    gBOverrideASGB.Height = 37;
                }
            }
            gBoxOverrideLarge = !gBoxOverrideLarge;
        }
        private bool GbJoggingLarge = false;
        private void GrpBoxJogging_Click(object sender, EventArgs e)
        {
            if (GbJoggingLarge)
                Gb_Jogging.Height = 75;
            else
                Gb_Jogging.Height = 150;
            GbJoggingLarge = !GbJoggingLarge;
        }

        internal void SetUndoText(string txt)
        {
            if (txt == null) txt = "";
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
                unDoToolStripMenuItem.Text = Localization.GetString("mainInfoUndo");    // "Undo last action";
                unDoToolStripMenuItem.Enabled = false;
                unDo2ToolStripMenuItem.Text = Localization.GetString("mainInfoUndo");    //"Undo last action";
                unDo2ToolStripMenuItem.Enabled = false;
                unDo3ToolStripMenuItem.Text = Localization.GetString("mainInfoUndo");    //"Undo last action";
                unDo3ToolStripMenuItem.Enabled = false;
            }
        }

        /* StatusStripSet messages:
         * 0: import, loggingEnabled, start import
         * 1: Key-usage, grblLastMessage, importOptions, setEditMode, fileLoading
         * 2: Exception, Marked group, Toggle penup, conversionInfo, clrEditmode
         */
        private void StatusStripSet(int nr, string text, Color color)
        {
            try
            {
                if (nr == 0)
                {
                    if (toolStripStatusLabel0 == null) return;
                    if (toolStripStatusLabel0.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel0.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel0.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel0.Text = "[ " + text + " ]"; toolStripStatusLabel0.BackColor = color; }); }
                    else
                    { toolStripStatusLabel0.Text = "[ " + text + " ]"; toolStripStatusLabel0.BackColor = color; }
                }
                else if (nr == 1)
                {
                    if (toolStripStatusLabel1 == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel1.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel1.Text = "[ " + text + " ]"; toolStripStatusLabel1.BackColor = color; }); }
                    else
                    { toolStripStatusLabel1.Text = "[ " + text + " ]"; toolStripStatusLabel1.BackColor = color; }
                }
                else if (nr == 2)
                {
                    if (toolStripStatusLabel2 == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel2.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel2.Text = "[ " + text + " ]"; toolStripStatusLabel2.BackColor = color; }); }
                    else
                    { toolStripStatusLabel2.Text = "[ " + text + " ]"; toolStripStatusLabel2.BackColor = color; }
                }
            }
            catch (Exception err)
            { Logger.Error(err, "StatusStripSet nr:{0}, text:'{1}', color:{2}", nr, text, color.ToString()); }
        }

        private void StatusStripColor(int nr, Color color)
        {
            try
            {
                if (nr == 0)
                {
                    if (toolStripStatusLabel0 == null) return;
                    if (toolStripStatusLabel0.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel0.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel0.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel0.BackColor = color; }); }
                    else
                    { toolStripStatusLabel0.BackColor = color; }
                }
                else if (nr == 1)
                {
                    if (toolStripStatusLabel1 == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel1.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel1.BackColor = color; }); }
                    else
                    { toolStripStatusLabel1.BackColor = color; }
                }
                else if (nr == 2)
                {
                    if (toolStripStatusLabel2 == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel2.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel2.BackColor = color; }); }
                    else
                    { toolStripStatusLabel2.BackColor = color; }
                }
            }
            catch (Exception err)
            { Logger.Error(err, "StatusStripColor nr:{0}, color:{1}", nr, color.ToString()); }
        }

        private void StatusStripClear(int nr1, int nr2 = -1)//, string rem="")
        {
            if ((nr1 == 0) || (nr2 == 0))
            {
                if (toolStripStatusLabel0 == null) return;
                toolStripStatusLabel0.Text = ""; toolStripStatusLabel0.BackColor = SystemColors.Control; toolStripStatusLabel0.ToolTipText = "";
            }
            if ((nr1 == 1) || (nr2 == 1))
            {
                if (toolStripStatusLabel1 == null) return;
                toolStripStatusLabel1.Text = ""; toolStripStatusLabel1.BackColor = SystemColors.Control; toolStripStatusLabel1.ToolTipText = "";
            }
            if ((nr1 == 2) || (nr2 == 2))
            {
                if (toolStripStatusLabel2 == null) return;
                toolStripStatusLabel2.Text = ""; toolStripStatusLabel2.BackColor = SystemColors.Control; toolStripStatusLabel2.ToolTipText = "";
            }
        }
        private void StatusStripClear()
        {
            toolStripStatusLabel0.Text = toolStripStatusLabel1.Text = toolStripStatusLabel2.Text = "";
            toolStripStatusLabel0.BackColor = toolStripStatusLabel1.BackColor = toolStripStatusLabel2.BackColor = SystemColors.Control;
        }

        private void CopyContentToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Graphics g = Graphics.FromImage(img);
            Point pB1 = PointToScreen(pictureBox1.Location);
            Point pB2 = new Point(pB1.X + splitContainer1.Panel2.Left + tLPRechtsUnten.Left + 6, pB1.Y + tLPRechts.Location.Y + splitContainer1.Panel2.Top + tLPRechtsUnten.Top + 30);
            g.CopyFromScreen(pB2, new Point(0, 0), new Size(pictureBox1.Width, pictureBox1.Height));

            Clipboard.SetImage(img);
            img.Dispose();
            g.Dispose();
        }

        private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (pictureBox1.Focused && ((e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down) ||
                (e.KeyCode == Keys.Alt) || (e.KeyCode == Keys.Shift) || (e.KeyCode == Keys.Control)))
            { e.IsInputKey = true; }
        }

        private void ShowFormsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _serial_form?.BringToFront();
            if (_text_form != null) { _text_form.WindowState = FormWindowState.Normal; _text_form.BringToFront(); }
            if (_image_form != null) { _image_form.WindowState = FormWindowState.Normal; _image_form.BringToFront(); }
            if (_shape_form != null) { _shape_form.WindowState = FormWindowState.Normal; _shape_form.BringToFront(); }
            if (_wireCutter_form != null) { _wireCutter_form.WindowState = FormWindowState.Normal; _wireCutter_form.BringToFront(); }
            if (_barcode_form != null) { _barcode_form.WindowState = FormWindowState.Normal; _barcode_form.BringToFront(); }

            if (_setup_form != null) { _setup_form.WindowState = FormWindowState.Normal; _setup_form.BringToFront(); }
            if (_camera_form != null) { _camera_form.WindowState = FormWindowState.Normal; _camera_form.BringToFront(); }
            if (_coordSystem_form != null) { _coordSystem_form.WindowState = FormWindowState.Normal; _coordSystem_form.BringToFront(); }
            if (_laser_form != null) { _laser_form.WindowState = FormWindowState.Normal; _laser_form.BringToFront(); }
            if (_probing_form != null) { _probing_form.WindowState = FormWindowState.Normal; _probing_form.BringToFront(); }
            if (_heightmap_form != null) { _heightmap_form.WindowState = FormWindowState.Normal; _heightmap_form.BringToFront(); }
            if (_grbl_setup_form != null) { _grbl_setup_form.WindowState = FormWindowState.Normal; _grbl_setup_form.BringToFront(); }
            if (_process_form != null) { _process_form.WindowState = FormWindowState.Normal; _process_form.BringToFront(); }
            if (_grbl_setup_form != null) { _grbl_setup_form.WindowState = FormWindowState.Normal; _grbl_setup_form.BringToFront(); }
            //   _streaming_form.SendToBack();
        }

        private void CbAddGraphic_CheckedChanged(object sender, EventArgs e)
        {
            if (CbAddGraphic.Checked)
                CbAddGraphic.BackColor = Color.Yellow;
            else
                CbAddGraphic.BackColor = Color.Transparent;
        }

    }
}

