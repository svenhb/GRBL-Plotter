
namespace GrblPlotter
{
    partial class ControlProjector
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.btnColorMarker = new System.Windows.Forms.Button();
            this.btnColorTool = new System.Windows.Forms.Button();
            this.btnColorPenDown = new System.Windows.Forms.Button();
            this.btnColorPenUp = new System.Windows.Forms.Button();
            this.btnColorRuler = new System.Windows.Forms.Button();
            this.btnColorDimension = new System.Windows.Forms.Button();
            this.btnColorBackground = new System.Windows.Forms.Button();
            this.SetupPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.NudOffsetY = new System.Windows.Forms.NumericUpDown();
            this.NudOffsetX = new System.Windows.Forms.NumericUpDown();
            this.NudScaling = new System.Windows.Forms.NumericUpDown();
            this.CbMarker = new System.Windows.Forms.CheckBox();
            this.CbTool = new System.Windows.Forms.CheckBox();
            this.CbPenUp = new System.Windows.Forms.CheckBox();
            this.CbRuler = new System.Windows.Forms.CheckBox();
            this.NudDImension = new System.Windows.Forms.NumericUpDown();
            this.CbDimension = new System.Windows.Forms.CheckBox();
            this.NudRuler = new System.Windows.Forms.NumericUpDown();
            this.NudPenUp = new System.Windows.Forms.NumericUpDown();
            this.NudMarker = new System.Windows.Forms.NumericUpDown();
            this.NudPenDown = new System.Windows.Forms.NumericUpDown();
            this.NudTool = new System.Windows.Forms.NumericUpDown();
            this.SetupPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudOffsetX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudScaling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDImension)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudRuler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudPenUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMarker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudPenDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudTool)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(75, 317);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Offset Y";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(75, 291);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Offset X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(75, 265);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Scaling (also depends on machine limits)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Color";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(133, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Pen width";
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(7, 352);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(328, 23);
            this.BtnClose.TabIndex = 7;
            this.BtnClose.Text = "Close Setup (reopen with right-click)";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnColorMarker
            // 
            this.btnColorMarker.Location = new System.Drawing.Point(7, 223);
            this.btnColorMarker.Name = "btnColorMarker";
            this.btnColorMarker.Size = new System.Drawing.Size(120, 23);
            this.btnColorMarker.TabIndex = 6;
            this.btnColorMarker.Text = "Marker";
            this.btnColorMarker.UseVisualStyleBackColor = true;
            this.btnColorMarker.Click += new System.EventHandler(this.BtnColorMarker_Click);
            // 
            // btnColorTool
            // 
            this.btnColorTool.Location = new System.Drawing.Point(7, 194);
            this.btnColorTool.Name = "btnColorTool";
            this.btnColorTool.Size = new System.Drawing.Size(120, 23);
            this.btnColorTool.TabIndex = 5;
            this.btnColorTool.Text = "Tool";
            this.btnColorTool.UseVisualStyleBackColor = true;
            this.btnColorTool.Click += new System.EventHandler(this.BtnColorTool_Click);
            // 
            // btnColorPenDown
            // 
            this.btnColorPenDown.Location = new System.Drawing.Point(7, 165);
            this.btnColorPenDown.Name = "btnColorPenDown";
            this.btnColorPenDown.Size = new System.Drawing.Size(120, 23);
            this.btnColorPenDown.TabIndex = 4;
            this.btnColorPenDown.Text = "Pen-Down";
            this.btnColorPenDown.UseVisualStyleBackColor = true;
            this.btnColorPenDown.Click += new System.EventHandler(this.BtnColorPenDown_Click);
            // 
            // btnColorPenUp
            // 
            this.btnColorPenUp.Location = new System.Drawing.Point(7, 136);
            this.btnColorPenUp.Name = "btnColorPenUp";
            this.btnColorPenUp.Size = new System.Drawing.Size(120, 23);
            this.btnColorPenUp.TabIndex = 3;
            this.btnColorPenUp.Text = "Pen-Up";
            this.btnColorPenUp.UseVisualStyleBackColor = true;
            this.btnColorPenUp.Click += new System.EventHandler(this.BtnColorPenUp_Click);
            // 
            // btnColorRuler
            // 
            this.btnColorRuler.Location = new System.Drawing.Point(7, 107);
            this.btnColorRuler.Name = "btnColorRuler";
            this.btnColorRuler.Size = new System.Drawing.Size(120, 23);
            this.btnColorRuler.TabIndex = 2;
            this.btnColorRuler.Text = "Ruler";
            this.btnColorRuler.UseVisualStyleBackColor = true;
            this.btnColorRuler.Click += new System.EventHandler(this.BtnColorRuler_Click);
            // 
            // btnColorDimension
            // 
            this.btnColorDimension.Location = new System.Drawing.Point(7, 78);
            this.btnColorDimension.Name = "btnColorDimension";
            this.btnColorDimension.Size = new System.Drawing.Size(120, 23);
            this.btnColorDimension.TabIndex = 1;
            this.btnColorDimension.Text = "Dimension";
            this.btnColorDimension.UseVisualStyleBackColor = true;
            this.btnColorDimension.Click += new System.EventHandler(this.BtnColorDimension_Click);
            // 
            // btnColorBackground
            // 
            this.btnColorBackground.Location = new System.Drawing.Point(7, 49);
            this.btnColorBackground.Name = "btnColorBackground";
            this.btnColorBackground.Size = new System.Drawing.Size(120, 23);
            this.btnColorBackground.TabIndex = 0;
            this.btnColorBackground.Text = "Background";
            this.btnColorBackground.UseVisualStyleBackColor = true;
            this.btnColorBackground.Click += new System.EventHandler(this.BtnColorBackground_Click);
            // 
            // SetupPanel
            // 
            this.SetupPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SetupPanel.Controls.Add(this.checkBox1);
            this.SetupPanel.Controls.Add(this.BtnClose);
            this.SetupPanel.Controls.Add(this.label5);
            this.SetupPanel.Controls.Add(this.label6);
            this.SetupPanel.Controls.Add(this.label4);
            this.SetupPanel.Controls.Add(this.btnColorBackground);
            this.SetupPanel.Controls.Add(this.NudOffsetY);
            this.SetupPanel.Controls.Add(this.btnColorDimension);
            this.SetupPanel.Controls.Add(this.NudOffsetX);
            this.SetupPanel.Controls.Add(this.btnColorRuler);
            this.SetupPanel.Controls.Add(this.label3);
            this.SetupPanel.Controls.Add(this.btnColorPenUp);
            this.SetupPanel.Controls.Add(this.NudScaling);
            this.SetupPanel.Controls.Add(this.btnColorPenDown);
            this.SetupPanel.Controls.Add(this.CbMarker);
            this.SetupPanel.Controls.Add(this.btnColorTool);
            this.SetupPanel.Controls.Add(this.CbTool);
            this.SetupPanel.Controls.Add(this.btnColorMarker);
            this.SetupPanel.Controls.Add(this.CbPenUp);
            this.SetupPanel.Controls.Add(this.label1);
            this.SetupPanel.Controls.Add(this.CbRuler);
            this.SetupPanel.Controls.Add(this.NudDImension);
            this.SetupPanel.Controls.Add(this.CbDimension);
            this.SetupPanel.Controls.Add(this.NudRuler);
            this.SetupPanel.Controls.Add(this.label2);
            this.SetupPanel.Controls.Add(this.NudPenUp);
            this.SetupPanel.Controls.Add(this.NudMarker);
            this.SetupPanel.Controls.Add(this.NudPenDown);
            this.SetupPanel.Controls.Add(this.NudTool);
            this.SetupPanel.Location = new System.Drawing.Point(195, 12);
            this.SetupPanel.Name = "SetupPanel";
            this.SetupPanel.Size = new System.Drawing.Size(340, 380);
            this.SetupPanel.TabIndex = 1;
            this.SetupPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SetupPanel_MouseDown);
            this.SetupPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SetupPanel_MouseMove);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Setup";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::GrblPlotter.Properties.Settings.Default.projectorShowSetup;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "projectorShowSetup", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(163, 329);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(143, 17);
            this.checkBox1.TabIndex = 28;
            this.checkBox1.Text = "Show setup on form load";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // NudOffsetY
            // 
            this.NudOffsetY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorDisplayOffsetY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudOffsetY.DecimalPlaces = 2;
            this.NudOffsetY.Location = new System.Drawing.Point(7, 315);
            this.NudOffsetY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudOffsetY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.NudOffsetY.Name = "NudOffsetY";
            this.NudOffsetY.Size = new System.Drawing.Size(62, 20);
            this.NudOffsetY.TabIndex = 25;
            this.NudOffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudOffsetY.Value = global::GrblPlotter.Properties.Settings.Default.projectorDisplayOffsetY;
            this.NudOffsetY.ValueChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // NudOffsetX
            // 
            this.NudOffsetX.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorDisplayOffsetX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudOffsetX.DecimalPlaces = 2;
            this.NudOffsetX.Location = new System.Drawing.Point(7, 289);
            this.NudOffsetX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudOffsetX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.NudOffsetX.Name = "NudOffsetX";
            this.NudOffsetX.Size = new System.Drawing.Size(62, 20);
            this.NudOffsetX.TabIndex = 24;
            this.NudOffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudOffsetX.Value = global::GrblPlotter.Properties.Settings.Default.projectorDisplayOffsetX;
            this.NudOffsetX.ValueChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // NudScaling
            // 
            this.NudScaling.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorDisplayScale", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudScaling.DecimalPlaces = 2;
            this.NudScaling.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudScaling.Location = new System.Drawing.Point(7, 263);
            this.NudScaling.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudScaling.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudScaling.Name = "NudScaling";
            this.NudScaling.Size = new System.Drawing.Size(62, 20);
            this.NudScaling.TabIndex = 22;
            this.NudScaling.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudScaling.Value = global::GrblPlotter.Properties.Settings.Default.projectorDisplayScale;
            this.NudScaling.ValueChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // CbMarker
            // 
            this.CbMarker.AutoSize = true;
            this.CbMarker.Checked = global::GrblPlotter.Properties.Settings.Default.projectorShowMarker;
            this.CbMarker.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "projectorShowMarker", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbMarker.Location = new System.Drawing.Point(190, 227);
            this.CbMarker.Name = "CbMarker";
            this.CbMarker.Size = new System.Drawing.Size(53, 17);
            this.CbMarker.TabIndex = 21;
            this.CbMarker.Text = "Show";
            this.CbMarker.UseVisualStyleBackColor = true;
            this.CbMarker.CheckedChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // CbTool
            // 
            this.CbTool.AutoSize = true;
            this.CbTool.Checked = global::GrblPlotter.Properties.Settings.Default.projectorShowTool;
            this.CbTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "projectorShowTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbTool.Location = new System.Drawing.Point(190, 198);
            this.CbTool.Name = "CbTool";
            this.CbTool.Size = new System.Drawing.Size(53, 17);
            this.CbTool.TabIndex = 20;
            this.CbTool.Text = "Show";
            this.CbTool.UseVisualStyleBackColor = true;
            this.CbTool.CheckedChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // CbPenUp
            // 
            this.CbPenUp.AutoSize = true;
            this.CbPenUp.Checked = global::GrblPlotter.Properties.Settings.Default.projectorShowPenUp;
            this.CbPenUp.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "projectorShowPenUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbPenUp.Location = new System.Drawing.Point(190, 140);
            this.CbPenUp.Name = "CbPenUp";
            this.CbPenUp.Size = new System.Drawing.Size(53, 17);
            this.CbPenUp.TabIndex = 19;
            this.CbPenUp.Text = "Show";
            this.CbPenUp.UseVisualStyleBackColor = true;
            this.CbPenUp.CheckedChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // CbRuler
            // 
            this.CbRuler.AutoSize = true;
            this.CbRuler.Checked = global::GrblPlotter.Properties.Settings.Default.projectorShowRuler;
            this.CbRuler.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbRuler.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "projectorShowRuler", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbRuler.Location = new System.Drawing.Point(190, 111);
            this.CbRuler.Name = "CbRuler";
            this.CbRuler.Size = new System.Drawing.Size(53, 17);
            this.CbRuler.TabIndex = 18;
            this.CbRuler.Text = "Show";
            this.CbRuler.UseVisualStyleBackColor = true;
            this.CbRuler.CheckedChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // NudDImension
            // 
            this.NudDImension.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorWidthDimension", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudDImension.DecimalPlaces = 2;
            this.NudDImension.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudDImension.Location = new System.Drawing.Point(133, 81);
            this.NudDImension.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudDImension.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudDImension.Name = "NudDImension";
            this.NudDImension.Size = new System.Drawing.Size(51, 20);
            this.NudDImension.TabIndex = 10;
            this.NudDImension.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudDImension.Value = global::GrblPlotter.Properties.Settings.Default.projectorWidthDimension;
            // 
            // CbDimension
            // 
            this.CbDimension.AutoSize = true;
            this.CbDimension.Checked = global::GrblPlotter.Properties.Settings.Default.projectorShowDimension;
            this.CbDimension.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "projectorShowDimension", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbDimension.Location = new System.Drawing.Point(190, 82);
            this.CbDimension.Name = "CbDimension";
            this.CbDimension.Size = new System.Drawing.Size(53, 17);
            this.CbDimension.TabIndex = 17;
            this.CbDimension.Text = "Show";
            this.CbDimension.UseVisualStyleBackColor = true;
            this.CbDimension.CheckedChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            // 
            // NudRuler
            // 
            this.NudRuler.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorWidthRuler", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudRuler.DecimalPlaces = 2;
            this.NudRuler.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudRuler.Location = new System.Drawing.Point(133, 110);
            this.NudRuler.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudRuler.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudRuler.Name = "NudRuler";
            this.NudRuler.Size = new System.Drawing.Size(51, 20);
            this.NudRuler.TabIndex = 11;
            this.NudRuler.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudRuler.Value = global::GrblPlotter.Properties.Settings.Default.projectorWidthRuler;
            // 
            // NudPenUp
            // 
            this.NudPenUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorWidthPenUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudPenUp.DecimalPlaces = 2;
            this.NudPenUp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudPenUp.Location = new System.Drawing.Point(133, 139);
            this.NudPenUp.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudPenUp.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudPenUp.Name = "NudPenUp";
            this.NudPenUp.Size = new System.Drawing.Size(51, 20);
            this.NudPenUp.TabIndex = 12;
            this.NudPenUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudPenUp.Value = global::GrblPlotter.Properties.Settings.Default.projectorWidthPenUp;
            // 
            // NudMarker
            // 
            this.NudMarker.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorWidthMarker", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudMarker.DecimalPlaces = 2;
            this.NudMarker.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudMarker.Location = new System.Drawing.Point(133, 226);
            this.NudMarker.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudMarker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudMarker.Name = "NudMarker";
            this.NudMarker.Size = new System.Drawing.Size(51, 20);
            this.NudMarker.TabIndex = 15;
            this.NudMarker.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudMarker.Value = global::GrblPlotter.Properties.Settings.Default.projectorWidthMarker;
            // 
            // NudPenDown
            // 
            this.NudPenDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorWidthPenDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudPenDown.DecimalPlaces = 2;
            this.NudPenDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudPenDown.Location = new System.Drawing.Point(133, 168);
            this.NudPenDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudPenDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudPenDown.Name = "NudPenDown";
            this.NudPenDown.Size = new System.Drawing.Size(51, 20);
            this.NudPenDown.TabIndex = 13;
            this.NudPenDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudPenDown.Value = global::GrblPlotter.Properties.Settings.Default.projectorWidthPenDown;
            // 
            // NudTool
            // 
            this.NudTool.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "projectorWidthTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudTool.DecimalPlaces = 2;
            this.NudTool.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudTool.Location = new System.Drawing.Point(133, 197);
            this.NudTool.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NudTool.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NudTool.Name = "NudTool";
            this.NudTool.Size = new System.Drawing.Size(51, 20);
            this.NudTool.TabIndex = 14;
            this.NudTool.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NudTool.Value = global::GrblPlotter.Properties.Settings.Default.projectorWidthTool;
            // 
            // ControlProjector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.SetupPanel);
            this.Name = "ControlProjector";
            this.Text = "Projector";
            this.Load += new System.EventHandler(this.ControlProjector_Load);
            this.ResizeEnd += new System.EventHandler(this.CbShow_CheckedChanged);
            this.SizeChanged += new System.EventHandler(this.CbShow_CheckedChanged);
            this.Click += new System.EventHandler(this.CbShow_CheckedChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ControlProjector_Paint);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ControlProjector_MouseUp);
            this.SetupPanel.ResumeLayout(false);
            this.SetupPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudOffsetX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudScaling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudDImension)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudRuler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudPenUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudMarker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudPenDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudTool)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnColorRuler;
        private System.Windows.Forms.Button btnColorDimension;
        private System.Windows.Forms.Button btnColorBackground;
        private System.Windows.Forms.Button btnColorMarker;
        private System.Windows.Forms.Button btnColorTool;
        private System.Windows.Forms.Button btnColorPenDown;
        private System.Windows.Forms.Button btnColorPenUp;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NudMarker;
        private System.Windows.Forms.NumericUpDown NudTool;
        private System.Windows.Forms.NumericUpDown NudPenDown;
        private System.Windows.Forms.NumericUpDown NudPenUp;
        private System.Windows.Forms.NumericUpDown NudRuler;
        private System.Windows.Forms.NumericUpDown NudDImension;
        private System.Windows.Forms.CheckBox CbMarker;
        private System.Windows.Forms.CheckBox CbTool;
        private System.Windows.Forms.CheckBox CbPenUp;
        private System.Windows.Forms.CheckBox CbRuler;
        private System.Windows.Forms.CheckBox CbDimension;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown NudScaling;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NudOffsetY;
        private System.Windows.Forms.NumericUpDown NudOffsetX;
        private System.Windows.Forms.Panel SetupPanel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}