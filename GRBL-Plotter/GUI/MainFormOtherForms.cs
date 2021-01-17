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
/* 
 * 2020-09-18 split file
 * 2021-01-13 add 3rd serial com
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

/********************************************************************
 * Handle additional Forms, which can be opened via Menu strip
 * Handles just open, close, add event handler
 ********************************************************************/
        GCodeFromText _text_form = null;
        GCodeFromImage _image_form = null;
        GCodeFromShape _shape_form = null;
        GCodeForBarcode _barcode_form = null;

        ControlStreamingForm _streaming_form = null;
        ControlStreamingForm2 _streaming_form2 = null;
        ControlSerialForm _serial_form2 = null;
        SimpleSerialForm _serial_form3 = null;
        Control2ndGRBL _2ndGRBL_form = null;
        ControlCameraForm _camera_form = null;
        ControlDIYControlPad _diyControlPad = null;
        ControlCoordSystem _coordSystem_form = null;
        ControlLaser _laser_form = null;
        ControlProbing _probing_form = null;
        ControlHeightMapForm _heightmap_form = null;
        ControlSetupForm _setup_form = null;
        ControlJogPathCreator _jogPathCreator_form = null;



        #region MAIN-MENU GCode creation
        /********************************************************************
         * Text
         * _text_form
         ********************************************************************/
        private void textWizzardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_text_form == null)
            {
                _text_form = new GCodeFromText();
                _text_form.FormClosed += formClosed_TextToGCode;
                _text_form.btnApply.Click += getGCodeFromText;      // assign btn-click event
            }
            else
            {
                _text_form.Visible = false;
            }
            _text_form.Show(this);
            _text_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_TextToGCode(object sender, FormClosedEventArgs e)
        { _text_form = null; }

/********************************************************************
 * Image
 * _image_form
 ********************************************************************/
        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_image_form == null)
            {
                _image_form = new GCodeFromImage();
                _image_form.FormClosed += formClosed_ImageToGCode;
                _image_form.btnGenerate.Click += getGCodeFromImage;      // assign btn-click event
            }
            else
            {
                _image_form.Visible = false;
            }
            _image_form.Show(this);
            _image_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_ImageToGCode(object sender, FormClosedEventArgs e)
        { _image_form = null; }

/********************************************************************
 * Shape
 * _shape_form
 ********************************************************************/
        private void createSimpleShapesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_shape_form == null)
            {
                _shape_form = new GCodeFromShape();
                _shape_form.FormClosed += formClosed_ShapeToGCode;
                _shape_form.btnApply.Click += getGCodeFromShape;      // assign btn-click event
            }
            else
            {
                _shape_form.Visible = false;
            }
            _shape_form.Show(this);
            _shape_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_ShapeToGCode(object sender, FormClosedEventArgs e)
        { _shape_form = null; }

/********************************************************************
 * Barcode
 * _barcode_form
 ********************************************************************/
        private void createBarcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_barcode_form == null)
            {
                _barcode_form = new GCodeForBarcode();
                _barcode_form.FormClosed += formClosed_BarcodeToGCode;
                _barcode_form.btnGenerateBarcode1D.Click += getGCodeFromBarcode;      // assign btn-click event
                _barcode_form.btnGenerateBarcode2D.Click += getGCodeFromBarcode;      // assign btn-click event
            }
            else
            {
                _barcode_form.Visible = false;
            }
            _barcode_form.Show(this);
            _barcode_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_BarcodeToGCode(object sender, FormClosedEventArgs e)
        { _barcode_form = null; }

#endregion

#region MAIN-Menu Control
/********************************************************************
 * Streaming - Override Controls for grbl 0.9 or 1.1 - probably obsolet
 * _streaming_form, _streaming_form2
 ********************************************************************/
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



/********************************************************************
 * Control 2nd grbl via 2nd COM
 * _serial_form2, _2ndGRBL_form
 ********************************************************************/
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
		
/********************************************************************
 * Control 3rd serial COM
 * _serial_form3
 ********************************************************************/
        private void control3rdSerialCOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((_serial_form3 == null) || (_serial_form3.Disposing) || (_serial_form3.IsDisposed))
            {
                _serial_form3 = new SimpleSerialForm();// "COM Tool changer", 3);
                _serial_form3.FormClosed += formClosed_3rdSerialCOMForm;
            }
            else
            {
                _serial_form3.Visible = false;
            }
            Logger.Info("control3rdSerialCOMToolStripMenuItem_Click {0}", _serial_form3);
            _serial_form.set3rdSerial(_serial_form3);
            _serial_form3.Show(this);
            _serial_form3.WindowState = FormWindowState.Normal;
        }
        private void formClosed_3rdSerialCOMForm(object sender, FormClosedEventArgs e)
        {
            _serial_form3 = null;
            _serial_form.set3rdSerial();    // sign out
            Logger.Info("formClosed_3rdSerialCOMForm {0}", _serial_form3);
        }

/********************************************************************
* Camera 
* _camera_form
********************************************************************/
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


/********************************************************************
 * DIY control pad - take commands and Probe-results from outside
 * _diyControlPad
 ********************************************************************/
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


/********************************************************************
 * Coordinate systems
 * _coordSystem_form
 ********************************************************************/
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


/********************************************************************
 * Laser setup
 * _laser_form
 ********************************************************************/
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


/********************************************************************
 * Edge finder
 * _probing_form
 ********************************************************************/
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

            timerUpdateControlSource = "OnRaiseProbingEvent";
            updateControls();
            Properties.Settings.Default.counterUseProbing += 1;
        }


/********************************************************************
 * Height map
 * _heightmap_form
 ********************************************************************/
        private void heightMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_heightmap_form == null)
            {
                _heightmap_form = new ControlHeightMapForm();
                _heightmap_form.FormClosed += formClosed_HeightmapForm;
                _heightmap_form.btnStartHeightScan.Click += getGCodeScanHeightMap;      // in MainFormGetCodeTransform
                _heightmap_form.loadHeightMapToolStripMenuItem.Click += loadHeightMap;	// in MainFormGetCodeTransform
                _heightmap_form.btnApply.Click += applyHeightMap;						// in MainFormGetCodeTransform
                _heightmap_form.RaiseXYZEvent += OnRaisePositionClickEvent;				// in MainForm
                _heightmap_form.btnGCode.Click += getGCodeFromHeightMap;      			// in MainFormGetCodeTransform
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


/********************************************************************
 * Setup Form
 * _setup_form
 ********************************************************************/
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

/********************************************************************
 * Jog Path creator
 * _setup_form
 ********************************************************************/
        private void jogCreatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_jogPathCreator_form == null)
            {
                _jogPathCreator_form = new ControlJogPathCreator();
                _jogPathCreator_form.FormClosed += formClosed_JogCreator;
                _jogPathCreator_form.btnJogStart.Click += getGCodeJogCreator;      // assign btn-click event
                _jogPathCreator_form.btnExport.Click += getGCodeJogCreator2;      // assign btn-click event
                _jogPathCreator_form.btnJogStop.Click += btnJogStop_Click;      // assign btn-click event
            }
            else
            {
                _jogPathCreator_form.Visible = false;
            }
            _jogPathCreator_form.Show(null);// this);
            _jogPathCreator_form.WindowState = FormWindowState.Normal;
        }
        private void formClosed_JogCreator(object sender, FormClosedEventArgs e)
        { _jogPathCreator_form = null; }

/********************************************************************
* About Form
********************************************************************/
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }



#endregion

    }
}