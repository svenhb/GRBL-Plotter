using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter.GUI
{
    public partial class splashscreen : Form
    {
        public splashscreen()
        {
            InitializeComponent();
        }

        private void splashscreen_Load(object sender, EventArgs e)
        {
            label1.Text = " Ver. " + System.Windows.Forms.Application.ProductVersion.ToString();
        }
    }
}
