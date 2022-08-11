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
 * 2022-07-08 new form
*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.MachineControl
{
    public partial class GrblSetupForm : Form
    {

        private static int displayedSettings = 0;
        public GrblSetupForm()
        {
            this.Icon = Properties.Resources.Icon;
            InitializeComponent();
        }

        private void GrblSetupForm_Load(object sender, EventArgs e)
        {
            if (UpdateTable() > 0)
            {
                Height = 900;
                Top = 0;
            }
        }

        private int UpdateTable()
        {
            float value;
            string descritpion;
            displayedSettings = 0;
            dataGridView1.Rows.Clear();
            dataGridView1.AllowUserToAddRows = true;

            for (int i = 0; i < 200; i++)
            {
                value = Grbl.GetSetting(i);
                if (value > -1)
                {
                    descritpion = Grbl.GetSettingDescription(i.ToString());
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                    row.Cells[0].Value = i;
                    row.Cells[1].Value = value;
                    row.Cells[1].ToolTipText = "Double click to edit";
                    row.Cells[2].Value = descritpion;

                    if ((i >= 100) || (i == 11) || (i == 12) || (i == 24) || (i == 25) || (i == 27))
                    { row.Cells[1].Style.Format = "0.000"; }
                    else
                        row.Cells[1].Style.Format = "0";

                    if (((i >= 2) && (i <= 10)) || ((i >= 13) && (i <= 23)) || (i == 32))
                        row.Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    if (i >= 140)
                        row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                    else if (i >= 130)
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                    else if (i >= 120)
                        row.DefaultCellStyle.BackColor = Color.LightBlue;
                    else if (i >= 110)
                        row.DefaultCellStyle.BackColor = Color.LightCyan;
                    else if ((i >= 100) || ((i >= 2) && (i <= 4)))
                        row.DefaultCellStyle.BackColor = Color.LightYellow;

                    else if (i >= 30)
                        row.DefaultCellStyle.BackColor = Color.LightSkyBlue;
                    else if ((i >= 22) || (i == 5))
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                    else if (i >= 20)
                        row.DefaultCellStyle.BackColor = Color.Orange;

                    dataGridView1.Rows.Add(row);
                    displayedSettings++;
                }
                if (displayedSettings > 0)
                    dataGridView1.Visible = label2.Visible = true;
            }

            dataGridView1.ClearSelection();
            if ((lastEdited >= 0) && (lastEdited < displayedSettings))
                dataGridView1.Rows[lastEdited].Cells[1].Selected = true;

            dataGridView1.AllowUserToAddRows = false;
            return displayedSettings;
        }

        private void GrblSetupForm_SizeChanged(object sender, EventArgs e)
        {
            dataGridView1.Width = Width - 22;
            dataGridView1.Height = Height - 80;// 85;
            dataGridView1.Columns[2].Width = Width - 180;
            label2.Top = Height - 70;
            button1.Top = Height - 75;
            button1.Left = Width - 218;
            linkLabel1.Top = Height - 70;
            linkLabel1.Left = Width - 418;

            if (displayedSettings == 0)
                UpdateTable();
        }

        private void GrblSetupForm_Activated(object sender, EventArgs e)
        {
            if (displayedSettings == 0)
                UpdateTable();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
                dataGridView1.ClearSelection();
        }
        private static int lastEdited = -1;
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                string nr = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string format = dataGridView1.Rows[e.RowIndex].Cells[1].Style.Format;
                string val = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString().Replace(",", ".");
                if (format == "0.000")
                    val = string.Format("{0:0.000}", dataGridView1.Rows[e.RowIndex].Cells[1].Value).Replace(",", ".");

                //    string msg = "Actual: $" + nr + " = " + val;
                //    MessageBox.Show(msg);
                if (ShowInputDialog(ref val, "$" + nr + " =") == DialogResult.OK)
                {
                    SendCommandEvent(new CmdEventArgs("$" + nr + "=" + val));
                    timer1.Start();		// timer1.Interval = 500;
                    lastEdited = e.RowIndex;
                }
            }
        }


        private static DialogResult ShowInputDialog(ref string input, string nr)
        {
            System.Drawing.Size size = new System.Drawing.Size(240, 100);
            Form inputBox = new Form
            {
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = "Grbl Setting"
            };

            System.Windows.Forms.Label label = new Label
            {
                Size = new System.Drawing.Size(80, 23),
                Location = new System.Drawing.Point(5, 12),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Text = nr
            };
            inputBox.Controls.Add(label);

            System.Windows.Forms.TextBox textBox = new TextBox
            {
                Size = new System.Drawing.Size(size.Width - 100, 23),
                Location = new System.Drawing.Point(95, 10),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                TextAlign = System.Windows.Forms.HorizontalAlignment.Right,
                Text = input
            };
            inputBox.Controls.Add(textBox);

            Button okButton = new Button
            {
                DialogResult = System.Windows.Forms.DialogResult.OK,
                Name = "okButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&OK",
                Location = new System.Drawing.Point(size.Width - 80 - 80, 69)
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel,
                Name = "cancelButton",
                Size = new System.Drawing.Size(75, 23),
                Text = "&Cancel",
                Location = new System.Drawing.Point(size.Width - 80, 69)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;
            textBox.SelectionLength = 0;

            inputBox.Location = new Point(MousePosition.X + 40, MousePosition.Y - 20);
            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        public event EventHandler<CmdEventArgs> RaiseCmdEvent;
        protected virtual void SendCommandEvent(CmdEventArgs e)
        {
            EventHandler<CmdEventArgs> handler = RaiseCmdEvent;
            handler?.Invoke(this, e);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateTable();	// timer1.Interval = 500;
            timer1.Stop();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SendCommandEvent(new CmdEventArgs("$$"));
            timer1.Start();
            //	UpdateTable();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LinkLabel clickedLink = sender as LinkLabel;
                Process.Start(clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
         //       Logger.Error(err, "LinkLabel_LinkClicked ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }
    }
}
