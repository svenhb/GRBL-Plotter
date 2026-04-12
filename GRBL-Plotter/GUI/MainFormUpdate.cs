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
/* MainFormUpdate
 * Update controls etc.
 * 2021-07-26 new
 * 2021-09-19 line 389 change order of virtualJoystickXY.Enable
 * 2021-11-18 add processing of accessory D0-D3 from grbl-Mega-5X - line 210
 * 2021-11-22 change reg-key to get data-path from installation
 * 2021-12-14 change log path
 * 2023-01-10 line 132 get user.config path, change from PerUserRoaming to PerUserRoamingAndLocal
 * 2023-03-07 l:480 f:JoystickSetup() set joystick raster; l:560 f:UpdateControlEnablesInvoked show/hide stop
 * 2025-03-04 l:185 f:SetGUISize replace this.Text by showFormText();
 *            l:611 f:UpdateControlEnablesInvoked add showFormText()
 * 2025-03-14 l:235 f:LoadSettings skip if shutDown
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.UserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        private void UpdateLogging()
        {	// LogEnable { Level1=1, Level2=2, Level3=4, Level4=8, Detailed=16, Coordinates=32, Properties=64, Sort = 128, GroupAllGraphics = 256, ClipCode = 512, PathModification = 1024 }
            logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level4) > 0);
            logDetailed = logEnable && ((logFlags & (uint)LogEnables.PathModification) > 0);
            logStreaming = logEnable && ((logFlags & (uint)LogEnables.ClipCode) > 0);

            Gcode.LoggerTrace = Properties.Settings.Default.guiExtendedLoggingEnabled;
            Gcode.LoggerTraceImport = Gcode.LoggerTrace && Properties.Settings.Default.importGCAddComments;

            if (Gcode.LoggerTrace || Properties.Settings.Default.guiExtendedLoggingCOMEnabled)
            {
                foreach (var rule in NLog.LogManager.Configuration.LoggingRules)
                {
                    rule.EnableLoggingForLevel(NLog.LogLevel.Trace);
                    rule.EnableLoggingForLevel(NLog.LogLevel.Debug);
                }
                NLog.LogManager.ReconfigExistingLoggers();
            }
        }

        private void GetAppDataPath()
        {   // default: System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.CommonAppDataPath); 
            const string reg_key_user = "HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter";
            const string reg_key_admin = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\GRBL-Plotter";
            const string reg_key_admin2 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\GRBL-Plotter";
            string newPathUser = "", newPathAdmin = "", newPathAdmin2 = "";
            try
            { newPathUser = (string)Registry.GetValue(reg_key_user, "DataPath", 0); }
            catch { }
            try
            { newPathAdmin = (string)Registry.GetValue(reg_key_admin, "DataPath", 0); }
            catch { }
            try
            { newPathAdmin2 = (string)Registry.GetValue(reg_key_admin2, "DataPath", 0); }
            catch { }

            if (!string.IsNullOrEmpty(newPathUser))
            {
                Datapath.AppDataFolder = newPathUser;                   // get path from registry
                LogAppDataPath(reg_key_user);
                EventCollector.SetInstalled("HKCU");
            }
            else if (!string.IsNullOrEmpty(newPathAdmin))
            {
                Datapath.AppDataFolder = newPathAdmin;                   // get path from registry
                LogAppDataPath(reg_key_admin);
                EventCollector.SetInstalled("HKLM-WOW");
            }
            else if (!string.IsNullOrEmpty(newPathAdmin2))
            {
                Datapath.AppDataFolder = newPathAdmin2;                   // get path from registry
                LogAppDataPath(reg_key_admin2);
                EventCollector.SetInstalled("HKLM");
            }
            else // no setup?
            {
                if (Datapath.Application.StartsWith("C:\\Program"))
                {
                    newPathUser = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GRBL_Plotter";
                    string oldPathData = Datapath.Application + "\\data";

                    Datapath.AppDataFolder = newPathUser;                   // set path to my documents
                    LogAppDataPath("fall back");
                    Logger.Warn("GetAppDataPath: current path starts with C:\\Program, which is probably write protected: {0}", Datapath.Application);
                    Logger.Warn("GetAppDataPath: switch to new path {0}", newPathUser);

                    DirectoryInfo dir = new DirectoryInfo(newPathUser + "\\data");
                    if (!dir.Exists)
                    {
                        Logger.Warn("GetAppDataPath: copy data-folders from {0} to {1}", oldPathData, newPathUser + "\\data");
                        DirectoryCopy(oldPathData, newPathUser + "\\data", true);
                    }

                    Registry.SetValue(reg_key_user, "DataPath", newPathUser);           // store for next prog-start				
                    EventCollector.SetInstalled("FallBack", true);
                }
                else
                {
                    Datapath.AppDataFolder = Datapath.Application;      // use application path
                    LogAppDataPath("Default");
                    EventCollector.SetInstalled("COPY", true);
                }
            }
        }
        private void LogAppDataPath(string src)
        {
            NLog.LogManager.Configuration.Variables["basedir"] = Datapath.LogFiles;
            Logger.Info("GetAppDataPath from {0}: {1}", src, Datapath.AppDataFolder);
            Logger.Info("Application path: {0}", Datapath.Application);
            Logger.Info("user.config path: {0}", ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            //   Logger.Info("DirectoryCopy {0}  to  {1}", sourceDirName, destDirName);
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        private void SetGUISize()
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            Location = Properties.Settings.Default.locationMForm;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { this.CenterToScreen(); }
            this.Size = Properties.Settings.Default.mainFormSize;

            this.WindowState = Properties.Settings.Default.mainFormWinState;

            int splitDist = Properties.Settings.Default.mainFormSplitDistance;
            if ((splitDist > splitContainer1.Panel1MinSize) && (splitDist < (splitContainer1.Width - splitContainer1.Panel2MinSize)))
                splitContainer1.SplitterDistance = splitDist;
            splitContainer2.SplitterDistance = Properties.Settings.Default.DeviceLaserSplitterDistance;

            Logger.Trace("SetGUISize X:{0}  Y:{1}  Split1:{2}  Split2:{3}", Width, Height, splitContainer1.SplitterDistance, splitContainer2.SplitterDistance);
            ShowFormText();
            toolTip1.SetToolTip(this, this.Text);
        }

        private void RemoveCursorNavigation(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                ctrl.PreviewKeyDown += new PreviewKeyDownEventHandler(MainForm_PreviewKeyDown);
                RemoveCursorNavigation(ctrl.Controls);
            }
        }

        private void SetMenuShortCuts()
        {
            loadToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            loadMachineParametersToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Control | Keys.O;
            saveMachineParametersToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.Control | Keys.S;

            cmsCodeSelect.ShortcutKeys = Keys.Control | Keys.A;
            cmsCodeCopy.ShortcutKeys = Keys.Control | Keys.C;
            cmsCodePaste.ShortcutKeys = Keys.Control | Keys.V;
            cmsCodeSendLine.ShortcutKeys = Keys.Alt | Keys.Control | Keys.M;


            toggleBlockExpansionToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.E;
            foldCodeBlocks1stLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D2;
            foldCodeBlocks2ndLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D3;
            foldCodeBlocks3rdLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D4;
            expandCodeBlocksToolStripMenuItem1.ShortcutKeys = Keys.Alt | Keys.D1;

            cmsPicBoxMoveToMarkedPosition.ShortcutKeys = Keys.Control | Keys.M;
            cmsPicBoxPasteFromClipboard.ShortcutKeys = Keys.Control | Keys.V;
            cmsPicBoxReloadFile.ShortcutKeys = Keys.Control | Keys.R;
            cmsPicBoxDuplicatePath.ShortcutKeys = Keys.Control | Keys.D;

            toolStrip_tBRadiusCompValue.Text = string.Format(culture, "{0:0.000}", Properties.Settings.Default.crcValue);

        }

        // load settings
        public void LoadSettings(object sender, EventArgs e)
        {
            if (shutDown)
                return;

            Logger.Info("LoadSettings");

            if ((_setup_form != null) && (_setup_form.settingsReloaded))
            {
                _setup_form.settingsReloaded = false;
                SetGUISize();							// if reset ALL settings was pressed (incl. form locations)
            }
            pictureBox1.Invalidate();

            UpdateWholeApplication();

            if (Properties.Settings.Default.guiExtendedLoggingEnabled || Properties.Settings.Default.guiExtendedLoggingCOMEnabled)
                StatusStripSet(0, "Logging enabled", Color.Yellow);
            else
                StatusStripClear(0);

            _projector_form?.Invalidate();
        }
        public void Update2DView(object sender, EventArgs e)
        {
            if (shutDown) return;
            Logger.Info("Update2DView");
            GraphicPropertiesSetup();
            pictureBox1.Invalidate();
        }

        private void UpdateWholeApplication()	// after ini file, setup change, update controls
        {	// Update everything which could be changed via Setup or INI-file
            Logger.Info("UpdateWholeApplication");
            UpdateLogging();

            showFormInFront = Properties.Settings.Default.guiShowFormInFront;

            fCTBCode.BookmarkColor = Properties.Settings.Default.gui2DColorMarker; ;
            fCTBCode.LineInterval = (int)Properties.Settings.Default.FCTBLineInterval;
            //tbFile.Text
            lastLoadedFileName = Properties.Settings.Default.guiLastFileLoaded;

            CustomButtonsFillWithContent();		// FillToolListElements and resize custom buttons			
            GraphicPropertiesSetup();			// pen / brush colors
            MenuTranslateSetup();				// set rotary info in menu item 'translate'

            GuiEnableAxisABC();					// enable and resize axis ABC GUI buttons (set 0)
            JoystickSetup();                    // Joystick step and speed
                                                //        JoystickResize();                   // relative size

            CommandProtocol cp = CommandProtocol.grblCanJog;
            if (Grbl.isMarlin) { cp = CommandProtocol.Marlin; }
            else if (Grbl.isVersion_0) { cp = CommandProtocol.grblOld; }
            UserControlsSetAxisCount(Grbl.axisCount, cp);

            // 2-D view settings
            toolStripViewMachine.Checked = Properties.Settings.Default.machineLimitsShow;
            toolStripViewTool.Checked = Properties.Settings.Default.gui2DToolTableShow;
            toolStripViewMachineFix.Checked = Properties.Settings.Default.machineLimitsFix;

            // grbl communication
            int[] interval = new int[] { 500, 250, 200, 125, 100 };
            int index = Properties.Settings.Default.grblPollIntervalIndex;
            if ((index >= 0) && (index < 5))
                Grbl.pollInterval = interval[index];

            gamePadTimer.Enabled = Properties.Settings.Default.gamePadEnable;
            CheckMachineLimit();
            LoadHotkeys();

            // changed PWM settings			
            GuiVariables.variable["GMIS"] = (double)Properties.Settings.Default.importGCPWMDown;
            GuiVariables.variable["GMAS"] = (double)Properties.Settings.Default.importGCPWMUp;
            GuiVariables.variable["GZES"] = (double)Properties.Settings.Default.importGCPWMZero;
            GuiVariables.variable["GCTS"] = (double)(Properties.Settings.Default.importGCPWMDown + Properties.Settings.Default.importGCPWMUp) / 2;
            GuiVariables.WriteSettingsToRegistry(); // for use within external scripts

        }   // end load settings

        private void GuiEnableAxisABC()
        {
            string axisChars = "XYZABC";

            ctrl4thAxis = Properties.Settings.Default.ctrl4thUse;
            ctrl4thName = Properties.Settings.Default.ctrl4thName;
            mirrorRotaryToolStripMenuItem.Visible = ctrl4thAxis;

            if (Grbl.axisB || Grbl.axisC)
            {
                tLPRechtsOben.ColumnStyles[0].Width = 400;

            }
            else
            {
                tLPRechtsOben.ColumnStyles[0].Width = 236;
            }
            Logger.Trace("GuiEnableAxisABC {0}", Grbl.GetInfo("AXS1", "-"));
            if (Grbl.GetInfo("AXS1", "") != "")
            {
                bool skipClonedAxis = true;                             // WPos:10.000,20.000,20.000,30.000,-125.000
                char[] chars = Grbl.GetInfo("AXS1", "").ToCharArray();  // [AXS:5:XYYZA]
                for (int i = 0; i < Math.Min(6, chars.Length); i++)
                {
                    //        axisLabels[i].Text = chars[i].ToString();
                }
                if (chars.Length < 4)
                {
                    //          label_a.Visible = label_wa.Visible = label_ma.Visible = btnZeroA.Visible = false;
                    Grbl.axisA = false;
                }
                if (chars.Length < 5)
                {
                    //         label_b.Visible = label_wb.Visible = label_mb.Visible = btnZeroB.Visible = false;
                    Grbl.axisB = false;
                }
                if (chars.Length < 6)
                {
                    //          label_c.Visible = label_wc.Visible = label_mc.Visible = btnZeroC.Visible = false;
                    Grbl.axisC = false;
                }
            }
        }
        private void MenuTranslateSetup()
        {
            skaliereXAufDrehachseToolStripMenuItem.Enabled = false;
            skaliereXAufDrehachseToolStripMenuItem.BackColor = SystemColors.Control;
            skaliereXAufDrehachseToolStripMenuItem.ToolTipText = "Enable rotary axis in Setup - Control";
            skaliereAufXUnitsToolStripMenuItem.BackColor = SystemColors.Control;
            skaliereAufXUnitsToolStripMenuItem.ToolTipText = "Enable in Setup - Control";
            skaliereYAufDrehachseToolStripMenuItem.Enabled = false;
            skaliereYAufDrehachseToolStripMenuItem.BackColor = SystemColors.Control;
            skaliereYAufDrehachseToolStripMenuItem.ToolTipText = "Enable rotary axis in Setup - Control";
            skaliereAufYUnitsToolStripMenuItem.BackColor = SystemColors.Control;
            skaliereAufYUnitsToolStripMenuItem.ToolTipText = "Enable in Setup - Control";
            toolStrip_tb_rotary_diameter.Text = string.Format("{0:0.00}", Properties.Settings.Default.rotarySubstitutionDiameter);

            if (Properties.Settings.Default.rotarySubstitutionEnable)
            {
                string tmptxt = string.Format("Calculating rotary angle depending on part diameter ({0:0.00} units) and desired size.\r\nSet part diameter in Setup - Control.", Properties.Settings.Default.rotarySubstitutionDiameter);
                if (Properties.Settings.Default.rotarySubstitutionX)
                {
                    skaliereXAufDrehachseToolStripMenuItem.Enabled = true;
                    skaliereXAufDrehachseToolStripMenuItem.BackColor = Color.Yellow;
                    skaliereAufXUnitsToolStripMenuItem.BackColor = Color.Yellow;
                    skaliereAufXUnitsToolStripMenuItem.ToolTipText = tmptxt;
                    skaliereXAufDrehachseToolStripMenuItem.ToolTipText = "";
                }
                else
                {
                    skaliereYAufDrehachseToolStripMenuItem.Enabled = true;
                    skaliereYAufDrehachseToolStripMenuItem.BackColor = Color.Yellow;
                    skaliereAufYUnitsToolStripMenuItem.BackColor = Color.Yellow;
                    skaliereAufYUnitsToolStripMenuItem.ToolTipText = tmptxt;
                    skaliereYAufDrehachseToolStripMenuItem.ToolTipText = "";
                }
            }
            if (Properties.Settings.Default.rotarySubstitutionEnable && Properties.Settings.Default.rotarySubstitutionSetupEnable)
            {
                string[] commands;
                if (Properties.Settings.Default.rotarySubstitutionEnable)
                { commands = Properties.Settings.Default.rotarySubstitutionSetupOn.Split(';'); }
                else
                { commands = Properties.Settings.Default.rotarySubstitutionSetupOff.Split(';'); }
                Logger.Info("rotarySubstitutionSetupEnable {0} [Setup - Program control - Rotary axis control]", string.Join(";", commands));
                if ((_serial_form != null) && (_serial_form.SerialPortOpen))
                    foreach (string cmd in commands)
                    {
                        SendCommand(cmd.Trim());
                        Thread.Sleep(100);
                    }
            }

        }
        private void JoystickSetup()
        {
            joystickXYStep[0] = 0;
            joystickXYStep[1] = (double)Properties.Settings.Default.guiJoystickXYStep1;
            joystickXYStep[2] = (double)Properties.Settings.Default.guiJoystickXYStep2;
            joystickXYStep[3] = (double)Properties.Settings.Default.guiJoystickXYStep3;
            joystickXYStep[4] = (double)Properties.Settings.Default.guiJoystickXYStep4;
            joystickXYStep[5] = (double)Properties.Settings.Default.guiJoystickXYStep5;
            joystickXYSpeed[0] = 0.1;
            joystickXYSpeed[1] = (double)Properties.Settings.Default.guiJoystickXYSpeed1;
            joystickXYSpeed[2] = (double)Properties.Settings.Default.guiJoystickXYSpeed2;
            joystickXYSpeed[3] = (double)Properties.Settings.Default.guiJoystickXYSpeed3;
            joystickXYSpeed[4] = (double)Properties.Settings.Default.guiJoystickXYSpeed4;
            joystickXYSpeed[5] = (double)Properties.Settings.Default.guiJoystickXYSpeed5;
            joystickZStep[0] = 0;
            joystickZStep[1] = (double)Properties.Settings.Default.guiJoystickZStep1;
            joystickZStep[2] = (double)Properties.Settings.Default.guiJoystickZStep2;
            joystickZStep[3] = (double)Properties.Settings.Default.guiJoystickZStep3;
            joystickZStep[4] = (double)Properties.Settings.Default.guiJoystickZStep4;
            joystickZStep[5] = (double)Properties.Settings.Default.guiJoystickZStep5;
            joystickZSpeed[0] = 0.1;
            joystickZSpeed[1] = (double)Properties.Settings.Default.guiJoystickZSpeed1;
            joystickZSpeed[2] = (double)Properties.Settings.Default.guiJoystickZSpeed2;
            joystickZSpeed[3] = (double)Properties.Settings.Default.guiJoystickZSpeed3;
            joystickZSpeed[4] = (double)Properties.Settings.Default.guiJoystickZSpeed4;
            joystickZSpeed[5] = (double)Properties.Settings.Default.guiJoystickZSpeed5;
            joystickAStep[0] = 0;
            joystickAStep[1] = (double)Properties.Settings.Default.guiJoystickAStep1;
            joystickAStep[2] = (double)Properties.Settings.Default.guiJoystickAStep2;
            joystickAStep[3] = (double)Properties.Settings.Default.guiJoystickAStep3;
            joystickAStep[4] = (double)Properties.Settings.Default.guiJoystickAStep4;
            joystickAStep[5] = (double)Properties.Settings.Default.guiJoystickAStep5;
            joystickASpeed[0] = 0.1;
            joystickASpeed[1] = (double)Properties.Settings.Default.guiJoystickASpeed1;
            joystickASpeed[2] = (double)Properties.Settings.Default.guiJoystickASpeed2;
            joystickASpeed[3] = (double)Properties.Settings.Default.guiJoystickASpeed3;
            joystickASpeed[4] = (double)Properties.Settings.Default.guiJoystickASpeed4;
            joystickASpeed[5] = (double)Properties.Settings.Default.guiJoystickASpeed5;
        }

        // update controls on Main form (disable if streaming or no serial)
        // private void UpdateControlEnables()
        private void UpdateControlEnables()
        {
            if (this.InvokeRequired)
            { this.BeginInvoke((MethodInvoker)delegate () { UpdateControlEnablesInvoked(); }); }
            else
            { UpdateControlEnablesInvoked(); }
        }
        private void UpdateControlEnablesInvoked()//bool allowControl)
        {
            if (shutDown) { return; }

            bool isConnected = false;
            if (_serial_form != null)
            { isConnected = _serial_form.SerialPortOpen || Grbl.grblSimulate; }

            moveTimer.Enabled = false;
            UpdateCustomButtons(true);  // isConnected && (!isStreaming || allowControl)

            bool allowControl = isStreamingPause;
            Logger.Trace("◯◯◯ updateControls isConnected:{0} isStreaming:{1} streamingAllowControl:{2} source:{3}", isConnected, isStreaming, allowControl, timerUpdateControlSource);
            timerUpdateControlSource = "";
            gCodeToolStripMenuItem.Enabled = !isStreaming;
            ucFlowControl.EnableButtons(isConnected);
            ucOverrides.EnableButtons(isConnected);

            ucMoveToGraphic.EnableButtons(isConnected);
            ucSetCoordinateSystem.EnableButtons(isConnected);
            ucMoveToZero.EnableButtons(isConnected);
            ucStreaming.EnableButtonsStreaming(isConnected);
            ucJogControlAll.EnableButtons(isConnected);
            EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
            ShowFormText();
        }


        #region Custom Buttons

        private void CustomButtonsFillWithContent()
        {
            int customButtonUse = 0;
            SetCustomButton(btnCustom1, Properties.Settings.Default.guiCustomBtn1);//, 1);
            SetCustomButton(btnCustom2, Properties.Settings.Default.guiCustomBtn2);//, 2);
            SetCustomButton(btnCustom3, Properties.Settings.Default.guiCustomBtn3);//, 3);
            SetCustomButton(btnCustom4, Properties.Settings.Default.guiCustomBtn4);//, 4);
            SetCustomButton(btnCustom5, Properties.Settings.Default.guiCustomBtn5);//, 5);
            SetCustomButton(btnCustom6, Properties.Settings.Default.guiCustomBtn6);//, 6);
            SetCustomButton(btnCustom7, Properties.Settings.Default.guiCustomBtn7);//, 7);
            SetCustomButton(btnCustom8, Properties.Settings.Default.guiCustomBtn8);//, 8);
            SetCustomButton(btnCustom9, Properties.Settings.Default.guiCustomBtn9);//, 9);
            SetCustomButton(btnCustom10, Properties.Settings.Default.guiCustomBtn10);//, 10);
            SetCustomButton(btnCustom11, Properties.Settings.Default.guiCustomBtn11);//, 11);
            SetCustomButton(btnCustom12, Properties.Settings.Default.guiCustomBtn12);//, 12);

            customButtonUse += SetCustomButton(btnCustom13, Properties.Settings.Default.guiCustomBtn13);//, 13);
            customButtonUse += SetCustomButton(btnCustom14, Properties.Settings.Default.guiCustomBtn14);//, 14);
            customButtonUse += SetCustomButton(btnCustom15, Properties.Settings.Default.guiCustomBtn15);//, 15);
            customButtonUse += SetCustomButton(btnCustom16, Properties.Settings.Default.guiCustomBtn16);//, 16);

            if (customButtonUse == 0)
            {
                tLPCustomButton2.ColumnStyles[0].Width = 33.3f;
                tLPCustomButton2.ColumnStyles[1].Width = 33.3f;
                tLPCustomButton2.ColumnStyles[2].Width = 33.3f;
                tLPCustomButton2.ColumnStyles[3].Width = 0f;
                tLPCustomButton1.ColumnStyles[0].Width = 100f;
                tLPCustomButton1.ColumnStyles[1].Width = 0f;
            }
            else
            {
                tLPCustomButton2.ColumnStyles[0].Width = 25f;
                tLPCustomButton2.ColumnStyles[1].Width = 25f;
                tLPCustomButton2.ColumnStyles[2].Width = 25f;
                tLPCustomButton2.ColumnStyles[3].Width = 25f;
                tLPCustomButton1.ColumnStyles[0].Width = 100f;
                tLPCustomButton1.ColumnStyles[1].Width = 0f;
            }

            string[] tmp = Properties.Settings.Default.guiCustomBtn17.Split('|');
            if (tmp[0].Length > 1)      //Properties.ListSettings.Default.guiCustomBtn17.ToString().Length > 2)
            {
                if (customButtonUse == 0)
                {
                    tLPCustomButton1.ColumnStyles[0].Width = 75f;
                    tLPCustomButton1.ColumnStyles[1].Width = 25f;
                }
                else
                {
                    tLPCustomButton1.ColumnStyles[0].Width = 80f;
                    tLPCustomButton1.ColumnStyles[1].Width = 20f;
                }

                for (int i = 17; i <= 32; i++)
                {
                    if (CustomButtons17.ContainsKey(i))
                    {
                        Button b = CustomButtons17[i];
                        b.Width = btnCustom1.Width - 24;
                        b.Height = btnCustom1.Height;
                        SetCustomButton(b, Properties.Settings.Default["guiCustomBtn" + i.ToString()].ToString());//, i);
                    }
                }
            }

        }

        private void UpdateCustomButtons(bool enable)	//CustomButtonsEnable
        {
            btnCustom1.Enabled = enable;
            btnCustom2.Enabled = enable;
            btnCustom3.Enabled = enable;
            btnCustom4.Enabled = enable;
            btnCustom5.Enabled = enable;
            btnCustom6.Enabled = enable;
            btnCustom7.Enabled = enable;
            btnCustom8.Enabled = enable;
            btnCustom9.Enabled = enable;
            btnCustom10.Enabled = enable;
            btnCustom11.Enabled = enable;
            btnCustom12.Enabled = enable;
            btnCustom13.Enabled = enable;
            btnCustom14.Enabled = enable;
            btnCustom15.Enabled = enable;
            btnCustom16.Enabled = enable;
            for (int i = 17; i <= 32; i++)
            {
                if (CustomButtons17.ContainsKey(i))
                { CustomButtons17[i].Enabled = enable; }
            }
        }

        // store button information for buttons 17 to 32 (1-16 already setup manually)
        private readonly Dictionary<int, Button> CustomButtons17 = new Dictionary<int, Button>();
        private void CustomButtonsSetEvents()
        {
            CustomButtons17.Clear();
            for (int i = 17; i <= 32; i++)
            {
                Button b = new Button
                {
                    Text = "b" + i,
                    Name = "btnCustom" + i.ToString(culture),
                    Width = btnCustom1.Width - 20
                };
                b.MouseDown += BtnCustomButton_Click;
                CustomButtons17.Add(i, b);
                SetCustomButton(b, Properties.Settings.Default["guiCustomBtn" + i.ToString(culture)].ToString());//, i);
                flowLayoutPanel1.Controls.Add(b);
            }
        }

        #endregion


        internal static void SetGcodeVariables()	// applied in MainForm.cs, defined in MainFormObjects.cs
        {
            GuiVariables.variable["GMIX"] = VisuGCode.xyzSize.minx;
            GuiVariables.variable["GMAX"] = VisuGCode.xyzSize.maxx;
            GuiVariables.variable["GCTX"] = (VisuGCode.xyzSize.minx + VisuGCode.xyzSize.maxx) / 2;
            GuiVariables.variable["GMIY"] = VisuGCode.xyzSize.miny;
            GuiVariables.variable["GMAY"] = VisuGCode.xyzSize.maxy;
            GuiVariables.variable["GCTY"] = (VisuGCode.xyzSize.miny + VisuGCode.xyzSize.maxy) / 2;
            GuiVariables.variable["GMIZ"] = VisuGCode.xyzSize.minz;
            GuiVariables.variable["GMAZ"] = VisuGCode.xyzSize.maxz;
            GuiVariables.variable["GCTZ"] = (VisuGCode.xyzSize.minz + VisuGCode.xyzSize.maxz) / 2;
            GuiVariables.variable["GMIS"] = (double)Properties.Settings.Default.importGCPWMDown;
            GuiVariables.variable["GMAS"] = (double)Properties.Settings.Default.importGCPWMUp;
            GuiVariables.variable["GZES"] = (double)Properties.Settings.Default.importGCPWMZero;
            GuiVariables.variable["GCTS"] = (double)(Properties.Settings.Default.importGCPWMDown + Properties.Settings.Default.importGCPWMUp) / 2;
            GuiVariables.WriteDimensionToRegistry();
        }
    }
}
