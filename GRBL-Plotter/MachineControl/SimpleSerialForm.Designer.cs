namespace GrblPlotter
{
    partial class SimpleSerialForm
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
				timerSerial.Dispose();
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
            this.btnScanPort = new System.Windows.Forms.Button();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.cbBaud = new System.Windows.Forms.ComboBox();
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.cBCommand = new System.Windows.Forms.ComboBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tBMessageReady = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDTimeout = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nUDTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // btnScanPort
            // 
            this.btnScanPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnScanPort.Location = new System.Drawing.Point(207, 0);
            this.btnScanPort.Name = "btnScanPort";
            this.btnScanPort.Size = new System.Drawing.Size(73, 23);
            this.btnScanPort.TabIndex = 7;
            this.btnScanPort.Text = "Scan Ports";
            this.btnScanPort.UseVisualStyleBackColor = true;
            this.btnScanPort.Click += new System.EventHandler(this.BtnScanPort_Click);
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOpenPort.Location = new System.Drawing.Point(135, 0);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(73, 23);
            this.btnOpenPort.TabIndex = 6;
            this.btnOpenPort.Text = "Open";
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Click += new System.EventHandler(this.BtnOpenPort_Click);
            // 
            // cbBaud
            // 
            this.cbBaud.FormattingEnabled = true;
            this.cbBaud.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "250000"});
            this.cbBaud.Location = new System.Drawing.Point(70, 1);
            this.cbBaud.Name = "cbBaud";
            this.cbBaud.Size = new System.Drawing.Size(65, 21);
            this.cbBaud.TabIndex = 5;
            this.cbBaud.Text = "115200";
            // 
            // cbPort
            // 
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(2, 1);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(67, 21);
            this.cbPort.TabIndex = 4;
            this.cbPort.Text = "COM123";
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 115200;
            this.serialPort.ReadBufferSize = 2048;
            this.serialPort.ReadTimeout = 3000;
            this.serialPort.WriteTimeout = 3000;
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.SerialPort1_DataReceived);
            // 
            // rtbLog
            // 
            this.rtbLog.Font = new System.Drawing.Font("Lucida Console", 8F);
            this.rtbLog.Location = new System.Drawing.Point(2, 26);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(280, 278);
            this.rtbLog.TabIndex = 8;
            this.rtbLog.Text = "";
            this.rtbLog.WordWrap = false;
            // 
            // cBCommand
            // 
            this.cBCommand.FormattingEnabled = true;
            this.cBCommand.Location = new System.Drawing.Point(76, 311);
            this.cBCommand.Name = "cBCommand";
            this.cBCommand.Size = new System.Drawing.Size(137, 21);
            this.cBCommand.TabIndex = 19;
            this.cBCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TbCommand_KeyPress);
            // 
            // btnSend
            // 
            this.btnSend.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnSend.Location = new System.Drawing.Point(219, 310);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(61, 23);
            this.btnSend.TabIndex = 18;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.BtnSend_Click);
            // 
            // btnClear
            // 
            this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClear.Location = new System.Drawing.Point(2, 310);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(70, 23);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "Clear Log";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 342);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Busy-end Keyword";
            // 
            // tBMessageReady
            // 
            this.tBMessageReady.Location = new System.Drawing.Point(131, 339);
            this.tBMessageReady.Name = "tBMessageReady";
            this.tBMessageReady.Size = new System.Drawing.Size(50, 20);
            this.tBMessageReady.TabIndex = 21;
            this.tBMessageReady.Text = "ready";
            this.tBMessageReady.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tBMessageReady.TextChanged += new System.EventHandler(this.TbMessageReady_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 342);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Timeout";
            // 
            // nUDTimeout
            // 
            this.nUDTimeout.Location = new System.Drawing.Point(238, 339);
            this.nUDTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDTimeout.Name = "nUDTimeout";
            this.nUDTimeout.Size = new System.Drawing.Size(42, 20);
            this.nUDTimeout.TabIndex = 23;
            this.nUDTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nUDTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nUDTimeout.ValueChanged += new System.EventHandler(this.NudTimeout_ValueChanged);
            // 
            // SimpleSerialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 361);
            this.Controls.Add(this.nUDTimeout);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tBMessageReady);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cBCommand);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.btnScanPort);
            this.Controls.Add(this.btnOpenPort);
            this.Controls.Add(this.cbBaud);
            this.Controls.Add(this.cbPort);
            this.MinimumSize = new System.Drawing.Size(300, 400);
            this.Name = "SimpleSerialForm";
            this.Text = "3rd serial COM";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimpleSerialForm_FormClosing);
            this.Load += new System.EventHandler(this.SerialForm_Load);
            this.SizeChanged += new System.EventHandler(this.SimpleSerialForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.nUDTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnScanPort;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.ComboBox cbBaud;
        private System.Windows.Forms.ComboBox cbPort;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.ComboBox cBCommand;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tBMessageReady;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUDTimeout;
    }
}