namespace GrblPlotter
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
            this.ucJogControludZ = new GrblPlotter.UserControls.UCJogControlUD();
            this.ucJogControludY = new GrblPlotter.UserControls.UCJogControlUD();
            this.ucJogControludX = new GrblPlotter.UserControls.UCJogControlUD();
            this.btnJogZeroZ = new System.Windows.Forms.Button();
            this.btnJogZeroY = new System.Windows.Forms.Button();
            this.btnJogZeroX = new System.Windows.Forms.Button();
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
            this.groupBox6.Controls.Add(this.ucJogControludZ);
            this.groupBox6.Controls.Add(this.ucJogControludY);
            this.groupBox6.Controls.Add(this.ucJogControludX);
            this.groupBox6.Controls.Add(this.btnJogZeroZ);
            this.groupBox6.Controls.Add(this.btnJogZeroY);
            this.groupBox6.Controls.Add(this.btnJogZeroX);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // ucJogControludZ
            // 
            resources.ApplyResources(this.ucJogControludZ, "ucJogControludZ");
            this.ucJogControludZ.Name = "ucJogControludZ";
            this.ucJogControludZ.RaiseCmdEvent += new System.EventHandler<GrblPlotter.UserControls.UserControlCmdEventArgs>(this.ucJogControlUD1_RaiseCmdEvent);
            // 
            // ucJogControludY
            // 
            resources.ApplyResources(this.ucJogControludY, "ucJogControludY");
            this.ucJogControludY.Name = "ucJogControludY";
            this.ucJogControludY.RaiseCmdEvent += new System.EventHandler<GrblPlotter.UserControls.UserControlCmdEventArgs>(this.ucJogControlUD1_RaiseCmdEvent);
            // 
            // ucJogControludX
            // 
            resources.ApplyResources(this.ucJogControludX, "ucJogControludX");
            this.ucJogControludX.Name = "ucJogControludX";
            this.ucJogControludX.RaiseCmdEvent += new System.EventHandler<GrblPlotter.UserControls.UserControlCmdEventArgs>(this.ucJogControlUD1_RaiseCmdEvent);
            // 
            // btnJogZeroZ
            // 
            resources.ApplyResources(this.btnJogZeroZ, "btnJogZeroZ");
            this.btnJogZeroZ.Name = "btnJogZeroZ";
            this.btnJogZeroZ.UseVisualStyleBackColor = true;
            this.btnJogZeroZ.Click += new System.EventHandler(this.BtnJogZ_Click);
            // 
            // btnJogZeroY
            // 
            resources.ApplyResources(this.btnJogZeroY, "btnJogZeroY");
            this.btnJogZeroY.Name = "btnJogZeroY";
            this.btnJogZeroY.UseVisualStyleBackColor = true;
            this.btnJogZeroY.Click += new System.EventHandler(this.BtnJogY_Click);
            // 
            // btnJogZeroX
            // 
            resources.ApplyResources(this.btnJogZeroX, "btnJogZeroX");
            this.btnJogZeroX.Name = "btnJogZeroX";
            this.btnJogZeroX.UseVisualStyleBackColor = true;
            this.btnJogZeroX.Click += new System.EventHandler(this.BtnJogX_Click);
            // 
            // btnKillAlarm
            // 
            resources.ApplyResources(this.btnKillAlarm, "btnKillAlarm");
            this.btnKillAlarm.Name = "btnKillAlarm";
            this.btnKillAlarm.UseVisualStyleBackColor = true;
            this.btnKillAlarm.Click += new System.EventHandler(this.BtnKillAlarm_Click);
            // 
            // btnFeedHold
            // 
            resources.ApplyResources(this.btnFeedHold, "btnFeedHold");
            this.btnFeedHold.Name = "btnFeedHold";
            this.btnFeedHold.UseVisualStyleBackColor = true;
            this.btnFeedHold.Click += new System.EventHandler(this.BtnFeedHold_Click);
            // 
            // btnResume
            // 
            resources.ApplyResources(this.btnResume, "btnResume");
            this.btnResume.Name = "btnResume";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.BtnResume_Click);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Name = "btnReset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
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
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnHome
            // 
            resources.ApplyResources(this.btnHome, "btnHome");
            this.btnHome.Name = "btnHome";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.BtnHome_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnZeroZ
            // 
            resources.ApplyResources(this.btnZeroZ, "btnZeroZ");
            this.btnZeroZ.Name = "btnZeroZ";
            this.btnZeroZ.UseVisualStyleBackColor = true;
            this.btnZeroZ.Click += new System.EventHandler(this.BtnZeroZ_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // btnZeroY
            // 
            resources.ApplyResources(this.btnZeroY, "btnZeroY");
            this.btnZeroY.Name = "btnZeroY";
            this.btnZeroY.UseVisualStyleBackColor = true;
            this.btnZeroY.Click += new System.EventHandler(this.BtnZeroY_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnZeroX
            // 
            resources.ApplyResources(this.btnZeroX, "btnZeroX");
            this.btnZeroX.Name = "btnZeroX";
            this.btnZeroX.UseVisualStyleBackColor = true;
            this.btnZeroX.Click += new System.EventHandler(this.BtnZeroX_Click);
            // 
            // label_status
            // 
            resources.ApplyResources(this.label_status, "label_status");
            this.label_status.Name = "label_status";
            // 
            // label_mx
            // 
            resources.ApplyResources(this.label_mx, "label_mx");
            this.label_mx.Name = "label_mx";
            // 
            // label_my
            // 
            resources.ApplyResources(this.label_my, "label_my");
            this.label_my.Name = "label_my";
            // 
            // label_mz
            // 
            resources.ApplyResources(this.label_mz, "label_mz");
            this.label_mz.Name = "label_mz";
            // 
            // label_wz
            // 
            resources.ApplyResources(this.label_wz, "label_wz");
            this.label_wz.Name = "label_wz";
            // 
            // label_wx
            // 
            resources.ApplyResources(this.label_wx, "label_wx");
            this.label_wx.Name = "label_wx";
            // 
            // label_wy
            // 
            resources.ApplyResources(this.label_wy, "label_wy");
            this.label_wy.Name = "label_wy";
            // 
            // Control2ndGRBL
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.btnFeedHold);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnKillAlarm);
            this.Name = "Control2ndGRBL";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Control2ndGRBL_FormClosing);
            this.Load += new System.EventHandler(this.Control2ndGRBL_Load);
            this.groupBox6.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnJogZeroX;
        private System.Windows.Forms.Button btnJogZeroY;
        private System.Windows.Forms.Button btnJogZeroZ;
    //    private virtualJoystick.virtualJoystick virtualJoystickZ;
        private System.Windows.Forms.Button btnKillAlarm;
        private System.Windows.Forms.Button btnFeedHold;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Button btnReset;
      //  private virtualJoystick.virtualJoystick virtualJoystickY;
     //   private virtualJoystick.virtualJoystick virtualJoystickX;
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
        private UserControls.UCJogControlUD ucJogControludZ;
        private UserControls.UCJogControlUD ucJogControludY;
        private UserControls.UCJogControlUD ucJogControludX;
    }
}