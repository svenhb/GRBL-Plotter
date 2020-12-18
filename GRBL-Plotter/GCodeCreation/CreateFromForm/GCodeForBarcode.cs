/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2020 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * 2020-06-24 First version, using https://github.com/barnhill/barcodelib (but removed unneeded functions)
				and using https://github.com/codebude/QRCoder
 * 2020-12-09 line 166, 264 no return of Gcode, must be picked up at Graphic.GCode
*/

using System;
using System.Windows.Forms;
using QRCoder;
using System.IO;
using static QRCoder.PayloadGenerator;
using System.Drawing;
using System.Drawing.Imaging;

namespace GRBL_Plotter
{
    public partial class GCodeForBarcode : Form
    {
//        private  string barcodegcode = "";
//        public  string barcodeGCode
//        { get { return barcodegcode; } }

        private System.Drawing.Bitmap qrCodeImage = null;
        private System.Drawing.Image barcodeImage = null;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public GCodeForBarcode()
        {   Logger.Trace("++++++ GCodeForBarcode START ++++++");
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }

        private void GCodeForBarcode_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < Enum.GetNames(typeof(BarcodeCreation.TYPE)).Length; i++)
                comboBox1.Items.Add(((BarcodeCreation.TYPE)i).ToString());
            comboBox1.SelectedIndex = Properties.Settings.Default.importBarcode1DType;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.importBarcode1DType = comboBox1.SelectedIndex;
        }

        private void btnCheckBarcode1D_Click(object sender, EventArgs e)
        {   Logger.Trace("Check Barcode Type:{0} Text:{1}", ((BarcodeCreation.TYPE)comboBox1.SelectedIndex).ToString(), textBox1.Text);
 //           barcodegcode = "";
            BarcodeCreation.Barcode b = new BarcodeCreation.Barcode();
            barcodeImage = b.Encode((BarcodeCreation.TYPE)comboBox1.SelectedIndex, textBox1.Text);//, Color.Black, Color.White, 200, 100);

            if (b.GetError)
            {   pictureBox1.Image = null; }
            else
            {   pictureBox1.Image = barcodeImage;
                btnClipboardBarcode1D.Visible = true;
            }
        }
        public void btnGenerateBarcode_Click(object sender, EventArgs e)
        {   Logger.Trace("Generate Barcode Type:{0} Text:{1}", ((BarcodeCreation.TYPE)comboBox1.SelectedIndex).ToString(), textBox1.Text);
//            barcodegcode = "";
            BarcodeCreation.Barcode b = new BarcodeCreation.Barcode();
            barcodeImage = b.Encode((BarcodeCreation.TYPE)comboBox1.SelectedIndex, textBox1.Text);//, Color.Black, Color.White, 200, 100);

            if (b.GetError)
            { 	pictureBox1.Image = null; }
            else
            {   pictureBox1.Image = barcodeImage;
                generateGCode1D(b.EncodedValue, b.EncodedType.ToString(), 40, (double)nUDHeight1D.Value);
                btnClipboardBarcode1D.Visible = true;
            }
        }

        private void generateGCode1D(string code, string type, double width, double height)
        { 	
			Graphic.Init(Graphic.SourceTypes.Barcode, "", null, null);
			Graphic.graphicInformation.ResetOptions(false);
            Graphic.graphicInformation.SetGroup(Graphic.GroupOptions.ByType, Graphic.SortOptions.none);

            Graphic.SetHeaderInfo(string.Format(" Barcode type :{0} ", type));
            Graphic.SetHeaderInfo(string.Format(" Barcode text :{0} ", textBox1.Text));
			
			
            double scanGap = (double)Properties.Settings.Default.importBarcode1DScanGap;	// 100 DPI = 3.94 DPmm
            int xwidth = (int)Properties.Settings.Default.importBarcode1DLines;
            bool last1 = false;
            bool lastUp = false;
            double overall = code.Length * scanGap * xwidth;

            Logger.Trace("generateGCode1D scanGap:{0} factor:{1} code.Length:{2} overall-width:{3}", scanGap, xwidth, code.Length, overall);

            lblWidth1D.Text = (scanGap * xwidth * code.Length).ToString();

            Graphic.SetHeaderInfo(string.Format(" Barcode width:{0} height:{1}", lblWidth1D.Text, height));
			Graphic.SetType("Barcode");
			Graphic.SetLayer(textBox1.Text);
			Graphic.SetPenColor("black");
            Graphic.SetPenWidth(scanGap.ToString().Replace(',','.'));

            Graphic.SetGeometry(type);

            double xpos = 0;
            for (int pos = 0; pos < code.Length; pos++)
            {
                if (code[pos] == '1')
                { if (!last1)
                    {
                        if (!lastUp)
                            Graphic.StartPath(new System.Windows.Point(xpos, 0));			// start new on bottom
                        else
                            Graphic.StartPath(new System.Windows.Point(xpos, height));          // start new on bottom
                    }
                    else
                    {
                        if (!lastUp)
                            Graphic.AddLine(new System.Windows.Point(xpos, 0));		// start new on top
                        else
                            Graphic.AddLine(new System.Windows.Point(xpos, height));		// start new on top

                    }
                    for (int i = 0; i < xwidth; i++)
                    {
                        if (i > 0)
                        {
                            if (!lastUp)
                                Graphic.AddLine(new System.Windows.Point(xpos, 0));            // connect verticals on bottom
                            else
                                Graphic.AddLine(new System.Windows.Point(xpos, height));   // connect verticals on top
                        }
                        if (!lastUp)
                            Graphic.AddLine(new System.Windows.Point(xpos, height));   // move up
                        else
                            Graphic.AddLine(new System.Windows.Point(xpos, 0));            // move down

                        xpos += scanGap;
                        lastUp = !lastUp;
                    }
                    last1 = true;
                }
                else
                { if (last1)
                        Graphic.StopPath();
                    xpos += scanGap * xwidth;
                    last1 = false;
                }
            }
            Graphic.CreateGCode();      // result is saved as stringbuilder in Graphic.GCode;
        }

        private void btnCheckBarcode2D_Click(object sender, EventArgs e)
        {   Logger.Trace("Check QR-Code");
            generateQR();
        }

        private void btnGenerateQRCode_Click(object sender, EventArgs e)
        {   Logger.Trace("Generate QR-Code");
            generateQR();
            generateGCode2D();
        }
        private void generateQR()
        {   //barcodegcode = "";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData;

            if (tCQRPayload.SelectedIndex == 0)
            {
                Logger.Trace("TabControl 0");
                qrCodeData = qrGenerator.CreateQrCode(tBQRText.Text, QRCodeGenerator.ECCLevel.Q);
            }
            else if(tCQRPayload.SelectedIndex == 1)
            {
                Logger.Trace("TabControl 1");
                Url generator = new Url(tBQRURL.Text);
                string payload = generator.ToString();
                qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            }
            else //if (tCQRPayload.SelectedIndex == 2)
            {
                Logger.Trace("TabControl 2");
                WiFi.Authentication auth = WiFi.Authentication.nopass;
                if (rBWLAN1.Checked) auth = WiFi.Authentication.WPA;
                if (rBWLAN2.Checked) auth = WiFi.Authentication.WEP;
                WiFi generator = new WiFi(tBWLAN1.Text, tBWLAN2.Text, auth, cBWLAN1.Checked);
                string payload = generator.ToString();
                qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            }


            QRCode qrCode = new QRCode(qrCodeData);

            if (cBInsertLogo.Checked)
                qrCodeImage = qrCode.GetGraphic((int)Properties.Settings.Default.importBarcode2DLines, System.Drawing.Color.Black, System.Drawing.Color.White, (System.Drawing.Bitmap)pictureBox3.Image);
            else
                qrCodeImage = qrCode.GetGraphic((int)Properties.Settings.Default.importBarcode2DLines);	// pixel per module
            pictureBox2.Image = qrCodeImage;
            btnClipboardBarcode2D.Visible = true;
        }

        public void generateGCode2D()
        {
            Cursor.Current = Cursors.WaitCursor;
            Graphic.Init(Graphic.SourceTypes.Barcode, "", null, null);
			Graphic.graphicInformation.ResetOptions(false);
            Graphic.graphicInformation.SetGroup(Graphic.GroupOptions.ByType, Graphic.SortOptions.none);

            Graphic.SetHeaderInfo(string.Format(" QR-Code text :{0} ", tBQRText.Text));
  //          Graphic.SetHeaderInfo(string.Format(" QR-Code width:{0} height:{1}", width, height));

            lastWasBlack = false;
            int xwidth = (int)nUDLines2D.Value;
            double scanGap = (double)nUDScanGap2D.Value;	// 100 DPI = 3.94 DPmm
            Logger.Trace("generateGCode2D scanGap:{0} factor:{1} QR width:{2}", scanGap, xwidth, qrCodeImage.Width);

            lblWidth2D.Text = (scanGap * qrCodeImage.Width).ToString();

            Graphic.SetHeaderInfo(string.Format(" Barcode width:{0} ", lblWidth2D.Text));
			Graphic.SetType("QR-Code");
			Graphic.SetLayer(tBQRText.Text);
			Graphic.SetPenColor("black");
            Graphic.SetPenWidth(scanGap.ToString().Replace(',', '.'));
            Graphic.SetGeometry("QR-Code");

            int xi = 0, yi = 0;
            while (xi < qrCodeImage.Width)
            {
                while (yi < qrCodeImage.Height)
                {   checkPixel(xi, yi, true, scanGap);
                    yi++;
                }
                yi--; xi++;
                while ((yi >= 0) && (xi < qrCodeImage.Width))
                {   checkPixel(xi, yi, false, scanGap);
                    yi--;
                }
                yi++; xi++;
            }
			if (cBBorder2D.Checked)
			{	Graphic.StartPath(new System.Windows.Point(-scanGap, -scanGap)); 
				Graphic.AddLine(new System.Windows.Point(-scanGap, scanGap * (qrCodeImage.Height + 1)));
				Graphic.AddLine(new System.Windows.Point(scanGap * (qrCodeImage.Width + 1), scanGap * (qrCodeImage.Height + 1)));
				Graphic.AddLine(new System.Windows.Point(scanGap * (qrCodeImage.Width + 1), -scanGap));
				Graphic.AddLine(new System.Windows.Point(-scanGap, -scanGap)); 
				Graphic.StopPath();
			}
            Graphic.CreateGCode();      // result is saved as stringbuilder in Graphic.GCode;
        }

        private  bool lastWasBlack = false;
        private  void checkPixel(int xp, int yp, bool goUp, double factor)
        {
            System.Drawing.Color qrColor = qrCodeImage.GetPixel(xp, yp);          // Get pixel color
//            Logger.Trace("checkPixel x:{0} y:{1} color:{2} last1:{3}", xp, yp, qrColor, last1);
            int ycnc = qrCodeImage.Height - yp;             // vertical flip
            int dir = 0;
            if (!goUp) dir = -1;

            bool isBlack = (qrColor.GetBrightness() < 0.5);

            if (isBlack)
            {
                if (!lastWasBlack)
                { Graphic.StartPath(new System.Windows.Point(factor * xp, factor * (ycnc + dir)));           // start new on bottom
                }
                lastWasBlack = true;
            }
            else
            {   if (lastWasBlack)
                {   Graphic.AddLine(new System.Windows.Point(factor * xp, factor * (ycnc + dir)));
                    Graphic.StopPath();
                }
                lastWasBlack = false;
            }
        }

        private void btnClipboardBarcode1D_Click(object sender, EventArgs e)
        { System.Windows.Forms.Clipboard.SetImage(pictureBox1.Image); }

        private void btnClipboardBarcode2D_Click(object sender, EventArgs e)
        { System.Windows.Forms.Clipboard.SetImage(pictureBox2.Image); }

        private void cBInsertLogo_CheckedChanged(object sender, EventArgs e)
        {  gBLogo.Visible = cBInsertLogo.Checked;   }

        private static bool firstUsage = true;

        private void btnLogoInvert_Click(object sender, EventArgs e)
        {
            Bitmap newBitmap = new Bitmap(pictureBox3.Image);
            Graphics g = Graphics.FromImage(newBitmap);
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                {
                    new float[] {-1, 0, 0, 0, 0},
                    new float[] {0, -1, 0, 0, 0},
                    new float[] {0, 0, -1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {1, 1, 1, 0, 1}
                });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(newBitmap, new Rectangle(0, 0, pictureBox3.Image.Width, pictureBox3.Image.Height),
               0, 0, pictureBox3.Image.Width, pictureBox3.Image.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();
			pictureBox3.Image = newBitmap;
        }
        private void btnLogoFromClipboard_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Clipboard.ContainsImage())
				pictureBox3.Image = System.Windows.Forms.Clipboard.GetImage();
		}

        private void btnLogoFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog sfd = new OpenFileDialog();
                if (firstUsage)
                    sfd.InitialDirectory = Application.StartupPath + datapath.examples;
                sfd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif;";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(sfd.FileName)) return;
                    pictureBox3.Image = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(sfd.FileName));
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error opening file: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

