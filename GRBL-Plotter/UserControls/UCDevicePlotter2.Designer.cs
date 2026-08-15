namespace GrblPlotter.UserControls
{
    partial class UCDevicePlotter2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDevicePlotter2));
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BtnPenUp = new System.Windows.Forms.Button();
            this.BtnPenDown = new System.Windows.Forms.Button();
            this.BtnPenZero = new System.Windows.Forms.Button();
            this.BtnPenDownUp = new System.Windows.Forms.Button();
            this.btnCustom1 = new System.Windows.Forms.Button();
            this.PanelPenChange = new System.Windows.Forms.Panel();
            this.BtnGripperClose = new System.Windows.Forms.Button();
            this.BtnGripperOpen = new System.Windows.Forms.Button();
            this.NudToolNr = new System.Windows.Forms.NumericUpDown();
            this.BtnStartToolTake = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.BtnToolRemove = new System.Windows.Forms.Button();
            this.GbGripper = new System.Windows.Forms.GroupBox();
            this.GbToolChange = new System.Windows.Forms.GroupBox();
            this.BtnStartToolSelect = new System.Windows.Forms.Button();
            this.BtnStartToolProbe = new System.Windows.Forms.Button();
            this.TableLayoutPanel1.SuspendLayout();
            this.PanelPenChange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NudToolNr)).BeginInit();
            this.GbGripper.SuspendLayout();
            this.GbToolChange.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            resources.ApplyResources(this.TableLayoutPanel1, "TableLayoutPanel1");
            this.TableLayoutPanel1.Controls.Add(this.BtnPenUp, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.BtnPenDown, 0, 2);
            this.TableLayoutPanel1.Controls.Add(this.BtnPenZero, 0, 1);
            this.TableLayoutPanel1.Controls.Add(this.BtnPenDownUp, 1, 1);
            this.TableLayoutPanel1.Controls.Add(this.btnCustom1, 1, 0);
            this.TableLayoutPanel1.Controls.Add(this.PanelPenChange, 0, 3);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            // 
            // BtnPenUp
            // 
            resources.ApplyResources(this.BtnPenUp, "BtnPenUp");
            this.BtnPenUp.Name = "BtnPenUp";
            this.toolTip1.SetToolTip(this.BtnPenUp, resources.GetString("BtnPenUp.ToolTip"));
            this.BtnPenUp.UseVisualStyleBackColor = true;
            this.BtnPenUp.Click += new System.EventHandler(this.BtnPenUp_Click);
            // 
            // BtnPenDown
            // 
            resources.ApplyResources(this.BtnPenDown, "BtnPenDown");
            this.BtnPenDown.Name = "BtnPenDown";
            this.toolTip1.SetToolTip(this.BtnPenDown, resources.GetString("BtnPenDown.ToolTip"));
            this.BtnPenDown.UseVisualStyleBackColor = true;
            this.BtnPenDown.Click += new System.EventHandler(this.BtnPenDown_Click);
            // 
            // BtnPenZero
            // 
            resources.ApplyResources(this.BtnPenZero, "BtnPenZero");
            this.BtnPenZero.Name = "BtnPenZero";
            this.toolTip1.SetToolTip(this.BtnPenZero, resources.GetString("BtnPenZero.ToolTip"));
            this.BtnPenZero.UseVisualStyleBackColor = true;
            this.BtnPenZero.Click += new System.EventHandler(this.BtnPenZero_Click);
            // 
            // BtnPenDownUp
            // 
            resources.ApplyResources(this.BtnPenDownUp, "BtnPenDownUp");
            this.BtnPenDownUp.Name = "BtnPenDownUp";
            this.TableLayoutPanel1.SetRowSpan(this.BtnPenDownUp, 2);
            this.toolTip1.SetToolTip(this.BtnPenDownUp, resources.GetString("BtnPenDownUp.ToolTip"));
            this.BtnPenDownUp.UseVisualStyleBackColor = true;
            this.BtnPenDownUp.Click += new System.EventHandler(this.BtnPenDownUp_Click);
            // 
            // btnCustom1
            // 
            resources.ApplyResources(this.btnCustom1, "btnCustom1");
            this.btnCustom1.Name = "btnCustom1";
            this.btnCustom1.UseVisualStyleBackColor = true;
            this.btnCustom1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustom1_MouseDown);
            // 
            // PanelPenChange
            // 
            this.TableLayoutPanel1.SetColumnSpan(this.PanelPenChange, 2);
            this.PanelPenChange.Controls.Add(this.GbToolChange);
            this.PanelPenChange.Controls.Add(this.GbGripper);
            resources.ApplyResources(this.PanelPenChange, "PanelPenChange");
            this.PanelPenChange.Name = "PanelPenChange";
            // 
            // BtnGripperClose
            // 
            resources.ApplyResources(this.BtnGripperClose, "BtnGripperClose");
            this.BtnGripperClose.Name = "BtnGripperClose";
            this.toolTip1.SetToolTip(this.BtnGripperClose, resources.GetString("BtnGripperClose.ToolTip"));
            this.BtnGripperClose.UseVisualStyleBackColor = true;
            this.BtnGripperClose.Click += new System.EventHandler(this.BtnGripperClose_Click);
            // 
            // BtnGripperOpen
            // 
            resources.ApplyResources(this.BtnGripperOpen, "BtnGripperOpen");
            this.BtnGripperOpen.Name = "BtnGripperOpen";
            this.toolTip1.SetToolTip(this.BtnGripperOpen, resources.GetString("BtnGripperOpen.ToolTip"));
            this.BtnGripperOpen.UseVisualStyleBackColor = true;
            this.BtnGripperOpen.Click += new System.EventHandler(this.BtnGripperOpen_Click);
            // 
            // NudToolNr
            // 
            resources.ApplyResources(this.NudToolNr, "NudToolNr");
            this.NudToolNr.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.NudToolNr.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NudToolNr.Name = "NudToolNr";
            this.toolTip1.SetToolTip(this.NudToolNr, resources.GetString("NudToolNr.ToolTip"));
            this.NudToolNr.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // BtnStartToolTake
            // 
            resources.ApplyResources(this.BtnStartToolTake, "BtnStartToolTake");
            this.BtnStartToolTake.Name = "BtnStartToolTake";
            this.toolTip1.SetToolTip(this.BtnStartToolTake, resources.GetString("BtnStartToolTake.ToolTip"));
            this.BtnStartToolTake.UseVisualStyleBackColor = true;
            this.BtnStartToolTake.Click += new System.EventHandler(this.BtnStartToolTake_Click);
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = global::GrblPlotter.Properties.Settings.Default.DevicePlotterPenInHolder;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "DevicePlotterPenInHolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Name = "checkBox1";
            this.toolTip1.SetToolTip(this.checkBox1, resources.GetString("checkBox1.ToolTip"));
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // BtnToolRemove
            // 
            resources.ApplyResources(this.BtnToolRemove, "BtnToolRemove");
            this.BtnToolRemove.Name = "BtnToolRemove";
            this.toolTip1.SetToolTip(this.BtnToolRemove, resources.GetString("BtnToolRemove.ToolTip"));
            this.BtnToolRemove.UseVisualStyleBackColor = true;
            this.BtnToolRemove.Click += new System.EventHandler(this.BtnStartToolRemove_Click);
            // 
            // GbGripper
            // 
            this.GbGripper.Controls.Add(this.BtnGripperOpen);
            this.GbGripper.Controls.Add(this.BtnGripperClose);
            this.GbGripper.Controls.Add(this.checkBox1);
            resources.ApplyResources(this.GbGripper, "GbGripper");
            this.GbGripper.Name = "GbGripper";
            this.GbGripper.TabStop = false;
            // 
            // GbToolChange
            // 
            this.GbToolChange.Controls.Add(this.BtnStartToolProbe);
            this.GbToolChange.Controls.Add(this.BtnStartToolSelect);
            this.GbToolChange.Controls.Add(this.NudToolNr);
            this.GbToolChange.Controls.Add(this.BtnStartToolTake);
            this.GbToolChange.Controls.Add(this.BtnToolRemove);
            resources.ApplyResources(this.GbToolChange, "GbToolChange");
            this.GbToolChange.Name = "GbToolChange";
            this.GbToolChange.TabStop = false;
            // 
            // BtnStartToolSelect
            // 
            resources.ApplyResources(this.BtnStartToolSelect, "BtnStartToolSelect");
            this.BtnStartToolSelect.Name = "BtnStartToolSelect";
            this.BtnStartToolSelect.UseVisualStyleBackColor = true;
            this.BtnStartToolSelect.Click += new System.EventHandler(this.BtnStartToolSelect_Click);
            // 
            // BtnStartToolProbe
            // 
            resources.ApplyResources(this.BtnStartToolProbe, "BtnStartToolProbe");
            this.BtnStartToolProbe.Name = "BtnStartToolProbe";
            this.BtnStartToolProbe.UseVisualStyleBackColor = true;
            this.BtnStartToolProbe.Click += new System.EventHandler(this.BtnStartToolProbe_Click);
            // 
            // UCDevicePlotter2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.TableLayoutPanel1);
            this.Name = "UCDevicePlotter2";
            this.BackColorChanged += new System.EventHandler(this.UCDevicePlotter2_BackColorChanged);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            this.PanelPenChange.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NudToolNr)).EndInit();
            this.GbGripper.ResumeLayout(false);
            this.GbGripper.PerformLayout();
            this.GbToolChange.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnPenZero;
        private System.Windows.Forms.Button BtnPenUp;
        private System.Windows.Forms.Button BtnPenDown;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button BtnPenDownUp;
        private System.Windows.Forms.Button btnCustom1;
        private System.Windows.Forms.Panel PanelPenChange;
        private System.Windows.Forms.Button BtnStartToolTake;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.NumericUpDown NudToolNr;
        private System.Windows.Forms.Button BtnGripperClose;
        private System.Windows.Forms.Button BtnGripperOpen;
        private System.Windows.Forms.Button BtnToolRemove;
        private System.Windows.Forms.GroupBox GbGripper;
        private System.Windows.Forms.GroupBox GbToolChange;
        private System.Windows.Forms.Button BtnStartToolSelect;
        private System.Windows.Forms.Button BtnStartToolProbe;
    }
}
