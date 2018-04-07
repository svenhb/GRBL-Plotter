/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

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
/*  2016-09-18  improve performance for low-performance PC: during streaming show background-image with toolpath
 *              instead of redrawing toolpath with each onPaint.
 *              Joystick-control: adjustable step-width and speed.
 *  2016-12-31  Add GRBL 1.1 function
 *  2017-01-01  check form-location and fix strange location
 *  2017-01-03  Add 'Replace M3 by M4' during GCode file open
 *  2017-06-22  Cleanup transform actions
 *  2018-01-02  Add Override Buttons
 *              Bugfix - no zooming during  streaming - disable background image (XP Problems?)
 *  2018-03-18  Divide this file into several files
 */

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using virtualJoystick;
using System.Globalization;
using System.Threading;
using System.Text;

namespace GRBL_Plotter
{

    public partial class MainForm : Form
    {
        ControlSerialForm _serial_form = null;
        ControlSerialForm _serial_form2 = null;
        Control2ndGRBL _2ndGRBL_form = null;
        ControlStreamingForm _streaming_form = null;
        ControlStreamingForm2 _streaming_form2 = null;
        ControlCameraForm _camera_form = null;
        ControlSetupForm _setup_form = null;
        ControlHeightMapForm _heightmap_form = null;
        GCodeFromText _text_form = null;
        GCodeFromImage _image_form = null;
        GCodeFromShape _shape_form = null;

        private const string appName = "GRBL Plotter";
        private xyzPoint posMachine = new xyzPoint(0, 0, 0);
        private xyzPoint posWorld = new xyzPoint(0, 0, 0);
        private xyzPoint posProbe = new xyzPoint(0, 0, 0);
        private grblState machineStatus;
        public bool flagResetOffset = false;
        private double[] joystickXYStep = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickZStep = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickXYSpeed = { 0, 1, 2, 3, 4, 5 };
        private double[] joystickZSpeed = { 0, 1, 2, 3, 4, 5 };

        private bool ctrl4thAxis = false;
        private string ctrl4thName = "A";
        private string lastLoadSource = "Nothing loaded";
        private int coordinateG = 54;

        public MainForm()
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            gcode.setup();
            updateDrawing();
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Application_UnhandledException);
        }
        //Unhandled exception
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            MessageBox.Show(ex.Message, "Thread exception");
        }
        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show(ex.Message, "Application exception");
            }
        }

        // initialize Main form
        private void MainForm_Load(object sender, EventArgs e)
        {
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            Location = Properties.Settings.Default.locationMForm;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }
            this.Text = appName + " Ver " + System.Windows.Forms.Application.ProductVersion.ToString(); // Application.ProductVersion.ToString();    //Application.ProductVersion;

            if (_serial_form == null)
            {
                if (Properties.Settings.Default.useSerial2)
                {
                    _serial_form2 = new ControlSerialForm("COM Tool changer", 2);
                    _serial_form2.Show(this);
                }
                _serial_form = new ControlSerialForm("COM CNC", 1, _serial_form2);
                _serial_form.Show(this);
                _serial_form.RaisePosEvent += OnRaisePosEvent;
                _serial_form.RaiseStreamEvent += OnRaiseStreamEvent;
            }
            lbDimension.Select(0, 0);
            loadSettings(sender, e);
            updateControls();
            LoadRecentList();
            foreach (string item in MRUlist)
            {
                ToolStripMenuItem fileRecent = new ToolStripMenuItem(item, null, RecentFile_click);  //create new menu for each item in list
                toolStripMenuItem2.DropDownItems.Add(fileRecent); //add the menu to "recent" menu
            }

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            { loadFile(args[1]); }

            checkUpdate.CheckVersion();  // check update
        }
        // close Main form
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationMForm = Location;
            saveSettings();
            if (_2ndGRBL_form != null) _2ndGRBL_form.Close();
            if (_heightmap_form != null) _heightmap_form.Close();
            if (_setup_form != null) _setup_form.Close();
            if (_camera_form != null) _camera_form.Close();
            if (_streaming_form != null) _streaming_form.Close();
            if (_heightmap_form != null) _heightmap_form.Close();
            _serial_form.Close();
        }

        // handle position events from serial form
        private void OnRaisePosEvent(object sender, PosEventArgs e)
        {
            posWorld = e.PosWorld;
            posMachine = e.PosMachine;
            machineStatus = e.Status;
            if (e.StatMsg.Ov.Length > 1)    // check and submit override values
            {
                if (_streaming_form2 != null)
                { _streaming_form2.showOverrideValues(e.StatMsg.Ov); }

                string[] value = e.StatMsg.Ov.Split(',');
                if (value.Length > 2)
                {
                    lblOverrideFRValue.Text = value[0];
                    lblOverrideSSValue.Text = value[2];
                }
            }
            if (e.StatMsg.FS.Length > 1)    // check and submit override values
            {
                if (_streaming_form2 != null)
                    _streaming_form2.showActualValues(e.StatMsg.FS);
            }
            if (e.Status == grblState.probe)
            { posProbe = _serial_form.posProbe;
                if (_heightmap_form != null)
                    _heightmap_form.setPosProbe = posProbe;
            }

            label_mx.Text = string.Format("{0:0.000}", posMachine.X);
            label_my.Text = string.Format("{0:0.000}", posMachine.Y);
            label_mz.Text = string.Format("{0:0.000}", posMachine.Z);
            label_ma.Text = string.Format("{0:0.000}", posMachine.A);
            label_wx.Text = string.Format("{0:0.000}", posWorld.X);
            label_wy.Text = string.Format("{0:0.000}", posWorld.Y);
            label_wz.Text = string.Format("{0:0.000}", posWorld.Z);
            label_wa.Text = string.Format("{0:0.000}", posWorld.A);
  //          visuGCode.setPosTool(posWorld.X, posWorld.Y, posWorld.Z);
            visuGCode.setPosTool(posWorld);
            if (flagResetOffset)
            {
                double x = Properties.Settings.Default.lastOffsetX;
                double y = Properties.Settings.Default.lastOffsetY;
                double z = Properties.Settings.Default.lastOffsetZ;
                double a = Properties.Settings.Default.lastOffsetA;

                coordinateG = Properties.Settings.Default.lastOffsetCoord;
                _serial_form.addToLog("Restore saved position after reset\r\nand set initial feed rate:");
                sendCommand(String.Format("G{0}", coordinateG));

                if (ctrl4thAxis)
                    sendCommand(String.Format("G92 X{0} Y{1} Z{2} {3}{4}F{5}", x, y, z, ctrl4thName, a, Properties.Settings.Default.importGCXYFeed).Replace(',', '.'));
                else
                    sendCommand(String.Format("G92 X{0} Y{1} Z{2} F10", x, y, z).Replace(',', '.'));
                flagResetOffset = false;
                updateControls();
            }
            if (_camera_form != null)
            {
                _camera_form.setPosWorld = posWorld;
                _camera_form.setPosMachine = posMachine;
            }
            if (_heightmap_form != null)
            {
                _heightmap_form.setPosWorld = posWorld;
                _heightmap_form.setPosMachine = posMachine;
            }
            processStatus();
            processParserState(e.parserState);
            visuGCode.createMarkerPath();
            pictureBox1.Invalidate();
        }
        // handle status events from serial form
        private grblState lastMachineStatus = grblState.unknown;
        private string lastInfoText = "";
        private string lastLabelInfoText = "";
        private bool updateDrawingPath = false;
        private void processStatus() // {idle, run, hold, home, alarm, check, door}
        {
            if (machineStatus != lastMachineStatus)
            {
                label_status.Text = grbl.statusToText(machineStatus);
                label_status.BackColor = grbl.grblStateColor(machineStatus);
                switch (machineStatus)
                {
                    case grblState.idle:
                        if ((lastMachineStatus == grblState.hold) || (lastMachineStatus == grblState.alarm))
                        {
                            lbInfo.Text = lastInfoText;
                            lbInfo.BackColor = SystemColors.Control;
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        cBTool.Checked = _serial_form.toolInSpindle;
                        if (signalLock > 0)
                        {   btnKillAlarm.BackColor = SystemColors.Control; signalLock = 0; }
                        if (!isStreaming)                       // update drawing if G91 is used
                            updateDrawingPath = true;
                        break;
                    case grblState.run:
                        if (lastMachineStatus == grblState.hold)
                        {
                            lbInfo.Text = lastInfoText;
                            lbInfo.BackColor = SystemColors.Control;
                        }
                        signalResume = 0;
                        btnResume.BackColor = SystemColors.Control;
                        break;
                    case grblState.hold:
                        btnResume.BackColor = Color.Yellow;
                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = "Press 'Resume' to proceed";
                        lbInfo.BackColor = Color.Yellow;
                        if (signalResume == 0) { signalResume = 1; }
                        break;
                    case grblState.home:
                        break;
                    case grblState.alarm:
                        signalLock = 1;
                        btnKillAlarm.BackColor = Color.Yellow;
                        lbInfo.Text = "Press 'Kill Alarm' to proceed";
                        lbInfo.BackColor = Color.Yellow;
                        if (_heightmap_form != null)
                            _heightmap_form.stopScan();
                        break;
                    case grblState.check:
                        break;
                    case grblState.door:
                        break;
                    case grblState.probe:
                        lastInfoText = lbInfo.Text;
                        lbInfo.Text = string.Format("Probing: Z={0:0.000}", posProbe.Z);
                        lbInfo.BackColor = Color.Yellow;
                        break;
                    default:
                        break;
                }
            }
            lastMachineStatus = machineStatus;
        }

        // handle last sent commands from serial form
        private string actualFR = "";
        private string actualSS = "";
        private void processParserState(pState cmd)//string cmd)
        {   if (cmd.changed)
            {
                actualFR = cmd.FR.ToString();
                if (_streaming_form != null)
                    _streaming_form.show_value_FR(actualFR);
                actualSS = cmd.SS.ToString();
                if (_streaming_form != null)
                    _streaming_form.show_value_SS(actualSS);

                cBSpindle.Checked = (cmd.spindle <= 4) ? true : false;
                cBCoolant.Checked = (cmd.coolant <= 8) ? true : false;

                if (cmd.toolchange)
                    lblTool.Text = cmd.tool.ToString();

                lblCurrentG.Text = "G" + cmd.coord_select.ToString();
                lblCurrentG.BackColor = (cmd.coord_select == 54) ? Color.Lime : Color.Fuchsia;
                if (_camera_form != null)
                    _camera_form.setCoordG = cmd.coord_select;
            }
        }

        // update drawing on Main form and enable / disable 
        private void updateDrawing()
        {
            visuGCode.createImagePath();                                // show initial empty picture . just ruler and tool-pos
            pictureBox1.Invalidate();                                   // resfresh view
            if (visuGCode.containsG2G3Command())                        // disable X/Y independend scaling if G2 or G3 GCode is in use
            {                                                           // because it's not possible to stretch (convert 1st to G1 GCode)                skaliereXUmToolStripMenuItem.Enabled = false;
                skaliereAufXUnitsToolStripMenuItem.Enabled = false;
                skaliereYUmToolStripMenuItem.Enabled = false;
                skaliereAufYUnitsToolStripMenuItem.Enabled = false;
                skaliereXAufDrehachseToolStripMenuItem.Enabled = false;
                skaliereYAufDrehachseToolStripMenuItem.Enabled = false;
                ersetzteG23DurchLinienToolStripMenuItem.Enabled = true;
            }
            else
            {
                skaliereXUmToolStripMenuItem.Enabled = true;                // enable X/Y independend scaling because no G2 or G3 GCode
                skaliereAufXUnitsToolStripMenuItem.Enabled = true;
                skaliereYUmToolStripMenuItem.Enabled = true;
                skaliereAufYUnitsToolStripMenuItem.Enabled = true;
                skaliereXAufDrehachseToolStripMenuItem.Enabled = true;
                skaliereYAufDrehachseToolStripMenuItem.Enabled = true;
                ersetzteG23DurchLinienToolStripMenuItem.Enabled = false;
            }
        }

        // send command via serial form
        private void sendRealtimeCommand(int cmd)
        { _serial_form.realtimeCommand(cmd); }

        // send command via serial form
        private void sendCommand(string txt, bool jogging = false)
        { if ((jogging) && (_serial_form.isGrblVers0 == false))
                txt = "$J=" + txt;
            _serial_form.requestSend(txt);
        }

        private void OnRaiseOverrideMessage(object sender, OverrideMsgArgs e)
        { sendRealtimeCommand(e.MSG); }

        // get override events from form "StreamingForm" for GRBL 0.9
        private string overrideMessage = "";
        private void OnRaiseOverrideEvent(object sender, OverrideEventArgs e)
        { if (e.Source == overrideSource.feedRate)
                _serial_form.injectCode("F", (int)e.Value, e.Enable);
            if (e.Source == overrideSource.spindleSpeed)
                _serial_form.injectCode("S", (int)e.Value, e.Enable);

            overrideMessage = "";
            if (e.Enable)
                overrideMessage = " !!! Override !!!";
            lbInfo.Text = lastLabelInfoText + overrideMessage;
        }

        // Main-Menu File outsourced to MaiFormLoadFile.cs

        #region MAIN-MENU GCode creation
        // open text creation form
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
        }
        private void formClosed_TextToGCode(object sender, FormClosedEventArgs e)
        { _text_form = null; }

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
        }
        private void formClosed_ImageToGCode(object sender, FormClosedEventArgs e)
        { _image_form = null; }

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
        }
        private void formClosed_ShapeToGCode(object sender, FormClosedEventArgs e)
        { _shape_form = null; }
        #endregion

        #region MAIN-MENU GCode Transform
        private void mirrorXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            pBoxTransform.Reset();
            fCTBCode.Text = visuGCode.transformGCodeMirror(GCodeVisuAndTransform.translate.MirrorX);
            Cursor.Current = Cursors.Default;
            updateGUI();
        }

        private void mirrorYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            pBoxTransform.Reset();
            fCTBCode.Text = visuGCode.transformGCodeMirror(GCodeVisuAndTransform.translate.MirrorY);
            Cursor.Current = Cursors.Default;
            updateGUI();
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            pBoxTransform.Reset();
            fCTBCode.Text = visuGCode.transformGCodeRotate(90, 1, new xyPoint(0,0));
            Cursor.Current = Cursors.Default;
            updateGUI();
        }

        private void rotate90ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            pBoxTransform.Reset();
            fCTBCode.Text = visuGCode.transformGCodeRotate(-90, 1, new xyPoint(0, 0));
            Cursor.Current = Cursors.Default;
            updateGUI();
        }

        private void toolStrip_tb_rotate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double anglenew;
                if (Double.TryParse(toolStrip_tb_rotate.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out anglenew))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    pBoxTransform.Reset();
                    fCTBCode.Text = visuGCode.transformGCodeRotate(anglenew, 1, new xyPoint(0, 0));
                    Cursor.Current = Cursors.Default;
                    updateGUI();
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_rotate.Text = "0.0";
                }
            }
        }

        private void toolStrip_tb_XY_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double size;
                if (Double.TryParse(toolStrip_tb_XY_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out size))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    fCTBCode.Cursor = Cursors.WaitCursor;
                    pBoxTransform.Reset();
                    fCTBCode.Text = visuGCode.transformGCodeScale(size, size);
                    updateGUI();
                    fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                    fCTBCodeMarkLine();
                    fCTBCode.Cursor = Cursors.IBeam;
                    Cursor.Current = Cursors.Default;
                    showChangedMessage = true;
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_XY_scale.Text = "100.00";
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_XY_X_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = visuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_XY_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    toolStrip_tb_XY_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_XY_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_XY_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = visuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_XY_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    toolStrip_tb_XY_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_XY_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_X_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double size;
                if (Double.TryParse(toolStrip_tb_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out size))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    fCTBCode.Cursor = Cursors.WaitCursor;
                    pBoxTransform.Reset();
                    fCTBCode.Text = visuGCode.transformGCodeScale(size, 100);
                    updateGUI();
                    fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                    fCTBCodeMarkLine();
                    fCTBCode.Cursor = Cursors.IBeam;
                    Cursor.Current = Cursors.Default;
                    showChangedMessage = true;
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_X_scale.Text = "100.00";
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_X_X_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = visuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_X_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    if (Properties.Settings.Default.rotarySubstitutionEnable)
                    {
                        double length = (float)Properties.Settings.Default.rotarySubstitutionDiameter * Math.PI;
                        sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / length;
                    }
                    toolStrip_tb_X_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_X_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_X_X_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_X_A_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = visuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_X_A_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / 360;
                    toolStrip_tb_X_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_X_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_X_A_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double size;
                if (Double.TryParse(toolStrip_tb_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out size))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    fCTBCode.Cursor = Cursors.WaitCursor;
                    pBoxTransform.Reset();
                    fCTBCode.Text = visuGCode.transformGCodeScale(100, size);
                    updateGUI();
                    fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                    fCTBCodeMarkLine();
                    fCTBCode.Cursor = Cursors.IBeam;
                    Cursor.Current = Cursors.Default;
                    showChangedMessage = true;
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_Y_scale.Text = "100.00";
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_Y_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = visuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_Y_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    if (Properties.Settings.Default.rotarySubstitutionEnable)
                    {
                        double length = (float)Properties.Settings.Default.rotarySubstitutionDiameter * Math.PI;
                        sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / length;
                    }
                    toolStrip_tb_Y_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_Y_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_Y_A_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = visuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_Y_A_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / 360;
                    toolStrip_tb_Y_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_Y_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_Y_A_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_rotary_diameter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = visuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_rotary_diameter.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    Properties.Settings.Default.rotarySubstitutionDiameter = (decimal)sizenew;
                    string tmp = string.Format("Calculating rotary angle depending on part diameter ({0:0.00} units) and desired size.\r\nSet part diameter in Setup - Control.", Properties.Settings.Default.rotarySubstitutionDiameter);
                    skaliereAufXUnitsToolStripMenuItem.ToolTipText = tmp;
                    skaliereAufYUnitsToolStripMenuItem.ToolTipText = tmp;
                }
                else
                {
                    MessageBox.Show("Not a valid number", "Attention");
                    toolStrip_tb_rotary_diameter.Text = string.Format("{0:0.00}", Properties.Settings.Default.rotarySubstitutionDiameter);
                }
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ersetzteG23DurchLinienToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            pBoxTransform.Reset();
            fCTBCode.Text = visuGCode.replaceG23();
            Cursor.Current = Cursors.Default;
            updateGUI();
        }

        private void updateGUI()
        {
            updateDrawing();
            lbDimension.Text = visuGCode.xyzSize.getMinMaxString(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
            lbDimension.Select(0, 0);
            toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
            toolStrip_tb_X_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
            toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
            toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
        }
        #endregion

        #region MAIN-MENU Machine control
        private void controlStreamingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_serial_form.isGrblVers0)
            {
                if (_streaming_form2 != null)
                    _streaming_form2.Visible = false;
                if (_streaming_form == null)
                {
                    _streaming_form = new ControlStreamingForm();
                    _streaming_form.RaiseOverrideEvent += OnRaiseOverrideEvent;      // assign  event
                    _streaming_form.show_value_FR(actualFR);
                    _streaming_form.show_value_SS(actualSS);
                }
                else
                {
                    _streaming_form.Visible = false;
                }
                _streaming_form.Show(this);
            }
            else
            {
                if (_streaming_form != null)
                    _streaming_form.Visible = false;
                if (_streaming_form2 == null)
                {
                    _streaming_form2 = new ControlStreamingForm2();
                    _streaming_form2.RaiseOverrideEvent += OnRaiseOverrideMessage;      // assign  event
                }
                else
                {
                    _streaming_form2.Visible = false;
                }
                _streaming_form2.Show(this);
            }
        }
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
        }
        private void formClosed_2ndGRBLForm(object sender, FormClosedEventArgs e)
        { _2ndGRBL_form = null; }
        // open Camera form
        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_camera_form == null)
            {
                _camera_form = new ControlCameraForm();
                _camera_form.FormClosed += formClosed_CameraForm;
                _camera_form.RaiseXYEvent += OnRaiseCameraClickEvent;
                _camera_form.setPosMarker(visuGCode.GetPosMarker());
            }
            else
            {
                _camera_form.Visible = false;
            }
            _camera_form.Show(this);
        }
        private void formClosed_CameraForm(object sender, FormClosedEventArgs e)
        { _camera_form = null; }
        // Height Map
        private void heightMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_heightmap_form == null)
            {
                _heightmap_form = new ControlHeightMapForm();
                _heightmap_form.FormClosed += formClosed_HeightmapForm;
                _heightmap_form.btnStartHeightScan.Click += getGCodeFromHeightMap;      // assign btn-click event
                _heightmap_form.loadHeightMapToolStripMenuItem.Click += loadHeightMap;
                _heightmap_form.btnApply.Click += applyHeightMap;
                //                _heightmap_form.btnApply.Click += applyHeightMap;
                _heightmap_form.RaiseXYZEvent += OnRaisePositionClickEvent;

            }
            else
            {
                _heightmap_form.Visible = false;
            }
            _heightmap_form.Show(this);
        }
        private void formClosed_HeightmapForm(object sender, FormClosedEventArgs e)
        { _heightmap_form = null;
            GCodeVisuAndTransform.clearHeightMap();
            _serial_form.stopStreaming();
        }

        // open Setup form
        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_setup_form == null)
            {
                _setup_form = new ControlSetupForm();
                _setup_form.FormClosed += formClosed_SetupForm;
                _setup_form.btnApplyChangings.Click += loadSettings;
                _setup_form.btnReloadFile.Click += reStartConvertSVG;
                _setup_form.setLastLoadedFile(lastLoadSource);
                gamePadTimer.Enabled = false;
            }
            else
            {
                _setup_form.Visible = false;
            }
            _setup_form.Show(this);
        }
        private void formClosed_SetupForm(object sender, FormClosedEventArgs e)
        {   loadSettings(sender, e);
            _setup_form = null;
            updateDrawing();
            gamePadTimer.Enabled = Properties.Settings.Default.gPEnable;
        }
        #endregion
        // open About form
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }

        private void showLaserMode()
        {
            if (!_serial_form.isGrblVers0 && _serial_form.isLasermode)
            {
                lbInfo.Text = "Laser Mode active $32=1";
                lbInfo.BackColor = Color.Fuchsia;
            }
            else
            {
                lbInfo.Text = "Laser Mode not active $32=0";
                lbInfo.BackColor = Color.Lime;
            }
        }

        #region Streaming
        // handle file streaming
        TimeSpan elapsed;               //elapsed time from file burnin
        DateTime timeInit;              //time start to burning file
        private int signalResume = 0;   // blinking button
        private int signalLock = 0;     // blinking button
        private int signalPlay = 0;     // blinking button
        private bool isStreaming = false;
        private bool isStreamingPause = false;
        private bool isStreamingCheck = false;
        private bool isStreamingOk = true;
        private void OnRaiseStreamEvent(object sender, StreamEventArgs e)
        {
            int cPrgs = (int)Math.Max(0, Math.Min(100, e.CodeProgress));
            int bPrgs = (int)Math.Max(0, Math.Min(100, e.BuffProgress));
            pbFile.Value = cPrgs;
            pbBuffer.Value = bPrgs;
            lblFileProgress.Text = string.Format("Progress {0:0.0}%", e.CodeProgress);
            fCTBCode.Selection = fCTBCode.GetLine(e.CodeLine);
            fCTBCodeClickedLineNow = e.CodeLine - 1;
            fCTBCodeMarkLine();
            fCTBCode.DoCaretVisible();

            if (e.Status == grblStreaming.lasermode)
            {
                showLaserMode();
            }
            if (e.Status == grblStreaming.reset)
            {
                flagResetOffset = true;
                isStreaming = false;
                isStreamingCheck = false;
                lbInfo.Text = "Vers. "+_serial_form.grblVers;
                lbInfo.BackColor = Color.Lime;
                updateControls();
            }
            if (e.Status == grblStreaming.error)
            {
                pbFile.ForeColor = Color.Red;
                lbInfo.Text = "Error before line " + e.CodeLine.ToString();
                lbInfo.BackColor = Color.Fuchsia;
                fCTBCode.BookmarkLine(e.CodeLine - 1);
                fCTBCode.DoSelectionVisible();
                fCTBCode.CurrentLineColor = Color.Red;
                isStreamingOk = false;
            }
            if ((e.Status == grblStreaming.ok) && !isStreamingCheck)
            {
                updateControls();
                lbInfo.Text = "Send G-Code (" + e.CodeLine.ToString() + ")";
                lbInfo.BackColor = Color.Lime;
                signalPlay = 0;
                btnStreamStart.BackColor = SystemColors.Control;
                //                btnStreamPause.BackColor = SystemColors.Control; 
            }
            if (e.Status == grblStreaming.finish)
            {
                if (isStreamingOk)
                {
                    if (isStreamingCheck)
                    { lbInfo.Text = "Finish checking G-Code"; }
                    else
                    { lbInfo.Text = "Finish sending G-Code"; }
                    lbInfo.BackColor = Color.Lime;
                    pbFile.Value = 0;
                    pbBuffer.Value = 0;
                }
                isStreaming = false; isStreamingCheck = false;
                btnStreamStart.Image = Properties.Resources.btn_play;
                btnStreamStart.Enabled = true;
                btnStreamCheck.Enabled = true;
                picBoxCopy = 0;                     // don't show background image anymore
                pictureBox1.BackgroundImage = null;
                updateControls();
            }
            if (e.Status == grblStreaming.waitidle)
            {
                updateControls(true);
                btnStreamStart.Image = Properties.Resources.btn_play;
                isStreamingPause = true;
                lbInfo.Text = "Wait for IDLE, then pause (" + e.CodeLine.ToString() + ")";
                lbInfo.BackColor = Color.Yellow;
            }
            if (e.Status == grblStreaming.pause)
            {
                updateControls(true);
                btnStreamStart.Image = Properties.Resources.btn_play;
                isStreamingPause = true;
                lbInfo.Text = "Pause streaming - press play (" + e.CodeLine.ToString() + ")";
                signalPlay = 1;
                lbInfo.BackColor = Color.Yellow;
            }
            if (e.Status == grblStreaming.toolchange)
            {
                updateControls();
                btnStreamStart.Image = Properties.Resources.btn_play;
                lbInfo.Text = "Tool change...";
                lbInfo.BackColor = Color.Yellow;
                cBTool.Checked = _serial_form.toolInSpindle;
            }

            if (e.Status == grblStreaming.stop)
            {
                lbInfo.Text = " STOP streaming (" + e.CodeLine.ToString() + ")";
                lbInfo.BackColor = Color.Fuchsia;
            }
            lastLabelInfoText = lbInfo.Text;
            lbInfo.Text += overrideMessage;
        }
        private void btnStreamStart_Click(object sender, EventArgs e)
        {
            if (fCTBCode.LinesCount > 1)
            {
                if (!isStreaming)
                {
                    if (_streaming_form != null)
                    {
                        //                        _streaming_form.cBOverrideFREnable.Checked = false;
                        //                        _streaming_form.cBOverrideSSEnable.Checked = false;
                    }
                    isStreaming = true;
                    isStreamingPause = false;
                    isStreamingCheck = false;
                    isStreamingOk = true;
                    updateControls();
                    timeInit = DateTime.UtcNow;
                    elapsed = TimeSpan.Zero;
                    lbInfo.Text = "Send G-Code";
                    lbInfo.BackColor = Color.Lime;
                    for (int i = 0; i < fCTBCode.LinesCount; i++)
                        fCTBCode.UnbookmarkLine(i);
                    lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
                    _serial_form.startStreaming(fCTBCode.Lines);
                    btnStreamStart.Image = Properties.Resources.btn_pause;
                    btnStreamCheck.Enabled = false;
                    onPaint_setBackground();
                }
                else
                {
                    if (!isStreamingPause)
                    {
                        btnStreamStart.Image = Properties.Resources.btn_play;
                        _serial_form.pauseStreaming();
                        isStreamingPause = true;
                    }
                    else
                    {
                        btnStreamStart.Image = Properties.Resources.btn_pause;
                        _serial_form.pauseStreaming();
                        isStreamingPause = false;
                    }
                }
            }
        }
        private void btnStreamCheck_Click(object sender, EventArgs e)
        { if ((fCTBCode.LinesCount > 1) && (!isStreaming))
            {
                isStreaming = true;
                isStreamingCheck = true;
                isStreamingOk = true;
                updateControls();
                timeInit = DateTime.UtcNow;
                elapsed = TimeSpan.Zero;
                lbInfo.Text = "Check G-Code";
                lbInfo.BackColor = SystemColors.Control;
                for (int i = 0; i < fCTBCode.LinesCount; i++)
                    fCTBCode.UnbookmarkLine(i);
                _serial_form.startStreaming(fCTBCode.Lines, true);
                btnStreamStart.Enabled = false;
                onPaint_setBackground();
            }
        }
        private void btnStreamStop_Click(object sender, EventArgs e)
        {
            picBoxCopy = 0;                 // don't show background image anymore
            pictureBox1.BackgroundImage = null;
            btnStreamStart.Image = Properties.Resources.btn_play;
            btnStreamStart.BackColor = SystemColors.Control;
            btnStreamStart.Enabled = true;
            btnStreamCheck.Enabled = true;
            _serial_form.stopStreaming();
            if (isStreaming || isStreamingCheck)
            {
                lbInfo.Text = " STOP streaming (" + (fCTBCodeClickedLineNow + 1).ToString() + ")";
                lbInfo.BackColor = Color.Fuchsia;
            }
            isStreaming = false;
            isStreamingCheck = false;
            pbFile.Value = 0;
            pbBuffer.Value = 0;
            signalPlay = 0;

            updateControls();
        }
        private void btnStreamPause_Click(object sender, EventArgs e)
        { _serial_form.pauseStreaming(); }
        #endregion

        // handle event from create Height Map form
        private void getGCodeFromHeightMap(object sender, EventArgs e)
        {
            if (!isStreaming && _serial_form.serialPortOpen)
            { if (_heightmap_form.scanStarted)
                {
                    string[] commands = _heightmap_form.getCode.ToString().Split('\r');
                    _serial_form.isHeightProbing = true;
                    foreach (string cmd in commands)
                        sendCommand(cmd);
                    visuGCode.drawHeightMap(_heightmap_form.Map);
                }
                else
                {
                    _serial_form.stopStreaming();
                }
                isHeightMapApplied = false;
            }
        }
        private void loadHeightMap(object sender, EventArgs e)
        {
            visuGCode.drawHeightMap(_heightmap_form.Map);
            visuGCode.createMarkerPath();
            visuGCode.createImagePath();
            pictureBox1.BackgroundImage = null;
            pictureBox1.Invalidate();
            isHeightMapApplied = false;
        }

        private bool isHeightMapApplied = false;
        private StringBuilder codeBeforeHeightMap = new StringBuilder();
        private void applyHeightMap(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (!isHeightMapApplied)
            {
                codeBeforeHeightMap.Clear();
                foreach (string codeline in fCTBCode.Lines)
                { if (codeline.Length > 0)
                        codeBeforeHeightMap.AppendLine(codeline);
                }
                visuGCode.getGCodeLines(fCTBCode.Lines);
                fCTBCode.Text = visuGCode.applyHeightMap(fCTBCode.Lines, _heightmap_form.Map);
                updateGUI();
                _heightmap_form.setBtnApply(isHeightMapApplied);
                isHeightMapApplied = true;
            }
            else
            {
                fCTBCode.Text = codeBeforeHeightMap.ToString();
                updateGUI();
                _heightmap_form.setBtnApply(isHeightMapApplied);
                isHeightMapApplied = false;
            }
            Cursor.Current = Cursors.Default;
        }


        // handle event from create Text form
        private void getGCodeFromText(object sender, EventArgs e)
        { if (!isStreaming)
            {
                picBoxCopy = 0;                 // don't show background image anymore
                pictureBox1.BackgroundImage = null;
                fCTBCode.Text = _text_form.textGCode;
                redrawGCodePath();
                updateControls();
            }
        }
        // handle event from create Text form
        private void getGCodeFromShape(object sender, EventArgs e)
        {
            if (!isStreaming)
            {
                picBoxCopy = 0;                     // don't show background image anymore
                pictureBox1.BackgroundImage = null;
                fCTBCode.Text = _shape_form.shapeGCode;
                redrawGCodePath();
                updateControls();
            }
        }
        // handle event from create Image form
        private void getGCodeFromImage(object sender, EventArgs e)
        { if (!isStreaming)
            {
                picBoxCopy = 0;                 // don't show background image anymore
                pictureBox1.BackgroundImage = null;
                fCTBCode.Text = _image_form.imageGCode;
                redrawGCodePath();
                updateControls();
            }
        }
        // update 500ms
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (isStreaming)
            {
                elapsed = DateTime.UtcNow - timeInit;
                lblElapsed.Text = "Time " + elapsed.ToString(@"hh\:mm\:ss");
            }
            else
            { if (updateDrawingPath && visuGCode.containsG91Command())
                { redrawGCodePath();
                    pictureBox1.Invalidate(); // will be called by parent function
                }
                updateDrawingPath = false;
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
        }

        // handle positon click event from camera form
        private void OnRaisePositionClickEvent(object sender, XYZEventArgs e)
        {
            if (e.Command.IndexOf("G91") >= 0)
            {
                string final = e.Command;
                if (e.PosX != null)
                    final += string.Format(" X{0}", gcode.frmtNum((float)e.PosX));
                if (e.PosY != null)
                    final += string.Format(" Y{0}", gcode.frmtNum((float)e.PosY));
                if (e.PosZ != null)
                    final += string.Format(" Z{0}", gcode.frmtNum((float)e.PosZ));
                sendCommand(final.Replace(',', '.'), true);
            }
        }
        private void OnRaiseCameraClickEvent(object sender, XYEventArgs e)
        {
            if (e.Command == "a")
            {   if (fCTBCode.LinesCount > 1)
                {
                    routeTransformCode(e.Angle, e.Scale, e.Point);
                    visuGCode.setPosMarkerLine(fCTBCodeClickedLineNow);
                }
            }
           else
            {
                double realStepX = Math.Round(e.Point.X, 3);
                double realStepY = Math.Round(e.Point.Y, 3);
                int speed = 1000;
                string s = "";
                string[] line = e.Command.Split(';');
                foreach (string cmd in line)
                {
                    if (cmd.Trim() == "G92")
                    {   s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        sendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G0") >= 0) || (cmd.Trim().IndexOf("G1") >= 0))        // no jogging
                    {   s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        sendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G90") == 0) || (cmd.Trim().IndexOf("G91") == 0))      // no G0 G1, then jogging
                    {   speed = 100 + (int)Math.Sqrt(realStepX * realStepX + realStepY * realStepY) * 120;
                        s = String.Format("F{0} " + cmd + " X{1} Y{2}", speed, realStepX, realStepY).Replace(',', '.');
                        sendCommand(s, true);
                    }
                    else
                    {
                        sendCommand(cmd.Trim());
                    }
                }
            }
        }
        public void routeTransformCode(double angle, double scale, xyPoint offset)
        {
            fCTBCode.Text = visuGCode.transformGCodeRotate(angle, scale, offset);
            updateGUI();
            return;
        }


        #region GUI Objects

        private void btnOffsetApply_Click(object sender, EventArgs e)
        {
            //groupBox6.Width += 20 ;
            Cursor.Current = Cursors.WaitCursor;
            pBoxTransform.Reset();
            double offsetx = 0, offsety = 0;
            if (!Double.TryParse(tbOffsetX.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out offsetx))
            {
                MessageBox.Show("Not a valid number", "Attention");
                tbOffsetX.Text = string.Format("{0:0.00}", offsetx);
            }
            if (!Double.TryParse(tbOffsetY.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out offsety))
            {
                MessageBox.Show("Not a valid number", "Attention");
                tbOffsetY.Text = string.Format("{0:0.00}", offsety);
            }
            if (fCTBCode.Lines.Count > 1)
            {
                fCTBCode.Cursor = Cursors.WaitCursor;
                if (rBOrigin1.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset1); }
                if (rBOrigin2.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset2); }
                if (rBOrigin3.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset3); }
                if (rBOrigin4.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset4); }
                if (rBOrigin5.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset5); }
                if (rBOrigin6.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset6); }
                if (rBOrigin7.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset7); }
                if (rBOrigin8.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset8); }
                if (rBOrigin9.Checked) { fCTBCode.Text = visuGCode.transformGCodeOffset(-offsetx, -offsety, GCodeVisuAndTransform.translate.Offset9); }
                fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                fCTBCodeClickedLineLast = 0;
                fCTBCodeMarkLine();
                fCTBCode.Cursor = Cursors.IBeam;
                showChangedMessage = true;
                updateDrawing();
                lbDimension.Text = visuGCode.xyzSize.getMinMaxString(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
                lbDimension.Select(0, 0);
            }
            Cursor.Current = Cursors.Default;
        }
        // Setup Custom Buttons during loadSettings()
        string[] btnCustomCommand = new string[9];
        private void setCustomButton(Button btn, string text)
        {
            int index = Convert.ToUInt16(btn.Name.Substring("btnCustom".Length));
            string[] parts = text.Split('|');
            if (parts.Length > 1)
            {
                btn.Text = parts[0];
                if (File.Exists(parts[1]))
                { toolTip1.SetToolTip(btn, parts[0] + "\r\nFile: " + parts[1] + "\r\n" + File.ReadAllText(parts[1])); }
                else
                { toolTip1.SetToolTip(btn, parts[0] + "\r\n" + parts[1]); }
                btnCustomCommand[index] = parts[1];
            }
            else
                btnCustomCommand[index] = "";
        }
        private void btnCustomButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int index = Convert.ToUInt16(clickedButton.Name.Substring("btnCustom".Length));
            processCommands(btnCustomCommand[index]);
        }


        // virtualJoystic sends two step-width-values per second. One position should be reached before next command
        // speed (units/min) = 2 * stepsize * 60 * factor (to compensate speed-ramps)
        private void virtualJoystickXY_JoyStickEvent(object sender, JogEventArgs e)
        {
            int indexX = Math.Abs(e.JogPosX);
            int indexY = Math.Abs(e.JogPosY);
            int dirX = Math.Sign(e.JogPosX);
            int dirY = Math.Sign(e.JogPosY);
            int speed = (int)Math.Max(joystickXYSpeed[indexX], joystickXYSpeed[indexY]);
            String strX = gcode.frmtNum(joystickXYStep[indexX] * dirX);
            String strY = gcode.frmtNum(joystickXYStep[indexY] * dirY);
            String s = "";
            if (speed > 0)
            {
                if (e.JogPosX == 0)
                    s = String.Format("G91 Y{0} F{1}", strY, speed).Replace(',', '.');
                else if (e.JogPosY == 0)
                    s = String.Format("G91 X{0} F{1}", strX, speed).Replace(',', '.');
                else
                    s = String.Format("G91 X{0} Y{1} F{2}", strX, strY, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }
        private void virtualJoystickXY_Enter(object sender, EventArgs e)
        { if (_serial_form.isGrblVers0) sendCommand("G91G1F100"); }
        private void virtualJoystickXY_Leave(object sender, EventArgs e)
        { if (_serial_form.isGrblVers0) sendCommand("G90"); }
        private void virtualJoystickZ_JoyStickEvent(object sender, JogEventArgs e)
        {
            int indexZ = Math.Abs(e.JogPosY);
            int dirZ = Math.Sign(e.JogPosY);
            int speed = (int)joystickZSpeed[indexZ];
            String strZ = gcode.frmtNum(joystickZStep[indexZ] * dirZ);
            if (speed > 0)
            {
                String s = String.Format("G91 Z{0} F{1}", strZ, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }
        private void virtualJoystickA_JoyStickEvent(object sender, JogEventArgs e)
        {
            int indexZ = Math.Abs(e.JogPosY);
            int dirZ = Math.Sign(e.JogPosY);
            int speed = (int)joystickZSpeed[indexZ];
            String strZ = gcode.frmtNum(joystickZStep[indexZ] * dirZ);
            if (speed > 0)
            {
                String s = String.Format("G91 {0}{1} F{2}", ctrl4thName, strZ, speed).Replace(',', '.');
                sendCommand(s, true);
            }
        }

        // Spindle and coolant
        private void cBSpindle_CheckedChanged(object sender, EventArgs e)
        {
            if (cBSpindle.Checked)
            { sendCommand("M3 S" + tBSpeed.Text); }
            else
            { sendCommand("M5"); }
        }
        private void cBCoolant_CheckedChanged(object sender, EventArgs e)
        {
            if (cBCoolant.Checked)
            { sendCommand("M8"); }
            else
            { sendCommand("M9"); }
        }
        private void btnHome_Click(object sender, EventArgs e)
        { sendCommand("$H"); }
        private void btnZeroX_Click(object sender, EventArgs e)
        { sendCommand("G92 X0"); }
        private void btnZeroY_Click(object sender, EventArgs e)
        { sendCommand("G92 Y0"); }
        private void btnZeroZ_Click(object sender, EventArgs e)
        { sendCommand("G92 Z0"); }
        private void btnZeroA_Click(object sender, EventArgs e)
        { sendCommand("G92 " + ctrl4thName + "0"); }
        private void btnZeroXY_Click(object sender, EventArgs e)
        { sendCommand("G92 X0 Y0"); }
        private void btnZeroXYZ_Click(object sender, EventArgs e)
        { sendCommand("G92 X0 Y0 Z0"); }

        private void btnJogX_Click(object sender, EventArgs e)
        { sendCommand("G90 X0 F" + joystickXYSpeed[5].ToString(), true); }
        private void btnJogY_Click(object sender, EventArgs e)
        { sendCommand("G90 Y0 F" + joystickXYSpeed[5].ToString(), true); }
        private void btnJogZ_Click(object sender, EventArgs e)
        { sendCommand("G90 Z0 F" + joystickZSpeed[5].ToString(), true); }
        private void btnJogZeroA_Click(object sender, EventArgs e)
        { sendCommand("G90 " + ctrl4thName + "0 F" + joystickZSpeed[5].ToString(), true); }
        private void btnJogXY_Click(object sender, EventArgs e)
        { sendCommand("G90 X0 Y0 F" + joystickXYSpeed[5].ToString(), true); }
        private void btnJogStop_Click(object sender, EventArgs e)
        { sendRealtimeCommand(133); }    //0x85

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
            cBSpindle.Checked = false;
            cBCoolant.Checked = false;
            updateControls();
        }
        private void btnFeedHold_Click(object sender, EventArgs e)
        {
            sendRealtimeCommand('!');
            signalResume = 1;
            updateControls(true);
        }
        private void btnResume_Click(object sender, EventArgs e)
        {
            sendRealtimeCommand('~');
            btnResume.BackColor = SystemColors.Control;
            signalResume = 0;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            updateControls();
        }
        private void btnKillAlarm_Click(object sender, EventArgs e)
        {
            sendCommand("$X");
            signalLock = 0;
            btnKillAlarm.BackColor = SystemColors.Control;
            lbInfo.Text = "";
            lbInfo.BackColor = SystemColors.Control;
            updateControls();
        }

        #endregion

        public GCodeVisuAndTransform visuGCode = new GCodeVisuAndTransform();
        // Refresh drawing path in GCodeVisuAndTransform by applying no transform
        private void redrawGCodePath()
        {
            visuGCode.getGCodeLines(fCTBCode.Lines);
            updateDrawing();
            lbDimension.Text = visuGCode.xyzSize.getMinMaxString(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
            lbDimension.Select(0, 0);
            toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
            toolStrip_tb_X_X_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimx);
            toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
            toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.000}", visuGCode.xyzSize.dimy);
        }

        private void cBTool_CheckedChanged(object sender, EventArgs e)
        {
            _serial_form.toolInSpindle = cBTool.Checked;
        }

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

        private void processCommands(string command)
        {   string[] commands;
            if (File.Exists(command))
            {
                string fileCmd = File.ReadAllText(command);
                _serial_form.addToLog("file: " + command);
                commands = fileCmd.Split('\n');
            }
            else
            {
                commands = command.Split(';');
            }
            foreach (string btncmd in commands)
                sendCommand(btncmd.Trim());
        }
        private void processSpecialCommands(string command)
        {   if (command.ToLower().IndexOf("#start") >= 0) { btnStreamStart_Click(this, EventArgs.Empty); }
            else if (command.ToLower().IndexOf("#stop") >= 0) { btnStreamStop_Click(this, EventArgs.Empty); }
            else if(command.ToLower().IndexOf("#f100") >= 0) { sendRealtimeCommand(144); }
            else if(command.ToLower().IndexOf("#f+10") >= 0) { sendRealtimeCommand(145); }
            else if(command.ToLower().IndexOf("#f-10") >= 0) { sendRealtimeCommand(146); }
            else if(command.ToLower().IndexOf("#f+1") >= 0)  { sendRealtimeCommand(147); }
            else if(command.ToLower().IndexOf("#f-1") >= 0)  { sendRealtimeCommand(148); }
            else if(command.ToLower().IndexOf("#s100") >= 0) { sendRealtimeCommand(153); }
            else if(command.ToLower().IndexOf("#s+10") >= 0) { sendRealtimeCommand(154); }
            else if(command.ToLower().IndexOf("#s-10") >= 0) { sendRealtimeCommand(155); }
            else if(command.ToLower().IndexOf("#s+1") >= 0)  { sendRealtimeCommand(156); }
            else if(command.ToLower().IndexOf("#s-1") >= 0)  { sendRealtimeCommand(157); }
        }
        private bool gamePadSendCmd = false;
        private string gamePadSendString = "";
        private int gamePadRepitition = 0;
        private void gamePadTimer_Tick(object sender, EventArgs e)
        {
            string command = "";
            try
            { ControlGamePad.gamePad.Poll();
                var datas = ControlGamePad.gamePad.GetBufferedData();
                int absVal = 0, stepIndex = 0, feed = 10000, speed1 = 1, speed2 = 1;
                string cmdX = "", cmdY = "", cmdZ = "", cmdR = "", cmd="";
                bool stopJog = false;
                var prop = Properties.Settings.Default;

                gamePadRepitition++;
                if (gamePadRepitition > 4) { gamePadRepitition = 0; }

                if (datas.Length > 0)
                {
                    cmd = "G91";
                    foreach (var state in datas)
                    {
                        string offset = state.Offset.ToString();
                        int value = state.Value;
                        if ((value > 0) && (offset.IndexOf("Buttons") >= 0))
                        {   try
                            {   command = Properties.Settings.Default["gP" + offset].ToString();
                                if (command.IndexOf('#') >= 0)
                                { processSpecialCommands(command); }
                                else
                                { processCommands(command); }
                            }
                            catch { }
                        }


                        if ((offset == "X") || (offset == "Y") || (offset == "Z") || (offset == "RotationZ"))
                        {
                            if ((value > 28000) && (value < 36000))
                            { sendRealtimeCommand(133); stopJog = true;
                                gamePadSendCmd = false;
                                gamePadSendString = "";
                            }
                            else
                            {
                                stepIndex = gamePadIndex(value);// absVal) / 6500;
                                if (stepIndex > 0)
                                {   Int32.TryParse(prop["joyXYSpeed" + stepIndex.ToString()].ToString(), out speed1);
                                    Int32.TryParse(prop["joyZSpeed" + stepIndex.ToString()].ToString(), out speed2);
                                }

                                if (offset == "X")
                                {
                                    gamePadSendCmd = true;
                                    cmdX = gamePadGCode(value, stepIndex, prop.gPXAxis, prop.gPXInvert);    // refresh axis data
                                    feed = gamePadGCodeFeed(feed, speed1, speed2, prop.gPXAxis);
                                }
                                if (offset == "Y")
                                {
                                    gamePadSendCmd = true;
                                    cmdY = gamePadGCode(value, stepIndex, prop.gPYAxis, prop.gPYInvert);    // refresh axis data
                                    feed = gamePadGCodeFeed(feed, speed1, speed2, prop.gPYAxis);
                                }
                                if (offset == "Z")
                                {
                                    gamePadSendCmd = true;
                                    cmdZ = gamePadGCode(value, stepIndex, prop.gPZAxis, prop.gPZInvert);    // refresh axis data
                                    feed = gamePadGCodeFeed(feed, speed1, speed2, prop.gPZAxis);
                                }
                                if (offset == "RotationZ")
                                {
                                    gamePadSendCmd = true;
                                    cmdR = gamePadGCode(value, stepIndex, prop.gPRAxis, prop.gPRInvert);    // refresh axis data
                                    feed = gamePadGCodeFeed(feed, speed1, speed2, prop.gPRAxis);
                                }
                            }
                        }
                        else
                        {   gamePadSendCmd = false;
                            gamePadSendString = "";
                        }
                    }
                    cmd += cmdX + cmdY + cmdZ + cmdR;               // build up command word with last axis information
                    if (cmd.Length > 4)
                        gamePadSendString = cmd + "F" + feed;
                }
                if (gamePadSendCmd && !stopJog && gamePadRepitition == 0)
                { if (gamePadSendString.Length > 0)
                        sendCommand(gamePadSendString, true);
                }

            }
            catch
            {
                try { ControlGamePad.Initialize(); gamePadTimer.Interval = 100; }
                catch { gamePadTimer.Interval = 5000; }
            }
        }

        private int gamePadIndex(int value)         // calculate matching index for virtual joystick values
        {   int absval = Math.Abs(value - 32767);   // depending on joystick position (strange behavior)
            if (absval < 5000) { return 0; }
            if (absval < 12000) { return 1; }
            if (absval < 19000) { return 2; }
            if (absval < 26000) { return 3; }
            if (absval < 32700) { return 4; }
            if (absval >= 32700) { return 5; }
            return 0;
        }

        private string gamePadGCode(int value, int stpIndex, string axis, bool invert)
        {
            string sign = (((value < 32767) && (!invert)) || ((value > 32767) && (invert))) ? "-" : "";
            if (stpIndex > 0)
            {
                string sstep = Properties.Settings.Default["joyXYStep" + stpIndex.ToString()].ToString();
                if ((axis != "X") && (axis != "Y"))
                { sstep = Properties.Settings.Default["joyZStep" + stpIndex.ToString()].ToString();
                }
                return string.Format("{0}{1}{2}", axis, sign, sstep);
            }
            return "";
        }

        private int gamePadGCodeFeed(int feed, int speed1, int speed2, string axis)
        {   if ((axis != "X") && (axis != "Y"))
            { return speed2; }    // Math.Min(feed,speed2);}
                return speed1;  // Math.Min(feed,speed1);
        }

        private void moveToMarkedPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {   int clickedLine = fCTBCode.Selection.ToLine;
            sendCommand(fCTBCode.Lines[clickedLine], false);
        }


        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}

