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
 * 2019-08-15 add logger
 * 2019-09-07 use plotter class
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2020-07-05 use new Graphic class
 * 2020-12-09 line 127 no return of Gcode, must be picked up at Graphic.GCode
 * 2021-02-24 adapations for SVG-Font files
 * 2021-07-26 code clean up / code quality
 * 2021-09-10 add radioButtons to select line alignment: left, center, right line 168
 * 2022-02-28 check if a font is selected before creating text
 * 2022-03-04 check max in NudFontSize_ValueChanged
 * 2022-09-30 line 155 disable ApplyHatchFill (from SVG import)
 * 2022-12-30 add win system font choice, word wrap
 * 2023-01-06 ShowTextSize - check if (textFont.Size != null)
 * 2023-01-10 check font before 1st use
 * 2023-01-18 tBText.Invalidate after rezising the form
 * 2023-01-26 BtnSelectFont_Click add try/catch for tBText.Font = fontDialog1.Font;	// probably the cause of "Only TrueType fonts are supported. This is not a TrueType font."
 * 2023-01-29 check font size after width/heigth calc
 * 2023-11-02 l:139 f:FillFontSelector check if index is in range
 * 2023-11-24 l:414 f:BtnSelectFont_Click  add try/catch on fontDialog1.ShowDialog - Problem: if a newly installed font was selected, after closing the dialog with 'ok': 
																			"main Form ThreadException - Only TrueType fonts are supported. This is not a TrueType font."
 * 2024-02-06 add process automation
 * 2024-02-22 update proprties after loading ini-file
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


// http://imajeenyus.com/computer/20150110_single_line_fonts/
// Hershey code from: http://www.evilmadscientist.com/2011/hershey-text-an-inkscape-extension-for-engraving-fonts/
namespace GrblPlotter
{

    public partial class GCodeFromText : Form
    {
        private static Font textFont;
        private static Color textColor;
        private static Font initFont;
        private static Color initColor;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

#region load_close
        public GCodeFromText()
        {
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Logger.Info("++++++ GCodeFromText START ++++++");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        private void TextForm_Load(object sender, EventArgs e)
        {
            initFont = textFont = tBText.Font;
            initColor = textColor = tBText.ForeColor;

            FillFontSelector();

            Location = Properties.Settings.Default.locationTextForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            int toolCount = ToolTable.Init(" (TextForm_Load)");
            ToolProp tmpTool;
            bool defaultToolFound = false;
            for (int i = 0; i < toolCount; i++)
            {
                tmpTool = ToolTable.GetToolProperties(i);
                if (i == tmpTool.Toolnr)
                {
                    cBTool.Items.Add(i.ToString(culture) + ") " + tmpTool.Name);
                    if (i == Properties.Settings.Default.importGCToolDefNr)
                    {
                        cBTool.SelectedIndex = cBTool.Items.Count - 1;
                        defaultToolFound = true;
                    }
                }
            }
            if (!defaultToolFound)
            {
                cBTool.Items.Add("No tools found");
                cBTool.SelectedIndex = 0;
            }
            CBToolTable_CheckedChanged(sender, e);

            UpdateIniVariables();

            LoadPicture("svg");
        }

        private bool iniWasSet = false;
        public void UpdateIniVariables()
        {
            textFont = Properties.Settings.Default.createTextSystemFont;

            float newSize = Properties.Settings.Default.createTextSystemFontSize;

            if ((newSize > 0) && (newSize < Single.MaxValue))
            {
                textFont = new Font(textFont.Name, newSize, textFont.Style);//, GraphicsUnit.Millimeter);
                tBText.Font = textFont;
                ShowTextSize();
            }

            iniWasSet = true;

            LblInfoFont.Text = textFont.FontFamily.Name.ToString();
            textColor = ColorTranslator.FromHtml(Properties.Settings.Default.createTextFontColor);     //tBText.ForeColor;

            int tmp = Properties.Settings.Default.createTextAlignment;
            if (tmp == 1) { tBText.TextAlign = HorizontalAlignment.Left; }
            else if (tmp == 2) { tBText.TextAlign = HorizontalAlignment.Center; }
            else if (tmp == 3) { tBText.TextAlign = HorizontalAlignment.Right; }

            string tmpText = Properties.Settings.Default.createTextHersheyFontName;
            if ((tmpText == "") || (tmpText == "cBFont"))
                cBFont.SelectedIndex = 0;
            else
            {
                for (int i = 0; i < cBFont.Items.Count; i++)
                {
                    if (cBFont.Items[i].ToString() == tmpText)
                    {
                        cBFont.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (!Properties.Settings.Default.createTextHersheySelect)
            {   RbFont2.PerformClick();  }
            else
            {   RbFont1_CheckedChanged(null, null); }

            tBText.Text = Properties.Settings.Default.createTextFontText;
            tBText.Invalidate();
            ShowTextSize();
        }

        internal void SetText(string tmp, string opt, double size)
        {
            tBText.Text = tmp;
            /*	if (opt.ToLower().Contains("w"))
                {	if (size > 0) {	NUDWidth.Value = (decimal)size;}
                    BtnSetWidth.PerformClick();
                }
                if (opt.ToLower().Contains("h"))
                {	if (size > 0) {	NUDHeight.Value = (decimal)size;}
                    BtnSetHeight.PerformClick();
                }
            */
            btnApply.PerformClick();
        }

        private void FillFontSelector()
        {
            cBFont.Items.Clear();
            cBFont.Items.AddRange(GCodeFromFont.GetHersheyFontNames());
            cBFont.Items.AddRange(GCodeFromFont.FontFileName());

            string tmpText = Properties.Settings.Default.createTextHersheyFontName;
            if (tmpText == "")
                cBFont.SelectedIndex = 0;
            else
            {
                for (int i = 0; i < cBFont.Items.Count; i++)
                {
                    if (cBFont.Items[i].ToString() == tmpText)
                    {
                        cBFont.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void TextForm_FormClosing(object sender, FormClosingEventArgs e)
        {   SaveSettings();
            Logger.Trace("++++++ GCodeFromText STOP ++++++");
        }
#endregion
        private void SaveSettings()
        {
            Properties.Settings.Default.locationTextForm = Location;
            Properties.Settings.Default.createTextSystemFont = textFont;
			Properties.Settings.Default.createTextSystemFontSize = textFont.Size;
            Properties.Settings.Default.createTextFontColor = ColorTranslator.ToHtml(textColor);
            Properties.Settings.Default.Save();
        }

#region form_controls
        // get text, break it into chars, get path, etc... This event needs to be assigned in MainForm to poll text
        private void BtnApply_Click(object sender, EventArgs e)     // in MainForm:  _text_form.btnApply.Click += getGCodeFromText;
        { CreateText(); }

        public void CreateText()
        {
            VisuGCode.pathBackground.Reset();
            Graphic.CleanUp();
            Graphic.Init(Graphic.SourceType.Text, "", null, null);
            Graphic.graphicInformation.ApplyHatchFill = CbHatchFill.Checked;
            Graphic.graphicInformation.OptionNodesOnly = false;
            Graphic.graphicInformation.OptionCodeSortDistance = false;
            Graphic.graphicInformation.OptionZFromWidth = false;

            Graphic.graphicInformation.FigureEnable = true;
            Graphic.graphicInformation.GroupEnable = true;
            Graphic.graphicInformation.GroupOption = Graphic.GroupOption.ByType;

            Graphic.maxObjectCountBeforeReducingXML = 0;   // no limit

            Graphic.SetType("Text");

            if (CbOutline.Checked)
                Graphic.SetPenColor(tBText.ForeColor.ToKnownColor().ToString());
            else
                Graphic.SetPenColor("none");
            Graphic.SetPenFill(tBText.ForeColor.ToKnownColor().ToString());

            if (RbFont1.Checked)
            { /* "Get values from tool table" (importGCToolDefNr and importGCToolDefNrUse) will be processed in "Graphic2GCode.cs" */
                if (cBFont.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a font", "Error");
                    return;
                }
                Logger.Trace(culture, " createText()	");
                SaveSettings();
                GCodeFromFont.Reset();
                GCodeFromFont.GCText = tBText.Text;
                GCodeFromFont.GCFontName = cBFont.Text;// cBFont.Items[cBFont.SelectedIndex].ToString();
                Logger.Trace("cbfont via index: '{0}'  name: '{1}'", GCodeFromFont.GCFontName, cBFont.Text);


                GCodeFromFont.GCHeight = (double)nUDFontSize.Value;
                GCodeFromFont.GCFontDistance = (double)nUDFontDistance.Value;
                GCodeFromFont.GCLineDistance = (double)(nUDFontLine.Value / nUDFontSize.Value);
                GCodeFromFont.GCSpacing = (double)(nUDFontLine.Value / nUDFontSize.Value) / 1.5;
                GCodeFromFont.GCPauseChar = cBPauseChar.Checked;
                GCodeFromFont.GCPauseWord = cBPauseWord.Checked;
                GCodeFromFont.GCPauseLine = cBPauseLine.Checked;
                GCodeFromFont.GCConnectLetter = cBConnectLetter.Checked;

                if (Properties.Settings.Default.createTextHersheyLineBreakEnable)
                    GCodeFromFont.GetCode((double)nUDLineBreak.Value);      // do automatic page break
                else
                    GCodeFromFont.GetCode(0);   // no page break

                if (RbAlign2.Checked)
                    Graphic.AlignLines(1);      // 0=left, 1=center, 2=right
                else if (RbAlign3.Checked)
                    Graphic.AlignLines(2);		// 0=left, 1=center, 2=right
            }
            else
            {
                Graphic.SetFont(tBText.Font, cBPauseChar.Checked);      // CreateText
                Logger.Trace("CreateText  fontsize:{0}  size:{1}", textFont.Size, tBText.Font.Size);

                StringAlignment alignment = StringAlignment.Near;
                if (RbAlign2.Checked) { alignment = StringAlignment.Center; }
                else if (RbAlign3.Checked) { alignment = StringAlignment.Far; }

                Graphic.AddText(GetWrappedText(), alignment);
            }
            Logger.Trace("●●●● Create Text");
            Graphic.CreateGCode();      	// result is saved as stringbuilder in Graphic.GCode;
        }

        private string GetWrappedText()
        {
            string text = tBText.Text;
            int charPos;
            int lineLength;
            int maxCharPerLine = tBText.Text.Length;
            if (CbWordWrap.Enabled && (tBText.Lines.Count() > 0))
            {
                int lastLine = tBText.GetLineFromCharIndex(tBText.TextLength);
                text = "";
                maxCharPerLine = 0;
                int lastPos = 0;
                for (int i = 1; i <= lastLine; i++)
                {
                    charPos = tBText.GetFirstCharIndexFromLine(i);
                    lineLength = charPos - lastPos;
                    text += tBText.Text.Substring(lastPos, lineLength);
                    if (!char.IsControl(tBText.Text[charPos - 1])) { text += Environment.NewLine; }     // "\r\n"
                    lastPos = charPos;
                    maxCharPerLine = Math.Max(maxCharPerLine, lineLength);
                }
                text += tBText.Text.Substring(lastPos);
            }
            if (CbWordWrap.Checked)
            {
                decimal newVal = (maxCharPerLine * (nUDFontDistance.Value + nUDFontSize.Value * (decimal)1.1));
                newVal = Math.Max(newVal, (tBText.Width * (nUDFontSize.Value / (decimal)6.5)));
                if ((newVal > nUDLineBreak.Minimum) && (newVal < nUDLineBreak.Maximum))
                    nUDLineBreak.Value = newVal;
            }
            return text;
        }
        // adapt line distance depending on font size
        private void NudFontSize_ValueChanged(object sender, EventArgs e)
        {
            decimal tmp = nUDFontSize.Value * (decimal)1.5;
            if (tmp > nUDFontLine.Maximum)
                nUDFontLine.Maximum = tmp;
            nUDFontLine.Value = tmp;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        { this.Close(); }

        private void CBTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tmp = cBTool.SelectedItem.ToString();
            if (tmp.IndexOf(")") > 0)
            {
                int tnr = int.Parse(tmp.Substring(0, tmp.IndexOf(")")), culture);
                Properties.Settings.Default.importGCToolDefNr = tnr;
            }
        }

        private void GCodeFromText_Resize(object sender, EventArgs e)
        {
            tBText.Width = Width - (716 - 446);
            tBText.Height = Height - 280;
            btnApply.Left = Width - 151;
            btnApply.Top = Height - 88;
            CbInsertCode.Top = Height - 84;

            tabControl1.Width = Width - 17;
            panel1.Width = Width - 118;
            tabControl1.Height = Height - 36;
            panel1.Height = Height - 76;

            tBText.Invalidate();
            ShowTextSize();
        }

        private void CBToolTable_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = cBToolTable.Checked;
            label3.Enabled = enabled;
            cBTool.Enabled = enabled;
        }

        private void BtnLoadGraphic_Click(object sender, EventArgs e)
        {
            string s = (sender as Button).Text.ToLower(culture);
            LoadPicture(s);
        }
        private void LoadPicture(string name)
        {
            string fileName = Datapath.Fonts + "\\" + name + ".png";
            if (File.Exists(fileName))
            {
                pictureBox1.Size = panel1.Size;
                pictureBox1.Location = new Point();
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Load(fileName);
                image = Image.FromFile(fileName);
            }
        }

        private Image image;
        private Point mouseDown;
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse.Button == MouseButtons.Left)
            { mouseDown = mouse.Location; }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse.Button == MouseButtons.Left)
            {
                // Pan functions
                Point mousePosNow = mouse.Location;

                int deltaX = mousePosNow.X - mouseDown.X;
                int deltaY = mousePosNow.Y - mouseDown.Y;

                int newX = pictureBox1.Location.X + deltaX;
                int newY = pictureBox1.Location.Y + deltaY;

                pictureBox1.Location = new Point(newX, newY);
            }
        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            int newWidth = image.Width, newHeight = image.Height, newX = pictureBox1.Location.X, newY = pictureBox1.Location.Y;

            if (e.Delta > 0)
            {
                newWidth = pictureBox1.Size.Width + (pictureBox1.Size.Width / 10);
                newHeight = pictureBox1.Size.Height + (pictureBox1.Size.Height / 10);
                newX = pictureBox1.Location.X - ((pictureBox1.Size.Width / 10) / 2);
                newY = pictureBox1.Location.Y - ((pictureBox1.Size.Height / 10) / 2);
            }

            else if (e.Delta < 0)
            {
                newWidth = pictureBox1.Size.Width - (pictureBox1.Size.Width / 10);
                newHeight = pictureBox1.Size.Height - (pictureBox1.Size.Height / 10);
                newX = pictureBox1.Location.X + ((pictureBox1.Size.Width / 10) / 2);
                newY = pictureBox1.Location.Y + ((pictureBox1.Size.Height / 10) / 2);
            }
            pictureBox1.Size = new Size(newWidth, newHeight);
            pictureBox1.Location = new Point(newX, newY);
        }


        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;
            if (mouse.Button == MouseButtons.Left)
            {
                Point mousePosNow = mouse.Location;

                int deltaX = mousePosNow.X - mouseDown.X;
                int deltaY = mousePosNow.Y - mouseDown.Y;

                int newX = pictureBox1.Location.X + deltaX;
                int newY = pictureBox1.Location.Y + deltaY;

                pictureBox1.Location = new Point(newX, newY);
            }
        }
        #endregion

        private void BtnSelectFont_Click(object sender, EventArgs e)
        {
            RbFont2.Checked = true;

            fontDialog1.ShowColor = true;

            fontDialog1.Font = tBText.Font = textFont;
            fontDialog1.Color = tBText.ForeColor = textColor;

            try
            {   // if a newly installed font was selected, after closing the dialog with 'ok': "main Form ThreadException - Only TrueType fonts are supported. This is not a TrueType font."
                if (fontDialog1.ShowDialog() != DialogResult.Cancel)
                {
                    Logger.Info("BtnSelectFont_Click: Font:'{0}'", fontDialog1.Font.FontFamily.Name);
                    tBText.Font = fontDialog1.Font;
                    textFont = tBText.Font;
                    LblInfoFont.Text = textFont.FontFamily.Name.ToString();
                    textColor = tBText.ForeColor = fontDialog1.Color;
                    ShowTextSize();
                    SaveSettings();
                }
            }
            catch (Exception err)
            {
                Logger.Error(err, "BtnSelectFont_Click: Font:'{0}' ", fontDialog1.Font.FontFamily.Name);
                EventCollector.StoreException("SelFont " + err.Message);
                MessageBox.Show(Localization.GetString("textNewFontException") + "\r\n\r\nError from system:\r\n" + err.Message, Localization.GetString("mainAttention"));
                return;
            }
        }

        private void RbAlign1_CheckedChanged(object sender, EventArgs e)
        {
            int tmp = 0;
            if (RbAlign1.Checked) { tBText.TextAlign = HorizontalAlignment.Left; tmp = 1; }
            else if (RbAlign2.Checked) { tBText.TextAlign = HorizontalAlignment.Center; tmp = 2; }
            else if (RbAlign3.Checked) { tBText.TextAlign = HorizontalAlignment.Right; tmp = 3; }
            Properties.Settings.Default.createTextAlignment = tmp;
        }

        private void RbFont1_CheckedChanged(object sender, EventArgs e)
        {
            if (RbFont1.Checked)    // Hershey font
            {
                RbFont1.BackColor = GbFont1.BackColor = Color.Yellow;
                RbFont2.BackColor = GbFont2.BackColor = Color.Transparent;
                tBText.Font = initFont;
                tBText.ForeColor = initColor;
                cBPauseWord.Enabled = cBPauseLine.Enabled = cBConnectLetter.Enabled = true;
                CbOutline.Checked = true;
                CbOutline.Enabled = false;
            }
            else                   // system font
            {
                RbFont1.BackColor = GbFont1.BackColor = Color.Transparent;
                RbFont2.BackColor = GbFont2.BackColor = Color.Yellow;
                tBText.Font = textFont;
                tBText.ForeColor = textColor;
                cBPauseWord.Enabled = cBPauseLine.Enabled = cBConnectLetter.Enabled = false;
                if (CbHatchFill.Checked)
                {
                    CbOutline.Checked = true;
                    CbOutline.Enabled = true;
                }
            }
        }

        private void TbText_TextChanged(object sender, EventArgs e)
        {
            ShowTextSize();
        }
        private void ShowTextSize()
        {
            try
            {
                if ((textFont.Size != null) && (textFont.FontFamily != null))
                    Graphic.SetFont(textFont);
                else
                {
                    textFont = initFont;
                    Graphic.SetFont(textFont);
                }

                RectangleF b = Graphic.GetTextBounds(GetWrappedText(), StringAlignment.Near);
                LblInfoSize.Text = string.Format("{0} pt", textFont.Size);
			//	Properties.Settings.Default.createTextSystemFontSize = textFont.Size;

                LblInfoWidth.Text = string.Format("{0,9:0.00}", b.Width);
                LblInfoHeight.Text = string.Format("{0,9:0.00}", b.Height);
            }
            catch (Exception err)
            {
                Logger.Error(err, "ShowTextSize ");// textFont.Name:'{0}'  textFont.Size:'{1}' ", textFont.FontFamily.Name, textFont.Size);
            }
        }
        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LinkLabel clickedLink = sender as LinkLabel;
                Process.Start(clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "LinkLabel_LinkClicked ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }

        private void BtnSetWidth_Click(object sender, EventArgs e)
        {
            RectangleF b = Graphic.GetTextBounds(tBText.Text, StringAlignment.Near);
            float newSize = (float)NUDWidth.Value * textFont.Size / b.Width;
            if ((newSize > 0) && (newSize < Single.MaxValue))
            {
                textFont = new Font(textFont.Name, newSize, textFont.Style);//,GraphicsUnit.Millimeter);
                tBText.Font = textFont;
                ShowTextSize();
            }
            else
            { MessageBox.Show("Desired width causes invalid font size. Please choose other width.", "Error"); }
        }

        private void BtnSetHeight_Click(object sender, EventArgs e)
        {
            RectangleF b = Graphic.GetTextBounds(tBText.Text, StringAlignment.Near);
            float newSize = (float)NUDHeight.Value * textFont.Size / b.Height;
            if ((newSize > 0) && (newSize < Single.MaxValue))
            {
                textFont = new Font(textFont.Name, newSize, textFont.Style);//, GraphicsUnit.Millimeter);
                ShowTextSize();
            }
            else
            { MessageBox.Show("Desired heigth causes invalid font size. Please choose other heigth.", "Error"); }
        }

        private void CbWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            bool isWrapped = tBText.WordWrap = CbWordWrap.Checked;
            BtnSetWidth.Enabled = BtnSetHeight.Enabled = !isWrapped;
            NUDHeight.Enabled = NUDWidth.Enabled = !isWrapped;
            ShowTextSize();
        }

        private void CbHatchFill_CheckedChanged(object sender, EventArgs e)
        {
            CbOutline.CheckedChanged -= CbOutline_CheckedChanged;
            if (RbFont1.Checked)
            {
                CbOutline.Checked = true;
                CbOutline.Enabled = false;
            }
            else
            {
                CbOutline.Checked = true;
                CbOutline.Enabled = true;
            }
            CbOutline.CheckedChanged += CbOutline_CheckedChanged;
        }

        private void CbOutline_CheckedChanged(object sender, EventArgs e)
        {
            if (CbOutline.Enabled && !CbOutline.Checked)
            {
                CbHatchFill.CheckedChanged -= CbHatchFill_CheckedChanged;
                CbHatchFill.Checked = true;
                CbHatchFill.CheckedChanged += CbHatchFill_CheckedChanged;
            }
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

        private void BtnSaveIni_Click(object sender, EventArgs e)
        {
            SaveSettings();
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Machine Ini files (*.ini)|*.ini",
                    FileName = "CreateText_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ini"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var MyIni = new IniFile(sfd.FileName);
                    MyIni.WriteSection(IniFile.sectionText);	//"Create Text");    
                    Logger.Info("Save machine parameters as {0}", sfd.FileName);
                }
                sfd.Dispose();
            }
            catch (Exception err)
            {
                EventCollector.StoreException("BtnSaveIni_Click " + err.Message);
                Logger.Error(err, "BtnSaveIni_Click ");
                MessageBox.Show("SaveMachineParameters: \r\n" + err.Message, "Error");
            }
        }
GraphicsUnit units = GraphicsUnit.Point;
        private void TbText_FontChanged(object sender, EventArgs e)
        {
            Logger.Trace("tBText_FontChanged  iniWasSet:{0}", iniWasSet);
            if (iniWasSet)
            {
                float newSize = Properties.Settings.Default.createTextSystemFontSize;
                if ((newSize > 0) && (newSize < Single.MaxValue))
                {
                    textFont = new Font(textFont.Name, newSize, textFont.Style, GraphicsUnit.Millimeter);
                    tBText.Font = textFont;
                    ShowTextSize();
                }
            }

            iniWasSet = false;
        }
    }

    public partial class IniFile
    {
        internal static string[,] keyValueText = {
            {"Hershey Font",                "createTextHersheyFontName" },
            {"Hershey Letter Height",       "createTextHersheyFontSize" },
            {"Hershey Letter Distance",     "createTextHersheyLetterDistance"   },
            {"Hershey Line Distance",       "createTextHersheyLineDistance" },
            {"Hershey Line Break",          "createTextHersheyLineBreak"    },
            {"Hershey Line Break Enable",   "createTextHersheyLineBreakEnable"  },
            {"System Font",                 "createTextSystemFont"  },
            {"System Font Size",            "createTextSystemFontSize"  },
            {"System Font Set Size X",      "createTextSystemSizeX"  },
            {"System Font Set Size Y",      "createTextSystemSizeY"  },
            {"System Font Color",           "createTextFontColor"   },
            {"Font Use Hershey",            "createTextHersheySelect"   },
            {"Alignment",                   "createTextAlignment"   },
            {"Text",                        "createTextFontText"    },

            {"Hatch Fill Enable",           "importGraphicHatchFillEnable"},
            {"Hatch Fill Cross",            "importGraphicHatchFillCross"},
            {"Hatch Fill Distance",         "importGraphicHatchFillDistance"},
            {"Hatch Fill Angle",            "importGraphicHatchFillAngle"},
            {"Hatch Fill Angle Inc Enable", "importGraphicHatchFillAngleInc"},
            {"Hatch Fill Angle Inc",        "importGraphicHatchFillAngle2" },
            {"Hatch Fill Inset Enable",     "importGraphicHatchFillInsetEnable"},
            {"Hatch Fill Inset Enable2",    "importGraphicHatchFillInsetEnable2"},
            {"Hatch Fill Delete Path",      "importGraphicHatchFillDeletePath"},
            {"Hatch Fill Inset Distance",   "importGraphicHatchFillInset"},
            {"Hatch Fill Noise",            "importGraphicHatchFillNoise" },

            {"Noise Enable","importGraphicNoiseEnable"},
            {"Noise Amplitude","importGraphicNoiseAmplitude"},
            {"Noise Densitiy","importGraphicNoiseDensity"}
        };
        internal static string sectionText = "Create Text";
    }
}