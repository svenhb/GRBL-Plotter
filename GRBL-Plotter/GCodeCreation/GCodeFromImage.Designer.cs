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
        /*        if (loadedImage != null)
                {
                    try { loadedImage.Dispose(); }
                    catch { }
                }*/
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblInfo1 = new System.Windows.Forms.Label();
            this.btnKeepSizeReso = new System.Windows.Forms.Button();
            this.btnKeepSizeWidth = new System.Windows.Forms.Button();
            this.lblSizeOrig = new System.Windows.Forms.Label();
            this.nUDReso = new System.Windows.Forms.NumericUpDown();
            this.lblSizeResult = new System.Windows.Forms.Label();
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
            this.gBgcodeDirection = new System.Windows.Forms.GroupBox();
            this.rbEngravingPattern2 = new System.Windows.Forms.RadioButton();
            this.rbEngravingPattern1 = new System.Windows.Forms.RadioButton();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAsOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblImageSource = new System.Windows.Forms.Label();
            this.lblHueShift = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tBarHueShift = new System.Windows.Forms.TrackBar();
            this.nUDMaxColors = new System.Windows.Forms.NumericUpDown();
            this.lblColors = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbExceptColor = new System.Windows.Forms.CheckBox();
            this.cbSkipToolOrder = new System.Windows.Forms.CheckBox();
            this.nUDZBot = new System.Windows.Forms.NumericUpDown();
            this.tBRMin = new System.Windows.Forms.TrackBar();
            this.tBRMax = new System.Windows.Forms.TrackBar();
            this.tBGMax = new System.Windows.Forms.TrackBar();
            this.tBGMin = new System.Windows.Forms.TrackBar();
            this.tBBMax = new System.Windows.Forms.TrackBar();
            this.tBBMin = new System.Windows.Forms.TrackBar();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.rBMode0 = new System.Windows.Forms.RadioButton();
            this.rBMode1 = new System.Windows.Forms.RadioButton();
            this.rBMode2 = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.nUDColorPercent = new System.Windows.Forms.NumericUpDown();
            this.btnPresetCorrection3 = new System.Windows.Forms.Button();
            this.btnShowSettings = new System.Windows.Forms.Button();
            this.btnShowOrig = new System.Windows.Forms.Button();
            this.nUDGCodeOutlineSmooth = new System.Windows.Forms.NumericUpDown();
            this.cBGCodeOutlineShrink = new System.Windows.Forms.CheckBox();
            this.nUDSMax = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rBProcessTool = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.rBGrayS = new System.Windows.Forms.RadioButton();
            this.rBGrayZ = new System.Windows.Forms.RadioButton();
            this.label22 = new System.Windows.Forms.Label();
            this.nUDSMin = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.nUDZTop = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.cBCompress = new System.Windows.Forms.CheckBox();
            this.rBProcessZ = new System.Windows.Forms.RadioButton();
            this.cBPreview = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cBReduceColorsToolTable = new System.Windows.Forms.CheckBox();
            this.cBReduceColorsDithering = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.btnPresetCorrection4 = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.btnPresetCorrection2 = new System.Windows.Forms.Button();
            this.btnResetCorrection = new System.Windows.Forms.Button();
            this.btnPresetCorrection1 = new System.Windows.Forms.Button();
            this.tBToolList = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.lblSaturation = new System.Windows.Forms.Label();
            this.tBarSaturation = new System.Windows.Forms.TrackBar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cBFilterRemoveArtefact = new System.Windows.Forms.CheckBox();
            this.cBFilterHistogram = new System.Windows.Forms.CheckBox();
            this.lblCFB = new System.Windows.Forms.Label();
            this.lblCFG = new System.Windows.Forms.Label();
            this.lblCFR = new System.Windows.Forms.Label();
            this.cBFilterEdge = new System.Windows.Forms.CheckBox();
            this.cBPosterize = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label21 = new System.Windows.Forms.Label();
            this.cLBTools = new System.Windows.Forms.CheckedListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cBResolutionPenWidth = new System.Windows.Forms.CheckBox();
            this.gBgcodeSelection = new System.Windows.Forms.GroupBox();
            this.cBGCodeFill = new System.Windows.Forms.CheckBox();
            this.cBGCodeOutline = new System.Windows.Forms.CheckBox();
            this.cBGCodeOutlineSmooth = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label24 = new System.Windows.Forms.Label();
            this.lBUseCase = new System.Windows.Forms.ListBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.lblLastUseCase = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDReso)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).BeginInit();
            this.gBgcodeDirection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarHueShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMaxColors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZBot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDColorPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGCodeOutlineSmooth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSMax)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZTop)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarSaturation)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gBgcodeSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Name = "statusStrip1";
            this.toolTip1.SetToolTip(this.statusStrip1, resources.GetString("statusStrip1.ToolTip"));
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.lblInfo1);
            this.groupBox3.Controls.Add(this.btnKeepSizeReso);
            this.groupBox3.Controls.Add(this.btnKeepSizeWidth);
            this.groupBox3.Controls.Add(this.lblSizeOrig);
            this.groupBox3.Controls.Add(this.nUDReso);
            this.groupBox3.Controls.Add(this.lblSizeResult);
            this.groupBox3.Controls.Add(this.nUDHeight);
            this.groupBox3.Controls.Add(this.nUDWidth);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.cbLockRatio);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // lblInfo1
            // 
            resources.ApplyResources(this.lblInfo1, "lblInfo1");
            this.lblInfo1.Name = "lblInfo1";
            this.toolTip1.SetToolTip(this.lblInfo1, resources.GetString("lblInfo1.ToolTip"));
            // 
            // btnKeepSizeReso
            // 
            resources.ApplyResources(this.btnKeepSizeReso, "btnKeepSizeReso");
            this.btnKeepSizeReso.Name = "btnKeepSizeReso";
            this.toolTip1.SetToolTip(this.btnKeepSizeReso, resources.GetString("btnKeepSizeReso.ToolTip"));
            this.btnKeepSizeReso.UseVisualStyleBackColor = true;
            this.btnKeepSizeReso.Click += new System.EventHandler(this.btnKeepSizeReso_Click);
            // 
            // btnKeepSizeWidth
            // 
            resources.ApplyResources(this.btnKeepSizeWidth, "btnKeepSizeWidth");
            this.btnKeepSizeWidth.Name = "btnKeepSizeWidth";
            this.toolTip1.SetToolTip(this.btnKeepSizeWidth, resources.GetString("btnKeepSizeWidth.ToolTip"));
            this.btnKeepSizeWidth.UseVisualStyleBackColor = true;
            this.btnKeepSizeWidth.Click += new System.EventHandler(this.btnKeepSizeWidth_Click);
            // 
            // lblSizeOrig
            // 
            resources.ApplyResources(this.lblSizeOrig, "lblSizeOrig");
            this.lblSizeOrig.BackColor = System.Drawing.Color.White;
            this.lblSizeOrig.Name = "lblSizeOrig";
            this.toolTip1.SetToolTip(this.lblSizeOrig, resources.GetString("lblSizeOrig.ToolTip"));
            // 
            // nUDReso
            // 
            resources.ApplyResources(this.nUDReso, "nUDReso");
            this.nUDReso.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importImageReso", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDReso.DecimalPlaces = 2;
            this.nUDReso.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            this.nUDReso.Value = global::GRBL_Plotter.Properties.Settings.Default.importImageReso;
            this.nUDReso.ValueChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // lblSizeResult
            // 
            resources.ApplyResources(this.lblSizeResult, "lblSizeResult");
            this.lblSizeResult.BackColor = System.Drawing.Color.White;
            this.lblSizeResult.Name = "lblSizeResult";
            this.toolTip1.SetToolTip(this.lblSizeResult, resources.GetString("lblSizeResult.ToolTip"));
            // 
            // nUDHeight
            // 
            resources.ApplyResources(this.nUDHeight, "nUDHeight");
            this.nUDHeight.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importImageHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDHeight.DecimalPlaces = 1;
            this.nUDHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
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
            this.toolTip1.SetToolTip(this.nUDHeight, resources.GetString("nUDHeight.ToolTip"));
            this.nUDHeight.Value = global::GRBL_Plotter.Properties.Settings.Default.importImageHeight;
            this.nUDHeight.ValueChanged += new System.EventHandler(this.nUDWidthHeight_ValueChanged);
            // 
            // nUDWidth
            // 
            resources.ApplyResources(this.nUDWidth, "nUDWidth");
            this.nUDWidth.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importImageWidth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDWidth.DecimalPlaces = 1;
            this.nUDWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
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
            this.toolTip1.SetToolTip(this.nUDWidth, resources.GetString("nUDWidth.ToolTip"));
            this.nUDWidth.Value = global::GRBL_Plotter.Properties.Settings.Default.importImageWidth;
            this.nUDWidth.ValueChanged += new System.EventHandler(this.nUDWidthHeight_ValueChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // cbLockRatio
            // 
            resources.ApplyResources(this.cbLockRatio, "cbLockRatio");
            this.cbLockRatio.Checked = global::GRBL_Plotter.Properties.Settings.Default.importImageKeepRatio;
            this.cbLockRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLockRatio.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importImageKeepRatio", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbLockRatio.Name = "cbLockRatio";
            this.toolTip1.SetToolTip(this.cbLockRatio, resources.GetString("cbLockRatio.ToolTip"));
            this.cbLockRatio.UseVisualStyleBackColor = true;
            // 
            // lblGamma
            // 
            resources.ApplyResources(this.lblGamma, "lblGamma");
            this.lblGamma.Name = "lblGamma";
            this.toolTip1.SetToolTip(this.lblGamma, resources.GetString("lblGamma.ToolTip"));
            // 
            // lblContrast
            // 
            resources.ApplyResources(this.lblContrast, "lblContrast");
            this.lblContrast.Name = "lblContrast";
            this.toolTip1.SetToolTip(this.lblContrast, resources.GetString("lblContrast.ToolTip"));
            // 
            // lblBrightness
            // 
            resources.ApplyResources(this.lblBrightness, "lblBrightness");
            this.lblBrightness.Name = "lblBrightness";
            this.toolTip1.SetToolTip(this.lblBrightness, resources.GetString("lblBrightness.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.cbGrayscale);
            this.groupBox1.Controls.Add(this.rbModeDither);
            this.groupBox1.Controls.Add(this.rbModeGray);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // cbGrayscale
            // 
            resources.ApplyResources(this.cbGrayscale, "cbGrayscale");
            this.cbGrayscale.Name = "cbGrayscale";
            this.toolTip1.SetToolTip(this.cbGrayscale, resources.GetString("cbGrayscale.ToolTip"));
            this.cbGrayscale.UseVisualStyleBackColor = true;
            this.cbGrayscale.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // rbModeDither
            // 
            resources.ApplyResources(this.rbModeDither, "rbModeDither");
            this.rbModeDither.Name = "rbModeDither";
            this.toolTip1.SetToolTip(this.rbModeDither, resources.GetString("rbModeDither.ToolTip"));
            this.rbModeDither.UseVisualStyleBackColor = true;
            this.rbModeDither.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // rbModeGray
            // 
            resources.ApplyResources(this.rbModeGray, "rbModeGray");
            this.rbModeGray.Checked = true;
            this.rbModeGray.Name = "rbModeGray";
            this.rbModeGray.TabStop = true;
            this.toolTip1.SetToolTip(this.rbModeGray, resources.GetString("rbModeGray.ToolTip"));
            this.rbModeGray.UseVisualStyleBackColor = true;
            this.rbModeGray.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // btnInvert
            // 
            resources.ApplyResources(this.btnInvert, "btnInvert");
            this.btnInvert.BackgroundImage = global::GRBL_Plotter.Properties.Resources.inv2;
            this.btnInvert.Name = "btnInvert";
            this.toolTip1.SetToolTip(this.btnInvert, resources.GetString("btnInvert.ToolTip"));
            this.btnInvert.UseVisualStyleBackColor = true;
            this.btnInvert.Click += new System.EventHandler(this.btnInvert_Click);
            // 
            // btnHorizMirror
            // 
            resources.ApplyResources(this.btnHorizMirror, "btnHorizMirror");
            this.btnHorizMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_horizontal;
            this.btnHorizMirror.Name = "btnHorizMirror";
            this.toolTip1.SetToolTip(this.btnHorizMirror, resources.GetString("btnHorizMirror.ToolTip"));
            this.btnHorizMirror.UseVisualStyleBackColor = true;
            this.btnHorizMirror.Click += new System.EventHandler(this.btnHorizMirror_Click);
            // 
            // btnVertMirror
            // 
            resources.ApplyResources(this.btnVertMirror, "btnVertMirror");
            this.btnVertMirror.BackgroundImage = global::GRBL_Plotter.Properties.Resources.flip_vertical;
            this.btnVertMirror.Name = "btnVertMirror";
            this.toolTip1.SetToolTip(this.btnVertMirror, resources.GetString("btnVertMirror.ToolTip"));
            this.btnVertMirror.UseVisualStyleBackColor = true;
            this.btnVertMirror.Click += new System.EventHandler(this.btnVertMirror_Click);
            // 
            // btnRotateRight
            // 
            resources.ApplyResources(this.btnRotateRight, "btnRotateRight");
            this.btnRotateRight.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_r;
            this.btnRotateRight.Name = "btnRotateRight";
            this.toolTip1.SetToolTip(this.btnRotateRight, resources.GetString("btnRotateRight.ToolTip"));
            this.btnRotateRight.UseVisualStyleBackColor = true;
            this.btnRotateRight.Click += new System.EventHandler(this.btnRotateRight_Click);
            // 
            // btnRotateLeft
            // 
            resources.ApplyResources(this.btnRotateLeft, "btnRotateLeft");
            this.btnRotateLeft.BackgroundImage = global::GRBL_Plotter.Properties.Resources.turn_l;
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
            this.tBarGamma.TickFrequency = 10;
            this.toolTip1.SetToolTip(this.tBarGamma, resources.GetString("tBarGamma.ToolTip"));
            this.tBarGamma.Value = 1;
            this.tBarGamma.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tBarContrast
            // 
            resources.ApplyResources(this.tBarContrast, "tBarContrast");
            this.tBarContrast.Maximum = 127;
            this.tBarContrast.Minimum = -127;
            this.tBarContrast.Name = "tBarContrast";
            this.tBarContrast.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBarContrast, resources.GetString("tBarContrast.ToolTip"));
            this.tBarContrast.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tBarBrightness
            // 
            resources.ApplyResources(this.tBarBrightness, "tBarBrightness");
            this.tBarBrightness.Maximum = 127;
            this.tBarBrightness.Minimum = -127;
            this.tBarBrightness.Name = "tBarBrightness";
            this.tBarBrightness.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBarBrightness, resources.GetString("tBarBrightness.ToolTip"));
            this.tBarBrightness.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // gBgcodeDirection
            // 
            resources.ApplyResources(this.gBgcodeDirection, "gBgcodeDirection");
            this.gBgcodeDirection.Controls.Add(this.rbEngravingPattern2);
            this.gBgcodeDirection.Controls.Add(this.rbEngravingPattern1);
            this.gBgcodeDirection.Name = "gBgcodeDirection";
            this.gBgcodeDirection.TabStop = false;
            this.toolTip1.SetToolTip(this.gBgcodeDirection, resources.GetString("gBgcodeDirection.ToolTip"));
            // 
            // rbEngravingPattern2
            // 
            resources.ApplyResources(this.rbEngravingPattern2, "rbEngravingPattern2");
            this.rbEngravingPattern2.Name = "rbEngravingPattern2";
            this.toolTip1.SetToolTip(this.rbEngravingPattern2, resources.GetString("rbEngravingPattern2.ToolTip"));
            this.rbEngravingPattern2.UseVisualStyleBackColor = true;
            this.rbEngravingPattern2.CheckedChanged += new System.EventHandler(this.rbEngravingPattern2_CheckedChanged);
            // 
            // rbEngravingPattern1
            // 
            resources.ApplyResources(this.rbEngravingPattern1, "rbEngravingPattern1");
            this.rbEngravingPattern1.Checked = true;
            this.rbEngravingPattern1.Name = "rbEngravingPattern1";
            this.rbEngravingPattern1.TabStop = true;
            this.toolTip1.SetToolTip(this.rbEngravingPattern1, resources.GetString("rbEngravingPattern1.ToolTip"));
            this.rbEngravingPattern1.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            resources.ApplyResources(this.btnGenerate, "btnGenerate");
            this.btnGenerate.Name = "btnGenerate";
            this.toolTip1.SetToolTip(this.btnGenerate, resources.GetString("btnGenerate.ToolTip"));
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Image = global::GRBL_Plotter.Properties.Resources.modell;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, resources.GetString("pictureBox1.ToolTip"));
            this.pictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragDrop);
            this.pictureBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragEnter);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // contextMenuStrip1
            // 
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.setAsOriginalToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.toolTip1.SetToolTip(this.contextMenuStrip1, resources.GetString("contextMenuStrip1.ToolTip"));
            // 
            // copyToolStripMenuItem
            // 
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // setAsOriginalToolStripMenuItem
            // 
            resources.ApplyResources(this.setAsOriginalToolStripMenuItem, "setAsOriginalToolStripMenuItem");
            this.setAsOriginalToolStripMenuItem.Name = "setAsOriginalToolStripMenuItem";
            this.setAsOriginalToolStripMenuItem.Click += new System.EventHandler(this.setAsOriginalToolStripMenuItem_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblImageSource);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragDrop);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragEnter);
            // 
            // lblImageSource
            // 
            resources.ApplyResources(this.lblImageSource, "lblImageSource");
            this.lblImageSource.BackColor = System.Drawing.Color.White;
            this.lblImageSource.Name = "lblImageSource";
            this.toolTip1.SetToolTip(this.lblImageSource, resources.GetString("lblImageSource.ToolTip"));
            // 
            // lblHueShift
            // 
            resources.ApplyResources(this.lblHueShift, "lblHueShift");
            this.lblHueShift.Name = "lblHueShift";
            this.toolTip1.SetToolTip(this.lblHueShift, resources.GetString("lblHueShift.ToolTip"));
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // tBarHueShift
            // 
            resources.ApplyResources(this.tBarHueShift, "tBarHueShift");
            this.tBarHueShift.Maximum = 359;
            this.tBarHueShift.Name = "tBarHueShift";
            this.tBarHueShift.TickFrequency = 30;
            this.toolTip1.SetToolTip(this.tBarHueShift, resources.GetString("tBarHueShift.ToolTip"));
            this.tBarHueShift.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // nUDMaxColors
            // 
            resources.ApplyResources(this.nUDMaxColors, "nUDMaxColors");
            this.nUDMaxColors.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nUDMaxColors.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nUDMaxColors.Name = "nUDMaxColors";
            this.toolTip1.SetToolTip(this.nUDMaxColors, resources.GetString("nUDMaxColors.ToolTip"));
            this.nUDMaxColors.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nUDMaxColors.ValueChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // lblColors
            // 
            resources.ApplyResources(this.lblColors, "lblColors");
            this.lblColors.BackColor = System.Drawing.Color.White;
            this.lblColors.Name = "lblColors";
            this.toolTip1.SetToolTip(this.lblColors, resources.GetString("lblColors.ToolTip"));
            // 
            // cbExceptColor
            // 
            resources.ApplyResources(this.cbExceptColor, "cbExceptColor");
            this.cbExceptColor.BackColor = System.Drawing.Color.White;
            this.cbExceptColor.Name = "cbExceptColor";
            this.toolTip1.SetToolTip(this.cbExceptColor, resources.GetString("cbExceptColor.ToolTip"));
            this.cbExceptColor.UseVisualStyleBackColor = false;
            this.cbExceptColor.CheckedChanged += new System.EventHandler(this.cbExceptColor_CheckedChanged);
            this.cbExceptColor.BackColorChanged += new System.EventHandler(this.cbExceptColor_BackColorChanged);
            // 
            // cbSkipToolOrder
            // 
            resources.ApplyResources(this.cbSkipToolOrder, "cbSkipToolOrder");
            this.cbSkipToolOrder.Name = "cbSkipToolOrder";
            this.toolTip1.SetToolTip(this.cbSkipToolOrder, resources.GetString("cbSkipToolOrder.ToolTip"));
            this.cbSkipToolOrder.UseVisualStyleBackColor = true;
            this.cbSkipToolOrder.CheckedChanged += new System.EventHandler(this.justShowResult);
            // 
            // nUDZBot
            // 
            resources.ApplyResources(this.nUDZBot, "nUDZBot");
            this.nUDZBot.DecimalPlaces = 2;
            this.nUDZBot.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nUDZBot.Name = "nUDZBot";
            this.toolTip1.SetToolTip(this.nUDZBot, resources.GetString("nUDZBot.ToolTip"));
            this.nUDZBot.Value = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            // 
            // tBRMin
            // 
            resources.ApplyResources(this.tBRMin, "tBRMin");
            this.tBRMin.Maximum = 255;
            this.tBRMin.Name = "tBRMin";
            this.tBRMin.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBRMin, resources.GetString("tBRMin.ToolTip"));
            this.tBRMin.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tBRMax
            // 
            resources.ApplyResources(this.tBRMax, "tBRMax");
            this.tBRMax.Maximum = 255;
            this.tBRMax.Name = "tBRMax";
            this.tBRMax.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBRMax, resources.GetString("tBRMax.ToolTip"));
            this.tBRMax.Value = 255;
            this.tBRMax.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tBGMax
            // 
            resources.ApplyResources(this.tBGMax, "tBGMax");
            this.tBGMax.Maximum = 255;
            this.tBGMax.Name = "tBGMax";
            this.tBGMax.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBGMax, resources.GetString("tBGMax.ToolTip"));
            this.tBGMax.Value = 255;
            this.tBGMax.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tBGMin
            // 
            resources.ApplyResources(this.tBGMin, "tBGMin");
            this.tBGMin.Maximum = 255;
            this.tBGMin.Name = "tBGMin";
            this.tBGMin.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBGMin, resources.GetString("tBGMin.ToolTip"));
            this.tBGMin.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tBBMax
            // 
            resources.ApplyResources(this.tBBMax, "tBBMax");
            this.tBBMax.Maximum = 255;
            this.tBBMax.Name = "tBBMax";
            this.tBBMax.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBBMax, resources.GetString("tBBMax.ToolTip"));
            this.tBBMax.Value = 255;
            this.tBBMax.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tBBMin
            // 
            resources.ApplyResources(this.tBBMin, "tBBMin");
            this.tBBMin.Maximum = 255;
            this.tBBMin.Name = "tBBMin";
            this.tBBMin.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBBMin, resources.GetString("tBBMin.ToolTip"));
            this.tBBMin.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            this.toolTip1.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            this.toolTip1.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            this.toolTip1.SetToolTip(this.label15, resources.GetString("label15.ToolTip"));
            // 
            // rBMode0
            // 
            resources.ApplyResources(this.rBMode0, "rBMode0");
            this.rBMode0.Checked = true;
            this.rBMode0.Name = "rBMode0";
            this.rBMode0.TabStop = true;
            this.toolTip1.SetToolTip(this.rBMode0, resources.GetString("rBMode0.ToolTip"));
            this.rBMode0.UseVisualStyleBackColor = true;
            this.rBMode0.CheckedChanged += new System.EventHandler(this.rBMode0_CheckedChanged);
            // 
            // rBMode1
            // 
            resources.ApplyResources(this.rBMode1, "rBMode1");
            this.rBMode1.Name = "rBMode1";
            this.toolTip1.SetToolTip(this.rBMode1, resources.GetString("rBMode1.ToolTip"));
            this.rBMode1.UseVisualStyleBackColor = true;
            this.rBMode1.CheckedChanged += new System.EventHandler(this.rBMode0_CheckedChanged);
            // 
            // rBMode2
            // 
            resources.ApplyResources(this.rBMode2, "rBMode2");
            this.rBMode2.Name = "rBMode2";
            this.toolTip1.SetToolTip(this.rBMode2, resources.GetString("rBMode2.ToolTip"));
            this.rBMode2.UseVisualStyleBackColor = true;
            this.rBMode2.CheckedChanged += new System.EventHandler(this.rBMode0_CheckedChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.toolTip1.SetToolTip(this.btnTest, resources.GetString("btnTest.ToolTip"));
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCheckOrig_MouseDown);
            this.btnTest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnCheckOrig_MouseUp);
            // 
            // nUDColorPercent
            // 
            resources.ApplyResources(this.nUDColorPercent, "nUDColorPercent");
            this.nUDColorPercent.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDColorPercent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDColorPercent.Name = "nUDColorPercent";
            this.toolTip1.SetToolTip(this.nUDColorPercent, resources.GetString("nUDColorPercent.ToolTip"));
            this.nUDColorPercent.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // btnPresetCorrection3
            // 
            resources.ApplyResources(this.btnPresetCorrection3, "btnPresetCorrection3");
            this.btnPresetCorrection3.Name = "btnPresetCorrection3";
            this.toolTip1.SetToolTip(this.btnPresetCorrection3, resources.GetString("btnPresetCorrection3.ToolTip"));
            this.btnPresetCorrection3.UseVisualStyleBackColor = true;
            this.btnPresetCorrection3.Click += new System.EventHandler(this.btnPresetCorrection3_Click);
            // 
            // btnShowSettings
            // 
            resources.ApplyResources(this.btnShowSettings, "btnShowSettings");
            this.btnShowSettings.Name = "btnShowSettings";
            this.toolTip1.SetToolTip(this.btnShowSettings, resources.GetString("btnShowSettings.ToolTip"));
            this.btnShowSettings.UseVisualStyleBackColor = true;
            this.btnShowSettings.Click += new System.EventHandler(this.btnShowSettings_Click);
            // 
            // btnShowOrig
            // 
            resources.ApplyResources(this.btnShowOrig, "btnShowOrig");
            this.btnShowOrig.Name = "btnShowOrig";
            this.toolTip1.SetToolTip(this.btnShowOrig, resources.GetString("btnShowOrig.ToolTip"));
            this.btnShowOrig.UseVisualStyleBackColor = true;
            this.btnShowOrig.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShowOrig_MouseDown);
            this.btnShowOrig.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShowOrig_MouseUp);
            // 
            // nUDGCodeOutlineSmooth
            // 
            resources.ApplyResources(this.nUDGCodeOutlineSmooth, "nUDGCodeOutlineSmooth");
            this.nUDGCodeOutlineSmooth.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDGCodeOutlineSmooth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDGCodeOutlineSmooth.Name = "nUDGCodeOutlineSmooth";
            this.toolTip1.SetToolTip(this.nUDGCodeOutlineSmooth, resources.GetString("nUDGCodeOutlineSmooth.ToolTip"));
            this.nUDGCodeOutlineSmooth.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // cBGCodeOutlineShrink
            // 
            resources.ApplyResources(this.cBGCodeOutlineShrink, "cBGCodeOutlineShrink");
            this.cBGCodeOutlineShrink.Checked = true;
            this.cBGCodeOutlineShrink.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeOutlineShrink.Name = "cBGCodeOutlineShrink";
            this.toolTip1.SetToolTip(this.cBGCodeOutlineShrink, resources.GetString("cBGCodeOutlineShrink.ToolTip"));
            this.cBGCodeOutlineShrink.UseVisualStyleBackColor = true;
            // 
            // nUDSMax
            // 
            resources.ApplyResources(this.nUDSMax, "nUDSMax");
            this.nUDSMax.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDSMax.Name = "nUDSMax";
            this.toolTip1.SetToolTip(this.nUDSMax, resources.GetString("nUDSMax.ToolTip"));
            this.nUDSMax.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.rBMode2);
            this.groupBox6.Controls.Add(this.rBMode1);
            this.groupBox6.Controls.Add(this.rBMode0);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.BackColor = System.Drawing.Color.Yellow;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPictureToolStripMenuItem,
            this.pasteFromClipboardToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.toolTip1.SetToolTip(this.menuStrip1, resources.GetString("menuStrip1.ToolTip"));
            // 
            // loadPictureToolStripMenuItem
            // 
            resources.ApplyResources(this.loadPictureToolStripMenuItem, "loadPictureToolStripMenuItem");
            this.loadPictureToolStripMenuItem.Name = "loadPictureToolStripMenuItem";
            this.loadPictureToolStripMenuItem.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // pasteFromClipboardToolStripMenuItem
            // 
            resources.ApplyResources(this.pasteFromClipboardToolStripMenuItem, "pasteFromClipboardToolStripMenuItem");
            this.pasteFromClipboardToolStripMenuItem.Name = "pasteFromClipboardToolStripMenuItem";
            this.pasteFromClipboardToolStripMenuItem.Click += new System.EventHandler(this.pasteFromClipboardToolStripMenuItem_Click);
            // 
            // rBProcessTool
            // 
            resources.ApplyResources(this.rBProcessTool, "rBProcessTool");
            this.rBProcessTool.Checked = true;
            this.rBProcessTool.Name = "rBProcessTool";
            this.rBProcessTool.TabStop = true;
            this.toolTip1.SetToolTip(this.rBProcessTool, resources.GetString("rBProcessTool.ToolTip"));
            this.rBProcessTool.UseVisualStyleBackColor = true;
            this.rBProcessTool.CheckedChanged += new System.EventHandler(this.rBProcessZ_CheckedChanged);
            // 
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Controls.Add(this.rBGrayS);
            this.groupBox8.Controls.Add(this.rBGrayZ);
            this.groupBox8.Controls.Add(this.label22);
            this.groupBox8.Controls.Add(this.nUDSMin);
            this.groupBox8.Controls.Add(this.label23);
            this.groupBox8.Controls.Add(this.nUDSMax);
            this.groupBox8.Controls.Add(this.label8);
            this.groupBox8.Controls.Add(this.nUDZTop);
            this.groupBox8.Controls.Add(this.label9);
            this.groupBox8.Controls.Add(this.nUDZBot);
            this.groupBox8.Controls.Add(this.cBCompress);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox8, resources.GetString("groupBox8.ToolTip"));
            // 
            // rBGrayS
            // 
            resources.ApplyResources(this.rBGrayS, "rBGrayS");
            this.rBGrayS.Name = "rBGrayS";
            this.toolTip1.SetToolTip(this.rBGrayS, resources.GetString("rBGrayS.ToolTip"));
            this.rBGrayS.UseVisualStyleBackColor = true;
            // 
            // rBGrayZ
            // 
            resources.ApplyResources(this.rBGrayZ, "rBGrayZ");
            this.rBGrayZ.Checked = true;
            this.rBGrayZ.Name = "rBGrayZ";
            this.rBGrayZ.TabStop = true;
            this.toolTip1.SetToolTip(this.rBGrayZ, resources.GetString("rBGrayZ.ToolTip"));
            this.rBGrayZ.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            this.toolTip1.SetToolTip(this.label22, resources.GetString("label22.ToolTip"));
            // 
            // nUDSMin
            // 
            resources.ApplyResources(this.nUDSMin, "nUDSMin");
            this.nUDSMin.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDSMin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDSMin.Name = "nUDSMin";
            this.toolTip1.SetToolTip(this.nUDSMin, resources.GetString("nUDSMin.ToolTip"));
            this.nUDSMin.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            this.toolTip1.SetToolTip(this.label23, resources.GetString("label23.ToolTip"));
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // nUDZTop
            // 
            resources.ApplyResources(this.nUDZTop, "nUDZTop");
            this.nUDZTop.DecimalPlaces = 2;
            this.nUDZTop.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nUDZTop.Name = "nUDZTop";
            this.toolTip1.SetToolTip(this.nUDZTop, resources.GetString("nUDZTop.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            this.toolTip1.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // cBCompress
            // 
            resources.ApplyResources(this.cBCompress, "cBCompress");
            this.cBCompress.Checked = true;
            this.cBCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBCompress.Name = "cBCompress";
            this.toolTip1.SetToolTip(this.cBCompress, resources.GetString("cBCompress.ToolTip"));
            this.cBCompress.UseVisualStyleBackColor = true;
            // 
            // rBProcessZ
            // 
            resources.ApplyResources(this.rBProcessZ, "rBProcessZ");
            this.rBProcessZ.Name = "rBProcessZ";
            this.toolTip1.SetToolTip(this.rBProcessZ, resources.GetString("rBProcessZ.ToolTip"));
            this.rBProcessZ.UseVisualStyleBackColor = true;
            this.rBProcessZ.CheckedChanged += new System.EventHandler(this.rBProcessZ_CheckedChanged);
            // 
            // cBPreview
            // 
            resources.ApplyResources(this.cBPreview, "cBPreview");
            this.cBPreview.Name = "cBPreview";
            this.toolTip1.SetToolTip(this.cBPreview, resources.GetString("cBPreview.ToolTip"));
            this.cBPreview.UseVisualStyleBackColor = true;
            this.cBPreview.CheckedChanged += new System.EventHandler(this.justShowResult);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip1.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            // 
            // tabPage4
            // 
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.groupBox9);
            this.tabPage4.Controls.Add(this.tBToolList);
            this.tabPage4.Name = "tabPage4";
            this.toolTip1.SetToolTip(this.tabPage4, resources.GetString("tabPage4.ToolTip"));
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.cBReduceColorsToolTable);
            this.groupBox5.Controls.Add(this.nUDMaxColors);
            this.groupBox5.Controls.Add(this.cbExceptColor);
            this.groupBox5.Controls.Add(this.cBReduceColorsDithering);
            this.groupBox5.Controls.Add(this.lblColors);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox5, resources.GetString("groupBox5.ToolTip"));
            // 
            // cBReduceColorsToolTable
            // 
            resources.ApplyResources(this.cBReduceColorsToolTable, "cBReduceColorsToolTable");
            this.cBReduceColorsToolTable.Name = "cBReduceColorsToolTable";
            this.toolTip1.SetToolTip(this.cBReduceColorsToolTable, resources.GetString("cBReduceColorsToolTable.ToolTip"));
            this.cBReduceColorsToolTable.UseVisualStyleBackColor = true;
            this.cBReduceColorsToolTable.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // cBReduceColorsDithering
            // 
            resources.ApplyResources(this.cBReduceColorsDithering, "cBReduceColorsDithering");
            this.cBReduceColorsDithering.Name = "cBReduceColorsDithering";
            this.toolTip1.SetToolTip(this.cBReduceColorsDithering, resources.GetString("cBReduceColorsDithering.ToolTip"));
            this.cBReduceColorsDithering.UseVisualStyleBackColor = true;
            this.cBReduceColorsDithering.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // groupBox9
            // 
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Controls.Add(this.btnPresetCorrection4);
            this.groupBox9.Controls.Add(this.label20);
            this.groupBox9.Controls.Add(this.nUDColorPercent);
            this.groupBox9.Controls.Add(this.btnPresetCorrection3);
            this.groupBox9.Controls.Add(this.btnPresetCorrection2);
            this.groupBox9.Controls.Add(this.btnResetCorrection);
            this.groupBox9.Controls.Add(this.btnPresetCorrection1);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox9, resources.GetString("groupBox9.ToolTip"));
            // 
            // btnPresetCorrection4
            // 
            resources.ApplyResources(this.btnPresetCorrection4, "btnPresetCorrection4");
            this.btnPresetCorrection4.Name = "btnPresetCorrection4";
            this.toolTip1.SetToolTip(this.btnPresetCorrection4, resources.GetString("btnPresetCorrection4.ToolTip"));
            this.btnPresetCorrection4.UseVisualStyleBackColor = true;
            this.btnPresetCorrection4.Click += new System.EventHandler(this.btnPresetCorrection4_Click);
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            this.toolTip1.SetToolTip(this.label20, resources.GetString("label20.ToolTip"));
            // 
            // btnPresetCorrection2
            // 
            resources.ApplyResources(this.btnPresetCorrection2, "btnPresetCorrection2");
            this.btnPresetCorrection2.Name = "btnPresetCorrection2";
            this.toolTip1.SetToolTip(this.btnPresetCorrection2, resources.GetString("btnPresetCorrection2.ToolTip"));
            this.btnPresetCorrection2.UseVisualStyleBackColor = true;
            this.btnPresetCorrection2.Click += new System.EventHandler(this.btnPresetCorrection2_Click);
            // 
            // btnResetCorrection
            // 
            resources.ApplyResources(this.btnResetCorrection, "btnResetCorrection");
            this.btnResetCorrection.Name = "btnResetCorrection";
            this.toolTip1.SetToolTip(this.btnResetCorrection, resources.GetString("btnResetCorrection.ToolTip"));
            this.btnResetCorrection.UseVisualStyleBackColor = true;
            this.btnResetCorrection.Click += new System.EventHandler(this.btnResetCorrection_Click);
            // 
            // btnPresetCorrection1
            // 
            resources.ApplyResources(this.btnPresetCorrection1, "btnPresetCorrection1");
            this.btnPresetCorrection1.Name = "btnPresetCorrection1";
            this.toolTip1.SetToolTip(this.btnPresetCorrection1, resources.GetString("btnPresetCorrection1.ToolTip"));
            this.btnPresetCorrection1.UseVisualStyleBackColor = true;
            this.btnPresetCorrection1.Click += new System.EventHandler(this.btnPresetCorrection1_Click);
            // 
            // tBToolList
            // 
            resources.ApplyResources(this.tBToolList, "tBToolList");
            this.tBToolList.Name = "tBToolList";
            this.tBToolList.ReadOnly = true;
            this.toolTip1.SetToolTip(this.tBToolList, resources.GetString("tBToolList.ToolTip"));
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.lblSaturation);
            this.tabPage1.Controls.Add(this.tBarSaturation);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.lblBrightness);
            this.tabPage1.Controls.Add(this.tBarGamma);
            this.tabPage1.Controls.Add(this.lblContrast);
            this.tabPage1.Controls.Add(this.btnInvert);
            this.tabPage1.Controls.Add(this.tBarBrightness);
            this.tabPage1.Controls.Add(this.btnHorizMirror);
            this.tabPage1.Controls.Add(this.tBarContrast);
            this.tabPage1.Controls.Add(this.btnVertMirror);
            this.tabPage1.Controls.Add(this.lblGamma);
            this.tabPage1.Controls.Add(this.btnRotateRight);
            this.tabPage1.Controls.Add(this.btnRotateLeft);
            this.tabPage1.Name = "tabPage1";
            this.toolTip1.SetToolTip(this.tabPage1, resources.GetString("tabPage1.ToolTip"));
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // lblSaturation
            // 
            resources.ApplyResources(this.lblSaturation, "lblSaturation");
            this.lblSaturation.Name = "lblSaturation";
            this.toolTip1.SetToolTip(this.lblSaturation, resources.GetString("lblSaturation.ToolTip"));
            // 
            // tBarSaturation
            // 
            resources.ApplyResources(this.tBarSaturation, "tBarSaturation");
            this.tBarSaturation.Maximum = 255;
            this.tBarSaturation.Minimum = -255;
            this.tBarSaturation.Name = "tBarSaturation";
            this.tBarSaturation.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBarSaturation, resources.GetString("tBarSaturation.ToolTip"));
            this.tBarSaturation.Scroll += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.cBFilterRemoveArtefact);
            this.tabPage2.Controls.Add(this.cBFilterHistogram);
            this.tabPage2.Controls.Add(this.lblCFB);
            this.tabPage2.Controls.Add(this.lblCFG);
            this.tabPage2.Controls.Add(this.lblCFR);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.tBBMax);
            this.tabPage2.Controls.Add(this.tBBMin);
            this.tabPage2.Controls.Add(this.tBGMax);
            this.tabPage2.Controls.Add(this.tBGMin);
            this.tabPage2.Controls.Add(this.tBRMax);
            this.tabPage2.Controls.Add(this.tBRMin);
            this.tabPage2.Controls.Add(this.cBFilterEdge);
            this.tabPage2.Controls.Add(this.cBPosterize);
            this.tabPage2.Controls.Add(this.lblHueShift);
            this.tabPage2.Controls.Add(this.tBarHueShift);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Name = "tabPage2";
            this.toolTip1.SetToolTip(this.tabPage2, resources.GetString("tabPage2.ToolTip"));
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cBFilterRemoveArtefact
            // 
            resources.ApplyResources(this.cBFilterRemoveArtefact, "cBFilterRemoveArtefact");
            this.cBFilterRemoveArtefact.Name = "cBFilterRemoveArtefact";
            this.toolTip1.SetToolTip(this.cBFilterRemoveArtefact, resources.GetString("cBFilterRemoveArtefact.ToolTip"));
            this.cBFilterRemoveArtefact.UseVisualStyleBackColor = true;
            this.cBFilterRemoveArtefact.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // cBFilterHistogram
            // 
            resources.ApplyResources(this.cBFilterHistogram, "cBFilterHistogram");
            this.cBFilterHistogram.Name = "cBFilterHistogram";
            this.toolTip1.SetToolTip(this.cBFilterHistogram, resources.GetString("cBFilterHistogram.ToolTip"));
            this.cBFilterHistogram.UseVisualStyleBackColor = true;
            this.cBFilterHistogram.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // lblCFB
            // 
            resources.ApplyResources(this.lblCFB, "lblCFB");
            this.lblCFB.Name = "lblCFB";
            this.toolTip1.SetToolTip(this.lblCFB, resources.GetString("lblCFB.ToolTip"));
            // 
            // lblCFG
            // 
            resources.ApplyResources(this.lblCFG, "lblCFG");
            this.lblCFG.Name = "lblCFG";
            this.toolTip1.SetToolTip(this.lblCFG, resources.GetString("lblCFG.ToolTip"));
            // 
            // lblCFR
            // 
            resources.ApplyResources(this.lblCFR, "lblCFR");
            this.lblCFR.Name = "lblCFR";
            this.toolTip1.SetToolTip(this.lblCFR, resources.GetString("lblCFR.ToolTip"));
            // 
            // cBFilterEdge
            // 
            resources.ApplyResources(this.cBFilterEdge, "cBFilterEdge");
            this.cBFilterEdge.Name = "cBFilterEdge";
            this.toolTip1.SetToolTip(this.cBFilterEdge, resources.GetString("cBFilterEdge.ToolTip"));
            this.cBFilterEdge.UseVisualStyleBackColor = true;
            this.cBFilterEdge.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // cBPosterize
            // 
            resources.ApplyResources(this.cBPosterize, "cBPosterize");
            this.cBPosterize.Name = "cBPosterize";
            this.toolTip1.SetToolTip(this.cBPosterize, resources.GetString("cBPosterize.ToolTip"));
            this.cBPosterize.UseVisualStyleBackColor = true;
            this.cBPosterize.CheckedChanged += new System.EventHandler(this.applyColorCorrectionsEvent);
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Controls.Add(this.label21);
            this.tabPage3.Controls.Add(this.cLBTools);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Controls.Add(this.cbSkipToolOrder);
            this.tabPage3.Name = "tabPage3";
            this.toolTip1.SetToolTip(this.tabPage3, resources.GetString("tabPage3.ToolTip"));
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            this.toolTip1.SetToolTip(this.label21, resources.GetString("label21.ToolTip"));
            // 
            // cLBTools
            // 
            resources.ApplyResources(this.cLBTools, "cLBTools");
            this.cLBTools.CheckOnClick = true;
            this.cLBTools.FormattingEnabled = true;
            this.cLBTools.Name = "cLBTools";
            this.toolTip1.SetToolTip(this.cLBTools, resources.GetString("cLBTools.ToolTip"));
            this.cLBTools.SelectedIndexChanged += new System.EventHandler(this.cLBTools_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Controls.Add(this.groupBox2);
            this.tabPage5.Controls.Add(this.gBgcodeSelection);
            this.tabPage5.Controls.Add(this.gBgcodeDirection);
            this.tabPage5.Name = "tabPage5";
            this.toolTip1.SetToolTip(this.tabPage5, resources.GetString("tabPage5.ToolTip"));
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.cBResolutionPenWidth);
            this.groupBox2.Controls.Add(this.rBProcessTool);
            this.groupBox2.Controls.Add(this.rBProcessZ);
            this.groupBox2.Controls.Add(this.groupBox8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // cBResolutionPenWidth
            // 
            resources.ApplyResources(this.cBResolutionPenWidth, "cBResolutionPenWidth");
            this.cBResolutionPenWidth.Checked = global::GRBL_Plotter.Properties.Settings.Default.importImageResoApply;
            this.cBResolutionPenWidth.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importImageResoApply", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBResolutionPenWidth.Name = "cBResolutionPenWidth";
            this.toolTip1.SetToolTip(this.cBResolutionPenWidth, resources.GetString("cBResolutionPenWidth.ToolTip"));
            this.cBResolutionPenWidth.UseVisualStyleBackColor = true;
            // 
            // gBgcodeSelection
            // 
            resources.ApplyResources(this.gBgcodeSelection, "gBgcodeSelection");
            this.gBgcodeSelection.Controls.Add(this.cBGCodeFill);
            this.gBgcodeSelection.Controls.Add(this.cBGCodeOutlineShrink);
            this.gBgcodeSelection.Controls.Add(this.cBGCodeOutline);
            this.gBgcodeSelection.Controls.Add(this.nUDGCodeOutlineSmooth);
            this.gBgcodeSelection.Controls.Add(this.cBGCodeOutlineSmooth);
            this.gBgcodeSelection.Name = "gBgcodeSelection";
            this.gBgcodeSelection.TabStop = false;
            this.toolTip1.SetToolTip(this.gBgcodeSelection, resources.GetString("gBgcodeSelection.ToolTip"));
            // 
            // cBGCodeFill
            // 
            resources.ApplyResources(this.cBGCodeFill, "cBGCodeFill");
            this.cBGCodeFill.Checked = true;
            this.cBGCodeFill.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeFill.Name = "cBGCodeFill";
            this.toolTip1.SetToolTip(this.cBGCodeFill, resources.GetString("cBGCodeFill.ToolTip"));
            this.cBGCodeFill.UseVisualStyleBackColor = true;
            // 
            // cBGCodeOutline
            // 
            resources.ApplyResources(this.cBGCodeOutline, "cBGCodeOutline");
            this.cBGCodeOutline.Checked = true;
            this.cBGCodeOutline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeOutline.Name = "cBGCodeOutline";
            this.toolTip1.SetToolTip(this.cBGCodeOutline, resources.GetString("cBGCodeOutline.ToolTip"));
            this.cBGCodeOutline.UseVisualStyleBackColor = true;
            this.cBGCodeOutline.CheckedChanged += new System.EventHandler(this.cBGCodeOutline_CheckedChanged);
            // 
            // cBGCodeOutlineSmooth
            // 
            resources.ApplyResources(this.cBGCodeOutlineSmooth, "cBGCodeOutlineSmooth");
            this.cBGCodeOutlineSmooth.Checked = true;
            this.cBGCodeOutlineSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeOutlineSmooth.Name = "cBGCodeOutlineSmooth";
            this.toolTip1.SetToolTip(this.cBGCodeOutlineSmooth, resources.GetString("cBGCodeOutlineSmooth.ToolTip"));
            this.cBGCodeOutlineSmooth.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            this.toolTip1.SetToolTip(this.label24, resources.GetString("label24.ToolTip"));
            // 
            // lBUseCase
            // 
            resources.ApplyResources(this.lBUseCase, "lBUseCase");
            this.lBUseCase.FormattingEnabled = true;
            this.lBUseCase.Name = "lBUseCase";
            this.toolTip1.SetToolTip(this.lBUseCase, resources.GetString("lBUseCase.ToolTip"));
            // 
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.toolTip1.SetToolTip(this.btnLoad, resources.GetString("btnLoad.ToolTip"));
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click_1);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            this.toolTip1.SetToolTip(this.label16, resources.GetString("label16.ToolTip"));
            // 
            // lblLastUseCase
            // 
            resources.ApplyResources(this.lblLastUseCase, "lblLastUseCase");
            this.lblLastUseCase.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "useCaseLastLoaded", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblLastUseCase.Name = "lblLastUseCase";
            this.lblLastUseCase.Text = global::GRBL_Plotter.Properties.Settings.Default.useCaseLastLoaded;
            this.toolTip1.SetToolTip(this.lblLastUseCase, resources.GetString("lblLastUseCase.ToolTip"));
            // 
            // GCodeFromImage
            // 
            resources.ApplyResources(this, "$this");
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblLastUseCase);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lBUseCase);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.btnShowOrig);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnShowSettings);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cBPreview);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GCodeFromImage";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GCodeFromImage_FormClosing);
            this.Load += new System.EventHandler(this.ImageToGCode_Load);
            this.SizeChanged += new System.EventHandler(this.GCodeFromImage_Resize);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GCodeFromImage_KeyDown);
            this.Resize += new System.EventHandler(this.GCodeFromImage_Resize);
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
            this.gBgcodeDirection.ResumeLayout(false);
            this.gBgcodeDirection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarHueShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMaxColors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZBot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDColorPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGCodeOutlineSmooth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSMax)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZTop)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarSaturation)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gBgcodeSelection.ResumeLayout(false);
            this.gBgcodeSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.GroupBox gBgcodeDirection;
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
        private System.Windows.Forms.NumericUpDown nUDReso;
        private System.Windows.Forms.CheckBox cbGrayscale;
        private System.Windows.Forms.CheckBox cbExceptColor;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbSkipToolOrder;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadPictureToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown nUDZBot;
        private System.Windows.Forms.NumericUpDown nUDZTop;
        private System.Windows.Forms.RadioButton rBProcessTool;
        private System.Windows.Forms.RadioButton rBProcessZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cBCompress;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label lblColors;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem pasteFromClipboardToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBPreview;
        private System.Windows.Forms.Label lblSizeResult;
        private System.Windows.Forms.NumericUpDown nUDMaxColors;
        private System.Windows.Forms.TrackBar tBarHueShift;
        private System.Windows.Forms.Label lblHueShift;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox cBPosterize;
        private System.Windows.Forms.CheckBox cBFilterEdge;
        private System.Windows.Forms.TrackBar tBarSaturation;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblSaturation;
        private System.Windows.Forms.TrackBar tBBMax;
        private System.Windows.Forms.TrackBar tBBMin;
        private System.Windows.Forms.TrackBar tBGMax;
        private System.Windows.Forms.TrackBar tBGMin;
        private System.Windows.Forms.TrackBar tBRMax;
        private System.Windows.Forms.TrackBar tBRMin;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RadioButton rBMode2;
        private System.Windows.Forms.RadioButton rBMode1;
        private System.Windows.Forms.RadioButton rBMode0;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCFB;
        private System.Windows.Forms.Label lblCFG;
        private System.Windows.Forms.Label lblCFR;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBReduceColorsToolTable;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button btnPresetCorrection3;
        private System.Windows.Forms.Button btnPresetCorrection2;
        private System.Windows.Forms.Button btnResetCorrection;
        private System.Windows.Forms.Button btnPresetCorrection1;
        private System.Windows.Forms.Button btnShowSettings;
        private System.Windows.Forms.TextBox tBToolList;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.NumericUpDown nUDColorPercent;
        private System.Windows.Forms.CheckBox cBReduceColorsDithering;
        private System.Windows.Forms.CheckBox cBFilterHistogram;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label lblImageSource;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnPresetCorrection4;
        private System.Windows.Forms.Button btnShowOrig;
        private System.Windows.Forms.CheckBox cBFilterRemoveArtefact;
        private System.Windows.Forms.ToolStripMenuItem setAsOriginalToolStripMenuItem;
        private System.Windows.Forms.CheckBox cBGCodeOutline;
        private System.Windows.Forms.CheckBox cBGCodeFill;
        private System.Windows.Forms.Label lblSizeOrig;
        private System.Windows.Forms.Button btnKeepSizeReso;
        private System.Windows.Forms.Button btnKeepSizeWidth;
        private System.Windows.Forms.CheckedListBox cLBTools;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.CheckBox cBGCodeOutlineSmooth;
        private System.Windows.Forms.NumericUpDown nUDGCodeOutlineSmooth;
        private System.Windows.Forms.CheckBox cBGCodeOutlineShrink;
        private System.Windows.Forms.Label lblInfo1;
        private System.Windows.Forms.RadioButton rBGrayS;
        private System.Windows.Forms.RadioButton rBGrayZ;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown nUDSMin;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NumericUpDown nUDSMax;
        private System.Windows.Forms.GroupBox gBgcodeSelection;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cBResolutionPenWidth;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.ListBox lBUseCase;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblLastUseCase;
    }
}