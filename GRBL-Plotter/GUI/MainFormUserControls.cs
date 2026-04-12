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
/* MainFormUserControl
 * 2026-04-09 GUI rework for vers. 1.8.0.0
*/

using GrblPlotter.Helper;
using GrblPlotter.UserControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrblPlotter
{
    public partial class MainForm : Form
    {
        private static UserControl[] Leftside;// = [userControlStreaming, userControlOverrides, userControlRealtime, userControlSetOffset];
        private static UserControl[] Rightside;// = [userControlMoveToGraphic, userControlMoveToZero, userControlJogControlAll];

        private int TC_RouterPlotterLaserLastSelected = 0;
        private void SetInfoLabel(String txt, Color? col = null)
        {
            ucStreaming.SetStatusTextStreaming(txt, col);
        }

        private void UserControlsInitialize()
        {
            ucStreaming.RaiseGuiControlEvent += OnRaiseGuiControlEvent;
            ucOverrides.RaiseCmdEvent += OnRaiseCmdEvent;
            ucFlowControl.RaiseCmdEvent += OnRaiseCmdEvent;
            ucdro.RaiseCmdEvent += OnRaiseCmdEvent;

            ucMoveToGraphic.RaiseCmdEvent += OnRaiseCmdEvent;
            ucMoveToGraphic.RaiseGuiControlEvent += OnRaiseGuiControlEvent;
            ucSetCoordinateSystem.RaiseCmdEvent += OnRaiseCmdEvent;
            ucMoveToZero.RaiseCmdEvent += OnRaiseCmdEvent;
            ucJogControlAll.RaiseCmdEvent += OnRaiseCmdEvent;
            ucJogControlAll.RaiseGuiControlEvent += OnRaiseGuiControlEvent;
            ucSetOffset.RaiseTransformEvent += OnRaiseTransformEvent;
            MyControl.RaiseCmdEvent += OnRaiseCmdEvent;
            MyControl.RaiseGuiControlEvent += OnRaiseGuiControlEvent;

            ucDeviceLaser.RaiseGuiControlEvent += OnRaiseGuiControlEvent;
            ucDeviceLaser2.RaiseCmdEvent += OnRaiseCmdEvent;
            ucDevicePlotter.RaiseCmdEvent += OnRaiseCmdEvent;
            ucDevicePlotter2.RaiseCmdEvent += OnRaiseCmdEvent;
            ucDeviceRouter.RaiseGuiControlEvent += OnRaiseGuiControlEvent;
            ucDeviceRouter2.RaiseCmdEvent += OnRaiseCmdEvent;

            ucToolList.RaiseGuiControlEvent += OnRaiseGuiControlEvent;
            //     _setup_form.RaiseGuiControlEvent += OnRaiseGuiControlEvent;

            MyControl.NotifyYellow = Color.Yellow;
            MyControl.NotifyGreen = Color.LightGreen;
            MyControl.NotifyRed = Color.Orange;
            MyControl.ButtonActive = Color.Lime;
            MyControl.ButtonInactive = Color.LightGray;
            MyControl.PanelHighlight = Color.FromArgb(255, 255, 128);
        }

        private void UserControlsMainFormLoad()
        {
            Logger.Trace("############################### UserControlsMainFormLoad ");
            UserControlsSetAxisCount(3, CommandProtocol.grblOld);
            SetTableLayoutPanelLeftSizes();
            ucDevicePlotter.TcServoZAxis.SelectedIndexChanged += TC_RouterPlotterLaser_SelectedIndexChanged;
            TC_RouterPlotterLaser_SelectedIndexChanged(null, null);
            MyControl.GraphicImported = false;
            TC_RouterPlotterLaserLastSelected = tC_RouterPlotterLaser.SelectedIndex;

            if (Properties.Settings.Default.guiColorThemeEnable)
            {
                MyControl.SetColordesign(Properties.Settings.Default.guiColorThemePanel, Properties.Settings.Default.guiColorThemeButton);
                UserControlSetColors();
            }
            else
            {
                MyControl.SetColordesign(Color.WhiteSmoke, Color.Pink);
            }
        }

        public void UserControlSetColors()
        {
            /* set colors via HSV dialog in Setup - Program appearance - Misc - Color theme 
			 * scroll event convert colors and save as property, sends event    RaiseGuiControlEvent(null, new UserControlGuiControlEventArgs(GuiControl.guiUpdate, 99));
			 * received here line 276....  
			 */

            MyControl.ChangeColor(this);
            showFormsToolStripMenuItem.BackColor = MyControl.PanelHighlight;
            showFormsToolStripMenuItem.ForeColor = Colors.ContrastColor(MyControl.PanelHighlight);

            /* Left */
            ucStreaming.RestoreColors();
            ucOverrides.RestoreColors();
            ucFlowControl.RestoreColors();
            ucSetOffset.RestoreColors();

            /* Center */
            ucdro.RestoreColors();
            ucToolList.RestoreColors();
            ucDeviceLaser.RestoreColors();
            ucDevicePlotter.RestoreColors();
            ucDeviceRouter.RestoreColors();

            /* Right */
            ucDeviceLaser2.RestoreColors();
            ucMoveToGraphic.RestoreColors();
            ucSetCoordinateSystem.RestoreColors();
            ucMoveToZero.RestoreColors();
            ucJogControlAll.RestoreColors();
        }

        private void SetTableLayoutPanelLeftSizes()
        {
            TableLayoutRowStyleCollection styles = this.tLPLinks.RowStyles;
            styles[0].SizeType = SizeType.AutoSize;
            styles[1].SizeType = SizeType.AutoSize;
            styles[2].SizeType = SizeType.AutoSize;
            styles[3].SizeType = SizeType.AutoSize;

            splitContainer1.Panel1MinSize = (int)(DpiScaling * 308);
        }

        private void ResizeRightSide(string src)
        {
            int w = ucJogControlAll.GetControlWidth();
            w = Math.Max(w, ucMoveToGraphic.GetControlWidth());

            Logger.Trace("ResizeRightSide {0}  {1}", w, src);

            tLPRechtsUntenRechts.Width = w;
            tC_RouterPlotterLaser2.Width = w;
            ucMoveToGraphic.Width = w;
            ucSetCoordinateSystem.Width = w;
            ucMoveToZero.Width = w;
            ucJogControlAll.Width = w;
            ucdro.MainWidth = this.Width;
        }

        private void UserControlsSetAxisCount(int nr, CommandProtocol cp)
        {
            Logger.Trace("UserControlsSetAxisCount  axis:{0}  protocol:{1}", nr, cp);
            ucdro.SetAxisCount(nr, cp);
            ucMoveToZero.SetAxisCount(nr, cp);
            ucJogControlAll.SetAxisCount(nr, cp);

            int LayoutWidth = (int)(DpiScaling * 180);
            if (nr < 4) LayoutWidth = (int)(DpiScaling * 190);
            if (nr == 4) LayoutWidth = (int)(DpiScaling * 225);
            if (nr == 5) LayoutWidth = (int)(DpiScaling * 260);
            if (nr == 6) LayoutWidth = (int)(DpiScaling * 295);

            TableLayoutColumnStyleCollection styles = this.tLPRechtsUntenRechts.ColumnStyles;
            styles[0].SizeType = SizeType.Absolute;
            styles[0].Width = LayoutWidth;
        }

        private void OnRaiseTransformEvent(object sender, UserControlTransformEventArgs e)
        {
            if (isStreaming)
                return;
            if (fCTBCode.Lines.Count > 1)
            {
                TransformStart("Apply Offset");
                zoomFactor = 1;
                fCTBCode.Text = VisuGCode.TransformGCodeOffset(-e.OffsetX, -e.OffsetY, VisuGCode.GetTranslate(e.Nr));
                fCTBCodeClickedLineNow = fCTBCodeClickedLineLast;
                fCTBCodeClickedLineLast = 0;
                TransformEnd();
            }
            Cursor.Current = Cursors.Default;
        }

        private void OnRaiseCmdEvent(object sender, UserControlCmdEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Command) && (e.Realtime != 0))   // send realtime command
            { SendRealtimeCommand(e.Realtime); }
            else                                                // send regular command
            {
                if ((_serial_form != null) && (!_serial_form.RequestSend(e.Command, true)))     // check if COM is still open
                { }
            }
        }

        private void OnRaiseGuiControlEvent(object sender, UserControlGuiControlEventArgs e)
        {
            if (e.Gc == GuiControl.invalidate)
            { this.Invalidate(); }
            else if (e.Gc == GuiControl.streamStart)
            {
                UpdateLogging();
                StartStreaming(0, fCTBCode.LinesCount - 1);
            }
            else if (e.Gc == GuiControl.streamStartSection)
            {
                UpdateLogging();
                StreamSection();
            }

            else if (e.Gc == GuiControl.streamStop)
            { StopStreaming(true); UpdateLogging(); }
            else if (e.Gc == GuiControl.streamCheck)
            {
                if ((fCTBCode.LinesCount > 1) && (!isStreaming))
                {
                    ClearErrorLines();
                    Logger.Info("check code");
                    StatusStripSet(0, Localization.GetString("statusStripeStreamingCheck"), Color.Lime);        // Check G-Code on grbl controller
                    EventCollector.SetStreaming("Schk");
                    isStreaming = true;
                    isStreamingCheck = true;
                    isStreamingOk = true;
                    timerUpdateControlSource = "btnStreamCheck_Click";
                    UpdateControlEnables();
                    timeInit = DateTime.UtcNow;
                    elapsed = TimeSpan.Zero;
                    //      SetTextThreadSave(lbInfo, Localization.GetString("mainInfoCheckCode"), SystemColors.Control);
                    SetInfoLabel(Localization.GetString("mainInfoCheckCode"), SystemColors.Control);
                    fCTBCode.Bookmarks.Clear();
                    _serial_form.StartStreaming(fCTBCode.Lines, 0, fCTBCode.LinesCount - 1, true);
                    //         btnStreamStart.Enable = false;
                    OnPaint_setBackground();
                }
            }
            else if (e.Gc == GuiControl.simulateStart)
            {
                if ((!isStreaming) && (fCTBCode.LinesCount > 2))
                {
                    codeInfo = new XyzPoint();  // reset old positions
                    ucStreaming.SetStatusSimulationStart(!simuEnabled);
                    if (!simuEnabled)
                    { SimuStart(Properties.Settings.Default.gui2DColorSimulation); }
                    else
                    { SimuStop(); }
                }
            }
            else if (e.Gc == GuiControl.simulatePause)
            {
                bool tmp = simulationTimer.Enabled;
                simulationTimer.Enabled = !tmp;
                ucStreaming.SetStatusSimulationPause(tmp);
            }
            else if (e.Gc == GuiControl.simulateFaster)
            {
                VisuGCode.Simulation.speedFactor *= 2;
                VisuGCode.Simulation.dt *= 2;
                VisuGCode.Simulation.dwell_ms /= 2;
                if (VisuGCode.Simulation.dt == 32) VisuGCode.Simulation.dt = 25;
                if (VisuGCode.Simulation.dt > 102400)
                    VisuGCode.Simulation.dt = 102400;
                ucStreaming.SetTextTime(string.Format("{0} {1:0}%", Localization.GetString("mainSimuSpeed"), VisuGCode.Simulation.speedFactor));
            }
            else if (e.Gc == GuiControl.simulateSlower)
            {
                VisuGCode.Simulation.speedFactor /= 2;
                VisuGCode.Simulation.dt /= 2;
                VisuGCode.Simulation.dwell_ms *= 2;
                if (VisuGCode.Simulation.dt == 12) VisuGCode.Simulation.dt = 16;
                if (VisuGCode.Simulation.dt <= 1)
                    VisuGCode.Simulation.dt = 1;
                ucStreaming.SetTextTime(string.Format("{0} {1:0}%", Localization.GetString("mainSimuSpeed"), VisuGCode.Simulation.speedFactor));
            }
            else if (e.Gc == GuiControl.guiUpdate)
            {
                ResizeRightSide(string.Format("OnRaiseGuiControlEvent  {0}", sender.ToString()));
                if (e.IntVal == 13) // generate image data
                {
                    tC_RouterPlotterLaser.SelectedIndex = tC_RouterPlotterLaser.TabCount - 1;
                    //    MyControl.SetSelectedDevice(tC_RouterPlotterLaser.TabCount - 1);
                }
                if (e.IntVal == 99)
                {
                    UserControlSetColors();
                }
            }
            else if (e.Gc == GuiControl.highligh)
            {
                if (XmlMarker.GetGroupCount() > 0)                                                      // is Group-Tag present
                {
                //   Logger.Trace("Highlight clicked {0}", e.IntVal);
                    bool gcodeIsSeleced = false;
                    EnableBlockCommands(gcodeIsSeleced);
                    int clickedLine = XmlMarker.GetStartLineOfGroup(e.IntVal);
                    if (XmlMarker.GetGroup(clickedLine) && LineIsInRange(XmlMarker.lastGroup.LineStart))// is Group-Tag valid
                    {
                        SetTextSelection(XmlMarker.lastGroup.LineStart, XmlMarker.lastGroup.LineEnd);   // select Gcode

                        if (SelectionHandle.SelectedGroup != XmlMarker.lastGroup.Id)
                        {
                            VisuGCode.MarkSelectedGroup(XmlMarker.lastGroup.LineStart);                 // highlight 2D-view
                            SelectionHandle.SelectedMarkerLine = clickedLine;
                            SelectionHandle.SelectedGroup = XmlMarker.lastGroup.Id;
                            if (_setup_form != null)
                            {
                                _setup_form.LastMarkedColor = XmlMarker.lastGroup.PenColor;
                                _setup_form.LastMarkedWidth = Math.Round(XmlMarker.lastGroup.PenWidth, 3).ToString().Replace(',', '.');
                                Logger.Trace("FCTB GetGroup color:'{0}' width:'{1}'", XmlMarker.lastGroup.PenColor, XmlMarker.lastGroup.PenWidth);
                            }
                            gcodeIsSeleced = true;
                        }
                        else
                        {
                            VisuGCode.MarkSelectedFigure(-3);
                            SelectionHandle.ClearSelected();
                            ClearTextSelection();
                        }

                        Color highlight = Color.GreenYellow;
                        StatusStripSet(2, string.Format("Marked: {0}", fCTBCode.Lines[XmlMarker.lastGroup.LineStart]), highlight);
                        pictureBox1.Invalidate();
                    }
                    fCTBCodeClickedLineNow = XmlMarker.lastGroup.LineStart;
                    EnableBlockCommands(gcodeIsSeleced, true);                                            // enable CMS menu

                    if (!gcodeIsSeleced)
                        StatusStripClear(2);
                }
            }
            else if (e.Gc == GuiControl.reloadGraphic)
            {
                ReStartConvertFile(sender, e, -1);	// MainFormLoadFile.cs 1444
            }
            else if (e.Gc == GuiControl.openForm)
            {
                if (e.IntVal == 31)
                { Laseropen(sender, e); }       // laser tools
                if (e.IntVal == 21)
                { EdgeFinderopen(sender, e); }  // probing
                if (e.IntVal == 22)
                { HeightMapToolStripMenuItem_Click(sender, e); }
            }
        }

        private void TC_RouterPlotterLaser_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageCount2 = tC_RouterPlotterLaser2.TabCount;
            MyControl.SetSelectedDevice(tC_RouterPlotterLaser.SelectedIndex);
            if (tC_RouterPlotterLaser.SelectedIndex < pageCount2)
            {
                ucToolList.Visible = true;
                GbCustomButtons.Visible = false;
                tC_RouterPlotterLaser2.SelectedIndex = tC_RouterPlotterLaser.SelectedIndex;
                ucToolList.SwitchView(tC_RouterPlotterLaser.SelectedIndex, MyControl.SelectedPlotterMode);
                ucToolList.ReloadNeded();
                LoadProperties.Off();
                if (tC_RouterPlotterLaser.SelectedIndex == 1)   // Plotter
                {
                    Logger.Trace("Set width:{0}  Split2:{1}   2-width:{2}", splitContainer2.Width,splitContainer2.SplitterDistance, splitContainer2.Panel2.Width);
                    if (splitContainer2.Panel2.Width < 250)
                    {   
                        splitContainer2.SplitterDistance = splitContainer2.Width - 285;
                    }
                }
            }
            else
            {
                ucToolList.Visible = false;
                GbCustomButtons.Visible = true;
                LoadProperties.Init();
            }
            TC_RouterPlotterLaserLastSelected = tC_RouterPlotterLaser.SelectedIndex;
            UpdateForms();
        }
        private void TC_RouterPlotterLaser2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageCount1 = tC_RouterPlotterLaser.TabCount;

            if (tC_RouterPlotterLaser2.SelectedIndex < pageCount1)
            {
                tC_RouterPlotterLaser.SelectedIndex = tC_RouterPlotterLaser2.SelectedIndex;
            }
        }
        private void Tc_RouterPlotterLaser_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tC_RouterPlotterLaser.TabPages[e.Index];
            Color fc = (e.State == DrawItemState.Selected) ? Colors.ContrastColor(MyControl.NotifyYellow) : MyControl.PanelForeColor;
            if (!e.Bounds.IsEmpty)
            {
                e.Graphics.FillRectangle(new SolidBrush((e.State == DrawItemState.Selected) ? MyControl.NotifyYellow : page.BackColor), e.Bounds);
            }
            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, fc); // page.ForeColor);
        }

        internal static bool WithinAxisLimits(double x, double y, bool isAbsolute)
        {
            if (Properties.Settings.Default.machineLimitsAlarm && Properties.Settings.Default.machineLimitsShow)
            {
                if (!Dimensions.WithinLimits(Grbl.posMachine, x, y))
                {
                    decimal minx = Properties.Settings.Default.machineLimitsHomeX;
                    decimal maxx = minx + Properties.Settings.Default.machineLimitsRangeX;
                    decimal miny = Properties.Settings.Default.machineLimitsHomeY;
                    decimal maxy = miny + Properties.Settings.Default.machineLimitsRangeY;

                    string tmp = string.Format(culture, "minX: {0:0.0} moveTo: {1:0.0} maxX: {2:0.0}", minx, (Grbl.posMachine.X + x), maxx);
                    tmp += string.Format(culture, "\r\nminY: {0:0.0} moveTo: {1:0.0} maxY: {2:0.0}", miny, (Grbl.posMachine.Y + y), maxy);
                    System.Media.SystemSounds.Beep.Play();
                    DialogResult dialogResult = MessageBox.Show(Localization.GetString("mainLimits1") + tmp + Localization.GetString("mainLimits2"), Localization.GetString("mainAttention"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dialogResult == DialogResult.Cancel)
                        return false;
                }
            }
            return true;
        }

        internal void UpdateForms()
        {
            _text_form?.SetActiveDevice(TC_RouterPlotterLaserLastSelected);
            _tablet_form?.SetActiveDevice(TC_RouterPlotterLaserLastSelected);
        }
    }
}
