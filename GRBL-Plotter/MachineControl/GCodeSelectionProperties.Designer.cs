
namespace GrblPlotter.MachineControl
{
    partial class GCodeSelectionProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeSelectionProperties));
            this.label5 = new System.Windows.Forms.Label();
            this.NudAttributeWidth = new System.Windows.Forms.NumericUpDown();
            this.BtnAttributeColor = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.NudCenterX = new System.Windows.Forms.NumericUpDown();
            this.NudCenterY = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NudIncrement = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NudAttributeWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudCenterX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCenterY)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudIncrement)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NudAttributeWidth
            // 
            this.NudAttributeWidth.DecimalPlaces = 2;
            this.NudAttributeWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudAttributeWidth, "NudAttributeWidth");
            this.NudAttributeWidth.Name = "NudAttributeWidth";
            this.NudAttributeWidth.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.NudAttributeWidth.ValueChanged += new System.EventHandler(this.NudAttributeWidth_ValueChanged);
            // 
            // BtnAttributeColor
            // 
            resources.ApplyResources(this.BtnAttributeColor, "BtnAttributeColor");
            this.BtnAttributeColor.Name = "BtnAttributeColor";
            this.BtnAttributeColor.UseVisualStyleBackColor = true;
            this.BtnAttributeColor.Click += new System.EventHandler(this.BtnAttributeColor_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.BtnAttributeColor);
            this.groupBox2.Controls.Add(this.NudAttributeWidth);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.BtnCancel, "BtnCancel");
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOK
            // 
            this.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = true;
            // 
            // NudCenterX
            // 
            this.NudCenterX.DecimalPlaces = 3;
            this.NudCenterX.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NudCenterX, "NudCenterX");
            this.NudCenterX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudCenterX.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.NudCenterX.Name = "NudCenterX";
            this.NudCenterX.ValueChanged += new System.EventHandler(this.NudCenterX_ValueChanged);
            // 
            // NudCenterY
            // 
            this.NudCenterY.DecimalPlaces = 3;
            this.NudCenterY.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NudCenterY, "NudCenterY");
            this.NudCenterY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudCenterY.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.NudCenterY.Name = "NudCenterY";
            this.NudCenterY.ValueChanged += new System.EventHandler(this.NudCenterX_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NudCenterX);
            this.groupBox1.Controls.Add(this.NudCenterY);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // NudIncrement
            // 
            this.NudIncrement.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "selectionPropertyIncrement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudIncrement.DecimalPlaces = 3;
            resources.ApplyResources(this.NudIncrement, "NudIncrement");
            this.NudIncrement.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NudIncrement.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NudIncrement.Name = "NudIncrement";
            this.NudIncrement.Value = global::GrblPlotter.Properties.Settings.Default.selectionPropertyIncrement;
            this.NudIncrement.ValueChanged += new System.EventHandler(this.NudIncrement_ValueChanged);
            // 
            // GCodeSelectionProperties
            // 
            this.AcceptButton = this.BtnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NudIncrement);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GCodeSelectionProperties";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.GCodeSelectionProperties_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NudAttributeWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudCenterX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCenterY)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudIncrement)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.NumericUpDown NudCenterX;
        private System.Windows.Forms.NumericUpDown NudCenterY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NudAttributeWidth;
        private System.Windows.Forms.Button BtnAttributeColor;
        private System.Windows.Forms.NumericUpDown NudIncrement;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}