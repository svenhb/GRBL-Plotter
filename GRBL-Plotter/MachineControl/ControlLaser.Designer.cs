namespace GRBL_Plotter
{
    partial class ControlLaser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlLaser));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nUDMotionDelayPower = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.nUDMotionDelay = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.nUDMotionY = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nUDMotionX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.nUDMotionSpeed = new System.Windows.Forms.NumericUpDown();
            this.btnScanZ = new System.Windows.Forms.Button();
            this.nUDMotionZ = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDMotionPower = new System.Windows.Forms.NumericUpDown();
            this.cBLaserMode = new System.Windows.Forms.CheckBox();
            this.rBM3 = new System.Windows.Forms.RadioButton();
            this.nUDLaserPower = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.nUDSpeedStep = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.btnScanSpeed = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.nUDSpeedPower = new System.Windows.Forms.NumericUpDown();
            this.nUDSpeedMax = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nUDSpeedMin = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.nUDPowerStep = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.nUDPowerSpeed = new System.Windows.Forms.NumericUpDown();
            this.nUDPowerMax = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nUDPowerMin = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.btnScanPower = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnToolUpdate = new System.Windows.Forms.Button();
            this.lblToolProp = new System.Windows.Forms.Label();
            this.btnScanTool = new System.Windows.Forms.Button();
            this.cBTool = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionDelayPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionX)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionPower)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedMin)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerMin)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.nUDMotionDelayPower);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.nUDMotionDelay);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.nUDMotionY);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.nUDMotionX);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // nUDMotionDelayPower
            // 
            resources.ApplyResources(this.nUDMotionDelayPower, "nUDMotionDelayPower");
            this.nUDMotionDelayPower.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserMotionDelayPower", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMotionDelayPower.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDMotionDelayPower.Name = "nUDMotionDelayPower";
            this.nUDMotionDelayPower.Value = global::GRBL_Plotter.Properties.Settings.Default.laserMotionDelayPower;
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // nUDMotionDelay
            // 
            resources.ApplyResources(this.nUDMotionDelay, "nUDMotionDelay");
            this.nUDMotionDelay.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserMotionDelay", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMotionDelay.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDMotionDelay.Name = "nUDMotionDelay";
            this.nUDMotionDelay.Value = global::GRBL_Plotter.Properties.Settings.Default.laserMotionDelay;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // nUDMotionY
            // 
            resources.ApplyResources(this.nUDMotionY, "nUDMotionY");
            this.nUDMotionY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserMotionY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMotionY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nUDMotionY.Name = "nUDMotionY";
            this.nUDMotionY.Value = global::GRBL_Plotter.Properties.Settings.Default.laserMotionY;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // nUDMotionX
            // 
            resources.ApplyResources(this.nUDMotionX, "nUDMotionX");
            this.nUDMotionX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserMotionX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMotionX.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDMotionX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDMotionX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDMotionX.Name = "nUDMotionX";
            this.nUDMotionX.Value = global::GRBL_Plotter.Properties.Settings.Default.laserMotionX;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblText
            // 
            resources.ApplyResources(this.lblText, "lblText");
            this.lblText.Name = "lblText";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.nUDMotionSpeed);
            this.groupBox2.Controls.Add(this.btnScanZ);
            this.groupBox2.Controls.Add(this.nUDMotionZ);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.nUDMotionPower);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // nUDMotionSpeed
            // 
            resources.ApplyResources(this.nUDMotionSpeed, "nUDMotionSpeed");
            this.nUDMotionSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanZFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMotionSpeed.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDMotionSpeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDMotionSpeed.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDMotionSpeed.Name = "nUDMotionSpeed";
            this.nUDMotionSpeed.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanZFeed;
            // 
            // btnScanZ
            // 
            resources.ApplyResources(this.btnScanZ, "btnScanZ");
            this.btnScanZ.Name = "btnScanZ";
            this.btnScanZ.UseVisualStyleBackColor = true;
            this.btnScanZ.Click += new System.EventHandler(this.btnScanZ_Click);
            // 
            // nUDMotionZ
            // 
            resources.ApplyResources(this.nUDMotionZ, "nUDMotionZ");
            this.nUDMotionZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanZRange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMotionZ.Name = "nUDMotionZ";
            this.nUDMotionZ.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanZRange;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nUDMotionPower
            // 
            resources.ApplyResources(this.nUDMotionPower, "nUDMotionPower");
            this.nUDMotionPower.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanZPower", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMotionPower.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDMotionPower.Name = "nUDMotionPower";
            this.nUDMotionPower.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanZPower;
            // 
            // cBLaserMode
            // 
            resources.ApplyResources(this.cBLaserMode, "cBLaserMode");
            this.cBLaserMode.Name = "cBLaserMode";
            this.cBLaserMode.UseVisualStyleBackColor = true;
            this.cBLaserMode.CheckedChanged += new System.EventHandler(this.cBLaserMode_CheckedChanged);
            // 
            // rBM3
            // 
            resources.ApplyResources(this.rBM3, "rBM3");
            this.rBM3.Checked = true;
            this.rBM3.Name = "rBM3";
            this.rBM3.TabStop = true;
            this.rBM3.UseVisualStyleBackColor = true;
            // 
            // nUDLaserPower
            // 
            resources.ApplyResources(this.nUDLaserPower, "nUDLaserPower");
            this.nUDLaserPower.Name = "nUDLaserPower";
            this.nUDLaserPower.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.nUDSpeedStep);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.btnScanSpeed);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.nUDSpeedPower);
            this.groupBox3.Controls.Add(this.nUDSpeedMax);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.nUDSpeedMin);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // nUDSpeedStep
            // 
            resources.ApplyResources(this.nUDSpeedStep, "nUDSpeedStep");
            this.nUDSpeedStep.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanSpeedStep", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSpeedStep.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDSpeedStep.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDSpeedStep.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDSpeedStep.Name = "nUDSpeedStep";
            this.nUDSpeedStep.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanSpeedStep;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // btnScanSpeed
            // 
            resources.ApplyResources(this.btnScanSpeed, "btnScanSpeed");
            this.btnScanSpeed.Name = "btnScanSpeed";
            this.btnScanSpeed.UseVisualStyleBackColor = true;
            this.btnScanSpeed.Click += new System.EventHandler(this.btnScanSpeed_Click);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // nUDSpeedPower
            // 
            resources.ApplyResources(this.nUDSpeedPower, "nUDSpeedPower");
            this.nUDSpeedPower.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanSpeedPower", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSpeedPower.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDSpeedPower.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDSpeedPower.Name = "nUDSpeedPower";
            this.nUDSpeedPower.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanSpeedPower;
            // 
            // nUDSpeedMax
            // 
            resources.ApplyResources(this.nUDSpeedMax, "nUDSpeedMax");
            this.nUDSpeedMax.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanSpeedTo", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSpeedMax.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDSpeedMax.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDSpeedMax.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDSpeedMax.Name = "nUDSpeedMax";
            this.nUDSpeedMax.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanSpeedTo;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // nUDSpeedMin
            // 
            resources.ApplyResources(this.nUDSpeedMin, "nUDSpeedMin");
            this.nUDSpeedMin.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanSpeedFrom", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSpeedMin.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDSpeedMin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDSpeedMin.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDSpeedMin.Name = "nUDSpeedMin";
            this.nUDSpeedMin.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanSpeedFrom;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.nUDPowerStep);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.nUDPowerSpeed);
            this.groupBox4.Controls.Add(this.nUDPowerMax);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.nUDPowerMin);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // nUDPowerStep
            // 
            resources.ApplyResources(this.nUDPowerStep, "nUDPowerStep");
            this.nUDPowerStep.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanPowerStep", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPowerStep.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDPowerStep.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDPowerStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDPowerStep.Name = "nUDPowerStep";
            this.nUDPowerStep.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanPowerStep;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // nUDPowerSpeed
            // 
            resources.ApplyResources(this.nUDPowerSpeed, "nUDPowerSpeed");
            this.nUDPowerSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanPowerFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPowerSpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDPowerSpeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDPowerSpeed.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDPowerSpeed.Name = "nUDPowerSpeed";
            this.nUDPowerSpeed.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanPowerFeed;
            // 
            // nUDPowerMax
            // 
            resources.ApplyResources(this.nUDPowerMax, "nUDPowerMax");
            this.nUDPowerMax.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanPowerTo", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPowerMax.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDPowerMax.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDPowerMax.Name = "nUDPowerMax";
            this.nUDPowerMax.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanPowerTo;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // nUDPowerMin
            // 
            resources.ApplyResources(this.nUDPowerMin, "nUDPowerMin");
            this.nUDPowerMin.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "laserScanPowerFrom", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPowerMin.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDPowerMin.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDPowerMin.Name = "nUDPowerMin";
            this.nUDPowerMin.Value = global::GRBL_Plotter.Properties.Settings.Default.laserScanPowerFrom;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // btnScanPower
            // 
            resources.ApplyResources(this.btnScanPower, "btnScanPower");
            this.btnScanPower.Name = "btnScanPower";
            this.btnScanPower.UseVisualStyleBackColor = true;
            this.btnScanPower.Click += new System.EventHandler(this.btnScanPower_Click);
            // 
            // lblInfo
            // 
            resources.ApplyResources(this.lblInfo, "lblInfo");
            this.lblInfo.Name = "lblInfo";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.cBLaserMode);
            this.groupBox5.Controls.Add(this.rBM3);
            this.groupBox5.Controls.Add(this.nUDLaserPower);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.btnToolUpdate);
            this.groupBox6.Controls.Add(this.lblToolProp);
            this.groupBox6.Controls.Add(this.btnScanTool);
            this.groupBox6.Controls.Add(this.cBTool);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // btnToolUpdate
            // 
            resources.ApplyResources(this.btnToolUpdate, "btnToolUpdate");
            this.btnToolUpdate.Name = "btnToolUpdate";
            this.btnToolUpdate.UseVisualStyleBackColor = true;
            this.btnToolUpdate.Click += new System.EventHandler(this.btnToolUpdate_Click);
            // 
            // lblToolProp
            // 
            resources.ApplyResources(this.lblToolProp, "lblToolProp");
            this.lblToolProp.Name = "lblToolProp";
            // 
            // btnScanTool
            // 
            resources.ApplyResources(this.btnScanTool, "btnScanTool");
            this.btnScanTool.Name = "btnScanTool";
            this.btnScanTool.UseVisualStyleBackColor = true;
            this.btnScanTool.Click += new System.EventHandler(this.btnScanTool_Click);
            // 
            // cBTool
            // 
            resources.ApplyResources(this.cBTool, "cBTool");
            this.cBTool.FormattingEnabled = true;
            this.cBTool.Name = "cBTool";
            this.cBTool.SelectedIndexChanged += new System.EventHandler(this.cBTool_SelectedIndexChanged);
            // 
            // ControlLaser
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.btnScanPower);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.groupBox1);
            this.Name = "ControlLaser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlLaser_FormClosing);
            this.Load += new System.EventHandler(this.ControlLaser_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionDelayPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMotionPower)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSpeedMin)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPowerMin)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nUDMotionY;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nUDMotionX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnScanZ;
        private System.Windows.Forms.NumericUpDown nUDMotionZ;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cBLaserMode;
        private System.Windows.Forms.RadioButton rBM3;
        private System.Windows.Forms.RadioButton nUDLaserPower;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnScanSpeed;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nUDSpeedPower;
        private System.Windows.Forms.NumericUpDown nUDSpeedMax;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nUDSpeedMin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUDMotionPower;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnScanPower;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nUDPowerSpeed;
        private System.Windows.Forms.NumericUpDown nUDPowerMax;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nUDPowerMin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nUDMotionSpeed;
        private System.Windows.Forms.NumericUpDown nUDSpeedStep;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nUDPowerStep;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nUDMotionDelayPower;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown nUDMotionDelay;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ComboBox cBTool;
        private System.Windows.Forms.Label lblToolProp;
        private System.Windows.Forms.Button btnScanTool;
        private System.Windows.Forms.Button btnToolUpdate;
    }
}