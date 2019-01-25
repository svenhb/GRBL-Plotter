namespace GRBL_Plotter
{
    partial class ControlHeightMapForm
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
                if (heightLegendBMP != null)
                    heightLegendBMP.Dispose();
                if (heightMapBMP != null)
                    heightMapBMP.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlHeightMapForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.nUDDeltaY = new System.Windows.Forms.NumericUpDown();
            this.nUDDeltaX = new System.Windows.Forms.NumericUpDown();
            this.nUDGridY = new System.Windows.Forms.NumericUpDown();
            this.nUDGridX = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPosUR = new System.Windows.Forms.Button();
            this.btnPosLL = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nUDY2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nUDX2 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nUDX1 = new System.Windows.Forms.NumericUpDown();
            this.nUDY1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOffset = new System.Windows.Forms.Button();
            this.nUDProbeSpeed = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.nUDProbeUp = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.nUDProbeDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.btnStartHeightScan = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblMin = new System.Windows.Forms.Label();
            this.lblMax = new System.Windows.Forms.Label();
            this.lblMid = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblXDim = new System.Windows.Forms.Label();
            this.lblYDim = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadHeightMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveHeightMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePictureAsBWBMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePictureAsBMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapAsSTLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapAsX3DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cBGray = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnOffsetZ = new System.Windows.Forms.Button();
            this.btnZoomZ = new System.Windows.Forms.Button();
            this.btnInvertZ = new System.Windows.Forms.Button();
            this.btnCutOffZ = new System.Windows.Forms.Button();
            this.btnGCode = new System.Windows.Forms.Button();
            this.gB_Manipulation = new System.Windows.Forms.GroupBox();
            this.nUDCutOffZ = new System.Windows.Forms.NumericUpDown();
            this.nUDZoomZ = new System.Windows.Forms.NumericUpDown();
            this.nUDOffsetZ = new System.Windows.Forms.NumericUpDown();
            this.lblInfo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDDeltaY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDDeltaX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGridY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGridX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDY2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDX2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDX1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDY1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gB_Manipulation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDCutOffZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZoomZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetZ)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.nUDDeltaY);
            this.groupBox1.Controls.Add(this.nUDDeltaX);
            this.groupBox1.Controls.Add(this.nUDGridY);
            this.groupBox1.Controls.Add(this.nUDGridX);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnPosUR);
            this.groupBox1.Controls.Add(this.btnPosLL);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.nUDY2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.nUDX2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nUDX1);
            this.groupBox1.Controls.Add(this.nUDY1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // nUDDeltaY
            // 
            this.nUDDeltaY.DecimalPlaces = 2;
            this.nUDDeltaY.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDDeltaY, "nUDDeltaY");
            this.nUDDeltaY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDDeltaY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDDeltaY.Name = "nUDDeltaY";
            this.nUDDeltaY.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDDeltaY.ValueChanged += new System.EventHandler(this.nUDDeltaX_ValueChanged);
            // 
            // nUDDeltaX
            // 
            this.nUDDeltaX.DecimalPlaces = 2;
            this.nUDDeltaX.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDDeltaX, "nUDDeltaX");
            this.nUDDeltaX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDDeltaX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDDeltaX.Name = "nUDDeltaX";
            this.nUDDeltaX.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDDeltaX.ValueChanged += new System.EventHandler(this.nUDDeltaX_ValueChanged);
            // 
            // nUDGridY
            // 
            this.nUDGridY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "heightMapGridY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDGridY.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDGridY, "nUDGridY");
            this.nUDGridY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDGridY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDGridY.Name = "nUDGridY";
            this.nUDGridY.Value = global::GRBL_Plotter.Properties.Settings.Default.heightMapGridY;
            // 
            // nUDGridX
            // 
            this.nUDGridX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "heightMapGridX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDGridX.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDGridX, "nUDGridX");
            this.nUDGridX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDGridX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDGridX.Name = "nUDGridX";
            this.nUDGridX.Value = global::GRBL_Plotter.Properties.Settings.Default.heightMapGridX;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // btnPosUR
            // 
            resources.ApplyResources(this.btnPosUR, "btnPosUR");
            this.btnPosUR.Name = "btnPosUR";
            this.toolTip1.SetToolTip(this.btnPosUR, resources.GetString("btnPosUR.ToolTip"));
            this.btnPosUR.UseVisualStyleBackColor = true;
            this.btnPosUR.Click += new System.EventHandler(this.btnPosUR_Click);
            // 
            // btnPosLL
            // 
            resources.ApplyResources(this.btnPosLL, "btnPosLL");
            this.btnPosLL.Name = "btnPosLL";
            this.toolTip1.SetToolTip(this.btnPosLL, resources.GetString("btnPosLL.ToolTip"));
            this.btnPosLL.UseVisualStyleBackColor = true;
            this.btnPosLL.Click += new System.EventHandler(this.btnPosLL_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // nUDY2
            // 
            this.nUDY2.DecimalPlaces = 2;
            this.nUDY2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDY2, "nUDY2");
            this.nUDY2.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDY2.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nUDY2.Name = "nUDY2";
            this.nUDY2.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDY2.ValueChanged += new System.EventHandler(this.nUDX2_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // nUDX2
            // 
            this.nUDX2.DecimalPlaces = 2;
            this.nUDX2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDX2, "nUDX2");
            this.nUDX2.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDX2.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nUDX2.Name = "nUDX2";
            this.nUDX2.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nUDX2.ValueChanged += new System.EventHandler(this.nUDX2_ValueChanged);
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
            // nUDX1
            // 
            this.nUDX1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "heightMapX1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDX1.DecimalPlaces = 2;
            this.nUDX1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDX1, "nUDX1");
            this.nUDX1.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDX1.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nUDX1.Name = "nUDX1";
            this.nUDX1.Value = global::GRBL_Plotter.Properties.Settings.Default.heightMapX1;
            this.nUDX1.ValueChanged += new System.EventHandler(this.nUDX1_ValueChanged);
            // 
            // nUDY1
            // 
            this.nUDY1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "heightMapY1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDY1.DecimalPlaces = 2;
            this.nUDY1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDY1, "nUDY1");
            this.nUDY1.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDY1.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.nUDY1.Name = "nUDY1";
            this.nUDY1.Value = global::GRBL_Plotter.Properties.Settings.Default.heightMapY1;
            this.nUDY1.ValueChanged += new System.EventHandler(this.nUDX1_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnOffset);
            this.groupBox2.Controls.Add(this.nUDProbeSpeed);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.nUDProbeUp);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.nUDProbeDown);
            this.groupBox2.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnOffset
            // 
            resources.ApplyResources(this.btnOffset, "btnOffset");
            this.btnOffset.Name = "btnOffset";
            this.toolTip1.SetToolTip(this.btnOffset, resources.GetString("btnOffset.ToolTip"));
            this.btnOffset.UseVisualStyleBackColor = true;
            this.btnOffset.Click += new System.EventHandler(this.btnOffset_Click);
            // 
            // nUDProbeSpeed
            // 
            this.nUDProbeSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "heightMapProbeSpeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeSpeed.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDProbeSpeed, "nUDProbeSpeed");
            this.nUDProbeSpeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDProbeSpeed.Name = "nUDProbeSpeed";
            this.nUDProbeSpeed.Value = global::GRBL_Plotter.Properties.Settings.Default.heightMapProbeSpeed;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // nUDProbeUp
            // 
            this.nUDProbeUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "heightMapProbeHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeUp.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDProbeUp, "nUDProbeUp");
            this.nUDProbeUp.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDProbeUp.Name = "nUDProbeUp";
            this.nUDProbeUp.Value = global::GRBL_Plotter.Properties.Settings.Default.heightMapProbeHeight;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // nUDProbeDown
            // 
            this.nUDProbeDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "heightMapProbeDepth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDProbeDown.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDProbeDown, "nUDProbeDown");
            this.nUDProbeDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nUDProbeDown.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.nUDProbeDown.Name = "nUDProbeDown";
            this.nUDProbeDown.Value = global::GRBL_Plotter.Properties.Settings.Default.heightMapProbeDepth;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // btnStartHeightScan
            // 
            this.btnStartHeightScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            resources.ApplyResources(this.btnStartHeightScan, "btnStartHeightScan");
            this.btnStartHeightScan.Name = "btnStartHeightScan";
            this.toolTip1.SetToolTip(this.btnStartHeightScan, resources.GetString("btnStartHeightScan.ToolTip"));
            this.btnStartHeightScan.UseVisualStyleBackColor = false;
            this.btnStartHeightScan.Click += new System.EventHandler(this.btnStartHeightScan_Click);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // lblProgress
            // 
            resources.ApplyResources(this.lblProgress, "lblProgress");
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            this.lblProgress.Name = "lblProgress";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // lblMin
            // 
            resources.ApplyResources(this.lblMin, "lblMin");
            this.lblMin.Name = "lblMin";
            // 
            // lblMax
            // 
            resources.ApplyResources(this.lblMax, "lblMax");
            this.lblMax.Name = "lblMax";
            // 
            // lblMid
            // 
            resources.ApplyResources(this.lblMid, "lblMid");
            this.lblMid.Name = "lblMid";
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.toolTip1.SetToolTip(this.btnApply, resources.GetString("btnApply.ToolTip"));
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblXDim
            // 
            resources.ApplyResources(this.lblXDim, "lblXDim");
            this.lblXDim.Name = "lblXDim";
            // 
            // lblYDim
            // 
            resources.ApplyResources(this.lblYDim, "lblYDim");
            this.lblYDim.Name = "lblYDim";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Yellow;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadHeightMapToolStripMenuItem,
            this.saveHeightMapToolStripMenuItem,
            this.savePictureAsBWBMPToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // loadHeightMapToolStripMenuItem
            // 
            this.loadHeightMapToolStripMenuItem.Name = "loadHeightMapToolStripMenuItem";
            resources.ApplyResources(this.loadHeightMapToolStripMenuItem, "loadHeightMapToolStripMenuItem");
            this.loadHeightMapToolStripMenuItem.Click += new System.EventHandler(this.loadHeightMapToolStripMenuItem_Click);
            // 
            // saveHeightMapToolStripMenuItem
            // 
            this.saveHeightMapToolStripMenuItem.Name = "saveHeightMapToolStripMenuItem";
            resources.ApplyResources(this.saveHeightMapToolStripMenuItem, "saveHeightMapToolStripMenuItem");
            this.saveHeightMapToolStripMenuItem.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // savePictureAsBWBMPToolStripMenuItem
            // 
            this.savePictureAsBWBMPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.savePictureAsBMPToolStripMenuItem,
            this.saveMapAsSTLToolStripMenuItem,
            this.saveMapAsX3DToolStripMenuItem});
            this.savePictureAsBWBMPToolStripMenuItem.Name = "savePictureAsBWBMPToolStripMenuItem";
            resources.ApplyResources(this.savePictureAsBWBMPToolStripMenuItem, "savePictureAsBWBMPToolStripMenuItem");
            // 
            // savePictureAsBMPToolStripMenuItem
            // 
            this.savePictureAsBMPToolStripMenuItem.Name = "savePictureAsBMPToolStripMenuItem";
            resources.ApplyResources(this.savePictureAsBMPToolStripMenuItem, "savePictureAsBMPToolStripMenuItem");
            this.savePictureAsBMPToolStripMenuItem.Click += new System.EventHandler(this.savePictureAsBMPToolStripMenuItem_Click);
            // 
            // saveMapAsSTLToolStripMenuItem
            // 
            this.saveMapAsSTLToolStripMenuItem.Name = "saveMapAsSTLToolStripMenuItem";
            resources.ApplyResources(this.saveMapAsSTLToolStripMenuItem, "saveMapAsSTLToolStripMenuItem");
            this.saveMapAsSTLToolStripMenuItem.Click += new System.EventHandler(this.btnSaveSTL_Click);
            // 
            // saveMapAsX3DToolStripMenuItem
            // 
            this.saveMapAsX3DToolStripMenuItem.Name = "saveMapAsX3DToolStripMenuItem";
            resources.ApplyResources(this.saveMapAsX3DToolStripMenuItem, "saveMapAsX3DToolStripMenuItem");
            this.saveMapAsX3DToolStripMenuItem.Click += new System.EventHandler(this.btnSaveX3D_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cBGray);
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.lblYDim);
            this.groupBox3.Controls.Add(this.lblXDim);
            this.groupBox3.Controls.Add(this.pictureBox2);
            this.groupBox3.Controls.Add(this.lblMin);
            this.groupBox3.Controls.Add(this.lblMax);
            this.groupBox3.Controls.Add(this.lblMid);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // cBGray
            // 
            resources.ApplyResources(this.cBGray, "cBGray");
            this.cBGray.Name = "cBGray";
            this.toolTip1.SetToolTip(this.cBGray, resources.GetString("cBGray.ToolTip"));
            this.cBGray.UseVisualStyleBackColor = true;
            this.cBGray.CheckedChanged += new System.EventHandler(this.cBGray_CheckedChanged);
            // 
            // btnOffsetZ
            // 
            resources.ApplyResources(this.btnOffsetZ, "btnOffsetZ");
            this.btnOffsetZ.Name = "btnOffsetZ";
            this.toolTip1.SetToolTip(this.btnOffsetZ, resources.GetString("btnOffsetZ.ToolTip"));
            this.btnOffsetZ.UseVisualStyleBackColor = true;
            this.btnOffsetZ.Click += new System.EventHandler(this.btnOffsetZ_Click);
            // 
            // btnZoomZ
            // 
            resources.ApplyResources(this.btnZoomZ, "btnZoomZ");
            this.btnZoomZ.Name = "btnZoomZ";
            this.toolTip1.SetToolTip(this.btnZoomZ, resources.GetString("btnZoomZ.ToolTip"));
            this.btnZoomZ.UseVisualStyleBackColor = true;
            this.btnZoomZ.Click += new System.EventHandler(this.btnZoomZ_Click);
            // 
            // btnInvertZ
            // 
            resources.ApplyResources(this.btnInvertZ, "btnInvertZ");
            this.btnInvertZ.Name = "btnInvertZ";
            this.toolTip1.SetToolTip(this.btnInvertZ, resources.GetString("btnInvertZ.ToolTip"));
            this.btnInvertZ.UseVisualStyleBackColor = true;
            this.btnInvertZ.Click += new System.EventHandler(this.btnInvertZ_Click);
            // 
            // btnCutOffZ
            // 
            resources.ApplyResources(this.btnCutOffZ, "btnCutOffZ");
            this.btnCutOffZ.Name = "btnCutOffZ";
            this.toolTip1.SetToolTip(this.btnCutOffZ, resources.GetString("btnCutOffZ.ToolTip"));
            this.btnCutOffZ.UseVisualStyleBackColor = true;
            this.btnCutOffZ.Click += new System.EventHandler(this.btnCutOffZ_Click);
            // 
            // btnGCode
            // 
            resources.ApplyResources(this.btnGCode, "btnGCode");
            this.btnGCode.Name = "btnGCode";
            this.toolTip1.SetToolTip(this.btnGCode, resources.GetString("btnGCode.ToolTip"));
            this.btnGCode.UseVisualStyleBackColor = true;
            this.btnGCode.Click += new System.EventHandler(this.btnGCode_Click);
            // 
            // gB_Manipulation
            // 
            this.gB_Manipulation.Controls.Add(this.btnGCode);
            this.gB_Manipulation.Controls.Add(this.btnCutOffZ);
            this.gB_Manipulation.Controls.Add(this.nUDCutOffZ);
            this.gB_Manipulation.Controls.Add(this.btnInvertZ);
            this.gB_Manipulation.Controls.Add(this.btnZoomZ);
            this.gB_Manipulation.Controls.Add(this.nUDZoomZ);
            this.gB_Manipulation.Controls.Add(this.btnOffsetZ);
            this.gB_Manipulation.Controls.Add(this.nUDOffsetZ);
            resources.ApplyResources(this.gB_Manipulation, "gB_Manipulation");
            this.gB_Manipulation.Name = "gB_Manipulation";
            this.gB_Manipulation.TabStop = false;
            // 
            // nUDCutOffZ
            // 
            this.nUDCutOffZ.DecimalPlaces = 1;
            this.nUDCutOffZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDCutOffZ, "nUDCutOffZ");
            this.nUDCutOffZ.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDCutOffZ.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDCutOffZ.Name = "nUDCutOffZ";
            this.nUDCutOffZ.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nUDZoomZ
            // 
            this.nUDZoomZ.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDZoomZ, "nUDZoomZ");
            this.nUDZoomZ.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDZoomZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDZoomZ.Name = "nUDZoomZ";
            this.nUDZoomZ.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nUDOffsetZ
            // 
            this.nUDOffsetZ.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDOffsetZ, "nUDOffsetZ");
            this.nUDOffsetZ.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDOffsetZ.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDOffsetZ.Name = "nUDOffsetZ";
            // 
            // lblInfo
            // 
            resources.ApplyResources(this.lblInfo, "lblInfo");
            this.lblInfo.Name = "lblInfo";
            // 
            // ControlHeightMapForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.gB_Manipulation);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnStartHeightScan);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ControlHeightMapForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlHeightMapForm_FormClosing);
            this.Load += new System.EventHandler(this.ControlHeightMapForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDDeltaY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDDeltaX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGridY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDGridX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDY2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDX2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDX1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDY1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDProbeDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gB_Manipulation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nUDCutOffZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDZoomZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOffsetZ)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nUDX1;
        private System.Windows.Forms.NumericUpDown nUDY1;
        private System.Windows.Forms.NumericUpDown nUDX2;
        private System.Windows.Forms.NumericUpDown nUDY2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nUDGridY;
        private System.Windows.Forms.NumericUpDown nUDGridX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnPosUR;
        private System.Windows.Forms.Button btnPosLL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown nUDProbeSpeed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nUDProbeUp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nUDProbeDown;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.Button btnStartHeightScan;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nUDDeltaY;
        private System.Windows.Forms.NumericUpDown nUDDeltaX;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Label lblMid;
        public System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblXDim;
        private System.Windows.Forms.Label lblYDim;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        public System.Windows.Forms.ToolStripMenuItem loadHeightMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveHeightMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePictureAsBWBMPToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOffset;
        private System.Windows.Forms.CheckBox cBGray;
        private System.Windows.Forms.ToolStripMenuItem savePictureAsBMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMapAsSTLToolStripMenuItem;
        private System.Windows.Forms.GroupBox gB_Manipulation;
        private System.Windows.Forms.Button btnZoomZ;
        private System.Windows.Forms.NumericUpDown nUDZoomZ;
        private System.Windows.Forms.Button btnOffsetZ;
        private System.Windows.Forms.NumericUpDown nUDOffsetZ;
        private System.Windows.Forms.Button btnInvertZ;
        private System.Windows.Forms.ToolStripMenuItem saveMapAsX3DToolStripMenuItem;
        private System.Windows.Forms.Button btnCutOffZ;
        private System.Windows.Forms.NumericUpDown nUDCutOffZ;
        public System.Windows.Forms.Button btnGCode;
        private System.Windows.Forms.Label lblInfo;
    }
}