/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
 Load image				LoadExtern
 apply corrections		ProcessLoading	/ ApplyColorCorrections
 
 if (color mode)	use toolTable
	 count colors		CountResultColors - adjustedImage
	 show tools 		ListAvailableTools
	 
 else	 			use grayMap
	 count gray values
	 show gray values?
	 
 generate code			BtnGenerateClick
						GenerateResultImage / tmpToolNrArray (array with toolNumbers)
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
 * 2022-02-17 function CountImageColors line 1388 switch from int to long
 * 2022-03-24 add drop-down for tool-files and tool-table entries
 * 2022-03-28 move some functions to new file GCodeFromImageMisc
 * 2022-09-14 add if (adjustedImage == null) 
 * 2023-04-10 l:279 f:LoadUrl add try catch
 * 2023-06-08 l:110 f:GCodeFromImage_FormClosing    set immages=null to avoid ThreadException: Except: Parameter is not valid. Source: System.Drawing Target: Int32 get_Width()
 * 2024-05-04 l:585 f:ApplyColorCorrections check if originalImage!=null
*/

using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Bitmap originalImage;
        private Bitmap adjustedImage;
        private Bitmap resultImage;

        private static int toolTableCount = 0;      // amount of tools
        private static int imageColors = 0;         // amount of single colors

        private static int pixelCount = 100;
        private static bool useFullReso = false;
        private static decimal resoFullX = 1;
        private static decimal resoFullY = 1;
        private static decimal resoDesiredX = 1;
        private static decimal resoDesiredY = 1;
        private static int resoFactorX = 1;
        private static int resoFactorY = 1;

        private static readonly bool gcodeSpindleToggle = false; // Switch on/off spindle for Pen down/up (M3/M5)
        private static bool loadFromFile = false;
        private static readonly Color backgroundColor = Color.White;

        private static decimal AspectRatioOriginalImage; //Used to lock the aspect ratio when the option is selected
        private static decimal oldWidth = 0, oldHeight = 0;

        private string imagegcode = "";
        public string ImageGCode
        { get { return imagegcode; } }

        private bool useColorMode = false;
        private bool applyPercentLimit = false;

        internal static List<GrayProp> GrayValueMap = new List<GrayProp>();   // load color palette into this array
        private static int GrayValueMapAmountOfValues = 0;

        // Replace orginal color by nearest color from tool table
        // fill-up usedColor array
        private static short[,] resultToolNrArray;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool logEnable = true;

        private void GCodeFromImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Info("++++++ GCodeFromImage STOP ++++++");
            Properties.Settings.Default.locationImageForm = Location;
            pictureBox1.Image = null;
            adjustedImage =null;
            originalImage = null;
            resultImage = null;
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
            panel1.Width = Width - 560;
            pictureBox1.Width = Width - 560;
            panel1.Height = Height - 70;
            pictureBox1.Height = Height - 70;
            pictureBox1.Refresh();
        }

        public GCodeFromImage(bool loadFile = false)
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Logger.Info("++++++ GCodeFromImage loadFile:{0} START ++++++", loadFile);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();

            GrayValueMap = new List<GrayProp>();
            for (int i = 0; i <= 255; i++)
            { GrayValueMap.Add(new GrayProp((byte)i, 0)); }	// preset list

            loadFromFile = loadFile;
            FillUseCaseFileList(Datapath.Usecases);
            FillPatternFilesList(Datapath.Examples);
            this.Icon = Properties.Resources.Icon;
            UpdateLogging();
        }


        #region load picture
        //On form load
        /* not used ??? */
        private void GCodeFromImage_Load(object sender, EventArgs e)
        {
            //    this.Icon = Properties.Resources.Icon;
            lblStatus.Text = "Done";
            GetToolTableSettings();
            AutoZoomToolStripMenuItem_Click(this, null);//Set preview zoom mode
                                                        //    FillUseCaseFileList(Datapath.Usecases);
                                                        //    FillPatternFilesList(Datapath.Examples);
            RbStartGrayS.Checked = !Properties.Settings.Default.importImageGrayAsZ;
            //    UpdateToolTableList();
        }

        // load picture when form opens
        private void ImageToGCode_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.locationImageForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            RbStartGrayS.Checked = !Properties.Settings.Default.importImageGrayAsZ;

            DisableControlEvents();
            {
                if (Properties.Settings.Default.importImageColorMode)
                { tabControl2.SelectedTab = tabPage2Color; useColorMode = true; }           // show Color Mode tab
                else
                { tabControl2.SelectedTab = tabPage2Gray; }

                if (Properties.Settings.Default.importImageGrayVectorize)
                { RbGrayscaleVector.Checked = true; }
                else
                { RbGrayscalePattern.Checked = true; }

                if (!Properties.Settings.Default.importImageGrayAsZ)
                    RbStartGrayS.Checked = true;

                TabControl2_SelectedIndexChanged(null, null);           // don't 'ApplyColorCorrections'			
            }
            EnableControlEvents();
            ResetColorCorrectionControls();

            originalImage = new Bitmap(Properties.Resources.modell);
            if (loadFromFile) LoadExtern(lastFile, false);          // ProcessLoading later

            ProcessLoading();   		// reset color corrections
            UpdateToolTableList();		// show tool-files and last loaded tools
        }

        private static string lastFile = "";
        //OpenFile, save picture grayscaled to originalImage and save the original aspect ratio to ratio
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
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
                            Logger.Info("### btnLoad_Click: {0}", sfd.FileName);
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
            catch (Exception err)
            {
                Logger.Error(err, "btnLoad_Click ");
                MessageBox.Show("Error opening file: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadExtern(string file, bool process = true)		// called from MainFormLoadFile 466
        {
            if (!File.Exists(file)) return;
            lastFile = file;
            try
            {
                originalImage = new Bitmap(System.Drawing.Image.FromFile(file));
                Logger.Info("### LoadExtern: {0}", file);
                if (process) ProcessLoading();   // reset color corrections
            }
            catch (Exception err)
            {
                Logger.Error(err, "LoadExtern ");
                MessageBox.Show("Error loading image:\r\n" + err.Message, "Error");
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
        public void LoadClipboard()									// called from MainFormLoadFile 1383
        {
            try
            {
                IDataObject iData = Clipboard.GetDataObject();
                Logger.Info("### pasteFromClipboard");
                if ((iData != null) && (iData.GetDataPresent(DataFormats.Bitmap)))
                {
                    lastFile = "";
                    originalImage = new Bitmap(Clipboard.GetImage());
                    ProcessLoading();   // reset color corrections
                }
            }
            catch (Exception err)
            { Logger.Error(err, "LoadClipboard "); }
        }

        public void LoadUrl(string url)								// called from MainFormLoadFile 654
        {
            try
            {
                pictureBox1.Load(url);
                originalImage = new Bitmap(pictureBox1.Image);
                ProcessLoading();   // reset color corrections
            }
            catch (Exception err)
            {   Logger.Error(err, "LoadUrl {0} ", url);
                MessageBox.Show(string.Format("Error on loading from URL '{0}'\r\n{1}",url,err.Message),"Error");
            }
        }
        private void GCodeFromImage_DragEnter(object sender, DragEventArgs e)
        { e.Effect = DragDropEffects.All; }

        private void GCodeFromImage_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            { LoadExtern(files[0]); }
        }

        /**** after loading an image *****/
        private void ProcessLoading()
        {
            useColorMode = (tabControl2.SelectedIndex == 0);
            if (logEnable) Logger.Trace("ProcessLoading useColorMode:{0}", useColorMode);

            lblStatus.Text = "Opening file...";
            adjustedImage = new Bitmap(originalImage);
            resultImage = new Bitmap(originalImage);

            //    ResetColorCorrectionControls(); 

            AspectRatioOriginalImage = (decimal)((double)originalImage.Width / (double)originalImage.Height);         //Save ratio for future use if needled
            if (AspectRatioOriginalImage == 0)
            {
                Logger.Error("ProcessLoading ratio=0  width:{0}  height:{1}", originalImage.Width, originalImage.Height);
                AspectRatioOriginalImage = 1;
            }

            DisableControlEvents();
            {   /* set numeric-up/down controls */
                nUDHeight.ValueChanged -= NudWidthHeight_ValueChanged;
                oldWidth = Properties.Settings.Default.importImageWidth;    //nUDWidth.Value;
                oldHeight = (decimal)(oldWidth / AspectRatioOriginalImage);               //Initialize y size
                if (nUDHeight.Maximum < oldHeight) { nUDHeight.Maximum = oldHeight; }
                if (nUDHeight.Minimum > oldHeight) { nUDHeight.Minimum = oldHeight; }
                nUDHeight.Value = oldHeight;
                nUDHeight.ValueChanged += NudWidthHeight_ValueChanged;

                tabControl1.SelectedIndex = 0;      // switch to 1st tab after loading an image

                GetToolTableSettings();             // get max tools from tooltable or 255 in grayscale mode
                imageColors = CountImageColors();   // get amount of different colors in adjustedImage

                if (imageColors < toolTableCount)
                    nUDMaxColors.Value = imageColors;
            }
            EnableControlEvents();

            ListAvailableTools();                                   // fill CheckedListBoxTools and enable all Items
            ApplyColorCorrections("ProcessLoading");                                // show current result

            if (useColorMode)
                GenerateResultImage(ref resultToolNrArray);         // fill resultToolNrArray (Image-Pixel=ToolNr)
            else
                GenerateResultImageGray(ref resultToolNrArray);   	// fill resultToolNrArray (Image-Pixel=GrayVal)
        }

        /*    private void ShowInfo()
            {
                decimal resoY = nUDResoX.Value;
                if (nUDResoY.Enabled) { resoY = nUDResoY.Value; }

            //    int xSize = (int)(nUDWidth.Value / nUDResoX.Value);  //Total X pixels of resulting image for GCode generation
            //    int ySize = (int)(nUDHeight.Value / resoY); //Convert.ToInt32(float.Parse(tbHeight.Text, CultureInfo.InvariantCulture.NumberFormat) / float.Parse(tbRes.Text, CultureInfo.InvariantCulture.NumberFormat));
                int xSize = (int)(nUDWidth.Value / resoDesiredX);  //Total X pixels of resulting image for GCode generation
                int ySize = (int)(nUDHeight.Value / resoDesiredY); //Convert.ToInt32(float.Parse(tbHeight.Text, CultureInfo.InvariantCulture.NumberFormat) / float.Parse(tbRes.Text, CultureInfo.InvariantCulture.NumberFormat));
                pixelCount = xSize * ySize;

                Logger.Info("ShowInfo  pixelCount:{0}  {1} x {2}", pixelCount, xSize, ySize);
                lblSizeOrig.Text = "Original size: " + originalImage.Width + " x " + originalImage.Height + " = " + (originalImage.Width * originalImage.Height) + " px";
                lblSizeResult.Text = "Result size: " + xSize.ToString() + " x " + ySize.ToString() + " = " + pixelCount.ToString() + " px";
                string tmp = "Press 'Preview' to update tool list\r\n\r\nNumber of pens: " + (toolTableCount - 1) + "\r\n\r\n";
                tmp += "Original Image size (px):\r\n" + originalImage.Width + " px * " + originalImage.Height + " px = " + (originalImage.Width * originalImage.Height) + " px\r\n\r\n";
                tmp += "Result Image size (px)  :\r\n" + xSize.ToString() + " px * " + ySize.ToString() + " px = " + pixelCount.ToString() + " px\r\n\r\n";
                tmp += "Result image size(units): \r\nWidth: " + Math.Round(nUDWidth.Value, 1);
                tmp += "  Height: " + Math.Round(nUDHeight.Value, 1) + "\r\n\r\n";
                tBToolList.Text = tmp;
            }*/

        #endregion


        private void GetToolTableSettings()
        {
            if (useColorMode) // use color mode
            {
                //		if (log) Logger.Trace("GetToolTableSettings - ToolTable.Init");
                toolTableCount = ToolTable.Init(" (GCodeFromImage)");// - 1;       // 1 entry reserved
            }
            else
            {
                toolTableCount = 255;
            }
            nUDMaxColors.ValueChanged -= ApplyColorCorrectionsEvent;        // avoid event after manual change
            nUDMaxColors.Maximum = toolTableCount;
            nUDMaxColors.Value = toolTableCount;
            nUDMaxColors.ValueChanged += ApplyColorCorrectionsEvent;

            if (logEnable) Logger.Trace("GetToolTableSettings {0}  useColorMode:{1}", toolTableCount, useColorMode);
        }

        /***** CheckedListBoxTools *****/
        private void ListAvailableTools(bool all = true)
        {
            CheckedListBoxTools.SelectedIndexChanged -= CheckedListBoxTools_SelectedIndexChanged;
            CheckedListBoxTools.Items.Clear();          // CheckedListBox
            int listed = 0;
            int used = 0;
            if (useColorMode)
            {
                ToolTable.SortByToolNR(false);
                int tmpCount = pixelCount;                  // keep original counter 
                if (cbExceptColor.Checked)
                    tmpCount -= ToolTable.PixelCount(0);    // no color-except
                if (tmpCount < 1) { tmpCount = 1; }

                ToolTable.SortByPixelCount(false);
                int toolPixelCount;
                float percent;
                if (logEnable) Logger.Info("ListAvailableTools cnt1:{0}  all:{1}  exception-cnt:{2}  tmpCount:{3}", ToolTable.PixelCount(1), pixelCount, ToolTable.PixelCount(0), tmpCount);
                for (int i = 0; i < toolTableCount; i++)
                {
                    ToolTable.SetIndex(i);
                    toolPixelCount = ToolTable.PixelCount(i);
                    percent = (toolPixelCount * 100 / tmpCount);
                    if ((ToolTable.IndexToolNR() >= 0) && (all || ToolTable.IndexUse()))
                    {
                        CheckedListBoxTools.Items.Add(string.Format("{0,2}) {1,10}    {2,5:##0.00}%", ToolTable.IndexToolNR(), ToolTable.GetName(), percent), ToolTable.IndexSelected());
                        if (ToolTable.IndexSelected()) used++;
                        listed++;
                    }
                }
                if (logEnable) Logger.Trace("ListAvailableTools all:{0}  listed:{1}  used:{2}", toolTableCount, listed, used);
            }
            else
            {
                SortByGrayAmount(true);
                long toolPixelCount;
                float percent;
                bool unCheckBackground = cbExceptColor.Checked;
                //	int backGray = (int)GetGreyLevel(cbExceptColor.BackColor.R, cbExceptColor.BackColor.G, cbExceptColor.BackColor.B);
                int backGray = (cbExceptColor.BackColor.R + cbExceptColor.BackColor.G + cbExceptColor.BackColor.B) / 3;
                if (logEnable) Logger.Trace("ListAvailableTools GetGreyLevel {0}    {1}  {2}  {3} ", backGray, cbExceptColor.BackColor.R, cbExceptColor.BackColor.G, cbExceptColor.BackColor.B);
                bool useGray;

                for (int i = 0; i < GrayValueMap.Count; i++)
                {
                    toolPixelCount = GrayValueMap[i].Count;
                    percent = (toolPixelCount * 100 / pixelCount);
                    if ((all && (GrayValueMap[i].Count > 0)) || GrayValueMap[i].Use)
                    {
                        useGray = GrayValueMap[i].Use;
                        //               Logger.Trace("SetCheck index:{0}   use:{1}   GrayVal:{2}   backVal:{3}",i, useGray, GrayValueMap[i].GrayVal, backGray);
                        if (unCheckBackground && (GrayValueMap[i].GrayVal == backGray))
                        { GrayValueMap[i].Use = useGray = false; }

                        CheckedListBoxTools.Items.Add(string.Format("{0,3}) {1,6:###}    {2,5:##0.00}%", i, GrayValueMap[i].GrayVal.ToString(), percent), useGray);

                        if (useGray) used++;

                        listed++;
                    }
                }
                GrayValueMapAmountOfValues = listed;
                if (logEnable) Logger.Trace("ListAvailableTools gray all:{0}  listed:{1}  used:{2}", 255, listed, used);
            }
            CheckedListBoxTools.SelectedIndexChanged += CheckedListBoxTools_SelectedIndexChanged;
        }
        /// <summary>
        /// update result after deselecting tools
        /// </summary>
        private void CheckedListBoxTools_SelectedIndexChanged(object sender, EventArgs e)
        {

            int checkedCount = 0;
            if (useColorMode)
            {
                ToolTable.SortByToolNR(false);
                for (int i = 0; i < CheckedListBoxTools.Items.Count; i++)      // index = unknown
                {
                    if (Int32.TryParse((CheckedListBoxTools.Items[i].ToString().Split(')'))[0], out int toolNr))    // get toolNr from text
                    {
                        ToolTable.SetIndex(ToolTable.GetIndexByToolNR(toolNr));                     // get index from toolNr
                        ToolTable.SetSelected(CheckedListBoxTools.GetItemChecked(i));                          // set selected property of index
                        if (CheckedListBoxTools.GetItemChecked(i))
                            checkedCount++;
                        //				Logger.Trace("ClbTools_SelectedIndexChanged  i:{0}  toolNr:{1}   checked:{2}   IndexSelected:{3}", i, toolNr, CheckedListBoxTools.GetItemChecked(i), ToolTable.IndexSelected());
                    }
                }
                GenerateResultImage(ref resultToolNrArray);         // fill resultToolNrArray (Image-Pixel=ToolNr)
            }
            else
            {
                SortByGrayValue(false);
                string txt;

                for (int i = 0; i <= 255; i++)
                { GrayValueMap[i].Use = false; }

                bool isCheck;

                for (int i = 0; i < CheckedListBoxTools.Items.Count; i++)
                {
                    txt = CheckedListBoxTools.Items[i].ToString().Substring(8, 3);
                    //    Logger.Info("{0} '{1}'", i, txt);
                    if (Int32.TryParse(txt, out int grayIndex))    // get toolNr from text
                    {
                        isCheck = CheckedListBoxTools.GetItemChecked(i);
                        GrayValueMap[grayIndex].Use = isCheck;

                        if (isCheck)
                        {
                            checkedCount++;
                            //                    Logger.Trace("Index:{0}  grayIndex:{1}   Val:{2}  Used", i, grayIndex, GrayValueMap[grayIndex].GrayVal);
                        }
                    }
                }
                GenerateResultImageGray(ref resultToolNrArray);
            }
            if (logEnable) Logger.Trace("##### CheckedListBoxTools_SelectedIndexChanged Items:{0}   Checked:{1}", CheckedListBoxTools.Items.Count, checkedCount);


            pictureBox1.Image = resultImage;
            lblImageSource.Text = "result";
            Refresh();
        }


        /*****************************************************************
        ********************* Apply filters  *****************************
        ******************************************************************/
        //Contrast adjusted by user
        private bool preventEvent = false;
        private void ApplyColorCorrectionsEvent(object sender, EventArgs e)		// nUDMaxColors.ValueChanged  nUDResoX/Y, GrayscaleChannels, Gamma, Contrast, Bright, Satur, Hue, tBR/G/B/Min/Max
        {
            // cBReduceColorsToolTable, cBReduceColorsDithering, Filter...
            if (logEnable) Logger.Trace("ApplyColorCorrectionsEvent  sender:{0}   preventEvent:{1} ", ((Control)sender).Name, preventEvent);
            if (preventEvent) return;

            DisableControlEvents();
            {
                if ((((Control)sender).Name == "cBReduceColorsToolTable") && cBReduceColorsToolTable.Checked && (GrayValueMapAmountOfValues > 0))
                { nUDMaxColors.Maximum = GrayValueMapAmountOfValues; nUDMaxColors.Value = GrayValueMapAmountOfValues; }
                if (nUDResoY.Value < nUDResoX.Value)
                { nUDResoY.Value = nUDResoX.Value; }
            }
            EnableControlEvents();

            ApplyColorCorrections("ApplyColorCorrectionsEvent");
        }

        private bool scrollBarClicked = false;
        private void ApplyColorCorrectionsEventScrollBar(object sender, EventArgs e)		// Gamma, Contrast, Bright, Satur, Hue, tBR/G/B/Min/Max
        {
            if (scrollBarClicked)
                return;
            ApplyColorCorrectionsEvent(sender, e);
        }
        private void ScrollBar_MouseDown(object sender, MouseEventArgs e)
        { scrollBarClicked = true; }

        private void ScrollBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (!scrollBarClicked)
                return;
            scrollBarClicked = false;
            ApplyColorCorrectionsEvent(sender, e);
        }

        private void ApplyColorCorrections(string source)
        {
            Cursor.Current = Cursors.WaitCursor;

            /**********************/
            DisableControlEvents();

            bool _useColorMode = tabControl2.SelectedIndex == 0;
            useFullReso = (cBGCodeOutline.Checked && gBgcodeSelection.Enabled);             // && useColorMode
            UpdateLabels();
            lblStatus.Text = "Apply color corrections...";

            resoDesiredX = nUDResoX.Value;
            resoDesiredY = nUDResoX.Value;
            //          if (nUDResoY.Enabled) { resoDesiredY = nUDResoY.Value; }
            if (originalImage == null)
            {
                Logger.Info("●●●● ApplyColorCorrections originalImage = null");
                return;
            }
            Logger.Info("●●●● ApplyColorCorrections  source:{0}  Out-Size-mm:{1:0.00} x {2:0.00}   Original:{3} x {4}   useFullReso:{5}   redoColorAdjust:{6}", source, nUDWidth.Value, nUDHeight.Value, originalImage.Width, originalImage.Height, useFullReso, redoColorAdjust);
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
            pixelCount = xSize * ySize;
            Logger.Info("●●●  ApplyColorCorrections  pixelCount:{0}  Size:{1} x {2}   resoVal:{3}  desiredX:{4}  desiredY:{5}", pixelCount, xSize, ySize, nUDResoX.Value, resoDesiredX, resoDesiredY);

            //    ShowInfo();             // show size of orig and result reso
            //    Refresh();
            try
            {
				if (adjustedImage == null) {Logger.Warn("ApplyColorCorrections adjustedImage == null");return;}//if no image, do nothing

                ResizeNearestNeighbor filterResize = new ResizeNearestNeighbor(xSize, ySize);   // The class implements image resizing filter using nearest neighbor algorithm, which does not assume any interpolation.
                                                                                                // The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp color images for processing.
                                                                                                //	ResizeBicubic filterResize = new ResizeBicubic(xSize, ySize);		// The class implements image resizing filter using bicubic interpolation algorithm. 
                                                                                                // It uses bicubic kernel W(x) as described on Wikipedia (coefficient a is set to -0.5). 
                                                                                                // The filter accepts 8 grayscale images and 24 bpp color images for processing.
                                                                                                //	ResizeBilinear filterResize = new ResizeBilinear(xSize, ySize);		// The class implements image resizing filter using bilinear interpolation algorithm. 
                                                                                                // The filter accepts 8 grayscale images and 24/32 bpp color images for processing.

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
                        adjustedImage = ImgDither(adjustedImage);
                    else
                        adjustedImage = ImgGrayscale(adjustedImage);

                    adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB

                }
                if (CbBlackWhiteEnable.Checked)
                {
                    //     Grayscale filterGray = new Grayscale(0.333, 0.333, 0.333);  // convert to real grayscale
                    //     Bitmap grayImage = filterGray.Apply(adjustedImage);
                    Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(adjustedImage);

                    Threshold filterBW = new Threshold((int)tBarBWThreshold.Value); // 0 to 255
                    filterBW.ApplyInPlace(grayImage);   // The filter accepts 8 and 16 bpp grayscale images for processing.

                    GrayscaleToRGB filterRGB = new GrayscaleToRGB();
                    adjustedImage = filterRGB.Apply(grayImage);
                }

                if (_useColorMode)
                    CountResultColors();            // get all applied toolTable-colors and its count
                else
                    CountResultColorsGray();        // get all applied gray-colors and its count

                /*********************
				***** COLOR MODE *****
				**********************/
                if (_useColorMode && cBReduceColorsToolTable.Checked)		// for Color Mode
                {
                    ToolTable.SetAllSelected(true);     //  enable all tools
                    List<Color> myPalette = new List<Color>();
                    ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());

                    if (redoColorAdjust)        // CbExceptColor_CheckedChanged
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

                        if (applyPercentLimit)
                        {
                            applyPercentLimit = false;
                            for (int i = 0; i < toolTableCount; i++)
                            {
                                toolPixelCount = ToolTable.PixelCount(i);
                                percent = (toolPixelCount * 100 / tmpCount);
                                if (percent >= nUDColorPercent.Value)
                                { matchLimit++; }// tmp += toolTable.getName() + "  " + (toolTable.pixelCount(i) * 100 / tmpCount) + "\r\n"; }
                                else
                                { ToolTable.SetPresent(false); }    // below limit, disable color / tool-nr
                            }
                            if (matchLimit < nUDMaxColors.Minimum) { matchLimit = (int)nUDMaxColors.Maximum; }
                            if (matchLimit > nUDMaxColors.Maximum) { matchLimit = (int)nUDMaxColors.Maximum; }
                            //	nUDMaxColors.ValueChanged -= ApplyColorCorrectionsEvent;    // set new value without generating an event
                            nUDMaxColors.Value = matchLimit;
                            //	nUDMaxColors.ValueChanged += ApplyColorCorrectionsEvent;
                        }
                    }

                    if (cbExceptColor.Checked)
                        myPalette.Add(cbExceptColor.BackColor);
                    ToolTable.SortByPixelCount(false);                  // fill palette with colors in order of occurence
                    ToolTable.SetAllSelected(false);                    // 

                    for (int i = 0; i < (int)nUDMaxColors.Value; i++)   // add colors to AForge filter
                    {
                        ToolTable.SetIndex(i);
                        if ((ToolTable.IndexToolNR() >= 0) && ToolTable.IndexUse())
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
                /*************************
				***** Grayscale MODE *****
				**************************/
                else
                if (!_useColorMode && cBReduceColorsToolTable.Checked)		// for Grayscale Mode!!!
                {
                    List<Color> myPalette = new List<Color>();
                    ColorImageQuantizer ciq = new ColorImageQuantizer(new MedianCutQuantizer());

                    if (redoColorAdjust)        // CbExceptColor_CheckedChanged
                    {
                        redoColorAdjust = false;

                        int matchLimit = 0;
                        int tmpCount = pixelCount;                // keep original counter 
                        if (tmpCount < 1) { tmpCount = 1; }

                        long toolPixelCount;
                        decimal percent;

                        if (applyPercentLimit)
                        {
                            applyPercentLimit = false;
                            for (int i = 0; i < GrayValueMap.Count; i++)        //	preset CheckListBox in ListAvailableTools
                            {
                                toolPixelCount = GrayValueMap[i].Count;
                                percent = (toolPixelCount * 100 / tmpCount);
                                if (percent >= nUDColorPercent.Value)
                                { matchLimit++; GrayValueMap[i].Use = true; }
                                else
                                { GrayValueMap[i].Use = false; }
                            }
                            if (matchLimit < nUDMaxColors.Minimum) { matchLimit = (int)nUDMaxColors.Maximum; }
                            if (matchLimit > nUDMaxColors.Maximum) { matchLimit = (int)nUDMaxColors.Maximum; }
                            //	nUDMaxColors.ValueChanged -= ApplyColorCorrectionsEvent;    // set new value without generating an event
                            nUDMaxColors.Value = matchLimit;
                            //	nUDMaxColors.ValueChanged += ApplyColorCorrectionsEvent;
                        }
                    }

                    if (cbExceptColor.Checked)
                        myPalette.Add(cbExceptColor.BackColor);
                    SortByGrayAmount(true);
                    int col;
                    int maxIndex = Math.Min((int)nUDMaxColors.Value, GrayValueMap.Count - 1);
                    for (int i = 0; i < maxIndex; i++)   // add colors to AForge filter
                    {
                        if (GrayValueMap[i].Use)
                        {
                            col = GrayValueMap[i].GrayVal;
                            myPalette.Add(Color.FromArgb(255, col, col, col));	//ToolTable.IndexColor());
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

                adjustedImage = AForge.Imaging.Image.Clone(adjustedImage, originalImage.PixelFormat); //Format32bppARGB
                if (!cBPreview.Checked)
                    pictureBox1.Image = adjustedImage;

                resultImage = new Bitmap(adjustedImage);
                lblStatus.Text = "Done";
                lblImageSource.Text = "modified";
                Refresh();
            }
            catch (Exception err)
            {
                Logger.Error(err, "applyColorCorrections ");
                MessageBox.Show("Error resizing/balancing image: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ShowResultImage();  // if checked, show final result with tool colors

            /**********************/
            EnableControlEvents();

            Cursor.Current = Cursors.Default;
        }

        private void ShowResultImage(bool showResult = true)//, bool preview = false)
        {
            if (logEnable) Logger.Trace("ShowResultImage showResult:{0}", showResult);

            if (adjustedImage == null) {Logger.Warn("ShowResultImage adjustedImage == null");return;}//if no image, do nothing

            if (useColorMode)
                GenerateResultImage(ref resultToolNrArray);      // fill countColors
            else
                GenerateResultImageGray(ref resultToolNrArray);

            if (showResult)
            {
                //UpdateToolList();
                pictureBox1.Image = resultImage;
                lblImageSource.Text = "result";
            }
            Refresh();
        }

        /// <summary>
        /// Count usage of tool-colors
        /// </summary>
        private void CountResultColors()    	// update pixelCounts for specific tool-colors in ToolTable
        {
            Color myColor;
            if (cbExceptColor.Checked)
                ToolTable.SetExceptionColor(cbExceptColor.BackColor);
            else
                ToolTable.ClrExceptionColor();
            ToolTable.Clear();                  // ColorPresent = false; ToolSelected = true; PixelCount = 0; Diff = int.MaxValue;
            int mode = conversionMode;
            BitmapData dataAdjusted = null;
            sbyte myToolNr;//, myIndex;			// also -1 is possible
            Dictionary<Color, sbyte> lookUpToolNr = new Dictionary<Color, sbyte>();
            try
            {
                Rectangle rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
                dataAdjusted = adjustedImage.LockBits(rect, ImageLockMode.ReadOnly, adjustedImage.PixelFormat);

                pixelCount = adjustedImage.Width * adjustedImage.Height;

                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                int psize = 4;  // 32bppARGB GetPixelInfoSize(adjustedImage.PixelFormat);
                long bsize = dataAdjusted.Stride * adjustedImage.Height;
                byte[] pixelsAdjusted = new byte[bsize];
                Marshal.Copy(ptrAdjusted, pixelsAdjusted, 0, pixelsAdjusted.Length);
                byte r, g, b, a;
                for (long index = 0; index < pixelsAdjusted.Length; index += psize)
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
            finally
            {
                adjustedImage?.UnlockBits(dataAdjusted);
            }
            if (logEnable) Logger.Info("countResultColors - different colors: {0}", lookUpToolNr.Count);
        }


        private void CountResultColorsGray()    // update pixelCounts for specific gray values in GrayValueMap
        {
            BitmapData dataAdjusted = null;

            SortByGrayValue(false);
            foreach (GrayProp tmp in GrayValueMap)      // clear list
            { tmp.Count = 0; tmp.Use = false; }

            Dictionary<byte, long> lookUpGrayVal = new Dictionary<byte, long>();

            try
            {
                Rectangle rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
                dataAdjusted = adjustedImage.LockBits(rect, ImageLockMode.ReadOnly, adjustedImage.PixelFormat);

                pixelCount = adjustedImage.Width * adjustedImage.Height;

                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                int psize = 4;  // 32bppARGB GetPixelInfoSize(adjustedImage.PixelFormat);
                long bsize = dataAdjusted.Stride * adjustedImage.Height;
                byte[] pixelsAdjusted = new byte[bsize];
                Marshal.Copy(ptrAdjusted, pixelsAdjusted, 0, pixelsAdjusted.Length);
                byte b;
                for (long index = 0; index < pixelsAdjusted.Length; index += psize)
                {
                    b = pixelsAdjusted[index]; //g = pixelsAdjusted[index + 1]; r = pixelsAdjusted[index + 2]; a = pixelsAdjusted[index + 3];
                    {
                        if (lookUpGrayVal.TryGetValue(b, out long amount))                // myColor already registered
                        {
                            lookUpGrayVal[b]++;
                        }
                        else
                        {
                            lookUpGrayVal.Add(b, 1);
                        }
                    }
                }
            }
            finally
            {
                adjustedImage?.UnlockBits(dataAdjusted);
            }

            foreach (KeyValuePair<byte, long> gray in lookUpGrayVal)
            {
                GrayValueMap[gray.Key].Count = gray.Value;
                GrayValueMap[gray.Key].Use = true;
            }
            if (logEnable) Logger.Info("countResultColorsGray - different grays: {0}", lookUpGrayVal.Count);
        }


        /// <summary>
        /// Generate result image and fill resultToolNrArray
        /// </summary>
        private void GenerateResultImage(ref short[,] tmpToolNrArray)      // and count tool colors
        {//https://www.codeproject.com/Articles/17162/Fast-Color-Depth-Change-for-Bitmaps

            if (adjustedImage == null) {Logger.Warn("GenerateResultImage adjustedImage == null");return;}//if no image, do nothing
            if (logEnable) Logger.Trace("GenerateResultImage ");

            Color myColor, newColor;
            if (cbExceptColor.Checked)
                ToolTable.SetExceptionColor(cbExceptColor.BackColor);
            else
                ToolTable.ClrExceptionColor();

            int mode = conversionMode;
            BitmapData dataAdjusted = null;
            BitmapData dataResult = null;
            lblStatus.Text = "Generate result image...";

            Dictionary<Color, sbyte> lookUpToolNr = new Dictionary<Color, sbyte>();
            int rectWidth = -1, rectHeight = -1;
            try
            {
                rectWidth = Math.Min(adjustedImage.Width, resultImage.Width);
                rectHeight = Math.Min(adjustedImage.Height, resultImage.Height);
                Rectangle rectAdjusted = new Rectangle(0, 0, rectWidth, rectHeight);	//adjustedImage.Width, adjustedImage.Height);
                Rectangle rectResult = new Rectangle(0, 0, rectWidth, rectHeight);      //resultImage.Width, resultImage.Height);

                Logger.Trace("GenerateResultImage: size:{0} x {1}  bits:{2}", rectWidth, rectHeight, Image.GetPixelFormatSize(adjustedImage.PixelFormat));

                dataAdjusted = adjustedImage.LockBits(rectAdjusted, ImageLockMode.ReadOnly, adjustedImage.PixelFormat);
                dataResult = resultImage.LockBits(rectResult, ImageLockMode.WriteOnly, adjustedImage.PixelFormat);

                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                IntPtr ptrResult = dataResult.Scan0;
                int psize = 4;  // 32bppARGB GetPixelInfoSize(adjustedImage.PixelFormat);
                long bsize = dataAdjusted.Stride * rectHeight;	//adjustedImage.Height;
                byte[] pixelsAdjusted = new byte[bsize];
                byte[] pixelsResult = new byte[bsize];
                tmpToolNrArray = new short[rectWidth, rectHeight];	//adjustedImage.Width, adjustedImage.Height];
                Marshal.Copy(ptrAdjusted, pixelsAdjusted, 0, pixelsAdjusted.Length);

                byte r, g, b, a;
                int bx = 0, by = 0;
                for (long index = 0; index < pixelsAdjusted.Length; index += psize)
                {
                    b = pixelsAdjusted[index];      // https://stackoverflow.com/questions/8104461/pixelformat-format32bppargb-seems-to-have-wrong-byte-order
                    g = pixelsAdjusted[index + 1];
                    r = pixelsAdjusted[index + 2];
                    a = pixelsAdjusted[index + 3];

                    /***** index current color *****/
                    myColor = Color.FromArgb(a, r, g, b);
                    if (lookUpToolNr.TryGetValue(myColor, out sbyte myToolNr))
                    {        // color already indexed
                        ToolTable.SetIndex(ToolTable.GetIndexByToolNR(myToolNr));       // set as usable
                    }
                    else
                    {
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
                resultImage?.UnlockBits(dataResult);
                adjustedImage?.UnlockBits(dataAdjusted);
            }
            catch (Exception err)
            {
                string errString = string.Format("GenerateResultImage: size:{0} x {1}  bits:{2}", rectWidth, rectHeight, Image.GetPixelFormatSize(adjustedImage.PixelFormat));
                Logger.Error(err, "{0}  ", errString);
                EventCollector.StoreException(errString + "  " + err.Message);
                resultImage?.UnlockBits(dataResult);
                adjustedImage?.UnlockBits(dataAdjusted);
            }
            lblStatus.Text = "Done";
            pictureBox1.Image = resultImage;
            lblImageSource.Text = "result";
            Refresh();
        }

        private void GenerateResultImageGray(ref short[,] tmpToolNrArray)      // and count tool colors
        {//https://www.codeproject.com/Articles/17162/Fast-Color-Depth-Change-for-Bitmaps

            if (adjustedImage == null) {Logger.Warn("GenerateResultImageGray adjustedImage == null");return;}//if no image, do nothing
            if (logEnable) Logger.Trace("GenerateResultImageGray pixelFormat:{0}", adjustedImage.PixelFormat);

            BitmapData dataAdjusted = null;
            BitmapData dataResult = null;
            lblStatus.Text = "Generate result image...";

            SortByGrayValue(false);
            int rectWidth = -1, rectHeight = -1;
            try
            {
                rectWidth = Math.Min(adjustedImage.Width, resultImage.Width);
                rectHeight = Math.Min(adjustedImage.Height, resultImage.Height);
                Rectangle rectAdjusted = new Rectangle(0, 0, rectWidth, rectHeight);	//adjustedImage.Width, adjustedImage.Height);
                Rectangle rectResult = new Rectangle(0, 0, rectWidth, rectHeight);  //resultImage.Width, resultImage.Height);

                dataAdjusted = adjustedImage.LockBits(rectAdjusted, ImageLockMode.ReadOnly, adjustedImage.PixelFormat);
                dataResult = resultImage.LockBits(rectResult, ImageLockMode.WriteOnly, adjustedImage.PixelFormat);

                IntPtr ptrAdjusted = dataAdjusted.Scan0;
                IntPtr ptrResult = dataResult.Scan0;
                int psize = 4;  // 32bppARGB GetPixelInfoSize(adjustedImage.PixelFormat);
                long bsize = dataAdjusted.Stride * rectHeight;	//adjustedImage.Height;
                byte[] pixelsAdjusted = new byte[bsize];
                byte[] pixelsResult = new byte[bsize];
                tmpToolNrArray = new short[rectWidth, rectHeight];	//adjustedImage.Width, adjustedImage.Height];
                Marshal.Copy(ptrAdjusted, pixelsAdjusted, 0, pixelsAdjusted.Length);

                byte b;
                int bx = 0, by = 0;
                byte background = 255;
                for (long index = 0; index < pixelsAdjusted.Length; index += psize)
                {
                    b = pixelsAdjusted[index];      // https://stackoverflow.com/questions/8104461/pixelformat-format32bppargb-seems-to-have-wrong-byte-order

                    if (!GrayValueMap[b].Use)
                        b = background;                     // set white;

                    tmpToolNrArray[bx++, by] = b;	//myToolNr;

                    if (bx >= rectWidth)	//adjustedImage.Width)
                    { bx = 0; by++; }
                    // apply new color
                    pixelsResult[index] = b;// newColor.A;
                    pixelsResult[index + 1] = b;
                    pixelsResult[index + 2] = b;
                    pixelsResult[index + 3] = 255;
                }
                Marshal.Copy(pixelsResult, 0, ptrResult, pixelsResult.Length);
                resultImage?.UnlockBits(dataResult);
                adjustedImage?.UnlockBits(dataAdjusted);
            }
            catch (Exception err)
            {
                string errString = string.Format("GenerateResultImageGray: size:{0} x {1}  bits:{2}", rectWidth, rectHeight, Image.GetPixelFormatSize(adjustedImage.PixelFormat));
                Logger.Error(err, "{0}  ", errString);
                EventCollector.StoreException(errString + "  " + err.Message);

                //	Logger.Error(err,"generateResultImage pixelFormat:{0} ", adjustedImage.PixelFormat);
                //    EventCollector.StoreException("generateResultImageGray: size:"+rectWidth+" x "+rectHeight+"  pixelFormat: " + adjustedImage.PixelFormat.ToString() + "  " +err.Message);
                resultImage?.UnlockBits(dataResult);
                adjustedImage?.UnlockBits(dataAdjusted);
            }
            lblStatus.Text = "Done";
            pictureBox1.Image = resultImage;
            lblImageSource.Text = "result";
            Refresh();
        }



        private bool updateLabelColorCount = false;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (updateLabelColorCount)
            {
                if (logEnable) Logger.Trace("Timer1_Tick call CountImageColors");
                {
                    imageColors = CountImageColors();   // get amount of different colors in adjustedImage
                                                        //   if (imageColors < toolTableCount)
                    { nUDMaxColors.Maximum = imageColors; nUDMaxColors.Value = imageColors; }
                }
            }
        }
        /// <summary>
        /// Count amount of different colors in adjusted image
        /// </summary>
        private int CountImageColors()
        {   // Lock the bitmap's bits.  
            if (adjustedImage == null) {Logger.Warn("CountImageColors adjustedImage == null");return 1;}//if no image, do nothing
            Rectangle rect;
            try
            { 
                rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
            }
            catch (Exception err)
            {
                EventCollector.StoreException("CountImageColors1: size:" + adjustedImage.Width + " x " + adjustedImage.Height + " " + err);
                Logger.Error(err, "CountImageColors ");
                MessageBox.Show("Error count image colors - width or height not ok?:\r\n" + err.Message, "Error");
                return 1;
            }

            BitmapData bmpData = adjustedImage.LockBits(rect, ImageLockMode.ReadWrite, adjustedImage.PixelFormat);
            IntPtr ptr = bmpData.Scan0;                         // Get the address of the first line.
            long bytes = bmpData.Stride * adjustedImage.Height;  // Declare an array to hold the bytes of the bitmap.
            byte[] rgbValues = new byte[bytes];
            byte r, g, b, a;
            Marshal.Copy(ptr, rgbValues, 0, (int)bytes);             // Copy the RGB values into the array.
                                                                     //      int count = 0;
            long stride = bmpData.Stride;
            var differentColor = new HashSet<System.Drawing.Color>();          // count each color once
            try
            {
                for (long column = 0; column < bmpData.Height; column++)
                {
                    for (long row = 0; row < bmpData.Width; row++)
                    {
                        b = (byte)(rgbValues[(column * stride) + (row * 4) + 0]);  // https://stackoverflow.com/questions/8104461/pixelformat-format32bppargb-seems-to-have-wrong-byte-order
                        g = (byte)(rgbValues[(column * stride) + (row * 4) + 1]);
                        r = (byte)(rgbValues[(column * stride) + (row * 4) + 2]);
                        a = (byte)(rgbValues[(column * stride) + (row * 4) + 3]);
                        differentColor.Add(Color.FromArgb(a, r, g, b));
                        //            count++;
                    }
                }
            }
            catch (Exception err)
            {
                string errString = string.Format("CountImageColors2: size:{0} x {1}  bits:{2}", adjustedImage.Width, adjustedImage.Height, Image.GetPixelFormatSize(adjustedImage.PixelFormat));
                Logger.Error(err, "{0}  ", errString);
                EventCollector.StoreException(errString + "  " + err.Message);

                //    EventCollector.StoreException("CountImageColors2: size:"+adjustedImage.Width+" x "+adjustedImage.Height+" " +err);
                //    Logger.Error(err, "CountImageColors ");
                //    MessageBox.Show("Error count image colors - width or height not ok?:\r\n" + err.Message, "Error");
                return 1;
            }
            finally
            { adjustedImage.UnlockBits(bmpData); }

            lblColors.Text = "Number of colors: " + differentColor.Count.ToString();
            if (logEnable) Logger.Trace("CountImageColors Count:{0}   update:{1}", differentColor.Count, updateLabelColorCount);
            updateLabelColorCount = false;
            return differentColor.Count;
        }

        //Quick preview of the original image. Todo: use a new image container for fas return to processed image
        private void BtnCheckOrig_MouseDown(object sender, MouseEventArgs e)
        { ShowResultImage(); }
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

        /* update textbox */

        /*****************************************************************
        ********************* Generate GCode *****************************
        ******************************************************************/
        public void BtnGenerateClick(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (useColorMode)
                GenerateResultImage(ref resultToolNrArray);      // fill countColors
            else
                GenerateResultImageGray(ref resultToolNrArray);

            if (useColorMode)	//tabControl2.SelectedIndex == 0)     // Color mode
            {
                Logger.Info("▼▼▼▼ Generate GCode in color mode");
                cBPreview.Checked = true;
                GenerateGCodeHorizontal(true);	    // with or without tool-table
            }
            else
            {
                Logger.Info("▼▼▼▼ Generate GCode in grayscale mode mode - {0}", (RbGrayscaleVector.Checked ? "vectorize" : "height data"));
                if (RbGrayscaleVector.Checked)
                    GenerateGCodeHorizontal(false);	// with or without tool-table
                else
                    GenerateHeightData();
            }
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
                    if ((0 <= e.X) && (e.X < pictureBox1.ClientSize.Width) && (0 <= e.Y) && (e.Y < pictureBox1.ClientSize.Height))
                        clr = bmp.GetPixel(e.X, e.Y);
                    bmp.Dispose();
                }
                if (e.Button == MouseButtons.Left)
                {
                    if (tabPageSize.Visible)
                    {
                        decimal rX = (nUDWidth.Value / nUDHeight.Value) / ((decimal)pictureBox1.ClientSize.Width / (decimal)pictureBox1.ClientSize.Height);
                        decimal rY = 1 / rX;
                        if (rX > 1) { rY = 1; } else { rX = 1; }
                        decimal picBoxX = (decimal)e.X * rY / (decimal)pictureBox1.ClientSize.Width - (rY - 1) / 2;    // (decimal)pictureBox1.ClientSize.Width;
                        decimal picBoxY = (decimal)e.Y * rX / (decimal)pictureBox1.ClientSize.Height - (rX - 1) / 2;   // (decimal)pictureBox1.ClientSize.Height;
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
                        if (cbExceptColor.Checked)
                            LblExceptionValue.Text = HexConverter(clr);//.ToString();// + " = " + ToolTable.GetToolName(i);		
                        cbExceptColor.BackColor = clr;
                        ShowResultImage();
                    }
                }
                float zoom = (float)nUDWidth.Value / pictureBox1.Width;
                toolTip1.SetToolTip(pictureBox1, (e.X * zoom).ToString("0.##") + "  " + (e.Y * zoom).ToString("0.##") + "   " + clr.ToString());
                oldPoint = e.Location;
            }
        }
        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
        private void CbExceptColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbExceptColor.Checked)
            {
                ToolTable.SetExceptionColor(cbExceptColor.BackColor);
                LblExceptionValue.Text = HexConverter(cbExceptColor.BackColor);//.ToString();		
            }
            else
            {
                ToolTable.ClrExceptionColor();
                LblExceptionValue.Text = "";
            }
            redoColorAdjust = true;
            ApplyColorCorrections("CbExceptColor_CheckedChanged");
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
            ApplyColorCorrections("RbEngravingPattern2_CheckedChanged");
        }

        /// <summary>
        /// if 'Draw outline' is unchecked, disable smoothing
        /// </summary>
        private void CbGCodeOutline_CheckedChanged(object sender, EventArgs e)
        {
            cBGCodeOutlineSmooth.Enabled = cBGCodeOutline.Checked;
            nUDGCodeOutlineSmooth.Enabled = cBGCodeOutline.Checked;
            cBGCodeOutlineShrink.Enabled = cBGCodeOutline.Checked;
            ApplyColorCorrections("CbGCodeOutline_CheckedChanged");
        }

        /// <summary>
        /// Grayscale checked
        /// </summary>
        private void TabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            useColorMode = (tabControl2.SelectedIndex == 0);
            GbConversionWizzard.Enabled = GbColorReduction.Enabled = GbColorReplacingMode.Enabled = GbToolEnable.Enabled = useColorMode;

            if (logEnable) Logger.Trace("TabControl2_SelectedIndexChanged  ColorMode:{0}", useColorMode);

            DisableControlEvents();
            {
                if (!useColorMode)      // Grayscale
                {
                    cbGrayscale.Checked = true; cbGrayscale.Enabled = false;        // set and tight
                    cbGrayscale.BackColor = Color.Yellow;
                //    GbGcodeDirection.Enabled = true;                                // GroupBox direction / patttern
                    Properties.Settings.Default.importImageColorMode = false;
                    GbGrayscaleProcess.Enabled = true;
                    GbGrayscaleProcess.BackColor = Color.Yellow;
                    if (RbGrayscaleVector.Checked)
                    {
                        gBgcodeSelection.BackColor = Color.Yellow;
                //        GbGcodeDirection.BackColor = Color.WhiteSmoke;
                    }
                    else
                    {
                        gBgcodeSelection.BackColor = Color.WhiteSmoke;
                //        GbGcodeDirection.BackColor = Color.Yellow;
                    }
                    LblMode.Text = "Grayscale mode";
                    LblMode.BackColor = Color.Yellow;
                }
                else
                {
                    cbGrayscale.Checked = false; cbGrayscale.Enabled = true;
                    cbGrayscale.BackColor = Color.Transparent;
                //    GbGcodeDirection.Enabled = false;                               // GroupBox direction / patttern
                    Properties.Settings.Default.importImageColorMode = true;
                    GbGrayscaleProcess.Enabled = false;
                    GbGrayscaleProcess.BackColor = Color.WhiteSmoke;
                    gBgcodeSelection.BackColor = Color.Yellow;
                //    GbGcodeDirection.BackColor = Color.WhiteSmoke;
                    LblMode.Text = "Color mode";
                    LblMode.BackColor = Color.Yellow;

                    //	if (sender == null)
                    { toolTableCount = ToolTable.Init(" (Gcode from image)"); }
                }
                cBPreview.Checked = useColorMode;
            }
            EnableControlEvents();

            Highlight();

            if (sender != null)
            {
                ResetColorCorrectionControls(); ApplyColorCorrections("TabControl2"); lblImageSource.Text = "original";
            }
        }

        private void TabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            RbGrayscalePattern.CheckedChanged -= RbGrayscaleVector_CheckedChanged;
            RbGrayscaleVector.CheckedChanged -= RbGrayscaleVector_CheckedChanged;
            if (tabControl3.SelectedIndex == 0)
                RbGrayscalePattern.Checked = true;
            else
                RbGrayscaleVector.Checked = true;
            RbGrayscaleVector.CheckedChanged += RbGrayscaleVector_CheckedChanged;
            RbGrayscalePattern.CheckedChanged += RbGrayscaleVector_CheckedChanged;
        }


        /* Load useCase file */
        public string LaserModeReturnValue { get; set; }
        private void BtnLoadUseCase_Click(object sender, EventArgs e)
        {
            LaserModeReturnValue = "";
            if (string.IsNullOrEmpty(lBUseCase.Text))
                return;
            string path = Datapath.Usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            Logger.Trace("Load use case: '{0}'", path);
            MyIni.ReadAll();    // ReadImport();
         //   UpdateIniVariables();
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
                        LaserModeReturnValue = "$32=0 (laser mode off)";
                }

                if ((lasermode < 1) && laseruse)
                {
                    DialogResult dialogResult = MessageBox.Show("grbl laser mode ($32) is not activated, \r\nbut recommended if a laser will be used\r\n\r\n Press 'Yes' to fix this", "Attention", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        LaserModeReturnValue = "$32=1 (laser mode on)";
                }
            }
            UpdateToolTableList();              // show tool-files and last loaded tools
            GetToolTableSettings();             // get max tools from tooltable or 255 in grayscale mode
        }

        private void BtnGetPWMValues_Click(object sender, EventArgs e)
        {
            nUDSTop.Value = Properties.Settings.Default.importGCPWMZero;
            nUDSBottom.Value = Properties.Settings.Default.importGCPWMDown;
        }

        private void Highlight()
        {
            if (RbStartGrayZ.Checked)
            { RbStartGrayZ.BackColor = GbStartGrayZ.BackColor = Color.Yellow; }
            else
            { RbStartGrayZ.BackColor = GbStartGrayZ.BackColor = Color.WhiteSmoke; }

            if (RbStartGrayS.Checked)
            { RbStartGrayS.BackColor = GbStartGrayS.BackColor = Color.Yellow; }
            else
            { RbStartGrayS.BackColor = GbStartGrayS.BackColor = Color.WhiteSmoke; }

            if (RbEngravingLine.Checked)
            { RbEngravingLine.BackColor = GbEngravingLine.BackColor = Color.Yellow; }
            else 
            { RbEngravingLine.BackColor = GbEngravingLine.BackColor = Color.WhiteSmoke; }

            if (RbEngravingPattern.Checked)
            { RbEngravingPattern.BackColor = GbEngravingPattern.BackColor = Color.Yellow; }
            else
            { RbEngravingPattern.BackColor = GbEngravingPattern.BackColor = Color.WhiteSmoke; }
        }

        private void RbGrayZ_CheckedChanged(object sender, EventArgs e)
        {
            Highlight();
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string url = "https://grbl-plotter.de/index.php?";
            try
            {
                Button clickedLink = sender as Button;
                Process.Start(url + clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "BtnHelp_Click ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }

        private void RbEngravingLine_CheckedChanged(object sender, EventArgs e)
        {
            Highlight();
        }

        private void BtnReloadPattern_Click(object sender, EventArgs e)
        {

        }


    }


}
