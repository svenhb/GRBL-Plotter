/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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
namespace GrblPlotter
{
    partial class GCodeFromText
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
//				image.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeFromText));
            this.label1 = new System.Windows.Forms.Label();
            this.cBFont = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.GbFont1 = new System.Windows.Forms.GroupBox();
            this.nUDLineBreak = new System.Windows.Forms.NumericUpDown();
            this.nUDFontLine = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nUDFontDistance = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.nUDFontSize = new System.Windows.Forms.NumericUpDown();
            this.RbAlign3 = new System.Windows.Forms.RadioButton();
            this.RbAlign2 = new System.Windows.Forms.RadioButton();
            this.RbAlign1 = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cBConnectLetter = new System.Windows.Forms.CheckBox();
            this.cBTool = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cBPauseLine = new System.Windows.Forms.CheckBox();
            this.cBPauseWord = new System.Windows.Forms.CheckBox();
            this.cBPauseChar = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.RbFont2 = new System.Windows.Forms.RadioButton();
            this.RbFont1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.GbFont2 = new System.Windows.Forms.GroupBox();
            this.BtnSelectFont = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.LblFont = new System.Windows.Forms.Label();
            this.LblInfoFont = new System.Windows.Forms.Label();
            this.LblInfoSize = new System.Windows.Forms.Label();
            this.LblSize = new System.Windows.Forms.Label();
            this.LblWidth = new System.Windows.Forms.Label();
            this.LblHeight = new System.Windows.Forms.Label();
            this.LblInfoWidth = new System.Windows.Forms.Label();
            this.LblInfoHeight = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.NUDWidth = new System.Windows.Forms.NumericUpDown();
            this.NUDHeight = new System.Windows.Forms.NumericUpDown();
            this.BtnSetWidth = new System.Windows.Forms.Button();
            this.BtnSetHeight = new System.Windows.Forms.Button();
            this.CbWordWrap = new System.Windows.Forms.CheckBox();
            this.cBToolTable = new System.Windows.Forms.CheckBox();
            this.CbHatchFill = new System.Windows.Forms.CheckBox();
            this.CbInsertCode = new System.Windows.Forms.CheckBox();
            this.tBText = new System.Windows.Forms.TextBox();
            this.cBLineBreak = new System.Windows.Forms.CheckBox();
            this.CbOutline = new System.Windows.Forms.CheckBox();
            this.GbFont1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLineBreak)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.GbFont2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // cBFont
            // 
            this.cBFont.FormattingEnabled = true;
            resources.ApplyResources(this.cBFont, "cBFont");
            this.cBFont.Name = "cBFont";
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // GbFont1
            // 
            this.GbFont1.Controls.Add(this.nUDLineBreak);
            this.GbFont1.Controls.Add(this.cBLineBreak);
            this.GbFont1.Controls.Add(this.nUDFontLine);
            this.GbFont1.Controls.Add(this.label11);
            this.GbFont1.Controls.Add(this.nUDFontDistance);
            this.GbFont1.Controls.Add(this.label10);
            this.GbFont1.Controls.Add(this.nUDFontSize);
            this.GbFont1.Controls.Add(this.label1);
            this.GbFont1.Controls.Add(this.cBFont);
            resources.ApplyResources(this.GbFont1, "GbFont1");
            this.GbFont1.Name = "GbFont1";
            this.GbFont1.TabStop = false;
            // 
            // nUDLineBreak
            // 
            this.nUDLineBreak.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDLineBreak, "nUDLineBreak");
            this.nUDLineBreak.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.nUDLineBreak.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDLineBreak.Name = "nUDLineBreak";
            this.nUDLineBreak.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // nUDFontLine
            // 
            this.nUDFontLine.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDFontLine, "nUDFontLine");
            this.nUDFontLine.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.nUDFontLine.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDFontLine.Name = "nUDFontLine";
            this.nUDFontLine.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // nUDFontDistance
            // 
            this.nUDFontDistance.DecimalPlaces = 1;
            this.nUDFontDistance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDFontDistance, "nUDFontDistance");
            this.nUDFontDistance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nUDFontDistance.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nUDFontDistance.Name = "nUDFontDistance";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // nUDFontSize
            // 
            this.nUDFontSize.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDFontSize, "nUDFontSize");
            this.nUDFontSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nUDFontSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDFontSize.Name = "nUDFontSize";
            this.nUDFontSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDFontSize.ValueChanged += new System.EventHandler(this.NudFontSize_ValueChanged);
            // 
            // RbAlign3
            // 
            resources.ApplyResources(this.RbAlign3, "RbAlign3");
            this.RbAlign3.Name = "RbAlign3";
            this.RbAlign3.TabStop = true;
            this.RbAlign3.UseVisualStyleBackColor = true;
            this.RbAlign3.CheckedChanged += new System.EventHandler(this.RbAlign1_CheckedChanged);
            // 
            // RbAlign2
            // 
            resources.ApplyResources(this.RbAlign2, "RbAlign2");
            this.RbAlign2.Name = "RbAlign2";
            this.RbAlign2.TabStop = true;
            this.RbAlign2.UseVisualStyleBackColor = true;
            this.RbAlign2.CheckedChanged += new System.EventHandler(this.RbAlign1_CheckedChanged);
            // 
            // RbAlign1
            // 
            resources.ApplyResources(this.RbAlign1, "RbAlign1");
            this.RbAlign1.Checked = true;
            this.RbAlign1.Name = "RbAlign1";
            this.RbAlign1.TabStop = true;
            this.RbAlign1.UseVisualStyleBackColor = true;
            this.RbAlign1.CheckedChanged += new System.EventHandler(this.RbAlign1_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CbOutline);
            this.groupBox1.Controls.Add(this.cBToolTable);
            this.groupBox1.Controls.Add(this.CbHatchFill);
            this.groupBox1.Controls.Add(this.cBConnectLetter);
            this.groupBox1.Controls.Add(this.cBTool);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cBPauseLine);
            this.groupBox1.Controls.Add(this.cBPauseWord);
            this.groupBox1.Controls.Add(this.cBPauseChar);
            this.groupBox1.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cBConnectLetter
            // 
            resources.ApplyResources(this.cBConnectLetter, "cBConnectLetter");
            this.cBConnectLetter.Name = "cBConnectLetter";
            this.toolTip1.SetToolTip(this.cBConnectLetter, resources.GetString("cBConnectLetter.ToolTip"));
            this.cBConnectLetter.UseVisualStyleBackColor = true;
            // 
            // cBTool
            // 
            this.cBTool.FormattingEnabled = true;
            resources.ApplyResources(this.cBTool, "cBTool");
            this.cBTool.Name = "cBTool";
            this.cBTool.SelectedIndexChanged += new System.EventHandler(this.CBTool_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cBPauseLine
            // 
            resources.ApplyResources(this.cBPauseLine, "cBPauseLine");
            this.cBPauseLine.Name = "cBPauseLine";
            this.cBPauseLine.UseVisualStyleBackColor = true;
            // 
            // cBPauseWord
            // 
            resources.ApplyResources(this.cBPauseWord, "cBPauseWord");
            this.cBPauseWord.Name = "cBPauseWord";
            this.cBPauseWord.UseVisualStyleBackColor = true;
            // 
            // cBPauseChar
            // 
            resources.ApplyResources(this.cBPauseChar, "cBPauseChar");
            this.cBPauseChar.Name = "cBPauseChar";
            this.cBPauseChar.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.CbWordWrap);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.RbFont2);
            this.tabPage1.Controls.Add(this.RbFont1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.CbInsertCode);
            this.tabPage1.Controls.Add(this.btnApply);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.tBText);
            this.tabPage1.Controls.Add(this.GbFont2);
            this.tabPage1.Controls.Add(this.GbFont1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // RbFont2
            // 
            resources.ApplyResources(this.RbFont2, "RbFont2");
            this.RbFont2.Name = "RbFont2";
            this.RbFont2.UseVisualStyleBackColor = true;
            this.RbFont2.CheckedChanged += new System.EventHandler(this.RbFont1_CheckedChanged);
            // 
            // RbFont1
            // 
            resources.ApplyResources(this.RbFont1, "RbFont1");
            this.RbFont1.Checked = true;
            this.RbFont1.Name = "RbFont1";
            this.RbFont1.TabStop = true;
            this.RbFont1.UseVisualStyleBackColor = true;
            this.RbFont1.CheckedChanged += new System.EventHandler(this.RbFont1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RbAlign2);
            this.groupBox2.Controls.Add(this.RbAlign3);
            this.groupBox2.Controls.Add(this.RbAlign1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // GbFont2
            // 
            this.GbFont2.Controls.Add(this.BtnSetHeight);
            this.GbFont2.Controls.Add(this.BtnSetWidth);
            this.GbFont2.Controls.Add(this.NUDHeight);
            this.GbFont2.Controls.Add(this.NUDWidth);
            this.GbFont2.Controls.Add(this.linkLabel1);
            this.GbFont2.Controls.Add(this.LblInfoHeight);
            this.GbFont2.Controls.Add(this.LblInfoWidth);
            this.GbFont2.Controls.Add(this.LblHeight);
            this.GbFont2.Controls.Add(this.LblWidth);
            this.GbFont2.Controls.Add(this.LblInfoSize);
            this.GbFont2.Controls.Add(this.LblSize);
            this.GbFont2.Controls.Add(this.LblInfoFont);
            this.GbFont2.Controls.Add(this.LblFont);
            this.GbFont2.Controls.Add(this.BtnSelectFont);
            resources.ApplyResources(this.GbFont2, "GbFont2");
            this.GbFont2.Name = "GbFont2";
            this.GbFont2.TabStop = false;
            // 
            // BtnSelectFont
            // 
            resources.ApplyResources(this.BtnSelectFont, "BtnSelectFont");
            this.BtnSelectFont.Name = "BtnSelectFont";
            this.BtnSelectFont.UseVisualStyleBackColor = true;
            this.BtnSelectFont.Click += new System.EventHandler(this.BtnSelectFont_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button7);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.button6);
            this.tabPage2.Controls.Add(this.button5);
            this.tabPage2.Controls.Add(this.button4);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.button1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            resources.ApplyResources(this.button7, "button7");
            this.button7.Name = "button7";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.BtnLoadGraphic_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
            // 
            // button6
            // 
            resources.ApplyResources(this.button6, "button6");
            this.button6.Name = "button6";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.BtnLoadGraphic_Click);
            // 
            // button5
            // 
            resources.ApplyResources(this.button5, "button5");
            this.button5.Name = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.BtnLoadGraphic_Click);
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.BtnLoadGraphic_Click);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.BtnLoadGraphic_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.BtnLoadGraphic_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.BtnLoadGraphic_Click);
            // 
            // LblFont
            // 
            resources.ApplyResources(this.LblFont, "LblFont");
            this.LblFont.Name = "LblFont";
            // 
            // LblInfoFont
            // 
            resources.ApplyResources(this.LblInfoFont, "LblInfoFont");
            this.LblInfoFont.Name = "LblInfoFont";
            // 
            // LblInfoSize
            // 
            resources.ApplyResources(this.LblInfoSize, "LblInfoSize");
            this.LblInfoSize.Name = "LblInfoSize";
            // 
            // LblSize
            // 
            resources.ApplyResources(this.LblSize, "LblSize");
            this.LblSize.Name = "LblSize";
            // 
            // LblWidth
            // 
            resources.ApplyResources(this.LblWidth, "LblWidth");
            this.LblWidth.Name = "LblWidth";
            // 
            // LblHeight
            // 
            resources.ApplyResources(this.LblHeight, "LblHeight");
            this.LblHeight.Name = "LblHeight";
            // 
            // LblInfoWidth
            // 
            resources.ApplyResources(this.LblInfoWidth, "LblInfoWidth");
            this.LblInfoWidth.Name = "LblInfoWidth";
            this.toolTip1.SetToolTip(this.LblInfoWidth, resources.GetString("LblInfoWidth.ToolTip"));
            // 
            // LblInfoHeight
            // 
            resources.ApplyResources(this.LblInfoHeight, "LblInfoHeight");
            this.LblInfoHeight.Name = "LblInfoHeight";
            this.toolTip1.SetToolTip(this.LblInfoHeight, resources.GetString("LblInfoHeight.ToolTip"));
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "https://www.dafont.com";
            this.toolTip1.SetToolTip(this.linkLabel1, resources.GetString("linkLabel1.ToolTip"));
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // NUDWidth
            // 
            resources.ApplyResources(this.NUDWidth, "NUDWidth");
            this.NUDWidth.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NUDWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUDWidth.Name = "NUDWidth";
            this.toolTip1.SetToolTip(this.NUDWidth, resources.GetString("NUDWidth.ToolTip"));
            this.NUDWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // NUDHeight
            // 
            resources.ApplyResources(this.NUDHeight, "NUDHeight");
            this.NUDHeight.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NUDHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUDHeight.Name = "NUDHeight";
            this.toolTip1.SetToolTip(this.NUDHeight, resources.GetString("NUDHeight.ToolTip"));
            this.NUDHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // BtnSetWidth
            // 
            resources.ApplyResources(this.BtnSetWidth, "BtnSetWidth");
            this.BtnSetWidth.Name = "BtnSetWidth";
            this.toolTip1.SetToolTip(this.BtnSetWidth, resources.GetString("BtnSetWidth.ToolTip"));
            this.BtnSetWidth.UseVisualStyleBackColor = true;
            this.BtnSetWidth.Click += new System.EventHandler(this.BtnSetWidth_Click);
            // 
            // BtnSetHeight
            // 
            resources.ApplyResources(this.BtnSetHeight, "BtnSetHeight");
            this.BtnSetHeight.Name = "BtnSetHeight";
            this.toolTip1.SetToolTip(this.BtnSetHeight, resources.GetString("BtnSetHeight.ToolTip"));
            this.BtnSetHeight.UseVisualStyleBackColor = true;
            this.BtnSetHeight.Click += new System.EventHandler(this.BtnSetHeight_Click);
            // 
            // CbWordWrap
            // 
            resources.ApplyResources(this.CbWordWrap, "CbWordWrap");
            this.CbWordWrap.Name = "CbWordWrap";
            this.CbWordWrap.UseVisualStyleBackColor = true;
            this.CbWordWrap.CheckedChanged += new System.EventHandler(this.CbWordWrap_CheckedChanged);
            // 
            // cBToolTable
            // 
            resources.ApplyResources(this.cBToolTable, "cBToolTable");
            this.cBToolTable.Checked = global::GrblPlotter.Properties.Settings.Default.importGCToolTableUse;
            this.cBToolTable.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGCToolTableUse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolTable.Name = "cBToolTable";
            this.toolTip1.SetToolTip(this.cBToolTable, resources.GetString("cBToolTable.ToolTip"));
            this.cBToolTable.UseVisualStyleBackColor = true;
            this.cBToolTable.CheckedChanged += new System.EventHandler(this.CBToolTable_CheckedChanged);
            // 
            // CbHatchFill
            // 
            resources.ApplyResources(this.CbHatchFill, "CbHatchFill");
            this.CbHatchFill.Checked = global::GrblPlotter.Properties.Settings.Default.importGraphicHatchFillEnable;
            this.CbHatchFill.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGraphicHatchFillEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbHatchFill.Name = "CbHatchFill";
            this.toolTip1.SetToolTip(this.CbHatchFill, resources.GetString("CbHatchFill.ToolTip"));
            this.CbHatchFill.UseVisualStyleBackColor = true;
            this.CbHatchFill.CheckedChanged += new System.EventHandler(this.CbHatchFill_CheckedChanged);
            // 
            // CbInsertCode
            // 
            resources.ApplyResources(this.CbInsertCode, "CbInsertCode");
            this.CbInsertCode.Checked = global::GrblPlotter.Properties.Settings.Default.fromFormInsertEnable;
            this.CbInsertCode.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "fromFormInsertEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbInsertCode.Name = "CbInsertCode";
            this.CbInsertCode.UseVisualStyleBackColor = true;
            // 
            // tBText
            // 
            this.tBText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "createtextFontText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBText, "tBText");
            this.tBText.ForeColor = System.Drawing.Color.Black;
            this.tBText.Name = "tBText";
            this.tBText.Text = global::GrblPlotter.Properties.Settings.Default.createTextFontText;
            this.tBText.TextChanged += new System.EventHandler(this.TbText_TextChanged);
            // 
            // cBLineBreak
            // 
            resources.ApplyResources(this.cBLineBreak, "cBLineBreak");
            this.cBLineBreak.Checked = global::GrblPlotter.Properties.Settings.Default.createTextLineBreakEnable;
            this.cBLineBreak.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "createTextLineBreakEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBLineBreak.Name = "cBLineBreak";
            this.toolTip1.SetToolTip(this.cBLineBreak, resources.GetString("cBLineBreak.ToolTip"));
            this.cBLineBreak.UseVisualStyleBackColor = true;
            // 
            // CbOutline
            // 
            resources.ApplyResources(this.CbOutline, "CbOutline");
            this.CbOutline.Checked = true;
            this.CbOutline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CbOutline.Name = "CbOutline";
            this.toolTip1.SetToolTip(this.CbOutline, resources.GetString("CbOutline.ToolTip"));
            this.CbOutline.UseVisualStyleBackColor = true;
            this.CbOutline.CheckedChanged += new System.EventHandler(this.CbOutline_CheckedChanged);
            // 
            // GCodeFromText
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.tabControl1);
            this.Name = "GCodeFromText";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextForm_FormClosing);
            this.Load += new System.EventHandler(this.TextForm_Load);
            this.Resize += new System.EventHandler(this.GCodeFromText_Resize);
            this.GbFont1.ResumeLayout(false);
            this.GbFont1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLineBreak)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.GbFont2.ResumeLayout(false);
            this.GbFont2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cBConnectLetter;
        private System.Windows.Forms.CheckBox cBPauseChar;
        private System.Windows.Forms.CheckBox cBPauseLine;
        private System.Windows.Forms.CheckBox cBPauseWord;
        private System.Windows.Forms.CheckBox cBToolTable;
        private System.Windows.Forms.ComboBox cBFont;
        private System.Windows.Forms.ComboBox cBTool;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox GbFont1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUDFontDistance;
        private System.Windows.Forms.NumericUpDown nUDFontLine;
        private System.Windows.Forms.NumericUpDown nUDFontSize;
        private System.Windows.Forms.TextBox tBText;
        private System.Windows.Forms.ToolTip toolTip1;
        internal System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.NumericUpDown nUDLineBreak;
        private System.Windows.Forms.CheckBox cBLineBreak;
        private System.Windows.Forms.RadioButton RbAlign3;
        private System.Windows.Forms.RadioButton RbAlign2;
        private System.Windows.Forms.RadioButton RbAlign1;
        public System.Windows.Forms.CheckBox CbInsertCode;
        private System.Windows.Forms.Button BtnSelectFont;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.RadioButton RbFont2;
        private System.Windows.Forms.RadioButton RbFont1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox GbFont2;
        private System.Windows.Forms.CheckBox CbHatchFill;
        private System.Windows.Forms.Label LblInfoFont;
        private System.Windows.Forms.Label LblFont;
        private System.Windows.Forms.Label LblInfoSize;
        private System.Windows.Forms.Label LblSize;
        private System.Windows.Forms.Label LblInfoHeight;
        private System.Windows.Forms.Label LblInfoWidth;
        private System.Windows.Forms.Label LblHeight;
        private System.Windows.Forms.Label LblWidth;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button BtnSetHeight;
        private System.Windows.Forms.Button BtnSetWidth;
        private System.Windows.Forms.NumericUpDown NUDHeight;
        private System.Windows.Forms.NumericUpDown NUDWidth;
        private System.Windows.Forms.CheckBox CbWordWrap;
        private System.Windows.Forms.CheckBox CbOutline;
    }
}