/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

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

/*  Thanks to:
 *  3dpBurner Image2Gcode. A Image to GCODE converter for GRBL based devices.
    This file is part of 3dpBurner Image2Gcode application. 
    Copyright (C) 2015  Adrian V. J. (villamany) contact: villamany@gmail.com
*/
// 2018-11  split code into ...Create and ...Outline


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;

namespace GRBL_Plotter
{
    public partial class GCodeFromImage : Form
    {
        Bitmap originalImage;
        Bitmap adjustedImage;
        Bitmap resultImage;
        private static int maxToolTableIndex = 0;       // last index
        private static bool gcodeSpindleToggle = false; // Switch on/off spindle for Pen down/up (M3/M5)
        private static bool loadFromFile = false;
        private static Color backgroundColor = Color.White;

        private string imagegcode = "";
        public string imageGCode
        { get { return imagegcode; } }

        //On form load
        private void GCodeFromImage_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Done";
            getSettings();
            autoZoomToolStripMenuItem_Click(this, null);//Set preview zoom mode
        }
        private void GCodeFromImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationImageForm = Location;
        }
        private void GCodeFromImage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                loadClipboard();
                e.SuppressKeyPress = true;
            }
        }
        private void GCodeFromImage_Resize(object sender, EventArgs e)
        {
            panel1.Width = Width - 440;
            pictureBox1.Width = Width - 440;
            panel1.Height = Height - 60;
            pictureBox1.Height = Height - 60;
            pictureBox1.Refresh();
        }

        public GCodeFromImage(bool loadFile=false)
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            loadFromFile = loadFile;
            if (!rBImportSVGTool.Checked)
                rBImportSVGTool2.Checked = true;
        }


        #region load picture
        // load picture when form opens
        private void ImageToGCode_Load(object sender, EventArgs e)
        {
            if (loadFromFile) loadExtern(lastFile);
            originalImage = new Bitmap(Properties.Resources.modell);
            Location = Properties.Settings.Default.locationImageForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }
            processLoading();   // reset color corrections
        }

        private static string lastFile = "";
        //OpenFile, save picture grayscaled to originalImage and save the original aspect ratio to ratio
        private void btnLoad_Click(object sender, EventArgs e)
        {   try
            {   OpenFileDialog sfd = new OpenFileDialog();
                sfd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(sfd.FileName)) return;
                    lastFile = sfd.FileName;
                    originalImage = new Bitmap(System.Drawing.Image.FromFile(sfd.FileName));
                    processLoading();   // reset color corrections
                }
            }
            catch (Exception err)
            {   MessageBox.Show("Error opening file: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void loadExtern(string file)
        {   if (!File.Exists(file)) return;
            lastFile = file;
            originalImage = new Bitmap(System.Drawing.Image.FromFile(file));
            processLoading();   // reset color corrections
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        { Clipboard.SetImage(pictureBox1.Image); }

        private void setAsOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {   originalImage = new Bitmap(pictureBox1.Image);
            processLoading();
        }

        private void pasteFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {   loadClipboard(); }
        public void loadClipboard()
        {   IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                lastFile = "";
                originalImage = new Bitmap(Clipboard.GetImage());
                processLoading();   // reset color corrections
            }
        }

        public void loadURL(string url)
        {   pictureBox1.Load(url);
            originalImage = new Bitmap(pictureBox1.Image);
            processLoading();   // reset color corrections
        }
        private void GCodeFromImage_DragEnter(object sender, DragEventArgs e)
        {   e.Effect = DragDropEffects.All; }

        private void GCodeFromImage_DragDrop(object sender, DragEventArgs e)
        {   string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            { loadExtern(files[0]); }
        }

        decimal ratio; //Used to lock the aspect ratio when the option is selected
        private void processLoading()
        {   rBImportSVGTool.Checked = Properties.Settings.Default.importSVGToolSort;
            if (!rBImportSVGTool.Checked)
                rBImportSVGTool2.Checked = true;
            lblStatus.Text = "Opening file...";
            adjustedImage = new Bitmap(originalImage);
            resultImage = new Bitmap(originalImage);

            ratio = (decimal)originalImage.Width / (decimal)originalImage.Height;         //Save ratio for future use if needled
            nUDHeight.ValueChanged -= nUDWidthHeight_ValueChanged;
            oldWidth = Properties.Settings.Default.importImageWidth;    //nUDWidth.Value;
            oldHeight = oldWidth / ratio;               //Initialize y size
            nUDHeight.Value = oldHeight;
            nUDHeight.ValueChanged += nUDWidthHeight_ValueChanged;

            getSettings();
            setToolList();
            resetColorCorrection(); applyColorCorrections(); lblImageSource.Text = "original";
            tabControl1.SelectedIndex = 0;
            showInfo();
        }

        private void showInfo()
        {   int xSize = (int)(nUDWidth.Value / nUDReso.Value);  //Total X pixels of resulting image for GCode generation
            int ySize = (int)(nUDHeight.Value / nUDReso.Value); //Convert.ToInt32(float.Parse(tbHeight.Text, CultureInfo.InvariantCulture.NumberFormat) / float.Parse(tbRes.Text, CultureInfo.InvariantCulture.NumberFormat));
            pixelCount = xSize * ySize;
            lblSizeOrig.Text = "Original size: " + originalImage.Width + " x " + originalImage.Height + " = " + (originalImage.Width* originalImage.Height) + " px";
            lblSizeResult.Text = "Result size: " + xSize.ToString() + " x " + ySize.ToString() + " = " + pixelCount.ToString() + " px";
            string tmp = "Press 'Preview' to update tool list\r\n\r\nNumber of pens: " + (maxToolTableIndex-1)+"\r\n\r\n";
            tmp += "Original Image size (px):\r\n" + originalImage.Width + " px * " + originalImage.Height + " px = " + (originalImage.Width* originalImage.Height) + " px\r\n\r\n";
            tmp += "Result Image size (px)  :\r\n" + xSize.ToString() + " px * " + ySize.ToString() + " px = " + pixelCount.ToString() + " px\r\n\r\n";
            tmp += "Result image size(units): \r\nWidth: " + Math.Round(nUDWidth.Value, 1);
            tmp += "  Height: " + Math.Round(nUDHeight.Value, 1) + "\r\n\r\n";
            tBToolList.Text = tmp;
        }

        #endregion


        private void getSettings()
        {   maxToolTableIndex = toolTable.init() ;       // 1 entry reserved
            nUDMaxColors.Maximum = maxToolTableIndex-1;
            nUDMaxColors.Value = maxToolTableIndex-1;
            setToolList();
        }
        private void setToolList(bool all=true)
        {   cLBTools.Items.Clear();
            for (int i = 0; i < maxToolTableIndex; i++)
            {   toolTable.setIndex(i);
                if ((toolTable.indexToolNr() >= 0) && (all || toolTable.indexUse()))
                {   cLBTools.Items.Add(toolTable.indexToolNr() + ") " + toolTable.getName(), toolTable.indexSelected());
                }
            }
        }
        /// <summary>
        /// update result after deselecting tools
        /// </summary>
        private void cLBTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolTable.sortByToolNr();
            int toolNr = 0;
            for (int i = 0; i < cLBTools.Items.Count; i++)      // index = unknown
            {
                Int32.TryParse((cLBTools.Items[i].ToString().Split(')'))[0], out toolNr);    // get toolNr from text
                toolTable.setIndex(toolTable.getIndexByToolNr(toolNr));                     // get index from toolNr
                toolTable.setSelected(cLBTools.GetItemChecked(i));                          // set selected property of index
            }
            generateResultImage(ref resultToolNrArray);      // fill countColors
            showResultActive = true;
            pictureBox1.Image = resultImage;
            lblImageSource.Text = "result";
            Refresh();
        }


        #region preset color correction
        private void btnResetCorrection_Click(object sender, EventArgs e)
        { resetColorCorrection();  applyColorCorrections(); lblImageSource.Text = "original"; }

        private void btnPresetCorrection1_Click(object sender, EventArgs e)
        { resetColorCorrection(); presetColorCorrection(1); applyColorCorrections(); }
        private void btnPresetCorrection2_Click(object sender, EventArgs e)
        { resetColorCorrection(); presetColorCorrection(2); applyColorCorrections(); }
        private void btnPresetCorrection3_Click(object sender, EventArgs e)
        { resetColorCorrection(); presetColorCorrection(3); applyColorCorrections(); }
        private void btnPresetCorrection4_Click(object sender, EventArgs e)
        {   resetColorCorrection(); presetColorCorrection(4); applyColorCorrections(); adjustedImage = imgInvert(adjustedImage);
            originalImage = new Bitmap(adjustedImage); resetColorCorrection();
            disableControlEvents(); cbGrayscale.Checked = true; tBarGamma.Value = 10;
            cbExceptColor.Checked = true; toolTable.setExceptionColor(cbExceptColor.BackColor);
            cBPreview.Checked = true; cBReduceColorsToolTable.Checked = true; nUDMaxColors.Value = 2;
            enableControlEvents(); applyColorCorrections(); 
        }

        private void resetColorCorrection()
        {   disableControlEvents();
            tBarBrightness.Value = 0;
            tBarContrast.Value = 0;
            tBarGamma.Value = 100;
            tBarSaturation.Value = 0;
            cbGrayscale.Checked = false;
            tBRMin.Value = 0; tBRMax.Value = 255;
            tBGMin.Value = 0; tBGMax.Value = 255;
            tBBMin.Value = 0; tBBMax.Value = 255;
            tBarHueShift.Value = 0;
            cBFilterRemoveArtefact.Checked = false;
            cBFilterEdge.Checked = false;
            cBPosterize.Checked = false;
            cBFilterEdge.Checked = false;
            cBFilterHistogram.Checked = false;
            getSettings();
            cBReduceColorsToolTable.Checked = false;
            cBReduceColorsDithering.Checked = false;
            rBMode0.Checked = true;
            cbExceptColor.Checked = false;
            cbExceptColor.BackColor = Color.White;
            toolTable.clrExceptionColor();
            cBPreview.Checked = false;
            redoColorAdjust = false;
 //           updatePixelCountPerColorNeeded = false;
            lblImageSource.Text = "original";
            enableControlEvents();
        }
        private void disableControlEvents()
        {
            tBarBrightness.Scroll -= applyColorCorrectionsEvent;
            tBarContrast.Scroll -= applyColorCorrectionsEvent;
            tBarGamma.Scroll -= applyColorCorrectionsEvent;
            tBarSaturation.Scroll -= applyColorCorrectionsEvent;
            cbGrayscale.CheckedChanged -= applyColorCorrectionsEvent;
            tBRMin.Scroll -= applyColorCorrectionsEvent;
            tBGMin.Scroll -= applyColorCorrectionsEvent;
            tBBMin.Scroll -= applyColorCorrectionsEvent;
            tBarHueShift.Scroll -= applyColorCorrectionsEvent;
            cBPosterize.CheckedChanged -= applyColorCorrectionsEvent;
            cBFilterRemoveArtefact.CheckedChanged -= applyColorCorrectionsEvent;
            cBFilterEdge.CheckedChanged -= applyColorCorrectionsEvent;
            cBFilterHistogram.CheckedChanged -= applyColorCorrectionsEvent;
            cBReduceColorsToolTable.CheckedChanged -= applyColorCorrectionsEvent;
            cBReduceColorsDithering.CheckedChanged -= applyColorCorrectionsEvent;
            nUDMaxColors.ValueChanged -= applyColorCorrectionsEvent;
            cBPreview.CheckedChanged -= justShowResult;
            cbExceptColor.CheckedChanged -= cbExceptColor_CheckedChanged;
            rBMode0.CheckedChanged -= rBMode0_CheckedChanged;
            rBMode1.CheckedChanged -= rBMode0_CheckedChanged;
            rBMode2.CheckedChanged -= rBMode0_CheckedChanged;
        }
        private void enableControlEvents()
        {
            tBarBrightness.Scroll += applyColorCorrectionsEvent;
            tBarContrast.Scroll += applyColorCorrectionsEvent;
            tBarGamma.Scroll += applyColorCorrectionsEvent;
            tBarSaturation.Scroll += applyColorCorrectionsEvent;
            cbGrayscale.CheckedChanged += applyColorCorrectionsEvent;
            tBRMin.Scroll += applyColorCorrectionsEvent;
            tBGMin.Scroll += applyColorCorrectionsEvent;
            tBBMin.Scroll += applyColorCorrectionsEvent;
            tBarHueShift.Scroll += applyColorCorrectionsEvent;
            cBPosterize.CheckedChanged += applyColorCorrectionsEvent;
            cBFilterRemoveArtefact.CheckedChanged += applyColorCorrectionsEvent;
            cBFilterEdge.CheckedChanged += applyColorCorrectionsEvent;
            cBFilterHistogram.CheckedChanged += applyColorCorrectionsEvent;
            cBReduceColorsToolTable.CheckedChanged += applyColorCorrectionsEvent;
            cBReduceColorsDithering.CheckedChanged += applyColorCorrectionsEvent;
            nUDMaxColors.ValueChanged += applyColorCorrectionsEvent;
            cBPreview.CheckedChanged += justShowResult;
            cbExceptColor.CheckedChanged += cbExceptColor_CheckedChanged;
            rBMode0.CheckedChanged += rBMode0_CheckedChanged;
            rBMode1.CheckedChanged += rBMode0_CheckedChanged;
            rBMode2.CheckedChanged += rBMode0_CheckedChanged;
        }

        private static bool redoColorAdjust = false;
        private void presetColorCorrection(int tmp)
        {
            disableControlEvents();
            if (tmp == 1)                   // Image dark background
            {   tBarSaturation.Value = 128;
                tBRMin.Value = 64;
                tBGMin.Value = 64;
                tBBMin.Value = 64;
                cbExceptColor.Checked = true;
                cbExceptColor.BackColor = Color.FromArgb(255, 0, 0, 0);
                toolTable.setExceptionColor(cbExceptColor.BackColor);
                cBGCodeOutline.Checked = false;
            }
            else if (tmp == 2)              // graphic many colors
            {   cBPosterize.Checked = true;
                rBMode2.Checked = true;
            }
            else if (tmp == 3)              // comic few colors
            {   tBarBrightness.Value = -15;
                tBarContrast.Value = 20;
                tBarGamma.Value = 88;
                cBReduceColorsToolTable.Checked = true;
                cbExceptColor.Checked = true;   // except white
                cBFilterRemoveArtefact.Checked = true;
                rBMode0.Checked = true;
                redoColorAdjust = true;
            }
            else if (tmp == 4)              // comic few colors
            {   cbGrayscale.Checked = true;
                rbModeGray.Checked = true;
                cBFilterEdge.Checked = true;
            }
            cBPreview.Checked = true;
            enableControlEvents();
        }
        private string listColorCorrection()
        {   string tmp = "";
            tmp += "Reduce colors \t" + (cBReduceColorsToolTable.Checked ? "on" : "off") + " " + Convert.ToString(nUDMaxColors.Value) + "\r\n";
            tmp += "Dithering  \t" + (cBReduceColorsDithering.Checked ? "on" : "off") +"\r\n";
            tmp += "Except. col.\t" + (cbExceptColor.Checked ? "on" : "off") + " " + Convert.ToString(cbExceptColor.BackColor) + "\r\n";
            tmp += "Brightness \t"+ Convert.ToString(tBarBrightness.Value)+"\r\n";
            tmp += "Contrast   \t" + Convert.ToString(tBarContrast.Value) + "\r\n";
            tmp += "Gamma      \t" + Convert.ToString(tBarGamma.Value / 100.0f) + "\r\n";
            tmp += "Saturation \t" + Convert.ToString(tBarSaturation.Value) + "\r\n";
            tmp += "Grayscale  \t" + (cbGrayscale.Checked ? "on" : "off") + "\r\n";
            tmp += "Channel filter red   \t" + Convert.ToString(tBRMin.Value) + ";" + Convert.ToString(tBRMax.Value) + "\r\n";
            tmp += "Channel filter green \t" + Convert.ToString(tBGMin.Value) + ";" + Convert.ToString(tBGMax.Value) + "\r\n";
            tmp += "Channel filter blue  \t" + Convert.ToString(tBBMin.Value) + ";" + Convert.ToString(tBBMax.Value) + "\r\n";
            tmp += "Hue shift  \t" + Convert.ToString(tBarHueShift.Value) + "\r\n";
            tmp += "Edge filter   \t" + (cBFilterEdge.Checked? "on":"off") + "\r\n";
            tmp += "Histogram equ.\t" + (cBFilterHistogram.Checked ? "on" : "off") + "\r\n";
            tmp += "Posterize     \t" + (cBPosterize.Checked ? "on" : "off") + "\r\n";
            return tmp;
        }

        private void updateLabels()
        {   lblBrightness.Text = Convert.ToString(tBarBrightness.Value);
            lblContrast.Text = Convert.ToString(tBarContrast.Value);
            lblGamma.Text = Convert.ToString(tBarGamma.Value / 100.0f);
            lblSaturation.Text = Convert.ToString(tBarSaturation.Value);
            lblHueShift.Text = Convert.ToString(tBarHueShift.Value);
            lblCFR.Text = Convert.ToString(tBRMin.Value) + ";" + Convert.ToString(tBRMax.Value);
            lblCFG.Text = Convert.ToString(tBGMin.Value) + ";" + Convert.ToString(tBGMax.Value);
            lblCFB.Text = Convert.ToString(tBBMin.Value) + ";" + Convert.ToString(tBBMax.Value);
            Refresh();
        }
        private void btnShowSettings_Click(object sender, EventArgs e)
        {   MessageBox.Show(listColorCorrection(),"Color correction settings");
            System.Windows.Forms.Clipboard.SetText(listColorCorrection());
        }
        #endregion


        #region image manipulation

        private static decimal oldWidth=0, oldHeight=0;
        private void nUDWidthHeight_ValueChanged(object sender, EventArgs e)
        {   nUDWidth.ValueChanged -= nUDWidthHeight_ValueChanged;
            nUDHeight.ValueChanged -= nUDWidthHeight_ValueChanged;
            bool edit = false;
            if (oldWidth != nUDWidth.Value)
            {   oldWidth = nUDWidth.Value;
                if (cbLockRatio.Checked)
                {   oldHeight = oldWidth / ratio;
                    nUDHeight.Value = oldHeight;
                    nUDHeight.Invalidate();
                }
                nUDWidth.Value = oldWidth;
                edit = true;
            }
            if (oldHeight != nUDHeight.Value)
            {   oldHeight = nUDHeight.Value;
                if (cbLockRatio.Checked)
                {   oldWidth = oldHeight * ratio;
                    nUDWidth.Value = oldWidth;
                    nUDWidth.Invalidate();
                }
                nUDHeight.Value = oldHeight;
                edit = true;
            }
            if (edit)
                applyColorCorrections();
            nUDWidth.ValueChanged += nUDWidthHeight_ValueChanged;
            nUDHeight.ValueChanged += nUDWidthHeight_ValueChanged;
            Refresh();
        }

        private void btnKeepSizeWidth_Click(object sender, EventArgs e)
        { nUDWidth.Value = originalImage.Width * nUDReso.Value; }

        private void btnKeepSizeReso_Click(object sender, EventArgs e)
        { nUDReso.Value = nUDWidth.Value / originalImage.Width; }


        //Horizontal mirroing
        private void btnHorizMirror_Click(object sender, EventArgs e)
        {   if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            pictureBox1.Image = adjustedImage;
        }
        //Vertical mirroing
        private void btnVertMirror_Click(object sender, EventArgs e)
        {   if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            pictureBox1.Image = adjustedImage;
            showResultImage();
        }
        //Rotate right
        private void btnRotateRight_Click(object sender, EventArgs e)
        {   if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            originalImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            ratio = 1 / ratio;
            decimal s = nUDHeight.Value;
            nUDHeight.Value = nUDWidth.Value;
            nUDWidth.Value = s;
            pictureBox1.Image = adjustedImage;
            autoZoomToolStripMenuItem_Click(this, null);
            showResultImage();
        }
        //Rotate left
        private void btnRotateLeft_Click(object sender, EventArgs e)
        {   if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            ratio = 1 / ratio;
            decimal s = nUDHeight.Value;
            nUDHeight.Value = nUDWidth.Value;
            nUDWidth.Value = s;
            pictureBox1.Image = adjustedImage;
            autoZoomToolStripMenuItem_Click(this, null);
            showResultImage();
        }
        //Invert image color
        private void btnInvert_Click(object sender, EventArgs e)
        {   if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage = imgInvert(adjustedImage);
            originalImage = imgInvert(originalImage);
            pictureBox1.Image = adjustedImage;
            showResultImage();
        }

        //Contrast adjusted by user
        private void applyColorCorrectionsEvent(object sender, EventArgs e)
        {   applyColorCorrections();
        }

        private static int conversionMode = 0, conversionModeOld = 0;
        private void rBMode0_CheckedChanged(object sender, EventArgs e)
        {   conversionMode = 0;
            if (rBMode1.Checked) conversionMode = 1;
            else if (rBMode2.Checked) conversionMode = 2;
            if (conversionMode != conversionModeOld)
                applyColorCorrections();
            conversionModeOld = conversionMode;
        }

        //Interpolate a 8 bit grayscale value (0-255) between min,max
        private int interpolate(int grayValue, int min, int max)
        {   int dif = max - min;
            return (min + ((grayValue * dif) / 255));
        }

        //Apply dithering to an image (Convert to 1 bit)
        private Bitmap imgDirther(Bitmap input)
        {
            lblStatus.Text = "Dithering...";
            Refresh();
            var masks = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format1bppIndexed);
            var data = new sbyte[input.Width, input.Height];
            var inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {   var scanLine = inputData.Scan0;
                var line = new byte[inputData.Stride];
                for (var y = 0; y < inputData.Height; y++, scanLine += inputData.Stride)
                {   Marshal.Copy(scanLine, line, 0, line.Length);
                    for (var x = 0; x < input.Width; x++)
                    {
                        data[x, y] = (sbyte)(64 * (GetGreyLevel(line[x * 3 + 2], line[x * 3 + 1], line[x * 3 + 0]) - 0.5));
                    }
                }
            }
            finally
            {   input.UnlockBits(inputData);  }

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
        {   return (r * 0.299 + g * 0.587 + b * 0.114) / 255; }
        //Adjust brightness contrast and gamma of an image      
        private Bitmap imgBalance(Bitmap img, int brigh, int cont, int gam)
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
            g.DrawImage(output, new Rectangle(0, 0, output.Width, output.Height)
            , 0, 0, output.Width, output.Height,
            GraphicsUnit.Pixel, imageAttributes);
            return (output);
        }

        //Return a grayscale version of an image
        private Bitmap imgGrayscale(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);//create a blank bitmap the same size as original
            Graphics g = Graphics.FromImage(newBitmap);//get a graphics object from the new image
            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                {
                    new float[] {.299f, .299f, .299f, 0, 0},
                    new float[] {.587f, .587f, .587f, 0, 0},
                    new float[] {.114f, .114f, .114f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });
            ImageAttributes attributes = new ImageAttributes();//create some image attributes
            attributes.SetColorMatrix(colorMatrix);//set the color matrix attribute

            //draw the original image on the new image using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();//dispose the Graphics object
            return (newBitmap);
        }

        //Return a inverted colors version of a image
        private Bitmap imgInvert(Bitmap original)
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
            return (newBitmap);
        }

        //Invoked when the user input any value for image adjust
        private static int pixelCount = 100;
        private static bool useFullReso = false;
        private static decimal resoFull = 1;
        private static decimal resoDesired = 1;
        private static int resoFactor = 1;
        private void applyColorCorrections()
        {
            Cursor.Current = Cursors.WaitCursor;
            useFullReso = cBGCodeOutline.Checked;
            updateLabels();
            lblStatus.Text = "Apply color corrections...";

            resoDesired = nUDReso.Value;
            resoFactor = 1;
            if (useFullReso)                                        // if full resolution is needed
            {   resoFull = (nUDWidth.Value / originalImage.Width);      // get max possible resolution
                resoFactor = (int)Math.Ceiling(resoDesired / resoFull); // get rounded factor to set resolution
                if (resoFactor > 5)             
                    resoFactor = 5;
                resoDesired = resoDesired / resoFactor;              // set rounded value
            }
            int xSize = (int)(nUDWidth.Value / resoDesired);  //Total X pixels of resulting image for GCode generation
            int ySize = (int)(nUDHeight.Value / resoDesired); //Convert.ToInt32(float.Parse(tbHeight.Text, CultureInfo.InvariantCulture.NumberFormat) / float.Parse(tbRes.Text, CultureInfo.InvariantCulture.NumberFormat));

            showInfo();
            Refresh();
            try
            {
                if (adjustedImage == null) return;//if no image, do nothing

                ResizeNearestNeighbor filterResize = new ResizeNearestNeighbor(xSize, ySize);
                adjustedImage = filterResize.Apply(originalImage);
                adjustedImage = imgBalance(adjustedImage, tBarBrightness.Value, tBarContrast.Value, tBarGamma.Value);
                
                SaturationCorrection filterS = new SaturationCorrection((float)tBarSaturation.Value/255);
                filterS.ApplyInPlace(adjustedImage);       

                if (cbGrayscale.Checked)// cbDirthering.Text == "Dirthering FS 1 bit")
                {   if (rbModeDither.Checked)
                        adjustedImage = imgDirther(adjustedImage);
                    else
                        adjustedImage = imgGrayscale(adjustedImage);
                    adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB
                }
                updateLabelColorCount = true;

                // create filter
                ChannelFiltering filterC = new ChannelFiltering();
                // set channels' ranges to keep
                filterC.Red = new AForge.IntRange((int)tBRMin.Value, (int)tBRMax.Value);
                filterC.Green = new AForge.IntRange((int)tBGMin.Value, (int)tBGMax.Value);
                filterC.Blue = new AForge.IntRange((int)tBBMin.Value, (int)tBBMax.Value);
                filterC.ApplyInPlace(adjustedImage);

                HueShift filter1 = new HueShift((int)tBarHueShift.Value);   // self made filter, not part of AForge
                filter1.ApplyInPlace(adjustedImage);

                if (cBFilterEdge.Checked)
                {   Edges filterEdge = new Edges();
                    filterEdge.ApplyInPlace(adjustedImage);
                }
                if (cBFilterHistogram.Checked)
                {   HistogramEqualization filterHisto = new HistogramEqualization();
                    filterHisto.ApplyInPlace(adjustedImage);
                }
                if (cBPosterize.Checked)
                {   SimplePosterization filter2 = new SimplePosterization();
                    filter2.ApplyInPlace(adjustedImage);
                }

                toolTable.setAllSelected(true);     //  enable all tools
                if (cBReduceColorsToolTable.Checked)
                {   List<Color> myPalette = new List<Color>();
                    ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
                    countResultColors();        // get all possible tool-colors and its count

                    if (redoColorAdjust)        // 
                    {
                        redoColorAdjust = false;
                        toolTable.sortByPixelCount();
                        int matchLimit = 0;
                        toolTable.sortByToolNr();
                        int tmpCount = pixelCount;                // keep original counter 
                        if (useFullReso)
                            tmpCount = originalImage.Width * originalImage.Height;

                        if (cbExceptColor.Checked)
                            tmpCount -= toolTable.pixelCount(0);  // no color-except
                        for (int i = 0; i < maxToolTableIndex; i++)
                        {   if ((toolTable.pixelCount(i) * 100 / tmpCount) >= nUDColorPercent.Value)
                            { matchLimit++; }// tmp += toolTable.getName() + "  " + (toolTable.pixelCount(i) * 100 / tmpCount) + "\r\n"; }
                            else
                                toolTable.setPresent(false);
                        }
                        if (matchLimit < nUDMaxColors.Minimum) { matchLimit = (int)nUDMaxColors.Maximum; }
                        if (matchLimit > nUDMaxColors.Maximum) { matchLimit = (int)nUDMaxColors.Maximum; }
                        nUDMaxColors.ValueChanged -= applyColorCorrectionsEvent;    // set new value without generating an event
                        nUDMaxColors.Value = matchLimit;
                        nUDMaxColors.ValueChanged += applyColorCorrectionsEvent;
                    }

                    if (cbExceptColor.Checked)
                        myPalette.Add(cbExceptColor.BackColor);
                    toolTable.sortByPixelCount();                       // fill palette with colors in order of occurence
                    toolTable.setAllSelected(false);                                                    //        toolTable.clear();
                    for (int i = 0; i < (int)nUDMaxColors.Value; i++)   // add colors to AForge filter
                    {   toolTable.setIndex(i);
                        if (toolTable.indexToolNr() >= 0)
                        {   myPalette.Add(toolTable.indexColor());
                            toolTable.setSelected(true);
                        }
                    }
                    setToolList(false);      // show only applied colors

                    if (cBReduceColorsDithering.Checked)
                    {   OrderedColorDithering dithering = new OrderedColorDithering();
                        dithering.ColorTable = myPalette.ToArray();
                        adjustedImage = dithering.Apply(adjustedImage);
                    }
                    else
                        adjustedImage = ciq.ReduceColors(adjustedImage, myPalette.ToArray());// (int)nUDMaxColors.Value);                                                                                                 // adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB
                }
                else
                    setToolList();      

                if (cBFilterRemoveArtefact.Checked)
                {
                    adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, PixelFormat.Format24bppRgb);
                    RemoveArtefact filterRA = new RemoveArtefact(5,3);
                    filterRA.ApplyInPlace(adjustedImage);
                }
                adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB
                if (!cBPreview.Checked)
                    pictureBox1.Image = adjustedImage;

                resultImage = new Bitmap(adjustedImage);
                lblStatus.Text = "Done";
                lblImageSource.Text = "modified";
                Refresh();
            }
            catch (Exception e)
            {   MessageBox.Show("Error resizing/balancing image: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            showResultImage();  // if checked, show final result with tool colors
            Cursor.Current = Cursors.Default;
        }

        private static bool showResultActive = false;
        private static bool showResultPreview = false;
        private void showResultImage(bool showResult=true)//, bool preview = false)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            if (cBPreview.Checked || showResultPreview)
            {
                showResultPreview = false;
  /*              maxToolTableIndex = toolTable.clear();
                if (nUDMaxColors.Maximum != maxToolTableIndex - 1)
                {
                    nUDMaxColors.Maximum = maxToolTableIndex - 1;
                    nUDMaxColors.Value = maxToolTableIndex - 1;
                }*/
                generateResultImage(ref resultToolNrArray);      // fill countColors
                showResultActive = true;
                if (showResult)
                {   updateToolList();
                    pictureBox1.Image = resultImage;
                    lblImageSource.Text = "result";
                }
                Refresh();
            }
            else
            {   pictureBox1.Image = adjustedImage;
                showResultActive = false;
                lblImageSource.Text = "modified";
            }
        }

        /// <summary>
        /// Simulate creating result image, count usage of tool-colors
        /// </summary>
        private void countResultColors()    // update pixelCounts for specific tool-colors
        {
            Color myColor;
            if (cbExceptColor.Checked)
                toolTable.setExceptionColor(cbExceptColor.BackColor);
            else
                toolTable.clrExceptionColor();
            int mode = conversionMode;
            toolTable.clear();                  // setUse = false
            BitmapData dataAdjusted = null;
            sbyte myToolNr = 0;//, myIndex;
            Dictionary<Color, sbyte> lookUpToolNr = new Dictionary<Color, sbyte>();
            try
            {
                Rectangle rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
                dataAdjusted = adjustedImage.LockBits(rect, ImageLockMode.ReadOnly, adjustedImage.PixelFormat);

                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                int psize = 4;  // 32bppARGB GetPixelInfoSize(adjustedImage.PixelFormat);
                int bsize = dataAdjusted.Stride * adjustedImage.Height;
                byte[] pixelsAdjusted = new byte[bsize];
                Marshal.Copy(ptrAdjusted, pixelsAdjusted, 0, pixelsAdjusted.Length);
                byte r, g, b, a;
                for (int index = 0; index < pixelsAdjusted.Length; index += psize)
                {
                    b = pixelsAdjusted[index]; g = pixelsAdjusted[index + 1]; r = pixelsAdjusted[index + 2]; a = pixelsAdjusted[index + 3];
                    myColor = Color.FromArgb(a, r, g, b);
                    if (myColor.A == 0)                             // skip exception, removed: cbExceptAlpha.Checked
                    { myToolNr = -1; toolTable.sortByToolNr(); toolTable.setIndex(0); }
                    else
                    {
                        if (lookUpToolNr.TryGetValue(myColor, out myToolNr))
                        {   toolTable.setIndex(toolTable.getIndexByToolNr(myToolNr));
                        }
                        else
                        {   myToolNr = (sbyte)toolTable.getToolNr(myColor, mode);     // find nearest color in palette, sort by match, set index to 0
                            lookUpToolNr.Add(myColor, myToolNr);
                        }
                    }
                    toolTable.countPixel(); // count pixel / color
                    toolTable.setPresent(true);
                }
            }
            finally
            {   if (adjustedImage != null) adjustedImage.UnlockBits(dataAdjusted);
            }
            setToolList();
        }

        // Replace orginal color by nearest color from tool table
        // fill-up usedColor array
        private static sbyte[,] resultToolNrArray;
        /// <summary>
        /// Generate result image and fill resultToolNrArray
        /// </summary>
        private void generateResultImage(ref sbyte[,] tmpToolNrArray)      // and count tool colors
        {//https://www.codeproject.com/Articles/17162/Fast-Color-Depth-Change-for-Bitmaps
            Color myColor, newColor;
            if (cbExceptColor.Checked)
                toolTable.setExceptionColor(cbExceptColor.BackColor);
            else
                toolTable.clrExceptionColor();
            int mode = conversionMode;
            BitmapData dataAdjusted = null;
            BitmapData dataResult = null;
            lblStatus.Text = "Generate result image...";
            sbyte myToolNr = 0;//, myIndex;
            Dictionary<Color, sbyte> lookUpToolNr = new Dictionary<Color, sbyte>();
            try
            {
                Rectangle rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
                dataAdjusted = adjustedImage.LockBits(rect, ImageLockMode.ReadOnly, adjustedImage.PixelFormat);

                dataResult = resultImage.LockBits(rect, ImageLockMode.WriteOnly, adjustedImage.PixelFormat);
                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                IntPtr ptrResult = dataResult.Scan0;
                int psize = 4;  // 32bppARGB GetPixelInfoSize(adjustedImage.PixelFormat);
                int bsize = dataAdjusted.Stride * adjustedImage.Height;
                byte[] pixelsAdjusted = new byte[bsize];
                byte[] pixelsResult = new byte[bsize];
                tmpToolNrArray = new sbyte[adjustedImage.Width, adjustedImage.Height];
                Marshal.Copy(ptrAdjusted, pixelsAdjusted, 0, pixelsAdjusted.Length);

                byte r, g, b, a;
                int bx = 0, by = 0;
                for (int index = 0; index < pixelsAdjusted.Length; index += psize)
                {
                    b = pixelsAdjusted[index];      // https://stackoverflow.com/questions/8104461/pixelformat-format32bppargb-seems-to-have-wrong-byte-order
                    g = pixelsAdjusted[index + 1];
                    r = pixelsAdjusted[index + 2];
                    a = pixelsAdjusted[index + 3];
                    // get current color
                    myColor = Color.FromArgb(a, r, g, b);
                    if (lookUpToolNr.TryGetValue(myColor, out myToolNr))
                    {   toolTable.setIndex(toolTable.getIndexByToolNr(myToolNr));
                    }
                    else
                    {   myToolNr = (sbyte)toolTable.getToolNr(myColor, mode);     // find nearest color in palette, sort by match, set index to 0
                        lookUpToolNr.Add(myColor, myToolNr);
                    }

                    if (myColor.A == 0)                 // skip exception, removed: cbExceptAlpha.Checked
                    {   newColor = backgroundColor; myToolNr = -1; toolTable.sortByToolNr(); toolTable.setIndex(0); }// usedColorName[0] = "Alpha = 0      " + myColor.ToString(); }
                    else
                    {   
                        if ((myToolNr < 0) || (!toolTable.indexSelected()))  // -1 = alpha, -1 = exception color
                        {   newColor = backgroundColor; myToolNr = -1; }
                        else
                            newColor = toolTable.getColor();   // Color.FromArgb(255, r, g, b);
                    }
                    toolTable.countPixel(); // count pixel / color
                    toolTable.setPresent(true);
                    tmpToolNrArray[bx++, by] = myToolNr;

                    if (bx >= adjustedImage.Width)
                    { bx = 0; by++; }
                    // apply new color
                    pixelsResult[index] = newColor.B;// newColor.A;
                    pixelsResult[index + 1] = newColor.G;
                    pixelsResult[index + 2] = newColor.R;
                    pixelsResult[index + 3] = 255;
                }
                Marshal.Copy(pixelsResult, 0, ptrResult, pixelsResult.Length);
            }
            finally
            {
                if (resultImage != null) resultImage.UnlockBits(dataResult);
                if (adjustedImage != null) adjustedImage.UnlockBits(dataAdjusted);
            }
            lblStatus.Text = "Done";
        }

        #endregion

        private bool updateLabelColorCount = false;
        private void timer1_Tick(object sender, EventArgs e)
        {   if (updateLabelColorCount)
                countImageColors();
        }
        /// <summary>
        /// Count amount of different colors in adjusted image
        /// </summary>
        private void countImageColors()
        {   // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
            BitmapData bmpData = adjustedImage.LockBits(rect, ImageLockMode.ReadWrite, adjustedImage.PixelFormat);       
            IntPtr ptr = bmpData.Scan0;                         // Get the address of the first line.
            int bytes = bmpData.Stride * adjustedImage.Height;  // Declare an array to hold the bytes of the bitmap.
            byte[] rgbValues = new byte[bytes];
            byte r, g, b, a;         
            Marshal.Copy(ptr, rgbValues, 0, bytes);             // Copy the RGB values into the array.
            int count = 0;
            int stride = bmpData.Stride;
            var cnt = new HashSet<System.Drawing.Color>();      // count each color once
            for (int column = 0; column < bmpData.Height; column++)
            {   for (int row = 0; row < bmpData.Width; row++)
                {
                    b = (byte)(rgbValues[(column * stride) + (row * 4) + 0]);  // https://stackoverflow.com/questions/8104461/pixelformat-format32bppargb-seems-to-have-wrong-byte-order
                    g = (byte)(rgbValues[(column * stride) + (row * 4) + 1]);
                    r = (byte)(rgbValues[(column * stride) + (row * 4) + 2]);
                    a = (byte)(rgbValues[(column * stride) + (row * 4) + 3]);
                    cnt.Add(Color.FromArgb(a, r, g, b));
                    count++;
                }
            }
            adjustedImage.UnlockBits(bmpData);
            lblColors.Text = "Number of colors: " + cnt.Count.ToString();
            updateLabelColorCount = false;
        }

        //Quick preview of the original image. Todo: use a new image container for fas return to processed image
        private void btnCheckOrig_MouseDown(object sender, MouseEventArgs e)
        { showResultPreview = true; showResultImage();    }
        private void btnCheckOrig_MouseUp(object sender, MouseEventArgs e)
        {   if (adjustedImage == null) return;//if no image, do nothing
            pictureBox1.Image = adjustedImage;
            cBPreview.Checked = false;
            lblImageSource.Text = "modified";
        }

        private void btnShowOrig_MouseDown(object sender, MouseEventArgs e)
        { pictureBox1.Image = originalImage; lblImageSource.Text = "original"; }

        private void btnShowOrig_MouseUp(object sender, MouseEventArgs e)
        {   if (cBPreview.Checked)
            { pictureBox1.Image = resultImage; lblImageSource.Text = "result"; }
            else
            { pictureBox1.Image = adjustedImage; lblImageSource.Text = "modified"; }
        }

        private void updateToolList()
        {   string tmp1 = "", tmp2 ="";
            int skipToolNr = 1;

            toolTable.sortByToolNr();
            toolTable.setIndex(0);
            int tmpCount = pixelCount;
            if (useFullReso)
                tmpCount = originalImage.Width * originalImage.Height;

            if (cbExceptColor.Checked)
                tmpCount -= toolTable.pixelCount();     // no color-except

            if (!rBImportSVGTool.Checked)
                toolTable.sortByPixelCount();           // sort by color area (max. first)

            if (tmpCount == 0) { tmpCount = 100; }
            if (!rBImportSVGTool.Checked) { tmp2 = "Tools sorted by pixel count.\r\n"; }
            tmp2 += "Tool colors in use and pixel count:\r\n";
            String cIndex, cName, cCount, cPercent;
            int toolNr; 
            for (int i = 0; i < maxToolTableIndex; i++)
            {   toolTable.setIndex(i);
                toolNr = toolTable.indexToolNr();
                if (toolTable.indexUse() && (toolTable.pixelCount() > 0))       
                {   cIndex = toolNr.ToString().PadLeft(2, ' ') + ") ";
                    cName = toolTable.indexName().PadRight(15, ' ');
                    cCount = toolTable.pixelCount().ToString().PadLeft(8, ' ') + "  ";  
                    cPercent = string.Format("{0,4:0.0}",(toolTable.pixelCount() * 100 / tmpCount)) + "%";
                    if (toolNr < 0)
                        tmp1 += cIndex + cName + cCount + "\r\n";
                    else
                    {   if (cbSkipToolOrder.Checked)
                            tmp2 += skipToolNr++.ToString().PadLeft(2, ' ') + ") " + cName + cCount + cPercent + "\r\n";
                        else
                            tmp2 += cIndex + cName + cCount + cPercent + "\r\n";
                    }
                }
            }
            tBToolList.Text = tmp2 + "\r\n" + tmp1;
        }

        //Generate button click
        public void btnGenerate_Click(object sender, EventArgs e)
        {   Cursor.Current = Cursors.WaitCursor;
            tabControl1.SelectedTab = tabPage5;
            if (!showResultActive)
            {   cBPreview.Checked = true;
                showResultImage(true);      // generate and show result
            }

            if (rBProcessTool.Checked)
            {   if (rbEngravingPattern1.Checked)
                    generateGCodeHorizontal();
                else
                    generateGCodeDiagonal();
            }
            else
                generateHeightData();
            Cursor.Current = Cursors.Default;
            return;
        }

        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {   pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Width = panel1.Width;
            pictureBox1.Height = panel1.Height;
            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
        }

        private void justShowResult(object sender, EventArgs e)
        {   showResultImage(); }

        private Point oldPoint = new Point(0,0);
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {   if ((e.Location != oldPoint) || (e.Button == MouseButtons.Left))
            {   Color clr = backgroundColor;// GetColorAt(e.Location);
                if (pictureBox1.Image != null)
                {   Bitmap bmp = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
                    pictureBox1.DrawToBitmap(bmp, pictureBox1.ClientRectangle);
                    if ((0 <= e.X) && (e.X <= pictureBox1.ClientSize.Width) && (0 <= e.Y) && (e.Y <= pictureBox1.ClientSize.Height))
                        clr = bmp.GetPixel(e.X, e.Y);
                    bmp.Dispose();
                }
                if (e.Button == MouseButtons.Left)
                {   int i = toolTable.getToolNr(clr, conversionMode);
                    lblStatus.Text = clr.ToString() + " = " + toolTable.getToolName(i);
                    cbExceptColor.BackColor = clr;
                    showResultImage();
                }
                float zoom =  (float)nUDWidth.Value / pictureBox1.Width;
                toolTip1.SetToolTip(pictureBox1, (e.X * zoom).ToString() + "  " + (e.Y * zoom).ToString() + "   " +clr.ToString());
                oldPoint = e.Location;
            }
        }

        private void cbExceptColor_CheckedChanged(object sender, EventArgs e)
        {   if (cbExceptColor.Checked)
                toolTable.setExceptionColor(cbExceptColor.BackColor);
            else
                toolTable.clrExceptionColor();
            redoColorAdjust = true;
            applyColorCorrections();
        }

        /// <summary>
        /// Set contrast color for text
        /// </summary>
        private void cbExceptColor_BackColorChanged(object sender, EventArgs e)
        {   cbExceptColor.ForeColor = ContrastColor(cbExceptColor.BackColor);   }

        /// <summary>
        /// for 'diagonal' no outline
        /// </summary>
        private void rbEngravingPattern2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEngravingPattern2.Checked)
            {
                cBGCodeOutline.Checked = false;
                cBGCodeOutline.Enabled = false;
                cBGCodeFill.Checked = true;
                cBGCodeFill.Enabled = false;
            }
            else
            {
                cBGCodeOutline.Checked = true;
                cBGCodeOutline.Enabled = true;
                cBGCodeFill.Checked = true;
                cBGCodeFill.Enabled = true;
            }
        }

        /// <summary>
        /// if 'Draw outline' is unchecked, disable smoothing
        /// </summary>
        private void cBGCodeOutline_CheckedChanged(object sender, EventArgs e)
        {   cBGCodeOutlineSmooth.Enabled = cBGCodeOutline.Checked;
            nUDGCodeOutlineSmooth.Enabled = cBGCodeOutline.Checked;
            applyColorCorrections();
        }

        /// <summary>
        /// Grayscale checked
        /// </summary>
        private void rBProcessZ_CheckedChanged(object sender, EventArgs e)
        {   resetColorCorrection(); applyColorCorrections(); lblImageSource.Text = "original";
            if (rBProcessZ.Checked)
            {   cBPreview.Checked = false;
                cbGrayscale.Checked = true;
            }
        }
        /// <summary>
        /// Calculate contrast color from given color
        /// </summary>
        private Color ContrastColor(Color color)
        {   int d = 0;
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

    }
}
