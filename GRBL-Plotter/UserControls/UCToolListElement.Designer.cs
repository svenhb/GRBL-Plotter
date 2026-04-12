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
            this.nudCoordA = new System.Windows.Forms.NumericUpDown();
            this.nudCoordZ = new System.Windows.Forms.NumericUpDown();
            this.nudCoordY = new System.Windows.Forms.NumericUpDown();
            this.nudCoordX = new System.Windows.Forms.NumericUpDown();
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
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordX)).BeginInit();
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
            this.panelLaser.Location = new System.Drawing.Point(60, 0);
            this.panelLaser.Name = "panelLaser";
            this.panelLaser.Size = new System.Drawing.Size(264, 23);
            this.panelLaser.TabIndex = 0;
            // 
            // nudLaserDiameter
            // 
            this.nudLaserDiameter.DecimalPlaces = 2;
            this.nudLaserDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudLaserDiameter.Location = new System.Drawing.Point(0, 1);
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
            this.nudLaserDiameter.Size = new System.Drawing.Size(50, 20);
            this.nudLaserDiameter.TabIndex = 9;
            this.nudLaserDiameter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLaserDiameter.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            // 
            // cbLaserAir
            // 
            this.cbLaserAir.AutoSize = true;
            this.cbLaserAir.Location = new System.Drawing.Point(243, 5);
            this.cbLaserAir.Name = "cbLaserAir";
            this.cbLaserAir.Size = new System.Drawing.Size(15, 14);
            this.cbLaserAir.TabIndex = 2;
            this.cbLaserAir.UseVisualStyleBackColor = true;
            this.cbLaserAir.CheckedChanged += new System.EventHandler(this.CbM3_CheckedChanged);
            // 
            // cbLaserM3
            // 
            this.cbLaserM3.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbLaserM3.Location = new System.Drawing.Point(204, 1);
            this.cbLaserM3.Name = "cbLaserM3";
            this.cbLaserM3.Size = new System.Drawing.Size(30, 22);
            this.cbLaserM3.TabIndex = 2;
            this.cbLaserM3.Text = "M3";
            this.cbLaserM3.UseVisualStyleBackColor = true;
            this.cbLaserM3.CheckedChanged += new System.EventHandler(this.CbM3_CheckedChanged);
            // 
            // nudLaserPasses
            // 
            this.nudLaserPasses.Location = new System.Drawing.Point(163, 1);
            this.nudLaserPasses.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLaserPasses.Name = "nudLaserPasses";
            this.nudLaserPasses.Size = new System.Drawing.Size(40, 20);
            this.nudLaserPasses.TabIndex = 4;
            this.nudLaserPasses.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nudLaserPower.Location = new System.Drawing.Point(112, 1);
            this.nudLaserPower.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudLaserPower.Name = "nudLaserPower";
            this.nudLaserPower.Size = new System.Drawing.Size(50, 20);
            this.nudLaserPower.TabIndex = 3;
            this.nudLaserPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nudLaserFeedXY.Location = new System.Drawing.Point(51, 1);
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
            this.nudLaserFeedXY.Size = new System.Drawing.Size(60, 20);
            this.nudLaserFeedXY.TabIndex = 2;
            this.nudLaserFeedXY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.tbName.Location = new System.Drawing.Point(24, 2);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(85, 20);
            this.tbName.TabIndex = 1;
            // 
            // btnSetupFill
            // 
            this.btnSetupFill.Location = new System.Drawing.Point(324, 1);
            this.btnSetupFill.Name = "btnSetupFill";
            this.btnSetupFill.Size = new System.Drawing.Size(30, 23);
            this.btnSetupFill.TabIndex = 2;
            this.btnSetupFill.Text = "Fill";
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
            this.panelPlotter.Location = new System.Drawing.Point(110, 30);
            this.panelPlotter.Name = "panelPlotter";
            this.panelPlotter.Size = new System.Drawing.Size(214, 23);
            this.panelPlotter.TabIndex = 5;
            // 
            // CbPlotterUseLaser
            // 
            this.CbPlotterUseLaser.AutoSize = true;
            this.CbPlotterUseLaser.Location = new System.Drawing.Point(180, 4);
            this.CbPlotterUseLaser.Name = "CbPlotterUseLaser";
            this.CbPlotterUseLaser.Size = new System.Drawing.Size(15, 14);
            this.CbPlotterUseLaser.TabIndex = 11;
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
            this.nudPlotterDiameter.Location = new System.Drawing.Point(0, 1);
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
            this.nudPlotterDiameter.Size = new System.Drawing.Size(50, 20);
            this.nudPlotterDiameter.TabIndex = 10;
            this.nudPlotterDiameter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPlotterDiameter.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            // 
            // nudPlotterSPD
            // 
            this.nudPlotterSPD.Location = new System.Drawing.Point(112, 1);
            this.nudPlotterSPD.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudPlotterSPD.Name = "nudPlotterSPD";
            this.nudPlotterSPD.Size = new System.Drawing.Size(50, 20);
            this.nudPlotterSPD.TabIndex = 5;
            this.nudPlotterSPD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nudPlotterZPD.Location = new System.Drawing.Point(112, 1);
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
            this.nudPlotterZPD.Size = new System.Drawing.Size(50, 20);
            this.nudPlotterZPD.TabIndex = 4;
            this.nudPlotterZPD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nudPlotterFeedXY.Location = new System.Drawing.Point(51, 1);
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
            this.nudPlotterFeedXY.Size = new System.Drawing.Size(60, 20);
            this.nudPlotterFeedXY.TabIndex = 2;
            this.nudPlotterFeedXY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.panelRouter.Location = new System.Drawing.Point(110, 60);
            this.panelRouter.Name = "panelRouter";
            this.panelRouter.Size = new System.Drawing.Size(214, 23);
            this.panelRouter.TabIndex = 6;
            // 
            // nudRouterDiameter
            // 
            this.nudRouterDiameter.DecimalPlaces = 2;
            this.nudRouterDiameter.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudRouterDiameter.Location = new System.Drawing.Point(0, 1);
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
            this.nudRouterDiameter.Size = new System.Drawing.Size(50, 20);
            this.nudRouterDiameter.TabIndex = 11;
            this.nudRouterDiameter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nudRouterZPD.Location = new System.Drawing.Point(163, 1);
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
            this.nudRouterZPD.Size = new System.Drawing.Size(50, 20);
            this.nudRouterZPD.TabIndex = 4;
            this.nudRouterZPD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nudRouterFeedZ.Location = new System.Drawing.Point(112, 1);
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
            this.nudRouterFeedZ.Size = new System.Drawing.Size(50, 20);
            this.nudRouterFeedZ.TabIndex = 3;
            this.nudRouterFeedZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nudRouterFeedXY.Location = new System.Drawing.Point(51, 1);
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
            this.nudRouterFeedXY.Size = new System.Drawing.Size(60, 20);
            this.nudRouterFeedXY.TabIndex = 2;
            this.nudRouterFeedXY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.panelCoordinates.Controls.Add(this.nudCoordA);
            this.panelCoordinates.Controls.Add(this.nudCoordZ);
            this.panelCoordinates.Controls.Add(this.nudCoordY);
            this.panelCoordinates.Controls.Add(this.nudCoordX);
            this.panelCoordinates.Location = new System.Drawing.Point(24, 90);
            this.panelCoordinates.Name = "panelCoordinates";
            this.panelCoordinates.Size = new System.Drawing.Size(330, 23);
            this.panelCoordinates.TabIndex = 7;
            // 
            // tbGcode
            // 
            this.tbGcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbGcode.Location = new System.Drawing.Point(208, 2);
            this.tbGcode.Name = "tbGcode";
            this.tbGcode.Size = new System.Drawing.Size(119, 20);
            this.tbGcode.TabIndex = 8;
            // 
            // nudCoordA
            // 
            this.nudCoordA.DecimalPlaces = 1;
            this.nudCoordA.Location = new System.Drawing.Point(156, 1);
            this.nudCoordA.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCoordA.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudCoordA.Name = "nudCoordA";
            this.nudCoordA.Size = new System.Drawing.Size(50, 20);
            this.nudCoordA.TabIndex = 5;
            this.nudCoordA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCoordA.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudCoordZ
            // 
            this.nudCoordZ.DecimalPlaces = 1;
            this.nudCoordZ.Location = new System.Drawing.Point(104, 1);
            this.nudCoordZ.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCoordZ.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudCoordZ.Name = "nudCoordZ";
            this.nudCoordZ.Size = new System.Drawing.Size(50, 20);
            this.nudCoordZ.TabIndex = 4;
            this.nudCoordZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCoordZ.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudCoordY
            // 
            this.nudCoordY.DecimalPlaces = 1;
            this.nudCoordY.Location = new System.Drawing.Point(52, 1);
            this.nudCoordY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCoordY.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudCoordY.Name = "nudCoordY";
            this.nudCoordY.Size = new System.Drawing.Size(50, 20);
            this.nudCoordY.TabIndex = 3;
            this.nudCoordY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCoordY.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // nudCoordX
            // 
            this.nudCoordX.DecimalPlaces = 1;
            this.nudCoordX.Location = new System.Drawing.Point(0, 1);
            this.nudCoordX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCoordX.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nudCoordX.Name = "nudCoordX";
            this.nudCoordX.Size = new System.Drawing.Size(50, 20);
            this.nudCoordX.TabIndex = 2;
            this.nudCoordX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCoordX.ValueChanged += new System.EventHandler(this.Nud_ValueChanged);
            // 
            // CmsMoveTo
            // 
            this.CmsMoveTo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiMoveToPosition});
            this.CmsMoveTo.Name = "CmsMoveTo";
            this.CmsMoveTo.Size = new System.Drawing.Size(182, 48);
            // 
            // TsmiMoveToPosition
            // 
            this.TsmiMoveToPosition.Name = "TsmiMoveToPosition";
            this.TsmiMoveToPosition.Size = new System.Drawing.Size(181, 22);
            this.TsmiMoveToPosition.Text = "Move to XY position";
            this.TsmiMoveToPosition.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // UCToolListElement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelCoordinates);
            this.Controls.Add(this.panelRouter);
            this.Controls.Add(this.panelPlotter);
            this.Controls.Add(this.btnSetupFill);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.panelLaser);
            this.Name = "UCToolListElement";
            this.Size = new System.Drawing.Size(583, 113);
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
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCoordX)).EndInit();
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
        private System.Windows.Forms.NumericUpDown nudCoordX;
        private System.Windows.Forms.NumericUpDown nudCoordA;
        private System.Windows.Forms.NumericUpDown nudCoordZ;
        private System.Windows.Forms.NumericUpDown nudCoordY;
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
