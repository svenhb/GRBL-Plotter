namespace GRBL_Plotter
{
    partial class ControlStreamingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlStreamingForm));
            this.cBOverrideFREnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblOverrideFRValue = new System.Windows.Forms.Label();
            this.lblFRValue = new System.Windows.Forms.Label();
            this.nUDOverrideFRBtm = new System.Windows.Forms.NumericUpDown();
            this.nUDOverrideFRTop = new System.Windows.Forms.NumericUpDown();
            this.tBOverrideFR = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblOverrideSSValue = new System.Windows.Forms.Label();
            this.lblSSValue = new System.Windows.Forms.Label();
            this.cBOverrideSSEnable = new System.Windows.Forms.CheckBox();
            this.nUDOverrideSSBtm = new System.Windows.Forms.NumericUpDown();
            this.nUDOverrideSSTop = new System.Windows.Forms.NumericUpDown();
            this.tBOverrideSS = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideFRBtm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideFRTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBOverrideFR)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideSSBtm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideSSTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBOverrideSS)).BeginInit();
            this.SuspendLayout();
            // 
            // cBOverrideFREnable
            // 
            resources.ApplyResources(this.cBOverrideFREnable, "cBOverrideFREnable");
            this.cBOverrideFREnable.Name = "cBOverrideFREnable";
            this.toolTip1.SetToolTip(this.cBOverrideFREnable, resources.GetString("cBOverrideFREnable.ToolTip"));
            this.cBOverrideFREnable.UseVisualStyleBackColor = true;
            this.cBOverrideFREnable.CheckedChanged += new System.EventHandler(this.cBOverrideFREnable_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblOverrideFRValue);
            this.groupBox1.Controls.Add(this.lblFRValue);
            this.groupBox1.Controls.Add(this.cBOverrideFREnable);
            this.groupBox1.Controls.Add(this.nUDOverrideFRBtm);
            this.groupBox1.Controls.Add(this.nUDOverrideFRTop);
            this.groupBox1.Controls.Add(this.tBOverrideFR);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lblOverrideFRValue
            // 
            resources.ApplyResources(this.lblOverrideFRValue, "lblOverrideFRValue");
            this.lblOverrideFRValue.Name = "lblOverrideFRValue";
            this.toolTip1.SetToolTip(this.lblOverrideFRValue, resources.GetString("lblOverrideFRValue.ToolTip"));
            // 
            // lblFRValue
            // 
            resources.ApplyResources(this.lblFRValue, "lblFRValue");
            this.lblFRValue.Name = "lblFRValue";
            this.toolTip1.SetToolTip(this.lblFRValue, resources.GetString("lblFRValue.ToolTip"));
            // 
            // nUDOverrideFRBtm
            // 
            this.nUDOverrideFRBtm.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideFRBtm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOverrideFRBtm.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDOverrideFRBtm, "nUDOverrideFRBtm");
            this.nUDOverrideFRBtm.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDOverrideFRBtm.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDOverrideFRBtm.Name = "nUDOverrideFRBtm";
            this.toolTip1.SetToolTip(this.nUDOverrideFRBtm, resources.GetString("nUDOverrideFRBtm.ToolTip"));
            this.nUDOverrideFRBtm.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideFRBtm;
            this.nUDOverrideFRBtm.ValueChanged += new System.EventHandler(this.nUDOverrideFRBtm_ValueChanged);
            // 
            // nUDOverrideFRTop
            // 
            this.nUDOverrideFRTop.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideFRTop", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOverrideFRTop.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDOverrideFRTop, "nUDOverrideFRTop");
            this.nUDOverrideFRTop.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDOverrideFRTop.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDOverrideFRTop.Name = "nUDOverrideFRTop";
            this.toolTip1.SetToolTip(this.nUDOverrideFRTop, resources.GetString("nUDOverrideFRTop.ToolTip"));
            this.nUDOverrideFRTop.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideFRTop;
            this.nUDOverrideFRTop.ValueChanged += new System.EventHandler(this.nUDOverrideFRTop_ValueChanged);
            // 
            // tBOverrideFR
            // 
            resources.ApplyResources(this.tBOverrideFR, "tBOverrideFR");
            this.tBOverrideFR.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideFRValue", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBOverrideFR.LargeChange = 100;
            this.tBOverrideFR.Maximum = 10000;
            this.tBOverrideFR.Minimum = 100;
            this.tBOverrideFR.Name = "tBOverrideFR";
            this.tBOverrideFR.SmallChange = 10;
            this.tBOverrideFR.TickFrequency = 100;
            this.tBOverrideFR.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.toolTip1.SetToolTip(this.tBOverrideFR, resources.GetString("tBOverrideFR.ToolTip"));
            this.tBOverrideFR.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideFRValue;
            this.tBOverrideFR.Scroll += new System.EventHandler(this.tBOverrideFR_Scroll);
            this.tBOverrideFR.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tBOverrideFR_KeyUp);
            this.tBOverrideFR.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tBOverrideFR_MouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblOverrideSSValue);
            this.groupBox2.Controls.Add(this.lblSSValue);
            this.groupBox2.Controls.Add(this.cBOverrideSSEnable);
            this.groupBox2.Controls.Add(this.nUDOverrideSSBtm);
            this.groupBox2.Controls.Add(this.nUDOverrideSSTop);
            this.groupBox2.Controls.Add(this.tBOverrideSS);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lblOverrideSSValue
            // 
            resources.ApplyResources(this.lblOverrideSSValue, "lblOverrideSSValue");
            this.lblOverrideSSValue.Name = "lblOverrideSSValue";
            this.toolTip1.SetToolTip(this.lblOverrideSSValue, resources.GetString("lblOverrideSSValue.ToolTip"));
            // 
            // lblSSValue
            // 
            resources.ApplyResources(this.lblSSValue, "lblSSValue");
            this.lblSSValue.Name = "lblSSValue";
            this.toolTip1.SetToolTip(this.lblSSValue, resources.GetString("lblSSValue.ToolTip"));
            // 
            // cBOverrideSSEnable
            // 
            resources.ApplyResources(this.cBOverrideSSEnable, "cBOverrideSSEnable");
            this.cBOverrideSSEnable.Name = "cBOverrideSSEnable";
            this.toolTip1.SetToolTip(this.cBOverrideSSEnable, resources.GetString("cBOverrideSSEnable.ToolTip"));
            this.cBOverrideSSEnable.UseVisualStyleBackColor = true;
            this.cBOverrideSSEnable.CheckedChanged += new System.EventHandler(this.cBOverrideSSEnable_CheckedChanged);
            // 
            // nUDOverrideSSBtm
            // 
            this.nUDOverrideSSBtm.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideSSBtm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOverrideSSBtm.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDOverrideSSBtm, "nUDOverrideSSBtm");
            this.nUDOverrideSSBtm.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDOverrideSSBtm.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDOverrideSSBtm.Name = "nUDOverrideSSBtm";
            this.toolTip1.SetToolTip(this.nUDOverrideSSBtm, resources.GetString("nUDOverrideSSBtm.ToolTip"));
            this.nUDOverrideSSBtm.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideSSBtm;
            this.nUDOverrideSSBtm.ValueChanged += new System.EventHandler(this.nUDOverrideSSBtm_ValueChanged);
            // 
            // nUDOverrideSSTop
            // 
            this.nUDOverrideSSTop.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideSSTop", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOverrideSSTop.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDOverrideSSTop, "nUDOverrideSSTop");
            this.nUDOverrideSSTop.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDOverrideSSTop.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDOverrideSSTop.Name = "nUDOverrideSSTop";
            this.toolTip1.SetToolTip(this.nUDOverrideSSTop, resources.GetString("nUDOverrideSSTop.ToolTip"));
            this.nUDOverrideSSTop.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideSSTop;
            this.nUDOverrideSSTop.ValueChanged += new System.EventHandler(this.nUDOverrideSSTop_ValueChanged);
            // 
            // tBOverrideSS
            // 
            this.tBOverrideSS.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideSSValue", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBOverrideSS.LargeChange = 1000;
            resources.ApplyResources(this.tBOverrideSS, "tBOverrideSS");
            this.tBOverrideSS.Maximum = 10000;
            this.tBOverrideSS.Minimum = 100;
            this.tBOverrideSS.Name = "tBOverrideSS";
            this.tBOverrideSS.SmallChange = 100;
            this.tBOverrideSS.TickFrequency = 1000;
            this.tBOverrideSS.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.toolTip1.SetToolTip(this.tBOverrideSS, resources.GetString("tBOverrideSS.ToolTip"));
            this.tBOverrideSS.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideSSValue;
            this.tBOverrideSS.Scroll += new System.EventHandler(this.tBOverrideSS_Scroll);
            this.tBOverrideSS.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tBOverrideSS_KeyUp);
            this.tBOverrideSS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tBOverrideSS_MouseUp);
            // 
            // ControlStreamingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "ControlStreamingForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlStreamingForm_FormClosing);
            this.Load += new System.EventHandler(this.StreamingForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideFRBtm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideFRTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBOverrideFR)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideSSBtm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDOverrideSSTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBOverrideSS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.CheckBox cBOverrideFREnable;
        private System.Windows.Forms.TrackBar tBOverrideFR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nUDOverrideFRBtm;
        private System.Windows.Forms.NumericUpDown nUDOverrideFRTop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.CheckBox cBOverrideSSEnable;
        private System.Windows.Forms.NumericUpDown nUDOverrideSSBtm;
        private System.Windows.Forms.NumericUpDown nUDOverrideSSTop;
        private System.Windows.Forms.TrackBar tBOverrideSS;
        private System.Windows.Forms.Label lblOverrideFRValue;
        private System.Windows.Forms.Label lblFRValue;
        private System.Windows.Forms.Label lblOverrideSSValue;
        private System.Windows.Forms.Label lblSSValue;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}