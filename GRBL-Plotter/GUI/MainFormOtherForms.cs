/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2023-01-04 add _process_form
 * 2023-01-24 line 369 check if _diyControlPad != null
 * 2023-01-27 processAutomation removed fullScreen on start (like in ProjectorToolStrip)
 * 2023-12-01 l:682 f:OnRaiseProcessEvent add new action "Probing"
 * 2024-02-07 l:698 f:OnRaiseProcessEvent add Barcode, CreatText, 2D-View
 * 2024-02-12 split file, new  MainFormProcessAutomation.cs
 * 2025-06-03 add _tablet_form
*/

//using GrblPlotter.GCodeCreation.CreateFromForm;
using GrblPlotter.MachineControl;
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
        GCodeFromTablet _tablet_form = null;
        GCodeForWireCutter _wireCutter_form = null;

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
        ProcessAutomation _process_form = null;
        GrblSetupForm _grbl_setup_form = null;

        private void UpdateIniVariables()
        {
            _text_form?.UpdateIniVariables();
            _barcode_form?.UpdateIniVariables();
            _process_form?.UpdateIniVariables();
        }

        #region MAIN-MENU GCode creation

        /********************************************************************
         * Text
         * _wireCutter_form
         ********************************************************************/
        private void WireCutterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_wireCutter_form == null)
            {
                _wireCutter_form = new GCodeForWireCutter();
                _wireCutter_form.FormClosed += FormClosed_WireCutter;
                _wireCutter_form.btnApply.Click += GetGCodeForWireCutter;      // assign btn-click event
                EventCollector.SetOpenForm("Fwic");
            }
            else
            {
                _wireCutter_form.Visible = false;
            }

            if (showFormInFront) _wireCutter_form.Show(this);
            else _wireCutter_form.Show();  // this);

            showFormsToolStripMenuItem.Visible = true;
            _wireCutter_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_WireCutter(object sender, FormClosedEventArgs e)
        { _wireCutter_form = null; EventCollector.SetOpenForm("FCwic"); }

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
                EventCollector.SetOpenForm("Ftxt");
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
        { _text_form = null; EventCollector.SetOpenForm("FCtxt"); }

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
                EventCollector.SetOpenForm("Fimg");
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
        { _image_form = null; EventCollector.SetOpenForm("FCimg"); }

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
                EventCollector.SetOpenForm("Fsis");
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
        { _shape_form = null; EventCollector.SetOpenForm("FCsis"); }

        /********************************************************************
         * Direct control
         * _tablet_form
         ********************************************************************/
        private void DirectControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_tablet_form == null)
            {
                _tablet_form = new GCodeFromTablet();
                _tablet_form.RaiseCmdEvent += OnRaiseTabletCmdEvent;
                _tablet_form.FormClosed += FormClosed_Tablet;
                //    _tablet_form.BtnImport.Click += GetGCodeFromTablet;      // assign btn-click event
                _tablet_form.importWholeDrawingToolStripMenuItem.Click += GetGCodeFromTablet;
                EventCollector.SetOpenForm("Ftab");
            }
            else
            {
                _tablet_form.Visible = false;
            }

            if (showFormInFront) _tablet_form.Show(this);
            else _tablet_form.Show(); // this);

            showFormsToolStripMenuItem.Visible = true;
            _tablet_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_Tablet(object sender, FormClosedEventArgs e)
        { _tablet_form = null; EventCollector.SetOpenForm("FCtab"); }
        private void OnRaiseTabletCmdEvent(object sender, CmdEventArgs e)
        {
            if (e.Command.Contains("update"))
            {
                Logger.Trace("OnRaiseTabletCmdEvent update");
                NewCodeStart(false);
                SetFctbCodeText(_tablet_form.GetCode().ToString());
                NewCodeEnd();
                fCTBCode.Refresh();
                FoldBlocks1();
                return;
            }
            else if (e.Command.Contains("setup"))
            {
                SetupToolStripMenuItem_Click(sender, e);
                _setup_form?.ShowTab("setup");
            }
            else if (e.Command.Contains("tool"))
            {
                _setup_form?.UpdateToolTable();
            }
            else
            {
                string[] commands;
                commands = e.Command.Split(';');
                if (!_serial_form.SerialPortOpen)
                    return;
                foreach (string btncmd in commands)
                {
                    if (btncmd.Contains("stop"))
                    { if (!Grbl.isVersion_0) SendRealtimeCommand(133); }
                    else
                        SendCommand(btncmd.Trim());
                }
            }
        }
        private void GetGCodeFromTablet(object sender, EventArgs e)
        {
            if (!isStreaming)
            {
                string tmpCode = "(no gcode)";
                if (Graphic.GCode != null)
                {
                    tmpCode = Graphic.GCode.ToString();
                }
                InsertCodeFromForm(tmpCode, "from tablet");
                Properties.Settings.Default.counterImportText += 1;
                string source = "Iink";
                if (Properties.Settings.Default.fromFormInsertEnable)
                    source = "I" + source;
                AfterImport(source);
            }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

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
                EventCollector.SetOpenForm("Fbcd");
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
        { _barcode_form = null; EventCollector.SetOpenForm("FCbcd"); }

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
                EventCollector.SetOpenForm("F2nd");
            }
            else
            {
                _2ndGRBL_form.Visible = false;
            }
            _2ndGRBL_form.Show(this);
            _2ndGRBL_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_2ndGRBLForm(object sender, FormClosedEventArgs e)
        { _2ndGRBL_form = null; EventCollector.SetOpenForm("FC2nd"); }

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
                EventCollector.SetOpenForm("F3rd");
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
            EventCollector.SetOpenForm("FC3rd");
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
                _camera_form.RaiseProcessEvent += OnRaiseCameraProcessEvent;

                //               _camera_form.setPosMarker(grbl.posMarker);// visuGCode.GetPosMarker());
                EventCollector.SetOpenForm("Fcam");
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
        { _camera_form = null; EventCollector.SetOpenForm("FCcam"); }

        private void OnRaiseCameraProcessEvent(object sender, ProcessEventArgs e)
        {
            if (e.Command == "Fiducial")
            {
                _process_form?.Feedback("Camera Automatic", e.Value, (e.Value == "finished"));
            }
        }


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
                EventCollector.SetOpenForm("Fdiy");
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
        { _diyControlPad = null; EventCollector.SetOpenForm("FCdiy"); }

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
                    {
                        _diyControlPad?.SendFeedback("Error in parsing " + num, true);
                    }
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
                EventCollector.SetOpenForm("Fcord");
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
        { _coordSystem_form = null; EventCollector.SetOpenForm("FCcord"); }

        private void OnRaiseCoordSystemEvent(object sender, CmdEventArgs e)
        {
            SendCommand(e.Command); _serial_form.BringToFront();
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
                _laser_form.BtnMaterialTest.Click += GetGCodeFromLaser;      // assign btn-click event
                _laser_form.BtnMaterialSingleTest.Click += GetGCodeFromLaser;      // assign btn-click event
                EventCollector.SetOpenForm("Flas");
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
        { _laser_form = null; EventCollector.SetOpenForm("FClas"); }

        private void OnRaiseLaserEvent(object sender, CmdEventArgs e)
        {
            if (!_serial_form.RequestSend(e.Command, true))     // check if COM is still open
                UpdateControlEnables();
            Properties.Settings.Default.counterUseLaserSetup += 1;
        }

        private void GetGCodeFromLaser(object sender, EventArgs e)
        {
            if (!isStreaming)
            {
                ClearWorkspace();
                NewCodeStart(false);
                SetFctbCodeText(_laser_form.LaserGCode);
                NewCodeEnd();
                FoldBlocks1();
                foldLevelSelected = 1;
            }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"));
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
                _probing_form.RaiseProcessEvent += OnRaiseProbingProcessEvent;
                _probing_form.RaiseXYEvent += OnRaiseCameraClickEvent;
                EventCollector.SetOpenForm("Fprb");
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
        { _probing_form = null; EventCollector.SetOpenForm("FCprb"); }

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
            Properties.Settings.Default.counterUseProbing += 1;
        }

        private void OnRaiseProbingProcessEvent(object sender, ProcessEventArgs e)
        {
            if (e.Command == "Probe")
            {
                _process_form?.Feedback("Probe Automatic", e.Value, (e.Value == "finished"));
            }
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
                _heightmap_form.btnGCode.Click += GetGCodeFromHeightMap;                // in MainFormGetCodeTransform
                EventCollector.SetOpenForm("Fmap");
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
            EventCollector.SetOpenForm("FCmap");
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
                _setup_form.BtnApply2DViewChanges.Click += Update2DView;
                _setup_form.btnReloadFile.Click += ReStartConvertFileFromSetup;
                _setup_form.btnMoveToolXY.Click += MoveToPickup;
                _setup_form.btnGCPWMUp.Click += MoveToPickup;
                _setup_form.btnGCPWMDown.Click += MoveToPickup;
                _setup_form.btnGCPWMZero.Click += MoveToPickup;
                _setup_form.BtnSetGrblCustomString.Click += MoveToPickup;
                _setup_form.nUDImportGCPWMP93.ValueChanged += MoveToPickup;
                _setup_form.nUDImportGCPWMP94.ValueChanged += MoveToPickup;
                _setup_form.TbImportGCPWMSlider.ValueChanged += MoveToPickup;
                _setup_form.SetLastLoadedFile(lastLoadSource);
                gamePadTimer.Enabled = false;
                EventCollector.SetOpenForm("Fstp");
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
            EventCollector.SetOpenForm("FCstp");
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
                EventCollector.SetOpenForm("Fjog");
            }
            else
            {
                _jogPathCreator_form.Visible = false;
            }
            _jogPathCreator_form.Show(null);// this);
            _jogPathCreator_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_JogCreator(object sender, FormClosedEventArgs e)
        { _jogPathCreator_form = null; EventCollector.SetOpenForm("FCjog"); }


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
                EventCollector.SetOpenForm("Fprj");
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
                _projector_form.Location = Screen.AllScreens[(int)Properties.Settings.Default.projectorMonitorIndex].WorkingArea.Location;  // selectable index
                _projector_form.FormBorderStyle = FormBorderStyle.None;     // default = Sizable
                _projector_form.Show();
                _projector_form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                _projector_form.Show(this);
                _projector_form.WindowState = FormWindowState.Normal;
            }
        }
        private void FormClosed_Projector(object sender, FormClosedEventArgs e)
        { _projector_form = null; EventCollector.SetOpenForm("FCprj"); }


        /********************************************************************
         * GRBL Setup
         * _grbl_setup_form
         ********************************************************************/
        private void GrblSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_grbl_setup_form == null)
            {
                _grbl_setup_form = new GrblSetupForm();
                _grbl_setup_form.FormClosed += FormClosed_GrblSetup;
                _grbl_setup_form.RaiseCmdEvent += OnRaiseCoordSystemEvent;  // line 389
                EventCollector.SetOpenForm("Fgrbl");
            }
            else
            {
                _grbl_setup_form.Visible = false;
            }
            _grbl_setup_form.Show(null);// this);
            _grbl_setup_form.WindowState = FormWindowState.Normal;
        }
        private void FormClosed_GrblSetup(object sender, FormClosedEventArgs e)
        { _grbl_setup_form = null; EventCollector.SetOpenForm("FCgrbl"); }


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
