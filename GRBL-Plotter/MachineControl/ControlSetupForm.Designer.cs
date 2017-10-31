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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnResizeForm = new System.Windows.Forms.Button();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.rBImportDXFSize2 = new System.Windows.Forms.RadioButton();
            this.rBImportDXFSize1 = new System.Windows.Forms.RadioButton();
            this.cBImportSVGPauseP = new System.Windows.Forms.CheckBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.cBImportSVGNodesOnly = new System.Windows.Forms.CheckBox();
            this.cBImportSVGSort = new System.Windows.Forms.CheckBox();
            this.cBImportSVGResize = new System.Windows.Forms.CheckBox();
            this.tBImportSVGPalette = new System.Windows.Forms.TextBox();
            this.nUDSVGScale = new System.Windows.Forms.NumericUpDown();
            this.cBImportSVGPath = new System.Windows.Forms.CheckBox();
            this.cBImportSVGTool = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnReloadFile = new System.Windows.Forms.Button();
            this.cBImportSVGPauseE = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.nUDImportReduce = new System.Windows.Forms.NumericUpDown();
            this.cBImportSVGReduce = new System.Windows.Forms.CheckBox();
            this.cBImportSVGComments = new System.Windows.Forms.CheckBox();
            this.nUDImportSVGSegemnts = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label37 = new System.Windows.Forms.Label();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.label36 = new System.Windows.Forms.Label();
            this.tBImportGCIPD = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.tBImportGCIPU = new System.Windows.Forms.TextBox();
            this.cBImportGCUseIndividual = new System.Windows.Forms.CheckBox();
            this.label34 = new System.Windows.Forms.Label();
            this.nUDImportGCSegment = new System.Windows.Forms.NumericUpDown();
            this.cBImportGCNoArcs = new System.Windows.Forms.CheckBox();
            this.cBImportGCRelative = new System.Windows.Forms.CheckBox();
            this.cBImportGCCompress = new System.Windows.Forms.CheckBox();
            this.nUDImportDecPlaces = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.cBImportGCTool = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cBImportGCComments = new System.Windows.Forms.CheckBox();
            this.nUDImportGCSSpeed = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.tBImportGCFooter = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tBImportGCHeader = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rBImportGCSpindleCmd2 = new System.Windows.Forms.RadioButton();
            this.rBImportGCSpindleCmd1 = new System.Windows.Forms.RadioButton();
            this.cBImportGCUseSpindle = new System.Windows.Forms.CheckBox();
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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label33 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.cBImportGCUseZ = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDImportGCZUp = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCFeedZ = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nUDImportGCZDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cBcheckupdate = new System.Windows.Forms.CheckBox();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.cB4thUse = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label38 = new System.Windows.Forms.Label();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.label31 = new System.Windows.Forms.Label();
            this.tBRotarySetupOff = new System.Windows.Forms.TextBox();
            this.cBRotarySetupApply = new System.Windows.Forms.CheckBox();
            this.tBRotarySetupOn = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.nUDRotaryDiameter = new System.Windows.Forms.NumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.nUDRotaryScale = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.rBRotaryY = new System.Windows.Forms.RadioButton();
            this.rBRotaryX = new System.Windows.Forms.RadioButton();
            this.cBRotarySubstitute = new System.Windows.Forms.CheckBox();
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
            this.nUDHeightMap = new System.Windows.Forms.NumericUpDown();
            this.btnColorHeightMap = new System.Windows.Forms.Button();
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
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox19 = new System.Windows.Forms.GroupBox();
            this.linkLabel8 = new System.Windows.Forms.LinkLabel();
            this.linkLabel7 = new System.Windows.Forms.LinkLabel();
            this.groupBox18 = new System.Windows.Forms.GroupBox();
            this.label39 = new System.Windows.Forms.Label();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnApplyChangings = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox15.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSVGScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportReduce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportSVGSegemnts)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSegment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportDecPlaces)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSSpeed)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedXY)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox17.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRotaryDiameter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRotaryScale)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeightMap)).BeginInit();
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
            this.tabPage5.SuspendLayout();
            this.groupBox19.SuspendLayout();
            this.groupBox18.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip1.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.expandForm_Click);
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Name = "tabPage3";
            this.toolTip1.SetToolTip(this.tabPage3, resources.GetString("tabPage3.ToolTip"));
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.btnResizeForm);
            this.groupBox3.Controls.Add(this.groupBox15);
            this.groupBox3.Controls.Add(this.cBImportSVGPauseP);
            this.groupBox3.Controls.Add(this.groupBox14);
            this.groupBox3.Controls.Add(this.btnReloadFile);
            this.groupBox3.Controls.Add(this.cBImportSVGPauseE);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.nUDImportReduce);
            this.groupBox3.Controls.Add(this.cBImportSVGReduce);
            this.groupBox3.Controls.Add(this.cBImportSVGComments);
            this.groupBox3.Controls.Add(this.nUDImportSVGSegemnts);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox3, resources.GetString("groupBox3.ToolTip"));
            // 
            // btnResizeForm
            // 
            resources.ApplyResources(this.btnResizeForm, "btnResizeForm");
            this.btnResizeForm.BackColor = System.Drawing.Color.Yellow;
            this.btnResizeForm.Name = "btnResizeForm";
            this.toolTip1.SetToolTip(this.btnResizeForm, resources.GetString("btnResizeForm.ToolTip"));
            this.btnResizeForm.UseVisualStyleBackColor = false;
            this.btnResizeForm.Click += new System.EventHandler(this.btnResizeForm_Click);
            // 
            // groupBox15
            // 
            resources.ApplyResources(this.groupBox15, "groupBox15");
            this.groupBox15.Controls.Add(this.rBImportDXFSize2);
            this.groupBox15.Controls.Add(this.rBImportDXFSize1);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox15, resources.GetString("groupBox15.ToolTip"));
            // 
            // rBImportDXFSize2
            // 
            resources.ApplyResources(this.rBImportDXFSize2, "rBImportDXFSize2");
            this.rBImportDXFSize2.Name = "rBImportDXFSize2";
            this.toolTip1.SetToolTip(this.rBImportDXFSize2, resources.GetString("rBImportDXFSize2.ToolTip"));
            this.rBImportDXFSize2.UseVisualStyleBackColor = true;
            // 
            // rBImportDXFSize1
            // 
            resources.ApplyResources(this.rBImportDXFSize1, "rBImportDXFSize1");
            this.rBImportDXFSize1.Checked = global::GRBL_Plotter.Properties.Settings.Default.ImportDXFSize;
            this.rBImportDXFSize1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "ImportDXFSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rBImportDXFSize1.Name = "rBImportDXFSize1";
            this.rBImportDXFSize1.TabStop = true;
            this.toolTip1.SetToolTip(this.rBImportDXFSize1, resources.GetString("rBImportDXFSize1.ToolTip"));
            this.rBImportDXFSize1.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGPauseP
            // 
            resources.ApplyResources(this.cBImportSVGPauseP, "cBImportSVGPauseP");
            this.cBImportSVGPauseP.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPausePenDown;
            this.cBImportSVGPauseP.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPausePenDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPauseP.Name = "cBImportSVGPauseP";
            this.toolTip1.SetToolTip(this.cBImportSVGPauseP, resources.GetString("cBImportSVGPauseP.ToolTip"));
            this.cBImportSVGPauseP.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            resources.ApplyResources(this.groupBox14, "groupBox14");
            this.groupBox14.Controls.Add(this.cBImportSVGNodesOnly);
            this.groupBox14.Controls.Add(this.cBImportSVGSort);
            this.groupBox14.Controls.Add(this.cBImportSVGResize);
            this.groupBox14.Controls.Add(this.tBImportSVGPalette);
            this.groupBox14.Controls.Add(this.nUDSVGScale);
            this.groupBox14.Controls.Add(this.cBImportSVGPath);
            this.groupBox14.Controls.Add(this.cBImportSVGTool);
            this.groupBox14.Controls.Add(this.label7);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox14, resources.GetString("groupBox14.ToolTip"));
            // 
            // cBImportSVGNodesOnly
            // 
            resources.ApplyResources(this.cBImportSVGNodesOnly, "cBImportSVGNodesOnly");
            this.cBImportSVGNodesOnly.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGNodesOnly;
            this.cBImportSVGNodesOnly.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGNodesOnly", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGNodesOnly.Name = "cBImportSVGNodesOnly";
            this.toolTip1.SetToolTip(this.cBImportSVGNodesOnly, resources.GetString("cBImportSVGNodesOnly.ToolTip"));
            this.cBImportSVGNodesOnly.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGSort
            // 
            resources.ApplyResources(this.cBImportSVGSort, "cBImportSVGSort");
            this.cBImportSVGSort.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGToolSort;
            this.cBImportSVGSort.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGToolSort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGSort.Name = "cBImportSVGSort";
            this.toolTip1.SetToolTip(this.cBImportSVGSort, resources.GetString("cBImportSVGSort.ToolTip"));
            this.cBImportSVGSort.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGResize
            // 
            resources.ApplyResources(this.cBImportSVGResize, "cBImportSVGResize");
            this.cBImportSVGResize.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGRezise;
            this.cBImportSVGResize.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGRezise", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGResize.Name = "cBImportSVGResize";
            this.toolTip1.SetToolTip(this.cBImportSVGResize, resources.GetString("cBImportSVGResize.ToolTip"));
            this.cBImportSVGResize.UseVisualStyleBackColor = true;
            // 
            // tBImportSVGPalette
            // 
            resources.ApplyResources(this.tBImportSVGPalette, "tBImportSVGPalette");
            this.tBImportSVGPalette.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importPalette", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportSVGPalette.Name = "tBImportSVGPalette";
            this.tBImportSVGPalette.Text = global::GRBL_Plotter.Properties.Settings.Default.importPalette;
            this.toolTip1.SetToolTip(this.tBImportSVGPalette, resources.GetString("tBImportSVGPalette.ToolTip"));
            // 
            // nUDSVGScale
            // 
            resources.ApplyResources(this.nUDSVGScale, "nUDSVGScale");
            this.nUDSVGScale.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importSVGMaxSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDSVGScale.DecimalPlaces = 1;
            this.nUDSVGScale.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
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
            this.toolTip1.SetToolTip(this.nUDSVGScale, resources.GetString("nUDSVGScale.ToolTip"));
            this.nUDSVGScale.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGMaxSize;
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
            // cBImportSVGTool
            // 
            resources.ApplyResources(this.cBImportSVGTool, "cBImportSVGTool");
            this.cBImportSVGTool.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGToolColor;
            this.cBImportSVGTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGToolColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGTool.Name = "cBImportSVGTool";
            this.toolTip1.SetToolTip(this.cBImportSVGTool, resources.GetString("cBImportSVGTool.ToolTip"));
            this.cBImportSVGTool.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // btnReloadFile
            // 
            resources.ApplyResources(this.btnReloadFile, "btnReloadFile");
            this.btnReloadFile.Name = "btnReloadFile";
            this.toolTip1.SetToolTip(this.btnReloadFile, resources.GetString("btnReloadFile.ToolTip"));
            this.btnReloadFile.UseVisualStyleBackColor = true;
            this.btnReloadFile.Click += new System.EventHandler(this.btnReloadFile_Click);
            // 
            // cBImportSVGPauseE
            // 
            resources.ApplyResources(this.cBImportSVGPauseE, "cBImportSVGPauseE");
            this.cBImportSVGPauseE.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPauseElement;
            this.cBImportSVGPauseE.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPauseElement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPauseE.Name = "cBImportSVGPauseE";
            this.toolTip1.SetToolTip(this.cBImportSVGPauseE, resources.GetString("cBImportSVGPauseE.ToolTip"));
            this.cBImportSVGPauseE.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            this.toolTip1.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
            // 
            // nUDImportReduce
            // 
            resources.ApplyResources(this.nUDImportReduce, "nUDImportReduce");
            this.nUDImportReduce.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importSVGReduceLimit", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportReduce.DecimalPlaces = 2;
            this.nUDImportReduce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDImportReduce.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDImportReduce.Name = "nUDImportReduce";
            this.toolTip1.SetToolTip(this.nUDImportReduce, resources.GetString("nUDImportReduce.ToolTip"));
            this.nUDImportReduce.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGReduceLimit;
            // 
            // cBImportSVGReduce
            // 
            resources.ApplyResources(this.cBImportSVGReduce, "cBImportSVGReduce");
            this.cBImportSVGReduce.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGReduce;
            this.cBImportSVGReduce.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGReduce", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGReduce.Name = "cBImportSVGReduce";
            this.toolTip1.SetToolTip(this.cBImportSVGReduce, resources.GetString("cBImportSVGReduce.ToolTip"));
            this.cBImportSVGReduce.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGComments
            // 
            resources.ApplyResources(this.cBImportSVGComments, "cBImportSVGComments");
            this.cBImportSVGComments.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGAddComments;
            this.cBImportSVGComments.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGAddComments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGComments.Name = "cBImportSVGComments";
            this.toolTip1.SetToolTip(this.cBImportSVGComments, resources.GetString("cBImportSVGComments.ToolTip"));
            this.cBImportSVGComments.UseVisualStyleBackColor = true;
            // 
            // nUDImportSVGSegemnts
            // 
            resources.ApplyResources(this.nUDImportSVGSegemnts, "nUDImportSVGSegemnts");
            this.nUDImportSVGSegemnts.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importSVGBezier", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportSVGSegemnts.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nUDImportSVGSegemnts.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nUDImportSVGSegemnts.Name = "nUDImportSVGSegemnts";
            this.toolTip1.SetToolTip(this.nUDImportSVGSegemnts, resources.GetString("nUDImportSVGSegemnts.ToolTip"));
            this.nUDImportSVGSegemnts.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGBezier;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label37);
            this.groupBox2.Controls.Add(this.groupBox16);
            this.groupBox2.Controls.Add(this.label34);
            this.groupBox2.Controls.Add(this.nUDImportGCSegment);
            this.groupBox2.Controls.Add(this.cBImportGCNoArcs);
            this.groupBox2.Controls.Add(this.cBImportGCRelative);
            this.groupBox2.Controls.Add(this.cBImportGCCompress);
            this.groupBox2.Controls.Add(this.nUDImportDecPlaces);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.cBImportGCTool);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cBImportGCComments);
            this.groupBox2.Controls.Add(this.nUDImportGCSSpeed);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.tBImportGCFooter);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tBImportGCHeader);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.nUDImportGCFeedXY);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            this.toolTip1.SetToolTip(this.label37, resources.GetString("label37.ToolTip"));
            // 
            // groupBox16
            // 
            resources.ApplyResources(this.groupBox16, "groupBox16");
            this.groupBox16.Controls.Add(this.label36);
            this.groupBox16.Controls.Add(this.tBImportGCIPD);
            this.groupBox16.Controls.Add(this.label35);
            this.groupBox16.Controls.Add(this.tBImportGCIPU);
            this.groupBox16.Controls.Add(this.cBImportGCUseIndividual);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox16, resources.GetString("groupBox16.ToolTip"));
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            this.toolTip1.SetToolTip(this.label36, resources.GetString("label36.ToolTip"));
            // 
            // tBImportGCIPD
            // 
            resources.ApplyResources(this.tBImportGCIPD, "tBImportGCIPD");
            this.tBImportGCIPD.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCIndPenDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportGCIPD.Name = "tBImportGCIPD";
            this.tBImportGCIPD.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCIndPenDown;
            this.toolTip1.SetToolTip(this.tBImportGCIPD, resources.GetString("tBImportGCIPD.ToolTip"));
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            this.toolTip1.SetToolTip(this.label35, resources.GetString("label35.ToolTip"));
            // 
            // tBImportGCIPU
            // 
            resources.ApplyResources(this.tBImportGCIPU, "tBImportGCIPU");
            this.tBImportGCIPU.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCIndPenUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportGCIPU.Name = "tBImportGCIPU";
            this.tBImportGCIPU.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCIndPenUp;
            this.toolTip1.SetToolTip(this.tBImportGCIPU, resources.GetString("tBImportGCIPU.ToolTip"));
            // 
            // cBImportGCUseIndividual
            // 
            resources.ApplyResources(this.cBImportGCUseIndividual, "cBImportGCUseIndividual");
            this.cBImportGCUseIndividual.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCIndEnable;
            this.cBImportGCUseIndividual.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCIndEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCUseIndividual.Name = "cBImportGCUseIndividual";
            this.toolTip1.SetToolTip(this.cBImportGCUseIndividual, resources.GetString("cBImportGCUseIndividual.ToolTip"));
            this.cBImportGCUseIndividual.UseVisualStyleBackColor = true;
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            this.toolTip1.SetToolTip(this.label34, resources.GetString("label34.ToolTip"));
            // 
            // nUDImportGCSegment
            // 
            resources.ApplyResources(this.nUDImportGCSegment, "nUDImportGCSegment");
            this.nUDImportGCSegment.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCSegment", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCSegment.DecimalPlaces = 2;
            this.nUDImportGCSegment.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDImportGCSegment.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDImportGCSegment.Name = "nUDImportGCSegment";
            this.toolTip1.SetToolTip(this.nUDImportGCSegment, resources.GetString("nUDImportGCSegment.ToolTip"));
            this.nUDImportGCSegment.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCSegment;
            // 
            // cBImportGCNoArcs
            // 
            resources.ApplyResources(this.cBImportGCNoArcs, "cBImportGCNoArcs");
            this.cBImportGCNoArcs.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCNoArcs;
            this.cBImportGCNoArcs.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCNoArcs", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCNoArcs.Name = "cBImportGCNoArcs";
            this.toolTip1.SetToolTip(this.cBImportGCNoArcs, resources.GetString("cBImportGCNoArcs.ToolTip"));
            this.cBImportGCNoArcs.UseVisualStyleBackColor = true;
            // 
            // cBImportGCRelative
            // 
            resources.ApplyResources(this.cBImportGCRelative, "cBImportGCRelative");
            this.cBImportGCRelative.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCRelative;
            this.cBImportGCRelative.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCRelative", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCRelative.Name = "cBImportGCRelative";
            this.toolTip1.SetToolTip(this.cBImportGCRelative, resources.GetString("cBImportGCRelative.ToolTip"));
            this.cBImportGCRelative.UseVisualStyleBackColor = true;
            // 
            // cBImportGCCompress
            // 
            resources.ApplyResources(this.cBImportGCCompress, "cBImportGCCompress");
            this.cBImportGCCompress.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCCompress;
            this.cBImportGCCompress.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCCompress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCCompress.Name = "cBImportGCCompress";
            this.toolTip1.SetToolTip(this.cBImportGCCompress, resources.GetString("cBImportGCCompress.ToolTip"));
            this.cBImportGCCompress.UseVisualStyleBackColor = true;
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
            this.toolTip1.SetToolTip(this.nUDImportDecPlaces, resources.GetString("nUDImportDecPlaces.ToolTip"));
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
            this.toolTip1.SetToolTip(this.label19, resources.GetString("label19.ToolTip"));
            // 
            // cBImportGCTool
            // 
            resources.ApplyResources(this.cBImportGCTool, "cBImportGCTool");
            this.cBImportGCTool.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCTool;
            this.cBImportGCTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCTool.Name = "cBImportGCTool";
            this.toolTip1.SetToolTip(this.cBImportGCTool, resources.GetString("cBImportGCTool.ToolTip"));
            this.cBImportGCTool.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // cBImportGCComments
            // 
            resources.ApplyResources(this.cBImportGCComments, "cBImportGCComments");
            this.cBImportGCComments.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCAddComments;
            this.cBImportGCComments.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCAddComments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCComments.Name = "cBImportGCComments";
            this.toolTip1.SetToolTip(this.cBImportGCComments, resources.GetString("cBImportGCComments.ToolTip"));
            this.cBImportGCComments.UseVisualStyleBackColor = true;
            // 
            // nUDImportGCSSpeed
            // 
            resources.ApplyResources(this.nUDImportGCSSpeed, "nUDImportGCSSpeed");
            this.nUDImportGCSSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCSSpeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCSSpeed.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nUDImportGCSSpeed.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDImportGCSSpeed.Name = "nUDImportGCSSpeed";
            this.toolTip1.SetToolTip(this.nUDImportGCSSpeed, resources.GetString("nUDImportGCSSpeed.ToolTip"));
            this.nUDImportGCSSpeed.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCSSpeed;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            this.toolTip1.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
            // 
            // tBImportGCFooter
            // 
            resources.ApplyResources(this.tBImportGCFooter, "tBImportGCFooter");
            this.tBImportGCFooter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCFooter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportGCFooter.Name = "tBImportGCFooter";
            this.tBImportGCFooter.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCFooter;
            this.toolTip1.SetToolTip(this.tBImportGCFooter, resources.GetString("tBImportGCFooter.ToolTip"));
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // tBImportGCHeader
            // 
            resources.ApplyResources(this.tBImportGCHeader, "tBImportGCHeader");
            this.tBImportGCHeader.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCHeader", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportGCHeader.Name = "tBImportGCHeader";
            this.tBImportGCHeader.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCHeader;
            this.toolTip1.SetToolTip(this.tBImportGCHeader, resources.GetString("tBImportGCHeader.ToolTip"));
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.rBImportGCSpindleCmd2);
            this.groupBox6.Controls.Add(this.rBImportGCSpindleCmd1);
            this.groupBox6.Controls.Add(this.cBImportGCUseSpindle);
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
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.cBImportGCUsePWM);
            this.groupBox4.Controls.Add(this.nUDImportGCDlyDown);
            this.groupBox4.Controls.Add(this.nUDImportGCDlyUp);
            this.groupBox4.Controls.Add(this.nUDImportGCPWMDown);
            this.groupBox4.Controls.Add(this.nUDImportGCPWMUp);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox4, resources.GetString("groupBox4.ToolTip"));
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
            resources.ApplyResources(this.nUDImportGCDlyDown, "nUDImportGCDlyDown");
            this.nUDImportGCDlyDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMDlyDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCDlyDown.DecimalPlaces = 2;
            this.nUDImportGCDlyDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDImportGCDlyDown.Name = "nUDImportGCDlyDown";
            this.toolTip1.SetToolTip(this.nUDImportGCDlyDown, resources.GetString("nUDImportGCDlyDown.ToolTip"));
            this.nUDImportGCDlyDown.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMDlyDown;
            // 
            // nUDImportGCDlyUp
            // 
            resources.ApplyResources(this.nUDImportGCDlyUp, "nUDImportGCDlyUp");
            this.nUDImportGCDlyUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMDlyUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCDlyUp.DecimalPlaces = 2;
            this.nUDImportGCDlyUp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDImportGCDlyUp.Name = "nUDImportGCDlyUp";
            this.toolTip1.SetToolTip(this.nUDImportGCDlyUp, resources.GetString("nUDImportGCDlyUp.ToolTip"));
            this.nUDImportGCDlyUp.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMDlyUp;
            // 
            // nUDImportGCPWMDown
            // 
            resources.ApplyResources(this.nUDImportGCPWMDown, "nUDImportGCPWMDown");
            this.nUDImportGCPWMDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCPWMDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
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
            resources.ApplyResources(this.nUDImportGCPWMUp, "nUDImportGCPWMUp");
            this.nUDImportGCPWMUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCPWMUp.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
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
            this.toolTip1.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // nUDImportGCFeedXY
            // 
            resources.ApplyResources(this.nUDImportGCFeedXY, "nUDImportGCFeedXY");
            this.nUDImportGCFeedXY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCXYFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
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
            this.toolTip1.SetToolTip(this.nUDImportGCFeedXY, resources.GetString("nUDImportGCFeedXY.ToolTip"));
            this.nUDImportGCFeedXY.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCXYFeed;
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.numericUpDown1);
            this.groupBox5.Controls.Add(this.label33);
            this.groupBox5.Controls.Add(this.checkBox2);
            this.groupBox5.Controls.Add(this.cBImportGCUseZ);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.nUDImportGCZUp);
            this.groupBox5.Controls.Add(this.nUDImportGCFeedZ);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.nUDImportGCZDown);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox5, resources.GetString("groupBox5.ToolTip"));
            // 
            // numericUpDown1
            // 
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCZIncrement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericUpDown1.Name = "numericUpDown1";
            this.toolTip1.SetToolTip(this.numericUpDown1, resources.GetString("numericUpDown1.ToolTip"));
            this.numericUpDown1.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZIncrement;
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            this.toolTip1.SetToolTip(this.label33, resources.GetString("label33.ToolTip"));
            // 
            // checkBox2
            // 
            resources.ApplyResources(this.checkBox2, "checkBox2");
            this.checkBox2.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCZIncEnable;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCZIncEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox2.Name = "checkBox2";
            this.toolTip1.SetToolTip(this.checkBox2, resources.GetString("checkBox2.ToolTip"));
            this.checkBox2.UseVisualStyleBackColor = true;
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
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // nUDImportGCZUp
            // 
            resources.ApplyResources(this.nUDImportGCZUp, "nUDImportGCZUp");
            this.nUDImportGCZUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCZUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCZUp.DecimalPlaces = 1;
            this.nUDImportGCZUp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            this.toolTip1.SetToolTip(this.nUDImportGCZUp, resources.GetString("nUDImportGCZUp.ToolTip"));
            this.nUDImportGCZUp.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZUp;
            // 
            // nUDImportGCFeedZ
            // 
            resources.ApplyResources(this.nUDImportGCFeedZ, "nUDImportGCFeedZ");
            this.nUDImportGCFeedZ.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCZFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCFeedZ.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
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
            this.toolTip1.SetToolTip(this.nUDImportGCFeedZ, resources.GetString("nUDImportGCFeedZ.ToolTip"));
            this.nUDImportGCFeedZ.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZFeed;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // nUDImportGCZDown
            // 
            resources.ApplyResources(this.nUDImportGCZDown, "nUDImportGCZDown");
            this.nUDImportGCZDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCZDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCZDown.DecimalPlaces = 1;
            this.nUDImportGCZDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            this.toolTip1.SetToolTip(this.nUDImportGCZDown, resources.GetString("nUDImportGCZDown.ToolTip"));
            this.nUDImportGCZDown.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZDown;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.cBcheckupdate);
            this.tabPage2.Controls.Add(this.groupBox17);
            this.tabPage2.Controls.Add(this.groupBox12);
            this.tabPage2.Controls.Add(this.groupBox10);
            this.tabPage2.Controls.Add(this.groupBox9);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Name = "tabPage2";
            this.toolTip1.SetToolTip(this.tabPage2, resources.GetString("tabPage2.ToolTip"));
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cBcheckupdate
            // 
            resources.ApplyResources(this.cBcheckupdate, "cBcheckupdate");
            this.cBcheckupdate.Checked = global::GRBL_Plotter.Properties.Settings.Default.guiCheckUpdate;
            this.cBcheckupdate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBcheckupdate.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "guiCheckUpdate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBcheckupdate.Name = "cBcheckupdate";
            this.toolTip1.SetToolTip(this.cBcheckupdate, resources.GetString("cBcheckupdate.ToolTip"));
            this.cBcheckupdate.UseVisualStyleBackColor = true;
            // 
            // groupBox17
            // 
            resources.ApplyResources(this.groupBox17, "groupBox17");
            this.groupBox17.Controls.Add(this.cB4thUse);
            this.groupBox17.Controls.Add(this.comboBox1);
            this.groupBox17.Controls.Add(this.label38);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox17, resources.GetString("groupBox17.ToolTip"));
            // 
            // cB4thUse
            // 
            resources.ApplyResources(this.cB4thUse, "cB4thUse");
            this.cB4thUse.Checked = global::GRBL_Plotter.Properties.Settings.Default.ctrl4thUse;
            this.cB4thUse.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "ctrl4thUse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cB4thUse.Name = "cB4thUse";
            this.toolTip1.SetToolTip(this.cB4thUse, resources.GetString("cB4thUse.ToolTip"));
            this.cB4thUse.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.AllowDrop = true;
            this.comboBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrl4thName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4"),
            resources.GetString("comboBox1.Items5")});
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrl4thName;
            this.toolTip1.SetToolTip(this.comboBox1, resources.GetString("comboBox1.ToolTip"));
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            this.toolTip1.SetToolTip(this.label38, resources.GetString("label38.ToolTip"));
            // 
            // groupBox12
            // 
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.Controls.Add(this.groupBox13);
            this.groupBox12.Controls.Add(this.label30);
            this.groupBox12.Controls.Add(this.label29);
            this.groupBox12.Controls.Add(this.nUDRotaryDiameter);
            this.groupBox12.Controls.Add(this.label27);
            this.groupBox12.Controls.Add(this.nUDRotaryScale);
            this.groupBox12.Controls.Add(this.label26);
            this.groupBox12.Controls.Add(this.rBRotaryY);
            this.groupBox12.Controls.Add(this.rBRotaryX);
            this.groupBox12.Controls.Add(this.cBRotarySubstitute);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox12, resources.GetString("groupBox12.ToolTip"));
            // 
            // groupBox13
            // 
            resources.ApplyResources(this.groupBox13, "groupBox13");
            this.groupBox13.Controls.Add(this.label31);
            this.groupBox13.Controls.Add(this.tBRotarySetupOff);
            this.groupBox13.Controls.Add(this.cBRotarySetupApply);
            this.groupBox13.Controls.Add(this.tBRotarySetupOn);
            this.groupBox13.Controls.Add(this.label32);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox13, resources.GetString("groupBox13.ToolTip"));
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            this.toolTip1.SetToolTip(this.label31, resources.GetString("label31.ToolTip"));
            // 
            // tBRotarySetupOff
            // 
            resources.ApplyResources(this.tBRotarySetupOff, "tBRotarySetupOff");
            this.tBRotarySetupOff.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "rotarySubstitutionSetupOff", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBRotarySetupOff.Name = "tBRotarySetupOff";
            this.tBRotarySetupOff.Text = global::GRBL_Plotter.Properties.Settings.Default.rotarySubstitutionSetupOff;
            this.toolTip1.SetToolTip(this.tBRotarySetupOff, resources.GetString("tBRotarySetupOff.ToolTip"));
            // 
            // cBRotarySetupApply
            // 
            resources.ApplyResources(this.cBRotarySetupApply, "cBRotarySetupApply");
            this.cBRotarySetupApply.Checked = global::GRBL_Plotter.Properties.Settings.Default.rotarySubstitutionSetupEnable;
            this.cBRotarySetupApply.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "rotarySubstitutionSetupEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBRotarySetupApply.Name = "cBRotarySetupApply";
            this.toolTip1.SetToolTip(this.cBRotarySetupApply, resources.GetString("cBRotarySetupApply.ToolTip"));
            this.cBRotarySetupApply.UseVisualStyleBackColor = true;
            // 
            // tBRotarySetupOn
            // 
            resources.ApplyResources(this.tBRotarySetupOn, "tBRotarySetupOn");
            this.tBRotarySetupOn.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "rotarySubstitutionSetupOn", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBRotarySetupOn.Name = "tBRotarySetupOn";
            this.tBRotarySetupOn.Text = global::GRBL_Plotter.Properties.Settings.Default.rotarySubstitutionSetupOn;
            this.toolTip1.SetToolTip(this.tBRotarySetupOn, resources.GetString("tBRotarySetupOn.ToolTip"));
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            this.toolTip1.SetToolTip(this.label32, resources.GetString("label32.ToolTip"));
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            this.toolTip1.SetToolTip(this.label30, resources.GetString("label30.ToolTip"));
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            this.toolTip1.SetToolTip(this.label29, resources.GetString("label29.ToolTip"));
            // 
            // nUDRotaryDiameter
            // 
            resources.ApplyResources(this.nUDRotaryDiameter, "nUDRotaryDiameter");
            this.nUDRotaryDiameter.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "rotarySubstitutionDiameter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDRotaryDiameter.DecimalPlaces = 3;
            this.nUDRotaryDiameter.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDRotaryDiameter.Name = "nUDRotaryDiameter";
            this.toolTip1.SetToolTip(this.nUDRotaryDiameter, resources.GetString("nUDRotaryDiameter.ToolTip"));
            this.nUDRotaryDiameter.Value = global::GRBL_Plotter.Properties.Settings.Default.rotarySubstitutionDiameter;
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            this.toolTip1.SetToolTip(this.label27, resources.GetString("label27.ToolTip"));
            // 
            // nUDRotaryScale
            // 
            resources.ApplyResources(this.nUDRotaryScale, "nUDRotaryScale");
            this.nUDRotaryScale.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "rotarySubstitutionScale", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDRotaryScale.DecimalPlaces = 3;
            this.nUDRotaryScale.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDRotaryScale.Name = "nUDRotaryScale";
            this.toolTip1.SetToolTip(this.nUDRotaryScale, resources.GetString("nUDRotaryScale.ToolTip"));
            this.nUDRotaryScale.Value = global::GRBL_Plotter.Properties.Settings.Default.rotarySubstitutionScale;
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            this.toolTip1.SetToolTip(this.label26, resources.GetString("label26.ToolTip"));
            // 
            // rBRotaryY
            // 
            resources.ApplyResources(this.rBRotaryY, "rBRotaryY");
            this.rBRotaryY.Name = "rBRotaryY";
            this.toolTip1.SetToolTip(this.rBRotaryY, resources.GetString("rBRotaryY.ToolTip"));
            this.rBRotaryY.UseVisualStyleBackColor = true;
            // 
            // rBRotaryX
            // 
            resources.ApplyResources(this.rBRotaryX, "rBRotaryX");
            this.rBRotaryX.Name = "rBRotaryX";
            this.toolTip1.SetToolTip(this.rBRotaryX, resources.GetString("rBRotaryX.ToolTip"));
            this.rBRotaryX.UseVisualStyleBackColor = true;
            // 
            // cBRotarySubstitute
            // 
            resources.ApplyResources(this.cBRotarySubstitute, "cBRotarySubstitute");
            this.cBRotarySubstitute.Checked = global::GRBL_Plotter.Properties.Settings.Default.rotarySubstitutionEnable;
            this.cBRotarySubstitute.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "rotarySubstitutionEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBRotarySubstitute.Name = "cBRotarySubstitute";
            this.toolTip1.SetToolTip(this.cBRotarySubstitute, resources.GetString("cBRotarySubstitute.ToolTip"));
            this.cBRotarySubstitute.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Controls.Add(this.groupBox11);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox10, resources.GetString("groupBox10.ToolTip"));
            // 
            // groupBox11
            // 
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Controls.Add(this.rBCtrlReplaceM4);
            this.groupBox11.Controls.Add(this.rBCtrlReplaceM3);
            this.groupBox11.Controls.Add(this.cBCtrlMCmd);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox11, resources.GetString("groupBox11.ToolTip"));
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
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Controls.Add(this.cBSerial2);
            this.groupBox9.Controls.Add(this.cBSerialMinimize);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox9, resources.GetString("groupBox9.ToolTip"));
            // 
            // cBSerial2
            // 
            resources.ApplyResources(this.cBSerial2, "cBSerial2");
            this.cBSerial2.Checked = global::GRBL_Plotter.Properties.Settings.Default.useSerial2;
            this.cBSerial2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "useSerial2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBSerial2.Name = "cBSerial2";
            this.toolTip1.SetToolTip(this.cBSerial2, resources.GetString("cBSerial2.ToolTip"));
            this.cBSerial2.UseVisualStyleBackColor = true;
            // 
            // cBSerialMinimize
            // 
            resources.ApplyResources(this.cBSerialMinimize, "cBSerialMinimize");
            this.cBSerialMinimize.Checked = global::GRBL_Plotter.Properties.Settings.Default.serialMinimize;
            this.cBSerialMinimize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBSerialMinimize.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "serialMinimize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBSerialMinimize.Name = "cBSerialMinimize";
            this.toolTip1.SetToolTip(this.cBSerialMinimize, resources.GetString("cBSerialMinimize.ToolTip"));
            this.cBSerialMinimize.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Controls.Add(this.label18);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptProbe);
            this.groupBox7.Controls.Add(this.label17);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptSelect);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptPut);
            this.groupBox7.Controls.Add(this.label15);
            this.groupBox7.Controls.Add(this.tBToolChangeScriptGet);
            this.groupBox7.Controls.Add(this.cBToolChange);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox7, resources.GetString("groupBox7.ToolTip"));
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            this.toolTip1.SetToolTip(this.label18, resources.GetString("label18.ToolTip"));
            // 
            // tBToolChangeScriptProbe
            // 
            resources.ApplyResources(this.tBToolChangeScriptProbe, "tBToolChangeScriptProbe");
            this.tBToolChangeScriptProbe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptProbe", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptProbe.Name = "tBToolChangeScriptProbe";
            this.tBToolChangeScriptProbe.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptProbe;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptProbe, resources.GetString("tBToolChangeScriptProbe.ToolTip"));
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            this.toolTip1.SetToolTip(this.label17, resources.GetString("label17.ToolTip"));
            // 
            // tBToolChangeScriptSelect
            // 
            resources.ApplyResources(this.tBToolChangeScriptSelect, "tBToolChangeScriptSelect");
            this.tBToolChangeScriptSelect.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptSelect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptSelect.Name = "tBToolChangeScriptSelect";
            this.tBToolChangeScriptSelect.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptSelect;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptSelect, resources.GetString("tBToolChangeScriptSelect.ToolTip"));
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            this.toolTip1.SetToolTip(this.label16, resources.GetString("label16.ToolTip"));
            // 
            // tBToolChangeScriptPut
            // 
            resources.ApplyResources(this.tBToolChangeScriptPut, "tBToolChangeScriptPut");
            this.tBToolChangeScriptPut.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptPut", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptPut.Name = "tBToolChangeScriptPut";
            this.tBToolChangeScriptPut.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptPut;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptPut, resources.GetString("tBToolChangeScriptPut.ToolTip"));
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            this.toolTip1.SetToolTip(this.label15, resources.GetString("label15.ToolTip"));
            // 
            // tBToolChangeScriptGet
            // 
            resources.ApplyResources(this.tBToolChangeScriptGet, "tBToolChangeScriptGet");
            this.tBToolChangeScriptGet.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptGet", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptGet.Name = "tBToolChangeScriptGet";
            this.tBToolChangeScriptGet.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptGet;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptGet, resources.GetString("tBToolChangeScriptGet.ToolTip"));
            // 
            // cBToolChange
            // 
            resources.ApplyResources(this.cBToolChange, "cBToolChange");
            this.cBToolChange.Checked = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolChange;
            this.cBToolChange.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolChange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolChange.Name = "cBToolChange";
            this.toolTip1.SetToolTip(this.cBToolChange, resources.GetString("cBToolChange.ToolTip"));
            this.cBToolChange.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.lblcbnr);
            this.tabPage1.Controls.Add(this.btnChangeDefinition);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.lvCustomButtons);
            this.tabPage1.Name = "tabPage1";
            this.toolTip1.SetToolTip(this.tabPage1, resources.GetString("tabPage1.ToolTip"));
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblcbnr
            // 
            resources.ApplyResources(this.lblcbnr, "lblcbnr");
            this.lblcbnr.Name = "lblcbnr";
            this.toolTip1.SetToolTip(this.lblcbnr, resources.GetString("lblcbnr.ToolTip"));
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
            resources.ApplyResources(this.lvCustomButtons, "lvCustomButtons");
            this.lvCustomButtons.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvCustomButtons.FullRowSelect = true;
            this.lvCustomButtons.GridLines = true;
            this.lvCustomButtons.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvCustomButtons.MultiSelect = false;
            this.lvCustomButtons.Name = "lvCustomButtons";
            this.lvCustomButtons.Scrollable = false;
            this.toolTip1.SetToolTip(this.lvCustomButtons, resources.GetString("lvCustomButtons.ToolTip"));
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
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Controls.Add(this.groupBox8);
            this.tabPage4.Name = "tabPage4";
            this.toolTip1.SetToolTip(this.tabPage4, resources.GetString("tabPage4.ToolTip"));
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.nUDHeightMap);
            this.groupBox1.Controls.Add(this.btnColorHeightMap);
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
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // nUDHeightMap
            // 
            resources.ApplyResources(this.nUDHeightMap, "nUDHeightMap");
            this.nUDHeightMap.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthHeightMap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDHeightMap.DecimalPlaces = 1;
            this.nUDHeightMap.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDHeightMap.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDHeightMap.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDHeightMap.Name = "nUDHeightMap";
            this.toolTip1.SetToolTip(this.nUDHeightMap, resources.GetString("nUDHeightMap.ToolTip"));
            this.nUDHeightMap.Value = global::GRBL_Plotter.Properties.Settings.Default.widthHeightMap;
            // 
            // btnColorHeightMap
            // 
            resources.ApplyResources(this.btnColorHeightMap, "btnColorHeightMap");
            this.btnColorHeightMap.Name = "btnColorHeightMap";
            this.toolTip1.SetToolTip(this.btnColorHeightMap, resources.GetString("btnColorHeightMap.ToolTip"));
            this.btnColorHeightMap.UseVisualStyleBackColor = true;
            // 
            // nUDMarker
            // 
            resources.ApplyResources(this.nUDMarker, "nUDMarker");
            this.nUDMarker.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthMarker", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDMarker.DecimalPlaces = 1;
            this.nUDMarker.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            this.toolTip1.SetToolTip(this.btnColorMarker, resources.GetString("btnColorMarker.ToolTip"));
            this.btnColorMarker.UseVisualStyleBackColor = true;
            // 
            // nUDTool
            // 
            resources.ApplyResources(this.nUDTool, "nUDTool");
            this.nUDTool.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDTool.DecimalPlaces = 1;
            this.nUDTool.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            resources.ApplyResources(this.nUDPenDown, "nUDPenDown");
            this.nUDPenDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthPenDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPenDown.DecimalPlaces = 1;
            this.nUDPenDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            resources.ApplyResources(this.nUDPenUp, "nUDPenUp");
            this.nUDPenUp.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthPenUp", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDPenUp.DecimalPlaces = 1;
            this.nUDPenUp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            resources.ApplyResources(this.nUDRuler, "nUDRuler");
            this.nUDRuler.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "widthRuler", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDRuler.DecimalPlaces = 1;
            this.nUDRuler.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
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
            this.toolTip1.SetToolTip(this.btnColorTool, resources.GetString("btnColorTool.ToolTip"));
            this.btnColorTool.UseVisualStyleBackColor = true;
            // 
            // btnColorPenDown
            // 
            resources.ApplyResources(this.btnColorPenDown, "btnColorPenDown");
            this.btnColorPenDown.Name = "btnColorPenDown";
            this.toolTip1.SetToolTip(this.btnColorPenDown, resources.GetString("btnColorPenDown.ToolTip"));
            this.btnColorPenDown.UseVisualStyleBackColor = true;
            // 
            // btnColorPenUp
            // 
            resources.ApplyResources(this.btnColorPenUp, "btnColorPenUp");
            this.btnColorPenUp.Name = "btnColorPenUp";
            this.toolTip1.SetToolTip(this.btnColorPenUp, resources.GetString("btnColorPenUp.ToolTip"));
            this.btnColorPenUp.UseVisualStyleBackColor = true;
            // 
            // btnColorRuler
            // 
            resources.ApplyResources(this.btnColorRuler, "btnColorRuler");
            this.btnColorRuler.Name = "btnColorRuler";
            this.toolTip1.SetToolTip(this.btnColorRuler, resources.GetString("btnColorRuler.ToolTip"));
            this.btnColorRuler.UseVisualStyleBackColor = true;
            // 
            // btnColorBackground
            // 
            resources.ApplyResources(this.btnColorBackground, "btnColorBackground");
            this.btnColorBackground.Name = "btnColorBackground";
            this.toolTip1.SetToolTip(this.btnColorBackground, resources.GetString("btnColorBackground.ToolTip"));
            this.btnColorBackground.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            resources.ApplyResources(this.groupBox8, "groupBox8");
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
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox8, resources.GetString("groupBox8.ToolTip"));
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
            this.toolTip1.SetToolTip(this.label28, resources.GetString("label28.ToolTip"));
            // 
            // nUDJoyZSpeed5
            // 
            resources.ApplyResources(this.nUDJoyZSpeed5, "nUDJoyZSpeed5");
            this.nUDJoyZSpeed5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyZSpeed5, resources.GetString("nUDJoyZSpeed5.ToolTip"));
            this.nUDJoyZSpeed5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed5;
            // 
            // nUDJoyZSpeed4
            // 
            resources.ApplyResources(this.nUDJoyZSpeed4, "nUDJoyZSpeed4");
            this.nUDJoyZSpeed4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyZSpeed4, resources.GetString("nUDJoyZSpeed4.ToolTip"));
            this.nUDJoyZSpeed4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed4;
            // 
            // nUDJoyZSpeed3
            // 
            resources.ApplyResources(this.nUDJoyZSpeed3, "nUDJoyZSpeed3");
            this.nUDJoyZSpeed3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyZSpeed3, resources.GetString("nUDJoyZSpeed3.ToolTip"));
            this.nUDJoyZSpeed3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed3;
            // 
            // nUDJoyZSpeed2
            // 
            resources.ApplyResources(this.nUDJoyZSpeed2, "nUDJoyZSpeed2");
            this.nUDJoyZSpeed2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyZSpeed2, resources.GetString("nUDJoyZSpeed2.ToolTip"));
            this.nUDJoyZSpeed2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed2;
            // 
            // nUDJoyZSpeed1
            // 
            resources.ApplyResources(this.nUDJoyZSpeed1, "nUDJoyZSpeed1");
            this.nUDJoyZSpeed1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyZSpeed1, resources.GetString("nUDJoyZSpeed1.ToolTip"));
            this.nUDJoyZSpeed1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed1;
            // 
            // nUDJoyZStep5
            // 
            resources.ApplyResources(this.nUDJoyZStep5, "nUDJoyZStep5");
            this.nUDJoyZStep5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep5.DecimalPlaces = 3;
            this.nUDJoyZStep5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep5.Name = "nUDJoyZStep5";
            this.toolTip1.SetToolTip(this.nUDJoyZStep5, resources.GetString("nUDJoyZStep5.ToolTip"));
            this.nUDJoyZStep5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep5;
            // 
            // nUDJoyZStep4
            // 
            resources.ApplyResources(this.nUDJoyZStep4, "nUDJoyZStep4");
            this.nUDJoyZStep4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep4.DecimalPlaces = 3;
            this.nUDJoyZStep4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep4.Name = "nUDJoyZStep4";
            this.toolTip1.SetToolTip(this.nUDJoyZStep4, resources.GetString("nUDJoyZStep4.ToolTip"));
            this.nUDJoyZStep4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep4;
            // 
            // nUDJoyZStep3
            // 
            resources.ApplyResources(this.nUDJoyZStep3, "nUDJoyZStep3");
            this.nUDJoyZStep3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep3.DecimalPlaces = 3;
            this.nUDJoyZStep3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep3.Name = "nUDJoyZStep3";
            this.toolTip1.SetToolTip(this.nUDJoyZStep3, resources.GetString("nUDJoyZStep3.ToolTip"));
            this.nUDJoyZStep3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep3;
            // 
            // nUDJoyZStep2
            // 
            resources.ApplyResources(this.nUDJoyZStep2, "nUDJoyZStep2");
            this.nUDJoyZStep2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep2.DecimalPlaces = 3;
            this.nUDJoyZStep2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep2.Name = "nUDJoyZStep2";
            this.toolTip1.SetToolTip(this.nUDJoyZStep2, resources.GetString("nUDJoyZStep2.ToolTip"));
            this.nUDJoyZStep2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep2;
            // 
            // nUDJoyZStep1
            // 
            resources.ApplyResources(this.nUDJoyZStep1, "nUDJoyZStep1");
            this.nUDJoyZStep1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep1.DecimalPlaces = 3;
            this.nUDJoyZStep1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep1.Name = "nUDJoyZStep1";
            this.toolTip1.SetToolTip(this.nUDJoyZStep1, resources.GetString("nUDJoyZStep1.ToolTip"));
            this.nUDJoyZStep1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep1;
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            this.toolTip1.SetToolTip(this.label25, resources.GetString("label25.ToolTip"));
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            this.toolTip1.SetToolTip(this.label24, resources.GetString("label24.ToolTip"));
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            this.toolTip1.SetToolTip(this.label23, resources.GetString("label23.ToolTip"));
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            this.toolTip1.SetToolTip(this.label22, resources.GetString("label22.ToolTip"));
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            this.toolTip1.SetToolTip(this.label21, resources.GetString("label21.ToolTip"));
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            this.toolTip1.SetToolTip(this.label20, resources.GetString("label20.ToolTip"));
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
            resources.ApplyResources(this.nUDJoyXYSpeed5, "nUDJoyXYSpeed5");
            this.nUDJoyXYSpeed5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyXYSpeed5, resources.GetString("nUDJoyXYSpeed5.ToolTip"));
            this.nUDJoyXYSpeed5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed5;
            // 
            // nUDJoyXYSpeed4
            // 
            resources.ApplyResources(this.nUDJoyXYSpeed4, "nUDJoyXYSpeed4");
            this.nUDJoyXYSpeed4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyXYSpeed4, resources.GetString("nUDJoyXYSpeed4.ToolTip"));
            this.nUDJoyXYSpeed4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed4;
            // 
            // nUDJoyXYSpeed3
            // 
            resources.ApplyResources(this.nUDJoyXYSpeed3, "nUDJoyXYSpeed3");
            this.nUDJoyXYSpeed3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyXYSpeed3, resources.GetString("nUDJoyXYSpeed3.ToolTip"));
            this.nUDJoyXYSpeed3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed3;
            // 
            // nUDJoyXYSpeed2
            // 
            resources.ApplyResources(this.nUDJoyXYSpeed2, "nUDJoyXYSpeed2");
            this.nUDJoyXYSpeed2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyXYSpeed2, resources.GetString("nUDJoyXYSpeed2.ToolTip"));
            this.nUDJoyXYSpeed2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed2;
            // 
            // nUDJoyXYSpeed1
            // 
            resources.ApplyResources(this.nUDJoyXYSpeed1, "nUDJoyXYSpeed1");
            this.nUDJoyXYSpeed1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.toolTip1.SetToolTip(this.nUDJoyXYSpeed1, resources.GetString("nUDJoyXYSpeed1.ToolTip"));
            this.nUDJoyXYSpeed1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed1;
            // 
            // nUDJoyXYStep5
            // 
            resources.ApplyResources(this.nUDJoyXYStep5, "nUDJoyXYStep5");
            this.nUDJoyXYStep5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep5.DecimalPlaces = 3;
            this.nUDJoyXYStep5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep5.Name = "nUDJoyXYStep5";
            this.toolTip1.SetToolTip(this.nUDJoyXYStep5, resources.GetString("nUDJoyXYStep5.ToolTip"));
            this.nUDJoyXYStep5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep5;
            // 
            // nUDJoyXYStep4
            // 
            resources.ApplyResources(this.nUDJoyXYStep4, "nUDJoyXYStep4");
            this.nUDJoyXYStep4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep4.DecimalPlaces = 3;
            this.nUDJoyXYStep4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep4.Name = "nUDJoyXYStep4";
            this.toolTip1.SetToolTip(this.nUDJoyXYStep4, resources.GetString("nUDJoyXYStep4.ToolTip"));
            this.nUDJoyXYStep4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep4;
            // 
            // nUDJoyXYStep3
            // 
            resources.ApplyResources(this.nUDJoyXYStep3, "nUDJoyXYStep3");
            this.nUDJoyXYStep3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep3.DecimalPlaces = 3;
            this.nUDJoyXYStep3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep3.Name = "nUDJoyXYStep3";
            this.toolTip1.SetToolTip(this.nUDJoyXYStep3, resources.GetString("nUDJoyXYStep3.ToolTip"));
            this.nUDJoyXYStep3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep3;
            // 
            // nUDJoyXYStep2
            // 
            resources.ApplyResources(this.nUDJoyXYStep2, "nUDJoyXYStep2");
            this.nUDJoyXYStep2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep2.DecimalPlaces = 3;
            this.nUDJoyXYStep2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep2.Name = "nUDJoyXYStep2";
            this.toolTip1.SetToolTip(this.nUDJoyXYStep2, resources.GetString("nUDJoyXYStep2.ToolTip"));
            this.nUDJoyXYStep2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep2;
            // 
            // nUDJoyXYStep1
            // 
            resources.ApplyResources(this.nUDJoyXYStep1, "nUDJoyXYStep1");
            this.nUDJoyXYStep1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep1.DecimalPlaces = 3;
            this.nUDJoyXYStep1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep1.Name = "nUDJoyXYStep1";
            this.toolTip1.SetToolTip(this.nUDJoyXYStep1, resources.GetString("nUDJoyXYStep1.ToolTip"));
            this.nUDJoyXYStep1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep1;
            // 
            // tabPage5
            // 
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Controls.Add(this.groupBox19);
            this.tabPage5.Controls.Add(this.groupBox18);
            this.tabPage5.Name = "tabPage5";
            this.toolTip1.SetToolTip(this.tabPage5, resources.GetString("tabPage5.ToolTip"));
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox19
            // 
            resources.ApplyResources(this.groupBox19, "groupBox19");
            this.groupBox19.Controls.Add(this.linkLabel8);
            this.groupBox19.Controls.Add(this.linkLabel7);
            this.groupBox19.Name = "groupBox19";
            this.groupBox19.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox19, resources.GetString("groupBox19.ToolTip"));
            // 
            // linkLabel8
            // 
            resources.ApplyResources(this.linkLabel8, "linkLabel8");
            this.linkLabel8.Name = "linkLabel8";
            this.linkLabel8.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel8, resources.GetString("linkLabel8.ToolTip"));
            this.linkLabel8.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel8_LinkClicked);
            // 
            // linkLabel7
            // 
            resources.ApplyResources(this.linkLabel7, "linkLabel7");
            this.linkLabel7.Name = "linkLabel7";
            this.linkLabel7.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel7, resources.GetString("linkLabel7.ToolTip"));
            this.linkLabel7.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel7_LinkClicked);
            // 
            // groupBox18
            // 
            resources.ApplyResources(this.groupBox18, "groupBox18");
            this.groupBox18.Controls.Add(this.label39);
            this.groupBox18.Controls.Add(this.linkLabel6);
            this.groupBox18.Controls.Add(this.linkLabel5);
            this.groupBox18.Controls.Add(this.linkLabel4);
            this.groupBox18.Controls.Add(this.linkLabel3);
            this.groupBox18.Controls.Add(this.linkLabel2);
            this.groupBox18.Controls.Add(this.linkLabel1);
            this.groupBox18.Name = "groupBox18";
            this.groupBox18.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox18, resources.GetString("groupBox18.ToolTip"));
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            this.toolTip1.SetToolTip(this.label39, resources.GetString("label39.ToolTip"));
            // 
            // linkLabel6
            // 
            resources.ApplyResources(this.linkLabel6, "linkLabel6");
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel6, resources.GetString("linkLabel6.ToolTip"));
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel6_LinkClicked);
            // 
            // linkLabel5
            // 
            resources.ApplyResources(this.linkLabel5, "linkLabel5");
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel5, resources.GetString("linkLabel5.ToolTip"));
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel5_LinkClicked);
            // 
            // linkLabel4
            // 
            resources.ApplyResources(this.linkLabel4, "linkLabel4");
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel4, resources.GetString("linkLabel4.ToolTip"));
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
            // 
            // linkLabel3
            // 
            resources.ApplyResources(this.linkLabel3, "linkLabel3");
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel3, resources.GetString("linkLabel3.ToolTip"));
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // linkLabel2
            // 
            resources.ApplyResources(this.linkLabel2, "linkLabel2");
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel2, resources.GetString("linkLabel2.ToolTip"));
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel1, resources.GetString("linkLabel1.ToolTip"));
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
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
            this.Name = "ControlSetupForm";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSVGScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportReduce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportSVGSegemnts)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSegment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportDecPlaces)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSSpeed)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedXY)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRotaryDiameter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRotaryScale)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeightMap)).EndInit();
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
            this.tabPage5.ResumeLayout(false);
            this.groupBox19.ResumeLayout(false);
            this.groupBox19.PerformLayout();
            this.groupBox18.ResumeLayout(false);
            this.groupBox18.PerformLayout();
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
        private System.Windows.Forms.CheckBox cBImportSVGSort;
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
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.RadioButton rBRotaryY;
        private System.Windows.Forms.RadioButton rBRotaryX;
        private System.Windows.Forms.CheckBox cBRotarySubstitute;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.NumericUpDown nUDRotaryScale;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.NumericUpDown nUDRotaryDiameter;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox tBRotarySetupOff;
        private System.Windows.Forms.CheckBox cBRotarySetupApply;
        private System.Windows.Forms.TextBox tBRotarySetupOn;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox cBImportGCCompress;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.RadioButton rBImportDXFSize2;
        private System.Windows.Forms.RadioButton rBImportDXFSize1;
        private System.Windows.Forms.CheckBox cBImportGCNoArcs;
        private System.Windows.Forms.CheckBox cBImportGCRelative;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.NumericUpDown nUDImportGCSegment;
        private System.Windows.Forms.NumericUpDown nUDHeightMap;
        private System.Windows.Forms.Button btnColorHeightMap;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox tBImportGCIPD;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox tBImportGCIPU;
        private System.Windows.Forms.CheckBox cBImportGCUseIndividual;
        private System.Windows.Forms.CheckBox cBImportSVGNodesOnly;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.GroupBox groupBox17;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox cB4thUse;
        private System.Windows.Forms.CheckBox cBcheckupdate;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox19;
        private System.Windows.Forms.GroupBox groupBox18;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel8;
        private System.Windows.Forms.LinkLabel linkLabel7;
        private System.Windows.Forms.Label label39;
    }
}