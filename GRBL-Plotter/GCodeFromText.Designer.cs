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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeFromText));
            this.tBText = new System.Windows.Forms.TextBox();
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
            this.cBPauseLine = new System.Windows.Forms.CheckBox();
            this.cBPauseWord = new System.Windows.Forms.CheckBox();
            this.cBPauseChar = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tBText
            // 
            this.tBText.Location = new System.Drawing.Point(6, 126);
            this.tBText.Multiline = true;
            this.tBText.Name = "tBText";
            this.tBText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tBText.Size = new System.Drawing.Size(306, 95);
            this.tBText.TabIndex = 0;
            this.tBText.Text = "Max Mustermann\r\nEinbahnstr. 5\r\n12345 Berlin";
            this.tBText.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Char height";
            // 
            // cBFont
            // 
            this.cBFont.FormattingEnabled = true;
            this.cBFont.Location = new System.Drawing.Point(6, 15);
            this.cBFont.Name = "cBFont";
            this.cBFont.Size = new System.Drawing.Size(129, 21);
            this.cBFont.TabIndex = 6;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(112, 227);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(91, 23);
            this.btnApply.TabIndex = 9;
            this.btnApply.Text = "Create GCode";
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
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(140, 114);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Font";
            // 
            // nUDFontLine
            // 
            this.nUDFontLine.DecimalPlaces = 1;
            this.nUDFontLine.Location = new System.Drawing.Point(84, 80);
            this.nUDFontLine.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDFontLine.Name = "nUDFontLine";
            this.nUDFontLine.Size = new System.Drawing.Size(51, 20);
            this.nUDFontLine.TabIndex = 11;
            this.nUDFontLine.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDFontLine.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 82);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Line distance";
            // 
            // nUDFontDistance
            // 
            this.nUDFontDistance.DecimalPlaces = 1;
            this.nUDFontDistance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDFontDistance.Location = new System.Drawing.Point(84, 59);
            this.nUDFontDistance.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nUDFontDistance.Name = "nUDFontDistance";
            this.nUDFontDistance.Size = new System.Drawing.Size(51, 20);
            this.nUDFontDistance.TabIndex = 9;
            this.nUDFontDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Char distance";
            // 
            // nUDFontSize
            // 
            this.nUDFontSize.DecimalPlaces = 1;
            this.nUDFontSize.Location = new System.Drawing.Point(84, 38);
            this.nUDFontSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDFontSize.Name = "nUDFontSize";
            this.nUDFontSize.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.nUDFontSize.Size = new System.Drawing.Size(51, 20);
            this.nUDFontSize.TabIndex = 7;
            this.nUDFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            this.btnCancel.Location = new System.Drawing.Point(237, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cBPauseLine);
            this.groupBox1.Controls.Add(this.cBPauseWord);
            this.groupBox1.Controls.Add(this.cBPauseChar);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(152, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(157, 114);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "G-Code";
            // 
            // cBPauseLine
            // 
            this.cBPauseLine.AutoSize = true;
            this.cBPauseLine.Location = new System.Drawing.Point(9, 59);
            this.cBPauseLine.Name = "cBPauseLine";
            this.cBPauseLine.Size = new System.Drawing.Size(121, 17);
            this.cBPauseLine.TabIndex = 3;
            this.cBPauseLine.Text = "Add Pause after line";
            this.cBPauseLine.UseVisualStyleBackColor = true;
            // 
            // cBPauseWord
            // 
            this.cBPauseWord.AutoSize = true;
            this.cBPauseWord.Location = new System.Drawing.Point(9, 36);
            this.cBPauseWord.Name = "cBPauseWord";
            this.cBPauseWord.Size = new System.Drawing.Size(128, 17);
            this.cBPauseWord.TabIndex = 2;
            this.cBPauseWord.Text = "Add Pause after word";
            this.cBPauseWord.UseVisualStyleBackColor = true;
            // 
            // cBPauseChar
            // 
            this.cBPauseChar.AutoSize = true;
            this.cBPauseChar.Location = new System.Drawing.Point(9, 15);
            this.cBPauseChar.Name = "cBPauseChar";
            this.cBPauseChar.Size = new System.Drawing.Size(126, 17);
            this.cBPauseChar.TabIndex = 1;
            this.cBPauseChar.Text = "Add Pause after char";
            this.cBPauseChar.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "Other G-Code settings in\r\nSetup SVG-Import ";
            // 
            // GCodeFromText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(324, 261);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.tBText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(340, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(340, 300);
            this.Name = "GCodeFromText";
            this.Text = "Create GCode from Text";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextForm_FormClosing);
            this.Load += new System.EventHandler(this.TextForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFontSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tBText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cBFont;
        public System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown nUDFontLine;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nUDFontDistance;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nUDFontSize;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cBPauseLine;
        private System.Windows.Forms.CheckBox cBPauseWord;
        private System.Windows.Forms.CheckBox cBPauseChar;
    }
}