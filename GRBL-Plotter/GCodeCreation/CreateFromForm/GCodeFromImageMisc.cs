/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
 * 2022-03-28  split code into ...Create and ...Outline
*/

using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class GCodeFromImage : Form
    {

        private static void UpdateLogging()
        {	// LogEnable { Level1=1, Level2=2, Level3=4, Level4=8, Detailed=16, Coordinates=32, Properties=64, Sort = 128, GroupAllGraphics = 256, ClipCode = 512, PathModification = 1024 }
            uint logFlags = (uint)Properties.Settings.Default.importLoggerSettings;
            logEnable = Properties.Settings.Default.guiExtendedLoggingEnabled && ((logFlags & (uint)LogEnables.Level1) > 0);
        }


        #region preset color correction
        private void BtnResetCorrection_Click(object sender, EventArgs e)
        { ResetColorCorrectionControls(); ApplyColorCorrections(""); lblImageSource.Text = "original"; }

        private void BtnPresetCorrection1_Click(object sender, EventArgs e)
        { ResetColorCorrectionControls(); PresetColorCorrectionControls(1); ApplyColorCorrections("BtnPresetCorrection1_Click"); }
        private void BtnPresetCorrection2_Click(object sender, EventArgs e)
        { ResetColorCorrectionControls(); PresetColorCorrectionControls(2); ApplyColorCorrections("BtnPresetCorrection2_Click"); }
        private void BtnPresetCorrection3_Click(object sender, EventArgs e)     // Comic - few colors
        { ResetColorCorrectionControls(); PresetColorCorrectionControls(3); ApplyColorCorrections("BtnPresetCorrection3_Click"); }
        private void BtnPresetCorrection4_Click(object sender, EventArgs e)
        {
            ResetColorCorrectionControls(); PresetColorCorrectionControls(4); ApplyColorCorrections("BtnPresetCorrection4_Click"); adjustedImage = ImgInvert(adjustedImage);
            originalImage = new Bitmap(adjustedImage); ResetColorCorrectionControls();
            DisableControlEvents();
            {
                cbGrayscale.Checked = true; tBarGamma.Value = 10;
                cbExceptColor.Checked = true; ToolTable.SetExceptionColor(cbExceptColor.BackColor);
                cBPreview.Checked = true; cBReduceColorsToolTable.Checked = true;
                if (nUDMaxColors.Maximum >= 2)
                    nUDMaxColors.Value = 2;
            }
            EnableControlEvents(); ApplyColorCorrections("BtnPresetCorrection4_Click2");
        }

        private void ResetColorCorrectionControls()
        {
            if (logEnable) Logger.Trace("ResetColorCorrectionControls ");
            DisableControlEvents();
            {
                tBarBrightness.Value = 0;
                tBarContrast.Value = 0;
                tBarGamma.Value = 100;
                tBarBWThreshold.Value = 127;
                tBarSaturation.Value = 0;
                if (cbGrayscale.Enabled)
                    cbGrayscale.Checked = false;
                tBRMin.Value = 0; tBRMax.Value = 255;
                tBGMin.Value = 0; tBGMax.Value = 255;
                tBBMin.Value = 0; tBBMax.Value = 255;
                tBarHueShift.Value = 0;
                cBFilterRemoveArtefact.Checked = false;
                cBFilterEdge.Checked = false;
                cBPosterize.Checked = false;
                cBFilterHistogram.Checked = false;
                //    GetToolTableSettings();          // get actual toolTable and show
                cBReduceColorsToolTable.Checked = false;
                cBReduceColorsDithering.Checked = false;
                rBMode0.Checked = true;
                //   cbExceptColor.Checked = false;
                cbExceptColor.BackColor = Color.White;
                ToolTable.ClrExceptionColor();
                cBPreview.Checked = false;
                redoColorAdjust = false;

                CbBlackWhiteEnable.Checked = false;
				if (useColorMode)
                {	cBGCodeFill.Checked = true;				// only preset in color mode
					cBGCodeOutline.Checked = true;
					cBGCodeOutlineSmooth.Checked = true;
					cBGCodeOutlineShrink.Checked = true;
				}
				else
				{
					if (!cBGCodeFill.Checked && !cBGCodeOutline.Checked)
					{	cBGCodeOutline.Checked = true;}					// at least one must be checked
				}
				
                //           updatePixelCountPerColorNeeded = false;
                lblImageSource.Text = "original";
            }
            EnableControlEvents();
            updateLabelColorCount = true;
        }
        private void DisableControlEvents()
        {
            preventEvent = true;
            tBarBrightness.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBarContrast.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBarGamma.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBarSaturation.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBarHueShift.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBarBWThreshold.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBRMin.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBRMax.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBGMin.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBGMax.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBBMin.Scroll -= ApplyColorCorrectionsEventScrollBar;
            tBBMax.Scroll -= ApplyColorCorrectionsEventScrollBar;
            cBGCodeFill.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBGCodeOutline.CheckedChanged -= CbGCodeOutline_CheckedChanged;
            cBGCodeOutlineSmooth.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBGCodeOutlineShrink.CheckedChanged -= ApplyColorCorrectionsEvent;
            nUDGCodeOutlineSmooth.ValueChanged -= ApplyColorCorrectionsEvent;

            CbBlackWhiteEnable.CheckedChanged -= ApplyColorCorrectionsEvent;
            cbGrayscale.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBPosterize.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBFilterRemoveArtefact.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBFilterEdge.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBFilterHistogram.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBReduceColorsToolTable.CheckedChanged -= ApplyColorCorrectionsEvent;
            cBReduceColorsDithering.CheckedChanged -= ApplyColorCorrectionsEvent;
            nUDMaxColors.ValueChanged -= ApplyColorCorrectionsEvent;
            cBPreview.CheckedChanged -= JustShowResult;
            cbExceptColor.CheckedChanged -= CbExceptColor_CheckedChanged;
            rBMode0.CheckedChanged -= RbMode0_CheckedChanged;
            rBMode1.CheckedChanged -= RbMode0_CheckedChanged;
            rBMode2.CheckedChanged -= RbMode0_CheckedChanged;
			nUDResoX.ValueChanged -= ApplyColorCorrectionsEvent;
			nUDResoY.ValueChanged -= ApplyColorCorrectionsEvent;
            RbGrayscaleVector.CheckedChanged -= RbGrayscaleVector_CheckedChanged;
            RbGrayscalePattern.CheckedChanged -= RbGrayscaleVector_CheckedChanged;
            tabControl2.SelectedIndexChanged -= TabControl2_SelectedIndexChanged;
            RbStartGrayS.CheckedChanged -= RbGrayZ_CheckedChanged;
            RbStartGrayZ.CheckedChanged -= RbGrayZ_CheckedChanged;
        }
        private void EnableControlEvents()
        {
            preventEvent = false;
            tBarBrightness.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBarContrast.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBarGamma.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBarSaturation.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBarHueShift.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBarBWThreshold.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBRMin.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBRMax.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBGMin.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBGMax.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBBMin.Scroll += ApplyColorCorrectionsEventScrollBar;
            tBBMax.Scroll += ApplyColorCorrectionsEventScrollBar;
            cBGCodeFill.CheckedChanged += ApplyColorCorrectionsEvent;
            cBGCodeOutline.CheckedChanged += CbGCodeOutline_CheckedChanged;
            cBGCodeOutlineSmooth.CheckedChanged += ApplyColorCorrectionsEvent;
            cBGCodeOutlineShrink.CheckedChanged += ApplyColorCorrectionsEvent;
            nUDGCodeOutlineSmooth.ValueChanged += ApplyColorCorrectionsEvent;

            CbBlackWhiteEnable.CheckedChanged += ApplyColorCorrectionsEvent;
            cbGrayscale.CheckedChanged += ApplyColorCorrectionsEvent;
            cBPosterize.CheckedChanged += ApplyColorCorrectionsEvent;
            cBFilterRemoveArtefact.CheckedChanged += ApplyColorCorrectionsEvent;
            cBFilterEdge.CheckedChanged += ApplyColorCorrectionsEvent;
            cBFilterHistogram.CheckedChanged += ApplyColorCorrectionsEvent;
            cBReduceColorsToolTable.CheckedChanged += ApplyColorCorrectionsEvent;
            cBReduceColorsDithering.CheckedChanged += ApplyColorCorrectionsEvent;
            nUDMaxColors.ValueChanged += ApplyColorCorrectionsEvent;
            cBPreview.CheckedChanged += JustShowResult;
            cbExceptColor.CheckedChanged += CbExceptColor_CheckedChanged;
            rBMode0.CheckedChanged += RbMode0_CheckedChanged;
            rBMode1.CheckedChanged += RbMode0_CheckedChanged;
            rBMode2.CheckedChanged += RbMode0_CheckedChanged;
			nUDResoX.ValueChanged += ApplyColorCorrectionsEvent;
			nUDResoY.ValueChanged += ApplyColorCorrectionsEvent;
            RbGrayscaleVector.CheckedChanged += RbGrayscaleVector_CheckedChanged;
            RbGrayscalePattern.CheckedChanged += RbGrayscaleVector_CheckedChanged;
            tabControl2.SelectedIndexChanged += TabControl2_SelectedIndexChanged;
            RbStartGrayS.CheckedChanged += RbGrayZ_CheckedChanged;
            RbStartGrayZ.CheckedChanged += RbGrayZ_CheckedChanged;
        }

        private static bool redoColorAdjust = false;
        private void PresetColorCorrectionControls(int tmp)
        {
            Logger.Info("◯◯◯ PresetColorCorrectionControls :{0}", tmp);
            DisableControlEvents();
            {
                if (tmp == 1)                   // Image dark background
                {
                    tBarSaturation.Value = 128;
                    tBRMin.Value = 64;
                    tBGMin.Value = 64;
                    tBBMin.Value = 64;
                    cbExceptColor.Checked = true;
                    cbExceptColor.BackColor = Color.FromArgb(255, 0, 0, 0);
                    ToolTable.SetExceptionColor(cbExceptColor.BackColor);
                    cBGCodeOutline.Checked = false;
                }
                else if (tmp == 2)              // graphic many colors
                {
                    cBPosterize.Checked = true;
                    rBMode2.Checked = true;
                }
                else if (tmp == 3)              // comic few colors
                {
                    //        if (imageColors > toolTableCount)
                    {
                        tBarBrightness.Value = -15;
                        tBarContrast.Value = 20;
                        tBarGamma.Value = 88;
                    }
                    //     else
                    //     { nUDColorPercent.Value = 1; }
                    cBReduceColorsToolTable.Checked = true;
                    cbExceptColor.Checked = true;               // except white
                    cBFilterRemoveArtefact.Checked = true;
                    rBMode0.Checked = true;
                    redoColorAdjust = true;
                    applyPercentLimit = true;
                }
                else if (tmp == 4)              // comic few colors
                {
                    cbGrayscale.Checked = true;
                    rbModeGray.Checked = true;
                    cBFilterEdge.Checked = true;
                }
                cBPreview.Checked = true;
            }
            EnableControlEvents();
        }
        private string ListColorCorrection()
        {
            string tmp = "";
            tmp += "Reduce colors \t" + (cBReduceColorsToolTable.Checked ? "on" : "off") + " " + Convert.ToString(nUDMaxColors.Value) + "\r\n";
            tmp += "Dithering  \t" + (cBReduceColorsDithering.Checked ? "on" : "off") + "\r\n";
            tmp += "Except. col.\t" + (cbExceptColor.Checked ? "on" : "off") + " " + Convert.ToString(cbExceptColor.BackColor) + "\r\n";
            tmp += "Brightness \t" + Convert.ToString(tBarBrightness.Value) + "\r\n";
            tmp += "Contrast   \t" + Convert.ToString(tBarContrast.Value) + "\r\n";
            tmp += "Gamma      \t" + Convert.ToString(tBarGamma.Value / 100.0f) + "\r\n";
            tmp += "Saturation \t" + Convert.ToString(tBarSaturation.Value) + "\r\n";
            tmp += "Grayscale  \t" + (cbGrayscale.Checked ? "on" : "off") + "\r\n";
            tmp += "Channel filter red   \t" + Convert.ToString(tBRMin.Value) + ";" + Convert.ToString(tBRMax.Value) + "\r\n";
            tmp += "Channel filter green \t" + Convert.ToString(tBGMin.Value) + ";" + Convert.ToString(tBGMax.Value) + "\r\n";
            tmp += "Channel filter blue  \t" + Convert.ToString(tBBMin.Value) + ";" + Convert.ToString(tBBMax.Value) + "\r\n";
            tmp += "Hue shift  \t" + Convert.ToString(tBarHueShift.Value) + "\r\n";
            tmp += "Edge filter   \t" + (cBFilterEdge.Checked ? "on" : "off") + "\r\n";
            tmp += "Histogram equ.\t" + (cBFilterHistogram.Checked ? "on" : "off") + "\r\n";
            tmp += "Posterize     \t" + (cBPosterize.Checked ? "on" : "off") + "\r\n";
            return tmp;
        }

        private void UpdateLabels()
        {
            lblBrightness.Text = Convert.ToString(tBarBrightness.Value);
            lblContrast.Text = Convert.ToString(tBarContrast.Value);
            lblGamma.Text = Convert.ToString(tBarGamma.Value / 100.0f);
            lblSaturation.Text = Convert.ToString(tBarSaturation.Value);
            lblHueShift.Text = Convert.ToString(tBarHueShift.Value);
            lblCFR.Text = Convert.ToString(tBRMin.Value) + ";" + Convert.ToString(tBRMax.Value);
            lblCFG.Text = Convert.ToString(tBGMin.Value) + ";" + Convert.ToString(tBGMax.Value);
            lblCFB.Text = Convert.ToString(tBBMin.Value) + ";" + Convert.ToString(tBBMax.Value);
            LblThreshold.Text = Convert.ToString(tBarBWThreshold.Value);
            Refresh();
        }
        private void BtnShowSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ListColorCorrection(), "Color correction settings");
            string text = ListColorCorrection();
            if (!string.IsNullOrEmpty(text))
                System.Windows.Forms.Clipboard.SetText(ListColorCorrection());
        }
        #endregion


        #region image manipulation

        private void NudWidthHeight_ValueChanged(object sender, EventArgs e)
        {
            nUDWidth.ValueChanged -= NudWidthHeight_ValueChanged;
            nUDHeight.ValueChanged -= NudWidthHeight_ValueChanged;
            bool edit = false;
            if (AspectRatioOriginalImage == 0)
            {
                Logger.Error("NudWidthHeight_ValueChanged ratio=0  width:{0}  height:{1}", originalImage.Width, originalImage.Height);
                AspectRatioOriginalImage = 1;
            }
            if (oldWidth != nUDWidth.Value)
            {
                oldWidth = nUDWidth.Value;
                if (cbLockRatio.Checked)
                {
                    oldHeight = oldWidth / AspectRatioOriginalImage;
                    if (nUDHeight.Minimum > oldHeight) { nUDHeight.Minimum = oldHeight; }
                    if (nUDHeight.Maximum < oldHeight) { nUDHeight.Maximum = oldHeight; }
                    nUDHeight.Value = oldHeight;
                    nUDHeight.Invalidate();
                }
                nUDWidth.Value = oldWidth;
                edit = true;
            }
            if (oldHeight != nUDHeight.Value)
            {
                oldHeight = nUDHeight.Value;
                if (cbLockRatio.Checked)
                {
                    oldWidth = oldHeight * AspectRatioOriginalImage;
                    if (nUDWidth.Minimum > oldWidth) { nUDWidth.Minimum = oldWidth; }
                    nUDWidth.Value = oldWidth;
                    nUDWidth.Invalidate();
                }
                nUDHeight.Value = oldHeight;
                edit = true;
            }
            if (edit)
                ApplyColorCorrections("NudWidthHeight_ValueChanged");
            nUDWidth.ValueChanged += NudWidthHeight_ValueChanged;
            nUDHeight.ValueChanged += NudWidthHeight_ValueChanged;
            Refresh();
        }

        private void BtnKeepSizeWidth_Click(object sender, EventArgs e)
        {
            decimal val = originalImage.Width * nUDResoX.Value;
            if (nUDWidth.Maximum < val) { nUDWidth.Maximum = val; }
            if (nUDWidth.Minimum > val) { nUDWidth.Minimum = val; }
            nUDWidth.Value = val;
        }

        private void BtnKeepSizeReso_Click(object sender, EventArgs e)
        {
            decimal val = (decimal)(nUDWidth.Value / originalImage.Width);
            if (nUDResoX.Maximum < val) { nUDResoX.Maximum = val; }
            if (nUDResoX.Minimum > val) { nUDResoX.Minimum = val; }
            nUDResoX.Value = val;
        }


        //Horizontal mirroing
        private void BtnHorizMirror_Click(object sender, EventArgs e)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox1.Image = adjustedImage;
        }
        //Vertical mirroing
        private void BtnVertMirror_Click(object sender, EventArgs e)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            pictureBox1.Image = adjustedImage;
            ShowResultImage();
        }
        //Rotate right
        private void BtnRotateRight_Click(object sender, EventArgs e)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            AspectRatioOriginalImage = 1 / AspectRatioOriginalImage;
            decimal s = nUDHeight.Value;
            nUDHeight.Value = nUDWidth.Value;
            nUDWidth.Value = s;
            pictureBox1.Image = adjustedImage;
            AutoZoomToolStripMenuItem_Click(this, null);

			if (useColorMode)
				GenerateResultImage(ref resultToolNrArray);      // fill countColors
			else
				GenerateResultImageGray(ref resultToolNrArray);      

            ShowResultImage();
        }
        //Rotate left
        private void BtnRotateLeft_Click(object sender, EventArgs e)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            AspectRatioOriginalImage = 1 / AspectRatioOriginalImage;
            decimal s = nUDHeight.Value;
            nUDHeight.Value = nUDWidth.Value;
            nUDWidth.Value = s;
            pictureBox1.Image = adjustedImage;
            AutoZoomToolStripMenuItem_Click(this, null);
			
			if (useColorMode)
				GenerateResultImage(ref resultToolNrArray);      // fill countColors
			else
				GenerateResultImageGray(ref resultToolNrArray);      
			
            ShowResultImage();
        }
        //Invert image color
        private void BtnInvert_Click(object sender, EventArgs e)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage = ImgInvert(adjustedImage);
            originalImage = ImgInvert(originalImage);
            pictureBox1.Image = adjustedImage;
            ShowResultImage();
        }


        private void Channel_changed(object sender, EventArgs e)
        {
            cbGrayscaleChannel.CheckedChanged -= ApplyColorCorrectionsEvent;
            cbGrayscaleChannel.Checked = true;
            cbGrayscaleChannel.CheckedChanged += ApplyColorCorrectionsEvent;
            ApplyColorCorrectionsEvent(sender, e);
        }


        private static int conversionMode = 0, conversionModeOld = 0;
        private void RbMode0_CheckedChanged(object sender, EventArgs e)
        {
            conversionMode = 0;
            if (rBMode1.Checked) conversionMode = 1;
            else if (rBMode2.Checked) conversionMode = 2;
            if (conversionMode != conversionModeOld)
                ApplyColorCorrections("RbMode0_CheckedChanged");
            conversionModeOld = conversionMode;
        }

        //Apply dithering to an image (Convert to 1 bit)
        private Bitmap ImgDither(Bitmap input)
        {
            lblStatus.Text = "Dithering...";
            Refresh();
            var masks = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format1bppIndexed);
            var data = new sbyte[input.Width, input.Height];
            var inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                var scanLine = inputData.Scan0;
                var line = new byte[inputData.Stride];
                for (var y = 0; y < inputData.Height; y++, scanLine += inputData.Stride)
                {
                    Marshal.Copy(scanLine, line, 0, line.Length);
                    for (var x = 0; x < input.Width; x++)
                    {
                        data[x, y] = (sbyte)(64 * (GetGreyLevel(line[x * 3 + 2], line[x * 3 + 1], line[x * 3 + 0]) - 0.5));
                    }
                }
            }
            finally
            {
                input.UnlockBits(inputData);
            }

            var outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            try
            {
                var scanLine = outputData.Scan0;
                for (var y = 0; y < outputData.Height; y++, scanLine += outputData.Stride)
                {
                    var line = new byte[outputData.Stride];
                    for (var x = 0; x < input.Width; x++)
                    {
                        var j = data[x, y] > 0;
                        if (j) line[x / 8] |= masks[x % 8];
                        var error = (sbyte)(data[x, y] - (j ? 32 : -32));
                        if (x < input.Width - 1) data[x + 1, y] += (sbyte)(7 * error / 16);
                        if (y < input.Height - 1)
                        {
                            if (x > 0) data[x - 1, y + 1] += (sbyte)(3 * error / 16);
                            data[x, y + 1] += (sbyte)(5 * error / 16);
                            if (x < input.Width - 1) data[x + 1, y + 1] += (sbyte)(1 * error / 16);
                        }
                    }
                    Marshal.Copy(line, 0, scanLine, outputData.Stride);
                }
            }
            finally
            {
                output.UnlockBits(outputData);
            }
            lblStatus.Text = "Done";
            Refresh();
            return (output);
        }

        private static double GetGreyLevel(byte r, byte g, byte b)//aux for dirthering
        { return ((double)(r * 0.299) + (double)(g * 0.587) + (double)(b * 0.114)) / (double)255; }
        //Adjust brightness contrast and gamma of an image      
        public static Bitmap ImgBalance(Bitmap img, int brigh, int cont, int gam)
        {
            ImageAttributes imageAttributes;
            float brightness = (brigh / 100.0f) + 1.0f;
            float contrast = (cont / 100.0f) + 1.0f;
            float gamma = 1 / (gam / 100.0f);
            float adjustedBrightness = brightness - 1.0f;
            Bitmap output;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
            new float[] {contrast, 0, 0, 0, 0}, // scale red
            new float[] {0, contrast, 0, 0, 0}, // scale green
            new float[] {0, 0, contrast, 0, 0}, // scale blue
            new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
            new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            output = new Bitmap(img);
            imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(output);
            //            g.DrawRectangle(new Pen(new SolidBrush(Color.White)), 0, 0, output.Width, output.Height);   // remove transparency
            g.DrawImage(output, new Rectangle(0, 0, output.Width, output.Height)
            , 0, 0, output.Width, output.Height,
            GraphicsUnit.Pixel, imageAttributes);
            imageAttributes.Dispose();
            return (output);
        }

        private static Bitmap RemoveAlpha(Bitmap img)
        {

            Color myColor;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    myColor = img.GetPixel(x, y);    // Get pixel color
                    if (myColor.A < 128)
                    {
                        myColor = Color.FromArgb(255, 255, 255, 255);
                        img.SetPixel(x, y, myColor);
                    }
                }
            }
            return img;
            /*        Bitmap output = new Bitmap(img.Width, img.Height);//, PixelFormat.Format24bppRgb);
                    Graphics g = Graphics.FromImage(output);
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White)), 0, 0, output.Width, output.Height);   // remove transparency
                    g.DrawImage(img, 0, 0);
                    return (output);*/
        }

        //Return a grayscale version of an image
        private Bitmap ImgGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);//create a blank bitmap the same size as original
            Graphics g = Graphics.FromImage(newBitmap);//get a graphics object from the new image
                                                       //create the grayscale ColorMatrix
            bool useCMYK = false;
            ImageAttributes cmykAttributes = new ImageAttributes();
            ColorMatrix colorMatrixChannel;
            colorMatrixChannel = new ColorMatrix(
                    new float[][]
                    {
                        new float[] {.299f, .299f, .299f, 0, 0},
                        new float[] {.587f, .587f, .587f, 0, 0},
                        new float[] {.114f, .114f, .114f, 0, 0},
                        new float[] {    0,     0,     0, 1, 0},
                        new float[] {    0,     0,     0, 0, 1}
                    });
            if (cbGrayscaleChannel.Checked)
            {
                if (RbChannelR.Checked)
                {
                    colorMatrixChannel = new ColorMatrix(
                    new float[][]
                    {
                    //    new float[] {0, 0, 0, 0, 0},
                        //new float[] {0.5f, 0.5f, 0.5f, 0, 0},   //new float[] {1, 1, 1, 0, 0},   //new float[] {.84f, .84f, .84f, 0, 0},
                        //new float[] {0.5f, 0.5f, 0.5f, 0, 0},   //new float[] {1, 1, 1, 0, 0},   //new float[] {.16f, .16f, .16f, 0, 0},
                        new float[] {-1, -1, -1, 0, 0}, //
                        new float[] {1, 1, 1, 0, 0},    //
                        new float[] {1, 1, 1, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });
                }
                else if (RbChannelG.Checked)
                {
                    colorMatrixChannel = new ColorMatrix(
                    new float[][]
                    {
                        new float[] {1, 1, 1, 0, 0},    //new float[] {0.5f, 0.5f, 0.5f, 0, 0},   //new float[] {1, 1, 1, 0, 0},   //new float[] {.73f, .73f, .73f, 0, 0},
                        new float[] {-1, -1, -1, 0, 0}, //new float[] {0, 0, 0, 0, 0},
                        new float[] {1, 1, 1, 0, 0},    //new float[] {0.5f, 0.5f, 0.5f, 0, 0},   //new float[] {1, 1, 1, 0, 0},   //new float[] {.27f, .27f, .27f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });
                }
                else if (RbChannelB.Checked)
                {
                    colorMatrixChannel = new ColorMatrix(
                    new float[][]
                    {
                        new float[] {1, 1, 1, 0, 0},    //new float[] {0.5f, 0.5f, 0.5f, 0, 0},   //new float[] {1, 1, 1, 0, 0},   //new float[] {.34f, .34f, .34f, 0, 0},
                        new float[] {1, 1, 1, 0, 0},    //new float[] {0.5f, 0.5f, 0.5f, 0, 0},   //new float[] {1, 1, 1, 0, 0},   //new float[] {.66f, .66f, .66f, 0, 0},
                        new float[] {-1, -1, -1, 0, 0}, //new float[] {0, 0, 0, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });
                }
                else if (RbChannelC.Checked)
                {
                    cmykAttributes.SetOutputChannel(ColorChannelFlag.ColorChannelC,
                                    System.Drawing.Imaging.ColorAdjustType.Bitmap);
                    useCMYK = true;
                }
                else if (RbChannelM.Checked)
                {
                    cmykAttributes.SetOutputChannel(ColorChannelFlag.ColorChannelM,
                                    System.Drawing.Imaging.ColorAdjustType.Bitmap);
                    useCMYK = true;
                }
                else if (RbChannelY.Checked)
                {
                    cmykAttributes.SetOutputChannel(ColorChannelFlag.ColorChannelY,
                                    System.Drawing.Imaging.ColorAdjustType.Bitmap);
                    useCMYK = true;
                }
                else if (RbChannelK.Checked)
                {
                    cmykAttributes.SetOutputChannel(ColorChannelFlag.ColorChannelK,
                                    System.Drawing.Imaging.ColorAdjustType.Bitmap);
                    useCMYK = true;
                }
            }

            ImageAttributes attributes = new ImageAttributes();     //create some image attributes
            if (useCMYK)
            {
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, cmykAttributes);
            }
            else
            {
                attributes.SetColorMatrix(colorMatrixChannel);       //set the color matrix attribute
                                                                     //draw the original image on the new image using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            g.Dispose();//dispose the Graphics object
            attributes.Dispose();
            return (newBitmap);
        }

        //Return a inverted colors version of a image
        private static Bitmap ImgInvert(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);//create a blank bitmap the same size as original
            Graphics g = Graphics.FromImage(newBitmap);//get a graphics object from the new image
            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                {
                    new float[] {-1, 0, 0, 0, 0},
                    new float[] {0, -1, 0, 0, 0},
                    new float[] {0, 0, -1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {1, 1, 1, 0, 1}
                });
            ImageAttributes attributes = new ImageAttributes();//create some image attributes
            attributes.SetColorMatrix(colorMatrix);//set the color matrix attribute

            //draw the original image on the new image using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();//dispose the Graphics object
            attributes.Dispose();
            return (newBitmap);
        }

        #endregion



        /// <summary>
        /// Calculate contrast color from given color
        /// </summary>
        private static Color ContrastColor(Color color)
        {
            int d;
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

        private void FillUseCaseFileList(string filepath)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(filepath);
                lBUseCase.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("ini"))
                        lBUseCase.Items.Add(Path.GetFileName(Files[i]));
                }
                if (lBUseCase.Items.Count == 0)
                {
                    lBUseCase.Items.Add("No files found");
                    Logger.Error("fillUseCaseFileList no files found in {0}", filepath);
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "fillUseCaseFileList no file found {0}", filepath);
            }
        }

        private void FillPatternFilesList(string filepath)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(filepath);
                CBoxPatternFiles.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Path.GetFileName(Files[i]).ToLower().StartsWith("pattern"))
                        CBoxPatternFiles.Items.Add(Path.GetFileName(Files[i]));
                }
                if (CBoxPatternFiles.Items.Count == 0)
                {
                    CBoxPatternFiles.Items.Add("No files found");
                    Logger.Error("fillPatternFilesList no files found in {0}", filepath);
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "fillPatternFilesList no file found {0}", filepath);
            }
        }

        public string patternFile = "";
        private void CBoxPatternFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            patternFile = CBoxPatternFiles.SelectedItem.ToString();
        }

        /**********************************
        ***** 
        ***********************************/
        private void UpdateToolTableList()
        {
            if (logEnable) Logger.Trace("UpdateToolTableList");
            FillToolTableFileList(Datapath.Tools);			// list tool table files
            CboxToolFiles.Text = "Last loaded: " + Properties.Settings.Default.toolTableLastLoaded;

            string defaultToolList = Datapath.Tools + "\\" + ToolTable.DefaultFileName;
            LoadToolList(defaultToolList);                  // list tools from _current.csv			
        }

        /* list all CSV files from tools-folder */
        private void FillToolTableFileList(string filepath)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(filepath);
                CboxToolFiles.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("csv"))
                    {
                        string name = Path.GetFileName(Files[i]);
                        if (name != ToolTable.DefaultFileName)
                            CboxToolFiles.Items.Add(name);
                    }
                }
            }
            catch (Exception err) { Logger.Error(err, "FillToolTableFileList "); }
        }

        /* copy selected tool-file to _current.csv */
        private void CboxToolFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CboxToolFiles.Text))
            {
                string newTool = Datapath.Tools + "\\" + CboxToolFiles.Text;
                string defTool = Datapath.Tools + "\\" + ToolTable.DefaultFileName;
                try
                {
                    System.IO.File.Copy(newTool, defTool, true);
                    Properties.Settings.Default.toolTableLastLoaded = CboxToolFiles.Text;
                }
                catch (Exception err)
                {
                    Logger.Error(err, "CboxToolFiles_SelectedIndexChanged could not copy file to default {0}", newTool);
                    EventCollector.StoreException("CboxToolFiles_SelectedIndexChanged: " + newTool + " " + err.Message + " - ");
                }

                LoadToolList(defTool);
                GetToolTableSettings();
            }
        }

        /* Display tools from selected tool table */
        private bool LoadToolList(string file)
        {
			UpdateLogging();
            if (File.Exists(file))
            {
                Logger.Trace("Load Tool Table {0}", file);
                CboxToolTable.Items.Clear();
                string[] readText;

                try
                {
                    readText = File.ReadAllLines(file);
                }
                catch (IOException err)
                {
                    // read already opened file???
                    // https://stackoverflow.com/questions/9759697/reading-a-file-used-by-another-process
                    EventCollector.StoreException("LoadToolList IOException: " + file + " " + err.Message + " - ");
                    Logger.Error(err, "LoadToolList IOException:{0}", file);
                    MessageBox.Show("Could not read " + file + "\r\n" + err.Message, "Error");
                    return false;
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }

                string[] col;
                CboxToolTable.Items.Add("Nr  Color  Name              Pen-Width");
                foreach (string s in readText)
                {
                    if (s.StartsWith("#") || s.StartsWith("/"))     // jump over comments
                        continue;

                    if (s.Length > 10)
                    {
                        col = s.Split(',');
                        if (col.Length > 7)
                            CboxToolTable.Items.Add(string.Format("{0,3} {1,6} {2,20} {3} ", col[0].Trim(), col[1].Trim(), col[2], col[7]));
                        else
                            CboxToolTable.Items.Add(s);
                    }
                }
                return true;
            }
            return false;
        }

        /* OwnerDrawFixed - colorize tool-entry lines */
        private void CboxToolTable_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            string text = ((ComboBox)sender).Items[e.Index].ToString();
            SolidBrush brush = new SolidBrush(Color.Black);
            if ((e.Index > 0) && (text.Length > 10))
            {
                //    Logger.Info("CboxToolTable_DrawItem  '{0}'  {1}", text.Substring(4, 6), text);
                try
                {
                    long clr = Convert.ToInt32(text.Substring(4, 6), 16) | 0xff000000;
                    brush = new SolidBrush(ContrastColor(Color.FromArgb((int)clr)));
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)clr)), e.Bounds);
                }
                catch { }
                e.Graphics.DrawString(text, ((Control)sender).Font, brush, e.Bounds.X, e.Bounds.Y);
            }
            else { e.Graphics.DrawString(text, ((Control)sender).Font, brush, e.Bounds.X, e.Bounds.Y); }
        }

        private void RbGrayscaleVector_CheckedChanged(object sender, EventArgs e)
        {
            if (RbGrayscaleVector.Checked)
            {
                gBgcodeSelection.BackColor = Color.Yellow;
                GbGcodeDirection.BackColor = Color.WhiteSmoke;
            }
            else
            {
                gBgcodeSelection.BackColor = Color.WhiteSmoke;
                GbGcodeDirection.BackColor = Color.Yellow;
            }
            ResetColorCorrectionControls(); 
			ApplyColorCorrections("RbGrayscaleVector_CheckedChanged"); 
			lblImageSource.Text = "original";
        }

        /* OwnerDrawFixed - colorize tab-handle */
        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)	// highlight color or grayscale mode
        {
            TabPage page = tabControl1.TabPages[e.Index];
            Color col = e.Index == tabControl1.SelectedIndex ? Color.Yellow : Color.White;
            e.Graphics.FillRectangle(new SolidBrush(col), e.Bounds);

            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, Font, paddedBounds, page.ForeColor);
        }
        /* OwnerDrawFixed - colorize tab-handle */
        private void TabControl2_DrawItem(object sender, DrawItemEventArgs e)	// highlight color or grayscale mode
        {
            TabPage page = tabControl2.TabPages[e.Index];
            Color col = e.Index == tabControl2.SelectedIndex ? Color.Yellow : Color.White;
            e.Graphics.FillRectangle(new SolidBrush(col), e.Bounds);

            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, Font, paddedBounds, page.ForeColor);
        }


        public static void SortByGrayAmount(bool invert)
        {
            if ((GrayValueMap == null) || (GrayValueMap.Count == 0))
            {
                Logger.Error("SortByGrayAmount toolTableArray is empty - do Init");
            }
            List<GrayProp> SortedList;
            if (!invert)
                SortedList = GrayValueMap.OrderBy(o => o.Count).ToList();
            else
                SortedList = GrayValueMap.OrderByDescending(o => o.Count).ToList();
            GrayValueMap = SortedList;
        }

        public static void SortByGrayValue(bool invert)
        {
            if ((GrayValueMap == null) || (GrayValueMap.Count == 0))
            {
                Logger.Error("SortByGrayValue toolTableArray is empty - do Init");
            }
            List<GrayProp> SortedList;
            if (!invert)
                SortedList = GrayValueMap.OrderBy(o => o.GrayVal).ToList();
            else
                SortedList = GrayValueMap.OrderByDescending(o => o.GrayVal).ToList();
            GrayValueMap = SortedList;
        }


        internal class GrayProp
        {   // tool properties general
            public byte GrayVal { get; set; }
            public long Count { get; set; }
            public bool Use { get; set; }

            public GrayProp()
            { ResetToolProperties(); }

            public GrayProp(byte gray, long cnt)
            {
                GrayVal = gray; Count = cnt; Use = true;
            }

            public void ResetToolProperties()
            {
                GrayVal = 0; Use = true; Count = 0;
            }
        }



    }
}
