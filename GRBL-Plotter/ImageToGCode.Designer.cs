namespace GRBL_Plotter
{
    partial class ImageToGCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageToGCode));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.cbLockRatio = new System.Windows.Forms.CheckBox();
            this.tbRes = new System.Windows.Forms.TextBox();
            this.lblGamma = new System.Windows.Forms.Label();
            this.lblContrast = new System.Windows.Forms.Label();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbModeDither = new System.Windows.Forms.RadioButton();
            this.rbModeGray = new System.Windows.Forms.RadioButton();
            this.btnInvert = new System.Windows.Forms.Button();
            this.btnHorizMirror = new System.Windows.Forms.Button();
            this.btnVertMirror = new System.Windows.Forms.Button();
            this.btnRotateRight = new System.Windows.Forms.Button();
            this.btnRotateLeft = new System.Windows.Forms.Button();
            this.tBarGamma = new System.Windows.Forms.TrackBar();
            this.tBarContrast = new System.Windows.Forms.TrackBar();
            this.tBarBrightness = new System.Windows.Forms.TrackBar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.tbLaserMax = new System.Windows.Forms.TextBox();
            this.tbLaserMin = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbEngravingPattern2 = new System.Windows.Forms.RadioButton();
            this.rbEngravingPattern1 = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(688, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "open";
            this.toolStripButton1.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 444);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(688, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(118, 17);
            this.lblStatus.Text = "toolStripStatusLabel1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(488, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(200, 438);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.lblGamma);
            this.tabPage1.Controls.Add(this.lblContrast);
            this.tabPage1.Controls.Add(this.lblBrightness);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.btnInvert);
            this.tabPage1.Controls.Add(this.btnHorizMirror);
            this.tabPage1.Controls.Add(this.btnVertMirror);
            this.tabPage1.Controls.Add(this.btnRotateRight);
            this.tabPage1.Controls.Add(this.btnRotateLeft);
            this.tabPage1.Controls.Add(this.tBarGamma);
            this.tabPage1.Controls.Add(this.tBarContrast);
            this.tabPage1.Controls.Add(this.tBarBrightness);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(192, 412);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.tbWidth);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.tbHeight);
            this.groupBox3.Controls.Add(this.cbLockRatio);
            this.groupBox3.Controls.Add(this.tbRes);
            this.groupBox3.Location = new System.Drawing.Point(6, 186);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(183, 73);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Width";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(87, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Reso";
            // 
            // tbWidth
            // 
            this.tbWidth.Location = new System.Drawing.Point(44, 18);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.Size = new System.Drawing.Size(36, 20);
            this.tbWidth.TabIndex = 9;
            this.tbWidth.Text = "50";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Height";
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(44, 44);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(36, 20);
            this.tbHeight.TabIndex = 10;
            this.tbHeight.Text = "50";
            // 
            // cbLockRatio
            // 
            this.cbLockRatio.AutoSize = true;
            this.cbLockRatio.Checked = true;
            this.cbLockRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLockRatio.Location = new System.Drawing.Point(90, 17);
            this.cbLockRatio.Name = "cbLockRatio";
            this.cbLockRatio.Size = new System.Drawing.Size(46, 17);
            this.cbLockRatio.TabIndex = 12;
            this.cbLockRatio.Text = "ratio";
            this.cbLockRatio.UseVisualStyleBackColor = true;
            // 
            // tbRes
            // 
            this.tbRes.Location = new System.Drawing.Point(128, 44);
            this.tbRes.Name = "tbRes";
            this.tbRes.Size = new System.Drawing.Size(31, 20);
            this.tbRes.TabIndex = 11;
            this.tbRes.Text = "0.5";
            // 
            // lblGamma
            // 
            this.lblGamma.AutoSize = true;
            this.lblGamma.Location = new System.Drawing.Point(75, 101);
            this.lblGamma.Name = "lblGamma";
            this.lblGamma.Size = new System.Drawing.Size(35, 13);
            this.lblGamma.TabIndex = 19;
            this.lblGamma.Text = "label6";
            // 
            // lblContrast
            // 
            this.lblContrast.AutoSize = true;
            this.lblContrast.Location = new System.Drawing.Point(75, 52);
            this.lblContrast.Name = "lblContrast";
            this.lblContrast.Size = new System.Drawing.Size(35, 13);
            this.lblContrast.TabIndex = 18;
            this.lblContrast.Text = "label5";
            // 
            // lblBrightness
            // 
            this.lblBrightness.AutoSize = true;
            this.lblBrightness.Location = new System.Drawing.Point(75, 3);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(35, 13);
            this.lblBrightness.TabIndex = 17;
            this.lblBrightness.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Gamma";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Contrast";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Brightness";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbModeDither);
            this.groupBox1.Controls.Add(this.rbModeGray);
            this.groupBox1.Location = new System.Drawing.Point(3, 265);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(178, 62);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Conversion mode";
            // 
            // rbModeDither
            // 
            this.rbModeDither.AutoSize = true;
            this.rbModeDither.Location = new System.Drawing.Point(16, 39);
            this.rbModeDither.Name = "rbModeDither";
            this.rbModeDither.Size = new System.Drawing.Size(67, 17);
            this.rbModeDither.TabIndex = 1;
            this.rbModeDither.Text = "Dithering";
            this.rbModeDither.UseVisualStyleBackColor = true;
            this.rbModeDither.CheckedChanged += new System.EventHandler(this.rbModeGray_CheckedChanged);
            // 
            // rbModeGray
            // 
            this.rbModeGray.AutoSize = true;
            this.rbModeGray.Checked = true;
            this.rbModeGray.Location = new System.Drawing.Point(16, 16);
            this.rbModeGray.Name = "rbModeGray";
            this.rbModeGray.Size = new System.Drawing.Size(72, 17);
            this.rbModeGray.TabIndex = 0;
            this.rbModeGray.TabStop = true;
            this.rbModeGray.Text = "Grayscale";
            this.rbModeGray.UseVisualStyleBackColor = true;
            this.rbModeGray.CheckedChanged += new System.EventHandler(this.rbModeGray_CheckedChanged);
            // 
            // btnInvert
            // 
            this.btnInvert.BackgroundImage = global::GRBL_Plotter.Properties.Resources.inv2;
            this.btnInvert.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnInvert.Location = new System.Drawing.Point(131, 155);
            this.btnInvert.Name = "btnInvert";
            this.btnInvert.Size = new System.Drawing.Size(25, 25);
            this.btnInvert.TabIndex = 8;
            this.btnInvert.Text = "button5";
            this.btnInvert.UseVisualStyleBackColor = true;
            this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
            // 
            // btnHorizMirror
            // 
            this.btnHorizMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_horizontal;
            this.btnHorizMirror.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnHorizMirror.Location = new System.Drawing.Point(102, 155);
            this.btnHorizMirror.Name = "btnHorizMirror";
            this.btnHorizMirror.Size = new System.Drawing.Size(23, 23);
            this.btnHorizMirror.TabIndex = 7;
            this.btnHorizMirror.UseVisualStyleBackColor = true;
            this.btnHorizMirror.Click += new System.EventHandler(this.btnHorizMirror_Click);
            // 
            // btnVertMirror
            // 
            this.btnVertMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_vertical;
            this.btnVertMirror.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnVertMirror.Location = new System.Drawing.Point(71, 153);
            this.btnVertMirror.Name = "btnVertMirror";
            this.btnVertMirror.Size = new System.Drawing.Size(25, 25);
            this.btnVertMirror.TabIndex = 6;
            this.btnVertMirror.UseVisualStyleBackColor = true;
            this.btnVertMirror.Click += new System.EventHandler(this.btnVertMirror_Click);
            // 
            // btnRotateRight
            // 
            this.btnRotateRight.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_r;
            this.btnRotateRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRotateRight.Location = new System.Drawing.Point(40, 153);
            this.btnRotateRight.Name = "btnRotateRight";
            this.btnRotateRight.Size = new System.Drawing.Size(25, 25);
            this.btnRotateRight.TabIndex = 5;
            this.btnRotateRight.UseVisualStyleBackColor = true;
            this.btnRotateRight.Click += new System.EventHandler(this.btnRotateRight_Click);
            // 
            // btnRotateLeft
            // 
            this.btnRotateLeft.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_l;
            this.btnRotateLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRotateLeft.Location = new System.Drawing.Point(9, 153);
            this.btnRotateLeft.Name = "btnRotateLeft";
            this.btnRotateLeft.Size = new System.Drawing.Size(25, 25);
            this.btnRotateLeft.TabIndex = 4;
            this.btnRotateLeft.UseVisualStyleBackColor = true;
            this.btnRotateLeft.Click += new System.EventHandler(this.btnRotateLeft_Click);
            // 
            // tBarGamma
            // 
            this.tBarGamma.AutoSize = false;
            this.tBarGamma.Location = new System.Drawing.Point(3, 117);
            this.tBarGamma.Maximum = 500;
            this.tBarGamma.Minimum = 1;
            this.tBarGamma.Name = "tBarGamma";
            this.tBarGamma.Size = new System.Drawing.Size(186, 30);
            this.tBarGamma.TabIndex = 3;
            this.tBarGamma.Value = 1;
            this.tBarGamma.Scroll += new System.EventHandler(this.tBarGamma_Scroll);
            // 
            // tBarContrast
            // 
            this.tBarContrast.AutoSize = false;
            this.tBarContrast.Location = new System.Drawing.Point(3, 68);
            this.tBarContrast.Maximum = 127;
            this.tBarContrast.Minimum = -127;
            this.tBarContrast.Name = "tBarContrast";
            this.tBarContrast.Size = new System.Drawing.Size(186, 30);
            this.tBarContrast.TabIndex = 2;
            this.tBarContrast.Scroll += new System.EventHandler(this.tBarContrast_Scroll);
            // 
            // tBarBrightness
            // 
            this.tBarBrightness.AutoSize = false;
            this.tBarBrightness.Location = new System.Drawing.Point(3, 19);
            this.tBarBrightness.Maximum = 127;
            this.tBarBrightness.Minimum = -127;
            this.tBarBrightness.Name = "tBarBrightness";
            this.tBarBrightness.Size = new System.Drawing.Size(186, 30);
            this.tBarBrightness.TabIndex = 1;
            this.tBarBrightness.Scroll += new System.EventHandler(this.tBarBrightness_Scroll);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnGenerate);
            this.tabPage2.Controls.Add(this.tbLaserMax);
            this.tabPage2.Controls.Add(this.tbLaserMin);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 412);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(65, 186);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "Convert";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // tbLaserMax
            // 
            this.tbLaserMax.Location = new System.Drawing.Point(84, 113);
            this.tbLaserMax.Name = "tbLaserMax";
            this.tbLaserMax.Size = new System.Drawing.Size(39, 20);
            this.tbLaserMax.TabIndex = 2;
            this.tbLaserMax.Text = "1";
            // 
            // tbLaserMin
            // 
            this.tbLaserMin.Location = new System.Drawing.Point(84, 87);
            this.tbLaserMin.Name = "tbLaserMin";
            this.tbLaserMin.Size = new System.Drawing.Size(49, 20);
            this.tbLaserMin.TabIndex = 1;
            this.tbLaserMin.Text = "-10";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbEngravingPattern2);
            this.groupBox2.Controls.Add(this.rbEngravingPattern1);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(183, 75);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // rbEngravingPattern2
            // 
            this.rbEngravingPattern2.AutoSize = true;
            this.rbEngravingPattern2.Location = new System.Drawing.Point(6, 42);
            this.rbEngravingPattern2.Name = "rbEngravingPattern2";
            this.rbEngravingPattern2.Size = new System.Drawing.Size(65, 17);
            this.rbEngravingPattern2.TabIndex = 1;
            this.rbEngravingPattern2.TabStop = true;
            this.rbEngravingPattern2.Text = "diagonal";
            this.rbEngravingPattern2.UseVisualStyleBackColor = true;
            // 
            // rbEngravingPattern1
            // 
            this.rbEngravingPattern1.AutoSize = true;
            this.rbEngravingPattern1.Location = new System.Drawing.Point(6, 19);
            this.rbEngravingPattern1.Name = "rbEngravingPattern1";
            this.rbEngravingPattern1.Size = new System.Drawing.Size(70, 17);
            this.rbEngravingPattern1.TabIndex = 0;
            this.rbEngravingPattern1.TabStop = true;
            this.rbEngravingPattern1.Text = "horizontal";
            this.rbEngravingPattern1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GRBL_Plotter.Properties.Resources.modell;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(11, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(279, 217);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 320);
            this.panel1.TabIndex = 4;
            // 
            // ImageToGCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 466);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ImageToGCode";
            this.Text = "Image2GCode";
            this.Load += new System.EventHandler(this.ImageToGCode_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TrackBar tBarBrightness;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.Button btnInvert;
        private System.Windows.Forms.Button btnHorizMirror;
        private System.Windows.Forms.Button btnVertMirror;
        private System.Windows.Forms.Button btnRotateRight;
        private System.Windows.Forms.Button btnRotateLeft;
        private System.Windows.Forms.TrackBar tBarGamma;
        private System.Windows.Forms.TrackBar tBarContrast;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.TextBox tbRes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbModeDither;
        private System.Windows.Forms.RadioButton rbModeGray;
        private System.Windows.Forms.CheckBox cbLockRatio;
        private System.Windows.Forms.Label lblGamma;
        private System.Windows.Forms.Label lblContrast;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbEngravingPattern2;
        private System.Windows.Forms.RadioButton rbEngravingPattern1;
        private System.Windows.Forms.TextBox tbLaserMax;
        private System.Windows.Forms.TextBox tbLaserMin;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGenerate;
    }
}