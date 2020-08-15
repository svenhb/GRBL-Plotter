/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

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
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2019-12-07 show current settings on start up MyIni.showIniSettings(true)
 * 2020-08-10
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{
    public partial class ControlSetupUseCase : Form
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string ReturnValue1 { get; set; }

        public ControlSetupUseCase()
        {   InitializeComponent();
            this.Icon = Properties.Resources.Icon;
        }

        private void ControlSetupUseCase_Load(object sender, EventArgs e)
        {   fillUseCaseFileList(Application.StartupPath + datapath.usecases);
            tBUseCaseInfo.Text += "\r\n\r\nLast loaded: " + Properties.Settings.Default.useCaseLastLoaded;
            tBSetup.Text = "Last loaded: "+ Properties.Settings.Default.useCaseLastLoaded+"\r\n";
            string path = Application.StartupPath + datapath.usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            tBSetup.Text += "Actually Set:\r\n" + MyIni.showIniSettings(true);
            tBSetup.Select(0, 0);
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            ReturnValue1 = "";
            if (lBUseCase.Text == "")
                return;
            string path = Application.StartupPath + datapath.usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            Logger.Trace("▄▄▄▄▄▄▄ Load use case: '{0}'", path);
            MyIni.ReadAll();    // ReadImport();
            Properties.Settings.Default.useCaseLastLoaded = lBUseCase.Text; ;
            lblLastUseCase.Text = lBUseCase.Text;

            bool laseruse = Properties.Settings.Default.importGCSpindleToggleLaser;
            float lasermode = grbl.getSetting(32);

            if (lasermode >= 0)
            {   
                if ((lasermode > 0) && !laseruse)
                {
                    DialogResult dialogResult = MessageBox.Show("grbl laser mode ($32) is activated, \r\nbut not recommended\r\n\r\n Press 'Yes' to fix this", "Attention", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        ReturnValue1 = "$32=0 (laser mode off)";
                }

                if ((lasermode < 1) && laseruse)
                {
                    DialogResult dialogResult = MessageBox.Show("grbl laser mode ($32) is not activated, \r\nbut recommended if a laser will be used\r\n\r\n Press 'Yes' to fix this", "Attention", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        ReturnValue1 = "$32=1 (laser mode on)";
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            ReturnValue1 = "";
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void fillUseCaseFileList(string Root)
        {
            List<string> FileArray = new List<string>();
            try
            {   string[] Files = System.IO.Directory.GetFiles(Root);
                lBUseCase.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {   if (Files[i].ToLower().EndsWith("ini"))
                        lBUseCase.Items.Add(Path.GetFileName(Files[i]));
                }
            }
            catch //(Exception Ex)
            {   //throw (Ex);
            }
        }

        private void lBUseCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = Application.StartupPath + datapath.usecases +"\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            tBUseCaseInfo.Text = MyIni.ReadUseCaseInfo();
            tBSetup.Text = MyIni.showIniSettings();
            btnLoad.BackColor = System.Drawing.Color.LightGreen;
        }
    }
}
