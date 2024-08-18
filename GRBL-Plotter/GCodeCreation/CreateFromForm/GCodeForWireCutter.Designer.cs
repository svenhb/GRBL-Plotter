namespace GrblPlotter
{
    partial class GCodeForWireCutter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeForWireCutter));
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.NudArrayGapY = new System.Windows.Forms.NumericUpDown();
            this.NudArrayGapX = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NudArrayY = new System.Windows.Forms.NumericUpDown();
            this.NudArrayX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.NudCRingSegments = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.NudCRingOut = new System.Windows.Forms.NumericUpDown();
            this.NudCRingIn = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.NudToolDiameter = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayGapY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayGapX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayX)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudCRingSegments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCRingOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCRingIn)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudToolDiameter)).BeginInit();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.NudArrayGapY);
            this.groupBox1.Controls.Add(this.NudArrayGapX);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.NudArrayY);
            this.groupBox1.Controls.Add(this.NudArrayX);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // NudArrayGapY
            // 
            this.NudArrayGapY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterGapY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudArrayGapY.DecimalPlaces = 1;
            this.NudArrayGapY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudArrayGapY, "NudArrayGapY");
            this.NudArrayGapY.Name = "NudArrayGapY";
            this.toolTip1.SetToolTip(this.NudArrayGapY, resources.GetString("NudArrayGapY.ToolTip"));
            this.NudArrayGapY.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterGapY;
            // 
            // NudArrayGapX
            // 
            this.NudArrayGapX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterGapX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudArrayGapX.DecimalPlaces = 1;
            this.NudArrayGapX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudArrayGapX, "NudArrayGapX");
            this.NudArrayGapX.Name = "NudArrayGapX";
            this.toolTip1.SetToolTip(this.NudArrayGapX, resources.GetString("NudArrayGapX.ToolTip"));
            this.NudArrayGapX.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterGapX;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // NudArrayY
            // 
            this.NudArrayY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterArrayY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NudArrayY, "NudArrayY");
            this.NudArrayY.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.NudArrayY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudArrayY.Name = "NudArrayY";
            this.toolTip1.SetToolTip(this.NudArrayY, resources.GetString("NudArrayY.ToolTip"));
            this.NudArrayY.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterArrayY;
            // 
            // NudArrayX
            // 
            this.NudArrayX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterArrayX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NudArrayX, "NudArrayX");
            this.NudArrayX.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.NudArrayX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudArrayX.Name = "NudArrayX";
            this.toolTip1.SetToolTip(this.NudArrayX, resources.GetString("NudArrayX.ToolTip"));
            this.NudArrayX.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterArrayX;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.NudCRingSegments);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.NudCRingOut);
            this.groupBox2.Controls.Add(this.NudCRingIn);
            this.groupBox2.Controls.Add(this.label6);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // NudCRingSegments
            // 
            this.NudCRingSegments.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterRingSections", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NudCRingSegments, "NudCRingSegments");
            this.NudCRingSegments.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.NudCRingSegments.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NudCRingSegments.Name = "NudCRingSegments";
            this.toolTip1.SetToolTip(this.NudCRingSegments, resources.GetString("NudCRingSegments.ToolTip"));
            this.NudCRingSegments.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterRingSections;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NudCRingOut
            // 
            this.NudCRingOut.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterRingDiameterOut", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudCRingOut.DecimalPlaces = 1;
            this.NudCRingOut.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.NudCRingOut, "NudCRingOut");
            this.NudCRingOut.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudCRingOut.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudCRingOut.Name = "NudCRingOut";
            this.toolTip1.SetToolTip(this.NudCRingOut, resources.GetString("NudCRingOut.ToolTip"));
            this.NudCRingOut.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterRingDiameterOut;
            // 
            // NudCRingIn
            // 
            this.NudCRingIn.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterRingDiameterIn", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudCRingIn.DecimalPlaces = 1;
            this.NudCRingIn.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.NudCRingIn, "NudCRingIn");
            this.NudCRingIn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudCRingIn.Name = "NudCRingIn";
            this.toolTip1.SetToolTip(this.NudCRingIn, resources.GetString("NudCRingIn.ToolTip"));
            this.NudCRingIn.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterRingDiameterIn;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.NudToolDiameter);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // NudToolDiameter
            // 
            this.NudToolDiameter.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "wirecutterToolDiameter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudToolDiameter.DecimalPlaces = 1;
            this.NudToolDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudToolDiameter, "NudToolDiameter");
            this.NudToolDiameter.Name = "NudToolDiameter";
            this.toolTip1.SetToolTip(this.NudToolDiameter, resources.GetString("NudToolDiameter.ToolTip"));
            this.NudToolDiameter.Value = global::GrblPlotter.Properties.Settings.Default.wirecutterToolDiameter;
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // GCodeForWireCutter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnApply);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::GrblPlotter.Properties.Settings.Default, "locationWireCutterForm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::GrblPlotter.Properties.Settings.Default.locationWireCutterForm;
            this.Name = "GCodeForWireCutter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayGapY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayGapX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudArrayX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudCRingSegments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCRingOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCRingIn)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudToolDiameter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NudArrayX;
        private System.Windows.Forms.NumericUpDown NudArrayY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NudArrayGapY;
        private System.Windows.Forms.NumericUpDown NudArrayGapX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NudCRingOut;
        private System.Windows.Forms.NumericUpDown NudCRingIn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown NudCRingSegments;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown NudToolDiameter;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}