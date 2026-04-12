using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCJogControlUD
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.BtnDown = new System.Windows.Forms.Button();
            this.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi5 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi4 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi3 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi2 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi1 = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnUp = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.PanelButtons = new System.Windows.Forms.Panel();
            this.PanelJoyStick = new System.Windows.Forms.Panel();
            this.LblJogProp = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ContextMenuStrip.SuspendLayout();
            this.PanelButtons.SuspendLayout();
            this.PanelJoyStick.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnDown
            // 
            this.BtnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.BtnDown.ContextMenuStrip = this.ContextMenuStrip;
            this.BtnDown.Location = new System.Drawing.Point(0, 80);
            this.BtnDown.Margin = new System.Windows.Forms.Padding(0);
            this.BtnDown.Name = "BtnDown";
            this.BtnDown.Size = new System.Drawing.Size(40, 40);
            this.BtnDown.TabIndex = 11;
            this.BtnDown.UseVisualStyleBackColor = true;
            this.BtnDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.BtnDown.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.BtnDown.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.BtnDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // ContextMenuStrip
            // 
            this.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsmi5,
            this.Tsmi4,
            this.Tsmi3,
            this.Tsmi2,
            this.Tsmi1});
            this.ContextMenuStrip.Name = "contextMenuStrip1";
            this.ContextMenuStrip.Size = new System.Drawing.Size(81, 114);
            this.ContextMenuStrip.Text = "Step size";
            // 
            // Tsmi5
            // 
            this.Tsmi5.Name = "Tsmi5";
            this.Tsmi5.Size = new System.Drawing.Size(80, 22);
            this.Tsmi5.Tag = "5";
            this.Tsmi5.Text = "5";
            this.Tsmi5.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi4
            // 
            this.Tsmi4.Name = "Tsmi4";
            this.Tsmi4.Size = new System.Drawing.Size(80, 22);
            this.Tsmi4.Tag = "4";
            this.Tsmi4.Text = "4";
            this.Tsmi4.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi3
            // 
            this.Tsmi3.Name = "Tsmi3";
            this.Tsmi3.Size = new System.Drawing.Size(80, 22);
            this.Tsmi3.Tag = "3";
            this.Tsmi3.Text = "3";
            this.Tsmi3.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi2
            // 
            this.Tsmi2.Name = "Tsmi2";
            this.Tsmi2.Size = new System.Drawing.Size(80, 22);
            this.Tsmi2.Tag = "2";
            this.Tsmi2.Text = "2";
            this.Tsmi2.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi1
            // 
            this.Tsmi1.Name = "Tsmi1";
            this.Tsmi1.Size = new System.Drawing.Size(80, 22);
            this.Tsmi1.Tag = "1";
            this.Tsmi1.Text = "1";
            this.Tsmi1.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // BtnUp
            // 
            this.BtnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.BtnUp.ContextMenuStrip = this.ContextMenuStrip;
            this.BtnUp.Location = new System.Drawing.Point(0, 0);
            this.BtnUp.Margin = new System.Windows.Forms.Padding(0);
            this.BtnUp.Name = "BtnUp";
            this.BtnUp.Size = new System.Drawing.Size(40, 40);
            this.BtnUp.TabIndex = 9;
            this.BtnUp.UseVisualStyleBackColor = true;
            this.BtnUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.BtnUp.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.BtnUp.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.BtnUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // BtnStop
            // 
            this.BtnStop.BackgroundImage = global::GrblPlotter.Properties.Resources.stop;
            this.BtnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.BtnStop.Location = new System.Drawing.Point(0, 40);
            this.BtnStop.Margin = new System.Windows.Forms.Padding(0);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(40, 40);
            this.BtnStop.TabIndex = 23;
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.BtnStop.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.BtnStop.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            // 
            // PanelButtons
            // 
            this.PanelButtons.Controls.Add(this.BtnUp);
            this.PanelButtons.Controls.Add(this.BtnDown);
            this.PanelButtons.Controls.Add(this.BtnStop);
            this.PanelButtons.Location = new System.Drawing.Point(0, 0);
            this.PanelButtons.Margin = new System.Windows.Forms.Padding(0);
            this.PanelButtons.Name = "PanelButtons";
            this.PanelButtons.Size = new System.Drawing.Size(30, 120);
            this.PanelButtons.TabIndex = 24;
            // 
            // PanelJoyStick
            // 
            this.PanelJoyStick.Controls.Add(this.LblJogProp);
            this.PanelJoyStick.Location = new System.Drawing.Point(0, 0);
            this.PanelJoyStick.Margin = new System.Windows.Forms.Padding(0);
            this.PanelJoyStick.Name = "PanelJoyStick";
            this.PanelJoyStick.Size = new System.Drawing.Size(30, 120);
            this.PanelJoyStick.TabIndex = 25;
            this.PanelJoyStick.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelJoyStick_Paint);
            this.PanelJoyStick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.PanelJoyStick.MouseLeave += new System.EventHandler(this.PanelJoyStick_MouseLeave);
            this.PanelJoyStick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseMove);
            this.PanelJoyStick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // LblJogProp
            // 
            this.LblJogProp.AutoSize = true;
            this.LblJogProp.Location = new System.Drawing.Point(13, 95);
            this.LblJogProp.Name = "LblJogProp";
            this.LblJogProp.Size = new System.Drawing.Size(29, 13);
            this.LblJogProp.TabIndex = 25;
            this.LblJogProp.Text = "Prop";
            this.LblJogProp.Visible = false;
            // 
            // UCJogControlUD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.PanelButtons);
            this.Controls.Add(this.PanelJoyStick);
            this.DoubleBuffered = true;
            this.Name = "UCJogControlUD";
            this.Size = new System.Drawing.Size(40, 120);
            this.Load += new System.EventHandler(this.UserControlJogControlUD_Load);
            this.ContextMenuStrip.ResumeLayout(false);
            this.PanelButtons.ResumeLayout(false);
            this.PanelJoyStick.ResumeLayout(false);
            this.PanelJoyStick.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Button BtnDown;
        private Button BtnUp;
        private Button BtnStop;
        private ContextMenuStrip ContextMenuStrip;
        private ToolStripMenuItem Tsmi5;
        private ToolStripMenuItem Tsmi4;
        private ToolStripMenuItem Tsmi3;
        private ToolStripMenuItem Tsmi2;
        private ToolStripMenuItem Tsmi1;
        private Panel PanelButtons;
        private Panel PanelJoyStick;
        private ToolTip toolTip1;
        private Label LblJogProp;
    }
}
