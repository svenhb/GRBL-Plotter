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
/* * 2020-09-18 split file
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
using System.Threading.Tasks;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {

        private string oldRaw = "";
        private grblState lastMachineStatus = grblState.unknown;
        private string lastInfoText = "";
        private string lastLabelInfoText = "";
        private bool updateDrawingPath = false;
        private grblState machineStatus;
        private mState machineStatusMessage;
        private pState machineParserState;

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
            machineStatus = e.Status;
            machineStatusMessage = e.StatMsg;
            machineParserState = e.parserState;

/***** Restore saved position after reset and set initial feed rate: *****/
            if (flagResetOffset || (e.Status == grblState.reset))
            { processReset(); }

/***** process grblState {idle, run, hold, home, alarm, check, door} *****/
            processStatus();

/***** check and submit override values, set labels, checkbox *****/
            processStatusMessage(e.StatMsg);

/***** set DRO digital-read-out labels with machine and work coordinates *****/
			if (!simuEnabled)
				updateDRO();
				
/***** parser state Spinde/Coolant on/off, on other Forms: FeedRate, SpindleSpeed, G54-Coord *****/
            processParserState(e.parserState); 
			
/***** update 2D view *****/			
            if (grbl.posChanged)
            {   VisuGCode.createMarkerPath();
                VisuGCode.updatePathPositions();
                checkMachineLimit();
                pictureBox1.Invalidate();
                if (Properties.Settings.Default.flowCheckRegistryChange && !isStreaming)
                    gui.writePositionToRegistry();
                grbl.posChanged = false;
            }
            if (grbl.wcoChanged)
            {   checkMachineLimit();
                grbl.wcoChanged = false;
            }
			if (((isStreaming || isStreamingRequestStop)) && Properties.Settings.Default.guiProgressShow)
                VisuGCode.ProcessedPath.processedPathDraw(grbl.posWork);
			
			
            if (_diyControlPad != null)
            {   if (oldRaw != e.Raw)
                {   _diyControlPad.sendFeedback(e.Raw);     //hand over original grbl text
                    oldRaw = e.Raw;
                }
            }			
        }




		private void processStatusMessage(mState StatMsg)
		{   
            if (logPosEvent) Logger.Trace("processStatusMessage  ");
			if (StatMsg.Ov.Length > 1)    
            {	processOverrideValues(StatMsg.Ov);	}
            if (StatMsg.FS.Length > 1)   
            {	processOverrideCurrentFeedSpeed(StatMsg.FS);  }

			cBSpindle.CheckedChanged -= cBSpindle_CheckedChanged;	// disable event-handler
			cBCoolant.CheckedChanged -= cBCoolant_CheckedChanged;

			if (StatMsg.A.Contains("S"))
			{	btnOverrideSpindle.Image = Properties.Resources.led_on;   // Spindle on CW
				btnOverrideSpindle.Text = "Spindle CW";
				cBSpindle.Checked = true;
			}
			if (StatMsg.A.Contains("C"))
			{	btnOverrideSpindle.Image = Properties.Resources.led_on;   // Spindle on CCW
				btnOverrideSpindle.Text = "Spindle CCW";
				cBSpindle.Checked = true;
			}
			if (!StatMsg.A.Contains("S") && !StatMsg.A.Contains("C"))
			{ btnOverrideSpindle.Image = Properties.Resources.led_off; cBSpindle.Checked = false; }  // Spindle off

			if (StatMsg.A.Contains("F")) { btnOverrideFlood.Image = Properties.Resources.led_on; cBCoolant.Checked = true; }   // Flood on
			else { btnOverrideFlood.Image = Properties.Resources.led_off; cBCoolant.Checked = false; }

			if (StatMsg.A.Contains("M")) { btnOverrideMist.Image = Properties.Resources.led_on; } // Mist on
			else { btnOverrideMist.Image = Properties.Resources.led_off; }

			cBCoolant.CheckedChanged += cBCoolant_CheckedChanged;	// enable even-handler
			cBSpindle.CheckedChanged += cBSpindle_CheckedChanged;
		}

		private void processOverrideValues(string txt)
        {   if (_streaming_form2 != null)
			{ _streaming_form2.showOverrideValues(txt); }

			string[] value = txt.Split(',');
			if (value.Length > 2)
			{
				lblOverrideFRValue.Text = value[0];
				lblOverrideRapidValue.Text = value[1];
				lblOverrideSSValue.Text = value[2];
			}
		}

		private void processOverrideCurrentFeedSpeed(string txt)
        {   if (_streaming_form2 != null)
            {   _streaming_form2.showActualValues(txt);}
		
			string[] value = txt.Split(',');
			if (value.Length > 1)
			{	lblStatusFeed.Text = value[0];	// + " mm/min";
				lblStatusSpeed.Text = value[1];	// + " RPM";
			}
			else
			{	lblStatusFeed.Text = value[0];	// + " mm/min";
				lblStatusSpeed.Text = "-";		// + " RPM";
			}
		}


		private void updateDRO()
		{   label_mx.Text = string.Format("{0:0.000}", grbl.posMachine.X);
            label_my.Text = string.Format("{0:0.000}", grbl.posMachine.Y);
            label_mz.Text = string.Format("{0:0.000}", grbl.posMachine.Z);
            label_wx.Text = string.Format("{0:0.000}", grbl.posWork.X);
            label_wy.Text = string.Format("{0:0.000}", grbl.posWork.Y);
            label_wz.Text = string.Format("{0:0.000}", grbl.posWork.Z);
            if (grbl.axisA)
            {   label_ma.Text = string.Format("{0:0.000}", grbl.posMachine.A);
                label_wa.Text = string.Format("{0:0.000}", grbl.posWork.A);
            }
            if (grbl.axisB)
            {   label_mb.Text = string.Format("{0:0.000}", grbl.posMachine.B);
                label_wb.Text = string.Format("{0:0.000}", grbl.posWork.B);
            }
            if (grbl.axisC)
            {   label_mc.Text = string.Format("{0:0.000}", grbl.posMachine.C);
                label_wc.Text = string.Format("{0:0.000}", grbl.posWork.C);
            }			
		}

/***************************************************************
 * handle status events from serial form
 * {idle, run, hold, home, alarm, check, door}
 * only action on status change
 ***************************************************************/
        private void processStatus() 
        {
            if (logPosEvent) Logger.Trace("processStatus  Status:{0}", machineStatus.ToString());
            if ((machineStatus != lastMachineStatus) || (grbl.lastMessage.Length > 5))
            {
				// label at DRO
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
						posProbe = _serial_form.posProbe;
						if (_diyControlPad != null)
						{
							if (alternateZ != null)
								posProbe.Z = (double)alternateZ;
							//_diyControlPad.sendFeedback("Probe: "+posProbe.Z.ToString());
						}
						if (_heightmap_form != null)
							_heightmap_form.setPosProbe = posProbe;

						if (_probing_form != null)
						{
							Logger.Info("Update Probing {0}", grbl.displayCoord("PRB"));
                            _probing_form.setPosProbe = grbl.getCoord("PRB");
                        }

                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = string.Format("{0}: X:{1:0.00} Y:{2:0.00} Z:{3:0.00}", Localization.getString("mainInfoProbing"), posProbe.X, posProbe.Y, posProbe.Z);
                        lbInfo.BackColor = Color.Yellow;
                        break;

                    case grblState.unknown:
                        timerUpdateControlSource = "grblState.unknown";
                        updateControls();
//                        if (Properties.Settings.Default.serialMinimize && _serial_form.serialPortOpen)
//                            _serial_form.WindowState = FormWindowState.Minimized;
                        break;

                    default:
                        break;
                }
                if (_probing_form != null)
                {   _probing_form.setGrblSaState = machineStatus; }

            }
            lastMachineStatus = machineStatus;
        }


/********************************************************
 * handle last sent commands from serial form
 * FeedRate, SpindleSpeed, Spinde/Coolant on/off, G54-Coord
 * update other forms
 ********************************************************/
        private void processParserState(pState cmd)
        {
            if (logPosEvent) Logger.Trace("processParserState FR:{0}  SS:{1}  spindle:{2}  coolant:{3}  Tool:{4}  Coord:{5}",cmd.FR.ToString(), cmd.SS.ToString(), cmd.spindle, cmd.coolant, cmd.tool.ToString(), cmd.coord_select.ToString());
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
                { 	_coordSystem_form.markActiveCoordSystem(lblCurrentG.Text);
                    _coordSystem_form.updateTLO(cmd.TLOactive, cmd.tool_length);
                }
            }
        }

		private void processReset()
        {
            timerUpdateControlSource = "processReset";
            updateControls();
            if (logPosEvent) Logger.Trace("processReset");
            if (!_serial_form.checkGRBLSettingsOk())   // check 30 kHz limit
			{	statusStripSet(1, grbl.lastMessage, Color.Orange);
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

				Logger.Info("restoreWorkCoordinates [Setup - Program behavior - Flow control - Behavior after grbl reset]");
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
			{   _serial_form.addToLog("* Code after reset: " + Properties.Settings.Default.resetSendCode + " in [Setup - Program behavior - Flow control]");
				processCommands(Properties.Settings.Default.resetSendCode);
			}

			setGRBLBuffer();
			flagResetOffset = false;
            MainTimer.Stop();
            MainTimer.Start();
            timerUpdateControls = true; timerUpdateControlSource = "process Reset";
            if (logPosEvent) Logger.Trace("ResetEvent()  connect {0}", _serial_form.serialPortOpen);
            resetStreaming();
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
                statusStripSet(1, string.Format("grbl-controller connected: vers: {0}, axis: {1}, buffer: {2}", _serial_form.grblVers, grbl.axisCount, grbl.RX_BUFFER_SIZE), Color.Lime);
            }
        }

    }
}