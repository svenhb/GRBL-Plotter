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
    partial class ControlCameraForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                pen1.Dispose();
                penDown.Dispose();
                penMarker.Dispose();
                penRuler.Dispose();
                penTool.Dispose();
                penUp.Dispose();
                brushText.Dispose();
                penTeach.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlCameraForm));
            this.pictureBoxVideo = new System.Windows.Forms.PictureBox();
            this.cmsPictureBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveMarkerToCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compensateAngleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showOverlayGraphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nUDCameraZoom = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStripCamera = new System.Windows.Forms.MenuStrip();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.camSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setRotationAngleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.teachScalingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upperPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachRadiusTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.lowerPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachRadiusBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox3 = new System.Windows.Forms.ToolStripTextBox();
            this.teachOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crossHairsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setZeroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachZeroPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachMarkerPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnApplyAngle = new System.Windows.Forms.Button();
            this.btnCamCoordTool = new System.Windows.Forms.Button();
            this.cBCamCoordMove = new System.Windows.Forms.CheckBox();
            this.lblAngle = new System.Windows.Forms.Label();
            this.btnCamCoordCam = new System.Windows.Forms.Button();
            this.lblOffset = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblCenterPos = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.cBShapeDetection = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnAutoCenter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).BeginInit();
            this.cmsPictureBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDCameraZoom)).BeginInit();
            this.menuStripCamera.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxVideo
            // 
            resources.ApplyResources(this.pictureBoxVideo, "pictureBoxVideo");
            this.pictureBoxVideo.ContextMenuStrip = this.cmsPictureBox;
            this.pictureBoxVideo.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBoxVideo.Name = "pictureBoxVideo";
            this.pictureBoxVideo.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBoxVideo, resources.GetString("pictureBoxVideo.ToolTip"));
            this.pictureBoxVideo.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxVideo_Paint);
            this.pictureBoxVideo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVideo_Click);
            this.pictureBoxVideo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVideo_MouseDown);
            this.pictureBoxVideo.MouseEnter += new System.EventHandler(this.pictureBoxVideo_MouseEnter);
            this.pictureBoxVideo.MouseLeave += new System.EventHandler(this.pictureBoxVideo_MouseLeave);
            this.pictureBoxVideo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVideo_MouseUp);
            this.pictureBoxVideo.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVideo_MouseWheel);
            // 
            // cmsPictureBox
            // 
            resources.ApplyResources(this.cmsPictureBox, "cmsPictureBox");
            this.cmsPictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveMarkerToCenterToolStripMenuItem,
            this.compensateAngleToolStripMenuItem,
            this.toolStripSeparator1,
            this.showOverlayGraphicsToolStripMenuItem});
            this.cmsPictureBox.Name = "cmsPictureBox";
            this.toolTip1.SetToolTip(this.cmsPictureBox, resources.GetString("cmsPictureBox.ToolTip"));
            // 
            // moveMarkerToCenterToolStripMenuItem
            // 
            resources.ApplyResources(this.moveMarkerToCenterToolStripMenuItem, "moveMarkerToCenterToolStripMenuItem");
            this.moveMarkerToCenterToolStripMenuItem.Name = "moveMarkerToCenterToolStripMenuItem";
            this.moveMarkerToCenterToolStripMenuItem.Click += new System.EventHandler(this.teachpoint1_process_Click);
            // 
            // compensateAngleToolStripMenuItem
            // 
            resources.ApplyResources(this.compensateAngleToolStripMenuItem, "compensateAngleToolStripMenuItem");
            this.compensateAngleToolStripMenuItem.Name = "compensateAngleToolStripMenuItem";
            this.compensateAngleToolStripMenuItem.Click += new System.EventHandler(this.teachpoint2_process_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // showOverlayGraphicsToolStripMenuItem
            // 
            resources.ApplyResources(this.showOverlayGraphicsToolStripMenuItem, "showOverlayGraphicsToolStripMenuItem");
            this.showOverlayGraphicsToolStripMenuItem.Checked = true;
            this.showOverlayGraphicsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showOverlayGraphicsToolStripMenuItem.Name = "showOverlayGraphicsToolStripMenuItem";
            this.showOverlayGraphicsToolStripMenuItem.Click += new System.EventHandler(this.showOverlayGraphicsToolStripMenuItem_Click);
            // 
            // nUDCameraZoom
            // 
            resources.ApplyResources(this.nUDCameraZoom, "nUDCameraZoom");
            this.nUDCameraZoom.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nUDCameraZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDCameraZoom.Name = "nUDCameraZoom";
            this.nUDCameraZoom.ReadOnly = true;
            this.toolTip1.SetToolTip(this.nUDCameraZoom, resources.GetString("nUDCameraZoom.ToolTip"));
            this.nUDCameraZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDCameraZoom.ValueChanged += new System.EventHandler(this.nUDCameraZoom_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // menuStripCamera
            // 
            resources.ApplyResources(this.menuStripCamera, "menuStripCamera");
            this.menuStripCamera.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.setZeroToolStripMenuItem});
            this.menuStripCamera.Name = "menuStripCamera";
            this.menuStripCamera.ShowItemToolTips = true;
            this.toolTip1.SetToolTip(this.menuStripCamera, resources.GetString("menuStripCamera.ToolTip"));
            // 
            // setupToolStripMenuItem
            // 
            resources.ApplyResources(this.setupToolStripMenuItem, "setupToolStripMenuItem");
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.camSourceToolStripMenuItem,
            this.setRotationAngleToolStripMenuItem,
            this.teachScalingToolStripMenuItem,
            this.teachOffsetToolStripMenuItem,
            this.colorsToolStripMenuItem});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            // 
            // camSourceToolStripMenuItem
            // 
            resources.ApplyResources(this.camSourceToolStripMenuItem, "camSourceToolStripMenuItem");
            this.camSourceToolStripMenuItem.AutoToolTip = true;
            this.camSourceToolStripMenuItem.Name = "camSourceToolStripMenuItem";
            // 
            // setRotationAngleToolStripMenuItem
            // 
            resources.ApplyResources(this.setRotationAngleToolStripMenuItem, "setRotationAngleToolStripMenuItem");
            this.setRotationAngleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.setRotationAngleToolStripMenuItem.Name = "setRotationAngleToolStripMenuItem";
            // 
            // toolStripTextBox1
            // 
            resources.ApplyResources(this.toolStripTextBox1, "toolStripTextBox1");
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox1_KeyUp);
            // 
            // teachScalingToolStripMenuItem
            // 
            resources.ApplyResources(this.teachScalingToolStripMenuItem, "teachScalingToolStripMenuItem");
            this.teachScalingToolStripMenuItem.AutoToolTip = true;
            this.teachScalingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upperPositionToolStripMenuItem,
            this.teachRadiusTopToolStripMenuItem,
            this.lowerPositionToolStripMenuItem,
            this.teachRadiusBottomToolStripMenuItem});
            this.teachScalingToolStripMenuItem.Name = "teachScalingToolStripMenuItem";
            // 
            // upperPositionToolStripMenuItem
            // 
            resources.ApplyResources(this.upperPositionToolStripMenuItem, "upperPositionToolStripMenuItem");
            this.upperPositionToolStripMenuItem.Name = "upperPositionToolStripMenuItem";
            this.upperPositionToolStripMenuItem.Click += new System.EventHandler(this.upperPositionToolStripMenuItem_Click);
            // 
            // teachRadiusTopToolStripMenuItem
            // 
            resources.ApplyResources(this.teachRadiusTopToolStripMenuItem, "teachRadiusTopToolStripMenuItem");
            this.teachRadiusTopToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2});
            this.teachRadiusTopToolStripMenuItem.Name = "teachRadiusTopToolStripMenuItem";
            // 
            // toolStripTextBox2
            // 
            resources.ApplyResources(this.toolStripTextBox2, "toolStripTextBox2");
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.TextChanged += new System.EventHandler(this.toolStripTextBox2_TextChanged);
            // 
            // lowerPositionToolStripMenuItem
            // 
            resources.ApplyResources(this.lowerPositionToolStripMenuItem, "lowerPositionToolStripMenuItem");
            this.lowerPositionToolStripMenuItem.Name = "lowerPositionToolStripMenuItem";
            this.lowerPositionToolStripMenuItem.Click += new System.EventHandler(this.lowerPositionToolStripMenuItem_Click);
            // 
            // teachRadiusBottomToolStripMenuItem
            // 
            resources.ApplyResources(this.teachRadiusBottomToolStripMenuItem, "teachRadiusBottomToolStripMenuItem");
            this.teachRadiusBottomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox3});
            this.teachRadiusBottomToolStripMenuItem.Name = "teachRadiusBottomToolStripMenuItem";
            // 
            // toolStripTextBox3
            // 
            resources.ApplyResources(this.toolStripTextBox3, "toolStripTextBox3");
            this.toolStripTextBox3.Name = "toolStripTextBox3";
            this.toolStripTextBox3.Leave += new System.EventHandler(this.toolStripTextBox3_Leave);
            this.toolStripTextBox3.TextChanged += new System.EventHandler(this.toolStripTextBox3_TextChanged);
            // 
            // teachOffsetToolStripMenuItem
            // 
            resources.ApplyResources(this.teachOffsetToolStripMenuItem, "teachOffsetToolStripMenuItem");
            this.teachOffsetToolStripMenuItem.AutoToolTip = true;
            this.teachOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teachToolStripMenuItem});
            this.teachOffsetToolStripMenuItem.Name = "teachOffsetToolStripMenuItem";
            // 
            // teachToolStripMenuItem
            // 
            resources.ApplyResources(this.teachToolStripMenuItem, "teachToolStripMenuItem");
            this.teachToolStripMenuItem.Name = "teachToolStripMenuItem";
            this.teachToolStripMenuItem.Click += new System.EventHandler(this.teachToolStripMenuItem_Click);
            // 
            // colorsToolStripMenuItem
            // 
            resources.ApplyResources(this.colorsToolStripMenuItem, "colorsToolStripMenuItem");
            this.colorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textToolStripMenuItem,
            this.crossHairsToolStripMenuItem});
            this.colorsToolStripMenuItem.Name = "colorsToolStripMenuItem";
            // 
            // textToolStripMenuItem
            // 
            resources.ApplyResources(this.textToolStripMenuItem, "textToolStripMenuItem");
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Click += new System.EventHandler(this.textToolStripMenuItem_Click);
            // 
            // crossHairsToolStripMenuItem
            // 
            resources.ApplyResources(this.crossHairsToolStripMenuItem, "crossHairsToolStripMenuItem");
            this.crossHairsToolStripMenuItem.Name = "crossHairsToolStripMenuItem";
            this.crossHairsToolStripMenuItem.Click += new System.EventHandler(this.crossHairsToolStripMenuItem_Click);
            // 
            // setZeroToolStripMenuItem
            // 
            resources.ApplyResources(this.setZeroToolStripMenuItem, "setZeroToolStripMenuItem");
            this.setZeroToolStripMenuItem.AutoToolTip = true;
            this.setZeroToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teachZeroPositionToolStripMenuItem,
            this.teachMarkerPositionToolStripMenuItem});
            this.setZeroToolStripMenuItem.Name = "setZeroToolStripMenuItem";
            // 
            // teachZeroPositionToolStripMenuItem
            // 
            resources.ApplyResources(this.teachZeroPositionToolStripMenuItem, "teachZeroPositionToolStripMenuItem");
            this.teachZeroPositionToolStripMenuItem.Name = "teachZeroPositionToolStripMenuItem";
            this.teachZeroPositionToolStripMenuItem.Click += new System.EventHandler(this.btnSetOffsetZero_Click);
            // 
            // teachMarkerPositionToolStripMenuItem
            // 
            resources.ApplyResources(this.teachMarkerPositionToolStripMenuItem, "teachMarkerPositionToolStripMenuItem");
            this.teachMarkerPositionToolStripMenuItem.Name = "teachMarkerPositionToolStripMenuItem";
            this.teachMarkerPositionToolStripMenuItem.Click += new System.EventHandler(this.btnSetOffsetMarker_Click);
            // 
            // btnApplyAngle
            // 
            resources.ApplyResources(this.btnApplyAngle, "btnApplyAngle");
            this.btnApplyAngle.Name = "btnApplyAngle";
            this.toolTip1.SetToolTip(this.btnApplyAngle, resources.GetString("btnApplyAngle.ToolTip"));
            this.btnApplyAngle.UseVisualStyleBackColor = true;
            this.btnApplyAngle.Click += new System.EventHandler(this.btnApplyAngle_Click);
            // 
            // btnCamCoordTool
            // 
            resources.ApplyResources(this.btnCamCoordTool, "btnCamCoordTool");
            this.btnCamCoordTool.BackColor = System.Drawing.Color.Lime;
            this.btnCamCoordTool.Name = "btnCamCoordTool";
            this.toolTip1.SetToolTip(this.btnCamCoordTool, resources.GetString("btnCamCoordTool.ToolTip"));
            this.btnCamCoordTool.UseVisualStyleBackColor = false;
            this.btnCamCoordTool.Click += new System.EventHandler(this.btnCamCoordTool_Click);
            // 
            // cBCamCoordMove
            // 
            resources.ApplyResources(this.cBCamCoordMove, "cBCamCoordMove");
            this.cBCamCoordMove.Name = "cBCamCoordMove";
            this.toolTip1.SetToolTip(this.cBCamCoordMove, resources.GetString("cBCamCoordMove.ToolTip"));
            this.cBCamCoordMove.UseVisualStyleBackColor = true;
            // 
            // lblAngle
            // 
            resources.ApplyResources(this.lblAngle, "lblAngle");
            this.lblAngle.Name = "lblAngle";
            this.toolTip1.SetToolTip(this.lblAngle, resources.GetString("lblAngle.ToolTip"));
            // 
            // btnCamCoordCam
            // 
            resources.ApplyResources(this.btnCamCoordCam, "btnCamCoordCam");
            this.btnCamCoordCam.Name = "btnCamCoordCam";
            this.toolTip1.SetToolTip(this.btnCamCoordCam, resources.GetString("btnCamCoordCam.ToolTip"));
            this.btnCamCoordCam.UseVisualStyleBackColor = true;
            this.btnCamCoordCam.Click += new System.EventHandler(this.btnCamCoordCam_Click);
            // 
            // lblOffset
            // 
            resources.ApplyResources(this.lblOffset, "lblOffset");
            this.lblOffset.Name = "lblOffset";
            this.toolTip1.SetToolTip(this.lblOffset, resources.GetString("lblOffset.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.nUDCameraZoom);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.btnCamCoordTool);
            this.groupBox2.Controls.Add(this.btnCamCoordCam);
            this.groupBox2.Controls.Add(this.cBCamCoordMove);
            this.groupBox2.Controls.Add(this.lblOffset);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // lblCenterPos
            // 
            resources.ApplyResources(this.lblCenterPos, "lblCenterPos");
            this.lblCenterPos.Name = "lblCenterPos";
            this.toolTip1.SetToolTip(this.lblCenterPos, resources.GetString("lblCenterPos.ToolTip"));
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lblAngle);
            this.groupBox3.Controls.Add(this.btnApplyAngle);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // cBShapeDetection
            // 
            resources.ApplyResources(this.cBShapeDetection, "cBShapeDetection");
            this.cBShapeDetection.Name = "cBShapeDetection";
            this.toolTip1.SetToolTip(this.cBShapeDetection, resources.GetString("cBShapeDetection.ToolTip"));
            this.cBShapeDetection.UseVisualStyleBackColor = true;
            this.cBShapeDetection.CheckedChanged += new System.EventHandler(this.cBShapeDetection_CheckedChanged);
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.lblCenterPos);
            this.groupBox4.Controls.Add(this.comboBox1);
            this.groupBox4.Controls.Add(this.btnAutoCenter);
            this.groupBox4.Controls.Add(this.cBShapeDetection);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // comboBox1
            // 
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3")});
            this.comboBox1.Name = "comboBox1";
            this.toolTip1.SetToolTip(this.comboBox1, resources.GetString("comboBox1.ToolTip"));
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnAutoCenter
            // 
            resources.ApplyResources(this.btnAutoCenter, "btnAutoCenter");
            this.btnAutoCenter.Name = "btnAutoCenter";
            this.toolTip1.SetToolTip(this.btnAutoCenter, resources.GetString("btnAutoCenter.ToolTip"));
            this.btnAutoCenter.UseVisualStyleBackColor = true;
            this.btnAutoCenter.Click += new System.EventHandler(this.btnAutoCenter_Click);
            // 
            // ControlCameraForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBoxVideo);
            this.Controls.Add(this.menuStripCamera);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripCamera;
            this.MaximizeBox = false;
            this.Name = "ControlCameraForm";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.camera_form_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.camera_form_FormClosed);
            this.Load += new System.EventHandler(this.camera_form_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControlCameraForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ControlCameraForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).EndInit();
            this.cmsPictureBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nUDCameraZoom)).EndInit();
            this.menuStripCamera.ResumeLayout(false);
            this.menuStripCamera.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxVideo;
        private System.Windows.Forms.NumericUpDown nUDCameraZoom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip menuStripCamera;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnCamCoordTool;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.ToolStripMenuItem setZeroToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBCamCoordMove;
        private System.Windows.Forms.Button btnApplyAngle;
        private System.Windows.Forms.Label lblAngle;
        private System.Windows.Forms.Button btnCamCoordCam;
        private System.Windows.Forms.ToolStripMenuItem teachZeroPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachMarkerPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem camSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachScalingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upperPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachRadiusTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripMenuItem lowerPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachRadiusBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox3;
        private System.Windows.Forms.ToolStripMenuItem teachOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setRotationAngleToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem colorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crossHairsToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox cBShapeDetection;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnAutoCenter;
        private System.Windows.Forms.Label lblCenterPos;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ContextMenuStrip cmsPictureBox;
        private System.Windows.Forms.ToolStripMenuItem moveMarkerToCenterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compensateAngleToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showOverlayGraphicsToolStripMenuItem;
    }
}