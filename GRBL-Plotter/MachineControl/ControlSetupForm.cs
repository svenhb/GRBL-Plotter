/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2017 Sven Hasemann contact: svenhb@web.de

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
        { toolTip1.SetToolTip(btnReloadFile,text); }

        private void SetupForm_Load(object sender, EventArgs e)
        {
            string[] parts;
            for (int i = 1; i <= 8; i++)
            {
                parts = Properties.Settings.Default["custom" + i.ToString()].ToString().Split('|');
                ListViewItem item = new ListViewItem((i-1).ToString());
                item.SubItems.AddRange(parts);
                lvCustomButtons.Items.Add(item);
            }
            lvCustomButtons.Items[0].Selected = true;
            setButtonColors(btnColorBackground,Properties.Settings.Default.colorBackground);
            setButtonColors(btnColorRuler,Properties.Settings.Default.colorRuler);
            setButtonColors(btnColorPenUp,Properties.Settings.Default.colorPenUp);
            setButtonColors(btnColorPenDown,Properties.Settings.Default.colorPenDown);
            setButtonColors(btnColorTool,Properties.Settings.Default.colorTool);
            setButtonColors(btnColorMarker, Properties.Settings.Default.colorMarker);
            setButtonColors(btnColorHeightMap, Properties.Settings.Default.colorHeightMap);
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
        }

        private void saveSettings()
        {  
            for (int i = 1; i <= 8; i++)
            {
                ListViewItem item = lvCustomButtons.Items[i - 1];
                Properties.Settings.Default["custom" + i.ToString()] = item.SubItems[1].Text + "|" + item.SubItems[2].Text;// + "|" + item.SubItems[3].Text;
            }
            Properties.Settings.Default.importGCDecPlaces = nUDImportDecPlaces.Value;
            Properties.Settings.Default.importGCSpindleCmd = rBImportGCSpindleCmd1.Checked;
            Properties.Settings.Default.ctrlReplaceM3 = rBCtrlReplaceM3.Checked;
            Properties.Settings.Default.rotarySubstitutionX = rBRotaryX.Checked;

            Properties.Settings.Default.Save();
         }

        int lastIndex = 0;
        private void lvCustomButtons_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (lvCustomButtons != null)
            { //MessageBox.Show(e.ItemIndex.ToString());
                lastIndex = e.ItemIndex;
                ListViewItem item = lvCustomButtons.Items[lastIndex];
                textBox1.Text = item.SubItems[1].Text;
                textBox2.Text = item.SubItems[2].Text;
//                textBox3.Text = item.SubItems[3].Text;
                lblcbnr.Text = lastIndex.ToString();
            }
        }

        private void btnChangeDefinition_Click(object sender, EventArgs e)
        {
            if (lvCustomButtons != null)
            {
                ListViewItem item = lvCustomButtons.Items[lastIndex]; // SelectedItems[0];
                item.SubItems[1].Text = textBox1.Text;
                item.SubItems[2].Text = textBox2.Text;
//                item.SubItems[3].Text = textBox3.Text;
            }
        }

        private void btnApplyChangings_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void btnColorBackground_Click(object sender, EventArgs e)
        { applyColor(btnColorBackground, "colorBackground"); }
        private void btnColorRuler_Click(object sender, EventArgs e)
        { applyColor(btnColorRuler, "colorRuler"); }
        private void btnColorPenUp_Click(object sender, EventArgs e)
        { applyColor(btnColorPenUp, "colorPenUp"); }
        private void btnColorPenDown_Click(object sender, EventArgs e)
        { applyColor(btnColorPenDown, "colorPenDown"); }
        private void btnColorTool_Click(object sender, EventArgs e)
        { applyColor(btnColorTool, "colorTool");        }
        private void btnColorMarker_Click(object sender, EventArgs e)
        { applyColor(btnColorMarker, "colorMarker"); }
        private void btnColorHeightMap_Click(object sender, EventArgs e)
        { applyColor(btnColorHeightMap, "colorHeightMap"); }

        private void applyColor(Button btn,string settings)
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

        private bool isExpand = false;
        private void expandForm_Click(object sender, EventArgs e)
        {
            if (!isExpand)
            {
                this.Width = 750;
                this.Height = 420;
                btnResizeForm.Text = "reduce <";
                isExpand = true;
            } 
       }

        private void btnResizeForm_Click(object sender, EventArgs e)
        {
            if (!isExpand)
            {
                this.Width = 750;
                this.Height = 400;
                btnResizeForm.Text = "reduce <";
                isExpand = true;
            }
            else
            {
                this.Width = 260;
                this.Height = 365;
                btnResizeForm.Text = "expand >";
                isExpand = false;
            }
        }

        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.locationSetForm = Location;
            saveSettings();  }

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
        {   Process.Start(@"https://openclipart.org/tags/svg");   }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   Process.Start(@"https://publicdomainvectors.org/"); }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   Process.Start(@"https://simplemaps.com/"); }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   Process.Start(@"http://www.cliparts101.com/"); }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   Process.Start(@"http://www.clker.com/"); }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   Process.Start(@"https://free.clipartof.com/"); }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   Process.Start(@"https://github.com/gnea/grbl/wiki"); }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {   Process.Start(@"http://linuxcnc.org/docs/html/gcode.html"); }

        private void tabPage6_Enter(object sender, EventArgs e)
        {   timer1.Enabled = true;   }

        private void tabPage6_Leave(object sender, EventArgs e)
        {   timer1.Enabled = false;  }

        private void timer1_Tick(object sender, EventArgs e)
        {   try
            {
                ControlGamePad.gamePad.Poll();
                var datas = ControlGamePad.gamePad.GetBufferedData();
                foreach (var state in datas)
                {
                    lblgp.Text = state.Offset + " Value: " + state.Value.ToString();
                    processGamepad(state);
                }
            }
            catch
            {
                try { ControlGamePad.Initialize(); timer1.Interval = 200; }
                catch { timer1.Interval = 5000; }
            }
        }
        private void processGamepad(SharpDX.DirectInput.JoystickUpdate state)
        {   string offset = state.Offset.ToString();
            int value = state.Value;
            if (offset.IndexOf("Buttons") >= 0)
            {   foreach (Control c in this.gBGP.Controls)
                {
                    if (c.Name == ("lbl" + offset)) if (c != null)
                    {   c.BackColor = (value > 0) ? Color.Lime : Color.LightGray; break; }
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
        {   hsFilterScrollSetLabels(); }
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
            int index = Convert.ToUInt16(clickedButton.Name.Substring("btnShapeSetSave".Length));
  //          MessageBox.Show(index.ToString());
            if (index == 1) { Properties.Settings.Default.camShapeSet1 = shapeSetSave(tBShapeSet1.Text); }
            if (index == 2) { Properties.Settings.Default.camShapeSet2 = shapeSetSave(tBShapeSet2.Text); }
            if (index == 3) { Properties.Settings.Default.camShapeSet3 = shapeSetSave(tBShapeSet3.Text); }
            if (index == 4) { Properties.Settings.Default.camShapeSet4 = shapeSetSave(tBShapeSet4.Text); }
            Properties.Settings.Default.Save();
        }

        private void btnShapeSetLoad_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int index = Convert.ToUInt16(clickedButton.Name.Substring("btnShapeSetLoad".Length));
 //           MessageBox.Show(index.ToString());
            if (index == 1) { tBShapeSet1.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet1); }
            if (index == 2) { tBShapeSet2.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet2); }
            if (index == 3) { tBShapeSet3.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet3); }
            if (index == 4) { tBShapeSet4.Text = shapeSetLoad(Properties.Settings.Default.camShapeSet4); }
        }

        private String shapeSetSave(string head)
        {   string txt = head+"|";
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
        {   string[] value = txt.Split('|');
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

        private void setLabelParameterSet(int index, string txt)
        {   if (index == 1) { tBShapeSet1.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 2) { tBShapeSet2.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 3) { tBShapeSet3.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
            if (index == 4) { tBShapeSet4.Text = (txt.Length == 0) ? "not set" : txt.Substring(0, txt.IndexOf('|')); }
        }

    }
}
