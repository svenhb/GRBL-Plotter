namespace GRBL_Plotter
{
    partial class ControlStreamingForm2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlStreamingForm2));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFRValue = new System.Windows.Forms.Label();
            this.btnOverrideFR0 = new System.Windows.Forms.Button();
            this.btnOverrideFR4 = new System.Windows.Forms.Button();
            this.btnOverrideFR3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOverrideFR2 = new System.Windows.Forms.Button();
            this.btnOverrideFR1 = new System.Windows.Forms.Button();
            this.lblOverrideFRValue = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSSValue = new System.Windows.Forms.Label();
            this.btnOverrideSS0 = new System.Windows.Forms.Button();
            this.btnOverrideSS4 = new System.Windows.Forms.Button();
            this.btnOverrideSS3 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOverrideSS2 = new System.Windows.Forms.Button();
            this.btnOverrideSS1 = new System.Windows.Forms.Button();
            this.lblOverrideSSValue = new System.Windows.Forms.Label();
            this.btnToggleSS = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnToggleMC = new System.Windows.Forms.Button();
            this.btnToggleFC = new System.Windows.Forms.Button();
            this.btnOverrideSD = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblFRValue);
            this.groupBox1.Controls.Add(this.btnOverrideFR0);
            this.groupBox1.Controls.Add(this.btnOverrideFR4);
            this.groupBox1.Controls.Add(this.btnOverrideFR3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnOverrideFR2);
            this.groupBox1.Controls.Add(this.btnOverrideFR1);
            this.groupBox1.Controls.Add(this.lblOverrideFRValue);
            this.groupBox1.Location = new System.Drawing.Point(1, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 195);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Feed Rate";
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Set:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Actual:";
            // 
            // lblFRValue
            // 
            this.lblFRValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFRValue.Location = new System.Drawing.Point(23, 29);
            this.lblFRValue.Name = "lblFRValue";
            this.lblFRValue.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblFRValue.Size = new System.Drawing.Size(65, 23);
            this.lblFRValue.TabIndex = 5;
            this.lblFRValue.Text = "00000";
            // 
            // btnOverrideFR0
            // 
            this.btnOverrideFR0.Location = new System.Drawing.Point(9, 116);
            this.btnOverrideFR0.Name = "btnOverrideFR0";
            this.btnOverrideFR0.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideFR0.TabIndex = 12;
            this.btnOverrideFR0.Text = "set 100%";
            this.btnOverrideFR0.UseVisualStyleBackColor = true;
            this.btnOverrideFR0.Click += new System.EventHandler(this.btnOverrideFR0_Click);
            // 
            // btnOverrideFR4
            // 
            this.btnOverrideFR4.Location = new System.Drawing.Point(9, 164);
            this.btnOverrideFR4.Name = "btnOverrideFR4";
            this.btnOverrideFR4.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideFR4.TabIndex = 11;
            this.btnOverrideFR4.Text = "-10%";
            this.btnOverrideFR4.UseVisualStyleBackColor = true;
            this.btnOverrideFR4.Click += new System.EventHandler(this.btnOverrideFR4_Click);
            // 
            // btnOverrideFR3
            // 
            this.btnOverrideFR3.Location = new System.Drawing.Point(9, 140);
            this.btnOverrideFR3.Name = "btnOverrideFR3";
            this.btnOverrideFR3.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideFR3.TabIndex = 10;
            this.btnOverrideFR3.Text = "-1%";
            this.btnOverrideFR3.UseVisualStyleBackColor = true;
            this.btnOverrideFR3.Click += new System.EventHandler(this.btnOverrideFR3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "%";
            // 
            // btnOverrideFR2
            // 
            this.btnOverrideFR2.Location = new System.Drawing.Point(9, 92);
            this.btnOverrideFR2.Name = "btnOverrideFR2";
            this.btnOverrideFR2.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideFR2.TabIndex = 8;
            this.btnOverrideFR2.Text = "+1%";
            this.btnOverrideFR2.UseVisualStyleBackColor = true;
            this.btnOverrideFR2.Click += new System.EventHandler(this.btnOverrideFR2_Click);
            // 
            // btnOverrideFR1
            // 
            this.btnOverrideFR1.Location = new System.Drawing.Point(9, 68);
            this.btnOverrideFR1.Name = "btnOverrideFR1";
            this.btnOverrideFR1.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideFR1.TabIndex = 7;
            this.btnOverrideFR1.Text = "+10%";
            this.btnOverrideFR1.UseVisualStyleBackColor = true;
            this.btnOverrideFR1.Click += new System.EventHandler(this.btnOverrideFR1_Click);
            // 
            // lblOverrideFRValue
            // 
            this.lblOverrideFRValue.AutoSize = true;
            this.lblOverrideFRValue.Location = new System.Drawing.Point(42, 52);
            this.lblOverrideFRValue.Name = "lblOverrideFRValue";
            this.lblOverrideFRValue.Size = new System.Drawing.Size(25, 13);
            this.lblOverrideFRValue.TabIndex = 6;
            this.lblOverrideFRValue.Text = "000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.lblSSValue);
            this.groupBox2.Controls.Add(this.btnOverrideSS0);
            this.groupBox2.Controls.Add(this.btnOverrideSS4);
            this.groupBox2.Controls.Add(this.btnOverrideSS3);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnOverrideSS2);
            this.groupBox2.Controls.Add(this.btnOverrideSS1);
            this.groupBox2.Controls.Add(this.lblOverrideSSValue);
            this.groupBox2.Location = new System.Drawing.Point(99, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(92, 195);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Spindle Speed";
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Set:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Actual:";
            // 
            // lblSSValue
            // 
            this.lblSSValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSSValue.Location = new System.Drawing.Point(23, 29);
            this.lblSSValue.Name = "lblSSValue";
            this.lblSSValue.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblSSValue.Size = new System.Drawing.Size(65, 23);
            this.lblSSValue.TabIndex = 5;
            this.lblSSValue.Text = "00000";
            // 
            // btnOverrideSS0
            // 
            this.btnOverrideSS0.Location = new System.Drawing.Point(9, 116);
            this.btnOverrideSS0.Name = "btnOverrideSS0";
            this.btnOverrideSS0.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideSS0.TabIndex = 12;
            this.btnOverrideSS0.Text = "set 100%";
            this.btnOverrideSS0.UseVisualStyleBackColor = true;
            this.btnOverrideSS0.Click += new System.EventHandler(this.btnOverrideSS0_Click);
            // 
            // btnOverrideSS4
            // 
            this.btnOverrideSS4.Location = new System.Drawing.Point(9, 164);
            this.btnOverrideSS4.Name = "btnOverrideSS4";
            this.btnOverrideSS4.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideSS4.TabIndex = 11;
            this.btnOverrideSS4.Text = "-10%";
            this.btnOverrideSS4.UseVisualStyleBackColor = true;
            this.btnOverrideSS4.Click += new System.EventHandler(this.btnOverrideSS4_Click);
            // 
            // btnOverrideSS3
            // 
            this.btnOverrideSS3.Location = new System.Drawing.Point(9, 140);
            this.btnOverrideSS3.Name = "btnOverrideSS3";
            this.btnOverrideSS3.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideSS3.TabIndex = 10;
            this.btnOverrideSS3.Text = "-1%";
            this.btnOverrideSS3.UseVisualStyleBackColor = true;
            this.btnOverrideSS3.Click += new System.EventHandler(this.btnOverrideSS3_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(69, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "%";
            // 
            // btnOverrideSS2
            // 
            this.btnOverrideSS2.Location = new System.Drawing.Point(9, 92);
            this.btnOverrideSS2.Name = "btnOverrideSS2";
            this.btnOverrideSS2.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideSS2.TabIndex = 8;
            this.btnOverrideSS2.Text = "+1%";
            this.btnOverrideSS2.UseVisualStyleBackColor = true;
            this.btnOverrideSS2.Click += new System.EventHandler(this.btnOverrideSS2_Click);
            // 
            // btnOverrideSS1
            // 
            this.btnOverrideSS1.Location = new System.Drawing.Point(9, 68);
            this.btnOverrideSS1.Name = "btnOverrideSS1";
            this.btnOverrideSS1.Size = new System.Drawing.Size(75, 23);
            this.btnOverrideSS1.TabIndex = 7;
            this.btnOverrideSS1.Text = "+10%";
            this.btnOverrideSS1.UseVisualStyleBackColor = true;
            this.btnOverrideSS1.Click += new System.EventHandler(this.btnOverrideSS1_Click);
            // 
            // lblOverrideSSValue
            // 
            this.lblOverrideSSValue.AutoSize = true;
            this.lblOverrideSSValue.Location = new System.Drawing.Point(42, 52);
            this.lblOverrideSSValue.Name = "lblOverrideSSValue";
            this.lblOverrideSSValue.Size = new System.Drawing.Size(25, 13);
            this.lblOverrideSSValue.TabIndex = 6;
            this.lblOverrideSSValue.Text = "000";
            // 
            // btnToggleSS
            // 
            this.btnToggleSS.Location = new System.Drawing.Point(6, 67);
            this.btnToggleSS.Name = "btnToggleSS";
            this.btnToggleSS.Size = new System.Drawing.Size(86, 38);
            this.btnToggleSS.TabIndex = 16;
            this.btnToggleSS.Text = "Toggle Spindle Stop";
            this.toolTip1.SetToolTip(this.btnToggleSS, resources.GetString("btnToggleSS.ToolTip"));
            this.btnToggleSS.UseVisualStyleBackColor = true;
            this.btnToggleSS.Click += new System.EventHandler(this.btnToggleSS_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnToggleMC);
            this.groupBox3.Controls.Add(this.btnToggleFC);
            this.groupBox3.Controls.Add(this.btnToggleSS);
            this.groupBox3.Location = new System.Drawing.Point(197, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(99, 194);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Toggle";
            // 
            // btnToggleMC
            // 
            this.btnToggleMC.Location = new System.Drawing.Point(6, 150);
            this.btnToggleMC.Name = "btnToggleMC";
            this.btnToggleMC.Size = new System.Drawing.Size(86, 38);
            this.btnToggleMC.TabIndex = 18;
            this.btnToggleMC.Text = "Toggle Mist Coolant";
            this.toolTip1.SetToolTip(this.btnToggleMC, resources.GetString("btnToggleMC.ToolTip"));
            this.btnToggleMC.UseVisualStyleBackColor = true;
            this.btnToggleMC.Click += new System.EventHandler(this.btnToggleMC_Click);
            // 
            // btnToggleFC
            // 
            this.btnToggleFC.Location = new System.Drawing.Point(6, 109);
            this.btnToggleFC.Name = "btnToggleFC";
            this.btnToggleFC.Size = new System.Drawing.Size(86, 38);
            this.btnToggleFC.TabIndex = 17;
            this.btnToggleFC.Text = "Toggle Flood Coolant";
            this.toolTip1.SetToolTip(this.btnToggleFC, resources.GetString("btnToggleFC.ToolTip"));
            this.btnToggleFC.UseVisualStyleBackColor = true;
            this.btnToggleFC.Click += new System.EventHandler(this.btnToggleFC_Click);
            // 
            // btnOverrideSD
            // 
            this.btnOverrideSD.Location = new System.Drawing.Point(10, 202);
            this.btnOverrideSD.Name = "btnOverrideSD";
            this.btnOverrideSD.Size = new System.Drawing.Size(173, 23);
            this.btnOverrideSD.TabIndex = 18;
            this.btnOverrideSD.Text = "Safety Door";
            this.toolTip1.SetToolTip(this.btnOverrideSD, resources.GetString("btnOverrideSD.ToolTip"));
            this.btnOverrideSD.UseVisualStyleBackColor = true;
            this.btnOverrideSD.Click += new System.EventHandler(this.btnOverrideSD_Click);
            // 
            // ControlStreamingForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 231);
            this.Controls.Add(this.btnOverrideSD);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Location = new System.Drawing.Point(50, 200);
            this.MaximumSize = new System.Drawing.Size(320, 270);
            this.MinimumSize = new System.Drawing.Size(320, 270);
            this.Name = "ControlStreamingForm2";
            this.Text = "Overrides GRBL 1.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlStreamingForm2_FormClosing);
            this.Load += new System.EventHandler(this.ControlStreamingForm2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblOverrideFRValue;
        private System.Windows.Forms.Label lblFRValue;
        private System.Windows.Forms.Button btnOverrideFR4;
        private System.Windows.Forms.Button btnOverrideFR3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOverrideFR2;
        private System.Windows.Forms.Button btnOverrideFR1;
        private System.Windows.Forms.Button btnOverrideFR0;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSSValue;
        private System.Windows.Forms.Button btnOverrideSS0;
        private System.Windows.Forms.Button btnOverrideSS4;
        private System.Windows.Forms.Button btnOverrideSS3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOverrideSS2;
        private System.Windows.Forms.Button btnOverrideSS1;
        private System.Windows.Forms.Label lblOverrideSSValue;
        private System.Windows.Forms.Button btnToggleSS;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnToggleMC;
        private System.Windows.Forms.Button btnToggleFC;
        private System.Windows.Forms.Button btnOverrideSD;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}