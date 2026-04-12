namespace GrblPlotter.UserControls
{
    partial class UCDeviceLaser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDeviceLaser));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.LblMax = new System.Windows.Forms.Label();
            this.LblMin = new System.Windows.Forms.Label();
            this.LblPasses = new System.Windows.Forms.Label();
            this.LblPowerMode = new System.Windows.Forms.Label();
            this.LblSpeed = new System.Windows.Forms.Label();
            this.LblPower = new System.Windows.Forms.Label();
            this.BtnFill = new System.Windows.Forms.Button();
            this.BtnUseZ = new System.Windows.Forms.Button();
            this.BtnOverlap = new System.Windows.Forms.Button();
            this.LblPowerMin = new System.Windows.Forms.Label();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.GbPowerSettings = new System.Windows.Forms.GroupBox();
            this.LblSet = new System.Windows.Forms.Label();
            this.LblLaserMinVal = new System.Windows.Forms.Label();
            this.LblLaserMaxVal = new System.Windows.Forms.Label();
            this.LblLaserSetVal = new System.Windows.Forms.Label();
            this.BtnGetMaterial = new System.Windows.Forms.Button();
            this.PanelTranslation = new System.Windows.Forms.Panel();
            this.LblZFinal = new System.Windows.Forms.Label();
            this.LblZSave = new System.Windows.Forms.Label();
            this.LbLZFeed = new System.Windows.Forms.Label();
            this.LbLZEnable = new System.Windows.Forms.Label();
            this.LblZInfo = new System.Windows.Forms.Label();
            this.LblZHeadline = new System.Windows.Forms.Label();
            this.LblOverlapInfo = new System.Windows.Forms.Label();
            this.LblOverlapValue = new System.Windows.Forms.Label();
            this.LblOverlapEnable = new System.Windows.Forms.Label();
            this.LblOverlapHeadline = new System.Windows.Forms.Label();
            this.BtnLaserTools = new System.Windows.Forms.Button();
            this.LblSetupHeadline = new System.Windows.Forms.Label();
            this.LblSetup1 = new System.Windows.Forms.Label();
            this.LblSetup2 = new System.Windows.Forms.Label();
            this.LblSetup3 = new System.Windows.Forms.Label();
            this.BtnHelp = new System.Windows.Forms.Button();
            this.NudDeviceLaserPowerMin = new System.Windows.Forms.NumericUpDown();
            this.CbAirAssist = new System.Windows.Forms.CheckBox();
            this.CbLaserpower = new System.Windows.Forms.CheckBox();
            this.NudDeviceLaserPasses = new System.Windows.Forms.NumericUpDown();
            this.NudDeviceLaserSpeed = new System.Windows.Forms.NumericUpDown();
            this.NudDeviceLaserPower = new System.Windows.Forms.NumericUpDown();
            this.BtnOffsetSort = new System.Windows.Forms.Button();
            this.GbPowerSettings.SuspendLayout();
            this.PanelTranslation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserPowerMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserPasses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserPower)).BeginInit();
            this.SuspendLayout();
            // 
            // LblMax
            // 
            resources.ApplyResources(this.LblMax, "LblMax");
            this.LblMax.Name = "LblMax";
            this.toolTip1.SetToolTip(this.LblMax, resources.GetString("LblMax.ToolTip"));
            // 
            // LblMin
            // 
            resources.ApplyResources(this.LblMin, "LblMin");
            this.LblMin.Name = "LblMin";
            this.toolTip1.SetToolTip(this.LblMin, resources.GetString("LblMin.ToolTip"));
            // 
            // LblPasses
            // 
            resources.ApplyResources(this.LblPasses, "LblPasses");
            this.LblPasses.Name = "LblPasses";
            this.toolTip1.SetToolTip(this.LblPasses, resources.GetString("LblPasses.ToolTip"));
            // 
            // LblPowerMode
            // 
            resources.ApplyResources(this.LblPowerMode, "LblPowerMode");
            this.LblPowerMode.Name = "LblPowerMode";
            this.toolTip1.SetToolTip(this.LblPowerMode, resources.GetString("LblPowerMode.ToolTip"));
            // 
            // LblSpeed
            // 
            resources.ApplyResources(this.LblSpeed, "LblSpeed");
            this.LblSpeed.Name = "LblSpeed";
            this.toolTip1.SetToolTip(this.LblSpeed, resources.GetString("LblSpeed.ToolTip"));
            // 
            // LblPower
            // 
            resources.ApplyResources(this.LblPower, "LblPower");
            this.LblPower.BackColor = System.Drawing.Color.Yellow;
            this.LblPower.Name = "LblPower";
            this.toolTip1.SetToolTip(this.LblPower, resources.GetString("LblPower.ToolTip"));
            this.LblPower.MouseEnter += new System.EventHandler(this.LblPower_MouseEnter);
            this.LblPower.MouseLeave += new System.EventHandler(this.LblPower_MouseLeave);
            // 
            // BtnFill
            // 
            resources.ApplyResources(this.BtnFill, "BtnFill");
            this.BtnFill.Name = "BtnFill";
            this.toolTip1.SetToolTip(this.BtnFill, resources.GetString("BtnFill.ToolTip"));
            this.BtnFill.UseVisualStyleBackColor = true;
            this.BtnFill.Click += new System.EventHandler(this.BtnFill_Click);
            // 
            // BtnUseZ
            // 
            resources.ApplyResources(this.BtnUseZ, "BtnUseZ");
            this.BtnUseZ.Name = "BtnUseZ";
            this.toolTip1.SetToolTip(this.BtnUseZ, resources.GetString("BtnUseZ.ToolTip"));
            this.BtnUseZ.UseVisualStyleBackColor = true;
            this.BtnUseZ.Click += new System.EventHandler(this.BtnUseZ_Click);
            // 
            // BtnOverlap
            // 
            resources.ApplyResources(this.BtnOverlap, "BtnOverlap");
            this.BtnOverlap.Name = "BtnOverlap";
            this.toolTip1.SetToolTip(this.BtnOverlap, resources.GetString("BtnOverlap.ToolTip"));
            this.BtnOverlap.UseVisualStyleBackColor = true;
            this.BtnOverlap.Click += new System.EventHandler(this.BtnOverlap_Click);
            // 
            // LblPowerMin
            // 
            resources.ApplyResources(this.LblPowerMin, "LblPowerMin");
            this.LblPowerMin.BackColor = System.Drawing.SystemColors.Control;
            this.LblPowerMin.Name = "LblPowerMin";
            this.toolTip1.SetToolTip(this.LblPowerMin, resources.GetString("LblPowerMin.ToolTip"));
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // GbPowerSettings
            // 
            this.GbPowerSettings.Controls.Add(this.LblSet);
            this.GbPowerSettings.Controls.Add(this.LblLaserMinVal);
            this.GbPowerSettings.Controls.Add(this.LblLaserMaxVal);
            this.GbPowerSettings.Controls.Add(this.LblMax);
            this.GbPowerSettings.Controls.Add(this.LblMin);
            this.GbPowerSettings.Controls.Add(this.LblLaserSetVal);
            resources.ApplyResources(this.GbPowerSettings, "GbPowerSettings");
            this.GbPowerSettings.Name = "GbPowerSettings";
            this.GbPowerSettings.TabStop = false;
            // 
            // LblSet
            // 
            resources.ApplyResources(this.LblSet, "LblSet");
            this.LblSet.Name = "LblSet";
            // 
            // LblLaserMinVal
            // 
            resources.ApplyResources(this.LblLaserMinVal, "LblLaserMinVal");
            this.LblLaserMinVal.Name = "LblLaserMinVal";
            // 
            // LblLaserMaxVal
            // 
            resources.ApplyResources(this.LblLaserMaxVal, "LblLaserMaxVal");
            this.LblLaserMaxVal.Name = "LblLaserMaxVal";
            // 
            // LblLaserSetVal
            // 
            resources.ApplyResources(this.LblLaserSetVal, "LblLaserSetVal");
            this.LblLaserSetVal.Name = "LblLaserSetVal";
            // 
            // BtnGetMaterial
            // 
            resources.ApplyResources(this.BtnGetMaterial, "BtnGetMaterial");
            this.BtnGetMaterial.Name = "BtnGetMaterial";
            this.BtnGetMaterial.UseVisualStyleBackColor = true;
            // 
            // PanelTranslation
            // 
            this.PanelTranslation.Controls.Add(this.LblSetup3);
            this.PanelTranslation.Controls.Add(this.LblSetup2);
            this.PanelTranslation.Controls.Add(this.LblSetup1);
            this.PanelTranslation.Controls.Add(this.LblSetupHeadline);
            this.PanelTranslation.Controls.Add(this.LblZFinal);
            this.PanelTranslation.Controls.Add(this.LblZSave);
            this.PanelTranslation.Controls.Add(this.LbLZFeed);
            this.PanelTranslation.Controls.Add(this.LbLZEnable);
            this.PanelTranslation.Controls.Add(this.LblZInfo);
            this.PanelTranslation.Controls.Add(this.LblZHeadline);
            this.PanelTranslation.Controls.Add(this.LblOverlapInfo);
            this.PanelTranslation.Controls.Add(this.LblOverlapValue);
            this.PanelTranslation.Controls.Add(this.LblOverlapEnable);
            this.PanelTranslation.Controls.Add(this.LblOverlapHeadline);
            resources.ApplyResources(this.PanelTranslation, "PanelTranslation");
            this.PanelTranslation.Name = "PanelTranslation";
            // 
            // LblZFinal
            // 
            resources.ApplyResources(this.LblZFinal, "LblZFinal");
            this.LblZFinal.Name = "LblZFinal";
            // 
            // LblZSave
            // 
            resources.ApplyResources(this.LblZSave, "LblZSave");
            this.LblZSave.Name = "LblZSave";
            // 
            // LbLZFeed
            // 
            resources.ApplyResources(this.LbLZFeed, "LbLZFeed");
            this.LbLZFeed.Name = "LbLZFeed";
            // 
            // LbLZEnable
            // 
            resources.ApplyResources(this.LbLZEnable, "LbLZEnable");
            this.LbLZEnable.Name = "LbLZEnable";
            // 
            // LblZInfo
            // 
            resources.ApplyResources(this.LblZInfo, "LblZInfo");
            this.LblZInfo.Name = "LblZInfo";
            // 
            // LblZHeadline
            // 
            resources.ApplyResources(this.LblZHeadline, "LblZHeadline");
            this.LblZHeadline.Name = "LblZHeadline";
            // 
            // LblOverlapInfo
            // 
            resources.ApplyResources(this.LblOverlapInfo, "LblOverlapInfo");
            this.LblOverlapInfo.Name = "LblOverlapInfo";
            // 
            // LblOverlapValue
            // 
            resources.ApplyResources(this.LblOverlapValue, "LblOverlapValue");
            this.LblOverlapValue.Name = "LblOverlapValue";
            // 
            // LblOverlapEnable
            // 
            resources.ApplyResources(this.LblOverlapEnable, "LblOverlapEnable");
            this.LblOverlapEnable.Name = "LblOverlapEnable";
            // 
            // LblOverlapHeadline
            // 
            resources.ApplyResources(this.LblOverlapHeadline, "LblOverlapHeadline");
            this.LblOverlapHeadline.Name = "LblOverlapHeadline";
            // 
            // BtnLaserTools
            // 
            resources.ApplyResources(this.BtnLaserTools, "BtnLaserTools");
            this.BtnLaserTools.Name = "BtnLaserTools";
            this.BtnLaserTools.UseVisualStyleBackColor = true;
            this.BtnLaserTools.Click += new System.EventHandler(this.BtnStartLaserTools_Click);
            // 
            // LblSetupHeadline
            // 
            resources.ApplyResources(this.LblSetupHeadline, "LblSetupHeadline");
            this.LblSetupHeadline.Name = "LblSetupHeadline";
            // 
            // LblSetup1
            // 
            resources.ApplyResources(this.LblSetup1, "LblSetup1");
            this.LblSetup1.Name = "LblSetup1";
            // 
            // LblSetup2
            // 
            resources.ApplyResources(this.LblSetup2, "LblSetup2");
            this.LblSetup2.Name = "LblSetup2";
            // 
            // LblSetup3
            // 
            resources.ApplyResources(this.LblSetup3, "LblSetup3");
            this.LblSetup3.Name = "LblSetup3";
            // 
            // BtnHelp
            // 
            this.BtnHelp.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.BtnHelp, "BtnHelp");
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Tag = "id=device#laser";
            this.toolTip1.SetToolTip(this.BtnHelp, resources.GetString("BtnHelp.ToolTip"));
            this.BtnHelp.UseVisualStyleBackColor = false;
            this.BtnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // NudDeviceLaserPowerMin
            // 
            this.NudDeviceLaserPowerMin.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceLaserPowerMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceLaserPowerMin.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NudDeviceLaserPowerMin, "NudDeviceLaserPowerMin");
            this.NudDeviceLaserPowerMin.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NudDeviceLaserPowerMin.Name = "NudDeviceLaserPowerMin";
            this.toolTip1.SetToolTip(this.NudDeviceLaserPowerMin, resources.GetString("NudDeviceLaserPowerMin.ToolTip"));
            this.NudDeviceLaserPowerMin.Value = global::GrblPlotter.Properties.Settings.Default.DeviceLaserPowerMin;
            // 
            // CbAirAssist
            // 
            resources.ApplyResources(this.CbAirAssist, "CbAirAssist");
            this.CbAirAssist.Checked = global::GrblPlotter.Properties.Settings.Default.DeviceLaserAir;
            this.CbAirAssist.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "DeviceLaserAir", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbAirAssist.Name = "CbAirAssist";
            this.CbAirAssist.UseVisualStyleBackColor = true;
            this.CbAirAssist.CheckedChanged += new System.EventHandler(this.CbAirAssist_CheckedChanged);
            // 
            // CbLaserpower
            // 
            resources.ApplyResources(this.CbLaserpower, "CbLaserpower");
            this.CbLaserpower.Checked = global::GrblPlotter.Properties.Settings.Default.DeviceLaserM3;
            this.CbLaserpower.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbLaserpower.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "DeviceLaserM3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbLaserpower.Name = "CbLaserpower";
            this.toolTip1.SetToolTip(this.CbLaserpower, resources.GetString("CbLaserpower.ToolTip"));
            this.CbLaserpower.UseVisualStyleBackColor = true;
            this.CbLaserpower.CheckedChanged += new System.EventHandler(this.CbLaserpower_CheckedChanged);
            // 
            // NudDeviceLaserPasses
            // 
            this.NudDeviceLaserPasses.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceLaserPasses", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NudDeviceLaserPasses, "NudDeviceLaserPasses");
            this.NudDeviceLaserPasses.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudDeviceLaserPasses.Name = "NudDeviceLaserPasses";
            this.toolTip1.SetToolTip(this.NudDeviceLaserPasses, resources.GetString("NudDeviceLaserPasses.ToolTip"));
            this.NudDeviceLaserPasses.Value = global::GrblPlotter.Properties.Settings.Default.DeviceLaserPasses;
            this.NudDeviceLaserPasses.ValueChanged += new System.EventHandler(this.CbAirAssist_CheckedChanged);
            // 
            // NudDeviceLaserSpeed
            // 
            this.NudDeviceLaserSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceLaserSpeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceLaserSpeed.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.NudDeviceLaserSpeed, "NudDeviceLaserSpeed");
            this.NudDeviceLaserSpeed.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.NudDeviceLaserSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudDeviceLaserSpeed.Name = "NudDeviceLaserSpeed";
            this.toolTip1.SetToolTip(this.NudDeviceLaserSpeed, resources.GetString("NudDeviceLaserSpeed.ToolTip"));
            this.NudDeviceLaserSpeed.Value = global::GrblPlotter.Properties.Settings.Default.DeviceLaserSpeed;
            this.NudDeviceLaserSpeed.ValueChanged += new System.EventHandler(this.CbAirAssist_CheckedChanged);
            // 
            // NudDeviceLaserPower
            // 
            this.NudDeviceLaserPower.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceLaserPower", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceLaserPower.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NudDeviceLaserPower, "NudDeviceLaserPower");
            this.NudDeviceLaserPower.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NudDeviceLaserPower.Name = "NudDeviceLaserPower";
            this.toolTip1.SetToolTip(this.NudDeviceLaserPower, resources.GetString("NudDeviceLaserPower.ToolTip"));
            this.NudDeviceLaserPower.Value = global::GrblPlotter.Properties.Settings.Default.DeviceLaserPower;
            this.NudDeviceLaserPower.ValueChanged += new System.EventHandler(this.CbAirAssist_CheckedChanged);
            // 
            // BtnOffsetSort
            // 
            resources.ApplyResources(this.BtnOffsetSort, "BtnOffsetSort");
            this.BtnOffsetSort.Name = "BtnOffsetSort";
            this.toolTip1.SetToolTip(this.BtnOffsetSort, resources.GetString("BtnOffsetSort.ToolTip"));
            this.BtnOffsetSort.UseVisualStyleBackColor = true;
            this.BtnOffsetSort.Click += new System.EventHandler(this.BtnOffsetSort_Click);
            // 
            // UCDeviceLaser
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.GbPowerSettings);
            this.Controls.Add(this.BtnOffsetSort);
            this.Controls.Add(this.BtnHelp);
            this.Controls.Add(this.BtnSetup);
            this.Controls.Add(this.NudDeviceLaserPowerMin);
            this.Controls.Add(this.BtnLaserTools);
            this.Controls.Add(this.BtnOverlap);
            this.Controls.Add(this.PanelTranslation);
            this.Controls.Add(this.BtnGetMaterial);
            this.Controls.Add(this.LblPowerMode);
            this.Controls.Add(this.LblPasses);
            this.Controls.Add(this.CbAirAssist);
            this.Controls.Add(this.BtnUseZ);
            this.Controls.Add(this.CbLaserpower);
            this.Controls.Add(this.NudDeviceLaserPasses);
            this.Controls.Add(this.NudDeviceLaserSpeed);
            this.Controls.Add(this.NudDeviceLaserPower);
            this.Controls.Add(this.LblPower);
            this.Controls.Add(this.LblSpeed);
            this.Controls.Add(this.BtnFill);
            this.Controls.Add(this.LblPowerMin);
            this.Name = "UCDeviceLaser";
            this.Load += new System.EventHandler(this.UCDeviceLaser_Load);
            this.SizeChanged += new System.EventHandler(this.UCDeviceLaser_Resize);
            this.Resize += new System.EventHandler(this.UCDeviceLaser_Resize);
            this.GbPowerSettings.ResumeLayout(false);
            this.GbPowerSettings.PerformLayout();
            this.PanelTranslation.ResumeLayout(false);
            this.PanelTranslation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserPowerMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserPasses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceLaserPower)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LblLaserSetVal;
        private System.Windows.Forms.Label LblSet;
        private System.Windows.Forms.Label LblLaserMaxVal;
        private System.Windows.Forms.Label LblLaserMinVal;
        private System.Windows.Forms.Label LblMax;
        private System.Windows.Forms.Label LblMin;
        private System.Windows.Forms.GroupBox GbPowerSettings;
        private System.Windows.Forms.Label LblPower;
        private System.Windows.Forms.NumericUpDown NudDeviceLaserPower;
        private System.Windows.Forms.Label LblPowerMode;
        private System.Windows.Forms.CheckBox CbLaserpower;
        private System.Windows.Forms.Label LblSpeed;
        private System.Windows.Forms.NumericUpDown NudDeviceLaserSpeed;
        private System.Windows.Forms.Button BtnSetup;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox CbAirAssist;
        private System.Windows.Forms.Label LblPasses;
        private System.Windows.Forms.NumericUpDown NudDeviceLaserPasses;
        private System.Windows.Forms.Button BtnFill;
        private System.Windows.Forms.Button BtnGetMaterial;
        private System.Windows.Forms.Button BtnUseZ;
        private System.Windows.Forms.Panel PanelTranslation;
        private System.Windows.Forms.Label LblOverlapValue;
        private System.Windows.Forms.Label LblOverlapEnable;
        private System.Windows.Forms.Label LblOverlapHeadline;
        private System.Windows.Forms.Label LblOverlapInfo;
        private System.Windows.Forms.Label LblZFinal;
        private System.Windows.Forms.Label LblZSave;
        private System.Windows.Forms.Label LbLZFeed;
        private System.Windows.Forms.Label LbLZEnable;
        private System.Windows.Forms.Label LblZInfo;
        private System.Windows.Forms.Label LblZHeadline;
        private System.Windows.Forms.Button BtnOverlap;
        private System.Windows.Forms.Button BtnLaserTools;
        private System.Windows.Forms.NumericUpDown NudDeviceLaserPowerMin;
        private System.Windows.Forms.Label LblPowerMin;
        private System.Windows.Forms.Label LblSetup3;
        private System.Windows.Forms.Label LblSetup2;
        private System.Windows.Forms.Label LblSetup1;
        private System.Windows.Forms.Label LblSetupHeadline;
        private System.Windows.Forms.Button BtnHelp;
        private System.Windows.Forms.Button BtnOffsetSort;
    }
}
