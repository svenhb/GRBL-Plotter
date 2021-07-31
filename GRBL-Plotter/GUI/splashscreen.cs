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
 * 2021-07-26 code clean up / code quality
*/

using System;
using System.IO;
using System.Windows.Forms;

//#pragma warning disable CA1305

namespace GrblPlotter.GUI
{
    public partial class Splashscreen : Form
    {
        public Splashscreen()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
        }

        private void Splashscreen_Load(object sender, EventArgs e)
        {
            //    label1.Text = " Ver. " + System.Windows.Forms.Application.ProductVersion.ToString();
            label1.Text = string.Format("Ver.:{0}", System.Windows.Forms.Application.ProductVersion.ToString());//, File.GetCreationTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd hh:mm:ss"));
        }
    }
}
