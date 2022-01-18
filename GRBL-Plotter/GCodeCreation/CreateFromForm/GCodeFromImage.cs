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

/*  Thanks to:
 *  3dpBurner Image2Gcode. A Image to GCODE converter for GRBL based devices.
    This file is part of 3dpBurner Image2Gcode application. 
    Copyright (C) 2015  Adrian V. J. (villamany) contact: villamany@gmail.com
*/
/*
 * 2018-11  split code into ...Create and ...Outline
 * 2019-08-15 add logger
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2021-04-03 add preset for S value range
 * 2021-04-14 line 1124 only horizontal scanning for process tool
 * 2021-07-26 code clean up / code quality
 * 2021-11-23 line 309 check nUDMaxColors.maximum, line 1137 add catch
 * 2021-12-10 fix ThreadException LockBits line 1079, line 512
 * 2022-01-07 add try/catch for BtnLoad_Click, LoadExtern, CountImageColors
*/

using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class GCodeFromImage : Form
    {
        Bitmap originalImage;
        Bitmap adjustedImage;
        Bitmap resultImage;
        private static int toolTableCount = 0;      // amount of tools
        private static int imageColors = 0;         // amount of single colors

        private static readonly bool gcodeSpindleToggle = false; // Switch on/off spindle for Pen down/up (M3/M5)
        private static bool loadFromFile = false;
        private static readonly Color backgroundColor = Color.White;

        private string imagegcode = "";
        public string ImageGCode
        { get { return imagegcode; } }

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        //On form load
        private void GCodeFromImage_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Icon;
            lblStatus.Text = "Done";
            GetToolTableSettings();
            AutoZoomToolStripMenuItem_Click(this, null);//Set preview zoom mode
            FillUseCaseFileList(Datapath.Usecases);
            FillPatternFilesList(Datapath.Examples);
            rBGrayZ.Checked = Properties.Settings.Default.importImageGrayAsZ;
        }
        private void GCodeFromImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Info("++++++ GCodeFromImage STOP ++++++");
            Properties.Settings.Default.locationImageForm = Location;
        }
        private void GCodeFromImage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                LoadClipboard();
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

        public GCodeFromImage(bool loadFile = false)
        {
            Logger.Info(culture, "++++++ GCodeFromImage loadFile:{0} START ++++++", loadFile);
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            loadFromFile = loadFile;
            FillUseCaseFileList(Datapath.Usecases);
            FillPatternFilesList(Datapath.Examples);
            this.Icon = Properties.Resources.Icon;
        }


        #region load picture
        // load picture when form opens
        private void ImageToGCode_Load(object sender, EventArgs e)
        {
            if (loadFromFile) LoadExtern(lastFile);
            originalImage = new Bitmap(Properties.Resources.modell);
            Location = Properties.Settings.Default.locationImageForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }
            ProcessLoading();   // reset color corrections
            if (!Properties.Settings.Default.importImageGrayAsZ)
                rBGrayS.Checked = true;
            Highlight();
        }

        private static string lastFile = "";
        //OpenFile, save picture grayscaled to originalImage and save the original aspect ratio to ratio
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try {
                using (OpenFileDialog sfd = new OpenFileDialog())
                {
                    sfd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (!File.Exists(sfd.FileName)) return;
                        lastFile = sfd.FileName;
                        try
                        {
                            originalImage = new Bitmap(System.Drawing.Image.FromFile(sfd.FileName));
						    Logger.Info("### btnLoad_Click: {0}",sfd.FileName);
                            ProcessLoading();   // reset color corrections
                        }
                        catch (Exception err)
                        {
                            Logger.Error(err, "BtnLoad_Click ");
                            MessageBox.Show("Error loading image:\r\n" + err.Message, "Error");
                        }
                    }
                }
            }
            catch (Exception err) {
                Logger.Error(err, "btnLoad_Click ");
                MessageBox.Show("Error opening file: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadExtern(string file)
        {
            if (!File.Exists(file)) return;
            lastFile = file;
            try
            {
                originalImage = new Bitmap(System.Drawing.Image.FromFile(file));
                Logger.Info("### LoadExtern: {0}", file);
                ProcessLoading();   // reset color corrections
            }
            catch (Exception err)
            {   Logger.Error(err, "LoadExtern ");
                MessageBox.Show("Error loading image:\r\n"+err.Message,"Error");
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        { Clipboard.SetImage(pictureBox1.Image); }

        private void SetAsOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            originalImage = new Bitmap(pictureBox1.Image);
            ProcessLoading();
        }

        private void PasteFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        { LoadClipboard(); }
        public void LoadClipboard()
        {
			try
            {	IDataObject iData = Clipboard.GetDataObject();
				Logger.Info("### pasteFromClipboard");
				if ((iData != null) && (iData.GetDataPresent(DataFormats.Bitmap)))
				{
					lastFile = "";
					originalImage = new Bitmap(Clipboard.GetImage());
					ProcessLoading();   // reset color corrections
				}
			}
			catch (Exception err)
			{	Logger.Error(err,"LoadClipboard ");	}
        }

        public void LoadUrl(string url)
        {
            pictureBox1.Load(url);
            originalImage = new Bitmap(pictureBox1.Image);
            ProcessLoading();   // reset color corrections
        }
        private void GCodeFromImage_DragEnter(object sender, DragEventArgs e)
        { e.Effect = DragDropEffects.All; }

        private void GCodeFromImage_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            { LoadExtern(files[0]); }
        }

        decimal ratio; //Used to lock the aspect ratio when the option is selected
        private void ProcessLoading()
        {   //rBImportSVGTool.Checked = Properties.Settings.Default.importSVGToolSort;
            //          if (!rBImportSVGTool.Checked)
            //              rBImportSVGTool2.Checked = true;
            Logger.Trace("processLoading");
            lblStatus.Text = "Opening file...";
            adjustedImage = new Bitmap(originalImage);
            resultImage = new Bitmap(originalImage);

            ratio = (decimal)((double)originalImage.Width / (double)originalImage.Height);         //Save ratio for future use if needled
            if (ratio == 0) 
            {   Logger.Error("ProcessLoading ratio=0  width:{0}  height:{1}", originalImage.Width, originalImage.Height);
                ratio = 1;
            }
            nUDHeight.ValueChanged -= NudWidthHeight_ValueChanged;
            oldWidth = Properties.Settings.Default.importImageWidth;    //nUDWidth.Value;
            oldHeight = (decimal)(oldWidth / ratio);               //Initialize y size
            if (nUDHeight.Maximum < oldHeight) { nUDHeight.Maximum = oldHeight; }
            if (nUDHeight.Minimum > oldHeight) { nUDHeight.Minimum = oldHeight; }
            nUDHeight.Value = oldHeight;
            nUDHeight.ValueChanged += NudWidthHeight_ValueChanged;

            tabControl1.SelectedIndex = 0;
            tabControl2.SelectedIndex = 0;

            GetToolTableSettings();
            imageColors = CountImageColors();
            if (imageColors < toolTableCount)
                nUDMaxColors.Value = imageColors;
            ListAvailableTools();
            ResetColorCorrection(); 
            ApplyColorCorrections(); 
            lblImageSource.Text = "original";
            ShowInfo();
        }

        private void ShowInfo()
        {
            decimal resoY = nUDResoX.Value;
            if (nUDResoY.Enabled) { resoY = nUDResoY.Value; }

            int xSize = (int)(nUDWidth.Value / nUDResoX.Value);  //Total X pixels of resulting image for GCode generation
            int ySize = (int)(nUDHeight.Value / resoY); //Convert.ToInt32(float.Parse(tbHeight.Text, CultureInfo.InvariantCulture.NumberFormat) / float.Parse(tbRes.Text, CultureInfo.InvariantCulture.NumberFormat));
            pixelCount = xSize * ySize;
            lblSizeOrig.Text = "Original size: " + originalImage.Width + " x " + originalImage.Height + " = " + (originalImage.Width * originalImage.Height) + " px";
            lblSizeResult.Text = "Result size: " + xSize.ToString() + " x " + ySize.ToString() + " = " + pixelCount.ToString() + " px";
            string tmp = "Press 'Preview' to update tool list\r\n\r\nNumber of pens: " + (toolTableCount - 1) + "\r\n\r\n";
            tmp += "Original Image size (px):\r\n" + originalImage.Width + " px * " + originalImage.Height + " px = " + (originalImage.Width * originalImage.Height) + " px\r\n\r\n";
            tmp += "Result Image size (px)  :\r\n" + xSize.ToString() + " px * " + ySize.ToString() + " px = " + pixelCount.ToString() + " px\r\n\r\n";
            tmp += "Result image size(units): \r\nWidth: " + Math.Round(nUDWidth.Value, 1);
            tmp += "  Height: " + Math.Round(nUDHeight.Value, 1) + "\r\n\r\n";
            tBToolList.Text = tmp;
        }

        #endregion


        private void GetToolTableSettings()
        {
            toolTableCount = ToolTable.Init();// - 1;       // 1 entry reserved
            nUDMaxColors.Maximum = toolTableCount ;
            nUDMaxColors.Value = toolTableCount ;
            ListAvailableTools();
        }
        private void ListAvailableTools(bool all = true)
        {
            cLBTools.Items.Clear();
            ToolTable.SortByToolNR(false);
            int tmpCount = pixelCount;                // keep original counter 
            if (cbExceptColor.Checked)
                tmpCount -= ToolTable.PixelCount(0);  // no color-except
            if (tmpCount < 1) { tmpCount = 1; }

            ToolTable.SortByPixelCount(false);
            int listed = 0;
            int toolPixelCount;
            float percent;
            for (int i = 0; i < toolTableCount; i++)
            {
                ToolTable.SetIndex(i);
                toolPixelCount = ToolTable.PixelCount(i);
                percent = (toolPixelCount * 100 / tmpCount);
                if ((ToolTable.IndexToolNR() >= 0) && (all || ToolTable.IndexUse()))
                {
                    cLBTools.Items.Add(string.Format("{0,2}) {1,10} {2,2}%",ToolTable.IndexToolNR(), ToolTable.GetName(), percent), ToolTable.IndexSelected());
                    listed++;
                }
            }
         //   Logger.Info("setToolList all:{0} listed:{1}", toolTableCount, listed);
        }
        /// <summary>
        /// update result after deselecting tools
        /// </summary>
        private void ClbTools_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolTable.SortByToolNR(false);
            //   int toolNr;
            for (int i = 0; i < cLBTools.Items.Count; i++)      // index = unknown
            {
                if (Int32.TryParse((cLBTools.Items[i].ToString().Split(')'))[0], out int toolNr))    // get toolNr from text
                {
                    ToolTable.SetIndex(ToolTable.GetIndexByToolNR(toolNr));                     // get index from toolNr
                    ToolTable.SetSelected(cLBTools.GetItemChecked(i));                          // set selected property of index
                }
            }
            GenerateResultImage(ref resultToolNrArray);      // fill countColors
            showResultActive = true;
            pictureBox1.Image = resultImage;
            lblImageSource.Text = "result";
            Refresh();
        }


        #region preset color correction
        private void BtnResetCorrection_Click(object sender, EventArgs e)
        { ResetColorCorrection(); ApplyColorCorrections(); lblImageSource.Text = "original"; }

        private void BtnPresetCorrection1_Click(object sender, EventArgs e)
        { ResetColorCorrection(); PresetColorCorrection(1); ApplyColorCorrections(); }
        private void BtnPresetCorrection2_Click(object sender, EventArgs e)
        { ResetColorCorrection(); PresetColorCorrection(2); ApplyColorCorrections(); }
        private void BtnPresetCorrection3_Click(object sender, EventArgs e)     // Comic - few colors
        { ResetColorCorrection(); PresetColorCorrection(3); ApplyColorCorrections(); }
        private void BtnPresetCorrection4_Click(object sender, EventArgs e)
        {
            ResetColorCorrection(); PresetColorCorrection(4); ApplyColorCorrections(); adjustedImage = ImgInvert(adjustedImage);
            originalImage = new Bitmap(adjustedImage); ResetColorCorrection();
            DisableControlEvents(); cbGrayscale.Checked = true; tBarGamma.Value = 10;
            cbExceptColor.Checked = true; ToolTable.SetExceptionColor(cbExceptColor.BackColor);
            cBPreview.Checked = true; cBReduceColorsToolTable.Checked = true; 
			if (nUDMaxColors.Maximum >= 2)
				nUDMaxColors.Value = 2;
            EnableControlEvents(); ApplyColorCorrections();
        }

        private void ResetColorCorrection()
        {
            DisableControlEvents();
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
            GetToolTableSettings();          // get actual toolTable and show
            cBReduceColorsToolTable.Checked = false;
            cBReduceColorsDithering.Checked = false;
            rBMode0.Checked = true;
            cbExceptColor.Checked = false;
            cbExceptColor.BackColor = Color.White;
            ToolTable.ClrExceptionColor();
            cBPreview.Checked = false;
            redoColorAdjust = false;
            //           updatePixelCountPerColorNeeded = false;
            lblImageSource.Text = "original";
            EnableControlEvents();
        }
        private void DisableControlEvents()
        {
            tBarBrightness.Scroll -= ApplyColorCorrectionsEvent;
            tBarContrast.Scroll -= ApplyColorCorrectionsEvent;
            tBarGamma.Scroll -= ApplyColorCorrectionsEvent;
            tBarSaturation.Scroll -= ApplyColorCorrectionsEvent;
            cbGrayscale.CheckedChanged -= ApplyColorCorrectionsEvent;
            tBRMin.Scroll -= ApplyColorCorrectionsEvent;
            tBGMin.Scroll -= ApplyColorCorrectionsEvent;
            tBBMin.Scroll -= ApplyColorCorrectionsEvent;
            tBarHueShift.Scroll -= ApplyColorCorrectionsEvent;
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
        }
        private void EnableControlEvents()
        {
            tBarBrightness.Scroll += ApplyColorCorrectionsEvent;
            tBarContrast.Scroll += ApplyColorCorrectionsEvent;
            tBarGamma.Scroll += ApplyColorCorrectionsEvent;
            tBarSaturation.Scroll += ApplyColorCorrectionsEvent;
            cbGrayscale.CheckedChanged += ApplyColorCorrectionsEvent;
            tBRMin.Scroll += ApplyColorCorrectionsEvent;
            tBGMin.Scroll += ApplyColorCorrectionsEvent;
            tBBMin.Scroll += ApplyColorCorrectionsEvent;
            tBarHueShift.Scroll += ApplyColorCorrectionsEvent;
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
        }

        private static bool redoColorAdjust = false;
        private void PresetColorCorrection(int tmp)
        {
            Logger.Info("redoColorAdjust :{0}", tmp);
            DisableControlEvents();
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
                if (imageColors > toolTableCount)
                {
                    tBarBrightness.Value = -15;
                    tBarContrast.Value = 20;
                    tBarGamma.Value = 88;
                }
                else 
                { nUDColorPercent.Value = 1; }
                cBReduceColorsToolTable.Checked = true;
                cbExceptColor.Checked = true;               // except white
                cBFilterRemoveArtefact.Checked = true;
                rBMode0.Checked = true;
                redoColorAdjust = true;
            }
            else if (tmp == 4)              // comic few colors
            {
                cbGrayscale.Checked = true;
                rbModeGray.Checked = true;
                cBFilterEdge.Checked = true;
            }
            cBPreview.Checked = true;
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
            Refresh();
        }
        private void BtnShowSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ListColorCorrection(), "Color correction settings");
            System.Windows.Forms.Clipboard.SetText(ListColorCorrection());
        }
        #endregion


        #region image manipulation

        private static decimal oldWidth = 0, oldHeight = 0;
        private void NudWidthHeight_ValueChanged(object sender, EventArgs e)
        {
            nUDWidth.ValueChanged -= NudWidthHeight_ValueChanged;
            nUDHeight.ValueChanged -= NudWidthHeight_ValueChanged;
            bool edit = false;
            if (ratio == 0)
            {
                Logger.Error("NudWidthHeight_ValueChanged ratio=0  width:{0}  height:{1}", originalImage.Width, originalImage.Height);
                ratio = 1;
            }
            if (oldWidth != nUDWidth.Value)
            {
                oldWidth = nUDWidth.Value;
                if (cbLockRatio.Checked)
                {
                    oldHeight = oldWidth / ratio;
					if (nUDHeight.Minimum > oldHeight)	{nUDHeight.Minimum = oldHeight;}
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
                    oldWidth = oldHeight * ratio;
					if (nUDWidth.Minimum > oldWidth)	{nUDWidth.Minimum = oldWidth;}
                    nUDWidth.Value = oldWidth;
                    nUDWidth.Invalidate();
                }
                nUDHeight.Value = oldHeight;
                edit = true;
            }
            if (edit)
                ApplyColorCorrections();
            nUDWidth.ValueChanged += NudWidthHeight_ValueChanged;
            nUDHeight.ValueChanged += NudWidthHeight_ValueChanged;
            Refresh();
        }

        private void BtnKeepSizeWidth_Click(object sender, EventArgs e)
        {   decimal val = originalImage.Width * nUDResoX.Value;
            if (nUDWidth.Maximum < val) { nUDWidth.Maximum = val; }
            if (nUDWidth.Minimum > val) { nUDWidth.Minimum = val; }
            nUDWidth.Value = val; }

        private void BtnKeepSizeReso_Click(object sender, EventArgs e)
        {   decimal val = (decimal)(nUDWidth.Value / originalImage.Width);
            if (nUDResoX.Maximum < val) { nUDResoX.Maximum = val; }
            if (nUDResoX.Minimum > val) { nUDResoX.Minimum = val; }
            nUDResoX.Value = val; }


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
            ratio = 1 / ratio;
            decimal s = nUDHeight.Value;
            nUDHeight.Value = nUDWidth.Value;
            nUDWidth.Value = s;
            pictureBox1.Image = adjustedImage;
            AutoZoomToolStripMenuItem_Click(this, null);
            ShowResultImage();
        }
        //Rotate left
        private void BtnRotateLeft_Click(object sender, EventArgs e)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            adjustedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            originalImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            ratio = 1 / ratio;
            decimal s = nUDHeight.Value;
            nUDHeight.Value = nUDWidth.Value;
            nUDWidth.Value = s;
            pictureBox1.Image = adjustedImage;
            AutoZoomToolStripMenuItem_Click(this, null);
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

        //Contrast adjusted by user
        private void ApplyColorCorrectionsEvent(object sender, EventArgs e)
        {   if (nUDResoY.Value < nUDResoX.Value) { nUDResoY.Value = nUDResoX.Value; }
            ApplyColorCorrections();
        }

        private static int conversionMode = 0, conversionModeOld = 0;
        private void RbMode0_CheckedChanged(object sender, EventArgs e)
        {
            conversionMode = 0;
            if (rBMode1.Checked) conversionMode = 1;
            else if (rBMode2.Checked) conversionMode = 2;
            if (conversionMode != conversionModeOld)
                ApplyColorCorrections();
            conversionModeOld = conversionMode;
        }

        //Apply dithering to an image (Convert to 1 bit)
        private Bitmap ImgDirther(Bitmap input)
        {
            lblStatus.Text = "Dithering...";
            Refresh();
            var masks = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };
            var output = new Bitmap(input.Width, input.Height, PixelFormat.Format1bppIndexed);
            var data = new sbyte[input.Width, input.Height];
            var inputData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try {
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
            finally { 
				input.UnlockBits(inputData); 
			}

            var outputData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            try {
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
            finally {
                output.UnlockBits(outputData);
            }
            lblStatus.Text = "Done";
            Refresh();
            return (output);
        }

        private static double GetGreyLevel(byte r, byte g, byte b)//aux for dirthering
        { return (r * 0.299 + g * 0.587 + b * 0.114) / 255; }
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

        //Invoked when the user input any value for image adjust
        private static int pixelCount = 100;
        private static bool useFullReso = false;
        private static decimal resoFullX = 1;
        private static decimal resoFullY = 1;
        private static decimal resoDesiredX = 1;
        private static decimal resoDesiredY = 1;
        private static int resoFactorX = 1;
        private static int resoFactorY = 1;
        private void ApplyColorCorrections()
        {
            Cursor.Current = Cursors.WaitCursor;
            useFullReso = (cBGCodeOutline.Checked && rBProcessTool.Checked);
            UpdateLabels();
            lblStatus.Text = "Apply color corrections...";

            resoDesiredX = nUDResoX.Value;
            resoDesiredY = nUDResoX.Value;
            //          if (nUDResoY.Enabled) { resoDesiredY = nUDResoY.Value; }

            resoFactorX = 1;
            resoFactorY = 1;
            if (useFullReso)                                        // if full resolution is needed
            {
                resoFullX = (nUDWidth.Value / originalImage.Width);      // get max possible resolution
                resoFactorX = (int)Math.Ceiling(resoDesiredX / resoFullX); // get rounded factor to set resolution
                if (resoFactorX > 5)
                    resoFactorX = 5;
                resoDesiredX /= resoFactorX;              // set rounded value
                resoFullY = (nUDHeight.Value / originalImage.Height);      // get max possible resolution
                resoFactorY = (int)Math.Ceiling(resoDesiredY / resoFullY); // get rounded factor to set resolution
                if (resoFactorY > 5)
                    resoFactorY = 5;
                resoDesiredY /= resoFactorY;              // set rounded value    
            }
            lblInfo1.Text = "ResoX: " + Math.Round(resoDesiredX, 3) + "  factorX: " + resoFactorX + "   ResoY: " + Math.Round(resoDesiredY, 3) + "  factorY: " + resoFactorY;
            int xSize = (int)(nUDWidth.Value / resoDesiredX);  //Total X pixels of resulting image for GCode generation
            int ySize = (int)(nUDHeight.Value / resoDesiredY); //Convert.ToInt32(float.Parse(tbHeight.Text, CultureInfo.InvariantCulture.NumberFormat) / float.Parse(tbRes.Text, CultureInfo.InvariantCulture.NumberFormat));

            ShowInfo();             // show size of orig and result reso
            Refresh();
            try {
                if (adjustedImage == null) return;//if no image, do nothing

                ResizeNearestNeighbor filterResize = new ResizeNearestNeighbor(xSize, ySize);
                adjustedImage = filterResize.Apply(originalImage);
                adjustedImage = ImgBalance(adjustedImage, tBarBrightness.Value, tBarContrast.Value, tBarGamma.Value);
                adjustedImage = RemoveAlpha(adjustedImage);

                SaturationCorrection filterS = new SaturationCorrection((float)tBarSaturation.Value / 255);
                filterS.ApplyInPlace(adjustedImage);

                // create filter
                ChannelFiltering filterC = new ChannelFiltering
                {
                    // set channels' ranges to keep
                    Red = new AForge.IntRange((int)tBRMin.Value, (int)tBRMax.Value),
                    Green = new AForge.IntRange((int)tBGMin.Value, (int)tBGMax.Value),
                    Blue = new AForge.IntRange((int)tBBMin.Value, (int)tBBMax.Value)
                };
                filterC.ApplyInPlace(adjustedImage);

                HueShift filter1 = new HueShift((int)tBarHueShift.Value);   // self made filter, not part of AForge
                filter1.ApplyInPlace(adjustedImage);

                if (cBFilterEdge.Checked)
                {
                    Edges filterEdge = new Edges();
                    filterEdge.ApplyInPlace(adjustedImage);
                }
                if (cBFilterHistogram.Checked)
                {
                    HistogramEqualization filterHisto = new HistogramEqualization();
                    filterHisto.ApplyInPlace(adjustedImage);
                }
                if (cBPosterize.Checked)
                {
                    SimplePosterization filter2 = new SimplePosterization();
                    filter2.ApplyInPlace(adjustedImage);
                }

                if (cBFilterRemoveArtefact.Checked)
                {
                    adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, PixelFormat.Format24bppRgb);
                    RemoveArtefact filterRA = new RemoveArtefact(5, 3);
                    filterRA.ApplyInPlace(adjustedImage);
                }

                if (cbGrayscale.Checked)// cbDirthering.Text == "Dirthering FS 1 bit")
                {
                    if (rbModeDither.Checked)
                        adjustedImage = ImgDirther(adjustedImage);
                    else
                        adjustedImage = ImgGrayscale(adjustedImage);

                    adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB
                }
                updateLabelColorCount = true;

                ToolTable.SetAllSelected(true);     //  enable all tools
                if (cBReduceColorsToolTable.Checked)
                {
                    List<Color> myPalette = new List<Color>();
                    ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());
                    CountResultColors();        // get all possible tool-colors and its count

                    if (redoColorAdjust)        // 
                    {
                        redoColorAdjust = false;

                        int matchLimit = 0;
                        ToolTable.SortByToolNR(false);
                        int tmpCount = pixelCount;                // keep original counter 
                        if (cbExceptColor.Checked)
                            tmpCount -= ToolTable.PixelCount(0);  // no color-except
                        if (tmpCount < 1) { tmpCount = 1; }

                        int toolPixelCount;
                        decimal percent;
                        Logger.Info("applyColorCorrections redoColorAdjust pixelCount:{0} tmpCount:{1} percentLimit:{2} ", pixelCount, tmpCount, nUDColorPercent.Value);
                        for (int i = 0; i < toolTableCount; i++)
                        {
                            toolPixelCount = ToolTable.PixelCount(i);
                            percent = (toolPixelCount * 100 / tmpCount);
                            Logger.Info("applyColorCorrections redoColorAdjust {0,2} {1,3} {2,10} {3,8} {4,7} {5}%", i, ToolTable.GetToolNr(), ToolTable.GetName(), ColorTranslator.ToHtml(ToolTable.GetColor()), toolPixelCount, percent) ;
                            if (percent >= nUDColorPercent.Value)
                            { matchLimit++; }// tmp += toolTable.getName() + "  " + (toolTable.pixelCount(i) * 100 / tmpCount) + "\r\n"; }
                            else
                            { ToolTable.SetPresent(false); }    // below limit, disable color / tool-nr
                        }
                        if (matchLimit < nUDMaxColors.Minimum) { matchLimit = (int)nUDMaxColors.Maximum; }
                        if (matchLimit > nUDMaxColors.Maximum) { matchLimit = (int)nUDMaxColors.Maximum; }
                        nUDMaxColors.ValueChanged -= ApplyColorCorrectionsEvent;    // set new value without generating an event
                        nUDMaxColors.Value = matchLimit;
                        nUDMaxColors.ValueChanged += ApplyColorCorrectionsEvent;
                    }

                    if (cbExceptColor.Checked)
                        myPalette.Add(cbExceptColor.BackColor);
                    ToolTable.SortByPixelCount(false);                  // fill palette with colors in order of occurence
                    ToolTable.SetAllSelected(false);                    // 
                    for (int i = 0; i < (int)nUDMaxColors.Value; i++)   // add colors to AForge filter
                    {
                        ToolTable.SetIndex(i);
                        if (ToolTable.IndexToolNR() >= 0)
                        {
                            myPalette.Add(ToolTable.IndexColor());
                            ToolTable.SetSelected(true);
                        }
                    }
                    ListAvailableTools(false);      // show only applied colors

                    if (cBReduceColorsDithering.Checked)
                    {
                        OrderedColorDithering dithering = new OrderedColorDithering
                        {
                            ColorTable = myPalette.ToArray()
                        };
                        adjustedImage = dithering.Apply(adjustedImage);
                    }
                    else
                        adjustedImage = ciq.ReduceColors(adjustedImage, myPalette.ToArray());// (int)nUDMaxColors.Value);                                                                                                 // adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB
                }
                else
                    ListAvailableTools();

           /*     if (cBFilterRemoveArtefact.Checked)
                {
                    adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, PixelFormat.Format24bppRgb);
                    RemoveArtefact filterRA = new RemoveArtefact(5, 3);
                    filterRA.ApplyInPlace(adjustedImage);
                }*/
                adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB
                if (!cBPreview.Checked)
                    pictureBox1.Image = adjustedImage;

                resultImage = new Bitmap(adjustedImage);
                lblStatus.Text = "Done";
                lblImageSource.Text = "modified";
                Refresh();
            }
            catch (Exception err) {
                Logger.Error(err, "applyColorCorrections ");
                MessageBox.Show("Error resizing/balancing image: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ShowResultImage();  // if checked, show final result with tool colors
            Cursor.Current = Cursors.Default;
        }

        private static bool showResultActive = false;
        private static bool showResultPreview = false;
        private void ShowResultImage(bool showResult = true)//, bool preview = false)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            if (cBPreview.Checked || showResultPreview)
            {
                showResultPreview = false;

                GenerateResultImage(ref resultToolNrArray);      // fill countColors
                showResultActive = true;
                if (showResult)
                {
                    UpdateToolList();
                    pictureBox1.Image = resultImage;
                    lblImageSource.Text = "result";
                }
                Refresh();
            }
            else
            {
                pictureBox1.Image = adjustedImage;
                showResultActive = false;
                lblImageSource.Text = "modified";
            }
        }

        /// <summary>
        /// Simulate creating result image, count usage of tool-colors
        /// </summary>
        private void CountResultColors()    // update pixelCounts for specific tool-colors
        {
            Color myColor;
            if (cbExceptColor.Checked)
                ToolTable.SetExceptionColor(cbExceptColor.BackColor);
            else
                ToolTable.ClrExceptionColor();
            ToolTable.Clear();                  // ColorPresent = false; ToolSelected = true; PixelCount = 0; Diff = int.MaxValue;
            int mode = conversionMode;
            BitmapData dataAdjusted = null;
            sbyte myToolNr;//, myIndex;
            Dictionary<Color, sbyte> lookUpToolNr = new Dictionary<Color, sbyte>();
            try {
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
                    { myToolNr = -1; ToolTable.SortByToolNR(false); ToolTable.SetIndex(0); }
                    else
                    {
                        if (lookUpToolNr.TryGetValue(myColor, out myToolNr))                // myColor already registered
                        {
                            ToolTable.SetIndex(ToolTable.GetIndexByToolNR(myToolNr));       // set index
                        }
                        else
                        {
                            myToolNr = (sbyte)ToolTable.GetToolNRByColor(myColor, mode);    // find nearest color in palette, sort by match, set index to 0
                            lookUpToolNr.Add(myColor, myToolNr);
                        }
                    }
                    ToolTable.CountPixel();                                                 // count pixel / color of selected index
                    ToolTable.SetPresent(true);                                             // set flag "ColorPresent"
                }
            }
            finally {
                if (adjustedImage != null) adjustedImage.UnlockBits(dataAdjusted);
            }
            Logger.Info("countResultColors - different colors: {0}", lookUpToolNr.Count);
            ListAvailableTools();                      // list tools IndexUse
        }

        // Replace orginal color by nearest color from tool table
        // fill-up usedColor array
        private static sbyte[,] resultToolNrArray;
        /// <summary>
        /// Generate result image and fill resultToolNrArray
        /// </summary>
        private void GenerateResultImage(ref sbyte[,] tmpToolNrArray)      // and count tool colors
        {//https://www.codeproject.com/Articles/17162/Fast-Color-Depth-Change-for-Bitmaps
            Color myColor, newColor;
            if (cbExceptColor.Checked)
                ToolTable.SetExceptionColor(cbExceptColor.BackColor);
            else
                ToolTable.ClrExceptionColor();
            ToolTable.Clear();                  // ColorPresent = false; ToolSelected = true; PixelCount = 0; Diff = int.MaxValue;
            int mode = conversionMode;
            BitmapData dataAdjusted = null;
            BitmapData dataResult = null;
            lblStatus.Text = "Generate result image...";
            //     sbyte myToolNr;//, myIndex;
            Dictionary<Color, sbyte> lookUpToolNr = new Dictionary<Color, sbyte>();
            try {
				
				int rectWidth, rectHeight;
				rectWidth = Math.Min(adjustedImage.Width, resultImage.Width);
				rectHeight = Math.Min(adjustedImage.Height, resultImage.Height);
                Rectangle rectAdjusted = new Rectangle(0, 0, rectWidth, rectHeight);	//adjustedImage.Width, adjustedImage.Height);
                Rectangle rectResult = new Rectangle(0, 0, rectWidth, rectHeight);	//resultImage.Width, resultImage.Height);
				
                dataAdjusted = adjustedImage.LockBits(rectAdjusted, ImageLockMode.ReadOnly, adjustedImage.PixelFormat);
                dataResult = resultImage.LockBits(rectResult, ImageLockMode.WriteOnly, adjustedImage.PixelFormat);
				
                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                IntPtr ptrResult = dataResult.Scan0;
                int psize = 4;  // 32bppARGB GetPixelInfoSize(adjustedImage.PixelFormat);
                int bsize = dataAdjusted.Stride * rectHeight;	//adjustedImage.Height;
                byte[] pixelsAdjusted = new byte[bsize];
                byte[] pixelsResult = new byte[bsize];
                tmpToolNrArray = new sbyte[rectWidth, rectHeight];	//adjustedImage.Width, adjustedImage.Height];
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
                    if (lookUpToolNr.TryGetValue(myColor, out sbyte myToolNr)) {        // color already indexed
                        ToolTable.SetIndex(ToolTable.GetIndexByToolNR(myToolNr));       // set as usable
                    }
                    else {
                        myToolNr = (sbyte)ToolTable.GetToolNRByColor(myColor, mode);     // find nearest color in palette, sort by match, set index to 0
                        lookUpToolNr.Add(myColor, myToolNr);
                    }

                    if (myColor.A == 0)                 // skip exception, removed: cbExceptAlpha.Checked
                    { newColor = backgroundColor; myToolNr = -1; ToolTable.SortByToolNR(false); ToolTable.SetIndex(0); }// usedColorName[0] = "Alpha = 0      " + myColor.ToString(); }
                    else
                    {
                        if ((myToolNr < 0) || (!ToolTable.IndexSelected()))  // -1 = alpha, -1 = exception color
                        { newColor = backgroundColor; myToolNr = -1; }
                        else
                            newColor = ToolTable.GetColor();   // Color.FromArgb(255, r, g, b);
                    }
                    ToolTable.CountPixel(); // count pixel / color
                    ToolTable.SetPresent(true);
                    tmpToolNrArray[bx++, by] = myToolNr;

                    if (bx >= rectWidth)	//adjustedImage.Width)
                    { bx = 0; by++; }
                    // apply new color
                    pixelsResult[index] = newColor.B;// newColor.A;
                    pixelsResult[index + 1] = newColor.G;
                    pixelsResult[index + 2] = newColor.R;
                    pixelsResult[index + 3] = 255;
                }
                Marshal.Copy(pixelsResult, 0, ptrResult, pixelsResult.Length);
                if (resultImage != null) resultImage.UnlockBits(dataResult);
                if (adjustedImage != null) adjustedImage.UnlockBits(dataAdjusted);
            }
			catch (Exception err) {	
				Logger.Error(err,"generateResultImage ");
				Properties.Settings.Default.guiLastEndReason += "generateResultImage:"+err;
                if (resultImage != null) resultImage.UnlockBits(dataResult);
                if (adjustedImage != null) adjustedImage.UnlockBits(dataAdjusted);
			}
            lblStatus.Text = "Done";
            Logger.Info("generateResultImage lookUpToolNr:{0}", lookUpToolNr);
        }

        #endregion

        private bool updateLabelColorCount = false;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (updateLabelColorCount)
                CountImageColors();
        }
        /// <summary>
        /// Count amount of different colors in adjusted image
        /// </summary>
        private int CountImageColors()
        {   // Lock the bitmap's bits.  
            Rectangle rect;
            try
            {
                rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
            }
            catch (Exception err)
            {
                Logger.Error(err, "CountImageColors ");
                MessageBox.Show("Error count image colors - width or height not ok?:\r\n" + err.Message, "Error");
                return 1;
            }

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
            {
                for (int row = 0; row < bmpData.Width; row++)
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
            return cnt.Count;
        }

        //Quick preview of the original image. Todo: use a new image container for fas return to processed image
        private void BtnCheckOrig_MouseDown(object sender, MouseEventArgs e)
        { showResultPreview = true; ShowResultImage(); }
        private void BtnCheckOrig_MouseUp(object sender, MouseEventArgs e)
        {
            if (adjustedImage == null) return;//if no image, do nothing
            pictureBox1.Image = adjustedImage;
            cBPreview.Checked = false;
            lblImageSource.Text = "modified";
        }

        private void BtnShowOrig_MouseDown(object sender, MouseEventArgs e)
        { pictureBox1.Image = originalImage; lblImageSource.Text = "original"; }

        private void BtnShowOrig_MouseUp(object sender, MouseEventArgs e)
        {
            if (cBPreview.Checked)
            { pictureBox1.Image = resultImage; lblImageSource.Text = "result"; }
            else
            { pictureBox1.Image = adjustedImage; lblImageSource.Text = "modified"; }
        }

        private void UpdateToolList()
        {
            string tmp1 = "", tmp2;
            int skipToolNr = 1;

            ToolTable.SortByToolNR(false);
            ToolTable.SetIndex(0);
            int tmpCount = pixelCount;
            if (cbExceptColor.Checked)
                tmpCount -= ToolTable.PixelCount(0);     // no color-except
            if (tmpCount < 1) { tmpCount = 1; }

            ToolTable.SortByPixelCount(false);           // sort by color area (max. first)

            Logger.Info("updateToolList pixelCount:{0}  tmpCount:{1}", pixelCount, tmpCount);

            tmp2 = "Tools sorted by pixel count.\r\n";
            tmp2 += "Tool colors in use and pixel count:\r\n";
            String cIndex, cName, cCount, cPercent;
            int toolNr;
            for (int i = 0; i < toolTableCount; i++)
            {
                ToolTable.SetIndex(i);
                toolNr = ToolTable.IndexToolNR();
                if (ToolTable.IndexUse() && (ToolTable.PixelCount() > 0))
                {
                    cIndex = toolNr.ToString().PadLeft(2, ' ') + ") ";
                    cName = ToolTable.IndexName().PadRight(15, ' ');
                    cCount = ToolTable.PixelCount().ToString().PadLeft(8, ' ') + "  ";
                    cPercent = string.Format("{0,4:0.0}", (ToolTable.PixelCount() * 100 / tmpCount)) + "%";
                    if (toolNr < 0)
                        tmp1 += cIndex + cName + cCount + "\r\n";
                    else
                    {
                        if (cbSkipToolOrder.Checked)
                            tmp2 += skipToolNr++.ToString().PadLeft(2, ' ') + ") " + cName + cCount + cPercent + "\r\n";
                        else
                            tmp2 += cIndex + cName + cCount + cPercent + "\r\n";
                    }
                }
            }
            tBToolList.Text = tmp2 + "\r\n" + tmp1;
        }

        //Generate button click
        public void BtnGenerateClick(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            tabControl1.SelectedTab = tabPage5;

            if (rBProcessTool.Checked)
            {
                if (!showResultActive)
                {
                    cBPreview.Checked = true;
                    ShowResultImage(true);      // generate and show result
                }
                GenerateGCodeHorizontal();
            }
            else
                GenerateHeightData();
            Cursor.Current = Cursors.Default;
            return;
        }

        private void AutoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Width = panel1.Width;
            pictureBox1.Height = panel1.Height;
            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
        }

        private void JustShowResult(object sender, EventArgs e)
        { ShowResultImage(); }

        private Point oldPoint = new Point(0, 0);
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Location != oldPoint) || (e.Button == MouseButtons.Left))
            {
                Color clr = backgroundColor;// GetColorAt(e.Location);
                if (pictureBox1.Image != null)
                {
                    Bitmap bmp = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
                    pictureBox1.DrawToBitmap(bmp, pictureBox1.ClientRectangle);
                    if ((0 <= e.X) && (e.X <= pictureBox1.ClientSize.Width) && (0 <= e.Y) && (e.Y <= pictureBox1.ClientSize.Height))
                        clr = bmp.GetPixel(e.X, e.Y);
                    bmp.Dispose();
                }
                if (e.Button == MouseButtons.Left)
                {
                    if (tabControl3.Visible)
                    {
                        decimal rX = (nUDWidth.Value / nUDHeight.Value) / ((decimal)pictureBox1.ClientSize.Width / (decimal)pictureBox1.ClientSize.Height);
                        decimal rY = 1 / rX;
                        if (rX > 1) { rY = 1; } else { rX = 1; }
                        decimal picBoxX = (decimal)e.X * rY / (decimal)pictureBox1.ClientSize.Width - (rY - 1)/2;    // (decimal)pictureBox1.ClientSize.Width;
                        decimal picBoxY = (decimal)e.Y * rX / (decimal)pictureBox1.ClientSize.Height - (rX - 1)/2;   // (decimal)pictureBox1.ClientSize.Height;
                        if (picBoxX < 0) { picBoxX = 0; }
                        if (picBoxX > 1) { picBoxX = 1; }
                        if (picBoxY < 0) { picBoxY = 0; }
                        if (picBoxY > 1) { picBoxY = 1; }
                        NuDSpiralCenterX.Value = picBoxX;
                        NuDSpiralCenterY.Value = 1 - picBoxY;
                    }
                    else
                    {
                        int i = ToolTable.GetToolNRByColor(clr, conversionMode);
                        lblStatus.Text = clr.ToString() + " = " + ToolTable.GetToolName(i);
                        cbExceptColor.BackColor = clr;
                        ShowResultImage();
                    }
                }
                float zoom = (float)nUDWidth.Value / pictureBox1.Width;
                toolTip1.SetToolTip(pictureBox1, (e.X * zoom).ToString("0.##") + "  " + (e.Y * zoom).ToString("0.##") + "   " + clr.ToString());
                oldPoint = e.Location;
            }
        }

        private void CbExceptColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbExceptColor.Checked)
                ToolTable.SetExceptionColor(cbExceptColor.BackColor);
            else
                ToolTable.ClrExceptionColor();
            redoColorAdjust = true;
            ApplyColorCorrections();
        }

        /// <summary>
        /// Set contrast color for text
        /// </summary>
        private void CbExceptColor_BackColorChanged(object sender, EventArgs e)
        { cbExceptColor.ForeColor = ContrastColor(cbExceptColor.BackColor); }

        /// <summary>
        /// for 'diagonal' no outline
        /// </summary>
        /// 
        private void RbEngravingPattern2_CheckedChanged(object sender, EventArgs e)
        {
            cBOnlyLeftToRight.Enabled = !rbEngravingPattern2.Checked;
            ApplyColorCorrections();
        }

        /// <summary>
        /// if 'Draw outline' is unchecked, disable smoothing
        /// </summary>
        private void CbGCodeOutline_CheckedChanged(object sender, EventArgs e)
        {
            cBGCodeOutlineSmooth.Enabled = cBGCodeOutline.Checked;
            nUDGCodeOutlineSmooth.Enabled = cBGCodeOutline.Checked;
            ApplyColorCorrections();
        }

        /// <summary>
        /// Grayscale checked
        /// </summary>
        private void TabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool useZ;
            if (tabControl2.SelectedIndex == 1)
            {
                rBProcessZ.Checked = useZ = true; rBProcessTool.Checked = false;
                nUDResoY.Enabled = cBOnlyLeftToRight.Enabled = true;
                Highlight();
            }
            else
            {
                rBProcessZ.Checked = useZ = false; rBProcessTool.Checked = true;
                cBOnlyLeftToRight.Enabled = false;
            }

         //   resetColorCorrection(); 
         //   applyColorCorrections(); 
         //   lblImageSource.Text = "original";
            cBPreview.Checked = !useZ;
            cbGrayscale.Checked = useZ;         // will cause new event applyColorCorrectionsEvent -> applyColorCorrections();
            gBgcodeSelection.Enabled = !useZ;
        }


        public string ReturnValue1 { get; set; }
        private void BtnLoad_Click_1(object sender, EventArgs e)
        {
            ReturnValue1 = "";
            if (string.IsNullOrEmpty(lBUseCase.Text))
                return;
            string path = Datapath.Usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            Logger.Trace("Load use case: '{0}'", path);
            MyIni.ReadAll();    // ReadImport();
            Properties.Settings.Default.useCaseLastLoaded = lBUseCase.Text; ;
            lblLastUseCase.Text = lBUseCase.Text;

            bool laseruse = Properties.Settings.Default.importGCSpindleToggleLaser;
            float lasermode = Grbl.GetSetting(32);
            FillUseCaseFileList(Datapath.Usecases);
            FillPatternFilesList(Datapath.Examples);

            if (lasermode >= 0)
            {
                if ((lasermode > 0) && !laseruse)
                {
                    DialogResult dialogResult = MessageBox.Show("grbl laser mode ($32) is activated, \r\nbut not recommended\r\n\r\n Press 'Yes' to fix this", "Attention", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        ReturnValue1 = "$32=0 (laser mode off)";
                }

                if ((lasermode < 1) && laseruse)
                {
                    DialogResult dialogResult = MessageBox.Show("grbl laser mode ($32) is not activated, \r\nbut recommended if a laser will be used\r\n\r\n Press 'Yes' to fix this", "Attention", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        ReturnValue1 = "$32=1 (laser mode on)";
                }
            }
        }

        private void BtnGetPWMValues_Click(object sender, EventArgs e)
        {
            nUDSTop.Value = Properties.Settings.Default.importGCPWMZero;
            nUDSBottom.Value = Properties.Settings.Default.importGCPWMDown;
        }

        private void Highlight()
        {
            if (rBGrayZ.Checked)
                groupBox8.BackColor = Color.Yellow;
            else
                groupBox8.BackColor = Color.WhiteSmoke;

            if (rBGrayS.Checked)
                groupBox4.BackColor = Color.Yellow;
            else
                groupBox4.BackColor = Color.WhiteSmoke;

        }

        private void RbGrayZ_CheckedChanged(object sender, EventArgs e)
        {
            Highlight();
        }

        private void BtnReloadPattern_Click(object sender, EventArgs e)
        {

        }

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

        private void FillUseCaseFileList(string Root)
        {
            try {
                string[] Files = System.IO.Directory.GetFiles(Root);
                lBUseCase.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("ini"))
                        lBUseCase.Items.Add(Path.GetFileName(Files[i]));
                }
                if (lBUseCase.Items.Count == 0)
                {   lBUseCase.Items.Add("No files found");
                    Logger.Error("fillUseCaseFileList no files found in {0}",Root);
                }
            }
            catch (Exception err) {
                Logger.Error(err, "fillUseCaseFileList no file found {0}", Root);
            }
        }

        private void FillPatternFilesList(string Root)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(Root);
                CBoxPatternFiles.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Path.GetFileName(Files[i]).ToLower().StartsWith("pattern"))
                        CBoxPatternFiles.Items.Add(Path.GetFileName(Files[i]));
                }
                if (CBoxPatternFiles.Items.Count == 0)
                {
                    CBoxPatternFiles.Items.Add("No files found");
                    Logger.Error("fillPatternFilesList no files found in {0}", Root);
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "fillPatternFilesList no file found {0}", Root);
            }
        }

        public string patternFile = "";
        private void CBoxPatternFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            patternFile = CBoxPatternFiles.SelectedItem.ToString();
        }

    }
}
