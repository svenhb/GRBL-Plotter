﻿namespace GrblPlotter
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
                components.Dispose(); 
            if (disposing && (adjustedImage != null))
                adjustedImage.Dispose();
            if (disposing && (originalImage != null))
                originalImage.Dispose();
            if (disposing && (resultImage != null))
                resultImage.Dispose();
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
            this.GbOutputSizeSet = new System.Windows.Forms.GroupBox();
            this.rbEngravingPattern2 = new System.Windows.Forms.RadioButton();
            this.lblInfo1 = new System.Windows.Forms.Label();
            this.rbEngravingPattern1 = new System.Windows.Forms.RadioButton();
            this.btnKeepSizeReso = new System.Windows.Forms.Button();
            this.btnKeepSizeWidth = new System.Windows.Forms.Button();
            this.lblSizeOrig = new System.Windows.Forms.Label();
            this.lblSizeResult = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
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
            this.label27 = new System.Windows.Forms.Label();
            this.CBoxPatternFiles = new System.Windows.Forms.ComboBox();
            this.BtnReloadPattern = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.CbPenUpOn0 = new System.Windows.Forms.CheckBox();
            this.cBCompress = new System.Windows.Forms.CheckBox();
            this.rbEngravingPattern4 = new System.Windows.Forms.RadioButton();
            this.rbEngravingPattern3 = new System.Windows.Forms.RadioButton();
            this.label25 = new System.Windows.Forms.Label();
            this.NuDSpiralCenterY = new System.Windows.Forms.NumericUpDown();
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
            this.button3 = new System.Windows.Forms.Button();
            this.label29 = new System.Windows.Forms.Label();
            this.RbStartGrayS = new System.Windows.Forms.RadioButton();
            this.RbStartGraySpecial = new System.Windows.Forms.RadioButton();
            this.RbStartGrayZ = new System.Windows.Forms.RadioButton();
            this.GbColorReplacingMode = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GbStartGrayZ = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.cBPreview = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSetup = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.GbUseCaseLoad = new System.Windows.Forms.GroupBox();
            this.lBUseCase = new System.Windows.Forms.ListBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage2Color = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.CboxToolTable = new System.Windows.Forms.ComboBox();
            this.CboxToolFiles = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.tabPage2Gray = new System.Windows.Forms.TabPage();
            this.GbStartGraySpecial = new System.Windows.Forms.GroupBox();
            this.label43 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.GbStartGrayS = new System.Windows.Forms.GroupBox();
            this.tabPageSize = new System.Windows.Forms.TabPage();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.RbEngravingPattern = new System.Windows.Forms.RadioButton();
            this.GbEngravingPattern = new System.Windows.Forms.GroupBox();
            this.RbEngravingLine = new System.Windows.Forms.RadioButton();
            this.GbEngravingLine = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.gBgcodeSelection = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cBGCodeFill = new System.Windows.Forms.CheckBox();
            this.cBGCodeOutline = new System.Windows.Forms.CheckBox();
            this.cBGCodeOutlineSmooth = new System.Windows.Forms.CheckBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.GbPixelArt = new System.Windows.Forms.GroupBox();
            this.RbPixelArtShape = new System.Windows.Forms.RadioButton();
            this.GbDrawShape = new System.Windows.Forms.GroupBox();
            this.TbPixelArtDrawShapeFileDialog = new System.Windows.Forms.Button();
            this.label33 = new System.Windows.Forms.Label();
            this.RbPixelArtDrawShapeRect = new System.Windows.Forms.RadioButton();
            this.label32 = new System.Windows.Forms.Label();
            this.BtnPixelArtCalcSize = new System.Windows.Forms.Button();
            this.label31 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.GbGrayscaleProcess = new System.Windows.Forms.GroupBox();
            this.RbPixelArt = new System.Windows.Forms.RadioButton();
            this.RbGrayscaleVector = new System.Windows.Forms.RadioButton();
            this.RbGrayscalePattern = new System.Windows.Forms.RadioButton();
            this.GbOutputSizeShow = new System.Windows.Forms.GroupBox();
            this.LbLSizeYCode = new System.Windows.Forms.Label();
            this.LbLSizeXCode = new System.Windows.Forms.Label();
            this.LbLSizeYPic = new System.Windows.Forms.Label();
            this.LbLSizeXPic = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.GbToolEnable = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.CbSortInvert = new System.Windows.Forms.CheckBox();
            this.RbSortToolsByNumber = new System.Windows.Forms.RadioButton();
            this.RbSortToolsByPixel = new System.Windows.Forms.RadioButton();
            this.tBToolList = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.CheckedListBoxTools = new System.Windows.Forms.CheckedListBox();
            this.GbColorReduction = new System.Windows.Forms.GroupBox();
            this.LblExceptionValue = new System.Windows.Forms.Label();
            this.cBReduceColorsToolTable = new System.Windows.Forms.CheckBox();
            this.cBReduceColorsDithering = new System.Windows.Forms.CheckBox();
            this.GbConversionWizzard = new System.Windows.Forms.GroupBox();
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
            this.cBResolutionPenWidth = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lblLastUseCase = new System.Windows.Forms.Label();
            this.cBToolChange = new System.Windows.Forms.CheckBox();
            this.cBImportGCToolM0 = new System.Windows.Forms.CheckBox();
            this.cBImportGCTool = new System.Windows.Forms.CheckBox();
            this.tBCodeValue2 = new System.Windows.Forms.TextBox();
            this.tBCodeAfter = new System.Windows.Forms.TextBox();
            this.tBCodeValue1 = new System.Windows.Forms.TextBox();
            this.tBCodeBefore = new System.Windows.Forms.TextBox();
            this.nUDSpecialTop = new System.Windows.Forms.NumericUpDown();
            this.nUDSpecialBottom = new System.Windows.Forms.NumericUpDown();
            this.CbLaserOnly = new System.Windows.Forms.CheckBox();
            this.cBLaserModeOffEnd = new System.Windows.Forms.CheckBox();
            this.cBLaserModeOnStart = new System.Windows.Forms.CheckBox();
            this.nUDSBottom = new System.Windows.Forms.NumericUpDown();
            this.nUDSTop = new System.Windows.Forms.NumericUpDown();
            this.nUDZTop = new System.Windows.Forms.NumericUpDown();
            this.nUDZBottom = new System.Windows.Forms.NumericUpDown();
            this.nUDResoY = new System.Windows.Forms.NumericUpDown();
            this.Cb2DViewHide0 = new System.Windows.Forms.CheckBox();
            this.CbEngravingTopDown = new System.Windows.Forms.CheckBox();
            this.CbEngravingCross = new System.Windows.Forms.CheckBox();
            this.cBOnlyLeftToRight = new System.Windows.Forms.CheckBox();
            this.NudEngravingAngle = new System.Windows.Forms.NumericUpDown();
            this.TbPixelArtDrawShapeScript = new System.Windows.Forms.TextBox();
            this.RbPixelArtDrawShapeScript = new System.Windows.Forms.RadioButton();
            this.CbPixelArtShapeFill = new System.Windows.Forms.CheckBox();
            this.NuDPixelArtShapePenDiameter = new System.Windows.Forms.NumericUpDown();
            this.RbPixelArtDrawShapeCircle = new System.Windows.Forms.RadioButton();
            this.NuDPixelArtGapSize = new System.Windows.Forms.NumericUpDown();
            this.NuDPixelArtDotSize = new System.Windows.Forms.NumericUpDown();
            this.NuDPixelArtDotsPerPixel = new System.Windows.Forms.NumericUpDown();
            this.RbPixelArtPbP = new System.Windows.Forms.RadioButton();
            this.nUDResoX = new System.Windows.Forms.NumericUpDown();
            this.nUDHeight = new System.Windows.Forms.NumericUpDown();
            this.nUDWidth = new System.Windows.Forms.NumericUpDown();
            this.cbLockRatio = new System.Windows.Forms.CheckBox();
            this.CbPixelArtLimit = new System.Windows.Forms.CheckBox();
            this.NuDPixelArtLimitCount = new System.Windows.Forms.NumericUpDown();
            this.label44 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.GbOutputSizeSet.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBWThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarBrightness)).BeginInit();
            this.groupBox7.SuspendLayout();
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
            this.GbColorReplacingMode.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.GbStartGrayZ.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.GbUseCaseLoad.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage2Color.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2Gray.SuspendLayout();
            this.GbStartGraySpecial.SuspendLayout();
            this.GbStartGrayS.SuspendLayout();
            this.tabPageSize.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.GbEngravingPattern.SuspendLayout();
            this.GbEngravingLine.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.gBgcodeSelection.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.GbPixelArt.SuspendLayout();
            this.GbDrawShape.SuspendLayout();
            this.GbGrayscaleProcess.SuspendLayout();
            this.GbOutputSizeShow.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.GbToolEnable.SuspendLayout();
            this.GbColorReduction.SuspendLayout();
            this.GbConversionWizzard.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.GbSpecialEffects.SuspendLayout();
            this.GbColorEffects.SuspendLayout();
            this.GbCOlorCorrection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpecialTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpecialBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudEngravingAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtShapePenDiameter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtGapSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtDotSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtDotsPerPixel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtLimitCount)).BeginInit();
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
            // GbOutputSizeSet
            // 
            this.GbOutputSizeSet.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbOutputSizeSet.Controls.Add(this.rbEngravingPattern2);
            this.GbOutputSizeSet.Controls.Add(this.lblInfo1);
            this.GbOutputSizeSet.Controls.Add(this.rbEngravingPattern1);
            this.GbOutputSizeSet.Controls.Add(this.btnKeepSizeReso);
            this.GbOutputSizeSet.Controls.Add(this.btnKeepSizeWidth);
            this.GbOutputSizeSet.Controls.Add(this.lblSizeOrig);
            this.GbOutputSizeSet.Controls.Add(this.nUDResoX);
            this.GbOutputSizeSet.Controls.Add(this.lblSizeResult);
            this.GbOutputSizeSet.Controls.Add(this.nUDHeight);
            this.GbOutputSizeSet.Controls.Add(this.nUDWidth);
            this.GbOutputSizeSet.Controls.Add(this.label4);
            this.GbOutputSizeSet.Controls.Add(this.label6);
            this.GbOutputSizeSet.Controls.Add(this.label5);
            this.GbOutputSizeSet.Controls.Add(this.cbLockRatio);
            resources.ApplyResources(this.GbOutputSizeSet, "GbOutputSizeSet");
            this.GbOutputSizeSet.Name = "GbOutputSizeSet";
            this.GbOutputSizeSet.TabStop = false;
            // 
            // rbEngravingPattern2
            // 
            resources.ApplyResources(this.rbEngravingPattern2, "rbEngravingPattern2");
            this.rbEngravingPattern2.Name = "rbEngravingPattern2";
            this.rbEngravingPattern2.UseVisualStyleBackColor = true;
            this.rbEngravingPattern2.CheckedChanged += new System.EventHandler(this.RbEngravingPattern2_CheckedChanged);
            // 
            // lblInfo1
            // 
            resources.ApplyResources(this.lblInfo1, "lblInfo1");
            this.lblInfo1.Name = "lblInfo1";
            // 
            // rbEngravingPattern1
            // 
            resources.ApplyResources(this.rbEngravingPattern1, "rbEngravingPattern1");
            this.rbEngravingPattern1.Name = "rbEngravingPattern1";
            this.rbEngravingPattern1.UseVisualStyleBackColor = true;
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
            // lblSizeResult
            // 
            resources.ApplyResources(this.lblSizeResult, "lblSizeResult");
            this.lblSizeResult.BackColor = System.Drawing.Color.White;
            this.lblSizeResult.Name = "lblSizeResult";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.Color.Yellow;
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.BackColor = System.Drawing.Color.Yellow;
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
            this.groupBox7.BackColor = System.Drawing.Color.WhiteSmoke;
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
            // cBCompress
            // 
            resources.ApplyResources(this.cBCompress, "cBCompress");
            this.cBCompress.Checked = true;
            this.cBCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBCompress.Name = "cBCompress";
            this.toolTip1.SetToolTip(this.cBCompress, resources.GetString("cBCompress.ToolTip"));
            this.cBCompress.UseVisualStyleBackColor = true;
            // 
            // rbEngravingPattern4
            // 
            resources.ApplyResources(this.rbEngravingPattern4, "rbEngravingPattern4");
            this.rbEngravingPattern4.Name = "rbEngravingPattern4";
            this.toolTip1.SetToolTip(this.rbEngravingPattern4, resources.GetString("rbEngravingPattern4.ToolTip"));
            this.rbEngravingPattern4.UseVisualStyleBackColor = true;
            // 
            // rbEngravingPattern3
            // 
            resources.ApplyResources(this.rbEngravingPattern3, "rbEngravingPattern3");
            this.rbEngravingPattern3.Checked = true;
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
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.Tag = "id=form-image";
            this.toolTip1.SetToolTip(this.button3, resources.GetString("button3.ToolTip"));
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            this.toolTip1.SetToolTip(this.label29, resources.GetString("label29.ToolTip"));
            // 
            // RbStartGrayS
            // 
            resources.ApplyResources(this.RbStartGrayS, "RbStartGrayS");
            this.RbStartGrayS.Name = "RbStartGrayS";
            this.toolTip1.SetToolTip(this.RbStartGrayS, resources.GetString("RbStartGrayS.ToolTip"));
            this.RbStartGrayS.UseVisualStyleBackColor = true;
            this.RbStartGrayS.CheckedChanged += new System.EventHandler(this.RbGrayZ_CheckedChanged);
            // 
            // RbStartGraySpecial
            // 
            resources.ApplyResources(this.RbStartGraySpecial, "RbStartGraySpecial");
            this.RbStartGraySpecial.Name = "RbStartGraySpecial";
            this.toolTip1.SetToolTip(this.RbStartGraySpecial, resources.GetString("RbStartGraySpecial.ToolTip"));
            this.RbStartGraySpecial.UseVisualStyleBackColor = true;
            this.RbStartGraySpecial.CheckedChanged += new System.EventHandler(this.RbGrayZ_CheckedChanged);
            // 
            // RbStartGrayZ
            // 
            this.RbStartGrayZ.BackColor = System.Drawing.Color.Yellow;
            this.RbStartGrayZ.Checked = true;
            resources.ApplyResources(this.RbStartGrayZ, "RbStartGrayZ");
            this.RbStartGrayZ.Name = "RbStartGrayZ";
            this.RbStartGrayZ.TabStop = true;
            this.toolTip1.SetToolTip(this.RbStartGrayZ, resources.GetString("RbStartGrayZ.ToolTip"));
            this.RbStartGrayZ.UseVisualStyleBackColor = false;
            this.RbStartGrayZ.CheckedChanged += new System.EventHandler(this.RbGrayZ_CheckedChanged);
            // 
            // GbColorReplacingMode
            // 
            this.GbColorReplacingMode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbColorReplacingMode.Controls.Add(this.rBMode2);
            this.GbColorReplacingMode.Controls.Add(this.rBMode1);
            this.GbColorReplacingMode.Controls.Add(this.rBMode0);
            resources.ApplyResources(this.GbColorReplacingMode, "GbColorReplacingMode");
            this.GbColorReplacingMode.Name = "GbColorReplacingMode";
            this.GbColorReplacingMode.TabStop = false;
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
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
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
            this.tabPage2Gray.Controls.Add(this.RbStartGraySpecial);
            this.tabPage2Gray.Controls.Add(this.RbStartGrayZ);
            this.tabPage2Gray.Controls.Add(this.GbStartGraySpecial);
            this.tabPage2Gray.Controls.Add(this.RbStartGrayS);
            this.tabPage2Gray.Controls.Add(this.GbStartGrayS);
            this.tabPage2Gray.Controls.Add(this.GbStartGrayZ);
            resources.ApplyResources(this.tabPage2Gray, "tabPage2Gray");
            this.tabPage2Gray.Name = "tabPage2Gray";
            this.tabPage2Gray.UseVisualStyleBackColor = true;
            // 
            // GbStartGraySpecial
            // 
            this.GbStartGraySpecial.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbStartGraySpecial.Controls.Add(this.tBCodeValue2);
            this.GbStartGraySpecial.Controls.Add(this.label43);
            this.GbStartGraySpecial.Controls.Add(this.label42);
            this.GbStartGraySpecial.Controls.Add(this.label41);
            this.GbStartGraySpecial.Controls.Add(this.label40);
            this.GbStartGraySpecial.Controls.Add(this.tBCodeAfter);
            this.GbStartGraySpecial.Controls.Add(this.tBCodeValue1);
            this.GbStartGraySpecial.Controls.Add(this.tBCodeBefore);
            this.GbStartGraySpecial.Controls.Add(this.label38);
            this.GbStartGraySpecial.Controls.Add(this.nUDSpecialTop);
            this.GbStartGraySpecial.Controls.Add(this.label39);
            this.GbStartGraySpecial.Controls.Add(this.nUDSpecialBottom);
            resources.ApplyResources(this.GbStartGraySpecial, "GbStartGraySpecial");
            this.GbStartGraySpecial.Name = "GbStartGraySpecial";
            this.GbStartGraySpecial.TabStop = false;
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.Name = "label43";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // GbStartGrayS
            // 
            this.GbStartGrayS.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbStartGrayS.Controls.Add(this.CbLaserOnly);
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
            // tabPageSize
            // 
            this.tabPageSize.Controls.Add(this.tabControl3);
            this.tabPageSize.Controls.Add(this.GbGrayscaleProcess);
            this.tabPageSize.Controls.Add(this.GbOutputSizeSet);
            this.tabPageSize.Controls.Add(this.GbOutputSizeShow);
            resources.ApplyResources(this.tabPageSize, "tabPageSize");
            this.tabPageSize.Name = "tabPageSize";
            this.tabPageSize.UseVisualStyleBackColor = true;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage2);
            this.tabControl3.Controls.Add(this.tabPage3);
            this.tabControl3.Controls.Add(this.tabPage5);
            this.tabControl3.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            resources.ApplyResources(this.tabControl3, "tabControl3");
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControl3_DrawItem);
            this.tabControl3.SelectedIndexChanged += new System.EventHandler(this.TabControl3_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.RbEngravingPattern);
            this.tabPage2.Controls.Add(this.GbEngravingPattern);
            this.tabPage2.Controls.Add(this.RbEngravingLine);
            this.tabPage2.Controls.Add(this.GbEngravingLine);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // RbEngravingPattern
            // 
            resources.ApplyResources(this.RbEngravingPattern, "RbEngravingPattern");
            this.RbEngravingPattern.Name = "RbEngravingPattern";
            this.RbEngravingPattern.TabStop = true;
            this.RbEngravingPattern.UseVisualStyleBackColor = true;
            this.RbEngravingPattern.CheckedChanged += new System.EventHandler(this.RbEngravingLine_CheckedChanged);
            // 
            // GbEngravingPattern
            // 
            this.GbEngravingPattern.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbEngravingPattern.Controls.Add(this.label27);
            this.GbEngravingPattern.Controls.Add(this.rbEngravingPattern3);
            this.GbEngravingPattern.Controls.Add(this.label26);
            this.GbEngravingPattern.Controls.Add(this.CBoxPatternFiles);
            this.GbEngravingPattern.Controls.Add(this.label25);
            this.GbEngravingPattern.Controls.Add(this.NuDSpiralCenterY);
            this.GbEngravingPattern.Controls.Add(this.rbEngravingPattern4);
            this.GbEngravingPattern.Controls.Add(this.NuDSpiralCenterX);
            this.GbEngravingPattern.Controls.Add(this.BtnReloadPattern);
            resources.ApplyResources(this.GbEngravingPattern, "GbEngravingPattern");
            this.GbEngravingPattern.Name = "GbEngravingPattern";
            this.GbEngravingPattern.TabStop = false;
            // 
            // RbEngravingLine
            // 
            this.RbEngravingLine.BackColor = System.Drawing.Color.Yellow;
            this.RbEngravingLine.Checked = true;
            resources.ApplyResources(this.RbEngravingLine, "RbEngravingLine");
            this.RbEngravingLine.Name = "RbEngravingLine";
            this.RbEngravingLine.TabStop = true;
            this.RbEngravingLine.UseVisualStyleBackColor = false;
            this.RbEngravingLine.CheckedChanged += new System.EventHandler(this.RbEngravingLine_CheckedChanged);
            // 
            // GbEngravingLine
            // 
            this.GbEngravingLine.BackColor = System.Drawing.Color.Yellow;
            this.GbEngravingLine.Controls.Add(this.CbEngravingTopDown);
            this.GbEngravingLine.Controls.Add(this.label21);
            this.GbEngravingLine.Controls.Add(this.CbEngravingCross);
            this.GbEngravingLine.Controls.Add(this.cBOnlyLeftToRight);
            this.GbEngravingLine.Controls.Add(this.NudEngravingAngle);
            resources.ApplyResources(this.GbEngravingLine, "GbEngravingLine");
            this.GbEngravingLine.Name = "GbEngravingLine";
            this.GbEngravingLine.TabStop = false;
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.gBgcodeSelection);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
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
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.GbPixelArt);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // GbPixelArt
            // 
            this.GbPixelArt.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbPixelArt.Controls.Add(this.label44);
            this.GbPixelArt.Controls.Add(this.NuDPixelArtLimitCount);
            this.GbPixelArt.Controls.Add(this.CbPixelArtLimit);
            this.GbPixelArt.Controls.Add(this.RbPixelArtShape);
            this.GbPixelArt.Controls.Add(this.GbDrawShape);
            this.GbPixelArt.Controls.Add(this.label32);
            this.GbPixelArt.Controls.Add(this.NuDPixelArtGapSize);
            this.GbPixelArt.Controls.Add(this.BtnPixelArtCalcSize);
            this.GbPixelArt.Controls.Add(this.label31);
            this.GbPixelArt.Controls.Add(this.NuDPixelArtDotSize);
            this.GbPixelArt.Controls.Add(this.label29);
            this.GbPixelArt.Controls.Add(this.NuDPixelArtDotsPerPixel);
            this.GbPixelArt.Controls.Add(this.RbPixelArtPbP);
            this.GbPixelArt.Controls.Add(this.label24);
            resources.ApplyResources(this.GbPixelArt, "GbPixelArt");
            this.GbPixelArt.Name = "GbPixelArt";
            this.GbPixelArt.TabStop = false;
            // 
            // RbPixelArtShape
            // 
            resources.ApplyResources(this.RbPixelArtShape, "RbPixelArtShape");
            this.RbPixelArtShape.Name = "RbPixelArtShape";
            this.RbPixelArtShape.UseVisualStyleBackColor = true;
            // 
            // GbDrawShape
            // 
            this.GbDrawShape.Controls.Add(this.TbPixelArtDrawShapeFileDialog);
            this.GbDrawShape.Controls.Add(this.TbPixelArtDrawShapeScript);
            this.GbDrawShape.Controls.Add(this.RbPixelArtDrawShapeScript);
            this.GbDrawShape.Controls.Add(this.CbPixelArtShapeFill);
            this.GbDrawShape.Controls.Add(this.label33);
            this.GbDrawShape.Controls.Add(this.NuDPixelArtShapePenDiameter);
            this.GbDrawShape.Controls.Add(this.RbPixelArtDrawShapeRect);
            this.GbDrawShape.Controls.Add(this.RbPixelArtDrawShapeCircle);
            resources.ApplyResources(this.GbDrawShape, "GbDrawShape");
            this.GbDrawShape.Name = "GbDrawShape";
            this.GbDrawShape.TabStop = false;
            // 
            // TbPixelArtDrawShapeFileDialog
            // 
            resources.ApplyResources(this.TbPixelArtDrawShapeFileDialog, "TbPixelArtDrawShapeFileDialog");
            this.TbPixelArtDrawShapeFileDialog.Name = "TbPixelArtDrawShapeFileDialog";
            this.TbPixelArtDrawShapeFileDialog.UseVisualStyleBackColor = true;
            this.TbPixelArtDrawShapeFileDialog.Click += new System.EventHandler(this.TbPixelArtDrawShapeFileDialog_Click);
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // RbPixelArtDrawShapeRect
            // 
            resources.ApplyResources(this.RbPixelArtDrawShapeRect, "RbPixelArtDrawShapeRect");
            this.RbPixelArtDrawShapeRect.Name = "RbPixelArtDrawShapeRect";
            this.RbPixelArtDrawShapeRect.UseVisualStyleBackColor = true;
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            // 
            // BtnPixelArtCalcSize
            // 
            resources.ApplyResources(this.BtnPixelArtCalcSize, "BtnPixelArtCalcSize");
            this.BtnPixelArtCalcSize.Name = "BtnPixelArtCalcSize";
            this.BtnPixelArtCalcSize.UseVisualStyleBackColor = true;
            this.BtnPixelArtCalcSize.Click += new System.EventHandler(this.BtnPixelArtCalcSize_Click);
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // GbGrayscaleProcess
            // 
            this.GbGrayscaleProcess.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbGrayscaleProcess.Controls.Add(this.RbPixelArt);
            this.GbGrayscaleProcess.Controls.Add(this.RbGrayscaleVector);
            this.GbGrayscaleProcess.Controls.Add(this.RbGrayscalePattern);
            resources.ApplyResources(this.GbGrayscaleProcess, "GbGrayscaleProcess");
            this.GbGrayscaleProcess.Name = "GbGrayscaleProcess";
            this.GbGrayscaleProcess.TabStop = false;
            // 
            // RbPixelArt
            // 
            resources.ApplyResources(this.RbPixelArt, "RbPixelArt");
            this.RbPixelArt.Name = "RbPixelArt";
            this.RbPixelArt.TabStop = true;
            this.RbPixelArt.UseVisualStyleBackColor = true;
            this.RbPixelArt.CheckedChanged += new System.EventHandler(this.RbGrayscaleVector_CheckedChanged);
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
            // GbOutputSizeShow
            // 
            this.GbOutputSizeShow.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbOutputSizeShow.Controls.Add(this.LbLSizeYCode);
            this.GbOutputSizeShow.Controls.Add(this.LbLSizeXCode);
            this.GbOutputSizeShow.Controls.Add(this.LbLSizeYPic);
            this.GbOutputSizeShow.Controls.Add(this.LbLSizeXPic);
            this.GbOutputSizeShow.Controls.Add(this.label37);
            this.GbOutputSizeShow.Controls.Add(this.label36);
            this.GbOutputSizeShow.Controls.Add(this.label35);
            this.GbOutputSizeShow.Controls.Add(this.label34);
            resources.ApplyResources(this.GbOutputSizeShow, "GbOutputSizeShow");
            this.GbOutputSizeShow.Name = "GbOutputSizeShow";
            this.GbOutputSizeShow.TabStop = false;
            // 
            // LbLSizeYCode
            // 
            resources.ApplyResources(this.LbLSizeYCode, "LbLSizeYCode");
            this.LbLSizeYCode.Name = "LbLSizeYCode";
            // 
            // LbLSizeXCode
            // 
            resources.ApplyResources(this.LbLSizeXCode, "LbLSizeXCode");
            this.LbLSizeXCode.Name = "LbLSizeXCode";
            // 
            // LbLSizeYPic
            // 
            resources.ApplyResources(this.LbLSizeYPic, "LbLSizeYPic");
            this.LbLSizeYPic.Name = "LbLSizeYPic";
            // 
            // LbLSizeXPic
            // 
            resources.ApplyResources(this.LbLSizeXPic, "LbLSizeXPic");
            this.LbLSizeXPic.Name = "LbLSizeXPic";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.GbToolEnable);
            this.tabPage4.Controls.Add(this.GbColorReplacingMode);
            this.tabPage4.Controls.Add(this.GbColorReduction);
            this.tabPage4.Controls.Add(this.GbConversionWizzard);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // GbToolEnable
            // 
            this.GbToolEnable.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbToolEnable.Controls.Add(this.label30);
            this.GbToolEnable.Controls.Add(this.CbSortInvert);
            this.GbToolEnable.Controls.Add(this.RbSortToolsByNumber);
            this.GbToolEnable.Controls.Add(this.RbSortToolsByPixel);
            this.GbToolEnable.Controls.Add(this.tBToolList);
            this.GbToolEnable.Controls.Add(this.cbSkipToolOrder);
            this.GbToolEnable.Controls.Add(this.label10);
            this.GbToolEnable.Controls.Add(this.CheckedListBoxTools);
            resources.ApplyResources(this.GbToolEnable, "GbToolEnable");
            this.GbToolEnable.Name = "GbToolEnable";
            this.GbToolEnable.TabStop = false;
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // CbSortInvert
            // 
            resources.ApplyResources(this.CbSortInvert, "CbSortInvert");
            this.CbSortInvert.Name = "CbSortInvert";
            this.CbSortInvert.UseVisualStyleBackColor = true;
            this.CbSortInvert.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // RbSortToolsByNumber
            // 
            resources.ApplyResources(this.RbSortToolsByNumber, "RbSortToolsByNumber");
            this.RbSortToolsByNumber.Name = "RbSortToolsByNumber";
            this.RbSortToolsByNumber.UseVisualStyleBackColor = true;
            this.RbSortToolsByNumber.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
            // 
            // RbSortToolsByPixel
            // 
            resources.ApplyResources(this.RbSortToolsByPixel, "RbSortToolsByPixel");
            this.RbSortToolsByPixel.Checked = true;
            this.RbSortToolsByPixel.Name = "RbSortToolsByPixel";
            this.RbSortToolsByPixel.TabStop = true;
            this.RbSortToolsByPixel.UseVisualStyleBackColor = true;
            this.RbSortToolsByPixel.CheckedChanged += new System.EventHandler(this.ApplyColorCorrectionsEvent);
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
            // GbColorReduction
            // 
            this.GbColorReduction.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbColorReduction.Controls.Add(this.LblExceptionValue);
            this.GbColorReduction.Controls.Add(this.cBReduceColorsToolTable);
            this.GbColorReduction.Controls.Add(this.nUDMaxColors);
            this.GbColorReduction.Controls.Add(this.cbExceptColor);
            this.GbColorReduction.Controls.Add(this.cBReduceColorsDithering);
            this.GbColorReduction.Controls.Add(this.lblColors);
            resources.ApplyResources(this.GbColorReduction, "GbColorReduction");
            this.GbColorReduction.Name = "GbColorReduction";
            this.GbColorReduction.TabStop = false;
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
            // GbConversionWizzard
            // 
            this.GbConversionWizzard.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbConversionWizzard.Controls.Add(this.btnPresetCorrection4);
            this.GbConversionWizzard.Controls.Add(this.label20);
            this.GbConversionWizzard.Controls.Add(this.nUDColorPercent);
            this.GbConversionWizzard.Controls.Add(this.btnPresetCorrection3);
            this.GbConversionWizzard.Controls.Add(this.btnPresetCorrection2);
            this.GbConversionWizzard.Controls.Add(this.btnResetCorrection);
            this.GbConversionWizzard.Controls.Add(this.btnPresetCorrection1);
            resources.ApplyResources(this.GbConversionWizzard, "GbConversionWizzard");
            this.GbConversionWizzard.Name = "GbConversionWizzard";
            this.GbConversionWizzard.TabStop = false;
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
            // lblLastUseCase
            // 
            resources.ApplyResources(this.lblLastUseCase, "lblLastUseCase");
            this.lblLastUseCase.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "useCaseLastLoaded", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblLastUseCase.Name = "lblLastUseCase";
            this.lblLastUseCase.Text = global::GrblPlotter.Properties.Settings.Default.useCaseLastLoaded;
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
            // tBCodeValue2
            // 
            this.tBCodeValue2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importImageSpecialCodeValue2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBCodeValue2, "tBCodeValue2");
            this.tBCodeValue2.Name = "tBCodeValue2";
            this.tBCodeValue2.Text = global::GrblPlotter.Properties.Settings.Default.importImageSpecialCodeValue2;
            // 
            // tBCodeAfter
            // 
            this.tBCodeAfter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importImageSpecialCodeAfter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBCodeAfter, "tBCodeAfter");
            this.tBCodeAfter.Name = "tBCodeAfter";
            this.tBCodeAfter.Text = global::GrblPlotter.Properties.Settings.Default.importImageSpecialCodeAfter;
            // 
            // tBCodeValue1
            // 
            this.tBCodeValue1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importImageSpecialCodeValue1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBCodeValue1, "tBCodeValue1");
            this.tBCodeValue1.Name = "tBCodeValue1";
            this.tBCodeValue1.Text = global::GrblPlotter.Properties.Settings.Default.importImageSpecialCodeValue1;
            // 
            // tBCodeBefore
            // 
            this.tBCodeBefore.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importImageSpecialCodeBefore", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBCodeBefore, "tBCodeBefore");
            this.tBCodeBefore.Name = "tBCodeBefore";
            this.tBCodeBefore.Text = global::GrblPlotter.Properties.Settings.Default.importImageSpecialCodeBefore;
            // 
            // nUDSpecialTop
            // 
            this.nUDSpecialTop.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageSpecialMax", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSpecialTop.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDSpecialTop, "nUDSpecialTop");
            this.nUDSpecialTop.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDSpecialTop.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDSpecialTop.Name = "nUDSpecialTop";
            this.nUDSpecialTop.Value = global::GrblPlotter.Properties.Settings.Default.importImageSpecialMax;
            // 
            // nUDSpecialBottom
            // 
            this.nUDSpecialBottom.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageSpecialMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSpecialBottom.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDSpecialBottom, "nUDSpecialBottom");
            this.nUDSpecialBottom.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDSpecialBottom.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDSpecialBottom.Name = "nUDSpecialBottom";
            this.toolTip1.SetToolTip(this.nUDSpecialBottom, resources.GetString("nUDSpecialBottom.ToolTip"));
            this.nUDSpecialBottom.Value = global::GrblPlotter.Properties.Settings.Default.importImageSpecialMin;
            // 
            // CbLaserOnly
            // 
            resources.ApplyResources(this.CbLaserOnly, "CbLaserOnly");
            this.CbLaserOnly.Checked = global::GrblPlotter.Properties.Settings.Default.importImageSLaserOnly;
            this.CbLaserOnly.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageSLaserOnly", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbLaserOnly.Name = "CbLaserOnly";
            this.toolTip1.SetToolTip(this.CbLaserOnly, resources.GetString("CbLaserOnly.ToolTip"));
            this.CbLaserOnly.UseVisualStyleBackColor = true;
            // 
            // cBLaserModeOffEnd
            // 
            resources.ApplyResources(this.cBLaserModeOffEnd, "cBLaserModeOffEnd");
            this.cBLaserModeOffEnd.Checked = global::GrblPlotter.Properties.Settings.Default.importImageSLasermodeEnd;
            this.cBLaserModeOffEnd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageSLasermodeEnd", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBLaserModeOffEnd.Name = "cBLaserModeOffEnd";
            this.toolTip1.SetToolTip(this.cBLaserModeOffEnd, resources.GetString("cBLaserModeOffEnd.ToolTip"));
            this.cBLaserModeOffEnd.UseVisualStyleBackColor = true;
            // 
            // cBLaserModeOnStart
            // 
            resources.ApplyResources(this.cBLaserModeOnStart, "cBLaserModeOnStart");
            this.cBLaserModeOnStart.Checked = global::GrblPlotter.Properties.Settings.Default.importImageSLasermodeStart;
            this.cBLaserModeOnStart.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageSLasermodeStart", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBLaserModeOnStart.Name = "cBLaserModeOnStart";
            this.toolTip1.SetToolTip(this.cBLaserModeOnStart, resources.GetString("cBLaserModeOnStart.ToolTip"));
            this.cBLaserModeOnStart.UseVisualStyleBackColor = true;
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
            // CbEngravingTopDown
            // 
            resources.ApplyResources(this.CbEngravingTopDown, "CbEngravingTopDown");
            this.CbEngravingTopDown.Checked = global::GrblPlotter.Properties.Settings.Default.importImageEngravingTopDown;
            this.CbEngravingTopDown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbEngravingTopDown.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageEngravingTopDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbEngravingTopDown.Name = "CbEngravingTopDown";
            this.toolTip1.SetToolTip(this.CbEngravingTopDown, resources.GetString("CbEngravingTopDown.ToolTip"));
            this.CbEngravingTopDown.UseVisualStyleBackColor = true;
            // 
            // CbEngravingCross
            // 
            resources.ApplyResources(this.CbEngravingCross, "CbEngravingCross");
            this.CbEngravingCross.Checked = global::GrblPlotter.Properties.Settings.Default.importImageEngravingCross;
            this.CbEngravingCross.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageEngravingCross", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbEngravingCross.Name = "CbEngravingCross";
            this.toolTip1.SetToolTip(this.CbEngravingCross, resources.GetString("CbEngravingCross.ToolTip"));
            this.CbEngravingCross.UseVisualStyleBackColor = true;
            // 
            // cBOnlyLeftToRight
            // 
            resources.ApplyResources(this.cBOnlyLeftToRight, "cBOnlyLeftToRight");
            this.cBOnlyLeftToRight.Checked = global::GrblPlotter.Properties.Settings.Default.importImageEngravingOneDirection;
            this.cBOnlyLeftToRight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBOnlyLeftToRight.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageEngravingOneDirection", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBOnlyLeftToRight.Name = "cBOnlyLeftToRight";
            this.toolTip1.SetToolTip(this.cBOnlyLeftToRight, resources.GetString("cBOnlyLeftToRight.ToolTip"));
            this.cBOnlyLeftToRight.UseVisualStyleBackColor = true;
            // 
            // NudEngravingAngle
            // 
            this.NudEngravingAngle.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImageEngravingAngle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudEngravingAngle.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.NudEngravingAngle, "NudEngravingAngle");
            this.NudEngravingAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.NudEngravingAngle.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.NudEngravingAngle.Name = "NudEngravingAngle";
            this.toolTip1.SetToolTip(this.NudEngravingAngle, resources.GetString("NudEngravingAngle.ToolTip"));
            this.NudEngravingAngle.Value = global::GrblPlotter.Properties.Settings.Default.importImageEngravingAngle;
            // 
            // TbPixelArtDrawShapeScript
            // 
            this.TbPixelArtDrawShapeScript.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtDrawShapeScriptText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.TbPixelArtDrawShapeScript, "TbPixelArtDrawShapeScript");
            this.TbPixelArtDrawShapeScript.Name = "TbPixelArtDrawShapeScript";
            this.TbPixelArtDrawShapeScript.Text = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtDrawShapeScriptText;
            // 
            // RbPixelArtDrawShapeScript
            // 
            resources.ApplyResources(this.RbPixelArtDrawShapeScript, "RbPixelArtDrawShapeScript");
            this.RbPixelArtDrawShapeScript.Checked = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtDrawShapeScript;
            this.RbPixelArtDrawShapeScript.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtDrawShapeScript", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RbPixelArtDrawShapeScript.Name = "RbPixelArtDrawShapeScript";
            this.RbPixelArtDrawShapeScript.TabStop = true;
            this.RbPixelArtDrawShapeScript.UseVisualStyleBackColor = true;
            // 
            // CbPixelArtShapeFill
            // 
            resources.ApplyResources(this.CbPixelArtShapeFill, "CbPixelArtShapeFill");
            this.CbPixelArtShapeFill.Checked = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtShapeFill;
            this.CbPixelArtShapeFill.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtShapeFill", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbPixelArtShapeFill.Name = "CbPixelArtShapeFill";
            this.CbPixelArtShapeFill.UseVisualStyleBackColor = true;
            // 
            // NuDPixelArtShapePenDiameter
            // 
            this.NuDPixelArtShapePenDiameter.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtShapePenDiameter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NuDPixelArtShapePenDiameter.DecimalPlaces = 1;
            this.NuDPixelArtShapePenDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NuDPixelArtShapePenDiameter, "NuDPixelArtShapePenDiameter");
            this.NuDPixelArtShapePenDiameter.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NuDPixelArtShapePenDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NuDPixelArtShapePenDiameter.Name = "NuDPixelArtShapePenDiameter";
            this.NuDPixelArtShapePenDiameter.Value = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtShapePenDiameter;
            // 
            // RbPixelArtDrawShapeCircle
            // 
            resources.ApplyResources(this.RbPixelArtDrawShapeCircle, "RbPixelArtDrawShapeCircle");
            this.RbPixelArtDrawShapeCircle.Checked = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtDrawShapeCircle;
            this.RbPixelArtDrawShapeCircle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtDrawShapeCircle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RbPixelArtDrawShapeCircle.Name = "RbPixelArtDrawShapeCircle";
            this.RbPixelArtDrawShapeCircle.TabStop = true;
            this.RbPixelArtDrawShapeCircle.UseVisualStyleBackColor = true;
            // 
            // NuDPixelArtGapSize
            // 
            this.NuDPixelArtGapSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtGapSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NuDPixelArtGapSize.DecimalPlaces = 1;
            this.NuDPixelArtGapSize.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.NuDPixelArtGapSize, "NuDPixelArtGapSize");
            this.NuDPixelArtGapSize.Name = "NuDPixelArtGapSize";
            this.NuDPixelArtGapSize.Value = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtGapSize;
            this.NuDPixelArtGapSize.ValueChanged += new System.EventHandler(this.BtnPixelArtCalcSize_Click);
            // 
            // NuDPixelArtDotSize
            // 
            this.NuDPixelArtDotSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtDotSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NuDPixelArtDotSize.DecimalPlaces = 1;
            this.NuDPixelArtDotSize.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.NuDPixelArtDotSize, "NuDPixelArtDotSize");
            this.NuDPixelArtDotSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NuDPixelArtDotSize.Name = "NuDPixelArtDotSize";
            this.NuDPixelArtDotSize.Value = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtDotSize;
            this.NuDPixelArtDotSize.ValueChanged += new System.EventHandler(this.BtnPixelArtCalcSize_Click);
            // 
            // NuDPixelArtDotsPerPixel
            // 
            this.NuDPixelArtDotsPerPixel.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtDotsPerPixel", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NuDPixelArtDotsPerPixel, "NuDPixelArtDotsPerPixel");
            this.NuDPixelArtDotsPerPixel.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NuDPixelArtDotsPerPixel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NuDPixelArtDotsPerPixel.Name = "NuDPixelArtDotsPerPixel";
            this.NuDPixelArtDotsPerPixel.Value = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtDotsPerPixel;
            this.NuDPixelArtDotsPerPixel.ValueChanged += new System.EventHandler(this.BtnPixelArtCalcSize_Click);
            // 
            // RbPixelArtPbP
            // 
            resources.ApplyResources(this.RbPixelArtPbP, "RbPixelArtPbP");
            this.RbPixelArtPbP.Checked = global::GrblPlotter.Properties.Settings.Default.importImagePixelArtDrawDot;
            this.RbPixelArtPbP.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImagePixelArtDrawDot", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RbPixelArtPbP.Name = "RbPixelArtPbP";
            this.RbPixelArtPbP.TabStop = true;
            this.RbPixelArtPbP.UseVisualStyleBackColor = true;
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
            // cbLockRatio
            // 
            resources.ApplyResources(this.cbLockRatio, "cbLockRatio");
            this.cbLockRatio.Checked = global::GrblPlotter.Properties.Settings.Default.importImageKeepRatio;
            this.cbLockRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLockRatio.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importImageKeepRatio", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbLockRatio.Name = "cbLockRatio";
            this.cbLockRatio.UseVisualStyleBackColor = true;
            // 
            // CbPixelArtLimit
            // 
            resources.ApplyResources(this.CbPixelArtLimit, "CbPixelArtLimit");
            this.CbPixelArtLimit.Checked = true;
            this.CbPixelArtLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbPixelArtLimit.Name = "CbPixelArtLimit";
            this.toolTip1.SetToolTip(this.CbPixelArtLimit, resources.GetString("CbPixelArtLimit.ToolTip"));
            this.CbPixelArtLimit.UseVisualStyleBackColor = true;
            // 
            // NuDPixelArtLimitCount
            // 
            resources.ApplyResources(this.NuDPixelArtLimitCount, "NuDPixelArtLimitCount");
            this.NuDPixelArtLimitCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NuDPixelArtLimitCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NuDPixelArtLimitCount.Name = "NuDPixelArtLimitCount";
            this.toolTip1.SetToolTip(this.NuDPixelArtLimitCount, resources.GetString("NuDPixelArtLimitCount.ToolTip"));
            this.NuDPixelArtLimitCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.Name = "label44";
            // 
            // GCodeFromImage
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button3);
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
            this.GbOutputSizeSet.ResumeLayout(false);
            this.GbOutputSizeSet.PerformLayout();
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
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
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
            this.GbColorReplacingMode.ResumeLayout(false);
            this.GbColorReplacingMode.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.GbStartGrayZ.ResumeLayout(false);
            this.GbStartGrayZ.PerformLayout();
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
            this.GbStartGraySpecial.ResumeLayout(false);
            this.GbStartGraySpecial.PerformLayout();
            this.GbStartGrayS.ResumeLayout(false);
            this.GbStartGrayS.PerformLayout();
            this.tabPageSize.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.GbEngravingPattern.ResumeLayout(false);
            this.GbEngravingPattern.PerformLayout();
            this.GbEngravingLine.ResumeLayout(false);
            this.GbEngravingLine.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.gBgcodeSelection.ResumeLayout(false);
            this.gBgcodeSelection.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.GbPixelArt.ResumeLayout(false);
            this.GbPixelArt.PerformLayout();
            this.GbDrawShape.ResumeLayout(false);
            this.GbDrawShape.PerformLayout();
            this.GbGrayscaleProcess.ResumeLayout(false);
            this.GbGrayscaleProcess.PerformLayout();
            this.GbOutputSizeShow.ResumeLayout(false);
            this.GbOutputSizeShow.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.GbToolEnable.ResumeLayout(false);
            this.GbToolEnable.PerformLayout();
            this.GbColorReduction.ResumeLayout(false);
            this.GbColorReduction.PerformLayout();
            this.GbConversionWizzard.ResumeLayout(false);
            this.GbConversionWizzard.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.GbSpecialEffects.ResumeLayout(false);
            this.GbSpecialEffects.PerformLayout();
            this.GbColorEffects.ResumeLayout(false);
            this.GbColorEffects.PerformLayout();
            this.GbCOlorCorrection.ResumeLayout(false);
            this.GbCOlorCorrection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tBarSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpecialTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpecialBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudEngravingAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtShapePenDiameter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtGapSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtDotSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtDotsPerPixel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDResoX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NuDPixelArtLimitCount)).EndInit();
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
        private System.Windows.Forms.RadioButton rbEngravingPattern2;
        private System.Windows.Forms.RadioButton rbEngravingPattern1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox GbOutputSizeSet;
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
        private System.Windows.Forms.GroupBox GbColorReplacingMode;
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
        private System.Windows.Forms.GroupBox GbConversionWizzard;
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
        private System.Windows.Forms.GroupBox GbColorReduction;
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
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox CbLaserOnly;
        private System.Windows.Forms.NumericUpDown NudEngravingAngle;
        private System.Windows.Forms.CheckBox CbEngravingCross;
        private System.Windows.Forms.RadioButton RbEngravingLine;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.GroupBox GbEngravingLine;
        private System.Windows.Forms.RadioButton RbEngravingPattern;
        private System.Windows.Forms.GroupBox GbEngravingPattern;
        private System.Windows.Forms.CheckBox CbEngravingTopDown;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox GbPixelArt;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.RadioButton RbPixelArt;
        private System.Windows.Forms.NumericUpDown NuDPixelArtDotsPerPixel;
        private System.Windows.Forms.RadioButton RbPixelArtShape;
        private System.Windows.Forms.RadioButton RbPixelArtPbP;
        private System.Windows.Forms.NumericUpDown NuDPixelArtDotSize;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button BtnPixelArtCalcSize;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.NumericUpDown NuDPixelArtGapSize;
        private System.Windows.Forms.GroupBox GbDrawShape;
        private System.Windows.Forms.RadioButton RbPixelArtDrawShapeCircle;
        private System.Windows.Forms.RadioButton RbSortToolsByNumber;
        private System.Windows.Forms.RadioButton RbSortToolsByPixel;
        private System.Windows.Forms.CheckBox CbSortInvert;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.RadioButton RbPixelArtDrawShapeRect;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.NumericUpDown NuDPixelArtShapePenDiameter;
        private System.Windows.Forms.CheckBox CbPixelArtShapeFill;
        private System.Windows.Forms.GroupBox GbOutputSizeShow;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label LbLSizeYCode;
        private System.Windows.Forms.Label LbLSizeXCode;
        private System.Windows.Forms.Label LbLSizeYPic;
        private System.Windows.Forms.Label LbLSizeXPic;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox TbPixelArtDrawShapeScript;
        private System.Windows.Forms.RadioButton RbPixelArtDrawShapeScript;
        private System.Windows.Forms.Button TbPixelArtDrawShapeFileDialog;
        private System.Windows.Forms.GroupBox GbStartGraySpecial;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.NumericUpDown nUDSpecialTop;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.NumericUpDown nUDSpecialBottom;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.TextBox tBCodeAfter;
        private System.Windows.Forms.TextBox tBCodeValue1;
        private System.Windows.Forms.TextBox tBCodeBefore;
        private System.Windows.Forms.TextBox tBCodeValue2;
        private System.Windows.Forms.RadioButton RbStartGraySpecial;
        private System.Windows.Forms.CheckBox CbPixelArtLimit;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.NumericUpDown NuDPixelArtLimitCount;
    }
}