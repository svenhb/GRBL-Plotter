using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCStreaming
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCStreaming));
            this.GbStreaming = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.btnSimulatePause = new System.Windows.Forms.Button();
            this.btnSimulateSlower = new System.Windows.Forms.Button();
            this.btnSimulateFaster = new System.Windows.Forms.Button();
            this.LblStatusStreaming = new System.Windows.Forms.Label();
            this.LblStatusGrbl = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnStreamStart = new System.Windows.Forms.Button();
            this.btnStreamStop = new System.Windows.Forms.Button();
            this.btnStreamCheck = new System.Windows.Forms.Button();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbBuffer = new System.Windows.Forms.ProgressBar();
            this.pbFile = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.lblProgress = new System.Windows.Forms.Label();
            this.LblTime = new System.Windows.Forms.Label();
            this.lblProgressText = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.GbStreaming.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // GbStreaming
            // 
            this.GbStreaming.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.GbStreaming, "GbStreaming");
            this.GbStreaming.Name = "GbStreaming";
            this.GbStreaming.TabStop = false;
            this.toolTip1.SetToolTip(this.GbStreaming, resources.GetString("GbStreaming.ToolTip"));
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LblStatusStreaming, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.LblStatusGrbl, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.btnSimulate, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSimulatePause, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSimulateSlower, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSimulateFaster, 2, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // btnSimulate
            // 
            resources.ApplyResources(this.btnSimulate, "btnSimulate");
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.UseVisualStyleBackColor = true;
            this.btnSimulate.Click += new System.EventHandler(this.BtnSimulate_Click);
            // 
            // btnSimulatePause
            // 
            resources.ApplyResources(this.btnSimulatePause, "btnSimulatePause");
            this.btnSimulatePause.Image = global::GrblPlotter.Properties.Resources.btn_pause;
            this.btnSimulatePause.Name = "btnSimulatePause";
            this.btnSimulatePause.UseVisualStyleBackColor = true;
            this.btnSimulatePause.Click += new System.EventHandler(this.BtnSimulatePause_Click);
            // 
            // btnSimulateSlower
            // 
            resources.ApplyResources(this.btnSimulateSlower, "btnSimulateSlower");
            this.btnSimulateSlower.Name = "btnSimulateSlower";
            this.btnSimulateSlower.UseVisualStyleBackColor = true;
            this.btnSimulateSlower.Click += new System.EventHandler(this.BtnSimulateSlower_Click);
            // 
            // btnSimulateFaster
            // 
            resources.ApplyResources(this.btnSimulateFaster, "btnSimulateFaster");
            this.btnSimulateFaster.Name = "btnSimulateFaster";
            this.btnSimulateFaster.UseVisualStyleBackColor = true;
            this.btnSimulateFaster.Click += new System.EventHandler(this.BtnSimulateFaster_Click);
            // 
            // LblStatusStreaming
            // 
            this.LblStatusStreaming.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.LblStatusStreaming, "LblStatusStreaming");
            this.LblStatusStreaming.Name = "LblStatusStreaming";
            // 
            // LblStatusGrbl
            // 
            this.LblStatusGrbl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            resources.ApplyResources(this.LblStatusGrbl, "LblStatusGrbl");
            this.LblStatusGrbl.Name = "LblStatusGrbl";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel6, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnStreamCheck, 0, 1);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.btnStreamStart, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnStreamStop, 1, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // btnStreamStart
            // 
            resources.ApplyResources(this.btnStreamStart, "btnStreamStart");
            this.btnStreamStart.Image = global::GrblPlotter.Properties.Resources.btn_play;
            this.btnStreamStart.Name = "btnStreamStart";
            this.btnStreamStart.UseVisualStyleBackColor = true;
            this.btnStreamStart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnStreamStart_Click);
            // 
            // btnStreamStop
            // 
            resources.ApplyResources(this.btnStreamStop, "btnStreamStop");
            this.btnStreamStop.Image = global::GrblPlotter.Properties.Resources.btn_stop;
            this.btnStreamStop.Name = "btnStreamStop";
            this.btnStreamStop.UseVisualStyleBackColor = true;
            this.btnStreamStop.Click += new System.EventHandler(this.BtnStreamStop_Click);
            // 
            // btnStreamCheck
            // 
            resources.ApplyResources(this.btnStreamCheck, "btnStreamCheck");
            this.btnStreamCheck.Name = "btnStreamCheck";
            this.btnStreamCheck.UseVisualStyleBackColor = true;
            this.btnStreamCheck.Click += new System.EventHandler(this.BtnStreamCheck_Click);
            // 
            // tableLayoutPanel6
            // 
            resources.ApplyResources(this.tableLayoutPanel6, "tableLayoutPanel6");
            this.tableLayoutPanel6.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pbBuffer);
            this.panel1.Controls.Add(this.pbFile);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pbBuffer
            // 
            resources.ApplyResources(this.pbBuffer, "pbBuffer");
            this.pbBuffer.Name = "pbBuffer";
            // 
            // pbFile
            // 
            resources.ApplyResources(this.pbFile, "pbFile");
            this.pbFile.Name = "pbFile";
            // 
            // tableLayoutPanel7
            // 
            resources.ApplyResources(this.tableLayoutPanel7, "tableLayoutPanel7");
            this.tableLayoutPanel7.Controls.Add(this.lblProgress, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.LblTime, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.lblProgressText, 1, 0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            // 
            // lblProgress
            // 
            resources.ApplyResources(this.lblProgress, "lblProgress");
            this.lblProgress.Name = "lblProgress";
            // 
            // LblTime
            // 
            resources.ApplyResources(this.LblTime, "LblTime");
            this.LblTime.Name = "LblTime";
            // 
            // lblProgressText
            // 
            resources.ApplyResources(this.lblProgressText, "lblProgressText");
            this.lblProgressText.Name = "lblProgressText";
            // 
            // UCStreaming
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbStreaming);
            this.Name = "UCStreaming";
            this.Resize += new System.EventHandler(this.UCStreaming_Resize);
            this.GbStreaming.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox GbStreaming;
    private ProgressBar pbBuffer;
    private ProgressBar pbFile;
    private Button btnStreamCheck;
    private Button btnStreamStop;
    private Button btnStreamStart;
    private Label LblTime;
    private Label lblProgress;
    private Button btnSimulateFaster;
    private Button btnSimulatePause;
    private Button btnSimulate;
    private Button btnSimulateSlower;
        private Label LblStatusStreaming;
        private Label LblStatusGrbl;
        private Label lblProgressText;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel7;
        private ToolTip toolTip1;
    }
}
