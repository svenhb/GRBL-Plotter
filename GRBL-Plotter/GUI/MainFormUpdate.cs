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
/* MainFormUpdate
 * Update controls etc.
 * 2021-07-26 new
 * 2021-09-19 line 389 change order of virtualJoystickXY.Enabled
*/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

//#pragma warning disable CA1303
//#pragma warning disable CA1304
//#pragma warning disable CA1305
//#pragma warning disable CA1307

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
            if (Gcode.LoggerTrace)
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
            const string reg_key = "\\SOFTWARE\\GRBL-Plotter";
            string newPath1="", newPath2 = "";
            try
            {   newPath1 = (string)Registry.GetValue("HKEY_LOCAL_MACHINE" + reg_key, "DataPath", 0); }   // if installed via admin, regkey is in HKLM

            catch { }
            try
            {   newPath2 = (string)Registry.GetValue("HKEY_CURRENT_USER" + reg_key, "DataPath", 0); }  // if installed via user, regkey is in HKCU
            catch { }
            
            if (!string.IsNullOrEmpty(newPath1))
            {   // regkey is set during regular setup
                Datapath.AppDataFolder = newPath1;                   // get path from registry
                NLog.LogManager.Configuration.Variables["basedir"] = newPath1;
                Logger.Info("GetAppDataPath from HKEY_LOCAL_MACHINE: {0}", newPath1);
            }
            else if (!string.IsNullOrEmpty(newPath2))
            {   // regkey is set during regular setup
                Datapath.AppDataFolder = newPath2;                   // get path from registry
                NLog.LogManager.Configuration.Variables["basedir"] = newPath2;
                Logger.Info("GetAppDataPath from HKEY_CURRENT_USER: {0}", newPath2);
            }
            else // no setup?
            {
                newPath1 = Datapath.AppDataFolder = Datapath.Application;      // use application path
                NLog.LogManager.Configuration.Variables["basedir"] = newPath1;
                Logger.Info("GetAppDataPath Application.StartupPath: {0}", newPath1);
            }
        }

        private void SetGUISize()
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            Location = Properties.Settings.Default.locationMForm;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { this.CenterToScreen(); }
            Size = Properties.Settings.Default.mainFormSize;

            WindowState = Properties.Settings.Default.mainFormWinState;

            int splitDist = Properties.Settings.Default.mainFormSplitDistance;
            if ((splitDist > splitContainer1.Panel1MinSize) && (splitDist < (splitContainer1.Width - splitContainer1.Panel2MinSize)))
                splitContainer1.SplitterDistance = splitDist;

            this.Text = string.Format("{0} Ver.:{1}", appName, System.Windows.Forms.Application.ProductVersion.ToString(culture));
//            this.Text = string.Format("{0} Ver. {1}  Date {2}", appName, System.Windows.Forms.Application.ProductVersion.ToString(culture), File.GetCreationTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("yyyy-mm-dd hh:mm:ss"));
            toolTip1.SetToolTip(this, this.Text);
			
            SplitContainer1_SplitterMoved(null, null);
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

            foldCodeBlocks1stLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D2;
            foldCodeBlocks2ndLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D3;
            foldCodeBlocks3rdLevelToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.D4;
            expandCodeBlocksToolStripMenuItem1.ShortcutKeys = Keys.Alt | Keys.D1;

            cmsPicBoxMoveToMarkedPosition.ShortcutKeys = Keys.Control | Keys.M;
            cmsPicBoxPasteFromClipboard.ShortcutKeys = Keys.Control | Keys.V;

            toolStrip_tBRadiusCompValue.Text = string.Format(culture, "{0:0.000}", Properties.Settings.Default.crcValue);

        }

        // load settings
        public void LoadSettings(object sender, EventArgs e)
        {
            Logger.Info("LoadSettings");

            if ((_setup_form != null) && (_setup_form.settingsReloaded))
            {
                _setup_form.settingsReloaded = false;
                SetGUISize();							// if reset ALL settings was pressed (incl. form locations)
            }
            pictureBox1.Invalidate();

            UpdateWholeApplication();
        }

        private void UpdateWholeApplication()	// after ini file, setup change, update controls
        {	// Update everything which could be changed via Setup or INI-file
            Logger.Info("UpdateWholeApplication");
			UpdateLogging();

            fCTBCode.BookmarkColor = Properties.Settings.Default.gui2DColorMarker; ;
            fCTBCode.LineInterval = (int)Properties.Settings.Default.FCTBLineInterval;
            tbFile.Text = Properties.Settings.Default.guiLastFileLoaded;
			
			CustomButtonsFillWithContent();		// fill and resize custom buttons			
            GraphicPropertiesSetup();			// pen / brush colors
            MenuTranslateSetup();				// set rotary info in menu item 'translate'

            GuiEnableAxisABC();					// enable and resize axis ABC GUI buttons (set 0)
            virtualJoystickA.Visible |= ctrl4thAxis || Grbl.axisA;
            virtualJoystickA.JoystickText = ctrl4thName;
            JoystickSetup();					// Joystick step and speed
            JoystickResize();					// relative size

// 2-D view settings
            toolStripViewMachine.Checked = Properties.Settings.Default.machineLimitsShow;
            toolStripViewTool.Checked = Properties.Settings.Default.gui2DToolTableShow;
            toolStripViewMachineFix.Checked = Properties.Settings.Default.machineLimitsFix;

            if (Properties.Settings.Default.importGCSDirM3)
                cBSpindle.Text = "Spindle CW";
            else
                cBSpindle.Text = "Spindle CCW";

// grbl communication
            int[] interval = new int[] { 500, 250, 200, 125, 100 };
            int index = Properties.Settings.Default.grblPollIntervalIndex;
            if ((index >= 0) && (index < 5))
                Grbl.pollInterval = interval[index];

            gamePadTimer.Enabled = Properties.Settings.Default.gamePadEnable;
            CheckMachineLimit();
            LoadHotkeys();

// changed PWM settings			
            toolTip1.SetToolTip(btnPenUp,   string.Format("send 'M3 S{0}'", Properties.Settings.Default.importGCPWMUp));
            toolTip1.SetToolTip(btnPenDown, string.Format("send 'M3 S{0}'", Properties.Settings.Default.importGCPWMDown));
            GuiVariables.variable["GMIS"] = (double)Properties.Settings.Default.importGCPWMDown;
            GuiVariables.variable["GMAS"] = (double)Properties.Settings.Default.importGCPWMUp;
            GuiVariables.variable["GZES"] = (double)Properties.Settings.Default.importGCPWMZero;
            GuiVariables.variable["GCTS"] = (double)(Properties.Settings.Default.importGCPWMDown + Properties.Settings.Default.importGCPWMUp) / 2;
            GuiVariables.WriteSettingsToRegistry();	// for use within external scripts
        }   // end load settings

        private void GuiEnableAxisABC()
        { 
            ctrl4thAxis = Properties.Settings.Default.ctrl4thUse;
            ctrl4thName = Properties.Settings.Default.ctrl4thName;
            label_a.Visible = ctrl4thAxis || Grbl.axisA || simulateA;
            label_a.Text = ctrl4thName;
            label_wa.Visible = ctrl4thAxis || Grbl.axisA || simulateA;
            label_ma.Visible = ctrl4thAxis || Grbl.axisA || simulateA;
            btnZeroA.Visible = ctrl4thAxis || Grbl.axisA;
            mirrorRotaryToolStripMenuItem.Visible = ctrl4thAxis;
            btnZeroA.Text = "Zero " + ctrl4thName;
            if (Properties.Settings.Default.guiLanguage == "de-DE")
                btnZeroA.Text = ctrl4thName + " nullen";

            btnJogZeroA.Visible = ctrl4thAxis || Grbl.axisA;
            btnJogZeroA.Text = ctrl4thName + "=0";

            if (Grbl.axisB || Grbl.axisC)
            {
                label_a.Location = new Point(230, 14);      // move A controls to upper right
                label_wa.Location = new Point(251, 14);
                label_ma.Location = new Point(263, 32);
                btnZeroA.Location = new Point(335, 14);
                label_status0.Location = new Point(1, 118); // keep home and status
                label_status.Location = new Point(1, 138);
                btnHome.Location = new Point(106, 111);
                btnHome.Size = new Size(122, 57);
                groupBoxCoordinates.Width = 394;            // extend width
                tLPRechtsOben.ColumnStyles[0].Width = 400;

                label_c.Visible = Grbl.axisC;
                label_wc.Visible = Grbl.axisC;
                label_mc.Visible = Grbl.axisC;
                btnZeroC.Visible = Grbl.axisC;
            }
            else
            {
                label_a.Location = new Point(1, 110);      // move A controls to lower left
                label_wa.Location = new Point(22, 110);
                label_ma.Location = new Point(34, 128);
                btnZeroA.Location = new Point(106, 110);
                groupBoxCoordinates.Width = 230;
                tLPRechtsOben.ColumnStyles[0].Width = 236;

                if (ctrl4thAxis || Grbl.axisA || simulateA)
                {
                    label_status0.Location = new Point(1, 128);
                    label_status.Location = new Point(1, 148);
                    btnHome.Location = new Point(106, 138);
                    btnHome.Size = new Size(122, 30);
                }
                else
                {
                    label_status0.Location = new Point(1, 118);
                    label_status.Location = new Point(1, 138);
                    btnHome.Location = new Point(106, 111);
                    btnHome.Size = new Size(122, 57);
                }
            }
        }
        private void MenuTranslateSetup()
        {   skaliereXAufDrehachseToolStripMenuItem.Enabled = false;
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
            virtualJoystickXY.JoystickLabel = joystickXYStep;
            virtualJoystickZ.JoystickLabel = joystickZStep;
            virtualJoystickA.JoystickLabel = joystickAStep;
            virtualJoystickB.JoystickLabel = joystickAStep;
            virtualJoystickC.JoystickLabel = joystickAStep;
        }

        // update controls on Main form (disable if streaming or no serial)
        private void UpdateControlEnables()
        { UpdateControlEnables(false); }
        private void UpdateControlEnables(bool allowControl)
        {
            bool isConnected = false;
            if (_serial_form != null)
            { isConnected = _serial_form.SerialPortOpen || Grbl.grblSimulate; }

            UpdateCustomButtons(true);  // isConnected && (!isStreaming || allowControl)

            allowControl = isStreamingPause;
            Logger.Trace("updateControls isConnected:{0} isStreaming:{1} streamingAllowControl:{2} source:{3}", isConnected, isStreaming, allowControl, timerUpdateControlSource);
            timerUpdateControlSource = "";

            virtualJoystickC.Enabled = isConnected && (!isStreaming || allowControl);
            virtualJoystickB.Enabled = isConnected && (!isStreaming || allowControl);
            virtualJoystickA.Enabled = isConnected && (!isStreaming || allowControl);
            virtualJoystickZ.Enabled = isConnected && (!isStreaming || allowControl);
            virtualJoystickXY.Enabled = isConnected && (!isStreaming || allowControl);
            btnHome.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroX.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroY.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroZ.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroA.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroB.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroC.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroXY.Enabled = isConnected & !isStreaming | allowControl;
            btnZeroXYZ.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroX.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroY.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroZ.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroA.Enabled = isConnected & !isStreaming | allowControl;
            btnJogZeroXY.Enabled = isConnected & !isStreaming | allowControl;
            btnOffsetApply.Enabled = !isStreaming;
            gCodeToolStripMenuItem.Enabled = !isStreaming;

            cBSpindle.Enabled = isConnected & !isStreaming | allowControl;
            btnPenUp.Enabled = isConnected & !isStreaming | allowControl;
            btnPenDown.Enabled = isConnected & !isStreaming | allowControl;

            tBSpeed.Enabled = isConnected & !isStreaming | allowControl;
            lblSpeed.Enabled = isConnected & !isStreaming | allowControl;
            gB_Jog0.Enabled = isConnected & !isStreaming | allowControl;
            cBCoolant.Enabled = isConnected & !isStreaming | allowControl;
            cBTool.Enabled = isConnected & !isStreaming | allowControl;
            btnReset.Enabled = isConnected;
            btnFeedHold.Enabled = isConnected;
            btnResume.Enabled = isConnected;
            btnKillAlarm.Enabled = isConnected;
            btnStreamStart.Enabled = isConnected;// & isFileLoaded;
            btnStreamStop.Enabled = isConnected; // & isFileLoaded;
            btnStreamCheck.Enabled = isConnected;// & isFileLoaded;

            if (!Grbl.isVersion_0)
            {
                btnJogStop.Visible = true;
                gBoxOverride.Enabled = isConnected;
                tableLayoutPanel4.RowStyles[0].Height = 30f;
                tableLayoutPanel4.RowStyles[1].Height = 30f;
                tableLayoutPanel4.RowStyles[2].Height = 40f;
            }
            else
            {
                btnJogStop.Visible = false;
                gBoxOverride.Enabled = false;
                tableLayoutPanel4.RowStyles[0].Height = 40f;
                tableLayoutPanel4.RowStyles[1].Height = 0f;
                tableLayoutPanel4.RowStyles[2].Height = 60f;
            }
            EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
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
            if (tmp[0].Length > 1)      //Properties.Settings.Default.guiCustomBtn17.ToString().Length > 2)
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
        }
    }
}
