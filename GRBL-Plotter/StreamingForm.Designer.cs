namespace GRBL_Plotter
{
    partial class StreamingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreamingForm));
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
            this.cBOverrideFREnable.AutoSize = true;
            this.cBOverrideFREnable.Location = new System.Drawing.Point(6, 42);
            this.cBOverrideFREnable.Name = "cBOverrideFREnable";
            this.cBOverrideFREnable.Size = new System.Drawing.Size(66, 17);
            this.cBOverrideFREnable.TabIndex = 0;
            this.cBOverrideFREnable.Text = "Override";
            this.toolTip1.SetToolTip(this.cBOverrideFREnable, "Apply new feed rate value and replace all upcomming feed rates in gcode.\r\nNote: o" +
        "verride takes effect after the current GRBL buffer is processed.");
            this.cBOverrideFREnable.UseVisualStyleBackColor = true;
            this.cBOverrideFREnable.CheckedChanged += new System.EventHandler(this.cBOverrideFREnable_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(223, 67);
            this.label1.TabIndex = 2;
            this.label1.Text = "GRBL Vers. 0.9:\r\n\'Override\' will inject feed rate (F) or spindle speed (S) comman" +
    "d immediately.\r\nAlso later upcoming F or S commands will be replaced by override" +
    "-value.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblOverrideFRValue);
            this.groupBox1.Controls.Add(this.lblFRValue);
            this.groupBox1.Controls.Add(this.cBOverrideFREnable);
            this.groupBox1.Controls.Add(this.nUDOverrideFRBtm);
            this.groupBox1.Controls.Add(this.nUDOverrideFRTop);
            this.groupBox1.Controls.Add(this.tBOverrideFR);
            this.groupBox1.Location = new System.Drawing.Point(3, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(100, 267);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Feed Rate";
            // 
            // lblOverrideFRValue
            // 
            this.lblOverrideFRValue.AutoSize = true;
            this.lblOverrideFRValue.Location = new System.Drawing.Point(36, 62);
            this.lblOverrideFRValue.Name = "lblOverrideFRValue";
            this.lblOverrideFRValue.Size = new System.Drawing.Size(25, 13);
            this.lblOverrideFRValue.TabIndex = 6;
            this.lblOverrideFRValue.Text = "000";
            this.toolTip1.SetToolTip(this.lblOverrideFRValue, "New feed rate value");
            // 
            // lblFRValue
            // 
            this.lblFRValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFRValue.Location = new System.Drawing.Point(6, 16);
            this.lblFRValue.Name = "lblFRValue";
            this.lblFRValue.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblFRValue.Size = new System.Drawing.Size(65, 23);
            this.lblFRValue.TabIndex = 5;
            this.lblFRValue.Text = "00000";
            this.toolTip1.SetToolTip(this.lblFRValue, "Last feed rate sent to GRBL");
            // 
            // nUDOverrideFRBtm
            // 
            this.nUDOverrideFRBtm.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideFRBtm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDOverrideFRBtm.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDOverrideFRBtm.Location = new System.Drawing.Point(6, 241);
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
            this.nUDOverrideFRBtm.Size = new System.Drawing.Size(65, 20);
            this.nUDOverrideFRBtm.TabIndex = 3;
            this.nUDOverrideFRBtm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDOverrideFRBtm, "Set lower limit for track bar");
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
            this.nUDOverrideFRTop.Location = new System.Drawing.Point(6, 85);
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
            this.nUDOverrideFRTop.Size = new System.Drawing.Size(65, 20);
            this.nUDOverrideFRTop.TabIndex = 4;
            this.nUDOverrideFRTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDOverrideFRTop, "Set upper limit for track bar");
            this.nUDOverrideFRTop.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideFRTop;
            this.nUDOverrideFRTop.ValueChanged += new System.EventHandler(this.nUDOverrideFRTop_ValueChanged);
            // 
            // tBOverrideFR
            // 
            this.tBOverrideFR.AutoSize = false;
            this.tBOverrideFR.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideFRValue", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBOverrideFR.LargeChange = 100;
            this.tBOverrideFR.Location = new System.Drawing.Point(6, 111);
            this.tBOverrideFR.Maximum = 10000;
            this.tBOverrideFR.Minimum = 100;
            this.tBOverrideFR.Name = "tBOverrideFR";
            this.tBOverrideFR.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tBOverrideFR.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tBOverrideFR.Size = new System.Drawing.Size(65, 124);
            this.tBOverrideFR.SmallChange = 10;
            this.tBOverrideFR.TabIndex = 1;
            this.tBOverrideFR.TickFrequency = 100;
            this.tBOverrideFR.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.toolTip1.SetToolTip(this.tBOverrideFR, "Set feed rate override value");
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
            this.groupBox2.Location = new System.Drawing.Point(122, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(100, 267);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Spindle Speed";
            // 
            // lblOverrideSSValue
            // 
            this.lblOverrideSSValue.AutoSize = true;
            this.lblOverrideSSValue.Location = new System.Drawing.Point(36, 62);
            this.lblOverrideSSValue.Name = "lblOverrideSSValue";
            this.lblOverrideSSValue.Size = new System.Drawing.Size(25, 13);
            this.lblOverrideSSValue.TabIndex = 7;
            this.lblOverrideSSValue.Text = "000";
            this.toolTip1.SetToolTip(this.lblOverrideSSValue, "New spindle speed value");
            // 
            // lblSSValue
            // 
            this.lblSSValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSSValue.Location = new System.Drawing.Point(11, 16);
            this.lblSSValue.Name = "lblSSValue";
            this.lblSSValue.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblSSValue.Size = new System.Drawing.Size(65, 23);
            this.lblSSValue.TabIndex = 7;
            this.lblSSValue.Text = "00000";
            this.toolTip1.SetToolTip(this.lblSSValue, "Last spindle speed sent to GRBL");
            // 
            // cBOverrideSSEnable
            // 
            this.cBOverrideSSEnable.AutoSize = true;
            this.cBOverrideSSEnable.Location = new System.Drawing.Point(6, 42);
            this.cBOverrideSSEnable.Name = "cBOverrideSSEnable";
            this.cBOverrideSSEnable.Size = new System.Drawing.Size(66, 17);
            this.cBOverrideSSEnable.TabIndex = 0;
            this.cBOverrideSSEnable.Text = "Override";
            this.toolTip1.SetToolTip(this.cBOverrideSSEnable, "Apply new spindle speed value and replace all upcomming spindle speeds in gcode\r\n" +
        "Note: override takes effect after the current GRBL buffer is processed.");
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
            this.nUDOverrideSSBtm.Location = new System.Drawing.Point(6, 241);
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
            this.nUDOverrideSSBtm.Size = new System.Drawing.Size(65, 20);
            this.nUDOverrideSSBtm.TabIndex = 3;
            this.nUDOverrideSSBtm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDOverrideSSBtm, "Set lower limit for track bar");
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
            this.nUDOverrideSSTop.Location = new System.Drawing.Point(6, 85);
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
            this.nUDOverrideSSTop.Size = new System.Drawing.Size(65, 20);
            this.nUDOverrideSSTop.TabIndex = 4;
            this.nUDOverrideSSTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDOverrideSSTop, "Set upper limit for track bar");
            this.nUDOverrideSSTop.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideSSTop;
            this.nUDOverrideSSTop.ValueChanged += new System.EventHandler(this.nUDOverrideSSTop_ValueChanged);
            // 
            // tBOverrideSS
            // 
            this.tBOverrideSS.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "overrideSSValue", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBOverrideSS.LargeChange = 1000;
            this.tBOverrideSS.Location = new System.Drawing.Point(6, 111);
            this.tBOverrideSS.Maximum = 10000;
            this.tBOverrideSS.Minimum = 100;
            this.tBOverrideSS.Name = "tBOverrideSS";
            this.tBOverrideSS.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tBOverrideSS.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tBOverrideSS.Size = new System.Drawing.Size(45, 124);
            this.tBOverrideSS.SmallChange = 100;
            this.tBOverrideSS.TabIndex = 1;
            this.tBOverrideSS.TickFrequency = 1000;
            this.tBOverrideSS.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.toolTip1.SetToolTip(this.tBOverrideSS, "Set spindle speed override value");
            this.tBOverrideSS.Value = global::GRBL_Plotter.Properties.Settings.Default.overrideSSValue;
            this.tBOverrideSS.Scroll += new System.EventHandler(this.tBOverrideSS_Scroll);
            this.tBOverrideSS.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tBOverrideSS_KeyUp);
            this.tBOverrideSS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tBOverrideSS_MouseUp);
            // 
            // StreamingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 341);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StreamingForm";
            this.Text = "Overrides";
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