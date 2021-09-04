
namespace GrblPlotter.GUI
{
    partial class MainFormStreamSection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFormStreamSection));
            this.NudStreamStart = new System.Windows.Forms.NumericUpDown();
            this.NudStreamStop = new System.Windows.Forms.NumericUpDown();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.BtnSetMax = new System.Windows.Forms.Button();
            this.BtnSetMin = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NudStreamStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudStreamStop)).BeginInit();
            this.SuspendLayout();
            // 
            // NudStreamStart
            // 
            resources.ApplyResources(this.NudStreamStart, "NudStreamStart");
            this.NudStreamStart.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NudStreamStart.Name = "NudStreamStart";
            this.NudStreamStart.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudStreamStart.ValueChanged += new System.EventHandler(this.NudStreamStart_ValueChanged);
            // 
            // NudStreamStop
            // 
            resources.ApplyResources(this.NudStreamStop, "NudStreamStop");
            this.NudStreamStop.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NudStreamStop.Name = "NudStreamStop";
            this.NudStreamStop.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // BtnStart
            // 
            this.BtnStart.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.BtnStart, "BtnStart");
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.BtnCancel, "BtnCancel");
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lblPosition
            // 
            resources.ApplyResources(this.lblPosition, "lblPosition");
            this.lblPosition.Name = "lblPosition";
            // 
            // BtnSetMax
            // 
            resources.ApplyResources(this.BtnSetMax, "BtnSetMax");
            this.BtnSetMax.Name = "BtnSetMax";
            this.BtnSetMax.UseVisualStyleBackColor = true;
            this.BtnSetMax.Click += new System.EventHandler(this.BtnSetMax_Click);
            // 
            // BtnSetMin
            // 
            resources.ApplyResources(this.BtnSetMin, "BtnSetMin");
            this.BtnSetMin.Name = "BtnSetMin";
            this.BtnSetMin.UseVisualStyleBackColor = true;
            this.BtnSetMin.Click += new System.EventHandler(this.BtnSetMin_Click);
            // 
            // MainFormStreamSection
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BtnSetMin);
            this.Controls.Add(this.BtnSetMax);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.NudStreamStop);
            this.Controls.Add(this.NudStreamStart);
            this.Name = "MainFormStreamSection";
            ((System.ComponentModel.ISupportInitialize)(this.NudStreamStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudStreamStop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NudStreamStart;
        private System.Windows.Forms.NumericUpDown NudStreamStop;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Button BtnSetMax;
        private System.Windows.Forms.Button BtnSetMin;
    }
}