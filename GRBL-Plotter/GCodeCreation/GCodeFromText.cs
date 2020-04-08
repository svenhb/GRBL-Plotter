/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2019 Sven Hasemann contact: svenhb@web.de

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
*/

using System;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Drawing;

// http://imajeenyus.com/computer/20150110_single_line_fonts/
// Hershey code from: http://www.evilmadscientist.com/2011/hershey-text-an-inkscape-extension-for-engraving-fonts/
namespace GRBL_Plotter
{
    public partial class GCodeFromText : Form
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public GCodeFromText()
        {   Logger.Trace("++++++ GCodeFromText START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        private string textgcode = "";

        public string textGCode
        { get { return textgcode; } }

        private void TextForm_Load(object sender, EventArgs e)
        {
            cBFont.Items.AddRange(GCodeFromFont.getHersheyFontNames());
            cBFont.Items.AddRange(GCodeFromFont.fontFileName());

            cBFont.SelectedIndex = Properties.Settings.Default.createTextFontIndex;

            Location = Properties.Settings.Default.locationTextForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }

            int toolCount = toolTable.init();
            toolProp tmpTool;
            bool defaultToolFound = false;
            for (int i = 0; i < toolCount; i++)
            {
                tmpTool = toolTable.getToolProperties(i);
                if (i == tmpTool.toolnr)
                {
                    cBTool.Items.Add(i.ToString() + ") " + tmpTool.name);
                    if (i == Properties.Settings.Default.importGCToolDefNr)
                    {
                        cBTool.SelectedIndex = cBTool.Items.Count - 1;
                        defaultToolFound = true;
                    }
                }
            }
            if (!defaultToolFound)
                cBTool.SelectedIndex = 0;
            cBToolTable_CheckedChanged(sender, e);
        }

        private void TextForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ GCodeFromText STOP ++++++");
            Properties.Settings.Default.createTextFontIndex = cBFont.SelectedIndex;
            Properties.Settings.Default.locationTextForm = Location;
            Properties.Settings.Default.Save();
        }

        // get text, break it into chars, get path, etc... This event needs to be assigned in MainForm to poll text
        private void btnApply_Click(object sender, EventArgs e)     // in MainForm:  _text_form.btnApply.Click += getGCodeFromText;
        {
            GCodeFromFont.reset();
            GCodeFromFont.gcText = tBText.Text;
            GCodeFromFont.gcFontName = cBFont.Items[cBFont.SelectedIndex].ToString();
            GCodeFromFont.gcHeight = (double)nUDFontSize.Value;
            GCodeFromFont.gcFontDistance = (double)nUDFontDistance.Value;
            GCodeFromFont.gcLineDistance = (double)(nUDFontLine.Value / nUDFontSize.Value);
            GCodeFromFont.gcSpacing = (double)(nUDFontLine.Value / nUDFontSize.Value) / 1.5;
            GCodeFromFont.gcPauseChar = cBPauseChar.Checked;
            GCodeFromFont.gcPauseWord = cBPauseWord.Checked;
            GCodeFromFont.gcPauseLine = cBPauseLine.Checked;
            GCodeFromFont.gcConnectLetter = cBConnectLetter.Checked;

            bool groupObjects = Properties.Settings.Default.importGroupObjects;          
            Plotter.StartCode();        // initalize variables
            Plotter.InsertText("");
            Plotter.IsPathFigureEnd = true;
            Plotter.SortCode();         // sort objects
            textgcode = Plotter.FinalGCode("Text import","");
            Properties.Settings.Default.importGroupObjects = groupObjects;           
        }

        // adapt line distance depending on font size
        private void nUDFontSize_ValueChanged(object sender, EventArgs e)
        {   nUDFontLine.Value = nUDFontSize.Value * (decimal)1.5;   }

        private void btnCancel_Click(object sender, EventArgs e)
        {   this.Close();   }

        private void cBTool_SelectedIndexChanged(object sender, EventArgs e)
        {   string tmp = cBTool.SelectedItem.ToString();
            if (tmp.IndexOf(")") > 0)
            {   int tnr = int.Parse(tmp.Substring(0, tmp.IndexOf(")")));
                Properties.Settings.Default.importGCToolDefNr = tnr;
            }
        }

        private void GCodeFromText_Resize(object sender, EventArgs e)
        {   tBText.Width = this.Width - 24;
            tBText.Height = this.Height - 230;
            btnApply.Left = this.Width - 138;
            btnApply.Top  = this.Height - 70;
        }

        private void cBToolTable_CheckedChanged(object sender, EventArgs e)
        {   bool enabled = cBToolTable.Checked;
            label3.Enabled = enabled;
            cBTool.Enabled = enabled;
        }
    }
}
