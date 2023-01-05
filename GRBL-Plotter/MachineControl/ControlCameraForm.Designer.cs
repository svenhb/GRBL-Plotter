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
namespace GrblPlotter
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
                penTeachMark.Dispose();
                penUp.Dispose();
                penDown.Dispose();
                penRuler.Dispose();
                penTool.Dispose();
                penMarker.Dispose();
                penDimension.Dispose();
                brushText.Dispose();
                penTeach.Dispose();
				brushMachineLimit.Dispose();
            //    exampleFrameXy.Dispose();			// will be disposed in ControlCameraForm.cs line 294
            //    exampleFrameFix.Dispose();		// will be disposed in ControlCameraForm.cs line 295
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
            this.setWork00ToHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compensateAngleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.FiducialAddCoordinateIn2DViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FiducialRemoveLastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FiducialListClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ReferenceAddPointInImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReferenceRemoveLastPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReferenceClearListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.showOverlayGraphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nUDCameraZoom = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStripCamera = new System.Windows.Forms.MenuStrip();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulateCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraMountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripCameraMount = new System.Windows.Forms.ToolStripComboBox();
            this.camSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distortionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAndTeachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setRotationAngleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.teachScalingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upperPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachRadiusTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.lowerPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachRadiusBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox3 = new System.Windows.Forms.ToolStripTextBox();
            this.teachScaling2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachRadiusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setTeachRadiusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox4 = new System.Windows.Forms.ToolStripTextBox();
            this.toolPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setToolPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setMachinePositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crossHairsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.penupPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineLimitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dimensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnApplyAngle = new System.Windows.Forms.Button();
            this.btnCamCoordTool = new System.Windows.Forms.Button();
            this.cBCamCoordMove = new System.Windows.Forms.CheckBox();
            this.lblAngle = new System.Windows.Forms.Label();
            this.btnCamCoordCam = new System.Windows.Forms.Button();
            this.TbSetPoints = new System.Windows.Forms.TextBox();
            this.TbRealPoints = new System.Windows.Forms.TextBox();
            this.btnAutoCenter = new System.Windows.Forms.Button();
            this.GbZoom = new System.Windows.Forms.GroupBox();
            this.GbOffset = new System.Windows.Forms.GroupBox();
            this.BtnSetOffsetZero = new System.Windows.Forms.Button();
            this.BtnSetOffsetMarker = new System.Windows.Forms.Button();
            this.lblCenterPos = new System.Windows.Forms.Label();
            this.GbMeasure = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CbScale = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cBRotateArround0 = new System.Windows.Forms.CheckBox();
            this.cBShapeDetection = new System.Windows.Forms.CheckBox();
            this.BtnStartAutomatic = new System.Windows.Forms.Button();
            this.cBShapeShowFilter = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerFlowControl = new System.Windows.Forms.Timer(this.components);
            this.timerFreezFrame = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.CbUseShapeRecognition = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).BeginInit();
            this.cmsPictureBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDCameraZoom)).BeginInit();
            this.menuStripCamera.SuspendLayout();
            this.GbZoom.SuspendLayout();
            this.GbOffset.SuspendLayout();
            this.GbMeasure.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxVideo
            // 
            resources.ApplyResources(this.pictureBoxVideo, "pictureBoxVideo");
            this.pictureBoxVideo.ContextMenuStrip = this.cmsPictureBox;
            this.pictureBoxVideo.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBoxVideo.Name = "pictureBoxVideo";
            this.pictureBoxVideo.TabStop = false;
            this.pictureBoxVideo.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBoxVideo_Paint);
            this.pictureBoxVideo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBoxVideo_Click);
            this.pictureBoxVideo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBoxVideo_MouseDown);
            this.pictureBoxVideo.MouseEnter += new System.EventHandler(this.PictureBoxVideo_MouseEnter);
            this.pictureBoxVideo.MouseLeave += new System.EventHandler(this.PictureBoxVideo_MouseLeave);
            this.pictureBoxVideo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBoxVideo_MouseMove);
            this.pictureBoxVideo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBoxVideo_MouseUp);
            this.pictureBoxVideo.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBoxVideo_MouseWheel);
            // 
            // cmsPictureBox
            // 
            this.cmsPictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setWork00ToHereToolStripMenuItem,
            this.compensateAngleToolStripMenuItem,
            this.toolStripSeparator1,
            this.FiducialAddCoordinateIn2DViewToolStripMenuItem,
            this.FiducialRemoveLastToolStripMenuItem,
            this.FiducialListClearToolStripMenuItem,
            this.toolStripSeparator2,
            this.ReferenceAddPointInImageToolStripMenuItem,
            this.ReferenceRemoveLastPointToolStripMenuItem,
            this.ReferenceClearListToolStripMenuItem,
            this.toolStripSeparator3,
            this.showOverlayGraphicsToolStripMenuItem});
            this.cmsPictureBox.Name = "cmsPictureBox";
            resources.ApplyResources(this.cmsPictureBox, "cmsPictureBox");
            // 
            // setWork00ToHereToolStripMenuItem
            // 
            this.setWork00ToHereToolStripMenuItem.Name = "setWork00ToHereToolStripMenuItem";
            resources.ApplyResources(this.setWork00ToHereToolStripMenuItem, "setWork00ToHereToolStripMenuItem");
            this.setWork00ToHereToolStripMenuItem.Click += new System.EventHandler(this.SetWork00ToHereToolStripMenuItem_Click);
            // 
            // compensateAngleToolStripMenuItem
            // 
            this.compensateAngleToolStripMenuItem.Name = "compensateAngleToolStripMenuItem";
            resources.ApplyResources(this.compensateAngleToolStripMenuItem, "compensateAngleToolStripMenuItem");
            this.compensateAngleToolStripMenuItem.Click += new System.EventHandler(this.SetAngleToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // FiducialAddCoordinateIn2DViewToolStripMenuItem
            // 
            this.FiducialAddCoordinateIn2DViewToolStripMenuItem.Name = "FiducialAddCoordinateIn2DViewToolStripMenuItem";
            resources.ApplyResources(this.FiducialAddCoordinateIn2DViewToolStripMenuItem, "FiducialAddCoordinateIn2DViewToolStripMenuItem");
            this.FiducialAddCoordinateIn2DViewToolStripMenuItem.Click += new System.EventHandler(this.FiducialAddCoordinateIn2DViewToolStripMenuItem_Click);
            // 
            // FiducialRemoveLastToolStripMenuItem
            // 
            this.FiducialRemoveLastToolStripMenuItem.Name = "FiducialRemoveLastToolStripMenuItem";
            resources.ApplyResources(this.FiducialRemoveLastToolStripMenuItem, "FiducialRemoveLastToolStripMenuItem");
            this.FiducialRemoveLastToolStripMenuItem.Click += new System.EventHandler(this.FiducialRemoveLastToolStripMenuItem_Click);
            // 
            // FiducialListClearToolStripMenuItem
            // 
            this.FiducialListClearToolStripMenuItem.Name = "FiducialListClearToolStripMenuItem";
            resources.ApplyResources(this.FiducialListClearToolStripMenuItem, "FiducialListClearToolStripMenuItem");
            this.FiducialListClearToolStripMenuItem.Click += new System.EventHandler(this.FiducialListClearToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // ReferenceAddPointInImageToolStripMenuItem
            // 
            this.ReferenceAddPointInImageToolStripMenuItem.Name = "ReferenceAddPointInImageToolStripMenuItem";
            resources.ApplyResources(this.ReferenceAddPointInImageToolStripMenuItem, "ReferenceAddPointInImageToolStripMenuItem");
            this.ReferenceAddPointInImageToolStripMenuItem.Click += new System.EventHandler(this.ReferenceAddPointInImageToolStripMenuItem_Click);
            // 
            // ReferenceRemoveLastPointToolStripMenuItem
            // 
            this.ReferenceRemoveLastPointToolStripMenuItem.Name = "ReferenceRemoveLastPointToolStripMenuItem";
            resources.ApplyResources(this.ReferenceRemoveLastPointToolStripMenuItem, "ReferenceRemoveLastPointToolStripMenuItem");
            this.ReferenceRemoveLastPointToolStripMenuItem.Click += new System.EventHandler(this.ReferenceRemoveLastPointToolStripMenuItem_Click);
            // 
            // ReferenceClearListToolStripMenuItem
            // 
            this.ReferenceClearListToolStripMenuItem.Name = "ReferenceClearListToolStripMenuItem";
            resources.ApplyResources(this.ReferenceClearListToolStripMenuItem, "ReferenceClearListToolStripMenuItem");
            this.ReferenceClearListToolStripMenuItem.Click += new System.EventHandler(this.ReferenceClearListToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // showOverlayGraphicsToolStripMenuItem
            // 
            this.showOverlayGraphicsToolStripMenuItem.Checked = true;
            this.showOverlayGraphicsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showOverlayGraphicsToolStripMenuItem.Name = "showOverlayGraphicsToolStripMenuItem";
            resources.ApplyResources(this.showOverlayGraphicsToolStripMenuItem, "showOverlayGraphicsToolStripMenuItem");
            this.showOverlayGraphicsToolStripMenuItem.Click += new System.EventHandler(this.ShowOverlayGraphicsToolStripMenuItem_Click);
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
            this.nUDCameraZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDCameraZoom.ValueChanged += new System.EventHandler(this.NudCameraZoom_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // menuStripCamera
            // 
            this.menuStripCamera.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.viewToolStripMenuItem});
            resources.ApplyResources(this.menuStripCamera, "menuStripCamera");
            this.menuStripCamera.Name = "menuStripCamera";
            this.menuStripCamera.ShowItemToolTips = true;
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simulateCameraToolStripMenuItem,
            this.cameraMountToolStripMenuItem,
            this.camSourceToolStripMenuItem,
            this.distortionToolStripMenuItem,
            this.setRotationAngleToolStripMenuItem,
            this.teachScalingToolStripMenuItem,
            this.teachScaling2ToolStripMenuItem,
            this.toolPositionToolStripMenuItem,
            this.teachOffsetToolStripMenuItem,
            this.colorsToolStripMenuItem});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            resources.ApplyResources(this.setupToolStripMenuItem, "setupToolStripMenuItem");
            // 
            // simulateCameraToolStripMenuItem
            // 
            this.simulateCameraToolStripMenuItem.CheckOnClick = true;
            this.simulateCameraToolStripMenuItem.Name = "simulateCameraToolStripMenuItem";
            resources.ApplyResources(this.simulateCameraToolStripMenuItem, "simulateCameraToolStripMenuItem");
            this.simulateCameraToolStripMenuItem.CheckedChanged += new System.EventHandler(this.SimulateCameraToolStripMenuItem_CheckedChanged);
            // 
            // cameraMountToolStripMenuItem
            // 
            this.cameraMountToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCameraMount});
            this.cameraMountToolStripMenuItem.Name = "cameraMountToolStripMenuItem";
            resources.ApplyResources(this.cameraMountToolStripMenuItem, "cameraMountToolStripMenuItem");
            // 
            // toolStripCameraMount
            // 
            this.toolStripCameraMount.Name = "toolStripCameraMount";
            resources.ApplyResources(this.toolStripCameraMount, "toolStripCameraMount");
            // 
            // camSourceToolStripMenuItem
            // 
            this.camSourceToolStripMenuItem.AutoToolTip = true;
            this.camSourceToolStripMenuItem.Name = "camSourceToolStripMenuItem";
            resources.ApplyResources(this.camSourceToolStripMenuItem, "camSourceToolStripMenuItem");
            // 
            // distortionToolStripMenuItem
            // 
            this.distortionToolStripMenuItem.Checked = global::GrblPlotter.Properties.Settings.Default.cameraDistortionEnable;
            this.distortionToolStripMenuItem.CheckOnClick = true;
            this.distortionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.distortionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetAndTeachToolStripMenuItem});
            this.distortionToolStripMenuItem.Name = "distortionToolStripMenuItem";
            resources.ApplyResources(this.distortionToolStripMenuItem, "distortionToolStripMenuItem");
            this.distortionToolStripMenuItem.CheckedChanged += new System.EventHandler(this.DistortionToolStripMenuItem_CheckedChanged);
            // 
            // resetAndTeachToolStripMenuItem
            // 
            this.resetAndTeachToolStripMenuItem.Name = "resetAndTeachToolStripMenuItem";
            resources.ApplyResources(this.resetAndTeachToolStripMenuItem, "resetAndTeachToolStripMenuItem");
            this.resetAndTeachToolStripMenuItem.Click += new System.EventHandler(this.ResetAndTeachToolStripMenuItem_Click);
            // 
            // setRotationAngleToolStripMenuItem
            // 
            this.setRotationAngleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.setRotationAngleToolStripMenuItem.Name = "setRotationAngleToolStripMenuItem";
            resources.ApplyResources(this.setRotationAngleToolStripMenuItem, "setRotationAngleToolStripMenuItem");
            // 
            // toolStripTextBox1
            // 
            resources.ApplyResources(this.toolStripTextBox1, "toolStripTextBox1");
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ToolStripTextBox1_KeyUp);
            // 
            // teachScalingToolStripMenuItem
            // 
            this.teachScalingToolStripMenuItem.AutoToolTip = true;
            this.teachScalingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upperPositionToolStripMenuItem,
            this.teachRadiusTopToolStripMenuItem,
            this.lowerPositionToolStripMenuItem,
            this.teachRadiusBottomToolStripMenuItem});
            this.teachScalingToolStripMenuItem.Name = "teachScalingToolStripMenuItem";
            resources.ApplyResources(this.teachScalingToolStripMenuItem, "teachScalingToolStripMenuItem");
            // 
            // upperPositionToolStripMenuItem
            // 
            this.upperPositionToolStripMenuItem.Name = "upperPositionToolStripMenuItem";
            resources.ApplyResources(this.upperPositionToolStripMenuItem, "upperPositionToolStripMenuItem");
            this.upperPositionToolStripMenuItem.Click += new System.EventHandler(this.UpperPositionToolStripMenuItem_Click);
            // 
            // teachRadiusTopToolStripMenuItem
            // 
            this.teachRadiusTopToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2});
            this.teachRadiusTopToolStripMenuItem.Name = "teachRadiusTopToolStripMenuItem";
            resources.ApplyResources(this.teachRadiusTopToolStripMenuItem, "teachRadiusTopToolStripMenuItem");
            // 
            // toolStripTextBox2
            // 
            resources.ApplyResources(this.toolStripTextBox2, "toolStripTextBox2");
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Leave += new System.EventHandler(this.ToolStripTextBox2_Leave);
            this.toolStripTextBox2.TextChanged += new System.EventHandler(this.ToolStripTextBox2_TextChanged);
            // 
            // lowerPositionToolStripMenuItem
            // 
            this.lowerPositionToolStripMenuItem.Name = "lowerPositionToolStripMenuItem";
            resources.ApplyResources(this.lowerPositionToolStripMenuItem, "lowerPositionToolStripMenuItem");
            this.lowerPositionToolStripMenuItem.Click += new System.EventHandler(this.LowerPositionToolStripMenuItem_Click);
            // 
            // teachRadiusBottomToolStripMenuItem
            // 
            this.teachRadiusBottomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox3});
            this.teachRadiusBottomToolStripMenuItem.Name = "teachRadiusBottomToolStripMenuItem";
            resources.ApplyResources(this.teachRadiusBottomToolStripMenuItem, "teachRadiusBottomToolStripMenuItem");
            // 
            // toolStripTextBox3
            // 
            resources.ApplyResources(this.toolStripTextBox3, "toolStripTextBox3");
            this.toolStripTextBox3.Name = "toolStripTextBox3";
            this.toolStripTextBox3.Leave += new System.EventHandler(this.ToolStripTextBox3_Leave);
            this.toolStripTextBox3.TextChanged += new System.EventHandler(this.ToolStripTextBox3_TextChanged);
            // 
            // teachScaling2ToolStripMenuItem
            // 
            this.teachScaling2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teachRadiusToolStripMenuItem,
            this.setTeachRadiusToolStripMenuItem});
            this.teachScaling2ToolStripMenuItem.Name = "teachScaling2ToolStripMenuItem";
            resources.ApplyResources(this.teachScaling2ToolStripMenuItem, "teachScaling2ToolStripMenuItem");
            // 
            // teachRadiusToolStripMenuItem
            // 
            this.teachRadiusToolStripMenuItem.Name = "teachRadiusToolStripMenuItem";
            resources.ApplyResources(this.teachRadiusToolStripMenuItem, "teachRadiusToolStripMenuItem");
            this.teachRadiusToolStripMenuItem.Click += new System.EventHandler(this.TeachRadiusToolStripMenuItem_Click);
            // 
            // setTeachRadiusToolStripMenuItem
            // 
            this.setTeachRadiusToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox4});
            this.setTeachRadiusToolStripMenuItem.Name = "setTeachRadiusToolStripMenuItem";
            resources.ApplyResources(this.setTeachRadiusToolStripMenuItem, "setTeachRadiusToolStripMenuItem");
            // 
            // toolStripTextBox4
            // 
            resources.ApplyResources(this.toolStripTextBox4, "toolStripTextBox4");
            this.toolStripTextBox4.Name = "toolStripTextBox4";
            this.toolStripTextBox4.TextChanged += new System.EventHandler(this.ToolStripTextBox4_TextChanged);
            // 
            // toolPositionToolStripMenuItem
            // 
            this.toolPositionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setToolPositionToolStripMenuItem,
            this.setMachinePositionToolStripMenuItem});
            this.toolPositionToolStripMenuItem.Name = "toolPositionToolStripMenuItem";
            resources.ApplyResources(this.toolPositionToolStripMenuItem, "toolPositionToolStripMenuItem");
            // 
            // setToolPositionToolStripMenuItem
            // 
            this.setToolPositionToolStripMenuItem.Name = "setToolPositionToolStripMenuItem";
            resources.ApplyResources(this.setToolPositionToolStripMenuItem, "setToolPositionToolStripMenuItem");
            this.setToolPositionToolStripMenuItem.Click += new System.EventHandler(this.SetToolPositionToolStripMenuItem_Click);
            // 
            // setMachinePositionToolStripMenuItem
            // 
            this.setMachinePositionToolStripMenuItem.Name = "setMachinePositionToolStripMenuItem";
            resources.ApplyResources(this.setMachinePositionToolStripMenuItem, "setMachinePositionToolStripMenuItem");
            this.setMachinePositionToolStripMenuItem.Click += new System.EventHandler(this.SetMachinePositionToolStripMenuItem_Click);
            // 
            // teachOffsetToolStripMenuItem
            // 
            this.teachOffsetToolStripMenuItem.AutoToolTip = true;
            this.teachOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teachToolStripMenuItem});
            this.teachOffsetToolStripMenuItem.Name = "teachOffsetToolStripMenuItem";
            resources.ApplyResources(this.teachOffsetToolStripMenuItem, "teachOffsetToolStripMenuItem");
            // 
            // teachToolStripMenuItem
            // 
            this.teachToolStripMenuItem.Name = "teachToolStripMenuItem";
            resources.ApplyResources(this.teachToolStripMenuItem, "teachToolStripMenuItem");
            this.teachToolStripMenuItem.Click += new System.EventHandler(this.TeachToolStripMenuItem_Click);
            // 
            // colorsToolStripMenuItem
            // 
            this.colorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textToolStripMenuItem,
            this.crossHairsToolStripMenuItem});
            this.colorsToolStripMenuItem.Name = "colorsToolStripMenuItem";
            resources.ApplyResources(this.colorsToolStripMenuItem, "colorsToolStripMenuItem");
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            resources.ApplyResources(this.textToolStripMenuItem, "textToolStripMenuItem");
            this.textToolStripMenuItem.Click += new System.EventHandler(this.TextToolStripMenuItem_Click);
            // 
            // crossHairsToolStripMenuItem
            // 
            this.crossHairsToolStripMenuItem.Name = "crossHairsToolStripMenuItem";
            resources.ApplyResources(this.crossHairsToolStripMenuItem, "crossHairsToolStripMenuItem");
            this.crossHairsToolStripMenuItem.Click += new System.EventHandler(this.CrossHairsToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.penupPathToolStripMenuItem,
            this.machineLimitsToolStripMenuItem,
            this.dimensionToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // penupPathToolStripMenuItem
            // 
            this.penupPathToolStripMenuItem.Checked = global::GrblPlotter.Properties.Settings.Default.cameraShowPathPenUp;
            this.penupPathToolStripMenuItem.CheckOnClick = true;
            this.penupPathToolStripMenuItem.Name = "penupPathToolStripMenuItem";
            resources.ApplyResources(this.penupPathToolStripMenuItem, "penupPathToolStripMenuItem");
            // 
            // machineLimitsToolStripMenuItem
            // 
            this.machineLimitsToolStripMenuItem.Checked = global::GrblPlotter.Properties.Settings.Default.cameraShowPathLimits;
            this.machineLimitsToolStripMenuItem.CheckOnClick = true;
            this.machineLimitsToolStripMenuItem.Name = "machineLimitsToolStripMenuItem";
            resources.ApplyResources(this.machineLimitsToolStripMenuItem, "machineLimitsToolStripMenuItem");
            // 
            // dimensionToolStripMenuItem
            // 
            this.dimensionToolStripMenuItem.Checked = global::GrblPlotter.Properties.Settings.Default.cameraShowPathDimension;
            this.dimensionToolStripMenuItem.CheckOnClick = true;
            this.dimensionToolStripMenuItem.Name = "dimensionToolStripMenuItem";
            resources.ApplyResources(this.dimensionToolStripMenuItem, "dimensionToolStripMenuItem");
            // 
            // btnApplyAngle
            // 
            resources.ApplyResources(this.btnApplyAngle, "btnApplyAngle");
            this.btnApplyAngle.Name = "btnApplyAngle";
            this.toolTip1.SetToolTip(this.btnApplyAngle, resources.GetString("btnApplyAngle.ToolTip"));
            this.btnApplyAngle.UseVisualStyleBackColor = true;
            this.btnApplyAngle.Click += new System.EventHandler(this.BtnApplyAngle_Click);
            // 
            // btnCamCoordTool
            // 
            this.btnCamCoordTool.BackColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.btnCamCoordTool, "btnCamCoordTool");
            this.btnCamCoordTool.Name = "btnCamCoordTool";
            this.toolTip1.SetToolTip(this.btnCamCoordTool, resources.GetString("btnCamCoordTool.ToolTip"));
            this.btnCamCoordTool.UseVisualStyleBackColor = false;
            this.btnCamCoordTool.Click += new System.EventHandler(this.BtnCamCoordTool_Click);
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
            this.btnCamCoordCam.Click += new System.EventHandler(this.BtnCamCoordCam_Click);
            // 
            // TbSetPoints
            // 
            resources.ApplyResources(this.TbSetPoints, "TbSetPoints");
            this.TbSetPoints.Name = "TbSetPoints";
            this.toolTip1.SetToolTip(this.TbSetPoints, resources.GetString("TbSetPoints.ToolTip"));
            // 
            // TbRealPoints
            // 
            resources.ApplyResources(this.TbRealPoints, "TbRealPoints");
            this.TbRealPoints.Name = "TbRealPoints";
            this.toolTip1.SetToolTip(this.TbRealPoints, resources.GetString("TbRealPoints.ToolTip"));
            // 
            // btnAutoCenter
            // 
            resources.ApplyResources(this.btnAutoCenter, "btnAutoCenter");
            this.btnAutoCenter.Name = "btnAutoCenter";
            this.toolTip1.SetToolTip(this.btnAutoCenter, resources.GetString("btnAutoCenter.ToolTip"));
            this.btnAutoCenter.UseVisualStyleBackColor = true;
            this.btnAutoCenter.Click += new System.EventHandler(this.BtnAutoCenter_Click);
            // 
            // GbZoom
            // 
            this.GbZoom.Controls.Add(this.label3);
            this.GbZoom.Controls.Add(this.nUDCameraZoom);
            resources.ApplyResources(this.GbZoom, "GbZoom");
            this.GbZoom.Name = "GbZoom";
            this.GbZoom.TabStop = false;
            // 
            // GbOffset
            // 
            this.GbOffset.Controls.Add(this.BtnSetOffsetZero);
            this.GbOffset.Controls.Add(this.BtnSetOffsetMarker);
            this.GbOffset.Controls.Add(this.btnCamCoordTool);
            this.GbOffset.Controls.Add(this.btnCamCoordCam);
            this.GbOffset.Controls.Add(this.cBCamCoordMove);
            resources.ApplyResources(this.GbOffset, "GbOffset");
            this.GbOffset.Name = "GbOffset";
            this.GbOffset.TabStop = false;
            // 
            // BtnSetOffsetZero
            // 
            resources.ApplyResources(this.BtnSetOffsetZero, "BtnSetOffsetZero");
            this.BtnSetOffsetZero.Name = "BtnSetOffsetZero";
            this.BtnSetOffsetZero.UseVisualStyleBackColor = true;
            this.BtnSetOffsetZero.Click += new System.EventHandler(this.BtnSetOffsetZero_Click);
            // 
            // BtnSetOffsetMarker
            // 
            resources.ApplyResources(this.BtnSetOffsetMarker, "BtnSetOffsetMarker");
            this.BtnSetOffsetMarker.Name = "BtnSetOffsetMarker";
            this.BtnSetOffsetMarker.UseVisualStyleBackColor = true;
            this.BtnSetOffsetMarker.Click += new System.EventHandler(this.BtnSetOffsetMarker_Click);
            // 
            // lblCenterPos
            // 
            resources.ApplyResources(this.lblCenterPos, "lblCenterPos");
            this.lblCenterPos.Name = "lblCenterPos";
            // 
            // GbMeasure
            // 
            this.GbMeasure.Controls.Add(this.label6);
            this.GbMeasure.Controls.Add(this.CbScale);
            this.GbMeasure.Controls.Add(this.label1);
            this.GbMeasure.Controls.Add(this.lblAngle);
            this.GbMeasure.Controls.Add(this.btnApplyAngle);
            this.GbMeasure.Controls.Add(this.cBRotateArround0);
            resources.ApplyResources(this.GbMeasure, "GbMeasure");
            this.GbMeasure.Name = "GbMeasure";
            this.GbMeasure.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // CbScale
            // 
            resources.ApplyResources(this.CbScale, "CbScale");
            this.CbScale.Checked = global::GrblPlotter.Properties.Settings.Default.cameraScaleOnRotate;
            this.CbScale.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "cameraScaleOnRotate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbScale.Name = "CbScale";
            this.CbScale.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cBRotateArround0
            // 
            resources.ApplyResources(this.cBRotateArround0, "cBRotateArround0");
            this.cBRotateArround0.Checked = global::GrblPlotter.Properties.Settings.Default.cameraRotateArround0;
            this.cBRotateArround0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBRotateArround0.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "cameraRotateArround0", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBRotateArround0.Name = "cBRotateArround0";
            this.cBRotateArround0.UseVisualStyleBackColor = true;
            // 
            // cBShapeDetection
            // 
            resources.ApplyResources(this.cBShapeDetection, "cBShapeDetection");
            this.cBShapeDetection.Name = "cBShapeDetection";
            this.toolTip1.SetToolTip(this.cBShapeDetection, resources.GetString("cBShapeDetection.ToolTip"));
            this.cBShapeDetection.UseVisualStyleBackColor = true;
            this.cBShapeDetection.CheckedChanged += new System.EventHandler(this.CbShapeDetection_CheckedChanged);
            // 
            // BtnStartAutomatic
            // 
            resources.ApplyResources(this.BtnStartAutomatic, "BtnStartAutomatic");
            this.BtnStartAutomatic.Name = "BtnStartAutomatic";
            this.BtnStartAutomatic.UseVisualStyleBackColor = true;
            this.BtnStartAutomatic.Click += new System.EventHandler(this.BtnStartAutomatic_Click);
            // 
            // cBShapeShowFilter
            // 
            resources.ApplyResources(this.cBShapeShowFilter, "cBShapeShowFilter");
            this.cBShapeShowFilter.Name = "cBShapeShowFilter";
            this.cBShapeShowFilter.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3")});
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // timerFlowControl
            // 
            this.timerFlowControl.Interval = 1000;
            this.timerFlowControl.Tick += new System.EventHandler(this.TimerFlowControl_Tick);
            // 
            // timerFreezFrame
            // 
            this.timerFreezFrame.Tick += new System.EventHandler(this.TimerFreezFrame_Tick);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl1_DrawItem);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBox1);
            this.tabPage1.Controls.Add(this.CbUseShapeRecognition);
            this.tabPage1.Controls.Add(this.TbRealPoints);
            this.tabPage1.Controls.Add(this.TbSetPoints);
            this.tabPage1.Controls.Add(this.BtnStartAutomatic);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = global::GrblPlotter.Properties.Settings.Default.cameraScaleOnRotate;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "cameraScaleOnRotate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // CbUseShapeRecognition
            // 
            resources.ApplyResources(this.CbUseShapeRecognition, "CbUseShapeRecognition");
            this.CbUseShapeRecognition.Checked = true;
            this.CbUseShapeRecognition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbUseShapeRecognition.Name = "CbUseShapeRecognition";
            this.toolTip1.SetToolTip(this.CbUseShapeRecognition, resources.GetString("CbUseShapeRecognition.ToolTip"));
            this.CbUseShapeRecognition.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.lblCenterPos);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.btnAutoCenter);
            this.tabPage2.Controls.Add(this.comboBox1);
            this.tabPage2.Controls.Add(this.cBShapeShowFilter);
            this.tabPage2.Controls.Add(this.cBShapeDetection);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.GbMeasure);
            this.tabPage3.Controls.Add(this.GbZoom);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.GbOffset);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // ControlCameraForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBoxVideo);
            this.Controls.Add(this.menuStripCamera);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripCamera;
            this.MaximizeBox = false;
            this.Name = "ControlCameraForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Camera_form_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Camera_form_FormClosed);
            this.Load += new System.EventHandler(this.Camera_form_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControlCameraForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ControlCameraForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).EndInit();
            this.cmsPictureBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nUDCameraZoom)).EndInit();
            this.menuStripCamera.ResumeLayout(false);
            this.menuStripCamera.PerformLayout();
            this.GbZoom.ResumeLayout(false);
            this.GbZoom.PerformLayout();
            this.GbOffset.ResumeLayout(false);
            this.GbOffset.PerformLayout();
            this.GbMeasure.ResumeLayout(false);
            this.GbMeasure.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox cBCamCoordMove;
        private System.Windows.Forms.Button btnApplyAngle;
        private System.Windows.Forms.Label lblAngle;
        private System.Windows.Forms.Button btnCamCoordCam;
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
        private System.Windows.Forms.GroupBox GbZoom;
        private System.Windows.Forms.GroupBox GbOffset;
        private System.Windows.Forms.GroupBox GbMeasure;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem colorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crossHairsToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox cBShapeDetection;
        private System.Windows.Forms.Button btnAutoCenter;
        private System.Windows.Forms.Label lblCenterPos;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ContextMenuStrip cmsPictureBox;
        private System.Windows.Forms.ToolStripMenuItem compensateAngleToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showOverlayGraphicsToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBRotateArround0;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem distortionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetAndTeachToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setToolPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraMountToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripCameraMount;
        private System.Windows.Forms.ToolStripMenuItem teachScaling2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachRadiusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setTeachRadiusToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox4;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem penupPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem machineLimitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dimensionToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBShapeShowFilter;
        private System.Windows.Forms.Button BtnStartAutomatic;
        private System.Windows.Forms.Timer timerFlowControl;
        private System.Windows.Forms.Timer timerFreezFrame;
        private System.Windows.Forms.TextBox TbSetPoints;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ToolStripMenuItem simulateCameraToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button BtnSetOffsetMarker;
        private System.Windows.Forms.Button BtnSetOffsetZero;
        private System.Windows.Forms.CheckBox CbScale;
        private System.Windows.Forms.TextBox TbRealPoints;
        private System.Windows.Forms.CheckBox CbUseShapeRecognition;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolStripMenuItem setWork00ToHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FiducialAddCoordinateIn2DViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FiducialRemoveLastToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FiducialListClearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ReferenceAddPointInImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReferenceRemoveLastPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReferenceClearListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem setMachinePositionToolStripMenuItem;
    }
}