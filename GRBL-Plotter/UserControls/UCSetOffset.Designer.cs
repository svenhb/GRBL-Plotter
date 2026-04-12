using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCSetOffset
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCSetOffset));
            this.GbOffset = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnShowNud = new System.Windows.Forms.Button();
            this.rBOrigin8 = new System.Windows.Forms.RadioButton();
            this.rBOrigin1 = new System.Windows.Forms.RadioButton();
            this.BtnOffset = new System.Windows.Forms.Button();
            this.rBOrigin2 = new System.Windows.Forms.RadioButton();
            this.NudY = new System.Windows.Forms.NumericUpDown();
            this.rBOrigin3 = new System.Windows.Forms.RadioButton();
            this.NudX = new System.Windows.Forms.NumericUpDown();
            this.rBOrigin4 = new System.Windows.Forms.RadioButton();
            this.lblY = new System.Windows.Forms.Label();
            this.rBOrigin5 = new System.Windows.Forms.RadioButton();
            this.lblX = new System.Windows.Forms.Label();
            this.rBOrigin6 = new System.Windows.Forms.RadioButton();
            this.rBOrigin9 = new System.Windows.Forms.RadioButton();
            this.rBOrigin7 = new System.Windows.Forms.RadioButton();
            this.LblDimension = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.GbOffset.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudX)).BeginInit();
            this.SuspendLayout();
            // 
            // GbOffset
            // 
            this.GbOffset.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.GbOffset, "GbOffset");
            this.GbOffset.Name = "GbOffset";
            this.GbOffset.TabStop = false;
            this.toolTip1.SetToolTip(this.GbOffset, resources.GetString("GbOffset.ToolTip"));
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.LblDimension, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnShowNud);
            this.panel1.Controls.Add(this.rBOrigin8);
            this.panel1.Controls.Add(this.rBOrigin1);
            this.panel1.Controls.Add(this.BtnOffset);
            this.panel1.Controls.Add(this.rBOrigin2);
            this.panel1.Controls.Add(this.NudY);
            this.panel1.Controls.Add(this.rBOrigin3);
            this.panel1.Controls.Add(this.NudX);
            this.panel1.Controls.Add(this.rBOrigin4);
            this.panel1.Controls.Add(this.lblY);
            this.panel1.Controls.Add(this.rBOrigin5);
            this.panel1.Controls.Add(this.lblX);
            this.panel1.Controls.Add(this.rBOrigin6);
            this.panel1.Controls.Add(this.rBOrigin9);
            this.panel1.Controls.Add(this.rBOrigin7);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // BtnShowNud
            // 
            this.BtnShowNud.BackColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.BtnShowNud, "BtnShowNud");
            this.BtnShowNud.Name = "BtnShowNud";
            this.toolTip1.SetToolTip(this.BtnShowNud, resources.GetString("BtnShowNud.ToolTip"));
            this.BtnShowNud.UseVisualStyleBackColor = false;
            this.BtnShowNud.Click += new System.EventHandler(this.BtnShowNud_Click);
            // 
            // rBOrigin8
            // 
            resources.ApplyResources(this.rBOrigin8, "rBOrigin8");
            this.rBOrigin8.Name = "rBOrigin8";
            this.rBOrigin8.UseVisualStyleBackColor = true;
            // 
            // rBOrigin1
            // 
            resources.ApplyResources(this.rBOrigin1, "rBOrigin1");
            this.rBOrigin1.Name = "rBOrigin1";
            this.rBOrigin1.UseVisualStyleBackColor = true;
            // 
            // BtnOffset
            // 
            resources.ApplyResources(this.BtnOffset, "BtnOffset");
            this.BtnOffset.Name = "BtnOffset";
            this.toolTip1.SetToolTip(this.BtnOffset, resources.GetString("BtnOffset.ToolTip"));
            this.BtnOffset.UseVisualStyleBackColor = true;
            this.BtnOffset.Click += new System.EventHandler(this.BtnOffsetApply_Click);
            // 
            // rBOrigin2
            // 
            resources.ApplyResources(this.rBOrigin2, "rBOrigin2");
            this.rBOrigin2.Name = "rBOrigin2";
            this.rBOrigin2.UseVisualStyleBackColor = true;
            // 
            // NudY
            // 
            this.NudY.DecimalPlaces = 3;
            resources.ApplyResources(this.NudY, "NudY");
            this.NudY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.NudY.Name = "NudY";
            this.NudY.ValueChanged += new System.EventHandler(this.numericUpDownOffset_ValueChanged);
            // 
            // rBOrigin3
            // 
            resources.ApplyResources(this.rBOrigin3, "rBOrigin3");
            this.rBOrigin3.Name = "rBOrigin3";
            this.rBOrigin3.UseVisualStyleBackColor = true;
            // 
            // NudX
            // 
            this.NudX.DecimalPlaces = 3;
            resources.ApplyResources(this.NudX, "NudX");
            this.NudX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.NudX.Name = "NudX";
            this.NudX.ValueChanged += new System.EventHandler(this.numericUpDownOffset_ValueChanged);
            // 
            // rBOrigin4
            // 
            resources.ApplyResources(this.rBOrigin4, "rBOrigin4");
            this.rBOrigin4.Name = "rBOrigin4";
            this.rBOrigin4.UseVisualStyleBackColor = true;
            // 
            // lblY
            // 
            resources.ApplyResources(this.lblY, "lblY");
            this.lblY.Name = "lblY";
            // 
            // rBOrigin5
            // 
            resources.ApplyResources(this.rBOrigin5, "rBOrigin5");
            this.rBOrigin5.Name = "rBOrigin5";
            this.rBOrigin5.UseVisualStyleBackColor = true;
            // 
            // lblX
            // 
            resources.ApplyResources(this.lblX, "lblX");
            this.lblX.Name = "lblX";
            // 
            // rBOrigin6
            // 
            resources.ApplyResources(this.rBOrigin6, "rBOrigin6");
            this.rBOrigin6.Name = "rBOrigin6";
            this.rBOrigin6.UseVisualStyleBackColor = true;
            // 
            // rBOrigin9
            // 
            resources.ApplyResources(this.rBOrigin9, "rBOrigin9");
            this.rBOrigin9.Name = "rBOrigin9";
            this.rBOrigin9.UseVisualStyleBackColor = true;
            // 
            // rBOrigin7
            // 
            resources.ApplyResources(this.rBOrigin7, "rBOrigin7");
            this.rBOrigin7.Checked = true;
            this.rBOrigin7.Name = "rBOrigin7";
            this.rBOrigin7.TabStop = true;
            this.rBOrigin7.UseVisualStyleBackColor = true;
            // 
            // LblDimension
            // 
            this.LblDimension.BackColor = System.Drawing.Color.LightYellow;
            resources.ApplyResources(this.LblDimension, "LblDimension");
            this.LblDimension.Name = "LblDimension";
            // 
            // UCSetOffset
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbOffset);
            this.Name = "UCSetOffset";
            this.Load += new System.EventHandler(this.UCSetOffset_Load);
            this.GbOffset.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox GbOffset;
        private RadioButton rBOrigin9;
        private RadioButton rBOrigin8;
        private RadioButton rBOrigin7;
        private RadioButton rBOrigin6;
        private RadioButton rBOrigin5;
        private RadioButton rBOrigin4;
        private RadioButton rBOrigin3;
        private RadioButton rBOrigin2;
        private RadioButton rBOrigin1;
        private Button BtnOffset;
        private NumericUpDown NudY;
        private NumericUpDown NudX;
        private Label lblY;
        private Label lblX;
        private Label LblDimension;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        private Button BtnShowNud;
        private ToolTip toolTip1;
    }
}
