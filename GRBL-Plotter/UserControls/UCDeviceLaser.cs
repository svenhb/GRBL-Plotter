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
using System.Diagnostics;
using System.Windows.Forms;
using static GrblPlotter.DeviceToolProperties;

namespace GrblPlotter.UserControls
{
    public partial class UCDeviceLaser : UserControl
    {
        private readonly float DpiScaling;
        private int _SpindleMin = 0;
        private int _SpindleMax = 1000;
        private string _SpindleSet = "0";
        private bool _LaserMode = false;
        private readonly MyControl m = new MyControl();
        private readonly ToolProperty toolProp = new ToolProperty();
        private OptionPropHatchFill fill = new OptionPropHatchFill();

        public int SpindleMin
        {
            get { return _SpindleMin; }
            set
            {
                _SpindleMin = value;
                LblLaserMinVal.Text = _SpindleMin.ToString();
            }
        }
        public int SpindleMax
        {
            get { return _SpindleMax; }
            set
            {
                _SpindleMax = value;
                LblLaserMaxVal.Text = _SpindleMax.ToString();
            }
        }
        public string SpindleSet
        {
            get { return _SpindleSet; }
            set
            {
                _SpindleSet = value;
                LblLaserSetVal.Text = _SpindleSet;
            }
        }

        public bool LaserMode
        {
            get { return _LaserMode; }
            set
            {
                _LaserMode = value;
            }
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent;
        protected virtual void OnRaiseGuiControlEvent(UserControlGuiControlEventArgs e)
        { RaiseGuiControlEvent?.Invoke(this, e); }

        public UCDeviceLaser()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
        }

        private void UCDeviceLaser_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            UpdateOptions();
            UpdateTools();
            SetBtnFillColor();
        }
        private void UpdateTools()
        {   /* defined in ToolList.cs DeviceToolProperties */
            toolProp.Laser.Diameter = (float)Properties.Settings.Default.DeviceLaserToolDiameter;
            toolProp.Laser.FeedXY = (float)NudDeviceLaserSpeed.Value;
            toolProp.Laser.FeedZ = (float)Properties.Settings.Default.DeviceLaserZFeed;
            toolProp.Laser.FinalZ = (float)Properties.Settings.Default.DeviceLaserZDown;
            toolProp.Laser.SaveZ = (float)Properties.Settings.Default.DeviceLaserZSave;
            toolProp.Laser.FinalS = (float)NudDeviceLaserPower.Value;
            toolProp.Laser.SaveS = (float)NudDeviceLaserPowerMin.Value;
            toolProp.Laser.UseM3 = CbLaserpower.Checked;
            toolProp.Laser.UseAir = CbAirAssist.Checked;
            toolProp.Laser.UseSorZ = Properties.Settings.Default.DeviceLaserZEnable;
            toolProp.Laser.Passes = (int)NudDeviceLaserPasses.Value;
            toolProp.Laser.Fill = fill.Copy();

            MyControl.SetToolsProperties(0, toolProp);
        }

        private void UpdateOptions()
        {
            fill.Enable = Properties.Settings.Default.DeviceLaserHatchFillEnable;
            fill.Cross = Properties.Settings.Default.DeviceLaserHatchFillCross;
            fill.Distance = (float)Properties.Settings.Default.DeviceLaserHatchFillDistance;
            // DistanceOffsetEnable
            // DistanceOffset
            fill.Angle = (float)Properties.Settings.Default.DeviceLaserHatchFillAngle;
            fill.AngleIncrementEnable = Properties.Settings.Default.DeviceLaserHatchFillAngleIncrementEnable;
            fill.AngleIncrement = (float)Properties.Settings.Default.DeviceLaserHatchFillAngleIncrement;
            // InsetEnable
            fill.InsetDistance = (float)Properties.Settings.Default.DeviceLaserHatchFillInsetDistance;
            // InsetShrink
            fill.InsetEnable = (fill.InsetDistance > 0);
            fill.DeletePath = Properties.Settings.Default.DeviceLaserHatchFillDeletePath;
        }

        private void UpdateSettings()
        {
            Properties.Settings.Default.DeviceLaserHatchFillEnable = fill.Enable;
            Properties.Settings.Default.DeviceLaserHatchFillCross = fill.Cross;
            Properties.Settings.Default.DeviceLaserHatchFillDistance = (decimal)fill.Distance;
            // DistanceOffsetEnable
            // DistanceOffset
            Properties.Settings.Default.DeviceLaserHatchFillAngle = (decimal)fill.Angle;
            Properties.Settings.Default.DeviceLaserHatchFillAngleIncrementEnable = fill.AngleIncrementEnable;
            Properties.Settings.Default.DeviceLaserHatchFillAngleIncrement = (decimal)fill.AngleIncrement;
            // InsetEnable
            Properties.Settings.Default.DeviceLaserHatchFillInsetDistance = (decimal)fill.InsetDistance;
            // InsetShrink
            Properties.Settings.Default.DeviceLaserHatchFillDeletePath = fill.DeletePath;

            fill.InsetEnable = (fill.InsetDistance > 0);
            Properties.Settings.Default.Save();
        }

        private void UCDeviceLaser_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = (Width + 3) - (int)(DpiScaling * MyControl.BtnSetupRight);
            BtnHelp.Left = BtnSetup.Left - (int)(21 * DpiScaling);
            NudDeviceLaserPasses.Width = (int)(DpiScaling * 53);
            NudDeviceLaserPower.Width = (int)(DpiScaling * 53);
            NudDeviceLaserSpeed.Width = (int)(DpiScaling * 53);
        }

        private void CbLaserpower_CheckedChanged(object sender, EventArgs e)
        {
            CbLaserpower.Text = CbLaserpower.Checked ? "M3" : "M4";
            MyControl.SettingWasChanged(true);
            UpdateTools();
        }

        private void CbAirAssist_CheckedChanged(object sender, EventArgs e)
        {
            MyControl.SettingWasChanged(true);
            UpdateTools();
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
                new ControlDefaults(LblSetup1.Text, "DeviceLaserToolDiameter", new decimal[] { 0.01m, 10m, 0.1m, 2m }),
                new ControlDefaults(LblSetup2.Text, "DeviceLaserCmndAirOn"),
                new ControlDefaults(LblSetup3.Text, "DeviceLaserCmndAirOff")
            };
            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
        }

        private void BtnFill_Click(object sender, EventArgs e)
        {
            //    Logger.Trace("before {0}", fill.Distance);
            List<ControlDefaults> cd = new List<ControlDefaults>();
            OptionPropHatchFill.ControlDefaultsSetList(cd);
            MyControl.TestSimpleSetup(Localization.GetString("optionFillHeadlinePlotter"), Cursor.Position, ref fill, cd);
            //    Logger.Trace("after {0}", fill.Distance);
            UpdateSettings();		//             Properties.Settings.Default.Save();
            UpdateTools();			//             MyControl.SetToolsProperties(0, toolProp);
            SetBtnFillColor();
            MyControl.SettingWasChanged(true);
        }

        private void BtnOffsetSort_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults("Enable offset to origin", "importGraphicOffsetOrigin"),
                new ControlDefaults("Offset X", "importGraphicOffsetOriginX", new decimal[] { 0.1m, 100m, 1m, 1m }),
                new ControlDefaults("Offset Y", "importGraphicOffsetOriginY", new decimal[] { 0.1m, 100m, 1m, 1m }),
                new ControlDefaults("Sort by distance", "importGraphicSortDistance")
            };
            MyControl.ShowSimpleSetup("Offset and sort", "Set the origin (lower left edge) of the imported graphic to the given values and moves the largest figure to the end of the list.", Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            SetBtnFillColor();
        }

        private void BtnOverlap_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblOverlapEnable.Text, "importGraphicExtendPathEnable"),
                new ControlDefaults(LblOverlapValue.Text, "importGraphicExtendPathValue", new decimal[] { 0.01m, 10m, 0.1m, 2m })
            };
            MyControl.ShowSimpleSetup(LblOverlapHeadline.Text, LblOverlapInfo.Text, Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            SetBtnFillColor();
        }

        private void BtnUseZ_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LbLZEnable.Text, "DeviceLaserZEnable"),
                new ControlDefaults(LbLZFeed.Text, "DeviceLaserZFeed", new decimal[] { 10m, 10000m, 10m, 0m }),
                new ControlDefaults(LblZSave.Text, "DeviceLaserZSave", new decimal[] { 0.0m, 100m, 1m, 1m }),
                new ControlDefaults(LblZFinal.Text, "DeviceLaserZDown", new decimal[] { -100m, 100m, 1m, 1m })
            };
            MyControl.ShowSimpleSetup(LblZHeadline.Text, LblZInfo.Text, Cursor.Position, cd);
            UpdateTools();
            SetBtnFillColor();
            MyControl.SettingWasChanged(true);
        }

        private void BtnStartLaserTools_Click(object sender, EventArgs e)
        {
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.openForm, 31));
        }
        #endregion

        private void SetBtnFillColor()
        {
            BtnFill.BackColor = fill.Enable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnFill.ForeColor = Colors.ContrastColor(BtnFill.BackColor);
            bool offs = Properties.Settings.Default.importGraphicOffsetOrigin || Properties.Settings.Default.importGraphicSortDistance;
            BtnOffsetSort.BackColor = offs ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnOffsetSort.ForeColor = Colors.ContrastColor(BtnOffsetSort.BackColor);
            BtnOverlap.BackColor = Properties.Settings.Default.importGraphicExtendPathEnable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnOverlap.ForeColor = Colors.ContrastColor(BtnOverlap.BackColor);
            BtnUseZ.BackColor = Properties.Settings.Default.DeviceLaserZEnable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnUseZ.ForeColor = Colors.ContrastColor(BtnUseZ.BackColor);
        }

        private void LblPower_MouseEnter(object sender, EventArgs e)
        {
            GbPowerSettings.Visible = true;
        }

        private void LblPower_MouseLeave(object sender, EventArgs e)
        {
            GbPowerSettings.Visible = false;
        }

        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            LblPower.BackColor = BtnSetup.BackColor = MyControl.NotifyYellow;
            LblPower.ForeColor = BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            MyControl.ChangeColor(GbPowerSettings, MyControl.PanelHighlight, Colors.ContrastColor(MyControl.PanelHighlight));
            SetBtnFillColor();
        }
    }
}
