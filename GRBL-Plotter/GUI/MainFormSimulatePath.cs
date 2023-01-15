/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2023 Sven Hasemann contact: svenhb@web.de

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
 * 2020-03-11 split from MainForm.cs
 * 2021-07-02 code clean up / code quality
 * 2021-12-02 add range test for index
 * 2022-03-29 line 115 check if (_serial_form != null)
 * 2022-04-07 reset codeInfo on start
 * 2023-01-07 use SetTextThreadSave(lbInfo...
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MainForm
    {
        #region simulate path
        private static int simuLine = 0;
        private static bool simuEnabled = false;
        private static XyzPoint codeInfo = new XyzPoint();
        private static bool simulateA = false;

        private void BtnSimulate_Click(object sender, EventArgs e)
        {
            if ((!isStreaming) && (fCTBCode.LinesCount > 2))
            {
                codeInfo = new XyzPoint();  // reset old positions
                if (!simuEnabled)
                { SimuStart(Properties.Settings.Default.gui2DColorSimulation); }
                else
                { SimuStop(); }
            }
        }
        private void SimuStart(Color col)
        {
            label_wx.ForeColor = col;
            label_wy.ForeColor = col;
            label_wz.ForeColor = col;
            label_wa.ForeColor = col;
            label_wb.ForeColor = col;
            label_wc.ForeColor = col;

            Color invers = ContrastColor(col);
            label_wx.BackColor = invers;
            label_wy.BackColor = invers;
            label_wz.BackColor = invers;
            label_wa.BackColor = invers;
            label_wb.BackColor = invers;
            label_wc.BackColor = invers;

            simulateA = VisuGCode.ContainsTangential();
            if (simulateA)
                UpdateWholeApplication();

            pbFile.Maximum = fCTBCode.LinesCount;

            if (LineIsInRange(fCTBCodeClickedLineLast))
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);

            btnSimulate.Text = Localization.GetString("mainSimuStop");
            simuLine = 0;
            fCTBCodeClickedLineNow = simuLine;
            fCTBCodeClickedLineLast = simuLine;
            simuEnabled = true;
            simulationTimer.Enabled = true;
            //         VisuGCode.markSelectedFigure(-1);
            //         VisuGCode.setPosMarkerLine(simuLine, false);
            btnStreamStart.Enabled = false;
            btnStreamStop.Enabled = false;
            btnStreamCheck.Enabled = false;
            btnSimulateFaster.Enabled = true;
            btnSimulateSlower.Enabled = true;
            VisuGCode.Simulation.Reset();

            double factor = 100 * VisuGCode.Simulation.dt / 50;
            lblElapsed.Text = string.Format("{0} {1:0}%", Localization.GetString("mainSimuSpeed"), factor);
            btnSimulatePause.Visible = true;
            lbInfo.BackColor = System.Drawing.Color.LightGreen;
        }
        private void SimuStop()
        {
            label_wx.ForeColor = Color.Black;
            label_wy.ForeColor = Color.Black;
            label_wz.ForeColor = Color.Black;
            label_wa.ForeColor = Color.Black;
            label_wb.ForeColor = Color.Black;
            label_wc.ForeColor = Color.Black;
            Color invers = Control.DefaultBackColor;
            label_wx.BackColor = invers;
            label_wy.BackColor = invers;
            label_wz.BackColor = invers;
            label_wa.BackColor = invers;
            label_wb.BackColor = invers;
            label_wc.BackColor = invers;

            if (simulateA)
            {
                simulateA = false;
                UpdateWholeApplication();
            }

            bool isConnected = false;
            if (((_serial_form != null) && (_serial_form.SerialPortOpen)) || Grbl.grblSimulate)
                isConnected = true;

            simuEnabled = false;
            simulationTimer.Enabled = false;
            btnSimulate.Text = Localization.GetString("mainSimuStart");
            pbFile.Value = 0;
            btnStreamStart.Enabled = isConnected;
            btnStreamStop.Enabled = isConnected;
            btnStreamCheck.Enabled = isConnected;
            btnSimulateFaster.Enabled = false;
            btnSimulateSlower.Enabled = false;
            lblFileProgress.Text = string.Format("{0} {1:0.0}%", Localization.GetString("mainProgress"), 0);
            lblElapsed.Text = "Time";
            btnSimulatePause.Visible = false;
            lbInfo.BackColor = System.Drawing.SystemColors.Control;
            VisuGCode.Simulation.pathSimulation.Reset();
            pictureBox1.Invalidate();
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            simuLine = VisuGCode.Simulation.Next(ref codeInfo);

            if (LineIsInRange(simuLine))   //(simuLine >= 0)
            {
                SetTextThreadSave(lbInfo, string.Format("Line {0}: {1}", (simuLine + 1), fCTBCode.Lines[simuLine]));

                fCTBCode.Selection = fCTBCode.GetLine(simuLine);

                if (LineIsInRange(fCTBCodeClickedLineLast))
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);

                //    fCTBCode.BookmarkLine(simuLine);
                if (this.fCTBCode.InvokeRequired)
                { this.fCTBCode.BeginInvoke((MethodInvoker)delegate () { this.fCTBCode.BookmarkLine(simuLine); }); }
                else
                { this.fCTBCode.BookmarkLine(simuLine); }

                fCTBCode.DoCaretVisible();
                fCTBCodeClickedLineLast = simuLine;
                pictureBox1.Invalidate(); // avoid too much events

                label_wx.Text = string.Format("{0:0.000}", codeInfo.X);
                label_wy.Text = string.Format("{0:0.000}", codeInfo.Y);
                label_wz.Text = string.Format("{0:0.000}", codeInfo.Z);
                label_wa.Text = string.Format("{0:0.0}", codeInfo.A * 180 / Math.PI);
            }
            else
            {
                SetTextThreadSave(lbInfo, string.Format("Line {0}", (simuLine + 1)));
                SimuStop();
                simuLine = 0;   // Math.Abs(simuLine);
                VisuGCode.Simulation.Reset();
                FastColoredTextBoxNS.Range mySelection = fCTBCode.Range;
                FastColoredTextBoxNS.Place selStart;
                selStart.iLine = 0;
                selStart.iChar = 0;
                mySelection.Start = selStart;
                mySelection.End = selStart;
                fCTBCode.Selection = mySelection;

                if (LineIsInRange(fCTBCodeClickedLineLast))
                    fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);

                //    fCTBCode.BookmarkLine(simuLine);
                if (this.fCTBCode.InvokeRequired)
                { this.fCTBCode.BeginInvoke((MethodInvoker)delegate () { this.fCTBCode.BookmarkLine(simuLine); }); }
                else
                { this.fCTBCode.BookmarkLine(simuLine); }

                fCTBCode.DoCaretVisible();
                fCTBCodeClickedLineLast = simuLine;
                VisuGCode.Simulation.pathSimulation.Reset();
                pictureBox1.Invalidate(); // avoid too much events
                SetTextThreadSave(lbInfo, string.Format("Simulation finished"));
                return;
            }
            if (pbFile.Maximum < simuLine)
            {
                Logger.Error("SimulationTimer_Tick pbFile.Maximum < simuLine {0} {1}", pbFile.Maximum, simuLine);
                pbFile.Maximum = simuLine;
            }
            pbFile.Value = simuLine;
            lblFileProgress.Text = string.Format("{0} {1:0.0}%", Localization.GetString("mainProgress"), (100 * simuLine / (fCTBCode.LinesCount - 2)));
            pictureBox1.Invalidate();
        }

        private void BtnSimulateFaster_Click(object sender, EventArgs e)
        {
            VisuGCode.Simulation.dt *= 2;
            if (VisuGCode.Simulation.dt > 102400)
                VisuGCode.Simulation.dt = 102400;
            double factor = 100 * VisuGCode.Simulation.dt / 50;
            lblElapsed.Text = string.Format("{0} {1:0}%", Localization.GetString("mainSimuSpeed"), factor);
        }

        private void BtnSimulateSlower_Click(object sender, EventArgs e)
        {
            VisuGCode.Simulation.dt /= 2;
            if (VisuGCode.Simulation.dt < 25)
                VisuGCode.Simulation.dt = 25;
            double factor = 100 * VisuGCode.Simulation.dt / 50;
            lblElapsed.Text = string.Format("{0} {1:0}%", Localization.GetString("mainSimuSpeed"), factor);
        }
        private void BtnSimulatePause_Click(object sender, EventArgs e)
        {
            bool tmp = simulationTimer.Enabled;
            simulationTimer.Enabled = !tmp;
            if (tmp)
                btnSimulatePause.Image = Properties.Resources.btn_play;
            else
                btnSimulatePause.Image = Properties.Resources.btn_pause;
        }

        #endregion
    }
}
