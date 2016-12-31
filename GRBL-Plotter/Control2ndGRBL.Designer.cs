namespace GRBL_Plotter
{
    partial class Control2ndGRBL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Control2ndGRBL));
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnJogZeroZ = new System.Windows.Forms.Button();
            this.btnJogZeroY = new System.Windows.Forms.Button();
            this.btnJogZeroX = new System.Windows.Forms.Button();
            this.virtualJoystickY = new virtualJoystick.virtualJoystick();
            this.virtualJoystickX = new virtualJoystick.virtualJoystick();
            this.btnJogStop = new System.Windows.Forms.Button();
            this.virtualJoystickZ = new virtualJoystick.virtualJoystick();
            this.btnKillAlarm = new System.Windows.Forms.Button();
            this.btnFeedHold = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnZeroZ = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnZeroY = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnZeroX = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.label_mx = new System.Windows.Forms.Label();
            this.label_my = new System.Windows.Forms.Label();
            this.label_mz = new System.Windows.Forms.Label();
            this.label_wz = new System.Windows.Forms.Label();
            this.label_wx = new System.Windows.Forms.Label();
            this.label_wy = new System.Windows.Forms.Label();
            this.groupBox6.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnJogZeroZ);
            this.groupBox6.Controls.Add(this.btnJogZeroY);
            this.groupBox6.Controls.Add(this.btnJogZeroX);
            this.groupBox6.Controls.Add(this.virtualJoystickY);
            this.groupBox6.Controls.Add(this.virtualJoystickX);
            this.groupBox6.Controls.Add(this.btnJogStop);
            this.groupBox6.Controls.Add(this.virtualJoystickZ);
            this.groupBox6.Location = new System.Drawing.Point(2, 148);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(159, 266);
            this.groupBox6.TabIndex = 21;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Control / Jogging";
            // 
            // btnJogZeroZ
            // 
            this.btnJogZeroZ.Location = new System.Drawing.Point(107, 53);
            this.btnJogZeroZ.Name = "btnJogZeroZ";
            this.btnJogZeroZ.Size = new System.Drawing.Size(45, 20);
            this.btnJogZeroZ.TabIndex = 4;
            this.btnJogZeroZ.Text = "Z=0";
            this.btnJogZeroZ.UseVisualStyleBackColor = true;
            this.btnJogZeroZ.Click += new System.EventHandler(this.btnJogZ_Click);
            // 
            // btnJogZeroY
            // 
            this.btnJogZeroY.Location = new System.Drawing.Point(55, 53);
            this.btnJogZeroY.Name = "btnJogZeroY";
            this.btnJogZeroY.Size = new System.Drawing.Size(45, 20);
            this.btnJogZeroY.TabIndex = 3;
            this.btnJogZeroY.Text = "Y=0";
            this.btnJogZeroY.UseVisualStyleBackColor = true;
            this.btnJogZeroY.Click += new System.EventHandler(this.btnJogY_Click);
            // 
            // btnJogZeroX
            // 
            this.btnJogZeroX.Location = new System.Drawing.Point(4, 53);
            this.btnJogZeroX.Name = "btnJogZeroX";
            this.btnJogZeroX.Size = new System.Drawing.Size(45, 20);
            this.btnJogZeroX.TabIndex = 2;
            this.btnJogZeroX.Text = "X=0";
            this.btnJogZeroX.UseVisualStyleBackColor = true;
            this.btnJogZeroX.Click += new System.EventHandler(this.btnJogX_Click);
            // 
            // virtualJoystickY
            // 
            this.virtualJoystickY.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("virtualJoystickY.BackgroundImage")));
            this.virtualJoystickY.Joystick2Dimension = false;
            this.virtualJoystickY.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickY.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickY.JoystickRaster = 5;
            this.virtualJoystickY.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickY.Location = new System.Drawing.Point(55, 79);
            this.virtualJoystickY.MaximumSize = new System.Drawing.Size(400, 400);
            this.virtualJoystickY.MinimumSize = new System.Drawing.Size(25, 100);
            this.virtualJoystickY.Name = "virtualJoystickY";
            this.virtualJoystickY.Size = new System.Drawing.Size(45, 180);
            this.virtualJoystickY.TabIndex = 30;
            this.virtualJoystickY.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickY_JoyStickEvent);
            this.virtualJoystickY.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickY.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // virtualJoystickX
            // 
            this.virtualJoystickX.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("virtualJoystickX.BackgroundImage")));
            this.virtualJoystickX.Joystick2Dimension = false;
            this.virtualJoystickX.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickX.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickX.JoystickRaster = 5;
            this.virtualJoystickX.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickX.Location = new System.Drawing.Point(4, 79);
            this.virtualJoystickX.MaximumSize = new System.Drawing.Size(400, 400);
            this.virtualJoystickX.MinimumSize = new System.Drawing.Size(25, 100);
            this.virtualJoystickX.Name = "virtualJoystickX";
            this.virtualJoystickX.Size = new System.Drawing.Size(45, 180);
            this.virtualJoystickX.TabIndex = 29;
            this.virtualJoystickX.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickX_JoyStickEvent);
            this.virtualJoystickX.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickX.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // btnJogStop
            // 
            this.btnJogStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnJogStop.Location = new System.Drawing.Point(4, 19);
            this.btnJogStop.Name = "btnJogStop";
            this.btnJogStop.Size = new System.Drawing.Size(148, 28);
            this.btnJogStop.TabIndex = 28;
            this.btnJogStop.Text = "STOP Jogging";
            this.btnJogStop.UseVisualStyleBackColor = false;
            this.btnJogStop.Click += new System.EventHandler(this.btnJogStop_Click);
            // 
            // virtualJoystickZ
            // 
            this.virtualJoystickZ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("virtualJoystickZ.BackgroundImage")));
            this.virtualJoystickZ.Joystick2Dimension = false;
            this.virtualJoystickZ.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickZ.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickZ.JoystickRaster = 5;
            this.virtualJoystickZ.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickZ.Location = new System.Drawing.Point(107, 79);
            this.virtualJoystickZ.MaximumSize = new System.Drawing.Size(400, 400);
            this.virtualJoystickZ.MinimumSize = new System.Drawing.Size(25, 100);
            this.virtualJoystickZ.Name = "virtualJoystickZ";
            this.virtualJoystickZ.Size = new System.Drawing.Size(45, 180);
            this.virtualJoystickZ.TabIndex = 24;
            this.virtualJoystickZ.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickZ_JoyStickEvent);
            this.virtualJoystickZ.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickZ.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // btnKillAlarm
            // 
            this.btnKillAlarm.Location = new System.Drawing.Point(167, 325);
            this.btnKillAlarm.Name = "btnKillAlarm";
            this.btnKillAlarm.Size = new System.Drawing.Size(113, 40);
            this.btnKillAlarm.TabIndex = 18;
            this.btnKillAlarm.Text = "Kill Alarm";
            this.btnKillAlarm.UseVisualStyleBackColor = true;
            this.btnKillAlarm.Click += new System.EventHandler(this.btnKillAlarm_Click);
            // 
            // btnFeedHold
            // 
            this.btnFeedHold.Location = new System.Drawing.Point(167, 167);
            this.btnFeedHold.Name = "btnFeedHold";
            this.btnFeedHold.Size = new System.Drawing.Size(113, 40);
            this.btnFeedHold.TabIndex = 16;
            this.btnFeedHold.Text = "Feed Hold";
            this.btnFeedHold.UseVisualStyleBackColor = true;
            this.btnFeedHold.Click += new System.EventHandler(this.btnFeedHold_Click);
            // 
            // btnResume
            // 
            this.btnResume.Location = new System.Drawing.Point(167, 213);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(113, 40);
            this.btnResume.TabIndex = 17;
            this.btnResume.Text = "Resume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(167, 259);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(113, 60);
            this.btnReset.TabIndex = 15;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnHome);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnZeroZ);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnZeroY);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnZeroX);
            this.groupBox2.Controls.Add(this.label_status);
            this.groupBox2.Controls.Add(this.label_mx);
            this.groupBox2.Controls.Add(this.label_my);
            this.groupBox2.Controls.Add(this.label_mz);
            this.groupBox2.Controls.Add(this.label_wz);
            this.groupBox2.Controls.Add(this.label_wx);
            this.groupBox2.Controls.Add(this.label_wy);
            this.groupBox2.Location = new System.Drawing.Point(2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(193, 140);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tool Coordinates (World / Machine)";
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(122, 107);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(63, 30);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label5.Location = new System.Drawing.Point(0, 98);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(62, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Status:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label4.Location = new System.Drawing.Point(13, 78);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(25, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Z";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnZeroZ
            // 
            this.btnZeroZ.Location = new System.Drawing.Point(122, 78);
            this.btnZeroZ.Name = "btnZeroZ";
            this.btnZeroZ.Size = new System.Drawing.Size(63, 23);
            this.btnZeroZ.TabIndex = 3;
            this.btnZeroZ.Text = "Zero Z";
            this.btnZeroZ.UseVisualStyleBackColor = true;
            this.btnZeroZ.Click += new System.EventHandler(this.btnZeroZ_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label3.Location = new System.Drawing.Point(15, 45);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(23, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Y";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnZeroY
            // 
            this.btnZeroY.Location = new System.Drawing.Point(122, 45);
            this.btnZeroY.Name = "btnZeroY";
            this.btnZeroY.Size = new System.Drawing.Size(63, 23);
            this.btnZeroY.TabIndex = 2;
            this.btnZeroY.Text = "Zero Y";
            this.btnZeroY.UseVisualStyleBackColor = true;
            this.btnZeroY.Click += new System.EventHandler(this.btnZeroY_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label2.Location = new System.Drawing.Point(15, 13);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(23, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "X";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnZeroX
            // 
            this.btnZeroX.Location = new System.Drawing.Point(122, 12);
            this.btnZeroX.Name = "btnZeroX";
            this.btnZeroX.Size = new System.Drawing.Size(63, 23);
            this.btnZeroX.TabIndex = 1;
            this.btnZeroX.Text = "Zero X";
            this.btnZeroX.UseVisualStyleBackColor = true;
            this.btnZeroX.Click += new System.EventHandler(this.btnZeroX_Click);
            // 
            // label_status
            // 
            this.label_status.AutoSize = true;
            this.label_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_status.Location = new System.Drawing.Point(2, 119);
            this.label_status.Name = "label_status";
            this.label_status.Size = new System.Drawing.Size(56, 20);
            this.label_status.TabIndex = 7;
            this.label_status.Text = "Status";
            // 
            // label_mx
            // 
            this.label_mx.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label_mx.Location = new System.Drawing.Point(48, 31);
            this.label_mx.Name = "label_mx";
            this.label_mx.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_mx.Size = new System.Drawing.Size(72, 12);
            this.label_mx.TabIndex = 1;
            this.label_mx.Text = "0000.000";
            this.label_mx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_my
            // 
            this.label_my.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label_my.Location = new System.Drawing.Point(48, 63);
            this.label_my.Name = "label_my";
            this.label_my.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_my.Size = new System.Drawing.Size(72, 14);
            this.label_my.TabIndex = 2;
            this.label_my.Text = "0000.000";
            this.label_my.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_mz
            // 
            this.label_mz.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label_mz.Location = new System.Drawing.Point(48, 96);
            this.label_mz.Name = "label_mz";
            this.label_mz.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_mz.Size = new System.Drawing.Size(72, 14);
            this.label_mz.TabIndex = 3;
            this.label_mz.Text = "0000.000";
            this.label_mz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_wz
            // 
            this.label_wz.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_wz.Location = new System.Drawing.Point(44, 78);
            this.label_wz.Name = "label_wz";
            this.label_wz.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_wz.Size = new System.Drawing.Size(76, 20);
            this.label_wz.TabIndex = 6;
            this.label_wz.Text = "0000.000";
            this.label_wz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_wx
            // 
            this.label_wx.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_wx.Location = new System.Drawing.Point(44, 13);
            this.label_wx.Name = "label_wx";
            this.label_wx.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_wx.Size = new System.Drawing.Size(76, 20);
            this.label_wx.TabIndex = 4;
            this.label_wx.Text = "0000.000";
            this.label_wx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_wy
            // 
            this.label_wy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label_wy.Location = new System.Drawing.Point(44, 45);
            this.label_wy.Name = "label_wy";
            this.label_wy.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_wy.Size = new System.Drawing.Size(76, 20);
            this.label_wy.TabIndex = 5;
            this.label_wy.Text = "0000.000";
            this.label_wy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Control2ndGRBL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 421);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.btnFeedHold);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnKillAlarm);
            this.MinimumSize = new System.Drawing.Size(300, 460);
            this.Name = "Control2ndGRBL";
            this.Text = "Control2ndGRBL";
            this.Load += new System.EventHandler(this.Control2ndGRBL_Load);
            this.groupBox6.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnJogStop;
        private System.Windows.Forms.Button btnJogZeroX;
        private System.Windows.Forms.Button btnJogZeroY;
        private System.Windows.Forms.Button btnJogZeroZ;
        private virtualJoystick.virtualJoystick virtualJoystickZ;
        private System.Windows.Forms.Button btnKillAlarm;
        private System.Windows.Forms.Button btnFeedHold;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnReset;
        private virtualJoystick.virtualJoystick virtualJoystickY;
        private virtualJoystick.virtualJoystick virtualJoystickX;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnZeroZ;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnZeroY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnZeroX;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label label_mx;
        private System.Windows.Forms.Label label_my;
        private System.Windows.Forms.Label label_mz;
        private System.Windows.Forms.Label label_wz;
        private System.Windows.Forms.Label label_wx;
        private System.Windows.Forms.Label label_wy;
    }
}