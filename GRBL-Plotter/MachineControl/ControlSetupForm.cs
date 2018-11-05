/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2018 Sven Hasemann contact: svenhb@web.de

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
 *  *  2017-01-01  check form-location and fix strange location
*/
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Xml;
using System.IO;
using System.Text;

namespace GRBL_Plotter
{
    public partial class ControlSetupForm : Form
    {
        public ControlSetupForm()
        {
            CultureInfo ci = new CultureInfo(Properties.Settings.Default.language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            InitializeComponent();
        }

        public void setLastLoadedFile(string text)
        { toolTip1.SetToolTip(btnReloadFile, text); }

        private string defaultToolList = "tools.csv";
        private void SetupForm_Load(object sender, EventArgs e)
        {
            defaultToolList = System.Windows.Forms.Application.StartupPath + "\\tools.csv";
            if ((!ImportCSVToDgv(defaultToolList)) || (dGVToolList.Rows.Count == 1))
            {
                string[] tmp = toolTable.defaultTool;  // { "1", "000000", "Black", "0.0", "0.0", "0.0", "3.0", "500" };
                dGVToolList.Rows.Add(tmp);
            }

            string text;
            string[] parts;// = new string[] { "-", "(-)" };
            dGVCustomBtn.Rows.Clear();
            int row = 0;
            for (int i = 1; i <= 8; i++)
            {
                parts = new string[2];
                text = Properties.Settings.Default["custom" + i.ToString()].ToString();
                if (text.IndexOf('|') > 0)
                { parts = text.Split('|'); }
                else
                { parts[0] = "-";parts[1] = "(-)"; }
                dGVCustomBtn.Rows.Add();
                dGVCustomBtn.Rows[row].Cells[0].Value = i.ToString();
                dGVCustomBtn.Rows[row].Cells[1].Value = parts[0];
                dGVCustomBtn.Rows[row].Cells[2].Value = parts[1];
                row++;
             }

         //   lvCustomButtons.Items[0].Selected = true;
            setButtonColors(btnColorBackground, Properties.Settings.Default.colorBackground);
            setButtonColors(btnColorRuler, Properties.Settings.Default.colorRuler);
            setButtonColors(btnColorPenUp, Properties.Settings.Default.colorPenUp);
            setButtonColors(btnColorPenDown, Properties.Settings.Default.colorPenDown);
            setButtonColors(btnColorTool, Properties.Settings.Default.colorTool);
            setButtonColors(btnColorMarker, Properties.Settings.Default.colorMarker);
            setButtonColors(btnColorHeightMap, Properties.Settings.Default.colorHeightMap);
            setButtonColors(btnColorMachineLimit, Properties.Settings.Default.colorMachineLimit);
            nUDImportDecPlaces.Value = Properties.Settings.Default.importGCDecPlaces;

            Location = Properties.Settings.Default.locationSetForm;
            Size desktopSize = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            if ((Location.X < -20) || (Location.X > (desktopSize.Width - 100)) || (Location.Y < -20) || (Location.Y > (desktopSize.Height - 100))) { Location = new Point(0, 0); }

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

            lblFilePath.Text = System.Windows.Forms.Application.StartupPath;

            hsFilterScrollSetLabels();

            setLabelParameterSet(1, Properties.Settings.Default.camShapeSet1);
            setLabelParameterSet(2, Properties.Settings.Default.camShapeSet2);
            setLabelParameterSet(3, Properties.Settings.Default.camShapeSet3);
            setLabelParameterSet(4, Properties.Settings.Default.camShapeSet4);

            cBImportGCTool_CheckedChanged(sender, e);

            dGVToolList.SortCompare += new DataGridViewSortCompareEventHandler(this.dGV_SortColor);
        }

        private void saveSettings()
        {
            for (int i = 1; i <= 8; i++)
            {
                try { Properties.Settings.Default["custom" + i.ToString()] = dGVCustomBtn.Rows[i - 1].Cells[1].Value + "|" + dGVCustomBtn.Rows[i - 1].Cells[2].Value; }
                catch { Properties.Settings.Default["custom" + i.ToString()] = " | "; }
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"https://openclipart.org/tags/svg"); }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"https://publicdomainvectors.org/"); }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"https://simplemaps.com/"); }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"http://www.cliparts101.com/"); }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"http://www.clker.com/"); }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"https://free.clipartof.com/"); }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"https://github.com/gnea/grbl/wiki"); }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { Process.Start(@"http://linuxcnc.org/docs/html/gcode.html"); }

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
        {   //ToolNr,color,name,X,Y,Z,diameter,XYspeed,Z-step, Zspeed, spindleSpeed, overlap
            string[] deflt = { "1", "000000", "not set", "0", "0", "0", "1", "100", "-1", "100", "1000", "100" };
            var csv = new StringBuilder();
            foreach (DataGridViewRow dgvR in dGVToolList.Rows)
            {
                bool firstColumn = true;
                if (dgvR.Cells[0].Value != null)
                {   for (int j = 0; j < dGVToolList.Columns.Count; ++j)
                    {   object val = dgvR.Cells[j].Value;
                        if (!firstColumn)
                            csv.Append(';');
                        if (val == null)
                            csv.Append(deflt[j]);   // fill with default value
                        else
                            csv.AppendFormat("{0}",val);
                        firstColumn = false;
                    }
                    csv.Append("\r\n");
                }
            }
            File.WriteAllText(file, csv.ToString());
            dGVToolList.DefaultCellStyle.NullValue = dGVToolList.Columns.Count;

        }

        private bool ImportCSVToDgv(string file)
        {
            if (File.Exists(file))
            {
                dGVToolList.Rows.Clear();
                string[] readText = File.ReadAllLines(file);
                string[] col;
                string tmp;
                int row = 0;
                foreach (string s in readText)
                {
                    if (s.Length > 10)
                    {
                        col = s.Split(';');
                        dGVToolList.Rows.Add();
                        for (int j = 0; j < col.Length; ++j)
                        {   tmp = col[j].Trim();
                            dGVToolList.Rows[row].Cells[j].Value = tmp.Length == 0? "0":tmp;  // fill up empty cells
                        }
                        try
                        {   long clr = Convert.ToInt32(col[1].Substring(0,6), 16) | 0xff000000;
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
            if (dGVToolList.Rows.Count > 1)
                ExportDgvToCSV(defaultToolList);

        }
        private void btnReNumberTools_Click(object sender, EventArgs e)
        {   int number = 1;
            foreach (DataGridViewRow dgvR in dGVToolList.Rows)
            {   dgvR.Cells[0].Value = number++;
            }
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
            saveFileDialog1.Filter = "CSV File|*.csv";
            saveFileDialog1.Title = "Save Tool List as CSV";
            saveFileDialog1.ShowDialog();
            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                ExportDgvToCSV(saveFileDialog1.FileName);
            }
        }

        private static string importPath = Application.StartupPath+"\\_misc";
        private void btnToolImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = importPath;
            openFileDialog1.Filter = "CSV File|*.csv";
            openFileDialog1.Title = "Load Tool List";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportCSVToDgv(openFileDialog1.FileName);
                ExportDgvToCSV(defaultToolList);
                importPath = openFileDialog1.FileName;
            }
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
            nUDImportGCSSpeed.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTSSpeed.Checked);
            nUDImportGCFeedXY.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTXYFeed.Checked);
            nUDImportGCFeedZ.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTZFeed.Checked);
            nUDImportGCZDown.Enabled = !(cBImportGCTool.Checked && cBToolChange.Checked && cBImportGCTTZDeepth.Checked);
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
    }
}
