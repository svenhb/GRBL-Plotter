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

        public void showMessage(string headline, string text)
        {
            this.Text = headline;
            label1.Text = text;
            this.Width = Math.Min(label1.Width + 5,200);
            this.Height = label1.Height + 45;
            btnClose.Top = label1.Height + 10;
            btnClose.Left = this.Width / 2 - 37;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
