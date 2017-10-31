using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace GRBL_Plotter
{
    class checkUpdate
    {
        public delegate void NewVersionDlg(Version current, Version latest, string name, string url);
//        public static event NewVersionDlg NewVersion;

        public static void CheckVersion()
        {
            if (Properties.Settings.Default.guiCheckUpdate)
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(checkUpdate.AsyncCheckVersion));
        }

        private static void AsyncCheckVersion(object foo)
        {
            try
            { CheckSite(@"https://api.github.com/repos/svenhb/GRBL-Plotter/releases/latest"); } //official https
            catch
            {
          //      try { CheckSite(@"https://github.com/svenhb/GRBL-Plotter/releases"); }//http mirror
         //       catch { }
            }
        }

        private static void CheckSite(string site)
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

                if (current < latest)
                {
                    MessageBox.Show("A new GRBL-Plotter version is available\r\nInstalled Version: " + current + "\r\nLatest Version     : " + latest + "\r\n\r\nCheck:\r\nhttps://github.com/svenhb/GRBL-Plotter/releases", "New Version",
                        MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);// (MessageBoxOptions)0x40000);
                }
            }
        }
    }
}
