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
 * 2021-07-02 code clean up / code quality
 * 2023-03-07 l:40 f:ShowMessage get color from message and set panel color
 * 2023-03-31 l:65 f:ShowMessage reduce size if there is no color to show / check if hex-num / replace label by textBox
 * 2023-4-16 add guiLanguage
*/
using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        public void ShowMessage(string headline, string text, int mode)
        {
            if (mode == 1)
            {
                int r1 = text.IndexOf('[');
                int r2 = text.IndexOf(']');
                if ((r1 > 0) && (r2 > r1))
                {
                    string hex = text.Substring(r1 + 1, r2 - r1 - 1);
                    text = text.Substring(0, r1) + text.Substring(r2 + 1);

                    if (Int32.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int numericValue))
                    { hex = "#" + hex; }        // then # is missing

                    LblHex.Text = hex;
                    try
                    {
                        ColorPanel.BackColor = System.Drawing.ColorTranslator.FromHtml(hex);
                        LblHex.ForeColor = ContrastColor(System.Drawing.ColorTranslator.FromHtml(hex));
                    }
                    catch { }
                }
                else
                {
                    this.SizeChanged -= MessageForm_SizeChanged;
                    ColorPanel.Visible = LblHex.Visible = false;
                    btnContinue.Top = btnClose.Top = 126;
                    Height = 186;
                }

                this.Text = headline;
                tBInfo2.Text = text;
                tBInfo2.Visible = true;
                this.BackColor = tBInfo2.BackColor = Color.Yellow;
            }
            else
            {
                this.Text = headline;
                tBInfo.Text = text;
                tBInfo.Visible = true;
                btnContinue.Visible = false;
                this.Width = Math.Min(tBInfo.Width + 5, 200);
                this.Height = tBInfo.Height + 45;
                btnClose.Top = tBInfo.Height + 10;
                btnClose.Left = this.Width / 2 - 37;
                this.Width = 600;
                this.Height = 600;
                this.Top = 0;
                this.Left = 400;

                ColorPanel.Visible = LblHex.Visible = false;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MessageForm_SizeChanged(object sender, EventArgs e)
        {
            tBInfo.Width = this.Width - 26;
            tBInfo.Height = this.Height - 75;
            btnClose.Left = this.Width / 2 - 45;
            btnClose.Top = this.Height - 65;
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            /*  this.Width  = 600;
              this.Height = 600;
              this.Top = 0;
              this.Left = 400;*/
        }

        private static Color ContrastColor(Color myColor)
        {
            int d;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * myColor.R + 0.587 * myColor.G + 0.114 * myColor.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }
    }
}
