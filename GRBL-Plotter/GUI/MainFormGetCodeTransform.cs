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
*/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class MainForm : Form
    {

        // handle event from create Height Map form
        #region heightmap
        private void getGCodeScanHeightMap(object sender, EventArgs e)
        {
            if (!isStreaming && _serial_form.serialPortOpen)
            {
                if (_heightmap_form.scanStarted)
                {
                    string[] commands = _heightmap_form.getCode.ToString().Split('\r');
                    _serial_form.isHeightProbing = true;
                    foreach (string cmd in commands)            // fill up send queue
                    {
                        if (machineStatus == grblState.alarm)
                            break;
                        sendCommand(cmd);
                    }
                    VisuGCode.drawHeightMap(_heightmap_form.Map);
                    VisuGCode.createMarkerPath();
                    VisuGCode.calcDrawingArea();
                    pictureBox1.BackgroundImage = null;
                    pictureBox1.Invalidate();
                    if (_diyControlPad != null)
                    { _diyControlPad.isHeightProbing = true; }
					Properties.Settings.Default.counterUseHeightMap += 1;
				
                }
                else
                {
                    _serial_form.stopStreaming();
                    if (_diyControlPad != null)
                    { _diyControlPad.isHeightProbing = false; }
                }
                isHeightMapApplied = false;
            }
        }
        private void loadHeightMap(object sender, EventArgs e)
        {
            if (_heightmap_form.mapIsLoaded)
            {
                VisuGCode.drawHeightMap(_heightmap_form.Map);
                VisuGCode.createMarkerPath();
                VisuGCode.calcDrawingArea();
                pictureBox1.BackgroundImage = null;
                pictureBox1.Invalidate();
                isHeightMapApplied = false;
                _heightmap_form.mapIsLoaded = false;
            }
        }

        private bool isHeightMapApplied = false;
        private StringBuilder codeBeforeHeightMap = new StringBuilder();
        private void applyHeightMap(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (!isHeightMapApplied)
            {
                loadHeightMap(sender, e);
                codeBeforeHeightMap.Clear();
                foreach (string codeline in fCTBCode.Lines)
                {
                    if (codeline.Length > 0)
                        codeBeforeHeightMap.AppendLine(codeline);
                }
                VisuGCode.getGCodeLines(fCTBCode.Lines, null, null);
                fCTBCode.Text = VisuGCode.applyHeightMap(fCTBCode.Lines, _heightmap_form.Map);
                update_GCode_Depending_Controls();
                _heightmap_form.setBtnApply(isHeightMapApplied);
                isHeightMapApplied = true;
            }
            else
            {
                fCTBCode.Text = codeBeforeHeightMap.ToString();
                update_GCode_Depending_Controls();
                _heightmap_form.setBtnApply(isHeightMapApplied);
                isHeightMapApplied = false;
            }
            Cursor.Current = Cursors.Default;
        }
        
        private void getGCodeFromHeightMap(object sender, EventArgs e)
        {
            if (!isStreaming)
            {
                simuStop();
                VisuGCode.clearHeightMap();
                newCodeStart();
                setfCTBCodeText(_heightmap_form.scanCode.ToString().Replace(',', '.'));
                setLastLoadedFile("from height map", "");
                newCodeEnd();
            }
        }
        
        #endregion
        
        // handle event from create Text,  shape, barcode, image, jog path creator
        #region create_from_form
        private void getGCodeFromText(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromText");
            if (!isStreaming)
            {
                importOptions = "";
                simuStop();
                newCodeStart(false);                        // don't clear graphics
                setfCTBCodeText(Graphic.GCode.ToString());  // getGCodeFromText
                setLastLoadedFile("from text", "");
                newCodeEnd();
                foldCode();
                importOptions = Graphic.graphicInformation.ListOptions();
                if (importOptions.Length > 1)
                {   importOptions = "Import options: " + importOptions;
                    statusStripSet(1, importOptions, Color.Yellow);
                }
                Properties.Settings.Default.counterImportText += 1;
            }
            else
                MessageBox.Show(Localization.getString("mainStreamingActive"), Localization.getString("mainAttention"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        // handle event from create Shape
        private void getGCodeFromShape(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromShape");
            if (!isStreaming)
            {
                simuStop();
                newCodeStart();
                VisuGCode.pathBackground = (GraphicsPath)_shape_form.pathBackground.Clone();
                setfCTBCodeText(_shape_form.shapeGCode);
                setLastLoadedFile("from shape", "");
                newCodeEnd();
                Properties.Settings.Default.counterImportShape += 1;
           }
            else
                MessageBox.Show(Localization.getString("mainStreamingActive"));
        }

        private void getGCodeFromBarcode(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromBarcode");
            if (!isStreaming)
            {
                simuStop();
                newCodeStart(false);                        // don't clear graphics
                setfCTBCodeText(Graphic.GCode.ToString());  // getGCodeFromBarcode
                setLastLoadedFile("from barcode", "");
                newCodeEnd();
                Properties.Settings.Default.counterImportBarcode += 1;
            }
            else
                MessageBox.Show(Localization.getString("mainStreamingActive"));

            Cursor.Current = Cursors.Default;
        }

        // handle event from create Image form
        private void getGCodeFromImage(object sender, EventArgs e)
        {
            Logger.Info("getGCodeFromImage");
            if (!isStreaming)
            {
                simuStop();
                newCodeStart();
                setfCTBCodeText(_image_form.imageGCode);
                if (Properties.Settings.Default.importImageResoApply)
                    penDown.Width = (float)Properties.Settings.Default.importImageReso;
                else
                    penDown.Width = (float)Properties.Settings.Default.gui2DWidthPenDown;
                setLastLoadedFile("from image", "");
                newCodeEnd();
                foldCode();
                Properties.Settings.Default.counterImportImage += 1;
            }
            else
                MessageBox.Show(Localization.getString("mainStreamingActive"));
        }
        
        private void getGCodeJogCreator(object sender, EventArgs e)
        {
            Logger.Info("getGCodeJogCreator");
            if (!isStreaming)
            {   sendCommands(_jogPathCreator_form.jogGCode,true); }
            else
                MessageBox.Show(Localization.getString("mainStreamingActive"));
        }
        
        private void getGCodeJogCreator2(object sender, EventArgs e)
        {
            Logger.Info("getGCodeJogCreator2");
            if (!isStreaming)
            {
                simuStop();
                importOptions = "";
                newCodeStart();
                setfCTBCodeText(_jogPathCreator_form.jogGCode);
                setLastLoadedFile("from jog path creator", "");
                newCodeEnd();
                showImportOptions();
            }
            else
                MessageBox.Show(Localization.getString("mainStreamingActive"));
        }
        
        #endregion

        // handle positon click event from camera form
        #region camera
        private void OnRaisePositionClickEvent(object sender, XYZEventArgs e)
        {
            if (e.Command.IndexOf("G91") >= 0)
            {
                string final = e.Command;
                if (grbl.isMarlin) final += ";G1 ";
                if (e.PosX != null)
                    final += string.Format(" X{0}", gcode.frmtNum((float)e.PosX));
                if (e.PosY != null)
                    final += string.Format(" Y{0}", gcode.frmtNum((float)e.PosY));
                if (e.PosZ != null)
                    final += string.Format(" Z{0}", gcode.frmtNum((float)e.PosZ));
                sendCommands(final.Replace(',', '.'), true);
            }
        }
        private void OnRaiseCameraClickEvent(object sender, XYEventArgs e)
        {
            if (e.Command == "a")
            {
                if (fCTBCode.LinesCount > 1)
                {
                    VisuGCode.markSelectedFigure(-1);           // rotate all figures
                    transformStart(string.Format("Rotate {0:0.00}", e.Angle));
                    fCTBCode.Text = VisuGCode.transformGCodeRotate(e.Angle, e.Scale, e.Point, false);     // use given center
                    transformEnd();
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
                    {
                        s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        sendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G0") >= 0) || (cmd.Trim().IndexOf("G1") >= 0))        // no jogging
                    {
                        s = String.Format(cmd + " X{0} Y{1}", realStepX, realStepY).Replace(',', '.');
                        sendCommand(s);
                    }
                    else if ((cmd.Trim().IndexOf("G90") == 0) || (cmd.Trim().IndexOf("G91") == 0))      // no G0 G1, then jogging
                    {
                        speed = 100 + (int)Math.Sqrt(realStepX * realStepX + realStepY * realStepY) * 120;
                        s = String.Format("{0} X{1} Y{2} F{3}", cmd, realStepX, realStepY, speed).Replace(',', '.');
                        if (grbl.isMarlin)
                            s = String.Format("{0}; G1 X{1} Y{2} F{3}", cmd, realStepX, realStepY, speed).Replace(',', '.');

                        sendCommands(s, true);
                    }
                    else
                    {
                        sendCommand(cmd.Trim());
                    }
                }
            }
        }        
        #endregion

        #region MAIN-MENU GCode Transform

        private void transformStart(string action, bool resetMark = true)
        {   Cursor.Current = Cursors.WaitCursor;
            unDo.setCode(fCTBCode.Text,action,this);
            showPicBoxBgImage = false;                      // don't show background image anymore
            pictureBox1.BackgroundImage = null;
            pBoxTransform.Reset();
    /*        if (resetMark)
            {   visuGCode.markSelectedFigure(-1);           // hide highlight
                grbl.posMarker = new xyPoint(0, 0);
            }*/
        }

        private void transformEnd()
        {   VisuGCode.getGCodeLines(fCTBCode.Lines, null, null);                    // get code path
            VisuGCode.calcDrawingArea();                                 // calc ruler dimension
            VisuGCode.drawMachineLimit(toolTable.getToolCordinates());
            pictureBox1.Invalidate();                                   // resfresh view
            update_GCode_Depending_Controls();                          // update GUI controls
            timerUpdateControlSource = "transformEnd";
            updateControls();                                           // update control enable 
            enableCmsCodeBlocks(VisuGCode.codeBlocksAvailable());
            this.Cursor = Cursors.Default;
            setEditMode(false);
        }

        private void btnOffsetApply_Click(object sender, EventArgs e)
        {
            double offsetx = 0, offsety = 0;
            if (!Double.TryParse(tbOffsetX.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out offsetx))
            {
                MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                tbOffsetX.Text = string.Format("{0:0.00}", offsetx);
            }
            if (!Double.TryParse(tbOffsetY.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out offsety))
            {
                MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                tbOffsetY.Text = string.Format("{0:0.00}", offsety);
            }
            if (fCTBCode.Lines.Count > 1)
            {
                transformStart("Apply Offset");
                zoomFactor = 1;
                if (rBOrigin1.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset1); }
                if (rBOrigin2.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset2); }
                if (rBOrigin3.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset3); }
                if (rBOrigin4.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset4); }
                if (rBOrigin5.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset5); }
                if (rBOrigin6.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset6); }
                if (rBOrigin7.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset7); }
                if (rBOrigin8.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset8); }
                if (rBOrigin9.Checked) { fCTBCode.Text = VisuGCode.transformGCodeOffset(-offsetx, -offsety, VisuGCode.translate.Offset9); }
                fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                fCTBCodeClickedLineLast = 0;

                transformEnd();
            }
            Cursor.Current = Cursors.Default;
        }

        private void mirrorXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Mirror X");
            fCTBCode.Text = VisuGCode.transformGCodeMirror(VisuGCode.translate.MirrorX);
            transformEnd();
        }

        private void mirrorYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Mirror Y");
            fCTBCode.Text = VisuGCode.transformGCodeMirror(VisuGCode.translate.MirrorY);
            transformEnd();
        }

        private void mirrorRotaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Rotate");
            fCTBCode.Text = VisuGCode.transformGCodeMirror(VisuGCode.translate.MirrorRotary);
            transformEnd();
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Rotate 90");
            fCTBCode.Text = VisuGCode.transformGCodeRotate(90, 1, new xyPoint(0, 0));
            transformEnd();
        }

        private void rotate90ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            transformStart("Rotate -90");
            fCTBCode.Text = VisuGCode.transformGCodeRotate(-90, 1, new xyPoint(0, 0));
            transformEnd();
        }

        private void rotate180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Rotate 180");
            fCTBCode.Text = VisuGCode.transformGCodeRotate(180, 1, new xyPoint(0, 0));
            transformEnd();
        }

        private void toolStrip_tb_rotate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double anglenew;
                if (Double.TryParse(toolStrip_tb_rotate.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out anglenew))
                {
                    transformStart(string.Format("Rotate {0:0.00}", anglenew));
                    fCTBCode.Text = VisuGCode.transformGCodeRotate(anglenew, 1, new xyPoint(0, 0));
                    transformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_rotate.Text = "0.0";
                }
                e.SuppressKeyPress = true;
            }
        }

        private void toolStrip_tb_XY_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double size;
                if (Double.TryParse(toolStrip_tb_XY_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out size))
                {
                    transformStart("Scale");
                    fCTBCode.Text = VisuGCode.transformGCodeScale(size, size);
                    transformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_XY_scale.Text = "100.00";
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_XY_X_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_XY_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    toolStrip_tb_XY_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_XY_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_XY_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = VisuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_XY_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    toolStrip_tb_XY_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_XY_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_X_scale_KeyDown(object sender, KeyEventArgs e)    // scale X in %
        {
            if (e.KeyValue == (char)13)
            {
                double size;
                if (Double.TryParse(toolStrip_tb_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out size))
                {
                    transformStart("Scale");
                    fCTBCode.Text = VisuGCode.transformGCodeScale(size, 100);
                    transformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_X_scale.Text = "100.00";
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_X_X_scale_KeyDown(object sender, KeyEventArgs e)      // scale X to given units
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_X_X_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    if (Properties.Settings.Default.rotarySubstitutionEnable && Properties.Settings.Default.rotarySubstitutionX)
                    {
                        double length = (float)Properties.Settings.Default.rotarySubstitutionDiameter * Math.PI;
                        sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / length;
                    }
                    toolStrip_tb_X_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_X_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_X_X_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_X_A_scale_KeyDown(object sender, KeyEventArgs e)      // scale X to circumfence of given degree
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew; // get degree
                double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_X_A_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / 360;
                    toolStrip_tb_X_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_X_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_X_A_scale.Text = string.Format("{0:0.00}", 90);
                }
                e.SuppressKeyPress = true;
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
                    transformStart("Scale");
                    fCTBCode.Text = VisuGCode.transformGCodeScale(100, size);
                    transformEnd();
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_Y_scale.Text = "100.00";
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_Y_Y_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = VisuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_Y_Y_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    if (Properties.Settings.Default.rotarySubstitutionEnable && !Properties.Settings.Default.rotarySubstitutionX)
                    {
                        double length = (float)Properties.Settings.Default.rotarySubstitutionDiameter * Math.PI;
                        sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / length;
                    }
                    toolStrip_tb_Y_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_Y_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.00}", sizeold);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_Y_A_scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = VisuGCode.xyzSize.dimy;
                if (Double.TryParse(toolStrip_tb_Y_A_scale.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    sizenew = (float)Properties.Settings.Default.rotarySubstitutionScale * sizenew / 360;
                    toolStrip_tb_Y_scale.Text = string.Format("{0:0.00000}", (100 * sizenew / sizeold));
                    toolStrip_tb_Y_scale_KeyDown(sender, e);
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_Y_A_scale.Text = string.Format("{0:0.00}", 90);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void toolStrip_tb_rotary_diameter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)13)
            {
                double sizenew;
                double sizeold = VisuGCode.xyzSize.dimx;
                if (Double.TryParse(toolStrip_tb_rotary_diameter.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sizenew))
                {
                    Properties.Settings.Default.rotarySubstitutionDiameter = (decimal)sizenew;
                    string tmp = string.Format("Calculating rotary angle depending on part diameter ({0:0.00} units) and desired size.\r\nSet part diameter in Setup - Control.", Properties.Settings.Default.rotarySubstitutionDiameter);
                    skaliereAufXUnitsToolStripMenuItem.ToolTipText = tmp;
                    skaliereAufYUnitsToolStripMenuItem.ToolTipText = tmp;
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tb_rotary_diameter.Text = string.Format("{0:0.00}", Properties.Settings.Default.rotarySubstitutionDiameter);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
                transformEnd();
            }
        }

        private void toolStrip_tBRadiusCompValue_KeyDown(object sender, KeyEventArgs e)
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
                        unDo.setCode(fCTBCode.Text, string.Format("Radius compensation {0:0.00}", radius), this);
                        showPicBoxBgImage = false;                  // don't show background image anymore
                        pictureBox1.BackgroundImage = null;
                        pBoxTransform.Reset();
                        grbl.posMarker = new xyzPoint(0, 0, 0);

                        fCTBCode.Text = VisuGCode.transformGCodeRadiusCorrection(radius);
//                        showMessageForm(log.get());
                        transformEnd();
                    }
                }
                else
                {
                    MessageBox.Show(Localization.getString("mainParseError"), Localization.getString("mainAttention"));
                    toolStrip_tBRadiusCompValue.Text = string.Format("{0:0.000}", radius);
                }
                e.SuppressKeyPress = true;
                gCodeToolStripMenuItem.HideDropDown();
            }
        }

        private void ersetzteG23DurchLinienToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Replace G2 / G3");
            fCTBCode.Text = VisuGCode.replaceG23();
            transformEnd();
        }

        private void convertZToSspindleSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Convert Z");
            fCTBCode.Text = VisuGCode.convertZ();
            transformEnd();
        }

        private void removeAnyZMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transformStart("Remove any Z");
            fCTBCode.Text = VisuGCode.removeZ();
            transformEnd();
        }

        private void update_GCode_Depending_Controls()
        {
            lbDimension.Text = VisuGCode.xyzSize.getMinMaxString() + "\r\n" + VisuGCode.getProcessingTime(); //String.Format("X:[ {0:0.0} | {1:0.0} ];    Y:[ {2:0.0} | {3:0.0} ];    Z:[ {4:0.0} | {5:0.0} ]", visuGCode.xyzSize.minx, visuGCode.xyzSize.maxx, visuGCode.xyzSize.miny, visuGCode.xyzSize.maxy, visuGCode.xyzSize.minz, visuGCode.xyzSize.maxz);
            lbDimension.Select(0, 0);
//            toolTip1.SetToolTip(lbDimension, visuGCode.getProcessingTime());
            checkMachineLimit();
            toolStrip_tb_XY_X_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimx);
            toolStrip_tb_X_X_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimx);
            toolStrip_tb_XY_Y_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimy);
            toolStrip_tb_Y_Y_scale.Text = string.Format("{0:0.000}", VisuGCode.xyzSize.dimy);
            btnSimulate.Enabled = true;
            setGcodeVariables();

            if (VisuGCode.containsG2G3Command())                        // disable X/Y independend scaling if G2 or G3 GCode is in use
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
        private void checkMachineLimit()
        {
            if ((Properties.Settings.Default.machineLimitsShow) && (pictureBox1.BackgroundImage == null))
            {
                if (!VisuGCode.xyzSize.withinLimits(grbl.posMachine, grbl.posWork))
                {
                    lbDimension.BackColor = Color.Fuchsia;
                    decimal minx = Properties.Settings.Default.machineLimitsHomeX;
                    decimal maxx = minx + Properties.Settings.Default.machineLimitsRangeX;
                    decimal miny = Properties.Settings.Default.machineLimitsHomeY;
                    decimal maxy = miny + Properties.Settings.Default.machineLimitsRangeY;
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
        private void btnLimitExceed_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Graphics dimension exceeds machine dimension!\r\nTransformation is recommended to avoid damaging the machine! ", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            MessageBox.Show(Localization.getString("mainLimits3"), Localization.getString("mainAttention"));
        }

        #endregion

        private void unDoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fCTBCode.Text = unDo.getCode();
            transformEnd();
        }
    }
}
