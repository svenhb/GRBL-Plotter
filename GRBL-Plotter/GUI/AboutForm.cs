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
 * 2019-10-29 localization of strings
 * 2021-07-26 code clean up / code quality
 * 2023-01-02 GetLinkerTimestampUtc add try catch
*/


using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
            linkLabel2.Text = Datapath.AppDataFolder;   // System.Windows.Forms.Application.StartupPath;
            toolTip1.SetToolTip(linkLabel2, "Open file explorer and visit '" + Datapath.AppDataFolder + "'");
        }

        /* link to github */
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { Process.Start(@"https://github.com/svenhb/GRBL-Plotter"); }
            catch (Exception err)
            { MessageBox.Show("Could not open URL : " + err.Message, "Error"); }
        }

        /* show actual verison */
        private void AboutForm_Load(object sender, EventArgs e)
        {
            lblVersion.Text = string.Format("{0}    {1}", System.Windows.Forms.Application.ProductVersion.ToString(), GetLinkerTimestampUtc(System.Reflection.Assembly.GetExecutingAssembly()).ToString("yyyy-MM-dd"));   //File.GetCreationTime(System.Reflection.Assembly.GetExecutingAssembly().Location)
        }

        /* check prog. version */
        private void Button2_Click(object sender, EventArgs e)
        {
            CheckUpdate.CheckVersion(true, Properties.Settings.Default.guiLastEndReason);
        }

        /* open AppData */
        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { Process.Start(Datapath.AppDataFolder); }// (@"c:\test");
            catch (Exception err)
            { MessageBox.Show("Could not open folder : " + err.Message, "Error"); }
        }

        /* open home page */
        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { Process.Start(@"https://GRBL-Plotter.de/?setlang=en"); }
            catch (Exception err)
            { MessageBox.Show("Could not open URL : " + err.Message, "Error"); }
        }

        /* date/time of compilation */
        public static DateTime GetLinkerTimestampUtc(System.Reflection.Assembly assembly)
        {
            var location = assembly.Location;
            if (!string.IsNullOrEmpty(location))
                return GetLinkerTimestampUtc(location);
            else
                return DateTime.MinValue;
        }

        public static DateTime GetLinkerTimestampUtc(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var bytes = new byte[2048];

            try
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    file.Read(bytes, 0, bytes.Length);
                }
            }
            catch
            { return DateTime.MinValue; }

            var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(secondsSince1970);
        }
    }
}
