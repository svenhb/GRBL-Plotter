namespace GRBL_Plotter
{
    partial class SerialForm2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialForm2));
            this.btnGRBLReset = new System.Windows.Forms.Button();
            this.btnGRBLCommand4 = new System.Windows.Forms.Button();
            this.toolTipSerial = new System.Windows.Forms.ToolTip(this.components);
            this.btnGRBLCommand3 = new System.Windows.Forms.Button();
            this.btnGRBLCommand2 = new System.Windows.Forms.Button();
            this.btnGRBLCommand1 = new System.Windows.Forms.Button();
            this.btnGRBLCommand0 = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.timerSerial = new System.Windows.Forms.Timer(this.components);
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.btnScanPort = new System.Windows.Forms.Button();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.cbBaud = new System.Windows.Forms.ComboBox();
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblPos = new System.Windows.Forms.Label();
            this.cBCommand = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnGRBLReset
            // 
            this.btnGRBLReset.Location = new System.Drawing.Point(190, 417);
            this.btnGRBLReset.Name = "btnGRBLReset";
            this.btnGRBLReset.Size = new System.Drawing.Size(89, 23);
            this.btnGRBLReset.TabIndex = 27;
            this.btnGRBLReset.Text = "RESET";
            this.btnGRBLReset.UseVisualStyleBackColor = true;
            this.btnGRBLReset.Click += new System.EventHandler(this.btnGRBLReset_Click);
            // 
            // btnGRBLCommand4
            // 
            this.btnGRBLCommand4.Location = new System.Drawing.Point(152, 417);
            this.btnGRBLCommand4.Name = "btnGRBLCommand4";
            this.btnGRBLCommand4.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand4.TabIndex = 26;
            this.btnGRBLCommand4.Text = "$X";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand4, resources.GetString("btnGRBLCommand4.ToolTip"));
            this.btnGRBLCommand4.UseVisualStyleBackColor = true;
            this.btnGRBLCommand4.Click += new System.EventHandler(this.btnGRBLCommand4_Click);
            // 
            // btnGRBLCommand3
            // 
            this.btnGRBLCommand3.Location = new System.Drawing.Point(114, 417);
            this.btnGRBLCommand3.Name = "btnGRBLCommand3";
            this.btnGRBLCommand3.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand3.TabIndex = 25;
            this.btnGRBLCommand3.Text = "$N";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand3, resources.GetString("btnGRBLCommand3.ToolTip"));
            this.btnGRBLCommand3.UseVisualStyleBackColor = true;
            this.btnGRBLCommand3.Click += new System.EventHandler(this.btnGRBLCommand3_Click);
            // 
            // btnGRBLCommand2
            // 
            this.btnGRBLCommand2.Location = new System.Drawing.Point(76, 417);
            this.btnGRBLCommand2.Name = "btnGRBLCommand2";
            this.btnGRBLCommand2.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand2.TabIndex = 24;
            this.btnGRBLCommand2.Text = "$#";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand2, resources.GetString("btnGRBLCommand2.ToolTip"));
            this.btnGRBLCommand2.UseVisualStyleBackColor = true;
            this.btnGRBLCommand2.Click += new System.EventHandler(this.btnGRBLCommand2_Click);
            // 
            // btnGRBLCommand1
            // 
            this.btnGRBLCommand1.Location = new System.Drawing.Point(38, 417);
            this.btnGRBLCommand1.Name = "btnGRBLCommand1";
            this.btnGRBLCommand1.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand1.TabIndex = 23;
            this.btnGRBLCommand1.Text = "$$";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand1, resources.GetString("btnGRBLCommand1.ToolTip"));
            this.btnGRBLCommand1.UseVisualStyleBackColor = true;
            this.btnGRBLCommand1.Click += new System.EventHandler(this.btnGRBLCommand1_Click);
            // 
            // btnGRBLCommand0
            // 
            this.btnGRBLCommand0.Location = new System.Drawing.Point(0, 417);
            this.btnGRBLCommand0.Name = "btnGRBLCommand0";
            this.btnGRBLCommand0.Size = new System.Drawing.Size(32, 23);
            this.btnGRBLCommand0.TabIndex = 22;
            this.btnGRBLCommand0.Text = "$";
            this.toolTipSerial.SetToolTip(this.btnGRBLCommand0, "Get list of all GRBL commands");
            this.btnGRBLCommand0.UseVisualStyleBackColor = true;
            this.btnGRBLCommand0.Click += new System.EventHandler(this.btnGRBLCommand0_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(236, 393);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(43, 23);
            this.btnSend.TabIndex = 21;
            this.btnSend.Text = "Send";
            this.toolTipSerial.SetToolTip(this.btnSend, "Press to send command");
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(0, 393);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(70, 23);
            this.btnClear.TabIndex = 19;
            this.btnClear.Text = "Clear Log";
            this.toolTipSerial.SetToolTip(this.btnClear, "Clear list above");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // timerSerial
            // 
            this.timerSerial.Enabled = true;
            this.timerSerial.Interval = 500;
            this.timerSerial.Tick += new System.EventHandler(this.timerSerial_Tick);
            // 
            // rtbLog
            // 
            this.rtbLog.Font = new System.Drawing.Font("Lucida Console", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.Location = new System.Drawing.Point(2, 50);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(280, 339);
            this.rtbLog.TabIndex = 18;
            this.rtbLog.Text = "";
            this.rtbLog.WordWrap = false;
            // 
            // serialPort2
            // 
            this.serialPort2.BaudRate = 115200;
            this.serialPort2.ReadBufferSize = 2048;
            this.serialPort2.ReadTimeout = 3000;
            this.serialPort2.WriteTimeout = 3000;
            this.serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort2_DataReceived);
            // 
            // btnScanPort
            // 
            this.btnScanPort.Location = new System.Drawing.Point(206, 0);
            this.btnScanPort.Name = "btnScanPort";
            this.btnScanPort.Size = new System.Drawing.Size(55, 23);
            this.btnScanPort.TabIndex = 17;
            this.btnScanPort.Text = "Scan";
            this.btnScanPort.UseVisualStyleBackColor = true;
            this.btnScanPort.Click += new System.EventHandler(this.btnScanPort_Click);
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.Location = new System.Drawing.Point(145, 0);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(55, 23);
            this.btnOpenPort.TabIndex = 16;
            this.btnOpenPort.Text = "Open";
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
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
            this.cbBaud.Location = new System.Drawing.Point(74, 2);
            this.cbBaud.Name = "cbBaud";
            this.cbBaud.Size = new System.Drawing.Size(65, 21);
            this.cbBaud.TabIndex = 15;
            this.cbBaud.Text = "115200";
            // 
            // cbPort
            // 
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(1, 2);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(67, 21);
            this.cbPort.TabIndex = 14;
            this.cbPort.Text = "COM123";
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(2, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(65, 21);
            this.lblStatus.TabIndex = 28;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPos
            // 
            this.lblPos.AutoSize = true;
            this.lblPos.Location = new System.Drawing.Point(71, 30);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(35, 13);
            this.lblPos.TabIndex = 29;
            this.lblPos.Text = "label1";
            // 
            // cBCommand
            // 
            this.cBCommand.FormattingEnabled = true;
            this.cBCommand.Items.AddRange(new object[] {
            "$H (Homing)",
            "G90 G1 X1 F500 (absolute)",
            "G91 G1 X1 F500 (relarive)"});
            this.cBCommand.Location = new System.Drawing.Point(76, 395);
            this.cBCommand.Name = "cBCommand";
            this.cBCommand.Size = new System.Drawing.Size(156, 21);
            this.cBCommand.TabIndex = 30;
            this.cBCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCommand_KeyPress);
            // 
            // SerialForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 441);
            this.Controls.Add(this.cBCommand);
            this.Controls.Add(this.lblPos);
            this.Controls.Add(this.lblStatus);
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
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::GRBL_Plotter.Properties.Settings.Default, "locationSerForm2", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::GRBL_Plotter.Properties.Settings.Default.locationSerForm2;
            this.MinimumSize = new System.Drawing.Size(300, 480);
            this.Name = "SerialForm2";
            this.Text = "COM Tool changer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialForm_FormClosing);
            this.Load += new System.EventHandler(this.SerialForm_Load);
            this.Resize += new System.EventHandler(this.SerialForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGRBLReset;
        private System.Windows.Forms.Button btnGRBLCommand4;
        private System.Windows.Forms.ToolTip toolTipSerial;
        private System.Windows.Forms.Button btnGRBLCommand3;
        private System.Windows.Forms.Button btnGRBLCommand2;
        private System.Windows.Forms.Button btnGRBLCommand1;
        private System.Windows.Forms.Button btnGRBLCommand0;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Timer timerSerial;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.Button btnScanPort;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.ComboBox cbBaud;
        private System.Windows.Forms.ComboBox cbPort;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblPos;
        private System.Windows.Forms.ComboBox cBCommand;
    }
}