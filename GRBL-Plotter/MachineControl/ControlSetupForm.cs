/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2025 Sven Hasemann contact: svenhb@web.de

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
 * 2017-01-01 check form-location and fix strange location
 * 2019-03-09 add color, width, X/Y and invert for pathRotaryInfo to show rotary over X or Y
 * 2019-08-15 add logger
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
 * 2019-12-07 add extended logging enable
 * 2020-01-04 add labels to gamePad X,Y,Z,R track bars
 * 2020-02-07 add tangential axis
 * 2020-03-05 bug fix save gui2D colors
 * 2020-07-23 hide logging
 * 2021-02-06 add gamePad PointOfViewController0
 * 2021-03-28 btnDeleteToolTable_Click only delete if a file is selected
 * 2021-07-26 code clean up / code quality
 * 2021-11-23 line 79 add try/catch
 * 2022-01-02 move MakeAbsolutePath to class Datapath
 * 2022-06-25 line 1692 SetZeroMinMax add try catch
 * 2023-04-27 add new tab 'filter'; highlight tabs with enabled options
 * 2023-08-01 check if (!e.Bounds.IsEmpty)
 * 2023-09-17 new option multi file import
 * 2024-02-04 add noise option
 * 2024-05-23 new control CbImportGraphicSortDistanceStart
 * 2024-07-21 l:337 f:SaveSettings only save custom buttons if edited
 * 2024-11-18 l:250 f:SetupForm_Load change (encodeIndex < GuiVariables.SaveEncoding.Length) to (encodeIndex < CBoxSaveEncoding.Items.Count)   
 * 2025-02-23 add M6PassThrough CbToolChangeM6PassThrough #435
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlSetupForm : Form
    {
        private readonly Color inactive = Color.WhiteSmoke;
        internal bool settingsReloaded = false;
        private bool dGVCustomBtnChanged = false;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal string lastMarkedColor = "";
        internal string lastMarkedWidth = "";

        public string LastMarkedColor
        {
            set
            {
                lastMarkedColor = value;
                BtnGetFilterValueRemoveColor.Text = value;
                BtnGetFilterValueKeepColor.Text = value;
            }
        }
        internal string LastMarkedWidth
        {
            set
            {
                lastMarkedWidth = value;
                BtnGetFilterValueRemoveWidth.Text = value;
                BtnGetFilterValueKeepWidth.Text = value;
            }
        }
        internal string NewCustomString
        {
            set
            {
                TbGrblCustomString.Text = value;
            }
        }
        public ControlSetupForm()
        {
            Logger.Trace("++++++ ControlSetupForm START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        public void SetLastLoadedFile(string text)
        { toolTip1.SetToolTip(btnReloadFile, text); }

        private string defaultToolList = "tools.csv";
        private void SetupForm_Load(object sender, EventArgs e)
        {
            string tpath = Datapath.Tools;
            try
            {
                if (!System.IO.Directory.Exists(tpath))
                    System.IO.Directory.CreateDirectory(tpath);
            }
            catch (IOException err)
            {
                EventCollector.StoreException("SetupForm_Load Error creating path:" + tpath + " Error:" + err.Message);
                Datapath.AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);  // apply new data path
                MessageBox.Show("Path to tool-tables does not exist and could not be created: " + tpath + "\r\nPath will be modified to " + Datapath.AppDataFolder, "Error");
                Logger.Error(err, "SetupForm_Load path nok:{0}, changed to: {1}", tpath, Datapath.AppDataFolder);
                tpath = Datapath.Tools;
            }
            catch (Exception err)
            {
                EventCollector.StoreException("SetupForm_Load Error accessing path:" + tpath + " Error:" + err.Message);
                MessageBox.Show("Path to tool-tables can't be accessed: " + tpath + "\r\nError: " + err.Message, "Error");
                Logger.Error(err, "SetupForm_Load path nok:{0}", tpath);
                //throw;		// unknown exception...
            }
            defaultToolList = tpath + "\\" + ToolTable.DefaultFileName;

            if ((!ImportCSVToDgv(defaultToolList)) || (dGVToolList.Rows.Count == 1))
            {
                string[] tmp = ToolTable.defaultTool;  // { "1", "000000", "Black", "0.0", "0.0", "0.0", "3.0", "500" };
                dGVToolList.Rows.Add(tmp);
            }
            FillToolTableFileList(Datapath.Tools);
            FillUseCaseFileList(Datapath.Usecases);
            lblToolListLoaded.Text = Properties.Settings.Default.toolTableLastLoaded;
            tab2gB1.Text += " ( " + Datapath.Tools + " )";

            SetCustomBtnTable();

            SetButtonColors(btnColorBackground, Properties.Settings.Default.gui2DColorBackground);
            SetButtonColors(btnColorBackgroundPath, Properties.Settings.Default.gui2DColorBackgroundPath);
            SetButtonColors(btnColorDimension, Properties.Settings.Default.gui2DColorDimension);
            SetButtonColors(btnColorRuler, Properties.Settings.Default.gui2DColorRuler);
            SetButtonColors(btnColorPenUp, Properties.Settings.Default.gui2DColorPenUp);
            SetButtonColors(btnColorPenDown, Properties.Settings.Default.gui2DColorPenDown);
            SetButtonColors(btnColorRotaryInfo, Properties.Settings.Default.gui2DColorRotaryInfo);
            SetButtonColors(btnColorTool, Properties.Settings.Default.gui2DColorTool);
            SetButtonColors(btnColorMarker, Properties.Settings.Default.gui2DColorMarker);
            SetButtonColors(btnColorHeightMap, Properties.Settings.Default.gui2DColorHeightMap);
            SetButtonColors(btnColorMachineLimit, Properties.Settings.Default.gui2DColorMachineLimit);
            SetButtonColors(btnColorSimulation, Properties.Settings.Default.gui2DColorSimulation);
            SetProjectorButtons();

            decimal tmpPlaces = Properties.Settings.Default.importGCDecPlaces;
            if ((tmpPlaces < 0) || (tmpPlaces > 6)) { Properties.Settings.Default.importGCDecPlaces = tmpPlaces = 3; }		// set default
            nUDImportDecPlaces.Value = tmpPlaces;

            Location = Properties.Settings.Default.locationSetForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { CenterToScreen(); }

            rBImportGraphicClip1.Checked = !Properties.Settings.Default.importGraphicClip;

            int group = Properties.Settings.Default.importGroupItem;
            rBImportSVGGroupItem1.Checked = (group == 1);
            rBImportSVGGroupItem2.Checked = (group == 2);
            rBImportSVGGroupItem3.Checked = (group == 3);
            rBImportSVGGroupItem4.Checked = (group == 4);

            int sort = Properties.Settings.Default.importGroupSort;
            rBImportSVGSort0.Checked = (sort == 0);
            rBImportSVGSort1.Checked = (sort == 1);
            rBImportSVGSort2.Checked = (sort == 2);
            rBImportSVGSort3.Checked = (sort == 3);
            rBImportSVGSort4.Checked = (sort == 4);

            if (!Properties.Settings.Default.importRepeatComplete)
                rBImportSVGRepeat2.Checked = true;

            if (Properties.Settings.Default.importGCSpindleCmd)
                rBImportGCSpindleCmd1.Checked = true;
            else
                rBImportGCSpindleCmd2.Checked = true;

            if (Properties.Settings.Default.ctrlReplaceM3)
                rBCtrlReplaceM3.Checked = true;
            else
                rBCtrlReplaceM4.Checked = true;

            if (Properties.Settings.Default.rotarySubstitutionX)
                rBRotaryX.Checked = true;
            else
                rBRotaryY.Checked = true;

            if (Properties.Settings.Default.importUnitmm)
                rBImportUnitmm.Checked = true;
            else
                rBImportUnitInch.Checked = true;

            if (Properties.Settings.Default.importSVGDPI96)
                cBImportSVG_DPI_96.Checked = true;
            else
                cBImportSVG_DPI_72.Checked = true;

            if (Properties.Settings.Default.importPDNLayerVisible)
                rBImportPDNLayerVisible.Checked = true;
            else
                rBImportPDNLayerAll.Checked = true;

            if (Properties.Settings.Default.importVectorizeDetectTransparency)
                rBImportVectorizeTransparency.Checked = true;
            else
                rBImportVectorizeBrightness.Checked = true;

            if (Properties.Settings.Default.importVectorizeAlgorithmPoTrace)
                rBImportVectorizeAlgoPoTrace.Checked = true;
            else
                rBImportVectorizeAlgoMyTrace.Checked = true;

            if (Properties.Settings.Default.importVectorizeDpiFromImage)
                RrBImportVectorizeDPIFromImage.Checked = true;
            else
                RrBImportVectorizeDPISet.Checked = true;

            if (Properties.Settings.Default.importGraphicDevelopmentFeedX)
                rBImportGraphicDevelopFeedX.Checked = true;
            else
                rBImportGraphicDevelopFeedY.Checked = true;

            NudImportGraphicDevelopNotchDistance.Enabled = !Properties.Settings.Default.importGraphicDevelopmentNoCurve;
            LblImportGraphicDevelopNotchDistance.Enabled = !Properties.Settings.Default.importGraphicDevelopmentNoCurve;

            lblFilePath.Text = Datapath.AppDataFolder;

            HsFilterScrollSetLabels();

            SetLabelParameterSet(1, Properties.Settings.Default.camShapeSet1);
            SetLabelParameterSet(2, Properties.Settings.Default.camShapeSet2);
            SetLabelParameterSet(3, Properties.Settings.Default.camShapeSet3);
            SetLabelParameterSet(4, Properties.Settings.Default.camShapeSet4);

            CbImportGCTool_CheckedChanged(sender, e);

            dGVToolList.SortCompare += new DataGridViewSortCompareEventHandler(this.DgvSortColor);

            ListHotkeys();
            lblJoystickSize.Text = hScrollBar1.Value.ToString();

            cBoxPollInterval.SelectedIndex = Properties.Settings.Default.grblPollIntervalIndex;
            foreach (Encoding encode in GuiVariables.SaveEncoding)
            { CBoxSaveEncoding.Items.Add(encode.BodyName); }
            int encodeIndex = Properties.Settings.Default.FCTBSaveEncodingIndex;
            if ((encodeIndex >= 0) && (encodeIndex < CBoxSaveEncoding.Items.Count))     //GuiVariables.SaveEncoding.Length))
                CBoxSaveEncoding.SelectedIndex = Properties.Settings.Default.FCTBSaveEncodingIndex;
            else
                CBoxSaveEncoding.SelectedIndex = Properties.Settings.Default.FCTBSaveEncodingIndex = 0;

            CheckVisibility();
            CbImportSVGGroup_CheckedChanged(sender, e);
            CbToolTableUse_CheckedChanged(sender, e);
            CbImportSVGResize_CheckedChanged(sender, e);
            CbImportGCUsePWM_CheckedChanged(sender, e);
            CbImportGCUseIndividual_CheckedChanged(sender, e);
            CbImportGCDragKnife_CheckedChanged(sender, e);
            CbImportGCLineSegments_CheckedChanged(sender, e);
            CbImportGCNoArcs_CheckedChanged(sender, e);
            CbImportGCTangential_CheckStateChanged(sender, e);
            CbImportGraphicHatchFill_CheckStateChanged(sender, e);
            CbPathOverlapEnable_CheckStateChanged(sender, e);
            CbImportGraphicNoise_CheckStateChanged(sender, e);
            CbToolChangeM6PassThrough_CheckedChanged(sender, e);

            uint val = Properties.Settings.Default.importLoggerSettings;
            cBLogLevel1.Checked = (val & (uint)LogEnables.Level1) > 0;
            cBLogLevel2.Checked = (val & (uint)LogEnables.Level2) > 0;
            cBLogLevel3.Checked = (val & (uint)LogEnables.Level3) > 0;
            cBLogLevel4.Checked = (val & (uint)LogEnables.Level4) > 0;
            cBLog0.Checked = (val & (uint)LogEnables.Detailed) > 0;
            cBLog1.Checked = (val & (uint)LogEnables.Coordinates) > 0;
            cBLog2.Checked = (val & (uint)LogEnables.Properties) > 0;
            cBLog3.Checked = (val & (uint)LogEnables.Sort) > 0;
            cBLog4.Checked = (val & (uint)LogEnables.GroupAllGraphics) > 0;
            cBLog5.Checked = (val & (uint)LogEnables.ClipCode) > 0;
            cBLog6.Checked = (val & (uint)LogEnables.PathModification) > 0;

            CheckZEngraveExceed();

            rBStreanProtocoll2.Checked = !Properties.Settings.Default.grblStreamingProtocol1;

            PWMIncValue = (int)Properties.Settings.Default.setupPWMIncrement;
            btnPWMInc.Text = "Inc. " + PWMIncValue.ToString();
            SetPWMIncValues(PWMIncValue);

            LblAccessorySpindleVal.Text = Properties.Settings.Default.grblRunTimeSpindle.ToString("F0");
            LblAccessoryFloodVal.Text = Properties.Settings.Default.grblRunTimeFlood.ToString("F0");
            LblAccessoryMistVal.Text = Properties.Settings.Default.grblRunTimeMist.ToString("F0");

            CbAux1ZMode.SelectedIndex = Properties.Settings.Default.importGCAux1ZMode;
            CbAux2ZMode.SelectedIndex = Properties.Settings.Default.importGCAux2ZMode;

            if (Properties.Settings.Default.guiJoystickApperance1)
                RbApperance1.Checked = true;
            else
                RbApperance2.Checked = true;

            if (Properties.Settings.Default.importGraphicFilterChoiceRemove)
            {
                RbimportGraphicFilterChoiceRemove1.Checked = true; //GbFilterRemove.BackColor = Color.Yellow; GbFilterKeep.BackColor = Color.WhiteSmoke;
            }
            else
            {
                RbimportGraphicFilterChoiceRemove2.Checked = true; //GbFilterRemove.BackColor = Color.WhiteSmoke; GbFilterKeep.BackColor = Color.Yellow;
            }

            LblZEngrave.Text = Properties.Settings.Default.importGCZDown.ToString("0.0");

            if (Properties.Settings.Default.multipleLoadByX)
                RbMultipleLoadByX.Checked = true;
            else
                RbMultipleLoadByY.Checked = true;

            if (Properties.Settings.Default.multipleLoadLimitNo)
                RbMultipleLoadLimitNo.Checked = true;
            else
                RbMultipleLoadLimitDim.Checked = true;
            RbMultipleLoadLimitNo_CheckedChanged(sender, e);

            int tmpIndex = Properties.Settings.Default.importSVGAddOnPosition;
            if ((tmpIndex >= 0) && (tmpIndex < CbImportSVGAddOnPosition.Items.Count))
            { CbImportSVGAddOnPosition.SelectedIndex = tmpIndex; }
            else
            {
                CbImportSVGAddOnPosition.SelectedIndex = Properties.Settings.Default.importSVGAddOnPosition = 0;
            }

            CbImportGraphicSortDistanceStart.SelectedIndex = Properties.Settings.Default.importGraphicSortDistanceStart;

            TbGrblCustomString.Text = Grbl.GetInfo("VER1");
        }

        public void ShowTab(string tab)
        {
            if (tab == "setup")
                tabControl_Level1.SelectedIndex = 1;
        }
        public void UpdateToolTable()
        {
            Logger.Trace("UpdateToolTable  {0}", Properties.Settings.Default.toolTableLastLoaded);
            LoadToolList(Properties.Settings.Default.toolTableLastLoaded);
        }

        private void SaveSettings()
        {
            if (dGVCustomBtnChanged)
            {
                Logger.Trace("SaveSettings CustomBtnTable");
                for (int i = 1; i <= 32; i++)
                {
                    try { Properties.Settings.Default["guiCustomBtn" + i.ToString()] = dGVCustomBtn.Rows[i - 1].Cells[1].Value + "|" + dGVCustomBtn.Rows[i - 1].Cells[2].Value + "|" + dGVCustomBtn.Rows[i - 1].Cells[3].Value; }
                    catch { Properties.Settings.Default["guiCustomBtn" + i.ToString()] = " | | "; }
                }
            }
            Properties.Settings.Default.importGCDecPlaces = nUDImportDecPlaces.Value;
            Properties.Settings.Default.importGCSpindleCmd = rBImportGCSpindleCmd1.Checked;
            Properties.Settings.Default.ctrlReplaceM3 = rBCtrlReplaceM3.Checked;
            Properties.Settings.Default.rotarySubstitutionX = rBRotaryX.Checked;

            Properties.Settings.Default.Save();

            ExportDgvToCSV(defaultToolList);
            ToolTable.Init(" (SaveSettings)");
            SetCustomBtnTable();
        }

        private void SetCustomBtnTable()
        {
            Logger.Trace("setCustomBtnTable");
            string text;
            string[] parts;// = new string[] { "-", "(-)" };
            dGVCustomBtn.Rows.Clear();
            int row = 0;
            for (int i = 1; i <= 32; i++)
            {
                parts = new string[] { " ", " ", " ", " " };
                text = Properties.Settings.Default["guiCustomBtn" + i.ToString()].ToString();
                if (text.IndexOf('|') > 0)
                {
                    string[] tmp = text.Split('|');
                    for (int k = 0; k < tmp.Length; k++)
                        parts[k] = tmp[k];
                }

                dGVCustomBtn.Rows.Add();
                dGVCustomBtn.Rows[row].Cells[0].Value = i.ToString();
                dGVCustomBtn.Rows[row].Cells[1].Value = parts[0];
                dGVCustomBtn.Rows[row].Cells[2].Value = parts[1];
                dGVCustomBtn.Rows[row].Cells[3].Value = parts[2];
                row++;
            }
            dGVCustomBtnChanged = false;
        }
        private void DGVCustomBtn_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dGVCustomBtnChanged = true;
        }

        private void BtnApplyChangings_Click(object sender, EventArgs e)
        { SaveSettings(); }

        private void BtnColorBackground_Click(object sender, EventArgs e)
        { ApplyColor(btnColorBackground, "gui2DColorBackground"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorBackgroundPath_Click(object sender, EventArgs e)
        { ApplyColor(btnColorBackgroundPath, "gui2DColorBackgroundPath"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorDimension_Click(object sender, EventArgs e)
        { ApplyColor(btnColorDimension, "gui2DColorDimension"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorRuler_Click(object sender, EventArgs e)
        { ApplyColor(btnColorRuler, "gui2DColorRuler"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorPenUp_Click(object sender, EventArgs e)
        { ApplyColor(btnColorPenUp, "gui2DColorPenUp"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorPenDown_Click(object sender, EventArgs e)
        { ApplyColor(btnColorPenDown, "gui2DColorPenDown"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorRotaryInfo_Click(object sender, EventArgs e)
        { ApplyColor(btnColorRotaryInfo, "gui2DColorRotaryInfo"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorTool_Click(object sender, EventArgs e)
        { ApplyColor(btnColorTool, "gui2DColorTool"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorMarker_Click(object sender, EventArgs e)
        { ApplyColor(btnColorMarker, "gui2DColorMarker"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorHeightMap_Click(object sender, EventArgs e)
        { ApplyColor(btnColorHeightMap, "gui2DColorHeightMap"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorMachineLimit_Click(object sender, EventArgs e)
        { ApplyColor(btnColorMachineLimit, "gui2DColorMachineLimit"); BtnApply2DViewChanges.PerformClick(); }
        private void BtnColorSimulation_Click(object sender, EventArgs e)
        { ApplyColor(btnColorSimulation, "gui2DColorSimulation"); BtnApply2DViewChanges.PerformClick(); }

        private void ApplyColor(Button btn, string settings)
        {
            colorDialog1.AnyColor = true;
            colorDialog1.SolidColorOnly = false;
            colorDialog1.Color = (Color)Properties.Settings.Default[settings];
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                SetButtonColors(btn, colorDialog1.Color);
                Properties.Settings.Default[settings] = colorDialog1.Color;
                SaveSettings();
            }
        }
        private static void SetButtonColors(Button btn, Color col)
        {
            btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
        }
        private static Color ContrastColor(Color myColor)
        {
            int d;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * myColor.R + 0.587 * myColor.G + 0.114 * myColor.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

        private void BtnReloadFile_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ ControlSetupForm STOP ++++++");
            Properties.Settings.Default.locationSetForm = Location;
            SaveSettings();
        }

        private void NudImportDecPlaces_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
            Gcode.SetDecimalPlaces((int)nUDImportDecPlaces.Value);
        }

        private void BtnJoyXYCalc_Click(object sender, EventArgs e)
        {
            double time = 0.5;
            double correct = 1;
            try
            {
                nUDJoyXYSpeed1.Value = (decimal)((double)nUDJoyXYStep1.Value / time * 60 * correct);
                nUDJoyXYSpeed2.Value = (decimal)((double)nUDJoyXYStep2.Value / time * 60 * correct);
                nUDJoyXYSpeed3.Value = (decimal)((double)nUDJoyXYStep3.Value / time * 60 * correct);
                nUDJoyXYSpeed4.Value = (decimal)((double)nUDJoyXYStep4.Value / time * 60 * correct);
                nUDJoyXYSpeed5.Value = (decimal)((double)nUDJoyXYStep5.Value / time * 60 * correct);
            }
            catch
            {//(Exception err) {
                MessageBox.Show("Value is out of range", "Error");
            }
        }

        private void BtnJoyZCalc_Click(object sender, EventArgs e)
        {
            double time = 0.5;
            double correct = 1;
            try
            {
                nUDJoyZSpeed1.Value = (decimal)((double)nUDJoyZStep1.Value / time * 60 * correct);
                nUDJoyZSpeed2.Value = (decimal)((double)nUDJoyZStep2.Value / time * 60 * correct);
                nUDJoyZSpeed3.Value = (decimal)((double)nUDJoyZStep3.Value / time * 60 * correct);
                nUDJoyZSpeed4.Value = (decimal)((double)nUDJoyZStep4.Value / time * 60 * correct);
                nUDJoyZSpeed5.Value = (decimal)((double)nUDJoyZStep5.Value / time * 60 * correct);
            }
            catch
            {//(Exception err) {
                MessageBox.Show("Value is out of range", "Error");
            }
        }


        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LinkLabel clickedLink = sender as LinkLabel;
                Process.Start(clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "LinkLabel_LinkClicked ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }

        private void TabPage24_Enter(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            try { ControlGamePad.Initialize(); timer1.Interval = 200; }
            catch (Exception err) { Logger.Error(err, "TabPage24_Enter "); }
        }

        private void TabPage24_Leave(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ControlGamePad.gamePad != null)
                {
                    ControlGamePad.gamePad.Poll();
                    var datas = ControlGamePad.gamePad.GetBufferedData();
                    //     lblgp.Text = "";
                    foreach (var state in datas)
                    {
                        lblgp.Text = state.Offset + " Value: " + state.Value.ToString() + " Hex: " + state.Value.ToString("X4");// + " | ";
                        ProcessGamepad(state);
                    }
                }
            }
            catch
            {
                try { ControlGamePad.Initialize(); timer1.Interval = 200; }
                catch { timer1.Interval = 5000; }
            }
        }
        private void ProcessGamepad(SharpDX.DirectInput.JoystickUpdate state)
        {
            string offset = state.Offset.ToString();
            int value = state.Value;
            if (offset.IndexOf("Buttons") >= 0)
            {
                foreach (Control c in this.tab7gB1.Controls)
                {
                    if (c.Name == ("lbl" + offset)) if (c != null)
                        { c.BackColor = (value > 0) ? Color.Lime : Color.LightGray; break; }
                }
            }
            else if (offset.IndexOf("PointOfViewControllers0") >= 0)
            {
                lblPOVC00.BackColor = lblPOVC01.BackColor = lblPOVC02.BackColor = lblPOVC03.BackColor = Color.LightGray;
                lblPOVC04.BackColor = lblPOVC05.BackColor = lblPOVC06.BackColor = lblPOVC07.BackColor = Color.LightGray;
                if (value == 0) { lblPOVC00.BackColor = Color.Lime; } // up
                else if (value == 4500) { lblPOVC01.BackColor = Color.Lime; } // up-right
                else if (value == 9000) { lblPOVC02.BackColor = Color.Lime; } // right
                else if (value == 13500) { lblPOVC03.BackColor = Color.Lime; } // down-right
                else if (value == 18000) { lblPOVC04.BackColor = Color.Lime; } // down
                else if (value == 22500) { lblPOVC05.BackColor = Color.Lime; } // down-left
                else if (value == 27000) { lblPOVC06.BackColor = Color.Lime; } // left
                else if (value == 31500) { lblPOVC07.BackColor = Color.Lime; } // up-left
            }

            else if (offset == "X")
            { trackBarX.Value = value; lblValX.Text = GamePadGetValue(value); lblValX.BackColor = GamePadGetColor(value); }
            else if (offset == "Y")
            { trackBarY.Value = value; lblValY.Text = GamePadGetValue(value); lblValY.BackColor = GamePadGetColor(value); }
            else if (offset == "Z")
            { trackBarZ.Value = value; lblValZ.Text = GamePadGetValue(value); lblValZ.BackColor = GamePadGetColor(value); }
            else if (offset == "RotationZ")
            { trackBarR.Value = value; lblValR.Text = GamePadGetValue(value); lblValR.BackColor = GamePadGetColor(value); }
        }
        private string GamePadGetValue(int value)
        { return (value - nUDOffset.Value).ToString(); }
        private Color GamePadGetColor(int value)
        {
            int center = (int)Math.Abs(value - nUDOffset.Value);
            Color tmp = Color.Transparent;
            if (center <= 5)
                return tmp;
            if (center <= nUDDead.Value)
            { tmp = Color.Lime; }
            else if (center <= nUDMinimum.Value)
            { tmp = Color.Yellow; }
            else
            { tmp = Color.Fuchsia; }
            return tmp;
        }

        private void HsFilterScroll(object sender, ScrollEventArgs e)
        { HsFilterScrollSetLabels(); }
        private void HsFilterScrollSetLabels()
        {
            lblFilterRed1.Text = hSFilterRed1.Value.ToString();
            lblFilterRed2.Text = hSFilterRed2.Value.ToString();
            lblFilterGreen1.Text = hSFilterGreen1.Value.ToString();
            lblFilterGreen2.Text = hSFilterGreen2.Value.ToString();
            lblFilterBlue1.Text = hSFilterBlue1.Value.ToString();
            lblFilterBlue2.Text = hSFilterBlue2.Value.ToString();
        }

        private void BtnShapeSetSave_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int btnSetIndex = Convert.ToUInt16(clickedButton.Name.Substring("btnShapeSetSave".Length));
            //          MessageBox.Show(index.ToString());
            if (btnSetIndex == 1) { Properties.Settings.Default.camShapeSet1 = ShapeSetSave(tBShapeSet1.Text); }
            if (btnSetIndex == 2) { Properties.Settings.Default.camShapeSet2 = ShapeSetSave(tBShapeSet2.Text); }
            if (btnSetIndex == 3) { Properties.Settings.Default.camShapeSet3 = ShapeSetSave(tBShapeSet3.Text); }
            if (btnSetIndex == 4) { Properties.Settings.Default.camShapeSet4 = ShapeSetSave(tBShapeSet4.Text); }
            Properties.Settings.Default.Save();
        }

        private void BtnShapeSetLoad_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int btnLoadIndex = Convert.ToUInt16(clickedButton.Name.Substring("btnShapeSetLoad".Length));
            //           MessageBox.Show(index.ToString());
            if (btnLoadIndex == 1) { tBShapeSet1.Text = ShapeSetLoad(Properties.Settings.Default.camShapeSet1); }
            if (btnLoadIndex == 2) { tBShapeSet2.Text = ShapeSetLoad(Properties.Settings.Default.camShapeSet2); }
            if (btnLoadIndex == 3) { tBShapeSet3.Text = ShapeSetLoad(Properties.Settings.Default.camShapeSet3); }
            if (btnLoadIndex == 4) { tBShapeSet4.Text = ShapeSetLoad(Properties.Settings.Default.camShapeSet4); }
        }

        private String ShapeSetSave(string head)
        {
            string txt = head + "|";
            txt += hSFilterRed1.Value.ToString() + "|";
            txt += hSFilterRed2.Value.ToString() + "|";
            txt += hSFilterGreen1.Value.ToString() + "|";
            txt += hSFilterGreen2.Value.ToString() + "|";
            txt += hSFilterBlue1.Value.ToString() + "|";
            txt += hSFilterBlue2.Value.ToString() + "|";
            txt += cBFilterOuside.Checked.ToString() + "|";
            txt += cBShapeCircle.Checked.ToString() + "|";
            txt += cBShapeRect.Checked.ToString() + "|";
            txt += nUDShapeSizeMin.Value.ToString() + "|";
            txt += nUDShapeSizeMax.Value.ToString() + "|";
            txt += nUDShapeDistMin.Value.ToString() + "|";
            txt += nUDShapeDistMax.Value.ToString() + "|";
            textBox3.Text = txt;
            return txt;
        }

        private string ShapeSetLoad(string txt)
        {
            string[] value = txt.Split('|');
            int i = 1;
            try
            {
                hSFilterRed1.Value = Convert.ToInt16(value[i++]);
                hSFilterRed2.Value = Convert.ToInt16(value[i++]);
                hSFilterGreen1.Value = Convert.ToInt16(value[i++]);
                hSFilterGreen2.Value = Convert.ToInt16(value[i++]);
                hSFilterBlue1.Value = Convert.ToInt16(value[i++]);
                hSFilterBlue2.Value = Convert.ToInt16(value[i++]);
                cBFilterOuside.Checked = (value[i++] == "True");
                cBShapeCircle.Checked = (value[i++] == "True");
                cBShapeRect.Checked = (value[i++] == "True");
                nUDShapeSizeMin.Value = Convert.ToDecimal(value[i++]);
                nUDShapeSizeMax.Value = Convert.ToDecimal(value[i++]);
                nUDShapeDistMin.Value = Convert.ToDecimal(value[i++]);
                nUDShapeDistMax.Value = Convert.ToDecimal(value[i++]);
                HsFilterScrollSetLabels();
                return value[0];
            }
            catch (Exception err) { Logger.Error(err, "ShapeSetLoad "); }
            return "not set";
        }

        private void SetLabelParameterSet(int lblSetIndex, string txt)
        {
            if (txt.IndexOf('|') < 1)
                txt = "";
            if (lblSetIndex == 1) { tBShapeSet1.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (lblSetIndex == 2) { tBShapeSet2.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (lblSetIndex == 3) { tBShapeSet3.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (lblSetIndex == 4) { tBShapeSet4.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
        }

        private void ExportDgvToCSV(string file)
        {
            // fields: ToolNr,color,name,X,Y,Z,diameter,XYspeed,Z - speed,Z - save,Z - depth,Z - step, spindleSpeed, overlap, gcode
            int[] cellWidth = { 3, 7, -20, 5, 5, 5, 5, 4, 4, 5, 4, 4, 5, 6, 4, 1, 1, 1, 1, 1, 1 };
            string format;
            var csv = new StringBuilder();
            csv.AppendLine("# T-Nr; Color; T-Name; ExPosX; ExPosY; ExPosZ; ExPosA; T-Diameter; XY-Feed; Z-Feed; Z-Save; Z-Deepth; Z-Step; Spindle-Spd; Step-over %, GCode");
            foreach (DataGridViewRow dgvR in dGVToolList.Rows)
            {
                //            Logger.Trace("ExportDgvToCSV row {0}", dgvR.ToString());
                bool firstColumn = true;
                if (dgvR.Cells[0].Value != null)
                {
                    for (int j = 0; j < dGVToolList.Columns.Count; ++j)
                    {
                        object val = dgvR.Cells[j].Value;
                        //              Logger.Trace("ExportDgvToCSV cell {0} {1}   {2}", j, val, cellWidth[j].ToString());
                        if (!firstColumn)
                            csv.Append(',');                            // csv delimiter
                        if (val == null)
                            csv.Append(ToolTable.defaultTool[j]);       // fill with default value
                        else
                        {
                            format = "{0," + cellWidth[j].ToString() + "}";
                            string valstr = string.Format(format, val).Replace(",", ".");  // remove any false delimiter
                            csv.AppendFormat("{0}", valstr);
                        }
                        firstColumn = false;
                    }
                    csv.Append("\r\n");
                }
            }
            Logger.Trace("Save Tool Table {0}", file);
            try
            {
                File.WriteAllText(file, csv.ToString());
            }
            catch (IOException err)
            {
                EventCollector.StoreException("Error:" + err.Message + " ExportDgvToCSV: " + file);
                Logger.Error(err, "ExportDgvToCSV IOException:{0}", file);
                MessageBox.Show("Current tool-table settings could not be saved " + file + "\r\n" + err.Message, "Error");
            }
            catch (Exception err)
            { // access denied
                EventCollector.StoreException("Error:" + err.Message + " ExportDgvToCSV: " + file);
                Logger.Error(err, "ExportDgvToCSV IOException:{0}", file);
                MessageBox.Show("Current tool-table settings could not be saved " + file + "\r\n" + err.Message, "Error");
                //				throw;		// unknown exception...
            }
            dGVToolList.DefaultCellStyle.NullValue = dGVToolList.Columns.Count;
            FillToolTableFileList(Datapath.Tools);
        }

        private bool ImportCSVToDgv(string file)
        {
            if (File.Exists(file))
            {
                Logger.Trace("Load Tool Table {0}", file);
                dGVToolList.Rows.Clear();
                string[] readText;

                try
                {
                    readText = File.ReadAllLines(file);
                }
                catch (IOException err)
                {
                    // read already opened file???
                    // https://stackoverflow.com/questions/9759697/reading-a-file-used-by-another-process
                    EventCollector.StoreException("ImportCSVToDgv IOException: " + file + " " + err.Message + " - ");
                    Logger.Error(err, "ImportCSVToDgv IOException:{0}", file);
                    MessageBox.Show("Could not read " + file + "\r\n" + err.Message, "Error");
                    return false;
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }

                string[] col;
                string tmp;
                int row = 0;
                foreach (string s in readText)
                {
                    if (s.StartsWith("#") || s.StartsWith("/"))     // jump over comments
                        continue;

                    if (s.Length > 10)
                    {
                        col = s.Split(',');
                        dGVToolList.Rows.Add();
                        for (int j = 0; j < dGVToolList.Columns.Count; ++j)
                        //   for (int j = 0; j < ToolTable.defaultTool.Length; ++j)
                        {
                            if (j < col.Length)
                            {
                                tmp = col[j].Trim();
                                dGVToolList.Rows[row].Cells[j].Value = tmp;  // fill up empty cells
                            }
                            else
                                dGVToolList.Rows[row].Cells[j].Value = ToolTable.defaultTool[j];
                        }
                        try
                        {
                            long clr = Convert.ToInt32(col[1].Trim().Substring(0, 6), 16) | 0xff000000;
                            dGVToolList.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb((int)clr);
                            dGVToolList.Rows[row].DefaultCellStyle.ForeColor = ContrastColor(Color.FromArgb((int)clr));
                        }
                        catch
                        {
                            dGVToolList.Rows[row].DefaultCellStyle.BackColor = Color.White;
                            dGVToolList.Rows[row].DefaultCellStyle.ForeColor = Color.Black;
                        }
                    }
                    row++;
                }
                return true;
            }
            return false;
        }

        private void DgvToolList_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            // change color if changed
            object val = dGVToolList.Rows[e.RowIndex].Cells[1].Value;
            if ((val != null) && (e.ColumnIndex == 1))
            {
                string myColor = val.ToString();
                if (myColor.Length > 6)
                { dGVToolList.Rows[e.RowIndex].Cells[1].Value = myColor.Substring(0, 6); }
                try
                {
                    long clr = Convert.ToInt32(myColor.Substring(0, 6), 16) | 0xff000000;
                    dGVToolList.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb((int)clr);
                    dGVToolList.Rows[e.RowIndex].DefaultCellStyle.ForeColor = ContrastColor(Color.FromArgb((int)clr));
                }
                catch
                {
                    dGVToolList.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    dGVToolList.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            Properties.Settings.Default.toolTableOriginal = false;
            lblToolListChanged.Text = "Changed List should be saved \r\nvia 'Save tool table'";
            lblToolListChanged.BackColor = Color.Yellow;
        }
        private void BtnReNumberTools_Click(object sender, EventArgs e)
        {
            int number = 1;
            foreach (DataGridViewRow dgvR in dGVToolList.Rows)
            {
                dgvR.Cells[0].Value = number++;
            }
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dGVToolList;
            try
            {
                //  int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == 0)
                    return;
                // get index of the column for the selected cell
                //   int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex - 1, selectedRow);
                dgv.ClearSelection();
                //                dgv.Rows[rowIndex - 1].Cells[colIndex].Selected = true;
                dgv.Rows[rowIndex - 1].Selected = true;
            }
            catch (Exception err) { Logger.Error(err, "BtnUp_Click "); }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dGVToolList;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == totalRows - 1)
                    return;
                // get index of the column for the selected cell
                //   int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex + 1, selectedRow);
                dgv.ClearSelection();
                //              dgv.Rows[rowIndex + 1].Cells[colIndex].Selected = true;
                dgv.Rows[rowIndex + 1].Selected = true;
            }
            catch (Exception err) { Logger.Error(err, "BtnDown_Click "); }
        }

        private void DgvSortColor(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index != 1) return;
            long lcolor1 = 0, lcolor2 = 0;
            string scolor1 = e.CellValue1.ToString().ToUpper();
            string scolor2 = e.CellValue2.ToString().ToUpper();

            if (scolor1.Length > 6)
            { scolor1 = scolor1.Substring(0, 6); }
            try { lcolor1 = Convert.ToInt32(scolor1, 16) | 0xff000000; }
            catch (Exception err)
            { Logger.Error(err, "DgvSortColor could not convert color1:{0} ", scolor1); }

            if (scolor2.Length > 6)
            { scolor2 = scolor2.Substring(0, 6); }
            try { lcolor2 = Convert.ToInt32(scolor2, 16) | 0xff000000; }
            catch (Exception err)
            { Logger.Error(err, "DgvSortColor could not convert color2:{0} ", scolor2); }

            Color ccolor1 = Color.FromArgb((int)lcolor1);
            Color ccolor2 = Color.FromArgb((int)lcolor2);
            double brighness1 = (0.299 * ccolor1.R * ccolor1.R + 0.587 * ccolor1.G * ccolor1.G + 0.114 * ccolor1.B * ccolor1.B);
            double brighness2 = (0.299 * ccolor2.R * ccolor2.R + 0.587 * ccolor2.G * ccolor2.G + 0.114 * ccolor2.B * ccolor2.B);
            e.SortResult = brighness1.CompareTo(brighness2);
            e.Handled = true;
        }
        private void BtnToolExport_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the List
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                InitialDirectory = importPath,
                Filter = "CSV File|*.csv",
                Title = "Save Tool List as CSV"
            };
            saveFileDialog1.ShowDialog();
            // If the file name is not an empty string open it for saving.  
            if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                ExportDgvToCSV(saveFileDialog1.FileName);
            }
            FillToolTableFileList(Datapath.Tools);
            saveFileDialog1.Dispose();
        }

        private static readonly string importPath = Datapath.Tools;
        private void BtnToolImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Path.GetFullPath(Properties.Settings.Default.toolTableLastLoaded),//importPath,
                Filter = "CSV File|*.csv",
                Title = "Load Tool List"
            };
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadToolList(openFileDialog1.FileName);
            }
            openFileDialog1.Dispose();
        }
        private void LoadToolList(string filename)
        {
            Logger.Trace("LoadToolList {0}", filename);
            ImportCSVToDgv(filename);
            Properties.Settings.Default.toolTableOriginal = true;
            Properties.Settings.Default.toolTableLastLoaded = filename; // Path.GetFileName(filename);
            lblToolListLoaded.Text = (filename.Contains(Datapath.Tools)) ? Path.GetFileName(filename) : filename;
            lblToolListChanged.Text = "orginal";
            lblToolListChanged.BackColor = Color.Transparent;
            ExportDgvToCSV(defaultToolList);
            this.Refresh();
        }
        private void BtnLoadToolTable_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lbFiles.Text))
            {
                MessageBox.Show("No file selected...", "Attention");
                return;
            }
            string path = Datapath.Tools + "\\" + lbFiles.Text;
            if (path.StartsWith("laser"))
                cBToolLaser.Checked = true;
            //           MessageBox.Show(path);
            LoadToolList(path);
        }
        private void BtnDeleteToolTable_Click(object sender, EventArgs e)
        {
            string path = Datapath.Tools + "\\" + lbFiles.Text;
            Logger.Info("DeleteToolTable '{0}'", path);
            if (!string.IsNullOrEmpty(lbFiles.Text))
            {
                try
                {
                    File.Delete(path);
                    FillToolTableFileList(Datapath.Tools);
                }
                catch (IOException err)
                {
                    EventCollector.StoreException("Error BtnDeleteToolTable_Click:" + path);
                    Logger.Error(err, "BtnDeleteToolTable_Click IOException:{0}", path);
                    MessageBox.Show("Could not delete " + path, "Error");
                    return;
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }
            }
        }

        internal string commandToSend = "";
        private void BtnMoveToolXY_Click(object sender, EventArgs e)
        {
            float xoff = (float)nUDToolOffsetX.Value;
            float yoff = (float)nUDToolOffsetY.Value;

            if ((dGVToolList.CurrentRow == null) ||
                (dGVToolList.CurrentRow.Cells[3].Value == null) ||
                (dGVToolList.CurrentRow.Cells[4].Value == null) ||
                (dGVToolList.CurrentRow.Cells[5].Value == null))
            {
                MessageBox.Show("No valid position (null)");
                return;
            }
            string sx = dGVToolList.CurrentRow.Cells[3].Value.ToString();
            string sy = dGVToolList.CurrentRow.Cells[4].Value.ToString();
            string sz = dGVToolList.CurrentRow.Cells[5].Value.ToString();
            //     float x, y;//, z=0;
            if ((sx.Length < 1) || (sy.Length < 1) || (sz.Length < 1))
            {
                MessageBox.Show("No valid position (length)");
                return;
            }

            if (float.TryParse(sx, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float x) && float.TryParse(sy, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float y))
            {
                x += xoff; y += yoff;
                DialogResult result = MessageBox.Show("Are you sure to move to machine XY-position " + x.ToString() + ":" + y.ToString() + " ?\r\n", "Attention", MessageBoxButtons.YesNo);
                commandToSend = "";
                if (result == DialogResult.Yes)
                {
                    commandToSend = String.Format("G53 G91 G0 X{0} Y{1}", x, y).Replace(',', '.');
                    if (Grbl.isMarlin)
                        commandToSend = String.Format("G53;G91;G0 X{0} Y{1}", x, y).Replace(',', '.');
                }
            }
            else { MessageBox.Show("No valid position (parse)"); return; }
        }

        private void Lbl1_Click(object sender, EventArgs e)
        { ShowFileContent(tBToolChangeScriptPut); }
        private void Lbl2_Click(object sender, EventArgs e)
        { ShowFileContent(tBToolChangeScriptSelect); }
        private void Lbl3_Click(object sender, EventArgs e)
        { ShowFileContent(tBToolChangeScriptGet); }
        private void Lbl4_Click(object sender, EventArgs e)
        { ShowFileContent(tBToolChangeScriptProbe); }

        private static void ShowFileContent(TextBox tmp)
        {
            if (File.Exists(tmp.Text))
            {
                string tmpText;
                try
                {
                    tmpText = File.ReadAllText(tmp.Text).Replace('\t', ' ');
                }
                catch (IOException err)
                {
                    EventCollector.StoreException("Error ShowFileContent:" + tmp.Text);
                    Logger.Error(err, "ShowFileContent IOException:{0}", tmp.Text);
                    MessageBox.Show("Could not read " + tmp.Text, "Error");
                    return;
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }

                MessageBox.Show("Current directory: " + Directory.GetCurrentDirectory() + "\r\nFile: '" + tmp.Text + "'\r\n\r\n" + tmpText, "File content of " + tmp.Text);
            }
            else
                MessageBox.Show("Current directory: " + Directory.GetCurrentDirectory() + "\r\nFile: '" + tmp.Text + "' not found", "File content");
        }

        private void BtnShowScriptSub_Click(object sender, EventArgs e)
        { ShowFileContent(tBImportGCSubroutine); }

        private void CbImportGCTool_CheckedChanged(object sender, EventArgs e)
        {
            HighlightPenOptions_Click(sender, e);
            CheckVisibility();
            tabControl1.Invalidate();
        }

        private void HighlightPenOptions_Click(object sender, EventArgs e)
        {

            if (cBImportGCRelative.Checked)
                cBImportGCRelative.BackColor = Color.Yellow;
            else
                cBImportGCRelative.BackColor = Color.Transparent;

            if (CbConvertToPolar.Checked)
                GbConvertToPolar.BackColor = Color.Yellow;
            else
                GbConvertToPolar.BackColor = Color.Transparent;

            if (cBImportSVGRepeat.Checked)
                cBImportSVGRepeat.BackColor = Color.Yellow;
            else
                cBImportSVGRepeat.BackColor = Color.Transparent;

            if (cBImportGraphicTile.Checked)
                gBClipping.BackColor = Color.Yellow;
            else
                gBClipping.BackColor = Color.WhiteSmoke;

            if (CbimportGraphicFilterEnable.Checked)
                GbFilter.BackColor = Color.Yellow;
            else
                GbFilter.BackColor = Color.WhiteSmoke;

            /*    if (RbimportGraphicFilterChoiceRemove1.Checked)
                { GbFilterRemove.BackColor = Color.Yellow; GbFilterKeep.BackColor = Color.WhiteSmoke; }
                else
                { GbFilterRemove.BackColor = Color.WhiteSmoke; GbFilterKeep.BackColor = Color.Yellow; }
            */
            if (cBDashedLine1.Checked)
            { cBDashedLine1.BackColor = cBDashedLine2.BackColor = Color.Yellow; }
            else
            { cBDashedLine1.BackColor = cBDashedLine2.BackColor = Color.Transparent; }

            if (cBImportPenWidthToZ.Checked)
                cBImportPenWidthToZ.BackColor = Color.Yellow;
            else
                cBImportPenWidthToZ.BackColor = Color.Transparent;

            if (cBImportSVGNodesOnly.Checked)
                cBImportSVGNodesOnly.BackColor = Color.Yellow;
            else
                cBImportSVGNodesOnly.BackColor = Color.Transparent;

            if (cBImportSVGCircleToDot.Checked)
            { cBImportSVGCircleToDot.BackColor = Color.Yellow; }
            else
            { cBImportSVGCircleToDot.BackColor = Color.Transparent; }

            if (cBImportLasermode.Checked)
                cBImportLasermode.BackColor = Color.Yellow;
            else
                cBImportLasermode.BackColor = Color.Transparent;

            if (cBImportGCUseZ.Checked || cBImportGCUseZ2.Checked)
            { GbSeveralPasses.BackColor = tab1_2gB3.BackColor = cBImportGCUseZ2.BackColor = Color.Yellow; }
            else
            { GbSeveralPasses.BackColor = tab1_2gB3.BackColor = cBImportGCUseZ2.BackColor = inactive; }

            if (cBImportGCUsePWM.Checked || cBImportGCUsePWM2.Checked)
            { tab1_2gB4.BackColor = cBImportGCUsePWM2.BackColor = Color.Yellow; }
            else
            { tab1_2gB4.BackColor = cBImportGCUsePWM2.BackColor = inactive; }

            if (cBImportGCUseSpindle.Checked || cBImportGCUseSpindle2.Checked)
            { tab1_2gB5.BackColor = cBImportGCUseSpindle2.BackColor = Color.Yellow; }
            else
            { tab1_2gB5.BackColor = cBImportGCUseSpindle2.BackColor = inactive; }

            if (cBImportGCUseIndividual.Checked || cBImportGCUseIndividual2.Checked)
            { tab1_2gB6.BackColor = cBImportGCUseIndividual2.BackColor = Color.Yellow; }
            else
            { tab1_2gB6.BackColor = cBImportGCUseIndividual2.BackColor = inactive; }

            if (cBImportGCNoArcs.Checked)
                cBImportGCNoArcs.BackColor = Color.Yellow;
            else
                cBImportGCNoArcs.BackColor = Color.Transparent;

            if (cBImportGCDragKnife.Checked)
                tab1_3gB2.BackColor = Color.Yellow;
            else
                tab1_3gB2.BackColor = inactive;

            if (cBImportGCTangential.Checked)
            {
                tab1_3gB5.BackColor = Color.Yellow;
                if (cBImportGCZIncEnable.Checked)
                {
                    MessageBox.Show("Tangential axis doesn't work with Z-Axis 'Use several passes'.\r\nThis option will be disabled");
                    cBImportGCZIncEnable.Checked = false;
                }
            }
            else
                tab1_3gB5.BackColor = inactive;

            if (cBImportGraphicOffsetLargestRemove.Checked)
                cBImportGraphicOffsetLargestRemove.BackColor = Color.Yellow;
            else
                cBImportGraphicOffsetLargestRemove.BackColor = inactive;

            if (CbImportSVGPathStartNewFigure.Checked)
                CbImportSVGPathStartNewFigure.BackColor = Color.Yellow;
            else
                CbImportSVGPathStartNewFigure.BackColor = inactive;

            if (CbImportSVGApplyFill.Checked)
                CbImportSVGApplyFill.BackColor = Color.Yellow;
            else
                CbImportSVGApplyFill.BackColor = inactive;

            if (cBImportGraphicHatchFill.Checked)
                gBHatchFill.BackColor = Color.Yellow;
            else
                gBHatchFill.BackColor = inactive;

            if (cBimportGraphicAddFrameEnable.Checked)
                gBPathAddOn1.BackColor = Color.Yellow;
            else
                gBPathAddOn1.BackColor = inactive;

            if (cBimportGraphicMultiplyGraphicsEnable.Checked)
                gBPathAddOn2.BackColor = Color.Yellow;
            else
                gBPathAddOn2.BackColor = inactive;

            if (cBimportGraphicLeadInEnable.Checked)
                gBPathAddOn3.BackColor = Color.Yellow;
            else
                gBPathAddOn3.BackColor = inactive;

            if (CbWireBenderEnable.Checked)
                GbWireBender.BackColor = Color.Yellow;
            else
                GbWireBender.BackColor = Color.WhiteSmoke;

            if (cBImportGraphicDevelopEnable.Checked)
                gBDevelop.BackColor = Color.Yellow;
            else
                gBDevelop.BackColor = Color.WhiteSmoke;

            cBImportGraphicHatchFillInset2.Enabled = cBImportGraphicHatchFillInset.Enabled && cBImportGraphicHatchFillInset.Checked;
        }

        private void BtnFileDialogTT1_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            //            MessageBox.Show(clickedButton.Name);
            if (clickedButton.Name.IndexOf("TT1") > 0)
                SetFilePath(tBToolChangeScriptPut);
            else if (clickedButton.Name.IndexOf("TT2") > 0)
                SetFilePath(tBToolChangeScriptSelect);
            else if (clickedButton.Name.IndexOf("TT3") > 0)
                SetFilePath(tBToolChangeScriptGet);
            else if (clickedButton.Name.IndexOf("TT4") > 0)
                SetFilePath(tBToolChangeScriptProbe);
            else if (clickedButton.Name.IndexOf("SubR") > 0)
                SetFilePath(tBImportGCSubroutine);
            else if (clickedButton.Name.IndexOf("CircleToDot") > 0)
                SetFilePath(TbImportCircleToDotScript);
        }
        private void SetFilePath(TextBox tmp, string filter = "GCode (*.nc)|*.nc|All Files (*.*)|*.*")
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            string ipath = Datapath.MakeAbsolutePath(tmp.Text);
            Logger.Info("SetFilePath initiial: box:{0}   makeAbsolute:{1}", tmp.Text, ipath);
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

        private void ControlSetupForm_SizeChanged(object sender, EventArgs e)
        {
            int y = Height;
            btnApplyChangings.Top = y - 64;
            btnReloadFile.Top = y - 64;
            cBshowImportDialog.Top = y - 60;

            tabControl_Level1.Height = y - 64;
            tab2gB2.Height = y - 200;
            dGVToolList.Height = y - 275;

            tabControl4.Height = y - 102;
            dGVCustomBtn.Height = y - 197;
            tab6lbl1.Top = y - 190;
            tab6lbl2.Top = y - 190;
            lblFilePath.Top = y - 146;
        }

        private void TbKeyPad_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox clickedTB = sender as TextBox;
            clickedTB.Text = e.KeyData.ToString();
            if (!string.IsNullOrEmpty(e.KeyData.ToString()))
                Clipboard.SetText(e.KeyData.ToString());
        }

        private void ListHotkeys()
        {
            tBHotkeyList.Clear();
            string fileName = Datapath.Hotkeys;
            if (!File.Exists(fileName))
            {
                tBHotkeyList.Text = "File 'hotkeys.xml' not found, no hotkeys set!";
                Logger.Error("File 'hotkeys.xml' not found in ", fileName);
                return;
            }
            string tmp;
            try
            {
                tmp = File.ReadAllText(fileName);
            }
            catch (IOException err)
            {
                EventCollector.StoreException("Error ListHotkeys:" + fileName);
                Logger.Error(err, "ListHotkeys IOException:{0}", fileName);
                MessageBox.Show("Could not read " + fileName, "Error");
                return;
            }
            catch
            {//(Exception err) { 
                throw;      // unknown exception...
            }

            tBHotkeyList.Text = tmp;
            lblPathHotkeys.Text = fileName;
        }
        private void BtnOpenHotkeys_Click(object sender, EventArgs e)
        { Process.Start("notepad.exe", lblPathHotkeys.Text); }

        private void BtnHotkeyRefresh_Click(object sender, EventArgs e)
        { ListHotkeys(); }

        private void BtnMachineRangeGet_Click(object sender, EventArgs e)
        {
            decimal maxX = (decimal)Grbl.GetSetting(130);
            decimal maxY = (decimal)Grbl.GetSetting(131);
            decimal maxZ = (decimal)Grbl.GetSetting(132);
            if ((maxX < 0) || (maxY < 0) || (maxZ < 0))
                MessageBox.Show(string.Format("No information available - please connect grbl-controller ($130={0}; $131={1}; $132={2}; )", maxX, maxY, maxZ), "Attention!");
            else
            {
                if (maxX > 0) nUDMachineRangeX.Value = maxX;
                if (maxY > 0) nUDMachineRangeY.Value = maxY;
                if (maxZ > 0) nUDMachineRangeZ.Value = maxZ;
            }
        }

        private void HscrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            lblJoystickSize.Text = hScrollBar1.Value.ToString();
        }

        private void CbGPEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGPEnable.Checked)
            {
                try { ControlGamePad.Initialize(); }
                catch (Exception err) { Logger.Error(err, "CbGPEnable_CheckedChanged "); }

            }
        }

        private void Cbsimulation_CheckedChanged(object sender, EventArgs e)
        {
            Grbl.grblSimulate = cBsimulation.Checked;
            Grbl.axisA = true;
            Grbl.axisB = true;
            Grbl.axisC = true;
        }

        private void CboxPollInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.grblPollIntervalIndex = cBoxPollInterval.SelectedIndex;
        }
        private void CBoxSaveEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FCTBSaveEncodingIndex = CBoxSaveEncoding.SelectedIndex;
        }

        private void BtnCheckSpindle_Click(object sender, EventArgs e)
        {
            if (Grbl.GetSetting(30) < 0)
                MessageBox.Show("No information available - please connect grbl-controller", "Attention!");
            else
            {
                string tmp = "";
                tmp += string.Format("Current grbl Settings:\rMax spindle speed:\t$30={0}\rMin spindle speed:\t$31={1}\rLaser Mode:\t\t$32={2}", Grbl.GetSetting(30), Grbl.GetSetting(31), Grbl.GetSetting(32));
                MessageBox.Show(tmp, "Information");
            }
        }

        private void RbImportSVGGroupItem0_CheckedChanged(object sender, EventArgs e)
        {
            //   int group = Properties.Settings.Default.importGroupItem;
            if (rBImportSVGGroupItem1.Checked) Properties.Settings.Default.importGroupItem = 1;
            if (rBImportSVGGroupItem2.Checked) Properties.Settings.Default.importGroupItem = 2;
            if (rBImportSVGGroupItem3.Checked) Properties.Settings.Default.importGroupItem = 3;
            if (rBImportSVGGroupItem4.Checked) Properties.Settings.Default.importGroupItem = 4;
        }

        private void RbImportSVGSort0_CheckedChanged(object sender, EventArgs e)
        {
            if (rBImportSVGSort0.Checked)
                Properties.Settings.Default.importGroupSort = 0;
            if (rBImportSVGSort1.Checked)
                Properties.Settings.Default.importGroupSort = 1;
            if (rBImportSVGSort2.Checked)
                Properties.Settings.Default.importGroupSort = 2;
            if (rBImportSVGSort3.Checked)
                Properties.Settings.Default.importGroupSort = 3;
            if (rBImportSVGSort4.Checked)
                Properties.Settings.Default.importGroupSort = 4;
        }

        private void DgvToolList_SelectionChanged(object sender, EventArgs e)
        {
            bool enabled = (dGVToolList.SelectedRows.Count > 0);
            btnUp.Visible = enabled;
            btnDown.Visible = enabled;
        }

        private void FillToolTableFileList(string filepath)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(filepath);

                lbFiles.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("csv"))
                    {
                        string name = Path.GetFileName(Files[i]);
                        if (name != ToolTable.DefaultFileName)
                            lbFiles.Items.Add(name);
                    }
                }
            }
            catch (Exception err) { Logger.Error(err, "FillToolTableFileList "); }
        }



        private void CbToolLaser_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = !cBToolLaser.Checked;
            tab2gB2lbl1.Visible = enabled;
            nUDToolOffsetX.Visible = enabled;
            nUDToolOffsetY.Visible = enabled;
            nUDToolOffsetZ.Visible = enabled;
            btnMoveToolXY.Visible = enabled;
            //ToolNr,color,name,X,Y,Z,diameter,XYspeed,Zdeepth, Zinc, Zspeed, spindleSpeed, stepover
            dGVToolList.Columns[1].Visible = enabled;
            dGVToolList.Columns[3].Visible = enabled;
            dGVToolList.Columns[4].Visible = enabled;
            dGVToolList.Columns[5].Visible = enabled;
            dGVToolList.Columns[6].Visible = enabled;
            dGVToolList.Columns[8].Visible = enabled;
            dGVToolList.Columns[9].Visible = enabled;
            dGVToolList.Columns[10].Visible = enabled;
            dGVToolList.Columns[12].Visible = enabled;
            dGVToolList.Columns[13].Visible = enabled;
        }

        private void CbImportSVGGroup_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBImportSVGGroup.Checked;
            tab1_1_4gB2.Enabled = enable;
            tab1_1_4gB3.Enabled = enable;
        }

        private void CbToolTableUse_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBToolTableUse.Checked;
            cBImportGCTTSSpeed.Enabled = enable;
            cBImportGCTTSSpeedPWM.Enabled = enable;
            cBImportGCTTXYFeed.Enabled = enable;
            cBImportGCTTZAxis.Enabled = (enable && cBImportGCUseZ.Checked);
            CheckVisibility();
            if (cBToolTableUse.Checked)
                tab1_1gB5.BackColor = Color.Yellow;
            else
                tab1_1gB5.BackColor = Color.WhiteSmoke;
        }

        private void CbImportSVGResize_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBImportSVGResize.Checked;
            nUDSVGScale.Enabled = enable;
            lblSVGScale.Enabled = enable;
        }

        private void CheckVisibility()
        {
            bool optionUseZ = cBImportGCUseZ.Checked;
            //   bool optionUseTTVal = cBToolTableUse.Checked;

            bool enable = cBToolTableUse.Checked;
            cBImportGCTTSSpeed.Enabled = enable;
            cBImportGCTTXYFeed.Enabled = enable;
            //       cBImportGCTTZDeepth.Enabled = (enable && cBImportGCUseZ.Checked);
            cBImportGCTTZAxis.Enabled = (enable && cBImportGCUseZ.Checked);
            //       cBImportGCTTZIncrement.Enabled = (enable && cBImportGCUseZ.Checked && cBImportGCZIncEnable.Checked);
            cBToolTableDefault.Enabled = enable;
            numericUpDown2.Enabled = cBToolTableDefault.Checked && enable;

            // Use Z Group
            tab1_2lbl31.Enabled = optionUseZ;
            tab1_2lbl32.Enabled = optionUseZ;
            tab1_2lbl33.Enabled = optionUseZ;
            nUDImportGCFeedZ.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled));
            nUDImportGCZUp.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled));
            nUDImportGCZDown.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled));
            cBImportGCZIncEnable.Enabled = optionUseZ;
            //			if (cBImportGCTangential.Checked)
            //			{	cBImportGCZIncEnable.Enabled = false; cBImportGCZIncEnable.Checked=false; }
            tab1_2lbl35.Enabled = (optionUseZ && cBImportGCZIncEnable.Checked);
            cBImportGCZIncNoZUp.Enabled = cBImportGCZIncStartZero.Enabled = (optionUseZ && cBImportGCZIncEnable.Checked);
            nUDImportGCZIncrement.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled) && cBImportGCZIncEnable.Checked);

            nUDImportGCFeedXY.Enabled = !(cBImportGCTTXYFeed.Checked && cBImportGCTTXYFeed.Enabled);
            nUDImportGCSSpeed.Enabled = !(cBImportGCTTSSpeed.Checked && cBImportGCTTSSpeed.Enabled);
            nUDImportGCPWMDown.Enabled = !(cBImportGCTTSSpeed.Checked && cBImportGCTTSSpeed.Enabled);
        }

        private void CbImportGCUsePWM_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGCUsePWM.Checked;
            //tab1_2lbl41.Enabled = enable;
            //tab1_2lbl42.Enabled = enable;
            //tab1_2lbl43.Enabled = enable;
            //tab1_2lbl44.Enabled = enable;
            nUDImportGCPWMUp.Enabled = enable;
            nUDImportGCDlyUp.Enabled = enable;
            nUDImportGCPWMDown.Enabled = enable;
            nUDImportGCDlyDown.Enabled = enable;
            nUDImportGCPWMZero.Enabled = enable;
            btnGCPWMUp.Enabled = enable;
            btnGCPWMZero.Enabled = enable;
            btnGCPWMDown.Enabled = enable;
            TbImportGCPWMSlider.Enabled = enable;
            LblImportGCPWMSliderMin.Enabled = enable;
            LblImportGCPWMSliderMax.Enabled = enable;
            cBImportGCPWMSkipM30.Enabled = enable;
            cBImportGCPWMSendCode.Enabled = enable;
            HighlightPenOptions_Click(sender, e);
        }

        private void CbImportGCUseIndividual_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGCUseIndividual.Checked;
            tab1_2lbl61.Enabled = enable;
            tab1_2lbl62.Enabled = enable;
            tBImportGCIPU.Enabled = enable;
            tBImportGCIPD.Enabled = enable;
            HighlightPenOptions_Click(sender, e);
        }

        private void CbImportGCDragKnife_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGCDragKnife.Checked;
            nUDImportGCDragKnifeLength.Enabled = enable;
            nUDImportGCDragKnifePercent.Enabled = enable;
            nUDImportGCDragKnifeAngle.Enabled = enable;
            cBImportGCDragKnifePercent.Enabled = enable;
            cBImportGCDragKnifeUse.Enabled = enable;
            lblDrag1.Enabled = enable;
            lblDrag2.Enabled = enable;
            HighlightPenOptions_Click(sender, e);
            TabControl3.Invalidate();
        }

        private void CbImportGCLineSegments_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGCLineSegments.Checked;
            nUDImportGCLineSegment.Enabled = enable;
            cBImportGCLineSegmentsEquidistant.Enabled = enable;
            cBImportGCSubEnable.Enabled = enable;
            btnShowScriptSub.Enabled = enable;
            btnFileDialogSubR.Enabled = enable;
            cBImportGCSubFirst.Enabled = enable;
            tBImportGCSubroutine.Enabled = enable;
        }

        private void CbImportGCNoArcs_CheckedChanged(object sender, EventArgs e)
        {
            //            nUDImportGCSegment.Enabled = cBImportGCNoArcs.Checked;
            HighlightPenOptions_Click(sender, e);
        }

        private void FillUseCaseFileList(string Root)
        {
            try
            {
                string[] Files = System.IO.Directory.GetFiles(Root);

                lBUseCase.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("ini"))
                        lBUseCase.Items.Add(Path.GetFileName(Files[i]));
                }
            }
            catch (Exception err) { Logger.Error(err, "FillUseCaseFileList "); }
        }


        private void BtnLoadUseCase_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lBUseCase.Text))
            {
                string path = Datapath.Usecases + "\\" + lBUseCase.Text;
                var MyIni = new IniFile(path);
                Logger.Trace("Load Use Case   {0}", path);

                MyIni.ReadAll();   // ReadImport();
                                   //    UpdateIniVariables();
                FillUseCaseFileList(Datapath.Usecases);
                Properties.Settings.Default.useCaseLastLoaded = lBUseCase.Text;
                lblLastUseCase.Text = lBUseCase.Text;

                dGVToolList.CellEndEdit -= new DataGridViewCellEventHandler(DgvToolList_CellLeave);
                dGVToolList.CellLeave -= new DataGridViewCellEventHandler(DgvToolList_CellLeave);
                ImportCSVToDgv(defaultToolList);
                dGVToolList.CellEndEdit += new DataGridViewCellEventHandler(DgvToolList_CellLeave);
                dGVToolList.CellLeave += new DataGridViewCellEventHandler(DgvToolList_CellLeave);

                lblToolListLoaded.Text = Properties.Settings.Default.toolTableLastLoaded;
                if (Properties.Settings.Default.toolTableOriginal)
                {
                    lblToolListChanged.Text = "orginal";
                    lblToolListChanged.BackColor = Color.Transparent;
                }
            }
        }

        private void BtnUseCaseSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Datapath.Usecases,
                Filter = "Use cases (*.ini)|*.ini",
                FileName = "new_use_case.ini"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(sfd.FileName))
                        File.Delete(sfd.FileName);
                }
                catch (IOException err)
                {
                    EventCollector.StoreException("Error BtnUseCaseSave_Click:" + sfd.FileName);
                    Logger.Error(err, "BtnUseCaseSave_Click IOException:{0}", sfd.FileName);
                    MessageBox.Show("Could not delete old " + sfd.FileName, "Error");
                    return;
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }

                var MyIni = new IniFile(sfd.FileName);
                MyIni.WriteImport();
            }
            FillUseCaseFileList(Datapath.Usecases);
            sfd.Dispose();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            string file = lBUseCase.Text;
            if (string.IsNullOrEmpty(file))
                return;

            string fileName = Datapath.Usecases + "\\" + file;

            if (!File.Exists(fileName))
                return;

            try
            {
                File.Delete(fileName);
            }
            catch (IOException err)
            {
                EventCollector.StoreException("Error BtnDelete_Click:" + fileName);
                Logger.Error(err, "BtnDelete_Click IOException:{0}", fileName);
                MessageBox.Show("Could not delete old " + fileName, "Error");
                return;
            }
            catch
            {//(Exception err) { 
                throw;      // unknown exception...
            }
            FillUseCaseFileList(Datapath.Usecases);
        }

        private void LbUseCase_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = Datapath.Usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            tBUseCaseSetting2.Text = MyIni.ReadUseCaseInfo();
            tBUseCaseSetting1.Text = MyIni.ShowIniSettings(); ;
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool show = (tabControl_Level1.SelectedIndex == 0);
            btnReloadFile.Visible = show;
            cBshowImportDialog.Visible = show;
        }

        private void CbImportGCTangential_CheckStateChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGCTangential.Checked;
            lblDImportGCTangential1.Enabled = enable;
            lblDImportGCTangential2.Enabled = enable;
            lblDImportGCTangential3.Enabled = enable;
            lblDImportGCTangential4.Enabled = enable;
            cBoxImportGCTangentialName.Enabled = enable;
            nUDImportGCTangentialSwivel.Enabled = enable;
            nUDImportGCTangentialSwivel2.Enabled = enable;
            nUDImportGCTangentialUnits.Enabled = enable;
            cBImportGCTangentialRange.Enabled = enable;
            cBImportGCTangentialShortingEnable.Enabled = enable;
            nUDImportGCTangentialShorting.Enabled = enable;
            if (cBImportGCTangentialRange.Checked && enable)
            { cBImportGCNoArcs.Checked = true; }
            HighlightPenOptions_Click(sender, e);
            TabControl3.Invalidate();
        }

        private void CbImportGCTangentialRange_CheckStateChanged(object sender, EventArgs e)
        {
            if (cBImportGCTangentialRange.Checked)
            { cBImportGCNoArcs.Checked = true; }
            HighlightPenOptions_Click(sender, e);
        }
        private void CbImportGraphicHatchFill_CheckStateChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGraphicHatchFill.Checked || CbImportSVGApplyFill.Checked;
            lblHatchFill1.Enabled = enable;
            lblHatchFill2.Enabled = enable;
            cBImportGraphicHatchFillCross.Enabled = enable;
            cBImportGraphicHatchFillDash.Enabled = enable;

            cBImportGraphicHatchFillChangeOffset.Enabled = enable;
            nUDHatchFillDist.Enabled = enable;
            nUDHatchFillOff.Enabled = enable;

            cBImportGraphicHatchFillChangeAngle.Enabled = enable;
            nUDHatchFillAngle.Enabled = enable;
            nUDHatchFillAngle2.Enabled = enable;
            cBImportGraphicHatchFillInset.Enabled = enable;
            nUDHatchFillInset.Enabled = enable;
            cBImportGraphicHatchFillInset2.Enabled = enable;
            CbImportGraphicHatchFillNoise.Enabled = enable;
            CbImportGraphicHatchFillDeletePath.Enabled = enable;

            if (enable)
            { cBImportGCNoArcs.Checked = true; }
            HighlightPenOptions_Click(sender, e);
            TabControl3.Invalidate();

            CbImportGraphicNoise_CheckStateChanged(sender, e);
        }

        private void BtnOpenLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("notepad.exe", Datapath.LogFiles + "\\logfile.txt");
            }
            catch (Exception err)
            { MessageBox.Show("Could not open log file: \r\n" + err.Message, "Error"); }
        }

        private void CbLog1_CheckedChanged(object sender, EventArgs e)
        {
            uint val = 0;
            val += cBLogLevel1.Checked ? (uint)LogEnables.Level1 : 0;
            val += cBLogLevel2.Checked ? (uint)LogEnables.Level2 : 0;
            val += cBLogLevel3.Checked ? (uint)LogEnables.Level3 : 0;
            val += cBLogLevel4.Checked ? (uint)LogEnables.Level4 : 0;
            val += cBLog0.Checked ? (uint)LogEnables.Detailed : 0;
            val += cBLog1.Checked ? (uint)LogEnables.Coordinates : 0;
            val += cBLog2.Checked ? (uint)LogEnables.Properties : 0;
            val += cBLog3.Checked ? (uint)LogEnables.Sort : 0;
            val += cBLog4.Checked ? (uint)LogEnables.GroupAllGraphics : 0;
            val += cBLog5.Checked ? (uint)LogEnables.ClipCode : 0;
            val += cBLog6.Checked ? (uint)LogEnables.PathModification : 0;

            Properties.Settings.Default.importLoggerSettings = val;
        }

        private void CbExtendedLogging_CheckStateChanged(object sender, EventArgs e)
        {
            gBLoggingOptions.Visible = cBExtendedLogging.Checked;
        }

        private void CbImportGraphicTile_CheckedChanged(object sender, EventArgs e)
        {
            if (cBImportGraphicTile.Checked)
                gBClipping.BackColor = Color.Yellow;
            else
                gBClipping.BackColor = Color.WhiteSmoke;
        }

        private void CbPathOverlapEnable_CheckStateChanged(object sender, EventArgs e)
        {
            bool enable = cBPathOverlapEnable.Checked;
            nUDPathOverlapValue.Enabled = enable;
            if (enable)
                tab1_3gB8.BackColor = Color.Yellow;
            else
                tab1_3gB8.BackColor = Color.WhiteSmoke;

        }

        private void BtnReloadSettings_Click(object sender, EventArgs e)
        {
            settingsReloaded = true;
            Logger.Info("+++++ Set default settings +++++");
            Properties.Settings.Default.ctrlUpgradeRequired = false;
            Properties.Settings.Default.Reset();
            SetupForm_Load(sender, e);
            btnApplyChangings.PerformClick();
            SaveSettings();
        }

        private void LblEnableLogging_Click(object sender, EventArgs e)
        { groupBox3.Visible = true; }

        private void CheckZEngraveExceed()
        { }// lblImportPenWidthToZWarning.Visible = (nUDImportGCZDown.Value > Math.Min(nUDImportPenWidthToZMax.Value, nUDImportPenWidthToZMin.Value)); }
           //
        private void NudImportPenWidthToZMin_ValueChanged(object sender, EventArgs e)
        {
            //   CheckZEngraveExceed();
            LblZEngrave.Text = nUDImportGCZDown.Value.ToString("0.0");
        }

        private void BtnNotifierMail_Test_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Notifier.SendMail("Mail sent via GRBL-Plotter [Setup - Program behavior - Notifier] at " + DateTime.Now.ToString("yyyy-dd-MM h:mm:ss"), "Email"));    // in Notifier.cs
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (tbNotifierPBToken.Text.Length > 5)
                MessageBox.Show(Notifier.PushBullet("Note sent via GRBL-Plotter [Setup - Program behavior - Notifier] at " + DateTime.Now.ToString("yyyy-dd-MM h:mm:ss"), "Pushbullet"));    // in Notifier.cs
            else
                MessageBox.Show("Please enter a valid API access token");
        }


        private void TbImportGCPWMSlider_VisibleChanged(object sender, EventArgs e)
        {
            LblImportGCPWMSliderMin.Text = string.Format("$31={0}", Grbl.GetSetting(31));
            LblImportGCPWMSliderMax.Text = string.Format("$30={0}", Grbl.GetSetting(30));
            TbImportGCPWMSlider.Minimum = (int)Grbl.GetSetting(31);
            TbImportGCPWMSlider.Maximum = (int)Grbl.GetSetting(30);
        }

        private void TbImportGCPWMSlider_Scroll(object sender, EventArgs e)
        {
            if (cBImportGCUsePWM.Enabled && cBImportGCPWMSendCode.Checked)
            { commandToSend = String.Format("M{0} S{1}\r\n", "3", TbImportGCPWMSlider.Value); }
            LblImportGCPWMSliderSet.Text = string.Format("{0}", TbImportGCPWMSlider.Value);
            LblImportGCPWMSliderSet.Top = 60 + (int)(140 * (1 - (float)TbImportGCPWMSlider.Value / (float)TbImportGCPWMSlider.Maximum));
        }

        private void NudImportGCPWMUp_ValueChanged(object sender, EventArgs e)      // send PWM Pen up code
        { btnGCPWMUp.PerformClick(); }
        private void NudImportGCPWMDown_ValueChanged(object sender, EventArgs e)    // send PWM Pen up code
        { btnGCPWMDown.PerformClick(); }
        private void NudImportGCPWMZero_ValueChanged(object sender, EventArgs e)
        { btnGCPWMZero.PerformClick(); }
        private void NudImportGCPWMP93_ValueChanged(object sender, EventArgs e)
        {
            if (cBImportGCUsePWM.Enabled && cBImportGCPWMSendCode.Checked)
            { commandToSend = String.Format("M{0} S{1}\r\n", "3", nUDImportGCPWMP93.Value); }
        }
        private void NudImportGCPWMP94_ValueChanged(object sender, EventArgs e)
        {
            if (cBImportGCUsePWM.Enabled && cBImportGCPWMSendCode.Checked)
            { commandToSend = String.Format("M{0} S{1}\r\n", "3", nUDImportGCPWMP94.Value); }
        }

        // Event handler must be assigned in GRBL-Plotter\GUI\MainForm.cs\MainFormOtherForms.cs - (483, 29) : _setup_form.btnGCPWMDown.Click += moveToPickup;
        private void BtnGCPWMUp_Click(object sender, EventArgs e)
        {
            SetZeroMinMax();
            if (cBImportGCUsePWM.Enabled && cBImportGCPWMSendCode.Checked)
            { commandToSend = String.Format("M{0} S{1}\r\n", "3", nUDImportGCPWMUp.Value); }
            if ((nUDImportGCPWMUp.Value >= TbImportGCPWMSlider.Minimum) && (nUDImportGCPWMUp.Value <= TbImportGCPWMSlider.Maximum))
                TbImportGCPWMSlider.Value = (int)nUDImportGCPWMUp.Value;
        }
        private void BtnGCPWMDown_Click(object sender, EventArgs e)
        {
            SetZeroMinMax();
            if (cBImportGCUsePWM.Enabled && cBImportGCPWMSendCode.Checked)
            { commandToSend = String.Format("M{0} S{1}\r\n", "3", nUDImportGCPWMDown.Value); }
            if ((nUDImportGCPWMDown.Value >= TbImportGCPWMSlider.Minimum) && (nUDImportGCPWMDown.Value <= TbImportGCPWMSlider.Maximum))
                TbImportGCPWMSlider.Value = (int)nUDImportGCPWMDown.Value;
        }
        private void BtnGCPWMZero_Click(object sender, EventArgs e)
        {
            if (cBImportGCUsePWM.Enabled && cBImportGCPWMSendCode.Checked)
            { commandToSend = String.Format("M{0} S{1}\r\n", "3", nUDImportGCPWMZero.Value); }
            if ((nUDImportGCPWMZero.Value >= TbImportGCPWMSlider.Minimum) && (nUDImportGCPWMZero.Value <= TbImportGCPWMSlider.Maximum))
                TbImportGCPWMSlider.Value = (int)nUDImportGCPWMZero.Value;
        }
        private void SetZeroMinMax()
        {   //nUDImportGCPWMZero.Value = (nUDImportGCPWMUp.Value + nUDImportGCPWMDown.Value) / 2;
            try
            {
                decimal max = Math.Max(nUDImportGCPWMUp.Value, nUDImportGCPWMDown.Value);
                decimal min = Math.Min(nUDImportGCPWMUp.Value, nUDImportGCPWMDown.Value);
                if (nUDImportGCPWMZero.Value > max) nUDImportGCPWMZero.Value = max;
                if (nUDImportGCPWMZero.Value < min) nUDImportGCPWMZero.Value = min;
                nUDImportGCPWMZero.Maximum = max;
                nUDImportGCPWMZero.Minimum = min;
            }
            catch (Exception err)
            {
                Logger.Error(err, "SetZeroMinMax ");
            }
        }

        private void CbImportGCPWMSendCode_CheckedChanged(object sender, EventArgs e)
        {
            Color tmpColor = Control.DefaultBackColor; //SystemColors.Window;
            if (cBImportGCPWMSendCode.Checked)
            { tmpColor = Color.Orange; }
            nUDImportGCPWMUp.BackColor = tmpColor;
            nUDImportGCPWMDown.BackColor = tmpColor;
            nUDImportGCPWMZero.BackColor = tmpColor;
            nUDImportGCPWMP93.BackColor = tmpColor;
            nUDImportGCPWMP94.BackColor = tmpColor;
            btnGCPWMUp.BackColor = tmpColor;
            btnGCPWMDown.BackColor = tmpColor;
            btnGCPWMZero.BackColor = tmpColor;
            cBImportGCPWMSendCode.BackColor = tmpColor;
            TbImportGCPWMSlider.BackColor = tmpColor;
        }

        private void LblInfoPWM_Click(object sender, EventArgs e)
        { MessageBox.Show(toolTip1.GetToolTip(lblInfoPWM), "Info"); }

        private int PWMIncValue = 1;
        private void BtnPWMInc_Click(object sender, EventArgs e)
        {
            if (PWMIncValue <= 1)
            { PWMIncValue = 10; }
            else if (PWMIncValue == 10)
            { PWMIncValue = 100; }
            else if (PWMIncValue >= 100)
            { PWMIncValue = 1; }
            btnPWMInc.Text = "Inc. " + PWMIncValue.ToString();
            SetPWMIncValues(PWMIncValue);
            Properties.Settings.Default.setupPWMIncrement = PWMIncValue;
        }
        private void SetPWMIncValues(int inc)
        {
            nUDImportGCPWMUp.Increment = inc;
            nUDImportGCPWMZero.Increment = inc;
            nUDImportGCPWMDown.Increment = inc;
        }

        private bool pwmAdvanced = false;
        private void BtnPWMAdvanced_Click(object sender, EventArgs e)
        {
            pwmAdvanced = !pwmAdvanced;
            TbImportGCPWMSlider.Visible = LblImportGCPWMSliderMin.Visible = LblImportGCPWMSliderMax.Visible = LblImportGCPWMSliderSet.Visible = pwmAdvanced;
            lblPWMP91.Visible = lblPWMP93.Visible = lblPWMP94.Visible = pwmAdvanced;
            btnGCPWMZero.Visible = tBImportGCPWMTextP93.Visible = tBImportGCPWMTextP94.Visible = pwmAdvanced;
            nUDImportGCPWMZero.Visible = nUDImportGCPWMP93.Visible = nUDImportGCPWMP94.Visible = pwmAdvanced;
            nUDImportGCDlyP93.Visible = nUDImportGCDlyP94.Visible = pwmAdvanced;
        }
        private void CbWireBenderEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (CbWireBenderEnable.Checked)
                GbWireBender.BackColor = Color.Yellow;
            else
                GbWireBender.BackColor = Color.WhiteSmoke;
        }

        private void CbImportGraphicDevelopEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (cBImportGraphicDevelopEnable.Checked)
                gBDevelop.BackColor = Color.Yellow;
            else
                gBDevelop.BackColor = Color.WhiteSmoke;
        }

        private void CbImportGraphicDevelopNoCurve_CheckedChanged(object sender, EventArgs e)
        {
            NudImportGraphicDevelopNotchDistance.Enabled = !CbImportGraphicDevelopNoCurve.Checked;
            LblImportGraphicDevelopNotchDistance.Enabled = !CbImportGraphicDevelopNoCurve.Checked;
        }

        private void BtnThrow_Click(object sender, EventArgs e)
        {
            throw new InvalidOperationException("Just test");
        }

        private void BtnAccessorySpindleReset_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.grblRunTimeSpindle = 0;
            Properties.Settings.Default.Save();
            LblAccessorySpindleVal.Text = Properties.Settings.Default.grblRunTimeSpindle.ToString();
        }

        private void BtnAccessoryFloodReset_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.grblRunTimeFlood = 0;
            Properties.Settings.Default.Save();
            LblAccessoryFloodVal.Text = Properties.Settings.Default.grblRunTimeFlood.ToString();
        }

        private void BtnAccessoryMistReset_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.grblRunTimeMist = 0;
            Properties.Settings.Default.Save();
            LblAccessoryMistVal.Text = Properties.Settings.Default.grblRunTimeMist.ToString();
        }

        private void BtnAccessoryRefresh_Click(object sender, EventArgs e)
        {
            LblAccessorySpindleVal.Text = Properties.Settings.Default.grblRunTimeSpindle.ToString("F0");
            LblAccessoryFloodVal.Text = Properties.Settings.Default.grblRunTimeFlood.ToString("F0");
            LblAccessoryMistVal.Text = Properties.Settings.Default.grblRunTimeMist.ToString("F0");
        }

        private void CbAux1ZMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.importGCAux1ZMode = CbAux1ZMode.SelectedIndex;
        }

        private void CbAux2ZMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.importGCAux2ZMode = CbAux2ZMode.SelectedIndex;
        }

        private void BtnProjectorColorBackground_Click(object sender, EventArgs e)
        { ApplyColor(BtnProjectorColorBackground, "projectorColorBackground"); btnApplyChangings.PerformClick(); }

        private void BtnProjectorColorDimension_Click(object sender, EventArgs e)
        { ApplyColor(BtnProjectorColorDimension, "projectorColorDimension"); btnApplyChangings.PerformClick(); }

        private void BtnProjectorColorRuler_Click(object sender, EventArgs e)
        { ApplyColor(BtnProjectorColorRuler, "projectorColorRuler"); btnApplyChangings.PerformClick(); }

        private void BtnProjectorColorPenUp_Click(object sender, EventArgs e)
        { ApplyColor(BtnProjectorColorPenUp, "projectorColorPenUp"); btnApplyChangings.PerformClick(); }

        private void BtnProjectorColorPenDown_Click(object sender, EventArgs e)
        { ApplyColor(BtnProjectorColorPenDown, "projectorColorPenDown"); btnApplyChangings.PerformClick(); }

        private void BtnProjectorColorTool_Click(object sender, EventArgs e)
        { ApplyColor(BtnProjectorColorTool, "projectorColorTool"); btnApplyChangings.PerformClick(); }

        private void BtnProjectorColorMarker_Click(object sender, EventArgs e)
        { ApplyColor(BtnProjectorColorMarker, "projectorColorMarker"); btnApplyChangings.PerformClick(); }

        private void SetProjectorButtons()
        {
            SetButtonColors(BtnProjectorColorBackground, Properties.Settings.Default.projectorColorBackground);
            SetButtonColors(BtnProjectorColorDimension, Properties.Settings.Default.projectorColorDimension);
            SetButtonColors(BtnProjectorColorRuler, Properties.Settings.Default.projectorColorRuler);
            SetButtonColors(BtnProjectorColorPenUp, Properties.Settings.Default.projectorColorPenUp);
            SetButtonColors(BtnProjectorColorPenDown, Properties.Settings.Default.projectorColorPenDown);
            SetButtonColors(BtnProjectorColorTool, Properties.Settings.Default.projectorColorTool);
            SetButtonColors(BtnProjectorColorMarker, Properties.Settings.Default.projectorColorMarker);
        }

        private void BtnProjectorCalc_Click(object sender, EventArgs e)
        {
            decimal scaling = NudProjectorSet.Value / NudProjectorReal.Value;
            NudProjectorScaling.Value *= scaling;
            NudProjectorReal.Value = NudProjectorSet.Value;
        }

        private void CbProjectorScaleEnable_CheckedChanged(object sender, EventArgs e)
        {
            GbProjectorScale.Enabled = CbProjectorScaleEnable.Checked;
        }

        private void BtnLoadToolTableDialog_Click(object sender, EventArgs e)
        {

        }

        private void BtnImportGraphicClipGetDim_Click(object sender, EventArgs e)
        {
            if (VisuGCode.clipDimension.X > 0)
                nUDImportGraphicClipWidth.Value = (decimal)VisuGCode.clipDimension.X;
            if (VisuGCode.clipDimension.Y > 0)
                nUDImportGraphicClipHeight.Value = (decimal)VisuGCode.clipDimension.Y;
            Logger.Trace("Setup clipDimension {0:0.00} {1:0.00}", VisuGCode.clipDimension.X, VisuGCode.clipDimension.Y);
        }

        private void BtnImportGraphicClipGetOff_Click(object sender, EventArgs e)
        {
            if (VisuGCode.clipOffset.X >= 0)
                nUDImportGraphicClipOffsetX.Value = (decimal)VisuGCode.clipOffset.X;
            if (VisuGCode.clipOffset.Y >= 0)
                nUDImportGraphicClipOffsetY.Value = (decimal)VisuGCode.clipOffset.Y;
            Logger.Trace("Setup clipOffset {0:0.00} {1:0.00}", VisuGCode.clipOffset.X, VisuGCode.clipOffset.Y);
        }

        private void CbimportGraphicFilterEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (CbimportGraphicFilterEnable.Checked)
                GbFilter.BackColor = Color.Yellow;
            else
                GbFilter.BackColor = Color.WhiteSmoke;
        }

        private void BtnGetFilterValueRemove_Click(object sender, EventArgs e)
        {
            string tmp;
            Button clickedButton = sender as Button;
            if (clickedButton.Name == "BtnGetFilterValueRemoveWidth")      //(rBImportSVGGroupItem2.Checked)
                tmp = lastMarkedWidth;
            else
                tmp = lastMarkedColor;
            if (!string.IsNullOrEmpty(tmp) && (tmp != "0"))
                TbimportGraphicFilterListRemove.Text += tmp + ";";
            Logger.Trace("Setup BtnGetFilterValue Remove '{0}'", tmp);
        }

        private void BtnGetFilterValueKeep_Click(object sender, EventArgs e)
        {
            string tmp;
            Button clickedButton = sender as Button;
            if (clickedButton.Name == "BtnGetFilterValueKeepWidth")       //(rBImportSVGGroupItem2.Checked)
                tmp = lastMarkedWidth;
            else
                tmp = lastMarkedColor;
            if (!string.IsNullOrEmpty(tmp) && (tmp != "0"))
                TbimportGraphicFilterListKeep.Text += tmp + ";";
            Logger.Trace("Setup BtnGetFilterValue Keep '{0}'", tmp);
        }

        private void BtnGetFilterValueClear_Click(object sender, EventArgs e)
        {
            TbimportGraphicFilterListRemove.Text = TbimportGraphicFilterListKeep.Text = "";
        }

        private void BtnGetFilterLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Datapath.Filter,
                Filter = "Filter list|*.ini",
                Title = "Load filter list "
            };
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var MyIni = new IniFile(openFileDialog1.FileName);
                Logger.Trace("Load Filter list   {0}", openFileDialog1.FileName);
                MyIni.ReadImport();   // ReadAll();
            }
            openFileDialog1.Dispose();

            if (Properties.Settings.Default.importGraphicFilterChoiceRemove)
                RbimportGraphicFilterChoiceRemove1.Checked = true;
            else
                RbimportGraphicFilterChoiceRemove2.Checked = true;
        }

        private void BtnGetFilterSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Datapath.Filter,
                Filter = "Filter list|*.ini",
                FileName = "Filter.ini"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(sfd.FileName))
                        File.Delete(sfd.FileName);
                }
                catch (IOException err)
                {
                    EventCollector.StoreException("Error BtnGetFilterSave_Click:" + sfd.FileName);
                    Logger.Error(err, "BtnGetFilterSave_Click IOException:{0}", sfd.FileName);
                    MessageBox.Show("Could not delete old " + sfd.FileName, "Error");
                    return;
                }
                catch
                {//(Exception err) { 
                    throw;      // unknown exception...
                }

                string txt = "[Graphics Import]\r\n";
                txt += "Filter remove=" + Properties.Settings.Default.importGraphicFilterChoiceRemove.ToString() + "\r\n";
                txt += "Filter list remove=" + Properties.Settings.Default.importGraphicFilterListRemove + "\r\n";
                txt += "Filter list keep=" + Properties.Settings.Default.importGraphicFilterListKeep + "\r\n";
                File.WriteAllText(sfd.FileName, txt);
            }
            sfd.Dispose();
        }

        private void TabControl2_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabControl1_Level2.TabPages[e.Index];
            if (!e.Bounds.IsEmpty)
            {
                if (IsOptionEnabledTabControl2(e.Index))
                    e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush((e.State == DrawItemState.Selected) ? Color.White : page.BackColor), e.Bounds);
            }
            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, page.ForeColor);
        }
        private bool IsOptionEnabledTabControl2(int tabIndex)
        {
            var prop = Properties.Settings.Default;
            switch (tabIndex)
            {
                case 0:
                    return (prop.importSVGApplyFill || prop.importSVGPathNewFigure || prop.importGraphicOffsetLargestRemove);
                    break;
                case 1:
                    return (prop.importGCZEnable && prop.importGCZIncEnable && prop.importGCZIncNoZUp);
                    break;
                case 2:
                    return (prop.importGCLineSegmentation || prop.importGCRelative || prop.importGCConvertToPolar);// || prop.importGraphicLeadOutEnable);
                    break;
                default:
                    break;
            }
            return false;
        }

        private void TabControl3_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = TabControl3.TabPages[e.Index];
            if (!e.Bounds.IsEmpty)
            {
                if (IsOptionEnabledTabControl3(e.Index))
                    e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush((e.State == DrawItemState.Selected) ? Color.White : page.BackColor), e.Bounds);
            }
            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, page.ForeColor);
        }

        private bool IsOptionEnabledTabControl3(int tabIndex)
        {
            var prop = Properties.Settings.Default;
            bool result = false;
            switch (tabIndex)
            {
                case 0:
                    result = false;
                    break;
                case 1:
                    result = (prop.importSVGNodesOnly || prop.importSVGCircleToDot || prop.importDepthFromWidth || prop.importDepthFromWidthRamp || prop.importSVGCircleToDotZ || prop.importPWMFromWidth || prop.importSVGCircleToDotS);
                    break;
                case 2:
                    result = (prop.importGraphicAddFrameEnable || prop.importGraphicMultiplyGraphicsEnable || prop.importGraphicLeadInEnable);// || prop.importGraphicLeadOutEnable);
                    break;
                case 3:
                    result = (prop.importGCDragKnifeEnable || prop.importGCTangentialEnable || prop.importGraphicHatchFillEnable || prop.importGraphicExtendPathEnable || prop.importGraphicNoiseEnable);
                    break;
                case 4:
                    result = (prop.importGraphicClipEnable);
                    break;
                case 5:
                    result = (prop.importGCToolTableUse);
                    break;
                case 6:
                    result = (prop.importGraphicFilterEnable);
                    break;
                case 7:
                    result = (prop.importGraphicDevelopmentFeedInvert || prop.importGraphicWireBenderEnable);
                    break;
                case 8:
                    result = (prop.importGCAux1Enable || prop.importGCAux2Enable);
                    break;
                default:
                    break;
            }
            //    Logger.Trace("IsOptionEnabledTabControl3  index:{0} result:{1}", tabIndex, result);

            return result;
        }

        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabControl1.TabPages[e.Index];
            if (!e.Bounds.IsEmpty)
            {
                if (IsOptionEnabledTabControl1(e.Index))
                    e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), e.Bounds);
                else
                    e.Graphics.FillRectangle(new SolidBrush((e.State == DrawItemState.Selected) ? Color.White : page.BackColor), e.Bounds);
            }
            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, page.ForeColor);
        }

        private bool IsOptionEnabledTabControl1(int tabIndex)
        {
            var prop = Properties.Settings.Default;
            switch (tabIndex)
            {
                case 0:
                    return (prop.importGCZEnable);
                    break;
                case 1:
                    return (prop.importGCPWMEnable);
                    break;
                case 2:
                    return (prop.importGCSpindleToggle);
                    break;
                case 3:
                    return (prop.importGCIndEnable);
                    break;
                default:
                    break;
            }
            return false;
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string url = "https://grbl-plotter.de/index.php?";
            try
            {
                Button clickedLink = sender as Button;
                Process.Start(url + clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "BtnHelp_Click ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }

        private void CbImportGraphicHatchFillInset_CheckedChanged(object sender, EventArgs e)
        {
            cBImportGraphicHatchFillInset2.Enabled = cBImportGraphicHatchFillInset.Enabled && cBImportGraphicHatchFillInset.Checked;
        }

        private void RbMultipleLoadLimitNo_CheckedChanged(object sender, EventArgs e)
        {
            bool check = RbMultipleLoadLimitNo.Checked;
            nUDMultipleLoadNoX.Enabled = nUDMultipleLoadNoY.Enabled = check;
            nUDMultipleLoadDimX.Enabled = nUDMultipleLoadDimY.Enabled = !check;
        }

        private void BtnResize1_Click(object sender, EventArgs e)
        {
            string tag = (string)((Button)sender).Tag;
            if (tag != "")
            {
                var size = tag.Split(';');
                if (size.Length > 1)
                {
                    try
                    {
                        int x = Int32.Parse(size[0]);
                        int y = Int32.Parse(size[1]);
                        Properties.Settings.Default.mainFormSize = new Size(x, y);
                        Properties.Settings.Default.Save();
                        settingsReloaded = true;
                        btnApplyChangings.PerformClick();
                        Logger.Info("Resize {0} {1} {2}", tag, x, y);
                    }
                    catch { }
                }
            }
        }

        private void NudImportGraphicOffsetOriginX_ValueChanged(object sender, EventArgs e)
        {
            GuiVariables.offsetOriginX = (double)NudImportGraphicOffsetOriginX.Value;   //Properties.Settings.Default.importGraphicOffsetOriginX;
            GuiVariables.offsetOriginY = (double)NudImportGraphicOffsetOriginY.Value;   //Properties.Settings.Default.importGraphicOffsetOriginY;

        }

        private void BtnImportSVGSelectFrameFile_Click(object sender, EventArgs e)
        {
            SetFilePath(TbImportSVGAddOnFile, "SVG (*.svg)|*.svg");
        }

        private void CbImportSVGAddOnPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.importSVGAddOnPosition = CbImportSVGAddOnPosition.SelectedIndex;
        }

        private void BtnSetSValues_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.importImageSMin = Properties.Settings.Default.importGCPWMZero;
            Properties.Settings.Default.importImageSMax = Properties.Settings.Default.importGCPWMDown;
        }

        private void CbImportGraphicNoise_CheckStateChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGraphicNoise.Checked || (CbImportGraphicHatchFillNoise.Enabled && CbImportGraphicHatchFillNoise.Checked);
            NudNoiseAmplitude.Enabled = NudNoiseDensity.Enabled = enable;
            if (cBImportGraphicNoise.Checked)
                gBNoise.BackColor = Color.Yellow;
            else
                gBNoise.BackColor = Color.WhiteSmoke;
        }

        private void BtnSaveIni_Click(object sender, EventArgs e)
        {
            try
            {
                Button clickedIni = sender as Button;
                string section = clickedIni.Tag.ToString();

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Machine Ini files (*.ini)|*.ini",
                    FileName = "SetupImport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".ini"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var MyIni = new IniFile(sfd.FileName);
                    if (section == "Gcode generation")
                        MyIni.WriteSection(IniFile.sectionSetupGcodeGeneration);
                    else if (section == "Graphics Import")
                        MyIni.WriteSection(IniFile.sectionSetupImportParameter);
                    else if (section == "SvgDxfCsv")
                        MyIni.WriteSection(IniFile.sectionSetupSvgDxfCsv);
                    Logger.Info("Save machine parameters as '{0}' {1}", section, sfd.FileName);
                }
                sfd.Dispose();
            }
            catch (Exception err)
            {
                EventCollector.StoreException("BtnSaveIni_ImportParameter_Click " + err.Message);
                Logger.Error(err, "BtnSaveIni_ImportParameter_Click ");
                MessageBox.Show("SaveMachineParameters: \r\n" + err.Message, "Error");
            }
        }

        private void CbImportGraphicSortDistanceStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.importGraphicSortDistanceStart = CbImportGraphicSortDistanceStart.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void CbImportGraphicSortDistance_CheckStateChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGraphicSortDistance.Checked;
            CbImportGraphicSortDistanceStart.Enabled = enable;
            lblSorting.Enabled = enable;
            cBImportGraphicSortDistanceRotatePath.Enabled = enable;
            cBImportGraphicLargestLast.Enabled = enable;
        }

        private void CbImportGraphicOffsetOrigin_CheckStateChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGraphicOffsetOrigin.Checked;
            NudImportGraphicOffsetOriginX.Enabled = enable;
            NudImportGraphicOffsetOriginY.Enabled = enable;
            cBImportGraphicOffsetLargestLast.Enabled = enable;
            cBImportGraphicOffsetLargestRemove.Enabled = enable;
        }

        private void CheckBox15_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void BtnSetPoTraceDefaults_Click(object sender, EventArgs e)
        {
            NudImportPoTraceTurdsize.Value = 2;
            NudImportPoTraceAlphamax.Value = (decimal)1.0;
            NudImportPoTraceOptotolerance.Value = (decimal)0.2;
            CBImportPoTraceOptimazion.Checked = true;
        }

        private void CbToolChangeM6PassThrough_CheckedChanged(object sender, EventArgs e)
        {
            gBToolChange.Enabled = !CbToolChangeM6PassThrough.Checked;
            CbToolChangeM6PassThrough.BackColor = CbToolChangeM6PassThrough.Checked ? Color.Yellow : SystemColors.Control;
        }

        private void BtnSetGrblCustomString_Click(object sender, EventArgs e)
        {
            commandToSend = String.Format("$I={0};RST", TbGrblCustomString.Text);
        }

        private void BtnSetGrblCustomStringIniFile_Click(object sender, EventArgs e)
        {
            try
            {
                string fname = TbGrblCustomString.Text;
                if (fname == "") return;

                string section = "Info";
                string localDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string iniPath = Datapath.Usecases + "\\" + fname + ".ini";
                var MyIni = new IniFile(iniPath);

                if (!File.Exists(iniPath))  // Write entry manually to force UTF-16 encoding
                {
                    string myunicode = string.Format("[{0}]\r\nDate={1}\r\nUse case info={2}", section, localDate, fname);
                    File.WriteAllText(iniPath, myunicode, Encoding.Unicode);
                }

                MyIni.Write("Date", localDate, section);
                //    MyIni.Write("Use case info", fname);
                MyIni.Write("Set Defaults", "True", section);
                MyIni.WriteSection(IniFile.sectionSetupGcodeGeneration);
                MyIni.WriteSection(IniFile.sectionSetupMachineLimits);
                var setup = Properties.Settings.Default;
                MyIni.Write("PauseCode Enable", setup.flowControlEnable.ToString(), "Flow Control");
                MyIni.Write("PauseCode Code", setup.flowControlText.ToString(), "Flow Control");

                //   MyIni.WriteButtons();
                //      MyIni.WriteJoystick();
                MyIni.WriteGrblSetting();
                Logger.Info("Save machine parameters as '{0}' {1}", "SetupGcodeGeneration", iniPath);
            }
            catch (Exception err)
            {
                EventCollector.StoreException("BtnSetGrblCustomStringIniFile_Click " + err.Message);
                Logger.Error(err, "BtnSetGrblCustomStringIniFile_Click ");
                MessageBox.Show("SaveMachineParameters: \r\n" + err.Message, "Error");
            }
        }

        MessageForm _message_form = null;
        private void BtnTestGrblCustomStringIniFile_Click(object sender, EventArgs e)
        {
            string fname = TbGrblCustomString.Text;
            if (fname == "") return;

            string path = Datapath.Usecases + "\\" + fname + ".ini";
            if (!File.Exists(path))
            {
                Logger.Trace("⚠⚠⚠ BtnTestGrblCustomStringIniFile - FAIL ini-file not found: '{0}'", path);
                MessageBox.Show("Error", "File not found");
                return;
            }
            var MyIni = new IniFile(path);

            if (_message_form != null)
            {
                _message_form.Close();
                _message_form = null;
            }
            if (true)
            {
                uint duration = 5;
                _message_form = new MessageForm();
                _message_form.Show();

                if (_message_form != null)
                {
                    string html = MyIni.ShowIniMachineSettingsHTML("Machine defaults");
                    _message_form.DontClose = false;
                    _message_form.ShowMessage(600, 800, "Saved Machine Defaults", html, (int)duration);     // show graphic import options
                }
            }
        }

        private void BtnOpenGrblCustomStringIniFile_Click(object sender, EventArgs e)
        {
            string fname = TbGrblCustomString.Text;
            if (fname == "") return;
            string iniPath = Datapath.Usecases + "\\" + fname + ".ini";
            Process.Start("notepad.exe", iniPath);
        }

        private void NudInfoTextSize1_ValueChanged(object sender, EventArgs e)
        {
            BtnApply2DViewChanges.PerformClick();
        }

        private void BtnApply2DViewChanges_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}

