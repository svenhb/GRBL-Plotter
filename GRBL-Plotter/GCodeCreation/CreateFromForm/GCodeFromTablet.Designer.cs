namespace GrblPlotter
{
    partial class GCodeFromTablet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeFromTablet));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.BtnHelp = new System.Windows.Forms.Button();
            this.CboxPapersize = new System.Windows.Forms.ComboBox();
            this.CbUpdate = new System.Windows.Forms.CheckBox();
            this.RbMode3 = new System.Windows.Forms.RadioButton();
            this.RbMode2 = new System.Windows.Forms.RadioButton();
            this.RbMode1 = new System.Windows.Forms.RadioButton();
            this.RbMode0 = new System.Windows.Forms.RadioButton();
            this.BtnReduce = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.BtnSwitchXy = new System.Windows.Forms.Button();
            this.tBflowControl = new System.Windows.Forms.TextBox();
            this.cBToolChange = new System.Windows.Forms.CheckBox();
            this.cBImportGCToolM0 = new System.Windows.Forms.CheckBox();
            this.cBImportGCTool = new System.Windows.Forms.CheckBox();
            this.NudPointDistance = new System.Windows.Forms.NumericUpDown();
            this.CbMovementPenUp = new System.Windows.Forms.CheckBox();
            this.NudSizeX = new System.Windows.Forms.NumericUpDown();
            this.NudSizeY = new System.Windows.Forms.NumericUpDown();
            this.CbFitToCurve = new System.Windows.Forms.CheckBox();
            this.NudSizePen = new System.Windows.Forms.NumericUpDown();
            this.GbMode = new System.Windows.Forms.GroupBox();
            this.BtnFitCurve = new System.Windows.Forms.Button();
            this.GbPlotter = new System.Windows.Forms.GroupBox();
            this.CbMovementMouse = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LblToolTable = new System.Windows.Forms.Label();
            this.CboxToolFiles = new System.Windows.Forms.ComboBox();
            this.GbToolChange = new System.Windows.Forms.GroupBox();
            this.cBflowControl = new System.Windows.Forms.CheckBox();
            this.BtnOpenSetup = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.PSelectedColor = new System.Windows.Forms.Panel();
            this.FlpToolSelection = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TabletPanel = new System.Windows.Forms.Panel();
            this.Tablet = new TabletControl.TabletControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNewClear = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.importXyzDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dLiveUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importWholeDrawingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plotterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolPanel = new System.Windows.Forms.Panel();
            this.GbColor = new System.Windows.Forms.GroupBox();
            this.CbTransparency = new System.Windows.Forms.CheckBox();
            this.GbPen = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SetupPanel = new System.Windows.Forms.Panel();
            this.BtnImport = new System.Windows.Forms.Button();
            this.BtnCloseSetup = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.LblInfo = new System.Windows.Forms.Label();
            this.GbModify = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.LblStrokes = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GbSize = new System.Windows.Forms.GroupBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.TssLblActualPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.TssLblCanvasData = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.NudPointDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSizeX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSizeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSizePen)).BeginInit();
            this.GbMode.SuspendLayout();
            this.GbPlotter.SuspendLayout();
            this.GbToolChange.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.TabletPanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.ToolPanel.SuspendLayout();
            this.GbColor.SuspendLayout();
            this.GbPen.SuspendLayout();
            this.SetupPanel.SuspendLayout();
            this.GbModify.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.GbSize.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // BtnRefresh
            // 
            resources.ApplyResources(this.BtnRefresh, "BtnRefresh");
            this.BtnRefresh.Name = "BtnRefresh";
            this.toolTip1.SetToolTip(this.BtnRefresh, resources.GetString("BtnRefresh.ToolTip"));
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // BtnHelp
            // 
            this.BtnHelp.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.BtnHelp, "BtnHelp");
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Tag = "id=form-tablet";
            this.toolTip1.SetToolTip(this.BtnHelp, resources.GetString("BtnHelp.ToolTip"));
            this.BtnHelp.UseVisualStyleBackColor = false;
            this.BtnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // CboxPapersize
            // 
            this.CboxPapersize.FormattingEnabled = true;
            resources.ApplyResources(this.CboxPapersize, "CboxPapersize");
            this.CboxPapersize.Name = "CboxPapersize";
            this.toolTip1.SetToolTip(this.CboxPapersize, resources.GetString("CboxPapersize.ToolTip"));
            this.CboxPapersize.SelectedIndexChanged += new System.EventHandler(this.CboxPapersize_SelectedIndexChanged);
            // 
            // CbUpdate
            // 
            resources.ApplyResources(this.CbUpdate, "CbUpdate");
            this.CbUpdate.Name = "CbUpdate";
            this.toolTip1.SetToolTip(this.CbUpdate, resources.GetString("CbUpdate.ToolTip"));
            this.CbUpdate.UseVisualStyleBackColor = true;
            // 
            // RbMode3
            // 
            resources.ApplyResources(this.RbMode3, "RbMode3");
            this.RbMode3.Name = "RbMode3";
            this.RbMode3.TabStop = true;
            this.toolTip1.SetToolTip(this.RbMode3, resources.GetString("RbMode3.ToolTip"));
            this.RbMode3.UseVisualStyleBackColor = true;
            this.RbMode3.CheckedChanged += new System.EventHandler(this.RbMode_CheckedChanged);
            // 
            // RbMode2
            // 
            resources.ApplyResources(this.RbMode2, "RbMode2");
            this.RbMode2.Name = "RbMode2";
            this.RbMode2.TabStop = true;
            this.toolTip1.SetToolTip(this.RbMode2, resources.GetString("RbMode2.ToolTip"));
            this.RbMode2.UseVisualStyleBackColor = true;
            this.RbMode2.CheckedChanged += new System.EventHandler(this.RbMode_CheckedChanged);
            // 
            // RbMode1
            // 
            resources.ApplyResources(this.RbMode1, "RbMode1");
            this.RbMode1.Name = "RbMode1";
            this.RbMode1.TabStop = true;
            this.toolTip1.SetToolTip(this.RbMode1, resources.GetString("RbMode1.ToolTip"));
            this.RbMode1.UseVisualStyleBackColor = true;
            this.RbMode1.CheckedChanged += new System.EventHandler(this.RbMode_CheckedChanged);
            // 
            // RbMode0
            // 
            resources.ApplyResources(this.RbMode0, "RbMode0");
            this.RbMode0.Checked = true;
            this.RbMode0.Name = "RbMode0";
            this.RbMode0.TabStop = true;
            this.toolTip1.SetToolTip(this.RbMode0, resources.GetString("RbMode0.ToolTip"));
            this.RbMode0.UseVisualStyleBackColor = true;
            this.RbMode0.CheckedChanged += new System.EventHandler(this.RbMode_CheckedChanged);
            // 
            // BtnReduce
            // 
            resources.ApplyResources(this.BtnReduce, "BtnReduce");
            this.BtnReduce.Name = "BtnReduce";
            this.toolTip1.SetToolTip(this.BtnReduce, resources.GetString("BtnReduce.ToolTip"));
            this.BtnReduce.UseVisualStyleBackColor = true;
            this.BtnReduce.Click += new System.EventHandler(this.BtnReduce_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // BtnSwitchXy
            // 
            resources.ApplyResources(this.BtnSwitchXy, "BtnSwitchXy");
            this.BtnSwitchXy.Name = "BtnSwitchXy";
            this.toolTip1.SetToolTip(this.BtnSwitchXy, resources.GetString("BtnSwitchXy.ToolTip"));
            this.BtnSwitchXy.UseVisualStyleBackColor = true;
            this.BtnSwitchXy.Click += new System.EventHandler(this.BtnSwitchXy_Click);
            // 
            // tBflowControl
            // 
            this.tBflowControl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "flowControlText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBflowControl, "tBflowControl");
            this.tBflowControl.Name = "tBflowControl";
            this.tBflowControl.Text = global::GrblPlotter.Properties.Settings.Default.flowControlText;
            this.toolTip1.SetToolTip(this.tBflowControl, resources.GetString("tBflowControl.ToolTip"));
            // 
            // cBToolChange
            // 
            resources.ApplyResources(this.cBToolChange, "cBToolChange");
            this.cBToolChange.Checked = global::GrblPlotter.Properties.Settings.Default.ctrlToolChange;
            this.cBToolChange.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "ctrlToolChange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolChange.Name = "cBToolChange";
            this.toolTip1.SetToolTip(this.cBToolChange, resources.GetString("cBToolChange.ToolTip"));
            this.cBToolChange.UseVisualStyleBackColor = true;
            // 
            // cBImportGCToolM0
            // 
            resources.ApplyResources(this.cBImportGCToolM0, "cBImportGCToolM0");
            this.cBImportGCToolM0.Checked = global::GrblPlotter.Properties.Settings.Default.importGCToolM0;
            this.cBImportGCToolM0.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGCToolM0", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCToolM0.Name = "cBImportGCToolM0";
            this.toolTip1.SetToolTip(this.cBImportGCToolM0, resources.GetString("cBImportGCToolM0.ToolTip"));
            this.cBImportGCToolM0.UseVisualStyleBackColor = true;
            // 
            // cBImportGCTool
            // 
            resources.ApplyResources(this.cBImportGCTool, "cBImportGCTool");
            this.cBImportGCTool.Checked = global::GrblPlotter.Properties.Settings.Default.importGCTool;
            this.cBImportGCTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGCTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCTool.Name = "cBImportGCTool";
            this.toolTip1.SetToolTip(this.cBImportGCTool, resources.GetString("cBImportGCTool.ToolTip"));
            this.cBImportGCTool.UseVisualStyleBackColor = true;
            // 
            // NudPointDistance
            // 
            this.NudPointDistance.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "tabletPointDistance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudPointDistance.DecimalPlaces = 2;
            this.NudPointDistance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.NudPointDistance, "NudPointDistance");
            this.NudPointDistance.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudPointDistance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudPointDistance.Name = "NudPointDistance";
            this.toolTip1.SetToolTip(this.NudPointDistance, resources.GetString("NudPointDistance.ToolTip"));
            this.NudPointDistance.Value = global::GrblPlotter.Properties.Settings.Default.tabletPointDistance;
            this.NudPointDistance.ValueChanged += new System.EventHandler(this.NudPointDistance_ValueChanged);
            // 
            // CbMovementPenUp
            // 
            resources.ApplyResources(this.CbMovementPenUp, "CbMovementPenUp");
            this.CbMovementPenUp.Checked = global::GrblPlotter.Properties.Settings.Default.tabletPlotterMoveAir;
            this.CbMovementPenUp.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "tabletPlotterMoveAir", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbMovementPenUp.Name = "CbMovementPenUp";
            this.toolTip1.SetToolTip(this.CbMovementPenUp, resources.GetString("CbMovementPenUp.ToolTip"));
            this.CbMovementPenUp.UseVisualStyleBackColor = true;
            // 
            // NudSizeX
            // 
            this.NudSizeX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "tabletSizeX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudSizeX.DecimalPlaces = 1;
            this.NudSizeX.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NudSizeX, "NudSizeX");
            this.NudSizeX.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudSizeX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudSizeX.Name = "NudSizeX";
            this.toolTip1.SetToolTip(this.NudSizeX, resources.GetString("NudSizeX.ToolTip"));
            this.NudSizeX.Value = global::GrblPlotter.Properties.Settings.Default.tabletSizeX;
            this.NudSizeX.ValueChanged += new System.EventHandler(this.NudSizeX_ValueChanged);
            // 
            // NudSizeY
            // 
            this.NudSizeY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "tabletSizeY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudSizeY.DecimalPlaces = 1;
            this.NudSizeY.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.NudSizeY, "NudSizeY");
            this.NudSizeY.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NudSizeY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudSizeY.Name = "NudSizeY";
            this.toolTip1.SetToolTip(this.NudSizeY, resources.GetString("NudSizeY.ToolTip"));
            this.NudSizeY.Value = global::GrblPlotter.Properties.Settings.Default.tabletSizeY;
            this.NudSizeY.ValueChanged += new System.EventHandler(this.NudSizeX_ValueChanged);
            // 
            // CbFitToCurve
            // 
            resources.ApplyResources(this.CbFitToCurve, "CbFitToCurve");
            this.CbFitToCurve.Checked = global::GrblPlotter.Properties.Settings.Default.tabletFitToCurve;
            this.CbFitToCurve.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbFitToCurve.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "tabletFitToCurve", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbFitToCurve.Name = "CbFitToCurve";
            this.toolTip1.SetToolTip(this.CbFitToCurve, resources.GetString("CbFitToCurve.ToolTip"));
            this.CbFitToCurve.UseVisualStyleBackColor = true;
            this.CbFitToCurve.CheckedChanged += new System.EventHandler(this.CbFitToCurve_CheckedChanged);
            // 
            // NudSizePen
            // 
            this.NudSizePen.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "tabletSizePen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudSizePen.DecimalPlaces = 1;
            resources.ApplyResources(this.NudSizePen, "NudSizePen");
            this.NudSizePen.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudSizePen.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudSizePen.Name = "NudSizePen";
            this.toolTip1.SetToolTip(this.NudSizePen, resources.GetString("NudSizePen.ToolTip"));
            this.NudSizePen.Value = global::GrblPlotter.Properties.Settings.Default.tabletSizePen;
            this.NudSizePen.ValueChanged += new System.EventHandler(this.NudSizePen_ValueChanged);
            // 
            // GbMode
            // 
            this.GbMode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbMode.Controls.Add(this.RbMode3);
            this.GbMode.Controls.Add(this.RbMode2);
            this.GbMode.Controls.Add(this.RbMode1);
            this.GbMode.Controls.Add(this.RbMode0);
            this.GbMode.Controls.Add(this.CbFitToCurve);
            resources.ApplyResources(this.GbMode, "GbMode");
            this.GbMode.Name = "GbMode";
            this.GbMode.TabStop = false;
            // 
            // BtnFitCurve
            // 
            resources.ApplyResources(this.BtnFitCurve, "BtnFitCurve");
            this.BtnFitCurve.Name = "BtnFitCurve";
            this.BtnFitCurve.UseVisualStyleBackColor = true;
            this.BtnFitCurve.Click += new System.EventHandler(this.BtnFitCurve_Click);
            // 
            // GbPlotter
            // 
            this.GbPlotter.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbPlotter.Controls.Add(this.CbMovementPenUp);
            this.GbPlotter.Controls.Add(this.CbMovementMouse);
            resources.ApplyResources(this.GbPlotter, "GbPlotter");
            this.GbPlotter.Name = "GbPlotter";
            this.GbPlotter.TabStop = false;
            // 
            // CbMovementMouse
            // 
            resources.ApplyResources(this.CbMovementMouse, "CbMovementMouse");
            this.CbMovementMouse.Checked = global::GrblPlotter.Properties.Settings.Default.tabletPlotterMoveMouse;
            this.CbMovementMouse.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "tabletPlotterMoveMouse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbMovementMouse.Name = "CbMovementMouse";
            this.CbMovementMouse.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // LblToolTable
            // 
            resources.ApplyResources(this.LblToolTable, "LblToolTable");
            this.LblToolTable.Name = "LblToolTable";
            // 
            // CboxToolFiles
            // 
            resources.ApplyResources(this.CboxToolFiles, "CboxToolFiles");
            this.CboxToolFiles.FormattingEnabled = true;
            this.CboxToolFiles.Name = "CboxToolFiles";
            this.CboxToolFiles.SelectedIndexChanged += new System.EventHandler(this.CboxToolFiles_SelectedIndexChanged);
            // 
            // GbToolChange
            // 
            this.GbToolChange.Controls.Add(this.tBflowControl);
            this.GbToolChange.Controls.Add(this.cBflowControl);
            this.GbToolChange.Controls.Add(this.BtnOpenSetup);
            this.GbToolChange.Controls.Add(this.cBToolChange);
            this.GbToolChange.Controls.Add(this.cBImportGCToolM0);
            this.GbToolChange.Controls.Add(this.cBImportGCTool);
            resources.ApplyResources(this.GbToolChange, "GbToolChange");
            this.GbToolChange.Name = "GbToolChange";
            this.GbToolChange.TabStop = false;
            // 
            // cBflowControl
            // 
            resources.ApplyResources(this.cBflowControl, "cBflowControl");
            this.cBflowControl.Checked = global::GrblPlotter.Properties.Settings.Default.flowControlEnable;
            this.cBflowControl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBflowControl.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "flowControlEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBflowControl.Name = "cBflowControl";
            this.cBflowControl.UseVisualStyleBackColor = true;
            // 
            // BtnOpenSetup
            // 
            resources.ApplyResources(this.BtnOpenSetup, "BtnOpenSetup");
            this.BtnOpenSetup.Name = "BtnOpenSetup";
            this.BtnOpenSetup.UseVisualStyleBackColor = true;
            this.BtnOpenSetup.Click += new System.EventHandler(this.BtnOpenSetup_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // PSelectedColor
            // 
            this.PSelectedColor.BackColor = System.Drawing.Color.Black;
            this.PSelectedColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.PSelectedColor, "PSelectedColor");
            this.PSelectedColor.Name = "PSelectedColor";
            // 
            // FlpToolSelection
            // 
            resources.ApplyResources(this.FlpToolSelection, "FlpToolSelection");
            this.FlpToolSelection.Name = "FlpToolSelection";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.TabletPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ToolPanel, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // TabletPanel
            // 
            this.TabletPanel.Controls.Add(this.Tablet);
            resources.ApplyResources(this.TabletPanel, "TabletPanel");
            this.TabletPanel.Name = "TabletPanel";
            // 
            // Tablet
            // 
            resources.ApplyResources(this.Tablet, "Tablet");
            this.Tablet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tablet.FitToCurve = false;
            this.Tablet.Name = "Tablet";
            this.Tablet.OnPenUpReduce = true;
            this.Tablet.PenSize = 2D;
            this.Tablet.PointDistance = 0.2D;
            this.Tablet.ZoomOnResize = true;
            this.Tablet.TabletEvent += new TabletControl.TabletEventHandler(this.Tablet_TabletEvent);
            this.Tablet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GCodeFromTablet_KeyDown);
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.plotterToolStripMenuItem,
            this.setupToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNewClear,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Click += new System.EventHandler(this.MenuHead_Click);
            // 
            // MenuNewClear
            // 
            this.MenuNewClear.Name = "MenuNewClear";
            resources.ApplyResources(this.MenuNewClear, "MenuNewClear");
            this.MenuNewClear.Click += new System.EventHandler(this.MenuNewClear_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.MenuOpen_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.MenuSave_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importXyzDataToolStripMenuItem,
            this.dLiveUpdateToolStripMenuItem,
            this.importWholeDrawingToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Click += new System.EventHandler(this.MenuHead_Click);
            // 
            // importXyzDataToolStripMenuItem
            // 
            this.importXyzDataToolStripMenuItem.Name = "importXyzDataToolStripMenuItem";
            resources.ApplyResources(this.importXyzDataToolStripMenuItem, "importXyzDataToolStripMenuItem");
            this.importXyzDataToolStripMenuItem.Click += new System.EventHandler(this.MenuImportXyzData);
            // 
            // dLiveUpdateToolStripMenuItem
            // 
            this.dLiveUpdateToolStripMenuItem.CheckOnClick = true;
            this.dLiveUpdateToolStripMenuItem.Name = "dLiveUpdateToolStripMenuItem";
            resources.ApplyResources(this.dLiveUpdateToolStripMenuItem, "dLiveUpdateToolStripMenuItem");
            // 
            // importWholeDrawingToolStripMenuItem
            // 
            this.importWholeDrawingToolStripMenuItem.Name = "importWholeDrawingToolStripMenuItem";
            resources.ApplyResources(this.importWholeDrawingToolStripMenuItem, "importWholeDrawingToolStripMenuItem");
            this.importWholeDrawingToolStripMenuItem.Click += new System.EventHandler(this.MenuImportWholeDrawing_Click);
            // 
            // plotterToolStripMenuItem
            // 
            this.plotterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.liveUpdateToolStripMenuItem});
            this.plotterToolStripMenuItem.Name = "plotterToolStripMenuItem";
            resources.ApplyResources(this.plotterToolStripMenuItem, "plotterToolStripMenuItem");
            this.plotterToolStripMenuItem.Click += new System.EventHandler(this.MenuHead_Click);
            // 
            // liveUpdateToolStripMenuItem
            // 
            this.liveUpdateToolStripMenuItem.CheckOnClick = true;
            this.liveUpdateToolStripMenuItem.Name = "liveUpdateToolStripMenuItem";
            resources.ApplyResources(this.liveUpdateToolStripMenuItem, "liveUpdateToolStripMenuItem");
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            resources.ApplyResources(this.setupToolStripMenuItem, "setupToolStripMenuItem");
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.MenuSetup_Click);
            // 
            // ToolPanel
            // 
            this.ToolPanel.Controls.Add(this.GbColor);
            this.ToolPanel.Controls.Add(this.GbMode);
            this.ToolPanel.Controls.Add(this.GbPen);
            resources.ApplyResources(this.ToolPanel, "ToolPanel");
            this.ToolPanel.Name = "ToolPanel";
            // 
            // GbColor
            // 
            this.GbColor.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbColor.Controls.Add(this.CbTransparency);
            this.GbColor.Controls.Add(this.CboxToolFiles);
            this.GbColor.Controls.Add(this.LblToolTable);
            this.GbColor.Controls.Add(this.PSelectedColor);
            this.GbColor.Controls.Add(this.FlpToolSelection);
            this.GbColor.Controls.Add(this.label5);
            resources.ApplyResources(this.GbColor, "GbColor");
            this.GbColor.Name = "GbColor";
            this.GbColor.TabStop = false;
            // 
            // CbTransparency
            // 
            resources.ApplyResources(this.CbTransparency, "CbTransparency");
            this.CbTransparency.Checked = global::GrblPlotter.Properties.Settings.Default.tabletShowTransparency;
            this.CbTransparency.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbTransparency.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "tabletShowTransparency", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbTransparency.Name = "CbTransparency";
            this.CbTransparency.UseVisualStyleBackColor = true;
            this.CbTransparency.CheckedChanged += new System.EventHandler(this.CbTransparency_CheckedChanged);
            // 
            // GbPen
            // 
            this.GbPen.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbPen.Controls.Add(this.label1);
            this.GbPen.Controls.Add(this.NudSizePen);
            this.GbPen.Controls.Add(this.label4);
            resources.ApplyResources(this.GbPen, "GbPen");
            this.GbPen.Name = "GbPen";
            this.GbPen.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // SetupPanel
            // 
            this.SetupPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SetupPanel.Controls.Add(this.BtnImport);
            this.SetupPanel.Controls.Add(this.BtnCloseSetup);
            this.SetupPanel.Controls.Add(this.label6);
            this.SetupPanel.Controls.Add(this.LblInfo);
            this.SetupPanel.Controls.Add(this.GbToolChange);
            this.SetupPanel.Controls.Add(this.GbModify);
            this.SetupPanel.Controls.Add(this.groupBox1);
            this.SetupPanel.Controls.Add(this.GbPlotter);
            this.SetupPanel.Controls.Add(this.GbSize);
            resources.ApplyResources(this.SetupPanel, "SetupPanel");
            this.SetupPanel.Name = "SetupPanel";
            // 
            // BtnImport
            // 
            resources.ApplyResources(this.BtnImport, "BtnImport");
            this.BtnImport.Name = "BtnImport";
            this.BtnImport.UseVisualStyleBackColor = true;
            this.BtnImport.Click += new System.EventHandler(this.BtnImport_Click);
            // 
            // BtnCloseSetup
            // 
            resources.ApplyResources(this.BtnCloseSetup, "BtnCloseSetup");
            this.BtnCloseSetup.Name = "BtnCloseSetup";
            this.BtnCloseSetup.UseVisualStyleBackColor = true;
            this.BtnCloseSetup.Click += new System.EventHandler(this.BtnCloseSetup_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // LblInfo
            // 
            resources.ApplyResources(this.LblInfo, "LblInfo");
            this.LblInfo.Name = "LblInfo";
            // 
            // GbModify
            // 
            this.GbModify.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbModify.Controls.Add(this.label7);
            this.GbModify.Controls.Add(this.LblStrokes);
            this.GbModify.Controls.Add(this.BtnFitCurve);
            this.GbModify.Controls.Add(this.BtnReduce);
            this.GbModify.Controls.Add(this.NudPointDistance);
            resources.ApplyResources(this.GbModify, "GbModify");
            this.GbModify.Name = "GbModify";
            this.GbModify.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // LblStrokes
            // 
            resources.ApplyResources(this.LblStrokes, "LblStrokes");
            this.LblStrokes.Name = "LblStrokes";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.groupBox1.Controls.Add(this.CbUpdate);
            this.groupBox1.Controls.Add(this.BtnRefresh);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // GbSize
            // 
            this.GbSize.BackColor = System.Drawing.Color.WhiteSmoke;
            this.GbSize.Controls.Add(this.BtnSwitchXy);
            this.GbSize.Controls.Add(this.CboxPapersize);
            this.GbSize.Controls.Add(this.NudSizeX);
            this.GbSize.Controls.Add(this.label3);
            this.GbSize.Controls.Add(this.NudSizeY);
            this.GbSize.Controls.Add(this.label2);
            resources.ApplyResources(this.GbSize, "GbSize");
            this.GbSize.Name = "GbSize";
            this.GbSize.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TssLblActualPos,
            this.TssLblCanvasData});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // TssLblActualPos
            // 
            resources.ApplyResources(this.TssLblActualPos, "TssLblActualPos");
            this.TssLblActualPos.Name = "TssLblActualPos";
            // 
            // TssLblCanvasData
            // 
            this.TssLblCanvasData.Name = "TssLblCanvasData";
            resources.ApplyResources(this.TssLblCanvasData, "TssLblCanvasData");
            // 
            // GCodeFromTablet
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SetupPanel);
            this.Controls.Add(this.BtnHelp);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GCodeFromTablet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GCodeFromTablet_FormClosing);
            this.Load += new System.EventHandler(this.GCodeFromTablet_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GCodeFromTablet_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GCodeFromTablet_KeyDown);
            this.Resize += new System.EventHandler(this.GCodeFromTablet_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.NudPointDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSizeX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSizeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudSizePen)).EndInit();
            this.GbMode.ResumeLayout(false);
            this.GbMode.PerformLayout();
            this.GbPlotter.ResumeLayout(false);
            this.GbPlotter.PerformLayout();
            this.GbToolChange.ResumeLayout(false);
            this.GbToolChange.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.TabletPanel.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ToolPanel.ResumeLayout(false);
            this.GbColor.ResumeLayout(false);
            this.GbColor.PerformLayout();
            this.GbPen.ResumeLayout(false);
            this.GbPen.PerformLayout();
            this.SetupPanel.ResumeLayout(false);
            this.SetupPanel.PerformLayout();
            this.GbModify.ResumeLayout(false);
            this.GbModify.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.GbSize.ResumeLayout(false);
            this.GbSize.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox GbPlotter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NudSizePen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NudSizeY;
        private System.Windows.Forms.CheckBox CbMovementPenUp;
        private System.Windows.Forms.NumericUpDown NudSizeX;
        private System.Windows.Forms.CheckBox CbMovementMouse;
        private TabletControl.TabletControl Tablet;
        private System.Windows.Forms.ComboBox CboxToolFiles;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel PSelectedColor;
        private System.Windows.Forms.FlowLayoutPanel FlpToolSelection;
        private System.Windows.Forms.CheckBox CbUpdate;
        private System.Windows.Forms.Button BtnRefresh;
        private System.Windows.Forms.GroupBox GbToolChange;
        private System.Windows.Forms.CheckBox cBToolChange;
        private System.Windows.Forms.CheckBox cBImportGCToolM0;
        private System.Windows.Forms.CheckBox cBImportGCTool;
        private System.Windows.Forms.Label LblToolTable;
        private System.Windows.Forms.Button BtnOpenSetup;
        private System.Windows.Forms.TextBox tBflowControl;
        private System.Windows.Forms.CheckBox cBflowControl;
        private System.Windows.Forms.CheckBox CbTransparency;
        private System.Windows.Forms.GroupBox GbMode;
        private System.Windows.Forms.RadioButton RbMode3;
        private System.Windows.Forms.RadioButton RbMode2;
        private System.Windows.Forms.RadioButton RbMode1;
        private System.Windows.Forms.RadioButton RbMode0;
        private System.Windows.Forms.CheckBox CbFitToCurve;
        private System.Windows.Forms.Button BtnHelp;
        private System.Windows.Forms.ComboBox CboxPapersize;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel TssLblActualPos;
        private System.Windows.Forms.Panel TabletPanel;
        private System.Windows.Forms.GroupBox GbColor;
        private System.Windows.Forms.GroupBox GbSize;
        private System.Windows.Forms.GroupBox GbPen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnSwitchXy;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.ToolStripStatusLabel TssLblCanvasData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BtnReduce;
        private System.Windows.Forms.Button BtnFitCurve;
        private System.Windows.Forms.NumericUpDown NudPointDistance;
        private System.Windows.Forms.GroupBox GbModify;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Panel SetupPanel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem MenuNewClear;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plotterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.Panel ToolPanel;
        private System.Windows.Forms.ToolStripMenuItem dLiveUpdateToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem importWholeDrawingToolStripMenuItem;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button BtnCloseSetup;
        private System.Windows.Forms.ToolStripMenuItem liveUpdateToolStripMenuItem;
        private System.Windows.Forms.Label LblStrokes;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button BtnImport;
        private System.Windows.Forms.ToolStripMenuItem importXyzDataToolStripMenuItem;
    }
}