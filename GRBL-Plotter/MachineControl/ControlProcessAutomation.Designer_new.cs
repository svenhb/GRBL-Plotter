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
    partial class ProcessAutomation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessAutomation));
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.LblCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.CmsDataGridEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveRowUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveRowDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TbProcessInfo = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.BtnLoad = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.LblLoaded = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button3 = new System.Windows.Forms.Button();
            this.GbCounter = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TbFill = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.NudDigitis = new System.Windows.Forms.NumericUpDown();
            this.LblCounterResult = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TbCounterRear = new System.Windows.Forms.TextBox();
            this.NudCounter = new System.Windows.Forms.NumericUpDown();
            this.TbCounterFront = new System.Windows.Forms.TextBox();
            this.GbData = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TbDataDelimiter = new System.Windows.Forms.TextBox();
            this.LblDataLoaded = new System.Windows.Forms.Label();
            this.BtnLoadData = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.NudDataIndex = new System.Windows.Forms.NumericUpDown();
            this.BtnDataIndexClear = new System.Windows.Forms.Button();
            this.GbControl = new System.Windows.Forms.GroupBox();
            this.BtnNew = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.LblInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.NudTimerInterval = new System.Windows.Forms.NumericUpDown();
            this.FctbData = new FastColoredTextBoxNS.FastColoredTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.CmsDataGridEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.GbCounter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDigitis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCounter)).BeginInit();
            this.GbData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDataIndex)).BeginInit();
            this.GbControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudTimerInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FctbData)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnStart
            // 
            resources.ApplyResources(this.BtnStart, "BtnStart");
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnStop
            // 
            resources.ApplyResources(this.BtnStop, "BtnStop");
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // LblCount
            // 
            resources.ApplyResources(this.LblCount, "LblCount");
            this.LblCount.Name = "LblCount";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.CmsDataGridEdit;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
            this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.DataGridView1_CurrentCellDirtyStateChanged);
            this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DataGridView1_EditingControlShowing);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
            // 
            // CmsDataGridEdit
            // 
            this.CmsDataGridEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteRowToolStripMenuItem,
            this.moveRowUpToolStripMenuItem,
            this.moveRowDownToolStripMenuItem});
            this.CmsDataGridEdit.Name = "CmsDataGridEdit";
            resources.ApplyResources(this.CmsDataGridEdit, "CmsDataGridEdit");
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            resources.ApplyResources(this.deleteRowToolStripMenuItem, "deleteRowToolStripMenuItem");
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
            // 
            // moveRowUpToolStripMenuItem
            // 
            this.moveRowUpToolStripMenuItem.Name = "moveRowUpToolStripMenuItem";
            resources.ApplyResources(this.moveRowUpToolStripMenuItem, "moveRowUpToolStripMenuItem");
            this.moveRowUpToolStripMenuItem.Click += new System.EventHandler(this.MoveRowUpToolStripMenuItem_Click);
            // 
            // moveRowDownToolStripMenuItem
            // 
            this.moveRowDownToolStripMenuItem.Name = "moveRowDownToolStripMenuItem";
            resources.ApplyResources(this.moveRowDownToolStripMenuItem, "moveRowDownToolStripMenuItem");
            this.moveRowDownToolStripMenuItem.Click += new System.EventHandler(this.MoveRowDownToolStripMenuItem_Click);
            // 
            // TbProcessInfo
            // 
            resources.ApplyResources(this.TbProcessInfo, "TbProcessInfo");
            this.TbProcessInfo.Name = "TbProcessInfo";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // BtnLoad
            // 
            resources.ApplyResources(this.BtnLoad, "BtnLoad");
            this.BtnLoad.Name = "BtnLoad";
            this.BtnLoad.UseVisualStyleBackColor = true;
            this.BtnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // LblLoaded
            // 
            resources.ApplyResources(this.LblLoaded, "LblLoaded");
            this.LblLoaded.Name = "LblLoaded";
            // 
            // splitContainer1
            // 
            this.splitContainer1.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::GrblPlotter.Properties.Settings.Default, "processSplitDistance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.button3);
            this.splitContainer1.Panel1.Controls.Add(this.GbCounter);
            this.splitContainer1.Panel1.Controls.Add(this.GbData);
            this.splitContainer1.Panel1.Controls.Add(this.GbControl);
            this.splitContainer1.Panel1.Controls.Add(this.FctbData);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel1.SizeChanged += new System.EventHandler(this.SplitContainer1_Panel1_SizeChanged);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Panel2.Controls.Add(this.TbProcessInfo);
            this.splitContainer1.SplitterDistance = global::GrblPlotter.Properties.Settings.Default.processSplitDistance;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.Tag = "id=form-pautomation";
            this.toolTip1.SetToolTip(this.button3, resources.GetString("button3.ToolTip"));
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.BtnHelp_Click);
            // 
            // GbCounter
            // 
            this.GbCounter.Controls.Add(this.label7);
            this.GbCounter.Controls.Add(this.TbFill);
            this.GbCounter.Controls.Add(this.TbDataDelimiter);
            this.GbCounter.Controls.Add(this.label6);
            this.GbCounter.Controls.Add(this.NudDigitis);
            this.GbCounter.Controls.Add(this.LblCounterResult);
            this.GbCounter.Controls.Add(this.label5);
            this.GbCounter.Controls.Add(this.TbCounterRear);
            this.GbCounter.Controls.Add(this.NudCounter);
            this.GbCounter.Controls.Add(this.TbCounterFront);
            resources.ApplyResources(this.GbCounter, "GbCounter");
            this.GbCounter.Name = "GbCounter";
            this.GbCounter.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // TbFill
            // 
            this.TbFill.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "processCounterFill", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.TbFill, "TbFill");
            this.TbFill.Name = "TbFill";
            this.TbFill.Text = global::GrblPlotter.Properties.Settings.Default.processCounterFill;
            this.TbFill.TextChanged += new System.EventHandler(this.TbCounter_TextChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // NudDigitis
            // 
            this.NudDigitis.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "processCounterDigits", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NudDigitis, "NudDigitis");
            this.NudDigitis.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudDigitis.Name = "NudDigitis";
            this.NudDigitis.Value = global::GrblPlotter.Properties.Settings.Default.processCounterDigits;
            this.NudDigitis.ValueChanged += new System.EventHandler(this.TbCounter_TextChanged);
            // 
            // LblCounterResult
            // 
            resources.ApplyResources(this.LblCounterResult, "LblCounterResult");
            this.LblCounterResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.LblCounterResult.Name = "LblCounterResult";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // TbCounterRear
            // 
            this.TbCounterRear.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "processCounterRear", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.TbCounterRear, "TbCounterRear");
            this.TbCounterRear.Name = "TbCounterRear";
            this.TbCounterRear.Text = global::GrblPlotter.Properties.Settings.Default.processCounterRear;
            this.TbCounterRear.TextChanged += new System.EventHandler(this.TbCounter_TextChanged);
            // 
            // NudCounter
            // 
            this.NudCounter.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "processCounter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NudCounter, "NudCounter");
            this.NudCounter.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.NudCounter.Name = "NudCounter";
            this.NudCounter.Value = global::GrblPlotter.Properties.Settings.Default.processCounter;
            this.NudCounter.ValueChanged += new System.EventHandler(this.TbCounter_TextChanged);
            // 
            // TbCounterFront
            // 
            this.TbCounterFront.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "processCounterFront", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.TbCounterFront, "TbCounterFront");
            this.TbCounterFront.Name = "TbCounterFront";
            this.TbCounterFront.Text = global::GrblPlotter.Properties.Settings.Default.processCounterFront;
            this.TbCounterFront.TextChanged += new System.EventHandler(this.TbCounter_TextChanged);
            // 
            // GbData
            // 
            this.GbData.Controls.Add(this.comboBox1);
            this.GbData.Controls.Add(this.label8);
            this.GbData.Controls.Add(this.LblDataLoaded);
            this.GbData.Controls.Add(this.BtnLoadData);
            this.GbData.Controls.Add(this.label4);
            this.GbData.Controls.Add(this.NudDataIndex);
            this.GbData.Controls.Add(this.BtnDataIndexClear);
            resources.ApplyResources(this.GbData, "GbData");
            this.GbData.Name = "GbData";
            this.GbData.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownHeight = 200;
            this.comboBox1.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4")});
            this.comboBox1.Name = "comboBox1";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // TbDataDelimiter
            // 
            this.TbDataDelimiter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "processDataDelimeter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.TbDataDelimiter, "TbDataDelimiter");
            this.TbDataDelimiter.Name = "TbDataDelimiter";
            this.TbDataDelimiter.Text = global::GrblPlotter.Properties.Settings.Default.processDataDelimeter;
            // 
            // LblDataLoaded
            // 
            resources.ApplyResources(this.LblDataLoaded, "LblDataLoaded");
            this.LblDataLoaded.Name = "LblDataLoaded";
            // 
            // BtnLoadData
            // 
            resources.ApplyResources(this.BtnLoadData, "BtnLoadData");
            this.BtnLoadData.Name = "BtnLoadData";
            this.BtnLoadData.UseVisualStyleBackColor = true;
            this.BtnLoadData.Click += new System.EventHandler(this.BtnLoadData_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // NudDataIndex
            // 
            this.NudDataIndex.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "processDataIndex", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.NudDataIndex, "NudDataIndex");
            this.NudDataIndex.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudDataIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudDataIndex.Name = "NudDataIndex";
            this.NudDataIndex.Value = global::GrblPlotter.Properties.Settings.Default.processDataIndex;
            this.NudDataIndex.ValueChanged += new System.EventHandler(this.NudDataIndex_ValueChanged);
            // 
            // BtnDataIndexClear
            // 
            resources.ApplyResources(this.BtnDataIndexClear, "BtnDataIndexClear");
            this.BtnDataIndexClear.Name = "BtnDataIndexClear";
            this.BtnDataIndexClear.UseVisualStyleBackColor = true;
            this.BtnDataIndexClear.Click += new System.EventHandler(this.BtnDataIndexClear_Click);
            // 
            // GbControl
            // 
            this.GbControl.Controls.Add(this.BtnNew);
            this.GbControl.Controls.Add(this.BtnSave);
            this.GbControl.Controls.Add(this.BtnStart);
            this.GbControl.Controls.Add(this.LblInfo);
            this.GbControl.Controls.Add(this.BtnStop);
            this.GbControl.Controls.Add(this.BtnLoad);
            this.GbControl.Controls.Add(this.label3);
            this.GbControl.Controls.Add(this.LblLoaded);
            this.GbControl.Controls.Add(this.label2);
            this.GbControl.Controls.Add(this.label1);
            this.GbControl.Controls.Add(this.checkBox1);
            this.GbControl.Controls.Add(this.NudTimerInterval);
            this.GbControl.Controls.Add(this.LblCount);
            resources.ApplyResources(this.GbControl, "GbControl");
            this.GbControl.Name = "GbControl";
            this.GbControl.TabStop = false;
            // 
            // BtnNew
            // 
            resources.ApplyResources(this.BtnNew, "BtnNew");
            this.BtnNew.Name = "BtnNew";
            this.BtnNew.UseVisualStyleBackColor = true;
            this.BtnNew.Click += new System.EventHandler(this.BtnNew_Click);
            // 
            // BtnSave
            // 
            resources.ApplyResources(this.BtnSave, "BtnSave");
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // LblInfo
            // 
            this.LblInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.LblInfo, "LblInfo");
            this.LblInfo.Name = "LblInfo";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = global::GrblPlotter.Properties.Settings.Default.processOpenOnProgStart;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "processOpenOnProgStart", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // NudTimerInterval
            // 
            this.NudTimerInterval.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "processTimerInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.NudTimerInterval.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            resources.ApplyResources(this.NudTimerInterval, "NudTimerInterval");
            this.NudTimerInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NudTimerInterval.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.NudTimerInterval.Name = "NudTimerInterval";
            this.NudTimerInterval.Value = global::GrblPlotter.Properties.Settings.Default.processTimerInterval;
            // 
            // FctbData
            // 
            this.FctbData.AllowMacroRecording = false;
            this.FctbData.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.FctbData.AutoIndent = false;
            this.FctbData.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+\\s*(?<range>=)\\s*(?<range>[^;]+);";
            resources.ApplyResources(this.FctbData, "FctbData");
            this.FctbData.BackBrush = null;
            this.FctbData.CharCnWidth = 13;
            this.FctbData.CharHeight = 12;
            this.FctbData.CharWidth = 7;
            this.FctbData.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.FctbData.DelayedTextChangedInterval = 200;
            this.FctbData.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.FctbData.Hotkeys = resources.GetString("FctbData.Hotkeys");
            this.FctbData.IsReplaceMode = false;
            this.FctbData.Name = "FctbData";
            this.FctbData.Paddings = new System.Windows.Forms.Padding(0);
            this.FctbData.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.FctbData.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("FctbData.ServiceColors")));
            this.FctbData.ShowFoldingLines = true;
            this.FctbData.Zoom = 100;
            this.FctbData.Click += new System.EventHandler(this.FctbData_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // ProcessAutomation
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::GrblPlotter.Properties.Settings.Default, "processLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Location = global::GrblPlotter.Properties.Settings.Default.processLocation;
            this.Name = "ProcessAutomation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlProcessAutomation_FormClosing);
            this.Load += new System.EventHandler(this.ControlProcessAutomation_Load);
            this.SizeChanged += new System.EventHandler(this.ControlProcessAutomation_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.CmsDataGridEdit.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.GbCounter.ResumeLayout(false);
            this.GbCounter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDigitis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NudCounter)).EndInit();
            this.GbData.ResumeLayout(false);
            this.GbData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudDataIndex)).EndInit();
            this.GbControl.ResumeLayout(false);
            this.GbControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudTimerInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FctbData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox TbProcessInfo;
        private System.Windows.Forms.Label LblCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button BtnLoad;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label LblLoaded;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.NumericUpDown NudTimerInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button BtnDataIndexClear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown NudDataIndex;
        private FastColoredTextBoxNS.FastColoredTextBox FctbData;
        private System.Windows.Forms.GroupBox GbControl;
        private System.Windows.Forms.GroupBox GbData;
        private System.Windows.Forms.Button BtnLoadData;
        private System.Windows.Forms.GroupBox GbCounter;
        private System.Windows.Forms.TextBox TbCounterFront;
        private System.Windows.Forms.TextBox TbCounterRear;
        private System.Windows.Forms.NumericUpDown NudCounter;
        private System.Windows.Forms.NumericUpDown NudDigitis;
        private System.Windows.Forms.Label LblCounterResult;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TbFill;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label LblDataLoaded;
        private System.Windows.Forms.Button BtnNew;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TbDataDelimiter;
        private System.Windows.Forms.ContextMenuStrip CmsDataGridEdit;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveRowUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveRowDownToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBox1;
    }
}