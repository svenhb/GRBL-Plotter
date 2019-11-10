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
 * 2017-01-01 check form-location and fix strange location
 * 2019-03-09 add color, width, X/Y and invert for pathRotaryInfo to show rotary over X or Y
 * 2019-08-15 add logger
 * 2019-10-25 remove icon to reduce resx size, load icon on run-time
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
//using System.Windows.Input;

namespace GRBL_Plotter
{
    public partial class ControlSetupForm : Form
    {
        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
  //      private const string toolTableDefaultName = "current.csv";

        public ControlSetupForm()
        {
            Logger.Trace("++++++ ControlSetupForm START ++++++");
            this.Icon = Properties.Resources.Icon;
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.guiLanguage);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        public void setLastLoadedFile(string text)
        { toolTip1.SetToolTip(btnReloadFile, text); }

        private string defaultToolList = "tools.csv";
        private void SetupForm_Load(object sender, EventArgs e)
        {
            defaultToolList = System.Windows.Forms.Application.StartupPath + datapath.tools + "\\" + toolTable.defaultFileName;
            if ((!ImportCSVToDgv(defaultToolList)) || (dGVToolList.Rows.Count == 1))
            {
                string[] tmp = toolTable.defaultTool;  // { "1", "000000", "Black", "0.0", "0.0", "0.0", "3.0", "500" };
                dGVToolList.Rows.Add(tmp);
            }
            fillToolTableFileList(Application.StartupPath + datapath.tools);
            fillUseCaseFileList(Application.StartupPath + datapath.usecases);
            lblToolListLoaded.Text = Properties.Settings.Default.toolTableLastLoaded;

            string text;
            string[] parts;// = new string[] { "-", "(-)" };
            dGVCustomBtn.Rows.Clear();
            int row = 0;
            for (int i = 1; i <= 32; i++)
            {
                parts = new string[2];
                text = Properties.Settings.Default["guiCustomBtn" + i.ToString()].ToString();
                if (text.IndexOf('|') > 0)
                { parts = text.Split('|'); }
                else
                { parts[0] = " ";parts[1] = " "; }
                dGVCustomBtn.Rows.Add();
                dGVCustomBtn.Rows[row].Cells[0].Value = i.ToString();
                dGVCustomBtn.Rows[row].Cells[1].Value = parts[0];
                dGVCustomBtn.Rows[row].Cells[2].Value = parts[1];
                row++;
             }

         //   lvCustomButtons.Items[0].Selected = true;
            setButtonColors(btnColorBackground, Properties.Settings.Default.gui2DColorBackground);
            setButtonColors(btnColorRuler, Properties.Settings.Default.gui2DColorRuler);
            setButtonColors(btnColorPenUp, Properties.Settings.Default.gui2DColorPenUp);
            setButtonColors(btnColorPenDown, Properties.Settings.Default.gui2DColorPenDown);
            setButtonColors(btnColorTool, Properties.Settings.Default.gui2DColorTool);
            setButtonColors(btnColorMarker, Properties.Settings.Default.gui2DColorMarker);
            setButtonColors(btnColorHeightMap, Properties.Settings.Default.gui2DColorHeightMap);
            setButtonColors(btnColorMachineLimit, Properties.Settings.Default.gui2DColorMachineLimit);
            nUDImportDecPlaces.Value = Properties.Settings.Default.importGCDecPlaces;

            Location = Properties.Settings.Default.locationSetForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }

            //if (cBImportSVGSort0.Checked)
            int sort = Properties.Settings.Default.importGroupSort;
            rBImportSVGSort0.Checked = (sort == 0);
            rBImportSVGSort1.Checked = (sort == 1);
            rBImportSVGSort2.Checked = (sort == 2);

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

            lblFilePath.Text = System.Windows.Forms.Application.StartupPath;

            hsFilterScrollSetLabels();

            setLabelParameterSet(1, Properties.Settings.Default.camShapeSet1);
            setLabelParameterSet(2, Properties.Settings.Default.camShapeSet2);
            setLabelParameterSet(3, Properties.Settings.Default.camShapeSet3);
            setLabelParameterSet(4, Properties.Settings.Default.camShapeSet4);

            cBImportGCTool_CheckedChanged(sender, e);

            dGVToolList.SortCompare += new DataGridViewSortCompareEventHandler(this.dGV_SortColor);

            listHotkeys();
            lblJoystickSize.Text = hScrollBar1.Value.ToString();

            cBoxPollInterval.SelectedIndex = Properties.Settings.Default.grblPollIntervalIndex;

            checkVisibility();
            cBImportSVGGroup_CheckedChanged(sender, e);
            cBToolTableUse_CheckedChanged(sender, e);
            cBImportSVGResize_CheckedChanged(sender, e);
            cBImportGCUsePWM_CheckedChanged(sender, e);
            cBImportGCUseIndividual_CheckedChanged(sender, e);
            cBImportGCDragKnife_CheckedChanged(sender, e);
            cBImportGCLineSegments_CheckedChanged(sender, e);
            cBImportGCNoArcs_CheckedChanged(sender, e);
        }

        private void saveSettings()
        {
            for (int i = 1; i <= 32; i++)
            {
                try { Properties.Settings.Default["guiCustomBtn" + i.ToString()] = dGVCustomBtn.Rows[i - 1].Cells[1].Value + "|" + dGVCustomBtn.Rows[i - 1].Cells[2].Value; }
                catch { Properties.Settings.Default["guiCustomBtn" + i.ToString()] = " | "; }
            }
            Properties.Settings.Default.importGCDecPlaces = nUDImportDecPlaces.Value;
            Properties.Settings.Default.importGCSpindleCmd = rBImportGCSpindleCmd1.Checked;
            Properties.Settings.Default.ctrlReplaceM3 = rBCtrlReplaceM3.Checked;
            Properties.Settings.Default.rotarySubstitutionX = rBRotaryX.Checked;

            Properties.Settings.Default.Save();

            ExportDgvToCSV(defaultToolList);
        }
        
        private void btnApplyChangings_Click(object sender, EventArgs e)
        {   saveSettings();  }

        private void btnColorBackground_Click(object sender, EventArgs e)
        { applyColor(btnColorBackground, "colorBackground"); }
        private void btnColorRuler_Click(object sender, EventArgs e)
        { applyColor(btnColorRuler, "colorRuler"); }
        private void btnColorPenUp_Click(object sender, EventArgs e)
        { applyColor(btnColorPenUp, "colorPenUp"); }
        private void btnColorPenDown_Click(object sender, EventArgs e)
        { applyColor(btnColorPenDown, "colorPenDown"); }
        private void btnColorRotaryInfo_Click(object sender, EventArgs e)
        { applyColor(btnColorRotaryInfo, "colorRotaryInfo"); }
        private void btnColorTool_Click(object sender, EventArgs e)
        { applyColor(btnColorTool, "colorTool"); }
        private void btnColorMarker_Click(object sender, EventArgs e)
        { applyColor(btnColorMarker, "colorMarker"); }
        private void btnColorHeightMap_Click(object sender, EventArgs e)
        { applyColor(btnColorHeightMap, "colorHeightMap"); }
        private void btnColorMachineLimit_Click(object sender, EventArgs e)
        { applyColor(btnColorMachineLimit, "colorMachineLimit"); }

        private void applyColor(Button btn, string settings)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                setButtonColors(btn, colorDialog1.Color);
                Properties.Settings.Default[settings] = colorDialog1.Color;
                saveSettings();
            }
        }
        private void setButtonColors(Button btn, Color col)
        {
            btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
        }
        private Color ContrastColor(Color color)
        {
            int d = 0;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }

        private void btnReloadFile_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Trace("++++++ ControlSetupForm STOP ++++++");
            Properties.Settings.Default.locationSetForm = Location;
            saveSettings();
        }

        private void nUDImportDecPlaces_ValueChanged(object sender, EventArgs e)
        {
            saveSettings();
            gcode.setDecimalPlaces((int)nUDImportDecPlaces.Value);
        }

        private void btnJoyXYCalc_Click(object sender, EventArgs e)
        {
            double time = 0.5;
            double correct = 1;
            nUDJoyXYSpeed1.Value = (decimal)((double)nUDJoyXYStep1.Value / time * 60 * correct);
            nUDJoyXYSpeed2.Value = (decimal)((double)nUDJoyXYStep2.Value / time * 60 * correct);
            nUDJoyXYSpeed3.Value = (decimal)((double)nUDJoyXYStep3.Value / time * 60 * correct);
            nUDJoyXYSpeed4.Value = (decimal)((double)nUDJoyXYStep4.Value / time * 60 * correct);
            nUDJoyXYSpeed5.Value = (decimal)((double)nUDJoyXYStep5.Value / time * 60 * correct);
        }

        private void btnJoyZCalc_Click(object sender, EventArgs e)
        {
            double time = 0.5;
            double correct = 1;
            nUDJoyZSpeed1.Value = (decimal)((double)nUDJoyZStep1.Value / time * 60 * correct);
            nUDJoyZSpeed2.Value = (decimal)((double)nUDJoyZStep2.Value / time * 60 * correct);
            nUDJoyZSpeed3.Value = (decimal)((double)nUDJoyZStep3.Value / time * 60 * correct);
            nUDJoyZSpeed4.Value = (decimal)((double)nUDJoyZStep4.Value / time * 60 * correct);
            nUDJoyZSpeed5.Value = (decimal)((double)nUDJoyZStep5.Value / time * 60 * correct);
        }


        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   LinkLabel clickedLink = sender as LinkLabel;
            Process.Start(clickedLink.Tag.ToString()); }

        private void tabPage6_Enter(object sender, EventArgs e)
        { timer1.Enabled = true; }

        private void tabPage6_Leave(object sender, EventArgs e)
        { timer1.Enabled = false; }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {   if (ControlGamePad.gamePad != null)
                {   ControlGamePad.gamePad.Poll();
                    var datas = ControlGamePad.gamePad.GetBufferedData();
                    foreach (var state in datas)
                    {
                        lblgp.Text = state.Offset + " Value: " + state.Value.ToString();
                        processGamepad(state);
                    }
                }
            }
            catch
            {
                try { ControlGamePad.Initialize(); timer1.Interval = 200; }
                catch { timer1.Interval = 5000; }
            }
        }
        private void processGamepad(SharpDX.DirectInput.JoystickUpdate state)
        {
            string offset = state.Offset.ToString();
            int value = state.Value;
            if (offset.IndexOf("Buttons") >= 0)
            {
                foreach (Control c in this.gBGP.Controls)
                {
                    if (c.Name == ("lbl" + offset)) if (c != null)
                        { c.BackColor = (value > 0) ? Color.Lime : Color.LightGray; break; }
                }
            }
            if (offset == "X")
            { trackBarX.Value = value; }
            if (offset == "Y")
            { trackBarY.Value = value; }
            if (offset == "Z")
            { trackBarZ.Value = value; }
            if (offset == "RotationZ")
            { trackBarR.Value = value; }
        }


        private void hsFilterScroll(object sender, ScrollEventArgs e)
        { hsFilterScrollSetLabels(); }
        private void hsFilterScrollSetLabels()
        {
            lblFilterRed1.Text = hSFilterRed1.Value.ToString();
            lblFilterRed2.Text = hSFilterRed2.Value.ToString();
            lblFilterGreen1.Text = hSFilterGreen1.Value.ToString();
            lblFilterGreen2.Text = hSFilterGreen2.Value.ToString();
            lblFilterBlue1.Text = hSFilterBlue1.Value.ToString();
            lblFilterBlue2.Text = hSFilterBlue2.Value.ToString();
        }

        private void btnShapeSetSave_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int btnSetIndex = Convert.ToUInt16(clickedButton.Name.Substring("btnShapeSetSave".Length));
            //          MessageBox.Show(index.ToString());
            if (btnSetIndex == 1) { Properties.Settings.Default.camShapeSet1 = shapeSetSave(tBShapeSet1.Text); }
            if (btnSetIndex == 2) { Properties.Settings.Default.camShapeSet2 = shapeSetSave(tBShapeSet2.Text); }
            if (btnSetIndex == 3) { Properties.Settings.Default.camShapeSet3 = shapeSetSave(tBShapeSet3.Text); }
            if (btnSetIndex == 4) { Properties.Settings.Default.camShapeSet4 = shapeSetSave(tBShapeSet4.Text); }
            Properties.Settings.Default.Save();
        }

        private void btnShapeSetLoad_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int btnLoadIndex = Convert.ToUInt16(clickedButton.Name.Substring("btnShapeSetLoad".Length));
            //           MessageBox.Show(index.ToString());
            if (btnLoadIndex == 1) { tBShapeSet1.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet1); }
            if (btnLoadIndex == 2) { tBShapeSet2.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet2); }
            if (btnLoadIndex == 3) { tBShapeSet3.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet3); }
            if (btnLoadIndex == 4) { tBShapeSet4.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet4); }
        }

        private String shapeSetSave(string head)
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

        private string shapeSetLoad(string txt)
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
                cBFilterOuside.Checked = (value[i++] == "True") ? true : false;
                cBShapeCircle.Checked = (value[i++] == "True") ? true : false;
                cBShapeRect.Checked = (value[i++] == "True") ? true : false;
                nUDShapeSizeMin.Value = Convert.ToDecimal(value[i++]);
                nUDShapeSizeMax.Value = Convert.ToDecimal(value[i++]);
                nUDShapeDistMin.Value = Convert.ToDecimal(value[i++]);
                nUDShapeDistMax.Value = Convert.ToDecimal(value[i++]);
                hsFilterScrollSetLabels();
                return value[0];
            }
            catch { }
            return "not set";
        }

        private void setLabelParameterSet(int lblSetIndex, string txt)
        {
            if (lblSetIndex == 1) { tBShapeSet1.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (lblSetIndex == 2) { tBShapeSet2.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (lblSetIndex == 3) { tBShapeSet3.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (lblSetIndex == 4) { tBShapeSet4.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
        }

        private void ExportDgvToCSV(string file)
        {
// fields: ToolNr,color,name,X,Y,Z,diameter,XYspeed,Z - speed,Z - save,Z - depth,Z - step, spindleSpeed, overlap, gcode
            int[] cellWidth  = {3,7,-20,5,5,5,4,4,5,4,4,5,6,4,1,1,1 };
            string format = "{0,2}";
            var csv = new StringBuilder();
            csv.AppendLine("# T-Nr; Color; T-Name; ExPosX; ExPosY; ExPosZ; T-Diameter; XY-Feed; Z-Feed; Z-Save; Z-Deepth; Z-Step; Spindle-Spd; Step-over %, GCode");
            foreach (DataGridViewRow dgvR in dGVToolList.Rows)
            {
                bool firstColumn = true;
                if (dgvR.Cells[0].Value != null)
                {   for (int j = 0; j < dGVToolList.Columns.Count; ++j)
                    {   object val = dgvR.Cells[j].Value;
                        if (!firstColumn)
                            csv.Append(',');                            // csv delimiter
                        if (val == null)
                            csv.Append(toolTable.defaultTool[j]);       // fill with default value
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
            File.WriteAllText(file, csv.ToString());
            dGVToolList.DefaultCellStyle.NullValue = dGVToolList.Columns.Count;
            fillToolTableFileList(Application.StartupPath + datapath.tools);
        }

        private bool ImportCSVToDgv(string file)
        {
            if (File.Exists(file))
            {
                Logger.Trace("Load Tool Table {0}", file);
                dGVToolList.Rows.Clear();
                string[] readText = File.ReadAllLines(file);
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
                        for (int j = 0; j < toolTable.defaultTool.Length; ++j)
                        {   if (j < col.Length)
                            {
                                tmp = col[j].Trim();
                                dGVToolList.Rows[row].Cells[j].Value = tmp;  // fill up empty cells
                            }
                            else
                                dGVToolList.Rows[row].Cells[j].Value = toolTable.defaultTool[j];
                        }
                        try
                        {   long clr = Convert.ToInt32(col[1].Trim().Substring(0,6), 16) | 0xff000000;
                            dGVToolList.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb((int)clr);
                            dGVToolList.Rows[row].DefaultCellStyle.ForeColor = ContrastColor(Color.FromArgb((int)clr));
                        }
                        catch
                        {   dGVToolList.Rows[row].DefaultCellStyle.BackColor = Color.White;
                            dGVToolList.Rows[row].DefaultCellStyle.ForeColor = Color.Black;
                        }
                    }
                    row++;
                }
                return true;
            }
            return false;
        }

        private void dGVToolList_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            // change color if changed
            object val = dGVToolList.Rows[e.RowIndex].Cells[1].Value;
            if ((val != null) && (e.ColumnIndex == 1))
            {   string color = val.ToString();
                if (color.Length > 6)
                {   dGVToolList.Rows[e.RowIndex].Cells[1].Value = color.Substring(0, 6); }
                try
                {   long clr = Convert.ToInt32(color.Substring(0, 6), 16) | 0xff000000;
                    dGVToolList.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb((int)clr);
                    dGVToolList.Rows[e.RowIndex].DefaultCellStyle.ForeColor = ContrastColor(Color.FromArgb((int)clr));
                }
                catch
                {   dGVToolList.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    dGVToolList.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
    //        if (dGVToolList.Rows.Count > 1)
   //             ExportDgvToCSV(defaultToolList);

            Properties.Settings.Default.toolTableOriginal = false;
            lblToolListChanged.Text = "Changed List should be saved \r\nvia 'Save tool table'";
            lblToolListChanged.BackColor = Color.Yellow;
        }
        private void btnReNumberTools_Click(object sender, EventArgs e)
        {   int number = 1;
            foreach (DataGridViewRow dgvR in dGVToolList.Rows)
            {   dgvR.Cells[0].Value = number++;
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dGVToolList;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == 0)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex - 1, selectedRow);
                dgv.ClearSelection();
//                dgv.Rows[rowIndex - 1].Cells[colIndex].Selected = true;
                dgv.Rows[rowIndex - 1].Selected = true;
            }
            catch { }
        }

        private void btnDown_Click(object sender, EventArgs e)
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
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex + 1, selectedRow);
                dgv.ClearSelection();
  //              dgv.Rows[rowIndex + 1].Cells[colIndex].Selected = true;
                dgv.Rows[rowIndex + 1].Selected = true;
            }
            catch { }
        }

        private void dGV_SortColor(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index != 1) return;
            long lcolor1 = Convert.ToInt32(e.CellValue1.ToString().Substring(0, 6), 16) | 0xff000000;
            long lcolor2 = Convert.ToInt32(e.CellValue2.ToString().Substring(0, 6), 16) | 0xff000000;
            Color ccolor1 = Color.FromArgb((int)lcolor1);
            Color ccolor2 = Color.FromArgb((int)lcolor2);
            double brighness1 = (0.299 * ccolor1.R * ccolor1.R + 0.587 * ccolor1.G * ccolor1.G + 0.114 * ccolor1.B * ccolor1.B);
            double brighness2 = (0.299 * ccolor2.R * ccolor2.R + 0.587 * ccolor2.G * ccolor2.G + 0.114 * ccolor2.B * ccolor2.B);
            e.SortResult = brighness1.CompareTo(brighness2);
            e.Handled = true;
        }
        private void btnToolExport_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the List
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = importPath;
            saveFileDialog1.Filter = "CSV File|*.csv";
            saveFileDialog1.Title = "Save Tool List as CSV";
            saveFileDialog1.ShowDialog();
            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                ExportDgvToCSV(saveFileDialog1.FileName);
            }
            fillToolTableFileList(Application.StartupPath + datapath.tools);
        }

        private static string importPath = Application.StartupPath + datapath.tools;
        private void btnToolImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = importPath;
            openFileDialog1.Filter = "CSV File|*.csv";
            openFileDialog1.Title = "Load Tool List";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadToolList(openFileDialog1.FileName);
     //           ImportCSVToDgv(openFileDialog1.FileName);
      //          ExportDgvToCSV(defaultToolList);
         //       importPath = openFileDialog1.FileName;
            }
        }
        private void loadToolList(string filename)
        {   
            ImportCSVToDgv(filename);
            Properties.Settings.Default.toolTableOriginal = true;
            Properties.Settings.Default.toolTableLastLoaded = Path.GetFileName(filename);
            lblToolListLoaded.Text = Path.GetFileName(filename);
            lblToolListChanged.Text = "orginal";
            lblToolListChanged.BackColor = Color.Transparent;
            ExportDgvToCSV(defaultToolList);
        }
        private void btnLoadToolTable_Click(object sender, EventArgs e)
        {
            if (lbFiles.Text == "")
            {
                MessageBox.Show("No file selected...", "Attention");
                return;
            }
            string path = Application.StartupPath + datapath.tools + "\\" + lbFiles.Text;
            if (path.StartsWith("laser"))
                cBToolLaser.Checked = true;
            //           MessageBox.Show(path);
            loadToolList(path);
        }
        private void btnDeleteToolTable_Click(object sender, EventArgs e)
        {
            File.Delete(Application.StartupPath + datapath.tools + "\\" + lbFiles.Text);
            fillToolTableFileList(Application.StartupPath + datapath.tools);
        }

        public string commandToSend = "";
        private void btnMoveToolXY_Click(object sender, EventArgs e)
        {
            float xoff = (float)nUDToolOffsetX.Value;
            float yoff = (float)nUDToolOffsetY.Value;
            if (dGVToolList.CurrentRow.Cells[3].Value == null)
            {
                MessageBox.Show("No valid position");
                return;
            }
            string sx = dGVToolList.CurrentRow.Cells[3].Value.ToString();
            string sy = dGVToolList.CurrentRow.Cells[4].Value.ToString();
            string sz = dGVToolList.CurrentRow.Cells[5].Value.ToString();
            float x=0, y=0, z=0;
            if ((sx.Length < 1) || (sy.Length < 1) || (sz.Length < 1))
            {
                MessageBox.Show("No valid position");
                return;
            }
            try
            {
                x = float.Parse(sx, System.Globalization.NumberFormatInfo.InvariantInfo);
                y = float.Parse(sy, System.Globalization.NumberFormatInfo.InvariantInfo);
                z = float.Parse(sz, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            catch { MessageBox.Show("No valid position"); return; }
            x = x + xoff; y = y + yoff;
            DialogResult result = MessageBox.Show("Are you sure to move to machine XY-position "+x.ToString()+":"+y.ToString()+" ?\r\n", "Attention", MessageBoxButtons.YesNo);
            commandToSend = "";
            if (result == DialogResult.Yes)
            {   commandToSend = String.Format("G53 G91 G0 X{0} Y{1}",x,y).Replace(',', '.');
            }
        }

        private void lbl_1_Click(object sender, EventArgs e)
        {  showFileContent(tBToolChangeScriptPut);        }
        private void lbl_2_Click(object sender, EventArgs e)
        { showFileContent(tBToolChangeScriptSelect); }
        private void lbl_3_Click(object sender, EventArgs e)
        { showFileContent(tBToolChangeScriptGet); }
        private void lbl_4_Click(object sender, EventArgs e)
        { showFileContent(tBToolChangeScriptProbe); }

        private void showFileContent(TextBox tmp)
        {
            if (File.Exists(tmp.Text))
            {   MessageBox.Show("Current directory: " + Directory.GetCurrentDirectory() + "\r\nFile: '" + tmp.Text+ "'\r\n\r\n" + File.ReadAllText(tmp.Text).Replace('\t',' '), "File content of " + tmp.Text);
            }
            else
                MessageBox.Show("Current directory: " + Directory.GetCurrentDirectory() + "\r\nFile: '" + tmp.Text + "' not found", "File content");
        }

        private void btnShowScriptSub_Click(object sender, EventArgs e)
        {  showFileContent(tBImportGCSubroutine);   }

        private void cBImportGCTool_CheckedChanged(object sender, EventArgs e)
        {
            /*         nUDImportGCSSpeed.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTSSpeed.Checked);
                     nUDImportGCFeedXY.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTXYFeed.Checked);
                     nUDImportGCFeedZ.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTZFeed.Checked);
                     nUDImportGCZDown.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTZDeepth.Checked);
                     nUDImportGCZIncrement.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTZIncrement.Checked);*/
            checkVisibility();
        }

        private void btnFileDialogTT1_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
//            MessageBox.Show(clickedButton.Name);
            if (clickedButton.Name.IndexOf("TT1") > 0)
                setFilePath(tBToolChangeScriptPut);
            else if (clickedButton.Name.IndexOf("TT2") > 0)
                setFilePath(tBToolChangeScriptSelect);
            else if (clickedButton.Name.IndexOf("TT3") > 0)
                setFilePath(tBToolChangeScriptGet);
            else if (clickedButton.Name.IndexOf("TT4") > 0)
                setFilePath(tBToolChangeScriptProbe);
            else if (clickedButton.Name.IndexOf("SubR") > 0)
                setFilePath(tBImportGCSubroutine);
        }
        private void setFilePath(TextBox tmp)
        { 
            OpenFileDialog opnDlg = new OpenFileDialog();
            string ipath = makeAbsolutePath(tmp.Text);
            opnDlg.InitialDirectory = ipath.Substring(0,ipath.LastIndexOf("\\"));
            opnDlg.Filter = "GCode (*.nc)|*.nc|All Files (*.*)|*.*";
//            MessageBox.Show(opnDlg.InitialDirectory+"\r\n"+ Application.StartupPath);
            if (opnDlg.ShowDialog(this) == DialogResult.OK)
            {
                FileInfo f = new FileInfo(opnDlg.FileName);
                string path = "";
                if (f.DirectoryName == Application.StartupPath)
                    path = f.Name;  // only file name
                else if (f.DirectoryName.StartsWith(Application.StartupPath))
                    path = f.FullName.Replace(Application.StartupPath, ".");
                else
                    path = f.FullName;  // Full path
                if (path.StartsWith(@".\"))
                    path = path.Substring(2);
                tmp.Text = path;
            }
        }
        private static string makeAbsolutePath(string iFilename)
        {
            string iNewFilename = "";
            if ((iFilename == "") || (!File.Exists(iFilename)))
                iFilename = Application.StartupPath;

            // Get full name considering relative path
            FileInfo f = new FileInfo(iFilename);

            if (iFilename == Application.StartupPath)
                iNewFilename = Application.StartupPath;
            else if (!(iFilename.StartsWith(".") || iFilename.StartsWith("\\")) && !f.DirectoryName.StartsWith(Application.StartupPath))  // File in child folder
                iNewFilename = Application.StartupPath + "\\"
                             + iFilename.Substring(1);  // leave period out of string
            else if (iFilename.StartsWith(".\\"))       // File in child folder
                iNewFilename = Application.StartupPath
                             + iFilename.Substring(1) ; // leave period out of string
            else if (!iFilename.Contains("\\"))         // Consider file in StartupPath
                iNewFilename = Application.StartupPath
                             + "\\" + iFilename ;
            else
                iNewFilename = f.FullName ; // keep full path

            return iNewFilename;
        }

        private void ControlSetupForm_SizeChanged(object sender, EventArgs e)
        {
            int y = Height;
            tabControl1.Height = y - 64;
            btnApplyChangings.Top = y - 64;
            gBToolChange.Height = y - 98;
            gBToolTable.Height = y - 192;
            dGVToolList.Height = y - 64;
        }

        private void tB_KeyPad_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox clickedTB = sender as TextBox;
            clickedTB.Text = e.KeyData.ToString();
            Clipboard.SetText(e.KeyData.ToString());
        }

        private void listHotkeys()
        {   tBHotkeyList.Clear();
            string fileName = Application.StartupPath + datapath.hotkeys;
            if (!File.Exists(fileName))
            {   tBHotkeyList.Text = "File 'hotkeys.xml' not found, no hotkeys set!";
                Logger.Error("File 'hotkeys.xml' not found in ", fileName);
                return;
            }
            string tmp = File.ReadAllText(fileName); ;
            tBHotkeyList.Text = tmp;
            lblPathHotkeys.Text = fileName;
        }
        private void btnOpenHotkeys_Click(object sender, EventArgs e)
        {   Process.Start("notepad.exe", lblPathHotkeys.Text);  }

        private void btnHotkeyRefresh_Click(object sender, EventArgs e)
        {   listHotkeys(); }

        private void btnMachineRangeGet_Click(object sender, EventArgs e)
        {   if (grbl.getSetting(130) < 0)
                MessageBox.Show("No information available - please connect grbl-controller","Attention!");
            else
            {   nUDMachineRangeX.Value = (decimal)grbl.getSetting(130);
                nUDMachineRangeY.Value = (decimal)grbl.getSetting(131);
                nUDMachineRangeZ.Value = (decimal)grbl.getSetting(132);
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {   lblJoystickSize.Text = hScrollBar1.Value.ToString();
        }

        private void cBGPEnable_CheckedChanged(object sender, EventArgs e)
        {   if (cBGPEnable.Checked)
            {   try { ControlGamePad.Initialize(); }
                catch { }
            }
        }

        private void cBsimulation_CheckedChanged(object sender, EventArgs e)
        {   grbl.grblSimulate = cBsimulation.Checked;
            grbl.axisA = true;
            grbl.axisB = true;
            grbl.axisC = true;
        }

        private void cBoxPollInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.grblPollIntervalIndex = cBoxPollInterval.SelectedIndex;
        }

        private void btnCheckSpindle_Click(object sender, EventArgs e)
        {
            if (grbl.getSetting(30) < 0)
                MessageBox.Show("No information available - please connect grbl-controller", "Attention!");
            else
            {
                string tmp = "";
                tmp += string.Format("Current grbl Settings:\rMax spindle speed:\t$30={0}\rMin spindle speed:\t$31={1}\rLaser Mode:\t\t$32={2}", grbl.getSetting(30), grbl.getSetting(31), grbl.getSetting(32));
                MessageBox.Show(tmp,"Information");
            }
        }

        private void rBImportSVGSort0_CheckedChanged(object sender, EventArgs e)
        {
            if (rBImportSVGSort0.Checked)
                Properties.Settings.Default.importGroupSort = 0;
            if (rBImportSVGSort1.Checked)
                Properties.Settings.Default.importGroupSort = 1;
            if (rBImportSVGSort2.Checked)
                Properties.Settings.Default.importGroupSort = 2;
        }

        private void dGVToolList_SelectionChanged(object sender, EventArgs e)
        {
            bool enabled = (dGVToolList.SelectedRows.Count > 0);
            btnUp.Visible = enabled;
            btnDown.Visible = enabled;
        }

        private void fillToolTableFileList(string Root)
        {
            List<string> FileArray = new List<string>();
            try
            {
                string[] Files = System.IO.Directory.GetFiles(Root);
                string[] Folders = System.IO.Directory.GetDirectories(Root);

                lbFiles.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("csv"))
                    {   string name = Path.GetFileName(Files[i]);
                        if (name != toolTable.defaultFileName)
                            lbFiles.Items.Add(name);
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }



        private void cBToolLaser_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = !cBToolLaser.Checked;
            lblToolOffset.Visible = enabled;
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

        private void cBImportSVGGroup_CheckedChanged(object sender, EventArgs e)
        {   bool enable = cBImportSVGGroup.Checked;
            rBImportSVGSort0.Enabled = enable;
            rBImportSVGSort1.Enabled = enable;
            rBImportSVGSort2.Enabled = enable;
            cBImportSVGSortInvert.Enabled = enable;
        }

        private void cBToolTableUse_CheckedChanged(object sender, EventArgs e)
        {   bool enable = cBToolTableUse.Checked;
            cBImportGCTTSSpeed.Enabled = enable;
            cBImportGCTTXYFeed.Enabled = enable;
     //       cBImportGCTTZDeepth.Enabled = (enable && cBImportGCUseZ.Checked);
            cBImportGCTTZAxis.Enabled = (enable && cBImportGCUseZ.Checked);
      //      cBImportGCTTZIncrement.Enabled = (enable && cBImportGCUseZ.Checked && cBImportGCZIncEnable.Checked);
            checkVisibility();
        }

        private void cBImportSVGResize_CheckedChanged(object sender, EventArgs e)
        {   bool enable = cBImportSVGResize.Checked;
            nUDSVGScale.Enabled = enable;
            lblSVGScale.Enabled = enable;
        }

        private void checkVisibility()
        {   bool optionUseZ = cBImportGCUseZ.Checked;
            bool optionUseTTVal = cBToolTableUse.Checked;

            bool enable = cBToolTableUse.Checked;
            cBImportGCTTSSpeed.Enabled = enable;
            cBImportGCTTXYFeed.Enabled = enable;
     //       cBImportGCTTZDeepth.Enabled = (enable && cBImportGCUseZ.Checked);
            cBImportGCTTZAxis.Enabled = (enable && cBImportGCUseZ.Checked);
     //       cBImportGCTTZIncrement.Enabled = (enable && cBImportGCUseZ.Checked && cBImportGCZIncEnable.Checked);
            cBToolTableDefault.Enabled = enable;
            numericUpDown2.Enabled = cBToolTableDefault.Checked && enable;

            // Use Z Group
            lblZUse1.Enabled = optionUseZ;
            lblZUse2.Enabled = optionUseZ;
            lblZUse3.Enabled = optionUseZ;
            nUDImportGCFeedZ.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled));
            nUDImportGCZUp.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled));
            nUDImportGCZDown.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled));
            cBImportGCZIncEnable.Enabled = optionUseZ;
            lblZUse4.Enabled = (optionUseZ && cBImportGCZIncEnable.Checked );
            cBImportGCZIncStartZero.Enabled = (optionUseZ && cBImportGCZIncEnable.Checked);
            nUDImportGCZIncrement.Enabled = (optionUseZ && !(cBImportGCTTZAxis.Checked && cBImportGCTTZAxis.Enabled) && cBImportGCZIncEnable.Checked);

            nUDImportGCFeedXY.Enabled = !(cBImportGCTTXYFeed.Checked && cBImportGCTTXYFeed.Enabled);
            nUDImportGCSSpeed.Enabled = !(cBImportGCTTSSpeed.Checked && cBImportGCTTSSpeed.Enabled);
        }

        private void cBImportGCUsePWM_CheckedChanged(object sender, EventArgs e)
        {   bool enable = cBImportGCUsePWM.Checked;
            lblPWM1.Enabled = enable;
            lblPWM2.Enabled = enable;
            lblPWM3.Enabled = enable;
            lblPWM4.Enabled = enable;
            nUDImportGCPWMUp.Enabled = enable;
            nUDImportGCDlyUp.Enabled = enable;
            nUDImportGCPWMDown.Enabled = enable;
            nUDImportGCDlyDown.Enabled = enable;
        }

        private void cBImportGCUseIndividual_CheckedChanged(object sender, EventArgs e)
        {   bool enable = cBImportGCUseIndividual.Checked;
            lblUseInd1.Enabled = enable;
            lblUseInd2.Enabled = enable;
            tBImportGCIPU.Enabled = enable;
            tBImportGCIPD.Enabled = enable;
        }

        private void cBImportGCDragKnife_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = cBImportGCDragKnife.Checked;
            nUDImportGCDragKnifeLength.Enabled = enable;
            nUDImportGCDragKnifePercent.Enabled = enable;
            nUDImportGCDragKnifeAngle.Enabled = enable;
            cBImportGCDragKnifePercent.Enabled = enable;
            lblDrag1.Enabled = enable;
            lblDrag2.Enabled = enable;
        }

        private void cBImportGCLineSegments_CheckedChanged(object sender, EventArgs e)
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

        private void cBImportGCNoArcs_CheckedChanged(object sender, EventArgs e)
        {
            nUDImportGCSegment.Enabled = cBImportGCNoArcs.Checked;
        }

        private void fillUseCaseFileList(string Root)
        {
            List<string> FileArray = new List<string>();
            try
            {
                string[] Files = System.IO.Directory.GetFiles(Root);
                string[] Folders = System.IO.Directory.GetDirectories(Root);

                lBUseCase.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("ini"))
                        lBUseCase.Items.Add(Path.GetFileName(Files[i]));
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }

        
        private void btnLoadUseCase_Click(object sender, EventArgs e)
        {
            if (lBUseCase.Text != "")
            {   string path = Application.StartupPath + datapath.usecases + "\\" + lBUseCase.Text;
                var MyIni = new IniFile(path);
                Logger.Trace("Load Use Case   {0}", path);

                MyIni.ReadAll();   // ReadImport();
                fillUseCaseFileList(Application.StartupPath + datapath.usecases );
                Properties.Settings.Default.useCaseLastLoaded = lBUseCase.Text;
                lblLastUseCase.Text = lBUseCase.Text;

                dGVToolList.CellEndEdit -= new DataGridViewCellEventHandler(dGVToolList_CellLeave);
                dGVToolList.CellLeave   -= new DataGridViewCellEventHandler(dGVToolList_CellLeave);
                ImportCSVToDgv(defaultToolList);
                dGVToolList.CellEndEdit += new DataGridViewCellEventHandler(dGVToolList_CellLeave);
                dGVToolList.CellLeave   += new DataGridViewCellEventHandler(dGVToolList_CellLeave);

                lblToolListLoaded.Text = Properties.Settings.Default.toolTableLastLoaded;
                if (Properties.Settings.Default.toolTableOriginal)
                {   lblToolListChanged.Text = "orginal";
                    lblToolListChanged.BackColor = Color.Transparent;
                }
            }
        }

        private void btnUseCaseSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Application.StartupPath + datapath.usecases;
            sfd.Filter = "Use cases (*.ini)|*.ini";
            sfd.FileName = "new_use_case.ini";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var MyIni = new IniFile(sfd.FileName);
                MyIni.WriteImport();
            }
            fillUseCaseFileList(Application.StartupPath + datapath.usecases);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {   File.Delete(Application.StartupPath + datapath.usecases + "\\" + lBUseCase.Text);
            fillUseCaseFileList(Application.StartupPath + datapath.usecases);
        }

        private void lBUseCase_SelectedIndexChanged(object sender, EventArgs e)
        {   string path = Application.StartupPath + datapath.usecases + "\\" + lBUseCase.Text;
            var MyIni = new IniFile(path);
            tBUseCaseSetting2.Text = MyIni.ReadUseCaseInfo();
            tBUseCaseSetting1.Text = MyIni.showIniSettings(); ;
        }
    }
}
