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
/* 
 * 2020-09-18 split file
 * 2020-12-28 add Marlin support (replace commands)
 * 2021-07-02 code clean up / code quality
 * 2021-09-29 update Grbl.Status line 60
 * 2021-09-30 no VisuGCode.ProcessedPath.ProcessedPathDraw if VisuGCode.largeDataAmount
 * 2021-11-18 add processing of accessory D0-D3 from grbl-Mega-5X - line 139
 * 2022-02-24
 * 2023-03-09 simplify NULL check; case GrblState.unknown: UpdateControlEnables();
 * 2024-02-14 l:160 f:ProcessStatusMessage add grblDigialIn -Out
 * 2024-02-24 l:61 f:OnRaisePosEvent submit Grbl.StatMsg
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.Helper;
using GrblPlotter.UserControls;
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
            Grbl.StatMsg = e.StatMsg;

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

            if (StatMsg.A.Contains("S"))
            {
                ucOverrides.SetStateSpindle(true);
                ucDeviceRouter2.SetStatusSpindle(true);
                ucDeviceLaser2.SetStatusSpindle(true);
            }
            if (StatMsg.A.Contains("C"))
            {
                ucOverrides.SetStateSpindle(true);
                ucDeviceRouter2.SetStatusSpindle(true);
                ucDeviceLaser2.SetStatusSpindle(true);
            }
            if (!StatMsg.A.Contains("S") && !StatMsg.A.Contains("C"))
            {
                ucOverrides.SetStateSpindle(false);
                ucDeviceRouter2.SetStatusSpindle(false);
                ucDeviceLaser2.SetStatusSpindle(false);
            }  // Spindle off

            if (StatMsg.A.Contains("F"))
            {
                ucOverrides.SetStateFlood(true);
                ucDeviceRouter2.SetStatusFlood(true);
                ucDeviceLaser2.SetStatusMWord("M8");
            }   // Flood on
            else
            {
                ucOverrides.SetStateFlood(false);
                ucDeviceRouter2.SetStatusFlood(false);
                ucDeviceLaser2.SetStatusMWord("M9");
            }

            if (StatMsg.A.Contains("M"))
            {
                ucOverrides.SetStateMist(true);
                ucDeviceRouter2.SetStatusMist(true);
                ucDeviceLaser2.SetStatusMWord("M7");
            } // Mist on
            else
            {
                ucOverrides.SetStateMist(false);
                ucDeviceRouter2.SetStatusMist(false);
                ucDeviceLaser2.SetStatusMWord("M9");
            }


            if (Properties.Settings.Default.grblDescriptionDxEnable)
            {
                if (StatMsg.A.Contains("D"))
                {
                    string digits = StatMsg.A.Substring(StatMsg.A.IndexOf("D") + 1);     // Digital pins in order '3210'

                    ucOverrides.SetStateDx(digits);
                    int din = 0;
                    int dout = 0;
                    if (digits.Length == 4)
                    {
                        for (int i = 0; i < 4; i++)
                        { dout |= ((digits[i] == '1') ? 1 : 0) << (3 - i); }
                    }
                    else if (digits.Length == 8)
                    {
                        for (int i = 0; i < 4; i++)
                        { din |= ((digits[i] == '1') ? 1 : 0) << i; }
                        for (int i = 4; i < 8; i++)
                        { dout |= ((digits[i] == '1') ? 1 : 0) << (7 - i - 4); }
                    }
                    Grbl.grblDigitalIn = (byte)din;
                    Grbl.grblDigitalOut = (byte)dout;
                }
            }

            if (_probing_form != null)
            {
                _probing_form.SetGrblMachineState = StatMsg;
            }

        }
        private void ProcessOverrideValues(string txt)
        {
            _streaming_form2?.ShowOverrideValues(txt);

            string[] value = txt.Split(',');
            if (value.Length > 2)
            {
                ucOverrides.SetLabelFeedSet(value[0]);
                ucOverrides.SetLabelRapidSet(value[1]);
                ucOverrides.SetLabelSpindleSet(value[2]);
            }
        }

        private void ProcessOverrideCurrentFeedSpeed(string txt)
        {
            _streaming_form2?.ShowActualValues(txt);

            string[] value = txt.Split(',');
            if (value.Length > 1)
            {
                ucOverrides.SetLabelFeedValue(value[0]);
                ucOverrides.SetLabelSpindleValue(value[1]);
                ucDeviceLaser.SpindleSet = ucDeviceRouter.SpindleSet = value[1];
            }
            else if (value.Length > 0)
            {
                ucOverrides.SetLabelFeedValue(value[0]);
                ucOverrides.SetLabelSpindleValue("-");
            }
        }

        private void UpdateDRO()
        {
            ucdro.SetWCO(Grbl.posWork);
            ucdro.SetMCO(Grbl.posMachine);
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
                ucStreaming.SetStatusTextGrbl(Grbl.StatusToText(machineStatus), Grbl.GrblStateColor(machineStatus));

                switch (machineStatus)
                {
                    case GrblState.idle:
                        if ((lastMachineStatus == GrblState.hold) || (lastMachineStatus == GrblState.alarm))
                        {
                            StatusStripClear(1, 2);//, "grblState.idle");
                            SetInfoLabel(lastInfoText, SystemColors.Control);

                            if (!_serial_form.CheckGRBLSettingsOk())   // check 30 kHz limit
                            {
                                StatusStripSet(1, Grbl.lastMessage, Color.Fuchsia);
                                StatusStripSet(2, Localization.GetString("statusStripeCheckCOM"), Color.Yellow);
                            }
                        }
                        ucFlowControl.HighlightResume(false);
                        Properties.Settings.Default.DevicePlotterPenInHolder = _serial_form.ToolInSpindle;
                        //           CbTool.Checked = _serial_form.ToolInSpindle;
                        //     if (signalLock > 0)
                        //     { btnKillAlarm.BackColor = SystemColors.Control; signalLock = 0; }
                        ucFlowControl.HighlightKillAlarm(false);
                        if (!isStreaming)                       // update drawing if G91 is used
                            updateDrawingPath = true;

                        Grbl.lastMessage = "";
                        break;

                    case GrblState.run:
                        if (lastMachineStatus == GrblState.hold)
                        {
                            SetInfoLabel(lastInfoText, SystemColors.Control);
                        }
                     //   signalResume = 0;
                    //    btnResume.BackColor = SystemColors.Control;
                        ucFlowControl.HighlightResume(false);
                        break;

                    case GrblState.hold:
                   //     btnResume.BackColor = GroupColor.Yellow;
                        ucFlowControl.HighlightResume(true);
                        lastInfoText = ucStreaming.GetStatusTextStreaming();// lbInfo.Text;
                        lblInfoText = Localization.GetString("mainInfoResume");     //"Press 'Resume' to proceed";

                        if (Grbl.lastErrorNr > 0)
                            lblInfoColor = Color.Fuchsia;
                        else
                            lblInfoColor = Color.Yellow;

                        SetInfoLabel(lblInfoText, lblInfoColor);

                        StatusStripSet(1, Grbl.StatusToText(machineStatus), Grbl.GrblStateColor(machineStatus));
                        StatusStripSet(2, lblInfoText, lblInfoColor);
                //        if (signalResume == 0) { signalResume = 1; }
                        break;

                    case GrblState.home:
                        break;

                    case GrblState.alarm:
                   //     signalLock = 1;
                   //     btnKillAlarm.BackColor = GroupColor.Yellow;
                        ucFlowControl.HighlightKillAlarm(true);
                        lblInfoText = Localization.GetString("mainInfoKill");     //"Press 'Kill Alarm' to proceed";
                        SetInfoLabel(lblInfoText, Color.Yellow);

                        StatusStripSet(1, Grbl.StatusToText(machineStatus) + " " + Grbl.lastMessage, Grbl.GrblStateColor(machineStatus));
                        StatusStripSet(2, lblInfoText, Color.Yellow);
                        Grbl.lastMessage = "";
                        _heightmap_form?.StopScan();
                        break;

                    case GrblState.check:
                        break;

                    case GrblState.door:
                        ucFlowControl.HighlightResume(true);
                        lastInfoText = ucStreaming.GetStatusTextStreaming(); //lbInfo.Text;
                        lblInfoText = Localization.GetString("mainInfoResume");     //"Press 'Resume' to proceed";
                        SetInfoLabel(lblInfoText, Color.Yellow);

                        StatusStripSet(1, Grbl.StatusToText(machineStatus), Grbl.GrblStateColor(machineStatus));
                        StatusStripSet(2, lblInfoText, Color.Yellow);
                    //    if (signalResume == 0) { signalResume = 1; }
                        break;

                    case GrblState.probe:
                        posProbe = _serial_form.posProbe;
                        if (_diyControlPad != null)
                        {
                            if (alternateZ != null)
                                posProbe.Z = (double)alternateZ;
                        }
                        if (_heightmap_form != null)
                            _heightmap_form.SetPosProbe = posProbe;

                        if (_probing_form != null)
                        {
                            Logger.Info("Update Probing {0}", Grbl.DisplayCoord("PRB"));
                            _probing_form.SetPosProbe = Grbl.GetCoord("PRB");
                        }

                        lastInfoText = ucStreaming.GetStatusTextStreaming(); //lbInfo.Text;
                        SetInfoLabel(string.Format("{0}: X:{1:0.00} Y:{2:0.00} Z:{3:0.00}", Localization.GetString("mainInfoProbing"), posProbe.X, posProbe.Y, posProbe.Z), Color.Yellow);
                        break;

                    case GrblState.unknown:
                        UpdateControlEnables();
                        break;

                    case GrblState.notConnected:
                        SetInfoLabel("No connection - press 'RESET'", Color.Fuchsia);
                        StatusStripSet(1, "No connection - press 'RESET'", Color.Fuchsia);
                        StatusStripSet(2, msg, Color.Fuchsia);
                        isStreaming = false;
                        Grbl.Clear();
                        GuiEnableAxisABC();
                        UpdateControlEnables();
                        break;

                    default:
                        break;
                }
                if (_probing_form != null)
                { _probing_form.SetGrblState = machineStatus; }

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

                ucdro.SetGLabel(cmd.coord_select);
                if (_camera_form != null)
                    _camera_form.SetCoordG = cmd.coord_select;
                if (_coordSystem_form != null)
                {
                    _coordSystem_form.MarkActiveCoordSystem("G" + cmd.coord_select.ToString());// lblCurrentG.Text);
                    _coordSystem_form.UpdateTLO(cmd.TLOactive, cmd.tool_length);
                }
            }
        }	// Except: Could not load file or assembly 'AForge, Version=2.2.5.0, Culture=neutral, PublicKeyToken=c1db6ff4eaa06aeb' or one of its dependencies. The system cannot find the file specified. Source: GRBL-Plotter Target: Void ProcessParserState(GrblPlotter.ParsState), File: D:\Projekte\GRBL-Plotter\GRBL-Plotter\GUI\MainFormInterface.cs, Method:ProcessParserState, LineNumber: 443

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
                StatusStripClear(0);
                if (!Grbl.isMarlin) StatusStripSet(2, string.Format("grbl-controller connected: vers: {0}, axis: {1}, buffer: {2}", _serial_form.GrblVers, Grbl.axisCount, Grbl.RX_BUFFER_SIZE), Color.Lime);
                else StatusStripSet(2, string.Format("Marlin connected: axis: {0}", Grbl.axisCount), Color.Lime);

                delayedStatusStripClear1 = 8;
                GuiEnableAxisABC();
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
                    ucDeviceLaser.SpindleMax = ucDeviceRouter.SpindleMax = (int)Grbl.GetSetting(id);
                    break;
                case 31:
                    ucDeviceLaser.SpindleMin = ucDeviceRouter.SpindleMin = (int)Grbl.GetSetting(id);
                    break;
                case 32:
                    ucDeviceLaser.LaserMode = (Grbl.GetSetting(id) == 1);
                    break;

                default:
                    break;
            }
        }
    }
}