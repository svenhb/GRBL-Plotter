namespace GrblPlotter.UserControls
{
    partial class UCSetCoordinateSystem
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCSetCoordinateSystem));
            this.GbSetCoordinate = new System.Windows.Forms.GroupBox();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.TlpButtons = new System.Windows.Forms.TableLayoutPanel();
            this.BtnG54 = new System.Windows.Forms.Button();
            this.BtnG59 = new System.Windows.Forms.Button();
            this.BtnG55 = new System.Windows.Forms.Button();
            this.BtnG58 = new System.Windows.Forms.Button();
            this.BtnG56 = new System.Windows.Forms.Button();
            this.BtnG57 = new System.Windows.Forms.Button();
            this.LblSetupHeadline = new System.Windows.Forms.Label();
            this.GbSetCoordinate.SuspendLayout();
            this.TlpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // GbSetCoordinate
            // 
            this.GbSetCoordinate.Controls.Add(this.LblSetupHeadline);
            this.GbSetCoordinate.Controls.Add(this.BtnSetup);
            this.GbSetCoordinate.Controls.Add(this.TlpButtons);
            resources.ApplyResources(this.GbSetCoordinate, "GbSetCoordinate");
            this.GbSetCoordinate.Name = "GbSetCoordinate";
            this.GbSetCoordinate.TabStop = false;
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // TlpButtons
            // 
            resources.ApplyResources(this.TlpButtons, "TlpButtons");
            this.TlpButtons.Controls.Add(this.BtnG54, 0, 0);
            this.TlpButtons.Controls.Add(this.BtnG59, 2, 1);
            this.TlpButtons.Controls.Add(this.BtnG55, 1, 0);
            this.TlpButtons.Controls.Add(this.BtnG58, 1, 1);
            this.TlpButtons.Controls.Add(this.BtnG56, 2, 0);
            this.TlpButtons.Controls.Add(this.BtnG57, 0, 1);
            this.TlpButtons.Name = "TlpButtons";
            // 
            // BtnG54
            // 
            resources.ApplyResources(this.BtnG54, "BtnG54");
            this.BtnG54.Name = "BtnG54";
            this.BtnG54.Tag = "54";
            this.BtnG54.UseVisualStyleBackColor = true;
            this.BtnG54.Click += new System.EventHandler(this.BtnG5x_Click);
            // 
            // BtnG59
            // 
            resources.ApplyResources(this.BtnG59, "BtnG59");
            this.BtnG59.Name = "BtnG59";
            this.BtnG59.Tag = "59";
            this.BtnG59.UseVisualStyleBackColor = true;
            this.BtnG59.Click += new System.EventHandler(this.BtnG5x_Click);
            // 
            // BtnG55
            // 
            resources.ApplyResources(this.BtnG55, "BtnG55");
            this.BtnG55.Name = "BtnG55";
            this.BtnG55.Tag = "55";
            this.BtnG55.UseVisualStyleBackColor = true;
            this.BtnG55.Click += new System.EventHandler(this.BtnG5x_Click);
            // 
            // BtnG58
            // 
            resources.ApplyResources(this.BtnG58, "BtnG58");
            this.BtnG58.Name = "BtnG58";
            this.BtnG58.Tag = "58";
            this.BtnG58.UseVisualStyleBackColor = true;
            this.BtnG58.Click += new System.EventHandler(this.BtnG5x_Click);
            // 
            // BtnG56
            // 
            resources.ApplyResources(this.BtnG56, "BtnG56");
            this.BtnG56.Name = "BtnG56";
            this.BtnG56.Tag = "56";
            this.BtnG56.UseVisualStyleBackColor = true;
            this.BtnG56.Click += new System.EventHandler(this.BtnG5x_Click);
            // 
            // BtnG57
            // 
            resources.ApplyResources(this.BtnG57, "BtnG57");
            this.BtnG57.Name = "BtnG57";
            this.BtnG57.Tag = "57";
            this.BtnG57.UseVisualStyleBackColor = true;
            this.BtnG57.Click += new System.EventHandler(this.BtnG5x_Click);
            // 
            // LblSetupHeadline
            // 
            resources.ApplyResources(this.LblSetupHeadline, "LblSetupHeadline");
            this.LblSetupHeadline.Name = "LblSetupHeadline";
            // 
            // UCSetCoordinateSystem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GbSetCoordinate);
            this.Name = "UCSetCoordinateSystem";
            this.Load += new System.EventHandler(this.UCSetCoordinateSystem_Load);
            this.Resize += new System.EventHandler(this.UCSetCoordinateSystem_Resize);
            this.GbSetCoordinate.ResumeLayout(false);
            this.GbSetCoordinate.PerformLayout();
            this.TlpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GbSetCoordinate;
        private System.Windows.Forms.Button BtnSetup;
        private System.Windows.Forms.Button BtnG59;
        private System.Windows.Forms.Button BtnG58;
        private System.Windows.Forms.Button BtnG57;
        private System.Windows.Forms.Button BtnG56;
        private System.Windows.Forms.Button BtnG55;
        private System.Windows.Forms.Button BtnG54;
        private System.Windows.Forms.TableLayoutPanel TlpButtons;
        private System.Windows.Forms.Label LblSetupHeadline;
    }
}
