/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2021 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

//#pragma warning disable CA1307

// http://imajeenyus.com/computer/20150110_single_line_fonts/
// Hershey code from: http://www.evilmadscientist.com/2011/hershey-text-an-inkscape-extension-for-engraving-fonts/
namespace GrblPlotter
{
    public partial class GCodeFromText : Form
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public GCodeFromText()
        {
            Logger.Trace("++++++ GCodeFromText START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        //        private string textgcode = "";

        //        public string textGCode
        //        { get { return textgcode; } }

        private void TextForm_Load(object sender, EventArgs e)
        {
            FillFontSelector();

            Location = Properties.Settings.Default.locationTextForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            int toolCount = ToolTable.Init();
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
                if (cBTool.Items.Count > 0)
                    cBTool.SelectedIndex = 0;
                else
                {
                    cBToolTable.Checked = false;
                    cBToolTable.Enabled = false;
                    cBTool.Enabled = false;
                }
            }
            CBToolTable_CheckedChanged(sender, e);

            nUDFontSize.Value = Properties.Settings.Default.createTextFontSize;
            nUDFontLine.Value = Properties.Settings.Default.createTextLineDistance;
            nUDFontDistance.Value = Properties.Settings.Default.createTextFontDistance;
            nUDLineBreak.Value = Properties.Settings.Default.createTextLineBreak;

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
            Properties.Settings.Default.Save();
        }

        // get text, break it into chars, get path, etc... This event needs to be assigned in MainForm to poll text
        private void BtnApply_Click(object sender, EventArgs e)     // in MainForm:  _text_form.btnApply.Click += getGCodeFromText;
        { CreateText(); }

        public void CreateText()
        {
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

            Graphic.CleanUp();
            Graphic.Init(Graphic.SourceType.Text, "", null, null);
            Graphic.graphicInformation.OptionNodesOnly = false;
            Graphic.graphicInformation.OptionSortCode = false;
            Graphic.graphicInformation.OptionZFromWidth = false;

            if (Properties.Settings.Default.createTextLineBreakEnable)
                GCodeFromFont.GetCode((double)nUDLineBreak.Value);      // do automatic page break
            else
                GCodeFromFont.GetCode(0);   // no page break
            Graphic.CreateGCode();      // result is saved as stringbuilder in Graphic.GCode;
        }

        // adapt line distance depending on font size
        private void NudFontSize_ValueChanged(object sender, EventArgs e)
        { nUDFontLine.Value = nUDFontSize.Value * (decimal)1.5; }

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
            tBText.Width = Width - 37;
            tBText.Height = Height - 250;
            btnApply.Left = Width - 151;
            btnApply.Top = Height - 88;

            tabControl1.Width = Width - 17;
            panel1.Width = Width - 118;
            tabControl1.Height = Height - 36;
            panel1.Height = Height - 76;
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

                // Prevent image from zooming out beyond original size
                /*      if (newWidth < image.Width)
                      {
                          newWidth = image.Width;
                          newHeight = image.Height;
                          newX = 0;
                          newY = 0;
                      }*/
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
    }
}
