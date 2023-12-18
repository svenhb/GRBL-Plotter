namespace GrblPlotter
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
            this.button3 = new System.Windows.Forms.Button();
            this.cBSnap = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRotate = new System.Windows.Forms.Button();
            this.nUDRaster = new System.Windows.Forms.NumericUpDown();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gBJogParameter = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tBCodeEnd = new System.Windows.Forms.TextBox();
            this.tBCodeStart = new System.Windows.Forms.TextBox();
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
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.PictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
            // 
            // btnUndo
            // 
            resources.ApplyResources(this.btnUndo, "btnUndo");
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.BtnUndo_Click);
            // 
            // gBPathCreator
            // 
            this.gBPathCreator.Controls.Add(this.button3);
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
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.SkyBlue;
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.Tag = "id=form-jog-path";
            this.toolTip1.SetToolTip(this.button3, resources.GetString("button3.ToolTip"));
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
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
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnRotate
            // 
            resources.ApplyResources(this.btnRotate, "btnRotate");
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Click += new System.EventHandler(this.BtnRotate_Click);
            // 
            // nUDRaster
            // 
            this.nUDRaster.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createJogPathRaster", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDRaster.DecimalPlaces = 1;
            resources.ApplyResources(this.nUDRaster, "nUDRaster");
            this.nUDRaster.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nUDRaster.Name = "nUDRaster";
            this.toolTip1.SetToolTip(this.nUDRaster, resources.GetString("nUDRaster.ToolTip"));
            this.nUDRaster.Value = global::GrblPlotter.Properties.Settings.Default.createJogPathRaster;
            this.nUDRaster.ValueChanged += new System.EventHandler(this.NumericUpDown2_ValueChanged);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.toolTip1.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // gBJogParameter
            // 
            this.gBJogParameter.Controls.Add(this.label4);
            this.gBJogParameter.Controls.Add(this.label3);
            this.gBJogParameter.Controls.Add(this.tBCodeEnd);
            this.gBJogParameter.Controls.Add(this.tBCodeStart);
            this.gBJogParameter.Controls.Add(this.btnExport);
            this.gBJogParameter.Controls.Add(this.btnJogStop);
            this.gBJogParameter.Controls.Add(this.btnJogStart);
            this.gBJogParameter.Controls.Add(this.label2);
            this.gBJogParameter.Controls.Add(this.nUDFeedrate);
            resources.ApplyResources(this.gBJogParameter, "gBJogParameter");
            this.gBJogParameter.Name = "gBJogParameter";
            this.gBJogParameter.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tBCodeEnd
            // 
            resources.ApplyResources(this.tBCodeEnd, "tBCodeEnd");
            this.tBCodeEnd.Name = "tBCodeEnd";
            this.toolTip1.SetToolTip(this.tBCodeEnd, resources.GetString("tBCodeEnd.ToolTip"));
            // 
            // tBCodeStart
            // 
            this.tBCodeStart.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "createJogPathCodeStart", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBCodeStart, "tBCodeStart");
            this.tBCodeStart.Name = "tBCodeStart";
            this.tBCodeStart.Text = global::GrblPlotter.Properties.Settings.Default.createJogPathCodeStart;
            this.toolTip1.SetToolTip(this.tBCodeStart, resources.GetString("tBCodeStart.ToolTip"));
            // 
            // btnExport
            // 
            resources.ApplyResources(this.btnExport, "btnExport");
            this.btnExport.Name = "btnExport";
            this.toolTip1.SetToolTip(this.btnExport, resources.GetString("btnExport.ToolTip"));
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // btnJogStop
            // 
            this.btnJogStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnJogStop.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "createJogPathCodeEnd", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.btnJogStop, "btnJogStop");
            this.btnJogStop.Name = "btnJogStop";
            this.btnJogStop.Text = global::GrblPlotter.Properties.Settings.Default.createJogPathCodeEnd;
            this.toolTip1.SetToolTip(this.btnJogStop, resources.GetString("btnJogStop.ToolTip"));
            this.btnJogStop.UseVisualStyleBackColor = false;
            // 
            // btnJogStart
            // 
            this.btnJogStart.BackColor = System.Drawing.Color.GreenYellow;
            resources.ApplyResources(this.btnJogStart, "btnJogStart");
            this.btnJogStart.Name = "btnJogStart";
            this.toolTip1.SetToolTip(this.btnJogStart, resources.GetString("btnJogStart.ToolTip"));
            this.btnJogStart.UseVisualStyleBackColor = false;
            this.btnJogStart.Click += new System.EventHandler(this.BtnJogStart_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nUDFeedrate
            // 
            this.nUDFeedrate.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "createJogPathFeedrate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
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
            this.nUDFeedrate.Value = global::GrblPlotter.Properties.Settings.Default.createJogPathFeedrate;
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
        internal System.Windows.Forms.Button btnJogStop;
        internal System.Windows.Forms.Button btnJogStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUDFeedrate;
        private System.Windows.Forms.NumericUpDown nUDRaster;
        private System.Windows.Forms.Button btnRotate;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox cBSnap;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tBCodeEnd;
        private System.Windows.Forms.TextBox tBCodeStart;
        private System.Windows.Forms.Button button3;
    }
}