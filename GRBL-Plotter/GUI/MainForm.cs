/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

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
using System.Timers;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {
        ControlSerialForm _serial_form = null;
        splashscreen _splashscreen = null;

        MessageForm _message_form = null;

        private const string appName = "GRBL Plotter";
        private const string fileLastProcessed = "lastProcessed";
        private xyzPoint posProbe = new xyzPoint(0, 0, 0);
        private double? alternateZ = null;
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
        private int mainTimerCount = 0;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static uint logFlags = 0;
        private static bool logEnable = false;
        private static bool logDetailed = false;
		private static bool logPosEvent = false;
		private static bool logStreaming = false;


		private void updateLogging()
		{	// LogEnable { Level1=1, Level2=2, Level3=4, Level4=8, Detailed=16, Coordinates=32, Properties=64, Sort = 128, GroupAllGraphics = 256, ClipCode = 512, PathModification = 1024 }
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
			logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnable.Level4) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnable.PathModification) > 0);
			logStreaming = logEnable && ((logFlags & (uint)LogEnable.ClipCode) > 0);
		}

        public MainForm()
        {	updateLogging();
            Logger.Info("###### GRBL-Plotter Ver. {0} START ######", Application.ProductVersion);
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnable.Level4) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnable.Detailed) > 0);

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
            MessageBox.Show(ex.Message + "\r\n\r\n" + GetAllFootprints(ex) + "\r\n\r\nCheck " + Application.StartupPath + "\\logfile.txt", "Main Form Thread exception");
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
                MessageBox.Show(ex.Message + "\r\n\r\n" + GetAllFootprints(ex) + "\r\n\r\nCheck " + Application.StartupPath + "\\logfile.txt", "Main Form Application exception");
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
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { this.CenterToScreen(); }
            Size = Properties.Settings.Default.mainFormSize;

            WindowState = Properties.Settings.Default.mainFormWinState;
			
			int splitDist = Properties.Settings.Default.mainFormSplitDistance;
			if ((splitDist > splitContainer1.Panel1MinSize) && (splitDist < (splitContainer1.Width - splitContainer1.Panel2MinSize)))
				splitContainer1.SplitterDistance = splitDist;

            this.Text = appName + " Ver. " + System.Windows.Forms.Application.ProductVersion.ToString();
            toolTip1.SetToolTip(this, this.Text);

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
            {   Button b = new Button();
                b.Text = "b" + i;
                b.Name = "btnCustom" + i.ToString();
                b.Width = btnCustom1.Width - 20;
				b.MouseDown+= btnCustomButton_Click;
                CustomButtons17.Add(i, b);
                setCustomButton(b, Properties.Settings.Default["guiCustomBtn" + i.ToString()].ToString(), i);
                flowLayoutPanel1.Controls.Add(b);
            }
			
            loadSettings(sender, e);    // includes loadHotkeys();

            cmsPicBoxEnable(false);
            
//            updateControls();           // default: all disabled (_serial_form = null) 2020-12-11
            
            LoadRecentList();
            cmsPicBoxReloadFile.ToolTipText = string.Format("Load '{0}'", MRUlist[0]);
            LoadExtensionList();

            try { 	if (ControlGamePad.Initialize())
						Logger.Info("GamePad found");
				}
            catch (Exception er) { Logger.Error(er, " MainForm_Load - ControlGamePad.Initialize"); }

            foreach (string item in MRUlist)
            {   ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);  //create new menu for each item in list
                toolStripMenuItem2.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            }

            if (Properties.Settings.Default.guiCheckUpdate)
            {   statusStripSet(2,Localization.getString("statusStripeCheckUpdate"), Color.LightGreen);
                checkUpdate.CheckVersion();     // check update
            }

            grbl.init();                    // load and set grbl messages
            toolTable.init();               // fill structure
            gui.resetVariables();			// will be filled in MainFormLoadFile.cs 1617
            Logger.Trace("MainForm_Load finish, start splashScreen timer");

            mainTimerCount = 0;
            SplashScreenTimer.Enabled = true;
            SplashScreenTimer.Stop();
            SplashScreenTimer.Start();  // 1st event after 1500
        }

        private void SplashScreenTimer_Tick(object sender, EventArgs e)
        {
            if (_splashscreen != null)          // 2nd occurance, hide splashscreen windows
            {
                this.Opacity = 100;
                if (_serial_form == null)
                {
                    if (Properties.Settings.Default.ctrlUseSerial2)
                    {   _serial_form2 = new ControlSerialForm("COM Tool changer", 2);
                        _serial_form2.Show(this);
                    }
                    if (Properties.Settings.Default.ctrlUseSerial3)
                    { _serial_form3 = new SimpleSerialForm();// "COM simple", 3);
                        _serial_form3.Show(this);
                    }
                    _serial_form = new ControlSerialForm("COM CNC", 1, _serial_form2, _serial_form3);
                    _serial_form.Show(this);
                    _serial_form.RaisePosEvent += OnRaisePosEvent;
                    _serial_form.RaiseStreamEvent += OnRaiseStreamEvent;
                }
                if (Properties.Settings.Default.ctrlUseSerialDIY)
                { DIYControlopen(sender, e); }
                _splashscreen.Close();
                _splashscreen.Dispose();
                _splashscreen = null;
                Logger.Info("++++++ MainForm SplashScreen closed          -> mainTimer:{0}", mainTimerCount);

                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    Logger.Info("Load file via CommandLineArgs[1] {0}", args[1]);
                    loadFile(args[1]);
                }
                SplashScreenTimer.Stop();
                SplashScreenTimer.Interval = 2000;
                SplashScreenTimer.Start();
                resetStreaming(false);
                timerUpdateControls = true;
            }
            else
            {   SplashScreenTimer.Enabled = false;      // 1st occurance, show splashscreen windows
                statusStripClear(2, 2);
                Logger.Info("++++++ MainForm SplashScreen Timer disabled  -> mainTimer:{0}", mainTimerCount);
                timerUpdateControlSource = "SplashScreenTimer_Tick";
    //            updateControls();
	//			updateLayout();
                MainTimer.Stop();
                MainTimer.Start();
            }
        }

        // close Main form
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {   // Note all other forms will be closed, before reaching following code...
            Logger.Trace("###### FormClosing ");
            //_jogPathCreator_form.Close();
            //_serial_form3.Close();

            Properties.Settings.Default.mainFormWinState = WindowState;
            WindowState = FormWindowState.Normal;
            Properties.Settings.Default.mainFormSize = Size;
            Properties.Settings.Default.locationMForm = Location;
            ControlPowerSaving.EnableStandby();
            Properties.Settings.Default.mainFormSplitDistance = splitContainer1.SplitterDistance;

            saveSettings();
            Logger.Info("###### GRBL-Plotter STOP ######", Application.ProductVersion);
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Logger.Info("###+++ GRBL-Plotter FormClosed +++###", Application.ProductVersion);
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Trace("##**** GRBL-Plotter EXIT");
            this.Close();
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

        private void sendCommands(string txt, bool jogging = false) 
        {
            if (txt.Contains(";"))
            {   string[] commands = txt.Split(';');
                foreach (string cmd in commands)
                { if (cmd.Length > 0) sendCommand(cmd.Trim(), jogging); }
            }
            else
                sendCommand(txt, jogging);
        }

        private void sendCommand(string txt, bool jogging = false)
        {
            if ((jogging) && (grbl.isVersion_0 == false))   // includes (grbl.isMarlin == false)
                txt = "$J=" + txt;
            txt = gui.insertVariable(txt);			// will be filled in MainFormLoadFile.cs 1617, defined in MainFormObjects.cs
            if (!_serial_form.requestSend(txt))     // check if COM is still open
            {   timerUpdateControlSource = "sendCommand";
                updateControls();
            }

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
        // update 500ms
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (timerUpdateControls)
            {   timerUpdateControls = false;
                Logger.Trace("MainTimer_Tick - timerUpdateControls {0}", timerUpdateControlSource);
                updateLayout();
                updateControls();       // enable controls if serial connected
//                resizeJoystick();       // shows / hide A,B,C joystick controls
                Invalidate();
            }

            if (isStreaming)
            {   elapsed = DateTime.UtcNow - timeInit;
                lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
				
				if(signalShowToolExchangeMessage)
				{	signalShowToolExchangeMessage=false;
                    showToolChangeMessage();
				}
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
            {   if (delayedSend-- == 1)
                {  // _serial_form.addToLog("* Code from [Setup - Flow control]");
                    _serial_form.addToLog("* Code after pause/stop: " + Properties.Settings.Default.flowControlText + " [Setup - Program behavior - Flow control]");
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
            mainTimerCount++;
        }

		
		


        #region GUI Objects

        // Setup Custom Buttons during loadSettings()
        string[] btnCustomCommand = new string[33];
        private int setCustomButton(Button btn, string text, int cnt)
        {
            int index = Convert.ToUInt16(btn.Name.Substring("btnCustom".Length));
            if (text.Contains("|"))
            {   string[] parts = text.Split('|');
                Color btnColor = Control.DefaultBackColor;
                btn.Text = parts[0];
                if (parts.Count() > 2)
                {
                    if (parts[2].Length > 3)
                    {
                        Color tmp = Control.DefaultBackColor; //SystemColors.Control;
                        try
                        { tmp = ColorTranslator.FromHtml(parts[2]); }
                        catch
                        { tmp = Control.DefaultBackColor; }
                        btnColor = tmp;
                    }
                }
                setButtonColors(btn, btnColor);

                if (parts[1].Length > 0)
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
                    { toolTip1.SetToolTip(btn, parts[0] + "\r\n" + parts[1].Replace(";","\r\n")); }
                }
                else
                {
                    toolTip1.SetToolTip(btn, "Right click to change content");
                }

                btnCustomCommand[index] = parts[1];
                return parts[0].Trim().Length;// + parts[1].Trim().Length;
            }
            else
                btnCustomCommand[index] = "";
            return 0;
        }
        private void setButtonColors(Button btn, Color col)
        {   btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
            if (col==Control.DefaultBackColor)
                btn.UseVisualStyleBackColor = true;
        }
        private Color ContrastColor(Color color)
        {   int d = 0;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

        private void btnCustomButton_Click(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;
            int index = Convert.ToUInt16(clickedButton.Name.Substring("btnCustom".Length));
            if (e.Button == MouseButtons.Right)
            {
                using (ButtonEdit f = new ButtonEdit(index))
                {   var result = f.ShowDialog(this);
                    if (result == DialogResult.OK)
                    { timerUpdateControlSource = "btnCustomButton_Click"; updateControls(); updateLayout(); }
                }
            }
            else
            {   if (clickedButton.Text != "")
                { 	processCommands(btnCustomCommand[index]); }
            }
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
                String s = "G91 ";
                if (grbl.isMarlin) { s += ";G1 "; }
                if (index_X == 0)
                    s += String.Format("Y{0} F{1}", strY, speed).Replace(',', '.');
                else if (index_Y == 0)
                    s += String.Format("X{0} F{1}", strX, speed).Replace(',', '.');
                else
                    s += String.Format("X{0} Y{1} F{2}", strX, strY, speed).Replace(',', '.');

                sendCommands(s, true);
            }
        }
        private void virtualJoystickXY_MouseUp(object sender, MouseEventArgs e)
        { if (!grbl.isVersion_0 && cBSendJogStop.Checked) sendRealtimeCommand(133); }
        private void btnJogStop_Click(object sender, EventArgs e)
        { if (!grbl.isVersion_0) sendRealtimeCommand(133); }    //0x85

        private void virtualJoystickXY_Enter(object sender, EventArgs e)
        { if (grbl.isVersion_0) sendCommands("G91;G1F100");
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
                String s = "G91 ";
                if (grbl.isMarlin) { s += ";G1 "; }
                s += String.Format("Z{0} F{1}", strZ, speed).Replace(',', '.');
                sendCommands(s, true);
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
                String s = "G91 ";
                if (grbl.isMarlin) { s += ";G1 "; }
                s += String.Format("{0}{1} F{2}", name, strZ, speed).Replace(',', '.');
                sendCommands(s, true);
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
        {   if (grbl.isMarlin)
                sendCommand("G28");
            else
                sendCommand("$H"); }
        private void btnZeroX_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin?"G92":zeroCmd) + " X0"); }
        private void btnZeroY_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin ? "G92" : zeroCmd) + " Y0"); }
        private void btnZeroZ_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin ? "G92" : zeroCmd) + " Z0"); }
        private void btnZeroA_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin ? "G92" : zeroCmd) + " " + ctrl4thName + "0"); }
        private void btnZeroB_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin ? "G92" : zeroCmd) + " B0"); }
        private void btnZeroC_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin ? "G92" : zeroCmd) + " C0"); }
        private void btnZeroXY_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin ? "G92" : zeroCmd) + " X0 Y0"); }
        private void btnZeroXYZ_Click(object sender, EventArgs e)
        { sendCommands((grbl.isMarlin ? "G92" : zeroCmd) + " X0 Y0 Z0"); }

        private void btnJogX_Click(object sender, EventArgs e)
        { btnMoveZero("X0", joystickXYSpeed[5].ToString()); }
        private void btnJogY_Click(object sender, EventArgs e)
        { btnMoveZero("Y0", joystickXYSpeed[5].ToString()); }
        private void btnJogZ_Click(object sender, EventArgs e)
        { btnMoveZero("Z0", joystickZSpeed[5].ToString()); }
        private void btnJogZeroA_Click(object sender, EventArgs e)
        { btnMoveZero(ctrl4thName+"0", joystickZSpeed[5].ToString()); }
        private void btnJogXY_Click(object sender, EventArgs e)
        { btnMoveZero("X0Y0", joystickXYSpeed[5].ToString()); }

        private void btnMoveZero(string axis, string fed){
            string seperate = "";
            string mode = "";
            if (grbl.isMarlin) { seperate += ";"; mode = "G1"; }
            string feed = "F" + fed;

            if (cBMoveG0.Checked){
                sendCommands(string.Format("G90{0} G0 {1}", seperate, axis)); }
            else{
                sendCommands(string.Format("G90{0}{1} {2} {3}", seperate, mode, axis, feed), true); }
        }

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
            timerUpdateControlSource = "grblFeedHold";
            updateControls(true);
        }
        private void btnResume_Click(object sender, EventArgs e)
        { grblResume(); statusStripClear(); }

        private void grblResume()
        {   sendRealtimeCommand('~');
            Logger.Trace("Resume");
            btnResume.BackColor = SystemColors.Control;
            signalResume = 0;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            timerUpdateControlSource = "grblResume";
            updateControls();
        }
        private void btnKillAlarm_Click(object sender, EventArgs e)
        {   grblKillAlarm(); }
        private void grblKillAlarm()
        {   sendCommand("$X");
            Logger.Trace("KillAlarm");
            signalLock = 0;
            btnKillAlarm.BackColor = SystemColors.Control;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            timerUpdateControlSource = "grblKillAlarm";
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
        {   if (command.Length <= 1)
                return;
            if (!_serial_form.serialPortOpen)
            {   _serial_form.addToLog("serial port is closed");
                return;
            }
            string[] commands= { };
            //            Logger.Trace("processCommands");

            if (!command.StartsWith("(") && command.Contains('\\') && (!isStreaming || isStreamingPause))
            {
                if (File.Exists(command))
                {
                    string fileCmd = File.ReadAllText(command);
                    _serial_form.addToLog("* File: " + command);
                    commands = fileCmd.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }
                else
                    _serial_form.addToLog("Script/file does not exists: "+ command);
            }
            else
            {   commands = command.Split(';'); }

            if (_diyControlPad != null)
            {   _diyControlPad.isHeightProbing = false; }

            foreach (string btncmd in commands)
            {   if (btncmd.StartsWith("($") && (_diyControlPad != null))
                {
                    string tmp = btncmd.Replace("($", "[");
                    tmp = tmp.Replace(")", "]");
                    _diyControlPad.sendFeedback(tmp);
                }
                else
                {   if (!processSpecialCommands(command) && (!isStreaming || isStreamingPause))
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
            Logger.Trace("resizeJoystick() visible:  A:{0} B:{1} C:{2}", grbl.axisA, grbl.axisB, grbl.axisC);

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
            virtualJoystickXY.Invalidate();
            virtualJoystickZ.Invalidate();
            virtualJoystickA.Invalidate();
            virtualJoystickB.Invalidate();
            virtualJoystickC.Invalidate();
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
//                toolStripStatusLabel0.Text = "[ " + text + " ]";
                if (this.toolStripStatusLabel0.GetCurrentParent().InvokeRequired)
                { this.toolStripStatusLabel0.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { this.toolStripStatusLabel0.Text = "[ " + text + " ]"; }); }
                else
                { this.toolStripStatusLabel0.Text = "[ " + text + " ]"; }
                toolStripStatusLabel0.BackColor = color;
            }
            else if (nr == 1)

            {
//                toolStripStatusLabel1.Text = "[ " + text + " ]";
                if (this.toolStripStatusLabel1.GetCurrentParent().InvokeRequired)
                { this.toolStripStatusLabel1.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { this.toolStripStatusLabel1.Text = "[ " + text + " ]";  toolStripStatusLabel1.BackColor = color; }); }
                else
                { this.toolStripStatusLabel1.Text = "[ " + text + " ]"; toolStripStatusLabel1.BackColor = color; }
 //               toolStripStatusLabel1.BackColor = color;
            }
            else if (nr == 2)
            {
//                toolStripStatusLabel2.Text = "[ " + text + " ]";
                if (this.toolStripStatusLabel2.GetCurrentParent().InvokeRequired)
                { this.toolStripStatusLabel2.GetCurrentParent().BeginInvoke((MethodInvoker)delegate () { this.toolStripStatusLabel2.Text = "[ " + text + " ]"; toolStripStatusLabel2.BackColor = color; }); }
                else
                { this.toolStripStatusLabel2.Text = "[ " + text + " ]"; toolStripStatusLabel2.BackColor = color; }
        //        toolStripStatusLabel2.BackColor = color;
            }
//            Logger.Trace("statusStripSet nr {0} text {1}", nr, text);
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

        private void statusStripClear(int nr1, int nr2=-1, string rem="")
        {   if ((nr1 == 0) || (nr2 == 0))
            { toolStripStatusLabel0.Text = ""; toolStripStatusLabel0.BackColor = SystemColors.Control; toolStripStatusLabel0.ToolTipText = ""; }
            if ((nr1 == 1) || (nr2 == 1))
            { toolStripStatusLabel1.Text = ""; toolStripStatusLabel1.BackColor = SystemColors.Control; toolStripStatusLabel1.ToolTipText = ""; }
            if ((nr1 == 2) || (nr2 == 2))
            { toolStripStatusLabel2.Text = ""; toolStripStatusLabel2.BackColor = SystemColors.Control; toolStripStatusLabel2.ToolTipText = ""; }
//            Logger.Trace("statusStripClear {0} {1} {2}",nr1,nr2, rem);
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

