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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tBSetup = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblLastUseCase = new System.Windows.Forms.Label();
            this.cBshowImportDialog = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tBUseCaseInfo
            // 
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
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.Color.Yellow;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // tBSetup
            // 
            resources.ApplyResources(this.tBSetup, "tBSetup");
            this.tBSetup.Name = "tBSetup";
            this.tBSetup.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            this.Controls.Add(this.lblLastUseCase);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tBSetup);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.tBUseCaseInfo);
            this.Controls.Add(this.lBUseCase);
            this.Controls.Add(this.cBshowImportDialog);
            this.Name = "ControlSetupUseCase";
            this.Load += new System.EventHandler(this.ControlSetupUseCase_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cBshowImportDialog;
        private System.Windows.Forms.TextBox tBUseCaseInfo;
        private System.Windows.Forms.ListBox lBUseCase;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox tBSetup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLastUseCase;
    }
}