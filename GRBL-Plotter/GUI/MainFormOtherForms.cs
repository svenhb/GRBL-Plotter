/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
 * 2021-07-15 code clean up / code quality
 * 2022-03-06 changed from form.show(this) to .show() to be able to stay behaind main form
 * 2022-04-06 add _projector_form with monitor selection
*/

using System;
using System.Windows.Forms;

namespace GrblPlotter
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
        ControlDiyControlPad _diyControlPad = null;
        ControlCoordSystem _coordSystem_form = null;
        ControlLaser _laser_form = null;
        ControlProbing _probing_form = null;
        ControlHeightMapForm _heightmap_form = null;
        ControlSetupForm _setup_form = null;
        ControlJogPathCreator _jogPathCreator_form = null;
        ControlProjector _projector_form = null;


        #region MAIN-MENU GCode creation
        /********************************************************************
         * Text
         * _text_form
         ********************************************************************/
        private void TextWizzardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_text_form == null)
            {
                _text_form = new GCodeFromText();
                _text_form.FormClosed += FormClosed_TextToGCode;
                _text_form.btnApply.Click += GetGCodeFromText;      // assign btn-click event
            }
            else
            {
                _text_form.Visible = false;
            }

            if (showFormInFront) _text_form.Show(this);
            else _text_form.Show();  // this);

            showFormsToolStripMenuItem.Visible = true;
            _text_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_TextToGCode(object sender, FormClosedEventArgs e)
        { _text_form = null; }

        /********************************************************************
         * Image
         * _image_form
         ********************************************************************/
        private void ImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_image_form == null)
            {
                _image_form = new GCodeFromImage();
                _image_form.FormClosed += FormClosed_ImageToGCode;
                _image_form.btnGenerate.Click += GetGCodeFromImage;      // assign btn-click event in MainFormgetCodetransform.cs
                _image_form.BtnReloadPattern.Click += LoadLastGraphic;
                _image_form.CBoxPatternFiles.SelectedIndexChanged += LoadSelectedGraphicImage;
            }
            else
            {
                _image_form.Visible = false;
            }
            if (showFormInFront) _image_form.Show(this);
            else _image_form.Show();
			
            showFormsToolStripMenuItem.Visible = true;
            _image_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_ImageToGCode(object sender, FormClosedEventArgs e)
        { _image_form = null; }

        /********************************************************************
         * Shape
         * _shape_form
         ********************************************************************/
        private void CreateSimpleShapesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_shape_form == null)
            {
                _shape_form = new GCodeFromShape();
                _shape_form.FormClosed += FormClosed_ShapeToGCode;
                _shape_form.btnApply.Click += GetGCodeFromShape;      // assign btn-click event
            }
            else
            {
                _shape_form.Visible = false;
            }

            if (showFormInFront) _shape_form.Show(this);
            else _shape_form.Show(); // this);

            showFormsToolStripMenuItem.Visible = true;
            _shape_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_ShapeToGCode(object sender, FormClosedEventArgs e)
        { _shape_form = null; }

        /********************************************************************
         * Barcode
         * _barcode_form
         ********************************************************************/
        private void CreateBarcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_barcode_form == null)
            {
                _barcode_form = new GCodeForBarcode();
                _barcode_form.FormClosed += FormClosed_BarcodeToGCode;
                _barcode_form.btnGenerateBarcode1D.Click += GetGCodeFromBarcode;      // assign btn-click event
                _barcode_form.btnGenerateBarcode2D.Click += GetGCodeFromBarcode;      // assign btn-click event
            }
            else
            {
                _barcode_form.Visible = false;
            }

            if (showFormInFront) _barcode_form.Show(this);
            else _barcode_form.Show();   // this);

            showFormsToolStripMenuItem.Visible = true;
            _barcode_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_BarcodeToGCode(object sender, FormClosedEventArgs e)
        { _barcode_form = null; }

        #endregion

        #region MAIN-Menu Control
        /********************************************************************
         * Streaming - Override Controls for grbl 0.9 or 1.1 - probably obsolet
         * _streaming_form, _streaming_form2
         ********************************************************************/
        private void ControlStreamingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Grbl.isVersion_0)
            {
                if (_streaming_form2 != null)
                    _streaming_form2.Visible = false;
                if (_streaming_form == null)
                {
                    _streaming_form = new ControlStreamingForm();
                    _streaming_form.FormClosed += FormClosed_StreamingForm;
                    _streaming_form.RaiseOverrideEvent += OnRaiseOverrideEvent;      // assign  event
                    _streaming_form.ShowValueFR(actualFR);
                    _streaming_form.ShowValueSS(actualSS);
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
                    _streaming_form2.FormClosed += FormClosed_StreamingForm;
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
        private void FormClosed_StreamingForm(object sender, FormClosedEventArgs e)
        { _streaming_form = null; _streaming_form2 = null; }



        /********************************************************************
         * Control 2nd grbl via 2nd COM
         * _serial_form2, _2ndGRBL_form
         ********************************************************************/
        private void Control2ndGRBLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_2ndGRBL_form == null)
            {
                _2ndGRBL_form = new Control2ndGRBL(_serial_form2);
                if (_serial_form2 == null)
                {
                    _serial_form2 = new ControlSerialForm("COM Tool changer", 2);
                    _serial_form2.Show(this);
                }
                _2ndGRBL_form.Set2ndSerial(_serial_form2);
                _serial_form.Set2ndSerial(_serial_form2);
                _2ndGRBL_form.FormClosed += FormClosed_2ndGRBLForm;
            }
            else
            {
                _2ndGRBL_form.Visible = false;
            }
            _2ndGRBL_form.Show(this);
            _2ndGRBL_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_2ndGRBLForm(object sender, FormClosedEventArgs e)
        { _2ndGRBL_form = null; }

        /********************************************************************
         * Control 3rd serial COM
         * _serial_form3
         ********************************************************************/
        private void Control3rdSerialCOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((_serial_form3 == null) || (_serial_form3.Disposing) || (_serial_form3.IsDisposed))
            {
                _serial_form3 = new SimpleSerialForm();// "COM Tool changer", 3);
                _serial_form3.FormClosed += FormClosed_3rdSerialCOMForm;
            }
            else
            {
                _serial_form3.Visible = false;
            }
            Logger.Info("control3rdSerialCOMToolStripMenuItem_Click {0}", _serial_form3);
            _serial_form.Set3rdSerial(_serial_form3);
            _serial_form3.Show(this);
            _serial_form3.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_3rdSerialCOMForm(object sender, FormClosedEventArgs e)
        {
            _serial_form3 = null;
            _serial_form.Set3rdSerial(null);    // sign out
            Logger.Info("formClosed_3rdSerialCOMForm {0}", _serial_form3);
        }

        /********************************************************************
        * Camera 
        * _camera_form
        ********************************************************************/
        private void CameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_camera_form == null)
            {
                _camera_form = new ControlCameraForm();
                _camera_form.FormClosed += FormClosed_CameraForm;
                _camera_form.RaiseXYEvent += OnRaiseCameraClickEvent;
                //               _camera_form.setPosMarker(grbl.posMarker);// visuGCode.GetPosMarker());
            }
            else
            {
                _camera_form.Visible = false;
            }

            if (showFormInFront) _camera_form.Show(this);
            else _camera_form.Show();        // this);

            showFormsToolStripMenuItem.Visible = true;
            _camera_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_CameraForm(object sender, FormClosedEventArgs e)
        { _camera_form = null; }


        /********************************************************************
         * DIY control pad - take commands and Probe-results from outside
         * _diyControlPad
         ********************************************************************/
        private void DIYControlopen(object sender, EventArgs e)
        {
            if (_diyControlPad == null)
            {
                _diyControlPad = new ControlDiyControlPad();
                _diyControlPad.FormClosed += FormClosed_DIYControlForm;
                _diyControlPad.RaiseStreamEvent += OnRaiseDIYCommandEvent;
            }
            else
            {
                _diyControlPad.Visible = false;
            }
            _diyControlPad.Show(this);
            _diyControlPad.WindowState = FormWindowState.Normal;
            if (_heightmap_form != null)
            { _heightmap_form.DiyControlConnected = _diyControlPad.IsConnected; }
        }
        private void FormClosed_DIYControlForm(object sender, FormClosedEventArgs e)
        { _diyControlPad = null; }
		
        private void OnRaiseDIYCommandEvent(object sender, CommandEventArgs e)
        {
            if (e.RealTimeCommand > 0x00)
            {
                if (!isStreaming || isStreamingPause)
                    SendRealtimeCommand(e.RealTimeCommand);
            }
            else
            {
                if ((!isStreaming || isStreamingPause) && !_serial_form.IsHeightProbing)    // only hand over DIY commands in normal mode
                    SendCommand(e.Command);
                if (e.Command.StartsWith("(PRB:Z"))
                {
                    string num = e.Command.Substring(6);
               //     double myZ;
                    num = num.Trim(')');
                    alternateZ = null;
                    if (double.TryParse(num, out double myZ))
                    { alternateZ = myZ; }
                    else
                    { _diyControlPad.SendFeedback("Error in parsing " + num, true); }
                }
            }
        }


        /********************************************************************
         * Coordinate systems
         * _coordSystem_form
         ********************************************************************/
        private void CoordSystemopen(object sender, EventArgs e)
        {
            if (_coordSystem_form == null)
            {
                _coordSystem_form = new ControlCoordSystem();
                _coordSystem_form.FormClosed += FormClosed_CoordSystemForm;
                _coordSystem_form.RaiseCmdEvent += OnRaiseCoordSystemEvent;
            }
            else
            {
                _coordSystem_form.Visible = false;
            }

            if (showFormInFront) _coordSystem_form.Show(this);
            else _coordSystem_form.Show();       // this);

            showFormsToolStripMenuItem.Visible = true;
            _coordSystem_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_CoordSystemForm(object sender, FormClosedEventArgs e)
        { _coordSystem_form = null; }
		
        private void OnRaiseCoordSystemEvent(object sender, CmdEventArgs e)
        {
            SendCommand(e.Command);
        }


        /********************************************************************
         * Laser setup
         * _laser_form
         ********************************************************************/
        private void Laseropen(object sender, EventArgs e)
        {
            if (_laser_form == null)
            {
                _laser_form = new ControlLaser();
                _laser_form.FormClosed += FormClosed_LaserForm;
                _laser_form.RaiseCmdEvent += OnRaiseLaserEvent;
            }
            else
            {
                _laser_form.Visible = false;
            }

            if (showFormInFront) _laser_form.Show(this);
            else _laser_form.Show();     // this);

            showFormsToolStripMenuItem.Visible = true;
            _laser_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_LaserForm(object sender, FormClosedEventArgs e)
        { _laser_form = null; }
		
        private void OnRaiseLaserEvent(object sender, CmdEventArgs e)
        {
            if (!_serial_form.RequestSend(e.Command, true))     // check if COM is still open
                UpdateControlEnables();
            Properties.Settings.Default.counterUseLaserSetup += 1;
        }


        /********************************************************************
         * Edge finder
         * _probing_form
         ********************************************************************/
        private void EdgeFinderopen(object sender, EventArgs e)
        {
            if (_probing_form == null)
            {
                _probing_form = new ControlProbing();
                _probing_form.FormClosed += FormClosed_ProbingForm;
                _probing_form.RaiseCmdEvent += OnRaiseProbingEvent;
                _probing_form.btnGetAngleEF.Click += BtnGetAngleEF_Click;
            }
            else
            {
                _probing_form.Visible = false;
            }

            if (showFormInFront) _probing_form.Show(this);
            else _probing_form.Show();       // this);

            showFormsToolStripMenuItem.Visible = true;
            _probing_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_ProbingForm(object sender, FormClosedEventArgs e)
        { _probing_form = null; }
		
        private void BtnGetAngleEF_Click(object sender, EventArgs e)
        {
            if ((VisuGCode.xyzSize.dimx > 0) && (VisuGCode.xyzSize.dimy > 0))
            {
                TransformStart("Rotate");
                fCTBCode.Text = VisuGCode.TransformGCodeRotate(_probing_form.GetAngle, 1, new XyPoint(0, 0), false);    // use given center
                TransformEnd();
            }
        }
        private void OnRaiseProbingEvent(object sender, CmdEventArgs e)
        {
            string[] commands;
            commands = e.Command.Split(';');
            if (!_serial_form.SerialPortOpen)
                return;
            foreach (string btncmd in commands)
            {
                SendCommand(btncmd.Trim());
            }

            timerUpdateControlSource = "OnRaiseProbingEvent";
            UpdateControlEnables();
            Properties.Settings.Default.counterUseProbing += 1;
        }


        /********************************************************************
         * Height map
         * _heightmap_form
         ********************************************************************/
        private void HeightMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_heightmap_form == null)
            {
                _heightmap_form = new ControlHeightMapForm();
                _heightmap_form.FormClosed += FormClosed_HeightmapForm;
                _heightmap_form.btnStartHeightScan.Click += GetGCodeScanHeightMap;      // in MainFormGetCodeTransform
                _heightmap_form.loadHeightMapToolStripMenuItem.Click += LoadHeightMap;	// in MainFormGetCodeTransform
                _heightmap_form.btnApply.Click += ApplyHeightMap;						// in MainFormGetCodeTransform
                _heightmap_form.RaiseXyzEvent += OnRaisePositionClickEvent;				// in MainForm
                _heightmap_form.btnGCode.Click += GetGCodeFromHeightMap;      			// in MainFormGetCodeTransform
            }
            else
            {
                _heightmap_form.Visible = false;
            }

            if (showFormInFront) _heightmap_form.Show(this);
            else _heightmap_form.Show(); // this);

            showFormsToolStripMenuItem.Visible = true;
            _heightmap_form.WindowState = FormWindowState.Normal;
            if (_diyControlPad != null)
            { _heightmap_form.DiyControlConnected = _diyControlPad.IsConnected; }
        }
        private void FormClosed_HeightmapForm(object sender, FormClosedEventArgs e)
        {
            _heightmap_form = null;
            VisuGCode.ClearHeightMap();
            _serial_form.StopStreaming(true);   // isNotStartup = true
        }


        /********************************************************************
         * Setup Form
         * _setup_form
         ********************************************************************/
        private void SetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_setup_form == null)
            {
                _setup_form = new ControlSetupForm();
                _setup_form.FormClosed += FormClosed_SetupForm;
                _setup_form.btnApplyChangings.Click += LoadSettings;
                _setup_form.btnReloadFile.Click += ReStartConvertFileFromSetup;
                _setup_form.btnMoveToolXY.Click += MoveToPickup;
                _setup_form.btnGCPWMUp.Click += MoveToPickup;
                _setup_form.btnGCPWMDown.Click += MoveToPickup;
                _setup_form.btnGCPWMZero.Click += MoveToPickup;
                _setup_form.SetLastLoadedFile(lastLoadSource);
                gamePadTimer.Enabled = false;
            }
            else
            {
                _setup_form.Visible = false;
            }

            if (showFormInFront) _setup_form.Show(this);
            else _setup_form.Show();// null);// this);

            showFormsToolStripMenuItem.Visible = true;
            _setup_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_SetupForm(object sender, FormClosedEventArgs e)
        {
            LoadSettings(sender, e);
            _setup_form = null;
            VisuGCode.DrawMachineLimit();// ToolTable.GetToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
            gamePadTimer.Enabled = Properties.Settings.Default.gamePadEnable;
        }

        /********************************************************************
         * Jog Path creator
         * _setup_form
         ********************************************************************/
        private void JogCreatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_jogPathCreator_form == null)
            {
                _jogPathCreator_form = new ControlJogPathCreator();
                _jogPathCreator_form.FormClosed += FormClosed_JogCreator;
                _jogPathCreator_form.btnJogStart.Click += GetGCodeJogCreator;      // assign btn-click event
                _jogPathCreator_form.btnExport.Click += GetGCodeJogCreator2;      // assign btn-click event
                _jogPathCreator_form.btnJogStop.Click += BtnJogStop_Click;      // assign btn-click event
            }
            else
            {
                _jogPathCreator_form.Visible = false;
            }
            _jogPathCreator_form.Show(null);// this);
            _jogPathCreator_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_JogCreator(object sender, FormClosedEventArgs e)
        { _jogPathCreator_form = null; }

 
		/********************************************************************
         * Projector - a form to be displayed via a projector on the workpiece
         * _projector_form
         ********************************************************************/
        private void ProjectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_projector_form == null)
            {
                _projector_form = new ControlProjector();
                _projector_form.FormClosed += FormClosed_Projector;
            }
            else
            {
                _projector_form.Visible = false;
            }
			
			if (Screen.AllScreens.Length > 1)
			{	
				if ((int)Properties.Settings.Default.projectorMonitorIndex >= Screen.AllScreens.Length)
					Properties.Settings.Default.projectorMonitorIndex = Screen.AllScreens.Length - 1;
				
				_projector_form.StartPosition = FormStartPosition.Manual;
				_projector_form.Location = Screen.AllScreens[(int)Properties.Settings.Default.projectorMonitorIndex].WorkingArea.Location;	// selectable index
				_projector_form.FormBorderStyle = FormBorderStyle.None;		// default = Sizable
				_projector_form.Show();
				_projector_form.WindowState = FormWindowState.Maximized;
			} 
			else
			{	_projector_form.Show(this);
				_projector_form.WindowState = FormWindowState.Normal;
			}
        }
        private void FormClosed_Projector(object sender, FormClosedEventArgs e)
        { 	_projector_form = null; }


        /********************************************************************
        * About Form
        ********************************************************************/
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }
        #endregion
    }
}