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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.nUDImportGCZDown = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.nUDShapeY = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.nUDShapeX = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.nUDShapeR = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.rBShape2 = new System.Windows.Forms.RadioButton();
            this.rBShape1 = new System.Windows.Forms.RadioButton();
            this.rBShape3 = new System.Windows.Forms.RadioButton();
            this.rBToolpath2 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cBToolpathPocket = new System.Windows.Forms.CheckBox();
            this.rBToolpath3 = new System.Windows.Forms.RadioButton();
            this.rBToolpath1 = new System.Windows.Forms.RadioButton();
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
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolOverlap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolSpindleSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolFeedXY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolZStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDToolDiameter)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeR)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
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
            this.groupBox1.Location = new System.Drawing.Point(1, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 61);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tool";
            // 
            // nUDToolOverlap
            // 
            this.nUDToolOverlap.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nUDToolOverlap.Location = new System.Drawing.Point(240, 32);
            this.nUDToolOverlap.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDToolOverlap.Name = "nUDToolOverlap";
            this.nUDToolOverlap.Size = new System.Drawing.Size(59, 20);
            this.nUDToolOverlap.TabIndex = 11;
            this.nUDToolOverlap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDToolOverlap, "Overlap of paths when making a pocket");
            this.nUDToolOverlap.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(237, 16);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(44, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "Overlap";
            // 
            // nUDToolSpindleSpeed
            // 
            this.nUDToolSpindleSpeed.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDToolSpindleSpeed.Location = new System.Drawing.Point(305, 32);
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
            this.nUDToolSpindleSpeed.Size = new System.Drawing.Size(59, 20);
            this.nUDToolSpindleSpeed.TabIndex = 9;
            this.nUDToolSpindleSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDToolSpindleSpeed, "Spindle speed");
            this.nUDToolSpindleSpeed.Value = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(302, 16);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(64, 13);
            this.label15.TabIndex = 8;
            this.label15.Text = "Spindle-Spd";
            // 
            // nUDToolFeedZ
            // 
            this.nUDToolFeedZ.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDToolFeedZ.Location = new System.Drawing.Point(175, 32);
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
            this.nUDToolFeedZ.Size = new System.Drawing.Size(59, 20);
            this.nUDToolFeedZ.TabIndex = 7;
            this.nUDToolFeedZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDToolFeedZ, "Feed rate for Z");
            this.nUDToolFeedZ.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(172, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 13);
            this.label14.TabIndex = 6;
            this.label14.Text = "Feed-Z";
            // 
            // nUDToolFeedXY
            // 
            this.nUDToolFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDToolFeedXY.Location = new System.Drawing.Point(110, 32);
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
            this.nUDToolFeedXY.Size = new System.Drawing.Size(59, 20);
            this.nUDToolFeedXY.TabIndex = 5;
            this.nUDToolFeedXY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDToolFeedXY, "Feed rate for XY direction");
            this.nUDToolFeedXY.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(107, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Feed-XY";
            // 
            // nUDToolZStep
            // 
            this.nUDToolZStep.DecimalPlaces = 1;
            this.nUDToolZStep.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDToolZStep.Location = new System.Drawing.Point(58, 32);
            this.nUDToolZStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDToolZStep.Name = "nUDToolZStep";
            this.nUDToolZStep.Size = new System.Drawing.Size(46, 20);
            this.nUDToolZStep.TabIndex = 3;
            this.nUDToolZStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDToolZStep, "Depth per pass (not final depth)");
            this.nUDToolZStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(69, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Z+";
            // 
            // nUDToolDiameter
            // 
            this.nUDToolDiameter.DecimalPlaces = 1;
            this.nUDToolDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDToolDiameter.Location = new System.Drawing.Point(6, 32);
            this.nUDToolDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDToolDiameter.Name = "nUDToolDiameter";
            this.nUDToolDiameter.Size = new System.Drawing.Size(46, 20);
            this.nUDToolDiameter.TabIndex = 1;
            this.nUDToolDiameter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDToolDiameter, "Tool diameter to calculate offset path offset");
            this.nUDToolDiameter.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Diameter";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nUDImportGCZDown);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.nUDShapeY);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.nUDShapeX);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.nUDShapeR);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.rBShape2);
            this.groupBox2.Controls.Add(this.rBShape1);
            this.groupBox2.Controls.Add(this.rBShape3);
            this.groupBox2.Location = new System.Drawing.Point(1, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(299, 84);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Shape";
            // 
            // nUDImportGCZDown
            // 
            this.nUDImportGCZDown.DecimalPlaces = 1;
            this.nUDImportGCZDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDImportGCZDown.Location = new System.Drawing.Point(240, 57);
            this.nUDImportGCZDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDImportGCZDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDImportGCZDown.Name = "nUDImportGCZDown";
            this.nUDImportGCZDown.Size = new System.Drawing.Size(55, 20);
            this.nUDImportGCZDown.TabIndex = 19;
            this.nUDImportGCZDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDImportGCZDown, "Final deepth of the shape");
            this.nUDImportGCZDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            -2147483648});
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(237, 41);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(52, 13);
            this.label20.TabIndex = 18;
            this.label20.Text = "Depth (Z)";
            // 
            // nUDShapeY
            // 
            this.nUDShapeY.DecimalPlaces = 1;
            this.nUDShapeY.Location = new System.Drawing.Point(80, 57);
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
            this.nUDShapeY.Size = new System.Drawing.Size(55, 20);
            this.nUDShapeY.TabIndex = 17;
            this.nUDShapeY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDShapeY.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDShapeY.ValueChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(77, 41);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(66, 13);
            this.label19.TabIndex = 16;
            this.label19.Text = "Y-Dimension";
            // 
            // nUDShapeX
            // 
            this.nUDShapeX.DecimalPlaces = 1;
            this.nUDShapeX.Location = new System.Drawing.Point(6, 57);
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
            this.nUDShapeX.Size = new System.Drawing.Size(55, 20);
            this.nUDShapeX.TabIndex = 15;
            this.nUDShapeX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDShapeX.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDShapeX.ValueChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 41);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(66, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "X-Dimension";
            // 
            // nUDShapeR
            // 
            this.nUDShapeR.DecimalPlaces = 1;
            this.nUDShapeR.Location = new System.Drawing.Point(156, 57);
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
            this.nUDShapeR.Size = new System.Drawing.Size(55, 20);
            this.nUDShapeR.TabIndex = 13;
            this.nUDShapeR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDShapeR.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nUDShapeR.ValueChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(153, 41);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(40, 13);
            this.label17.TabIndex = 12;
            this.label17.Text = "Radius";
            // 
            // rBShape2
            // 
            this.rBShape2.AutoSize = true;
            this.rBShape2.Location = new System.Drawing.Point(80, 19);
            this.rBShape2.Name = "rBShape2";
            this.rBShape2.Size = new System.Drawing.Size(131, 17);
            this.rBShape2.TabIndex = 2;
            this.rBShape2.Text = "Rectangle round edge";
            this.rBShape2.UseVisualStyleBackColor = true;
            this.rBShape2.CheckedChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // rBShape1
            // 
            this.rBShape1.AutoSize = true;
            this.rBShape1.Checked = true;
            this.rBShape1.Location = new System.Drawing.Point(6, 19);
            this.rBShape1.Name = "rBShape1";
            this.rBShape1.Size = new System.Drawing.Size(74, 17);
            this.rBShape1.TabIndex = 1;
            this.rBShape1.TabStop = true;
            this.rBShape1.Text = "Rectangle";
            this.rBShape1.UseVisualStyleBackColor = true;
            this.rBShape1.CheckedChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // rBShape3
            // 
            this.rBShape3.AutoSize = true;
            this.rBShape3.Location = new System.Drawing.Point(217, 19);
            this.rBShape3.Name = "rBShape3";
            this.rBShape3.Size = new System.Drawing.Size(51, 17);
            this.rBShape3.TabIndex = 0;
            this.rBShape3.Text = "Circle";
            this.rBShape3.UseVisualStyleBackColor = true;
            this.rBShape3.CheckedChanged += new System.EventHandler(this.nUDShapeR_ValueChanged);
            // 
            // rBToolpath2
            // 
            this.rBToolpath2.AutoSize = true;
            this.rBToolpath2.Location = new System.Drawing.Point(6, 42);
            this.rBToolpath2.Name = "rBToolpath2";
            this.rBToolpath2.Size = new System.Drawing.Size(111, 17);
            this.rBToolpath2.TabIndex = 20;
            this.rBToolpath2.Text = "Outside the shape";
            this.rBToolpath2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.rBToolpath3);
            this.groupBox3.Controls.Add(this.rBToolpath1);
            this.groupBox3.Controls.Add(this.rBToolpath2);
            this.groupBox3.Location = new System.Drawing.Point(1, 160);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(268, 99);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Toolpath";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cBToolpathPocket);
            this.groupBox4.Location = new System.Drawing.Point(156, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(101, 74);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pocket";
            // 
            // cBToolpathPocket
            // 
            this.cBToolpathPocket.AutoSize = true;
            this.cBToolpathPocket.Location = new System.Drawing.Point(6, 19);
            this.cBToolpathPocket.Name = "cBToolpathPocket";
            this.cBToolpathPocket.Size = new System.Drawing.Size(94, 17);
            this.cBToolpathPocket.TabIndex = 0;
            this.cBToolpathPocket.Text = "Create Pocket";
            this.cBToolpathPocket.UseVisualStyleBackColor = true;
            // 
            // rBToolpath3
            // 
            this.rBToolpath3.AutoSize = true;
            this.rBToolpath3.Location = new System.Drawing.Point(6, 65);
            this.rBToolpath3.Name = "rBToolpath3";
            this.rBToolpath3.Size = new System.Drawing.Size(103, 17);
            this.rBToolpath3.TabIndex = 22;
            this.rBToolpath3.Text = "Inside the shape";
            this.rBToolpath3.UseVisualStyleBackColor = true;
            // 
            // rBToolpath1
            // 
            this.rBToolpath1.AutoSize = true;
            this.rBToolpath1.Checked = true;
            this.rBToolpath1.Location = new System.Drawing.Point(6, 19);
            this.rBToolpath1.Name = "rBToolpath1";
            this.rBToolpath1.Size = new System.Drawing.Size(145, 17);
            this.rBToolpath1.TabIndex = 21;
            this.rBToolpath1.TabStop = true;
            this.rBToolpath1.Text = "On the shape (engraving)";
            this.rBToolpath1.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(148, 265);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(98, 23);
            this.btnApply.TabIndex = 29;
            this.btnApply.Text = "Create GCode";
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
            this.groupBox5.Location = new System.Drawing.Point(309, 70);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(71, 84);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Origin";
            // 
            // rBOrigin9
            // 
            this.rBOrigin9.AutoSize = true;
            this.rBOrigin9.Location = new System.Drawing.Point(51, 60);
            this.rBOrigin9.Name = "rBOrigin9";
            this.rBOrigin9.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin9.TabIndex = 8;
            this.rBOrigin9.UseVisualStyleBackColor = true;
            // 
            // rBOrigin8
            // 
            this.rBOrigin8.AutoSize = true;
            this.rBOrigin8.Location = new System.Drawing.Point(31, 60);
            this.rBOrigin8.Name = "rBOrigin8";
            this.rBOrigin8.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin8.TabIndex = 7;
            this.rBOrigin8.UseVisualStyleBackColor = true;
            // 
            // rBOrigin7
            // 
            this.rBOrigin7.AutoSize = true;
            this.rBOrigin7.Location = new System.Drawing.Point(11, 60);
            this.rBOrigin7.Name = "rBOrigin7";
            this.rBOrigin7.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin7.TabIndex = 6;
            this.rBOrigin7.UseVisualStyleBackColor = true;
            // 
            // rBOrigin6
            // 
            this.rBOrigin6.AutoSize = true;
            this.rBOrigin6.Location = new System.Drawing.Point(51, 41);
            this.rBOrigin6.Name = "rBOrigin6";
            this.rBOrigin6.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin6.TabIndex = 5;
            this.rBOrigin6.UseVisualStyleBackColor = true;
            // 
            // rBOrigin5
            // 
            this.rBOrigin5.AutoSize = true;
            this.rBOrigin5.Checked = true;
            this.rBOrigin5.Location = new System.Drawing.Point(31, 41);
            this.rBOrigin5.Name = "rBOrigin5";
            this.rBOrigin5.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin5.TabIndex = 4;
            this.rBOrigin5.TabStop = true;
            this.rBOrigin5.UseVisualStyleBackColor = true;
            // 
            // rBOrigin4
            // 
            this.rBOrigin4.AutoSize = true;
            this.rBOrigin4.Location = new System.Drawing.Point(11, 41);
            this.rBOrigin4.Name = "rBOrigin4";
            this.rBOrigin4.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin4.TabIndex = 3;
            this.rBOrigin4.UseVisualStyleBackColor = true;
            // 
            // rBOrigin3
            // 
            this.rBOrigin3.AutoSize = true;
            this.rBOrigin3.Location = new System.Drawing.Point(51, 22);
            this.rBOrigin3.Name = "rBOrigin3";
            this.rBOrigin3.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin3.TabIndex = 2;
            this.rBOrigin3.UseVisualStyleBackColor = true;
            // 
            // rBOrigin2
            // 
            this.rBOrigin2.AutoSize = true;
            this.rBOrigin2.Location = new System.Drawing.Point(31, 22);
            this.rBOrigin2.Name = "rBOrigin2";
            this.rBOrigin2.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin2.TabIndex = 1;
            this.rBOrigin2.UseVisualStyleBackColor = true;
            // 
            // rBOrigin1
            // 
            this.rBOrigin1.AutoSize = true;
            this.rBOrigin1.Location = new System.Drawing.Point(11, 22);
            this.rBOrigin1.Name = "rBOrigin1";
            this.rBOrigin1.Size = new System.Drawing.Size(14, 13);
            this.rBOrigin1.TabIndex = 0;
            this.rBOrigin1.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(305, 265);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // GCodeFromShape
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 291);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(400, 330);
            this.Name = "GCodeFromShape";
            this.Text = "Create GCode from Shapes";
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
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDShapeR)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
    }
}