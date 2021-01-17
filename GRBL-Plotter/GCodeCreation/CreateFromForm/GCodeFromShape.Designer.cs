namespace GRBL_Plotter
{
    partial class GCodeFromShape
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
                pathBackground.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeFromShape));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cBNoZUp = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cBToolSet = new System.Windows.Forms.CheckBox();
            this.cBTool = new System.Windows.Forms.ComboBox();
            this.nUDToolOverlap = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.nUDToolSpindleSpeed = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.nUDToolFeedZ = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.nUDToolFeedXY = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.nUDToolZStep = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nUDToolDiameter = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nUDShapeY = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.nUDShapeX = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.nUDShapeR = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.rBShape2 = new System.Windows.Forms.RadioButton();
            this.rBShape1 = new System.Windows.Forms.RadioButton();
            this.rBShape3 = new System.Windows.Forms.RadioButton();
            this.nUDImportGCZDown = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.rBToolpath2 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rBToolpath3 = new System.Windows.Forms.RadioButton();
            this.rBToolpath1 = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cBToolpathPocket = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rBOrigin9 = new System.Windows.Forms.RadioButton();
            this.rBOrigin8 = new System.Windows.Forms.RadioButton();
            this.rBOrigin7 = new System.Windows.Forms.RadioButton();
            this.rBOrigin6 = new System.Windows.Forms.RadioButton();
            this.rBOrigin5 = new System.Windows.Forms.RadioButton();
            this.rBOrigin4 = new System.Windows.Forms.RadioButton();
            this.rBOrigin3 = new System.Windows.Forms.RadioButton();
            this.rBOrigin2 = new System.Windows.Forms.RadioButton();
            this.rBOrigin1 = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rBBevel1 = new System.Windows.Forms.RadioButton();
            this.rBBevel2 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDBevelR = new System.Windows.Forms.NumericUpDown();
            this.rB4 = new System.Windows.Forms.RadioButton();
            this.rB3 = new System.Windows.Forms.RadioButton();
            this.rB2 = new System.Windows.Forms.RadioButton();
            this.rB1 = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolOverlap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolSpindleSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolFeedXY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolZStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolDiameter)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDBevelR)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cBNoZUp);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cBToolSet);
            this.groupBox1.Controls.Add(this.cBTool);
            this.groupBox1.Controls.Add(this.nUDToolOverlap);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.nUDToolSpindleSpeed);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.nUDToolFeedZ);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.nUDToolFeedXY);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.nUDToolZStep);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.nUDToolDiameter);
            this.groupBox1.Controls.Add(this.label3);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cBNoZUp
            // 
            resources.ApplyResources(this.cBNoZUp, "cBNoZUp");
            this.cBNoZUp.Name = "cBNoZUp";
            this.cBNoZUp.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cBToolSet
            // 
            resources.ApplyResources(this.cBToolSet, "cBToolSet");
            this.cBToolSet.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCToolUseRouter;
            this.cBToolSet.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCToolUseRouter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolSet.Name = "cBToolSet";
            this.cBToolSet.UseVisualStyleBackColor = true;
            this.cBToolSet.CheckedChanged += new System.EventHandler(this.cBToolSet_CheckedChanged);
            // 
            // cBTool
            // 
            this.cBTool.FormattingEnabled = true;
            resources.ApplyResources(this.cBTool, "cBTool");
            this.cBTool.Name = "cBTool";
            this.cBTool.SelectedIndexChanged += new System.EventHandler(this.cBTool_SelectedIndexChanged);
            // 
            // nUDToolOverlap
            // 
            this.nUDToolOverlap.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDToolOverlap, "nUDToolOverlap");
            this.nUDToolOverlap.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDToolOverlap.Name = "nUDToolOverlap";
            this.toolTip1.SetToolTip(this.nUDToolOverlap, resources.GetString("nUDToolOverlap.ToolTip"));
            this.nUDToolOverlap.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            this.toolTip1.SetToolTip(this.label16, resources.GetString("label16.ToolTip"));
            // 
            // nUDToolSpindleSpeed
            // 
            this.nUDToolSpindleSpeed.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDToolSpindleSpeed, "nUDToolSpindleSpeed");
            this.nUDToolSpindleSpeed.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.nUDToolSpindleSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDToolSpindleSpeed.Name = "nUDToolSpindleSpeed";
            this.toolTip1.SetToolTip(this.nUDToolSpindleSpeed, resources.GetString("nUDToolSpindleSpeed.ToolTip"));
            this.nUDToolSpindleSpeed.Value = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // nUDToolFeedZ
            // 
            this.nUDToolFeedZ.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDToolFeedZ, "nUDToolFeedZ");
            this.nUDToolFeedZ.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDToolFeedZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDToolFeedZ.Name = "nUDToolFeedZ";
            this.toolTip1.SetToolTip(this.nUDToolFeedZ, resources.GetString("nUDToolFeedZ.ToolTip"));
            this.nUDToolFeedZ.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // nUDToolFeedXY
            // 
            this.nUDToolFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDToolFeedXY, "nUDToolFeedXY");
            this.nUDToolFeedXY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDToolFeedXY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDToolFeedXY.Name = "nUDToolFeedXY";
            this.toolTip1.SetToolTip(this.nUDToolFeedXY, resources.GetString("nUDToolFeedXY.ToolTip"));
            this.nUDToolFeedXY.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // nUDToolZStep
            // 
            this.nUDToolZStep.DecimalPlaces = 2;
            this.nUDToolZStep.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDToolZStep, "nUDToolZStep");
            this.nUDToolZStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDToolZStep.Name = "nUDToolZStep";
            this.toolTip1.SetToolTip(this.nUDToolZStep, resources.GetString("nUDToolZStep.ToolTip"));
            this.nUDToolZStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // nUDToolDiameter
            // 
            this.nUDToolDiameter.DecimalPlaces = 1;
            this.nUDToolDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDToolDiameter, "nUDToolDiameter");
            this.nUDToolDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDToolDiameter.Name = "nUDToolDiameter";
            this.toolTip1.SetToolTip(this.nUDToolDiameter, resources.GetString("nUDToolDiameter.ToolTip"));
            this.nUDToolDiameter.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nUDShapeY);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.nUDShapeX);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.nUDShapeR);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.rBShape2);
            this.groupBox2.Controls.Add(this.rBShape1);
            this.groupBox2.Controls.Add(this.rBShape3);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // nUDShapeY
            // 
            this.nUDShapeY.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDShapeY, "nUDShapeY");
            this.nUDShapeY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDShapeY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDShapeY.Name = "nUDShapeY";
            this.nUDShapeY.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDShapeY.ValueChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // nUDShapeX
            // 
            this.nUDShapeX.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDShapeX, "nUDShapeX");
            this.nUDShapeX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDShapeX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDShapeX.Name = "nUDShapeX";
            this.nUDShapeX.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDShapeX.ValueChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // nUDShapeR
            // 
            this.nUDShapeR.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDShapeR, "nUDShapeR");
            this.nUDShapeR.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDShapeR.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDShapeR.Name = "nUDShapeR";
            this.nUDShapeR.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nUDShapeR.ValueChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // rBShape2
            // 
            resources.ApplyResources(this.rBShape2, "rBShape2");
            this.rBShape2.Name = "rBShape2";
            this.rBShape2.UseVisualStyleBackColor = true;
            this.rBShape2.CheckedChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // rBShape1
            // 
            resources.ApplyResources(this.rBShape1, "rBShape1");
            this.rBShape1.Checked = true;
            this.rBShape1.Name = "rBShape1";
            this.rBShape1.TabStop = true;
            this.rBShape1.UseVisualStyleBackColor = true;
            this.rBShape1.CheckedChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // rBShape3
            // 
            resources.ApplyResources(this.rBShape3, "rBShape3");
            this.rBShape3.Name = "rBShape3";
            this.rBShape3.UseVisualStyleBackColor = true;
            this.rBShape3.CheckedChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // nUDImportGCZDown
            // 
            this.nUDImportGCZDown.DecimalPlaces = 2;
            this.nUDImportGCZDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDImportGCZDown, "nUDImportGCZDown");
            this.nUDImportGCZDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            -2147418112});
            this.nUDImportGCZDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDImportGCZDown.Name = "nUDImportGCZDown";
            this.toolTip1.SetToolTip(this.nUDImportGCZDown, resources.GetString("nUDImportGCZDown.ToolTip"));
            this.nUDImportGCZDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            -2147483648});
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // rBToolpath2
            // 
            resources.ApplyResources(this.rBToolpath2, "rBToolpath2");
            this.rBToolpath2.Name = "rBToolpath2";
            this.rBToolpath2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rBToolpath3);
            this.groupBox3.Controls.Add(this.rBToolpath1);
            this.groupBox3.Controls.Add(this.rBToolpath2);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // rBToolpath3
            // 
            resources.ApplyResources(this.rBToolpath3, "rBToolpath3");
            this.rBToolpath3.Name = "rBToolpath3";
            this.rBToolpath3.UseVisualStyleBackColor = true;
            // 
            // rBToolpath1
            // 
            resources.ApplyResources(this.rBToolpath1, "rBToolpath1");
            this.rBToolpath1.Checked = true;
            this.rBToolpath1.Name = "rBToolpath1";
            this.rBToolpath1.TabStop = true;
            this.rBToolpath1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // cBToolpathPocket
            // 
            resources.ApplyResources(this.cBToolpathPocket, "cBToolpathPocket");
            this.cBToolpathPocket.Name = "cBToolpathPocket";
            this.cBToolpathPocket.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rBOrigin9);
            this.groupBox5.Controls.Add(this.rBOrigin8);
            this.groupBox5.Controls.Add(this.rBOrigin7);
            this.groupBox5.Controls.Add(this.rBOrigin6);
            this.groupBox5.Controls.Add(this.rBOrigin5);
            this.groupBox5.Controls.Add(this.rBOrigin4);
            this.groupBox5.Controls.Add(this.rBOrigin3);
            this.groupBox5.Controls.Add(this.rBOrigin2);
            this.groupBox5.Controls.Add(this.rBOrigin1);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // rBOrigin9
            // 
            resources.ApplyResources(this.rBOrigin9, "rBOrigin9");
            this.rBOrigin9.Name = "rBOrigin9";
            this.rBOrigin9.UseVisualStyleBackColor = true;
            // 
            // rBOrigin8
            // 
            resources.ApplyResources(this.rBOrigin8, "rBOrigin8");
            this.rBOrigin8.Name = "rBOrigin8";
            this.rBOrigin8.UseVisualStyleBackColor = true;
            // 
            // rBOrigin7
            // 
            resources.ApplyResources(this.rBOrigin7, "rBOrigin7");
            this.rBOrigin7.Name = "rBOrigin7";
            this.rBOrigin7.UseVisualStyleBackColor = true;
            // 
            // rBOrigin6
            // 
            resources.ApplyResources(this.rBOrigin6, "rBOrigin6");
            this.rBOrigin6.Name = "rBOrigin6";
            this.rBOrigin6.UseVisualStyleBackColor = true;
            // 
            // rBOrigin5
            // 
            resources.ApplyResources(this.rBOrigin5, "rBOrigin5");
            this.rBOrigin5.Checked = true;
            this.rBOrigin5.Name = "rBOrigin5";
            this.rBOrigin5.TabStop = true;
            this.rBOrigin5.UseVisualStyleBackColor = true;
            // 
            // rBOrigin4
            // 
            resources.ApplyResources(this.rBOrigin4, "rBOrigin4");
            this.rBOrigin4.Name = "rBOrigin4";
            this.rBOrigin4.UseVisualStyleBackColor = true;
            // 
            // rBOrigin3
            // 
            resources.ApplyResources(this.rBOrigin3, "rBOrigin3");
            this.rBOrigin3.Name = "rBOrigin3";
            this.rBOrigin3.UseVisualStyleBackColor = true;
            // 
            // rBOrigin2
            // 
            resources.ApplyResources(this.rBOrigin2, "rBOrigin2");
            this.rBOrigin2.Name = "rBOrigin2";
            this.rBOrigin2.UseVisualStyleBackColor = true;
            // 
            // rBOrigin1
            // 
            resources.ApplyResources(this.rBOrigin1, "rBOrigin1");
            this.rBOrigin1.Name = "rBOrigin1";
            this.rBOrigin1.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cBToolpathPocket);
            this.tabPage1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.rB4);
            this.tabPage2.Controls.Add(this.rB3);
            this.tabPage2.Controls.Add(this.rB2);
            this.tabPage2.Controls.Add(this.rB1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rBBevel1);
            this.groupBox6.Controls.Add(this.rBBevel2);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.nUDBevelR);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // rBBevel1
            // 
            resources.ApplyResources(this.rBBevel1, "rBBevel1");
            this.rBBevel1.Checked = true;
            this.rBBevel1.Name = "rBBevel1";
            this.rBBevel1.TabStop = true;
            this.rBBevel1.UseVisualStyleBackColor = true;
            this.rBBevel1.CheckedChanged += new System.EventHandler(this.rBBevel1_CheckedChanged);
            // 
            // rBBevel2
            // 
            resources.ApplyResources(this.rBBevel2, "rBBevel2");
            this.rBBevel2.Name = "rBBevel2";
            this.rBBevel2.UseVisualStyleBackColor = true;
            this.rBBevel2.CheckedChanged += new System.EventHandler(this.rBBevel1_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nUDBevelR
            // 
            this.nUDBevelR.DecimalPlaces = 2;
            resources.ApplyResources(this.nUDBevelR, "nUDBevelR");
            this.nUDBevelR.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDBevelR.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDBevelR.Name = "nUDBevelR";
            this.nUDBevelR.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // rB4
            // 
            resources.ApplyResources(this.rB4, "rB4");
            this.rB4.Name = "rB4";
            this.rB4.TabStop = true;
            this.rB4.UseVisualStyleBackColor = true;
            this.rB4.CheckedChanged += new System.EventHandler(this.rB1_CheckedChanged);
            // 
            // rB3
            // 
            resources.ApplyResources(this.rB3, "rB3");
            this.rB3.Name = "rB3";
            this.rB3.TabStop = true;
            this.rB3.UseVisualStyleBackColor = true;
            this.rB3.CheckedChanged += new System.EventHandler(this.rB1_CheckedChanged);
            // 
            // rB2
            // 
            resources.ApplyResources(this.rB2, "rB2");
            this.rB2.Name = "rB2";
            this.rB2.TabStop = true;
            this.rB2.UseVisualStyleBackColor = true;
            this.rB2.CheckedChanged += new System.EventHandler(this.rB1_CheckedChanged);
            // 
            // rB1
            // 
            resources.ApplyResources(this.rB1, "rB1");
            this.rB1.Name = "rB1";
            this.rB1.TabStop = true;
            this.rB1.UseVisualStyleBackColor = true;
            this.rB1.CheckedChanged += new System.EventHandler(this.rB1_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.nUDImportGCZDown);
            this.groupBox7.Controls.Add(this.label20);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // GCodeFromShape
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "GCodeFromShape";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShapeToGCode_FormClosing);
            this.Load += new System.EventHandler(this.ShapeToGCode_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolOverlap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolSpindleSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolFeedZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolFeedXY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolZStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolDiameter)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDBevelR)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nUDToolOverlap;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown nUDToolSpindleSpeed;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown nUDToolFeedZ;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nUDToolFeedXY;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nUDToolZStep;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nUDToolDiameter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown nUDImportGCZDown;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown nUDShapeY;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown nUDShapeX;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.NumericUpDown nUDShapeR;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.RadioButton rBShape2;
        private System.Windows.Forms.RadioButton rBShape1;
        private System.Windows.Forms.RadioButton rBShape3;
        private System.Windows.Forms.RadioButton rBToolpath2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cBToolpathPocket;
        private System.Windows.Forms.RadioButton rBToolpath3;
        private System.Windows.Forms.RadioButton rBToolpath1;
        public System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rBOrigin9;
        private System.Windows.Forms.RadioButton rBOrigin8;
        private System.Windows.Forms.RadioButton rBOrigin7;
        private System.Windows.Forms.RadioButton rBOrigin6;
        private System.Windows.Forms.RadioButton rBOrigin5;
        private System.Windows.Forms.RadioButton rBOrigin4;
        private System.Windows.Forms.RadioButton rBOrigin3;
        private System.Windows.Forms.RadioButton rBOrigin2;
        private System.Windows.Forms.RadioButton rBOrigin1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cBToolSet;
        private System.Windows.Forms.ComboBox cBTool;
        private System.Windows.Forms.CheckBox cBNoZUp;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.NumericUpDown nUDBevelR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rB4;
        private System.Windows.Forms.RadioButton rB3;
        private System.Windows.Forms.RadioButton rB2;
        private System.Windows.Forms.RadioButton rB1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rBBevel1;
        private System.Windows.Forms.RadioButton rBBevel2;
        private System.Windows.Forms.GroupBox groupBox7;
    }
}