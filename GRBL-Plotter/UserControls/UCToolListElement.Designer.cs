namespace GrblPlotter.UserControls
{
    partial class UCToolListElement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCToolListElement));
            this.panelLaser = new System.Windows.Forms.Panel();
            this.nudLaserDiameter = new System.Windows.Forms.NumericUpDown();
            this.cbLaserAir = new System.Windows.Forms.CheckBox();
            this.cbLaserM3 = new System.Windows.Forms.CheckBox();
            this.nudLaserPasses = new System.Windows.Forms.NumericUpDown();
            this.nudLaserPower = new System.Windows.Forms.NumericUpDown();
            this.nudLaserFeedXY = new System.Windows.Forms.NumericUpDown();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnSetupFill = new System.Windows.Forms.Button();
            this.panelPlotter = new System.Windows.Forms.Panel();
            this.CbPlotterUseLaser = new System.Windows.Forms.CheckBox();
            this.nudPlotterDiameter = new System.Windows.Forms.NumericUpDown();
            this.nudPlotterSPD = new System.Windows.Forms.NumericUpDown();
            this.nudPlotterZPD = new System.Windows.Forms.NumericUpDown();
            this.nudPlotterFeedXY = new System.Windows.Forms.NumericUpDown();
            this.panelRouter = new System.Windows.Forms.Panel();
            this.nudRouterDiameter = new System.Windows.Forms.NumericUpDown();
            this.nudRouterZPD = new System.Windows.Forms.NumericUpDown();
            this.nudRouterFeedZ = new System.Windows.Forms.NumericUpDown();
            this.nudRouterFeedXY = new System.Windows.Forms.NumericUpDown();
            this.panelCoordinates = new System.Windows.Forms.Panel();
            this.tbGcode = new System.Windows.Forms.TextBox();
            this.CmsMoveTo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TsmiMoveToPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLaser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserDiameter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserPasses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserFeedXY)).BeginInit();
            this.panelPlotter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterDiameter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterSPD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterZPD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterFeedXY)).BeginInit();
            this.panelRouter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterDiameter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterZPD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterFeedXY)).BeginInit();
            this.panelCoordinates.SuspendLayout();
            this.CmsMoveTo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLaser
            // 
            this.panelLaser.Controls.Add(this.nudLaserDiameter);
            this.panelLaser.Controls.Add(this.cbLaserAir);
            this.panelLaser.Controls.Add(this.cbLaserM3);
            this.panelLaser.Controls.Add(this.nudLaserPasses);
            this.panelLaser.Controls.Add(this.nudLaserPower);
            this.panelLaser.Controls.Add(this.nudLaserFeedXY);
            resources.ApplyResources(this.panelLaser, "panelLaser");
            this.panelLaser.Name = "panelLaser";
            // 
            // nudLaserDiameter
            // 
            this.nudLaserDiameter.DecimalPlaces = 2;
            this.nudLaserDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudLaserDiameter, "nudLaserDiameter");
            this.nudLaserDiameter.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudLaserDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudLaserDiameter.Name = "nudLaserDiameter";
            this.nudLaserDiameter.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            // 
            // cbLaserAir
            // 
            resources.ApplyResources(this.cbLaserAir, "cbLaserAir");
            this.cbLaserAir.Name = "cbLaserAir";
            this.cbLaserAir.UseVisualStyleBackColor = true;
            this.cbLaserAir.CheckedChanged += new System.EventHandler(this.CbM3_CheckedChanged);
            // 
            // cbLaserM3
            // 
            resources.ApplyResources(this.cbLaserM3, "cbLaserM3");
            this.cbLaserM3.Name = "cbLaserM3";
            this.cbLaserM3.UseVisualStyleBackColor = true;
            this.cbLaserM3.CheckedChanged += new System.EventHandler(this.CbM3_CheckedChanged);
            // 
            // nudLaserPasses
            // 
            resources.ApplyResources(this.nudLaserPasses, "nudLaserPasses");
            this.nudLaserPasses.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLaserPasses.Name = "nudLaserPasses";
            this.nudLaserPasses.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLaserPasses.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudLaserPower
            // 
            this.nudLaserPower.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nudLaserPower, "nudLaserPower");
            this.nudLaserPower.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLaserPower.Name = "nudLaserPower";
            this.nudLaserPower.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLaserPower.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudLaserFeedXY
            // 
            this.nudLaserFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudLaserFeedXY, "nudLaserFeedXY");
            this.nudLaserFeedXY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudLaserFeedXY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudLaserFeedXY.Name = "nudLaserFeedXY";
            this.nudLaserFeedXY.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLaserFeedXY.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // tbName
            // 
            this.tbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            // 
            // btnSetupFill
            // 
            resources.ApplyResources(this.btnSetupFill, "btnSetupFill");
            this.btnSetupFill.Name = "btnSetupFill";
            this.btnSetupFill.UseVisualStyleBackColor = true;
            this.btnSetupFill.Click += new System.EventHandler(this.BtnSetupFill_Click);
            // 
            // panelPlotter
            // 
            this.panelPlotter.Controls.Add(this.CbPlotterUseLaser);
            this.panelPlotter.Controls.Add(this.nudPlotterDiameter);
            this.panelPlotter.Controls.Add(this.nudPlotterSPD);
            this.panelPlotter.Controls.Add(this.nudPlotterZPD);
            this.panelPlotter.Controls.Add(this.nudPlotterFeedXY);
            resources.ApplyResources(this.panelPlotter, "panelPlotter");
            this.panelPlotter.Name = "panelPlotter";
            // 
            // CbPlotterUseLaser
            // 
            resources.ApplyResources(this.CbPlotterUseLaser, "CbPlotterUseLaser");
            this.CbPlotterUseLaser.Name = "CbPlotterUseLaser";
            this.CbPlotterUseLaser.UseVisualStyleBackColor = true;
            this.CbPlotterUseLaser.CheckedChanged += new System.EventHandler(this.CbPlotterUseLaser_CheckedChanged);
            // 
            // nudPlotterDiameter
            // 
            this.nudPlotterDiameter.DecimalPlaces = 2;
            this.nudPlotterDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudPlotterDiameter, "nudPlotterDiameter");
            this.nudPlotterDiameter.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudPlotterDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudPlotterDiameter.Name = "nudPlotterDiameter";
            this.nudPlotterDiameter.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            // 
            // nudPlotterSPD
            // 
            resources.ApplyResources(this.nudPlotterSPD, "nudPlotterSPD");
            this.nudPlotterSPD.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudPlotterSPD.Name = "nudPlotterSPD";
            this.nudPlotterSPD.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPlotterSPD.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudPlotterZPD
            // 
            this.nudPlotterZPD.DecimalPlaces = 1;
            this.nudPlotterZPD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudPlotterZPD, "nudPlotterZPD");
            this.nudPlotterZPD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudPlotterZPD.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudPlotterZPD.Name = "nudPlotterZPD";
            this.nudPlotterZPD.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // nudPlotterFeedXY
            // 
            this.nudPlotterFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudPlotterFeedXY, "nudPlotterFeedXY");
            this.nudPlotterFeedXY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudPlotterFeedXY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPlotterFeedXY.Name = "nudPlotterFeedXY";
            this.nudPlotterFeedXY.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudPlotterFeedXY.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // panelRouter
            // 
            this.panelRouter.Controls.Add(this.nudRouterDiameter);
            this.panelRouter.Controls.Add(this.nudRouterZPD);
            this.panelRouter.Controls.Add(this.nudRouterFeedZ);
            this.panelRouter.Controls.Add(this.nudRouterFeedXY);
            resources.ApplyResources(this.panelRouter, "panelRouter");
            this.panelRouter.Name = "panelRouter";
            // 
            // nudRouterDiameter
            // 
            this.nudRouterDiameter.DecimalPlaces = 2;
            this.nudRouterDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudRouterDiameter, "nudRouterDiameter");
            this.nudRouterDiameter.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudRouterDiameter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudRouterDiameter.Name = "nudRouterDiameter";
            this.nudRouterDiameter.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            // 
            // nudRouterZPD
            // 
            this.nudRouterZPD.DecimalPlaces = 2;
            this.nudRouterZPD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nudRouterZPD, "nudRouterZPD");
            this.nudRouterZPD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudRouterZPD.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudRouterZPD.Name = "nudRouterZPD";
            this.nudRouterZPD.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.nudRouterZPD.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudRouterFeedZ
            // 
            this.nudRouterFeedZ.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudRouterFeedZ, "nudRouterFeedZ");
            this.nudRouterFeedZ.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudRouterFeedZ.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRouterFeedZ.Name = "nudRouterFeedZ";
            this.nudRouterFeedZ.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudRouterFeedZ.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudRouterFeedXY
            // 
            this.nudRouterFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nudRouterFeedXY, "nudRouterFeedXY");
            this.nudRouterFeedXY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudRouterFeedXY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRouterFeedXY.Name = "nudRouterFeedXY";
            this.nudRouterFeedXY.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudRouterFeedXY.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // panelCoordinates
            // 
            this.panelCoordinates.Controls.Add(this.tbGcode);
            resources.ApplyResources(this.panelCoordinates, "panelCoordinates");
            this.panelCoordinates.Name = "panelCoordinates";
            // 
            // tbGcode
            // 
            this.tbGcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.tbGcode, "tbGcode");
            this.tbGcode.Name = "tbGcode";
            // 
            // CmsMoveTo
            // 
            this.CmsMoveTo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiMoveToPosition});
            this.CmsMoveTo.Name = "CmsMoveTo";
            resources.ApplyResources(this.CmsMoveTo, "CmsMoveTo");
            // 
            // TsmiMoveToPosition
            // 
            this.TsmiMoveToPosition.Name = "TsmiMoveToPosition";
            resources.ApplyResources(this.TsmiMoveToPosition, "TsmiMoveToPosition");
            this.TsmiMoveToPosition.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // UCToolListElement
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelCoordinates);
            this.Controls.Add(this.panelRouter);
            this.Controls.Add(this.panelPlotter);
            this.Controls.Add(this.btnSetupFill);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.panelLaser);
            this.Name = "UCToolListElement";
            this.Load += new System.EventHandler(this.UCToolListElement_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UCToolListElement_Paint);
            this.panelLaser.ResumeLayout(false);
            this.panelLaser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserDiameter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserPasses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLaserFeedXY)).EndInit();
            this.panelPlotter.ResumeLayout(false);
            this.panelPlotter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterDiameter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterSPD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterZPD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPlotterFeedXY)).EndInit();
            this.panelRouter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterDiameter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterZPD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterFeedZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRouterFeedXY)).EndInit();
            this.panelCoordinates.ResumeLayout(false);
            this.panelCoordinates.PerformLayout();
            this.CmsMoveTo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelLaser;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.NumericUpDown nudLaserFeedXY;
        private System.Windows.Forms.NumericUpDown nudLaserPower;
        private System.Windows.Forms.NumericUpDown nudLaserPasses;
        private System.Windows.Forms.CheckBox cbLaserM3;
        private System.Windows.Forms.CheckBox cbLaserAir;
        private System.Windows.Forms.Button btnSetupFill;
        private System.Windows.Forms.Panel panelPlotter;
        private System.Windows.Forms.NumericUpDown nudPlotterFeedXY;
        private System.Windows.Forms.Panel panelRouter;
        private System.Windows.Forms.NumericUpDown nudRouterFeedXY;
        private System.Windows.Forms.Panel panelCoordinates;
        private System.Windows.Forms.NumericUpDown nudRouterFeedZ;
        private System.Windows.Forms.NumericUpDown nudPlotterZPD;
        private System.Windows.Forms.NumericUpDown nudRouterZPD;
        private System.Windows.Forms.NumericUpDown nudPlotterSPD;
        private System.Windows.Forms.TextBox tbGcode;
        private System.Windows.Forms.NumericUpDown nudLaserDiameter;
        private System.Windows.Forms.NumericUpDown nudPlotterDiameter;
        private System.Windows.Forms.NumericUpDown nudRouterDiameter;
        private System.Windows.Forms.CheckBox CbPlotterUseLaser;
        private System.Windows.Forms.ContextMenuStrip CmsMoveTo;
        private System.Windows.Forms.ToolStripMenuItem TsmiMoveToPosition;
    }
}
