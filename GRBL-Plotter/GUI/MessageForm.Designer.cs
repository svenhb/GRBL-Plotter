namespace GrblPlotter
{
    partial class MessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
            this.btnClose = new System.Windows.Forms.Button();
            this.tBInfo = new System.Windows.Forms.TextBox();
            this.btnContinue = new System.Windows.Forms.Button();
            this.ColorPanel = new System.Windows.Forms.Panel();
            this.LblHex = new System.Windows.Forms.Label();
            this.tBInfo2 = new System.Windows.Forms.TextBox();
            this.ColorPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(207, 174);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // tBInfo
            // 
            this.tBInfo.Location = new System.Drawing.Point(3, 3);
            this.tBInfo.Multiline = true;
            this.tBInfo.Name = "tBInfo";
            this.tBInfo.ReadOnly = true;
            this.tBInfo.Size = new System.Drawing.Size(279, 119);
            this.tBInfo.TabIndex = 2;
            this.tBInfo.Visible = false;
            // 
            // btnContinue
            // 
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnContinue.Location = new System.Drawing.Point(3, 174);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(160, 23);
            this.btnContinue.TabIndex = 0;
            this.btnContinue.Text = "Continue streaming";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // ColorPanel
            // 
            this.ColorPanel.BackColor = System.Drawing.Color.Transparent;
            this.ColorPanel.Controls.Add(this.LblHex);
            this.ColorPanel.Location = new System.Drawing.Point(12, 128);
            this.ColorPanel.Name = "ColorPanel";
            this.ColorPanel.Size = new System.Drawing.Size(260, 40);
            this.ColorPanel.TabIndex = 5;
            // 
            // LblHex
            // 
            this.LblHex.AutoSize = true;
            this.LblHex.Location = new System.Drawing.Point(12, 14);
            this.LblHex.Name = "LblHex";
            this.LblHex.Size = new System.Drawing.Size(31, 13);
            this.LblHex.TabIndex = 0;
            this.LblHex.Text = "Color";
            // 
            // tBInfo2
            // 
            this.tBInfo2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBInfo2.Location = new System.Drawing.Point(3, 3);
            this.tBInfo2.Multiline = true;
            this.tBInfo2.Name = "tBInfo2";
            this.tBInfo2.ReadOnly = true;
            this.tBInfo2.Size = new System.Drawing.Size(279, 119);
            this.tBInfo2.TabIndex = 6;
            this.tBInfo2.Visible = false;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(284, 201);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.ColorPanel);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.tBInfo);
            this.Controls.Add(this.tBInfo2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MessageForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessageForm";
            this.Load += new System.EventHandler(this.MessageForm_Load);
            this.SizeChanged += new System.EventHandler(this.MessageForm_SizeChanged);
            this.ColorPanel.ResumeLayout(false);
            this.ColorPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox tBInfo;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Panel ColorPanel;
        private System.Windows.Forms.Label LblHex;
        private System.Windows.Forms.TextBox tBInfo2;
    }
}