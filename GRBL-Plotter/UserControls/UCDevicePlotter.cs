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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static GrblPlotter.DeviceToolProperties;

namespace GrblPlotter.UserControls
{
    public partial class UCDevicePlotter : UserControl
    {
        private readonly float DpiScaling;
        private readonly ToolProperty toolProp = new ToolProperty();
        private OptionPropHatchFill fill = new OptionPropHatchFill();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }
        public UCDevicePlotter()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
        }
        private void UCDevicePlotter_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            int i = Properties.Settings.Default.DevicePlotterControlIndex;
            if (i < TcServoZAxis.TabCount)
                TcServoZAxis.SelectedIndex = i;
            MyControl.SelectedPlotterMode = TcServoZAxis.SelectedIndex;
            CbDepthControl_CheckedChanged(sender, e);
            UpdateOptions();
            UpdateTools();
            SetBtnFillColor();
        }

        private void BtnGCPWMUp_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMUp).Replace(",", "."), 0, sender, e));
        }

        private void BtnGCPWMDown_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMDown).Replace(",", "."), 0, sender, e));
        }
        private void TbImportGCPWMTextP93_TextChanged(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMP93).Replace(",", "."), 0, sender, e));
        }

        private void TbImportGCPWMTextP94_TextChanged(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMP94).Replace(",", "."), 0, sender, e));
        }

        private void NudImportGCPWMUp_ValueChanged(object sender, EventArgs e)
        {
            //    if (cBImportGCPWMSendCode.Checked)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", nUDImportGCPWMUp.Value).Replace(",", "."), 0, sender, e));
            }
        }

        private void NudImportGCPWMDown_ValueChanged(object sender, EventArgs e)
        {
            //    if (cBImportGCPWMSendCode.Checked)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", nUDImportGCPWMDown.Value).Replace(",", "."), 0, sender, e));
            }
            MyControl.SettingWasChanged(true);
            UpdateTools();
        }

        private void BtnGCPWMZero_Click(object sender, EventArgs e)
        {
            OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", Properties.Settings.Default.importGCPWMZero).Replace(",", "."), 0, sender, e));
            UpdateTools();
        }

        private void NudImportGCPWMZero_ValueChanged(object sender, EventArgs e)
        {
            //    if (cBImportGCPWMSendCode.Checked)
            {
                OnRaiseCmdEvent(new UserControlCmdEventArgs(string.Format("M3 S{0}", nUDImportGCPWMZero.Value).Replace(",", "."), 0, sender, e));
            }
            MyControl.SettingWasChanged(true);
            UpdateTools();
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DevicePlotterControlIndex = TcServoZAxis.SelectedIndex;
            MyControl.SelectedPlotterMode = TcServoZAxis.SelectedIndex;
        }

        private void UpdateTools()
        {
            toolProp.Plotter.Diameter = (float)Properties.Settings.Default.DevicePlotterToolDiameter;
            toolProp.Plotter.FeedXY = (float)NudDevicePlotterFeedXY.Value;
            toolProp.Plotter.FeedZ = (float)NudDevicePlotterFeedZ.Value;
            toolProp.Plotter.FinalZ = (float)NudDevicePlotterZDown.Value;
            toolProp.Plotter.SaveZ = (float)NudDevicePlotterZUp.Value;
            toolProp.Plotter.FinalS = (float)nUDImportGCPWMDown.Value;
            toolProp.Plotter.SaveS = (float)nUDImportGCPWMUp.Value;
            toolProp.Plotter.UseM3 = true;
            toolProp.Plotter.UseAir = false;
            toolProp.Plotter.UseSorZ = false;   // TcServoZAxis.SelectedIndex == 0;
            toolProp.Plotter.Passes = 1;
            toolProp.Plotter.Fill = fill.Copy();

            MyControl.SetToolsProperties(1, toolProp);
        }

        private void UpdateOptions()
        {
            fill.Enable = Properties.Settings.Default.DevicePlotterHatchFillEnable;
            fill.Cross = Properties.Settings.Default.DevicePlotterHatchFillCross;
            fill.Distance = (float)Properties.Settings.Default.DevicePlotterHatchFillDistance;
            // DistanceOffsetEnable
            // DistanceOffset
            fill.Angle = (float)Properties.Settings.Default.DevicePlotterHatchFillAngle;
            fill.AngleIncrementEnable = Properties.Settings.Default.DevicePlotterHatchFillAngleIncrementEnable;
            fill.AngleIncrement = (float)Properties.Settings.Default.DevicePlotterHatchFillAngleIncrement;
            // InsetEnable
            fill.InsetDistance = (float)Properties.Settings.Default.DevicePlotterHatchFillInsetDistance;
            // InsetShrink
            fill.InsetEnable = (fill.InsetDistance > 0);
            fill.DeletePath = Properties.Settings.Default.DevicePlotterHatchFillDeletePath;
        }
        private void UpdateSettings()
        {
            Properties.Settings.Default.DevicePlotterHatchFillEnable = fill.Enable;
            Properties.Settings.Default.DevicePlotterHatchFillCross = fill.Cross;
            Properties.Settings.Default.DevicePlotterHatchFillDistance = (decimal)fill.Distance;
            // DistanceOffsetEnable
            // DistanceOffset
            Properties.Settings.Default.DevicePlotterHatchFillAngle = (decimal)fill.Angle;
            Properties.Settings.Default.DevicePlotterHatchFillAngleIncrementEnable = fill.AngleIncrementEnable;
            Properties.Settings.Default.DevicePlotterHatchFillAngleIncrement = (decimal)fill.AngleIncrement;
            // InsetEnable
            Properties.Settings.Default.DevicePlotterHatchFillInsetDistance = (decimal)fill.InsetDistance;
            // InsetShrink
            Properties.Settings.Default.DevicePlotterHatchFillDeletePath = fill.DeletePath;

            fill.InsetEnable = (fill.InsetDistance > 0);
            Properties.Settings.Default.Save();
        }

        #region buttons
        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string url = "https://grbl-plotter.de/index.php?";
            try
            {
                Button clickedLink = sender as Button;
                Process.Start(url + clickedLink.Tag.ToString());
            }
            catch (Exception err)
            {
                Logger.Error(err, "BtnHelp_Click ");
                MessageBox.Show("Could not open the link: " + err.Message, "Error");
            }
        }

        private void BtnSetup_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblSetupToolDiameter.Text, "DevicePlotterToolDiameter", new decimal[] { 0.01m, 10m, 0.1m, 2m })
            };
            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd);
            UpdateTools();
            MyControl.SettingWasChanged(true);
        }

        private void BtnFill_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>();
            OptionPropHatchFill.ControlDefaultsSetList(cd);
            MyControl.TestSimpleSetup(Localization.GetString("optionFillHeadlinePlotter"), Cursor.Position, ref fill, cd);

            UpdateSettings();
            UpdateTools();
            SetBtnFillColor();
            MyControl.SettingWasChanged(true);
        }
        private void BtnNoise_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblNoiseEnable.Text, "importGraphicNoiseEnable"),
                new ControlDefaults(LblNoiseAmplitude.Text, "importGraphicNoiseAmplitude", new decimal[] { 0.1m, 100m, 0.5m, 1m }),
                new ControlDefaults(LblNoiseDistance.Text, "importGraphicNoiseDensity", new decimal[] { 0.1m, 100m, 0.5m, 1m })
            };
            MyControl.ShowSimpleSetup(LblNoiseHeadline.Text, LblNoiseInfo.Text, Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            SetBtnFillColor();
        }
        private void BtnSplit_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblSplitEnable.Text, "importGCLineSegmentation"),
                new ControlDefaults(LblSplitSegmentLength.Text, "importGCLineSegmentLength", new decimal[] { 1m, 10000m, 10m, 1m }),
                new ControlDefaults(LblSplitSegmentEquidistant.Text, "importGCLineSegmentEquidistant"),
                new ControlDefaults(LblSplitSubEnable.Text, "importGCSubEnable"),
                new ControlDefaults(LblSplitSubroutine.Text, "importGCSubroutine"),
                new ControlDefaults(LblSplitSubFirst.Text, "importGCSubFirst"),
                new ControlDefaults(LblSplitSubPenUpDown.Text, "importGCSubPenUpDown")
            };
            MyControl.ShowSimpleSetup(LblSplitHeadline.Text, LblSplitInfo.Text, Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            SetBtnFillColor();
        }

        private void BtnPenChange_Click(object sender, EventArgs e)
        {
            Logger.Trace("BtnPenChange_Click ctrlToolScriptPut:{0}", Properties.Settings.Default.ctrlToolScriptPut);
            var p = Properties.Settings.Default;
            if (string.IsNullOrEmpty(p.ctrlToolScriptPut)) { p.ctrlToolScriptPut = Datapath.Scripts + "\\"; }
            if (string.IsNullOrEmpty(p.ctrlToolScriptSelect)) { p.ctrlToolScriptSelect = Datapath.Scripts + "\\"; }
            if (string.IsNullOrEmpty(p.ctrlToolScriptGet)) { p.ctrlToolScriptGet = Datapath.Scripts + "\\"; }
            if (string.IsNullOrEmpty(p.ctrlToolScriptProbe)) { p.ctrlToolScriptProbe = Datapath.Scripts + "\\"; }

            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblSetupPenChangeRBNo.Text, "DevicePlotterPenChangeRBNo"),
                new ControlDefaults(LblSetupPenChangeRBManual.Text, "DevicePlotterPenChangeRBManual"),
                new ControlDefaults(LblSetupPenChangeRBAutomatic.Text, "DevicePlotterPenChangeRBAutomatic", new decimal[] { 0.1m, 10m, 0.1m, 2m }),
                new ControlDefaults("Pen is in holder", "DevicePlotterPenInHolder"),

                /* Tool exchange scripts? */
                new ControlDefaults("Script put", "ctrlToolScriptPut"),
                new ControlDefaults("Script select", "ctrlToolScriptSelect"),
                new ControlDefaults("Script get", "ctrlToolScriptGet"),
                new ControlDefaults("Script probe", "ctrlToolScriptProbe"),

                new ControlDefaults(LblSetupFlowControlEnable.Text, "flowControlEnable"),
                new ControlDefaults(LblSetupFlowControlText.Text, "flowControlText")
            };
            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd);
            Properties.Settings.Default.Save();

            Properties.Settings.Default.gui2DToolTableShow = Properties.Settings.Default.DevicePlotterPenChangeRBAutomatic;
            UpdateTools();
            SetBtnFillColor();
            MyControl.SettingWasChanged(true);
        }


        #endregion

        private void SetBtnFillColor()
        {
            BtnFill.BackColor = fill.Enable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnFill.ForeColor = Colors.ContrastColor(BtnFill.BackColor);
            BtnNoise.BackColor = Properties.Settings.Default.importGraphicNoiseEnable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnNoise.ForeColor = Colors.ContrastColor(BtnNoise.BackColor);
            BtnSplit.BackColor = Properties.Settings.Default.importGCLineSegmentation ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnSplit.ForeColor = Colors.ContrastColor(BtnSplit.BackColor);
            BtnPenChange.BackColor = !Properties.Settings.Default.DevicePlotterPenChangeRBNo ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnPenChange.ForeColor = Colors.ContrastColor(BtnPenChange.BackColor);
        }

        private void UCDevicePlotter_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = (Width + 3) - (int)(DpiScaling * MyControl.BtnSetupRight);
            BtnHelp.Left = BtnSetup.Left - (int)(21 * DpiScaling);
        }

        private void NudDevicePlotter_ValueChanged(object sender, EventArgs e)
        {
            MyControl.SettingWasChanged(true);
            UpdateTools();
        }

        private void TcServoZAxis_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = TcServoZAxis.TabPages[e.Index];
            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);

            Color colText = (e.State == DrawItemState.Selected) ? Colors.ContrastColor(MyControl.NotifyYellow) : Colors.ContrastColor(page.BackColor);
            if (!e.Bounds.IsEmpty)
            {
                e.Graphics.FillRectangle(new SolidBrush((e.State == DrawItemState.Selected) ? MyControl.NotifyYellow : page.BackColor), e.Bounds);
            }
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, colText);//  page.ForeColor);
        }

        private void BtnAdvancedZ_Click(object sender, EventArgs e)
        {
            PanelAdvancedZ.Visible = !PanelAdvancedZ.Visible;
        }

        private void BtnAdvancedS_Click(object sender, EventArgs e)
        {
            PanelAdvancedS.Visible = !PanelAdvancedS.Visible;
        }

        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            SetBtnFillColor();
            MyControl.ChangeColor(PanelAdvancedS, MyControl.PanelHighlight, Colors.ContrastColor(MyControl.PanelHighlight));
            MyControl.ChangeColor(PanelAdvancedZ, MyControl.PanelHighlight, Colors.ContrastColor(MyControl.PanelHighlight));
        }

        private void CbDepthControl_CheckedChanged(object sender, EventArgs e)
        {
            if (CbDepthControl.Checked)
                CbDepthControl.BackColor = MyControl.NotifyYellow;
            else
                CbDepthControl.BackColor = SystemColors.Control;
        }
    }
}
