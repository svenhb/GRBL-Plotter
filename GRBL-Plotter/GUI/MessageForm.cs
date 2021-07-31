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
 * 2021-07-02 code clean up / code quality
*/
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            InitializeComponent();
        }

        public void ShowMessage(string headline, string text, int mode)
        {
            if (mode == 1)
            {
                this.Text = headline;
                lblInfo.Text = text;
                lblInfo.Visible = true;
                this.BackColor = Color.Yellow;

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
    }
}
