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
 * 2026-01-05 GUI rework for vers. 1.8.0.0
 
 If 'Laser', 'Plotter' or 'Router' is selected, a slightliy different workflow is applied:
 - Start Import - GraphicCollectData.cs - Graphic.Init calls MyControl.ChangeGraphicOptionsDeviceSpecific(graphicInformation)
	to reset all options for import, just keep device specific.
	Path import parameter? Remove short moves, Distance to be assumes as equal... etc ?????
	
 - When creating special paths in GraphicCollectData.cs - CreateGCode, tool specific settings will be taken into account:
	- GraphicGenerateHatchFill.cs - HatchFill. 
	- Add noise will be applied during path creation, not possible tool depending
	- Repeat paths
 
 - save tool list as XML
 
*/

using GrblPlotter.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static GrblPlotter.DeviceToolProperties;
using static GrblPlotter.Gcode;
using static GrblPlotter.Graphic;

namespace GrblPlotter.UserControls
{
    public enum CommandProtocol { Marlin, grblOld, grblCanJog }
    public enum GuiControl { streamStart, streamStartSection, streamStop, streamPause, streamCheck, simulateStart, simulateStop, simulatePause, simulateFaster, simulateSlower, guiUpdate, highligh, reloadGraphic, invalidate, openForm }
    public enum DeviceSelection { Laser, Plotter, Router, Individual }

    public struct DistFeed
    {
        public DistFeed(float d, int f)
        {
            Dist = d;
            Feed = f;
        }
        public float Dist { get; }
        public int Feed { get; }
    };
    public struct ControlDefaults
    {
        public string Text { get; }
        public string Prop { get; }
        public string Cmnd { get; }
        public decimal[] Set;
        public ControlDefaults(string t, string p, decimal[] s)
        {
            Text = t;
            Prop = p;
            Set = s;
            Cmnd = "";
        }
        public ControlDefaults(string t, string p, string c)
        {
            Text = t;
            Prop = p;
            Set = new decimal[0];
            Cmnd = c;
        }
        public ControlDefaults(string t, string p)
        {
            Text = t;
            Prop = p;
            Set = new decimal[0];
            Cmnd = "";
        }
    };
    public class MyControl
    {
        private static readonly bool log = true;
        public static Color ButtonBackColor { get; set; }
        public static Color ButtonForeColor { get; set; }
        public static Color PanelBackColor { get; set; }
        public static Color PanelForeColor { get; set; }
        public static Color PanelHighlight { get; set; }

        public static Color ButtonActive { get; set; }
        public static Color ButtonInactive { get; set; }

        public static Color NotifyYellow { get; set; }
        public static Color NotifyGreen { get; set; }
        public static Color NotifyRed { get; set; }


        public static int MinimumHeightFolded = 18;
        public static int BtnSetupRight = 20;

        public static float DpiScaling { get; set; }
        public static DeviceSelection SelectedDevice { get; set; }
        public static int SelectedPlotterMode { get; set; }
        public static bool GraphicImported { get; set; }
        public static GroupOption GroupBy { get; set; }
        public static bool ApplyToolList { get; set; }
        private static bool settingWasChanged = false;


        private static int DeviceLaserZPasses = 0;

        private static ToolProperty ToolsProperties = new ToolProperty();//{ get; set; }

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        internal static void InitToolProperties()
        {
            GroupBy = GroupOption.ByColor;
        }

        internal static void SetToolsProperties(int deviceNr, ToolProperty p)
        {
            if (deviceNr == 0)
            {
                ToolsProperties.Laser = p.Laser.Copy();
                if (log) Logger.Trace("●●●●● SetToolsProperties device:{0}  settings:{1}", deviceNr, p.Laser.Settings());
            }
            else if (deviceNr == 1)
            {
                ToolsProperties.Plotter = p.Plotter.Copy();
                if (log) Logger.Trace("●●●●● SetToolsProperties device:{0}  settings:{1}", deviceNr, p.Plotter.Settings());
            }
            else
            {
                ToolsProperties.Router = p.Router.Copy();
                if (log) Logger.Trace("●●●●● SetToolsProperties device:{0}  settings:{1}", deviceNr, p.Router.Settings());
            }
        }
        internal static void SettingWasChanged(bool changed = true)
        {
            settingWasChanged = changed;
            if (changed)
            {
                //     ucToolList.FillToolListElements();
            }
        }
        internal static bool GetSettingWasChanged()
        { return settingWasChanged; }

        internal static ToolProperty GetToolsProperties()
        {
            return ToolsProperties;
        }
        internal static void SetSelectedDevice(int tab)
        {
            if (tab == 0)
                SelectedDevice = DeviceSelection.Laser;
            else if (tab == 1)
                SelectedDevice = DeviceSelection.Plotter;
            else if (tab == 2)
                SelectedDevice = DeviceSelection.Router;
            else
                SelectedDevice = DeviceSelection.Individual;
        }
        internal static DeviceToolProperties GetActualDevice()
        {
            if (SelectedDevice == DeviceSelection.Laser)
                return ToolsProperties.Laser;
            else if (SelectedDevice == DeviceSelection.Plotter)
                return ToolsProperties.Plotter;
            else if (SelectedDevice == DeviceSelection.Router)
                return ToolsProperties.Router;

            Logger.Warn("GetActualDevice - no device");
            return new DeviceToolProperties(); ;
        }
        internal static OptionPropHatchFill GetActualFill()
        {
            if (SelectedDevice == DeviceSelection.Laser)
                return ToolsProperties.Laser.Fill;
            else if (SelectedDevice == DeviceSelection.Plotter)
                return ToolsProperties.Plotter.Fill;
            else if (SelectedDevice == DeviceSelection.Router)
                return ToolsProperties.Router.Fill;

            Logger.Warn("GetActualFill - no device");
            return new OptionPropHatchFill(); ;
        }
        internal static bool GetActualFillEnable()
        {
            if (SelectedDevice == DeviceSelection.Laser)
                return ToolsProperties.Laser.Fill.Enable;
            else if (SelectedDevice == DeviceSelection.Plotter)
                return ToolsProperties.Plotter.Fill.Enable;
            else if (SelectedDevice == DeviceSelection.Router)
                return ToolsProperties.Router.Fill.Enable;

            Logger.Warn("GetActualFillEnable - no device");
            return Properties.Settings.Default.importGraphicHatchFillEnable;
        }
        internal static double GetActualWidth()
        {
            if (SelectedDevice == DeviceSelection.Laser)
                return (double)Properties.Settings.Default.DeviceLaserToolDiameter;
            else if (SelectedDevice == DeviceSelection.Plotter)
                return (double)Properties.Settings.Default.DevicePlotterToolDiameter;
            else if (SelectedDevice == DeviceSelection.Router)
                return (double)Properties.Settings.Default.DeviceRouterToolDiameter;
            return 0.09;
        }
        internal static string GetSelectedDeviceName()
        {
            if (SelectedDevice == DeviceSelection.Laser)
                return "Laser";
            else if (SelectedDevice == DeviceSelection.Plotter)
                return "Plotter";
            else if (SelectedDevice == DeviceSelection.Router)
                return "Router";
            return "All";
        }
        internal static bool UseToolList()
        { return (SelectedDevice != DeviceSelection.Individual); }	//<= 2); }

        internal static void StartConvert(Graphic.SourceType type)
        {
            //  if (UseToolList() && GetActualFillEnable())
            { GraphicImported = true; }
        }

        /***************************************************************
		* Override import options
		****************************************************************/
        internal static void ChangeGraphicOptionsDeviceSpecific(Graphic.GraphicInformationClass info)
        {
            /* in GraphicCollectData.cs 301 Graphic.Init() */
            DeviceLaserZPasses = 0;

            if (UseToolList())
            { if (log) Logger.Trace("►►►►►► ChangeGraphicOptionsDeviceSpecific device: {0} UseToolList: {1}   Nothing to do", GetSelectedDeviceName(), UseToolList()); }
            else
            {
                if (log) Logger.Trace("►►►►►► ChangeGraphicOptionsDeviceSpecific device: {0} UseToolList: {1}   {2}", GetSelectedDeviceName(), UseToolList(), GetActualDevice().Settings());
                return;
            }

            info.OptionSpecialDevelopment = false;
            info.OptionSpecialWireBender = false;
            info.OptionSpecialConvertToPolar = false;
            info.OptionClipCode = false;
            info.OptionNodesOnly = false;
            info.OptionTangentialAxis = false;
            info.OptionDragTool = false;
            info.OptionExtendPath = false;
            info.OptionRampOnPenDown = false;
            info.OptionSFromWidth = false;
            info.OptionZFromWidth = false;
            info.ImportDxfConsiderZ = false;
            info.ApplyHatchFillSVG = true;

            info.ConvertArcToLine = UseToolList() || GetActualFillEnable();

            info.GroupOption = GroupBy; //GroupOption.ByColor;
            info.GroupEnable = true;
            info.FigureEnable = true;

            if (ApplyToolList)
                info.SortOption = SortOption.ByToolNr;
            else
                info.SortOption = SortOption.none;

            info.OptionCodeSortDistance = false;
            info.OptionCodeSortDistanceNewStartOnClosedPath = false;
            info.OptionCodeSortDistanceLargestLast = false;
            info.OptionCodeSortDimension = false;
            info.OptionCodeOffset = false;
            info.OptionCodeOffsetLargestLast = false;
            info.OptionCodeOffsetLargestRemove = false;

            info.OptionRepeatCode = false;
            info.OptionRepeatCodeComplete = false;
            info.OptionRepeatCodeZEnable = false;
            info.OptionNoise = false;

            bool useInkFromTablet = (info.SourceType == SourceType.Ink);
            bool useDepthFromWidth = Properties.Settings.Default.DevicePlotterDepthControl;

            if (useInkFromTablet || useDepthFromWidth)
            {
                info.PenWidthMin = 0;
                info.PenWidthMax = (double)Properties.Settings.Default.tabletSizePen;
            }
            if (SelectedDevice == DeviceSelection.Laser)
            {
                info.ImportRemoveShortMoves = true;
                info.ImportRemoveShortMovesLimit = 0.05;

                info.OptionCodeOffset = Properties.Settings.Default.importGraphicOffsetOrigin;    //true;
                info.OptionCodeOffsetLargestLast = true;
                info.OptionCodeOffsetLargestRemove = false;

                info.OptionCodeSortDistance = Properties.Settings.Default.importGraphicSortDistance;       //true;  
                info.OptionCodeSortDistanceStartIndex = 3;  // min-x, min-y
                info.OptionCodeSortDistanceNewStartOnClosedPath = true;
                info.OptionCodeSortDistanceLargestLast = true;  //
                                                                //    info.OptionCodeSortDimension = false;

                info.OptionRepeatCode = true;
                info.OptionExtendPath = Properties.Settings.Default.importGraphicExtendPathEnable;
                info.OptionCodeSortDimension = Properties.Settings.Default.importGraphicExtendPathEnable;

                if (Properties.Settings.Default.DeviceLaserZEnable)
                {
                    // add z-info to path data. Rep=1 - Z=0; Rep=2 - Z=0 & Z=max.
                    info.OptionRepeatCodeZEnable = true;
                    info.OptionRepeatCodeZValue = GetActualDevice().FinalZ;
                    info.ImportDxfConsiderZ = true;
                }
                if (useInkFromTablet || useDepthFromWidth)
                {
                    info.OptionSFromWidth = true;
                    info.OptionRepeatCode = false;      // <- important
                    info.OptionCodeOffsetLargestLast = false;
                    info.OptionCodeSortDistance = false;
                }
            }

            else if (SelectedDevice == DeviceSelection.Plotter)
            {
                info.ImportRemoveShortMoves = true;
                info.ImportRemoveShortMovesLimit = 0.1;
                info.OptionCodeOffset = true;
                info.OptionCodeOffsetLargestLast = info.OptionCodeOffsetLargestRemove = false;
                info.OptionCodeSortDistance = true;
                info.OptionCodeSortDistanceNewStartOnClosedPath = true;
                info.OptionCodeSortDistanceLargestLast = false;
                info.OptionCodeSortDimension = false;

                //    if (Properties.Settings.Default.DevicePlotterHatchFillEnable)
                //   { info.ApplyHatchFillSVG = true; }
                info.OptionNoise = Properties.Settings.Default.importGraphicNoiseEnable;

                if (useInkFromTablet || useDepthFromWidth)
                {
                    info.OptionCodeSortDistance = false;
                    info.OptionCodeSortDistanceNewStartOnClosedPath = false;
                    if (SelectedPlotterMode == 0)               // servo enable
                    {
                        info.OptionSFromWidth = true;
                    }
                    else
                    {
                        info.OptionZFromWidth = true;
                        info.ImportDxfConsiderZ = true;
                    }
                }

                info.OptionRepeatCodeComplete = false; // Properties.Settings.Default.importRepeatComplete
            }

            else if (SelectedDevice == DeviceSelection.Router)
            {
                info.OptionTangentialAxis = Properties.Settings.Default.importGCTangentialEnable;
                info.OptionDragTool = Properties.Settings.Default.importGCDragKnifeEnable;

                if (useInkFromTablet || useDepthFromWidth)
                {
                    info.OptionZFromWidth = true;
                    info.ImportDxfConsiderZ = true;
                }
            }
        }

        /***************************************************************
		* Override GCode setup
		****************************************************************/
        internal static void OverideGcodeSetup()
        {   /* in Graphoc2GcodeRelated.cs 846 GCode.Setup()*/
            if (UseToolList())
            { if (log) Logger.Trace("►►►►►► OverideGcodeSetup device: {0} UseToolList: {1}   Nothing to do", GetSelectedDeviceName(), UseToolList()); }
            else
            {
                if (log) Logger.Trace("►►►►►► OverideGcodeSetup device: {0} UseToolList: {1}   {2}", GetSelectedDeviceName(), UseToolList(), GetActualDevice().Settings());
                return;
            }

            Gcode.GcodeXYFeed = GetActualDevice().FeedXY;
            Gcode.Spindle.Speed = GetActualDevice().FinalS;
            Gcode.Spindle.SpindleCmd = GetActualDevice().UseM3 ? "3" : "4";
            Gcode.Spindle.LasermodeEnable = false;

            Gcode.OptionZAxis.Enable = false;	//(SelectedDevice == DeviceSelection.Router) || ((SelectedDevice == DeviceSelection.Plotter) && (SelectedPlotterMode == 1)); //GcodeDefaults.ZEnable;    // Properties.ListSettings.Default.importGCZEnable;            Gcode.Up = GetActualDevice().SaveZ;			//(float)Properties.ListSettings.Default.importGCZUp;
            Gcode.OptionZAxis.Up = GetActualDevice().SaveZ;		//(float)Properties.ListSettings.Default.importGCZDown;
            Gcode.OptionZAxis.Down = GetActualDevice().FinalZ;      //(float)Properties.ListSettings.Default.importGCZDown;
            Gcode.OptionZAxis.finalZ = GetActualDevice().FinalZ;
            Gcode.OptionZAxis.Feed = GetActualDevice().FeedZ;
            Gcode.OptionZAxis.IncrementEnable = false;

            Gcode.OptionPWM.Enable = false;	//((SelectedDevice == DeviceSelection.Plotter) && (SelectedPlotterMode == 0)); //GcodeDefaults.PWMEnable;   // Properties.ListSettings.Default.importGCPWMEnable;
            Gcode.OptionPWM.Up = GetActualDevice().SaveS;           // (float)Properties.ListSettings.Default.importGCPWMUp;
            Gcode.OptionPWM.DlyUp = (float)Properties.Settings.Default.importGCPWMDlyUp;
            Gcode.OptionPWM.Down = GetActualDevice().FinalS;         // (float)Properties.ListSettings.Default.importGCPWMDown;
            Gcode.OptionPWM.DlyDown = (float)Properties.Settings.Default.importGCPWMDlyDown;

            DepthFromWidth.SEnable = false;

            Gcode.ModificationTangential.Enable = false;

            Gcode.Control.GcodeRelative = false;
            Gcode.Control.ToolChangeAddCommand = false;
            Gcode.Control.ToolChangeM0Enable = false;

            Gcode.ModificationSegmentation.Enable = false;

            bool useInkFromTablet = (Graphic.graphicInformation.SourceType == SourceType.Ink);
            bool useDepthFromWidth = Properties.Settings.Default.DevicePlotterDepthControl;
            Gcode.Import.SelectedDevice = SelectedDevice;
            Gcode.Import.SelectedPlotterMode = SelectedPlotterMode;

            /*
             * Some values will be overwritten in Graphic2GCodeRelated - GetValuesFromToolList  1869
             * */
            if (SelectedDevice == DeviceSelection.Laser)
            {
                Gcode.Spindle.LasermodeEnable = true;
                if (Properties.Settings.Default.DeviceLaserZEnable)
                {
                    Gcode.OptionZAxis.Enable = true;
                }
                if (useInkFromTablet || useDepthFromWidth)
                {
                    DepthFromWidth.SMin = (float)Properties.Settings.Default.DeviceLaserPowerMin;// Properties.Settings.Default.importGCPWMZero;
                    DepthFromWidth.SMax = (float)Properties.Settings.Default.DeviceLaserPower;
                    DepthFromWidth.SEnable = true;
                }
                Logger.Trace("OverideGcodeSetup Laser");
            }

            else if (SelectedDevice == DeviceSelection.Plotter)
            {
                Gcode.Spindle.LasermodeEnable = false;
                Gcode.OptionZAxis.PreventSpindle = true;

                if (SelectedPlotterMode == 0)               // servo enable
                {
                    Gcode.OptionPWM.Enable = true;
                    if (useInkFromTablet || useDepthFromWidth)
                    {
                        DepthFromWidth.SMin = (float)Properties.Settings.Default.importGCPWMZero;
                        DepthFromWidth.SMax = (float)Properties.Settings.Default.importGCPWMDown;
                        DepthFromWidth.SEnable = true;
                    }
                }
                else                                        //Z-axis
                {
                    Gcode.OptionZAxis.Enable = true;
                    if (useInkFromTablet || useDepthFromWidth)
                    {
                        DepthFromWidth.ZMin = (float)0;// Properties.Settings.Default.importGCPWMZero;
                        DepthFromWidth.ZMax = (float)Properties.Settings.Default.DevicePlotterZDown;
                    }
                }

                Logger.Trace("OverideGcodeSetup Plotter mode:{0}  Z:{1}  PWM:{2}", SelectedPlotterMode, Gcode.OptionZAxis.Enable, Gcode.OptionPWM.Enable);
                if (Properties.Settings.Default.DevicePlotterPenChangeRBManual)
                {
                    Gcode.Control.ToolChangeAddCommand = true;
                    Gcode.Control.ToolChangeM0Enable = true;
                }
                else if (Properties.Settings.Default.DevicePlotterPenChangeRBAutomatic)
                {
                    Gcode.Control.ToolChangeAddCommand = true;
                    Properties.Settings.Default.ctrlToolChange = true;
                }
                Gcode.ModificationSegmentation.Enable = Properties.Settings.Default.importGCLineSegmentation;
            }

            else if (SelectedDevice == DeviceSelection.Router)
            {
                Gcode.OptionZAxis.Enable = true;
                Gcode.ModificationTangential.Enable = Properties.Settings.Default.importGCTangentialEnable;
                if (useInkFromTablet || useDepthFromWidth)
                {
                    DepthFromWidth.ZMin = (float)0;// Properties.Settings.Default.importGCPWMZero;
                    DepthFromWidth.ZMax = (float)Properties.Settings.Default.DeviceRouterZDown;
                }
            }
        }

        internal void SetLabelSave(Label l, String s)
        {
            if (l.InvokeRequired)
            { l.BeginInvoke((MethodInvoker)delegate () { l.Text = s; }); }
            else
            { l.Text = s; }
        }
        internal void SetLabelSave(Label l, String s, Color c)
        {
            if (l.InvokeRequired)
            { l.BeginInvoke((MethodInvoker)delegate () { l.Text = s; l.BackColor = c; l.ForeColor = Colors.ContrastColor(c); }); }
            else
            { l.Text = s; l.BackColor = c; l.ForeColor = Colors.ContrastColor(c); }
        }

        /*
https://stackoverflow.com/questions/61145347/c-how-to-make-a-dark-mode-theme-in-windows-forms-separate-form-as-select-the
		https://psycodedeveloper.wordpress.com/2013/01/18/implementing-your-own-colour-themes-in-a-c-windows-forms-application/
		https://github.com/BlueMystical/Dark-Mode-Forms
        */
        internal static void SetColordesign(Color panelBackColor, Color buttonBackColor)
        {
            Color panelForeColor = Colors.ContrastColor(panelBackColor);
            Color buttonForeColor = Colors.ContrastColor(buttonBackColor);
            MyControl.PanelBackColor = panelBackColor;
            MyControl.PanelForeColor = panelForeColor;
            MyControl.ButtonBackColor = buttonBackColor;
            MyControl.ButtonForeColor = buttonForeColor;

            Properties.Settings.Default.guiColorThemePanel = panelBackColor;
            Properties.Settings.Default.guiColorThemeButton = buttonBackColor;
        }
        internal static void TriggerColorTheme(object sender)
        {
            RaiseGuiControlEvent(sender, new UserControlGuiControlEventArgs(GuiControl.guiUpdate, 99));
        }

        public static void EnableButtons(System.Windows.Forms.Control ctrl, bool enable)
        {
            foreach (System.Windows.Forms.Control l in ctrl.Controls)
            {
                if (l is Button)
                {
                    l.Enabled = enable;
                }
                else
                { EnableButtons(l, enable); }
            }
        }


        // Reset all the controls to the user's default Control color. 
        public static void ChangeColor(System.Windows.Forms.Control ctrl)
        {
            //   ctrl.BackColor = SystemColors.Control;
            //   ctrl.ForeColor = SystemColors.ControlText;
            if (ctrl is Label)
                ctrl.ForeColor = PanelForeColor;
            else if ((ctrl is Button) || (ctrl is CheckBox))
            {
                bool en = ctrl.Enabled;
                ctrl.BackColor = ButtonBackColor;
                ctrl.ForeColor = ButtonForeColor;
                ctrl.Enabled = en;
            }
            else
            {
                ctrl.BackColor = PanelBackColor;
                ctrl.ForeColor = PanelForeColor;
            }
            if (ctrl.HasChildren)
            {
                // Recursively call this method for each child control.
                foreach (System.Windows.Forms.Control childControl in ctrl.Controls)
                {
                    ChangeColor(childControl);
                }
            }
        }

        public static void ChangeColor(System.Windows.Forms.Control ctrl, Color back, Color fore)
        {
            //    if (ctrl is !NumericUpDown)
            {
                ctrl.BackColor = back;
                ctrl.ForeColor = fore;
            }
            if (ctrl.HasChildren)
            {
                // Recursively call this method for each child control.
                foreach (System.Windows.Forms.Control childControl in ctrl.Controls)
                {
                    ChangeColor(childControl, back, fore);
                }
            }
        }

        public static Image BitmapReplaceColor(Image BmpI, Color cnew)
        {
            Bitmap Bmp = new Bitmap(BmpI);
            int centerX = Bmp.Width / 2;
            int centerY = Bmp.Height / 2;
            Color cold = Bmp.GetPixel(centerX, centerY);

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    if (Bmp.GetPixel(x, y) == cold)
                    { Bmp.SetPixel(x, y, cnew); }
                }
            return Bmp;
        }

        public static Image BitmapSetToGray(Image BmpI)
        {
            Bitmap Bmp = new Bitmap(BmpI);
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    if (c.A > 128)
                    {
                        rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                        /*    else	// keep transpaency
                                rgb = 250;*/
                        Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                    }
                }
            return Bmp;
        }

        internal static void SetSetupBtnAppearance(Button btn)
        {
            btn.Font = new System.Drawing.Font("Microsoft Sans Serif", DpiScaling * 6.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            int w = (int)(DpiScaling * 20);
            btn.Size = new System.Drawing.Size(w, w);
            btn.Text = "";// "⚙";
            btn.UseVisualStyleBackColor = true;
            btn.BackgroundImage = Properties.Resources.setup;
            btn.BackgroundImageLayout = ImageLayout.Zoom;
            btn.BackColor = NotifyYellow;
        }

        internal static void ShowSimpleSetup(string title, string info, Point pos, List<ControlDefaults> cd) //string[,] settings, params decimal[] nudParameter)
        {
            /*    for (int i = 0; i < cd.Count; i++)
                {
                    var test = Properties.Settings.Default[cd[i].Prop];
                    var t2 = Properties.Settings.Default[cd[i].Prop];
                    Type GetStaticType<T>(T x) => typeof(T);
                    NLog.LogManager.GetCurrentClassLogger().Trace("ShowSimpleSetup {0}  type:{1}  string:{2}", cd[i].Prop, test, (t2 is string));//GetStaticType(test));
                }*/

            using (Form form = new Form())
            {
                int posX = (int)(DpiScaling * 10);
                int posY = (int)(DpiScaling * 5);
                int relY = (int)(DpiScaling * 23);
                int textLen = 20;
                int nudWidth = (int)(DpiScaling * 50);

                form.Text = title;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.WindowState = FormWindowState.Normal;
                form.StartPosition = FormStartPosition.Manual;
                form.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
                //    form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

                CheckBox checkBox;
                RadioButton radioButton;
                NumericUpDown nud;
                TextBox tb;
                Button btn;
                Label lblInfo;
                Label lbl;

                lblInfo = new Label();
                if (info != "")
                {
                    lblInfo.Location = new Point(posX, posY);
                    lblInfo.AutoSize = false;
                    lblInfo.Text = info;
                    lblInfo.Height = 3 * relY;
                    posY += 3 * relY;

                    form.Controls.Add(lblInfo);
                }
                for (int i = 0; i < cd.Count; i++)
                {
                    if (cd[i].Prop.Contains("BtnSend"))
                    {
                        btn = new Button();
                        btn.Location = new Point(posX, posY);
                        btn.AutoSize = true;
                        btn.Text = cd[i].Text;
                        btn.Tag = cd[i].Cmnd;
                        btn.UseVisualStyleBackColor = true;
                        btn.Click += Btn_Click;
                        /*   btn.Click += (sender, e) =>
                           {
                               OnRaiseCmdEvent(null, new UserControlCmdEventArgs((string)clickedButton.Tag, 0, sender, e));
                           };*/
                        posY += (int)(1.2 * relY);

                        form.Controls.Add(btn);
                    }
                    else
                    {
                        try
                        { var test = Properties.Settings.Default[cd[i].Prop]; }
                        catch
                        {
                            posY += (int)(DpiScaling * 5); ;
                            lbl = new Label();
                            lbl.Location = new Point(posX, posY);
                            lbl.AutoSize = true;
                            lbl.Text = "# " + cd[i].Prop;
                            if (lbl.Text.Length > textLen - 6)
                                textLen = lbl.Text.Length;
                            posY += relY;

                            form.Controls.Add(lbl);
                            continue;
                        }
                        var property = Properties.Settings.Default[cd[i].Prop];

                        if (property is bool)
                        {
                            if (cd[i].Prop.Contains("RB"))
                            {
                                radioButton = new RadioButton();
                                radioButton.Location = new Point(posX, posY);
                                radioButton.AutoSize = true;
                                radioButton.Name = "rb1";
                                radioButton.Text = cd[i].Text;
                                radioButton.UseVisualStyleBackColor = true;
                                radioButton.Checked = (bool)property;// Properties.ListSettings.Default[settings[deviceNr, 1]];//.ctrlSendStopJog;
                                radioButton.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, cd[i].Prop, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                                radioButton.Click += RB_Click;
                                if (cd[i].Text.Length > textLen)
                                    textLen = cd[i].Text.Length;
                                posY += relY;
                                form.Controls.Add(radioButton);
                            }
                            else
                            {
                                checkBox = new CheckBox();
                                checkBox.Location = new Point(posX, posY);
                                checkBox.AutoSize = true;
                                //	checkBox.AutoScaleMode = AutoScaleMode.None;	//AutoScaleMode.Dpi;

                                checkBox.Text = cd[i].Text;
                                checkBox.UseVisualStyleBackColor = true;
                                checkBox.Checked = (bool)property;// Properties.ListSettings.Default[settings[deviceNr, 1]];//.ctrlSendStopJog;
                                checkBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GrblPlotter.Properties.Settings.Default, cd[i].Prop, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                                if (cd[i].Text.Length > textLen)
                                    textLen = cd[i].Text.Length;
                                posY += relY;
                                form.Controls.Add(checkBox);
                            }
                        }
                        else if (property is string)
                        {
                            tb = new TextBox();
                            tb.Location = new Point(posX, posY - (int)(DpiScaling * 2));
                            int f = (int)(((string)property).Length * 6.4);
                            tb.Width = Math.Max(1 * nudWidth, f);
                            tb.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GrblPlotter.Properties.Settings.Default, cd[i].Prop, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                            tb.Text = (string)property;
                            if (((string)property).Length > textLen)
                                textLen = ((string)property).Length;
                            form.Controls.Add(tb);

                            if (cd[i].Prop.Contains("ctrlToolScript") || cd[i].Prop.Contains("importGCSubroutine"))
                            {
                                btn = new Button();
                                btn.Location = new Point(posX + tb.Width, posY - 3);
                                btn.AutoSize = true;
                                btn.Text = cd[i].Text;
                                btn.Tag = cd[i].Prop;
                                btn.UseVisualStyleBackColor = true;
                                btn.Click += SetFilePath_Click;
                                int tl = (cd[i].Text.Length) + ((string)property).Length;
                                if (tl > textLen - 6)
                                    textLen = tl;
                                posY += (int)(1.2 * relY);
                                form.Controls.Add(btn);
                            }
                            else
                            {
                                lbl = new Label();
                                lbl.Location = new Point(posX + tb.Width, posY + 2);
                                lbl.AutoSize = true;
                                lbl.Text = cd[i].Text;
                                int tl = (cd[i].Text.Length) + ((string)property).Length;
                                lbl.Scale(DpiScaling);
                                if (tl > textLen - 6)
                                    textLen = tl;
                                posY += relY;
                                form.Controls.Add(lbl);
                            }
                        }
                        else if ((property is int) || (property is decimal))
                        {
                            nud = new NumericUpDown();
                            nud.Location = new Point(posX, posY - (int)(DpiScaling * 2));
                            nud.Width = nudWidth;
                            nud.TextAlign = HorizontalAlignment.Right;
                            nud.AutoScaleMode = AutoScaleMode.None; //AutoScaleMode.Dpi;

                            if (cd[i].Set.Length >= 3)
                            {
                                nud.Value = nud.Minimum = cd[i].Set[0];
                                nud.Maximum = cd[i].Set[1];
                                nud.Increment = cd[i].Set[2];
                                nud.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GrblPlotter.Properties.Settings.Default, cd[i].Prop, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                            }
                            if (cd[i].Set.Length >= 4)
                            {
                                nud.DecimalPlaces = (int)cd[i].Set[3];
                            }
                            lbl = new Label();
                            lbl.Location = new Point(posX + nudWidth + 3, posY + 2);
                            lbl.AutoSize = true;
                            lbl.Text = cd[i].Text;
                            lbl.Scale(DpiScaling);
                            if (cd[i].Text.Length > textLen - 6)
                                textLen = cd[i].Text.Length;
                            posY += relY;

                            form.Controls.Add(nud);
                            form.Controls.Add(lbl);
                        }
                    }
                }
                textLen = Math.Max(textLen, (info.Length + 1) / 4);
                form.Size = new Size((int)(DpiScaling * (30 + 8 * textLen)), posY += (int)(2.5 * relY));    //(int)(DpiScaling * (50 + 20 * cd.Count)));
                if (pos.X > form.Size.Width)
                    pos.X -= form.Size.Width - (int)(DpiScaling * 20);
                if (pos.Y > DpiScaling * 20)
                    pos.Y -= (int)(DpiScaling * 20);

                lblInfo.Width = form.Width - 15;
                form.Location = pos;
                form.ShowDialog();
            }
        }
        internal static event EventHandler<UserControlCmdEventArgs> RaiseCmdEvent = delegate { };
        internal static event EventHandler<UserControlGuiControlEventArgs> RaiseGuiControlEvent = delegate { };

        private static void Btn_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            RaiseCmdEvent(null, new UserControlCmdEventArgs((string)clickedButton.Tag, 0, sender, e));
        }
        private static void RB_Click(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null && !rb.Checked)
            {
                rb.Checked = !rb.Checked;
            }
        }

        internal static void TestSimpleSetup(string title, Point pos, ref OptionPropHatchFill obj, List<ControlDefaults> cd)
        {
            using (Form form = new Form())
            {
                int posX = (int)(DpiScaling * 10);
                int posY = (int)(DpiScaling * 5);
                int relY = (int)(DpiScaling * 22);
                int textLen = 20;
                int nudWidth = (int)(DpiScaling * 50);

                form.Text = title;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.WindowState = FormWindowState.Normal;
                form.StartPosition = FormStartPosition.Manual;
                form.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
                //    form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

                CheckBox checkBox;
                NumericUpDown nud;
                TextBox tb;
                Button btn;
                Label lbl;
                for (int i = 0; i < cd.Count; i++)
                {
                    if (cd[i].Prop.Contains("BtnSend"))
                    {
                        btn = new Button();
                        btn.Location = new Point(posX, posY);
                        btn.AutoSize = true;
                        btn.Text = cd[i].Text;
                        btn.Tag = cd[i].Cmnd;
                        btn.UseVisualStyleBackColor = true;
                        btn.Click += Btn_Click;
                        posY += (int)(1.2 * relY);
                        form.Controls.Add(btn);
                    }
                    else
                    {
                        try
                        { var test = obj[cd[i].Prop]; }
                        catch
                        {
                            posY += (int)(DpiScaling * 5); ;
                            lbl = new Label();
                            lbl.Location = new Point(posX, posY);
                            lbl.AutoSize = true;
                            lbl.Text = "# " + cd[i].Prop;
                            if (lbl.Text.Length > textLen - 6)
                                textLen = lbl.Text.Length;
                            posY += relY;

                            form.Controls.Add(lbl);
                            continue;
                        }
                        var property = obj[cd[i].Prop];
                        if (property is bool)
                        {
                            checkBox = new CheckBox
                            {
                                Location = new Point(posX, posY),
                                AutoSize = true,
                                Text = cd[i].Text,
                                UseVisualStyleBackColor = true,
                                Checked = (bool)property,
                            };
                            checkBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", obj, cd[i].Prop, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                            if (cd[i].Text.Length > textLen)
                                textLen = cd[i].Text.Length;
                            posY += relY;

                            form.Controls.Add(checkBox);
                        }
                        else if (property is string)
                        {
                            tb = new TextBox();
                            tb.Location = new Point(posX, posY - (int)(DpiScaling * 2));
                            tb.Width = 2 * nudWidth;
                            tb.DataBindings.Add(new System.Windows.Forms.Binding("Text", obj, cd[i].Prop, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

                            lbl = new Label();
                            lbl.Location = new Point(posX + 2 * nudWidth, posY + 2);
                            lbl.AutoSize = true;
                            lbl.Text = cd[i].Text;

                            if (cd[i].Text.Length > textLen - 6)
                                textLen = cd[i].Text.Length;
                            posY += relY;

                            form.Controls.Add(tb);
                            form.Controls.Add(lbl);
                        }
                        else if ((property is int) || (property is float) || (property is double) || (property is decimal))
                        {
                            nud = new NumericUpDown();
                            nud.Location = new Point(posX, posY - (int)(DpiScaling * 2));
                            nud.Width = nudWidth;
                            nud.TextAlign = HorizontalAlignment.Right;
                            nud.AutoScaleMode = AutoScaleMode.None; //AutoScaleMode.Dpi;

                            if (cd[i].Set.Length >= 3)
                            {
                                nud.Value = nud.Minimum = cd[i].Set[0];
                                nud.Maximum = cd[i].Set[1];
                                nud.Increment = cd[i].Set[2];
                                nud.DataBindings.Add(new System.Windows.Forms.Binding("Value", obj, cd[i].Prop, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
                            }
                            if (cd[i].Set.Length >= 4)
                            {
                                nud.DecimalPlaces = (int)cd[i].Set[3];
                            }
                            lbl = new Label();
                            lbl.Location = new Point(posX + nudWidth + 3, posY + 2);
                            lbl.AutoSize = true;
                            lbl.Text = cd[i].Text;
                            if (cd[i].Text.Length > textLen - 6)
                                textLen = cd[i].Text.Length;
                            posY += relY;

                            form.Controls.Add(nud);
                            form.Controls.Add(lbl);
                        }
                    }
                }
                form.Size = new Size((int)(DpiScaling * (30 + 8 * textLen)), posY += (int)(2.5 * relY));    //(int)(DpiScaling * (50 + 20 * cd.Count)));
                if (pos.X > form.Size.Width)
                    pos.X -= form.Size.Width - (int)(DpiScaling * 20);
                if (pos.Y > DpiScaling * 20)
                    pos.Y -= (int)(DpiScaling * 20);
                form.Location = pos;
                form.ShowDialog();
            }
        }


        private static void SetFilePath_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            SetFilePath((string)clickedButton.Tag);
        }
        private static void SetFilePath(string prop, string filter = "GCode (*.nc)|*.nc|All Files (*.*)|*.*")
        {
            string file = (string)Properties.Settings.Default[prop];
            OpenFileDialog opnDlg = new OpenFileDialog();
            string ipath = Datapath.MakeAbsolutePath(file);
            //    if (log) Logger.Info("SetFilePath initiial: box:{0}   makeAbsolute:{1}", tmp.Text, ipath);
            opnDlg.InitialDirectory = ipath.Substring(0, ipath.LastIndexOf("\\"));
            opnDlg.Filter = filter;  //"GCode (*.nc)|*.nc|All Files (*.*)|*.*";
            if (opnDlg.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default[prop] = opnDlg.FileName;
            }
            opnDlg.Dispose();
        }
    }

    /* Handle button clicks from UserControls in main window */
    public class UserControlCmdEventArgs : EventArgs
    {
        private readonly string _command;
        private readonly byte _realtime;
        private readonly object _sender;
        private readonly EventArgs _e;
        public UserControlCmdEventArgs(string cmd, byte realt, object sender, EventArgs e)
        {
            _command = cmd;
            _realtime = realt;
            _sender = sender;
            _e = e;
        }
        public string Command
        { get { return _command; } }
        public byte Realtime
        { get { return _realtime; } }
        public object Sender
        { get { return _sender; } }
        public EventArgs e
        { get { return _e; } }
    }
    public class UserControlGuiControlEventArgs : EventArgs
    {
        private readonly GuiControl _gc;
        private readonly int _intVal;
        public UserControlGuiControlEventArgs(GuiControl gc, int i = 0)
        {
            _gc = gc;
            _intVal = i;
        }
        public GuiControl Gc
        { get { return _gc; } }
        public int IntVal
        { get { return _intVal; } }
    }
    public class UserControlTransformEventArgs : EventArgs
    {
        private readonly double _x;
        private readonly double _y;
        private readonly int _nr;

        private readonly object _sender;
        private readonly EventArgs _e;
        public UserControlTransformEventArgs(double x, double y, int nr, object sender, EventArgs e)
        {
            _x = x;
            _y = y;
            _nr = nr;
            _sender = sender;
            _e = e;
        }
        public double OffsetX
        { get { return _x; } }
        public double OffsetY
        { get { return _y; } }
        public int Nr
        { get { return _nr; } }
        public object Sender
        { get { return _sender; } }
        public EventArgs e
        { get { return _e; } }
    }



    //using System.Windows.Forms;
    //using System.Drawing;
    //using System.Drawing.Drawing2D;
    //using System.ComponentModel;

    namespace Styling_Toggle_Button
    {
        public class SButton : CheckBox
        {
            //Fields
            private Color onBackColor = Color.MediumSlateBlue;
            private Color onToggleColor = Color.WhiteSmoke;
            private Color offBackColor = Color.Gray;
            private Color offToggleColor = Color.Gainsboro;
            private bool solidStyle = true;

            //Properties
            public Color OnBackColor
            {
                get { return onBackColor; }
                set
                {
                    onBackColor = value;
                    this.Invalidate();
                }
            }

            public Color OnToggleColor
            {
                get { return onToggleColor; }
                set
                {
                    onToggleColor = value;
                    this.Invalidate();
                }
            }
            public Color OffBackColor
            {
                get { return offBackColor; }
                set
                {
                    offBackColor = value;
                    this.Invalidate();
                }
            }
            public Color OffToggleColor
            {
                get { return offToggleColor; }
                set
                {
                    offToggleColor = value;
                    this.Invalidate();
                }
            }

            /*     [Browsable(false)]
                 public override string Text
                 {
                     get { return base.Text; }
                     set { }
                 }*/
            [DefaultValue(true)]
            public bool SolidStyle
            {
                get { return solidStyle; }
                set
                {
                    solidStyle = value;
                    this.Invalidate();
                }
            }
            //Constructor
            public SButton()
            {
                this.MinimumSize = new Size(24, 18);
            }
            //Methods
            private GraphicsPath GetFigurePath()
            {
                int arcSize = this.Height - 1;
                Rectangle leftArc = new Rectangle(0, 0, arcSize, arcSize);
                Rectangle rightArc = new Rectangle(this.Width - arcSize - 2, 0, arcSize, arcSize);

                GraphicsPath path = new GraphicsPath();
                //      path.StartFigure();
                path.AddArc(leftArc, 90, 180);
                path.AddArc(rightArc, 270, 180);
                path.CloseFigure();

                return path;
            }
            protected override void OnPaint(PaintEventArgs pevent)
            {
                //   base.OnPaint(pevent);
                int toggleSize = this.Height - 5;
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                //      pevent.Graphics.Clear(this.Parent.BackColor);

                if (this.Checked) //ON
                {
                    //Draw the control surface
                    if (solidStyle)
                        pevent.Graphics.FillPath(new SolidBrush(onBackColor), GetFigurePath());
                    else pevent.Graphics.DrawPath(new Pen(onBackColor, 2), GetFigurePath());
                    //Draw the toggle
                    pevent.Graphics.FillEllipse(new SolidBrush(onToggleColor),
                     new Rectangle(this.Width - this.Height + 1, 2, toggleSize, toggleSize));
                }
                else //OFF
                {
                    //Draw the control surface
                    if (solidStyle)
                        pevent.Graphics.FillPath(new SolidBrush(offBackColor), GetFigurePath());
                    else pevent.Graphics.DrawPath(new Pen(offBackColor, 2), GetFigurePath());
                    //Draw the toggle
                    pevent.Graphics.FillEllipse(new SolidBrush(offToggleColor),
                     new Rectangle(2, 2, toggleSize, toggleSize));
                }
            }


        }

        public class RoundedButton : Button
        {
            public RoundedButton()
            {
                this.BackColor = Color.OrangeRed;
                this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                this.FlatAppearance.BorderColor = Color.Black;
                this.FlatAppearance.BorderSize = 1;
                this.Width = 140;
                this.Height = 45;
            }
            public int rdus = 30;
            System.Drawing.Drawing2D.GraphicsPath GetRoundPath(RectangleF Rect, int radius)
            {
                float r2 = radius / 2f;
                System.Drawing.Drawing2D.GraphicsPath GraphPath = new System.Drawing.Drawing2D.GraphicsPath();
                GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
                GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
                GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
                GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
                GraphPath.AddArc(Rect.X + Rect.Width - radius,
                    Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
                GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
                GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
                GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
                GraphPath.CloseFigure();
                return GraphPath;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
                using (System.Drawing.Drawing2D.GraphicsPath GraphPath = GetRoundPath(Rect, rdus))
                {
                    this.Region = new Region(GraphPath);
                    using (Pen pen = new Pen(Color.CadetBlue, 1.75f))
                    {
                        pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                        e.Graphics.DrawPath(pen, GraphPath);
                    }
                }
            }

        }
    }




}
