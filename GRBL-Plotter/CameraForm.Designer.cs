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
    partial class CameraForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraForm));
            this.pictureBoxVideo = new System.Windows.Forms.PictureBox();
            this.nUDCameraZoom = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStripCamera = new System.Windows.Forms.MenuStrip();
            this.camSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.setZeroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachZeroPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.teachMarkerPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnApplyAngle = new System.Windows.Forms.Button();
            this.btnCamOffsetPlus = new System.Windows.Forms.Button();
            this.lblOffset = new System.Windows.Forms.Label();
            this.cBCamOffset = new System.Windows.Forms.CheckBox();
            this.lblAngle = new System.Windows.Forms.Label();
            this.btnCamOffsetMinus = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDCameraZoom)).BeginInit();
            this.menuStripCamera.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxVideo
            // 
            this.pictureBoxVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBoxVideo.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBoxVideo.Location = new System.Drawing.Point(0, 51);
            this.pictureBoxVideo.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxVideo.Name = "pictureBoxVideo";
            this.pictureBoxVideo.Size = new System.Drawing.Size(640, 480);
            this.pictureBoxVideo.TabIndex = 0;
            this.pictureBoxVideo.TabStop = false;
            this.pictureBoxVideo.Click += new System.EventHandler(this.pictureBoxVideo_Click);
            this.pictureBoxVideo.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxVideo_Paint);
            this.pictureBoxVideo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVideo_MouseDown);
            this.pictureBoxVideo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVideo_MouseUp);
            // 
            // nUDCameraZoom
            // 
            this.nUDCameraZoom.Location = new System.Drawing.Point(38, 28);
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
            this.nUDCameraZoom.Size = new System.Drawing.Size(35, 20);
            this.nUDCameraZoom.TabIndex = 8;
            this.nUDCameraZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDCameraZoom.ValueChanged += new System.EventHandler(this.nUDCameraZoom_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Zoom";
            // 
            // menuStripCamera
            // 
            this.menuStripCamera.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.camSourceToolStripMenuItem,
            this.rotationToolStripMenuItem,
            this.teachScalingToolStripMenuItem,
            this.teachOffsetToolStripMenuItem,
            this.setZeroToolStripMenuItem});
            this.menuStripCamera.Location = new System.Drawing.Point(0, 0);
            this.menuStripCamera.Name = "menuStripCamera";
            this.menuStripCamera.ShowItemToolTips = true;
            this.menuStripCamera.Size = new System.Drawing.Size(640, 24);
            this.menuStripCamera.TabIndex = 12;
            this.menuStripCamera.Text = "menuStrip1";
            // 
            // camSourceToolStripMenuItem
            // 
            this.camSourceToolStripMenuItem.AutoToolTip = true;
            this.camSourceToolStripMenuItem.Name = "camSourceToolStripMenuItem";
            this.camSourceToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.camSourceToolStripMenuItem.Text = "Source";
            this.camSourceToolStripMenuItem.ToolTipText = "Select camera source.";
            // 
            // rotationToolStripMenuItem
            // 
            this.rotationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setRotationAngleToolStripMenuItem});
            this.rotationToolStripMenuItem.Name = "rotationToolStripMenuItem";
            this.rotationToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.rotationToolStripMenuItem.Text = "Rotation";
            // 
            // setRotationAngleToolStripMenuItem
            // 
            this.setRotationAngleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.setRotationAngleToolStripMenuItem.Name = "setRotationAngleToolStripMenuItem";
            this.setRotationAngleToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.setRotationAngleToolStripMenuItem.Text = "Set Rotation angle";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.MaxLength = 6;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(50, 23);
            this.toolStripTextBox1.Text = "000,00";
            this.toolStripTextBox1.Leave += new System.EventHandler(this.nUDCameraZoom_ValueChanged);
            this.toolStripTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox1_KeyUp);
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
            this.teachScalingToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.teachScalingToolStripMenuItem.Text = "Teach Scaling";
            this.teachScalingToolStripMenuItem.ToolTipText = "1) Home Plotter\r\n2) move camera to upper position\r\n3) click in picture to teach d" +
    "istance of top radius\r\n4) move camera to lower position\r\n5) click in picture to " +
    "teach distance of bottom radius\r\n";
            // 
            // upperPositionToolStripMenuItem
            // 
            this.upperPositionToolStripMenuItem.Name = "upperPositionToolStripMenuItem";
            this.upperPositionToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.upperPositionToolStripMenuItem.Text = "Teach Upper Position";
            this.upperPositionToolStripMenuItem.ToolTipText = "Teach camera view";
            this.upperPositionToolStripMenuItem.Click += new System.EventHandler(this.upperPositionToolStripMenuItem_Click);
            // 
            // teachRadiusTopToolStripMenuItem
            // 
            this.teachRadiusTopToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2});
            this.teachRadiusTopToolStripMenuItem.Name = "teachRadiusTopToolStripMenuItem";
            this.teachRadiusTopToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.teachRadiusTopToolStripMenuItem.Text = "Set teach radius top";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(30, 23);
            this.toolStripTextBox2.Text = "30";
            this.toolStripTextBox2.TextChanged += new System.EventHandler(this.toolStripTextBox2_TextChanged);
            // 
            // lowerPositionToolStripMenuItem
            // 
            this.lowerPositionToolStripMenuItem.Name = "lowerPositionToolStripMenuItem";
            this.lowerPositionToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.lowerPositionToolStripMenuItem.Text = "Teach Lower Position";
            this.lowerPositionToolStripMenuItem.ToolTipText = "Teach camera view";
            this.lowerPositionToolStripMenuItem.Click += new System.EventHandler(this.lowerPositionToolStripMenuItem_Click);
            // 
            // teachRadiusBottomToolStripMenuItem
            // 
            this.teachRadiusBottomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox3});
            this.teachRadiusBottomToolStripMenuItem.Name = "teachRadiusBottomToolStripMenuItem";
            this.teachRadiusBottomToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.teachRadiusBottomToolStripMenuItem.Text = "Set teach radius bottom";
            // 
            // toolStripTextBox3
            // 
            this.toolStripTextBox3.Name = "toolStripTextBox3";
            this.toolStripTextBox3.Size = new System.Drawing.Size(30, 23);
            this.toolStripTextBox3.Text = "20";
            this.toolStripTextBox3.TextChanged += new System.EventHandler(this.toolStripTextBox3_TextChanged);
            // 
            // teachOffsetToolStripMenuItem
            // 
            this.teachOffsetToolStripMenuItem.AutoToolTip = true;
            this.teachOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teachToolStripMenuItem});
            this.teachOffsetToolStripMenuItem.Name = "teachOffsetToolStripMenuItem";
            this.teachOffsetToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.teachOffsetToolStripMenuItem.Text = "Teach Offset";
            this.teachOffsetToolStripMenuItem.ToolTipText = "1) Zero X,Y position\r\n2) Mark tool position (at X=0,Y=0)\r\n3) Move until marker is" +
    " in center of camera view\r\n4) Teach actual position as offset";
            // 
            // teachToolStripMenuItem
            // 
            this.teachToolStripMenuItem.Name = "teachToolStripMenuItem";
            this.teachToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.teachToolStripMenuItem.Text = "Teach Camera Offset";
            this.teachToolStripMenuItem.ToolTipText = "1) Zero X,Y position\r\n2) Mark tool position (at X=0,Y=0)\r\n3) Move until marker is" +
    " in center of camera view\r\n4) Teach actual position as offset";
            this.teachToolStripMenuItem.Click += new System.EventHandler(this.teachToolStripMenuItem_Click);
            // 
            // setZeroToolStripMenuItem
            // 
            this.setZeroToolStripMenuItem.AutoToolTip = true;
            this.setZeroToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.teachZeroPositionToolStripMenuItem,
            this.teachMarkerPositionToolStripMenuItem});
            this.setZeroToolStripMenuItem.Name = "setZeroToolStripMenuItem";
            this.setZeroToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.setZeroToolStripMenuItem.Text = "Set Coordinates";
            this.setZeroToolStripMenuItem.ToolTipText = "Set Coordinate zero with camera offset";
            // 
            // teachZeroPositionToolStripMenuItem
            // 
            this.teachZeroPositionToolStripMenuItem.Name = "teachZeroPositionToolStripMenuItem";
            this.teachZeroPositionToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.teachZeroPositionToolStripMenuItem.Text = "Set new coordinates to match Zero";
            this.teachZeroPositionToolStripMenuItem.Click += new System.EventHandler(this.btnSetOffsetZero_Click);
            // 
            // teachMarkerPositionToolStripMenuItem
            // 
            this.teachMarkerPositionToolStripMenuItem.Name = "teachMarkerPositionToolStripMenuItem";
            this.teachMarkerPositionToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.teachMarkerPositionToolStripMenuItem.Text = "Set new coordinates to match Marker";
            this.teachMarkerPositionToolStripMenuItem.Click += new System.EventHandler(this.btnSetOffsetMarker_Click);
            // 
            // btnApplyAngle
            // 
            this.btnApplyAngle.Location = new System.Drawing.Point(122, 26);
            this.btnApplyAngle.Name = "btnApplyAngle";
            this.btnApplyAngle.Size = new System.Drawing.Size(74, 23);
            this.btnApplyAngle.TabIndex = 17;
            this.btnApplyAngle.Text = "Apply Angle";
            this.toolTip1.SetToolTip(this.btnApplyAngle, "Use right mouse button to measure angle.\r\nPress button to transform GCode with me" +
        "asured angle.");
            this.btnApplyAngle.UseVisualStyleBackColor = true;
            this.btnApplyAngle.Click += new System.EventHandler(this.btnApplyAngle_Click);
            // 
            // btnCamOffsetPlus
            // 
            this.btnCamOffsetPlus.Location = new System.Drawing.Point(477, 26);
            this.btnCamOffsetPlus.Name = "btnCamOffsetPlus";
            this.btnCamOffsetPlus.Size = new System.Drawing.Size(82, 23);
            this.btnCamOffsetPlus.TabIndex = 14;
            this.btnCamOffsetPlus.Text = "+ Cam Offset";
            this.btnCamOffsetPlus.UseVisualStyleBackColor = true;
            this.btnCamOffsetPlus.Click += new System.EventHandler(this.btnCamOffsetPlus_Click);
            // 
            // lblOffset
            // 
            this.lblOffset.AutoSize = true;
            this.lblOffset.Location = new System.Drawing.Point(199, 31);
            this.lblOffset.Name = "lblOffset";
            this.lblOffset.Size = new System.Drawing.Size(35, 13);
            this.lblOffset.TabIndex = 16;
            this.lblOffset.Text = "label1";
            // 
            // cBCamOffset
            // 
            this.cBCamOffset.AutoSize = true;
            this.cBCamOffset.Location = new System.Drawing.Point(370, 30);
            this.cBCamOffset.Name = "cBCamOffset";
            this.cBCamOffset.Size = new System.Drawing.Size(106, 17);
            this.cBCamOffset.TabIndex = 13;
            this.cBCamOffset.Text = "Draw Cam Offset";
            this.cBCamOffset.UseVisualStyleBackColor = true;
            // 
            // lblAngle
            // 
            this.lblAngle.AutoSize = true;
            this.lblAngle.Location = new System.Drawing.Point(79, 31);
            this.lblAngle.Name = "lblAngle";
            this.lblAngle.Size = new System.Drawing.Size(32, 13);
            this.lblAngle.TabIndex = 18;
            this.lblAngle.Text = "0.00°";
            // 
            // btnCamOffsetMinus
            // 
            this.btnCamOffsetMinus.Location = new System.Drawing.Point(558, 26);
            this.btnCamOffsetMinus.Name = "btnCamOffsetMinus";
            this.btnCamOffsetMinus.Size = new System.Drawing.Size(82, 23);
            this.btnCamOffsetMinus.TabIndex = 19;
            this.btnCamOffsetMinus.Text = "- Cam Offset";
            this.btnCamOffsetMinus.UseVisualStyleBackColor = true;
            this.btnCamOffsetMinus.Click += new System.EventHandler(this.btnCamOffsetMinus_Click);
            // 
            // CameraForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 532);
            this.Controls.Add(this.btnCamOffsetMinus);
            this.Controls.Add(this.lblAngle);
            this.Controls.Add(this.btnApplyAngle);
            this.Controls.Add(this.lblOffset);
            this.Controls.Add(this.btnCamOffsetPlus);
            this.Controls.Add(this.cBCamOffset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nUDCameraZoom);
            this.Controls.Add(this.pictureBoxVideo);
            this.Controls.Add(this.menuStripCamera);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::GRBL_Plotter.Properties.Settings.Default, "locationCamForm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::GRBL_Plotter.Properties.Settings.Default.locationCamForm;
            this.MainMenuStrip = this.menuStripCamera;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(656, 570);
            this.MinimumSize = new System.Drawing.Size(656, 570);
            this.Name = "CameraForm";
            this.Text = "Camera";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.camera_form_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.camera_form_FormClosed);
            this.Load += new System.EventHandler(this.camera_form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDCameraZoom)).EndInit();
            this.menuStripCamera.ResumeLayout(false);
            this.menuStripCamera.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxVideo;
        private System.Windows.Forms.NumericUpDown nUDCameraZoom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip menuStripCamera;
        private System.Windows.Forms.ToolStripMenuItem camSourceToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem rotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachScalingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upperPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lowerPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setRotationAngleToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem teachRadiusTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripMenuItem teachRadiusBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox3;
        private System.Windows.Forms.ToolStripMenuItem teachOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachToolStripMenuItem;
        private System.Windows.Forms.Button btnCamOffsetPlus;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.ToolStripMenuItem setZeroToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBCamOffset;
        private System.Windows.Forms.Button btnApplyAngle;
        private System.Windows.Forms.Label lblAngle;
        private System.Windows.Forms.Button btnCamOffsetMinus;
        private System.Windows.Forms.ToolStripMenuItem teachZeroPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem teachMarkerPositionToolStripMenuItem;
    }
}