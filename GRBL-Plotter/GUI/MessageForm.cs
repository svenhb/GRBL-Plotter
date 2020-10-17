using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            InitializeComponent();
        }

        public void showMessage(string headline, string text, int mode)
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
                this.Width  = 600;
              this.Height = 600;
              this.Top = 0;
              this.Left = 400;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MessageForm_SizeChanged(object sender, EventArgs e)
        {
            tBInfo.Width  = this.Width - 26;
            tBInfo.Height = this.Height- 75;
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
