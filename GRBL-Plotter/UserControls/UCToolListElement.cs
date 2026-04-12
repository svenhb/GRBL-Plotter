/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static GrblPlotter.DeviceToolProperties;

namespace GrblPlotter.UserControls
{
    public partial class UCToolListElement : UserControl
    {
        private readonly float DpiScaling;
        private int visibleTab = 0;
        private bool controlEnabled = true;
        public int Id { get; set; }
        internal ToolProperty toolProp = new ToolProperty();
        internal DeviceToolProperties[] device = { new DeviceToolProperties(), new DeviceToolProperties(), new DeviceToolProperties() };
        public bool SettingChanged { get; set; }
        public bool PlotterUseS { get; set; }
        public int Grouping { get; set; }
        public bool AllowDrag { get; set; }

        public event EventHandler<XYEventArgs> RaiseXYEvent;
        protected virtual void OnRaiseXYEvent(XYEventArgs e)
        { RaiseXYEvent?.Invoke(this, e); }

        internal ToolProperty GetToolProperty()
        {
            GetToolProps();
            return toolProp;
        }
        private void SetNud(NumericUpDown tmp, decimal val)
        {
            if (val < tmp.Minimum) { val = tmp.Minimum; }
            if (val > tmp.Maximum) { val = tmp.Maximum; }
            tmp.Value = val;
        }

        public UCToolListElement()
        {
            InitializeComponent();
        }
        internal UCToolListElement(ToolProperty tools, int groupBy)
        {
            /* SetToolProps */
            Grouping = groupBy;
            toolProp = tools.Copy();
            device[0] = toolProp.Laser;
            device[1] = toolProp.Plotter;
            device[2] = toolProp.Router;

            DpiScaling = (float)DeviceDpi / 96;
            AllowDrag = true;

            BackColor = Color.LightBlue;
            SettingChanged = false;

            InitializeComponent();

            SetNud(nudLaserDiameter, (decimal)toolProp.Laser.Diameter);
            tbName.Text = toolProp.ToolName;//ToolName;

            nudCoordX.Value = (decimal)toolProp.Position.X;
            nudCoordY.Value = (decimal)toolProp.Position.Y;
            nudCoordZ.Value = (decimal)toolProp.Position.Z;
            nudCoordA.Value = (decimal)toolProp.Position.A;
            tbGcode.Text = toolProp.Gcode;

            SetNud(nudLaserDiameter, (decimal)toolProp.Laser.Diameter);
            SetNud(nudLaserFeedXY, (decimal)toolProp.Laser.FeedXY);
            SetNud(nudLaserPower, (decimal)toolProp.Laser.FinalS);
            SetNud(nudLaserPasses, (decimal)toolProp.Laser.Passes);
            cbLaserM3.Checked = toolProp.Laser.UseM3;
            cbLaserAir.Checked = toolProp.Laser.UseAir;//LaserAir;

            SetNud(nudPlotterDiameter, (decimal)toolProp.Plotter.Diameter);
            SetNud(nudPlotterFeedXY, (decimal)toolProp.Plotter.FeedXY);
            SetNud(nudPlotterSPD, (decimal)toolProp.Plotter.FinalS);
            SetNud(nudPlotterZPD, (decimal)toolProp.Plotter.FinalZ);
            CbPlotterUseLaser.Checked = toolProp.Plotter.UseSorZ;

            SetNud(nudRouterDiameter, (decimal)toolProp.Router.Diameter);
            SetNud(nudRouterFeedXY, (decimal)toolProp.Router.FeedXY);
            SetNud(nudRouterFeedZ, (decimal)toolProp.Router.FeedZ);
            SetNud(nudRouterZPD, (decimal)toolProp.Router.FinalZ);

            ContextMenu contextMenu = new ContextMenu();
            this.tbName.ContextMenu = contextMenu;
            this.tbGcode.ContextMenu = contextMenu;
            this.nudLaserDiameter.ContextMenu = contextMenu;
            this.nudLaserFeedXY.ContextMenu = contextMenu;
            this.nudLaserPower.ContextMenu = contextMenu;
            this.nudLaserPasses.ContextMenu = contextMenu;
            this.nudPlotterDiameter.ContextMenu = contextMenu;
            this.nudPlotterFeedXY.ContextMenu = contextMenu;
            this.nudPlotterSPD.ContextMenu = contextMenu;
            this.nudPlotterZPD.ContextMenu = contextMenu;
            this.nudRouterDiameter.ContextMenu = contextMenu;
            this.nudRouterFeedXY.ContextMenu = contextMenu;
            this.nudRouterFeedZ.ContextMenu = contextMenu;
            this.nudRouterZPD.ContextMenu = contextMenu;

            this.nudCoordX.ContextMenuStrip = CmsMoveTo;
            this.nudCoordY.ContextMenuStrip = CmsMoveTo;
            this.nudCoordZ.ContextMenuStrip = CmsMoveTo;
            this.nudCoordA.ContextMenuStrip = CmsMoveTo;
        }
        internal void PresetCoordinates(double x, double y, double z, double a)
        {
            nudCoordX.Value = (decimal)x;
            nudCoordY.Value = (decimal)y;
            nudCoordZ.Value = (decimal)z;
            nudCoordA.Value = (decimal)a;
            GetToolProps();
        }
        private void GetToolProps()
        {
            toolProp.Position = new XyzPoint((double)nudCoordX.Value, (double)nudCoordY.Value, (double)nudCoordZ.Value, (double)nudCoordA.Value);
            toolProp.Gcode = tbGcode.Text;
            toolProp.ToolName = tbName.Text;

            toolProp.Laser.FeedXY = (float)nudLaserFeedXY.Value;
            toolProp.Laser.FinalS = (float)nudLaserPower.Value;
            toolProp.Laser.Passes = (int)nudLaserPasses.Value;
            toolProp.Laser.UseM3 = cbLaserM3.Checked;
            toolProp.Laser.UseAir = cbLaserAir.Checked;

            toolProp.Plotter.FeedXY = (float)nudPlotterFeedXY.Value;
            toolProp.Plotter.FinalS = (float)nudPlotterSPD.Value;
            toolProp.Plotter.FinalZ = (float)nudPlotterZPD.Value;
            toolProp.Plotter.UseSorZ = CbPlotterUseLaser.Checked;

            toolProp.Router.FeedXY = (float)nudRouterFeedXY.Value;
            toolProp.Router.FeedZ = (float)nudRouterFeedZ.Value;
            toolProp.Router.FinalZ = (float)nudRouterZPD.Value;
        }
        public void SwitchView(int tabDevice, int tabPlotterMode)
        {
            PlotterUseS = (tabPlotterMode == 0);
            visibleTab = tabDevice;
            SetVisible(0, (tabDevice == 0));
            SetVisible(1, (tabDevice == 1));
            SetVisible(2, (tabDevice == 2));
            SetVisible(3, (tabDevice == 3));
            SetFillBtn(tabDevice);
        }
        public void SetEnable(bool en)
        {
            panelLaser.Enabled = en;
            panelPlotter.Enabled = en;
            panelRouter.Enabled = en;
            panelCoordinates.Enabled = en;
            SetFillBtn(visibleTab);
            controlEnabled = en;
            Invalidate();
        }
        private void SetFillBtn(int tabDevice)
        {
            if (tabDevice == 0)
                btnSetupFill.BackColor = toolProp.Laser.Fill.Enable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            else if (tabDevice == 1)
                btnSetupFill.BackColor = toolProp.Plotter.Fill.Enable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            else
                btnSetupFill.BackColor = toolProp.Router.Fill.Enable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            if (!btnSetupFill.Enabled)
                btnSetupFill.BackColor = Colors.GrayColor(btnSetupFill.BackColor);
        }

        private void SetVisible(int tab, bool en)
        {
            if (tab == 0)
            {
                panelLaser.Visible = en;
            }
            if (tab == 1)
            {
                panelPlotter.Visible = en;
                if (en)
                {
                    nudPlotterZPD.Visible = !PlotterUseS;
                    nudPlotterSPD.Visible = PlotterUseS;
                    CbPlotterUseLaser.Visible = !PlotterUseS;
                }
            }
            if (tab == 2)
            {
                panelRouter.Visible = en;
            }
            if (tab == 3)
            {
                panelCoordinates.Visible = en;
            }
        }

        private void UCToolListElement_Paint(object sender, PaintEventArgs e)
        {
            Graphics _graphics;
            // Textformat
            StringFormat stringCenter = new StringFormat();
            stringCenter.Alignment = StringAlignment.Center;
            stringCenter.LineAlignment = StringAlignment.Center;

            _graphics = this.CreateGraphics();
            _graphics.Clear(Parent.BackColor); //SystemColors.Control);

            _graphics.FillPath(new SolidBrush(BackColor), GetFigurePath());
            _graphics.DrawPath(new Pen(Color.Black, 1), GetFigurePath());

            int dotSize = ClientRectangle.Height - 6;
            int off = 3;
            //	Color cshow = controlEnabled? toolProp.GroupColor:Colors.GrayColor(toolProp.GroupColor);
            Color cshow = toolProp.GroupColor;
            if (Grouping == 0)
            {
                _graphics.FillEllipse(new SolidBrush(cshow), new Rectangle(off, off, dotSize, dotSize));
                _graphics.DrawString(this.toolProp.ToolNr.ToString(), this.Font, new SolidBrush(Colors.ContrastColor(toolProp.GroupColor)), off + dotSize / 2, ClientRectangle.Height / 2, stringCenter);
            }
            else
            {
                _graphics.FillEllipse(new SolidBrush(Color.LightGray), new Rectangle(off, off, dotSize, dotSize));
                _graphics.DrawString(this.toolProp.ToolNr.ToString(), this.Font, new SolidBrush(Color.Black), off + dotSize / 2, ClientRectangle.Height / 2, stringCenter);
            }
        }

        private GraphicsPath GetFigurePath()
        {
            int w = ClientRectangle.Width;
            int h = ClientRectangle.Height;
            int arcSize = h - 1;
            Rectangle leftArc = new Rectangle(0, 0, arcSize, arcSize);
            Rectangle rightArc = new Rectangle(w - arcSize - 2, 0, arcSize, arcSize);

            GraphicsPath path = new GraphicsPath();
            //      path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();

            return path;
        }

        private void UCToolListElement_Load(object sender, EventArgs e)
        {
            int top = 1;
            panelLaser.Top = top;
            panelPlotter.Top = top;
            panelRouter.Top = top;
            panelCoordinates.Top = top;
        }

        private void Nud_ValueChanged(object sender, System.EventArgs e)
        {
            SettingChanged = true;
            OnClick(e);
            BackColor = Color.OrangeRed;
            Invalidate();
        }

        private void CbM3_CheckedChanged(object sender, System.EventArgs e)
        {
            cbLaserM3.Text = cbLaserM3.Checked ? "M3" : "M4";
            SettingChanged = true;
            OnClick(e);
            BackColor = Color.OrangeRed;
            Invalidate();
        }
        private void BtnSetupFill_Click(object sender, System.EventArgs e)
        {
            OptionPropHatchFill tmp = device[visibleTab].Fill;
            List<ControlDefaults> cd = new List<ControlDefaults>();
            OptionPropHatchFill.ControlDefaultsSetList(cd);
            MyControl.TestSimpleSetup("Hatch fill Nr.:" + toolProp.ToolNr.ToString(), Cursor.Position, ref tmp, cd);
            device[visibleTab].Fill = tmp;
            SetFillBtn(visibleTab);
        }


        private bool _isDragging = false;
        private readonly int _DDradius = 10;
        private int _mX = 0;
        private int _mY = 0;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
            _mX = e.X;
            _mY = e.Y;
            this._isDragging = false;
        }

        // https://www.codeproject.com/articles/Using-the-FlowLayoutPanel-and-Reordering-with-Drag#comments-section
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDragging)
            {
                // This is a check to see if the mouse is moving while pressed.
                // Without this, the DragDrop is fired directly when the control is clicked, now you have to drag a few pixels first.
                if (e.Button == MouseButtons.Left && _DDradius > 0 && this.AllowDrag)
                {
                    int num1 = _mX - e.X;
                    int num2 = _mY - e.Y;
                    if (((num1 * num1) + (num2 * num2)) > _DDradius)
                    {
                        DoDragDrop(this, DragDropEffects.All);
                        _isDragging = true;
                        return;
                    }
                }
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
            base.OnMouseUp(e);
        }

        private void CbPlotterUseLaser_CheckedChanged(object sender, EventArgs e)
        {
            nudPlotterZPD.Enabled = !CbPlotterUseLaser.Checked;
            nudPlotterFeedXY.Enabled = !CbPlotterUseLaser.Checked;
            SettingChanged = true;
            OnClick(e);
            BackColor = Color.OrangeRed;
            Invalidate();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OnRaiseXYEvent(new XYEventArgs(0, (double)nudCoordX.Value, (double)nudCoordY.Value, "")); // set new coordinates
        }
    }
}
