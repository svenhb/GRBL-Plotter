namespace GrblPlotter.MachineControl
{
    partial class ControlMoveXY
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlMoveXY));
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ucJogControlXY = new GrblPlotter.UserControls.UCJogControlXY();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 38);
            this.label1.TabIndex = 25;
            this.label1.Text = "Move to fiducial until probe is triggered";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 236);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 38);
            this.button1.TabIndex = 26;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ucJogControlXY
            // 
            this.ucJogControlXY.Location = new System.Drawing.Point(3, 50);
            this.ucJogControlXY.Name = "ucJogControlXY";
            this.ucJogControlXY.Size = new System.Drawing.Size(180, 180);
            this.ucJogControlXY.TabIndex = 27;
            this.ucJogControlXY.RaiseCmdEvent += new System.EventHandler<GrblPlotter.UserControls.UserControlCmdEventArgs>(this.ucJogControlXY_RaiseCmdEvent);
            // 
            // ControlMoveXY
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(187, 272);
            this.Controls.Add(this.ucJogControlXY);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(203, 227);
            this.Name = "ControlMoveXY";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Move XY";
            this.Load += new System.EventHandler(this.ControlMoveXY_Load);
            this.Resize += new System.EventHandler(this.ControlMoveXY_Resize);
            this.ResumeLayout(false);

        }

        #endregion

   //     private virtualJoystick.virtualJoystick virtualJoystickXY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private UserControls.UCJogControlXY ucJogControlXY;
    }
}