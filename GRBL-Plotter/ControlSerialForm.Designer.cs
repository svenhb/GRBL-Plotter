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
    partial class ControlSerialForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlSerialForm));
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.cbBaud = new System.Windows.Forms.ComboBox();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.btnScanPort = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnGRBLCommand0 = new System.Windows.Forms.Button();
            this.btnGRBLCommand1 = new System.Windows.Forms.Button();
            this.btnGRBLCommand2 = new System.Windows.Forms.Button();
            this.btnGRBLCommand3 = new System.Windows.Forms.Button();
            this.toolTipSerial = new System.Windows.Forms.ToolTip(this.components);
            this.btnGRBLCommand4 = new System.Windows.Forms.Button();
            this.lblSrPos = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSrBf = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSrFS = new System.Windows.Forms.Label();
            this.lblSrPn = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblSrOv = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblSrA = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblSrLn = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.timerSerial = new System.Windows.Forms.Timer(this.components);
            this.btnGRBLReset = new System.Windows.Forms.Button();
            this.lblSrState = new System.Windows.Forms.Label();
            this.cBCommand = new System.Windows.Forms.ComboBox();
            this.cbStatus = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbPort
            // 
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(2, 2);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(67, 21);
            this.cbPort.TabIndex = 0;
            this.cbPort.Text = "COM123";
            this.toolTipSerial.SetToolTip(this.cbPort, "Select free serial port");
            // 
            // cbBaud
            // 
            this.cbBaud.FormattingEnabled = true;
            this.cbBaud.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cbBaud.Location = new System.Drawing.Point(70, 2);
            this.cbBaud.Name = "cbBaud";
            this.cbBaud.Size = new System.Drawing.Size(65, 21);
            this.cbBaud.TabIndex = 1;
            this.cbBaud.Text = "115200";
            this.toolTipSerial.SetToolTip(this.cbBaud, "Select speed");
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.Location = new System.Drawing.Point(135, 1);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(73, 23);
            this.btnOpenPort.TabIndex = 2;
            this.btnOpenPort.Text = "Open";
            this.toolTipSerial.SetToolTip(this.btnOpenPort, "Open selected COM port with selected speed");
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
            // 
            // btnScanPort
            // 
            this.btnScanPort.Location = new System.Drawing.Point(207, 1);
            this.btnScanPort.Name = "btnScanPort";
            this.btnScanPort.Size = new System.Drawing.Size(73, 23);
            this.btnScanPort.TabIndex = 3;
            this.btnScanPort.Text = "Scan Ports";
            this.toolTipSerial.SetToolTip(this.btnScanPort, "Scan for free ports");
            this.btnScanPort.UseVisualStyleBackColor = true;
            this.btnScanPort.Click += new System.EventHandler(this.btnScanPort_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.ReadBufferSize = 2048;
            this.serialPort1.ReadTimeout = 3000;
            this.serialPort1.WriteTimeout = 3000;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // rtbLog
            // 
            this.rtbLog.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.Location = new System.Drawing.Point(2, 117);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(280, 270);
            this.rtbLog.TabIndex = 4;
            this.rtbLog.Text = "";
            this.rtbLog.WordWrap = false;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(1, 393);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(70, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear Log";
            this.toolTipSerial.SetToolTip(this.btnClear, "Clear list above");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(237, 393);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(43, 23);
            this.btnSend.TabIndex = 7;
            this.btnSend.Text = "Send";
            this.toolTipSerial.SetToolTip(this.btnSend, "Press to send command");
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnGRBLCommand0
            // 
            this.btnGRBLCommand0.Location = new System.Drawing.Point(1, 417);
            this.btnGRBLCommand0.Name = "btnGRBLCommand0";
            this.btnGRBLCommand0.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand0.TabIndex = 8;
            this.btnGRBLCommand0.Text = "$";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand0, "Get list of all GRBL commands");
            this.btnGRBLCommand0.UseVisualStyleBackColor = true;
            this.btnGRBLCommand0.Click += new System.EventHandler(this.btnGRBLCommand0_Click);
            // 
            // btnGRBLCommand1
            // 
            this.btnGRBLCommand1.Location = new System.Drawing.Point(39, 417);
            this.btnGRBLCommand1.Name = "btnGRBLCommand1";
            this.btnGRBLCommand1.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand1.TabIndex = 9;
            this.btnGRBLCommand1.Text = "$$";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand1, resources.GetString("btnGRBLCommand1.ToolTip"));
            this.btnGRBLCommand1.UseVisualStyleBackColor = true;
            this.btnGRBLCommand1.Click += new System.EventHandler(this.btnGRBLCommand1_Click);
            // 
            // btnGRBLCommand2
            // 
            this.btnGRBLCommand2.Location = new System.Drawing.Point(77, 417);
            this.btnGRBLCommand2.Name = "btnGRBLCommand2";
            this.btnGRBLCommand2.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand2.TabIndex = 10;
            this.btnGRBLCommand2.Text = "$#";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand2, resources.GetString("btnGRBLCommand2.ToolTip"));
            this.btnGRBLCommand2.UseVisualStyleBackColor = true;
            this.btnGRBLCommand2.Click += new System.EventHandler(this.btnGRBLCommand2_Click);
            // 
            // btnGRBLCommand3
            // 
            this.btnGRBLCommand3.Location = new System.Drawing.Point(115, 417);
            this.btnGRBLCommand3.Name = "btnGRBLCommand3";
            this.btnGRBLCommand3.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand3.TabIndex = 11;
            this.btnGRBLCommand3.Text = "$N";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand3, resources.GetString("btnGRBLCommand3.ToolTip"));
            this.btnGRBLCommand3.UseVisualStyleBackColor = true;
            this.btnGRBLCommand3.Click += new System.EventHandler(this.btnGRBLCommand3_Click);
            // 
            // btnGRBLCommand4
            // 
            this.btnGRBLCommand4.Location = new System.Drawing.Point(153, 417);
            this.btnGRBLCommand4.Name = "btnGRBLCommand4";
            this.btnGRBLCommand4.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand4.TabIndex = 12;
            this.btnGRBLCommand4.Text = "$X";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand4, resources.GetString("btnGRBLCommand4.ToolTip"));
            this.btnGRBLCommand4.UseVisualStyleBackColor = true;
            this.btnGRBLCommand4.Click += new System.EventHandler(this.btnGRBLCommand4_Click);
            // 
            // lblSrPos
            // 
            this.lblSrPos.AutoSize = true;
            this.lblSrPos.Location = new System.Drawing.Point(65, 20);
            this.lblSrPos.Name = "lblSrPos";
            this.lblSrPos.Size = new System.Drawing.Size(103, 13);
            this.lblSrPos.TabIndex = 15;
            this.lblSrPos.Text = "0.000,-10.000,5.000";
            this.toolTipSerial.SetToolTip(this.lblSrPos, "Work Position");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Bf:";
            this.toolTipSerial.SetToolTip(this.label2, "Buffer State");
            // 
            // lblSrBf
            // 
            this.lblSrBf.AutoSize = true;
            this.lblSrBf.Location = new System.Drawing.Point(23, 37);
            this.lblSrBf.Name = "lblSrBf";
            this.lblSrBf.Size = new System.Drawing.Size(40, 13);
            this.lblSrBf.TabIndex = 20;
            this.lblSrBf.Text = "15,128";
            this.toolTipSerial.SetToolTip(this.lblSrBf, "Buffer State");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(70, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "FS:";
            this.toolTipSerial.SetToolTip(this.label4, "Current Feed and Speed");
            // 
            // lblSrFS
            // 
            this.lblSrFS.AutoSize = true;
            this.lblSrFS.Location = new System.Drawing.Point(94, 37);
            this.lblSrFS.Name = "lblSrFS";
            this.lblSrFS.Size = new System.Drawing.Size(52, 13);
            this.lblSrFS.TabIndex = 22;
            this.lblSrFS.Text = "500,8000";
            this.toolTipSerial.SetToolTip(this.lblSrFS, "Current Feed and Speed");
            // 
            // lblSrPn
            // 
            this.lblSrPn.AutoSize = true;
            this.lblSrPn.Location = new System.Drawing.Point(200, 37);
            this.lblSrPn.Name = "lblSrPn";
            this.lblSrPn.Size = new System.Drawing.Size(66, 13);
            this.lblSrPn.TabIndex = 24;
            this.lblSrPn.Text = "XYZPDHRS";
            this.toolTipSerial.SetToolTip(this.lblSrPn, "Input Pin State");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(174, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Pn:";
            this.toolTipSerial.SetToolTip(this.label7, "Input Pin State");
            // 
            // lblSrOv
            // 
            this.lblSrOv.AutoSize = true;
            this.lblSrOv.Location = new System.Drawing.Point(94, 50);
            this.lblSrOv.Name = "lblSrOv";
            this.lblSrOv.Size = new System.Drawing.Size(67, 13);
            this.lblSrOv.TabIndex = 26;
            this.lblSrOv.Text = "100,100,100";
            this.toolTipSerial.SetToolTip(this.lblSrOv, "Override Values");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(70, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Ov:";
            this.toolTipSerial.SetToolTip(this.label9, "Override Values");
            // 
            // lblSrA
            // 
            this.lblSrA.AutoSize = true;
            this.lblSrA.Location = new System.Drawing.Point(200, 50);
            this.lblSrA.Name = "lblSrA";
            this.lblSrA.Size = new System.Drawing.Size(29, 13);
            this.lblSrA.TabIndex = 28;
            this.lblSrA.Text = "SFM";
            this.toolTipSerial.SetToolTip(this.lblSrA, "Accessory State");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(174, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 27;
            this.label11.Text = "A:";
            this.toolTipSerial.SetToolTip(this.label11, "Accessory State");
            // 
            // lblSrLn
            // 
            this.lblSrLn.AutoSize = true;
            this.lblSrLn.Location = new System.Drawing.Point(23, 50);
            this.lblSrLn.Name = "lblSrLn";
            this.lblSrLn.Size = new System.Drawing.Size(37, 13);
            this.lblSrLn.TabIndex = 30;
            this.lblSrLn.Text = "99999";
            this.toolTipSerial.SetToolTip(this.lblSrLn, "Line Number");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(2, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(22, 13);
            this.label13.TabIndex = 29;
            this.label13.Text = "Ln:";
            this.toolTipSerial.SetToolTip(this.label13, "Line Number");
            // 
            // timerSerial
            // 
            this.timerSerial.Enabled = true;
            this.timerSerial.Interval = 500;
            this.timerSerial.Tick += new System.EventHandler(this.timerSerial_Tick);
            // 
            // btnGRBLReset
            // 
            this.btnGRBLReset.Location = new System.Drawing.Point(191, 417);
            this.btnGRBLReset.Name = "btnGRBLReset";
            this.btnGRBLReset.Size = new System.Drawing.Size(89, 23);
            this.btnGRBLReset.TabIndex = 13;
            this.btnGRBLReset.Text = "RESET";
            this.btnGRBLReset.UseVisualStyleBackColor = true;
            this.btnGRBLReset.Click += new System.EventHandler(this.btnGRBLReset_Click);
            // 
            // lblSrState
            // 
            this.lblSrState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSrState.Location = new System.Drawing.Point(2, 16);
            this.lblSrState.Name = "lblSrState";
            this.lblSrState.Size = new System.Drawing.Size(65, 21);
            this.lblSrState.TabIndex = 14;
            this.lblSrState.Text = "Status";
            this.lblSrState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cBCommand
            // 
            this.cBCommand.FormattingEnabled = true;
            this.cBCommand.Items.AddRange(new object[] {
            "$H (Homing)",
            "G90 G1 X1 F500 (absolute)",
            "G91 G1 X1 F500 (relarive)"});
            this.cBCommand.Location = new System.Drawing.Point(75, 395);
            this.cBCommand.Name = "cBCommand";
            this.cBCommand.Size = new System.Drawing.Size(156, 21);
            this.cBCommand.TabIndex = 16;
            this.cBCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCommand_KeyPress);
            // 
            // cbStatus
            // 
            this.cbStatus.AutoSize = true;
            this.cbStatus.Location = new System.Drawing.Point(5, 66);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(168, 17);
            this.cbStatus.TabIndex = 17;
            this.cbStatus.Text = "Show Real-time Status Report";
            this.cbStatus.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbStatus);
            this.groupBox1.Controls.Add(this.lblSrA);
            this.groupBox1.Controls.Add(this.lblSrLn);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.lblSrState);
            this.groupBox1.Controls.Add(this.lblSrOv);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblSrPn);
            this.groupBox1.Controls.Add(this.lblSrPos);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblSrFS);
            this.groupBox1.Controls.Add(this.lblSrBf);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(4, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 84);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Real-time Status Report";
            // 
            // SerialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 441);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cBCommand);
            this.Controls.Add(this.btnGRBLReset);
            this.Controls.Add(this.btnGRBLCommand4);
            this.Controls.Add(this.btnGRBLCommand3);
            this.Controls.Add(this.btnGRBLCommand2);
            this.Controls.Add(this.btnGRBLCommand1);
            this.Controls.Add(this.btnGRBLCommand0);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.btnScanPort);
            this.Controls.Add(this.btnOpenPort);
            this.Controls.Add(this.cbBaud);
            this.Controls.Add(this.cbPort);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 480);
            this.Name = "SerialForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "COM C";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialForm_FormClosing);
            this.Load += new System.EventHandler(this.SerialForm_Load);
            this.Resize += new System.EventHandler(this.SerialForm_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPort;
        private System.Windows.Forms.ComboBox cbBaud;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.Button btnScanPort;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnGRBLCommand0;
        private System.Windows.Forms.Button btnGRBLCommand1;
        private System.Windows.Forms.ToolTip toolTipSerial;
        private System.Windows.Forms.Button btnGRBLCommand2;
        private System.Windows.Forms.Button btnGRBLCommand3;
        private System.Windows.Forms.Timer timerSerial;
        private System.Windows.Forms.Button btnGRBLCommand4;
        private System.Windows.Forms.Button btnGRBLReset;
        private System.Windows.Forms.Label lblSrState;
        private System.Windows.Forms.Label lblSrPos;
        private System.Windows.Forms.ComboBox cBCommand;
        private System.Windows.Forms.CheckBox cbStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSrBf;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSrFS;
        private System.Windows.Forms.Label lblSrPn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblSrOv;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblSrA;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblSrLn;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}