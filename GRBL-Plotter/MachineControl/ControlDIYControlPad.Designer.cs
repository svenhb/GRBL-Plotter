namespace GRBL_Plotter
{
    partial class ControlDIYControlPad
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
            this.btnScanPort = new System.Windows.Forms.Button();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.cbBaud = new System.Windows.Forms.ComboBox();
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cBFeedback = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnScanPort
            // 
            this.btnScanPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnScanPort.Location = new System.Drawing.Point(207, 1);
            this.btnScanPort.Name = "btnScanPort";
            this.btnScanPort.Size = new System.Drawing.Size(73, 23);
            this.btnScanPort.TabIndex = 7;
            this.btnScanPort.Text = "Scan Ports";
            this.toolTip1.SetToolTip(this.btnScanPort, "Scan for free ports");
            this.btnScanPort.UseVisualStyleBackColor = true;
            this.btnScanPort.Click += new System.EventHandler(this.btnScanPort_Click);
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOpenPort.Location = new System.Drawing.Point(135, 1);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(73, 23);
            this.btnOpenPort.TabIndex = 6;
            this.btnOpenPort.Text = "Open";
            this.toolTip1.SetToolTip(this.btnOpenPort, "Open selected COM port with selected speed");
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
            this.cbBaud.Location = new System.Drawing.Point(70, 2);
            this.cbBaud.Name = "cbBaud";
            this.cbBaud.Size = new System.Drawing.Size(65, 21);
            this.cbBaud.TabIndex = 5;
            this.cbBaud.Text = "115200";
            this.toolTip1.SetToolTip(this.cbBaud, "Select speed");
            // 
            // cbPort
            // 
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(2, 2);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(67, 21);
            this.cbPort.TabIndex = 4;
            this.cbPort.Text = "COM123";
            this.toolTip1.SetToolTip(this.cbPort, "Select free serial port");
            // 
            // rtbLog
            // 
            this.rtbLog.Font = new System.Drawing.Font("Lucida Console", 8F);
            this.rtbLog.Location = new System.Drawing.Point(2, 48);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(280, 208);
            this.rtbLog.TabIndex = 8;
            this.rtbLog.Text = "";
            this.rtbLog.WordWrap = false;
            // 
            // serialPort
            // 
            this.serialPort.RtsEnable = true;
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            // 
            // cBFeedback
            // 
            this.cBFeedback.AutoSize = true;
            this.cBFeedback.Checked = global::GRBL_Plotter.Properties.Settings.Default.feedbackDIY;
            this.cBFeedback.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBFeedback.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "feedbackDIY", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBFeedback.Location = new System.Drawing.Point(2, 25);
            this.cBFeedback.Name = "cBFeedback";
            this.cBFeedback.Size = new System.Drawing.Size(164, 17);
            this.cBFeedback.TabIndex = 9;
            this.cBFeedback.Text = "show GRBL-Plotter feedback";
            this.cBFeedback.UseVisualStyleBackColor = true;
            // 
            // ControlDIYControlPad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.cBFeedback);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.btnScanPort);
            this.Controls.Add(this.btnOpenPort);
            this.Controls.Add(this.cbBaud);
            this.Controls.Add(this.cbPort);
            this.Name = "ControlDIYControlPad";
            this.Text = "ControlDIYControlPad";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlDIYControlPad_FormClosing);
            this.Load += new System.EventHandler(this.ControlDIYControlPad_Load);
            this.Resize += new System.EventHandler(this.ControlDIYControlPad_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnScanPort;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.ComboBox cbBaud;
        private System.Windows.Forms.ComboBox cbPort;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cBFeedback;
    }
}