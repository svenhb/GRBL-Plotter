using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCMoveToGraphic
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCMoveToGraphic));
            this.Btn1 = new System.Windows.Forms.Button();
            this.Btn2 = new System.Windows.Forms.Button();
            this.Btn3 = new System.Windows.Forms.Button();
            this.Btn4 = new System.Windows.Forms.Button();
            this.Btn5 = new System.Windows.Forms.Button();
            this.Btn6 = new System.Windows.Forms.Button();
            this.Btn7 = new System.Windows.Forms.Button();
            this.Btn8 = new System.Windows.Forms.Button();
            this.Btn9 = new System.Windows.Forms.Button();
            this.GbGraphic = new System.Windows.Forms.GroupBox();
            this.NudFeed = new System.Windows.Forms.NumericUpDown();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.BtnFraming = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.PanelTranslation = new System.Windows.Forms.Panel();
            this.LblSetupHeadline = new System.Windows.Forms.Label();
            this.LblSetupOption2 = new System.Windows.Forms.Label();
            this.LblSetupOption3 = new System.Windows.Forms.Label();
            this.LblSetupOption4 = new System.Windows.Forms.Label();
            this.GbGraphic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudFeed)).BeginInit();
            this.PanelTranslation.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn1
            // 
            resources.ApplyResources(this.Btn1, "Btn1");
            this.Btn1.Name = "Btn1";
            this.toolTip1.SetToolTip(this.Btn1, resources.GetString("Btn1.ToolTip"));
            this.Btn1.UseVisualStyleBackColor = true;
            this.Btn1.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn2
            // 
            resources.ApplyResources(this.Btn2, "Btn2");
            this.Btn2.Name = "Btn2";
            this.toolTip1.SetToolTip(this.Btn2, resources.GetString("Btn2.ToolTip"));
            this.Btn2.UseVisualStyleBackColor = true;
            this.Btn2.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn3
            // 
            this.Btn3.BackgroundImage = global::GrblPlotter.Properties.Resources.a_de;
            resources.ApplyResources(this.Btn3, "Btn3");
            this.Btn3.Name = "Btn3";
            this.toolTip1.SetToolTip(this.Btn3, resources.GetString("Btn3.ToolTip"));
            this.Btn3.UseVisualStyleBackColor = true;
            this.Btn3.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn4
            // 
            resources.ApplyResources(this.Btn4, "Btn4");
            this.Btn4.Name = "Btn4";
            this.toolTip1.SetToolTip(this.Btn4, resources.GetString("Btn4.ToolTip"));
            this.Btn4.UseVisualStyleBackColor = true;
            this.Btn4.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn5
            // 
            resources.ApplyResources(this.Btn5, "Btn5");
            this.Btn5.Name = "Btn5";
            this.toolTip1.SetToolTip(this.Btn5, resources.GetString("Btn5.ToolTip"));
            this.Btn5.UseVisualStyleBackColor = true;
            this.Btn5.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn6
            // 
            this.Btn6.BackgroundImage = global::GrblPlotter.Properties.Resources.a_re;
            resources.ApplyResources(this.Btn6, "Btn6");
            this.Btn6.Name = "Btn6";
            this.toolTip1.SetToolTip(this.Btn6, resources.GetString("Btn6.ToolTip"));
            this.Btn6.UseVisualStyleBackColor = true;
            this.Btn6.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn7
            // 
            resources.ApplyResources(this.Btn7, "Btn7");
            this.Btn7.Name = "Btn7";
            this.toolTip1.SetToolTip(this.Btn7, resources.GetString("Btn7.ToolTip"));
            this.Btn7.UseVisualStyleBackColor = true;
            this.Btn7.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn8
            // 
            resources.ApplyResources(this.Btn8, "Btn8");
            this.Btn8.Name = "Btn8";
            this.toolTip1.SetToolTip(this.Btn8, resources.GetString("Btn8.ToolTip"));
            this.Btn8.UseVisualStyleBackColor = true;
            this.Btn8.Click += new System.EventHandler(this.Btn_Click);
            // 
            // Btn9
            // 
            resources.ApplyResources(this.Btn9, "Btn9");
            this.Btn9.Name = "Btn9";
            this.toolTip1.SetToolTip(this.Btn9, resources.GetString("Btn9.ToolTip"));
            this.Btn9.UseVisualStyleBackColor = true;
            this.Btn9.Click += new System.EventHandler(this.Btn_Click);
            // 
            // GbGraphic
            // 
            this.GbGraphic.Controls.Add(this.PanelTranslation);
            this.GbGraphic.Controls.Add(this.NudFeed);
            this.GbGraphic.Controls.Add(this.BtnSetup);
            this.GbGraphic.Controls.Add(this.BtnFraming);
            this.GbGraphic.Controls.Add(this.Btn7);
            this.GbGraphic.Controls.Add(this.Btn9);
            this.GbGraphic.Controls.Add(this.Btn1);
            this.GbGraphic.Controls.Add(this.Btn8);
            this.GbGraphic.Controls.Add(this.Btn2);
            this.GbGraphic.Controls.Add(this.Btn3);
            this.GbGraphic.Controls.Add(this.Btn6);
            this.GbGraphic.Controls.Add(this.Btn4);
            this.GbGraphic.Controls.Add(this.Btn5);
            resources.ApplyResources(this.GbGraphic, "GbGraphic");
            this.GbGraphic.Name = "GbGraphic";
            this.GbGraphic.TabStop = false;
            this.toolTip1.SetToolTip(this.GbGraphic, resources.GetString("GbGraphic.ToolTip"));
            // 
            // NudFeed
            // 
            resources.ApplyResources(this.NudFeed, "NudFeed");
            this.NudFeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "UserControlMoveToGraphicFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudFeed.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NudFeed.Maximum = new decimal(new int[] {
            99900,
            0,
            0,
            0});
            this.NudFeed.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NudFeed.Name = "NudFeed";
            this.toolTip1.SetToolTip(this.NudFeed, resources.GetString("NudFeed.ToolTip"));
            this.NudFeed.Value = global::GrblPlotter.Properties.Settings.Default.UserControlMoveToGraphicFeed;
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // BtnFraming
            // 
            this.BtnFraming.BackgroundImage = global::GrblPlotter.Properties.Resources.framing;
            resources.ApplyResources(this.BtnFraming, "BtnFraming");
            this.BtnFraming.Name = "BtnFraming";
            this.toolTip1.SetToolTip(this.BtnFraming, resources.GetString("BtnFraming.ToolTip"));
            this.BtnFraming.UseVisualStyleBackColor = true;
            this.BtnFraming.Click += new System.EventHandler(this.BtnFraming_Click);
            // 
            // PanelTranslation
            // 
            this.PanelTranslation.Controls.Add(this.LblSetupOption4);
            this.PanelTranslation.Controls.Add(this.LblSetupOption3);
            this.PanelTranslation.Controls.Add(this.LblSetupOption2);
            this.PanelTranslation.Controls.Add(this.LblSetupHeadline);
            resources.ApplyResources(this.PanelTranslation, "PanelTranslation");
            this.PanelTranslation.Name = "PanelTranslation";
            // 
            // LblSetupHeadline
            // 
            resources.ApplyResources(this.LblSetupHeadline, "LblSetupHeadline");
            this.LblSetupHeadline.Name = "LblSetupHeadline";
            // 
            // LblSetupOption2
            // 
            resources.ApplyResources(this.LblSetupOption2, "LblSetupOption2");
            this.LblSetupOption2.Name = "LblSetupOption2";
            // 
            // LblSetupOption3
            // 
            resources.ApplyResources(this.LblSetupOption3, "LblSetupOption3");
            this.LblSetupOption3.Name = "LblSetupOption3";
            // 
            // LblSetupOption4
            // 
            resources.ApplyResources(this.LblSetupOption4, "LblSetupOption4");
            this.LblSetupOption4.Name = "LblSetupOption4";
            // 
            // UCMoveToGraphic
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbGraphic);
            this.Name = "UCMoveToGraphic";
            this.Load += new System.EventHandler(this.UCMoveToGraphic_Load);
            this.Resize += new System.EventHandler(this.UCMoveToGraphic_Resize);
            this.GbGraphic.ResumeLayout(false);
            this.GbGraphic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudFeed)).EndInit();
            this.PanelTranslation.ResumeLayout(false);
            this.PanelTranslation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button Btn1;
        private Button Btn2;
        private Button Btn3;
        private Button Btn4;
        private Button Btn5;
        private Button Btn6;
        private Button Btn7;
        private Button Btn8;
        private Button Btn9;
        private GroupBox GbGraphic;
        private Button BtnFraming;
        private ToolTip toolTip1;
        private Button BtnSetup;
        private NumericUpDown NudFeed;
        private Panel PanelTranslation;
        private Label LblSetupHeadline;
        private Label LblSetupOption4;
        private Label LblSetupOption3;
        private Label LblSetupOption2;
    }
}
