namespace GrblPlotter.UserControls
{
    partial class UCDevicePlotter2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDevicePlotter2));
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnPenUp = new System.Windows.Forms.Button();
            this.BtnPenDown = new System.Windows.Forms.Button();
            this.BtnPenZero = new System.Windows.Forms.Button();
            this.BtnPenDownUp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.BtnPenUp, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.BtnPenDown, 0, 2);
            this.TableLayoutPanel1.Controls.Add(this.BtnPenZero, 0, 1);
            this.TableLayoutPanel1.Controls.Add(this.BtnPenDownUp, 1, 1);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // BtnPenUp
            // 
            resources.ApplyResources(this.BtnPenUp, "BtnPenUp");
            this.BtnPenUp.Name = "BtnPenUp";
            this.toolTip1.SetToolTip(this.BtnPenUp, resources.GetString("BtnPenUp.ToolTip"));
            this.BtnPenUp.UseVisualStyleBackColor = true;
            this.BtnPenUp.Click += new System.EventHandler(this.BtnPenUp_Click);
            // 
            // BtnPenDown
            // 
            resources.ApplyResources(this.BtnPenDown, "BtnPenDown");
            this.BtnPenDown.Name = "BtnPenDown";
            this.toolTip1.SetToolTip(this.BtnPenDown, resources.GetString("BtnPenDown.ToolTip"));
            this.BtnPenDown.UseVisualStyleBackColor = true;
            this.BtnPenDown.Click += new System.EventHandler(this.BtnPenDown_Click);
            // 
            // BtnPenZero
            // 
            resources.ApplyResources(this.BtnPenZero, "BtnPenZero");
            this.BtnPenZero.Name = "BtnPenZero";
            this.toolTip1.SetToolTip(this.BtnPenZero, resources.GetString("BtnPenZero.ToolTip"));
            this.BtnPenZero.UseVisualStyleBackColor = true;
            this.BtnPenZero.Click += new System.EventHandler(this.BtnPenZero_Click);
            // 
            // BtnPenDownUp
            // 
            resources.ApplyResources(this.BtnPenDownUp, "BtnPenDownUp");
            this.BtnPenDownUp.Name = "BtnPenDownUp";
            this.TableLayoutPanel1.SetRowSpan(this.BtnPenDownUp, 2);
            this.toolTip1.SetToolTip(this.BtnPenDownUp, resources.GetString("BtnPenDownUp.ToolTip"));
            this.BtnPenDownUp.UseVisualStyleBackColor = true;
            this.BtnPenDownUp.Click += new System.EventHandler(this.BtnPenDownUp_Click);
            // 
            // UCDevicePlotter2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.TableLayoutPanel1);
            this.Name = "UCDevicePlotter2";
            this.BackColorChanged += new System.EventHandler(this.UCDevicePlotter2_BackColorChanged);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnPenZero;
        private System.Windows.Forms.Button BtnPenUp;
        private System.Windows.Forms.Button BtnPenDown;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button BtnPenDownUp;
    }
}
