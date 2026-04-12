using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    partial class UCJogControlXY
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCJogControlXY));
            this.Btn9 = new System.Windows.Forms.Button();
            this.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi5 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi4 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi3 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi2 = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi1 = new System.Windows.Forms.ToolStripMenuItem();
            this.Btn8 = new System.Windows.Forms.Button();
            this.Btn7 = new System.Windows.Forms.Button();
            this.Btn6 = new System.Windows.Forms.Button();
            this.Btn5 = new System.Windows.Forms.Button();
            this.Btn4 = new System.Windows.Forms.Button();
            this.Btn3 = new System.Windows.Forms.Button();
            this.Btn2 = new System.Windows.Forms.Button();
            this.Btn1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.PanelButtons = new System.Windows.Forms.Panel();
            this.PanelJoyStick = new System.Windows.Forms.Panel();
            this.LblJogProp = new System.Windows.Forms.Label();
            this.ContextMenuStrip.SuspendLayout();
            this.PanelButtons.SuspendLayout();
            this.PanelJoyStick.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn9
            // 
            this.Btn9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn9.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn9.Location = new System.Drawing.Point(80, 80);
            this.Btn9.Name = "Btn9";
            this.Btn9.Size = new System.Drawing.Size(40, 40);
            this.Btn9.TabIndex = 8;
            this.Btn9.UseVisualStyleBackColor = true;
            this.Btn9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn9.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn9.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn9.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // ContextMenuStrip
            // 
            this.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsmi5,
            this.Tsmi4,
            this.Tsmi3,
            this.Tsmi2,
            this.Tsmi1});
            this.ContextMenuStrip.Name = "contextMenuStrip";
            this.ContextMenuStrip.Size = new System.Drawing.Size(81, 114);
            // 
            // Tsmi5
            // 
            this.Tsmi5.Name = "Tsmi5";
            this.Tsmi5.Size = new System.Drawing.Size(80, 22);
            this.Tsmi5.Text = "5";
            this.Tsmi5.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi4
            // 
            this.Tsmi4.Name = "Tsmi4";
            this.Tsmi4.Size = new System.Drawing.Size(80, 22);
            this.Tsmi4.Text = "4";
            this.Tsmi4.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi3
            // 
            this.Tsmi3.Name = "Tsmi3";
            this.Tsmi3.Size = new System.Drawing.Size(80, 22);
            this.Tsmi3.Text = "3";
            this.Tsmi3.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi2
            // 
            this.Tsmi2.Name = "Tsmi2";
            this.Tsmi2.Size = new System.Drawing.Size(80, 22);
            this.Tsmi2.Text = "2";
            this.Tsmi2.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Tsmi1
            // 
            this.Tsmi1.Name = "Tsmi1";
            this.Tsmi1.Size = new System.Drawing.Size(80, 22);
            this.Tsmi1.Text = "1";
            this.Tsmi1.Click += new System.EventHandler(this.ContextMenuStrip_Click);
            // 
            // Btn8
            // 
            this.Btn8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn8.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn8.Location = new System.Drawing.Point(40, 80);
            this.Btn8.Name = "Btn8";
            this.Btn8.Size = new System.Drawing.Size(40, 40);
            this.Btn8.TabIndex = 7;
            this.Btn8.UseVisualStyleBackColor = true;
            this.Btn8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn8.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn8.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn8.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // Btn7
            // 
            this.Btn7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn7.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn7.Location = new System.Drawing.Point(0, 80);
            this.Btn7.Name = "Btn7";
            this.Btn7.Size = new System.Drawing.Size(40, 40);
            this.Btn7.TabIndex = 6;
            this.Btn7.UseVisualStyleBackColor = true;
            this.Btn7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn7.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn7.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn7.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // Btn6
            // 
            this.Btn6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn6.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn6.Location = new System.Drawing.Point(80, 40);
            this.Btn6.Name = "Btn6";
            this.Btn6.Size = new System.Drawing.Size(40, 40);
            this.Btn6.TabIndex = 5;
            this.Btn6.UseVisualStyleBackColor = true;
            this.Btn6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn6.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn6.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn6.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // Btn5
            // 
            this.Btn5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn5.BackgroundImage")));
            this.Btn5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn5.Location = new System.Drawing.Point(40, 40);
            this.Btn5.Name = "Btn5";
            this.Btn5.Size = new System.Drawing.Size(40, 40);
            this.Btn5.TabIndex = 4;
            this.Btn5.UseVisualStyleBackColor = true;
            this.Btn5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn5.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn5.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            // 
            // Btn4
            // 
            this.Btn4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn4.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn4.Location = new System.Drawing.Point(0, 40);
            this.Btn4.Name = "Btn4";
            this.Btn4.Size = new System.Drawing.Size(40, 40);
            this.Btn4.TabIndex = 3;
            this.Btn4.UseVisualStyleBackColor = true;
            this.Btn4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn4.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn4.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // Btn3
            // 
            this.Btn3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn3.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn3.Location = new System.Drawing.Point(80, 0);
            this.Btn3.Name = "Btn3";
            this.Btn3.Size = new System.Drawing.Size(40, 40);
            this.Btn3.TabIndex = 2;
            this.Btn3.UseVisualStyleBackColor = true;
            this.Btn3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn3.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn3.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // Btn2
            // 
            this.Btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn2.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn2.Location = new System.Drawing.Point(40, 0);
            this.Btn2.Name = "Btn2";
            this.Btn2.Size = new System.Drawing.Size(40, 40);
            this.Btn2.TabIndex = 1;
            this.Btn2.UseVisualStyleBackColor = true;
            this.Btn2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn2.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn2.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // Btn1
            // 
            this.Btn1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn1.ContextMenuStrip = this.ContextMenuStrip;
            this.Btn1.Location = new System.Drawing.Point(0, 0);
            this.Btn1.Name = "Btn1";
            this.Btn1.Size = new System.Drawing.Size(40, 40);
            this.Btn1.TabIndex = 0;
            this.Btn1.UseVisualStyleBackColor = true;
            this.Btn1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.Btn1.MouseLeave += new System.EventHandler(this.Btn_MouseLeave);
            this.Btn1.MouseHover += new System.EventHandler(this.Btn_MouseHover);
            this.Btn1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // PanelButtons
            // 
            this.PanelButtons.Controls.Add(this.Btn1);
            this.PanelButtons.Controls.Add(this.Btn9);
            this.PanelButtons.Controls.Add(this.Btn8);
            this.PanelButtons.Controls.Add(this.Btn2);
            this.PanelButtons.Controls.Add(this.Btn7);
            this.PanelButtons.Controls.Add(this.Btn3);
            this.PanelButtons.Controls.Add(this.Btn6);
            this.PanelButtons.Controls.Add(this.Btn4);
            this.PanelButtons.Controls.Add(this.Btn5);
            this.PanelButtons.Location = new System.Drawing.Point(0, 0);
            this.PanelButtons.Margin = new System.Windows.Forms.Padding(0);
            this.PanelButtons.Name = "PanelButtons";
            this.PanelButtons.Size = new System.Drawing.Size(120, 120);
            this.PanelButtons.TabIndex = 9;
            // 
            // PanelJoyStick
            // 
            this.PanelJoyStick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PanelJoyStick.Controls.Add(this.LblJogProp);
            this.PanelJoyStick.Location = new System.Drawing.Point(0, 0);
            this.PanelJoyStick.Margin = new System.Windows.Forms.Padding(0);
            this.PanelJoyStick.Name = "PanelJoyStick";
            this.PanelJoyStick.Size = new System.Drawing.Size(120, 120);
            this.PanelJoyStick.TabIndex = 10;
            this.PanelJoyStick.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelJoyStick_Paint);
            this.PanelJoyStick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.PanelJoyStick.MouseLeave += new System.EventHandler(this.PanelJoyStick_MouseLeave);
            this.PanelJoyStick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseMove);
            this.PanelJoyStick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            // 
            // LblJogProp
            // 
            this.LblJogProp.AutoSize = true;
            this.LblJogProp.Location = new System.Drawing.Point(45, 47);
            this.LblJogProp.Name = "LblJogProp";
            this.LblJogProp.Size = new System.Drawing.Size(29, 13);
            this.LblJogProp.TabIndex = 10;
            this.LblJogProp.Text = "Prop";
            this.LblJogProp.Visible = false;
            // 
            // UCJogControlXY
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.PanelButtons);
            this.Controls.Add(this.PanelJoyStick);
            this.DoubleBuffered = true;
            this.Name = "UCJogControlXY";
            this.Size = new System.Drawing.Size(120, 120);
            this.Load += new System.EventHandler(this.UserControlJogControlXY_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VirtualJoystick_MouseUp);
            this.ContextMenuStrip.ResumeLayout(false);
            this.PanelButtons.ResumeLayout(false);
            this.PanelJoyStick.ResumeLayout(false);
            this.PanelJoyStick.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Button Btn9;
        private Button Btn8;
        private Button Btn7;
        private Button Btn6;
        private Button Btn5;
        private Button Btn4;
        private Button Btn3;
        private Button Btn2;
        private Button Btn1;
        private ContextMenuStrip ContextMenuStrip;
        private ToolStripMenuItem Tsmi5;
        private ToolStripMenuItem Tsmi4;
        private ToolStripMenuItem Tsmi3;
        private ToolStripMenuItem Tsmi2;
        private ToolStripMenuItem Tsmi1;
        private ToolTip toolTip1;
        private Panel PanelButtons;
        private Panel PanelJoyStick;
        private Label LblJogProp;
    }
}
