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
 * 2020-03-11 split from MainForm.cs
 *
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GRBL_Plotter
{   public partial class MainForm
    {
        #region simulate path
        private static int simuLine = 0;
        private static bool simuEnabled = false;
		private static xyzPoint codeInfo = new xyzPoint();
        private static bool simulateA = false;

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            if ((!isStreaming) && (fCTBCode.LinesCount > 2))
            {
                if (!simuEnabled)
                { simuStart(Properties.Settings.Default.gui2DColorSimulation); }
                else
                { simuStop(); }
            }
        }
        private void simuStart(Color col)
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

            simulateA = VisuGCode.containsTangential();
            if (simulateA)
                updateLayout();

            pbFile.Maximum = fCTBCode.LinesCount;
            fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
            btnSimulate.Text = Localization.getString("mainSimuStop");
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
            lblElapsed.Text = string.Format("{0} {1:0}%", Localization.getString("mainSimuSpeed"), factor);
            btnSimulatePause.Visible = true;
            lbInfo.BackColor = System.Drawing.Color.LightGreen;
        }
        private void simuStop()
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
            {   simulateA = false;
                updateLayout();
            }

            bool isConnected = _serial_form.serialPortOpen || grbl.grblSimulate;
            simuEnabled = false;
            simulationTimer.Enabled = false;
            btnSimulate.Text = Localization.getString("mainSimuStart");
            pbFile.Value = 0;
            btnStreamStart.Enabled = isConnected;
            btnStreamStop.Enabled = isConnected;
            btnStreamCheck.Enabled = isConnected;
            btnSimulateFaster.Enabled = false;
            btnSimulateSlower.Enabled = false;
            lblFileProgress.Text = string.Format("{0} {1:0.0}%", Localization.getString("mainProgress"), 0);
            lblElapsed.Text = "Time";
            btnSimulatePause.Visible = false;
            lbInfo.BackColor = System.Drawing.SystemColors.Control;
            VisuGCode.Simulation.pathSimulation.Reset();
            pictureBox1.Invalidate();
        }

        private void simulationTimer_Tick(object sender, EventArgs e)
        {
            simuLine = VisuGCode.Simulation.Next(ref codeInfo);
			
            if ((simuLine >= 0) && (simuLine < fCTBCode.Lines.Count()))
                lbInfo.Text = string.Format("Line {0}: {1}",(simuLine+1), fCTBCode.Lines[simuLine] );
            else
                lbInfo.Text = string.Format("Line {0}", (simuLine + 1));

            if (simuLine >= 0)
            {   fCTBCode.Selection = fCTBCode.GetLine(simuLine);
                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                fCTBCode.BookmarkLine(simuLine);
                fCTBCode.DoCaretVisible();
                fCTBCodeClickedLineLast = simuLine;
                pictureBox1.Invalidate(); // avoid too much events
				
				label_wx.Text = string.Format("{0:0.000}", codeInfo.X);
				label_wy.Text = string.Format("{0:0.000}", codeInfo.Y);
				label_wz.Text = string.Format("{0:0.000}", codeInfo.Z);
				label_wa.Text = string.Format("{0:0.0}", codeInfo.A*180/Math.PI);
            }
            else
            {   simuStop();
                simuLine = 0;   // Math.Abs(simuLine);
                VisuGCode.Simulation.Reset();
                FastColoredTextBoxNS.Range mySelection = fCTBCode.Range;
                FastColoredTextBoxNS.Place selStart;
                selStart.iLine = 0;
                selStart.iChar = 0;
                mySelection.Start = selStart;
                mySelection.End = selStart;
                fCTBCode.Selection = mySelection;

                fCTBCode.UnbookmarkLine(fCTBCodeClickedLineLast);
                fCTBCode.BookmarkLine(simuLine);
                fCTBCode.DoCaretVisible();
                fCTBCodeClickedLineLast = simuLine;
                VisuGCode.Simulation.pathSimulation.Reset();
                pictureBox1.Invalidate(); // avoid too much events
                lbInfo.Text = string.Format("Simulation finished");
                return;
            }
            pbFile.Value = simuLine;
            lblFileProgress.Text = string.Format("{0} {1:0.0}%", Localization.getString("mainProgress"), (100 * simuLine / (fCTBCode.LinesCount - 2)));
            pictureBox1.Invalidate();
        }

        private void btnSimulateFaster_Click(object sender, EventArgs e)
        {
            VisuGCode.Simulation.dt *= 2;
            if (VisuGCode.Simulation.dt > 102400)
                VisuGCode.Simulation.dt = 102400;
            double factor = 100 * VisuGCode.Simulation.dt / 50;
            lblElapsed.Text = string.Format("{0} {1:0}%", Localization.getString("mainSimuSpeed"), factor);
        }

        private void btnSimulateSlower_Click(object sender, EventArgs e)
        {
            VisuGCode.Simulation.dt /= 2;
            if (VisuGCode.Simulation.dt < 25)
                VisuGCode.Simulation.dt = 25;
            double factor = 100 * VisuGCode.Simulation.dt / 50;
            lblElapsed.Text = string.Format("{0} {1:0}%", Localization.getString("mainSimuSpeed"), factor);
        }
        private void btnSimulatePause_Click(object sender, EventArgs e)
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
