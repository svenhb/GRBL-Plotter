using GrblPlotter.UserControls;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GrblPlotter.MachineControl
{
    public partial class ControlATC : Form
    {
        private int LastSelectedToolIndex = -1;

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent;
        protected virtual void OnRaiseGuiControlEvent(UserControlGuiControlEventArgs e)
        { RaiseGuiControlEvent?.Invoke(this, e); }

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ControlATC()
        {
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
            ToolChanger.Init();
        }
        private void ControlATC_Load(object sender, EventArgs e)
        {
            var psd = Properties.Settings.Default;
            RbPenChange0.Checked = psd.DevicePlotterPenChangeRBNo;
            RbPenChange1.Checked = psd.DevicePlotterPenChangeRBManual;
            RbPenChange2.Checked = psd.DevicePlotterPenChangeRBAutomatic;
            EnableAutomatic(RbPenChange2.Checked);
            FillListView();
            RbPenChange2.CheckedChanged += RbPenChange2_CheckedChanged;
        }
        private void BtnPresetX_Click(object sender, EventArgs e)
        {
            int maxNr = (int)NudMaxToolNr.Value;
            decimal x = 0, val = NudPresetX.Value;
            ToolChanger.toolPositionArray.Clear();
            ToolPosition tp;
            for (int i = 1; i <= maxNr; i++)
            {
                val = NudPresetX.Value;
                tp = new ToolPosition(i, x, 0, 0, 0, (float)Math.Round(2 * val / 3));
                ToolChanger.toolPositionArray.Add(tp);
                x += val;
            }
            ToolChanger.WriteXML();
            FillListView();
        }
        private void BtnPresetY_Click(object sender, EventArgs e)
        {
            int maxNr = (int)NudMaxToolNr.Value;
            decimal y = 0, val = NudPresetY.Value;
            ToolChanger.toolPositionArray.Clear();
            ToolPosition tp;
            for (int i = 1; i <= maxNr; i++)
            {
                tp = new ToolPosition(i, 0, y, 0, 0, (float)Math.Round(2 * val / 3));
                ToolChanger.toolPositionArray.Add(tp);
                y += val;
            }
            ToolChanger.WriteXML();
            FillListView();
        }

        private void FillListView()
        {
            LastSelectedToolIndex = -1;
            NudToolNr.Value = 1;
            GbEdit.Enabled = false;
            LvToolPositions.Items.Clear();
            ListViewItem item;
            ToolPosition tp;
            for (int i = 0; i < ToolChanger.toolPositionArray.Count; i++)
            {
                tp = ToolChanger.toolPositionArray[i];
                item = new ListViewItem(tp.ToolNr.ToString());
                item.SubItems.Add(tp.Position.X.ToString("0.0"));
                item.SubItems.Add(tp.Position.Y.ToString("0.0"));
                item.SubItems.Add(tp.Position.Z.ToString("0.0"));
                item.SubItems.Add(tp.Position.A.ToString("0.0"));
                item.SubItems.Add(tp.Diameter.ToString("0.0"));

                LvToolPositions.Items.Add(item);// Range(new ListViewItem[] { item1, item2, item3 });
            }
            TbDescription.Text = ToolChanger.Description;
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.guiUpdate, 98));
        }

        private void BtnEditFileDialogTT_Click(object sender, EventArgs e)
        {
            var psd = Properties.Settings.Default;
            string fileName = "";
            Button clickedButton = sender as Button;
            if (clickedButton.Name.IndexOf("TT1") > 0)
                fileName = psd.ctrlToolScriptPut;
            else if (clickedButton.Name.IndexOf("TT2") > 0)
                fileName = psd.ctrlToolScriptSelect;
            else if (clickedButton.Name.IndexOf("TT3") > 0)
                fileName = psd.ctrlToolScriptGet;
            else if (clickedButton.Name.IndexOf("TT4") > 0)
                fileName = psd.ctrlToolScriptProbe;

            Logger.Trace("BtnEditFileDialogTT_Click '{0}'", fileName);
            string ipath = Datapath.MakeAbsolutePath(fileName);
            Logger.Info("SetFilePath initiial: box:'{0}'   makeAbsolute:'{1}'", fileName, ipath);
            if (File.Exists(ipath))
                Process.Start("notepad.exe", ipath);
            else
                Logger.Trace("not found");
        }

        private void BtnFileDialogTT_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            //            MessageBox.Show(clickedButton.ToolName);
            if (clickedButton.Name.IndexOf("TT1") > 0)
                SetFilePath(tBToolChangeScriptPut);
            else if (clickedButton.Name.IndexOf("TT2") > 0)
                SetFilePath(tBToolChangeScriptSelect);
            else if (clickedButton.Name.IndexOf("TT3") > 0)
                SetFilePath(tBToolChangeScriptGet);
            else if (clickedButton.Name.IndexOf("TT4") > 0)
                SetFilePath(tBToolChangeScriptProbe);
        }
        private void SetFilePath(TextBox tmp, string filter = "GCode (*.nc)|*.nc|All Files (*.*)|*.*")
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            string ipath = Datapath.MakeAbsolutePath(tmp.Text);
            Logger.Info("SetFilePath initiial: box:'{0}'   makeAbsolute:'{1}'", tmp.Text, ipath);
            opnDlg.InitialDirectory = ipath.Substring(0, ipath.LastIndexOf("\\"));
            opnDlg.Filter = filter;  //"GCode (*.nc)|*.nc|All Files (*.*)|*.*";
            //            MessageBox.Show(opnDlg.InitialDirectory+"\r\n"+ Application.StartupPath);
            if (opnDlg.ShowDialog(this) == DialogResult.OK)
            {
                FileInfo f = new FileInfo(opnDlg.FileName);
                string path;
                Logger.Info("SetFilePath DirectoryName:{0}   Datapath.AppDataFolder:{1}", f.DirectoryName, Datapath.AppDataFolder);
                if (f.DirectoryName == Datapath.AppDataFolder)
                    path = f.Name;  // only file name
                else if (f.DirectoryName.StartsWith(Datapath.AppDataFolder))
                    path = f.FullName.Replace(Datapath.AppDataFolder, ".");
                else
                    path = f.FullName;  // Full path
                if (path.StartsWith(@".\"))
                    path = path.Substring(2);
                tmp.Text = path;
                Logger.Info("SetFilePath changed: box:{0}   makeAbsolute:{1}", path, opnDlg.FileName);
            }
            opnDlg.Dispose();
        }

        private void LvToolPositions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indices = LvToolPositions.SelectedIndices;
            if (indices.Count > 0)
            {
                int i = indices[0];
                if (i < ToolChanger.toolPositionArray.Count)
                {
                    LastSelectedToolIndex = i;
                    ToolPosition tp = ToolChanger.toolPositionArray[i];
                    LblToolNr.Text = tp.ToolNr.ToString();
                    NudEditX.Value = (decimal)tp.Position.X;
                    NudEditY.Value = (decimal)tp.Position.Y;
                    NudEditZ.Value = (decimal)tp.Position.Z;
                    NudEditA.Value = (decimal)tp.Position.A;
                    NudEditD.Value = (decimal)tp.Diameter;
                    NudToolNr.Value = tp.ToolNr;
                }
                GbEdit.Enabled = true;
            }
        }

        private void BtnApplyEdit_Click(object sender, EventArgs e)
        {
            if (LastSelectedToolIndex >= 0)
            {
                ToolPosition tp = ToolChanger.toolPositionArray[LastSelectedToolIndex];
                XyzPoint tmp = new XyzPoint();
                tmp.X = (double)NudEditX.Value;
                tmp.Y = (double)NudEditY.Value;
                tmp.Z = (double)NudEditZ.Value;
                tmp.A = (double)NudEditA.Value;
                tp.Position = tmp;
                tp.Diameter = (float)NudEditD.Value;
                ToolChanger.toolPositionArray[LastSelectedToolIndex] = tp;
                ToolChanger.WriteXML();
                FillListView();
            }
        }

        private void BtnMoveToToolOffset_Click(object sender, EventArgs e)
        {
            double x = (double)NudOffsetX.Value;
            double y = (double)NudOffsetY.Value;
            double z = (double)NudOffsetZ.Value;

            Logger.Trace("BtnMoveToToolOffset_Click x:{0}  y:{1}  z:{2}", x, y, z);
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 Z{0:0.000}", z).Replace(",", "."), 0, sender, e));
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 X{0:0.000} Y{1:0.000}", x, y).Replace(",", "."), 0, sender, e));
        }

        private void entry1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indices = LvToolPositions.SelectedIndices;
            if (indices.Count > 0)
            {
                int i = indices[0];
                if (i < ToolChanger.toolPositionArray.Count)
                {
                    //      LastSelectedToolIndex = i;
                    ToolPosition tp = ToolChanger.toolPositionArray[i];
                    double x = (double)NudOffsetX.Value + tp.Position.X;
                    double y = (double)NudOffsetY.Value + tp.Position.Y;
                    double z = (double)NudOffsetZ.Value + tp.Position.Z;

                    Logger.Trace("entry1ToolStripMenuItem_Click x:{0}  y:{1}  z:{2}", x, y, z);
                    OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 Z{0:0.000}", z).Replace(",", "."), 0, sender, e));
                    OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 X{0:0.000} Y{1:0.000}", x, y).Replace(",", "."), 0, sender, e));
                }
            }
        }

        private void BtnGripperOpen_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}", Properties.Settings.Default.ctrlToolCommandGripperOpen).Replace(",", "."), 0, sender, e));
        }
        private void BtnGripperClose_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}", Properties.Settings.Default.ctrlToolCommandGripperClose).Replace(",", "."), 0, sender, e));
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Logger.Trace("BtnSave_Click");
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Datapath.Tools,
                Filter = ToolChanger.defaultFileExtension,
                FileName = ToolChanger.defaultFileName
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Logger.Trace("BtnSave_Click Save actual tool changer: {0}", sfd.FileName);
                ToolChanger.WriteXML(sfd.FileName);
            }
            sfd.Dispose();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            Logger.Trace("BtnLoad_Click");
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = Datapath.Tools,
                Filter = ToolChanger.defaultFileExtension,
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Logger.Trace("BtnLoad_Click Load tool changer: {0}", ofd.FileName);
                ToolChanger.ReadXML(ofd.FileName);
                RbPenChange2.Checked = true;
            }
            ofd.Dispose();
            ToolChanger.WriteXML(); // save as default
            FillListView();
        }

        private void TbDescription_TextChanged(object sender, EventArgs e)
        {
            ToolChanger.Description = TbDescription.Text;
        }
        private void BtnStartToolSelect_Click(object sender, EventArgs e)
        {
            int penNr = (int)NudToolNr.Value;
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("T{0}", penNr).Replace(",", "."), 0, sender, e));
            SendScript(Properties.Settings.Default.ctrlToolScriptSelect, sender, e);                                                                                                                       //   LocalOnRaiseCmdEvent(new UserControlCmdEventArgs(Properties.Settings.Default.ctrlToolScriptGet, 1, sender, e));
        }
        private void BtnStartToolTake_Click(object sender, EventArgs e)
        {
            int penNr = (int)NudToolNr.Value;
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("T{0}", penNr).Replace(",", "."), 0, sender, e));
            SendScript(Properties.Settings.Default.ctrlToolScriptGet, sender, e);
        }
        private void BtnStartToolRemove_Click(object sender, EventArgs e)
        {
            int penNr = (int)NudToolNr.Value;
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("T{0}", penNr).Replace(",", "."), 0, sender, e));
            SendScript(Properties.Settings.Default.ctrlToolScriptPut, sender, e);
        }
        private void BtnStartToolProbe_Click(object sender, EventArgs e)
        {
            int penNr = (int)NudToolNr.Value;
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("T{0}", penNr).Replace(",", "."), 0, sender, e));
            SendScript(Properties.Settings.Default.ctrlToolScriptProbe, sender, e);
        }
        private void SendScript(string script, object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(Datapath.MakeAbsolutePath(script), 1, sender, e));// use ProcessCommands instead of _serial_form.RequestSend
        }
        private void BtnHoming_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs("$H", 0, sender, e));
        }

        private void NudOffset_ValueChanged(object sender, EventArgs e)
        {
            ToolChanger.WriteXML();
        }

        private void ControlATC_FormClosing(object sender, FormClosingEventArgs e)
        {
            var psd = Properties.Settings.Default;
            psd.DevicePlotterPenChangeRBNo = RbPenChange0.Checked;
            psd.DevicePlotterPenChangeRBManual = RbPenChange1.Checked;
            psd.DevicePlotterPenChangeRBAutomatic = RbPenChange2.Checked;
            Properties.Settings.Default.Save();
            ToolChanger.WriteXML();
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.guiUpdate, 98));
        }

        private void RbPenChange2_CheckedChanged(object sender, EventArgs e)
        {
            EnableAutomatic(RbPenChange2.Checked);
        }
        private void EnableAutomatic(bool enable)
        {
            TabControlAutomatic.Enabled = enable;
            Properties.Settings.Default.gui2DToolTableShow = enable;
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.guiUpdate, 98));
        }

        private void BtnOffsetGet_Click(object sender, EventArgs e)
        {
            NudOffsetX.Value = (decimal)Grbl.posMachine.X;
            NudOffsetY.Value = (decimal)Grbl.posMachine.Y;
            NudOffsetZ.Value = (decimal)Grbl.posMachine.Z;
            NudOffsetA.Value = (decimal)Grbl.posMachine.A;
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string url = "https://grbl-plotter.de/index.php?";
            try
            {
                System.Windows.Forms.Button clickedLink = sender as System.Windows.Forms.Button;
                Process.Start(url + clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "BtnHelp_Click ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }
        private void BtnSetToolNr1_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("T{0}", 1).Replace(",", "."), 0, sender, e));

            double x = (double)NudOffsetX.Value + ToolChanger.toolPositionArray[0].Position.X;
            double y = (double)NudOffsetY.Value + ToolChanger.toolPositionArray[0].Position.Y;
            double z = (double)NudOffsetZ.Value + ToolChanger.toolPositionArray[0].Position.Z;

            Logger.Trace("BtnSetToolNr1_Click x:{0}  y:{1}  z:{2}", x, y, z);
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 Z{0:0.000}", z).Replace(",", "."), 0, sender, e));
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("G53 G91 G0 X{0:0.000} Y{1:0.000}", x, y).Replace(",", "."), 0, sender, e));
        }

        private void BtnSendPickUp_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            string part = clickedButton.Name.Substring("BtnSend".Length);   // remove "Btn"
            Logger.Trace("BtnSendPickUp_Click   {0}", part);
            string cmd = this.Controls.Find("Tb" + part, true)[0].Text;
            OnRaiseCmdEvent(new UserControlCmdEventArgs(cmd, 0, sender, e));
        }

        private void BtnCopyPickUp_Click(object sender, EventArgs e)
        {
            CopyCommandsToClipboard("TbPickUp");
        }
        private void BtnCopyRemove_Click(object sender, EventArgs e)
        {
            CopyCommandsToClipboard("TbRemove");
        }
        private void CopyCommandsToClipboard(string part)
        {
            string all = "";
            for (int i = 1; i <= 8; i++)
            {
                all += this.Controls.Find(part + i.ToString(), true)[0].Text + "\r\n";
            }
            System.Windows.Forms.Clipboard.SetText(all);
        }
    }
}
