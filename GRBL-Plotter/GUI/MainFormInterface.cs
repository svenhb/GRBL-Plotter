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
 * 2020-09-18 split file
 * 2020-12-28 add Marlin support (replace commands)
 * 2021-07-02 code clean up / code quality
 * 2021-09-29 update Grbl.Status line 60
 * 2021-09-30 no VisuGCode.ProcessedPath.ProcessedPathDraw if VisuGCode.largeDataAmount
 * 2021-11-18 add processing of accessory D0-D3 from grbl-Mega-5X - line 139
 * 2022-02-24
 * 2023-03-09 simplify NULL check; case GrblState.unknown: UpdateControlEnables();
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MainForm : Form
    {

        private string oldRaw = "";
        private GrblState lastMachineStatus = GrblState.unknown;
        private string lastInfoText = "";
        private string lastLabelInfoText = "";
        private bool updateDrawingPath = false;
        private GrblState machineStatus;

        private string actualFR = "";
        private string actualSS = "";

        private bool timerUpdateControls = false;
        private string timerUpdateControlSource = "";

        /************************************************************
         * handle status report and position event from serial form
         * processStatus()
         ************************************************************/
        private void OnRaisePosEvent(object sender, PosEventArgs e)
        {
            //  if (logPosEvent)  Logger.Trace("OnRaisePosEvent  {0}  connect {1}  status {2}", e.Status.ToString(), _serial_form.serialPortOpen, e.Status.ToString());
            Grbl.Status = machineStatus = e.Status;

            /***** Restore saved position after reset and set initial feed rate: *****/
            if (ResetDetected || (e.Status == GrblState.reset))
            { ProcessReset(); }

            /***** process grblState {idle, run, hold, home, alarm, check, door} *****/
            ProcessStatus(e.Raw);

            /***** check and submit override values, set labels, checkbox *****/
            ProcessStatusMessage(e.StatMsg);

            /***** set DRO digital-read-out labels with machine and work coordinates *****/
            if (!simuEnabled)
                UpdateDRO();

            /***** parser state Spinde/Coolant on/off, on other Forms: FeedRate, SpindleSpeed, G54-Coord *****/
            ProcessParserState(e.ParserState);

            /***** update 2D view *****/
            if (Grbl.posChanged)
            {
                VisuGCode.CreateMarkerPath();
                VisuGCode.UpdatePathPositions();
                CheckMachineLimit();
                pictureBox1.Invalidate();
                if (Properties.Settings.Default.flowCheckRegistryChange && !isStreaming)
                    GuiVariables.WritePositionToRegistry();
                Grbl.posChanged = false;
            }
            if (Grbl.wcoChanged)
            {
                CheckMachineLimit();
                Grbl.wcoChanged = false;
            }
            //         if (((isStreaming || isStreamingRequestStop)) && Properties.Settings.Default.guiProgressShow && !VisuGCode.largeDataAmount)
            if (isStreaming && Properties.Settings.Default.guiProgressShow && !VisuGCode.largeDataAmount)
                VisuGCode.ProcessedPath.ProcessedPathDraw(Grbl.posWork);


            if (_diyControlPad != null)
            {
                if (oldRaw != e.Raw)
                {
                    _diyControlPad.SendFeedback(e.Raw);     //hand over original grbl text
                    oldRaw = e.Raw;
                }
            }
        }

        private void ProcessStatusMessage(ModState StatMsg)
        {
            if (logPosEvent) Logger.Trace("processStatusMessage  ");
            if (StatMsg.Ov.Length > 1)
            { ProcessOverrideValues(StatMsg.Ov); }
            if (StatMsg.FS.Length > 1)
            { ProcessOverrideCurrentFeedSpeed(StatMsg.FS); }

            /***** if no Accessory State character is given, no accessory is set and D is 0000 *****/
            if (StatMsg.A.Contains("S"))
            {
                btnOverrideSpindle.Image = Properties.Resources.led_on;   // Spindle on CW
                                                                          //        btnOverrideSpindle.Text = "Spindle CW";
                RbSpindleCW.Checked = true;
                //    CbSpindle.BackColor = Color.Lime;
            }
            if (StatMsg.A.Contains("C"))
            {
                btnOverrideSpindle.Image = Properties.Resources.led_on;   // Spindle on CCW
                                                                          //       btnOverrideSpindle.Text = "Spindle CCW";
                RbSpindleCCW.Checked = true;
                //    CbSpindle.BackColor = Color.Lime;
            }
            if (!StatMsg.A.Contains("S") && !StatMsg.A.Contains("C"))
            {
                btnOverrideSpindle.Image = Properties.Resources.led_off;
                //   CbSpindle.BackColor = Color.Fuchsia;
                //CbSpindle.Checked = false; 
            }  // Spindle off

            if (StatMsg.A.Contains("F"))
            {
                btnOverrideFlood.Image = Properties.Resources.led_on;
                CbCoolant.BackColor = Color.Lime;
            }   // Flood on
            else
            {
                btnOverrideFlood.Image = Properties.Resources.led_off;
                CbCoolant.BackColor = Color.Transparent;    // Color.Fuchsia;
            }

            if (StatMsg.A.Contains("M"))
            {
                btnOverrideMist.Image = Properties.Resources.led_on;
                CbMist.BackColor = Color.Lime;
            } // Mist on
            else
            {
                btnOverrideMist.Image = Properties.Resources.led_off;
                CbMist.BackColor = Color.Transparent;    // Color.Fuchsia;
            }

            if (Properties.Settings.Default.grblDescriptionDxEnable)
            {
                if (StatMsg.A.Contains("D"))
                {
                    string digits = StatMsg.A.Substring(StatMsg.A.IndexOf("D") + 1, 4);     // Digital pins in order '3210'
                    if (digits.Length == 4)
                    {
                        SetAccessoryButton(BtnOverrideD3, (digits[0] == '1'));
                        SetAccessoryButton(BtnOverrideD2, (digits[1] == '1'));
                        SetAccessoryButton(BtnOverrideD1, (digits[2] == '1'));
                        SetAccessoryButton(BtnOverrideD0, (digits[3] == '1'));
                    }
                }
                else
                {
                    SetAccessoryButton(BtnOverrideD3, false);
                    SetAccessoryButton(BtnOverrideD2, false);
                    SetAccessoryButton(BtnOverrideD1, false);
                    SetAccessoryButton(BtnOverrideD0, false);
                }
            }
        }
        private void SetAccessoryButton(Button Btn, bool setOn)
        {
            if (setOn)
            { Btn.Image = Properties.Resources.led_on; Btn.Tag = "on"; }
            else
            { Btn.Image = Properties.Resources.led_off; ; Btn.Tag = "off"; }
        }
        private void ProcessOverrideValues(string txt)
        {
            _streaming_form2?.ShowOverrideValues(txt);

            string[] value = txt.Split(',');
            if (value.Length > 2)
            {
                SetTextThreadSave(lblOverrideFRValue, value[0]);
                SetTextThreadSave(lblOverrideRapidValue, value[1]);
                SetTextThreadSave(lblOverrideSSValue, value[2]);
            }
        }

        private void ProcessOverrideCurrentFeedSpeed(string txt)
        {
            _streaming_form2?.ShowActualValues(txt);

            string[] value = txt.Split(',');
            if (value.Length > 1)
            {
                SetTextThreadSave(lblStatusFeed, value[0]);  // + " mm/min";
                SetTextThreadSave(lblStatusSpeed, value[1]); // + " RPM";
                SetTextThreadSave(LblSpeedSetVal, value[1], Color.Lime);
                SetTextThreadSave(LblLaserSetVal, value[1], Color.Lime);
            }
            else if (value.Length > 0)
            {
                SetTextThreadSave(lblStatusFeed, value[0]);  // + " mm/min";
                SetTextThreadSave(lblStatusSpeed, "-");      // + " RPM";
            }
        }


        private void UpdateDRO()
        {
            if (label_mx.InvokeRequired)
            { label_mx.BeginInvoke((MethodInvoker)delegate () { UpdateDROText(); }); }
            else
            { UpdateDROText(); }
        }
        private void UpdateDROText()
        {
            label_mx.Text = string.Format("{0:0.000}", Grbl.posMachine.X);
            label_my.Text = string.Format("{0:0.000}", Grbl.posMachine.Y);
            label_mz.Text = string.Format("{0:0.000}", Grbl.posMachine.Z);
            label_wx.Text = string.Format("{0:0.000}", Grbl.posWork.X);
            label_wy.Text = string.Format("{0:0.000}", Grbl.posWork.Y);
            label_wz.Text = string.Format("{0:0.000}", Grbl.posWork.Z);
            if (Grbl.axisA)
            {
                label_ma.Text = string.Format("{0:0.000}", Grbl.posMachine.A);
                label_wa.Text = string.Format("{0:0.000}", Grbl.posWork.A);
            }
            if (Grbl.axisB)
            {
                label_mb.Text = string.Format("{0:0.000}", Grbl.posMachine.B);
                label_wb.Text = string.Format("{0:0.000}", Grbl.posWork.B);
            }
            if (Grbl.axisC)
            {
                label_mc.Text = string.Format("{0:0.000}", Grbl.posMachine.C);
                label_wc.Text = string.Format("{0:0.000}", Grbl.posWork.C);
            }
        }

        /***************************************************************
         * handle status events from serial form
         * {idle, run, hold, home, alarm, check, door}
         * only action on status change
         ***************************************************************/
        private void ProcessStatus(string msg)
        {
            string lblInfoText = "";	// rename
            Color lblInfoColor = Color.Black;
            if (logPosEvent)
                Logger.Trace("processStatus  Status:{0}", machineStatus.ToString());
            if ((machineStatus != lastMachineStatus) || (Grbl.lastMessage.Length > 5))
            {
                // label at DRO
                if (label_status.InvokeRequired)
                { label_status.BeginInvoke((MethodInvoker)delegate () { label_status.Text = Grbl.StatusToText(machineStatus); }); }
                else
                { label_status.Text = Grbl.StatusToText(machineStatus); }
                label_status.BackColor = Grbl.GrblStateColor(machineStatus);

                switch (machineStatus)
                {
                    case GrblState.idle:
                        if ((lastMachineStatus == GrblState.hold) || (lastMachineStatus == GrblState.alarm))
                        {
                            StatusStripClear(1, 2);//, "grblState.idle");
                            SetTextThreadSave(lbInfo, lastInfoText, SystemColors.Control);

                            if (!_serial_form.CheckGRBLSettingsOk())   // check 30 kHz limit
                            {
                                StatusStripSet(1, Grbl.lastMessage, Color.Fuchsia);
                                StatusStripSet(2, Localization.GetString("statusStripeCheckCOM"), Color.Yellow);
                            }
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        CbTool.Checked = _serial_form.ToolInSpindle;
                        if (signalLock > 0)
                        { btnKillAlarm.BackColor = SystemColors.Control; signalLock = 0; }
                        if (!isStreaming)                       // update drawing if G91 is used
                            updateDrawingPath = true;
                        //       StatusStripClear(1, 2);//, "grblState.idle2");
                        Grbl.lastMessage = "";
                        break;

                    case GrblState.run:
                        if (lastMachineStatus == GrblState.hold)
                        {
                            //           statusStripClear();
                            SetTextThreadSave(lbInfo, lastInfoText, SystemColors.Control);
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        break;

                    case GrblState.hold:
                        btnResume.BackColor = Color.Yellow;
                        lastInfoText = lbInfo.Text;
                        lblInfoText = Localization.GetString("mainInfoResume");     //"Press 'Resume' to proceed";

                        if (Grbl.lastErrorNr > 0)
                            lblInfoColor = Color.Fuchsia;
                        else
                            lblInfoColor = Color.Yellow;

                        SetTextThreadSave(lbInfo, lblInfoText, lblInfoColor);
                        //       statusStripClear();
                        StatusStripSet(1, Grbl.StatusToText(machineStatus), Grbl.GrblStateColor(machineStatus));
                        StatusStripSet(2, lblInfoText, lblInfoColor);
                        if (signalResume == 0) { signalResume = 1; }
                        break;

                    case GrblState.home:
                        break;

                    case GrblState.alarm:
                        signalLock = 1;
                        btnKillAlarm.BackColor = Color.Yellow;
                        lblInfoText = Localization.GetString("mainInfoKill");     //"Press 'Kill Alarm' to proceed";
                        SetTextThreadSave(lbInfo, lblInfoText, Color.Yellow);

                        StatusStripSet(1, Grbl.StatusToText(machineStatus) + " " + Grbl.lastMessage, Grbl.GrblStateColor(machineStatus));
                        StatusStripSet(2, lblInfoText, Color.Yellow);
                        Grbl.lastMessage = "";
                        _heightmap_form?.StopScan();
                        break;

                    case GrblState.check:
                        break;

                    case GrblState.door:
                        btnResume.BackColor = Color.Yellow;
                        lastInfoText = lbInfo.Text;
                        lblInfoText = Localization.GetString("mainInfoResume");     //"Press 'Resume' to proceed";
                        SetTextThreadSave(lbInfo, lblInfoText, Color.Yellow);

                        StatusStripSet(1, Grbl.StatusToText(machineStatus), Grbl.GrblStateColor(machineStatus));
                        StatusStripSet(2, lblInfoText, Color.Yellow);
                        if (signalResume == 0) { signalResume = 1; }
                        break;

                    case GrblState.probe:
                        posProbe = _serial_form.posProbe;
                        if (_diyControlPad != null)
                        {
                            if (alternateZ != null)
                                posProbe.Z = (double)alternateZ;
                            //_diyControlPad.sendFeedback("Probe: "+posProbe.Z.ToString());
                        }
                        if (_heightmap_form != null)
                            _heightmap_form.SetPosProbe = posProbe;

                        if (_probing_form != null)
                        {
                            Logger.Info("Update Probing {0}", Grbl.DisplayCoord("PRB"));
                            _probing_form.SetPosProbe = Grbl.GetCoord("PRB");
                        }

                        lastInfoText = lbInfo.Text;
                        SetTextThreadSave(lbInfo, string.Format("{0}: X:{1:0.00} Y:{2:0.00} Z:{3:0.00}", Localization.GetString("mainInfoProbing"), posProbe.X, posProbe.Y, posProbe.Z), Color.Yellow);
                        break;

                    case GrblState.unknown:
                        //      timerUpdateControlSource = "grblState.unknown";
                        UpdateControlEnables();
                        break;

                    case GrblState.notConnected:
                        //SetInfoLabel("No connection", Color.Fuchsia);
                        SetTextThreadSave(lbInfo, "No connection - press 'RESET'", Color.Fuchsia);
                        StatusStripSet(1, "No connection - press 'RESET'", Color.Fuchsia);
                        StatusStripSet(2, msg, Color.Fuchsia);
                        isStreaming = false;
                        UpdateControlEnables();
                        break;

                    default:
                        break;
                }
                if (_probing_form != null)
                { _probing_form.SetGrblSaState = machineStatus; }

            }
            lastMachineStatus = machineStatus;
        }

        /********************************************************
         * handle last sent commands from serial form
         * FeedRate, SpindleSpeed, Spinde/Coolant on/off, G54-Coord
         * update other forms
         ********************************************************/
        private void ProcessParserState(ParsState cmd)
        {
            if (logPosEvent) Logger.Trace("processParserState FR:{0}  SS:{1}  spindle:{2}  coolant:{3}  Tool:{4}  Coord:{5}", cmd.FR.ToString(), cmd.SS.ToString(), cmd.spindle, cmd.coolant, cmd.tool.ToString(), cmd.coord_select.ToString());
            if (cmd.changed)
            {
                actualFR = cmd.FR.ToString();
                _streaming_form?.ShowValueFR(actualFR);
                actualSS = cmd.SS.ToString();
                _streaming_form?.ShowValueSS(actualSS);

                if (Grbl.isVersion_0)
                {
                    /*    CbSpindle.CheckedChanged -= CbSpindle_CheckedChanged;
                        CbSpindle.Checked = (cmd.spindle <= 4);// ? true : false;  // M3, M4 start, M5 stop
                        CbSpindle.CheckedChanged += CbSpindle_CheckedChanged;
                        CbCoolant.CheckedChanged -= CbCoolant_CheckedChanged;
                        CbCoolant.Checked = (cmd.coolant <= 8);// ? true : false;  // M7, M8 on   M9 coolant off
                        CbCoolant.CheckedChanged += CbCoolant_CheckedChanged;*/
                    if (cmd.spindle <= 4) CbSpindle.BackColor = Color.Lime;
                    else CbSpindle.BackColor = Color.Transparent;
                    if (cmd.spindle <= 8) CbCoolant.BackColor = Color.Lime;
                    else CbCoolant.BackColor = Color.Transparent;

                }
                if (cmd.toolchange)
                    lblTool.Text = cmd.tool.ToString();

                lblCurrentG.Text = "G" + cmd.coord_select.ToString();
                lblCurrentG.BackColor = (cmd.coord_select == 54) ? Color.Lime : Color.Fuchsia;
                if (_camera_form != null)
                    _camera_form.SetCoordG = cmd.coord_select;
                if (_coordSystem_form != null)
                {
                    _coordSystem_form.MarkActiveCoordSystem(lblCurrentG.Text);
                    _coordSystem_form.UpdateTLO(cmd.TLOactive, cmd.tool_length);
                }
            }
        }

        private void ProcessReset()
        {
            timerUpdateControlSource = "processReset";

            ShowGrblLastMessage();      // from Grbl.lastMessage  in MainForm.cs

            if (isStreaming)
            { ResetStreaming(); }

            Logger.Info("### MainForm ProcessReset");
            if (!_serial_form.CheckGRBLSettingsOk())   // check 30 kHz limit
            {
                StatusStripSet(1, Grbl.lastMessage, Color.Orange);
                StatusStripSet(2, Localization.GetString("statusStripeCheckCOM"), Color.Yellow);
            }

            if (Properties.Settings.Default.resetRestoreWorkCoordinates)
            {
                double x = Properties.Settings.Default.grblLastOffsetX;
                double y = Properties.Settings.Default.grblLastOffsetY;
                double z = Properties.Settings.Default.grblLastOffsetZ;
                double a = Properties.Settings.Default.grblLastOffsetA;
                double b = Properties.Settings.Default.grblLastOffsetB;
                double c = Properties.Settings.Default.grblLastOffsetC;

                Logger.Info("restoreWorkCoordinates [Setup - Program behavior - Flow control - Behavior after grbl reset]");
                coordinateG = Properties.Settings.Default.grblLastOffsetCoord;
                _serial_form.AddToLog("* Restore last selected coordinate system:");
                SendCommand(String.Format("G{0}", coordinateG));

                _serial_form.AddToLog("* Restore saved position after reset\r\n* and set initial feed rate:");
                string setAxis = String.Format("{0} X{1} Y{2} Z{3} ", (Grbl.isMarlin ? "G92 " : zeroCmd), x, y, z);

                if (Grbl.axisA)
                {
                    if (ctrl4thAxis)
                        setAxis += String.Format("{0}{1} ", ctrl4thName, a);
                    else
                        setAxis += String.Format("A{0} ", a);
                }
                if (Grbl.axisB)
                    setAxis += String.Format("B{0} ", b);
                if (Grbl.axisC)
                    setAxis += String.Format("C{0} ", c);

                setAxis += String.Format("F{0}", Properties.Settings.Default.importGCXYFeed);

                SendCommands(setAxis.Replace(',', '.'));
            }

            if (Properties.Settings.Default.resetSendCodeEnable)
            {
                _serial_form.AddToLog("* Code after reset: " + Properties.Settings.Default.resetSendCode + " in [Setup - Program behavior - Flow control]");
                ProcessCommands(Properties.Settings.Default.resetSendCode);
            }

            ResetDetected = false;
            timerUpdateControls = true;
            SetGRBLBuffer();
            Logger.Trace("ResetEvent()  connect:{0}  msg:{1}", _serial_form.SerialPortOpen, Grbl.lastMessage);
            EventCollector.SetStreaming("Srst");
            UpdateControlEnables();
            ResetStreaming(false);
        }

        private void SetGRBLBuffer()
        {
            if (!Grbl.axisUpdate && (Grbl.axisCount > 0))
            {
                Grbl.axisUpdate = true;
                if (Properties.Settings.Default.grblBufferAutomatic)
                {
                    if (Grbl.bufferSize > 0)
                    {
                        Grbl.RX_BUFFER_SIZE = (Grbl.bufferSize != 128) ? Grbl.bufferSize : 127;
                        if (!Grbl.isMarlin) _serial_form.AddToLog("* Read buffer size of " + Grbl.RX_BUFFER_SIZE + " bytes");
                        Logger.Info("Read buffer size of {0} [Setup - Flow control - grbl buffer size]   Grbl.axisCount:{1}", Grbl.RX_BUFFER_SIZE, Grbl.axisCount);
                    }
                    else
                    {
                        if (Grbl.axisCount > 3)
                            Grbl.RX_BUFFER_SIZE = 255;
                        else
                            Grbl.RX_BUFFER_SIZE = 127;

                        if (!Grbl.isMarlin) _serial_form.AddToLog("* Assume buffer size of " + Grbl.RX_BUFFER_SIZE + " bytes");
                        Logger.Info("Assume buffer size of {0} [Setup - Flow control - grbl buffer size]   Grbl.axisCount:{1}", Grbl.RX_BUFFER_SIZE, Grbl.axisCount);
                    }
                }
                else
                {  //if (grbl.RX_BUFFER_SIZE != 127)
                    if (!Grbl.isMarlin) _serial_form.AddToLog("* Buffer size was manually set to " + Grbl.RX_BUFFER_SIZE + " bytes!\r* Check [Setup - Flow control]");
                    Logger.Info("Buffer size was set manually to {0} [Setup - Flow control - grbl buffer size]  Grbl.axisCount:{1}", Grbl.RX_BUFFER_SIZE, Grbl.axisCount);
                }
                if (!Grbl.isMarlin) StatusStripSet(2, string.Format("grbl-controller connected: vers: {0}, axis: {1}, buffer: {2}", _serial_form.GrblVers, Grbl.axisCount, Grbl.RX_BUFFER_SIZE), Color.Lime);
                else StatusStripSet(2, string.Format("Marlin connected: axis: {0}", Grbl.axisCount), Color.Lime);

                delayedStatusStripClear1 = 8;
            }
        }


        private void SetTextThreadSave(Label label, string txt, Color bcolor)
        {
            SetTextThreadSave(label, txt);
            label.BackColor = bcolor;
        }
        private void SetTextThreadSave(Label label, string txt)
        {
            if (label.InvokeRequired)
            { label.BeginInvoke((MethodInvoker)delegate () { label.Text = txt; }); }
            else
            { label.Text = txt; }
        }

        private void UpdateGrblSettings(int id)
        {
            switch (id)
            {
                case 30:
                    SetTextThreadSave(LblSpeedMaxVal, Grbl.GetSetting(id).ToString(), Color.Lime);
                    SetTextThreadSave(LblLaserMaxVal, Grbl.GetSetting(id).ToString(), Color.Lime);
                    break;
                case 31:
                    SetTextThreadSave(LblSpeedMinVal, Grbl.GetSetting(id).ToString(), Color.Lime);
                    SetTextThreadSave(LblLaserMinVal, Grbl.GetSetting(id).ToString(), Color.Lime);
                    break;
                case 32:
                    if (Grbl.GetSetting(id) == 1)
                        SetTextThreadSave(CbLasermodeVal, "ON", Color.Lime);
                    else
                        SetTextThreadSave(CbLasermodeVal, "OFF", Color.Lime);
                    break;

                default:
                    break;
            }
        }
    }
}