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
 * 2023-04-16 add guiLanguage
 * 2023-06-20 f:ShowMessage remove mode, select type via form size
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

        public bool DontClose { get; set; } = false;
        private bool colorChange = false;
        private int progressStep = 10;

        public MessageForm()
        {
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        public void ShowMessage(int newWidth, int newHeight, string headline, string text, int delay)
        {

            this.Width = newWidth;
            this.Height = newHeight;
            if (newWidth >= 500)         // Form or MessageBox
            {
                colorChange = true;
                btnContinue.Visible = false;
                btnClose.Left = this.Width / 2 - 37;
                btnClose.Top = this.Height - 80;
                if (delay > 0)
                {
                    toolStripProgressBar1.Maximum = delay * 1000;
                    toolStripProgressBar1.Value = delay * 1000;
                    progressStep = 100;
                    timer1.Enabled = true;
                }
                else
                    statusStrip1.Visible = false;

            }
            else
            {
                btnContinue.Top = btnClose.Top = this.Height - 65;
                btnContinue.Left = 10;
                btnContinue.Width = 2 * Width / 3 - 20;
                btnClose.Left = 2 * Width / 3;
                btnClose.Width = Width / 3 - 20;
            //    webBrowser1.ScrollBarsEnabled = false;
                statusStrip1.Visible = false;
            }

            this.Text = headline;
            if (!text.Contains("html"))
                text = MessageText.HtmlHeader + "<body>\r\n" + text + "</body></html>\r\n";

            webBrowser1.DocumentText = text;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    /*    private static Color ContrastColor(Color myColor)
        {
            int d;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * myColor.R + 0.587 * myColor.G + 0.114 * myColor.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }*/

        void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Document.MouseOver += new HtmlElementEventHandler(Body_MouseOver);
            webBrowser1.Document.MouseLeave += new HtmlElementEventHandler(Body_MouseLeave);
            webBrowser1.Document.Body.Style = "zoom:80%";
        }

        void Body_MouseOver(object sender, HtmlElementEventArgs e)
        {
            DontClose = true;
            if (colorChange)
            {
                webBrowser1.Document.Body.Document.BackColor = Color.LightSkyBlue;
                this.Text = "STOP auto close";
            }
        }
        void Body_MouseLeave(object sender, HtmlElementEventArgs e)
        {
            DontClose = false;
            if (colorChange)
            {
                webBrowser1.Document.Body.Document.BackColor = Color.White;
                this.Text = "Auto close";
            }
        }
        private void MessageForm_MouseEnter(object sender, EventArgs e)
        {
            DontClose = true;
            //       tBInfo2.BackColor = Color.WhiteSmoke;
        }

        private void MessageForm_MouseLeave(object sender, EventArgs e)
        {
            DontClose = false;
            //       tBInfo2.BackColor = SystemColors.Control;
        }

        public void WebBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {   // https://stackoverflow.com/questions/18035579/how-to-open-a-link-in-webbrowser-control-in-external-browser
            if (!(e.Url.ToString().Equals("about:blank", StringComparison.InvariantCultureIgnoreCase)))
            {
                System.Diagnostics.Process.Start(e.Url.ToString());
                e.Cancel = true;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            int val = toolStripProgressBar1.Value - progressStep;
            if (val >= 0)
                toolStripProgressBar1.Value = val;
        }
    }
}
