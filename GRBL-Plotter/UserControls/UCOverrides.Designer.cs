using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCOverrides
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCOverrides));
            this.BtnFeed1 = new System.Windows.Forms.Button();
            this.BtnFeed2 = new System.Windows.Forms.Button();
            this.BtnFeed3 = new System.Windows.Forms.Button();
            this.BtnFeed4 = new System.Windows.Forms.Button();
            this.BtnFeed5 = new System.Windows.Forms.Button();
            this.LblFeedSet0 = new System.Windows.Forms.Label();
            this.LblFeedSet = new System.Windows.Forms.Label();
            this.BtnRapid1 = new System.Windows.Forms.Button();
            this.BtnRapid2 = new System.Windows.Forms.Button();
            this.BtnRapid3 = new System.Windows.Forms.Button();
            this.LblSpindleSet = new System.Windows.Forms.Label();
            this.LblSpindleSet0 = new System.Windows.Forms.Label();
            this.BtnSpindle5 = new System.Windows.Forms.Button();
            this.BtnSpindle4 = new System.Windows.Forms.Button();
            this.BtnSpindle3 = new System.Windows.Forms.Button();
            this.BtnSpindle2 = new System.Windows.Forms.Button();
            this.BtnSpindle1 = new System.Windows.Forms.Button();
            this.LblRapidSet = new System.Windows.Forms.Label();
            this.LblRapidSet0 = new System.Windows.Forms.Label();
            this.GbOverrides = new System.Windows.Forms.GroupBox();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.LblToggle = new System.Windows.Forms.Label();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnOverrideD0 = new System.Windows.Forms.Button();
            this.BtnOverrideD3 = new System.Windows.Forms.Button();
            this.BtnOverrideD1 = new System.Windows.Forms.Button();
            this.BtnOverrideD2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnToggleSpindle = new System.Windows.Forms.Button();
            this.BtnToggleMist = new System.Windows.Forms.Button();
            this.BtnToggleFlood = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.LblSpindleValue = new System.Windows.Forms.Label();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.LblSpindle = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.LblFeedValue = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.LblFeed = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.LblRapid = new System.Windows.Forms.Label();
            this.LblRapidValue = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.PanelTranslation = new System.Windows.Forms.Panel();
            this.LblSetupOption4 = new System.Windows.Forms.Label();
            this.LblSetupOption3 = new System.Windows.Forms.Label();
            this.LblSetupOption2 = new System.Windows.Forms.Label();
            this.LblSetupHeadline = new System.Windows.Forms.Label();
            this.LblSetupOption1 = new System.Windows.Forms.Label();
            this.GbOverrides.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.tableLayoutPanel13.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.panel7.SuspendLayout();
            this.PanelTranslation.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnFeed1
            // 
            resources.ApplyResources(this.BtnFeed1, "BtnFeed1");
            this.BtnFeed1.Name = "BtnFeed1";
            this.BtnFeed1.UseVisualStyleBackColor = true;
            this.BtnFeed1.Click += new System.EventHandler(this.BtnFeed1_Click);
            // 
            // BtnFeed2
            // 
            resources.ApplyResources(this.BtnFeed2, "BtnFeed2");
            this.BtnFeed2.Name = "BtnFeed2";
            this.BtnFeed2.UseVisualStyleBackColor = true;
            this.BtnFeed2.Click += new System.EventHandler(this.BtnFeed2_Click);
            // 
            // BtnFeed3
            // 
            resources.ApplyResources(this.BtnFeed3, "BtnFeed3");
            this.BtnFeed3.Name = "BtnFeed3";
            this.BtnFeed3.UseVisualStyleBackColor = true;
            this.BtnFeed3.Click += new System.EventHandler(this.BtnFeed3_Click);
            // 
            // BtnFeed4
            // 
            resources.ApplyResources(this.BtnFeed4, "BtnFeed4");
            this.BtnFeed4.Name = "BtnFeed4";
            this.BtnFeed4.UseVisualStyleBackColor = true;
            this.BtnFeed4.Click += new System.EventHandler(this.BtnFeed4_Click);
            // 
            // BtnFeed5
            // 
            resources.ApplyResources(this.BtnFeed5, "BtnFeed5");
            this.BtnFeed5.Name = "BtnFeed5";
            this.BtnFeed5.UseVisualStyleBackColor = true;
            this.BtnFeed5.Click += new System.EventHandler(this.BtnFeed5_Click);
            // 
            // LblFeedSet0
            // 
            resources.ApplyResources(this.LblFeedSet0, "LblFeedSet0");
            this.LblFeedSet0.Name = "LblFeedSet0";
            // 
            // LblFeedSet
            // 
            resources.ApplyResources(this.LblFeedSet, "LblFeedSet");
            this.LblFeedSet.Name = "LblFeedSet";
            // 
            // BtnRapid1
            // 
            resources.ApplyResources(this.BtnRapid1, "BtnRapid1");
            this.BtnRapid1.Name = "BtnRapid1";
            this.BtnRapid1.UseVisualStyleBackColor = true;
            this.BtnRapid1.Click += new System.EventHandler(this.BtnRapid1_Click);
            // 
            // BtnRapid2
            // 
            resources.ApplyResources(this.BtnRapid2, "BtnRapid2");
            this.BtnRapid2.Name = "BtnRapid2";
            this.BtnRapid2.UseVisualStyleBackColor = true;
            this.BtnRapid2.Click += new System.EventHandler(this.BtnRapid2_Click);
            // 
            // BtnRapid3
            // 
            resources.ApplyResources(this.BtnRapid3, "BtnRapid3");
            this.BtnRapid3.Name = "BtnRapid3";
            this.BtnRapid3.UseVisualStyleBackColor = true;
            this.BtnRapid3.Click += new System.EventHandler(this.BtnRapid3_Click);
            // 
            // LblSpindleSet
            // 
            resources.ApplyResources(this.LblSpindleSet, "LblSpindleSet");
            this.LblSpindleSet.Name = "LblSpindleSet";
            // 
            // LblSpindleSet0
            // 
            resources.ApplyResources(this.LblSpindleSet0, "LblSpindleSet0");
            this.LblSpindleSet0.Name = "LblSpindleSet0";
            // 
            // BtnSpindle5
            // 
            resources.ApplyResources(this.BtnSpindle5, "BtnSpindle5");
            this.BtnSpindle5.Name = "BtnSpindle5";
            this.BtnSpindle5.UseVisualStyleBackColor = true;
            this.BtnSpindle5.Click += new System.EventHandler(this.BtnSpindle5_Click);
            // 
            // BtnSpindle4
            // 
            resources.ApplyResources(this.BtnSpindle4, "BtnSpindle4");
            this.BtnSpindle4.Name = "BtnSpindle4";
            this.BtnSpindle4.UseVisualStyleBackColor = true;
            this.BtnSpindle4.Click += new System.EventHandler(this.BtnSpindle4_Click);
            // 
            // BtnSpindle3
            // 
            resources.ApplyResources(this.BtnSpindle3, "BtnSpindle3");
            this.BtnSpindle3.Name = "BtnSpindle3";
            this.BtnSpindle3.UseVisualStyleBackColor = true;
            this.BtnSpindle3.Click += new System.EventHandler(this.BtnSpindle3_Click);
            // 
            // BtnSpindle2
            // 
            resources.ApplyResources(this.BtnSpindle2, "BtnSpindle2");
            this.BtnSpindle2.Name = "BtnSpindle2";
            this.BtnSpindle2.UseVisualStyleBackColor = true;
            this.BtnSpindle2.Click += new System.EventHandler(this.BtnSpindle2_Click);
            // 
            // BtnSpindle1
            // 
            resources.ApplyResources(this.BtnSpindle1, "BtnSpindle1");
            this.BtnSpindle1.Name = "BtnSpindle1";
            this.BtnSpindle1.UseVisualStyleBackColor = true;
            this.BtnSpindle1.Click += new System.EventHandler(this.BtnSpindle1_Click);
            // 
            // LblRapidSet
            // 
            resources.ApplyResources(this.LblRapidSet, "LblRapidSet");
            this.LblRapidSet.Name = "LblRapidSet";
            // 
            // LblRapidSet0
            // 
            resources.ApplyResources(this.LblRapidSet0, "LblRapidSet0");
            this.LblRapidSet0.Name = "LblRapidSet0";
            // 
            // GbOverrides
            // 
            this.GbOverrides.Controls.Add(this.PanelTranslation);
            this.GbOverrides.Controls.Add(this.BtnSetup);
            this.GbOverrides.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.GbOverrides, "GbOverrides");
            this.GbOverrides.Name = "GbOverrides";
            this.GbOverrides.TabStop = false;
            this.toolTip1.SetToolTip(this.GbOverrides, resources.GetString("GbOverrides.ToolTip"));
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tableLayoutPanel11);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // tableLayoutPanel11
            // 
            resources.ApplyResources(this.tableLayoutPanel11, "tableLayoutPanel11");
            this.tableLayoutPanel11.Controls.Add(this.LblToggle, 0, 0);
            this.tableLayoutPanel11.Controls.Add(this.tableLayoutPanel12, 0, 2);
            this.tableLayoutPanel11.Controls.Add(this.tableLayoutPanel13, 0, 1);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            // 
            // LblToggle
            // 
            resources.ApplyResources(this.LblToggle, "LblToggle");
            this.LblToggle.Name = "LblToggle";
            // 
            // tableLayoutPanel12
            // 
            resources.ApplyResources(this.tableLayoutPanel12, "tableLayoutPanel12");
            this.tableLayoutPanel12.Controls.Add(this.BtnOverrideD0, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.BtnOverrideD3, 3, 0);
            this.tableLayoutPanel12.Controls.Add(this.BtnOverrideD1, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.BtnOverrideD2, 2, 0);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            // 
            // BtnOverrideD0
            // 
            resources.ApplyResources(this.BtnOverrideD0, "BtnOverrideD0");
            this.BtnOverrideD0.Image = global::GrblPlotter.Properties.Resources.led_off;
            this.BtnOverrideD0.Name = "BtnOverrideD0";
            this.BtnOverrideD0.Tag = "off";
            this.BtnOverrideD0.UseVisualStyleBackColor = true;
            this.BtnOverrideD0.Click += new System.EventHandler(this.BtnOverrideD0_Click);
            // 
            // BtnOverrideD3
            // 
            resources.ApplyResources(this.BtnOverrideD3, "BtnOverrideD3");
            this.BtnOverrideD3.Image = global::GrblPlotter.Properties.Resources.led_off;
            this.BtnOverrideD3.Name = "BtnOverrideD3";
            this.BtnOverrideD3.Tag = "off";
            this.BtnOverrideD3.UseVisualStyleBackColor = true;
            this.BtnOverrideD3.Click += new System.EventHandler(this.BtnOverrideD3_Click);
            // 
            // BtnOverrideD1
            // 
            resources.ApplyResources(this.BtnOverrideD1, "BtnOverrideD1");
            this.BtnOverrideD1.Image = global::GrblPlotter.Properties.Resources.led_off;
            this.BtnOverrideD1.Name = "BtnOverrideD1";
            this.BtnOverrideD1.Tag = "off";
            this.BtnOverrideD1.UseVisualStyleBackColor = true;
            this.BtnOverrideD1.Click += new System.EventHandler(this.BtnOverrideD1_Click);
            // 
            // BtnOverrideD2
            // 
            resources.ApplyResources(this.BtnOverrideD2, "BtnOverrideD2");
            this.BtnOverrideD2.Image = global::GrblPlotter.Properties.Resources.led_off;
            this.BtnOverrideD2.Name = "BtnOverrideD2";
            this.BtnOverrideD2.Tag = "off";
            this.BtnOverrideD2.UseVisualStyleBackColor = true;
            this.BtnOverrideD2.Click += new System.EventHandler(this.BtnOverrideD2_Click);
            // 
            // tableLayoutPanel13
            // 
            resources.ApplyResources(this.tableLayoutPanel13, "tableLayoutPanel13");
            this.tableLayoutPanel13.Controls.Add(this.BtnToggleSpindle, 0, 0);
            this.tableLayoutPanel13.Controls.Add(this.BtnToggleMist, 2, 0);
            this.tableLayoutPanel13.Controls.Add(this.BtnToggleFlood, 1, 0);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            // 
            // BtnToggleSpindle
            // 
            resources.ApplyResources(this.BtnToggleSpindle, "BtnToggleSpindle");
            this.BtnToggleSpindle.Image = global::GrblPlotter.Properties.Resources.led_off;
            this.BtnToggleSpindle.Name = "BtnToggleSpindle";
            this.BtnToggleSpindle.UseVisualStyleBackColor = true;
            this.BtnToggleSpindle.Click += new System.EventHandler(this.BtnToggleSpindle_Click);
            // 
            // BtnToggleMist
            // 
            resources.ApplyResources(this.BtnToggleMist, "BtnToggleMist");
            this.BtnToggleMist.Image = global::GrblPlotter.Properties.Resources.led_off;
            this.BtnToggleMist.Name = "BtnToggleMist";
            this.BtnToggleMist.UseVisualStyleBackColor = true;
            this.BtnToggleMist.Click += new System.EventHandler(this.BtnToggleMist_Click);
            // 
            // BtnToggleFlood
            // 
            resources.ApplyResources(this.BtnToggleFlood, "BtnToggleFlood");
            this.BtnToggleFlood.Image = global::GrblPlotter.Properties.Resources.led_off;
            this.BtnToggleFlood.Name = "BtnToggleFlood";
            this.BtnToggleFlood.UseVisualStyleBackColor = true;
            this.BtnToggleFlood.Click += new System.EventHandler(this.BtnToggleFlood_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tableLayoutPanel5);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // tableLayoutPanel6
            // 
            resources.ApplyResources(this.tableLayoutPanel6, "tableLayoutPanel6");
            this.tableLayoutPanel6.Controls.Add(this.LblSpindleValue, 5, 0);
            this.tableLayoutPanel6.Controls.Add(this.BtnSpindle1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.BtnSpindle2, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.BtnSpindle5, 4, 0);
            this.tableLayoutPanel6.Controls.Add(this.BtnSpindle3, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.BtnSpindle4, 3, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            // 
            // LblSpindleValue
            // 
            resources.ApplyResources(this.LblSpindleValue, "LblSpindleValue");
            this.LblSpindleValue.Name = "LblSpindleValue";
            // 
            // tableLayoutPanel7
            // 
            resources.ApplyResources(this.tableLayoutPanel7, "tableLayoutPanel7");
            this.tableLayoutPanel7.Controls.Add(this.LblSpindle, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.panel6, 1, 0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            // 
            // LblSpindle
            // 
            resources.ApplyResources(this.LblSpindle, "LblSpindle");
            this.LblSpindle.Name = "LblSpindle";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.LblSpindleSet);
            this.panel6.Controls.Add(this.LblSpindleSet0);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.LblFeedValue, 5, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnFeed1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnFeed2, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnFeed3, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnFeed4, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.BtnFeed5, 4, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // LblFeedValue
            // 
            resources.ApplyResources(this.LblFeedValue, "LblFeedValue");
            this.LblFeedValue.Name = "LblFeedValue";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.panel5, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.LblFeed, 0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.LblFeedSet);
            this.panel5.Controls.Add(this.LblFeedSet0);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // LblFeed
            // 
            resources.ApplyResources(this.LblFeed, "LblFeed");
            this.LblFeed.Name = "LblFeed";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel8);
            this.panel2.Controls.Add(this.LblRapidValue);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // tableLayoutPanel8
            // 
            resources.ApplyResources(this.tableLayoutPanel8, "tableLayoutPanel8");
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel9, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.tableLayoutPanel10, 0, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            // 
            // tableLayoutPanel9
            // 
            resources.ApplyResources(this.tableLayoutPanel9, "tableLayoutPanel9");
            this.tableLayoutPanel9.Controls.Add(this.BtnRapid1, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.BtnRapid3, 2, 0);
            this.tableLayoutPanel9.Controls.Add(this.BtnRapid2, 1, 0);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            // 
            // tableLayoutPanel10
            // 
            resources.ApplyResources(this.tableLayoutPanel10, "tableLayoutPanel10");
            this.tableLayoutPanel10.Controls.Add(this.panel7, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.LblRapid, 0, 0);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.LblRapidSet0);
            this.panel7.Controls.Add(this.LblRapidSet);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // LblRapid
            // 
            resources.ApplyResources(this.LblRapid, "LblRapid");
            this.LblRapid.Name = "LblRapid";
            // 
            // LblRapidValue
            // 
            resources.ApplyResources(this.LblRapidValue, "LblRapidValue");
            this.LblRapidValue.Name = "LblRapidValue";
            // 
            // PanelTranslation
            // 
            this.PanelTranslation.Controls.Add(this.LblSetupOption1);
            this.PanelTranslation.Controls.Add(this.LblSetupOption4);
            this.PanelTranslation.Controls.Add(this.LblSetupOption3);
            this.PanelTranslation.Controls.Add(this.LblSetupOption2);
            this.PanelTranslation.Controls.Add(this.LblSetupHeadline);
            resources.ApplyResources(this.PanelTranslation, "PanelTranslation");
            this.PanelTranslation.Name = "PanelTranslation";
            // 
            // LblSetupOption4
            // 
            resources.ApplyResources(this.LblSetupOption4, "LblSetupOption4");
            this.LblSetupOption4.Name = "LblSetupOption4";
            // 
            // LblSetupOption3
            // 
            resources.ApplyResources(this.LblSetupOption3, "LblSetupOption3");
            this.LblSetupOption3.Name = "LblSetupOption3";
            // 
            // LblSetupOption2
            // 
            resources.ApplyResources(this.LblSetupOption2, "LblSetupOption2");
            this.LblSetupOption2.Name = "LblSetupOption2";
            // 
            // LblSetupHeadline
            // 
            resources.ApplyResources(this.LblSetupHeadline, "LblSetupHeadline");
            this.LblSetupHeadline.Name = "LblSetupHeadline";
            // 
            // LblSetupOption1
            // 
            resources.ApplyResources(this.LblSetupOption1, "LblSetupOption1");
            this.LblSetupOption1.Name = "LblSetupOption1";
            // 
            // UCOverrides
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbOverrides);
            this.Name = "UCOverrides";
            this.Load += new System.EventHandler(this.UserControlOverrides_Load);
            this.Resize += new System.EventHandler(this.UCOverrides_Resize);
            this.GbOverrides.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel13.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.PanelTranslation.ResumeLayout(false);
            this.PanelTranslation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Button BtnFeed5;
        private Button BtnFeed4;
        private Button BtnFeed3;
        private Button BtnFeed2;
        private Button BtnFeed1;
        private Label LblFeedSet;
        private Label LblFeedSet0;
        private Button BtnRapid3;
        private Button BtnRapid2;
        private Button BtnRapid1;
        private Label LblRapidSet;
        private Label LblRapidSet0;
        private Label LblSpindleSet;
        private Label LblSpindleSet0;
        private Button BtnSpindle5;
        private Button BtnSpindle4;
        private Button BtnSpindle3;
        private Button BtnSpindle2;
        private Button BtnSpindle1;
        private GroupBox GbOverrides;
        private TableLayoutPanel tableLayoutPanel1;
        private Button BtnOverrideD3;
        private Button BtnOverrideD2;
        private Button BtnOverrideD1;
        private Button BtnOverrideD0;
        private Button BtnToggleMist;
        private Button BtnToggleFlood;
        private Button BtnToggleSpindle;
        private Label LblFeedValue;
        private Label LblRapidValue;
        private Label LblSpindleValue;
        private Panel panel1;
        private Label LblFeed;
        private Panel panel2;
        private Label LblRapid;
        private Panel panel3;
        private Label LblSpindle;
        private Panel panel4;
        private Label LblToggle;
        private Button BtnSetup;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private Panel panel5;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel7;
        private Panel panel6;
        private TableLayoutPanel tableLayoutPanel8;
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel10;
        private Panel panel7;
        private TableLayoutPanel tableLayoutPanel11;
        private TableLayoutPanel tableLayoutPanel12;
        private TableLayoutPanel tableLayoutPanel13;
        private ToolTip toolTip1;
        private Panel PanelTranslation;
        private Label LblSetupOption1;
        private Label LblSetupOption4;
        private Label LblSetupOption3;
        private Label LblSetupOption2;
        private Label LblSetupHeadline;
    }
}
