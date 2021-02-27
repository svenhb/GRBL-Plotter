/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.nUDFontLine = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nUDFontDistance = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.nUDFontSize = new System.Windows.Forms.NumericUpDown();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cBToolTable = new System.Windows.Forms.CheckBox();
            this.cBConnectLetter = new System.Windows.Forms.CheckBox();
            this.cBTool = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cBPauseLine = new System.Windows.Forms.CheckBox();
            this.cBPauseWord = new System.Windows.Forms.CheckBox();
            this.cBPauseChar = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tBText = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button6 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
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
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.nUDFontLine);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.nUDFontDistance);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.nUDFontSize);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cBFont);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // nUDFontLine
            // 
            this.nUDFontLine.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDFontLine, "nUDFontLine");
            this.nUDFontLine.Maximum = new decimal(new int[] {
            1000,
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
            this.nUDFontDistance.Minimum = new decimal(new int[] {
            100,
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
            1000,
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
            this.nUDFontSize.ValueChanged += new System.EventHandler(this.nUDFontSize_ValueChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cBToolTable);
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
            // cBToolTable
            // 
            resources.ApplyResources(this.cBToolTable, "cBToolTable");
            this.cBToolTable.Checked = global::GRBL_Plotter.Properties.Settings.Default.importGCToolTableUse;
            this.cBToolTable.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "importGCToolTableUse", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBToolTable.Name = "cBToolTable";
            this.toolTip1.SetToolTip(this.cBToolTable, resources.GetString("cBToolTable.ToolTip"));
            this.cBToolTable.UseVisualStyleBackColor = true;
            this.cBToolTable.CheckedChanged += new System.EventHandler(this.cBToolTable_CheckedChanged);
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
            this.cBTool.SelectedIndexChanged += new System.EventHandler(this.cBTool_SelectedIndexChanged);
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
            // tBText
            // 
            this.tBText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "createtextFontText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBText, "tBText");
            this.tBText.Name = "tBText";
            this.tBText.Text = global::GRBL_Plotter.Properties.Settings.Default.createTextFontText;
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
            this.tabPage1.Controls.Add(this.btnApply);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.tBText);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            // button6
            // 
            resources.ApplyResources(this.button6, "button6");
            this.button6.Name = "button6";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.btnLoadGraphic_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            // 
            // button5
            // 
            resources.ApplyResources(this.button5, "button5");
            this.button5.Name = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.btnLoadGraphic_Click);
            // 
            // button4
            // 
            resources.ApplyResources(this.button4, "button4");
            this.button4.Name = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnLoadGraphic_Click);
            // 
            // button3
            // 
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnLoadGraphic_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnLoadGraphic_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnLoadGraphic_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // button7
            // 
            resources.ApplyResources(this.button7, "button7");
            this.button7.Name = "button7";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.btnLoadGraphic_Click);
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox3;
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
        public System.Windows.Forms.Button btnApply;
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
    }
}