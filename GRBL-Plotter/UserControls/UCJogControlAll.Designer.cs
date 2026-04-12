using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCJogControlAll
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCJogControlAll));
            this.GbJogControl = new System.Windows.Forms.GroupBox();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.LblSetupHeadline = new System.Windows.Forms.Label();
            this.userControlJogControludC = new GrblPlotter.UserControls.UCJogControlUD();
            this.userControlJogControludB = new GrblPlotter.UserControls.UCJogControlUD();
            this.userControlJogControludA = new GrblPlotter.UserControls.UCJogControlUD();
            this.userControlJogControludZ = new GrblPlotter.UserControls.UCJogControlUD();
            this.userControlJogControlXY = new GrblPlotter.UserControls.UCJogControlXY();
            this.GbJogControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // GbJogControl
            // 
            this.GbJogControl.Controls.Add(this.LblSetupHeadline);
            this.GbJogControl.Controls.Add(this.BtnSetup);
            this.GbJogControl.Controls.Add(this.userControlJogControludC);
            this.GbJogControl.Controls.Add(this.userControlJogControludB);
            this.GbJogControl.Controls.Add(this.userControlJogControludA);
            this.GbJogControl.Controls.Add(this.userControlJogControludZ);
            this.GbJogControl.Controls.Add(this.userControlJogControlXY);
            resources.ApplyResources(this.GbJogControl, "GbJogControl");
            this.GbJogControl.Name = "GbJogControl";
            this.GbJogControl.TabStop = false;
            this.toolTip1.SetToolTip(this.GbJogControl, resources.GetString("GbJogControl.ToolTip"));
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.Tag = "";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // LblSetupHeadline
            // 
            resources.ApplyResources(this.LblSetupHeadline, "LblSetupHeadline");
            this.LblSetupHeadline.Name = "LblSetupHeadline";
            // 
            // userControlJogControludC
            // 
            resources.ApplyResources(this.userControlJogControludC, "userControlJogControludC");
            this.userControlJogControludC.Name = "userControlJogControludC";
            // 
            // userControlJogControludB
            // 
            resources.ApplyResources(this.userControlJogControludB, "userControlJogControludB");
            this.userControlJogControludB.Name = "userControlJogControludB";
            // 
            // userControlJogControludA
            // 
            resources.ApplyResources(this.userControlJogControludA, "userControlJogControludA");
            this.userControlJogControludA.Name = "userControlJogControludA";
            // 
            // userControlJogControludZ
            // 
            resources.ApplyResources(this.userControlJogControludZ, "userControlJogControludZ");
            this.userControlJogControludZ.Name = "userControlJogControludZ";
            // 
            // userControlJogControlXY
            // 
            resources.ApplyResources(this.userControlJogControlXY, "userControlJogControlXY");
            this.userControlJogControlXY.Name = "userControlJogControlXY";
            // 
            // UCJogControlAll
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbJogControl);
            this.Name = "UCJogControlAll";
            this.Load += new System.EventHandler(this.UCJogControlAll_Load);
            this.Resize += new System.EventHandler(this.UCJogControlAll_Resize);
            this.GbJogControl.ResumeLayout(false);
            this.GbJogControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox GbJogControl;
        private UCJogControlUD userControlJogControludC;
        private UCJogControlUD userControlJogControludB;
        private UCJogControlUD userControlJogControludA;
        private UCJogControlUD userControlJogControludZ;
        private UCJogControlXY userControlJogControlXY;
        private Button BtnSetup;
        private ToolTip toolTip1;
        private Label LblSetupHeadline;
    }
}
