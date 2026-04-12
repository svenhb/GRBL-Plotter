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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GrblPlotter.DeviceToolProperties;

namespace GrblPlotter.UserControls
{
    public partial class UCDeviceRouter : UserControl
    {
        private readonly float DpiScaling;
        private int _SpindleMin = 0;
        private int _SpindleMax = 1000;
        private string _SpindleSet = "0";
        private readonly ToolProperty toolProp = new ToolProperty();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent;
        internal virtual void OnRaiseCmdEvent(UserControlCmdEventArgs e)
        { RaiseCmdEvent?.Invoke(this, e); }

        public event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent;
        protected virtual void OnRaiseGuiControlEvent(UserControlGuiControlEventArgs e)
        { RaiseGuiControlEvent?.Invoke(this, e); }
        
        public UCDeviceRouter()
        {
            InitializeComponent();
            DpiScaling = (float)DeviceDpi / 96;
    //        UpdateOptions();
            UpdateTools();
        }
        private void UCDeviceRouter_Load(object sender, EventArgs e)
        {
            MyControl.SetSetupBtnAppearance(BtnSetup);
            SetBtnFillColor();
            UpdateTools();
        }

        public int SpindleMin
        {
            get { return _SpindleMin; }
            set
            {
                _SpindleMin = value;
                LblSpindleMinVal.Text = _SpindleMin.ToString();
            }
        }
        public int SpindleMax
        {
            get { return _SpindleMax; }
            set
            {
                _SpindleMax = value;
                LblSpindleMaxVal.Text = _SpindleMax.ToString();
            }
        }
        public string SpindleSet
        {
            get { return _SpindleSet; }
            set
            {
                _SpindleSet = value;
                LblSpindleSetVal.Text = _SpindleSet;
            }
        }

        private void UpdateTools()
        {
            toolProp.Router.Diameter= (float)Properties.Settings.Default.DeviceRouterToolDiameter;
            toolProp.Router.FeedXY = (float)NudDeviceRouterFeedXY.Value;
            toolProp.Router.FeedZ = (float)NudDeviceRouterFeedZ.Value;
            toolProp.Router.FinalZ = (float)NudDeviceRouterZDown.Value;
            toolProp.Router.SaveZ = (float)NudDeviceRouterZUp.Value;
            toolProp.Router.UseM3 = true;
            toolProp.Router.UseAir = false;
            toolProp.Router.Passes = 1;
            toolProp.Router.UseSorZ = false;
            toolProp.Laser.UseSorZ = false;

            MyControl.SetToolsProperties(2, toolProp);
        }
        private void NudDeviceRouter_ValueChanged(object sender, EventArgs e)
        {
            UpdateTools();
            MyControl.SettingWasChanged(true);
        }
        private void SetBtnFillColor()
        {
            BtnTangential.BackColor = Properties.Settings.Default.importGCTangentialEnable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnTangential.ForeColor = Colors.ContrastColor(BtnTangential.BackColor);
            BtnDragTool.BackColor = Properties.Settings.Default.importGCDragKnifeEnable ? MyControl.ButtonActive : MyControl.ButtonInactive;
            BtnDragTool.ForeColor = Colors.ContrastColor(BtnDragTool.BackColor);
        }
		
        private void UCDeviceRouter_Resize(object sender, EventArgs e)
        {
            BtnSetup.Left = (Width + 3) - (int)(DpiScaling * MyControl.BtnSetupRight);
            BtnHelp.Left = BtnSetup.Left - (int)(21 * DpiScaling);
        }

        private void LblSpindleSpeed_MouseEnter(object sender, EventArgs e)
        {
            GbSpindleSpeed.Visible = true;
        }

        private void LblSpindleSpeed_MouseLeave(object sender, EventArgs e)
        {
            GbSpindleSpeed.Visible = false;
        }

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

        #region buttons
        private void BtnSetup_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>();
            cd.Add(new ControlDefaults(LblSetup1.Text, "DeviceRouterToolDiameter", new decimal[] { 0.01m, 10m, 0.1m, 2m }));
        //    cd.Add(new ControlDefaults(LblSetup2.Text, "flowControlText"));
            MyControl.ShowSimpleSetup(LblSetupHeadline.Text, "", Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            UpdateTools();
        }

        public void RestoreColors()
        {
            BtnSetup.BackColor = MyControl.NotifyYellow;
            BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            LblSpindleSpeed.BackColor = BtnSetup.BackColor = MyControl.NotifyYellow;
            LblSpindleSpeed.ForeColor = BtnSetup.ForeColor = Colors.ContrastColor(MyControl.NotifyYellow);
            MyControl.ChangeColor(GbSpindleSpeed, MyControl.PanelHighlight, Colors.ContrastColor(MyControl.PanelHighlight));
        }
				
        private void BtnTangential_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblTangentialEnable.Text, "importGCTangentialEnable"),
                new ControlDefaults(LblTangentialAxisName.Text, "importGCTangentialAxis"),
                new ControlDefaults(LblTangentialSwivelAngle.Text, "importGCTangentialAngle", new decimal[] { 0m, 90m, 1m, 0m }),
                new ControlDefaults(LblTangentialDeviAngle.Text, "importGCTangentialAngleDevi", new decimal[] { 0m, 30m, 1m, 0m }),
                new ControlDefaults(LblTangentialUnitsPerTurn.Text, "importGCTangentialTurn", new decimal[] { 0.1m, 360m, 1m, 1m }),
                new ControlDefaults(LblTangentialLimitAngle.Text, "importGCTangentialRange"),
                new ControlDefaults(LblTangentialPathShorteningEnable.Text, "importGCTangentialShorteningEnable"),
                new ControlDefaults(LblTangentialPathShortening.Text, "importGCTangentialShortening", new decimal[] { 0m, 100m, 1m, 1m })
			};
            MyControl.ShowSimpleSetup(LblTangentialHeadline.Text, LblTangentialInfo.Text, Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            SetBtnFillColor();
        }
        private void BtnDragTool_Click(object sender, EventArgs e)
        {
            List<ControlDefaults> cd = new List<ControlDefaults>
            {
                new ControlDefaults(LblDragToolEnable.Text, "importGCDragKnifeEnable"),
                new ControlDefaults(LblDragToolLength.Text, "importGCDragKnifeLength", new decimal[] { 0.01m, 100m, 1m, 2m }),
                new ControlDefaults(LblDragToolPercentEnable.Text, "importGCDragKnifePercentEnable"),
                new ControlDefaults(LblDragToolPercent.Text, "importGCDragKnifePercent", new decimal[] { 1m, 100m, 1m, 0m }),
                new ControlDefaults(LblDragToolAngle.Text, "importGCDragKnifeAngle", new decimal[] { 0m, 180m, 1m, 0m }),
                new ControlDefaults(LblDragToolTangentialEnable.Text, "importGCDragKnifeUse")
			};
            MyControl.ShowSimpleSetup(LblDragToolHeadline.Text, LblDragToolInfo.Text, Cursor.Position, cd);
            MyControl.SettingWasChanged(true);
            SetBtnFillColor();
        }

        private void BtnStartProbing_Click(object sender, EventArgs e)
		{
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.openForm, 21));			
		}

        private void BtnStartHeightmap_Click(object sender, EventArgs e)
		{
            OnRaiseGuiControlEvent(new UserControlGuiControlEventArgs(GuiControl.openForm, 22));			
		}
		#endregion
    }
}
