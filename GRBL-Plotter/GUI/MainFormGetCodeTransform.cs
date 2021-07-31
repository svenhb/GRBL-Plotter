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
 * 2019-07-08 add foldCode() to text and image import 
 * 2020-01-01 replace #if debuginfo by Logger.Info
 * 2021-01-15 import code from jog path creator
 * 2021-01-20 move code for camera handling from 'MainForm' to here
 * 2021-01-20 bug fix rotation from camera form
 * 2021-07-02 code clean up / code quality
*/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

//#pragma warning disable CA1303
//#pragma warning disable CA1305
//#pragma warning disable CA1307

namespace GrblPlotter
{
    public partial class MainForm : Form
    {

        // handle event from create Height Map form
        #region heightmap
        private void GetGCodeScanHeightMap(object sender, EventArgs e)
        {
            if (!isStreaming && _serial_form.SerialPortOpen)
            {
                if (_heightmap_form.scanStarted)
                {
                    string[] commands = _heightmap_form.GetCode.ToString().Split('\r');
                    _serial_form.IsHeightProbing = true;
                    foreach (string cmd in commands)            // fill up send queue
                    {
                        if (machineStatus == GrblState.alarm)
                            break;
                        SendCommand(cmd);
                    }
                    VisuGCode.DrawHeightMap(_heightmap_form.Map);
                    VisuGCode.CreateMarkerPath();
                    VisuGCode.CalcDrawingArea();
                    pictureBox1.BackgroundImage = null;
                    pictureBox1.Invalidate();
                    if (_diyControlPad != null)
                    { _diyControlPad.isHeightProbing = true; }
                    Properties.Settings.Default.counterUseHeightMap += 1;

                }
                else
                {
                    _serial_form.StopStreaming(true);   // isNotStartup = true
                    if (_diyControlPad != null)
                    { _diyControlPad.isHeightProbing = false; }
                }
                isHeightMapApplied = false;
            }
        }
        private void LoadHeightMap(object sender, EventArgs e)
        {
            if (_heightmap_form.mapIsLoaded)
            {
                VisuGCode.DrawHeightMap(_heightmap_form.Map);
                VisuGCode.CreateMarkerPath();
                VisuGCode.CalcDrawingArea();
                pictureBox1.BackgroundImage = null;
                pictureBox1.Invalidate();
                isHeightMapApplied = false;
                _heightmap_form.mapIsLoaded = false;
            }
        }

        private bool isHeightMapApplied = false;
        private readonly StringBuilder codeBeforeHeightMap = new StringBuilder();
        private void ApplyHeightMap(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (!isHeightMapApplied)
            {
                LoadHeightMap(sender, e);
                codeBeforeHeightMap.Clear();
                foreach (string codeline in fCTBCode.Lines)
                {
                    if (codeline.Length > 0)
                        codeBeforeHeightMap.AppendLine(codeline);
                }
                VisuGCode.GetGCodeLines(fCTBCode.Lines, null, null);
                fCTBCode.Text = VisuGCode.ApplyHeightMap(_heightmap_form.Map);//fCTBCode.Lines,
                Update_GCode_Depending_Controls();
                _heightmap_form.SetBtnApply(isHeightMapApplied);
                isHeightMapApplied = true;
            }
            else
            {
                fCTBCode.Text = codeBeforeHeightMap.ToString();
                Update_GCode_Depending_Controls();
                _heightmap_form.SetBtnApply(isHeightMapApplied);
                isHeightMapApplied = false;
            }
            Cursor.Current = Cursors.Default;
        }

        private void GetGCodeFromHeightMap(object sender, EventArgs e)
        {
            if (!isStreaming)
            {
                SimuStop();
                VisuGCode.ClearHeightMap();
                NewCodeStart();
                SetFctbCodeText(_heightmap_form.scanCode.ToString().Replace(',', '.'));
                SetLastLoadedFile("from height map", "");
                NewCodeEnd();
            }
        }

        #endregion

        // handle event from create Text,  shape, barcode, image, jog path creator
        #region create_from_form
        private void GetGCodeFromText(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromText");
            if (!isStreaming)
            {
                importOptions = "";
                SimuStop();
                NewCodeStart(false);                        // don't clear graphics
                SetFctbCodeText(Graphic.GCode.ToString());  // getGCodeFromText
                SetLastLoadedFile("from text", "");
                NewCodeEnd();
                FoldCode();
                importOptions = Graphic.graphicInformation.ListOptions();
                if (importOptions.Length > 1)
                {
                    importOptions = "Import options: " + importOptions;
                    StatusStripSet(1, importOptions, Color.Yellow);
                }
                Properties.Settings.Default.counterImportText += 1;
            }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"), Localization.GetString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        // handle event from create Shape
        private void GetGCodeFromShape(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromShape");
            if (!isStreaming)
            {
                SimuStop();
                NewCodeStart();
                VisuGCode.pathBackground = (GraphicsPath)_shape_form.PathBackground.Clone();
                SetFctbCodeText(_shape_form.ShapeGCode);
                SetLastLoadedFile("from shape", "");
                NewCodeEnd();
                Properties.Settings.Default.counterImportShape += 1;
            }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"));
        }

        private void GetGCodeFromBarcode(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromBarcode");
            if (!isStreaming)
            {
                SimuStop();
                NewCodeStart(false);                        // don't clear graphics
                SetFctbCodeText(Graphic.GCode.ToString());  // getGCodeFromBarcode
                SetLastLoadedFile("from barcode", "");
                NewCodeEnd();
                Properties.Settings.Default.counterImportBarcode += 1;
            }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"));

            Cursor.Current = Cursors.Default;
        }

        // handle event from create Image form
        private void GetGCodeFromImage(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromImage");
            if (!isStreaming)
            {
                SimuStop();
                NewCodeStart();
                SetFctbCodeText(_image_form.ImageGCode);
                if (Properties.Settings.Default.importImageResoApply)
                    penDown.Width = (float)Properties.Settings.Default.importImageReso;
                else
                    penDown.Width = (float)Properties.Settings.Default.gui2DWidthPenDown;
                SetLastLoadedFile("from image", "");
                NewCodeEnd();
                FoldCode();
                Properties.Settings.Default.counterImportImage += 1;
            }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"));
        }

        private void GetGCodeJogCreator(object sender, EventArgs e)
        {
            Logger.Info("getGCodeJogCreator");
            if (!isStreaming)
            { SendCommands(_jogPathCreator_form.JogGCode, true); }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"));
        }

        private void GetGCodeJogCreator2(object sender, EventArgs e)
        {
            Logger.Info("getGCodeJogCreator2");
            if (!isStreaming)
            {
                SimuStop();
                importOptions = "";
                NewCodeStart();
                SetFctbCodeText(_jogPathCreator_form.JogGCode);
                SetLastLoadedFile("from jog path creator", "");
                NewCodeEnd();
                ShowImportOptions();
            }
            else
                MessageBox.Show(Localization.GetString("mainStreamingActive"));
        }

        #endregion

        // handle positon click event from camera form
        #region camera
        private void OnRaisePositionClickEvent(object sender, XyzEventArgs e)
        {
            if (e.Command.IndexOf("G91") >= 0)
            {
                string final = e.Command;
                if (Grbl.isMarlin) final += ";G1 ";
                if (e.PosX != null)
                    final += string.Format(" X{0}", Gcode.FrmtNum((float)e.PosX));
                if (e.PosY != null)
                    final += string.Format(" Y{0}", Gcode.FrmtNum((float)e.PosY));
                if (e.PosZ != null)
                    final += string.Format(" Z{0}", Gcode.FrmtNum((float)e.PosZ));
                SendCommands(final.Replace(',', '.'), true);
            }
        }
        private void OnRaiseCameraClickEvent(object sender, XYEventArgs e)
        {
            if (e.Command == "a")
            {
                if (fCTBCode.LinesCount > 1)
                {
                    VisuGCode.MarkSelectedFigure(-1);           // rotate all figures
                    TransformStart(string.Format("Rotate {0:0.00}", e.Angle));
                    fCTBCode.Text = VisuGCode.TransformGCodeRotate(e.Angle, e.Scale, e.Point, false);     // use given center
                    TransformEnd();
                }
            }
            else
            {
                double realStepX = Math.Round(e.Point.X, 3);
                double realStepY = Math.Round(e.Point.Y, 3);
                int speed;
                string s;
                string[] line = e.Command.Split(';');
                foreach (string cmd in line)
                {
                    if (cmd.Trim() == "G92")
                    {
                        s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        SendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G0") >= 0) || (cmd.Trim().IndexOf("G1") >= 0))        // no jogging
                    {
                        s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        SendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G90") == 0) || (cmd.Trim().IndexOf("G91") == 0))      // no G0 G1, then jogging
                    {
                        speed = 100 + (int)Math.Sqrt(realStepX * realStepX + realStepY * realStepY) * 120;
                        s = String.Format("{0} X{1} Y{2} F{3}", cmd, realStepX, realStepY, speed).Replace(',', '.');
                        if (Grbl.isMarlin)
                            s = String.Format("{0}; G1 X{1} Y{2} F{3}", cmd, realStepX, realStepY, speed).Replace(',', '.');

                        SendCommands(s, true);
                    }
                    else
                    {
                        SendCommand(cmd.Trim());
                    }
                }
            }
        }
        #endregion

        #region MAIN-MENU GCode Transform

        private void TransformStart(string action)//, bool resetMark = true)
        {
            Cursor.Current = Cursors.WaitCursor;
            UnDo.SetCode(fCTBCode.Text, action, this);
            showPicBoxBgImage = false;                      // don't show background image anymore
            pictureBox1.BackgroundImage = null;
            pBoxTransform.Reset();
            /*        if (resetMark)
                    {   visuGCode.markSelectedFigure(-1);           // hide highlight
                        grbl.posMarker = new xyPoint(0, 0);
                    }*/
        }

        private void TransformEnd()
        {
            VisuGCode.GetGCodeLines(fCTBCode.Lines, null, null);                    // get code path
            VisuGCode.CalcDrawingArea();                                 // calc ruler dimension
            VisuGCode.DrawMachineLimit();// ToolTable.GetToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
            Update_GCode_Depending_Controls();                          // update GUI controls
            timerUpdateControlSource = "transformEnd";
            UpdateControlEnables();                                           // update control enable 
            EnableCmsCodeBlocks(VisuGCode.CodeBlocksAvailable());
            this.Cursor = Cursors.Default;
            SetEditMode(false);
        }

        private void BtnOffsetApply_Click(object sender, EventArgs e)
        {
            //     double offsetx = 0, offsety = 0;
            if (!Double.TryParse(tbOffsetX.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double offsetx))
            {
                MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                offsetx = 0;
                tbOffsetX.Text = string.Format("{0:0.00}", offsetx);
            }
            if (!Double.TryParse(tbOffsetY.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double offsety))
            {
                MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                offsety = 0;
                tbOffsetY.Text = string.Format("{0:0.00}", offsety);
            }
            if (fCTBCode.Lines.Count > 1)
            {
                TransformStart("Apply Offset");
                zoomFactor = 1;
                if (rBOrigin1.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset1); }
                if (rBOrigin2.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset2); }
                if (rBOrigin3.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset3); }
                if (rBOrigin4.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset4); }
                if (rBOrigin5.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset5); }
                if (rBOrigin6.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset6); }
                if (rBOrigin7.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset7); }
                if (rBOrigin8.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset8); }
                if (rBOrigin9.Checked) { fCTBCode.Text = VisuGCode.TransformGCodeOffset(-offsetx, -offsety, VisuGCode.Translate.Offset9); }
                fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                fCTBCodeClickedLineLast = 0;

                TransformEnd();
            }
            Cursor.Current = Cursors.Default;
        }

        private void MirrorXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Mirror X");
            fCTBCode.Text = VisuGCode.TransformGCodeMirror(VisuGCode.Translate.MirrorX);
            TransformEnd();
        }

        private void MirrorYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Mirror Y");
            fCTBCode.Text = VisuGCode.TransformGCodeMirror(VisuGCode.Translate.MirrorY);
            TransformEnd();
        }

        private void MirrorRotaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Rotate");
            fCTBCode.Text = VisuGCode.TransformGCodeMirror(VisuGCode.Translate.MirrorRotary);
            TransformEnd();
        }

        private void Rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Rotate 90");
            fCTBCode.Text = VisuGCode.TransformGCodeRotate(90, 1, new XyPoint(0, 0));
            TransformEnd();
        }

        private void Rotate90ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TransformStart("Rotate -90");
            fCTBCode.Text = VisuGCode.TransformGCodeRotate(-90, 1, new XyPoint(0, 0));
            TransformEnd();
        }

        private void Rotate180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Rotate 180");
            fCTBCode.Text = VisuGCode.TransformGCodeRotate(180, 1, new XyPoint(0, 0));
            TransformEnd();
        }

        private void ToolStrip_tb_rotate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //    double anglenew;
                if (Double.TryParse(toolStrip_tb_rotate.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double anglenew))
                {
                    TransformStart(string.Format("Rotate {0:0.00}", anglenew));
                    fCTBCode.Text = VisuGCode.TransformGCodeRotate(anglenew, 1, new XyPoint(0, 0));
                    TransformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_rotate.Text = "0.0";
                }
                e.SuppressKeyPress = true;
            }
        }

        private void ToolStrip_tb_XY_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //    double size;
                if (Double.TryParse(toolStrip_tb_XY_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double size))
                {
                    TransformStart("Scale");
                    fCTBCode.Text = VisuGCode.TransformGCodeScale(size, size);
                    TransformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_XY_scale.Text = "100.00";
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_XY_X_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //     double sizenew;
                double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_XY_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sizenew))
                {
                    toolStrip_tb_XY_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    ToolStrip_tb_XY_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_XY_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //    double sizenew;
                double sizeold = VisuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_XY_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sizenew))
                {
                    toolStrip_tb_XY_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    ToolStrip_tb_XY_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_X_scale_KeyDown(object sender, KeyEventArgs e)    // scale X in %
        {
            if (e.KeyValue == (char)13)
            {
                //    double size;
                if (Double.TryParse(toolStrip_tb_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double size))
                {
                    TransformStart("Scale");
                    fCTBCode.Text = VisuGCode.TransformGCodeScale(size, 100);
                    TransformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_X_scale.Text = "100.00";
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_X_X_scale_KeyDown(object sender, KeyEventArgs e)      // scale X to given units
        {
            if (e.KeyValue == (char)13)
            {
                //    double sizenew;
                double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_X_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sizenew))
                {
                    if (Properties.Settings.Default.rotarySubstitutionEnable && Properties.Settings.Default.rotarySubstitutionX)
                    {
                        double length = (float)Properties.Settings.Default.rotarySubstitutionDiameter * Math.PI;
                        sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / length;
                    }
                    toolStrip_tb_X_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    ToolStrip_tb_X_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_X_X_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_X_A_scale_KeyDown(object sender, KeyEventArgs e)      // scale X to circumfence of given degree
        {
            if (e.KeyValue == (char)13)
            {
                //     double sizenew; // get degree
                double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_X_A_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sizenew))
                {
                    sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / 360;
                    toolStrip_tb_X_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    ToolStrip_tb_X_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_X_A_scale.Text = string.Format("{0:0.00}", 90);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //     double size;
                if (Double.TryParse(toolStrip_tb_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double size))
                {
                    TransformStart("Scale");
                    fCTBCode.Text = VisuGCode.TransformGCodeScale(100, size);
                    TransformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_Y_scale.Text = "100.00";
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_Y_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //    double sizenew;
                double sizeold = VisuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_Y_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sizenew))
                {
                    if (Properties.Settings.Default.rotarySubstitutionEnable && !Properties.Settings.Default.rotarySubstitutionX)
                    {
                        double length = (float)Properties.Settings.Default.rotarySubstitutionDiameter * Math.PI;
                        sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / length;
                    }
                    toolStrip_tb_Y_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    ToolStrip_tb_Y_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_Y_A_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //    double sizenew;
                double sizeold = VisuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_Y_A_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sizenew))
                {
                    sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / 360;
                    toolStrip_tb_Y_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    ToolStrip_tb_Y_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_Y_A_scale.Text = string.Format("{0:0.00}", 90);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ToolStrip_tb_rotary_diameter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                //    double sizenew;
                //  double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_rotary_diameter.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sizenew))
                {
                    Properties.Settings.Default.rotarySubstitutionDiameter = (decimal)sizenew;
                    string tmp = string.Format("Calculating rotary angle depending on part diameter ({0:0.00} units) and desired size.\r\nSet part diameter in Setup - Control.", Properties.Settings.Default.rotarySubstitutionDiameter);
                    skaliereAufXUnitsToolStripMenuItem.ToolTipText = tmp;
                    skaliereAufYUnitsToolStripMenuItem.ToolTipText = tmp;
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tb_rotary_diameter.Text = string.Format("{0:0.00}", Properties.Settings.Default.rotarySubstitutionDiameter);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
                TransformEnd();
            }
        }

        private void ToolStrip_tBRadiusCompValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double radius = Properties.Settings.Default.crcValue;
                if (Double.TryParse(toolStrip_tBRadiusCompValue.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out radius))
                {
                    Properties.Settings.Default.crcValue = radius;
                    Properties.Settings.Default.guiBackgroundShow = toolStripViewBackground.Checked = true;
                    {
                        //transformStart();
                        Cursor.Current = Cursors.WaitCursor;
                        UnDo.SetCode(fCTBCode.Text, string.Format("Radius compensation {0:0.00}", radius), this);
                        showPicBoxBgImage = false;                  // don't show background image anymore
                        pictureBox1.BackgroundImage = null;
                        pBoxTransform.Reset();
                        Grbl.PosMarker = new XyzPoint(0, 0, 0);

                        fCTBCode.Text = VisuGCode.TransformGCodeRadiusCorrection(radius);
                        //                        showMessageForm(log.get());
                        TransformEnd();
                    }
                }
                else
                {
                    MessageBox.Show(Localization.GetString("mainParseError"), Localization.GetString("mainAttention"));
                    toolStrip_tBRadiusCompValue.Text = string.Format("{0:0.000}", radius);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ErsetzteG23DurchLinienToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Replace G2 / G3");
            fCTBCode.Text = VisuGCode.ReplaceG23();
            TransformEnd();
        }

        private void ConvertZToSspindleSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Convert Z");
            fCTBCode.Text = VisuGCode.ConvertZ();
            TransformEnd();
        }

        private void RemoveAnyZMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransformStart("Remove any Z");
            fCTBCode.Text = VisuGCode.RemoveZ();
            TransformEnd();
        }

        private void Update_GCode_Depending_Controls()
        {
            lbDimension.Text = VisuGCode.xyzSize.GetMinMaxString() + "\r\n" + VisuGCode.GetProcessingTime(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
            lbDimension.Select(0, 0);
            //            toolTip1.SetToolTip(lbDimension, visuGCode.getProcessingTime());
            CheckMachineLimit();
            toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimx);
            toolStrip_tb_X_X_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimx);
            toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimy);
            toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimy);
            btnSimulate.Enabled = true;
            SetGcodeVariables();

            if (VisuGCode.ContainsG2G3Command())                        // disable X/Y independend scaling if G2 or G3 GCode is in use
            {                                                           // because it's not possible to stretch (convert 1st to G1 GCode)                skaliereXUmToolStripMenuItem.Enabled = false;
                skaliereXUmToolStripMenuItem.Enabled = false;
                skaliereYUmToolStripMenuItem.Enabled = false;
                skaliereAufXUnitsToolStripMenuItem.Enabled = false;
                skaliereAufYUnitsToolStripMenuItem.Enabled = false;
                skaliereXAufDrehachseToolStripMenuItem.Enabled = false;
                skaliereYAufDrehachseToolStripMenuItem.Enabled = false;
                ersetzteG23DurchLinienToolStripMenuItem.Enabled = true;
            }
            else
            {
                skaliereXUmToolStripMenuItem.Enabled = true;                // enable X/Y independend scaling because no G2 or G3 GCode
                skaliereYUmToolStripMenuItem.Enabled = true;
                skaliereAufXUnitsToolStripMenuItem.Enabled = true;
                skaliereAufYUnitsToolStripMenuItem.Enabled = true;
                skaliereXAufDrehachseToolStripMenuItem.Enabled = true;
                skaliereYAufDrehachseToolStripMenuItem.Enabled = true;
                ersetzteG23DurchLinienToolStripMenuItem.Enabled = false;
            }
        }
        //     private static int limitcnt = 0;
        private void CheckMachineLimit()
        {
            if ((Properties.Settings.Default.machineLimitsShow) && (pictureBox1.BackgroundImage == null))
            {
                if (!VisuGCode.xyzSize.WithinLimits(Grbl.posMachine, Grbl.posWork))
                {
                    lbDimension.BackColor = Color.Fuchsia;
                    //          decimal minx = Properties.Settings.Default.machineLimitsHomeX;
                    //  decimal maxx = minx + Properties.Settings.Default.machineLimitsRangeX;
                    //          decimal miny = Properties.Settings.Default.machineLimitsHomeY;
                    //  decimal maxy = miny + Properties.Settings.Default.machineLimitsRangeY;
                    btnLimitExceed.Visible = true;
                }
                else
                {
                    lbDimension.BackColor = Color.Lime;
                    toolTip1.SetToolTip(lbDimension, "");
                    btnLimitExceed.Visible = false;
                }
            }
            else
            {
                lbDimension.BackColor = Color.FromArgb(255, 255, 128);
                btnLimitExceed.Visible = false;
            }
        }

        private void BtnLimitExceed_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Graphics dimension exceeds machine dimension!\r\nTransformation is recommended to avoid damaging the machine! ", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            MessageBox.Show(Localization.GetString("mainLimits3"), Localization.GetString("mainAttention"));
        }

        #endregion

        private void UnDoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCTBCode.Text = UnDo.GetCode();
            TransformEnd();
        }
    }
}
