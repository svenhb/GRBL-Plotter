namespace GrblPlotter.UserControls
{
    partial class UCDeviceRouter2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDeviceRouter2));
            this.CbMist = new System.Windows.Forms.CheckBox();
            this.CbSpindle = new System.Windows.Forms.CheckBox();
            this.CbCoolant = new System.Windows.Forms.CheckBox();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CbMist
            // 
            resources.ApplyResources(this.CbMist, "CbMist");
            this.CbMist.Name = "CbMist";
            this.CbMist.UseVisualStyleBackColor = true;
            this.CbMist.CheckedChanged += new System.EventHandler(this.CbMist_CheckedChanged);
            // 
            // CbSpindle
            // 
            resources.ApplyResources(this.CbSpindle, "CbSpindle");
            this.CbSpindle.Name = "CbSpindle";
            this.CbSpindle.UseVisualStyleBackColor = true;
            this.CbSpindle.CheckedChanged += new System.EventHandler(this.CbSpindle_CheckedChanged);
            // 
            // CbCoolant
            // 
            resources.ApplyResources(this.CbCoolant, "CbCoolant");
            this.CbCoolant.Name = "CbCoolant";
            this.CbCoolant.UseVisualStyleBackColor = true;
            this.CbCoolant.CheckedChanged += new System.EventHandler(this.CbCoolant_CheckedChanged);
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.CbSpindle, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.CbMist, 0, 2);
            this.TableLayoutPanel1.Controls.Add(this.CbCoolant, 0, 1);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // UCDeviceRouter2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.TableLayoutPanel1);
            this.Name = "UCDeviceRouter2";
            this.BackColorChanged += new System.EventHandler(this.UCDeviceRouter2_BackColorChanged);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox CbMist;
        private System.Windows.Forms.CheckBox CbSpindle;
        private System.Windows.Forms.CheckBox CbCoolant;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
    }
}
