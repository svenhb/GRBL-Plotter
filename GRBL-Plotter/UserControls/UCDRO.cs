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
using NLog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter.UserControls
{
    public partial class UCDRO : UserControl
    {
        private readonly float DpiScaling;

        private GrblPoint WCO = new GrblPoint();
        private GrblPoint MCO = new GrblPoint();
        private int widthOriginal = 240;
        private bool shrinkENable = true;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public string ZeroString { get; set; }
        public string ZeroCommand { get; set; }
        public int MainWidth { get; set; }

        System.Windows.Forms.Timer foldTimer = new System.Windows.Forms.Timer();
        public UCDRO()
        {
            InitializeComponent();
            ZeroString = "0.000";
            ZeroCommand = "G10L20P0";
            MyControl.DpiScaling = DpiScaling = (float)DeviceDpi / 96;
            GbDRO.Click += GbDRO_Click;
            gBoxDROSetCoord.Click += GbDRO_Click;
            LblX.Click += GbDRO_Click;
            LblY.Click += GbDRO_Click;
            LblZ.Click += GbDRO_Click;
            LblA.Click += GbDRO_Click;
            Lbl_wx.Click += GbDRO_Click;
            Lbl_wy.Click += GbDRO_Click;
            Lbl_wz.Click += GbDRO_Click;
            Lbl_wa.Click += GbDRO_Click;
            GbDRO.MouseEnter += UCDRO_MouseEnter;
            LblX.MouseEnter += UCDRO_MouseEnter;
            LblY.MouseEnter += UCDRO_MouseEnter;
            LblZ.MouseEnter += UCDRO_MouseEnter;
            LblA.MouseEnter += UCDRO_MouseEnter;
            Lbl_wx.MouseEnter += UCDRO_MouseEnter;
            Lbl_wy.MouseEnter += UCDRO_MouseEnter;
            Lbl_wz.MouseEnter += UCDRO_MouseEnter;
            Lbl_wa.MouseEnter += UCDRO_MouseEnter;
            BtnHome.MouseEnter += UCDRO_MouseEnter;
            GbDRO.MouseLeave += UCDRO_MouseLeave;
            LblX.MouseLeave += UCDRO_MouseLeave;
            LblY.MouseLeave += UCDRO_MouseLeave;
            LblZ.MouseLeave += UCDRO_MouseLeave;
            LblA.MouseLeave += UCDRO_MouseLeave;
            Lbl_wx.MouseLeave += UCDRO_MouseLeave;
            Lbl_wy.MouseLeave += UCDRO_MouseLeave;
            Lbl_wz.MouseLeave += UCDRO_MouseLeave;
            Lbl_wa.MouseLeave += UCDRO_MouseLeave;
            BtnHome.MouseLeave += UCDRO_MouseLeave;

            foldTimer.Interval = 500;
            foldTimer.Tick += new System.EventHandler(FoldTimer_Tick);
            foldTimer.Stop();
        }

        private void UserControlDRO_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            ZeroString = "0.000";
            ZeroCommand = "G10L20P0";
            MainWidth = Properties.Settings.Default.mainFormSize.Width;
            SetWidth((MainWidth < 1400) && MyControl.UseToolList());
        }

        public delegate void SetTextWCODelegate();
        public delegate void SetTextMCODelegate();

        internal void SetWCO(GrblPoint val)
        {
            WCO = val;
            if (Lbl_wx.InvokeRequired)
                Lbl_wx.Invoke(new SetTextWCODelegate(DisplayWCO));
            else
                DisplayWCO();
        }

        internal void SetMCO(GrblPoint val)
        {
            MCO = val;
            if (Lbl_mx.InvokeRequired)
                Lbl_mx.Invoke(new SetTextMCODelegate(DisplayMCO));
            else
                DisplayMCO();
        }
        internal void SetGLabel(int val)
        {
            lblCurrentG.Text = "G" + val.ToString();
            lblCurrentG.BackColor = (val == 54) ? Color.Lime : Color.Fuchsia;
        }

        internal void SetSimulationView(bool simu, Color col)
        {
            if (simu)
            {
                Lbl_wx.ForeColor = col;
                Lbl_wy.ForeColor = col;
                Lbl_wz.ForeColor = col;
                Lbl_wa.ForeColor = col;
                Lbl_wb.ForeColor = col;
                Lbl_wc.ForeColor = col;

                Color invers = Colors.ContrastColor(col);
                Lbl_wx.BackColor = invers;
                Lbl_wy.BackColor = invers;
                Lbl_wz.BackColor = invers;
                Lbl_wa.BackColor = invers;
                Lbl_wb.BackColor = invers;
                Lbl_wc.BackColor = invers;
            }
            else
            {
                col = MyControl.PanelForeColor;
                Lbl_wx.ForeColor = col;	//Color.Black;
                Lbl_wy.ForeColor = col;	//Color.Black;
                Lbl_wz.ForeColor = col;	//Color.Black;
                Lbl_wa.ForeColor = col;	//Color.Black;
                Lbl_wb.ForeColor = col;	//Color.Black;
                Lbl_wc.ForeColor = col;	//Color.Black;
                Color invers = MyControl.PanelBackColor;	//Control.DefaultBackColor;
                Lbl_wx.BackColor = invers;
                Lbl_wy.BackColor = invers;
                Lbl_wz.BackColor = invers;
                Lbl_wa.BackColor = invers;
                Lbl_wb.BackColor = invers;
                Lbl_wc.BackColor = invers;
            }
        }
        internal void SetAxisCount(int val, CommandProtocol cp)
        {
            if (val < 4)
            {
                LblA.Visible = Lbl_ma.Visible = Lbl_wa.Visible = BtnZeroA.Visible = false;
                LblB.Visible = Lbl_mb.Visible = Lbl_wb.Visible = BtnZeroB.Visible = false;
                LblC.Visible = Lbl_mc.Visible = Lbl_wc.Visible = BtnZeroC.Visible = false;
                BtnHome.Top = (int)(DpiScaling * 116);
                Height = (int)(DpiScaling * 170);
                this.Width = (int)(DpiScaling * 240);
                this.Height = (int)(DpiScaling * 170);
            }
            else if (val == 4)
            {
                LblA.Visible = Lbl_ma.Visible = Lbl_wa.Visible = BtnZeroA.Visible = true;
                LblB.Visible = Lbl_mb.Visible = Lbl_wb.Visible = BtnZeroB.Visible = false;
                LblC.Visible = Lbl_mc.Visible = Lbl_wc.Visible = BtnZeroC.Visible = false;
                BtnHome.Top = (int)(DpiScaling * 150);
                Height = (int)(DpiScaling * 200);
                this.Width = (int)(DpiScaling * 240);
                this.Height = (int)(DpiScaling * 200);
            }
            else if (val == 5)
            {
                LblA.Visible = Lbl_ma.Visible = Lbl_wa.Visible = BtnZeroA.Visible = true;
                LblB.Visible = Lbl_mb.Visible = Lbl_wb.Visible = BtnZeroB.Visible = true;
                LblC.Visible = Lbl_mc.Visible = Lbl_wc.Visible = BtnZeroC.Visible = false;
            }
            else if (val == 6)
            {
                LblA.Visible = Lbl_ma.Visible = Lbl_wa.Visible = BtnZeroA.Visible = true;
                LblB.Visible = Lbl_mb.Visible = Lbl_wb.Visible = BtnZeroB.Visible = true;
                LblC.Visible = Lbl_mc.Visible = Lbl_wc.Visible = BtnZeroC.Visible = true;
                BtnHome.Top = (int)(DpiScaling * 116);
                Height = (int)(DpiScaling * 170);
            }

            if (val > 4)
            {
                BtnHome.Top = (int)(DpiScaling * 116);
                Height = (int)(DpiScaling * 170);
                LblA.Top = LblX.Top; BtnZeroA.Top = BtnZeroX.Top;
                LblA.Left = LblB.Left; BtnZeroA.Left = BtnZeroB.Left;
                Lbl_wa.Top = Lbl_wx.Top; Lbl_wa.Left = Lbl_wb.Left;
                Lbl_ma.Top = Lbl_mx.Top; Lbl_ma.Left = Lbl_mb.Left;
                this.Width = (int)(DpiScaling * 400);
                this.Height = (int)(DpiScaling * 170);
            }
            else
            {
                LblA.Top = (int)(DpiScaling * 108); BtnZeroA.Top = (int)(DpiScaling * 106);
                LblA.Left = (int)(DpiScaling * 6); BtnZeroA.Left = (int)(DpiScaling * 112);
                Lbl_wa.Location = new System.Drawing.Point((int)(DpiScaling * 31), (int)(DpiScaling * 108));
                Lbl_ma.Location = new System.Drawing.Point((int)(DpiScaling * 64), (int)(DpiScaling * 128));
            }
            widthOriginal = this.Width;
            foldTimer.Start();
        }

        internal void TriggerCmd(string action)
        {
            if (action.Contains("XYZ")) { BtnZeroXYZ.PerformClick(); }
            else if (action.Contains("XY")) { BtnZeroXY.PerformClick(); }
            else if (action.Contains("X")) { BtnZeroX.PerformClick(); }
            else if (action.Contains("Y")) { BtnZeroY.PerformClick(); }
            else if (action.Contains("Z")) { BtnZeroZ.PerformClick(); }
            else if (action.Contains("A")) { BtnZeroA.PerformClick(); }
        }
        private void DisplayWCO()
        {
            Lbl_wx.Text = string.Format("{0:0.000}", WCO.X);
            Lbl_wy.Text = string.Format("{0:0.000}", WCO.Y);
            Lbl_wz.Text = string.Format("{0:0.000}", WCO.Z);
            Lbl_wa.Text = string.Format("{0:0.000}", WCO.A);
            Lbl_wb.Text = string.Format("{0:0.000}", WCO.B);
            Lbl_wc.Text = string.Format("{0:0.000}", WCO.C);
        }
        private void DisplayMCO()
        {
            Lbl_mx.Text = string.Format("{0:0.000}", MCO.X);
            Lbl_my.Text = string.Format("{0:0.000}", MCO.Y);
            Lbl_mz.Text = string.Format("{0:0.000}", MCO.Z);
            Lbl_ma.Text = string.Format("{0:0.000}", MCO.A);
            Lbl_mb.Text = string.Format("{0:0.000}", MCO.B);
            Lbl_mc.Text = string.Format("{0:0.000}", MCO.C);
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            //     OnRaiseCmdEvent(new UserControlCmdEventArgs("$H", 0, sender, e));     // 
            if (Grbl.isMarlin)
                OnRaiseCmdEvent(new UserControlCmdEventArgs("G28", 0, sender, e));     // 
            else
                OnRaiseCmdEvent(new UserControlCmdEventArgs("$H", 0, sender, e));     // 

        }

        private void BtnZeroX_MouseUp(object sender, MouseEventArgs e)
        { if (!CheckShowSetCoord(e)) OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1}", ZeroCommand, ZeroString), 0, sender, e)); }

        private void BtnZeroY_MouseUp(object sender, MouseEventArgs e)
        { if (!CheckShowSetCoord(e)) OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}Y{1}", ZeroCommand, ZeroString), 0, sender, e)); }

        private void BtnZeroZ_MouseUp(object sender, MouseEventArgs e)
        { if (!CheckShowSetCoord(e)) OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}Z{1}", ZeroCommand, ZeroString), 0, sender, e)); }

        private void BtnZeroA_MouseUp(object sender, MouseEventArgs e)
        { if (!CheckShowSetCoord(e)) OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}A{1}", ZeroCommand, ZeroString), 0, sender, e)); }

        private void BtnZeroB_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}B{1}", ZeroCommand, ZeroString), 0, sender, e)); }

        private void BtnZeroC_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}C{1}", ZeroCommand, ZeroString), 0, sender, e)); }

        private void BtnZeroXY_MouseUp(object sender, MouseEventArgs e)
        { if (!CheckShowSetCoord(e)) OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1}Y{2}", ZeroCommand, ZeroString, ZeroString), 0, sender, e)); }

        private void BtnZeroXYZ_MouseUp(object sender, MouseEventArgs e)
        { if (!CheckShowSetCoord(e)) OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1}Y{2}Z{3}", ZeroCommand, ZeroString, ZeroString, ZeroString), 0, sender, e)); }

        private void BtnSetCoordX_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}X{1:0.000}", ZeroCommand, NudSetCoordX.Value), 0, sender, e)); }    // zeroCmd = "G10 L20 P0";

        private void BtnSetCoordY_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}Y{1:0.000}", ZeroCommand, NudSetCoordY.Value), 0, sender, e)); }    // zeroCmd = "G10 L20 P0";

        private void BtnSetCoordZ_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}Z{1:0.000}", ZeroCommand, NudSetCoordZ.Value), 0, sender, e)); }    // zeroCmd = "G10 L20 P0";

        private void BtnSetCoordA_Click(object sender, EventArgs e)
        { OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("{0}A{1:0.000}", ZeroCommand, NudSetCoordA.Value), 0, sender, e)); }    // zeroCmd = "G10 L20 P0";

        private bool CheckShowSetCoord(MouseEventArgs e)
        {
            Logger.Trace("CheckShowSetCoord   {0}", e.Button);
            if (e.Button == MouseButtons.Right)
                ShowSetCoord();
            return (e.Button == MouseButtons.Right);
        }
        private bool gBoxDROShowSetCoord = false;
        private void GbDRO_Click(object sender, EventArgs e)
        { ShowSetCoord(); }
        private void ShowSetCoord()
        {
            if (!gBoxDROShowSetCoord)
            { gBoxDROSetCoord.Visible = true; }
            else
            { gBoxDROSetCoord.Visible = false; }
            gBoxDROShowSetCoord = !gBoxDROShowSetCoord;
        }

        private void SetWidth(bool small)
        {
            if (!small)
            {
                this.Width = widthOriginal;
                BtnHome.Width = (int)(DpiScaling * 223);
                GbDRO.Text = "Tool Coordinates(Work / Machine)";
                lblCurrentG.Left = this.Width - (int)(DpiScaling * 65);
            }
            else
            {
                this.Width = (int)(DpiScaling * 110);
                BtnHome.Width = (int)(DpiScaling * 106);
                GbDRO.Text = "Tool Coordinates";
                gBoxDROSetCoord.Visible = gBoxDROShowSetCoord = false;
                lblCurrentG.Left = this.Width - (int)(DpiScaling * 25);
            }
        }

        protected virtual void FoldTimer_Tick(object sender, EventArgs e)
        { UCDRO_MouseLeave(sender, e); }

        private void UCDRO_MouseEnter(object sender, EventArgs e)
        {
            SetWidth(false);
            foldTimer.Start();
        }

        private void UCDRO_MouseLeave(object sender, EventArgs e)
        {
            var screenPosition = Cursor.Position;
            var clientPosition = GbDRO.PointToClient(screenPosition);
            if (clientPosition.Y < 0 || clientPosition.Y > GbDRO.Height ||
                clientPosition.X < 0 || clientPosition.X > GbDRO.Width)
            {
                if ((MainWidth < 1300) && MyControl.UseToolList())
                {
                    SetWidth(true);
                }
                foldTimer.Stop();
            }
        }
        public void RestoreColors()
        {
            lblCurrentG.ForeColor = Color.Black;
            gBoxDROSetCoord.BackColor = MyControl.PanelHighlight;
            gBoxDROSetCoord.ForeColor = Colors.ContrastColor(MyControl.PanelHighlight);
        }

    }
}
