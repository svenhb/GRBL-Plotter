/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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

            nUDFontSize.Value = Properties.Settings.Default.createTextFontSize;
            nUDFontLine.Value = Properties.Settings.Default.createTextLineDistance;
            nUDFontDistance.Value = Properties.Settings.Default.createTextFontDistance;
            nUDLineBreak.Value = Properties.Settings.Default.createTextLineBreak;

            initFont = tBText.Font;
            textFont = Properties.Settings.Default.createTextSystemFont;
            LblInfoFont.Text = textFont.FontFamily.Name.ToString();
            initColor = textColor = tBText.ForeColor;
			
			Logger.Info("TextForm_Load initFont:'{0}'", initFont);
			Logger.Info("TextForm_Load textFont:'{0}'", textFont);
			
			if ((textFont == null) || (textFont.Size == null))
			{
				Logger.Error("TextForm_Load font unknown '{0}'", textFont);
				textFont = initFont; 
			}
			
            ShowTextSize();
            RbFont1_CheckedChanged(sender, e);

            LoadPicture("svg");
        }

        private void FillFontSelector()
        {
            cBFont.Items.Clear();
            cBFont.Items.AddRange(GCodeFromFont.GetHersheyFontNames());
            cBFont.Items.AddRange(GCodeFromFont.FontFileName());

            int tmpIndex = Properties.Settings.Default.createTextFontIndex;
            if (tmpIndex < cBFont.Items.Count)
            { cBFont.SelectedIndex = Properties.Settings.Default.createTextFontIndex; }
        }

        private void TextForm_FormClosing(object sender, FormClosingEventArgs e)
        { SaveSettings(); }

        private void SaveSettings()
        {
            Properties.Settings.Default.createTextFontSize = nUDFontSize.Value;
            Properties.Settings.Default.createTextLineDistance = nUDFontLine.Value;
            Properties.Settings.Default.createTextFontDistance = nUDFontDistance.Value;
            Properties.Settings.Default.createTextLineBreak = nUDLineBreak.Value;

            Logger.Trace("++++++ GCodeFromText STOP ++++++");
            Properties.Settings.Default.createTextFontIndex = cBFont.SelectedIndex;
            Properties.Settings.Default.locationTextForm = Location;

            Properties.Settings.Default.createTextSystemFont = textFont;

            Properties.Settings.Default.Save();
        }

        // get text, break it into chars, get path, etc... This event needs to be assigned in MainForm to poll text
        private void BtnApply_Click(object sender, EventArgs e)     // in MainForm:  _text_form.btnApply.Click += getGCodeFromText;
        { CreateText(); }

        public void CreateText()
        {
            VisuGCode.pathBackground.Reset();
            Graphic.CleanUp();
            Graphic.Init(Graphic.SourceType.Text, "", null, null);
            Graphic.graphicInformation.ApplyHatchFill = CbHatchFill.Checked;    // false;			// no SVG import with fillColor "none"
            Graphic.graphicInformation.OptionNodesOnly = false;
            Graphic.graphicInformation.OptionSortCode = false;
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
                GCodeFromFont.GCFontName = cBFont.Items[cBFont.SelectedIndex].ToString();
                GCodeFromFont.GCHeight = (double)nUDFontSize.Value;
                GCodeFromFont.GCFontDistance = (double)nUDFontDistance.Value;
                GCodeFromFont.GCLineDistance = (double)(nUDFontLine.Value / nUDFontSize.Value);
                GCodeFromFont.GCSpacing = (double)(nUDFontLine.Value / nUDFontSize.Value) / 1.5;
                GCodeFromFont.GCPauseChar = cBPauseChar.Checked;
                GCodeFromFont.GCPauseWord = cBPauseWord.Checked;
                GCodeFromFont.GCPauseLine = cBPauseLine.Checked;
                GCodeFromFont.GCConnectLetter = cBConnectLetter.Checked;

                if (Properties.Settings.Default.createTextLineBreakEnable)
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
                Color c = tBText.ForeColor;
                //    Graphic.SetPenColor(c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"));

                /*    if (CbOutline.Checked)
                        Graphic.SetPenColor(tBText.ForeColor.ToKnownColor().ToString());
                    else
                        Graphic.SetPenColor("none");

                    Graphic.SetPenFill(tBText.ForeColor.ToKnownColor().ToString());
                */
                Graphic.SetFont(tBText.Font, cBPauseChar.Checked);      // CreateText

                StringAlignment alignment = StringAlignment.Near;
                if (RbAlign2.Checked) { alignment = StringAlignment.Center; }
                else if (RbAlign3.Checked) { alignment = StringAlignment.Far; }

                Graphic.AddText(GetWrappedText(), alignment);
            }

            Graphic.CreateGCode();      	// result is saved as stringbuilder in Graphic.GCode;
        }

        private string GetWrappedText()
        {
            string text = tBText.Text;
            int charPos = 0;
            int lineLength = 0;
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

            ShowTextSize();
        }

        private void CBToolTable_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = cBToolTable.Checked;
            label3.Enabled = enabled;
            cBTool.Enabled = enabled;
        }

        #region pan_zoom
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

            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                textFont = tBText.Font = fontDialog1.Font;
                LblInfoFont.Text = textFont.FontFamily.Name.ToString();
                textColor = tBText.ForeColor = fontDialog1.Color;

                ShowTextSize();
            }
        }

        private void RbAlign1_CheckedChanged(object sender, EventArgs e)
        {
            if (RbAlign1.Checked) { tBText.TextAlign = HorizontalAlignment.Left; }
            else if (RbAlign2.Checked) { tBText.TextAlign = HorizontalAlignment.Center; }
            else if (RbAlign3.Checked) { tBText.TextAlign = HorizontalAlignment.Right; }
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

        private void tBText_TextChanged(object sender, EventArgs e)
        {
            ShowTextSize();
        }
        private void ShowTextSize()
        {
			try
			{
				if (textFont.Size != null)
					Graphic.SetFont(textFont);      // ShowTextSize
				else
				{ textFont = initFont; Graphic.SetFont(textFont); }// ShowTextSize

				RectangleF b = Graphic.GetTextBounds(GetWrappedText(), StringAlignment.Near);
				LblInfoSize.Text = string.Format("{0} pt", textFont.Size);
				LblInfoWidth.Text = string.Format("{0,9:0.00}", b.Width);
				LblInfoHeight.Text = string.Format("{0,9:0.00}", b.Height);
			}
            catch (Exception err)
            {
                Logger.Error(err, "ShowTextSize textFont.Name:'{0}'  textFont.Size:'{1}' ", textFont.FontFamily.Name, textFont.Size);
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
            textFont = new Font(textFont.Name, newSize, textFont.Style);
            tBText.Font = textFont;
            ShowTextSize();
        }

        private void BtnSetHeight_Click(object sender, EventArgs e)
        {
            RectangleF b = Graphic.GetTextBounds(tBText.Text, StringAlignment.Near);
            float newSize = (float)NUDHeight.Value * textFont.Size / b.Height;
            textFont = new Font(textFont.Name, newSize, textFont.Style);
            tBText.Font = textFont;
            ShowTextSize();
        }

        private void CbWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            tBText.WordWrap = CbWordWrap.Checked;
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
    }
}
