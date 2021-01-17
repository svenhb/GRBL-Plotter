namespace GRBL_Plotter
{
    partial class ControlJogPathCreator
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

                startIcon.Dispose();
                ruler.Dispose();
                rubberBand.Dispose();
                penStartIcon.Dispose();
                penRuler.Dispose();
                penRubberBand.Dispose();
                penjogPath.Dispose();
                penGrid.Dispose();
                penActualLine.Dispose();
                pBoxTransform.Dispose();
                pBoxOrig.Dispose();
                jogPath.Dispose();
                grid.Dispose();
                actualLine.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlJogPathCreator));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnUndo = new System.Windows.Forms.Button();
            this.gBPathCreator = new System.Windows.Forms.GroupBox();
            this.cBSnap = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRotate = new System.Windows.Forms.Button();
            this.nUDRaster = new System.Windows.Forms.NumericUpDown();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gBJogParameter = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnJogStop = new System.Windows.Forms.Button();
            this.btnJogStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDFeedrate = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gBPathCreator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRaster)).BeginInit();
            this.gBJogParameter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFeedrate)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            // 
            // btnUndo
            // 
            resources.ApplyResources(this.btnUndo, "btnUndo");
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // gBPathCreator
            // 
            this.gBPathCreator.Controls.Add(this.cBSnap);
            this.gBPathCreator.Controls.Add(this.btnLoad);
            this.gBPathCreator.Controls.Add(this.btnSave);
            this.gBPathCreator.Controls.Add(this.btnRotate);
            this.gBPathCreator.Controls.Add(this.nUDRaster);
            this.gBPathCreator.Controls.Add(this.btnDelete);
            this.gBPathCreator.Controls.Add(this.label1);
            this.gBPathCreator.Controls.Add(this.btnUndo);
            resources.ApplyResources(this.gBPathCreator, "gBPathCreator");
            this.gBPathCreator.Name = "gBPathCreator";
            this.gBPathCreator.TabStop = false;
            // 
            // cBSnap
            // 
            resources.ApplyResources(this.cBSnap, "cBSnap");
            this.cBSnap.Checked = true;
            this.cBSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBSnap.Name = "cBSnap";
            this.cBSnap.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRotate
            // 
            resources.ApplyResources(this.btnRotate, "btnRotate");
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // nUDRaster
            // 
            this.nUDRaster.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "createJogPathRaster", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDRaster.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDRaster, "nUDRaster");
            this.nUDRaster.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDRaster.Name = "nUDRaster";
            this.toolTip1.SetToolTip(this.nUDRaster, resources.GetString("nUDRaster.ToolTip"));
            this.nUDRaster.Value = global::GRBL_Plotter.Properties.Settings.Default.createJogPathRaster;
            this.nUDRaster.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.toolTip1.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // gBJogParameter
            // 
            this.gBJogParameter.Controls.Add(this.btnExport);
            this.gBJogParameter.Controls.Add(this.btnJogStop);
            this.gBJogParameter.Controls.Add(this.btnJogStart);
            this.gBJogParameter.Controls.Add(this.label2);
            this.gBJogParameter.Controls.Add(this.nUDFeedrate);
            resources.ApplyResources(this.gBJogParameter, "gBJogParameter");
            this.gBJogParameter.Name = "gBJogParameter";
            this.gBJogParameter.TabStop = false;
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Name = "btnExport";
            this.toolTip1.SetToolTip(this.btnExport, resources.GetString("btnExport.ToolTip"));
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnJogStop
            // 
            resources.ApplyResources(this.btnJogStop, "btnJogStop");
            this.btnJogStop.Name = "btnJogStop";
            this.toolTip1.SetToolTip(this.btnJogStop, resources.GetString("btnJogStop.ToolTip"));
            this.btnJogStop.UseVisualStyleBackColor = true;
            // 
            // btnJogStart
            // 
            resources.ApplyResources(this.btnJogStart, "btnJogStart");
            this.btnJogStart.Name = "btnJogStart";
            this.toolTip1.SetToolTip(this.btnJogStart, resources.GetString("btnJogStart.ToolTip"));
            this.btnJogStart.UseVisualStyleBackColor = true;
            this.btnJogStart.Click += new System.EventHandler(this.btnJogStart_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nUDFeedrate
            // 
            this.nUDFeedrate.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GRBL_Plotter.Properties.Settings.Default, "createJogPathFeedrate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDFeedrate.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDFeedrate, "nUDFeedrate");
            this.nUDFeedrate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nUDFeedrate.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDFeedrate.Name = "nUDFeedrate";
            this.toolTip1.SetToolTip(this.nUDFeedrate, resources.GetString("nUDFeedrate.ToolTip"));
            this.nUDFeedrate.Value = global::GRBL_Plotter.Properties.Settings.Default.createJogPathFeedrate;
            // 
            // ControlJogPathCreator
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gBJogParameter);
            this.Controls.Add(this.gBPathCreator);
            this.Controls.Add(this.pictureBox1);
            this.Name = "ControlJogPathCreator";
            this.Load += new System.EventHandler(this.ControlJogPathCreator_Load);
            this.SizeChanged += new System.EventHandler(this.ControlJogPathCreator_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gBPathCreator.ResumeLayout(false);
            this.gBPathCreator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRaster)).EndInit();
            this.gBJogParameter.ResumeLayout(false);
            this.gBJogParameter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFeedrate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.GroupBox gBPathCreator;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gBJogParameter;
        public System.Windows.Forms.Button btnJogStop;
        public System.Windows.Forms.Button btnJogStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUDFeedrate;
        private System.Windows.Forms.NumericUpDown nUDRaster;
        private System.Windows.Forms.Button btnRotate;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox cBSnap;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}