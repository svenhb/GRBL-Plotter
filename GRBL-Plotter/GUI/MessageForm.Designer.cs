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
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // tBInfo
            // 
            resources.ApplyResources(this.tBInfo, "tBInfo");
            this.tBInfo.Name = "tBInfo";
            this.tBInfo.ReadOnly = true;
            // 
            // btnContinue
            // 
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.Yes;
            resources.ApplyResources(this.btnContinue, "btnContinue");
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // ColorPanel
            // 
            this.ColorPanel.BackColor = System.Drawing.Color.Transparent;
            this.ColorPanel.Controls.Add(this.LblHex);
            resources.ApplyResources(this.ColorPanel, "ColorPanel");
            this.ColorPanel.Name = "ColorPanel";
            // 
            // LblHex
            // 
            resources.ApplyResources(this.LblHex, "LblHex");
            this.LblHex.Name = "LblHex";
            // 
            // tBInfo2
            // 
            resources.ApplyResources(this.tBInfo2, "tBInfo2");
            this.tBInfo2.Name = "tBInfo2";
            this.tBInfo2.ReadOnly = true;
            // 
            // MessageForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.ColorPanel);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.tBInfo);
            this.Controls.Add(this.tBInfo2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MessageForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
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