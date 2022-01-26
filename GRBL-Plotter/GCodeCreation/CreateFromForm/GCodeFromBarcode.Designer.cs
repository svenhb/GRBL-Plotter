namespace GrblPlotter
{
    partial class GCodeForBarcode
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
				qrCodeImage.Dispose();
				barcodeImage.Dispose();
	//			qrCode.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeForBarcode));
            this.btnGenerateBarcode1D = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnGenerateBarcode2D = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnClipboardBarcode1D = new System.Windows.Forms.Button();
            this.btnCheckBarcode1D = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.nUDHeight1D = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.lblWidth1D = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nUDLines1D = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDScanGap1D = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gBLogo = new System.Windows.Forms.GroupBox();
            this.btnLogoInvert = new System.Windows.Forms.Button();
            this.btnLogoFromFile = new System.Windows.Forms.Button();
            this.btnLogoFromClipboard = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.tCQRPayload = new System.Windows.Forms.TabControl();
            this.tPQR1 = new System.Windows.Forms.TabPage();
            this.tBQRText = new System.Windows.Forms.TextBox();
            this.tPQR2 = new System.Windows.Forms.TabPage();
            this.tBQRURL = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.cBWLAN1 = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.rBWLAN2 = new System.Windows.Forms.RadioButton();
            this.rBWLAN1 = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tBWLAN2 = new System.Windows.Forms.TextBox();
            this.tBWLAN1 = new System.Windows.Forms.TextBox();
            this.btnClipboardBarcode2D = new System.Windows.Forms.Button();
            this.cBInsertLogo = new System.Windows.Forms.CheckBox();
            this.btnCheckBarcode2D = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblWidth2D = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cBBorder2D = new System.Windows.Forms.CheckBox();
            this.nUDLines2D = new System.Windows.Forms.NumericUpDown();
            this.nUDScanGap2D = new System.Windows.Forms.NumericUpDown();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.CbInsertCode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight1D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLines1D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDScanGap1D)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.gBLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.tCQRPayload.SuspendLayout();
            this.tPQR1.SuspendLayout();
            this.tPQR2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLines2D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDScanGap2D)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerateBarcode1D
            // 
            resources.ApplyResources(this.btnGenerateBarcode1D, "btnGenerateBarcode1D");
            this.btnGenerateBarcode1D.Name = "btnGenerateBarcode1D";
            this.btnGenerateBarcode1D.UseVisualStyleBackColor = true;
            this.btnGenerateBarcode1D.Click += new System.EventHandler(this.BtnGenerateBarcodeClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::GrblPlotter.Properties.Resources.barcode;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // btnGenerateBarcode2D
            // 
            resources.ApplyResources(this.btnGenerateBarcode2D, "btnGenerateBarcode2D");
            this.btnGenerateBarcode2D.Name = "btnGenerateBarcode2D";
            this.btnGenerateBarcode2D.UseVisualStyleBackColor = true;
            this.btnGenerateBarcode2D.Click += new System.EventHandler(this.BtnGenerateQRCode_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnClipboardBarcode1D);
            this.tabPage1.Controls.Add(this.btnCheckBarcode1D);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.nUDHeight1D);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.lblWidth1D);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.nUDLines1D);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.nUDScanGap1D);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Controls.Add(this.btnGenerateBarcode1D);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnClipboardBarcode1D
            // 
            resources.ApplyResources(this.btnClipboardBarcode1D, "btnClipboardBarcode1D");
            this.btnClipboardBarcode1D.Name = "btnClipboardBarcode1D";
            this.btnClipboardBarcode1D.UseVisualStyleBackColor = true;
            this.btnClipboardBarcode1D.Click += new System.EventHandler(this.BtnClipboardBarcode1D_Click);
            // 
            // btnCheckBarcode1D
            // 
            resources.ApplyResources(this.btnCheckBarcode1D, "btnCheckBarcode1D");
            this.btnCheckBarcode1D.Name = "btnCheckBarcode1D";
            this.btnCheckBarcode1D.UseVisualStyleBackColor = true;
            this.btnCheckBarcode1D.Click += new System.EventHandler(this.BtnCheckBarcode1D_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // nUDHeight1D
            // 
            this.nUDHeight1D.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importBarcode1DHeight", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDHeight1D.DecimalPlaces = 1;
            this.nUDHeight1D.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            resources.ApplyResources(this.nUDHeight1D, "nUDHeight1D");
            this.nUDHeight1D.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDHeight1D.Name = "nUDHeight1D";
            this.nUDHeight1D.Value = global::GrblPlotter.Properties.Settings.Default.importBarcode1DHeight;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lblWidth1D
            // 
            resources.ApplyResources(this.lblWidth1D, "lblWidth1D");
            this.lblWidth1D.Name = "lblWidth1D";
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importBarcode1DText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.Text = global::GrblPlotter.Properties.Settings.Default.importBarcode1DText;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // nUDLines1D
            // 
            this.nUDLines1D.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importBarcode1DLines", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDLines1D, "nUDLines1D");
            this.nUDLines1D.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDLines1D.Name = "nUDLines1D";
            this.nUDLines1D.Value = global::GrblPlotter.Properties.Settings.Default.importBarcode1DLines;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nUDScanGap1D
            // 
            this.nUDScanGap1D.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importBarcode1DScanGap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDScanGap1D.DecimalPlaces = 2;
            this.nUDScanGap1D.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDScanGap1D, "nUDScanGap1D");
            this.nUDScanGap1D.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDScanGap1D.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nUDScanGap1D.Name = "nUDScanGap1D";
            this.nUDScanGap1D.Value = global::GrblPlotter.Properties.Settings.Default.importBarcode1DScanGap;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gBLogo);
            this.tabPage2.Controls.Add(this.tCQRPayload);
            this.tabPage2.Controls.Add(this.btnClipboardBarcode2D);
            this.tabPage2.Controls.Add(this.cBInsertLogo);
            this.tabPage2.Controls.Add(this.btnCheckBarcode2D);
            this.tabPage2.Controls.Add(this.pictureBox2);
            this.tabPage2.Controls.Add(this.lblWidth2D);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.btnGenerateBarcode2D);
            this.tabPage2.Controls.Add(this.cBBorder2D);
            this.tabPage2.Controls.Add(this.nUDLines2D);
            this.tabPage2.Controls.Add(this.nUDScanGap2D);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gBLogo
            // 
            this.gBLogo.Controls.Add(this.btnLogoInvert);
            this.gBLogo.Controls.Add(this.btnLogoFromFile);
            this.gBLogo.Controls.Add(this.btnLogoFromClipboard);
            this.gBLogo.Controls.Add(this.pictureBox3);
            resources.ApplyResources(this.gBLogo, "gBLogo");
            this.gBLogo.Name = "gBLogo";
            this.gBLogo.TabStop = false;
            // 
            // btnLogoInvert
            // 
            resources.ApplyResources(this.btnLogoInvert, "btnLogoInvert");
            this.btnLogoInvert.Name = "btnLogoInvert";
            this.btnLogoInvert.UseVisualStyleBackColor = true;
            this.btnLogoInvert.Click += new System.EventHandler(this.BtnLogoInvert_Click);
            // 
            // btnLogoFromFile
            // 
            resources.ApplyResources(this.btnLogoFromFile, "btnLogoFromFile");
            this.btnLogoFromFile.Name = "btnLogoFromFile";
            this.btnLogoFromFile.UseVisualStyleBackColor = true;
            this.btnLogoFromFile.Click += new System.EventHandler(this.BtnLogoFromFile_Click);
            // 
            // btnLogoFromClipboard
            // 
            resources.ApplyResources(this.btnLogoFromClipboard, "btnLogoFromClipboard");
            this.btnLogoFromClipboard.Name = "btnLogoFromClipboard";
            this.btnLogoFromClipboard.UseVisualStyleBackColor = true;
            this.btnLogoFromClipboard.Click += new System.EventHandler(this.BtnLogoFromClipboard_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::GrblPlotter.Properties.Resources.logo;
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // tCQRPayload
            // 
            this.tCQRPayload.Controls.Add(this.tPQR1);
            this.tCQRPayload.Controls.Add(this.tPQR2);
            this.tCQRPayload.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tCQRPayload, "tCQRPayload");
            this.tCQRPayload.Name = "tCQRPayload";
            this.tCQRPayload.SelectedIndex = 0;
            // 
            // tPQR1
            // 
            this.tPQR1.Controls.Add(this.tBQRText);
            resources.ApplyResources(this.tPQR1, "tPQR1");
            this.tPQR1.Name = "tPQR1";
            this.tPQR1.UseVisualStyleBackColor = true;
            // 
            // tBQRText
            // 
            this.tBQRText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importBarcode2DText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBQRText, "tBQRText");
            this.tBQRText.Name = "tBQRText";
            this.tBQRText.Text = global::GrblPlotter.Properties.Settings.Default.importBarcode2DText;
            // 
            // tPQR2
            // 
            this.tPQR2.Controls.Add(this.tBQRURL);
            resources.ApplyResources(this.tPQR2, "tPQR2");
            this.tPQR2.Name = "tPQR2";
            this.tPQR2.UseVisualStyleBackColor = true;
            // 
            // tBQRURL
            // 
            this.tBQRURL.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importBarcode2DURL", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBQRURL, "tBQRURL");
            this.tBQRURL.Name = "tBQRURL";
            this.tBQRURL.Text = global::GrblPlotter.Properties.Settings.Default.importBarcode2DURL;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.radioButton1);
            this.tabPage3.Controls.Add(this.cBWLAN1);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.rBWLAN2);
            this.tabPage3.Controls.Add(this.rBWLAN1);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.tBWLAN2);
            this.tabPage3.Controls.Add(this.tBWLAN1);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.TabStop = true;
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // cBWLAN1
            // 
            resources.ApplyResources(this.cBWLAN1, "cBWLAN1");
            this.cBWLAN1.Name = "cBWLAN1";
            this.cBWLAN1.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // rBWLAN2
            // 
            resources.ApplyResources(this.rBWLAN2, "rBWLAN2");
            this.rBWLAN2.Name = "rBWLAN2";
            this.rBWLAN2.UseVisualStyleBackColor = true;
            // 
            // rBWLAN1
            // 
            resources.ApplyResources(this.rBWLAN1, "rBWLAN1");
            this.rBWLAN1.Checked = true;
            this.rBWLAN1.Name = "rBWLAN1";
            this.rBWLAN1.TabStop = true;
            this.rBWLAN1.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tBWLAN2
            // 
            this.tBWLAN2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importBarcode2DWlanPass", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBWLAN2, "tBWLAN2");
            this.tBWLAN2.Name = "tBWLAN2";
            this.tBWLAN2.Text = global::GrblPlotter.Properties.Settings.Default.importBarcode2DWlanPass;
            // 
            // tBWLAN1
            // 
            this.tBWLAN1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, "importBarcode2DWlanSSID", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBWLAN1, "tBWLAN1");
            this.tBWLAN1.Name = "tBWLAN1";
            this.tBWLAN1.Text = global::GrblPlotter.Properties.Settings.Default.importBarcode2DWlanSSID;
            // 
            // btnClipboardBarcode2D
            // 
            resources.ApplyResources(this.btnClipboardBarcode2D, "btnClipboardBarcode2D");
            this.btnClipboardBarcode2D.Name = "btnClipboardBarcode2D";
            this.btnClipboardBarcode2D.UseVisualStyleBackColor = true;
            this.btnClipboardBarcode2D.Click += new System.EventHandler(this.BtnClipboardBarcode2D_Click);
            // 
            // cBInsertLogo
            // 
            resources.ApplyResources(this.cBInsertLogo, "cBInsertLogo");
            this.cBInsertLogo.Name = "cBInsertLogo";
            this.cBInsertLogo.UseVisualStyleBackColor = true;
            this.cBInsertLogo.CheckedChanged += new System.EventHandler(this.CbInsertLogo_CheckedChanged);
            // 
            // btnCheckBarcode2D
            // 
            resources.ApplyResources(this.btnCheckBarcode2D, "btnCheckBarcode2D");
            this.btnCheckBarcode2D.Name = "btnCheckBarcode2D";
            this.btnCheckBarcode2D.UseVisualStyleBackColor = true;
            this.btnCheckBarcode2D.Click += new System.EventHandler(this.BtnCheckBarcode2D_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Image = global::GrblPlotter.Properties.Resources.qrcode;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // lblWidth2D
            // 
            resources.ApplyResources(this.lblWidth2D, "lblWidth2D");
            this.lblWidth2D.Name = "lblWidth2D";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // cBBorder2D
            // 
            resources.ApplyResources(this.cBBorder2D, "cBBorder2D");
            this.cBBorder2D.Checked = global::GrblPlotter.Properties.Settings.Default.importBarcode2DBorder;
            this.cBBorder2D.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBBorder2D.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "importBarcode2DBorder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBBorder2D.Name = "cBBorder2D";
            this.cBBorder2D.UseVisualStyleBackColor = true;
            // 
            // nUDLines2D
            // 
            this.nUDLines2D.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importBarcode2DLines", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.nUDLines2D, "nUDLines2D");
            this.nUDLines2D.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDLines2D.Name = "nUDLines2D";
            this.nUDLines2D.Value = global::GrblPlotter.Properties.Settings.Default.importBarcode2DLines;
            // 
            // nUDScanGap2D
            // 
            this.nUDScanGap2D.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, "importBarcode2DScanGap", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.nUDScanGap2D.DecimalPlaces = 2;
            this.nUDScanGap2D.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nUDScanGap2D, "nUDScanGap2D");
            this.nUDScanGap2D.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nUDScanGap2D.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nUDScanGap2D.Name = "nUDScanGap2D";
            this.nUDScanGap2D.Value = global::GrblPlotter.Properties.Settings.Default.importBarcode2DScanGap;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.AddExtension = false;
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // CbInsertCode
            // 
            resources.ApplyResources(this.CbInsertCode, "CbInsertCode");
            this.CbInsertCode.Checked = global::GrblPlotter.Properties.Settings.Default.fromFormInsertEnable;
            this.CbInsertCode.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, "fromFormInsertEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbInsertCode.Name = "CbInsertCode";
            this.CbInsertCode.UseVisualStyleBackColor = true;
            // 
            // GCodeForBarcode
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CbInsertCode);
            this.Controls.Add(this.tabControl1);
            this.Name = "GCodeForBarcode";
            this.Load += new System.EventHandler(this.GCodeForBarcode_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDHeight1D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLines1D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDScanGap1D)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.gBLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.tCQRPayload.ResumeLayout(false);
            this.tPQR1.ResumeLayout(false);
            this.tPQR1.PerformLayout();
            this.tPQR2.ResumeLayout(false);
            this.tPQR2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDLines2D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDScanGap2D)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnGenerateBarcode1D;
        private System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.Button btnGenerateBarcode2D;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnCheckBarcode1D;
        private System.Windows.Forms.Label lblWidth1D;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUDLines1D;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUDScanGap1D;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nUDHeight1D;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCheckBarcode2D;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblWidth2D;
        private System.Windows.Forms.TextBox tBQRText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nUDLines2D;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nUDScanGap2D;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cBBorder2D;
        private System.Windows.Forms.Button btnClipboardBarcode2D;
        private System.Windows.Forms.Button btnClipboardBarcode1D;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.CheckBox cBInsertLogo;
        private System.Windows.Forms.TabControl tCQRPayload;
        private System.Windows.Forms.TabPage tPQR1;
        private System.Windows.Forms.TabPage tPQR2;
        private System.Windows.Forms.TextBox tBQRURL;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox gBLogo;
        private System.Windows.Forms.Button btnLogoInvert;
        private System.Windows.Forms.Button btnLogoFromFile;
        private System.Windows.Forms.Button btnLogoFromClipboard;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox cBWLAN1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton rBWLAN2;
        private System.Windows.Forms.RadioButton rBWLAN1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tBWLAN2;
        private System.Windows.Forms.TextBox tBWLAN1;
        private System.Windows.Forms.RadioButton radioButton1;
        public System.Windows.Forms.CheckBox CbInsertCode;
    }
}