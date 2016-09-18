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
    partial class SetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnResizeForm = new System.Windows.Forms.Button();
            this.btnReloadFile = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nUDImportDecPlaces = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblcbnr = new System.Windows.Forms.Label();
            this.btnChangeDefinition = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lvCustomButtons = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.btnJoyZCalc = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.btnJoyXYCalc = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnColorMarker = new System.Windows.Forms.Button();
            this.btnColorTool = new System.Windows.Forms.Button();
            this.btnColorPenDown = new System.Windows.Forms.Button();
            this.btnColorPenUp = new System.Windows.Forms.Button();
            this.btnColorRuler = new System.Windows.Forms.Button();
            this.btnColorBackground = new System.Windows.Forms.Button();
            this.btnApplyChangings = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tBImportSVGPalette = new System.Windows.Forms.TextBox();
            this.cBImportSVGPath = new System.Windows.Forms.CheckBox();
            this.cBImportSVGPauseP = new System.Windows.Forms.CheckBox();
            this.cBImportSVGTool = new System.Windows.Forms.CheckBox();
            this.cBImportSVGPauseE = new System.Windows.Forms.CheckBox();
            this.nUDSVGScale = new System.Windows.Forms.NumericUpDown();
            this.nUDImportReduce = new System.Windows.Forms.NumericUpDown();
            this.cBImportSVGReduce = new System.Windows.Forms.CheckBox();
            this.cBImportSVGComments = new System.Windows.Forms.CheckBox();
            this.cBImportSVGResize = new System.Windows.Forms.CheckBox();
            this.nUDImportSVGSegemnts = new System.Windows.Forms.NumericUpDown();
            this.cBImportGCTool = new System.Windows.Forms.CheckBox();
            this.cBImportGCComments = new System.Windows.Forms.CheckBox();
            this.tBImportGCFooter = new System.Windows.Forms.TextBox();
            this.tBImportGCHeader = new System.Windows.Forms.TextBox();
            this.cBImportGCUseSpindle = new System.Windows.Forms.CheckBox();
            this.nUDImportGCSSpeed = new System.Windows.Forms.NumericUpDown();
            this.cBImportGCUsePWM = new System.Windows.Forms.CheckBox();
            this.nUDImportGCDlyDown = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCDlyUp = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCPWMDown = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCPWMUp = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCFeedXY = new System.Windows.Forms.NumericUpDown();
            this.cBImportGCUseZ = new System.Windows.Forms.CheckBox();
            this.nUDImportGCZUp = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCFeedZ = new System.Windows.Forms.NumericUpDown();
            this.nUDImportGCZDown = new System.Windows.Forms.NumericUpDown();
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
            this.cBSerial2 = new System.Windows.Forms.CheckBox();
            this.tBToolChangeScriptProbe = new System.Windows.Forms.TextBox();
            this.tBToolChangeScriptSelect = new System.Windows.Forms.TextBox();
            this.tBToolChangeScriptPut = new System.Windows.Forms.TextBox();
            this.tBToolChangeScriptGet = new System.Windows.Forms.TextBox();
            this.cBToolChange = new System.Windows.Forms.CheckBox();
            this.cBSerialMinimize = new System.Windows.Forms.CheckBox();
            this.nUDMarker = new System.Windows.Forms.NumericUpDown();
            this.nUDTool = new System.Windows.Forms.NumericUpDown();
            this.nUDPenDown = new System.Windows.Forms.NumericUpDown();
            this.nUDPenUp = new System.Windows.Forms.NumericUpDown();
            this.nUDRuler = new System.Windows.Forms.NumericUpDown();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportDecPlaces)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSVGScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportReduce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportSVGSegemnts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedXY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.nUDMarker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDTool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRuler)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(712, 286);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.expandForm_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnResizeForm);
            this.tabPage3.Controls.Add(this.btnReloadFile);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(704, 260);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "SVG Import";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnResizeForm
            // 
            this.btnResizeForm.BackColor = System.Drawing.Color.Yellow;
            this.btnResizeForm.Location = new System.Drawing.Point(105, 231);
            this.btnResizeForm.Name = "btnResizeForm";
            this.btnResizeForm.Size = new System.Drawing.Size(98, 23);
            this.btnResizeForm.TabIndex = 11;
            this.btnResizeForm.Text = "expand window >";
            this.btnResizeForm.UseVisualStyleBackColor = false;
            this.btnResizeForm.Click += new System.EventHandler(this.btnResizeForm_Click);
            // 
            // btnReloadFile
            // 
            this.btnReloadFile.Location = new System.Drawing.Point(3, 231);
            this.btnReloadFile.Name = "btnReloadFile";
            this.btnReloadFile.Size = new System.Drawing.Size(94, 23);
            this.btnReloadFile.TabIndex = 10;
            this.btnReloadFile.Text = "Reload SVG file";
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
            this.groupBox3.Location = new System.Drawing.Point(3, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 222);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "SVG Conversion";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(165, 60);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(29, 13);
            this.label14.TabIndex = 9;
            this.label14.Text = "units";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(165, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "units";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bezier line Segments";
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
            this.groupBox2.Location = new System.Drawing.Point(209, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(489, 222);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "G-Code creation";
            // 
            // nUDImportDecPlaces
            // 
            this.nUDImportDecPlaces.Location = new System.Drawing.Point(91, 24);
            this.nUDImportDecPlaces.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nUDImportDecPlaces.Name = "nUDImportDecPlaces";
            this.nUDImportDecPlaces.Size = new System.Drawing.Size(31, 20);
            this.nUDImportDecPlaces.TabIndex = 20;
            this.nUDImportDecPlaces.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDImportDecPlaces.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nUDImportDecPlaces.ValueChanged += new System.EventHandler(this.nUDImportDecPlaces_ValueChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 26);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(79, 13);
            this.label19.TabIndex = 19;
            this.label19.Text = "Decimal places";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(5, 117);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(76, 13);
            this.label13.TabIndex = 16;
            this.label13.Text = "G-Code Footer";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(5, 79);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "G-Code Header";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cBImportGCUseSpindle);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.nUDImportGCSSpeed);
            this.groupBox6.Location = new System.Drawing.Point(340, 14);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(132, 62);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Spindle On/Off";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Spindle Spd";
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
            this.groupBox4.Location = new System.Drawing.Point(339, 89);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(133, 120);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Servo Control";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 98);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Delay after";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Delay after";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Pen Down";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Pen Up";
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
            this.groupBox5.Location = new System.Drawing.Point(173, 14);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(161, 195);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Z-Axis";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Z-Height save";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Z-Height engrave";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Z Feedrate";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(66, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "XY Feedrate";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblcbnr);
            this.tabPage1.Controls.Add(this.btnChangeDefinition);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.lvCustomButtons);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(704, 260);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Custom Buttons";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblcbnr
            // 
            this.lblcbnr.AutoSize = true;
            this.lblcbnr.Location = new System.Drawing.Point(6, 176);
            this.lblcbnr.Name = "lblcbnr";
            this.lblcbnr.Size = new System.Drawing.Size(13, 13);
            this.lblcbnr.TabIndex = 7;
            this.lblcbnr.Text = "0";
            // 
            // btnChangeDefinition
            // 
            this.btnChangeDefinition.Location = new System.Drawing.Point(29, 199);
            this.btnChangeDefinition.Name = "btnChangeDefinition";
            this.btnChangeDefinition.Size = new System.Drawing.Size(190, 23);
            this.btnChangeDefinition.TabIndex = 6;
            this.btnChangeDefinition.Text = "Change Button definitions in list";
            this.toolTip1.SetToolTip(this.btnChangeDefinition, "Press button to update values in list. and save the settings.");
            this.btnChangeDefinition.UseVisualStyleBackColor = true;
            this.btnChangeDefinition.Click += new System.EventHandler(this.btnChangeDefinition_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(119, 173);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(579, 20);
            this.textBox2.TabIndex = 4;
            this.toolTip1.SetToolTip(this.textBox2, "GCode to send. Seperate lines with \';\'.");
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(29, 173);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(90, 20);
            this.textBox1.TabIndex = 3;
            this.toolTip1.SetToolTip(this.textBox1, "Define text of button");
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
            this.lvCustomButtons.Location = new System.Drawing.Point(3, 6);
            this.lvCustomButtons.MultiSelect = false;
            this.lvCustomButtons.Name = "lvCustomButtons";
            this.lvCustomButtons.Scrollable = false;
            this.lvCustomButtons.Size = new System.Drawing.Size(698, 165);
            this.lvCustomButtons.TabIndex = 1;
            this.lvCustomButtons.UseCompatibleStateImageBehavior = false;
            this.lvCustomButtons.View = System.Windows.Forms.View.Details;
            this.lvCustomButtons.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvCustomButtons_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Nr.";
            this.columnHeader1.Width = 26;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Label";
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "G-Code";
            this.columnHeader3.Width = 575;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox8);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.cBSerial2);
            this.tabPage2.Controls.Add(this.cBSerialMinimize);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(704, 260);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Misc / Colors";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            this.groupBox8.Location = new System.Drawing.Point(238, 6);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(282, 248);
            this.groupBox8.TabIndex = 5;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Joystick setup";
            // 
            // btnJoyZCalc
            // 
            this.btnJoyZCalc.Location = new System.Drawing.Point(165, 150);
            this.btnJoyZCalc.Name = "btnJoyZCalc";
            this.btnJoyZCalc.Size = new System.Drawing.Size(111, 23);
            this.btnJoyZCalc.TabIndex = 29;
            this.btnJoyZCalc.Text = "Calc. best speed";
            this.toolTip1.SetToolTip(this.btnJoyZCalc, "Calculate Speeds to reach step width within repetition rate of 0.5 seconds");
            this.btnJoyZCalc.UseVisualStyleBackColor = true;
            this.btnJoyZCalc.Click += new System.EventHandler(this.btnJoyZCalc_Click);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(166, 25);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(104, 13);
            this.label28.TabIndex = 28;
            this.label28.Text = "Z Step width & Speed";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(49, 25);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(111, 13);
            this.label25.TabIndex = 16;
            this.label25.Text = "XY Step width & Speed";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 127);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(34, 13);
            this.label24.TabIndex = 15;
            this.label24.Text = "Pos 5";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 106);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(34, 13);
            this.label23.TabIndex = 14;
            this.label23.Text = "Pos 4";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 85);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(34, 13);
            this.label22.TabIndex = 13;
            this.label22.Text = "Pos 3";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 64);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(34, 13);
            this.label21.TabIndex = 12;
            this.label21.Text = "Pos 2";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 43);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(34, 13);
            this.label20.TabIndex = 11;
            this.label20.Text = "Pos 1";
            // 
            // btnJoyXYCalc
            // 
            this.btnJoyXYCalc.Location = new System.Drawing.Point(46, 150);
            this.btnJoyXYCalc.Name = "btnJoyXYCalc";
            this.btnJoyXYCalc.Size = new System.Drawing.Size(111, 23);
            this.btnJoyXYCalc.TabIndex = 10;
            this.btnJoyXYCalc.Text = "Calc. best speed";
            this.toolTip1.SetToolTip(this.btnJoyXYCalc, "Calculate Speeds to reach step width within repetition rate of 0.5 seconds");
            this.btnJoyXYCalc.UseVisualStyleBackColor = true;
            this.btnJoyXYCalc.Click += new System.EventHandler(this.btnJoyXYCalc_Click);
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
            this.groupBox7.Location = new System.Drawing.Point(3, 54);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(229, 200);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Tool change";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 158);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(141, 13);
            this.label18.TabIndex = 8;
            this.label18.Text = "Path to script to PROBE tool";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 78);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(145, 13);
            this.label17.TabIndex = 6;
            this.label17.Text = "Path to script to SELECT tool";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 39);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(150, 13);
            this.label16.TabIndex = 4;
            this.label16.Text = "Path to script to REMOVE tool";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 116);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(146, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Path to script to PICK UP tool";
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
            this.groupBox1.Location = new System.Drawing.Point(526, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(175, 195);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drawing colors";
            // 
            // btnColorMarker
            // 
            this.btnColorMarker.Location = new System.Drawing.Point(6, 164);
            this.btnColorMarker.Name = "btnColorMarker";
            this.btnColorMarker.Size = new System.Drawing.Size(100, 23);
            this.btnColorMarker.TabIndex = 9;
            this.btnColorMarker.Text = "Marker";
            this.btnColorMarker.UseVisualStyleBackColor = true;
            this.btnColorMarker.Click += new System.EventHandler(this.btnColorMarker_Click);
            // 
            // btnColorTool
            // 
            this.btnColorTool.Location = new System.Drawing.Point(6, 135);
            this.btnColorTool.Name = "btnColorTool";
            this.btnColorTool.Size = new System.Drawing.Size(100, 23);
            this.btnColorTool.TabIndex = 4;
            this.btnColorTool.Text = "Tool";
            this.btnColorTool.UseVisualStyleBackColor = true;
            this.btnColorTool.Click += new System.EventHandler(this.btnColorTool_Click);
            // 
            // btnColorPenDown
            // 
            this.btnColorPenDown.Location = new System.Drawing.Point(6, 106);
            this.btnColorPenDown.Name = "btnColorPenDown";
            this.btnColorPenDown.Size = new System.Drawing.Size(100, 23);
            this.btnColorPenDown.TabIndex = 3;
            this.btnColorPenDown.Text = "Pen Down";
            this.btnColorPenDown.UseVisualStyleBackColor = true;
            this.btnColorPenDown.Click += new System.EventHandler(this.btnColorPenDown_Click);
            // 
            // btnColorPenUp
            // 
            this.btnColorPenUp.Location = new System.Drawing.Point(6, 77);
            this.btnColorPenUp.Name = "btnColorPenUp";
            this.btnColorPenUp.Size = new System.Drawing.Size(100, 23);
            this.btnColorPenUp.TabIndex = 2;
            this.btnColorPenUp.Text = "Pen Up";
            this.btnColorPenUp.UseVisualStyleBackColor = true;
            this.btnColorPenUp.Click += new System.EventHandler(this.btnColorPenUp_Click);
            // 
            // btnColorRuler
            // 
            this.btnColorRuler.Location = new System.Drawing.Point(6, 48);
            this.btnColorRuler.Name = "btnColorRuler";
            this.btnColorRuler.Size = new System.Drawing.Size(100, 23);
            this.btnColorRuler.TabIndex = 1;
            this.btnColorRuler.Text = "Ruler";
            this.btnColorRuler.UseVisualStyleBackColor = true;
            this.btnColorRuler.Click += new System.EventHandler(this.btnColorRuler_Click);
            // 
            // btnColorBackground
            // 
            this.btnColorBackground.Location = new System.Drawing.Point(6, 19);
            this.btnColorBackground.Name = "btnColorBackground";
            this.btnColorBackground.Size = new System.Drawing.Size(100, 23);
            this.btnColorBackground.TabIndex = 0;
            this.btnColorBackground.Text = "Background";
            this.btnColorBackground.UseVisualStyleBackColor = true;
            this.btnColorBackground.Click += new System.EventHandler(this.btnColorBackground_Click);
            // 
            // btnApplyChangings
            // 
            this.btnApplyChangings.Location = new System.Drawing.Point(550, 286);
            this.btnApplyChangings.Name = "btnApplyChangings";
            this.btnApplyChangings.Size = new System.Drawing.Size(162, 23);
            this.btnApplyChangings.TabIndex = 7;
            this.btnApplyChangings.Text = "Apply changings to buttons";
            this.toolTip1.SetToolTip(this.btnApplyChangings, "Press button to apply new settings to buttons");
            this.btnApplyChangings.UseVisualStyleBackColor = true;
            this.btnApplyChangings.Click += new System.EventHandler(this.btnApplyChangings_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGToolSort;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGToolSort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(8, 175);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(145, 17);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Text = "Sort objects by pen index";
            this.toolTip1.SetToolTip(this.checkBox1, "Calculate pen number by using closest color in given GIMP palette.\r\nPen number ca" +
        "n then be used to select tool number or Z-deepth");
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tBImportSVGPalette
            // 
            this.tBImportSVGPalette.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importPalette", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportSVGPalette.Location = new System.Drawing.Point(8, 150);
            this.tBImportSVGPalette.Name = "tBImportSVGPalette";
            this.tBImportSVGPalette.Size = new System.Drawing.Size(186, 20);
            this.tBImportSVGPalette.TabIndex = 18;
            this.tBImportSVGPalette.Text = global::GRBL_Plotter.Properties.Settings.Default.importPalette;
            this.toolTip1.SetToolTip(this.tBImportSVGPalette, resources.GetString("tBImportSVGPalette.ToolTip"));
            // 
            // cBImportSVGPath
            // 
            this.cBImportSVGPath.AutoSize = true;
            this.cBImportSVGPath.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPathExtend;
            this.cBImportSVGPath.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGPath.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPathExtend", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPath.Location = new System.Drawing.Point(8, 75);
            this.cBImportSVGPath.Name = "cBImportSVGPath";
            this.cBImportSVGPath.Size = new System.Drawing.Size(86, 17);
            this.cBImportSVGPath.TabIndex = 13;
            this.cBImportSVGPath.Text = "Path overlap";
            this.toolTip1.SetToolTip(this.cBImportSVGPath, "Finally move to 1st and 2nd point of path (for cutter)");
            this.cBImportSVGPath.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGPauseP
            // 
            this.cBImportSVGPauseP.AutoSize = true;
            this.cBImportSVGPauseP.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPausePenDown;
            this.cBImportSVGPauseP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGPauseP.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPausePenDown", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPauseP.Location = new System.Drawing.Point(8, 115);
            this.cBImportSVGPauseP.Name = "cBImportSVGPauseP";
            this.cBImportSVGPauseP.Size = new System.Drawing.Size(190, 17);
            this.cBImportSVGPauseP.TabIndex = 12;
            this.cBImportSVGPauseP.Text = "Pause (M0) before each pen down";
            this.cBImportSVGPauseP.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGTool
            // 
            this.cBImportSVGTool.AutoSize = true;
            this.cBImportSVGTool.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGToolColor;
            this.cBImportSVGTool.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGToolColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGTool.Location = new System.Drawing.Point(8, 135);
            this.cBImportSVGTool.Name = "cBImportSVGTool";
            this.cBImportSVGTool.Size = new System.Drawing.Size(179, 17);
            this.cBImportSVGTool.TabIndex = 14;
            this.cBImportSVGTool.Text = "Get pen index from color palette:";
            this.toolTip1.SetToolTip(this.cBImportSVGTool, "Calculate pen number by using closest color in given GIMP palette.\r\nPen number ca" +
        "n then be used to select tool number or Z-deepth");
            this.cBImportSVGTool.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGPauseE
            // 
            this.cBImportSVGPauseE.AutoSize = true;
            this.cBImportSVGPauseE.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGPauseElement;
            this.cBImportSVGPauseE.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGPauseE.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGPauseElement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGPauseE.Location = new System.Drawing.Point(8, 95);
            this.cBImportSVGPauseE.Name = "cBImportSVGPauseE";
            this.cBImportSVGPauseE.Size = new System.Drawing.Size(164, 17);
            this.cBImportSVGPauseE.TabIndex = 11;
            this.cBImportSVGPauseE.Text = "Pause (M0) before each path";
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
            this.nUDSVGScale.Location = new System.Drawing.Point(116, 34);
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
            this.nUDSVGScale.Size = new System.Drawing.Size(48, 20);
            this.nUDSVGScale.TabIndex = 10;
            this.nUDSVGScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDSVGScale.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGMaxSize;
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
            this.nUDImportReduce.Location = new System.Drawing.Point(116, 54);
            this.nUDImportReduce.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDImportReduce.Name = "nUDImportReduce";
            this.nUDImportReduce.Size = new System.Drawing.Size(48, 20);
            this.nUDImportReduce.TabIndex = 8;
            this.nUDImportReduce.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDImportReduce.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGReduceLimit;
            // 
            // cBImportSVGReduce
            // 
            this.cBImportSVGReduce.AutoSize = true;
            this.cBImportSVGReduce.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGReduce;
            this.cBImportSVGReduce.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGReduce.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGReduce", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGReduce.Location = new System.Drawing.Point(8, 55);
            this.cBImportSVGReduce.Name = "cBImportSVGReduce";
            this.cBImportSVGReduce.Size = new System.Drawing.Size(110, 17);
            this.cBImportSVGReduce.TabIndex = 7;
            this.cBImportSVGReduce.Text = "Remove Moves <";
            this.cBImportSVGReduce.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGComments
            // 
            this.cBImportSVGComments.AutoSize = true;
            this.cBImportSVGComments.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGAddComments;
            this.cBImportSVGComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGComments.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGAddComments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGComments.Location = new System.Drawing.Point(8, 199);
            this.cBImportSVGComments.Name = "cBImportSVGComments";
            this.cBImportSVGComments.Size = new System.Drawing.Size(148, 17);
            this.cBImportSVGComments.TabIndex = 6;
            this.cBImportSVGComments.Text = "Additional SVG comments";
            this.cBImportSVGComments.UseVisualStyleBackColor = true;
            // 
            // cBImportSVGResize
            // 
            this.cBImportSVGResize.AutoSize = true;
            this.cBImportSVGResize.Checked = global::GRBL_Plotter.Properties.Settings.Default.importSVGRezise;
            this.cBImportSVGResize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportSVGResize.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importSVGRezise", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportSVGResize.Location = new System.Drawing.Point(8, 37);
            this.cBImportSVGResize.Name = "cBImportSVGResize";
            this.cBImportSVGResize.Size = new System.Drawing.Size(95, 17);
            this.cBImportSVGResize.TabIndex = 4;
            this.cBImportSVGResize.Text = "Rezise to max.";
            this.cBImportSVGResize.UseVisualStyleBackColor = true;
            // 
            // nUDImportSVGSegemnts
            // 
            this.nUDImportSVGSegemnts.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importSVGBezier", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportSVGSegemnts.Location = new System.Drawing.Point(124, 14);
            this.nUDImportSVGSegemnts.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nUDImportSVGSegemnts.Name = "nUDImportSVGSegemnts";
            this.nUDImportSVGSegemnts.Size = new System.Drawing.Size(40, 20);
            this.nUDImportSVGSegemnts.TabIndex = 1;
            this.nUDImportSVGSegemnts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDImportSVGSegemnts.Value = global::GRBL_Plotter.Properties.Settings.Default.importSVGBezier;
            // 
            // cBImportGCTool
            // 
            this.cBImportGCTool.AutoSize = true;
            this.cBImportGCTool.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCTool;
            this.cBImportGCTool.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportGCTool.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCTool.Location = new System.Drawing.Point(6, 175);
            this.cBImportGCTool.Name = "cBImportGCTool";
            this.cBImportGCTool.Size = new System.Drawing.Size(158, 17);
            this.cBImportGCTool.TabIndex = 17;
            this.cBImportGCTool.Text = "Add Tool Change command";
            this.cBImportGCTool.UseVisualStyleBackColor = true;
            // 
            // cBImportGCComments
            // 
            this.cBImportGCComments.AutoSize = true;
            this.cBImportGCComments.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCAddComments;
            this.cBImportGCComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportGCComments.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCAddComments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCComments.Location = new System.Drawing.Point(6, 199);
            this.cBImportGCComments.Name = "cBImportGCComments";
            this.cBImportGCComments.Size = new System.Drawing.Size(159, 17);
            this.cBImportGCComments.TabIndex = 7;
            this.cBImportGCComments.Text = "Additional GCode comments";
            this.cBImportGCComments.UseVisualStyleBackColor = true;
            // 
            // tBImportGCFooter
            // 
            this.tBImportGCFooter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCFooter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportGCFooter.Location = new System.Drawing.Point(8, 130);
            this.tBImportGCFooter.Name = "tBImportGCFooter";
            this.tBImportGCFooter.Size = new System.Drawing.Size(159, 20);
            this.tBImportGCFooter.TabIndex = 15;
            this.tBImportGCFooter.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCFooter;
            // 
            // tBImportGCHeader
            // 
            this.tBImportGCHeader.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "importGCHeader", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBImportGCHeader.Location = new System.Drawing.Point(8, 92);
            this.tBImportGCHeader.Name = "tBImportGCHeader";
            this.tBImportGCHeader.Size = new System.Drawing.Size(159, 20);
            this.tBImportGCHeader.TabIndex = 13;
            this.tBImportGCHeader.Text = global::GRBL_Plotter.Properties.Settings.Default.importGCHeader;
            this.toolTip1.SetToolTip(this.tBImportGCHeader, "Example: G17 G21 G40 G54 G80 G90 G94");
            // 
            // cBImportGCUseSpindle
            // 
            this.cBImportGCUseSpindle.AutoSize = true;
            this.cBImportGCUseSpindle.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCSpindleToggle;
            this.cBImportGCUseSpindle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCSpindleToggle", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCUseSpindle.Location = new System.Drawing.Point(9, 14);
            this.cBImportGCUseSpindle.Name = "cBImportGCUseSpindle";
            this.cBImportGCUseSpindle.Size = new System.Drawing.Size(97, 17);
            this.cBImportGCUseSpindle.TabIndex = 0;
            this.cBImportGCUseSpindle.Text = "Spindle On/Off";
            this.toolTip1.SetToolTip(this.cBImportGCUseSpindle, "Switch Spindle off on Pen up and on on Pen down (for laser engraver)");
            this.cBImportGCUseSpindle.UseVisualStyleBackColor = true;
            // 
            // nUDImportGCSSpeed
            // 
            this.nUDImportGCSSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCSSpeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCSSpeed.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nUDImportGCSSpeed.Location = new System.Drawing.Point(73, 36);
            this.nUDImportGCSSpeed.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nUDImportGCSSpeed.Name = "nUDImportGCSSpeed";
            this.nUDImportGCSSpeed.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCSSpeed.TabIndex = 10;
            this.nUDImportGCSSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDImportGCSSpeed.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCSSpeed;
            // 
            // cBImportGCUsePWM
            // 
            this.cBImportGCUsePWM.AutoSize = true;
            this.cBImportGCUsePWM.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMEnable;
            this.cBImportGCUsePWM.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCPWMEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCUsePWM.Location = new System.Drawing.Point(9, 13);
            this.cBImportGCUsePWM.Name = "cBImportGCUsePWM";
            this.cBImportGCUsePWM.Size = new System.Drawing.Size(127, 17);
            this.cBImportGCUsePWM.TabIndex = 15;
            this.cBImportGCUsePWM.Text = "Spindle Spd as PWM";
            this.toolTip1.SetToolTip(this.cBImportGCUsePWM, "Control RC-Servo via spindle speed (special GRBL-version needed to generate 1ms -" +
        " 2ms PWM output)");
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
            this.nUDImportGCDlyDown.Location = new System.Drawing.Point(69, 96);
            this.nUDImportGCDlyDown.Name = "nUDImportGCDlyDown";
            this.nUDImportGCDlyDown.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCDlyDown.TabIndex = 14;
            this.nUDImportGCDlyDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDImportGCDlyDown, "Insert G4: delay in seconds before next command");
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
            this.nUDImportGCDlyUp.Location = new System.Drawing.Point(69, 56);
            this.nUDImportGCDlyUp.Name = "nUDImportGCDlyUp";
            this.nUDImportGCDlyUp.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCDlyUp.TabIndex = 13;
            this.nUDImportGCDlyUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDImportGCDlyUp, "Insert G4: delay in seconds before next command");
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
            this.nUDImportGCPWMDown.Location = new System.Drawing.Point(69, 76);
            this.nUDImportGCPWMDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDImportGCPWMDown.Name = "nUDImportGCPWMDown";
            this.nUDImportGCPWMDown.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCPWMDown.TabIndex = 12;
            this.nUDImportGCPWMDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDImportGCPWMDown, "Spindle speed for pen down");
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
            this.nUDImportGCPWMUp.Location = new System.Drawing.Point(69, 36);
            this.nUDImportGCPWMUp.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDImportGCPWMUp.Name = "nUDImportGCPWMUp";
            this.nUDImportGCPWMUp.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCPWMUp.TabIndex = 11;
            this.nUDImportGCPWMUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDImportGCPWMUp, "Spindle speed for pen up");
            this.nUDImportGCPWMUp.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCPWMUp;
            // 
            // nUDImportGCFeedXY
            // 
            this.nUDImportGCFeedXY.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "importGCXYFeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDImportGCFeedXY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nUDImportGCFeedXY.Location = new System.Drawing.Point(69, 53);
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
            this.nUDImportGCFeedXY.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCFeedXY.TabIndex = 11;
            this.nUDImportGCFeedXY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDImportGCFeedXY.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCXYFeed;
            // 
            // cBImportGCUseZ
            // 
            this.cBImportGCUseZ.AutoSize = true;
            this.cBImportGCUseZ.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCZEnable;
            this.cBImportGCUseZ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBImportGCUseZ.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCZEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGCUseZ.Location = new System.Drawing.Point(6, 13);
            this.cBImportGCUseZ.Name = "cBImportGCUseZ";
            this.cBImportGCUseZ.Size = new System.Drawing.Size(77, 17);
            this.cBImportGCUseZ.TabIndex = 12;
            this.cBImportGCUseZ.Text = "Use Z-Axis";
            this.toolTip1.SetToolTip(this.cBImportGCUseZ, "Insert Z-Axis moves into G-Code");
            this.cBImportGCUseZ.UseVisualStyleBackColor = true;
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
            this.nUDImportGCZUp.Location = new System.Drawing.Point(99, 56);
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
            this.nUDImportGCZUp.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCZUp.TabIndex = 8;
            this.nUDImportGCZUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.nUDImportGCFeedZ.Location = new System.Drawing.Point(99, 36);
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
            this.nUDImportGCFeedZ.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCFeedZ.TabIndex = 9;
            this.nUDImportGCFeedZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDImportGCFeedZ.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZFeed;
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
            this.nUDImportGCZDown.Location = new System.Drawing.Point(99, 76);
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
            this.nUDImportGCZDown.Size = new System.Drawing.Size(53, 20);
            this.nUDImportGCZDown.TabIndex = 7;
            this.nUDImportGCZDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDImportGCZDown.Value = global::GRBL_Plotter.Properties.Settings.Default.importGCZDown;
            // 
            // nUDJoyZSpeed5
            // 
            this.nUDJoyZSpeed5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZSpeed5.Location = new System.Drawing.Point(229, 125);
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
            this.nUDJoyZSpeed5.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyZSpeed5.TabIndex = 27;
            this.nUDJoyZSpeed5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZSpeed5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed5;
            // 
            // nUDJoyZSpeed4
            // 
            this.nUDJoyZSpeed4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZSpeed4.Location = new System.Drawing.Point(229, 104);
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
            this.nUDJoyZSpeed4.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyZSpeed4.TabIndex = 26;
            this.nUDJoyZSpeed4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZSpeed4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed4;
            // 
            // nUDJoyZSpeed3
            // 
            this.nUDJoyZSpeed3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZSpeed3.Location = new System.Drawing.Point(229, 83);
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
            this.nUDJoyZSpeed3.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyZSpeed3.TabIndex = 25;
            this.nUDJoyZSpeed3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZSpeed3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed3;
            // 
            // nUDJoyZSpeed2
            // 
            this.nUDJoyZSpeed2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZSpeed2.Location = new System.Drawing.Point(229, 62);
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
            this.nUDJoyZSpeed2.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyZSpeed2.TabIndex = 24;
            this.nUDJoyZSpeed2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZSpeed2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed2;
            // 
            // nUDJoyZSpeed1
            // 
            this.nUDJoyZSpeed1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZSpeed1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZSpeed1.Location = new System.Drawing.Point(229, 41);
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
            this.nUDJoyZSpeed1.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyZSpeed1.TabIndex = 23;
            this.nUDJoyZSpeed1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZSpeed1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZSpeed1;
            // 
            // nUDJoyZStep5
            // 
            this.nUDJoyZStep5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep5.DecimalPlaces = 3;
            this.nUDJoyZStep5.Location = new System.Drawing.Point(166, 125);
            this.nUDJoyZStep5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep5.Name = "nUDJoyZStep5";
            this.nUDJoyZStep5.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyZStep5.TabIndex = 22;
            this.nUDJoyZStep5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZStep5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep5;
            // 
            // nUDJoyZStep4
            // 
            this.nUDJoyZStep4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep4.DecimalPlaces = 3;
            this.nUDJoyZStep4.Location = new System.Drawing.Point(166, 104);
            this.nUDJoyZStep4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep4.Name = "nUDJoyZStep4";
            this.nUDJoyZStep4.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyZStep4.TabIndex = 21;
            this.nUDJoyZStep4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZStep4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep4;
            // 
            // nUDJoyZStep3
            // 
            this.nUDJoyZStep3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep3.DecimalPlaces = 3;
            this.nUDJoyZStep3.Location = new System.Drawing.Point(166, 83);
            this.nUDJoyZStep3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep3.Name = "nUDJoyZStep3";
            this.nUDJoyZStep3.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyZStep3.TabIndex = 20;
            this.nUDJoyZStep3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZStep3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep3;
            // 
            // nUDJoyZStep2
            // 
            this.nUDJoyZStep2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep2.DecimalPlaces = 3;
            this.nUDJoyZStep2.Location = new System.Drawing.Point(166, 62);
            this.nUDJoyZStep2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep2.Name = "nUDJoyZStep2";
            this.nUDJoyZStep2.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyZStep2.TabIndex = 19;
            this.nUDJoyZStep2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZStep2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep2;
            // 
            // nUDJoyZStep1
            // 
            this.nUDJoyZStep1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyZStep1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyZStep1.DecimalPlaces = 3;
            this.nUDJoyZStep1.Location = new System.Drawing.Point(166, 41);
            this.nUDJoyZStep1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyZStep1.Name = "nUDJoyZStep1";
            this.nUDJoyZStep1.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyZStep1.TabIndex = 18;
            this.nUDJoyZStep1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyZStep1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyZStep1;
            // 
            // nUDJoyXYSpeed5
            // 
            this.nUDJoyXYSpeed5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYSpeed5.Location = new System.Drawing.Point(109, 125);
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
            this.nUDJoyXYSpeed5.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyXYSpeed5.TabIndex = 9;
            this.nUDJoyXYSpeed5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYSpeed5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed5;
            // 
            // nUDJoyXYSpeed4
            // 
            this.nUDJoyXYSpeed4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYSpeed4.Location = new System.Drawing.Point(109, 104);
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
            this.nUDJoyXYSpeed4.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyXYSpeed4.TabIndex = 8;
            this.nUDJoyXYSpeed4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYSpeed4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed4;
            // 
            // nUDJoyXYSpeed3
            // 
            this.nUDJoyXYSpeed3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYSpeed3.Location = new System.Drawing.Point(109, 83);
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
            this.nUDJoyXYSpeed3.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyXYSpeed3.TabIndex = 7;
            this.nUDJoyXYSpeed3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYSpeed3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed3;
            // 
            // nUDJoyXYSpeed2
            // 
            this.nUDJoyXYSpeed2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYSpeed2.Location = new System.Drawing.Point(109, 62);
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
            this.nUDJoyXYSpeed2.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyXYSpeed2.TabIndex = 6;
            this.nUDJoyXYSpeed2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYSpeed2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed2;
            // 
            // nUDJoyXYSpeed1
            // 
            this.nUDJoyXYSpeed1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYSpeed1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYSpeed1.Location = new System.Drawing.Point(109, 41);
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
            this.nUDJoyXYSpeed1.Size = new System.Drawing.Size(48, 20);
            this.nUDJoyXYSpeed1.TabIndex = 5;
            this.nUDJoyXYSpeed1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYSpeed1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYSpeed1;
            // 
            // nUDJoyXYStep5
            // 
            this.nUDJoyXYStep5.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep5", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep5.DecimalPlaces = 3;
            this.nUDJoyXYStep5.Location = new System.Drawing.Point(46, 125);
            this.nUDJoyXYStep5.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep5.Name = "nUDJoyXYStep5";
            this.nUDJoyXYStep5.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyXYStep5.TabIndex = 4;
            this.nUDJoyXYStep5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYStep5.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep5;
            // 
            // nUDJoyXYStep4
            // 
            this.nUDJoyXYStep4.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep4", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep4.DecimalPlaces = 3;
            this.nUDJoyXYStep4.Location = new System.Drawing.Point(46, 104);
            this.nUDJoyXYStep4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep4.Name = "nUDJoyXYStep4";
            this.nUDJoyXYStep4.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyXYStep4.TabIndex = 3;
            this.nUDJoyXYStep4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYStep4.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep4;
            // 
            // nUDJoyXYStep3
            // 
            this.nUDJoyXYStep3.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep3", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep3.DecimalPlaces = 3;
            this.nUDJoyXYStep3.Location = new System.Drawing.Point(46, 83);
            this.nUDJoyXYStep3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep3.Name = "nUDJoyXYStep3";
            this.nUDJoyXYStep3.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyXYStep3.TabIndex = 2;
            this.nUDJoyXYStep3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYStep3.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep3;
            // 
            // nUDJoyXYStep2
            // 
            this.nUDJoyXYStep2.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep2.DecimalPlaces = 3;
            this.nUDJoyXYStep2.Location = new System.Drawing.Point(46, 62);
            this.nUDJoyXYStep2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep2.Name = "nUDJoyXYStep2";
            this.nUDJoyXYStep2.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyXYStep2.TabIndex = 1;
            this.nUDJoyXYStep2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYStep2.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep2;
            // 
            // nUDJoyXYStep1
            // 
            this.nUDJoyXYStep1.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "joyXYStep1", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDJoyXYStep1.DecimalPlaces = 3;
            this.nUDJoyXYStep1.Location = new System.Drawing.Point(46, 41);
            this.nUDJoyXYStep1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nUDJoyXYStep1.Name = "nUDJoyXYStep1";
            this.nUDJoyXYStep1.Size = new System.Drawing.Size(60, 20);
            this.nUDJoyXYStep1.TabIndex = 0;
            this.nUDJoyXYStep1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDJoyXYStep1.Value = global::GRBL_Plotter.Properties.Settings.Default.joyXYStep1;
            // 
            // cBSerial2
            // 
            this.cBSerial2.AutoSize = true;
            this.cBSerial2.Checked = global::GRBL_Plotter.Properties.Settings.Default.useSerial2;
            this.cBSerial2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBSerial2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "useSerial2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBSerial2.Location = new System.Drawing.Point(6, 6);
            this.cBSerial2.Name = "cBSerial2";
            this.cBSerial2.Size = new System.Drawing.Size(191, 17);
            this.cBSerial2.TabIndex = 4;
            this.cBSerial2.Text = "Use 2nd serial port (restart needed)";
            this.toolTip1.SetToolTip(this.cBSerial2, "Opens a 2nd COM port to control a 2nd GRBL. Usefull for tool changer or 4th axis." +
        "");
            this.cBSerial2.UseVisualStyleBackColor = true;
            // 
            // tBToolChangeScriptProbe
            // 
            this.tBToolChangeScriptProbe.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptProbe", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptProbe.Location = new System.Drawing.Point(6, 174);
            this.tBToolChangeScriptProbe.Name = "tBToolChangeScriptProbe";
            this.tBToolChangeScriptProbe.Size = new System.Drawing.Size(217, 20);
            this.tBToolChangeScriptProbe.TabIndex = 7;
            this.tBToolChangeScriptProbe.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptProbe;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptProbe, "4th probe tool length");
            // 
            // tBToolChangeScriptSelect
            // 
            this.tBToolChangeScriptSelect.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptSelect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptSelect.Location = new System.Drawing.Point(6, 94);
            this.tBToolChangeScriptSelect.Name = "tBToolChangeScriptSelect";
            this.tBToolChangeScriptSelect.Size = new System.Drawing.Size(217, 20);
            this.tBToolChangeScriptSelect.TabIndex = 5;
            this.tBToolChangeScriptSelect.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptSelect;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptSelect, "2nd bring new tool in front of spindle");
            // 
            // tBToolChangeScriptPut
            // 
            this.tBToolChangeScriptPut.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptPut", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptPut.Location = new System.Drawing.Point(6, 55);
            this.tBToolChangeScriptPut.Name = "tBToolChangeScriptPut";
            this.tBToolChangeScriptPut.Size = new System.Drawing.Size(217, 20);
            this.tBToolChangeScriptPut.TabIndex = 3;
            this.tBToolChangeScriptPut.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptPut;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptPut, "First the old tool needs to be removed from spidle (except the first time)");
            // 
            // tBToolChangeScriptGet
            // 
            this.tBToolChangeScriptGet.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolScriptGet", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tBToolChangeScriptGet.Location = new System.Drawing.Point(6, 132);
            this.tBToolChangeScriptGet.Name = "tBToolChangeScriptGet";
            this.tBToolChangeScriptGet.Size = new System.Drawing.Size(217, 20);
            this.tBToolChangeScriptGet.TabIndex = 1;
            this.tBToolChangeScriptGet.Text = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolScriptGet;
            this.toolTip1.SetToolTip(this.tBToolChangeScriptGet, "3rd pick up new tool");
            // 
            // cBToolChange
            // 
            this.cBToolChange.AutoSize = true;
            this.cBToolChange.Checked = global::GRBL_Plotter.Properties.Settings.Default.ctrlToolChange;
            this.cBToolChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBToolChange.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "ctrlToolChange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolChange.Location = new System.Drawing.Point(6, 19);
            this.cBToolChange.Name = "cBToolChange";
            this.cBToolChange.Size = new System.Drawing.Size(121, 17);
            this.cBToolChange.TabIndex = 0;
            this.cBToolChange.Text = "Perform tool change";
            this.cBToolChange.UseVisualStyleBackColor = true;
            // 
            // cBSerialMinimize
            // 
            this.cBSerialMinimize.AutoSize = true;
            this.cBSerialMinimize.Checked = global::GRBL_Plotter.Properties.Settings.Default.serialMinimize;
            this.cBSerialMinimize.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "serialMinimize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBSerialMinimize.Location = new System.Drawing.Point(6, 25);
            this.cBSerialMinimize.Name = "cBSerialMinimize";
            this.cBSerialMinimize.Size = new System.Drawing.Size(201, 17);
            this.cBSerialMinimize.TabIndex = 2;
            this.cBSerialMinimize.Text = "Minimize serial forms after connection";
            this.toolTip1.SetToolTip(this.cBSerialMinimize, "Will minimize the serial windows after sucessful connection");
            this.cBSerialMinimize.UseVisualStyleBackColor = true;
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
            this.nUDMarker.Location = new System.Drawing.Point(112, 167);
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
            this.nUDMarker.Size = new System.Drawing.Size(51, 20);
            this.nUDMarker.TabIndex = 10;
            this.nUDMarker.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDMarker, "Pen width");
            this.nUDMarker.Value = global::GRBL_Plotter.Properties.Settings.Default.widthMarker;
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
            this.nUDTool.Location = new System.Drawing.Point(112, 138);
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
            this.nUDTool.Size = new System.Drawing.Size(51, 20);
            this.nUDTool.TabIndex = 8;
            this.nUDTool.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDTool, "Pen width");
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
            this.nUDPenDown.Location = new System.Drawing.Point(112, 109);
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
            this.nUDPenDown.Size = new System.Drawing.Size(51, 20);
            this.nUDPenDown.TabIndex = 7;
            this.nUDPenDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDPenDown, "Pen width");
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
            this.nUDPenUp.Location = new System.Drawing.Point(112, 80);
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
            this.nUDPenUp.Size = new System.Drawing.Size(51, 20);
            this.nUDPenUp.TabIndex = 6;
            this.nUDPenUp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDPenUp, "Pen width");
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
            this.nUDRuler.Location = new System.Drawing.Point(112, 51);
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
            this.nUDRuler.Size = new System.Drawing.Size(51, 20);
            this.nUDRuler.TabIndex = 5;
            this.nUDRuler.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.nUDRuler, "Pen width");
            this.nUDRuler.Value = global::GRBL_Plotter.Properties.Settings.Default.widthRuler;
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 286);
            this.Controls.Add(this.btnApplyChangings);
            this.Controls.Add(this.tabControl1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::GRBL_Plotter.Properties.Settings.Default, "locationSetForm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::GRBL_Plotter.Properties.Settings.Default.locationSetForm;
            this.MaximumSize = new System.Drawing.Size(730, 350);
            this.MinimumSize = new System.Drawing.Size(230, 325);
            this.Name = "SetupForm";
            this.Text = "Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportDecPlaces)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nUDSVGScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportReduce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportSVGSegemnts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCSSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCDlyUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCPWMUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedXY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCFeedZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDImportGCZDown)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.nUDMarker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDTool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDPenUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRuler)).EndInit();
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
        private System.Windows.Forms.Button btnColorBackground;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnColorTool;
        private System.Windows.Forms.Button btnColorPenDown;
        private System.Windows.Forms.Button btnColorPenUp;
        private System.Windows.Forms.Button btnColorRuler;
        private System.Windows.Forms.NumericUpDown nUDRuler;
        private System.Windows.Forms.NumericUpDown nUDTool;
        private System.Windows.Forms.NumericUpDown nUDPenDown;
        private System.Windows.Forms.NumericUpDown nUDPenUp;
        private System.Windows.Forms.NumericUpDown nUDMarker;
        private System.Windows.Forms.Button btnColorMarker;
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
        private System.Windows.Forms.GroupBox groupBox8;
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
        private System.Windows.Forms.NumericUpDown nUDImportDecPlaces;
    }
}