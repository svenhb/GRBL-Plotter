/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

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
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            linkLabel2.Text = System.Windows.Forms.Application.StartupPath;
            toolTip1.SetToolTip(linkLabel2, "Open file explorer and visit '"+ System.Windows.Forms.Application.StartupPath + "'");
//            linkLabel3.Text = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"https://github.com/svenhb/GRBL-Plotter");
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            lblVersion.Text = Application.ProductVersion.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkUpdate.CheckVersion(true);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.StartupPath);// (@"c:\test");
        }

    }
}
