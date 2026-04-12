namespace GrblPlotter.UserControls
{
    partial class UCDeviceLaser2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDeviceLaser2));
            this.BtnSetup = new System.Windows.Forms.Button();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Panel = new System.Windows.Forms.Panel();
            this.CbLaser = new System.Windows.Forms.CheckBox();
            this.CbPilotLaser = new System.Windows.Forms.CheckBox();
            this.CbAirAssist = new System.Windows.Forms.CheckBox();
            this.LblUptime = new System.Windows.Forms.Label();
            this.PbLaser = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TableLayoutPanel1.SuspendLayout();
            this.Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PbLaser)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.toolTip1.SetToolTip(this.BtnSetup, resources.GetString("BtnSetup.ToolTip"));
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.Panel, 0, 1);
            this.TableLayoutPanel1.Controls.Add(this.CbPilotLaser, 0, 2);
            this.TableLayoutPanel1.Controls.Add(this.CbAirAssist, 1, 2);
            this.TableLayoutPanel1.Controls.Add(this.LblUptime, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.PbLaser, 1, 0);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // Panel
            // 
            resources.ApplyResources(this.Panel, "Panel");
            this.Panel.Controls.Add(this.CbLaser);
            this.Panel.Name = "Panel";
            // 
            // CbLaser
            // 
            resources.ApplyResources(this.CbLaser, "CbLaser");
            this.CbLaser.Name = "CbLaser";
            this.toolTip1.SetToolTip(this.CbLaser, resources.GetString("CbLaser.ToolTip"));
            this.CbLaser.UseVisualStyleBackColor = true;
            this.CbLaser.CheckedChanged += new System.EventHandler(this.CbLaser_CheckedChanged);
            // 
            // CbPilotLaser
            // 
            resources.ApplyResources(this.CbPilotLaser, "CbPilotLaser");
            this.CbPilotLaser.Name = "CbPilotLaser";
            this.toolTip1.SetToolTip(this.CbPilotLaser, resources.GetString("CbPilotLaser.ToolTip"));
            this.CbPilotLaser.UseVisualStyleBackColor = true;
            this.CbPilotLaser.CheckedChanged += new System.EventHandler(this.CbPilotLaser_CheckedChanged);
            // 
            // CbAirAssist
            // 
            resources.ApplyResources(this.CbAirAssist, "CbAirAssist");
            this.CbAirAssist.Name = "CbAirAssist";
            this.toolTip1.SetToolTip(this.CbAirAssist, resources.GetString("CbAirAssist.ToolTip"));
            this.CbAirAssist.UseVisualStyleBackColor = true;
            this.CbAirAssist.CheckedChanged += new System.EventHandler(this.CbAirAssist_CheckedChanged);
            // 
            // LblUptime
            // 
            resources.ApplyResources(this.LblUptime, "LblUptime");
            this.LblUptime.Name = "LblUptime";
            this.toolTip1.SetToolTip(this.LblUptime, resources.GetString("LblUptime.ToolTip"));
            // 
            // PbLaser
            // 
            resources.ApplyResources(this.PbLaser, "PbLaser");
            this.PbLaser.Name = "PbLaser";
            this.TableLayoutPanel1.SetRowSpan(this.PbLaser, 2);
            this.PbLaser.TabStop = false;
            // 
            // UCDeviceLaser2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.BtnSetup);
            this.Controls.Add(this.TableLayoutPanel1);
            this.Name = "UCDeviceLaser2";
            this.Load += new System.EventHandler(this.UCDeviceLaser2_Load);
            this.Resize += new System.EventHandler(this.UCDeviceLaser2_Resize);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            this.Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PbLaser)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox PbLaser;
        private System.Windows.Forms.CheckBox CbLaser;
        private System.Windows.Forms.Button BtnSetup;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.CheckBox CbPilotLaser;
        private System.Windows.Forms.CheckBox CbAirAssist;
        private System.Windows.Forms.Label LblUptime;
    }
}
