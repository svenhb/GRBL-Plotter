namespace GRBL_Plotter
{
    partial class GCodeFromImage
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
                adjustedImage.Dispose();
                originalImage.Dispose();
                resultImage.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeFromImage));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.nUDReso = new System.Windows.Forms.NumericUpDown();
            this.nUDHeight = new System.Windows.Forms.NumericUpDown();
            this.nUDWidth = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbLockRatio = new System.Windows.Forms.CheckBox();
            this.lblGamma = new System.Windows.Forms.Label();
            this.lblContrast = new System.Windows.Forms.Label();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbGrayscale = new System.Windows.Forms.CheckBox();
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbEngravingPattern2 = new System.Windows.Forms.RadioButton();
            this.rbEngravingPattern1 = new System.Windows.Forms.RadioButton();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbExceptColor = new System.Windows.Forms.CheckBox();
            this.nUDMode = new System.Windows.Forms.NumericUpDown();
            this.cbSkipToolOrder = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbExceptAlpha = new System.Windows.Forms.CheckBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnList = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDReso)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMode)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            resources.ApplyResources(this.lblStatus, "lblStatus");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.nUDReso);
            this.groupBox3.Controls.Add(this.nUDHeight);
            this.groupBox3.Controls.Add(this.nUDWidth);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.cbLockRatio);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // nUDReso
            // 
            this.nUDReso.DecimalPlaces = 1;
            this.nUDReso.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDReso, "nUDReso");
            this.nUDReso.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDReso.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDReso.Name = "nUDReso";
            this.toolTip1.SetToolTip(this.nUDReso, resources.GetString("nUDReso.ToolTip"));
            this.nUDReso.Value = new decimal(new int[] {
            4,
            0,
            0,
            65536});
            this.nUDReso.ValueChanged += new System.EventHandler(this.nUDReso_ValueChanged);
            // 
            // nUDHeight
            // 
            this.nUDHeight.DecimalPlaces = 1;
            this.nUDHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDHeight, "nUDHeight");
            this.nUDHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDHeight.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nUDHeight.Name = "nUDHeight";
            this.nUDHeight.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDHeight.ValueChanged += new System.EventHandler(this.nUDHeight_ValueChanged);
            // 
            // nUDWidth
            // 
            this.nUDWidth.DecimalPlaces = 1;
            this.nUDWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDWidth, "nUDWidth");
            this.nUDWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDWidth.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nUDWidth.Name = "nUDWidth";
            this.nUDWidth.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDWidth.ValueChanged += new System.EventHandler(this.nUDWidth_ValueChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // cbLockRatio
            // 
            resources.ApplyResources(this.cbLockRatio, "cbLockRatio");
            this.cbLockRatio.Checked = true;
            this.cbLockRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLockRatio.Name = "cbLockRatio";
            this.cbLockRatio.UseVisualStyleBackColor = true;
            // 
            // lblGamma
            // 
            resources.ApplyResources(this.lblGamma, "lblGamma");
            this.lblGamma.Name = "lblGamma";
            // 
            // lblContrast
            // 
            resources.ApplyResources(this.lblContrast, "lblContrast");
            this.lblContrast.Name = "lblContrast";
            // 
            // lblBrightness
            // 
            resources.ApplyResources(this.lblBrightness, "lblBrightness");
            this.lblBrightness.Name = "lblBrightness";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbGrayscale);
            this.groupBox1.Controls.Add(this.rbModeDither);
            this.groupBox1.Controls.Add(this.rbModeGray);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cbGrayscale
            // 
            resources.ApplyResources(this.cbGrayscale, "cbGrayscale");
            this.cbGrayscale.Name = "cbGrayscale";
            this.cbGrayscale.UseVisualStyleBackColor = true;
            this.cbGrayscale.CheckedChanged += new System.EventHandler(this.cbGrayscale_CheckedChanged);
            // 
            // rbModeDither
            // 
            resources.ApplyResources(this.rbModeDither, "rbModeDither");
            this.rbModeDither.Name = "rbModeDither";
            this.rbModeDither.UseVisualStyleBackColor = true;
            this.rbModeDither.CheckedChanged += new System.EventHandler(this.rbModeGray_CheckedChanged);
            // 
            // rbModeGray
            // 
            resources.ApplyResources(this.rbModeGray, "rbModeGray");
            this.rbModeGray.Checked = true;
            this.rbModeGray.Name = "rbModeGray";
            this.rbModeGray.TabStop = true;
            this.rbModeGray.UseVisualStyleBackColor = true;
            this.rbModeGray.CheckedChanged += new System.EventHandler(this.rbModeGray_CheckedChanged);
            // 
            // btnInvert
            // 
            this.btnInvert.BackgroundImage = global::GRBL_Plotter.Properties.Resources.inv2;
            resources.ApplyResources(this.btnInvert, "btnInvert");
            this.btnInvert.Name = "btnInvert";
            this.toolTip1.SetToolTip(this.btnInvert, resources.GetString("btnInvert.ToolTip"));
            this.btnInvert.UseVisualStyleBackColor = true;
            this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
            // 
            // btnHorizMirror
            // 
            this.btnHorizMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_horizontal;
            resources.ApplyResources(this.btnHorizMirror, "btnHorizMirror");
            this.btnHorizMirror.Name = "btnHorizMirror";
            this.toolTip1.SetToolTip(this.btnHorizMirror, resources.GetString("btnHorizMirror.ToolTip"));
            this.btnHorizMirror.UseVisualStyleBackColor = true;
            this.btnHorizMirror.Click += new System.EventHandler(this.btnHorizMirror_Click);
            // 
            // btnVertMirror
            // 
            this.btnVertMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_vertical;
            resources.ApplyResources(this.btnVertMirror, "btnVertMirror");
            this.btnVertMirror.Name = "btnVertMirror";
            this.toolTip1.SetToolTip(this.btnVertMirror, resources.GetString("btnVertMirror.ToolTip"));
            this.btnVertMirror.UseVisualStyleBackColor = true;
            this.btnVertMirror.Click += new System.EventHandler(this.btnVertMirror_Click);
            // 
            // btnRotateRight
            // 
            this.btnRotateRight.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_r;
            resources.ApplyResources(this.btnRotateRight, "btnRotateRight");
            this.btnRotateRight.Name = "btnRotateRight";
            this.toolTip1.SetToolTip(this.btnRotateRight, resources.GetString("btnRotateRight.ToolTip"));
            this.btnRotateRight.UseVisualStyleBackColor = true;
            this.btnRotateRight.Click += new System.EventHandler(this.btnRotateRight_Click);
            // 
            // btnRotateLeft
            // 
            this.btnRotateLeft.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_l;
            resources.ApplyResources(this.btnRotateLeft, "btnRotateLeft");
            this.btnRotateLeft.Name = "btnRotateLeft";
            this.toolTip1.SetToolTip(this.btnRotateLeft, resources.GetString("btnRotateLeft.ToolTip"));
            this.btnRotateLeft.UseVisualStyleBackColor = true;
            this.btnRotateLeft.Click += new System.EventHandler(this.btnRotateLeft_Click);
            // 
            // tBarGamma
            // 
            resources.ApplyResources(this.tBarGamma, "tBarGamma");
            this.tBarGamma.Maximum = 500;
            this.tBarGamma.Minimum = 1;
            this.tBarGamma.Name = "tBarGamma";
            this.tBarGamma.Value = 1;
            this.tBarGamma.Scroll += new System.EventHandler(this.tBarGamma_Scroll);
            // 
            // tBarContrast
            // 
            resources.ApplyResources(this.tBarContrast, "tBarContrast");
            this.tBarContrast.Maximum = 127;
            this.tBarContrast.Minimum = -127;
            this.tBarContrast.Name = "tBarContrast";
            this.tBarContrast.Scroll += new System.EventHandler(this.tBarContrast_Scroll);
            // 
            // tBarBrightness
            // 
            resources.ApplyResources(this.tBarBrightness, "tBarBrightness");
            this.tBarBrightness.Maximum = 127;
            this.tBarBrightness.Minimum = -127;
            this.tBarBrightness.Name = "tBarBrightness";
            this.tBarBrightness.Scroll += new System.EventHandler(this.tBarBrightness_Scroll);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbEngravingPattern2);
            this.groupBox2.Controls.Add(this.rbEngravingPattern1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // rbEngravingPattern2
            // 
            resources.ApplyResources(this.rbEngravingPattern2, "rbEngravingPattern2");
            this.rbEngravingPattern2.Name = "rbEngravingPattern2";
            this.rbEngravingPattern2.UseVisualStyleBackColor = true;
            // 
            // rbEngravingPattern1
            // 
            resources.ApplyResources(this.rbEngravingPattern1, "rbEngravingPattern1");
            this.rbEngravingPattern1.Checked = true;
            this.rbEngravingPattern1.Name = "rbEngravingPattern1";
            this.rbEngravingPattern1.TabStop = true;
            this.rbEngravingPattern1.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            resources.ApplyResources(this.btnGenerate, "btnGenerate");
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GRBL_Plotter.Properties.Resources.modell;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tBarBrightness);
            this.groupBox4.Controls.Add(this.lblGamma);
            this.groupBox4.Controls.Add(this.btnInvert);
            this.groupBox4.Controls.Add(this.tBarContrast);
            this.groupBox4.Controls.Add(this.btnHorizMirror);
            this.groupBox4.Controls.Add(this.lblContrast);
            this.groupBox4.Controls.Add(this.btnVertMirror);
            this.groupBox4.Controls.Add(this.tBarGamma);
            this.groupBox4.Controls.Add(this.btnRotateRight);
            this.groupBox4.Controls.Add(this.lblBrightness);
            this.groupBox4.Controls.Add(this.btnRotateLeft);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // cbExceptColor
            // 
            resources.ApplyResources(this.cbExceptColor, "cbExceptColor");
            this.cbExceptColor.BackColor = System.Drawing.Color.White;
            this.cbExceptColor.Name = "cbExceptColor";
            this.toolTip1.SetToolTip(this.cbExceptColor, resources.GetString("cbExceptColor.ToolTip"));
            this.cbExceptColor.UseVisualStyleBackColor = false;
            this.cbExceptColor.CheckedChanged += new System.EventHandler(this.cbExceptColor_CheckedChanged);
            // 
            // nUDMode
            // 
            resources.ApplyResources(this.nUDMode, "nUDMode");
            this.nUDMode.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nUDMode.Name = "nUDMode";
            this.toolTip1.SetToolTip(this.nUDMode, resources.GetString("nUDMode.ToolTip"));
            this.nUDMode.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // cbSkipToolOrder
            // 
            resources.ApplyResources(this.cbSkipToolOrder, "cbSkipToolOrder");
            this.cbSkipToolOrder.Name = "cbSkipToolOrder";
            this.toolTip1.SetToolTip(this.cbSkipToolOrder, resources.GetString("cbSkipToolOrder.ToolTip"));
            this.cbSkipToolOrder.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbExceptColor);
            this.groupBox5.Controls.Add(this.cbExceptAlpha);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // cbExceptAlpha
            // 
            resources.ApplyResources(this.cbExceptAlpha, "cbExceptAlpha");
            this.cbExceptAlpha.Checked = true;
            this.cbExceptAlpha.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbExceptAlpha.Name = "cbExceptAlpha";
            this.cbExceptAlpha.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCheckOrig_MouseDown);
            this.btnTest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnCheckOrig_MouseUp);
            // 
            // btnList
            // 
            resources.ApplyResources(this.btnList, "btnList");
            this.btnList.Name = "btnList";
            this.btnList.UseVisualStyleBackColor = true;
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbSkipToolOrder);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.nUDMode);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // GCodeFromImage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.btnList);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "GCodeFromImage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GCodeFromImage_FormClosing);
            this.Load += new System.EventHandler(this.ImageToGCode_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDReso)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWidth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMode)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TrackBar tBarBrightness;
        private System.Windows.Forms.Button btnInvert;
        private System.Windows.Forms.Button btnHorizMirror;
        private System.Windows.Forms.Button btnVertMirror;
        private System.Windows.Forms.Button btnRotateRight;
        private System.Windows.Forms.Button btnRotateLeft;
        private System.Windows.Forms.TrackBar tBarGamma;
        private System.Windows.Forms.TrackBar tBarContrast;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nUDWidth;
        private System.Windows.Forms.NumericUpDown nUDHeight;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown nUDReso;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.CheckBox cbGrayscale;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cbExceptAlpha;
        private System.Windows.Forms.CheckBox cbExceptColor;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.NumericUpDown nUDMode;
        private System.Windows.Forms.Button btnList;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbSkipToolOrder;
        private System.Windows.Forms.Label label7;
    }
}