using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace GRBL_Plotter
{
    class checkUpdate
    {
        private static bool showAny = false;
        public static void CheckVersion(bool showAnyResult=false)
        {
            showAny = showAnyResult;
            if (Properties.Settings.Default.guiCheckUpdate)
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(checkUpdate.AsyncCheckVersion));
        }

        private static void AsyncCheckVersion(object foo)
        {
            try
            { CheckSite1(@"https://api.github.com/repos/svenhb/GRBL-Plotter/releases/latest"); } //official https
            catch
            {
                try
                { CheckSite2(@"http://svenhb.bplaced.net/GRBL-Plotter.php"); }  // get Version-Nr and count individual ip to get an idea of amount of users
                catch
                { }
            }
        }

        // Suddenly it was not possible anymore to get latest version from here (@"https://api.github.com/repos/svenhb/GRBL-Plotter/releases/latest"); 
        // workarround: put file 'GRBL-Plotter.txt' with actual version on own server

        private static void CheckSite2(string site)
        {
           using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                string vers = wc.DownloadString(site);
                Version current = typeof(checkUpdate).Assembly.GetName().Version; 
                Version latest = new Version(vers);
                showResult(current, latest);
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
