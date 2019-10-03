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
        {   InitializeComponent();  }

        private void ControlSetupUseCase_Load(object sender, EventArgs e)
        {   fillUseCaseFileList(Application.StartupPath + datapath.usecases);
            tBUseCaseInfo.Text += "\r\n\r\nLast loaded: " + Properties.Settings.Default.useCaseLastLoaded;
            tBSetup.Text = "Last loaded: "+ Properties.Settings.Default.useCaseLastLoaded;
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            ReturnValue1 = "";
            if (lBUseCase.Text == "")
                return;
            string path = Application.StartupPath + datapath.usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            Logger.Trace("Load use case: '{0}'", path);
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
            catch (Exception Ex)
            {   throw (Ex);  }
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
