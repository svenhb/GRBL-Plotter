using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCDRO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDRO));
            this.GbDRO = new System.Windows.Forms.GroupBox();
            this.gBoxDROSetCoord = new System.Windows.Forms.GroupBox();
            this.BtnSetCoordA = new System.Windows.Forms.Button();
            this.NudSetCoordA = new System.Windows.Forms.NumericUpDown();
            this.LblSetCoordA = new System.Windows.Forms.Label();
            this.BtnSetCoordZ = new System.Windows.Forms.Button();
            this.BtnSetCoordY = new System.Windows.Forms.Button();
            this.BtnSetCoordX = new System.Windows.Forms.Button();
            this.NudSetCoordZ = new System.Windows.Forms.NumericUpDown();
            this.NudSetCoordY = new System.Windows.Forms.NumericUpDown();
            this.NudSetCoordX = new System.Windows.Forms.NumericUpDown();
            this.LblSetCoordZ = new System.Windows.Forms.Label();
            this.LblSetCoordY = new System.Windows.Forms.Label();
            this.LblSetCoordX = new System.Windows.Forms.Label();
            this.BtnZeroC = new System.Windows.Forms.Button();
            this.BtnZeroB = new System.Windows.Forms.Button();
            this.Lbl_mc = new System.Windows.Forms.Label();
            this.Lbl_mb = new System.Windows.Forms.Label();
            this.Lbl_wc = new System.Windows.Forms.Label();
            this.Lbl_wb = new System.Windows.Forms.Label();
            this.LblC = new System.Windows.Forms.Label();
            this.LblB = new System.Windows.Forms.Label();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.BtnHelp = new System.Windows.Forms.Button();
            this.lblCurrentG = new System.Windows.Forms.Label();
            this.BtnHome = new System.Windows.Forms.Button();
            this.BtnZeroA = new System.Windows.Forms.Button();
            this.Lbl_ma = new System.Windows.Forms.Label();
            this.Lbl_wa = new System.Windows.Forms.Label();
            this.LblA = new System.Windows.Forms.Label();
            this.BtnZeroXYZ = new System.Windows.Forms.Button();
            this.BtnZeroXY = new System.Windows.Forms.Button();
            this.BtnZeroZ = new System.Windows.Forms.Button();
            this.BtnZeroY = new System.Windows.Forms.Button();
            this.BtnZeroX = new System.Windows.Forms.Button();
            this.Lbl_mz = new System.Windows.Forms.Label();
            this.Lbl_my = new System.Windows.Forms.Label();
            this.Lbl_mx = new System.Windows.Forms.Label();
            this.Lbl_wz = new System.Windows.Forms.Label();
            this.Lbl_wy = new System.Windows.Forms.Label();
            this.Lbl_wx = new System.Windows.Forms.Label();
            this.LblZ = new System.Windows.Forms.Label();
            this.LblY = new System.Windows.Forms.Label();
            this.LblX = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.GbDRO.SuspendLayout();
            this.gBoxDROSetCoord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordX)).BeginInit();
            this.SuspendLayout();
            // 
            // GbDRO
            // 
            this.GbDRO.Controls.Add(this.gBoxDROSetCoord);
            this.GbDRO.Controls.Add(this.BtnZeroC);
            this.GbDRO.Controls.Add(this.BtnZeroB);
            this.GbDRO.Controls.Add(this.Lbl_mc);
            this.GbDRO.Controls.Add(this.Lbl_mb);
            this.GbDRO.Controls.Add(this.Lbl_wc);
            this.GbDRO.Controls.Add(this.Lbl_wb);
            this.GbDRO.Controls.Add(this.LblC);
            this.GbDRO.Controls.Add(this.LblB);
            this.GbDRO.Controls.Add(this.BtnSetup);
            this.GbDRO.Controls.Add(this.BtnHelp);
            this.GbDRO.Controls.Add(this.lblCurrentG);
            this.GbDRO.Controls.Add(this.BtnHome);
            this.GbDRO.Controls.Add(this.BtnZeroA);
            this.GbDRO.Controls.Add(this.Lbl_ma);
            this.GbDRO.Controls.Add(this.Lbl_wa);
            this.GbDRO.Controls.Add(this.LblA);
            this.GbDRO.Controls.Add(this.BtnZeroXYZ);
            this.GbDRO.Controls.Add(this.BtnZeroXY);
            this.GbDRO.Controls.Add(this.BtnZeroZ);
            this.GbDRO.Controls.Add(this.BtnZeroY);
            this.GbDRO.Controls.Add(this.BtnZeroX);
            this.GbDRO.Controls.Add(this.Lbl_mz);
            this.GbDRO.Controls.Add(this.Lbl_my);
            this.GbDRO.Controls.Add(this.Lbl_mx);
            this.GbDRO.Controls.Add(this.Lbl_wz);
            this.GbDRO.Controls.Add(this.Lbl_wy);
            this.GbDRO.Controls.Add(this.Lbl_wx);
            this.GbDRO.Controls.Add(this.LblZ);
            this.GbDRO.Controls.Add(this.LblY);
            this.GbDRO.Controls.Add(this.LblX);
            resources.ApplyResources(this.GbDRO, "GbDRO");
            this.GbDRO.Name = "GbDRO";
            this.GbDRO.TabStop = false;
            this.toolTip1.SetToolTip(this.GbDRO, resources.GetString("GbDRO.ToolTip"));
            // 
            // gBoxDROSetCoord
            // 
            this.gBoxDROSetCoord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.gBoxDROSetCoord.Controls.Add(this.BtnSetCoordA);
            this.gBoxDROSetCoord.Controls.Add(this.NudSetCoordA);
            this.gBoxDROSetCoord.Controls.Add(this.LblSetCoordA);
            this.gBoxDROSetCoord.Controls.Add(this.BtnSetCoordZ);
            this.gBoxDROSetCoord.Controls.Add(this.BtnSetCoordY);
            this.gBoxDROSetCoord.Controls.Add(this.BtnSetCoordX);
            this.gBoxDROSetCoord.Controls.Add(this.NudSetCoordZ);
            this.gBoxDROSetCoord.Controls.Add(this.NudSetCoordY);
            this.gBoxDROSetCoord.Controls.Add(this.NudSetCoordX);
            this.gBoxDROSetCoord.Controls.Add(this.LblSetCoordZ);
            this.gBoxDROSetCoord.Controls.Add(this.LblSetCoordY);
            this.gBoxDROSetCoord.Controls.Add(this.LblSetCoordX);
            resources.ApplyResources(this.gBoxDROSetCoord, "gBoxDROSetCoord");
            this.gBoxDROSetCoord.Name = "gBoxDROSetCoord";
            this.gBoxDROSetCoord.TabStop = false;
            // 
            // BtnSetCoordA
            // 
            resources.ApplyResources(this.BtnSetCoordA, "BtnSetCoordA");
            this.BtnSetCoordA.Name = "BtnSetCoordA";
            this.BtnSetCoordA.UseVisualStyleBackColor = true;
            // 
            // NudSetCoordA
            // 
            this.NudSetCoordA.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "mainFormSetCoordA", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudSetCoordA.DecimalPlaces = 2;
            resources.ApplyResources(this.NudSetCoordA, "NudSetCoordA");
            this.NudSetCoordA.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudSetCoordA.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.NudSetCoordA.Name = "NudSetCoordA";
            this.NudSetCoordA.Value = global::GrblPlotter.Properties.Settings.Default.mainFormSetCoordA;
            // 
            // LblSetCoordA
            // 
            resources.ApplyResources(this.LblSetCoordA, "LblSetCoordA");
            this.LblSetCoordA.Name = "LblSetCoordA";
            // 
            // BtnSetCoordZ
            // 
            resources.ApplyResources(this.BtnSetCoordZ, "BtnSetCoordZ");
            this.BtnSetCoordZ.Name = "BtnSetCoordZ";
            this.BtnSetCoordZ.UseVisualStyleBackColor = true;
            // 
            // BtnSetCoordY
            // 
            resources.ApplyResources(this.BtnSetCoordY, "BtnSetCoordY");
            this.BtnSetCoordY.Name = "BtnSetCoordY";
            this.BtnSetCoordY.UseVisualStyleBackColor = true;
            // 
            // BtnSetCoordX
            // 
            resources.ApplyResources(this.BtnSetCoordX, "BtnSetCoordX");
            this.BtnSetCoordX.Name = "BtnSetCoordX";
            this.BtnSetCoordX.UseVisualStyleBackColor = true;
            // 
            // NudSetCoordZ
            // 
            this.NudSetCoordZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "mainFormSetCoordZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudSetCoordZ.DecimalPlaces = 2;
            resources.ApplyResources(this.NudSetCoordZ, "NudSetCoordZ");
            this.NudSetCoordZ.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudSetCoordZ.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.NudSetCoordZ.Name = "NudSetCoordZ";
            this.NudSetCoordZ.Value = global::GrblPlotter.Properties.Settings.Default.mainFormSetCoordZ;
            // 
            // NudSetCoordY
            // 
            this.NudSetCoordY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "mainFormSetCoordY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudSetCoordY.DecimalPlaces = 2;
            resources.ApplyResources(this.NudSetCoordY, "NudSetCoordY");
            this.NudSetCoordY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudSetCoordY.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.NudSetCoordY.Name = "NudSetCoordY";
            this.NudSetCoordY.Value = global::GrblPlotter.Properties.Settings.Default.mainFormSetCoordY;
            // 
            // NudSetCoordX
            // 
            this.NudSetCoordX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "mainFormSetCoordX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudSetCoordX.DecimalPlaces = 2;
            resources.ApplyResources(this.NudSetCoordX, "NudSetCoordX");
            this.NudSetCoordX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudSetCoordX.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.NudSetCoordX.Name = "NudSetCoordX";
            this.NudSetCoordX.Value = global::GrblPlotter.Properties.Settings.Default.mainFormSetCoordX;
            // 
            // LblSetCoordZ
            // 
            resources.ApplyResources(this.LblSetCoordZ, "LblSetCoordZ");
            this.LblSetCoordZ.Name = "LblSetCoordZ";
            // 
            // LblSetCoordY
            // 
            resources.ApplyResources(this.LblSetCoordY, "LblSetCoordY");
            this.LblSetCoordY.Name = "LblSetCoordY";
            // 
            // LblSetCoordX
            // 
            resources.ApplyResources(this.LblSetCoordX, "LblSetCoordX");
            this.LblSetCoordX.Name = "LblSetCoordX";
            // 
            // BtnZeroC
            // 
            resources.ApplyResources(this.BtnZeroC, "BtnZeroC");
            this.BtnZeroC.Name = "BtnZeroC";
            this.toolTip1.SetToolTip(this.BtnZeroC, resources.GetString("BtnZeroC.ToolTip"));
            this.BtnZeroC.UseVisualStyleBackColor = true;
            this.BtnZeroC.Click += new System.EventHandler(this.BtnZeroC_Click);
            // 
            // BtnZeroB
            // 
            resources.ApplyResources(this.BtnZeroB, "BtnZeroB");
            this.BtnZeroB.Name = "BtnZeroB";
            this.toolTip1.SetToolTip(this.BtnZeroB, resources.GetString("BtnZeroB.ToolTip"));
            this.BtnZeroB.UseVisualStyleBackColor = true;
            this.BtnZeroB.Click += new System.EventHandler(this.BtnZeroB_Click);
            // 
            // Lbl_mc
            // 
            resources.ApplyResources(this.Lbl_mc, "Lbl_mc");
            this.Lbl_mc.Name = "Lbl_mc";
            // 
            // Lbl_mb
            // 
            resources.ApplyResources(this.Lbl_mb, "Lbl_mb");
            this.Lbl_mb.Name = "Lbl_mb";
            // 
            // Lbl_wc
            // 
            resources.ApplyResources(this.Lbl_wc, "Lbl_wc");
            this.Lbl_wc.Name = "Lbl_wc";
            // 
            // Lbl_wb
            // 
            resources.ApplyResources(this.Lbl_wb, "Lbl_wb");
            this.Lbl_wb.Name = "Lbl_wb";
            // 
            // LblC
            // 
            resources.ApplyResources(this.LblC, "LblC");
            this.LblC.Name = "LblC";
            // 
            // LblB
            // 
            resources.ApplyResources(this.LblB, "LblB");
            this.LblB.Name = "LblB";
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.toolTip1.SetToolTip(this.BtnSetup, resources.GetString("BtnSetup.ToolTip"));
            this.BtnSetup.UseVisualStyleBackColor = true;
            // 
            // BtnHelp
            // 
            resources.ApplyResources(this.BtnHelp, "BtnHelp");
            this.BtnHelp.Name = "BtnHelp";
            this.toolTip1.SetToolTip(this.BtnHelp, resources.GetString("BtnHelp.ToolTip"));
            this.BtnHelp.UseVisualStyleBackColor = true;
            // 
            // lblCurrentG
            // 
            resources.ApplyResources(this.lblCurrentG, "lblCurrentG");
            this.lblCurrentG.BackColor = System.Drawing.SystemColors.Control;
            this.lblCurrentG.Name = "lblCurrentG";
            // 
            // BtnHome
            // 
            resources.ApplyResources(this.BtnHome, "BtnHome");
            this.BtnHome.Name = "BtnHome";
            this.toolTip1.SetToolTip(this.BtnHome, resources.GetString("BtnHome.ToolTip"));
            this.BtnHome.UseVisualStyleBackColor = true;
            this.BtnHome.Click += new System.EventHandler(this.BtnHome_Click);
            // 
            // BtnZeroA
            // 
            resources.ApplyResources(this.BtnZeroA, "BtnZeroA");
            this.BtnZeroA.Name = "BtnZeroA";
            this.toolTip1.SetToolTip(this.BtnZeroA, resources.GetString("BtnZeroA.ToolTip"));
            this.BtnZeroA.UseVisualStyleBackColor = true;
            this.BtnZeroA.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnZeroA_MouseUp);
            // 
            // Lbl_ma
            // 
            resources.ApplyResources(this.Lbl_ma, "Lbl_ma");
            this.Lbl_ma.Name = "Lbl_ma";
            // 
            // Lbl_wa
            // 
            resources.ApplyResources(this.Lbl_wa, "Lbl_wa");
            this.Lbl_wa.Name = "Lbl_wa";
            this.toolTip1.SetToolTip(this.Lbl_wa, resources.GetString("Lbl_wa.ToolTip"));
            // 
            // LblA
            // 
            resources.ApplyResources(this.LblA, "LblA");
            this.LblA.Name = "LblA";
            this.toolTip1.SetToolTip(this.LblA, resources.GetString("LblA.ToolTip"));
            // 
            // BtnZeroXYZ
            // 
            resources.ApplyResources(this.BtnZeroXYZ, "BtnZeroXYZ");
            this.BtnZeroXYZ.Name = "BtnZeroXYZ";
            this.toolTip1.SetToolTip(this.BtnZeroXYZ, resources.GetString("BtnZeroXYZ.ToolTip"));
            this.BtnZeroXYZ.UseVisualStyleBackColor = true;
            this.BtnZeroXYZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnZeroXYZ_MouseUp);
            // 
            // BtnZeroXY
            // 
            resources.ApplyResources(this.BtnZeroXY, "BtnZeroXY");
            this.BtnZeroXY.Name = "BtnZeroXY";
            this.toolTip1.SetToolTip(this.BtnZeroXY, resources.GetString("BtnZeroXY.ToolTip"));
            this.BtnZeroXY.UseVisualStyleBackColor = true;
            this.BtnZeroXY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnZeroXY_MouseUp);
            // 
            // BtnZeroZ
            // 
            resources.ApplyResources(this.BtnZeroZ, "BtnZeroZ");
            this.BtnZeroZ.Name = "BtnZeroZ";
            this.toolTip1.SetToolTip(this.BtnZeroZ, resources.GetString("BtnZeroZ.ToolTip"));
            this.BtnZeroZ.UseVisualStyleBackColor = true;
            this.BtnZeroZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnZeroZ_MouseUp);
            // 
            // BtnZeroY
            // 
            resources.ApplyResources(this.BtnZeroY, "BtnZeroY");
            this.BtnZeroY.Name = "BtnZeroY";
            this.toolTip1.SetToolTip(this.BtnZeroY, resources.GetString("BtnZeroY.ToolTip"));
            this.BtnZeroY.UseVisualStyleBackColor = true;
            this.BtnZeroY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnZeroY_MouseUp);
            // 
            // BtnZeroX
            // 
            resources.ApplyResources(this.BtnZeroX, "BtnZeroX");
            this.BtnZeroX.Name = "BtnZeroX";
            this.toolTip1.SetToolTip(this.BtnZeroX, resources.GetString("BtnZeroX.ToolTip"));
            this.BtnZeroX.UseVisualStyleBackColor = true;
            this.BtnZeroX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BtnZeroX_MouseUp);
            // 
            // Lbl_mz
            // 
            resources.ApplyResources(this.Lbl_mz, "Lbl_mz");
            this.Lbl_mz.Name = "Lbl_mz";
            // 
            // Lbl_my
            // 
            resources.ApplyResources(this.Lbl_my, "Lbl_my");
            this.Lbl_my.Name = "Lbl_my";
            // 
            // Lbl_mx
            // 
            resources.ApplyResources(this.Lbl_mx, "Lbl_mx");
            this.Lbl_mx.Name = "Lbl_mx";
            // 
            // Lbl_wz
            // 
            resources.ApplyResources(this.Lbl_wz, "Lbl_wz");
            this.Lbl_wz.Name = "Lbl_wz";
            this.toolTip1.SetToolTip(this.Lbl_wz, resources.GetString("Lbl_wz.ToolTip"));
            // 
            // Lbl_wy
            // 
            resources.ApplyResources(this.Lbl_wy, "Lbl_wy");
            this.Lbl_wy.Name = "Lbl_wy";
            this.toolTip1.SetToolTip(this.Lbl_wy, resources.GetString("Lbl_wy.ToolTip"));
            // 
            // Lbl_wx
            // 
            resources.ApplyResources(this.Lbl_wx, "Lbl_wx");
            this.Lbl_wx.Name = "Lbl_wx";
            this.toolTip1.SetToolTip(this.Lbl_wx, resources.GetString("Lbl_wx.ToolTip"));
            // 
            // LblZ
            // 
            resources.ApplyResources(this.LblZ, "LblZ");
            this.LblZ.Name = "LblZ";
            this.toolTip1.SetToolTip(this.LblZ, resources.GetString("LblZ.ToolTip"));
            // 
            // LblY
            // 
            resources.ApplyResources(this.LblY, "LblY");
            this.LblY.Name = "LblY";
            this.toolTip1.SetToolTip(this.LblY, resources.GetString("LblY.ToolTip"));
            // 
            // LblX
            // 
            resources.ApplyResources(this.LblX, "LblX");
            this.LblX.Name = "LblX";
            this.toolTip1.SetToolTip(this.LblX, resources.GetString("LblX.ToolTip"));
            // 
            // UCDRO
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbDRO);
            this.Name = "UCDRO";
            this.Load += new System.EventHandler(this.UserControlDRO_Load);
            this.MouseEnter += new System.EventHandler(this.UCDRO_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.UCDRO_MouseLeave);
            this.GbDRO.ResumeLayout(false);
            this.GbDRO.PerformLayout();
            this.gBoxDROSetCoord.ResumeLayout(false);
            this.gBoxDROSetCoord.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSetCoordX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox GbDRO;
        private Label Lbl_mz;
        private Label Lbl_my;
        private Label Lbl_mx;
        private Label Lbl_wz;
        private Label Lbl_wy;
        private Label Lbl_wx;
        private Label LblZ;
        private Label LblY;
        private Label LblX;
        private Button BtnZeroZ;
        private Button BtnZeroY;
        private Button BtnZeroX;
        private Button BtnZeroA;
        private Label Lbl_ma;
        private Label Lbl_wa;
        private Label LblA;
        private Button BtnZeroXYZ;
        private Button BtnZeroXY;
        private Button BtnHome;
        private Label lblCurrentG;
        private Button BtnSetup;
        private Button BtnHelp;
        private ToolTip toolTip1;
        private Button BtnZeroC;
        private Button BtnZeroB;
        private Label Lbl_mc;
        private Label Lbl_mb;
        private Label Lbl_wc;
        private Label Lbl_wb;
        private Label LblC;
        private Label LblB;
        private GroupBox gBoxDROSetCoord;
        private Button BtnSetCoordA;
        private NumericUpDown NudSetCoordA;
        private Label LblSetCoordA;
        private Button BtnSetCoordZ;
        private Button BtnSetCoordY;
        private Button BtnSetCoordX;
        private NumericUpDown NudSetCoordZ;
        private NumericUpDown NudSetCoordY;
        private NumericUpDown NudSetCoordX;
        private Label LblSetCoordZ;
        private Label LblSetCoordY;
        private Label LblSetCoordX;
    }
}
