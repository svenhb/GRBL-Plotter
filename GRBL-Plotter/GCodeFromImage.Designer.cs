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
            this.statusStrip1.Location = new System.Drawing.Point(0, 384);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(374, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(118, 17);
            this.lblStatus.Text = "toolStripStatusLabel1";
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
            this.groupBox3.Location = new System.Drawing.Point(2, 208);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 73);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output size in units";
            // 
            // nUDReso
            // 
            this.nUDReso.DecimalPlaces = 1;
            this.nUDReso.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDReso.Location = new System.Drawing.Point(152, 45);
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
            this.nUDReso.Size = new System.Drawing.Size(42, 20);
            this.nUDReso.TabIndex = 24;
            this.toolTip1.SetToolTip(this.nUDReso, "Distance between lines");
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
            this.nUDHeight.Location = new System.Drawing.Point(42, 45);
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
            this.nUDHeight.Size = new System.Drawing.Size(53, 20);
            this.nUDHeight.TabIndex = 23;
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
            this.nUDWidth.Location = new System.Drawing.Point(42, 19);
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
            this.nUDWidth.Size = new System.Drawing.Size(53, 20);
            this.nUDWidth.TabIndex = 5;
            this.nUDWidth.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDWidth.ValueChanged += new System.EventHandler(this.nUDWidth_ValueChanged);
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
            this.label6.Location = new System.Drawing.Point(99, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Resolution";
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
            // cbLockRatio
            // 
            this.cbLockRatio.AutoSize = true;
            this.cbLockRatio.Checked = true;
            this.cbLockRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLockRatio.Location = new System.Drawing.Point(121, 20);
            this.cbLockRatio.Name = "cbLockRatio";
            this.cbLockRatio.Size = new System.Drawing.Size(73, 17);
            this.cbLockRatio.TabIndex = 12;
            this.cbLockRatio.Text = "keep ratio";
            this.cbLockRatio.UseVisualStyleBackColor = true;
            // 
            // lblGamma
            // 
            this.lblGamma.AutoSize = true;
            this.lblGamma.Location = new System.Drawing.Point(67, 114);
            this.lblGamma.Name = "lblGamma";
            this.lblGamma.Size = new System.Drawing.Size(35, 13);
            this.lblGamma.TabIndex = 19;
            this.lblGamma.Text = "label6";
            // 
            // lblContrast
            // 
            this.lblContrast.AutoSize = true;
            this.lblContrast.Location = new System.Drawing.Point(67, 65);
            this.lblContrast.Name = "lblContrast";
            this.lblContrast.Size = new System.Drawing.Size(35, 13);
            this.lblContrast.TabIndex = 18;
            this.lblContrast.Text = "label5";
            // 
            // lblBrightness
            // 
            this.lblBrightness.AutoSize = true;
            this.lblBrightness.Location = new System.Drawing.Point(67, 16);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(35, 13);
            this.lblBrightness.TabIndex = 17;
            this.lblBrightness.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Gamma";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Contrast";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Brightness";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbGrayscale);
            this.groupBox1.Controls.Add(this.rbModeDither);
            this.groupBox1.Controls.Add(this.rbModeGray);
            this.groupBox1.Location = new System.Drawing.Point(208, 210);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(162, 71);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Conversion mode";
            // 
            // cbGrayscale
            // 
            this.cbGrayscale.AutoSize = true;
            this.cbGrayscale.Location = new System.Drawing.Point(8, 18);
            this.cbGrayscale.Name = "cbGrayscale";
            this.cbGrayscale.Size = new System.Drawing.Size(73, 17);
            this.cbGrayscale.TabIndex = 2;
            this.cbGrayscale.Text = "Grayscale";
            this.cbGrayscale.UseVisualStyleBackColor = true;
            this.cbGrayscale.CheckedChanged += new System.EventHandler(this.cbGrayscale_CheckedChanged);
            // 
            // rbModeDither
            // 
            this.rbModeDither.AutoSize = true;
            this.rbModeDither.Location = new System.Drawing.Point(89, 41);
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
            this.rbModeGray.Location = new System.Drawing.Point(8, 41);
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
            this.btnInvert.Location = new System.Drawing.Point(132, 171);
            this.btnInvert.Name = "btnInvert";
            this.btnInvert.Size = new System.Drawing.Size(25, 25);
            this.btnInvert.TabIndex = 8;
            this.btnInvert.Text = "button5";
            this.toolTip1.SetToolTip(this.btnInvert, "Invert colors");
            this.btnInvert.UseVisualStyleBackColor = true;
            this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
            // 
            // btnHorizMirror
            // 
            this.btnHorizMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_horizontal;
            this.btnHorizMirror.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnHorizMirror.Location = new System.Drawing.Point(101, 171);
            this.btnHorizMirror.Name = "btnHorizMirror";
            this.btnHorizMirror.Size = new System.Drawing.Size(25, 23);
            this.btnHorizMirror.TabIndex = 7;
            this.toolTip1.SetToolTip(this.btnHorizMirror, "Horizontal flip");
            this.btnHorizMirror.UseVisualStyleBackColor = true;
            this.btnHorizMirror.Click += new System.EventHandler(this.btnHorizMirror_Click);
            // 
            // btnVertMirror
            // 
            this.btnVertMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_vertical;
            this.btnVertMirror.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnVertMirror.Location = new System.Drawing.Point(70, 171);
            this.btnVertMirror.Name = "btnVertMirror";
            this.btnVertMirror.Size = new System.Drawing.Size(25, 25);
            this.btnVertMirror.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnVertMirror, "Vertical flip");
            this.btnVertMirror.UseVisualStyleBackColor = true;
            this.btnVertMirror.Click += new System.EventHandler(this.btnVertMirror_Click);
            // 
            // btnRotateRight
            // 
            this.btnRotateRight.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_r;
            this.btnRotateRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRotateRight.Location = new System.Drawing.Point(39, 171);
            this.btnRotateRight.Name = "btnRotateRight";
            this.btnRotateRight.Size = new System.Drawing.Size(25, 25);
            this.btnRotateRight.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnRotateRight, "Rotate picture 90° cw");
            this.btnRotateRight.UseVisualStyleBackColor = true;
            this.btnRotateRight.Click += new System.EventHandler(this.btnRotateRight_Click);
            // 
            // btnRotateLeft
            // 
            this.btnRotateLeft.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_l;
            this.btnRotateLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRotateLeft.Location = new System.Drawing.Point(8, 171);
            this.btnRotateLeft.Name = "btnRotateLeft";
            this.btnRotateLeft.Size = new System.Drawing.Size(25, 25);
            this.btnRotateLeft.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnRotateLeft, "Rotate picture 90° ccw");
            this.btnRotateLeft.UseVisualStyleBackColor = true;
            this.btnRotateLeft.Click += new System.EventHandler(this.btnRotateLeft_Click);
            // 
            // tBarGamma
            // 
            this.tBarGamma.AutoSize = false;
            this.tBarGamma.Location = new System.Drawing.Point(8, 130);
            this.tBarGamma.Maximum = 500;
            this.tBarGamma.Minimum = 1;
            this.tBarGamma.Name = "tBarGamma";
            this.tBarGamma.Size = new System.Drawing.Size(149, 30);
            this.tBarGamma.TabIndex = 3;
            this.tBarGamma.Value = 1;
            this.tBarGamma.Scroll += new System.EventHandler(this.tBarGamma_Scroll);
            // 
            // tBarContrast
            // 
            this.tBarContrast.AutoSize = false;
            this.tBarContrast.Location = new System.Drawing.Point(8, 81);
            this.tBarContrast.Maximum = 127;
            this.tBarContrast.Minimum = -127;
            this.tBarContrast.Name = "tBarContrast";
            this.tBarContrast.Size = new System.Drawing.Size(149, 30);
            this.tBarContrast.TabIndex = 2;
            this.tBarContrast.Scroll += new System.EventHandler(this.tBarContrast_Scroll);
            // 
            // tBarBrightness
            // 
            this.tBarBrightness.AutoSize = false;
            this.tBarBrightness.Location = new System.Drawing.Point(8, 32);
            this.tBarBrightness.Maximum = 127;
            this.tBarBrightness.Minimum = -127;
            this.tBarBrightness.Name = "tBarBrightness";
            this.tBarBrightness.Size = new System.Drawing.Size(149, 30);
            this.tBarBrightness.TabIndex = 1;
            this.tBarBrightness.Scroll += new System.EventHandler(this.tBarBrightness_Scroll);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbEngravingPattern2);
            this.groupBox2.Controls.Add(this.rbEngravingPattern1);
            this.groupBox2.Location = new System.Drawing.Point(2, 287);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(85, 65);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Plot direction";
            // 
            // rbEngravingPattern2
            // 
            this.rbEngravingPattern2.AutoSize = true;
            this.rbEngravingPattern2.Location = new System.Drawing.Point(6, 41);
            this.rbEngravingPattern2.Name = "rbEngravingPattern2";
            this.rbEngravingPattern2.Size = new System.Drawing.Size(65, 17);
            this.rbEngravingPattern2.TabIndex = 1;
            this.rbEngravingPattern2.Text = "diagonal";
            this.rbEngravingPattern2.UseVisualStyleBackColor = true;
            // 
            // rbEngravingPattern1
            // 
            this.rbEngravingPattern1.AutoSize = true;
            this.rbEngravingPattern1.Checked = true;
            this.rbEngravingPattern1.Location = new System.Drawing.Point(6, 19);
            this.rbEngravingPattern1.Name = "rbEngravingPattern1";
            this.rbEngravingPattern1.Size = new System.Drawing.Size(70, 17);
            this.rbEngravingPattern1.TabIndex = 0;
            this.rbEngravingPattern1.TabStop = true;
            this.rbEngravingPattern1.Text = "horizontal";
            this.rbEngravingPattern1.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(270, 358);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(100, 24);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "Create GCode";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GRBL_Plotter.Properties.Resources.modell;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(5, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 200);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 200);
            this.panel1.TabIndex = 4;
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
            this.groupBox4.Location = new System.Drawing.Point(208, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(162, 202);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Color correction";
            // 
            // cbExceptColor
            // 
            this.cbExceptColor.AutoSize = true;
            this.cbExceptColor.BackColor = System.Drawing.Color.White;
            this.cbExceptColor.Location = new System.Drawing.Point(6, 42);
            this.cbExceptColor.Name = "cbExceptColor";
            this.cbExceptColor.Size = new System.Drawing.Size(50, 17);
            this.cbExceptColor.TabIndex = 1;
            this.cbExceptColor.Text = "Color";
            this.toolTip1.SetToolTip(this.cbExceptColor, "Left click inside picture to select color");
            this.cbExceptColor.UseVisualStyleBackColor = false;
            this.cbExceptColor.CheckedChanged += new System.EventHandler(this.cbExceptColor_CheckedChanged);
            // 
            // nUDMode
            // 
            this.nUDMode.Location = new System.Drawing.Point(45, 18);
            this.nUDMode.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nUDMode.Name = "nUDMode";
            this.nUDMode.Size = new System.Drawing.Size(33, 20);
            this.nUDMode.TabIndex = 27;
            this.toolTip1.SetToolTip(this.nUDMode, resources.GetString("nUDMode.ToolTip"));
            this.nUDMode.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // cbSkipToolOrder
            // 
            this.cbSkipToolOrder.AutoSize = true;
            this.cbSkipToolOrder.Location = new System.Drawing.Point(9, 42);
            this.cbSkipToolOrder.Name = "cbSkipToolOrder";
            this.cbSkipToolOrder.Size = new System.Drawing.Size(137, 17);
            this.cbSkipToolOrder.TabIndex = 29;
            this.cbSkipToolOrder.Text = "Skip tool nr from palette";
            this.toolTip1.SetToolTip(this.cbSkipToolOrder, "Instead of using tool number from palette (e.g. 3,7,9) tools will be counted from" +
        " zero (0,1,2)");
            this.cbSkipToolOrder.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(2, 358);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(85, 24);
            this.btnLoad.TabIndex = 24;
            this.btnLoad.Text = "Load picture";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbExceptColor);
            this.groupBox5.Controls.Add(this.cbExceptAlpha);
            this.groupBox5.Location = new System.Drawing.Point(93, 287);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(75, 65);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Exceptions";
            // 
            // cbExceptAlpha
            // 
            this.cbExceptAlpha.AutoSize = true;
            this.cbExceptAlpha.Checked = true;
            this.cbExceptAlpha.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbExceptAlpha.Location = new System.Drawing.Point(6, 19);
            this.cbExceptAlpha.Name = "cbExceptAlpha";
            this.cbExceptAlpha.Size = new System.Drawing.Size(64, 17);
            this.cbExceptAlpha.TabIndex = 0;
            this.cbExceptAlpha.Text = "alpha=0";
            this.cbExceptAlpha.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(106, 358);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(71, 24);
            this.btnTest.TabIndex = 26;
            this.btnTest.Text = "Show result";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCheckOrig_MouseDown);
            this.btnTest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnCheckOrig_MouseUp);
            // 
            // btnList
            // 
            this.btnList.Location = new System.Drawing.Point(183, 358);
            this.btnList.Name = "btnList";
            this.btnList.Size = new System.Drawing.Size(64, 24);
            this.btnList.TabIndex = 28;
            this.btnList.Text = "List colors";
            this.btnList.UseVisualStyleBackColor = true;
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbSkipToolOrder);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.nUDMode);
            this.groupBox6.Location = new System.Drawing.Point(174, 287);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(196, 65);
            this.groupBox6.TabIndex = 29;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Color replacing";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Mode";
            // 
            // GCodeFromImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 406);
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
            this.MinimumSize = new System.Drawing.Size(390, 445);
            this.Name = "GCodeFromImage";
            this.Text = "Create GCode from Image";
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