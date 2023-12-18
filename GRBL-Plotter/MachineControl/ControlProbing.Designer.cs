namespace GrblPlotter
{
    partial class ControlProbing
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
	//			cBold.Dispose();
	//			cBnow.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlProbing));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cBZProbing = new System.Windows.Forms.CheckBox();
            this.gBHardware = new System.Windows.Forms.GroupBox();
            this.LblProbeContact = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nUDProbeDiameter = new System.Windows.Forms.NumericUpDown();
            this.nUDOffsetX = new System.Windows.Forms.NumericUpDown();
            this.nUDOffsetY = new System.Windows.Forms.NumericUpDown();
            this.nUDOffsetZ = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.lblProbeFinal = new System.Windows.Forms.Label();
            this.nUDWorkpieceDiameter = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeFinalZ = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeFinalY = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeFinalX = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeSaveZ = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeSaveY = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeSaveX = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeTravelZ = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeTravelY = new System.Windows.Forms.NumericUpDown();
            this.nUDProbeTravelX = new System.Windows.Forms.NumericUpDown();
            this.rBCF2 = new System.Windows.Forms.RadioButton();
            this.rBCF1 = new System.Windows.Forms.RadioButton();
            this.btnGetAngleEF = new System.Windows.Forms.Button();
            this.tBAngle = new System.Windows.Forms.TextBox();
            this.cBSetCoordTL = new System.Windows.Forms.CheckBox();
            this.btnSaveTL = new System.Windows.Forms.Button();
            this.cBSetCenterZero = new System.Windows.Forms.CheckBox();
            this.nUDFindCenterAngle = new System.Windows.Forms.NumericUpDown();
            this.cBFindCenterStartFromCenter = new System.Windows.Forms.CheckBox();
            this.cBFindCenterUseX = new System.Windows.Forms.CheckBox();
            this.cBFindCenterUseY = new System.Windows.Forms.CheckBox();
            this.cBFindCenterInvert = new System.Windows.Forms.CheckBox();
            this.TbSetPoints = new System.Windows.Forms.TextBox();
            this.CbProbeSkipMove = new System.Windows.Forms.CheckBox();
            this.BtnGetFiducialOffset = new System.Windows.Forms.Button();
            this.BtnMoveLeft = new System.Windows.Forms.Button();
            this.BtnMoveRight = new System.Windows.Forms.Button();
            this.CbProbe2ndChance = new System.Windows.Forms.CheckBox();
            this.gBMovement = new System.Windows.Forms.GroupBox();
            this.nUDProbeFeedZ = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.nUDProbeFeedXY = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label20 = new System.Windows.Forms.Label();
            this.lblEFStatus = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblEFProgressInfo = new System.Windows.Forms.Label();
            this.BtnStartEF = new System.Windows.Forms.Button();
            this.lblEFProgress = new System.Windows.Forms.Label();
            this.rB9 = new System.Windows.Forms.RadioButton();
            this.progressBarEF = new System.Windows.Forms.ProgressBar();
            this.btnCancelEF = new System.Windows.Forms.Button();
            this.rB8 = new System.Windows.Forms.RadioButton();
            this.rB7 = new System.Windows.Forms.RadioButton();
            this.rB6 = new System.Windows.Forms.RadioButton();
            this.rB5 = new System.Windows.Forms.RadioButton();
            this.rB4 = new System.Windows.Forms.RadioButton();
            this.rB3 = new System.Windows.Forms.RadioButton();
            this.rB2 = new System.Windows.Forms.RadioButton();
            this.rB1 = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblCFAngle = new System.Windows.Forms.Label();
            this.lblCFStatus = new System.Windows.Forms.Label();
            this.lblCFProgressInfo = new System.Windows.Forms.Label();
            this.BtnStartCF = new System.Windows.Forms.Button();
            this.lblCFProgress = new System.Windows.Forms.Label();
            this.progressBarCF = new System.Windows.Forms.ProgressBar();
            this.BtnCancelCF = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnClearTL = new System.Windows.Forms.Button();
            this.pBTL = new System.Windows.Forms.PictureBox();
            this.lblTLStatus = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblTLProgressInfo = new System.Windows.Forms.Label();
            this.BtnStartTL = new System.Windows.Forms.Button();
            this.lblTLProgress = new System.Windows.Forms.Label();
            this.progressBarTL = new System.Windows.Forms.ProgressBar();
            this.btnCancelTL = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.GbFiducialOffset = new System.Windows.Forms.GroupBox();
            this.NudProbeFiducialOffsetX = new System.Windows.Forms.NumericUpDown();
            this.NudProbeFiducialOffsetY = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.CbProbeScale = new System.Windows.Forms.CheckBox();
            this.BtnStartFiducial = new System.Windows.Forms.Button();
            this.BtnCancelFiducial = new System.Windows.Forms.Button();
            this.LblFiducial = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gBCoordinates = new System.Windows.Forms.GroupBox();
            this.btnProbeCoordClear = new System.Windows.Forms.Button();
            this.rBProbeCoord2 = new System.Windows.Forms.RadioButton();
            this.rBProbeCoord1 = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.timerFlowControl = new System.Windows.Forms.Timer(this.components);
            this.BtnHelp_ImportParameter = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.gBHardware.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeDiameter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWorkpieceDiameter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFinalZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFinalY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFinalX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSaveZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSaveY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSaveX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeTravelZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeTravelY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeTravelX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFindCenterAngle)).BeginInit();
            this.gBMovement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFeedXY)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBTL)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.GbFiducialOffset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudProbeFiducialOffsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudProbeFiducialOffsetY)).BeginInit();
            this.gBCoordinates.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // cBZProbing
            // 
            resources.ApplyResources(this.cBZProbing, "cBZProbing");
            this.cBZProbing.Checked = global::GrblPlotter.Properties.Settings.Default.probingEdgeZ;
            this.cBZProbing.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "probingEdgeZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBZProbing.Name = "cBZProbing";
            this.toolTip1.SetToolTip(this.cBZProbing, resources.GetString("cBZProbing.ToolTip"));
            this.cBZProbing.UseVisualStyleBackColor = true;
            this.cBZProbing.CheckedChanged += new System.EventHandler(this.CbZProbing_CheckedChanged);
            // 
            // gBHardware
            // 
            this.gBHardware.Controls.Add(this.LblProbeContact);
            this.gBHardware.Controls.Add(this.label7);
            this.gBHardware.Controls.Add(this.label6);
            this.gBHardware.Controls.Add(this.label5);
            this.gBHardware.Controls.Add(this.nUDProbeDiameter);
            this.gBHardware.Controls.Add(this.nUDOffsetX);
            this.gBHardware.Controls.Add(this.nUDOffsetY);
            this.gBHardware.Controls.Add(this.nUDOffsetZ);
            this.gBHardware.Controls.Add(this.label4);
            this.gBHardware.Controls.Add(this.label1);
            resources.ApplyResources(this.gBHardware, "gBHardware");
            this.gBHardware.Name = "gBHardware";
            this.gBHardware.TabStop = false;
            this.gBHardware.Enter += new System.EventHandler(this.ControlProbing_Click);
            // 
            // LblProbeContact
            // 
            resources.ApplyResources(this.LblProbeContact, "LblProbeContact");
            this.LblProbeContact.BackColor = System.Drawing.SystemColors.Control;
            this.LblProbeContact.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblProbeContact.Name = "LblProbeContact";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
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
            // nUDProbeDiameter
            // 
            this.nUDProbeDiameter.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingToolDiameter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeDiameter.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDProbeDiameter, "nUDProbeDiameter");
            this.nUDProbeDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDProbeDiameter.Name = "nUDProbeDiameter";
            this.toolTip1.SetToolTip(this.nUDProbeDiameter, resources.GetString("nUDProbeDiameter.ToolTip"));
            this.nUDProbeDiameter.Value = global::GrblPlotter.Properties.Settings.Default.probingToolDiameter;
            // 
            // nUDOffsetX
            // 
            this.nUDOffsetX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingOffsetX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOffsetX.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDOffsetX, "nUDOffsetX");
            this.nUDOffsetX.Name = "nUDOffsetX";
            this.toolTip1.SetToolTip(this.nUDOffsetX, resources.GetString("nUDOffsetX.ToolTip"));
            this.nUDOffsetX.Value = global::GrblPlotter.Properties.Settings.Default.probingOffsetX;
            this.nUDOffsetX.ValueChanged += new System.EventHandler(this.NudOffset_ValueChanged);
            // 
            // nUDOffsetY
            // 
            this.nUDOffsetY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingOffsetY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOffsetY.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDOffsetY, "nUDOffsetY");
            this.nUDOffsetY.Name = "nUDOffsetY";
            this.toolTip1.SetToolTip(this.nUDOffsetY, resources.GetString("nUDOffsetY.ToolTip"));
            this.nUDOffsetY.Value = global::GrblPlotter.Properties.Settings.Default.probingOffsetY;
            this.nUDOffsetY.ValueChanged += new System.EventHandler(this.NudOffset_ValueChanged);
            // 
            // nUDOffsetZ
            // 
            this.nUDOffsetZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingOffsetZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOffsetZ.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDOffsetZ, "nUDOffsetZ");
            this.nUDOffsetZ.Name = "nUDOffsetZ";
            this.toolTip1.SetToolTip(this.nUDOffsetZ, resources.GetString("nUDOffsetZ.ToolTip"));
            this.nUDOffsetZ.Value = global::GrblPlotter.Properties.Settings.Default.probingOffsetZ;
            this.nUDOffsetZ.ValueChanged += new System.EventHandler(this.NudOffset_ValueChanged);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            this.toolTip1.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
            // 
            // lblProbeFinal
            // 
            resources.ApplyResources(this.lblProbeFinal, "lblProbeFinal");
            this.lblProbeFinal.Name = "lblProbeFinal";
            this.toolTip1.SetToolTip(this.lblProbeFinal, resources.GetString("lblProbeFinal.ToolTip"));
            // 
            // nUDWorkpieceDiameter
            // 
            this.nUDWorkpieceDiameter.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingWorkpieceDiameter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDWorkpieceDiameter.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDWorkpieceDiameter, "nUDWorkpieceDiameter");
            this.nUDWorkpieceDiameter.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDWorkpieceDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDWorkpieceDiameter.Name = "nUDWorkpieceDiameter";
            this.toolTip1.SetToolTip(this.nUDWorkpieceDiameter, resources.GetString("nUDWorkpieceDiameter.ToolTip"));
            this.nUDWorkpieceDiameter.Value = global::GrblPlotter.Properties.Settings.Default.probingWorkpieceDiameter;
            // 
            // nUDProbeFinalZ
            // 
            this.nUDProbeFinalZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingFinalZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeFinalZ.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeFinalZ, "nUDProbeFinalZ");
            this.nUDProbeFinalZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeFinalZ.Name = "nUDProbeFinalZ";
            this.toolTip1.SetToolTip(this.nUDProbeFinalZ, resources.GetString("nUDProbeFinalZ.ToolTip"));
            this.nUDProbeFinalZ.Value = global::GrblPlotter.Properties.Settings.Default.probingFinalZ;
            // 
            // nUDProbeFinalY
            // 
            this.nUDProbeFinalY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingFinalY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeFinalY.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeFinalY, "nUDProbeFinalY");
            this.nUDProbeFinalY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeFinalY.Name = "nUDProbeFinalY";
            this.toolTip1.SetToolTip(this.nUDProbeFinalY, resources.GetString("nUDProbeFinalY.ToolTip"));
            this.nUDProbeFinalY.Value = global::GrblPlotter.Properties.Settings.Default.probingFinalY;
            // 
            // nUDProbeFinalX
            // 
            this.nUDProbeFinalX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingFinalX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeFinalX.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeFinalX, "nUDProbeFinalX");
            this.nUDProbeFinalX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeFinalX.Name = "nUDProbeFinalX";
            this.toolTip1.SetToolTip(this.nUDProbeFinalX, resources.GetString("nUDProbeFinalX.ToolTip"));
            this.nUDProbeFinalX.Value = global::GrblPlotter.Properties.Settings.Default.probingFinalX;
            // 
            // nUDProbeSaveZ
            // 
            this.nUDProbeSaveZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingSaveZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeSaveZ.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeSaveZ, "nUDProbeSaveZ");
            this.nUDProbeSaveZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeSaveZ.Name = "nUDProbeSaveZ";
            this.toolTip1.SetToolTip(this.nUDProbeSaveZ, resources.GetString("nUDProbeSaveZ.ToolTip"));
            this.nUDProbeSaveZ.Value = global::GrblPlotter.Properties.Settings.Default.probingSaveZ;
            this.nUDProbeSaveZ.ValueChanged += new System.EventHandler(this.NudProbeSave_ValueChanged);
            // 
            // nUDProbeSaveY
            // 
            this.nUDProbeSaveY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingSaveY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeSaveY.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeSaveY, "nUDProbeSaveY");
            this.nUDProbeSaveY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeSaveY.Name = "nUDProbeSaveY";
            this.toolTip1.SetToolTip(this.nUDProbeSaveY, resources.GetString("nUDProbeSaveY.ToolTip"));
            this.nUDProbeSaveY.Value = global::GrblPlotter.Properties.Settings.Default.probingSaveY;
            this.nUDProbeSaveY.ValueChanged += new System.EventHandler(this.NudProbeSave_ValueChanged);
            // 
            // nUDProbeSaveX
            // 
            this.nUDProbeSaveX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingSaveX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeSaveX.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeSaveX, "nUDProbeSaveX");
            this.nUDProbeSaveX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeSaveX.Name = "nUDProbeSaveX";
            this.toolTip1.SetToolTip(this.nUDProbeSaveX, resources.GetString("nUDProbeSaveX.ToolTip"));
            this.nUDProbeSaveX.Value = global::GrblPlotter.Properties.Settings.Default.probingSaveX;
            this.nUDProbeSaveX.ValueChanged += new System.EventHandler(this.NudProbeSave_ValueChanged);
            // 
            // nUDProbeTravelZ
            // 
            this.nUDProbeTravelZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingTravelZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeTravelZ.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeTravelZ, "nUDProbeTravelZ");
            this.nUDProbeTravelZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeTravelZ.Name = "nUDProbeTravelZ";
            this.toolTip1.SetToolTip(this.nUDProbeTravelZ, resources.GetString("nUDProbeTravelZ.ToolTip"));
            this.nUDProbeTravelZ.Value = global::GrblPlotter.Properties.Settings.Default.probingTravelZ;
            // 
            // nUDProbeTravelY
            // 
            this.nUDProbeTravelY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingTravelY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeTravelY.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeTravelY, "nUDProbeTravelY");
            this.nUDProbeTravelY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeTravelY.Name = "nUDProbeTravelY";
            this.toolTip1.SetToolTip(this.nUDProbeTravelY, resources.GetString("nUDProbeTravelY.ToolTip"));
            this.nUDProbeTravelY.Value = global::GrblPlotter.Properties.Settings.Default.probingTravelY;
            // 
            // nUDProbeTravelX
            // 
            this.nUDProbeTravelX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingTravelX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeTravelX.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDProbeTravelX, "nUDProbeTravelX");
            this.nUDProbeTravelX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDProbeTravelX.Name = "nUDProbeTravelX";
            this.toolTip1.SetToolTip(this.nUDProbeTravelX, resources.GetString("nUDProbeTravelX.ToolTip"));
            this.nUDProbeTravelX.Value = global::GrblPlotter.Properties.Settings.Default.probingTravelX;
            // 
            // rBCF2
            // 
            resources.ApplyResources(this.rBCF2, "rBCF2");
            this.rBCF2.Name = "rBCF2";
            this.toolTip1.SetToolTip(this.rBCF2, resources.GetString("rBCF2.ToolTip"));
            this.rBCF2.UseVisualStyleBackColor = true;
            this.rBCF2.CheckedChanged += new System.EventHandler(this.RbCF_CheckedCHanged);
            // 
            // rBCF1
            // 
            resources.ApplyResources(this.rBCF1, "rBCF1");
            this.rBCF1.Name = "rBCF1";
            this.toolTip1.SetToolTip(this.rBCF1, resources.GetString("rBCF1.ToolTip"));
            this.rBCF1.UseVisualStyleBackColor = true;
            this.rBCF1.CheckedChanged += new System.EventHandler(this.RbCF_CheckedCHanged);
            // 
            // btnGetAngleEF
            // 
            resources.ApplyResources(this.btnGetAngleEF, "btnGetAngleEF");
            this.btnGetAngleEF.Name = "btnGetAngleEF";
            this.toolTip1.SetToolTip(this.btnGetAngleEF, resources.GetString("btnGetAngleEF.ToolTip"));
            this.btnGetAngleEF.UseVisualStyleBackColor = true;
            this.btnGetAngleEF.Click += new System.EventHandler(this.BtnGetAngleEFClick);
            // 
            // tBAngle
            // 
            resources.ApplyResources(this.tBAngle, "tBAngle");
            this.tBAngle.Name = "tBAngle";
            this.toolTip1.SetToolTip(this.tBAngle, resources.GetString("tBAngle.ToolTip"));
            // 
            // cBSetCoordTL
            // 
            this.cBSetCoordTL.Checked = true;
            this.cBSetCoordTL.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.cBSetCoordTL, "cBSetCoordTL");
            this.cBSetCoordTL.Name = "cBSetCoordTL";
            this.toolTip1.SetToolTip(this.cBSetCoordTL, resources.GetString("cBSetCoordTL.ToolTip"));
            this.cBSetCoordTL.UseVisualStyleBackColor = true;
            // 
            // btnSaveTL
            // 
            resources.ApplyResources(this.btnSaveTL, "btnSaveTL");
            this.btnSaveTL.Name = "btnSaveTL";
            this.toolTip1.SetToolTip(this.btnSaveTL, resources.GetString("btnSaveTL.ToolTip"));
            this.btnSaveTL.UseVisualStyleBackColor = true;
            this.btnSaveTL.Click += new System.EventHandler(this.BtnSaveTL_Click);
            // 
            // cBSetCenterZero
            // 
            resources.ApplyResources(this.cBSetCenterZero, "cBSetCenterZero");
            this.cBSetCenterZero.Checked = global::GrblPlotter.Properties.Settings.Default.probingEdgeCenter;
            this.cBSetCenterZero.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "probingEdgeCenter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBSetCenterZero.Name = "cBSetCenterZero";
            this.toolTip1.SetToolTip(this.cBSetCenterZero, resources.GetString("cBSetCenterZero.ToolTip"));
            this.cBSetCenterZero.UseVisualStyleBackColor = true;
            // 
            // nUDFindCenterAngle
            // 
            this.nUDFindCenterAngle.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDFindCenterAngle, "nUDFindCenterAngle");
            this.nUDFindCenterAngle.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nUDFindCenterAngle.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.nUDFindCenterAngle.Name = "nUDFindCenterAngle";
            this.toolTip1.SetToolTip(this.nUDFindCenterAngle, resources.GetString("nUDFindCenterAngle.ToolTip"));
            this.nUDFindCenterAngle.ValueChanged += new System.EventHandler(this.NudFindCenterAngle_ValueChanged);
            // 
            // cBFindCenterStartFromCenter
            // 
            resources.ApplyResources(this.cBFindCenterStartFromCenter, "cBFindCenterStartFromCenter");
            this.cBFindCenterStartFromCenter.Name = "cBFindCenterStartFromCenter";
            this.toolTip1.SetToolTip(this.cBFindCenterStartFromCenter, resources.GetString("cBFindCenterStartFromCenter.ToolTip"));
            this.cBFindCenterStartFromCenter.UseVisualStyleBackColor = true;
            // 
            // cBFindCenterUseX
            // 
            resources.ApplyResources(this.cBFindCenterUseX, "cBFindCenterUseX");
            this.cBFindCenterUseX.Checked = true;
            this.cBFindCenterUseX.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBFindCenterUseX.Name = "cBFindCenterUseX";
            this.toolTip1.SetToolTip(this.cBFindCenterUseX, resources.GetString("cBFindCenterUseX.ToolTip"));
            this.cBFindCenterUseX.UseVisualStyleBackColor = true;
            this.cBFindCenterUseX.CheckedChanged += new System.EventHandler(this.CbFindCenterUseX_CheckedChanged);
            // 
            // cBFindCenterUseY
            // 
            resources.ApplyResources(this.cBFindCenterUseY, "cBFindCenterUseY");
            this.cBFindCenterUseY.Checked = true;
            this.cBFindCenterUseY.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBFindCenterUseY.Name = "cBFindCenterUseY";
            this.toolTip1.SetToolTip(this.cBFindCenterUseY, resources.GetString("cBFindCenterUseY.ToolTip"));
            this.cBFindCenterUseY.UseVisualStyleBackColor = true;
            this.cBFindCenterUseY.CheckedChanged += new System.EventHandler(this.CbFindCenterUseY_CheckedChanged);
            // 
            // cBFindCenterInvert
            // 
            resources.ApplyResources(this.cBFindCenterInvert, "cBFindCenterInvert");
            this.cBFindCenterInvert.Checked = global::GrblPlotter.Properties.Settings.Default.probingInvertLogic;
            this.cBFindCenterInvert.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "probingInvertLogic", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBFindCenterInvert.Name = "cBFindCenterInvert";
            this.toolTip1.SetToolTip(this.cBFindCenterInvert, resources.GetString("cBFindCenterInvert.ToolTip"));
            this.cBFindCenterInvert.UseVisualStyleBackColor = true;
            // 
            // TbSetPoints
            // 
            resources.ApplyResources(this.TbSetPoints, "TbSetPoints");
            this.TbSetPoints.Name = "TbSetPoints";
            this.toolTip1.SetToolTip(this.TbSetPoints, resources.GetString("TbSetPoints.ToolTip"));
            // 
            // CbProbeSkipMove
            // 
            resources.ApplyResources(this.CbProbeSkipMove, "CbProbeSkipMove");
            this.CbProbeSkipMove.Checked = global::GrblPlotter.Properties.Settings.Default.probingFiducialSkip1stMove;
            this.CbProbeSkipMove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbProbeSkipMove.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "probingFiducialSkip1stMove", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbProbeSkipMove.Name = "CbProbeSkipMove";
            this.toolTip1.SetToolTip(this.CbProbeSkipMove, resources.GetString("CbProbeSkipMove.ToolTip"));
            this.CbProbeSkipMove.UseVisualStyleBackColor = true;
            // 
            // BtnGetFiducialOffset
            // 
            resources.ApplyResources(this.BtnGetFiducialOffset, "BtnGetFiducialOffset");
            this.BtnGetFiducialOffset.Name = "BtnGetFiducialOffset";
            this.toolTip1.SetToolTip(this.BtnGetFiducialOffset, resources.GetString("BtnGetFiducialOffset.ToolTip"));
            this.BtnGetFiducialOffset.UseVisualStyleBackColor = true;
            this.BtnGetFiducialOffset.Click += new System.EventHandler(this.BtnGetFiducialOffset_Click);
            // 
            // BtnMoveLeft
            // 
            resources.ApplyResources(this.BtnMoveLeft, "BtnMoveLeft");
            this.BtnMoveLeft.Name = "BtnMoveLeft";
            this.toolTip1.SetToolTip(this.BtnMoveLeft, resources.GetString("BtnMoveLeft.ToolTip"));
            this.BtnMoveLeft.UseVisualStyleBackColor = true;
            this.BtnMoveLeft.Click += new System.EventHandler(this.BtnMoveLeft_Click);
            // 
            // BtnMoveRight
            // 
            resources.ApplyResources(this.BtnMoveRight, "BtnMoveRight");
            this.BtnMoveRight.Name = "BtnMoveRight";
            this.toolTip1.SetToolTip(this.BtnMoveRight, resources.GetString("BtnMoveRight.ToolTip"));
            this.BtnMoveRight.UseVisualStyleBackColor = true;
            this.BtnMoveRight.Click += new System.EventHandler(this.BtnMoveRight_Click);
            // 
            // CbProbe2ndChance
            // 
            resources.ApplyResources(this.CbProbe2ndChance, "CbProbe2ndChance");
            this.CbProbe2ndChance.Checked = global::GrblPlotter.Properties.Settings.Default.probingFiducial2ndChance;
            this.CbProbe2ndChance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbProbe2ndChance.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "probingFiducial2ndChance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbProbe2ndChance.Name = "CbProbe2ndChance";
            this.toolTip1.SetToolTip(this.CbProbe2ndChance, resources.GetString("CbProbe2ndChance.ToolTip"));
            this.CbProbe2ndChance.UseVisualStyleBackColor = true;
            // 
            // gBMovement
            // 
            this.gBMovement.Controls.Add(this.nUDProbeFeedZ);
            this.gBMovement.Controls.Add(this.nUDProbeFinalZ);
            this.gBMovement.Controls.Add(this.nUDProbeFinalY);
            this.gBMovement.Controls.Add(this.nUDProbeFinalX);
            this.gBMovement.Controls.Add(this.nUDProbeSaveZ);
            this.gBMovement.Controls.Add(this.nUDProbeSaveY);
            this.gBMovement.Controls.Add(this.nUDProbeSaveX);
            this.gBMovement.Controls.Add(this.label8);
            this.gBMovement.Controls.Add(this.nUDProbeTravelZ);
            this.gBMovement.Controls.Add(this.label9);
            this.gBMovement.Controls.Add(this.label10);
            this.gBMovement.Controls.Add(this.nUDProbeTravelY);
            this.gBMovement.Controls.Add(this.nUDProbeFeedXY);
            this.gBMovement.Controls.Add(this.nUDProbeTravelX);
            this.gBMovement.Controls.Add(this.label2);
            this.gBMovement.Controls.Add(this.label14);
            this.gBMovement.Controls.Add(this.lblProbeFinal);
            this.gBMovement.Controls.Add(this.label3);
            this.gBMovement.Controls.Add(this.label13);
            this.gBMovement.Controls.Add(this.label15);
            resources.ApplyResources(this.gBMovement, "gBMovement");
            this.gBMovement.Name = "gBMovement";
            this.gBMovement.TabStop = false;
            this.gBMovement.Enter += new System.EventHandler(this.ControlProbing_Click);
            // 
            // nUDProbeFeedZ
            // 
            this.nUDProbeFeedZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingFeedZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeFeedZ.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDProbeFeedZ, "nUDProbeFeedZ");
            this.nUDProbeFeedZ.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDProbeFeedZ.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDProbeFeedZ.Name = "nUDProbeFeedZ";
            this.nUDProbeFeedZ.Value = global::GrblPlotter.Properties.Settings.Default.probingFeedZ;
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
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // nUDProbeFeedXY
            // 
            this.nUDProbeFeedXY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingFeedXY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeFeedXY.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDProbeFeedXY, "nUDProbeFeedXY");
            this.nUDProbeFeedXY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDProbeFeedXY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDProbeFeedXY.Name = "nUDProbeFeedXY";
            this.nUDProbeFeedXY.Value = global::GrblPlotter.Properties.Settings.Default.probingFeedXY;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControl1_Selected);
            this.tabControl1.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TabControl1_Deselecting);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.BtnHelp_ImportParameter);
            this.tabPage1.Controls.Add(this.cBSetCenterZero);
            this.tabPage1.Controls.Add(this.btnGetAngleEF);
            this.tabPage1.Controls.Add(this.tBAngle);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.lblEFStatus);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.lblEFProgressInfo);
            this.tabPage1.Controls.Add(this.BtnStartEF);
            this.tabPage1.Controls.Add(this.lblEFProgress);
            this.tabPage1.Controls.Add(this.rB9);
            this.tabPage1.Controls.Add(this.progressBarEF);
            this.tabPage1.Controls.Add(this.btnCancelEF);
            this.tabPage1.Controls.Add(this.rB8);
            this.tabPage1.Controls.Add(this.rB7);
            this.tabPage1.Controls.Add(this.rB6);
            this.tabPage1.Controls.Add(this.rB5);
            this.tabPage1.Controls.Add(this.rB4);
            this.tabPage1.Controls.Add(this.rB3);
            this.tabPage1.Controls.Add(this.rB2);
            this.tabPage1.Controls.Add(this.rB1);
            this.tabPage1.Controls.Add(this.cBZProbing);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // lblEFStatus
            // 
            resources.ApplyResources(this.lblEFStatus, "lblEFStatus");
            this.lblEFStatus.Name = "lblEFStatus";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // lblEFProgressInfo
            // 
            resources.ApplyResources(this.lblEFProgressInfo, "lblEFProgressInfo");
            this.lblEFProgressInfo.Name = "lblEFProgressInfo";
            // 
            // BtnStartEF
            // 
            resources.ApplyResources(this.BtnStartEF, "BtnStartEF");
            this.BtnStartEF.Name = "BtnStartEF";
            this.BtnStartEF.UseVisualStyleBackColor = true;
            this.BtnStartEF.Click += new System.EventHandler(this.BtnStartEF_Click);
            // 
            // lblEFProgress
            // 
            resources.ApplyResources(this.lblEFProgress, "lblEFProgress");
            this.lblEFProgress.Name = "lblEFProgress";
            // 
            // rB9
            // 
            resources.ApplyResources(this.rB9, "rB9");
            this.rB9.Name = "rB9";
            this.rB9.TabStop = true;
            this.rB9.UseVisualStyleBackColor = true;
            this.rB9.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // progressBarEF
            // 
            resources.ApplyResources(this.progressBarEF, "progressBarEF");
            this.progressBarEF.Name = "progressBarEF";
            // 
            // btnCancelEF
            // 
            resources.ApplyResources(this.btnCancelEF, "btnCancelEF");
            this.btnCancelEF.Name = "btnCancelEF";
            this.btnCancelEF.UseVisualStyleBackColor = true;
            this.btnCancelEF.Click += new System.EventHandler(this.BtnCancelEF_Click);
            // 
            // rB8
            // 
            resources.ApplyResources(this.rB8, "rB8");
            this.rB8.Name = "rB8";
            this.rB8.TabStop = true;
            this.rB8.UseVisualStyleBackColor = true;
            this.rB8.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // rB7
            // 
            resources.ApplyResources(this.rB7, "rB7");
            this.rB7.Name = "rB7";
            this.rB7.TabStop = true;
            this.rB7.UseVisualStyleBackColor = true;
            this.rB7.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // rB6
            // 
            resources.ApplyResources(this.rB6, "rB6");
            this.rB6.Name = "rB6";
            this.rB6.TabStop = true;
            this.rB6.UseVisualStyleBackColor = true;
            this.rB6.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // rB5
            // 
            resources.ApplyResources(this.rB5, "rB5");
            this.rB5.Name = "rB5";
            this.rB5.TabStop = true;
            this.rB5.UseVisualStyleBackColor = true;
            this.rB5.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // rB4
            // 
            resources.ApplyResources(this.rB4, "rB4");
            this.rB4.Name = "rB4";
            this.rB4.TabStop = true;
            this.rB4.UseVisualStyleBackColor = true;
            this.rB4.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // rB3
            // 
            resources.ApplyResources(this.rB3, "rB3");
            this.rB3.Name = "rB3";
            this.rB3.TabStop = true;
            this.rB3.UseVisualStyleBackColor = true;
            this.rB3.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // rB2
            // 
            resources.ApplyResources(this.rB2, "rB2");
            this.rB2.Name = "rB2";
            this.rB2.TabStop = true;
            this.rB2.UseVisualStyleBackColor = true;
            this.rB2.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // rB1
            // 
            resources.ApplyResources(this.rB1, "rB1");
            this.rB1.Name = "rB1";
            this.rB1.TabStop = true;
            this.rB1.UseVisualStyleBackColor = true;
            this.rB1.CheckedChanged += new System.EventHandler(this.RbEF_CheckedCHanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.cBFindCenterInvert);
            this.tabPage2.Controls.Add(this.cBFindCenterUseY);
            this.tabPage2.Controls.Add(this.cBFindCenterUseX);
            this.tabPage2.Controls.Add(this.cBFindCenterStartFromCenter);
            this.tabPage2.Controls.Add(this.lblCFAngle);
            this.tabPage2.Controls.Add(this.nUDFindCenterAngle);
            this.tabPage2.Controls.Add(this.nUDWorkpieceDiameter);
            this.tabPage2.Controls.Add(this.lblCFStatus);
            this.tabPage2.Controls.Add(this.lblCFProgressInfo);
            this.tabPage2.Controls.Add(this.BtnStartCF);
            this.tabPage2.Controls.Add(this.lblCFProgress);
            this.tabPage2.Controls.Add(this.progressBarCF);
            this.tabPage2.Controls.Add(this.BtnCancelCF);
            this.tabPage2.Controls.Add(this.rBCF2);
            this.tabPage2.Controls.Add(this.rBCF1);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.label21);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lblCFAngle
            // 
            resources.ApplyResources(this.lblCFAngle, "lblCFAngle");
            this.lblCFAngle.Name = "lblCFAngle";
            // 
            // lblCFStatus
            // 
            resources.ApplyResources(this.lblCFStatus, "lblCFStatus");
            this.lblCFStatus.Name = "lblCFStatus";
            // 
            // lblCFProgressInfo
            // 
            resources.ApplyResources(this.lblCFProgressInfo, "lblCFProgressInfo");
            this.lblCFProgressInfo.Name = "lblCFProgressInfo";
            // 
            // BtnStartCF
            // 
            resources.ApplyResources(this.BtnStartCF, "BtnStartCF");
            this.BtnStartCF.Name = "BtnStartCF";
            this.BtnStartCF.UseVisualStyleBackColor = true;
            this.BtnStartCF.Click += new System.EventHandler(this.BtnStartCF_Click);
            // 
            // lblCFProgress
            // 
            resources.ApplyResources(this.lblCFProgress, "lblCFProgress");
            this.lblCFProgress.Name = "lblCFProgress";
            // 
            // progressBarCF
            // 
            resources.ApplyResources(this.progressBarCF, "progressBarCF");
            this.progressBarCF.Name = "progressBarCF";
            // 
            // BtnCancelCF
            // 
            resources.ApplyResources(this.BtnCancelCF, "BtnCancelCF");
            this.BtnCancelCF.Name = "BtnCancelCF";
            this.BtnCancelCF.UseVisualStyleBackColor = true;
            this.BtnCancelCF.Click += new System.EventHandler(this.BtnCancelCF_Click);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button2);
            this.tabPage3.Controls.Add(this.btnSaveTL);
            this.tabPage3.Controls.Add(this.btnClearTL);
            this.tabPage3.Controls.Add(this.pBTL);
            this.tabPage3.Controls.Add(this.lblTLStatus);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.lblTLProgressInfo);
            this.tabPage3.Controls.Add(this.BtnStartTL);
            this.tabPage3.Controls.Add(this.lblTLProgress);
            this.tabPage3.Controls.Add(this.progressBarTL);
            this.tabPage3.Controls.Add(this.btnCancelTL);
            this.tabPage3.Controls.Add(this.cBSetCoordTL);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnClearTL
            // 
            resources.ApplyResources(this.btnClearTL, "btnClearTL");
            this.btnClearTL.Name = "btnClearTL";
            this.btnClearTL.UseVisualStyleBackColor = true;
            this.btnClearTL.Click += new System.EventHandler(this.BtnClearTL_Click);
            // 
            // pBTL
            // 
            resources.ApplyResources(this.pBTL, "pBTL");
            this.pBTL.Name = "pBTL";
            this.pBTL.TabStop = false;
            // 
            // lblTLStatus
            // 
            resources.ApplyResources(this.lblTLStatus, "lblTLStatus");
            this.lblTLStatus.Name = "lblTLStatus";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // lblTLProgressInfo
            // 
            resources.ApplyResources(this.lblTLProgressInfo, "lblTLProgressInfo");
            this.lblTLProgressInfo.Name = "lblTLProgressInfo";
            // 
            // BtnStartTL
            // 
            resources.ApplyResources(this.BtnStartTL, "BtnStartTL");
            this.BtnStartTL.Name = "BtnStartTL";
            this.BtnStartTL.UseVisualStyleBackColor = true;
            this.BtnStartTL.Click += new System.EventHandler(this.BtnStartTL_Click);
            // 
            // lblTLProgress
            // 
            resources.ApplyResources(this.lblTLProgress, "lblTLProgress");
            this.lblTLProgress.Name = "lblTLProgress";
            // 
            // progressBarTL
            // 
            resources.ApplyResources(this.progressBarTL, "progressBarTL");
            this.progressBarTL.Name = "progressBarTL";
            // 
            // btnCancelTL
            // 
            resources.ApplyResources(this.btnCancelTL, "btnCancelTL");
            this.btnCancelTL.Name = "btnCancelTL";
            this.btnCancelTL.UseVisualStyleBackColor = true;
            this.btnCancelTL.Click += new System.EventHandler(this.BtnCancelTL_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.TbSetPoints);
            this.tabPage4.Controls.Add(this.CbProbe2ndChance);
            this.tabPage4.Controls.Add(this.GbFiducialOffset);
            this.tabPage4.Controls.Add(this.CbProbeSkipMove);
            this.tabPage4.Controls.Add(this.CbProbeScale);
            this.tabPage4.Controls.Add(this.BtnStartFiducial);
            this.tabPage4.Controls.Add(this.BtnCancelFiducial);
            this.tabPage4.Controls.Add(this.LblFiducial);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // GbFiducialOffset
            // 
            this.GbFiducialOffset.Controls.Add(this.BtnMoveRight);
            this.GbFiducialOffset.Controls.Add(this.BtnMoveLeft);
            this.GbFiducialOffset.Controls.Add(this.NudProbeFiducialOffsetX);
            this.GbFiducialOffset.Controls.Add(this.NudProbeFiducialOffsetY);
            this.GbFiducialOffset.Controls.Add(this.BtnGetFiducialOffset);
            this.GbFiducialOffset.Controls.Add(this.label16);
            this.GbFiducialOffset.Controls.Add(this.label17);
            resources.ApplyResources(this.GbFiducialOffset, "GbFiducialOffset");
            this.GbFiducialOffset.Name = "GbFiducialOffset";
            this.GbFiducialOffset.TabStop = false;
            // 
            // NudProbeFiducialOffsetX
            // 
            this.NudProbeFiducialOffsetX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingFiducialOffsetX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudProbeFiducialOffsetX.DecimalPlaces = 2;
            this.NudProbeFiducialOffsetX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudProbeFiducialOffsetX, "NudProbeFiducialOffsetX");
            this.NudProbeFiducialOffsetX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NudProbeFiducialOffsetX.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.NudProbeFiducialOffsetX.Name = "NudProbeFiducialOffsetX";
            this.NudProbeFiducialOffsetX.Value = global::GrblPlotter.Properties.Settings.Default.probingFiducialOffsetX;
            // 
            // NudProbeFiducialOffsetY
            // 
            this.NudProbeFiducialOffsetY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "probingFiducialOffsetY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudProbeFiducialOffsetY.DecimalPlaces = 2;
            this.NudProbeFiducialOffsetY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudProbeFiducialOffsetY, "NudProbeFiducialOffsetY");
            this.NudProbeFiducialOffsetY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NudProbeFiducialOffsetY.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.NudProbeFiducialOffsetY.Name = "NudProbeFiducialOffsetY";
            this.NudProbeFiducialOffsetY.Value = global::GrblPlotter.Properties.Settings.Default.probingFiducialOffsetY;
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // CbProbeScale
            // 
            resources.ApplyResources(this.CbProbeScale, "CbProbeScale");
            this.CbProbeScale.Checked = global::GrblPlotter.Properties.Settings.Default.cameraScaleOnRotate;
            this.CbProbeScale.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "cameraScaleOnRotate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbProbeScale.Name = "CbProbeScale";
            this.CbProbeScale.UseVisualStyleBackColor = true;
            // 
            // BtnStartFiducial
            // 
            resources.ApplyResources(this.BtnStartFiducial, "BtnStartFiducial");
            this.BtnStartFiducial.Name = "BtnStartFiducial";
            this.BtnStartFiducial.UseVisualStyleBackColor = true;
            this.BtnStartFiducial.Click += new System.EventHandler(this.BtnStartFiducial_Click);
            // 
            // BtnCancelFiducial
            // 
            resources.ApplyResources(this.BtnCancelFiducial, "BtnCancelFiducial");
            this.BtnCancelFiducial.Name = "BtnCancelFiducial";
            this.BtnCancelFiducial.UseVisualStyleBackColor = true;
            this.BtnCancelFiducial.Click += new System.EventHandler(this.BtnCancelFiducial_Click);
            // 
            // LblFiducial
            // 
            resources.ApplyResources(this.LblFiducial, "LblFiducial");
            this.LblFiducial.Name = "LblFiducial";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // gBCoordinates
            // 
            this.gBCoordinates.Controls.Add(this.btnProbeCoordClear);
            this.gBCoordinates.Controls.Add(this.rBProbeCoord2);
            this.gBCoordinates.Controls.Add(this.rBProbeCoord1);
            this.gBCoordinates.Controls.Add(this.label11);
            resources.ApplyResources(this.gBCoordinates, "gBCoordinates");
            this.gBCoordinates.Name = "gBCoordinates";
            this.gBCoordinates.TabStop = false;
            // 
            // btnProbeCoordClear
            // 
            resources.ApplyResources(this.btnProbeCoordClear, "btnProbeCoordClear");
            this.btnProbeCoordClear.Name = "btnProbeCoordClear";
            this.btnProbeCoordClear.UseVisualStyleBackColor = true;
            this.btnProbeCoordClear.Click += new System.EventHandler(this.BtnProbeCoordClear_Click);
            // 
            // rBProbeCoord2
            // 
            resources.ApplyResources(this.rBProbeCoord2, "rBProbeCoord2");
            this.rBProbeCoord2.Name = "rBProbeCoord2";
            this.rBProbeCoord2.UseVisualStyleBackColor = true;
            this.rBProbeCoord2.CheckedChanged += new System.EventHandler(this.RbProbeCoord1_CheckedChanged);
            // 
            // rBProbeCoord1
            // 
            resources.ApplyResources(this.rBProbeCoord1, "rBProbeCoord1");
            this.rBProbeCoord1.Checked = global::GrblPlotter.Properties.Settings.Default.probingCoordG10;
            this.rBProbeCoord1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "probingCoordG10", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rBProbeCoord1.Name = "rBProbeCoord1";
            this.rBProbeCoord1.TabStop = true;
            this.rBProbeCoord1.UseVisualStyleBackColor = true;
            this.rBProbeCoord1.CheckedChanged += new System.EventHandler(this.RbProbeCoord1_CheckedChanged);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // timerFlowControl
            // 
            this.timerFlowControl.Interval = 500;
            this.timerFlowControl.Tick += new System.EventHandler(this.TimerFlowControl_Tick);
            // 
            // BtnHelp_ImportParameter
            // 
            this.BtnHelp_ImportParameter.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.BtnHelp_ImportParameter, "BtnHelp_ImportParameter");
            this.BtnHelp_ImportParameter.Name = "BtnHelp_ImportParameter";
            this.BtnHelp_ImportParameter.Tag = "id=form-probe#edgefinder";
            this.toolTip1.SetToolTip(this.BtnHelp_ImportParameter, resources.GetString("BtnHelp_ImportParameter.ToolTip"));
            this.BtnHelp_ImportParameter.UseVisualStyleBackColor = false;
            this.BtnHelp_ImportParameter.Click += new System.EventHandler(this.BtnHelp_ImportParameter_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.Tag = "id=form-probe#centerfinder";
            this.toolTip1.SetToolTip(this.button1, resources.GetString("button1.ToolTip"));
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.BtnHelp_ImportParameter_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.Tag = "id=form-probe#toollength";
            this.toolTip1.SetToolTip(this.button2, resources.GetString("button2.ToolTip"));
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.BtnHelp_ImportParameter_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.Tag = "id=form-probe#fiducial";
            this.toolTip1.SetToolTip(this.button3, resources.GetString("button3.ToolTip"));
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.BtnHelp_ImportParameter_Click);
            // 
            // ControlProbing
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gBCoordinates);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.gBMovement);
            this.Controls.Add(this.gBHardware);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::GrblPlotter.Properties.Settings.Default, "locationProbingForm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::GrblPlotter.Properties.Settings.Default.locationProbingForm;
            this.Name = "ControlProbing";
            this.Load += new System.EventHandler(this.ControlProbing_Load);
            this.Click += new System.EventHandler(this.ControlProbing_Click);
            this.gBHardware.ResumeLayout(false);
            this.gBHardware.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeDiameter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDWorkpieceDiameter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFinalZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFinalY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFinalX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSaveZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSaveY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSaveX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeTravelZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeTravelY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeTravelX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFindCenterAngle)).EndInit();
            this.gBMovement.ResumeLayout(false);
            this.gBMovement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFeedZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeFeedXY)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBTL)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.GbFiducialOffset.ResumeLayout(false);
            this.GbFiducialOffset.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudProbeFiducialOffsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudProbeFiducialOffsetY)).EndInit();
            this.gBCoordinates.ResumeLayout(false);
            this.gBCoordinates.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown nUDProbeDiameter;
        private System.Windows.Forms.NumericUpDown nUDProbeTravelX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUDProbeFeedXY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUDOffsetZ;
        private System.Windows.Forms.NumericUpDown nUDOffsetY;
        private System.Windows.Forms.NumericUpDown nUDOffsetX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cBZProbing;
        private System.Windows.Forms.GroupBox gBHardware;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox gBMovement;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nUDProbeTravelZ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nUDProbeTravelY;
        private System.Windows.Forms.NumericUpDown nUDProbeFinalZ;
        private System.Windows.Forms.NumericUpDown nUDProbeFinalY;
        private System.Windows.Forms.Label lblProbeFinal;
        private System.Windows.Forms.NumericUpDown nUDProbeFinalX;
        private System.Windows.Forms.NumericUpDown nUDProbeSaveZ;
        private System.Windows.Forms.NumericUpDown nUDProbeSaveY;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nUDProbeSaveX;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button BtnStartEF;
        private System.Windows.Forms.RadioButton rB9;
        private System.Windows.Forms.RadioButton rB8;
        private System.Windows.Forms.RadioButton rB7;
        private System.Windows.Forms.RadioButton rB6;
        private System.Windows.Forms.RadioButton rB5;
        private System.Windows.Forms.RadioButton rB4;
        private System.Windows.Forms.RadioButton rB3;
        private System.Windows.Forms.RadioButton rB2;
        private System.Windows.Forms.RadioButton rB1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ProgressBar progressBarEF;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblEFProgress;
        private System.Windows.Forms.Label lblEFProgressInfo;
        private System.Windows.Forms.Button btnCancelEF;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RadioButton rBCF2;
        private System.Windows.Forms.RadioButton rBCF1;
        private System.Windows.Forms.Label lblEFStatus;
        private System.Windows.Forms.Label lblCFStatus;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblCFProgressInfo;
        private System.Windows.Forms.Button BtnStartCF;
        private System.Windows.Forms.Label lblCFProgress;
        private System.Windows.Forms.ProgressBar progressBarCF;
        private System.Windows.Forms.Button BtnCancelCF;
        private System.Windows.Forms.TextBox tBAngle;
        private System.Windows.Forms.Label label20;
        internal System.Windows.Forms.Button btnGetAngleEF;
        private System.Windows.Forms.NumericUpDown nUDWorkpieceDiameter;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblTLStatus;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblTLProgressInfo;
        private System.Windows.Forms.Button BtnStartTL;
        private System.Windows.Forms.Label lblTLProgress;
        private System.Windows.Forms.ProgressBar progressBarTL;
        private System.Windows.Forms.Button btnCancelTL;
        private System.Windows.Forms.PictureBox pBTL;
        private System.Windows.Forms.Button btnClearTL;
        private System.Windows.Forms.GroupBox gBCoordinates;
        private System.Windows.Forms.RadioButton rBProbeCoord1;
        private System.Windows.Forms.RadioButton rBProbeCoord2;
        private System.Windows.Forms.Button btnProbeCoordClear;
        private System.Windows.Forms.CheckBox cBSetCoordTL;
        private System.Windows.Forms.Button btnSaveTL;
        private System.Windows.Forms.CheckBox cBSetCenterZero;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblCFAngle;
        private System.Windows.Forms.NumericUpDown nUDFindCenterAngle;
        private System.Windows.Forms.CheckBox cBFindCenterStartFromCenter;
        private System.Windows.Forms.NumericUpDown nUDProbeFeedZ;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox cBFindCenterUseY;
        private System.Windows.Forms.CheckBox cBFindCenterUseX;
        private System.Windows.Forms.CheckBox cBFindCenterInvert;
        private System.Windows.Forms.Label LblProbeContact;
        private System.Windows.Forms.Timer timerFlowControl;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox TbSetPoints;
        private System.Windows.Forms.Label LblFiducial;
        private System.Windows.Forms.Button BtnStartFiducial;
        private System.Windows.Forms.Button BtnCancelFiducial;
        private System.Windows.Forms.CheckBox CbProbeScale;
        private System.Windows.Forms.CheckBox CbProbeSkipMove;
        private System.Windows.Forms.NumericUpDown NudProbeFiducialOffsetY;
        private System.Windows.Forms.NumericUpDown NudProbeFiducialOffsetX;
        private System.Windows.Forms.Button BtnGetFiducialOffset;
        private System.Windows.Forms.GroupBox GbFiducialOffset;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button BtnMoveRight;
        private System.Windows.Forms.Button BtnMoveLeft;
        private System.Windows.Forms.CheckBox CbProbe2ndChance;
        private System.Windows.Forms.Button BtnHelp_ImportParameter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}