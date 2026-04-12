using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCFlowControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCFlowControl));
            this.GbFlowControl = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnFeedHold = new System.Windows.Forms.Button();
            this.BtnResume = new System.Windows.Forms.Button();
            this.BtnSafetyDoor = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnReset = new System.Windows.Forms.Button();
            this.BtnKillAlarm = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.GbFlowControl.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // GbFlowControl
            // 
            this.GbFlowControl.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.GbFlowControl, "GbFlowControl");
            this.GbFlowControl.Name = "GbFlowControl";
            this.GbFlowControl.TabStop = false;
            this.toolTip1.SetToolTip(this.GbFlowControl, resources.GetString("GbFlowControl.ToolTip"));
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.BtnFeedHold, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.BtnResume, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.BtnSafetyDoor, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // BtnFeedHold
            // 
            resources.ApplyResources(this.BtnFeedHold, "BtnFeedHold");
            this.BtnFeedHold.Name = "BtnFeedHold";
            this.toolTip1.SetToolTip(this.BtnFeedHold, resources.GetString("BtnFeedHold.ToolTip"));
            this.BtnFeedHold.UseVisualStyleBackColor = true;
            this.BtnFeedHold.Click += new System.EventHandler(this.BtnFeedHold_Click);
            // 
            // BtnResume
            // 
            resources.ApplyResources(this.BtnResume, "BtnResume");
            this.BtnResume.Name = "BtnResume";
            this.toolTip1.SetToolTip(this.BtnResume, resources.GetString("BtnResume.ToolTip"));
            this.BtnResume.UseVisualStyleBackColor = true;
            this.BtnResume.Click += new System.EventHandler(this.BtnResume_Click);
            // 
            // BtnSafetyDoor
            // 
            resources.ApplyResources(this.BtnSafetyDoor, "BtnSafetyDoor");
            this.BtnSafetyDoor.Name = "BtnSafetyDoor";
            this.toolTip1.SetToolTip(this.BtnSafetyDoor, resources.GetString("BtnSafetyDoor.ToolTip"));
            this.BtnSafetyDoor.UseVisualStyleBackColor = true;
            this.BtnSafetyDoor.Click += new System.EventHandler(this.BtnSafetyDoor_Click);
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.BtnReset, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.BtnKillAlarm, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // BtnReset
            // 
            this.BtnReset.BackColor = System.Drawing.Color.LightPink;
            resources.ApplyResources(this.BtnReset, "BtnReset");
            this.BtnReset.Name = "BtnReset";
            this.toolTip1.SetToolTip(this.BtnReset, resources.GetString("BtnReset.ToolTip"));
            this.BtnReset.UseVisualStyleBackColor = false;
            this.BtnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // BtnKillAlarm
            // 
            resources.ApplyResources(this.BtnKillAlarm, "BtnKillAlarm");
            this.BtnKillAlarm.Name = "BtnKillAlarm";
            this.toolTip1.SetToolTip(this.BtnKillAlarm, resources.GetString("BtnKillAlarm.ToolTip"));
            this.BtnKillAlarm.UseVisualStyleBackColor = true;
            this.BtnKillAlarm.Click += new System.EventHandler(this.BtnKillAlarm_Click);
            // 
            // UCFlowControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbFlowControl);
            this.Name = "UCFlowControl";
            this.Load += new System.EventHandler(this.UCFlowControl_Load);
            this.GbFlowControl.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox GbFlowControl;
        private Button BtnReset;
        private Button BtnKillAlarm;
        private Button BtnResume;
        private Button BtnFeedHold;
        private Button BtnSafetyDoor;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private ToolTip toolTip1;
    }
}
