/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2016 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace GRBL_Plotter
{
    partial class ControlSetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlSetupForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnResizeForm = new System.Windows.Forms.Button();
            this.btnReloadFile = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tBImportSVGPalette = new System.Windows.Forms.TextBox();
            this.cBImportSVGPath = new System.Windows.Forms.CheckBox();
            this.cBImportSVGPauseP = new System.Windows.Forms.CheckBox();
            this.cBImportSVGTool = new System.Windows.Forms.CheckBox();
            this.cBImportSVGPauseE = new System.Windows.Forms.CheckBox();
            this.nUDSVGScale = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.nUDImportReduce = new System.Windows.Forms.NumericUpDown();
            this.cBImportSVGReduce = new System.Windows.Forms.CheckBox();
            this.cBImportSVGComments = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cBImportSVGResize = new System.Windows.Forms.CheckBox();
            this.nUDImportSVGSegemnts = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nUDImportDecPlaces = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.cBImportGCTool = new System.Windows.Forms.CheckBox();
            this.cBImportGCComments = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tBImportGCFooter = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tBImportGCHeader = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rBImportGCSpindleCmd2 = new System.Windows.Forms.RadioButton();
            this.rBImportGCSpindleCmd1 = new System.Windows.Forms.RadioButton();
            this.cBImportGCUseSpindle = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nUDImportGCSSpeed = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cBImportGCUsePWM = new System.Windows.Forms.CheckBox();
            this.nUDImportGCDlyDown = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCDlyUp = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCPWMDown = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCPWMUp = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.nUDImportGCFeedXY = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cBImportGCUseZ = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDImportGCZUp = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCFeedZ = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nUDImportGCZDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.rBCtrlReplaceM4 = new System.Windows.Forms.RadioButton();
            this.rBCtrlReplaceM3 = new System.Windows.Forms.RadioButton();
            this.cBCtrlMCmd = new System.Windows.Forms.CheckBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.cBSerial2 = new System.Windows.Forms.CheckBox();
            this.cBSerialMinimize = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tBToolChangeScriptProbe = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tBToolChangeScriptSelect = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tBToolChangeScriptPut = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tBToolChangeScriptGet = new System.Windows.Forms.TextBox();
            this.cBToolChange = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblcbnr = new System.Windows.Forms.Label();
            this.btnChangeDefinition = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lvCustomButtons = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nUDMarker = new System.Windows.Forms.NumericUpDown();
            this.btnColorMarker = new System.Windows.Forms.Button();
            this.nUDTool = new System.Windows.Forms.NumericUpDown();
            this.nUDPenDown = new System.Windows.Forms.NumericUpDown();
            this.nUDPenUp = new System.Windows.Forms.NumericUpDown();
            this.nUDRuler = new System.Windows.Forms.NumericUpDown();
            this.btnColorTool = new System.Windows.Forms.Button();
            this.btnColorPenDown = new System.Windows.Forms.Button();
            this.btnColorPenUp = new System.Windows.Forms.Button();
            this.btnColorRuler = new System.Windows.Forms.Button();
            this.btnColorBackground = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnJoyZCalc = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.nUDJoyZSpeed5 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZSpeed4 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZSpeed3 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZSpeed2 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZSpeed1 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZStep5 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZStep4 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZStep3 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZStep2 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyZStep1 = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.btnJoyXYCalc = new System.Windows.Forms.Button();
            this.nUDJoyXYSpeed5 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYSpeed4 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYSpeed3 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYSpeed2 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYSpeed1 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYStep5 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYStep4 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYStep3 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYStep2 = new System.Windows.Forms.NumericUpDown();
            this.nUDJoyXYStep1 = new System.Windows.Forms.NumericUpDown();
            this.btnApplyChangings = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSVGScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportReduce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportSVGSegemnts)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportDecPlaces)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSSpeed)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedXY)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDMarker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDTool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRuler)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.expandForm_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnResizeForm);
            this.tabPage3.Controls.Add(this.btnReloadFile);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnResizeForm
            // 
            this.btnResizeForm.BackColor = System.Drawing.Color.Yellow;
            resources.ApplyResources(this.btnResizeForm, "btnResizeForm");
            this.btnResizeForm.Name = "btnResizeForm";
            this.btnResizeForm.UseVisualStyleBackColor = false;
            this.btnResizeForm.Click += new System.EventHandler(this.btnResizeForm_Click);
            // 
            // btnReloadFile
            // 
            resources.ApplyResources(this.btnReloadFile, "btnReloadFile");
            this.btnReloadFile.Name = "btnReloadFile";
            this.btnReloadFile.UseVisualStyleBackColor = true;
            this.btnReloadFile.Click += new System.EventHandler(this.btnReloadFile_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Controls.Add(this.tBImportSVGPalette);
            this.groupBox3.Controls.Add(this.cBImportSVGPath);
            this.groupBox3.Controls.Add(this.cBImportSVGPauseP);
            this.groupBox3.Controls.Add(this.cBImportSVGTool);
            this.groupBox3.Controls.Add(this.cBImportSVGPauseE);
            this.groupBox3.Controls.Add(this.nUDSVGScale);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.nUDImportReduce);
            this.groupBox3.Controls.Add(this.cBImportSVGReduce);
            this.groupBox3.Controls.Add(this.cBImportSVGComments);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.cBImportSVGResize);
            this.groupBox3.Controls.Add(this.nUDImportSVGSegemnts);
            this.groupBox3.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGToolSort;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGToolSort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Name = "checkBox1";
            this.toolTip1.SetToolTip(this.checkBox1, resources.GetString("checkBox1.ToolTip"));
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tBImportSVGPalette
            // 
            this.tBImportSVGPalette.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importPalette", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBImportSVGPalette, "tBImportSVGPalette");
            this.tBImportSVGPalette.Name = "tBImportSVGPalette";
            this.tBImportSVGPalette.Text = global::GRBL_Plotter.Properties.Settings.Default.importPalette;
            this.toolTip1.SetToolTip(this.tBImportSVGPalette, resources.GetString("tBImportSVGPalette.ToolTip"));
            // 
            // cBImportSVGPath
            // 
            resources.ApplyResources(this.cBImportSVGPath, "cBImportSVGPath");
            this.cBImportSVGPath.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPathExtend;
            this.cBImportSVGPath.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPathExtend", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPath.Name = "cBImportSVGPath";
            this.toolTip1.SetToolTip(this.cBImportSVGPath, resources.GetString("cBImportSVGPath.ToolTip"));
            this.cBImportSVGPath.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGPauseP
            // 
            resources.ApplyResources(this.cBImportSVGPauseP, "cBImportSVGPauseP");
            this.cBImportSVGPauseP.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPausePenDown;
            this.cBImportSVGPauseP.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPausePenDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPauseP.Name = "cBImportSVGPauseP";
            this.cBImportSVGPauseP.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGTool
            // 
            resources.ApplyResources(this.cBImportSVGTool, "cBImportSVGTool");
            this.cBImportSVGTool.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGToolColor;
            this.cBImportSVGTool.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGToolColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGTool.Name = "cBImportSVGTool";
            this.toolTip1.SetToolTip(this.cBImportSVGTool, resources.GetString("cBImportSVGTool.ToolTip"));
            this.cBImportSVGTool.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGPauseE
            // 
            resources.ApplyResources(this.cBImportSVGPauseE, "cBImportSVGPauseE");
            this.cBImportSVGPauseE.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPauseElement;
            this.cBImportSVGPauseE.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPauseElement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPauseE.Name = "cBImportSVGPauseE";
            this.cBImportSVGPauseE.UseVisualStyleBackColor = true;
            // 
            // nUDSVGScale
            // 
            this.nUDSVGScale.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importSVGMaxSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSVGScale.DecimalPlaces = 1;
            this.nUDSVGScale.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDSVGScale, "nUDSVGScale");
            this.nUDSVGScale.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDSVGScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDSVGScale.Name = "nUDSVGScale";
            this.nUDSVGScale.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGMaxSize;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // nUDImportReduce
            // 
            this.nUDImportReduce.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importSVGReduceLimit", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportReduce.DecimalPlaces = 2;
            this.nUDImportReduce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDImportReduce, "nUDImportReduce");
            this.nUDImportReduce.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDImportReduce.Name = "nUDImportReduce";
            this.nUDImportReduce.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGReduceLimit;
            // 
            // cBImportSVGReduce
            // 
            resources.ApplyResources(this.cBImportSVGReduce, "cBImportSVGReduce");
            this.cBImportSVGReduce.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGReduce;
            this.cBImportSVGReduce.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGReduce.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGReduce", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGReduce.Name = "cBImportSVGReduce";
            this.cBImportSVGReduce.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGComments
            // 
            resources.ApplyResources(this.cBImportSVGComments, "cBImportSVGComments");
            this.cBImportSVGComments.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGAddComments;
            this.cBImportSVGComments.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGAddComments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGComments.Name = "cBImportSVGComments";
            this.cBImportSVGComments.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // cBImportSVGResize
            // 
            resources.ApplyResources(this.cBImportSVGResize, "cBImportSVGResize");
            this.cBImportSVGResize.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGRezise;
            this.cBImportSVGResize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGResize.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGRezise", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGResize.Name = "cBImportSVGResize";
            this.cBImportSVGResize.UseVisualStyleBackColor = true;
            this.cBImportSVGResize.CheckedChanged += new System.EventHandler(this.cBImportSVGResize_CheckedChanged);
            // 
            // nUDImportSVGSegemnts
            // 
            this.nUDImportSVGSegemnts.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importSVGBezier", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDImportSVGSegemnts, "nUDImportSVGSegemnts");
            this.nUDImportSVGSegemnts.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nUDImportSVGSegemnts.Name = "nUDImportSVGSegemnts";
            this.nUDImportSVGSegemnts.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGBezier;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nUDImportDecPlaces);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.cBImportGCTool);
            this.groupBox2.Controls.Add(this.cBImportGCComments);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.tBImportGCFooter);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tBImportGCHeader);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.nUDImportGCFeedXY);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.label11);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // nUDImportDecPlaces
            // 
            resources.ApplyResources(this.nUDImportDecPlaces, "nUDImportDecPlaces");
            this.nUDImportDecPlaces.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nUDImportDecPlaces.Name = "nUDImportDecPlaces";
            this.nUDImportDecPlaces.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nUDImportDecPlaces.ValueChanged += new System.EventHandler(this.nUDImportDecPlaces_ValueChanged);
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // cBImportGCTool
            // 
            resources.ApplyResources(this.cBImportGCTool, "cBImportGCTool");
            this.cBImportGCTool.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCTool;
            this.cBImportGCTool.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportGCTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCTool.Name = "cBImportGCTool";
            this.cBImportGCTool.UseVisualStyleBackColor = true;
            // 
            // cBImportGCComments
            // 
            resources.ApplyResources(this.cBImportGCComments, "cBImportGCComments");
            this.cBImportGCComments.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCAddComments;
            this.cBImportGCComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportGCComments.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCAddComments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCComments.Name = "cBImportGCComments";
            this.cBImportGCComments.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // tBImportGCFooter
            // 
            this.tBImportGCFooter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCFooter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBImportGCFooter, "tBImportGCFooter");
            this.tBImportGCFooter.Name = "tBImportGCFooter";
            this.tBImportGCFooter.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCFooter;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // tBImportGCHeader
            // 
            this.tBImportGCHeader.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCHeader", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBImportGCHeader, "tBImportGCHeader");
            this.tBImportGCHeader.Name = "tBImportGCHeader";
            this.tBImportGCHeader.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCHeader;
            this.toolTip1.SetToolTip(this.tBImportGCHeader, resources.GetString("tBImportGCHeader.ToolTip"));
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rBImportGCSpindleCmd2);
            this.groupBox6.Controls.Add(this.rBImportGCSpindleCmd1);
            this.groupBox6.Controls.Add(this.cBImportGCUseSpindle);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.nUDImportGCSSpeed);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox6, resources.GetString("groupBox6.ToolTip"));
            // 
            // rBImportGCSpindleCmd2
            // 
            resources.ApplyResources(this.rBImportGCSpindleCmd2, "rBImportGCSpindleCmd2");
            this.rBImportGCSpindleCmd2.Name = "rBImportGCSpindleCmd2";
            this.toolTip1.SetToolTip(this.rBImportGCSpindleCmd2, resources.GetString("rBImportGCSpindleCmd2.ToolTip"));
            this.rBImportGCSpindleCmd2.UseVisualStyleBackColor = true;
            // 
            // rBImportGCSpindleCmd1
            // 
            resources.ApplyResources(this.rBImportGCSpindleCmd1, "rBImportGCSpindleCmd1");
            this.rBImportGCSpindleCmd1.Checked = true;
            this.rBImportGCSpindleCmd1.Name = "rBImportGCSpindleCmd1";
            this.rBImportGCSpindleCmd1.TabStop = true;
            this.toolTip1.SetToolTip(this.rBImportGCSpindleCmd1, resources.GetString("rBImportGCSpindleCmd1.ToolTip"));
            this.rBImportGCSpindleCmd1.UseVisualStyleBackColor = true;
            // 
            // cBImportGCUseSpindle
            // 
            resources.ApplyResources(this.cBImportGCUseSpindle, "cBImportGCUseSpindle");
            this.cBImportGCUseSpindle.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCSpindleToggle;
            this.cBImportGCUseSpindle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCSpindleToggle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCUseSpindle.Name = "cBImportGCUseSpindle";
            this.toolTip1.SetToolTip(this.cBImportGCUseSpindle, resources.GetString("cBImportGCUseSpindle.ToolTip"));
            this.cBImportGCUseSpindle.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // nUDImportGCSSpeed
            // 
            this.nUDImportGCSSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCSSpeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCSSpeed.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDImportGCSSpeed, "nUDImportGCSSpeed");
            this.nUDImportGCSSpeed.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDImportGCSSpeed.Name = "nUDImportGCSSpeed";
            this.nUDImportGCSSpeed.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCSSpeed;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cBImportGCUsePWM);
            this.groupBox4.Controls.Add(this.nUDImportGCDlyDown);
            this.groupBox4.Controls.Add(this.nUDImportGCDlyUp);
            this.groupBox4.Controls.Add(this.nUDImportGCPWMDown);
            this.groupBox4.Controls.Add(this.nUDImportGCPWMUp);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label10);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // cBImportGCUsePWM
            // 
            resources.ApplyResources(this.cBImportGCUsePWM, "cBImportGCUsePWM");
            this.cBImportGCUsePWM.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMEnable;
            this.cBImportGCUsePWM.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCUsePWM.Name = "cBImportGCUsePWM";
            this.toolTip1.SetToolTip(this.cBImportGCUsePWM, resources.GetString("cBImportGCUsePWM.ToolTip"));
            this.cBImportGCUsePWM.UseVisualStyleBackColor = true;
            // 
            // nUDImportGCDlyDown
            // 
            this.nUDImportGCDlyDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMDlyDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCDlyDown.DecimalPlaces = 2;
            this.nUDImportGCDlyDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDImportGCDlyDown, "nUDImportGCDlyDown");
            this.nUDImportGCDlyDown.Name = "nUDImportGCDlyDown";
            this.toolTip1.SetToolTip(this.nUDImportGCDlyDown, resources.GetString("nUDImportGCDlyDown.ToolTip"));
            this.nUDImportGCDlyDown.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMDlyDown;
            // 
            // nUDImportGCDlyUp
            // 
            this.nUDImportGCDlyUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMDlyUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCDlyUp.DecimalPlaces = 2;
            this.nUDImportGCDlyUp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDImportGCDlyUp, "nUDImportGCDlyUp");
            this.nUDImportGCDlyUp.Name = "nUDImportGCDlyUp";
            this.toolTip1.SetToolTip(this.nUDImportGCDlyUp, resources.GetString("nUDImportGCDlyUp.ToolTip"));
            this.nUDImportGCDlyUp.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMDlyUp;
            // 
            // nUDImportGCPWMDown
            // 
            this.nUDImportGCPWMDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCPWMDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDImportGCPWMDown, "nUDImportGCPWMDown");
            this.nUDImportGCPWMDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDImportGCPWMDown.Name = "nUDImportGCPWMDown";
            this.toolTip1.SetToolTip(this.nUDImportGCPWMDown, resources.GetString("nUDImportGCPWMDown.ToolTip"));
            this.nUDImportGCPWMDown.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMDown;
            // 
            // nUDImportGCPWMUp
            // 
            this.nUDImportGCPWMUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCPWMUp.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDImportGCPWMUp, "nUDImportGCPWMUp");
            this.nUDImportGCPWMUp.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDImportGCPWMUp.Name = "nUDImportGCPWMUp";
            this.toolTip1.SetToolTip(this.nUDImportGCPWMUp, resources.GetString("nUDImportGCPWMUp.ToolTip"));
            this.nUDImportGCPWMUp.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMUp;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // nUDImportGCFeedXY
            // 
            this.nUDImportGCFeedXY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCXYFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDImportGCFeedXY, "nUDImportGCFeedXY");
            this.nUDImportGCFeedXY.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nUDImportGCFeedXY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDImportGCFeedXY.Name = "nUDImportGCFeedXY";
            this.nUDImportGCFeedXY.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCXYFeed;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cBImportGCUseZ);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.nUDImportGCZUp);
            this.groupBox5.Controls.Add(this.nUDImportGCFeedZ);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.nUDImportGCZDown);
            this.groupBox5.Controls.Add(this.label5);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // cBImportGCUseZ
            // 
            resources.ApplyResources(this.cBImportGCUseZ, "cBImportGCUseZ");
            this.cBImportGCUseZ.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCZEnable;
            this.cBImportGCUseZ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportGCUseZ.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCZEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCUseZ.Name = "cBImportGCUseZ";
            this.toolTip1.SetToolTip(this.cBImportGCUseZ, resources.GetString("cBImportGCUseZ.ToolTip"));
            this.cBImportGCUseZ.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nUDImportGCZUp
            // 
            this.nUDImportGCZUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCZUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCZUp.DecimalPlaces = 1;
            this.nUDImportGCZUp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDImportGCZUp, "nUDImportGCZUp");
            this.nUDImportGCZUp.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDImportGCZUp.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDImportGCZUp.Name = "nUDImportGCZUp";
            this.nUDImportGCZUp.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZUp;
            // 
            // nUDImportGCFeedZ
            // 
            this.nUDImportGCFeedZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCZFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCFeedZ.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDImportGCFeedZ, "nUDImportGCFeedZ");
            this.nUDImportGCFeedZ.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.nUDImportGCFeedZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDImportGCFeedZ.Name = "nUDImportGCFeedZ";
            this.nUDImportGCFeedZ.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZFeed;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // nUDImportGCZDown
            // 
            this.nUDImportGCZDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCZDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCZDown.DecimalPlaces = 1;
            this.nUDImportGCZDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDImportGCZDown, "nUDImportGCZDown");
            this.nUDImportGCZDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDImportGCZDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDImportGCZDown.Name = "nUDImportGCZDown";
            this.nUDImportGCZDown.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZDown;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox10);
            this.tabPage2.Controls.Add(this.groupBox9);
            this.tabPage2.Controls.Add(this.groupBox7);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.groupBox11);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.rBCtrlReplaceM4);
            this.groupBox11.Controls.Add(this.rBCtrlReplaceM3);
            this.groupBox11.Controls.Add(this.cBCtrlMCmd);
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // rBCtrlReplaceM4
            // 
            resources.ApplyResources(this.rBCtrlReplaceM4, "rBCtrlReplaceM4");
            this.rBCtrlReplaceM4.Name = "rBCtrlReplaceM4";
            this.rBCtrlReplaceM4.TabStop = true;
            this.toolTip1.SetToolTip(this.rBCtrlReplaceM4, resources.GetString("rBCtrlReplaceM4.ToolTip"));
            this.rBCtrlReplaceM4.UseVisualStyleBackColor = true;
            // 
            // rBCtrlReplaceM3
            // 
            resources.ApplyResources(this.rBCtrlReplaceM3, "rBCtrlReplaceM3");
            this.rBCtrlReplaceM3.Name = "rBCtrlReplaceM3";
            this.rBCtrlReplaceM3.TabStop = true;
            this.toolTip1.SetToolTip(this.rBCtrlReplaceM3, resources.GetString("rBCtrlReplaceM3.ToolTip"));
            this.rBCtrlReplaceM3.UseVisualStyleBackColor = true;
            // 
            // cBCtrlMCmd
            // 
            resources.ApplyResources(this.cBCtrlMCmd, "cBCtrlMCmd");
            this.cBCtrlMCmd.Checked = global::GRBL_Plotter.Properties.Settings.Default.ctrlReplaceEnable;
            this.cBCtrlMCmd.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "ctrlReplaceEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBCtrlMCmd.Name = "cBCtrlMCmd";
            this.toolTip1.SetToolTip(this.cBCtrlMCmd, resources.GetString("cBCtrlMCmd.ToolTip"));
            this.cBCtrlMCmd.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.cBSerial2);
            this.groupBox9.Controls.Add(this.cBSerialMinimize);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // cBSerial2
            // 
            resources.ApplyResources(this.cBSerial2, "cBSerial2");
            this.cBSerial2.Checked = global::GRBL_Plotter.Properties.Settings.Default.useSerial2;
            this.cBSerial2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBSerial2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "useSerial2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBSerial2.Name = "cBSerial2";
            this.toolTip1.SetToolTip(this.cBSerial2, resources.GetString("cBSerial2.ToolTip"));
            this.cBSerial2.UseVisualStyleBackColor = true;
            // 
            // cBSerialMinimize
            // 
            resources.ApplyResources(this.cBSerialMinimize, "cBSerialMinimize");
            this.cBSerialMinimize.Checked = global::GRBL_Plotter.Properties.Settings.Default.serialMinimize;
            this.cBSerialMinimize.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "serialMinimize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBSerialMinimize.Name = "cBSerialMinimize";
            this.toolTip1.SetToolTip(this.cBSerialMinimize, resources.GetString("cBSerialMinimize.ToolTip"));
            this.cBSerialMinimize.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label18);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptProbe);
            this.groupBox7.Controls.Add(this.label17);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptSelect);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptPut);
            this.groupBox7.Controls.Add(this.label15);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptGet);
            this.groupBox7.Controls.Add(this.cBToolChange);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // tBToolChangeScriptProbe
            // 
            this.tBToolChangeScriptProbe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptProbe", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBToolChangeScriptProbe, "tBToolChangeScriptProbe");
            this.tBToolChangeScriptProbe.Name = "tBToolChangeScriptProbe";
            this.tBToolChangeScriptProbe.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptProbe;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptProbe, resources.GetString("tBToolChangeScriptProbe.ToolTip"));
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // tBToolChangeScriptSelect
            // 
            this.tBToolChangeScriptSelect.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptSelect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBToolChangeScriptSelect, "tBToolChangeScriptSelect");
            this.tBToolChangeScriptSelect.Name = "tBToolChangeScriptSelect";
            this.tBToolChangeScriptSelect.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptSelect;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptSelect, resources.GetString("tBToolChangeScriptSelect.ToolTip"));
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // tBToolChangeScriptPut
            // 
            this.tBToolChangeScriptPut.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptPut", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBToolChangeScriptPut, "tBToolChangeScriptPut");
            this.tBToolChangeScriptPut.Name = "tBToolChangeScriptPut";
            this.tBToolChangeScriptPut.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptPut;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptPut, resources.GetString("tBToolChangeScriptPut.ToolTip"));
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // tBToolChangeScriptGet
            // 
            this.tBToolChangeScriptGet.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptGet", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBToolChangeScriptGet, "tBToolChangeScriptGet");
            this.tBToolChangeScriptGet.Name = "tBToolChangeScriptGet";
            this.tBToolChangeScriptGet.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptGet;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptGet, resources.GetString("tBToolChangeScriptGet.ToolTip"));
            // 
            // cBToolChange
            // 
            resources.ApplyResources(this.cBToolChange, "cBToolChange");
            this.cBToolChange.Checked = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolChange;
            this.cBToolChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBToolChange.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolChange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolChange.Name = "cBToolChange";
            this.cBToolChange.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblcbnr);
            this.tabPage1.Controls.Add(this.btnChangeDefinition);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.lvCustomButtons);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblcbnr
            // 
            resources.ApplyResources(this.lblcbnr, "lblcbnr");
            this.lblcbnr.Name = "lblcbnr";
            // 
            // btnChangeDefinition
            // 
            resources.ApplyResources(this.btnChangeDefinition, "btnChangeDefinition");
            this.btnChangeDefinition.Name = "btnChangeDefinition";
            this.toolTip1.SetToolTip(this.btnChangeDefinition, resources.GetString("btnChangeDefinition.ToolTip"));
            this.btnChangeDefinition.UseVisualStyleBackColor = true;
            this.btnChangeDefinition.Click += new System.EventHandler(this.btnChangeDefinition_Click);
            // 
            // textBox2
            // 
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.Name = "textBox2";
            this.toolTip1.SetToolTip(this.textBox2, resources.GetString("textBox2.ToolTip"));
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.toolTip1.SetToolTip(this.textBox1, resources.GetString("textBox1.ToolTip"));
            // 
            // lvCustomButtons
            // 
            this.lvCustomButtons.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvCustomButtons.FullRowSelect = true;
            this.lvCustomButtons.GridLines = true;
            this.lvCustomButtons.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            resources.ApplyResources(this.lvCustomButtons, "lvCustomButtons");
            this.lvCustomButtons.MultiSelect = false;
            this.lvCustomButtons.Name = "lvCustomButtons";
            this.lvCustomButtons.Scrollable = false;
            this.lvCustomButtons.UseCompatibleStateImageBehavior = false;
            this.lvCustomButtons.View = System.Windows.Forms.View.Details;
            this.lvCustomButtons.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvCustomButtons_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Controls.Add(this.groupBox8);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nUDMarker);
            this.groupBox1.Controls.Add(this.btnColorMarker);
            this.groupBox1.Controls.Add(this.nUDTool);
            this.groupBox1.Controls.Add(this.nUDPenDown);
            this.groupBox1.Controls.Add(this.nUDPenUp);
            this.groupBox1.Controls.Add(this.nUDRuler);
            this.groupBox1.Controls.Add(this.btnColorTool);
            this.groupBox1.Controls.Add(this.btnColorPenDown);
            this.groupBox1.Controls.Add(this.btnColorPenUp);
            this.groupBox1.Controls.Add(this.btnColorRuler);
            this.groupBox1.Controls.Add(this.btnColorBackground);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // nUDMarker
            // 
            this.nUDMarker.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthMarker", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMarker.DecimalPlaces = 1;
            this.nUDMarker.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDMarker, "nUDMarker");
            this.nUDMarker.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDMarker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDMarker.Name = "nUDMarker";
            this.toolTip1.SetToolTip(this.nUDMarker, resources.GetString("nUDMarker.ToolTip"));
            this.nUDMarker.Value = global::GRBL_Plotter.Properties.Settings.Default.widthMarker;
            // 
            // btnColorMarker
            // 
            resources.ApplyResources(this.btnColorMarker, "btnColorMarker");
            this.btnColorMarker.Name = "btnColorMarker";
            this.btnColorMarker.UseVisualStyleBackColor = true;
            // 
            // nUDTool
            // 
            this.nUDTool.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDTool.DecimalPlaces = 1;
            this.nUDTool.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDTool, "nUDTool");
            this.nUDTool.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDTool.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDTool.Name = "nUDTool";
            this.toolTip1.SetToolTip(this.nUDTool, resources.GetString("nUDTool.ToolTip"));
            this.nUDTool.Value = global::GRBL_Plotter.Properties.Settings.Default.widthTool;
            // 
            // nUDPenDown
            // 
            this.nUDPenDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthPenDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPenDown.DecimalPlaces = 1;
            this.nUDPenDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDPenDown, "nUDPenDown");
            this.nUDPenDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDPenDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDPenDown.Name = "nUDPenDown";
            this.toolTip1.SetToolTip(this.nUDPenDown, resources.GetString("nUDPenDown.ToolTip"));
            this.nUDPenDown.Value = global::GRBL_Plotter.Properties.Settings.Default.widthPenDown;
            // 
            // nUDPenUp
            // 
            this.nUDPenUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthPenUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPenUp.DecimalPlaces = 1;
            this.nUDPenUp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDPenUp, "nUDPenUp");
            this.nUDPenUp.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDPenUp.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDPenUp.Name = "nUDPenUp";
            this.toolTip1.SetToolTip(this.nUDPenUp, resources.GetString("nUDPenUp.ToolTip"));
            this.nUDPenUp.Value = global::GRBL_Plotter.Properties.Settings.Default.widthPenUp;
            // 
            // nUDRuler
            // 
            this.nUDRuler.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthRuler", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDRuler.DecimalPlaces = 1;
            this.nUDRuler.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDRuler, "nUDRuler");
            this.nUDRuler.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDRuler.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDRuler.Name = "nUDRuler";
            this.toolTip1.SetToolTip(this.nUDRuler, resources.GetString("nUDRuler.ToolTip"));
            this.nUDRuler.Value = global::GRBL_Plotter.Properties.Settings.Default.widthRuler;
            // 
            // btnColorTool
            // 
            resources.ApplyResources(this.btnColorTool, "btnColorTool");
            this.btnColorTool.Name = "btnColorTool";
            this.btnColorTool.UseVisualStyleBackColor = true;
            // 
            // btnColorPenDown
            // 
            resources.ApplyResources(this.btnColorPenDown, "btnColorPenDown");
            this.btnColorPenDown.Name = "btnColorPenDown";
            this.btnColorPenDown.UseVisualStyleBackColor = true;
            // 
            // btnColorPenUp
            // 
            resources.ApplyResources(this.btnColorPenUp, "btnColorPenUp");
            this.btnColorPenUp.Name = "btnColorPenUp";
            this.btnColorPenUp.UseVisualStyleBackColor = true;
            // 
            // btnColorRuler
            // 
            resources.ApplyResources(this.btnColorRuler, "btnColorRuler");
            this.btnColorRuler.Name = "btnColorRuler";
            this.btnColorRuler.UseVisualStyleBackColor = true;
            // 
            // btnColorBackground
            // 
            resources.ApplyResources(this.btnColorBackground, "btnColorBackground");
            this.btnColorBackground.Name = "btnColorBackground";
            this.btnColorBackground.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.btnJoyZCalc);
            this.groupBox8.Controls.Add(this.label28);
            this.groupBox8.Controls.Add(this.nUDJoyZSpeed5);
            this.groupBox8.Controls.Add(this.nUDJoyZSpeed4);
            this.groupBox8.Controls.Add(this.nUDJoyZSpeed3);
            this.groupBox8.Controls.Add(this.nUDJoyZSpeed2);
            this.groupBox8.Controls.Add(this.nUDJoyZSpeed1);
            this.groupBox8.Controls.Add(this.nUDJoyZStep5);
            this.groupBox8.Controls.Add(this.nUDJoyZStep4);
            this.groupBox8.Controls.Add(this.nUDJoyZStep3);
            this.groupBox8.Controls.Add(this.nUDJoyZStep2);
            this.groupBox8.Controls.Add(this.nUDJoyZStep1);
            this.groupBox8.Controls.Add(this.label25);
            this.groupBox8.Controls.Add(this.label24);
            this.groupBox8.Controls.Add(this.label23);
            this.groupBox8.Controls.Add(this.label22);
            this.groupBox8.Controls.Add(this.label21);
            this.groupBox8.Controls.Add(this.label20);
            this.groupBox8.Controls.Add(this.btnJoyXYCalc);
            this.groupBox8.Controls.Add(this.nUDJoyXYSpeed5);
            this.groupBox8.Controls.Add(this.nUDJoyXYSpeed4);
            this.groupBox8.Controls.Add(this.nUDJoyXYSpeed3);
            this.groupBox8.Controls.Add(this.nUDJoyXYSpeed2);
            this.groupBox8.Controls.Add(this.nUDJoyXYSpeed1);
            this.groupBox8.Controls.Add(this.nUDJoyXYStep5);
            this.groupBox8.Controls.Add(this.nUDJoyXYStep4);
            this.groupBox8.Controls.Add(this.nUDJoyXYStep3);
            this.groupBox8.Controls.Add(this.nUDJoyXYStep2);
            this.groupBox8.Controls.Add(this.nUDJoyXYStep1);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // btnJoyZCalc
            // 
            resources.ApplyResources(this.btnJoyZCalc, "btnJoyZCalc");
            this.btnJoyZCalc.Name = "btnJoyZCalc";
            this.toolTip1.SetToolTip(this.btnJoyZCalc, resources.GetString("btnJoyZCalc.ToolTip"));
            this.btnJoyZCalc.UseVisualStyleBackColor = true;
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // nUDJoyZSpeed5
            // 
            this.nUDJoyZSpeed5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyZSpeed5, "nUDJoyZSpeed5");
            this.nUDJoyZSpeed5.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyZSpeed5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyZSpeed5.Name = "nUDJoyZSpeed5";
            this.nUDJoyZSpeed5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed5;
            // 
            // nUDJoyZSpeed4
            // 
            this.nUDJoyZSpeed4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyZSpeed4, "nUDJoyZSpeed4");
            this.nUDJoyZSpeed4.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyZSpeed4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyZSpeed4.Name = "nUDJoyZSpeed4";
            this.nUDJoyZSpeed4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed4;
            // 
            // nUDJoyZSpeed3
            // 
            this.nUDJoyZSpeed3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyZSpeed3, "nUDJoyZSpeed3");
            this.nUDJoyZSpeed3.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyZSpeed3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyZSpeed3.Name = "nUDJoyZSpeed3";
            this.nUDJoyZSpeed3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed3;
            // 
            // nUDJoyZSpeed2
            // 
            this.nUDJoyZSpeed2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyZSpeed2, "nUDJoyZSpeed2");
            this.nUDJoyZSpeed2.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyZSpeed2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyZSpeed2.Name = "nUDJoyZSpeed2";
            this.nUDJoyZSpeed2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed2;
            // 
            // nUDJoyZSpeed1
            // 
            this.nUDJoyZSpeed1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyZSpeed1, "nUDJoyZSpeed1");
            this.nUDJoyZSpeed1.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyZSpeed1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyZSpeed1.Name = "nUDJoyZSpeed1";
            this.nUDJoyZSpeed1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed1;
            // 
            // nUDJoyZStep5
            // 
            this.nUDJoyZStep5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep5.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyZStep5, "nUDJoyZStep5");
            this.nUDJoyZStep5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep5.Name = "nUDJoyZStep5";
            this.nUDJoyZStep5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep5;
            // 
            // nUDJoyZStep4
            // 
            this.nUDJoyZStep4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep4.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyZStep4, "nUDJoyZStep4");
            this.nUDJoyZStep4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep4.Name = "nUDJoyZStep4";
            this.nUDJoyZStep4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep4;
            // 
            // nUDJoyZStep3
            // 
            this.nUDJoyZStep3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep3.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyZStep3, "nUDJoyZStep3");
            this.nUDJoyZStep3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep3.Name = "nUDJoyZStep3";
            this.nUDJoyZStep3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep3;
            // 
            // nUDJoyZStep2
            // 
            this.nUDJoyZStep2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep2.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyZStep2, "nUDJoyZStep2");
            this.nUDJoyZStep2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep2.Name = "nUDJoyZStep2";
            this.nUDJoyZStep2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep2;
            // 
            // nUDJoyZStep1
            // 
            this.nUDJoyZStep1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep1.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyZStep1, "nUDJoyZStep1");
            this.nUDJoyZStep1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep1.Name = "nUDJoyZStep1";
            this.nUDJoyZStep1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep1;
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // btnJoyXYCalc
            // 
            resources.ApplyResources(this.btnJoyXYCalc, "btnJoyXYCalc");
            this.btnJoyXYCalc.Name = "btnJoyXYCalc";
            this.toolTip1.SetToolTip(this.btnJoyXYCalc, resources.GetString("btnJoyXYCalc.ToolTip"));
            this.btnJoyXYCalc.UseVisualStyleBackColor = true;
            // 
            // nUDJoyXYSpeed5
            // 
            this.nUDJoyXYSpeed5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyXYSpeed5, "nUDJoyXYSpeed5");
            this.nUDJoyXYSpeed5.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyXYSpeed5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyXYSpeed5.Name = "nUDJoyXYSpeed5";
            this.nUDJoyXYSpeed5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed5;
            // 
            // nUDJoyXYSpeed4
            // 
            this.nUDJoyXYSpeed4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyXYSpeed4, "nUDJoyXYSpeed4");
            this.nUDJoyXYSpeed4.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyXYSpeed4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyXYSpeed4.Name = "nUDJoyXYSpeed4";
            this.nUDJoyXYSpeed4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed4;
            // 
            // nUDJoyXYSpeed3
            // 
            this.nUDJoyXYSpeed3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyXYSpeed3, "nUDJoyXYSpeed3");
            this.nUDJoyXYSpeed3.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyXYSpeed3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyXYSpeed3.Name = "nUDJoyXYSpeed3";
            this.nUDJoyXYSpeed3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed3;
            // 
            // nUDJoyXYSpeed2
            // 
            this.nUDJoyXYSpeed2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyXYSpeed2, "nUDJoyXYSpeed2");
            this.nUDJoyXYSpeed2.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyXYSpeed2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyXYSpeed2.Name = "nUDJoyXYSpeed2";
            this.nUDJoyXYSpeed2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed2;
            // 
            // nUDJoyXYSpeed1
            // 
            this.nUDJoyXYSpeed1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDJoyXYSpeed1, "nUDJoyXYSpeed1");
            this.nUDJoyXYSpeed1.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nUDJoyXYSpeed1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDJoyXYSpeed1.Name = "nUDJoyXYSpeed1";
            this.nUDJoyXYSpeed1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed1;
            // 
            // nUDJoyXYStep5
            // 
            this.nUDJoyXYStep5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep5.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyXYStep5, "nUDJoyXYStep5");
            this.nUDJoyXYStep5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep5.Name = "nUDJoyXYStep5";
            this.nUDJoyXYStep5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep5;
            // 
            // nUDJoyXYStep4
            // 
            this.nUDJoyXYStep4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep4.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyXYStep4, "nUDJoyXYStep4");
            this.nUDJoyXYStep4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep4.Name = "nUDJoyXYStep4";
            this.nUDJoyXYStep4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep4;
            // 
            // nUDJoyXYStep3
            // 
            this.nUDJoyXYStep3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep3.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyXYStep3, "nUDJoyXYStep3");
            this.nUDJoyXYStep3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep3.Name = "nUDJoyXYStep3";
            this.nUDJoyXYStep3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep3;
            // 
            // nUDJoyXYStep2
            // 
            this.nUDJoyXYStep2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep2.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyXYStep2, "nUDJoyXYStep2");
            this.nUDJoyXYStep2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep2.Name = "nUDJoyXYStep2";
            this.nUDJoyXYStep2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep2;
            // 
            // nUDJoyXYStep1
            // 
            this.nUDJoyXYStep1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep1.DecimalPlaces = 3;
            resources.ApplyResources(this.nUDJoyXYStep1, "nUDJoyXYStep1");
            this.nUDJoyXYStep1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep1.Name = "nUDJoyXYStep1";
            this.nUDJoyXYStep1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep1;
            // 
            // btnApplyChangings
            // 
            resources.ApplyResources(this.btnApplyChangings, "btnApplyChangings");
            this.btnApplyChangings.Name = "btnApplyChangings";
            this.toolTip1.SetToolTip(this.btnApplyChangings, resources.GetString("btnApplyChangings.ToolTip"));
            this.btnApplyChangings.UseVisualStyleBackColor = true;
            this.btnApplyChangings.Click += new System.EventHandler(this.btnApplyChangings_Click);
            // 
            // ControlSetupForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnApplyChangings);
            this.Controls.Add(this.tabControl1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::GRBL_Plotter.Properties.Settings.Default, "locationSetForm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::GRBL_Plotter.Properties.Settings.Default.locationSetForm;
            this.Name = "ControlSetupForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSVGScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportReduce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportSVGSegemnts)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportDecPlaces)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSSpeed)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedXY)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nUDMarker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDTool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRuler)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZSpeed1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyZStep1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYSpeed1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDJoyXYStep1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lvCustomButtons;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnChangeDefinition;
        public System.Windows.Forms.Button btnApplyChangings;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cBImportSVGResize;
        private System.Windows.Forms.NumericUpDown nUDImportSVGSegemnts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown nUDImportGCSSpeed;
        private System.Windows.Forms.NumericUpDown nUDImportGCFeedZ;
        private System.Windows.Forms.NumericUpDown nUDImportGCZUp;
        private System.Windows.Forms.NumericUpDown nUDImportGCZDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cBImportSVGComments;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cBImportGCUseSpindle;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cBImportGCUsePWM;
        private System.Windows.Forms.NumericUpDown nUDImportGCDlyDown;
        private System.Windows.Forms.NumericUpDown nUDImportGCDlyUp;
        private System.Windows.Forms.NumericUpDown nUDImportGCPWMDown;
        private System.Windows.Forms.NumericUpDown nUDImportGCPWMUp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nUDImportGCFeedXY;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cBImportGCUseZ;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tBImportGCFooter;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tBImportGCHeader;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nUDImportReduce;
        private System.Windows.Forms.CheckBox cBImportSVGReduce;
        private System.Windows.Forms.CheckBox cBImportGCComments;
        private System.Windows.Forms.NumericUpDown nUDSVGScale;
        public System.Windows.Forms.Button btnReloadFile;
        private System.Windows.Forms.CheckBox cBImportSVGPauseE;
        private System.Windows.Forms.CheckBox cBImportSVGPauseP;
        private System.Windows.Forms.Label lblcbnr;
        private System.Windows.Forms.Button btnResizeForm;
        private System.Windows.Forms.CheckBox cBImportSVGPath;
        private System.Windows.Forms.CheckBox cBSerialMinimize;
        private System.Windows.Forms.TextBox tBImportSVGPalette;
        private System.Windows.Forms.CheckBox cBImportSVGTool;
        private System.Windows.Forms.CheckBox cBImportGCTool;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox tBToolChangeScriptGet;
        private System.Windows.Forms.CheckBox cBToolChange;
        private System.Windows.Forms.CheckBox cBSerial2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tBToolChangeScriptPut;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tBToolChangeScriptSelect;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tBToolChangeScriptProbe;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown nUDImportDecPlaces;
        private System.Windows.Forms.RadioButton rBImportGCSpindleCmd2;
        private System.Windows.Forms.RadioButton rBImportGCSpindleCmd1;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.RadioButton rBCtrlReplaceM4;
        private System.Windows.Forms.RadioButton rBCtrlReplaceM3;
        private System.Windows.Forms.CheckBox cBCtrlMCmd;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nUDMarker;
        private System.Windows.Forms.Button btnColorMarker;
        private System.Windows.Forms.NumericUpDown nUDTool;
        private System.Windows.Forms.NumericUpDown nUDPenDown;
        private System.Windows.Forms.NumericUpDown nUDPenUp;
        private System.Windows.Forms.NumericUpDown nUDRuler;
        private System.Windows.Forms.Button btnColorTool;
        private System.Windows.Forms.Button btnColorPenDown;
        private System.Windows.Forms.Button btnColorPenUp;
        private System.Windows.Forms.Button btnColorRuler;
        private System.Windows.Forms.Button btnColorBackground;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button btnJoyZCalc;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.NumericUpDown nUDJoyZSpeed5;
        private System.Windows.Forms.NumericUpDown nUDJoyZSpeed4;
        private System.Windows.Forms.NumericUpDown nUDJoyZSpeed3;
        private System.Windows.Forms.NumericUpDown nUDJoyZSpeed2;
        private System.Windows.Forms.NumericUpDown nUDJoyZSpeed1;
        private System.Windows.Forms.NumericUpDown nUDJoyZStep5;
        private System.Windows.Forms.NumericUpDown nUDJoyZStep4;
        private System.Windows.Forms.NumericUpDown nUDJoyZStep3;
        private System.Windows.Forms.NumericUpDown nUDJoyZStep2;
        private System.Windows.Forms.NumericUpDown nUDJoyZStep1;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnJoyXYCalc;
        private System.Windows.Forms.NumericUpDown nUDJoyXYSpeed5;
        private System.Windows.Forms.NumericUpDown nUDJoyXYSpeed4;
        private System.Windows.Forms.NumericUpDown nUDJoyXYSpeed3;
        private System.Windows.Forms.NumericUpDown nUDJoyXYSpeed2;
        private System.Windows.Forms.NumericUpDown nUDJoyXYSpeed1;
        private System.Windows.Forms.NumericUpDown nUDJoyXYStep5;
        private System.Windows.Forms.NumericUpDown nUDJoyXYStep4;
        private System.Windows.Forms.NumericUpDown nUDJoyXYStep3;
        private System.Windows.Forms.NumericUpDown nUDJoyXYStep2;
        private System.Windows.Forms.NumericUpDown nUDJoyXYStep1;
    }
}