/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2016 Sven Hasemann contact: svenhb@web.de

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
namespace GRBL_Plotter
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                picBoxBackround.Dispose();
     //           gcodeImage.Dispose();
                StyleComment.Dispose();
                StyleFWord.Dispose();
                StyleGWord.Dispose();
                StyleMWord.Dispose();
                StyleSWord.Dispose();
                StyleTool.Dispose();
                StyleXAxis.Dispose();
                StyleYAxis.Dispose();
                StyleZAxis.Dispose();
                penDown.Dispose();
                penMarker.Dispose();
                penRuler.Dispose();
                penTool.Dispose();
                penUp.Dispose();
                //                visuGCode.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveMachineParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMachineParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlStreamingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.control2ndGRBLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createGCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textWizzardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createSimpleShapesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label_mx = new System.Windows.Forms.Label();
            this.label_my = new System.Windows.Forms.Label();
            this.label_mz = new System.Windows.Forms.Label();
            this.label_wx = new System.Windows.Forms.Label();
            this.label_wy = new System.Windows.Forms.Label();
            this.label_wz = new System.Windows.Forms.Label();
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pbBuffer = new System.Windows.Forms.ProgressBar();
            this.btnStreamStop = new System.Windows.Forms.Button();
            this.btnStreamCheck = new System.Windows.Forms.Button();
            this.btnMirrorY = new System.Windows.Forms.Button();
            this.btnMirrorX = new System.Windows.Forms.Button();
            this.btnShiftToZero = new System.Windows.Forms.Button();
            this.btnTransformCode = new System.Windows.Forms.Button();
            this.tbChangeAngle = new System.Windows.Forms.TextBox();
            this.tbChangeSize = new System.Windows.Forms.TextBox();
            this.cmsScale = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.scaleToWidthOfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripScaleTextBoxWidth = new System.Windows.Forms.ToolStripTextBox();
            this.scaleToHeightOfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripScaleTextBoxHeight = new System.Windows.Forms.ToolStripTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbDimension = new System.Windows.Forms.Label();
            this.lbInfo = new System.Windows.Forms.Label();
            this.lblRemaining = new System.Windows.Forms.Label();
            this.pbFile = new System.Windows.Forms.ProgressBar();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblFileProgress = new System.Windows.Forms.Label();
            this.tbFile = new System.Windows.Forms.TextBox();
            this.btnStreamStart = new System.Windows.Forms.Button();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.cmsCode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsCodeSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodePaste = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeSendLine = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnZeroXYZ = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnZeroXY = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnZeroZ = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnZeroY = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnZeroX = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.btnJogZeroXY = new System.Windows.Forms.Button();
            this.btnJogZeroZ = new System.Windows.Forms.Button();
            this.btnJogZeroY = new System.Windows.Forms.Button();
            this.btnJogZeroX = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCustom8 = new System.Windows.Forms.Button();
            this.btnCustom7 = new System.Windows.Forms.Button();
            this.btnCustom6 = new System.Windows.Forms.Button();
            this.btnCustom5 = new System.Windows.Forms.Button();
            this.btnCustom1 = new System.Windows.Forms.Button();
            this.btnCustom2 = new System.Windows.Forms.Button();
            this.btnCustom3 = new System.Windows.Forms.Button();
            this.btnCustom4 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnFeedHold = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnKillAlarm = new System.Windows.Forms.Button();
            this.tBURL = new System.Windows.Forms.TextBox();
            this.btnJogStop = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmsPictureBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deletenotMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteThisCodeLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToFirstPosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tLPAussen = new System.Windows.Forms.TableLayoutPanel();
            this.tLPLinks = new System.Windows.Forms.TableLayoutPanel();
            this.fCTBCode = new FastColoredTextBoxNS.FastColoredTextBox();
            this.tLPRechts = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUnten = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUntenRechts = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblTool = new System.Windows.Forms.Label();
            this.cBTool = new System.Windows.Forms.CheckBox();
            this.virtualJoystickZ = new virtualJoystick.virtualJoystick();
            this.virtualJoystickXY = new virtualJoystick.virtualJoystick();
            this.label9 = new System.Windows.Forms.Label();
            this.tBSpeed = new System.Windows.Forms.TextBox();
            this.cBCoolant = new System.Windows.Forms.CheckBox();
            this.cBSpindle = new System.Windows.Forms.CheckBox();
            this.tLPMitteUnten = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsOben = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.cmsScale.SuspendLayout();
            this.cmsCode.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.cmsPictureBox.SuspendLayout();
            this.tLPAussen.SuspendLayout();
            this.tLPLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).BeginInit();
            this.tLPRechts.SuspendLayout();
            this.tLPRechtsUnten.SuspendLayout();
            this.tLPRechtsUntenRechts.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tLPMitteUnten.SuspendLayout();
            this.tLPRechtsOben.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.machineToolStripMenuItem,
            this.createGCodeToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveMachineParametersToolStripMenuItem,
            this.loadMachineParametersToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.loadToolStripMenuItem.Text = "Open File";
            this.loadToolStripMenuItem.ToolTipText = "Open GCode or SVG file";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(204, 22);
            this.toolStripMenuItem2.Text = "Open Recent File";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(201, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.saveToolStripMenuItem.Text = "Save GCode";
            this.saveToolStripMenuItem.ToolTipText = "Save displayed GCode to a file";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(201, 6);
            // 
            // saveMachineParametersToolStripMenuItem
            // 
            this.saveMachineParametersToolStripMenuItem.Name = "saveMachineParametersToolStripMenuItem";
            this.saveMachineParametersToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.saveMachineParametersToolStripMenuItem.Text = "Export Machine Settings";
            this.saveMachineParametersToolStripMenuItem.ToolTipText = "Export GUI-Settings (Button definitions, Joystick parameters) to an INI-file";
            this.saveMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.saveMachineParametersToolStripMenuItem_Click);
            // 
            // loadMachineParametersToolStripMenuItem
            // 
            this.loadMachineParametersToolStripMenuItem.Name = "loadMachineParametersToolStripMenuItem";
            this.loadMachineParametersToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.loadMachineParametersToolStripMenuItem.Text = "Import Machine Settings";
            this.loadMachineParametersToolStripMenuItem.ToolTipText = "Import GUI-Settings (Button definitions, Joystick parameters) from an INI-file";
            this.loadMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.loadMachineParametersToolStripMenuItem_Click);
            // 
            // machineToolStripMenuItem
            // 
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlStreamingToolStripMenuItem,
            this.control2ndGRBLToolStripMenuItem,
            this.toolStripMenuItem1,
            this.setupToolStripMenuItem});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            this.machineToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.machineToolStripMenuItem.Text = "Machine";
            // 
            // controlStreamingToolStripMenuItem
            // 
            this.controlStreamingToolStripMenuItem.Name = "controlStreamingToolStripMenuItem";
            this.controlStreamingToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.controlStreamingToolStripMenuItem.Text = "Control Streaming";
            this.controlStreamingToolStripMenuItem.ToolTipText = "Override Feed rate and Spindle speed";
            this.controlStreamingToolStripMenuItem.Click += new System.EventHandler(this.controlStreamingToolStripMenuItem_Click);
            // 
            // control2ndGRBLToolStripMenuItem
            // 
            this.control2ndGRBLToolStripMenuItem.Name = "control2ndGRBLToolStripMenuItem";
            this.control2ndGRBLToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.control2ndGRBLToolStripMenuItem.Text = "Control 2nd GRBL";
            this.control2ndGRBLToolStripMenuItem.Click += new System.EventHandler(this.control2ndGRBLToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
            this.toolStripMenuItem1.Text = "Show Camera";
            this.toolStripMenuItem1.ToolTipText = "Show camera picture";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.cameraToolStripMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.setupToolStripMenuItem.Text = "Setup";
            this.setupToolStripMenuItem.ToolTipText = "Setup GUI and import parameters";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
            // 
            // createGCodeToolStripMenuItem
            // 
            this.createGCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textWizzardToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.createSimpleShapesToolStripMenuItem});
            this.createGCodeToolStripMenuItem.Name = "createGCodeToolStripMenuItem";
            this.createGCodeToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.createGCodeToolStripMenuItem.Text = "Create GCode";
            // 
            // textWizzardToolStripMenuItem
            // 
            this.textWizzardToolStripMenuItem.Name = "textWizzardToolStripMenuItem";
            this.textWizzardToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.textWizzardToolStripMenuItem.Text = "Create Text";
            this.textWizzardToolStripMenuItem.Click += new System.EventHandler(this.textWizzardToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.imageToolStripMenuItem.Text = "Convert Image";
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.imageToolStripMenuItem_Click);
            // 
            // createSimpleShapesToolStripMenuItem
            // 
            this.createSimpleShapesToolStripMenuItem.Name = "createSimpleShapesToolStripMenuItem";
            this.createSimpleShapesToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.createSimpleShapesToolStripMenuItem.Text = "Create Simple Shapes";
            this.createSimpleShapesToolStripMenuItem.Visible = false;
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // label_mx
            // 
            this.label_mx.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label_mx.Location = new System.Drawing.Point(48, 31);
            this.label_mx.Name = "label_mx";
            this.label_mx.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_mx.Size = new System.Drawing.Size(72, 12);
            this.label_mx.TabIndex = 1;
            this.label_mx.Text = "0000.000";
            this.label_mx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_my
            // 
            this.label_my.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label_my.Location = new System.Drawing.Point(48, 63);
            this.label_my.Name = "label_my";
            this.label_my.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_my.Size = new System.Drawing.Size(72, 14);
            this.label_my.TabIndex = 2;
            this.label_my.Text = "0000.000";
            this.label_my.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_mz
            // 
            this.label_mz.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label_mz.Location = new System.Drawing.Point(48, 96);
            this.label_mz.Name = "label_mz";
            this.label_mz.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_mz.Size = new System.Drawing.Size(72, 14);
            this.label_mz.TabIndex = 3;
            this.label_mz.Text = "0000.000";
            this.label_mz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_wx
            // 
            this.label_wx.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_wx.Location = new System.Drawing.Point(44, 13);
            this.label_wx.Name = "label_wx";
            this.label_wx.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_wx.Size = new System.Drawing.Size(76, 20);
            this.label_wx.TabIndex = 4;
            this.label_wx.Text = "0000.000";
            this.label_wx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_wy
            // 
            this.label_wy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_wy.Location = new System.Drawing.Point(44, 45);
            this.label_wy.Name = "label_wy";
            this.label_wy.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_wy.Size = new System.Drawing.Size(76, 20);
            this.label_wy.TabIndex = 5;
            this.label_wy.Text = "0000.000";
            this.label_wy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_wz
            // 
            this.label_wz.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_wz.Location = new System.Drawing.Point(44, 78);
            this.label_wz.Name = "label_wz";
            this.label_wz.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_wz.Size = new System.Drawing.Size(76, 20);
            this.label_wz.TabIndex = 6;
            this.label_wz.Text = "0000.000";
            this.label_wz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainTimer
            // 
            this.MainTimer.Enabled = true;
            this.MainTimer.Interval = 500;
            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pbBuffer);
            this.groupBox1.Controls.Add(this.btnStreamStop);
            this.groupBox1.Controls.Add(this.btnStreamCheck);
            this.groupBox1.Controls.Add(this.btnMirrorY);
            this.groupBox1.Controls.Add(this.btnMirrorX);
            this.groupBox1.Controls.Add(this.btnShiftToZero);
            this.groupBox1.Controls.Add(this.btnTransformCode);
            this.groupBox1.Controls.Add(this.tbChangeAngle);
            this.groupBox1.Controls.Add(this.tbChangeSize);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lbDimension);
            this.groupBox1.Controls.Add(this.lbInfo);
            this.groupBox1.Controls.Add(this.lblRemaining);
            this.groupBox1.Controls.Add(this.pbFile);
            this.groupBox1.Controls.Add(this.lblElapsed);
            this.groupBox1.Controls.Add(this.lblFileProgress);
            this.groupBox1.Controls.Add(this.tbFile);
            this.groupBox1.Controls.Add(this.btnStreamStart);
            this.groupBox1.Controls.Add(this.btnOpenFile);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(287, 148);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "G-Code File";
            // 
            // pbBuffer
            // 
            this.pbBuffer.Location = new System.Drawing.Point(219, 46);
            this.pbBuffer.Name = "pbBuffer";
            this.pbBuffer.Size = new System.Drawing.Size(64, 12);
            this.pbBuffer.TabIndex = 3;
            // 
            // btnStreamStop
            // 
            this.btnStreamStop.Image = global::GRBL_Plotter.Properties.Resources.btn_stop;
            this.btnStreamStop.Location = new System.Drawing.Point(60, 13);
            this.btnStreamStop.Name = "btnStreamStop";
            this.btnStreamStop.Size = new System.Drawing.Size(23, 23);
            this.btnStreamStop.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnStreamStop, "Stop sending code to GRBL");
            this.btnStreamStop.UseVisualStyleBackColor = true;
            this.btnStreamStop.Click += new System.EventHandler(this.btnStreamStop_Click);
            // 
            // btnStreamCheck
            // 
            this.btnStreamCheck.Location = new System.Drawing.Point(3, 35);
            this.btnStreamCheck.Name = "btnStreamCheck";
            this.btnStreamCheck.Size = new System.Drawing.Size(80, 23);
            this.btnStreamCheck.TabIndex = 2;
            this.btnStreamCheck.Text = "Check Code";
            this.btnStreamCheck.UseVisualStyleBackColor = true;
            this.btnStreamCheck.Click += new System.EventHandler(this.btnStreamCheck_Click);
            // 
            // btnMirrorY
            // 
            this.btnMirrorY.Location = new System.Drawing.Point(84, 123);
            this.btnMirrorY.Name = "btnMirrorY";
            this.btnMirrorY.Size = new System.Drawing.Size(80, 23);
            this.btnMirrorY.TabIndex = 8;
            this.btnMirrorY.Text = "Mirror Y";
            this.btnMirrorY.UseVisualStyleBackColor = true;
            this.btnMirrorY.Click += new System.EventHandler(this.btnShiftToZero_MirrorXY_Click);
            // 
            // btnMirrorX
            // 
            this.btnMirrorX.Location = new System.Drawing.Point(3, 123);
            this.btnMirrorX.Name = "btnMirrorX";
            this.btnMirrorX.Size = new System.Drawing.Size(80, 23);
            this.btnMirrorX.TabIndex = 7;
            this.btnMirrorX.Text = "Mirror X";
            this.btnMirrorX.UseVisualStyleBackColor = true;
            this.btnMirrorX.Click += new System.EventHandler(this.btnShiftToZero_MirrorXY_Click);
            // 
            // btnShiftToZero
            // 
            this.btnShiftToZero.Location = new System.Drawing.Point(170, 123);
            this.btnShiftToZero.Name = "btnShiftToZero";
            this.btnShiftToZero.Size = new System.Drawing.Size(113, 23);
            this.btnShiftToZero.TabIndex = 9;
            this.btnShiftToZero.Text = "Set Offset to 0;0";
            this.toolTip1.SetToolTip(this.btnShiftToZero, "Set GCode XY Offset to zero");
            this.btnShiftToZero.UseVisualStyleBackColor = true;
            this.btnShiftToZero.Click += new System.EventHandler(this.btnShiftToZero_MirrorXY_Click);
            // 
            // btnTransformCode
            // 
            this.btnTransformCode.Location = new System.Drawing.Point(170, 100);
            this.btnTransformCode.Name = "btnTransformCode";
            this.btnTransformCode.Size = new System.Drawing.Size(113, 23);
            this.btnTransformCode.TabIndex = 6;
            this.btnTransformCode.Text = "Transform GCode";
            this.toolTip1.SetToolTip(this.btnTransformCode, "Recalculate GCode XY positions, applying scale and angle");
            this.btnTransformCode.UseVisualStyleBackColor = true;
            this.btnTransformCode.Click += new System.EventHandler(this.btnTransformCode_Click);
            // 
            // tbChangeAngle
            // 
            this.tbChangeAngle.Location = new System.Drawing.Point(123, 102);
            this.tbChangeAngle.Name = "tbChangeAngle";
            this.tbChangeAngle.Size = new System.Drawing.Size(41, 20);
            this.tbChangeAngle.TabIndex = 5;
            this.tbChangeAngle.Text = "0.00";
            // 
            // tbChangeSize
            // 
            this.tbChangeSize.ContextMenuStrip = this.cmsScale;
            this.tbChangeSize.Location = new System.Drawing.Point(34, 102);
            this.tbChangeSize.Name = "tbChangeSize";
            this.tbChangeSize.Size = new System.Drawing.Size(43, 20);
            this.tbChangeSize.TabIndex = 4;
            this.tbChangeSize.Text = "100.00";
            this.toolTip1.SetToolTip(this.tbChangeSize, "Right click for other options");
            // 
            // cmsScale
            // 
            this.cmsScale.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleToWidthOfToolStripMenuItem,
            this.scaleToHeightOfToolStripMenuItem});
            this.cmsScale.Name = "contextMenuStripScale";
            this.cmsScale.Size = new System.Drawing.Size(167, 48);
            // 
            // scaleToWidthOfToolStripMenuItem
            // 
            this.scaleToWidthOfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripScaleTextBoxWidth});
            this.scaleToWidthOfToolStripMenuItem.Name = "scaleToWidthOfToolStripMenuItem";
            this.scaleToWidthOfToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.scaleToWidthOfToolStripMenuItem.Text = "Scale to width of";
            // 
            // toolStripScaleTextBoxWidth
            // 
            this.toolStripScaleTextBoxWidth.Name = "toolStripScaleTextBoxWidth";
            this.toolStripScaleTextBoxWidth.Size = new System.Drawing.Size(100, 23);
            this.toolStripScaleTextBoxWidth.Text = "100";
            this.toolStripScaleTextBoxWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripScaleTextBoxWidth_KeyDown);
            this.toolStripScaleTextBoxWidth.TextChanged += new System.EventHandler(this.toolStripScaleTextBox1_TextChanged);
            // 
            // scaleToHeightOfToolStripMenuItem
            // 
            this.scaleToHeightOfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripScaleTextBoxHeight});
            this.scaleToHeightOfToolStripMenuItem.Name = "scaleToHeightOfToolStripMenuItem";
            this.scaleToHeightOfToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.scaleToHeightOfToolStripMenuItem.Text = "Scale to height of";
            // 
            // toolStripScaleTextBoxHeight
            // 
            this.toolStripScaleTextBoxHeight.Name = "toolStripScaleTextBoxHeight";
            this.toolStripScaleTextBoxHeight.Size = new System.Drawing.Size(100, 23);
            this.toolStripScaleTextBoxHeight.Text = "50";
            this.toolStripScaleTextBoxHeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripScaleTextBoxWidth_KeyDown);
            this.toolStripScaleTextBoxHeight.TextChanged += new System.EventHandler(this.toolStripScaleTextBoxHeight_TextChanged);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(3, 105);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "Scale";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(161, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "°";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(76, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "%   Angle";
            // 
            // lbDimension
            // 
            this.lbDimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbDimension.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.lbDimension.Location = new System.Drawing.Point(3, 87);
            this.lbDimension.Name = "lbDimension";
            this.lbDimension.Size = new System.Drawing.Size(280, 13);
            this.lbDimension.TabIndex = 16;
            this.lbDimension.Text = "Dimensions";
            // 
            // lbInfo
            // 
            this.lbInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbInfo.Location = new System.Drawing.Point(3, 61);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(280, 21);
            this.lbInfo.TabIndex = 13;
            this.lbInfo.Text = "Message";
            // 
            // lblRemaining
            // 
            this.lblRemaining.AutoSize = true;
            this.lblRemaining.Location = new System.Drawing.Point(229, 67);
            this.lblRemaining.Name = "lblRemaining";
            this.lblRemaining.Size = new System.Drawing.Size(0, 13);
            this.lblRemaining.TabIndex = 12;
            // 
            // pbFile
            // 
            this.pbFile.Location = new System.Drawing.Point(89, 23);
            this.pbFile.Name = "pbFile";
            this.pbFile.Size = new System.Drawing.Size(194, 35);
            this.pbFile.TabIndex = 8;
            // 
            // lblElapsed
            // 
            this.lblElapsed.AutoSize = true;
            this.lblElapsed.Location = new System.Drawing.Point(169, 10);
            this.lblElapsed.Name = "lblElapsed";
            this.lblElapsed.Size = new System.Drawing.Size(30, 13);
            this.lblElapsed.TabIndex = 7;
            this.lblElapsed.Text = "Time";
            // 
            // lblFileProgress
            // 
            this.lblFileProgress.AutoSize = true;
            this.lblFileProgress.Location = new System.Drawing.Point(87, 10);
            this.lblFileProgress.Name = "lblFileProgress";
            this.lblFileProgress.Size = new System.Drawing.Size(48, 13);
            this.lblFileProgress.TabIndex = 6;
            this.lblFileProgress.Text = "Progress";
            // 
            // tbFile
            // 
            this.tbFile.Location = new System.Drawing.Point(36, 167);
            this.tbFile.Name = "tbFile";
            this.tbFile.Size = new System.Drawing.Size(251, 20);
            this.tbFile.TabIndex = 0;
            this.tbFile.Visible = false;
            // 
            // btnStreamStart
            // 
            this.btnStreamStart.Image = global::GRBL_Plotter.Properties.Resources.btn_play;
            this.btnStreamStart.Location = new System.Drawing.Point(3, 13);
            this.btnStreamStart.Name = "btnStreamStart";
            this.btnStreamStart.Size = new System.Drawing.Size(51, 23);
            this.btnStreamStart.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnStreamStart, "Send code from text-editor to GRBL");
            this.btnStreamStart.UseVisualStyleBackColor = true;
            this.btnStreamStart.Click += new System.EventHandler(this.btnStreamStart_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(157, 165);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(27, 23);
            this.btnOpenFile.TabIndex = 1;
            this.btnOpenFile.Text = "...";
            this.toolTip1.SetToolTip(this.btnOpenFile, "Select file to load");
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Visible = false;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // cmsCode
            // 
            this.cmsCode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsCodeSelect,
            this.cmsCodeCopy,
            this.cmsCodePaste,
            this.cmsCodeSendLine});
            this.cmsCode.Name = "cmsCode";
            this.cmsCode.ShowImageMargin = false;
            this.cmsCode.Size = new System.Drawing.Size(101, 92);
            this.cmsCode.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsCode_ItemClicked);
            // 
            // cmsCodeSelect
            // 
            this.cmsCodeSelect.Name = "cmsCodeSelect";
            this.cmsCodeSelect.Size = new System.Drawing.Size(100, 22);
            this.cmsCodeSelect.Text = "Select All";
            // 
            // cmsCodeCopy
            // 
            this.cmsCodeCopy.Name = "cmsCodeCopy";
            this.cmsCodeCopy.Size = new System.Drawing.Size(100, 22);
            this.cmsCodeCopy.Text = "Copy";
            // 
            // cmsCodePaste
            // 
            this.cmsCodePaste.Name = "cmsCodePaste";
            this.cmsCodePaste.Size = new System.Drawing.Size(100, 22);
            this.cmsCodePaste.Text = "Paste";
            // 
            // cmsCodeSendLine
            // 
            this.cmsCodeSendLine.Name = "cmsCodeSendLine";
            this.cmsCodeSendLine.Size = new System.Drawing.Size(100, 22);
            this.cmsCodeSendLine.Text = "Send Line";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnHome);
            this.groupBox2.Controls.Add(this.btnZeroXYZ);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btnZeroXY);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnZeroZ);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnZeroY);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnZeroX);
            this.groupBox2.Controls.Add(this.label_status);
            this.groupBox2.Controls.Add(this.label_mx);
            this.groupBox2.Controls.Add(this.label_my);
            this.groupBox2.Controls.Add(this.label_mz);
            this.groupBox2.Controls.Add(this.label_wz);
            this.groupBox2.Controls.Add(this.label_wx);
            this.groupBox2.Controls.Add(this.label_wy);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 140);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tool Coordinates (World / Machine)";
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(122, 107);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(117, 30);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnZeroXYZ
            // 
            this.btnZeroXYZ.Location = new System.Drawing.Point(171, 62);
            this.btnZeroXYZ.Name = "btnZeroXYZ";
            this.btnZeroXYZ.Size = new System.Drawing.Size(68, 23);
            this.btnZeroXYZ.TabIndex = 5;
            this.btnZeroXYZ.Text = "Zero XYZ";
            this.btnZeroXYZ.UseVisualStyleBackColor = true;
            this.btnZeroXYZ.Click += new System.EventHandler(this.btnZeroXYZ_Click);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label5.Location = new System.Drawing.Point(0, 98);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(62, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Status:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnZeroXY
            // 
            this.btnZeroXY.Location = new System.Drawing.Point(171, 27);
            this.btnZeroXY.Name = "btnZeroXY";
            this.btnZeroXY.Size = new System.Drawing.Size(68, 23);
            this.btnZeroXY.TabIndex = 4;
            this.btnZeroXY.Text = "Zero XY";
            this.btnZeroXY.UseVisualStyleBackColor = true;
            this.btnZeroXY.Click += new System.EventHandler(this.btnZeroXY_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label4.Location = new System.Drawing.Point(13, 78);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(25, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Z";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnZeroZ
            // 
            this.btnZeroZ.Location = new System.Drawing.Point(122, 78);
            this.btnZeroZ.Name = "btnZeroZ";
            this.btnZeroZ.Size = new System.Drawing.Size(50, 23);
            this.btnZeroZ.TabIndex = 3;
            this.btnZeroZ.Text = "Zero Z";
            this.btnZeroZ.UseVisualStyleBackColor = true;
            this.btnZeroZ.Click += new System.EventHandler(this.btnZeroZ_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label3.Location = new System.Drawing.Point(15, 45);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(23, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Y";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnZeroY
            // 
            this.btnZeroY.Location = new System.Drawing.Point(122, 45);
            this.btnZeroY.Name = "btnZeroY";
            this.btnZeroY.Size = new System.Drawing.Size(50, 23);
            this.btnZeroY.TabIndex = 2;
            this.btnZeroY.Text = "Zero Y";
            this.btnZeroY.UseVisualStyleBackColor = true;
            this.btnZeroY.Click += new System.EventHandler(this.btnZeroY_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label2.Location = new System.Drawing.Point(15, 13);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(23, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "X";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnZeroX
            // 
            this.btnZeroX.Location = new System.Drawing.Point(122, 12);
            this.btnZeroX.Name = "btnZeroX";
            this.btnZeroX.Size = new System.Drawing.Size(50, 23);
            this.btnZeroX.TabIndex = 1;
            this.btnZeroX.Text = "Zero X";
            this.btnZeroX.UseVisualStyleBackColor = true;
            this.btnZeroX.Click += new System.EventHandler(this.btnZeroX_Click);
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_status.Location = new System.Drawing.Point(2, 119);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(56, 20);
            this.label_status.TabIndex = 7;
            this.label_status.Text = "Status";
            // 
            // btnJogZeroXY
            // 
            this.btnJogZeroXY.Location = new System.Drawing.Point(4, 35);
            this.btnJogZeroXY.Name = "btnJogZeroXY";
            this.btnJogZeroXY.Size = new System.Drawing.Size(81, 20);
            this.btnJogZeroXY.TabIndex = 5;
            this.btnJogZeroXY.Text = "Move XY=0";
            this.toolTip1.SetToolTip(this.btnJogZeroXY, "Send G90 G0 X0 Y0");
            this.btnJogZeroXY.UseVisualStyleBackColor = true;
            this.btnJogZeroXY.Click += new System.EventHandler(this.btnJogXY_Click);
            // 
            // btnJogZeroZ
            // 
            this.btnJogZeroZ.Location = new System.Drawing.Point(86, 14);
            this.btnJogZeroZ.Name = "btnJogZeroZ";
            this.btnJogZeroZ.Size = new System.Drawing.Size(40, 20);
            this.btnJogZeroZ.TabIndex = 4;
            this.btnJogZeroZ.Text = "Z=0";
            this.toolTip1.SetToolTip(this.btnJogZeroZ, "Send G90 G0 Z0");
            this.btnJogZeroZ.UseVisualStyleBackColor = true;
            this.btnJogZeroZ.Click += new System.EventHandler(this.btnJogZ_Click);
            // 
            // btnJogZeroY
            // 
            this.btnJogZeroY.Location = new System.Drawing.Point(45, 14);
            this.btnJogZeroY.Name = "btnJogZeroY";
            this.btnJogZeroY.Size = new System.Drawing.Size(40, 20);
            this.btnJogZeroY.TabIndex = 3;
            this.btnJogZeroY.Text = "Y=0";
            this.toolTip1.SetToolTip(this.btnJogZeroY, "Send G90 G0 Y0");
            this.btnJogZeroY.UseVisualStyleBackColor = true;
            this.btnJogZeroY.Click += new System.EventHandler(this.btnJogY_Click);
            // 
            // btnJogZeroX
            // 
            this.btnJogZeroX.Location = new System.Drawing.Point(4, 14);
            this.btnJogZeroX.Name = "btnJogZeroX";
            this.btnJogZeroX.Size = new System.Drawing.Size(40, 20);
            this.btnJogZeroX.TabIndex = 2;
            this.btnJogZeroX.Text = "X=0";
            this.toolTip1.SetToolTip(this.btnJogZeroX, "Send G90 G0 X0");
            this.btnJogZeroX.UseVisualStyleBackColor = true;
            this.btnJogZeroX.Click += new System.EventHandler(this.btnJogX_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tableLayoutPanel1);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(254, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(315, 140);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Custom Buttons";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnCustom8, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom7, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom5, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom4, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(309, 121);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnCustom8
            // 
            this.btnCustom8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom8.Location = new System.Drawing.Point(157, 91);
            this.btnCustom8.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom8.Name = "btnCustom8";
            this.btnCustom8.Size = new System.Drawing.Size(149, 29);
            this.btnCustom8.TabIndex = 7;
            this.btnCustom8.Text = "button12";
            this.btnCustom8.UseVisualStyleBackColor = true;
            this.btnCustom8.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom7
            // 
            this.btnCustom7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom7.Location = new System.Drawing.Point(157, 61);
            this.btnCustom7.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom7.Name = "btnCustom7";
            this.btnCustom7.Size = new System.Drawing.Size(149, 28);
            this.btnCustom7.TabIndex = 6;
            this.btnCustom7.Text = "button12";
            this.btnCustom7.UseVisualStyleBackColor = true;
            this.btnCustom7.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom6
            // 
            this.btnCustom6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom6.Location = new System.Drawing.Point(157, 31);
            this.btnCustom6.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom6.Name = "btnCustom6";
            this.btnCustom6.Size = new System.Drawing.Size(149, 28);
            this.btnCustom6.TabIndex = 5;
            this.btnCustom6.Text = "button12";
            this.btnCustom6.UseVisualStyleBackColor = true;
            this.btnCustom6.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom5
            // 
            this.btnCustom5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom5.Location = new System.Drawing.Point(157, 1);
            this.btnCustom5.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom5.Name = "btnCustom5";
            this.btnCustom5.Size = new System.Drawing.Size(149, 28);
            this.btnCustom5.TabIndex = 4;
            this.btnCustom5.Text = "button11";
            this.btnCustom5.UseVisualStyleBackColor = true;
            this.btnCustom5.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom1
            // 
            this.btnCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom1.Location = new System.Drawing.Point(3, 1);
            this.btnCustom1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom1.Name = "btnCustom1";
            this.btnCustom1.Size = new System.Drawing.Size(148, 28);
            this.btnCustom1.TabIndex = 0;
            this.btnCustom1.Text = "button7";
            this.btnCustom1.UseVisualStyleBackColor = true;
            this.btnCustom1.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom2
            // 
            this.btnCustom2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom2.Location = new System.Drawing.Point(3, 31);
            this.btnCustom2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom2.Name = "btnCustom2";
            this.btnCustom2.Size = new System.Drawing.Size(148, 28);
            this.btnCustom2.TabIndex = 1;
            this.btnCustom2.Text = "button8";
            this.btnCustom2.UseVisualStyleBackColor = true;
            this.btnCustom2.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom3
            // 
            this.btnCustom3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom3.Location = new System.Drawing.Point(3, 61);
            this.btnCustom3.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom3.Name = "btnCustom3";
            this.btnCustom3.Size = new System.Drawing.Size(148, 28);
            this.btnCustom3.TabIndex = 2;
            this.btnCustom3.Text = "button9";
            this.btnCustom3.UseVisualStyleBackColor = true;
            this.btnCustom3.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom4
            // 
            this.btnCustom4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCustom4.Location = new System.Drawing.Point(3, 91);
            this.btnCustom4.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.btnCustom4.Name = "btnCustom4";
            this.btnCustom4.Size = new System.Drawing.Size(148, 29);
            this.btnCustom4.TabIndex = 3;
            this.btnCustom4.Text = "button10";
            this.btnCustom4.UseVisualStyleBackColor = true;
            this.btnCustom4.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnFeedHold
            // 
            this.btnFeedHold.Location = new System.Drawing.Point(3, 305);
            this.btnFeedHold.Name = "btnFeedHold";
            this.btnFeedHold.Size = new System.Drawing.Size(100, 23);
            this.btnFeedHold.TabIndex = 16;
            this.btnFeedHold.Text = "Feed Hold";
            this.toolTip1.SetToolTip(this.btnFeedHold, resources.GetString("btnFeedHold.ToolTip"));
            this.btnFeedHold.UseVisualStyleBackColor = true;
            this.btnFeedHold.Click += new System.EventHandler(this.btnFeedHold_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(106, 305);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 43);
            this.btnReset.TabIndex = 15;
            this.btnReset.Text = "RESET";
            this.toolTip1.SetToolTip(this.btnReset, resources.GetString("btnReset.ToolTip"));
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(3, 330);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(100, 43);
            this.btnResume.TabIndex = 17;
            this.btnResume.Text = "Resume";
            this.toolTip1.SetToolTip(this.btnResume, resources.GetString("btnResume.ToolTip"));
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnKillAlarm
            // 
            this.btnKillAlarm.Location = new System.Drawing.Point(106, 350);
            this.btnKillAlarm.Name = "btnKillAlarm";
            this.btnKillAlarm.Size = new System.Drawing.Size(120, 23);
            this.btnKillAlarm.TabIndex = 18;
            this.btnKillAlarm.Text = "Kill Alarm";
            this.toolTip1.SetToolTip(this.btnKillAlarm, resources.GetString("btnKillAlarm.ToolTip"));
            this.btnKillAlarm.UseVisualStyleBackColor = true;
            this.btnKillAlarm.Click += new System.EventHandler(this.btnKillAlarm_Click);
            // 
            // tBURL
            // 
            this.tBURL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tBURL.Location = new System.Drawing.Point(3, 357);
            this.tBURL.Name = "tBURL";
            this.tBURL.Size = new System.Drawing.Size(330, 20);
            this.tBURL.TabIndex = 0;
            this.tBURL.Text = "Paste URL of SVG file here";
            this.toolTip1.SetToolTip(this.tBURL, "Check\r\nhttps://openclipart.org/\r\nfor SVG files");
            this.tBURL.TextChanged += new System.EventHandler(this.tBURL_TextChanged);
            // 
            // btnJogStop
            // 
            this.btnJogStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnJogStop.Location = new System.Drawing.Point(136, 69);
            this.btnJogStop.Name = "btnJogStop";
            this.btnJogStop.Size = new System.Drawing.Size(91, 41);
            this.btnJogStop.TabIndex = 28;
            this.btnJogStop.Text = "STOP Jogging";
            this.toolTip1.SetToolTip(this.btnJogStop, "Stop current jogging motion");
            this.btnJogStop.UseVisualStyleBackColor = false;
            this.btnJogStop.Click += new System.EventHandler(this.btnJogStop_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = global::GRBL_Plotter.Properties.Resources.modell;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ContextMenuStrip = this.cmsPictureBox;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(330, 348);
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // cmsPictureBox
            // 
            this.cmsPictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deletenotMarkToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteThisCodeLineToolStripMenuItem,
            this.moveToFirstPosToolStripMenuItem,
            this.deletePathToolStripMenuItem});
            this.cmsPictureBox.Name = "cmsPictureBox";
            this.cmsPictureBox.Size = new System.Drawing.Size(215, 98);
            // 
            // deletenotMarkToolStripMenuItem
            // 
            this.deletenotMarkToolStripMenuItem.Name = "deletenotMarkToolStripMenuItem";
            this.deletenotMarkToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.deletenotMarkToolStripMenuItem.Text = "Mark code (no delete)";
            this.deletenotMarkToolStripMenuItem.ToolTipText = "Click to toggle";
            this.deletenotMarkToolStripMenuItem.Click += new System.EventHandler(this.deletenotMarkToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(211, 6);
            // 
            // deleteThisCodeLineToolStripMenuItem
            // 
            this.deleteThisCodeLineToolStripMenuItem.Name = "deleteThisCodeLineToolStripMenuItem";
            this.deleteThisCodeLineToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.deleteThisCodeLineToolStripMenuItem.Text = "Delete this code line";
            this.deleteThisCodeLineToolStripMenuItem.ToolTipText = "Delete/Mark code line";
            this.deleteThisCodeLineToolStripMenuItem.Click += new System.EventHandler(this.deleteThisCodeLineToolStripMenuItem_Click);
            // 
            // moveToFirstPosToolStripMenuItem
            // 
            this.moveToFirstPosToolStripMenuItem.Name = "moveToFirstPosToolStripMenuItem";
            this.moveToFirstPosToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.moveToFirstPosToolStripMenuItem.Text = "Move to first pos.";
            this.moveToFirstPosToolStripMenuItem.ToolTipText = "Mark path from marked pos. to end (and move to beginning of path)";
            this.moveToFirstPosToolStripMenuItem.Click += new System.EventHandler(this.moveToFirstPosToolStripMenuItem_Click);
            // 
            // deletePathToolStripMenuItem
            // 
            this.deletePathToolStripMenuItem.Name = "deletePathToolStripMenuItem";
            this.deletePathToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.deletePathToolStripMenuItem.Text = "Delete corresponding path";
            this.deletePathToolStripMenuItem.ToolTipText = "Delete/Mark complete path of marked pos.";
            this.deletePathToolStripMenuItem.Click += new System.EventHandler(this.deletePathToolStripMenuItem_Click);
            // 
            // tLPAussen
            // 
            this.tLPAussen.AutoSize = true;
            this.tLPAussen.ColumnCount = 2;
            this.tLPAussen.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tLPAussen.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPAussen.Controls.Add(this.tLPLinks, 0, 0);
            this.tLPAussen.Controls.Add(this.tLPRechts, 1, 0);
            this.tLPAussen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPAussen.Location = new System.Drawing.Point(0, 24);
            this.tLPAussen.Name = "tLPAussen";
            this.tLPAussen.RowCount = 1;
            this.tLPAussen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPAussen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tLPAussen.Size = new System.Drawing.Size(884, 538);
            this.tLPAussen.TabIndex = 19;
            // 
            // tLPLinks
            // 
            this.tLPLinks.ColumnCount = 1;
            this.tLPLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tLPLinks.Controls.Add(this.fCTBCode, 0, 1);
            this.tLPLinks.Controls.Add(this.groupBox1, 0, 0);
            this.tLPLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPLinks.Location = new System.Drawing.Point(3, 3);
            this.tLPLinks.Name = "tLPLinks";
            this.tLPLinks.RowCount = 2;
            this.tLPLinks.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tLPLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPLinks.Size = new System.Drawing.Size(294, 532);
            this.tLPLinks.TabIndex = 19;
            // 
            // fCTBCode
            // 
            this.fCTBCode.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fCTBCode.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+\\s*(?<range>=)\\s*(?<range>[^;]+);";
            this.fCTBCode.AutoScrollMinSize = new System.Drawing.Size(193, 12);
            this.fCTBCode.BackBrush = null;
            this.fCTBCode.CharHeight = 12;
            this.fCTBCode.CharWidth = 7;
            this.fCTBCode.ContextMenuStrip = this.cmsCode;
            this.fCTBCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fCTBCode.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fCTBCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fCTBCode.Font = new System.Drawing.Font("Courier New", 8F);
            this.fCTBCode.IsReplaceMode = false;
            this.fCTBCode.Location = new System.Drawing.Point(3, 157);
            this.fCTBCode.Name = "fCTBCode";
            this.fCTBCode.Paddings = new System.Windows.Forms.Padding(0);
            this.fCTBCode.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fCTBCode.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fCTBCode.ServiceColors")));
            this.fCTBCode.Size = new System.Drawing.Size(288, 372);
            this.fCTBCode.TabIndex = 24;
            this.fCTBCode.Text = "Paste GCode or load file";
            this.fCTBCode.Zoom = 100;
            this.fCTBCode.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChanged);
            this.fCTBCode.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChangedDelayed);
            this.fCTBCode.Click += new System.EventHandler(this.fCTBCode_Click);
            this.fCTBCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fCTBCode_KeyDown);
            // 
            // tLPRechts
            // 
            this.tLPRechts.ColumnCount = 1;
            this.tLPRechts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPRechts.Controls.Add(this.tLPRechtsUnten, 0, 1);
            this.tLPRechts.Controls.Add(this.tLPRechtsOben, 0, 0);
            this.tLPRechts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPRechts.Location = new System.Drawing.Point(303, 3);
            this.tLPRechts.Name = "tLPRechts";
            this.tLPRechts.RowCount = 2;
            this.tLPRechts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this.tLPRechts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 248F));
            this.tLPRechts.Size = new System.Drawing.Size(578, 532);
            this.tLPRechts.TabIndex = 21;
            // 
            // tLPRechtsUnten
            // 
            this.tLPRechtsUnten.ColumnCount = 2;
            this.tLPRechtsUnten.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPRechtsUnten.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 236F));
            this.tLPRechtsUnten.Controls.Add(this.tLPRechtsUntenRechts, 1, 0);
            this.tLPRechtsUnten.Controls.Add(this.tLPMitteUnten, 0, 0);
            this.tLPRechtsUnten.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPRechtsUnten.Location = new System.Drawing.Point(0, 152);
            this.tLPRechtsUnten.Margin = new System.Windows.Forms.Padding(0);
            this.tLPRechtsUnten.Name = "tLPRechtsUnten";
            this.tLPRechtsUnten.RowCount = 1;
            this.tLPRechtsUnten.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tLPRechtsUnten.Size = new System.Drawing.Size(578, 380);
            this.tLPRechtsUnten.TabIndex = 12;
            // 
            // tLPRechtsUntenRechts
            // 
            this.tLPRechtsUntenRechts.ColumnCount = 1;
            this.tLPRechtsUntenRechts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tLPRechtsUntenRechts.Controls.Add(this.groupBox6, 0, 0);
            this.tLPRechtsUntenRechts.Location = new System.Drawing.Point(342, 0);
            this.tLPRechtsUntenRechts.Margin = new System.Windows.Forms.Padding(0);
            this.tLPRechtsUntenRechts.Name = "tLPRechtsUntenRechts";
            this.tLPRechtsUntenRechts.RowCount = 1;
            this.tLPRechtsUntenRechts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 379F));
            this.tLPRechtsUntenRechts.Size = new System.Drawing.Size(236, 379);
            this.tLPRechtsUntenRechts.TabIndex = 19;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnJogStop);
            this.groupBox6.Controls.Add(this.groupBox3);
            this.groupBox6.Controls.Add(this.lblTool);
            this.groupBox6.Controls.Add(this.cBTool);
            this.groupBox6.Controls.Add(this.virtualJoystickZ);
            this.groupBox6.Controls.Add(this.virtualJoystickXY);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.tBSpeed);
            this.groupBox6.Controls.Add(this.cBCoolant);
            this.groupBox6.Controls.Add(this.cBSpindle);
            this.groupBox6.Controls.Add(this.btnKillAlarm);
            this.groupBox6.Controls.Add(this.btnFeedHold);
            this.groupBox6.Controls.Add(this.btnResume);
            this.groupBox6.Controls.Add(this.btnReset);
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(230, 373);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Control / Jogging";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnJogZeroX);
            this.groupBox3.Controls.Add(this.btnJogZeroXY);
            this.groupBox3.Controls.Add(this.btnJogZeroY);
            this.groupBox3.Controls.Add(this.btnJogZeroZ);
            this.groupBox3.Location = new System.Drawing.Point(0, 55);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(130, 58);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Move to";
            // 
            // lblTool
            // 
            this.lblTool.Location = new System.Drawing.Point(115, 39);
            this.lblTool.Name = "lblTool";
            this.lblTool.Size = new System.Drawing.Size(110, 13);
            this.lblTool.TabIndex = 26;
            // 
            // cBTool
            // 
            this.cBTool.AutoSize = true;
            this.cBTool.Location = new System.Drawing.Point(3, 38);
            this.cBTool.Name = "cBTool";
            this.cBTool.Size = new System.Drawing.Size(106, 17);
            this.cBTool.TabIndex = 25;
            this.cBTool.Text = "Tool is in Spindle";
            this.cBTool.UseVisualStyleBackColor = true;
            this.cBTool.CheckedChanged += new System.EventHandler(this.cBTool_CheckedChanged);
            // 
            // virtualJoystickZ
            // 
            this.virtualJoystickZ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("virtualJoystickZ.BackgroundImage")));
            this.virtualJoystickZ.Joystick2Dimension = false;
            this.virtualJoystickZ.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickZ.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickZ.JoystickRaster = 5;
            this.virtualJoystickZ.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickZ.Location = new System.Drawing.Point(186, 119);
            this.virtualJoystickZ.MaximumSize = new System.Drawing.Size(400, 400);
            this.virtualJoystickZ.MinimumSize = new System.Drawing.Size(25, 100);
            this.virtualJoystickZ.Name = "virtualJoystickZ";
            this.virtualJoystickZ.Size = new System.Drawing.Size(40, 180);
            this.virtualJoystickZ.TabIndex = 24;
            this.virtualJoystickZ.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickZ_JoyStickEvent);
            this.virtualJoystickZ.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickZ.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // virtualJoystickXY
            // 
            this.virtualJoystickXY.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("virtualJoystickXY.BackgroundImage")));
            this.virtualJoystickXY.Joystick2Dimension = true;
            this.virtualJoystickXY.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickXY.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickXY.JoystickRaster = 5;
            this.virtualJoystickXY.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickXY.Location = new System.Drawing.Point(4, 119);
            this.virtualJoystickXY.MaximumSize = new System.Drawing.Size(400, 400);
            this.virtualJoystickXY.MinimumSize = new System.Drawing.Size(25, 100);
            this.virtualJoystickXY.Name = "virtualJoystickXY";
            this.virtualJoystickXY.Size = new System.Drawing.Size(180, 180);
            this.virtualJoystickXY.TabIndex = 23;
            this.virtualJoystickXY.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickXY_JoyStickEvent);
            this.virtualJoystickXY.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickXY.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(103, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Speed";
            // 
            // tBSpeed
            // 
            this.tBSpeed.Location = new System.Drawing.Point(64, 12);
            this.tBSpeed.Name = "tBSpeed";
            this.tBSpeed.Size = new System.Drawing.Size(39, 20);
            this.tBSpeed.TabIndex = 21;
            this.tBSpeed.Text = "1000";
            // 
            // cBCoolant
            // 
            this.cBCoolant.AutoSize = true;
            this.cBCoolant.Location = new System.Drawing.Point(165, 14);
            this.cBCoolant.Name = "cBCoolant";
            this.cBCoolant.Size = new System.Drawing.Size(62, 17);
            this.cBCoolant.TabIndex = 20;
            this.cBCoolant.Text = "Coolant";
            this.cBCoolant.UseVisualStyleBackColor = true;
            this.cBCoolant.CheckedChanged += new System.EventHandler(this.cBCoolant_CheckedChanged);
            // 
            // cBSpindle
            // 
            this.cBSpindle.AutoSize = true;
            this.cBSpindle.Location = new System.Drawing.Point(3, 15);
            this.cBSpindle.Name = "cBSpindle";
            this.cBSpindle.Size = new System.Drawing.Size(61, 17);
            this.cBSpindle.TabIndex = 19;
            this.cBSpindle.Text = "Spindle";
            this.cBSpindle.UseVisualStyleBackColor = true;
            this.cBSpindle.CheckedChanged += new System.EventHandler(this.cBSpindle_CheckedChanged);
            // 
            // tLPMitteUnten
            // 
            this.tLPMitteUnten.AutoSize = true;
            this.tLPMitteUnten.ColumnCount = 1;
            this.tLPMitteUnten.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPMitteUnten.Controls.Add(this.pictureBox1, 0, 0);
            this.tLPMitteUnten.Controls.Add(this.tBURL, 0, 1);
            this.tLPMitteUnten.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPMitteUnten.Location = new System.Drawing.Point(3, 3);
            this.tLPMitteUnten.Name = "tLPMitteUnten";
            this.tLPMitteUnten.RowCount = 2;
            this.tLPMitteUnten.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPMitteUnten.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tLPMitteUnten.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tLPMitteUnten.Size = new System.Drawing.Size(336, 374);
            this.tLPMitteUnten.TabIndex = 20;
            // 
            // tLPRechtsOben
            // 
            this.tLPRechtsOben.ColumnCount = 2;
            this.tLPRechtsOben.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 251F));
            this.tLPRechtsOben.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPRechtsOben.Controls.Add(this.groupBox5, 1, 0);
            this.tLPRechtsOben.Controls.Add(this.groupBox2, 0, 0);
            this.tLPRechtsOben.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPRechtsOben.Location = new System.Drawing.Point(3, 3);
            this.tLPRechtsOben.Name = "tLPRechtsOben";
            this.tLPRechtsOben.RowCount = 1;
            this.tLPRechtsOben.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tLPRechtsOben.Size = new System.Drawing.Size(572, 146);
            this.tLPRechtsOben.TabIndex = 13;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 562);
            this.Controls.Add(this.tLPAussen);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GRBL Plotter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.cmsScale.ResumeLayout(false);
            this.cmsCode.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.cmsPictureBox.ResumeLayout(false);
            this.tLPAussen.ResumeLayout(false);
            this.tLPLinks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).EndInit();
            this.tLPRechts.ResumeLayout(false);
            this.tLPRechtsUnten.ResumeLayout(false);
            this.tLPRechtsUnten.PerformLayout();
            this.tLPRechtsUntenRechts.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tLPMitteUnten.ResumeLayout(false);
            this.tLPMitteUnten.PerformLayout();
            this.tLPRechtsOben.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label_mx;
        private System.Windows.Forms.Label label_my;
        private System.Windows.Forms.Label label_mz;
        private System.Windows.Forms.Label label_wx;
        private System.Windows.Forms.Label label_wy;
        private System.Windows.Forms.Label label_wz;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar pbFile;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblFileProgress;
        private System.Windows.Forms.ProgressBar pbBuffer;
        private System.Windows.Forms.Button btnStreamStart;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.TextBox tbFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label lblRemaining;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnZeroXYZ;
        private System.Windows.Forms.Button btnZeroXY;
        private System.Windows.Forms.Button btnZeroZ;
        private System.Windows.Forms.Button btnZeroY;
        private System.Windows.Forms.Button btnZeroX;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnCustom6;
        private System.Windows.Forms.Button btnCustom5;
        private System.Windows.Forms.Button btnCustom4;
        private System.Windows.Forms.Button btnCustom3;
        private System.Windows.Forms.Button btnCustom2;
        private System.Windows.Forms.Button btnCustom1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnJogZeroXY;
        private System.Windows.Forms.Button btnJogZeroZ;
        private System.Windows.Forms.Button btnJogZeroY;
        private System.Windows.Forms.Button btnJogZeroX;
        private System.Windows.Forms.Button btnFeedHold;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.Label lbDimension;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbChangeAngle;
        private System.Windows.Forms.Button btnTransformCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbChangeSize;
        private System.Windows.Forms.ContextMenuStrip cmsCode;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeSelect;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeCopy;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeSendLine;
        private System.Windows.Forms.ToolStripMenuItem cmsCodePaste;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tLPAussen;
        private System.Windows.Forms.TableLayoutPanel tLPLinks;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TableLayoutPanel tLPRechts;
        private System.Windows.Forms.TableLayoutPanel tLPRechtsUnten;
        private System.Windows.Forms.TableLayoutPanel tLPRechtsUntenRechts;
        private System.Windows.Forms.TableLayoutPanel tLPRechtsOben;
        private FastColoredTextBoxNS.FastColoredTextBox fCTBCode;
        private System.Windows.Forms.Button btnKillAlarm;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCustom8;
        private System.Windows.Forms.Button btnCustom7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tBSpeed;
        private System.Windows.Forms.CheckBox cBCoolant;
        private System.Windows.Forms.CheckBox cBSpindle;
        private System.Windows.Forms.ContextMenuStrip cmsScale;
        private System.Windows.Forms.ToolStripMenuItem scaleToWidthOfToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripScaleTextBoxWidth;
        private System.Windows.Forms.ToolStripMenuItem scaleToHeightOfToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripScaleTextBoxHeight;
        private System.Windows.Forms.Button btnShiftToZero;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tLPMitteUnten;
        private System.Windows.Forms.TextBox tBURL;
        private System.Windows.Forms.Button btnMirrorY;
        private System.Windows.Forms.Button btnMirrorX;
        private virtualJoystick.virtualJoystick virtualJoystickZ;
        private virtualJoystick.virtualJoystick virtualJoystickXY;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnStreamCheck;
        private System.Windows.Forms.Button btnStreamStop;
        private System.Windows.Forms.ContextMenuStrip cmsPictureBox;
        private System.Windows.Forms.ToolStripMenuItem moveToFirstPosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteThisCodeLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deletenotMarkToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBTool;
        private System.Windows.Forms.Label lblTool;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem machineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlStreamingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem createGCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textWizzardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createSimpleShapesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMachineParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMachineParametersToolStripMenuItem;
        private System.Windows.Forms.Button btnJogStop;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ToolStripMenuItem control2ndGRBLToolStripMenuItem;
    }
}

