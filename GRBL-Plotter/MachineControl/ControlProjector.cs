/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2024 Sven Hasemann contact: svenhb@web.de

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
 * 2022-03-25 New form to be displayed with a projector on the work-piece
 * 2022-04-06 Add buttons to minimize / maximize windows, monitor selection
 * 2024-05-02 l:230 f:BtnProjectorCalc_Click check min/max before setting new NudScaling.Value
*/


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class ControlProjector : Form
    {
        private readonly Pen penUp = new Pen(Color.Green, 0.1F);
        private readonly Pen penDown = new Pen(Color.White, 0.2F);
        private readonly Pen penTool = new Pen(Color.Gray, 0.5F);
        private readonly Pen penMarker = new Pen(Color.DeepPink, 1F);
        private readonly Pen penDimension = new Pen(Color.LightGray, 1F);
        private readonly Pen penRuler = new Pen(Color.LightGray, 0.1F);
        private readonly Pen penGrid1 = new Pen(Color.LightGray, 0.01F);
        private readonly Pen penGrid10 = new Pen(Color.LightSlateGray, 0.01F);
        private readonly Pen penGrid100 = new Pen(Color.Gray, 0.1F);

        public ControlProjector()
        {
            InitializeComponent();
        }

        private void ControlProjector_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Icon;
            SetButtonColors(btnColorBackground, Properties.Settings.Default.projectorColorBackground);
            SetButtonColors(btnColorDimension, Properties.Settings.Default.projectorColorDimension);
            SetButtonColors(btnColorRuler, Properties.Settings.Default.projectorColorRuler);
            SetButtonColors(btnColorPenUp, Properties.Settings.Default.projectorColorPenUp);
            SetButtonColors(btnColorPenDown, Properties.Settings.Default.projectorColorPenDown);
            SetButtonColors(btnColorTool, Properties.Settings.Default.projectorColorTool);
            SetButtonColors(btnColorMarker, Properties.Settings.Default.projectorColorMarker);
            SetupPens();
            SetupPanel.Visible = Properties.Settings.Default.projectorShowSetup;
        }

        private void ControlProjector_Paint(object sender, PaintEventArgs e)
        {
            double offset = (double)Properties.Settings.Default.machineLimitsRangeX / 40;       // view size
            double minx = (double)Properties.Settings.Default.machineLimitsHomeX - Grbl.posWCO.X - offset;
            double miny = (double)Properties.Settings.Default.machineLimitsHomeY - Grbl.posWCO.Y - offset;
            double xRange = (double)Properties.Settings.Default.machineLimitsRangeX + 2 * offset;
            double yRange = (double)Properties.Settings.Default.machineLimitsRangeY + 2 * offset;

            double picScaling = Math.Min(this.Width / (xRange), this.Height / (yRange)) * (double)NudScaling.Value;               // calculate scaling px/unit
            double zoomFactor = 1;// (double)NudScaling.Value;

            e.Graphics.ScaleTransform((float)picScaling, (float)-picScaling);           // apply scaling (flip Y)
            e.Graphics.TranslateTransform((float)-minx + (float)NudOffsetX.Value, (float)(-yRange - miny) + (float)NudOffsetY.Value);       // apply offset

            if (Properties.Settings.Default.projectorShowRuler)
            {
                if ((picScaling * zoomFactor) > 10)
                    e.Graphics.DrawPath(penGrid1, VisuGCode.pathGrid1);          // grid   1mm
                if ((picScaling * zoomFactor) > 2)
                    e.Graphics.DrawPath(penGrid10, VisuGCode.pathGrid10);        // grid  10mm
                if ((picScaling * zoomFactor) > 0.1)
                    e.Graphics.DrawPath(penGrid100, VisuGCode.pathGrid100);      // grid 100mm
                if ((picScaling * zoomFactor) > 0.01)
                    e.Graphics.DrawPath(penGrid100, VisuGCode.pathGrid1000);     // grid 1000mm
                e.Graphics.DrawPath(penGrid100, VisuGCode.pathGrid10000);        // grid 10000mm
                e.Graphics.DrawPath(penRuler, VisuGCode.pathRuler);
            }

            if (Properties.Settings.Default.projectorShowTool)
                e.Graphics.DrawPath(penTool, VisuGCode.pathTool);

            if (Properties.Settings.Default.projectorShowMarker)
                e.Graphics.DrawPath(penMarker, VisuGCode.pathMarker);

            if (Properties.Settings.Default.projectorShowDimension)
                e.Graphics.DrawPath(penDimension, VisuGCode.pathDimension);

            e.Graphics.DrawPath(penDown, VisuGCode.pathPenDown);

            if (Properties.Settings.Default.projectorShowPenUp)
                e.Graphics.DrawPath(penUp, VisuGCode.pathPenUp);
        }

        private void SetupPens()
        {
            this.BackColor = Properties.Settings.Default.projectorColorBackground;
            SetupPanel.BackColor = SystemColors.Control;

            penUp.Color = Properties.Settings.Default.projectorColorPenUp;
            penDown.Color = Properties.Settings.Default.projectorColorPenDown;
            penTool.Color = Properties.Settings.Default.projectorColorTool;
            penMarker.Color = Properties.Settings.Default.projectorColorMarker;
            penDimension.Color = Properties.Settings.Default.projectorColorDimension;
            penRuler.Color = Properties.Settings.Default.projectorColorRuler;

            float factorWidth = 1;
            penUp.Width = (float)Properties.Settings.Default.projectorWidthPenUp * factorWidth;
            penDown.Width = (float)Properties.Settings.Default.projectorWidthPenDown * factorWidth;
            penTool.Width = (float)Properties.Settings.Default.projectorWidthTool * factorWidth;
            penMarker.Width = (float)Properties.Settings.Default.projectorWidthMarker * factorWidth;
            penDimension.Width = 2 * (float)Properties.Settings.Default.projectorWidthPenDown * factorWidth;
            penRuler.Width = (float)Properties.Settings.Default.projectorWidthRuler * factorWidth;

            penUp.LineJoin = LineJoin.Round;
            penDown.LineJoin = LineJoin.Round;
            penDown.StartCap = LineCap.Round;
            penDown.EndCap = LineCap.Round;
            penTool.LineJoin = LineJoin.Round;
            penMarker.LineJoin = LineJoin.Round;
            penDimension.LineJoin = LineJoin.Round;
        }

        private void BtnColorBackground_Click(object sender, EventArgs e)
        { ApplyColor(btnColorBackground, "projectorColorBackground"); }
        private void BtnColorDimension_Click(object sender, EventArgs e)
        { ApplyColor(btnColorDimension, "projectorColorDimension"); }
        private void BtnColorRuler_Click(object sender, EventArgs e)
        { ApplyColor(btnColorRuler, "projectorColorRuler"); }
        private void BtnColorPenUp_Click(object sender, EventArgs e)
        { ApplyColor(btnColorPenUp, "projectorColorPenUp"); }
        private void BtnColorPenDown_Click(object sender, EventArgs e)
        { ApplyColor(btnColorPenDown, "projectorColorPenDown"); }
        private void BtnColorTool_Click(object sender, EventArgs e)
        { ApplyColor(btnColorTool, "projectorColorTool"); }
        private void BtnColorMarker_Click(object sender, EventArgs e)
        { ApplyColor(btnColorMarker, "projectorColorMarker"); }

        private void ApplyColor(Button btn, string settings)
        {
            using (ColorDialog colorDialog1 = new ColorDialog())
            {
                colorDialog1.AnyColor = true;
                colorDialog1.Color = (Color)Properties.Settings.Default[settings];
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    SetButtonColors(btn, colorDialog1.Color);
                    Properties.Settings.Default[settings] = colorDialog1.Color;
                }
            }
            SetupPens();
            this.Invalidate();
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

        private void BtnClose_Click(object sender, EventArgs e)
        {
            SetupPanel.Visible = false;
        }

        private void ControlProjector_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                SetupPanel.Visible = true;
            }
        }

        private void CbShow_CheckedChanged(object sender, EventArgs e)
        {
            SetupPens();
            this.Invalidate();
        }

        private Point p = new Point();
        private void SetupPanel_MouseDown(object sender, MouseEventArgs e)
        {
            p.X = e.X;
            p.Y = e.Y;
        }

        private void SetupPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SetupPanel.Location = new Point(e.X - p.X + SetupPanel.Location.X, e.Y - p.Y + SetupPanel.Location.Y);
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
        }

        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        private void BtnProjectorCalc_Click(object sender, EventArgs e)
        {
            decimal scaling = NudProjectorSet.Value / NudProjectorReal.Value;
			decimal newScaling = NudScaling.Value * scaling;
			if ((newScaling >= NudScaling.Minimum) && (newScaling <= NudScaling.Maximum))
            {	
				NudScaling.Value = newScaling;
				NudProjectorReal.Value = NudProjectorSet.Value;
			}
        }

        private void CbProjectorScaleEnable_CheckedChanged(object sender, EventArgs e)
        {
            GbProjectorScale.Enabled = CbProjectorScaleEnable.Checked;
        }
    }
}
