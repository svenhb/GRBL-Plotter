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
            this.LblLetterHeight = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.GbFont1 = new System.Windows.Forms.GroupBox();
            this.LblLineDistance = new System.Windows.Forms.Label();
            this.LblLetterDistance = new System.Windows.Forms.Label();
            this.RbAlign3 = new System.Windows.Forms.RadioButton();
            this.RbAlign2 = new System.Windows.Forms.RadioButton();
            this.RbAlign1 = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.GbCodeSettings = new System.Windows.Forms.GroupBox();
            this.CbOutline = new System.Windows.Forms.CheckBox();
            this.cBTool = new System.Windows.Forms.ComboBox();
            this.LblTool = new System.Windows.Forms.Label();
            this.LblOtherGCode = new System.Windows.Forms.Label();
            this.cBConnectLetter = new System.Windows.Forms.CheckBox();
            this.cBPauseLine = new System.Windows.Forms.CheckBox();
            this.cBPauseWord = new System.Windows.Forms.CheckBox();
            this.cBPauseChar = new System.Windows.Forms.CheckBox();
            this.BtnSaveIni = new System.Windows.Forms.Button();
            this.BtnHelp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.LblInfoWidth = new System.Windows.Forms.Label();
            this.LblInfoHeight = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.BtnSetWidth = new System.Windows.Forms.Button();
            this.BtnSetHeight = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.GbCodeOptions = new System.Windows.Forms.GroupBox();
            this.CbSmoothPath = new System.Windows.Forms.CheckBox();
            this.CbWordWrap = new System.Windows.Forms.CheckBox();
            this.RbFont2 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.GbFont2 = new System.Windows.Forms.GroupBox();
            this.LblHeight = new System.Windows.Forms.Label();
            this.LblWidth = new System.Windows.Forms.Label();
            this.LblInfoSize = new System.Windows.Forms.Label();
            this.LblSize = new System.Windows.Forms.Label();
            this.LblInfoFont = new System.Windows.Forms.Label();
            this.LblFont = new System.Windows.Forms.Label();
            this.BtnSelectFont = new System.Windows.Forms.Button();
            this.LblToolDiameter = new System.Windows.Forms.Label();
            this.LblToolDiameter1 = new System.Windows.Forms.Label();
            this.LblDevice = new System.Windows.Forms.Label();
            this.LblDevice1 = new System.Windows.Forms.Label();
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
            this.label1 = new System.Windows.Forms.Label();
            this.NudCurvefittingError = new System.Windows.Forms.NumericUpDown();
            this.cBImportGraphicNoise = new System.Windows.Forms.CheckBox();
            this.cBToolTable = new System.Windows.Forms.CheckBox();
            this.CbHatchFill = new System.Windows.Forms.CheckBox();
            this.RbFont1 = new System.Windows.Forms.RadioButton();
            this.CbInsertCode = new System.Windows.Forms.CheckBox();
            this.tBText = new System.Windows.Forms.TextBox();
            this.NUDHeight = new System.Windows.Forms.NumericUpDown();
            this.NUDWidth = new System.Windows.Forms.NumericUpDown();
            this.nUDLineBreak = new System.Windows.Forms.NumericUpDown();
            this.cBLineBreak = new System.Windows.Forms.CheckBox();
            this.nUDFontLine = new System.Windows.Forms.NumericUpDown();
            this.nUDFontDistance = new System.Windows.Forms.NumericUpDown();
            this.NudFontSize = new System.Windows.Forms.NumericUpDown();
            this.cBFont = new System.Windows.Forms.ComboBox();
            this.GbFont1.SuspendLayout();
            this.GbCodeSettings.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.GbCodeOptions.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.GbFont2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCurvefittingError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLineBreak)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // LblLetterHeight
            // 
            resources.ApplyResources(this.LblLetterHeight, "LblLetterHeight");
            this.LblLetterHeight.Name = "LblLetterHeight";
            this.toolTip1.SetToolTip(this.LblLetterHeight, resources.GetString("LblLetterHeight.ToolTip"));
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
            this.GbFont1.Controls.Add(this.LblLineDistance);
            this.GbFont1.Controls.Add(this.nUDFontDistance);
            this.GbFont1.Controls.Add(this.LblLetterDistance);
            this.GbFont1.Controls.Add(this.NudFontSize);
            this.GbFont1.Controls.Add(this.LblLetterHeight);
            this.GbFont1.Controls.Add(this.cBFont);
            resources.ApplyResources(this.GbFont1, "GbFont1");
            this.GbFont1.Name = "GbFont1";
            this.GbFont1.TabStop = false;
            // 
            // LblLineDistance
            // 
            resources.ApplyResources(this.LblLineDistance, "LblLineDistance");
            this.LblLineDistance.Name = "LblLineDistance";
            this.toolTip1.SetToolTip(this.LblLineDistance, resources.GetString("LblLineDistance.ToolTip"));
            // 
            // LblLetterDistance
            // 
            resources.ApplyResources(this.LblLetterDistance, "LblLetterDistance");
            this.LblLetterDistance.Name = "LblLetterDistance";
            this.toolTip1.SetToolTip(this.LblLetterDistance, resources.GetString("LblLetterDistance.ToolTip"));
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
            // GbCodeSettings
            // 
            this.GbCodeSettings.Controls.Add(this.cBImportGraphicNoise);
            this.GbCodeSettings.Controls.Add(this.CbOutline);
            this.GbCodeSettings.Controls.Add(this.cBToolTable);
            this.GbCodeSettings.Controls.Add(this.CbHatchFill);
            this.GbCodeSettings.Controls.Add(this.cBTool);
            this.GbCodeSettings.Controls.Add(this.LblTool);
            this.GbCodeSettings.Controls.Add(this.LblOtherGCode);
            resources.ApplyResources(this.GbCodeSettings, "GbCodeSettings");
            this.GbCodeSettings.Name = "GbCodeSettings";
            this.GbCodeSettings.TabStop = false;
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
            // cBTool
            // 
            this.cBTool.FormattingEnabled = true;
            resources.ApplyResources(this.cBTool, "cBTool");
            this.cBTool.Name = "cBTool";
            this.cBTool.SelectedIndexChanged += new System.EventHandler(this.CBTool_SelectedIndexChanged);
            // 
            // LblTool
            // 
            resources.ApplyResources(this.LblTool, "LblTool");
            this.LblTool.Name = "LblTool";
            // 
            // LblOtherGCode
            // 
            resources.ApplyResources(this.LblOtherGCode, "LblOtherGCode");
            this.LblOtherGCode.Name = "LblOtherGCode";
            // 
            // cBConnectLetter
            // 
            resources.ApplyResources(this.cBConnectLetter, "cBConnectLetter");
            this.cBConnectLetter.Name = "cBConnectLetter";
            this.toolTip1.SetToolTip(this.cBConnectLetter, resources.GetString("cBConnectLetter.ToolTip"));
            this.cBConnectLetter.UseVisualStyleBackColor = true;
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
            // BtnSaveIni
            // 
            this.BtnSaveIni.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.BtnSaveIni, "BtnSaveIni");
            this.BtnSaveIni.Name = "BtnSaveIni";
            this.BtnSaveIni.Tag = "id=form-text";
            this.toolTip1.SetToolTip(this.BtnSaveIni, resources.GetString("BtnSaveIni.ToolTip"));
            this.BtnSaveIni.UseVisualStyleBackColor = false;
            this.BtnSaveIni.Click += new System.EventHandler(this.BtnSaveIni_Click);
            // 
            // BtnHelp
            // 
            this.BtnHelp.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.BtnHelp, "BtnHelp");
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Tag = "id=form-text";
            this.toolTip1.SetToolTip(this.BtnHelp, resources.GetString("BtnHelp.ToolTip"));
            this.BtnHelp.UseVisualStyleBackColor = false;
            this.BtnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
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
            this.tabPage1.Controls.Add(this.GbCodeOptions);
            this.tabPage1.Controls.Add(this.CbWordWrap);
            this.tabPage1.Controls.Add(this.GbCodeSettings);
            this.tabPage1.Controls.Add(this.RbFont2);
            this.tabPage1.Controls.Add(this.RbFont1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.CbInsertCode);
            this.tabPage1.Controls.Add(this.btnApply);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.tBText);
            this.tabPage1.Controls.Add(this.GbFont2);
            this.tabPage1.Controls.Add(this.GbFont1);
            this.tabPage1.Controls.Add(this.LblToolDiameter);
            this.tabPage1.Controls.Add(this.LblToolDiameter1);
            this.tabPage1.Controls.Add(this.LblDevice);
            this.tabPage1.Controls.Add(this.LblDevice1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // GbCodeOptions
            // 
            this.GbCodeOptions.Controls.Add(this.label1);
            this.GbCodeOptions.Controls.Add(this.NudCurvefittingError);
            this.GbCodeOptions.Controls.Add(this.CbSmoothPath);
            this.GbCodeOptions.Controls.Add(this.cBPauseChar);
            this.GbCodeOptions.Controls.Add(this.cBPauseWord);
            this.GbCodeOptions.Controls.Add(this.cBPauseLine);
            this.GbCodeOptions.Controls.Add(this.cBConnectLetter);
            resources.ApplyResources(this.GbCodeOptions, "GbCodeOptions");
            this.GbCodeOptions.Name = "GbCodeOptions";
            this.GbCodeOptions.TabStop = false;
            // 
            // CbSmoothPath
            // 
            resources.ApplyResources(this.CbSmoothPath, "CbSmoothPath");
            this.CbSmoothPath.Name = "CbSmoothPath";
            this.CbSmoothPath.UseVisualStyleBackColor = true;
            // 
            // CbWordWrap
            // 
            resources.ApplyResources(this.CbWordWrap, "CbWordWrap");
            this.CbWordWrap.Name = "CbWordWrap";
            this.CbWordWrap.UseVisualStyleBackColor = true;
            this.CbWordWrap.CheckedChanged += new System.EventHandler(this.CbWordWrap_CheckedChanged);
            // 
            // RbFont2
            // 
            resources.ApplyResources(this.RbFont2, "RbFont2");
            this.RbFont2.Name = "RbFont2";
            this.RbFont2.UseVisualStyleBackColor = true;
            this.RbFont2.CheckedChanged += new System.EventHandler(this.RbFont1_CheckedChanged);
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
            // LblHeight
            // 
            resources.ApplyResources(this.LblHeight, "LblHeight");
            this.LblHeight.Name = "LblHeight";
            // 
            // LblWidth
            // 
            resources.ApplyResources(this.LblWidth, "LblWidth");
            this.LblWidth.Name = "LblWidth";
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
            // LblInfoFont
            // 
            resources.ApplyResources(this.LblInfoFont, "LblInfoFont");
            this.LblInfoFont.Name = "LblInfoFont";
            // 
            // LblFont
            // 
            resources.ApplyResources(this.LblFont, "LblFont");
            this.LblFont.Name = "LblFont";
            // 
            // BtnSelectFont
            // 
            resources.ApplyResources(this.BtnSelectFont, "BtnSelectFont");
            this.BtnSelectFont.Name = "BtnSelectFont";
            this.BtnSelectFont.UseVisualStyleBackColor = true;
            this.BtnSelectFont.Click += new System.EventHandler(this.BtnSelectFont_Click);
            // 
            // LblToolDiameter
            // 
            resources.ApplyResources(this.LblToolDiameter, "LblToolDiameter");
            this.LblToolDiameter.Name = "LblToolDiameter";
            // 
            // LblToolDiameter1
            // 
            resources.ApplyResources(this.LblToolDiameter1, "LblToolDiameter1");
            this.LblToolDiameter1.Name = "LblToolDiameter1";
            // 
            // LblDevice
            // 
            resources.ApplyResources(this.LblDevice, "LblDevice");
            this.LblDevice.Name = "LblDevice";
            // 
            // LblDevice1
            // 
            resources.ApplyResources(this.LblDevice1, "LblDevice1");
            this.LblDevice1.Name = "LblDevice1";
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NudCurvefittingError
            // 
            this.NudCurvefittingError.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createTextHersheySmoothCurveFittingError", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudCurvefittingError.DecimalPlaces = 2;
            resources.ApplyResources(this.NudCurvefittingError, "NudCurvefittingError");
            this.NudCurvefittingError.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.NudCurvefittingError.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NudCurvefittingError.Name = "NudCurvefittingError";
            this.NudCurvefittingError.Value = global::GrblPlotter.Properties.Settings.Default.createTextHersheySmoothCurveFittingError;
            // 
            // cBImportGraphicNoise
            // 
            resources.ApplyResources(this.cBImportGraphicNoise, "cBImportGraphicNoise");
            this.cBImportGraphicNoise.Checked = global::GrblPlotter.Properties.Settings.Default.importGraphicNoiseEnable;
            this.cBImportGraphicNoise.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGraphicNoiseEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBImportGraphicNoise.Name = "cBImportGraphicNoise";
            this.toolTip1.SetToolTip(this.cBImportGraphicNoise, resources.GetString("cBImportGraphicNoise.ToolTip"));
            this.cBImportGraphicNoise.UseVisualStyleBackColor = true;
            // 
            // cBToolTable
            // 
            resources.ApplyResources(this.cBToolTable, "cBToolTable");
            this.cBToolTable.Checked = global::GrblPlotter.Properties.Settings.Default.importGCToolListUse;
            this.cBToolTable.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importGCToolListUse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            // RbFont1
            // 
            resources.ApplyResources(this.RbFont1, "RbFont1");
            this.RbFont1.Checked = global::GrblPlotter.Properties.Settings.Default.createTextHersheySelect;
            this.RbFont1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "createTextHersheySelect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RbFont1.Name = "RbFont1";
            this.RbFont1.TabStop = true;
            this.RbFont1.UseVisualStyleBackColor = true;
            this.RbFont1.CheckedChanged += new System.EventHandler(this.RbFont1_CheckedChanged);
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
            this.tBText.FontChanged += new System.EventHandler(this.TbText_FontChanged);
            this.tBText.TextChanged += new System.EventHandler(this.TbText_TextChanged);
            // 
            // NUDHeight
            // 
            this.NUDHeight.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createTextSystemSizeY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.NUDHeight.Value = global::GrblPlotter.Properties.Settings.Default.createTextSystemSizeY;
            // 
            // NUDWidth
            // 
            this.NUDWidth.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createTextSystemSizeX", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.NUDWidth.Value = global::GrblPlotter.Properties.Settings.Default.createTextSystemSizeX;
            // 
            // nUDLineBreak
            // 
            this.nUDLineBreak.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createTextHersheyLineBreak", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.nUDLineBreak.Value = global::GrblPlotter.Properties.Settings.Default.createTextHersheyLineBreak;
            // 
            // cBLineBreak
            // 
            resources.ApplyResources(this.cBLineBreak, "cBLineBreak");
            this.cBLineBreak.Checked = global::GrblPlotter.Properties.Settings.Default.createTextHersheyLineBreakEnable;
            this.cBLineBreak.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "createTextHersheyLineBreakEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBLineBreak.Name = "cBLineBreak";
            this.toolTip1.SetToolTip(this.cBLineBreak, resources.GetString("cBLineBreak.ToolTip"));
            this.cBLineBreak.UseVisualStyleBackColor = true;
            // 
            // nUDFontLine
            // 
            this.nUDFontLine.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createTextHersheyLineDistance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDFontLine.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDFontLine, "nUDFontLine");
            this.nUDFontLine.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDFontLine.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDFontLine.Name = "nUDFontLine";
            this.nUDFontLine.Value = global::GrblPlotter.Properties.Settings.Default.createTextHersheyLineDistance;
            // 
            // nUDFontDistance
            // 
            this.nUDFontDistance.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createTextHersheyLetterDistance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.nUDFontDistance.Value = global::GrblPlotter.Properties.Settings.Default.createTextHersheyLetterDistance;
            // 
            // NudFontSize
            // 
            this.NudFontSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createTextHersheyFontSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudFontSize.DecimalPlaces = 1;
            resources.ApplyResources(this.NudFontSize, "NudFontSize");
            this.NudFontSize.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.NudFontSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudFontSize.Name = "NudFontSize";
            this.NudFontSize.Value = global::GrblPlotter.Properties.Settings.Default.createTextHersheyFontSize;
            this.NudFontSize.ValueChanged += new System.EventHandler(this.NudFontSize_ValueChanged);
            this.NudFontSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NudFontSize_KeyDown);
            // 
            // cBFont
            // 
            this.cBFont.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "createTextHersheyFontName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBFont.FormattingEnabled = true;
            resources.ApplyResources(this.cBFont, "cBFont");
            this.cBFont.Name = "cBFont";
            this.cBFont.Text = global::GrblPlotter.Properties.Settings.Default.createTextHersheyFontName;
            // 
            // GCodeFromText
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.BtnHelp);
            this.Controls.Add(this.BtnSaveIni);
            this.Controls.Add(this.tabControl1);
            this.Name = "GCodeFromText";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextForm_FormClosing);
            this.Load += new System.EventHandler(this.TextForm_Load);
            this.Resize += new System.EventHandler(this.GCodeFromText_Resize);
            this.GbFont1.ResumeLayout(false);
            this.GbFont1.PerformLayout();
            this.GbCodeSettings.ResumeLayout(false);
            this.GbCodeSettings.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.GbCodeOptions.ResumeLayout(false);
            this.GbCodeOptions.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.GbFont2.ResumeLayout(false);
            this.GbFont2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCurvefittingError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLineBreak)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudFontSize)).EndInit();
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
        private System.Windows.Forms.GroupBox GbCodeSettings;
        private System.Windows.Forms.GroupBox GbFont1;
        private System.Windows.Forms.Label LblLetterDistance;
        private System.Windows.Forms.Label LblLineDistance;
        private System.Windows.Forms.Label LblLetterHeight;
        private System.Windows.Forms.Label LblOtherGCode;
        private System.Windows.Forms.Label LblTool;
        private System.Windows.Forms.NumericUpDown nUDFontDistance;
        private System.Windows.Forms.NumericUpDown nUDFontLine;
        private System.Windows.Forms.NumericUpDown NudFontSize;
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
        private System.Windows.Forms.Button BtnHelp;
        private System.Windows.Forms.Button BtnSaveIni;
        private System.Windows.Forms.CheckBox cBImportGraphicNoise;
        private System.Windows.Forms.Label LblDevice1;
        private System.Windows.Forms.Label LblDevice;
        private System.Windows.Forms.Label LblToolDiameter1;
        private System.Windows.Forms.Label LblToolDiameter;
        private System.Windows.Forms.GroupBox GbCodeOptions;
        private System.Windows.Forms.CheckBox CbSmoothPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NudCurvefittingError;
    }
}