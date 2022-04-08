namespace GrblPlotter
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
            this.LblMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblInfo1 = new System.Windows.Forms.Label();
            this.btnKeepSizeReso = new System.Windows.Forms.Button();
            this.btnKeepSizeWidth = new System.Windows.Forms.Button();
            this.lblSizeOrig = new System.Windows.Forms.Label();
            this.nUDResoX = new System.Windows.Forms.NumericUpDown();
            this.lblSizeResult = new System.Windows.Forms.Label();
            this.nUDHeight = new System.Windows.Forms.NumericUpDown();
            this.nUDWidth = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbLockRatio = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.lblGamma = new System.Windows.Forms.Label();
            this.lblContrast = new System.Windows.Forms.Label();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.RbChannelK = new System.Windows.Forms.RadioButton();
            this.RbChannelY = new System.Windows.Forms.RadioButton();
            this.RbChannelM = new System.Windows.Forms.RadioButton();
            this.RbChannelC = new System.Windows.Forms.RadioButton();
            this.RbChannelB = new System.Windows.Forms.RadioButton();
            this.RbChannelG = new System.Windows.Forms.RadioButton();
            this.RbChannelR = new System.Windows.Forms.RadioButton();
            this.cbGrayscaleChannel = new System.Windows.Forms.CheckBox();
            this.cbGrayscale = new System.Windows.Forms.CheckBox();
            this.rbModeDither = new System.Windows.Forms.RadioButton();
            this.rbModeGray = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.LblThreshold = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tBarBWThreshold = new System.Windows.Forms.TrackBar();
            this.CbBlackWhiteEnable = new System.Windows.Forms.CheckBox();
            this.btnInvert = new System.Windows.Forms.Button();
            this.btnHorizMirror = new System.Windows.Forms.Button();
            this.btnVertMirror = new System.Windows.Forms.Button();
            this.btnRotateRight = new System.Windows.Forms.Button();
            this.btnRotateLeft = new System.Windows.Forms.Button();
            this.tBarGamma = new System.Windows.Forms.TrackBar();
            this.tBarContrast = new System.Windows.Forms.TrackBar();
            this.tBarBrightness = new System.Windows.Forms.TrackBar();
            this.GbGcodeDirection = new System.Windows.Forms.GroupBox();
            this.label27 = new System.Windows.Forms.Label();
            this.CBoxPatternFiles = new System.Windows.Forms.ComboBox();
            this.BtnReloadPattern = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.CbPenUpOn0 = new System.Windows.Forms.CheckBox();
            this.nUDResoY = new System.Windows.Forms.NumericUpDown();
            this.cBCompress = new System.Windows.Forms.CheckBox();
            this.Cb2DViewHide0 = new System.Windows.Forms.CheckBox();
            this.rbEngravingPattern4 = new System.Windows.Forms.RadioButton();
            this.rbEngravingPattern3 = new System.Windows.Forms.RadioButton();
            this.label25 = new System.Windows.Forms.Label();
            this.cBOnlyLeftToRight = new System.Windows.Forms.CheckBox();
            this.NuDSpiralCenterY = new System.Windows.Forms.NumericUpDown();
            this.rbEngravingPattern2 = new System.Windows.Forms.RadioButton();
            this.rbEngravingPattern1 = new System.Windows.Forms.RadioButton();
            this.NuDSpiralCenterX = new System.Windows.Forms.NumericUpDown();
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
            this.btnTest = new System.Windows.Forms.Button();
            this.nUDColorPercent = new System.Windows.Forms.NumericUpDown();
            this.btnPresetCorrection3 = new System.Windows.Forms.Button();
            this.btnShowSettings = new System.Windows.Forms.Button();
            this.btnShowOrig = new System.Windows.Forms.Button();
            this.nUDGCodeOutlineSmooth = new System.Windows.Forms.NumericUpDown();
            this.cBGCodeOutlineShrink = new System.Windows.Forms.CheckBox();
            this.btnGetPWMValues = new System.Windows.Forms.Button();
            this.cBToolChange = new System.Windows.Forms.CheckBox();
            this.cBImportGCToolM0 = new System.Windows.Forms.CheckBox();
            this.cBImportGCTool = new System.Windows.Forms.CheckBox();
            this.nUDSBottom = new System.Windows.Forms.NumericUpDown();
            this.nUDZBottom = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GbStartGrayZ = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nUDZTop = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.RbStartGrayS = new System.Windows.Forms.RadioButton();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.cBPreview = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSetup = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cBResolutionPenWidth = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.GbUseCaseLoad = new System.Windows.Forms.GroupBox();
            this.lBUseCase = new System.Windows.Forms.ListBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.lblLastUseCase = new System.Windows.Forms.Label();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage2Color = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.CboxToolTable = new System.Windows.Forms.ComboBox();
            this.CboxToolFiles = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.tabPage2Gray = new System.Windows.Forms.TabPage();
            this.RbStartGrayZ = new System.Windows.Forms.RadioButton();
            this.LblStartGrayMode = new System.Windows.Forms.Label();
            this.GbStartGrayS = new System.Windows.Forms.GroupBox();
            this.cBLaserModeOffEnd = new System.Windows.Forms.CheckBox();
            this.cBLaserModeOnStart = new System.Windows.Forms.CheckBox();
            this.nUDSTop = new System.Windows.Forms.NumericUpDown();
            this.tabPageSize = new System.Windows.Forms.TabPage();
            this.GbGrayscaleProcess = new System.Windows.Forms.GroupBox();
            this.RbGrayscaleVector = new System.Windows.Forms.RadioButton();
            this.RbGrayscalePattern = new System.Windows.Forms.RadioButton();
            this.gBgcodeSelection = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cBGCodeFill = new System.Windows.Forms.CheckBox();
            this.cBGCodeOutline = new System.Windows.Forms.CheckBox();
            this.cBGCodeOutlineSmooth = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.GbToolEnable = new System.Windows.Forms.GroupBox();
            this.tBToolList = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.CheckedListBoxTools = new System.Windows.Forms.CheckedListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.LblExceptionValue = new System.Windows.Forms.Label();
            this.cBReduceColorsToolTable = new System.Windows.Forms.CheckBox();
            this.cBReduceColorsDithering = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.btnPresetCorrection4 = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.btnPresetCorrection2 = new System.Windows.Forms.Button();
            this.btnResetCorrection = new System.Windows.Forms.Button();
            this.btnPresetCorrection1 = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.GbSpecialEffects = new System.Windows.Forms.GroupBox();
            this.cBFilterEdge = new System.Windows.Forms.CheckBox();
            this.cBFilterRemoveArtefact = new System.Windows.Forms.CheckBox();
            this.cBPosterize = new System.Windows.Forms.CheckBox();
            this.cBFilterHistogram = new System.Windows.Forms.CheckBox();
            this.GbColorEffects = new System.Windows.Forms.GroupBox();
            this.lblCFB = new System.Windows.Forms.Label();
            this.lblCFG = new System.Windows.Forms.Label();
            this.lblCFR = new System.Windows.Forms.Label();
            this.GbCOlorCorrection = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lblSaturation = new System.Windows.Forms.Label();
            this.tBarSaturation = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBWThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).BeginInit();
            this.GbGcodeDirection.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDSpiralCenterY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDSpiralCenterX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarHueShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMaxColors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDColorPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGCodeOutlineSmooth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZBottom)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.GbStartGrayZ.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZTop)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.GbUseCaseLoad.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage2Color.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2Gray.SuspendLayout();
            this.GbStartGrayS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSTop)).BeginInit();
            this.tabPageSize.SuspendLayout();
            this.GbGrayscaleProcess.SuspendLayout();
            this.gBgcodeSelection.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.GbToolEnable.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.GbSpecialEffects.SuspendLayout();
            this.GbColorEffects.SuspendLayout();
            this.GbCOlorCorrection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarSaturation)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LblMode,
            this.lblStatus});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // LblMode
            // 
            this.LblMode.Name = "LblMode";
            resources.ApplyResources(this.LblMode, "LblMode");
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            resources.ApplyResources(this.lblStatus, "lblStatus");
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox3.Controls.Add(this.lblInfo1);
            this.groupBox3.Controls.Add(this.btnKeepSizeReso);
            this.groupBox3.Controls.Add(this.btnKeepSizeWidth);
            this.groupBox3.Controls.Add(this.lblSizeOrig);
            this.groupBox3.Controls.Add(this.nUDResoX);
            this.groupBox3.Controls.Add(this.lblSizeResult);
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
            // lblInfo1
            // 
            resources.ApplyResources(this.lblInfo1, "lblInfo1");
            this.lblInfo1.Name = "lblInfo1";
            // 
            // btnKeepSizeReso
            // 
            resources.ApplyResources(this.btnKeepSizeReso, "btnKeepSizeReso");
            this.btnKeepSizeReso.Name = "btnKeepSizeReso";
            this.btnKeepSizeReso.UseVisualStyleBackColor = true;
            this.btnKeepSizeReso.Click += new System.EventHandler(this.BtnKeepSizeReso_Click);
            // 
            // btnKeepSizeWidth
            // 
            resources.ApplyResources(this.btnKeepSizeWidth, "btnKeepSizeWidth");
            this.btnKeepSizeWidth.Name = "btnKeepSizeWidth";
            this.btnKeepSizeWidth.UseVisualStyleBackColor = true;
            this.btnKeepSizeWidth.Click += new System.EventHandler(this.BtnKeepSizeWidth_Click);
            // 
            // lblSizeOrig
            // 
            resources.ApplyResources(this.lblSizeOrig, "lblSizeOrig");
            this.lblSizeOrig.BackColor = System.Drawing.Color.White;
            this.lblSizeOrig.Name = "lblSizeOrig";
            // 
            // nUDResoX
            // 
            this.nUDResoX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageReso", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDResoX.DecimalPlaces = 2;
            this.nUDResoX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDResoX, "nUDResoX");
            this.nUDResoX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDResoX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nUDResoX.Name = "nUDResoX";
            this.toolTip1.SetToolTip(this.nUDResoX, resources.GetString("nUDResoX.ToolTip"));
            this.nUDResoX.Value = global::GrblPlotter.Properties.Settings.Default.importImageReso;
            this.nUDResoX.ValueChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // lblSizeResult
            // 
            resources.ApplyResources(this.lblSizeResult, "lblSizeResult");
            this.lblSizeResult.BackColor = System.Drawing.Color.White;
            this.lblSizeResult.Name = "lblSizeResult";
            // 
            // nUDHeight
            // 
            this.nUDHeight.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDHeight.DecimalPlaces = 1;
            this.nUDHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDHeight, "nUDHeight");
            this.nUDHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDHeight.Name = "nUDHeight";
            this.nUDHeight.Value = global::GrblPlotter.Properties.Settings.Default.importImageHeight;
            this.nUDHeight.ValueChanged += new System.EventHandler(this.NudWidthHeight_ValueChanged);
            // 
            // nUDWidth
            // 
            this.nUDWidth.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageWidth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDWidth.DecimalPlaces = 1;
            this.nUDWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDWidth, "nUDWidth");
            this.nUDWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDWidth.Name = "nUDWidth";
            this.nUDWidth.Value = global::GrblPlotter.Properties.Settings.Default.importImageWidth;
            this.nUDWidth.ValueChanged += new System.EventHandler(this.NudWidthHeight_ValueChanged);
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
            this.cbLockRatio.Checked = global::GrblPlotter.Properties.Settings.Default.importImageKeepRatio;
            this.cbLockRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLockRatio.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageKeepRatio", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbLockRatio.Name = "cbLockRatio";
            this.cbLockRatio.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
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
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.groupBox10);
            this.groupBox1.Controls.Add(this.cbGrayscaleChannel);
            this.groupBox1.Controls.Add(this.cbGrayscale);
            this.groupBox1.Controls.Add(this.rbModeDither);
            this.groupBox1.Controls.Add(this.rbModeGray);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.RbChannelK);
            this.groupBox10.Controls.Add(this.RbChannelY);
            this.groupBox10.Controls.Add(this.RbChannelM);
            this.groupBox10.Controls.Add(this.RbChannelC);
            this.groupBox10.Controls.Add(this.RbChannelB);
            this.groupBox10.Controls.Add(this.RbChannelG);
            this.groupBox10.Controls.Add(this.RbChannelR);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // RbChannelK
            // 
            resources.ApplyResources(this.RbChannelK, "RbChannelK");
            this.RbChannelK.Name = "RbChannelK";
            this.RbChannelK.UseVisualStyleBackColor = true;
            this.RbChannelK.CheckedChanged += new System.EventHandler(this.Channel_changed);
            // 
            // RbChannelY
            // 
            resources.ApplyResources(this.RbChannelY, "RbChannelY");
            this.RbChannelY.Name = "RbChannelY";
            this.RbChannelY.UseVisualStyleBackColor = true;
            this.RbChannelY.CheckedChanged += new System.EventHandler(this.Channel_changed);
            // 
            // RbChannelM
            // 
            resources.ApplyResources(this.RbChannelM, "RbChannelM");
            this.RbChannelM.Name = "RbChannelM";
            this.RbChannelM.UseVisualStyleBackColor = true;
            this.RbChannelM.CheckedChanged += new System.EventHandler(this.Channel_changed);
            // 
            // RbChannelC
            // 
            resources.ApplyResources(this.RbChannelC, "RbChannelC");
            this.RbChannelC.Checked = true;
            this.RbChannelC.Name = "RbChannelC";
            this.RbChannelC.TabStop = true;
            this.RbChannelC.UseVisualStyleBackColor = true;
            this.RbChannelC.CheckedChanged += new System.EventHandler(this.Channel_changed);
            // 
            // RbChannelB
            // 
            resources.ApplyResources(this.RbChannelB, "RbChannelB");
            this.RbChannelB.Name = "RbChannelB";
            this.RbChannelB.UseVisualStyleBackColor = true;
            this.RbChannelB.CheckedChanged += new System.EventHandler(this.Channel_changed);
            // 
            // RbChannelG
            // 
            resources.ApplyResources(this.RbChannelG, "RbChannelG");
            this.RbChannelG.Name = "RbChannelG";
            this.RbChannelG.UseVisualStyleBackColor = true;
            this.RbChannelG.CheckedChanged += new System.EventHandler(this.Channel_changed);
            // 
            // RbChannelR
            // 
            resources.ApplyResources(this.RbChannelR, "RbChannelR");
            this.RbChannelR.Name = "RbChannelR";
            this.RbChannelR.UseVisualStyleBackColor = true;
            this.RbChannelR.CheckedChanged += new System.EventHandler(this.Channel_changed);
            // 
            // cbGrayscaleChannel
            // 
            resources.ApplyResources(this.cbGrayscaleChannel, "cbGrayscaleChannel");
            this.cbGrayscaleChannel.Name = "cbGrayscaleChannel";
            this.cbGrayscaleChannel.UseVisualStyleBackColor = true;
            this.cbGrayscaleChannel.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // cbGrayscale
            // 
            resources.ApplyResources(this.cbGrayscale, "cbGrayscale");
            this.cbGrayscale.Name = "cbGrayscale";
            this.cbGrayscale.UseVisualStyleBackColor = true;
            this.cbGrayscale.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // rbModeDither
            // 
            resources.ApplyResources(this.rbModeDither, "rbModeDither");
            this.rbModeDither.Name = "rbModeDither";
            this.rbModeDither.UseVisualStyleBackColor = true;
            this.rbModeDither.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // rbModeGray
            // 
            resources.ApplyResources(this.rbModeGray, "rbModeGray");
            this.rbModeGray.Checked = true;
            this.rbModeGray.Name = "rbModeGray";
            this.rbModeGray.TabStop = true;
            this.rbModeGray.UseVisualStyleBackColor = true;
            this.rbModeGray.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox8.Controls.Add(this.LblThreshold);
            this.groupBox8.Controls.Add(this.label7);
            this.groupBox8.Controls.Add(this.tBarBWThreshold);
            this.groupBox8.Controls.Add(this.CbBlackWhiteEnable);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // LblThreshold
            // 
            resources.ApplyResources(this.LblThreshold, "LblThreshold");
            this.LblThreshold.Name = "LblThreshold";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // tBarBWThreshold
            // 
            resources.ApplyResources(this.tBarBWThreshold, "tBarBWThreshold");
            this.tBarBWThreshold.LargeChange = 10;
            this.tBarBWThreshold.Maximum = 255;
            this.tBarBWThreshold.Name = "tBarBWThreshold";
            this.tBarBWThreshold.TickFrequency = 32;
            this.tBarBWThreshold.Value = 127;
            this.tBarBWThreshold.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBarBWThreshold.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBarBWThreshold.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // CbBlackWhiteEnable
            // 
            resources.ApplyResources(this.CbBlackWhiteEnable, "CbBlackWhiteEnable");
            this.CbBlackWhiteEnable.Name = "CbBlackWhiteEnable";
            this.CbBlackWhiteEnable.UseVisualStyleBackColor = true;
            this.CbBlackWhiteEnable.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // btnInvert
            // 
            this.btnInvert.BackgroundImage = global::GrblPlotter.Properties.Resources.inv2;
            resources.ApplyResources(this.btnInvert, "btnInvert");
            this.btnInvert.Name = "btnInvert";
            this.toolTip1.SetToolTip(this.btnInvert, resources.GetString("btnInvert.ToolTip"));
            this.btnInvert.UseVisualStyleBackColor = true;
            this.btnInvert.Click += new System.EventHandler(this.BtnInvert_Click);
            // 
            // btnHorizMirror
            // 
            this.btnHorizMirror.BackgroundImage = global::GrblPlotter.Properties.Resources.flip_horizontal;
            resources.ApplyResources(this.btnHorizMirror, "btnHorizMirror");
            this.btnHorizMirror.Name = "btnHorizMirror";
            this.toolTip1.SetToolTip(this.btnHorizMirror, resources.GetString("btnHorizMirror.ToolTip"));
            this.btnHorizMirror.UseVisualStyleBackColor = true;
            this.btnHorizMirror.Click += new System.EventHandler(this.BtnHorizMirror_Click);
            // 
            // btnVertMirror
            // 
            this.btnVertMirror.BackgroundImage = global::GrblPlotter.Properties.Resources.flip_vertical;
            resources.ApplyResources(this.btnVertMirror, "btnVertMirror");
            this.btnVertMirror.Name = "btnVertMirror";
            this.toolTip1.SetToolTip(this.btnVertMirror, resources.GetString("btnVertMirror.ToolTip"));
            this.btnVertMirror.UseVisualStyleBackColor = true;
            this.btnVertMirror.Click += new System.EventHandler(this.BtnVertMirror_Click);
            // 
            // btnRotateRight
            // 
            this.btnRotateRight.BackgroundImage = global::GrblPlotter.Properties.Resources.turn_r;
            resources.ApplyResources(this.btnRotateRight, "btnRotateRight");
            this.btnRotateRight.Name = "btnRotateRight";
            this.toolTip1.SetToolTip(this.btnRotateRight, resources.GetString("btnRotateRight.ToolTip"));
            this.btnRotateRight.UseVisualStyleBackColor = true;
            this.btnRotateRight.Click += new System.EventHandler(this.BtnRotateRight_Click);
            // 
            // btnRotateLeft
            // 
            this.btnRotateLeft.BackgroundImage = global::GrblPlotter.Properties.Resources.turn_l;
            resources.ApplyResources(this.btnRotateLeft, "btnRotateLeft");
            this.btnRotateLeft.Name = "btnRotateLeft";
            this.toolTip1.SetToolTip(this.btnRotateLeft, resources.GetString("btnRotateLeft.ToolTip"));
            this.btnRotateLeft.UseVisualStyleBackColor = true;
            this.btnRotateLeft.Click += new System.EventHandler(this.BtnRotateLeft_Click);
            // 
            // tBarGamma
            // 
            resources.ApplyResources(this.tBarGamma, "tBarGamma");
            this.tBarGamma.LargeChange = 20;
            this.tBarGamma.Maximum = 500;
            this.tBarGamma.Minimum = 1;
            this.tBarGamma.Name = "tBarGamma";
            this.tBarGamma.TickFrequency = 10;
            this.tBarGamma.Value = 1;
            this.tBarGamma.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBarGamma.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBarGamma.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // tBarContrast
            // 
            resources.ApplyResources(this.tBarContrast, "tBarContrast");
            this.tBarContrast.LargeChange = 20;
            this.tBarContrast.Maximum = 127;
            this.tBarContrast.Minimum = -127;
            this.tBarContrast.Name = "tBarContrast";
            this.tBarContrast.TickFrequency = 32;
            this.tBarContrast.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBarContrast.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBarContrast.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // tBarBrightness
            // 
            resources.ApplyResources(this.tBarBrightness, "tBarBrightness");
            this.tBarBrightness.LargeChange = 20;
            this.tBarBrightness.Maximum = 127;
            this.tBarBrightness.Minimum = -127;
            this.tBarBrightness.Name = "tBarBrightness";
            this.tBarBrightness.TickFrequency = 32;
            this.tBarBrightness.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBarBrightness.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBarBrightness.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // GbGcodeDirection
            // 
            this.GbGcodeDirection.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbGcodeDirection.Controls.Add(this.label27);
            this.GbGcodeDirection.Controls.Add(this.CBoxPatternFiles);
            this.GbGcodeDirection.Controls.Add(this.BtnReloadPattern);
            this.GbGcodeDirection.Controls.Add(this.label26);
            this.GbGcodeDirection.Controls.Add(this.groupBox7);
            this.GbGcodeDirection.Controls.Add(this.rbEngravingPattern4);
            this.GbGcodeDirection.Controls.Add(this.rbEngravingPattern3);
            this.GbGcodeDirection.Controls.Add(this.label25);
            this.GbGcodeDirection.Controls.Add(this.cBOnlyLeftToRight);
            this.GbGcodeDirection.Controls.Add(this.NuDSpiralCenterY);
            this.GbGcodeDirection.Controls.Add(this.rbEngravingPattern2);
            this.GbGcodeDirection.Controls.Add(this.rbEngravingPattern1);
            this.GbGcodeDirection.Controls.Add(this.NuDSpiralCenterX);
            resources.ApplyResources(this.GbGcodeDirection, "GbGcodeDirection");
            this.GbGcodeDirection.Name = "GbGcodeDirection";
            this.GbGcodeDirection.TabStop = false;
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // CBoxPatternFiles
            // 
            this.CBoxPatternFiles.FormattingEnabled = true;
            resources.ApplyResources(this.CBoxPatternFiles, "CBoxPatternFiles");
            this.CBoxPatternFiles.Name = "CBoxPatternFiles";
            this.CBoxPatternFiles.SelectedIndexChanged += new System.EventHandler(this.CBoxPatternFiles_SelectedIndexChanged);
            // 
            // BtnReloadPattern
            // 
            resources.ApplyResources(this.BtnReloadPattern, "BtnReloadPattern");
            this.BtnReloadPattern.Name = "BtnReloadPattern";
            this.BtnReloadPattern.UseVisualStyleBackColor = true;
            this.BtnReloadPattern.Click += new System.EventHandler(this.BtnReloadPattern_Click);
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.CbPenUpOn0);
            this.groupBox7.Controls.Add(this.nUDResoY);
            this.groupBox7.Controls.Add(this.cBCompress);
            this.groupBox7.Controls.Add(this.label18);
            this.groupBox7.Controls.Add(this.Cb2DViewHide0);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // CbPenUpOn0
            // 
            resources.ApplyResources(this.CbPenUpOn0, "CbPenUpOn0");
            this.CbPenUpOn0.Checked = true;
            this.CbPenUpOn0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbPenUpOn0.Name = "CbPenUpOn0";
            this.CbPenUpOn0.UseVisualStyleBackColor = true;
            // 
            // nUDResoY
            // 
            this.nUDResoY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageResoY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDResoY.DecimalPlaces = 2;
            this.nUDResoY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDResoY, "nUDResoY");
            this.nUDResoY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDResoY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nUDResoY.Name = "nUDResoY";
            this.toolTip1.SetToolTip(this.nUDResoY, resources.GetString("nUDResoY.ToolTip"));
            this.nUDResoY.Value = global::GrblPlotter.Properties.Settings.Default.importImageResoY;
            this.nUDResoY.ValueChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
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
            // Cb2DViewHide0
            // 
            resources.ApplyResources(this.Cb2DViewHide0, "Cb2DViewHide0");
            this.Cb2DViewHide0.Checked = global::GrblPlotter.Properties.Settings.Default.importImage2DViewHideZero;
            this.Cb2DViewHide0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Cb2DViewHide0.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImage2DViewHideZero", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Cb2DViewHide0.Name = "Cb2DViewHide0";
            this.toolTip1.SetToolTip(this.Cb2DViewHide0, resources.GetString("Cb2DViewHide0.ToolTip"));
            this.Cb2DViewHide0.UseVisualStyleBackColor = true;
            // 
            // rbEngravingPattern4
            // 
            resources.ApplyResources(this.rbEngravingPattern4, "rbEngravingPattern4");
            this.rbEngravingPattern4.Name = "rbEngravingPattern4";
            this.rbEngravingPattern4.TabStop = true;
            this.toolTip1.SetToolTip(this.rbEngravingPattern4, resources.GetString("rbEngravingPattern4.ToolTip"));
            this.rbEngravingPattern4.UseVisualStyleBackColor = true;
            // 
            // rbEngravingPattern3
            // 
            resources.ApplyResources(this.rbEngravingPattern3, "rbEngravingPattern3");
            this.rbEngravingPattern3.Name = "rbEngravingPattern3";
            this.rbEngravingPattern3.TabStop = true;
            this.toolTip1.SetToolTip(this.rbEngravingPattern3, resources.GetString("rbEngravingPattern3.ToolTip"));
            this.rbEngravingPattern3.UseVisualStyleBackColor = true;
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // cBOnlyLeftToRight
            // 
            resources.ApplyResources(this.cBOnlyLeftToRight, "cBOnlyLeftToRight");
            this.cBOnlyLeftToRight.Checked = true;
            this.cBOnlyLeftToRight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBOnlyLeftToRight.Name = "cBOnlyLeftToRight";
            this.toolTip1.SetToolTip(this.cBOnlyLeftToRight, resources.GetString("cBOnlyLeftToRight.ToolTip"));
            this.cBOnlyLeftToRight.UseVisualStyleBackColor = true;
            // 
            // NuDSpiralCenterY
            // 
            this.NuDSpiralCenterY.DecimalPlaces = 2;
            this.NuDSpiralCenterY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NuDSpiralCenterY, "NuDSpiralCenterY");
            this.NuDSpiralCenterY.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NuDSpiralCenterY.Name = "NuDSpiralCenterY";
            this.NuDSpiralCenterY.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // rbEngravingPattern2
            // 
            resources.ApplyResources(this.rbEngravingPattern2, "rbEngravingPattern2");
            this.rbEngravingPattern2.Name = "rbEngravingPattern2";
            this.rbEngravingPattern2.UseVisualStyleBackColor = true;
            this.rbEngravingPattern2.CheckedChanged += new System.EventHandler(this.RbEngravingPattern2_CheckedChanged);
            // 
            // rbEngravingPattern1
            // 
            resources.ApplyResources(this.rbEngravingPattern1, "rbEngravingPattern1");
            this.rbEngravingPattern1.Checked = true;
            this.rbEngravingPattern1.Name = "rbEngravingPattern1";
            this.rbEngravingPattern1.TabStop = true;
            this.rbEngravingPattern1.UseVisualStyleBackColor = true;
            // 
            // NuDSpiralCenterX
            // 
            this.NuDSpiralCenterX.DecimalPlaces = 2;
            this.NuDSpiralCenterX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NuDSpiralCenterX, "NuDSpiralCenterX");
            this.NuDSpiralCenterX.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NuDSpiralCenterX.Name = "NuDSpiralCenterX";
            this.NuDSpiralCenterX.Value = new decimal(new int[] {
            50,
            0,
            0,
            131072});
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.btnGenerate, "btnGenerate");
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.BtnGenerateClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Image = global::GrblPlotter.Properties.Resources.modell;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragDrop);
            this.pictureBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragEnter);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.setAsOriginalToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // setAsOriginalToolStripMenuItem
            // 
            this.setAsOriginalToolStripMenuItem.Name = "setAsOriginalToolStripMenuItem";
            resources.ApplyResources(this.setAsOriginalToolStripMenuItem, "setAsOriginalToolStripMenuItem");
            this.setAsOriginalToolStripMenuItem.Click += new System.EventHandler(this.SetAsOriginalToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblImageSource);
            this.panel1.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragDrop);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.GCodeFromImage_DragEnter);
            // 
            // lblImageSource
            // 
            resources.ApplyResources(this.lblImageSource, "lblImageSource");
            this.lblImageSource.BackColor = System.Drawing.Color.White;
            this.lblImageSource.Name = "lblImageSource";
            // 
            // lblHueShift
            // 
            resources.ApplyResources(this.lblHueShift, "lblHueShift");
            this.lblHueShift.Name = "lblHueShift";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // tBarHueShift
            // 
            resources.ApplyResources(this.tBarHueShift, "tBarHueShift");
            this.tBarHueShift.LargeChange = 20;
            this.tBarHueShift.Maximum = 359;
            this.tBarHueShift.Name = "tBarHueShift";
            this.tBarHueShift.TickFrequency = 30;
            this.toolTip1.SetToolTip(this.tBarHueShift, resources.GetString("tBarHueShift.ToolTip"));
            this.tBarHueShift.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBarHueShift.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBarHueShift.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // nUDMaxColors
            // 
            resources.ApplyResources(this.nUDMaxColors, "nUDMaxColors");
            this.nUDMaxColors.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDMaxColors.Minimum = new decimal(new int[] {
            1,
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
            this.nUDMaxColors.ValueChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // lblColors
            // 
            resources.ApplyResources(this.lblColors, "lblColors");
            this.lblColors.BackColor = System.Drawing.Color.White;
            this.lblColors.Name = "lblColors";
            // 
            // cbExceptColor
            // 
            resources.ApplyResources(this.cbExceptColor, "cbExceptColor");
            this.cbExceptColor.BackColor = System.Drawing.Color.White;
            this.cbExceptColor.Checked = true;
            this.cbExceptColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbExceptColor.Name = "cbExceptColor";
            this.toolTip1.SetToolTip(this.cbExceptColor, resources.GetString("cbExceptColor.ToolTip"));
            this.cbExceptColor.UseVisualStyleBackColor = false;
            this.cbExceptColor.CheckedChanged += new System.EventHandler(this.CbExceptColor_CheckedChanged);
            this.cbExceptColor.BackColorChanged += new System.EventHandler(this.CbExceptColor_BackColorChanged);
            // 
            // cbSkipToolOrder
            // 
            resources.ApplyResources(this.cbSkipToolOrder, "cbSkipToolOrder");
            this.cbSkipToolOrder.Name = "cbSkipToolOrder";
            this.toolTip1.SetToolTip(this.cbSkipToolOrder, resources.GetString("cbSkipToolOrder.ToolTip"));
            this.cbSkipToolOrder.UseVisualStyleBackColor = true;
            this.cbSkipToolOrder.CheckedChanged += new System.EventHandler(this.JustShowResult);
            // 
            // tBRMin
            // 
            resources.ApplyResources(this.tBRMin, "tBRMin");
            this.tBRMin.Maximum = 255;
            this.tBRMin.Name = "tBRMin";
            this.tBRMin.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBRMin, resources.GetString("tBRMin.ToolTip"));
            this.tBRMin.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBRMin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBRMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // tBRMax
            // 
            resources.ApplyResources(this.tBRMax, "tBRMax");
            this.tBRMax.Maximum = 255;
            this.tBRMax.Name = "tBRMax";
            this.tBRMax.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBRMax, resources.GetString("tBRMax.ToolTip"));
            this.tBRMax.Value = 255;
            this.tBRMax.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBRMax.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBRMax.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // tBGMax
            // 
            resources.ApplyResources(this.tBGMax, "tBGMax");
            this.tBGMax.Maximum = 255;
            this.tBGMax.Name = "tBGMax";
            this.tBGMax.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBGMax, resources.GetString("tBGMax.ToolTip"));
            this.tBGMax.Value = 255;
            this.tBGMax.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBGMax.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBGMax.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // tBGMin
            // 
            resources.ApplyResources(this.tBGMin, "tBGMin");
            this.tBGMin.Maximum = 255;
            this.tBGMin.Name = "tBGMin";
            this.tBGMin.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBGMin, resources.GetString("tBGMin.ToolTip"));
            this.tBGMin.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBGMin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBGMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // tBBMax
            // 
            resources.ApplyResources(this.tBBMax, "tBBMax");
            this.tBBMax.Maximum = 255;
            this.tBBMax.Name = "tBBMax";
            this.tBBMax.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBBMax, resources.GetString("tBBMax.ToolTip"));
            this.tBBMax.Value = 255;
            this.tBBMax.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBBMax.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBBMax.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // tBBMin
            // 
            resources.ApplyResources(this.tBBMin, "tBBMin");
            this.tBBMin.Maximum = 255;
            this.tBBMin.Name = "tBBMin";
            this.tBBMin.TickFrequency = 32;
            this.toolTip1.SetToolTip(this.tBBMin, resources.GetString("tBBMin.ToolTip"));
            this.tBBMin.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBBMin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBBMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
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
            this.rBMode0.CheckedChanged += new System.EventHandler(this.RbMode0_CheckedChanged);
            // 
            // rBMode1
            // 
            resources.ApplyResources(this.rBMode1, "rBMode1");
            this.rBMode1.Name = "rBMode1";
            this.toolTip1.SetToolTip(this.rBMode1, resources.GetString("rBMode1.ToolTip"));
            this.rBMode1.UseVisualStyleBackColor = true;
            this.rBMode1.CheckedChanged += new System.EventHandler(this.RbMode0_CheckedChanged);
            // 
            // rBMode2
            // 
            resources.ApplyResources(this.rBMode2, "rBMode2");
            this.rBMode2.Name = "rBMode2";
            this.toolTip1.SetToolTip(this.rBMode2, resources.GetString("rBMode2.ToolTip"));
            this.rBMode2.UseVisualStyleBackColor = true;
            this.rBMode2.CheckedChanged += new System.EventHandler(this.RbMode0_CheckedChanged);
            // 
            // btnTest
            // 
            resources.ApplyResources(this.btnTest, "btnTest");
            this.btnTest.Name = "btnTest";
            this.toolTip1.SetToolTip(this.btnTest, resources.GetString("btnTest.ToolTip"));
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCheckOrig_MouseDown);
            this.btnTest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnCheckOrig_MouseUp);
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
            this.btnPresetCorrection3.Click += new System.EventHandler(this.BtnPresetCorrection3_Click);
            // 
            // btnShowSettings
            // 
            resources.ApplyResources(this.btnShowSettings, "btnShowSettings");
            this.btnShowSettings.Name = "btnShowSettings";
            this.toolTip1.SetToolTip(this.btnShowSettings, resources.GetString("btnShowSettings.ToolTip"));
            this.btnShowSettings.UseVisualStyleBackColor = true;
            this.btnShowSettings.Click += new System.EventHandler(this.BtnShowSettings_Click);
            // 
            // btnShowOrig
            // 
            resources.ApplyResources(this.btnShowOrig, "btnShowOrig");
            this.btnShowOrig.Name = "btnShowOrig";
            this.toolTip1.SetToolTip(this.btnShowOrig, resources.GetString("btnShowOrig.ToolTip"));
            this.btnShowOrig.UseVisualStyleBackColor = true;
            this.btnShowOrig.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnShowOrig_MouseDown);
            this.btnShowOrig.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnShowOrig_MouseUp);
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
            this.nUDGCodeOutlineSmooth.ValueChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // cBGCodeOutlineShrink
            // 
            resources.ApplyResources(this.cBGCodeOutlineShrink, "cBGCodeOutlineShrink");
            this.cBGCodeOutlineShrink.Checked = true;
            this.cBGCodeOutlineShrink.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeOutlineShrink.Name = "cBGCodeOutlineShrink";
            this.toolTip1.SetToolTip(this.cBGCodeOutlineShrink, resources.GetString("cBGCodeOutlineShrink.ToolTip"));
            this.cBGCodeOutlineShrink.UseVisualStyleBackColor = true;
            this.cBGCodeOutlineShrink.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // btnGetPWMValues
            // 
            resources.ApplyResources(this.btnGetPWMValues, "btnGetPWMValues");
            this.btnGetPWMValues.Name = "btnGetPWMValues";
            this.toolTip1.SetToolTip(this.btnGetPWMValues, resources.GetString("btnGetPWMValues.ToolTip"));
            this.btnGetPWMValues.UseVisualStyleBackColor = true;
            this.btnGetPWMValues.Click += new System.EventHandler(this.BtnGetPWMValues_Click);
            // 
            // cBToolChange
            // 
            resources.ApplyResources(this.cBToolChange, "cBToolChange");
            this.cBToolChange.Checked = global::GrblPlotter.Properties.Settings.Default.ctrlToolChange;
            this.cBToolChange.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "ctrlToolChange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolChange.Name = "cBToolChange";
            this.toolTip1.SetToolTip(this.cBToolChange, resources.GetString("cBToolChange.ToolTip"));
            this.cBToolChange.UseVisualStyleBackColor = true;
            // 
            // cBImportGCToolM0
            // 
            resources.ApplyResources(this.cBImportGCToolM0, "cBImportGCToolM0");
            this.cBImportGCToolM0.Checked = global::GrblPlotter.Properties.Settings.Default.importGCToolM0;
            this.cBImportGCToolM0.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGCToolM0", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCToolM0.Name = "cBImportGCToolM0";
            this.toolTip1.SetToolTip(this.cBImportGCToolM0, resources.GetString("cBImportGCToolM0.ToolTip"));
            this.cBImportGCToolM0.UseVisualStyleBackColor = true;
            // 
            // cBImportGCTool
            // 
            resources.ApplyResources(this.cBImportGCTool, "cBImportGCTool");
            this.cBImportGCTool.Checked = global::GrblPlotter.Properties.Settings.Default.importGCTool;
            this.cBImportGCTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGCTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCTool.Name = "cBImportGCTool";
            this.toolTip1.SetToolTip(this.cBImportGCTool, resources.GetString("cBImportGCTool.ToolTip"));
            this.cBImportGCTool.UseVisualStyleBackColor = true;
            // 
            // nUDSBottom
            // 
            this.nUDSBottom.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageSMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDSBottom, "nUDSBottom");
            this.nUDSBottom.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDSBottom.Name = "nUDSBottom";
            this.toolTip1.SetToolTip(this.nUDSBottom, resources.GetString("nUDSBottom.ToolTip"));
            this.nUDSBottom.Value = global::GrblPlotter.Properties.Settings.Default.importImageSMax;
            // 
            // nUDZBottom
            // 
            this.nUDZBottom.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageZMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDZBottom.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDZBottom, "nUDZBottom");
            this.nUDZBottom.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nUDZBottom.Name = "nUDZBottom";
            this.toolTip1.SetToolTip(this.nUDZBottom, resources.GetString("nUDZBottom.ToolTip"));
            this.nUDZBottom.Value = global::GrblPlotter.Properties.Settings.Default.importImageZMin;
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox6.Controls.Add(this.rBMode2);
            this.groupBox6.Controls.Add(this.rBMode1);
            this.groupBox6.Controls.Add(this.rBMode0);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Yellow;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPictureToolStripMenuItem,
            this.pasteFromClipboardToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // loadPictureToolStripMenuItem
            // 
            this.loadPictureToolStripMenuItem.Name = "loadPictureToolStripMenuItem";
            resources.ApplyResources(this.loadPictureToolStripMenuItem, "loadPictureToolStripMenuItem");
            this.loadPictureToolStripMenuItem.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // pasteFromClipboardToolStripMenuItem
            // 
            this.pasteFromClipboardToolStripMenuItem.Name = "pasteFromClipboardToolStripMenuItem";
            resources.ApplyResources(this.pasteFromClipboardToolStripMenuItem, "pasteFromClipboardToolStripMenuItem");
            this.pasteFromClipboardToolStripMenuItem.Click += new System.EventHandler(this.PasteFromClipboardToolStripMenuItem_Click);
            // 
            // GbStartGrayZ
            // 
            this.GbStartGrayZ.BackColor = System.Drawing.Color.Yellow;
            this.GbStartGrayZ.Controls.Add(this.label8);
            this.GbStartGrayZ.Controls.Add(this.nUDZTop);
            this.GbStartGrayZ.Controls.Add(this.label9);
            this.GbStartGrayZ.Controls.Add(this.nUDZBottom);
            resources.ApplyResources(this.GbStartGrayZ, "GbStartGrayZ");
            this.GbStartGrayZ.Name = "GbStartGrayZ";
            this.GbStartGrayZ.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // nUDZTop
            // 
            this.nUDZTop.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageZMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDZTop.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDZTop, "nUDZTop");
            this.nUDZTop.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nUDZTop.Name = "nUDZTop";
            this.nUDZTop.Value = global::GrblPlotter.Properties.Settings.Default.importImageZMax;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // RbStartGrayS
            // 
            resources.ApplyResources(this.RbStartGrayS, "RbStartGrayS");
            this.RbStartGrayS.Name = "RbStartGrayS";
            this.RbStartGrayS.UseVisualStyleBackColor = true;
            this.RbStartGrayS.CheckedChanged += new System.EventHandler(this.RbGrayZ_CheckedChanged);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // cBPreview
            // 
            resources.ApplyResources(this.cBPreview, "cBPreview");
            this.cBPreview.Name = "cBPreview";
            this.cBPreview.UseVisualStyleBackColor = true;
            this.cBPreview.CheckedChanged += new System.EventHandler(this.JustShowResult);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSetup);
            this.tabControl1.Controls.Add(this.tabPageSize);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl1_DrawItem);
            // 
            // tabPageSetup
            // 
            this.tabPageSetup.Controls.Add(this.groupBox4);
            this.tabPageSetup.Controls.Add(this.GbUseCaseLoad);
            this.tabPageSetup.Controls.Add(this.tabControl2);
            resources.ApplyResources(this.tabPageSetup, "tabPageSetup");
            this.tabPageSetup.Name = "tabPageSetup";
            this.tabPageSetup.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox4.Controls.Add(this.cBResolutionPenWidth);
            this.groupBox4.Controls.Add(this.checkBox1);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // cBResolutionPenWidth
            // 
            this.cBResolutionPenWidth.Checked = global::GrblPlotter.Properties.Settings.Default.importImageResoApply;
            this.cBResolutionPenWidth.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageResoApply", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.cBResolutionPenWidth, "cBResolutionPenWidth");
            this.cBResolutionPenWidth.Name = "cBResolutionPenWidth";
            this.cBResolutionPenWidth.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = global::GrblPlotter.Properties.Settings.Default.gui2DColorPenDownModeEnable;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "gui2DColorPenDownModeEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // GbUseCaseLoad
            // 
            this.GbUseCaseLoad.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbUseCaseLoad.Controls.Add(this.lBUseCase);
            this.GbUseCaseLoad.Controls.Add(this.btnLoad);
            this.GbUseCaseLoad.Controls.Add(this.label16);
            this.GbUseCaseLoad.Controls.Add(this.lblLastUseCase);
            resources.ApplyResources(this.GbUseCaseLoad, "GbUseCaseLoad");
            this.GbUseCaseLoad.Name = "GbUseCaseLoad";
            this.GbUseCaseLoad.TabStop = false;
            // 
            // lBUseCase
            // 
            this.lBUseCase.FormattingEnabled = true;
            resources.ApplyResources(this.lBUseCase, "lBUseCase");
            this.lBUseCase.Name = "lBUseCase";
            // 
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoadUseCase_Click);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // lblLastUseCase
            // 
            resources.ApplyResources(this.lblLastUseCase, "lblLastUseCase");
            this.lblLastUseCase.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "useCaseLastLoaded", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblLastUseCase.Name = "lblLastUseCase";
            this.lblLastUseCase.Text = global::GrblPlotter.Properties.Settings.Default.useCaseLastLoaded;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage2Color);
            this.tabControl2.Controls.Add(this.tabPage2Gray);
            this.tabControl2.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            resources.ApplyResources(this.tabControl2, "tabControl2");
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl2_DrawItem);
            this.tabControl2.SelectedIndexChanged += new System.EventHandler(this.TabControl2_SelectedIndexChanged);
            // 
            // tabPage2Color
            // 
            this.tabPage2Color.Controls.Add(this.groupBox2);
            this.tabPage2Color.Controls.Add(this.label19);
            this.tabPage2Color.Controls.Add(this.CboxToolTable);
            this.tabPage2Color.Controls.Add(this.CboxToolFiles);
            this.tabPage2Color.Controls.Add(this.label28);
            resources.ApplyResources(this.tabPage2Color, "tabPage2Color");
            this.tabPage2Color.Name = "tabPage2Color";
            this.tabPage2Color.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cBToolChange);
            this.groupBox2.Controls.Add(this.cBImportGCToolM0);
            this.groupBox2.Controls.Add(this.cBImportGCTool);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // CboxToolTable
            // 
            this.CboxToolTable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            resources.ApplyResources(this.CboxToolTable, "CboxToolTable");
            this.CboxToolTable.FormattingEnabled = true;
            this.CboxToolTable.Name = "CboxToolTable";
            this.CboxToolTable.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.CboxToolTable_DrawItem);
            // 
            // CboxToolFiles
            // 
            this.CboxToolFiles.FormattingEnabled = true;
            resources.ApplyResources(this.CboxToolFiles, "CboxToolFiles");
            this.CboxToolFiles.Name = "CboxToolFiles";
            this.CboxToolFiles.SelectedIndexChanged += new System.EventHandler(this.CboxToolFiles_SelectedIndexChanged);
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // tabPage2Gray
            // 
            this.tabPage2Gray.Controls.Add(this.RbStartGrayZ);
            this.tabPage2Gray.Controls.Add(this.RbStartGrayS);
            this.tabPage2Gray.Controls.Add(this.LblStartGrayMode);
            this.tabPage2Gray.Controls.Add(this.GbStartGrayS);
            this.tabPage2Gray.Controls.Add(this.GbStartGrayZ);
            resources.ApplyResources(this.tabPage2Gray, "tabPage2Gray");
            this.tabPage2Gray.Name = "tabPage2Gray";
            this.tabPage2Gray.UseVisualStyleBackColor = true;
            // 
            // RbStartGrayZ
            // 
            this.RbStartGrayZ.BackColor = System.Drawing.Color.Yellow;
            this.RbStartGrayZ.Checked = global::GrblPlotter.Properties.Settings.Default.importImageGrayAsZ;
            this.RbStartGrayZ.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageGrayAsZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.RbStartGrayZ, "RbStartGrayZ");
            this.RbStartGrayZ.Name = "RbStartGrayZ";
            this.RbStartGrayZ.TabStop = true;
            this.RbStartGrayZ.UseVisualStyleBackColor = false;
            this.RbStartGrayZ.CheckedChanged += new System.EventHandler(this.RbGrayZ_CheckedChanged);
            // 
            // LblStartGrayMode
            // 
            this.LblStartGrayMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.LblStartGrayMode, "LblStartGrayMode");
            this.LblStartGrayMode.Name = "LblStartGrayMode";
            // 
            // GbStartGrayS
            // 
            this.GbStartGrayS.Controls.Add(this.cBLaserModeOffEnd);
            this.GbStartGrayS.Controls.Add(this.cBLaserModeOnStart);
            this.GbStartGrayS.Controls.Add(this.btnGetPWMValues);
            this.GbStartGrayS.Controls.Add(this.nUDSBottom);
            this.GbStartGrayS.Controls.Add(this.label23);
            this.GbStartGrayS.Controls.Add(this.nUDSTop);
            this.GbStartGrayS.Controls.Add(this.label22);
            resources.ApplyResources(this.GbStartGrayS, "GbStartGrayS");
            this.GbStartGrayS.Name = "GbStartGrayS";
            this.GbStartGrayS.TabStop = false;
            // 
            // cBLaserModeOffEnd
            // 
            resources.ApplyResources(this.cBLaserModeOffEnd, "cBLaserModeOffEnd");
            this.cBLaserModeOffEnd.Checked = global::GrblPlotter.Properties.Settings.Default.importImageSLasermodeEnd;
            this.cBLaserModeOffEnd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageSLasermodeEnd", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBLaserModeOffEnd.Name = "cBLaserModeOffEnd";
            this.cBLaserModeOffEnd.UseVisualStyleBackColor = true;
            // 
            // cBLaserModeOnStart
            // 
            resources.ApplyResources(this.cBLaserModeOnStart, "cBLaserModeOnStart");
            this.cBLaserModeOnStart.Checked = global::GrblPlotter.Properties.Settings.Default.importImageSLasermodeStart;
            this.cBLaserModeOnStart.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageSLasermodeStart", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBLaserModeOnStart.Name = "cBLaserModeOnStart";
            this.cBLaserModeOnStart.UseVisualStyleBackColor = true;
            // 
            // nUDSTop
            // 
            this.nUDSTop.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageSMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSTop.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDSTop, "nUDSTop");
            this.nUDSTop.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDSTop.Name = "nUDSTop";
            this.nUDSTop.Value = global::GrblPlotter.Properties.Settings.Default.importImageSMin;
            // 
            // tabPageSize
            // 
            this.tabPageSize.Controls.Add(this.GbGrayscaleProcess);
            this.tabPageSize.Controls.Add(this.gBgcodeSelection);
            this.tabPageSize.Controls.Add(this.GbGcodeDirection);
            this.tabPageSize.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.tabPageSize, "tabPageSize");
            this.tabPageSize.Name = "tabPageSize";
            this.tabPageSize.UseVisualStyleBackColor = true;
            // 
            // GbGrayscaleProcess
            // 
            this.GbGrayscaleProcess.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbGrayscaleProcess.Controls.Add(this.RbGrayscaleVector);
            this.GbGrayscaleProcess.Controls.Add(this.RbGrayscalePattern);
            resources.ApplyResources(this.GbGrayscaleProcess, "GbGrayscaleProcess");
            this.GbGrayscaleProcess.Name = "GbGrayscaleProcess";
            this.GbGrayscaleProcess.TabStop = false;
            // 
            // RbGrayscaleVector
            // 
            resources.ApplyResources(this.RbGrayscaleVector, "RbGrayscaleVector");
            this.RbGrayscaleVector.Name = "RbGrayscaleVector";
            this.RbGrayscaleVector.TabStop = true;
            this.RbGrayscaleVector.UseVisualStyleBackColor = true;
            this.RbGrayscaleVector.CheckedChanged += new System.EventHandler(this.RbGrayscaleVector_CheckedChanged);
            // 
            // RbGrayscalePattern
            // 
            resources.ApplyResources(this.RbGrayscalePattern, "RbGrayscalePattern");
            this.RbGrayscalePattern.Checked = true;
            this.RbGrayscalePattern.Name = "RbGrayscalePattern";
            this.RbGrayscalePattern.TabStop = true;
            this.RbGrayscalePattern.UseVisualStyleBackColor = true;
            this.RbGrayscalePattern.CheckedChanged += new System.EventHandler(this.RbGrayscaleVector_CheckedChanged);
            // 
            // gBgcodeSelection
            // 
            this.gBgcodeSelection.BackColor = System.Drawing.Color.WhiteSmoke;
            this.gBgcodeSelection.Controls.Add(this.label17);
            this.gBgcodeSelection.Controls.Add(this.cBGCodeFill);
            this.gBgcodeSelection.Controls.Add(this.cBGCodeOutlineShrink);
            this.gBgcodeSelection.Controls.Add(this.cBGCodeOutline);
            this.gBgcodeSelection.Controls.Add(this.nUDGCodeOutlineSmooth);
            this.gBgcodeSelection.Controls.Add(this.cBGCodeOutlineSmooth);
            resources.ApplyResources(this.gBgcodeSelection, "gBgcodeSelection");
            this.gBgcodeSelection.Name = "gBgcodeSelection";
            this.gBgcodeSelection.TabStop = false;
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // cBGCodeFill
            // 
            resources.ApplyResources(this.cBGCodeFill, "cBGCodeFill");
            this.cBGCodeFill.Checked = true;
            this.cBGCodeFill.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeFill.Name = "cBGCodeFill";
            this.cBGCodeFill.UseVisualStyleBackColor = true;
            this.cBGCodeFill.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // cBGCodeOutline
            // 
            resources.ApplyResources(this.cBGCodeOutline, "cBGCodeOutline");
            this.cBGCodeOutline.Checked = true;
            this.cBGCodeOutline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeOutline.Name = "cBGCodeOutline";
            this.cBGCodeOutline.UseVisualStyleBackColor = true;
            this.cBGCodeOutline.CheckedChanged += new System.EventHandler(this.CbGCodeOutline_CheckedChanged);
            // 
            // cBGCodeOutlineSmooth
            // 
            resources.ApplyResources(this.cBGCodeOutlineSmooth, "cBGCodeOutlineSmooth");
            this.cBGCodeOutlineSmooth.Checked = true;
            this.cBGCodeOutlineSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBGCodeOutlineSmooth.Name = "cBGCodeOutlineSmooth";
            this.cBGCodeOutlineSmooth.UseVisualStyleBackColor = true;
            this.cBGCodeOutlineSmooth.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.GbToolEnable);
            this.tabPage4.Controls.Add(this.groupBox6);
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Controls.Add(this.groupBox9);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // GbToolEnable
            // 
            this.GbToolEnable.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbToolEnable.Controls.Add(this.tBToolList);
            this.GbToolEnable.Controls.Add(this.cbSkipToolOrder);
            this.GbToolEnable.Controls.Add(this.label10);
            this.GbToolEnable.Controls.Add(this.CheckedListBoxTools);
            resources.ApplyResources(this.GbToolEnable, "GbToolEnable");
            this.GbToolEnable.Name = "GbToolEnable";
            this.GbToolEnable.TabStop = false;
            // 
            // tBToolList
            // 
            resources.ApplyResources(this.tBToolList, "tBToolList");
            this.tBToolList.Name = "tBToolList";
            this.tBToolList.ReadOnly = true;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // CheckedListBoxTools
            // 
            this.CheckedListBoxTools.CheckOnClick = true;
            resources.ApplyResources(this.CheckedListBoxTools, "CheckedListBoxTools");
            this.CheckedListBoxTools.FormattingEnabled = true;
            this.CheckedListBoxTools.Name = "CheckedListBoxTools";
            this.CheckedListBoxTools.SelectedIndexChanged += new System.EventHandler(this.CheckedListBoxTools_SelectedIndexChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox5.Controls.Add(this.LblExceptionValue);
            this.groupBox5.Controls.Add(this.cBReduceColorsToolTable);
            this.groupBox5.Controls.Add(this.nUDMaxColors);
            this.groupBox5.Controls.Add(this.cbExceptColor);
            this.groupBox5.Controls.Add(this.cBReduceColorsDithering);
            this.groupBox5.Controls.Add(this.lblColors);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // LblExceptionValue
            // 
            resources.ApplyResources(this.LblExceptionValue, "LblExceptionValue");
            this.LblExceptionValue.Name = "LblExceptionValue";
            // 
            // cBReduceColorsToolTable
            // 
            resources.ApplyResources(this.cBReduceColorsToolTable, "cBReduceColorsToolTable");
            this.cBReduceColorsToolTable.Name = "cBReduceColorsToolTable";
            this.cBReduceColorsToolTable.UseVisualStyleBackColor = true;
            this.cBReduceColorsToolTable.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // cBReduceColorsDithering
            // 
            resources.ApplyResources(this.cBReduceColorsDithering, "cBReduceColorsDithering");
            this.cBReduceColorsDithering.Name = "cBReduceColorsDithering";
            this.cBReduceColorsDithering.UseVisualStyleBackColor = true;
            this.cBReduceColorsDithering.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // groupBox9
            // 
            this.groupBox9.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox9.Controls.Add(this.btnPresetCorrection4);
            this.groupBox9.Controls.Add(this.label20);
            this.groupBox9.Controls.Add(this.nUDColorPercent);
            this.groupBox9.Controls.Add(this.btnPresetCorrection3);
            this.groupBox9.Controls.Add(this.btnPresetCorrection2);
            this.groupBox9.Controls.Add(this.btnResetCorrection);
            this.groupBox9.Controls.Add(this.btnPresetCorrection1);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // btnPresetCorrection4
            // 
            resources.ApplyResources(this.btnPresetCorrection4, "btnPresetCorrection4");
            this.btnPresetCorrection4.Name = "btnPresetCorrection4";
            this.btnPresetCorrection4.UseVisualStyleBackColor = true;
            this.btnPresetCorrection4.Click += new System.EventHandler(this.BtnPresetCorrection4_Click);
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // btnPresetCorrection2
            // 
            resources.ApplyResources(this.btnPresetCorrection2, "btnPresetCorrection2");
            this.btnPresetCorrection2.Name = "btnPresetCorrection2";
            this.btnPresetCorrection2.UseVisualStyleBackColor = true;
            this.btnPresetCorrection2.Click += new System.EventHandler(this.BtnPresetCorrection2_Click);
            // 
            // btnResetCorrection
            // 
            resources.ApplyResources(this.btnResetCorrection, "btnResetCorrection");
            this.btnResetCorrection.Name = "btnResetCorrection";
            this.btnResetCorrection.UseVisualStyleBackColor = true;
            this.btnResetCorrection.Click += new System.EventHandler(this.BtnResetCorrection_Click);
            // 
            // btnPresetCorrection1
            // 
            resources.ApplyResources(this.btnPresetCorrection1, "btnPresetCorrection1");
            this.btnPresetCorrection1.Name = "btnPresetCorrection1";
            this.btnPresetCorrection1.UseVisualStyleBackColor = true;
            this.btnPresetCorrection1.Click += new System.EventHandler(this.BtnPresetCorrection1_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.GbSpecialEffects);
            this.tabPage1.Controls.Add(this.GbColorEffects);
            this.tabPage1.Controls.Add(this.GbCOlorCorrection);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // GbSpecialEffects
            // 
            this.GbSpecialEffects.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbSpecialEffects.Controls.Add(this.cBFilterEdge);
            this.GbSpecialEffects.Controls.Add(this.cBFilterRemoveArtefact);
            this.GbSpecialEffects.Controls.Add(this.cBPosterize);
            this.GbSpecialEffects.Controls.Add(this.cBFilterHistogram);
            resources.ApplyResources(this.GbSpecialEffects, "GbSpecialEffects");
            this.GbSpecialEffects.Name = "GbSpecialEffects";
            this.GbSpecialEffects.TabStop = false;
            // 
            // cBFilterEdge
            // 
            resources.ApplyResources(this.cBFilterEdge, "cBFilterEdge");
            this.cBFilterEdge.Name = "cBFilterEdge";
            this.cBFilterEdge.UseVisualStyleBackColor = true;
            this.cBFilterEdge.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // cBFilterRemoveArtefact
            // 
            resources.ApplyResources(this.cBFilterRemoveArtefact, "cBFilterRemoveArtefact");
            this.cBFilterRemoveArtefact.Name = "cBFilterRemoveArtefact";
            this.cBFilterRemoveArtefact.UseVisualStyleBackColor = true;
            this.cBFilterRemoveArtefact.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // cBPosterize
            // 
            resources.ApplyResources(this.cBPosterize, "cBPosterize");
            this.cBPosterize.Name = "cBPosterize";
            this.cBPosterize.UseVisualStyleBackColor = true;
            this.cBPosterize.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // cBFilterHistogram
            // 
            resources.ApplyResources(this.cBFilterHistogram, "cBFilterHistogram");
            this.cBFilterHistogram.Name = "cBFilterHistogram";
            this.cBFilterHistogram.UseVisualStyleBackColor = true;
            this.cBFilterHistogram.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // GbColorEffects
            // 
            this.GbColorEffects.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbColorEffects.Controls.Add(this.label13);
            this.GbColorEffects.Controls.Add(this.label12);
            this.GbColorEffects.Controls.Add(this.tBarHueShift);
            this.GbColorEffects.Controls.Add(this.lblCFB);
            this.GbColorEffects.Controls.Add(this.lblHueShift);
            this.GbColorEffects.Controls.Add(this.lblCFG);
            this.GbColorEffects.Controls.Add(this.tBRMin);
            this.GbColorEffects.Controls.Add(this.lblCFR);
            this.GbColorEffects.Controls.Add(this.tBRMax);
            this.GbColorEffects.Controls.Add(this.label15);
            this.GbColorEffects.Controls.Add(this.tBGMin);
            this.GbColorEffects.Controls.Add(this.label14);
            this.GbColorEffects.Controls.Add(this.tBGMax);
            this.GbColorEffects.Controls.Add(this.tBBMin);
            this.GbColorEffects.Controls.Add(this.tBBMax);
            resources.ApplyResources(this.GbColorEffects, "GbColorEffects");
            this.GbColorEffects.Name = "GbColorEffects";
            this.GbColorEffects.TabStop = false;
            // 
            // lblCFB
            // 
            resources.ApplyResources(this.lblCFB, "lblCFB");
            this.lblCFB.Name = "lblCFB";
            // 
            // lblCFG
            // 
            resources.ApplyResources(this.lblCFG, "lblCFG");
            this.lblCFG.Name = "lblCFG";
            // 
            // lblCFR
            // 
            resources.ApplyResources(this.lblCFR, "lblCFR");
            this.lblCFR.Name = "lblCFR";
            // 
            // GbCOlorCorrection
            // 
            this.GbCOlorCorrection.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbCOlorCorrection.Controls.Add(this.label1);
            this.GbCOlorCorrection.Controls.Add(this.label11);
            this.GbCOlorCorrection.Controls.Add(this.btnRotateLeft);
            this.GbCOlorCorrection.Controls.Add(this.lblSaturation);
            this.GbCOlorCorrection.Controls.Add(this.btnRotateRight);
            this.GbCOlorCorrection.Controls.Add(this.tBarSaturation);
            this.GbCOlorCorrection.Controls.Add(this.lblGamma);
            this.GbCOlorCorrection.Controls.Add(this.label2);
            this.GbCOlorCorrection.Controls.Add(this.btnVertMirror);
            this.GbCOlorCorrection.Controls.Add(this.label3);
            this.GbCOlorCorrection.Controls.Add(this.tBarContrast);
            this.GbCOlorCorrection.Controls.Add(this.btnHorizMirror);
            this.GbCOlorCorrection.Controls.Add(this.lblBrightness);
            this.GbCOlorCorrection.Controls.Add(this.tBarBrightness);
            this.GbCOlorCorrection.Controls.Add(this.tBarGamma);
            this.GbCOlorCorrection.Controls.Add(this.btnInvert);
            this.GbCOlorCorrection.Controls.Add(this.lblContrast);
            resources.ApplyResources(this.GbCOlorCorrection, "GbCOlorCorrection");
            this.GbCOlorCorrection.Name = "GbCOlorCorrection";
            this.GbCOlorCorrection.TabStop = false;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // lblSaturation
            // 
            resources.ApplyResources(this.lblSaturation, "lblSaturation");
            this.lblSaturation.Name = "lblSaturation";
            // 
            // tBarSaturation
            // 
            resources.ApplyResources(this.tBarSaturation, "tBarSaturation");
            this.tBarSaturation.LargeChange = 20;
            this.tBarSaturation.Maximum = 255;
            this.tBarSaturation.Minimum = -255;
            this.tBarSaturation.Name = "tBarSaturation";
            this.tBarSaturation.TickFrequency = 32;
            this.tBarSaturation.Scroll += new System.EventHandler(this.ApplyColorCorrectionsEventScrollBar);
            this.tBarSaturation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseDown);
            this.tBarSaturation.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ScrollBar_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // GCodeFromImage
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWidth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBWThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).EndInit();
            this.GbGcodeDirection.ResumeLayout(false);
            this.GbGcodeDirection.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDSpiralCenterY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDSpiralCenterX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarHueShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMaxColors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBRMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBGMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBBMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDColorPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGCodeOutlineSmooth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZBottom)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.GbStartGrayZ.ResumeLayout(false);
            this.GbStartGrayZ.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZTop)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.GbUseCaseLoad.ResumeLayout(false);
            this.GbUseCaseLoad.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage2Color.ResumeLayout(false);
            this.tabPage2Color.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2Gray.ResumeLayout(false);
            this.GbStartGrayS.ResumeLayout(false);
            this.GbStartGrayS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSTop)).EndInit();
            this.tabPageSize.ResumeLayout(false);
            this.GbGrayscaleProcess.ResumeLayout(false);
            this.GbGrayscaleProcess.PerformLayout();
            this.gBgcodeSelection.ResumeLayout(false);
            this.gBgcodeSelection.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.GbToolEnable.ResumeLayout(false);
            this.GbToolEnable.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.GbSpecialEffects.ResumeLayout(false);
            this.GbSpecialEffects.PerformLayout();
            this.GbColorEffects.ResumeLayout(false);
            this.GbColorEffects.PerformLayout();
            this.GbCOlorCorrection.ResumeLayout(false);
            this.GbCOlorCorrection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarSaturation)).EndInit();
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
        private System.Windows.Forms.GroupBox GbGcodeDirection;
        private System.Windows.Forms.RadioButton rbEngravingPattern2;
        private System.Windows.Forms.RadioButton rbEngravingPattern1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.NumericUpDown nUDWidth;
        private System.Windows.Forms.NumericUpDown nUDHeight;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown nUDResoX;
        private System.Windows.Forms.CheckBox cbGrayscale;
        private System.Windows.Forms.CheckBox cbExceptColor;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbSkipToolOrder;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadPictureToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown nUDZBottom;
        private System.Windows.Forms.NumericUpDown nUDZTop;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cBCompress;
        private System.Windows.Forms.GroupBox GbStartGrayZ;
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
        private System.Windows.Forms.CheckedListBox CheckedListBoxTools;
        private System.Windows.Forms.CheckBox cBGCodeOutlineSmooth;
        private System.Windows.Forms.NumericUpDown nUDGCodeOutlineSmooth;
        private System.Windows.Forms.CheckBox cBGCodeOutlineShrink;
        private System.Windows.Forms.Label lblInfo1;
        private System.Windows.Forms.RadioButton RbStartGrayS;
        private System.Windows.Forms.RadioButton RbStartGrayZ;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown nUDSTop;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NumericUpDown nUDSBottom;
        private System.Windows.Forms.GroupBox gBgcodeSelection;
        private System.Windows.Forms.CheckBox cBResolutionPenWidth;
        private System.Windows.Forms.ListBox lBUseCase;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblLastUseCase;
        private System.Windows.Forms.Button btnGetPWMValues;
        private System.Windows.Forms.CheckBox cBOnlyLeftToRight;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.NumericUpDown nUDResoY;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage2Color;
        private System.Windows.Forms.TabPage tabPage2Gray;
        private System.Windows.Forms.GroupBox GbStartGrayS;
        private System.Windows.Forms.CheckBox cBLaserModeOffEnd;
        private System.Windows.Forms.CheckBox cBLaserModeOnStart;
        private System.Windows.Forms.CheckBox Cb2DViewHide0;
        private System.Windows.Forms.RadioButton rbEngravingPattern3;
        private System.Windows.Forms.RadioButton rbEngravingPattern4;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox CbPenUpOn0;
        private System.Windows.Forms.NumericUpDown NuDSpiralCenterY;
        private System.Windows.Forms.NumericUpDown NuDSpiralCenterX;
        private System.Windows.Forms.Label label25;
        internal System.Windows.Forms.Button BtnReloadPattern;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.RadioButton RbChannelB;
        private System.Windows.Forms.RadioButton RbChannelG;
        private System.Windows.Forms.RadioButton RbChannelR;
        private System.Windows.Forms.CheckBox cbGrayscaleChannel;
        private System.Windows.Forms.RadioButton RbChannelK;
        private System.Windows.Forms.RadioButton RbChannelY;
        private System.Windows.Forms.RadioButton RbChannelM;
        private System.Windows.Forms.RadioButton RbChannelC;
        public System.Windows.Forms.ComboBox CBoxPatternFiles;
        private System.Windows.Forms.TabPage tabPageSetup;
        private System.Windows.Forms.Label LblStartGrayMode;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TabPage tabPageSize;
        private System.Windows.Forms.ComboBox CboxToolFiles;
        private System.Windows.Forms.ComboBox CboxToolTable;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.CheckBox cBImportGCToolM0;
        private System.Windows.Forms.CheckBox cBImportGCTool;
        private System.Windows.Forms.CheckBox cBToolChange;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.GroupBox GbSpecialEffects;
        private System.Windows.Forms.GroupBox GbColorEffects;
        private System.Windows.Forms.GroupBox GbCOlorCorrection;
        private System.Windows.Forms.GroupBox GbUseCaseLoad;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox GbGrayscaleProcess;
        private System.Windows.Forms.RadioButton RbGrayscalePattern;
        private System.Windows.Forms.Label LblExceptionValue;
        private System.Windows.Forms.ToolStripStatusLabel LblMode;
        private System.Windows.Forms.GroupBox GbToolEnable;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label LblThreshold;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar tBarBWThreshold;
        private System.Windows.Forms.CheckBox CbBlackWhiteEnable;
        private System.Windows.Forms.RadioButton RbGrayscaleVector;
    }
}