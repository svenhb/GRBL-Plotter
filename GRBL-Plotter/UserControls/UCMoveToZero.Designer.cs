using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCMoveToZero
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCMoveToZero));
            this.BtnX = new System.Windows.Forms.Button();
            this.BtnY = new System.Windows.Forms.Button();
            this.BtnZ = new System.Windows.Forms.Button();
            this.BtnXY = new System.Windows.Forms.Button();
            this.BtnA = new System.Windows.Forms.Button();
            this.BtnB = new System.Windows.Forms.Button();
            this.BtnC = new System.Windows.Forms.Button();
            this.CbG0 = new System.Windows.Forms.CheckBox();
            this.BtnStop = new System.Windows.Forms.Button();
            this.GbZero = new System.Windows.Forms.GroupBox();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.LblSetupHeadline = new System.Windows.Forms.Label();
            this.GbZero.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnX
            // 
            resources.ApplyResources(this.BtnX, "BtnX");
            this.BtnX.Name = "BtnX";
            this.toolTip1.SetToolTip(this.BtnX, resources.GetString("BtnX.ToolTip"));
            this.BtnX.UseVisualStyleBackColor = true;
            this.BtnX.Click += new System.EventHandler(this.BtnX_Click);
            // 
            // BtnY
            // 
            resources.ApplyResources(this.BtnY, "BtnY");
            this.BtnY.Name = "BtnY";
            this.toolTip1.SetToolTip(this.BtnY, resources.GetString("BtnY.ToolTip"));
            this.BtnY.UseVisualStyleBackColor = true;
            this.BtnY.Click += new System.EventHandler(this.BtnY_Click);
            // 
            // BtnZ
            // 
            resources.ApplyResources(this.BtnZ, "BtnZ");
            this.BtnZ.Name = "BtnZ";
            this.toolTip1.SetToolTip(this.BtnZ, resources.GetString("BtnZ.ToolTip"));
            this.BtnZ.UseVisualStyleBackColor = true;
            this.BtnZ.Click += new System.EventHandler(this.BtnZ_Click);
            // 
            // BtnXY
            // 
            resources.ApplyResources(this.BtnXY, "BtnXY");
            this.BtnXY.Name = "BtnXY";
            this.toolTip1.SetToolTip(this.BtnXY, resources.GetString("BtnXY.ToolTip"));
            this.BtnXY.UseVisualStyleBackColor = true;
            this.BtnXY.Click += new System.EventHandler(this.BtnXY_Click);
            // 
            // BtnA
            // 
            resources.ApplyResources(this.BtnA, "BtnA");
            this.BtnA.Name = "BtnA";
            this.toolTip1.SetToolTip(this.BtnA, resources.GetString("BtnA.ToolTip"));
            this.BtnA.UseVisualStyleBackColor = true;
            this.BtnA.Click += new System.EventHandler(this.BtnA_Click);
            // 
            // BtnB
            // 
            resources.ApplyResources(this.BtnB, "BtnB");
            this.BtnB.Name = "BtnB";
            this.toolTip1.SetToolTip(this.BtnB, resources.GetString("BtnB.ToolTip"));
            this.BtnB.UseVisualStyleBackColor = true;
            this.BtnB.Click += new System.EventHandler(this.BtnB_Click);
            // 
            // BtnC
            // 
            resources.ApplyResources(this.BtnC, "BtnC");
            this.BtnC.Name = "BtnC";
            this.toolTip1.SetToolTip(this.BtnC, resources.GetString("BtnC.ToolTip"));
            this.BtnC.UseVisualStyleBackColor = true;
            this.BtnC.Click += new System.EventHandler(this.BtnC_Click);
            // 
            // CbG0
            // 
            resources.ApplyResources(this.CbG0, "CbG0");
            this.CbG0.Name = "CbG0";
            this.toolTip1.SetToolTip(this.CbG0, resources.GetString("CbG0.ToolTip"));
            this.CbG0.UseVisualStyleBackColor = true;
            // 
            // BtnStop
            // 
            resources.ApplyResources(this.BtnStop, "BtnStop");
            this.BtnStop.Name = "BtnStop";
            this.toolTip1.SetToolTip(this.BtnStop, resources.GetString("BtnStop.ToolTip"));
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // GbZero
            // 
            this.GbZero.Controls.Add(this.LblSetupHeadline);
            this.GbZero.Controls.Add(this.BtnSetup);
            this.GbZero.Controls.Add(this.BtnXY);
            this.GbZero.Controls.Add(this.BtnStop);
            this.GbZero.Controls.Add(this.BtnX);
            this.GbZero.Controls.Add(this.CbG0);
            this.GbZero.Controls.Add(this.BtnY);
            this.GbZero.Controls.Add(this.BtnC);
            this.GbZero.Controls.Add(this.BtnZ);
            this.GbZero.Controls.Add(this.BtnB);
            this.GbZero.Controls.Add(this.BtnA);
            resources.ApplyResources(this.GbZero, "GbZero");
            this.GbZero.Name = "GbZero";
            this.GbZero.TabStop = false;
            this.toolTip1.SetToolTip(this.GbZero, resources.GetString("GbZero.ToolTip"));
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // LblSetupHeadline
            // 
            resources.ApplyResources(this.LblSetupHeadline, "LblSetupHeadline");
            this.LblSetupHeadline.Name = "LblSetupHeadline";
            // 
            // UCMoveToZero
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbZero);
            this.Name = "UCMoveToZero";
            this.Load += new System.EventHandler(this.UCMoveToZero_Load);
            this.Resize += new System.EventHandler(this.UCMoveToZero_Resize);
            this.GbZero.ResumeLayout(false);
            this.GbZero.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button BtnX;
        private Button BtnY;
        private Button BtnZ;
        private Button BtnXY;
        private Button BtnA;
        private Button BtnB;
        private Button BtnC;
        private CheckBox CbG0;
        private Button BtnStop;
        private GroupBox GbZero;
        private ToolTip toolTip1;
        private Button BtnSetup;
        private Label LblSetupHeadline;
    }
}
