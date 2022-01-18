namespace GrblPlotter
{
    partial class ControlSetupUseCase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlSetupUseCase));
            this.tBUseCaseInfo = new System.Windows.Forms.TextBox();
            this.lBUseCase = new System.Windows.Forms.ListBox();
            this.BtnLoad = new System.Windows.Forms.Button();
            this.BtnOk = new System.Windows.Forms.Button();
            this.tBSetup = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LblUseCaseHeader = new System.Windows.Forms.Label();
            this.lblLastUseCase = new System.Windows.Forms.Label();
            this.cBshowImportDialog = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tBUseCaseInfo
            // 
            this.tBUseCaseInfo.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.tBUseCaseInfo, "tBUseCaseInfo");
            this.tBUseCaseInfo.Name = "tBUseCaseInfo";
            this.tBUseCaseInfo.ReadOnly = true;
            // 
            // lBUseCase
            // 
            this.lBUseCase.FormattingEnabled = true;
            resources.ApplyResources(this.lBUseCase, "lBUseCase");
            this.lBUseCase.Name = "lBUseCase";
            this.lBUseCase.SelectedIndexChanged += new System.EventHandler(this.LbUseCase_SelectedIndexChanged);
            // 
            // BtnLoad
            // 
            resources.ApplyResources(this.BtnLoad, "BtnLoad");
            this.BtnLoad.Name = "BtnLoad";
            this.BtnLoad.UseVisualStyleBackColor = true;
            this.BtnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // BtnOk
            // 
            this.BtnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            resources.ApplyResources(this.BtnOk, "BtnOk");
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.UseVisualStyleBackColor = false;
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // tBSetup
            // 
            this.tBSetup.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.tBSetup, "tBSetup");
            this.tBSetup.Name = "tBSetup";
            this.tBSetup.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // LblUseCaseHeader
            // 
            resources.ApplyResources(this.LblUseCaseHeader, "LblUseCaseHeader");
            this.LblUseCaseHeader.Name = "LblUseCaseHeader";
            // 
            // lblLastUseCase
            // 
            resources.ApplyResources(this.lblLastUseCase, "lblLastUseCase");
            this.lblLastUseCase.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "useCaseLastLoaded", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.lblLastUseCase.Name = "lblLastUseCase";
            this.lblLastUseCase.Text = global::GrblPlotter.Properties.Settings.Default.useCaseLastLoaded;
            // 
            // cBshowImportDialog
            // 
            resources.ApplyResources(this.cBshowImportDialog, "cBshowImportDialog");
            this.cBshowImportDialog.Checked = global::GrblPlotter.Properties.Settings.Default.importShowUseCaseDialog;
            this.cBshowImportDialog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBshowImportDialog.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importShowUseCaseDialog", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBshowImportDialog.Name = "cBshowImportDialog";
            this.cBshowImportDialog.UseVisualStyleBackColor = true;
            // 
            // ControlSetupUseCase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LblUseCaseHeader);
            this.Controls.Add(this.lblLastUseCase);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tBSetup);
            this.Controls.Add(this.BtnOk);
            this.Controls.Add(this.BtnLoad);
            this.Controls.Add(this.tBUseCaseInfo);
            this.Controls.Add(this.lBUseCase);
            this.Controls.Add(this.cBshowImportDialog);
            this.Name = "ControlSetupUseCase";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlSetupUseCase_FormClosing);
            this.Load += new System.EventHandler(this.ControlSetupUseCase_Load);
            this.SizeChanged += new System.EventHandler(this.ControlSetupUseCase_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cBshowImportDialog;
        private System.Windows.Forms.TextBox tBUseCaseInfo;
        private System.Windows.Forms.ListBox lBUseCase;
        private System.Windows.Forms.Button BtnLoad;
        private System.Windows.Forms.Button BtnOk;
        private System.Windows.Forms.TextBox tBSetup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLastUseCase;
        private System.Windows.Forms.Label LblUseCaseHeader;
    }
}