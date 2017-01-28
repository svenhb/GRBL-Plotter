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
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.deutschToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.fCTBCode = new FastColoredTextBoxNS.FastColoredTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmsPictureBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deletenotMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteThisCodeLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToFirstPosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tLPAussen = new System.Windows.Forms.TableLayoutPanel();
            this.tLPLinks = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechts = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUnten = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUntenRechts = new System.Windows.Forms.TableLayoutPanel();
            this.tLPMitteUnten = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsOben = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.cmsScale.SuspendLayout();
            this.cmsCode.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.cmsPictureBox.SuspendLayout();
            this.tLPAussen.SuspendLayout();
            this.tLPLinks.SuspendLayout();
            this.tLPRechts.SuspendLayout();
            this.tLPRechtsUnten.SuspendLayout();
            this.tLPRechtsUntenRechts.SuspendLayout();
            this.tLPMitteUnten.SuspendLayout();
            this.tLPRechtsOben.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.machineToolStripMenuItem,
            this.createGCodeToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.toolTip1.SetToolTip(this.menuStrip1, resources.GetString("menuStrip1.ToolTip"));
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.toolStripSeparator3,
            this.saveMachineParametersToolStripMenuItem,
            this.loadMachineParametersToolStripMenuItem,
            this.toolStripMenuItem3});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // loadToolStripMenuItem
            // 
            resources.ApplyResources(this.loadToolStripMenuItem, "loadToolStripMenuItem");
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // saveMachineParametersToolStripMenuItem
            // 
            resources.ApplyResources(this.saveMachineParametersToolStripMenuItem, "saveMachineParametersToolStripMenuItem");
            this.saveMachineParametersToolStripMenuItem.Name = "saveMachineParametersToolStripMenuItem";
            this.saveMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.saveMachineParametersToolStripMenuItem_Click);
            // 
            // loadMachineParametersToolStripMenuItem
            // 
            resources.ApplyResources(this.loadMachineParametersToolStripMenuItem, "loadMachineParametersToolStripMenuItem");
            this.loadMachineParametersToolStripMenuItem.Name = "loadMachineParametersToolStripMenuItem";
            this.loadMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.loadMachineParametersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.deutschToolStripMenuItem});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            // 
            // toolStripMenuItem4
            // 
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // deutschToolStripMenuItem
            // 
            resources.ApplyResources(this.deutschToolStripMenuItem, "deutschToolStripMenuItem");
            this.deutschToolStripMenuItem.Name = "deutschToolStripMenuItem";
            this.deutschToolStripMenuItem.Click += new System.EventHandler(this.deutschToolStripMenuItem_Click);
            // 
            // machineToolStripMenuItem
            // 
            resources.ApplyResources(this.machineToolStripMenuItem, "machineToolStripMenuItem");
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlStreamingToolStripMenuItem,
            this.control2ndGRBLToolStripMenuItem,
            this.toolStripMenuItem1,
            this.setupToolStripMenuItem});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            // 
            // controlStreamingToolStripMenuItem
            // 
            resources.ApplyResources(this.controlStreamingToolStripMenuItem, "controlStreamingToolStripMenuItem");
            this.controlStreamingToolStripMenuItem.Name = "controlStreamingToolStripMenuItem";
            this.controlStreamingToolStripMenuItem.Click += new System.EventHandler(this.controlStreamingToolStripMenuItem_Click);
            // 
            // control2ndGRBLToolStripMenuItem
            // 
            resources.ApplyResources(this.control2ndGRBLToolStripMenuItem, "control2ndGRBLToolStripMenuItem");
            this.control2ndGRBLToolStripMenuItem.Name = "control2ndGRBLToolStripMenuItem";
            this.control2ndGRBLToolStripMenuItem.Click += new System.EventHandler(this.control2ndGRBLToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.cameraToolStripMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            resources.ApplyResources(this.setupToolStripMenuItem, "setupToolStripMenuItem");
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
            // 
            // createGCodeToolStripMenuItem
            // 
            resources.ApplyResources(this.createGCodeToolStripMenuItem, "createGCodeToolStripMenuItem");
            this.createGCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textWizzardToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.createSimpleShapesToolStripMenuItem});
            this.createGCodeToolStripMenuItem.Name = "createGCodeToolStripMenuItem";
            // 
            // textWizzardToolStripMenuItem
            // 
            resources.ApplyResources(this.textWizzardToolStripMenuItem, "textWizzardToolStripMenuItem");
            this.textWizzardToolStripMenuItem.Name = "textWizzardToolStripMenuItem";
            this.textWizzardToolStripMenuItem.Click += new System.EventHandler(this.textWizzardToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            resources.ApplyResources(this.imageToolStripMenuItem, "imageToolStripMenuItem");
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.imageToolStripMenuItem_Click);
            // 
            // createSimpleShapesToolStripMenuItem
            // 
            resources.ApplyResources(this.createSimpleShapesToolStripMenuItem, "createSimpleShapesToolStripMenuItem");
            this.createSimpleShapesToolStripMenuItem.Name = "createSimpleShapesToolStripMenuItem";
            this.createSimpleShapesToolStripMenuItem.Click += new System.EventHandler(this.createSimpleShapesToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // label_mx
            // 
            resources.ApplyResources(this.label_mx, "label_mx");
            this.label_mx.Name = "label_mx";
            this.toolTip1.SetToolTip(this.label_mx, resources.GetString("label_mx.ToolTip"));
            // 
            // label_my
            // 
            resources.ApplyResources(this.label_my, "label_my");
            this.label_my.Name = "label_my";
            this.toolTip1.SetToolTip(this.label_my, resources.GetString("label_my.ToolTip"));
            // 
            // label_mz
            // 
            resources.ApplyResources(this.label_mz, "label_mz");
            this.label_mz.Name = "label_mz";
            this.toolTip1.SetToolTip(this.label_mz, resources.GetString("label_mz.ToolTip"));
            // 
            // label_wx
            // 
            resources.ApplyResources(this.label_wx, "label_wx");
            this.label_wx.Name = "label_wx";
            this.toolTip1.SetToolTip(this.label_wx, resources.GetString("label_wx.ToolTip"));
            // 
            // label_wy
            // 
            resources.ApplyResources(this.label_wy, "label_wy");
            this.label_wy.Name = "label_wy";
            this.toolTip1.SetToolTip(this.label_wy, resources.GetString("label_wy.ToolTip"));
            // 
            // label_wz
            // 
            resources.ApplyResources(this.label_wz, "label_wz");
            this.label_wz.Name = "label_wz";
            this.toolTip1.SetToolTip(this.label_wz, resources.GetString("label_wz.ToolTip"));
            // 
            // MainTimer
            // 
            this.MainTimer.Enabled = true;
            this.MainTimer.Interval = 500;
            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
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
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // pbBuffer
            // 
            resources.ApplyResources(this.pbBuffer, "pbBuffer");
            this.pbBuffer.Name = "pbBuffer";
            this.toolTip1.SetToolTip(this.pbBuffer, resources.GetString("pbBuffer.ToolTip"));
            // 
            // btnStreamStop
            // 
            resources.ApplyResources(this.btnStreamStop, "btnStreamStop");
            this.btnStreamStop.Image = global::GRBL_Plotter.Properties.Resources.btn_stop;
            this.btnStreamStop.Name = "btnStreamStop";
            this.toolTip1.SetToolTip(this.btnStreamStop, resources.GetString("btnStreamStop.ToolTip"));
            this.btnStreamStop.UseVisualStyleBackColor = true;
            this.btnStreamStop.Click += new System.EventHandler(this.btnStreamStop_Click);
            // 
            // btnStreamCheck
            // 
            resources.ApplyResources(this.btnStreamCheck, "btnStreamCheck");
            this.btnStreamCheck.Name = "btnStreamCheck";
            this.toolTip1.SetToolTip(this.btnStreamCheck, resources.GetString("btnStreamCheck.ToolTip"));
            this.btnStreamCheck.UseVisualStyleBackColor = true;
            this.btnStreamCheck.Click += new System.EventHandler(this.btnStreamCheck_Click);
            // 
            // btnMirrorY
            // 
            resources.ApplyResources(this.btnMirrorY, "btnMirrorY");
            this.btnMirrorY.Name = "btnMirrorY";
            this.toolTip1.SetToolTip(this.btnMirrorY, resources.GetString("btnMirrorY.ToolTip"));
            this.btnMirrorY.UseVisualStyleBackColor = true;
            this.btnMirrorY.Click += new System.EventHandler(this.btnShiftToZero_MirrorXY_Click);
            // 
            // btnMirrorX
            // 
            resources.ApplyResources(this.btnMirrorX, "btnMirrorX");
            this.btnMirrorX.Name = "btnMirrorX";
            this.toolTip1.SetToolTip(this.btnMirrorX, resources.GetString("btnMirrorX.ToolTip"));
            this.btnMirrorX.UseVisualStyleBackColor = true;
            this.btnMirrorX.Click += new System.EventHandler(this.btnShiftToZero_MirrorXY_Click);
            // 
            // btnShiftToZero
            // 
            resources.ApplyResources(this.btnShiftToZero, "btnShiftToZero");
            this.btnShiftToZero.Name = "btnShiftToZero";
            this.toolTip1.SetToolTip(this.btnShiftToZero, resources.GetString("btnShiftToZero.ToolTip"));
            this.btnShiftToZero.UseVisualStyleBackColor = true;
            this.btnShiftToZero.Click += new System.EventHandler(this.btnShiftToZero_MirrorXY_Click);
            // 
            // btnTransformCode
            // 
            resources.ApplyResources(this.btnTransformCode, "btnTransformCode");
            this.btnTransformCode.Name = "btnTransformCode";
            this.toolTip1.SetToolTip(this.btnTransformCode, resources.GetString("btnTransformCode.ToolTip"));
            this.btnTransformCode.UseVisualStyleBackColor = true;
            this.btnTransformCode.Click += new System.EventHandler(this.btnTransformCode_Click);
            // 
            // tbChangeAngle
            // 
            resources.ApplyResources(this.tbChangeAngle, "tbChangeAngle");
            this.tbChangeAngle.Name = "tbChangeAngle";
            this.toolTip1.SetToolTip(this.tbChangeAngle, resources.GetString("tbChangeAngle.ToolTip"));
            // 
            // tbChangeSize
            // 
            resources.ApplyResources(this.tbChangeSize, "tbChangeSize");
            this.tbChangeSize.ContextMenuStrip = this.cmsScale;
            this.tbChangeSize.Name = "tbChangeSize";
            this.toolTip1.SetToolTip(this.tbChangeSize, resources.GetString("tbChangeSize.ToolTip"));
            // 
            // cmsScale
            // 
            resources.ApplyResources(this.cmsScale, "cmsScale");
            this.cmsScale.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleToWidthOfToolStripMenuItem,
            this.scaleToHeightOfToolStripMenuItem});
            this.cmsScale.Name = "contextMenuStripScale";
            this.toolTip1.SetToolTip(this.cmsScale, resources.GetString("cmsScale.ToolTip"));
            // 
            // scaleToWidthOfToolStripMenuItem
            // 
            resources.ApplyResources(this.scaleToWidthOfToolStripMenuItem, "scaleToWidthOfToolStripMenuItem");
            this.scaleToWidthOfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripScaleTextBoxWidth});
            this.scaleToWidthOfToolStripMenuItem.Name = "scaleToWidthOfToolStripMenuItem";
            // 
            // toolStripScaleTextBoxWidth
            // 
            resources.ApplyResources(this.toolStripScaleTextBoxWidth, "toolStripScaleTextBoxWidth");
            this.toolStripScaleTextBoxWidth.Name = "toolStripScaleTextBoxWidth";
            this.toolStripScaleTextBoxWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripScaleTextBoxWidth_KeyDown);
            this.toolStripScaleTextBoxWidth.TextChanged += new System.EventHandler(this.toolStripScaleTextBox1_TextChanged);
            // 
            // scaleToHeightOfToolStripMenuItem
            // 
            resources.ApplyResources(this.scaleToHeightOfToolStripMenuItem, "scaleToHeightOfToolStripMenuItem");
            this.scaleToHeightOfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripScaleTextBoxHeight});
            this.scaleToHeightOfToolStripMenuItem.Name = "scaleToHeightOfToolStripMenuItem";
            // 
            // toolStripScaleTextBoxHeight
            // 
            resources.ApplyResources(this.toolStripScaleTextBoxHeight, "toolStripScaleTextBoxHeight");
            this.toolStripScaleTextBoxHeight.Name = "toolStripScaleTextBoxHeight";
            this.toolStripScaleTextBoxHeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripScaleTextBoxWidth_KeyDown);
            this.toolStripScaleTextBoxHeight.TextChanged += new System.EventHandler(this.toolStripScaleTextBoxHeight_TextChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // lbDimension
            // 
            resources.ApplyResources(this.lbDimension, "lbDimension");
            this.lbDimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbDimension.Name = "lbDimension";
            this.toolTip1.SetToolTip(this.lbDimension, resources.GetString("lbDimension.ToolTip"));
            // 
            // lbInfo
            // 
            resources.ApplyResources(this.lbInfo, "lbInfo");
            this.lbInfo.Name = "lbInfo";
            this.toolTip1.SetToolTip(this.lbInfo, resources.GetString("lbInfo.ToolTip"));
            // 
            // lblRemaining
            // 
            resources.ApplyResources(this.lblRemaining, "lblRemaining");
            this.lblRemaining.Name = "lblRemaining";
            this.toolTip1.SetToolTip(this.lblRemaining, resources.GetString("lblRemaining.ToolTip"));
            // 
            // pbFile
            // 
            resources.ApplyResources(this.pbFile, "pbFile");
            this.pbFile.Name = "pbFile";
            this.toolTip1.SetToolTip(this.pbFile, resources.GetString("pbFile.ToolTip"));
            // 
            // lblElapsed
            // 
            resources.ApplyResources(this.lblElapsed, "lblElapsed");
            this.lblElapsed.Name = "lblElapsed";
            this.toolTip1.SetToolTip(this.lblElapsed, resources.GetString("lblElapsed.ToolTip"));
            // 
            // lblFileProgress
            // 
            resources.ApplyResources(this.lblFileProgress, "lblFileProgress");
            this.lblFileProgress.Name = "lblFileProgress";
            this.toolTip1.SetToolTip(this.lblFileProgress, resources.GetString("lblFileProgress.ToolTip"));
            // 
            // tbFile
            // 
            resources.ApplyResources(this.tbFile, "tbFile");
            this.tbFile.Name = "tbFile";
            this.toolTip1.SetToolTip(this.tbFile, resources.GetString("tbFile.ToolTip"));
            // 
            // btnStreamStart
            // 
            resources.ApplyResources(this.btnStreamStart, "btnStreamStart");
            this.btnStreamStart.Image = global::GRBL_Plotter.Properties.Resources.btn_play;
            this.btnStreamStart.Name = "btnStreamStart";
            this.toolTip1.SetToolTip(this.btnStreamStart, resources.GetString("btnStreamStart.ToolTip"));
            this.btnStreamStart.UseVisualStyleBackColor = true;
            this.btnStreamStart.Click += new System.EventHandler(this.btnStreamStart_Click);
            // 
            // btnOpenFile
            // 
            resources.ApplyResources(this.btnOpenFile, "btnOpenFile");
            this.btnOpenFile.Name = "btnOpenFile";
            this.toolTip1.SetToolTip(this.btnOpenFile, resources.GetString("btnOpenFile.ToolTip"));
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // cmsCode
            // 
            resources.ApplyResources(this.cmsCode, "cmsCode");
            this.cmsCode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsCodeSelect,
            this.cmsCodeCopy,
            this.cmsCodePaste,
            this.cmsCodeSendLine});
            this.cmsCode.Name = "cmsCode";
            this.cmsCode.ShowImageMargin = false;
            this.toolTip1.SetToolTip(this.cmsCode, resources.GetString("cmsCode.ToolTip"));
            this.cmsCode.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsCode_ItemClicked);
            // 
            // cmsCodeSelect
            // 
            resources.ApplyResources(this.cmsCodeSelect, "cmsCodeSelect");
            this.cmsCodeSelect.Name = "cmsCodeSelect";
            // 
            // cmsCodeCopy
            // 
            resources.ApplyResources(this.cmsCodeCopy, "cmsCodeCopy");
            this.cmsCodeCopy.Name = "cmsCodeCopy";
            // 
            // cmsCodePaste
            // 
            resources.ApplyResources(this.cmsCodePaste, "cmsCodePaste");
            this.cmsCodePaste.Name = "cmsCodePaste";
            // 
            // cmsCodeSendLine
            // 
            resources.ApplyResources(this.cmsCodeSendLine, "cmsCodeSendLine");
            this.cmsCodeSendLine.Name = "cmsCodeSendLine";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
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
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // btnHome
            // 
            resources.ApplyResources(this.btnHome, "btnHome");
            this.btnHome.Name = "btnHome";
            this.toolTip1.SetToolTip(this.btnHome, resources.GetString("btnHome.ToolTip"));
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnZeroXYZ
            // 
            resources.ApplyResources(this.btnZeroXYZ, "btnZeroXYZ");
            this.btnZeroXYZ.Name = "btnZeroXYZ";
            this.toolTip1.SetToolTip(this.btnZeroXYZ, resources.GetString("btnZeroXYZ.ToolTip"));
            this.btnZeroXYZ.UseVisualStyleBackColor = true;
            this.btnZeroXYZ.Click += new System.EventHandler(this.btnZeroXYZ_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // btnZeroXY
            // 
            resources.ApplyResources(this.btnZeroXY, "btnZeroXY");
            this.btnZeroXY.Name = "btnZeroXY";
            this.toolTip1.SetToolTip(this.btnZeroXY, resources.GetString("btnZeroXY.ToolTip"));
            this.btnZeroXY.UseVisualStyleBackColor = true;
            this.btnZeroXY.Click += new System.EventHandler(this.btnZeroXY_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // btnZeroZ
            // 
            resources.ApplyResources(this.btnZeroZ, "btnZeroZ");
            this.btnZeroZ.Name = "btnZeroZ";
            this.toolTip1.SetToolTip(this.btnZeroZ, resources.GetString("btnZeroZ.ToolTip"));
            this.btnZeroZ.UseVisualStyleBackColor = true;
            this.btnZeroZ.Click += new System.EventHandler(this.btnZeroZ_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // btnZeroY
            // 
            resources.ApplyResources(this.btnZeroY, "btnZeroY");
            this.btnZeroY.Name = "btnZeroY";
            this.toolTip1.SetToolTip(this.btnZeroY, resources.GetString("btnZeroY.ToolTip"));
            this.btnZeroY.UseVisualStyleBackColor = true;
            this.btnZeroY.Click += new System.EventHandler(this.btnZeroY_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // btnZeroX
            // 
            resources.ApplyResources(this.btnZeroX, "btnZeroX");
            this.btnZeroX.Name = "btnZeroX";
            this.toolTip1.SetToolTip(this.btnZeroX, resources.GetString("btnZeroX.ToolTip"));
            this.btnZeroX.UseVisualStyleBackColor = true;
            this.btnZeroX.Click += new System.EventHandler(this.btnZeroX_Click);
            // 
            // label_status
            // 
            resources.ApplyResources(this.label_status, "label_status");
            this.label_status.Name = "label_status";
            this.toolTip1.SetToolTip(this.label_status, resources.GetString("label_status.ToolTip"));
            // 
            // btnJogZeroXY
            // 
            resources.ApplyResources(this.btnJogZeroXY, "btnJogZeroXY");
            this.btnJogZeroXY.Name = "btnJogZeroXY";
            this.toolTip1.SetToolTip(this.btnJogZeroXY, resources.GetString("btnJogZeroXY.ToolTip"));
            this.btnJogZeroXY.UseVisualStyleBackColor = true;
            this.btnJogZeroXY.Click += new System.EventHandler(this.btnJogXY_Click);
            // 
            // btnJogZeroZ
            // 
            resources.ApplyResources(this.btnJogZeroZ, "btnJogZeroZ");
            this.btnJogZeroZ.Name = "btnJogZeroZ";
            this.toolTip1.SetToolTip(this.btnJogZeroZ, resources.GetString("btnJogZeroZ.ToolTip"));
            this.btnJogZeroZ.UseVisualStyleBackColor = true;
            this.btnJogZeroZ.Click += new System.EventHandler(this.btnJogZ_Click);
            // 
            // btnJogZeroY
            // 
            resources.ApplyResources(this.btnJogZeroY, "btnJogZeroY");
            this.btnJogZeroY.Name = "btnJogZeroY";
            this.toolTip1.SetToolTip(this.btnJogZeroY, resources.GetString("btnJogZeroY.ToolTip"));
            this.btnJogZeroY.UseVisualStyleBackColor = true;
            this.btnJogZeroY.Click += new System.EventHandler(this.btnJogY_Click);
            // 
            // btnJogZeroX
            // 
            resources.ApplyResources(this.btnJogZeroX, "btnJogZeroX");
            this.btnJogZeroX.Name = "btnJogZeroX";
            this.toolTip1.SetToolTip(this.btnJogZeroX, resources.GetString("btnJogZeroX.ToolTip"));
            this.btnJogZeroX.UseVisualStyleBackColor = true;
            this.btnJogZeroX.Click += new System.EventHandler(this.btnJogX_Click);
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.tableLayoutPanel1);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox5, resources.GetString("groupBox5.ToolTip"));
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.btnCustom8, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom7, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom5, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCustom4, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.toolTip1.SetToolTip(this.tableLayoutPanel1, resources.GetString("tableLayoutPanel1.ToolTip"));
            // 
            // btnCustom8
            // 
            resources.ApplyResources(this.btnCustom8, "btnCustom8");
            this.btnCustom8.Name = "btnCustom8";
            this.toolTip1.SetToolTip(this.btnCustom8, resources.GetString("btnCustom8.ToolTip"));
            this.btnCustom8.UseVisualStyleBackColor = true;
            this.btnCustom8.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom7
            // 
            resources.ApplyResources(this.btnCustom7, "btnCustom7");
            this.btnCustom7.Name = "btnCustom7";
            this.toolTip1.SetToolTip(this.btnCustom7, resources.GetString("btnCustom7.ToolTip"));
            this.btnCustom7.UseVisualStyleBackColor = true;
            this.btnCustom7.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom6
            // 
            resources.ApplyResources(this.btnCustom6, "btnCustom6");
            this.btnCustom6.Name = "btnCustom6";
            this.toolTip1.SetToolTip(this.btnCustom6, resources.GetString("btnCustom6.ToolTip"));
            this.btnCustom6.UseVisualStyleBackColor = true;
            this.btnCustom6.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom5
            // 
            resources.ApplyResources(this.btnCustom5, "btnCustom5");
            this.btnCustom5.Name = "btnCustom5";
            this.toolTip1.SetToolTip(this.btnCustom5, resources.GetString("btnCustom5.ToolTip"));
            this.btnCustom5.UseVisualStyleBackColor = true;
            this.btnCustom5.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom1
            // 
            resources.ApplyResources(this.btnCustom1, "btnCustom1");
            this.btnCustom1.Name = "btnCustom1";
            this.toolTip1.SetToolTip(this.btnCustom1, resources.GetString("btnCustom1.ToolTip"));
            this.btnCustom1.UseVisualStyleBackColor = true;
            this.btnCustom1.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom2
            // 
            resources.ApplyResources(this.btnCustom2, "btnCustom2");
            this.btnCustom2.Name = "btnCustom2";
            this.toolTip1.SetToolTip(this.btnCustom2, resources.GetString("btnCustom2.ToolTip"));
            this.btnCustom2.UseVisualStyleBackColor = true;
            this.btnCustom2.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom3
            // 
            resources.ApplyResources(this.btnCustom3, "btnCustom3");
            this.btnCustom3.Name = "btnCustom3";
            this.toolTip1.SetToolTip(this.btnCustom3, resources.GetString("btnCustom3.ToolTip"));
            this.btnCustom3.UseVisualStyleBackColor = true;
            this.btnCustom3.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom4
            // 
            resources.ApplyResources(this.btnCustom4, "btnCustom4");
            this.btnCustom4.Name = "btnCustom4";
            this.toolTip1.SetToolTip(this.btnCustom4, resources.GetString("btnCustom4.ToolTip"));
            this.btnCustom4.UseVisualStyleBackColor = true;
            this.btnCustom4.Click += new System.EventHandler(this.btnCustomButton_Click);
            // 
            // btnFeedHold
            // 
            resources.ApplyResources(this.btnFeedHold, "btnFeedHold");
            this.btnFeedHold.Name = "btnFeedHold";
            this.toolTip1.SetToolTip(this.btnFeedHold, resources.GetString("btnFeedHold.ToolTip"));
            this.btnFeedHold.UseVisualStyleBackColor = true;
            this.btnFeedHold.Click += new System.EventHandler(this.btnFeedHold_Click);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Name = "btnReset";
            this.toolTip1.SetToolTip(this.btnReset, resources.GetString("btnReset.ToolTip"));
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnResume
            // 
            resources.ApplyResources(this.btnResume, "btnResume");
            this.btnResume.Name = "btnResume";
            this.toolTip1.SetToolTip(this.btnResume, resources.GetString("btnResume.ToolTip"));
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnKillAlarm
            // 
            resources.ApplyResources(this.btnKillAlarm, "btnKillAlarm");
            this.btnKillAlarm.Name = "btnKillAlarm";
            this.toolTip1.SetToolTip(this.btnKillAlarm, resources.GetString("btnKillAlarm.ToolTip"));
            this.btnKillAlarm.UseVisualStyleBackColor = true;
            this.btnKillAlarm.Click += new System.EventHandler(this.btnKillAlarm_Click);
            // 
            // tBURL
            // 
            resources.ApplyResources(this.tBURL, "tBURL");
            this.tBURL.Name = "tBURL";
            this.toolTip1.SetToolTip(this.tBURL, resources.GetString("tBURL.ToolTip"));
            this.tBURL.TextChanged += new System.EventHandler(this.tBURL_TextChanged);
            // 
            // btnJogStop
            // 
            resources.ApplyResources(this.btnJogStop, "btnJogStop");
            this.btnJogStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnJogStop.Name = "btnJogStop";
            this.toolTip1.SetToolTip(this.btnJogStop, resources.GetString("btnJogStop.ToolTip"));
            this.btnJogStop.UseVisualStyleBackColor = false;
            this.btnJogStop.Click += new System.EventHandler(this.btnJogStop_Click);
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
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
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.btnJogZeroX);
            this.groupBox3.Controls.Add(this.btnJogZeroXY);
            this.groupBox3.Controls.Add(this.btnJogZeroY);
            this.groupBox3.Controls.Add(this.btnJogZeroZ);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // lblTool
            // 
            resources.ApplyResources(this.lblTool, "lblTool");
            this.lblTool.Name = "lblTool";
            this.toolTip1.SetToolTip(this.lblTool, resources.GetString("lblTool.ToolTip"));
            // 
            // cBTool
            // 
            resources.ApplyResources(this.cBTool, "cBTool");
            this.cBTool.Name = "cBTool";
            this.toolTip1.SetToolTip(this.cBTool, resources.GetString("cBTool.ToolTip"));
            this.cBTool.UseVisualStyleBackColor = true;
            this.cBTool.CheckedChanged += new System.EventHandler(this.cBTool_CheckedChanged);
            // 
            // virtualJoystickZ
            // 
            resources.ApplyResources(this.virtualJoystickZ, "virtualJoystickZ");
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
            this.virtualJoystickZ.Name = "virtualJoystickZ";
            this.toolTip1.SetToolTip(this.virtualJoystickZ, resources.GetString("virtualJoystickZ.ToolTip"));
            this.virtualJoystickZ.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickZ_JoyStickEvent);
            this.virtualJoystickZ.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickZ.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // virtualJoystickXY
            // 
            resources.ApplyResources(this.virtualJoystickXY, "virtualJoystickXY");
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
            this.virtualJoystickXY.Name = "virtualJoystickXY";
            this.toolTip1.SetToolTip(this.virtualJoystickXY, resources.GetString("virtualJoystickXY.ToolTip"));
            this.virtualJoystickXY.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickXY_JoyStickEvent);
            this.virtualJoystickXY.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickXY.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            this.toolTip1.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // tBSpeed
            // 
            resources.ApplyResources(this.tBSpeed, "tBSpeed");
            this.tBSpeed.Name = "tBSpeed";
            this.toolTip1.SetToolTip(this.tBSpeed, resources.GetString("tBSpeed.ToolTip"));
            // 
            // cBCoolant
            // 
            resources.ApplyResources(this.cBCoolant, "cBCoolant");
            this.cBCoolant.Name = "cBCoolant";
            this.toolTip1.SetToolTip(this.cBCoolant, resources.GetString("cBCoolant.ToolTip"));
            this.cBCoolant.UseVisualStyleBackColor = true;
            this.cBCoolant.CheckedChanged += new System.EventHandler(this.cBCoolant_CheckedChanged);
            // 
            // cBSpindle
            // 
            resources.ApplyResources(this.cBSpindle, "cBSpindle");
            this.cBSpindle.Name = "cBSpindle";
            this.toolTip1.SetToolTip(this.cBSpindle, resources.GetString("cBSpindle.ToolTip"));
            this.cBSpindle.UseVisualStyleBackColor = true;
            this.cBSpindle.CheckedChanged += new System.EventHandler(this.cBSpindle_CheckedChanged);
            // 
            // fCTBCode
            // 
            resources.ApplyResources(this.fCTBCode, "fCTBCode");
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
            this.fCTBCode.BackBrush = null;
            this.fCTBCode.CharHeight = 12;
            this.fCTBCode.CharWidth = 7;
            this.fCTBCode.ContextMenuStrip = this.cmsCode;
            this.fCTBCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fCTBCode.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fCTBCode.IsReplaceMode = false;
            this.fCTBCode.Name = "fCTBCode";
            this.fCTBCode.Paddings = new System.Windows.Forms.Padding(0);
            this.fCTBCode.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fCTBCode.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fCTBCode.ServiceColors")));
            this.fCTBCode.ToolTip = null;
            this.toolTip1.SetToolTip(this.fCTBCode, resources.GetString("fCTBCode.ToolTip"));
            this.fCTBCode.Zoom = 100;
            this.fCTBCode.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChanged);
            this.fCTBCode.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChangedDelayed);
            this.fCTBCode.Click += new System.EventHandler(this.fCTBCode_Click);
            this.fCTBCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fCTBCode_KeyDown);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = global::GRBL_Plotter.Properties.Resources.modell;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ContextMenuStrip = this.cmsPictureBox;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, resources.GetString("pictureBox1.ToolTip"));
            this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // cmsPictureBox
            // 
            resources.ApplyResources(this.cmsPictureBox, "cmsPictureBox");
            this.cmsPictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deletenotMarkToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteThisCodeLineToolStripMenuItem,
            this.moveToFirstPosToolStripMenuItem,
            this.deletePathToolStripMenuItem});
            this.cmsPictureBox.Name = "cmsPictureBox";
            this.toolTip1.SetToolTip(this.cmsPictureBox, resources.GetString("cmsPictureBox.ToolTip"));
            // 
            // deletenotMarkToolStripMenuItem
            // 
            resources.ApplyResources(this.deletenotMarkToolStripMenuItem, "deletenotMarkToolStripMenuItem");
            this.deletenotMarkToolStripMenuItem.Name = "deletenotMarkToolStripMenuItem";
            this.deletenotMarkToolStripMenuItem.Click += new System.EventHandler(this.deletenotMarkToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // deleteThisCodeLineToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteThisCodeLineToolStripMenuItem, "deleteThisCodeLineToolStripMenuItem");
            this.deleteThisCodeLineToolStripMenuItem.Name = "deleteThisCodeLineToolStripMenuItem";
            this.deleteThisCodeLineToolStripMenuItem.Click += new System.EventHandler(this.deleteThisCodeLineToolStripMenuItem_Click);
            // 
            // moveToFirstPosToolStripMenuItem
            // 
            resources.ApplyResources(this.moveToFirstPosToolStripMenuItem, "moveToFirstPosToolStripMenuItem");
            this.moveToFirstPosToolStripMenuItem.Name = "moveToFirstPosToolStripMenuItem";
            this.moveToFirstPosToolStripMenuItem.Click += new System.EventHandler(this.moveToFirstPosToolStripMenuItem_Click);
            // 
            // deletePathToolStripMenuItem
            // 
            resources.ApplyResources(this.deletePathToolStripMenuItem, "deletePathToolStripMenuItem");
            this.deletePathToolStripMenuItem.Name = "deletePathToolStripMenuItem";
            this.deletePathToolStripMenuItem.Click += new System.EventHandler(this.deletePathToolStripMenuItem_Click);
            // 
            // tLPAussen
            // 
            resources.ApplyResources(this.tLPAussen, "tLPAussen");
            this.tLPAussen.Controls.Add(this.tLPLinks, 0, 0);
            this.tLPAussen.Controls.Add(this.tLPRechts, 1, 0);
            this.tLPAussen.Name = "tLPAussen";
            this.toolTip1.SetToolTip(this.tLPAussen, resources.GetString("tLPAussen.ToolTip"));
            // 
            // tLPLinks
            // 
            resources.ApplyResources(this.tLPLinks, "tLPLinks");
            this.tLPLinks.Controls.Add(this.fCTBCode, 0, 1);
            this.tLPLinks.Controls.Add(this.groupBox1, 0, 0);
            this.tLPLinks.Name = "tLPLinks";
            this.toolTip1.SetToolTip(this.tLPLinks, resources.GetString("tLPLinks.ToolTip"));
            // 
            // tLPRechts
            // 
            resources.ApplyResources(this.tLPRechts, "tLPRechts");
            this.tLPRechts.Controls.Add(this.tLPRechtsUnten, 0, 1);
            this.tLPRechts.Controls.Add(this.tLPRechtsOben, 0, 0);
            this.tLPRechts.Name = "tLPRechts";
            this.toolTip1.SetToolTip(this.tLPRechts, resources.GetString("tLPRechts.ToolTip"));
            // 
            // tLPRechtsUnten
            // 
            resources.ApplyResources(this.tLPRechtsUnten, "tLPRechtsUnten");
            this.tLPRechtsUnten.Controls.Add(this.tLPRechtsUntenRechts, 1, 0);
            this.tLPRechtsUnten.Controls.Add(this.tLPMitteUnten, 0, 0);
            this.tLPRechtsUnten.Name = "tLPRechtsUnten";
            this.toolTip1.SetToolTip(this.tLPRechtsUnten, resources.GetString("tLPRechtsUnten.ToolTip"));
            // 
            // tLPRechtsUntenRechts
            // 
            resources.ApplyResources(this.tLPRechtsUntenRechts, "tLPRechtsUntenRechts");
            this.tLPRechtsUntenRechts.Controls.Add(this.groupBox6, 0, 0);
            this.tLPRechtsUntenRechts.Name = "tLPRechtsUntenRechts";
            this.toolTip1.SetToolTip(this.tLPRechtsUntenRechts, resources.GetString("tLPRechtsUntenRechts.ToolTip"));
            // 
            // tLPMitteUnten
            // 
            resources.ApplyResources(this.tLPMitteUnten, "tLPMitteUnten");
            this.tLPMitteUnten.Controls.Add(this.pictureBox1, 0, 0);
            this.tLPMitteUnten.Controls.Add(this.tBURL, 0, 1);
            this.tLPMitteUnten.Name = "tLPMitteUnten";
            this.toolTip1.SetToolTip(this.tLPMitteUnten, resources.GetString("tLPMitteUnten.ToolTip"));
            // 
            // tLPRechtsOben
            // 
            resources.ApplyResources(this.tLPRechtsOben, "tLPRechtsOben");
            this.tLPRechtsOben.Controls.Add(this.groupBox5, 1, 0);
            this.tLPRechtsOben.Controls.Add(this.groupBox2, 0, 0);
            this.tLPRechtsOben.Name = "tLPRechtsOben";
            this.toolTip1.SetToolTip(this.tLPRechtsOben, resources.GetString("tLPRechtsOben.ToolTip"));
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tLPAussen);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
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
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.cmsPictureBox.ResumeLayout(false);
            this.tLPAussen.ResumeLayout(false);
            this.tLPLinks.ResumeLayout(false);
            this.tLPRechts.ResumeLayout(false);
            this.tLPRechtsUnten.ResumeLayout(false);
            this.tLPRechtsUnten.PerformLayout();
            this.tLPRechtsUntenRechts.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem deutschToolStripMenuItem;
    }
}

