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
 * 2019-10-27 add logger
*/

using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace GRBL_Plotter
{
    class checkUpdate
    {
        private static bool showAny = false;
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void CheckVersion(bool showAnyResult=false)
        {
            showAny = showAnyResult;
            if (Properties.Settings.Default.guiCheckUpdate)
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(checkUpdate.AsyncCheckVersion));
        }

        private static void AsyncCheckVersion(object foo)
        {
            try
            {   CheckSite1(@"https://api.github.com/repos/svenhb/GRBL-Plotter/releases/latest"); } //official https
            catch
            {
                try
                {
                    CultureInfo ci = CultureInfo.InstalledUICulture;
                    ci = CultureInfo.CurrentUICulture;
                    string get = "";
                    get += "?vers=" + Application.ProductVersion;
                    get += "&langset=" + Properties.Settings.Default.guiLanguage; // add next get with &
                    get += "&langori=" + ci.Name;
                    CheckSite2(@"http://svenhb.bplaced.net/GRBL-Plotter.php"+get);   // get Version-Nr and count individual ip to get an idea of amount of users
                }
                catch (Exception ex)
                { Logger.Error(ex, "AsyncCheckVersion - CheckSite2"); }
            }
        }

        // Suddenly it was not possible anymore to get latest version from here (@"https://api.github.com/repos/svenhb/GRBL-Plotter/releases/latest"); 
        // workarround: put file 'GRBL-Plotter.txt' with actual version on own server

        private static void CheckSite2(string site)
        {
           using (System.Net.WebClient wc = new System.Net.WebClient())
            {   try
                {
                    string vers = wc.DownloadString(site);
                    Version current = typeof(checkUpdate).Assembly.GetName().Version;
                    Version latest = new Version(vers);
                    showResult(current, latest);
                } catch (Exception ex)
                { Logger.Error(ex,"CheckSite2"); }

            }
        }

        private static void CheckSite1(string site)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Headers.Add("User-Agent: .Net WebClient");
                string json = wc.DownloadString(site);

                string url = null;
                string versionstr = null;
                string name = null;

                foreach (Match m in Regex.Matches(json, @"""browser_download_url"":""([^""]+)"""))
                    if (url == null)
                        url = m.Groups[1].Value;
                foreach (Match m in Regex.Matches(json, @"""tag_name"":""v([^""]+)"""))
                    if (versionstr == null)
                        versionstr = m.Groups[1].Value;
                foreach (Match m in Regex.Matches(json, @"""name"":""([^""]+)"""))
                    if (name == null)
                        name = m.Groups[1].Value;

                Version current = typeof(checkUpdate).Assembly.GetName().Version;
                Version latest = new Version(versionstr);

                showResult(current, latest);
            }
        }

        private static void showResult(Version current, Version latest)
        {
            if ((current < latest) || showAny)
            {
                String txt = "A new GRBL-Plotter version is available";
                String title = "New Version";
                if (current >= latest)
                {
                    txt = "Installed version is up to date - nothing to do";
                    title = "Information";
                }

                if (MessageBox.Show(txt + "\r\nInstalled Version: " + current + "\r\nLatest Version     : " + latest + "\r\n\r\nCheck: https://github.com/svenhb/GRBL-Plotter/releases \r\n\r\nOpen website?", title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false) == DialogResult.Yes)    // (MessageBoxOptions)0x40000);
                {
                    System.Diagnostics.Process.Start(@"https://github.com/svenhb/GRBL-Plotter/releases");
                }
            }
        }

    }
}
