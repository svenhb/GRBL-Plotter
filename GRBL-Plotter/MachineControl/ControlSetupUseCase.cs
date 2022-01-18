/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2022 Sven Hasemann contact: svenhb@web.de

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
 * 2021-07-25 code clean up
 * 2022-01-13 rework
*/

using System;
using System.IO;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlSetupUseCase : Form
    {

        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string ReturnValue1 { get; set; }

        public ControlSetupUseCase()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
            timer.Interval = 500;
            timer.Stop();
            timer.Elapsed += TimerElapsed;
        }

        private void ControlSetupUseCase_Load(object sender, EventArgs e)
        {
            FillUseCaseFileList(Datapath.Usecases);
            tBSetup.Text = "Last loaded: " + Properties.Settings.Default.useCaseLastLoaded + "\r\n";
            string path = Datapath.Usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            tBSetup.Text += "Actually Set:\r\n" + MyIni.ShowIniSettings(true);
            tBSetup.Select(0, 0);
            this.Size = Properties.Settings.Default.sizeUseCase;
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            timer.Stop();
            if (Properties.Settings.Default.useCaseLastLoaded == "")
            { MessageBox.Show(this.Owner, Localization.GetString("useCaseInfo"), Localization.GetString("useCaseInfo2"), MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            ReturnValue1 = "";
            if (string.IsNullOrEmpty(lBUseCase.Text))
                return;
            string path = Datapath.Usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            Logger.Trace("▄▄▄▄▄▄▄ Load use case: '{0}'", path);
            MyIni.ReadAll();    // ReadImport();
            Properties.Settings.Default.useCaseLastLoaded = lBUseCase.Text; ;
            lblLastUseCase.Text = Path.GetFileName(path);

            bool laseruse = Properties.Settings.Default.importGCSpindleToggleLaser;
            float lasermode = Grbl.GetSetting(32);

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
        private void BtnOk_Click(object sender, EventArgs e)
        {
            ReturnValue1 = "";
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FillUseCaseFileList(string Root)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(Root);
                lBUseCase.Items.Clear();
                lBUseCase.Items.Add(Localization.GetString("useCaseItem0"));
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("ini"))
                        lBUseCase.Items.Add(Path.GetFileName(Files[i]));
                }
                lBUseCase.SelectedIndex = 0;
            }
            catch //(Exception Ex)
            {
                Logger.Error("FillUseCaseFileList no files in {0}", Root);
            }
        }

        private void LbUseCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = Datapath.Usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            tBUseCaseInfo.Text = MyIni.ReadUseCaseInfo();

            bool iniAvailable = File.Exists(path);
            tBSetup.Text = MyIni.ShowIniSettings(!iniAvailable);
            BtnLoad.Enabled = iniAvailable;
            if (iniAvailable)
            {
                BtnLoad.BackColor = LblUseCaseHeader.BackColor = System.Drawing.Color.Yellow;
                LblUseCaseHeader.Text = string.Format(Localization.GetString("useCaseHeader2"), lBUseCase.Text );
            }
            else
            {   BtnLoad.BackColor = System.Drawing.SystemColors.Control;
                LblUseCaseHeader.BackColor = BtnOk.BackColor;
                LblUseCaseHeader.Text = string.Format(Localization.GetString("useCaseHeader1"), Properties.Settings.Default.useCaseLastLoaded);
                tBUseCaseInfo.Text = Localization.GetString("useCaseInfo");
            }
        }

        private void ControlSetupUseCase_SizeChanged(object sender, EventArgs e)
        {
            tBUseCaseInfo.Width = Width - 24;
            lBUseCase.Width = Width - 370;
            lBUseCase.Height = tBSetup.Height = Height - 250;

            BtnLoad.Width = BtnOk.Width = Width - 240;
            BtnLoad.Top = Height - 122;
            BtnOk.Top = cBshowImportDialog.Top = Height - 96;

            label1.Top = lblLastUseCase.Top = Height - 64;
        }

        private void ControlSetupUseCase_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.sizeUseCase = this.Size;
            Properties.Settings.Default.Save();
        }
    }
}
