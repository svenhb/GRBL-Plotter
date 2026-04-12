/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.GUI;
using GrblPlotter.Helper;
using GrblPlotter.UserControls;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


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
        private bool shutDown = false;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logDetailed = false;
        private static readonly bool logPosEvent = false;
        private static bool logStreaming = false;

        internal static float DpiScaling;


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

            System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);

            this.Icon = Properties.Resources.Icon;  // set icon

            // Attention: no MessageBox during splashScreen: never visible and application waits for action!
            Logger.Info(culture, "▼▼▼▼▼▼▼▼▼▼ MainForm SplashScreen start");
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
                                                                                    //	expandGCode = Properties.ListSettings.Default.FCTBBlockExpandOnSelect;
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

            LoadRecentList();               // open Recent.txt and FillToolListElements menu
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
            MyControl.SetSelectedDevice(toolSelect);

            LoadExtensionList();			// FillToolListElements menu with available extension-scripts
            CmsPicBoxEnable(false);         // no graphic - no tasks
            DpiScaling = (float)DeviceDpi / 96;

            UserControlsInitialize();       // add event listeners

            try
            {
                if (ControlGamePad.Initialize())
                    Logger.Info(culture, "GamePad found");
            }
            catch (Exception err) { Logger.Error(err, " MainForm - ControlGamePad.Initialize "); }

            Grbl.Init();                    // load and set grbl messages in grblRelated.cs
            CodeMessage.Init();

            GuiVariables.ResetVariables();	// set variables in MainFormObjects.cs			

            if (Properties.Settings.Default.guiExtendedLoggingEnabled || Properties.Settings.Default.guiExtendedLoggingCOMEnabled)
                StatusStripSet(0, "Logging enabled", Color.Yellow);

            LoadProperties.Init();
        }

        // initialize Main form
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Use the Constructor in a Windows Form for ensuring that initialization is done properly.
            // Use load event: code that requires the window size and location to be known.
            SetGUISize();               // resize GUI arcodring last size and check if within display in MainFormUpdate.cs

            if (Properties.Settings.Default.guiCheckUpdate)
            {
                StatusStripSet(2, Localization.GetString("statusStripeCheckUpdate"), Color.LightGreen);
                CheckUpdate.CheckVersion(false, Properties.Settings.Default.guiLastEndReason);     // check update
            }
            UserControlsMainFormLoad();     // apply visible height

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
                Logger.Info(culture, "▲▲▲▲ MainForm SplashScreen closed          -> mainTimer:{0}", mainTimerCount);

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
                Logger.Info(culture, "▲▲▲▲▲▲▲▲▲▲ MainForm SplashScreen Timer disabled  -> mainTimer:{0}", mainTimerCount);
                timerUpdateControlSource = "SplashScreenTimer_Tick";

                splitContainer2.SplitterDistance = Properties.Settings.Default.DeviceLaserSplitterDistance;
             
                MainTimer.Stop();
                MainTimer.Start();
            }
        }

        // close Main form
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {   // Note all other forms will be closed, before reaching following code...
            shutDown = true;
            Logger.Info("###### FormClosing ");
            if (isStreaming)
                EventCollector.SetStreaming("CLOST");
            else
                EventCollector.SetStreaming("CLOSE");

            loadTimer.Stop();
            Properties.Settings.Default.mainFormWinState = WindowState;
            Properties.Settings.Default.mainFormSize = Size;
            Properties.Settings.Default.locationMForm = Location;
            Properties.Settings.Default.guiToolSelection = tC_RouterPlotterLaser.SelectedIndex;
            Properties.Settings.Default.mainFormSplitDistance = splitContainer1.SplitterDistance;
            Properties.Settings.Default.DeviceLaserSplitterDistance = splitContainer2.SplitterDistance;
            Properties.Settings.Default.guiLastEnd = DateTime.Now.Ticks;
            Logger.Trace("Save Split2:{0}", splitContainer2.SplitterDistance);
            SaveSettings();

            WindowState = FormWindowState.Normal;
            ControlPowerSaving.EnableStandby();
            Logger.Info("###### GRBL-Plotter STOP ######");

            DeleteTempFile();
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
            //    lbInfo.Text = lastLabelInfoText + overrideMessage;
            SetInfoLabel(lastLabelInfoText + overrideMessage);
        }

        private void ShowLaserMode()
        {
            if (!Grbl.isVersion_0 && _serial_form.IsLasermode)
            {
                SetInfoLabel(Localization.GetString("mainInfoLaserModeOn"), Color.Fuchsia);
            }
            else
            {
                SetInfoLabel(Localization.GetString("mainInfoLaserModeOff"), Color.Lime);
            }
        }
        // update 500ms
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (shutDown) { return; }

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
                //     elapsed = DateTime.UtcNow - timeInit;
                //     lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss", culture);

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
                    System.Windows.Forms.Application.DoEvents();
                }
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
                    ucStreaming.SetStatusTextGrbl("", SystemColors.Control);
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

     
        private void BtnJogStop_Click(object sender, EventArgs e)
        { if (!Grbl.isVersion_0) SendRealtimeCommand(133); }    //0x85


        private void BtnReset_Click()
        {
            if (_serial_form.IsConnectedToGrbl())
            {
                Logger.Trace("BtnReset_Click  IsConnectedToGrbl");
                _serial_form.GrblReset(true);   // savePos
            }
            isStreaming = false;
            ucStreaming.ResetProgress();
            ucFlowControl.HighlightResume(false);
            SetInfoLabel("", SystemColors.Control);
            /*    CbSpindle.CheckedChanged -= CbSpindle_CheckedChanged;
                CbSpindle.Checked = false;
                CbSpindle.CheckedChanged += CbSpindle_CheckedChanged;
                CbCoolant.CheckedChanged -= CbSpindle_CheckedChanged;
                CbCoolant.Checked = false;
                CbCoolant.CheckedChanged += CbSpindle_CheckedChanged;
            */
            UpdateControlEnables();
            ControlPowerSaving.EnableStandby();
        }
     
        private void GrblFeedHold()
        {
            SendRealtimeCommand('!');
            Logger.Trace("FeedHold");
            //     signalResume = 1;
            timerUpdateControlSource = "grblFeedHold";
            UpdateControlEnables();	// true overwrite streaming
        }
     

        private void GrblResume()
        {
            SendRealtimeCommand('~');
            Logger.Trace("Resume");
            //   btnResume.BackColor = SystemColors.Control;
            ucFlowControl.HighlightResume(false);
            //   signalResume = 0;
            //    lbInfo.Text = "";
            //    lbInfo.BackColor = SystemColors.Control;
            SetInfoLabel("", SystemColors.Control);
            timerUpdateControlSource = "grblResume";
            UpdateControlEnables();
        }
      
        private void GrblKillAlarm()
        {
            SendCommand("$X");
            Logger.Trace("KillAlarm");
            //     signalLock = 0;
            //     btnKillAlarm.BackColor = SystemColors.Control;
            ucFlowControl.HighlightKillAlarm(false);
            //    lbInfo.Text = "";
            //    lbInfo.BackColor = SystemColors.Control;
            SetInfoLabel("", SystemColors.Control);
            timerUpdateControlSource = "grblKillAlarm";
            UpdateControlEnables();
        }
        #endregion

 

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
                    _serial_form.AddToLog("Script/file does not exists: " + Path.GetFullPath(command));
                    Logger.Warn("ProcessCommands Script/file does not exists: {0}  {1}", command, Path.GetFullPath(command));
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
            else if (command.ToLower(culture).IndexOf("#stop") >= 0) { StopStreaming(true); UpdateLogging(); commandFound = true; }
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
                ResizeRightSide("MainForm_Resize");

            for (int i = 17; i <= 32; i++)
            {
                if (CustomButtons17.ContainsKey(i))
                {
                    Button b = CustomButtons17[i];
                    b.Width = btnCustom1.Width - 24;
                    b.Height = btnCustom1.Height;
                }
            }
        }

        private readonly bool gBoxDROShowSetCoord = false;

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
                    { toolStripStatusLabel0.Text = "[ " + text + " ]"; toolStripStatusLabel0.BackColor = color; toolStripStatusLabel0.ForeColor = Colors.ContrastColor(color); }
                }
                else if (nr == 1)
                {
                    if (toolStripStatusLabel1 == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel1.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel1.Text = "[ " + text + " ]"; toolStripStatusLabel1.BackColor = color; }); }
                    else
                    { toolStripStatusLabel1.Text = "[ " + text + " ]"; toolStripStatusLabel1.BackColor = color; toolStripStatusLabel1.ForeColor = Colors.ContrastColor(color); }
                }
                else if (nr == 2)
                {
                    if (toolStripStatusLabel2 == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel2.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel2.Text = "[ " + text + " ]"; toolStripStatusLabel2.BackColor = color; }); }
                    else
                    { toolStripStatusLabel2.Text = "[ " + text + " ]"; toolStripStatusLabel2.BackColor = color; toolStripStatusLabel2.ForeColor = Colors.ContrastColor(color); }
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
                    { toolStripStatusLabel0.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel0.BackColor = color; toolStripStatusLabel0.ForeColor = Colors.ContrastColor(color); }); }
                    else
                    { toolStripStatusLabel0.BackColor = color; }
                }
                else if (nr == 1)
                {
                    if (toolStripStatusLabel1 == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel1.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel1.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel1.BackColor = color; toolStripStatusLabel1.ForeColor = Colors.ContrastColor(color); }); }
                    else
                    { toolStripStatusLabel1.BackColor = color; }
                }
                else if (nr == 2)
                {
                    if (toolStripStatusLabel2 == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent() == null) return;
                    if (toolStripStatusLabel2.GetCurrentParent().InvokeRequired)
                    { toolStripStatusLabel2.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { toolStripStatusLabel2.BackColor = color; toolStripStatusLabel2.ForeColor = Colors.ContrastColor(color); }); }
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
            System.Drawing.Image img = new Bitmap(pictureBox1.Width, pictureBox1.Height);

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
            if (_tablet_form != null) { _tablet_form.WindowState = FormWindowState.Normal; _tablet_form.BringToFront(); }

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
            {
                CbAddGraphic.BackColor = Color.Yellow;
                CbAddGraphic.ForeColor = Colors.ContrastColor(Color.Yellow);
            }
            else
            {
                CbAddGraphic.BackColor = Color.Transparent;
                CbAddGraphic.ForeColor = MyControl.PanelForeColor;
            }
        }

    }
}

