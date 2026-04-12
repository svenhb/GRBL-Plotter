namespace GrblPlotter.UserControls
{
    partial class UCDeviceRouter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDeviceRouter));
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnSetup = new System.Windows.Forms.Button();
            this.LblSpindleSpeed = new System.Windows.Forms.Label();
            this.GbSpindleSpeed = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.LblSpindleMinVal = new System.Windows.Forms.Label();
            this.LblSpindleMaxVal = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.LblSpindleSetVal = new System.Windows.Forms.Label();
            this.NudDeviceRouterSpindle = new System.Windows.Forms.NumericUpDown();
            this.NudDeviceRouterFeedZ = new System.Windows.Forms.NumericUpDown();
            this.NudDeviceRouterZDown = new System.Windows.Forms.NumericUpDown();
            this.NudDeviceRouterZUp = new System.Windows.Forms.NumericUpDown();
            this.NudDeviceRouterFeedXY = new System.Windows.Forms.NumericUpDown();
            this.BtnProbing = new System.Windows.Forms.Button();
            this.BtnHeightmap = new System.Windows.Forms.Button();
            this.PanelTranslation = new System.Windows.Forms.Panel();
            this.LblDragToolInfo = new System.Windows.Forms.Label();
            this.LblDragToolHeadline = new System.Windows.Forms.Label();
            this.LblDragToolTangentialEnable = new System.Windows.Forms.Label();
            this.LblDragToolAngle = new System.Windows.Forms.Label();
            this.LblDragToolPercent = new System.Windows.Forms.Label();
            this.LblDragToolPercentEnable = new System.Windows.Forms.Label();
            this.LblDragToolLength = new System.Windows.Forms.Label();
            this.LblDragToolEnable = new System.Windows.Forms.Label();
            this.LblTangentialInfo = new System.Windows.Forms.Label();
            this.LblTangentialHeadline = new System.Windows.Forms.Label();
            this.LblTangentialPathShortening = new System.Windows.Forms.Label();
            this.LblTangentialPathShorteningEnable = new System.Windows.Forms.Label();
            this.LblTangentialLimitAngle = new System.Windows.Forms.Label();
            this.LblTangentialUnitsPerTurn = new System.Windows.Forms.Label();
            this.LblTangentialDeviAngle = new System.Windows.Forms.Label();
            this.LblTangentialSwivelAngle = new System.Windows.Forms.Label();
            this.LblTangentialAxisName = new System.Windows.Forms.Label();
            this.LblTangentialEnable = new System.Windows.Forms.Label();
            this.BtnTangential = new System.Windows.Forms.Button();
            this.BtnDragTool = new System.Windows.Forms.Button();
            this.LblSetupHeadline = new System.Windows.Forms.Label();
            this.LblSetup1 = new System.Windows.Forms.Label();
            this.LblSetup2 = new System.Windows.Forms.Label();
            this.BtnHelp = new System.Windows.Forms.Button();
            this.GbSpindleSpeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterSpindle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterZDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterZUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterFeedXY)).BeginInit();
            this.PanelTranslation.SuspendLayout();
            this.SuspendLayout();
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // BtnSetup
            // 
            resources.ApplyResources(this.BtnSetup, "BtnSetup");
            this.BtnSetup.Name = "BtnSetup";
            this.BtnSetup.UseVisualStyleBackColor = true;
            this.BtnSetup.Click += new System.EventHandler(this.BtnSetup_Click);
            // 
            // LblSpindleSpeed
            // 
            resources.ApplyResources(this.LblSpindleSpeed, "LblSpindleSpeed");
            this.LblSpindleSpeed.BackColor = System.Drawing.Color.Yellow;
            this.LblSpindleSpeed.Name = "LblSpindleSpeed";
            this.LblSpindleSpeed.MouseEnter += new System.EventHandler(this.LblSpindleSpeed_MouseEnter);
            this.LblSpindleSpeed.MouseLeave += new System.EventHandler(this.LblSpindleSpeed_MouseLeave);
            // 
            // GbSpindleSpeed
            // 
            this.GbSpindleSpeed.Controls.Add(this.label14);
            this.GbSpindleSpeed.Controls.Add(this.LblSpindleMinVal);
            this.GbSpindleSpeed.Controls.Add(this.LblSpindleMaxVal);
            this.GbSpindleSpeed.Controls.Add(this.label17);
            this.GbSpindleSpeed.Controls.Add(this.label18);
            this.GbSpindleSpeed.Controls.Add(this.LblSpindleSetVal);
            resources.ApplyResources(this.GbSpindleSpeed, "GbSpindleSpeed");
            this.GbSpindleSpeed.Name = "GbSpindleSpeed";
            this.GbSpindleSpeed.TabStop = false;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // LblSpindleMinVal
            // 
            resources.ApplyResources(this.LblSpindleMinVal, "LblSpindleMinVal");
            this.LblSpindleMinVal.Name = "LblSpindleMinVal";
            // 
            // LblSpindleMaxVal
            // 
            resources.ApplyResources(this.LblSpindleMaxVal, "LblSpindleMaxVal");
            this.LblSpindleMaxVal.Name = "LblSpindleMaxVal";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // LblSpindleSetVal
            // 
            resources.ApplyResources(this.LblSpindleSetVal, "LblSpindleSetVal");
            this.LblSpindleSetVal.Name = "LblSpindleSetVal";
            // 
            // NudDeviceRouterSpindle
            // 
            this.NudDeviceRouterSpindle.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceRouterSpindle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceRouterSpindle.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.NudDeviceRouterSpindle, "NudDeviceRouterSpindle");
            this.NudDeviceRouterSpindle.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NudDeviceRouterSpindle.Name = "NudDeviceRouterSpindle";
            this.NudDeviceRouterSpindle.Value = global::GrblPlotter.Properties.Settings.Default.DeviceRouterSpindle;
            // 
            // NudDeviceRouterFeedZ
            // 
            this.NudDeviceRouterFeedZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceRouterSpeedZ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceRouterFeedZ.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.NudDeviceRouterFeedZ, "NudDeviceRouterFeedZ");
            this.NudDeviceRouterFeedZ.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.NudDeviceRouterFeedZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudDeviceRouterFeedZ.Name = "NudDeviceRouterFeedZ";
            this.NudDeviceRouterFeedZ.Value = global::GrblPlotter.Properties.Settings.Default.DeviceRouterSpeedZ;
            this.NudDeviceRouterFeedZ.ValueChanged += new System.EventHandler(this.NudDeviceRouter_ValueChanged);
            // 
            // NudDeviceRouterZDown
            // 
            this.NudDeviceRouterZDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceRouterZDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceRouterZDown.DecimalPlaces = 1;
            this.NudDeviceRouterZDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudDeviceRouterZDown, "NudDeviceRouterZDown");
            this.NudDeviceRouterZDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.NudDeviceRouterZDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.NudDeviceRouterZDown.Name = "NudDeviceRouterZDown";
            this.NudDeviceRouterZDown.Value = global::GrblPlotter.Properties.Settings.Default.DeviceRouterZDown;
            this.NudDeviceRouterZDown.ValueChanged += new System.EventHandler(this.NudDeviceRouter_ValueChanged);
            // 
            // NudDeviceRouterZUp
            // 
            this.NudDeviceRouterZUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceRouterZUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceRouterZUp.DecimalPlaces = 1;
            resources.ApplyResources(this.NudDeviceRouterZUp, "NudDeviceRouterZUp");
            this.NudDeviceRouterZUp.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudDeviceRouterZUp.Name = "NudDeviceRouterZUp";
            this.NudDeviceRouterZUp.Value = global::GrblPlotter.Properties.Settings.Default.DeviceRouterZUp;
            this.NudDeviceRouterZUp.ValueChanged += new System.EventHandler(this.NudDeviceRouter_ValueChanged);
            // 
            // NudDeviceRouterFeedXY
            // 
            this.NudDeviceRouterFeedXY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "DeviceRouterSpeedXY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDeviceRouterFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.NudDeviceRouterFeedXY, "NudDeviceRouterFeedXY");
            this.NudDeviceRouterFeedXY.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.NudDeviceRouterFeedXY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudDeviceRouterFeedXY.Name = "NudDeviceRouterFeedXY";
            this.NudDeviceRouterFeedXY.Value = global::GrblPlotter.Properties.Settings.Default.DeviceRouterSpeedXY;
            this.NudDeviceRouterFeedXY.ValueChanged += new System.EventHandler(this.NudDeviceRouter_ValueChanged);
            // 
            // BtnProbing
            // 
            resources.ApplyResources(this.BtnProbing, "BtnProbing");
            this.BtnProbing.Name = "BtnProbing";
            this.BtnProbing.UseVisualStyleBackColor = true;
            this.BtnProbing.Click += new System.EventHandler(this.BtnStartProbing_Click);
            // 
            // BtnHeightmap
            // 
            resources.ApplyResources(this.BtnHeightmap, "BtnHeightmap");
            this.BtnHeightmap.Name = "BtnHeightmap";
            this.BtnHeightmap.UseVisualStyleBackColor = true;
            this.BtnHeightmap.Click += new System.EventHandler(this.BtnStartHeightmap_Click);
            // 
            // PanelTranslation
            // 
            this.PanelTranslation.Controls.Add(this.LblSetup2);
            this.PanelTranslation.Controls.Add(this.LblSetup1);
            this.PanelTranslation.Controls.Add(this.LblSetupHeadline);
            this.PanelTranslation.Controls.Add(this.LblDragToolInfo);
            this.PanelTranslation.Controls.Add(this.LblDragToolHeadline);
            this.PanelTranslation.Controls.Add(this.LblDragToolTangentialEnable);
            this.PanelTranslation.Controls.Add(this.LblDragToolAngle);
            this.PanelTranslation.Controls.Add(this.LblDragToolPercent);
            this.PanelTranslation.Controls.Add(this.LblDragToolPercentEnable);
            this.PanelTranslation.Controls.Add(this.LblDragToolLength);
            this.PanelTranslation.Controls.Add(this.LblDragToolEnable);
            this.PanelTranslation.Controls.Add(this.LblTangentialInfo);
            this.PanelTranslation.Controls.Add(this.LblTangentialHeadline);
            this.PanelTranslation.Controls.Add(this.LblTangentialPathShortening);
            this.PanelTranslation.Controls.Add(this.LblTangentialPathShorteningEnable);
            this.PanelTranslation.Controls.Add(this.LblTangentialLimitAngle);
            this.PanelTranslation.Controls.Add(this.LblTangentialUnitsPerTurn);
            this.PanelTranslation.Controls.Add(this.LblTangentialDeviAngle);
            this.PanelTranslation.Controls.Add(this.LblTangentialSwivelAngle);
            this.PanelTranslation.Controls.Add(this.LblTangentialAxisName);
            this.PanelTranslation.Controls.Add(this.LblTangentialEnable);
            resources.ApplyResources(this.PanelTranslation, "PanelTranslation");
            this.PanelTranslation.Name = "PanelTranslation";
            // 
            // LblDragToolInfo
            // 
            resources.ApplyResources(this.LblDragToolInfo, "LblDragToolInfo");
            this.LblDragToolInfo.Name = "LblDragToolInfo";
            // 
            // LblDragToolHeadline
            // 
            resources.ApplyResources(this.LblDragToolHeadline, "LblDragToolHeadline");
            this.LblDragToolHeadline.Name = "LblDragToolHeadline";
            // 
            // LblDragToolTangentialEnable
            // 
            resources.ApplyResources(this.LblDragToolTangentialEnable, "LblDragToolTangentialEnable");
            this.LblDragToolTangentialEnable.Name = "LblDragToolTangentialEnable";
            // 
            // LblDragToolAngle
            // 
            resources.ApplyResources(this.LblDragToolAngle, "LblDragToolAngle");
            this.LblDragToolAngle.Name = "LblDragToolAngle";
            // 
            // LblDragToolPercent
            // 
            resources.ApplyResources(this.LblDragToolPercent, "LblDragToolPercent");
            this.LblDragToolPercent.Name = "LblDragToolPercent";
            // 
            // LblDragToolPercentEnable
            // 
            resources.ApplyResources(this.LblDragToolPercentEnable, "LblDragToolPercentEnable");
            this.LblDragToolPercentEnable.Name = "LblDragToolPercentEnable";
            // 
            // LblDragToolLength
            // 
            resources.ApplyResources(this.LblDragToolLength, "LblDragToolLength");
            this.LblDragToolLength.Name = "LblDragToolLength";
            // 
            // LblDragToolEnable
            // 
            resources.ApplyResources(this.LblDragToolEnable, "LblDragToolEnable");
            this.LblDragToolEnable.Name = "LblDragToolEnable";
            // 
            // LblTangentialInfo
            // 
            resources.ApplyResources(this.LblTangentialInfo, "LblTangentialInfo");
            this.LblTangentialInfo.Name = "LblTangentialInfo";
            // 
            // LblTangentialHeadline
            // 
            resources.ApplyResources(this.LblTangentialHeadline, "LblTangentialHeadline");
            this.LblTangentialHeadline.Name = "LblTangentialHeadline";
            // 
            // LblTangentialPathShortening
            // 
            resources.ApplyResources(this.LblTangentialPathShortening, "LblTangentialPathShortening");
            this.LblTangentialPathShortening.Name = "LblTangentialPathShortening";
            // 
            // LblTangentialPathShorteningEnable
            // 
            resources.ApplyResources(this.LblTangentialPathShorteningEnable, "LblTangentialPathShorteningEnable");
            this.LblTangentialPathShorteningEnable.Name = "LblTangentialPathShorteningEnable";
            // 
            // LblTangentialLimitAngle
            // 
            resources.ApplyResources(this.LblTangentialLimitAngle, "LblTangentialLimitAngle");
            this.LblTangentialLimitAngle.Name = "LblTangentialLimitAngle";
            // 
            // LblTangentialUnitsPerTurn
            // 
            resources.ApplyResources(this.LblTangentialUnitsPerTurn, "LblTangentialUnitsPerTurn");
            this.LblTangentialUnitsPerTurn.Name = "LblTangentialUnitsPerTurn";
            // 
            // LblTangentialDeviAngle
            // 
            resources.ApplyResources(this.LblTangentialDeviAngle, "LblTangentialDeviAngle");
            this.LblTangentialDeviAngle.Name = "LblTangentialDeviAngle";
            // 
            // LblTangentialSwivelAngle
            // 
            resources.ApplyResources(this.LblTangentialSwivelAngle, "LblTangentialSwivelAngle");
            this.LblTangentialSwivelAngle.Name = "LblTangentialSwivelAngle";
            // 
            // LblTangentialAxisName
            // 
            resources.ApplyResources(this.LblTangentialAxisName, "LblTangentialAxisName");
            this.LblTangentialAxisName.Name = "LblTangentialAxisName";
            // 
            // LblTangentialEnable
            // 
            resources.ApplyResources(this.LblTangentialEnable, "LblTangentialEnable");
            this.LblTangentialEnable.Name = "LblTangentialEnable";
            // 
            // BtnTangential
            // 
            resources.ApplyResources(this.BtnTangential, "BtnTangential");
            this.BtnTangential.Name = "BtnTangential";
            this.BtnTangential.UseVisualStyleBackColor = true;
            this.BtnTangential.Click += new System.EventHandler(this.BtnTangential_Click);
            // 
            // BtnDragTool
            // 
            resources.ApplyResources(this.BtnDragTool, "BtnDragTool");
            this.BtnDragTool.Name = "BtnDragTool";
            this.BtnDragTool.UseVisualStyleBackColor = true;
            this.BtnDragTool.Click += new System.EventHandler(this.BtnDragTool_Click);
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
            // BtnHelp
            // 
            this.BtnHelp.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.BtnHelp, "BtnHelp");
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Tag = "id=device#router";
            this.BtnHelp.UseVisualStyleBackColor = false;
            this.BtnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // UCDeviceRouter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.BtnHelp);
            this.Controls.Add(this.BtnSetup);
            this.Controls.Add(this.GbSpindleSpeed);
            this.Controls.Add(this.BtnDragTool);
            this.Controls.Add(this.BtnTangential);
            this.Controls.Add(this.PanelTranslation);
            this.Controls.Add(this.BtnHeightmap);
            this.Controls.Add(this.BtnProbing);
            this.Controls.Add(this.LblSpindleSpeed);
            this.Controls.Add(this.NudDeviceRouterSpindle);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.NudDeviceRouterFeedZ);
            this.Controls.Add(this.NudDeviceRouterZDown);
            this.Controls.Add(this.NudDeviceRouterZUp);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.NudDeviceRouterFeedXY);
            this.Name = "UCDeviceRouter";
            this.Load += new System.EventHandler(this.UCDeviceRouter_Load);
            this.Resize += new System.EventHandler(this.UCDeviceRouter_Resize);
            this.GbSpindleSpeed.ResumeLayout(false);
            this.GbSpindleSpeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterSpindle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterFeedZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterZDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterZUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDeviceRouterFeedXY)).EndInit();
            this.PanelTranslation.ResumeLayout(false);
            this.PanelTranslation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown NudDeviceRouterFeedXY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NudDeviceRouterFeedZ;
        internal System.Windows.Forms.NumericUpDown NudDeviceRouterZDown;
        internal System.Windows.Forms.NumericUpDown NudDeviceRouterZUp;
        private System.Windows.Forms.Button BtnSetup;
        private System.Windows.Forms.Label LblSpindleSpeed;
        private System.Windows.Forms.NumericUpDown NudDeviceRouterSpindle;
        private System.Windows.Forms.GroupBox GbSpindleSpeed;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label LblSpindleMinVal;
        private System.Windows.Forms.Label LblSpindleMaxVal;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label LblSpindleSetVal;
        private System.Windows.Forms.Button BtnProbing;
        private System.Windows.Forms.Button BtnHeightmap;
        private System.Windows.Forms.Panel PanelTranslation;
        private System.Windows.Forms.Label LblTangentialUnitsPerTurn;
        private System.Windows.Forms.Label LblTangentialDeviAngle;
        private System.Windows.Forms.Label LblTangentialSwivelAngle;
        private System.Windows.Forms.Label LblTangentialAxisName;
        private System.Windows.Forms.Label LblTangentialEnable;
        private System.Windows.Forms.Label LblTangentialPathShortening;
        private System.Windows.Forms.Label LblTangentialPathShorteningEnable;
        private System.Windows.Forms.Label LblTangentialLimitAngle;
        private System.Windows.Forms.Label LblTangentialInfo;
        private System.Windows.Forms.Label LblTangentialHeadline;
        private System.Windows.Forms.Label LblDragToolInfo;
        private System.Windows.Forms.Label LblDragToolHeadline;
        private System.Windows.Forms.Label LblDragToolTangentialEnable;
        private System.Windows.Forms.Label LblDragToolAngle;
        private System.Windows.Forms.Label LblDragToolPercent;
        private System.Windows.Forms.Label LblDragToolPercentEnable;
        private System.Windows.Forms.Label LblDragToolLength;
        private System.Windows.Forms.Label LblDragToolEnable;
        private System.Windows.Forms.Button BtnTangential;
        private System.Windows.Forms.Button BtnDragTool;
        private System.Windows.Forms.Label LblSetup2;
        private System.Windows.Forms.Label LblSetup1;
        private System.Windows.Forms.Label LblSetupHeadline;
        private System.Windows.Forms.Button BtnHelp;
    }
}
