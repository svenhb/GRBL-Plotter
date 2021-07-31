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
 * 2019-10-27 add logger
 * 2019-12-07 send some user info in line 56
 * 2020-05-06 add Logger.Info
 * 2021-05-06 new method to get unique id
 * 2021-07-26 code clean up / code quality
*/

using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

//#pragma warning disable CA1303	// Do not pass literals as localized parameters
//#pragma warning disable CA1307

namespace GrblPlotter
{
    class CheckUpdate
    {
        private enum CounterType { import, usage };

        private static bool showAny = false;
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

        public static void CheckVersion(bool showAnyResult = false)
        {
 //           Logger.Info("CheckVersion");
            showAny = showAnyResult;
            //            if (Properties.Settings.Default.guiCheckUpdate)
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(CheckUpdate.AsyncCheckVersion));
        }

        private static void AsyncCheckVersion(object foo)
        {
            try
            {
                TryCheckSite2();
            } //official https
            catch
            {
                CheckSite1(@"https://api.github.com/repos/svenhb/GRBL-Plotter/releases/latest");
                //   throw;
            }
        }

        private static void TryCheckSite2()
        {
            try
            {
                CultureInfo ci = CultureInfo.InstalledUICulture;
                ci = CultureInfo.CurrentUICulture;
                Logger.Trace(culture, " Vers.:{0}  ID:{1}  LangSet:{2}  LangOri:{3}  url:{4}", Application.ProductVersion, GetID(), Properties.Settings.Default.guiLanguage, ci.Name, Properties.Settings.Default.guiCheckUpdateURL);
                string get = "";
                get += "?vers=" + Application.ProductVersion;
                get += "&hwid=" + GetID();
                get += "&langset=" + Properties.Settings.Default.guiLanguage; // add next get with &
                get += "&langori=" + ci.Name;
                get += "&import=" + GetCounters(CounterType.import);
                get += "&usage=" + GetCounters(CounterType.usage);
                if (!Properties.Settings.Default.guiCheckUpdateURL.StartsWith("http"))
                {
                    Properties.Settings.Default.guiCheckUpdateURL = "https://GRBL-Plotter.de";
                    Properties.Settings.Default.Save();
                    Logger.Info(culture, "Reset URL:{0}", Properties.Settings.Default.guiCheckUpdateURL);
                }
                CheckSite2(Properties.Settings.Default.guiCheckUpdateURL + "/GRBL-Plotter.php" + get);   // get Version-Nr and count individual ip to get an idea of amount of users
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "AsyncCheckVersion - CheckSite2"); //throw;
            }

        }


        private static string GetCounters(CounterType type)
        {
            try
            {
                if (type == CounterType.import)
                {
                    uint gcode = Properties.Settings.Default.counterImportGCode;
                    uint svg = Properties.Settings.Default.counterImportSVG;
                    uint dxf = Properties.Settings.Default.counterImportDXF;
                    uint hpgl = Properties.Settings.Default.counterImportHPGL;
                    uint csv = Properties.Settings.Default.counterImportCSV;
                    uint drill = Properties.Settings.Default.counterImportDrill;
                    uint gerber = Properties.Settings.Default.counterImportGerber;
                    uint image = Properties.Settings.Default.counterImportImage;
                    uint barcode = Properties.Settings.Default.counterImportBarcode;
                    uint text = Properties.Settings.Default.counterImportText;
                    uint shape = Properties.Settings.Default.counterImportShape;
                    uint extension = Properties.Settings.Default.counterImportExtension;
                    string tmp = string.Format(culture, "{0}-{1}-{2}-{3}-{4}-{5}-{6}_", gcode, svg, dxf, hpgl, csv, drill, gerber);
                    tmp += string.Format(culture, "{0}-{1}-{2}-{3}-{4}", image, barcode, text, shape, extension);
                    Logger.Trace(culture, " getCounters import {0}  gc,svg,dxf,hpgl,csv,drill,gerber _ img,barc,txt,shape,ext", tmp);
                    return tmp;
                }
                else if (type == CounterType.usage)
                {
                    uint laser = Properties.Settings.Default.counterUseLaserSetup;
                    uint probe = Properties.Settings.Default.counterUseProbing;
                    uint height = Properties.Settings.Default.counterUseHeightMap;
                    string tmp = string.Format(culture, "{0}-{1}-{2}", laser, probe, height);
                    Logger.Trace(culture, " getCounters usage {0}   laser,probe,height", tmp);
                    return tmp;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, " getCounters"); //throw;
            }
            return "0";
        }
        // Suddenly it was not possible anymore to get latest version from here (@"https://api.github.com/repos/svenhb/GRBL-Plotter/releases/latest"); 
        // workarround: put file 'GRBL-Plotter.txt' with actual version on own server

        private static void CheckSite2(string site)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                try
                {
                    Logger.Trace(culture, "CheckSite2 {0}", site);
                    string[] lines = wc.DownloadString(site).Split(';');
                    string vers = lines[0];
                    Logger.Trace(culture, "CheckSite2 {0}", String.Join(" | ", lines));
                    Version current = typeof(CheckUpdate).Assembly.GetName().Version;
                    Version latest = new Version(vers);

                    String info = "";
                    if (lines.Length > 1)
                        info = lines[1];
                    ShowResult(current, latest, info);
                }
                catch (WebException ex)
                {
                    Logger.Error(ex, "CheckSite2 1)");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "CheckSite2 2)"); //throw;
                }
            }
        }

        private static void CheckSite1(string site)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                Logger.Trace(culture, "CheckSite1 {0}", site);
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

                Version current = typeof(CheckUpdate).Assembly.GetName().Version;
                Version latest = new Version(versionstr);

                ShowResult(current, latest, "");
                Logger.Trace(culture, "CheckSite1 {0}", json);
            }
        }

        private static void ShowResult(Version current, Version latest, String info)
        {
            String txt = "A new GRBL-Plotter version is available";
            String title = "New Version";
            if (current >= latest)
            {
                txt = "Installed version is up to date - nothing to do";
                title = "Information";
            }
            if (info != null)
            {
                Logger.Info("CheckVersion: current:{0} latest:{1}  info:{2}", current, latest, info.Replace("\n","|"));

                if (!string.IsNullOrEmpty(info))
                    info = "\r\n" + info + "\r\n";
            }
            if ((current < latest) || showAny)
            {
                if (MessageBox.Show(txt + "\r\nInstalled Version: " + current + "\r\nLatest Version     : " + latest + "\r\n" + info + "\r\nCheck: https://github.com/svenhb/GRBL-Plotter/releases \r\n\r\nOpen website?", title,
                MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false) == DialogResult.Yes)    // (MessageBoxOptions)0x40000);
                {
                    System.Diagnostics.Process.Start(@"https://github.com/svenhb/GRBL-Plotter/releases");
                }
            }
        }
        //#pragma warning disable CA1305
        private static string GetID()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");
            mbsList = mos.Get();
            string processorId = string.Empty;
            foreach (ManagementBaseObject mo in mbsList)
            { processorId = mo["ProcessorID"] as string; }

            mos = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
            mbsList = mos.Get();
            mos.Dispose();
            string systemId = string.Empty;
            foreach (ManagementBaseObject mo in mbsList)
            { systemId = mo["UUID"] as string; }

            var hash = new System.Security.Cryptography.SHA1Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(processorId + systemId));
            string result = string.Concat(hash.Select(b => b.ToString("x2")));
            //	hash.Dispose();
            return result;
        }
    }
}
