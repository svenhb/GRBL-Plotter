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
 * 2020-09-23 new file
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter.GUI
{
    public partial class ButtonEdit : Form
    {
        private int index = 0;
        private Color btnColor = SystemColors.Control;

        // Trace, Debug, Info, Warn, Error, Fatal
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public ButtonEdit(int i)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;

            index = i;
            if ((index >= 1) && (index <= 32))
            {
                string txt = Properties.Settings.Default["guiCustomBtn" + index.ToString()].ToString();
                extractButtonInfo(txt);
            }
            fillComboBoxPreset(Application.StartupPath + datapath.buttons);
        }
        private void extractButtonInfo(string txt)
        {
            string[] values = txt.Split('|');
            if (values.Length > 1)
            {
                tBTitle.Text = values[0];
                tBCode.Text = values[1].Replace(";", "\r\n");
                btnApply.Enabled = true;
            }
            else
                btnApply.Enabled = false;

            if ((values.Length > 2) && (values[2].Length > 3))
                btnColor = ColorTranslator.FromHtml(values[2]);

            setButtonColors(btnSetColor, btnColor);
            lblColor.Text = ColorTranslator.ToHtml(btnColor);
        }

        private Dictionary<int, string> presetButtonText = new Dictionary<int, string>();
        private void fillComboBoxPreset(string Root)
        {
            presetButtonText.Clear();
            List<string> FileArray = new List<string>();
            try
            {
                string[] Files = System.IO.Directory.GetFiles(Root);
                cBPresets.Items.Clear();
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].ToLower().EndsWith("ini"))
                    {
                        string value = getContent(Files[i]);
//                        Logger.Trace("Fill {0}   {1}", Files[i], value);
                        if (value.Length > 1)
                        {
                            cBPresets.Items.Add(Path.GetFileName(Files[i]));
                            presetButtonText.Add(cBPresets.Items.Count-1,value);
                        }
                    }
                }
            }
            catch //(Exception Ex)
            {   //throw (Ex);
            }
        }
        private string getContent(string file)
        {
            if (File.Exists(file))
            {
                string[] readText = File.ReadAllLines(file);
                foreach (string s in readText)
                {
                    if (s.StartsWith("Button"))
                    {   if(s.Contains('='))
                        { return s.Substring(s.IndexOf('=')+1); }
                    }
                }
            }
            return "";
        }
        private void cBPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetButtonText.ContainsKey(cBPresets.SelectedIndex))
                extractButtonInfo(presetButtonText[cBPresets.SelectedIndex]);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string value = tBTitle.Text + "|" + tBCode.Text.Replace("|", ",").Replace("\n", ";").Replace("\r", "")+"|"+ ColorTranslator.ToHtml(btnColor);
            Properties.Settings.Default["guiCustomBtn" + index.ToString()] = value;
            Properties.Settings.Default.Save();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {   Close();   }

        private void btnDelete_Click(object sender, EventArgs e)
        {   tBTitle.Clear();
            tBCode.Clear();
            btnColor = Control.DefaultBackColor;
            setButtonColors(btnSetColor, btnColor);
            btnSetColor.UseVisualStyleBackColor = true;
            lblColor.Text = ColorTranslator.ToHtml(btnColor);
        }
        private void setButtonColors(Button btn, Color col)
        {   btn.BackColor = col;
            btn.ForeColor = ContrastColor(col);
            if (col == Control.DefaultBackColor)
                btn.UseVisualStyleBackColor = true;
        }
        private Color ContrastColor(Color color)
        {   int d = 0;
            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font
            return Color.FromArgb(d, d, d);
        }
        private void applyColor(Button btn)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {   setButtonColors(btn, colorDialog1.Color);
                btnColor = colorDialog1.Color;
                lblColor.Text = ColorTranslator.ToHtml(btnColor);
            }
        }

        private void btnSetColor_Click(object sender, EventArgs e)
        {
            applyColor(btnSetColor);
        }

        private void btnResetColor_Click(object sender, EventArgs e)
        {   btnColor = Control.DefaultBackColor;
            setButtonColors(btnSetColor, btnColor);
            btnSetColor.UseVisualStyleBackColor = true;
            lblColor.Text = ColorTranslator.ToHtml(btnColor);
        }
    }
}
